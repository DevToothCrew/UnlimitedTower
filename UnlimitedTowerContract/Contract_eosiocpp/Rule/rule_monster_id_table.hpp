#pragma once
#include "../Common/common_header.hpp"


//@abi table crmid i64
class crmid
{
  public:
    uint8_t m_id;
public:
    uint8_t primary_key() const {return m_id;}
};

typedef multi_index<N(crmid),crmid> monster_id_data_table;
