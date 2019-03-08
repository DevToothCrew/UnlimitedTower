#pragma once
#include "../Common/common_header.hpp"
#include "../Common/common_seed.hpp"

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

  private:
    battle_data_table stage_table;

  private:
    const uint32_t warrior_speed = 34;
    const uint32_t wizard_speed = 29;
    const uint32_t priest_speed = 32;
    const uint32_t archer_speed = 42;
    const uint32_t thief_speed = 50;
    const uint32_t beginner_speed = 25;

  private:
    const uint32_t oper_hp = 240;
    const uint32_t oper_attack = 20;
    const uint32_t oper_defense = 50;
    const uint32_t oper_critical = 10;
    const uint32_t defense_constant = 200;

    const uint32_t decimal = 100;
    const uint32_t max_battle_member_count = 20;
    const uint32_t max_party_count = 10;
    
  private:
    std::vector<uint32_t> battle_location_list;

  public:
    cbattle_system(account_name _self,
                   cparty_system &_party_controller,
                   clogin_system &_login_controller)
        : owner(_self),
          user_battle_table(_self, _self),
          party_controller(_party_controller),
          login_controller(_login_controller),
          stage_table(_self,_self)
    {
        battle_location_list.push_back(HERO_POSITION);
        battle_location_list.push_back(SERVANT_POSITION_1);
        battle_location_list.push_back(SERVANT_POSITION_2);
        battle_location_list.push_back(SERVANT_POSITION_3);
        battle_location_list.push_back(SERVANT_POSITION_4);
        battle_location_list.push_back(MONSTER_POSITION_5);
        battle_location_list.push_back(MONSTER_POSITION_6);
        battle_location_list.push_back(MONSTER_POSITION_7);
        battle_location_list.push_back(MONSTER_POSITION_8);
        battle_location_list.push_back(MONSTER_POSITION_9);
    }

#pragma region static data test
    void set_stage_data()
    {
        uint32_t l_stage_count = 1;
        for (uint32_t j = 0; j < 200; ++j)
        {
            if((j!=0) &&(j%10 == 0))
            {
                l_stage_count++;
            }
            stage_info enemy_info;
            enemy_info.type_index = j;
            enemy_info.base_str = 1;
            enemy_info.base_dex = 1;
            enemy_info.base_int = 1;
            enemy_info.base_job = j;

            auto cur_stage_iter = stage_table.find(l_stage_count);
            if (cur_stage_iter == stage_table.end())
            {
                stage_table.emplace(owner, [&](auto &new_stage) {
                    new_stage.stage = l_stage_count;
                    new_stage.enemy_list.push_back(enemy_info);
                });
            }
            else
            {
                stage_table.modify(cur_stage_iter, owner, [&](auto &new_stage) {
                    new_stage.enemy_list.push_back(enemy_info);
                });
            }
        }
    }
