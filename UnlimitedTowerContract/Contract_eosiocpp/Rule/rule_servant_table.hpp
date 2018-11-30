#pragma once
#include "../Common/common_header.hpp"


struct servenet_stat
{
    uint32_t s_str;
    uint32_t s_dex;
    uint32_t s_int;
};

//@abi table crservant i64
class crservant
{
  public:
    uint64_t s_job;
    servenet_stat s_min_range;
    servenet_stat s_max_range;
public:
    uint64_t primary_key() const {return s_job;}
};

typedef multi_index<N(crservant),crservant> servant_data_table;
