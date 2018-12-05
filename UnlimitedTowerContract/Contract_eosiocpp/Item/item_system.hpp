#pragma once
#include "../Common/common_header.hpp"
#include "../Gacha/gacha_system.hpp"

class citem_system
    {
    private:
        account_name owner;
        clogin_system &login_controller;
        cgacha_system &gacha_controller;
    public:
        citem_system(account_name _self,
        clogin_system &_login_controller,
        cgacha_system &_gacha_controller) 
        : owner(_self),
        login_controller(_login_controller),
        gacha_controller(_gacha_controller)
        {

        }
        void sell_item(account_name _user,uint8_t _item_location,uint64_t _item_index)
        {
            require_auth(_user);
            auto &user_item_table = gacha_controller.get_user_item_table();
            auto user_item_iter = user_item_table.find(_user);
            eosio_assert(user_item_iter->i_item_list[_item_location].i_index == _item_index,"not exist this item information");
            eosio_assert(user_item_iter->i_item_list[_item_location].i_state != item_state::item_equip,"this item already equip");

            auto &user_auth_table = login_controller.get_auth_user_table();
            auto user_auth_iter = user_auth_table.find(_user);
            eosio_assert(user_auth_iter != user_auth_table.end(),"not exist user");

            auto &user_log_table = login_controller.get_log_table();
            auto user_log_iter = user_log_table.find(_user);
            eosio_assert(user_log_iter != user_log_table.end(),"not exist user log table");

            user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
                update_log.l_get_gold += 100;
            });

            user_auth_table.modify(user_auth_iter,owner,[&](auto &sell_item_money)
            {
                sell_item_money.a_game_money+=100;
            });

            user_item_table.modify(user_item_iter, owner, [&](auto &del_item) {
                del_item.i_item_list.erase(del_item.i_item_list.begin() + _item_location);
            });
        }
        void buy_item(account_name _user,uint8_t _item_location,uint64_t _item_index)
        {
            require_auth(_user);
            print("buy item\n");
        }
        void equip_servant_item(account_name _user,uint8_t _item_location,uint64_t _item_index,uint64_t _object_index,uint8_t _equip_slot)
        {
            require_auth(_user);
            auto &user_item_table = gacha_controller.get_user_item_table();
            auto user_item_iter = user_item_table.find(_user);
            eosio_assert((user_item_iter->i_item_list[_item_location].i_index==_item_index),"not exist item");
            eosio_assert(user_item_iter->i_item_list[_item_location].i_slot==_equip_slot,"mis match equip slot");
            eosio_assert(user_item_iter->i_item_list[_item_location].i_state==item_state::item_none,"impossible equip item state");
            //장착한 아이템 능력치 용병에게 반영
            auto &user_servant_table = gacha_controller.get_user_servant_table();
            auto user_servant_iter = user_servant_table.find(_user);
            int check_exist_id = -1;
            for(uint32_t i=0;i<user_servant_iter->s_servant_list.size();++i)
            {
                if(user_servant_iter->s_servant_list[i].s_index == _object_index)
                {
                    eosio_assert(user_servant_iter->s_servant_list[i].s_status.job==user_item_iter->i_item_list[_item_location].i_status.job,"mis match job");
                    user_servant_table.modify(user_servant_iter, owner, [&](auto &equip_servant) {
                        equip_servant.s_servant_list[i].s_plus_status.plus_str += user_item_iter->i_item_list[_item_location].i_status.basic_str;
                        equip_servant.s_servant_list[i].s_plus_status.plus_dex += user_item_iter->i_item_list[_item_location].i_status.basic_dex;
                        equip_servant.s_servant_list[i].s_plus_status.plus_int += user_item_iter->i_item_list[_item_location].i_status.basic_int;
                        equip_servant.s_servant_list[i].s_equip_slot[_equip_slot] = _item_index;
                    });
                    user_item_table.modify(user_item_iter,owner,[&](auto& new_equip)
                    {
                        new_equip.i_item_list[_item_location].i_state = item_state::item_equip;
                    });
                    check_exist_id = i;
                    break;
                }
            }
            eosio_assert(check_exist_id!=-1,"not exist servant");
        }
        void equip_hero_item(account_name _user, uint8_t _hero_slot,uint8_t _item_location, uint64_t _item_index,uint8_t _equip_slot)
        {
            require_auth(_user);
            auto &user_item_table = gacha_controller.get_user_item_table();
            auto user_item_iter = user_item_table.find(_user);
            eosio_assert((user_item_iter->i_item_list[_item_location].i_index==_item_index),"not exist item");
            eosio_assert(user_item_iter->i_item_list[_item_location].i_slot==_equip_slot,"mis match equip slot");
            eosio_assert(user_item_iter->i_item_list[_item_location].i_state==item_state::item_none,"impossible equip item state");

            auto &auth_user_table = login_controller.get_auth_user_table();
            auto user_auth_iter = auth_user_table.find(_user);
            eosio_assert(user_auth_iter->a_hero_list[_hero_slot].status.job == user_item_iter->i_item_list[_item_location].i_status.job,"mis match job");
            auth_user_table.modify(user_auth_iter,owner,[&](auto &equip_hero)
            {
                equip_hero.a_hero_list[_hero_slot].plus_status.plus_str += user_item_iter->i_item_list[_item_location].i_status.basic_str;
                equip_hero.a_hero_list[_hero_slot].plus_status.plus_dex += user_item_iter->i_item_list[_item_location].i_status.basic_dex;
                equip_hero.a_hero_list[_hero_slot].plus_status.plus_int += user_item_iter->i_item_list[_item_location].i_status.basic_int;
                equip_hero.a_hero_list[_hero_slot].equip[_equip_slot]= _item_index;
            });

            user_item_table.modify(user_item_iter, owner, [&](auto &equip_item) {
                equip_item.i_item_list[_item_location].i_state = item_state::item_equip;
            });
        }
        void unequip_servant_item(account_name _user,uint32_t _servant_location,uint64_t _object_index,uint8_t _equip_slot)
        {
            require_auth(_user);
            //장착한 아이템 능력치 용병에게 반영
            auto &user_servant_table = gacha_controller.get_user_servant_table();
            auto user_servant_iter = user_servant_table.find(_user);
            eosio_assert(user_servant_iter != user_servant_table.end(),"not exist user servant table");

            auto &user_item_table = gacha_controller.get_user_item_table();
            auto user_item_iter = user_item_table.find(_user);
            eosio_assert(user_item_iter != user_item_table.end(),"not exist user item table");

            int check_exist_id = -1;

            for(uint32_t i=0;i<user_item_iter->i_item_list.size();++i)
            {
                if(user_item_iter->i_item_list[i].i_index == user_servant_iter->s_servant_list[_servant_location].s_equip_slot[_equip_slot])
                {
                    user_servant_table.modify(user_servant_iter, owner, [&](auto &unequip_servant) {
                        unequip_servant.s_servant_list[_servant_location].s_plus_status.plus_str -= user_item_iter->i_item_list[i].i_status.basic_str;
                        unequip_servant.s_servant_list[_servant_location].s_plus_status.plus_dex -= user_item_iter->i_item_list[i].i_status.basic_dex;
                        unequip_servant.s_servant_list[_servant_location].s_plus_status.plus_int -= user_item_iter->i_item_list[i].i_status.basic_int;
                        unequip_servant.s_servant_list[_servant_location].s_equip_slot[_equip_slot] = 0;
                    });
                    user_item_table.modify(user_item_iter, owner, [&](auto &unequip_item) {
                        unequip_item.i_item_list[i].i_state = item_state::item_none;
                    });
                    check_exist_id = i;
                    break;
                }
            }
            eosio_assert(check_exist_id!=-1,"not exist item");
        }
        void unequip_hero_item(account_name _user, uint8_t _hero_slot, uint8_t _item_location, uint64_t _item_index, uint8_t _equip_slot)
        {
            require_auth(_user);
            auto &user_item_table = gacha_controller.get_user_item_table();
            auto user_item_iter = user_item_table.find(_user);
            eosio_assert(user_item_iter!=user_item_table.end(),"not exist user item table");
            eosio_assert((user_item_iter->i_item_list[_item_location].i_index==_item_index),"not exist item");
            eosio_assert(user_item_iter->i_item_list[_item_location].i_slot==_equip_slot,"mis match equip slot");
            eosio_assert(user_item_iter->i_item_list[_item_location].i_state==item_state::item_none,"impossible equip item state");

            auto &auth_user_table = login_controller.get_auth_user_table();
            auto user_auth_iter = auth_user_table.find(_user);
            eosio_assert(user_auth_iter!=auth_user_table.end(),"not exist user auth table");

            auth_user_table.modify(user_auth_iter,owner,[&](auto &unequip_hero)
            {
                unequip_hero.a_hero_list[_hero_slot].plus_status.plus_str -= user_item_iter->i_item_list[_item_location].i_status.basic_str;
                unequip_hero.a_hero_list[_hero_slot].plus_status.plus_dex -= user_item_iter->i_item_list[_item_location].i_status.basic_dex;
                unequip_hero.a_hero_list[_hero_slot].plus_status.plus_int -= user_item_iter->i_item_list[_item_location].i_status.basic_int;
                unequip_hero.a_hero_list[_hero_slot].equip[_equip_slot]= 0;
            });

            user_item_table.modify(user_item_iter, owner, [&](auto &unequip_item) {
                unequip_item.i_item_list[_item_location].i_state = item_state::item_none;
            });
        }
    };