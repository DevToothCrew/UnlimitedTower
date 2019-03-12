#include "Common/common_header.hpp"
#include "Common/common_seed.hpp"
#include "battletest.hpp"

//------------------------------------------------------------------------//
//----------------------------unlimited_tower_action----------------------//
//------------------------------------------------------------------------//

//------------------------------------------------------------------------//
//----------------------------Token_action--------------------------------//
//------------------------------------------------------------------------//
#pragma region Token action

ACTION battletest::create(eosio::name issuer, asset maximum_supply)
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

ACTION battletest::issue(eosio::name to, asset quantity, string memo)
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

ACTION battletest::transfer(name from, name to, asset quantity, string memo)
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

void battletest::sub_balance(name user, asset value)
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

void battletest::add_balance(name user, asset value, name ram_payer)
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
void battletest::substr_value(std::string _value, std::vector<std::string> &_value_list, std::vector<size_t> &_size_list, uint32_t _size)
{
    _size_list.resize(_size);
    for (uint32_t i = 0; i < _size_list.size(); ++i)
    {
        if (i == 0)
        {
            _size_list[i] = _value.find(':');
            _value_list.push_back(_value.substr(0, _size_list[i]));
        }
        else
        {
            if (_value.find(':', _size_list[i - 1] + 1) == std::string::npos)
            {
                _size_list[i] = _value.length() - (_size_list[i - 1]);
                _value_list.push_back(_value.substr(_size_list[i - 1] + 1, (_size_list[i] - 1) - _size_list[i - 1]));
                break;
            }
            else
            {
                _size_list[i] = _value.find(':', _size_list[i - 1] + 1);
                _value_list.push_back(_value.substr(_size_list[i - 1] + 1, (_size_list[i] - 1) - _size_list[i - 1]));
            }
        }
    }
}

ACTION battletest::dbinsert(std::string _table, std::string _value)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 1");
    std::vector<size_t> size_list;
    std::vector<std::string> value_list;
    uint32_t value;
    if (_table == "dbbody")
    {
        value = atoll(_value.c_str());
        insert_body(value);
    }
    else if (_table == "dbhair")
    {
        value = atoll(_value.c_str());
        insert_hair(value);
    }
    else if (_table == "dbhead")
    {
        value = atoll(_value.c_str());
        insert_head(value);
    }
    else if (_table == "dbgender")
    {
        value = atoll(_value.c_str());
        insert_gender(value);
    }
    else if (_table == "dbmonsterup")
    {
        value = atoi(_value.c_str());
        insert_upgrade_monster_ratio(value);
    }
    else if (_table == "dblevel")
    {
        value = atoi(_value.c_str());
        insert_level(value);
    }
    else if (_table == "dbservantjob")
    {
        substr_value(_value, value_list, size_list, 4);
        insert_job(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
    }
    else if (_table == "dbitemgrade")
    {
        substr_value(_value, value_list, size_list, 4);
        insert_item_grade(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
    }
    else if (_table == "dbmonstergd")
    {
        substr_value(_value, value_list, size_list, 4);
        insert_monster_grade(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
    }
    else if (_table == "dbgraderatio")
    {
        substr_value(_value, value_list, size_list, 2);
        insert_grade_ratio(atoi(value_list[0].c_str()), atoi(value_list[1].c_str()));
    }
    else if (_table == "dbservant")
    {
        substr_value(_value, value_list, size_list, 2);
        insert_servant_id(atoll(value_list[0].c_str()), atoll(value_list[1].c_str()));
    }
    else if (_table == "dbmonster")
    {
        substr_value(_value, value_list, size_list, 7);
        insert_monster_id(atoll(value_list[0].c_str()), atoll(value_list[1].c_str()), atoll(value_list[2].c_str()),
                          atoll(value_list[3].c_str()), atoll(value_list[4].c_str()), atoll(value_list[5].c_str()), atoll(value_list[6].c_str()));
    }
    else if (_table == "dbitemup")
    {
        substr_value(_value, value_list, size_list, 3);
        insert_upgrade_item_ratio(atoll(value_list[0].c_str()), atoi(value_list[1].c_str()), atoll(value_list[2].c_str()));
    }
    else if (_table == "dbconsumable")
    {
    }
    else if (_table == "dbcommonitem")
    {
        substr_value(_value, value_list, size_list, 8);
        insert_common_item_id(atoll(value_list[0].c_str()),
                              atoll(value_list[1].c_str()),
                              atoi(value_list[2].c_str()),
                              atoi(value_list[3].c_str()),
                              atoi(value_list[4].c_str()),
                              atoi(value_list[5].c_str()),
                              atoll(value_list[6].c_str()),
                              atoll(value_list[7].c_str()));
    }
    else if (_table == "dbequipitem")
    {
        substr_value(_value, value_list, size_list, 10);
        insert_equip_item_id(atoll(value_list[0].c_str()),
                             atoll(value_list[1].c_str()),
                             atoll(value_list[2].c_str()),
                             atoll(value_list[3].c_str()),
                             atoll(value_list[4].c_str()),
                             atoll(value_list[5].c_str()),
                             atoi(value_list[6].c_str()),
                             atoi(value_list[7].c_str()),
                             atoi(value_list[8].c_str()),
                             atoll(value_list[9].c_str()));
    }
}
void battletest::insert_job(std::string _status, uint64_t _job, uint64_t _min, uint64_t _max)
{
    servant_job_db servant_job_db_table(_self, _self.value);
    auto servant_job_iter = servant_job_db_table.find(_job);
    if (servant_job_iter == servant_job_db_table.end())
    {
        servant_job_db_table.emplace(_self, [&](auto &new_job) {
            new_job.job = _job;
            if (_status == "all")
            {
                new_job.min_range.base_str = _min;
                new_job.min_range.base_dex = _min;
                new_job.min_range.base_int = _min;

                new_job.max_range.base_str = _max;
                new_job.max_range.base_dex = _max;
                new_job.max_range.base_int = _max;
            }
            else if (_status == "str")
            {
                new_job.min_range.base_str = _min;
                new_job.max_range.base_str = _max;
            }
            else if (_status == "dex")
            {
                new_job.min_range.base_dex = _min;
                new_job.max_range.base_dex = _max;
            }
            else if (_status == "int")
            {
                new_job.min_range.base_int = _min;
                new_job.max_range.base_int = _max;
            }
        });
    }
    else
    {
        servant_job_db_table.modify(servant_job_iter, _self, [&](auto &new_job) {
            if (_status == "all")
            {
                new_job.min_range.base_str = _min;
                new_job.min_range.base_dex = _min;
                new_job.min_range.base_int = _min;

                new_job.max_range.base_str = _max;
                new_job.max_range.base_dex = _max;
                new_job.max_range.base_int = _max;
            }
            else if (_status == "str")
            {
                new_job.min_range.base_str = _min;
                new_job.max_range.base_str = _max;
            }
            else if (_status == "dex")
            {
                new_job.min_range.base_dex = _min;
                new_job.max_range.base_dex = _max;
            }
            else if (_status == "int")
            {
                new_job.min_range.base_int = _min;
                new_job.max_range.base_int = _max;
            }
        });
    }
}

void battletest::insert_body(uint64_t _appear)
{
    body_db body_db_table(_self, _self.value);
    body_db_table.emplace(_self, [&](auto &new_body) {
        new_body.body = _appear;
    });
}

void battletest::insert_hair(uint64_t _appear)
{
    hair_db hair_db_table(_self, _self.value);
    hair_db_table.emplace(_self, [&](auto &new_hair) {
        new_hair.hair = _appear;
    });
}
void battletest::insert_head(uint64_t _appear)
{
    head_db head_db_table(_self, _self.value);
    head_db_table.emplace(_self, [&](auto &new_head) {
        new_head.head = _appear;
    });
}

void battletest::insert_gender(uint64_t _appear)
{
    gender_db gender_db_table(_self, _self.value);
    gender_db_table.emplace(_self, [&](auto &new_gender) {
        new_gender.gender = _appear;
    });
}

void battletest::insert_servant_id(uint64_t _servant_id, uint64_t _gacha_id)
{
    servnt_db servant_id_db_table(_self, _self.value);
    auto servant_id_iter = servant_id_db_table.find(_servant_id);
    if (servant_id_iter == servant_id_db_table.end())
    {
        servant_id_db_table.emplace(_self, [&](auto &new_servant_id) {
            new_servant_id.servant_id = _servant_id;
            new_servant_id.gacha_id = _gacha_id;
        });
    }
    else
    {
        servant_id_db_table.modify(servant_id_iter, _self, [&](auto &new_servant_id) {
            new_servant_id.gacha_id = _gacha_id;
        });
    }
}

void battletest::insert_monster_id(uint64_t _monster_id, uint64_t _gacha_id, uint64_t _type, uint64_t _main_status, uint64_t _skill1, uint64_t _skill2, uint64_t _skill3)
{
    monster_db monster_id_db_table(_self, _self.value);
    auto monster_id_iter = monster_id_db_table.find(_monster_id);
    if (monster_id_iter == monster_id_db_table.end())
    {
        monster_id_db_table.emplace(_self, [&](auto &new_monster_id) {
            new_monster_id.gacha_id = _gacha_id;
            new_monster_id.monster_id = _monster_id;
            new_monster_id.type = _type;
            new_monster_id.main_status = _main_status;

            if (_skill1 != 0 || _skill2 != 0 || _skill3 != 0)
            {
                skill_object skill;
                skill.skill_1 = _skill1;
                skill.skill_2 = _skill2;
                skill.skill_3 = _skill3;
                new_monster_id.skills.push_back(skill);
            }
        });
    }
    else
    {
        monster_id_db_table.modify(monster_id_iter, _self, [&](auto &new_monster_id) {
            new_monster_id.gacha_id = _gacha_id;
            new_monster_id.type = _type;
            new_monster_id.main_status = _main_status;

            if (_skill1 != 0 || _skill2 != 0 || _skill3 != 0)
            {
                skill_object skill;
                skill.skill_1 = _skill1;
                skill.skill_2 = _skill2;
                skill.skill_3 = _skill3;
                new_monster_id.skills.push_back(skill);
            }
        });
    }
}

void battletest::insert_monster_grade(std::string _status, uint64_t _grade, uint64_t _min, uint64_t _max)
{
    monster_grade_db monster_grade_db_table(_self, _self.value);
    auto monster_grade_iter = monster_grade_db_table.find(_grade);
    if (monster_grade_iter == monster_grade_db_table.end())
    {
        monster_grade_db_table.emplace(_self, [&](auto &new_monster_grade) {
            new_monster_grade.grade = _grade;
            if (_status == "all")
            {
                new_monster_grade.min_range.base_dex = _min;
                new_monster_grade.min_range.base_int = _min;
                new_monster_grade.min_range.base_str = _min;
                new_monster_grade.max_range.base_dex = _max;
                new_monster_grade.max_range.base_int = _max;
                new_monster_grade.max_range.base_str = _max;
            }
            else if (_status == "str")
            {
                new_monster_grade.max_range.base_str = _max;
                new_monster_grade.min_range.base_str = _min;
            }
            else if (_status == "dex")
            {
                new_monster_grade.max_range.base_dex = _max;
                new_monster_grade.min_range.base_dex = _min;
            }
            else if (_status == "int")
            {
                new_monster_grade.max_range.base_int = _max;
                new_monster_grade.min_range.base_int = _min;
            }
        });
    }
    else
    {
        monster_grade_db_table.modify(monster_grade_iter, _self, [&](auto &new_monster_grade) {
            if (_status == "all")
            {
                new_monster_grade.min_range.base_dex = _min;
                new_monster_grade.min_range.base_int = _min;
                new_monster_grade.min_range.base_str = _min;
                new_monster_grade.max_range.base_dex = _max;
                new_monster_grade.max_range.base_int = _max;
                new_monster_grade.max_range.base_str = _max;
            }
            else if (_status == "str")
            {
                new_monster_grade.max_range.base_str = _max;
                new_monster_grade.min_range.base_str = _min;
            }
            else if (_status == "dex")
            {
                new_monster_grade.max_range.base_dex = _max;
                new_monster_grade.min_range.base_dex = _min;
            }
            else if (_status == "int")
            {
                new_monster_grade.max_range.base_int = _max;
                new_monster_grade.min_range.base_int = _min;
            }
        });
    }
}

void battletest::insert_equip_item_id(uint64_t _item_id, uint64_t _item_set_id, uint64_t _job, uint64_t _tier, uint64_t _type, uint64_t _option, uint32_t _status_value_min, uint32_t _status_value_max, uint32_t _upgrade_status_value, uint64_t _random_option_id)
{
    equipitem_db item_id_db_table(_self, _self.value);
    auto item_id_iter = item_id_db_table.find(_item_id);
    if (item_id_iter == item_id_db_table.end())
    {
        item_id_db_table.emplace(_self, [&](auto &new_item_id) {
            new_item_id.set_id = _item_set_id;
            new_item_id.item_id = _item_id;
            new_item_id.job = _job;
            new_item_id.type = _type;
            new_item_id.tier = _tier;
            new_item_id.option = _option;
            new_item_id.status_value_min = _status_value_min;
            new_item_id.status_value_max = _status_value_max;
            new_item_id.upgrade_status_value = _upgrade_status_value;
            new_item_id.random_option_id = _random_option_id;
        });
    }
    else
    {
        item_id_db_table.modify(item_id_iter, _self, [&](auto &new_item_id) {
            new_item_id.set_id = _item_set_id;
            new_item_id.job = _job;
            new_item_id.type = _type;
            new_item_id.tier = _tier;
            new_item_id.option = _option;
            new_item_id.status_value_min = _status_value_min;
            new_item_id.status_value_max = _status_value_max;
            new_item_id.upgrade_status_value = _upgrade_status_value;
            new_item_id.random_option_id = _random_option_id;
        });
    }
}

void battletest::insert_common_item_id(uint64_t _item_id, uint64_t _item_gacha_id, uint32_t _type, uint32_t _param_1, uint32_t _param_2, uint32_t _param_3, uint64_t _sell_id, uint64_t _sell_cost)
{
    commonitem_db item_id_db_table(_self, _self.value);
    auto item_id_iter = item_id_db_table.find(_item_id);
    if (item_id_iter == item_id_db_table.end())
    {
        item_id_db_table.emplace(_self, [&](auto &new_item_id) {
            new_item_id.gacha_id = _item_gacha_id;
            new_item_id.item_id = _item_id;
            new_item_id.type = _type;
            new_item_id.param_1 = _param_1;
            new_item_id.param_2 = _param_2;
            new_item_id.param_3 = _param_3;
            new_item_id.sell_id = _sell_id;
            new_item_id.sell_cost = _sell_cost;
        });
    }
    else
    {
        item_id_db_table.modify(item_id_iter, _self, [&](auto &new_item_id) {
            new_item_id.gacha_id = _item_gacha_id;
            new_item_id.type = _type;
            new_item_id.param_1 = _param_1;
            new_item_id.param_2 = _param_2;
            new_item_id.param_3 = _param_3;
            new_item_id.sell_id = _sell_id;
            new_item_id.sell_cost = _sell_cost;
        });
    }
}

void battletest::insert_item_grade(std::string _status, uint64_t _grade, uint64_t _min, uint64_t _max)
{
    item_grade_db item_grade_db_table(_self, _self.value);
    auto item_grade_iter = item_grade_db_table.find(_grade);
    if (item_grade_iter == item_grade_db_table.end())
    {
        item_grade_db_table.emplace(_self, [&](auto &new_item_grade) {
            new_item_grade.grade = _grade;
            if (_status == "all")
            {
                new_item_grade.min_range.base_dex = _min;
                new_item_grade.min_range.base_int = _min;
                new_item_grade.min_range.base_str = _min;
                new_item_grade.max_range.base_dex = _max;
                new_item_grade.max_range.base_int = _max;
                new_item_grade.max_range.base_str = _max;
            }
            else if (_status == "str")
            {
                new_item_grade.max_range.base_str = _max;
                new_item_grade.min_range.base_str = _min;
            }
            else if (_status == "dex")
            {
                new_item_grade.max_range.base_dex = _max;
                new_item_grade.min_range.base_dex = _min;
            }
            else if (_status == "int")
            {
                new_item_grade.max_range.base_int = _max;
                new_item_grade.min_range.base_int = _min;
            }
        });
    }
    else
    {
        item_grade_db_table.modify(item_grade_iter, _self, [&](auto &new_item_grade) {
            new_item_grade.grade = _grade;
            if (_status == "all")
            {
                new_item_grade.min_range.base_dex = _min;
                new_item_grade.min_range.base_int = _min;
                new_item_grade.min_range.base_str = _min;
                new_item_grade.max_range.base_dex = _max;
                new_item_grade.max_range.base_int = _max;
                new_item_grade.max_range.base_str = _max;
            }
            else if (_status == "str")
            {
                new_item_grade.max_range.base_str = _max;
                new_item_grade.min_range.base_str = _min;
            }
            else if (_status == "dex")
            {
                new_item_grade.max_range.base_dex = _max;
                new_item_grade.min_range.base_dex = _min;
            }
            else if (_status == "int")
            {
                new_item_grade.max_range.base_int = _max;
                new_item_grade.min_range.base_int = _min;
            }
        });
    }
}

void battletest::insert_grade_ratio(uint64_t _grade, uint64_t _ratio)
{
    grade_ratio_db grade_ratio_db_table(_self, _self.value);
    auto grade_ratio_iter = grade_ratio_db_table.find(_grade);
    if (grade_ratio_iter == grade_ratio_db_table.end())
    {
        grade_ratio_db_table.emplace(_self, [&](auto &new_grade_ratio) {
            new_grade_ratio.grade = _grade;
            new_grade_ratio.ratio = _ratio;
        });
    }
    else
    {
        grade_ratio_db_table.modify(grade_ratio_iter, _self, [&](auto &new_grade_ratio) {
            new_grade_ratio.ratio = _ratio;
        });
    }
}

void battletest::insert_upgrade_monster_ratio(uint32_t _main)
{
    upgrade_monster_ratio_db upgrade_monster_ratio_db_table(_self, _self.value);
    upgrade_monster_ratio_db_table.emplace(_self, [&](auto &new_upgrade__monster_ratio) {
        new_upgrade__monster_ratio.main_monster = _main;
        std::vector<upgrade_monster_sub> upgrade_sub_list;
        std::vector<uint32_t> upgrade_temp_ratio_list;
        std::vector<uint32_t> upgrade_ratio_list;
        uint64_t max_ratio = 10000;

        for (uint32_t i = 0; i <= _main; i++)
        {
            upgrade_temp_ratio_list.push_back(max_ratio);
            max_ratio = max_ratio / 2;
        }

        uint32_t temp = _main + 1;
        upgrade_ratio_list.resize(temp);
        for (uint32_t i = 0; i < upgrade_temp_ratio_list.size(); i++)
        {
            upgrade_ratio_list[temp - 1] = upgrade_temp_ratio_list[i];
            temp--;
        }
        for (uint32_t i = 0; i <= _main; i++)
        {
            upgrade_monster_sub new_sub;
            new_sub.sub_monster = i;
            new_sub.ratio = upgrade_ratio_list[i];
            new_upgrade__monster_ratio.sub.push_back(new_sub);
        }
    });
}

void battletest::insert_upgrade_item_ratio(uint64_t _main, uint32_t _material, uint64_t _ratio)
{
    upgrade_item_ratio_db upgrade_itme_ratio_db_table(_self, _self.value);
    auto upgrade_item_ratio_iter = upgrade_itme_ratio_db_table.find(_main);
    if (upgrade_item_ratio_iter == upgrade_itme_ratio_db_table.end())
    {
        upgrade_itme_ratio_db_table.emplace(_self, [&](auto &new_upgrade_item_ratio) {
            new_upgrade_item_ratio.upgrade_item = _main;
            new_upgrade_item_ratio.material = _material;
            new_upgrade_item_ratio.ratio = _ratio;
        });
    }
    else
    {
        upgrade_itme_ratio_db_table.modify(upgrade_item_ratio_iter, _self, [&](auto &new_upgrade_item_ratio) {
            new_upgrade_item_ratio.material = _material;
            new_upgrade_item_ratio.ratio = _ratio;
        });
    }
}

void battletest::insert_consumables_id(uint32_t _id, uint32_t _type, uint64_t _price)
{
    user_consumables_db user_consumables_db_table(_self, _self.value);
    user_consumables_db_table.emplace(_self, [&](auto &new_consumables_id) {
        new_consumables_id.id = _id;
        new_consumables_id.type = _type;
        new_consumables_id.price = _price;
    });
}

void battletest::insert_level(uint32_t _id)
{
    uint64_t temp_exp = (-180 + 259 * _id + 93 * _id * _id + 8 * _id * _id * _id) * 5 / 6;
    lv_exp lv_exp_table(_self, _self.value);
    lv_exp_table.emplace(_self, [&](auto &new_lv_exp) {
        new_lv_exp.lv = _id + 1;
        new_lv_exp.exp = temp_exp;
    });
}

ACTION battletest::dberase(std::string _table, std::string _value)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 3");

    std::vector<size_t> size_list;
    std::vector<std::string> value_list;
    uint64_t value;
    if (_table == "dbbody")
    {
        value = atoll(_value.c_str());
        erase_body(value);
    }
    else if (_table == "dbhair")
    {
        value = atoll(_value.c_str());
        erase_hair(value);
    }
    else if (_table == "dbhead")
    {
        value = atoll(_value.c_str());
        erase_head(value);
    }
    else if (_table == "dbgender")
    {
        value = atoll(_value.c_str());
        erase_gender(value);
    }
    else if (_table == "dbmonsterup")
    {
        value = atoi(_value.c_str());
        erase_upgrade_monster_ratio(value);
    }
    else if (_table == "dblevel")
    {
        value = atoi(_value.c_str());
        erase_level(value);
    }
    else if (_table == "dbservantjob")
    {
        value = atoll(_value.c_str());
        erase_job(value);
    }
    else if (_table == "dbitemgrade")
    {
        value = atoll(_value.c_str());
        erase_item_grade(value);
    }
    else if (_table == "dbmonstergd")
    {
        value = atoll(_value.c_str());
        erase_monster_grade(value);
    }
    else if (_table == "dbgraderatio")
    {
        value = atoll(_value.c_str());
        erase_grade_ratio(value);
    }
    else if (_table == "dbitemup")
    {
        value = atoi(_value.c_str());
        erase_upgrade_item_ratio(value);
    }
    else if (_table == "dbservant")
    {
        value = atoll(_value.c_str());
        erase_servant_id(value);
    }
    else if (_table == "dbmonster")
    {
        value = atoll(_value.c_str());
        erase_monster_id(value);
    }
    else if (_table == "dbconsumable")
    {
    }
    else if (_table == "dbcommonitem")
    {
        value = atoll(_value.c_str());
        erase_equip_item_id(value);
    }
    else if (_table == "dbequipitem")
    {
        value = atoll(_value.c_str());
        erase_common_item_id(value);
    }
}

void battletest::erase_job(uint64_t _job)
{
    servant_job_db servant_job_db_table(_self, _self.value);
    auto job_iter = servant_job_db_table.find(_job);
    eosio_assert(job_iter != servant_job_db_table.end(), "Not Exist Job Data");
    servant_job_db_table.erase(job_iter);
}

void battletest::erase_body(uint64_t _appear)
{
    body_db body_db_table(_self, _self.value);
    auto body_iter = body_db_table.find(_appear);
    eosio_assert(body_iter != body_db_table.end(), "Not Find Body Data");
    body_db_table.erase(body_iter);
}

void battletest::erase_hair(uint64_t _appear)
{
    hair_db hair_db_table(_self, _self.value);
    auto hair_iter = hair_db_table.find(_appear);
    eosio_assert(hair_iter != hair_db_table.end(), "Not Find Hair Data");
    hair_db_table.erase(hair_iter);
}

void battletest::erase_head(uint64_t _appear)
{
    head_db head_db_table(_self, _self.value);
    auto head_iter = head_db_table.find(_appear);
    eosio_assert(head_iter != head_db_table.end(), "Not Find Head Data");
    head_db_table.erase(head_iter);
}

void battletest::erase_gender(uint64_t _appear)
{
    gender_db gender_db_table(_self, _self.value);
    auto gender_iter = gender_db_table.find(_appear);
    eosio_assert(gender_iter != gender_db_table.end(), "Not Find Gender Data");
    gender_db_table.erase(gender_iter);
}

void battletest::erase_servant_id(uint64_t _id)
{
    servnt_db servant_id_db_table(_self, _self.value);
    auto servant_id_iter = servant_id_db_table.find(_id);
    eosio_assert(servant_id_iter != servant_id_db_table.end(), "Not Find Servant ID Data");
    servant_id_db_table.erase(servant_id_iter);
}

void battletest::erase_monster_id(uint64_t _id)
{
    monster_db monster_id_db_table(_self, _self.value);
    auto monster_id_iter = monster_id_db_table.find(_id);
    eosio_assert(monster_id_iter != monster_id_db_table.end(), "Not Find Monster ID Data");
    monster_id_db_table.erase(monster_id_iter);
}

void battletest::erase_monster_grade(uint64_t _grade)
{
    monster_grade_db monster_grade_db_table(_self, _self.value);
    auto monster_grade_iter = monster_grade_db_table.find(_grade);
    eosio_assert(monster_grade_iter != monster_grade_db_table.end(), "Not Exist Monster Grade");
    monster_grade_db_table.erase(monster_grade_iter);
}

void battletest::erase_item_grade(uint64_t _grade)
{
    item_grade_db item_grade_db_table(_self, _self.value);
    auto item_grade_iter = item_grade_db_table.find(_grade);
    eosio_assert(item_grade_iter != item_grade_db_table.end(), "Not Exist Item Grade Data");
    item_grade_db_table.erase(item_grade_iter);
}

void battletest::erase_equip_item_id(uint64_t _id)
{
    equipitem_db item_id_db_table(_self, _self.value);
    auto item_id_iter = item_id_db_table.find(_id);
    eosio_assert(item_id_iter != item_id_db_table.end(), "Not Exist Item ID 2");
    item_id_db_table.erase(item_id_iter);
}

void battletest::erase_common_item_id(uint64_t _id)
{
    commonitem_db item_id_db_table(_self, _self.value);
    auto item_id_iter = item_id_db_table.find(_id);
    eosio_assert(item_id_iter != item_id_db_table.end(), "Not Exist Item ID 2");
    item_id_db_table.erase(item_id_iter);
}

void battletest::erase_grade_ratio(uint64_t _grade)
{
    grade_ratio_db grade_ratio_db_table(_self, _self.value);
    auto grade_ratio_db_iter = grade_ratio_db_table.find(_grade);
    eosio_assert(grade_ratio_db_iter != grade_ratio_db_table.end(), "Not Exist Grade Ratio Data");
    grade_ratio_db_table.erase(grade_ratio_db_iter);
}

void battletest::erase_upgrade_monster_ratio(uint32_t _main)
{
    upgrade_monster_ratio_db upgrade_monster_ratio_db_table(_self, _self.value);
    auto upgrade_monster_ratio_db_iter = upgrade_monster_ratio_db_table.find(_main);
    eosio_assert(upgrade_monster_ratio_db_iter != upgrade_monster_ratio_db_table.end(), "Not Exist upgrade Main Monster Data");
    upgrade_monster_ratio_db_table.erase(upgrade_monster_ratio_db_iter);
}

void battletest::erase_upgrade_item_ratio(uint32_t _main)
{
    upgrade_item_ratio_db upgrade_item_ratio_db_table(_self, _self.value);
    auto upgrade_item_ratio_db_iter = upgrade_item_ratio_db_table.find(_main);
    eosio_assert(upgrade_item_ratio_db_iter != upgrade_item_ratio_db_table.end(), "Not Exist upgrade Main Item Data");
    upgrade_item_ratio_db_table.erase(upgrade_item_ratio_db_iter);
}

void battletest::erase_consumables_id(uint32_t _id)
{
    user_consumables_db user_consumables_db_table(_self, _self.value);
    auto user_consumables_db_iter = user_consumables_db_table.find(_id);
    eosio_assert(user_consumables_db_iter != user_consumables_db_table.end(), "Not Exist Consumables Item ID Data");
    user_consumables_db_table.erase(user_consumables_db_iter);
}

void battletest::erase_level(uint32_t _id)
{
    lv_exp lv_exp_table(_self, _self.value);
    auto lv_exp_iter = lv_exp_table.find(_id);
    eosio_assert(lv_exp_iter != lv_exp_table.end(), "Not exist Level to Exp Data");
    lv_exp_table.erase(lv_exp_iter);
}

ACTION battletest::dbinit()
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

    commonitem_db commonitem_db_table(_self, _self.value);
    for (auto item_grade_db_table_iter = commonitem_db_table.begin(); item_grade_db_table_iter != commonitem_db_table.end();)
    {
        auto iter = commonitem_db_table.find(item_grade_db_table_iter->primary_key());
        item_grade_db_table_iter++;
        commonitem_db_table.erase(iter);
    }
}

