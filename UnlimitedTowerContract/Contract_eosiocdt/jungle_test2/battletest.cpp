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
                _value_list.push_back(_value.substr(_size_list[i - 1] + 1, _size_list[i]));
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
    // if (_table == "dbbody")
    // {
    //     value = atoll(_value.c_str());
    //     insert_body(value);
    // }
    // else if (_table == "dbhair")
    // {
    //     value = atoll(_value.c_str());
    //     insert_hair(value);
    // }
    // else if (_table == "dbhead")
    // {
    //     value = atoll(_value.c_str());
    //     insert_head(value);
    // }
    // else if (_table == "dbgender")
    // {
    //     value = atoll(_value.c_str());
    //     insert_gender(value);
    // }
    // else if (_table == "dbmonsterup")
    // {
    //     // value = atoi(_value.c_str());
    //     // insert_upgrade_monster_ratio(value);
    // }
    // else if (_table == "dblevel")
    // {
    //     substr_value(_value, value_list, size_list, 3);
    //     insert_level(atoll(value_list[0].c_str()),
    //                  atoi(value_list[1].c_str()),
    //                  atoi(value_list[2].c_str()));
    // }
    // else if (_table == "dbservantjob")
    // {
    //     substr_value(_value, value_list, size_list, 4);
    //     insert_job(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
    // }
    // else if (_table == "dbitemgrade")
    // {
    //     substr_value(_value, value_list, size_list, 4);
    //     insert_item_grade(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
    // }
    // else if (_table == "dbmonstergd")
    // {
    //     substr_value(_value, value_list, size_list, 4);
    //     insert_monster_grade(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
    // }
    // else if (_table == "dbgraderatio")
    // {
    //     substr_value(_value, value_list, size_list, 2);
    //     insert_grade_ratio(atoi(value_list[0].c_str()), atoi(value_list[1].c_str()));
    // }
    // else if (_table == "dbservant")
    // {
    //     substr_value(_value, value_list, size_list, 7);
    //     insert_servant(atoll(value_list[0].c_str()),
    //                    atoll(value_list[1].c_str()),
    //                    atoi(value_list[2].c_str()),
    //                    atoi(value_list[3].c_str()),
    //                    atoi(value_list[4].c_str()),
    //                    atoi(value_list[5].c_str()),
    //                    atoi(value_list[6].c_str()));
    // }
    // else if (_table == "dbmonster")
    // {
    //     substr_value(_value, value_list, size_list, 4);
    //     insert_monster(atoll(value_list[0].c_str()),
    //                    atoll(value_list[1].c_str()),
    //                    atoll(value_list[2].c_str()),
    //                    atoll(value_list[3].c_str()));
    // }
    //     else if (_table == "dbgrind")
    //  {
    //      substr_value(_value, value_list, size_list, 2);
    //      insert_servant_grind_item(atoll(value_list[0].c_str()),
    //                                atoll(value_list[1].c_str()));
    //  }
    // else if (_table == "dbitemup")
    // {
    // }
    // else if (_table == "dballitem")
    // {
    //     substr_value(_value, value_list, size_list, 5);
    //     insert_all_item_id(atoll(value_list[0].c_str()),
    //                        atoi(value_list[1].c_str()),
    //                        atoll(value_list[2].c_str()),
    //                        atoll(value_list[3].c_str()),
    //                        atoll(value_list[4].c_str()));
    // }
    // else if (_table == "dbequipment")
    // {
    //     substr_value(_value, value_list, size_list, 13);
    //     insert_equip_item_id(atoll(value_list[0].c_str()),
    //                          atoll(value_list[1].c_str()),
    //                          atoll(value_list[2].c_str()),
    //                          atoll(value_list[3].c_str()),
    //                          atoll(value_list[4].c_str()),
    //                          atoll(value_list[5].c_str()),
    //                          atoll(value_list[6].c_str()),
    //                          atoll(value_list[7].c_str()),
    //                          atoll(value_list[8].c_str()),
    //                          atoll(value_list[9].c_str()),
    //                          atoll(value_list[10].c_str()),
    //                          atoll(value_list[11].c_str()),
    //                          atoll(value_list[12].c_str()));
    // }
    // else if (_table == "dbservantlv")
    // {
    //     // substr_value(_value, value_list, size_list, 4);
    //     // insert_servant_lv(atoll(value_list[0].c_str()),
    //     //                   atoll(value_list[1].c_str()),
    //     //                   atoll(value_list[2].c_str()),
    //     //                   atoll(value_list[3].c_str()));
    // }
    // else if (_table == "dbmonsterlv")
    // {
    //     // substr_value(_value, value_list, size_list, 4);
    //     // insert_monster_lv(atoll(value_list[0].c_str()),
    //     //                   atoll(value_list[1].c_str()),
    //     //                   atoll(value_list[2].c_str()),
    //     //                   atoll(value_list[3].c_str()));
    // }
    // else if (_table == "dbstatusserv")
    // {
    //     // substr_value(_value, value_list, size_list, 2);
    //     // insert_servant_lv_status(atoll(value_list[0].c_str()),
    //     //                   atoll(value_list[1].c_str()));
    // }
    // else if (_table == "dbstatusmon")
    // {
    //     // substr_value(_value, value_list, size_list, 2);
    //     // insert_monster_lv_status(atoll(value_list[0].c_str()),
    //     //                  atoll(value_list[1].c_str()));
    // }
    // else if (_table == "dbpassive")
    // {
    //     substr_value(_value, value_list, size_list, 10);
    //     insert_passive(atoll(value_list[0].c_str()),
    //                    atoi(value_list[1].c_str()),
    //                    atoi(value_list[2].c_str()),
    //                    atoi(value_list[3].c_str()),
    //                    atoi(value_list[4].c_str()),
    //                    atoi(value_list[5].c_str()),
    //                    atoi(value_list[6].c_str()),
    //                    atoi(value_list[7].c_str()),
    //                    atoi(value_list[8].c_str()),
    //                    atoi(value_list[9].c_str()));
    // }
    // else if (_table == "dbactive")
    // {
    //     substr_value(_value, value_list, size_list, 15);
    //     insert_active(atoll(value_list[0].c_str()),
    //                   atoi(value_list[1].c_str()),
    //                   atoi(value_list[2].c_str()),
    //                   atoi(value_list[3].c_str()),
    //                   atoi(value_list[4].c_str()),
    //                   atoi(value_list[5].c_str()),
    //                   atoi(value_list[6].c_str()),
    //                   atoi(value_list[7].c_str()),
    //                   atoi(value_list[8].c_str()),
    //                   atoi(value_list[9].c_str()),
    //                   atoi(value_list[10].c_str()),
    //                   atoi(value_list[11].c_str()),
    //                   atoi(value_list[12].c_str()),
    //                   atoi(value_list[13].c_str()),
    //                   atoi(value_list[14].c_str()));
    // }
    // else if (_table == "dbgachapool")
    // {
    //     substr_value(_value, value_list, size_list, 2);
    //     insert_gacha_pool(atoll(value_list[0].c_str()),
    //                       atoll(value_list[1].c_str()));
    // }
    // else if (_table == "dbstageenemy")
    // {
    //     substr_value(_value, value_list, size_list, 15);
    //     insert_stage_enemy(atoll(value_list[0].c_str()),
    //                        atoi(value_list[1].c_str()),
    //                        atoi(value_list[2].c_str()),
    //                        atoi(value_list[3].c_str()),
    //                        atoi(value_list[4].c_str()),
    //                        atoi(value_list[5].c_str()),
    //                        atoi(value_list[6].c_str()),
    //                        atoi(value_list[7].c_str()),
    //                        atoi(value_list[8].c_str()),
    //                        atoi(value_list[9].c_str()),
    //                        atoi(value_list[10].c_str()),
    //                        atoi(value_list[11].c_str()),
    //                        atoi(value_list[12].c_str()),
    //                        atoi(value_list[13].c_str()),
    //                        atoi(value_list[14].c_str()));
    // }
    if (_table == "dbstage")
    {
        substr_value(_value, value_list, size_list, 9);
        insert_stage(atoll(value_list[0].c_str()),
                     atoi(value_list[1].c_str()),
                     atoi(value_list[2].c_str()),
                     atoi(value_list[3].c_str()),
                     atoi(value_list[4].c_str()),
                     atoi(value_list[5].c_str()),
                     atoi(value_list[6].c_str()),
                     atoi(value_list[7].c_str()),
                     atoi(value_list[8].c_str()));
    }
    // else if (_table == "dbmonsterlist")
    // {
    //     substr_value(_value, value_list, size_list, 3);
    //     insert_stage_monster(atoll(value_list[0].c_str()),
    //                          atoi(value_list[1].c_str()),
    //                          atoi(value_list[2].c_str()));
    // }

    if (_table == "dbreward")
    {
        substr_value(_value, value_list, size_list, 9);
        insert_stage_reward(atoll(value_list[0].c_str()),
                      atoi(value_list[1].c_str()),
                      atoi(value_list[2].c_str()),
                      atoi(value_list[3].c_str()),
                      atoi(value_list[4].c_str()),
                      atoi(value_list[5].c_str()),
                      atoi(value_list[6].c_str()),
                      atoi(value_list[7].c_str()),
                      atoi(value_list[8].c_str()));
    }
    // else if (_table == "dbrewardlist")
    // {
    //     substr_value(_value, value_list, size_list, 6);
    //     insert_reward(atoll(value_list[0].c_str()),
    //                   atoi(value_list[1].c_str()),
    //                   atoi(value_list[2].c_str()),
    //                   atoi(value_list[3].c_str()),
    //                   atoi(value_list[4].c_str()),
    //                   atoi(value_list[5].c_str()));
    // }
    // else if (_table == "dbclassstat")
    // {
    //             substr_value(_value, value_list, size_list, 7);
    //     insert_class_stat(atoll(value_list[0].c_str()),
    //                    atoll(value_list[1].c_str()),
    //                    atoi(value_list[2].c_str()),
    //                    atoi(value_list[3].c_str()),
    //                    atoi(value_list[4].c_str()),
    //                    atoi(value_list[5].c_str()),
    //                    atoi(value_list[6].c_str()));
    // }
    // else if(_table == "dbclassstat_passive")
    // {
    //     substr_value(_value, value_list, size_list, 4);
    //     insert_class_stat_passive(atoi(value_list[0].c_str()),
    //                               atoi(value_list[1].c_str()),
    //                               atoi(value_list[2].c_str()),
    //                               atoi(value_list[3].c_str()));
    // }
    //     else if(_table == "dbclassstat_active")
    // {
    //     substr_value(_value, value_list, size_list, 4);
    //     insert_class_stat_active(atoi(value_list[0].c_str()),
    //                               atoi(value_list[1].c_str()),
    //                               atoi(value_list[2].c_str()),
    //                               atoi(value_list[3].c_str()));
    // }

    //     else if (_table == "dbjobstat")
    // {
    //             substr_value(_value, value_list, size_list, 7);
    //     insert_job_stat(atoll(value_list[0].c_str()),
    //                    atoll(value_list[1].c_str()),
    //                    atoi(value_list[2].c_str()),
    //                    atoi(value_list[3].c_str()),
    //                    atoi(value_list[4].c_str()),
    //                    atoi(value_list[5].c_str()),
    //                    atoi(value_list[6].c_str()));
    // }
    // else if(_table == "dbjobstat_passive")
    // {
    //     substr_value(_value, value_list, size_list, 4);
    //     insert_job_stat_passive(atoi(value_list[0].c_str()),
    //                               atoi(value_list[1].c_str()),
    //                               atoi(value_list[2].c_str()),
    //                               atoi(value_list[3].c_str()));
    // }
    //     else if(_table == "dbjobstat_active")
    // {
    //     substr_value(_value, value_list, size_list, 4);
    //     insert_job_stat_active(atoi(value_list[0].c_str()),
    //                               atoi(value_list[1].c_str()),
    //                               atoi(value_list[2].c_str()),
    //                               atoi(value_list[3].c_str()));
    // }
    // else
    // {
    //     eosio_assert(1 == 0, "Not Exist Table");
    // }
}

