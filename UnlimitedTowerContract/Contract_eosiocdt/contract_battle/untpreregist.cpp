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

ACTION untpreregist::create(eosio::name issuer, asset maximum_supply)
{
    require_auth(owner_auth);

    auto sym = maximum_supply.symbol;
    eosio_assert(sym.is_valid(), "invalid symbol name");
    eosio_assert(maximum_supply.is_valid(), "invalid Supply");

    eosio_assert(maximum_supply.amount > 0, "max supply more than 0");

    stats statstable(_self, sym.code().raw());
    auto existing = statstable.find(sym.code().raw());
    eosio_assert(existing == statstable.end(), "token symbol already exists");

    statstable.emplace(_self, [&](auto &s) {
        s.supply.symbol = maximum_supply.symbol;
        s.max_supply = maximum_supply;
        s.issuer = issuer;
    });
}

ACTION untpreregist::issue(eosio::name to, asset quantity, string memo)
{
    auto sym = quantity.symbol;
    eosio_assert(sym.is_valid(), "Invalid symbol name");
    eosio_assert(memo.size() <= 256, "Memo has more than 256 bytes");

    stats statstable(_self, sym.code().raw());
    auto existing = statstable.find(sym.code().raw());
    eosio_assert(existing != statstable.end(), "Token with symbol does now exist, Create token before issue");
    const auto &st = *existing;

    require_auth(owner_auth);
    eosio_assert(quantity.is_valid(), "Invalid quantity");
    eosio_assert(quantity.amount > 0, "Must issue positive quantity");

    eosio_assert(quantity.symbol == st.supply.symbol, "Symbol precision mismatch");
    eosio_assert(quantity.amount <= st.max_supply.amount - st.supply.amount, "Quantity exceeds available supply");

    statstable.modify(st, same_payer, [&](auto &s) {
        s.supply += quantity;
    });

    add_balance(st.issuer, quantity, st.issuer);

    if (to != st.issuer)
    {
        action(permission_level{st.issuer, "active"_n},
               st.issuer, "transfer"_n,
               std::make_tuple(st.issuer, to, quantity, memo))
            .send();
    }
}

ACTION untpreregist::transfer(name from, name to, asset quantity, string memo)
{
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(from.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "black list user1");

    eosio_assert(from != to, "Cannot transfer to self");
    require_auth(from);
    eosio_assert(is_account(to), "To account does not exist");
    auto sym = quantity.symbol.code().raw();
    stats statstable(_self, sym);
    const auto &st = statstable.get(sym, "Not exist symbol");

    require_recipient(from);
    require_recipient(to);

    eosio_assert(quantity.is_valid(), "Invalid quantity");
    eosio_assert(quantity.amount > 0, "Must transfer positive quantity");
    eosio_assert(quantity.symbol == st.supply.symbol, "Symbol precision mismatch");
    eosio_assert(memo.size() <= 256, "Memo has more than 256 bytes");

    sub_balance(from, quantity);
    add_balance(to, quantity, from);
}

void untpreregist::sub_balance(name user, asset value)
{
    account from_acnts(_self, user.value);

    const auto &from = from_acnts.get(value.symbol.code().raw(), "No balance object found");
    eosio_assert(from.balance.amount >= value.amount, "over account balance");

    if (from.balance.amount == value.amount)
    {
        from_acnts.erase(from);
    }
    else
    {
        from_acnts.modify(from, _self, [&](auto &a) {
            a.balance -= value;
        });
    }
}