#pragma endresion

//------------------------------------------------------------------------//
//-----------------------------setdata_action-----------------------------//
//------------------------------------------------------------------------//

#pragma region set
ACTION battletest::setmaster(eosio::name _master)
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

        if (total_token_log_iter != total_token_log_table.end())
        {
            total_token_log_table.emplace(_self, [&](auto &move_master) {
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

ACTION battletest::settokenlog()
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
    eosio_assert(total_token_log_iter == total_token_log_table.end(), "Exist Token Log");
    total_token_log_table.emplace(_self, [&](auto &preregist) {
        preregist.owner = system_master_iter->master;
    });
}

#pragma endregion

//------------------------------------------------------------------------//
//-------------------------------login_action-----------------------------//
//------------------------------------------------------------------------//

#pragma region login

void battletest::signup(eosio::name _user)
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
        new_party.servant_list.resize(4);
        new_party.monster_list.resize(5);
    });
}

// eosio.token recipient
// memo description spec
//-------------------------------------------------------------------------

ACTION battletest::eostransfer(eosio::name sender, eosio::name receiver)
{
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(sender.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "BlackList User 3");

    whitelist whitelist_table(_self, _self.value);
    auto whitelist_iter = whitelist_table.find(sender.value);
    eosio_assert(whitelist_iter != whitelist_table.end(), "Not White User 2");

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
void battletest::eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func)
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
    else if (res.action == "cheat") //test
    {
        system_master system_master_table(_self, _self.value);
        auto system_master_iter = system_master_table.begin();
        eosio_assert(system_master_iter->state != system_state::pause, "Server Pause 7");

        herocheat(sender);
        partycheat(sender);
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
ACTION battletest::initmaster()
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

    system_master_table.emplace(_self, [&](auto &owner_master) {
        owner_master.master = _self;
        owner_master.state = system_state::pause;
    });
}

#pragma endregion

#pragma reion init all table

