#include "Common/common_header.hpp"
#include "Common/common_seed.hpp"
#include "untpreregist.hpp"

//------------------------------------------------------------------------//
//----------------------------unlimited_tower_action----------------------//
//------------------------------------------------------------------------//

//------------------------------------------------------------------------//
//----------------------------Token_action--------------------------------//
//------------------------------------------------------------------------//
#pragma region Token action

ACTION untpreregist::create(eosio::name _issuer, asset _maximum_supply)
{
    require_auth(owner_auth);

    auto sym = _maximum_supply.symbol;
    eosio_assert(sym.is_valid(), "invalid symbol name");
    eosio_assert(_maximum_supply.is_valid(), "invalid Supply");

    eosio_assert(_maximum_supply.amount > 0, "max supply more than 0");

    stat statstable(owner, sym.code().raw());
    auto existing = statstable.find(sym.code().raw());
    eosio_assert(existing == statstable.end(), "token symbol already exists");

    statstable.emplace(owner, [&](auto &s) {
        s.supply.symbol = _maximum_supply.symbol;
        s.max_supply = _maximum_supply;
        s.issuer = _issuer;
    });
}

ACTION untpreregist::issue(eosio::name _to, asset _quantity, string _memo)
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

ACTION untpreregist::transfer(name _from, name _to, asset _quantity, string _memo)
{
    blacklist blacklist_table(owner, owner.value);
    auto blacklist_iter = blacklist_table.find(_from.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "black list user1");

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

void untpreregist::sub_balance(name _user, asset _value)
{
    accounts from_acnts(owner, _user.value);

    const auto &from = from_acnts.get(_value.symbol.code().raw(), "No balance object found");
    eosio_assert(from.balance.amount >= _value.amount, "over account balance");

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

void untpreregist::add_balance(name _user, asset _value, name _ram_payer)
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

ACTION untpreregist::dbinsert(uint32_t _kind, uint32_t _appear, uint32_t _id, uint32_t _index, uint32_t _job, uint32_t _tier, uint32_t _type, uint32_t _grade, uint32_t _min, uint32_t _max, uint32_t _ratio)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(master_iter->master.value);
    eosio_assert(owner_iter->state == euser_state::pause, "not server checking1");

    switch (_kind)
    {
    case db_index::job:
    {
        insert_job(_job, _min, _max);
        break;
    }
    case db_index::body:
    {
        insert_body(_appear);
        break;
    }
    case db_index::hair:
    {
        insert_hair(_appear);
        break;
    }
    case db_index::head:
    {
        insert_head(_appear);
        break;
    }
    case db_index::gender:
    {
        insert_gender(_appear);
        break;
    }
    case db_index::servant_id:
    {
        insert_servant_id(_id, _index);
        break;
    }
    case db_index::monster_id:
    {
        insert_monster_id(_id);
        break;
    }
    case db_index::monster_grade:
    {
        insert_monster_grade(_grade, _min, _max);
        break;
    }
    case db_index::item_id:
    {
        insert_item_id(_id, _type, _job, _tier);
        break;
    }
    case db_index::item_grade:
    {
        insert_item_grade(_grade, _min, _max);
        break;
    }
    case db_index::grade_ratio:
    {
        insert_grade_ratio(_grade, _ratio);
        break;
    }
    }
}
void untpreregist::insert_job(uint32_t _job, uint32_t _min, uint32_t _max)
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

void untpreregist::insert_body(uint32_t _appear)
{
    body_db body_db_table(owner, owner.value);
    body_db_table.emplace(owner, [&](auto &new_body) {
        new_body.body = _appear;
    });
}

void untpreregist::insert_hair(uint32_t _appear)
{
    hair_db hair_db_table(owner, owner.value);
    hair_db_table.emplace(owner, [&](auto &new_hair) {
        new_hair.hair = _appear;
    });
}
void untpreregist::insert_head(uint32_t _appear)
{
    head_db head_db_table(owner, owner.value);
    head_db_table.emplace(owner, [&](auto &new_head) {
        new_head.head = _appear;
    });
}

void untpreregist::insert_gender(uint32_t _appear)
{
    gender_db gender_db_table(owner, owner.value);
    gender_db_table.emplace(owner, [&](auto &new_gender) {
        new_gender.gender = _appear;
    });
}

void untpreregist::insert_servant_id(uint32_t _id, uint32_t _index)
{
    servant_id_db servant_id_db_table(owner, owner.value);
    servant_id_db_table.emplace(owner, [&](auto &new_servant_id) {
        new_servant_id.id = _id;
        new_servant_id.index = _index;
    });
}

void untpreregist::insert_monster_id(uint32_t _id)
{
    monster_id_db monster_id_db_table(owner, owner.value);
    monster_id_db_table.emplace(owner, [&](auto &new_monster_id) {
        new_monster_id.id = _id;
    });
}

void untpreregist::insert_monster_grade(uint32_t _grade, uint32_t _min, uint32_t _max)
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

void untpreregist::insert_item_id(uint32_t _id, uint32_t _type, uint32_t _job, uint32_t _tier)
{
    item_id_db item_id_db_table(owner, owner.value);
    item_id_db_table.emplace(owner, [&](auto &new_item_id) {
        new_item_id.id = _id;
        new_item_id.type = _type;
        new_item_id.job = _job;
        new_item_id.tier = _tier;
    });
}

void untpreregist::insert_item_grade(uint32_t _grade, uint32_t _min, uint32_t _max)
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

void untpreregist::insert_grade_ratio(uint32_t _grade, uint32_t _ratio)
{
    grade_ratio_db grade_ratio_db_table(owner, owner.value);
    grade_ratio_db_table.emplace(owner, [&](auto &new_grade_ratio)
    {
        new_grade_ratio.grade = _grade;
        new_grade_ratio.ratio = _ratio;
    });
}

ACTION untpreregist::dbmodify(uint32_t _kind, uint32_t _appear, uint32_t _id, uint32_t _index, uint32_t _job, uint32_t _tier, uint32_t _type, uint32_t _grade, uint32_t _min, uint32_t _max, uint32_t _ratio)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(master_iter->master.value);
    eosio_assert(owner_iter->state == euser_state::pause, "not server checking2");
    switch (_kind)
    {
    case db_index::job:
    {
        modify_job(_job, _min, _max);
        break;
    }
    case db_index::body:
    {
        modify_body(_appear);
        break;
    }
    case db_index::hair:
    {
        modify_hair(_appear);
        break;
    }
    case db_index::head:
    {
        modify_head(_appear);
        break;
    }
    case db_index::gender:
    {
        modify_gender(_appear);
        break;
    }
    case db_index::servant_id:
    {
        modify_servant_id(_id, _index);
        break;
    }
    case db_index::monster_id:
    {
        modify_monster_id(_id);
        break;
    }
    case db_index::monster_grade:
    {
        modify_monster_grade(_grade, _min, _max);
        break;
    }
    case db_index::item_id:
    {
        modify_item_id(_id, _type, _job, _tier);
        break;
    }
    case db_index::item_grade:
    {
        modify_item_grade(_grade, _min, _max);
        break;
    }
    case db_index::grade_ratio:
    {
        modify_grade_ratio(_grade, _ratio);
        break;
    }
    }
}

void untpreregist::modify_job(uint32_t _job, uint32_t _min, uint32_t _max)
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

void untpreregist::modify_body(uint32_t _appear)
{
    body_db body_db_table(owner, owner.value);
    auto body_iter = body_db_table.find(_appear);
    eosio_assert(body_iter != body_db_table.end(), "not find body data");
    body_db_table.modify(body_iter, owner, [&](auto &modify_body) {
        modify_body.body = _appear;
    });
}

void untpreregist::modify_hair(uint32_t _appear)
{
    hair_db hair_db_table(owner, owner.value);
    auto hair_iter = hair_db_table.find(_appear);
    eosio_assert(hair_iter != hair_db_table.end(), "not find hair data");
    hair_db_table.modify(hair_iter, owner, [&](auto &modify_hair) {
        modify_hair.hair = _appear;
    });
}

void untpreregist::modify_head(uint32_t _appear)
{
    head_db head_db_table(owner, owner.value);
    auto head_iter = head_db_table.find(_appear);
    eosio_assert(head_iter != head_db_table.end(), "not find head data");
    head_db_table.modify(head_iter, owner, [&](auto &modify_head) {
        modify_head.head = _appear;
    });
}

void untpreregist::modify_gender(uint32_t _appear)
{
    gender_db gender_db_table(owner, owner.value);
    auto gender_iter = gender_db_table.find(_appear);
    eosio_assert(gender_iter != gender_db_table.end(), "not find gender data");
    gender_db_table.modify(gender_iter, owner, [&](auto &modify_gender) {
        modify_gender.gender = _appear;
    });
}

void untpreregist::modify_servant_id(uint32_t _id, uint32_t _index)
{
    servant_id_db servant_id_db_table(owner, owner.value);
    auto servant_id_iter = servant_id_db_table.find(_id);
    eosio_assert(servant_id_iter != servant_id_db_table.end(), "not find servant id data");
    servant_id_db_table.modify(servant_id_iter, owner, [&](auto &modify_servant_id) {
        modify_servant_id.id = _id;
        modify_servant_id.index = _index;
    });
}

void untpreregist::modify_monster_id(uint32_t _id)
{
    monster_id_db monster_id_db_table(owner, owner.value);
    auto monster_id_iter = monster_id_db_table.find(_id);
    eosio_assert(monster_id_iter != monster_id_db_table.end(), "not find monster id data");
    monster_id_db_table.modify(monster_id_iter, owner, [&](auto &modify_monster_id) {
        modify_monster_id.id = _id;
    });
}

