#pragma once
#include "../Common/common_header.hpp"
#include "../Table/log_table.hpp"
#include "../Table/auth_user_table.hpp"
#include "../Table/test_static_stage_table.hpp"
// need login & login time log
struct transfer_action {
    name from;
    std::string action;
    std::string param;
    uint32_t type;
    name seller;
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

  private:
    battle_data_table stage;
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
    clogin_system(account_name _self)
        : owner(_self),
          auth_user_table(_self, _self),
          user_log_table(_self, _self),
          stage(_self,_self)
    {
    }
    battle_data_table &get_battle_stage_table()
    {
        return stage;
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
        auto transfer_data = eosio::unpack_action_data<st_transfer>();
        eosio_assert(transfer_data.quantity.symbol == S(4, EOS), "only accepts EOS for deposits");
        eosio_assert(transfer_data.quantity.is_valid(), "Invalid token transfer");
        eosio_assert(transfer_data.quantity.amount > 0, "Quantity must be positive");

        auto log_find_iter = user_log_table.find(sender);
        user_log_table.modify(log_find_iter,owner,[&](auto &buy_log)
        {
            buy_log.l_use_eos+=transfer_data.quantity;
        });
        transfer_action res;
        size_t l_center = transfer_data.memo.find(':');
        size_t l_next = transfer_data.memo.find(':',l_center + 1);

        res.action = transfer_data.memo.substr(0, l_center);;
        if(l_next != std::string::npos)
        {
            res.type = atoi(transfer_data.memo.substr(l_center+1).c_str()); 
        }
        else
        {
            
        }
        res.from.value = sender;
        func(res);
    }
    void create_account(const account_name _user)
    {
        auto new_user_iter = auth_user_table.find(_user);
        eosio_assert(new_user_iter == auth_user_table.end(), "exist account");
        auth_user_table.emplace(owner, [&](auto &new_user) {
            new_user.auth_set_user(_user);
            new_user.a_state = euser_state::create;

            shero_info first_hero;
            first_hero.equip.resize(max_equip_slot);
            first_hero.current_state = ehero_state::set_look;
            
            new_user.a_hero_list.push_back(first_hero);
        });

        auto user_log_iter = user_log_table.find(_user);
        eosio_assert(user_log_iter == user_log_table.end(), "exist account");
        user_log_table.emplace(owner, [&](auto &new_log) {
            new_log.log_set_user(_user);
        });
    }
    void set_look(const account_name _user, uint8_t _hero_slot, uint8_t _head, uint8_t _hair,uint8_t _body)
    {
        auto user_iter = auth_user_table.find(_user);
        eosio_assert(user_iter != auth_user_table.end(), "unknown account");
        eosio_assert(user_iter->a_hero_slot >= _hero_slot,"need more character slot");
        eosio_assert(user_iter->a_hero_list[_hero_slot].current_state == ehero_state::set_look,"already completed look setting");

        auth_user_table.modify(user_iter, owner, [&](auto &hero_look_set) {
            hero_look_set.a_hero_list[_hero_slot].current_state = ehero_state::set_status;
            hero_look_set.a_hero_list[_hero_slot].look.head = _head;
            hero_look_set.a_hero_list[_hero_slot].look.hair = _hair;
            hero_look_set.a_hero_list[_hero_slot].look.body = _body;
        });
    }
    void set_status(const account_name _user, uint8_t _hero_slot)
    {
        auto user_iter = auth_user_table.find(_user);
        eosio_assert(user_iter != auth_user_table.end(), "unknown account");
        eosio_assert(user_iter->a_hero_slot >= _hero_slot,"need more character slot");
        eosio_assert(user_iter->a_hero_list[_hero_slot].current_state == ehero_state::set_status,"already completed status setting");

        auth_user_table.modify(user_iter, owner, [&](auto &hero_status_set) {
            hero_status_set.a_hero_list[_hero_slot].status.basic_str = random_value(10);
            hero_status_set.a_hero_list[_hero_slot].status.basic_dex = random_value(10);
            hero_status_set.a_hero_list[_hero_slot].status.basic_int = random_value(10);
        });
    }
    void complete_hero_set(account_name _user, uint8_t _hero_slot)
    {
        auto user_iter = auth_user_table.find(_user);
        eosio_assert(user_iter != auth_user_table.end(), "unknown account");
        eosio_assert(user_iter->a_hero_slot >= _hero_slot,"need more character slot");
        eosio_assert(user_iter->a_hero_list[_hero_slot].current_state == ehero_state::set_status,"already completed status setting");
    
        auth_user_table.modify(user_iter, owner, [&](auto &hero_state_set) {
            hero_state_set.a_hero_list[_hero_slot].current_state = ehero_state::set_complete;
        });
    }
    uint64_t random_value(uint32_t _range)
    {
        checksum256 l_result;
        uint64_t l_source = tapos_block_num() * tapos_block_prefix();
        sha256((char *)&l_source, sizeof(l_source), &l_result);
        uint64_t *l_p = reinterpret_cast<uint64_t *>(&l_result.hash);
        uint64_t l_random_result = *l_p % _range;
        if (l_random_result == 0)
        {
            l_random_result++;
        }
        return l_random_result;
    }
    void add_hero_slot(account_name _user)
    {
        auto user_iter = auth_user_table.find(_user);
        eosio_assert(user_iter != auth_user_table.end(),"not find user info");
        if (user_iter->a_hero_slot < max_charcterslot)
        {
            auth_user_table.modify(user_iter, owner, [&](auto &user_add_character) {
                shero_info new_hero;
                new_hero.equip.resize(max_equip_slot);
                new_hero.current_state = ehero_state::set_look;

                user_add_character.a_hero_list.push_back(new_hero);
                user_add_character.a_hero_slot+=1;
            });
        }
    }
    void add_item_slot(account_name _user)
    {
        auto user_iter = auth_user_table.find(_user);
        eosio_assert(user_iter != auth_user_table.end(), "not find user info");
        auth_user_table.modify(user_iter, owner, [&](auto &user_add_item_slot) {
            user_add_item_slot.a_item_slot += 10;
        });
    }
    void add_servant_slot(account_name _user)
    {
        auto user_iter = auth_user_table.find(_user);
        eosio_assert(user_iter != auth_user_table.end(), "not find user info");
        auth_user_table.modify(user_iter, owner, [&](auto &user_add_servant_slot) {
            user_add_servant_slot.a_servant_slot += 10;
        });
    }
    void add_monster_slot(account_name _user)
    {
        auto user_iter = auth_user_table.find(_user);
        eosio_assert(user_iter != auth_user_table.end(), "not find user info");
        auth_user_table.modify(user_iter, owner, [&](auto &user_add_monster_slot) {
            user_add_monster_slot.a_monster_slot += 10;
        });
    }

#pragma region static data test
    void init_stage_data()
    {
        uint32_t l_stage_count = 0;
        for (uint32_t j = 0; j < 200; ++j)
        {
            if((j!=0) &&(j%10 == 0))
            {
                l_stage_count++;
            }
            stage_info enemy_info;
            enemy_info.type_index = j;
            enemy_info.s_str = j + l_stage_count;
            enemy_info.s_dex = j + l_stage_count;
            enemy_info.s_int = j + l_stage_count;
            enemy_info.s_job = j;

            auto cur_stage_iter = stage.find(l_stage_count);
            if (cur_stage_iter == stage.end())
            {
                stage.emplace(owner, [&](auto &new_stage) {
                    new_stage.stage = l_stage_count;
                    new_stage.enemy_list.push_back(enemy_info);
                });
            }
            else
            {
                stage.modify(cur_stage_iter, owner, [&](auto &new_stage) {
                    new_stage.enemy_list.push_back(enemy_info);
                });
            }
        }
    }
    #pragma endregion
};