#pragma endregion
    uint32_t get_attack(uint32_t _job, status_info _status)
    {
        uint32_t attack = 0;
        if ( _job == job_list::warrior || _job == job_list::beginner )
        {
            attack = (_status.basic_str + _status.plus_str) * oper_attack;
            return attack;
        }
        else if ( _job == job_list::archer || _job == job_list::thief )
        {
            attack = (_status.basic_dex + _status.plus_dex) * oper_attack;
            return attack;
        }
        else if ( _job == job_list::wizard || _job == job_list::priest)
        {
            attack = (_status.basic_int + _status.plus_int) * oper_attack;
            return attack;
        }
        else
        {
            return attack;
        }
    }

    uint32_t get_speed(uint32_t _job)
    {
        if ( _job == job_list::warrior )
        {
            return warrior_speed;
        }
        else if ( _job == job_list::archer )
        {
            return archer_speed;
        }
        else if ( _job == job_list::wizard )
        {
            return wizard_speed;
        }
        else if ( _job == job_list::priest )
        {
           return priest_speed;
        }
        else if ( _job == job_list::beginner )
        {
            return beginner_speed;
        }
        else if ( _job == job_list::thief )
        {
            return thief_speed;
        }
        else
        {
            return 0;
        }
    }

    uint64_t get_damage(uint32_t _atk, uint32_t _dfs)
    {
        uint32_t damage = (_atk * ((defense_constant * decimal) / (defense_constant + _dfs))) / decimal;
        return damage;
    }

    uint32_t get_buff_turn(uint32_t _buff)
    {
        if( _buff == battle_buff_state::poison )
        {
            return 4;
        }
        else if( _buff == battle_buff_state::strength_collect_wait )
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    // void set_buff(uint32_t _buff, std::vector<battle_member_state> &_state_list)
    // {
    //     for(uint32_t i=0; i<_state_list.size(); ++i)
    //     {
    //         if(_state_list[i].state == _buff)
    //         {
    //             _state_list[i].count = get_buff_turn(_buff);
    //             return; 
    //         }
    //     }

    //     battle_member_state debuff;
    //     debuff.count = get_buff_turn(_buff);;
    //     debuff.state = _buff;
    //     _state_list.push_back(debuff);
    // }

    void start_battle(account_name _user, uint8_t _party_number, uint8_t _stage)
    {
        require_auth(_user);
#pragma region stage test
        const auto &stage_iter = stage_table.get(_stage, "not exist stage info");
#pragma endregion
        auto &user_auth_table = login_controller.get_auth_user_table();
        auto user_auth_iter = user_auth_table.find(_user);
        eosio_assert(user_auth_iter != user_auth_table.end(), "not exist user auth data");
        eosio_assert(user_auth_iter->state != euser_state::travel && user_auth_iter->state != euser_state::tower, "playing battle....");

        user_auth_table.modify(user_auth_iter, owner, [&](auto &user_state_change) {
                user_state_change.state = euser_state::travel;
        });

        user_partys user_party_table(owner, _user);
        const auto &user_party_iter = user_party_table.get(_party_number, "not exist user party data");
        eosio_assert(user_party_iter.state != party_state::on_tower_defense,"this party impossible battle");

        user_servants user_servant_table(owner,_user);
        user_monsters user_monster_table(owner,_user);
        uint32_t randering_location = 0;

        auto user_battle_iter = user_battle_table.find(_user);
        if (user_battle_iter == user_battle_table.end())
        {
            user_battle_table.emplace(owner, [&](auto &new_battle_set) {
                new_battle_set.battle_set_user(_user);
                new_battle_set.turn_count = START_BATTLE;
                new_battle_set.preference = 0;
                new_battle_set.reward_list.clear();
    
                randering_location = battle_location_list[0];
                new_battle_set.my_party_status_list[randering_location].now_hp = (user_auth_iter->hero.status.basic_str + user_auth_iter->hero.status.plus_str) * oper_hp;
                new_battle_set.my_party_status_list[randering_location].defense = (user_auth_iter->hero.status.basic_dex + user_auth_iter->hero.status.plus_dex) * oper_defense;
                new_battle_set.my_party_status_list[randering_location].critical = (user_auth_iter->hero.status.basic_int + user_auth_iter->hero.status.plus_int) * oper_critical;
                new_battle_set.my_party_status_list[randering_location].attack = get_attack(user_auth_iter->hero.job, user_auth_iter->hero.status);
                new_battle_set.my_party_status_list[randering_location].speed = get_speed(user_auth_iter->hero.job);
                new_battle_set.my_party_status_list[randering_location].party_index = 0;
                new_battle_set.my_party_status_list[randering_location].state = battle_action_state::wait;

                for(uint32_t i = 1; i < party_controller.max_servant_slot; ++i)
                {
                    if (user_party_iter.party[i] == 0) //파티 멤버가 비어있으면
                    {
                        continue;
                    }
                    const auto &user_servant_iter = user_servant_table.get(user_party_iter.party[i], "not exist servant data");

                    randering_location = battle_location_list[i];
                    new_battle_set.my_party_status_list[randering_location].now_hp = ( user_servant_iter.servant.status.basic_str + user_servant_iter.servant.status.plus_str )  * oper_hp;
                    new_battle_set.my_party_status_list[randering_location].defense = ( user_servant_iter.servant.status.basic_dex + user_servant_iter.servant.status.plus_dex ) * oper_defense;
                    new_battle_set.my_party_status_list[randering_location].critical = ( user_servant_iter.servant.status.basic_int + user_servant_iter.servant.status.plus_int ) * oper_critical;
                    new_battle_set.my_party_status_list[randering_location].attack = get_attack(user_servant_iter.servant.job, user_servant_iter.servant.status);
                    new_battle_set.my_party_status_list[randering_location].speed = get_speed(user_servant_iter.servant.job);
                    new_battle_set.my_party_status_list[randering_location].party_index = user_servant_iter.index;
                    new_battle_set.my_party_status_list[randering_location].state = battle_action_state::wait;
                }
                for (uint32_t i = party_controller.max_servant_slot; i < party_controller.max_monster_slot; ++i)
                {
                    if (user_party_iter.party[i] == 0) //파티 멤버가 비어있으면
                    {
                        continue;
                    }
                    const auto &user_monster_iter = user_monster_table.get(user_party_iter.party[i], "not exist monster data");

                    randering_location = battle_location_list[i];
                    new_battle_set.my_party_status_list[randering_location].now_hp = (user_monster_iter.monster.status.basic_str + user_monster_iter.monster.status.plus_str) * oper_hp;
                    new_battle_set.my_party_status_list[randering_location].defense = (user_monster_iter.monster.status.basic_dex + user_monster_iter.monster.status.plus_dex) * oper_defense;
                    new_battle_set.my_party_status_list[randering_location].critical = (user_monster_iter.monster.status.basic_int + user_monster_iter.monster.status.plus_int) * oper_critical;
                    new_battle_set.my_party_status_list[randering_location].attack = get_attack(beginner, user_monster_iter.monster.status);
                    new_battle_set.my_party_status_list[randering_location].speed = get_speed(beginner);
                    new_battle_set.my_party_status_list[randering_location].party_index = user_monster_iter.index;
                    new_battle_set.my_party_status_list[randering_location].state = battle_action_state::wait;
                }

                //enemy info setting
                for (uint32_t i = 0; i < max_party_count; ++i)
                {
                    new_battle_set.enemy_party_status_list[i].now_hp = stage_iter.enemy_list[i].base_str * oper_hp;
                    new_battle_set.enemy_party_status_list[i].defense = stage_iter.enemy_list[i].base_dex * oper_defense;
                    new_battle_set.enemy_party_status_list[i].critical = stage_iter.enemy_list[i].base_int * oper_critical;
                    new_battle_set.enemy_party_status_list[i].attack = stage_iter.enemy_list[i].base_str * oper_attack;
                    new_battle_set.enemy_party_status_list[i].speed = beginner_speed;
                    new_battle_set.enemy_party_status_list[i].party_index = stage_iter.enemy_list[i].type_index;
                    new_battle_set.enemy_party_status_list[i].state = battle_action_state::wait;
                }
            });
        }
        else
        {
            user_battle_table.modify(user_battle_iter, owner, [&](auto &new_battle_set) {
               new_battle_set.battle_set_user(_user);
                new_battle_set.turn_count = START_BATTLE;
                new_battle_set.preference = 0;
                new_battle_set.reward_list.clear();
    
                randering_location = battle_location_list[0];
                new_battle_set.my_party_status_list[randering_location].now_hp = (user_auth_iter->hero.status.basic_str + user_auth_iter->hero.status.plus_str) * oper_hp;
                new_battle_set.my_party_status_list[randering_location].defense = (user_auth_iter->hero.status.basic_dex + user_auth_iter->hero.status.plus_dex) * oper_defense;
                new_battle_set.my_party_status_list[randering_location].critical = (user_auth_iter->hero.status.basic_int + user_auth_iter->hero.status.plus_int) * oper_critical;
                new_battle_set.my_party_status_list[randering_location].attack = get_attack(user_auth_iter->hero.job, user_auth_iter->hero.status);
                new_battle_set.my_party_status_list[randering_location].speed = get_speed(user_auth_iter->hero.job);
                new_battle_set.my_party_status_list[randering_location].party_index = 0;
                new_battle_set.my_party_status_list[randering_location].state = battle_action_state::wait;

                for(uint32_t i = 1; i < party_controller.max_servant_slot; ++i)
                {
                    const auto &user_servant_iter = user_servant_table.get(user_party_iter.party[i], "not exist servant data");

                    randering_location = battle_location_list[i];
                    new_battle_set.my_party_status_list[randering_location].now_hp = ( user_servant_iter.servant.status.basic_str + user_servant_iter.servant.status.plus_str )  * oper_hp;
                    new_battle_set.my_party_status_list[randering_location].defense = ( user_servant_iter.servant.status.basic_dex + user_servant_iter.servant.status.plus_dex ) * oper_defense;
                    new_battle_set.my_party_status_list[randering_location].critical = ( user_servant_iter.servant.status.basic_int + user_servant_iter.servant.status.plus_int ) * oper_critical;
                    new_battle_set.my_party_status_list[randering_location].attack = get_attack(user_servant_iter.servant.job, user_servant_iter.servant.status);
                    new_battle_set.my_party_status_list[randering_location].speed = get_speed(user_servant_iter.servant.job);
                    new_battle_set.my_party_status_list[randering_location].party_index = user_servant_iter.index;
                    new_battle_set.my_party_status_list[randering_location].state = battle_action_state::wait;
                }
                for (uint32_t i = party_controller.max_servant_slot; i < party_controller.max_monster_slot; ++i)
                {
                    const auto &user_monster_iter = user_monster_table.get(user_party_iter.party[i], "not exist monster data");

                    randering_location = battle_location_list[i];
                    new_battle_set.my_party_status_list[randering_location].now_hp = (user_monster_iter.monster.status.basic_str + user_monster_iter.monster.status.plus_str) * oper_hp;
                    new_battle_set.my_party_status_list[randering_location].defense = (user_monster_iter.monster.status.basic_dex + user_monster_iter.monster.status.plus_dex) * oper_defense;
                    new_battle_set.my_party_status_list[randering_location].critical = (user_monster_iter.monster.status.basic_int + user_monster_iter.monster.status.plus_int) * oper_critical;
                    new_battle_set.my_party_status_list[randering_location].attack = get_attack(beginner, user_monster_iter.monster.status);
                    new_battle_set.my_party_status_list[randering_location].speed = get_speed(beginner);
                    new_battle_set.my_party_status_list[randering_location].party_index = user_monster_iter.index;
                    new_battle_set.my_party_status_list[randering_location].state = battle_action_state::wait;
                }

                //enemy info setting
                for (uint32_t i = 0; i < max_party_count; ++i)
                {
                    new_battle_set.enemy_party_status_list[i].now_hp = stage_iter.enemy_list[i].base_str * oper_hp;
                    new_battle_set.enemy_party_status_list[i].defense = stage_iter.enemy_list[i].base_dex * oper_defense;
                    new_battle_set.enemy_party_status_list[i].critical = stage_iter.enemy_list[i].base_int * oper_critical;
                    new_battle_set.enemy_party_status_list[i].attack = stage_iter.enemy_list[i].base_str * oper_attack;
                    new_battle_set.enemy_party_status_list[i].speed = beginner_speed;
                    new_battle_set.enemy_party_status_list[i].party_index = stage_iter.enemy_list[i].type_index;
                    new_battle_set.enemy_party_status_list[i].state = battle_action_state::wait;
                }
            });
        }
    }

    void active_turn(account_name _user, uint8_t _hero_action, uint8_t _monster_action, uint8_t _hero_target, uint8_t _monster_target)
    {
        require_auth(_user);
        auto &user_auth_table = login_controller.get_auth_user_table();
        const auto &user_auth_iter = user_auth_table.get(_user);
        eosio_assert(user_auth_iter.state == euser_state::travel || user_auth_iter.state == euser_state::tower, "already over battle");

        uint32_t user_dead_count = 0;
        uint32_t enemy_dead_count = 0;
        uint64_t seed = safeseed::get_seed(owner, _user);

        auto user_battle_iter = user_battle_table.find(_user);
        eosio_assert(user_battle_iter != user_battle_table.end(),"not setting battle data");
        user_battle_table.modify(user_battle_iter, owner, [&](auto &battle_state) {
            uint32_t battle_preference = safeseed::get_random_battle_value(seed, 2, 0, 0);
            battle_state.preference = battle_preference;
            battle_state.turn_count += 1;

            std::vector<attack_speed> temp_order_list;
            temp_order_list.resize(20);

            for (uint32_t i = 0; i < max_party_count; ++i)
            {
                temp_order_list[i].member_array_index = i;
                temp_order_list[i].member_speed = user_battle_iter->my_party_status_list[i].speed;
            }

            for (uint32_t i = 0; i < max_party_count; ++i)
            {
                temp_order_list[i + max_party_count].member_array_index = i + max_party_count;
                temp_order_list[i + max_party_count].member_speed = user_battle_iter->enemy_party_status_list[i].speed;
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


            uint32_t user_action;
            for (uint32_t i = 0; i < max_battle_member_count; ++i)
            {
                battle_state.attack_order_list[i] = temp_order_list[i].member_array_index;

                uint32_t index = temp_order_list[i].member_array_index;
                if (index < max_party_count)         //자기 파티에 대한 처리
                {
                    if(battle_state.my_party_status_list[index].now_hp == 0)
                    {
                        continue;
                    }
                    if (index == battle_location_list[0])
                    {
                        eosio_assert(battle_state.enemy_party_status_list[_hero_target].now_hp != 0 , "hero target is dead");
                        battle_state.my_party_status_list[index].state = _hero_action;
                        if (_hero_action == battle_action_state::attack)
                        {
                            uint32_t damage = get_damage(battle_state.my_party_status_list[index].attack, battle_state.enemy_party_status_list[_hero_target].defense);

                            battle_state.my_party_status_list[index].target = _hero_target;

                            if (battle_state.enemy_party_status_list[_hero_target].now_hp <= damage)
                            {
                                battle_state.enemy_party_status_list[_hero_target].now_hp = 0;
                            }
                            else
                            {
                                battle_state.enemy_party_status_list[_hero_target].now_hp -= damage;
                            }
                        }
                        else if ( _hero_action == battle_action_state::defense)
                        {
                            battle_state.my_party_status_list[index].now_hp += 2;
                        }
                        continue;
                    }
                    else if (index == battle_location_list[party_controller.hero_partner_monster_slot])
                    {
                        eosio_assert(battle_state.enemy_party_status_list[_monster_target].now_hp != 0 , "monster target is dead");
                        battle_state.my_party_status_list[index].state = _monster_action;
                        if (_monster_action == battle_action_state::attack)
                        {
                            uint32_t damage = get_damage(battle_state.my_party_status_list[index].attack, battle_state.enemy_party_status_list[_monster_target].defense);

                            battle_state.my_party_status_list[index].target = _monster_target;
                            if (battle_state.enemy_party_status_list[_monster_target].now_hp <= damage)
                            {
                                battle_state.enemy_party_status_list[_monster_target].now_hp = 0;
                            }
                            else
                            {
                                battle_state.enemy_party_status_list[_monster_target].now_hp -= damage;
                            }
                        }
                        else if(_monster_action == battle_action_state::defense)
                        {
                            battle_state.my_party_status_list[index].now_hp += 2;
                        }
                        continue;
                    }

                    user_action = safeseed::get_random_battle_value(seed, battle_action_state::state_count, battle_action_state::attack, i);
                    for (uint32_t enemy = 0; enemy < max_party_count; ++enemy)
                    {
                        if (battle_state.enemy_party_status_list[enemy].now_hp == 0)
                        {
                            continue;
                        }
                        if (user_action == battle_action_state::attack)
                        {
                            battle_state.my_party_status_list[index].state = battle_action_state::attack;
                            battle_state.my_party_status_list[index].target = enemy;

                            uint32_t damage = get_damage(battle_state.my_party_status_list[index].attack, battle_state.enemy_party_status_list[enemy].defense);;
                            if (battle_state.enemy_party_status_list[enemy].now_hp <= damage)
                            {
                                battle_state.enemy_party_status_list[enemy].now_hp = 0;
                            }
                            else
                            {
                                battle_state.enemy_party_status_list[enemy].now_hp -= damage;
                            }
                            break;
                        }
                        else if (user_action == battle_action_state::defense)
                        {
                            battle_state.my_party_status_list[index].now_hp += 2;
                            break;
                        }
                    }
                }
                else if(index >= 10) 
                {
                    index = index - 10;
                    if (battle_state.enemy_party_status_list[index].now_hp == 0)
                    {
                        continue;
                    }

                    user_action = safeseed::get_random_battle_value(seed, battle_action_state::state_count, battle_action_state::attack, i);
                    for (uint32_t enemy = 0; enemy < max_party_count; ++enemy)
                    {
                        if (battle_state.my_party_status_list[enemy].now_hp == 0)
                        {
                            continue;
                        }
                        if (user_action == battle_action_state::attack)
                        {
                            battle_state.enemy_party_status_list[index].state = battle_action_state::attack;
                            battle_state.enemy_party_status_list[index].target = enemy;

                            uint32_t damage = get_damage(battle_state.enemy_party_status_list[index].attack, battle_state.my_party_status_list[enemy].defense);;
                            if (battle_state.my_party_status_list[enemy].now_hp <= damage)
                            {
                                battle_state.my_party_status_list[enemy].now_hp = 0;
                            }
                            else
                            {
                                battle_state.my_party_status_list[enemy].now_hp -= damage;
                            }
                            break;
                        }
                        else if (user_action == battle_action_state::defense)
                        {
                            battle_state.enemy_party_status_list[index].now_hp += 2;
                            break;
                        }
                    }
                }
            }
            //게임의 종료 여부 체크 
            for (uint32_t i = 0; i < max_battle_member_count; ++i)
            {
                if (i < max_party_count)
                {
                    if (battle_state.my_party_status_list[i].now_hp == 0)
                    {
                        user_dead_count += 1;
                    }
                }
                else
                {
                    if (battle_state.enemy_party_status_list[i - 10].now_hp == 0)
                    {
                        enemy_dead_count += 1;
                    }
                }
            }
        });
        if (enemy_dead_count == 10)
        {
            win_reward(_user);
        }
        else if (user_dead_count == 10)
        {
            fail_reward(_user);
        }
    }

    void win_reward(account_name _user)
    {
        auto &user_auth_table = login_controller.get_auth_user_table();
        auto user_auth_iter = user_auth_table.find(_user);
        eosio_assert(user_auth_iter != user_auth_table.end(), "not exist user auth data");

        uint64_t seed = safeseed::get_seed(owner, _user);
        uint32_t reward = safeseed::get_random_value(seed, 1000, 100, 0); //1000 , 100  test value

        auto user_battle_iter = user_battle_table.find(_user);
        eosio_assert(user_battle_iter != user_battle_table.end(), "not exist user battle data");

        auto &user_log_table = login_controller.get_log_table();
        auto user_log_iter = user_log_table.find(_user);
        eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data");

        user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
            update_log.last_stage_num = user_battle_iter->stage_number;
            if (user_log_iter->top_clear_stage < user_battle_iter->stage_number)
            {
                update_log.top_clear_stage = user_battle_iter->stage_number;
            }
            update_log.battle_count += 1;
            update_log.get_gold += reward;
        });

        user_battle_table.modify(user_battle_iter, owner, [&](auto &add_win_reward) {
            add_win_reward.turn_count = END_BATTLE;
            add_win_reward.reward_list.push_back(reward);
        });

        user_auth_table.modify(user_auth_iter, owner, [&](auto &user_state) {
            user_state.state = euser_state::lobby;
            user_state.game_money += reward;
        });

        asset battle_reward(0, S(4, UTG));
        battle_reward.amount = reward * 1000;

        action(permission_level{owner, N(active)},
               owner, N(tokentrans),
               std::make_tuple(owner, _user, battle_reward, std::string("battle reward")))
            .send();
    }

    void fail_reward(account_name _user)
    {
        auto &user_auth_table = login_controller.get_auth_user_table();
        auto user_auth_iter = user_auth_table.find(_user);
        eosio_assert(user_auth_iter != user_auth_table.end(), "not exist user auth data");

        auto user_battle_iter = user_battle_table.find(_user);
        eosio_assert(user_battle_iter != user_battle_table.end(), "not exist user battle data");
        user_battle_table.modify(user_battle_iter, owner, [&](auto &add_win_reward) {
            add_win_reward.turn_count = END_BATTLE;
        });

        auto &user_log_table = login_controller.get_log_table();
        auto user_log_iter = user_log_table.find(_user);
        eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data");

        user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
            update_log.last_stage_num = user_battle_iter->stage_number;
            update_log.battle_count += 1;
        });

        user_auth_table.modify(user_auth_iter, owner, [&](auto &user_state) {
            user_state.state = euser_state::lobby;
        });
    }

#pragma region init
    void init_all_battle_data()
    {
        require_auth2(owner, N(owner));
        auto &user_auth_table = login_controller.get_auth_user_table();

        for (auto user_battle_iter = user_battle_table.begin(); user_battle_iter != user_battle_table.end();)
        {
            auto user_auth_iter = user_auth_table.find(user_battle_iter->primary_key());
            user_auth_table.modify(user_auth_iter, owner, [&](auto &user_state) {
                user_state.state = euser_state::lobby;
            });

            auto iter = user_battle_table.find(user_battle_iter->primary_key());
            user_battle_iter++;
            user_battle_table.erase(iter);
        }
    }

    void init_stage_data()
    {
        require_auth2(owner, N(owner));
        for (auto stage_iter = stage_table.begin(); stage_iter != stage_table.end();)
        {
            auto iter = stage_table.find(stage_iter->primary_key());
            stage_iter++;
            stage_table.erase(iter);
        }
    }
#pragma endregion
};