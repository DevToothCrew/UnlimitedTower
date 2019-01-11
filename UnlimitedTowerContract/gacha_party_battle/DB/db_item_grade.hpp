#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbitemgrade i64
class cdbitemgrade
{
  public:
    uint64_t grade;
    object_status min_range;
    object_status max_range;
  public:
    uint64_t primary_key() const {return grade;}
};

typedef multi_index<N(cdbitemgrade),cdbitemgrade> item_grade_db;
