#include "Common/common_header.hpp"
#include "Common/common_seed.hpp"
#include "unlimitgacha.hpp"

//------------------------------------------------------------------------//
//----------------------------unlimited_tower_action----------------------//
//------------------------------------------------------------------------//
#pragma region Token action

ACTION unlimitgacha::create(eosio::name _issuer, asset _maximum_supply)
{
    require_auth(owner);

    auto sym = _maximum_supply.symbol;
    eosio_assert(sym.is_valid(), "Invalid symbol name");
    eosio_assert(_maximum_supply.is_valid(), "Invalid Supply");

    eosio_assert(_maximum_supply.amount > 0, "Max-supply must be positive");

    stat statstable(owner, sym.code().raw());
    auto existing = statstable.find(sym.code().raw());
    eosio_assert(existing == statstable.end(), "Tokenwith symbol already exists");

    statstable.emplace(owner, [&](auto &s) {
        s.supply.symbol = _maximum_supply.symbol;
        s.max_supply = _maximum_supply;
        s.issuer = _issuer;
    });
}

ACTION unlimitgacha::issue(eosio::name _to, asset _quantity, string _memo)
{
    auto sym = _quantity.symbol;
    eosio_assert(sym.is_valid(), "Invalid symbol name");
    eosio_assert(_memo.size() <= 256, "Memo has more than 256 bytes");

    uint64_t sym_name = sym.code().raw();
    stat statstable(owner, sym_name);
    auto existing = statstable.find(sym.code().raw());
    eosio_assert(existing != statstable.end(), "Token with symbol does now exist, Create token before issue");
    const auto &st = *existing;

    require_auth(st.issuer);
    eosio_assert(_quantity.is_valid(), "Invalid quantity");
    eosio_assert(_quantity.amount > 0, "Must issue positive quantity");

    eosio_assert(_quantity.symbol == st.supply.symbol, "Symbol precision mismatch");
    eosio_assert(_quantity.amount <= st.max_supply.amount - st.supply.amount, "Quantity exceeds available supply");

    statstable.modify(st, owner, [&](auto &s) {
        s.supply += _quantity;
    });

    add_balance(st.issuer, _quantity, st.issuer);

    if (_to != st.issuer)
    {
        action(permission_level{st.issuer, "active"_n},
               st.issuer, "tokentrans"_n,
               std::make_tuple(st.issuer, _to, _quantity, _memo))
            .send();
    }
}

ACTION unlimitgacha::tokentrans(name _from, name _to, asset _quantity, string _memo)
{
    eosio_assert(_from != _to, "Cannot transfer to self");
    require_auth(_from);
    eosio_assert(is_account(_to), "To account does not exist");
    auto sym = _quantity.symbol.code().raw();
    stat statstable(owner, sym);
    const auto &st = statstable.get(sym, "Not exist symbol");

    require_recipient(_from);
    require_recipient(_to);

    eosio_assert(_quantity.is_valid(), "Invalid quantity");
    eosio_assert(_quantity.amount > 0, "Must transfer positive quantity");
    eosio_assert(_quantity.symbol == st.supply.symbol, "Symbol precision mismatch");
    eosio_assert(_memo.size() <= 250, "Memo has more than 256 bytes");

    sub_balance(_from, _quantity);
    add_balance(_to, _quantity, _from);
}

void unlimitgacha::sub_balance(name _user, asset _value)
{
    accounts from_acnts(owner, _user.value);

    const auto &from = from_acnts.get(_value.symbol.code().raw(), "No balance object found");
    eosio_assert(from.balance.amount >= _value.amount, "Overdrawn balance");

    if (from.balance.amount == _value.amount)
    {
        from_acnts.erase(from);
    }
    else
    {
        from_acnts.modify(from, owner, [&](auto &a) {
            a.balance -= _value;
        });
    }
}

void unlimitgacha::add_balance(name _user, asset _value, name _ram_payer)
{
    accounts to_acnts(owner, _user.value);
    auto to = to_acnts.find(_value.symbol.code().raw());
    if (to == to_acnts.end())
    {
        to_acnts.emplace(_ram_payer, [&](auto &a) {
            a.balance = _value;
        });
    }
    else
    {
        to_acnts.modify(to, _ram_payer, [&](auto &a) {
            a.balance += _value;
        });
    }
}

#pragma endregion

#pragma region set
ACTION unlimitgacha::setdata()
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

    uint64_t l_seed = safeseed::get_seed(owner.value, now());
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

