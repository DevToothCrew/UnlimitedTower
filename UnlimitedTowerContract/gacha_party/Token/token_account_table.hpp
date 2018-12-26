#pragma once
#include "../Common/common_header.hpp"

//@abi table ctaccount i64
class ctaccount
{
public:
    asset balance;
    uint64_t primary_key() const { return balance.symbol.name(); }
    EOSLIB_SERIALIZE(
        ctaccount,
        (balance))
};

typedef eosio::multi_index<N(ctaccount), ctaccount> token_accounts;