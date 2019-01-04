#pragma once
#include "../Common/common_header.hpp"

enum battle_action_state
{
    wait = 0,
    attack,
    defense,
    use_skill,
    state_count,
};

enum battle_buff_state
{
    none = 0,
    sleep,
    poison,
    strength_collect_wait,
};

enum active_skill_list
{
    none_active = 0,
    double_attack, //더블 어택 - 15% 확률로 2번 공격한다.
    all_attack,    //참격 - 적 앞라인 전체 적을 공격력의 35%로 공격한다.
    heal,          //치유 - 대상 아군을 지능 수치의 100%만큼 치유한다.
    no_think,      //무아지경 - 공격력의 50%로 랜덤한 적 4개체 타격 (같은 개체 타격 가능)
    poison_arrow,  //독화살 - 적에게 공격력 40%의 데미지를 입히고, 상태이상 : 중독을 건다 (중독 : 4턴 동안 턴 마다 공격력의 30%의 데미지를 입힌다.)
    active_skill_count,
};

enum passive_skill_list
{
    none_passive = 0,
    iron_wall,      //철벽 - 방어율 10% 상승 (방어력 아님)
    blood_attack,   //피의 일격 - 치명타 시 데미지 10%만큼 회복
    deceiver,       //기만자 - HP 100%일 때 자신의 속도 10 증가
    strength_flag,  //힘의 깃발 - 자신의 양 옆 캐릭터에게 공격력 15% 상승 버프를 부여
    sniper,         //저격수 - 자신의 앞에 캐릭터가 존재할 경우 공격력 50% 증가
    strength_collect,//힘모으기 - 방어시 다음 턴 공격력 25% 증가
    passive_skill_count,
};
// 1 + 8 = 9
struct battle_member_state
{
    uint32_t count;       //캐릭터 상태의 지속 턴 횟수
    uint32_t state;           //캐릭터의 현재 상태
};
// 4 + 4 + 4 + 4 + 4 + 4 + 8 + sbattle_member_state(9) = 41
// sbattle_member_state 당 9 총 5개의 버프창이 있으면 45 + 32 = 77
struct battle_staus_info
{
    uint32_t state;
    uint32_t speed;
    uint32_t critical;
    uint32_t defense;
    uint32_t attack;
    uint32_t now_hp = 0;
    std::vector<battle_member_state> state_list;
    uint64_t party_index = 0;
    uint32_t active_skill = active_skill_list::none_active;
    uint32_t passive_skill = passive_skill_list::none_passive;
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
    account_name user;
public:
    uint32_t turn_count;
    uint32_t stage_number;
    uint32_t preference;
    std::vector<uint32_t> reward_list;
    std::vector<battle_staus_info> battle_state_list;
    std::vector<attack_speed> attack_order_list;
public:
    cbattle() {
        preference = 0;
        turn_count = 0;
        battle_state_list.resize(20);
        attack_order_list.resize(20);
    }
    uint64_t primary_key() const {return user;}
    void battle_set_user(account_name _user) {user = _user;}
    EOSLIB_SERIALIZE(
        cbattle,
        (user)
        (turn_count)
        (stage_number)
        (preference)
        (reward_list)
        (battle_state_list)
        (attack_order_list)
    )
};

typedef multi_index<N(cbattle),cbattle> user_battle_info;