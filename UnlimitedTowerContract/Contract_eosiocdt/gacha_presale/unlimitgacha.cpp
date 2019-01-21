#include "Common/common_header.hpp"
#include "Common/common_seed.hpp"
#include "unlimitgacha.hpp"

//------------------------------------------------------------------------//
//----------------------------unlimited_tower_action----------------------//
//------------------------------------------------------------------------//

//------------------------------------------------------------------------//
//----------------------------Token_action--------------------------------//
//------------------------------------------------------------------------//
#pragma region Token action

ACTION unlimitgacha::create(eosio::name _issuer, asset _maximum_supply)
{
    require_auth(owner_auth);

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

    stat statstable(owner, sym.code().raw());
    auto existing = statstable.find(sym.code().raw());
    eosio_assert(existing != statstable.end(), "Token with symbol does now exist, Create token before issue");
    const auto &st = *existing;

    require_auth(st.issuer);
    eosio_assert(_quantity.is_valid(), "Invalid quantity");
    eosio_assert(_quantity.amount > 0, "Must issue positive quantity");

    eosio_assert(_quantity.symbol == st.supply.symbol, "Symbol precision mismatch");
    eosio_assert(_quantity.amount <= st.max_supply.amount - st.supply.amount, "Quantity exceeds available supply");

    statstable.modify(st, same_payer, [&](auto &s) {
        s.supply += _quantity;
    });

    add_balance(st.issuer, _quantity, st.issuer);

    if (_to != st.issuer)
    {
        action(permission_level{st.issuer, "active"_n},
               st.issuer, "transfer"_n,
               std::make_tuple(st.issuer, _to, _quantity, _memo))
            .send();
    }
}

ACTION unlimitgacha::transfer(name _from, name _to, asset _quantity, string _memo)
{
    black_list black_list_table(owner, owner.value);
    auto black_list_iter = black_list_table.find(_from.value);
    eosio_assert(black_list_iter == black_list_table.end(), "this user already exist in black list");

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
    eosio_assert(_memo.size() <= 256, "Memo has more than 256 bytes");

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
        to_acnts.modify(to, same_payer, [&](auto &a) {
            a.balance += _value;
        });
    }
}

#pragma endregion
//------------------------------------------------------------------------//
//----------------------------------db_action-----------------------------//
//------------------------------------------------------------------------//

#pragma region db_insert action

ACTION unlimitgacha::dbinsert(uint32_t _kind, uint32_t _appear, uint32_t _id, uint32_t _index, uint32_t _job, uint32_t _tier, uint32_t _type, uint32_t _grade, uint32_t _min, uint32_t _max)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(master_iter->master.value);
    eosio_assert(owner_iter->state == euser_state::pause, " not server checking");

    switch (_kind)
    {
    case db_choice::job:
    {
        insert_job(_job, _min, _max);
        break;
    }
    case db_choice::body:
    {
        insert_body(_appear);
        break;
    }
    case db_choice::hair:
    {
        insert_hair(_appear);
        break;
    }
    case db_choice::head:
    {
        insert_head(_appear);
        break;
    }
    case db_choice::gender:
    {
        insert_gender(_appear);
        break;
    }
    case db_choice::servant_id:
    {
        insert_servant_id(_id, _index);
        break;
    }
    case db_choice::monster_id:
    {
        insert_monster_id(_id);
        break;
    }
    case db_choice::monster_grade:
    {
        insert_monster_grade(_grade, _min, _max);
        break;
    }
    case db_choice::item_id:
    {
        insert_item_id(_id, _type, _job, _tier);
        break;
    }
    case db_choice::item_grade:
    {
        insert_item_grade(_grade, _min, _max);
        break;
    }
    }
}
void unlimitgacha::insert_job(uint32_t _job, uint32_t _min, uint32_t _max)
{
    servant_job_db servant_job_db_table(owner, owner.value);
    servant_job_db_table.emplace(owner, [&](auto &new_job) {
        new_job.job = _job;
        new_job.min_range.base_str = _min;
        new_job.min_range.base_dex = _min;
        new_job.min_range.base_int = _min;

        new_job.max_range.base_str = _max;
        new_job.max_range.base_dex = _max;
        new_job.max_range.base_int = _max;
    });
}

void unlimitgacha::insert_body(uint32_t _appear)
{
    body_db body_db_table(owner, owner.value);
    body_db_table.emplace(owner, [&](auto &new_body) {
        new_body.body = _appear;
    });
}

void unlimitgacha::insert_hair(uint32_t _appear)
{
    hair_db hair_db_table(owner, owner.value);
    hair_db_table.emplace(owner, [&](auto &new_hair) {
        new_hair.hair = _appear;
    });
}
void unlimitgacha::insert_head(uint32_t _appear)
{
    head_db head_db_table(owner, owner.value);
    head_db_table.emplace(owner, [&](auto &new_head) {
        new_head.head = _appear;
    });
}

void unlimitgacha::insert_gender(uint32_t _appear)
{
    gender_db gender_db_table(owner, owner.value);
    gender_db_table.emplace(owner, [&](auto &new_gender) {
        new_gender.gender = _appear;
    });
}

void unlimitgacha::insert_servant_id(uint32_t _id, uint32_t _index)
{
    servant_id_db servant_id_db_table(owner, owner.value);
    servant_id_db_table.emplace(owner, [&](auto &new_servant_id) {
        new_servant_id.id = _id;
        new_servant_id.index = _index;
    });
}

void unlimitgacha::insert_monster_id(uint32_t _id)
{
    monster_id_db monster_id_db_table(owner, owner.value);
    monster_id_db_table.emplace(owner, [&](auto &new_monster_id) {
        new_monster_id.id = _id;
    });
}

void unlimitgacha::insert_monster_grade(uint32_t _grade, uint32_t _min, uint32_t _max)
{
    monster_grade_db monster_grade_db_table(owner, owner.value);
    monster_grade_db_table.emplace(owner, [&](auto &new_monster_grade) {
        new_monster_grade.grade = _grade;
        new_monster_grade.min_range.base_dex = _min;
        new_monster_grade.min_range.base_int = _min;
        new_monster_grade.min_range.base_str = _min;
        new_monster_grade.max_range.base_dex = _max;
        new_monster_grade.max_range.base_int = _max;
        new_monster_grade.max_range.base_str = _max;
    });
}

