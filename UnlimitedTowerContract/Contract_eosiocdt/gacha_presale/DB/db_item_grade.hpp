#pragma once
#include "../Common/common_header.hpp"



//class [[eosio::table]] cdbitemgrade
TABLE cdbitemgrade
{
  public:
    uint64_t grade;
    object_status min_range;
    object_status max_range;
  public:
    uint64_t primary_key() const {return grade;}
};

typedef eosio::multi_index<"cdbitemgrade"_n,cdbitemgrade> item_grade_db;
