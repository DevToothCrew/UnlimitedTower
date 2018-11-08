#pragma once
#include "../Common/common_header.hpp"
#include "../Gacha/gacha_system.hpp"

//item type number = 100 ~ 150 // hand
//item type number = 151 ~ 180 // body
//item type number = 181 ~ 199 // neck

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
        void equip_servent_item(account_name _user,uint8_t _item_location,uint64_t _item_index,uint64_t _object_index,uint8_t _item_slot)
        {
            auto &user_items = gacha_controller.get_item_table();
            const auto &item_get_iter = user_items.get(_user);
            eosio_assert((item_get_iter.i_item_list[_item_location].i_index==_item_index),"not exist item");

            //해당 아이템이 맞는 슬롯인지 체크
            #pragma region slot check
            if(_item_slot == equip_list::hand)
            {
                if(item_get_iter.i_item_list[_item_location].i_type_index > 150)
                {
                    print("this slot only hand equip item");
                    return;
                }
            }
            else if(_item_slot == equip_list::body)
            {
                if (item_get_iter.i_item_list[_item_location].i_type_index > 180 && item_get_iter.i_item_list[_item_location].i_type_index < 151)
                {
                    print("this slot only body equip item");
                    return;
                }
            }
            else if(_item_slot == equip_list::neck)
            {
                if (item_get_iter.i_item_list[_item_location].i_type_index > 200 && item_get_iter.i_item_list[_item_location].i_type_index < 181)
                {
                    print("this slot only neck equip item");
                    return;
                }
            }
            #pragma endregion
            
            const auto &equip_get_iter = equip.get(_user);
            auto equip_find_iter = equip.find(_user);
            bool l_is_exist = false;
            //아이템 장착 테이블에 데이터 반영
            for (uint32_t i = 0; i != equip_get_iter.servent_list.size(); ++i)
            {
                if (equip_get_iter.servent_list[i].servent_index == _object_index)
                {
                    equip.modify(equip_find_iter, owner, [&](auto &ser) {
                        ser.servent_list[i].item_list[_item_slot] = _item_index;
                    });
                    l_is_exist = true;
                    break;
                }
            }
            if(l_is_exist==false)
            {
                equip.modify(equip_find_iter,owner,[&](auto &new_equip)
                {
                    equip_item_info new_ser;
                    new_ser.servent_index = _object_index;
                    new_ser.item_list.resize(3);
                    new_ser.item_list[_item_slot] = _item_index;
                    new_equip.servent_list.push_back(new_ser);
                });
            }

            //장착한 아이템 능력치 용병에게 반영
            auto &servents = gacha_controller.get_servent_table();
            const auto &cur_get_iter = servents.get(_user);
            int l_servent_location = -1;
            for(uint32_t i=0;i<cur_get_iter.s_servent_list.size();++i)
            {
                if(cur_get_iter.s_servent_list[i].s_index == _object_index)
                {
                    l_servent_location = i;
                    break;
                }
            }
            auto cur_find_iter = servents.find(_user);
            servents.modify(cur_find_iter,owner,[&](auto &ser){
                ser.s_servent_list[l_servent_location].plus_status_info.plus_str += item_get_iter.i_item_list[_item_location].i_status_info.strength;
                ser.s_servent_list[l_servent_location].plus_status_info.plus_dex += item_get_iter.i_item_list[_item_location].i_status_info.dexterity;
                ser.s_servent_list[l_servent_location].plus_status_info.plus_int += item_get_iter.i_item_list[_item_location].i_status_info.intelligence;
            });

        }
        void equip_hero_item(account_name _user, uint8_t _character_slot,uint8_t _item_location, uint64_t _item_index,uint8_t _item_slot)
        {
            auto &user_items = gacha_controller.get_item_table();
            const auto &item_get_iter = user_items.get(_user);
            eosio_assert((item_get_iter.i_item_list[_item_location].i_index == _item_index), "not exist item");

            if (_item_slot == equip_list::hand)
            {
                if (item_get_iter.i_item_list[_item_location].i_type_index > 150)
                {
                    print("this slot only hand equip item");
                    return;
                }
            }
            else if (_item_slot == equip_list::body)
            {
                if (item_get_iter.i_item_list[_item_location].i_type_index > 180 && item_get_iter.i_item_list[_item_location].i_type_index < 151)
                {
                    print("this slot only body equip item");
                    return;
                }
            }
            else if (_item_slot == equip_list::neck)
            {
                if (item_get_iter.i_item_list[_item_location].i_type_index > 200 && item_get_iter.i_item_list[_item_location].i_type_index < 181)
                {
                    print("this slot only neck equip item");
                    return;
                }
            }


            const auto &equip_get_iter = equip.get(_user);
        }
        void unequip_servent_item(account_name _user,uint64_t _object_index,uint8_t _item_slot)
        {
            auto &user_items = gacha_controller.get_item_table();
            const auto &item_get_iter = user_items.get(_user);

            const auto &equip_get_iter = equip.get(_user);
            auto equip_find_iter = equip.find(_user);
            uint32_t l_item_index;
            uint8_t l_item_location;
            //아이템 장착 테이블에 데이터 반영
            for (uint32_t i = 0; i != equip_get_iter.servent_list.size(); ++i)
            {
                if (equip_get_iter.servent_list[i].servent_index == _object_index)
                {
                    l_item_index = equip_get_iter.servent_list[i].item_list[_item_slot];
                    equip.modify(equip_find_iter, owner, [&](auto &ser) {
                        ser.servent_list[i].item_list[_item_slot] = 0;
                    });
                    break;
                }
            }

            for(uint32_t i=0;i<item_get_iter.i_item_list.size();++i)
            {
                if(item_get_iter.i_item_list[i].i_index == l_item_index)
                {
                    l_item_location=i;
                    break;
                }
            }
            //장착한 아이템 능력치 용병에게 반영
            auto &servents = gacha_controller.get_servent_table();
            const auto &cur_get_iter = servents.get(_user);
            int l_servent_location = -1;
            for (uint32_t i = 0; i < cur_get_iter.s_servent_list.size(); ++i)
            {
                if (cur_get_iter.s_servent_list[i].s_index == _object_index)
                {
                    l_servent_location = i;
                    break;
                }
            }
            auto cur_find_iter = servents.find(_user);
            servents.modify(cur_find_iter, owner, [&](auto &ser) {
                ser.s_servent_list[l_servent_location].plus_status_info.plus_str -= item_get_iter.i_item_list[l_item_location].i_status_info.strength;
                ser.s_servent_list[l_servent_location].plus_status_info.plus_dex -= item_get_iter.i_item_list[l_item_location].i_status_info.dexterity;
                ser.s_servent_list[l_servent_location].plus_status_info.plus_int -= item_get_iter.i_item_list[l_item_location].i_status_info.intelligence;
            });
        }
        void unequip_hero_item(account_name _user, uint8_t _character_slot,uint8_t _item_slot)
        {
            
        }

    };