#pragma once
#include "../Common/common_header.hpp"



enum party_state
{
    on_wait = 1,
    on_tower_defense,
};

// 8 * 10 = 80
//party_info 당  8
struct party_info
{
    uint32_t state = party_state::on_wait;
    std::vector<uint64_t> index_list;
};

// 8 + 80 = 88
// vector당 80
// 112 + 88 = 200
//@abi table cparty i64
class cparty
{
private:
    account_name user;
public:
    std::vector<party_info> party_list;
public:
    cparty() {}
    uint64_t primary_key() const {return user;}
    void add_party(uint8_t _party_number)
    {
        party_info temp;
        party_list.push_back(temp);
        party_list[_party_number].index_list.resize(10);
    }
    void party_set_user(account_name _user) {user = _user;}
    EOSLIB_SERIALIZE(
        cparty,
        (user)
        (party_list)
    )
};

typedef multi_index<N(cparty),cparty> user_partys;