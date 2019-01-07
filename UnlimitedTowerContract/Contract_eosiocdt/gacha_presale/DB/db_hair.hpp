#pragma once
#include "../Common/common_header.hpp"



//class [[eosio::table]] cdbhair
TABLE cdbhair
{
  public:
    uint64_t hair;
public:
    uint64_t primary_key() const {return hair;}
};

typedef eosio::multi_index<"cdbhair"_n,cdbhair> hair_db;
