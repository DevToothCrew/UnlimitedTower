#pragma once
#include "../Common/common_header.hpp"

// 1 + 1 + 1 = 3
struct sobject_appear
{
    uint8_t head = 0;
    uint8_t hair = 0;
    uint8_t body = 0;
};
// 4 + 4 + 4 + 1 = 13
struct sobject_status
{
    uint32_t basic_str = 0;
    uint32_t basic_dex = 0;
    uint32_t basic_int = 0;
    uint8_t job = 0;
};
// 4 + 4 + 4 + 4 = 16
struct sobject_plus_status
{
    uint32_t plus_str = 0;
    uint32_t plus_dex = 0;
    uint32_t plus_int = 0;
    uint32_t stat_point = 0;
};
// 8 + 4 + 3 + 13 + 16 + 4 = 48
struct cservantinfo
{
    uint64_t s_index;           //서번트 인덱스번호
    sobject_appear s_appear;    //서번트 외형 정보
    sobject_status s_status;    //서번트 힘민지 직업
    sobject_plus_status s_plus_status;  //서번트 추가힘민지
    std::vector<uint32_t> s_equip_slot; //서번트 장비 리스트
    uint32_t s_exp = 0; //서번트 경험치
    uint32_t s_state; //서번트 상태
};

// 8 + 48 = 56
// vector당 48
//@abi table cservant i64
class cservant
{
  private:
    account_name s_user;

  public:
    std::vector<cservantinfo> s_servant_list;

  public:
    cservant()
    {
    }
    uint64_t primary_key() const { return s_user; }
    void servant_set_user(account_name _user) { s_user = _user; }
    EOSLIB_SERIALIZE(
        cservant,
        (s_user)(s_servant_list))
};

typedef multi_index<N(cservant), cservant> user_servants;