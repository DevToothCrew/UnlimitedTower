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
#pragma region Token action

ACTION unlimited_tower::create(name issuer, asset maximum_supply)
{
    require_auth(owner);

    auto sym = maximum_supply.symbol;
    eosio_assert(sym.is_valid(), "Invalid symbol name");
    eosio_assert(maximum_supply.is_valid(), "Invalid Supply");
    
    eosio_assert(maximum_supply.amount > 0, "Max-supply must be positive");

    stat statstable(owner, sym.code().raw());
    auto existing = statstable.find(sym.code().raw());
    eosio_assert(existing == statstable.end(), "Tokenwith symbol already exists");

    statstable.emplace(owner, [&](auto &s) {
        s.supply.symbol = maximum_supply.symbol;
        s.max_supply = maximum_supply;
        s.issuer = issuer;
    });
}

ACTION unlimited_tower::issue(name to, asset quantity, string memo)
    {
       auto sym = quantity.symbol;
       eosio_assert(sym.is_valid(), "Invalid symbol name");
       eosio_assert(memo.size() <= 256, "Memo has more than 256 bytes");

       uint64_t sym_name = sym.code().raw();
       stat statstable(owner, sym_name);
       auto existing = statstable.find(sym.code().raw());
       eosio_assert(existing != statstable.end(), "Token with symbol does now exist, Create token before issue");
       const auto& st = *existing;

       require_auth(st.issuer);
       eosio_assert(quantity.is_valid(), "Invalid quantity");
       eosio_assert(quantity.amount > 0, "Must issue positive quantity");

       eosio_assert(quantity.symbol == st.supply.symbol, "Symbol precision mismatch");
       eosio_assert(quantity.amount <= st.max_supply.amount - st.supply.amount, "Quantity exceeds available supply");

       statstable.modify(st, owner, [&](auto& s){
           s.supply += quantity;
       });

       add_balance(st.issuer, quantity, st.issuer);

        if (to != st.issuer)
        {
            action(permission_level{st.issuer, "active"_n},
                   st.issuer, "tokentrans"_n,
                   std::make_tuple(st.issuer, to, quantity, memo))
                .send();
        }
    }

ACTION unlimited_tower::tokentrans(name from, name to, asset quantity, string memo)
{
    eosio_assert(from != to, "Cannot transfer to self");
       require_auth(from);
       eosio_assert(is_account(to), "To account does not exist");
       auto sym = quantity.symbol.code().raw();
       stat statstable(owner, sym);
       const auto& st = statstable.get(sym, "Not exist symbol");

       require_recipient(from);
       require_recipient(to);

       eosio_assert(quantity.is_valid(), "Invalid quantity");
       eosio_assert(quantity.amount > 0, "Must transfer positive quantity");
       eosio_assert(quantity.symbol == st.supply.symbol, "Symbol precision mismatch");
       eosio_assert(memo.size() <= 250, "Memo has more than 256 bytes");

       sub_balance(from, quantity);
       add_balance(to, quantity, from);
}
#pragma endregion

#pragma region set
ACTION unlimited_tower::setdata()
{
    set_db_data();
}
#pragma endregion

#pragma region Login
ACTION unlimited_tower::signup(eosio::name _user)
{
    create_account(_user);
}
// eosio.token recipient
// memo description spec
//-------------------------------------------------------------------------
void unlimited_tower::eostransfer(eosio::name sender, eosio::name receiver)
{
    eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
        if (ad.action.size() == 0)
        {
            print("action size zero\n");
        }
        else if (ad.action == action_gacha)
        {
            gacha_controller.start_gacha(sender, ad.type);
        }
    });
}
#pragma endregion

#pragma resion init db table

ACTION unlimited_tower::initdata()
{
    init_db_data();
}
#pragma endregion

#pragma resion delete user table

ACTION unlimited_tower::deleteuser(eosio::name _user)
{
    delete_user_data(_user);
    delete_user_object_data(_user);
    delete_user_gacha_result_data(_user);
}
#pragma endregion

#pragma resion init all table

ACTION unlimited_tower::initalluser()
{
    init_all_user_auth_data();
    init_all_user_log_data();
    init_all_object_gacha_data();
}
#pragma endregion

#pragma resion init token

ACTION unlimited_tower::inittoken(asset _token)
{
    init_all_balance();
    init_stat(_token);
}
#pragma endregion

//------------------------------------------------------------------------//
//-------------------------------login_function---------------------------//
//------------------------------------------------------------------------//
#pragma resion login_function

