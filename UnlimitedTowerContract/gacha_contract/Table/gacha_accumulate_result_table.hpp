#pragma once
#include "../Common/common_header.hpp"



//@abi table caccumulate i64
class caccumulate
{ 
public:
    account_name user;
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

typedef multi_index<N(caccumulate),caccumulate> user_gacha_accumulates;