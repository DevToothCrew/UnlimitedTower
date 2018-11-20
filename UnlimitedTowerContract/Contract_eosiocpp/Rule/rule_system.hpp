#pragma once
#include "../Common/common_header.hpp"
#include "../Rule/rule_servent_table.hpp"
#include "../Rule/rule_head_table.hpp"
#include "../Rule/rule_hair_table.hpp"
#include "../Rule/rule_body_table.hpp"
#include "../Rule/rule_item_id_table.hpp"
#include "../Rule/rule_item_tier_table.hpp"
#include "../Rule/rule_monster_grade_table.hpp"
#include "../Rule/rule_monster_id_table.hpp"

class crule_system
{
  private:
    account_name owner;

  private:
    servent_data_table  servent_rule;
    head_data_table head_rule;
    hair_data_table hair_rule;
    body_data_table body_rule;
    monster_grade_data_table monster_grade_rule;
    monster_id_data_table monster_id_rule;
    item_id_data_table item_id_rule;
    item_tier_data_table item_tier_rule;

  public:
    crule_system(account_name _self)
        : owner(_self),
          servent_rule(_self, _self),
          head_rule(_self,_self),
          hair_rule(_self,_self),
          body_rule(_self,_self),
          monster_grade_rule(_self,_self),
          monster_id_rule(_self,_self),
          item_id_rule(_self,_self),
          item_tier_rule(_self,_self)
    {
    }
    servent_data_table &get_servent_rule_table()
    {
        return servent_rule;
    }
    head_data_table &get_head_rule_table()
    {
        return head_rule;
    }
    hair_data_table &get_hair_rule_table()
    {
        return hair_rule;
    }
    body_data_table &get_body_rule_table()
    {
        return body_rule;
    }
    monster_grade_data_table &get_monster_grade_rule_table()
    {
        return monster_grade_rule;
    }
    monster_id_data_table &get_monster_id_rule_table()
    {
        return monster_id_rule;
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
            servent_rule.emplace(owner, [&](auto& a) {
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
            head_rule.emplace(owner,[&](auto& a)
            {
                a.h_head = head_rule.available_primary_key();
            });
            hair_rule.emplace(owner, [&](auto& a) {
                a.h_hair = head_rule.available_primary_key();
            });
            body_rule.emplace(owner, [&](auto& a) {
                a.b_body = head_rule.available_primary_key();
            });
        }
        for(uint32_t i=0;i<5;++i)
        {
            monster_grade_rule.emplace(owner,[&](auto &a)
            {
                a.m_grade = monster_grade_rule.available_primary_key();
                a.m_min_range.m_str = random_value(10);
                a.m_min_range.m_dex = random_value(10);
                a.m_min_range.m_int = random_value(10);

                a.m_max_range.m_str = random_value(10) + 25;
                a.m_max_range.m_dex = random_value(10) + 25;
                a.m_max_range.m_int = random_value(10) + 25;
            });

        }
        for(uint32_t i=0;i<30;++i)
        {
            monster_id_rule.emplace(owner,[&](auto &a)
            {
                a.m_id = monster_id_rule.available_primary_key();
            });
        }
        for (uint32_t i = 0; i < 70; ++i)
        {
            item_id_rule.emplace(owner, [&](auto &a) {
                a.i_id = item_id_rule.available_primary_key();
                if(i < 30)
                {
                a.i_type = random_value(2);
                }
                else
                {
                    a.i_type = random_value(3);
                }
            });
        }
        for(uint32_t i=1;i<=5;++i)
        {
            item_tier_rule.emplace(owner,[&](auto &a)
            {
                a.i_tier = item_tier_rule.available_primary_key();
                a.i_level = i * 10;
            });
        }
    }
};