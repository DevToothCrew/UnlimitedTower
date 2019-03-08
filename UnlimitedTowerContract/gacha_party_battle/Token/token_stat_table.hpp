#pragma once
#include "../Common/common_header.hpp"


//@abi table ctstat i64
class ctstat
{
public:
    asset supply;
    asset max_supply;
    account_name issuer;

    uint64_t primary_key() const { return supply.symbol.name(); }
    EOSLIB_SERIALIZE(
        ctstat,
        (supply)(max_supply)(issuer))
};

typedef eosio::multi_index<N(ctstat), ctstat> token_stats;