#include "Common/common_header.hpp"

CONTRACT unlimited_tower : public contract
{
#pragma restion contract constructor
  public:
    using contract::contract;
    name _owner;
    unlimited_tower(eosio::name _self, eosio::name _code, datastream<const char *> ds) : contract(_self, _code, ds)
    {
        _owner = _self;
    }
#pragma endresion



#pragma resion token action
  public:
    ACTION create(name issuer, asset maximum_supply);
    ACTION issue(name to, asset quantity, string memo);
    ACTION transfer(name from, name to, asset quantity, string memo);
#pragma endresion

#pragma resion token table
  private:
    TABLE account
    {
        asset balance;
        uint64_t primary_key() const { return balance.symbol.code().raw(); }
    };

    TABLE stats
    {
        asset supply;
        asset max_supply;
        name issuer;
        uint64_t primary_key() const { return supply.symbol.code().raw(); }
    };

    typedef eosio::multi_index<"account"_n, firstToken::account> accounts;
    typedef eosio::multi_index<"stats"_n, stats> stat;
#pragma endresion

#pragma resion token function
    void sub_balance(name owner, asset value);
    void add_balance(name owner, asset value, name ram_payer);
#pragma endresion

#pragma resion token delete init
    void delete_user_balance(name _user);
    void init_stat(asset _token);
    void init_all_balance();
#pragma endresion







};