#include "Common/common_header.hpp"

#define MAINTENANCE 0

#include "Token/token_account_table.hpp"
#include "Token/token_stat_table.hpp"
#include "Table/auth_user_table.hpp"
#include "Table/item_table.hpp"
#include "Table/log_table.hpp"
#include "Table/monster_table.hpp"
#include "Table/servant_table.hpp"
#include "Table/gacha_result_table.hpp"
#include "Table/gacha_accumulate_result_table.hpp"
#include "Table/party_table.hpp"



#include "DB/db_system.hpp"

#include "Login/login_system.hpp"
#include "Token/token_system.hpp"
#include "Gacha/gacha_system.hpp"
#include "Party/party_system.hpp"


    class cmain_logic : public contract
    {
      private:
        ctoken_system token_controller;

      private:
        clogin_system login_controller;
        cgacha_system gacha_controller;
        cparty_system party_controller;
        cdb_system db_controller;

        const char *action_change_stat="changestat";
        const char *action_gacha="gacha";
        const char *action_add_party="addparty";
    public:
        cmain_logic(account_name _self) :
        contract(_self) ,
        token_controller(_self,login_controller),
        login_controller(_self,db_controller),
        gacha_controller(_self,login_controller,db_controller,token_controller),
        party_controller(_self,login_controller,gacha_controller),
        db_controller(_self)
        {
            
        }
#pragma region token
        //@abi action
        void create(account_name _issuer, asset _maximum_supply)
        {
            token_controller.create(_issuer, _maximum_supply);
        }
        //@abi action
        void issue(account_name _to, asset _quantity, string _memo)
        {
            token_controller.issue(_to, _quantity, _memo);
        }
        //@abi action
        void tokentrans(account_name _from, account_name _to, asset _quantity, string _memo)
        {
            token_controller.transfer(_from, _to, _quantity, _memo);
        }
#pragma endregion

#pragma region set
        //@abi action
        void setdata()
        {
            db_controller.set_db_data();
        }
#pragma endregion


#pragma region Login
        //@abi action
        void signup(account_name _user)
        {
            print("account create\n");
            login_controller.create_account(_user);
            party_controller.party_init(_user);
        }
        //@abi action
        void lookset(account_name _user ,uint8_t _head,uint8_t _hair,uint8_t _body)
        {
            print("set look account from account\n");
            login_controller.set_look(_user,_head,_hair,_body);
        }
        //@abi action
        void statset(account_name _user)
        {
            print("set status account from account\n");
            login_controller.set_status(_user);
        }
        //@abi action
        void completehero(account_name _user)
        {
            login_controller.complete_hero_set(_user);
        }
        // eosio.token recipient
        // memo description spec
        //-------------------------------------------------------------------------
        void eostransfer(account_name sender, account_name receiver)
        {
            print("trasnfer\n");
            login_controller.eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
            if (ad.action.size() == 0)
            {
                print("action size zero\n");
            }
            else if(ad.action == action_gacha)
            {
                gacha_controller.start_gacha(sender,ad.type);
            }
            else if(ad.action == action_change_stat)
            {
                login_controller.change_status(sender);
            }
            else if (ad.action == action_add_party)
            {
                party_controller.add_party_list(sender);
            }
            });
        }
#pragma endregion

#pragma resion Party
        //@abi action
        void setparty(account_name _user,uint8_t _party_number,const std::vector<uint32_t> &_new_party_list)
        {
            party_controller.set_party(_user,_party_number,_new_party_list);
        }
#pragma endregion

#pragma resion cheat
        //@abi action
        void gachacheat(account_name _user)
        {
            gacha_controller.gacha_cheat(_user);
        }
        //@abi action
        void partycheat(account_name _user)
        {
            party_controller.set_automatic_party(_user);
        }
#pragma endregion



#pragma resion init db table
        //@abi action
        void initdata()
        {
            db_controller.init_db_data();
        }
#pragma endregion

#pragma resion delete user table
        //@abi action 
        void deleteuser(account_name _user)
        {
            login_controller.delete_user_data(_user);
            gacha_controller.delete_user_object_data(_user);
            gacha_controller.delete_user_gacha_result_data(_user);
        }
#pragma endregion

#pragma resion init all table
        //@abi action 
        void initalluser()
        {
            login_controller.init_all_user_auth_data();
            login_controller.init_all_user_log_data();
            gacha_controller.init_all_object_gacha_data();

        }
#pragma endregion

#pragma resion init token
        //@abi action
        void inittoken(asset _token)
        {
            token_controller.init_all_balance();
            token_controller.init_stat(_token);
        }
#pragma endregion
    };

#undef EOSIO_ABI

#define EOSIO_ABI( TYPE, MEMBERS ) \
extern "C" { \
    void apply( uint64_t receiver, uint64_t code, uint64_t action ) { \
        auto self = receiver; \
        TYPE thiscontract( self ); \
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
            execute_action(&thiscontract, &cmain_logic::eostransfer);\
        }\
    } \
}
// eos 금액에 대해 체크 하는 함

    EOSIO_ABI(cmain_logic,(create)(issue)(tokentrans)(setdata)(signup)(lookset)(statset)(completehero)(eostransfer)(setparty)(gachacheat)(partycheat)(initdata)(deleteuser)(initalluser)(inittoken) )
