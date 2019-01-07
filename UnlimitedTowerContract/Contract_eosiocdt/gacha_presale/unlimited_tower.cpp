#include "Common/common_header.hpp"


#include "Token/token_account_table.hpp"
#include "Token/token_stat_table.hpp"
#include "Table/auth_user_table.hpp"
#include "Table/item_table.hpp"
#include "Table/log_table.hpp"
#include "Table/monster_table.hpp"
#include "Table/servant_table.hpp"
#include "Table/gacha_result_table.hpp"
#include "Table/gacha_accumulate_result_table.hpp"


#include "DB/db_system.hpp"

#include "Login/login_system.hpp"
#include "Token/token_system.hpp"
#include "Gacha/gacha_system.hpp"


    class unlimited_tower : public contract
    {
      private:
        ctoken_system token_controller;

      private:
        clogin_system login_controller;
        cgacha_system gacha_controller;
        cdb_system db_controller;

        const char *action_gacha="gacha";
    public:
        cmain_logic(account_name _self) :
        contract(_self) ,
        token_controller(_self,login_controller),
        login_controller(_self,db_controller),
        gacha_controller(_self,login_controller,db_controller,token_controller),
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
            login_controller.create_account(_user);
        }
        // eosio.token recipient
        // memo description spec
        //-------------------------------------------------------------------------
        void eostransfer(account_name sender, account_name receiver)
        {
            login_controller.eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
            if (ad.action.size() == 0)
            {
                print("action size zero\n");
            }
            else if(ad.action == action_gacha)
            {
                gacha_controller.start_gacha(sender,ad.type);
            }
            });
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

    EOSIO_ABI(cmain_logic,(create)(issue)(tokentrans)(setdata)(signup)(eostransfer)(initdata)(deleteuser)(initalluser)(inittoken) )
