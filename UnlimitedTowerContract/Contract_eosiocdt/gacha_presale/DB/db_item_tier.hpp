#pragma once
#include "../Common/common_header.hpp"



//class [[eosio::table]] cdbitemtier
TABLE cdbitemtier
{
  public:
    uint64_t tier;
    uint64_t level;
  public:
    uint64_t primary_key() const {return tier;}
};

typedef eosio::multi_index<"cdbitemtier"_n,cdbitemtier> item_tier_db;
