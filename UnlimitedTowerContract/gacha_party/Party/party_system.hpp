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
    const uint32_t max_servant_slot = 5;
    const uint32_t max_monster_slot = 10;
    const uint32_t hero_party_location = 0;
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

        void set_party(account_name _user, uint8_t _party_number, const std::vector<uint32_t> &_party_list)
        {
            require_auth(_user);
            auto &auth_user_table = login_controller.get_auth_user_table();
            auto auth_user_iter = auth_user_table.find(_user);
            eosio_assert(auth_user_iter->hero.state == hero_state::set_travel_party || auth_user_iter->hero.state == hero_state::set_complete,
                         "this hero impossible in party member");

            auto user_party_iter = user_party_table.find(_user);
            eosio_assert(user_party_iter != user_party_table.end(), "not exist party list");
            eosio_assert(user_party_iter->party_list.size() > _party_number,"neeed more party");
            eosio_assert(user_party_iter->party_list[_party_number].state != party_state::on_tower_defense,"this party on tower unmodified");

            user_party_table.modify(user_party_iter, owner, [&](auto &save_party) {
                user_servants user_servant_table(owner, _user);
                for (uint32_t i = 1; i < max_servant_slot; ++i)
                {
                    if (_party_list[i] == 0)
                    {
                        save_party.party_list[_party_number].index_list[i] = _party_list[i];
                        continue;
                    }
                    auto user_servant_iter = user_servant_table.find(_party_list[i]);
                    eosio_assert(user_servant_iter != user_servant_table.end(), "not exist servant data");
                    eosio_assert(user_servant_iter->party_number == EMPTY_PARTY, "already in party member servant");
                    user_servant_table.modify(user_servant_iter, owner, [&](auto &set_party) {
                        set_party.party_number = _party_number;
                    });
                    save_party.party_list[_party_number].index_list[i] = _party_list[i];
                }

                user_monsters user_monster_table(owner, _user);
                for (uint32_t i = max_servant_slot; i < max_monster_slot; ++i)
                {
                    if (_party_list[i] == 0)
                    {
                        save_party.party_list[_party_number].index_list[i] = _party_list[i];
                        continue;
                    }
                    auto user_monster_iter = user_monster_table.find(_party_list[i]);
                    eosio_assert(user_monster_iter != user_monster_table.end(), "not exist monster data");
                    eosio_assert(user_monster_iter->party_number == EMPTY_PARTY, "already in party member monster");

                    user_monster_table.modify(user_monster_iter, owner, [&](auto &set_party) {
                        set_party.party_number = _party_number;
                    });
                    save_party.party_list[_party_number].index_list[i] = _party_list[i];
                }
            });
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
        void reset_user_party_data(account_name _user)
        {
            require_auth2(_user, N(owner));
            auto iter = user_party_table.find(_user);
            eosio_assert(iter != user_party_table.end(),"not exist party data");
            user_party_table.erase(iter);
        }
#pragma endregion
    };