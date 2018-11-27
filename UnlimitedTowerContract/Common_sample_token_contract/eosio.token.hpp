/**
 *  @file
 *  @copyright defined in eos/LICENSE.txt
 */
#pragma once

#include <eosiolib/asset.hpp>
#include <eosiolib/eosio.hpp>

#include <string>

namespace eosiosystem {
   class system_contract;
}
uint64_t mul(uint64_t a,uint64_t b)
{
    if(a==0 || b==0)
    {
        return 0;
    }
    uint64_t c = a * b;
    eosio_assert(c/a == b,"mul error");
    return c;
}
uint64_t div(uint64_t a,uint64_t b)
{
    eosio_assert(b != 0,"impossible div");
    if(a==0)
    {
        return 0;
    }
    uint64_t c = a / b;
    eosio_assert(a == b * c + a % b,"div error");
    return c;
}
uint64_t add(uint64_t a,uint64_t b)
{
    uint64_t c = a + b;
    eosio_assert(c >= a,"add error");
    return c;
}
uint64_t sub(uint64_t a,uint64_t b)
{
    eosio_assert(b<=a ,"impossible sub");
    return a-b;
}

namespace eosio {

   using std::string;

   class token : public contract {
      public:
         token( account_name self ):contract(self){}
         //@abi action
         void create( account_name issuer,
                      asset        maximum_supply);
         //@abi action
         void issue( account_name to, asset quantity, string memo );
         //@abi action
         void transfer( account_name from,
                        account_name to,
                        asset        quantity,
                        string       memo );
         //@abi action
         void apply(uint64_t receiver, uint64_t code, uint64_t action)
         

      
         inline asset get_supply( symbol_name sym )const;
         
         inline asset get_balance( account_name owner, symbol_name sym )const;

      private:
         //@abi table accounts i64
         struct account {
            asset    balance;

            uint64_t primary_key()const { return balance.symbol.name(); }
         };
         //@abi table stats i64
         struct currency {
            asset          supply;
            asset          max_supply;
            account_name   issuer;

            uint64_t primary_key()const { return supply.symbol.name(); }
         };

         typedef eosio::multi_index<N(accounts), account> accounts;
         typedef eosio::multi_index<N(stat), currency> stats;

         void sub_balance( account_name owner, asset value );
         void add_balance( account_name owner, asset value, account_name ram_payer );

      public:
         struct transfer_args {
            account_name  from;
            account_name  to;
            asset         quantity;
            string        memo;
         };
   };

   asset token::get_supply( symbol_name sym )const
   {
      stats statstable( _self, sym );
      const auto& st = statstable.get( sym );
      return st.supply;
   }

   asset token::get_balance( account_name owner, symbol_name sym )const
   {
      accounts accountstable( _self, owner );
      const auto& ac = accountstable.get( sym );
      return ac.balance;
   }

} /// namespace eosio
