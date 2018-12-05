#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbitemid i64
class cdbitemid
{
  public:
    uint64_t i_id;
    uint32_t i_type;
    uint32_t i_job;
public:
    uint64_t primary_key() const {return i_id;}
};

typedef multi_index<N(cdbitemid),cdbitemid> item_id_db;
