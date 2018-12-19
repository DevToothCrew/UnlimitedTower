#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbitemgrade i64
class cdbitemgrade
{
  public:
    uint64_t i_grade;
    object_status i_min_range;
    object_status i_max_range;
  public:
    uint64_t primary_key() const {return i_grade;}
};

typedef multi_index<N(cdbitemgrade),cdbitemgrade> item_grade_db;
