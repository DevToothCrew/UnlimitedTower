#pragma once
#include "../Common/common_header.hpp"


// 8 + 3 + 16 + 16 + 12 + 4 + 4 = 63
struct cservantinfo
{
    uint64_t s_index;           //서번트 인덱스번호
    sobject_appear s_appear;    //서번트 외형 정보
    sobject_status s_status;    //서번트 힘민지 직업
    sobject_plus_status s_plus_status;  //서번트 추가힘민지
    std::vector<uint32_t> s_equip_slot; //서번트 장비 리스트
    uint32_t s_exp = 0; //서번트 경험치
    uint32_t s_state = eobject_state::on_inventory; //서번트 상태
};

// 8 + 63 = 71
// vector당 63
// 112 + 71 = 183
//@abi table cservant i64
class cservant
{
  private:
    account_name s_user;

  public:
    std::vector<cservantinfo> servant_list;

  public:
    cservant()
    {
    }
    uint64_t primary_key() const { return s_user; }
    void servant_set_user(account_name _user) { s_user = _user; }
    EOSLIB_SERIALIZE(
        cservant,
        (s_user)(servant_list))
};

typedef multi_index<N(cservant), cservant> user_servants;