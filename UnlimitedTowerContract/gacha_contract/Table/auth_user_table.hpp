#pragma once
#include "../Common/common_header.hpp"

enum euser_state
{
    login = 1,
    lobby,
    battle,
    battle_win,
    battle_lose,
    tower,
    travel,
    pause,
    black,
};

enum ehero_state
{   
    set_look = 1,
    set_status,
    set_change_status,
    set_complete,
    set_tower_party,
    set_travel_party,
};

enum eobject_state
{
    on_inventory,
    on_equip_slot,
    on_party,
    on_tower,
    object_presale,
};

// 1 + 1 + 1 = 3
struct sobject_appear
{
    uint32_t head = 0;
    uint32_t hair = 0;
    uint32_t body = 0;
};
// 4 + 4 + 4 + 1 = 13
struct sobject_status
{
    uint32_t basic_str = 0;
    uint32_t basic_dex = 0;
    uint32_t basic_int = 0;
    uint32_t job = 0;
};
// 4 + 4 + 4 + 4 = 16
struct sobject_plus_status
{
    uint32_t plus_str = 0;
    uint32_t plus_dex = 0;
    uint32_t plus_int = 0;
    uint32_t stat_point = 0;
};

// 4 + 3 + 16 + 16 + 12 = 51
struct shero_info
{
    uint32_t h_state;
    sobject_appear h_appear;
    sobject_status h_status;
    sobject_plus_status h_plus_status;
    std::vector<uint32_t> h_equip_slot;
};

//primary_key = 112
//8 + 4 + 4 + 4 + shero_info(51) = 71
//hero slot 한개 추가당 51
//총 히어로 3마리 가정하면 유저당 8 + 4 + 4 + 4 + (51 * 3) = 173
//112 + 173 = 285 byte
//@abi table cuserauth i64
class cuserauth
{
private:
    account_name a_user;
public:
    std::vector<shero_info> a_hero_list;
    uint32_t a_game_money;
    uint32_t a_state;
    uint32_t a_hero_slot;
public:
    cuserauth() {
        a_game_money = 100;
        a_state = euser_state::login;
        a_hero_slot = 0;
    }
    uint64_t primary_key() const {return a_user;}
    void auth_set_user(account_name _user) {a_user = _user;}
    EOSLIB_SERIALIZE(
        cuserauth,
        (a_user)
        (a_hero_list)
        (a_game_money)
        (a_state)
        (a_hero_slot)
    )
};

typedef multi_index<N(cuserauth),cuserauth> auth_users;