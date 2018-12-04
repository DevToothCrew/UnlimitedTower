#pragma once
#include "../Common/common_header.hpp"

enum action_list
{
    attack_action = 0,
    defense_action,
    action_count,
    
};
enum battle_prefenence
{
    user = 0,
    enemy = 1,
};


class cbattle_system
    {
    private:
        account_name owner;
        user_battle_table battles;
        cparty_system &party_controller;
        clogin_system &login_controller;
        cgacha_system &gacha_controller;
    private:
    const uint32_t oper_hp = 240;
    const uint32_t oper_attack = 20;
    const uint32_t oper_defense = 50;
    const uint32_t oper_critical = 10;
    const uint32_t hero_party_location = 2;
    const uint32_t hero_party_monster_location = 5;
    const uint32_t warrior_speed = 34;
    const uint32_t wizard_speed = 29;
    const uint32_t priest_speed = 32;
    const uint32_t archer_speed = 42;
    const uint32_t thief_speed = 50;
    const uint32_t beginner_speed = 25;
    const uint32_t decimal = 100;
    const uint32_t my_party_count = 10;
    const uint32_t enemy_part_count = 20;
    const uint32_t max_battle_member_count=20;
      public:
        cbattle_system(account_name _self,
        cparty_system &_party_controller,
        clogin_system &_login_controller,
        cgacha_system &_gacha_controller 
        ) 
        : owner(_self),
        battles(_self,_self),
        party_controller(_party_controller),
        login_controller(_login_controller),
        gacha_controller(_gacha_controller)
        {

        }
        uint64_t random_seed(uint64_t _seed, uint32_t _range, uint32_t _min, uint32_t _random_count)
        {
            uint64_t l_result;
            uint64_t l_seed;

            l_seed = (_seed >> (1 * _random_count));
            l_result = l_seed % _range;
            if (l_result < _min)
            {
                return l_result += _min;
            }
            return l_result;
        }
        uint64_t random_value(uint32_t _range)
        {
            checksum256 l_result;
            uint64_t l_source = tapos_block_num() * tapos_block_prefix() * now();
            sha256((char *)&l_source, sizeof(l_source), &l_result);
            uint64_t *l_p = reinterpret_cast<uint64_t *>(&l_result.hash);
            uint64_t l_random_result = *l_p % _range;
            return l_random_result;
        }
        void set_battle(account_name _user,uint8_t _stage)
        {
            require_auth(_user);
            #pragma region test
            print("set battle field\n");
            auto cur_player_iter = battles.find(_user);
            eosio_assert(cur_player_iter == battles.end(),"playing battle....");

            battles.emplace(owner,[&](auto &new_user){
                new_user.battle_set_user(_user);
                new_user.b_stage_index = _stage;
            });
            auto &user_auth = login_controller.get_auth_user_table();
            auto user_find_iter = user_auth.find(_user);
            user_auth.modify(user_find_iter, owner, [&](auto &user) {
                if (_stage != 0)
                {
                    user.a_state = static_cast<uint8_t>(euser_state::travel);
                }
                else
                {
                    user.a_state = static_cast<uint8_t>(euser_state::tower);
                }
            });
#pragma endregion
        }
        void start_battle(account_name _user,uint8_t _party_number)
        {
            require_auth(_user);
            const auto &battle_get_iter = battles.get(_user);
        
            auto &stage_info = login_controller.get_battle_stage_table();
            const auto &stage_get_iter = stage_info.get(battle_get_iter.b_stage_index);

            auto &user_monster = gacha_controller.get_user_monster_table();
            const auto &monster_get_iter = user_monster.get(_user);

            auto &user_servant = gacha_controller.get_user_servant_table();
            const auto &servant_get_iter = user_servant.get(_user);

            auto &user_hero = login_controller.get_auth_user_table();
            const auto &hero_get_iter = user_hero.get(_user);

            uint32_t mid = 0;
            uint32_t left = 0;
            uint32_t ser_right = 0;
            uint32_t mon_right = 0;

            //need to static stage info
            auto &user_party_table = party_controller.get_user_party_table();
            const auto &party_get_iter = user_party_table.get(_user);

            auto cur_player_iter = battles.find(_user);
            eosio_assert(cur_player_iter != battles.end(),"not exist account in battle");
            battles.modify(cur_player_iter, owner, [&](auto &new_user) {
                for (uint32_t i = 0; i < party_controller.max_servant_slot; ++i)
                {
                    if(i==hero_party_location) //히어로 능력치 셋팅
                    {
                        uint32_t hero_slot = party_get_iter.p_party_list[_party_number].object_id_list[i];
                        new_user.b_battle_state_list[i].party_object_index = hero_slot;
                        new_user.b_battle_state_list[i].now_hp = hero_get_iter.a_hero_list[hero_slot].status.basic_str * oper_hp;
                        new_user.b_battle_state_list[i].defense = hero_get_iter.a_hero_list[hero_slot].status.basic_dex * oper_defense;
                        new_user.b_battle_state_list[i].critical = hero_get_iter.a_hero_list[hero_slot].status.basic_int * oper_critical;

                        if (hero_get_iter.a_hero_list[hero_slot].status.job == warrior)
                        {
                            new_user.b_battle_state_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.basic_str * oper_attack;
                            new_user.b_battle_state_list[i].speed = warrior_speed;
                        }
                        else if (hero_get_iter.a_hero_list[hero_slot].status.job == archer)
                        {
                            new_user.b_battle_state_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.basic_dex * oper_attack;
                            new_user.b_battle_state_list[i].speed = archer_speed;
                        }
                        else if (hero_get_iter.a_hero_list[hero_slot].status.job == wizard)
                        {
                            new_user.b_battle_state_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.basic_int * oper_attack;
                            new_user.b_battle_state_list[i].speed = wizard_speed;
                        }
                        else if (hero_get_iter.a_hero_list[hero_slot].status.job == priest)
                        {
                            new_user.b_battle_state_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.basic_int * oper_attack;
                            new_user.b_battle_state_list[i].speed = priest_speed;
                        }
                        else if (hero_get_iter.a_hero_list[hero_slot].status.job == beginner)
                        {
                            new_user.b_battle_state_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.basic_str * oper_attack;
                            new_user.b_battle_state_list[i].speed = beginner_speed;
                        }
                        else if (hero_get_iter.a_hero_list[hero_slot].status.job == thief)
                        {
                            new_user.b_battle_state_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.basic_dex * oper_attack;
                            new_user.b_battle_state_list[i].speed = thief_speed;
                        }
                    }
                    //servant 능력치 셋팅
                    for (uint32_t j = 0; j < servant_get_iter.s_servant_list.size(); ++j)
                    {
                        if (servant_get_iter.s_servant_list[j].s_index == party_get_iter.p_party_list[_party_number].object_id_list[i])
                        {
                            new_user.b_battle_state_list[i].now_hp = servant_get_iter.s_servant_list[j].status_info.basic_str * oper_hp;
                            new_user.b_battle_state_list[i].defense = servant_get_iter.s_servant_list[j].status_info.basic_dex * oper_defense;
                            new_user.b_battle_state_list[i].critical = servant_get_iter.s_servant_list[j].status_info.basic_int * oper_critical;

                            if(servant_get_iter.s_servant_list[j].status_info.job == warrior)
                            {
                               new_user.b_battle_state_list[i].attack = servant_get_iter.s_servant_list[j].status_info.basic_str * oper_attack;
                               new_user.b_battle_state_list[i].speed = warrior_speed;
                            }
                            else if(servant_get_iter.s_servant_list[j].status_info.job == archer)
                            {
                               new_user.b_battle_state_list[i].attack = servant_get_iter.s_servant_list[j].status_info.basic_dex * oper_attack;
                               new_user.b_battle_state_list[i].speed = archer_speed;
                            }
                            else if (servant_get_iter.s_servant_list[j].status_info.job == wizard)
                            {
                                new_user.b_battle_state_list[i].attack = servant_get_iter.s_servant_list[j].status_info.basic_int * oper_attack;
                                new_user.b_battle_state_list[i].speed =wizard_speed;
                            }
                            else if (servant_get_iter.s_servant_list[j].status_info.job == priest)
                            {
                                new_user.b_battle_state_list[i].attack = servant_get_iter.s_servant_list[j].status_info.basic_int * oper_attack;
                                new_user.b_battle_state_list[i].speed = priest_speed;
                            }
                            else if (servant_get_iter.s_servant_list[j].status_info.job == beginner)
                            {
                                new_user.b_battle_state_list[i].attack = servant_get_iter.s_servant_list[j].status_info.basic_str * oper_attack;
                                new_user.b_battle_state_list[i].speed = beginner_speed;
                            }
                            else if(servant_get_iter.s_servant_list[j].status_info.job == thief)
                            {
                               new_user.b_battle_state_list[i].attack = servant_get_iter.s_servant_list[j].status_info.basic_dex * oper_attack;
                               new_user.b_battle_state_list[i].speed = thief_speed;
                            }
                            break;
                        }
                    }
                    new_user.b_battle_state_list[i].party_object_index = party_get_iter.p_party_list[_party_number].object_id_list[i];
                }
                for (uint32_t i = party_controller.max_servant_slot; i < party_controller.max_monster_slot; ++i)
                {
                    for (uint32_t j = 0; j < monster_get_iter.m_monster_list.size(); ++j)
                    {
                        if (monster_get_iter.m_monster_list[j].m_index == party_get_iter.p_party_list[_party_number].object_id_list[i])
                        {
                            new_user.b_battle_state_list[i].now_hp = monster_get_iter.m_monster_list[j].m_status_info.basic_str * oper_hp;
                            new_user.b_battle_state_list[i].defense = monster_get_iter.m_monster_list[j].m_status_info.basic_dex * oper_defense;
                            new_user.b_battle_state_list[i].critical = monster_get_iter.m_monster_list[j].m_status_info.basic_int * oper_critical;
                            if(monster_get_iter.m_monster_list[j].m_type_index < 10)
                            {
                                new_user.b_battle_state_list[i].attack = monster_get_iter.m_monster_list[j].m_status_info.basic_str * oper_attack;
                                new_user.b_battle_state_list[i].speed = beginner_speed;
                            }
                            else if (monster_get_iter.m_monster_list[j].m_type_index > 10 && monster_get_iter.m_monster_list[j].m_type_index < 20)
                            {
                                new_user.b_battle_state_list[i].attack = monster_get_iter.m_monster_list[j].m_status_info.basic_dex * oper_attack;
                                new_user.b_battle_state_list[i].speed = archer_speed;
                            }
                            else if (monster_get_iter.m_monster_list[j].m_type_index > 20 && monster_get_iter.m_monster_list[j].m_type_index < 30)
                            {
                                new_user.b_battle_state_list[i].attack = monster_get_iter.m_monster_list[j].m_status_info.basic_int * oper_attack;
                                new_user.b_battle_state_list[i].speed = thief_speed;
                            }
                            break;
                        }
                    }
                    new_user.b_battle_state_list[i].party_object_index = party_get_iter.p_party_list[_party_number].object_id_list[i];
                }
                //enemy info setting
                for(uint32_t i = my_party_count; i < enemy_part_count; ++i)
                {
                    new_user.b_battle_state_list[i].now_hp = stage_get_iter.enemy_list[i-10].s_str * oper_hp;
                    new_user.b_battle_state_list[i].defense = stage_get_iter.enemy_list[i-10].s_dex * oper_defense ;
                    new_user.b_battle_state_list[i].critical = stage_get_iter.enemy_list[i-10].s_int * oper_critical;
                    new_user.b_battle_state_list[i].attack = stage_get_iter.enemy_list[i-10].s_str * oper_attack;
                    new_user.b_battle_state_list[i].speed = beginner_speed;
                    new_user.b_battle_state_list[i].party_object_index = stage_get_iter.enemy_list[i].type_index;
                }
            });
        }
        void active_turn(account_name _user,uint8_t _characteraction,uint8_t _monsteraction,uint8_t _character_target,uint8_t _monster_target)
        {
            require_auth(_user);
            // bool battle_check = false;
            // battle_check = battle_state(_user);
            // eosio_assert((battle_check==false) ,"Already closed battle");
            uint32_t user_dead_count = 0;
            uint32_t enemy_dead_count = 0;
            
            auto cur_player_iter = battles.find(_user);
            battles.modify(cur_player_iter,owner,[&](auto &battle_state)
            {
                uint32_t battle_preference = random_value(2);
                std::vector<attack_speed> temp_order_list;
                temp_order_list.resize(20);
                for (uint32_t i = 0; i < max_battle_member_count; ++i)
                {
                    temp_order_list[i].member_array_index = i;
                    temp_order_list[i].member_speed = cur_player_iter->b_battle_state_list[i].speed;
                }
                if(battle_preference==user)
                {
                    std::sort(temp_order_list.begin(), temp_order_list.end(),
                          [&](attack_speed a, attack_speed b) {
                              return a.member_array_index < b.member_array_index;
                          });
                }
                else if(battle_preference==enemy)
                {
                    std::sort(temp_order_list.begin(), temp_order_list.end(),
                              [&](attack_speed a, attack_speed b) {
                                  return a.member_array_index > b.member_array_index;
                              });
                }
                std::sort(temp_order_list.begin(), temp_order_list.end(),
                          [&](attack_speed a, attack_speed b) {
                              return a.member_speed > b.member_speed;
                          });
                battle_state.b_preference = battle_preference;
                battle_state.b_turn_count++;
                uint64_t l_user_action_seed = tapos_block_num() * tapos_block_prefix() * now();
                uint64_t l_user_attack_seed = tapos_block_num() * tapos_block_prefix() * now();
                uint32_t l_user_action;

                for (uint32_t i = 0; i < max_battle_member_count; ++i)
                {
                    battle_state.attack_order_list[i].member_array_index = temp_order_list[i].member_array_index;
                    battle_state.attack_order_list[i].member_speed = temp_order_list[i].member_speed;

                    uint32_t index = temp_order_list[i].member_array_index;
                    if(index < my_party_count)
                    {
                        if (index == hero_party_location || index == hero_party_monster_location)
                        {
                            //플레이어에 대한 처리
                            continue;
                        }
                        l_user_action = random_seed(l_user_action_seed, action_count, 0, i);
                        for (uint32_t enemy = my_party_count; enemy < enemy_part_count; ++enemy)
                        {
                            if (battle_state.b_battle_state_list[enemy].now_hp == 0)
                            {
                                continue;
                            }
                            if (l_user_action == attack_action)
                            {
                                battle_state.b_battle_state_list[index].action = attack_action;
                                uint32_t l_damage = (battle_state.b_battle_state_list[index].attack * ((200 * decimal) / (200 + battle_state.b_battle_state_list[enemy].defense))) / decimal;
                                print("l_damage : ", l_damage, "\n");
                                if(battle_state.b_battle_state_list[enemy].now_hp <= l_damage)
                                {
                                    battle_state.b_battle_state_list[enemy].now_hp = 0;
                                }
                                else
                                {
                                battle_state.b_battle_state_list[enemy].now_hp -= l_damage;
                                }
                                break;
                            }
                            else if (l_user_action == defense_action)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        l_user_action = random_seed(l_user_action_seed, action_count, 0, i);
                        for (uint32_t enemy = 0; enemy < my_party_count; ++enemy)
                        {
                            if (battle_state.b_battle_state_list[enemy].now_hp == 0)
                            {
                                continue;
                            }
                            if (l_user_action == attack_action)
                            {
                                battle_state.b_battle_state_list[index].action = attack_action;
                                uint32_t l_damage = (battle_state.b_battle_state_list[index].attack * ((200 * decimal) / (200 + battle_state.b_battle_state_list[enemy].defense))) / decimal;
                                print("l_damage : ", l_damage, "\n");
                                if (battle_state.b_battle_state_list[enemy].now_hp <= l_damage)
                                {
                                    battle_state.b_battle_state_list[enemy].now_hp = 0;
                                }
                                else
                                {
                                    battle_state.b_battle_state_list[enemy].now_hp -= l_damage;
                                }
                                break;
                            }
                            else if (l_user_action == defense_action)
                            {
                                break;
                            }
                        }
                    }
                }
                for (uint32_t i = 0; i < max_battle_member_count; ++i)
                {
                    if (i < my_party_count)
                    {
                        if (battle_state.b_battle_state_list[i].now_hp == 0)
                        {
                            user_dead_count += 1;
                        }
                    }
                    else
                    {
                        if (battle_state.b_battle_state_list[i].now_hp == 0)
                        {
                            enemy_dead_count += 1;
                        }
                    }
                }
                // for(uint32_t i=0;i<20;++i)
                // {
                //     print(i,"\n");
                //     print("index : ",temp_order_list[i].member_array_index,"\n");
                //     print("speed : ",temp_order_list[i].member_speed,"\n");
                // }
            });
            if(enemy_dead_count == party_controller.max_total_member)
            {
                print("user win");   
                win_reward(_user);
            }
            else if(user_dead_count == party_controller.max_total_member)
            {
                print("user lose");  
                fail_reward(_user);
            }
            print("enemy_count : ",uint32_t{enemy_dead_count},"\n");
            print("user_count : ",uint32_t{user_dead_count},"\n");
        }

        void win_reward(account_name _user)
        {
            auto &user_auth = login_controller.get_auth_user_table();
            auto user_find_iter = user_auth.find(_user);

            uint32_t l_reward = random_value(1000) + 100;

            const auto &battle_get_iter = battles.get(_user);

            auto &logs = login_controller.get_log_table();
            auto cur_log_iter = logs.find(_user);

            logs.modify(cur_log_iter, owner, [&](auto &update_log) {
                update_log.l_last_stage_num = battle_get_iter.b_stage_index;
                if (cur_log_iter->l_top_clear_stage < battle_get_iter.b_stage_index)
                {
                    update_log.l_top_clear_stage = battle_get_iter.b_stage_index;
                }
                update_log.l_battle_count++;
                update_log.l_get_gold += l_reward;
            });
            
            user_auth.modify(user_find_iter, owner, [&](auto &user) {
                user.a_game_money += l_reward;
                user.a_state = static_cast<uint8_t>(euser_state::battle_result);
            });
            auto battle_find_iter = battles.find(_user);
            eosio_assert(battle_find_iter != battles.end(), "not find battle data");
            battles.erase(battle_find_iter);
        }
        void fail_reward(account_name _user)
        {
            auto &user_auth = login_controller.get_auth_user_table();
            auto user_find_iter = user_auth.find(_user);

            const auto &battle_get_iter = battles.get(_user);

            auto &logs = login_controller.get_log_table();
            auto cur_log_iter = logs.find(_user);

            logs.modify(cur_log_iter, owner, [&](auto &update_log) {
                update_log.l_last_stage_num = battle_get_iter.b_stage_index;
                update_log.l_battle_count++;
            });

            user_auth.modify(user_find_iter, owner, [&](auto &user) {
                user.a_state = static_cast<uint8_t>(euser_state::battle_result);
            });
            auto battle_find_iter = battles.find(_user);
            eosio_assert(battle_find_iter != battles.end(), "not find battle data");
            battles.erase(battle_find_iter);
        }
        // bool battle_state(account_name _user)
        // {
        //     auto &user_auth = login_controller.get_auth_user_table();
        //     auto user_get_iter = user_auth.find(_user);
        //     if (user_get_iter->a_state == euser_state::battle_result)
        //     {
        //         user_auth.modify(user_get_iter,owner,[&](auto& a)
        //         {
        //             a.a_state = static_cast<uint8_t>(euser_state::none);
        //         });
        //         auto battle_find_iter = battles.find(_user);
        //         eosio_assert(battle_find_iter!=battles.end(),"not find battle data");
        //         battles.erase(battle_find_iter);
        //         print("end battle");
        //         return true;
        //     }
        //     else
        //     {
        //     return false;
        //     }
        // }
    };