#include "Common/common_header.hpp"

#define MAINTENANCE 0

#include "Table/auth_user_table.hpp"
#include "Table/battle_table.hpp"
#include "Table/item_table.hpp"
#include "Table/log_table.hpp"
#include "Table/monster_table.hpp"
#include "Table/party_table.hpp"
#include "Table/servant_table.hpp"
#include "Table/test_static_stage_table.hpp"

#include "DB/db_system.hpp"

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
        cdb_system db_controller;

        const char *add_hero="addhero";
        const char *change_stat="changestat";
        const char *gacha_gacha="gacha";
        const char *add_party="addparty";
    public:
        cmain_logic(account_name _self) :
        contract(_self) ,
        login_controller(_self,db_controller),
        battle_controller(_self,party_controller,login_controller,gacha_controller),
        party_controller(_self,login_controller,gacha_controller),
        gacha_controller(_self,login_controller,db_controller),
        item_controller(_self,login_controller,gacha_controller),
        db_controller(_self)
        {
            
        }
#pragma region init
        //@abi action
        void datainit()
        {
            db_controller.init_db_data();
        }
        //@abi action
        void stageinit()
        {
            login_controller.init_stage_data();
        }
#pragma endregion


#pragma region Login
        //@abi action
        void signup(account_name _user)
        {
            print("account create\n");
            login_controller.create_account(_user);
            gacha_controller.user_inventory_init(_user);
            party_controller.party_init(_user);
        }
        //@abi action
        void lookset(account_name _user,uint8_t _hero_slot ,uint8_t _head,uint8_t _hair,uint8_t _body)
        {
            print("set look account from account\n");
            login_controller.set_look(_user,_hero_slot,_head,_hair,_body);
        }
        //@abi action
        void statset(account_name _user,uint8_t _hero_slot)
        {
            print("set status account from account\n");
            login_controller.set_status(_user,_hero_slot);
        }
        //@abi action
        void completehero(account_name _user,uint32_t _hero_slot)
        {
            login_controller.complete_hero_set(_user,_hero_slot);
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
            if(ad.action == add_hero)        
            {
                login_controller.add_hero_slot(sender);
            }
            else if(ad.action == change_stat)
            {
                login_controller.change_status(sender,ad.type);
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


#pragma region Battle
        //@abi action
        void startbattle(account_name _user,uint8_t _party_number,uint8_t _stage)
        {
            battle_controller.start_battle(_user,_party_number,_stage);
        }
        //@abi action
        void activeturn(account_name _user,uint8_t _hero_action,uint8_t _monster_action,uint8_t _hero_target,uint8_t _monster_target)
        {
            print("active turn action\n");
            battle_controller.active_turn(_user,_hero_action,_monster_action,_hero_target,_monster_target);
        }

#pragma endregion

#pragma resion Party
        //@abi action
        void setparty(account_name _user,uint8_t _party_number,const std::vector<uint32_t> &_party_list)
        {
            party_controller.set_party(_user,_party_number,_party_list);
        }
#pragma endregion

#pragma resion market
        //abi action
        void sellitem(account_name _user,uint8_t _item_location,uint64_t _item_index)
        {
            print("sell item action \n");
            item_controller.sell_item(_user,_item_location,_item_index);
        }
        //@abi action
        void equipser(account_name _user,uint8_t _item_location,uint64_t _item_index,uint64_t _object_index,uint8_t _item_slot)
        {
            print("equip item\n");
            item_controller.equip_servant_item(_user,_item_location,_item_index,_object_index,_item_slot);
        }
        //@abi action
        void unequipser(account_name _user,uint32_t _servant_location,uint8_t _item_slot)
        {
            print("un equip item\n");
            item_controller.unequip_servant_item(_user,_servant_location,_item_slot);
        }
        //@abi action
        void equiphero(account_name _user, uint8_t _hero_slot,uint8_t _item_location, uint64_t _item_index,uint8_t _item_slot)
        {
            print("equip item\n");
            item_controller.equip_hero_item(_user, _hero_slot,_item_location, _item_index,_item_slot);
        }
        //@abi action
        void unequiphero(account_name _user, uint8_t _hero_slot,uint8_t _item_slot)
        {
            print("un equip item\n");
            item_controller.unequip_hero_item(_user, _hero_slot,_item_slot);
        }
#pragma endregion


#pragma resion reset
        //@abi action
        void resetuser()
        {
            battle_controller.reset_all_battle_data();
            login_controller.reset_all_user_log_data();
            login_controller.reset_all_user_auth_data();
            gacha_controller.reset_all_user_object_data();
            party_controller.reset_all_user_party_data();
        }
        //@abi action
        void resetdata()
        {
            db_controller.reset_db_data();
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
// eos 금액에 대해 체크 하는 함

    EOSIO_ABI(cmain_logic,(datainit)(stageinit)(signup)(lookset)(statset)(completehero)(transfer)(startbattle)(activeturn)(setparty)(sellitem)(equipser)(unequipser)(equiphero)(unequiphero)(resetuser)(resetdata) )