ACTION battletest::dblistinsert(std::string _list, std::string _primary_key, std::vector<std::string> _value_list)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 1");
    if (_list == "dbstage_monsterlist")
    {
        uint64_t stage_id = atoll(_primary_key.c_str());
        stage_db stage_db_table(_self, _self.value);
        auto stage_db_iter = stage_db_table.find(stage_id);
        eosio_assert(stage_db_iter != stage_db_table.end(), "DB List Insert : Not Exist Stage");

        stage_db_table.modify(stage_db_iter, _self, [&](auto &new_data) {
            for (uint32_t i = 0; i < _value_list.size();)
            {
                stage_monster_info new_monster;
                new_monster.monster_id = atoi(_value_list[i].c_str());
                new_monster.position = atoi(_value_list[i+1].c_str());
                new_data.monster_list.push_back(new_monster);
                i += 2;
            }
        });
    }
    if (_list == "dbreward_rewardlist")
    {
        uint64_t stage_id = atoll(_primary_key.c_str());
        reward_db reward_db_table(_self, _self.value);
        auto stage_db_iter = reward_db_table.find(stage_id);
        eosio_assert(stage_db_iter != reward_db_table.end(), "DB List Insert : Not Exist Reward");

        reward_db_table.modify(stage_db_iter, _self, [&](auto &new_data) {
            for (uint32_t i = 0; i < _value_list.size();)
            {
                reward_info new_reward;
                new_reward.type = atoi(_value_list[i].c_str());
                new_reward.id = atoi(_value_list[i + 1].c_str());
                new_reward.grade = atoi(_value_list[i + 2].c_str());
                new_reward.per = atoi(_value_list[i + 3].c_str());
                new_reward.count = atoi(_value_list[i + 4].c_str());
                new_data.reward_list.push_back(new_reward);
                i += 5;
            }
        });
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

void battletest::insert_servant(uint64_t _servant_id, uint32_t _job, uint32_t _body, uint32_t _gender, uint32_t _head, uint32_t _hair, uint32_t _grade)
{
    servant_db servant_id_db_table(_self, _self.value);
    auto servant_id_iter = servant_id_db_table.find(_servant_id);
    if (servant_id_iter == servant_id_db_table.end())
    {
        servant_id_db_table.emplace(_self, [&](auto &new_servant_id) {
            new_servant_id.id = _servant_id;
            new_servant_id.job = _job;
            new_servant_id.body = _body;
            new_servant_id.gender = _gender;
            new_servant_id.head = _head;
            new_servant_id.hair = _hair;
            new_servant_id.grade = _grade;
        });
    }
    else
    {
        servant_id_db_table.modify(servant_id_iter, _self, [&](auto &new_servant_id) {
            new_servant_id.job = _job;
            new_servant_id.body = _body;
            new_servant_id.gender = _gender;
            new_servant_id.head = _head;
            new_servant_id.hair = _hair;
            new_servant_id.grade = _grade;
        });
    }
}

void battletest::insert_monster(uint64_t _monster_id, uint64_t _tribe, uint64_t _type, uint64_t _monster_class)
{
    monster_db monster_id_db_table(_self, _self.value);
    auto monster_id_iter = monster_id_db_table.find(_monster_id);
    if (monster_id_iter == monster_id_db_table.end())
    {
        monster_id_db_table.emplace(_self, [&](auto &new_monster_id) {
            new_monster_id.id = _monster_id;
            new_monster_id.tribe = _tribe;
            new_monster_id.type = _type;
            new_monster_id.monster_class = _monster_class;
        });
    }
    else
    {
        monster_id_db_table.modify(monster_id_iter, _self, [&](auto &new_monster_id) {
            new_monster_id.tribe = _tribe;
            new_monster_id.type = _type;
            new_monster_id.monster_class = _monster_class;
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

void battletest::insert_all_item_id(uint64_t _item_id, uint32_t _type, uint32_t _param, uint64_t _sell_id, uint64_t _sell_cost)
{
    allitem_db item_id_db_table(_self, _self.value);
    auto item_id_iter = item_id_db_table.find(_item_id);
    if (item_id_iter == item_id_db_table.end())
    {
        item_id_db_table.emplace(_self, [&](auto &new_item_id) {
            new_item_id.id = _item_id;
            new_item_id.type = _type;
            if (_param == 0)
            {
                new_item_id.item_param_list.resize(0);
            }
            else
            {
                new_item_id.item_param_list.push_back(_param);
            }
            new_item_id.sell_id = _sell_id;
            new_item_id.sell_cost = _sell_cost;
        });
    }
    else
    {
        item_id_db_table.modify(item_id_iter, _self, [&](auto &new_item_id) {
            new_item_id.id = _item_id;
            new_item_id.type = _type;
            new_item_id.item_param_list.push_back(_param);
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

// void battletest::insert_upgrade_item_ratio(uint64_t _main, uint32_t _material, uint64_t _ratio)
// {
//     upgrade_item_ratio_db upgrade_itme_ratio_db_table(_self, _self.value);
//     auto upgrade_item_ratio_iter = upgrade_itme_ratio_db_table.find(_main);
//     if (upgrade_item_ratio_iter == upgrade_itme_ratio_db_table.end())
//     {
//         upgrade_itme_ratio_db_table.emplace(_self, [&](auto &new_upgrade_item_ratio) {
//             new_upgrade_item_ratio.upgrade_item = _main;
//             new_upgrade_item_ratio.material = _material;
//             new_upgrade_item_ratio.ratio = _ratio;
//         });
//     }
//     else
//     {
//         upgrade_itme_ratio_db_table.modify(upgrade_item_ratio_iter, _self, [&](auto &new_upgrade_item_ratio) {
//             new_upgrade_item_ratio.material = _material;
//             new_upgrade_item_ratio.ratio = _ratio;
//         });
//     }
// }

ACTION battletest::insertequipr(uint64_t _main, std::vector<uint64_t> &_upgrade_ratio, uint64_t _material_id, std::vector<uint64_t> &_material_count, std::vector<uint64_t> &_use_UTG)
{
    upgrade_equipment_ratio_dbs upgrade_equipment_ratio_db_table(_self, _self.value);
    auto upgrade_equipment_ratio_db_iter = upgrade_equipment_ratio_db_table.find(_main);
    if (upgrade_equipment_ratio_db_iter == upgrade_equipment_ratio_db_table.end())
    {
        upgrade_equipment_ratio_db_table.emplace(_self, [&](auto &new_upgrade_equipment_iter) {
            new_upgrade_equipment_iter.equipment_type_grade = _main;
            new_upgrade_equipment_iter.upgrade_ratio = _upgrade_ratio;
            new_upgrade_equipment_iter.material_id = _material_id;
            new_upgrade_equipment_iter.material_count = _material_count;
            new_upgrade_equipment_iter.use_UTG = _use_UTG;
        });
    }
    else
    {
        upgrade_equipment_ratio_db_table.modify(upgrade_equipment_ratio_db_iter, _self, [&](auto &new_upgrade_equipment_iter) {
            new_upgrade_equipment_iter.equipment_type_grade = _main;
            new_upgrade_equipment_iter.upgrade_ratio = _upgrade_ratio;
            new_upgrade_equipment_iter.material_id = _material_id;
            new_upgrade_equipment_iter.material_count = _material_count;
            new_upgrade_equipment_iter.use_UTG = _use_UTG;
        });
    }
}

// void battletest::insert_level(uint32_t _id)
// {
//     uint64_t temp_exp = (-180 + 259 * _id + 93 * _id * _id + 8 * _id * _id * _id) * 5 / 6;
//     lv_exp lv_exp_table(_self, _self.value);
//     lv_exp_table.emplace(_self, [&](auto &new_lv_exp) {
//         new_lv_exp.lv = _id + 1;
//         new_lv_exp.exp = temp_exp;
//     });
// }
void battletest::insert_level(uint32_t _level, uint32_t _rank_exp, uint32_t _char_exp)
{
    lv_exp lv_exp_table(_self, _self.value);
    for (uint32_t i = 1; i <= _level; ++i)
    {
        lv_exp_table.emplace(_self, [&](auto &new_lv_exp) {
            new_lv_exp.lv = i;
            new_lv_exp.rank_exp = (i + 1) * (i + 1) * 8;
            new_lv_exp.char_exp = (((i) * (i)) * ((i + 1) * 8));
        });
    }
}

// void battletest::insert_servant_lv(uint64_t _job, uint64_t _lv_up_str, uint64_t _lv_up_dex, uint64_t _lv_up_int)
// {
//     servant_lv_db servant_lv_db_table(_self, _self.value);
//     auto servant_lv_db_iter = servant_lv_db_table.find(_job);
//     if (servant_lv_db_iter == servant_lv_db_table.end())
//     {
//         servant_lv_db_table.emplace(_self, [&](auto &new_servant_lv) {
//             new_servant_lv.job = _job;
//             new_servant_lv.lvup_str = _lv_up_str;
//             new_servant_lv.lvup_dex = _lv_up_dex;
//             new_servant_lv.lvup_int = _lv_up_int;
//         });
//     }
//     else
//     {
//         servant_lv_db_table.modify(servant_lv_db_iter, _self, [&](auto &new_servant_lv) {
//             new_servant_lv.job = _job;
//             new_servant_lv.lvup_str = _lv_up_str;
//             new_servant_lv.lvup_dex = _lv_up_dex;
//             new_servant_lv.lvup_int = _lv_up_int;
//         });
//     }
// }

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

// void battletest::insert_servant_lv_status(uint64_t _type, uint64_t _num)
// {
//     servant_lv_status_db servant_lv_status_db_table(_self, _self.value);

//     servant_lv_status_db_table.emplace(_self, [&](auto &new_servant_lv_status_db) {
//         new_servant_lv_status_db.grade = _type;
//         std::vector<lv_status_sub> lv_status_list;
//         std::vector<uint64_t> temp_list;
//         for (uint32_t i = 0; i <= 9; i++)
//         {
//             temp_list.push_back(_num);
//             _num += 1;
//         }

//         for (uint32_t i = 0; i <= 9; i++)
//         {
//             lv_status_sub new_sub;
//             new_sub.pre_status = i;
//             new_sub.update_status = temp_list[i];

//             new_servant_lv_status_db.lv_status.push_back(new_sub);
//         }
//     });
// }

// void battletest::insert_monster_lv_status(uint64_t _type, uint64_t _num)
// {
//     monster_lv_status_db monster_lv_status_db_table(_self, _self.value);

//     monster_lv_status_db_table.emplace(_self, [&](auto &new_monster_lv_status_db) {
//         new_monster_lv_status_db.grade = _type;
//         std::vector<lv_status_sub> lv_status_list;
//         std::vector<uint64_t> temp_list;
//         for (uint32_t i = 0; i <= 9; i++)
//         {
//             temp_list.push_back(_num);
//             _num += 1;
//         }

//         for (uint32_t i = 0; i <= 9; i++)
//         {
//             lv_status_sub new_sub;
//             new_sub.pre_status = i;
//             new_sub.update_status = temp_list[i];

//             new_monster_lv_status_db.lv_status.push_back(new_sub);
//         }
//     });
// }

void battletest::insert_equipment_lv_status(uint64_t _type) //, uint64_t _num)
{
    equipment_lv_status_db equipment_lv_status_db_table(_self, _self.value);

    equipment_lv_status_db_table.emplace(_self, [&](auto &new_equipment_lv_status_db) {
        new_equipment_lv_status_db.type_grade = _type;
        std::vector<lv_status_sub> lv_status_list;
        std::vector<uint64_t> temp_list;
        // if (_type <= 25)
        // {
        //     for (uint32_t i = 0; i <= 9; i++)
        //     {
        //         temp_list.push_back(_num);
        //         _num += 2;
        //     }

        //     for (uint32_t i = 0; i <= 9; i++)
        //     {
        //         lv_status_sub new_sub;
        //         new_sub.pre_status = i;
        //         new_sub.update_status = temp_list[i];

        //         new_equipment_lv_status_db.change_status.push_back(new_sub);
        //     }
        // }
        // else
        // {
        //     for (uint32_t i = 0; i <= 9; i++)
        //     {
        //         temp_list.push_back(_num);
        //         _num += 1;
        //     }

        //     for (uint32_t i = 0; i <= 9; i++)
        //     {
        //         lv_status_sub new_sub;
        //         new_sub.pre_status = i;
        //         new_sub.update_status = temp_list[i];

        //         new_equipment_lv_status_db.change_status.push_back(new_sub);
        //     }
        // }

        switch (_type)
        {
        case 11:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_wepon_legendary_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 12:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_wepon_unique_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 13:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_wepon_rare_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 14:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_wepon_uncommon_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 15:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_wepon_common_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 21:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_armor_legendary_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 22:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_armor_unique_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 23:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_armor_rare_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 24:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_armor_uncommon_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 25:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_armor_common_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 31:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_ac_legendary_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 32:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_ac_unique_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 33:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_ac_rare_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 34:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_ac_uncommon_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        case 35:
        {
            for (uint32_t i = 0; i <= 9; i++)
            {
                lv_status_sub new_sub;
                new_sub.pre_status = i;
                new_sub.update_status = equipment_ac_common_status_list[i];
                new_equipment_lv_status_db.change_status.push_back(new_sub);
            }
            break;
        }
        }
    });
}

void battletest::insert_passive(uint64_t _id, uint32_t _job, uint32_t _monster_class, uint32_t _enable_stack, uint32_t _max_stack,
                                uint32_t _effect_type, uint32_t _effect_value, uint32_t _effect_value_add, uint32_t _target, uint32_t _role_target)
{
    passive_db passive_db_table(_self, _self.value);
    auto passive_db_iter = passive_db_table.find(_id);
    if (passive_db_iter == passive_db_table.end())
    {
        passive_db_table.emplace(_self, [&](auto &new_passive) {
            new_passive.passive_id = _id;
            new_passive.job = _job;
            new_passive.monster_class = _monster_class;
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
        passive_db_table.modify(passive_db_iter, _self, [&](auto &new_passive) {
            new_passive.job = _job;
            new_passive.monster_class = _monster_class;
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

void battletest::insert_active(uint64_t _id, uint32_t _job, uint32_t _monster_class, uint32_t _active_per,
                               uint32_t _skill_type, uint32_t _attack_type, uint32_t _dmg_type, uint32_t _target, uint32_t _target_count, uint32_t _target_range,
                               uint32_t _hit_count, uint32_t _atk_per, uint32_t _atk_per_add, uint32_t _heal_per, uint32_t _heal_per_add)
{
    active_db active_db_table(_self, _self.value);
    auto active_db_iter = active_db_table.find(_id);
    if (active_db_iter == active_db_table.end())
    {
        active_db_table.emplace(_self, [&](auto &new_active) {
            new_active.active_id = _id;
            new_active.job = _job;
            new_active.monster_class = _monster_class;
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
            new_active.monster_class = _monster_class;
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

void battletest::insert_gacha_pool(uint64_t _gacha_id, uint64_t _db_index)
{
    main_gacha_db main_gacha_db_table(_self, _self.value);
    auto main_gacha_db_iter = main_gacha_db_table.find(_gacha_id);
    if (main_gacha_db_iter == main_gacha_db_table.end())
    {
        main_gacha_db_table.emplace(_self, [&](auto &new_gacha) {
            new_gacha.gacha_id = _gacha_id;
            new_gacha.db_index = _db_index;
        });
    }
    else
    {
        main_gacha_db_table.modify(main_gacha_db_iter, _self, [&](auto &new_gacha) {
            new_gacha.db_index = _db_index;
        });
    }
}

void battletest::insert_stage_enemy(uint64_t _id,
                                    uint32_t _grade,
                                    uint32_t _enemy_class,
                                    uint32_t _type,
                                    uint32_t _enemy_str,
                                    uint32_t _enemy_dex,
                                    uint32_t _enemy_int,
                                    uint32_t _physical_cri_per,
                                    uint32_t _physical_cri_dmg,
                                    uint32_t _magic_cri_per,
                                    uint32_t _magic_cri_dmg,
                                    uint32_t _speed,
                                    uint32_t _avoid,
                                    uint32_t _active,
                                    uint32_t _passive)
{
    stage_enemy_db stage_enemy_db_table(_self, _self.value);
    auto enemy_iter = stage_enemy_db_table.find(_id);
    if (enemy_iter == stage_enemy_db_table.end())
    {
        stage_enemy_db_table.emplace(_self, [&](auto &new_enemy) {
            new_enemy.id = _id;
            new_enemy.grade = _grade;
            new_enemy.enemy_class = _enemy_class;
            new_enemy.type = _type;
            new_enemy.enemy_str = _enemy_str;
            new_enemy.enemy_dex = _enemy_dex;
            new_enemy.enemy_int = _enemy_int;
            new_enemy.physical_cri_per = _physical_cri_per;
            new_enemy.physical_cri_dmg = _physical_cri_dmg;
            new_enemy.magic_cri_per = _magic_cri_per;
            new_enemy.magic_cri_dmg = _magic_cri_dmg;
            new_enemy.speed = _speed;
            new_enemy.avoid = _avoid;
            if (_active != 0)
            {
                new_enemy.active_list.push_back(_active);
            }
            if (_passive != 0)
            {
                new_enemy.passive_list.push_back(_passive);
            }
        });
    }
    else
    {
        stage_enemy_db_table.modify(enemy_iter, _self, [&](auto &new_enemy) {
            new_enemy.grade = _grade;
            new_enemy.enemy_class = _enemy_class;
            new_enemy.type = _type;
            new_enemy.enemy_str = _enemy_str;
            new_enemy.enemy_dex = _enemy_dex;
            new_enemy.enemy_int = _enemy_int;
            new_enemy.physical_cri_per = _physical_cri_per;
            new_enemy.physical_cri_dmg = _physical_cri_dmg;
            new_enemy.magic_cri_per = _magic_cri_per;
            new_enemy.magic_cri_dmg = _magic_cri_dmg;
            new_enemy.speed = _speed;
            new_enemy.avoid = _avoid;
            if (_active != 0)
            {
                new_enemy.active_list.push_back(_active);
            }
            if (_passive != 0)
            {
                new_enemy.passive_list.push_back(_passive);
            }
        });
    }
}

void battletest::insert_stage(uint64_t _stage_id,
                              uint32_t _stage_type,
                              uint32_t _stage_floor,
                              uint32_t _need_stage_id,
                              uint32_t _stage_group_index,
                              uint32_t _need_entrance_item_id,
                              uint32_t _need_entrance_item_count,
                              uint32_t _enemy_level_min,
                              uint32_t _enemy_level_max)
{
    stage_db stage_db_table(_self, _self.value);
    auto stage_iter = stage_db_table.find(_stage_id);
    if (stage_iter == stage_db_table.end())
    {
        stage_db_table.emplace(_self, [&](auto &new_stage) {
            new_stage.stage_id = _stage_id;
            new_stage.stage_type = _stage_type;
            new_stage.stage_floor = _stage_floor;
            new_stage.need_stage_id = _need_stage_id;
            new_stage.stage_group_index = _stage_group_index;
            new_stage.need_entrance_item_id = _need_entrance_item_id;
            new_stage.need_entrance_item_count = _need_entrance_item_count;
            new_stage.enemy_level_min = _enemy_level_min;
            new_stage.enemy_level_max = _enemy_level_max;
        });
    }
    else
    {
        stage_db_table.modify(stage_iter, _self, [&](auto &new_stage) {
            new_stage.stage_type = _stage_type;
            new_stage.stage_floor = _stage_floor;
            new_stage.need_stage_id = _need_stage_id;
            new_stage.stage_group_index = _stage_group_index;
            new_stage.need_entrance_item_id = _need_entrance_item_id;
            new_stage.need_entrance_item_count = _need_entrance_item_count;
            new_stage.enemy_level_min = _enemy_level_min;
            new_stage.enemy_level_max = _enemy_level_max;
        });
    }
}

void battletest::insert_stage_monster(uint64_t _stage_id, uint32_t _monster_id, uint32_t _position)
{
    stage_db stage_db_table(_self, _self.value);
    auto stage_iter = stage_db_table.find(_stage_id);
    eosio_assert(stage_iter != stage_db_table.end(), "Not Exist Stage 1");
    stage_db_table.modify(stage_iter, _self, [&](auto &new_stage) {
        stage_monster_info monster;
        monster.monster_id = _monster_id;
        monster.position = _position;
        new_stage.monster_list.push_back(monster);
    });
}

// void battletest::insert_temp(uint64_t _stage_id, std::vector<uint32_t> &_monster_position)
// {
//     stage_db stage_db_table(_self, _self.value);
//     auto stage_db_iter = stage_db_table.find(_stage_id);
    

// }


void battletest::insert_stage_reward(uint64_t _stage_id,
                                     uint32_t _rank_exp,
                                     uint32_t _char_exp,
                                     uint32_t _first_reward_type,
                                     uint32_t _first_reward_id,
                                     uint32_t _first_reward_grade,
                                     uint32_t _first_reward_count,
                                     uint32_t _reward_utg_min,
                                     uint32_t _reward_utg_max)
{
    reward_db reward_db_table(_self, _self.value);
    auto reward_iter = reward_db_table.find(_stage_id);
    if (reward_iter == reward_db_table.end())
    {
        reward_db_table.emplace(_self, [&](auto &new_data) {
            new_data.stage_id = _stage_id;
            new_data.rank_exp = _rank_exp;
            new_data.char_exp = _char_exp;
            new_data.first_reward_type = _first_reward_type;
            new_data.first_reward_id = _first_reward_id;
            new_data.first_reward_grade = _first_reward_grade;
            new_data.first_reward_count = _first_reward_count;
            new_data.reward_utg_min = _reward_utg_min;
            new_data.reward_utg_max = _reward_utg_max;
        });
    }
    else
    {
        reward_db_table.modify(reward_iter, _self, [&](auto &new_data) {
            new_data.rank_exp = _rank_exp;
            new_data.char_exp = _char_exp;
            new_data.first_reward_type = _first_reward_type;
            new_data.first_reward_id = _first_reward_id;
            new_data.first_reward_grade = _first_reward_grade;
            new_data.first_reward_count = _first_reward_count;
            new_data.reward_utg_min = _reward_utg_min;
            new_data.reward_utg_max = _reward_utg_max;
        });
    }
}
void battletest::insert_reward(uint64_t _stage_id, uint32_t _type, uint32_t _id, uint32_t _grade, uint32_t _per, uint32_t _count)
{
    reward_db reward_db_table(_self, _self.value);
    auto reward_iter = reward_db_table.find(_stage_id);
    eosio_assert(reward_iter != reward_db_table.end(), "Not Exist Stage ID 3");
    reward_db_table.modify(reward_iter, _self, [&](auto &new_data) {
        reward_info reward;
        reward.type = _type;
        reward.id = _id;
        reward.grade = _grade;
        reward.per = _per;
        reward.count = _count;
        new_data.reward_list.push_back(reward);
    });
}

void battletest::insert_class_stat(uint64_t _id,
                                   uint32_t _speed,
                                   uint32_t _avoid,
                                   uint32_t _physical_cri_per,
                                   uint32_t _physical_cri_dmg,
                                   uint32_t _magic_cri_per,
                                   uint32_t _magic_cri_dmg)
{
    class_stat_db class_stat_db_table(_self, _self.value);
    auto class_iter = class_stat_db_table.find(_id);
    if (class_iter == class_stat_db_table.end())
    {
        class_stat_db_table.emplace(_self, [&](auto &new_data) {
            new_data.id = _id;
            new_data.speed = _speed;
            new_data.avoid = _avoid;
            new_data.physical_cri_per = _physical_cri_per;
            new_data.physical_cri_dmg = _physical_cri_dmg;
            new_data.magic_cri_per = _magic_cri_per;
            new_data.magic_cri_dmg = _magic_cri_dmg;
        });
    }
    else
    {
        class_stat_db_table.modify(class_iter, _self, [&](auto &new_data) {
            new_data.speed = _speed;
            new_data.avoid = _avoid;
            new_data.physical_cri_per = _physical_cri_per;
            new_data.physical_cri_dmg = _physical_cri_dmg;
            new_data.magic_cri_per = _magic_cri_per;
            new_data.magic_cri_dmg = _magic_cri_dmg;
        });
    }
}

void battletest::insert_job_stat(uint64_t _id,
                                 uint32_t _speed,
                                 uint32_t _avoid,
                                 uint32_t _physical_cri_per,
                                 uint32_t _physical_cri_dmg,
                                 uint32_t _magic_cri_per,
                                 uint32_t _magic_cri_dmg)
{
    job_stat_db class_stat_db_table(_self, _self.value);
    auto class_iter = class_stat_db_table.find(_id);
    if (class_iter == class_stat_db_table.end())
    {
        class_stat_db_table.emplace(_self, [&](auto &new_data) {
            new_data.id = _id;
            new_data.speed = _speed;
            new_data.avoid = _avoid;
            new_data.physical_cri_per = _physical_cri_per;
            new_data.physical_cri_dmg = _physical_cri_dmg;
            new_data.magic_cri_per = _magic_cri_per;
            new_data.magic_cri_dmg = _magic_cri_dmg;
        });
    }
    else
    {
        class_stat_db_table.modify(class_iter, _self, [&](auto &new_data) {
            new_data.speed = _speed;
            new_data.avoid = _avoid;
            new_data.physical_cri_per = _physical_cri_per;
            new_data.physical_cri_dmg = _physical_cri_dmg;
            new_data.magic_cri_per = _magic_cri_per;
            new_data.magic_cri_dmg = _magic_cri_dmg;
        });
    }
}

void battletest::insert_class_stat_passive(uint64_t _claas, uint64_t _private_id,
                                           uint32_t _per,
                                           uint32_t _public_id)
{
    class_stat_db class_stat_db_table(_self, _self.value);
    auto class_iter = class_stat_db_table.find(_claas);
    eosio_assert(class_iter != class_stat_db_table.end(), "Not Exist Claas Stat Data");

    class_stat_db_table.modify(class_iter, _self, [&](auto &new_data) {
        if (_private_id != 0)
        {
            new_data.passive_private_list.push_back(_private_id);
        }
        if (_public_id != 0)
        {
            new_data.passive_public_list.push_back(_public_id);
        }
        new_data.passive_private_per = _per;
    });
}

void battletest::insert_class_stat_active(uint64_t _claas, uint64_t _private_id,
                                          uint32_t _per,
                                          uint32_t _public_id)
{
    class_stat_db class_stat_db_table(_self, _self.value);
    auto class_iter = class_stat_db_table.find(_claas);
    eosio_assert(class_iter != class_stat_db_table.end(), "Not Exist Claas Stat Data");

    class_stat_db_table.modify(class_iter, _self, [&](auto &new_data) {
        if (_private_id != 0)
        {
            new_data.active_private_list.push_back(_private_id);
        }
        if (_public_id != 0)
        {
            new_data.active_public_list.push_back(_public_id);
        }
        new_data.active_private_per = _per;
    });
}

void battletest::insert_job_stat_passive(uint64_t _job, uint64_t _private_id,
                                         uint32_t _per,
                                         uint32_t _public_id)
{
    job_stat_db class_stat_db_table(_self, _self.value);
    auto class_iter = class_stat_db_table.find(_job);
    eosio_assert(class_iter != class_stat_db_table.end(), "Not Exist Job Stat Data");

    class_stat_db_table.modify(class_iter, _self, [&](auto &new_data) {
        if (_private_id != 0)
        {
            new_data.passive_private_list.push_back(_private_id);
        }
        if (_public_id != 0)
        {
            new_data.passive_public_list.push_back(_public_id);
        }
        new_data.passive_private_per = _per;
    });
}

void battletest::insert_job_stat_active(uint64_t _job, uint64_t _private_id,
                                        uint32_t _per,
                                        uint32_t _public_id)
{
    job_stat_db class_stat_db_table(_self, _self.value);
    auto class_iter = class_stat_db_table.find(_job);
    eosio_assert(class_iter != class_stat_db_table.end(), "Not Exist Job Stat Data");

    class_stat_db_table.modify(class_iter, _self, [&](auto &new_data) {
        if (_private_id != 0)
        {
            new_data.active_private_list.push_back(_private_id);
        }
        if (_public_id != 0)
        {
            new_data.active_public_list.push_back(_public_id);
        }
        new_data.active_private_per = _per;
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
    // if (_table == "dbbody")
    // {
    //     value = atoll(_value.c_str());
    //     erase_body(value);
    // }
    // else if (_table == "dbhair")
    // {
    //     value = atoll(_value.c_str());
    //     erase_hair(value);
    // }
    // else if (_table == "dbhead")
    // {
    //     value = atoll(_value.c_str());
    //     erase_head(value);
    // }
    // else if (_table == "dbgender")
    // {
    //     value = atoll(_value.c_str());
    //     erase_gender(value);
    // }
    //     else if (_table == "dbservantjob")
    // {
    //     value = atoll(_value.c_str());
    //     erase_job(value);
    // }
    // else if (_table == "dbitemgrade")
    // {
    //     value = atoll(_value.c_str());
    //     erase_item_grade(value);
    // }
    //     else if (_table == "dbmonstergd")
    // {
    //     value = atoll(_value.c_str());
    //     erase_monster_grade(value);
    // }
    //     else if (_table == "dbgraderatio")
    // {
    //     value = atoll(_value.c_str());
    //     erase_grade_ratio(value);
    // }
    if (_table == "dbmonsterup")
    {
        value = atoi(_value.c_str());
        erase_upgrade_monster_ratio(value);
    }
    if (_table == "dblevel")
    {
        value = atoi(_value.c_str());
        erase_level(value);
    }
    if (_table == "dbburn")
    {
        value = atoi(_value.c_str());
    }
    if (_table == "dbservants")
    {
        value = atoll(_value.c_str());
        erase_servant_id(value);
    }
    if (_table == "dbmonsters")
    {
        value = atoll(_value.c_str());
        erase_monster_id(value);
    }
    if (_table == "dballitem")
    {
        value = atoll(_value.c_str());
        erase_all_item_id(value);
    }
    if (_table == "dbequipment")
    {
        value = atoll(_value.c_str());
        erase_equip_item_id(value);
    }
    else if (_table =="dbservnatlv")
    {
        value = atoll(_value.c_str());
        erase_servant_lv(value);
    }
    if (_table =="dbmonsterlv")
    {
        value = atoll(_value.c_str());
        erase_monster_lv(value);
    }
    if (_table =="dbstatusserv")
    {
        value = atoll(_value.c_str());
        erase_servant_lv_status(value);
    }
    if (_table =="dbstatusmon")
    {
        value = atoll(_value.c_str());
        erase_monster_lv_status(value);
    }
    if (_table == "dbstatusmon_change_status")
    {
        value = atoll(_value.c_str());
        erase_monster_lv_status_list(value);
    }
    if (_table == "dbpassive")
    {
        value = atoll(_value.c_str());
        erase_passive(value);
    }
    if (_table == "dbactive")
    {
        value = atoll(_value.c_str());
        erase_active(value);
    }
    if (_table == "dbgachapool")
    {
        value = atoll(_value.c_str());
        erase_gacha_pool(value);
    }
    if (_table == "dbstageenemy")
    {
        value = atoll(_value.c_str());
        erase_stage_enemy(value);
    }
    if (_table == "dbstatusequi")
    {
        value = atoll(_value.c_str());
        erase_equipment_lv_status(value);
    }
    if( _table == "dbstatusequi_change_status")
    {
        value = atoll(_value.c_str());
        erase_equipment_lv_status_list(value);
    }
    if (_table == "dbstage") //스테이지 전체를 날림
    {
        value = atoll(_value.c_str());
        erase_stage(value);
    }
    if (_table == "dbstage_monsterlist") //스테이지에 몬스터 리스트만 전체 날림
    {
        value = atoll(_value.c_str());
        erase_stage_monster_list(value);
    }
    if (_table == "dbreward") //보상 테이블을 전체 날림
    {
        value = atoll(_value.c_str());
        erase_stage_reward(value);
    }
    if (_table == "db_reward_rewardlist") //보상 테이블에 보상 리스트만 날림
    {
        value = atoll(_value.c_str());
        erase_stage_reward_list(value);
    }
    if (_table == "dbclassstat")
    {
        value = atoll(_value.c_str());
        erase_class_state(value);
    }
    if (_table == "dbjobstat")
    {
        value = atoll(_value.c_str());
        erase_job_state(value);
    }
    // else
    // {
    //     eosio_assert(1 == 0, "Not Exist Table");
    // }
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
    equipment_db item_id_db_table(_self, _self.value);
    auto item_id_iter = item_id_db_table.find(_id);
    eosio_assert(item_id_iter != item_id_db_table.end(), "Not Exist Item ID 2");
    item_id_db_table.erase(item_id_iter);
}

void battletest::erase_all_item_id(uint64_t _id)
{
    allitem_db item_id_db_table(_self, _self.value);
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

void battletest::erase_servant_lv_status_list(uint64_t _type)
{
    servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
    auto servant_lv_status_db_iter = servant_lv_status_db_table.find(_type);
    eosio_assert(servant_lv_status_db_iter != servant_lv_status_db_table.end(), "Not exist servant_lv_status Data");
    servant_lv_status_db_table.modify(servant_lv_status_db_iter, _self, [&](auto &new_data) {
        new_data.change_status.clear();
    });
}

void battletest::erase_monster_lv_status(uint64_t _type)
{
    monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
    auto monster_lv_status_db_iter = monster_lv_status_db_table.find(_type);
    eosio_assert(monster_lv_status_db_iter != monster_lv_status_db_table.end(), "Not exist monster_lv_status Data");
    monster_lv_status_db_table.erase(monster_lv_status_db_iter);
}

void battletest::erase_monster_lv_status_list(uint64_t _type)
{
    monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
    auto monster_lv_status_db_iter = monster_lv_status_db_table.find(_type);
    eosio_assert(monster_lv_status_db_iter != monster_lv_status_db_table.end(), "Not exist monster_lv_status Data");
    monster_lv_status_db_table.modify(monster_lv_status_db_iter, _self, [&](auto &new_data) {
        new_data.change_status.clear();
    });
}

void battletest::erase_equipment_lv_status(uint64_t _type)
{
    equipment_lv_status_db equipment_lv_status_db_table(_self, _self.value);
    auto equipment_lv_status_db_iter = equipment_lv_status_db_table.find(_type);
    eosio_assert(equipment_lv_status_db_iter != equipment_lv_status_db_table.end(), "Not exist equipment_lv_status Data");
    equipment_lv_status_db_table.erase(equipment_lv_status_db_iter);
}

void battletest::erase_equipment_lv_status_list(uint64_t _type)
{
    equipment_lv_status_db equipment_lv_status_db_table(_self, _self.value);
    auto equipment_lv_status_db_iter = equipment_lv_status_db_table.find(_type);
    eosio_assert(equipment_lv_status_db_iter != equipment_lv_status_db_table.end(), "Not exist equipment_lv_status Data");
    equipment_lv_status_db_table.modify(equipment_lv_status_db_iter, _self, [&](auto &new_data)
    {
        new_data.change_status.clear();
    });
}

void battletest::erase_passive(uint64_t _id)
{
    passive_db passive_db_table(_self, _self.value);
    auto passive_db_iter = passive_db_table.find(_id);
    eosio_assert(passive_db_iter != passive_db_table.end(), "Not Exist Passive 1");
    passive_db_table.erase(passive_db_iter);
}

void battletest::erase_active(uint64_t _id)
{
    active_db active_db_table(_self, _self.value);
    auto active_db_iter = active_db_table.find(_id);
    eosio_assert(active_db_iter != active_db_table.end(), "Not Exist Active 1");
    active_db_table.erase(active_db_iter);
}

void battletest::erase_gacha_pool(uint64_t _id)
{
    main_gacha_db main_gacha_db_table(_self, _self.value);
    auto main_gacha_db_iter = main_gacha_db_table.find(_id);
    eosio_assert(main_gacha_db_iter != main_gacha_db_table.end(), "Not Exist Gacha 1");
    main_gacha_db_table.erase(main_gacha_db_iter);
}

void battletest::erase_stage_enemy(uint64_t _id)
{
    stage_enemy_db stage_enemy_db_table(_self, _self.value);
    auto enemy_iter = stage_enemy_db_table.find(_id);
    eosio_assert(enemy_iter != stage_enemy_db_table.end(), "Not Exist Enemy");
    stage_enemy_db_table.erase(enemy_iter);
}

void battletest::erase_stage(uint64_t _id)
{
    stage_db stage_db_table(_self, _self.value);
    auto stage_iter = stage_db_table.find(_id);
    eosio_assert(stage_iter != stage_db_table.end(), "Not Exist Stage 1");
    stage_db_table.erase(stage_iter);
}
void battletest::erase_stage_monster_list(uint64_t _id)
{
    stage_db stage_db_table(_self, _self.value);
    auto stage_iter = stage_db_table.find(_id);
    eosio_assert(stage_iter != stage_db_table.end(), "Not Exist Stage 1");
    stage_db_table.modify(stage_iter, _self, [&](auto &del_mon) {
        del_mon.monster_list.clear();
    });
}

void battletest::erase_stage_reward(uint64_t _id)
{
    reward_db reward_db_table(_self, _self.value);
    auto reward_iter = reward_db_table.find(_id);
    eosio_assert(reward_iter != reward_db_table.end(), "Not Exist Stage ID 4");
    reward_db_table.erase(reward_iter);
}

void battletest::erase_stage_reward_list(uint64_t _id)
{
    reward_db reward_db_table(_self, _self.value);
    auto reward_iter = reward_db_table.find(_id);
    eosio_assert(reward_iter != reward_db_table.end(), "Not Exist Stage ID 5");
    reward_db_table.modify(reward_iter, _self, [&](auto &new_data) {
        new_data.reward_list.clear();
    });
}

void battletest::erase_class_state(uint64_t _id)
{
    class_stat_db class_stat_db_table(_self, _self.value);
    auto class_iter = class_stat_db_table.find(_id);
    eosio_assert(class_iter != class_stat_db_table.end(), "Not Exist Class 1");
    class_stat_db_table.erase(class_iter);
}

void battletest::erase_job_state(uint64_t _id)
{
    job_stat_db class_stat_db_table(_self, _self.value);
    auto class_iter = class_stat_db_table.find(_id);
    eosio_assert(class_iter != class_stat_db_table.end(), "Not Exist job 1");
    class_stat_db_table.erase(class_iter);
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

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(pre_log_iter->user.value);

    if (user_log_iter == user_log_table.end())
    {
        user_log_table.emplace(_self, [&](auto &user_log) {
            user_log.user = pre_log_iter->user;
            user_log.servant_num = pre_log_iter->servant_num;
            user_log.monster_num = pre_log_iter->monster_num;
            user_log.equipment_num = pre_log_iter->item_num;
            user_log.gacha_num = pre_log_iter->gacha_num;
            user_log.item_num = 0;
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

        user_auths auth_user_table(_self, _self.value);
        auto auth_user_iter = auth_user_table.find(pre_user_iter->user.value);
        auto auth_user_iter2 = auth_user_table.find(pre_log_iter->user.value);
        eosio_assert(auth_user_iter == auth_user_table.end(), "Not exist auth_user");
        auth_user_table.emplace(_self, [&](auto &auth_user) {
            auth_user.user = pre_user_iter->user;
            auth_user.state = user_state::pre_regist;
            auth_user.exp = 0;
            auth_user.current_servant_inventory = 0;
            auth_user.current_monster_inventory = 0;
            auth_user.current_equipment_inventory = 0;
            auth_user.current_item_inventory = 0;
            auth_user.servant_inventory = 50;
            auth_user.monster_inventory = 50;
            auth_user.equipment_inventory = 50;
            auth_user.item_inventory = 50;
        });
        //pre_user_table.erase(_user.value);
    }

    user_preregist_servants user_preregist_servant_table(_self, _user.value);
    for (auto iter2 = user_preregist_servant_table.begin(); iter2 != user_preregist_servant_table.end();)
    {
        const auto &user_preregist_servant_iter = user_preregist_servant_table.get(iter2->primary_key(), "not exsit data");

        // 스탯 변경 참조 테이블 read
        servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
        uint64_t temp_grade = 5;
        auto servant_lv_status_db_iter = servant_lv_status_db_table.find(temp_grade);

        main_gacha_db main_gacha_db_table(_self, _self.value);
        auto main_gacha_db_iter = main_gacha_db_table.find(user_preregist_servant_iter.id);
        eosio_assert(main_gacha_db_iter != main_gacha_db_table.end(), "Not exist main_gacha_db_iter2");
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
            auto servant_db_iter = servant_db_table.find(main_gacha_db_iter->db_index);
            move_mail.mail_type = 1;
            move_mail.type_id = main_gacha_db_iter->db_index;
            move_mail.grade = 0;
            move_mail.status.basic_str = servant_lv_status_db_iter->change_status[user_preregist_servant_iter.status.basic_str].update_status;
            move_mail.status.basic_dex = servant_lv_status_db_iter->change_status[user_preregist_servant_iter.status.basic_dex].update_status;
            move_mail.status.basic_int = servant_lv_status_db_iter->change_status[user_preregist_servant_iter.status.basic_int].update_status;
        });
        iter2++;
    }
    //    for(auto iter2 = user_preregist_monster_table.begin(); iter2 != user_preregist_monster_table.end();)
    //    {
    //       //  user_preregist_servant_table.erase(_user.value);
    //        iter2++;
    //    }

    user_preregist_monsters user_preregist_monster_table(_self, _user.value);
    for (auto iter3 = user_preregist_monster_table.begin(); iter3 != user_preregist_monster_table.end();)
    {
        const auto &user_preregist_monster_iter = user_preregist_monster_table.get(iter3->primary_key(), "Not exsit data");

        // 스탯 변경 참조 테이블 read
        monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
        auto monster_lv_status_db_iter = monster_lv_status_db_table.find(user_preregist_monster_iter.grade);

        eosio_assert(monster_lv_status_db_iter != monster_lv_status_db_table.end(), "Not exist monster_lv_status Data");
        main_gacha_db main_gacha_db_table(_self, _self.value);
        auto main_gacha_db_iter = main_gacha_db_table.find(user_preregist_monster_iter.id);
        eosio_assert(main_gacha_db_iter != main_gacha_db_table.end(), "Not exist main_gacha_db_iter1");
        monster_db monster_db_table(_self, _self.value);
        auto monster_db_iter = monster_db_table.find(main_gacha_db_iter->db_index);
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
            monster_db monster_db_table(_self, _self.value);

            auto monster_id_db_iter = monster_db_table.find(user_preregist_monster_iter.id);
            move_mail.mail_type = 2;
            move_mail.type_id = main_gacha_db_iter->db_index;
            move_mail.grade = user_preregist_monster_iter.grade;
            move_mail.status.basic_str = monster_lv_status_db_iter->change_status[user_preregist_monster_iter.status.basic_str].update_status;
            move_mail.status.basic_dex = monster_lv_status_db_iter->change_status[user_preregist_monster_iter.status.basic_dex].update_status;
            move_mail.status.basic_int = monster_lv_status_db_iter->change_status[user_preregist_monster_iter.status.basic_int].update_status;
        });
        iter3++;
    }
    // for(auto iter3 = user_preregist_monster_table.begin(); iter3 != user_preregist_monster_table.end();)
    // {
    //     //user_preregist_monster_table.erase(_user.value);
    //     iter3++;
    // }

    user_preregist_items user_preregist_item_table(_self, _user.value);
    for (auto iter4 = user_preregist_item_table.begin(); iter4 != user_preregist_item_table.end();)
    {
        const auto &user_preregist_item_iter = user_preregist_item_table.get(iter4->primary_key(), "Not exsit data");

        equipment_lv_status_db equipment_lv_status_db_table(_self, _self.value);
        uint64_t _check_type = user_preregist_item_iter.type;
        uint64_t _check_grade = user_preregist_item_iter.grade;
        uint64_t _check_sum = _check_type * 10 + _check_grade;
        auto equipment_lv_status_db_iter = equipment_lv_status_db_table.find(_check_sum);
        eosio_assert(equipment_lv_status_db_iter != equipment_lv_status_db_table.end(), "Not exist equipment_lv_status Data");
        main_gacha_db main_gacha_db_table(_self, _self.value);
        auto main_gacha_db_iter = main_gacha_db_table.find(user_preregist_item_iter.id);
        eosio_assert(main_gacha_db_iter != main_gacha_db_table.end(), "Not exist main_gacha_db_iter3");

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
            //  allitem_db commonitem_db_table(_self, _self.value);
            //   auto common_item_db_iter = commonitem_db_table.find(user_preregist_item_iter.id);

            move_mail.type_id = main_gacha_db_iter->db_index;
            move_mail.mail_type = 3;
            // move_mail.job = user_preregist_item_iter.job;
            //  move_mail.tier = user_preregist_item_iter.tier;
            move_mail.grade = user_preregist_item_iter.grade;
            move_mail.status.basic_str = equipment_lv_status_db_iter->change_status[user_preregist_item_iter.main_status].update_status;
        });
        iter4++;
    }

    // // for(auto iter4 = user_preregist_item_table.begin(); iter4 != user_preregist_item_table.end();)
    // // {
    // //     //user_preregist_item_table.erase(_user.value);
    // //     iter4++;
    // // }
}

ACTION battletest::dbinit(std::string _table)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 4");

    // if (_table == "dbbody")
    // {
    //     body_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbhair")
    // {
    //     hair_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbhead")
    // {
    //     head_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbgender")
    // {
    //     gender_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbitemgrade")
    // {
    //     item_grade_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbmonstergd")
    // {
    //     monster_grade_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbservantjob")
    // {
    //     servant_job_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }

    // else if (_table == "dbgraderatio")
    // {
    //     grade_ratio_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbequipup")
    // {
    //     upgrade_equipment_ratio_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbmonsterup")
    // {
    //     upgrade_monster_ratio_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbitemup")
    // {
    //     upgrade_item_ratio_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dblevel")
    // {
    //     lv_exp my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbservantlv")
    // {
    //     servant_lv_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbmonsterlv")
    // {
    //     monster_lv_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbstatusserv")
    // {
    //     servant_lv_status_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbstatusmon")
    // {
    //     monster_lv_status_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbpassive")
    // {
    //     passive_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbactive")
    // {
    //     active_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbmonster")
    // {
    //     monster_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbservants")
    // {
    //     servant_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbequipment")
    // {
    //     equipment_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dballitem")
    // {
    //     allitem_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbservantid")
    // {
    //     servant_id_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbmonsterid")
    // {
    //     monster_id_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbitemid")
    // {
    //     item_id_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else if (_table == "dbgachapool")
    // {
    //     main_gacha_db my_table(_self, _self.value);
    //     for (auto iter = my_table.begin(); iter != my_table.end();)
    //     {
    //         auto erase_iter = my_table.find(iter->primary_key());
    //         iter++;
    //         my_table.erase(erase_iter);
    //     }
    // }
    // else
    // {
    //     eosio_assert(1 == 0, "Not Exist Table");
    // }
}

ACTION battletest::setdata(eosio::name _contract, std::string _table)
{
    require_auth(_self);
    // if(_table == "dbstatusequi")
    // {
    //     equipment_lv_status_db upgrade_item_ratio_db_table(_contract, _contract.value);
    //     equipment_lv_status_db my_table(_self, _self.value);
    //     for (auto iter = upgrade_item_ratio_db_table.begin(); iter != upgrade_item_ratio_db_table.end();)
    //     {
    //         const auto &data = upgrade_item_ratio_db_table.get(iter->primary_key(), "nost exist data");
    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.type_grade = data.type_grade;
    //             new_data.change_status = data.change_status;
    //         });
    //         iter++;
    //     }
    // }
    // if (_table == "dbbody")
    // {
    //     body_db body_table(_contract, _contract.value);
    //     for (auto iter = body_table.begin(); iter != body_table.end();)
    //     {
    //         const auto &data_iter = body_table.get(iter->primary_key(), "Not Exist Data");
    //         insert_body(data_iter.body);
    //         iter++;
    //     }
    // }
    // else if (_table == "dbhair")
    // {
    //     hair_db hair_table(_contract, _contract.value);
    //     for (auto iter = hair_table.begin(); iter != hair_table.end();)
    //     {
    //         const auto &data_iter = hair_table.get(iter->primary_key(), "Not Exist Data");
    //         insert_hair(data_iter.hair);
    //         iter++;
    //     }
    // }
    // else if (_table == "dbhead")
    // {
    //     head_db head_table(_contract, _contract.value);
    //     for (auto iter = head_table.begin(); iter != head_table.end();)
    //     {
    //         const auto &data_iter = head_table.get(iter->primary_key(), "Not Exist Data");
    //         insert_head(data_iter.head);
    //         iter++;
    //     }
    // }
    // else if (_table == "dbgender")
    // {
    //     gender_db gender_table(_contract, _contract.value);
    //     for (auto iter = gender_table.begin(); iter != gender_table.end();)
    //     {
    //         const auto &data_iter = gender_table.get(iter->primary_key(), "Not Exist Data");
    //         insert_gender(data_iter.gender);
    //         iter++;
    //     }
    // }
    // else if (_table == "dbitemgrade")
    // {
    //     item_grade_db item_grade_table(_contract, _contract.value);
    //     for (auto iter = item_grade_table.begin(); iter != item_grade_table.end();)
    //     {
    //         const auto &data_iter = item_grade_table.get(iter->primary_key(), "Not Exist Data");
    //         insert_item_grade(std::string("all"), data_iter.grade, 0, 10);
    //         iter++;
    //     }
    // }
    // else if (_table == "dbmonstergd")
    // {
    //     monster_grade_db monster_grade_table(_contract, _contract.value);
    //     for (auto iter = monster_grade_table.begin(); iter != monster_grade_table.end();)
    //     {
    //         const auto &data_iter = monster_grade_table.get(iter->primary_key(), "Not Exist Data");
    //         insert_monster_grade(std::string("all"), data_iter.grade, 0, 10);
    //         iter++;
    //     }
    // }
    // else if (_table == "dbservantjob")
    // {
    //     servant_job_db servant_job_table(_contract, _contract.value);
    //     for (auto iter = servant_job_table.begin(); iter != servant_job_table.end();)
    //     {
    //         const auto &data_iter = servant_job_table.get(iter->primary_key(), "Not Exist Data");
    //         insert_job(std::string("all"), data_iter.job, 0, 10);
    //         iter++;
    //     }
    // }
    // else if (_table == "dbgraderatio")
    // {
    //     grade_ratio_db other_table(_contract, _contract.value);
    //     grade_ratio_db my_table(_self, _self.value);
    //     for (auto iter = other_table.begin(); iter != other_table.end();)
    //     {
    //         const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");

    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.grade = get_iter.grade;
    //             new_data.ratio = get_iter.ratio;
    //         });
    //         iter++;
    //     }
    // }
    // if (_table == "dbburn")
    // {
    //     burnitem_db other_table(_contract, _contract.value);
    //      burnitem_db my_table(_self, _self.value);
    //              for (auto iter2 = other_table.begin(); iter2 != other_table.end();)
    //     {
    //         const auto &upgrade_monster_iter = other_table.get(iter2->primary_key(), "nost exist data");
    //         my_table.emplace(_self, [&](auto &new_data)
    //         {
    //             new_data.servant_job = upgrade_monster_iter.servant_job;
    //             new_data.result_item_id = upgrade_monster_iter.result_item_id;
    //         });
    //         iter2++;
    //     }
    // }
    // if (_table == "dbmonsterup")
    // {
    //     upgrade_monster_ratio_db upgrade_monster_ratio_db_table(_contract, _contract.value);
    //     upgrade_monster_ratio_db my_table(_self, _self.value);
    //     for (auto iter2 = upgrade_monster_ratio_db_table.begin(); iter2 != upgrade_monster_ratio_db_table.end();)
    //     {
    //         const auto &upgrade_monster_iter = upgrade_monster_ratio_db_table.get(iter2->primary_key(), "nost exist data");
    //         my_table.emplace(_self, [&](auto &new_data)
    //         {
    //             new_data.main_monster_grade_upgrade = upgrade_monster_iter.main_monster_grade_upgrade;
    //             new_data.sub = upgrade_monster_iter.sub;
    //         });
    //         iter2++;
    //     }
    // }
    // if (_table == "dbequipup")
    // {
    //     upgrade_equipment_ratio_dbs upgrade_item_ratio_db_table(_contract, _contract.value);
    //     upgrade_equipment_ratio_dbs my_table(_self, _self.value);
    //     for (auto iter = upgrade_item_ratio_db_table.begin(); iter != upgrade_item_ratio_db_table.end();)
    //     {
    //         const auto &data = upgrade_item_ratio_db_table.get(iter->primary_key(), "nost exist data");
    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.equipment_type_grade = data.equipment_type_grade;
    //             new_data.material_id = data.material_id;
    //             new_data.upgrade_ratio = data.upgrade_ratio;
    //             new_data.material_count= data.material_count;
    //             new_data.use_UTG = data.use_UTG;
    //         });
    //         iter++;
    //     }
    // }
    // if (_table == "dblevel")
    // {
    //     lv_exp other_lv_exp_table(_contract, _contract.value);
    //     lv_exp my_table(_self, _self.value);
    //     for (auto iter15 = other_lv_exp_table.begin(); iter15 != other_lv_exp_table.end();)
    //     {
    //         const auto &lv_exp_iter = other_lv_exp_table.get(iter15->primary_key(), "nost exist data");
    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.lv = lv_exp_iter.lv;
    //             new_data.rank_exp = lv_exp_iter.rank_exp;
    //             new_data.char_exp = lv_exp_iter.char_exp;
    //         });
    //         iter15++;
    //     }
    // }
    // if (_table == "dbservantlv")
    // {
    //     servant_lv_db servant_lv_db_table(_contract, _contract.value);
    //     servant_lv_db my_table(_self, _self.value);
    //     for (auto iter = servant_lv_db_table.begin(); iter != servant_lv_db_table.end();)
    //     {
    //         const auto &servant_lv_iter = servant_lv_db_table.get(iter->primary_key(), "not exist data ");

    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.job = servant_lv_iter.job;
    //             new_data.lvup_str = servant_lv_iter.lvup_str;
    //             new_data.lvup_dex = servant_lv_iter.lvup_dex;
    //             new_data.lvup_int = servant_lv_iter.lvup_int;
    //         });
    //         iter++;
    //     }
    // }
    // else if (_table == "dbmonsterlv")
    // {
    //     monster_lv_db monster_lv_db_table(_contract, _contract.value);
    //     monster_lv_db my_table(_self, _self.value);
    //     for (auto iter = monster_lv_db_table.begin(); iter != monster_lv_db_table.end();)
    //     {
    //         const auto &monster_lv_iter = monster_lv_db_table.get(iter->primary_key(), "not exist data ");

    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.monster_class_grade = monster_lv_iter.monster_class_grade;
    //             new_data.lvup_str = monster_lv_iter.lvup_str;
    //             new_data.lvup_dex = monster_lv_iter.lvup_dex;
    //             new_data.lvup_int = monster_lv_iter.lvup_int;
    //         });
    //         iter++;
    //     }
    // }
    // else if (_table == "dbstatusserv")
    // {
    //     servant_lv_status_db other_table(_contract, _contract.value);
    //     servant_lv_status_db my_table(_self, _self.value);
    //     for (auto iter = other_table.begin(); iter != other_table.end();)
    //     {
    //         const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");

    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.grade = get_iter.grade;
    //             new_data.change_status.insert(new_data.change_status.end(), get_iter.change_status.begin(), get_iter.change_status.end());
    //         });
    //         iter++;
    //     }
    // }
    // else if (_table == "dbstatusmon")
    // {
    //     monster_lv_status_db other_table(_contract, _contract.value);
    //     monster_lv_status_db my_table(_self, _self.value);
    //     for (auto iter = other_table.begin(); iter != other_table.end();)
    //     {
    //         const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");

    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.grade = get_iter.grade;
    //             new_data.change_status.insert(new_data.change_status.end(), get_iter.change_status.begin(), get_iter.change_status.end());
    //         });
    //         iter++;
    //     }
    // }
    //     else if (_table == "dbpassive")
    //     {
    //         passive_db other_table(_contract, _contract.value);
    //         passive_db my_table(_self, _self.value);
    //         for (auto iter = other_table.begin(); iter != other_table.end();)
    //         {
    //             const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");

    //             my_table.emplace(_self, [&](auto &new_data) {
    //                 new_data.passive_id = get_iter.passive_id;
    //                 new_data.enable_stack = get_iter.enable_stack;
    //                 new_data.max_stack = get_iter.max_stack;
    //                 new_data.effect_type = get_iter.effect_type;
    //                 new_data.effect_value = get_iter.effect_value;
    //                 new_data.effect_value_add = get_iter.effect_value_add;
    //                 new_data.target = get_iter.target;
    //                 new_data.role_target = get_iter.role_target;
    //             });
    //             iter++;
    //         }
    //     }
    //     else if (_table == "dbactive")
    //     {
    //         active_db other_table(_contract, _contract.value);
    //         active_db my_table(_self, _self.value);
    //         for (auto iter = other_table.begin(); iter != other_table.end();)
    //         {
    //             const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");

    //             my_table.emplace(_self, [&](auto &new_data) {
    //                 new_data.active_id = get_iter.active_id;
    //                 new_data.job = get_iter.job;
    //                 new_data.active_per = get_iter.active_per;
    //                 new_data.skill_type = get_iter.skill_type;
    //                 new_data.attack_type = get_iter.attack_type;
    //                 new_data.dmg_type = get_iter.dmg_type;
    //                 new_data.target = get_iter.target;
    //                 new_data.target_count = get_iter.target_count;
    //                 new_data.target_range = get_iter.target_range;
    //                 new_data.hit_count = get_iter.hit_count;
    //                 new_data.atk_per = get_iter.atk_per;
    //                 new_data.atk_per_add = get_iter.atk_per_add;
    //                 new_data.heal_per = get_iter.heal_per;
    //                 new_data.heal_per_add = get_iter.heal_per_add;
    //             });
    //             iter++;
    //         }
    //     }
    // if (_table == "dbmonster")
    // {
    //     mon_db monster_table(_contract, _contract.value);
    //     monster_db my_table(_self, _self.value);
    //     for (auto iter = monster_table.begin(); iter != monster_table.end();)
    //     {
    //         const auto &data_iter = monster_table.get(iter->primary_key(), "Not Exist Data");
    //         my_table.emplace(_self, [&](auto &new_monster)
    //         {
    //             new_monster.id = data_iter.monster_id;
    //             new_monster.tribe = data_iter.tribe;
    //             new_monster.type = data_iter.type;
    //             new_monster.monster_class = data_iter.monster_class;
    //         });
    //         iter++;
    //     }
    // }
    // if (_table == "dbservant")
    // {
    //     servant_db servant_table(_contract, _contract.value);
    //     servant_db my_table(_self, _self.value);
    //     for (auto iter = servant_table.begin(); iter != servant_table.end();)
    //     {
    //         const auto &data_iter = servant_table.get(iter->primary_key(), "Not Exist Data");
    //         my_table.emplace(_self, [&](auto &new_ser) {
    //             new_ser.id = data_iter.id;
    //             new_ser.body = data_iter.body;
    //             new_ser.head = data_iter.head;
    //             new_ser.gender = data_iter.gender;
    //             new_ser.head = data_iter.head;
    //             new_ser.hair = data_iter.hair;
    //             new_ser.job = data_iter.job;
    //             new_ser.grade = data_iter.grade;
    //         });
    //         iter++;
    //     }
    // }
    // if (_table == "dbitemshop")
    // {
    //     item_shop other_table(_contract, _contract.value);
    //     item_shop my_table(_self, _self.value);
    //     for (auto iter = other_table.begin(); iter != other_table.end();)
    //     {
    //         const auto &data_iter = other_table.get(iter->primary_key(), "Not Exist Data");
    //         my_table.emplace(_self, [&](auto &new_ser) {
    //             new_ser.id = data_iter.id;
    //             new_ser.goods_type = data_iter.goods_type;
    //             new_ser.goods_limited = data_iter.goods_limited;
    //             new_ser.goods_count = data_iter.goods_count;
    //             new_ser.price_type = data_iter.price_type;
    //             new_ser.price_count = data_iter.price_count;
    //         });
    //         iter++;
    //     }
    // }
    // if (_table == "dbequipment")
    // {
    //     equipment_db equip_item_table(_contract, _contract.value);
    //     equipment_db my_table(_self, _self.value);
    //     for (auto iter = equip_item_table.begin(); iter != equip_item_table.end();)
    //     {
    //         const auto &data_iter = equip_item_table.get(iter->primary_key(), "Not Exist Data");
    //         my_table.emplace(_self, [&](auto &new_a) {
    //             new_a.item_id = data_iter.item_id;
    //             new_a.set_id = data_iter.set_id;
    //             new_a.type = data_iter.type;
    //             new_a.tier = data_iter.tier;
    //             new_a.job = data_iter.job;

    //             new_a.option_list=data_iter.option_list;
    //             new_a.option_value_min_list=data_iter.option_value_min_list;
    //             new_a.option_value_max_list= data_iter.option_value_max_list;
    //             new_a.upgrade_option_value_list=data_iter.upgrade_option_value_list;
    //             new_a.random_option_id_list=data_iter.random_option_id_list;
    //             new_a.grade_multi_list = data_iter.grade_multi_list;

    //         });
    //         iter++;
    //     }
    // }
    // else if (_table == "dballitem")
    // {
    //     allitem_db common_item_table(_contract, _contract.value);
    //     allitem_db my_table(_self, _self.value);
    //     for (auto iter = common_item_table.begin(); iter != common_item_table.end();)
    //     {
    //         const auto &data_iter = common_item_table.get(iter->primary_key(), "Not Exist Data");
    //         my_table.emplace(_self, [&](auto &new_a) {
    //             new_a.id = data_iter.id;
    //             new_a.type = data_iter.type;
    //             new_a.item_param_list.insert(new_a.item_param_list.begin(), data_iter.item_param_list.begin(), data_iter.item_param_list.end());
    //             new_a.sell_id = data_iter.sell_id;
    //             new_a.sell_cost = data_iter.sell_cost;
    //         });
    //         iter++;
    //     }
    // }
    // else if (_table == "dbgachapool")
    // {
    //     main_gacha_db other_table(_contract, _contract.value);
    //     main_gacha_db my_table(_self, _self.value);
    //     for (auto iter = other_table.begin(); iter != other_table.end();)
    //     {
    //         const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.gacha_id = get_iter.gacha_id;
    //             new_data.db_index = get_iter.db_index;
    //         });
    //         iter++;
    //     }
    // }
    // else if (_table == "dbstageenemy")
    // {
    //     stage_enemy_db other_table(_contract, _contract.value);
    //     stage_enemy_db my_table(_self, _self.value);
    //     for (auto iter = other_table.begin(); iter != other_table.end();)
    //     {
    //         const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.id = get_iter.id;
    //             new_data.grade = get_iter.grade;
    //             new_data.enemy_class = get_iter.enemy_class;
    //             new_data.type = get_iter.type;
    //             new_data.enemy_str = get_iter.enemy_str;
    //             new_data.enemy_dex = get_iter.enemy_dex;
    //             new_data.enemy_int = get_iter.enemy_int;
    //             new_data.physical_cri_per = get_iter.physical_cri_per;
    //             new_data.physical_cri_dmg = get_iter.physical_cri_dmg;
    //             new_data.magic_cri_per = get_iter.magic_cri_per;
    //             new_data.magic_cri_dmg = get_iter.magic_cri_dmg;
    //             new_data.speed = get_iter.speed;
    //             new_data.avoid = get_iter.avoid;
    //             new_data.active_list = get_iter.active_list;
    //             new_data.passive_list = get_iter.passive_list;
    //         });
    //         iter++;
    //     }
    // }
    // else if (_table == "dbreward")
    // {
    //     reward_db other_table(_contract, _contract.value);
    //     reward_db my_table(_self, _self.value);
    //     for (auto iter = other_table.begin(); iter != other_table.end();)
    //     {
    //         const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.stage_id = get_iter.stage_id;
    //             new_data.rank_exp = get_iter.rank_exp;
    //             new_data.char_exp = get_iter.char_exp;
    //             new_data.first_reward_type = get_iter.first_reward_type;
    //             new_data.first_reward_id = get_iter.first_reward_id;
    //             new_data.first_reward_grade = get_iter.first_reward_grade;
    //             new_data.first_reward_count = get_iter.first_reward_count;
    //             new_data.reward_utg_min = get_iter.reward_utg_min;
    //             new_data.reward_utg_max = get_iter.reward_utg_max;
    //             new_data.reward_list.insert(new_data.reward_list.begin(), get_iter.reward_list.begin(), get_iter.reward_list.end());
    //         });
    //         iter++;
    //     }
    // }
    // else
    // {
    //     eosio_assert(1 == 0, "Not Exist Table");
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

void battletest::signup(eosio::name _user)
{
    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter == user_log_table.end(), "Signup : Already Log Table");

    user_log_table.emplace(_self, [&](auto &new_log) {
        new_log.user = _user;
        new_log.servant_num = 0;
        new_log.monster_num = 0;
        new_log.equipment_num = 0;
        new_log.gacha_num = 0;
        new_log.item_num = 0;
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

    user_auths auth_user_table(_self, _self.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "Signup : Already Auth Table");

    auth_user_table.emplace(_self, [&](auto &new_user) {
        new_user.user = _user;
        new_user.state = user_state::auth_regist;
        new_user.exp = 0;
        new_user.current_servant_inventory = 0;
        new_user.current_monster_inventory = 0;
        new_user.current_equipment_inventory = 0;
        new_user.current_item_inventory = 0;
        new_user.servant_inventory = 50;
        new_user.monster_inventory = 50;
        new_user.equipment_inventory = 50;
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
        new_party.servant_list.resize(5);
        new_party.monster_list.resize(5);
    });
}

/////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////mail system////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////

ACTION battletest::nftmail(eosio::name _user, std::string _type, uint64_t _index)
{
    require_auth(NFT_CONTRACT);

    //  메일 테이블에 변경된 스탯으로 저장
    mail_db mail_db_table(_self, _user.value);
    mail_db_table.emplace(_self, [&](auto &move_mail) {
        uint64_t change_type;
        uint32_t first_index = mail_db_table.available_primary_key();
        if (first_index == 0)
        {
            move_mail.mail_index = 1;
        }
        else
        {
            move_mail.mail_index = mail_db_table.available_primary_key();
        }
        if (_type == "servant")
        {
            change_type = 5;
        }
        else if (_type == "monster")
        {
            change_type = 6;
        }
        else if (_type == "item")
        {
            change_type = 7;
        }
        move_mail.mail_type = change_type;
        move_mail.type_id = _index;
        move_mail.grade = 0;
    });
}

ACTION battletest::mailopen(eosio::name _user, const std::vector<uint64_t> &_mail_index)
{

    require_auth(_user);

    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "mailopen : black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "mailopen : Server Pause");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);

    mail_db mail_db_table(_self, _user.value);
    main_gacha_db main_gacha_db_table(_self, _self.value);

    for (uint32_t i = 0; i < _mail_index.size(); ++i)
    {
        auto mail_db_iter = mail_db_table.find(_mail_index[i]);

        eosio_assert(mail_db_iter != mail_db_table.end(), "mailopen : Not exist mail data");

        eosio_assert((mail_db_iter->mail_type == 1 || mail_db_iter->mail_type == 2 | mail_db_iter->mail_type == 3 || mail_db_iter->mail_type == 4 || mail_db_iter->mail_type == 5 || mail_db_iter->mail_type == 6 || mail_db_iter->mail_type == 7), "Not exist select type");

        if (mail_db_iter->mail_type == 1) //서번트
        {
            eosio_assert(check_inventory(_user) == true, "mailopen : your inventory is full");
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
                auto servant_db_iter = servant_db_table.find(mail_db_iter->type_id);
                eosio_assert(servant_db_iter != servant_db_table.end(), "mailopen : Not exist servant_db_iter1");

                // 스탯 변경 참조 테이블 read
                servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
                auto servant_lv_status_db_iter = servant_lv_status_db_table.find(5); //아직은 서번트 등급은 0이지만 5로 표현
                // auto servant_lv_status_db_iter = servant_lv_status_db_table.find(mail_db_iter->grade);
                move_servant.party_number = 0;
                move_servant.servant.state = object_state::on_inventory;
                move_servant.servant.exp = 0; //servant_db_iter->exp;
                move_servant.servant.id = servant_db_iter->id;
                move_servant.servant.grade = mail_db_iter->grade;
                move_servant.servant.status.basic_str = mail_db_iter->status.basic_str;
                move_servant.servant.status.basic_dex = mail_db_iter->status.basic_dex;
                move_servant.servant.status.basic_int = mail_db_iter->status.basic_int;
                move_servant.servant.equip_slot.resize(3);

                uint64_t _seed = safeseed::get_seed_value(_user.value, now() + i);

                uint32_t random_active = safeseed::get_random_value(_seed, SERVANT_JOB_COUNT, DEFAULT_MIN_DB, servant_random_count);
                uint32_t active_id = get_servant_active_skill(random_active, _seed);
                if (active_id != 0)
                {
                    move_servant.servant.active_skill.push_back(active_id);
                }

                uint32_t random_passive = safeseed::get_random_value(_seed, SERVANT_JOB_COUNT, DEFAULT_MIN_DB, servant_random_count);
                uint32_t passive_id = get_servant_passive_skill(random_passive, _seed);
                if (passive_id != 0)
                {
                    move_servant.servant.passive_skill.push_back(passive_id);
                }
            });

            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_servant_inventory += 1;
            });
            // for (auto iter4 = mail_db_table.begin(); iter4 != mail_db_table.end();)
            // {
            //     iter4++;
            // }
        }
        else if (mail_db_iter->mail_type == 2) //몬스터
        {
            eosio_assert(check_inventory(_user) == true, "mailopen : your inventory is full");
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
                auto monster_db_iter = monster_db_table.find(mail_db_iter->type_id);
                eosio_assert(monster_db_iter != monster_db_table.end(), "mailopen : Not exist monster_db_iter2");
                move_monster.monster.id = monster_db_iter->id;
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

                uint64_t _seed = safeseed::get_seed_value(_user.value, now() + i);

                uint32_t active_id = get_monster_active_skill(monster_db_iter->monster_class, _seed);
                if (active_id != 0)
                {
                    move_monster.monster.active_skill.push_back(active_id);
                }

                uint32_t passive_id = get_monster_passive_skill(monster_db_iter->monster_class, _seed);
                if (passive_id != 0)
                {
                    move_monster.monster.passive_skill.push_back(passive_id);
                }
            });
            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_monster_inventory += 1;
            });
        }
        else if (mail_db_iter->mail_type == 3) //아이템
        {
            eosio_assert(check_inventory(_user) == true, "mailopen : your inventory is full");
            user_equip_items user_equipment_table(_self, _user.value);
            user_equipment_table.emplace(_self, [&](auto &move_item) {
                uint32_t first_index = user_equipment_table.available_primary_key();
                if (first_index == 0)
                {
                    move_item.index = 1;
                }
                else
                {
                    move_item.index = user_equipment_table.available_primary_key();
                }
                equipment_db equipitem_db_table(_self, _self.value);
                auto equipitem_db_iter = equipitem_db_table.find(mail_db_iter->type_id);
                eosio_assert(equipitem_db_iter != equipitem_db_table.end(), "mailopen : Not exist equipment_db_iter3");
                allitem_db commonitem_db_table(_self, _self.value);
                auto commonitem_db_iter = commonitem_db_table.find(mail_db_iter->type_id);
                eosio_assert(commonitem_db_iter != commonitem_db_table.end(), "mailopen : Not exist commonitem_db_iter 3");

                move_item.equipment.id = commonitem_db_iter->id;
                move_item.equipment.state = object_state::on_inventory;
                move_item.equipment.type = commonitem_db_iter->type;
                move_item.equipment.tier = equipitem_db_iter->tier;
                move_item.equipment.job = equipitem_db_iter->job;
                move_item.equipment.grade = mail_db_iter->grade;
                move_item.equipment.upgrade = 0;
                move_item.equipment.value = mail_db_iter->status.basic_str;
            });
            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_equipment_inventory += 1;
            });
        }

        else if (mail_db_iter->mail_type == 4) //UTG 및 재화
        {
            //아직 미구현
        }
        else if (mail_db_iter->mail_type == 5) //UTS
        {
            std::string temp = "UTS";
            eosio_assert(check_inventory(_user) == true, "mailopen : your inventory is full");
            uts_db uts_db_table(NFT_CONTRACT, NFT_CONTRACT.value);
            auto uts_db_iter = uts_db_table.find(mail_db_iter->type_id);
            eosio_assert(uts_db_iter != uts_db_table.end(), "mailopen : end table error");
            std::string temp_type = "servant";
            nftexchange(uts_db_iter->owner, uts_db_iter->master, temp_type, uts_db_iter->t_idx);
            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_servant_inventory += 1;
            });
            action(permission_level{get_self(), "active"_n},
                   NFT_CONTRACT, "deletedata"_n,
                   std::make_tuple(temp, uts_db_iter->idx))
                .send();
        }

        else if (mail_db_iter->mail_type == 6) //UTM
        {
            std::string temp = "UTM";
            eosio_assert(check_inventory(_user) == true, "mailopen : your inventory is full");
            utm_db utm_db_table(NFT_CONTRACT, NFT_CONTRACT.value);
            auto utm_db_iter = utm_db_table.find(mail_db_iter->type_id);
            eosio_assert(utm_db_iter != utm_db_table.end(), "mailopen : end table error");
            std::string temp_type = "monster";
            nftexchange(utm_db_iter->owner, utm_db_iter->master, temp_type, utm_db_iter->t_idx);
            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_monster_inventory += 1;
            });
            action(permission_level{get_self(), "active"_n},
                   NFT_CONTRACT, "deletedata"_n,
                   std::make_tuple(temp, utm_db_iter->idx))
                .send();
        }
        else if (mail_db_iter->mail_type == 7) //UTI
        {
            std::string temp = "UTI";
            eosio_assert(check_inventory(_user) == true, "mailopen : your inventory is full");
            uti_db uti_db_table(NFT_CONTRACT, NFT_CONTRACT.value);
            auto uti_db_iter = uti_db_table.find(mail_db_iter->type_id);
             eosio_assert(uti_db_iter != uti_db_table.end(), "mailopen : end table error");
            std::string temp_type = "item";
            nftexchange(uti_db_iter->owner, uti_db_iter->master, temp_type, uti_db_iter->t_idx);

            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_equipment_inventory += 1;
            });
            action(permission_level{get_self(), "active"_n},
                   NFT_CONTRACT, "deletedata"_n,
                   std::make_tuple(temp, uti_db_iter->idx))
                .send();
        }
        mail_db_table.erase(mail_db_iter);
    }
}

// eosio.token recipient
// memo description spec
//-------------------------------------------------------------------------

ACTION battletest::eostransfer(eosio::name sender, eosio::name receiver)
{
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(sender.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "Eos Transfer : BlackList User");

    eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
        eosio_assert(ad.action.size() != 0, "Eos Transfer : Wrong Action");
        if (ad.action == action_signup)
        {
            signup(sender);
        }
        else if (ad.action == action_gacha)
        {
            start_gacha(sender, ad.type);
        }
        else if (ad.action == "cheat")
        {
            testcheat(sender);
        }
        else if (ad.action == action_inventory)
        {
            buy_inventory(sender, ad.type);
        }
    });
}

template <typename T>
void battletest::eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func)
{
    require_auth(sender);
    auto transfer_data = eosio::unpack_action_data<st_transfer>();
    eosio_assert(transfer_data.to == receiver, "Eos Transfer : Reciver Miss Match"); //to 랑 receiver 가 같을 경우만 <- receiver 조작에 대한 예외 처리
    eosio_assert(transfer_data.from == sender, "Eos Transfer : Sender Miss Match");  //from 이랑 sender 가 같을 경우만 <- sender 조작에 대한 예외처리
    eosio_assert(transfer_data.quantity.symbol == symbol("EOS", 4), "Eos Transfer : Only Accepts EOS");
    eosio_assert(transfer_data.quantity.is_valid(), "Eos Transfer : Invalid Token Transfer");
    eosio_assert(transfer_data.quantity.amount > 0, "Eos Transfer : Quantity Must Be Positive");

    transfer_action res;
    size_t l_center = transfer_data.memo.find(':');

    res.action = transfer_data.memo.substr(0, l_center);
    if (transfer_data.from == _self)        //컨트랙트에서 돈이 나갈 경우
    {
        if (transfer_data.to != "eosio.ram"_n &&
        transfer_data.to != "eosio.ramfee"_n &&
        transfer_data.to != "eosio.stake"_n) //eosio.token 에게 보내는것도 아니면 받는사람이 마스터인지 확인한다
        {
            system_master system_master_table(_self, _self.value);
            auto system_master_iter = system_master_table.find(transfer_data.to.value);
            eosio_assert(system_master_iter != system_master_table.end(), "Eos Transfer : Impossible Send EOS");
        }
    }
    else        //다른 사람이 보낼 경우
    {
        system_master system_master_table(_self, _self.value);
        auto system_master_iter = system_master_table.find(transfer_data.from.value);
        if (system_master_iter == system_master_table.end())    //보낸 계정이 마스터 계정이 아닌경우
        {
            if (res.action == "gacha")
            {
                system_master system_master_table(_self, _self.value);
                auto system_master_iter = system_master_table.begin();
                eosio_assert(system_master_iter->state != system_state::pause, "Eos Transfer Gacha : Server Pause");

                size_t l_next = transfer_data.memo.find(':', l_center + 1);
                size_t l_end = transfer_data.memo.length() - (l_next + 1);

                eosio_assert(transfer_data.memo.find(':') != std::string::npos, "Eos Transfer Gacha : Seed Memo [:] Error");
                eosio_assert(transfer_data.memo.find(':', l_center + 1) != std::string::npos, "Eos Transfer Gacha : Seed Memo [:] Error");
                eosio_assert(transfer_data.quantity.amount == 10000, "Eos Transfer Gacha : Gacha need 1.0000 EOS");

                std::string l_seed = transfer_data.memo.substr(l_center + 1, (l_next - l_center - 1));
                std::string l_sha = transfer_data.memo.substr(l_next + 1, l_end);

                res.type = safeseed::check_seed(l_seed, l_sha);

                eosio_assert(res.type != 0, "Eos Transfer Gacha : Wrong Seed Convert");
            }
            else if (res.action == "signup")
            {
                system_master system_master_table(_self, _self.value);
                auto system_master_iter = system_master_table.begin();
                eosio_assert(system_master_iter->state != system_state::pause, "Eos Transfer Signup : Server Pause");

                eosio_assert(transfer_data.quantity.amount == 10000, "Eos Transfer Signup : Signup Need 1.0000 EOS");
            }
            else if (res.action == "cheat") //test
            {
                system_master system_master_table(_self, _self.value);
                auto system_master_iter = system_master_table.begin();
                eosio_assert(system_master_iter->state != system_state::pause, "Eos Transfer Cheat : Server Pause");

                whitelist whitelist_table(_self, _self.value);
                auto whitelist_iter = whitelist_table.find(sender.value);
                eosio_assert(whitelist_iter != whitelist_table.end(), "Eos Transfer Cheat : Empty White User / Not Yet White User");
            }
            else if (res.action == "buyinventory") //인벤토리 구매
            {
                system_master system_master_table(_self, _self.value);
                auto system_master_iter = system_master_table.begin();
                eosio_assert(system_master_iter->state != system_state::pause, "Eos Transfer buyinventory : Server Pause");

                eosio_assert(transfer_data.memo.find(':') != std::string::npos, "Eos Transfer buyinventory : Seed Memo [:] Error");
                eosio_assert(transfer_data.quantity.amount == 5000, "Eos Transfer buyinventory : buyinventory need 0.5000 EOS");

                size_t l_end = transfer_data.memo.length() - (l_center + 1);
                std::string result = transfer_data.memo.substr(l_center + 1, l_end);

                uint64_t result_seed = atoll(result.c_str());

                res.type = result_seed;

                eosio_assert(res.type != 0, "Wrong seed convert");
            }
            else
            {
                eosio_assert(1 == 0, "Wrong Action 4");
            }
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

    eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 9");

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
uint32_t battletest::change_servant_statue(uint32_t _status_grade)
{
    servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
    auto status_iter = servant_lv_status_db_table.find(5); //서번트는 등급 고정
    int a = 0;
    for (uint32_t i = 0; i < status_iter->change_status.size(); ++i)
    {
        if (status_iter->change_status[i].pre_status == _status_grade)
        {
            a = 1;
            return status_iter->change_status[i].update_status;
        }
    }
    eosio_assert(a != 0, "Not Exist Status Servant 1");
    return 0;
}

uint32_t battletest::get_monster_passive_skill(uint32_t _monster_class, uint32_t _seed)
{
    uint32_t passive_id = 0;
    class_stat_db class_stat_db_table(_self, _self.value);
    auto class_stat_db_iter = class_stat_db_table.find(_monster_class);
    eosio_assert(class_stat_db_iter != class_stat_db_table.end(), "Monster Passive : Empty Class Stat Table / Not Set Class");

    passive_db passive_db_table(_self, _self.value);
    if (class_stat_db_iter->passive_private_per == 0) //전용 패시브 확률이 0인데
    {
        if (class_stat_db_iter->passive_public_list.size() == 0) //공용 패시브 리스트가 존재 하지 않으면
        {
            return passive_id;
        }
        uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->passive_public_list.size(), 0, 1);
        auto passive_db_iter = passive_db_table.find(class_stat_db_iter->passive_public_list[random_skill_index]);
        eosio_assert(passive_db_iter != passive_db_table.end(), "Monster Passive : Empty Public Active ID / Not Set Public Passive ID");
        passive_id = passive_db_iter->passive_id;
        return passive_id;
    }
    else //전용 패시브 확률이 0이 아닌데
    {
        if (class_stat_db_iter->passive_private_list.size() == 0) //전용 패시브 리스트가 존재 하지 않지만
        {
            if (class_stat_db_iter->passive_public_list.size() != 0) //공용 패시브 리스트가 존재하면
            {
                uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->passive_public_list.size(), 0, 1);
                auto passive_db_iter = passive_db_table.find(class_stat_db_iter->passive_public_list[random_skill_index]);
                eosio_assert(passive_db_iter != passive_db_table.end(), "Monster Passive : Empty Public Passive ID / Not Set Public Passive ID");
                passive_id = passive_db_iter->passive_id;
                return passive_id;
            }
            return passive_id;
        }
        else
        {
            uint32_t random_private_per = safeseed::get_random_value(_seed, 100, 0, 0);
            if (class_stat_db_iter->passive_private_per >= random_private_per)
            {
                uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->passive_private_list.size(), 0, 1);
                auto passive_db_iter = passive_db_table.find(class_stat_db_iter->passive_private_list[random_skill_index]);
                eosio_assert(passive_db_iter != passive_db_table.end(), "Monster Passive : Empty Private Passive ID / Not Set Private Passive ID");
                passive_id = passive_db_iter->passive_id;
                return passive_id;
            }
            else
            {
                uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->passive_public_list.size(), 0, 1);
                auto passive_db_iter = passive_db_table.find(class_stat_db_iter->passive_public_list[random_skill_index]);
                eosio_assert(passive_db_iter != passive_db_table.end(), "Monster Passive : Empty Public Passive ID / Not Set Public Passive ID");
                passive_id = passive_db_iter->passive_id;
                return passive_id;
            }
        }
    }
}