void untpreregist::modify_monster_grade(uint32_t _grade, uint32_t _min, uint32_t _max)
{
    monster_grade_db monster_grade_db_table(owner, owner.value);
    auto monster_grade_iter = monster_grade_db_table.find(_grade);
    eosio_assert(monster_grade_iter != monster_grade_db_table.end(), "not exist monster grade1");
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

void untpreregist::modify_item_grade(uint32_t _grade, uint32_t _min, uint32_t _max)
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

void untpreregist::modify_item_id(uint32_t _id, uint32_t _type, uint32_t _job, uint32_t _tier)
{
    item_id_db item_id_db_table(owner, owner.value);
    auto item_id_iter = item_id_db_table.find(_id);
    eosio_assert(item_id_iter != item_id_db_table.end(), "not exist item id1");
    item_id_db_table.modify(item_id_iter, owner, [&](auto &modify_item_id) {
        modify_item_id.id = _id;
        modify_item_id.type = _type;
        modify_item_id.job = _job;
        modify_item_id.tier = _tier;
    });
}

void untpreregist::modify_grade_ratio(uint32_t _grade, uint32_t _ratio)
{
    grade_ratio_db grade_ratio_db_table(owner, owner.value);
    auto grade_ratio_db_iter = grade_ratio_db_table.find(_grade);
    eosio_assert( grade_ratio_db_iter != grade_ratio_db_table.end() , "not exist grade ratio data" );
    grade_ratio_db_table.modify(grade_ratio_db_iter ,owner, [&](auto &new_grade_ratio) {
        new_grade_ratio.ratio = _ratio;
    });
}


ACTION untpreregist::dberase(uint32_t _kind, uint32_t _appear, uint32_t _id, uint32_t _job, uint32_t _tier, uint32_t _type, uint32_t _grade, uint32_t _min, uint32_t _max)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();

    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(master_iter -> master.value);
    eosio_assert(owner_iter->state == euser_state::pause, "not server checking3");

    switch (_kind)
    {
    case db_index::job:
    {
        erase_job(_job);
        break;
    }
    case db_index::body:
    {
        erase_body(_appear);
        break;
    }
    case db_index::hair:
    {
        erase_hair(_appear);
        break;
    }
    case db_index::head:
    {
        erase_head(_appear);
        break;
    }
    case db_index::gender:
    {
        erase_gender(_appear);
        break;
    }
    case db_index::servant_id:
    {
        erase_servant_id(_id);
        break;
    }
    case db_index::monster_id:
    {
        erase_monster_id(_id);
        break;
    }
    case db_index::monster_grade:
    {
        erase_monster_grade(_grade);
        break;
    }
    case db_index::item_id:
    {
        erase_item_id(_id);
        break;
    }
    case db_index::item_grade:
    {
        erase_item_grade(_grade);
        break;
    }
    case db_index::grade_ratio:
    {
        erase_grade_ratio(_grade);
        break;
    }
    }
}

void untpreregist::erase_job(uint32_t _job)
{
    servant_job_db servant_job_db_table(owner, owner.value);
    auto job_iter = servant_job_db_table.find(_job);
    eosio_assert(job_iter != servant_job_db_table.end(), "not exist job data");
    servant_job_db_table.erase(job_iter);
}

void untpreregist::erase_body(uint32_t _appear)
{
    body_db body_db_table(owner, owner.value);
    auto body_iter = body_db_table.find(_appear);
    eosio_assert(body_iter != body_db_table.end(), "not find body data");
    body_db_table.erase(body_iter);
}

void untpreregist::erase_hair(uint32_t _appear)
{
    hair_db hair_db_table(owner, owner.value);
    auto hair_iter = hair_db_table.find(_appear);
    eosio_assert(hair_iter != hair_db_table.end(), "not find hair data");
    hair_db_table.erase(hair_iter);
}

void untpreregist::erase_head(uint32_t _appear)
{
    head_db head_db_table(owner, owner.value);
    auto head_iter = head_db_table.find(_appear);
    eosio_assert(head_iter != head_db_table.end(), "not find head data");
    head_db_table.erase(head_iter);
}

void untpreregist::erase_gender(uint32_t _appear)
{
    gender_db gender_db_table(owner, owner.value);
    auto gender_iter = gender_db_table.find(_appear);
    eosio_assert(gender_iter != gender_db_table.end(), "not find gender data");
    gender_db_table.erase(gender_iter);
}

void untpreregist::erase_servant_id(uint32_t _id)
{
    servant_id_db servant_id_db_table(owner, owner.value);
    auto servant_id_iter = servant_id_db_table.find(_id);
    eosio_assert(servant_id_iter != servant_id_db_table.end(), "not find servant id data");
    servant_id_db_table.erase(servant_id_iter);
}

void untpreregist::erase_monster_id(uint32_t _id)
{
    monster_id_db monster_id_db_table(owner, owner.value);
    auto monster_id_iter = monster_id_db_table.find(_id);
    eosio_assert(monster_id_iter != monster_id_db_table.end(), "not find monster id data");
    monster_id_db_table.erase(monster_id_iter);
}

void untpreregist::erase_monster_grade(uint32_t _grade)
{
    monster_grade_db monster_grade_db_table(owner, owner.value);
    auto monster_grade_iter = monster_grade_db_table.find(_grade);
    eosio_assert(monster_grade_iter != monster_grade_db_table.end(), "not exist monster grade2");
    monster_grade_db_table.erase(monster_grade_iter);
}

void untpreregist::erase_item_grade(uint32_t _grade)
{
    item_grade_db item_grade_db_table(owner, owner.value);
    auto item_grade_iter = item_grade_db_table.find(_grade);
    eosio_assert(item_grade_iter != item_grade_db_table.end(), "not exist item grade data");
    item_grade_db_table.erase(item_grade_iter);
}

void untpreregist::erase_item_id(uint32_t _id)
{
    item_id_db item_id_db_table(owner, owner.value);
    auto item_id_iter = item_id_db_table.find(_id);
    eosio_assert(item_id_iter != item_id_db_table.end(), "not exist item id2");
    item_id_db_table.erase(item_id_iter);
}

void untpreregist::erase_grade_ratio(uint32_t _grade)
{
    grade_ratio_db grade_ratio_db_table(owner, owner.value);
    auto grade_ratio_db_iter = grade_ratio_db_table.find(_grade);
    eosio_assert( grade_ratio_db_iter != grade_ratio_db_table.end() , "not exist grade ratio data" );
    grade_ratio_db_table.erase(grade_ratio_db_iter);
}

ACTION untpreregist::dbinit()
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();

    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(master_iter -> master.value);
    eosio_assert(owner_iter->state == euser_state::pause, "not server checking4");

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
ACTION untpreregist::setmaster(eosio::name _master)
{
    require_auth(owner_auth);
    master master_table(owner, owner.value);
    auto master_iter = master_table.find(_master.value);
    eosio_assert(master_iter == master_table.end(), "already set master1");
    master_table.emplace(owner, [&](auto &set_master)
    {
        set_master.master = _master;   
    });

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(_master.value);
    eosio_assert(owner_iter == user_auth_table.end(),"already set master2");
    user_auth_table.emplace(owner, [&](auto &gm_set) {
        gm_set.user = _master;
        gm_set.state = euser_state::lobby;

        hero_info first_hero;
        first_hero.equip_slot.resize(max_equip_slot);
        first_hero.state = hero_state::set_complete;

        gm_set.hero = first_hero;
    });

}

ACTION untpreregist::setpreregist()
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();

    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    total_token_logs total_token_log_table(owner, owner.value);
    auto total_token_log_iter = total_token_log_table.find(master_iter->master.value);
    eosio_assert(total_token_log_iter == total_token_log_table.end(),"already set preregist log");
    total_token_log_table.emplace(owner, [&](auto &preregist) {
        preregist.owner = master_iter->master;
    });
}

#pragma endregion

//------------------------------------------------------------------------//
//-------------------------------login_action-----------------------------//
//------------------------------------------------------------------------//

#pragma region login

