#pragma once
#include "../Common/common_header.hpp"



enum party_state
{
    on_wait = 1,
    on_tower_defense,
};


// 8 + 80 = 88
// vector당 80
// 112 + 88 = 200

//class [[eosio::table("cparty")]] 
class [[eosio::table]] cparty
//TABLE cparty
{
public:
    uint32_t index;
    uint32_t state;
    std::vector<uint64_t> party;
public:
    cparty() {
        state=party_state::on_wait;
    }
    uint64_t primary_key() const {return index;}
};

typedef eosio::multi_index<"cparty"_n,cparty> user_partys;
