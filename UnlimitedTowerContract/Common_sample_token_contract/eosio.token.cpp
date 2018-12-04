/**
 *  @file
 *  @copyright defined in eos/LICENSE.txt
 */

#include "eosio.token.hpp"

// #undef EOSIO_ABI
// #define EOSIO_ABI( TYPE, MEMBERS ) \
// extern "C" { \
//    void apply( uint64_t receiver, uint64_t code, uint64_t action ) { \
//       auto self = receiver; \
//       TYPE thiscontract( self ); \
//       if(code==N(eosio)) {\
//         print("receiver : ",name{receiver},"\n");\
//       } \
//       if (code == N(eosio)){ \
//           print("code :",name{code},"\n");\
//       } \
//       if (receiver == N(eosio)){ \
//           print("receiver :",name{receiver},"\n");\
//       } \
//       if( action == N(onerror)) { \
//          /* onerror is only valid if it is for the "eosio" code account and authorized by "eosio"'s "active permission */ \
//          eosio_assert(code == N(eosio), "onerror action's are only valid from the \"eosio\" system account"); \
//          print("receiver : ",name{receiver},"\n","code :",name{code},"\n","action : ",name{action},"\n");\
//       } \
//       if( code == self || action == N(onerror) ) { \
//          switch( action ) { \
//             EOSIO_API( TYPE, MEMBERS ) \
//          } \
//          /* does not allow destructor of thiscontract to run: eosio_exit(0); */ \
//       } \
//     }\
// } 
//      execute_action(&thiscontract, &token::apply);\
// / eosio_assert((code != N(eosio) && action != N(setcode)) && (code != N(eosio) && action != N(setabi)), "can't update code");
void token::create(account_name issuer,
                   asset maximum_supply)
{
    require_auth(_self);

    auto sym = maximum_supply.symbol;
    eosio_assert(sym.is_valid(), "invalid symbol name");
    eosio_assert(maximum_supply.is_valid(), "invalid supply");
    eosio_assert(maximum_supply.amount > 0, "max-supply must be positive");

    stats statstable(_self, sym.name());
    auto existing = statstable.find(sym.name());
    eosio_assert(existing == statstable.end(), "token with symbol already exists");

    statstable.emplace(_self, [&](auto &s) {
        s.supply.symbol = maximum_supply.symbol;
        s.max_supply = maximum_supply;
        s.issuer = issuer;
    });
}
void token::apply(uint64_t receiver, uint64_t code, uint64_t action)
{
   print("receiver : ",name{receiver},"\n","code :",name{code},"\n","action : ",name{action},"\n");
   eosio_assert( (code!=N(eosio) && action!=N(setcode)) && (code!=N(eosio) && action!=N(setabi)), "can't update code" );
}
void token::issue(account_name to, asset quantity, string memo)
{
    auto sym = quantity.symbol;
    eosio_assert(sym.is_valid(), "invalid symbol name");
    eosio_assert(memo.size() <= 256, "memo has more than 256 bytes");

    auto sym_name = sym.name();
    stats statstable(_self, sym_name);
    auto existing = statstable.find(sym_name);
    eosio_assert(existing != statstable.end(), "token with symbol does not exist, create token before issue");
    const auto &st = *existing;

    require_auth(st.issuer);
    eosio_assert(quantity.is_valid(), "invalid quantity");
    eosio_assert(quantity.amount > 0, "must issue positive quantity");

    eosio_assert(quantity.symbol == st.supply.symbol, "symbol precision mismatch");
    eosio_assert(quantity.amount <= st.max_supply.amount - st.supply.amount, "quantity exceeds available supply");

    statstable.modify(st, 0, [&](auto &s) {
        s.supply.amount = safemath::add64(s.supply.amount, quantity.amount);
    });

    add_balance(st.issuer, quantity, st.issuer);
    mul_balance(st.issuer, quantity, st.issuer);
    div_balance(st.issuer, quantity, st.issuer);

    if (to != st.issuer)
    {
        SEND_INLINE_ACTION(*this, transfer, {st.issuer, N(active)}, {st.issuer, to, quantity, memo});
    }
}