void unlimitgacha::insert_item_id(uint32_t _id, uint32_t _type, uint32_t _job, uint32_t _tier)
{
    item_id_db item_id_db_table(owner, owner.value);
    item_id_db_table.emplace(owner, [&](auto &new_item_id) {
        new_item_id.id = _id;
        new_item_id.type = _type;
        new_item_id.job = _job;
        new_item_id.tier = _tier;
    });
}

void unlimitgacha::insert_item_grade(uint32_t _grade, uint32_t _min, uint32_t _max)
{
    item_grade_db item_grade_db_table(owner, owner.value);
    item_grade_db_table.emplace(owner, [&](auto &new_item_grade) {
        new_item_grade.grade = _grade;
        new_item_grade.min_range.base_dex = _min;
        new_item_grade.min_range.base_int = _min;
        new_item_grade.min_range.base_str = _min;
        new_item_grade.max_range.base_dex = _max;
        new_item_grade.max_range.base_int = _max;
        new_item_grade.max_range.base_str = _max;
    });
}

ACTION unlimitgacha::dbmodify(uint32_t _kind, uint32_t _appear, uint32_t _id, uint32_t _index, uint32_t _job, uint32_t _tier, uint32_t _type, uint32_t _grade, uint32_t _min, uint32_t _max)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(master_iter->master.value);
    eosio_assert(owner_iter->state == euser_state::pause, " not server checking");
    switch (_kind)
    {
    case db_choice::job:
    {
        modify_job(_job, _min, _max);
        break;
    }
    case db_choice::body:
    {
        modify_body(_appear);
        break;
    }
    case db_choice::hair:
    {
        modify_hair(_appear);
        break;
    }
    case db_choice::head:
    {
        modify_head(_appear);
        break;
    }
    case db_choice::gender:
    {
        modify_gender(_appear);
        break;
    }
    case db_choice::servant_id:
    {
        modify_servant_id(_id, _index);
        break;
    }
    case db_choice::monster_id:
    {
        modify_monster_id(_id);
        break;
    }
    case db_choice::monster_grade:
    {
        modify_monster_grade(_grade, _min, _max);
        break;
    }
    case db_choice::item_id:
    {
        modify_item_id(_id, _type, _job, _tier);
        break;
    }
    case db_choice::item_grade:
    {
        modify_item_grade(_grade, _min, _max);
        break;
    }
    }
}

void unlimitgacha::modify_job(uint32_t _job, uint32_t _min, uint32_t _max)
{
    servant_job_db servant_job_db_table(owner, owner.value);
    auto job_iter = servant_job_db_table.find(_job);
    eosio_assert(job_iter != servant_job_db_table.end(), "not exist job data");
    servant_job_db_table.modify(job_iter, owner, [&](auto &modify_job) {
        modify_job.job = _job;
        modify_job.min_range.base_str = _min;
        modify_job.min_range.base_dex = _min;
        modify_job.min_range.base_int = _min;

        modify_job.max_range.base_str = _max;
        modify_job.max_range.base_dex = _max;
        modify_job.max_range.base_int = _max;
    });
}

void unlimitgacha::modify_body(uint32_t _appear)
{
    body_db body_db_table(owner, owner.value);
    auto body_iter = body_db_table.find(_appear);
    eosio_assert(body_iter != body_db_table.end(), "not find body data");
    body_db_table.modify(body_iter, owner, [&](auto &modify_body) {
        modify_body.body = _appear;
    });
}

void unlimitgacha::modify_hair(uint32_t _appear)
{
    hair_db hair_db_table(owner, owner.value);
    auto hair_iter = hair_db_table.find(_appear);
    eosio_assert(hair_iter != hair_db_table.end(), "not find hair data");
    hair_db_table.modify(hair_iter, owner, [&](auto &modify_hair) {
        modify_hair.hair = _appear;
    });
}

void unlimitgacha::modify_head(uint32_t _appear)
{
    head_db head_db_table(owner, owner.value);
    auto head_iter = head_db_table.find(_appear);
    eosio_assert(head_iter != head_db_table.end(), "not find head data");
    head_db_table.modify(head_iter, owner, [&](auto &modify_head) {
        modify_head.head = _appear;
    });
}

void unlimitgacha::modify_gender(uint32_t _appear)
{
    gender_db gender_db_table(owner, owner.value);
    auto gender_iter = gender_db_table.find(_appear);
    eosio_assert(gender_iter != gender_db_table.end(), "not find gender data");
    gender_db_table.modify(gender_iter, owner, [&](auto &modify_gender) {
        modify_gender.gender = _appear;
    });
}

void unlimitgacha::modify_servant_id(uint32_t _id, uint32_t _index)
{
    servant_id_db servant_id_db_table(owner, owner.value);
    auto servant_id_iter = servant_id_db_table.find(_id);
    eosio_assert(servant_id_iter != servant_id_db_table.end(), "not find servant id data");
    servant_id_db_table.modify(servant_id_iter, owner, [&](auto &modify_servant_id) {
        modify_servant_id.id = _id;
        modify_servant_id.index = _index;
    });
}

void unlimitgacha::modify_monster_id(uint32_t _id)
{
    monster_id_db monster_id_db_table(owner, owner.value);
    auto monster_id_iter = monster_id_db_table.find(_id);
    eosio_assert(monster_id_iter != monster_id_db_table.end(), "not find monster id data");
    monster_id_db_table.modify(monster_id_iter, owner, [&](auto &modify_monster_id) {
        modify_monster_id.id = _id;
    });
}

void unlimitgacha::modify_monster_grade(uint32_t _grade, uint32_t _min, uint32_t _max)
{
    monster_grade_db monster_grade_db_table(owner, owner.value);
    auto monster_grade_iter = monster_grade_db_table.find(_grade);
    eosio_assert(monster_grade_iter != monster_grade_db_table.end(), "not exist monster grade data");
    monster_grade_db_table.modify(monster_grade_iter, owner, [&](auto &modify_monster_grade) {
        modify_monster_grade.grade = _grade;
        modify_monster_grade.min_range.base_str = _min;
        modify_monster_grade.min_range.base_dex = _min;
        modify_monster_grade.min_range.base_int = _min;

        modify_monster_grade.max_range.base_str = _max;
        modify_monster_grade.max_range.base_dex = _max;
        modify_monster_grade.max_range.base_int = _max;
    });
}