void untpreregist::add_balance(name user, asset value, name ram_payer)
{
    account to_acnts(_self, user.value);
    auto to = to_acnts.find(value.symbol.code().raw());
    if (to == to_acnts.end())
    {
        to_acnts.emplace(ram_payer, [&](auto &a) {
            a.balance = value;
        });
    }
    else
    {
        to_acnts.modify(to, same_payer, [&](auto &a) {
            a.balance += value;
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
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 1");

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
    servant_job_db servant_job_db_table(_self, _self.value);
    servant_job_db_table.emplace(_self, [&](auto &new_job) {
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
    body_db body_db_table(_self, _self.value);
    body_db_table.emplace(_self, [&](auto &new_body) {
        new_body.body = _appear;
    });
}

void untpreregist::insert_hair(uint32_t _appear)
{
    hair_db hair_db_table(_self, _self.value);
    hair_db_table.emplace(_self, [&](auto &new_hair) {
        new_hair.hair = _appear;
    });
}
void untpreregist::insert_head(uint32_t _appear)
{
    head_db head_db_table(_self, _self.value);
    head_db_table.emplace(_self, [&](auto &new_head) {
        new_head.head = _appear;
    });
}

void untpreregist::insert_gender(uint32_t _appear)
{
    gender_db gender_db_table(_self, _self.value);
    gender_db_table.emplace(_self, [&](auto &new_gender) {
        new_gender.gender = _appear;
    });
}

void untpreregist::insert_servant_id(uint32_t _id, uint32_t _index)
{
    servant_id_db servant_id_db_table(_self, _self.value);
    servant_id_db_table.emplace(_self, [&](auto &new_servant_id) {
        new_servant_id.id = _id;
        new_servant_id.index = _index;
    });
}

void untpreregist::insert_monster_id(uint32_t _id)
{
    monster_id_db monster_id_db_table(_self, _self.value);
    monster_id_db_table.emplace(_self, [&](auto &new_monster_id) {
        new_monster_id.id = _id;
    });
}

void untpreregist::insert_monster_grade(uint32_t _grade, uint32_t _min, uint32_t _max)
{
    monster_grade_db monster_grade_db_table(_self, _self.value);
    monster_grade_db_table.emplace(_self, [&](auto &new_monster_grade) {
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
    item_id_db item_id_db_table(_self, _self.value);
    item_id_db_table.emplace(_self, [&](auto &new_item_id) {
        new_item_id.id = _id;
        new_item_id.type = _type;
        new_item_id.job = _job;
        new_item_id.tier = _tier;
    });
}

void untpreregist::insert_item_grade(uint32_t _grade, uint32_t _min, uint32_t _max)
{
    item_grade_db item_grade_db_table(_self, _self.value);
    item_grade_db_table.emplace(_self, [&](auto &new_item_grade) {
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
    grade_ratio_db grade_ratio_db_table(_self, _self.value);
    grade_ratio_db_table.emplace(_self, [&](auto &new_grade_ratio)
    {
        new_grade_ratio.grade = _grade;
        new_grade_ratio.ratio = _ratio;
    });
}

ACTION untpreregist::dbmodify(uint32_t _kind, uint32_t _appear, uint32_t _id, uint32_t _index, uint32_t _job, uint32_t _tier, uint32_t _type, uint32_t _grade, uint32_t _min, uint32_t _max, uint32_t _ratio)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 2");

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
    servant_job_db servant_job_db_table(_self, _self.value);
    auto job_iter = servant_job_db_table.find(_job);
    eosio_assert(job_iter != servant_job_db_table.end(), "Not Exist Job Data");
    servant_job_db_table.modify(job_iter, _self, [&](auto &modify_job) {
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
    body_db body_db_table(_self, _self.value);
    auto body_iter = body_db_table.find(_appear);
    eosio_assert(body_iter != body_db_table.end(), "Not Find Body Data");
    body_db_table.modify(body_iter, _self, [&](auto &modify_body) {
        modify_body.body = _appear;
    });
}

void untpreregist::modify_hair(uint32_t _appear)
{
    hair_db hair_db_table(_self, _self.value);
    auto hair_iter = hair_db_table.find(_appear);
    eosio_assert(hair_iter != hair_db_table.end(), "Not Find Hair Data");
    hair_db_table.modify(hair_iter, _self, [&](auto &modify_hair) {
        modify_hair.hair = _appear;
    });
}

void untpreregist::modify_head(uint32_t _appear)
{
    head_db head_db_table(_self, _self.value);
    auto head_iter = head_db_table.find(_appear);
    eosio_assert(head_iter != head_db_table.end(), "Not Find Head Data");
    head_db_table.modify(head_iter, _self, [&](auto &modify_head) {
        modify_head.head = _appear;
    });
}

void untpreregist::modify_gender(uint32_t _appear)
{
    gender_db gender_db_table(_self, _self.value);
    auto gender_iter = gender_db_table.find(_appear);
    eosio_assert(gender_iter != gender_db_table.end(), "Not Find Gender Data");
    gender_db_table.modify(gender_iter, _self, [&](auto &modify_gender) {
        modify_gender.gender = _appear;
    });
}

void untpreregist::modify_servant_id(uint32_t _id, uint32_t _index)
{
    servant_id_db servant_id_db_table(_self, _self.value);
    auto servant_id_iter = servant_id_db_table.find(_id);
    eosio_assert(servant_id_iter != servant_id_db_table.end(), "Not Find Servant ID Data");
    servant_id_db_table.modify(servant_id_iter, _self, [&](auto &modify_servant_id) {
        modify_servant_id.id = _id;
        modify_servant_id.index = _index;
    });
}

void untpreregist::modify_monster_id(uint32_t _id)
{
    monster_id_db monster_id_db_table(_self, _self.value);
    auto monster_id_iter = monster_id_db_table.find(_id);
    eosio_assert(monster_id_iter != monster_id_db_table.end(), "Not Find Monster ID Data");
    monster_id_db_table.modify(monster_id_iter, _self, [&](auto &modify_monster_id) {
        modify_monster_id.id = _id;
    });
}

void untpreregist::modify_monster_grade(uint32_t _grade, uint32_t _min, uint32_t _max)
{
    monster_grade_db monster_grade_db_table(_self, _self.value);
    auto monster_grade_iter = monster_grade_db_table.find(_grade);
    eosio_assert(monster_grade_iter != monster_grade_db_table.end(), "Not Exist Monster Grade");
    monster_grade_db_table.modify(monster_grade_iter, _self, [&](auto &modify_monster_grade) {
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
    item_grade_db item_grade_db_table(_self, _self.value);
    auto item_grade_iter = item_grade_db_table.find(_grade);
    eosio_assert(item_grade_iter != item_grade_db_table.end(), "Not Exist Item Grade Data");
    item_grade_db_table.modify(item_grade_iter, _self, [&](auto &modify_item_grade) {
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
    item_id_db item_id_db_table(_self, _self.value);
    auto item_id_iter = item_id_db_table.find(_id);
    eosio_assert(item_id_iter != item_id_db_table.end(), "Not Exist Item ID");
    item_id_db_table.modify(item_id_iter, _self, [&](auto &modify_item_id) {
        modify_item_id.id = _id;
        modify_item_id.type = _type;
        modify_item_id.job = _job;
        modify_item_id.tier = _tier;
    });
}

void untpreregist::modify_grade_ratio(uint32_t _grade, uint32_t _ratio)
{
    grade_ratio_db grade_ratio_db_table(_self, _self.value);
    auto grade_ratio_db_iter = grade_ratio_db_table.find(_grade);
    eosio_assert( grade_ratio_db_iter != grade_ratio_db_table.end() , "Not Exist Grade Ratio Data" );
    grade_ratio_db_table.modify(grade_ratio_db_iter ,_self, [&](auto &new_grade_ratio) {
        new_grade_ratio.ratio = _ratio;
    });
}


ACTION untpreregist::dberase(uint32_t _kind, uint32_t _appear, uint32_t _id, uint32_t _job, uint32_t _tier, uint32_t _type, uint32_t _grade, uint32_t _min, uint32_t _max)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 3");

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
    servant_job_db servant_job_db_table(_self, _self.value);
    auto job_iter = servant_job_db_table.find(_job);
    eosio_assert(job_iter != servant_job_db_table.end(), "Not Exist Job Data");
    servant_job_db_table.erase(job_iter);
}

void untpreregist::erase_body(uint32_t _appear)
{
    body_db body_db_table(_self, _self.value);
    auto body_iter = body_db_table.find(_appear);
    eosio_assert(body_iter != body_db_table.end(), "Not Find Body Data");
    body_db_table.erase(body_iter);
}

void untpreregist::erase_hair(uint32_t _appear)
{
    hair_db hair_db_table(_self, _self.value);
    auto hair_iter = hair_db_table.find(_appear);
    eosio_assert(hair_iter != hair_db_table.end(), "Not Find Hair Data");
    hair_db_table.erase(hair_iter);
}

void untpreregist::erase_head(uint32_t _appear)
{
    head_db head_db_table(_self, _self.value);
    auto head_iter = head_db_table.find(_appear);
    eosio_assert(head_iter != head_db_table.end(), "Not Find Head Data");
    head_db_table.erase(head_iter);
}

void untpreregist::erase_gender(uint32_t _appear)
{
    gender_db gender_db_table(_self, _self.value);
    auto gender_iter = gender_db_table.find(_appear);
    eosio_assert(gender_iter != gender_db_table.end(), "Not Find Gender Data");
    gender_db_table.erase(gender_iter);
}

void untpreregist::erase_servant_id(uint32_t _id)
{
    servant_id_db servant_id_db_table(_self, _self.value);
    auto servant_id_iter = servant_id_db_table.find(_id);
    eosio_assert(servant_id_iter != servant_id_db_table.end(), "Not Find Servant ID Data");
    servant_id_db_table.erase(servant_id_iter);
}

void untpreregist::erase_monster_id(uint32_t _id)
{
    monster_id_db monster_id_db_table(_self, _self.value);
    auto monster_id_iter = monster_id_db_table.find(_id);
    eosio_assert(monster_id_iter != monster_id_db_table.end(), "Not Find Monster ID Data");
    monster_id_db_table.erase(monster_id_iter);
}

void untpreregist::erase_monster_grade(uint32_t _grade)
{
    monster_grade_db monster_grade_db_table(_self, _self.value);
    auto monster_grade_iter = monster_grade_db_table.find(_grade);
    eosio_assert(monster_grade_iter != monster_grade_db_table.end(), "Not Exist Monster Grade");
    monster_grade_db_table.erase(monster_grade_iter);
}

void untpreregist::erase_item_grade(uint32_t _grade)
{
    item_grade_db item_grade_db_table(_self, _self.value);
    auto item_grade_iter = item_grade_db_table.find(_grade);
    eosio_assert(item_grade_iter != item_grade_db_table.end(), "Not Exist Item Grade Data");
    item_grade_db_table.erase(item_grade_iter);
}

void untpreregist::erase_item_id(uint32_t _id)
{
    item_id_db item_id_db_table(_self, _self.value);
    auto item_id_iter = item_id_db_table.find(_id);
    eosio_assert(item_id_iter != item_id_db_table.end(), "Not Exist Item ID 2");
    item_id_db_table.erase(item_id_iter);
}

void untpreregist::erase_grade_ratio(uint32_t _grade)
{
    grade_ratio_db grade_ratio_db_table(_self, _self.value);
    auto grade_ratio_db_iter = grade_ratio_db_table.find(_grade);
    eosio_assert( grade_ratio_db_iter != grade_ratio_db_table.end() , "Not Exist Grade Ratio Data" );
    grade_ratio_db_table.erase(grade_ratio_db_iter);
}

ACTION untpreregist::dbinit()
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "owner"_n;
    require_auth(master_auth);

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 4");

    head_db head_db_table(_self, _self.value);
    hair_db hair_db_table(_self, _self.value);
    body_db body_db_table(_self, _self.value);
    monster_grade_db monster_grade_db_table(_self, _self.value);
    monster_id_db monster_id_db_table(_self, _self.value);
    item_id_db item_id_db_table(_self, _self.value);
    item_grade_db item_grade_db_table(_self, _self.value);

    gender_db gender_db_table(_self, _self.value);
    servant_job_db servant_job_db_table(_self, _self.value);
    servant_id_db servant_id_db_table(_self, _self.value);

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
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    if (system_master_iter == system_master_table.end())
    {
        require_auth(owner_auth);

        system_master_table.emplace(_self, [&](auto &set_master) {
            set_master.master = _self;
            set_master.state = system_state::pause;
        });
    }
    else
    {
        permission_level master_auth;
        master_auth.actor = system_master_iter->master;
        master_auth.permission = "owner"_n;
        require_auth(master_auth);

        total_token_logs total_token_log_table(_self, _self.value);
        auto total_token_log_iter = total_token_log_table.find(system_master_iter->master.value);

        if(total_token_log_iter != total_token_log_table.end())
        {
            total_token_log_table.emplace(_self, [&](auto &move_master)
            {
                move_master.owner = _master;
                move_master.total_token_amount = total_token_log_iter->total_token_amount;
            });
            total_token_log_table.erase(total_token_log_iter);
        }

        system_master_table.emplace(_self, [&](auto &move_master) {
            move_master.master = _master;
            move_master.state = system_state::pause;
        });
        system_master_table.erase(system_master_iter);
    }
}

ACTION untpreregist::settokenlog()
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 5");

    total_token_logs total_token_log_table(_self, _self.value);
    auto total_token_log_iter = total_token_log_table.find(system_master_iter->master.value);
    eosio_assert(total_token_log_iter == total_token_log_table.end(),"Exist Token Log");
    total_token_log_table.emplace(_self, [&](auto &preregist) {
        preregist.owner = system_master_iter->master;
    });
}

#pragma endregion

//------------------------------------------------------------------------//
//-------------------------------login_action-----------------------------//
//------------------------------------------------------------------------//

#pragma region login


void untpreregist::signup(eosio::name _user)
{
    auth_users auth_user_table(_self, _self.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "User Already Signup 3");

    auth_user_table.emplace(_self, [&](auto &new_user) {
        new_user.user = _user;
        new_user.state = user_state::lobby;

        hero_info new_hero;
        new_hero.state = hero_state::set_look;
        new_hero.equip_slot.resize(3);

        new_user.hero = new_hero;
    });

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter == user_log_table.end(), "User Already Signup 4");
    user_log_table.emplace(_self, [&](auto &new_log) {
        new_log.user = _user;
    });

    user_partys user_party_table(_self, _user.value);
    user_party_table.emplace(_self, [&](auto &new_party) {
        uint32_t first_index = user_party_table.available_primary_key();
        if (first_index == 0)
        {
            new_party.index = 1;
        }
        else
        {
            new_party.index = user_party_table.available_primary_key();
        }
        new_party.party.resize(MAX_PARTY_MEMBER);
    });
}

// eosio.token recipient
// memo description spec
//-------------------------------------------------------------------------

ACTION untpreregist::eostransfer(eosio::name sender, eosio::name receiver)
{
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(sender.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "BlackList User 3");

    eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
        eosio_assert(ad.action.size() != 0, "Wrong Action");
        eosio_assert(ad.action != action_signup, "Need Presignup");
        if (ad.action == action_signup)
        {
            signup(sender);
        }
        else if (ad.action == action_gacha)
        {
            start_gacha(sender, ad.type);
        }
    });
}

template <typename T>
void untpreregist::eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func)
{
    require_auth(sender);
    auto transfer_data = eosio::unpack_action_data<st_transfer>();
    eosio_assert(transfer_data.quantity.symbol == symbol("EOS", 4), "Only Accepts EOS for deposits");
    eosio_assert(transfer_data.quantity.is_valid(), "Invalid token transfer");
    eosio_assert(transfer_data.quantity.amount > 0, "Quantity must be positive");

    transfer_action res;
    size_t l_center = transfer_data.memo.find(':');

    res.action = transfer_data.memo.substr(0, l_center);

    if (res.action == "gacha")
    {
        system_master system_master_table(_self, _self.value);
        auto system_master_iter = system_master_table.begin();
        eosio_assert(system_master_iter->state != system_state::pause, "Server Pause 4");

        size_t l_next = transfer_data.memo.find(':', l_center + 1);
        size_t l_end = transfer_data.memo.length() - (l_next + 1);

        eosio_assert(transfer_data.memo.find(':') != std::string::npos, "Seed Memo [:] Error");
        eosio_assert(transfer_data.memo.find(':', l_center + 1) != std::string::npos, "Seed Memo [:] Error");
        eosio_assert(transfer_data.quantity.amount == 10000, "Gacha need 1.0000 EOS"); 

        std::string l_seed = transfer_data.memo.substr(l_center + 1, (l_next - l_center - 1));
        std::string l_sha = transfer_data.memo.substr(l_next + 1, l_end);

        res.type = safeseed::check_seed(l_seed, l_sha);
        res.quantity = transfer_data.quantity;

        eosio_assert(res.type != 0, "Wrong seed convert");
    }
    else if (res.action == "signup")
    {
        system_master system_master_table(_self, _self.value);
        auto system_master_iter = system_master_table.begin();
        eosio_assert(system_master_iter->state != system_state::pause, "Server Pause 6");

        eosio_assert(transfer_data.quantity.amount == 10000, "signup need 1.0000 EOS");
    }
    else
    {
        system_master system_master_table(_self, _self.value);
        if (_self != sender)
        {
            auto system_master_iter = system_master_table.find(sender.value);
            eosio_assert(system_master_iter != system_master_table.end(), "Impossible Send EOS");
        }
        else if (_self == sender)
        {
            auto system_master_iter = system_master_table.find(receiver.value);
            eosio_assert(system_master_iter != system_master_table.end(), "Impossible Recv EOS");
        }
    }

    func(res);
}

#pragma endregion

#pragma resion init db table
ACTION untpreregist::initmaster()
{
    require_auth(owner_auth);
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    total_token_logs total_token_log_table(_self, _self.value);
    auto total_token_log_iter = total_token_log_table.find(system_master_iter->master.value);

    if (total_token_log_iter != total_token_log_table.end())
    {
        total_token_log_table.emplace(_self, [&](auto &move_master) {
            move_master.owner = _self;
            move_master.total_token_amount = total_token_log_iter->total_token_amount;
        });
        total_token_log_table.erase(total_token_log_iter);
    }

    system_master_table.erase(system_master_iter);

    system_master_table.emplace(_self, [&](auto &owner_master)
    {
        owner_master.master = _self;   
        owner_master.state = system_state::pause;
    });
}

#pragma endregion


#pragma reion init all table

ACTION untpreregist::inittokenlog()
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 8");

    total_token_logs total_token_log_table(_self, _self.value);
    auto total_token_iter = total_token_log_table.begin();
    eosio_assert(total_token_iter != total_token_log_table.end(), "Not Exist Token log Data");

    total_token_log_table.erase(total_token_iter);
}

#pragma endregion



//------------------------------------------------------------------------//
//-------------------------------gacha_function---------------------------//
//------------------------------------------------------------------------//
uint32_t untpreregist::get_random_grade(uint64_t _rate)
{
    uint32_t grade;
    grade_ratio_db grade_ratio_db_table(_self, _self.value);
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
    servant_job_db servant_job_table(_self, _self.value);
    uint32_t random_job = safeseed::get_random_value(_seed, SERVANT_JOB_COUNT, DEFAULT_MIN_DB, servant_random_count);
    const auto &servant_job_db_iter = servant_job_table.get(random_job, "Not Get Servant Job Data 1");

    servant_random_count += 1;
    uint32_t random_body = gacha_servant_body(_seed, servant_random_count);

    servant_random_count += 1;
    gender_db gender_table(_self, _self.value);
    uint32_t random_gender = safeseed::get_random_value(_seed, GEMDER_COUNT, DEFAULT_MIN_DB, servant_random_count);
    const auto &gender_db_iter = gender_table.get(random_gender, "Not Get Gender Data 1");

    servant_random_count += 1;
    uint32_t random_head = gacha_servant_head(_seed, servant_random_count);

    servant_random_count += 1;
    uint32_t random_hair = gacha_servant_hair(_seed, servant_random_count);

    servant_id_db servant_id_table(_self, _self.value);
    uint32_t servant_index = get_servant_index(random_job, random_body, random_gender, random_head, random_hair);
    const auto &servant_id_db_iter = servant_id_table.get(servant_index, "Not Exist Servant ID 1");

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Not Exist User Log 3");
 
    result_info result;
    user_servants user_servant_table(_self, _user.value);
    user_servant_table.emplace(_self, [&](auto &update_user_servant_list) {
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

        new_servant.appear.head = random_head;
        new_servant.appear.hair = random_hair;
        new_servant.appear.body = random_body;
        new_servant.appear.gender = random_gender;
        new_servant.job = random_job;

        servant_random_count += 1;
        new_servant.status.basic_str = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_str, servant_job_db_iter.min_range.base_str, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_dex = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_dex, servant_job_db_iter.min_range.base_dex, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_int = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_int, servant_job_db_iter.min_range.base_int, servant_random_count);
        
        new_servant.equip_slot.resize(3);
        new_servant.state = object_state::on_inventory;

        result.index = update_user_servant_list.index;
        result.type = result::servant;

        update_user_servant_list.party_number = EMPTY_PARTY;
        update_user_servant_list.servant = new_servant;
    });

    user_gacha_results user_gacha_result_table(_self, _self.value);
    user_gacha_totals user_gacha_total_table(_self, _self.value);

    auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
    if (user_gacha_result_iter == user_gacha_result_table.end())
    {
        user_gacha_result_table.emplace(_self, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result = result;
        });
    }
    else
    {
        user_gacha_result_table.modify(user_gacha_result_iter, _self, [&](auto &new_result) {
            new_result.result = result;
        });
    }

    auto user_gacha_total_iter = user_gacha_total_table.find(_user.value);
    if (user_gacha_total_iter == user_gacha_total_table.end())
    {
        user_gacha_total_table.emplace(_self, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_total_table.modify(user_gacha_total_iter, _self, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    //로그 남기는 부분
    user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
        update_log.servant_num += 1;
        update_log.gacha_num += 1;
    });
}

uint8_t untpreregist::gacha_servant_head(uint64_t _seed, uint32_t _count)
{
    head_db head_db_table(_self, _self.value);
    uint8_t random_head = safeseed::get_random_value(_seed, HEAD_COUNT, DEFAULT_MIN_DB, _count);
    const auto &head_db_iter = head_db_table.get(random_head, "Not Exist Head Info");
    return head_db_iter.head;
}

uint8_t untpreregist::gacha_servant_hair(uint64_t _seed, uint32_t _count)
{
    hair_db hair_db_table(_self, _self.value);
    uint8_t random_hair = safeseed::get_random_value(_seed, HAIR_COUNT, DEFAULT_MIN_DB, _count);
    const auto &hair_db_iter = hair_db_table.get(random_hair, "Not Exist Hair Info");
    return hair_db_iter.hair;
}

uint8_t untpreregist::gacha_servant_body(uint64_t _seed, uint32_t _count)
{
    body_db body_db_table(_self, _self.value);
    uint8_t random_body = safeseed::get_random_value(_seed, BODY_COUNT, DEFAULT_MIN_DB, _count);
    const auto &body_db_iter = body_db_table.get(random_body, "Not Exist Body Info");
    return body_db_iter.body;
}

void untpreregist::gacha_monster_id(eosio::name _user, uint64_t _seed)
{
    monster_id_db monster_id_db_table(_self, _self.value);
    uint32_t random_monster_id = safeseed::get_random_value(_seed, MONSTER_ID_COUNT, default_min, monster_random_count);
    random_monster_id += 20000;
    const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id, "Not Exist Monster ID 1");

    monster_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, monster_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    monster_grade_db monster_grade_db_table(_self, _self.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "Not Exist Monster Grade 4");

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Not Exist User Log 3");

    result_info result;
    user_monsters user_monster_table(_self, _user.value);
    user_monster_table.emplace(_self, [&](auto &update_user_monster_list) {
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
        new_monster.state = object_state::on_inventory;

        result.index = update_user_monster_list.index;
        result.type = result::monster;

        update_user_monster_list.party_number = EMPTY_PARTY;
        update_user_monster_list.monster = new_monster;
    });

    user_gacha_results user_gacha_result_table(_self, _self.value);
    user_gacha_totals user_gacha_total_table(_self, _self.value);

    auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
    if (user_gacha_result_iter == user_gacha_result_table.end())
    {
        user_gacha_result_table.emplace(_self, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result = result;
        });
    }
    else
    {
        user_gacha_result_table.modify(user_gacha_result_iter, _self, [&](auto &new_result) {
            new_result.result = result;
        });
    }

    auto user_gacha_total_iter = user_gacha_total_table.find(_user.value);
    if (user_gacha_total_iter == user_gacha_total_table.end())
    {
        user_gacha_total_table.emplace(_self, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_total_table.modify(user_gacha_total_iter, _self, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
        update_log.gacha_num += 1;
        update_log.monster_num += 1;
    });
}

void untpreregist::gacha_item_id(eosio::name _user, uint64_t _seed)
{
    item_id_db item_id_db_table(_self, _self.value);
    uint32_t random_item_id = safeseed::get_random_value(_seed, ITEM_ID_COUNT, default_min, item_random_count);
    random_item_id += 30000;
    const auto &item_id_db_iter = item_id_db_table.get(random_item_id, "Not Exist Item ID 1");

    item_random_count += 1;
    uint32_t random_rate = safeseed::get_random_value(_seed, max_rate, default_min, item_random_count);
    uint32_t random_grade = get_random_grade(random_rate);

    item_grade_db item_grade_db_table(_self, _self.value);
    const auto &item_grade_db_iter = item_grade_db_table.get(random_grade, "Not Exist Tier 1");

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Not Exist User Log 5");

    result_info result;
    user_items user_item_table(_self, _user.value);
    user_item_table.emplace(_self, [&](auto &update_user_item_list) {
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
        new_item.state = object_state::on_inventory;

        result.index = update_user_item_list.index;
        result.type = result::item;

        update_user_item_list.item = new_item;
    });

    user_gacha_results user_gacha_result_table(_self, _self.value);
    user_gacha_totals user_gacha_total_table(_self, _self.value);

    auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
    if (user_gacha_result_iter == user_gacha_result_table.end())
    {
        user_gacha_result_table.emplace(_self, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result = result;
        });
    }
    else
    {
        user_gacha_result_table.modify(user_gacha_result_iter, _self, [&](auto &new_result) {
            new_result.result = result;
        });
    }

    auto user_gacha_total_iter = user_gacha_total_table.find(_user.value);
    if (user_gacha_total_iter == user_gacha_total_table.end())
    {
        user_gacha_total_table.emplace(_self, [&](auto &new_result) {
            new_result.user = _user;
            new_result.result_list.push_back(result);
        });
    }
    else
    {
        user_gacha_total_table.modify(user_gacha_total_iter, _self, [&](auto &new_result) {
            new_result.result_list.push_back(result);
        });
    }

    user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
        update_log.item_num += 1;
        update_log.gacha_num += 1;
    });
}
uint64_t untpreregist::get_user_seed_value(uint64_t _user)
{
    total_token_logs total_token_log_table(_self, _self.value);
    auto total_token_iter = total_token_log_table.begin();

    uint64_t user;

    user_logs user_log_table(_self, _self.value);
    const auto &user_log_iter = user_log_table.get(_user, "Not Exist User Log 5");
    user = _user + user_log_iter.gacha_num + now();
    if (user <= _user)
    {
        user = user_log_iter.gacha_num + now();
    }

    return user;
}

void untpreregist::start_gacha(eosio::name _user, uint64_t _seed)
{
    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Not Exist User Log 7");

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


#pragma region blacklist action

ACTION untpreregist::deleteblack(eosio::name _user)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter != blacklist_table.end(), "User Not Black List");

    blacklist_table.erase(blacklist_iter);
}

