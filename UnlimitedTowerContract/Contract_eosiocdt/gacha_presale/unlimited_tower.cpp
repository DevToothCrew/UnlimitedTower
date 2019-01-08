#include "Common/common_header.hpp"
#include "Common/common_seed.hpp"
#include "unlimited_tower.hpp"

//------------------------------------------------------------------------//
//----------------------------unlimited_tower_action----------------------//
//------------------------------------------------------------------------//
#pragma region Token action

ACTION unlimited_tower::create(eosio::name issuer, asset maximum_supply)
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

ACTION unlimited_tower::issue(eosio::name to, asset quantity, string memo)
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

ACTION unlimited_tower::tokentrans(name from, name to, asset quantity, string memo)
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

void unlimited_tower::sub_balance(name owner, asset value)
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

void unlimited_tower::add_balance(name owner, asset value, name ram_payer)
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

#pragma endregion

#pragma region set
ACTION unlimited_tower::setdata()
{
    servant_db servant_db_table(owner, owner.value);
    head_db head_db_table(owner, owner.value);
    hair_db hair_db_table(owner, owner.value);
    body_db body_db_table(owner, owner.value);
    monster_grade_db monster_grade_db_table(owner, owner.value);
    monster_id_db monster_id_db_table(owner, owner.value);
    item_id_db item_id_db_table(owner, owner.value);
    item_tier_db item_tier_db_table(owner, owner.value);
    item_grade_db item_grade_db_table(owner, owner.value);

    eosio::require_auth(owner);
    uint64_t l_seed = safeseed::get_seed(owner.value,now());
    for (uint8_t i = 0; i < servant_job_count; ++i)
    {
        servant_db_table.emplace(owner, [&](auto &a) {
            a.job = servant_db_table.available_primary_key();
            if (random_count >= 8)
            {
                random_count = 0;
            }
            a.min_range.base_str = DEFAULT_MIN;
            a.min_range.base_dex = DEFAULT_MIN;
            a.min_range.base_int = DEFAULT_MIN;

            a.max_range.base_str = DEFAULT_MAX;
            a.max_range.base_dex = DEFAULT_MAX;
            a.max_range.base_int = DEFAULT_MAX;
        });
    }
    for (uint8_t i = 0; i < head_count; ++i)
    {
        head_db_table.emplace(owner, [&](auto &a) {
            a.head = head_db_table.available_primary_key();
        });
        hair_db_table.emplace(owner, [&](auto &a) {
            a.hair = hair_db_table.available_primary_key();
        });
    }
    for (uint8_t i = 0; i < body_count; ++i)
    {
        body_db_table.emplace(owner, [&](auto &a) {
            a.body = body_db_table.available_primary_key();
        });
    }

    for (uint8_t i = 0; i < monster_grade_count; ++i)
    {
        monster_grade_db_table.emplace(owner, [&](auto &a) {
            a.monster_grade = monster_grade_db_table.available_primary_key();
            if (random_count >= 8)
            {
                random_count = 0;
            }
            if (i == 0)
            {
                a.min_range.base_str = DEFAULT_MIN;
                a.min_range.base_dex = DEFAULT_MIN;
                a.min_range.base_int = DEFAULT_MIN;

                a.max_range.base_str = DEFAULT_MAX;
                a.max_range.base_dex = DEFAULT_MAX;
                a.max_range.base_int = DEFAULT_MAX;
            }
            else if (i == 1)
            {
                a.min_range.base_str = DEFAULT_MIN;
                a.min_range.base_dex = DEFAULT_MIN;
                a.min_range.base_int = DEFAULT_MIN;

                a.max_range.base_str = DEFAULT_MAX;
                a.max_range.base_dex = DEFAULT_MAX;
                a.max_range.base_int = DEFAULT_MAX;
            }
            else if (i == 2)
            {
                a.min_range.base_str = DEFAULT_MIN;
                a.min_range.base_dex = DEFAULT_MIN;
                a.min_range.base_int = DEFAULT_MIN;

                a.max_range.base_str = DEFAULT_MAX;
                a.max_range.base_dex = DEFAULT_MAX;
                a.max_range.base_int = DEFAULT_MAX;
            }
            else if (i == 3)
            {
                a.min_range.base_str = DEFAULT_MIN;
                a.min_range.base_dex = DEFAULT_MIN;
                a.min_range.base_int = DEFAULT_MIN;

                a.max_range.base_str = DEFAULT_MAX;
                a.max_range.base_dex = DEFAULT_MAX;
                a.max_range.base_int = DEFAULT_MAX;
            }
            else
            {
                a.min_range.base_str = 0;
                a.min_range.base_dex = 0;
                a.min_range.base_int = 0;

                a.max_range.base_str = DEFAULT_MAX;
                a.max_range.base_dex = DEFAULT_MAX;
                a.max_range.base_int = DEFAULT_MAX;
            }
        });
    }
    for (uint8_t i = 0; i < monster_id_count; ++i)
    {
        monster_id_db_table.emplace(owner, [&](auto &a) {
            a.look = monster_id_db_table.available_primary_key();
        });
    }
    for (uint8_t i = 0; i < item_id_count; ++i)
    {
        item_id_db_table.emplace(owner, [&](auto &a) {
            a.id = item_id_db_table.available_primary_key();
            if (random_count >= 8)
            {
                random_count = 0;
            }
            a.slot = safeseed::get_random_value(l_seed, item_slot_count, 0, random_count);
            random_count += 1;
            a.job = safeseed::get_random_value(l_seed, servant_job_count, 0, random_count);
            random_count += 1;
        });
    }
    for (uint32_t i = 1; i <= item_tier_count; ++i)
    {
        item_tier_db_table.emplace(owner, [&](auto &a) {
            a.tier = item_tier_db_table.available_primary_key();
            a.level = i * 10;
        });
    }
    for (uint32_t i = 0; i < item_grade_count; ++i)
    {
        item_grade_db_table.emplace(owner, [&](auto &a) {
            a.grade = item_grade_db_table.available_primary_key();
            if (random_count >= 8)
            {
                random_count = 0;
            }
            a.min_range.base_str = DEFAULT_MIN;
            a.min_range.base_dex = DEFAULT_MIN;
            a.min_range.base_int = DEFAULT_MIN;

            a.max_range.base_str = DEFAULT_MAX;
            a.max_range.base_dex = DEFAULT_MAX;
            a.max_range.base_int = DEFAULT_MAX;
        });
    }
}
#pragma endregion

