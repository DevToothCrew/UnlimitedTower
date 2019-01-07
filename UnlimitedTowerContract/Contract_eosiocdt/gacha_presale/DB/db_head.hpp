#pragma once
#include "../Common/common_header.hpp"



//class [[eosio::table]] cdbhead
TABLE cdbhead
{
  public:
    uint64_t head;

  public:
    uint64_t primary_key() const { return head; }
};

typedef eosio::multi_index<"cdbhead"_n,cdbhead> head_db;
//typedef multi_index<N(cdbhead),cdbhead> head_db;