void untpreregist::presignup(eosio::name _user, uint64_t _seed)
{
    auth_users auth_user_table(owner, owner.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "user already signup1");
    auth_user_table.emplace(owner, [&](auto &new_user) {
        new_user.user = _user;
        new_user.state = euser_state::pre_regist;

        hero_info first_hero;
        first_hero.equip_slot.resize(max_equip_slot);
        first_hero.state = hero_state::set_look;

        new_user.hero = first_hero;
    });

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter == user_log_table.end(), "user already signup2");
    user_log_table.emplace(owner, [&](auto &new_log) {
        new_log.user = _user;
    });

    uint64_t user = _user.value + now();
    uint64_t seed = safeseed::get_seed_value(user, _seed);


    monster_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(seed, max_rate, default_min, monster_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    monster_grade_db monster_grade_db_table(owner, owner.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "not exist monster grade3");

    user_preregist_monsters user_monster_table(owner, _user.value);
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

void untpreregist::signup(eosio::name _user)
{
    auth_users auth_user_table(owner, owner.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "user already signup2");

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
    eosio_assert(user_log_iter == user_log_table.end(), "user already signup3");
    user_log_table.emplace(owner, [&](auto &new_log) {
        new_log.user = _user;
    });
}
ACTION untpreregist::preregistmov(eosio::name _user)
{
    eosio::require_auth(_user);

    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();

    auth_users auth_user_table(owner, owner.value);
    auto owner_iter = auth_user_table.find(master_iter->master.value);
    eosio_assert(owner_iter != auth_user_table.end(),"not set master1");
    eosio_assert(owner_iter->state != euser_state::pause, " server checking6 ");

    blacklist blacklist_table(owner, owner.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "black list user2");

    total_token_logs total_token_log_table(owner, owner.value);
    auto total_token_log_iter = total_token_log_table.find(master_iter->master.value);
    eosio_assert(total_token_log_iter == total_token_log_table.end(), "It is still a preregist period");

    auto pre_user_iter = auth_user_table.find(_user.value);
    eosio_assert(pre_user_iter != auth_user_table.end(), "You are not a preregists participant");
    eosio_assert(pre_user_iter->state == euser_state::pre_regist, "already move object");

    auth_user_table.modify(pre_user_iter, owner, [&](auto &pre_user_move_object) {
        pre_user_move_object.state = euser_state::lobby;
    });

    user_servants user_servant_table(owner, _user.value);
    user_preregist_servants user_servant_pre_table(owner, _user.value);
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
    user_preregist_monsters user_monster_pre_table(owner, _user.value);
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
    user_preregist_items user_item_pre_table(owner, _user.value);
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
    delete_user_preregist_data(_user);
}

// eosio.token recipient
// memo description spec
//-------------------------------------------------------------------------

ACTION untpreregist::eostransfer(eosio::name sender, eosio::name receiver)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();

    auth_users auth_user_table(owner, owner.value);
    auto owner_iter = auth_user_table.find(master_iter->master.value);
    eosio_assert(owner_iter != auth_user_table.end(),"not set master2");
    eosio_assert(owner_iter->state != euser_state::pause, "server checking");

    blacklist blacklist_table(owner, owner.value);
    auto blacklist_iter = blacklist_table.find(sender.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "black list user3");

    eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
        eosio_assert(ad.action.size() != 0,"wrong action");
        if (ad.action == action_signup)
        {
            total_token_logs total_token_log_table(owner, owner.value);
            auto total_token_log_iter = total_token_log_table.find(master_iter->master.value);
            eosio_assert(total_token_log_iter == total_token_log_table.end(), "need to preregist signup");

            signup(sender);
        }
        else if (ad.action == action_preregist_signup)
        {
            total_token_logs total_token_log_table(owner, owner.value);
            auto total_token_log_iter = total_token_log_table.find(master_iter->master.value);
            eosio_assert(total_token_log_iter != total_token_log_table.end(), "pre sale time over");

            presignup(sender, ad.type);

            asset preregist_signup_reward(0, symbol(symbol_code("UTG"), 4));
            preregist_signup_reward.amount = 30000000; // 3000 UTG

            uint64_t limt_check = total_token_log_iter->total_token_amount + preregist_signup_reward.amount;
            if (limt_check <= limit_token_amount)
            {
                total_token_log_table.modify(total_token_log_iter, owner, [&](auto &update_participation_list) {
                    update_participation_list.total_token_amount += preregist_signup_reward.amount;
                });

                action(permission_level{get_self(), "active"_n},
                       get_self(), "transfer"_n,
                       std::make_tuple(owner, sender, preregist_signup_reward, std::string("preregist signup reward")))
                    .send();
            }
            else
            {
                eosio_assert(limt_check < limit_token_amount, "end preregist");
            }
        }
        else if (ad.action == action_gacha)
        {
            total_token_logs total_token_log_table(owner, owner.value);
            auto total_token_log_iter = total_token_log_table.find(master_iter->master.value);
            if (total_token_log_iter == total_token_log_table.end())
            {
                start_gacha(sender, ad.type);
            }
            else
            {
                preregist_gacha(sender, ad.type);

                asset gacha_reward(0, symbol(symbol_code("UTG"), 4));
                if (total_token_log_iter->total_token_amount < 150000000) //1만eos 제한 300000000000
                {
                    gacha_reward.amount = 30000000;
                }
                else if (total_token_log_iter->total_token_amount < 450000000) //3만eos 제한 900000000000
                {
                    gacha_reward.amount = 20000000;
                }
                else if (total_token_log_iter->total_token_amount < 750000000) //6만eos 제한 1500000000000
                {
                    gacha_reward.amount = 10000000;
                }
                else
                {
                    gacha_reward.amount = 5000000;
                }

                total_token_log_table.modify(total_token_log_iter, owner, [&](auto &update_participation_list) {
                    update_participation_list.total_token_amount += gacha_reward.amount;
                });

                action(permission_level{get_self(), "active"_n},
                       get_self(), "transfer"_n,
                       std::make_tuple(owner, sender, gacha_reward, std::string("preregist gacha reward")))
                    .send();
            }
        }
    });
}

template <typename T>
void untpreregist::eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func)
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

        eosio_assert(user_log_iter != user_log_table.end(), "not exist user log1");
        user_log_table.modify(user_log_iter, owner, [&](auto &buy_log) {
            buy_log.use_eos += transfer_data.quantity.amount;
        });
    }
    else if (res.action == "presignup")
    {
        size_t l_next = transfer_data.memo.find(':', l_center + 1);
        size_t l_end = transfer_data.memo.length() - (l_next + 1);

        eosio_assert(transfer_data.memo.find(':') != std::string::npos, "seed memo [:] error");
        eosio_assert(transfer_data.memo.find(':', l_center + 1) != std::string::npos, "seed memo [:] error");
        eosio_assert(transfer_data.quantity.amount == 10000, "presignup need 1.0000 EOS");

        std::string l_seed = transfer_data.memo.substr(l_center + 1, (l_next - l_center - 1));
        std::string l_sha = transfer_data.memo.substr(l_next + 1, l_end);

        res.type = safeseed::check_seed(l_seed, l_sha);

        eosio_assert(res.type != 0, "wrong seed");
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
            eosio_assert(master_iter != master_table.end(), "impossible send EOS");
        }
        else
        {
            auto master_iter = master_table.find(receiver.value);
            eosio_assert(master_iter != master_table.end(), "impossible recv EOS");
        }
        // uint32_t error_code = 0;
        // eosio_assert(error_code != 0 , " impossible send to this contract ");
    }

    func(res);
}

#pragma endregion

#pragma resion init db table
ACTION untpreregist::initmaster(eosio::name _master)
{
    master master_table(owner,owner.value);
    auto master_iter = master_table.find(_master.value);
    eosio_assert(master_iter != master_table.end(),"not set master5");
    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    master_table.erase(master_iter);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(_master.value);
    eosio_assert(owner_iter != user_auth_table.end(), "not set master6");
    user_auth_table.erase(owner_iter);
}

#pragma endregion

#pragma region delete user table

ACTION untpreregist::deleteuser(eosio::name _user)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();

    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    delete_user_data(_user);
    delete_user_object_data(_user);
    delete_user_preregist_data(_user);
    delete_user_balance(_user);
}

void untpreregist::delete_user_data(eosio::name _user)
{
    auth_users auth_user_table(owner, owner.value);
    auto user_auth_iter = auth_user_table.find(_user.value);
    eosio_assert(user_auth_iter != auth_user_table.end(), "not exist user auth1");
    auth_user_table.erase(user_auth_iter);

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user auth2");
    if(user_log_iter->gacha_num != 0)
    {
        delete_user_gacha_result_data(_user);
    }
    user_log_table.erase(user_log_iter);
}

void untpreregist::delete_user_object_data(eosio::name _user)
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

void untpreregist::delete_user_preregist_data(eosio::name _user)
{
    user_preregist_servants user_servant_table(owner, _user.value);
    for (auto user_servant_iter = user_servant_table.begin(); user_servant_iter != user_servant_table.end();)
    {
        auto iter = user_servant_table.find(user_servant_iter->primary_key());
        user_servant_iter++;
        user_servant_table.erase(iter);
    }

    user_preregist_monsters user_monster_table(owner, _user.value);
    for (auto user_monster_iter = user_monster_table.begin(); user_monster_iter != user_monster_table.end();)
    {
        auto iter = user_monster_table.find(user_monster_iter->primary_key());
        user_monster_iter++;
        user_monster_table.erase(iter);
    }

    user_preregist_items user_item_table(owner, _user.value);
    for (auto user_item_iter = user_item_table.begin(); user_item_iter != user_item_table.end();)
    {
        auto iter = user_item_table.find(user_item_iter->primary_key());
        user_item_iter++;
        user_item_table.erase(iter);
    }
}

void untpreregist::delete_user_gacha_result_data(eosio::name _user)
{
    user_gacha_results user_gacha_current_result_table(owner, owner.value);
    user_gacha_totals user_gacha_total_table(owner, owner.value);

    auto iter = user_gacha_current_result_table.find(_user.value);
    eosio_assert(iter != user_gacha_current_result_table.end(), "not exist gacha result data");
    user_gacha_current_result_table.erase(iter);

    auto total_iter = user_gacha_total_table.find(_user.value);
    eosio_assert(total_iter != user_gacha_total_table.end(), "not exist gacha total data");
    user_gacha_total_table.erase(total_iter);
}
#pragma endregion

#pragma reion init all table



ACTION untpreregist::initprelog()
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();

    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    total_token_logs total_token_log_table(owner, owner.value);
    auto iter = total_token_log_table.find(master_iter->master.value);
    eosio_assert(iter != total_token_log_table.end(), "not exist preregist log data");
    total_token_log_table.erase(iter);
}


#pragma endregion

#pragma resion init token

ACTION untpreregist::inittoken(asset _token)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();

    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    init_all_balance();
    init_stat(_token);
}

void untpreregist::delete_user_balance(eosio::name _user)
{
    accounts user_balance_table(owner, _user.value);
    for (auto user_balance_iter = user_balance_table.begin(); user_balance_iter != user_balance_table.end();)
    {
        auto iter = user_balance_table.find(user_balance_iter->primary_key());
        user_balance_iter++;
        user_balance_table.erase(iter);
    }
}

void untpreregist::init_stat(asset _token)
{
    stat statstable(owner, _token.symbol.code().raw());
    for (auto token_stat_iter = statstable.begin(); token_stat_iter != statstable.end();)
    {
        auto iter = statstable.find(token_stat_iter->primary_key());
        token_stat_iter++;
        statstable.erase(iter);
    }
}

void untpreregist::init_all_balance()
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
uint32_t untpreregist::get_random_grade(uint64_t _rate)
{
    uint32_t grade;
    grade_ratio_db grade_ratio_db_table(owner, owner.value);
    for (auto iter = grade_ratio_db_table.begin(); iter != grade_ratio_db_table.end();)
    {
        if (_rate <= iter->ratio)
        {
            grade = iter->grade;
            break;
        }
        iter++;
    }
    return grade;
}
uint32_t untpreregist::get_servant_index(uint32_t _job, uint32_t _body, uint32_t _gender, uint32_t _head, uint32_t _hair)
{
    uint32_t result_id;
    result_id = (_job * 1000000) + (_body * 100000) + (_gender * 10000) + (_head * 100) + (_hair * 1);
    return result_id;
}

