#pragma once
#include "../Common/common_header.hpp"

struct item_stat
{
    uint8_t i_str;
    uint8_t i_dex;
    uint8_t i_int;
};
//@abi table critemid i64
class critemid
{
  public:
    uint8_t i_id;
    uint8_t i_type;
    item_stat i_min_range;
    item_stat i_max_range;
public:
    uint8_t primary_key() const {return i_id;}
};

typedef multi_index<N(critemid),critemid> item_id_data_table;