uint32_t battletest::get_monster_active_skill(uint32_t _monster_class, uint32_t _seed)
{
    uint32_t active_id = 0;
    class_stat_db class_stat_db_table(_self, _self.value);
    auto class_stat_db_iter = class_stat_db_table.find(_monster_class);
    eosio_assert(class_stat_db_iter != class_stat_db_table.end(), "Monster Actvie : Empty Class Stat Table / Not Set Class");

    active_db active_db_table(_self, _self.value);
    if (class_stat_db_iter->active_private_per == 0) //전용 액티브 확률이 0인데
    {
        if (class_stat_db_iter->active_public_list.size() == 0)
        {
            return active_id;
        }
        uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->active_public_list.size(), 0, 1);
        auto actvie_db_iter = active_db_table.find(class_stat_db_iter->active_public_list[random_skill_index]);
        eosio_assert(actvie_db_iter != active_db_table.end(), "Monster Active : Empty Public Active ID / Not Set Public Active ID");
        active_id = actvie_db_iter->active_id;
        return active_id;
    }
    else
    {
        if (class_stat_db_iter->active_private_list.size() == 0)
        {
            if (class_stat_db_iter->active_public_list.size() != 0)
            {
                uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->active_public_list.size(), 0, 1);
                auto actvie_db_iter = active_db_table.find(class_stat_db_iter->active_public_list[random_skill_index]);
                eosio_assert(actvie_db_iter != active_db_table.end(), "Monster Active : Empty Public Active ID / Not Set Public Active ID");
                active_id = actvie_db_iter->active_id;
                return active_id;
            }
            return active_id;
        }
        uint32_t random_private_per = safeseed::get_random_value(_seed, 100, 0, 0);
        if (class_stat_db_iter->active_private_per >= random_private_per)
        {
            uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->active_private_list.size(), 0, 1);
            auto actvie_db_iter = active_db_table.find(class_stat_db_iter->active_private_list[random_skill_index]);
            eosio_assert(actvie_db_iter != active_db_table.end(), "Monster Private : Empty Public Active ID / Not Set Private Active ID");
            active_id = actvie_db_iter->active_id;
            return active_id;
        }
        else
        {
            uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->active_public_list.size(), 0, 1);
            auto actvie_db_iter = active_db_table.find(class_stat_db_iter->active_public_list[random_skill_index]);
            eosio_assert(actvie_db_iter != active_db_table.end(), "Monster Active : Empty Public Active ID / Not Set Public Active ID");
            active_id = actvie_db_iter->active_id;
            return active_id;
        }
    }
}

uint32_t battletest::get_servant_passive_skill(uint32_t _job, uint32_t _seed)
{
    uint32_t passive_id = 0;
    job_stat_db class_stat_db_table(_self, _self.value);
    auto class_stat_db_iter = class_stat_db_table.find(_job);
    eosio_assert(class_stat_db_iter != class_stat_db_table.end(), "Servant Passive : Empty Job Stat Table / Not Set Job");

    passive_db passive_db_table(_self, _self.value);
    if (class_stat_db_iter->passive_private_per == 0) //전용 패시브 확률이 0인데
    {
        if (class_stat_db_iter->passive_public_list.size() == 0) //공용 패시브 리스트가 존재 하지 않으면
        {
            return passive_id;
        }
        uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->passive_public_list.size(), 0, 1);
        auto passive_db_iter = passive_db_table.find(class_stat_db_iter->passive_public_list[random_skill_index]);
        eosio_assert(passive_db_iter != passive_db_table.end(), "Servant Passive : Empty Public Passive ID / Not Set Public Passive ID");
        passive_id = passive_db_iter->passive_id;
        return passive_id;
    }
    else //전용 패시브 확률이 0이 아닌데
    {
        if (class_stat_db_iter->passive_private_list.size() == 0) //전용 패시브 리스트가 존재 하지 않지만
        {
            if (class_stat_db_iter->passive_public_list.size() != 0) //공용 패시브 리스트가 존재하면
            {
                uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->passive_public_list.size(), 0, 1);
                auto passive_db_iter = passive_db_table.find(class_stat_db_iter->passive_public_list[random_skill_index]);
                eosio_assert(passive_db_iter != passive_db_table.end(), "Servant Passive : Empty Public Passive ID / Not Set Public Passive ID");
                passive_id = passive_db_iter->passive_id;
                return passive_id;
            }
            return passive_id;
        }
        else
        {
            uint32_t random_private_per = safeseed::get_random_value(_seed, 100, 0, 0);
            if (class_stat_db_iter->passive_private_per >= random_private_per)
            {
                uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->passive_private_list.size(), 0, 1);
                auto passive_db_iter = passive_db_table.find(class_stat_db_iter->passive_private_list[random_skill_index]);
                eosio_assert(passive_db_iter != passive_db_table.end(), "Servant Passive : Empty Private Passive ID / Not Set Private Passive ID");
                passive_id = passive_db_iter->passive_id;
                return passive_id;
            }
            else
            {
                uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->passive_public_list.size(), 0, 1);
                auto passive_db_iter = passive_db_table.find(class_stat_db_iter->passive_public_list[random_skill_index]);
                eosio_assert(passive_db_iter != passive_db_table.end(), "Servant Passive : Empty Public Passive ID / Not Set Public Passive ID");
                passive_id = passive_db_iter->passive_id;
                return passive_id;
            }
        }
    }
}

uint32_t battletest::get_servant_active_skill(uint32_t _job, uint32_t _seed)
{
    uint32_t active_id = 0;
    job_stat_db class_stat_db_table(_self, _self.value);
    auto class_stat_db_iter = class_stat_db_table.find(_job);
     eosio_assert(class_stat_db_iter != class_stat_db_table.end(), "Servant Active : Empty Job Stat Table / Not Set Job");

    active_db active_db_table(_self, _self.value);
    if (class_stat_db_iter->active_private_per == 0) //전용 액티브 확률이 0인데
    {
        if (class_stat_db_iter->active_public_list.size() == 0)
        {
            return active_id;
        }
        uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->active_public_list.size(), 0, 1);
        auto actvie_db_iter = active_db_table.find(class_stat_db_iter->active_public_list[random_skill_index]);
        eosio_assert(actvie_db_iter != active_db_table.end(), "Servant Active : Empty Public Active ID / Not Set Public Active ID");
        active_id = actvie_db_iter->active_id;
        return active_id;
    }
    else
    {
        if (class_stat_db_iter->active_private_list.size() == 0)
        {
            if (class_stat_db_iter->active_public_list.size() != 0)
            {
                uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->active_public_list.size(), 0, 1);
                auto actvie_db_iter = active_db_table.find(class_stat_db_iter->active_public_list[random_skill_index]);
                eosio_assert(actvie_db_iter != active_db_table.end(), "Servant Active : Empty Public Active ID / Not Set Public Active ID");
                active_id = actvie_db_iter->active_id;
                return active_id;
            }
            return active_id;
        }
        uint32_t random_private_per = safeseed::get_random_value(_seed, 100, 0, 0);
        if (class_stat_db_iter->active_private_per >= random_private_per)
        {
            uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->active_private_list.size(), 0, 1);
            auto actvie_db_iter = active_db_table.find(class_stat_db_iter->active_private_list[random_skill_index]);
            eosio_assert(actvie_db_iter != active_db_table.end(), "Servant Active : Empty Private Active ID / Not Set Private Active ID");
            active_id = actvie_db_iter->active_id;
            return active_id;
        }
        else
        {
            uint32_t random_skill_index = safeseed::get_random_value(_seed, class_stat_db_iter->active_public_list.size(), 0, 1);
            auto actvie_db_iter = active_db_table.find(class_stat_db_iter->active_public_list[random_skill_index]);
            eosio_assert(actvie_db_iter != active_db_table.end(), "Servant Active : Empty Public Active ID / Not Set Public Active ID");
            active_id = actvie_db_iter->active_id;
            return active_id;
        }
    }
}