template <typename T>
void unlimited_tower::eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func)
{
    require_auth(sender);
    auto transfer_data = eosio::unpack_action_data<st_transfer>();
    eosio_assert(transfer_data.quantity.symbol == symbol("EOS", 4), "only accepts EOS for deposits");
    eosio_assert(transfer_data.quantity.is_valid(), "Invalid token transfer");
    eosio_assert(transfer_data.quantity.amount > 0, "Quantity must be positive");

    transfer_action res;
    size_t l_center = transfer_data.memo.find(':');

    res.action = transfer_data.memo.substr(0, l_center);
    if (res.action == "gacha")
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
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data");
    user_log_table.modify(user_log_iter, owner, [&](auto &buy_log) {
        buy_log.use_eos += transfer_data.quantity;
    });
    func(res);
}
void unlimited_tower::create_account(const eosio::name _user)
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

void unlimited_tower::set_look(const eosio::name _user, uint8_t _head, uint8_t _hair, uint8_t _body)
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
    eosio_assert(user_iter->hero.state == hero_state::set_look, "already completed look setting");

    auth_user_table.modify(user_iter, owner, [&](auto &hero_look_set) {
        hero_look_set.hero.state = hero_state::set_status;
        hero_look_set.hero.appear.head = _head;
        hero_look_set.hero.appear.hair = _hair;
        hero_look_set.hero.appear.body = _body;
    });
}

void unlimited_tower::set_status(eosio::name _user)
{
    require_auth(_user);
    auto user_iter = auth_user_table.find(_user.value);
    eosio_assert(user_iter != auth_user_table.end(), "unknown account");
    eosio_assert(user_iter->hero.state == hero_state::set_status, "free roulette completed status setting");

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

void unlimited_tower::change_status(eosio::name _user)
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

void unlimited_tower::complete_hero_set(eosio::name _user)
{
    require_auth(_user);
    auto user_iter = auth_user_table.find(_user.value);
    eosio_assert(user_iter != auth_user_table.end(), "unknown account");
    eosio_assert(user_iter->hero.state == hero_state::set_change_status || user_iter->hero.state == hero_state::set_status, "need to look setting & status setting");

    auth_user_table.modify(user_iter, owner, [&](auto &hero_state_set) {
        hero_state_set.hero.state = hero_state::set_complete;
    });
}
#pragma endregion

//------------------------------------------------------------------------//
//-------------------------------gacha_function---------------------------//
//------------------------------------------------------------------------//

void unlimited_tower::gacha_servant_job(eosio::name _user, uint64_t _seed)
{
    uint8_t random_job = safeseed::get_random_value(_seed, db_controller.servant_job_count, default_min, servant_random_count);
    const auto &servant_db_iter = servant_db_table.get(random_job, "not get servant job data");

    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data");

    result_info result;
    user_servants user_servant_table(owner, _user.value);
    user_servant_table.emplace(owner, [&](auto &update_user_servant_list) {
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
        servant_random_count += 1;
        new_servant.appear.hair = gacha_servant_hair(_seed, servant_random_count);
        servant_random_count += 1;
        new_servant.appear.hair = gacha_servant_head(_seed, servant_random_count);
        servant_random_count += 1;
        new_servant.appear.body = gacha_servant_body(_seed, servant_random_count);
        new_servant.job = servant_db_iter.job;
        servant_random_count += 1;
        new_servant.status.basic_str = safeseed::get_random_value(_seed, servant_db_iter.max_range.base_str, servant_db_iter.min_range.base_str, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_dex = safeseed::get_random_value(_seed, servant_db_iter.max_range.base_dex, servant_db_iter.min_range.base_dex, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_int = safeseed::get_random_value(_seed, servant_db_iter.max_range.base_int, servant_db_iter.min_range.base_int, servant_random_count);
        new_servant.equip_slot.resize(3);
        new_servant.state = eobject_state::on_inventory;

        result.index = update_user_servant_list.index;
        result.type = result::servant;

        update_user_servant_list.servant = new_servant;
    });

    auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
    if (user_gacha_result_iter == user_gacha_result_table.end())
    {
        user_gacha_result_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user.value;
            new_result.result = result;
        });
    }
    else
    {
        user_gacha_result_table.modify(user_gacha_result_iter, owner, [&](auto &new_result) {
            new_result.result = result;
        });
    }

    auto user_gacha_accumulate_iter = user_gacha_accumulate_table.find(_user.value);
    if (user_gacha_accumulate_iter == user_gacha_accumulate_table.end())
    {
        user_gacha_accumulate_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user.value;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_accumulate_table.modify(user_gacha_accumulate_iter, owner, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    //로그 남기는 부분
    user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
        update_log.servant_num++;
        update_log.gacha_num++;
    });
}

uint8_t unlimited_tower::gacha_servant_head(uint64_t _seed, uint32_t _count)
{
    uint8_t random_head = safeseed::get_random_value(_seed, db_controller.head_count, default_min, _count);
    const auto &head_db_iter = head_db_table.get(random_head, "not exist head info");
    return head_db_iter.head;
}

uint8_t unlimited_tower::gacha_servant_hair(uint64_t _seed, uint32_t _count)
{
    uint8_t random_hair = safeseed::get_random_value(_seed, db_controller.hair_count, default_min, _count);
    const auto &hair_db_iter = hair_db_table.get(random_hair, "not exist hair info");
    return hair_db_iter.hair;
}

uint8_t unlimited_tower::gacha_servant_body(uint64_t _seed, uint32_t _count)
{
    uint8_t random_body = safeseed::get_random_value(_seed, db_controller.body_count, default_min, _count);
    const auto &body_db_iter = body_db_table.get(random_body, "not exist body info");
    return body_db_iter.body;
}

void unlimited_tower::gacha_monster_id(eosio::name _user, uint64_t _seed)
{
    uint8_t random_monster_id = safeseed::get_random_value(_seed, db_controller.monster_id_count, default_min, monster_random_count);
    const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id, "not exist monster id");

    monster_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, monster_random_count);
    uint8_t random_grade;
    if (random_rate < grade_five_rate)
    {
        random_grade = 4;
    }
    else if (random_rate < grade_four_rate)
    {
        random_grade = 3;
    }
    else
    {
        random_grade = 2;
    }

    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "not exist monster grade");

    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data");

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
        monster_random_count += 1;
        new_monster.status.basic_str = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_str, monster_grade_db_iter.min_range.base_str, monster_random_count);
        monster_random_count += 1;
        new_monster.status.basic_dex = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_dex, monster_grade_db_iter.min_range.base_dex, monster_random_count);
        monster_random_count += 1;
        new_monster.status.basic_int = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_int, monster_grade_db_iter.min_range.base_int, monster_random_count);
        new_monster.state = eobject_state::on_inventory;

        result.index = update_user_monster_list.index;
        result.type = result::monster;

        update_user_monster_list.monster = new_monster;
    });

    auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
    if (user_gacha_result_iter == user_gacha_result_table.end())
    {
        user_gacha_result_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user.value;
            new_result.result = result;
        });
    }
    else
    {
        user_gacha_result_table.modify(user_gacha_result_iter, owner, [&](auto &new_result) {
            new_result.result = result;
        });
    }

    auto user_gacha_accumulate_iter = user_gacha_accumulate_table.find(_user.value);
    if (user_gacha_accumulate_iter == user_gacha_accumulate_table.end())
    {
        user_gacha_accumulate_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user.value;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_accumulate_table.modify(user_gacha_accumulate_iter, owner, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
        update_log.gacha_num++;
        update_log.monster_num++;
    });
}