void unlimitgacha::modify_item_grade(uint32_t _grade, uint32_t _min, uint32_t _max)
{
    item_grade_db item_grade_db_table(owner, owner.value);
    auto item_grade_iter = item_grade_db_table.find(_grade);
    eosio_assert(item_grade_iter != item_grade_db_table.end(), "not exist item grade data");
    item_grade_db_table.modify(item_grade_iter, owner, [&](auto &modify_item_grade) {
        modify_item_grade.grade = _grade;
        modify_item_grade.min_range.base_str = _min;
        modify_item_grade.min_range.base_dex = _min;
        modify_item_grade.min_range.base_int = _min;

        modify_item_grade.max_range.base_str = _max;
        modify_item_grade.max_range.base_dex = _max;
        modify_item_grade.max_range.base_int = _max;
    });
}

void unlimitgacha::modify_item_id(uint32_t _id, uint32_t _type, uint32_t _job, uint32_t _tier)
{
    item_id_db item_id_db_table(owner, owner.value);
    auto item_id_iter = item_id_db_table.find(_id);
    eosio_assert(item_id_iter != item_id_db_table.end(), "not exist item id data");
    item_id_db_table.modify(item_id_iter, owner, [&](auto &modify_item_id) {
        modify_item_id.id = _id;
        modify_item_id.type = _type;
        modify_item_id.job = _job;
        modify_item_id.tier = _tier;
    });
}

ACTION unlimitgacha::dberase(uint32_t _kind, uint32_t _appear, uint32_t _id, uint32_t _job, uint32_t _tier, uint32_t _type, uint32_t _grade, uint32_t _min, uint32_t _max)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(master_iter -> master.value);
    eosio_assert(owner_iter->state == euser_state::pause, " not server checking");

    switch (_kind)
    {
    case db_choice::job:
    {
        erase_job(_job);
        break;
    }
    case db_choice::body:
    {
        erase_body(_appear);
        break;
    }
    case db_choice::hair:
    {
        erase_hair(_appear);
        break;
    }
    case db_choice::head:
    {
        erase_head(_appear);
        break;
    }
    case db_choice::gender:
    {
        erase_gender(_appear);
        break;
    }
    case db_choice::servant_id:
    {
        erase_servant_id(_id);
        break;
    }
    case db_choice::monster_id:
    {
        erase_monster_id(_id);
        break;
    }
    case db_choice::monster_grade:
    {
        erase_monster_grade(_grade);
        break;
    }
    case db_choice::item_id:
    {
        erase_item_id(_id);
        break;
    }
    case db_choice::item_grade:
    {
        erase_item_grade(_grade);
        break;
    }
    }
}

void unlimitgacha::erase_job(uint32_t _job)
{
    servant_job_db servant_job_db_table(owner, owner.value);
    auto job_iter = servant_job_db_table.find(_job);
    eosio_assert(job_iter != servant_job_db_table.end(), "not exist job data");
    servant_job_db_table.erase(job_iter);
}

void unlimitgacha::erase_body(uint32_t _appear)
{
    body_db body_db_table(owner, owner.value);
    auto body_iter = body_db_table.find(_appear);
    eosio_assert(body_iter != body_db_table.end(), "not find body data");
    body_db_table.erase(body_iter);
}

void unlimitgacha::erase_hair(uint32_t _appear)
{
    hair_db hair_db_table(owner, owner.value);
    auto hair_iter = hair_db_table.find(_appear);
    eosio_assert(hair_iter != hair_db_table.end(), "not find hair data");
    hair_db_table.erase(hair_iter);
}

void unlimitgacha::erase_head(uint32_t _appear)
{
    head_db head_db_table(owner, owner.value);
    auto head_iter = head_db_table.find(_appear);
    eosio_assert(head_iter != head_db_table.end(), "not find head data");
    head_db_table.erase(head_iter);
}

void unlimitgacha::erase_gender(uint32_t _appear)
{
    gender_db gender_db_table(owner, owner.value);
    auto gender_iter = gender_db_table.find(_appear);
    eosio_assert(gender_iter != gender_db_table.end(), "not find gender data");
    gender_db_table.erase(gender_iter);
}

void unlimitgacha::erase_servant_id(uint32_t _id)
{
    servant_id_db servant_id_db_table(owner, owner.value);
    auto servant_id_iter = servant_id_db_table.find(_id);
    eosio_assert(servant_id_iter != servant_id_db_table.end(), "not find servant id data");
    servant_id_db_table.erase(servant_id_iter);
}

void unlimitgacha::erase_monster_id(uint32_t _id)
{
    monster_id_db monster_id_db_table(owner, owner.value);
    auto monster_id_iter = monster_id_db_table.find(_id);
    eosio_assert(monster_id_iter != monster_id_db_table.end(), "not find monster id data");
    monster_id_db_table.erase(monster_id_iter);
}

void unlimitgacha::erase_monster_grade(uint32_t _grade)
{
    monster_grade_db monster_grade_db_table(owner, owner.value);
    auto monster_grade_iter = monster_grade_db_table.find(_grade);
    eosio_assert(monster_grade_iter != monster_grade_db_table.end(), "not exist monster grade data");
    monster_grade_db_table.erase(monster_grade_iter);
}

void unlimitgacha::erase_item_grade(uint32_t _grade)
{
    item_grade_db item_grade_db_table(owner, owner.value);
    auto item_grade_iter = item_grade_db_table.find(_grade);
    eosio_assert(item_grade_iter != item_grade_db_table.end(), "not exist item grade data");
    item_grade_db_table.erase(item_grade_iter);
}

void unlimitgacha::erase_item_id(uint32_t _id)
{
    item_id_db item_id_db_table(owner, owner.value);
    auto item_id_iter = item_id_db_table.find(_id);
    eosio_assert(item_id_iter != item_id_db_table.end(), "not exist item id data");
    item_id_db_table.erase(item_id_iter);
}