void battletest::gacha_servant_id(eosio::name _user, uint64_t _seed)
{
    servant_job_db servant_job_table(_self, _self.value);
    uint32_t random_job = safeseed::get_random_value(_seed, SERVANT_JOB_COUNT, DEFAULT_MIN_DB, servant_random_count);
    const auto &servant_job_db_iter = servant_job_table.get(random_job, "Gacha Servant : Empty Servant Job");

    servant_random_count += 1;
    uint32_t random_body = gacha_servant_body(_seed, servant_random_count);

    servant_random_count += 1;
    gender_db gender_table(_self, _self.value);
    uint32_t random_gender = safeseed::get_random_value(_seed, GEMDER_COUNT, DEFAULT_MIN_DB, servant_random_count);
    const auto &gender_db_iter = gender_table.get(random_gender, "Gacha Servant : Empty Servant Gender");

    servant_random_count += 1;
    uint32_t random_head = gacha_servant_head(_seed, servant_random_count);

    servant_random_count += 1;
    uint32_t random_hair = gacha_servant_hair(_seed, servant_random_count);

    servant_db servant_id_table(_self, _self.value);
    uint32_t servant_index = get_servant_index(random_job, random_body, random_gender, random_head, random_hair);
    const auto &servant_id_db_iter = servant_id_table.get(servant_index, "Gacha Servant : Empty Servant ID");
   

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);

    user_auths auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(_user.value);
    eosio_assert(auth_user_iter != auth_user_table.end(), "Gacha Servant : Empty Auth Table / Not Yet Signup");

    eosio_assert(user_log_iter != user_log_table.end(), "Gacha Servant : Empty Log Table / Not Yet Signup");

    std::string new_data;
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

        new_servant.id = servant_id_db_iter.id;
        new_servant.exp = 0;
        new_servant.grade = 5;
        servant_random_count += 1;

        new_servant.status.basic_str = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_str, servant_job_db_iter.min_range.base_str, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_dex = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_dex, servant_job_db_iter.min_range.base_dex, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_int = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_int, servant_job_db_iter.min_range.base_int, servant_random_count);

        new_servant.status.basic_str = change_servant_statue(new_servant.status.basic_str);
        new_servant.status.basic_dex = change_servant_statue(new_servant.status.basic_dex);
        new_servant.status.basic_int = change_servant_statue(new_servant.status.basic_int);

        new_servant.equip_slot.resize(3);
        new_servant.state = object_state::on_inventory;

        uint32_t active_id = get_servant_active_skill(servant_id_db_iter.job, _seed);
        if (active_id != 0)
        {
            new_servant.active_skill.push_back(active_id);
        }

        uint32_t passive_id = get_servant_passive_skill(servant_id_db_iter.job, _seed);
        if (passive_id != 0)
        {
            new_servant.passive_skill.push_back(passive_id);
        }

        result.index = update_user_servant_list.index;
        result.type = result::servant;

        update_user_servant_list.party_number = EMPTY_PARTY;
        update_user_servant_list.servant = new_servant;

        new_data += to_string(update_user_servant_list.index) + ":";
        new_data += to_string(update_user_servant_list.party_number) + ":";
        new_data += to_string(update_user_servant_list.servant.state) + ":";
        new_data += to_string(update_user_servant_list.servant.exp) + ":";
        new_data += to_string(update_user_servant_list.servant.id) + ":";
        new_data += to_string(update_user_servant_list.servant.level) + ":";
        new_data += to_string(update_user_servant_list.servant.grade) + ":";
        new_data += to_string(update_user_servant_list.servant.status.basic_str) + ":";
        new_data += to_string(update_user_servant_list.servant.status.basic_dex) + ":";
        new_data += to_string(update_user_servant_list.servant.status.basic_int) + ":";
        new_data += "[";
        new_data += to_string(update_user_servant_list.servant.equip_slot[0]) + ":";
        new_data += to_string(update_user_servant_list.servant.equip_slot[1]) + ":";
        new_data += to_string(update_user_servant_list.servant.equip_slot[2]);
        new_data += "]:";
        new_data += "[";
        for (uint32_t i = 0; i < update_user_servant_list.servant.passive_skill.size(); ++i)
        {
            if (i + 1 == update_user_servant_list.servant.passive_skill.size())
            {
                new_data += to_string(update_user_servant_list.servant.passive_skill[i]);
            }
            else
            {
                new_data += to_string(update_user_servant_list.servant.passive_skill[i]) + ":";
            }
        }
        new_data += "]:";
        new_data += "[";
        for (uint32_t i = 0; i < update_user_servant_list.servant.active_skill.size(); ++i)
        {
            if (i + 1 == update_user_servant_list.servant.active_skill.size())
            {
                new_data += to_string(update_user_servant_list.servant.active_skill[i]);
            }
            else
            {
                new_data += to_string(update_user_servant_list.servant.active_skill[i]) + ":";
            }
        }
        new_data += "]";
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

    auth_user_table.modify(auth_user_iter, _self, [&](auto &update_auth_user) {
        update_auth_user.current_servant_inventory += 1;
    });

    action(permission_level{get_self(), "active"_n},
           get_self(), "resultgacha"_n,
           std::make_tuple(_user, std::string("servant"), new_data))
        .send();
}

uint8_t battletest::gacha_servant_head(uint64_t _seed, uint32_t _count)
{
    head_db head_db_table(_self, _self.value);
    uint8_t random_head = safeseed::get_random_value(_seed, HEAD_COUNT, DEFAULT_MIN_DB, _count);
    const auto &head_db_iter = head_db_table.get(random_head, "Empty Servant Head");
    return head_db_iter.head;
}

uint8_t battletest::gacha_servant_hair(uint64_t _seed, uint32_t _count)
{
    hair_db hair_db_table(_self, _self.value);
    uint8_t random_hair = safeseed::get_random_value(_seed, HAIR_COUNT, DEFAULT_MIN_DB, _count);
    const auto &hair_db_iter = hair_db_table.get(random_hair, "Empty Servant Hair");
    return hair_db_iter.hair;
}

uint8_t battletest::gacha_servant_body(uint64_t _seed, uint32_t _count)
{
    body_db body_db_table(_self, _self.value);
    uint8_t random_body = safeseed::get_random_value(_seed, BODY_COUNT, DEFAULT_MIN_DB, _count);
    const auto &body_db_iter = body_db_table.get(random_body, "Empty Servant Body");
    return body_db_iter.body;
}

uint32_t battletest::change_monster_statue(uint32_t _grade, uint32_t _status_grade)
{
    monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
    auto status_iter = monster_lv_status_db_table.find(_grade); //서번트는 등급 고정
    int a = 0;
    for (uint32_t i = 0; i < status_iter->change_status.size(); ++i)
    {
        if (status_iter->change_status[i].pre_status == _status_grade)
        {
            a = 1;
            return status_iter->change_status[i].update_status;
        }
    }
    eosio_assert(a != 0, "Not Exist Status Monster 1");
    return 0;
}

void battletest::gacha_monster_id(eosio::name _user, uint64_t _seed)
{
    main_gacha_db main_gacha_db_table(_self, _self.value);
    auto gacha_db_iter = main_gacha_db_table.find(2); //1 ser 2 mon 3 item
    eosio_assert(gacha_db_iter != main_gacha_db_table.end(), "Gacha Monster : Empty Max Count");
    uint32_t random_monster_id = safeseed::get_random_value(_seed, gacha_db_iter->db_index, DEFAULT_MIN_DB, monster_random_count);
    random_monster_id += MONSTER_GACHA_ID_START;

    auto gacha_monster_db_iter = main_gacha_db_table.find(random_monster_id);
    eosio_assert(gacha_monster_db_iter != main_gacha_db_table.end(), "Gacha Monster : Empty Gacha ID");

    monster_db monster_id_db_table(_self, _self.value);
    const auto &monster_id_db_iter = monster_id_db_table.get(gacha_monster_db_iter->db_index, "Gacha Monster : Empty Monster ID");

    monster_random_count += 1;
    uint64_t random_rate = safeseed::get_random_value(_seed, GACHA_MAX_RATE, DEFAULT_MIN, monster_random_count);
    uint64_t random_grade = get_random_grade(random_rate);

    monster_grade_db monster_grade_db_table(_self, _self.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "Gacha Monster : Empty Grade");

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);

    user_auths auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(_user.value);

    eosio_assert(user_log_iter != user_log_table.end(), "Gacha Monster : Empty Log Table / Not Yet Signup");

    std::string new_data;
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
        new_monster.monster_class = monster_id_db_iter.monster_class;
        new_monster.type = monster_id_db_iter.type;
        new_monster.exp = 0;
        new_monster.upgrade = 0;
        monster_random_count += 1;
        new_monster.status.basic_str = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_str, monster_grade_db_iter.min_range.base_str, monster_random_count);
        monster_random_count += 1;
        new_monster.status.basic_dex = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_dex, monster_grade_db_iter.min_range.base_dex, monster_random_count);
        monster_random_count += 1;
        new_monster.status.basic_int = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_int, monster_grade_db_iter.min_range.base_int, monster_random_count);

        new_monster.status.basic_str = change_monster_statue(new_monster.grade, new_monster.status.basic_str);
        new_monster.status.basic_dex = change_monster_statue(new_monster.grade, new_monster.status.basic_dex);
        new_monster.status.basic_int = change_monster_statue(new_monster.grade, new_monster.status.basic_int);

        new_monster.state = object_state::on_inventory;

        uint32_t active_id = get_monster_active_skill(monster_id_db_iter.monster_class, _seed);
        if (active_id != 0)
        {
            new_monster.active_skill.push_back(active_id);
        }

        uint32_t passive_id = get_monster_passive_skill(monster_id_db_iter.monster_class, _seed);
        if (passive_id != 0)
        {
            new_monster.passive_skill.push_back(passive_id);
        }

        result.index = update_user_monster_list.index;
        result.type = result::monster;

        update_user_monster_list.party_number = EMPTY_PARTY;
        update_user_monster_list.monster = new_monster;

        new_data += to_string(update_user_monster_list.index) + ":";
        new_data += to_string(update_user_monster_list.party_number) + ":";
        new_data += to_string(update_user_monster_list.monster.id) + ":";
        new_data += to_string(update_user_monster_list.monster.state) + ":";
        new_data += to_string(update_user_monster_list.monster.exp) + ":";
        new_data += to_string(update_user_monster_list.monster.type) + ":";
        new_data += to_string(update_user_monster_list.monster.monster_class) + ":";
        new_data += to_string(update_user_monster_list.monster.grade) + ":";
        new_data += to_string(update_user_monster_list.monster.upgrade) + ":";
        new_data += to_string(update_user_monster_list.monster.level) + ":";
        new_data += to_string(update_user_monster_list.monster.status.basic_str) + ":";
        new_data += to_string(update_user_monster_list.monster.status.basic_dex) + ":";
        new_data += to_string(update_user_monster_list.monster.status.basic_int) + ":";
        new_data += "[";
        for (uint32_t i = 0; i < update_user_monster_list.monster.passive_skill.size(); ++i)
        {
            if (i + 1 == update_user_monster_list.monster.passive_skill.size())
            {
                new_data += to_string(update_user_monster_list.monster.passive_skill[i]);
            }
            else
            {
                new_data += to_string(update_user_monster_list.monster.passive_skill[i]) + ":";
            }
        }
        new_data += "]:";
        new_data += "[";
        for (uint32_t i = 0; i < update_user_monster_list.monster.active_skill.size(); ++i)
        {
            if (i + 1 == update_user_monster_list.monster.active_skill.size())
            {
                new_data += to_string(update_user_monster_list.monster.active_skill[i]);
            }
            else
            {
                new_data += to_string(update_user_monster_list.monster.active_skill[i]) + ":";
            }
        }
        new_data += "]";
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

    auth_user_table.modify(auth_user_iter, _self, [&](auto &update_auth_user) {
        update_auth_user.current_monster_inventory += 1;
    });

    action(permission_level{get_self(), "active"_n},
           get_self(), "resultgacha"_n,
           std::make_tuple(_user, std::string("monster"), new_data))
        .send();
}

uint32_t battletest::change_equipment_statue(uint32_t _grade, uint32_t _status_grade)
{
    equipment_lv_status_db equipment_lv_status_db_table(_self, _self.value);
    auto status_iter = equipment_lv_status_db_table.find(_grade);
    int a = 0;
    for (uint32_t i = 0; i < status_iter->change_status.size(); ++i)
    {
        if (status_iter->change_status[i].pre_status == _status_grade)
        {
            a = 1;
            return status_iter->change_status[i].update_status;
        }
    }
    eosio_assert(a != 0, "Not Exist Status equipment 1");
    return 0;
}

void battletest::gacha_equipment_id(eosio::name _user, uint64_t _seed)
{
    main_gacha_db main_gacha_db_table(_self, _self.value);
    auto gacha_db_iter = main_gacha_db_table.find(3); //1 ser 2 mon 3 item
    eosio_assert(gacha_db_iter != main_gacha_db_table.end(), "Gacha Equipment : Empty Max Count / Not Set Count");
    uint32_t random_item_id = safeseed::get_random_value(_seed, gacha_db_iter->db_index, DEFAULT_MIN_DB, item_random_count);
    random_item_id += ITEM_GACHA_ID_START;

    auto gacha_id_db_iter = main_gacha_db_table.find(random_item_id);
    eosio_assert(gacha_id_db_iter != main_gacha_db_table.end(), "Gacha Equipment : Empty Gacha ID / Not Set Gacha ID");

    equipment_db equip_item_table(_self, _self.value);
    const auto &equip_item_iter = equip_item_table.get(gacha_id_db_iter->db_index, "Gacha Equipmnet : Empty Equipment ID / Not Set Equipment ID");

    item_random_count += 1;
    uint64_t random_rate = safeseed::get_random_value(_seed, GACHA_MAX_RATE, DEFAULT_MIN, item_random_count);
    uint64_t random_grade = get_random_grade(random_rate);

    item_grade_db item_grade_db_table(_self, _self.value);
    const auto &item_grade_db_iter = item_grade_db_table.get(random_grade, "Gacha Equipment : Empty Grade / Not Set Grade");

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);

    user_auths auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(_user.value);
    eosio_assert(auth_user_iter != auth_user_table.end(), "Gacha Equipment : Empty Auth Table / Not Yet Signup");

    eosio_assert(user_log_iter != user_log_table.end(), "Gacha Equipment : Empty Log Table / Not Yet Signup");

    std::string new_data;
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

        equipment_info new_item;
        new_item.id = equip_item_iter.item_id;
        new_item.type = equip_item_iter.type;
        new_item.tier = equip_item_iter.tier;
        new_item.job = equip_item_iter.job;
        new_item.grade = item_grade_db_iter.grade;
        item_random_count += 1;
        new_item.value = safeseed::get_random_value(_seed, equip_item_iter.option_value_max_list[0] + 1, equip_item_iter.option_value_min_list[0], item_random_count);
        new_item.state = object_state::on_inventory;

        result.index = update_user_item_list.index;
        result.type = result::item;

        update_user_item_list.equipment = new_item;

        new_data += to_string(update_user_item_list.index) + ":";
        new_data += to_string(update_user_item_list.equipment.id) + ":";
        new_data += to_string(update_user_item_list.equipment.state) + ":";
        new_data += to_string(update_user_item_list.equipment.type) + ":";
        new_data += to_string(update_user_item_list.equipment.tier) + ":";
        new_data += to_string(update_user_item_list.equipment.job) + ":";
        new_data += to_string(update_user_item_list.equipment.grade) + ":";
        new_data += to_string(update_user_item_list.equipment.upgrade) + ":";
        new_data += to_string(update_user_item_list.equipment.value) + ":";
        new_data += to_string(update_user_item_list.equipment.equipservantindex);
    });

    user_gacha_results user_gacha_result_table(_self, _self.value);
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

    user_gacha_totals user_gacha_total_table(_self, _self.value);
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
        update_log.equipment_num += 1;
        update_log.gacha_num += 1;
    });

    auth_user_table.modify(auth_user_iter, _self, [&](auto &update_auth_user) {
        update_auth_user.current_equipment_inventory += 1;
    });

    action(permission_level{get_self(), "active"_n},
           get_self(), "resultgacha"_n,
           std::make_tuple(_user, std::string("equipment"), new_data))
        .send();
}


void battletest::gacha_item_id(eosio::name _user)
{
    allitem_db allitem_db_table(_self, _self.value);
    auto allitem_db_iter = allitem_db_table.find(500100);

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);

    user_auths auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(_user.value);

    eosio_assert(auth_user_iter != auth_user_table.end(),"Not Exist User 5");

    eosio_assert(user_log_iter != user_log_table.end(), "Not Exist User Log 5");

    item_info items;
    user_items user_item_table(_self, _user.value);
    user_item_table.emplace(_self, [&](auto &update_user_item_list) {
        update_user_item_list.id = allitem_db_iter->id;
        items.index = 0;
        update_user_item_list.type = 0;
        items.count = 5;
        update_user_item_list.item_list.push_back(items);
    });

    auth_user_table.modify(auth_user_iter, _self, [&](auto &update_auth_user){
        update_auth_user.current_item_inventory +=1;
    });
}


uint64_t battletest::get_user_seed_value(uint64_t _user)
{
    uint64_t user;

    user_logs user_log_table(_self, _self.value);
    const auto &user_log_iter = user_log_table.get(_user, "Seed Value : Log Table Empty / Not Yet Signup");
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
    eosio_assert(user_log_iter != user_log_table.end(), "Start Gacha : Log Table Empty / Not yet signup");

    eosio_assert(check_inventory(_user) == true, "Start Gacha : Inventory Is Full");

    uint64_t l_user = get_user_seed_value(_user.value);
    uint64_t l_seed = safeseed::get_seed_value(l_user, _seed);
    //가차 뽑기 참여 횟수

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
        gacha_equipment_id(_user, l_seed);
    }

    servant_random_count = 0;
    monster_random_count = 0;
    equipment_random_count = 0;
}

bool battletest::check_inventory(eosio::name _user)
{
    user_auths user_auth_table(_self, _self.value);
    auto users_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(users_auth_iter != user_auth_table.end(), "Not Exist User");

    if (users_auth_iter->current_servant_inventory >= users_auth_iter->servant_inventory)
    {
        return false;
    }
    else if (users_auth_iter->current_monster_inventory >= users_auth_iter->monster_inventory)
    {
        return false;
    }
    else if (users_auth_iter->current_equipment_inventory >= users_auth_iter->equipment_inventory)
    {
        return false;
    }
    else if (users_auth_iter->current_item_inventory >= users_auth_iter->item_inventory)
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

ACTION battletest::resultgacha(eosio::name _who, std::string _type, std::string _result)
{
    require_auth(_self);
    require_recipient(_self);
}

ACTION battletest::resultpre(eosio::name _from, eosio::name _to, std::string _result)
{
    require_auth(_self);
    require_recipient(_from);
}

ACTION battletest::resultparty(eosio::name _who, std::string _party_info, std::string _servant_list, std::string _monster_list)
{
    require_auth(_self);
    require_recipient(_self);
}

ACTION battletest::battlestate(eosio::name _who, std::string _stage_info, std::vector<std::string> &_my_state_list, std::vector<std::string> &_enemy_state_list)
{
    require_auth(_self);
    require_recipient(_self);
}

ACTION battletest::battleaction(eosio::name _who, std::string _turn, std::vector<std::string> &_data)
{
    require_auth(_self);
    require_recipient(_self);
}

ACTION battletest::battleresult(eosio::name _who, std::vector<std::string> &_reward)
{
    require_auth(_self);
    require_recipient(_self);
}
#pragma endresion

#pragma region party function

//------------------------------------------------------------------------//
//-------------------------------party_function---------------------------//
//------------------------------------------------------------------------//

bool battletest::check_same_party(eosio::name _user, uint32_t _party_number, const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list)
{
    uint32_t servant_same_count = 0;
    uint32_t monster_same_count = 0;

    user_partys user_party_table(_self, _user.value);            //
    auto user_party_iter = user_party_table.find(_party_number); //유저 파티 이터레이터는 파티 테이블 번호
    eosio_assert(user_party_iter != user_party_table.end(), "Check Same Party : Same Party Table");

    for(uint32_t i = 0 ; i < 5; ++i)
    {
        if(user_party_iter->servant_list[i] == _servant_list[i])
        {
            servant_same_count += 1;
        }
    }
    for (uint32_t i = 0; i < 5; ++i)
    {
        if (user_party_iter->monster_list[i] == _monster_list[i])
        {
            monster_same_count += 1;
        }
    }

    if(servant_same_count == 5 && monster_same_count == 5)
    {
        return true;
    }
    return false;
}

bool battletest::check_empty_party(const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list)
{
    uint32_t empty_count = 0;
    for (uint32_t i = 0; i < _servant_list.size(); ++i)
    {
        if (_servant_list[i] == 0)
        {
            empty_count += 1;
        }
    }
    for (uint32_t i = 0; i < _monster_list.size(); ++i)
    {
        if (_monster_list[i] == 0)
        {
            empty_count += 1;
        }
    }

    if (empty_count == 10)
    {
        return true;
    }

    return false;
}

ACTION battletest::saveparty(eosio::name _user, uint32_t _party_number, const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list)
{
    require_auth(_user);
    eosio_assert(_party_number == 1, "Save Party : Wrong Party Number / Party Number Only 1"); //잘못된 파티 넘버 체크
    eosio_assert(false == check_empty_party(_servant_list, _monster_list),"Save Party : Empty Party List");                                           
    user_partys user_party_table(_self, _user.value);                                                        //
    auto user_party_iter = user_party_table.find(_party_number);                                             //유저 파티 이터레이터는 파티 테이블 번호
    eosio_assert(user_party_iter != user_party_table.end(), "Save Party : Empty Auth Table / Not Yet Signup");                              //유저 파티 이터레이터를 기준으로 테이블 조회 시 없으면
    eosio_assert(user_party_iter->state != party_state::on_tower_defense, "Save Party : Impossible Save Party / On Tower"); //유저 파티 상태가 타워 디펜스중이 아닐시
    eosio_assert(false == check_same_party(_user, _party_number, _servant_list, _monster_list),"Save Party : Same Party");
    
    user_servants user_servant_table(_self, _user.value);
    user_monsters user_monster_table(_self, _user.value);

    std::string party_info;
    party_info += to_string(user_party_iter->index) + ":";
    party_info += to_string(user_party_iter->state);

    std::vector<uint32_t> servant_pos = {0, 1, 2, 3, 4};
    std::vector<uint32_t> monster_pos = {5, 6, 7, 8, 9};

    std::string servant_list;
    std::string monster_list;

    for (uint32_t i = 0; i < 5; ++i)
    {
        if (user_party_iter->servant_list[i] != EMPTY_PARTY)
        {
            auto user_servant_iter = user_servant_table.find(user_party_iter->servant_list[i]);
            eosio_assert(user_servant_iter != user_servant_table.end(), "Save Party : Empty Servant Table / Wrong Servant Index");
            user_servant_table.modify(user_servant_iter, _self, [&](auto &set_party) {
                set_party.party_number = EMPTY_PARTY;
            });
        }
    }
    for (uint32_t i = 0; i < 5; ++i)
    {
        if (user_party_iter->monster_list[i] != EMPTY_PARTY)
        {
            auto user_monster_iter = user_monster_table.find(user_party_iter->monster_list[i]);
            eosio_assert(user_monster_iter != user_monster_table.end(), "Save Party : Empty Monster Table / Wrong Monster Index");
            user_monster_table.modify(user_monster_iter, _self, [&](auto &set_party) {
                set_party.party_number = EMPTY_PARTY;
            });
        }
    }

    user_party_table.modify(user_party_iter, _self, [&](auto &save_party) {
        servant_list += "[";
        for (uint32_t i = 0; i < 5; ++i) //서번트에 대한 파티 배치 처리
        {
            if (_servant_list[i] == EMPTY_PARTY)
            {
                uint32_t pos = servant_pos[i];
                save_party.servant_list[pos] = 0;
                if (i + 1 == 5)
                {
                    servant_list += to_string(save_party.servant_list[i]);
                }
                else
                {
                    servant_list += to_string(save_party.servant_list[i]) + ":";
                }
                continue;
            }
            auto user_servant_iter = user_servant_table.find(_servant_list[i]);
            eosio_assert(user_servant_iter != user_servant_table.end(), "Save Party : Empty Servant Table / Wrong Servant Index");
            eosio_assert(user_servant_iter->party_number == EMPTY_PARTY, "Save Party : Already Set Servant / Servant Set Other Party");
            eosio_assert(user_servant_iter->servant.state == object_state::on_inventory, "Save Party : Servant State Not On Inventory / Servant Not On Inventory");
            user_servant_table.modify(user_servant_iter, owner, [&](auto &set_party) {
                set_party.party_number = _party_number;
            });
            uint32_t pos = servant_pos[i];
            save_party.servant_list[i] = _servant_list[pos];

            if (i + 1 == 5)
            {
                servant_list += to_string(save_party.servant_list[i]);
            }
            else
            {
                servant_list += to_string(save_party.servant_list[i]) + ":";
            }
        }
        servant_list += "]";
        monster_list += "[";
        for (uint32_t i = 0; i < 5; ++i)
        {
            if (_monster_list[i] == EMPTY_PARTY)
            {
                uint32_t pos = monster_pos[i] - 5;
                save_party.monster_list[pos] = 0;
                if (i + 1 == 5)
                {
                    monster_list += to_string(save_party.monster_list[i]);
                }
                else
                {
                    monster_list += to_string(save_party.monster_list[i]) + ":";
                }
                continue;
            }
            auto user_monster_iter = user_monster_table.find(_monster_list[i]);
            eosio_assert(user_monster_iter != user_monster_table.end(), "Save Party : Empty Monster Table / Wrong Monster Index");
            eosio_assert(user_monster_iter->party_number == EMPTY_PARTY, "Save Party : Already Set Monster / Monster Set Other Party");
            eosio_assert(user_monster_iter->monster.state == object_state::on_inventory, "Save Party : Monster State Not On Inventory / Monster Not On Inventory");
            user_monster_table.modify(user_monster_iter, owner, [&](auto &set_party) {
                set_party.party_number = _party_number;
            });
            uint32_t pos = monster_pos[i] - 5;
            save_party.monster_list[pos] = _monster_list[i];
            if (i + 1 == 5)
            {
                monster_list += to_string(save_party.monster_list[i]);
            }
            else
            {
                monster_list += to_string(save_party.monster_list[i]) + ":";
            }
        }
        monster_list += "]";
    });

    action(permission_level{get_self(), "active"_n},
           get_self(), "resultparty"_n,
           std::make_tuple(_user, party_info, servant_list, monster_list))
        .send();
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
ACTION battletest::testcheat(eosio::name _user)
{
    require_auth(_user);
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "Cheat : Server Pause");

    whitelist whitelist_table(_self, _self.value);
    auto whitelist_iter = whitelist_table.find(_user.value);
    eosio_assert(whitelist_iter != whitelist_table.end(), "Cheat : Empty WhiteList / Not Yet Add White");

    user_auths auth_user_table(_self, _self.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "Cheat : Already Auth Table / Already Signup");
    auth_user_table.emplace(_self, [&](auto &new_user) {
        new_user.user = _user;
        new_user.state = user_state::lobby;
        new_user.exp = 0;
        new_user.rank = 1;
        new_user.current_servant_inventory = 0;
        new_user.current_monster_inventory = 0;
        new_user.current_item_inventory = 0;
        new_user.current_equipment_inventory = 0;
        new_user.servant_inventory = 50;
        new_user.monster_inventory = 50;
        new_user.item_inventory = 50;
        new_user.equipment_inventory = 50;
    });

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter == user_log_table.end(), "Cheat : Already Log Table / Already Signup");
    user_log_table.emplace(_self, [&](auto &new_log) {
        new_log.user = _user;
    });

    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.find(1);
    eosio_assert(user_party_iter == user_party_table.end(), "Cheat : Already Party Table / Already Signup");
    user_party_table.emplace(_self, [&](auto &automatic_party) {
        automatic_party.index = 1;
        automatic_party.servant_list.resize(5);
        automatic_party.monster_list.resize(5);
        automatic_party.servant_list[0] = 1;
    });

    servant_job_db servant_job_table(_self, _self.value);
    uint64_t _seed = _user.value + now();
    uint32_t random_job = 1;
    const auto &servant_job_db_iter = servant_job_table.get(random_job, "Cheat : Empty Servant Job / Wrong Servant Job");
    uint32_t random_body = gacha_servant_body(_seed, servant_random_count);

    servant_random_count += 1;
    gender_db gender_table(_self, _self.value);
    uint32_t random_gender = safeseed::get_random_value(_seed, GEMDER_COUNT, DEFAULT_MIN_DB, servant_random_count);
    const auto &gender_db_iter = gender_table.get(random_gender, "Cheat : Empty Servant Gender / Wrong Servant Gender");

    servant_random_count += 1;
    uint32_t random_head = gacha_servant_head(_seed, servant_random_count);

    servant_random_count += 1;
    uint32_t random_hair = gacha_servant_hair(_seed, servant_random_count);

    servant_db servant_id_table(_self, _self.value);
    uint32_t servant_index = get_servant_index(random_job, random_body, random_gender, random_head, random_hair);
    const auto &servant_id_db_iter = servant_id_table.get(servant_index, "Cheat : Empty Servant ID / Wrong Servant ID");

    user_servants user_servant_table(_self, _user.value);
    user_servant_table.emplace(_self, [&](auto &update_user_servant_list) {
        update_user_servant_list.index = 1;

        servant_info new_servant;
        new_servant.id = servant_id_db_iter.id;
        new_servant.exp = 0;
        new_servant.grade = 5;
        servant_random_count += 1;
        new_servant.status.basic_str = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_str, servant_job_db_iter.min_range.base_str, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_dex = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_dex, servant_job_db_iter.min_range.base_dex, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_int = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_int, servant_job_db_iter.min_range.base_int, servant_random_count);
        new_servant.status.basic_str = change_servant_statue(new_servant.status.basic_str);
        new_servant.status.basic_dex = change_servant_statue(new_servant.status.basic_dex);
        new_servant.status.basic_int = change_servant_statue(new_servant.status.basic_int);
        new_servant.equip_slot.resize(3);
        new_servant.state = object_state::on_inventory;

        update_user_servant_list.party_number = 1;
        update_user_servant_list.servant = new_servant;
    });
}

#pragma endregion


#pragma region item equipment function
//------------------------------------------------------------------------//
//-------------------------item_equipment_function------------------------//
//------------------------------------------------------------------------//

#pragma endregion

#pragma region battle function
//------------------------------------------------------------------------//
//---------------------------------battle_function------------------------//
//------------------------------------------------------------------------//
uint32_t battletest::get_max_hp(status_info _status)
{
    uint32_t hp;
    hp = ((_status.basic_str * 14) + (_status.basic_dex * 5) + (_status.basic_int * 3));
    return hp;
}
uint32_t battletest::get_magic_attack(status_info _status)
{
    uint32_t magic_attack;
    magic_attack = ((_status.basic_int) * 300) / decimal; //3.0 * 100
    return magic_attack;
}
uint32_t battletest::get_physical_attack(status_info _status)
{
    uint32_t physical_attack;
    physical_attack = ((_status.basic_str + _status.basic_dex) * 220) / decimal; //2.2 * 100
    return physical_attack;
}
uint32_t battletest::get_magic_defense(status_info _status)
{
    uint32_t magic_defense;
    magic_defense = ((_status.basic_int) * 130) / decimal; //1.3 * 100
    return magic_defense;
}
uint32_t battletest::get_physical_defense(status_info _status)
{
    uint32_t physical_defense;
    physical_defense = ((_status.basic_dex) * 130) / decimal; //1.3 * 100
    return physical_defense;
}

void battletest::check_physical_attack_option(eosio::name _user, uint32_t _index, uint32_t &_attack)
{
    user_servants user_servant_table(_self, _user.value);
    auto user_servant_iter = user_servant_table.find(_index);
    eosio_assert(user_servant_iter != user_servant_table.end(), "Physical Attack Option : Empty Servant Index / Wrong Servant Index");

    user_equip_items user_equipment_table(_self, _user.value);
    equipment_db equipment_db_table(_self, _self.value);
    for (uint32_t i = 0; i < user_servant_iter->servant.equip_slot.size(); ++i)
    {
        if (user_servant_iter->servant.equip_slot[i] == 0)
        {
            continue;
        }
        auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[i]);
        eosio_assert(user_equipment_iter != user_equipment_table.end(), "Physical Attack Option : Empty Equipment Index / Wrong Equipment Index");

        auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
        eosio_assert(db_equipment_iter != equipment_db_table.end(), "Physical Attack Option : Empty Equipment ID / Wrong Equipment ID");

        for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
        {
            if (db_equipment_iter->option_list[a] == option_list::physical_attack)
            {
                _attack += user_equipment_iter->equipment.value;
            }
        }
    }
}
void battletest::check_magic_attack_option(eosio::name _user, uint32_t _index, uint32_t &_attack)
{
    user_servants user_servant_table(_self, _user.value);
    auto user_servant_iter = user_servant_table.find(_index);
    eosio_assert(user_servant_iter != user_servant_table.end(), "Magic Attack Option : Empty Servant Index / Wrong Servant Index");

    user_equip_items user_equipment_table(_self, _user.value);
    equipment_db equipment_db_table(_self, _self.value);
    for (uint32_t i = 0; i < user_servant_iter->servant.equip_slot.size(); ++i)
    {
        if (user_servant_iter->servant.equip_slot[i] == 0)
        {
            continue;
        }
        auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[i]);
        eosio_assert(user_equipment_iter != user_equipment_table.end(), "Magic Attack Option : Empty Equipment Index / Wrong Equipment Index");

        auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
        eosio_assert(db_equipment_iter != equipment_db_table.end(), "Magic Attack Option : Empty Equipment ID / Wrong Equipment ID");

        for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
        {
            if (db_equipment_iter->option_list[a] == option_list::magic_attack)
            {
                _attack += user_equipment_iter->equipment.value;
            }
        }
    }
}
void battletest::check_physical_defense_option(eosio::name _user, uint32_t _index, uint32_t &_defense)
{
    user_servants user_servant_table(_self, _user.value);
    auto user_servant_iter = user_servant_table.find(_index);
    eosio_assert(user_servant_iter != user_servant_table.end(), "Physical Defense Option : Empty Servant Index / Wrong Servant Index");

    user_equip_items user_equipment_table(_self, _user.value);
    equipment_db equipment_db_table(_self, _self.value);
    for (uint32_t i = 0; i < user_servant_iter->servant.equip_slot.size(); ++i)
    {
        if (user_servant_iter->servant.equip_slot[i] == 0)
        {
            continue;
        }
        auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[i]);
        eosio_assert(user_equipment_iter != user_equipment_table.end(), "Physical Defense Option : Empty Equipment Index / Wrong Equipment Index");

        auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
        eosio_assert(db_equipment_iter != equipment_db_table.end(), "Physical Defense Option : Empty Equipment ID / Wrong Equipment ID");

        for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
        {
            if (db_equipment_iter->option_list[a] == option_list::physical_defense)
            {
                _defense += user_equipment_iter->equipment.value;
            }
        }
    }
}
void battletest::check_magic_defense_option(eosio::name _user, uint32_t _index, uint32_t &_defense)
{
    user_servants user_servant_table(_self, _user.value);
    auto user_servant_iter = user_servant_table.find(_index);
    eosio_assert(user_servant_iter != user_servant_table.end(), "Magic Defense Option : Empty Servant Index / Wrong Servant Index");

    user_equip_items user_equipment_table(_self, _user.value);
    equipment_db equipment_db_table(_self, _self.value);
    for (uint32_t i = 0; i < user_servant_iter->servant.equip_slot.size(); ++i)
    {
        if (user_servant_iter->servant.equip_slot[i] == 0)
        {
            continue;
        }
        auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[i]);
        eosio_assert(user_equipment_iter != user_equipment_table.end(), "Magic Defense Option : Empty Equipment Index / Wrong Equipment Index");

        auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
        eosio_assert(db_equipment_iter != equipment_db_table.end(), "Magic Defense Option : Empty Equipment ID / Wrong Equipment ID");

        for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
        {
            if (db_equipment_iter->option_list[a] == option_list::magic_defense)
            {
                _defense += user_equipment_iter->equipment.value;
            }
        }
    }
}

