#pragma once
#include "../Common/common_header.hpp"
#include "../Gacha/gacha_system.hpp"

class citem_system
    {
    private:
        account_name owner;
        clogin_system &login_controller;
        cgacha_system &gacha_controller;
        equip_data_table equip;
    public:
        citem_system(account_name _self,
        clogin_system &_login_controller,
        cgacha_system &_gacha_controller) 
        : owner(_self),
        login_controller(_login_controller),
        gacha_controller(_gacha_controller),
        equip(_self,_self)
        {

        }
        equip_data_table &get_equip_table()
        {
            return equip;
        }
        void equip_init(account_name _user)
        {
            auto cur_player_iter = equip.find(_user);
            eosio_assert(cur_player_iter == equip.end(),"exist account");
            equip.emplace(owner,[&](auto &new_equip)
            {
                new_equip.set_account(_user);
            });
        }
        void sell_item(account_name _user,uint8_t _item_location,uint64_t _item_index)
        {
            print("sell item\n");
            auto &user_items = gacha_controller.get_item_table();
            const auto &item_get_iter = user_items.get(_user);
            eosio_assert((item_get_iter.i_item_list[_item_location].i_index==_item_index),"not exist item");
            //아이템이 장착중인지 체크가 필요함 
            //테이블 구조를 변경해야함
            auto &user_auth = login_controller.get_auth_user_table();
            auto auth_find_iter = user_auth.find(_user);
            user_auth.modify(auth_find_iter,owner,[&](auto &add_money)
            {
                add_money.a_game_money+=100;
            });
            auto &log = login_controller.get_log_table();
            auto log_find_iter = log.find(_user);
            log.modify(log_find_iter,owner,[&](auto &update_log)
            {
                update_log.l_get_gold+=100;
            });
            auto item_find_iter = user_items.find(_user);
            user_items.modify(item_find_iter,owner,[&](auto &del_item)
            {
                del_item.i_item_list.erase(del_item.i_item_list.begin()+_item_location);
            });
        }
        void buy_item(account_name _user,uint8_t _item_location,uint64_t _item_index)
        {
            print("buy item\n");
        }
        void equip_servant_item(account_name _user,uint8_t _item_location,uint64_t _item_index,uint64_t _object_index,uint8_t _equip_slot)
        {
            auto &user_items = gacha_controller.get_item_table();
            auto item_find_iter = user_items.find(_user);
            eosio_assert((item_find_iter->i_item_list[_item_location].i_index==_item_index),"not exist item");
            eosio_assert(item_find_iter->i_item_list[_item_location].i_type_equip==_equip_slot,"mis match equip slot");
            eosio_assert(item_find_iter->i_item_list[_item_location].i_item_state==item_none,"impossible equip item state");
            //장착한 아이템 능력치 용병에게 반영
            auto &servants = gacha_controller.get_servant_table();
            auto cur_find_iter = servants.find(_user);
            int check_exist_id = -1;
            for(uint32_t i=0;i<cur_find_iter->s_servant_list.size();++i)
            {
                if(cur_find_iter->s_servant_list[i].s_index == _object_index)
                {
                    eosio_assert(cur_find_iter->s_servant_list[i].status_info.job==item_find_iter->i_item_list[_item_location].i_job,"mis match job");
                    servants.modify(cur_find_iter, owner, [&](auto &ser) {
                        ser.s_servant_list[i].plus_status_info.plus_str += item_find_iter->i_item_list[_item_location].i_status_info.basic_str;
                        ser.s_servant_list[i].plus_status_info.plus_dex += item_find_iter->i_item_list[_item_location].i_status_info.basic_dex;
                        ser.s_servant_list[i].plus_status_info.plus_int += item_find_iter->i_item_list[_item_location].i_status_info.basic_int;
                        ser.s_servant_list[i].s_equip[_equip_slot] = _item_index;
                    });
                    user_items.modify(item_find_iter,owner,[&](auto& new_equip)
                    {
                        new_equip.i_item_list[_item_location].i_item_state = item_equip;
                    });
                    check_exist_id = i;
                    break;
                }
            }
            eosio_assert(check_exist_id!=-1,"not exist servant");
        }
        void equip_hero_item(account_name _user, uint8_t _hero_slot,uint8_t _item_location, uint64_t _item_index,uint8_t _equip_slot)
        {
            auto &user_items = gacha_controller.get_item_table();
            auto item_find_iter = user_items.find(_user);
            eosio_assert((item_find_iter->i_item_list[_item_location].i_index==_item_index),"not exist item");
            eosio_assert(item_find_iter->i_item_list[_item_location].i_type_equip==_equip_slot,"mis match equip slot");
            eosio_assert(item_find_iter->i_item_list[_item_location].i_item_state==item_none,"impossible equip item state");

            auto &auth_user_table = login_controller.get_auth_user_table();
            auto cur_find_iter = auth_user_table.find(_user);
            eosio_assert(cur_find_iter->a_hero_list[_hero_slot].status.job == item_find_iter->i_item_list[_item_location].i_job,"mis match job");
            auth_user_table.modify(cur_find_iter,owner,[&](auto &new_hero_equip)
            {
                new_hero_equip.a_hero_list[_hero_slot].plus_status.plus_str += item_find_iter->i_item_list[_item_location].i_status_info.basic_str;
                new_hero_equip.a_hero_list[_hero_slot].plus_status.plus_dex += item_find_iter->i_item_list[_item_location].i_status_info.basic_dex;
                new_hero_equip.a_hero_list[_hero_slot].plus_status.plus_int += item_find_iter->i_item_list[_item_location].i_status_info.basic_int;
                new_hero_equip.a_hero_list[_hero_slot].equip[_equip_slot]= _item_index;
            });

            user_items.modify(item_find_iter, owner, [&](auto &new_equip) {
                new_equip.i_item_list[_item_location].i_item_state = item_equip;
            });
        }
        void unequip_servant_item(account_name _user,uint32_t _servant_location,uint64_t _object_index,uint8_t _equip_slot)
        {
            //장착한 아이템 능력치 용병에게 반영
            auto &servants = gacha_controller.get_servant_table();
            auto cur_find_iter = servants.find(_user);

            auto &user_items = gacha_controller.get_item_table();
            auto item_find_iter = user_items.find(_user);
            int check_exist_id = -1;

            for(uint32_t i=0;i<item_find_iter->i_item_list.size();++i)
            {
                if(item_find_iter->i_item_list[i].i_index == cur_find_iter->s_servant_list[_servant_location].s_equip[_equip_slot])
                {
                    servants.modify(cur_find_iter, owner, [&](auto &ser) {
                        ser.s_servant_list[_servant_location].plus_status_info.plus_str -= item_find_iter->i_item_list[i].i_status_info.basic_str;
                        ser.s_servant_list[_servant_location].plus_status_info.plus_dex -= item_find_iter->i_item_list[i].i_status_info.basic_dex;
                        ser.s_servant_list[_servant_location].plus_status_info.plus_int -= item_find_iter->i_item_list[i].i_status_info.basic_int;
                        ser.s_servant_list[_servant_location].s_equip[_equip_slot] = 0;
                    });
                    user_items.modify(item_find_iter, owner, [&](auto &new_equip) {
                        new_equip.i_item_list[i].i_item_state = item_none;
                    });
                    check_exist_id = i;
                    break;
                }
            }
            eosio_assert(check_exist_id!=-1,"not exist item");
        }
        void unequip_hero_item(account_name _user, uint8_t _hero_slot, uint8_t _item_location, uint64_t _item_index, uint8_t _equip_slot)
        {
            auto &user_items = gacha_controller.get_item_table();
            auto item_find_iter = user_items.find(_user);
            eosio_assert((item_find_iter->i_item_list[_item_location].i_index==_item_index),"not exist item");
            eosio_assert(item_find_iter->i_item_list[_item_location].i_type_equip==_equip_slot,"mis match equip slot");
            eosio_assert(item_find_iter->i_item_list[_item_location].i_item_state==item_none,"impossible equip item state");

            auto &auth_user_table = login_controller.get_auth_user_table();
            auto cur_find_iter = auth_user_table.find(_user);
            eosio_assert(cur_find_iter->a_hero_list[_hero_slot].status.job == item_find_iter->i_item_list[_item_location].i_job,"mis match job");
            auth_user_table.modify(cur_find_iter,owner,[&](auto &new_hero_equip)
            {
                new_hero_equip.a_hero_list[_hero_slot].plus_status.plus_str -= item_find_iter->i_item_list[_item_location].i_status_info.basic_str;
                new_hero_equip.a_hero_list[_hero_slot].plus_status.plus_dex -= item_find_iter->i_item_list[_item_location].i_status_info.basic_dex;
                new_hero_equip.a_hero_list[_hero_slot].plus_status.plus_int -= item_find_iter->i_item_list[_item_location].i_status_info.basic_int;
                new_hero_equip.a_hero_list[_hero_slot].equip[_equip_slot]= 0;
            });

            user_items.modify(item_find_iter, owner, [&](auto &new_equip) {
                new_equip.i_item_list[_item_location].i_item_state = item_none;
            });
        }
    };