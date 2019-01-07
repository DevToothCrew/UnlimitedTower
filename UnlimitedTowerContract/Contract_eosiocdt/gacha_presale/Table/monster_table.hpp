#pragma once
#include "../Common/common_header.hpp"
#include "./servant_table.hpp"
//#include "Table/servant_table.hpp"

//struct monster_info
TABLE monster_info
{
   uint32_t state; //몬스터 상태값
   uint32_t exp = 0; //경험치
   uint32_t type = 0;   //속성 타입
   uint32_t look;
   uint32_t grade; // 등급
   uint32_t upgrade = 0; //강화수치
   status_info status; //기본 힘,민,지 추가 힘,민,지
};

 //

//class [[eosio::table]] cmonster
TABLE cmonster
{
public:
    uint64_t index;
    uint32_t party_number;
    monster_info monster;
public:
    cmonster() {
        party_number = EMPTY_PARTY;
    }
    uint64_t primary_key() const {return index;}
    EOSLIB_SERIALIZE(
        cmonster,
        (index)
        (party_number)
        (monster)
    )
};

typedef eosio::multi_index<"cmonster"_n,cmonster> user_monsters;
