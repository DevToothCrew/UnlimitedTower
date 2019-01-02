#pragma once
#include "../Common/common_header.hpp"
#include "../Table/log_table.hpp"
#include "../Table/auth_user_table.hpp"

struct transfer_action {
    name from;
    std::string action;
    std::string param;
    uint64_t type;
    name to;
    asset quantity;
};
enum job_list
{
    beginner = 0,
    warrior,
    archer,
    wizard,
    priest,
    thief,
};

class clogin_system
{
  private:
    account_name owner;
    auth_users auth_user_table;
    user_logs user_log_table;
    cdb_system &db_controller;

  private:
    const uint32_t max_equip_slot = 3;
    uint32_t hero_total_status = 24;

  public:
    struct st_transfer
    {
        account_name from;
        account_name to;
        asset quantity;
        string memo;
    };
  public:
    clogin_system(account_name _self,cdb_system &_db_controller)
        : owner(_self),
          db_controller(_db_controller),
          auth_user_table(_self, _self),
          user_log_table(_self, _self)
    {
    }
    auth_users &get_auth_user_table()
    {
        return auth_user_table;
    }

    user_logs &get_log_table()
    {
        return user_log_table;
    }

    uint64_t get_user_seed_value(uint64_t _user)
    {
        const auto &user_log_iter = user_log_table.get(_user,"not exist log in login seed");
        uint64_t user = _user + user_log_iter.gacha_num;
        return user;
    }

    template<typename T>
    void eosiotoken_transfer(account_name sender, account_name receiver, T func) 
    {
        require_auth2(sender,N(owner));
        auto transfer_data = eosio::unpack_action_data<st_transfer>();
        eosio_assert(transfer_data.quantity.symbol == S(4, EOS), "only accepts EOS for deposits");
        eosio_assert(transfer_data.quantity.is_valid(), "Invalid token transfer");
        eosio_assert(transfer_data.quantity.amount > 0, "Quantity must be positive");

        transfer_action res;
        size_t l_center = transfer_data.memo.find(':');
        
        res.action = transfer_data.memo.substr(0, l_center);
        
        if(res.action == "gacha")
        {
            size_t l_next = transfer_data.memo.find(':', l_center + 1);
            size_t l_end = transfer_data.memo.length() - (l_next + 1);

            eosio_assert(transfer_data.memo.find(':')!=std::string::npos,"seed memo [:] error");
            eosio_assert(transfer_data.memo.find(':',l_center + 1)!=std::string::npos,"seed memo [:] error");
            eosio_assert(transfer_data.quantity.amount == 10000, "gacha need 1.0000 EOS");

            std::string l_seed = transfer_data.memo.substr(l_center + 1, (l_next - l_center - 1));
            std::string l_sha = transfer_data.memo.substr(l_next + 1, l_end);
            
            res.type = safeseed::check_seed(l_seed, l_sha);

            eosio_assert(res.type != 0 ,"wrong seed convert");
        }

        auto user_log_iter = user_log_table.find(sender);
        eosio_assert(user_log_iter != user_log_table.end(),"not exist user log data");
        user_log_table.modify(user_log_iter, owner, [&](auto &buy_log) {
            buy_log.use_eos += transfer_data.quantity;
        });
        func(res);
    }
    void create_account(const account_name _user)
    {
        require_auth(_user);
        auto new_user_iter = auth_user_table.find(_user);
        eosio_assert(new_user_iter == auth_user_table.end(), "exist account");
        auth_user_table.emplace(owner, [&](auto &new_user) {
            new_user.auth_set_user(_user);
            new_user.state = euser_state::lobby;

            hero_info first_hero;
            first_hero.equip_slot.resize(max_equip_slot);
            first_hero.state = hero_state::set_look;
            
            new_user.hero = first_hero;
        });

        auto user_log_iter = user_log_table.find(_user);
        eosio_assert(user_log_iter == user_log_table.end(), "exist account");
        user_log_table.emplace(owner, [&](auto &new_log) {
            new_log.log_set_user(_user);
        });
    }

#pragma region reset
    void reset_all_user_auth_data()
    {
        require_auth2(owner, N(owner));
        for (auto user_auth_iter = auth_user_table.begin(); user_auth_iter != auth_user_table.end();)
        {
            auto iter = auth_user_table.find(user_auth_iter->primary_key());
            user_auth_iter++;
            auth_user_table.erase(iter);
        }
    }

    void reset_all_user_log_data()
    {
        require_auth2(owner, N(owner));
        for (auto user_log_iter = user_log_table.begin(); user_log_iter != user_log_table.end();)
        {
            auto iter = user_log_table.find(user_log_iter->primary_key());
            user_log_iter++;
            user_log_table.erase(iter);
        }
    }

    void delete_user_data(account_name _user)
    {
        require_auth2(owner, N(owner));
        auto user_auth_iter = auth_user_table.find(_user);
        eosio_assert(user_auth_iter != auth_user_table.end(), "not exist user auth data");
        auth_user_table.erase(user_auth_iter);

        auto user_log_iter = user_log_table.find(_user);
        eosio_assert(user_log_iter != user_log_table.end(), "not exist user auth data");
        user_log_table.erase(user_log_iter);
    }

    void reset_user_auth_data(account_name _user)
    {
        auto iter = auth_user_table.find(_user);
        eosio_assert(iter!=auth_user_table.end(),"not exist user auth data");
        auth_user_table.erase(iter);
    }

    void reset_user_log_data(account_name _user)
    {
        auto iter = user_log_table.find(_user);
        eosio_assert(iter!=user_log_table.end(),"not exist user auth data");
        user_log_table.erase(iter);
    }
#pragma endregion



};