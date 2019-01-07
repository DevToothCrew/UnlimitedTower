#pragma once
#include "../Common/common_header.hpp"


enum result
{
    servant = 1,
    monster,
    item,
};

//struct result_info
TABLE result_info
{
    uint64_t index;
    uint32_t type;
};


//class [[eosio::table]] cgacharesult
TABLE cgacharesult
{ 
public:
    uint64_t user;
    result_info result;
public:
    cgacharesult() {}
    uint64_t primary_key() const {return user;}
    EOSLIB_SERIALIZE(
        cgacharesult,
        (user)
        (result)
    )
};

typedef eosio::multi_index<"cgacharesult"_n,cgacharesult> user_gacha_results;
