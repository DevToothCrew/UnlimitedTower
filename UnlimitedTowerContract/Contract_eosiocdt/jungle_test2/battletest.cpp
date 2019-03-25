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
                _value_list.push_back(_value.substr(_size_list[i - 1] + 1, (_size_list[i]) - _size_list[i - 1]));
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
        substr_value(_value, value_list, size_list, 9);
        insert_monster_id(atoll(value_list[0].c_str()),
                                atoll(value_list[1].c_str()),
                             atoll(value_list[2].c_str()),
                             atoll(value_list[3].c_str()),
                             atoll(value_list[4].c_str()),
                             atoll(value_list[5].c_str()),
                             atoll(value_list[6].c_str()),
                             atoll(value_list[7].c_str()),
                             atoll(value_list[8].c_str()));
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
    else if (_table == "dbservantlv")
    {
        substr_value(_value, value_list, size_list, 4);
        insert_servant_lv(atoll(value_list[0].c_str()),
                          atoll(value_list[1].c_str()),
                          atoll(value_list[2].c_str()),
                          atoll(value_list[3].c_str()));
    }
    else if (_table == "dbmonsterlv")
    {
        substr_value(_value, value_list, size_list, 4);
        insert_monster_lv(atoll(value_list[0].c_str()),
                          atoll(value_list[1].c_str()),
                          atoll(value_list[2].c_str()),
                          atoll(value_list[3].c_str()));
    }
    else if (_table == "dbstatusserv")
    {
        substr_value(_value, value_list, size_list, 2);
        insert_servant_lv_status(atoll(value_list[0].c_str()),
                          atoll(value_list[1].c_str()));
    }
    else if (_table == "dbstatusmon")
    {
        substr_value(_value, value_list, size_list, 2);
        insert_monster_lv_status(atoll(value_list[0].c_str()),
                         atoll(value_list[1].c_str()));
    }
	  else if(_table == "dbpassive")
    {
        substr_value(_value, value_list, size_list, 8);
        insert_passive(atoll(value_list[0].c_str()),
                             atoi(value_list[1].c_str()),
                             atoi(value_list[2].c_str()),
                             atoi(value_list[3].c_str()),
                             atoi(value_list[4].c_str()),
                             atoi(value_list[5].c_str()),
                             atoi(value_list[6].c_str()),
                             atoi(value_list[7].c_str()));
    }
    else if(_table == "dbactive")
    {
        substr_value(_value, value_list, size_list, 14);
        insert_active(atoll(value_list[0].c_str()),
                       atoi(value_list[1].c_str()),
                       atoi(value_list[2].c_str()),
                       atoi(value_list[3].c_str()),
                       atoi(value_list[4].c_str()),
                       atoi(value_list[5].c_str()),
                       atoi(value_list[6].c_str()),
                       atoi(value_list[7].c_str()),
                       atoi(value_list[8].c_str()),
                       atoi(value_list[9].c_str()),
                       atoi(value_list[10].c_str()),
                       atoi(value_list[11].c_str()),
                       atoi(value_list[12].c_str()),
                       atoi(value_list[13].c_str()));
    }
    else
    {
        eosio_assert(1 == 0 ,"Not Exist Table");
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
    servant_db servant_id_db_table(_self, _self.value);
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

void battletest::insert_monster_id(uint64_t _monster_id, uint64_t _gacha_id, uint64_t _gacha_type, uint64_t _tribe, uint64_t _type, uint64_t _monster_class, uint64_t _skill1, uint64_t _skill2, uint64_t _skill3)
{
    monster_db monster_id_db_table(_self, _self.value);
    auto monster_id_iter = monster_id_db_table.find(_monster_id);
    if (monster_id_iter == monster_id_db_table.end())
    {
        monster_id_db_table.emplace(_self, [&](auto &new_monster_id) {
            new_monster_id.gacha_id = _gacha_id;
            new_monster_id.monster_id = _monster_id;
            new_monster_id.gacha_type = _gacha_type;
            new_monster_id.tribe = _tribe;
            new_monster_id.type = _type;
            new_monster_id.monster_class = _monster_class;

            if (_skill1 != 0 || _skill2 != 0 || _skill3 != 0)
            {
                skill_object skill;
                skill.passive_skill_1 = _skill1;
                skill.active_skill_2 = _skill2;
                skill.active_skill_3 = _skill3;
                new_monster_id.skills.push_back(skill);
            }
        });
    }
    else
    {
        monster_id_db_table.modify(monster_id_iter, _self, [&](auto &new_monster_id) {
            new_monster_id.gacha_id = _gacha_id;
            new_monster_id.type = _type;
            new_monster_id.monster_class = _monster_class;

            if (_skill1 != 0 || _skill2 != 0 || _skill3 != 0)
            {
                skill_object skill;
                skill.passive_skill_1 = _skill1;
                skill.active_skill_2 = _skill2;
                skill.active_skill_3 = _skill3;
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

void battletest::insert_equip_item_id(uint64_t _item_id, uint64_t _item_set_id, uint64_t _tier, uint64_t _type, uint64_t _job,uint64_t _option, uint32_t _status_value_min, uint32_t _status_value_max, uint32_t _upgrade_status_value, uint64_t _random_option_id)
{
    equipitem_db equipitem_id_db_table(_self, _self.value);
    auto item_id_iter = equipitem_id_db_table.find(_item_id);
    if (item_id_iter == equipitem_id_db_table.end())
    {
        equipitem_id_db_table.emplace(_self, [&](auto &new_item_id) {
            new_item_id.item_id = _item_id;
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
    else
    {
        equipitem_id_db_table.modify(item_id_iter, _self, [&](auto &new_item_id) {
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

void battletest::insert_servant_lv(uint64_t _job, uint64_t _lv_up_str, uint64_t _lv_up_dex, uint64_t _lv_up_int)
{
    servant_lv_db servant_lv_db_table(_self, _self.value);
    auto servant_lv_db_iter = servant_lv_db_table.find(_job);
    if (servant_lv_db_iter == servant_lv_db_table.end())
    {
        servant_lv_db_table.emplace(_self, [&](auto &new_servant_lv) {
            new_servant_lv.job = _job;
            new_servant_lv.lvup_str = _lv_up_str;
            new_servant_lv.lvup_dex = _lv_up_dex;
            new_servant_lv.lvup_int = _lv_up_int;
        });
    }
    else
    {
        servant_lv_db_table.modify(servant_lv_db_iter, _self, [&](auto &new_servant_lv) {
            new_servant_lv.job = _job;
            new_servant_lv.lvup_str = _lv_up_str;
            new_servant_lv.lvup_dex = _lv_up_dex;
            new_servant_lv.lvup_int = _lv_up_int;
        });
    }
}

void battletest::insert_monster_lv(uint64_t _monster_class_grade, uint64_t _lv_up_str, uint64_t _lv_up_dex, uint64_t _lv_up_int)
{
    monster_lv_db monster_lv_db_table(_self, _self.value);
    auto monster_lv_db_iter = monster_lv_db_table.find(_monster_class_grade);
    if (monster_lv_db_iter == monster_lv_db_table.end())
    {
        monster_lv_db_table.emplace(_self, [&](auto &new_monster_lv) {
            new_monster_lv.monster_class_grade = _monster_class_grade;
            new_monster_lv.lvup_str = _lv_up_str;
            new_monster_lv.lvup_dex = _lv_up_dex;
            new_monster_lv.lvup_int = _lv_up_int;
        });
    }
    else
    {
        monster_lv_db_table.modify(monster_lv_db_iter, _self, [&](auto &new_monster_lv) {
            new_monster_lv.monster_class_grade = _monster_class_grade;
            new_monster_lv.lvup_str = _lv_up_str;
            new_monster_lv.lvup_dex = _lv_up_dex;
            new_monster_lv.lvup_int = _lv_up_int;
        });
    }
}

void battletest::insert_servant_lv_status(uint64_t _type, uint64_t _num)
{
    servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
    
    servant_lv_status_db_table.emplace(_self, [&](auto &new_servant_lv_status_db) {
        new_servant_lv_status_db.grade = _type;
        std::vector<lv_status_sub> lv_status_list;
        std::vector<uint64_t> temp_list;
        for (uint32_t i = 0; i <= 9; i++)
        {
            temp_list.push_back(_num);
            _num += 1;
        }

        for (uint32_t i = 0; i <= 9; i++)
        {
            lv_status_sub new_sub;
            new_sub.pre_status = i;
            new_sub.update_status = temp_list[i];

            new_servant_lv_status_db.lv_status.push_back(new_sub);
        }
    });
}

void battletest::insert_monster_lv_status(uint64_t _type, uint64_t _num)
{
    monster_lv_status_db monster_lv_status_db_table(_self, _self.value);

    monster_lv_status_db_table.emplace(_self, [&](auto &new_monster_lv_status_db) {
        new_monster_lv_status_db.grade = _type;
        std::vector<lv_status_sub> lv_status_list;
        std::vector<uint64_t> temp_list;
        for (uint32_t i = 0; i <= 9; i++)
        {
            temp_list.push_back(_num);
            _num += 1;
        }

        for (uint32_t i = 0; i <= 9; i++)
        {
            lv_status_sub new_sub;
            new_sub.pre_status = i;
            new_sub.update_status = temp_list[i];

            new_monster_lv_status_db.lv_status.push_back(new_sub);
        }
    });
}

void battletest::insert_passive(uint64_t _id, uint32_t _enable_stack, uint32_t _max_stack,
                                uint32_t _effect_type, uint32_t _effect_value, uint32_t _effect_value_add, uint32_t _target, uint32_t _role_target)
{
    passive_db passive_db_table(_self, _self.value);
    auto passive_db_iter = passive_db_table.find(_id);
    if(passive_db_iter == passive_db_table.end())
    {
        passive_db_table.emplace(_self, [&](auto &new_passive)
        {
            new_passive.passive_id = _id;
            new_passive.enable_stack = _enable_stack;
            new_passive.max_stack = _max_stack;
            new_passive.effect_type = _effect_type;
            new_passive.effect_value = _effect_value;
            new_passive.effect_value_add = _effect_value_add;
            new_passive.target = _target;
            new_passive.role_target = _role_target;
        });
    }
    else
    {
        passive_db_table.modify(passive_db_iter ,_self, [&](auto &new_passive)
        {
            new_passive.enable_stack = _enable_stack;
            new_passive.max_stack = _max_stack;
            new_passive.effect_type = _effect_type;
            new_passive.effect_value = _effect_value;
            new_passive.effect_value_add = _effect_value_add;
            new_passive.target = _target;
            new_passive.role_target = _role_target;
        });
    }
}

void battletest::insert_active(uint64_t _id, uint32_t _job, uint32_t _active_per,
                               uint32_t _skill_type, uint32_t _attack_type, uint32_t _dmg_type,uint32_t _target, uint32_t _target_count, uint32_t _target_range,
                               uint32_t _hit_count, uint32_t _atk_per, uint32_t _atk_per_add, uint32_t _heal_per, uint32_t _heal_per_add)
{
    active_db active_db_table(_self, _self.value);
    auto active_db_iter = active_db_table.find(_id);
    if(active_db_iter == active_db_table.end())
    {
        active_db_table.emplace(_self, [&](auto &new_active)
        {
            new_active.active_id = _id;
            new_active.job = _job;
            new_active.active_per = _active_per;
            new_active.skill_type = _skill_type;
            new_active.attack_type = _attack_type;
            new_active.dmg_type = _dmg_type;
            new_active.target = _target;
            new_active.target_count = _target_count;
            new_active.target_range = _target_range;
            new_active.hit_count = _hit_count;
            new_active.atk_per = _atk_per;
            new_active.atk_per_add = _atk_per_add;
            new_active.heal_per = _heal_per;
            new_active.heal_per_add = _heal_per_add;
        });
    }
    else
    {
        active_db_table.modify(active_db_iter, _self, [&](auto &new_active) {
            new_active.job = _job;
            new_active.active_per = _active_per;
            new_active.skill_type = _skill_type;
            new_active.attack_type = _attack_type;
            new_active.dmg_type = _dmg_type;
            new_active.target = _target;
            new_active.target_count = _target_count;
            new_active.target_range = _target_range;
            new_active.hit_count = _hit_count;
            new_active.atk_per = _atk_per;
            new_active.atk_per_add = _atk_per_add;
            new_active.heal_per = _heal_per;
            new_active.heal_per_add = _heal_per_add;
        });
    }

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
        erase_common_item_id(value);
    }
    else if (_table == "dbequipitem")
    {
        value = atoll(_value.c_str());
        erase_equip_item_id(value);
    }
    else if (_table =="dbservnatlv")
    {
        value = atoll(_value.c_str());
        erase_servant_lv(value);
    }
    else if (_table =="dbmonsterlv")
    {
        value = atoll(_value.c_str());
        erase_monster_lv(value);
    }
    else if (_table =="dbstatusserv")
    {
        value = atoll(_value.c_str());
        erase_servant_lv_status(value);
    }
    else if (_table =="dbstatusmon")
    {
        value = atoll(_value.c_str());
        erase_monster_lv_status(value);
    }
	else if (_table == "dbpassive")
    {
        value = atoll(_value.c_str());
        erase_passive(value);
    }
    else if (_table == "dbactive")
    {
        value = atoll(_value.c_str());
        erase_active(value);
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
    servant_db servant_id_db_table(_self, _self.value);
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

void battletest::erase_servant_lv(uint64_t _job)
{
    servant_lv_db servant_lv_db_table(_self, _self.value);
    auto servant_lv_db_iter = servant_lv_db_table.find(_job);
    eosio_assert(servant_lv_db_iter != servant_lv_db_table.end(), "Not exist servant_lv Data");
    servant_lv_db_table.erase(servant_lv_db_iter);
}

void battletest::erase_monster_lv(uint64_t _monster_class_grade)
{
    monster_lv_db monster_lv_db_table(_self, _self.value);
    auto monster_lv_db_iter = monster_lv_db_table.find(_monster_class_grade);
    eosio_assert(monster_lv_db_iter != monster_lv_db_table.end(), "Not exist monster_lv Data");
    monster_lv_db_table.erase(monster_lv_db_iter);
}

void battletest::erase_servant_lv_status(uint64_t _type)
{
    servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
    auto servant_lv_status_db_iter = servant_lv_status_db_table.find(_type);
    eosio_assert(servant_lv_status_db_iter != servant_lv_status_db_table.end(), "Not exist servant_lv_status Data");
    servant_lv_status_db_table.erase(servant_lv_status_db_iter); 
}

void battletest::erase_monster_lv_status(uint64_t _type)
{   
    monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
    auto monster_lv_status_db_iter = monster_lv_status_db_table.find(_type);
    eosio_assert(monster_lv_status_db_iter != monster_lv_status_db_table.end(), "Not exist monster_lv_status Data");
    monster_lv_status_db_table.erase(monster_lv_status_db_iter); 
}   

void battletest::erase_passive(uint64_t _id)
{
    passive_db passive_db_table(_self, _self.value);
    auto passive_db_iter = passive_db_table.find(_id);
    eosio_assert(passive_db_iter != passive_db_table.end(),"Not Exist Passive 1");
    passive_db_table.erase(passive_db_iter);
}

void battletest::erase_active(uint64_t _id)
{
    active_db active_db_table(_self, _self.value);
    auto active_db_iter = active_db_table.find(_id);
    eosio_assert(active_db_iter != active_db_table.end(),"Not Exist Active 1");
    active_db_table.erase(active_db_iter);
}

ACTION battletest::movedb(eosio::name _user)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 3");

    pre_users pre_user_table(_self, _self.value);
    auto pre_user_iter = pre_user_table.find(_user.value);
    eosio_assert(pre_user_iter != pre_user_table.end(), "Not exist user data");

    pre_logs pre_log_table(_self, _self.value);
    auto pre_log_iter = pre_log_table.find(_user.value);
    eosio_assert(pre_log_iter != pre_log_table.end(), "Not exist pre log data ");

    user_logs user_log_table(_self,_self.value);
    auto user_log_iter = user_log_table.find(pre_log_iter->user.value);
    //eosio_assert(user_log_iter == user_log_table.end(), "Not exist user log");
    user_log_table.emplace(_self, [&](auto &user_log){ 
        user_log.user = pre_log_iter->user;
        user_log.servant_num = pre_log_iter->servant_num;
        user_log.monster_num = pre_log_iter->monster_num;
        user_log.item_num = pre_log_iter->item_num;
        user_log.gacha_num = pre_log_iter->gacha_num;
        user_log.login_time =0;
        user_log.get_gold = 0;
        user_log.use_gold = 0;
        user_log.use_eos = pre_log_iter->use_eos;
        user_log.battle_count = 0;
        user_log.last_stage_num = 0;
        user_log.last_tower_num = 0;
        user_log.top_clear_stage = 0;
        user_log.top_clear_tower = 0;
        user_log.add_party_count = 0;
        user_log.soul_powder = 0; 
        user_log.mail = pre_log_iter->servant_num + pre_log_iter->monster_num + pre_log_iter->item_num;
    });
    //pre_log_table.erase(_user.value);

    auth_users auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(pre_user_iter->user.value);
    auto auth_user_iter2 = auth_user_table.find(pre_log_iter->user.value);
    eosio_assert(auth_user_iter == auth_user_table.end(), "Not exist auth_user");
    auth_user_table.emplace(_self, [&](auto &auth_user){
        auth_user.user = pre_user_iter->user;
        auth_user.state = user_state::pre_regist;
        auth_user.exp = 0;
        auth_user.current_servant_inventory = 0;
        auth_user.current_monster_inventory = 0;
        auth_user.current_equipitem_inventory =0;
        auth_user.current_item_inventory = 0;
        auth_user.servant_inventory = 50;
        auth_user.monster_inventory = 50;
        auth_user.equipitem_inventory = 50;
        auth_user.item_inventory = 50;        
        
    });
    //pre_user_table.erase(_user.value);


    user_preregist_monsters user_preregist_monster_table(_self, _user.value);
    for (auto iter2 = user_preregist_monster_table.begin(); iter2 != user_preregist_monster_table.end();)
    {
        const auto &user_preregist_monster_iter = user_preregist_monster_table.get(iter2->primary_key(), "Not exsit data");

       // 스탯 변경 참조 테이블 read
        monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
        auto monster_lv_status_db_iter = monster_lv_status_db_table.find(user_preregist_monster_iter.grade);

        eosio_assert(monster_lv_status_db_iter != monster_lv_status_db_table.end(),"Not exist monster_lv_status Data");

        //  메일 테이블에 변경된 스탯으로 저장

        mail_db mail_db_table(_self, _user.value);
        mail_db_table.emplace(_self, [&](auto &move_mail) {
            uint32_t first_index = mail_db_table.available_primary_key();
            if (first_index == 0)
            {
                move_mail.mail_index = 1;
            }
            else
            {
                move_mail.mail_index = mail_db_table.available_primary_key();
            }
            monster_db monster_db_table(_self,_self.value);
            auto monster_second =monster_db_table.get_index<"second"_n>();
            auto monster_id_db_iter = monster_second.find(user_preregist_monster_iter.id);

            move_mail.type_id = user_preregist_monster_iter.id;
            move_mail.type = 2;
            move_mail.grade = user_preregist_monster_iter.grade;
            move_mail.status.basic_str = monster_lv_status_db_iter->lv_status[user_preregist_monster_iter.status.basic_str].update_status;
            move_mail.status.basic_dex = monster_lv_status_db_iter->lv_status[user_preregist_monster_iter.status.basic_dex].update_status;
            move_mail.status.basic_int = monster_lv_status_db_iter->lv_status[user_preregist_monster_iter.status.basic_int].update_status;
            move_mail.status.plus_str = user_preregist_monster_iter.status.plus_str;
            move_mail.status.plus_dex = user_preregist_monster_iter.status.plus_dex;
            move_mail.status.plus_int = user_preregist_monster_iter.status.plus_int;
        });
        iter2++;
    }
    // for(auto iter2 = user_preregist_monster_table.begin(); iter2 != user_preregist_monster_table.end();)
    // {
    //     //user_preregist_monster_table.erase(_user.value);
    //     iter2++;
    // }

    user_preregist_servants user_preregist_servant_table(_self, _user.value);
    auto user_preresgist_servant_iter = user_preregist_servant_table.find(_user.value);
    for (auto iter3 = user_preregist_servant_table.begin(); iter3 != user_preregist_servant_table.end();)
    {
        const auto &user_preregist_servant_iter = user_preregist_servant_table.get(iter3->primary_key(), "not exsit data");

        // 스탯 변경 참조 테이블 read
        servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
        uint64_t temp_grade = 5;
        auto servant_lv_status_db_iter = servant_lv_status_db_table.find(temp_grade);

        eosio_assert(servant_lv_status_db_iter != servant_lv_status_db_table.end(), "Not exist servant_lv_status Data");

        //  메일 테이블에 변경된 스탯으로 저장
        mail_db mail_db_table(_self, _user.value);
        mail_db_table.emplace(_self, [&](auto &move_mail) {
            uint32_t first_index = mail_db_table.available_primary_key();
            if (first_index == 0)
            {
                move_mail.mail_index = 1;
            }
            else
            {
                move_mail.mail_index = mail_db_table.available_primary_key();
            }
            servant_db servant_db_table(_self, _self.value);
            auto servant_second = servant_db_table.get_index<"second"_n>();
            auto servant_db_iter = servant_second.find(user_preregist_servant_iter.id);
            move_mail.type_id = user_preregist_servant_iter.id;
            move_mail.type = 1;
            move_mail.status.basic_str = servant_lv_status_db_iter->lv_status[user_preregist_servant_iter.status.basic_str].update_status;
            move_mail.status.basic_dex = servant_lv_status_db_iter->lv_status[user_preregist_servant_iter.status.basic_dex].update_status;
            move_mail.status.basic_int = servant_lv_status_db_iter->lv_status[user_preregist_servant_iter.status.basic_int].update_status;
            move_mail.status.plus_str = 0;
            move_mail.status.plus_dex = 0;
            move_mail.status.plus_int = 0;
        });
        iter3++;
    }
//    for(auto iter4 = user_preregist_monster_table.begin(); iter4 != user_preregist_monster_table.end();)
//    {
//       //  user_preregist_servant_table.erase(_user.value);
//        iter4++;
//    }

   user_preregist_items user_preregist_item_table(_self, _user.value);
    for (auto iter4 = user_preregist_item_table.begin(); iter4 != user_preregist_item_table.end();)
    {
        const auto &user_preregist_item_iter = user_preregist_item_table.get(iter4->primary_key(), "Not exsit data");
        mail_db mail_db_table(_self, _user.value);
        mail_db_table.emplace(_self, [&](auto & move_mail){
            uint32_t first_index = mail_db_table.available_primary_key();
            if (first_index == 0)
            {
                move_mail.mail_index = 1;
            }
            else
            {
                move_mail.mail_index = mail_db_table.available_primary_key();
            }
            commonitem_db commonitem_db_table(_self, _self.value);
            auto item_second = commonitem_db_table.get_index<"second"_n>();
            auto common_item_db_iter = item_second.find(user_preregist_item_iter.id);            
            
            move_mail.type_id = user_preregist_item_iter.id;
            move_mail.type = 3;
            move_mail.job = user_preregist_item_iter.job;
            move_mail.tier = user_preregist_item_iter.tier;
            move_mail.grade = user_preregist_item_iter.grade;
            move_mail.atk = 0;
            move_mail.def =0;           
            move_mail.status.basic_str =  user_preregist_item_iter.main_status;
            move_mail.status.basic_dex = 0;
            move_mail.status.basic_int = 0;
            move_mail.status.plus_str = 0;
            move_mail.status.plus_dex = 0;
            move_mail.status.plus_int = 0;
        });
        iter4++;
    }

    // // for(auto iter4 = user_preregist_item_table.begin(); iter4 != user_preregist_item_table.end();)
    // // {
    // //     //user_preregist_item_table.erase(_user.value);
    // //     iter4++;
    // // }

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

    monster_db monster_id_bd_table(_self,_self.value);

    
    // for (auto monster_db_table_iter = monster_id_bd_table.begin(); monster_db_table_iter != monster_id_bd_table.end();)
    // {
    //     auto iter = monster_id_bd_table.find(monster_db_table_iter->primary_key());
    //     monster_db_table_iter++;
    //     monster_id_bd_table.erase(iter);
    // }

    // for (auto gender_db_table_iter = gender_db_table.begin(); gender_db_table_iter != gender_db_table.end();)
    // {
    //     auto iter = gender_db_table.find(gender_db_table_iter->primary_key());
    //     gender_db_table_iter++;
    //     gender_db_table.erase(iter);
    // }

    // for (auto servant_id_db_table_iter = servant_id_db_table.begin(); servant_id_db_table_iter != servant_id_db_table.end();)
    // {
    //     auto iter = servant_id_db_table.find(servant_id_db_table_iter->primary_key());
    //     servant_id_db_table_iter++;
    //     servant_id_db_table.erase(iter);
    // }

    // for (auto servant_job_db_table_iter = servant_job_db_table.begin(); servant_job_db_table_iter != servant_job_db_table.end();)
    // {
    //     auto iter = servant_job_db_table.find(servant_job_db_table_iter->primary_key());
    //     servant_job_db_table_iter++;
    //     servant_job_db_table.erase(iter);
    // }

    // for (auto head_db_table_iter = head_db_table.begin(); head_db_table_iter != head_db_table.end();)
    // {
    //     auto iter = head_db_table.find(head_db_table_iter->primary_key());
    //     head_db_table_iter++;
    //     head_db_table.erase(iter);
    // }

    // for (auto hair_db_table_iter = hair_db_table.begin(); hair_db_table_iter != hair_db_table.end();)
    // {
    //     auto iter = hair_db_table.find(hair_db_table_iter->primary_key());
    //     hair_db_table_iter++;
    //     hair_db_table.erase(iter);
    // }

    // for (auto body_db_table_iter = body_db_table.begin(); body_db_table_iter != body_db_table.end();)
    // {
    //     auto iter = body_db_table.find(body_db_table_iter->primary_key());
    //     body_db_table_iter++;
    //     body_db_table.erase(iter);
    // }

    // for (auto monster_grade_db_table_iter = monster_grade_db_table.begin(); monster_grade_db_table_iter != monster_grade_db_table.end();)
    // {
    //     auto iter = monster_grade_db_table.find(monster_grade_db_table_iter->primary_key());
    //     monster_grade_db_table_iter++;
    //     monster_grade_db_table.erase(iter);
    // }

    // for (auto monster_id_db_table_iter = monster_id_db_table.begin(); monster_id_db_table_iter != monster_id_db_table.end();)
    // {
    //     auto iter = monster_id_db_table.find(monster_id_db_table_iter->primary_key());
    //     monster_id_db_table_iter++;
    //     monster_id_db_table.erase(iter);
    // }

    // for (auto item_id_db_table_iter = item_id_db_table.begin(); item_id_db_table_iter != item_id_db_table.end();)
    // {
    //     auto iter = item_id_db_table.find(item_id_db_table_iter->primary_key());
    //     item_id_db_table_iter++;
    //     item_id_db_table.erase(iter);
    // }

    // for (auto item_grade_db_table_iter = item_grade_db_table.begin(); item_grade_db_table_iter != item_grade_db_table.end();)
    // {
    //     auto iter = item_grade_db_table.find(item_grade_db_table_iter->primary_key());
    //     item_grade_db_table_iter++;
    //     item_grade_db_table.erase(iter);
    // }

    // commonitem_db commonitem_db_table(_self, _self.value);
    // for (auto item_grade_db_table_iter = commonitem_db_table.begin(); item_grade_db_table_iter != commonitem_db_table.end();)
    // {
    //     auto iter = commonitem_db_table.find(item_grade_db_table_iter->primary_key());
    //     item_grade_db_table_iter++;
    //     commonitem_db_table.erase(iter);
    // }
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

ACTION battletest::loginuser(eosio::name _user)
{
    require_auth(_user);

    auth_users auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(_user.value);
    eosio_assert(auth_user_iter != auth_user_table.end(), "Not exsit User info");

    //사전 예약한 유저 로그인인지 체크
    if (auth_user_iter->state == user_state::pre_regist)
    {
        uint64_t l_user = _user.value;
        uint64_t l_seed = safeseed::get_seed_value(l_user, now());
        tutorial_servant(_user, l_seed);

        auth_user_table.modify(auth_user_iter, _self, [&](auto &new_result) {
            new_result.state = tutorial_state::start_tutorial;
            new_result.current_servant_inventory +=1;
        });
    }

    // //처음 signup 한 유저
    // else if (auth_user_iter->state == user_state::auth_regist)
    // {
    //     uint64_t l_user = _user.value;
    //     uint64_t l_seed = safeseed::get_seed_value(l_user, now());
    //     tutorial_servant(_user, l_seed);

    //     auth_user_table.modify(auth_user_iter, _self, [&](auto &new_result) {
    //         new_result.state = tutorial_state::start_tutorial;
    //         new_result.current_servant_inventory +=1;
    //     });
    // }
}


// ACTION battletest::lookset(eosio::name _user, uint64_t _body, uint64_t _head, uint64_t _hair, uint64_t _gender, std::string _seed)
// {
//     require_auth(_user);

//     size_t center = _seed.find(':');
//     size_t end = _seed.length() - (center + 1);
//     eosio_assert(_seed.find(':') != std::string::npos, "Wrong Seed Error");

//     std::string result_seed = _seed.substr(0, center);
//     std::string result_sha = _seed.substr(center + 1, end);

//     uint64_t check_result = safeseed::check_seed(result_seed, result_sha);
//     uint64_t user = get_user_seed_value(_user.value);
//     uint64_t status_seed = safeseed::get_seed_value(user, check_result);

//     std::vector<uint64_t> status_list;
//     safeseed::get_total_rand(status_list, hero_total_status ,status_seed);

//     auth_users auth_user_table(_self, _self.value);
//     auto auth_user_iter = auth_user_table.find(_user.value);
//     eosio_assert(auth_user_iter != auth_user_table.end(), "Not exist User info");
      
//     body_db body_db_table(_self, _self.value);
//     auto body_db_iter = body_db_table.get(_body, "not exist body info");

//     head_db head_db_table(_self, _self.value);
//     auto head_db_iter = head_db_table.get(_head, "not exist head info");

//     hair_db hair_db_table(_self, _self.value);
//     auto hair_db_iter = hair_db_table.get(_hair, "not exist hair info");

//     gender_db gender_db_table(_self, _self.value);
//     auto gender_db_iter = gender_db_table.get(_gender, "not exist gender info");

//     eosio_assert(auth_user_iter->state == user_state::look, "already completed look setting");

//     auth_user_table.modify(auth_user_iter, _self, [&](auto &hero_look_set) {
//         hero_look_set.state = user_state::dice;

//         hero_look_set.hero.appear.head = _head;
//         hero_look_set.hero.appear.hair = _hair;
//         hero_look_set.hero.appear.body = _body;
//         hero_look_set.hero.appear.gender = _gender;
//         hero_look_set.hero.status.basic_str = status_list[0];
//         hero_look_set.hero.status.basic_dex = status_list[1];
//         hero_look_set.hero.status.basic_int = status_list[2];
//         hero_look_set.hero.status.plus_str = 0;
//         hero_look_set.hero.status.plus_dex = 0;
//         hero_look_set.hero.status.plus_int = 0;
//     });
// }

void battletest::signup(eosio::name _user)
{
    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter == user_log_table.end(), "User Already Signup 4");
    
    user_log_table.emplace(_self, [&](auto &new_log) {        
        new_log.user = _user;
        new_log.servant_num =0;
        new_log.monster_num = 0;
        new_log.item_num = 0;
        new_log.gacha_num =0;
        new_log.login_time =0;
        new_log.get_gold = 0;
        new_log.use_gold = 0;
        new_log.use_eos = 0;
        new_log.battle_count = 0;
        new_log.last_stage_num = 0;
        new_log.last_tower_num = 0;
        new_log.top_clear_stage = 0;
        new_log.top_clear_tower = 0;
        new_log.add_party_count = 0;
        new_log.soul_powder = 0; 
        new_log.mail = 0;

    });

    auth_users auth_user_table(_self, _self.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "User Already Signup 3");

    auth_user_table.emplace(_self, [&](auto &new_user) {
        new_user.user = _user;
        new_user.state = user_state::auth_regist;
        new_user.exp = 0;
        new_user.current_servant_inventory = 0;
        new_user.current_monster_inventory = 0;
        new_user.current_equipitem_inventory =0;
        new_user.current_item_inventory = 0;
        new_user.servant_inventory = 50;
        new_user.monster_inventory = 50;
        new_user.equipitem_inventory = 50;
        new_user.item_inventory = 50;    
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
ACTION battletest::tutorial(eosio::name _user, uint64_t _monster)
{
    // require_auth(_user);

    // auth_users auth_user_table(_self, _self.value);
    // auto new_user_iter = auth_user_table.find(_user.value);
    // eosio_assert(new_user_iter == auth_user_table.end(), "User Already Signup 3");

    // user_logs user_log_table(_self, _self.value);
    // auto user_log_iter = user_log_table.find(_user.value);
    // eosio_assert(user_log_iter == user_log_table.end(), "User Already Signup 4");

    // start_monster start_monster_table(_self, _self.value);

    // uint64_t random_monster_id = _monster;

    // auto monster_second = start_monster_table.get_index<"second"_n>();
    // const auto &monster_id_db_iter = monster_second.get(random_monster_id, "Not Exist Monster ID 1");

    // uint64_t random_grade = 5;

    // monster_grade_db monster_grade_db_table(_self, _self.value);
    // const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "Not Exist Monster Grade 4");

    // result_info result;
    // user_monsters user_monster_table(_self, _user.value);
    // user_monster_table.emplace(_self, [&](auto &update_user_monster_list) {
    //     uint32_t first_index = user_monster_table.available_primary_key();
    //     if (first_index == 0)
    //     {
    //         update_user_monster_list.index = 1;
    //     }
    //     else
    //     {
    //         update_user_monster_list.index = user_monster_table.available_primary_key();
    //     }

    //     monster_info new_monster;
    //     new_monster.id = monster_id_db_iter.monster_id;
    //     new_monster.grade = monster_grade_db_iter.grade;
    //     monster_random_count += 1;
    //     new_monster.status.basic_str = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_str, monster_grade_db_iter.min_range.base_str, monster_random_count);
    //     monster_random_count += 1;
    //     new_monster.status.basic_dex = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_dex, monster_grade_db_iter.min_range.base_dex, monster_random_count);
    //     monster_random_count += 1;
    //     new_monster.status.basic_int = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_int, monster_grade_db_iter.min_range.base_int, monster_random_count);
    //     new_monster.state = object_state::on_inventory;

    //     result.index = update_user_monster_list.index;
    //     result.type = result::monster;

    //     update_user_monster_list.party_number = EMPTY_PARTY;
    //     update_user_monster_list.monster = new_monster;
    // });

    // user_gacha_results user_gacha_result_table(_self, _self.value);
    // user_gacha_totals user_gacha_total_table(_self, _self.value);

    // auto user_gacha_result_iter = user_gacha_result_table.find(_user.value);
    // if (user_gacha_result_iter == user_gacha_result_table.end())
    // {
    //     user_gacha_result_table.emplace(_self, [&](auto &new_result) {
    //         new_result.user = _user;
    //         new_result.result = result;
    //     });
    // }
    // else
    // {
    //     user_gacha_result_table.modify(user_gacha_result_iter, _self, [&](auto &new_result) {
    //         new_result.result = result;
    //     });
    // }

    // auto user_gacha_total_iter = user_gacha_total_table.find(_user.value);
    // if (user_gacha_total_iter == user_gacha_total_table.end())
    // {
    //     user_gacha_total_table.emplace(_self, [&](auto &new_result) {
    //         new_result.user = _user;
    //         new_result.result_list.push_back(result);
    //     });
    // }
    // else
    // {
    //     user_gacha_total_table.modify(user_gacha_total_iter, _self, [&](auto &new_result) {
    //         new_result.result_list.push_back(result);
    //     });
    // }

    // user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
    //     update_log.gacha_num += 1;
    //     update_log.monster_num += 1;
    // });

}


ACTION battletest::movemail(eosio::name _user, uint64_t _index)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 4");

    require_auth(_user);

    mail_db mail_db_table(_self, _user.value);
    auto mail_db_iter = mail_db_table.find(_index);
    eosio_assert(mail_db_iter != mail_db_table.end(), "Not exist mail data");

    auth_users auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(_user.value);

    eosio_assert((mail_db_iter->type == 1 || mail_db_iter->type == 2 | mail_db_iter->type == 3), "Not exist select type");
    if (mail_db_iter->type == 1) //서번트
    {
        
        eosio_assert((auth_user_iter->current_servant_inventory <= auth_user_iter->servant_inventory), "Your Need more inventory"); //인벤토리 공간이 있을때
        eosio_assert(check_inventory(_user) == true , "your inventory is full");
        user_servants user_servant_table(_self, _user.value);
        user_servant_table.emplace(_self, [&](auto &move_servant) {
            uint32_t first_index = user_servant_table.available_primary_key();
            if (first_index == 0)
            {
                move_servant.index = 1;
            }
            else
            {
                move_servant.index = user_servant_table.available_primary_key();
            }
            servant_db servant_db_table(_self, _self.value);
            auto servant_second = servant_db_table.get_index<"second"_n>();
            auto servant_db_iter = servant_second.find(mail_db_iter->type_id);

            uint32_t _job, _body, _gender, _head, _hair;
            uint32_t temp1, temp2, temp3, temp4;

            _job = servant_db_iter->servant_id / 1000000;
            temp1 = servant_db_iter->servant_id % 1000000;
            _body = temp1 / 100000;
            temp2 = temp1 % 100000;
            _gender = temp2 / 10000;
            temp3 = temp2 % 10000;
            _head = temp3 / 100;
            temp4 = temp3 % 100;
            _hair = temp4 / 1;

            move_servant.party_number = 0;
            move_servant.servant.state = object_state::on_inventory;
            move_servant.servant.exp = 0;
            move_servant.servant.appear.head = _head;
            move_servant.servant.appear.hair = _hair;
            move_servant.servant.appear.body = _body;
            move_servant.servant.appear.gender = _gender;
            move_servant.servant.job = _job;
            move_servant.servant.stat_point = 0;
            move_servant.servant.status.basic_str = mail_db_iter->status.basic_str;
            move_servant.servant.status.basic_dex = mail_db_iter->status.basic_dex;
            move_servant.servant.status.basic_int = mail_db_iter->status.basic_int;
            move_servant.servant.status.plus_str = mail_db_iter->status.plus_str;
            move_servant.servant.status.plus_dex = mail_db_iter->status.plus_dex;
            move_servant.servant.status.plus_int = mail_db_iter->status.plus_int;
            move_servant.servant.equip_slot.resize(3);
            move_servant.servant.passive_skill.resize(1);
            move_servant.servant.active_skill.resize(1);
        });
        auth_user_table.modify(auth_user_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.current_servant_inventory += 1;
        });
        // for (auto iter4 = mail_db_table.begin(); iter4 != mail_db_table.end();)
        // {

        //     iter4++;
        // }
    }
    else if (mail_db_iter->type == 2) //몬스터
    {
        //eosio_assert((auth_user_iter->current_monster_inventory <= auth_user_iter->monster_inventory), "Your Need more inventory"); //인벤토리 공간이 있을때
        eosio_assert(check_inventory(_user) == true , "your inventory is full");
        user_monsters user_monster_table(_self, _user.value);
        user_monster_table.emplace(_self, [&](auto &move_monster) {
            uint32_t first_index = user_monster_table.available_primary_key();
            if (first_index == 0)
            {
                move_monster.index = 1;
            }
            else
            {
                move_monster.index = user_monster_table.available_primary_key();
            }

            monster_db monster_db_table(_self, _self.value);
            auto monster_second = monster_db_table.get_index<"second"_n>();
            auto monster_db_iter = monster_second.find(mail_db_iter->type_id);
            move_monster.monster.id = monster_db_iter->monster_id;
            move_monster.party_number = 0;
            move_monster.monster.state = object_state::on_inventory;
            move_monster.monster.exp = 0;
            move_monster.monster.type = monster_db_iter->type;
            move_monster.monster.monster_class = monster_db_iter->monster_class;
            move_monster.monster.grade = mail_db_iter->grade;
            move_monster.monster.upgrade = 0;
            move_monster.monster.status.basic_str = mail_db_iter->status.basic_str;
            move_monster.monster.status.basic_dex = mail_db_iter->status.basic_dex;
            move_monster.monster.status.basic_int = mail_db_iter->status.basic_int;
            move_monster.monster.status.plus_str = mail_db_iter->status.plus_str;
            move_monster.monster.status.plus_dex = mail_db_iter->status.plus_dex;
            move_monster.monster.status.plus_int = mail_db_iter->status.plus_int;
            move_monster.monster.passive_skill.resize(3);
            move_monster.monster.active_skill.resize(3);
        });
        auth_user_table.modify(auth_user_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.current_monster_inventory += 1;
        });
    }
    else if (mail_db_iter->type == 3) //아이템
    {
        //eosio_assert((auth_user_iter->current_equipitem_inventory <= auth_user_iter->equipitem_inventory), "Your Need more inventory"); //인벤토리 공간이 있을때
        eosio_assert(check_inventory(_user) == true , "your inventory is full");
        user_equip_items user_item_table(_self, _user.value);
        user_item_table.emplace(_self, [&](auto &move_item) {
            uint32_t first_index = user_item_table.available_primary_key();
            if (first_index == 0)
            {
                move_item.index = 1;
            }
            else
            {
                move_item.index = user_item_table.available_primary_key();
            }

            equipitem_db equipitem_db_table(_self, _self.value);
            auto equipitem_second = equipitem_db_table.get_index<"second"_n>();
            auto equipitem_db_iter = equipitem_second.find(mail_db_iter->type_id);
            commonitem_db commonitem_db_table(_self, _self.value);
            auto commonitem_second = commonitem_db_table.get_index<"second"_n>();
            auto commonitem_db_iter = commonitem_second.find(mail_db_iter->type_id);

            move_item.item.id = commonitem_db_iter->item_id;
            move_item.item.state = object_state::on_inventory;
            move_item.item.type = commonitem_db_iter->type;
            move_item.item.tier = mail_db_iter->tier;
            move_item.item.job = mail_db_iter->job;
            move_item.item.grade = mail_db_iter->grade;
            move_item.item.upgrade = 0;
            move_item.item.atk = 0;
            move_item.item.def = 0;
            move_item.item.status.basic_str = mail_db_iter->status.basic_str;
            move_item.item.status.basic_dex = mail_db_iter->status.basic_dex;
            move_item.item.status.basic_int = mail_db_iter->status.basic_int;
            move_item.item.status.plus_str = mail_db_iter->status.plus_str;
            move_item.item.status.plus_dex = mail_db_iter->status.plus_dex;
            move_item.item.status.plus_int = mail_db_iter->status.plus_int;
        });
        auth_user_table.modify(auth_user_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.current_equipitem_inventory += 1;
        });
    }
    mail_db_table.erase(mail_db_iter);
}   

// void battletest::change_status(eosio::name _user, uint64_t _seed)
// {
//     auth_users auth_user_table(_self, _self.value);
//     auto auth_user_iter = auth_user_table.find(_user.value);
//     eosio_assert(auth_user_iter != auth_user_table.end(), "Not Exist auth user log");
//     eosio_assert(auth_user_iter->state == user_state::dice, "Impossible Status Dice");

//     uint64_t l_user = get_user_seed_value(_user.value);
//     uint64_t l_seed = safeseed::get_seed_value(l_user, _seed);

//     std::vector<uint64_t> status_list;
//     safeseed::get_total_rand(status_list, hero_total_status, l_seed);

//     auth_user_table.modify(auth_user_iter, _self, [&](auto &hero_look_set) {
//         hero_look_set.hero.status.basic_str = status_list[0];
//         hero_look_set.hero.status.basic_dex = status_list[1];
//         hero_look_set.hero.status.basic_int = status_list[2];
//     });
// }

// eosio.token recipient
// memo description spec
//-------------------------------------------------------------------------

ACTION battletest::eostransfer(eosio::name sender, eosio::name receiver)
{
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(sender.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "BlackList User 3");

    eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
        eosio_assert(ad.action.size() != 0, "Wrong Action");
        //eosio_assert(ad.action != action_signup, "Need Presignup");
        if (ad.action == action_signup)
        {
            signup(sender);
        }
        else if (ad.action == action_gacha)
        {
            start_gacha(sender, ad.type);
        }
        else if(ad.action == "cheat")
        {
            herocheat(sender);
            partycheat(sender);
            balancetest(sender);
        }

    });
}

template <typename T>
void battletest::eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func)
{
    require_auth(sender);
    auto transfer_data = eosio::unpack_action_data<st_transfer>();

 	eosio_assert(transfer_data.to == receiver, "Wrong Action 1");


    eosio_assert(transfer_data.quantity.symbol == symbol("EOS", 4), "Only Accepts EOS for deposits");
    eosio_assert(transfer_data.quantity.is_valid(), "Invalid token transfer");
    eosio_assert(transfer_data.quantity.amount > 0, "Quantity must be positive");

    transfer_action res;
    size_t l_center = transfer_data.memo.find(':');

    res.action = transfer_data.memo.substr(0, l_center);

    if(transfer_data.to != _self)
    {
        if (transfer_data.to != "eosio.token"_n)
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
    }   
    
	else if (res.action == "gacha")
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

        whitelist whitelist_table(_self, _self.value);
        auto whitelist_iter = whitelist_table.find(sender.value);
        eosio_assert(whitelist_iter != whitelist_table.end(), "Not White User 1");

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

    servant_db servant_id_table(_self, _self.value);
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
    uint32_t random_item_id = safeseed::get_random_value(_seed, ITEM_ID_COUNT, DEFAULT_MIN_DB, item_random_count);
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
    user_equip_items user_item_table(_self, _user.value);
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
    
    eosio_assert(check_inventory(_user) == true , "your inventory is full");

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

void battletest::tutorial_servant(eosio::name _user, uint64_t _seed)
{
    servant_job_db servant_job_table(_self, _self.value);
    uint32_t random_job = 2;
    const auto &servant_job_db_iter = servant_job_table.get(random_job, "Not Get Servant Job Data 2");
    uint32_t random_body = 1;
    gender_db gender_table(_self, _self.value);
    uint32_t random_gender = 2;
    const auto &gender_db_iter = gender_table.get(random_gender, "Not Get Gender Data 2");

    uint32_t random_head = 1;
    uint32_t random_hair = 1;

    servant_id_db servant_id_table(_self, _self.value);
    uint32_t servant_index = 1110202;
    const auto &servant_id_db_iter = servant_id_table.get(servant_index, "Not Exist Servant ID 2");

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Not Exist User Log 8");

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
        update_user_servant_list.party_number = 0;
        update_user_servant_list.servant.appear.head = random_head;
        update_user_servant_list.servant.appear.hair = random_hair;
        update_user_servant_list.servant.appear.body = random_body;
        update_user_servant_list.servant.appear.gender = random_gender;
        update_user_servant_list.servant.job = random_job;
        update_user_servant_list.servant.status.basic_str = 47;
        update_user_servant_list.servant.status.basic_dex = 47;
        update_user_servant_list.servant.status.basic_int = 47;
        update_user_servant_list.servant.equip_slot.resize(3);
        update_user_servant_list.servant.passive_skill.resize(1);
        update_user_servant_list.servant.active_skill.resize(1);


        // std::string servant_result = "ser:";
        // servant_result += to_string(update_user_servant_list.index) + ":";
        // servant_result += to_string(update_user_servant_list.id) + ":";
        // servant_result += to_string(update_user_servant_list.status.basic_str) + ":";
        // servant_result += to_string(update_user_servant_list.status.basic_dex) + ":";
        // servant_result += to_string(update_user_servant_list.status.basic_int) + ":";

        // action(permission_level{get_self(), "active"_n},
        //        get_self(), "resultgacha"_n,
        //        std::make_tuple(_self, _user ,servant_result))
        //     .send();

        result.index = update_user_servant_list.index;
        result.type = result::servant;
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

    // auth_users auth_user_table(_self, _self.value);
    // auto auth_user_iter = auth_user_table.find(_user.value);

    // auth_user_table.modify(auth_user_iter, _self, [&](auto &update_users){
    //     update_users.current_servant_inventory +=1;        
    // });  

}

bool battletest::check_inventory(eosio::name _user)
{
    auth_users auth_user_table(_self, _self.value);
    auto auth_users_iter = auth_user_table.find(_user.value);

    if(auth_users_iter->current_servant_inventory >= auth_users_iter->servant_inventory)
    {
        return false;
    }
    else if(auth_users_iter->current_monster_inventory >= auth_users_iter->monster_inventory)
    {
        return false;
    }
    else if(auth_users_iter->current_equipitem_inventory >= auth_users_iter->equipitem_inventory)
    {
        return false;
    }
    else if(auth_users_iter->current_item_inventory >= auth_users_iter->item_inventory)
    {
        return false;
    }
    else
    {
        return true;
    }    
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

ACTION battletest::battlestate(eosio::name _who, std::vector<std::string> &_my_state_list, std::vector<std::string> &_enemy_state_list)
{
    require_auth(_self);
    require_recipient(_self);
}

ACTION battletest::battleaction(eosio::name _who, std::string _header ,std::vector<std::string> &_data)
{
    require_auth(_self);
    require_recipient(_self);
}
#pragma endresion

#pragma region party function

//------------------------------------------------------------------------//
//-------------------------------party_function---------------------------//
//------------------------------------------------------------------------//

ACTION battletest::saveparty(eosio::name _user, uint32_t _party_number, const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list)
{
    require_auth(_user);
    eosio_assert(_party_number > 0, "wrong party_number");                                                   //잘못된 파티 넘버 체크
    user_partys user_party_table(_self, _user.value);                                                        //
    auto user_party_iter = user_party_table.find(_party_number);                                             //유저 파티 이터레이터는 파티 테이블 번호
    eosio_assert(user_party_iter != user_party_table.end(), "not exist party");                              //유저 파티 이터레이터를 기준으로 테이블 조회 시 없으면
    eosio_assert(user_party_iter->state != party_state::on_tower_defense, "this party on tower unmodified"); //유저 파티 상태가 타워 디펜스중이 아닐시
    user_servants user_servant_table(_self, _user.value);
    user_monsters user_monster_table(_self, _user.value);

 	for (uint32_t i = 0; i < user_party_iter->servant_list.size(); ++i)
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
    for (uint32_t i = 0; i < user_party_iter->monster_list.size(); ++i)
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
      for (uint32_t i = 0; i < user_party_iter->servant_list.size(); ++i)//서번트에 대한 파티 배치 처리
        {
            auto user_servant_iter = user_servant_table.find(_servant_list[i]);
            eosio_assert(user_servant_iter != user_servant_table.end(), "not exist servant data");
            if (_servant_list[i] == EMPTY_PARTY)
            {
                save_party.servant_list[i] = _servant_list[i];
                continue;
            }
            eosio_assert(user_servant_iter->party_number == EMPTY_PARTY || user_servant_iter->party_number == _party_number, "already in party member servant");
            user_servant_table.modify(user_servant_iter, owner, [&](auto &set_party) {
                set_party.party_number = _party_number;
            });
            save_party.servant_list[i] = _servant_list[i];
        }
         for (uint32_t i = 0; i < user_party_iter->monster_list.size(); ++i)
        {
            auto user_monster_iter = user_monster_table.find(_monster_list[i]);
            eosio_assert(user_monster_iter != user_monster_table.end(), "not exist monster data");  
			if (_monster_list[i] == EMPTY_PARTY)
            {
                save_party.monster_list[i] = _monster_list[i];
                continue;
            }

          
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
        //new_hero.appear.body = 1;
        //new_hero.appear.head = 1;
        //new_hero.appear.hair = 1;
        //new_hero.appear.gender = 1;
        //new_hero.job = 1;
        //new_hero.status.basic_str = 100;
        //new_hero.status.basic_dex = 100;
        //new_hero.status.basic_int = 100;
        //new_hero.equip_slot.resize(3);
        new_user.current_servant_inventory = 0;
        new_user.current_monster_inventory = 0;
        new_user.current_item_inventory = 0;
        new_user.current_equipitem_inventory = 0;
        new_user.servant_inventory = 50;
        new_user.monster_inventory = 50;
        new_user.item_inventory = 50;
        new_user.equipitem_inventory = 50;

        //new_user.hero = new_hero;
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
        new_party.servant_list.resize(5);
        new_party.monster_list.resize(5);
    });
}

ACTION battletest::partycheat(eosio::name _user)
{
    require_auth(_user);
    uint64_t user_value = get_user_seed_value(_user.value);
    uint64_t seed = safeseed::get_seed_value(user_value, now());

    user_consum_item user_consumables_table(_self, _user.value);
    user_consumables_table.emplace(_self, [&](auto &new_consumables) {
        new_consumables.id = 40001;
        new_consumables.type = 1;
        new_consumables.count = 100;
    });

    for (uint32_t i = 0; i < 5; ++i)
    {
        gacha_servant_id(_user, seed);
    }
    for (uint32_t i = 0; i < 5; ++i)
    {
        gacha_monster_id(_user, seed);
    }
    // for (uint32_t i = 0; i < 10; ++i)
    // {
    //     gacha_item_id(_user, seed);
    // }


    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.find(1);
    if (user_party_iter == user_party_table.end())
    {
        user_party_table.emplace(_self, [&](auto &automatic_party) {
            automatic_party.index = 1;
            for (uint32_t i = 0; i < 5; ++i)
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
            for (uint32_t i = 0; i < 5; ++i)
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

    // body_db body_table("overflow1111"_n, "overflow1111"_n.value);
    // for (auto iter = body_table.begin(); iter != body_table.end();)
    // {
    //     const auto &data_iter = body_table.get(iter->primary_key(), "Not Exist Data");
    //     insert_body(data_iter.body);
    //     iter++;
    // }

    // hair_db hair_table("overflow1111"_n, "overflow1111"_n.value);
    // for (auto iter = hair_table.begin(); iter != hair_table.end();)
    // {
    //     const auto &data_iter = hair_table.get(iter->primary_key(), "Not Exist Data");
    //     insert_hair(data_iter.hair);
    //     iter++;
    // }

    // head_db head_table("overflow1111"_n, "overflow1111"_n.value);
    // for (auto iter = head_table.begin(); iter != head_table.end();)
    // {
    //     const auto &data_iter = head_table.get(iter->primary_key(), "Not Exist Data");
    //     insert_head(data_iter.head);
    //     iter++;
    // }

    // gender_db gender_table("overflow1111"_n, "overflow1111"_n.value);
    // for (auto iter = gender_table.begin(); iter != gender_table.end();)
    // {
    //     const auto &data_iter = gender_table.get(iter->primary_key(), "Not Exist Data");
    //     insert_gender(data_iter.gender);
    //     iter++;
    // }

    // item_grade_db item_grade_table("overflow1111"_n, "overflow1111"_n.value);
    // for (auto iter = item_grade_table.begin(); iter != item_grade_table.end();)
    // {
    //     const auto &data_iter = item_grade_table.get(iter->primary_key(), "Not Exist Data");
    //     insert_item_grade(std::string("all"), data_iter.grade, 0, 10);
    //     iter++;
    // }

    // monster_grade_db monster_grade_table("overflow1111"_n, "overflow1111"_n.value);
    // for (auto iter = monster_grade_table.begin(); iter != monster_grade_table.end();)
    // {
    //     const auto &data_iter = monster_grade_table.get(iter->primary_key(), "Not Exist Data");
    //     insert_monster_grade(std::string("all"), data_iter.grade, 0, 10);
    //     iter++;
    // }

    // servant_job_db servant_job_table("overflow1111"_n, "overflow1111"_n.value);
    // for (auto iter = servant_job_table.begin(); iter != servant_job_table.end();)
    // {
    //     const auto &data_iter = servant_job_table.get(iter->primary_key(), "Not Exist Data");
    //     insert_job(std::string("all"), data_iter.job, 0, 10);
    //     iter++;
    // }

    // monster_db monster_table("overflow1111"_n, "overflow1111"_n.value);
    // for (auto iter = monster_table.begin(); iter != monster_table.end();)
    // {
    //     const auto &data_iter = monster_table.get(iter->primary_key(), "Not Exist Data");
    //     insert_monster_id(data_iter.monster_id, data_iter.gacha_id, data_iter.gacha_type, data_iter.tribe, data_iter.type, data_iter.monster_class, 0, 0, 0);
    //     iter++;
    // }

    // servant_db servant_table("overflow1111"_n, "overflow1111"_n.value);
    // for (auto iter = servant_table.begin(); iter != servant_table.end();)
    // {
    //     const auto &data_iter = servant_table.get(iter->primary_key(), "Not Exist Data");
    //     insert_servant_id(data_iter.servant_id, data_iter.gacha_id);
    //     iter++;
    // }

    // commonitem_db common_item_table("overflow1111"_n, "overflow1111"_n.value);
    // for (auto iter = common_item_table.begin(); iter != common_item_table.end();)
    // {
    //     const auto &data_iter = common_item_table.get(iter->primary_key(), "Not Exist Data");
    //     insert_common_item_id(data_iter.item_id,
    //                           data_iter.gacha_id,
    //                           data_iter.type,
    //                           data_iter.param_1,
    //                           data_iter.param_2,
    //                           data_iter.param_3,
    //                           data_iter.sell_id,
    //                           data_iter.sell_cost);
    //     iter++;
    // }

    equipitem_db equip_item_table("unlimitedmas"_n, "unlimitedmas"_n.value);
    for (auto iter = equip_item_table.begin(); iter != equip_item_table.end();)
    {
        const auto &data_iter = equip_item_table.get(iter->primary_key(), "Not Exist Data");
        insert_equip_item_id(data_iter.item_id,
                             data_iter.set_id,
                             data_iter.type,
                             data_iter.tier,
                             data_iter.job,
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
uint32_t battletest::get_max_hp(total_status _status)
{
    uint32_t hp;
    hp = ((_status.total_str * 14) + (_status.total_dex * 5) + (_status.total_int  * 3));
    return hp;
}
uint32_t battletest::get_magic_attack(total_status _status)
{
    uint32_t magic_attack;
    magic_attack = ((_status.total_int) * 220) / decimal; //2.2 * 100
    return magic_attack;
}
uint32_t battletest::get_physical_attack(total_status _status)
{
    uint32_t physical_attack;
    physical_attack = ((_status.total_str + _status.total_dex) * 140) / decimal; //1.4 * 100
    return physical_attack;
}
uint32_t battletest::get_magic_defense(total_status _status)
{
    uint32_t magic_defense;
    magic_defense = ((_status.total_int) * 130) / decimal; //1.3 * 100
    return magic_defense;
}
uint32_t battletest::get_physical_defense(total_status _status)
{
    uint32_t physical_defense;
    physical_defense = ((_status.total_dex) * 130) / decimal; //1.3 * 100
    return physical_defense;

}


battletest::status_info battletest::get_level_up_monster_status(uint64_t _id, uint64_t _grade, status_info _status)
{
    status_info status = _status;
    monster_db monster_db_table(_self, _self.value);
    auto monster_iter = monster_db_table.find(_id);
    eosio_assert(monster_iter != monster_db_table.end(), "Not Exist Monster ID 4");
   if (monster_iter->main_status == monster_main_status::main_str) //??    {
        if (_grade == grade_list::common)
        {
            status.basic_str += common_str[0];
            status.basic_dex += common_str[1];
            status.basic_int += common_str[2];
        }
        else if (_grade == grade_list::uncommon)
        {
            status.basic_str += uncommon_str[0];
            status.basic_dex += uncommon_str[1];
            status.basic_int += uncommon_str[2];
        }
        else if (_grade == grade_list::rare)
        {
            status.basic_str += rare_str[0];
            status.basic_dex += rare_str[1];
            status.basic_int += rare_str[2];
        }
        else if (_grade == grade_list::unique)
        {
            status.basic_str += unique_str[0];
            status.basic_dex += unique_str[1];
            status.basic_int += unique_str[2];
        }
        else if (_grade == grade_list::legenary)
        {
            status.basic_str += legenary_str[0];
            status.basic_dex += legenary_str[1];
            status.basic_int += legenary_str[2];
        }
        return status;
    }
    else if (monster_iter->main_status == monster_main_status::main_dex) //誘?    {
        if (_grade == grade_list::common)
        {
            status.basic_str += common_dex[0];
            status.basic_dex += common_dex[1];
            status.basic_int += common_dex[2];
        }
        else if (_grade == grade_list::uncommon)
        {
            status.basic_str += uncommon_dex[0];
            status.basic_dex += uncommon_dex[1];
            status.basic_int += uncommon_dex[2];
        }
        else if (_grade == grade_list::rare)
        {
            status.basic_str += rare_dex[0];
            status.basic_dex += rare_dex[1];
            status.basic_int += rare_dex[2];
        }
        else if (_grade == grade_list::unique)
        {
            status.basic_str += unique_dex[0];
            status.basic_dex += unique_dex[1];
            status.basic_int += unique_dex[2];
        }
        else if (_grade == grade_list::legenary)
        {
            status.basic_str += legenary_dex[0];
            status.basic_dex += legenary_dex[1];
            status.basic_int += legenary_dex[2];
        }
        return status;
    }
    else if (monster_iter->main_status == monster_main_status::main_int) //吏
    {
        if (_grade == grade_list::common)
        {
            status.basic_str += common_int[0];
            status.basic_dex += common_int[1];
            status.basic_int += common_int[2];
        }
        else if (_grade == grade_list::uncommon)
        {
            status.basic_str += uncommon_int[0];
            status.basic_dex += uncommon_int[1];
            status.basic_int += uncommon_int[2];
        }
        else if (_grade == grade_list::rare)
        {
            status.basic_str += rare_int[0];
            status.basic_dex += rare_int[1];
            status.basic_int += rare_int[2];
        }
        else if (_grade == grade_list::unique)
        {
            status.basic_str += unique_int[0];
            status.basic_dex += unique_int[1];
            status.basic_int += unique_int[2];
        }
        else if (_grade == grade_list::legenary)
        {
            status.basic_str += legenary_int[0];
            status.basic_dex += legenary_int[1];
            status.basic_int += legenary_int[2];
        }
        return status;
    }
    return status;
}
battletest::status_info battletest::get_level_up_servant_status(uint64_t _job, status_info _status)
{
    status_info status = _status;

    if (_job == job_list::warrior)
    {
        status.basic_str += warrior_level_up[0];
        status.basic_dex += warrior_level_up[1];
        status.basic_int += warrior_level_up[2];
        return status;
    }
    else if(_job == job_list::thief)
    {
        status.basic_str += thief_level_up[0];
        status.basic_dex += thief_level_up[1];
        status.basic_int += thief_level_up[2];
        return status;
    }
    else if (_job == job_list::archer )
    {
        status.basic_str += archer_level_up[0];
        status.basic_dex += archer_level_up[1];
        status.basic_int += archer_level_up[2];
        return status;
    }
    else if(_job == job_list::priest)
    {
        status.basic_str += cleric_level_up[0];
        status.basic_dex += cleric_level_up[1];
        status.basic_int += cleric_level_up[2];
        return status;
    }
    else if (_job == job_list::wizard)
    {
        status.basic_str += magician_level_up[0];
        status.basic_dex += magician_level_up[1];
        status.basic_int += magician_level_up[2];
        return status;
    }
    return status;
}


battletest::status_info battletest::get_grade_status(uint64_t _grade, status_info _status)
{
    status_info status;
    if(_grade == grade_list::ser)
    {
        status.basic_str = servant_status_list[_status.basic_str];
        status.basic_dex = servant_status_list[_status.basic_dex];
        status.basic_int = servant_status_list[_status.basic_int];
    }
    else if (_grade == grade_list::legenary)
    {
        status.basic_str = monster_legendary_status_list[_status.basic_str];
        status.basic_dex = monster_legendary_status_list[_status.basic_dex];
        status.basic_int = monster_legendary_status_list[_status.basic_int];
    }
    else if (_grade == grade_list::unique)
    {
        status.basic_str = monster_unique_status_list[_status.basic_str];
        status.basic_dex = monster_unique_status_list[_status.basic_dex];
        status.basic_int = monster_unique_status_list[_status.basic_int];
    }
    else if (_grade == grade_list::rare)
    {
        status.basic_str = monster_rare_status_list[_status.basic_str];
        status.basic_dex = monster_rare_status_list[_status.basic_dex];
        status.basic_int = monster_rare_status_list[_status.basic_int];
    }
    else if (_grade == grade_list::uncommon)
    {
        status.basic_str = monster_uncommon_status_list[_status.basic_str];
        status.basic_dex = monster_uncommon_status_list[_status.basic_dex];
        status.basic_int = monster_uncommon_status_list[_status.basic_int];
    }
    else if (_grade == grade_list::common)
    {
        status.basic_str = monster_common_status_list[_status.basic_str];
        status.basic_dex = monster_common_status_list[_status.basic_dex];
        status.basic_int = monster_common_status_list[_status.basic_int];
    }
    return status;
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
    _atk = _atk / decimal;
    uint32_t damage = (_atk * ((defense_constant * decimal) / (defense_constant + (_dfs / decimal))));
    return damage;
}


void battletest::set_stage_state(uint64_t _stage_id, std::vector<battle_state> &_enemy_state_list ,std::vector<std::string> &_state)
{
    battle_state get_state;

    stage_db stage_db_table(_self, _self.value);
    auto stage_db_iter = stage_db_table.find(_stage_id);
    eosio_assert(stage_db_iter != stage_db_table.end(), "Not Exist Stage ID 2");
    for (uint32_t i = 0; i < stage_db_iter->monster_list.size(); ++i)
    {
        status_info stage_status;
        stage_status.basic_str = monster_common_status_list[4];
        stage_status.basic_dex = monster_common_status_list[4];
        stage_status.basic_int = monster_common_status_list[4];

        uint64_t level = safeseed::get_random_value(now(), stage_db_iter->monster_level_max, stage_db_iter->monster_level_min, 0);
        for (uint32_t j = 2; j <= level; ++j)
        {
            status_info level_up;
            level_up = get_level_up_monster_status(stage_db_iter->monster_list[i].monster_id, grade_list::rare, stage_status);
            stage_status.basic_str += level_up.basic_str;
            stage_status.basic_dex += level_up.basic_dex;
            stage_status.basic_int += level_up.basic_int;
        }

        total_status status;
        status.total_str = (stage_status.basic_str + stage_status.plus_str) * decimal;
        status.total_dex = (stage_status.basic_dex + stage_status.plus_dex) * decimal;
        status.total_int = (stage_status.basic_int + stage_status.plus_int) * decimal;

        get_state.speed = get_speed(beginner);
        get_state.id = stage_db_iter->monster_list[i].monster_id;
        get_state.index = 0;
        get_state.position = stage_db_iter->monster_list[i].position;

        get_state.now_hp = get_max_hp(status);
        get_state.physical_attack = get_physical_attack(status);
        get_state.physical_crit_dmg = get_physical_attack(status) * oper_critical_damage / 10000;
        get_state.physical_defense = get_physical_defense(status);
        get_state.magic_attack = get_magic_attack(status);
        get_state.magic_crit_dmg = get_magic_attack(status) * oper_critical_damage / 10000;
        get_state.magic_defense = get_magic_defense(status);
        get_state.physical_crit_per = oper_critical;
        get_state.magic_crit_per = oper_critical;
        get_state.avoid = 5;
        get_state.state = battle_member_state::live;
        get_state.status = status;
        _enemy_state_list.push_back(get_state);

        std::string state;
        state += to_string(get_state.position) + ":";
        state += to_string(get_state.index) + ":";
        state += to_string(get_state.id) + ":";
        state += to_string(get_state.now_hp) + ":";
        state += to_string(get_state.physical_attack) + ":";
        state += to_string(get_state.magic_attack) + ":";
        state += to_string(get_state.physical_defense) + ":";
        state += to_string(get_state.magic_defense) + ":";
        state += to_string(get_state.physical_crit_per) + ":";
        state += to_string(get_state.magic_crit_per) + ":";
        state += to_string(get_state.physical_crit_dmg) + ":";
        state += to_string(get_state.magic_crit_dmg) + ":";
        state += to_string(get_state.avoid) + ":";
        state += to_string(get_state.state) + ":";
        state += to_string(get_state.speed) + ":";
        state += to_string(get_state.type) + ":";
        state += to_string(get_state.job_class) + ":";
        state += "[";
        state += "]";
        state += ":";
        if (get_state.passive_skill_list.size() == 0)
        {
            state += "[";
            state += "]";
            state += ":";
        }
        else
        {
            state += "[";
            for (uint32_t i = 0; i < get_state.passive_skill_list.size(); ++i)
            {
                state += to_string(get_state.passive_skill_list[i].id) + ":";
                state += to_string(get_state.passive_skill_list[i].per) + ":";
                state += to_string(get_state.passive_skill_list[i].attack_type) + ":";
                state += to_string(get_state.passive_skill_list[i].dmg_type) + ":";
                state += to_string(get_state.passive_skill_list[i].target) + ":";
                if (get_state.passive_skill_list.size() - 1 == i)
                {
                    state += to_string(get_state.passive_skill_list[i].target_count);
                }
                else
                {
                    state += to_string(get_state.passive_skill_list[i].target_count) + ":";
                }
            }
            state += "]";
            state += ":";
        }

        if (get_state.active_skill_list.size() == 0)
        {
            state += "[";
            state += "]";
            state += ":";
        }
        else
        {
            state += "[";
            for (uint32_t i = 0; i < get_state.active_skill_list.size(); ++i)
            {
                state += to_string(get_state.active_skill_list[i].id) + ":";
                state += to_string(get_state.active_skill_list[i].per) + ":";
                state += to_string(get_state.active_skill_list[i].attack_type) + ":";
                state += to_string(get_state.active_skill_list[i].dmg_type) + ":";
                state += to_string(get_state.active_skill_list[i].target) + ":";
                if (get_state.active_skill_list.size() - 1 == i)
                {
                    state += to_string(get_state.active_skill_list[i].target_count);
                }
                else
                {
                    state += to_string(get_state.active_skill_list[i].target_count) + ":";
                }
            }
            state += "]";
            state += ":";
        }
        state += to_string(get_state.status.total_str) + ":";
        state += to_string(get_state.status.total_dex) + ":";
        state += to_string(get_state.status.total_int);

        _state.push_back(state);
    }
}



battletest::battle_state battletest::get_user_state(eosio::name _user, std::string _type, uint64_t _index, uint32_t _position, std::vector<std::string> &_state)
{
    battle_state get_state;
    total_status status;
    
    if(_type == "ser")
    {
        user_servants user_servant_table(_self, _user.value);
        auto user_servant_iter = user_servant_table.find(_index);
        eosio_assert(user_servant_iter != user_servant_table.end(),"Not Exist Servant 10");

        status.total_str = (user_servant_iter->servant.status.basic_str + user_servant_iter->servant.status.plus_str) * decimal;
        status.total_dex = (user_servant_iter->servant.status.basic_dex + user_servant_iter->servant.status.plus_dex) * decimal;
        status.total_int = (user_servant_iter->servant.status.basic_int + user_servant_iter->servant.status.plus_int) * decimal;

        //?⑥떆釉뚯쓽 寃쎌슦 ?꾪닾吏꾩엯?쒖뿉 ?곸슜?섎뒗 寃껊쭔 ?곸슜 ?쒖폒 以섏빞 ??        // if (user_servant_iter->servant.passive_skill.size() != 0)
        // {
        //     passive_db passive_db_table(_self, _self.value);
        //     for (uint32_t i = 0; i < user_servant_iter->servant.passive_skill.size(); ++i)
        //     {
        //         auto passive_db_iter = passive_db_table.find(user_servant_iter->servant.passive_skill[i]);
        //         eosio_assert(passive_db_iter != passive_db_table.end(), "Not Exist Servant Passive 1");
        //         if (passive_db_iter->passive_id == 100006) //??n% 利앷?
        //         {
        //             status.total_str += (status.total_str * passive_db_iter->effect_value) / 100;
        //         }
        //         else if (passive_db_iter->passive_id == 100007) //誘쇱꺽 n% 利앷?
        //         {
        //             status.total_dex += (status.total_dex * passive_db_iter->effect_value) / 100;
        //         }
        //         else if (passive_db_iter->passive_id == 100008) //吏??n% 利앷?
        //         {
        //             status.total_int += (status.total_int * passive_db_iter->effect_value) / 100;
        //         }
        //         skill_info passive;
        //         passive.id = user_servant_iter->servant.passive_skill[i];
        //         passive.per = 100;
        //         get_state.passive_skill_list.push_back(passive);
        //     }
        // }

        // if (user_servant_iter->servant.active_skill.size() != 0)
        // {
        //     active_db active_db_table(_self, _self.value);
        //     for (uint32_t i = 0; i < user_servant_iter->servant.active_skill.size(); ++i)
        //     {

        //         skill_info active;
        //         if (user_servant_iter->servant.job == job_list::warrior)
        //         {
        //             active.id = active_name::active_defense;
        //         }
        //         else if (user_servant_iter->servant.job == job_list::thief)
        //         {
        //             active.id = (active_name::active_fast_attack);
        //         }
        //         else if (user_servant_iter->servant.job == job_list::archer)
        //         {
        //             active.id = (active_name::active_multi_shot);
        //         }
        //         else if (user_servant_iter->servant.job == job_list::wizard)
        //         {
        //             active.id = (active_name::active_magic_strike);
        //         }
        //         else if (user_servant_iter->servant.job == job_list::priest)
        //         {
        //             active.id = (active_name::active_guided_arrow);
        //         }
        //         else
        //         {
        //             active.id = active_name::active_bash);
        //         }
        //         // auto active_db_iter = active_db_table.find(user_servant_iter->servant.active_skill[i]);
        //         // eosio_assert(active_db_iter != active_db_table.end(), "Not Exist Servant Active 1");

        //         auto active_db_iter = active_db_table.find(active.id);
        //         eosio_assert(active_db_iter != active_db_table.end(), "Not Exist Servant Active 1");

                
        //         active.id = active_db_iter->active_id;
        //         active.per = active_db_iter->active_per;
        //         active.attack_type = active_db_iter->attack_type;
        //         active.dmg_type = active_db_iter->dmg_type;
        //         active.target = active_db_iter->target;
        //         active.target_count = active_db_iter->target_count;
        //         get_state.active_skill_list.push_back(active);
        //     }
        // }

        active_db active_db_table(_self, _self.value);
        skill_info active;
        if (user_servant_iter->servant.job == job_list::warrior)
        {
            active.id = active_name::active_defense;
        }
        else if (user_servant_iter->servant.job == job_list::thief)
        {
            active.id = (active_name::active_fast_attack);
        }
        else if (user_servant_iter->servant.job == job_list::archer)
        {
            active.id = (active_name::active_multi_shot);
        }
        else if (user_servant_iter->servant.job == job_list::wizard)
        {
            active.id = (active_name::active_magic_strike);
        }
        else if (user_servant_iter->servant.job == job_list::priest)
        {
            active.id = (active_name::active_guided_arrow);
        }
        else
        {
            active.id = (active_name::active_bash);
        }
        // auto active_db_iter = active_db_table.find(user_servant_iter->servant.active_skill[i]);
        // eosio_assert(active_db_iter != active_db_table.end(), "Not Exist Servant Active 1");

        auto active_db_iter = active_db_table.find(active.id);
        eosio_assert(active_db_iter != active_db_table.end(), "Not Exist Servant Active 1");

        active.id = active_db_iter->active_id;
        active.per = active_db_iter->active_per;
        active.attack_type = active_db_iter->attack_type;
        active.dmg_type = active_db_iter->dmg_type;
        active.target = active_db_iter->target;
        active.target_count = active_db_iter->target_count;
        get_state.active_skill_list.push_back(active);

        get_state.speed = get_speed(user_servant_iter->servant.job);
        get_state.index = _index;
        get_state.id = get_servant_index(user_servant_iter->servant.job, user_servant_iter->servant.appear.body, user_servant_iter->servant.appear.gender, user_servant_iter->servant.appear.head, user_servant_iter->servant.appear.hair);
        get_state.type = 0;
        get_state.job_class = 0;
        get_state.position = _position;
    }
    else if(_type == "mon")
    {
        user_monsters user_monster_table(_self, _user.value);
        auto user_monster_iter = user_monster_table.find(_index);
        eosio_assert(user_monster_iter != user_monster_table.end(),"Not Exist Monster 10");

        status.total_str = (user_monster_iter->monster.status.basic_str + user_monster_iter->monster.status.plus_str) * decimal;
        status.total_dex = (user_monster_iter->monster.status.basic_dex + user_monster_iter->monster.status.plus_dex) * decimal;
        status.total_int = (user_monster_iter->monster.status.basic_int + user_monster_iter->monster.status.plus_int) * decimal;

        // if (user_monster_iter->monster.passive_skill.size() != 0)
        // {
        //     passive_db passive_db_table(_self, _self.value);
        //     for (uint32_t i = 0; i < user_monster_iter->monster.passive_skill.size(); ++i)
        //     {
        //         auto passive_db_iter = passive_db_table.find(user_monster_iter->monster.passive_skill[i]);
        //         eosio_assert(passive_db_iter != passive_db_table.end(), "Not Exist Servant Passive 1");
        //         if (passive_db_iter->passive_id == 100006) //??n% 利앷?
        //         {
        //             status.total_str += (status.total_str * passive_db_iter->effect_value) / 100;
        //         }
        //         else if (passive_db_iter->passive_id == 100007) //誘쇱꺽 n% 利앷?
        //         {
        //             status.total_dex += (status.total_dex * passive_db_iter->effect_value) / 100;
        //         }
        //         else if (passive_db_iter->passive_id == 100008) //吏??n% 利앷?
        //         {
        //             status.total_int += (status.total_int * passive_db_iter->effect_value) / 100;
        //         }
        //         skill_info passive;
        //         passive.id = user_monster_iter->monster.passive_skill[i];
        //         passive.per = 100;
        //         get_state.passive_skill_list.push_back(passive);
        //     }
        // }

        // if (user_monster_iter->monster.active_skill.size() != 0)
        // {
        //     active_db active_db_table(_self, _self.value);
        //     for (uint32_t i = 0; i < user_monster_iter->monster.active_skill.size(); ++i)
        //     {
        //         auto active_db_iter = active_db_table.find(user_monster_iter->monster.active_skill[i]);
        //         eosio_assert(active_db_iter != active_db_table.end(),"Not Exist Monster Active 1");

        //         skill_info active;
        //         active.id = active_db_iter->active_id;
        //         active.per = active_db_iter->active_per;
        //         active.attack_type = active_db_iter->attack_type;
        //         active.dmg_type = active_db_iter->dmg_type;
        //         active.target = active_db_iter->target;
        //         active.target_count = active_db_iter->target_count;
        //         get_state.active_skill_list.push_back(active);
        //     }
        // }
        active_db active_db_table(_self, _self.value);

        skill_info active;
        if(_index < 3)
        {
            active.id = active_name::active_bash;
        }
        else
        {
          active.id = active_name::active_magic_strike;
        }
        
        auto active_db_iter = active_db_table.find(active.id);
        eosio_assert(active_db_iter != active_db_table.end(), "Not Exist Monster Active 1");

        active.id = active_db_iter->active_id;
        active.per = active_db_iter->active_per;
        active.attack_type = active_db_iter->attack_type;
        active.dmg_type = active_db_iter->dmg_type;
        active.target = active_db_iter->target;
        active.target_count = active_db_iter->target_count;
        get_state.active_skill_list.push_back(active);

        get_state.speed = get_speed(beginner);
        get_state.index = _index;
        get_state.id = user_monster_iter->monster.id;
        get_state.type = user_monster_iter->monster.type;
        get_state.job_class = 0;
        get_state.position = _position;
    }

    get_state.now_hp = get_max_hp(status);
    get_state.physical_attack = get_physical_attack(status);
    get_state.physical_crit_dmg = get_physical_attack(status) * oper_critical_damage / 10000;
    get_state.physical_defense = get_physical_defense(status);
    get_state.magic_attack = get_magic_attack(status);
    get_state.magic_crit_dmg = get_magic_attack(status) * oper_critical_damage / 10000;
    get_state.magic_defense = get_magic_defense(status);
    get_state.physical_crit_per = oper_critical;
    get_state.magic_crit_per = oper_critical;
    get_state.avoid = 5;
    get_state.state = battle_member_state::live;
    get_state.status = status;

    if (get_state.passive_skill_list.size() != 0)
    {
        //?⑥떆釉뚭? ?꾪닾?쒖뿉留??곸슜?섎뒗吏 ?먮떒 ?꾩슂
        for (uint32_t i = 0; i < get_state.passive_skill_list.size(); ++i)
        {
            passive_db passive_db_table(_self, _self.value);
            auto passive_db_iter = passive_db_table.find(get_state.passive_skill_list[i].id);
            eosio_assert(passive_db_iter != passive_db_table.end(),"Not Exist Passive Skill");
            if (passive_db_iter->passive_id == 100001) //臾쇨났 n%利앷?
            {
                get_state.physical_attack += (get_state.physical_attack * passive_db_iter->effect_value) / 100;
            }
            else if (passive_db_iter->passive_id == 100002) //留덇났 n%利앷?
            {
                get_state.magic_attack += (get_state.magic_attack * passive_db_iter->effect_value) / 100;
            }
            else if (passive_db_iter->passive_id == 100003) //臾쇰갑 n% 利앷?
            {
                get_state.physical_defense += (get_state.physical_defense * passive_db_iter->effect_value) / 100;
            }
            else if (passive_db_iter->passive_id == 100004) //留덈갑 n% 利앷?
            {
                get_state.magic_defense += (get_state.magic_defense * passive_db_iter->effect_value) / 100;
            }
            else if (passive_db_iter->passive_id == 100005) //?앸챸??n% 利앷?
            {
                get_state.now_hp += (get_state.now_hp * passive_db_iter->effect_value) / 100;
            }
        }
    }

    std::string state;
    state += to_string(get_state.position) + ":";
    state += to_string(get_state.index) + ":";
    state += to_string(get_state.id) + ":";
    state += to_string(get_state.now_hp) + ":";
    state += to_string(get_state.physical_attack) + ":";
    state += to_string(get_state.magic_attack) + ":";
    state += to_string(get_state.physical_defense) + ":";
    state += to_string(get_state.magic_defense) + ":";
    state += to_string(get_state.physical_crit_per) + ":";
    state += to_string(get_state.magic_crit_per) + ":";
    state += to_string(get_state.physical_crit_dmg) + ":";
    state += to_string(get_state.magic_crit_dmg) + ":";
    state += to_string(get_state.avoid) + ":";
    state += to_string(get_state.state) + ":";
    state += to_string(get_state.speed) + ":";
    state += to_string(get_state.type) + ":";
    state += to_string(get_state.job_class) + ":";
    state += "[";
    state += "]";
    state += ":";
    if(get_state.passive_skill_list.size() == 0)
    {
        state += "[";
        state += "]";
        state += ":";
    }
    else
    {
        state += "[";
        for (uint32_t i = 0; i < get_state.passive_skill_list.size(); ++i)
        {
            state += to_string(get_state.passive_skill_list[i].id) + ":";
            state += to_string(get_state.passive_skill_list[i].per) + ":";
            state += to_string(get_state.passive_skill_list[i].attack_type) + ":";
            state += to_string(get_state.passive_skill_list[i].dmg_type) + ":";
            state += to_string(get_state.passive_skill_list[i].target) + ":";
            if (get_state.passive_skill_list.size() - 1 == i)
            {
                state += to_string(get_state.passive_skill_list[i].target_count);
            }
            else
            {
                state += to_string(get_state.passive_skill_list[i].target_count) + ":";
            }
        }
        state += "]";
        state += ":";
    }

    if(get_state.active_skill_list.size() == 0)
    {
        state += "[";
        state += "]";
        state += ":";
    }
    else
    {
        state += "[";
        for (uint32_t i = 0; i < get_state.active_skill_list.size(); ++i)
        {
            state += to_string(get_state.active_skill_list[i].id) + ":";
            state += to_string(get_state.active_skill_list[i].per) + ":";
            state += to_string(get_state.active_skill_list[i].attack_type) + ":";
            state += to_string(get_state.active_skill_list[i].dmg_type) + ":";
            state += to_string(get_state.active_skill_list[i].target) + ":";
            if(get_state.active_skill_list.size() - 1 == i)
            {
                state += to_string(get_state.active_skill_list[i].target_count);
            }
            else
            {
                state += to_string(get_state.active_skill_list[i].target_count) + ":";
            }
            
        }
        state += "]";
        state += ":";
    }
    state += to_string(get_state.status.total_str) + ":";
    state += to_string(get_state.status.total_dex) + ":";
    state += to_string(get_state.status.total_int);

    _state.push_back(state);
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

ACTION battletest::stagestart(eosio::name _user, uint32_t _party_number, uint32_t _stage)
{
    require_auth(_user);

    whitelist whitelist_table(_self, _self.value);
    auto whitelist_iter = whitelist_table.find(_user.value);
    eosio_assert(whitelist_iter != whitelist_table.end(), "Not White User 3");

    std::vector<uint32_t> servant_pos_list = {3, 1, 0, 2, 4};
    std::vector<uint32_t> monster_pos_list = {8, 6, 5, 7, 9};

    //?섏쨷???ㅽ뀒?댁? 踰덊샇濡?李얘쾶 ?댁쨾?쇳븿
    stage_db stage_db_table(_self, _self.value);
    auto stage_db_iter = stage_db_table.begin();
    eosio_assert(stage_db_iter != stage_db_table.end(), "Not Exist Stage ID 1");

    auth_users user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Not Exist User 3");
    eosio_assert(user_auth_iter->state == user_state::lobby, "Already Battle");
    user_auth_table.modify(user_auth_iter, _self, [&](auto &user_state_change) {
        user_state_change.state = user_state::stage;
    });

    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.find(_party_number);
    eosio_assert(user_party_iter != user_party_table.end(),"Not Exist Party");
    eosio_assert(user_party_iter->state == party_state::on_wait, "Party Impossible Battle");

    std::vector<std::string> my_state;
    std::vector<std::string> enemy_state;

    battle_state_list user_battle_table(_self, _self.value);
    auto user_battle_iter = user_battle_table.find(_user.value);
    if (user_battle_iter == user_battle_table.end())
    {
        user_battle_table.emplace(_self, [&](auto &new_battle_set) {
            new_battle_set.user = _user;
            new_battle_set.stage_type = stage_db_iter->stage_type;
            new_battle_set.enemy_user = _user;
            new_battle_set.stage_number = stage_db_iter->stage_id;
            new_battle_set.state = stage_state::start;

            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter->servant_list[i] == 0) //?뚰떚 硫ㅻ쾭媛 鍮꾩뼱?덉쑝硫?                {
                    continue;
                }
                battle_state servant_battle_state = get_user_state(_user, "ser",user_party_iter->servant_list[i],servant_pos_list[i], my_state);
                new_battle_set.my_state_list.push_back(servant_battle_state);
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter->monster_list[i] == 0) //?뚰떚 硫ㅻ쾭媛 鍮꾩뼱?덉쑝硫?                {
                    continue;
                }
                battle_state monster_battle_state = get_user_state(_user, "mon",user_party_iter->monster_list[i], monster_pos_list[i], my_state);
                new_battle_set.my_state_list.push_back(monster_battle_state);
            }
            set_stage_state(stage_db_iter->stage_id, new_battle_set.enemy_state_list, enemy_state);
        });
    }
    else
    {
        user_battle_table.modify(user_battle_iter, _self, [&](auto &new_battle_set) {
            new_battle_set.stage_type = stage_db_iter->stage_type;
            new_battle_set.enemy_user = _user;
            new_battle_set.stage_number = stage_db_iter->stage_id;
            new_battle_set.state = stage_state::start;
            new_battle_set.my_state_list.clear();
            new_battle_set.enemy_state_list.clear();

            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter->servant_list[i] == 0) //?뚰떚 硫ㅻ쾭媛 鍮꾩뼱?덉쑝硫?                {
                    continue;
                }
                battle_state servant_battle_state = get_user_state(_user, "ser",user_party_iter->servant_list[i],servant_pos_list[i], my_state);
                new_battle_set.my_state_list.push_back(servant_battle_state);
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter->monster_list[i] == 0) //?뚰떚 硫ㅻ쾭媛 鍮꾩뼱?덉쑝硫?                {
                    continue;
                }
                battle_state monster_battle_state = get_user_state(_user, "mon",user_party_iter->monster_list[i], monster_pos_list[i], my_state);
                new_battle_set.my_state_list.push_back(monster_battle_state);
            }
            set_stage_state(stage_db_iter->stage_id, new_battle_set.enemy_state_list, enemy_state);
        });
    }

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

    action(permission_level{get_self(), "active"_n},
           get_self(), "battlestate"_n,
           std::make_tuple(_user, my_state, enemy_state))
        .send();
}

#pragma endregion

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#pragma region active turn
int battletest::get_random_target(const std::vector<battle_state> &_enemy_state_list, uint64_t _seed, uint32_t _max, uint32_t _min)
{
    int target_key = safeseed::get_random_value(_seed, _max, _min, 0);
    if ((_enemy_state_list[target_key].state == battle_member_state::dead))
    {
        target_key = -1;
        for (uint32_t i = _min; i < _max; i++)
        {
            if ( (_enemy_state_list[i].state != battle_member_state::dead) )
            {
                target_key = i;
                return target_key;
            }
        }
        return target_key;
    }
    else
    {
        return target_key;
    }
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

bool battletest::check_activate_skill(skill_info _skill, uint64_t _rate)
{
    if(_skill.per > _rate)
    {
        return true;
    }
    else
    {
        return false;
    }
}

bool battletest::check_buff_state(buff_info &_buff)
{
    if(_buff.turn == 0)
    {
        return false;
    }
    else
    {
        return true;
    }
}

void battletest::init_buff_effect(battle_state &_state, buff_info _buff)
{
    if(_buff.id == buff_state::defense)
    {
        
    }
}

void battletest::init_buff_turn(std::vector<battle_state> &_state_list)
{
    for (uint32_t i = 0; i < _state_list.size(); ++i)
    {
        if(_state_list[i].buff_list.size() != 0)
        {
            for (uint32_t buff = 0; buff < _state_list[i].buff_list.size(); ++buff)
            {
                if(_state_list[i].buff_list[buff].turn - 1 <= 0)
                {
                    _state_list[i].buff_list.erase(_state_list[i].buff_list.begin() + buff);
                }
                else
                {
                    _state_list[i].buff_list[buff].turn -= 1;
                }
            }
        }
    }
}




bool battletest::set_action(uint32_t _action,
                            uint64_t _seed,
                            std::vector<battle_state> &_my_state_list,
                            std::vector<battle_state> &_enemy_state_list,
                            uint64_t _my_key, battle_action_info &_action_info,
                            std::vector<std::string> &_data)
{
    std::string action_data;
    _action_info.my_position = _my_state_list[_my_key].position;
    _action_info.action_type = _action;
    if (_action == action_type::attack)
    {
        int enemy_key = get_random_target(_my_state_list, _seed, _enemy_state_list.size(), 0);
        if (enemy_key == -1) //?곷? ?뚰떚媛 紐⑤몢 二쎌? ?곹깭
        {
            return false;
        }

        battle_action new_action;
        new_action = get_target_action(_action, _my_state_list, _enemy_state_list, _seed, _my_key, enemy_key);
        if (_enemy_state_list[enemy_key].now_hp <= new_action.damage)
        {
            _enemy_state_list[enemy_key].now_hp = 0;
            _enemy_state_list[enemy_key].state = battle_member_state::dead;
        }
        else
        {
            _enemy_state_list[enemy_key].now_hp -= new_action.damage;
        }
        _action_info.battle_action_list.push_back(new_action);
    }
    else if (_action == action_type::skill)
    {
        if (_my_state_list[_my_key].active_skill_list[0].id == active_name::active_multi_shot) //2??怨듦꺽
        {
            for (uint32_t i = 0; i < _my_state_list[_my_key].active_skill_list[0].target_count; ++i)
            {
                uint64_t new_seed = safeseed::get_seed_value(now(),_seed);
                int enemy_key = get_random_target(_my_state_list, new_seed, _enemy_state_list.size(), 0);
                if (enemy_key == -1) //?곷? ?뚰떚媛 紐⑤몢 二쎌? ?곹깭
                {
                    return false;
                }
                new_seed = new_seed >> 1;

                battle_action new_action;
                new_action = get_target_action(_action,
                                               _my_state_list, _enemy_state_list, _seed, _my_key, enemy_key);
                if (_enemy_state_list[enemy_key].now_hp <= new_action.damage)
                {
                    _enemy_state_list[enemy_key].now_hp = 0;
                    _enemy_state_list[enemy_key].state = battle_member_state::dead;
                }
                else
                {
                    _enemy_state_list[enemy_key].now_hp -= new_action.damage;
                }
                _action_info.battle_action_list.push_back(new_action);
            }
        }
        else if(_my_state_list[_my_key].active_skill_list[0].id == active_name::active_heal)        //?먯뒪??        {
            int enemy_key = get_random_target(_my_state_list, _seed, _my_state_list.size(), 0);
            if (enemy_key == -1) //?곷? ?뚰떚媛 紐⑤몢 二쎌? ?곹깭
            {
                return false;
            }
            battle_action new_action;
            new_action = get_target_action(_action,
                                           _my_state_list, _my_state_list, _seed, _my_key, enemy_key);
            uint32_t max_hp = get_max_hp(_my_state_list[enemy_key].status);
            if (max_hp > _my_state_list[enemy_key].now_hp + new_action.damage)
            {
                _my_state_list[enemy_key].now_hp += new_action.damage;
            }
            _action_info.battle_action_list.push_back(new_action);
        }
        else   //1???ㅽ궗
        {
            int enemy_key = get_random_target(_my_state_list, _seed, _enemy_state_list.size(), 0);
            if (enemy_key == -1) //?곷? ?뚰떚媛 紐⑤몢 二쎌? ?곹깭
            {
                return false;
            }
            battle_action new_action;
            new_action = get_target_action(_action,
                                           _my_state_list, _enemy_state_list, _seed, _my_key, enemy_key);
            if (_enemy_state_list[enemy_key].now_hp <= new_action.damage)
            {
                _enemy_state_list[enemy_key].now_hp = 0;
                _enemy_state_list[enemy_key].state = battle_member_state::dead;
            }
            else
            {
                _enemy_state_list[enemy_key].now_hp -= new_action.damage;
            }
            _action_info.battle_action_list.push_back(new_action);
        }
    }

    action_data +=  to_string(_action_info.my_position) + ":";
    action_data +=  to_string(_action_info.action_type) + ":";
    action_data +=  "[";
    for(uint32_t i=0;i<_action_info.battle_action_list.size(); ++i)
    {
        action_data +=  to_string(_action_info.battle_action_list[i].target_position) + ":";
        action_data +=  to_string(_action_info.battle_action_list[i].avoid) + ":";
        action_data +=  to_string(_action_info.battle_action_list[i].critical) + ":";
        if(i + 1 == _action_info.battle_action_list.size()){
        action_data +=  to_string(_action_info.battle_action_list[i].damage);
        }
        else
        {
            action_data +=  to_string(_action_info.battle_action_list[i].damage) + ":";
        }
    }
    action_data += "]";

    _data.push_back(action_data);
    return true;
}

void battletest::set_skill_damage(uint32_t _skill_id, battle_state &_state ,uint32_t &_attack, uint32_t &_cri_dmg)
{
    switch (_skill_id)
    {
    case active_name::active_defense:       //踰꾪봽 ?ㅽ궗
    {
        break;
    }
    case active_name::active_fast_attack:  //鍮⑤━ ?뚮━湲?    {
        break;
    }
    case active_name::active_bash:
    {
        _attack = _attack * 210 / 100;
        _cri_dmg = _cri_dmg * 210 / 100;
        break;
    }
    case active_name::active_critical_strike:   //?뺤젙 ?щ━
    {
        break;
    }
    case active_name::active_heal:
    {
        _attack = _attack * 150 / 100;
        _cri_dmg = _cri_dmg * 150 / 100;
        break;
    }
    case active_name::active_magic_strike:
    {
        _attack = _attack * 180 / 100;
        _cri_dmg = _cri_dmg * 180 / 100;
        break;
    }
    case active_name::active_multi_shot:
    {
        _attack = _attack * 125 / 100;
        _cri_dmg = _cri_dmg * 125 / 100;
        break;
    }
    case active_name::active_guided_arrow:
    {
        _attack = _attack * 150 / 100;
        _cri_dmg = _cri_dmg * 150 / 100;
        break;
    }
    default:
    {
        eosio_assert(1 == 0, "Wrong Skill ID 1");
        break;
    }
    }
}

void battletest::set_skill_type(skill_info _skill, battle_state &_state ,uint32_t &_attack, uint32_t &_cri_dmg, uint32_t &_cri_per, uint32_t &_defense)
{
    switch (_skill.id)
    {
    case active_name::active_defense:
    case active_name::active_fast_attack:
    case active_name::active_bash:
    case active_name::active_critical_strike:
    case active_name::active_heal:
    case active_name::active_magic_strike:
    case active_name::active_multi_shot:
    case active_name::active_guided_arrow:
    {
        set_attack_type(_skill.attack_type, _state, _attack, _cri_dmg, _cri_per);
        set_dmg_type(_skill.dmg_type, _state,_defense);
        break;
    }
    default:
    {
        eosio_assert(1 == 0 , "Wrong Skill ID 1");
        break;
    }
    }
}

void battletest::set_dmg_type(uint32_t _dmg_type, battle_state &_state , uint32_t &_defense)
{
    switch (_dmg_type)
    {
    case dmg_type::buff_heal_none:
    {   
        break;
    }
    case dmg_type::physical_dfs:
    {
        _defense = _state.physical_defense;
        break;
    }
    case dmg_type::magic_dfs:
    {
        _defense = _state.magic_defense;
        break;
    }
    default:
    {
        eosio_assert(1 == 0, "Wrong Dmg Type 1");
        break;
    }
    }
}


void battletest::set_attack_type(uint32_t _atk_type, battle_state &_state ,uint32_t &_attack, uint32_t &_cri_dmg, uint32_t &_cri_per)
{
    switch (_atk_type)
    {
    case atk_type::buff_none:
    {
        break;
    }
    case atk_type::physical_atk:
    {
        _attack = _state.physical_attack;
        _cri_dmg = _state.physical_crit_dmg;
        _cri_per = _state.physical_crit_per;
        break;
    }
    case atk_type::magic_atk:
    {
        _attack = _state.magic_attack;
        _cri_dmg = _state.magic_crit_dmg;
        _cri_per = _state.magic_crit_per;
        break;
    }
    default:
    {
        eosio_assert(1 == 0, "Wrong Atk Type 1");
        break;
    }
    }
}


battletest::battle_action battletest::get_target_action(uint32_t _active_id ,std::vector<battle_state> &_my_state_list, std::vector<battle_state> &_enemy_state_list, uint64_t _seed, uint64_t _my_key, uint64_t _target_key)
{
    uint32_t cur_target_key = _target_key;
    uint32_t cur_damage = 0;
    uint32_t cur_attack = 0;
    uint32_t cur_cirtical = 0;
    uint32_t cur_cri_per = 0;
    uint32_t cur_defense = 0;
    uint32_t max_hp = 0;
    uint32_t cur_hp = 0;

    switch (_active_id)
    {
    case action_type::attack:
    {
        cur_attack = _my_state_list[_my_key].physical_attack;
        cur_cirtical = _my_state_list[_my_key].physical_crit_dmg;
        cur_cri_per = _my_state_list[_my_key].physical_crit_per;
        cur_defense = _my_state_list[_my_key].physical_defense;
        break;
    }
    case action_type::skill:
    {
        set_skill_type(_my_state_list[_my_key].active_skill_list[0], _my_state_list[_my_key],
                       cur_attack, cur_cirtical, cur_cri_per, cur_defense);
        set_skill_damage(_my_state_list[_my_key].active_skill_list[0].id, _my_state_list[_my_key],
                         cur_attack, cur_cirtical);

        switch (_my_state_list[_my_key].active_skill_list[0].id)
        {
        case active_name::active_bash:
        case active_name::active_fast_attack:
        case active_name::active_multi_shot:
        case active_name::active_guided_arrow:
        {
            battle_action new_action;
            if (true == check_avoid(_enemy_state_list[cur_target_key].avoid, _seed))
            {
                new_action.target_position = _enemy_state_list[cur_target_key].position;
                new_action.avoid = 1;
                new_action.critical = 0;
                new_action.damage = 0;
                return new_action;
            }
            break;
        }
        case active_name::active_critical_strike:
        {
            battle_action new_action;
            if (true == check_avoid(_enemy_state_list[cur_target_key].avoid, _seed))
            {
                new_action.target_position = _enemy_state_list[cur_target_key].position;
                new_action.avoid = 1;
                new_action.critical = 0;
                new_action.damage = 0;
                return new_action;
            }
            if (_enemy_state_list[cur_target_key].buff_list.size() != 0)
            {
                for (uint32_t i = 0; i < _enemy_state_list[cur_target_key].buff_list.size(); ++i)
                {
                    if (true == check_buff_state(_enemy_state_list[cur_target_key].buff_list[i]))
                    {
                        if (_enemy_state_list[cur_target_key].buff_list[i].id == buff_state::defense)
                        {
                            cur_damage = get_damage(cur_cirtical, _enemy_state_list[cur_target_key].physical_defense + (_enemy_state_list[cur_target_key].physical_defense / 2));
                        }
                    }
                }
            }
            else
            {
                cur_damage = get_damage(cur_cirtical, _enemy_state_list[cur_target_key].physical_defense);
            }
            new_action.target_position = _enemy_state_list[cur_target_key].position;
            new_action.avoid = 0;
            new_action.critical = 1;
            new_action.damage = cur_damage;
            return new_action;
        }
        case active_name::active_heal:
        {
            battle_action new_action;
            new_action.target_position = _enemy_state_list[cur_target_key].position;
            new_action.avoid = 0;
            new_action.critical = 0;
            new_action.damage = (cur_attack * 150) / 100;
            return new_action;
        }
        case active_name::active_magic_strike:
        {
            break;
        }
        default:
        {
            eosio_assert(1 == 0, "Wrong Skill ID 2");
            break;
        }
        }

        break;
    }
    default:

    {
        eosio_assert(1 == 0, "Wrong Action Type 1");
        break;
    }
    }

    battle_action new_action;
    if (false == check_critical(cur_cri_per, _seed))
    {
        if (_enemy_state_list[cur_target_key].buff_list.size() != 0) //踰꾪봽媛 ?덈뒗吏 ?뺤씤
        {
            for (uint32_t i = 0; i < _enemy_state_list[cur_target_key].buff_list.size(); ++i)
            {
                if (true == check_buff_state(_enemy_state_list[cur_target_key].buff_list[i]))
                {
                    if (_enemy_state_list[cur_target_key].buff_list[i].id == buff_state::defense)
                    {
                        cur_damage = get_damage(cur_attack, cur_defense + (cur_defense * 30 / 100));
                    }
                }
            }
        }
        else
        {
            cur_damage = get_damage(cur_attack, cur_defense);
        }
        new_action.target_position = _enemy_state_list[cur_target_key].position;
        new_action.avoid = 0;
        new_action.critical = 0;
        new_action.damage = cur_damage;
    }
    else
    {
        if (_enemy_state_list[cur_target_key].buff_list.size() != 0)
        {
            for (uint32_t i = 0; i < _enemy_state_list[cur_target_key].buff_list.size(); ++i)
            {
                if (true == check_buff_state(_enemy_state_list[cur_target_key].buff_list[i]))
                {
                    if (_enemy_state_list[cur_target_key].buff_list[i].id == buff_state::defense)
                    {
                        cur_damage = get_damage(cur_cirtical, cur_defense + (cur_defense * 30 / 100));
                    }
                }
            }
        }
        else
        {
            cur_damage = get_damage(cur_cirtical, cur_defense);
        }
        new_action.target_position = _enemy_state_list[cur_target_key].position;
        new_action.avoid = 0;
        new_action.critical = 1;
        new_action.damage = cur_damage;
    }
    return new_action;
}


ACTION battletest::activeturn(eosio::name _user, uint32_t _turn, std::string _seed)
{
    require_auth(_user);
    whitelist whitelist_table(_self, _self.value);
    auto whitelist_iter = whitelist_table.find(_user.value);
    eosio_assert(whitelist_iter != whitelist_table.end(), "Not White User 4");
    //?⑥닔濡?類닿린
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
    eosio_assert(user_battle_state_iter != battle_state_list_table.end(), "Need Start Battle 1");
    eosio_assert(user_battle_state_iter->state == stage_state::start, "End Battle 1");

    battle_actions battle_action_table(_self, _self.value);
    auto user_battle_action_iter = battle_action_table.find(_user.value);
    eosio_assert(user_battle_action_iter != battle_action_table.end(), "Need Start Battle 2");
    eosio_assert(user_battle_action_iter->turn == _turn - 1, "Wrong Trun 1");

    std::vector<uint64_t> order_random_list;
    safeseed::get_battle_rand_list(order_random_list, battle_seed);

    std::vector<std::string> data;
    //諛고????곹깭瑜?諛붽퓭二쇰뒗 遺遺?    battle_state_list_table.modify(user_battle_state_iter, _self, [&](auto &battle_state) {
        //踰꾪봽 ?대???吏꾪뻾
        init_buff_turn(battle_state.my_state_list);
        init_buff_turn(battle_state.enemy_state_list);
        //怨듦꺽?쒖꽌 ?뺣젹?섎뒗 遺遺?        std::vector<battle_order_struct> second_attack_order_list;
        std::vector<battle_order_struct> attack_order_list;
        std::vector<battle_order_struct> skill_order_list;
        //?곕뱶由ъ뒪??異붽?
        for (uint32_t i = 0; i < user_battle_state_iter->my_state_list.size(); ++i)
        {
            if ((user_battle_state_iter->my_state_list[i].state == battle_member_state::dead))
            {
                continue;
            }
            battle_order_struct new_order;
            new_order.speed = user_battle_state_iter->my_state_list[i].speed;
            new_order.position = user_battle_state_iter->my_state_list[i].position;
            new_order.key = i;
            new_order.second_speed = order_random_list[i];
            skill_order_list.push_back(new_order);
        }
        for (uint32_t i = 0; i < user_battle_state_iter->enemy_state_list.size(); ++i)
        {
            if ((user_battle_state_iter->enemy_state_list[i].state == battle_member_state::dead))
            {
                continue;
            }
            battle_order_struct new_order;
            new_order.speed = user_battle_state_iter->enemy_state_list[i].speed;
            new_order.position = user_battle_state_iter->enemy_state_list[i].position;
            new_order.key = i;
            new_order.second_speed = order_random_list[i + 10];
            skill_order_list.push_back(new_order);
        }
        std::sort(skill_order_list.begin(), skill_order_list.end(), sort_compare);
        //---------------------

        //諛고????≪뀡?뚯씠釉붿뿉 ?곗씠?곕? 異붽??댁＜??遺遺?        battle_action_table.modify(user_battle_action_iter, _self, [&](auto &update_action) {
            update_action.turn += 1;
            update_action.battle_info_list.clear();
            //癒쇱? ?ㅽ궗 諛쒕룞 ?щ? ?뺤씤
            for(uint32_t i = 0; i<skill_order_list.size(); ++i)
            {
                uint32_t my_key = skill_order_list[i].key;
                if (skill_order_list[i].position < max_party_count) //???뚰떚?몃뜲
                {
                    uint64_t action_rate = safeseed::get_random_value(skill_order_list[i].second_speed, 100, 0, 0);
                    if (true == check_activate_skill(battle_state.my_state_list[my_key].active_skill_list[0], action_rate)) //?≫떚釉??ㅽ궗??諛쒕룞?섎㈃
                    {
                        if (battle_state.my_state_list[my_key].active_skill_list[0].id == active_name::active_fast_attack) //?⑥뒪???댄깮
                        {
                            skill_order_list[i].action = action_type::skill;
                            attack_order_list.push_back(skill_order_list[i]);
                        }
                        else if (battle_state.my_state_list[my_key].active_skill_list[0].id == active_name::active_defense)    //踰꾪봽 ?ㅽ궗
                        {
                            buff_info new_buff;
                            new_buff.id = buff_state::defense;
                            new_buff.turn = 1;
                            battle_state.my_state_list[my_key].buff_list.push_back(new_buff);

                            battle_action_info action_info;
                            action_info.my_position = battle_state.my_state_list[my_key].position;
                            action_info.action_type = action_type::skill;
                            update_action.battle_info_list.push_back(action_info);

                            std::string action_data;
                            action_data += to_string(action_info.my_position) + ":";
                            action_data += to_string(action_info.action_type) + ":";
                            action_data += "[";
                            action_data += "]";
                            data.push_back(action_data);
                        }
                        else if (battle_state.my_state_list[my_key].active_skill_list[0].id == active_name::active_heal)    //???ㅽ궗
                        {
                            battle_action_info action_info;
                            if(false == set_action(action_type::skill,
                                       skill_order_list[i].second_speed,
                                       battle_state.my_state_list,
                                       battle_state.enemy_state_list,
                                       my_key, action_info,data))
                                       {
                                           break;
                                       }
                            update_action.battle_info_list.push_back(action_info);
                        }
                        else
                        {
                            skill_order_list[i].action = action_type::skill;
                            second_attack_order_list.push_back(skill_order_list[i]);
                        }
                    }
                    else    //?ㅽ궗??諛쒕룞?섏? ?딆쑝硫?                    {
                        skill_order_list[i].action = action_type::attack;
                        second_attack_order_list.push_back(skill_order_list[i]);
                    }
                }   
                else    //?곸쓽 寃쎌슦
                {
                    skill_order_list[i].action = action_type::attack;
                    second_attack_order_list.push_back(skill_order_list[i]);
                }
            }
            //?⑥뒪???댄깮 ?좊뱾 泥섎━
            for (uint32_t i = 0; i < attack_order_list.size(); ++i)
            {
                uint32_t my_key = attack_order_list[i].key;
                if (attack_order_list[i].position < max_party_count) //?먭린 ?뚰떚?????泥섎━
                {
                    if (battle_state.my_state_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    battle_action_info action_info;
                    if (false == set_action(action_type::skill,
                                            attack_order_list[i].second_speed,
                                            battle_state.my_state_list,
                                            battle_state.enemy_state_list,
                                            my_key, action_info,data))
                    {
                        break;
                    }
                    update_action.battle_info_list.push_back(action_info);
                }
                else // ?곷? ?뚰떚??寃쎌슦
                {
                    if (battle_state.enemy_state_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    battle_action_info action_info;
                    if (false == set_action(action_type::skill,
                                            attack_order_list[i].second_speed,
                                            battle_state.enemy_state_list,
                                            battle_state.my_state_list,
                                            my_key, action_info,data))
                    {
                        break;
                    }
                    update_action.battle_info_list.push_back(action_info);
                }
            }
            //洹몃깷 ?쇰컲 怨듦꺽?섎뒗 ?좊뱾?????泥섎━ ===============================================================
            for (uint32_t i = 0; i < second_attack_order_list.size(); ++i)
            {
                uint32_t my_key = second_attack_order_list[i].key;
                if (second_attack_order_list[i].position < max_party_count) //?먭린 ?뚰떚?????泥섎━
                {
                    if (battle_state.my_state_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    battle_action_info action_info;
                    if (false == set_action(second_attack_order_list[i].action,
                                            second_attack_order_list[i].second_speed,
                                            battle_state.my_state_list,
                                            battle_state.enemy_state_list,
                                            my_key, action_info,data))
                    {
                        break;
                    }
                    update_action.battle_info_list.push_back(action_info);
                }
                else // ?곷? ?뚰떚??寃쎌슦
                {
                    if (battle_state.enemy_state_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    battle_action_info action_info;
                    if (false == set_action(second_attack_order_list[i].action,
                                            second_attack_order_list[i].second_speed,
                                            battle_state.enemy_state_list,
                                            battle_state.my_state_list,
                                            my_key, action_info,data))
                    {
                        break;
                    }
                    update_action.battle_info_list.push_back(action_info);
                }
            }

            //寃뚯엫??醫낅즺 ?щ? 泥댄겕
            for (uint32_t i = 0; i < battle_state.my_state_list.size(); ++i)
            {
                if (battle_state.my_state_list[i].state == battle_member_state::dead)
                {
                    user_dead_count += 1;
                }
            }

            for (uint32_t i = 0; i < battle_state.enemy_state_list.size(); ++i)
            {
                if (battle_state.enemy_state_list[i].state == battle_member_state::dead)
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

    action(permission_level{get_self(), "active"_n},
           get_self(), "battleaction"_n,
           std::make_tuple(_user, std::string("action"), data))
        .send();
}

bool battletest::check_level_up(uint64_t _cur_exp, uint64_t _pre_exp)
{
    lv_exp lv_exp_table(_self, _self.value);
    auto lv_exp_iter = lv_exp_table.find(50);
    if(lv_exp_iter->exp >= _cur_exp)    //?대? 留뚮젟??寃쎌슦
    {
        return false;
    }

    uint64_t level_up_line = 0;
    for(auto iter = lv_exp_table.begin(); iter != lv_exp_table.end();)
    {
        if(iter->exp > _cur_exp)
        {
            level_up_line = iter->exp;
            break;
        }
        iter++;
    }

    if(level_up_line == 0)
    {
        return false;
    }
    else if (level_up_line <= _pre_exp)
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
    battle_state_list battle_state_list_table(_self, _self.value);
    auto user_battle_state_iter = battle_state_list_table.find(_user.value);
    eosio_assert(user_battle_state_iter != battle_state_list_table.end(), "End Battle 1");
    eosio_assert(user_battle_state_iter->state == stage_state::start, "Already Get Reward 1");
    battle_state_list_table.modify(user_battle_state_iter, _self, [&](auto &end_battle) {
        end_battle.state = stage_state::win;
    });

    stage_db stage_db_table(_self, _self.value);
    auto stage_db_iter = stage_db_table.find(user_battle_state_iter->stage_number);
    eosio_assert(stage_db_iter != stage_db_table.end(),"Not Exist Stage");

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Not Exist Log 1");
    user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
        update_log.last_stage_num = stage_db_iter->stage_id;
        update_log.battle_count += 1;
    });

    battle_reward_list battle_reward_list_table(_self, _self.value);
    auto user_battle_reward_iter = battle_reward_list_table.find(_user.value);
    if (user_battle_reward_iter == battle_reward_list_table.end())
    {
        battle_reward_list_table.emplace(_self, [&](auto &set_reward) {
            set_reward.user = _user;
            for (uint32_t i = 0; i < 10; ++i)
            {
                set_reward.get_exp_list.push_back(stage_db_iter->char_exp);
            }
        });
    }
    else
    {
        battle_reward_list_table.modify(user_battle_reward_iter, _self, [&](auto &set_reward) {
            set_reward.get_exp_list.clear();
            for (uint32_t i = 0; i < 10; ++i)
            {
                set_reward.get_exp_list.push_back(stage_db_iter->char_exp);
            }
        });
    }

    //蹂댁긽 諛섏쁺
    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.begin();
    eosio_assert(user_party_iter != user_party_table.end(), "Not Exist Party");

    auth_users user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Not Exist Hero");
    uint64_t cur_exp = user_auth_iter->hero.exp + stage_db_iter->char_exp;
    uint64_t pre_exp = user_auth_iter->hero.exp;
    user_auth_table.modify(user_auth_iter, _self, [&](auto &upadate_hero_exp) {
        if (true == check_level_up(cur_exp, pre_exp))
        {
            upadate_hero_exp.hero.status = get_level_up_servant_status(user_auth_iter->hero.job, user_auth_iter->hero.status);
            upadate_hero_exp.hero.stat_point += 1;
            upadate_hero_exp.hero.exp += stage_db_iter->char_exp;
        }
        else{
            upadate_hero_exp.hero.exp += stage_db_iter->char_exp;
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
        uint64_t cur_exp = user_servant_iter->servant.exp + stage_db_iter->char_exp;
        uint64_t pre_exp = user_servant_iter->servant.exp;
        user_servant_table.modify(user_servant_iter, _self, [&](auto &update_servant_exp) {
            if (true == check_level_up(cur_exp, pre_exp))
            {
                update_servant_exp.servant.status = get_level_up_servant_status(user_servant_iter->servant.job, user_servant_iter->servant.status);
                update_servant_exp.servant.stat_point += 1;
                update_servant_exp.servant.exp += stage_db_iter->char_exp;
            }
            else
            {
                update_servant_exp.servant.exp += stage_db_iter->char_exp;
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
        uint64_t cur_exp = user_monster_iter->monster.exp + stage_db_iter->char_exp;
        uint64_t pre_exp = user_monster_iter->monster.exp;
        user_monster_table.modify(user_monster_iter, _self, [&](auto &update_monster_exp) {
            if (true == check_level_up(cur_exp, pre_exp))
            {
                update_monster_exp.monster.status = get_level_up_monster_status(user_monster_iter->monster.id, user_monster_iter->monster.grade, user_monster_iter->monster.status);
                update_monster_exp.monster.exp += stage_db_iter->char_exp;
            }
            else
            {
                update_monster_exp.monster.exp += stage_db_iter->char_exp;
            }
        });
    }

    // asset battle_result(0, symbol(symbol_code("UTG"), 4));
    // battle_result.amount = 10000000;

    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "transfer"_n,
    //        std::make_tuple(_self, _user, battle_result, std::string("battle reward")))
    //     .send();
}

void battletest::fail_reward(eosio::name _user)
{
    battle_state_list battle_state_list_table(_self, _self.value);
    auto user_battle_state_iter = battle_state_list_table.find(_user.value);
    eosio_assert(user_battle_state_iter != battle_state_list_table.end(), "End Battle 1");
    eosio_assert(user_battle_state_iter->state == stage_state::start, "Already Get Reward 2");
    battle_state_list_table.modify(user_battle_state_iter, _self, [&](auto &end_battle) {
        end_battle.state = stage_state::lose;
    });

    stage_db stage_db_table(_self, _self.value);
    auto stage_db_iter = stage_db_table.find(user_battle_state_iter->stage_number);
    eosio_assert(stage_db_iter != stage_db_table.end(), "Not Exist Stage");

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
        update_log.last_stage_num = stage_db_iter->stage_id;
        update_log.battle_count += 1;
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

ACTION battletest::stageexit(eosio::name _user)
{
    require_auth(_user);

    auth_users user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Not Exist User 2");
    eosio_assert(user_auth_iter->state == user_state::stage, "Already End Battle");
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
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "Server Pause");

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
    user_consum_item user_consumables_table(_self, _user.value);
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
    user_equip_items user_item_table(_self, _user.value);
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
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "Server Pause");

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
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "Server Pause");

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
    user_equip_items user_item_table(_self, _user.value);
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
    user_equip_items user_item_table(_self, _user.value);
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
        user_equip_items user_item_table2(_self, _user.value);
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
    // user_equip_items user_item_table(_self, _user.value);
    // auto user_item_iter = user_item_table.find(_item_index);
    // eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
    // eosio_assert(user_item_iter->item.state == object_state::on_equip_slot, "already unequip this item");

    // uint32_t slot = user_item_iter->item.type - 1;
    // user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item) {
    //     unequip_item.item.state = object_state::on_inventory;
    // });

    // auth_users user_auth_table(_self, _self.value);
    // auto user_auth_iter = user_auth_table.find(_user.value);
    // eosio_assert(user_auth_iter != user_auth_table.end(), "not exist user info");

    // user_auth_table.modify(user_auth_iter, _self, [&](auto &unequip_hero) {
    //     unequip_hero.hero.status.plus_str -= (user_item_iter->item.status.basic_str + user_item_iter->item.status.plus_str);
    //     unequip_hero.hero.status.plus_dex -= (user_item_iter->item.status.basic_dex + user_item_iter->item.status.plus_dex);
    //     unequip_hero.hero.status.plus_int -= (user_item_iter->item.status.basic_int + user_item_iter->item.status.plus_int);
    //     unequip_hero.hero.equip_slot[slot] = 0;
    // });
}

void battletest::equip_hero(eosio::name _user, uint32_t _item_index)
{
    // user_equip_items user_item_table(_self, _user.value);
    // auto user_item_iter = user_item_table.find(_item_index);
    // auth_users user_auth_table(_self, _self.value);
    // auto user_auth_iter = user_auth_table.find(_user.value);
    // eosio_assert(user_item_iter != user_item_table.end(), "not exist item info");
    // eosio_assert(user_item_iter->item.state != object_state::on_equip_slot, "already equip this item");
    // eosio_assert(user_auth_iter != user_auth_table.end(), "not exist user info");

    // //eosio_assert(compare_item(user_auth_iter->hero.job, user_item_iter->item.job), "this item is not equipment");

    // uint32_t slot;
    // if (user_item_iter->item.type >= 3)
    // {
    //     slot = 2;
    // }
    // else
    // {
    //     slot = user_item_iter->item.type - 1;
    // }

    // status_info item_status;
    // if (user_auth_iter->hero.equip_slot[slot] != 0) //기존에 아이템 착용 상태 시
    // {
    //     user_equip_items user_item_table2(_self, _user.value);
    //     auto user_item_iter2 = user_item_table2.find(user_auth_iter->hero.equip_slot[slot]);
    //     user_item_table2.modify(user_item_iter2, _self, [&](auto &unequip_item) {
    //         unequip_item.item.state = object_state::on_inventory;
    //     });

    //     item_status = user_item_iter2->item.status;

    //     user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item) {
    //         unequip_item.item.state = object_state::on_equip_slot;
    //     });

    //     user_auth_table.modify(user_auth_iter, _self, [&](auto &unequip_hero) {
    //         unequip_hero.hero.status.plus_str += (user_item_iter->item.status.basic_str + user_item_iter->item.status.plus_str);
    //         unequip_hero.hero.status.plus_dex += (user_item_iter->item.status.basic_dex + user_item_iter->item.status.plus_dex);
    //         unequip_hero.hero.status.plus_int += (user_item_iter->item.status.basic_int + user_item_iter->item.status.plus_int);
    //         unequip_hero.hero.equip_slot[slot] = _item_index;
    //         unequip_hero.hero.status.plus_str -= (item_status.basic_str + item_status.plus_str);
    //         unequip_hero.hero.status.plus_dex -= (item_status.basic_dex + item_status.plus_dex);
    //         unequip_hero.hero.status.plus_int -= (item_status.basic_int + item_status.plus_int);
    //     });
    // }
    // else
    // {
    //     user_item_table.modify(user_item_iter, _self, [&](auto &unequip_item) {
    //         unequip_item.item.state = object_state::on_equip_slot;
    //     });

    //     user_auth_table.modify(user_auth_iter, _self, [&](auto &unequip_hero) {
    //         unequip_hero.hero.status.plus_str += (user_item_iter->item.status.basic_str + user_item_iter->item.status.plus_str);
    //         unequip_hero.hero.status.plus_dex += (user_item_iter->item.status.basic_dex + user_item_iter->item.status.plus_dex);
    //         unequip_hero.hero.status.plus_int += (user_item_iter->item.status.basic_int + user_item_iter->item.status.plus_int);
    //         unequip_hero.hero.equip_slot[slot] = _item_index;
    //     });
    // }
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
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "Server Pause");

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
    user_equip_items user_item_table(_self, _user.value);
    auto user_item_iter = user_item_table.find(_item);

    user_consum_item user_consumables_table(_self, _user.value);
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
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "Server Pause");

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
    user_consum_item user_consumables_table(_self, _user.value);
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
    user_consum_item user_consumables_table(_self, _user.value);
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
    require_auth("dlwodnjs1111"_n);
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
        //set_hero.hero.exp = 0;
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
    require_auth("dlwodnjs1111"_n);
    if (_loser == "dlwodnjs1111"_n)
    {
        user_partys user_party_table(_self, _winner.value);
        auto user_party_winner_iter = user_party_table.find(_winner_party_num);
        eosio_assert(user_party_winner_iter != user_party_table.end(), "Wrong Party Number 1");
        user_party_table.modify(user_party_winner_iter, _self, [&](auto &set_party) {
            set_party.state = party_state::on_tower_defense;
        });
    }
    else
    {
        user_partys user_winner_party_table(_self, _winner.value);
        auto user_party_winner_iter = user_winner_party_table.find(_winner_party_num);
        eosio_assert(user_party_winner_iter != user_winner_party_table.end(), "Wrong Party Number 2");
        user_winner_party_table.modify(user_party_winner_iter, _self, [&](auto &set_party) {
            set_party.state = party_state::on_tower_defense;
        });

        user_partys user_loser_party_table(_self, _loser.value);
        auto user_party_loser_iter = user_loser_party_table.find(_loser_party_num);
        eosio_assert(user_party_loser_iter != user_loser_party_table.end(), "Wrong Party Number 3");
        user_loser_party_table.modify(user_party_loser_iter, _self, [&](auto &set_party) {
            set_party.state = party_state::on_wait;
        });
    }
}

#pragma endregion

ACTION battletest::deletebattle(eosio::name _user)
{
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


    user_equip_items user_preregist_item_table(_self, _user.value);
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

    mail_db mail_db_table(_self, _user.value);
    auto mail_db_iter = mail_db_table.begin();
    if(mail_db_iter != mail_db_table.end())
    {
        for(auto iter = mail_db_table.begin(); iter!= mail_db_table.end();)
        {
            auto iter2 = mail_db_table.find(iter->primary_key());
            iter = mail_db_table.erase(iter2);
        }
   //     mail_db_table.erase(mail_db_iter);
    }


        // mail_db mail_db_table(_self, _user.value);
        // for (auto item = mail_db_table.begin(); item != mail_db_table.end();)
        // {
        //     auto iter = mail_db_table.find(item->primary_key());
        //     mail_db_table.erase(iter);
        //     item++;
        // }
    




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
            set_servant_status.servant.status = get_grade_status(0, user_servant_iter->servant.status);
        });
        iter++;
    }

    user_monsters user_monster_table(_self, _user.value);
    for(auto iter = user_monster_table.begin(); iter != user_monster_table.end();)
    {
        auto user_monster_iter = user_monster_table.find(iter->primary_key());
        user_monster_table.modify(user_monster_iter, _self, [&](auto &set_monster_status)
        {
            set_monster_status.monster.status = get_grade_status(user_monster_iter->monster.grade, user_monster_iter->monster.status);
        });
        iter++;
    }


//     // user_equip_items user_item_table(_self, _user.value);
//     // for(auto iter = user_item_table.begin(); iter != user_item_table.end();)
//     // {
//     //     auto user_item_iter = user_item_table.find(iter->primary_key());
//     //     user_item_table.modify(user_item_iter, _self, [&](auto &set_item_status)
//     //     {

//     //     });
//     //     iter++;
//     // }
}
ACTION battletest::change(eosio::name _user, std::string _kind , uint64_t _grade, uint64_t _id, std::string _status)
{
    std::vector<std::string> value_list;
    std::vector<size_t> size_list;
    if(_kind == "servant")
    {
        status_info status;
        substr_value(_status, value_list, size_list, 5);
        uint32_t index = atoi(value_list[0].c_str());
        uint32_t level = atoi(value_list[1].c_str());
        status.basic_str = atoll(value_list[2].c_str());
        status.basic_dex = atoll(value_list[3].c_str());
        status.basic_int = atoll(value_list[4].c_str());

        status = get_grade_status(0, status);

        user_servants user_servant_table(_self, _user.value);
        auto user_servant_iter = user_servant_table.find(index);
        eosio_assert(user_servant_iter != user_servant_table.end(),"Not Exist Servant 4");
        user_servant_table.modify(user_servant_iter, _self, [&](auto &set_servant) {
            uint32_t _job, _body, _gender, _head, _hair;
            uint32_t temp1, temp2, temp3, temp4;
            if (_id != 0)
            {
                servant_db servant_db_table(_self, _self.value);
                auto servant_db_iter = servant_db_table.find(_id);
                eosio_assert(servant_db_iter != servant_db_table.end(),"Not Exist Servant ID 5");
                _job = _id / 1000000;
                temp1 = _id % 1000000;
                _body = temp1 / 100000;
                temp2 = temp1 % 100000;
                _gender = temp2 / 10000;
                temp3 = temp2 % 10000;
                _head = temp3 / 100;
                temp4 = temp3 % 100;
                _hair = temp4 / 1;
                set_servant.servant.appear.head = _head;
                set_servant.servant.appear.hair = _hair;
                set_servant.servant.appear.body = _body;
                set_servant.servant.appear.gender = _gender;
                set_servant.servant.job = _job;
            }
            else
            {
                _job = user_servant_iter->servant.job;
            }

            if(level == 1)
            {
                set_servant.servant.status = status;
            }
            else
            {
                for(uint32_t i=2; i<=level; ++i)
                {
                    status = get_level_up_servant_status(_job, status);
                    set_servant.servant.status = status;
                }
                lv_exp lv_exp_table(_self, _self.value);
                auto lv_iter = lv_exp_table.find(level);
                eosio_assert(lv_iter != lv_exp_table.end(),"Not Exist Level");
                set_servant.servant.exp = lv_iter->exp;
            }
        });
    }
    else if(_kind == "monster")
    {
        status_info status;
        substr_value(_status, value_list, size_list, 5);
        uint32_t index = atoi(value_list[0].c_str());
        uint32_t level = atoi(value_list[1].c_str());
        status.basic_str = atoll(value_list[2].c_str());
        status.basic_dex = atoll(value_list[3].c_str());
        status.basic_int = atoll(value_list[4].c_str());

        status = get_grade_status(_grade, status);

        user_monsters user_monster_table(_self, _user.value);
        auto user_monster_iter = user_monster_table.find(index);
        eosio_assert(user_monster_iter != user_monster_table.end(),"Not Exist Monster 4");
        user_monster_table.modify(user_monster_iter, _self, [&](auto &set_monster) {
            if (_id != 0)
            {
                monster_db monster_db_table(_self, _self.value);
                auto monster_db_iter = monster_db_table.find(_id);
                eosio_assert(monster_db_iter != monster_db_table.end(),"Not Exist Monster ID 5");
                set_monster.monster.id = _id;
            }
            set_monster.monster.grade = _grade;
            if(level == 1)
            {
                set_monster.monster.status = status;
            }
            else
            {
                for(uint32_t i=2; i<=level; ++i)
                {
                    status = get_level_up_monster_status(_id, _grade, status);
                    set_monster.monster.status = status;
                }
                lv_exp lv_exp_table(_self, _self.value);
                auto lv_iter = lv_exp_table.find(level);
                eosio_assert(lv_iter != lv_exp_table.end(),"Not Exist Level");
                set_monster.monster.exp = lv_iter->exp;
            }
        });
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
                execute_action(name(receiver), name(code), &battletest::eostransfer);          \
            }                                                                                  \
        }                                                                                      \
    }
// eos 금액에 대해 체크 하는 함

EOSIO_DISPATCH(battletest, (battleaction)(battlestate)(loginuser)(movemail)(balancetest)(movedb)(change)(testsnap)(towersnap)(claim)(settower)(equipment)(unequipment)(itemstore)(sellobject)(upgrade)(dberasestg)(dbinsertstg)(deletewhite)(addwhite)(deleteuser)(deletebattle)(stageexit)(deletebattle)(saveparty)(stagestart)(activeturn)(setdata)(herocheat)(partycheat)(resultpre)(resultgacha)(create)(issue)(transfer)(setmaster)(settokenlog)(eostransfer)(initmaster)(inittokenlog)(deleteblack)(addblack)(setpause)(dbinsert)(dberase)(dbinit))