ACTION untpreregist::addblack(eosio::name _user)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "Black List User 4");

    blacklist_table.emplace(_self, [&](auto &new_black_user) {
        new_black_user.user = _user;
    });
}

#pragma endregion

#pragma region owenr action

ACTION untpreregist::setpause(uint64_t _state)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    system_master_table.modify(system_master_iter, _self, [&](auto &set_owner_account) {
        set_owner_account.state = _state;
    });
}

#pragma endregion

ACTION untpreregist::resultgacha(eosio::name _from, eosio::name _to ,std::string _result)
{
    require_auth(_self);
    require_recipient(_from);
}

ACTION untpreregist::resultpre(eosio::name _from, eosio::name _to ,std::string _result)
{
    require_auth(_self);
    require_recipient(_from);
}

#pragma endresion



#pragma region party function

//------------------------------------------------------------------------//
//-------------------------------party_function---------------------------//
//------------------------------------------------------------------------//

ACTION untpreregist::setparty(eosio::name _user, uint32_t _party_number, const std::vector<uint32_t> &_party_list)
{
    require_auth(_user);
    eosio_assert(_party_number > 0, "wrong party_number");                                                   //잘못된 파티 넘버 체크
    user_partys user_party_table(_self, _user.value);                                                              //
    auto user_party_iter = user_party_table.find(_party_number);                                             //유저 파티 이터레이터는 파티 테이블 번호
    eosio_assert(user_party_iter != user_party_table.end(), "not exist party");                              //유저 파티 이터레이터를 기준으로 테이블 조회 시 없으면
    eosio_assert(user_party_iter->state != party_state::on_tower_defense, "this party on tower unmodified"); //유저 파티 상태가 타워 디펜스중이 아닐시
    user_servants user_servant_table(_self, _user.value);
    user_monsters user_monster_table(_self, _user.value);

    for (uint32_t i = 1; i < MAX_SERVANT_SLOT; ++i) //서번트 (4개) 까지 for문
    {
        if (user_party_iter->party[i] != EMPTY_PARTY &&
            user_party_iter->party[i] != _party_list[i])
        {
            auto user_servant_iter = user_servant_table.find(user_party_iter->party[i]);
            eosio_assert(user_servant_iter != user_servant_table.end(), "fatal party data mis");
            user_servant_table.modify(user_servant_iter, owner, [&](auto &set_party) {
                set_party.party_number = EMPTY_PARTY;
            });
        }
    }
    for (uint32_t i = MAX_SERVANT_SLOT; i < MAX_MONSTER_SLOT; ++i)
    {
        if (user_party_iter->party[i] != EMPTY_PARTY &&
            user_party_iter->party[i] != _party_list[i])
        {
            auto user_monster_iter = user_monster_table.find(user_party_iter->party[i]);
            eosio_assert(user_monster_iter != user_monster_table.end(), "fatal party data mis");
            user_monster_table.modify(user_monster_iter, owner, [&](auto &set_party) {
                set_party.party_number = EMPTY_PARTY;
            });
        }
    }
    user_party_table.modify(user_party_iter, _self, [&](auto &save_party) {
        for (uint32_t i = 1; i < MAX_SERVANT_SLOT; ++i)
        {
            if (_party_list[i] == EMPTY_PARTY)
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
        for (uint32_t i = MAX_SERVANT_SLOT; i < MAX_MONSTER_SLOT; ++i)
        {
            if (_party_list[i] == EMPTY_PARTY)
            {
                save_party.party[i] = _party_list[i];
                continue;
            }
            if (i != PAIR_SLOT)
            {
                eosio_assert(_party_list[i - PAIR_SLOT] != EMPTY_PARTY, "need set servant");
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
    user_logs user_log_table(_self, _self.value);

    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "not find user information to log");
    uint32_t l_p_count = user_log_iter->add_party_count;
    l_p_count++;
    user_log_table.modify(user_log_iter, _self, [&](auto &buy_party_log) {
        buy_party_log.add_party_count = l_p_count;
    });
    user_partys user_party_table(_self, _user.value);
    user_party_table.emplace(_self, [&](auto &new_party) {
        new_party.index = user_party_table.available_primary_key();
        new_party.party.resize(10);
    });
}
#pragma region party cheat
ACTION untpreregist::herocheat(eosio::name _user)
{
        auth_users auth_user_table(_self, _self.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "User Already Signup 3");
    auth_user_table.emplace(_self, [&](auto &new_user) {
        new_user.user = _user;
        new_user.state = user_state::lobby;

        hero_info new_hero;
        new_hero.state = hero_state::set_complete;
        new_hero.appear.body = 1;
        new_hero.appear.head = 1;
        new_hero.appear.hair = 1;
        new_hero.appear.gender = 1;

        new_hero.status.basic_str = 100;
        new_hero.status.basic_dex = 100;
        new_hero.status.basic_int = 100;

        new_hero.equip_slot.resize(3);

        new_user.hero = new_hero;
    });

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter == user_log_table.end(), "User Already Signup 4");
    user_log_table.emplace(_self, [&](auto &new_log) {
        new_log.user = _user;
    });

    user_partys user_party_table(_self, _user.value);
    user_party_table.emplace(_self, [&](auto &new_party) {
        uint32_t first_index = user_party_table.available_primary_key();
        if (first_index == 0)
        {
            new_party.index = 1;
        }
        else
        {
            new_party.index = user_party_table.available_primary_key();
        }
        new_party.party.resize(MAX_PARTY_MEMBER);
    });
}


ACTION untpreregist::partycheat(eosio::name _user)
{
    uint64_t user_value = get_user_seed_value(_user.value);
    uint64_t seed = safeseed::get_seed_value(user_value, now());

    for(uint32_t i = 0; i < 4; ++i)
    {
        gacha_servant_id(_user, seed);
    }
    for(uint32_t i = 0; i < 5; ++i)
    {
        gacha_monster_id(_user, seed);
    }

    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.find(1);
    if (user_party_iter == user_party_table.end())
    {
        user_party_table.emplace(_self, [&](auto &automatic_party) {
            automatic_party.index = 1;
            for (uint32_t i = 1; i < 10; ++i)
            {
                if (i < 5)
                {
                    automatic_party.party[i] = i;
                }
                else
                {
                    uint32_t monster_index = i - 4;
                    automatic_party.party[i] = monster_index;
                }
            }
        });
    }
    else
    {
        user_party_table.modify(user_party_iter, _self, [&](auto &automatic_party) {
            for (uint32_t i = 1; i < 10; ++i)
            {
                if (i < 5)
                {
                    automatic_party.party[i] = i;
                }
                else
                {
                    uint32_t monster_index = i - 4;
                    automatic_party.party[i] = monster_index;
                }
            }
        });
    }
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
   user_servants user_servant_table(_self, _user.value);
   auto user_servant_iter = user_servant_table.find(_index);
   eosio_assert(user_servant_iter != user_servant_table.end(),"not exist servant info");
   eosio_assert(user_servant_iter->party_number == 0,"this servant already in party");

   asset servant_sell_result(0, symbol(symbol_code("UTG"), 4));
   servant_sell_result.amount = 10000000;

   action(permission_level{get_self(), "active"_n},
          get_self(), "tokentrans"_n,
          std::make_tuple(_self, _user, servant_sell_result, std::string("servant sell result")))
       .send();

   user_servant_table.erase(user_servant_iter);
}

void untpreregist::sell_monster(eosio::name _user, uint32_t _index)
{
   user_monsters user_monster_table(_self, _user.value);
   auto user_monster_iter = user_monster_table.find(_index);
   eosio_assert(user_monster_iter != user_monster_table.end(),"not exist servant info");
   eosio_assert(user_monster_iter->party_number == 0,"this servant already in party");

   asset monster_sell_result(0, symbol(symbol_code("UTG"), 4));
   monster_sell_result.amount = 10000000;

   action(permission_level{get_self(), "active"_n},
          get_self(), "tokentrans"_n,
          std::make_tuple(_self, _user, monster_sell_result, std::string("monster sell result")))
       .send();

   user_monster_table.erase(user_monster_iter);
}

void untpreregist::sell_item(eosio::name _user, uint32_t _index)
{
   user_items user_item_table(_self, _user.value);
   auto user_item_iter = user_item_table.find(_index);
   eosio_assert(user_item_iter != user_item_table.end(),"not exist item info");
   eosio_assert(user_item_iter->item.state != object_state::on_equip_slot,"item is equip");

   asset item_sell_result(0, symbol(symbol_code("UTG"), 4));
   item_sell_result.amount = 10000000;

   action(permission_level{get_self(), "active"_n},
          get_self(), "tokentrans"_n,
          std::make_tuple(_self, _user, item_sell_result, std::string("item sell result")))
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
   user_items user_item_table(_self, _user.value);
   auto user_item_iter = user_item_table.find(_item_index);
   eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
   eosio_assert(user_item_iter->item.state == object_state::on_equip_slot, "already unequip this item");

   uint32_t slot = user_item_iter->item.type - 1;
   user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item)
   {
       unequip_item.item.state = object_state::on_inventory;
   });

   user_servants user_servant_table(_self, _user.value);
   auto user_servant_iter = user_servant_table.find(_servant_index);
   eosio_assert(user_servant_iter != user_servant_table.end(),"not exist servant info");

   user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant)
   {
       unequip_servant.servant.status.plus_str -= user_item_iter->item.status.basic_str;
       unequip_servant.servant.status.plus_dex -= user_item_iter->item.status.basic_dex;
       unequip_servant.servant.status.plus_int -= user_item_iter->item.status.basic_int;
       unequip_servant.servant.equip_slot[slot] = 0;
   });
}

