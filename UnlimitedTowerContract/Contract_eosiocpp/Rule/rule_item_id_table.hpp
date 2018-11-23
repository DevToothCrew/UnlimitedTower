#pragma once
#include "../Common/common_header.hpp"


//@abi table critemid i64
class critemid
{
  public:
    uint64_t i_id;
    uint8_t i_type;
    uint8_t i_job;

public:
    uint64_t primary_key() const {return i_id;}
};

typedef multi_index<N(critemid),critemid> item_id_data_table;
