#pragma once
#include "../Common/common_header.hpp"
#include "Table/servant_table.hpp"

// 8 + 4 + 16 + 16 + 4 + 4 + 4 + 4 = 60
struct cmonsterinfo
{
    uint64_t m_index;  //고유 인덱스
    uint32_t m_type;   //외형 - 타입
    sobject_status m_status; // 힘민지
    sobject_plus_status m_plus_status; //추가 힘민지
    uint32_t m_exp = 0; //경험치
    uint32_t m_grade; // 등급
    uint32_t m_reinforce = 0; //강화수치
    uint32_t m_state = object_state::in_ineventory;; //몬스터 상태값
};

// 8 + 60 = 68
// vector 당 60
// 112 + 68 = 180
//@abi table cmonster i64
class cmonster
{
private:
    account_name m_user;
public:
    std::vector<cmonsterinfo> monster_list;
public:
    cmonster() {}
    uint64_t primary_key() const {return m_user;}
    void monster_set_user(account_name _user) {m_user = _user;}
    EOSLIB_SERIALIZE(
        cmonster,
        (m_user)
        (monster_list)
    )
};

typedef multi_index<N(cmonster),cmonster> user_monsters;