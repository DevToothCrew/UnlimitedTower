#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbseedlog i64
class cdbseedlog
{
  public:
    account_name owner;
    uint64_t count;
  public:
    uint64_t primary_key() const { return owner; }
};

typedef multi_index<N(cdbseedlog),cdbseedlog> seed_log_db;