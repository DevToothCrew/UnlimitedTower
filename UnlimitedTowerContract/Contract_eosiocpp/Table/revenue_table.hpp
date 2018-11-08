#pragma once
#include "../Common/common_header.hpp"

enum revenue_type
{
    tax = 0,
    add_character,
    change_status,
};


//@abi table crevenue i64
class crevenue
{
  private:
    account_name owner;
  public:
    asset r_tax;
    asset r_add_character;
    asset r_change_character_status;
  public:
    crevenue() : 
    r_tax(0,S(4,EOS)),
    r_add_character(0,S(4,EOS)),
    r_change_character_status(0,S(4,EOS))
    {

    }
    uint64_t primary_key() const { return owner; }
    void set_revenue_owner(account_name _owner) {owner = _owner;}
    EOSLIB_SERIALIZE(
        crevenue,
        (owner)
        (r_tax)
        (r_add_character)
        (r_change_character_status)
    )
};
typedef multi_index< N(crevenue), crevenue> revenue_table;