ACTION battletest::inittokenlog()
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
uint64_t battletest::get_random_grade(uint64_t _rate)
{
    uint64_t grade;
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
uint32_t battletest::get_servant_index(uint32_t _job, uint32_t _body, uint32_t _gender, uint32_t _head, uint32_t _hair)
{
    uint32_t result_id;
    result_id = (_job * 1000000) + (_body * 100000) + (_gender * 10000) + (_head * 100) + (_hair * 1);
    return result_id;
}

void battletest::gacha_servant_id(eosio::name _user, uint64_t _seed)
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

    servnt_db servant_id_table(_self, _self.value);
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

uint8_t battletest::gacha_servant_head(uint64_t _seed, uint32_t _count)
{
    head_db head_db_table(_self, _self.value);
    uint8_t random_head = safeseed::get_random_value(_seed, HEAD_COUNT, DEFAULT_MIN_DB, _count);
    const auto &head_db_iter = head_db_table.get(random_head, "Not Exist Head Info");
    return head_db_iter.head;
}

uint8_t battletest::gacha_servant_hair(uint64_t _seed, uint32_t _count)
{
    hair_db hair_db_table(_self, _self.value);
    uint8_t random_hair = safeseed::get_random_value(_seed, HAIR_COUNT, DEFAULT_MIN_DB, _count);
    const auto &hair_db_iter = hair_db_table.get(random_hair, "Not Exist Hair Info");
    return hair_db_iter.hair;
}

uint8_t battletest::gacha_servant_body(uint64_t _seed, uint32_t _count)
{
    body_db body_db_table(_self, _self.value);
    uint8_t random_body = safeseed::get_random_value(_seed, BODY_COUNT, DEFAULT_MIN_DB, _count);
    const auto &body_db_iter = body_db_table.get(random_body, "Not Exist Body Info");
    return body_db_iter.body;
}

void battletest::gacha_monster_id(eosio::name _user, uint64_t _seed)
{
    monster_db monster_id_db_table(_self, _self.value);
    uint64_t random_monster_id = safeseed::get_random_value(_seed, MONSTER_ID_COUNT, DEFAULT_MIN_DB, monster_random_count);
    random_monster_id += MONSTER_GACHA_ID_START;
    auto monster_second = monster_id_db_table.get_index<"second"_n>();
    const auto &monster_id_db_iter = monster_second.get(random_monster_id, "Not Exist Monster ID 1");

    monster_random_count += 1;
    uint64_t random_rate = safeseed::get_random_value(_seed, GACHA_MAX_RATE, DEFAULT_MIN, monster_random_count);
    uint64_t random_grade = get_random_grade(random_rate);

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
        new_monster.id = monster_id_db_iter.monster_id;
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

void battletest::gacha_item_id(eosio::name _user, uint64_t _seed)
{
    commonitem_db item_id_db_table(_self, _self.value);
    uint32_t random_item_id = safeseed::get_random_value(_seed, ITEM_ID_COUNT, DEFAULT_MIN, item_random_count);
    random_item_id += ITEM_GACHA_ID_START;
    auto item_second = item_id_db_table.get_index<"second"_n>();
    const auto &item_id_db_iter = item_second.get(random_item_id, "Not Exist Item ID 1");

    equipitem_db equip_item_table(_self, _self.value);
    const auto &equip_item_iter = equip_item_table.get(item_id_db_iter.item_id, "Not Exist Equip ID 1");

    item_random_count += 1;
    uint64_t random_rate = safeseed::get_random_value(_seed, GACHA_MAX_RATE, DEFAULT_MIN, item_random_count);
    uint64_t random_grade = get_random_grade(random_rate);

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
        new_item.id = equip_item_iter.item_id;
        new_item.type = equip_item_iter.type;
        new_item.tier = equip_item_iter.tier;
        new_item.job = equip_item_iter.job;
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
uint64_t battletest::get_user_seed_value(uint64_t _user)
{
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

void battletest::start_gacha(eosio::name _user, uint64_t _seed)
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
        uint64_t l_gacha_result_type = safeseed::get_random_value(l_seed, GACHA_MAX_RATE, DEFAULT_MIN, DEFAULT_RANDOM_COUNT);
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

ACTION battletest::deleteblack(eosio::name _user)
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

ACTION battletest::addblack(eosio::name _user)
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

#pragma region white action

ACTION battletest::deletewhite(eosio::name _user)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    whitelist white_table(_self, _self.value);
    auto whitelist_iter = white_table.find(_user.value);
    eosio_assert(whitelist_iter != white_table.end(), "User Not White List");

    white_table.erase(whitelist_iter);
}

ACTION battletest::addwhite(eosio::name _user)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    whitelist white_table(_self, _self.value);
    auto whitelist_iter = white_table.find(_user.value);
    eosio_assert(whitelist_iter == white_table.end(), "White List User");

    white_table.emplace(_self, [&](auto &new_white_user) {
        new_white_user.user = _user;
    });
}

#pragma endregion

#pragma region owenr action

ACTION battletest::setpause(uint64_t _state)
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

ACTION battletest::resultgacha(eosio::name _from, eosio::name _to, std::string _result)
{
    require_auth(_self);
    require_recipient(_from);
}

ACTION battletest::resultpre(eosio::name _from, eosio::name _to, std::string _result)
{
    require_auth(_self);
    require_recipient(_from);
}

#pragma endresion

#pragma region party function

//------------------------------------------------------------------------//
//-------------------------------party_function---------------------------//
//------------------------------------------------------------------------//

ACTION battletest::setparty(eosio::name _user, uint32_t _party_number, const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list)
{
    require_auth(_user);
    eosio_assert(_party_number > 0, "wrong party_number");                                                   //잘못된 파티 넘버 체크
    user_partys user_party_table(_self, _user.value);                                                        //
    auto user_party_iter = user_party_table.find(_party_number);                                             //유저 파티 이터레이터는 파티 테이블 번호
    eosio_assert(user_party_iter != user_party_table.end(), "not exist party");                              //유저 파티 이터레이터를 기준으로 테이블 조회 시 없으면
    eosio_assert(user_party_iter->state != party_state::on_tower_defense, "this party on tower unmodified"); //유저 파티 상태가 타워 디펜스중이 아닐시
    user_servants user_servant_table(_self, _user.value);
    user_monsters user_monster_table(_self, _user.value);

    for (uint32_t i = 0; i < 4; ++i)
    {
        if (user_party_iter->servant_list[i] != EMPTY_PARTY &&
            user_party_iter->servant_list[i] != _servant_list[i])
        {
            auto user_servant_iter = user_servant_table.find(user_party_iter->servant_list[i]);
            eosio_assert(user_servant_iter != user_servant_table.end(), "fatal party data mis");
            user_servant_table.modify(user_servant_iter, _self, [&](auto &set_party) {
                set_party.party_number = EMPTY_PARTY;
            });
        }
    }
    for (uint32_t i = 0; i < 5; ++i)
    {
        if (user_party_iter->monster_list[i] != EMPTY_PARTY &&
            user_party_iter->monster_list[i] != _monster_list[i])
        {
            auto user_monster_iter = user_monster_table.find(user_party_iter->monster_list[i]);
            eosio_assert(user_monster_iter != user_monster_table.end(), "fatal party data mis");
            user_monster_table.modify(user_monster_iter, _self, [&](auto &set_party) {
                set_party.party_number = EMPTY_PARTY;
            });
        }
    }
    user_party_table.modify(user_party_iter, _self, [&](auto &save_party) {
        for (uint32_t i = 0; i < 4; ++i) //서번트에 대한 파티 배치 처리
        {
            if (_servant_list[i] == EMPTY_PARTY)
            {
                save_party.servant_list[i] = _servant_list[i];
                continue;
            }
            auto user_servant_iter = user_servant_table.find(_servant_list[i]);
            eosio_assert(user_servant_iter != user_servant_table.end(), "not exist servant data");
            eosio_assert(user_servant_iter->party_number == EMPTY_PARTY || user_servant_iter->party_number == _party_number, "already in party member servant");
            user_servant_table.modify(user_servant_iter, owner, [&](auto &set_party) {
                set_party.party_number = _party_number;
            });
            save_party.servant_list[i] = _servant_list[i];
        }
        for (uint32_t i = 0; i < 5; ++i)
        {
            if (_monster_list[i] == EMPTY_PARTY)
            {
                save_party.monster_list[i] = _monster_list[i];
                continue;
            }

            auto user_monster_iter = user_monster_table.find(_monster_list[i]);
            eosio_assert(user_monster_iter != user_monster_table.end(), "not exist monster data");
            eosio_assert(user_monster_iter->party_number == EMPTY_PARTY || user_monster_iter->party_number == _party_number, "already in party member monster");
            user_monster_table.modify(user_monster_iter, owner, [&](auto &set_party) {
                set_party.party_number = _party_number;
            });
            save_party.monster_list[i] = _monster_list[i];
        }
    });
}

void battletest::add_party_list(eosio::name _user)
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
        new_party.servant_list.resize(4);
        new_party.monster_list.resize(5);
    });
}
#pragma region party cheat
ACTION battletest::herocheat(eosio::name _user)
{
    require_auth(_user);
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
        new_hero.job = 1;

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
        new_party.servant_list.resize(4);
        new_party.monster_list.resize(5);
    });
}

ACTION battletest::partycheat(eosio::name _user)
{
    require_auth(_user);
    uint64_t user_value = get_user_seed_value(_user.value);
    uint64_t seed = safeseed::get_seed_value(user_value, now());

    user_consumables user_consumables_table(_self, _user.value);
    user_consumables_table.emplace(_self, [&](auto &new_consumables) {
        new_consumables.id = 40001;
        new_consumables.type = 1;
        new_consumables.count = 100;
    });

    for (uint32_t i = 0; i < 4; ++i)
    {
        gacha_servant_id(_user, seed);
    }
    for (uint32_t i = 0; i < 5; ++i)
    {
        gacha_monster_id(_user, seed);
    }
    for (uint32_t i = 0; i < 10; ++i)
    {
        gacha_item_id(_user, seed);
    }

    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.find(1);
    if (user_party_iter == user_party_table.end())
    {
        user_party_table.emplace(_self, [&](auto &automatic_party) {
            automatic_party.index = 1;
            for (uint32_t i = 0; i < 4; ++i)
            {
                automatic_party.servant_list[i] = i + 1;
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                automatic_party.monster_list[i] = i + 1;
            }
        });
    }
    else
    {
        user_party_table.modify(user_party_iter, _self, [&](auto &automatic_party) {
            for (uint32_t i = 0; i < 4; ++i)
            {
                automatic_party.servant_list[i] = i + 1;
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                automatic_party.monster_list[i] = i + 1;
            }
        });
    }
}

#pragma endregion

#pragma region item equipment function
//------------------------------------------------------------------------//
//-------------------------item_equipment_function------------------------//
//------------------------------------------------------------------------//

#pragma endregion

ACTION battletest::setdata()
{
    require_auth(_self);

    body_db body_table("overflow1111"_n, "overflow1111"_n.value);
    for (auto iter = body_table.begin(); iter != body_table.end();)
    {
        const auto &data_iter = body_table.get(iter->primary_key(), "Not Exist Data");
        insert_body(data_iter.body);
        iter++;
    }

    hair_db hair_table("overflow1111"_n, "overflow1111"_n.value);
    for (auto iter = hair_table.begin(); iter != hair_table.end();)
    {
        const auto &data_iter = hair_table.get(iter->primary_key(), "Not Exist Data");
        insert_hair(data_iter.hair);
        iter++;
    }

    head_db head_table("overflow1111"_n, "overflow1111"_n.value);
    for (auto iter = head_table.begin(); iter != head_table.end();)
    {
        const auto &data_iter = head_table.get(iter->primary_key(), "Not Exist Data");
        insert_head(data_iter.head);
        iter++;
    }

    gender_db gender_table("overflow1111"_n, "overflow1111"_n.value);
    for (auto iter = gender_table.begin(); iter != gender_table.end();)
    {
        const auto &data_iter = gender_table.get(iter->primary_key(), "Not Exist Data");
        insert_gender(data_iter.gender);
        iter++;
    }

    item_grade_db item_grade_table("overflow1111"_n, "overflow1111"_n.value);
    for (auto iter = item_grade_table.begin(); iter != item_grade_table.end();)
    {
        const auto &data_iter = item_grade_table.get(iter->primary_key(), "Not Exist Data");
        insert_item_grade(std::string("all"), data_iter.grade, 0, 10);
        iter++;
    }

    monster_grade_db monster_grade_table("overflow1111"_n, "overflow1111"_n.value);
    for (auto iter = monster_grade_table.begin(); iter != monster_grade_table.end();)
    {
        const auto &data_iter = monster_grade_table.get(iter->primary_key(), "Not Exist Data");
        insert_monster_grade(std::string("all"), data_iter.grade, 0, 10);
        iter++;
    }

    servant_job_db servant_job_table("overflow1111"_n, "overflow1111"_n.value);
    for (auto iter = servant_job_table.begin(); iter != servant_job_table.end();)
    {
        const auto &data_iter = servant_job_table.get(iter->primary_key(), "Not Exist Data");
        insert_job(std::string("all"), data_iter.job, 0, 10);
        iter++;
    }

    monster_db monster_table("overflow1111"_n, "overflow1111"_n.value);
    for (auto iter = monster_table.begin(); iter != monster_table.end();)
    {
        const auto &data_iter = monster_table.get(iter->primary_key(), "Not Exist Data");
        insert_monster_id(data_iter.monster_id, data_iter.gacha_id, data_iter.type, data_iter.main_status, 0, 0, 0);
        iter++;
    }

    servnt_db servant_table("overflow1111"_n, "overflow1111"_n.value);
    for (auto iter = servant_table.begin(); iter != servant_table.end();)
    {
        const auto &data_iter = servant_table.get(iter->primary_key(), "Not Exist Data");
        insert_servant_id(data_iter.servant_id, data_iter.gacha_id);
        iter++;
    }

    commonitem_db common_item_table("overflow1111"_n, "overflow1111"_n.value);
    for (auto iter = common_item_table.begin(); iter != common_item_table.end();)
    {
        const auto &data_iter = common_item_table.get(iter->primary_key(), "Not Exist Data");
        insert_common_item_id(data_iter.item_id,
                              data_iter.gacha_id,
                              data_iter.type,
                              data_iter.param_1,
                              data_iter.param_2,
                              data_iter.param_3,
                              data_iter.sell_id,
                              data_iter.sell_cost);
        iter++;
    }

    equipitem_db equip_item_table("overflow1111"_n, "overflow1111"_n.value);
    for (auto iter = equip_item_table.begin(); iter != equip_item_table.end();)
    {
        const auto &data_iter = equip_item_table.get(iter->primary_key(), "Not Exist Data");
        insert_equip_item_id(data_iter.item_id,
                             data_iter.set_id,
                             data_iter.job,
                             data_iter.type,
                             data_iter.tier,
                             data_iter.option,
                             0,
                             0,
                             0,
                             0);
        iter++;
    }
    // user_consumables_db other_consumables_db_table("unlimitedmas"_n, "unlimitedmas"_n.value);
    // for(auto iter11 = other_consumables_db_table.begin(); iter11 != other_consumables_db_table.end();)
    // {
    //     const auto &user_consumables_db_iter = other_consumables_db_table.get(iter11->primary_key(), "not exist data");
    //     insert_consumables_id(user_consumables_db_iter.id, user_consumables_db_iter.type, user_consumables_db_iter.price);
    //     iter11++;
    // }

    // lv_exp other_lv_exp_table("unlimitedmas"_n, "unlimitedmas"_n.value);
    // for(auto iter15 = other_lv_exp_table.begin(); iter15 != other_lv_exp_table.end();)
    // {
    //     const auto &lv_exp_iter = other_lv_exp_table.get(iter15->primary_key(), "nost exist data");
    //     insert_level(lv_exp_iter.lv);
    //     iter15++;
    // }

    // upgrade_item_ratio_db upgrade_item_ratio_db_table("unlimitedmas"_n, "unlimitedmas"_n.value);
    // for(auto iter = upgrade_item_ratio_db_table.begin(); iter != upgrade_item_ratio_db_table.end();)
    // {
    //     const auto &upgrade_item_iter = upgrade_item_ratio_db_table.get(iter->primary_key(), "nost exist data");
    //     insert_upgrade_item_ratio(upgrade_item_iter.upgrade_item, upgrade_item_iter.material, upgrade_item_iter.ratio);
    //     iter++;
    // }

    // upgrade_monster_ratio_db upgrade_monster_ratio_db_table("unlimitedmas"_n, "unlimitedmas"_n.value);
    // for (auto iter2 = upgrade_monster_ratio_db_table.begin(); iter2 != upgrade_monster_ratio_db_table.end();)
    // {
    //     const auto &upgrade_monster_iter = upgrade_monster_ratio_db_table.get(iter2->primary_key(), "nost exist data");
    //     insert_upgrade_monster_ratio(upgrade_monster_iter.main_monster);
    //     iter2++;
    // }
}

