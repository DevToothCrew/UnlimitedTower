#pragma once
#include "../Common/common_header.hpp"
class cgacha_system
    {
    private:
        account_name owner;
        clogin_system &login_controller;
        crule_system &rule_controller;
        user_monster_table monsters;
        user_servant_table servants;
        user_item_table items;
        uint32_t random_count;
        const uint32_t not_exist_min = 0;
    public:
        const uint32_t max_rate = 100000;
        const uint32_t grade_three_rate = 89000;
        const uint32_t grade_four_rate = 9000;
        const uint32_t grade_five_rate = 2000;
    public:
        cgacha_system(account_name _self,
        clogin_system &_login_controller,
        crule_system &_rule_controller) 
        : owner(_self),
        login_controller(_login_controller),
        rule_controller(_rule_controller),
        monsters(_self,_self),
        servants(_self,_self),
        items(_self,_self)
        {
            random_count = 0;
        }
        user_monster_table &get_monster_table()
        {
            return monsters;
        }
        user_servant_table &get_servant_table()
        {
            return servants;
        }
        user_item_table &get_item_table()
        {
            return items;
        }
        void user_own_object_init(account_name _user)
        {
            servants.emplace(owner, [&](auto &new_servant) {
                new_servant.servant_set_user(_user);
            });
            monsters.emplace(owner, [&](auto &new_monster) {
                new_monster.monster_set_user(_user);
            });
            items.emplace(owner, [&](auto &new_item) {
                new_item.item_set_user(_user);
            });
        }
        uint64_t random_seed(uint64_t _seed,uint32_t _range,uint32_t _min,uint32_t _random_count)
        {
            uint64_t l_result;
            uint64_t l_seed;

            l_seed = (_seed >> (2 * _random_count));
            l_result = l_seed % _range;
            print("r_count : ", _random_count, "\n");
            print("l_seed : ", l_seed, "\n");
            print("l_result : ", l_result, "\n");
            print("-------------------------------\n");
            if(l_result < _min)
            {
                return l_result+=_min;
            }
            return l_result;
        }
        void gacha_servant_job(account_name _user,uint64_t _seed)
        {
            uint8_t result_job = random_seed(_seed,rule_controller.servant_job_count,not_exist_min,random_count++);
            auto &servant_job = rule_controller.get_servant_rule_table();
            const auto &job_iter = servant_job.get(result_job,"not exist servant job");

            auto &log = login_controller.get_log_table();
            auto find_log_iter = log.find(_user);

            auto cur_user_servant = servants.find(_user);
            //eosio_assert(cur_user_servant->s_servant_list.size()<find_log_iter->l_servant_slot_count,"need more slot");
            servants.modify(cur_user_servant, owner, [&](auto &servant) {
                cservantinfo ser;
                ser.s_index = find_log_iter->l_servant_num + 1;
                ser.appear_info.hair = gacha_servant_hair(_seed,random_count++);
                ser.appear_info.hair = gacha_servant_head(_seed,random_count++);
                ser.appear_info.body = gacha_servant_body(_seed,random_count++);
                ser.status_info.job = job_iter.s_job;
                ser.status_info.basic_str = random_seed(_seed,job_iter.s_max_range.s_str,job_iter.s_min_range.s_str,random_count++);
                ser.status_info.basic_dex = random_seed(_seed,job_iter.s_max_range.s_dex,job_iter.s_min_range.s_dex,random_count++);
                ser.status_info.basic_int = random_seed(_seed,job_iter.s_max_range.s_int,job_iter.s_min_range.s_int,random_count++);
                ser.s_equip.resize(3);
                servant.s_servant_list.push_back(ser);
            });

            //로그 남기는 부분
            log.modify(find_log_iter, owner, [&](auto &update_log) {
                update_log.l_servant_num++;
                update_log.l_gacha_num++;
            });

        }
        uint8_t gacha_servant_head(uint64_t _seed,uint32_t _count)
        {
            uint8_t result_head = random_seed(_seed,rule_controller.head_count,not_exist_min,_count);
            auto &servant_head = rule_controller.get_head_rule_table();
            const auto &head_iter = servant_head.get(result_head,"not exist head info");
            return head_iter.h_head;
        }
        uint8_t gacha_servant_hair(uint64_t _seed,uint32_t _count)
        {
            uint8_t result_hair = random_seed(_seed,rule_controller.hair_count,not_exist_min,_count);
            auto &servant_hair = rule_controller.get_hair_rule_table();
            const auto &hair_iter = servant_hair.get(result_hair,"not exist hair info");
            return hair_iter.h_hair;
        }
        uint8_t gacha_servant_body(uint64_t _seed,uint32_t _count)
        {
            uint8_t result_body = random_seed(_seed,rule_controller.body_count,not_exist_min,_count);
            auto &servant_body = rule_controller.get_body_rule_table();
            const auto &body_iter = servant_body.get(result_body, "not exist body info");
            return body_iter.b_body;
        }
        //---------------------------------------------------------------------------------//
        void gacha_monster_id(account_name _user,uint64_t _seed)
        {           
            uint32_t result_rate = random_seed(_seed,max_rate,not_exist_min,random_count++);
            uint8_t result_id;
            if(result_rate < grade_five_rate)
            {
                result_id = 4;
            }
            else if(result_rate < grade_four_rate)
            {
                result_id = 3;
            }
            else
            {
                result_id = 2;
            }
            //uint8_t result_id = random_seed(_seed,rule_controller.monster_id_count,not_exist_min,random_count++);
            auto &monster_id = rule_controller.get_monster_id_rule_table();
            const auto &id_iter = monster_id.get(result_id,"not exist monster id");

            uint8_t result_grade = random_seed(_seed,rule_controller.monster_grade_count,not_exist_min,random_count++);
            auto &monster_grade = rule_controller.get_monster_grade_rule_table();
            const auto &grade_iter = monster_grade.get(result_grade,"not exist monster grade");

            auto &log = login_controller.get_log_table();
            auto find_log_iter = log.find(_user);

            auto cur_user_monster = monsters.find(_user);
            //eosio_assert(cur_user_monster->m_monster_list.size()<find_log_iter->l_monster_slot_count,"need more slot");
            monsters.modify(cur_user_monster, owner, [&](auto &new_monster) {
                cmonsterinfo monster;
                monster.m_index = find_log_iter->l_monster_num + 1;
                monster.m_type_index = id_iter.m_id;
                monster.m_status_info.basic_str = random_seed(_seed,grade_iter.m_max_range.m_str,grade_iter.m_min_range.m_str,random_count++);
                monster.m_status_info.basic_dex = random_seed(_seed,grade_iter.m_max_range.m_dex,grade_iter.m_min_range.m_dex,random_count++);
                monster.m_status_info.basic_int = random_seed(_seed,grade_iter.m_max_range.m_int,grade_iter.m_min_range.m_int,random_count++);
                new_monster.m_monster_list.push_back(monster);
            });

            log.modify(find_log_iter, owner, [&](auto &update_log) {
                update_log.l_gacha_num++;
                update_log.l_monster_num++;
            });
        }
        //-----------------------------------------------------------------------------//
        void gacha_item_id(account_name _user,uint64_t _seed)
        {
            uint8_t result_id = random_seed(_seed,rule_controller.item_id_count,not_exist_min,random_count++);
            auto &item_id = rule_controller.get_item_id_rule_table();
            const auto &id_iter = item_id.get(result_id, "not exist item id");

            uint8_t result_tier = random_seed(_seed,rule_controller.item_tier_count,not_exist_min,random_count++);
            auto &item_tier = rule_controller.get_item_tier_rule_table();
            const auto &tier_iter = item_tier.get(result_tier,"not exist tier info");

            auto &log = login_controller.get_log_table();
            auto find_log_iter = log.find(_user);

            auto cur_user_item = items.find(_user);
            //eosio_assert(cur_user_item->i_item_list.size()<find_log_iter->l_item_slot_count,"need more slot");
            items.modify(cur_user_item, owner, [&](auto &new_item) {
                citeminfo item;
                item.i_index = find_log_iter->l_item_num + 1;
                item.i_type_index = id_iter.i_id;
                item.i_type_equip = id_iter.i_type;
                item.i_tier = tier_iter.i_tier;
                item.i_status_info.basic_str = random_seed(_seed,tier_iter.i_max_range.i_str,tier_iter.i_min_range.i_str,random_count++);
                item.i_status_info.basic_dex = random_seed(_seed,tier_iter.i_max_range.i_dex,tier_iter.i_min_range.i_dex,random_count++);
                item.i_status_info.basic_int = random_seed(_seed,tier_iter.i_max_range.i_int,tier_iter.i_min_range.i_int,random_count++);
                item.i_job = id_iter.i_job;
                item.i_item_state = item_none;
                new_item.i_item_list.push_back(item);
            });

            log.modify(find_log_iter, owner, [&](auto &update_log) {
                update_log.l_item_num++;
                update_log.l_gacha_num++;
            });
        }
        void start_gacha(account_name _user)
        {
            auto &log = login_controller.get_log_table();
            auto find_log_iter = log.find(_user);
            eosio_assert(find_log_iter != log.end(),"unknown account");

            uint64_t l_seed = tapos_block_num() * tapos_block_prefix() * now();

            if(find_log_iter->l_gacha_num == 0)
            {
                gacha_monster_id(_user,l_seed);
            }
            else
            {
                uint64_t l_gacha_result_type = random_seed(l_seed,3,not_exist_min,random_count);
                if(l_gacha_result_type == 0)
                {
                    gacha_servant_job(_user,l_seed);
                }
                else if(l_gacha_result_type == 1)
                {
                    gacha_monster_id(_user,l_seed);
                }
                else
                {
                    gacha_item_id(_user,l_seed);
                }
            }
            random_count=0;
        }

    };