void battletest::set_passive_attack_effect(character_state_data &_state, attacker_info &_attack)
{
    for (uint32_t i = 0; i < _state.passive_skill_list.size(); ++i)
    {
        passive_db passive_db_table(_self, _self.value);
        auto passive_iter = passive_db_table.find(_state.passive_skill_list[i]);
        eosio_assert(passive_iter != passive_db_table.end(), "Passive Effect : Empty Passive ID / Wrong Passive ID");
        if (passive_iter->passive_id == passive_name::passive_physical_attack)  
        {
            _attack.p_atk += (_attack.p_atk * passive_iter->effect_value) / 100;
        }
        else if (passive_iter->passive_id == passive_name::passive_magic_attack)
        {
            _attack.m_atk += (_attack.m_atk * passive_iter->effect_value) / 100;
        }
        else if (passive_iter->passive_id == passive_name::passive_warrior)
        {
            _attack.p_atk += (_attack.p_atk * passive_iter->effect_value) / 100;
        }
        else if (passive_iter->passive_id == passive_name::passive_thief)
        {
            _attack.m_atk += (_attack.p_atk * passive_iter->effect_value) / 100;
        }
    }
}
// void set_passive_defense_effect(character_state_data &_state, defender_info &_defense)
// {
//     for (uint32_t i = 0; i < _state.passive_skill_list.size(); ++i)
//     {
//         passive_db passive_db_table(_self, _self.value);
//         auto passive_iter = passive_db_table.find(_state.passive_skill_list[i]);
//         eosio_assert(passive_iter != passive_db_table.end(), "Passive Effect : Empty Passive ID / Wrong Passive ID");
//         if (passive_iter->passive_id == passive_name::passive_physical_defense)  
//         {
//             _defense.p_dfs += (_attack.p_dfs * passive_iter->effect_value) / 100;
//         }
//         else if (passive_iter->passive_id == passive_name::passive_magic_defense)
//         {
//             _attack.m_dfs += (_attack.m_dfs * passive_iter->effect_value) / 100;
//         }
//         else if (passive_iter->passive_id == passive_name::passive_archer)
//         {

//         }
//     }
// }
// void set_party_attack_passive_effect(std::vector<character_state_data> _state, attacker_info &_attack)
// {
//     for(uint32_t i = 0; i < _state.size(); ++i)
//     {
        
//     }
// }

// uint32_t get_same_class_count(std::vector<character_state_data> _state)
// {
//     uint32_t count = 0;
//     for (uint32_t i = 0; i < _state.size(); ++i)
//     {
//         if(_state[i].position < 10)
//     }
//     return count;
// }

battletest::status_info battletest::get_level_up_monster_status(uint64_t _class, uint64_t _grade, status_info _status)
{
    uint64_t lv_id = (10 * _class) + _grade;
    monster_lv_db monster_lv_db_table(_self, _self.value);
    auto monster_lv_db_iter = monster_lv_db_table.find(lv_id);
    eosio_assert(monster_lv_db_iter != monster_lv_db_table.end(), "Monster Levle Up : Empty Level / Wrong Level");
    uint32_t up_str = monster_lv_db_iter->lvup_str / 100;
    uint32_t up_dex = monster_lv_db_iter->lvup_dex / 100;
    uint32_t up_int = monster_lv_db_iter->lvup_int / 100;

    status_info status = _status;
    status.basic_str += up_str;
    status.basic_dex += up_dex;
    status.basic_int += up_int;

    return status;
}

battletest::status_info battletest::get_level_up_servant_status(uint64_t _job, status_info _status)
{
    status_info status = _status;
    servant_lv_db servant_lv_db_table(_self, _self.value);
    auto servant_lv_db_iter = servant_lv_db_table.find(_job);
    eosio_assert(servant_lv_db_iter != servant_lv_db_table.end(), "Servant Level Up : Empty Level / Wrong Level");
    uint32_t up_str = servant_lv_db_iter->lvup_str / 100;
    uint32_t up_dex = servant_lv_db_iter->lvup_dex / 100;
    uint32_t up_int = servant_lv_db_iter->lvup_int / 100;

    status.basic_str += up_str;
    status.basic_dex += up_dex;
    status.basic_int += up_int;

    return status;
}


uint64_t battletest::get_damage(uint32_t _atk, uint32_t _dfs)
{
    uint32_t damage = ((_atk * ((defense_constant * decimal) / (defense_constant + _dfs))));
    damage = damage / decimal;
    return damage;
}

void battletest::set_stage_state(uint64_t _stage_id, std::vector<character_state_data> &_enemy_state_list, std::vector<std::string> &_state)
{
    character_state_data get_state;

    stage_db stage_db_table(_self, _self.value);
    auto stage_db_iter = stage_db_table.find(_stage_id);
    eosio_assert(stage_db_iter != stage_db_table.end(), "Set Enemy State : Empty Stage / Wrong Stage ID");
    for (uint32_t i = 0; i < stage_db_iter->monster_list.size(); ++i)
    {
        stage_enemy_db stage_enemy_db_table(_self, _self.value);
        auto stage_enemy_db_iter = stage_enemy_db_table.find(stage_db_iter->monster_list[i].monster_id);
        eosio_assert(stage_enemy_db_iter != stage_enemy_db_table.end(), "Set Enemy State : Empty Enemy ID / Wrong Enemy ID");

        status_info stage_status;
        stage_status.basic_str = stage_enemy_db_iter->enemy_str;
        stage_status.basic_dex = stage_enemy_db_iter->enemy_dex;
        stage_status.basic_int = stage_enemy_db_iter->enemy_int;

        uint64_t level = safeseed::get_random_value(now(), stage_db_iter->enemy_level_max, stage_db_iter->enemy_level_min, 0);
        for (uint32_t j = 2; j <= level; ++j)
        {
            status_info level_up;
            level_up = get_level_up_monster_status(stage_enemy_db_iter->enemy_class, stage_enemy_db_iter->grade, stage_status);
            stage_status.basic_str = level_up.basic_str;
            stage_status.basic_dex = level_up.basic_dex;
            stage_status.basic_int = level_up.basic_int;
        }

        status_info status;
        status.basic_str = (stage_status.basic_str);
        status.basic_dex = (stage_status.basic_dex);
        status.basic_int = (stage_status.basic_int);

        get_state.grade = stage_enemy_db_iter->grade;
        get_state.id = stage_db_iter->monster_list[i].monster_id;
        get_state.index = 0;
        get_state.position = stage_db_iter->monster_list[i].position;

        get_state.now_hp = get_max_hp(status);
        get_state.state = battle_member_state::live;
        get_state.status = status;

        active_db active_db_table(_self, _self.value);
        for (uint32_t i = 0; i < stage_enemy_db_iter->active_list.size(); ++i)
        {
            auto active_db_iter = active_db_table.find(stage_enemy_db_iter->active_list[i]);
            eosio_assert(active_db_iter != active_db_table.end(), "Set Enemy State : Empty Active ID / Wrong Active ID");

            get_state.active_skill_list.push_back(active_db_iter->active_id);
        }

        passive_db passive_db_table(_self, _self.value);
        for (uint32_t i = 0; i < stage_enemy_db_iter->passive_list.size(); ++i)
        {
            auto passive_db_iter = passive_db_table.find(stage_enemy_db_iter->passive_list[i]);
            eosio_assert(passive_db_iter != passive_db_table.end(), "Set Enemy State : Empty Passive ID / Wrong Passive ID");
            if (passive_db_iter->passive_id == passive_name::passive_str)
            {
                status.basic_str += (status.basic_str * passive_db_iter->effect_value) / 100;
            }
            else if (passive_db_iter->passive_id == passive_name::passive_dex)
            {
                status.basic_dex += (status.basic_dex * passive_db_iter->effect_value) / 100;
            }
            else if (passive_db_iter->passive_id == passive_name::passive_int)
            {
                status.basic_int += (status.basic_int * passive_db_iter->effect_value) / 100;
            }
            get_state.passive_skill_list.push_back(passive_db_iter->passive_id);
        }

        _enemy_state_list.push_back(get_state);

        std::string state;
        state += to_string(get_state.grade) + ":";
        state += to_string(get_state.position) + ":";
        state += to_string(get_state.index) + ":";
        state += to_string(get_state.id) + ":";
        state += to_string(get_state.now_hp) + ":";
        state += to_string(get_state.state) + ":";
        state += "[";
        for (uint32_t i = 0; i < get_state.passive_skill_list.size(); ++i)
        {
            if (get_state.passive_skill_list.size() - 1 == i)
            {
                state += to_string(get_state.passive_skill_list[i]);
            }
            else
            {
                state += to_string(get_state.passive_skill_list[i]) + ":";
            }
        }
        state += "]";
        state += ":";

        state += "[";
        for (uint32_t i = 0; i < get_state.active_skill_list.size(); ++i)
        {
            if (get_state.active_skill_list.size() - 1 == i)
            {
                state += to_string(get_state.active_skill_list[i]);
            }
            else
            {
                state += to_string(get_state.active_skill_list[i]) + ":";
            }
        }
        state += "]";
        state += ":";

        state += to_string(get_state.status.basic_str) + ":";
        state += to_string(get_state.status.basic_dex) + ":";
        state += to_string(get_state.status.basic_int);

        _state.push_back(state);
    }
}

battletest::character_state_data battletest::get_user_state(eosio::name _user, std::string _type, uint64_t _index, uint32_t _position, std::vector<std::string> &_state)
{
    character_state_data get_state;
    status_info status;
    uint32_t increase_hp = 0;

    if (_type == "ser")
    {
        user_servants user_servant_table(_self, _user.value);
        auto user_servant_iter = user_servant_table.find(_index);
        eosio_assert(user_servant_iter != user_servant_table.end(), "Set User State : Empty Servant Index / Wrong Servant Index");

        status.basic_str = (user_servant_iter->servant.status.basic_str);
        status.basic_dex = (user_servant_iter->servant.status.basic_dex);
        status.basic_int = (user_servant_iter->servant.status.basic_int);

        user_equip_items user_equipment_table(_self, _user.value);
        equipment_db equipment_db_table(_self, _self.value);
        for (uint32_t i = 0; i < user_servant_iter->servant.equip_slot.size(); ++i)         //서번트 장비 슬롯을 돌면서 힘민지에 대한 증가 효과 적용
        {
            if (user_servant_iter->servant.equip_slot[i] == 0)
            {
                continue;
            }
            auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[i]);
            eosio_assert(user_equipment_iter != user_equipment_table.end(), "Set User State : Empty Equipment Index / Wrong Equipment Index");

            auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
            eosio_assert(db_equipment_iter != equipment_db_table.end(), "Set User State : Empty Equipment ID / Wrong Equipment ID");

            for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
            {
                if (db_equipment_iter->option_list[a] == option_list::status_str)   //힘 증가
                {
                    status.basic_str += user_equipment_iter->equipment.value;
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_dex)  //민 증가
                {
                    status.basic_dex += user_equipment_iter->equipment.value;
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_int)  //지 증가
                {
                    status.basic_int += user_equipment_iter->equipment.value;
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_hp)   //hp 증가
                {
                    increase_hp += user_equipment_iter->equipment.value;
                }
            }
        }

        if (user_servant_iter->servant.passive_skill.size() != 0)       //서번트의 패시브 스킬들을 확인하면서 힘민지에 대한 증가 효과 적용
        {
            passive_db passive_db_table(_self, _self.value);
            for (uint32_t i = 0; i < user_servant_iter->servant.passive_skill.size(); ++i)
            {
                auto passive_db_iter = passive_db_table.find(user_servant_iter->servant.passive_skill[i]);
                eosio_assert(passive_db_iter != passive_db_table.end(), "Set User State : Empty Servant Passive ID / Wrong Servant Passive ID");
                if (passive_db_iter->passive_id == passive_name::passive_str)
                {
                    status.basic_str += (status.basic_str * passive_db_iter->effect_value) / 100;
                }
                else if (passive_db_iter->passive_id == passive_name::passive_dex)
                {
                    status.basic_dex += (status.basic_dex * passive_db_iter->effect_value) / 100;
                }
                else if (passive_db_iter->passive_id == passive_name::passive_int)
                {
                    status.basic_int += (status.basic_int * passive_db_iter->effect_value) / 100;
                }
                get_state.passive_skill_list.push_back(passive_db_iter->passive_id);
            }
        }

        get_state.now_hp = get_max_hp(status);      
        get_state.now_hp += increase_hp;

        if (user_servant_iter->servant.active_skill.size() != 0)
        {
            active_db active_db_table(_self, _self.value);
            for (uint32_t i = 0; i < user_servant_iter->servant.active_skill.size(); ++i)
            {
                auto active_db_iter = active_db_table.find(user_servant_iter->servant.active_skill[i]);
                eosio_assert(active_db_iter != active_db_table.end(), "Set User State : Empty Servant Active ID / Wrong Servant Active ID");

                get_state.active_skill_list.push_back(active_db_iter->active_id);
            }
        }

        servant_db servant_db_table(_self, _self.value);
        auto servant_db_iter = servant_db_table.find(user_servant_iter->servant.id);
        eosio_assert(servant_db_iter != servant_db_table.end(), "Set User State : Empty Servant ID / Wrong Servant ID");

        get_state.grade = 5;
        get_state.index = _index;
        get_state.id = user_servant_iter->servant.id;
        get_state.position = _position;
    }
    else if (_type == "mon")
    {
        user_monsters user_monster_table(_self, _user.value);
        auto user_monster_iter = user_monster_table.find(_index);
        eosio_assert(user_monster_iter != user_monster_table.end(), "Set User State : Empty Monster Index / Wrong Monster Index");

        status.basic_str = (user_monster_iter->monster.status.basic_str);
        status.basic_dex = (user_monster_iter->monster.status.basic_dex);
        status.basic_int = (user_monster_iter->monster.status.basic_int);

        if (user_monster_iter->monster.passive_skill.size() != 0)
        {
            passive_db passive_db_table(_self, _self.value);
            for (uint32_t i = 0; i < user_monster_iter->monster.passive_skill.size(); ++i)
            {
                auto passive_db_iter = passive_db_table.find(user_monster_iter->monster.passive_skill[i]);
                eosio_assert(passive_db_iter != passive_db_table.end(), "Set User State : Empty Monster Passive ID / Wrong Monster Passive ID");
                if (passive_db_iter->passive_id == passive_name::passive_str)
                {
                    status.basic_str += (status.basic_str * passive_db_iter->effect_value) / 100;
                }
                else if (passive_db_iter->passive_id == passive_name::passive_dex) //誘쇱꺽 n% 利앷?
                {
                    status.basic_dex += (status.basic_dex * passive_db_iter->effect_value) / 100;
                }
                else if (passive_db_iter->passive_id == passive_name::passive_int) //吏??n% 利앷?
                {
                    status.basic_int += (status.basic_int * passive_db_iter->effect_value) / 100;
                }
                get_state.passive_skill_list.push_back(passive_db_iter->passive_id);
            }
        }

        if (user_monster_iter->monster.active_skill.size() != 0)
        {
            active_db active_db_table(_self, _self.value);
            for (uint32_t i = 0; i < user_monster_iter->monster.active_skill.size(); ++i)
            {
                auto active_db_iter = active_db_table.find(user_monster_iter->monster.active_skill[i]);
                eosio_assert(active_db_iter != active_db_table.end(), "Set User State : Empty Monster Active ID / Wrong Monster Active ID");

                get_state.active_skill_list.push_back(active_db_iter->active_id);
            }
        }
        get_state.grade = user_monster_iter->monster.grade;
        get_state.index = _index;
        get_state.id = user_monster_iter->monster.id;
        get_state.position = _position;
        get_state.now_hp = get_max_hp(status);
    }

    get_state.state = battle_member_state::live;
    get_state.status = status;

    if (get_state.passive_skill_list.size() != 0)
    {
        for (uint32_t i = 0; i < get_state.passive_skill_list.size(); ++i)
        {
            passive_db passive_db_table(_self, _self.value);
            auto passive_db_iter = passive_db_table.find(get_state.passive_skill_list[i]);
            eosio_assert(passive_db_iter != passive_db_table.end(), "Set User State : Empty Passive ID / Wrong Passive ID");
            if (passive_db_iter->passive_id == passive_name::passive_hp)
            {
                get_state.now_hp += (get_state.now_hp * passive_db_iter->effect_value) / 100;
            }
        }
    }
    //=================================영수증 처리==========================================//
    std::string state;
    state += to_string(get_state.grade) + ":";
    state += to_string(get_state.position) + ":";
    state += to_string(get_state.index) + ":";
    state += to_string(get_state.id) + ":";
    state += to_string(get_state.now_hp) + ":";
    state += to_string(get_state.state) + ":";

    state += "[";
    for (uint32_t i = 0; i < get_state.passive_skill_list.size(); ++i)
    {
        if (get_state.passive_skill_list.size() - 1 == i)
        {
            state += to_string(get_state.passive_skill_list[i]);
        }
        else
        {
            state += to_string(get_state.passive_skill_list[i]) + ":";
        }
    }
    state += "]";
    state += ":";

    state += "[";
    for (uint32_t i = 0; i < get_state.active_skill_list.size(); ++i)
    {
        if (get_state.active_skill_list.size() - 1 == i)
        {
            state += to_string(get_state.active_skill_list[i]);
        }
        else
        {
            state += to_string(get_state.active_skill_list[i]) + ":";
        }
    }
    state += "]";
    state += ":";

    state += to_string(get_state.status.basic_str) + ":";
    state += to_string(get_state.status.basic_dex) + ":";
    state += to_string(get_state.status.basic_int);

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
uint32_t battletest::get_stage_id(uint32_t _stage_type, uint32_t _stage_floor)
{
    uint32_t stage_id;
    stage_id = 100000 + (_stage_type * 100) + (_stage_floor * 1);
    return stage_id;
}

bool battletest::possible_start(eosio::name _user, uint32_t _party_number)
{
    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.find(_party_number);
    eosio_assert(user_party_iter != user_party_table.end(), "Possible Start : Empty Party Table / Not Yet Signup");

    uint32_t servant_count = 0;
    uint32_t monster_count = 0;

    for(uint32_t i = 0; i < 5; ++i)
    {
        if(user_party_iter->servant_list[i] == 0)
        {
            servant_count += 1;
        }
    }
    for(uint32_t  i = 0; i < 5; ++i)
    {
        if(user_party_iter->monster_list[i] == 0)
        {
            monster_count += 1;
        }
    }

    if(servant_count == 5 && monster_count == 5)    //파티가 전부 비어있으면
    {
        return false;
    }

    return true;
}

ACTION battletest::stagestart(eosio::name _user, uint32_t _party_number, uint32_t _stage_type, uint32_t _stage_floor)
{
    require_auth(_user);

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "Stage Start : Server Pause");

    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "Stage Start : BlackList User");

    whitelist whitelist_table(_self, _self.value);
    auto whitelist_iter = whitelist_table.find(_user.value);
    eosio_assert(whitelist_iter != whitelist_table.end(), "Stage Start : Empty White / Not Refer White List");

    eosio_assert(check_inventory(_user) == true, "stagestart : Inventory Is Full");
    
    std::vector<uint32_t> servant_pos_list = {0, 1, 2, 3, 4};
    std::vector<uint32_t> monster_pos_list = {5, 6, 7, 8, 9};

    //스테이지 정보 읽어오기
    uint32_t stage_id = get_stage_id(_stage_type, _stage_floor);
    stage_db stage_db_table(_self, _self.value);
    auto stage_db_iter = stage_db_table.find(stage_id);
    eosio_assert(stage_db_iter != stage_db_table.end(), "Stage Start : Empty Stage / Not Set Stage");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Stage Start : Empty Auth Table / Not Yet Signup");
    eosio_assert(user_auth_iter->state == user_state::lobby, "Stage Start : Already Battle");
    user_auth_table.modify(user_auth_iter, _self, [&](auto &user_state_change) {
        user_state_change.state = user_state::stage;
    });

    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.find(_party_number);
    eosio_assert(user_party_iter != user_party_table.end(), "Stage Start : Empty Party Table / Not Yet Signup");
    eosio_assert(user_party_iter->state == party_state::on_wait, "Stage Start : Party State Wrong");
    eosio_assert(true == possible_start(_user, _party_number), "Stage Start : Empty Party");

    std::string stage_info;
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
            new_battle_set.stage_number = stage_db_iter->stage_floor;
            new_battle_set.turn = 0;

            auto n = name{new_battle_set.user};
            auto e = name{new_battle_set.enemy_user};
            stage_info += n.to_string() + ":";
            stage_info += to_string(new_battle_set.stage_type) + ":";
            stage_info += e.to_string() + ":";
            stage_info += to_string(new_battle_set.stage_number) + ":";
            stage_info += to_string(new_battle_set.turn);

            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter->servant_list[i] == 0) //파티 멤버가 비어있으면
                {
                    continue;
                }
                character_state_data servant_battle_state = get_user_state(_user, "ser", user_party_iter->servant_list[i], servant_pos_list[i], my_state);
                new_battle_set.my_state_list.push_back(servant_battle_state);
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter->monster_list[i] == 0) //파티 멤버가 비어있으면
                {
                    continue;
                }
                character_state_data monster_battle_state = get_user_state(_user, "mon", user_party_iter->monster_list[i], monster_pos_list[i], my_state);
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
            new_battle_set.stage_number = stage_db_iter->stage_floor;
            new_battle_set.turn = 0;
            new_battle_set.my_state_list.clear();
            new_battle_set.enemy_state_list.clear();

            auto n = name{new_battle_set.user};
            auto e = name{new_battle_set.enemy_user};
            stage_info += n.to_string() + ":";
            stage_info += to_string(new_battle_set.stage_type) + ":";
            stage_info += e.to_string() + ":";
            stage_info += to_string(new_battle_set.stage_number) + ":";
            stage_info += to_string(new_battle_set.turn);

            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter->servant_list[i] == 0) //파티 멤버가 비어있으면
                {
                    continue;
                }
                character_state_data servant_battle_state = get_user_state(_user, "ser", user_party_iter->servant_list[i], servant_pos_list[i], my_state);
                new_battle_set.my_state_list.push_back(servant_battle_state);
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter->monster_list[i] == 0) //파티 멤버가 비어있으면
                {
                    continue;
                }
                character_state_data monster_battle_state = get_user_state(_user, "mon", user_party_iter->monster_list[i], monster_pos_list[i], my_state);
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
            new_battle_action.character_action_list.clear();
        });
    }
    else
    {
        user_battle_action_table.modify(user_battle_action_iter, _self, [&](auto &new_battle_action) {
            new_battle_action.turn = START_BATTLE;
            new_battle_action.character_action_list.clear();
        });
    }

    battle_reward_list battle_reward_list_table(_self, _self.value);
    auto battle_reward_iter = battle_reward_list_table.find(_user.value);
    if (battle_reward_iter == battle_reward_list_table.end())
    {
        battle_reward_list_table.emplace(_self, [&](auto &set_reward) {
            set_reward.user = _user;
            set_reward.reward_money = 0;
            exp_info user_exp;
            set_reward.get_rank_exp = user_exp;
            set_reward.get_char_exp_list.clear();
            set_reward.get_servant_list.clear();
            set_reward.get_monster_list.clear();
            set_reward.get_equipment_list.clear();
            set_reward.get_item_list.clear();
        });
    }
    else
    {
        battle_reward_list_table.modify(battle_reward_iter, _self, [&](auto &set_reward) {
            set_reward.reward_money = 0;
            exp_info user_exp;
            set_reward.get_rank_exp = user_exp;
            set_reward.get_char_exp_list.clear();
            set_reward.get_servant_list.clear();
            set_reward.get_monster_list.clear();
            set_reward.get_equipment_list.clear();
            set_reward.get_item_list.clear();
        });
    }

    action(permission_level{get_self(), "active"_n},
           get_self(), "battlestate"_n,
           std::make_tuple(_user, stage_info, my_state, enemy_state))
        .send();
}

