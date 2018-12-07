#pragma once
#include "../Common/common_header.hpp"


//@abi table cdbhead i64
class cdbhead
{
  public:
    uint64_t head;

  public:
    uint64_t primary_key() const { return head; }
};

typedef multi_index<N(cdbhead),cdbhead> head_db;
