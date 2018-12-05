#pragma once
#include "../Common/common_header.hpp"

enum ehero_state
{   
    set_look = 1,
    set_status,
    set_change_status,
    set_complete,
    set_tower_party,
    set_travel_party,
};

enum euser_state
{
    login = 1,
    lobby,
    battle_win,
    battle_lose,
    tower,
    travel,
    pause,
    black,
};
// 1 + 1 + 1 = 3
struct suser_look
{
   uint8_t head;
   uint8_t hair;
   uint8_t body;
};
// 4 + 4 + 4 = 12
struct suser_status
{
    uint32_t basic_str;
    uint32_t basic_dex;
    uint32_t basic_int;
    uint32_t job;
};
struct suser_plus_status
{
    uint32_t plus_str = 0;
    uint32_t plus_dex = 0;
    uint32_t plus_int = 0;
    uint32_t stat_point = 0;
};


// 3 + 12 = 15
struct shero_info
{
    uint32_t current_state;
    suser_look look;
    suser_status status;
    suser_plus_status plus_status;
    std::vector<uint32_t> equip;
};



// 8 + 15 + 4 + 8 = 35
// vector 한개당 15
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