#pragma once
#include "../Common/common_header.hpp"



//class [[eosio::table]] cdbmgrade
TABLE cdbmgrade
{
  public:
    uint64_t monster_grade;
    object_status min_range;
    object_status max_range;
public:
    uint64_t primary_key() const {return monster_grade;}
};

typedef eosio::multi_index<"cdbmgrade"_n,cdbmgrade> monster_grade_db;
