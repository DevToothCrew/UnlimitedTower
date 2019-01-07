#pragma once
#include "../Common/common_header.hpp"



//class [[eosio::table]] caccumulate
TABLE caccumulate
{ 
public:
    uint64_t user;
    std::vector<result_info> result_list;
public:
    caccumulate() {}
    uint64_t primary_key() const {return user;}
    EOSLIB_SERIALIZE(
        caccumulate,
        (user)
        (result_list)
    )
};

typedef eosio::multi_index<"caccumulate"_n,caccumulate> user_gacha_accumulates;
