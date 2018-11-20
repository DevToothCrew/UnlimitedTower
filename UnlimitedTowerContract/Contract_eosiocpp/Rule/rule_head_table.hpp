#pragma once
#include "../Common/common_header.hpp"


//@abi table crhead i64
class crhead
{
  public:
    uint64_t h_head;

  public:
    uint64_t primary_key() const { return h_head; }
};

typedef multi_index<N(crhead),crhead> head_data_table;
