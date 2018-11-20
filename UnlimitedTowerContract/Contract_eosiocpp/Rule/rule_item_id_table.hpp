#pragma once
#include "../Common/common_header.hpp"


//@abi table critemid i64
class critemid
{
  public:
    uint8_t i_id;
    uint8_t i_type;
public:
    uint8_t primary_key() const {return i_id;}
};

typedef multi_index<N(critemid),critemid> item_id_data_table;
