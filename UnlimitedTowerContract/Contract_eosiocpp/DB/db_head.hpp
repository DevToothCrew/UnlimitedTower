#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbhead i64
class cdbhead
{
  public:
    uint64_t h_head;

  public:
    uint64_t primary_key() const { return h_head; }
};

typedef multi_index<N(cdbhead),cdbhead> head_db;
