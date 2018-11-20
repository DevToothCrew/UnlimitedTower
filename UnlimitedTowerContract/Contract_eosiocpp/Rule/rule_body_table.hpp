#pragma once
#include "../Common/common_header.hpp"


//@abi table crbody i64
class crbody
{
  public:
    uint64_t b_body;
public:
    uint64_t primary_key() const {return b_body;}
};

typedef multi_index<N(crbody),crbody> body_data_table;
