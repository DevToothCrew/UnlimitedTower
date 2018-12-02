#pragma once
#include "../Common/common_header.hpp"


//(4 * 19) + 8 = 84
//@abi table cuserlog i64
class cuserlog
{
private:
    account_name l_user;
public:
    uint32_t l_servant_num;
    uint32_t l_servant_slot_count;
    uint32_t l_monster_num;
    uint32_t l_monster_slot_count;
    uint32_t l_item_num;
    uint32_t l_item_slot_count;
    uint32_t l_gacha_num;
    uint32_t l_login_time;
    uint32_t l_get_gold;
    asset l_get_eos;
    uint32_t l_use_gold;
    asset l_use_eos;
    uint32_t l_battle_count;
    uint32_t l_last_stage_num;
    uint32_t l_last_tower_num;
    uint32_t l_top_clear_stage;
    uint32_t l_top_clear_tower;
    uint32_t l_party_count;
    uint32_t l_character_count;
public:
    cuserlog():
    l_use_eos(0,S(4,EOS)),
    l_get_eos(0,S(4,EOS))
    {
        l_servant_num = 0;
        l_servant_slot_count=50;
        l_monster_num = 0;
        l_monster_slot_count=50;
        l_item_num = 0;
        l_item_slot_count=50;
        l_gacha_num = 0;
        l_login_time = 0;
        l_get_gold = 0;
        l_use_gold = 0;
        l_battle_count = 0;
        l_last_stage_num = 0;
        l_last_tower_num = 0;
        l_top_clear_stage = 0;
        l_top_clear_tower = 0;
        l_party_count = 0;
        l_character_count = 0;
    }
    uint64_t primary_key() const {return l_user;}
    void log_set_user(account_name _user){l_user = _user;}
    
    EOSLIB_SERIALIZE(
            cuserlog,
            (l_user)
            (l_servant_num)
            (l_servant_slot_count)
            (l_monster_num)
            (l_monster_slot_count)
            (l_item_num)
            (l_item_slot_count)
            (l_gacha_num)
            (l_login_time)
            (l_get_gold)
            (l_get_eos)
            (l_use_gold)
            (l_use_eos)
            (l_battle_count)
            (l_last_stage_num)
            (l_last_tower_num)
            (l_top_clear_stage)
            (l_top_clear_tower)
            (l_party_count)
            (l_character_count)
        )
};

typedef multi_index<N(cuserlog),cuserlog> user_logs;