#pragma region battle function
//------------------------------------------------------------------------//
//---------------------------------battle_function------------------------//
//------------------------------------------------------------------------//

uint32_t battletest::get_attack(uint32_t _job, status_info _status)
{
    uint32_t attack = 0;
    if (_job == job_list::warrior || _job == job_list::beginner)
    {
        attack = (_status.basic_str + _status.plus_str) * oper_attack;
        return attack;
    }
    else if (_job == job_list::archer || _job == job_list::thief)
    {
        attack = (_status.basic_dex + _status.plus_dex) * oper_attack;
        return attack;
    }
    else if (_job == job_list::wizard || _job == job_list::priest)
    {
        attack = (_status.basic_int + _status.plus_int) * oper_attack;
        return attack;
    }
    else
    {
        return attack;
    }
}

uint32_t battletest::get_speed(uint32_t _job)
{
    if (_job == job_list::warrior)
    {
        return warrior_speed;
    }
    else if (_job == job_list::archer)
    {
        return archer_speed;
    }
    else if (_job == job_list::wizard)
    {
        return wizard_speed;
    }
    else if (_job == job_list::priest)
    {
        return priest_speed;
    }
    else if (_job == job_list::beginner)
    {
        return beginner_speed;
    }
    else if (_job == job_list::thief)
    {
        return thief_speed;
    }
    else
    {
        return 0;
    }
}

uint64_t battletest::get_damage(uint32_t _atk, uint32_t _dfs)
{
    uint32_t damage = (_atk * ((defense_constant * decimal) / (defense_constant + _dfs))) / decimal;
    return damage;
}

battletest::battle_state battletest::get_stage_state(status_info _status, uint64_t _job, uint64_t _index, uint64_t _id, uint64_t _position)
{
    battle_state get_state;
    get_state.now_hp = (_status.basic_str + _status.plus_str) * oper_hp;
    if (get_state.now_hp == 0)
    {
        get_state.now_hp = 10;
    }
    get_state.attack = get_attack(_job, _status);
    get_state.defense = (_status.basic_dex + _status.plus_dex) * oper_defense;
    get_state.crit_per = oper_critical;
    get_state.crit_dmg = get_attack(_job, _status) * oper_critical_damage / 10000;
    get_state.avoid = 5;
    get_state.speed = get_speed(_job);
    get_state.index = _index;
    get_state.id = _id;
    get_state.position = _position;
    get_state.state = battle_action_state::live;
    return get_state;
}

bool battletest::check_critical(uint64_t _critcal_per, uint64_t _seed)
{
    uint64_t rand_critical_percent = safeseed::get_random_value(_seed, 100, 0, 0);
    if (_critcal_per < rand_critical_percent)
    {
        return false;
    }
    else
    {
        return true;
    }
}

bool battletest::check_avoid(uint64_t _avoid_per, uint64_t _seed)
{
    uint64_t rand_avoid_percent = safeseed::get_random_value(_seed, 100, 0, 0);
    if (_avoid_per < rand_avoid_percent)
    {
        return false;
    }
    else
    {
        return true;
    }
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

ACTION battletest::startbattle(eosio::name _user, uint32_t _party_number, uint32_t _stage)
{
    require_auth(_user);

    whitelist whitelist_table(_self, _self.value);
    auto whitelist_iter = whitelist_table.find(_user.value);
    eosio_assert(whitelist_iter != whitelist_table.end(), "Not White User 3");

#pragma region stage test
    // stage_db stage_db_table(owner, owner.value);
    // const auto &stage_iter = stage_db_table.get(_stage, "not exist stage info");
#pragma endregion
    std::vector<uint32_t> servant_pos_list = {1, 2, 3, 4};
    std::vector<uint32_t> monster_pos_list = {5, 6, 7, 8, 9};
    std::vector<uint32_t> enemy_pos_list = {14, 12, 10, 11, 13, 19, 17, 15, 16, 18};

    auth_users user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Not Exist User 3");
    eosio_assert(user_auth_iter->state != user_state::stage && user_auth_iter->state != user_state::tower, "Already Battle");
    user_auth_table.modify(user_auth_iter, _self, [&](auto &user_state_change) {
        user_state_change.state = user_state::stage;
    });

    user_partys user_party_table(_self, _user.value);
    const auto &user_party_iter = user_party_table.get(_party_number, "Not Exist Party 1");
    eosio_assert(user_party_iter.state != party_state::on_tower_defense, "Party Impossible Battle");

    user_servants user_servant_table(_self, _user.value);
    user_monsters user_monster_table(_self, _user.value);

    battle_actions user_battle_action_table(_self, _self.value);
    auto user_battle_action_iter = user_battle_action_table.find(_user.value);
    if (user_battle_action_iter == user_battle_action_table.end())
    {
        user_battle_action_table.emplace(_self, [&](auto &new_battle_action) {
            new_battle_action.user = _user;
            new_battle_action.turn = START_BATTLE;
            new_battle_action.battle_info_list.clear();
        });
    }
    else
    {
        user_battle_action_table.modify(user_battle_action_iter, _self, [&](auto &new_battle_action) {
            new_battle_action.turn = START_BATTLE;
            new_battle_action.battle_info_list.clear();
        });
    }

    battle_state_list user_battle_table(_self, _self.value);
    auto user_battle_iter = user_battle_table.find(_user.value);
    if (user_battle_iter == user_battle_table.end())
    {
        std::vector<uint64_t> random_list;
        safeseed::get_battle_rand_list(random_list, now() + _user.value);
        user_battle_table.emplace(_self, [&](auto &new_battle_set) {
            new_battle_set.user = _user;
            new_battle_set.stage_type = 1;
            new_battle_set.enemy_user = _user;
            new_battle_set.stage_number = _stage;
            new_battle_set.state = stage_state::start;

            uint32_t servant_id_index = get_servant_index(user_auth_iter->hero.job, user_auth_iter->hero.appear.body, user_auth_iter->hero.appear.gender, user_auth_iter->hero.appear.head, user_auth_iter->hero.appear.hair);
            battle_state hero_battle_state = get_stage_state(user_auth_iter->hero.status, user_auth_iter->hero.job, 0, servant_id_index, 0);
            new_battle_set.my_state_list.push_back(hero_battle_state);

            uint32_t party_pos = 0;

            for (uint32_t i = 0; i < 4; ++i)
            {
                if (user_party_iter.servant_list[i] == 0) //파티 멤버가 비어있으면
                {
                    continue;
                }
                const auto &user_servant_iter = user_servant_table.get(user_party_iter.servant_list[i], "Not Exist Servant 1");
                uint32_t servant_id_index = get_servant_index(user_servant_iter.servant.job, user_servant_iter.servant.appear.body, user_servant_iter.servant.appear.gender, user_servant_iter.servant.appear.head, user_servant_iter.servant.appear.hair);
                battle_state servant_battle_state = get_stage_state(user_servant_iter.servant.status, user_servant_iter.servant.job, user_servant_iter.index, servant_id_index, servant_pos_list[i]);
                new_battle_set.my_state_list.push_back(servant_battle_state);
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter.monster_list[i] == 0) //파티 멤버가 비어있으면
                {
                    continue;
                }
                const auto &user_monster_iter = user_monster_table.get(user_party_iter.monster_list[i], "Not Exist Monster 1");
                battle_state monster_battle_state = get_stage_state(user_monster_iter.monster.status, beginner, user_monster_iter.index, user_monster_iter.monster.id, monster_pos_list[i]);
                new_battle_set.my_state_list.push_back(monster_battle_state);
            }
            uint32_t count = 0;
            uint64_t enemy_count = safeseed::get_random_value(random_list[count], 11, 1, 0);

            for (uint32_t i = 0; i < enemy_count; ++i)
            {
                uint64_t enemy_index = safeseed::get_random_value(random_list[count], 64, 1, 0);
                enemy_index += MONSTER_GACHA_ID_START;
                monster_db monster_id_db_table(_self, _self.value);
                auto monster_second = monster_id_db_table.get_index<"second"_n>();
                auto monster_id_iter = monster_second.find(enemy_index);
                uint64_t enemy_attack = 50000;
                count++;
                battle_state monster_battle_state;
                monster_battle_state.now_hp = 100;
                monster_battle_state.defense = 10;
                monster_battle_state.crit_per = 10;
                monster_battle_state.crit_dmg = enemy_attack * oper_critical_damage / 10000;
                monster_battle_state.avoid = 10;
                monster_battle_state.attack = enemy_attack;
                monster_battle_state.speed = beginner_speed;
                monster_battle_state.index = 0;
                monster_battle_state.id = monster_id_iter->monster_id;
                monster_battle_state.position = enemy_pos_list[i];
                monster_battle_state.state = battle_action_state::live;
                new_battle_set.enemy_state_list.push_back(monster_battle_state);
            }
        });
    }
    else
    {
        std::vector<uint64_t> random_list;
        safeseed::get_battle_rand_list(random_list, now() + _user.value);
        user_battle_table.modify(user_battle_iter, _self, [&](auto &new_battle_set) {
            new_battle_set.stage_type = 1;
            new_battle_set.enemy_user = _user;
            new_battle_set.stage_number = _stage;
            new_battle_set.state = stage_state::start;
            new_battle_set.my_state_list.clear();
            new_battle_set.enemy_state_list.clear();

            uint32_t servant_id_index = get_servant_index(user_auth_iter->hero.job, user_auth_iter->hero.appear.body, user_auth_iter->hero.appear.gender, user_auth_iter->hero.appear.head, user_auth_iter->hero.appear.hair);
            battle_state hero_battle_state = get_stage_state(user_auth_iter->hero.status, user_auth_iter->hero.job, 0, servant_id_index, 0);
            new_battle_set.my_state_list.push_back(hero_battle_state);

            uint32_t party_pos = 0;

            for (uint32_t i = 0; i < 4; ++i)
            {
                if (user_party_iter.servant_list[i] == 0) //파티 멤버가 비어있으면
                {
                    continue;
                }
                const auto &user_servant_iter = user_servant_table.get(user_party_iter.servant_list[i], "Not Exist Servant 1");
                uint32_t servant_id_index = get_servant_index(user_servant_iter.servant.job, user_servant_iter.servant.appear.body, user_servant_iter.servant.appear.gender, user_servant_iter.servant.appear.head, user_servant_iter.servant.appear.hair);
                battle_state servant_battle_state = get_stage_state(user_servant_iter.servant.status, user_servant_iter.servant.job, user_servant_iter.index, servant_id_index, servant_pos_list[i]);
                new_battle_set.my_state_list.push_back(servant_battle_state);
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter.monster_list[i] == 0) //파티 멤버가 비어있으면
                {
                    continue;
                }
                const auto &user_monster_iter = user_monster_table.get(user_party_iter.monster_list[i], "Not Exist Monster 1");
                battle_state monster_battle_state = get_stage_state(user_monster_iter.monster.status, beginner, user_monster_iter.index, user_monster_iter.monster.id, monster_pos_list[i]);
                new_battle_set.my_state_list.push_back(monster_battle_state);
            }

            uint32_t count = 0;
            uint64_t enemy_count = safeseed::get_random_value(random_list[count], 11, 1, 0);
            eosio_assert(enemy_count != 0, "error");
            for (uint32_t i = 0; i < enemy_count; ++i)
            {
                uint64_t enemy_index = safeseed::get_random_value(random_list[count], 64, 1, 0);
                enemy_index += MONSTER_GACHA_ID_START;
                monster_db monster_id_db_table(_self, _self.value);
                auto monster_second = monster_id_db_table.get_index<"second"_n>();
                auto monster_id_iter = monster_second.find(enemy_index);
                //uint64_t enemy_attack = safeseed::get_random_value(random_list[count], 100000, 1000, 0);
                uint64_t enemy_attack = 50000;
                count++;

                battle_state monster_battle_state;
                monster_battle_state.now_hp = 50;
                monster_battle_state.defense = 10;
                monster_battle_state.crit_per = 10;
                monster_battle_state.crit_dmg = enemy_attack * oper_critical_damage / 10000;
                monster_battle_state.avoid = 10;
                monster_battle_state.attack = enemy_attack;
                monster_battle_state.speed = beginner_speed;
                monster_battle_state.index = 0;
                monster_battle_state.id = monster_id_iter->monster_id;
                monster_battle_state.position = enemy_pos_list[i];
                monster_battle_state.state = battle_action_state::live;
                new_battle_set.enemy_state_list.push_back(monster_battle_state);
            }
        });
    }
}

#pragma endregion

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#pragma region active turn
int battletest::get_random_target(const std::vector<battle_state> &_enemy_state_list, uint64_t _seed, uint32_t _max, uint32_t _min)
{
    int target_key = safeseed::get_random_value(_seed, _max, _min, 0);
    if (_enemy_state_list[target_key].now_hp == 0 || _enemy_state_list[target_key].state == battle_action_state::dead)
    {
        target_key = -1;
        for (uint32_t i = _min; i < _max; i++)
        {
            if (_enemy_state_list[i].now_hp != 0 && _enemy_state_list[target_key].state != battle_action_state::dead)
            {
                target_key = i;
                break;
            }
        }
    }
    return target_key;
}

bool battletest::sort_compare(const battle_order_struct &a, const battle_order_struct &b)
{
    if (a.speed > b.speed)
    {
        return true;
    }
    else if (a.speed == b.speed)
    {
        return a.second_speed > b.second_speed;
    }
    else
    {
        return false;
    }
}

int battletest::get_target_key(const std::vector<battle_state> &_enemy_state_list, uint64_t _target_position)
{
    int target_key = -1;
    for (uint32_t i = 0; i < _enemy_state_list.size(); ++i)
    {
        if (_enemy_state_list[i].position == _target_position)
        {
            target_key = i;
            return target_key;
        }
    }
    return target_key;
}

