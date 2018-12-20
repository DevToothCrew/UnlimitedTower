#pragma once
#include "../Common/common_header.hpp"

class cparty_system
    {
    private:
        account_name owner;
        clogin_system &login_controller;
        cgacha_system &gacha_controller;
    public:
    const uint32_t max_servant_slot = 5;
    const uint32_t hero_partner_slot = 5;
    const uint32_t max_monster_slot = 10;
    const uint32_t pair_slot = 5;
    const uint32_t hero_party_location = 0;
    const uint32_t empty_party_slot = 0;
    public:
        cparty_system(account_name _self,
        clogin_system &_login_controller,
        cgacha_system &_gacha_controller) 
        : owner(_self),
        login_controller(_login_controller),
        gacha_controller(_gacha_controller)
        {

        }
        void party_init(account_name _user)
        {
            require_auth(_user);
            user_partys user_party_table(owner, _user);
            user_party_table.emplace(owner, [&](auto &new_party)
            {
                uint32_t first_index = user_party_table.available_primary_key();
                if(first_index == 0)
                {
                    new_party.index = 1;
                }
                else
                {
                    new_party.index = user_party_table.available_primary_key();
                }
                new_party.party.resize(10);
            });
        } 

        void set_party(account_name _user, uint8_t _party_number, const std::vector<uint32_t> &_party_list)
        {
            require_auth(_user);
            eosio_assert(_party_number > 0 ,"wrong party_number");

            // auto &auth_user_table = login_controller.get_auth_user_table();
            // auto auth_user_iter = auth_user_table.find(_user);
            // eosio_assert(auth_user_iter->hero.state == hero_state::set_travel_party || auth_user_iter->hero.state == hero_state::set_complete,
            //              "this hero impossible in party member");

            user_partys user_party_table(owner, _user);
            auto user_party_iter = user_party_table.find(_party_number);
            eosio_assert(user_party_iter != user_party_table.end(), "not exist party");
            eosio_assert(user_party_iter->state != party_state::on_tower_defense,"this party on tower unmodified");

            user_servants user_servant_table(owner, _user);
            user_monsters user_monster_table(owner, _user);

            for (uint32_t i = 1; i < max_servant_slot; ++i)
            {
                if(user_party_iter->party[i] != empty_party_slot && 
                user_party_iter->party[i] != _party_list[i])
                {
                    auto user_servant_iter = user_servant_table.find(user_party_iter->party[i]);
                    eosio_assert(user_servant_iter != user_servant_table.end(),"fatal party data mis");
                    user_servant_table.modify(user_servant_iter, owner, [&](auto &set_party) {
                        set_party.party_number = EMPTY_PARTY;
                    });
                }
            }
            for (uint32_t i = max_servant_slot; i < max_monster_slot; ++i)
            {
                if (user_party_iter->party[i] != empty_party_slot &&
                    user_party_iter->party[i] != _party_list[i])
                {
                    auto user_monster_iter = user_monster_table.find(user_party_iter->party[i]);
                    eosio_assert(user_monster_iter != user_monster_table.end(), "fatal party data mis");
                    user_monster_table.modify(user_monster_iter, owner, [&](auto &set_party) {
                        set_party.party_number = EMPTY_PARTY;
                    });
                }
            }

            user_party_table.modify(user_party_iter, owner, [&](auto &save_party) {
                
                for (uint32_t i = 1; i < max_servant_slot; ++i)
                {
                    if (_party_list[i] == empty_party_slot)
                    {
                        save_party.party[i] = _party_list[i];
                        continue;
                    }
                    auto user_servant_iter = user_servant_table.find(_party_list[i]);
                    eosio_assert(user_servant_iter != user_servant_table.end(), "not exist servant data");
                    eosio_assert(user_servant_iter->party_number == EMPTY_PARTY || user_servant_iter->party_number == _party_number, "already in party member servant");
                    user_servant_table.modify(user_servant_iter, owner, [&](auto &set_party) {
                        set_party.party_number = _party_number;
                    });
                    save_party.party[i] = _party_list[i];
                }

                for (uint32_t i = max_servant_slot; i < max_monster_slot; ++i)
                {
                    if (_party_list[i] == empty_party_slot)
                    {
                        save_party.party[i] = _party_list[i];
                        continue;
                    }
                    if (i != hero_partner_slot)
                    {
                        eosio_assert(_party_list[i - pair_slot] != empty_party_slot ,"need set servant");
                    }
                    auto user_monster_iter = user_monster_table.find(_party_list[i]);
                    eosio_assert(user_monster_iter != user_monster_table.end(), "not exist monster data");
                    eosio_assert(user_monster_iter->party_number == EMPTY_PARTY || user_monster_iter->party_number == _party_number, "already in party member monster");

                    user_monster_table.modify(user_monster_iter, owner, [&](auto &set_party) {
                        set_party.party_number = _party_number;
                    });
                    save_party.party[i] = _party_list[i];
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
            user_partys user_party_table(owner, _user);
            user_party_table.emplace(owner, [&](auto &new_party)
            {
                new_party.index = user_party_table.available_primary_key();
                new_party.party.resize(10);
            });
        }

#pragma region reset
        void reset_user_party_data(account_name _user)
        {
            user_partys user_party_table(owner, _user);
            for (auto user_party_iter = user_party_table.begin(); user_party_iter != user_party_table.end();)
            {
                auto iter = user_party_table.find(user_party_iter->primary_key());
                user_party_iter++;
                user_party_table.erase(iter);
            }
        }
        void reset_all_user_party_data()
        {
            require_auth2(owner, N(owner));
            auto &user_auth_table = login_controller.get_auth_user_table();
            for (auto user_name_iter = user_auth_table.begin(); user_name_iter != user_auth_table.end();)
            {
                reset_user_party_data(user_name_iter->primary_key());
            }
        }

        void delete_party_data(account_name _user)
        {
            require_auth2(owner, N(owner));
            user_partys user_party_table(owner, _user);
            for (auto user_party_iter = user_party_table.begin(); user_party_iter != user_party_table.end();)
            {
                auto iter = user_party_table.find(user_party_iter->primary_key());
                user_party_iter++;
                user_party_table.erase(iter);
            }
        }
#pragma endregion

#pragma region party cheat
        void set_automatic_party(account_name _user)
        {
            uint32_t first_party = 1;
            user_partys user_party_table(owner, _user);
            auto user_party_iter = user_party_table.find(first_party);
            eosio_assert(user_party_iter != user_party_table.end(), "not exist party list");
            
            user_servants user_servant_table(owner, _user);
            user_monsters user_monster_table(owner, _user);

            user_party_table.modify(user_party_iter, owner, [&](auto &automatic_party)
            {
                for(uint32_t i=1; i<10; ++i)
                {
                    if (i < 5)
                    {
                        automatic_party.party[i] = i;

                        auto user_servant_iter = user_servant_table.find(i);
                        user_servant_table.modify(user_servant_iter, owner, [&](auto &set_party) {
                            set_party.party_number = first_party;
                        });
                    }
                    else
                    {
                        uint32_t monster_index = i - 4;
                        automatic_party.party[i] = monster_index;
                        
                        auto user_monster_iter = user_monster_table.find(monster_index);
                        user_monster_table.modify(user_monster_iter, owner, [&](auto &set_party) {
                            set_party.party_number = first_party;
                        });
                    }
                }
            });
        }
#pragma endregion
    };