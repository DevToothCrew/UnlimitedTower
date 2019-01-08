#pragma once
#include "../Common/common_header.hpp"
#include "../Common/common_seed.hpp"



        void gacha_servant_job(eosio::name _user,uint64_t _seed)
        {
            uint8_t random_job = safeseed::get_random_value(_seed,db_controller.servant_job_count,default_min,servant_random_count);
            const auto &servant_db_iter = servant_db_table.get(random_job,"not get servant job data");

            auto user_log_iter = user_log_table.find(_user.value);
            eosio_assert(user_log_iter != user_log_table.end(),"not exist user log data");

            result_info result;
            user_servants user_servant_table(owner, _user.value);
            user_servant_table.emplace( owner, [&](auto &update_user_servant_list) {
                uint32_t first_index = user_servant_table.available_primary_key();
                if (first_index == 0)
                {
                    update_user_servant_list.index = 1;
                }
                else
                {
                    update_user_servant_list.index = user_servant_table.available_primary_key();
                }
                
                servant_info new_servant;
                servant_random_count+=1;
                new_servant.appear.hair = gacha_servant_hair(_seed,servant_random_count);
                servant_random_count+=1;
                new_servant.appear.hair = gacha_servant_head(_seed,servant_random_count);
                servant_random_count+=1;
                new_servant.appear.body = gacha_servant_body(_seed,servant_random_count);
                new_servant.job = servant_db_iter.job;
                servant_random_count+=1;
                new_servant.status.basic_str = safeseed::get_random_value(_seed,servant_db_iter.max_range.base_str,servant_db_iter.min_range.base_str,servant_random_count);
                servant_random_count+=1;
                new_servant.status.basic_dex = safeseed::get_random_value(_seed,servant_db_iter.max_range.base_dex,servant_db_iter.min_range.base_dex,servant_random_count);
                servant_random_count+=1;
                new_servant.status.basic_int = safeseed::get_random_value(_seed,servant_db_iter.max_range.base_int,servant_db_iter.min_range.base_int,servant_random_count);
                new_servant.equip_slot.resize(3);
                new_servant.state = eobject_state::on_inventory;

                result.index = update_user_servant_list.index;
                result.type = result::servant;
                
                update_user_servant_list.servant = new_servant;
            });

            auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
            if(user_gacha_result_iter == user_gacha_result_table.end())
            {
                user_gacha_result_table.emplace(owner, [&](auto &new_result)
                {
                    new_result.user = _user.value;
                    new_result.result = result;
                });
            }
            else
            {
                user_gacha_result_table.modify(user_gacha_result_iter, owner, [&](auto &new_result)
                {
                    new_result.result = result;
                });
            }

            auto user_gacha_accumulate_iter = user_gacha_accumulate_table.find(_user.value);
            if(user_gacha_accumulate_iter == user_gacha_accumulate_table.end())
            {
                user_gacha_accumulate_table.emplace(owner, [&](auto &new_result)
                {
                    new_result.user = _user.value;
                    new_result.result_list.push_back(result);
                });
            }
            else
            {
                user_gacha_accumulate_table.modify(user_gacha_accumulate_iter, owner, [&](auto &new_result)
                {
                    new_result.result_list.push_back(result);
                });
            }


            //로그 남기는 부분
            user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
                update_log.servant_num++;
                update_log.gacha_num++;
            });
        }

        uint8_t gacha_servant_head(uint64_t _seed,uint32_t _count)
        {
            uint8_t random_head = safeseed::get_random_value(_seed,db_controller.head_count,default_min,_count);
            const auto &head_db_iter = head_db_table.get(random_head,"not exist head info");
            return head_db_iter.head;
        }

        uint8_t gacha_servant_hair(uint64_t _seed,uint32_t _count)
        {
            uint8_t random_hair = safeseed::get_random_value(_seed,db_controller.hair_count,default_min,_count);
            const auto &hair_db_iter = hair_db_table.get(random_hair,"not exist hair info");
            return hair_db_iter.hair;
        }

        uint8_t gacha_servant_body(uint64_t _seed,uint32_t _count)
        {
            uint8_t random_body = safeseed::get_random_value(_seed,db_controller.body_count,default_min,_count);
            const auto &body_db_iter = body_db_table.get(random_body, "not exist body info");
            return body_db_iter.body;
        }

        void gacha_monster_id(eosio::name _user,uint64_t _seed)
        {   
            uint8_t random_monster_id = safeseed::get_random_value(_seed,db_controller.monster_id_count,default_min,monster_random_count);
            const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id,"not exist monster id");

            monster_random_count+=1;
            uint32_t random_rate = safeseed::get_random_value(_seed,max_rate,default_min,monster_random_count);
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

            const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade,"not exist monster grade");

            auto user_log_iter = user_log_table.find(_user.value);
            eosio_assert(user_log_iter != user_log_table.end(),"not exist user log data");

            result_info result;
            user_monsters user_monster_table(owner, _user.value);
            user_monster_table.emplace(owner, [&](auto &update_user_monster_list) {
                uint32_t first_index = user_monster_table.available_primary_key();
                if (first_index == 0)
                {
                    update_user_monster_list.index = 1;
                }
                else
                {
                    update_user_monster_list.index = user_monster_table.available_primary_key();
                }

                monster_info new_monster;
                new_monster.look = monster_id_db_iter.look;
                new_monster.grade = monster_grade_db_iter.monster_grade;
                monster_random_count+=1;
                new_monster.status.basic_str = safeseed::get_random_value(_seed,monster_grade_db_iter.max_range.base_str,monster_grade_db_iter.min_range.base_str,monster_random_count);
                monster_random_count+=1;
                new_monster.status.basic_dex = safeseed::get_random_value(_seed,monster_grade_db_iter.max_range.base_dex,monster_grade_db_iter.min_range.base_dex,monster_random_count);
                monster_random_count+=1;
                new_monster.status.basic_int = safeseed::get_random_value(_seed,monster_grade_db_iter.max_range.base_int,monster_grade_db_iter.min_range.base_int,monster_random_count);
                new_monster.state = eobject_state::on_inventory;

                result.index = update_user_monster_list.index;
                result.type = result::monster;
                
                update_user_monster_list.monster = new_monster;
            });

            auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
            if(user_gacha_result_iter == user_gacha_result_table.end())
            {
                user_gacha_result_table.emplace(owner, [&](auto &new_result)
                {
                    new_result.user = _user.value;
                    new_result.result = result;
                });
            }
            else
            {
                user_gacha_result_table.modify(user_gacha_result_iter, owner, [&](auto &new_result)
                {
                    new_result.result = result;
                });
            }


            auto user_gacha_accumulate_iter = user_gacha_accumulate_table.find(_user.value);
            if(user_gacha_accumulate_iter == user_gacha_accumulate_table.end())
            {
                user_gacha_accumulate_table.emplace(owner, [&](auto &new_result)
                {
                    new_result.user = _user.value;
                    new_result.result_list.push_back(result);
                });
            }
            else
            {
                user_gacha_accumulate_table.modify(user_gacha_accumulate_iter, owner, [&](auto &new_result)
                {
                    new_result.result_list.push_back(result);
                });
            }

            user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
                update_log.gacha_num++;
                update_log.monster_num++;
            });
        }

        void gacha_item_id(eosio::name _user,uint64_t _seed)
        {
            uint8_t random_item_id = safeseed::get_random_value(_seed,db_controller.item_id_count,default_min,item_random_count);
            const auto &item_id_db_iter = item_id_db_table.get(random_item_id, "not exist item id");

            item_random_count+=1;
            uint8_t random_item_tier = safeseed::get_random_value(_seed,db_controller.item_tier_count,default_min,item_random_count);
            const auto &item_tier_db_iter = item_tier_db_table.get(random_item_tier,"not exist tier info");

            item_random_count+=1;
            uint8_t random_item_grade = safeseed::get_random_value(_seed,db_controller.item_grade_count,default_min,item_random_count);
            const auto &item_grade_db_iter = item_grade_db_table.get(random_item_grade,"not exist tier info");


            auto user_log_iter = user_log_table.find(_user.value);
            eosio_assert(user_log_iter != user_log_table.end(),"not exist user log data");

            result_info result;
            user_items user_item_table(owner, _user.value);
            user_item_table.emplace(owner, [&](auto &update_user_item_list) {
                uint32_t first_index = user_item_table.available_primary_key();
                if(first_index == 0)
                {
                    update_user_item_list.index = 1;
                }
                else
                {
                    update_user_item_list.index = user_item_table.available_primary_key();
                }

                item_info new_item;
                new_item.id = item_id_db_iter.id;
                new_item.slot = item_id_db_iter.slot;
                new_item.tier = item_tier_db_iter.tier;
                item_random_count+=1;
                new_item.status.basic_str = safeseed::get_random_value(_seed,item_grade_db_iter.max_range.base_str,item_grade_db_iter.min_range.base_str,item_random_count);
                item_random_count+=1;
                new_item.status.basic_dex = safeseed::get_random_value(_seed,item_grade_db_iter.max_range.base_dex,item_grade_db_iter.min_range.base_dex,item_random_count);
                item_random_count+=1;
                new_item.status.basic_int = safeseed::get_random_value(_seed,item_grade_db_iter.max_range.base_int,item_grade_db_iter.min_range.base_int,item_random_count);
                new_item.job = item_id_db_iter.job;
                new_item.state = eobject_state::on_inventory;
                new_item.grade = item_grade_db_iter.grade;

                result.index = update_user_item_list.index;
                result.type = result::item;

                update_user_item_list.item = new_item;
            });


            auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
            if(user_gacha_result_iter == user_gacha_result_table.end())
            {
                user_gacha_result_table.emplace(owner, [&](auto &new_result)
                {
                    new_result.user = _user.value;
                    new_result.result = result;
                });
            }
            else
            {
                user_gacha_result_table.modify(user_gacha_result_iter, owner, [&](auto &new_result)
                {
                    new_result.result = result;
                });
            }

            auto user_gacha_accumulate_iter = user_gacha_accumulate_table.find(_user.value);
            if(user_gacha_accumulate_iter == user_gacha_accumulate_table.end())
            {
                user_gacha_accumulate_table.emplace(owner, [&](auto &new_result)
                {
                    new_result.user = _user.value;
                    new_result.result_list.push_back(result);
                });
            }
            else
            {
                user_gacha_accumulate_table.modify(user_gacha_accumulate_iter, owner, [&](auto &new_result)
                {
                    new_result.result_list.push_back(result);
                });
            }


            user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
                update_log.item_num++;
                update_log.gacha_num++;
            });
        }

        void start_gacha(eosio::name _user)
        {
            auto user_log_iter = user_log_table.find(_user.value);
            eosio_assert(user_log_iter != user_log_table.end(),"unknown account");

            uint64_t l_seed = safeseed::get_seed(_user.value);

            if(user_log_iter->gacha_num == 0)
            {
                gacha_monster_id(_user,l_seed);
            }
            else
            {
                uint64_t l_gacha_result_type = safeseed::get_random_value(l_seed,max_rate,default_min,DEFAULE_RANDOM_COUNT);
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
        //void reset_all_user_object_data(eosio::name _user)
	void reset_all_user_object_data(uint64_t _user)
        {
            user_servants user_servant_table(owner, _user.value);
            for (auto user_servant_iter = user_servant_table.begin(); user_servant_iter != user_servant_table.end();)
            {
                auto iter = user_servant_table.find(user_servant_iter->primary_key());
                user_servant_iter++;
                user_servant_table.erase(iter);
            }

            user_monsters user_monster_table(owner, _user.value);
            for (auto user_monster_iter = user_monster_table.begin(); user_monster_iter != user_monster_table.end();)
            {
                auto iter = user_monster_table.find(user_monster_iter->primary_key());
                user_monster_iter++;
                user_monster_table.erase(iter);
            }

            user_items user_item_table(owner, _user.value);
            for (auto user_item_iter = user_item_table.begin(); user_item_iter != user_item_table.end();)
            {
                auto iter = user_item_table.find(user_item_iter->primary_key());
                user_item_iter++;
                user_item_table.erase(iter);
            }
        }
        void reset_user_gacha_result_data(eosio::name _user)
        {
            auto iter = user_gacha_result_table.find(_user.value);
            eosio_assert(iter!=user_gacha_result_table.end(),"not exist gacha result data");
            user_gacha_result_table.erase(iter);

            auto accumulate_iter = user_gacha_accumulate_table.find(_user.value);
            eosio_assert(accumulate_iter != user_gacha_accumulate_table.end(), "not exist gacha accumulate data");
            user_gacha_accumulate_table.erase(accumulate_iter);
        }

        void reset_all_object_gacha_data()
        {
            require_auth(owner);
            auto &user_auth_table = login_controller.get_auth_user_table();
            for (auto user_name_iter = user_auth_table.begin(); user_name_iter != user_auth_table.end();)
            {
                reset_all_user_object_data(user_name_iter->primary_key());
                reset_user_gacha_result_data(user_name_iter->primary_key());
            }
        }

        void delete_object_data(uint64_t _user)
        {
            require_auth(owner);
            user_servants user_servant_table(owner, _user);
            for (auto user_servant_iter = user_servant_table.begin(); user_servant_iter != user_servant_table.end();)
            {
                auto iter = user_servant_table.find(user_servant_iter->primary_key());
                user_servant_iter++;
                user_servant_table.erase(iter);
            }

            user_monsters user_monster_table(owner, _user);
            for (auto user_monster_iter = user_monster_table.begin(); user_monster_iter != user_monster_table.end();)
            {
                auto iter = user_monster_table.find(user_monster_iter->primary_key());
                user_monster_iter++;
                user_monster_table.erase(iter);
            }

            user_items user_item_table(owner, _user);
            for (auto user_item_iter = user_item_table.begin(); user_item_iter != user_item_table.end();)
            {
                auto iter = user_item_table.find(user_item_iter->primary_key());
                user_item_iter++;
                user_item_table.erase(iter);
            }
        }
#pragma endregion

#pragma region gacha cheat
        void gacha_cheat(eosio::name _user)
        {
            require_auth(_user);
            uint64_t l_seed = safeseed::get_seed(_user.value);
            for(uint32_t i=0;i<5;++i)
            {
                if(i < 4)
                {
                    gacha_servant_job(_user,l_seed);
                }
                gacha_monster_id(_user,l_seed);
            }
        }
#pragma endregion
    };