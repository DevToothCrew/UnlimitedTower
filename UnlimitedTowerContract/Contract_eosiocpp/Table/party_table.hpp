#pragma once
#include "../Common/common_header.hpp"

// 8 
//sparty_info 당  8
struct sparty_info
{
    std::vector<uint64_t> party_id_list;
};

// 8 + 16 = 24
// vector당 16
//@abi table cparty i64
class cparty
{
private:
    account_name p_user;
public:
    std::vector<sparty_info> p_party_list;
public:
    cparty() {}
    uint64_t primary_key() const {return p_user;}
    void add_party(uint8_t _party_number)
    {
        sparty_info temp;
        p_party_list.push_back(temp);
        p_party_list[_party_number].party_id_list.resize(10);
    }
    void party_set_user(account_name _user) {p_user = _user;}
    EOSLIB_SERIALIZE(
        cparty,
        (p_user)
        (p_party_list)
    )
};

typedef multi_index<N(cparty),cparty> user_partys;