void unlimited_tower::gacha_item_id(eosio::name _user, uint64_t _seed)
{
    uint8_t random_item_id = safeseed::get_random_value(_seed, db_controller.item_id_count, default_min, item_random_count);
    const auto &item_id_db_iter = item_id_db_table.get(random_item_id, "not exist item id");

    item_random_count += 1;
    uint8_t random_item_tier = safeseed::get_random_value(_seed, db_controller.item_tier_count, default_min, item_random_count);
    const auto &item_tier_db_iter = item_tier_db_table.get(random_item_tier, "not exist tier info");

    item_random_count += 1;
    uint8_t random_item_grade = safeseed::get_random_value(_seed, db_controller.item_grade_count, default_min, item_random_count);
    const auto &item_grade_db_iter = item_grade_db_table.get(random_item_grade, "not exist tier info");

    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data");

    result_info result;
    user_items user_item_table(owner, _user.value);
    user_item_table.emplace(owner, [&](auto &update_user_item_list) {
        uint32_t first_index = user_item_table.available_primary_key();
        if (first_index == 0)
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
        item_random_count += 1;
        new_item.status.basic_str = safeseed::get_random_value(_seed, item_grade_db_iter.max_range.base_str, item_grade_db_iter.min_range.base_str, item_random_count);
        item_random_count += 1;
        new_item.status.basic_dex = safeseed::get_random_value(_seed, item_grade_db_iter.max_range.base_dex, item_grade_db_iter.min_range.base_dex, item_random_count);
        item_random_count += 1;
        new_item.status.basic_int = safeseed::get_random_value(_seed, item_grade_db_iter.max_range.base_int, item_grade_db_iter.min_range.base_int, item_random_count);
        new_item.job = item_id_db_iter.job;
        new_item.state = eobject_state::on_inventory;
        new_item.grade = item_grade_db_iter.grade;

        result.index = update_user_item_list.index;
        result.type = result::item;

        update_user_item_list.item = new_item;
    });

    auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
    if (user_gacha_result_iter == user_gacha_result_table.end())
    {
        user_gacha_result_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user.value;
            new_result.result = result;
        });
    }
    else
    {
        user_gacha_result_table.modify(user_gacha_result_iter, owner, [&](auto &new_result) {
            new_result.result = result;
        });
    }

    auto user_gacha_accumulate_iter = user_gacha_accumulate_table.find(_user.value);
    if (user_gacha_accumulate_iter == user_gacha_accumulate_table.end())
    {
        user_gacha_accumulate_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user.value;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_accumulate_table.modify(user_gacha_accumulate_iter, owner, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
        update_log.item_num++;
        update_log.gacha_num++;
    });
}