#pragma region login

ACTION unlimited_tower::freesalesign(eosio::name _user)
{
    require_auth(_user);
    auth_users auth_user_table(owner, owner.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "exist account");
    auth_user_table.emplace(owner, [&](auto &new_user) {
        new_user.user = _user;
        new_user.state = euser_state::freesale;

        hero_info first_hero;
        first_hero.equip_slot.resize(max_equip_slot);
        first_hero.state = hero_state::set_look;

        new_user.hero = first_hero;
    });

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter == user_log_table.end(), "exist account");
    user_log_table.emplace(owner, [&](auto &new_log) {
        new_log.user = _user;
    });
}

ACTION unlimited_tower::signup(eosio::name _user)
{
    freesalesign(_user);
    // require_auth(_user);
    // auth_users auth_user_table(owner, owner.value);
    // auto new_user_iter = auth_user_table.find(_user.value);
    // eosio_assert(new_user_iter == auth_user_table.end(), "exist account");
    // auth_user_table.emplace(owner, [&](auto &new_user) {
    //     new_user.user = _user;
    //     new_user.state = euser_state::login;

    //     hero_info first_hero;
    //     first_hero.equip_slot.resize(max_equip_slot);
    //     first_hero.state = hero_state::set_look;

    //     new_user.hero = first_hero;
    // });

    // user_logs user_log_table(owner, owner.value);
    // auto user_log_iter = user_log_table.find(_user.value);
    // eosio_assert(user_log_iter == user_log_table.end(), "exist account");
    // user_log_table.emplace(owner, [&](auto &new_log) {
    //     new_log.user = _user;
    // });
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
            start_gacha(sender, ad.type);
        }
    });
}

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
        size_t l_next = transfer_data.memo.find(':', l_center + 1);
        size_t l_end = transfer_data.memo.length() - (l_next + 1);

        eosio_assert(transfer_data.memo.find(':') != std::string::npos, "seed memo [:] error");
        eosio_assert(transfer_data.memo.find(':', l_center + 1) != std::string::npos, "seed memo [:] error");
        eosio_assert(transfer_data.quantity.amount == 1, "gacha need 1.0000 EOS"); //test 100

        std::string l_seed = transfer_data.memo.substr(l_center + 1, (l_next - l_center - 1));
        std::string l_sha = transfer_data.memo.substr(l_next + 1, l_end);

        res.type = safeseed::check_seed(l_seed, l_sha);

        eosio_assert(res.type != 0, "wrong seed convert");
    }

    res.to.value = receiver.value;
    res.from.value = sender.value;

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(sender.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data");
    user_log_table.modify(user_log_iter, owner, [&](auto &buy_log) {
        buy_log.use_eos += transfer_data.quantity;
    });
    func(res);
}

