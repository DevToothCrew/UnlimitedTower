
#pragma once
#include "../Common/common_header.hpp"


#pragma resion token system
ACTION unlimited_tower::create(name issuer, asset maximum_supply)
{
    require_auth(_self);

    auto sym = maximum_supply.symbol;
    eosio_assert(sym.is_valid(), "Invalid symbol name");
    eosio_assert(maximum_supply.is_valid(), "Invalid Supply");
    eosio_assert(maximum_supply.amount > 0, "Max-supply must be positive");

    stat statstable(_self, sym.code().raw());
    auto existing = statstable.find(sym.code().raw());
    eosio_assert(existing == statstable.end(), "Tokenwith symbol already exists");

    statstable.emplace(_self, [&](auto &s) {
        s.supply.symbol = maximum_supply.symbol;
        s.max_supply = maximum_supply;
        s.issuer = issuer;
    });
    }

    ACTION unlimited_tower::issue(name to, asset quantity, string memo)
    {
       auto sym = quantity.symbol;
       eosio_assert(sym.is_valid(), "Invalid symbol name");
       eosio_assert(memo.size() <= 256, "Memo has more than 256 bytes");

       uint64_t sym_name = sym.code().raw();
       stat statstable(_self, sym_name);
       auto existing = statstable.find(sym.code().raw());
       eosio_assert(existing != statstable.end(), "Token with symbol does now exist, Create token before issue");
       const auto& st = *existing;

       require_auth(st.issuer);
       eosio_assert(quantity.is_valid(), "Invalid quantity");
       eosio_assert(quantity.amount > 0, "Must issue positive quantity");

       eosio_assert(quantity.symbol == st.supply.symbol, "Symbol precision mismatch");
       eosio_assert(quantity.amount <= st.max_supply.amount - st.supply.amount, "Quantity exceeds available supply");

       statstable.modify(st, _self, [&](auto& s){
           s.supply += quantity;
       });

       add_balance(st.issuer, quantity, st.issuer);

        if (to != st.issuer)
        {
            action(permission_level{st.issuer, "active"_n},
                   st.issuer, "tokentrans"_n,
                   std::make_tuple(st.issuer, to, quantity, memo))
                .send();
        }
    }

   ACTION unlimited_tower::transfer(name from, name to, asset quantity, string memo)
   {
       eosio_assert(from != to, "Cannot transfer to self");
       require_auth(from);
       eosio_assert(is_account(to), "To account does not exist");
       auto sym = quantity.symbol.code().raw();
       stat statstable(_self, sym);
       const auto& st = statstable.get(sym, "Not exist symbol");

       require_recipient(from);
       require_recipient(to);

       eosio_assert(quantity.is_valid(), "Invalid quantity");
       eosio_assert(quantity.amount > 0, "Must transfer positive quantity");
       eosio_assert(quantity.symbol == st.supply.symbol, "Symbol precision mismatch");
       eosio_assert(memo.size() <= 250, "Memo has more than 256 bytes");

       sub_balance(from, quantity);
       add_balance(to, quantity, from);
   }

    inline asset get_supply(symbol_name sym) const;
    inline asset get_balance(account_name owner, symbol_name sym) const;

   ACTION unlimited_tower::sub_balance(name owner, asset value)
   {
       accounts from_acnts(_self, owner.value);

       const auto& from = from_acnts.get(value.symbol.code().raw(), "No balance object found");
       eosio_assert(from.balance.amount >= value.amount, "Overdrawn balance");

       if(from.balance.amount == value.amount){
           from_acnts.erase(from);
       }
       else{
           from_acnts.modify(from, owner, [&](auto& a){
               a.balance -= value;
           });
       }
   }

   ACTION unlimited_tower::add_balance(name owner, asset value, name ram_payer)
   {
       accounts to_acnts(_self, owner.value);
       auto to = to_acnts.find(value.symbol.code().raw());
       if(to == to_acnts.end()){
           to_acnts.emplace( ram_payer, [&](auto& a){
               a.balance = value;
           });
       }
       else{
           to_acnts.modify(to, ram_payer, [&](auto& a){
               a.balance += value;
           });
       }
   }


    void delete_user_balance(name _user)
    {
        token_accounts user_balance_table(_self, _user.value);
        for (auto user_balance_iter = user_balance_table.begin(); user_balance_iter != user_balance_table.end();)
        {
            auto iter = user_balance_table.find(user_balance_iter->primary_key());
            user_balance_iter++;
            user_balance_table.erase(iter);
        }
    }

    void init_stat(asset _token)
    {
        require_auth(_self);
        token_stats statstable(_self, _token.symbol.code().raw());
        for (auto token_stat_iter = statstable.begin(); token_stat_iter != statstable.end();)
        {
            auto iter = statstable.find(token_stat_iter->primary_key());
            token_stat_iter++;
            statstable.erase(iter);
        }
    }

    void init_all_balance()
    {
        require_auth(_self));
        auto &user_auth_table = login_controller.get_auth_user_table();
        for (auto user_name_iter = user_auth_table.begin(); user_name_iter != user_auth_table.end();)
        {
            delete_user_balance(user_name_iter->primary_key());
            user_name_iter++;
        }

        token_accounts user_balance_table(_self, owner.value);
        for (auto user_balance_iter = user_balance_table.begin(); user_balance_iter != user_balance_table.end();)
        {
            auto iter = user_balance_table.find(user_balance_iter->primary_key());
            user_balance_iter++;
            user_balance_table.erase(iter);
        }
    }

#pragma endresion



// asset unlimited_tower::get_supply(symbol_name sym) const
// {
//     token_stats statstable(owner, sym);
//     const auto &st = statstable.get(sym);
//     return st.supply;
// }

// asset unlimited_tower::get_balance(account_name owner, symbol_name sym) const
// {
//     token_accounts accountstable(owner, owner.value);
//     const auto &ac = accountstable.get(sym);
//     return ac.balance;
// }