#pragma endregion

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#pragma region active turn
int battletest::get_random_target(const std::vector<character_state_data> &_enemy_state_list, uint64_t _seed, uint32_t _max, uint32_t _min)
{
    int target_key = safeseed::get_random_value(_seed, _max, _min, 0);
    if ((_enemy_state_list[target_key].state == battle_member_state::dead))
    {
        target_key = -1;
        for (uint32_t i = _min; i < _max; i++)
        {
            if ((_enemy_state_list[i].state != battle_member_state::dead))
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

int battletest::get_target_key(const std::vector<character_state_data> &_enemy_state_list, uint64_t _target_position)
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

bool battletest::check_activate_skill(uint32_t _skill, uint64_t _rate)
{
    active_db active_db_table(_self, _self.value);
    auto active_iter = active_db_table.find(_skill);
    eosio_assert(active_iter != active_db_table.end(), "Activate Skill : Empty Activd ID / Wrong Active ID");
    if (active_iter->active_per > _rate)
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
    if (_buff.turn == 0)
    {
        return false;
    }
    else
    {
        return true;
    }
}

void battletest::init_buff_effect(character_state_data &_state, buff_info _buff)
{
    if (_buff.id == buff_state::defense)
    {
    }
}

void battletest::init_buff_turn(std::vector<character_state_data> &_state_list)
{
    for (uint32_t i = 0; i < _state_list.size(); ++i)
    {
        if (_state_list[i].buff_list.size() != 0)
        {
            for (uint32_t buff = 0; buff < _state_list[i].buff_list.size(); ++buff)
            {
                if (_state_list[i].buff_list[buff].turn - 1 <= 0)
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

bool battletest::check_type_up(eosio::name _user, uint32_t _attacker, uint32_t _defender)
{
    monster_db monster_db_table(_self, _self.value);
    auto attack_monster_iter = monster_db_table.find(_attacker);
    eosio_assert(attack_monster_iter != monster_db_table.end(),"Check Up Type Attack : Empty Monster ID / Wrong Monster ID");

    auto defender_monster_iter = monster_db_table.find(_defender);
    eosio_assert(defender_monster_iter != monster_db_table.end(), "Check Up Type Defender : Empty Monster ID / Wrong Monster ID");

    if (attack_monster_iter->type == monster_type::water &&
        defender_monster_iter->type == monster_type::fire)
    {
        return true;
    }
    else if (attack_monster_iter->type == monster_type::fire &&
             defender_monster_iter->type == monster_type::earth)
    {
        return true;
    }
    else if (attack_monster_iter->type == monster_type::earth &&
             defender_monster_iter->type == monster_type::wind)
    {
        return true;
    }
    else if (attack_monster_iter->type == monster_type::wind &&
             defender_monster_iter->type == monster_type::water)
    {
        return true;
    }
    else if (attack_monster_iter->type == monster_type::light &&
             defender_monster_iter->type == monster_type::dark)
    {
        return true;
    }
    else if (attack_monster_iter->type == monster_type::dark &&
             defender_monster_iter->type == monster_type::light)
    {
        return true;
    }
    else
    {
        return false;
    }
    
}
bool battletest::check_type_down(eosio::name _user, uint32_t _attacker, uint32_t _defender)
{
    monster_db monster_db_table(_self, _self.value);
    auto attack_monster_iter = monster_db_table.find(_attacker);
    eosio_assert(attack_monster_iter != monster_db_table.end(), "Check Up Type Attack : Empty Monster ID / Wrong Monster ID");

    auto defender_monster_iter = monster_db_table.find(_defender);
    eosio_assert(defender_monster_iter != monster_db_table.end(), "Check Up Type Defender : Empty Monster ID / Wrong Monster ID");

    if (attack_monster_iter->type == monster_type::fire &&
        defender_monster_iter->type == monster_type::water)
    {
        return true;
    }
    else if (attack_monster_iter->type == monster_type::water &&
             defender_monster_iter->type == monster_type::wind)
    {
        return true;
    }
    else if (attack_monster_iter->type == monster_type::wind &&
             defender_monster_iter->type == monster_type::earth)
    {
        return true;
    }
    else if (attack_monster_iter->type == monster_type::earth &&
             defender_monster_iter->type == monster_type::fire)
    {
        return true;
    }
    else
    {
        return false;
    }
}

bool battletest::set_action(eosio::name _user,
                            uint32_t _action,
                            uint64_t _seed,
                            std::vector<character_state_data> &_my_state_list,
                            std::vector<character_state_data> &_enemy_state_list,
                            uint64_t _my_key, character_action_data &_action_info,
                            std::vector<std::string> &_data)
{
    std::string action_data;
    _action_info.my_position = _my_state_list[_my_key].position;
    _action_info.action_type = _action;
    if (_action == action_type::attack)
    {
        int enemy_key = get_random_target(_enemy_state_list, _seed, _enemy_state_list.size(), 0);
        if (enemy_key == -1) //상대 파티가 모두 죽은 상태
        {
            return false;
        }

        action_info new_action;
        new_action = get_target_action(_user, _action, _my_state_list, _enemy_state_list, _seed, _my_key, enemy_key);

        if ((_my_state_list[_my_key].position >= 5 && _my_state_list[_my_key].position < 10) &&         //my_state 가 나일때
            (_enemy_state_list[enemy_key].position >= 10 && _enemy_state_list[enemy_key].position < 20))
        {
            if(check_type_up(_user, _my_state_list[_my_key].id , _enemy_state_list[enemy_key].id) == true)  //상성이면
            {
                new_action.damage += (new_action.damage * 50) / 100;
            }
            else
            {
                if(check_type_down(_user, _my_state_list[_my_key].id , _enemy_state_list[enemy_key].id) == true)    //반대 상성이면
                {
                    new_action.damage -= (new_action.damage * 30) / 100;
                }
            }
        }
        else if ((_my_state_list[_my_key].position >= 10 && _my_state_list[_my_key].position < 20) &&   //my_state  적일때
                 (_enemy_state_list[enemy_key].position >= 5 && _enemy_state_list[enemy_key].position < 10))
        {
            if(check_type_up(_user, _my_state_list[_my_key].id , _enemy_state_list[enemy_key].id) == true)
            {
                new_action.damage += (new_action.damage * 50) / 100;
            }
            else
            {
                if(check_type_down(_user, _my_state_list[_my_key].id , _enemy_state_list[enemy_key].id) == true)
                {
                    new_action.damage -= (new_action.damage * 30) / 100;
                }
            }
        }

        if (_enemy_state_list[enemy_key].now_hp <= new_action.damage)
        {
            _enemy_state_list[enemy_key].now_hp = 0;
            _enemy_state_list[enemy_key].state = battle_member_state::dead;
        }
        else
        {
            _enemy_state_list[enemy_key].now_hp -= new_action.damage;
        }
        _action_info.action_info_list.push_back(new_action);
    }
    else if (_action == action_type::skill)
    {
        active_db active_db_table(_self, _self.value);
        auto active_iter = active_db_table.find(_my_state_list[_my_key].active_skill_list[0]);
        eosio_assert(active_iter != active_db_table.end(), "Set Action : Empty Active ID / Wrong Active ID");

        if (active_iter->active_id == active_name::active_heal) //힐스킬
        {
            int enemy_key = get_random_target(_my_state_list, _seed, _my_state_list.size(), 0);
            if (enemy_key == -1) //상대 파티가 모두 죽은 상태
            {
                return false;
            }
            action_info new_action;
            new_action = get_target_action(_user, _action,
                                           _my_state_list, _my_state_list, _seed, _my_key, enemy_key);
            uint32_t max_hp = get_max_hp(_my_state_list[enemy_key].status);
            if (_my_state_list[enemy_key].passive_skill_list[0] == passive_name::passive_hp) //max hp利앷? ?⑥떆釉뚭? ?덈뒗吏 ?뺤씤
            {
                passive_db passive_db_table(_self, _self.value);
                auto passive_iter = passive_db_table.find(_my_state_list[enemy_key].passive_skill_list[0]);
                eosio_assert(passive_iter != passive_db_table.end(), "Set Action : Empty Passive ID / Wrong Passive ID");
                max_hp += (max_hp * passive_iter->effect_value) / 100;
            }
            if (max_hp >= _my_state_list[enemy_key].now_hp + new_action.damage)
            {
                _my_state_list[enemy_key].now_hp += new_action.damage;
            }
            else
            {
                _my_state_list[enemy_key].now_hp = max_hp;
            }
            _action_info.action_info_list.push_back(new_action);
        }

        else
        {
            for (uint32_t i = 0; i < active_iter->target_count; ++i)
            {
                uint64_t new_seed = safeseed::get_seed_value(now(), _seed);
                int enemy_key = get_random_target(_enemy_state_list, new_seed, _enemy_state_list.size(), 0);
                if (enemy_key == -1) //상대 파티가 모두 죽은 상태
                {
                    return false;
                }
                new_seed = new_seed >> 1;

                action_info new_action;
                new_action = get_target_action(_user, _action,
                                               _my_state_list, _enemy_state_list, _seed, _my_key, enemy_key);

                if ((_my_state_list[_my_key].position >= 5 && _my_state_list[_my_key].position < 10) && //my_state 가 나일때
                    (_enemy_state_list[enemy_key].position >= 10 && _enemy_state_list[enemy_key].position < 20))
                {
                    if (check_type_up(_user, _my_state_list[_my_key].id, _enemy_state_list[enemy_key].id) == true)
                    {
                        new_action.damage += (new_action.damage * 50) / 100;
                    }
                    else
                    {
                        if (check_type_down(_user, _my_state_list[_my_key].id, _enemy_state_list[enemy_key].id) == true)
                        {
                            new_action.damage -= (new_action.damage * 30) / 100;
                        }
                    }
                }
                else if ((_my_state_list[_my_key].position >= 10 && _my_state_list[_my_key].position < 20) && //my_state  적일때
                         (_enemy_state_list[enemy_key].position >= 5 && _enemy_state_list[enemy_key].position < 10))
                {
                    if (check_type_up(_user, _my_state_list[_my_key].id, _enemy_state_list[enemy_key].id) == true)
                    {
                        new_action.damage += (new_action.damage * 50) / 100;
                    }
                    else
                    {
                        if (check_type_down(_user, _my_state_list[_my_key].id, _enemy_state_list[enemy_key].id) == true)
                        {
                            new_action.damage -= (new_action.damage * 30) / 100;
                        }
                    }
                }

                if (_enemy_state_list[enemy_key].now_hp <= new_action.damage)
                {
                    _enemy_state_list[enemy_key].now_hp = 0;
                    _enemy_state_list[enemy_key].state = battle_member_state::dead;
                }
                else
                {
                    _enemy_state_list[enemy_key].now_hp -= new_action.damage;
                }
                _action_info.action_info_list.push_back(new_action);
            }
        }
    }

    action_data += to_string(_action_info.my_position) + ":";
    action_data += to_string(_action_info.action_type) + ":";
    action_data += "[";
    for (uint32_t i = 0; i < _action_info.action_info_list.size(); ++i)
    {
        action_data += to_string(_action_info.action_info_list[i].target_position) + ":";
        action_data += to_string(_action_info.action_info_list[i].avoid) + ":";
        action_data += to_string(_action_info.action_info_list[i].critical) + ":";
        if (i + 1 == _action_info.action_info_list.size())
        {
            action_data += to_string(_action_info.action_info_list[i].damage);
        }
        else
        {
            action_data += to_string(_action_info.action_info_list[i].damage) + ":";
        }
    }
    action_data += "]";

    _data.push_back(action_data);
    return true;
}

void battletest::set_skill_damage(uint32_t _skill_id, uint32_t &_attack, uint32_t &_cri_dmg)
{
    active_db active_db_table(_self, _self.value);
    auto active_iter = active_db_table.find(_skill_id);
    eosio_assert(active_iter != active_db_table.end(), "Skill Dmg Type : Empty Active ID / Wrong Activd ID");

    switch (active_iter->active_id)
    {
    case active_name::active_defense: //버프 스킬
    {
        break;
    }
    case active_name::active_fast_attack: //빨리 때리기
    {
        break;
    }
    case active_name::active_bash:
    {
        _attack = (_attack * active_iter->atk_per) / 100;
        _cri_dmg = (_cri_dmg * active_iter->atk_per) / 100;
        break;
    }
    case active_name::active_critical_strike: //확정 크리
    {
        break;
    }
    case active_name::active_heal:
    {
        _attack = (_attack * active_iter->heal_per) / 100;
        _cri_dmg = (_cri_dmg * active_iter->heal_per) / 100;
        break;
    }
    case active_name::active_magic_strike:
    {
        _attack = (_attack * active_iter->atk_per) / 100;
        _cri_dmg = (_cri_dmg * active_iter->atk_per) / 100;
        break;
    }
    case active_name::active_multi_shot:
    {
        _attack = (_attack * active_iter->atk_per) / 100;
        _cri_dmg = (_cri_dmg * active_iter->atk_per) / 100;
        break;
    }
    case active_name::active_guided_arrow:
    {
        _attack = (_attack * active_iter->atk_per) / 100;
        _cri_dmg = (_cri_dmg * active_iter->atk_per) / 100;
        break;
    }
    default:
    {
        eosio_assert(1 == 0, "Wrong Skill ID 1");
        break;
    }
    }
}

void battletest::set_skill_type(eosio::name _user, uint32_t _skill_id,
                                character_state_data &_my_state,
                                character_state_data &_enemy_state,
                                uint32_t &_attack,
                                uint32_t &_cri_dmg,
                                uint32_t &_cri_per,
                                uint32_t &_target_avoid,
                                uint32_t &_target_defense)
{
    active_db active_db_table(_self, _self.value);
    auto active_iter = active_db_table.find(_skill_id);
    eosio_assert(active_iter != active_db_table.end(), "Skill Atk Dmg Type : Empty Active ID / Wrong Activd ID");

    switch (active_iter->active_id)
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
        set_attack_type(_user, active_iter->attack_type, _my_state, _attack, _cri_dmg, _cri_per);
        set_dmg_type(_user, active_iter->dmg_type, _enemy_state, _target_avoid, _target_defense);
        break;
    }
    default:
    {
        eosio_assert(1 == 0, "Skill Atk Dmg Type : Wrong Skill ID");
        break;
    }
    }
}

void battletest::set_attack_type(eosio::name _user, uint32_t _atk_type, character_state_data &_state, uint32_t &_attack, uint32_t &_cri_dmg, uint32_t &_cri_per)
{
    switch (_atk_type)
    {
    case atk_type::buff_none:
    {
        _attack = get_magic_attack(_state.status); //힐 스킬 계수 반영
        break;
    }
    case atk_type::physical_atk:
    {
        if (_state.position < 5)
        {
            servant_db servant_db_table(_self, _self.value);
            auto servant_iter = servant_db_table.find(_state.id);
            eosio_assert(servant_iter != servant_db_table.end(), "Attack Type Phy : Empty Servant ID / Wrong Servant ID");

            job_stat_db job_stat_db_table(_self, _self.value);
            auto job_iter = job_stat_db_table.find(servant_iter->job);
            eosio_assert(job_iter != job_stat_db_table.end(), "Attack Type Phy :  Empty Servant Job / Wrong Servant Job");

            _attack = get_physical_attack(_state.status);
            check_physical_attack_option(_user, _state.index, _attack);
            if (_state.passive_skill_list[0] == passive_name::passive_physical_attack)
            {
                passive_db passive_db_table(_self, _self.value);
                auto passive_iter = passive_db_table.find(_state.passive_skill_list[0]);
                eosio_assert(passive_iter != passive_db_table.end(), "Attack Type Phy : Empty Servant Passive ID / Wrong Servant Passive ID");
                _attack += (_attack * passive_iter->effect_value) / 100;
            }
            _cri_dmg = (_attack * job_iter->physical_cri_dmg) / 100;
            _cri_per = job_iter->physical_cri_per;
        }
        else
        {
            monster_db monster_db_table(_self, _self.value);
            auto monster_iter = monster_db_table.find(_state.id);
            eosio_assert(monster_iter != monster_db_table.end(), "Attack Type Phy : Empty Monster ID / Wrong Monster ID");

            class_stat_db class_stat_db_table(_self, _self.value);
            auto class_stat_iter = class_stat_db_table.find(monster_iter->monster_class);
            eosio_assert(class_stat_iter != class_stat_db_table.end(), "Attack Type Phy : Empty Monster Class / Wrong Monster Class");

            _attack = get_physical_attack(_state.status);
            if (_state.passive_skill_list[0] == passive_name::passive_physical_attack)
            {
                passive_db passive_db_table(_self, _self.value);
                auto passive_iter = passive_db_table.find(_state.passive_skill_list[0]);
                eosio_assert(passive_iter != passive_db_table.end(), "Attack Type Phy : Empty Monster Passive ID / Wrong Monster Passive ID");
                _attack += (_attack * passive_iter->effect_value) / 100;
            }
            _cri_dmg = (_attack * class_stat_iter->physical_cri_dmg) / 100;
            _cri_per = class_stat_iter->physical_cri_per;
        }
        break;
    }
    case atk_type::magic_atk:
    {
        if (_state.position < 5)
        {
            servant_db servant_db_table(_self, _self.value);
            auto servant_iter = servant_db_table.find(_state.id);
            eosio_assert(servant_iter != servant_db_table.end(), "Attack Type Mgc : Empty Servant ID / Wrong Servant ID");

            job_stat_db job_stat_db_table(_self, _self.value);
            auto job_iter = job_stat_db_table.find(servant_iter->job);
            eosio_assert(job_iter != job_stat_db_table.end(), "Attack Type Mgc :  Empty Servant Job / Wrong Servant Job");

            _attack = get_magic_attack(_state.status);
            check_magic_attack_option(_user, _state.index, _attack);
            if (_state.passive_skill_list[0] == passive_name::passive_magic_attack)
            {
                passive_db passive_db_table(_self, _self.value);
                auto passive_iter = passive_db_table.find(_state.passive_skill_list[0]);
                eosio_assert(passive_iter != passive_db_table.end(), "Attack Type Mgc : Empty Servant Passive ID / Wrong Servant Passive ID");
                _attack += (_attack * passive_iter->effect_value) / 100;
            }
            _cri_dmg = (_attack * job_iter->magic_cri_dmg) / 100;
            _cri_per = job_iter->magic_cri_per;
        }
        else
        {
            monster_db monster_db_table(_self, _self.value);
            auto monster_iter = monster_db_table.find(_state.id);
            eosio_assert(monster_iter != monster_db_table.end(), "Attack Type Mgc : Empty Monster ID / Wrong Monster ID");

            class_stat_db class_stat_db_table(_self, _self.value);
            auto class_stat_iter = class_stat_db_table.find(monster_iter->monster_class);
            eosio_assert(class_stat_iter != class_stat_db_table.end(), "Attack Type Mgc : Empty Monster Class / Wrong Monster Class");

            _attack = get_magic_attack(_state.status);

            if (_state.passive_skill_list[0] == passive_name::passive_magic_attack)
            {
                passive_db passive_db_table(_self, _self.value);
                auto passive_iter = passive_db_table.find(_state.passive_skill_list[0]);
                eosio_assert(passive_iter != passive_db_table.end(), "Attack Type Mgc : Empty Monster Passive ID / Wrong Monster Passive ID");
                _attack += (_attack * passive_iter->effect_value) / 100;
            }
            _cri_dmg = (_attack * class_stat_iter->magic_cri_dmg) / 100;
            _cri_per = class_stat_iter->magic_cri_per;
        }
        break;
    }
    default:
    {
        eosio_assert(1 == 0, "Attack Type : Wrong Attack Type");
        break;
    }
    }
}

void battletest::set_dmg_type(eosio::name _user, uint32_t _dmg_type, character_state_data &_state, uint32_t &_avoid, uint32_t &_defense)
{
    switch (_dmg_type)
    {
    case dmg_type::buff_heal_none:
    {
        break;
    }
    case dmg_type::physical_dfs:
    {
        if (_state.position < 5)
        {
            servant_db servant_db_table(_self, _self.value);
            auto servant_iter = servant_db_table.find(_state.id);
            eosio_assert(servant_iter != servant_db_table.end(), "Defense Type Phy : Empty Servant ID / Wrong Servant ID");

            job_stat_db job_stat_db_table(_self, _self.value);
            auto job_iter = job_stat_db_table.find(servant_iter->job);
            eosio_assert(job_iter != job_stat_db_table.end(), "Defense Type Phy :  Empty Servant Job / Wrong Servant Job");

            _defense = get_physical_defense(_state.status);
            check_physical_defense_option(_user, _state.index, _defense);
            if (_state.passive_skill_list[0] == passive_name::passive_physical_defense)
            {
                passive_db passive_db_table(_self, _self.value);
                auto passive_iter = passive_db_table.find(_state.passive_skill_list[0]);
                eosio_assert(passive_iter != passive_db_table.end(), "Defense Type Phy : Empty Servant Passive ID / Wrong Servant Passive ID");
                _defense += (_defense * passive_iter->effect_value) / 100;
            }
            _avoid = job_iter->avoid;
        }
        else
        {
            monster_db monster_db_table(_self, _self.value);
            auto monster_iter = monster_db_table.find(_state.id);
            eosio_assert(monster_iter != monster_db_table.end(), "Defense Type Phy : Empty Monster ID / Wrong Monster ID");

            class_stat_db class_stat_db_table(_self, _self.value);
            auto class_stat_iter = class_stat_db_table.find(monster_iter->monster_class);
            eosio_assert(class_stat_iter != class_stat_db_table.end(), "Defense Type Phy : Empty Monster Class / Wrong Monster Class");

            _defense = get_physical_defense(_state.status);
            if (_state.passive_skill_list[0] == passive_name::passive_physical_defense)
            {
                passive_db passive_db_table(_self, _self.value);
                auto passive_iter = passive_db_table.find(_state.passive_skill_list[0]);
                eosio_assert(passive_iter != passive_db_table.end(), "Defense Type Phy : Empty Monster Passive ID/ Wrong Monster Passive ID");
                _defense += (_defense * passive_iter->effect_value) / 100;
            }
            _avoid = class_stat_iter->avoid;
        }

        break;
    }
    case dmg_type::magic_dfs:
    {
        if (_state.position < 5)
        {
            servant_db servant_db_table(_self, _self.value);
            auto servant_iter = servant_db_table.find(_state.id);
            eosio_assert(servant_iter != servant_db_table.end(), "Defense Type Mgc : Empty Servant ID / Wrong Servant ID");

            job_stat_db job_stat_db_table(_self, _self.value);
            auto job_iter = job_stat_db_table.find(servant_iter->job);
            eosio_assert(job_iter != job_stat_db_table.end(), "Defense Type Mgc : Empty Servant Job / Wrong Servant Job");

            _defense = get_magic_defense(_state.status);
            check_magic_defense_option(_user, _state.index, _defense);
            if (_state.passive_skill_list[0] == passive_name::passive_magic_defense)
            {
                passive_db passive_db_table(_self, _self.value);
                auto passive_iter = passive_db_table.find(_state.passive_skill_list[0]);
                eosio_assert(passive_iter != passive_db_table.end(), "Defense Type Mgc : Empty Servant Passive ID / Wrong Servant Passive ID");
                _defense += (_defense * passive_iter->effect_value) / 100;
            }
            _avoid = job_iter->avoid;
        }
        else
        {
            monster_db monster_db_table(_self, _self.value);
            auto monster_iter = monster_db_table.find(_state.id);
            eosio_assert(monster_iter != monster_db_table.end(), "Defense Type Mgc : Empty Monster ID / Wrong Monster ID");

            class_stat_db class_stat_db_table(_self, _self.value);
            auto class_stat_iter = class_stat_db_table.find(monster_iter->monster_class);
            eosio_assert(class_stat_iter != class_stat_db_table.end(), "Defense Type Mgc : Empty Monster Class / Wrong Monster Class");


            _defense = get_magic_defense(_state.status);
            if (_state.passive_skill_list[0] == passive_name::passive_magic_defense)
            {
                passive_db passive_db_table(_self, _self.value);
                auto passive_iter = passive_db_table.find(_state.passive_skill_list[0]);
                eosio_assert(passive_iter != passive_db_table.end(), "Defense Type Mgc : Empty Monster Passive ID / Wrong Monster Passive ID");
                _defense += (_defense * passive_iter->effect_value) / 100;
            }
            _avoid = class_stat_iter->avoid;
        }
        break;
    }
    default:
    {
        eosio_assert(1 == 0, "Defense Type : Wrong Defense Type");
        break;
    }
    }
}

void battletest::set_attacker(eosio::name _user, std::vector<character_state_data> &_state, uint32_t _key , attacker_info &_attack)
{
    if(_state[_key].position < 5)
    {
        servant_db servant_db_table(_self, _self.value);
        auto servant_iter = servant_db_table.find(_state[_key].id);
        eosio_assert(servant_iter != servant_db_table.end(), "Target Action : Empty Servant ID / Wrong Servant ID");

        job_stat_db job_stat_db_table(_self, _self.value);
        auto job_iter = job_stat_db_table.find(servant_iter->job);
        eosio_assert(job_iter != job_stat_db_table.end(), "Target Action : Empty Servant Job / Wrong Servant Job");

        _attack.p_atk = get_physical_attack(_state[_key].status);

        _attack.p_cri_dmg = (_attack.p_atk * job_iter->physical_cri_dmg) / 100;
        _attack.p_cri_per = job_iter->physical_cri_per;

        _attack.m_atk = get_magic_attack(_state[_key].status);
        _attack.m_cri_dmg = (_attack.m_atk * job_iter->magic_cri_dmg) / 100;
        _attack.m_cri_per = job_iter->magic_cri_per;
    }
    else
    {
        monster_db monster_db_table(_self, _self.value);
        auto monster_iter = monster_db_table.find(_state[_key].id);
        eosio_assert(monster_iter != monster_db_table.end(), "Target Action : Empty Monster ID / Wrong Monster ID");

        class_stat_db class_stat_db_table(_self, _self.value);
        auto class_stat_iter = class_stat_db_table.find(monster_iter->monster_class);
        eosio_assert(class_stat_iter != class_stat_db_table.end(), "Target Action : Empty Monster Class / Wrong Monster Class");
    }
}
void battletest::set_defender(eosio::name _user, std::vector<character_state_data> &_state, uint32_t _key , defender_info &_defense)
{

}

battletest::action_info battletest::get_target_action(eosio::name _user, uint32_t _active_id, std::vector<character_state_data> &_my_state_list, std::vector<character_state_data> &_enemy_state_list, uint64_t _seed, uint64_t _my_key, uint64_t _target_key)
{
    attacker_info attack_info;
    defender_info defense_info;

    uint32_t cur_target_key = _target_key;
    uint32_t cur_damage = 0;
    uint32_t cur_attack = 0;
    uint32_t cur_cirtical_dmg = 0;
    uint32_t cur_cri_per = 0;
    uint32_t max_hp = 0;
    uint32_t cur_hp = 0;

    uint32_t target_defense = 0;
    uint32_t target_avoid = 0;

    switch (_active_id)
    {
    case action_type::attack:
    {
        if (_my_state_list[_my_key].position < 5) //서번트인경우
        {
            servant_db servant_db_table(_self, _self.value);
            auto servant_iter = servant_db_table.find(_my_state_list[_my_key].id);
            eosio_assert(servant_iter != servant_db_table.end(), "Target Action : Empty Servant ID / Wrong Servant ID");

            job_stat_db job_stat_db_table(_self, _self.value);
            auto job_iter = job_stat_db_table.find(servant_iter->job);
            eosio_assert(job_iter != job_stat_db_table.end(), "Target Action : Empty Servant Job / Wrong Servant Job");

            cur_attack = get_physical_attack(_my_state_list[_my_key].status);
            cur_cirtical_dmg = (cur_attack * job_iter->physical_cri_dmg) / 100;
            cur_cri_per = job_iter->physical_cri_per;
        }
        else //스테이지포함하면 전부 몬스터니까
        {
            monster_db monster_db_table(_self, _self.value);
            auto monster_iter = monster_db_table.find(_my_state_list[_my_key].id);
            eosio_assert(monster_iter != monster_db_table.end(), "Target Action : Empty Monster ID / Wrong Monster ID");

            class_stat_db class_stat_db_table(_self, _self.value);
            auto class_stat_iter = class_stat_db_table.find(monster_iter->monster_class);
            eosio_assert(class_stat_iter != class_stat_db_table.end(), "Target Action : Empty Monster Class / Wrong Monster Class");

            cur_attack = get_physical_attack(_my_state_list[_my_key].status);
            cur_cirtical_dmg = (cur_attack * class_stat_iter->physical_cri_dmg) / 100;
            cur_cri_per = class_stat_iter->physical_cri_per;
        }

        // monster_db monster_db_table(_self, _self.value);
        // auto monster_iter = monster_db_table.find(_enemy_state_list[_target_key].id);
        // eosio_assert(monster_iter != monster_db_table.end(), "Not Exist Monster ID 40");

        // class_stat_db class_stat_db_table(_self, _self.value);
        // auto class_stat_iter = class_stat_db_table.find(monster_iter->monster_class);
        // eosio_assert(class_stat_iter != class_stat_db_table.end(), "Not Exist Class ID 40");

        target_defense = get_physical_defense(_enemy_state_list[_target_key].status);
        if (_enemy_state_list[_target_key].passive_skill_list[0] == passive_name::passive_physical_defense)
        {
            passive_db passive_db_table(_self, _self.value);
            auto passive_iter = passive_db_table.find(_enemy_state_list[_target_key].passive_skill_list[0]);
            eosio_assert(passive_iter != passive_db_table.end(), "Target Action : Empty Monster Passive ID / Wrong Monster Passive ID");
            target_defense += (target_defense * passive_iter->effect_value) / 100;
        }
        //target_avoid = class_stat_iter->avoid;
        break;
    }
    case action_type::skill:
    {
        set_skill_type(_user,
                       _my_state_list[_my_key].active_skill_list[0],
                       _my_state_list[_my_key],
                       _enemy_state_list[_target_key],
                       cur_attack,
                       cur_cirtical_dmg,
                       cur_cri_per,
                       target_avoid,
                       target_defense);
        set_skill_damage(_my_state_list[_my_key].active_skill_list[0],
                         cur_attack, cur_cirtical_dmg);

        switch (_my_state_list[_my_key].active_skill_list[0])
        {
        case active_name::active_bash:
        case active_name::active_fast_attack:
        case active_name::active_multi_shot:
        case active_name::active_guided_arrow:
        {
            action_info new_action;
            if (true == check_avoid(target_avoid, _seed))
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
            action_info new_action;
            if (true == check_avoid(target_avoid, _seed))
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
                            cur_damage = get_damage(cur_cirtical_dmg, target_defense + ((target_defense * 30) / 100));
                        }
                    }
                }
            }
            else
            {
                cur_damage = get_damage(cur_cirtical_dmg, target_defense);
            }
            new_action.target_position = _enemy_state_list[cur_target_key].position;
            new_action.avoid = 0;
            new_action.critical = 1;
            new_action.damage = cur_damage;
            return new_action;
        }
        case active_name::active_heal:
        {
            action_info new_action;
            new_action.target_position = _enemy_state_list[cur_target_key].position;
            new_action.avoid = 0;
            new_action.critical = 0;
            new_action.damage = cur_attack;
            return new_action;
        }
        case active_name::active_magic_strike:
        {
            break;
        }
        default:
        {
            eosio_assert(1 == 0, "Target Action : Wrong Skill ID");
            break;
        }
        }

        break;
    }
    default:

    {
        eosio_assert(1 == 0, "Target Action : Wrong Action Type");
        break;
    }
    }

    action_info new_action;
    if (false == check_critical(cur_cri_per, _seed))
    {
        if (_enemy_state_list[cur_target_key].buff_list.size() != 0) //버프가 있는지 확인
        {
            for (uint32_t i = 0; i < _enemy_state_list[cur_target_key].buff_list.size(); ++i)
            {
                if (true == check_buff_state(_enemy_state_list[cur_target_key].buff_list[i]))
                {
                    if (_enemy_state_list[cur_target_key].buff_list[i].id == buff_state::defense)
                    {
                        cur_damage = get_damage(cur_attack, target_defense + (target_defense * 30 / 100));
                    }
                }
            }
        }
        else
        {
            cur_damage = get_damage(cur_attack, target_defense);
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
                        cur_damage = get_damage(cur_cirtical_dmg, target_defense + (target_defense * 30 / 100));
                    }
                }
            }
        }
        else
        {
            cur_damage = get_damage(cur_cirtical_dmg, target_defense);
        }
        new_action.target_position = _enemy_state_list[cur_target_key].position;
        new_action.avoid = 0;
        new_action.critical = 1;
        new_action.damage = cur_damage;
    }
    return new_action;
}
void battletest::set_attack_order_list(std::vector<battle_order_struct> &_list,
                                       std::vector<uint64_t> &_random_order_list,
                                       const std::vector<character_state_data> &_my_state_list,
                                       const std::vector<character_state_data> &_enemy_state_list)
{
    for (uint32_t i = 0; i < _my_state_list.size(); ++i)
    {
        if ((_my_state_list[i].state == battle_member_state::dead))
        {
            continue;
        }

        battle_order_struct new_order;
        if (_my_state_list[i].position < 5)
        {
            servant_db servant_db_table(_self, _self.value);
            auto servant_iter = servant_db_table.find(_my_state_list[i].id);
            eosio_assert(servant_iter != servant_db_table.end(), "Attack Order My : Empty Servant ID / Wrong Servant ID");

            job_stat_db job_stat_db_table(_self, _self.value);
            auto job_iter = job_stat_db_table.find(servant_iter->job);
            eosio_assert(job_iter != job_stat_db_table.end(), "Attack Order My : Empty Servant Job / Wrong Servant Job");

            new_order.speed = job_iter->speed;
        }
        else
        {
            monster_db monster_db_table(_self, _self.value);
            auto monster_iter = monster_db_table.find(_my_state_list[i].id);
            eosio_assert(monster_iter != monster_db_table.end(), "Attack Order My : Empty Monster ID / Wrong Monster ID");

            class_stat_db class_stat_db_table(_self, _self.value);
            auto class_stat_iter = class_stat_db_table.find(monster_iter->monster_class);
            eosio_assert(class_stat_iter != class_stat_db_table.end(), "Attack Order My : Empty Monster Class / Wrong Monster Class");

            new_order.speed = class_stat_iter->speed;
        }
        new_order.position = _my_state_list[i].position;
        new_order.key = i;
        new_order.second_speed = _random_order_list[i];
        _list.push_back(new_order);
    }
    for (uint32_t i = 0; i < _enemy_state_list.size(); ++i)
    {
        if ((_enemy_state_list[i].state == battle_member_state::dead))
        {
            continue;
        }

        battle_order_struct new_order;
        if (_enemy_state_list[i].position < 5)
        {
            servant_db servant_db_table(_self, _self.value);
            auto servant_iter = servant_db_table.find(_enemy_state_list[i].id);
            eosio_assert(servant_iter != servant_db_table.end(), "Attack Order Enemy : Empty Servant ID / Wrong Servant ID");

            job_stat_db job_stat_db_table(_self, _self.value);
            auto job_iter = job_stat_db_table.find(servant_iter->job);
            eosio_assert(job_iter != job_stat_db_table.end(), "Attack Order Enemy : Empty Servant Job / Wrong Servant Job");

            new_order.speed = job_iter->speed;
        }
        else
        {
            monster_db monster_db_table(_self, _self.value);
            auto monster_iter = monster_db_table.find(_enemy_state_list[i].id);
            eosio_assert(monster_iter != monster_db_table.end(), "Attack Order Enemy : Empty Monster ID / Wrong Monster ID");

            class_stat_db class_stat_db_table(_self, _self.value);
            auto class_stat_iter = class_stat_db_table.find(monster_iter->monster_class);
            eosio_assert(class_stat_iter != class_stat_db_table.end(), "Attack Order Enemy : Empty Monster Class / Wrong Monster Class");

            new_order.speed = class_stat_iter->speed;
        }
        new_order.position = _enemy_state_list[i].position;
        new_order.key = i;
        new_order.second_speed = _random_order_list[i + 10];
        _list.push_back(new_order);
    }
}

ACTION battletest::activeturn(eosio::name _user, uint32_t _turn, std::string _seed)
{
    require_auth(_user);

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "Active Turn : Server Pause");

    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "Active Turn : BlackList User");

    whitelist whitelist_table(_self, _self.value);
    auto whitelist_iter = whitelist_table.find(_user.value);
    eosio_assert(whitelist_iter != whitelist_table.end(), "Actvie Turn : Not White User");
    //?⑥닔濡?類닿린
    size_t center = _seed.find(':');
    size_t end = _seed.length() - (center + 1);
    eosio_assert(_seed.find(':') != std::string::npos, "Actvie Turn : Wrong Seed");

    std::string result_seed = _seed.substr(0, center);
    std::string result_sha = _seed.substr(center + 1, end);

    uint64_t check_result = safeseed::check_seed(result_seed, result_sha);
    uint64_t user = get_user_seed_value(_user.value);
    uint64_t battle_seed = safeseed::get_seed_value(user, check_result);

    uint32_t user_dead_count = 0;
    uint32_t enemy_dead_count = 0;

    battle_state_list battle_state_list_table(_self, _self.value);
    auto user_battle_state_iter = battle_state_list_table.find(_user.value);
    eosio_assert(user_battle_state_iter != battle_state_list_table.end(), "Actvie Turn : Empty State Table / Not Yet Stage Start");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Active Turn : Empty Auth Table / Not Yet Signup");
    eosio_assert(user_auth_iter->state == user_state::stage, "Active Turn ; User State Not Stage / Not Yet Stage Start");

    battle_actions battle_action_table(_self, _self.value);
    auto user_battle_action_iter = battle_action_table.find(_user.value);
    eosio_assert(user_battle_action_iter != battle_action_table.end(), "Actvie Turn : Empty Action Table / Not Yet Stage Start");
    eosio_assert(user_battle_action_iter->turn == _turn - 1, "Active Turn : Different Turn / Wrong Turn");

    std::vector<uint64_t> order_random_list;
    safeseed::get_battle_rand_list(order_random_list, battle_seed);

    std::string turn;
    std::vector<std::string> data;
    //배틀의 상태를 바꿔주는 부분
    battle_state_list_table.modify(user_battle_state_iter, _self, [&](auto &character_state_data) {
        character_state_data.turn += 1;
        //버프 턴부터 진행
        init_buff_turn(character_state_data.my_state_list);
        init_buff_turn(character_state_data.enemy_state_list);
        //공격순서 정렬하는 부분
        std::vector<battle_order_struct> second_attack_order_list;
        std::vector<battle_order_struct> attack_order_list;
        std::vector<battle_order_struct> skill_order_list;

        set_attack_order_list(skill_order_list,
                              order_random_list,
                              character_state_data.my_state_list,
                              character_state_data.enemy_state_list);
        std::sort(skill_order_list.begin(), skill_order_list.end(), sort_compare);
        //---------------------

        //배틀에 액션테이블에 데이터를 추가해주는 부분
        battle_action_table.modify(user_battle_action_iter, _self, [&](auto &update_action) {
            update_action.turn += 1;
            turn = to_string(update_action.turn);
            update_action.character_action_list.clear();
            //먼저 스킬 발동 여부 확인
            for (uint32_t i = 0; i < skill_order_list.size(); ++i)
            {
                uint32_t my_key = skill_order_list[i].key;
                if (skill_order_list[i].position < max_party_count) //내 파티인데
                {
                    if (character_state_data.my_state_list[my_key].active_skill_list.size() != 0) //액티브 스킬이 있으면
                    {
                        uint64_t action_rate = safeseed::get_random_value(skill_order_list[i].second_speed, 100, 0, 0);
                        if (true == check_activate_skill(character_state_data.my_state_list[my_key].active_skill_list[0], action_rate)) //액티브 스킬이 발동하면
                        {
                            if (character_state_data.my_state_list[my_key].active_skill_list[0] == active_name::active_fast_attack) //패스트 어택
                            {
                                skill_order_list[i].action = action_type::skill;
                                attack_order_list.push_back(skill_order_list[i]);
                            }
                            else if (character_state_data.my_state_list[my_key].active_skill_list[0] == active_name::active_defense) //버프 스킬
                            {
                                buff_info new_buff;
                                new_buff.id = buff_state::defense;
                                new_buff.turn = 1;
                                character_state_data.my_state_list[my_key].buff_list.push_back(new_buff);

                                character_action_data action_info;
                                action_info.my_position = character_state_data.my_state_list[my_key].position;
                                action_info.action_type = action_type::skill;
                                update_action.character_action_list.push_back(action_info);

                                std::string action_data;
                                action_data += to_string(action_info.my_position) + ":";
                                action_data += to_string(action_info.action_type) + ":";
                                action_data += "[";
                                action_data += "]";
                                data.push_back(action_data);
                            }
                            else if (character_state_data.my_state_list[my_key].active_skill_list[0] == active_name::active_heal) //힐 스킬
                            {
                                character_action_data action_info;
                                if (false == set_action(_user, action_type::skill,
                                                        skill_order_list[i].second_speed,
                                                        character_state_data.my_state_list,
                                                        character_state_data.enemy_state_list,
                                                        my_key, action_info, data))
                                {
                                    break;
                                }
                                update_action.character_action_list.push_back(action_info);
                            }
                            else
                            {
                                skill_order_list[i].action = action_type::skill;
                                second_attack_order_list.push_back(skill_order_list[i]);
                            }
                        }
                        else //스킬이 발동하지 않으면
                        {
                            skill_order_list[i].action = action_type::attack;
                            second_attack_order_list.push_back(skill_order_list[i]);
                        }
                    }
                    else //스킬이 없으면
                    {
                        skill_order_list[i].action = action_type::attack;
                        second_attack_order_list.push_back(skill_order_list[i]);
                    }
                }
                else //적의 경우
                {
                    skill_order_list[i].action = action_type::attack;
                    second_attack_order_list.push_back(skill_order_list[i]);
                }
            }
            //패스트 어택 애들 처리
            for (uint32_t i = 0; i < attack_order_list.size(); ++i)
            {
                uint32_t my_key = attack_order_list[i].key;
                if (attack_order_list[i].position < max_party_count) //자기 파티에 대한 처리
                {
                    if (character_state_data.my_state_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    character_action_data action_info;
                    if (false == set_action(_user, action_type::skill,
                                            attack_order_list[i].second_speed,
                                            character_state_data.my_state_list,
                                            character_state_data.enemy_state_list,
                                            my_key, action_info, data))
                    {
                        break;
                    }
                    update_action.character_action_list.push_back(action_info);
                }
                else // 상대 파티의 경우
                {
                    if (character_state_data.enemy_state_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    character_action_data action_info;
                    if (false == set_action(_user, action_type::skill,
                                            attack_order_list[i].second_speed,
                                            character_state_data.enemy_state_list,
                                            character_state_data.my_state_list,
                                            my_key, action_info, data))
                    {
                        break;
                    }
                    update_action.character_action_list.push_back(action_info);
                }
            }
            //그냥 일반 공격하는 애들에 대한 처리 ===============================================================
            for (uint32_t i = 0; i < second_attack_order_list.size(); ++i)
            {
                uint32_t my_key = second_attack_order_list[i].key;
                if (second_attack_order_list[i].position < max_party_count) //자기 파티에 대한 처리
                {
                    if (character_state_data.my_state_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    character_action_data action_info;
                    if (false == set_action(_user, second_attack_order_list[i].action,
                                            second_attack_order_list[i].second_speed,
                                            character_state_data.my_state_list,
                                            character_state_data.enemy_state_list,
                                            my_key, action_info, data))
                    {
                        break;
                    }
                    update_action.character_action_list.push_back(action_info);
                }
                else // 상대 파티의 경우
                {
                    if (character_state_data.enemy_state_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    character_action_data action_info;
                    if (false == set_action(_user, second_attack_order_list[i].action,
                                            second_attack_order_list[i].second_speed,
                                            character_state_data.enemy_state_list,
                                            character_state_data.my_state_list,
                                            my_key, action_info, data))
                    {
                        break;
                    }
                    update_action.character_action_list.push_back(action_info);
                }
            }

            //게임의 종료 여부 체크
            for (uint32_t i = 0; i < character_state_data.my_state_list.size(); ++i)
            {
                if (character_state_data.my_state_list[i].state == battle_member_state::dead)
                {
                    user_dead_count += 1;
                }
            }

            for (uint32_t i = 0; i < character_state_data.enemy_state_list.size(); ++i)
            {
                if (character_state_data.enemy_state_list[i].state == battle_member_state::dead)
                {
                    enemy_dead_count += 1;
                }
            }
        });
    });
    if (enemy_dead_count == user_battle_state_iter->enemy_state_list.size())
    {
        uint32_t stage_number = 100000 + (100 * user_battle_state_iter->stage_type) + (1 * user_battle_state_iter->stage_number);
        win_reward(_user, stage_number, battle_seed);
    }
    else if (user_dead_count == user_battle_state_iter->my_state_list.size())
    {
        uint32_t stage_number = 100000 + (100 * user_battle_state_iter->stage_type) + (1 * user_battle_state_iter->stage_number);
        fail_reward(_user, stage_number);
    }


    action(permission_level{get_self(), "active"_n},
           get_self(), "battleaction"_n,
           std::make_tuple(_user, turn ,data))
        .send();
}

uint32_t battletest::check_char_level_up(uint32_t _cur_level, uint64_t _get_exp)
{
    uint32_t level_up_count = 0;
    lv_exp lv_exp_table(_self, _self.value);
    auto lv_exp_iter = lv_exp_table.find(_cur_level);
    if (lv_exp_iter->lv == 50)
    {
        return level_up_count;
    }

    for (auto iter = lv_exp_iter; iter != lv_exp_table.end();)
    {
        if (_get_exp >= iter->char_exp)
        {
            level_up_count += 1;
            iter++;
        }
        else
        {
            return level_up_count;
        }
    }
    return level_up_count;
}

uint32_t battletest::check_rank_level_up(uint32_t _cur_level, uint64_t _get_exp)
{
    uint32_t level_up_count = 0;
    lv_exp lv_exp_table(_self, _self.value);
    auto lv_exp_iter = lv_exp_table.find(_cur_level);
    if (lv_exp_iter->lv == 50)
    {
        return level_up_count;
    }

    for (auto iter = lv_exp_iter; iter != lv_exp_table.end();)
    {
        if (_get_exp >= iter->rank_exp)
        {
            level_up_count += 1;
            iter++;
        }
        else
        {
            return level_up_count;
        }
    }
    return level_up_count;
}
battletest::servant_data battletest::get_reward_servant(eosio::name _user, uint32_t _job, uint32_t _grade, uint64_t _seed)
{
    servant_job_db servant_job_table(_self, _self.value);
    uint32_t random_job = _job;
    const auto &servant_job_db_iter = servant_job_table.get(random_job, "Get Reward Servant : Empty Servant Job / Wrong Servant Job");

    servant_random_count += 1;
    uint32_t random_body = gacha_servant_body(_seed, servant_random_count);

    servant_random_count += 1;
    gender_db gender_table(_self, _self.value);
    uint32_t random_gender = safeseed::get_random_value(_seed, GEMDER_COUNT, DEFAULT_MIN_DB, servant_random_count);
    const auto &gender_db_iter = gender_table.get(random_gender, "Get Reward Servant : Empty Servant Gender / Wrong Servant Gender");

    servant_random_count += 1;
    uint32_t random_head = gacha_servant_head(_seed, servant_random_count);

    servant_random_count += 1;
    uint32_t random_hair = gacha_servant_hair(_seed, servant_random_count);

    servant_db servant_id_table(_self, _self.value);
    uint32_t servant_index = get_servant_index(random_job, random_body, random_gender, random_head, random_hair);
    const auto &servant_id_db_iter = servant_id_table.get(servant_index, "Get Reward Servant : Empty Servant ID / Wrong Servnat ID");

    user_auths auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(_user.value);

    servant_data new_data;
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
        new_servant.id = servant_id_db_iter.id;
        new_servant.exp = 0;
        servant_random_count += 1;

        new_servant.status.basic_str = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_str, servant_job_db_iter.min_range.base_str, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_dex = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_dex, servant_job_db_iter.min_range.base_dex, servant_random_count);
        servant_random_count += 1;
        new_servant.status.basic_int = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_int, servant_job_db_iter.min_range.base_int, servant_random_count);

        new_servant.status.basic_str = change_servant_statue(new_servant.status.basic_str);
        new_servant.status.basic_dex = change_servant_statue(new_servant.status.basic_dex);
        new_servant.status.basic_int = change_servant_statue(new_servant.status.basic_int);

        new_servant.equip_slot.resize(3);
        new_servant.state = object_state::on_inventory;

        update_user_servant_list.party_number = EMPTY_PARTY;
        update_user_servant_list.servant = new_servant;

        new_data.index = update_user_servant_list.index;
        new_data.party_number = update_user_servant_list.party_number;
        new_data.servant = new_servant;
    });

      user_logs user_log_table(_self, _self.value);
        auto user_log_iter = user_log_table.find(_user.value);

        user_log_table.modify(user_log_iter, _self, [&](auto &add_log) {
            add_log.servant_num += 1;
        });

    auth_user_table.modify(auth_user_iter, _self, [&](auto &update_auth_user) {
        update_auth_user.current_servant_inventory += 1;
    });
    return new_data;
}