void untpreregist::gacha_servant_id(eosio::name _user, uint64_t _seed)
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
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log2");
 
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
    user_gacha_totals user_gacha_total_table(owner, owner.value);

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

    auto user_gacha_total_iter = user_gacha_total_table.find(_user.value);
    if (user_gacha_total_iter == user_gacha_total_table.end())
    {
        user_gacha_total_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_total_table.modify(user_gacha_total_iter, owner, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    //로그 남기는 부분
    user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
        update_log.servant_num += 1;
        update_log.gacha_num += 1;
    });
}

uint8_t untpreregist::gacha_servant_head(uint64_t _seed, uint32_t _count)
{
    head_db head_db_table(owner, owner.value);
    uint8_t random_head = safeseed::get_random_value(_seed, head_count, DEFAULT_MIN_DB, _count);
    const auto &head_db_iter = head_db_table.get(random_head, "not exist head info");
    return head_db_iter.head;
}

uint8_t untpreregist::gacha_servant_hair(uint64_t _seed, uint32_t _count)
{
    hair_db hair_db_table(owner, owner.value);
    uint8_t random_hair = safeseed::get_random_value(_seed, hair_count, DEFAULT_MIN_DB, _count);
    const auto &hair_db_iter = hair_db_table.get(random_hair, "not exist hair info");
    return hair_db_iter.hair;
}

uint8_t untpreregist::gacha_servant_body(uint64_t _seed, uint32_t _count)
{
    body_db body_db_table(owner, owner.value);
    uint8_t random_body = safeseed::get_random_value(_seed, body_count, DEFAULT_MIN_DB, _count);
    const auto &body_db_iter = body_db_table.get(random_body, "not exist body info");
    return body_db_iter.body;
}

void untpreregist::gacha_monster_id(eosio::name _user, uint64_t _seed)
{
    monster_id_db monster_id_db_table(owner, owner.value);
    uint32_t random_monster_id = safeseed::get_random_value(_seed, monster_id_count, default_min, monster_random_count);
    random_monster_id += 20000;
    const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id, "not exist monster id1");

    monster_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, monster_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    monster_grade_db monster_grade_db_table(owner, owner.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "not exist monster grade4");

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log3");

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
    user_gacha_totals user_gacha_total_table(owner, owner.value);

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

    auto user_gacha_total_iter = user_gacha_total_table.find(_user.value);
    if (user_gacha_total_iter == user_gacha_total_table.end())
    {
        user_gacha_total_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_total_table.modify(user_gacha_total_iter, owner, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
        update_log.gacha_num += 1;
        update_log.monster_num += 1;
    });
}

void untpreregist::gacha_item_id(eosio::name _user, uint64_t _seed)
{
    item_id_db item_id_db_table(owner, owner.value);
    uint32_t random_item_id = safeseed::get_random_value(_seed, item_id_count, default_min, item_random_count);
    random_item_id += 30000;
    const auto &item_id_db_iter = item_id_db_table.get(random_item_id, "not exist item id3");

    item_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, item_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    item_grade_db item_grade_db_table(owner, owner.value);
    const auto &item_grade_db_iter = item_grade_db_table.get(random_grade, "not exist tier");

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log4");

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
    user_gacha_totals user_gacha_total_table(owner, owner.value);

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

    auto user_gacha_total_iter = user_gacha_total_table.find(_user.value);
    if (user_gacha_total_iter == user_gacha_total_table.end())
    {
        user_gacha_total_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_total_table.modify(user_gacha_total_iter, owner, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
        update_log.item_num += 1;
        update_log.gacha_num += 1;
    });
}
uint64_t untpreregist::get_user_seed_value(uint64_t _user)
{
    user_logs user_log_table(owner, owner.value);
    const auto &user_log_iter = user_log_table.get(_user, "not exist log2");
    uint64_t user = _user + user_log_iter.gacha_num + now();
    if(user <= _user)
    {
        user = user_log_iter.gacha_num + now();
    }
    return user;
}

void untpreregist::start_gacha(eosio::name _user, uint64_t _seed)
{
    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log8");

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

void untpreregist::preregist_servant_id(eosio::name _user, uint64_t _seed)
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
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log5");

    result_info result;
    user_preregist_servants user_servant_table(owner, _user.value);
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
    user_gacha_totals user_gacha_total_table(owner, owner.value);

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

    auto user_gacha_total_iter = user_gacha_total_table.find(_user.value);
    if (user_gacha_total_iter == user_gacha_total_table.end())
    {
        user_gacha_total_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_total_table.modify(user_gacha_total_iter, owner, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    //로그 남기는 부분
    user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
        update_log.servant_num += 1;
        update_log.gacha_num += 1;
    });
}

void untpreregist::preregist_monster_id(eosio::name _user, uint64_t _seed)
{
    monster_id_db monster_id_db_table(owner, owner.value);
    uint64_t random_monster_id = safeseed::get_random_value(_seed, monster_id_count, DEFAULT_MIN_DB, monster_random_count);
    random_monster_id += 20000;
    const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id, "not exist monster id2");

    monster_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, monster_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    monster_grade_db monster_grade_db_table(owner, owner.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "not exist monster grade5");

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log6");

    result_info result;
    user_preregist_monsters user_monster_table(owner, _user.value);
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
    user_gacha_totals user_gacha_total_table(owner, owner.value);

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

    auto user_gacha_total_iter = user_gacha_total_table.find(_user.value);
    if (user_gacha_total_iter == user_gacha_total_table.end())
    {
        user_gacha_total_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_total_table.modify(user_gacha_total_iter, owner, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
        update_log.gacha_num += 1;
        update_log.monster_num += 1;
    });
}

