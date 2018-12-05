#pragma once
#include "../Common/common_header.hpp"
#include "../Common/common_seed.hpp"

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
        user_battle_info user_battle_table;
        cparty_system &party_controller;
        clogin_system &login_controller;
        cgacha_system &gacha_controller;
    private:
    const uint32_t oper_hp = 240;
    const uint32_t oper_attack = 20;
    const uint32_t oper_defense = 50;
    const uint32_t oper_critical = 10;
    const uint32_t defense_constant = 200;
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
        user_battle_table(_self,_self),
        party_controller(_party_controller),
        login_controller(_login_controller),
        gacha_controller(_gacha_controller)
        {

        }
        uint64_t random_seed(uint64_t _seed, uint32_t _range, uint32_t _min, uint32_t _random_count)
        {
            uint64_t l_result;
            _seed = (_seed >> (1 * _random_count));
            l_result = _seed % _range;
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
#pragma region stage test
            auto &stage_table = login_controller.get_battle_stage_table();
            const auto &stage_iter = stage_table.get(_stage, "not exist stage info");
#pragma endregion

            auto user_battle_iter = user_battle_table.find(_user);
            eosio_assert(user_battle_iter == user_battle_table.end(),"playing battle....");

            user_battle_table.emplace(owner,[&](auto &new_battle){
                new_battle.battle_set_user(_user);
                new_battle.b_stage_number = _stage;
            });
            auto &user_auth_table = login_controller.get_auth_user_table();
            auto user_auth_iter = user_auth_table.find(_user);
            user_auth_table.modify(user_auth_iter, owner, [&](auto &user_state_change) {
                if (_stage != 0)
                {
                    user_state_change.a_state = euser_state::travel;
                }
                else
                {
                    user_state_change.a_state = euser_state::tower;
                }
            });
        }
        void start_battle(account_name _user,uint8_t _party_number)
        {
            require_auth(_user);
            auto &user_monster_table = gacha_controller.get_user_monster_table();
            const auto &user_monster_iter = user_monster_table.get(_user);

            auto &user_servant_table = gacha_controller.get_user_servant_table();
            const auto &user_servant_iter = user_servant_table.get(_user);

            auto &user_auth_table = login_controller.get_auth_user_table();
            const auto &user_auth_iter = user_auth_table.get(_user);

            uint32_t mid = 0;
            uint32_t left = 0;
            uint32_t ser_right = 0;
            uint32_t mon_right = 0;

            //need to static stage info
            auto &user_party_table = party_controller.get_user_party_table();
            const auto &user_party_iter = user_party_table.get(_user);

            auto user_battle_iter = user_battle_table.find(_user);
            eosio_assert(user_battle_iter != user_battle_table.end(),"need to battle set battle");

        #pragma region stage test
            auto &stage_table = login_controller.get_battle_stage_table();
            const auto &stage_iter = stage_table.get(user_battle_iter->b_stage_number,"not exist stage info");
        #pragma endregion

            eosio_assert(user_battle_iter != user_battle_table.end(),"not exist account in battle");
            user_battle_table.modify(user_battle_iter, owner, [&](auto &new_battle_set) {
                for (uint32_t i = 0; i < party_controller.max_servant_slot; ++i)
                {
                    if(i==party_controller.hero_party_location) //히어로 능력치 셋팅
                    {
                        uint32_t hero_slot = user_party_iter.p_party_list[_party_number].object_id_list[i];
                        new_battle_set.b_battle_state_list[i].party_object_index = hero_slot;
                        new_battle_set.b_battle_state_list[i].now_hp = user_auth_iter.a_hero_list[hero_slot].status.basic_str * oper_hp;
                        new_battle_set.b_battle_state_list[i].defense = user_auth_iter.a_hero_list[hero_slot].status.basic_dex * oper_defense;
                        new_battle_set.b_battle_state_list[i].critical = user_auth_iter.a_hero_list[hero_slot].status.basic_int * oper_critical;

                        if (user_auth_iter.a_hero_list[hero_slot].status.job == warrior)
                        {
                            new_battle_set.b_battle_state_list[i].attack = user_auth_iter.a_hero_list[hero_slot].status.basic_str * oper_attack;
                            new_battle_set.b_battle_state_list[i].speed = warrior_speed;
                        }
                        else if (user_auth_iter.a_hero_list[hero_slot].status.job == archer)
                        {
                            new_battle_set.b_battle_state_list[i].attack = user_auth_iter.a_hero_list[hero_slot].status.basic_dex * oper_attack;
                            new_battle_set.b_battle_state_list[i].speed = archer_speed;
                        }
                        else if (user_auth_iter.a_hero_list[hero_slot].status.job == wizard)
                        {
                            new_battle_set.b_battle_state_list[i].attack = user_auth_iter.a_hero_list[hero_slot].status.basic_int * oper_attack;
                            new_battle_set.b_battle_state_list[i].speed = wizard_speed;
                        }
                        else if (user_auth_iter.a_hero_list[hero_slot].status.job == priest)
                        {
                            new_battle_set.b_battle_state_list[i].attack = user_auth_iter.a_hero_list[hero_slot].status.basic_int * oper_attack;
                            new_battle_set.b_battle_state_list[i].speed = priest_speed;
                        }
                        else if (user_auth_iter.a_hero_list[hero_slot].status.job == beginner)
                        {
                            new_battle_set.b_battle_state_list[i].attack = user_auth_iter.a_hero_list[hero_slot].status.basic_str * oper_attack;
                            new_battle_set.b_battle_state_list[i].speed = beginner_speed;
                        }
                        else if (user_auth_iter.a_hero_list[hero_slot].status.job == thief)
                        {
                            new_battle_set.b_battle_state_list[i].attack = user_auth_iter.a_hero_list[hero_slot].status.basic_dex * oper_attack;
                            new_battle_set.b_battle_state_list[i].speed = thief_speed;
                        }
                    }
                    //servant 능력치 셋팅
                    for (uint32_t j = 0; j < user_servant_iter.s_servant_list.size(); ++j)
                    {
                        if (user_servant_iter.s_servant_list[j].s_index == user_party_iter.p_party_list[_party_number].object_id_list[i])
                        {
                            new_battle_set.b_battle_state_list[i].now_hp = user_servant_iter.s_servant_list[j].s_status.basic_str * oper_hp;
                            new_battle_set.b_battle_state_list[i].defense = user_servant_iter.s_servant_list[j].s_status.basic_dex * oper_defense;
                            new_battle_set.b_battle_state_list[i].critical = user_servant_iter.s_servant_list[j].s_status.basic_int * oper_critical;

                            if(user_servant_iter.s_servant_list[j].s_status.job == warrior)
                            {
                               new_battle_set.b_battle_state_list[i].attack = user_servant_iter.s_servant_list[j].s_status.basic_str * oper_attack;
                               new_battle_set.b_battle_state_list[i].speed = warrior_speed;
                            }
                            else if(user_servant_iter.s_servant_list[j].s_status.job == archer)
                            {
                               new_battle_set.b_battle_state_list[i].attack = user_servant_iter.s_servant_list[j].s_status.basic_dex * oper_attack;
                               new_battle_set.b_battle_state_list[i].speed = archer_speed;
                            }
                            else if (user_servant_iter.s_servant_list[j].s_status.job == wizard)
                            {
                                new_battle_set.b_battle_state_list[i].attack = user_servant_iter.s_servant_list[j].s_status.basic_int * oper_attack;
                                new_battle_set.b_battle_state_list[i].speed =wizard_speed;
                            }
                            else if (user_servant_iter.s_servant_list[j].s_status.job == priest)
                            {
                                new_battle_set.b_battle_state_list[i].attack = user_servant_iter.s_servant_list[j].s_status.basic_int * oper_attack;
                                new_battle_set.b_battle_state_list[i].speed = priest_speed;
                            }
                            else if (user_servant_iter.s_servant_list[j].s_status.job == beginner)
                            {
                                new_battle_set.b_battle_state_list[i].attack = user_servant_iter.s_servant_list[j].s_status.basic_str * oper_attack;
                                new_battle_set.b_battle_state_list[i].speed = beginner_speed;
                            }
                            else if(user_servant_iter.s_servant_list[j].s_status.job == thief)
                            {
                               new_battle_set.b_battle_state_list[i].attack = user_servant_iter.s_servant_list[j].s_status.basic_dex * oper_attack;
                               new_battle_set.b_battle_state_list[i].speed = thief_speed;
                            }
                            break;
                        }
                    }
                    new_battle_set.b_battle_state_list[i].party_object_index = user_party_iter.p_party_list[_party_number].object_id_list[i];
                }
                for (uint32_t i = party_controller.max_servant_slot; i < party_controller.max_monster_slot; ++i)
                {
                    for (uint32_t j = 0; j < user_monster_iter.m_monster_list.size(); ++j)
                    {
                        if (user_monster_iter.m_monster_list[j].m_index == user_party_iter.p_party_list[_party_number].object_id_list[i])
                        {
                            new_battle_set.b_battle_state_list[i].now_hp = user_monster_iter.m_monster_list[j].m_status.basic_str * oper_hp;
                            new_battle_set.b_battle_state_list[i].defense = user_monster_iter.m_monster_list[j].m_status.basic_dex * oper_defense;
                            new_battle_set.b_battle_state_list[i].critical = user_monster_iter.m_monster_list[j].m_status.basic_int * oper_critical;
                            if(user_monster_iter.m_monster_list[j].m_type < 10)
                            {
                                new_battle_set.b_battle_state_list[i].attack = user_monster_iter.m_monster_list[j].m_status.basic_str * oper_attack;
                                new_battle_set.b_battle_state_list[i].speed = beginner_speed;
                            }
                            else if (user_monster_iter.m_monster_list[j].m_type > 10 && user_monster_iter.m_monster_list[j].m_type < 20)
                            {
                                new_battle_set.b_battle_state_list[i].attack = user_monster_iter.m_monster_list[j].m_status.basic_dex * oper_attack;
                                new_battle_set.b_battle_state_list[i].speed = archer_speed;
                            }
                            else if (user_monster_iter.m_monster_list[j].m_type > 20 && user_monster_iter.m_monster_list[j].m_type < 30)
                            {
                                new_battle_set.b_battle_state_list[i].attack = user_monster_iter.m_monster_list[j].m_status.basic_int * oper_attack;
                                new_battle_set.b_battle_state_list[i].speed = thief_speed;
                            }
                            break;
                        }
                    }
                    new_battle_set.b_battle_state_list[i].party_object_index = user_party_iter.p_party_list[_party_number].object_id_list[i];
                }
                //enemy info setting
                for(uint32_t i = my_party_count; i < enemy_part_count; ++i)
                {
                    new_battle_set.b_battle_state_list[i].now_hp = stage_iter.enemy_list[i-10].s_str * oper_hp;
                    new_battle_set.b_battle_state_list[i].defense = stage_iter.enemy_list[i-10].s_dex * oper_defense ;
                    new_battle_set.b_battle_state_list[i].critical = stage_iter.enemy_list[i-10].s_int * oper_critical;
                    new_battle_set.b_battle_state_list[i].attack = stage_iter.enemy_list[i-10].s_str * oper_attack;
                    new_battle_set.b_battle_state_list[i].speed = beginner_speed;
                    new_battle_set.b_battle_state_list[i].party_object_index = stage_iter.enemy_list[i].type_index;
                }
            });
        }
        void active_turn(account_name _user,uint8_t _hero_action,uint8_t _monster_action,uint8_t _hero_target,uint8_t _monster_target)
        {
            require_auth(_user);
            auto &user_auth_table = login_controller.get_auth_user_table();
            auto user_auth_iter = user_auth_table.find(_user);
            if(user_auth_iter->a_state == euser_state::battle_lose || user_auth_iter->a_state == euser_state::battle_win)
            {
                user_auth_table.modify(user_auth_iter,owner,[&](auto &user_state)
                {
                    user_state.a_state = euser_state::lobby;
                });
                end_battle(_user);
            }
            else
            {
                uint32_t user_dead_count = 0;
                uint32_t enemy_dead_count = 0;

                auto user_battle_iter = user_battle_table.find(_user);
                user_battle_table.modify(user_battle_iter, owner, [&](auto &battle_state) {
                    uint32_t battle_preference = random_seed(safeseed::get_seed(_user), 2, 0, 0);
                    std::vector<attack_speed> temp_order_list;
                    temp_order_list.resize(20);
                    for (uint32_t i = 0; i < max_battle_member_count; ++i)
                    {
                        temp_order_list[i].member_array_index = i;
                        temp_order_list[i].member_speed = user_battle_iter->b_battle_state_list[i].speed;
                    }
                    if (battle_preference == user)
                    {
                        std::sort(temp_order_list.begin(), temp_order_list.end(),
                                  [&](attack_speed a, attack_speed b) {
                                      return a.member_array_index < b.member_array_index;
                                  });
                    }
                    else if (battle_preference == enemy)
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
                    uint32_t l_user_action;

                    for (uint32_t i = 0; i < max_battle_member_count; ++i)
                    {
                        battle_state.attack_order_list[i].member_array_index = temp_order_list[i].member_array_index;
                        battle_state.attack_order_list[i].member_speed = temp_order_list[i].member_speed;

                        uint32_t index = temp_order_list[i].member_array_index;
                        if (index < my_party_count)
                        {
                            if (index == party_controller.hero_party_location)
                            {
                                if(_hero_action == attack_action)
                                {
                                    uint32_t l_damage = (battle_state.b_battle_state_list[index].attack * ((defense_constant * decimal) / 
                                    (defense_constant + battle_state.b_battle_state_list[_hero_target].defense))) / decimal;
                                    if (battle_state.b_battle_state_list[_hero_target].now_hp <= l_damage)
                                    {
                                        battle_state.b_battle_state_list[_hero_target].now_hp = 0;
                                    }
                                    else
                                    {
                                        battle_state.b_battle_state_list[_hero_target].now_hp -= l_damage;
                                    }
                                }
                                continue;
                            }
                            else if(index == party_controller.hero_party_monster_location)
                            {
                                if (_monster_action == attack_action)
                                {
                                    uint32_t l_damage = (battle_state.b_battle_state_list[index].attack * ((defense_constant * decimal) /
                                        (defense_constant + battle_state.b_battle_state_list[_monster_target].defense))) /decimal;
                                    if (battle_state.b_battle_state_list[_monster_target].now_hp <= l_damage)
                                    {
                                        battle_state.b_battle_state_list[_monster_target].now_hp = 0;
                                    }
                                    else
                                    {
                                        battle_state.b_battle_state_list[_monster_target].now_hp -= l_damage;
                                    }
                                }
                                continue;
                            }
                            l_user_action = random_seed(safeseed::get_seed(_user), action_count, 0, i);
                            for (uint32_t enemy = my_party_count; enemy < enemy_part_count; ++enemy)
                            {
                                if (battle_state.b_battle_state_list[enemy].now_hp == 0)
                                {
                                    continue;
                                }
                                if (l_user_action == attack_action)
                                {
                                    battle_state.b_battle_state_list[index].action = attack_action;
                                    uint32_t l_damage = (battle_state.b_battle_state_list[index].attack * ((defense_constant * decimal) / (defense_constant + battle_state.b_battle_state_list[enemy].defense))) / decimal;
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
                        else
                        {
                            l_user_action = random_seed(safeseed::get_seed(_user), action_count, 0, i);
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
                });
                if (enemy_dead_count == party_controller.max_total_member)
                {
                    print("user win");
                    win_reward(_user);
                }
                else if (user_dead_count == party_controller.max_total_member)
                {
                    print("user lose");
                    fail_reward(_user);
                }
                print("enemy_count : ", uint32_t{enemy_dead_count}, "\n");
                print("user_count : ", uint32_t{user_dead_count}, "\n");
            }
        }

        void win_reward(account_name _user)
        {
            auto &user_auth_table = login_controller.get_auth_user_table();
            auto user_auth_iter = user_auth_table.find(_user);

            uint32_t l_reward = random_value(1000) + 100;

            const auto &user_battle_iter = user_battle_table.get(_user);

            auto &user_log_table = login_controller.get_log_table();
            auto user_log_iter = user_log_table.find(_user);

            user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
                update_log.l_last_stage_num = user_battle_iter.b_stage_number;
                if (user_log_iter->l_top_clear_stage < user_battle_iter.b_stage_number)
                {
                    update_log.l_top_clear_stage = user_battle_iter.b_stage_number;
                }
                update_log.l_battle_count++;
                update_log.l_get_gold += l_reward;
            });
            
            user_auth_table.modify(user_auth_iter, owner, [&](auto &user_state) {
                user_state.a_game_money += l_reward;
                user_state.a_state = euser_state::battle_win;
            });

        }
        void fail_reward(account_name _user)
        {
            auto &user_auth_table = login_controller.get_auth_user_table();
            auto user_auth_iter = user_auth_table.find(_user);

            const auto &user_battle_iter = user_battle_table.get(_user);

            auto &user_log_table = login_controller.get_log_table();
            auto user_log_iter = user_log_table.find(_user);

            user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
                update_log.l_last_stage_num = user_battle_iter.b_stage_number;
                update_log.l_battle_count++;
            });

            user_auth_table.modify(user_auth_iter, owner, [&](auto &user_state) {
                user_state.a_state = euser_state::battle_lose;
            });
        }
        void end_battle(account_name _user)
        {
            auto user_battle_iter = user_battle_table.find(_user);
            eosio_assert(user_battle_iter != user_battle_table.end(), "already erase battle data");
            user_battle_table.erase(user_battle_iter);
        }
#pragma region reset
        void reset_all_battle_data()
        {
            require_auth2(owner,N(owner));
            for(auto user_battle_iter = user_battle_table.begin();user_battle_iter!=user_battle_table.end();)
            {
                auto iter = user_battle_table.find(user_battle_iter->primary_key());
                user_battle_iter++;
                user_battle_table.erase(iter);
            }
            auto &user_auth_table = login_controller.get_auth_user_table();
            for(auto user_auth_iter = user_auth_table.begin();user_auth_iter!=user_auth_table.end();user_auth_iter++)
            {
                user_auth_table.modify(user_auth_iter, owner, [&](auto &user_state_change) {
                    user_state_change.a_state = euser_state::lobby;
                });
            }
        }
#pragma endregion
    };