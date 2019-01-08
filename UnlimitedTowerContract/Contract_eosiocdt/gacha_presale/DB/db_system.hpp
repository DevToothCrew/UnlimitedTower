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




    ACTION setdata()
    {
        eosio::require_auth(owner);
	    uint64_t l_seed = safeseed::get_seed(owner.value);
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

    ACTION initdata()
    {
        eosio::require_auth(owner);

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