ACTION unlimitgacha::dbinit()
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(master_iter -> master.value);
    eosio_assert(owner_iter->state == euser_state::pause, " not server checking");

    head_db head_db_table(owner, owner.value);
    hair_db hair_db_table(owner, owner.value);
    body_db body_db_table(owner, owner.value);
    monster_grade_db monster_grade_db_table(owner, owner.value);
    monster_id_db monster_id_db_table(owner, owner.value);
    item_id_db item_id_db_table(owner, owner.value);
    item_grade_db item_grade_db_table(owner, owner.value);

    gender_db gender_db_table(owner, owner.value);
    servant_job_db servant_job_db_table(owner, owner.value);
    servant_id_db servant_id_db_table(owner, owner.value);

    for (auto gender_db_table_iter = gender_db_table.begin(); gender_db_table_iter != gender_db_table.end();)
    {
        auto iter = gender_db_table.find(gender_db_table_iter->primary_key());
        gender_db_table_iter++;
        gender_db_table.erase(iter);
    }

    for (auto servant_id_db_table_iter = servant_id_db_table.begin(); servant_id_db_table_iter != servant_id_db_table.end();)
    {
        auto iter = servant_id_db_table.find(servant_id_db_table_iter->primary_key());
        servant_id_db_table_iter++;
        servant_id_db_table.erase(iter);
    }

    for (auto servant_job_db_table_iter = servant_job_db_table.begin(); servant_job_db_table_iter != servant_job_db_table.end();)
    {
        auto iter = servant_job_db_table.find(servant_job_db_table_iter->primary_key());
        servant_job_db_table_iter++;
        servant_job_db_table.erase(iter);
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
}

#pragma endresion

//------------------------------------------------------------------------//
//-----------------------------setdata_action-----------------------------//
//------------------------------------------------------------------------//

#pragma region set
ACTION unlimitgacha::setmaster(eosio::name _master)
{
    require_auth(owner_auth);
    master master_table(owner, owner.value);
    auto master_iter = master_table.find(_master.value);
    eosio_assert(master_iter == master_table.end(), "already set master table");
    master_table.emplace(owner, [&](auto &set_master)
    {
        set_master.master = _master;   
    });

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(_master.value);
    eosio_assert(owner_iter == user_auth_table.end(),"already set master");
    user_auth_table.emplace(owner, [&](auto &gm_set) {
        gm_set.user = _master;
        gm_set.state = euser_state::lobby;

        hero_info first_hero;
        first_hero.equip_slot.resize(max_equip_slot);
        first_hero.state = hero_state::set_complete;

        gm_set.hero = first_hero;
    });

}

ACTION unlimitgacha::setpresale()
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    accumulate_presale_logs accumulate_presale_log_table(owner, owner.value);
    auto participation_log_iter = accumulate_presale_log_table.find(master_iter->master.value);
    eosio_assert(participation_log_iter == accumulate_presale_log_table.end(),"already set presale log");
    accumulate_presale_log_table.emplace(owner, [&](auto &pre_sale) {
        pre_sale.owner = master_iter->master;
    });
}

#pragma endregion

//------------------------------------------------------------------------//
//-------------------------------login_action-----------------------------//
//------------------------------------------------------------------------//

#pragma region login

void unlimitgacha::presalesign(eosio::name _user, uint64_t _seed)
{
    auth_users auth_user_table(owner, owner.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "this user already signup in presale signup");
    auth_user_table.emplace(owner, [&](auto &new_user) {
        new_user.user = _user;
        new_user.state = euser_state::presale;

        hero_info first_hero;
        first_hero.equip_slot.resize(max_equip_slot);
        first_hero.state = hero_state::set_look;

        new_user.hero = first_hero;
    });

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter == user_log_table.end(), "this user already signup in presale signup");
    user_log_table.emplace(owner, [&](auto &new_log) {
        new_log.user = _user;
    });

    uint64_t user = _user.value;
    uint64_t seed = safeseed::get_seed_value(user, _seed);

    //uint64_t seed = safeseed::get_seed(owner.value, _user.value);

    monster_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(seed, max_rate, default_min, monster_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    monster_grade_db monster_grade_db_table(owner, owner.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "not exist monster grade");

    user_pre_sale_monsters user_monster_table(owner, _user.value);
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

        update_user_monster_list.id = 20036;
        update_user_monster_list.grade = monster_grade_db_iter.grade;
        monster_random_count += 1;
        update_user_monster_list.status.basic_str = safeseed::get_random_value(seed, monster_grade_db_iter.max_range.base_str, monster_grade_db_iter.min_range.base_str, monster_random_count);
        monster_random_count += 1;
        update_user_monster_list.status.basic_dex = safeseed::get_random_value(seed, monster_grade_db_iter.max_range.base_dex, monster_grade_db_iter.min_range.base_dex, monster_random_count);
        monster_random_count += 1;
        update_user_monster_list.status.basic_int = safeseed::get_random_value(seed, monster_grade_db_iter.max_range.base_int, monster_grade_db_iter.min_range.base_int, monster_random_count);
    });
}

