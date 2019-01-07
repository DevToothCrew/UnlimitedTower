#pragma once
#include "../Common/common_header.hpp"


//class [[eosio::table]] cdbitemid
TABLE cdbitemid
{
  public:
    uint64_t id;
    uint64_t slot;
    uint64_t job;
public:
    uint64_t primary_key() const {return id;}
};

typedef eosio::multi_index<"cdbitemid"_n,cdbitemid> item_id_db;
