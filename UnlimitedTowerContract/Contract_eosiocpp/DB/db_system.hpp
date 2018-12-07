#pragma once
#include "../Common/common_header.hpp"
#include "../Common/common_seed.hpp"
#include "../DB/db_servant.hpp"
#include "../DB/db_head.hpp"
#include "../DB/db_hair.hpp"
#include "../DB/db_body.hpp"
#include "../DB/db_item_id.hpp"
#include "../DB/db_item_tier.hpp"
#include "../DB/db_item_grade.hpp"
#include "../DB/db_monster_grade.hpp"
#include "../DB/db_monster_id.hpp"



class cdb_system
{
  private:
    account_name owner;

  private:
    servant_db  servant_db_table;
    head_db head_db_table;
    hair_db hair_db_table;
    body_db body_db_table;
    monster_grade_db monster_grade_db_table;
    monster_id_db monster_id_db_table;
    item_id_db item_id_db_table;
    item_tier_db item_tier_db_table;
    item_grade_db item_grade_db_table;
  public:
    const uint8_t servant_job_count = 6;
    const uint8_t monster_id_count = 30;
    const uint8_t monster_grade_count = 5;
    const uint8_t item_id_count = 70;
    const uint8_t item_tier_count = 4;
    const uint8_t item_grade_count= 5;
    const uint8_t item_slot_count = 3;
    const uint8_t head_count = 3;
    const uint8_t hair_count = 3;
    const uint8_t body_count = 4;
    uint32_t random_count = 0;
  public:
    cdb_system(account_name _self)
        : owner(_self),
          servant_db_table(_self, _self),
          head_db_table(_self,_self),
          hair_db_table(_self,_self),
          body_db_table(_self,_self),
          monster_grade_db_table(_self,_self),
          monster_id_db_table(_self,_self),
          item_id_db_table(_self,_self),
          item_tier_db_table(_self,_self),
          item_grade_db_table(_self,_self)
    {
    }
    servant_db &get_servant_db_table()
    {
        return servant_db_table;
    }
    head_db &get_head_db_table()
    {
        return head_db_table;
    }
    hair_db &get_hair_db_table()
    {
        return hair_db_table;
    }
    body_db &get_body_db_table()
    {
        return body_db_table;
    }
    monster_grade_db &get_monster_grade_db_table()
    {
        return monster_grade_db_table;
    }
    monster_id_db &get_monster_id_db_table()
    {
        return monster_id_db_table;
    }
    item_id_db &get_item_id_db_table()
    {
        return item_id_db_table;
    }
    item_tier_db &get_item_tier_db_table()
    {
        return item_tier_db_table;
    }
    item_grade_db &get_item_grade_db_table()
    {
        return item_grade_db_table;
    }
    void init_data(account_name _user)
    {
        require_auth2(owner,N(owner));
        uint64_t l_seed = safeseed::get_seed(_user);
        for (uint8_t i = 0; i < servant_job_count; ++i)
        {
            servant_db_table.emplace(owner, [&](auto& a) {
                a.s_job = servant_db_table.available_primary_key();
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
            head_db_table.emplace(owner,[&](auto& a)
            {
                a.head = head_db_table.available_primary_key();
            });
            hair_db_table.emplace(owner, [&](auto& a) {
                a.hair = hair_db_table.available_primary_key();
            });

        }
        for (uint8_t i = 0; i < body_count; ++i)
        {
        body_db_table.emplace(owner, [&](auto &a) {
            a.body = body_db_table.available_primary_key();
        });
        }


        for(uint8_t i=0;i<monster_grade_count;++i)
        {
            monster_grade_db_table.emplace(owner,[&](auto &a)
            {
                a.m_grade = monster_grade_db_table.available_primary_key();
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
            monster_id_db_table.emplace(owner,[&](auto &a)
            {
                a.m_id = monster_id_db_table.available_primary_key();
            });
        }
        for (uint8_t i = 0; i < item_id_count; ++i)
        {
            item_id_db_table.emplace(owner, [&](auto &a) {
                a.i_id = item_id_db_table.available_primary_key();
                if (random_count >= 8)
                {
                    random_count = 0;
                }
                a.i_slot = safeseed::random_seed(safeseed::get_seed(_user),item_slot_count,0,random_count++);
                a.i_job = safeseed::random_seed(safeseed::get_seed(_user),servant_job_count,0,random_count++);
            });
        }
        for(uint32_t i=1;i<=item_tier_count;++i)
        {
            item_tier_db_table.emplace(owner,[&](auto &a)
            {
                a.i_tier = item_tier_db_table.available_primary_key();
                a.i_level = i * 10;
            });
        }
        for(uint32_t i=0;i<item_grade_count;++i)
        {
            item_grade_db_table.emplace(owner, [&](auto &a) {
                a.i_grade = item_grade_db_table.available_primary_key();
                if (random_count >= 8)
                {
                    random_count = 0;
                }
                a.i_min_range.i_str = safeseed::random_seed(safeseed::get_seed(_user), 10, 0, random_count++);
                a.i_min_range.i_dex = safeseed::random_seed(safeseed::get_seed(_user), 10, 0, random_count++);
                a.i_min_range.i_int = safeseed::random_seed(safeseed::get_seed(_user), 10, 0, random_count++);

                a.i_max_range.i_str = safeseed::random_seed(safeseed::get_seed(_user), 10, 0, random_count++) + 25;
                a.i_max_range.i_dex = safeseed::random_seed(safeseed::get_seed(_user), 10, 0, random_count++) + 25;
                a.i_max_range.i_int = safeseed::random_seed(safeseed::get_seed(_user), 10, 0, random_count++) + 25;
            });
        }
    }
};