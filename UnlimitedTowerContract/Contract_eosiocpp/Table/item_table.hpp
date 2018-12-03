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
    uint64_t i_index;
    uint32_t i_type_index;
    uint8_t i_type_equip;
    uint8_t i_tier;
    sobject_status i_status_info;
    uint32_t i_item_state;
    uint32_t i_job;
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