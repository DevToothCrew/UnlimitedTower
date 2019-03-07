#pragma once
#include "../Common/common_header.hpp"
struct stage_info
{
    uint32_t type_index;
    uint32_t base_str = 0;
    uint32_t base_dex = 0;
    uint32_t base_int = 0;
    uint32_t base_job = 0;
};

//@abi table cstagedata i64
class cstagedata
{
public:
    uint64_t stage;
    std::vector<stage_info> enemy_list;
public:
    uint64_t primary_key() const {return stage;}
};

typedef multi_index<N(cstagedata),cstagedata> battle_data_table;