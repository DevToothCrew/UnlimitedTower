#pragma once
#include "../Common/common_header.hpp"

//class [[eosio::table]] cuserlog
TABLE cuserlog
{
private:
    uint64_t user;
public:
    uint32_t servant_num;
    uint32_t monster_num;
    uint32_t item_num;
    uint32_t gacha_num;
    uint32_t login_time;
    uint32_t get_gold;
    asset get_eos;
    uint32_t use_gold;
    asset use_eos;
    uint32_t battle_count;
    uint32_t last_stage_num;
    uint32_t last_tower_num;
    uint32_t top_clear_stage;
    uint32_t top_clear_tower;
    uint32_t add_party_count;
public:
    cuserlog():
    use_eos(0,symbol(symbol_code("EOS"),4)),
    get_eos(0,symbol(symbol_code("EOS"),4))
    {
        servant_num = 0;
        monster_num = 0;
        item_num = 0;
        gacha_num = 0;
        login_time = 0;
        get_gold = 0;
        use_gold = 0;
        battle_count = 0;
        last_stage_num = 0;
        last_tower_num = 0;
        top_clear_stage = 0;
        top_clear_tower = 0;
        add_party_count = 0;
    }
    uint64_t primary_key() const {return user;}
    void log_set_user(uint64_t _user){user = _user;}
    
    EOSLIB_SERIALIZE(
            cuserlog,
            (user)
            (servant_num)
            (monster_num)
            (item_num)
            (gacha_num)
            (login_time)
            (get_gold)
            (get_eos)
            (use_gold)
            (use_eos)
            (battle_count)
            (last_stage_num)
            (last_tower_num)
            (top_clear_stage)
            (top_clear_tower)
            (add_party_count)
        )
};

typedef eosio::multi_index<"cuserlog"_n,cuserlog> user_logs;
