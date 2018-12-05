#pragma once
#include "../Common/common_header.hpp"
#include "Table/servant_table.hpp"

enum item_state
{
    item_none = 0,
    item_equip,
    item_notsell,
};
// 8 + 4 + 13 = 25
struct citeminfo
{
    uint64_t i_index; //아이템 인덱스
    uint32_t i_type;  //종류
    uint32_t i_slot;  //장착 타입
    uint32_t i_tier;  //티어
    sobject_status i_status; //힘민지 직업
    uint32_t i_state; //아이템 현재 상태
    uint32_t i_grade; //아이템 등급
    uint32_t i_reinforce; //아이템 강화 수치
};

// 8 + 25 = 33
// vector당 25
//@abi table citem i64
class citem
{
private:
    account_name i_user;
public:
    std::vector<citeminfo> i_item_list;
public:
    citem() {}
    uint64_t primary_key() const {return i_user;}
    void item_set_user(account_name _user) {i_user = _user;}
    EOSLIB_SERIALIZE(
        citem,
        (i_user)
        (i_item_list)
    )
};

typedef multi_index<N(citem),citem> user_items;