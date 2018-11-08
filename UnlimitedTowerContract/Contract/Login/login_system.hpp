#pragma once
#include "../../common_header.hpp"
#include "../../table/account_info.hpp"



    class clogin_system
    {
    private:
        eosio::name owner;
        account_info_table accounts;
        account_hero_list_table accountheros;
        hero_list_table heros;
        monster_list_table monsters;
        
    public:
        clogin_system(eosio::name self) 
        : owner(self),
          accounts(self,self),
          accountheros(self,self),
          heros(self,self),
          monsters(self,self)
        {

        }
        void init()
        {
            for(uint32_t i=1;i<10;++i)
            {
                auto hero_index = heros.find(i);
                if (hero_index == heros.end())
                {
                    heros.emplace(name("owner"), [&](auto &hero_info) {
                        hero_info.set_rand_hero_ability(i);
                    });
                }
                else
                {
                    if (i != hero_index->get_hero_index())
                    {
                        heros.emplace(name("owner"), [&](auto &hero_info) {
                            hero_info.set_rand_hero_ability(i);
                        });
                    }
                }
            }

            for(uint32_t i=1;i<10;++i)
            {
                auto monster_index = monsters.find(i);
                if (monster_index == monsters.end())
                {
                    monsters.emplace(name("owner"), [&](auto &monster_info) {
                        monster_info.set_rand_monster_ability(i);
                    });
                }
                else
                {
                    if (i != monster_index->get_monster_index())
                    {
                        monsters.emplace(name("owner"), [&](auto &monster_info) {
                            monster_info.set_rand_monster_ability(i);
                        });
                    }
                }
            }
        }
        void create(const uint64_t _user,const string _nick_name)
        {
            auto cur_player_itr = accounts.find( _user );
            eosio_assert(cur_player_itr == accounts.end(), "exist account");

            accounts.emplace(name("owner"), [&](auto &new_account) 
            {
                new_account.set_account_info(_user, _nick_name);
            });
        }
        void login(uint64_t _user)
        {
            auto cur_player_itr = accounts.find( _user );
            eosio_assert(cur_player_itr != accounts.end(), "unknown account");
            //로그인시 정보를 여기서 가져다가 뿌려주면 될듯
            printf("account : %llu\n nick_name : %s\n",cur_player_itr->get_account_name(),cur_player_itr->get_account_nick_name().c_str());
        }
        void gamble(uint64_t _user,uint8_t _what_kind_gamble )
        {
            auto cur_player_itr = accounts.find( _user );
            eosio_assert(cur_player_itr != accounts.end(), "unknown account");

            switch(_what_kind_gamble)
            {
            case 1:
            {
                //겜블 로직이 들어가야함
                uint32_t result = 5; //임시 값 겜블 결과 [히어로 인덱스]
                auto hero_index = heros.find(result);
                eosio_assert(hero_index != heros.end(), "unknown hero");

                accountheros.emplace(name("owner"),[&](auto &new_hero)
                {
                    new_hero.set_owner(_user);
                    new_hero.set_rand_hero_ability(result);
                });

                // accounts.modify(cur_player_itr, 0, [&](auto &modify_account) {
                //     modify_account.add_owner_hero(*hero_index);
                // });
                break;
            }
            case 2:
            {
                break;
            }
            }


        }    
    };