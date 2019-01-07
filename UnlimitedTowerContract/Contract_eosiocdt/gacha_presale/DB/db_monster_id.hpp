#pragma once
#include "../Common/common_header.hpp"



//class [[eosio::table]] cdbmonsterid
TABLE cdbmonsterid
{
  public:
    uint64_t look;
public:
    uint64_t primary_key() const {return look;}
};

typedef eosio::multi_index<"cdbmonsterid"_n,cdbmonsterid> monster_id_db;