#pragma endregion

#pragma resion init db table
ACTION unlimited_tower::initdata()
{
    eosio::require_auth(owner);

    servant_db servant_db_table(owner, owner.value);
    head_db head_db_table(owner, owner.value);
    hair_db hair_db_table(owner, owner.value);
    body_db body_db_table(owner, owner.value);
    monster_grade_db monster_grade_db_table(owner, owner.value);
    monster_id_db monster_id_db_table(owner, owner.value);
    item_id_db item_id_db_table(owner, owner.value);
    item_tier_db item_tier_db_table(owner, owner.value);
    item_grade_db item_grade_db_table(owner, owner.value);

    for (auto servant_db_table_iter = servant_db_table.begin(); servant_db_table_iter != servant_db_table.end();)
    {
        auto iter = servant_db_table.find(servant_db_table_iter->primary_key());
        servant_db_table_iter++;
        servant_db_table.erase(iter);
    }

    for (auto head_db_table_iter = head_db_table.begin(); head_db_table_iter != head_db_table.end();)
    {
        auto iter = head_db_table.find(head_db_table_iter->primary_key());
        head_db_table_iter++;
        head_db_table.erase(iter);
    }

    for (auto hair_db_table_iter = hair_db_table.begin(); hair_db_table_iter != hair_db_table.end();)
    {
        auto iter = hair_db_table.find(hair_db_table_iter->primary_key());
        hair_db_table_iter++;
        hair_db_table.erase(iter);
    }

    for (auto body_db_table_iter = body_db_table.begin(); body_db_table_iter != body_db_table.end();)
    {
        auto iter = body_db_table.find(body_db_table_iter->primary_key());
        body_db_table_iter++;
        body_db_table.erase(iter);
    }

    for (auto monster_grade_db_table_iter = monster_grade_db_table.begin(); monster_grade_db_table_iter != monster_grade_db_table.end();)
    {
        auto iter = monster_grade_db_table.find(monster_grade_db_table_iter->primary_key());
        monster_grade_db_table_iter++;
        monster_grade_db_table.erase(iter);
    }

    for (auto monster_id_db_table_iter = monster_id_db_table.begin(); monster_id_db_table_iter != monster_id_db_table.end();)
    {
        auto iter = monster_id_db_table.find(monster_id_db_table_iter->primary_key());
        monster_id_db_table_iter++;
        monster_id_db_table.erase(iter);
    }

    for (auto item_id_db_table_iter = item_id_db_table.begin(); item_id_db_table_iter != item_id_db_table.end();)
    {
        auto iter = item_id_db_table.find(item_id_db_table_iter->primary_key());
        item_id_db_table_iter++;
        item_id_db_table.erase(iter);
    }

    for (auto item_grade_db_table_iter = item_grade_db_table.begin(); item_grade_db_table_iter != item_grade_db_table.end();)
    {
        auto iter = item_grade_db_table.find(item_grade_db_table_iter->primary_key());
        item_grade_db_table_iter++;
        item_grade_db_table.erase(iter);
    }

    for (auto item_tier_db_table_iter = item_tier_db_table.begin(); item_tier_db_table_iter != item_tier_db_table.end();)
    {
        auto iter = item_tier_db_table.find(item_tier_db_table_iter->primary_key());
        item_tier_db_table_iter++;
        item_tier_db_table.erase(iter);
    }
}

#pragma endregion

#pragma resion delete user table

ACTION unlimited_tower::deleteuser(eosio::name _user)
{
    delete_user_data(_user.value);
    delete_user_object_data(_user.value);
    delete_user_gacha_result_data(_user.value);
}

void unlimited_tower::delete_user_data(uint64_t _user)
{
    require_auth(owner);
    auth_users auth_user_table(owner, owner.value);
    auto user_auth_iter = auth_user_table.find(_user);
    eosio_assert(user_auth_iter != auth_user_table.end(), "not exist user auth data");
    auth_user_table.erase(user_auth_iter);

    user_logs user_log_table(owner,owner.value);
    auto user_log_iter = user_log_table.find(_user);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user auth data");
    user_log_table.erase(user_log_iter);
}