void unlimitgacha::signup(eosio::name _user)
{
    auth_users auth_user_table(owner, owner.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "this user already exist in signup");

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
    eosio_assert(user_log_iter == user_log_table.end(), "this user already exist in signup");
    user_log_table.emplace(owner, [&](auto &new_log) {
        new_log.user = _user;
    });
}
ACTION unlimitgacha::presalemove(eosio::name _user)
{
    eosio::require_auth(_user);

    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");

    auth_users auth_user_table(owner, owner.value);
    auto owner_iter = auth_user_table.find(master_iter->master.value);
    eosio_assert(owner_iter != auth_user_table.end(),"not set owner");
    eosio_assert(owner_iter->state != euser_state::pause, " server checking... ");

    black_list black_list_table(owner, owner.value);
    auto black_list_iter = black_list_table.find(_user.value);
    eosio_assert(black_list_iter == black_list_table.end(), "this user already exist in black list");

    accumulate_presale_logs accumulate_presale_log_table(owner, owner.value);
    auto presale_log_iter = accumulate_presale_log_table.find(master_iter->master.value);
    eosio_assert(presale_log_iter == accumulate_presale_log_table.end(), "It is still a presale period");

    auto pre_user_iter = auth_user_table.find(_user.value);
    eosio_assert(pre_user_iter != auth_user_table.end(), "You are not a presales participant");
    eosio_assert(pre_user_iter->state == euser_state::presale, "already move object");

    auth_user_table.modify(pre_user_iter, owner, [&](auto &pre_user_move_object) {
        pre_user_move_object.state = euser_state::lobby;
    });

    user_servants user_servant_table(owner, _user.value);
    user_pre_sale_servants user_servant_pre_table(owner, _user.value);
    for (auto user_servant_iter = user_servant_pre_table.begin(); user_servant_iter != user_servant_pre_table.end();)
    {
        user_servant_table.emplace(owner, [&](auto &move_servant) {
            move_servant.index = user_servant_iter->index;
            move_servant.party_number = 0;
            move_servant.servant.id = user_servant_iter->id;
            move_servant.servant.state = eobject_state::on_inventory;
            move_servant.servant.status = user_servant_iter->status;
            move_servant.servant.equip_slot.resize(3);
        });

        user_servant_iter++;
    }

    user_monsters user_monster_table(owner, _user.value);
    user_pre_sale_monsters user_monster_pre_table(owner, _user.value);
    for (auto user_monster_iter = user_monster_pre_table.begin(); user_monster_iter != user_monster_pre_table.end();)
    {
        user_monster_table.emplace(owner, [&](auto &move_monster) {
            move_monster.index = user_monster_iter->index;
            move_monster.party_number = 0;
            move_monster.monster.id = user_monster_iter->id;
            move_monster.monster.state = eobject_state::on_inventory;
            move_monster.monster.grade = user_monster_iter->grade;
            move_monster.monster.status = user_monster_iter->status;
        });

        user_monster_iter++;
    }

    user_items user_item_table(owner, _user.value);
    user_pre_sale_items user_item_pre_table(owner, _user.value);
    for (auto user_item_iter = user_item_pre_table.begin(); user_item_iter != user_item_pre_table.end();)
    {
        user_item_table.emplace(owner, [&](auto &move_item) {
            move_item.index = user_item_iter->index;

            move_item.item.id = user_item_iter->id;
            move_item.item.type = user_item_iter->type;
            move_item.item.tier = user_item_iter->tier;
            move_item.item.job = user_item_iter->job;

            move_item.item.grade = user_item_iter->grade;
            move_item.item.status.basic_str = user_item_iter->main_status + 10;
            move_item.item.status.basic_dex = user_item_iter->main_status + 10;
            move_item.item.status.basic_int = user_item_iter->main_status + 10;
        });

        user_item_iter++;
    }
    delete_user_presale_data(_user);
}

// eosio.token recipient
// memo description spec
//-------------------------------------------------------------------------

ACTION unlimitgacha::eostransfer(eosio::name sender, eosio::name receiver)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");

    auth_users auth_user_table(owner, owner.value);
    auto owner_iter = auth_user_table.find(master_iter->master.value);
    eosio_assert(owner_iter != auth_user_table.end(),"not set owner");
    eosio_assert(owner_iter->state != euser_state::pause, " server checking... ");

    black_list black_list_table(owner, owner.value);
    auto black_list_iter = black_list_table.find(sender.value);
    eosio_assert(black_list_iter == black_list_table.end(), "this user already exist in black list");

    eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
        eosio_assert(ad.action.size() != 0,"wrong action");
        if (ad.action == action_signup)
        {
            accumulate_presale_logs accumulate_presale_log_table(owner, owner.value);
            auto presale_log_iter = accumulate_presale_log_table.find(master_iter->master.value);
            eosio_assert(presale_log_iter == accumulate_presale_log_table.end(), "need to presale signup");

            signup(sender);
        }
        else if (ad.action == action_presale_signup)
        {
            accumulate_presale_logs accumulate_presale_log_table(owner, owner.value);
            auto presale_log_iter = accumulate_presale_log_table.find(master_iter->master.value);
            eosio_assert(presale_log_iter != accumulate_presale_log_table.end(), "pre sale time over");

            presalesign(sender, ad.type);

            asset presale_signup_reward(0, symbol(symbol_code("UTG"), 4));
            presale_signup_reward.amount = 30000000; // 3000 UTG

            uint64_t limt_check = presale_log_iter->accumulate_token_amount + presale_signup_reward.amount;
            if (limt_check <= limit_token_amount)
            {
                accumulate_presale_log_table.modify(presale_log_iter, owner, [&](auto &update_participation_list) {
                    update_participation_list.accumulate_token_amount += presale_signup_reward.amount;
                });

                action(permission_level{get_self(), "active"_n},
                       get_self(), "transfer"_n,
                       std::make_tuple(owner, sender, presale_signup_reward, std::string("presale signup reward")))
                    .send();
            }
            else
            {
                eosio_assert(limt_check < limit_token_amount, "end presale");
            }
        }
        else if (ad.action == action_gacha)
        {
            accumulate_presale_logs accumulate_presale_log_table(owner, owner.value);
            auto presale_log_iter = accumulate_presale_log_table.find(master_iter->master.value);
            if (presale_log_iter == accumulate_presale_log_table.end())
            {
                start_gacha(sender, ad.type);
            }
            else
            {
                presale_gacha(sender, ad.type);

                asset gacha_reward(0, symbol(symbol_code("UTG"), 4));
                if (presale_log_iter->accumulate_token_amount < 300000000000) //1만명 제한 test 10명 골드 제한
                {
                    gacha_reward.amount = 30000000;
                }
                else if (presale_log_iter->accumulate_token_amount < 600000000000) //3만명 제한 test 40명 골드 제한
                {
                    gacha_reward.amount = 20000000;
                }
                else if (presale_log_iter->accumulate_token_amount < 600000000000) //6만명 제한 test 100명 골드로 변환
                {
                    gacha_reward.amount = 10000000;
                }
                else
                {
                    gacha_reward.amount = 5000000;
                }

                accumulate_presale_log_table.modify(presale_log_iter, owner, [&](auto &update_participation_list) {
                    update_participation_list.accumulate_token_amount += gacha_reward.amount;
                });
                
                action(permission_level{get_self(), "active"_n},
                       get_self(), "transfer"_n,
                       std::make_tuple(owner, sender, gacha_reward, std::string("presale gacha reward")))
                    .send();
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
        eosio_assert(transfer_data.quantity.amount == 10000, "gacha need 1.0000 EOS");

        std::string l_seed = transfer_data.memo.substr(l_center + 1, (l_next - l_center - 1));
        std::string l_sha = transfer_data.memo.substr(l_next + 1, l_end);

        res.type = safeseed::check_seed(l_seed, l_sha);

        eosio_assert(res.type != 0, "wrong seed convert");

        user_logs user_log_table(owner, owner.value);
        auto user_log_iter = user_log_table.find(sender.value);

        eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data in eosiotoken transfer");
        user_log_table.modify(user_log_iter, owner, [&](auto &buy_log) {
            buy_log.use_eos += transfer_data.quantity.amount;
        });
    }
    else if (res.action == "presalesignup")
    {
        size_t l_next = transfer_data.memo.find(':', l_center + 1);
        size_t l_end = transfer_data.memo.length() - (l_next + 1);

        eosio_assert(transfer_data.memo.find(':') != std::string::npos, "seed memo [:] error");
        eosio_assert(transfer_data.memo.find(':', l_center + 1) != std::string::npos, "seed memo [:] error");
        eosio_assert(transfer_data.quantity.amount == 10000, "presalesignup need 1.0000 EOS");

        std::string l_seed = transfer_data.memo.substr(l_center + 1, (l_next - l_center - 1));
        std::string l_sha = transfer_data.memo.substr(l_next + 1, l_end);

        res.type = safeseed::check_seed(l_seed, l_sha);

        eosio_assert(res.type != 0, "wrong seed convert");
    }
    else if (res.action == "signup")
    {
        eosio_assert(transfer_data.quantity.amount == 10000, "signup need 1.0000 EOS");
    }
    else
    {
        master master_table(owner, owner.value);
        if (owner != sender)
        {
            auto master_iter = master_table.find(sender.value);
            eosio_assert(master_iter != master_table.end(), "impossible send to this contract");
        }
        else
        {
            auto master_iter = master_table.find(receiver.value);
            eosio_assert(master_iter != master_table.end(), "receiver is not master");
        }
        // uint32_t error_code = 0;
        // eosio_assert(error_code != 0 , " impossible send to this contract ");
    }

    func(res);
}