void untpreregist::equip_servant(eosio::name _user, uint32_t _servant_index ,uint32_t _item_index)
{
   user_items user_item_table(_self, _user.value);
   auto user_item_iter = user_item_table.find(_item_index);
   eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
   eosio_assert(user_item_iter->item.state != object_state::on_equip_slot, "already equip this item");

   uint32_t slot = user_item_iter->item.type - 1;
   user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item)
   {
       unequip_item.item.state = object_state::on_equip_slot;
   });

   user_servants user_servant_table(_self, _user.value);
   auto user_servant_iter = user_servant_table.find(_servant_index);
   eosio_assert(user_servant_iter != user_servant_table.end(),"not exist servant info");

   user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant)
   {
       unequip_servant.servant.status.plus_str += user_item_iter->item.status.basic_str;
       unequip_servant.servant.status.plus_dex += user_item_iter->item.status.basic_dex;
       unequip_servant.servant.status.plus_int += user_item_iter->item.status.basic_int;
       unequip_servant.servant.equip_slot[slot] = _item_index;
   });
}
void untpreregist::unequip_hero(eosio::name _user, uint32_t _item_index)
{
   user_items user_item_table(_self, _user.value);
   auto user_item_iter = user_item_table.find(_item_index);
   eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
   eosio_assert(user_item_iter->item.state == object_state::on_equip_slot, "already unequip this item");

   uint32_t slot = user_item_iter->item.type - 1;
   user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item)
   {
       unequip_item.item.state = object_state::on_inventory;
   });

   auth_users user_auth_table(_self, _self.value);
   auto user_auth_iter = user_auth_table.find(_user.value);
   eosio_assert(user_auth_iter != user_auth_table.end(),"not exist user info");

   user_auth_table.modify(user_auth_iter, _self, [&](auto &unequip_hero)
   {
       unequip_hero.hero.status.plus_str -= user_item_iter->item.status.basic_str;
       unequip_hero.hero.status.plus_dex -= user_item_iter->item.status.basic_dex;
       unequip_hero.hero.status.plus_int -= user_item_iter->item.status.basic_int;
       unequip_hero.hero.equip_slot[slot] = 0;
   });
}

