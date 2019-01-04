#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbmonsterid i64
class cdbmonsterid
{
  public:
    uint64_t look;
public:
    uint64_t primary_key() const {return look;}
};

typedef multi_index<N(cdbmonsterid),cdbmonsterid> monster_id_db;
