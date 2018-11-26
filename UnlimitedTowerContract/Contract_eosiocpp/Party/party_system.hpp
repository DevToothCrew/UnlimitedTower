#pragma once
#include "../Common/common_header.hpp"

#pragma region test staice enum
enum object_type
{
    max_monster_type_id = 50,
    max_servant_type_id = 100,
};
#pragma endregion

class cparty_system
    {
    private:
        account_name owner;
        clogin_system &login_controller;
        cgacha_system &gacha_controller;
        user_party_table party_list;
    public:
        cparty_system(account_name _self,
        clogin_system &_login_controller,
        cgacha_system &_gacha_controller) 
        : owner(_self),
        login_controller(_login_controller),
        gacha_controller(_gacha_controller),
        party_list(_self,_self)
        {

        }
        user_party_table &get_user_party_table()
        {
            return party_list;
        }
        void party_init(account_name _user)
        {
            auto cur_player_iter = party_list.find(_user);
            eosio_assert(cur_player_iter == party_list.end(),"exist party list");
            party_list.emplace(owner, [&](auto &new_party) {
                new_party.party_set_user(_user);
                new_party.add_party(0);
            });
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
        void set_hero(account_name _user,uint32_t _party_number,uint8_t _character_slot)
        {
            //캐릭터 슬롯을 히어로의 아이디로 사용한다.
            //2번은 히어로의 위치
            auto party_find_iter = party_list.find(_user);
            eosio_assert(party_find_iter != party_list.end(),"not exist party list");
            party_list.modify(party_find_iter,owner,[&](auto& new_party_hero)
            {
                new_party_hero.p_party_list[_party_number].object_id_list[2] = _character_slot;
            });
        }
        void set_party(account_name _user,uint8_t _party_number,uint8_t _party_location_index,uint32_t _object_type,uint64_t _object_index)
        {  
            //need to hero setting
            eosio_assert(_party_location_index != 2,"this location only hero");
            
            auto party_find_iter = party_list.find(_user);
            eosio_assert(party_find_iter != party_list.end(),"not exist party list");

            const auto &party_get_iter = party_list.get(_user);
            uint32_t mid = 0;
            uint32_t left = 0;
            uint32_t right = 0;
            if(_party_location_index < 5)
            {
                eosio_assert(_object_type > max_monster_type_id,"this location only servant");
                bool l_use_index = false;
                auto &servants = gacha_controller.get_servant_table();
                const auto &servant_info = servants.get(_user);
                right = servant_info.s_servant_list.size()-1;
                while(left <= right)
                {
                    mid = (left+right)/2;
                    if ( servant_info.s_servant_list[mid].s_index == _object_index)
                    {
                        l_use_index = true;
                        break;
                    }
                    else if(_object_index > servant_info.s_servant_list[mid].s_index)
                    {
                        left = mid + 1;
                    }
                    else
                    {
                        right = mid - 1;
                    }
                }
                if(l_use_index == false)
                {
                    return;
                }
                for(uint32_t i =0; i<5;++i)
                {
                    if(party_get_iter.p_party_list[_party_number].object_id_list[i] == _object_index)
                    {
                        party_list.modify(party_find_iter,owner,[&](auto &new_member)
                        {
                            new_member.p_party_list[_party_number].object_id_list[i] = 0;
                            new_member.p_party_list[_party_number].object_id_list[_party_location_index] = _object_index;
                        });
                        return;
                    }
                }
                party_list.modify(party_find_iter, owner, [&](auto &new_member) {
                    new_member.p_party_list[_party_number].object_id_list[_party_location_index] = _object_index;
                });
            }
            else
            {
                eosio_assert(_object_type < max_monster_type_id,"this location only monster");
                bool l_use_index = false;
                auto &monsters = gacha_controller.get_monster_table();
                const auto &monster_info = monsters.get(_user);

                right = monster_info.m_monster_list.size()-1;
                while(left <= right)
                {
                    mid = (left+right)/2;
                    if ( monster_info.m_monster_list[mid].m_index == _object_index)
                    {
                        l_use_index = true;
                        break;
                    }
                    else if(_object_index > monster_info.m_monster_list[mid].m_index)
                    {
                        left = mid + 1;
                    }
                    else
                    {
                        right = mid - 1;
                    }
                }
                if (l_use_index == false)
                {
                    return;
                }
                for (uint32_t i = 5; i < 10; ++i)
                {
                    if (party_get_iter.p_party_list[_party_number].object_id_list[i] == _object_index)
                    {
                        party_list.modify(party_find_iter, owner, [&](auto &new_member) {
                            new_member.p_party_list[_party_number].object_id_list[i] = 0;
                            new_member.p_party_list[_party_number].object_id_list[_party_location_index] = _object_index;
                        });
                        return;
                    }
                }
                party_list.modify(party_find_iter, owner, [&](auto &new_member) {
                    new_member.p_party_list[_party_number].object_id_list[_party_location_index] = _object_index;
                });
            }
        }
        void add_party_list(account_name _user)
        {
            auto &log = login_controller.get_log_table();
            auto log_find_iter = log.find(_user);
            eosio_assert(log_find_iter != log.end(),"not find user information to log");

            const auto &log_get_iter = log.get(_user);
            uint32_t l_p_count = log_get_iter.l_party_count;
            l_p_count++;
            log.modify(log_find_iter,owner,[&](auto &add_count)
            {
                add_count.l_party_count = l_p_count;
            });
            auto cur_player_iter = party_list.find(_user);
            eosio_assert(cur_player_iter != party_list.end(), "not exist party list");
            party_list.modify(cur_player_iter,owner,[&](auto &new_party)
            {
                new_party.add_party(l_p_count);
            });
        }
    };