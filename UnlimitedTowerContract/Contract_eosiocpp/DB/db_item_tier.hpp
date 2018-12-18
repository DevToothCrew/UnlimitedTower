#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbitemtier i64
class cdbitemtier
{
  public:
    uint64_t i_tier;
    uint32_t i_level;
  public:
    uint64_t primary_key() const {return i_tier;}
};

typedef multi_index<N(cdbitemtier),cdbitemtier> item_tier_db;
