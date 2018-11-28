/**
 *  @file
 *  @copyright defined in eos/LICENSE.txt
 */
#pragma once

#include <variant>
#include <string>
#include <eosiolib/asset.hpp>
#include <eosiolib/eosio.hpp>
#include <eosiolib/dispatcher.hpp>

using namespace eosio;
using std::string;

namespace eosiosystem
{
class system_contract;
};
namespace safemath
{
uint64_t mul64(uint64_t a, uint64_t b)
{
      if (a == 0 || b == 0)
      {
            return 0;
      }
      uint64_t c = a * b;
      eosio_assert(c / a == b, "64 mul error");
      return c;
}
uint64_t div64(uint64_t a, uint64_t b)
{
      eosio_assert(b != 0, "64 impossible div");
      if (a == 0)
      {
            return 0;
      }
      uint64_t c = a / b;
      eosio_assert(a == b * c + a % b, "64 div error");
      return c;
}
uint64_t add64(uint64_t a, uint64_t b)
{
      uint64_t c = a + b;
      eosio_assert(c >= a, "64 add error");
      return c;
}
uint64_t sub64(uint64_t a, uint64_t b)
{

      eosio_assert(b <= a, "64 impossible sub");
      return a - b;
}

//128
uint128_t mul128(uint128_t a, uint128_t b)
{
      if (a == 0 || b == 0)
      {
            return 0;
      }
      uint128_t c = a * b;
      eosio_assert(c / a == b, "128 mul error");
      return c;
}
uint128_t div128(uint128_t a, uint128_t b)
{
      eosio_assert(b != 0, "128 impossible div");
      if (a == 0)
      {
            return 0;
      }
      uint128_t c = a / b;
      eosio_assert(a == b * c + a % b, "128 div error");
      return c;
}
uint128_t add128(uint128_t a, uint128_t b)
{
      uint128_t c = a + b;
      eosio_assert(c >= a, "128 add error");
      return c;
}
uint128_t sub128(uint128_t a, uint128_t b)
{
      eosio_assert(b <= a, "128 impossible sub");
      return a - b;
}
}; // namespace safemath



class token : public contract
{
    public:
      token(account_name self) : contract(self) {}
      //@abi action
      void create(account_name issuer,
                  asset maximum_supply);
      //@abi action
      void issue(account_name to, asset quantity, string memo);
      //@abi action
      void transfer(account_name from,
                    account_name to,
                    asset quantity,
                    string memo);
      //@abi action
      void apply(uint64_t receiver, uint64_t code, uint64_t action);
      //@abi action
      void calmul(uint64_t a, uint64_t b);
      //@abi action
      void caladd(uint64_t a, uint64_t b);
      //@abi action
      void calsub(uint64_t a, uint64_t b);
      //@abi action
      void caldiv(uint64_t a, uint64_t b);

      inline asset get_supply(symbol_name sym) const;

      inline asset get_balance(account_name owner, symbol_name sym) const;

    private:
      //@abi table accounts i64
      struct account
      {
            asset balance;

            uint64_t primary_key() const { return balance.symbol.name(); }
            EOSLIB_SERIALIZE(
                account,
                (balance))
      };
      //@abi table stats i64
      struct currency
      {
            asset supply;
            asset max_supply;
            account_name issuer;

            uint64_t primary_key() const { return supply.symbol.name(); }
            EOSLIB_SERIALIZE(
                currency,
                (supply)(max_supply)(issuer))
      };

      typedef eosio::multi_index<N(accounts), account> accounts;
      typedef eosio::multi_index<N(stats), currency> stats;

      void sub_balance(account_name owner, asset value);
      void add_balance(account_name owner, asset value, account_name ram_payer);
      void mul_balance(account_name owner, asset value, account_name ram_payer);
      void div_balance(account_name owner, asset value, account_name ram_payer);

    public:
      struct transfer_args
      {
            account_name from;
            account_name to;
            asset quantity;
            string memo;
      };
};

asset token::get_supply(symbol_name sym) const
{
      stats statstable(_self, sym);
      const auto &st = statstable.get(sym);
      return st.supply;
}

asset token::get_balance(account_name owner, symbol_name sym) const
{
      accounts accountstable(_self, owner);
      const auto &ac = accountstable.get(sym);
      return ac.balance;
}