#pragma endregion

#pragma resion init db table
ACTION unlimitgacha::initmaster(eosio::name _master)
{
    master master_table(owner,owner.value);
    auto master_iter = master_table.find(_master.value);
    eosio_assert(master_iter != master_table.end(),"not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    master_table.erase(master_iter);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(_master.value);
    eosio_assert(owner_iter != user_auth_table.end(), "not set master");
    user_auth_table.erase(owner_iter);
}

#pragma endregion

#pragma resion delete user table

ACTION unlimitgacha::deleteuser(eosio::name _user)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    delete_user_data(_user);
    delete_user_object_data(_user);
    delete_user_presale_data(_user);
    delete_user_balance(_user);
}

void unlimitgacha::delete_user_data(eosio::name _user)
{
    auth_users auth_user_table(owner, owner.value);
    auto user_auth_iter = auth_user_table.find(_user.value);
    eosio_assert(user_auth_iter != auth_user_table.end(), "not exist user auth data");
    auth_user_table.erase(user_auth_iter);

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user auth data");
    if(user_log_iter->gacha_num != 0)
    {
        delete_user_gacha_result_data(_user);
    }
    user_log_table.erase(user_log_iter);
}

void unlimitgacha::delete_user_object_data(eosio::name _user)
{
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

void unlimitgacha::delete_user_presale_data(eosio::name _user)
{
    user_pre_sale_servants user_servant_table(owner, _user.value);
    for (auto user_servant_iter = user_servant_table.begin(); user_servant_iter != user_servant_table.end();)
    {
        auto iter = user_servant_table.find(user_servant_iter->primary_key());
        user_servant_iter++;
        user_servant_table.erase(iter);
    }

    user_pre_sale_monsters user_monster_table(owner, _user.value);
    for (auto user_monster_iter = user_monster_table.begin(); user_monster_iter != user_monster_table.end();)
    {
        auto iter = user_monster_table.find(user_monster_iter->primary_key());
        user_monster_iter++;
        user_monster_table.erase(iter);
    }

    user_pre_sale_items user_item_table(owner, _user.value);
    for (auto user_item_iter = user_item_table.begin(); user_item_iter != user_item_table.end();)
    {
        auto iter = user_item_table.find(user_item_iter->primary_key());
        user_item_iter++;
        user_item_table.erase(iter);
    }
}

void unlimitgacha::delete_user_gacha_result_data(eosio::name _user)
{
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



ACTION unlimitgacha::initprelog()
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    accumulate_presale_logs accumulate_presale_log_table(owner, owner.value);
    auto iter = accumulate_presale_log_table.find(master_iter->master.value);
    eosio_assert(iter != accumulate_presale_log_table.end(), "not exist presale log data");
    accumulate_presale_log_table.erase(iter);
}


#pragma endregion

#pragma resion init token

ACTION unlimitgacha::inittoken(asset _token)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

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
//-------------------------------gacha_function---------------------------//
//------------------------------------------------------------------------//
uint32_t unlimitgacha::get_random_grade(uint64_t _rate)
{
    uint32_t grade;
    if (_rate <= one_grade_ratio)
    {
        grade = 1;
    }
    else if (_rate <= two_grade_ratio)
    {
        grade = 2;
    }
    else if (_rate <= three_grade_ratio)
    {
        grade = 3;
    }
    else if (_rate <= four_grade_ratio)
    {
        grade = 4;
    }
    else
    {
        grade = 5;
    }
    return grade;
}
uint32_t unlimitgacha::get_servant_index(uint32_t _job, uint32_t _body, uint32_t _gender, uint32_t _head, uint32_t _hair)
{
    uint32_t result_id;
    result_id = (_job * 1000000) + (_body * 100000) + (_gender * 10000) + (_head * 100) + (_hair * 1);
    return result_id;
}

void unlimitgacha::gacha_servant_id(eosio::name _user, uint64_t _seed)
{
    servant_job_db servant_job_table(owner, owner.value);
    uint32_t random_job = safeseed::get_random_value(_seed, servant_job_count, default_min, servant_random_count);
    const auto &servant_job_db_iter = servant_job_table.get(random_job, "not get servant job data");

    servant_random_count += 1;
    uint32_t random_body = gacha_servant_body(_seed, servant_random_count);

    servant_random_count += 1;
    gender_db gender_table(owner, owner.value);
    uint32_t random_gender = safeseed::get_random_value(_seed, gender_count, DEFAULT_MIN_DB, servant_random_count);
    const auto &gender_db_iter = gender_table.get(random_gender, "not get gender data");

    servant_random_count += 1;
    uint32_t random_head = gacha_servant_head(_seed, servant_random_count);

    servant_random_count += 1;
    uint32_t random_hair = gacha_servant_hair(_seed, servant_random_count);

    servant_id_db servant_id_table(owner, owner.value);
    uint32_t servant_index = get_servant_index(random_job, random_body, random_gender, random_head, random_hair);
    const auto &servant_id_db_iter = servant_id_table.get(servant_index, "not exist servant id");

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data in gacha servant");
 
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

        new_servant.id = servant_id_db_iter.id;
        servant_random_count += 1;
        new_servant.status.basic_str = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_str, servant_job_db_iter.min_range.base_str, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_dex = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_dex, servant_job_db_iter.min_range.base_dex, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_int = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_int, servant_job_db_iter.min_range.base_int, servant_random_count);
        
        new_servant.equip_slot.resize(3);
        new_servant.state = eobject_state::on_inventory;

        result.index = update_user_servant_list.index;
        result.type = result::servant;

        update_user_servant_list.party_number = 0;
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
    uint8_t random_head = safeseed::get_random_value(_seed, head_count, DEFAULT_MIN_DB, _count);
    const auto &head_db_iter = head_db_table.get(random_head, "not exist head info");
    return head_db_iter.head;
}

