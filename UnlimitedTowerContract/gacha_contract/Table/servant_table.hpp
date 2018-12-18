#pragma once
#include "../Common/common_header.hpp"


struct servant_info
{
   uint32_t state; //서번트 상태
   uint32_t exp = 0; //서번트 경험치
   uint32_t job;
   uint32_t stat_point;
   appear_info appear;    //서번트 외형 정보
   status_info status;    //기본 힘,민,지 추가 힘,민,지
   std::vector<uint32_t> equip_slot; //서번트 장비 리스트
};

//@abi table cservant i64
class cservant
{
  public:
    uint64_t index;
    servant_info servant;
  public:
    cservant()
    {
    }
    uint64_t primary_key() const { return index; }
    EOSLIB_SERIALIZE(
        cservant,
        (index)
        (servant)
        )
};

typedef multi_index<N(cservant), cservant> user_servants;