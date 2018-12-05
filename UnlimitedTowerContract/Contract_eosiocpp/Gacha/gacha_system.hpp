#pragma once
#include "../Common/common_header.hpp"
#include "../Common/common_seed.hpp"
class cgacha_system
    {
    private:
        account_name owner;
        clogin_system &login_controller;
        cdb_system &db_controller;
        user_monsters user_monster_table;
        user_servants user_servant_table;
        user_items user_item_table;

        uint32_t servant_random_count;
        uint32_t monster_random_count;
        uint32_t item_random_count;

    public:
        const uint32_t default_min = 0;
        const uint32_t max_rate = 100000;
        const uint32_t grade_three_rate = 89000;
        const uint32_t grade_four_rate = 9000;
        const uint32_t grade_five_rate = 2000;

    public:
        cgacha_system(account_name _self,
        clogin_system &_login_controller,
        cdb_system &_db_controller) 
        : owner(_self),
        login_controller(_login_controller),
        db_controller(_db_controller),
        user_monster_table(_self,_self),
        user_servant_table(_self,_self),
        user_item_table(_self,_self)
        {
            servant_random_count=0;
            monster_random_count=0;
            item_random_count=0;
        }
        user_monsters &get_user_monster_table()
        {
            return user_monster_table;
        }
        user_servants &get_user_servant_table()
        {
            return user_servant_table;
        }
        user_items &get_user_item_table()
        {
            return user_item_table;
        }
        void user_own_object_init(account_name _user)
        {
            user_servant_table.emplace(owner, [&](auto &new_servant) {
                new_servant.servant_set_user(_user);
            });
            user_monster_table.emplace(owner, [&](auto &new_monster) {
                new_monster.monster_set_user(_user);
            });
            user_item_table.emplace(owner, [&](auto &new_item) {
                new_item.item_set_user(_user);
            });
        }
        uint64_t random_seed(uint64_t _seed,uint32_t _max,uint32_t _min,uint32_t _random_count)
        {
            _seed= ((_seed) >> (2 * _random_count));
            uint64_t l_result = _seed % _max;
            if(l_result < _min)
            {
                return l_result+=_min;
            }
            return l_result;
        }
        void gacha_servant_job(account_name _user,uint64_t _seed)
        {
            uint8_t random_job = random_seed(_seed,db_controller.servant_job_count,default_min,servant_random_count++);
            auto &servant_job_db = db_controller.get_servant_db_table();
            const auto &servant_db_iter = servant_job_db.get(random_job,"not get servant job data");

            auto &user_log_table = login_controller.get_log_table();
            auto user_log_iter = user_log_table.find(_user);

            auto user_servant_list_iter = user_servant_table.find(_user);
            user_servant_table.modify(user_servant_list_iter, owner, [&](auto &update_user_servant_list) {
                cservantinfo new_servant;
                new_servant.s_index = user_log_iter->l_servant_num + 1;
                new_servant.s_appear.hair = gacha_servant_hair(_seed,servant_random_count++);
                new_servant.s_appear.hair = gacha_servant_head(_seed,servant_random_count++);
                new_servant.s_appear.body = gacha_servant_body(_seed,servant_random_count++);
                new_servant.s_status.job = servant_db_iter.s_job;
                new_servant.s_status.basic_str = random_seed(_seed,servant_db_iter.s_max_range.s_str,servant_db_iter.s_min_range.s_str,servant_random_count++);
                new_servant.s_status.basic_dex = random_seed(_seed,servant_db_iter.s_max_range.s_dex,servant_db_iter.s_min_range.s_dex,servant_random_count++);
                new_servant.s_status.basic_int = random_seed(_seed,servant_db_iter.s_max_range.s_int,servant_db_iter.s_min_range.s_int,servant_random_count++);
                new_servant.s_equip_slot.resize(3);
                
                update_user_servant_list.s_servant_list.push_back(new_servant);
            });

            //로그 남기는 부분
            user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
                update_log.l_servant_num++;
                update_log.l_gacha_num++;
            });

        }
        uint8_t gacha_servant_head(uint64_t _seed,uint32_t _count)
        {
            uint8_t random_head = random_seed(_seed,db_controller.head_count,default_min,_count);
            auto &servant_head_db = db_controller.get_head_db_table();
            const auto &head_db_iter = servant_head_db.get(random_head,"not exist head info");
            return head_db_iter.h_head;
        }
        uint8_t gacha_servant_hair(uint64_t _seed,uint32_t _count)
        {
            uint8_t random_hair = random_seed(_seed,db_controller.hair_count,default_min,_count);
            auto &servant_hair_db = db_controller.get_hair_db_table();
            const auto &hair_db_iter = servant_hair_db.get(random_hair,"not exist hair info");
            return hair_db_iter.h_hair;
        }
        uint8_t gacha_servant_body(uint64_t _seed,uint32_t _count)
        {
            uint8_t random_body = random_seed(_seed,db_controller.body_count,default_min,_count);
            auto &servant_body_db = db_controller.get_body_db_table();
            const auto &body_db_iter = servant_body_db.get(random_body, "not exist body info");
            return body_db_iter.b_body;
        }
        //---------------------------------------------------------------------------------//
        void gacha_monster_id(account_name _user,uint64_t _seed)
        {   
            uint8_t random_monster_id = random_seed(_seed,db_controller.monster_id_count,default_min,monster_random_count++);
            auto &monster_id_db = db_controller.get_monster_id_db_table();
            const auto &monster_id_db_iter = monster_id_db.get(random_monster_id,"not exist monster id");

            uint32_t random_rate = random_seed(_seed,max_rate,default_min,monster_random_count++);
            uint8_t random_grade;
            if(random_rate < grade_five_rate)
            {
                random_grade = 4;
            }
            else if(random_rate < grade_four_rate)
            {
                random_grade = 3;
            }
            else
            {
                random_grade = 2;
            }

            auto &monster_grade_db = db_controller.get_monster_grade_db_table();
            const auto &grade_db_iter = monster_grade_db.get(random_grade,"not exist monster grade");

            auto &user_log_table = login_controller.get_log_table();
            auto user_log_iter = user_log_table.find(_user);

            auto user_monster_list_iter = user_monster_table.find(_user);
            user_monster_table.modify(user_monster_list_iter, owner, [&](auto &update_user_monster_list) {
                cmonsterinfo new_monster;
                new_monster.m_index = user_log_iter->l_monster_num + 1;
                new_monster.m_type = monster_id_db_iter.m_id;
                new_monster.m_status.basic_str = random_seed(_seed,grade_db_iter.m_max_range.m_str,grade_db_iter.m_min_range.m_str,monster_random_count++);
                new_monster.m_status.basic_dex = random_seed(_seed,grade_db_iter.m_max_range.m_dex,grade_db_iter.m_min_range.m_dex,monster_random_count++);
                new_monster.m_status.basic_int = random_seed(_seed,grade_db_iter.m_max_range.m_int,grade_db_iter.m_min_range.m_int,monster_random_count++);
                update_user_monster_list.m_monster_list.push_back(new_monster);
            });

            user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
                update_log.l_gacha_num++;
                update_log.l_monster_num++;
            });
        }
        //-----------------------------------------------------------------------------//
        void gacha_item_id(account_name _user,uint64_t _seed)
        {
            uint8_t random_item_id = random_seed(_seed,db_controller.item_id_count,default_min,item_random_count++);
            auto &item_id_db = db_controller.get_item_id_db_table();
            const auto &item_id_db_iter = item_id_db.get(random_item_id, "not exist item id");

            uint8_t random_item_tier = random_seed(_seed,db_controller.item_tier_count,default_min,item_random_count++);
            auto &item_tier_db = db_controller.get_item_tier_db_table();
            const auto &item_tier_db_iter = item_tier_db.get(random_item_tier,"not exist tier info");

            uint8_t random_item_grade = random_seed(_seed,db_controller.item_grade_count,default_min,item_random_count++);
            auto &item_grade_db = db_controller.get_item_grade_db_table();
            const auto &item_grade_db_iter = item_grade_db.get(random_item_grade,"not exist tier info");

            auto &user_log_table = login_controller.get_log_table();
            auto user_log_iter = user_log_table.find(_user);

            auto user_item_list_iter = user_item_table.find(_user);
            user_item_table.modify(user_item_list_iter, owner, [&](auto &update_user_item_list) {
                citeminfo new_item;
                new_item.i_index = user_log_iter->l_item_num + 1;
                new_item.i_id = item_id_db_iter.i_id;
                new_item.i_slot = item_id_db_iter.i_slot;
                new_item.i_tier = item_tier_db_iter.i_tier;
                new_item.i_status.basic_str = random_seed(_seed,item_grade_db_iter.i_max_range.i_str,item_grade_db_iter.i_min_range.i_str,item_random_count++);
                new_item.i_status.basic_dex = random_seed(_seed,item_grade_db_iter.i_max_range.i_dex,item_grade_db_iter.i_min_range.i_dex,item_random_count++);
                new_item.i_status.basic_int = random_seed(_seed,item_grade_db_iter.i_max_range.i_int,item_grade_db_iter.i_min_range.i_int,item_random_count++);
                new_item.i_status.job = item_id_db_iter.i_job;
                new_item.i_state = item_state::item_inventory;
                new_item.i_grade = item_grade_db_iter.i_grade;
                update_user_item_list.i_item_list.push_back(new_item);
            });

            user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
                update_log.l_item_num++;
                update_log.l_gacha_num++;
            });
        }
        void start_gacha(account_name _user)
        {
            auto &user_log_table = login_controller.get_log_table();
            auto user_log_iter = user_log_table.find(_user);
            eosio_assert(user_log_iter != user_log_table.end(),"unknown account");

            uint64_t l_seed = safeseed::get_seed(_user);

            if(user_log_iter->l_gacha_num == 0)
            {
                gacha_monster_id(_user,l_seed);
            }
            else
            {
                uint64_t l_gacha_result_type = random_seed(l_seed,100000,default_min,0);
                if(l_gacha_result_type < 33333)
                {
                    gacha_servant_job(_user,l_seed);
                }
                else if(l_gacha_result_type > 33333 && l_gacha_result_type <= 66666)
                {
                    gacha_monster_id(_user,l_seed);
                }
                else
                {
                    gacha_item_id(_user,l_seed);
                }
            }
            servant_random_count = 0;
            monster_random_count = 0;
            item_random_count = 0;
        }
#pragma region reset
        void reset_all_user_object_data()
        {
            require_auth2(owner, N(owner));
            for (auto user_servant_iter = user_servant_table.begin(); user_servant_iter != user_servant_table.end();)
            {
                auto iter = user_servant_table.find(user_servant_iter->primary_key());
                user_servant_iter++;
                user_servant_table.erase(iter);
            }

            for (auto user_monster_iter = user_monster_table.begin(); user_monster_iter != user_monster_table.end();)
            {
                auto iter = user_monster_table.find(user_monster_iter->primary_key());
                user_monster_iter++;
                user_monster_table.erase(iter);
            }

            for (auto user_item_iter = user_item_table.begin(); user_item_iter != user_item_table.end();)
            {
                auto iter = user_item_table.find(user_item_iter->primary_key());
                user_item_iter++;
                user_item_table.erase(iter);
            }
        }
#pragma endregion
    };