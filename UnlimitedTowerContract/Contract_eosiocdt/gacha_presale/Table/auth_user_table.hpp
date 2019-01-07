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

enum hero_state
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
//struct appear_info
TABLE appear_info
{
    uint32_t head = 0;
    uint32_t hair = 0;
    uint32_t body = 0;
};

//struct status_info
TABLE status_info
{
    uint32_t basic_str = 0;
    uint32_t basic_dex = 0;
    uint32_t basic_int = 0;
    uint32_t plus_str = 0;
    uint32_t plus_dex = 0;
    uint32_t plus_int = 0;
};

//struct hero_info
TABLE hero_info
{
   uint32_t state; //히어로 상태
   uint32_t exp = 0; //히어로 경험치
   uint32_t job;
   uint32_t stat_point = 0;
   appear_info appear;    //히어로 외형 정보
   status_info status;    //기본 힘,민,지 추가 힘,민,지
   std::vector<uint32_t> equip_slot; //히어로 장비 리스트
};


//class [[eosio::table]] cuserauth
TABLE cuserauth
{
private:
    uint64_t user;
public:
    uint32_t game_money;
    uint32_t state;
    hero_info hero;
public:
    cuserauth() {
        game_money = 100;
        state = euser_state::login;
    }
    uint64_t primary_key() const {return user;}
    void auth_set_user(uint64_t _user) {user = _user;}

};

typedef eosio::multi_index<"cuserauth"_n,cuserauth> auth_users;
