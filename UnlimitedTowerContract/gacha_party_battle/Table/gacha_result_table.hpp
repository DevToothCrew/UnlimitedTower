#pragma once
#include "../Common/common_header.hpp"


enum result
{
    servant = 1,
    monster,
    item,
};

struct result_info
{
    uint64_t index;
    uint32_t type;
};


//@abi table cgacharesult i64
class cgacharesult
{ 
public:
    account_name user;
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

typedef multi_index<N(cgacharesult),cgacharesult> user_gacha_results;