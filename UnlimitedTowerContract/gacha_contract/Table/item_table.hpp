#pragma once
#include "../Common/common_header.hpp"
#include "Table/servant_table.hpp"


struct item_info
{
   uint32_t state; //아이템 현재 상태
   uint32_t id; //아이템 리소스 아이디
   uint32_t slot;  //장착 타입
   uint32_t tier;  //티어
   uint32_t job; //직업제한
   uint32_t grade; //아이템 등급
   uint32_t upgrade = 0; //아이템 강화 수치
   uint32_t atk = 0;
   uint32_t def = 0;
   status_info status; //기본 힘,민,지 추가 힘,민,지
};

//@abi table citem i64
class citem
{
public:
    uint64_t index;
    item_info item;
public:
    citem() {}
    uint64_t primary_key() const {return index;}
    EOSLIB_SERIALIZE(
        citem,
        (index)
        (item)
    )
};

typedef multi_index<N(citem),citem> user_items;