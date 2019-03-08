#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbseed i64
class cdbseed
{
  public:
    uint64_t index;
    uint64_t seed;
  public:
    uint64_t primary_key() const { return index; }
};

typedef multi_index<N(cdbseed),cdbseed> seed_db;