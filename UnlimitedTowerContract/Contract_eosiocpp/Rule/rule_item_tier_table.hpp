#pragma once
#include "../Common/common_header.hpp"


//@abi table critemtier i64
class critemtier
{
  public:
    uint64_t i_tier;
    uint8_t i_level;
public:
    uint64_t primary_key() const {return i_tier;}
};

typedef multi_index<N(critemtier),critemtier> item_tier_data_table;
