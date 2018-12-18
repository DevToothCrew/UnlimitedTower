#pragma once
#include "../Common/common_header.hpp"


struct object_status
{
    uint32_t base_str;
    uint32_t base_dex;
    uint32_t base_int;
};

//@abi table cdbservant i64
class cdbservant
{
  public:
    uint64_t s_job;
    object_status s_min_range;
    object_status s_max_range;
public:
    uint64_t primary_key() const {return s_job;}
};

typedef multi_index<N(cdbservant),cdbservant> servant_db;
