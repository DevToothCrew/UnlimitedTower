#pragma once
#include "../Common/common_header.hpp"
#include "../Rule/rule_servent_table.hpp"
#include "../Rule/rule_head_table.hpp"
#include "../Rule/rule_hair_table.hpp"
#include "../Rule/rule_body_table.hpp"

class crule_system
{
  private:
    account_name owner;

  private:
    servent_data_table servent_rule;
    head_data_table head_rule;
    hair_data_table hair_rule;
    body_data_table body_rule;
  public:
    crule_system(account_name _self)
        : owner(_self),
          servent_rule(_self, _self),
          head_rule(_self,_self),
          hair_rule(_self,_self),
          body_rule(_self,_self)
    {
    }
    uint64_t random_value(uint32_t _range)
    {
        checksum256 l_result;
        uint64_t l_source = tapos_block_num() * tapos_block_prefix();
        sha256((char *)&l_source, sizeof(l_source), &l_result);
        uint64_t *l_p = reinterpret_cast<uint64_t *>(&l_result.hash);
        uint64_t l_random_result = *l_p % _range;
        if (l_random_result == 0)
        {
            l_random_result++;
        }
        return l_random_result;
    }
    void init_data()
    {
        require_auth2(owner,N(owner));
        for (uint32_t i = 0; i < 5; ++i)
        {
            servent_rule.emplace(owner, [&](auto a) {
                a.s_job = servent_rule.available_primary_key();
                a.s_min_range.s_str = random_value(10);
                a.s_min_range.s_dex = random_value(10);
                a.s_min_range.s_int = random_value(10);

                a.s_max_range.s_str = random_value(10) + 25;
                a.s_max_range.s_dex = random_value(10) + 25;
                a.s_max_range.s_int = random_value(10) + 25;
            });
        }
        for(uint32_t i=0;i<3;++i)
        {
            head_rule.emplace(owner,[&](auto a)
            {
                a.h_head = head_rule.available_primary_key();
            });
            hair_rule.emplace(owner, [&](auto a) {
                a.h_hair = head_rule.available_primary_key();
            });
            body_rule.emplace(owner, [&](auto a) {
                a.b_body = head_rule.available_primary_key();
            });
        }
    }
};