void unlimited_tower::start_gacha(eosio::name _user)
{
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "unknown account");

    uint64_t l_seed = safeseed::get_seed(_user.value);

    if (user_log_iter->gacha_num == 0)
    {
        gacha_monster_id(_user, l_seed);
    }
    else
    {
        uint64_t l_gacha_result_type = safeseed::get_random_value(l_seed, max_rate, default_min, DEFAULE_RANDOM_COUNT);
        if (l_gacha_result_type < 33333)
        {
            gacha_servant_job(_user, l_seed);
        }
        else if (l_gacha_result_type > 33333 && l_gacha_result_type <= 66666)
        {
            gacha_monster_id(_user, l_seed);
        }
        else
        {
            gacha_item_id(_user, l_seed);
        }
    }
    servant_random_count = 0;
    monster_random_count = 0;
    item_random_count = 0;
}

//------------------------------------------------------------------------//
//-------------------------------Token_action-----------------------------//
//------------------------------------------------------------------------//

#pragma resion token action
ACTION unlimited_tower::create(name issuer, asset maximum_supply)
{
    require_auth(owner);

    auto sym = maximum_supply.symbol;
    eosio_assert(sym.is_valid(), "Invalid symbol name");
    eosio_assert(maximum_supply.is_valid(), "Invalid Supply");
    eosio_assert(maximum_supply.amount > 0, "Max-supply must be positive");

    stat statstable(owner, sym.code().raw());
    auto existing = statstable.find(sym.code().raw());
    eosio_assert(existing == statstable.end(), "Tokenwith symbol already exists");

    statstable.emplace(owner, [&](auto &s) {
        s.supply.symbol = maximum_supply.symbol;
        s.max_supply = maximum_supply;
        s.issuer = issuer;
    });
}

ACTION unlimited_tower::issue(name to, asset quantity, string memo)
{
    auto sym = quantity.symbol;
    eosio_assert(sym.is_valid(), "Invalid symbol name");
    eosio_assert(memo.size() <= 256, "Memo has more than 256 bytes");

    uint64_t sym_name = sym.code().raw();
    stat statstable(owner, sym_name);
    auto existing = statstable.find(sym.code().raw());
    eosio_assert(existing != statstable.end(), "Token with symbol does now exist, Create token before issue");
    const auto &st = *existing;

    require_auth(st.issuer);
    eosio_assert(quantity.is_valid(), "Invalid quantity");
    eosio_assert(quantity.amount > 0, "Must issue positive quantity");

    eosio_assert(quantity.symbol == st.supply.symbol, "Symbol precision mismatch");
    eosio_assert(quantity.amount <= st.max_supply.amount - st.supply.amount, "Quantity exceeds available supply");

    statstable.modify(st, owner, [&](auto &s) {
        s.supply += quantity;
    });

    add_balance(st.issuer, quantity, st.issuer);

    if (to != st.issuer)
    {
        action(permission_level{st.issuer, "active"_n},
               st.issuer, "tokentrans"_n,
               std::make_tuple(st.issuer, to, quantity, memo))
            .send();
    }
}

