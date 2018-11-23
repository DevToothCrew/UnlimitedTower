#pragma once
#include "../Common/common_header.hpp"

struct item_stat
{
    uint8_t i_str;
    uint8_t i_dex;
    uint8_t i_int;
};
//@abi table critemtier i64
class critemtier
{
  public:
    uint64_t i_tier;
    uint8_t i_level;
    item_stat i_min_range;
    item_stat i_max_range;
  public:
    uint64_t primary_key() const {return i_tier;}
};

typedef multi_index<N(critemtier),critemtier> item_tier_data_table;
