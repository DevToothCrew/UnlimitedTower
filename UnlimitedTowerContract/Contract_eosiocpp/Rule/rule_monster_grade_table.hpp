#pragma once
#include "../Common/common_header.hpp"


struct monster_stat
{
    uint8_t m_str;
    uint8_t m_dex;
    uint8_t m_int;
};

//@abi table crmgrade i64
class crmgrade
{
  public:
    uint8_t m_grade;
    monster_stat s_min_range;
    monster_stat s_max_rangeß;
public:
    uint8_t primary_key() const {return m_grade;}
};

typedef multi_index<N(crmgrade),crmgrade> monster_grade_data_table;