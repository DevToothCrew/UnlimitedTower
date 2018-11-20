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
    uint64_t i_id;
    uint8_t i_type;
    item_stat i_status;
public:
    uint64_t primary_key() const {return i_id;}
};

typedef multi_index<N(critemid),critemid> item_id_data_table;