ACTION unlimitgacha::setfreesale()
{
    eosio::require_auth(owner);
    participation_logs participation_log_table(owner, owner.value);
    participation_log_table.emplace(owner, [&](auto &free_sale) {
        free_sale.owner = owner;
    });
}

#pragma endregion

#pragma region login

ACTION unlimitgacha::freesalesign(eosio::name _user)
{
    require_auth(_user);
    auth_users auth_user_table(owner, owner.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "exist account");
    auth_user_table.emplace(owner, [&](auto &new_user) {
        new_user.user = _user;
        new_user.state = euser_state::lobby;

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

    uint64_t seed = safeseed::get_seed(owner.value, _user.value);

    monster_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(seed, max_rate, default_min, monster_random_count);
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

    user_free_sale_monsters user_monster_table(owner, _user.value);
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
        new_monster.look = 1;
        new_monster.grade = monster_grade_db_iter.monster_grade;
        monster_random_count += 1;
        new_monster.status.basic_str = safeseed::get_random_value(seed, monster_grade_db_iter.max_range.base_str, monster_grade_db_iter.min_range.base_str, monster_random_count);
        monster_random_count += 1;
        new_monster.status.basic_dex = safeseed::get_random_value(seed, monster_grade_db_iter.max_range.base_dex, monster_grade_db_iter.min_range.base_dex, monster_random_count);
        monster_random_count += 1;
        new_monster.status.basic_int = safeseed::get_random_value(seed, monster_grade_db_iter.max_range.base_int, monster_grade_db_iter.min_range.base_int, monster_random_count);
        new_monster.state = eobject_state::on_inventory;

        update_user_monster_list.monster = new_monster;
    });

    participation_logs participation_log_table(owner, owner.value);
    auto gacha_participation_iter = participation_log_table.find(owner.value);
    eosio_assert(gacha_participation_iter != participation_log_table.end(), "free sale time over");

    asset freesale_signup_reward(0, symbol(symbol_code("UTG"), 4));
    freesale_signup_reward.amount = 30000000; // 3000 UTG

    uint64_t limt_check = gacha_participation_iter->accumulate_token_amount + freesale_signup_reward.amount;
    if( limt_check <= limit_token_amount )
    {
        participation_log_table.modify(gacha_participation_iter, owner, [&](auto &update_participation_list) {
            update_participation_list.gacha_participation += 1;
            update_participation_list.accumulate_token_amount += freesale_signup_reward.amount;
        });
        action(permission_level{get_self(), "active"_n},
               get_self(), "tokentrans"_n,
               std::make_tuple(owner, _user, freesale_signup_reward, std::string("freesale signup reward")))
            .send();
    }
}

