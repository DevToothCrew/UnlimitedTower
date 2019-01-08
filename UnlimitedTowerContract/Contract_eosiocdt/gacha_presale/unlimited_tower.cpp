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

//------------------------------------------------------------------------//
//----------------------------unlimited_tower_action----------------------//
//------------------------------------------------------------------------//
#pragma region unlimited_tower ACTION
        
        ACTION create(eosio::name _issuer, asset _maximum_supply)
        {
            token_controller.create(_issuer, _maximum_supply);
        }        
        
        ACTION issue(eosio::name _to, asset _quantity, string _memo)
        {
            token_controller.issue(_to, _quantity, _memo);
        }
        
        ACTION tokentrans(eosio::name _from, eosio::name _to, asset _quantity, string _memo)
        {
            token_controller.transfer(_from, _to, _quantity, _memo);
        }
#pragma endregion

#pragma region set        
        ACTION setdata()
        {
            db_controller.set_db_data();
        }
#pragma endregion


#pragma region Login
        //@abi action
        ACTION signup(eosio::name _user)
        {
            login_controller.create_account(_user);
        }
        // eosio.token recipient
        // memo description spec
        //-------------------------------------------------------------------------
        void eostransfer(eosio::name sender, eosio::name receiver)
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

        ACTION initdata()
        {
            db_controller.init_db_data();
        }
#pragma endregion

#pragma resion delete user table

        ACTION deleteuser(eosio::name _user)
        {
            login_controller.delete_user_data(_user);
            gacha_controller.delete_user_object_data(_user);
            gacha_controller.delete_user_gacha_result_data(_user);
        }
#pragma endregion

#pragma resion init all table

        ACTION initalluser()
        {
            login_controller.init_all_user_auth_data();
            login_controller.init_all_user_log_data();
            gacha_controller.init_all_object_gacha_data();

        }
#pragma endregion

#pragma resion init token

        ACTION inittoken(asset _token)
        {
            token_controller.init_all_balance();
            token_controller.init_stat(_token);
        }
#pragma endregion

//------------------------------------------------------------------------//
//-------------------------------login_function---------------------------//
//------------------------------------------------------------------------//
#pragma resion login_function

