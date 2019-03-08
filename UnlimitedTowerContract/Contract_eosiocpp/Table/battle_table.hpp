#pragma once
#include "../Common/common_header.hpp"


enum ebattle_member_state
{
    none = 0,
    sleep,
    dead,
};
// 1 + 8 = 9
struct sbattle_member_state
{
    uint32_t turn_count;       //캐릭터 상태의 지속 턴 횟수
    uint32_t turn_state;           //캐릭터의 현재 상태
};
// 4 + 4 + 4 + 4 + 4 + 4 + 8 + sbattle_member_state(9) = 41
// sbattle_member_state 당 9 총 5개의 버프창이 있으면 45 + 32 = 77
struct sbattle_staus_info
{
    uint32_t action;
    uint32_t speed;
    uint32_t critical;
    uint32_t defense;
    uint32_t attack;
    uint32_t now_hp = 0;
    std::vector<sbattle_member_state> state_list;
    uint64_t party_object_index = 0;
};

// 4 + 4 = 8
struct attack_speed
{
    uint32_t member_array_index;
    uint32_t member_speed;
    uint32_t member_target;
};

// 8 + 1 + 4 + 1 + 1 + b_reward_list(4) + (77 * 20) + (8*20) = 1727
//@abi table cbattle i64
class cbattle
{
private:
    account_name b_user;
public:
    uint32_t b_turn_count;
    uint32_t b_stage_number;
    uint32_t b_party_number;
    uint32_t b_preference;
    std::vector<uint32_t> b_reward_list;
    std::vector<sbattle_staus_info> b_battle_state_list;
    std::vector<attack_speed> attack_order_list;
public:
    cbattle() {
        b_preference = 0;
        b_turn_count = 0;
        b_party_number = 0;
        b_battle_state_list.resize(20);
        attack_order_list.resize(20);
    }
    uint64_t primary_key() const {return b_user;}
    void battle_set_user(account_name _user) {b_user = _user;}
    EOSLIB_SERIALIZE(
        cbattle,
        (b_user)
        (b_turn_count)
        (b_stage_number)
        (b_party_number)
        (b_preference)
        (b_reward_list)
        (b_battle_state_list)
        (attack_order_list)
    )
};

typedef multi_index<N(cbattle),cbattle> user_battle_info;