battletest::monster_data battletest::get_reward_monster(eosio::name _user, uint32_t _id, uint32_t _grade, uint64_t _seed)
{
    monster_db monster_id_db_table(_self, _self.value);
    const auto &monster_id_db_iter = monster_id_db_table.get(_id, "Get Reward Monster : Empty Monster ID / Wrong Monster ID");

    uint64_t random_grade = _grade;

    monster_grade_db monster_grade_db_table(_self, _self.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "Get Reward Monster : Empty Monster Grade / Wrong Monster Grade");

    user_auths auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(_user.value);

    monster_data new_data;
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
        new_monster.monster_class = monster_id_db_iter.monster_class;
        new_monster.type = monster_id_db_iter.type;
        new_monster.exp = 0;
        new_monster.upgrade = 0;
        monster_random_count += 1;
        new_monster.status.basic_str = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_str, monster_grade_db_iter.min_range.base_str, monster_random_count);
        monster_random_count += 1;
        new_monster.status.basic_dex = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_dex, monster_grade_db_iter.min_range.base_dex, monster_random_count);
        monster_random_count += 1;
        new_monster.status.basic_int = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_int, monster_grade_db_iter.min_range.base_int, monster_random_count);

        new_monster.status.basic_str = change_monster_statue(new_monster.grade, new_monster.status.basic_str);
        new_monster.status.basic_dex = change_monster_statue(new_monster.grade, new_monster.status.basic_dex);
        new_monster.status.basic_int = change_monster_statue(new_monster.grade, new_monster.status.basic_int);

        new_monster.state = object_state::on_inventory;

        update_user_monster_list.party_number = EMPTY_PARTY;
        update_user_monster_list.monster = new_monster;

        new_data.index = update_user_monster_list.index;
        new_data.party_number = update_user_monster_list.party_number;
        new_data.monster = new_monster;
    });

      user_logs user_log_table(_self, _self.value);
        auto user_log_iter = user_log_table.find(_user.value);

        user_log_table.modify(user_log_iter, _self, [&](auto &add_log) {
            add_log.monster_num += 1;
        });

    auth_user_table.modify(auth_user_iter, _self, [&](auto &update_auth_user) {
        update_auth_user.current_monster_inventory += 1;
    });
    return new_data;
}

battletest::equip_data battletest::get_reward_equip(eosio::name _user, uint32_t _id, uint32_t _grade, uint64_t _seed)
{
    equipment_db equip_item_table(_self, _self.value);
    const auto &equip_item_iter = equip_item_table.get(_id, "Get Reward Equipment : Empty Equipment ID / Wrong Equipment ID");

    uint64_t random_grade = _grade;

    item_grade_db item_grade_db_table(_self, _self.value);
    const auto &item_grade_db_iter = item_grade_db_table.get(random_grade, "Get Reward Equipment : Empty Equipmnet Grade / Wrong Equipment Grade");

    user_auths auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(_user.value);

    equip_data new_data;
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

        equipment_info new_item;
        new_item.id = equip_item_iter.item_id;
        new_item.type = equip_item_iter.type;
        new_item.tier = equip_item_iter.tier;
        new_item.job = equip_item_iter.job;
        new_item.grade = item_grade_db_iter.grade;
        item_random_count += 1;
        new_item.value = safeseed::get_random_value(_seed, equip_item_iter.option_value_max_list[0], equip_item_iter.option_value_min_list[0], item_random_count);
        new_item.state = object_state::on_inventory;

        update_user_item_list.equipment = new_item;

        new_data.index = update_user_item_list.index;
        new_data.equipment = new_item;
    });

      user_logs user_log_table(_self, _self.value);
        auto user_log_iter = user_log_table.find(_user.value);

        user_log_table.modify(user_log_iter, _self, [&](auto &add_log) {
            add_log.equipment_num += 1;
        });

    auth_user_table.modify(auth_user_iter, _self, [&](auto &update_auth_user) {
        update_auth_user.current_equipment_inventory += 1;
    });
    return new_data;
}
battletest::item_data battletest::get_reward_item(eosio::name _user, uint32_t _id, uint32_t _count)
{
    allitem_db allitem_db_table(_self, _self.value);
    auto allitem_db_iter = allitem_db_table.find(_id);
    eosio_assert(allitem_db_iter != allitem_db_table.end(), "Get Reward Item : Empty Item ID / Wrong Item ID");

    item_data new_item;
    user_items user_item_table(_self, _user.value);
    auto user_item_iter = user_item_table.find(allitem_db_iter->id);
    if (user_item_iter == user_item_table.end())
    {
        user_item_table.emplace(_self, [&](auto &new_index) {
            new_index.id = allitem_db_iter->id;
            new_index.type = allitem_db_iter->type;
            //new_index.count = _count;
        });
        user_logs user_log_table(_self, _self.value);
        auto user_log_iter = user_log_table.find(_user.value);

        user_auths user_auth_table(_self, _self.value);
        auto user_auth_iter = user_auth_table.find(_user.value);

        user_log_table.modify(user_log_iter, _self, [&](auto &add_log) {
            add_log.item_num += _count;
        });
        user_auth_table.modify(user_auth_iter, _self, [&](auto &add_auth) {
            add_auth.current_item_inventory += _count;
        });
    }
    else
    {
        user_item_table.modify(user_item_iter, _self, [&](auto &new_index) {
            new_index.type = allitem_db_iter->type;
            //new_index.count += _count;
        });
        user_logs user_log_table(_self, _self.value);
        auto user_log_iter = user_log_table.find(_user.value);

        user_auths user_auth_table(_self, _self.value);
        auto user_auth_iter = user_auth_table.find(_user.value);

        user_log_table.modify(user_log_iter, _self, [&](auto &add_log) {
            add_log.item_num += _count;
        });
        user_auth_table.modify(user_auth_iter, _self, [&](auto &add_auth) {
            add_auth.current_item_inventory += _count;
        });
    }
    new_item.id = allitem_db_iter->id;
    new_item.type = allitem_db_iter->type;
    new_item.count = _count;

    return new_item;
}

void battletest::win_reward(eosio::name _user, uint64_t _stage_number, uint64_t _seed)
{
    exp_info get_rank_exp;
    std::vector<exp_info> get_char_exp_list;
    std::vector<monster_data> monster_list;
    std::vector<equip_data> equipment_list;
    std::vector<servant_data> servant_list;
    std::vector<item_data> item_list;
    std::vector<uint32_t> servant_pos_list = {0, 1, 2, 3, 4};
    std::vector<uint32_t> monster_pos_list = {5, 6, 7, 8, 9};

    reward_db reward_db_table(_self, _self.value);
    auto reward_iter = reward_db_table.find(_stage_number);
    eosio_assert(reward_iter != reward_db_table.end(), "Win Reward : Empty Reward ID / Wrong Reward ID");

    //보상 반영
    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.begin();
    eosio_assert(user_party_iter != user_party_table.end(), "Win Reward : Empty Party Number / Not Yet Signup");

    ///유저 경험치 처리
    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Win Reward : Empty Auth Table / Not Yet Signup");

    uint32_t get_exp = user_auth_iter->exp + reward_iter->rank_exp;
    uint32_t level_up_count = check_rank_level_up(user_auth_iter->rank, get_exp);
    user_auth_table.modify(user_auth_iter, _self, [&](auto &upadate_exp) {
        upadate_exp.rank += level_up_count;
        upadate_exp.exp = get_exp;
        upadate_exp.state = user_state::lobby;

        get_rank_exp.pos = 0;
        get_rank_exp.exp = reward_iter->rank_exp;
        get_rank_exp.lvup = level_up_count;
    });

    lv_exp lv_exp_table(_self, _self.value);
    user_servants user_servant_table(_self, _user.value);
    for (uint32_t i = 0; i < 5; ++i)
    {
        if (user_party_iter->servant_list[i] == 0)
        {
            continue;
        }
        auto user_servant_iter = user_servant_table.find(user_party_iter->servant_list[i]);
        eosio_assert(user_servant_iter != user_servant_table.end(), "Win Reward : Empty Servant Index / Wrong Servant Index");
        uint64_t get_exp = user_servant_iter->servant.exp + reward_iter->char_exp;

        servant_db servant_db_table(_self, _self.value);
        auto servant_db_iter = servant_db_table.find(user_servant_iter->servant.id);
        eosio_assert(servant_db_iter != servant_db_table.end(), "Win Reward : Empty Servant ID / Wrong Servant UD");
        user_servant_table.modify(user_servant_iter, _self, [&](auto &update_servant_exp) {
            uint32_t level_up_count = check_char_level_up(user_servant_iter->servant.level, get_exp);
            for (uint32_t i = 0; i < level_up_count; ++i)
            {
                update_servant_exp.servant.status = get_level_up_servant_status(servant_db_iter->job, user_servant_iter->servant.status);
            }
            update_servant_exp.servant.level += level_up_count;
            update_servant_exp.servant.exp = get_exp;

            exp_info char_exp;
            char_exp.pos = servant_pos_list[i];
            char_exp.exp = reward_iter->char_exp;
            char_exp.lvup = level_up_count;
            get_char_exp_list.push_back(char_exp);
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
        eosio_assert(user_monster_iter != user_monster_table.end(),"Win Reward : Empty Monster Index / Wrong Monster Index");
        uint64_t get_exp = user_monster_iter->monster.exp + reward_iter->char_exp;
        user_monster_table.modify(user_monster_iter, _self, [&](auto &update_monster_exp) {
            uint32_t level_up_count = check_char_level_up(user_monster_iter->monster.level, get_exp);
            for (uint32_t i = 0; i < level_up_count; ++i)
            {
                update_monster_exp.monster.status = get_level_up_monster_status(user_monster_iter->monster.monster_class, user_monster_iter->monster.grade, user_monster_iter->monster.status);
            }
            update_monster_exp.monster.level += level_up_count;
            update_monster_exp.monster.exp = get_exp;

            exp_info char_exp;
            char_exp.pos = monster_pos_list[i];
            char_exp.exp = reward_iter->char_exp;
            char_exp.lvup = level_up_count;
            get_char_exp_list.push_back(char_exp);
        });
    }

    //보상 상자 처리
    for (uint32_t i = 0; i < reward_iter->reward_list.size(); ++i)
    {
        if (reward_iter->reward_list[i].type == 1) //servant
        {
            uint64_t rate = safeseed::get_random_value(_seed, 10000, 0, 0);
            if (reward_iter->reward_list[i].per >= rate)
            {
                for (uint32_t ser = 0; ser < reward_iter->reward_list[i].count; ++ser)
                {
                    uint64_t seed = _seed;
                    uint32_t job = reward_iter->reward_list[i].id / 1000000;
                    servant_data new_servant = get_reward_servant(_user, job, reward_iter->reward_list[i].grade, seed);
                    servant_list.push_back(new_servant);
                    seed = seed >> 1;
                }
            }
        }
        else if (reward_iter->reward_list[i].type == 2) //monster
        {
            uint64_t rate = safeseed::get_random_value(_seed, 10000, 0, 1);
            if (reward_iter->reward_list[i].per >= rate)
            {
                for (uint32_t mon = 0; mon < reward_iter->reward_list[i].count; ++mon)
                {
                    uint64_t seed = _seed;
                    monster_data new_monster = get_reward_monster(_user, reward_iter->reward_list[i].id, reward_iter->reward_list[i].grade, seed);
                    monster_list.push_back(new_monster);
                    seed = seed >> 1;
                }
            }
        }
        else if (reward_iter->reward_list[i].type == 3) //equipment
        {
            uint64_t rate = safeseed::get_random_value(_seed, 10000, 0, 2);
            if (reward_iter->reward_list[i].per >= rate)
            {
                for (uint32_t equip = 0; equip < reward_iter->reward_list[i].count; ++equip)
                {
                    uint64_t seed = _seed;
                    equip_data new_equipment = get_reward_equip(_user, reward_iter->reward_list[i].id, reward_iter->reward_list[i].grade, seed);
                    equipment_list.push_back(new_equipment);
                    seed = seed >> 1;
                }
            }
        }
        else if (reward_iter->reward_list[i].type == 4) //item
        {
            uint64_t rate = safeseed::get_random_value(_seed, 10000, 0, 4);
            if (reward_iter->reward_list[i].per >= rate)
            {
                item_data new_item = get_reward_item(_user, reward_iter->reward_list[i].id, reward_iter->reward_list[i].count);
                item_list.push_back(new_item);
            }
        }
    }

    asset stage_reward_money(0, symbol(symbol_code("UTG"), 4));
    uint64_t random_money = safeseed::get_random_value(_seed, reward_iter->reward_utg_max + 1, reward_iter->reward_utg_min, 0);
    stage_reward_money.amount = random_money * 10000;

    action(permission_level{_self, "active"_n},
           _self, "transfer"_n,
           std::make_tuple(_self, _user, stage_reward_money, std::string("stage reward")))
        .send();

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Win Reward : Empty Log Table / Not Yet Signup");
    user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
        update_log.last_stage_num = _stage_number;
        update_log.battle_count += 1;
    });

    std::vector<std::string> reward;

    battle_reward_list battle_reward_list_table(_self, _self.value);
    auto user_battle_reward_iter = battle_reward_list_table.find(_user.value);
    if (user_battle_reward_iter == battle_reward_list_table.end())
    {
        battle_reward_list_table.emplace(_self, [&](auto &set_reward) {
            set_reward.user = _user;
            set_reward.reward_money = stage_reward_money.amount;
            set_reward.get_rank_exp = get_rank_exp;
            set_reward.get_char_exp_list = get_char_exp_list;
            if (servant_list.size() != 0)
            {
                set_reward.get_servant_list = servant_list;
            }
            if (monster_list.size() != 0)
            {
                set_reward.get_monster_list = monster_list;
            }
            if (equipment_list.size() != 0)
            {
                set_reward.get_equipment_list = equipment_list;
            }
            if (item_list.size() != 0)
            {
                set_reward.get_item_list = item_list;
            }
        });
    }
    else
    {
        battle_reward_list_table.modify(user_battle_reward_iter, _self, [&](auto &set_reward) {
            set_reward.get_char_exp_list.clear();
            set_reward.reward_money = stage_reward_money.amount;
            set_reward.get_rank_exp = get_rank_exp;
            set_reward.get_char_exp_list = get_char_exp_list;
            if (servant_list.size() != 0)
            {
                set_reward.get_servant_list = servant_list;
            }
            if (monster_list.size() != 0)
            {
                set_reward.get_monster_list = monster_list;
            }
            if (equipment_list.size() != 0)
            {
                set_reward.get_equipment_list = equipment_list;
            }
            if (item_list.size() != 0)
            {
                set_reward.get_item_list = item_list;
            }
        });
    }

    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "battleaction"_n,
    //        std::make_tuple(_user, std::string("stagewin"), reward))
    //     .send();
}

void battletest::fail_reward(eosio::name _user, uint64_t _stage_number)
{
    stage_db stage_db_table(_self, _self.value);
    auto stage_db_iter = stage_db_table.find(_stage_number);
    eosio_assert(stage_db_iter != stage_db_table.end(), "Fail Reward : Empty Stage ID / Wrong Stage ID");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Fail Reward : Empty Auth Table / Not Yet Signup");
    user_auth_table.modify(user_auth_iter, _self, [&](auto &user_state) {
        user_state.state = user_state::lobby;
    });

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Fail Reward : Empty Log Table / Not Yet Signup");
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
            exp_info user_exp;
            set_reward.get_rank_exp = user_exp;
            set_reward.get_char_exp_list.clear();
            set_reward.get_servant_list.clear();
            set_reward.get_monster_list.clear();
            set_reward.get_item_list.clear();
        });
    }
    else
    {
        battle_reward_list_table.modify(battle_reward_iter, _self, [&](auto &set_reward) {
            set_reward.reward_money = 0;
            exp_info user_exp;
            set_reward.get_rank_exp = user_exp;
            set_reward.get_char_exp_list.clear();
            set_reward.get_servant_list.clear();
            set_reward.get_monster_list.clear();
            set_reward.get_item_list.clear();
        });
    }
    std::vector<std::string> reward;

    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "battleaction"_n,
    //        std::make_tuple(_user, std::string("stagelose"), reward))
    //     .send();
}

ACTION battletest::stageexit(eosio::name _user)
{
    require_auth(_user);

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Stage Exit : Empty Auth Table / Not Yet Signup");
    eosio_assert(user_auth_iter->state == user_state::stage, "Stage Exit : Already End Stage");
    user_auth_table.modify(user_auth_iter, _self, [&](auto &update_user) {
        update_user.state = user_state::lobby;
    });

    battle_state_list battle_state_list_table(_self, _self.value);
    auto user_battle_state_iter = battle_state_list_table.find(_user.value);
    eosio_assert(user_battle_state_iter != battle_state_list_table.end(), "Stage Exit : Empty State Table / Not Yet Stage Start");
    battle_state_list_table.modify(user_battle_state_iter, _self, [&](auto &end_battle) {
        if (end_battle.turn == 0)
        {
            end_battle.turn = 10000;
        }
        end_battle.my_state_list.clear();
        end_battle.enemy_state_list.clear();
    });

    battle_actions battle_action_table(_self, _self.value);
    auto user_battle_action_iter = battle_action_table.find(_user.value);
    eosio_assert(user_battle_action_iter != battle_action_table.end(), "Stage Exit : Empty Action Table / Not Yet Stage Start");
    battle_action_table.modify(user_battle_action_iter, _self, [&](auto &end_action) {
        end_action.character_action_list.clear();
    });

    battle_reward_list battle_reward_list_table(_self, _self.value);
    auto battle_reward_iter = battle_reward_list_table.find(_user.value);
    if (battle_reward_iter == battle_reward_list_table.end())
    {
        battle_reward_list_table.emplace(_self, [&](auto &set_reward) {
            set_reward.user = _user;
            set_reward.reward_money = 0;
            exp_info user_exp;
            set_reward.get_rank_exp = user_exp;
            set_reward.get_char_exp_list.clear();
            set_reward.get_servant_list.clear();
            set_reward.get_monster_list.clear();
            set_reward.get_item_list.clear();
        });
    }
    else
    {
        battle_reward_list_table.modify(battle_reward_iter, _self, [&](auto &set_reward) {
            set_reward.reward_money = 0;
            exp_info user_exp;
            set_reward.get_rank_exp = user_exp;
            set_reward.get_char_exp_list.clear();
            set_reward.get_servant_list.clear();
            set_reward.get_monster_list.clear();
            set_reward.get_item_list.clear();
        });
    }
}

#pragma endregion

#pragma region tower_system

ACTION battletest::testsnap(eosio::name _user)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);
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
    user_auths user_auth_table(_self, _self.value);
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

void battletest::deletebattle(eosio::name _user)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

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
    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Not Exist User");
    user_auth_table.modify(user_auth_iter, _self, [&](auto &update_user) {
        update_user.state = user_state::lobby;
    });
}

ACTION battletest::deleteuser(eosio::name _user)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    user_auths pre_user_table(_self, _self.value);
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

    user_items user_consum_item_table(_self, _user.value);
    auto user_consum_item_iter = user_consum_item_table.begin();
    if (user_consum_item_iter != user_consum_item_table.end())
    {
        for (auto consumable = user_consum_item_table.begin(); consumable != user_consum_item_table.end();)
        {
            auto iter = user_consum_item_table.find(consumable->primary_key());
            consumable = user_consum_item_table.erase(iter);
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
    if (mail_db_iter != mail_db_table.end())
    {
        for (auto iter = mail_db_table.begin(); iter != mail_db_table.end();)
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

ACTION battletest::alluserdel()
{
    require_auth(_self);
    user_auths user_auth_table(_self, _self.value);
    for (auto iter = user_auth_table.begin(); iter != user_auth_table.end();)
    {
        auto iter_2 = user_auth_table.find(iter->primary_key());
        //deletebattle(iter_2->user);
        iter++;
        deletebattle(iter_2->user);
        deleteuser(iter_2->user);
    }
}

ACTION battletest::changetoken(eosio::name _user, std::string _type, uint64_t _index)
{
    require_auth("epochchasert"_n);
    if (_type == "servant")
    {
        user_servants user_servant_table(_self, _user.value);
        auto servant_iter = user_servant_table.find(_index);
        eosio_assert(servant_iter != user_servant_table.end(), "Not Exist Servant 20");
        user_servant_table.modify(servant_iter, _self, [&](auto &new_token) {
            new_token.servant.state = object_state::on_tokenization;
        });
    }
    else if (_type == "monster")
    {
        user_monsters user_monster_table(_self, _user.value);
        auto monster_iter = user_monster_table.find(_index);
        eosio_assert(monster_iter != user_monster_table.end(), "Not Exist Monster 20");
        user_monster_table.modify(monster_iter, _self, [&](auto &new_token) {
            new_token.monster.state = object_state::on_tokenization;
        });
    }
    else if (_type == "equipment")
    {
        user_equip_items user_equipment_table(_self, _user.value);
        auto equipment_iter = user_equipment_table.find(_index);
        eosio_assert(equipment_iter != user_equipment_table.end(), "Not Exist Monster 20");
        user_equipment_table.modify(equipment_iter, _self, [&](auto &new_token) {
            new_token.equipment.state = object_state::on_tokenization;
        });
    }
    else
    {
        eosio_assert(1 == 0, "Wrong Type Token");
    }
}

#pragma endregion

#pragma region sell contents function
//------------------------------------------------------------------------//
//-------------------------sell_function------------------------//
//------------------------------------------------------------------------//

ACTION battletest::servantburn(eosio::name _user, const std::vector<uint64_t> &_servant_list)
{
    require_auth(_user);
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), " servantburn : black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "servantburn : Server Pause");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);

    user_items user_items_table(_self, _user.value);
    user_servants user_servant_table(_self, _user.value);
    burnitem_db burnitem_db_table(_self, _self.value);
    servant_db servant_db_table(_self,_self.value);

    std::string contents_list;
    contents_list += "[";
    uint64_t check_item_count = 0;
    uint64_t check_inventory = 0;
    uint64_t original_inventory = 0;
    item_info items;

    for(uint32_t i = 0; i < _servant_list.size(); ++i)
    {
        auto user_servant_iter = user_servant_table.find(_servant_list[i]);
        eosio_assert(user_servant_iter != user_servant_table.end(), "servantburn : Empty servant info");
        eosio_assert(user_servant_iter->party_number == 0, "servantburn : this servant already in party");
        auto servant_db_iter = servant_db_table.find(user_servant_iter->servant.id);
        auto burnitem_db_iter = burnitem_db_table.find(servant_db_iter->job);
        auto user_items_iter = user_items_table.find(burnitem_db_iter->result_item_id);
        eosio_assert(user_auth_iter->current_servant_inventory >= 0, "servantburn : current_servant_inventory underflow error");

        if (user_servant_iter->servant.state == object_state::on_inventory)
        {
            if (user_items_iter == user_items_table.end())
            {
                user_items_table.emplace(_self, [&](auto &change_consumable) {
                    change_consumable.id = burnitem_db_iter->result_item_id;
                    items.index =0; 
                    change_consumable.type = 0;
                    if (user_servant_iter->servant.exp >= 960400)
                    {
                        items.count = 5;
                        change_consumable.item_list.push_back(items);
                        check_item_count = 5;
                    }
                    else
                    {
                        items.count = 1;
                        change_consumable.item_list.push_back(items);
                        check_item_count =1;
                    }
                });
                
                check_inventory += 1;
                user_servant_table.erase(user_servant_iter);

                contents_list += to_string(user_servant_iter->index) + ":";
                contents_list += to_string(user_servant_iter->servant.id) + ":";
                contents_list += to_string(user_items_iter->id) + ":";
                contents_list += to_string(user_items_iter->type) + ":";
                contents_list += to_string(check_item_count) + "]";
            }
            else
            {
                user_items_table.modify(user_items_iter, _self, [&](auto &change_consumable) {
                    if (user_servant_iter->servant.exp >= 960400)
                    {
                        uint64_t count_diff = 5;
                        for(uint64_t i = 0; i < change_consumable.item_list.size() ; i++)
                        {
                            if((change_consumable.item_list[i].count + count_diff) > 99)
                            {
                                count_diff = (change_consumable.item_list[i].count + count_diff) - 99;
                                change_consumable.item_list[i].count = 99;
                            }
                            else
                            {
                                change_consumable.item_list[i].count += count_diff;
                                count_diff = 0;
                                break;
                            }
                        }
                        if (count_diff != 0)
                        {
                            items.index = change_consumable.item_list.size();
                            items.count += count_diff;
                            change_consumable.item_list.push_back(items);                            
                        }
                        check_item_count = 5;
                    }
                    else
                    {
                        uint64_t count_diff = 1;
                        for(uint64_t i = 0; i < change_consumable.item_list.size() ; i++)
                        {
                            if((change_consumable.item_list[i].count + count_diff) > 99)
                            {
                                count_diff = (change_consumable.item_list[i].count + count_diff) - 99;
                                change_consumable.item_list[i].count = 99;
                            }
                            else
                            {
                                change_consumable.item_list[i].count += count_diff;
                                count_diff = 0;
                                break;
                            }
                        }
                        check_item_count =1;
                        if (count_diff != 0)
                        {
                            items.index = change_consumable.item_list.size();
                            items.count += count_diff;                          
                            change_consumable.item_list.push_back(items);
                            check_inventory +=1;
                        }
                    }
                    contents_list += to_string(user_servant_iter->index) + ":";
                    contents_list += to_string(user_servant_iter->servant.id) + ":";
                    contents_list += to_string(user_items_iter->id) + ":";
                    contents_list += to_string(user_items_iter->type) + ":";
                    contents_list += to_string(check_item_count) + "]";
                });

                user_servant_table.erase(user_servant_iter);
            }
        }

        else
        {
            eosio_assert(1 == 0, "servantburn : this servant is not inventory state");
        }
    }
    user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
        eosio_assert(user_auth_iter->current_item_inventory >= 0, "servantburn : current_item_inventory underflow error");
        if (change_auth_user.current_servant_inventory <= _servant_list.size())
        {
            change_auth_user.current_servant_inventory = 0;
        }
        else
        {
            change_auth_user.current_servant_inventory -= _servant_list.size();
        }
        change_auth_user.current_item_inventory += check_inventory;
    });

        std::string contents_type = "servantburn";
        action(permission_level{get_self(), "active"_n},
               get_self(), "contentslist"_n,
               std::make_tuple(_user ,contents_type ,contents_list))
            .send();
}


ACTION battletest::monsterburn(eosio::name _user, const std::vector<uint64_t> &_monster_list)
{
    require_auth(_user);

    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "monsterburn : black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "monsterburn : Server Pause");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);

    user_monsters user_monster_table(_self, _user.value);

    asset monster_sell_result(0, symbol(symbol_code("UTG"), 4));

    std::string contents_list;
    contents_list += "[";

    for (uint32_t i = 0; i < _monster_list.size(); ++i)
    {
        auto user_monster_iter = user_monster_table.find(_monster_list[i]);
        eosio_assert(user_monster_iter != user_monster_table.end(), "monsterburn : not exist monster info");
        eosio_assert(user_monster_iter->party_number == 0, "monsterburn : this monster already in party");

        if (user_monster_iter->monster.state == object_state::on_inventory)
        {
            if (user_monster_iter->monster.exp >= 960400)
            {
                monster_sell_result.amount += 1000000;
            }
            else
            {
                monster_sell_result.amount += 50000;
            }

            user_monster_table.erase(user_monster_iter);

            contents_list += to_string(user_monster_iter->index) + ":";
            contents_list += to_string(user_monster_iter->monster.id) + "]";
        }
        else
        {
            eosio_assert(1 == 0, "monsterburn : this monster is not inventory state");
        }
    }

    action(permission_level{get_self(), "active"_n},
           get_self(), "transfer"_n,
           std::make_tuple(_self, _user, monster_sell_result, std::string("monster sell result")))
        .send();

    user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
        if (change_auth_user.current_monster_inventory <= _monster_list.size())
        {
            change_auth_user.current_monster_inventory = 0;
        }
        else
        {
            change_auth_user.current_monster_inventory -= _monster_list.size();
        }
    });

    std::string contents_type = "monsterburn";
    action(permission_level{get_self(), "active"_n},
           get_self(), "contentslist"_n,
           std::make_tuple(_user, contents_type, contents_list))
        .send();
}

ACTION battletest::equipburn(eosio::name _user, const std::vector<uint64_t> &_equipment_list)
{
    require_auth(_user);
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "equipburn : black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "equipburn : Server Pause");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);

    user_equip_items user_equipment_table(_self, _user.value);

    asset equipment_sell_result(0, symbol(symbol_code("UTG"), 4));

    std::string contents_list;
    contents_list += "[";

    for (uint32_t i = 0; i < _equipment_list.size(); ++i)
    {
        auto user_equipment_iter = user_equipment_table.find(_equipment_list[i]);
        eosio_assert(user_equipment_iter != user_equipment_table.end(), "equipburn : not exist equipment info");
        eosio_assert(user_equipment_iter->equipment.state != object_state::on_equip_slot, "equipburn : this equipment is on equip");

        if (user_equipment_iter->equipment.state == object_state::on_inventory)
        {
            if (user_equipment_iter->equipment.grade == 1)
            {
                equipment_sell_result.amount += 500000;
            }
            else if (user_equipment_iter->equipment.grade == 2)
            {
                equipment_sell_result.amount += 150000;
            }
            else if (user_equipment_iter->equipment.grade == 3)
            {
                equipment_sell_result.amount += 100000;
            }
            else if (user_equipment_iter->equipment.grade == 4)
            {
                equipment_sell_result.amount += 70000;
            }
            else if (user_equipment_iter->equipment.grade == 5)
            {
                equipment_sell_result.amount += 50000;
            }
            else
            {
               eosio_assert(1 == 0 ,"equipburn : Wrong Type equipment grade");
            }
           
            contents_list += to_string(user_equipment_iter->index) + ":";
            contents_list += to_string(user_equipment_iter->equipment.id) + ":";
            contents_list += to_string(user_equipment_iter->equipment.grade) + "]";

            user_equipment_table.erase(user_equipment_iter);
        }
        else
        {
            eosio_assert(1 == 0, "equipburn : this equipment is not inventory state");
        }
    }

    action(permission_level{get_self(), "active"_n},
           get_self(), "transfer"_n,
           std::make_tuple(_self, _user, equipment_sell_result, std::string("equipment sell result")))
        .send();

    user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
        if (change_auth_user.current_equipment_inventory <= _equipment_list.size())
        {
            change_auth_user.current_equipment_inventory = 0;
        }
        else
        {
            change_auth_user.current_equipment_inventory -= _equipment_list.size();
        }
    });

    std::string contents_type = "equipmentburn";
    action(permission_level{get_self(), "active"_n},
           get_self(), "contentslist"_n,
           std::make_tuple(_user, contents_type, contents_list))
        .send();
}