void untpreregist::equip_hero(eosio::name _user, uint32_t _item_index)
{
   user_items user_item_table(_self, _user.value);
   auto user_item_iter = user_item_table.find(_item_index);
   eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
   eosio_assert(user_item_iter->item.state != object_state::on_equip_slot, "already equip this item");

   uint32_t slot = user_item_iter->item.type - 1;
   user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item)
   {
       unequip_item.item.state = object_state::on_equip_slot;
   });

   auth_users user_auth_table(_self, _self.value);
   auto user_auth_iter = user_auth_table.find(_user.value);
   eosio_assert(user_auth_iter != user_auth_table.end(),"not exist user info");

   user_auth_table.modify(user_auth_iter, _self, [&](auto &unequip_hero)
   {
       unequip_hero.hero.status.plus_str += user_item_iter->item.status.basic_str;
       unequip_hero.hero.status.plus_dex += user_item_iter->item.status.basic_dex;
       unequip_hero.hero.status.plus_int += user_item_iter->item.status.basic_int;
       unequip_hero.hero.equip_slot[slot] = _item_index;
   });
}


#pragma endregion


ACTION untpreregist::setdata()
{
  require_auth(_self);

   servant_job_db other_job_db_table("unlimittest1"_n, "unlimittest1"_n.value);
   for (auto iter1 = other_job_db_table.begin(); iter1 != other_job_db_table.end();)
   {
       const auto &job_iter = other_job_db_table.get(iter1->primary_key(), "not exsit data");
       insert_job(job_iter.job, job_iter.min_range.base_str, job_iter.max_range.base_str);
       iter1++;
   }

  body_db other_body_db_table("unlimittest1"_n, "unlimittest1"_n.value);
  for (auto iter2 = other_body_db_table.begin(); iter2 != other_body_db_table.end();)
  {
      const auto &body_iter = other_body_db_table.get(iter2->primary_key(), "not exsit data");
      insert_body(body_iter.body);
      iter2++;
  }
  gender_db other_gender_db_table("unlimittest1"_n, "unlimittest1"_n.value);
  for (auto iter3 = other_gender_db_table.begin(); iter3 != other_gender_db_table.end();)
  {
      const auto &gender_iter = other_gender_db_table.get(iter3->primary_key(), "not exsit data");
      insert_gender(gender_iter.gender);
      iter3++;
  }

  hair_db other_hair_db_table("unlimittest1"_n, "unlimittest1"_n.value);
  for (auto iter4 = other_hair_db_table.begin(); iter4 != other_hair_db_table.end();)
  {
      const auto &hair_iter = other_hair_db_table.get(iter4->primary_key(), "not exsit data");
      insert_hair(hair_iter.hair);
      iter4++;
  }

  head_db other_head_db_table("unlimittest1"_n, "unlimittest1"_n.value);
  for (auto iter5 = other_head_db_table.begin(); iter5 != other_head_db_table.end();)
  {
      const auto &head_iter = other_head_db_table.get(iter5->primary_key(), "not exsit data");
      insert_head(head_iter.head);
      iter5++;
  }

  monster_id_db other_monster_id_db_table("unlimittest1"_n, "unlimittest1"_n.value);
  for (auto iter6 = other_monster_id_db_table.begin(); iter6 != other_monster_id_db_table.end();)
  {
      const auto &monster_id_iter = other_monster_id_db_table.get(iter6->primary_key(), "not exsit data");
      insert_monster_id(monster_id_iter.id);
      iter6++;
  }

     servant_id_db other_servant_id_db_table("unlimittest1"_n, "unlimittest1"_n.value);
     for(auto iter = other_servant_id_db_table.begin(); iter != other_servant_id_db_table.end();)
     {
         const auto &servant_id_iter = other_servant_id_db_table.get(iter->primary_key(),"not exsit data");
         insert_servant_id(servant_id_iter.id, servant_id_iter.index);
         iter++;
     }

   monster_grade_db other_monster_grade_db_table("unlimittest1"_n, "unlimittest1"_n.value);
   for (auto iter = other_monster_grade_db_table.begin(); iter != other_monster_grade_db_table.end();)
   {
       const auto &monster_grade_iter = other_monster_grade_db_table.get(iter->primary_key(), "not exsit data");
       insert_monster_grade(monster_grade_iter.grade, monster_grade_iter.min_range.base_str, monster_grade_iter.max_range.base_str);
       iter++;
   }

  item_id_db other_item_id_db_table("unlimittest1"_n, "unlimittest1"_n.value);
  for (auto iter9 = other_item_id_db_table.begin(); iter9 != other_item_id_db_table.end();)
  {
      const auto &item_id_iter = other_item_id_db_table.get(iter9->primary_key(), "not exsit data");
      insert_item_id(item_id_iter.id, item_id_iter.type, item_id_iter.job, item_id_iter.tier);
      iter9++;
  }

  item_grade_db other_item_grade_db_table("unlimittest1"_n, "unlimittest1"_n.value);
  for (auto iter10 = other_item_grade_db_table.begin(); iter10 != other_item_grade_db_table.end();)
  {
      const auto &item_grade_iter = other_item_grade_db_table.get(iter10->primary_key(), "not exsit data");
      insert_item_grade(item_grade_iter.grade, item_grade_iter.min_range.base_str, item_grade_iter.max_range.base_str);
      iter10++;
  }
}


