#include "Common/common_header.hpp"
// #include "BackupTable/global_info.hpp"
// #include "BackupTable/account_info.hpp"
// #include "BackupTable/hero_info.hpp"
// #include "BackupTable/monster_info.hpp"
// #include "BackupTable/party_info.hpp"

#define MAINTENANCE 0

#include "Table/auth_user_table.hpp"
#include "Table/battle_table.hpp"
#include "Table/item_table.hpp"
#include "Table/log_table.hpp"
#include "Table/monster_table.hpp"
#include "Table/party_table.hpp"
#include "Table/servent_table.hpp"
#include "Table/test_static_data_table.hpp"
#include "Table/test_static_equip_table.hpp"
#include "Table/test_static_stage_table.hpp"

#include "Rule/rule_system.hpp"

#include "Login/login_system.hpp"
#include "Gacha/gacha_system.hpp"
#include "Item/item_system.hpp"
#include "Party/party_system.hpp"
#include "Battle/battle_system.hpp"


    class cmain_logic : public contract
    {
    private:
        clogin_system login_controller;
        cbattle_system battle_controller;
        cparty_system party_controller;
        cgacha_system gacha_controller;
        citem_system item_controller;
        crule_system rule_controller;

        const char *add_char="addchar";
        const char *change_stat="changestat";
        const char *gacha_gacha="gacha";
        const char *add_party="addparty";
    public:
        cmain_logic(account_name _self) :
        contract(_self) ,
        login_controller(_self),
        battle_controller(_self,party_controller,login_controller,gacha_controller),
        party_controller(_self,login_controller,gacha_controller),
        gacha_controller(_self,login_controller,rule_controller),
        item_controller(_self,login_controller,gacha_controller),
        rule_controller(_self)
        {
            
        }
#pragma region test
        //@abi action
        void datainit()
        {
            //login_controller.init_static_data();
            //login_controller.init_stage_data();
            rule_controller.init_data();
        }
#pragma endregion


#pragma region Login
        //@abi action
        void signup(account_name _user)
        {
            print("account create\n");
            login_controller.create_account(_user);
            gacha_controller.user_own_object_init(_user); //test func
            party_controller.party_init(_user);
            item_controller.equip_init(_user);
        }
        //@abi action
        void lookset(account_name _user,uint8_t _charater_slot ,uint8_t _head,uint8_t _face,uint8_t _body)
        {
            print("set look account from account\n");
            login_controller.set_look(_user,_charater_slot,_head,_face,_body);
        }
        //@abi action
        void statset(account_name _user,uint8_t _charater_slot)
        {
            print("set status account from account\n");
            login_controller.set_status(_user,_charater_slot);
        }
        //@abi action
        void changestatus(account_name _user,uint32_t _character_slot)
        {
            print("first create character dice for change status action\n");
            login_controller.set_status(_user,_character_slot);
        }
        // eosio.token recipient
        // memo description spec
        //-------------------------------------------------------------------------
        void transfer(account_name sender, account_name receiver)
        {
            print("trasnfer\n");
            login_controller.eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
            if (ad.action.size() == 0)
            {
                print("size zero\n");
            }
            if(ad.action == add_char)        
            {
                login_controller.add_chacater_slot(sender);
            }
            else if(ad.action == change_stat)
            {
                login_controller.set_status(sender,ad.type);
            }
            else if(ad.action == gacha_gacha)
            {
                gacha_controller.start_gacha(sender);
            }
            else if(ad.action == add_party)
            {
                party_controller.add_party_list(sender);
            }
            });
        }
#pragma endregion


#pragma region Gamble
        //@abi action
        void gacha(account_name _user)
        {
            gacha_controller.start_gacha(_user);
        }
#pragma endregion

#pragma region Battle
        //@abi action
        void setbattle(account_name _user,uint8_t _stage)
        {
            print("user start battle\n");
            battle_controller.set_battle(_user,_stage);
        }
        //@abi action
        void startbattle(account_name _user,uint8_t _party_number)
        {
            battle_controller.start_battle(_user,_party_number);
        }
        //@abi action
        void activeturn(account_name _user,uint8_t _characteraction,uint8_t _monsteraction,uint8_t _character_target,uint8_t _monster_target)
        {
            print("active turn action\n");
            battle_controller.active_turn(_user,_characteraction,_monsteraction,_character_target,_monster_target);
        }
#pragma endregion

#pragma resion Party
        //@abi action
        void setparty(account_name _user,uint8_t _party_number,uint8_t _party_location_index,uint32_t _object_type,uint64_t _object_index)
        {
            print("set party action\n");
            party_controller.set_party(_user,_party_number,_party_location_index,_object_type,_object_index);
        }
#pragma endregion

#pragma resion market
        //abi action
        void sellitem(account_name _user,uint8_t _item_location,uint64_t _item_index)
        {
            print("sell item action \n");
            item_controller.sell_item(_user,_item_location,_item_index);
        }
        //abi action
        void buyitem(account_name _user,uint8_t _item_location,uint64_t _item_index)
        {
            print("buy item action\n");
            item_controller.buy_item(_user,_item_location,_item_index);
        }
        //@abi action
        void equipser(account_name _user,uint8_t _item_location,uint64_t _item_index,uint64_t _object_index,uint8_t _item_slot)
        {
            print("equip item\n");
            item_controller.equip_servent_item(_user,_item_location,_item_index,_object_index,_item_slot);
        }
        //@abi action
        void unequipser(account_name _user,uint64_t _object_index,uint8_t _item_slot)
        {
            print("un equip item\n");
            item_controller.unequip_servent_item(_user,_object_index,_item_slot);
        }
        //@abi action
        void equiphero(account_name _user, uint8_t _character_slot,uint8_t _item_location, uint64_t _item_index,uint8_t _item_slot)
        {
            print("equip item\n");
            item_controller.equip_hero_item(_user, _character_slot,_item_location, _item_index,_item_slot);
        }
        //@abi action
        void unequiphero(account_name _user, uint8_t _character_slot,uint8_t _item_location, uint64_t _item_index,uint8_t _item_slot)
        {
            print("un equip item\n");
            item_controller.unequip_hero_item(_user, _character_slot,_item_location, _item_index,_item_slot);
        }
#pragma endregion

    };

#undef EOSIO_ABI

#define EOSIO_ABI( TYPE, MEMBERS ) \
extern "C" { \
    void apply( uint64_t receiver, uint64_t code, uint64_t action ) { \
        auto self = receiver; \
        TYPE thiscontract( self ); \
        if (MAINTENANCE == 1) { \
            require_auth(self); \
        }\
        if( action == N(onerror)) { \
            eosio_assert(code == N(eosio), "onerror action's are only valid from the \"eosio\" system account"); \
        } \
        if( code == self ) { \
            if (action != N(transfer)) {\
                switch( action ) { \
                    EOSIO_API( TYPE, MEMBERS ) \
                } \
            }\
        } \
        else if (code == N(eosio.token) && action == N(transfer) ) {\
            execute_action(&thiscontract, &cmain_logic::transfer);\
        }\
    } \
}


    EOSIO_ABI(cmain_logic,(datainit)(signup)(lookset)(statset)(changestatus)(transfer)(gacha)(setbattle)(startbattle)(activeturn)(setparty)(sellitem)(buyitem)(equipser)(unequipser)(equiphero)(unequiphero) )