template<typename T>
    void eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func) 
    {
        require_auth(sender);
        auto transfer_data = eosio::unpack_action_data<st_transfer>();
        eosio_assert(transfer_data.quantity.symbol == symbol("EOS",4), "only accepts EOS for deposits");
        eosio_assert(transfer_data.quantity.is_valid(), "Invalid token transfer");
        eosio_assert(transfer_data.quantity.amount > 0, "Quantity must be positive");

        transfer_action res;
        size_t l_center = transfer_data.memo.find(':');

        res.action = transfer_data.memo.substr(0, l_center);
        if(res.action == "gacha")
        {
            eosio_assert(transfer_data.quantity.amount == 10000, "gacha need 1.0000 EOS");
        }
        else if (res.action == "addparty")
        {
            eosio_assert(transfer_data.quantity.amount == 10000, "add party need 1.0000 EOS");
        }
        else if (res.action == "changestat")
        {
            eosio_assert(transfer_data.quantity.amount == 1000, "change stat need 0.1000 EOS");
        }

        res.to.value = receiver.value;
        res.from.value = sender.value;

        auto user_log_iter = user_log_table.find(sender.value);
        eosio_assert(user_log_iter != user_log_table.end(),"not exist user log data");
        user_log_table.modify(user_log_iter, owner, [&](auto &buy_log) {
            buy_log.use_eos += transfer_data.quantity;
        });
        func(res);
    }
    void create_account(const eosio::name _user)
    {
        require_auth(_user);
        auto new_user_iter = auth_user_table.find(_user.value);
        eosio_assert(new_user_iter == auth_user_table.end(), "exist account");
        auth_user_table.emplace(owner, [&](auto &new_user) {
            new_user.auth_set_user(_user.value);
            new_user.state = euser_state::login;

            hero_info first_hero;
            first_hero.equip_slot.resize(max_equip_slot);
            first_hero.state = hero_state::set_look;
            
            new_user.hero = first_hero;
        });

        auto user_log_iter = user_log_table.find(_user.value);
        eosio_assert(user_log_iter == user_log_table.end(), "exist account");
        user_log_table.emplace(owner, [&](auto &new_log) {
            new_log.log_set_user(_user.value);
        });
    }

    void set_look(const eosio::name _user, uint8_t _head, uint8_t _hair,uint8_t _body)
    {
        require_auth(_user);

        auto &user_head_db = db_controller.get_head_db_table();
        const auto &head_db_iter = user_head_db.get(_head, "not exist head info");

        auto &user_hair_db = db_controller.get_hair_db_table();
        const auto &hair_db_iter = user_hair_db.get(_hair, "not exist hair info");

        auto &user_body_db = db_controller.get_body_db_table();
        const auto &body_db_iter = user_body_db.get(_body, "not exist body info");

        auto user_iter = auth_user_table.find(_user.value);
        eosio_assert(user_iter != auth_user_table.end(), "unknown account");
        eosio_assert(user_iter->hero.state == hero_state::set_look,"already completed look setting");

        auth_user_table.modify(user_iter, owner, [&](auto &hero_look_set) {
            hero_look_set.hero.state = hero_state::set_status;
            hero_look_set.hero.appear.head = _head;
            hero_look_set.hero.appear.hair = _hair;
            hero_look_set.hero.appear.body = _body;
        });
    }

    void set_status(eosio::name _user)
    {
        require_auth(_user);
        auto user_iter = auth_user_table.find(_user.value);
        eosio_assert(user_iter != auth_user_table.end(), "unknown account");
        eosio_assert(user_iter->hero.state == hero_state::set_status,"free roulette completed status setting");

        uint64_t l_seed = safeseed::get_seed(_user.value);

        std::vector<uint64_t> randoms;
        auto &random_value = safeseed::get_rand(randoms);

         auth_user_table.modify(user_iter, owner, [&](auto &hero_status_set) {
            hero_status_set.hero.state = hero_state::set_change_status;

            hero_status_set.hero.status.basic_str = random_value[0];
            hero_status_set.hero.status.basic_dex = random_value[1];
            hero_status_set.hero.status.basic_int = random_value[2];
        });
    }

    void change_status(eosio::name _user)
    {
        auto user_iter = auth_user_table.find(_user.value);
        eosio_assert(user_iter != auth_user_table.end(), "unknown account");
        eosio_assert(user_iter->hero.state == hero_state::set_change_status, "already completed status setting");

        uint64_t l_seed = safeseed::get_seed(_user.value);

        std::vector<uint64_t> randoms;
        auto &random_value = safeseed::get_rand(randoms);

        auth_user_table.modify(user_iter, owner, [&](auto &hero_status_change) {

            hero_status_change.hero.status.basic_str = random_value[0];
            hero_status_change.hero.status.basic_dex = random_value[1];
            hero_status_change.hero.status.basic_int = random_value[2];
        });
    }

    void complete_hero_set(eosio::name _user)
    {
        require_auth(_user);
        auto user_iter = auth_user_table.find(_user.value);
        eosio_assert(user_iter != auth_user_table.end(), "unknown account");
        eosio_assert(user_iter->hero.state == hero_state::set_change_status || user_iter->hero.state == hero_state::set_status,"need to look setting & status setting");
    
        auth_user_table.modify(user_iter, owner, [&](auto &hero_state_set) {
            hero_state_set.hero.state = hero_state::set_complete;
        });
    }
#pragma endregion

