#pragma once
#include "../Common/common_header.hpp"
//0~49 monster 50 ~ 99 servent 100~199 item
class cgacha_system
    {
    private:
        account_name owner;
        clogin_system &login_controller;
        crule_system &rule_controller;
        user_monster_table monsters;
        user_servent_table servents;
        user_item_table items;

    public:
        cgacha_system(account_name _self,
        clogin_system &_login_controller,
        crule_system &_rule_controller) 
        : owner(_self),
        login_controller(_login_controller),
        rule_controller(_rule_controller),
        monsters(_self,_self),
        servents(_self,_self),
        items(_self,_self)
        {

        }
        user_monster_table &get_monster_table()
        {
            return monsters;
        }
        user_servent_table &get_servent_table()
        {
            return servents;
        }
        user_item_table &get_item_table()
        {
            return items;
        }
        void user_own_object_init(account_name _user)
        {
            servents.emplace(owner, [&](auto &new_servent) {
                new_servent.servent_set_user(_user);
            });
            monsters.emplace(owner, [&](auto &new_monster) {
                new_monster.monster_set_user(_user);
            });
            items.emplace(owner, [&](auto &new_item) {
                new_item.item_set_user(_user);
            });
        }
        uint64_t random_value(uint32_t _range)
        {
            checksum256 l_result;
            uint64_t l_source = tapos_block_num() * tapos_block_prefix();
            sha256((char *)&l_source, sizeof(l_source), &l_result);
            uint64_t *l_p = reinterpret_cast<uint64_t *>(&l_result.hash);
            uint64_t l_random_result = *l_p % _range;
            if(l_random_result == 0)
            {
                l_random_result++;
            }
            return l_random_result;
        }
        uint64_t random_min(uint32_t _min,uint32_t _max)
        {
            checksum256 l_result;
            uint64_t l_source = tapos_block_num() * tapos_block_prefix();
            sha256((char *)&l_source, sizeof(l_source), &l_result);
            uint64_t *l_p = reinterpret_cast<uint64_t *>(&l_result.hash);
            uint64_t l_random_result = *l_p % _max;
            if (l_random_result == 0)
            {
                l_random_result++;
            }
            else if(_min > l_random_result )
            {
                l_random_result+= _min;
            }
            return l_random_result;
        }
        void gacha_servent_job(account_name _user)
        {
            uint8_t result_job = random_value(rule_controller.servent_job_count);
            auto &servent_job = rule_controller.get_servent_rule_table();
            const auto &job_iter = servent_job.get(result_job,"not exist servent job");

            auto &log = login_controller.get_log_table();
            auto find_log_iter = log.find(_user);

            
            auto cur_user_servent = servents.find(_user);
            servents.modify(cur_user_servent, owner, [&](auto &servent) {
                cserventinfo ser;
                ser.s_index = find_log_iter->l_servent_num + 1;
                ser.appear_info.hair = gacha_servent_hair();
                ser.appear_info.face = gacha_servent_head();
                ser.appear_info.body = gacha_servent_body();
                ser.status_info.job = job_iter.s_job;
                ser.status_info.strength = random_min(job_iter.s_min_range.s_str,job_iter.s_max_range.s_str);
                ser.status_info.dexterity = random_min(job_iter.s_min_range.s_dex,job_iter.s_max_range.s_dex);
                ser.status_info.intelligence = random_min(job_iter.s_min_range.s_int,job_iter.s_max_range.s_int);
                servent.s_servent_list.push_back(ser);
            });

            //로그 남기는 부분
            log.modify(find_log_iter, owner, [&](auto &update_log) {
                update_log.l_servent_num++;
                update_log.l_gacha_num++;
            });

        }
        uint8_t gacha_servent_head()
        {
            uint8_t result_head = random_value(rule_controller.head_count);
            auto &servent_head = rule_controller.get_head_rule_table();
            const auto &head_iter = servent_head.get(result_head,"not exist head info");
            return head_iter.h_head;
        }
        uint8_t gacha_servent_hair()
        {
            uint8_t result_hair = random_value(rule_controller.hair_count);
            auto &servent_hair = rule_controller.get_hair_rule_table();
            const auto &hair_iter = servent_hair.get(result_hair,"not exist hair info");
            return hair_iter.h_hair;
        }
        uint8_t gacha_servent_body()
        {
            uint8_t result_body = random_value(rule_controller.body_count);
            auto &servent_body = rule_controller.get_body_rule_table();
            const auto &body_iter = servent_body.get(result_body, "not exist body info");
            return body_iter.b_body;
        }
        //---------------------------------------------------------------------------------//
        void gacha_monster_id(account_name _user)
        {           
            uint8_t result_id = random_value(rule_controller.monster_id_count);
            auto &monster_id = rule_controller.get_monster_id_rule_table();
            const auto &id_iter = monster_id.get(result_id,"not exist monster id");

            uint8_t result_grade = random_value(rule_controller.monster_grade_count);
            auto &monster_grade = rule_controller.get_monster_grade_rule_table();
            const auto &grade_iter = monster_grade.get(result_grade,"not exist monster grade");

            auto &log = login_controller.get_log_table();
            auto find_log_iter = log.find(_user);

            auto cur_user_monster = monsters.find(_user);
            monsters.modify(cur_user_monster, owner, [&](auto &new_monster) {
                cmonsterinfo monster;
                monster.m_index = find_log_iter->l_monster_num + 1;
                monster.m_type_index = id_iter.m_id;
                monster.m_status_info.strength = random_min(grade_iter.m_min_range.m_str,grade_iter.m_max_range.m_str);
                monster.m_status_info.dexterity = random_min(grade_iter.m_min_range.m_dex,grade_iter.m_max_range.m_dex);
                monster.m_status_info.intelligence = random_min(grade_iter.m_min_range.m_int,grade_iter.m_max_range.m_int);
                new_monster.m_monster_list.push_back(monster);
            });

            log.modify(find_log_iter, owner, [&](auto &update_log) {
                update_log.l_gacha_num++;
                update_log.l_monster_num++;
            });
        }
        //-----------------------------------------------------------------------------//
        void gacha_item_id(account_name _user)
        {
            uint8_t result_id = random_value(rule_controller.item_id_count);
            auto &item_id = rule_controller.get_item_id_rule_table();
            const auto &id_iter = item_id.get(result_id, "not exist item id");

            uint8_t result_tier = random_value(rule_controller.item_tier_count);
            auto &item_tier = rule_controller.get_item_tier_rule_table();
            const auto &tier_iter = item_tier.get(result_tier,"not exist tier info");

            auto &log = login_controller.get_log_table();
            auto find_log_iter = log.find(_user);

            auto cur_user_item = items.find(_user);
            items.modify(cur_user_item, owner, [&](auto &new_item) {
                citeminfo item;
                item.i_index = find_log_iter->l_item_num + 1;
                item.i_type_index = id_iter.i_id;
                item.i_type_equip = id_iter.i_type;
                item.i_tier = tier_iter.i_tier;
                item.i_status_info.strength = random_min(id_iter.i_min_range.i_str,id_iter.i_max_range.i_str);
                item.i_status_info.dexterity = random_min(id_iter.i_min_range.i_dex,id_iter.i_max_range.i_dex);
                item.i_status_info.intelligence = random_min(id_iter.i_min_range.i_int,id_iter.i_max_range.i_int);
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

            if(find_log_iter->l_gacha_num == 0)
            {
                gacha_monster_id(_user);
            }
            else
            {
                uint64_t l_gacha_result_type = random_value(3);
                if(l_gacha_result_type == 1)
                {
                    gacha_servent_job(_user);
                }
                else if(l_gacha_result_type == 2)
                {
                    gacha_monster_id(_user);
                }
                else
                {
                    gacha_item_id(_user);
                }
            }
        }

    };