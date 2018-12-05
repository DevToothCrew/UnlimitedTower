#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbhair i64
class cdbhair
{
  public:
    uint64_t h_hair;
public:
    uint64_t primary_key() const {return h_hair;}
};

typedef multi_index<N(cdbhair),cdbhair> hair_db;
