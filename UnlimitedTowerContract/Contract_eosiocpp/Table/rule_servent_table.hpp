#pragma once
#include "../Common/common_header.hpp"


struct servenet_stat
{
    uint8_t s_str;
    uint8_t s_dex;
    uint8_t s_int;
};

//@abi table crservent i64
class crservent
{
  public:
    uint8_t s_job;
    servenet_stat s_min_range;
    servenet_stat s_max_rangeß;
public:
    uint8_t primary_key() const {return s_job;}
};

typedef multi_index<N(crservent),crservent> servent_data_table;
