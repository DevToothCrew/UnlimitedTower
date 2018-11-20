#pragma once
#include "../Common/common_header.hpp"

// 1 + 1 + 1 = 3
struct sobject_appear
{
    uint8_t body = 0;
    uint8_t face = 0;
    uint8_t hair = 0;
};
// 4 + 4 + 4 + 1 = 13
struct sobject_status
{
    uint32_t strength = 0;
    uint32_t dexterity = 0;
    uint32_t intelligence = 0;
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
struct cserventinfo
{
    uint64_t s_index;
    sobject_appear appear_info;
    sobject_status status_info;
    sobject_plus_status plus_status_info;
    uint32_t s_exp = 0;
};

// 8 + 48 = 56
// vector당 48
//@abi table cservent i64
class cservent
{
  private:
    account_name s_user;

  public:
    std::vector<cserventinfo> s_servent_list;

  public:
    cservent()
    {
    }
    uint64_t primary_key() const { return s_user; }
    void servent_set_user(account_name _user) { s_user = _user; }
    EOSLIB_SERIALIZE(
        cservent,
        (s_user)(s_servent_list))
};

typedef multi_index<N(cservent), cservent> user_servent_table;