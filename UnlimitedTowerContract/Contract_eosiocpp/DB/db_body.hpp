#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbbody i64
class cdbbody
{
  public:
    uint64_t body;
public:
    uint64_t primary_key() const {return body;}
};

typedef multi_index<N(cdbbody),cdbbody> body_db;
