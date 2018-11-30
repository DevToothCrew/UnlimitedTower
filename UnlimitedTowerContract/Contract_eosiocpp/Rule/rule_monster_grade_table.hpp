#pragma once
#include "../Common/common_header.hpp"


struct monster_stat
{
    uint32_t m_str;
    uint32_t m_dex;
    uint32_t m_int;
};

//@abi table crmgrade i64
class crmgrade
{
  public:
    uint64_t m_grade;
    monster_stat m_min_range;
    monster_stat m_max_range;
public:
    uint64_t primary_key() const {return m_grade;}
};

typedef multi_index<N(crmgrade),crmgrade> monster_grade_data_table;