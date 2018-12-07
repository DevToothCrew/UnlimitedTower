#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbhair i64
class cdbhair
{
  public:
    uint64_t hair;
public:
    uint64_t primary_key() const {return hair;}
};

typedef multi_index<N(cdbhair),cdbhair> hair_db;
