#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbitemid i64
class cdbitemid
{
  public:
    uint64_t id;
    uint32_t slot;
    uint32_t job;
public:
    uint64_t primary_key() const {return id;}
};

typedef multi_index<N(cdbitemid),cdbitemid> item_id_db;