ACTION battletest::itemburn(eosio::name _user, const std::vector<uint64_t> &_item_list, const std::vector<uint64_t> &_count_list)
{
    require_auth(_user);
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "itemburn : black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "itemburn : Server Pause");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    user_items user_item_table(_self, _user.value);

    asset item_sell_result(0, symbol(symbol_code("UTG"), 4));

    std::string contents_list;
    contents_list += "[";
    uint64_t check_count = 0;
    uint64_t check_inventory = 0;

    for (uint32_t i = 0; i < _item_list.size(); ++i)
    {
        item_info items;
        auto user_item_iter = user_item_table.find(_item_list[i]);
        eosio_assert(user_item_iter != user_item_table.end(), "itemburn : not exist item info");
        eosio_assert(_item_list.size() == _count_list.size(), "itemburn : The lengths of the two do not match");

        user_item_table.modify(user_item_iter, _self, [&](auto &change_user_item) {
            uint64_t temp_count = _count_list[i];
            uint64_t temp_sum;

            for (uint64_t h = 0; h < change_user_item.item_list.size(); h++)
            {
                temp_sum += change_user_item.item_list[h].count;
            }
            eosio_assert(temp_sum >= _count_list[i], "itemburn : not sell item count");

            auto item_iter = change_user_item.item_list.end() - 1;
            
            for (uint64_t k = change_user_item.item_list.size() - 1; k >= 0; k--)
            {
                if (change_user_item.item_list[k].count < temp_count)
                {                    
                    temp_count = (temp_count - change_user_item.item_list[k].count);
                    change_user_item.item_list[k].count = 0;
                    change_user_item.item_list.erase(item_iter);
                    check_inventory +=1;
                }
                else
                {
                    change_user_item.item_list[k].count -= temp_count;
                    if (change_user_item.item_list[k].count == 0)
                    {
                        change_user_item.item_list.erase(item_iter);
                        check_inventory +=1;
                        item_iter--;
                    }
                    break;
                }
            }
            check_count += temp_count;
        });
        item_sell_result.amount += 10000 * _count_list[i];

        contents_list += to_string(user_item_iter->id) + ":";
        contents_list += to_string(user_item_iter->type) + ":";
        contents_list += to_string(check_count) + "]";
    }

    action(permission_level{get_self(), "active"_n},
           get_self(), "transfer"_n,
           std::make_tuple(_self, _user, item_sell_result, std::string("item sell result")))
        .send();

    user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
        if (change_auth_user.current_item_inventory <= _count_list.size())
        {
            change_auth_user.current_item_inventory = 0;
        }
        else
        {
            change_auth_user.current_item_inventory -= check_inventory; 
        }
    });

    std::string contents_type = "itemburn";  
    action(permission_level{get_self(), "active"_n},
           get_self(), "contentslist"_n,
           std::make_tuple(_user, contents_type, contents_list))
        .send();
}
#pragma endregion

#pragma region item equipment function
//------------------------------------------------------------------------//
//-------------------------item_equipment_function------------------------//
//------------------------------------------------------------------------//

ACTION battletest::equip(eosio::name _user, uint32_t _servant_index, uint32_t _item_index)
{
    require_auth(_user);
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "equip : black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "equip : Server Pause");

    user_equip_items user_equip_item_table(_self, _user.value);
    auto user_equip_item_iter = user_equip_item_table.find(_item_index);
    user_servants user_servant_table(_self, _user.value);
    auto user_servant_iter = user_servant_table.find(_servant_index);
    servant_db servant_db_table(_self, _self.value);
    auto servant_db_iter = servant_db_table.find(user_servant_iter->servant.id);
    equipment_db equipment_db_table(_self,_self.value);
    auto equipment_db_iter = equipment_db_table.find(user_equip_item_iter->equipment.id);


    eosio_assert(user_servant_iter != user_servant_table.end(), "equip : not exist servant info");
    eosio_assert(user_equip_item_iter != user_equip_item_table.end(), "equip : not exist item info1");
    eosio_assert(user_equip_item_iter->equipment.state != object_state::on_equip_slot, "equip : already equip this item");

    eosio_assert(compare_item(servant_db_iter->job, user_equip_item_iter->equipment.job), "equip : this item is not equipment");  //직업 비교 

    uint32_t slot;
    slot = user_equip_item_iter->equipment.type;

    if (user_servant_iter->servant.state == object_state::on_inventory && user_equip_item_iter->equipment.state == object_state::on_inventory)  
    {
        if (user_equip_item_iter->equipment.tier == 1) //모든 서번트 레벨 상관없이 착용 가능
        {
            status_info item_status;
            if (user_servant_iter->servant.equip_slot[slot] != 0) //기존에 아이템 착용 상태 시
            {
                user_equip_items user_equip_item_table2(_self, _user.value);
                auto user_equip_item_iter2 = user_equip_item_table2.find(user_servant_iter->servant.equip_slot[slot]);
                user_equip_item_table2.modify(user_equip_item_iter2, _self, [&](auto &unequip_item) {
                    unequip_item.equipment.state = object_state::on_inventory;
                    unequip_item.equipment.equipservantindex = 0;
                });

                user_equip_item_table.modify(user_equip_item_iter, _self, [&](auto &equip_item) {
                    equip_item.equipment.state = object_state::on_equip_slot;
                    equip_item.equipment.equipservantindex = user_servant_iter->index;
                });

                user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant) {
                    unequip_servant.servant.equip_slot[slot] = _item_index;
                });
            }
            else
            {
                user_equip_item_table.modify(user_equip_item_iter, _self, [&](auto &equip_item) {
                    equip_item.equipment.state = object_state::on_equip_slot;
                    equip_item.equipment.equipservantindex = user_servant_iter->index;
                });

                user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant) {
                    unequip_servant.servant.equip_slot[slot] = _item_index;
                });
            }
        }
        else if (user_equip_item_iter->equipment.tier >= 2)
        {
            if (user_servant_iter->servant.level >= user_equip_item_iter->equipment.tier * 10)
            {
                status_info item_status;
                if (user_servant_iter->servant.equip_slot[slot] != 0) //기존에 아이템 착용 상태 시
                {
                    user_equip_items user_equip_item_table2(_self, _user.value);
                    auto user_equip_item_iter2 = user_equip_item_table2.find(user_servant_iter->servant.equip_slot[slot]);
                    user_equip_item_table2.modify(user_equip_item_iter2, _self, [&](auto &unequip_item) {
                        unequip_item.equipment.state = object_state::on_inventory;
                        unequip_item.equipment.equipservantindex = 0;
                    });

                    user_equip_item_table.modify(user_equip_item_iter, _self, [&](auto &equip_item) {
                        equip_item.equipment.state = object_state::on_equip_slot;
                        equip_item.equipment.equipservantindex = user_servant_iter->index;
                    });

                    user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant) {
                        unequip_servant.servant.equip_slot[slot] = _item_index;
                    });
                }
                else
                {
                    user_equip_item_table.modify(user_equip_item_iter, _self, [&](auto &equip_item) {
                        equip_item.equipment.state = object_state::on_equip_slot;
                        equip_item.equipment.equipservantindex = user_servant_iter->index;
                    });

                    user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant) {
                        unequip_servant.servant.equip_slot[slot] = _item_index;
                    });
                }
            }
            else //티어에 비해 레벨이 낮은 경우
            {
                eosio_assert(0 == 1, "equip : There is not enough level.");
            }
        }
        else
        {
          eosio_assert(1 == 0, "equip : this is not equip tier item");
        }        
    }
    else
    {
        eosio_assert(1 == 0, "equip : this servant, equipment is not inventory state");
    }

    std::string contents_result;
    std::string contents_type = "equip";
    contents_result +="[";
    contents_result += to_string(user_servant_iter->index) + ":";
    contents_result += to_string(user_servant_iter->servant.id) + ":";
    contents_result += to_string(user_servant_iter->servant.equip_slot[slot]) + ":";
    contents_result += to_string(user_equip_item_iter->index) + ":";
    contents_result += to_string(user_equip_item_iter->equipment.id) + ":";
    contents_result += to_string(user_equip_item_iter->equipment.state) + ":";
    contents_result += to_string(user_equip_item_iter->equipment.equipservantindex) + "]";

    action(permission_level{get_self(), "active"_n},
           get_self(), "contents"_n,
           std::make_tuple(_user, contents_type, contents_result))
        .send();    
    
}

ACTION battletest::unequip(eosio::name _user, uint32_t _servant_index, uint32_t _item_index)
{
    require_auth(_user);
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "unequip : black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "unequip : Server Pause");

    user_equip_items user_equip_item_table(_self, _user.value);
    auto user_equip_item_iter = user_equip_item_table.find(_item_index);
    eosio_assert(user_equip_item_iter != user_equip_item_table.end(), "unequip : not exist item info");
    eosio_assert(user_equip_item_iter->equipment.state == object_state::on_equip_slot, "unequip : already unequip this item");

    uint32_t slot = user_equip_item_iter->equipment.type - 1;
    user_equip_item_table.modify(user_equip_item_iter, _self, [&](auto &unequip_item) {
        unequip_item.equipment.state = object_state::on_inventory;
        unequip_item.equipment.equipservantindex = 0;
    });

    user_servants user_servant_table(_self, _user.value);
    auto user_servant_iter = user_servant_table.find(_servant_index);
    eosio_assert(user_servant_iter != user_servant_table.end(), "unequip : not exist servant info");

    user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant) {
        unequip_servant.servant.equip_slot[slot] = 0;
    });

    std::string contents_result;
    std::string contents_type = "unequip";
    contents_result += "[";
    contents_result += to_string(user_servant_iter->index) + ":";
    contents_result += to_string(user_servant_iter->servant.id) + ":";
    contents_result += to_string(user_servant_iter->servant.equip_slot[slot]) + ":";
    contents_result += to_string(user_equip_item_iter->index) + ":";
    contents_result += to_string(user_equip_item_iter->equipment.id) + ":";
    contents_result += to_string(user_equip_item_iter->equipment.state) + ":";
    contents_result += to_string(user_equip_item_iter->equipment.equipservantindex) +"]";
    action(permission_level{get_self(), "active"_n},
           get_self(), "contents"_n,
           std::make_tuple(_user, contents_type, contents_result))
        .send();  

}


bool battletest::compare_item(uint32_t _user_servant, uint32_t _user_item)
{
    uint32_t compare = item_in[_user_servant];
    uint32_t bit_magic = compare & _user_item;

    if (_user_item == 0)
    {
        return false;
    }
    else if (_user_item == 1)
    {
        return true;
    }
    if (bit_magic == compare)
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

ACTION battletest::equipmentup(eosio::name _user, uint32_t _equipment, const std::vector<uint64_t> &_item_list)
{
    require_auth(_user);
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "equipmentup : black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "equipmentup : Server Pause");

    uint64_t equipment_type, equipment_grade, equipment_upgrade, db_ratio;

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);

    user_equip_items user_equip_item_table(_self, _user.value);
    auto user_equipment_iter = user_equip_item_table.find(_equipment);

    equipment_type = user_equipment_iter->equipment.type + 1;
    equipment_grade = user_equipment_iter->equipment.grade;
    equipment_upgrade = user_equipment_iter->equipment.upgrade;
    uint64_t sum = ((equipment_type * 10) + (equipment_grade * 1));
    upgrade_equipment_ratio_dbs upgrade_equipment_ratio_db_table(_self, _self.value);
    auto user_upgrade_equipment_iter = upgrade_equipment_ratio_db_table.find(sum);

    db_ratio = user_upgrade_equipment_iter->upgrade_ratio[equipment_upgrade];

    eosio_assert(user_equipment_iter != user_equip_item_table.end(), "equipmentup : not exist equipment info");
    eosio_assert(user_equipment_iter->equipment.state == object_state::on_inventory , "equipmentup : this item already in Wearing");
    eosio_assert(user_equipment_iter->equipment.upgrade != 9, "equipmentup : this item upgrade is MAX");

    user_items user_item_table(_self, _user.value);
    uint64_t temp_count =0;
    uint64_t check_inventory = 0;
    uint64_t check_total_item =0;

    for(uint32_t i=0; i< _item_list.size(); i++)
    {
        auto user_item_iter = user_item_table.find(_item_list[i]);  
        check_total_item += user_item_iter->item_list[i].count;
    }

    for (uint32_t i = 0; i < _item_list.size(); ++i)
    {
        item_info items;
        auto user_item_iter = user_item_table.find(_item_list[i]);        

        eosio_assert(user_item_iter != user_item_table.end(), "equipmentup : not exist consumables info");
        eosio_assert(user_item_iter->id == user_upgrade_equipment_iter->material_id, "equipmentup : upgrade sub item no match");
        eosio_assert(check_total_item >= user_upgrade_equipment_iter->material_count[equipment_upgrade], "equipmentup : not enough material count");

        uint64_t l_seed = safeseed::get_seed_value(_user.value, now());
        uint64_t upgrade_ratio = safeseed::get_random_value(l_seed, ITEM_UPGRADE_RATE, DEFAULT_MIN, DEFAULT_RANDOM_COUNT);
        uint64_t updatecheck = 0;

        user_equip_item_table.modify(user_equipment_iter, _self, [&](auto &upgrade_item) {
            if (db_ratio >= upgrade_ratio) // 성공 확률
            {
                upgrade_item.equipment.upgrade += 1;
                updatecheck = 1;
            }
        });

        user_item_table.modify(user_item_iter, _self, [&](auto &change_user_item) {
            uint64_t temp_count = user_upgrade_equipment_iter->material_count[equipment_upgrade];
            uint64_t temp_sum;

            auto item_iter = change_user_item.item_list.end() - 1;

            for (uint64_t k = change_user_item.item_list.size() - 1; k >= 0; k--)
            {
                if (change_user_item.item_list[k].count < temp_count)
                {
                    temp_count = (temp_count - change_user_item.item_list[k].count);
                    change_user_item.item_list[k].count = 0;
                    change_user_item.item_list.erase(item_iter);
                    check_inventory += 1;
                }
                else
                {
                    change_user_item.item_list[k].count -= temp_count;
                    if (change_user_item.item_list[k].count == 0)
                    {
                        change_user_item.item_list.erase(item_iter);
                        check_inventory += 1;
                        item_iter--;
                    }
                    break;
                }
            }
        });

        if (updatecheck == 0) //실패시
        {
            user_equip_item_table.erase(user_equipment_iter);
            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                eosio_assert(user_auth_iter->current_item_inventory >= 0, "equipmentup : current_item_inventory underflow error");
                change_auth_user.current_equipment_inventory -= 1;
                change_auth_user.current_item_inventory -= check_inventory;
            });
        }
    }


    asset upgrade_use_UTG_result(0, symbol(symbol_code("UTG"), 4));
    upgrade_use_UTG_result.amount = user_upgrade_equipment_iter->use_UTG[equipment_upgrade] *10000; 

    transfer(_user, _self, upgrade_use_UTG_result, std::string("upgrade use UTG result"));

    std::string contents_result;
    std::string contents_type = "equipmentup";
    contents_result += "[";
    contents_result += to_string(user_equipment_iter->index) + ":";
    contents_result += to_string(user_equipment_iter->equipment.id) + ":";
    contents_result += to_string(user_equipment_iter->equipment.upgrade) + ":";
    //contents_result += to_string(user_item_iter->id) + ":";
    //contents_result += to_string(user_item_iter->type) + ":";
    contents_result += to_string(temp_count)+ "]";
    action(permission_level{get_self(), "active"_n},
           get_self(), "contents"_n,
           std::make_tuple(_user, contents_type, contents_result))
        .send();  


}

ACTION battletest::monsterup(eosio::name _user, uint32_t _monster, uint32_t _monster2)
{
    require_auth(_user);
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "monsterup : black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "monsterup : Server Pause");
    
    user_monsters user_monster_table(_self, _user.value);
    auto user_monster_iter = user_monster_table.find(_monster);

    user_monsters second_user_monster_table(_self, _user.value);
    auto user_monster_iter2 = second_user_monster_table.find(_monster2);

    uint64_t db_ratio = 0;
    uint64_t find_sum_upgrade_iter = user_monster_iter->monster.grade * 10 + user_monster_iter->monster.upgrade;
    uint64_t find_sum_lv_iter = user_monster_iter->monster.monster_class * 10 + user_monster_iter->monster.grade;

    upgrade_monster_ratio_db upgrade_monsterratio_db_table(_self, _self.value);
    auto user_upgrade_monster_iter = upgrade_monsterratio_db_table.find(find_sum_upgrade_iter);

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);

   
    db_ratio = user_upgrade_monster_iter->sub[user_monster_iter2->monster.upgrade].ratio;

    eosio_assert(user_monster_iter != user_monster_table.end(), "monsterup : not exist monster info");
    eosio_assert(user_monster_iter2 != second_user_monster_table.end(), "monsterup : not exist monster info");

    eosio_assert((user_monster_iter->party_number == 0 || user_monster_iter2->party_number == 0), "monsterup : this monster already in party");
    eosio_assert((user_monster_iter->monster.upgrade != 9 || user_monster_iter2->monster.upgrade != 9), "monsterup : this monster upgrade is MAX");
    eosio_assert(user_monster_iter->index != user_monster_iter2->index, "monsterup : same monster not use");

    uint64_t l_seed = safeseed::get_seed_value(_user.value, now());
    uint64_t upgrade_ratio = safeseed::get_random_value(l_seed, MONSTER_UPGRADE_RATE, DEFAULT_MIN, DEFAULT_RANDOM_COUNT);
    uint64_t fail_upgrade = safeseed::get_random_value(l_seed, user_monster_iter->monster.upgrade + 1, DEFAULT_MIN, DEFAULT_RANDOM_COUNT);

    eosio_assert(user_monster_iter->monster.grade == user_monster_iter2->monster.grade, "monsterup : sub monster grade is high by main monster");       //메인보다 서브가 높을 수 없다.
    eosio_assert(user_monster_iter->monster.upgrade >= user_monster_iter2->monster.upgrade, "monsterup : sub monster upgrade is high by main monster"); //메인보다 서브가 높을 수 없다.

    if (db_ratio >= upgrade_ratio) // 성공 확률
    {
        user_monster_table.modify(user_monster_iter, _self, [&](auto &upgrade_monster) {
            upgrade_monster.monster.upgrade += 1;
        });
        user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.current_monster_inventory -= 1;
        });
    }
    else //실패
    {
        user_monster_table.modify(user_monster_iter, _self, [&](auto &upgrade_monster) {
            upgrade_monster.monster.upgrade = fail_upgrade;
        });
        user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.current_monster_inventory -= 2;
        });
    }
    second_user_monster_table.erase(user_monster_iter2);

    std::string contents_result;
    std::string contents_type = "monsterup";
    contents_result += "[";
    contents_result += to_string(user_monster_iter->index) + ":";
    contents_result += to_string(user_monster_iter->monster.id) + ":";
    contents_result += to_string(user_monster_iter->monster.upgrade) + "]";
    action(permission_level{get_self(), "active"_n},
           get_self(), "contents"_n,
           std::make_tuple(_user, contents_type, contents_result))
        .send();  

}


#pragma endregion

#pragma region store function
//------------------------------------------------------------------------//
//-------------------------------store_function---------------------------//
//------------------------------------------------------------------------//

ACTION battletest::itembuy(eosio::name _user, uint32_t _item_id, uint32_t _count)
{
    require_auth(_user);
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "itembuy : black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "itembuy : Server Pause");

    item_shop item_shop_table(_self, _self.value);
    auto item_shop_iter = item_shop_table.find(_item_id);
    eosio_assert(item_shop_iter != item_shop_table.end(), "itembuy : Not exist item shop data");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "itembuy : Not exist user_auths data");

    user_items user_items_table(_self, _user.value);
    auto user_items_iter = user_items_table.find(item_shop_iter->id);

    item_info items;
    uint64_t count_diff = _count;
    uint64_t check_inventory = 0; 
    uint64_t original_inventory = 0;
    if (user_items_iter == user_items_table.end())
    {
        user_items_table.emplace(_self, [&](auto &change_consumable) {
            change_consumable.id = item_shop_iter->id;
            change_consumable.type = item_shop_iter->goods_type;

            uint64_t sub_size = _count / 99;

            for (uint64_t i = 0; i <= sub_size; i++)
            {
                if (count_diff > 99)
                {
                    change_consumable.item_list[i].count = 99;

                    items.index = change_consumable.item_list.size();
                    items.count = 99;
                    change_consumable.item_list.push_back(items);
                    check_inventory += 1;
                    count_diff -= 99;
                }
                else
                {
                    change_consumable.item_list[i].count = count_diff;
                    items.index = change_consumable.item_list.size();
                    items.count = count_diff;
                    change_consumable.item_list.push_back(items);
                    check_inventory += 1;
                }
            }
        });
        
    }
    else
    {
        user_items_table.modify(user_items_iter, _self, [&](auto &change_consumable) {
            change_consumable.type = item_shop_iter->goods_type;

            for (uint64_t i = 0; i < change_consumable.item_list.size(); i++)
            {
                if ((change_consumable.item_list[i].count + count_diff) > 99)
                {
                    count_diff = (change_consumable.item_list[i].count + count_diff) - 99;
                    change_consumable.item_list[i].count = 99;                    
                }
                else
                {
                    change_consumable.item_list[i].count += count_diff;
                    count_diff = 0;
                    break;
                }
            }
            if (count_diff != 0)
            {
                items.index = change_consumable.item_list.size();
                items.count += count_diff;
                // original_inventory += items.index;
                // check_inventory = original_inventory - check_inventory;
                change_consumable.item_list.push_back(items);
                check_inventory +=1;
            }
        });
        asset nomal_order_buy_result(0, symbol(symbol_code("UTG"), 4));
        nomal_order_buy_result.amount = _count * item_shop_iter->price_count * 10000;

        transfer(_user, _self, nomal_order_buy_result, std::string("nomal order buy result"));
    }
    user_auth_table.modify(user_auth_iter, _self, [&](auto &add_auth) {
        eosio_assert(user_auth_iter->current_item_inventory >= 0, "itembuy : current_item_inventory underflow error");
        add_auth.current_item_inventory += check_inventory;
    });

    // std::string contents_result;
    // std::string contents_type = "itembuy";
    // contents_result += "[";
    // contents_result += "item_id:";
    // contents_result += to_string(user_item_iter->id) + ":";
    // contents_result += "item_type:";
    // contents_result += to_string(user_item_iter->item.type) + ":";
    // contents_result += "item_count:";
    // contents_result += to_string(user_item_iter->item.count);
    // contents_result += "]";
    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "contents"_n,
    //        std::make_tuple(_user, contents_type, contents_result))
    //     .send();  

}

void battletest::buy_inventory(eosio::name _user, uint64_t _type)
{
    require_auth(_user);
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "buyinventory : black list user");

    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "buyinventory : Server Pause");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    uint64_t plus_inventory = 5;
    if (_type == 1) //서번트 인벤토리
    {
        user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.servant_inventory += plus_inventory;
        });
    }
    else if (_type == 2) //몬스터 인벤토리
    {
        user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.monster_inventory += plus_inventory;
        });
    }
    else if (_type == 3) //장비 인벤토리
    {
        user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.equipment_inventory += plus_inventory;
        });
    }
    else if (_type == 4) //소모품 인벤토리
    {
        user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.item_inventory += plus_inventory;
        });
    }
    else
    {
        eosio_assert(_type < plus_inventory, "buyinventory : not exsit this action type");
    }

    std::string contents_result;
    std::string contents_type = "buyinventory";
    contents_result += "[";
    contents_result += to_string(_type) + ":";
    contents_result += to_string(plus_inventory) + "]";
    action(permission_level{get_self(), "active"_n},
           get_self(), "contents"_n,
           std::make_tuple(_user, contents_type, contents_result))
        .send();  

}

void battletest::nftexchange(eosio::name _owner, eosio::name _master, std::string _type, uint64_t _master_index)
{
    require_auth(_owner);
    if (_type == "servant")
    {
        if (_owner == _master)
        {
            user_servants user_servant_table(_self, _owner.value);
            auto servant_iter = user_servant_table.find(_master_index);
            eosio_assert(servant_iter != user_servant_table.end(), "nftexchange : Not Exist Servant 20");
            user_servant_table.modify(servant_iter, _self, [&](auto &new_token) {
                new_token.servant.state = object_state::on_inventory;
            });
        }
        else
        {
            user_servants master_table(_self, _master.value);
            auto master_iter = master_table.find(_master_index);
            eosio_assert(master_iter != master_table.end(), "nftexchange : Wrong Master Index 1");

            user_servants owner_table(_self, _owner.value);
            auto user_servant_iter = owner_table.find(master_iter->index);

            if (user_servant_iter == owner_table.end())
            {
                owner_table.emplace(_self, [&](auto &new_servant) {
                    uint32_t first_index = owner_table.available_primary_key();
                    if (first_index == 0)
                    {
                        new_servant.index = 1;
                    }
                    else
                    {
                        new_servant.index = owner_table.available_primary_key();
                    }
                    // new_servant.servant = master_iter->servant;
                    // new_servant.servant.state = object_state::on_inventory;
                    new_servant.servant.id = master_iter->servant.id;
                    new_servant.servant.state = object_state::on_inventory;
                    new_servant.servant.exp = master_iter->servant.exp;
                    new_servant.servant.grade = master_iter->servant.grade;
                    new_servant.servant.status.basic_str = master_iter->servant.status.basic_str;
                    new_servant.servant.status.basic_dex = master_iter->servant.status.basic_dex;
                    new_servant.servant.status.basic_int = master_iter->servant.status.basic_int;
                    new_servant.servant.equip_slot = master_iter->servant.equip_slot;
                    new_servant.servant.passive_skill = master_iter->servant.passive_skill;
                    new_servant.servant.active_skill = master_iter->servant.active_skill;
                });
            }
            else
            {
                owner_table.modify(user_servant_iter, _self, [&](auto &new_servant) {
                    new_servant.servant.id = master_iter->servant.id;
                    new_servant.servant.state = object_state::on_inventory;
                    new_servant.servant.exp = master_iter->servant.exp;
                    new_servant.servant.grade = master_iter->servant.grade;
                    new_servant.servant.status.basic_str = master_iter->servant.status.basic_str;
                    new_servant.servant.status.basic_dex = master_iter->servant.status.basic_dex;
                    new_servant.servant.status.basic_int = master_iter->servant.status.basic_int;
                    new_servant.servant.equip_slot = master_iter->servant.equip_slot;
                    new_servant.servant.passive_skill = master_iter->servant.passive_skill;
                    new_servant.servant.active_skill = master_iter->servant.active_skill;
                });
            }

            master_table.erase(master_iter);
        }
    }
    else if(_type == "monster")
    {
        if (_owner == _master)
        {
            user_monsters user_monster_table(_self, _owner.value);
            auto monster_iter = user_monster_table.find(_master_index);
            eosio_assert(monster_iter != user_monster_table.end(), "nftexchange : Not Exist Monster 20");
            user_monster_table.modify(monster_iter, _self, [&](auto &new_token) {
                new_token.monster.state = object_state::on_inventory;
            });
        }
        else
        {
            user_monsters master_table(_self, _master.value);
            auto master_iter = master_table.find(_master_index);
            eosio_assert(master_iter != master_table.end(),"nftexchange : Wrong Master Index 2");

            user_monsters owner_table(_self, _owner.value);
            owner_table.emplace(_self, [&](auto &new_monster) {
                uint32_t first_index = owner_table.available_primary_key();
                if (first_index == 0)
                {
                    new_monster.index = 1;
                }
                else
                {
                    new_monster.index = owner_table.available_primary_key();
                }
                new_monster.monster = master_iter->monster;
                new_monster.monster.state = object_state::on_inventory;
            });
        }
    }
    else if(_type == "equipment")
    {
        if (_owner == _master)
        {
            user_equip_items user_equipment_table(_self, _owner.value);
            auto equipment_iter = user_equipment_table.find(_master_index);
            eosio_assert(equipment_iter != user_equipment_table.end(), "nftexchange : Not Exist Monster 20");
            user_equipment_table.modify(equipment_iter, _self, [&](auto &new_token) {
                new_token.equipment.state = object_state::on_inventory;
            });
        }
        else
        {
            user_equip_items master_table(_self, _master.value);
            auto master_iter = master_table.find(_master_index);
            eosio_assert(master_iter != master_table.end(), "nftexchange : Wrong Master Index 3");

            user_equip_items owner_table(_self, _owner.value);
            owner_table.emplace(_self, [&](auto &new_equip) {
                uint32_t first_index = owner_table.available_primary_key();
                if (first_index == 0)
                {
                    new_equip.index = 1;
                }
                else
                {
                    new_equip.index = owner_table.available_primary_key();
                }
                new_equip.equipment = master_iter->equipment;
                new_equip.equipment.state = object_state::on_inventory;
            });
        }
    }
    else
    {
        eosio_assert(1 == 0 ,"nftexchange : Wrong Type Token");
    }
}

ACTION battletest::chat(name _user, asset _price, string _text)
{
   require_auth(_user);

   eosio_assert(_price.symbol == symbol("UTG", 4), "Only UTG can be uses.");

   // Get User's UTG balance
   account acnt_table(_self, _user.value);
   auto a_iter = acnt_table.find(_price.symbol.code().raw());

   // Get Chat table info
   chat_index chat_table(_self, _self.value);
   auto iter = chat_table.find(_self.value);

   // Set basic price
   string _symbol = "UTG";
   asset basic_price(1000000, symbol(symbol_code(_symbol), 4));

   if(iter == chat_table.end()){
       eosio_assert(a_iter->balance.amount >= basic_price.amount, "Not enough UTG.");
       eosio_assert(basic_price.amount == _price.amount, "Can not register at current price.");

       chat_table.emplace(_user, [&](auto &data){
           data.owner = _self;
           data.price = basic_price;
           data.user = _user;
           data.text = _text;
           data.start_time = now();
       });
   }
   else{
       if(iter->start_time < now() - 86400){
           eosio_assert(a_iter->balance.amount >= basic_price.amount, "Not enough UTG.");
           eosio_assert(basic_price.amount == _price.amount, "Can not register at current price.");

           chat_table.modify(iter, _user, [&](auto &data){
               data.price = basic_price;
               data.user = _user;
               data.text = _text;
               data.start_time = now();
           });
       }
       else{
           eosio_assert(a_iter->balance.amount >= _price.amount, "Not enough UTG.");
           basic_price.amount = iter->price.amount * 2;
           eosio_assert(basic_price.amount == _price.amount, "Can not register at current price.");

           chat_table.modify(iter, _user, [&](auto &data){
               data.price = _price;
               data.user = _user;
               data.text = _text;
               data.start_time = now();
           });
       }
   }

   string msg = "Regist chat message. " + std::to_string(_price.amount) + " UTG";
   transfer(_user, _self, _price, msg);
}

// ACTION battletest::change(eosio::name _user, std::string _kind, uint64_t _grade, uint64_t _id, std::string _status)
// {
//     // std::vector<std::string> value_list;
//     // std::vector<size_t> size_list;
//     // if(_kind == "servant")
//     // {
//     //     status_info status;
//     //     substr_value(_status, value_list, size_list, 5);
//     //     uint32_t index = atoi(value_list[0].c_str());
//     //     uint32_t level = atoi(value_list[1].c_str());
//     //     status.basic_str = atoll(value_list[2].c_str());
//     //     status.basic_dex = atoll(value_list[3].c_str());
//     //     status.basic_int = atoll(value_list[4].c_str());

//     //     status = get_grade_status(0, status);

//     //     user_servants user_servant_table(_self, _user.value);
//     //     auto user_servant_iter = user_servant_table.find(index);
//     //     eosio_assert(user_servant_iter != user_servant_table.end(),"Not Exist Servant 4");
//     //     user_servant_table.modify(user_servant_iter, _self, [&](auto &set_servant) {
//     //         uint32_t _job, _body, _gender, _head, _hair;
//     //         uint32_t temp1, temp2, temp3, temp4;
//     //         if (_id != 0)
//     //         {
//     //             servant_db servant_db_table(_self, _self.value);
//     //             auto servant_db_iter = servant_db_table.find(_id);
//     //             eosio_assert(servant_db_iter != servant_db_table.end(),"Not Exist Servant ID 5");
//     //             _job = _id / 1000000;
//     //             temp1 = _id % 1000000;
//     //             _body = temp1 / 100000;
//     //             temp2 = temp1 % 100000;
//     //             _gender = temp2 / 10000;
//     //             temp3 = temp2 % 10000;
//     //             _head = temp3 / 100;
//     //             temp4 = temp3 % 100;
//     //             _hair = temp4 / 1;
//     //             set_servant.servant.appear.head = _head;
//     //             set_servant.servant.appear.hair = _hair;
//     //             set_servant.servant.appear.body = _body;
//     //             set_servant.servant.appear.gender = _gender;
//     //             set_servant.servant.job = _job;
//     //         }
//     //         else
//     //         {
//     //             _job = user_servant_iter->servant.job;
//     //         }

//     //         if(level == 1)
//     //         {
//     //             set_servant.servant.status = status;
//     //         }
//     //         else
//     //         {
//     //             for(uint32_t i=2; i<=level; ++i)
//     //             {
//     //                 status = get_level_up_servant_status(_job, status);
//     //                 set_servant.servant.status = status;
//     //             }
//     //             lv_exp lv_exp_table(_self, _self.value);
//     //             auto lv_iter = lv_exp_table.find(level);
//     //             eosio_assert(lv_iter != lv_exp_table.end(),"Not Exist Level");
//     //             set_servant.servant.exp = lv_iter->exp;
//     //         }
//     //     });
//     // }
//     // else if(_kind == "monster")
//     // {
//     //     status_info status;
//     //     substr_value(_status, value_list, size_list, 5);
//     //     uint32_t index = atoi(value_list[0].c_str());
//     //     uint32_t level = atoi(value_list[1].c_str());
//     //     status.basic_str = atoll(value_list[2].c_str());
//     //     status.basic_dex = atoll(value_list[3].c_str());
//     //     status.basic_int = atoll(value_list[4].c_str());

//     //     status = get_grade_status(_grade, status);

//     //     user_monsters user_monster_table(_self, _user.value);
//     //     auto user_monster_iter = user_monster_table.find(index);
//     //     eosio_assert(user_monster_iter != user_monster_table.end(),"Not Exist Monster 4");
//     //     user_monster_table.modify(user_monster_iter, _self, [&](auto &set_monster) {
//     //         if (_id != 0)
//     //         {
//     //             monster_db monster_db_table(_self, _self.value);
//     //             auto monster_db_iter = monster_db_table.find(_id);
//     //             eosio_assert(monster_db_iter != monster_db_table.end(),"Not Exist Monster ID 5");
//     //             set_monster.monster.id = _id;
//     //         }
//     //         set_monster.monster.grade = _grade;
//     //         if(level == 1)
//     //         {
//     //             set_monster.monster.status = status;
//     //         }
//     //         else
//     //         {
//     //             for(uint32_t i=2; i<=level; ++i)
//     //             {
//     //                 status = get_level_up_monster_status(_id, _grade, status);
//     //                 set_monster.monster.status = status;
//     //             }
//     //             lv_exp lv_exp_table(_self, _self.value);
//     //             auto lv_iter = lv_exp_table.find(level);
//     //             eosio_assert(lv_iter != lv_exp_table.end(),"Not Exist Level");
//     //             set_monster.monster.exp = lv_iter->exp;
//     //         }
//     //     });
//     // }
// }
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

EOSIO_DISPATCH(battletest,
               (testcheat)(testsnap)(setdata)(resultpre)                                                                                                                      //test
               (create)(issue)(transfer)(changetoken)                                                                                                                         //token
               (towersnap)(claim)(settower)(chat)                                                                                                                                   //tower
               (deletewhite)(addwhite)(setmaster)(settokenlog)(eostransfer)(initmaster)(inittokenlog)(deleteblack)(addblack)(setpause)(dbinsert)(dberase)(dbinit)(alluserdel)(deleteuser)(dblistinsert) //db mastersystem
               (battleaction)(battlestate)(stageexit)(stagestart)(activeturn)                                                                                                 //battle
               (saveparty)(resultparty)(resultgacha)                                                                                                                          //party + gacha
               (itembuy)(monsterup)(equipmentup)(mailopen)(movedb)(equip)(unequip)(nftmail)(insertequipr)(servantburn)(monsterburn)(equipburn)(itemburn))                     //contants
