#pragma once
#include "../Common/common_header.hpp"



//@abi table cdbmgrade i64
class cdbmgrade
{
  public:
    uint64_t m_grade;
    object_status m_min_range;
    object_status m_max_range;
public:
    uint64_t primary_key() const {return m_grade;}
};

typedef multi_index<N(cdbmgrade),cdbmgrade> monster_grade_db;