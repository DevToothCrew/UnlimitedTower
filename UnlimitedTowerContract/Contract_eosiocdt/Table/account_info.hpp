#pragma once
#include "../Common/common_header.hpp"
#include "hero_info.hpp"
#include "monster_info.hpp"


    class [[eosio::table]] caccount
    {
    private:
        uint64_t user;
        std::string nick_name;
    public:
        uint64_t primary_key() const {return user;}
        void set_account_info(uint64_t _user, std::string _nickname)
        {
            user = _user;
            nick_name = _nickname;
        }

        uint64_t get_account_name() const {return user;}
        std::string get_account_nick_name() const {return nick_name;}
        EOSLIB_SERIALIZE(
            caccount,
            (user)
            (nick_name)
        )
    };
    typedef multi_index<name("caccount"),caccount> account_info_table;   
