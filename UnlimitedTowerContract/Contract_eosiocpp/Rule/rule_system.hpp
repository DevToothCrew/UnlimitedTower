#pragma once
#include "../Common/common_header.hpp"
#include "../Rule/rule_servant_table.hpp"
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
    servant_data_table  servant_rule;
    head_data_table head_rule;
    hair_data_table hair_rule;
    body_data_table body_rule;
    monster_grade_data_table monster_grade_rule;
    monster_id_data_table monster_id_rule;
    item_id_data_table item_id_rule;
    item_tier_data_table item_tier_rule;
  public:
    const uint8_t servant_job_count = 6;
    const uint8_t monster_id_count = 30;
    const uint8_t monster_grade_count = 5;
    const uint8_t item_id_count = 70;
    const uint8_t item_tier_count = 5;
    const uint8_t item_type_count = 3;
    const uint8_t head_count = 3;
    const uint8_t hair_count = 3;
    const uint8_t body_count = 4;
    uint32_t random_count = 0;
  public:
    crule_system(account_name _self)
        : owner(_self),
          servant_rule(_self, _self),
          head_rule(_self,_self),
          hair_rule(_self,_self),
          body_rule(_self,_self),
          monster_grade_rule(_self,_self),
          monster_id_rule(_self,_self),
          item_id_rule(_self,_self),
          item_tier_rule(_self,_self)
    {
    }
    servant_data_table &get_servant_rule_table()
    {
        return servant_rule;
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
    item_id_data_table &get_item_id_rule_table()
    {
        return item_id_rule;
    }
    item_tier_data_table &get_item_tier_rule_table()
    {
        return item_tier_rule;
    }
    uint64_t random_seed(uint64_t _seed, uint32_t _range, uint32_t _min, uint32_t _random_count)
    {
        uint64_t l_result;
        uint64_t l_seed;

        l_seed = (_seed >> (2 * _random_count));
        l_result = l_seed % _range;
        // print("r_count : ", _random_count, "\n");
        // print("l_seed : ", l_seed, "\n");
        // print("l_result : ", l_result, "\n");
        // print("-------------------------------\n");
        if (l_result < _min)
        {
            return l_result += _min;
        }
        return l_result;
    }
    void init_data()
    {
        require_auth2(owner,N(owner));
        uint64_t l_seed = tapos_block_num() * tapos_block_prefix() * now();
        require_auth2(owner,N(owner));
        for (uint8_t i = 0; i < servant_job_count; ++i)
        {
            servant_rule.emplace(owner, [&](auto& a) {
                a.s_job = servant_rule.available_primary_key();
                if(random_count >= 8 )
                {
                    random_count = 0;
                }
                a.s_min_range.s_str = 1;
                a.s_min_range.s_dex = 1;
                a.s_min_range.s_int = 1;

                a.s_max_range.s_str = 99;
                a.s_max_range.s_dex = 99;
                a.s_max_range.s_int = 99;
            });
        }
        for(uint8_t i=0;i<head_count;++i)
        {
            head_rule.emplace(owner,[&](auto& a)
            {
                a.h_head = head_rule.available_primary_key();
            });
            hair_rule.emplace(owner, [&](auto& a) {
                a.h_hair = hair_rule.available_primary_key();
            });
            body_rule.emplace(owner, [&](auto& a) {
                a.b_body = body_rule.available_primary_key();
            });
        }
        for(uint8_t i=0;i<monster_grade_count;++i)
        {
            monster_grade_rule.emplace(owner,[&](auto &a)
            {
                a.m_grade = monster_grade_rule.available_primary_key();
                if (random_count >= 8)
                {
                    random_count = 0;
                }
                if(i == 0)
                {
                    a.m_min_range.m_str = 1;
                    a.m_min_range.m_dex = 1;
                    a.m_min_range.m_int = 1;

                    a.m_max_range.m_str = 5;
                    a.m_max_range.m_dex = 5;
                    a.m_max_range.m_int = 5;
                }
                else if(i==1)
                {
                     a.m_min_range.m_str = 6;
                    a.m_min_range.m_dex = 6;
                    a.m_min_range.m_int = 6;

                    a.m_max_range.m_str = 10;
                    a.m_max_range.m_dex = 10;
                    a.m_max_range.m_int = 10;
                }
                else if(i==2)
                {                    
                    a.m_min_range.m_str = 10;
                    a.m_min_range.m_dex = 10;
                    a.m_min_range.m_int = 10;

                    a.m_max_range.m_str = 50;
                    a.m_max_range.m_dex = 50;
                    a.m_max_range.m_int = 50;
                }
                else if(i==3)
                {
                    a.m_min_range.m_str = 30;
                    a.m_min_range.m_dex = 30;
                    a.m_min_range.m_int = 30;

                    a.m_max_range.m_str = 150;
                    a.m_max_range.m_dex = 150;
                    a.m_max_range.m_int = 150;
                }
                else
                {
                    a.m_min_range.m_str = 100;
                    a.m_min_range.m_dex = 100;
                    a.m_min_range.m_int = 100;

                    a.m_max_range.m_str = 500;
                    a.m_max_range.m_dex = 500;
                    a.m_max_range.m_int = 500;
                }


            });

        }
        for(uint8_t i=0;i<monster_id_count;++i)
        {
            monster_id_rule.emplace(owner,[&](auto &a)
            {
                a.m_id = monster_id_rule.available_primary_key();
            });
        }
        for (uint8_t i = 0; i < item_id_count; ++i)
        {
            item_id_rule.emplace(owner, [&](auto &a) {
                a.i_id = item_id_rule.available_primary_key();
                if (random_count >= 8)
                {
                    random_count = 0;
                }
                a.i_type = random_seed(l_seed,item_type_count,0,random_count++);
                a.i_job = random_seed(l_seed,servant_job_count,0,random_count++);
            });
        }
        for(uint32_t i=1;i<=item_tier_count;++i)
        {
            item_tier_rule.emplace(owner,[&](auto &a)
            {
                a.i_tier = item_tier_rule.available_primary_key();
                a.i_level = i * 10;
                if (random_count >= 8)
                {
                    random_count = 0;
                }
                a.i_min_range.i_str = random_seed(l_seed,10,0,random_count++);
                a.i_min_range.i_dex = random_seed(l_seed,10,0,random_count++);
                a.i_min_range.i_int = random_seed(l_seed,10,0,random_count++);

                a.i_max_range.i_str = random_seed(l_seed,10,0,random_count++) + 25;
                a.i_max_range.i_dex = random_seed(l_seed,10,0,random_count++) + 25;
                a.i_max_range.i_int = random_seed(l_seed,10,0,random_count++) + 25;
            });
        }
    }
};