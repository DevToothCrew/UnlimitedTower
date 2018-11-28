#pragma once
#include "../Common/common_header.hpp"

enum action_list
{
    attack_action = 1,
    defense_action,
    action_count,
    
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
    const uint32_t oper_hp = 220;
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
            uint64_t l_source = tapos_block_num() * tapos_block_prefix();
            sha256((char *)&l_source, sizeof(l_source), &l_result);
            uint64_t *l_p = reinterpret_cast<uint64_t *>(&l_result.hash);
            uint64_t l_random_result = *l_p % _range;
            if (l_random_result == 0)
            {
                l_random_result++;
            }
            return l_random_result;
        }
        void set_battle(account_name _user,uint8_t _stage)
        {
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
            const auto &battle_get_iter = battles.get(_user);
        
            auto &stage_info = login_controller.get_battle_stage_table();
            const auto &stage_get_iter = stage_info.get(battle_get_iter.b_stage_index);

            auto &user_monster = gacha_controller.get_monster_table();
            const auto &monster_get_iter = user_monster.get(_user);

            auto &user_servant = gacha_controller.get_servant_table();
            const auto &servant_get_iter = user_servant.get(_user);

            auto &user_hero = login_controller.get_auth_user_table();
            const auto &hero_get_iter = user_hero.get(_user);

            uint32_t mid = 0;
            uint32_t left = 0;
            uint32_t ser_right = 0;
            uint32_t mon_right = 0;

            //need to static stage info
            auto &party_list = party_controller.get_user_party_table();
            const auto &party_get_iter = party_list.get(_user);

            auto cur_player_iter = battles.find(_user);
            eosio_assert(cur_player_iter != battles.end(),"not exist account in battle");
            battles.modify(cur_player_iter, owner, [&](auto &new_user) {
                for (uint32_t i = 0; i < party_controller.max_servant_slot; ++i)
                {
                    if(i==hero_party_location) //히어로 능력치 셋팅
                    {
                        uint32_t hero_slot = party_get_iter.p_party_list[_party_number].object_id_list[i];
                        new_user.b_my_party_list[i].party_object_index = hero_slot;
                        new_user.b_my_party_list[i].now_hp = hero_get_iter.a_hero_list[hero_slot].status.strength * oper_hp;
                        new_user.b_my_party_list[i].defense = hero_get_iter.a_hero_list[hero_slot].status.dexterity * oper_defense;
                        new_user.b_my_party_list[i].critical = hero_get_iter.a_hero_list[hero_slot].status.intelligence * oper_critical;

                        if (hero_get_iter.a_hero_list[hero_slot].status.job == warrior)
                        {
                            new_user.b_my_party_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.strength * oper_attack;
                            new_user.b_my_party_list[i].speed = warrior_speed;
                        }
                        else if (hero_get_iter.a_hero_list[hero_slot].status.job == archer)
                        {
                            new_user.b_my_party_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.dexterity * oper_attack;
                            new_user.b_my_party_list[i].speed = archer_speed;
                        }
                        else if (hero_get_iter.a_hero_list[hero_slot].status.job == wizard)
                        {
                            new_user.b_my_party_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.intelligence * oper_attack;
                            new_user.b_my_party_list[i].speed = wizard_speed;
                        }
                        else if (hero_get_iter.a_hero_list[hero_slot].status.job == priest)
                        {
                            new_user.b_my_party_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.intelligence * oper_attack;
                            new_user.b_my_party_list[i].speed = priest_speed;
                        }
                        else if (hero_get_iter.a_hero_list[hero_slot].status.job == beginner)
                        {
                            new_user.b_my_party_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.strength * oper_attack;
                            new_user.b_my_party_list[i].speed = beginner_speed;
                        }
                        else if (hero_get_iter.a_hero_list[hero_slot].status.job == thief)
                        {
                            new_user.b_my_party_list[i].attack = hero_get_iter.a_hero_list[hero_slot].status.dexterity * oper_attack;
                            new_user.b_my_party_list[i].speed = thief_speed;
                        }
                    }

                    for (uint32_t j = 0; j < servant_get_iter.s_servant_list.size(); ++j)
                    {
                        if (servant_get_iter.s_servant_list[j].s_index == party_get_iter.p_party_list[_party_number].object_id_list[i])
                        {
                            new_user.b_my_party_list[i].now_hp = servant_get_iter.s_servant_list[j].status_info.strength * oper_hp;
                            new_user.b_my_party_list[i].defense = servant_get_iter.s_servant_list[j].status_info.dexterity * oper_defense;
                            new_user.b_my_party_list[i].critical = servant_get_iter.s_servant_list[j].status_info.intelligence * oper_critical;

                            if(servant_get_iter.s_servant_list[j].status_info.job == warrior)
                            {
                               new_user.b_my_party_list[i].attack = servant_get_iter.s_servant_list[j].status_info.strength * oper_attack;
                               new_user.b_my_party_list[i].speed = warrior_speed;
                            }
                            else if(servant_get_iter.s_servant_list[j].status_info.job == archer)
                            {
                               new_user.b_my_party_list[i].attack = servant_get_iter.s_servant_list[j].status_info.dexterity * oper_attack;
                               new_user.b_my_party_list[i].speed = archer_speed;
                            }
                            else if (servant_get_iter.s_servant_list[j].status_info.job == wizard)
                            {
                                new_user.b_my_party_list[i].attack = servant_get_iter.s_servant_list[j].status_info.intelligence * oper_attack;
                                new_user.b_my_party_list[i].speed =wizard_speed;
                            }
                            else if (servant_get_iter.s_servant_list[j].status_info.job == priest)
                            {
                                new_user.b_my_party_list[i].attack = servant_get_iter.s_servant_list[j].status_info.intelligence * oper_attack;
                                new_user.b_my_party_list[i].speed = priest_speed;
                            }
                            else if (servant_get_iter.s_servant_list[j].status_info.job == beginner)
                            {
                                new_user.b_my_party_list[i].attack = servant_get_iter.s_servant_list[j].status_info.strength * oper_attack;
                                new_user.b_my_party_list[i].speed = beginner_speed;
                            }
                            else if(servant_get_iter.s_servant_list[j].status_info.job == thief)
                            {
                               new_user.b_my_party_list[i].attack = servant_get_iter.s_servant_list[j].status_info.dexterity * oper_attack;
                               new_user.b_my_party_list[i].speed = thief_speed;
                            }
                            break;
                        }
                    }
                    new_user.b_my_party_list[i].party_object_index = party_get_iter.p_party_list[_party_number].object_id_list[i];
                }
                for (uint32_t i = party_controller.max_servant_slot; i < party_controller.max_monster_slot; ++i)
                {
                    for (uint32_t j = 0; j < monster_get_iter.m_monster_list.size(); ++j)
                    {
                        if (monster_get_iter.m_monster_list[j].m_index == party_get_iter.p_party_list[_party_number].object_id_list[i])
                        {
                            new_user.b_my_party_list[i].now_hp = monster_get_iter.m_monster_list[j].m_status_info.strength * oper_hp;
                            new_user.b_my_party_list[i].defense = monster_get_iter.m_monster_list[j].m_status_info.dexterity * oper_defense;
                            new_user.b_my_party_list[i].critical = monster_get_iter.m_monster_list[j].m_status_info.intelligence * oper_critical;
                            if(monster_get_iter.m_monster_list[j].m_type_index < 10)
                            {
                                new_user.b_my_party_list[i].attack = monster_get_iter.m_monster_list[j].m_status_info.strength * oper_attack;
                                new_user.b_my_party_list[i].speed = beginner_speed;
                            }
                            else if (monster_get_iter.m_monster_list[j].m_type_index > 10 && monster_get_iter.m_monster_list[j].m_type_index < 20)
                            {
                                new_user.b_my_party_list[i].attack = monster_get_iter.m_monster_list[j].m_status_info.dexterity * oper_attack;
                                new_user.b_my_party_list[i].speed = archer_speed;
                            }
                            else if (monster_get_iter.m_monster_list[j].m_type_index > 20 && monster_get_iter.m_monster_list[j].m_type_index < 30)
                            {
                                new_user.b_my_party_list[i].attack = monster_get_iter.m_monster_list[j].m_status_info.intelligence * oper_attack;
                                new_user.b_my_party_list[i].speed = thief_speed;
                            }
                            break;
                        }
                    }
                    new_user.b_my_party_list[i].party_object_index = party_get_iter.p_party_list[_party_number].object_id_list[i];
                }
                //enemy info setting
                for(uint32_t i = 0; i < 10; ++i)
                {
                    new_user.b_enemy_party_list[i].now_hp = stage_get_iter.enemy_list[i].s_str * oper_hp;
                    new_user.b_enemy_party_list[i].defense = stage_get_iter.enemy_list[i].s_dex * oper_defense ;
                    new_user.b_enemy_party_list[i].critical = stage_get_iter.enemy_list[i].s_int * oper_critical;
                    new_user.b_enemy_party_list[i].attack = stage_get_iter.enemy_list[i].s_str * oper_attack;
                    new_user.b_enemy_party_list[i].speed = beginner_speed;
                    new_user.b_enemy_party_list[i].party_object_index = stage_get_iter.enemy_list[i].type_index;
                }
            });
        }
        void active_turn(account_name _user,uint8_t _characteraction,uint8_t _monsteraction,uint8_t _character_target,uint8_t _monster_target)
        {
            eosio_assert(battle_state(_user)==false ,"Already closed battle");
            uint8_t user_dead_count = 0;
            uint8_t enemy_dead_count = 0;
            auto cur_player_iter = battles.find(_user);
            battles.modify(cur_player_iter,owner,[&](auto &battle_state)
            {
                std::sort(battle_state.b_my_party_list.begin(),battle_state.b_my_party_list.end(),
                [](scharacter_info a,scharacter_info b){return a.speed > b.speed;});
                battle_state.b_turn_count++;
                uint64_t l_user_action_seed = tapos_block_num() * tapos_block_prefix() * now();
                uint64_t l_user_attack_seed = tapos_block_num() * tapos_block_prefix() * now();
                uint32_t l_user_action;
                for(uint32_t i=0;i<party_controller.max_total_member;++i)
                {
                    if (battle_state.b_my_party_list[i].now_hp == 0)
                    {
                        user_dead_count++;
                        continue;
                    }
                    if(i == hero_party_location || i == hero_party_monster_location)
                    {
                        //플레이어에 대한 처리
                        continue;
                    }
                    l_user_action = random_seed(l_user_action_seed,action_count,0,i);
                    for(uint32_t enemy=0;enemy<party_controller.max_total_member;++enemy)
                    {
                        if (battle_state.b_enemy_party_list[enemy].now_hp == 0)
                        {
                            continue;
                        }
                        if (l_user_action == attack_action)
                        {
                            uint32_t l_damage = (battle_state.b_my_party_list[i].attack * ((200 * decimal) / (200 + battle_state.b_enemy_party_list[enemy].defense))) / decimal;
                            print("l_damage : ", l_damage, "\n");
                            battle_state.b_enemy_party_list[enemy].now_hp -= l_damage;
                            break;
                        }
                        else if (l_user_action == defense_action)
                        {
                            break;
                        }
                    }

                }
                //enmey data decrease
                uint64_t l_enemy_action_seed = tapos_block_num() * tapos_block_prefix() * now();
                uint64_t l_enemy_attack_seed = tapos_block_num() * tapos_block_prefix() * now();
                uint32_t l_enemy_action;
                for(uint32_t i=0;i<party_controller.max_total_member;++i)
                {
                    if(battle_state.b_enemy_party_list[i].now_hp == 0)
                    {
                        enemy_dead_count++;
                        continue;
                    }
                    l_enemy_action = random_seed(l_enemy_action_seed,action_count,0,i);
                    for(uint32_t enemy=0;enemy<party_controller.max_total_member;++enemy)
                    {
                        if(battle_state.b_my_party_list[enemy].now_hp == 0)
                        {
                            continue;
                        }
                        if(l_enemy_action == attack_action)
                        {
                            uint32_t l_damage = (battle_state.b_enemy_party_list[i].attack * ((200*decimal)/(200+battle_state.b_my_party_list[enemy].defense))) / decimal;
                            print("l_damage : ",l_damage,"\n");
                            battle_state.b_my_party_list[enemy].now_hp -= l_damage;
                            break;
                        }
                        else if(l_enemy_action == defense_action)
                        {
                            break;
                        }
                    }
                }
            });
            if(enemy_dead_count == party_controller.max_total_member)
            {
                win_reward(_user);
            }
            else if(user_dead_count == party_controller.max_total_member)
            {
                fail_reward(_user);
            }
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
        }
        bool battle_state(account_name _user)
        {
            auto &user_auth = login_controller.get_auth_user_table();
            auto user_get_iter = user_auth.get(_user,"not exist user info in battle state");
            if (user_get_iter.a_state == euser_state::battle_result)
            {
                auto battle_find_iter = battles.find(_user);
                battles.erase(battle_find_iter);
                return true;
            }
            return false;
        }
    };