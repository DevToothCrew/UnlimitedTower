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
#pragma region test
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
#pragma endregion
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
        void gacha_servent_type(account_name _user)
        {

        }
        void gacha_monster_id(account_name _user)
        {           
            uint8_t result_id = random_value(30);
            auto &monster_id = rule_controller.get_monster_id_rule_table();
            const auto &id_iter = monster_id.get(result_id,"not exist monster id");

            uint8_t result_grade = random_value(5);
            auto &monster_grade = rule_controller.get_monster_grade_rule_table();
            const auto &grade_iter = monster_grade.get(result_grade,"not exist monster grade");

            auto &log = login_controller.get_log_table();
            auto find_log_iter = log.find(_user);

            log.modify(find_log_iter, owner, [&](auto &update_log) {
                update_log.l_gacha_num++;
                update_log.l_monster_num++;
            });

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

        }
        void gacha_item_tier(account_name _user)
        {

            
        }
        void start_gacha(account_name _user)
        {
            auto &log = login_controller.get_log_table();
            auto find_log_iter = log.find(_user);
            eosio_assert(find_log_iter != log.end(),"unknown account");

            cmonsterinfo monster;
            cserventinfo ser;
            citeminfo item;

            const auto &get_log_iter = log.get(_user);
            auto cur_user_monster = monsters.find(_user);

            auto &data = login_controller.get_static_data_table();

            if(get_log_iter.l_gacha_num == 0)
            {
                gacha_monster_id(_user);
            }
            else
            {
                //용병이 뽑혔을 경우
                uint64_t l_gacha_result_type = random_value(10);
                if(l_gacha_result_type < 2)
                {
                    uint8_t result = random_value(99);
                    if(result < 50)
                    {
                        result += 50;
                    }
                    const auto &data_get_ier = data.get(result);

                    auto cur_user_servent = servents.find(_user);
                    servents.modify(cur_user_servent, owner, [&](auto &servent) {
                        ser.s_index = get_log_iter.l_servent_num + 1;
                        ser.s_type_index = data_get_ier.type;
                        ser.status_info.strength = data_get_ier.status.s_str;
                        ser.status_info.dexterity =  data_get_ier.status.s_dex;
                        ser.status_info.intelligence = data_get_ier.status.s_int;
                        servent.s_servent_list.push_back(ser);
                    });
                    //로그 남기는 부분
                    log.modify(find_log_iter, owner, [&](auto &update_log) {
                        update_log.l_servent_num++;
                        update_log.l_gacha_num++;
                    });

                }
                else if(l_gacha_result_type < 6)
                {
                    //몬스터가 뽑혔을 경우
                    uint8_t result = random_value(49);
                    const auto &data_get_ier = data.get(result);

                    monsters.modify(cur_user_monster, owner, [&](auto &new_monster) {
                        monster.m_index = get_log_iter.l_monster_num + 1;
                        monster.m_type_index = data_get_ier.type;
                        monster.m_status_info.strength = data_get_ier.status.s_str;
                        monster.m_status_info.dexterity = data_get_ier.status.s_dex;
                        monster.m_status_info.intelligence = data_get_ier.status.s_int;
                        new_monster.m_monster_list.push_back(monster);
                    });
                    log.modify(find_log_iter, owner, [&](auto &update_log) {
                        update_log.l_monster_num++;
                        update_log.l_gacha_num++;
                    });
                }
                else
                {
                    //아이템이 뽑혔을 경우
                    uint8_t result = random_value(199);
                    if(result<100)
                    {
                        result+=100;   
                    }
                    const auto &data_get_ier = data.get(result);
                    auto cur_user_item = items.find(_user);
                    items.modify(cur_user_item, owner, [&](auto &new_item) {   
                            item.i_index = get_log_iter.l_item_num + 1;
                            item.i_type_index = data_get_ier.type;
                            item.i_status_info.strength = data_get_ier.status.s_str;
                            item.i_status_info.dexterity = data_get_ier.status.s_dex;
                            item.i_status_info.intelligence = data_get_ier.status.s_int;
                            new_item.i_item_list.push_back(item);
                    });
                    log.modify(find_log_iter, owner, [&](auto &update_log) {
                        update_log.l_item_num++;
                        update_log.l_gacha_num++;
                    });
                }
            }
        }

    };