#pragma once
#include "../Common/common_header.hpp"
#include "Table/servent_table.hpp"

// 8 + 4 + 13 + 16 + 4 = 46
struct cmonsterinfo
{
    uint64_t m_index;
    uint32_t m_type_index;
    sobject_status m_status_info;
    sobject_plus_status m_plus_status_info;
    uint32_t m_exp;
};

// 8 + 46 = 54
// vector 당 46
//@abi table cmonster i64
class cmonster
{
private:
    account_name m_user;
public:
    std::vector<cmonsterinfo> m_monster_list;
public:
    cmonster() {}
    uint64_t primary_key() const {return m_user;}
    void monster_set_user(account_name _user) {m_user = _user;}
    EOSLIB_SERIALIZE(
        cmonster,
        (m_user)
        (m_monster_list)
    )
};

typedef multi_index<N(cmonster),cmonster> user_monster_table;