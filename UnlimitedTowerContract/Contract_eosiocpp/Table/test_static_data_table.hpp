#pragma once
#include "../Common/common_header.hpp"


//this table item,servent,monster static data
struct static_status
{
    uint32_t s_str = 0;
    uint32_t s_dex = 0;
    uint32_t s_int = 0;
    uint8_t s_job = 0;
};

//@abi table cobjectdata i64
class cobjectdata
{
  public:
    uint32_t type;
    uint32_t gacha_rate;
    static_status status;
public:
    uint32_t primary_key() const {return type;}
};

typedef multi_index<N(cobjectdata),cobjectdata> static_data_table;






