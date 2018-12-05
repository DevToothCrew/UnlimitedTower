#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbmid i64
class cdbmid
{
  public:
    uint64_t m_id;
public:
    uint64_t primary_key() const {return m_id;}
};

typedef multi_index<N(cdbmid),cdbmid> monster_id_db;
