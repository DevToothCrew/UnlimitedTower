#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbitemtier i64
class cdbitemtier
{
  public:
    uint64_t tier;
    uint32_t level;
  public:
    uint64_t primary_key() const {return tier;}
};

typedef multi_index<N(cdbitemtier),cdbitemtier> item_tier_db;