uint8_t unlimitgacha::gacha_servant_hair(uint64_t _seed, uint32_t _count)
{
    hair_db hair_db_table(owner, owner.value);
    uint8_t random_hair = safeseed::get_random_value(_seed, hair_count, DEFAULT_MIN_DB, _count);
    const auto &hair_db_iter = hair_db_table.get(random_hair, "not exist hair info");
    return hair_db_iter.hair;
}

uint8_t unlimitgacha::gacha_servant_body(uint64_t _seed, uint32_t _count)
{
    body_db body_db_table(owner, owner.value);
    uint8_t random_body = safeseed::get_random_value(_seed, body_count, DEFAULT_MIN_DB, _count);
    const auto &body_db_iter = body_db_table.get(random_body, "not exist body info");
    return body_db_iter.body;
}

void unlimitgacha::gacha_monster_id(eosio::name _user, uint64_t _seed)
{
    monster_id_db monster_id_db_table(owner, owner.value);
    uint32_t random_monster_id = safeseed::get_random_value(_seed, monster_id_count, default_min, monster_random_count);
    random_monster_id += 20000;
    const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id, "not exist monster id");

    monster_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, monster_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    monster_grade_db monster_grade_db_table(owner, owner.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "not exist monster grade");

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data in gacha monster");

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
        new_monster.id = monster_id_db_iter.id;
        new_monster.grade = monster_grade_db_iter.grade;
        monster_random_count += 1;
        new_monster.status.basic_str = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_str, monster_grade_db_iter.min_range.base_str, monster_random_count);
        monster_random_count += 1;
        new_monster.status.basic_dex = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_dex, monster_grade_db_iter.min_range.base_dex, monster_random_count);
        monster_random_count += 1;
        new_monster.status.basic_int = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_int, monster_grade_db_iter.min_range.base_int, monster_random_count);
        new_monster.state = eobject_state::on_inventory;

        result.index = update_user_monster_list.index;
        result.type = result::monster;

        update_user_monster_list.party_number = 0;
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
    uint32_t random_item_id = safeseed::get_random_value(_seed, item_id_count, default_min, item_random_count);
    random_item_id += 30000;
    const auto &item_id_db_iter = item_id_db_table.get(random_item_id, "not exist item id");

    item_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, item_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    item_grade_db item_grade_db_table(owner, owner.value);
    const auto &item_grade_db_iter = item_grade_db_table.get(random_grade, "not exist tier info");

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data in gacha item");

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
        new_item.type = item_id_db_iter.type;
        new_item.tier = item_id_db_iter.tier;
        new_item.job = item_id_db_iter.job;
        new_item.grade = item_grade_db_iter.grade;
        item_random_count += 1;
        new_item.status.basic_str = safeseed::get_random_value(_seed, item_grade_db_iter.max_range.base_str, item_grade_db_iter.min_range.base_str, item_random_count);
        item_random_count += 1;
        new_item.status.basic_dex = safeseed::get_random_value(_seed, item_grade_db_iter.max_range.base_dex, item_grade_db_iter.min_range.base_dex, item_random_count);
        item_random_count += 1;
        new_item.status.basic_int = safeseed::get_random_value(_seed, item_grade_db_iter.max_range.base_int, item_grade_db_iter.min_range.base_int, item_random_count);
        new_item.state = eobject_state::on_inventory;

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
        if (l_gacha_result_type < 333)
        {
            gacha_servant_id(_user, l_seed);
        }
        else if (l_gacha_result_type > 333 && l_gacha_result_type <= 666)
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

void unlimitgacha::presale_gacha_servant_id(eosio::name _user, uint64_t _seed)
{
    servant_job_db servant_job_table(owner, owner.value);
    uint32_t random_job = safeseed::get_random_value(_seed, servant_job_count, DEFAULT_MIN_DB, servant_random_count);
    const auto &servant_job_db_iter = servant_job_table.get(random_job, "not get servant job data");

    servant_random_count += 1;
    uint32_t random_body = gacha_servant_body(_seed, servant_random_count);

    servant_random_count += 1;
    gender_db gender_table(owner, owner.value);
    uint32_t random_gender = safeseed::get_random_value(_seed, gender_count, DEFAULT_MIN_DB, servant_random_count);
    const auto &gender_db_iter = gender_table.get(random_gender, "not get gender data");

    servant_random_count += 1;
    uint32_t random_head = gacha_servant_head(_seed, servant_random_count);

    servant_random_count += 1;
    uint32_t random_hair = gacha_servant_hair(_seed, servant_random_count);

    servant_id_db servant_id_table(owner, owner.value);
    uint32_t servant_index = get_servant_index(random_job, random_body, random_gender, random_head, random_hair);
    const auto &servant_id_db_iter = servant_id_table.get(servant_index, "not exist servant id");

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data in pre gacha servant");

    result_info result;
    user_pre_sale_servants user_servant_table(owner, _user.value);
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

        update_user_servant_list.id = servant_id_db_iter.id;
        servant_random_count += 1;
        update_user_servant_list.status.basic_str = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_str, servant_job_db_iter.min_range.base_str, servant_random_count);
        servant_random_count += 1;
        update_user_servant_list.status.basic_dex = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_dex, servant_job_db_iter.min_range.base_dex, servant_random_count);
        servant_random_count += 1;
        update_user_servant_list.status.basic_int = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_int, servant_job_db_iter.min_range.base_int, servant_random_count);

        result.index = update_user_servant_list.index;
        result.type = result::servant;
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

