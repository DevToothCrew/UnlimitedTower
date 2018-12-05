#pragma once
#include "../Common/common_header.hpp"

struct item_stat
{
    uint32_t i_str;
    uint32_t i_dex;
    uint32_t i_int;
};
//@abi table cdbitemgrade i64
class cdbitemgrade
{
  public:
    uint64_t i_grade;
    item_stat i_min_range;
    item_stat i_max_range;
  public:
    uint64_t primary_key() const {return i_grade;}
};

typedef multi_index<N(cdbitemgrade),cdbitemgrade> item_grade_db;
