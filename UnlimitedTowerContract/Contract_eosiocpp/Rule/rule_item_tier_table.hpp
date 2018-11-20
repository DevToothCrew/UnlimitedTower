#pragma once
#include "../Common/common_header.hpp"


//@abi table critemtier i64
class critemtier
{
  public:
    uint8_t i_tier;
    uint8_t i_level;
public:
    uint8_t primary_key() const {return i_tier;}
};

typedef multi_index<N(critemtier),critemtier> item_tier_data_table;
