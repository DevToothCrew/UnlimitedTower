#pragma once
#include "../Common/common_header.hpp"



//class [[eosio::table]] cdbbody
TABLE cdbbody
{
  public:
    uint64_t body;
public:
    uint64_t primary_key() const {return body;}
};

typedef eosio::multi_index<"cdbbody"_n,cdbbody> body_db;