battletest::battle_action battletest::get_target_action(const std::vector<battle_state> &_my_state_list, const std::vector<battle_state> &_enemy_state_list, uint64_t _seed, uint64_t _my_key, uint64_t _target_key)
{
    battle_action new_action;
    if (true == check_avoid(_enemy_state_list[_target_key].avoid, _seed))
    {
        new_action.target_position = _enemy_state_list[_target_key].position;
        new_action.avoid = 1;
        new_action.critical = 0;
        new_action.damage = 0;
    }
    else
    {
        uint32_t cur_damage;
        if (false == check_critical(_my_state_list[_my_key].crit_per, _seed))
        {
            if (_enemy_state_list[_target_key].state == battle_action_state::defense)
            {
                cur_damage = get_damage(_my_state_list[_my_key].attack, _enemy_state_list[_target_key].defense + (_enemy_state_list[_target_key].defense / 2));
            }
            else
            {
                cur_damage = get_damage(_my_state_list[_my_key].attack, _enemy_state_list[_target_key].defense);
            }

            new_action.target_position = _enemy_state_list[_target_key].position;
            new_action.avoid = 0;
            new_action.critical = 0;
            new_action.damage = cur_damage;
        }
        else
        {
            if (_enemy_state_list[_target_key].state == battle_action_state::defense)
            {
                cur_damage = get_damage(_my_state_list[_my_key].crit_dmg, _enemy_state_list[_target_key].defense + (_enemy_state_list[_target_key].defense / 2));
            }
            else
            {
                cur_damage = get_damage(_my_state_list[_my_key].crit_dmg, _enemy_state_list[_target_key].defense);
            }
            new_action.target_position = _enemy_state_list[_target_key].position;
            new_action.avoid = 0;
            new_action.critical = 1;
            new_action.damage = cur_damage;
        }
    }

    return new_action;
}

battletest::battle_action_info battletest::get_action_info(uint64_t _my_pos, uint64_t _action_type, battle_action _action)
{
    battle_action_info action_info;
    action_info.my_position = _my_pos;
    action_info.action_type = _action_type;
    if (_action_type == battle_action_state::attack)
    {
        action_info.battle_action_list.push_back(_action);
    }
    return action_info;
}

ACTION battletest::activeturn(eosio::name _user, uint32_t _hero_action, uint32_t _monster_action, uint32_t _hero_target, uint32_t _monster_target, std::string _seed)
{
    require_auth(_user);
    whitelist whitelist_table(_self, _self.value);
    auto whitelist_iter = whitelist_table.find(_user.value);
    eosio_assert(whitelist_iter != whitelist_table.end(), "Not White User 4");
    //함수로 뺴기
    size_t center = _seed.find(':');
    size_t end = _seed.length() - (center + 1);
    eosio_assert(_seed.find(':') != std::string::npos, "Wrong Seed Error");

    std::string result_seed = _seed.substr(0, center);
    std::string result_sha = _seed.substr(center + 1, end);

    uint64_t check_result = safeseed::check_seed(result_seed, result_sha);
    uint64_t user = get_user_seed_value(_user.value);
    uint64_t battle_seed = safeseed::get_seed_value(user, check_result);

    uint32_t user_dead_count = 0;
    uint32_t enemy_dead_count = 0;

    battle_state_list battle_state_list_table(_self, _self.value);
    auto user_battle_state_iter = battle_state_list_table.find(_user.value);
    eosio_assert(user_battle_state_iter != battle_state_list_table.end(), "End Battle 1");

    battle_actions battle_action_table(_self, _self.value);
    auto user_battle_action_iter = battle_action_table.find(_user.value);
    eosio_assert(user_battle_action_iter != battle_action_table.end(), "End Battle 2");

    std::vector<uint64_t> order_random_list;
    safeseed::get_battle_rand_list(order_random_list, battle_seed);

    //배틀의 상태를 바꿔주는 부분
    battle_state_list_table.modify(user_battle_state_iter, _self, [&](auto &battle_state) {
        //공격순서 정렬하는 부분
        std::vector<battle_order_struct> defense_order_list;
        std::vector<battle_order_struct> attack_order_list;
        for (uint32_t i = 0; i < user_battle_state_iter->my_state_list.size(); ++i)
        {
            if ((user_battle_state_iter->my_state_list[i].state == battle_action_state::dead) || ((user_battle_state_iter->my_state_list[i].index == 0) && (i != 0)))
            {
                continue;
            }
            battle_order_struct new_order;
            new_order.speed = user_battle_state_iter->my_state_list[i].speed;
            new_order.position = user_battle_state_iter->my_state_list[i].position;
            new_order.key = i;
            new_order.second_speed = order_random_list[i];
            defense_order_list.push_back(new_order);
        }
        for (uint32_t i = 0; i < user_battle_state_iter->enemy_state_list.size(); ++i)
        {
            if ((user_battle_state_iter->enemy_state_list[i].state == battle_action_state::dead))
            {
                continue;
            }
            battle_order_struct new_order;
            new_order.speed = user_battle_state_iter->enemy_state_list[i].speed;
            new_order.position = user_battle_state_iter->enemy_state_list[i].position;
            new_order.key = i;
            new_order.second_speed = order_random_list[i + 10];
            defense_order_list.push_back(new_order);
        }
        std::sort(defense_order_list.begin(), defense_order_list.end(), sort_compare);
        //---------------------

        //배틀에 액션테이블에 데이터를 추가해주는 부분ㅛㅣ
        battle_action_table.modify(user_battle_action_iter, _self, [&](auto &update_action) {
            update_action.turn += 1;
            update_action.battle_info_list.clear();
            //방어하는 애들 먼저 처리
            for (uint32_t i = 0; i < defense_order_list.size(); ++i)
            {
                uint32_t my_key = defense_order_list[i].key;
                battle_action new_action;
                battle_action_info new_action_info;

                if (defense_order_list[i].position < max_party_count)
                {
                    if (user_battle_state_iter->my_state_list[my_key].position == HERO_LOCATION) //히어로 처리
                    {
                        if (_hero_action != battle_action_state::defense)
                        {
                            attack_order_list.push_back(defense_order_list[i]);
                            continue;
                        }
                    }
                    else if (user_battle_state_iter->my_state_list[my_key].position == PAIR_SLOT) //히어로의 페어 몬스터일 경우
                    {
                        if (_monster_action != battle_action_state::defense)
                        {
                            attack_order_list.push_back(defense_order_list[i]);
                            continue;
                        }
                    }
                    else
                    {
                        uint32_t monster_action = safeseed::get_random_value(defense_order_list[i].second_speed, battle_action_state::state_count, battle_action_state::attack, 0);
                        if (monster_action != battle_action_state::defense)
                        {
                            attack_order_list.push_back(defense_order_list[i]);
                            continue;
                        }
                    }
                    new_action_info = get_action_info(user_battle_state_iter->my_state_list[my_key].position, battle_action_state::defense, new_action);
                    update_action.battle_info_list.push_back(new_action_info);
                    battle_state.my_state_list[my_key].state = battle_action_state::defense;
                }
                else
                {
                    uint32_t monster_action = safeseed::get_random_value(defense_order_list[i].second_speed, battle_action_state::state_count, battle_action_state::attack, 0);
                    if (monster_action != battle_action_state::defense)
                    {
                        attack_order_list.push_back(defense_order_list[i]);
                        continue;
                    }

                    new_action_info = get_action_info(user_battle_state_iter->enemy_state_list[my_key].position, battle_action_state::defense, new_action);
                    update_action.battle_info_list.push_back(new_action_info);
                    battle_state.enemy_state_list[my_key].state = battle_action_state::defense;
                }
            }

            //공격하는 애들 처리
            for (uint32_t i = 0; i < attack_order_list.size(); ++i)
            {
                uint32_t my_key = attack_order_list[i].key;
                if (attack_order_list[i].position < max_party_count) //자기 파티에 대한 처리
                {
                    if (user_battle_state_iter->my_state_list[my_key].position == HERO_LOCATION) //히어로 처리
                    {
                        int target_key = get_target_key(user_battle_state_iter->enemy_state_list, _hero_target);
                        eosio_assert(target_key != -1, "Wrong Target 1");
                        if (user_battle_state_iter->my_state_list[my_key].now_hp == 0)
                        {
                            continue;
                        }
                        else if (user_battle_state_iter->enemy_state_list[target_key].state == battle_action_state::dead)
                        {
                            int enemy_key = get_random_target(user_battle_state_iter->enemy_state_list, attack_order_list[i].second_speed, user_battle_state_iter->enemy_state_list.size(), 0);
                            if (enemy_key == -1) //상대 파티가 모두 죽은 상태
                            {
                                break;
                            }
                            target_key = enemy_key;
                        }
                        battle_action new_action = get_target_action(user_battle_state_iter->my_state_list, user_battle_state_iter->enemy_state_list, attack_order_list[i].second_speed, my_key, target_key);
                        if (user_battle_state_iter->enemy_state_list[target_key].now_hp <= new_action.damage)
                        {
                            battle_state.enemy_state_list[target_key].now_hp = 0;
                            battle_state.enemy_state_list[target_key].state = battle_action_state::dead;
                        }
                        else
                        {
                            battle_state.enemy_state_list[target_key].now_hp -= new_action.damage;
                        }
                        battle_action_info new_action_info = get_action_info(user_battle_state_iter->my_state_list[my_key].position, battle_action_state::attack, new_action);
                        update_action.battle_info_list.push_back(new_action_info);
                    }

                    else if (user_battle_state_iter->my_state_list[my_key].position == PAIR_SLOT) //히어로의 페어 몬스터일 경우
                    {
                        int target_key = get_target_key(user_battle_state_iter->enemy_state_list, _monster_target);
                        eosio_assert(target_key != -1, "Wrong Target 2");
                        if (user_battle_state_iter->my_state_list[my_key].now_hp == 0)
                        {
                            continue;
                        }
                        else if (user_battle_state_iter->enemy_state_list[target_key].now_hp == 0)
                        {
                            int enemy_key = get_random_target(user_battle_state_iter->enemy_state_list, attack_order_list[i].second_speed, user_battle_state_iter->enemy_state_list.size(), 0);
                            if (enemy_key == -1) //상대 파티가 모두 죽은 상태
                            {
                                break;
                            }
                            target_key = enemy_key;
                        }
                        //공격할 경우
                        battle_action new_action = get_target_action(user_battle_state_iter->my_state_list, user_battle_state_iter->enemy_state_list, attack_order_list[i].second_speed, my_key, target_key);
                        //배틀 스테이트테이블에 결과 반영
                        if (user_battle_state_iter->enemy_state_list[target_key].now_hp <= new_action.damage)
                        {
                            battle_state.enemy_state_list[target_key].now_hp = 0;
                            battle_state.enemy_state_list[target_key].state = battle_action_state::dead;
                        }
                        else
                        {
                            battle_state.enemy_state_list[target_key].now_hp -= new_action.damage;
                        }
                        battle_action_info new_action_info = get_action_info(user_battle_state_iter->my_state_list[my_key].position, battle_action_state::attack, new_action);
                        update_action.battle_info_list.push_back(new_action_info);
                    }
                    else //다른 몬스터의 경우
                    {
                        if (user_battle_state_iter->my_state_list[my_key].now_hp == 0)
                        {
                            continue;
                        }
                        int enemy_key = get_random_target(user_battle_state_iter->enemy_state_list, attack_order_list[i].second_speed, user_battle_state_iter->enemy_state_list.size(), 0);
                        if (enemy_key == -1) //상대 파티가 모두 죽은 상태
                        {
                            break;
                        }
                        //공격할 경우
                        battle_action new_action = get_target_action(user_battle_state_iter->my_state_list, user_battle_state_iter->enemy_state_list, attack_order_list[i].second_speed, my_key, enemy_key);
                        //배틀 스테이트테이블에 결과 반영
                        if (user_battle_state_iter->enemy_state_list[enemy_key].now_hp <= new_action.damage)
                        {
                            battle_state.enemy_state_list[enemy_key].now_hp = 0;
                            battle_state.enemy_state_list[enemy_key].state = battle_action_state::dead;
                        }
                        else
                        {
                            battle_state.enemy_state_list[enemy_key].now_hp -= new_action.damage;
                        }
                        battle_action_info new_action_info = get_action_info(user_battle_state_iter->my_state_list[my_key].position, battle_action_state::attack, new_action);
                        update_action.battle_info_list.push_back(new_action_info);
                    }
                    battle_state.my_state_list[my_key].state = battle_action_state::attack;
                }

                else // 상대 파티의 경우
                {
                    if (user_battle_state_iter->enemy_state_list[my_key].state == battle_action_state::dead)
                    {
                        continue;
                    }
                    int enemy_key = get_random_target(user_battle_state_iter->my_state_list, attack_order_list[i].second_speed, user_battle_state_iter->my_state_list.size(), 0);
                    if (enemy_key == -1) //상대 파티가 모두 죽은 상태
                    {
                        break;
                    }
                    battle_action new_action = get_target_action(user_battle_state_iter->enemy_state_list, user_battle_state_iter->my_state_list, attack_order_list[i].second_speed, my_key, enemy_key);
                    if (user_battle_state_iter->my_state_list[enemy_key].now_hp <= new_action.damage)
                    {
                        battle_state.my_state_list[enemy_key].now_hp = 0;
                        battle_state.my_state_list[enemy_key].state = battle_action_state::dead;
                    }
                    else
                    {
                        battle_state.my_state_list[enemy_key].now_hp -= new_action.damage;
                    }
                    battle_action_info new_action_info = get_action_info(user_battle_state_iter->enemy_state_list[my_key].position, battle_action_state::attack, new_action);
                    update_action.battle_info_list.push_back(new_action_info);
                    battle_state.enemy_state_list[my_key].state = battle_action_state::attack;
                }
            }
            //게임의 종료 여부 체크
            for (uint32_t i = 0; i < user_battle_state_iter->my_state_list.size(); ++i)
            {
                if (user_battle_state_iter->my_state_list[i].state == battle_action_state::dead)
                {
                    user_dead_count += 1;
                }
            }

            for (uint32_t i = 0; i < user_battle_state_iter->enemy_state_list.size(); ++i)
            {
                if (user_battle_state_iter->enemy_state_list[i].state == battle_action_state::dead)
                {
                    enemy_dead_count += 1;
                }
            }
        });
    });
    if (enemy_dead_count == user_battle_state_iter->enemy_state_list.size())
    {
        win_reward(_user);
    }
    else if (user_dead_count == user_battle_state_iter->my_state_list.size())
    {
        fail_reward(_user);
    }
}

bool battletest::check_level_up(uint64_t _cur_exp, uint64_t _pre_exp)
{
    lv_exp lv_exp_table(_self, _self.value);

    uint64_t level_up_line;
    for(auto iter = lv_exp_table.begin(); iter != lv_exp_table.end();)
    {
        if(iter->exp > _cur_exp)
        {
            level_up_line = iter->exp;
            break;
        }
        iter++;
    }

    if (level_up_line <= _pre_exp)
    {
        return true;
    }
    else
    {
        return false;
    }
}

