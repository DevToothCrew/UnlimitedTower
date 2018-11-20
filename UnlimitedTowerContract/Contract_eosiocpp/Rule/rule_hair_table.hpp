#pragma once
#include "../Common/common_header.hpp"


//@abi table crhair i64
class crhair
{
  public:
    uint8_t h_hair;
public:
    uint8_t primary_key() const {return h_hair;}
};

typedef multi_index<N(crhair),crhair> hair_data_table;
