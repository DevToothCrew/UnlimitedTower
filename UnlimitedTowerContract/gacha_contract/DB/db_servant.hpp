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
    uint64_t job;
    object_status min_range;
    object_status max_range;
public:
    uint64_t primary_key() const {return job;}
};

typedef multi_index<N(cdbservant),cdbservant> servant_db;
