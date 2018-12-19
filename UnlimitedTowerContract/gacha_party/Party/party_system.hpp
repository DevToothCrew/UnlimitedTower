#pragma once
#include "../Common/common_header.hpp"


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

        void set_party(account_name _user,uint8_t _party_number,const std::vector<uint32_t> &_party_list)
        {
            require_auth(_user);
            auto &auth_user_table = login_controller.get_auth_user_table();
            auto auth_user_iter = auth_user_table.find(_user);
            eosio_assert(auth_user_iter->hero.state == hero_state::set_travel_party || auth_user_iter->hero.state == hero_state::set_complete,
                         "this hero impossible in party member");

            auto user_party_iter = user_party_table.find(_user);
            eosio_assert(user_party_iter != user_party_table.end(), "not exist party list");
            eosio_assert(user_party_iter->party_list[_party_number].state != party_state::on_tower,"this party on tower unmodified");
            eosio_assert(user_party_iter->party_list.size() >= _party_number,"neeed more party");

            //다른 파티에 해당 인덱스가 속해 있는지
            //해당 인덱스가 존재하는지

            for(uint32_t i=0;i<max_monster_slot;++i)
            {
                user_party_table.modify(user_party_iter, owner, [&](auto save_monster)
                {
                    
                });
            }

            for (uint32_t i = max_monster_slot; i < max_servant_slot; ++i)
            {
                if(i == hero_party_location)
                {
                    continue;
                }
            }

        }
        
        void add_party_list(account_name _user)
        {
            auto &user_log_table = login_controller.get_log_table();
            auto user_log_iter = user_log_table.find(_user);
            eosio_assert(user_log_iter != user_log_table.end(),"not find user information to log");

            uint32_t l_p_count = user_log_iter->add_party_count;
            l_p_count++;
            user_log_table.modify(user_log_iter,owner,[&](auto &buy_party_log)
            {
                buy_party_log.add_party_count = l_p_count;
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