ACTION unlimited_tower::transfer(name from, name to, asset quantity, string memo)
{
    eosio_assert(from != to, "Cannot transfer to self");
    require_auth(from);
    eosio_assert(is_account(to), "To account does not exist");
    auto sym = quantity.symbol.code().raw();
    stat statstable(owner, sym);
    const auto &st = statstable.get(sym, "Not exist symbol");

    require_recipient(from);
    require_recipient(to);

    eosio_assert(quantity.is_valid(), "Invalid quantity");
    eosio_assert(quantity.amount > 0, "Must transfer positive quantity");
    eosio_assert(quantity.symbol == st.supply.symbol, "Symbol precision mismatch");
    eosio_assert(memo.size() <= 250, "Memo has more than 256 bytes");

    sub_balance(from, quantity);
    add_balance(to, quantity, from);
}

inline asset get_supply(symbol_name sym) const;
inline asset get_balance(account_name owner, symbol_name sym) const;

ACTION unlimited_tower::sub_balance(name owner, asset value)
{
    accounts from_acnts(owner, owner.value);

    const auto &from = from_acnts.get(value.symbol.code().raw(), "No balance object found");
    eosio_assert(from.balance.amount >= value.amount, "Overdrawn balance");

    if (from.balance.amount == value.amount)
    {
        from_acnts.erase(from);
    }
    else
    {
        from_acnts.modify(from, owner, [&](auto &a) {
            a.balance -= value;
        });
    }
}

ACTION unlimited_tower::add_balance(name owner, asset value, name ram_payer)
{
    accounts to_acnts(owner, owner.value);
    auto to = to_acnts.find(value.symbol.code().raw());
    if (to == to_acnts.end())
    {
        to_acnts.emplace(ram_payer, [&](auto &a) {
            a.balance = value;
        });
    }
    else
    {
        to_acnts.modify(to, ram_payer, [&](auto &a) {
            a.balance += value;
        });
    }
}

void unlimited_tower::delete_user_balance(name _user)
{
    token_accounts user_balance_table(owner, _user.value);
    for (auto user_balance_iter = user_balance_table.begin(); user_balance_iter != user_balance_table.end();)
    {
        auto iter = user_balance_table.find(user_balance_iter->primary_key());
        user_balance_iter++;
        user_balance_table.erase(iter);
    }
}

void unlimited_tower::init_stat(asset _token)
{
    require_auth(owner);
    token_stats statstable(owner, _token.symbol.code().raw());
    for (auto token_stat_iter = statstable.begin(); token_stat_iter != statstable.end();)
    {
        auto iter = statstable.find(token_stat_iter->primary_key());
        token_stat_iter++;
        statstable.erase(iter);
    }
}

void unlimited_tower::init_all_balance()
{
    require_auth(owner);
    auto &user_auth_table = login_controller.get_auth_user_table();
    for (auto user_name_iter = user_auth_table.begin(); user_name_iter != user_auth_table.end();)
    {
        delete_user_balance(user_name_iter->primary_key());
        user_name_iter++;
    }

    token_accounts user_balance_table(owner, owner.value);
    for (auto user_balance_iter = user_balance_table.begin(); user_balance_iter != user_balance_table.end();)
    {
        auto iter = user_balance_table.find(user_balance_iter->primary_key());
        user_balance_iter++;
        user_balance_table.erase(iter);
    }
}

#pragma endresion

#undef EOSIO_ABI

#define EOSIO_ABI(TYPE, MEMBERS)                                                                                     \
    extern "C"                                                                                                       \
    {                                                                                                                \
        void apply(uint64_t receiver, uint64_t code, uint64_t action)                                                \
        {                                                                                                            \
            auto self = receiver;                                                                                    \
            TYPE thiscontract(self);                                                                                 \
            if (action == N(onerror))                                                                                \
            {                                                                                                        \
                eosio_assert(code == N(eosio), "onerror action's are only valid from the \"eosio\" system account"); \
            }                                                                                                        \
            if (code == self)                                                                                        \
            {                                                                                                        \
                if (action != N(transfer))                                                                           \
                {                                                                                                    \
                    switch (action)                                                                                  \
                    {                                                                                                \
                        EOSIO_API(TYPE, MEMBERS)                                                                     \
                    }                                                                                                \
                }                                                                                                    \
            }                                                                                                        \
            else if (code == N(eosio.token) && action == N(transfer))                                                \
            {                                                                                                        \
                execute_action(&thiscontract, &cmain_logic::eostransfer);                                            \
            }                                                                                                        \
        }                                                                                                            \
    }
// eos 금액에 대해 체크 하는 함

EOSIO_DISPATCH(unlimited_tower, (create)(issue)(tokentrans)(setdata)(signup)(eostransfer)(initdata)(deleteuser)(initalluser)(inittoken))
