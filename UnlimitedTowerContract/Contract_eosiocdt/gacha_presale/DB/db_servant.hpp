#pragma once
#include "../Common/common_header.hpp"


struct object_status
{
    uint64_t base_str;
    uint64_t base_dex;
    uint64_t base_int;
};


//struct [[eosio::table]] cdbservant
TABLE cdbservant
{
  //public:
    uint64_t job;
    object_status min_range;
    object_status max_range;
//public:
    uint64_t primary_key() const {return job;}
};

typedef eosio::multi_index<"cdbservant"_n,cdbservant> servant_db;
