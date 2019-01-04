#pragma once
#include "../Common/common_header.hpp"



//@abi table cdbmgrade i64
class cdbmgrade
{
  public:
    uint64_t monster_grade;
    object_status min_range;
    object_status max_range;
public:
    uint64_t primary_key() const {return monster_grade;}
};

typedef multi_index<N(cdbmgrade),cdbmgrade> monster_grade_db;