#undef EOSIO_DISPATCH

#define EOSIO_DISPATCH(TYPE, MEMBERS)                                                          \
    extern "C"                                                                                 \
    {                                                                                          \
        void apply(uint64_t receiver, uint64_t code, uint64_t action)                          \
        {                                                                                      \
            if (code == receiver)                                                              \
            {                                                                                  \
                eosio_assert(action != name("eostransfer").value, "Impossible This Action 1"); \
                switch (action)                                                                \
                {                                                                              \
                    EOSIO_DISPATCH_HELPER(TYPE, MEMBERS)                                       \
                }                                                                              \
                /* does not allow destructor of thiscontract to run: eosio_exit(0); */         \
            }                                                                                  \
            else if (code == name("eosio.token").value && action == name("transfer").value)    \
            {                                                                                  \
                eosio_assert(code == name("eosio.token").value, "Must transfer EOS");          \
                execute_action(name(receiver), name(code), &untpreregist::eostransfer);        \
            }                                                                                  \
        }                                                                                      \
    }
// eos 금액에 대해 체크 하는 함

EOSIO_DISPATCH(untpreregist, (setdata)(herocheat)(partycheat)(resultpre)(resultgacha)(create)(issue)(transfer)(setmaster)(settokenlog)(eostransfer)(initmaster)(inittokenlog)(deleteblack)(addblack)(setpause)(dbinsert)(dbmodify)(dberase)(dbinit))