ACTION unlimitgacha::signup(eosio::name _user)
{
    require_auth(_user);
    participation_logs participation_log_table(owner, owner.value);
    auto gacha_participation_iter = participation_log_table.find(owner.value);
    eosio_assert(gacha_participation_iter == participation_log_table.end(), "free sale time please freesalesign");

    auth_users auth_user_table(owner, owner.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    if (new_user_iter == auth_user_table.end())
    {
        auth_user_table.emplace(owner, [&](auto &new_user) {
            new_user.user = _user;
            new_user.state = euser_state::lobby;

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
    else
    {
        user_servants user_servant_table(owner, _user.value);
        user_free_sale_servants user_servant_free_table(owner, _user.value);
        for (auto user_servant_iter = user_servant_free_table.begin(); user_servant_iter != user_servant_free_table.end();)
        {
            user_servant_table.emplace(owner, [&](auto &move_servant) {
                move_servant.index = user_servant_iter->index;
                move_servant.party_number = user_servant_iter->party_number;
                move_servant.servant = user_servant_iter->servant;
            });
            user_servant_iter++;
        }

        user_monsters user_monster_table(owner, _user.value);
        user_free_sale_monsters user_monster_free_table(owner, _user.value);
        for (auto user_monster_iter = user_monster_free_table.begin(); user_monster_iter != user_monster_free_table.end();)
        {
            user_monster_table.emplace(owner, [&](auto &move_monster) {
                move_monster.index = user_monster_iter->index;
                move_monster.party_number = user_monster_iter->party_number;
                move_monster.monster = user_monster_iter->monster;
            });
            user_monster_iter++;
        }

        user_items user_item_table(owner, _user.value);
        user_free_sale_items user_item_free_table(owner, _user.value);
        for (auto user_item_iter = user_item_free_table.begin(); user_item_iter != user_item_free_table.end();)
        {
            user_item_table.emplace(owner, [&](auto &move_item) {
                move_item.index = user_item_iter->index;
                move_item.item = user_item_iter->item;
            });
            user_item_iter++;
        }
    }
}
// eosio.token recipient
// memo description spec
//-------------------------------------------------------------------------
ACTION unlimitgacha::eostransfer(eosio::name sender, eosio::name receiver)
{
    eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
        if (ad.action.size() == 0)
        {
            print("action size zero\n");
        }
        else if (ad.action == action_gacha)
        {
            participation_logs participation_log_table(owner, owner.value);
            auto gacha_participation_iter = participation_log_table.find(owner.value);
            if ( gacha_participation_iter == participation_log_table.end() ) 
            {
                start_gacha(sender, ad.type);
            }
            else
            {
                freesale_gacha(sender, ad.type);

                asset gacha_reward(0, symbol(symbol_code("UTG"), 4));
                if (gacha_participation_iter->gacha_participation < 3)
                {
                    gacha_reward.amount = 30000000;
                }
                else if (gacha_participation_iter->gacha_participation < 5)
                {
                    gacha_reward.amount = 20000000;
                }
                else
                {
                    gacha_reward.amount = 10000000;
                }

                uint64_t limt_check = gacha_participation_iter->accumulate_token_amount + gacha_reward.amount;
                if (limt_check <= limit_token_amount)
                {
                    participation_log_table.modify(gacha_participation_iter, owner, [&](auto &update_participation_list) {
                        update_participation_list.gacha_participation += 1;
                        update_participation_list.accumulate_token_amount += gacha_reward.amount;
                    });
                    action(permission_level{get_self(), "active"_n},
                           get_self(), "tokentrans"_n,
                           std::make_tuple(owner, sender, gacha_reward, std::string("free sale gacha reward")))
                        .send();
                }
            }
        }
    });
}

template <typename T>
void unlimitgacha::eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func)
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

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(sender.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data");
    user_log_table.modify(user_log_iter, owner, [&](auto &buy_log) {
        buy_log.use_eos += transfer_data.quantity.amount;
    });
    func(res);
}

#pragma endregion

#pragma resion init db table
ACTION unlimitgacha::initdata()
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

ACTION unlimitgacha::deleteuser(eosio::name _user)
{
    delete_user_data(_user);
    delete_user_object_data(_user);
    delete_user_freesale_data(_user);
    delete_user_gacha_result_data(_user);
    delete_user_balance(_user);
}

void unlimitgacha::delete_user_data(eosio::name _user)
{
    require_auth(owner);
    auth_users auth_user_table(owner, owner.value);
    auto user_auth_iter = auth_user_table.find(_user.value);
    eosio_assert(user_auth_iter != auth_user_table.end(), "not exist user auth data");
    auth_user_table.erase(user_auth_iter);

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user auth data");
    user_log_table.erase(user_log_iter);
}

void unlimitgacha::delete_user_object_data(eosio::name _user)
{
    require_auth(owner);
    user_servants user_servant_table(owner, _user.value);
    for (auto user_servant_iter = user_servant_table.begin(); user_servant_iter != user_servant_table.end();)
    {
        auto iter = user_servant_table.find(user_servant_iter->primary_key());
        user_servant_iter++;
        user_servant_table.erase(iter);
    }

    user_monsters user_monster_table(owner, _user.value);
    for (auto user_monster_iter = user_monster_table.begin(); user_monster_iter != user_monster_table.end();)
    {
        auto iter = user_monster_table.find(user_monster_iter->primary_key());
        user_monster_iter++;
        user_monster_table.erase(iter);
    }

    user_items user_item_table(owner, _user.value);
    for (auto user_item_iter = user_item_table.begin(); user_item_iter != user_item_table.end();)
    {
        auto iter = user_item_table.find(user_item_iter->primary_key());
        user_item_iter++;
        user_item_table.erase(iter);
    }
}

void unlimitgacha::delete_user_freesale_data(eosio::name _user)
{
    require_auth(owner);
    user_free_sale_servants user_servant_table(owner, _user.value);
    for (auto user_servant_iter = user_servant_table.begin(); user_servant_iter != user_servant_table.end();)
    {
        auto iter = user_servant_table.find(user_servant_iter->primary_key());
        user_servant_iter++;
        user_servant_table.erase(iter);
    }

    user_free_sale_monsters user_monster_table(owner, _user.value);
    for (auto user_monster_iter = user_monster_table.begin(); user_monster_iter != user_monster_table.end();)
    {
        auto iter = user_monster_table.find(user_monster_iter->primary_key());
        user_monster_iter++;
        user_monster_table.erase(iter);
    }

    user_free_sale_items user_item_table(owner, _user.value);
    for (auto user_item_iter = user_item_table.begin(); user_item_iter != user_item_table.end();)
    {
        auto iter = user_item_table.find(user_item_iter->primary_key());
        user_item_iter++;
        user_item_table.erase(iter);
    }
}

void unlimitgacha::delete_user_gacha_result_data(eosio::name _user)
{
    require_auth(owner);
    user_gacha_results user_gacha_current_result_table(owner, owner.value);
    user_gacha_accumulates user_gacha_accumulate_table(owner, owner.value);

    auto iter = user_gacha_current_result_table.find(_user.value);
    eosio_assert(iter != user_gacha_current_result_table.end(), "not exist gacha result data");
    user_gacha_current_result_table.erase(iter);

    auto accumulate_iter = user_gacha_accumulate_table.find(_user.value);
    eosio_assert(accumulate_iter != user_gacha_accumulate_table.end(), "not exist gacha accumulate data");
    user_gacha_accumulate_table.erase(accumulate_iter);
}
#pragma endregion

#pragma resion init all table

ACTION unlimitgacha::initalluser()
{
    init_all_object_gacha_data();
    init_all_user_auth_data();
    init_all_user_log_data();
}

void unlimitgacha::init_all_user_auth_data()
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

void unlimitgacha::init_all_user_log_data()
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

void unlimitgacha::init_all_object_gacha_data()
{
    require_auth(owner);
    auth_users user_auth_table(owner, owner.value);
    for (auto user_name_iter = user_auth_table.begin(); user_name_iter != user_auth_table.end();)
    {
        delete_user_object_data(user_name_iter->user);
        delete_user_freesale_data(user_name_iter->user);
        delete_user_balance(user_name_iter->user);
        delete_user_gacha_result_data(user_name_iter->user);
        user_name_iter++;
    }
}

ACTION unlimitgacha::initfreelog()
{
    init_freesale_log();
}

void unlimitgacha::init_freesale_log()
{
    require_auth(owner);
    participation_logs free_sale_log_table(owner, owner.value);

    auto iter = free_sale_log_table.find(owner.value);
    eosio_assert(iter != free_sale_log_table.end(), "not exist freesale log data");
    free_sale_log_table.erase(iter);
}

#pragma endregion

#pragma resion init token

ACTION unlimitgacha::inittoken(asset _token)
{
    init_all_balance();
    init_stat(_token);
}

void unlimitgacha::delete_user_balance(eosio::name _user)
{
    accounts user_balance_table(owner, _user.value);
    for (auto user_balance_iter = user_balance_table.begin(); user_balance_iter != user_balance_table.end();)
    {
        auto iter = user_balance_table.find(user_balance_iter->primary_key());
        user_balance_iter++;
        user_balance_table.erase(iter);
    }
}

void unlimitgacha::init_stat(asset _token)
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

void unlimitgacha::init_all_balance()
{
    require_auth(owner);

    auth_users user_auth_table(owner, owner.value);
    for (auto user_name_iter = user_auth_table.begin(); user_name_iter != user_auth_table.end();)
    {
        delete_user_balance(user_name_iter->user);
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

void unlimitgacha::gacha_servant_job(eosio::name _user, uint64_t _seed)
{
    servant_db servant_db_table(owner, owner.value);
    uint8_t random_job = safeseed::get_random_value(_seed, servant_job_count, default_min, servant_random_count);
    const auto &servant_db_iter = servant_db_table.get(random_job, "not get servant job data");

    user_logs user_log_table(owner, owner.value);
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
        update_log.servant_num += 1;
        update_log.gacha_num += 1;
    });
}

uint8_t unlimitgacha::gacha_servant_head(uint64_t _seed, uint32_t _count)
{
    head_db head_db_table(owner, owner.value);
    uint8_t random_head = safeseed::get_random_value(_seed, head_count, default_min, _count);
    const auto &head_db_iter = head_db_table.get(random_head, "not exist head info");
    return head_db_iter.head;
}

uint8_t unlimitgacha::gacha_servant_hair(uint64_t _seed, uint32_t _count)
{
    hair_db hair_db_table(owner, owner.value);
    uint8_t random_hair = safeseed::get_random_value(_seed, hair_count, default_min, _count);
    const auto &hair_db_iter = hair_db_table.get(random_hair, "not exist hair info");
    return hair_db_iter.hair;
}

uint8_t unlimitgacha::gacha_servant_body(uint64_t _seed, uint32_t _count)
{
    body_db body_db_table(owner, owner.value);
    uint8_t random_body = safeseed::get_random_value(_seed, body_count, default_min, _count);
    const auto &body_db_iter = body_db_table.get(random_body, "not exist body info");
    return body_db_iter.body;
}

void unlimitgacha::gacha_monster_id(eosio::name _user, uint64_t _seed)
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
        new_monster.state = eobject_state::on_inventory;

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
        update_log.gacha_num += 1;
        update_log.monster_num += 1;
    });
}