void battletest::win_reward(eosio::name _user)
{
    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Not Exist Log 1");

    user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
        // update_log.last_stage_num = user_battle_iter->stage_number;
        // if (user_log_iter->top_clear_stage < user_battle_iter->stage_number)
        // {
        //     update_log.top_clear_stage = user_battle_iter->stage_number;
        // }
        update_log.battle_count += 1;
    });

    battle_state_list battle_state_list_table(_self, _self.value);
    auto user_battle_state_iter = battle_state_list_table.find(_user.value);
    eosio_assert(user_battle_state_iter != battle_state_list_table.end(), "End Battle 1");
    eosio_assert(user_battle_state_iter->state == stage_state::start, "Already Get Reward 1");
    battle_state_list_table.modify(user_battle_state_iter, _self, [&](auto &end_battle) {
        end_battle.state = stage_state::win;
    });

    battle_reward_list battle_reward_list_table(_self, _self.value);
    auto user_battle_reward_iter = battle_reward_list_table.find(_user.value);
    if (user_battle_reward_iter == battle_reward_list_table.end())
    {
        battle_reward_list_table.emplace(_self, [&](auto &set_reward) {
            set_reward.user = _user;
            set_reward.reward_money = 10000000;
            for (uint32_t i = 0; i < 10; ++i)
            {
                set_reward.get_exp_list.push_back(100);
            }
        });
    }
    else
    {
        battle_reward_list_table.modify(user_battle_reward_iter, _self, [&](auto &set_reward) {
            set_reward.reward_money = 10000000;
            set_reward.get_exp_list.clear();
            for (uint32_t i = 0; i < 10; ++i)
            {
                set_reward.get_exp_list.push_back(100);
            }
        });
    }

    //보상 반영
    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.begin();
    eosio_assert(user_party_iter != user_party_table.end(), "Not Exist Party");

    auth_users user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Not Exist Hero");
    uint64_t cur_exp = user_auth_iter->hero.exp + 100;
    uint64_t pre_exp = user_auth_iter->hero.exp;
    user_auth_table.modify(user_auth_iter, _self, [&](auto &upadate_hero_exp) {
        if (true == check_level_up(cur_exp, pre_exp))
        {
            upadate_hero_exp.hero.stat_point += 1;
            upadate_hero_exp.hero.exp += 100;
        }
        else{
            upadate_hero_exp.hero.exp += 100;
        }
        upadate_hero_exp.state = user_state::lobby;
    });

    lv_exp lv_exp_table(_self, _self.value);
    user_servants user_servant_table(_self, _user.value);
    for (uint32_t i = 0; i < 4; ++i)
    {
        if (user_party_iter->servant_list[i] == 0)
        {
            continue;
        }
        auto user_servant_iter = user_servant_table.find(user_party_iter->servant_list[i]);
        uint64_t cur_exp = user_servant_iter->servant.exp + 100;
        uint64_t pre_exp = user_servant_iter->servant.exp;
        user_servant_table.modify(user_servant_iter, _self, [&](auto &update_servant_exp) {
            if (true == check_level_up(cur_exp, pre_exp))
            {
                update_servant_exp.servant.stat_point += 1;
                update_servant_exp.servant.exp += 100;
            }
            else
            {
                update_servant_exp.servant.exp += 100;
            }
        });
    }

    user_monsters user_monster_table(_self, _user.value);
    for (uint32_t i = 0; i < 5; ++i)
    {
        if (user_party_iter->monster_list[i] == 0)
        {
            continue;
        }
        auto user_monster_iter = user_monster_table.find(user_party_iter->monster_list[i]);
        uint64_t cur_exp = user_monster_iter->monster.exp + 100;
        uint64_t pre_exp = user_monster_iter->monster.exp;
        user_monster_table.modify(user_monster_iter, _self, [&](auto &update_monster_exp) {
            if (true == check_level_up(cur_exp, pre_exp))
            {
                update_monster_exp.monster.exp += 100;
            }
            else
            {
                update_monster_exp.monster.exp += 100;
            }
        });
    }

    asset battle_result(0, symbol(symbol_code("UTG"), 4));
    battle_result.amount = 10000000;

    action(permission_level{get_self(), "active"_n},
           get_self(), "transfer"_n,
           std::make_tuple(_self, _user, battle_result, std::string("battle reward")))
        .send();
}

void battletest::fail_reward(eosio::name _user)
{
    auth_users user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Not Exist User 1");

    user_auth_table.modify(user_auth_iter, _self, [&](auto &user_state) {
        user_state.state = user_state::lobby;
    });

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Not Exist Log 1");

    user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
        // update_log.last_stage_num = user_battle_iter->stage_number;
        // if (user_log_iter->top_clear_stage < user_battle_iter->stage_number)
        // {
        //     update_log.top_clear_stage = user_battle_iter->stage_number;
        // }
        update_log.battle_count += 1;
    });

    battle_state_list battle_state_list_table(_self, _self.value);
    auto user_battle_state_iter = battle_state_list_table.find(_user.value);
    eosio_assert(user_battle_state_iter != battle_state_list_table.end(), "End Battle 1");
    eosio_assert(user_battle_state_iter->state == stage_state::start, "Already Get Reward 2");
    battle_state_list_table.modify(user_battle_state_iter, _self, [&](auto &end_battle) {
        end_battle.state = stage_state::lose;
    });

    battle_reward_list battle_reward_list_table(_self, _self.value);
    auto battle_reward_iter = battle_reward_list_table.find(_user.value);
    if (battle_reward_iter == battle_reward_list_table.end())
    {
        battle_reward_list_table.emplace(_self, [&](auto &set_reward) {
            set_reward.user = _user;
            set_reward.reward_money = 0;
            set_reward.get_exp_list.clear();
        });
    }
    else
    {
        battle_reward_list_table.modify(battle_reward_iter, _self, [&](auto &set_reward) {
            set_reward.reward_money = 0;
            set_reward.get_exp_list.clear();
        });
    }
}

ACTION battletest::exitbattle(eosio::name _user)
{
    require_auth(_user);

    auth_users user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Not Exist User 2");
    user_auth_table.modify(user_auth_iter, _self, [&](auto &update_user) {
        update_user.state = user_state::lobby;
    });

    battle_state_list battle_state_list_table(_self, _self.value);
    auto user_battle_state_iter = battle_state_list_table.find(_user.value);
    eosio_assert(user_battle_state_iter != battle_state_list_table.end(), "End Battle 1");
    battle_state_list_table.modify(user_battle_state_iter, _self, [&](auto &end_battle) {
        end_battle.state = stage_state::lose;
    });
}

#pragma endregion

#pragma region item equipment function
//------------------------------------------------------------------------//
//-------------------------item_equipment_function------------------------//
//------------------------------------------------------------------------//
ACTION battletest::sellobject(eosio::name _user, uint32_t _type, uint32_t _index)
{
    require_auth(_user);

    switch (_type)
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

void battletest::sell_servant(eosio::name _user, uint32_t _index)
{
    user_servants user_servant_table(_self, _user.value);
    auto user_servant_iter = user_servant_table.find(_index);
    user_consumables user_consumables_table(_self, _user.value);
    auto user_consumables_iter = user_consumables_table.find(40003);

    eosio_assert(user_servant_iter != user_servant_table.end(), "not exist servant info");
    eosio_assert(user_servant_iter->party_number == 0, "this servant already in party");

    if (user_consumables_iter == user_consumables_table.end())
    {
        user_consumables_table.emplace(_self, [&](auto &change_consumable) {
            change_consumable.id = 40003;
            change_consumable.type = 2;
            change_consumable.count = user_servant_iter->servant.exp + 2;
        });
    }

    else
    {
        user_consumables_table.modify(user_consumables_iter, _self, [&](auto &change_consumable) {
            change_consumable.id = 40003;
            change_consumable.type = 2;
            change_consumable.count += user_servant_iter->servant.exp + 2;
        });
    }

    asset servant_sell_result(0, symbol(symbol_code("UTG"), 4));
    servant_sell_result.amount = 1000000;

    action(permission_level{get_self(), "active"_n},
           get_self(), "transfer"_n,
           std::make_tuple(_self, _user, servant_sell_result, std::string("servant sell result")))
        .send();

    user_servant_table.erase(user_servant_iter);
}

void battletest::sell_monster(eosio::name _user, uint32_t _index)
{
    user_monsters user_monster_table(_self, _user.value);
    auto user_monster_iter = user_monster_table.find(_index);
    eosio_assert(user_monster_iter != user_monster_table.end(), "not exist servant info");
    eosio_assert(user_monster_iter->party_number == 0, "this servant already in party");

    asset monster_sell_result(0, symbol(symbol_code("UTG"), 4));
    monster_sell_result.amount = 1000000;

    action(permission_level{get_self(), "active"_n},
           get_self(), "transfer"_n,
           std::make_tuple(_self, _user, monster_sell_result, std::string("monster sell result")))
        .send();

    user_monster_table.erase(user_monster_iter);
}

void battletest::sell_item(eosio::name _user, uint32_t _index)
{
    user_items user_item_table(_self, _user.value);
    auto user_item_iter = user_item_table.find(_index);
    eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
    eosio_assert(user_item_iter->item.state != object_state::on_equip_slot, "item is equip");

    asset item_sell_result(0, symbol(symbol_code("UTG"), 4));
    item_sell_result.amount = 1000000;

    action(permission_level{get_self(), "active"_n},
           get_self(), "transfer"_n,
           std::make_tuple(_self, _user, item_sell_result, std::string("item sell result")))
        .send();

    user_item_table.erase(user_item_iter);
}

ACTION battletest::equipment(eosio::name _user, uint32_t _type, uint32_t _servant_index, uint32_t _item_index)
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
        eosio_assert(_type < 3, "not exsit this action type");
        break;
    }
}

ACTION battletest::unequipment(eosio::name _user, uint32_t _type, uint32_t _servant_index, uint32_t _item_index)
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
        eosio_assert(_type < 3, "not exsit this action type");
        break;
    }
}

void battletest::unequip_servant(eosio::name _user, uint32_t _servant_index, uint32_t _item_index)
{
    user_items user_item_table(_self, _user.value);
    auto user_item_iter = user_item_table.find(_item_index);
    eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
    eosio_assert(user_item_iter->item.state == object_state::on_equip_slot, "already unequip this item");

    uint32_t slot = user_item_iter->item.type - 1;
    user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item) {
        unequip_item.item.state = object_state::on_inventory;
    });

    user_servants user_servant_table(_self, _user.value);
    auto user_servant_iter = user_servant_table.find(_servant_index);
    eosio_assert(user_servant_iter != user_servant_table.end(), "not exist servant info");

    user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant) {
        unequip_servant.servant.status.plus_str -= (user_item_iter->item.status.basic_str + user_item_iter->item.status.plus_str);
        unequip_servant.servant.status.plus_dex -= (user_item_iter->item.status.basic_dex + user_item_iter->item.status.plus_dex);
        unequip_servant.servant.status.plus_int -= (user_item_iter->item.status.basic_int + user_item_iter->item.status.plus_int);
        unequip_servant.servant.equip_slot[slot] = 0;
    });
}

void battletest::equip_servant(eosio::name _user, uint32_t _servant_index, uint32_t _item_index)
{
    user_items user_item_table(_self, _user.value);
    auto user_item_iter = user_item_table.find(_item_index);
    user_servants user_servant_table(_self, _user.value);
    auto user_servant_iter = user_servant_table.find(_servant_index);

    eosio_assert(user_servant_iter != user_servant_table.end(), "not exist servant info");
    eosio_assert(user_item_iter != user_item_table.end(), "not exist item info1");
    eosio_assert(user_item_iter->item.state != object_state::on_equip_slot, "already equip this item");

    eosio_assert(compare_item(user_servant_iter->servant.job, user_item_iter->item.job), "this item is not equipment");

    uint32_t slot;
    if (user_item_iter->item.type >= 3)
    {
        slot = 2;
    }
    else
    {
        slot = user_item_iter->item.type - 1;
    }

    status_info item_status;
    if (user_servant_iter->servant.equip_slot[slot] != 0) //기존에 아이템 착용 상태 시
    {
        user_items user_item_table2(_self, _user.value);
        auto user_item_iter2 = user_item_table2.find(user_servant_iter->servant.equip_slot[slot]);
        user_item_table2.modify(user_item_iter2, _self, [&](auto &unequip_item) {
            unequip_item.item.state = object_state::on_inventory;
        });

        item_status = user_item_iter2->item.status;

        user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item) {
            unequip_item.item.state = object_state::on_equip_slot;
        });

        user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant) {
            unequip_servant.servant.status.plus_str += (user_item_iter->item.status.basic_str + user_item_iter->item.status.plus_str);
            unequip_servant.servant.status.plus_dex += (user_item_iter->item.status.basic_dex + user_item_iter->item.status.plus_dex);
            unequip_servant.servant.status.plus_int += (user_item_iter->item.status.basic_int + user_item_iter->item.status.plus_int);
            unequip_servant.servant.equip_slot[slot] = _item_index;
            unequip_servant.servant.status.plus_str -= (item_status.basic_str + item_status.plus_str);
            unequip_servant.servant.status.plus_dex -= (item_status.basic_dex + item_status.plus_dex);
            unequip_servant.servant.status.plus_int -= (item_status.basic_int + item_status.plus_int);
        });
    }
    else
    {
        user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item) {
            unequip_item.item.state = object_state::on_equip_slot;
        });

        user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant) {
            unequip_servant.servant.status.plus_str += (user_item_iter->item.status.basic_str + user_item_iter->item.status.plus_str);
            unequip_servant.servant.status.plus_dex += (user_item_iter->item.status.basic_dex + user_item_iter->item.status.plus_dex);
            unequip_servant.servant.status.plus_int += (user_item_iter->item.status.basic_int + user_item_iter->item.status.plus_int);
            unequip_servant.servant.equip_slot[slot] = _item_index;
        });
    }
}
void battletest::unequip_hero(eosio::name _user, uint32_t _item_index)
{
    user_items user_item_table(_self, _user.value);
    auto user_item_iter = user_item_table.find(_item_index);
    eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
    eosio_assert(user_item_iter->item.state == object_state::on_equip_slot, "already unequip this item");

    uint32_t slot = user_item_iter->item.type - 1;
    user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item) {
        unequip_item.item.state = object_state::on_inventory;
    });

    auth_users user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "not exist user info");

    user_auth_table.modify(user_auth_iter, _self, [&](auto &unequip_hero) {
        unequip_hero.hero.status.plus_str -= (user_item_iter->item.status.basic_str + user_item_iter->item.status.plus_str);
        unequip_hero.hero.status.plus_dex -= (user_item_iter->item.status.basic_dex + user_item_iter->item.status.plus_dex);
        unequip_hero.hero.status.plus_int -= (user_item_iter->item.status.basic_int + user_item_iter->item.status.plus_int);
        unequip_hero.hero.equip_slot[slot] = 0;
    });
}

void battletest::equip_hero(eosio::name _user, uint32_t _item_index)
{
    user_items user_item_table(_self, _user.value);
    auto user_item_iter = user_item_table.find(_item_index);
    auth_users user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
    eosio_assert(user_item_iter->item.state != object_state::on_equip_slot, "already equip this item");
    eosio_assert(user_auth_iter != user_auth_table.end(), "not exist user info");

    eosio_assert(compare_item(user_auth_iter->hero.job, user_item_iter->item.job), "this item is not equipment");

    uint32_t slot;
    if (user_item_iter->item.type >= 3)
    {
        slot = 2;
    }
    else
    {
        slot = user_item_iter->item.type - 1;
    }

    status_info item_status;
    if (user_auth_iter->hero.equip_slot[slot] != 0) //기존에 아이템 착용 상태 시
    {
        user_items user_item_table2(_self, _user.value);
        auto user_item_iter2 = user_item_table2.find(user_auth_iter->hero.equip_slot[slot]);
        user_item_table2.modify(user_item_iter2, _self, [&](auto &unequip_item) {
            unequip_item.item.state = object_state::on_inventory;
        });

        item_status = user_item_iter2->item.status;

        user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item) {
            unequip_item.item.state = object_state::on_equip_slot;
        });

        user_auth_table.modify(user_auth_iter, _self, [&](auto &unequip_hero) {
            unequip_hero.hero.status.plus_str += (user_item_iter->item.status.basic_str + user_item_iter->item.status.plus_str);
            unequip_hero.hero.status.plus_dex += (user_item_iter->item.status.basic_dex + user_item_iter->item.status.plus_dex);
            unequip_hero.hero.status.plus_int += (user_item_iter->item.status.basic_int + user_item_iter->item.status.plus_int);
            unequip_hero.hero.equip_slot[slot] = _item_index;
            unequip_hero.hero.status.plus_str -= (item_status.basic_str + item_status.plus_str);
            unequip_hero.hero.status.plus_dex -= (item_status.basic_dex + item_status.plus_dex);
            unequip_hero.hero.status.plus_int -= (item_status.basic_int + item_status.plus_int);
        });
    }
    else
    {
        user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item) {
            unequip_item.item.state = object_state::on_equip_slot;
        });

        user_auth_table.modify(user_auth_iter, _self, [&](auto &unequip_hero) {
            unequip_hero.hero.status.plus_str += (user_item_iter->item.status.basic_str + user_item_iter->item.status.plus_str);
            unequip_hero.hero.status.plus_dex += (user_item_iter->item.status.basic_dex + user_item_iter->item.status.plus_dex);
            unequip_hero.hero.status.plus_int += (user_item_iter->item.status.basic_int + user_item_iter->item.status.plus_int);
            unequip_hero.hero.equip_slot[slot] = _item_index;
        });
    }
}

