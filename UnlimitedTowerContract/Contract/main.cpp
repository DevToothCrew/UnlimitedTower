#include "Common/common_header.hpp"
#include "Table/account_info.hpp"
#include "Login/login_system.hpp"

namespace eosio
{
    class [[eosio::contract]] cmain_logic : public contract
    {
    private:
        clogin_system login_controller;
    public:
        cmain_logic(eosio::name _self,eosio::name _code, datastream<const char*> _ds) :
        contract(_self,_code,_ds) ,
        login_controller(_self,_scope)
        {
            
        }

#region Login

        [[eosio::action]]
        void signup(uint64_t _user, string _nick_name)
        {
            login_controller.create(_user,_nick_name);
        }
       
        [[eosio::action]]
        void signin(uint64_t _user)
        {
            login_controller.login(_user);
        }

#endregion

#region Test

        [[eosio::action]]
        void init()
        {
            login_controller.init();
        }

#endregion

#region Gamble

        [[eosio::action]]
        void gamble(uint64_t _user,uint8_t _what_kind_gamble)
        {
            //login_controller.gamble(_user,_what_kind_gamble);
        }

#endregion

#region 

    };
    EOSIO_DISPATCH(cmain_logic,(signup)(signin)(init)(gamble))
}
