#pragma once
#include "../Common/common_header.hpp"
#include "../Common/common_seed.hpp"
#include "../DB/db_seed_log.hpp"
#include "../DB/db_seed.hpp"
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
    seed_db     seed_db_table;
    seed_log_db seed_log_db_table;
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
    const uint8_t item_tier_count = 3;
    const uint8_t item_grade_count= 5;
    const uint8_t item_slot_count = 3;
    const uint8_t head_count = 3;
    const uint8_t hair_count = 3;
    const uint8_t body_count = 4;
    uint32_t random_count = 0;
  public:
    cdb_system(account_name _self)
        : owner(_self),
          seed_db_table(_self,_self),
          seed_log_db_table(_self,_self),
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
    seed_db &get_seed_table()
    {
        return seed_db_table;
    }

    seed_log_db &get_seed_log_table()
    {
        return seed_log_db_table;
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

    seed_db &get_seed_db_table()
    {
        return seed_db_table;
    }

    seed_log_db &get_seed_log_db_tabe()
    {
        return seed_log_db_table;
    }


    void init_db_data()
    {
        require_auth2(owner,N(owner));

        uint64_t l_seed = safeseed::get_seed(owner,now());
        for (uint8_t i = 0; i < servant_job_count; ++i)
        {
            servant_db_table.emplace(owner, [&](auto& a) {
                a.job = servant_db_table.available_primary_key();
                if(random_count >= 8 )
                {
                    random_count = 0;
                }
                a.min_range.base_str = DEFAULT_MIN;
                a.min_range.base_dex = DEFAULT_MIN;
                a.min_range.base_int = DEFAULT_MIN;

                a.max_range.base_str = DEFAULT_MAX;
                a.max_range.base_dex = DEFAULT_MAX;
                a.max_range.base_int = DEFAULT_MAX;
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
                a.monster_grade = monster_grade_db_table.available_primary_key();
                if (random_count >= 8)
                {
                    random_count = 0;
                }
                if(i == 0)
                {
                    a.min_range.base_str = DEFAULT_MIN;
                    a.min_range.base_dex = DEFAULT_MIN;
                    a.min_range.base_int = DEFAULT_MIN;

                    a.max_range.base_str = DEFAULT_MAX;
                    a.max_range.base_dex = DEFAULT_MAX;
                    a.max_range.base_int = DEFAULT_MAX;
                }
                else if(i==1)
                {
                    a.min_range.base_str = DEFAULT_MIN;
                    a.min_range.base_dex = DEFAULT_MIN;
                    a.min_range.base_int = DEFAULT_MIN;

                    a.max_range.base_str = DEFAULT_MAX;
                    a.max_range.base_dex = DEFAULT_MAX;
                    a.max_range.base_int = DEFAULT_MAX;
                }
                else if(i==2)
                {                    
                    a.min_range.base_str = DEFAULT_MIN;
                    a.min_range.base_dex = DEFAULT_MIN;
                    a.min_range.base_int = DEFAULT_MIN;

                    a.max_range.base_str = DEFAULT_MAX;
                    a.max_range.base_dex = DEFAULT_MAX;
                    a.max_range.base_int = DEFAULT_MAX;
                }
                else if(i==3)
                {
                    a.min_range.base_str = DEFAULT_MIN;
                    a.min_range.base_dex = DEFAULT_MIN;
                    a.min_range.base_int = DEFAULT_MIN;

                    a.max_range.base_str = DEFAULT_MAX;
                    a.max_range.base_dex = DEFAULT_MAX;
                    a.max_range.base_int = DEFAULT_MAX;
                }
                else
                {
                    a.min_range.base_str = 0;
                    a.min_range.base_dex = 0;
                    a.min_range.base_int = 0;

                    a.max_range.base_str = DEFAULT_MAX;
                    a.max_range.base_dex = DEFAULT_MAX;
                    a.max_range.base_int = DEFAULT_MAX;
                }


            });

        }
        for(uint8_t i=0;i<monster_id_count;++i)
        {
            monster_id_db_table.emplace(owner,[&](auto &a)
            {
                a.look = monster_id_db_table.available_primary_key();
            });
        }
        for (uint8_t i = 0; i < item_id_count; ++i)
        {
            item_id_db_table.emplace(owner, [&](auto &a) {
                a.id = item_id_db_table.available_primary_key();
                if (random_count >= 8)
                {
                    random_count = 0;
                }
                a.slot = safeseed::get_random_value(l_seed,item_slot_count,0,random_count);
                random_count+=1;
                a.job = safeseed::get_random_value(l_seed,servant_job_count,0,random_count);
                random_count+=1;
            });
        }
        for(uint32_t i=1;i<=item_tier_count;++i)
        {
            item_tier_db_table.emplace(owner,[&](auto &a)
            {
                a.tier = item_tier_db_table.available_primary_key();
                a.level = i * 10;
            });
        }
        for(uint32_t i=0;i<item_grade_count;++i)
        {
            item_grade_db_table.emplace(owner, [&](auto &a) {
                a.grade = item_grade_db_table.available_primary_key();
                if (random_count >= 8)
                {
                    random_count = 0;
                }
                a.min_range.base_str = DEFAULT_MIN;
                a.min_range.base_dex = DEFAULT_MIN;
                a.min_range.base_int = DEFAULT_MIN;
      

                a.max_range.base_str = DEFAULT_MAX;    
                a.max_range.base_dex = DEFAULT_MAX;      
                a.max_range.base_int = DEFAULT_MAX;      

            });
        }
    }

    void reset_db_seed_data()
    {
        for (auto seed_db_iter = seed_db_table.begin(); seed_db_iter != seed_db_table.end();)
        {
            auto iter = seed_db_table.find(seed_db_iter->primary_key());
            seed_db_iter++;
            seed_db_table.erase(iter);
        }
    }

    void reset_db_data()
    {
        require_auth2(owner,N(owner));

        for (auto servant_db_table_iter = servant_db_table.begin(); servant_db_table_iter != servant_db_table.end();)
        {
            auto iter = servant_db_table.find(servant_db_table_iter->primary_key());
            servant_db_table_iter++;
            servant_db_table.erase(iter);
        }

        for (auto head_db_table_iter = head_db_table.begin(); head_db_table_iter != head_db_table.end();)
        {
            auto iter = head_db_table.find(head_db_table_iter->primary_key());
            head_db_table_iter++;
            head_db_table.erase(iter);
        }

        for (auto hair_db_table_iter = hair_db_table.begin(); hair_db_table_iter != hair_db_table.end();)
        {
            auto iter = hair_db_table.find(hair_db_table_iter->primary_key());
            hair_db_table_iter++;
            hair_db_table.erase(iter);
        }

        for (auto body_db_table_iter = body_db_table.begin(); body_db_table_iter != body_db_table.end();)
        {
            auto iter = body_db_table.find(body_db_table_iter->primary_key());
            body_db_table_iter++;
            body_db_table.erase(iter);
        }

        for (auto monster_grade_db_table_iter = monster_grade_db_table.begin(); monster_grade_db_table_iter != monster_grade_db_table.end();)
        {
            auto iter = monster_grade_db_table.find(monster_grade_db_table_iter->primary_key());
            monster_grade_db_table_iter++;
            monster_grade_db_table.erase(iter);
        }

        for (auto monster_id_db_table_iter = monster_id_db_table.begin(); monster_id_db_table_iter != monster_id_db_table.end();)
        {
            auto iter = monster_id_db_table.find(monster_id_db_table_iter->primary_key());
            monster_id_db_table_iter++;
            monster_id_db_table.erase(iter);
        }

        for (auto item_id_db_table_iter = item_id_db_table.begin(); item_id_db_table_iter != item_id_db_table.end();)
        {
            auto iter = item_id_db_table.find(item_id_db_table_iter->primary_key());
            item_id_db_table_iter++;
            item_id_db_table.erase(iter);
        }

        for (auto item_grade_db_table_iter = item_grade_db_table.begin(); item_grade_db_table_iter != item_grade_db_table.end();)
        {
            auto iter = item_grade_db_table.find(item_grade_db_table_iter->primary_key());
            item_grade_db_table_iter++;
            item_grade_db_table.erase(iter);
        }

        for (auto item_tier_db_table_iter = item_tier_db_table.begin(); item_tier_db_table_iter != item_tier_db_table.end();)
        {
            auto iter = item_tier_db_table.find(item_tier_db_table_iter->primary_key());
            item_tier_db_table_iter++;
            item_tier_db_table.erase(iter);
        }
    }
};