void unlimited_tower::delete_user_object_data(uint64_t _user)
{
    require_auth(owner);
    user_servants user_servant_table(owner, _user);
    for (auto user_servant_iter = user_servant_table.begin(); user_servant_iter != user_servant_table.end();)
    {
        auto iter = user_servant_table.find(user_servant_iter->primary_key());
        user_servant_iter++;
        user_servant_table.erase(iter);
    }

    user_monsters user_monster_table(owner, _user);
    for (auto user_monster_iter = user_monster_table.begin(); user_monster_iter != user_monster_table.end();)
    {
        auto iter = user_monster_table.find(user_monster_iter->primary_key());
        user_monster_iter++;
        user_monster_table.erase(iter);
    }

    user_items user_item_table(owner, _user);
    for (auto user_item_iter = user_item_table.begin(); user_item_iter != user_item_table.end();)
    {
        auto iter = user_item_table.find(user_item_iter->primary_key());
        user_item_iter++;
        user_item_table.erase(iter);
    }
}
void unlimited_tower::delete_user_gacha_result_data(uint64_t _user)
{
    require_auth(owner);
    user_gacha_results user_gacha_current_result_table(owner, owner.value);
    user_gacha_accumulates user_gacha_accumulate_table(owner, owner.value);

    auto iter = user_gacha_current_result_table.find(_user);
    eosio_assert(iter != user_gacha_current_result_table.end(), "not exist gacha result data");
    user_gacha_current_result_table.erase(iter);

    auto accumulate_iter = user_gacha_accumulate_table.find(_user);
    eosio_assert(accumulate_iter != user_gacha_accumulate_table.end(), "not exist gacha accumulate data");
    user_gacha_accumulate_table.erase(accumulate_iter);
}
#pragma endregion



#pragma resion init all table

ACTION unlimited_tower::initalluser()
{
    init_all_user_auth_data();
    init_all_user_log_data();
    init_all_object_gacha_data();
}

void unlimited_tower::init_all_user_auth_data()
{
    require_auth(owner);
    auth_users auth_user_table(owner, owner.value);
    for (auto user_auth_iter = auth_user_table.begin(); user_auth_iter != auth_user_table.end();)
    {
        auto iter = auth_user_table.find(user_auth_iter->primary_key());
        user_auth_iter++;
        auth_user_table.erase(iter);
    }
}

void unlimited_tower::init_all_user_log_data()
{
    require_auth(owner);
    user_logs user_log_table(owner, owner.value);
    for (auto user_log_iter = user_log_table.begin(); user_log_iter != user_log_table.end();)
    {
        auto iter = user_log_table.find(user_log_iter->primary_key());
        user_log_iter++;
        user_log_table.erase(iter);
    }
}

void unlimited_tower::init_all_object_gacha_data()
{
    require_auth(owner);
    auth_users user_auth_table(owner, owner.value);
    for (auto user_name_iter = user_auth_table.begin(); user_name_iter != user_auth_table.end();)
    {
        delete_user_object_data(user_name_iter->primary_key());
        delete_user_gacha_result_data(user_name_iter->primary_key());
        user_name_iter++;
    }
}

#pragma endregion

#pragma resion init token

ACTION unlimited_tower::inittoken(asset _token)
{
    init_all_balance();
    init_stat(_token);
}

void unlimited_tower::delete_user_balance(uint64_t _user)
{
    accounts user_balance_table(owner, _user);
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
    stat statstable(owner, _token.symbol.code().raw());
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

    auth_users user_auth_table(owner, owner.value);
    for (auto user_name_iter = user_auth_table.begin(); user_name_iter != user_auth_table.end();)
    {
        delete_user_balance(user_name_iter->primary_key());
        user_name_iter++;
    }

    accounts user_balance_table(owner, owner.value);
    for (auto user_balance_iter = user_balance_table.begin(); user_balance_iter != user_balance_table.end();)
    {
        auto iter = user_balance_table.find(user_balance_iter->primary_key());
        user_balance_iter++;
        user_balance_table.erase(iter);
    }
}

#pragma endregion

//------------------------------------------------------------------------//
//-------------------------------login_function---------------------------//
//------------------------------------------------------------------------//



//------------------------------------------------------------------------//
//-------------------------------gacha_function---------------------------//
//------------------------------------------------------------------------//

