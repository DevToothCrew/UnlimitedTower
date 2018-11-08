#pragma once
#include "../Common/common_header.hpp"
struct stage_info
{
    uint32_t type_index;
    uint32_t s_str = 0;
    uint32_t s_dex = 0;
    uint32_t s_int = 0;
    uint8_t s_job = 0;
};

//@abi table cbattledata i64
class cbattledata
{
public:
    uint64_t stage;
    std::vector<stage_info> enemy_list;
public:
    uint64_t primary_key() const {return stage;}
};

typedef multi_index<N(cbattledata),cbattledata> battle_data_table;