void unlimitgacha::gacha_item_id(eosio::name _user, uint64_t _seed)
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
        new_item.state = eobject_state::on_inventory;
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
        update_log.item_num += 1;
        update_log.gacha_num += 1;
    });
}
uint64_t unlimitgacha::get_user_seed_value(uint64_t _user)
{
    user_logs user_log_table(owner, owner.value);
    const auto &user_log_iter = user_log_table.get(_user, "not exist log in login seed");
    uint64_t user = _user + user_log_iter.gacha_num;
    return user;
}

void unlimitgacha::start_gacha(eosio::name _user, uint64_t _seed)
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
    servant_random_count = 0;
    monster_random_count = 0;
    item_random_count = 0;
}

void unlimitgacha::freesale_gacha_servant_job(eosio::name _user, uint64_t _seed)
{
    servant_db servant_db_table(owner, owner.value);
    uint8_t random_job = safeseed::get_random_value(_seed, servant_job_count, default_min, servant_random_count);
    const auto &servant_db_iter = servant_db_table.get(random_job, "not get servant job data");

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data");

    result_info result;
    user_free_sale_servants user_servant_table(owner, _user.value);
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
        update_log.servant_num += 1;
        update_log.gacha_num += 1;
    });
}

void unlimitgacha::freesale_gacha_monster_id(eosio::name _user, uint64_t _seed)
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
    user_free_sale_monsters user_monster_table(owner, _user.value);
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
        update_log.gacha_num += 1;
        update_log.monster_num += 1;
    });
}