void unlimited_tower::gacha_servant_job(eosio::name _user, uint64_t _seed)
{
    servant_db servant_db_table(owner, owner.value);
    uint8_t random_job = safeseed::get_random_value(_seed, servant_job_count, default_min, servant_random_count);
    const auto &servant_db_iter = servant_db_table.get(random_job, "not get servant job data");

    user_logs user_log_table(owner,_user.value);
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
        new_servant.state = eobject_state::on_freesale;

        result.index = update_user_servant_list.index;
        result.type = result::servant;

        update_user_servant_list.servant = new_servant;
    });

    user_gacha_results user_gacha_result_table(owner, owner.value);
    user_gacha_accumulates user_gacha_accumulate_table(owner, owner.value);

    auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
    if (user_gacha_result_iter == user_gacha_result_table.end())
    {
        user_gacha_result_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user;
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
            new_result.user = _user;
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
    head_db head_db_table(owner, owner.value);
    uint8_t random_head = safeseed::get_random_value(_seed, head_count, default_min, _count);
    const auto &head_db_iter = head_db_table.get(random_head, "not exist head info");
    return head_db_iter.head;
}

uint8_t unlimited_tower::gacha_servant_hair(uint64_t _seed, uint32_t _count)
{
    hair_db hair_db_table(owner, owner.value);
    uint8_t random_hair = safeseed::get_random_value(_seed, hair_count, default_min, _count);
    const auto &hair_db_iter = hair_db_table.get(random_hair, "not exist hair info");
    return hair_db_iter.hair;
}

uint8_t unlimited_tower::gacha_servant_body(uint64_t _seed, uint32_t _count)
{
    body_db body_db_table(owner, owner.value);
    uint8_t random_body = safeseed::get_random_value(_seed, body_count, default_min, _count);
    const auto &body_db_iter = body_db_table.get(random_body, "not exist body info");
    return body_db_iter.body;
}