//------------------------------------------------------------------------//
//-------------------------------gacha_function---------------------------//
//------------------------------------------------------------------------//


    void gacha_servant_job(eosio::name _user,uint64_t _seed)
    {
        uint8_t random_job = safeseed::get_random_value(_seed,db_controller.servant_job_count,default_min,servant_random_count);
        const auto &servant_db_iter = servant_db_table.get(random_job,"not get servant job data");

        auto user_log_iter = user_log_table.find(_user.value);
        eosio_assert(user_log_iter != user_log_table.end(),"not exist user log data");

        result_info result;
        user_servants user_servant_table(owner, _user.value);
        user_servant_table.emplace( owner, [&](auto &update_user_servant_list) {
        uint32_t first_index = user_servant_table.available_primary_key();
        if (first_index == 0)
        {
            update_user_servant_list.index = 1;
        }
        else
        {
            update_user_servant_list.index = user_servant_table.available_primary_key();
        }
                
        servant_info new_servant;
        servant_random_count+=1;
        new_servant.appear.hair = gacha_servant_hair(_seed,servant_random_count);
        servant_random_count+=1;
        new_servant.appear.hair = gacha_servant_head(_seed,servant_random_count);
        servant_random_count+=1;
        new_servant.appear.body = gacha_servant_body(_seed,servant_random_count);
        new_servant.job = servant_db_iter.job;
        servant_random_count+=1;
        new_servant.status.basic_str = safeseed::get_random_value(_seed,servant_db_iter.max_range.base_str,servant_db_iter.min_range.base_str,servant_random_count);
        servant_random_count+=1;
        new_servant.status.basic_dex = safeseed::get_random_value(_seed,servant_db_iter.max_range.base_dex,servant_db_iter.min_range.base_dex,servant_random_count);
        servant_random_count+=1;
        new_servant.status.basic_int = safeseed::get_random_value(_seed,servant_db_iter.max_range.base_int,servant_db_iter.min_range.base_int,servant_random_count);
        new_servant.equip_slot.resize(3);
        new_servant.state = eobject_state::on_inventory;

        result.index = update_user_servant_list.index;
        result.type = result::servant;
                
        update_user_servant_list.servant = new_servant;
        });

        auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
        if(user_gacha_result_iter == user_gacha_result_table.end())
        {
            user_gacha_result_table.emplace(owner, [&](auto &new_result)
            {
                new_result.user = _user.value;
                new_result.result = result;
            });
        }
        else
        {
            user_gacha_result_table.modify(user_gacha_result_iter, owner, [&](auto &new_result)
            {
                new_result.result = result;
            });
        }

        auto user_gacha_accumulate_iter = user_gacha_accumulate_table.find(_user.value);
        if(user_gacha_accumulate_iter == user_gacha_accumulate_table.end())
        {
            user_gacha_accumulate_table.emplace(owner, [&](auto &new_result)
            {
                new_result.user = _user.value;
                new_result.result_list.push_back(result);
            });
        }
        else
        {
            user_gacha_accumulate_table.modify(user_gacha_accumulate_iter, owner, [&](auto &new_result)
            {
                new_result.result_list.push_back(result);
            });
        }


        //로그 남기는 부분
        user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
            update_log.servant_num++;
            update_log.gacha_num++;
             });
        }

        uint8_t gacha_servant_head(uint64_t _seed,uint32_t _count)
        {
            uint8_t random_head = safeseed::get_random_value(_seed,db_controller.head_count,default_min,_count);
            const auto &head_db_iter = head_db_table.get(random_head,"not exist head info");
            return head_db_iter.head;
        }

        uint8_t gacha_servant_hair(uint64_t _seed,uint32_t _count)
        {
            uint8_t random_hair = safeseed::get_random_value(_seed,db_controller.hair_count,default_min,_count);
            const auto &hair_db_iter = hair_db_table.get(random_hair,"not exist hair info");
            return hair_db_iter.hair;
        }

        uint8_t gacha_servant_body(uint64_t _seed,uint32_t _count)
        {
            uint8_t random_body = safeseed::get_random_value(_seed,db_controller.body_count,default_min,_count);
            const auto &body_db_iter = body_db_table.get(random_body, "not exist body info");
            return body_db_iter.body;
        }

        void gacha_monster_id(eosio::name _user,uint64_t _seed)
        {   
            uint8_t random_monster_id = safeseed::get_random_value(_seed,db_controller.monster_id_count,default_min,monster_random_count);
            const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id,"not exist monster id");

            monster_random_count+=1;
            uint32_t random_rate = safeseed::get_random_value(_seed,max_rate,default_min,monster_random_count);
            uint8_t random_grade;
            if(random_rate < grade_five_rate)
            {
                random_grade = 4;
            }
            else if(random_rate < grade_four_rate)
            {
                random_grade = 3;
            }
            else
            {
                random_grade = 2;
            }

            const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade,"not exist monster grade");

            auto user_log_iter = user_log_table.find(_user.value);
            eosio_assert(user_log_iter != user_log_table.end(),"not exist user log data");

            result_info result;
            user_monsters user_monster_table(owner, _user.value);
            user_monster_table.emplace(owner, [&](auto &update_user_monster_list) {
                uint32_t first_index = user_monster_table.available_primary_key();
                if (first_index == 0)
                {
                    update_user_monster_list.index = 1;
                }
                else
                {
                    update_user_monster_list.index = user_monster_table.available_primary_key();
                }

                monster_info new_monster;
                new_monster.look = monster_id_db_iter.look;
                new_monster.grade = monster_grade_db_iter.monster_grade;
                monster_random_count+=1;
                new_monster.status.basic_str = safeseed::get_random_value(_seed,monster_grade_db_iter.max_range.base_str,monster_grade_db_iter.min_range.base_str,monster_random_count);
                monster_random_count+=1;
                new_monster.status.basic_dex = safeseed::get_random_value(_seed,monster_grade_db_iter.max_range.base_dex,monster_grade_db_iter.min_range.base_dex,monster_random_count);
                monster_random_count+=1;
                new_monster.status.basic_int = safeseed::get_random_value(_seed,monster_grade_db_iter.max_range.base_int,monster_grade_db_iter.min_range.base_int,monster_random_count);
                new_monster.state = eobject_state::on_inventory;

                result.index = update_user_monster_list.index;
                result.type = result::monster;
                
                update_user_monster_list.monster = new_monster;
            });

            auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
            if(user_gacha_result_iter == user_gacha_result_table.end())
            {
                user_gacha_result_table.emplace(owner, [&](auto &new_result)
                {
                    new_result.user = _user.value;
                    new_result.result = result;
                });
            }
            else
            {
                user_gacha_result_table.modify(user_gacha_result_iter, owner, [&](auto &new_result)
                {
                    new_result.result = result;
                });
            }


            auto user_gacha_accumulate_iter = user_gacha_accumulate_table.find(_user.value);
            if(user_gacha_accumulate_iter == user_gacha_accumulate_table.end())
            {
                user_gacha_accumulate_table.emplace(owner, [&](auto &new_result)
                {
                    new_result.user = _user.value;
                    new_result.result_list.push_back(result);
                });
            }
            else
            {
                user_gacha_accumulate_table.modify(user_gacha_accumulate_iter, owner, [&](auto &new_result)
                {
                    new_result.result_list.push_back(result);
                });
            }

            user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
                update_log.gacha_num++;
                update_log.monster_num++;
            });
        }

        void gacha_item_id(eosio::name _user,uint64_t _seed)
        {
            uint8_t random_item_id = safeseed::get_random_value(_seed,db_controller.item_id_count,default_min,item_random_count);
            const auto &item_id_db_iter = item_id_db_table.get(random_item_id, "not exist item id");

            item_random_count+=1;
            uint8_t random_item_tier = safeseed::get_random_value(_seed,db_controller.item_tier_count,default_min,item_random_count);
            const auto &item_tier_db_iter = item_tier_db_table.get(random_item_tier,"not exist tier info");

            item_random_count+=1;
            uint8_t random_item_grade = safeseed::get_random_value(_seed,db_controller.item_grade_count,default_min,item_random_count);
            const auto &item_grade_db_iter = item_grade_db_table.get(random_item_grade,"not exist tier info");


            auto user_log_iter = user_log_table.find(_user.value);
            eosio_assert(user_log_iter != user_log_table.end(),"not exist user log data");

            result_info result;
            user_items user_item_table(owner, _user.value);
            user_item_table.emplace(owner, [&](auto &update_user_item_list) {
                uint32_t first_index = user_item_table.available_primary_key();
                if(first_index == 0)
                {
                    update_user_item_list.index = 1;
                }
                else
                {
                    update_user_item_list.index = user_item_table.available_primary_key();
                }

                item_info new_item;
                new_item.id = item_id_db_iter.id;
                new_item.slot = item_id_db_iter.slot;
                new_item.tier = item_tier_db_iter.tier;
                item_random_count+=1;
                new_item.status.basic_str = safeseed::get_random_value(_seed,item_grade_db_iter.max_range.base_str,item_grade_db_iter.min_range.base_str,item_random_count);
                item_random_count+=1;
                new_item.status.basic_dex = safeseed::get_random_value(_seed,item_grade_db_iter.max_range.base_dex,item_grade_db_iter.min_range.base_dex,item_random_count);
                item_random_count+=1;
                new_item.status.basic_int = safeseed::get_random_value(_seed,item_grade_db_iter.max_range.base_int,item_grade_db_iter.min_range.base_int,item_random_count);
                new_item.job = item_id_db_iter.job;
                new_item.state = eobject_state::on_inventory;
                new_item.grade = item_grade_db_iter.grade;

                result.index = update_user_item_list.index;
                result.type = result::item;

                update_user_item_list.item = new_item;
            });


            auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
            if(user_gacha_result_iter == user_gacha_result_table.end())
            {
                user_gacha_result_table.emplace(owner, [&](auto &new_result)
                {
                    new_result.user = _user.value;
                    new_result.result = result;
                });
            }
            else
            {
                user_gacha_result_table.modify(user_gacha_result_iter, owner, [&](auto &new_result)
                {
                    new_result.result = result;
                });
            }

            auto user_gacha_accumulate_iter = user_gacha_accumulate_table.find(_user.value);
            if(user_gacha_accumulate_iter == user_gacha_accumulate_table.end())
            {
                user_gacha_accumulate_table.emplace(owner, [&](auto &new_result)
                {
                    new_result.user = _user.value;
                    new_result.result_list.push_back(result);
                });
            }
            else
            {
                user_gacha_accumulate_table.modify(user_gacha_accumulate_iter, owner, [&](auto &new_result)
                {
                    new_result.result_list.push_back(result);
                });
            }


            user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
                update_log.item_num++;
                update_log.gacha_num++;
            });
        }

        void start_gacha(eosio::name _user)
        {
            auto user_log_iter = user_log_table.find(_user.value);
            eosio_assert(user_log_iter != user_log_table.end(),"unknown account");

            uint64_t l_seed = safeseed::get_seed(_user.value);

            if(user_log_iter->gacha_num == 0)
            {
                gacha_monster_id(_user,l_seed);
            }
            else
            {
                uint64_t l_gacha_result_type = safeseed::get_random_value(l_seed,max_rate,default_min,DEFAULE_RANDOM_COUNT);
                if(l_gacha_result_type < 33333)
                {
                    gacha_servant_job(_user,l_seed);
                }
                else if(l_gacha_result_type > 33333 && l_gacha_result_type <= 66666)
                {
                    gacha_monster_id(_user,l_seed);
                }
                else
                {
                    gacha_item_id(_user,l_seed);
                }
            }
            servant_random_count = 0;
            monster_random_count = 0;
            item_random_count = 0;
        }







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

    EOSIO_DISPATCH(unlimited_tower,(create)(issue)(tokentrans)(setdata)(signup)(eostransfer)(initdata)(deleteuser)(initalluser)(inittoken) )
