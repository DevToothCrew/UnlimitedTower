#pragma once
#include "../Common/common_header.hpp"


//@abi table crhair i64
class crhair
{
  public:
    uint64_t h_hair;
public:
    uint64_t primary_key() const {return h_hair;}
};

typedef multi_index<N(crhair),crhair> hair_data_table;