void unlimited_tower::gacha_monster_id(eosio::name _user, uint64_t _seed)
{
    monster_id_db monster_id_db_table(owner, owner.value);
    uint8_t random_monster_id = safeseed::get_random_value(_seed, monster_id_count, default_min, monster_random_count);
    const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id, "not exist monster id");

    monster_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, monster_random_count);
    uint8_t random_grade;
    if (random_rate <= four_grade_ratio)
    {
        random_grade = 4;
    }
    else if (random_rate <= three_grade_ratio)
    {
        random_grade = 3;
    }
    else
    {
        random_grade = 2;
    }

    monster_grade_db monster_grade_db_table(owner, owner.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "not exist monster grade");

    user_logs user_log_table(owner, owner.value);
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
        new_monster.state = eobject_state::on_freesale;

        result.index = update_user_monster_list.index;
        result.type = result::monster;

        update_user_monster_list.monster = new_monster;
    });

    user_gacha_results user_gacha_result_table(owner, owner.value);
    user_gacha_accumulates user_gacha_accumulate_table(owner, owner.value);

    auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
    if (user_gacha_result_iter == user_gacha_result_table.end())
    {
        user_gacha_result_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user;
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
            new_result.user = _user;
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
    item_id_db item_id_db_table(owner, owner.value);
    uint8_t random_item_id = safeseed::get_random_value(_seed, item_id_count, default_min, item_random_count);
    const auto &item_id_db_iter = item_id_db_table.get(random_item_id, "not exist item id");

    item_tier_db item_tier_db_table(owner, owner.value);
    item_random_count += 1;
    uint8_t random_item_tier = safeseed::get_random_value(_seed, item_tier_count, default_min, item_random_count);
    const auto &item_tier_db_iter = item_tier_db_table.get(random_item_tier, "not exist tier info");

    item_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, item_random_count);
    uint8_t random_grade;
    if (random_rate <= four_grade_ratio)
    {
        random_grade = 4;
    }
    else if (random_rate <= three_grade_ratio)
    {
        random_grade = 3;
    }
    else
    {
        random_grade = 2;
    }

    item_grade_db item_grade_db_table(owner, owner.value);
    item_random_count += 1;
    uint8_t random_item_grade = safeseed::get_random_value(_seed, item_grade_count, default_min, item_random_count);
    const auto &item_grade_db_iter = item_grade_db_table.get(random_item_grade, "not exist tier info");

    user_logs user_log_table(owner, owner.value);
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
        new_item.state = eobject_state::on_freesale;
        new_item.grade = item_grade_db_iter.grade;

        result.index = update_user_item_list.index;
        result.type = result::item;

        update_user_item_list.item = new_item;
    });


    user_gacha_results user_gacha_result_table(owner, owner.value);
    user_gacha_accumulates user_gacha_accumulate_table(owner, owner.value);

    auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
    if (user_gacha_result_iter == user_gacha_result_table.end())
    {
        user_gacha_result_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user;
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
            new_result.user = _user;
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
uint64_t unlimited_tower::get_user_seed_value(uint64_t _user)
{
    user_logs user_log_table(owner, owner.value);
    const auto &user_log_iter = user_log_table.get(_user, "not exist log in login seed");
    uint64_t user = _user + user_log_iter.gacha_num;
    return user;
}

void unlimited_tower::start_gacha(eosio::name _user, uint64_t _seed)
{
    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "unknown account");

    uint64_t l_user = get_user_seed_value(_user.value);
    uint64_t l_seed = safeseed::get_seed_value(l_user, _seed);
       //가차 뽑기 참여 횟수 

    if (user_log_iter->gacha_num == 0)
    {
        gacha_monster_id(_user, l_seed);
    }
    else
    {
        uint64_t l_gacha_result_type = safeseed::get_random_value(l_seed, max_rate, default_min, DEFAULE_RANDOM_COUNT);
        if (l_gacha_result_type < 33)
        {
            gacha_servant_job(_user, l_seed);
        }
        else if (l_gacha_result_type > 33 && l_gacha_result_type <= 66)
        {
            gacha_monster_id(_user, l_seed);
        }
        else
        {
            gacha_item_id(_user, l_seed);
        }
    }
    asset gacha_reward(0,symbol(symbol_code("UTG"),4));
    //gacha_reward.amount = 10000000; //1000 UTG
    
    participation_logs participation_log_table(owner, owner.value);
    auto gacha_participation_iter = participation_log_table.find(owner.value);
    uint64_t temp_parti_count = 0;
    if(gacha_participation_iter == participation_log_table.end())
    {
        participation_log_table.emplace(owner, [&](auto &update_participation_list) {
            update_participation_list.owner = owner;
            update_participation_list.gacha_participation = 1;   
        
        });
        temp_parti_count = 1 ;
    }
    else
    {
        participation_log_table.modify(gacha_participation_iter, owner, [&](auto &update_participation_list) {
            temp_parti_count = update_participation_list.gacha_participation;
            update_participation_list.gacha_participation += 1;
        }); 
    }
    if(temp_parti_count <= 3){
        gacha_reward.amount = 30000000;
    }
    else if (temp_parti_count <= 5){
        gacha_reward.amount = 20000000;
    }
    else{
        gacha_reward.amount = 10000000;
    }
    action(permission_level{owner, "active"_n},
           owner, "tokentrans"_n,
           std::make_tuple(owner, _user, gacha_reward, std::string("gacha reward")))
        .send();

    servant_random_count = 0;
    monster_random_count = 0;
    item_random_count = 0;
}

#pragma endresion

#undef EOSIO_DISPATCH

#define EOSIO_DISPATCH(TYPE, MEMBERS)                                                                                                                                             \
    extern "C"                                                                                                                                                                    \
    {                                                                                                                                                                             \
        void apply(uint64_t receiver, uint64_t code, uint64_t action)                                                                                                             \
        {                                                                                                                                                                         \
            if (code == receiver)                                                                                                                                                 \
            {                                                                                                                                                                     \
                switch (action)                                                                                                                                                   \
                {                                                                                                                                                                 \
                    EOSIO_DISPATCH_HELPER(unlimited_tower, (create)(issue)(tokentrans)(setdata)(freesalesign)(signup)(eostransfer)(initdata)(deleteuser)(initalluser)(inittoken)) \
                }                                                                                                                                                                 \
            }                                                                                                                                                                     \
            else if (code == name("eosio.token").value && action == name("transfer").value)                                                                                       \
            {                                                                                                                                                                     \
                execute_action(name(receiver), name(receiver), &unlimited_tower::eostransfer);                                                                                        \
            }                                                                                                                                                                     \
        }                                                                                                                                                                         \
    }
// eos 금액에 대해 체크 하는 함

EOSIO_DISPATCH(unlimited_tower, (create)(issue)(tokentrans)(setdata)(freesalesign)(signup)(eostransfer)(initdata)(deleteuser)(initalluser)(inittoken))
