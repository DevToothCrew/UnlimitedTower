#pragma once
#include "../Common/common_header.hpp"
enum equip_list
{
    hand = 0,
    body,
    neck,
};
struct equip_item_info
{
    uint32_t servant_index = 0;
    std::vector<uint32_t> item_list;
};

//@abi table cequipdata i64
class cequipdata
{
private:
    account_name user;
public:    
    std::vector<equip_item_info> hero_list;
    std::vector<equip_item_info> servant_list;
public:
    cequipdata()
    {
        hero_list.resize(3);
        for (uint32_t i = 0; i < 3; ++i)
        {
            hero_list[i].item_list.resize(3);
        }
    }
    uint64_t primary_key() const { return user; }
    void set_account(account_name _user){user = _user;}
    EOSLIB_SERIALIZE(
        cequipdata,
        (user)
        (hero_list)
        (servant_list)
    )

};

typedef multi_index<N(cequipdata),cequipdata> equip_data_table;