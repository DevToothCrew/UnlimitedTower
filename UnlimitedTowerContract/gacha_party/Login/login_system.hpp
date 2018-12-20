#pragma once
#include "../Common/common_header.hpp"
#include "../Table/log_table.hpp"
#include "../Table/auth_user_table.hpp"

struct transfer_action {
    name from;
    std::string action;
    std::string param;
    uint32_t type;
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
    const uint32_t max_charcterslot = 3;
    const uint32_t max_equip_slot = 3;

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
            eosio_assert(transfer_data.quantity.amount == 10000,"gacha need 1.0000 EOS");
        }
        else if (res.action == "addparty")
        {
            eosio_assert(transfer_data.quantity.amount == 10000, "add party need 1.0000 EOS");
        }

        res.to.value = receiver;
        res.from.value = sender;

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
            new_user.state = euser_state::login;

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

    void set_look(const account_name _user, uint8_t _head, uint8_t _hair,uint8_t _body)
    {
        require_auth(_user);

        auto &user_head_db = db_controller.get_head_db_table();
        const auto &head_db_iter = user_head_db.get(_head, "not exist head info");

        auto &user_hair_db = db_controller.get_hair_db_table();
        const auto &hair_db_iter = user_hair_db.get(_hair, "not exist hair info");

        auto &user_body_db = db_controller.get_body_db_table();
        const auto &body_db_iter = user_body_db.get(_body, "not exist body info");

        auto user_iter = auth_user_table.find(_user);
        eosio_assert(user_iter != auth_user_table.end(), "unknown account");
        eosio_assert(user_iter->hero.state == hero_state::set_look,"already completed look setting");

        auth_user_table.modify(user_iter, owner, [&](auto &hero_look_set) {
            hero_look_set.hero.state = hero_state::set_status;
            hero_look_set.hero.appear.head = _head;
            hero_look_set.hero.appear.hair = _hair;
            hero_look_set.hero.appear.body = _body;
        });
    }

    void set_status(account_name _user)
    {
        require_auth(_user);
        auto user_iter = auth_user_table.find(_user);
        eosio_assert(user_iter != auth_user_table.end(), "unknown account");
        eosio_assert(user_iter->hero.state == hero_state::set_status,"free roulette completed status setting");

        uint64_t l_seed = safeseed::get_seed(_user);
        auth_user_table.modify(user_iter, owner, [&](auto &hero_status_set) {
            hero_status_set.hero.state = hero_state::set_change_status;
            hero_status_set.hero.status.basic_str = safeseed::get_random_value(l_seed,10,1,1);
            hero_status_set.hero.status.basic_dex = safeseed::get_random_value(l_seed,10,1,2);
            hero_status_set.hero.status.basic_int = safeseed::get_random_value(l_seed,10,1,3);
        });
    }

    void change_status(account_name _user)
    {
        auto user_iter = auth_user_table.find(_user);
        eosio_assert(user_iter != auth_user_table.end(), "unknown account");
        eosio_assert(user_iter->hero.state == hero_state::set_change_status, "already completed status setting");

        uint64_t l_seed = safeseed::get_seed(_user);
        auth_user_table.modify(user_iter, owner, [&](auto &hero_status_change) {
            hero_status_change.hero.status.basic_str = safeseed::get_random_value(l_seed,10,1,1);
            hero_status_change.hero.status.basic_dex = safeseed::get_random_value(l_seed,10,1,2);
            hero_status_change.hero.status.basic_int = safeseed::get_random_value(l_seed,10,1,3);
        });
    }

    void complete_hero_set(account_name _user)
    {
        require_auth(_user);
        auto user_iter = auth_user_table.find(_user);
        eosio_assert(user_iter != auth_user_table.end(), "unknown account");
        eosio_assert(user_iter->hero.state == hero_state::set_change_status || user_iter->hero.state == hero_state::set_status,"need to look setting & status setting");
    
        auth_user_table.modify(user_iter, owner, [&](auto &hero_state_set) {
            hero_state_set.hero.state = hero_state::set_complete;
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