void unlimitgacha::presale_gacha_monster_id(eosio::name _user, uint64_t _seed)
{
    monster_id_db monster_id_db_table(owner, owner.value);
    uint64_t random_monster_id = safeseed::get_random_value(_seed, monster_id_count, DEFAULT_MIN_DB, monster_random_count);
    random_monster_id += 20000;
    const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id, "not exist monster id");

    monster_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, monster_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    monster_grade_db monster_grade_db_table(owner, owner.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "not exist monster grade");

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data in pre gacha monster");

    result_info result;
    user_pre_sale_monsters user_monster_table(owner, _user.value);
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

        update_user_monster_list.id = monster_id_db_iter.id;
        update_user_monster_list.grade = monster_grade_db_iter.grade;
        monster_random_count += 1;
        update_user_monster_list.status.basic_str = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_str, monster_grade_db_iter.min_range.base_str, monster_random_count);
        monster_random_count += 1;
        update_user_monster_list.status.basic_dex = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_dex, monster_grade_db_iter.min_range.base_dex, monster_random_count);
        monster_random_count += 1;
        update_user_monster_list.status.basic_int = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_int, monster_grade_db_iter.min_range.base_int, monster_random_count);

        result.index = update_user_monster_list.index;
        result.type = result::monster;
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

void unlimitgacha::presale_gacha_item_id(eosio::name _user, uint64_t _seed)
{
    item_id_db item_id_db_table(owner, owner.value);
    uint64_t random_item_id = safeseed::get_random_value(_seed, item_id_count, DEFAULT_MIN_DB, item_random_count);
    random_item_id += 30000;
    const auto &item_id_db_iter = item_id_db_table.get(random_item_id, "not exist item id");

    item_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, item_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    item_grade_db item_grade_db_table(owner, owner.value);
    const auto &item_grade_db_iter = item_grade_db_table.get(random_grade, "not exist tier info");

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log data in pre gacha item");

    result_info result;
    user_pre_sale_items user_item_table(owner, _user.value);
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

        update_user_item_list.id = item_id_db_iter.id;
        update_user_item_list.type = item_id_db_iter.type;
        update_user_item_list.tier = item_id_db_iter.tier;
        update_user_item_list.job = item_id_db_iter.job;
        update_user_item_list.grade = item_grade_db_iter.grade;
        item_random_count += 1;
        update_user_item_list.main_status = safeseed::get_random_value(_seed, item_grade_db_iter.max_range.base_str, item_grade_db_iter.min_range.base_str, item_random_count);

        result.index = update_user_item_list.index;
        result.type = result::item;
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

void unlimitgacha::presale_gacha(eosio::name _user, uint64_t _seed)
{
    uint64_t l_user = get_user_seed_value(_user.value);
    uint64_t l_seed = safeseed::get_seed_value(l_user, _seed);

    uint64_t l_gacha_result_type = safeseed::get_random_value(l_seed, max_rate, default_min, DEFAULE_RANDOM_COUNT);
    if (l_gacha_result_type < 333)
    {
        presale_gacha_servant_id(_user, l_seed);
    }
    else if (l_gacha_result_type > 333 && l_gacha_result_type <= 666)
    {
        presale_gacha_monster_id(_user, l_seed);
    }
    else
    {
        presale_gacha_item_id(_user, l_seed);
    }

    servant_random_count = 0;
    monster_random_count = 0;
    item_random_count = 0;
}



#pragma region black_list action

ACTION unlimitgacha::deleteblack(eosio::name _user)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    black_list black_list_table(owner, owner.value);
    auto black_list_iter = black_list_table.find(_user.value);
    eosio_assert(black_list_iter != black_list_table.end(), "this user not exist in black list");

    black_list_table.erase(black_list_iter);
}

ACTION unlimitgacha::addblack(eosio::name _user)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    black_list black_list_table(owner, owner.value);
    auto black_list_iter = black_list_table.find(_user.value);
    eosio_assert(black_list_iter == black_list_table.end(), "this user already exist in black list");

    black_list_table.emplace(owner, [&](auto &new_black_user) {
        new_black_user.user = _user;
    });
}

#pragma endregion

#pragma region owenr action

ACTION unlimitgacha::setpause(uint64_t _state)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    eosio_assert(master_iter != master_table.end(), "not set master");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(master_iter->master.value);
    eosio_assert(owner_iter != user_auth_table.end(), "not set gm account");

    user_auth_table.modify(owner_iter, owner, [&](auto &set_owner_account) {
        set_owner_account.state = _state;
    });
}

#pragma endregion



#pragma endresion

#undef EOSIO_DISPATCH

#define EOSIO_DISPATCH(TYPE, MEMBERS)                                                       \
    extern "C"                                                                              \
    {                                                                                       \
        void apply(uint64_t receiver, uint64_t code, uint64_t action)                       \
        {                                                                                   \
            if (code == receiver)                                                           \
            {                                                                               \
                eosio_assert(action != name("eostransfer").value,"impossible this action");                                    \
                    switch (action)                                                         \
                    {                                                                       \
                        EOSIO_DISPATCH_HELPER(TYPE, MEMBERS)                                \
                    }                                                                       \
                    /* does not allow destructor of thiscontract to run: eosio_exit(0); */ \
            }                                                                               \
            else if (code == name("eosio.token").value && action == name("transfer").value) \
            {                                                                               \
                eosio_assert(code == name("eosio.token").value, "Must transfer EOS");       \
                execute_action(name(receiver), name(code), &unlimitgacha::eostransfer);     \
            }                                                                               \
        }                                                                                   \
    }
// eos 금액에 대해 체크 하는 함

EOSIO_DISPATCH(unlimitgacha, (create)(issue)(transfer)(setmaster)(setpresale)(presalemove)(eostransfer)(initmaster)(deleteuser)(initprelog)(inittoken)(deleteblack)(addblack)(setpause)(dbinsert)(dbmodify)(dberase)(dbinit))