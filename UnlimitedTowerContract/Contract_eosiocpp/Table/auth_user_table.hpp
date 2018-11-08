#pragma once
#include "../Common/common_header.hpp"

enum euser_state
{
    none = 1,
    look,
    status,
    lobby,
    battle_result,
    tower,
    travel,
};
// 1 + 1 + 1 = 3
struct suser_look
{
   uint8_t head;
   uint8_t face;
   uint8_t body;
};
// 4 + 4 + 4 = 12
struct suser_status
{
    uint32_t strength;
    uint32_t dexterity;
    uint32_t intelligence;
};
// 3 + 12 = 15
struct shero_info
{
    suser_look look;
    suser_status status;
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
    uint8_t a_state;
public:
    cuserauth() {
        a_hero_list.resize(3);
        a_game_money = 100;
        a_state = static_cast<uint8_t>(euser_state::none);
    }
    uint64_t primary_key() const {return a_user;}
    void auth_set_user(account_name _user) {a_user = _user;}
    EOSLIB_SERIALIZE(
        cuserauth,
        (a_user)
        (a_hero_list)
        (a_game_money)
        (a_state)
    )
};

typedef multi_index<N(cuserauth),cuserauth> auth_user_table;