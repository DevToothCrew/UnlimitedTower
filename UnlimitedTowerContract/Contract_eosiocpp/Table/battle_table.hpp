#pragma once
#include "../Common/common_header.hpp"


enum echaracter_state
{
    wait = 0,
    sleep,
};
// 1 + 8 = 9
struct scharacter_state
{
    uint8_t turn_count;       //캐릭터 상태의 지속 턴 횟수
    uint64_t state;           //캐릭터의 현재 상태
};
// 4 + 9 + 8 = 21 
//vector 개당 + 9
struct scharacter_info
{
    uint32_t speed;
    uint32_t critical;
    uint32_t defense;
    uint32_t attack;
    uint32_t now_hp = 0;
    std::vector<scharacter_state> state_list;
    uint64_t party_object_index = 0;
};
// 8 + 1 + 4 + 1 + 21 + 21 = 56
//vector 당 21
//@abi table cbattle i64
class cbattle
{
private:
    account_name b_user;
public:
    uint8_t b_turn_count;
    uint32_t b_stage_index;
    uint8_t b_party_number;
    std::vector<scharacter_info> b_my_party_list;
    std::vector<scharacter_info> b_enemy_party_list;
public:
    cbattle() {
        b_turn_count = 0;
        b_party_number = 0;
        b_my_party_list.resize(10);
        b_enemy_party_list.resize(10);
    }
    uint64_t primary_key() const {return b_user;}
    void battle_set_user(account_name _user) {b_user = _user;}
    EOSLIB_SERIALIZE(
        cbattle,
        (b_user)
        (b_turn_count)
        (b_stage_index)
        (b_party_number)
        (b_my_party_list)
        (b_enemy_party_list)
    )
};

typedef multi_index<N(cbattle),cbattle> user_battle_table;