void untpreregist::preregist_item_id(eosio::name _user, uint64_t _seed)
{
    item_id_db item_id_db_table(owner, owner.value);
    uint64_t random_item_id = safeseed::get_random_value(_seed, item_id_count, DEFAULT_MIN_DB, item_random_count);
    random_item_id += 30000;
    const auto &item_id_db_iter = item_id_db_table.get(random_item_id, "not exist item id4");

    item_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, item_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    item_grade_db item_grade_db_table(owner, owner.value);
    const auto &item_grade_db_iter = item_grade_db_table.get(random_grade, "not exist tier info");

    user_logs user_log_table(owner, owner.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not exist user log7");

    result_info result;
    user_preregist_items user_item_table(owner, _user.value);
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
    user_gacha_totals user_gacha_total_table(owner, owner.value);

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

    auto user_gacha_total_iter = user_gacha_total_table.find(_user.value);
    if (user_gacha_total_iter == user_gacha_total_table.end())
    {
        user_gacha_total_table.emplace(owner, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_total_table.modify(user_gacha_total_iter, owner, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    user_log_table.modify(user_log_iter, owner, [&](auto &update_log) {
        update_log.item_num += 1;
        update_log.gacha_num += 1;
    });
}


void untpreregist::preregist_gacha(eosio::name _user, uint64_t _seed)
{
    uint64_t l_user = get_user_seed_value(_user.value);
    uint64_t l_seed = safeseed::get_seed_value(l_user, _seed);

    uint64_t l_gacha_result_type = safeseed::get_random_value(l_seed, max_rate, default_min, DEFAULE_RANDOM_COUNT);
    if (l_gacha_result_type < 333)
    {
        preregist_servant_id(_user, l_seed);
    }
    else if (l_gacha_result_type > 333 && l_gacha_result_type <= 666)
    {
        preregist_monster_id(_user, l_seed);
    }
    else
    {
        preregist_item_id(_user, l_seed);
    }
    servant_random_count = 0;
    monster_random_count = 0;
    item_random_count = 0;
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


#pragma region party function

//------------------------------------------------------------------------//
//-------------------------------party_function---------------------------//
//------------------------------------------------------------------------//

	void untpreregist::party_init(eosio::name _user)
    {
            require_auth(_user);
            user_partys user_party_table(owner, _user.value);
            user_party_table.emplace(owner, [&](auto &new_party)
            {
                uint32_t first_index = user_party_table.available_primary_key();
                if(first_index == 0)
                {
                    new_party.index = 1;
                }
                else
                {
                    new_party.index = user_party_table.available_primary_key();
                }
                new_party.party.resize(10);
            });
        } 
    void untpreregist::set_party(uint64_t _user, uint8_t _party_number, const std::vector<uint32_t> &_party_list)
    {
            require_auth(_user);
            eosio_assert(_party_number > 0 ,"wrong party_number");          //잘못된 파티 넘버 체크 
            user_partys user_party_table(owner, _user);                     //
            auto user_party_iter = user_party_table.find(_party_number);   //유저 파티 이터레이터는 파티 테이블 번호
            eosio_assert(user_party_iter != user_party_table.end(), "not exist party"); //유저 파티 이터레이터를 기준으로 테이블 조회 시 없으면 
            eosio_assert(user_party_iter->state != party_state::on_tower_defense,"this party on tower unmodified"); //유저 파티 상태가 타워 디펜스중이 아닐시 
            user_servants user_servant_table(owner, _user);
            user_monsters user_monster_table(owner, _user);

            for (uint32_t i = 1; i < max_servant_slot; ++i)             //서번트 (4개) 까지 for문 
            {
                if(user_party_iter->party[i] != empty_party_slot && 
                user_party_iter->party[i] != _party_list[i])
                {
                    auto user_servant_iter = user_servant_table.find(user_party_iter->party[i]);
                    eosio_assert(user_servant_iter != user_servant_table.end(),"fatal party data mis");
                    user_servant_table.modify(user_servant_iter, owner, [&](auto &set_party) {
                        set_party.party_number = EMPTY_PARTY;
                    });
                }
            }
            for (uint32_t i = max_servant_slot; i < max_monster_slot; ++i)
            {
                if (user_party_iter->party[i] != empty_party_slot &&
                    user_party_iter->party[i] != _party_list[i])
                {
                    auto user_monster_iter = user_monster_table.find(user_party_iter->party[i]);
                    eosio_assert(user_monster_iter != user_monster_table.end(), "fatal party data mis");
                    user_monster_table.modify(user_monster_iter, owner, [&](auto &set_party) {
                        set_party.party_number = EMPTY_PARTY;
                    });
                }
            }
            user_party_table.modify(user_party_iter, owner, [&](auto &save_party) {
 
                for (uint32_t i = 1; i < max_servant_slot; ++i)
                {
                    if (_party_list[i] == empty_party_slot)
                    {
                        save_party.party[i] = _party_list[i];
                        continue;
                    }
                    auto user_servant_iter = user_servant_table.find(_party_list[i]);
                    eosio_assert(user_servant_iter != user_servant_table.end(), "not exist servant data");
                    eosio_assert(user_servant_iter->party_number == EMPTY_PARTY || user_servant_iter->party_number == _party_number, "already in party member servant");
                    user_servant_table.modify(user_servant_iter, owner, [&](auto &set_party) {
                      set_party.party_number = _party_number;
                    });
                    save_party.party[i] = _party_list[i];
                }
                for (uint32_t i = max_servant_slot; i < max_monster_slot; ++i)
                {
                    if (_party_list[i] == empty_party_slot)
                    {
                        save_party.party[i] = _party_list[i];
                        continue;
                    }
                    if (i != hero_partner_slot)
                    {
                        eosio_assert(_party_list[i - pair_slot] != empty_party_slot ,"need set servant");
                    }
                    auto user_monster_iter = user_monster_table.find(_party_list[i]);
                    eosio_assert(user_monster_iter != user_monster_table.end(), "not exist monster data");
                    eosio_assert(user_monster_iter->party_number == EMPTY_PARTY || user_monster_iter->party_number == _party_number, "already in party member monster");
                    user_monster_table.modify(user_monster_iter, owner, [&](auto &set_party) {
                        set_party.party_number = _party_number;
                    });
                    save_party.party[i] = _party_list[i];
                }
            });
        }

        void untpreregist::add_party_list(eosio::name _user)
        {
	    user_logs user_log_table(owner, owner.value);

            auto user_log_iter = user_log_table.find(_user.value);
            eosio_assert(user_log_iter != user_log_table.end(),"not find user information to log");
            uint32_t l_p_count = user_log_iter->add_party_count;
            l_p_count++;
            user_log_table.modify(user_log_iter,owner,[&](auto &buy_party_log)
            {
                buy_party_log.add_party_count = l_p_count;
            });
            user_partys user_party_table(owner, _user.value);
            user_party_table.emplace(owner, [&](auto &new_party)
            {
                new_party.index = user_party_table.available_primary_key();
                new_party.party.resize(10);
            });
        }
#pragma region party cheat

        void untpreregist::set_automatic_party(uint64_t _user)
        {
            uint32_t first_party = 1;
            user_partys user_party_table(owner, _user);
            auto user_party_iter = user_party_table.find(first_party);
            eosio_assert(user_party_iter != user_party_table.end(), "not exist party list");
         
            user_servants user_servant_table(owner, _user);
            user_monsters user_monster_table(owner, _user);
            user_party_table.modify(user_party_iter, owner, [&](auto &automatic_party)
            {
                for(uint32_t i=1; i<10; ++i)
                {
                    if (i < 5)
                    {
                        automatic_party.party[i] = i;

                        auto user_servant_iter = user_servant_table.find(i);
                        user_servant_table.modify(user_servant_iter, owner, [&](auto &set_party) {
                            set_party.party_number = first_party;
                        });
                    }
                    else
                    {
                        uint32_t monster_index = i - 4;
                        automatic_party.party[i] = monster_index;
                 
                        auto user_monster_iter = user_monster_table.find(monster_index);
                        user_monster_table.modify(user_monster_iter, owner, [&](auto &set_party) {
                            set_party.party_number = first_party;
                       });
                    }
                }
            });
       }

#pragma endregion

#pragma region item equipment function
//------------------------------------------------------------------------//
//-------------------------item_equipment_function------------------------//
//------------------------------------------------------------------------//
ACTION untpreregist::sellobject(eosio::name _user, uint32_t _type, uint32_t _index)
{
   require_auth(_user);

   switch(_type)
   {
   case result::servant:
   {
       sell_servant(_user, _index);
       break;
   }
   case result::monster:
   {
       sell_monster(_user, _index);
       break;
   }
   case result::item:
   {
       sell_item(_user, _index);
       break;
   }
   default:
   {
       eosio_assert(_type < 4, "wrong sell type");
       break;
   }
   };
}

void untpreregist::sell_servant(eosio::name _user, uint32_t _index)
{
   user_servants user_servant_table(owner, _user.value);
   auto user_servant_iter = user_servant_table.find(_index);
   eosio_assert(user_servant_iter != user_servant_table.end(),"not exist servant info");
   eosio_assert(user_servant_iter->party_number == 0,"this servant already in party");

   asset servant_sell_result(0, symbol(symbol_code("UTG"), 4));
   servant_sell_result.amount = 10000000;

   action(permission_level{get_self(), "active"_n},
          get_self(), "tokentrans"_n,
          std::make_tuple(owner, _user, servant_sell_result, std::string("servant sell result")))
       .send();

   user_servant_table.erase(user_servant_iter);
}

void untpreregist::sell_monster(eosio::name _user, uint32_t _index)
{
   user_monsters user_monster_table(owner, _user.value);
   auto user_monster_iter = user_monster_table.find(_index);
   eosio_assert(user_monster_iter != user_monster_table.end(),"not exist servant info");
   eosio_assert(user_monster_iter->party_number == 0,"this servant already in party");

   asset monster_sell_result(0, symbol(symbol_code("UTG"), 4));
   monster_sell_result.amount = 10000000;

   action(permission_level{get_self(), "active"_n},
          get_self(), "tokentrans"_n,
          std::make_tuple(owner, _user, monster_sell_result, std::string("monster sell result")))
       .send();

   user_monster_table.erase(user_monster_iter);
}

void untpreregist::sell_item(eosio::name _user, uint32_t _index)
{
   user_items user_item_table(owner, _user.value);
   auto user_item_iter = user_item_table.find(_index);
   eosio_assert(user_item_iter != user_item_table.end(),"not exist item info");
   eosio_assert(user_item_iter->item.state != eobject_state::on_equip_slot,"item is equip");

   asset item_sell_result(0, symbol(symbol_code("UTG"), 4));
   item_sell_result.amount = 10000000;

   action(permission_level{get_self(), "active"_n},
          get_self(), "tokentrans"_n,
          std::make_tuple(owner, _user, item_sell_result, std::string("item sell result")))
       .send();

   user_item_table.erase(user_item_iter);
}

ACTION untpreregist::equipment(eosio::name _user, uint32_t _type ,uint32_t _servant_index ,uint32_t _item_index)
{
   require_auth(_user);
   switch (_type)
   {
   case 1:
       equip_servant(_user, _servant_index, _item_index);
       break;
   case 2:
       equip_hero(_user, _item_index);
       break;
   default:
   eosio_assert(_type < 3 ,"not exsit this action type");
       break;
   }
}

ACTION untpreregist::unequipment(eosio::name _user, uint32_t _type ,uint32_t _servant_index ,uint32_t _item_index)
{
   require_auth(_user);
   switch (_type)
   {
   case 1:
       unequip_servant(_user, _servant_index, _item_index);
       break;
   case 2:
       unequip_hero(_user, _item_index);
       break;
   default:
   eosio_assert(_type < 3 ,"not exsit this action type");
       break;
   }
}

void untpreregist::unequip_servant(eosio::name _user, uint32_t _servant_index ,uint32_t _item_index)
{
   user_items user_item_table(owner, _user.value);
   auto user_item_iter = user_item_table.find(_item_index);
   eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
   eosio_assert(user_item_iter->item.state == eobject_state::on_equip_slot, "already unequip this item");

   uint32_t slot = user_item_iter->item.type - 1;
   user_item_table.modify(user_item_iter, owner, [&](auto &unequip_item)
   {
       unequip_item.item.state = eobject_state::on_inventory;
   });

   user_servants user_servant_table(owner, _user.value);
   auto user_servant_iter = user_servant_table.find(_servant_index);
   eosio_assert(user_servant_iter != user_servant_table.end(),"not exist servant info");

   user_servant_table.modify(user_servant_iter, owner, [&](auto &unequip_servant)
   {
       unequip_servant.servant.status.plus_str -= user_item_iter->item.status.basic_str;
       unequip_servant.servant.status.plus_dex -= user_item_iter->item.status.basic_dex;
       unequip_servant.servant.status.plus_int -= user_item_iter->item.status.basic_int;
       unequip_servant.servant.equip_slot[slot] = 0;
   });
}

void untpreregist::equip_servant(eosio::name _user, uint32_t _servant_index ,uint32_t _item_index)
{
   user_items user_item_table(owner, _user.value);
   auto user_item_iter = user_item_table.find(_item_index);
   eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
   eosio_assert(user_item_iter->item.state != eobject_state::on_equip_slot, "already equip this item");

   uint32_t slot = user_item_iter->item.type - 1;
   user_item_table.modify(user_item_iter, owner, [&](auto &unequip_item)
   {
       unequip_item.item.state = eobject_state::on_equip_slot;
   });

   user_servants user_servant_table(owner, _user.value);
   auto user_servant_iter = user_servant_table.find(_servant_index);
   eosio_assert(user_servant_iter != user_servant_table.end(),"not exist servant info");

   user_servant_table.modify(user_servant_iter, owner, [&](auto &unequip_servant)
   {
       unequip_servant.servant.status.plus_str += user_item_iter->item.status.basic_str;
       unequip_servant.servant.status.plus_dex += user_item_iter->item.status.basic_dex;
       unequip_servant.servant.status.plus_int += user_item_iter->item.status.basic_int;
       unequip_servant.servant.equip_slot[slot] = _item_index;
   });
}
void untpreregist::unequip_hero(eosio::name _user, uint32_t _item_index)
{
   user_items user_item_table(owner, _user.value);
   auto user_item_iter = user_item_table.find(_item_index);
   eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
   eosio_assert(user_item_iter->item.state == eobject_state::on_equip_slot, "already unequip this item");

   uint32_t slot = user_item_iter->item.type - 1;
   user_item_table.modify(user_item_iter, owner, [&](auto &unequip_item)
   {
       unequip_item.item.state = eobject_state::on_inventory;
   });

   auth_users user_auth_table(owner, owner.value);
   auto user_auth_iter = user_auth_table.find(_user.value);
   eosio_assert(user_auth_iter != user_auth_table.end(),"not exist user info");

   user_auth_table.modify(user_auth_iter, owner, [&](auto &unequip_hero)
   {
       unequip_hero.hero.status.plus_str -= user_item_iter->item.status.basic_str;
       unequip_hero.hero.status.plus_dex -= user_item_iter->item.status.basic_dex;
       unequip_hero.hero.status.plus_int -= user_item_iter->item.status.basic_int;
       unequip_hero.hero.equip_slot[slot] = 0;
   });
}

void untpreregist::equip_hero(eosio::name _user, uint32_t _item_index)
{
   user_items user_item_table(owner, _user.value);
   auto user_item_iter = user_item_table.find(_item_index);
   eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
   eosio_assert(user_item_iter->item.state != eobject_state::on_equip_slot, "already equip this item");

   uint32_t slot = user_item_iter->item.type - 1;
   user_item_table.modify(user_item_iter, owner, [&](auto &unequip_item)
   {
       unequip_item.item.state = eobject_state::on_equip_slot;
   });

   auth_users user_auth_table(owner, owner.value);
   auto user_auth_iter = user_auth_table.find(_user.value);
   eosio_assert(user_auth_iter != user_auth_table.end(),"not exist user info");

   user_auth_table.modify(user_auth_iter, owner, [&](auto &unequip_hero)
   {
       unequip_hero.hero.status.plus_str += user_item_iter->item.status.basic_str;
       unequip_hero.hero.status.plus_dex += user_item_iter->item.status.basic_dex;
       unequip_hero.hero.status.plus_int += user_item_iter->item.status.basic_int;
       unequip_hero.hero.equip_slot[slot] = _item_index;
   });
}


#pragma endregion

#pragma region battle function
//------------------------------------------------------------------------//
//---------------------------------battle_function------------------------//
//------------------------------------------------------------------------//


ACTION untpreregist::stageinsert(uint32_t _kind, uint32_t _stage_num, uint32_t _id, uint32_t _str, uint32_t _dex, uint32_t _int)
{
    require_auth(owner);
    switch (_kind)
    {
    case db_battle_choice::db_stage_num:
    {
        insert_stage(_stage_num);
        break;
    }
    case db_battle_choice::db_stage_monster:
    {
        insert_stage_monster(_stage_num, _id, _str, _dex, _int);
        break;
    }
    default:
    {
        uint32_t error_code = 0;
        uint32_t error_code1 = 0;
        uint32_t error_code2 = 0;
        uint32_t error_code3 = 0;
        uint32_t error_code4 = 0;
        uint32_t error_code5 = 0;

        eosio_assert(error_code != 0, "not exist type");
        break;
    }
    };
}


void untpreregist::insert_stage(uint64_t _stage_num)
{
    stage_db stage_db_table(owner, owner.value);
    stage_db_table.emplace(owner, [&](auto &new_stage) {
        new_stage.stage_num = _stage_num;
    });
}

void untpreregist::insert_stage_monster(uint64_t _stage_num, uint32_t _id, uint32_t _str, uint32_t _dex, uint32_t _int)
{
    stage_db stage_db_table(owner, owner.value);   
    auto stage_iter = stage_db_table.find(_stage_num);
    eosio_assert(stage_iter != stage_db_table.end(),"not exist stage number");
    stage_db_table.modify(stage_iter, owner, [&](auto &new_monster)
    {
        monster_info new_enemy;
        new_enemy.id = _id;
        new_enemy.status.basic_str = _str;
        new_enemy.status.basic_dex = _dex;
        new_enemy.status.basic_int = _int;

        new_monster.enemy_list.push_back(new_enemy);
    });
}

ACTION untpreregist::stagemodify(uint32_t _stage_num, uint32_t _id, uint32_t _str, uint32_t _dex, uint32_t _int)
{
    require_auth(owner);
    modify_stage_monster(_stage_num, _id, _str, _dex, _int);

}


void untpreregist::modify_stage_monster(uint64_t _stage_num, uint32_t _id, uint32_t _str, uint32_t _dex, uint32_t _int)
{
    require_auth(owner);
    stage_db stage_db_table(owner, owner.value);
    auto stage_iter = stage_db_table.find(_stage_num);
    eosio_assert(stage_iter != stage_db_table.end(), "not exist stage number");

    bool change = false;
    stage_db_table.modify(stage_iter, owner, [&](auto &new_monster)
    {
        for (uint32_t i = 0; i < stage_iter->enemy_list.size(); ++i)
        {
            if (stage_iter->enemy_list[i].id == _id)
            {
                new_monster.enemy_list[i].status.basic_str = _str;
                new_monster.enemy_list[i].status.basic_dex = _dex;
                new_monster.enemy_list[i].status.basic_int = _int;
                change = true;
                break;
            }
        }
        eosio_assert(change == true ,"not exist monster id");
    });
}

ACTION untpreregist::stageerease(uint32_t _kind, uint32_t _stage_num, uint32_t _id)
{
    require_auth(owner);
    switch (_kind)
    {
    case db_battle_choice::db_stage_num:
    {
        erase_stage(_stage_num);
        break;
    }
    case db_battle_choice::db_stage_monster:
    {
        erase_stage_monster(_stage_num, _id);
        break;
    }
    };
}

void untpreregist::erase_stage(uint64_t _stage_num)
{
    require_auth(owner);
    stage_db stage_db_table(owner, owner.value);

    auto stage_iter = stage_db_table.find(_stage_num);
    eosio_assert(stage_iter != stage_db_table.end(), "not exist stage data");
    stage_db_table.erase(stage_iter);
}


void untpreregist::erase_stage_monster(uint64_t _stage_num, uint32_t _id)
{
    require_auth(owner);
    stage_db stage_db_table(owner, owner.value);
    auto stage_iter = stage_db_table.find(_stage_num);
    eosio_assert(stage_iter != stage_db_table.end(), "not exist stage number");

    bool change = false;
    stage_db_table.modify(stage_iter, owner, [&](auto &new_monster)
    {
        for (uint32_t i = 0; i < stage_iter->enemy_list.size(); ++i)
        {
            if (stage_iter->enemy_list[i].id == _id)
            {
                new_monster.enemy_list.erase( new_monster.enemy_list.begin() + i);
                change=true;
                break;
            }
        }
        eosio_assert(change == true ,"not exist monster id");
    });

}

uint32_t untpreregist::get_attack(uint32_t _job, status_info _status)
    {
        uint32_t attack = 0;
        if ( _job == job_list::warrior || _job == job_list::beginner )
        {
            attack = (_status.basic_str + _status.plus_str) * oper_attack;
            return attack;
        }
        else if ( _job == job_list::archer || _job == job_list::thief )
        {
            attack = (_status.basic_dex + _status.plus_dex) * oper_attack;
            return attack;
        }
        else if ( _job == job_list::wizard || _job == job_list::priest)
        {
            attack = (_status.basic_int + _status.plus_int) * oper_attack;
            return attack;
        }
        else
        {
            return attack;
        }
    }

uint32_t untpreregist::get_speed(uint32_t _job)
    {
        if ( _job == job_list::warrior )
        {
            return warrior_speed;
        }
        else if ( _job == job_list::archer )
        {
            return archer_speed;
        }
        else if ( _job == job_list::wizard )
        {
            return wizard_speed;
        }
        else if ( _job == job_list::priest )
        {
           return priest_speed;
        }
        else if ( _job == job_list::beginner )
        {
            return beginner_speed;
        }
        else if ( _job == job_list::thief )
        {
            return thief_speed;
        }
        else
        {
            return 0;
        }
    }

 uint64_t untpreregist::get_damage(uint32_t _atk, uint32_t _dfs)
    {
        uint32_t damage = (_atk * ((defense_constant * decimal) / (defense_constant + _dfs))) / decimal;
        return damage;
    }

    // uint32_t unlimitgacha::get_buff_turn(uint32_t _buff)
    // {
    //     if( _buff == battle_buff_state::poison )
    //     {
    //         return 4;
    //     }
    //     else if( _buff == battle_buff_state::strength_collect_wait )
    //     {
    //         return 1;
    //     }
    //     else
    //     {
    //         return 0;
    //     }
    // }

 ACTION untpreregist::startbattle(eosio::name _user, uint8_t _party_number, uint8_t _stage)
    {
        require_auth(_user);
#pragma region stage test
        stage_db stage_db_table(owner, owner.value);
        const auto &stage_iter = stage_db_table.get(_stage, "not exist stage info");
#pragma endregion

        auth_users user_auth_table(owner, owner.value);
        auto user_auth_iter = user_auth_table.find(_user.value);
        eosio_assert(user_auth_iter != user_auth_table.end(),"not exist user info");
        eosio_assert(user_auth_iter->state != euser_state::travel && user_auth_iter->state != euser_state::tower, "playing battle....");

        user_auth_table.modify(user_auth_iter, owner, [&](auto &user_state_change) {
                user_state_change.state = euser_state::travel;
        });

        user_partys user_party_table(owner, _user.value);
        const auto &user_party_iter = user_party_table.get(_party_number, "not exist user party data");
        eosio_assert(user_party_iter.state != party_state::on_tower_defense,"this party impossible battle");

        user_servants user_servant_table(owner,_user.value);
        user_monsters user_monster_table(owner,_user.value); 
       
        battle_state_list user_battle_table(owner, owner.value);
        

        auto user_battle_iter = user_battle_table.find(_user.value);
        if (user_battle_iter == user_battle_table.end())
        {
            user_battle_table.emplace(owner, [&](auto &new_battle_set) {
                new_battle_set.user = _user;
                new_battle_set.turn = START_BATTLE;
                //new_battle_set.preference = 0;
                //new_battle_set.reward_list.clear();
    
                uint32_t randering_location = battle_location_list[0];
                new_battle_set.user_battle_state_list[0].now_hp = (user_auth_iter->hero.status.basic_str + user_auth_iter->hero.status.plus_str) * oper_hp;
                new_battle_set.user_battle_state_list[0].defense = (user_auth_iter->hero.status.basic_dex + user_auth_iter->hero.status.plus_dex) * oper_defense;
                new_battle_set.user_battle_state_list[0].crit_per = (user_auth_iter->hero.status.basic_int + user_auth_iter->hero.status.plus_int) * oper_critical;
                new_battle_set.user_battle_state_list[0].attack = get_attack(user_auth_iter->hero.job, user_auth_iter->hero.status);
                new_battle_set.user_battle_state_list[0].speed = get_speed(user_auth_iter->hero.job);
                new_battle_set.user_battle_state_list[0].index = 0;
                new_battle_set.user_battle_state_list[0].state = battle_action_state::wait;

                for(uint32_t i = 1; i < max_servant_slot; ++i)
                {
                    if (user_party_iter.party[i] == 0) //파티 멤버가 비어있으면
                    {
                        continue;
                    }
                    const auto &user_servant_iter = user_servant_table.get(user_party_iter.party[i], "not exist servant data");

                    randering_location = battle_location_list[i];
                    new_battle_set.user_battle_state_list[randering_location].now_hp = ( user_servant_iter.servant.status.basic_str + user_servant_iter.servant.status.plus_str )  * oper_hp;
                    new_battle_set.user_battle_state_list[randering_location].defense = ( user_servant_iter.servant.status.basic_dex + user_servant_iter.servant.status.plus_dex ) * oper_defense;
                    new_battle_set.user_battle_state_list[randering_location].crit_per = ( user_servant_iter.servant.status.basic_int + user_servant_iter.servant.status.plus_int ) * oper_critical;
                    //new_battle_set.user_battle_state_list[randering_location].attack = get_attack(user_servant_iter.servant.job, user_servant_iter.servant.status);
                    //new_battle_set.user_battle_state_list[randering_location].speed = get_speed(user_servant_iter.servant.job);
                    new_battle_set.user_battle_state_list[randering_location].index = user_servant_iter.index;
                    new_battle_set.user_battle_state_list[randering_location].state = battle_action_state::wait;
                }
                for (uint32_t i = max_servant_slot; i < max_monster_slot; ++i)
                {
                    if (user_party_iter.party[i] == 0) //파티 멤버가 비어있으면
                    {
                        continue;
                    }
                    const auto &user_monster_iter = user_monster_table.get(user_party_iter.party[i], "not exist monster data");

                    randering_location = battle_location_list[i];
                    new_battle_set.user_battle_state_list[randering_location].now_hp = (user_monster_iter.monster.status.basic_str + user_monster_iter.monster.status.plus_str) * oper_hp;
                    new_battle_set.user_battle_state_list[randering_location].defense = (user_monster_iter.monster.status.basic_dex + user_monster_iter.monster.status.plus_dex) * oper_defense;
                    new_battle_set.user_battle_state_list[randering_location].crit_per = (user_monster_iter.monster.status.basic_int + user_monster_iter.monster.status.plus_int) * oper_critical;
                    //new_battle_set.user_battle_state_list[randering_location].attack = get_attack(beginner, user_monster_iter.monster.status);
                   // new_battle_set.user_battle_state_list[randering_location].speed = get_speed(beginner);
                    new_battle_set.user_battle_state_list[randering_location].index = user_monster_iter.index;
                    new_battle_set.user_battle_state_list[randering_location].state = battle_action_state::wait;
                }

                //enemy info setting
                for (uint32_t i = 0; i < 20; ++i)
                {
                    new_battle_set.user_battle_state_list[i+10].now_hp = stage_iter.enemy_list[i].status.basic_str * oper_hp;
                    new_battle_set.user_battle_state_list[i+10].defense = stage_iter.enemy_list[i].status.basic_dex * oper_defense;
                    new_battle_set.user_battle_state_list[i+10].crit_per = stage_iter.enemy_list[i].status.basic_int * oper_critical;
                    new_battle_set.user_battle_state_list[i+10].attack = stage_iter.enemy_list[i].status.basic_str * oper_attack;
                    new_battle_set.user_battle_state_list[i+10].speed = beginner_speed;
                    new_battle_set.user_battle_state_list[i+10].index = stage_iter.enemy_list[i].id;
                    new_battle_set.user_battle_state_list[i+10].state = battle_action_state::wait;
                }
            });
        }
        else
        {
            user_battle_table.modify(user_battle_iter, owner, [&](auto &new_battle_set) {
               new_battle_set.user = _user;
                new_battle_set.turn = START_BATTLE;
                //new_battle_set.preference = 0;
                //new_battle_set.reward_list.clear();
    
                uint32_t randering_location = battle_location_list[0];
                new_battle_set.user_battle_state_list[0].now_hp = (user_auth_iter->hero.status.basic_str + user_auth_iter->hero.status.plus_str) * oper_hp;
                new_battle_set.user_battle_state_list[0].defense = (user_auth_iter->hero.status.basic_dex + user_auth_iter->hero.status.plus_dex) * oper_defense;
                new_battle_set.user_battle_state_list[0].crit_per = (user_auth_iter->hero.status.basic_int + user_auth_iter->hero.status.plus_int) * oper_critical;
                //new_battle_set.user_battle_state_list[0].attack = get_attack(user_auth_iter->hero.job, user_auth_iter->hero.status);
              //  new_battle_set.user_battle_state_list[0].speed = get_speed(user_auth_iter->hero.job);
                new_battle_set.user_battle_state_list[0].index = 0;
                new_battle_set.user_battle_state_list[0].state = battle_action_state::wait;

                for(uint32_t i = 1; i < max_servant_slot; ++i)
                {
                    const auto &user_servant_iter = user_servant_table.get(user_party_iter.party[i], "not exist servant data");

                    randering_location = battle_location_list[i];
                    new_battle_set.user_battle_state_list[randering_location].now_hp = ( user_servant_iter.servant.status.basic_str + user_servant_iter.servant.status.plus_str )  * oper_hp;
                    new_battle_set.user_battle_state_list[randering_location].defense = ( user_servant_iter.servant.status.basic_dex + user_servant_iter.servant.status.plus_dex ) * oper_defense;
                    new_battle_set.user_battle_state_list[randering_location].crit_per = ( user_servant_iter.servant.status.basic_int + user_servant_iter.servant.status.plus_int ) * oper_critical;
                    //new_battle_set.user_battle_state_list[randering_location].attack = get_attack(user_servant_iter.servant.job, user_servant_iter.servant.status);
                    //new_battle_set.user_battle_state_list[randering_location].speed = get_speed(user_servant_iter.servant.job);
                    new_battle_set.user_battle_state_list[randering_location].index = user_servant_iter.index;
                    new_battle_set.user_battle_state_list[randering_location].state = battle_action_state::wait;
                }
                for (uint32_t i = max_servant_slot; i < max_monster_slot; ++i)
                {
                    const auto &user_monster_iter = user_monster_table.get(user_party_iter.party[i], "not exist monster data");

                    uint32_t randering_location = battle_location_list[i];
                    new_battle_set.user_battle_state_list[randering_location].now_hp = (user_monster_iter.monster.status.basic_str + user_monster_iter.monster.status.plus_str) * oper_hp;
                    new_battle_set.user_battle_state_list[randering_location].defense = (user_monster_iter.monster.status.basic_dex + user_monster_iter.monster.status.plus_dex) * oper_defense;
                    new_battle_set.user_battle_state_list[randering_location].crit_per = (user_monster_iter.monster.status.basic_int + user_monster_iter.monster.status.plus_int) * oper_critical;
                    //new_battle_set.user_battle_state_list[randering_location].attack = get_attack(beginner, user_monster_iter.monster.status);
                    //new_battle_set.user_battle_state_list[randering_location].speed = get_speed(beginner);
                    new_battle_set.user_battle_state_list[randering_location].index = user_monster_iter.index;
                    new_battle_set.user_battle_state_list[randering_location].state = battle_action_state::wait;
                }

                //enemy info setting
                for (uint32_t i = 0; i < 20; ++i)
                {
                    new_battle_set.user_battle_state_list[i + 10].now_hp = stage_iter.enemy_list[i].status.basic_str * oper_hp;
                    new_battle_set.user_battle_state_list[i+10].defense = stage_iter.enemy_list[i].status.basic_dex * oper_defense;
                    new_battle_set.user_battle_state_list[i+10].crit_per = stage_iter.enemy_list[i].status.basic_int * oper_critical;
                    new_battle_set.user_battle_state_list[i+10].attack = stage_iter.enemy_list[i].status.basic_str * oper_attack;
                    new_battle_set.user_battle_state_list[i+10].speed = beginner_speed;
                    new_battle_set.user_battle_state_list[i+10].index = stage_iter.enemy_list[i].id;
                    new_battle_set.user_battle_state_list[i+10].state = battle_action_state::wait;
                }
            });
        }
    }

#pragma endregion

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#pragma region active turn 

void untpreregist::active_turn(eosio::name _user, uint8_t _hero_action, uint8_t _monster_action, uint8_t _hero_target, uint8_t _monster_target)
    {
        // require_auth(_user);

        // auth_users user_auth_table(owner, owner.value);
        // auto user_auth_iter = user_auth_table.find(_user.value);

        // eosio_assert(user_auth_iter->state == euser_state::travel || user_auth_iter->state == euser_state::tower, "already over battle");

        // uint32_t user_dead_count = 0;
        // uint32_t enemy_dead_count = 0;
        // uint64_t seed = safeseed::get_seed(owner, _user);

        // battle_infos battle_infos_table(owner,owner.value);
        // auto user_battle_iter = battle_infos_table.find(_user);

        // eosio_assert(user_battle_iter != battle_infos_table.end(),"not setting battle data");
        // battle_infos_table.modify(user_battle_iter, owner, [&](auto &battle_state) {
        //     uint32_t battle_preference = safeseed::get_random_battle_value(seed, 2, 0, 0);
        //     //battle_state.preference = battle_preference;
        //     battle_state.turn += 1;

        //     std::vector<attack_speed> temp_order_list;
        //     temp_order_list.resize(20);

        //     for (uint32_t i = 0; i < max_party_count; ++i)
        //     {
        //         temp_order_list[i].member_array_index = i;
        //         temp_order_list[i].member_speed = user_battle_iter->my_party_status_list[i].speed;
        //     }

        //     for (uint32_t i = 0; i < max_party_count; ++i)
        //     {
        //         temp_order_list[i + max_party_count].member_array_index = i + max_party_count;
        //         temp_order_list[i + max_party_count].member_speed = user_battle_iter->enemy_party_status_lzist[i].speed;
        //     }
 
        //     if (battle_preference == user)
        //     {
        //         std::sort(temp_order_list.begin(), temp_order_list.end(),
        //                   [&](attack_speed a, attack_speed b) {
        //                       return a.member_array_index < b.member_array_index;
        //                   });
        //     }
        //     else if (battle_preference == enemy)
        //     {
        //         std::sort(temp_order_list.begin(), temp_order_list.end(),
        //                   [&](attack_speed a, attack_speed b) {
        //                       return a.member_array_index > b.member_array_index;
        //                   });
        //     }
        //     std::sort(temp_order_list.begin(), temp_order_list.end(),
        //               [&](attack_speed a, attack_speed b) {
        //                   return a.member_speed > b.member_speed;
        //               });


        //     uint32_t user_action;
        //     for (uint32_t i = 0; i < max_battle_member_count; ++i)
        //     {
        //         battle_state.attack_order_list[i] = temp_order_list[i].member_array_index;

        //         uint32_t index = temp_order_list[i].member_array_index;
        //         if (index < max_party_count)         //자기 파티에 대한 처리
        //         {
        //             if(battle_state.my_party_status_list[index].now_hp == 0)
        //             {
        //                 continue;
        //             }
        //             if (index == battle_location_list[0])
        //             {
        //                 eosio_assert(battle_state.enemy_party_status_list[_hero_target].now_hp != 0 , "hero target is dead");
        //                 battle_state.my_party_status_list[index].state = _hero_action;
        //                 if (_hero_action == battle_action_state::attack)
        //                 {
        //                     uint32_t damage = get_damage(battle_state.my_party_status_list[index].attack, battle_state.enemy_party_status_list[_hero_target].defense);

        //                     battle_state.my_party_status_list[index].target = _hero_target;

        //                     if (battle_state.enemy_party_status_list[_hero_target].now_hp <= damage)
        //                     {
        //                         battle_state.enemy_party_status_list[_hero_target].now_hp = 0;
        //                     }
        //                     else
        //                     {
        //                         battle_state.enemy_party_status_list[_hero_target].now_hp -= damage;
        //                     }
        //                 }
        //                 else if ( _hero_action == battle_action_state::defense)
        //                 {
        //                     battle_state.my_party_status_list[index].now_hp += 2;
        //                 }
        //                 continue;
        //             }
        //             else if (index == battle_location_list[party_controller.hero_partner_monster_slot])
        //             {
        //                 eosio_assert(battle_state.enemy_party_status_list[_monster_target].now_hp != 0 , "monster target is dead");
        //                 battle_state.my_party_status_list[index].state = _monster_action;
        //                 if (_monster_action == battle_action_state::attack)
        //                 {
        //                     uint32_t damage = get_damage(battle_state.my_party_status_list[index].attack, battle_state.enemy_party_status_list[_monster_target].defense);

        //                     battle_state.my_party_status_list[index].target = _monster_target;
        //                     if (battle_state.enemy_party_status_list[_monster_target].now_hp <= damage)
        //                     {
        //                         battle_state.enemy_party_status_list[_monster_target].now_hp = 0;
        //                     }
        //                     else
        //                     {
        //                         battle_state.enemy_party_status_list[_monster_target].now_hp -= damage;
        //                     }
        //                 }
        //                 else if(_monster_action == battle_action_state::defense)
        //                 {
        //                     battle_state.my_party_status_list[index].now_hp += 2;
        //                 }
        //                 continue;
        //             }

        //             user_action = safeseed::get_random_battle_value(seed, battle_action_state::state_count, battle_action_state::attack, i);
        //             for (uint32_t enemy = 0; enemy < max_party_count; ++enemy)
        //             {
        //                 if (battle_state.enemy_party_status_list[enemy].now_hp == 0)
        //                 {
        //                     continue;
        //                 }
        //                 if (user_action == battle_action_state::attack)
        //                 {
        //                     battle_state.my_party_status_list[index].state = battle_action_state::attack;
        //                     battle_state.my_party_status_list[index].target = enemy;

        //                     uint32_t damage = get_damage(battle_state.my_party_status_list[index].attack, battle_state.enemy_party_status_list[enemy].defense);;
        //                     if (battle_state.enemy_party_status_list[enemy].now_hp <= damage)
        //                     {
        //                         battle_state.enemy_party_status_list[enemy].now_hp = 0;
        //                     }
        //                     else
        //                     {
        //                         battle_state.enemy_party_status_list[enemy].now_hp -= damage;
        //                     }
        //                     break;
        //                 }
        //                 else if (user_action == battle_action_state::defense)
        //                 {
        //                     battle_state.my_party_status_list[index].now_hp += 2;
        //                     break;
        //                 }
        //             }
        //         }
        //         else if(index >= 10) 
        //         {
        //             index = index - 10;
        //             if (battle_state.enemy_party_status_list[index].now_hp == 0)
        //             {
        //                 continue;
        //             }

        //             user_action = safeseed::get_random_battle_value(seed, battle_action_state::state_count, battle_action_state::attack, i);
        //             for (uint32_t enemy = 0; enemy < max_party_count; ++enemy)
        //             {
        //                 if (battle_state.my_party_status_list[enemy].now_hp == 0)
        //                 {
        //                     continue;
        //                 }
        //                 if (user_action == battle_action_state::attack)
        //                 {
        //                     battle_state.enemy_party_status_list[index].state = battle_action_state::attack;
        //                     battle_state.enemy_party_status_list[index].target = enemy;

        //                     uint32_t damage = get_damage(battle_state.enemy_party_status_list[index].attack, battle_state.my_party_status_list[enemy].defense);;
        //                     if (battle_state.my_party_status_list[enemy].now_hp <= damage)
        //                     {
        //                         battle_state.my_party_status_list[enemy].now_hp = 0;
        //                     }
        //                     else
        //                     {
        //                         battle_state.my_party_status_list[enemy].now_hp -= damage;
        //                     }
        //                     break;
        //                 }
        //                 else if (user_action == battle_action_state::defense)
        //                 {
        //                     battle_state.enemy_party_status_list[index].now_hp += 2;
        //                     break;
        //                 }
        //             }
        //         }
        //     }
        //     //게임의 종료 여부 체크 
        //     for (uint32_t i = 0; i < max_battle_member_count; ++i)
        //     {
        //         if (i < max_party_count)
        //         {
        //             if (battle_state.my_party_status_list[i].now_hp == 0)
        //             {
        //                 user_dead_count += 1;
        //             }
        //         }
        //         else
        //         {
        //             if (battle_state.enemy_party_status_list[i - 10].now_hp == 0)
        //             {
        //                 enemy_dead_count += 1;
        //             }
        //         }
        //     }
        // });
        // if (enemy_dead_count == 10)
        // {
        //     win_reward(_user);
        // }
        // else if (user_dead_count == 10)
        // {
        //     fail_reward(_user);
        // }
    }

#pragma endregion

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#pragma region blacklist action

ACTION untpreregist::deleteblack(eosio::name _user)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();

    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    blacklist blacklist_table(owner, owner.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter != blacklist_table.end(), "user not black list");

    blacklist_table.erase(blacklist_iter);
}

ACTION untpreregist::addblack(eosio::name _user)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();

    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    blacklist blacklist_table(owner, owner.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "black list user4");

    blacklist_table.emplace(owner, [&](auto &new_black_user) {
        new_black_user.user = _user;
    });
}

#pragma endregion

#pragma region owenr action

ACTION untpreregist::setpause(uint64_t _state)
{
    master master_table(owner, owner.value);
    auto master_iter = master_table.begin();

    permission_level master_auth;
    master_auth.actor = master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    auth_users user_auth_table(owner, owner.value);
    auto owner_iter = user_auth_table.find(master_iter->master.value);
    eosio_assert(owner_iter != user_auth_table.end(), "not set master7");

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
                execute_action(name(receiver), name(code), &untpreregist::eostransfer);     \
            }                                                                               \
        }                                                                                   \
    }
// eos 금액에 대해 체크 하는 함

EOSIO_DISPATCH(untpreregist, (create)(issue)(transfer)(setmaster)(setpreregist)(preregistmov)(eostransfer)(initmaster)(deleteuser)(initprelog)(inittoken)(deleteblack)(addblack)(setpause)(dbinsert)(dbmodify)(dberase)(dbinit)(startbattle))