#pragma once
#include "../Common/common_header.hpp"

#pragma region test staice enum
enum object_type
{
    monster_type_id = 50,
    servant_type_id = 100,
};
#pragma endregion

class cparty_system
    {
    private:
        account_name owner;
        clogin_system &login_controller;
        cgacha_system &gacha_controller;
        user_partys user_party_table;
    public:
    const uint32_t max_servant_slot = 10;
    const uint32_t max_monster_slot = 5;
    const uint32_t servant_pair = 5;
    const uint32_t hero_party_location = 7;
    const uint32_t hero_party_monster_location = 2;
    const uint32_t max_hero_slot = 3;
    const uint32_t max_total_member = 10;
    const uint32_t hero_slot_id = 10;
    public:
        cparty_system(account_name _self,
        clogin_system &_login_controller,
        cgacha_system &_gacha_controller) 
        : owner(_self),
        login_controller(_login_controller),
        gacha_controller(_gacha_controller),
        user_party_table(_self,_self)
        {

        }
        user_partys &get_user_party_table()
        {
            return user_party_table;
        }
        void party_init(account_name _user)
        {
            require_auth(_user);
            auto party_user_iter = user_party_table.find(_user);
            eosio_assert(party_user_iter == user_party_table.end(),"exist party list");
            user_party_table.emplace(owner, [&](auto &new_party) {
                new_party.party_set_user(_user);
                new_party.add_party(0);
            });
        } 
        void set_hero(account_name _user,uint32_t _party_number,uint8_t _hero_slot)
        {
            require_auth(_user);
            auto &user_log_table = login_controller.get_log_table();
            const auto &user_log_iter = user_log_table.get(_user,"not get user log data");
            eosio_assert(user_log_iter.l_add_party_count >= _party_number,"need more party");

            auto &auth_user_table = login_controller.get_auth_user_table();
            auto auth_user_iter = auth_user_table.find(_user);
            eosio_assert(auth_user_iter->a_hero_list[_hero_slot].h_state == ehero_state::set_travel_party || 
            auth_user_iter->a_hero_list[_hero_slot].h_state == ehero_state::set_complete,"this hero impossible in party member");

            auto user_party_iter = user_party_table.find(_user);
            eosio_assert(user_party_iter != user_party_table.end(),"not exist party list");
            eosio_assert(_hero_slot < max_hero_slot,"not hero index");

            auth_user_table.modify(auth_user_iter,owner,[&](auto& change_hero_state)
            {
                change_hero_state.a_hero_list[_hero_slot].h_state = ehero_state::set_travel_party;
            });

            user_party_table.modify(user_party_iter,owner,[&](auto& party_hero_set)
            {
                party_hero_set.party_list[_party_number].party_id_list[hero_party_location] = _hero_slot;
            });
        }
        void set_party(account_name _user,uint8_t _party_number,uint8_t _party_location_index,uint32_t _object_type,uint64_t _object_index)
        {  
            require_auth(_user);
            auto &user_log_table = login_controller.get_log_table();
            const auto &user_log_iter = user_log_table.get(_user, "not get user log data");
            eosio_assert(user_log_iter.l_add_party_count >= _party_number, "need more party");

            eosio_assert(_party_location_index != hero_party_location,"this location only hero");
            
            auto user_party_iter = user_party_table.find(_user);
            eosio_assert(user_party_iter != user_party_table.end(),"not exist party list");

            uint32_t mid = 0;
            uint32_t left = 0;
            uint32_t right = 0;
            //배치하려는 인덱스가 서번트만 들어갈 수 있는 공간의 경우
            if(_party_location_index < max_servant_slot && _party_location_index >= max_monster_slot)
            {
                eosio_assert(_object_type == servant_type_id,"this location only servant");
                bool l_use_index = false;
                int l_servant_location = -1;
                auto &user_servant_table = gacha_controller.get_user_servant_table();
                auto user_servant_iter = user_servant_table.find(_user);
                eosio_assert(user_servant_iter != user_servant_table.end(),"not exist user servant list");
                right = user_servant_iter->servant_list.size()-1;
                while(left <= right)
                {
                    mid = (left+right)/2;
                    if ( user_servant_iter->servant_list[mid].s_index == _object_index)
                    {
                        l_use_index = true;
                        l_servant_location = mid;
                        break;
                    }
                    else if(_object_index > user_servant_iter->servant_list[mid].s_index)
                    {
                        left = mid + 1;
                    }
                    else
                    {
                        right = mid - 1;
                    }
                }
                eosio_assert(l_use_index==true ,"not exist servent id");
                for(uint32_t i =0; i<max_servant_slot;++i)
                {
                    if(user_party_iter->party_list[_party_number].party_id_list[i] == _object_index)
                    {
                        //해당 인덱스 번호를 가진 서번트가 기존 파티에 배정이 되어 있을 경우
                        user_party_table.modify(user_party_iter,owner,[&](auto &add_party_servant)
                        {
                            add_party_servant.party_list[_party_number].party_id_list[i] = 0;
                            add_party_servant.party_list[_party_number].party_id_list[_party_location_index] = _object_index;
                        });
                        return;
                    }
                }
                user_party_table.modify(user_party_iter, owner, [&](auto &add_party_servant) {
                    add_party_servant.party_list[_party_number].party_id_list[_party_location_index] = _object_index;
                });
                user_servant_table.modify(user_servant_iter,owner,[&](auto &change_servant_state)
                {
                    change_servant_state.servant_list[l_servant_location].s_state = eobject_state::in_party;
                });
            }
            //배치하려는 인덱스가 몬스터만 들어갈 수 있는 공간의 경우
            else
            {
                eosio_assert(_object_type == monster_type_id,"this location only monster");
                if (_party_location_index != hero_party_monster_location)
                {
                    eosio_assert(user_party_iter->party_list[_party_number].party_id_list[_party_location_index + servant_pair] != 0, "need set member servant");
                }
                bool l_use_index = false;
                int l_monster_location = -1;
                auto &user_monster_table = gacha_controller.get_user_monster_table();
                auto user_monster_iter = user_monster_table.find(_user);
                eosio_assert(user_monster_iter!=user_monster_table.end(),"not exist user monster table");
                

                right = user_monster_iter->monster_list.size()-1;
                while(left <= right)
                {
                    mid = (left+right)/2;
                    if ( user_monster_iter->monster_list[mid].m_index == _object_index)
                    {
                        l_use_index = true;
                        l_monster_location = mid;
                        break;
                    }
                    else if(_object_index > user_monster_iter->monster_list[mid].m_index)
                    {
                        left = mid + 1;
                    }
                    else
                    {
                        right = mid - 1;
                    }
                }
                eosio_assert(l_use_index==true ,"not exist monster id");
                for (uint32_t i = max_servant_slot; i < max_monster_slot; ++i)
                {
                    if (user_party_iter->party_list[_party_number].party_id_list[i] == _object_index)
                    {
                         //해당 인덱스 번호를 가진 몬스터가 기존 파티에 배정이 되어 있을 경우
                        user_party_table.modify(user_party_iter, owner, [&](auto &add_party_monster) {
                            add_party_monster.party_list[_party_number].party_id_list[i] = 0;
                            add_party_monster.party_list[_party_number].party_id_list[_party_location_index] = _object_index;
                        });
                        return;
                    }
                }
                user_party_table.modify(user_party_iter, owner, [&](auto &add_party_monster) {
                    add_party_monster.party_list[_party_number].party_id_list[_party_location_index] = _object_index;
                });
                user_monster_table.modify(user_monster_iter, owner, [&](auto &change_monster_state) {
                    change_monster_state.monster_list[l_monster_location].m_state = eobject_state::in_party;
                });
            }
        }
        void add_party_list(account_name _user)
        {
            auto &user_log_table = login_controller.get_log_table();
            auto user_log_iter = user_log_table.find(_user);
            eosio_assert(user_log_iter != user_log_table.end(),"not find user information to log");

            uint32_t l_p_count = user_log_iter->l_add_party_count;
            l_p_count++;
            user_log_table.modify(user_log_iter,owner,[&](auto &buy_party_log)
            {
                buy_party_log.l_add_party_count = l_p_count;
            });
            auto user_party_iter = user_party_table.find(_user);
            eosio_assert(user_party_iter != user_party_table.end(), "not exist party list");
            user_party_table.modify(user_party_iter,owner,[&](auto &new_party)
            {
                new_party.add_party(l_p_count);
            });
        }

#pragma region reset
        void reset_all_user_party_data()
        {
            require_auth2(owner, N(owner));
            for (auto user_party_iter = user_party_table.begin(); user_party_iter != user_party_table.end();)
            {
                auto iter = user_party_table.find(user_party_iter->primary_key());
                user_party_iter++;
                user_party_table.erase(iter);
            }
        }
#pragma endregion
    };