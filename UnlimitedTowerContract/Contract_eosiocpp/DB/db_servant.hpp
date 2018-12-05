#pragma once
#include "../Common/common_header.hpp"


struct servenet_stat
{
    uint32_t s_str;
    uint32_t s_dex;
    uint32_t s_int;
};

//@abi table cdbservant i64
class cdbservant
{
  public:
    uint64_t s_job;
    servenet_stat s_min_range;
    servenet_stat s_max_range;
public:
    uint64_t primary_key() const {return s_job;}
};

typedef multi_index<N(cdbservant),cdbservant> servant_db;