void token::transfer(account_name from,
                     account_name to,
                     asset quantity,
                     string memo)
{
    eosio_assert(from != to, "cannot transfer to self");
    require_auth(from);
    eosio_assert(is_account(to), "to account does not exist");
    auto sym = quantity.symbol.name();
    stats statstable(_self, sym);
    const auto &st = statstable.get(sym);

    require_recipient(from);
    require_recipient(to);

    eosio_assert(quantity.is_valid(), "invalid quantity");
    eosio_assert(quantity.amount > 0, "must transfer positive quantity");
    eosio_assert(quantity.symbol == st.supply.symbol, "symbol precision mismatch");
    eosio_assert(memo.size() <= 256, "memo has more than 256 bytes");

    sub_balance(from, quantity);
    add_balance(to, quantity, from);
    mul_balance(to, quantity, from);
    div_balance(to, quantity, from);
}

void token::sub_balance(account_name owner, asset value)
{
    accounts from_acnts(_self, owner);

    const auto &from = from_acnts.get(value.symbol.name(), "no balance object found");
    eosio_assert(from.balance.amount >= value.amount, "overdrawn balance");

    if (from.balance.amount == value.amount)
    {
        from_acnts.erase(from);
    }
    else
    {
        from_acnts.modify(from, owner, [&](auto &a) {
            a.balance.symbol = value.symbol;
            print("sub : ", safemath::sub64(a.balance.amount, value.amount), "\n");
            a.balance.amount = safemath::sub64(a.balance.amount, value.amount);
        });
    }
}

void token::add_balance(account_name owner, asset value, account_name ram_payer)
{
    accounts to_acnts(_self, owner);
    auto to = to_acnts.find(value.symbol.name());
    if (to == to_acnts.end())
    {
        to_acnts.emplace(ram_payer, [&](auto &a) {
            a.balance = value;
        });
    }
    else
    {
        to_acnts.modify(to, 0, [&](auto &a) {
            a.balance.symbol = value.symbol;
            print("add : ", safemath::add64(a.balance.amount, value.amount), "\n");
            a.balance.amount = safemath::add64(a.balance.amount, value.amount);
        });
    }
}
void token::mul_balance(account_name owner, asset value, account_name ram_payer)
{
    accounts to_acnts(_self, owner);
    auto to = to_acnts.find(value.symbol.name());
    if (to == to_acnts.end())
    {
        to_acnts.emplace(ram_payer, [&](auto &a) {
            a.balance = value;
        });
    }
    else
    {
        to_acnts.modify(to, 0, [&](auto &a) {
            a.balance.symbol = value.symbol;
            print("mul : ", safemath::mul64(a.balance.amount, value.amount), "\n");
            a.balance.amount = safemath::mul64(a.balance.amount, value.amount);
        });
    }
}

void token::div_balance(account_name owner, asset value, account_name ram_payer)
{
    accounts to_acnts(_self, owner);
    auto to = to_acnts.find(value.symbol.name());
    if (to == to_acnts.end())
    {
        to_acnts.emplace(ram_payer, [&](auto &a) {
            a.balance = value;
        });
    }
    else
    {
        to_acnts.modify(to, 0, [&](auto &a) {
            a.balance.symbol = value.symbol;
            print("div : ", safemath::div64(a.balance.amount, value.amount), "\n");
            a.balance.amount = safemath::div64(a.balance.amount, value.amount);
        });
    }
}
void token::calculate(account_name _user)
{
    print("tapos_block_num : ", tapos_block_num(), "\n");
    print("tapos_block_prefix : ", tapos_block_prefix(), "\n");
    print("now : ", now(), "\n");

    
    uint64_t l_seed = safemath::get_seed(_user);
    print("seed : ",l_seed,"\n");

}

void token::calmul(uint64_t a, uint64_t b)
{
    print("no safe mul : ", a * b, "\n");
    print("mul64 :", safemath::mul64(a, b), "\n");
}

void token::caladd(uint64_t a, uint64_t b)
{
    print("no safe add : ", a + b, "\n");
    print("add64 :", safemath::add64(a, b), "\n");
}

void token::calsub(uint64_t a, uint64_t b)
{
    print("no safe sub : ", a - b, "\n");
    print("sub64 :", safemath::sub64(a, b), "\n");
}

void token::caldiv(uint64_t a, uint64_t b)
{

   print("no safe div : ", a / b, "\n");
    print("div64 :", safemath::div64(a, b), "\n");
}

EOSIO_ABI(token,(create)(apply)(issue)(transfer)(calmul)(caladd)(caldiv)(calsub)(calculate))