bool battletest::compare_item(uint32_t _user_servant, uint32_t _user_item)
{
    uint32_t temp = _user_servant & _user_item;

    if (_user_item == 0)
    {
        return true;
    }
    if (temp == _user_servant)
    {
        return true;
    }
    else
    {
        return false;
    }
}

#pragma endregion

#pragma region upgrade function
//------------------------------------------------------------------------//
//-----------------------------upgrade_function---------------------------//
//------------------------------------------------------------------------//

ACTION battletest::upgrade(eosio::name _user, uint32_t _type, uint32_t _index, uint32_t _index2)
{
    require_auth(_user);
    switch (_type)
    {
    case 1:
        upgrade_monster(_user, _index, _index2);
        break;
    case 2:
        upgrade_item(_user, _index, _index2);
        break;
    default:
        eosio_assert(_type < 4, "not exsit this action type");
        break;
    }
}

void battletest::upgrade_monster(eosio::name _user, uint32_t _monster_index, uint32_t _monster_index2)
{
    user_monsters user_monster_table(_self, _user.value);
    auto user_monster_iter = user_monster_table.find(_monster_index);

    user_monsters second_user_monster_table(_self, _user.value);
    auto user_monster_iter2 = second_user_monster_table.find(_monster_index2);

    upgrade_monster_ratio_db upgrade_monsterratio_db_table(_self, _self.value);
    auto user_upgrade_monster_iter = upgrade_monsterratio_db_table.find(user_monster_iter->monster.upgrade);

    uint64_t db_ratio = 0;
    db_ratio = user_upgrade_monster_iter->sub[user_monster_iter2->monster.upgrade].ratio;

    eosio_assert(user_monster_iter != user_monster_table.end(), "not exist monster info");
    eosio_assert(user_monster_iter2 != second_user_monster_table.end(), "not exist monster info");

    eosio_assert(user_monster_iter->party_number == 0, "this monster already in party");
    eosio_assert(user_monster_iter2->party_number == 0, "this monster already in party");
    eosio_assert(user_monster_iter->monster.upgrade != 9, "this monster upgrade is MAX");
    eosio_assert(user_monster_iter2->monster.upgrade != 9, "this monster upgrade is MAX");
    eosio_assert(user_monster_iter->index != user_monster_iter2->index, "same monster not use");

    uint64_t l_seed = safeseed::get_seed_value(_user.value, now());
    uint64_t upgrade_ratio = safeseed::get_random_value(l_seed, MONSTER_UPGRADE_RATE, DEFAULT_MIN, DEFAULT_RANDOM_COUNT);
    uint64_t fail_upgrade = safeseed::get_random_value(l_seed, user_monster_iter->monster.upgrade + 1, DEFAULT_MIN, DEFAULT_RANDOM_COUNT);

    eosio_assert(user_monster_iter->monster.grade == user_monster_iter2->monster.grade, "sub monster grade is high by main monster");       //메인보다 서브가 높을 수 없다.
    eosio_assert(user_monster_iter->monster.upgrade >= user_monster_iter2->monster.upgrade, "sub monster upgrade is high by main monster"); //메인보다 서브가 높을 수 없다.

    user_monster_table.modify(user_monster_iter, _self, [&](auto &upgrade_monster) {
        //eosio_assert(user_monster_iter->monster->exp !=10, "this monster not MAX Lv");        //이건 각각 몬스터 포문돌릴 때
        if (db_ratio >= upgrade_ratio) // 성공 확률
        {
            upgrade_monster.monster.upgrade += 1;
            upgrade_monster.monster.status.plus_str += 1;
            upgrade_monster.monster.status.plus_dex += 1;
            upgrade_monster.monster.status.plus_int += 1;
            upgrade_monster.monster.exp = 0;
        }
        else //실패
        {
            upgrade_monster.monster.upgrade = fail_upgrade;
            upgrade_monster.monster.exp = 0;
        }
    });
    second_user_monster_table.erase(user_monster_iter2);
}

void battletest::upgrade_item(eosio::name _user, uint32_t _item, uint32_t _item2)
{
    user_items user_item_table(_self, _user.value);
    auto user_item_iter = user_item_table.find(_item);

    user_consumables user_consumables_table(_self, _user.value);
    auto user_consumables_iter = user_consumables_table.find(_item2);

    upgrade_item_ratio_db upgrade_item_ratio_db_table(_self, _self.value);
    auto user_upgrade_item_iter = upgrade_item_ratio_db_table.find(user_item_iter->item.upgrade);

    uint64_t db_ratio;
    db_ratio = user_upgrade_item_iter->ratio;

    eosio_assert(user_item_iter != user_item_table.end(), "not exist monster info");
    eosio_assert(user_consumables_iter != user_consumables_table.end(), "not exist consumables info");

    eosio_assert(user_item_iter->item.state == 1, "this item already in Wearing");
    eosio_assert(user_item_iter->item.upgrade != 9, "this item upgrade is MAX");

    uint64_t l_seed = safeseed::get_seed_value(_user.value, now());
    uint64_t upgrade_ratio = safeseed::get_random_value(l_seed, ITEM_UPGRADE_RATE, DEFAULT_MIN, DEFAULT_RANDOM_COUNT);
    uint64_t updatecheck = 0;

    user_item_table.modify(user_item_iter, _self, [&](auto &upgrade_item) {
        eosio_assert(user_consumables_iter->count >= user_upgrade_item_iter->material, "not enough material count");
        if (db_ratio >= upgrade_ratio) // 성공 확률
        {
            upgrade_item.item.upgrade += 1;
            upgrade_item.item.status.plus_str += 1;
            upgrade_item.item.status.plus_dex += 1;
            upgrade_item.item.status.plus_int += 1;
            updatecheck = 1;
        }
    });
    user_consumables_table.modify(user_consumables_iter, _self, [&](auto &change_consumables) {
        change_consumables.count -= user_upgrade_item_iter->material;
    });
    if (updatecheck == 0)
    {
        user_item_table.erase(user_item_iter);
    }
}

#pragma endregion

#pragma region store function
//------------------------------------------------------------------------//
//-------------------------------store_function---------------------------//
//------------------------------------------------------------------------//

ACTION battletest::itemstore(eosio::name _user, uint32_t _type, uint32_t _count)
{
    require_auth(_user);

    switch (_type)
    {
    case 1:
        buy_nomal_order(_user, _count);
        break;
    case 2:
        buy_blessing_order(_user, _count);
        break;
    default:
        eosio_assert(_type < 4, "not exsit this action type");
        break;
    }
}

void battletest::buy_nomal_order(eosio::name _user, uint32_t _count)
{
    user_consumables user_consumables_table(_self, _user.value);
    auto user_consumables_iter = user_consumables_table.find(40001);

    user_consumables_db user_consumables_db_table(_self, _user.value);
    auto user_consumables_db_iter = user_consumables_db_table.find(40001);
    auto iter = *user_consumables_db_iter;

    if (user_consumables_iter == user_consumables_table.end())
    {
        user_consumables_table.emplace(_self, [&](auto &change_consumable) {
            change_consumable.id = 40001;
            change_consumable.type = 1;
            change_consumable.count = _count;
        });
        asset nomal_order_buy_result(0, symbol(symbol_code("UTG"), 4));
        nomal_order_buy_result.amount = 1000000 * _count;

        transfer(_user, _self, nomal_order_buy_result, std::string("nomal order buy result"));
    }
    else
    {
        user_consumables_table.modify(user_consumables_iter, _self, [&](auto &change_consumable) {
            change_consumable.id = 40001;
            change_consumable.type = 1;
            change_consumable.count += _count;
        });
        asset nomal_order_buy_result(0, symbol(symbol_code("UTG"), 4));
        nomal_order_buy_result.amount = 1000000 * _count;

        transfer(_user, _self, nomal_order_buy_result, std::string("nomal order buy result"));
    }
}
void battletest::buy_blessing_order(eosio::name _user, uint32_t _count)
{
    user_consumables user_consumables_table(_self, _user.value);
    auto user_consumables_iter = user_consumables_table.find(40002);

    user_consumables_db user_consumables_db_table(_self, _user.value);
    auto user_consumables_db_iter = user_consumables_db_table.find(40002);

    if (user_consumables_iter == user_consumables_table.end())
    {
        user_consumables_table.emplace(_self, [&](auto &change_consumable) {
            change_consumable.id = 40002;
            change_consumable.type = 1;
            change_consumable.count = _count;
        });
        asset blessing_order_buy_result(0, symbol(symbol_code("UTG"), 4));
        blessing_order_buy_result.amount = user_consumables_db_iter->price * _count;

        transfer(_user, _self, blessing_order_buy_result, std::string("blessing order buy result"));
    }
    else
    {
        user_consumables_table.modify(user_consumables_iter, _self, [&](auto &change_consumable) {
            change_consumable.id = 40002;
            change_consumable.type = 1;
            change_consumable.count += _count;
        });
        asset blessing_order_buy_result(0, symbol(symbol_code("UTG"), 4));
        blessing_order_buy_result.amount = user_consumables_db_iter->price * _count;

        transfer(_user, _self, blessing_order_buy_result, std::string("blessing order buy result"));
    }
}

#pragma endregion

#pragma region dbstage

ACTION battletest::dbinsertstg(std::string _kind, std::string _memo)
{
    stage_db stage_db_table(_self, _self.value);

    size_t first;
    if (_memo.find(':') == std::string::npos)
    {
        first = _memo.length();
    }
    else
    {
        first = _memo.find(':');
    }
    std::string value = _memo.substr(0, first);
    uint64_t id = atoll(value.c_str());

    if (_kind == "stage_id")
    {
        stage_db_table.emplace(_self, [&](auto &new_stage_id) {
            new_stage_id.stage_id = id;
        });
    }
    else if (_kind == "stage_drop")
    {
        std::vector<size_t> size_list;
        std::vector<std::string> value_list;
        size_list.resize(5);

        for (uint32_t i = 0; i < size_list.size(); ++i)
        {
            if (i == 0)
            {
                size_list[i] = _memo.find(':');
                value_list.push_back(_memo.substr(0, size_list[i]));
            }
            else
            {
                if (_memo.find(':', size_list[i - 1] + 1) == std::string::npos)
                {
                    size_list[i] = _memo.length() - (size_list[i - 1]);
                    value_list.push_back(_memo.substr(size_list[i - 1] + 1, (size_list[i] - 1) - size_list[i - 1]));
                    break;
                }
                else
                {
                    size_list[i] = _memo.find(':', size_list[i - 1] + 1);
                    value_list.push_back(_memo.substr(size_list[i - 1] + 1, (size_list[i] - 1) - size_list[i - 1]));
                }
            }
        }
        stage_drop_db stage_drop_db_table(_self, _self.value);
        stage_drop_db_table.emplace(_self, [&](auto &new_drop) {
            new_drop.drop_id = atoll(value_list[0].c_str());

            drop_object_info new_drop_item;
            new_drop_item.item_id = atoll(value_list[1].c_str());
            new_drop_item.item_drop_per = atoi(value_list[2].c_str());
            new_drop_item.item_drop_count_min = atoi(value_list[3].c_str());
            new_drop_item.item_drop_count_max = atoi(value_list[4].c_str());
            new_drop.drop_list.push_back(new_drop_item);
        });
    }
    else if (_kind == "all")
    {
        size_t seconde;
        std::vector<size_t> size_list;
        std::vector<std::string> id_list;
        std::vector<std::string> value_list;
        size_list.resize(11 * 2);
        for (uint32_t i = 0; i < size_list.size(); ++i)
        {
            if (i == 0)
            {
                size_list[i] = _memo.find(':', first + 1);
                id_list.push_back(_memo.substr(first + 1, (size_list[i] - 1) - first));
            }
            else
            {
                if (_memo.find(':', size_list[i - 1] + 1) == std::string::npos)
                {
                    size_list[i] = _memo.length() - (size_list[i - 1]);
                    if (i % 2 == 0)
                    {
                        id_list.push_back(_memo.substr(size_list[i - 1] + 1, (size_list[i] - 1) - size_list[i - 1]));
                    }
                    else
                    {
                        value_list.push_back(_memo.substr(size_list[i - 1] + 1, (size_list[i] - 1) - size_list[i - 1]));
                    }
                    break;
                }
                else
                {
                    size_list[i] = _memo.find(':', size_list[i - 1] + 1);
                    if (i % 2 == 0)
                    {
                        id_list.push_back(_memo.substr(size_list[i - 1] + 1, (size_list[i] - 1) - size_list[i - 1]));
                    }
                    else
                    {
                        value_list.push_back(_memo.substr(size_list[i - 1] + 1, (size_list[i] - 1) - size_list[i - 1]));
                    }
                }
            }
        }

        stage_db_table.emplace(_self, [&](auto &new_stage_id) {
            new_stage_id.stage_id = id;
            for (uint32_t i = 0; i < id_list.size(); ++i)
            {
                if (id_list[i] == "stage_type")
                {
                    new_stage_id.stage_type = atoi(value_list[i].c_str());
                }
                else if (id_list[i] == "need_stage_id")
                {
                    new_stage_id.need_stage_id = atoi(value_list[i].c_str());
                }
                else if (id_list[i] == "stage_group_list")
                {
                    new_stage_id.stage_group_list = atoi(value_list[i].c_str());
                }
                else if (id_list[i] == "need_entrance_item_id")
                {
                    new_stage_id.need_entrance_item_id = atoi(value_list[i].c_str());
                }
                else if (id_list[i] == "need_entrance_item_count")
                {
                    new_stage_id.need_entrance_item_count = atoi(value_list[i].c_str());
                }
                else if (id_list[i] == "monster_level_min")
                {
                    new_stage_id.monster_level_min = atoi(value_list[i].c_str());
                }
                else if (id_list[i] == "monster_level_max")
                {
                    new_stage_id.monster_level_max = atoll(value_list[i].c_str());
                }
                else if (id_list[i] == "boss_monster_level")
                {
                    new_stage_id.boss_monster_level = atoi(value_list[i].c_str());
                }
                else if (id_list[i] == "char_exp")
                {
                    new_stage_id.char_exp = atoi(value_list[i].c_str());
                }
                else if (id_list[i] == "first_reward_id")
                {
                    new_stage_id.first_reward_id = atoi(value_list[i].c_str());
                }
                else if (id_list[i] == "first_reward_value")
                {
                    new_stage_id.first_reward_value = atoi(value_list[i].c_str());
                }
                else
                {
                    eosio_assert(0 == 1, "Not Exist Column");
                }
            }
        });
    }
    else
    {
        size_t seconde;
        std::vector<size_t> size_list;
        std::vector<std::string> id_list;
        std::vector<std::string> value_list;
        std::string value;
        if (_kind == "monster_list" || _kind == "reward_list")
        {
            seconde = _memo.find(':', first + 1);
            value = _memo.substr(first + 1, seconde);
            uint32_t count = atoi(value.c_str());
            size_list.resize(count * 2);

            for (uint32_t i = 0; i < size_list.size(); ++i)
            {
                if (i == 0)
                {
                    size_list[i] = _memo.find(':', seconde + 1);
                    id_list.push_back(_memo.substr(seconde + 1, (size_list[i] - 1) - seconde));
                }
                else
                {
                    if (_memo.find(':', size_list[i - 1] + 1) == std::string::npos)
                    {
                        size_list[i] = _memo.length() - (size_list[i - 1]);
                        if (i % 2 == 0)
                        {
                            id_list.push_back(_memo.substr(size_list[i - 1] + 1, (size_list[i] - 1) - size_list[i - 1]));
                        }
                        else
                        {
                            value_list.push_back(_memo.substr(size_list[i - 1] + 1, (size_list[i] - 1) - size_list[i - 1]));
                        }
                        break;
                    }
                    else
                    {
                        size_list[i] = _memo.find(':', size_list[i - 1] + 1);
                        if (i % 2 == 0)
                        {
                            id_list.push_back(_memo.substr(size_list[i - 1] + 1, (size_list[i] - 1) - size_list[i - 1]));
                        }
                        else
                        {
                            value_list.push_back(_memo.substr(size_list[i - 1] + 1, (size_list[i] - 1) - size_list[i - 1]));
                        }
                    }
                }
            }
        }
        else
        {
            seconde = _memo.length() - first;
            value = _memo.substr(first + 1, seconde);
        }

        auto stage_db_iter = stage_db_table.find(id);
        eosio_assert(stage_db_iter != stage_db_table.end(), "Need Insert Stage ID 2");
        stage_db_table.modify(stage_db_iter, _self, [&](auto &new_stage_info) {
            if (_kind == "stage_type")
            {
                new_stage_info.stage_type = atoi(value.c_str());
            }
            else if (_kind == "need_stage_id")
            {
                new_stage_info.need_stage_id = atoi(value.c_str());
            }
            else if (_kind == "stage_group_list")
            {
                new_stage_info.stage_group_list = atoi(value.c_str());
            }
            else if (_kind == "need_entrance_item_id")
            {
                new_stage_info.need_entrance_item_id = atoi(value.c_str());
            }
            else if (_kind == "need_entrance_item_count")
            {
                new_stage_info.need_entrance_item_count = atoi(value.c_str());
            }
            else if (_kind == "monster_level_min")
            {
                new_stage_info.monster_level_min = atoi(value.c_str());
            }
            else if (_kind == "monster_level_max")
            {
                new_stage_info.monster_level_max = atoll(value.c_str());
            }
            else if (_kind == "monster_list")
            {
                eosio_assert(stage_db_iter->monster_list.size() <= 10, "Monster List Size Over");
                for (uint32_t i = 0; i < id_list.size(); ++i)
                {
                    stage_monster_info new_monster;
                    new_monster.monster_id = atoll(id_list[i].c_str());
                    new_monster.position = atoll(value_list[i].c_str());
                    new_stage_info.monster_list.push_back(new_monster);
                }
            }
            else if (_kind == "boss_monster_level")
            {
                new_stage_info.boss_monster_level = atoi(value.c_str());
            }
            else if (_kind == "boss_passive_list")
            {
                new_stage_info.boss_passive_list.push_back(atoi(value.c_str()));
            }
            else if (_kind == "boss_skill_list")
            {
                new_stage_info.boss_skill_list.push_back(atoi(value.c_str()));
            }
            else if (_kind == "char_exp")
            {
                new_stage_info.char_exp = atoi(value.c_str());
            }
            else if (_kind == "reward_list")
            {
                for (uint32_t i = 0; i < id_list.size(); ++i)
                {
                    reward_info new_reward;
                    new_reward.reward_id = atoll(id_list[i].c_str());
                    new_reward.reward_value = atoll(value_list[i].c_str());
                    new_stage_info.reward_list.push_back(new_reward);
                }
            }
            else if (_kind == "first_reward_id")
            {
                new_stage_info.first_reward_id = atoi(value.c_str());
            }
            else if (_kind == "first_reward_value")
            {
                new_stage_info.first_reward_value = atoi(value.c_str());
            }
            else
            {
                eosio_assert(0 == 1, "Not Exist Column");
            }
        });
    }
}
ACTION battletest::dberasestg(std::string _kind, std::string _memo)
{
    stage_db stage_db_table(_self, _self.value);
    size_t first;
    if (_memo.find(':') == std::string::npos)
    {
        first = _memo.length();
    }
    else
    {
        first = _memo.find(':');
    }
    std::string value = _memo.substr(0, first);
    uint64_t id = atoll(value.c_str());

    auto stage_db_iter = stage_db_table.find(id);

    if (_kind == "stage_id")
    {
        eosio_assert(stage_db_iter != stage_db_table.end(), "Need Insert Stage ID");
        stage_db_table.erase(stage_db_iter);
    }
    else if (_kind == "stage_drop")
    {
        stage_drop_db stage_drop_db_table(_self, _self.value);
        auto stage_drop_iter = stage_drop_db_table.find(id);
        eosio_assert(stage_drop_iter != stage_drop_db_table.end(), "Not Exist Drop ID");
        stage_drop_db_table.erase(stage_drop_iter);
    }
    else
    {
        stage_db_table.modify(stage_db_iter, _self, [&](auto &new_stage_info) {
            if (_kind == "monster_list")
            {
                new_stage_info.monster_list.clear();
            }
            else if (_kind == "boss_passive_list")
            {
                new_stage_info.boss_passive_list.clear();
            }
            else if (_kind == "boss_skill_list")
            {
                new_stage_info.boss_skill_list.clear();
            }
            else if (_kind == "reward_list")
            {
                new_stage_info.reward_list.clear();
            }
            else
            {
                eosio_assert(0 == 1, "Not Exist Column");
            }
        });
    }
}

