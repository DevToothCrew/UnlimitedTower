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
#include <eosiolib/crypto.h>
#include <eosiolib/transaction.hpp>


using namespace eosio;
using std::string;

namespace eosiosystem
{
class system_contract;
};
namespace safemath
{
uint64_t mul64(uint64_t a,uint64_t b)
{
    if(a==0 || b==0)
    {
        return 0;
    }
    uint64_t c = a * b;
    eosio_assert(c/a == b,"mul overflow error");
    return c;
}
uint64_t div64(uint64_t a,uint64_t b)
{
    eosio_assert(b != 0,"impossible div");
    if(a==0)
    {
        return 0;
    }
    uint64_t c = a / b;
    eosio_assert(a == b * c + a % b,"div overflow error");
    return c;
}
uint64_t add64(uint64_t a,uint64_t b)
{
    uint64_t c = a + b;
    eosio_assert(c >= a,"add overflow error");
    return c;
}
uint64_t sub64(uint64_t a,uint64_t b)
{
    eosio_assert(b<=a ,"impossible sub");
    return a-b;
}
//-------------------------------------------------------//
//-------------------------------------------------------//
//-------------------------------------------------------//
//-------------------------------------------------------//



uint64_t return_seed_value(uint64_t _user)
{
      uint64_t block_num = tapos_block_num();
      uint64_t block_prefix = tapos_block_prefix();
      uint64_t current_time = now();

      uint64_t first_seed = block_num * current_time;
      uint64_t second_seed;

      if(first_seed / block_num != current_time)
      {
            first_seed = block_num + block_prefix + current_time;
            if(first_seed < block_num)
            {
                  return _user + current_time;
            }
            return first_seed;
      }
      else
      {
            second_seed = first_seed * block_prefix;
            if(second_seed / first_seed != block_prefix)
            {
                  if(_user < current_time)
                  {
                        second_seed = current_time - _user;
                  }
                  else
                  {
                        second_seed = _user - current_time;
                  }
                  return second_seed;
            }
            return second_seed;
      }
}

uint64_t get_seed(uint64_t _user)
{
    uint64_t seed = return_seed_value(_user);

    checksum256 check_sum_result;
    sha256((char *)&seed, sizeof(seed), &check_sum_result);
    uint64_t *result_seed = reinterpret_cast<uint64_t *>(&check_sum_result.hash);

    return *result_seed;
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
      //@abi action
      void calculate(account_name _user);

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