void unlimitgacha::freesale_gacha_item_id(eosio::name _user, uint64_t _seed)
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
    user_free_sale_items user_item_table(owner, _user.value);
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
        update_log.item_num += 1;
        update_log.gacha_num += 1;
    });
}

void unlimitgacha::freesale_gacha(eosio::name _user, uint64_t _seed)
{
    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "unknown account");

    uint64_t l_user = get_user_seed_value(_user.value);
    uint64_t l_seed = safeseed::get_seed_value(l_user, _seed);
    //가차 뽑기 참여 횟수

    if (user_log_iter->gacha_num == 0)
    {
        freesale_gacha_monster_id(_user, l_seed);
    }
    else
    {
        uint64_t l_gacha_result_type = safeseed::get_random_value(l_seed, max_rate, default_min, DEFAULE_RANDOM_COUNT);
        if (l_gacha_result_type < 33)
        {
            freesale_gacha_servant_job(_user, l_seed);
        }
        else if (l_gacha_result_type > 33 && l_gacha_result_type <= 66)
        {
            freesale_gacha_monster_id(_user, l_seed);
        }
        else
        {
            freesale_gacha_item_id(_user, l_seed);
        }
    }

    servant_random_count = 0;
    monster_random_count = 0;
    item_random_count = 0;
}

#pragma endresion

#undef EOSIO_DISPATCH

#define EOSIO_DISPATCH(TYPE, MEMBERS)                                                       \
    extern "C"                                                                              \
    {                                                                                       \
        void apply(uint64_t receiver, uint64_t code, uint64_t action)                       \
        {                                                                                   \
            if (code == receiver)                                                           \
            {                                                                               \
                switch (action)                                                             \
                {                                                                           \
                    EOSIO_DISPATCH_HELPER(TYPE, MEMBERS)                                    \
                }                                                                           \
            }                                                                               \
            else if (code == name("eosio.token").value && action == name("transfer").value) \
            {                                                                               \
                execute_action(name(receiver), name(code), &unlimitgacha::eostransfer);     \
            }                                                                               \
        }                                                                                   \
    }
// eos 금액에 대해 체크 하는 함

EOSIO_DISPATCH(unlimitgacha, (create)(issue)(tokentrans)(setdata)(setfreesale)(freesalesign)(signup)(eostransfer)(initdata)(deleteuser)(initalluser)(initfreelog)(inittoken))