#pragma endregion

#pragma region tower_system

ACTION battletest::testsnap()
{
    eos_logs eos_log_table(_self, _self.value);
    eos_log_table.emplace(_self, [&](auto &new_test) {
        new_test.user = _self;
        new_test.signup_eos = 50000;
        new_test.status_change_num = 50000;
        new_test.gacha_eos = 50000;
    });
}

ACTION battletest::towersnap()
{
    require_auth("dlwodnjs111"_n);
    eos_logs eos_log_table(_self, _self.value);
    auto eos_log_iter = eos_log_table.find(_self.value);
    eosio_assert(eos_log_iter != eos_log_table.end(), "Need Set Eos Log");

    eos_snapshots eos_snapshot_table(_self, _self.value);
    eos_snapshot_table.emplace(_self, [&](auto &new_snap_shot) {
        uint32_t first_index = eos_snapshot_table.available_primary_key();
        if (first_index == 0)
        {
            new_snap_shot.snapshot_count = 1;
        }
        else
        {
            new_snap_shot.snapshot_count = eos_snapshot_table.available_primary_key();
        }
        new_snap_shot.signup_eos = eos_log_iter->signup_eos;
        new_snap_shot.status_change_num = eos_log_iter->status_change_num;
        new_snap_shot.gacha_eos = eos_log_iter->gacha_eos;
        new_snap_shot.total_eos = eos_log_iter->signup_eos + eos_log_iter->status_change_num + eos_log_iter->gacha_eos;
    });

    eos_log_table.modify(eos_log_iter, _self, [&](auto &new_log) {
        new_log.signup_eos = 0;
        new_log.status_change_num = 0;
        new_log.gacha_eos = 0;
    });
}

ACTION battletest::claim(eosio::name _user, uint64_t _snapcount)
{
    require_auth("dlwodnjs111"_n);
    auth_users user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Not Exist User");
    user_auth_table.modify(user_auth_iter, _self, [&](auto &set_hero) {
        set_hero.hero.exp = 0;
    });

    eos_snapshots eos_snapshot_table(_self, _self.value);
    auto eos_snapshot_iter = eos_snapshot_table.find(_snapcount);
    eosio_assert(eos_snapshot_iter != eos_snapshot_table.end(), "Wrong SnapShot");

    asset tower_reward(0, symbol(symbol_code("EOS"), 4));
    tower_reward.amount = eos_snapshot_iter->total_eos;

    action(permission_level{get_self(), "active"_n},
           "eosio.token"_n, "transfer"_n,
           std::make_tuple(_self, _user, tower_reward, std::string("tower reward")))
        .send();
}
ACTION battletest::settower(eosio::name _loser, eosio::name _winner, uint64_t _loser_party_num, uint64_t _winner_party_num)
{
    require_auth("dlwodnjs111"_n);
    if (_loser != "dlwodnjs111"_n)
    {
        user_partys user_party_table(_self, _winner.value);
        auto user_party_winner_iter = user_party_table.find(_loser_party_num);
        eosio_assert(user_party_winner_iter != user_party_table.end(), "Wrong Party Number 1");
        user_party_table.modify(user_party_winner_iter, _self, [&](auto &set_party) {
            set_party.state = party_state::on_tower_defense;
        });
    }
    else
    {
        user_partys user_winner_party_table(_self, _winner.value);
        auto user_party_winner_iter = user_winner_party_table.find(_loser_party_num);
        eosio_assert(user_party_winner_iter != user_winner_party_table.end(), "Wrong Party Number 2");
        user_winner_party_table.modify(user_party_winner_iter, _self, [&](auto &set_party) {
            set_party.state = party_state::on_tower_defense;
        });

        user_partys user_loser_party_table(_self, _loser.value);
        auto user_party_loser_iter = user_loser_party_table.find(_winner_party_num);
        eosio_assert(user_party_loser_iter != user_loser_party_table.end(), "Wrong Party Number 3");
        user_loser_party_table.modify(user_party_loser_iter, _self, [&](auto &set_party) {
            set_party.state = party_state::on_wait;
        });
    }
}

#pragma endregion

ACTION battletest::deletebattle(eosio::name _user)
{
    require_auth(_self);

    battle_reward_list user_battle_reward_table(_self, _self.value);
    auto user_battle_reward_iter = user_battle_reward_table.find(_user.value);
    if (user_battle_reward_iter != user_battle_reward_table.end())
    {
        user_battle_reward_table.erase(user_battle_reward_iter);
    }

    battle_state_list batlle_state_list_table(_self, _self.value);
    auto battle_state_list_iter = batlle_state_list_table.find(_user.value);
    if (battle_state_list_iter != batlle_state_list_table.end())
    {
        batlle_state_list_table.erase(battle_state_list_iter);
    }

    battle_actions battle_action_table(_self, _self.value);
    auto battle_action_iter = battle_action_table.find(_user.value);
    if (battle_action_iter != battle_action_table.end())
    {
        battle_action_table.erase(battle_action_iter);
    }
    auth_users user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Not Exist User");
    user_auth_table.modify(user_auth_iter, _self, [&](auto &update_user) {
        update_user.state = user_state::lobby;
    });
}

ACTION battletest::deleteuser(eosio::name _user)
{
    require_auth(_self);

    auth_users pre_user_table(_self, _self.value);
    auto pre_user_iter = pre_user_table.find(_user.value);
    if (pre_user_iter != pre_user_table.end())
    {
        pre_user_table.erase(pre_user_iter);
    }

    user_logs pre_log_table(_self, _self.value);
    auto pre_log_iter = pre_log_table.find(_user.value);
    if (pre_log_iter != pre_log_table.end())
    {
        pre_log_table.erase(pre_log_iter);
    }

    user_gacha_results user_gacha_result_table(_self, _self.value);
    auto gacha_result_iter = user_gacha_result_table.find(_user.value);
    if (gacha_result_iter != user_gacha_result_table.end())
    {
        user_gacha_result_table.erase(gacha_result_iter);
    }

    user_gacha_totals user_gacha_total_table(_self, _self.value);
    auto gacha_total_iter = user_gacha_total_table.find(_user.value);
    if (gacha_total_iter != user_gacha_total_table.end())
    {
        user_gacha_total_table.erase(gacha_total_iter);
    }

    user_items user_preregist_item_table(_self, _user.value);
    auto item_iter = user_preregist_item_table.begin();
    if (item_iter != user_preregist_item_table.end())
    {
        for (auto item = user_preregist_item_table.begin(); item != user_preregist_item_table.end();)
        {
            auto iter = user_preregist_item_table.find(item->primary_key());
            item = user_preregist_item_table.erase(iter);
        }
    }

    user_consumables user_consumables_table(_self, _user.value);
    auto user_consumables_iter = user_consumables_table.begin();
    if (user_consumables_iter != user_consumables_table.end())
    {
        for (auto consumables = user_consumables_table.begin(); consumables != user_consumables_table.end();)
        {
            auto iter = user_consumables_table.find(consumables->primary_key());
            consumables = user_consumables_table.erase(iter);
        }
    }

    user_monsters user_preregist_monster_table(_self, _user.value);
    auto mon_iter = user_preregist_monster_table.begin();
    if (mon_iter != user_preregist_monster_table.end())
    {
        for (auto mon = user_preregist_monster_table.begin(); mon != user_preregist_monster_table.end();)
        {
            auto iter = user_preregist_monster_table.find(mon->primary_key());
            mon = user_preregist_monster_table.erase(iter);
        }
    }

    user_servants user_preregist_servant_table(_self, _user.value);
    auto ser_iter = user_preregist_servant_table.begin();
    if (ser_iter != user_preregist_servant_table.end())
    {
        for (auto ser = user_preregist_servant_table.begin(); ser != user_preregist_servant_table.end();)
        {
            auto iter = user_preregist_servant_table.find(ser->primary_key());
            ser = user_preregist_servant_table.erase(iter);
        }
    }

    account to_acnts(_self, _user.value);
    auto to = to_acnts.begin();
    if (to != to_acnts.end())
    {
        add_balance(_self, to->balance, _self);
        sub_balance(_user, to->balance);
    }

    user_partys user_party_table(_self, _user.value);
    auto party_iter = user_party_table.begin();
    if (party_iter != user_party_table.end())
    {
        user_party_table.erase(party_iter);
    }
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


ACTION battletest::balancetest(eosio::name _user)
{
    user_servants user_servant_table(_self, _user.value);
    for(auto iter = user_servant_table.begin(); iter != user_servant_table.end();)
    {
        auto user_servant_iter = user_servant_table.find(iter->primary_key());
        user_servant_table.modify(user_servant_iter, _self, [&](auto &set_servant_status)
        {

        });
        iter++;
    }

    user_monsters user_monster_table(_self, _user.value);
    for(auto iter = user_monster_table.begin(); iter != user_monster_table.end();)
    {
        auto user_monster_iter = user_monster_table.find(iter->primary_key());
        user_monster_table.modify(user_monster_iter, _self, [&](auto &set_monster_status)
        {

        });
        iter++;
    }

    // user_items user_item_table(_self, _user.value);
    // for(auto iter = user_item_table.begin(); iter != user_item_table.end();)
    // {
    //     auto user_item_iter = user_item_table.find(iter->primary_key());
    //     user_item_table.modify(user_item_iter, _self, [&](auto &set_item_status)
    //     {

    //     });
    //     iter++;
    // }
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
                execute_action(name(receiver), name(code), &battletest::eostransfer);          \
            }                                                                                  \
        }                                                                                      \
    }
// eos 금액에 대해 체크 하는 함

EOSIO_DISPATCH(battletest, (balancetest)(testsnap)(towersnap)(claim)(settower)(equipment)(unequipment)(itemstore)(sellobject)(upgrade)(dberasestg)(dbinsertstg)(deletewhite)(addwhite)(deleteuser)(exitbattle)(deletebattle)(startbattle)(activeturn)(setdata)(herocheat)(partycheat)(resultpre)(resultgacha)(create)(issue)(transfer)(setmaster)(settokenlog)(eostransfer)(initmaster)(inittokenlog)(deleteblack)(addblack)(setpause)(dbinsert)(dberase)(dbinit))