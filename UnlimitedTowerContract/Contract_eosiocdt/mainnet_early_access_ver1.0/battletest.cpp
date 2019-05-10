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
void battletest::set_eos_log(uint64_t _total_amount)
{
    user_logs user_log_table(_self, _self.value);
    auto log_iter = user_log_table.find(_self.value);
    if(log_iter == user_log_table.end())
    {
        user_log_table.emplace(_self, [&](auto &new_data)
        {
            new_data.user = _self;
            new_data.use_eos = 0 + _total_amount;
        });
    }
    else
    {
        user_log_table.modify(log_iter, _self, [&](auto &new_data)
        {
            new_data.use_eos += _total_amount;
        });
    }
}

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

// ACTION battletest::dbinsert(std::string _table, std::string _value)
// {
//     system_master system_master_table(_self, _self.value);
//     auto system_master_iter = system_master_table.begin();

//     permission_level master_auth;
//     master_auth.actor = system_master_iter->master;
//     master_auth.permission = "active"_n;
//     require_auth(master_auth);

//     eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 1");

//     std::vector<size_t> size_list;
//     std::vector<std::string> value_list;
//     uint32_t value;
//     // if (_table == "dbbody")
//     // {
//     //     value = atoll(_value.c_str());
//     //     insert_body(value);
//     // }
//     // if (_table == "dbhair")
//     // {
//     //     value = atoll(_value.c_str());
//     //     insert_hair(value);
//     // }
//     // if (_table == "dbhead")
//     // {
//     //     value = atoll(_value.c_str());
//     //     insert_head(value);
//     // }
//     // if (_table == "dbgender")
//     // {
//     //     value = atoll(_value.c_str());
//     //     insert_gender(value);
//     // }
//     // else if (_table == "dbservantjob")
//     // {
//     //     substr_value(_value, value_list, size_list, 4);
//     //     insert_job(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
//     // }
//     // else if (_table == "dbitemgrade")
//     // {
//     //     substr_value(_value, value_list, size_list, 4);
//     //     insert_item_grade(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
//     // }
//     // else if (_table == "dbmonstergd")
//     // {
//     //     substr_value(_value, value_list, size_list, 4);
//     //     insert_monster_grade(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
//     // }
//     // if (_table == "dbgraderatio")
//     // {
//     //     substr_value(_value, value_list, size_list, 2);
//     //     insert_grade_ratio(atoi(value_list[0].c_str()), atoi(value_list[1].c_str()));
//     // }

//     // if (_table == "dblevel")
//     // {
//     //     substr_value(_value, value_list, size_list, 3);
//     //     insert_level(atoll(value_list[0].c_str()),
//     //                  atoi(value_list[1].c_str()),
//     //                  atoi(value_list[2].c_str()));
//     // }
//     // else if (_table == "dbservants")
//     // {
//     //     substr_value(_value, value_list, size_list, 7);
//     //     insert_servant(atoll(value_list[0].c_str()),
//     //                    atoll(value_list[1].c_str()),
//     //                    atoi(value_list[2].c_str()),
//     //                    atoi(value_list[3].c_str()),
//     //                    atoi(value_list[4].c_str()),
//     //                    atoi(value_list[5].c_str()),
//     //                    atoi(value_list[6].c_str()));
//     // }

//     // if (_table == "dbmonsters")
//     // {
//     //     substr_value(_value, value_list, size_list, 3);
//     //     insert_monster(atoll(value_list[0].c_str()),
//     //                    atoll(value_list[1].c_str()),
//     //                    atoll(value_list[2].c_str()));
//     // }

//     // if (_table == "dballitem")
//     // {
//     //     substr_value(_value, value_list, size_list, 5);
//     //     insert_all_item_id(atoll(value_list[0].c_str()),
//     //                        atoi(value_list[1].c_str()),
//     //                        atoll(value_list[2].c_str()),
//     //                        atoll(value_list[3].c_str()),
//     //                        atoll(value_list[4].c_str()));
//     // }
//     // if (_table == "dbequipment")
//     // {
//     //     substr_value(_value, value_list, size_list, 14);
//     //     std::vector<uint64_t> grade_list;
//     //     grade_list.push_back(atoll(value_list[9].c_str()));
//     //     grade_list.push_back(atoll(value_list[10].c_str()));
//     //     grade_list.push_back(atoll(value_list[11].c_str()));
//     //     grade_list.push_back(atoll(value_list[12].c_str()));
//     //     grade_list.push_back(atoll(value_list[13].c_str()));

//     //     insert_equip_item_id(atoll(value_list[0].c_str()),
//     //                          atoll(value_list[1].c_str()),
//     //                          atoll(value_list[2].c_str()),
//     //                          atoll(value_list[3].c_str()),
//     //                          atoll(value_list[4].c_str()),
//     //                          atoll(value_list[5].c_str()),
//     //                          atoll(value_list[6].c_str()),
//     //                          atoll(value_list[7].c_str()),
//     //                          atoll(value_list[8].c_str()),grade_list);
//     // }
//     if (_table == "dbserstat")
//     {
//         substr_value(_value, value_list, size_list, 10);
//         insert_job_stat(atoll(value_list[0].c_str()),
//                         atoi(value_list[1].c_str()),
//                         atoi(value_list[2].c_str()),
//                         atoi(value_list[3].c_str()),
//                         atoi(value_list[4].c_str()),
//                         atoi(value_list[5].c_str()),
//                         atoi(value_list[6].c_str()),
//                         atoi(value_list[7].c_str()),
//                         atoi(value_list[8].c_str()),
//                         atoi(value_list[9].c_str()));
//     }
//     // if (_table == "dbgachapool")
//     // {
//     //     substr_value(_value, value_list, size_list, 2);
//     //     insert_gacha_pool(atoll(value_list[0].c_str()),
//     //                       atoll(value_list[1].c_str()));
//     // }

//     // if (_table == "dbprepool")
//     // {
//     //     substr_value(_value, value_list, size_list, 2);
//     //     insert_pre_gacha_pool(atoll(value_list[0].c_str()),
//     //                           atoll(value_list[1].c_str()));
//     // }
//     // if (_table == "dbstatusserv")
//     // {
//     //     // substr_value(_value, value_list, size_list, 2);
//     //     // insert_servant_lv_status(atoll(value_list[0].c_str()),
//     //     //                   atoll(value_list[1].c_str()));
//     //     servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
//     //     servant_lv_status_db_table.emplace(_self, [&](auto &new_data) {
//     //         new_data.grade = 5;
//     //         for (uint32_t i = 0; i < servant_status_list.size(); ++i)
//     //         {
//     //             lv_status_sub data;
//     //             data.pre_status = i;
//     //             data.update_status = servant_status_list[i];
//     //             new_data.change_status.push_back(data);
//     //         }
//     //     });
//     // }

//     // if (_table == "dbstatusmon")
//     // {
//     //     substr_value(_value, value_list, size_list, 11);
//     //     std::vector<uint32_t> grade_list;
//     //     grade_list.push_back(atoi(value_list[1].c_str()));
//     //     grade_list.push_back(atoi(value_list[2].c_str()));
//     //     grade_list.push_back(atoi(value_list[3].c_str()));
//     //     grade_list.push_back(atoi(value_list[4].c_str()));
//     //     grade_list.push_back(atoi(value_list[5].c_str()));
//     //     grade_list.push_back(atoi(value_list[6].c_str()));
//     //     grade_list.push_back(atoi(value_list[7].c_str()));
//     //     grade_list.push_back(atoi(value_list[8].c_str()));
//     //     grade_list.push_back(atoi(value_list[9].c_str()));
//     //     grade_list.push_back(atoi(value_list[10].c_str()));

//     //     insert_status_monster(atoll(value_list[0].c_str()),grade_list);
//     // }

//     // if (_table == "dbstatusequi")
//     // {
//     //     substr_value(_value, value_list, size_list, 11);
//     //     std::vector<uint32_t> grade_list;
//     //     grade_list.push_back(atoi(value_list[1].c_str()));
//     //     grade_list.push_back(atoi(value_list[2].c_str()));
//     //     grade_list.push_back(atoi(value_list[3].c_str()));
//     //     grade_list.push_back(atoi(value_list[4].c_str()));
//     //     grade_list.push_back(atoi(value_list[5].c_str()));
//     //     grade_list.push_back(atoi(value_list[6].c_str()));
//     //     grade_list.push_back(atoi(value_list[7].c_str()));
//     //     grade_list.push_back(atoi(value_list[8].c_str()));
//     //     grade_list.push_back(atoi(value_list[9].c_str()));
//     //     grade_list.push_back(atoi(value_list[10].c_str()));

//     //     insert_status_equipment(atoll(value_list[0].c_str()), grade_list);
//     // }

//     if (_table == "dbstageenemy")
//     {
//         substr_value(_value, value_list, size_list, 11);
//         insert_stage_enemy(atoll(value_list[0].c_str()),
//                            atoi(value_list[1].c_str()),
//                            atoi(value_list[2].c_str()),
//                            atoi(value_list[3].c_str()),
//                            atoi(value_list[4].c_str()),
//                            atoi(value_list[5].c_str()),
//                            atoi(value_list[6].c_str()),
//                            atoi(value_list[7].c_str()),
//                            atoi(value_list[8].c_str()),
//                            atoi(value_list[9].c_str()),
//                            atoi(value_list[10].c_str()));
//     }

//     // if (_table == "dbserstat")
//     // {
//     //     substr_value(_value, value_list, size_list, 10);
//     //     insert_job_stat(atoll(value_list[0].c_str()),
//     //                     atoi(value_list[1].c_str()),
//     //                     atoi(value_list[2].c_str()),
//     //                     atoi(value_list[3].c_str()),
//     //                     atoi(value_list[4].c_str()),
//     //                     atoi(value_list[5].c_str()),
//     //                     atoi(value_list[6].c_str()),
//     //                     atoi(value_list[7].c_str()),
//     //                     atoi(value_list[8].c_str()),
//     //                     atoi(value_list[9].c_str()));
//     // }
//     // if (_table == "dbtribe")
//     // {
//     //     substr_value(_value, value_list, size_list, 11);
//     //     insert_tribe_stat(atoll(value_list[0].c_str()),
//     //                       atoi(value_list[1].c_str()),
//     //                       atoi(value_list[2].c_str()),
//     //                       atoi(value_list[3].c_str()),
//     //                       atoi(value_list[4].c_str()),
//     //                       atoi(value_list[5].c_str()),
//     //                       atoi(value_list[6].c_str()),
//     //                       atoi(value_list[7].c_str()),
//     //                       atoi(value_list[8].c_str()),
//     //                       atoi(value_list[9].c_str()),
//     //                       atoi(value_list[10].c_str()));
//     // }
//     // if (_table == "dbtype")
//     // {
//     //     substr_value(_value, value_list, size_list, 8);
//     //     insert_type(atoll(value_list[0].c_str()),
//     //                       atoi(value_list[1].c_str()),
//     //                       atoi(value_list[2].c_str()),
//     //                       atoi(value_list[3].c_str()),
//     //                       atoi(value_list[4].c_str()),
//     //                       atoi(value_list[5].c_str()),
//     //                       atoi(value_list[6].c_str()),
//     //                       atoi(value_list[7].c_str()));
//     // }
//     // if (_table == "dbmoupstat")
//     // {
//     //     substr_value(_value, value_list, size_list, 6);
//     //     insert_monster_upgrade(atoll(value_list[0].c_str()),
//     //                            atoi(value_list[1].c_str()),
//     //                            atoi(value_list[2].c_str()),
//     //                            atoi(value_list[3].c_str()),
//     //                            atoi(value_list[4].c_str()),
//     //                            atoi(value_list[5].c_str()));
//     // }
//     // if (_table == "dbequpstat")
//     // {
//     //     substr_value(_value, value_list, size_list, 6);
//     //     insert_equipment_upgrade(atoll(value_list[0].c_str()),
//     //                              atoi(value_list[1].c_str()),
//     //                              atoi(value_list[2].c_str()),
//     //                              atoi(value_list[3].c_str()),
//     //                              atoi(value_list[4].c_str()),
//     //                              atoi(value_list[5].c_str()));
//     // }
//     // if (_table == "dbpassive")
//     // {
//     //     substr_value(_value, value_list, size_list, 9);
//     //     insert_passive(atoll(value_list[0].c_str()),
//     //                    atoi(value_list[1].c_str()),
//     //                    atoi(value_list[2].c_str()),
//     //                    atoi(value_list[3].c_str()),
//     //                    atoi(value_list[4].c_str()),
//     //                    atoi(value_list[5].c_str()),
//     //                    atoi(value_list[6].c_str()),
//     //                    atoi(value_list[7].c_str()),
//     //                    atoi(value_list[8].c_str()));
//     // }
//     // if (_table == "dbactive")
//     // {
//     //     substr_value(_value, value_list, size_list, 19);
//     //     insert_active(atoll(value_list[0].c_str()),
//     //                   atoi(value_list[1].c_str()),
//     //                   atoi(value_list[2].c_str()),
//     //                   atoi(value_list[3].c_str()),
//     //                   atoi(value_list[4].c_str()),
//     //                   atoi(value_list[5].c_str()),
//     //                   atoi(value_list[6].c_str()),
//     //                   atoi(value_list[7].c_str()),
//     //                   atoi(value_list[8].c_str()),
//     //                   atoi(value_list[9].c_str()),
//     //                   atoi(value_list[10].c_str()),
//     //                   atoi(value_list[11].c_str()),
//     //                   atoi(value_list[12].c_str()),
//     //                   atoi(value_list[13].c_str()),
//     //                   atoi(value_list[14].c_str()),
//     //                   atoi(value_list[15].c_str()),
//     //                   atoi(value_list[16].c_str()),
//     //                   atoi(value_list[17].c_str()),
//     //                   atoi(value_list[18].c_str()));
//     // }
//     // if (_table == "dbgachapool")
//     // {
//     //     substr_value(_value, value_list, size_list, 2);
//     //     insert_gacha_pool(atoll(value_list[0].c_str()),
//     //                       atoll(value_list[1].c_str()));
//     // }

//     // if (_table == "dbprepool")
//     // {
//     //     substr_value(_value, value_list, size_list, 2);
//     //     insert_pre_gacha_pool(atoll(value_list[0].c_str()),
//     //                       atoll(value_list[1].c_str()));
//     // }

//     // if (_table == "dbstage")
//     // {
//     //     substr_value(_value, value_list, size_list, 9);
//     //     insert_stage(atoll(value_list[0].c_str()),
//     //                  atoi(value_list[1].c_str()),
//     //                  atoi(value_list[2].c_str()),
//     //                  atoi(value_list[3].c_str()),
//     //                  atoi(value_list[4].c_str()),
//     //                  atoi(value_list[5].c_str()),
//     //                  atoi(value_list[6].c_str()),
//     //                  atoi(value_list[7].c_str()),
//     //                  atoi(value_list[8].c_str()));
//     // }
//     // if (_table == "dbreward")
//     // {
//     //     substr_value(_value, value_list, size_list, 9);
//     //     insert_stage_reward(atoll(value_list[0].c_str()),
//     //                   atoi(value_list[1].c_str()),
//     //                   atoi(value_list[2].c_str()),
//     //                   atoi(value_list[3].c_str()),
//     //                   atoi(value_list[4].c_str()),
//     //                   atoi(value_list[5].c_str()),
//     //                   atoi(value_list[6].c_str()),
//     //                   atoi(value_list[7].c_str()),
//     //                   atoi(value_list[8].c_str()));
//     // }
//     // else
//     // {
//     //     eosio_assert(1 == 0, "Not Exist Table");
//     // }
//     // if (_table == "dbitemshop")
//     // {
//     //     substr_value(_value, value_list, size_list, 7);
//     //     insert_itemshop(atoll(value_list[0].c_str()),
//     //                     atoi(value_list[1].c_str()),
//     //                     atoi(value_list[2].c_str()),
//     //                     atoi(value_list[3].c_str()),
//     //                     atoi(value_list[4].c_str()),
//     //                     atoi(value_list[5].c_str()),
//     //                     atoi(value_list[6].c_str()));
//     // }
//     // if (_table == "tshoplist")
//     // {
//     //     substr_value(_value, value_list, size_list, 4);
//     //     insert_shoplist(atoll(value_list[0].c_str()),
//     //                     atoi(value_list[1].c_str()),
//     //                     atoi(value_list[2].c_str()),
//     //                     atoi(value_list[3].c_str()));
//     // }
//     // if (_table == "dbmonsterup")
//     // {
//     //     substr_value(_value, value_list, size_list, 2);
//     //     insert_upgrade_monster_ratio(atoll(value_list[0].c_str()),
//     //                                  atoll(value_list[1].c_str()));
//     // }
//     // if (_table == "dbburn")
//     // {
//     //     substr_value(_value, value_list, size_list, 2);
//     //     insert_servant_burn_item(atoll(value_list[0].c_str()),
//     //                               atoll(value_list[1].c_str()));
//     // }
//     // else if (_table == "dbitemup")
//     // {
//     // }
//  }

// ACTION battletest::dblistinsert(std::string _list, std::string _primary_key, std::vector<std::string> _value_list)
// {
//     system_master system_master_table(_self, _self.value);
//     auto system_master_iter = system_master_table.begin();

//     permission_level master_auth;
//     master_auth.actor = system_master_iter->master;
//     master_auth.permission = "active"_n;
//     require_auth(master_auth);

//     eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 1");
//     if (_list == "dbstage_monsterlist")
//     {
//         uint64_t stage_id = atoll(_primary_key.c_str());
//         stage_db stage_db_table(_self, _self.value);
//         auto stage_db_iter = stage_db_table.find(stage_id);
//         eosio_assert(stage_db_iter != stage_db_table.end(), "DB List Insert : Not Exist Stage");

//         stage_db_table.modify(stage_db_iter, _self, [&](auto &new_data) {
//             for (uint32_t i = 0; i < _value_list.size();)
//             {
//                 stage_monster_info new_monster;
//                 new_monster.monster_id = atoi(_value_list[i].c_str());
//                 new_monster.position = atoi(_value_list[i+1].c_str());
//                 new_data.monster_list.push_back(new_monster);
//                 i += 2;
//             }
//         });
//     }
//     if (_list == "dbreward_rewardlist")
//     {
//         uint64_t stage_id = atoll(_primary_key.c_str());
//         reward_db reward_db_table(_self, _self.value);
//         auto stage_db_iter = reward_db_table.find(stage_id);
//         eosio_assert(stage_db_iter != reward_db_table.end(), "DB List Insert : Not Exist Reward");

//         reward_db_table.modify(stage_db_iter, _self, [&](auto &new_data) {
//             for (uint32_t i = 0; i < _value_list.size();)
//             {
//                 reward_info new_reward;
//                 new_reward.type = atoi(_value_list[i].c_str());
//                 new_reward.id = atoi(_value_list[i + 1].c_str());
//                 new_reward.grade = atoi(_value_list[i + 2].c_str());
//                 new_reward.per = atoi(_value_list[i + 3].c_str());
//                 new_reward.count = atoi(_value_list[i + 4].c_str());
//                 new_data.reward_list.push_back(new_reward);
//                 i += 5;
//             }
//         });
//     }
// }

void battletest::insert_status_monster(uint64_t _grade, std::vector<uint32_t> _status_list)
{
    monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
    auto iter = monster_lv_status_db_table.find(_grade);
    if(iter == monster_lv_status_db_table.end())
    {
        monster_lv_status_db_table.emplace(_self, [&](auto &new_data)
        {
            new_data.grade = _grade;
            for(uint32_t i = 0; i < _status_list.size(); ++i)
            {
                lv_status_sub sub;
                sub.pre_status = i;
                sub.update_status = _status_list[i];
                new_data.change_status.push_back(sub);
            }
        });
    }
    else
    {
        monster_lv_status_db_table.modify(iter, _self, [&](auto &new_data) {
            for (uint32_t i = 0; i < _status_list.size(); ++i)
            {
                new_data.change_status[i].update_status = _status_list[i];
            }
        });
    }
}

void battletest::insert_status_equipment(uint64_t _grade, std::vector<uint32_t> _status_list)
{
    equipment_lv_status_db monster_lv_status_db_table(_self, _self.value);
    auto iter = monster_lv_status_db_table.find(_grade);
    if(iter == monster_lv_status_db_table.end())
    {
        monster_lv_status_db_table.emplace(_self, [&](auto &new_data)
        {
            new_data.type_grade = _grade;
            for(uint32_t i = 0; i < _status_list.size(); ++i)
            {
                lv_status_sub sub;
                sub.pre_status = i;
                sub.update_status = _status_list[i];
                new_data.change_status.push_back(sub);
            }
        });
    }
    else
    {
        monster_lv_status_db_table.modify(iter, _self, [&](auto &new_data) {
            for (uint32_t i = 0; i < _status_list.size(); ++i)
            {
                new_data.change_status[i].update_status = _status_list[i];
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

void battletest::insert_monster(uint64_t _monster_id, uint64_t _tribe, uint64_t _type)
{
    monster_db monster_id_db_table(_self, _self.value);
    auto monster_id_iter = monster_id_db_table.find(_monster_id);
    if (monster_id_iter == monster_id_db_table.end())
    {
        monster_id_db_table.emplace(_self, [&](auto &new_monster_id) {
            new_monster_id.id = _monster_id;
            new_monster_id.tribe = _tribe;
            new_monster_id.type = _type;
        });
    }
    else
    {
        monster_id_db_table.modify(monster_id_iter, _self, [&](auto &new_monster_id) {
            new_monster_id.tribe = _tribe;
            new_monster_id.type = _type;
        });
    }
}

void battletest::insert_equip_item_id(uint64_t _item_id,
                                      uint64_t _item_set_id,
                                      uint64_t _type,
                                      uint64_t _tier,
                                      uint64_t _job,
                                      uint64_t _option,
                                      uint64_t _option_value_min,
                                      uint64_t _option_value_max,
                                      uint64_t _upgrade_option_value,
                                      std::vector<uint64_t> _grade_multi)
{
    equipment_db my_table(_self, _self.value);
    auto iter = my_table.find(_item_id);
    if(iter == my_table.end())
    {
        my_table.emplace(_self, [&](auto &new_data)
        {
            new_data.item_id =_item_id;
            new_data.set_id = _item_set_id;
            new_data.type = _type;
            new_data.tier = _tier;
            new_data.job = _job;
            new_data.option_list.push_back(_option);
            new_data.option_value_min_list.push_back(_option_value_min);
            new_data.option_value_max_list.push_back(_option_value_max);
            new_data.upgrade_option_value_list.push_back(_upgrade_option_value);
            for (uint32_t i = 0; i < _grade_multi.size(); ++i)
            {
                new_data.grade_multi_list.push_back(_grade_multi[i]);
            }
        });
    }
    else
    {
        my_table.modify(iter, _self, [&](auto &new_data)
        {
            new_data.set_id = _item_set_id;
            new_data.type = _type;
            new_data.tier = _tier;
            new_data.job = _job;
            new_data.option_list[0] = _option;
            new_data.option_value_min_list[0] = (_option_value_min);
            new_data.option_value_max_list[0] = (_option_value_max);
            new_data.upgrade_option_value_list[0] = (_upgrade_option_value);
            new_data.grade_multi_list.clear();
            for (uint32_t i = 0; i < _grade_multi.size(); ++i)
            {
                new_data.grade_multi_list.push_back(_grade_multi[i]);
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

void battletest::insert_upgrade_monster_ratio(uint32_t _main, uint64_t _upgrade_price_count)
{
    upgrade_monster_ratio_db upgrade_monster_ratio_db_table(_self, _self.value);
    upgrade_monster_ratio_db_table.emplace(_self, [&](auto &new_upgrade__monster_ratio) {
        new_upgrade__monster_ratio.main_monster_grade_upgrade = _main;
        std::vector<upgrade_monster_sub> upgrade_sub_list;
        std::vector<uint32_t> upgrade_temp_ratio_list;
        std::vector<uint32_t> upgrade_ratio_list;
        uint64_t max_ratio = 10000;
        uint64_t main_grade = _main / 10;
        uint64_t main_upgrade = _main % 10;

        for (uint32_t i = 0; i <= main_upgrade; i++)
        {
            upgrade_temp_ratio_list.push_back(max_ratio);
            max_ratio = max_ratio / 2;
        }

        uint32_t temp = main_upgrade + 1;
        upgrade_ratio_list.resize(temp);
        for (uint32_t i = 0; i < upgrade_temp_ratio_list.size(); i++)
        {
            upgrade_ratio_list[temp - 1] = upgrade_temp_ratio_list[i];
            temp--;
        }
        for (uint32_t i = 0; i <= main_upgrade; i++)
        {
            upgrade_monster_sub new_sub;
            new_sub.sub_monster_upgrade = i;
            new_sub.ratio = upgrade_ratio_list[i];
            new_upgrade__monster_ratio.sub.push_back(new_sub);
        }                   
        new_upgrade__monster_ratio.use_UTG =_upgrade_price_count; 
    });

    
}

// ACTION battletest::insertequipr(uint64_t _main, std::vector<uint64_t> &_upgrade_ratio, uint64_t _material_id, std::vector<uint64_t> &_material_count, std::vector<uint64_t> &_use_UTG)
// {
//     system_master system_master_table(_self, _self.value);
//     auto system_master_iter = system_master_table.begin();

//     permission_level master_auth;
//     master_auth.actor = system_master_iter->master;
//     master_auth.permission = "active"_n;
//     require_auth(master_auth);

//     upgrade_equipment_ratio_dbs upgrade_equipment_ratio_db_table(_self, _self.value);
//     auto upgrade_equipment_ratio_db_iter = upgrade_equipment_ratio_db_table.find(_main);
//     if (upgrade_equipment_ratio_db_iter == upgrade_equipment_ratio_db_table.end())
//     {
//         upgrade_equipment_ratio_db_table.emplace(_self, [&](auto &new_upgrade_equipment_iter) {
//             new_upgrade_equipment_iter.equipment_type_grade = _main;
//             new_upgrade_equipment_iter.upgrade_ratio = _upgrade_ratio;
//             new_upgrade_equipment_iter.material_id = _material_id;
//             new_upgrade_equipment_iter.material_count = _material_count;
//             new_upgrade_equipment_iter.use_UTG = _use_UTG;
//         });
//     }
//     else
//     {
//         upgrade_equipment_ratio_db_table.modify(upgrade_equipment_ratio_db_iter, _self, [&](auto &new_upgrade_equipment_iter) {
//             new_upgrade_equipment_iter.equipment_type_grade = _main;
//             new_upgrade_equipment_iter.upgrade_ratio = _upgrade_ratio;
//             new_upgrade_equipment_iter.material_id = _material_id;
//             new_upgrade_equipment_iter.material_count = _material_count;
//             new_upgrade_equipment_iter.use_UTG = _use_UTG;
//         });
//     }
// }

void battletest::insert_level(uint32_t _level, uint32_t _rank_exp, uint32_t _char_exp)
{
    lv_exp lv_exp_table(_self, _self.value);
    auto iter = lv_exp_table.find(_level);
    if(iter ==  lv_exp_table.end())
    {
        lv_exp_table.emplace(_self, [&](auto &new_lv_exp) {
            new_lv_exp.lv = _level;
            new_lv_exp.rank_exp = _rank_exp;
            new_lv_exp.char_exp = _char_exp;
        });
    }
    else
    {
        lv_exp_table.modify(iter, _self, [&](auto &new_lv_exp) {
            new_lv_exp.lv = _level;
            new_lv_exp.rank_exp = _rank_exp;
            new_lv_exp.char_exp = _char_exp;
        });
    }
}




void battletest::insert_passive(uint64_t _passive_id, uint32_t _passive_type, uint32_t _job_class, uint32_t _enable_stack_max, uint32_t _effect_id,
                                uint32_t _effect_type_id, uint32_t _effect_value_a, uint32_t _effect_value_add_b, uint32_t _target_id)
{
    passive_db passive_db_table(_self, _self.value);
    auto passive_db_iter = passive_db_table.find(_passive_id);
    if (passive_db_iter == passive_db_table.end())
    {
        passive_db_table.emplace(_self, [&](auto &new_passive) {
            new_passive.passive_id = _passive_id;
            new_passive.passive_type = _passive_type;
            new_passive.job_class = _job_class;
            new_passive.enable_stack_max = _enable_stack_max;
            new_passive.effect_id = _effect_id;
            new_passive.effect_type_id = _effect_type_id;
            new_passive.effect_value_a = _effect_value_a;
            new_passive.effect_value_add_b = _effect_value_add_b;
            new_passive.target_id = _target_id;
        });
    }
    else
    {
        passive_db_table.modify(passive_db_iter, _self, [&](auto &new_passive) {
            new_passive.passive_type = _passive_type;
            new_passive.job_class = _job_class;
            new_passive.enable_stack_max = _enable_stack_max;
            new_passive.effect_id = _effect_id;
            new_passive.effect_type_id = _effect_type_id;
            new_passive.effect_value_a = _effect_value_a;
            new_passive.effect_value_add_b = _effect_value_add_b;
            new_passive.target_id = _target_id;
        });
    }
}

void battletest::insert_active(uint64_t _active_id,  uint32_t _job, uint32_t _tribe, uint32_t _active_per,
                               uint32_t _skill_type, uint32_t _active_turn, uint32_t _attack_type, uint32_t _dmg_type, uint32_t _elemental_type, uint32_t _target, uint32_t _target_count, 
                               uint32_t _hit_count, uint32_t _atk_per, uint32_t _atk_per_add, uint32_t _atk_per_2, uint32_t _atk_per_add_2, uint32_t _heal_per, uint32_t _heal_per_add,uint32_t _option_id)
{
    active_db active_db_table(_self, _self.value);
    auto active_db_iter = active_db_table.find(_active_id);
    if (active_db_iter == active_db_table.end())
    {
        active_db_table.emplace(_self, [&](auto &new_active) {
            new_active.active_id = _active_id;
            new_active.job = _job;
            new_active.tribe = _tribe;
            new_active.active_per = _active_per;
            new_active.skill_type = _skill_type;
            new_active.attack_type = _attack_type;
            new_active.dmg_type = _dmg_type;
            new_active.elemental_type = _elemental_type;
            new_active.target_type = _target;
            new_active.target_count = _target_count;
            new_active.active_turn = _active_turn;
            new_active.hit_count = _hit_count;
            new_active.atk_per_1 = _atk_per;
            new_active.atk_per_add_1 = _atk_per_add;
            new_active.atk_per_2 = _atk_per_2;
            new_active.atk_per_add_2= _atk_per_add_2;
            new_active.heal_per = _heal_per;
            new_active.heal_per_add = _heal_per_add;
            new_active.option_id = _option_id;
        });
    }
    else
    {
        active_db_table.modify(active_db_iter, _self, [&](auto &new_active) {
            new_active.job = _job;
            new_active.tribe = _tribe;
            new_active.active_per = _active_per;
            new_active.skill_type = _skill_type;
            new_active.attack_type = _attack_type;
            new_active.dmg_type = _dmg_type;
            new_active.elemental_type = _elemental_type;
            new_active.target_type = _target;
            new_active.target_count = _target_count;
            new_active.active_turn = _active_turn;
            new_active.hit_count = _hit_count;
            new_active.atk_per_1 = _atk_per;
            new_active.atk_per_add_1 = _atk_per_add;
            new_active.atk_per_2 = _atk_per_2;
            new_active.atk_per_add_2= _atk_per_add_2;
            new_active.heal_per = _heal_per;
            new_active.heal_per_add = _heal_per_add;
            new_active.option_id = _option_id;
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

void battletest::insert_pre_gacha_pool(uint64_t _gacha_id, uint64_t _db_index)
{
    pre_gacha_db pre_gacha_db_table(_self, _self.value);
    auto pre_gacha_db_iter = pre_gacha_db_table.find(_gacha_id);
    if (pre_gacha_db_iter == pre_gacha_db_table.end())
    {
        pre_gacha_db_table.emplace(_self, [&](auto &new_gacha) {
            new_gacha.gacha_id = _gacha_id;
            new_gacha.db_index = _db_index;
        });
    }
    else
    {
        pre_gacha_db_table.modify(pre_gacha_db_iter, _self, [&](auto &new_gacha) {
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
                                    uint32_t _cri_per,
                                    uint32_t _cri_dmg,
                                    uint32_t _speed,
                                    uint32_t _avoid)
{
    stage_enemy_db stage_enemy_db_table(_self, _self.value);
    auto enemy_iter = stage_enemy_db_table.find(_id);
    if (enemy_iter == stage_enemy_db_table.end())
    {
        stage_enemy_db_table.emplace(_self, [&](auto &new_enemy) {
            new_enemy.id = _id;
            new_enemy.grade = _grade;
            new_enemy.enemy_tribe = _enemy_class;
            new_enemy.type = _type;
            new_enemy.enemy_str = _enemy_str;
            new_enemy.enemy_dex = _enemy_dex;
            new_enemy.enemy_int = _enemy_int;
            new_enemy.cri_per = _cri_per;
            new_enemy.cri_dmg = _cri_dmg;
            new_enemy.speed = _speed;
            new_enemy.avoid = _avoid;
        });
    }
    else
    {
        stage_enemy_db_table.modify(enemy_iter, _self, [&](auto &new_enemy) {
            new_enemy.grade = _grade;
            new_enemy.enemy_tribe = _enemy_class;
            new_enemy.type = _type;
            new_enemy.enemy_str = _enemy_str;
            new_enemy.enemy_dex = _enemy_dex;
            new_enemy.enemy_int = _enemy_int;
            new_enemy.cri_per = _cri_per;
            new_enemy.cri_dmg = _cri_dmg;
            new_enemy.speed = _speed;
            new_enemy.avoid = _avoid;
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

void battletest::insert_tribe_stat(uint64_t _id,
                                    uint32_t _base_str,
                                    uint32_t _base_dex,
                                    uint32_t _base_int,
                                   uint32_t _speed,
                                   uint32_t _avoid,
                                   uint32_t _cri_per,
                                   uint32_t _cri_dmg,
                                   uint32_t _pair,
                                   uint32_t _triple,
                                   uint32_t _penta)
{
    tribe_db class_stat_db_table(_self, _self.value);
    auto class_iter = class_stat_db_table.find(_id);
    if (class_iter == class_stat_db_table.end())
    {
        class_stat_db_table.emplace(_self, [&](auto &new_data) {
            new_data.id = _id;
            new_data.base_str = _base_str;
            new_data.base_dex = _base_dex;
            new_data.base_int = _base_int;
            new_data.speed = _speed;
            new_data.avoid = _avoid;
            new_data.cri_per = _cri_per;
            new_data.cri_dmg = _cri_dmg;
            new_data.pair = _pair;
            new_data.triple = _triple;
            new_data.penta = _penta;
        });
    }
    else
    {
        class_stat_db_table.modify(class_iter, _self, [&](auto &new_data) {
            new_data.base_str = _base_str;
            new_data.base_dex = _base_dex;
            new_data.base_int = _base_int;
            new_data.speed = _speed;
            new_data.avoid = _avoid;
            new_data.cri_per = _cri_per;
            new_data.cri_dmg = _cri_dmg;
            new_data.pair = _pair;
            new_data.triple = _triple;
            new_data.penta = _penta;
        });
    }
}

void battletest::insert_job_stat(uint64_t _id,
                                uint32_t _job,
                                uint32_t _grade,
                                 uint32_t _base_str,
                                 uint32_t _base_dex,
                                 uint32_t _base_int,
                                 uint32_t _speed,
                                 uint32_t _avoid,
                                 uint32_t _cri_per,
                                 uint32_t _cri_dmg)
{
    serstat_db class_stat_db_table(_self, _self.value);
    auto class_iter = class_stat_db_table.find(_id);
    if (class_iter == class_stat_db_table.end())
    {
        class_stat_db_table.emplace(_self, [&](auto &new_data) {
            new_data.id = _id;
            new_data.job = _job;
            new_data.grade = _grade;
            new_data.base_str = _base_str;
            new_data.base_dex = _base_dex;
            new_data.base_int = _base_int;
            new_data.speed = _speed;
            new_data.avoid = _avoid;
            new_data.cri_per = _cri_per;
            new_data.cri_dmg = _cri_dmg;
        });
    }
    else
    {
        class_stat_db_table.modify(class_iter, _self, [&](auto &new_data) {
            new_data.job = _job;
            new_data.grade = _grade;
            new_data.base_str = _base_str;
            new_data.base_dex = _base_dex;
            new_data.base_int = _base_int;
            new_data.speed = _speed;
            new_data.avoid = _avoid;
            new_data.cri_per = _cri_per;
            new_data.cri_dmg = _cri_dmg;
        });
    }
}

void battletest::insert_type(uint64_t _id, uint32_t _strong, uint32_t _weak, uint32_t _strong_per, uint32_t _weak_per, uint32_t _pair, uint32_t _triple, uint32_t _penta)
{
    type_db my_table(_self, _self.value);
    auto class_iter = my_table.find(_id);
    if (class_iter == my_table.end())
    {
        my_table.emplace(_self, [&](auto &new_data) {
            new_data.id = _id;
            new_data.strong = _strong;
            new_data.weak = _weak;
            new_data.strong_per = _strong_per;
            new_data.weak_per = _weak_per;
            new_data.pair = _pair;
            new_data.triple = _triple;
            new_data.penta = _penta;
        });
    }
    else
    {
        my_table.modify(class_iter, _self, [&](auto &new_data) {
            new_data.strong = _strong;
            new_data.weak = _weak;
            new_data.strong_per = _strong_per;
            new_data.weak_per = _weak_per;
            new_data.pair = _pair;
            new_data.triple = _triple;
            new_data.penta = _penta;
        });
    }
}
void battletest::insert_monster_upgrade(uint64_t _upgrade, uint32_t _common, uint32_t _uncommon, uint32_t _rare, uint32_t _unique, uint32_t _legendary)
{
    upgrade_monster_db upgrade_db_table(_self, _self.value);
    auto upgrade_iter = upgrade_db_table.find(_upgrade);
    if(upgrade_iter == upgrade_db_table.end())
    {
        upgrade_db_table.emplace(_self, [&](auto &new_data)
        {
            new_data.upgrade = _upgrade;
            new_data.grade_list.resize(5);
            new_data.grade_list[0] = _common;
            new_data.grade_list[1] = _uncommon;
            new_data.grade_list[2] = _rare;
            new_data.grade_list[3] = _unique;
            new_data.grade_list[4] = _legendary;
        });
    }
    else
    {
        upgrade_db_table.modify(upgrade_iter ,_self, [&](auto &new_data)
        {
            new_data.grade_list[0] = _common;
            new_data.grade_list[1] = _uncommon;
            new_data.grade_list[2] = _rare;
            new_data.grade_list[3] = _unique;
            new_data.grade_list[4] = _legendary;
        });
    }
}

void battletest::insert_equipment_upgrade(uint64_t _upgrade, uint32_t _common, uint32_t _uncommon, uint32_t _rare, uint32_t _unique, uint32_t _legendary)
{
    upgrade_equipment_db upgrade_db_table(_self, _self.value);
    auto upgrade_iter = upgrade_db_table.find(_upgrade);
    if(upgrade_iter == upgrade_db_table.end())
    {
        upgrade_db_table.emplace(_self, [&](auto &new_data)
        {
            new_data.upgrade = _upgrade;
            new_data.grade_list.resize(5);
            new_data.grade_list[0] = _common;
            new_data.grade_list[1] = _uncommon;
            new_data.grade_list[2] = _rare;
            new_data.grade_list[3] = _unique;
            new_data.grade_list[4] = _legendary;
        });
    }
    else
    {
        upgrade_db_table.modify(upgrade_iter ,_self, [&](auto &new_data)
        {
            new_data.grade_list[0] = _common;
            new_data.grade_list[1] = _uncommon;
            new_data.grade_list[2] = _rare;
            new_data.grade_list[3] = _unique;
            new_data.grade_list[4] = _legendary;
        });
    }
}


// void battletest::insert_itemshop(uint64_t _id, uint64_t _goods_type,      
//                                         uint64_t _goods_id, uint64_t _goods_count, 
//                                         uint64_t _goods_limited, uint64_t _price_type, 
//                                         uint64_t _etc_type, uint64_t _price_count)
// {
//     item_shop item_table_shop_table(_self, _self.value);
//     auto item_table_shop_iter = item_table_shop_table.find(_id);

//     if (item_table_shop_iter == item_table_shop_table.end())
//     {
//         item_table_shop_table.emplace(_self, [&](auto &new_data) {
//             new_data.id = _id;
//             new_data.goods_type = _goods_type;
//             new_data.goods_id = _goods_id;
//             new_data.goods_count = _goods_count;
//             new_data.goods_limited = _goods_limited;
//             new_data.price_type = _price_type;
//             new_data.etc_type = _etc_type;
//             new_data.price_count = _price_count;
//         });
//     }
//     else
//     {
//         item_table_shop_table.modify(item_table_shop_iter, _self, [&](auto &new_data) {
//             new_data.goods_type = _goods_type;
//             new_data.goods_id = _goods_id;
//             new_data.goods_count = _goods_count;
//             new_data.goods_limited = _goods_limited;
//             new_data.price_type = _price_type;
//             new_data.etc_type = _etc_type;
//             new_data.price_count = _price_count;
//         });
//     }    
// }

// void battletest::insert_itemshop(uint64_t _id, uint64_t _type, uint64_t _product_id, uint64_t _product_count, uint64_t _limit_count,
//                             uint64_t _limit_max, uint64_t _price_id, uint64_t _price_count)
// {
//     shop_list shop_list_table(_self, _self.value);
//     auto shop_list_iter = shop_list_table.find(_id);
//     if(shop_list_iter == shop_list_table.end())
//     {
//         shop_list_table.emplace(_self, [&](auto &new_data){
//             new_data.id = _id;
//             new_data.type = _type;
//             new_data.product_id = _product_id;
//             new_data.product_count = _product_count;
//             new_data.limit_count = _limit_count;
//             new_data.limit_max = _limit_max;
//             new_data.price_id = _price_id;
//             new_data.price_count = _price_count;

//         });
//     }
//     else
//     {
//         shop_list_table.modify(shop_list_iter, _self, [&](auto &new_data){
//             new_data.type = _type;
//             new_data.product_id = _product_id;
//             new_data.product_count = _product_count;
//             new_data.limit_count = _limit_count;
//             new_data.limit_max = _limit_max;
//             new_data.price_id = _price_id;
//             new_data.price_count = _price_count;
//         });
//     }
    
// }

void battletest::insert_servant_burn_item(uint64_t _servant_job, uint64_t _result_item_id)
{
    burnitem_db burnitem_db_table(_self, _self.value);
    auto iter = burnitem_db_table.find(_servant_job);
    if(iter == burnitem_db_table.end())
    {
        burnitem_db_table.emplace(_self, [&](auto &new_data)
        {
            new_data.servant_job = _servant_job;
            new_data.result_item_id = _result_item_id;
        });
    }
    else
    {
        burnitem_db_table.modify(iter, _self, [&](auto &new_data) {
            new_data.result_item_id = _result_item_id;
        });
    }
}


// ACTION battletest::dberase(std::string _table, std::string _value)
// {
//     system_master system_master_table(_self, _self.value);
//     auto system_master_iter = system_master_table.begin();

//     permission_level master_auth;
//     master_auth.actor = system_master_iter->master;
//     master_auth.permission = "active"_n;
//     require_auth(master_auth);

//     eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 3");

//     std::vector<size_t> size_list;
//     std::vector<std::string> value_list;
//     uint64_t value;
//    // if (_table == "dbbody")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_body(value);
//     // }
//     // else if (_table == "dbhair")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_hair(value);
//     // }
//     // else if (_table == "dbhead")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_head(value);
//     // }
//     // else if (_table == "dbgender")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_gender(value);
//     // }
//     // if (_table == "dbservantjob")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_job(value);
//     // }
//     // else if (_table == "dbitemgrade")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_item_grade(value);
//     // }
//     //     else if (_table == "dbmonstergd")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_monster_grade(value);
//     // }
//     //     else if (_table == "dbgraderatio")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_grade_ratio(value);
//     // }
//     // if (_table == "dbmonsterup")
//     // {
//     //     value = atoi(_value.c_str());
//     //     erase_upgrade_monster_ratio(value);
//     // }
//     // if (_table == "dblevel")
//     // {
//     //     value = atoi(_value.c_str());
//     //     erase_level(value);
//     // }
//     // if (_table == "dbburn")
//     // {
//     //     value = atoi(_value.c_str());
//     // }
//     // if (_table == "dbservants")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_servant_id(value);
//     // }
//     // if (_table == "dbmonsters")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_monster_id(value);
//     // }
//     // if (_table == "dballitem")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_all_item_id(value);
//     // }
//     // if (_table == "dbequipment")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_equip_item_id(value);
//     // }
//     // else if (_table =="dbservnatlv")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_servant_lv(value);
//     // }
//     // if (_table =="dbmonsterlv")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_monster_lv(value);
//     // }
//     // if (_table =="dbstatusserv")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_servant_lv_status(value);
//     // }
//     // if (_table =="dbstatusmon")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_monster_lv_status(value);
//     // }
//     // if (_table == "dbstatusmon_change_status")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_monster_lv_status_list(value);
//     // }
//     // if (_table == "dbpassive")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_passive(value);
//     // }
//     // if (_table == "dbactive")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_active(value);
//     // }
//     // if (_table == "dbgachapool")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_gacha_pool(value);
//     // }
//     // if (_table == "dbprepool")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_pre_gacha_pool(value);
//     // }
//     // if (_table == "dbstageenemy")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_stage_enemy(value);
//     // }
//     // if (_table == "dbstatusequi")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_equipment_lv_status(value);
//     // }
//     // if( _table == "dbstatusequi_change_status")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_equipment_lv_status_list(value);
//     // }
//     // if (_table == "dbstage") 
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_stage(value);
//     // }
//     if (_table == "dbstage_monsterlist") 
//     {
//         value = atoll(_value.c_str());
//         erase_stage_monster_list(value);
//     }
//     // if (_table == "dbreward") 
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_stage_reward(value);
//     // }
//     // if (_table == "dbreward_rewardlist") 
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_stage_reward_list(value);
//     // }
//     // if (_table == "dbclassstat")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_class_state(value);
//     // }
//     // if (_table == "dbjobstat")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_job_state(value);
//     // }
//     // else
//     // {
//     //     eosio_assert(1 == 0, "Not Exist Table");
//     // }
//     // if (_table == "dbitemshop")
//     // {
//     //     value = atoll(_value.c_str());
//     //     erase_itemshop(value);
//     // }

// }

void battletest::erase_job(uint64_t _job)
{
    servant_job_db servant_job_db_table(_self, _self.value);
    auto job_iter = servant_job_db_table.find(_job);
    eosio_assert(job_iter != servant_job_db_table.end(), "Not Exist Job Data");
    servant_job_db_table.erase(job_iter);
}

// void battletest::erase_body(uint64_t _appear)
// {
//     body_db body_db_table(_self, _self.value);
//     auto body_iter = body_db_table.find(_appear);
//     eosio_assert(body_iter != body_db_table.end(), "Not Find Body Data");
//     body_db_table.erase(body_iter);
// }

// void battletest::erase_hair(uint64_t _appear)
// {
//     hair_db hair_db_table(_self, _self.value);
//     auto hair_iter = hair_db_table.find(_appear);
//     eosio_assert(hair_iter != hair_db_table.end(), "Not Find Hair Data");
//     hair_db_table.erase(hair_iter);
// }

// void battletest::erase_head(uint64_t _appear)
// {
//     head_db head_db_table(_self, _self.value);
//     auto head_iter = head_db_table.find(_appear);
//     eosio_assert(head_iter != head_db_table.end(), "Not Find Head Data");
//     head_db_table.erase(head_iter);
// }

// void battletest::erase_gender(uint64_t _appear)
// {
//     gender_db gender_db_table(_self, _self.value);
//     auto gender_iter = gender_db_table.find(_appear);
//     eosio_assert(gender_iter != gender_db_table.end(), "Not Find Gender Data");
//     gender_db_table.erase(gender_iter);
// }

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

// void battletest::erase_monster_grade(uint64_t _grade)
// {
//     monster_grade_db monster_grade_db_table(_self, _self.value);
//     auto monster_grade_iter = monster_grade_db_table.find(_grade);
//     eosio_assert(monster_grade_iter != monster_grade_db_table.end(), "Not Exist Monster Grade");
//     monster_grade_db_table.erase(monster_grade_iter);
// }

// void battletest::erase_item_grade(uint64_t _grade)
// {
//     item_grade_db item_grade_db_table(_self, _self.value);
//     auto item_grade_iter = item_grade_db_table.find(_grade);
//     eosio_assert(item_grade_iter != item_grade_db_table.end(), "Not Exist Item Grade Data");
//     item_grade_db_table.erase(item_grade_iter);
// }

// void battletest::erase_equip_item_id(uint64_t _id)
// {
//     equipment_db item_id_db_table(_self, _self.value);
//     auto item_id_iter = item_id_db_table.find(_id);
//     eosio_assert(item_id_iter != item_id_db_table.end(), "Not Exist Item ID 2");
//     item_id_db_table.erase(item_id_iter);
// }

// void battletest::erase_all_item_id(uint64_t _id)
// {
//     allitem_db item_id_db_table(_self, _self.value);
//     auto item_id_iter = item_id_db_table.find(_id);
//     eosio_assert(item_id_iter != item_id_db_table.end(), "Not Exist Item ID 2");
//     item_id_db_table.erase(item_id_iter);
// }

// void battletest::erase_grade_ratio(uint64_t _grade)
// {
//     grade_ratio_db grade_ratio_db_table(_self, _self.value);
//     auto grade_ratio_db_iter = grade_ratio_db_table.find(_grade);
//     eosio_assert(grade_ratio_db_iter != grade_ratio_db_table.end(), "Not Exist Grade Ratio Data");
//     grade_ratio_db_table.erase(grade_ratio_db_iter);
// }

// void battletest::erase_upgrade_monster_ratio(uint32_t _main)
// {
//     upgrade_monster_ratio_db upgrade_monster_ratio_db_table(_self, _self.value);
//     auto upgrade_monster_ratio_db_iter = upgrade_monster_ratio_db_table.find(_main);
//     eosio_assert(upgrade_monster_ratio_db_iter != upgrade_monster_ratio_db_table.end(), "Not Exist upgrade Main Monster Data");
//     upgrade_monster_ratio_db_table.erase(upgrade_monster_ratio_db_iter);
// }

// void battletest::erase_level(uint32_t _id)
// {
//     lv_exp lv_exp_table(_self, _self.value);
//     auto lv_exp_iter = lv_exp_table.find(_id);
//     eosio_assert(lv_exp_iter != lv_exp_table.end(), "Not exist Level to Exp Data");
//     lv_exp_table.erase(lv_exp_iter);
// }

// void battletest::erase_servant_lv_status(uint64_t _type)
// {
//     servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
//     auto servant_lv_status_db_iter = servant_lv_status_db_table.find(_type);
//     eosio_assert(servant_lv_status_db_iter != servant_lv_status_db_table.end(), "Not exist servant_lv_status Data");
//     servant_lv_status_db_table.erase(servant_lv_status_db_iter);
// }

// void battletest::erase_servant_lv_status_list(uint64_t _type)
// {
//     servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
//     auto servant_lv_status_db_iter = servant_lv_status_db_table.find(_type);
//     eosio_assert(servant_lv_status_db_iter != servant_lv_status_db_table.end(), "Not exist servant_lv_status Data");
//     servant_lv_status_db_table.modify(servant_lv_status_db_iter, _self, [&](auto &new_data) {
//         new_data.change_status.clear();
//     });
// }

// void battletest::erase_monster_lv_status(uint64_t _type)
// {
//     monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
//     auto monster_lv_status_db_iter = monster_lv_status_db_table.find(_type);
//     eosio_assert(monster_lv_status_db_iter != monster_lv_status_db_table.end(), "Not exist monster_lv_status Data");
//     monster_lv_status_db_table.erase(monster_lv_status_db_iter);
// }

// void battletest::erase_monster_lv_status_list(uint64_t _type)
// {
//     monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
//     auto monster_lv_status_db_iter = monster_lv_status_db_table.find(_type);
//     eosio_assert(monster_lv_status_db_iter != monster_lv_status_db_table.end(), "Not exist monster_lv_status Data");
//     monster_lv_status_db_table.modify(monster_lv_status_db_iter, _self, [&](auto &new_data) {
//         new_data.change_status.clear();
//     });
// }

// void battletest::erase_equipment_lv_status(uint64_t _type)
// {
//     equipment_lv_status_db equipment_lv_status_db_table(_self, _self.value);
//     auto equipment_lv_status_db_iter = equipment_lv_status_db_table.find(_type);
//     eosio_assert(equipment_lv_status_db_iter != equipment_lv_status_db_table.end(), "Not exist equipment_lv_status Data");
//     equipment_lv_status_db_table.erase(equipment_lv_status_db_iter);
// }

// void battletest::erase_equipment_lv_status_list(uint64_t _type)
// {
//     equipment_lv_status_db equipment_lv_status_db_table(_self, _self.value);
//     auto equipment_lv_status_db_iter = equipment_lv_status_db_table.find(_type);
//     eosio_assert(equipment_lv_status_db_iter != equipment_lv_status_db_table.end(), "Not exist equipment_lv_status Data");
//     equipment_lv_status_db_table.modify(equipment_lv_status_db_iter, _self, [&](auto &new_data)
//     {
//         new_data.change_status.clear();
//     });
// }

// void battletest::erase_passive(uint64_t _id)
// {
//     passive_db passive_db_table(_self, _self.value);
//     auto passive_db_iter = passive_db_table.find(_id);
//     eosio_assert(passive_db_iter != passive_db_table.end(), "Not Exist Passive 1");
//     passive_db_table.erase(passive_db_iter);
// }

// void battletest::erase_active(uint64_t _id)
// {
//     active_db active_db_table(_self, _self.value);
//     auto active_db_iter = active_db_table.find(_id);
//     eosio_assert(active_db_iter != active_db_table.end(), "Not Exist Active 1");
//     active_db_table.erase(active_db_iter);
// }

void battletest::erase_gacha_pool(uint64_t _id)
{
    main_gacha_db main_gacha_db_table(_self, _self.value);
    auto main_gacha_db_iter = main_gacha_db_table.find(_id);
    eosio_assert(main_gacha_db_iter != main_gacha_db_table.end(), "Not Exist Gacha 1");
    main_gacha_db_table.erase(main_gacha_db_iter);
}

// void battletest::erase_pre_gacha_pool(uint64_t _id)
// {
//     pre_gacha_db pre_gacha_db_table(_self, _self.value);
//     auto pre_gacha_db_iter = pre_gacha_db_table.find(_id);
//     eosio_assert(pre_gacha_db_iter != pre_gacha_db_table.end(), "Not Exist Pre Gacha 1");
//     pre_gacha_db_table.erase(pre_gacha_db_iter);
// }

// void battletest::erase_stage_enemy(uint64_t _id)
// {
//     stage_enemy_db stage_enemy_db_table(_self, _self.value);
//     auto enemy_iter = stage_enemy_db_table.find(_id);
//     eosio_assert(enemy_iter != stage_enemy_db_table.end(), "Not Exist Enemy");
//     stage_enemy_db_table.erase(enemy_iter);
// }

// void battletest::erase_stage(uint64_t _id)
// {
//     stage_db stage_db_table(_self, _self.value);
//     auto stage_iter = stage_db_table.find(_id);
//     eosio_assert(stage_iter != stage_db_table.end(), "Not Exist Stage 1");
//     stage_db_table.erase(stage_iter);
// }
void battletest::erase_stage_monster_list(uint64_t _id)
{
    stage_db stage_db_table(_self, _self.value);
    auto stage_iter = stage_db_table.find(_id);
    eosio_assert(stage_iter != stage_db_table.end(), "Not Exist Stage 1");
    stage_db_table.modify(stage_iter, _self, [&](auto &del_mon) {
        del_mon.monster_list.clear();
    });
}

// void battletest::erase_stage_reward(uint64_t _id)
// {
//     reward_db reward_db_table(_self, _self.value);
//     auto reward_iter = reward_db_table.find(_id);
//     eosio_assert(reward_iter != reward_db_table.end(), "Not Exist Stage ID 4");
//     reward_db_table.erase(reward_iter);
// }

void battletest::erase_stage_reward_list(uint64_t _id)
{
    reward_db reward_db_table(_self, _self.value);
    auto reward_iter = reward_db_table.find(_id);
    eosio_assert(reward_iter != reward_db_table.end(), "Not Exist Stage ID 5");
    reward_db_table.modify(reward_iter, _self, [&](auto &new_data) {
        new_data.reward_list.clear();
    });
}

// void battletest::erase_class_state(uint64_t _id)
// {
//     class_stat_db class_stat_db_table(_self, _self.value);
//     auto class_iter = class_stat_db_table.find(_id);
//     eosio_assert(class_iter != class_stat_db_table.end(), "Not Exist Class 1");
//     class_stat_db_table.erase(class_iter);
// }

// void battletest::erase_job_state(uint64_t _id)
// {
//     job_stat_db class_stat_db_table(_self, _self.value);
//     auto class_iter = class_stat_db_table.find(_id);
//     eosio_assert(class_iter != class_stat_db_table.end(), "Not Exist job 1");
//     class_stat_db_table.erase(class_iter);
// }

// void battletest::erase_itemshop(uint64_t _id)
// {
//     item_shop item_shop_table(_self, _self.value);
//     auto item_shop_iter = item_shop_table.find(_id);
//     eosio_assert(item_shop_iter != item_shop_table.end(), "Not Exist itemshop 1");
//     item_shop_table.erase(item_shop_iter);
// }


//=============================================================================================
// ACTION battletest::movedb(eosio::name _user)
// {
//     system_master system_master_table(_self, _self.value);
//     auto system_master_iter = system_master_table.begin();

//     permission_level master_auth;
//     master_auth.actor = system_master_iter->master;
//     master_auth.permission = "active"_n;
//     require_auth(master_auth);
//     eosio_assert(system_master_iter->state == system_state::pause, "movedb : Not Server Pause 3");

//     pre_users pre_user_table(_self, _self.value);
//     auto pre_user_iter = pre_user_table.find(_user.value);
//     eosio_assert(pre_user_iter != pre_user_table.end(), "movedb : Not exist user data");

//     pre_logs pre_log_table(_self, _self.value);
//     auto pre_log_iter = pre_log_table.find(_user.value);
//     eosio_assert(pre_log_iter != pre_log_table.end(), "movedb : Not exist pre log data ");

//     user_logs user_log_table(_self, _self.value);
//     auto user_log_iter = user_log_table.find(pre_log_iter->user.value);

//     if (user_log_iter == user_log_table.end())
//     {
//         user_log_table.emplace(_self, [&](auto &user_log) {
//             user_log.user = pre_log_iter->user;
//             user_log.servant_num = pre_log_iter->servant_num;
//             user_log.monster_num = pre_log_iter->monster_num + 1;
//             user_log.equipment_num = pre_log_iter->item_num;
//             user_log.gacha_num = pre_log_iter->gacha_num;
//             user_log.item_num = 0;
//             user_log.get_utg = 0;
//             user_log.use_utg = 0;
//             user_log.use_eos = pre_log_iter->use_eos;
//             user_log.battle_count = 0;
//             user_log.last_stage_num = 0;
//             user_log.last_tower_num = 0;
//             user_log.top_clear_stage = 0;
//             user_log.top_clear_tower = 0;
//             user_log.add_party_count = 0;
//             user_log.soul_powder = 0;
//             user_log.mail = pre_log_iter->servant_num + pre_log_iter->monster_num + pre_log_iter->item_num;
//         });
//         set_eos_log(pre_log_iter->use_eos);

//         user_auths auth_user_table(_self, _self.value);
//         auto auth_user_iter = auth_user_table.find(pre_user_iter->user.value);
//         eosio_assert(auth_user_iter == auth_user_table.end(), "movedb : Not exist auth_user");
//         auth_user_table.emplace(_self, [&](auto &auth_user) {
//             auth_user.user = pre_user_iter->user;
//             auth_user.state = user_state::lobby;
//             auth_user.exp = 0;
//             auth_user.current_servant_inventory = 1;
//             auth_user.current_monster_inventory = 1;
//             auth_user.current_equipment_inventory = 0;
//             auth_user.current_item_inventory = 0;
//             auth_user.servant_inventory = 50;
//             auth_user.monster_inventory = 50;
//             auth_user.equipment_inventory = 50;
//             auth_user.item_inventory = 50;
//         });
//     }

//     pre_log_table.erase(pre_log_iter);
//     pre_user_table.erase(pre_user_iter);

//     user_preregist_servants user_preregist_servant_table(_self, _user.value);
//     for (auto iter2 = user_preregist_servant_table.begin(); iter2 != user_preregist_servant_table.end();)
//     {
//         const auto &user_preregist_servant_iter = user_preregist_servant_table.get(iter2->primary_key(), "movedb : not exsit preregist servant data");

//         pre_gacha_db pre_gacha_db_table(_self, _self.value);
//         auto pre_gacha_db_iter = pre_gacha_db_table.find(user_preregist_servant_iter.id);
//         eosio_assert(pre_gacha_db_iter != pre_gacha_db_table.end(), "movedb : Not exist pre_gacha_db_iter by servant");

//         user_mail mail_db_table(_self, _user.value);
//         mail_db_table.emplace(_self, [&](auto &move_mail) {
//             uint32_t first_index = mail_db_table.available_primary_key();
//             if (first_index == 0)
//             {
//                 move_mail.mail_index = 1;
//             }
//             else
//             {
//                 move_mail.mail_index = mail_db_table.available_primary_key();
//             }
//             move_mail.mail_type = 1;
//             move_mail.type_index = user_preregist_servant_iter.index;
//             move_mail.count = 1;
//             move_mail.icon_id = pre_gacha_db_iter->db_index;
//             move_mail.get_time = now();
//         });
//         iter2++;
//     }
//     //    for(auto iter2 = user_preregist_monster_table.begin(); iter2 != user_preregist_monster_table.end();)
//     //    {
//     //       //  user_preregist_servant_table.erase(_user.value);
//     //        iter2++;
//     //    }

//     user_preregist_monsters user_preregist_monster_table(_self, _user.value);
//     for (auto iter3 = user_preregist_monster_table.begin(); iter3 != user_preregist_monster_table.end();)
//     {
//         const auto &user_preregist_monster_iter = user_preregist_monster_table.get(iter3->primary_key(), "movedb : Not exsit preregist monster data");

//         pre_gacha_db pre_gacha_db_table(_self, _self.value);
//         auto pre_gacha_db_iter = pre_gacha_db_table.find(user_preregist_monster_iter.id);
//         eosio_assert(pre_gacha_db_iter != pre_gacha_db_table.end(), "movedb : Not exist pre_gacha_db_iter by monster");

//         user_mail mail_db_table(_self, _user.value);
//         mail_db_table.emplace(_self, [&](auto &move_mail) {
//             uint32_t first_index = mail_db_table.available_primary_key();
//             if (first_index == 0)
//             {
//                 move_mail.mail_index = 1;
//             }
//             else
//             {
//                 move_mail.mail_index = mail_db_table.available_primary_key();
//             }
//             move_mail.mail_type = 2;
//             move_mail.type_index = user_preregist_monster_iter.index;
//             move_mail.count = 1;
//             move_mail.icon_id = pre_gacha_db_iter->db_index;
//             move_mail.get_time = now();
//         });
//         iter3++;
//     }
//     // for(auto iter3 = user_preregist_monster_table.begin(); iter3 != user_preregist_monster_table.end();)
//     // {
//     //     //user_preregist_monster_table.erase(_user.value);
//     //     iter3++;
//     // }

//     user_preregist_items user_preregist_item_table(_self, _user.value);
//     for (auto iter4 = user_preregist_item_table.begin(); iter4 != user_preregist_item_table.end();)
//     {
//         const auto &user_preregist_item_iter = user_preregist_item_table.get(iter4->primary_key(), "movedb : Not exsit preregist item data");

//         pre_gacha_db pre_gacha_db_table(_self, _self.value);
//         auto pre_gacha_db_iter = pre_gacha_db_table.find(user_preregist_item_iter.id);
//         eosio_assert(pre_gacha_db_iter != pre_gacha_db_table.end(), "movedb : Not exist pre_gacha_db_iter by item");

//         user_mail mail_db_table(_self, _user.value);
//         mail_db_table.emplace(_self, [&](auto &move_mail) {
//             uint32_t first_index = mail_db_table.available_primary_key();
//             if (first_index == 0)
//             {
//                 move_mail.mail_index = 1;
//             }
//             else
//             {
//                 move_mail.mail_index = mail_db_table.available_primary_key();
//             }
//             move_mail.mail_type = 3;
//             move_mail.type_index = user_preregist_item_iter.index;
//             move_mail.count = 1;
//             move_mail.icon_id = pre_gacha_db_iter->db_index;
//             move_mail.get_time = now();
//         });
//         iter4++;
//     }

//     // for(auto iter4 = user_preregist_item_table.begin(); iter4 != user_preregist_item_table.end();)
//     // {
//     //     //user_preregist_item_table.erase(_user.value);
//     //     iter4++;
//     // }

//     user_partys user_party_table(_self, _user.value);
//     auto user_party_iter = user_party_table.find(1);
//     eosio_assert(user_party_iter == user_party_table.end(), "Movedb : Already Party Table / Already Signup");
//     user_party_table.emplace(_self, [&](auto &automatic_party) {
//         automatic_party.index = 1;
//         automatic_party.servant_list.resize(5);
//         automatic_party.monster_list.resize(5);
//         automatic_party.servant_list[0] = 1;
//     });

//     servant_job_db servant_job_table(_self, _self.value);
//     uint64_t _seed = safeseed::get_seed_value(_user.value, now());
//     uint32_t random_job = 1;
//     const auto &servant_job_db_iter = servant_job_table.get(random_job, "Movedb : Empty Servant Job / Wrong Servant Job");
//     uint32_t random_body = gacha_servant_body(_seed, servant_random_count);

//     servant_random_count += 1;
//     gender_db gender_table(_self, _self.value);
//     uint32_t random_gender = safeseed::get_random_value(_seed, GEMDER_COUNT, DEFAULT_MIN_DB, servant_random_count);
//     const auto &gender_db_iter = gender_table.get(random_gender, "Movedb : Empty Servant Gender / Wrong Servant Gender");

//     servant_random_count += 1;
//     uint32_t random_head = gacha_servant_head(_seed, servant_random_count);

//     servant_random_count += 1;
//     uint32_t random_hair = gacha_servant_hair(_seed, servant_random_count);

//     servant_db servant_id_table(_self, _self.value);
//     uint32_t servant_index = get_servant_index(random_job, random_body, random_gender, random_head, random_hair);
//     const auto &servant_id_db_iter = servant_id_table.get(servant_index, "Movedb : Empty Servant ID / Wrong Servant ID");

//     serstat_db servant_base_table(_self, _self.value);
//     uint32_t servant_job_base = (servant_id_db_iter.job * 1000) + (servant_id_db_iter.grade * 100) + 1;
//     const auto &ser_iter = servant_base_table.get(servant_job_base, "Movedb Servant : Empty Servant Stat");

//     user_servants user_servant_table(_self, _user.value);
//     user_servant_table.emplace(_self, [&](auto &update_user_servant_list) {
//         update_user_servant_list.index = 1;

//         servant_info new_servant;
//         new_servant.id = servant_id_db_iter.id;
//         new_servant.exp = 0;
//         new_servant.grade = 5;
//         servant_random_count += 1;
//         new_servant.status.basic_str = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_str, servant_job_db_iter.min_range.base_str, servant_random_count);
//         servant_random_count += 1;
//         new_servant.status.basic_dex = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_dex, servant_job_db_iter.min_range.base_dex, servant_random_count);
//         servant_random_count += 1;
//         new_servant.status.basic_int = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_int, servant_job_db_iter.min_range.base_int, servant_random_count);

//         new_servant.status.basic_str = change_servant_statue(new_servant.status.basic_str) + ser_iter.base_str;
//         new_servant.status.basic_dex = change_servant_statue(new_servant.status.basic_dex) + ser_iter.base_dex;
//         new_servant.status.basic_int = change_servant_statue(new_servant.status.basic_int) + ser_iter.base_int;
//         new_servant.equip_slot.resize(3);
//         new_servant.state = object_state::on_party;

//         uint32_t active_id = get_servant_active_skill(servant_id_db_iter.job, _seed);
//         new_servant.active_skill.push_back(active_id);

//         uint32_t passive_id = get_servant_passive_skill(servant_id_db_iter.job, _seed);
//         new_servant.passive_skill.push_back(passive_id);

//         update_user_servant_list.party_number = 1;
//         update_user_servant_list.servant = new_servant;
//     });

//     monster_random_count += 1;
//     uint32_t random_monster_id = safeseed::get_random_value(_seed, 7, 1, monster_random_count);
//     random_monster_id += 103100;

//     monster_db monster_id_db_table(_self, _self.value);
//     const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id, "Movedb Monster : Empty Monster ID");

//     monster_random_count += 1;
//     uint64_t random_grade = 3;

//     monster_grade_db monster_grade_db_table(_self, _self.value);
//     const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "Movedb Monster : Empty Grade");

//     tribe_db tribe_db_table(_self, _self.value);
//     const auto &tribe_iter = tribe_db_table.get(monster_id_db_iter.tribe, "Movedb Monster : Empty Monster Tribe");

//     user_monsters user_monster_table(_self, _user.value);
//     user_monster_table.emplace(_self, [&](auto &update_user_monster_list) {
//         uint32_t first_index = user_monster_table.available_primary_key();
//         if (first_index == 0)
//         {
//             update_user_monster_list.index = 1;
//         }
//         else
//         {
//             update_user_monster_list.index = user_monster_table.available_primary_key();
//         }

//         monster_info new_monster;
//         new_monster.id = monster_id_db_iter.id;
//         new_monster.grade = monster_grade_db_iter.grade;
//         new_monster.tribe = monster_id_db_iter.tribe;
//         new_monster.type = monster_id_db_iter.type;
//         new_monster.exp = 0;
//         new_monster.upgrade = 0;
//         monster_random_count += 1;
//         new_monster.status.basic_str = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_str, monster_grade_db_iter.min_range.base_str, monster_random_count);
//         monster_random_count += 1;
//         new_monster.status.basic_dex = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_dex, monster_grade_db_iter.min_range.base_dex, monster_random_count);
//         monster_random_count += 1;
//         new_monster.status.basic_int = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_int, monster_grade_db_iter.min_range.base_int, monster_random_count);

//         new_monster.status.basic_str = change_monster_statue(new_monster.grade, new_monster.status.basic_str);
//         new_monster.status.basic_dex = change_monster_statue(new_monster.grade, new_monster.status.basic_dex);
//         new_monster.status.basic_int = change_monster_statue(new_monster.grade, new_monster.status.basic_int);

//         new_monster.status.basic_str = (new_monster.status.basic_str * tribe_iter.base_str) / 100;
//         new_monster.status.basic_dex = (new_monster.status.basic_dex * tribe_iter.base_dex) / 100;
//         new_monster.status.basic_int = (new_monster.status.basic_int * tribe_iter.base_int) / 100;

//         new_monster.state = object_state::on_inventory;

//         uint32_t passive_id = get_monster_passive_skill(_seed);
//         new_monster.passive_skill.push_back(passive_id);

//         update_user_monster_list.party_number = EMPTY_PARTY;
//         update_user_monster_list.monster = new_monster;
//     });

//     asset utg_cheat_money(0, symbol(symbol_code("UTG"), 4));
//     utg_cheat_money.amount = 1000 * 10000;

//     action(permission_level{_self, "active"_n},
//            _self, "transfer"_n,
//            std::make_tuple(_self, _user, utg_cheat_money, std::string("SignUp Reward")))
//         .send();


// }

// ACTION battletest::dbinit(std::string _table)
// {
//     system_master system_master_table(_self, _self.value);
//     auto system_master_iter = system_master_table.begin();

//     permission_level master_auth;
//     master_auth.actor = system_master_iter->master;
//     master_auth.permission = "active"_n;
//     require_auth(master_auth);

//     eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 4");

//     // if (_table == "dbitemshop")
//     // {
//     //     item_shop my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }


//     // if (_table == "dbbody")
//     // {
//     //     body_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbhair")
//     // {
//     //     hair_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbhead")
//     // {
//     //     head_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbgender")
//     // {
//     //     gender_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbitemgrade")
//     // {
//     //     item_grade_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbmonstergd")
//     // {
//     //     monster_grade_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbservantjob")
//     // {
//     //     servant_job_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }

//     // else if (_table == "dbgraderatio")
//     // {
//     //     grade_ratio_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbequipup")
//     // {
//     //     upgrade_equipment_ratio_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }

//     // if (_table == "dbtribe")
//     // {
//     //     tribe_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbmonsterup")
//     // {
//     //     upgrade_monster_ratio_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbitemup")
//     // {
//     //     upgrade_item_ratio_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dblevel")
//     // {
//     //     lv_exp my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }

//     // else if (_table == "dbstatusserv")
//     // {
//     //     servant_lv_status_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbstatusmon")
//     // {
//     //     monster_lv_status_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // if (_table == "dbpassive")
//     // {
//     //     passive_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // if (_table == "dbactive")
//     // {
//     //     active_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbmonsters")
//     // {
//     //     monster_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbservants")
//     // {
//     //     servant_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // if (_table == "dbequipment")
//     // {
//     //     equipment_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dballitem")
//     // {
//     //     allitem_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbservantid")
//     // {
//     //     servant_id_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbmonsterid")
//     // {
//     //     monster_id_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // else if (_table == "dbitemid")
//     // {
//     //     item_id_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // if (_table == "dbgachapool")
//     // {
//     //     main_gacha_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//         if (_table == "dbreward")
//     {
//         reward_db my_table(_self, _self.value);
//         for (auto iter = my_table.begin(); iter != my_table.end();)
//         {
//             auto erase_iter = my_table.find(iter->primary_key());
//             iter++;
//             my_table.erase(erase_iter);
//         }
//     }
//     // else
//     // {
//     //     eosio_assert(1 == 0, "Not Exist Table");
//     // }
// }

//  ACTION battletest::setdata(eosio::name _contract, eosio::name _user, std::string _table)
//  {
//     require_auth(_self);

    // if (_table == "preauth")
    // {
    //     pre_users pre_users_table(_contract, _contract.value);
    //     pre_users my_table(_self, _self.value);
    //     for (auto iter = pre_users_table.begin(); iter != pre_users_table.end();)
    //     {
    //         const auto &get_iter = pre_users_table.get(iter->primary_key(), "not exist data ");

    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.user = get_iter.user;
    //             new_data.state = get_iter.state;
    //         });
    //         iter++;
    //     }
    // }
    // else if (_table == "prelog")
    // {
    //     pre_logs pre_logs_table(_contract, _contract.value);
    //     pre_logs my_table(_self, _self.value);
    //     for (auto iter = pre_logs_table.begin(); iter != pre_logs_table.end();)
    //     {
    //         const auto &get_iter = pre_logs_table.get(iter->primary_key(), "not exist data ");

    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.user = get_iter.user;
    //             new_data.servant_num = get_iter.servant_num;
    //             new_data.monster_num = get_iter.monster_num;
    //             new_data.item_num = get_iter.item_num;
    //             new_data.gacha_num = get_iter.gacha_num;
    //             new_data.use_eos = get_iter.use_eos;
    //         });
    //         iter++;
    //     }
    // }
    // if (_table == "preservant")
    // {
    //     user_preregist_servants user_preregist_servants_table(_contract, _user.value);
    //     user_preregist_servants my_table(_self, _user.value);
    //     for (auto iter = user_preregist_servants_table.begin(); iter != user_preregist_servants_table.end();)
    //     {
    //         const auto &get_iter = user_preregist_servants_table.get(iter->primary_key(), "not exist data ");

    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.index = get_iter.index;
    //             new_data.id = get_iter.id;
    //             new_data.status.basic_str = get_iter.status.basic_str;
    //             new_data.status.basic_dex = get_iter.status.basic_dex;
    //             new_data.status.basic_int = get_iter.status.basic_int;

    //         });
    //         iter++;
    //     }
    // }

    // if (_table == "premonster")
    // {
    //     user_preregist_monsters user_preregist_monsters_table(_contract, _user.value);
    //     user_preregist_monsters my_table(_self, _user.value);
    //     for (auto iter = user_preregist_monsters_table.begin(); iter != user_preregist_monsters_table.end();)
    //     {
    //         const auto &get_iter = user_preregist_monsters_table.get(iter->primary_key(), "not exist data ");

    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.index = get_iter.index;
    //             new_data.id = get_iter.id;
    //             new_data.grade = get_iter.grade;
    //             new_data.status.basic_str = get_iter.status.basic_str;
    //             new_data.status.basic_dex = get_iter.status.basic_dex;
    //             new_data.status.basic_int = get_iter.status.basic_int;

    //         });
    //         iter++;
    //     }
    // }

    // else if (_table == "preitem")
    // {
    //     user_preregist_items user_preregist_items_table(_contract, _user.value);
    //     user_preregist_items my_table(_self, _user.value);
    //     for (auto iter = user_preregist_items_table.begin(); iter != user_preregist_items_table.end();)
    //     {
    //         const auto &get_iter = user_preregist_items_table.get(iter->primary_key(), "not exist data ");

    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.index = get_iter.index;
    //             new_data.id = get_iter.id;
    //             new_data.type = get_iter.type;
    //             new_data.tier = get_iter.tier;
    //             new_data.job = get_iter.job;
    //             new_data.grade = get_iter.grade;
    //             new_data.main_status = get_iter.main_status;
    //         });
    //         iter++;
    //     }
    // }

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
    //                 new_data.active_turn = get_iter.target_active_turnrange;
    //                 new_data.hit_count = get_iter.hit_count;
    //                 new_data.atk_per_1 = get_iter.atk_per_1;
    //                 new_data.atk_per_add_1 = get_iter.atk_per_add_1;
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
    //         });
    //         iter++;
    //     }
    // }
//     if (_table == "dbitemshop")
//    {
//        item_shop other_table(_contract, _contract.value);
//        item_shop my_table(_self, _self.value);
//        for (auto iter = other_table.begin(); iter != other_table.end();)
//        {
//            const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
//            my_table.emplace(_self, [&](auto &new_data) {
//                new_data.id = get_iter.id;
//                new_data.shop_type = get_iter.shop_type;
//                new_data.product_id = get_iter.product_id;
//                new_data.product_count = get_iter.product_count;
//                new_data.product_limit_max = get_iter.product_limit_max;
//                new_data.price_id = get_iter.price_id;
//                new_data.price_count = get_iter.price_count;
//            });
//            iter++;
//        }
//    }

//    if (_table == "tshoplist")
//    {
//        shop_list other_table(_contract, _contract.value);
//        shop_list my_table(_self, _self.value);
//        for (auto iter = other_table.begin(); iter != other_table.end();)
//        {
//            const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
//            my_table.emplace(_self, [&](auto &new_data) {
//                new_data.id = get_iter.id;
//                new_data.shop_type = get_iter.shop_type;
//                new_data.shop_item_id = get_iter.shop_item_id;
//                new_data.limit_count = get_iter.limit_count;
//            });
//            iter++;
//        }
//    }
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

    // else if (_table == "dbprepool")
    // {
    //     pre_gacha_db other_table(_contract, _contract.value);
    //     pre_gacha_db my_table(_self, _self.value);
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
//}
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

        // total_token_logs total_token_log_table(_self, _self.value);
        // auto total_token_log_iter = total_token_log_table.find(system_master_iter->master.value);

        // if (total_token_log_iter != total_token_log_table.end())
        // {
        //     total_token_log_table.emplace(_self, [&](auto &move_master) {
        //         move_master.owner = _master;
        //         move_master.total_token_amount = total_token_log_iter->total_token_amount;
        //     });
        //     total_token_log_table.erase(total_token_log_iter);
        // }

        system_master_table.emplace(_self, [&](auto &move_master) {
            move_master.master = _master;
            move_master.state = system_state::pause;
        });
        system_master_table.erase(system_master_iter);
    }
}


#pragma endregion

//------------------------------------------------------------------------//
//-------------------------------login_action-----------------------------//
//------------------------------------------------------------------------//

#pragma region login

void battletest::signup(eosio::name _user, uint64_t _use_eos)
{
    user_auths auth_user_table(_self, _self.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "Signup : Already Auth Table / Already Signup");
    auth_user_table.emplace(_self, [&](auto &new_user) {
        new_user.user = _user;
        new_user.state = user_state::lobby;
        new_user.exp = 0;
        new_user.rank = 1;
        new_user.current_servant_inventory = 1;
        new_user.current_monster_inventory = 1;
        new_user.current_item_inventory = 0;
        new_user.current_equipment_inventory = 0;
        new_user.servant_inventory = 50;
        new_user.monster_inventory = 50;
        new_user.item_inventory = 50;
        new_user.equipment_inventory = 50;
    });

    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.find(1);
    eosio_assert(user_party_iter == user_party_table.end(), "Signup : Already Party Table / Already Signup");
    user_party_table.emplace(_self, [&](auto &automatic_party) {
        automatic_party.index = 1;
        automatic_party.servant_list.resize(5);
        automatic_party.monster_list.resize(5);
        automatic_party.servant_list[0] = 1;
    });

    servant_job_db servant_job_table(_self, _self.value);
    uint64_t _seed = safeseed::get_seed_value(_user.value, now());
    uint32_t random_job = 1;
    const auto &servant_job_db_iter = servant_job_table.get(random_job, "Signup : Empty Servant Job / Wrong Servant Job");
    uint32_t random_body = gacha_servant_body(_seed, servant_random_count);

    servant_random_count += 1;
    gender_db gender_table(_self, _self.value);
    uint32_t random_gender = safeseed::get_random_value(_seed, GEMDER_COUNT, DEFAULT_MIN_DB, servant_random_count);
    const auto &gender_db_iter = gender_table.get(random_gender, "Signup : Empty Servant Gender / Wrong Servant Gender");

    servant_random_count += 1;
    uint32_t random_head = gacha_servant_head(_seed, servant_random_count);

    servant_random_count += 1;
    uint32_t random_hair = gacha_servant_hair(_seed, servant_random_count);

    servant_db servant_id_table(_self, _self.value);
    uint32_t servant_index = get_servant_index(random_job, random_body, random_gender, random_head, random_hair);
    const auto &servant_id_db_iter = servant_id_table.get(servant_index, "Signup : Empty Servant ID / Wrong Servant ID");

    serstat_db servant_base_table(_self, _self.value);
    uint32_t servant_job_base = (servant_id_db_iter.job * 1000) + (servant_id_db_iter.grade * 100) + 1;
    const auto &ser_iter = servant_base_table.get(servant_job_base, "Signup Servant : Empty Servant Stat");

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
        new_servant.status.basic_str = change_servant_statue(new_servant.status.basic_str) + ser_iter.base_str;
        new_servant.status.basic_dex = change_servant_statue(new_servant.status.basic_dex) + ser_iter.base_dex;
        new_servant.status.basic_int = change_servant_statue(new_servant.status.basic_int) + ser_iter.base_int;
        new_servant.equip_slot.resize(3);
        new_servant.state = object_state::on_party;

        uint32_t active_id = get_servant_active_skill(servant_id_db_iter.job, _seed);
        new_servant.active_skill.push_back(active_id);

        uint32_t passive_id = get_servant_passive_skill(servant_id_db_iter.job, _seed);
        new_servant.passive_skill.push_back(passive_id);

        update_user_servant_list.party_number = 1;
        update_user_servant_list.servant = new_servant;
    });


    monster_random_count += 1;
    uint32_t random_monster_id = safeseed::get_random_value(_seed, 7, 1, monster_random_count);
    random_monster_id += 103100;

    monster_db monster_id_db_table(_self, _self.value);
    const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id, "Signup Monster : Empty Monster ID");

    tribe_db tribe_db_table(_self, _self.value);
    const auto &tribe_iter = tribe_db_table.get(monster_id_db_iter.tribe, "Signup Monster : Empty Monster Tribe");

    monster_random_count += 1;
    uint64_t random_grade = 3;

    monster_grade_db monster_grade_db_table(_self, _self.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "Signup Monster : Empty Grade");

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
        new_monster.tribe = monster_id_db_iter.tribe;
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

        new_monster.status.basic_str = (new_monster.status.basic_str * tribe_iter.base_str) / 100;
        new_monster.status.basic_dex = (new_monster.status.basic_dex * tribe_iter.base_dex) / 100;
        new_monster.status.basic_int = (new_monster.status.basic_int * tribe_iter.base_int) / 100;

        new_monster.state = object_state::on_inventory;

        uint32_t passive_id = get_monster_passive_skill(_seed);
        new_monster.passive_skill.push_back(passive_id);

        update_user_monster_list.party_number = EMPTY_PARTY;
        update_user_monster_list.monster = new_monster;
    });


    asset utg_cheat_money(0, symbol(symbol_code("UTG"), 4));
    utg_cheat_money.amount = 1000 * 10000;

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter == user_log_table.end(), "Signup : Already Log Table / Already Signup");
    user_log_table.emplace(_self, [&](auto &new_log) {
        new_log.user = _user;
        new_log.use_eos += _use_eos;
        new_log.servant_num += 1;
        new_log.monster_num += 1;
        new_log.get_utg += utg_cheat_money.amount;
    });

    action(permission_level{_self, "active"_n},
           _self, "transfer"_n,
           std::make_tuple(_self, _user, utg_cheat_money, std::string("SignUp Reward")))
        .send();
}

void battletest::refer_signup(eosio::name _user, eosio::name _refer, uint64_t _use_eos)
{
    user_auths auth_user_table(_self, _self.value);
    auto new_user_iter = auth_user_table.find(_user.value);
    eosio_assert(new_user_iter == auth_user_table.end(), "Refer : Already Auth Table / Already Signup");
    auth_user_table.emplace(_self, [&](auto &new_user) {
        new_user.user = _user;
        new_user.state = user_state::lobby;
        new_user.exp = 0;
        new_user.rank = 1;
        new_user.current_servant_inventory = 1;
        new_user.current_monster_inventory = 1;
        new_user.current_item_inventory = 0;
        new_user.current_equipment_inventory = 0;
        new_user.servant_inventory = 50;
        new_user.monster_inventory = 50;
        new_user.item_inventory = 50;
        new_user.equipment_inventory = 50;
    });

    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.find(1);
    eosio_assert(user_party_iter == user_party_table.end(), "Refer : Already Party Table / Already Signup");
    user_party_table.emplace(_self, [&](auto &automatic_party) {
        automatic_party.index = 1;
        automatic_party.servant_list.resize(5);
        automatic_party.monster_list.resize(5);
        automatic_party.servant_list[0] = 1;
    });

    servant_job_db servant_job_table(_self, _self.value);
    uint64_t _seed = safeseed::get_seed_value(_user.value, now());
    uint32_t random_job = 1;
    const auto &servant_job_db_iter = servant_job_table.get(random_job, "Refer : Empty Servant Job / Wrong Servant Job");
    uint32_t random_body = gacha_servant_body(_seed, servant_random_count);

    servant_random_count += 1;
    gender_db gender_table(_self, _self.value);
    uint32_t random_gender = safeseed::get_random_value(_seed, GEMDER_COUNT, DEFAULT_MIN_DB, servant_random_count);
    const auto &gender_db_iter = gender_table.get(random_gender, "Refer : Empty Servant Gender / Wrong Servant Gender");

    servant_random_count += 1;
    uint32_t random_head = gacha_servant_head(_seed, servant_random_count);

    servant_random_count += 1;
    uint32_t random_hair = gacha_servant_hair(_seed, servant_random_count);

    servant_db servant_id_table(_self, _self.value);
    uint32_t servant_index = get_servant_index(random_job, random_body, random_gender, random_head, random_hair);
    const auto &servant_id_db_iter = servant_id_table.get(servant_index, "Refer : Empty Servant ID / Wrong Servant ID");

    serstat_db servant_base_table(_self, _self.value);
    uint32_t servant_job_base = (servant_id_db_iter.job * 1000) + (servant_id_db_iter.grade * 100) + 1;
    const auto &ser_iter = servant_base_table.get(servant_job_base, "Refer Servant : Empty Servant Stat");

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
        new_servant.status.basic_str = change_servant_statue(new_servant.status.basic_str) + ser_iter.base_str;
        new_servant.status.basic_dex = change_servant_statue(new_servant.status.basic_dex) + ser_iter.base_dex;
        new_servant.status.basic_int = change_servant_statue(new_servant.status.basic_int) + ser_iter.base_int;
        new_servant.equip_slot.resize(3);
        new_servant.state = object_state::on_party;

        uint32_t active_id = get_servant_active_skill(servant_id_db_iter.job, _seed);
        new_servant.active_skill.push_back(active_id);

        uint32_t passive_id = get_servant_passive_skill(servant_id_db_iter.job, _seed);
        new_servant.passive_skill.push_back(passive_id);

        update_user_servant_list.party_number = 1;
        update_user_servant_list.servant = new_servant;
    });


    monster_random_count += 1;
    uint32_t random_monster_id = safeseed::get_random_value(_seed, 7, 1, monster_random_count);
    random_monster_id += 103100;

    monster_db monster_id_db_table(_self, _self.value);
    const auto &monster_id_db_iter = monster_id_db_table.get(random_monster_id, "Signup Monster : Empty Monster ID");

    tribe_db tribe_db_table(_self, _self.value);
    const auto &tribe_iter = tribe_db_table.get(monster_id_db_iter.tribe, "Signup Monster : Empty Monster Tribe");

    uint64_t random_grade = 3;

    monster_grade_db monster_grade_db_table(_self, _self.value);
    const auto &monster_grade_db_iter = monster_grade_db_table.get(random_grade, "Signup Monster : Empty Grade");

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
        new_monster.tribe = monster_id_db_iter.tribe;
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

        new_monster.status.basic_str = (new_monster.status.basic_str * tribe_iter.base_str) / 100;
        new_monster.status.basic_dex = (new_monster.status.basic_dex * tribe_iter.base_dex) / 100;
        new_monster.status.basic_int = (new_monster.status.basic_int * tribe_iter.base_int) / 100;

        new_monster.state = object_state::on_inventory;

        uint32_t passive_id = get_monster_passive_skill(_seed);
        new_monster.passive_skill.push_back(passive_id);

        update_user_monster_list.party_number = EMPTY_PARTY;
        update_user_monster_list.monster = new_monster;
    });


    asset refer_signup_reward(0, symbol(symbol_code("UTG"), 4));
    refer_signup_reward.amount = 11000000;  //1100 UTG

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter == user_log_table.end(), "Refer : Already Log Table / Already Signup");
    user_log_table.emplace(_self, [&](auto &new_log) {
        new_log.user = _user;
        new_log.use_eos += _use_eos;
        new_log.servant_num += 1;
        new_log.monster_num += 1;
        new_log.get_utg += refer_signup_reward.amount;
    });


    action(permission_level{_self, "active"_n},
           _self, "transfer"_n,
           std::make_tuple(_self, _user, refer_signup_reward, std::string("Refer SignUp Reward")))
        .send();

    referlist refermaster_table(_self, _self.value);
    auto referlist_iter = refermaster_table.find(_refer.value);
    eosio_assert(referlist_iter != refermaster_table.end(), "Refer Signup : Not Exist Referer"); 

    referlist referlist_table(_self, referlist_iter->referer.value);    
    referlist_table.emplace(_self, [&](auto &regist_user) {
        regist_user.referer = _user;
    });

    asset refer_reward(0, symbol(symbol_code("UTG"), 4));
    refer_reward.amount = 1000000;    // 100 UTG

    auto log_refer_iter = user_log_table.find(referlist_iter->referer.value);
    eosio_assert(log_refer_iter != user_log_table.end(), "Refer : Empty Log Table / Not Yet Signup");
    user_log_table.modify(log_refer_iter, _self, [&](auto &new_log) {
        new_log.get_utg += refer_reward.amount;
    });

    action(permission_level{_self, "active"_n},
           _self, "transfer"_n,
           std::make_tuple(_self, referlist_iter->referer, refer_reward, std::string("Refer Reward")))
        .send();
}


/////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////mail system////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////

ACTION battletest::nftmail(eosio::name _user, std::string _type, uint64_t _token_index, uint64_t _icon_id)
{
    require_auth(NFT_CONTRACT);

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "nftmail : Empty Log Table / Not Yet Signup");

    user_mail user_mail_table(_self, _user.value);
    user_mail_table.emplace(_self, [&](auto &move_mail) {
        uint64_t change_type;
        uint32_t first_index = user_mail_table.available_primary_key();
        if (first_index == 0)
        {
            move_mail.mail_index = 1;
        }
        else
        {
            move_mail.mail_index = user_mail_table.available_primary_key();
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
        move_mail.type_index = _token_index;
        move_mail.count = 1;
        move_mail.icon_id = _icon_id;
        move_mail.get_time = now();
    });

    user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
        update_log.mail += 1;
    });

}


ACTION battletest::mailopen(eosio::name _user, const std::vector<uint64_t> &_mail_index)
{
    system_check(_user);

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "mailopen : Empty user_auth Table / Not Yet Signup");

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "mailopen : Empty Log Table / Not Yet Signup");

    user_mail user_mail_table(_self, _user.value);
    main_gacha_db main_gacha_db_table(_self, _self.value);
    pre_gacha_db pre_gacha_db_table(_self, _self.value);

    for (uint32_t i = 0; i < _mail_index.size(); ++i)
    {
        uint32_t _seed = safeseed::get_seed_value(_user.value + i, now());

        auto user_mail_iter = user_mail_table.find(_mail_index[i]);

        eosio_assert(user_mail_iter != user_mail_table.end(), "mailopen : Not exist mail data");

        eosio_assert((user_mail_iter->mail_type == 1 || user_mail_iter->mail_type == 2 | user_mail_iter->mail_type == 3 || user_mail_iter->mail_type == 4 || user_mail_iter->mail_type == 5 || user_mail_iter->mail_type == 6 || user_mail_iter->mail_type == 7), "Not exist select type");
      
        if (user_mail_iter->mail_type == 1) 
        {
            eosio_assert(check_inventory(_user) == true, "mailopen : your inventory is full");

            uint64_t temp_grade = 5;

            servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
            auto servant_lv_status_db_iter = servant_lv_status_db_table.find(temp_grade); 

            user_preregist_servants user_preregist_servant_table(_self, _user.value);
            auto user_preregist_servant_iter = user_preregist_servant_table.find(user_mail_iter->type_index);

            auto pre_gacha_db_iter = pre_gacha_db_table.find(user_preregist_servant_iter->id);
            eosio_assert(pre_gacha_db_iter != pre_gacha_db_table.end(), "mailopen :Not exist pre_gacha_db_iter");
            eosio_assert(servant_lv_status_db_iter != servant_lv_status_db_table.end(), "mailopen :Not exist servant_lv_status Data");

   
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
                auto servant_db_iter = servant_db_table.find(pre_gacha_db_iter->db_index);
                eosio_assert(servant_db_iter != servant_db_table.end(), "mailopen : Not exist servant_db_iter");

                serstat_db serstat_db_table(_self, _self.value);
                uint32_t stat_id = (1000 * servant_db_iter->job) + (100 *5) + 1;
                auto stat_iter = serstat_db_table.find(stat_id);

                // serstat_db servant_base_table(_self, _self.value);
                // uint32_t servant_job_base = (servant_db_iter->job * 1000) + (servant_db_iter->grade * 100) + 1;
                // const auto &ser_iter = servant_base_table.get(servant_job_base, "mailopen : Empty Servant Stat");

                move_servant.party_number = 0;
                move_servant.servant.state = object_state::on_inventory;
                move_servant.servant.exp = 0;
                move_servant.servant.id = servant_db_iter->id;
                move_servant.servant.grade = 5;

                move_servant.servant.status.basic_str = servant_lv_status_db_iter->change_status[user_preregist_servant_iter->status.basic_str].update_status + stat_iter->base_str;
                move_servant.servant.status.basic_dex = servant_lv_status_db_iter->change_status[user_preregist_servant_iter->status.basic_dex].update_status + stat_iter->base_dex;
                move_servant.servant.status.basic_int = servant_lv_status_db_iter->change_status[user_preregist_servant_iter->status.basic_int].update_status + stat_iter->base_int;
                // move_servant.servant.status.basic_str = change_servant_statue(move_servant.servant.status.basic_str) + ser_iter.base_str;
                // move_servant.servant.status.basic_dex = change_servant_statue(move_servant.servant.status.basic_dex) + ser_iter.base_dex;
                // move_servant.servant.status.basic_int = change_servant_statue(move_servant.servant.status.basic_int) + ser_iter.base_int;

                move_servant.servant.equip_slot.resize(3);

                uint32_t active_id = get_servant_active_skill(servant_db_iter->job, _seed);
                if (active_id != 0)
                {
                    move_servant.servant.active_skill.push_back(active_id);
                }

                uint32_t passive_id = get_servant_passive_skill(servant_db_iter->job, _seed);
                if (passive_id != 0)
                {
                    move_servant.servant.passive_skill.push_back(passive_id);
                }
            });

            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_servant_inventory += 1;
            });

            user_preregist_servant_table.erase(user_preregist_servant_iter);

        }

        else if (user_mail_iter->mail_type == 2) 
        {
            eosio_assert(check_inventory(_user) == true, "mailopen : your inventory is full");

            user_preregist_monsters user_preregist_monster_table(_self, _user.value);
            auto user_preregist_monster_iter = user_preregist_monster_table.find(user_mail_iter->type_index);

            auto pre_gacha_db_iter = pre_gacha_db_table.find(user_preregist_monster_iter->id);
            eosio_assert(pre_gacha_db_iter != pre_gacha_db_table.end(), "mailopen :Not exist pre_gacha_db_iter");

            monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
            auto monster_lv_status_db_iter = monster_lv_status_db_table.find(user_preregist_monster_iter->grade);
            eosio_assert(monster_lv_status_db_iter != monster_lv_status_db_table.end(), "mailopen :  Not exist monster_lv_status Data");

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
                auto monster_db_iter = monster_db_table.find(pre_gacha_db_iter->db_index);
                eosio_assert(monster_db_iter != monster_db_table.end(), "mailopen : Not exist monster_db_iter");

                tribe_db tribe_db_table(_self, _self.value);
                const auto &tribe_iter = tribe_db_table.get(monster_db_iter->tribe, "mailopen : Empty Monster Tribe");

                move_monster.monster.id = monster_db_iter->id;
                move_monster.party_number = 0;
                move_monster.monster.state = object_state::on_inventory;
                move_monster.monster.exp = 0;
                move_monster.monster.type = monster_db_iter->type;
                move_monster.monster.tribe = monster_db_iter->tribe;
                move_monster.monster.grade = user_preregist_monster_iter->grade;
                move_monster.monster.upgrade = 0;
                move_monster.monster.status.basic_str = monster_lv_status_db_iter->change_status[user_preregist_monster_iter->status.basic_str].update_status;
                move_monster.monster.status.basic_dex = monster_lv_status_db_iter->change_status[user_preregist_monster_iter->status.basic_dex].update_status;
                move_monster.monster.status.basic_int = monster_lv_status_db_iter->change_status[user_preregist_monster_iter->status.basic_int].update_status;

                // move_monster.monster.status.basic_str = change_monster_statue(move_monster.monster.grade, move_monster.monster.status.basic_str);
                // move_monster.monster.status.basic_dex = change_monster_statue(move_monster.monster.grade, move_monster.monster.status.basic_dex);
                // move_monster.monster.status.basic_int = change_monster_statue(move_monster.monster.grade, move_monster.monster.status.basic_int);

                move_monster.monster.status.basic_str = (move_monster.monster.status.basic_str * tribe_iter.base_str) / 100;
                move_monster.monster.status.basic_dex = (move_monster.monster.status.basic_dex * tribe_iter.base_dex) / 100;
                move_monster.monster.status.basic_int = (move_monster.monster.status.basic_int * tribe_iter.base_int) / 100;

                uint32_t passive_id = get_monster_passive_skill(_seed);
                move_monster.monster.passive_skill.push_back(passive_id);
            });

            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_monster_inventory += 1;
            });

            user_preregist_monster_table.erase(user_preregist_monster_iter);

        }

        else if (user_mail_iter->mail_type == 3) 
        {
            eosio_assert(check_inventory(_user) == true, "mailopen : your inventory is full");

            user_preregist_items user_preregist_item_table(_self, _user.value);
            auto user_preregist_item_iter = user_preregist_item_table.find(user_mail_iter->type_index);

            equipment_lv_status_db equipment_lv_status_db_table(_self, _self.value);
            uint64_t _check_type = user_preregist_item_iter->type;
            if (_check_type >= 3)
            {
                _check_type = 3;
            }
            uint64_t _check_grade = user_preregist_item_iter->grade;
            uint64_t _check_sum = _check_type * 10 + _check_grade;
            

            auto equipment_lv_status_db_iter = equipment_lv_status_db_table.find(_check_sum);
            eosio_assert(equipment_lv_status_db_iter != equipment_lv_status_db_table.end(), "mailopen : Not exist equipment_lv_status Data");

            auto pre_gacha_db_iter = pre_gacha_db_table.find(user_preregist_item_iter->id);
            eosio_assert(pre_gacha_db_iter != pre_gacha_db_table.end(), "mailopen :Not exist pre_gacha_db_iter");

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
                auto equipitem_db_iter = equipitem_db_table.find(pre_gacha_db_iter->db_index);
                eosio_assert(equipitem_db_iter != equipitem_db_table.end(), "mailopen : Not exist equipment_db_iter3");
                allitem_db commonitem_db_table(_self, _self.value);
                auto commonitem_db_iter = commonitem_db_table.find(pre_gacha_db_iter->db_index);
                eosio_assert(commonitem_db_iter != commonitem_db_table.end(), "mailopen : Not exist commonitem_db_iter 3");

                item_grade_db item_grade_db_table(_self, _self.value);
                auto item_grade_db_iter = item_grade_db_table.find(user_preregist_item_iter->grade);

                move_item.equipment.id = equipitem_db_iter->item_id;
                move_item.equipment.state = object_state::on_inventory;
                move_item.equipment.type = equipitem_db_iter->type;
                move_item.equipment.tier = equipitem_db_iter->tier;
                move_item.equipment.job = equipitem_db_iter->job;
                move_item.equipment.grade = user_preregist_item_iter->grade;
                move_item.equipment.upgrade = 0;

                uint32_t type_grade = ((equipitem_db_iter->type + 1) * 10) + item_grade_db_iter->grade;
                move_item.equipment.value = equipment_lv_status_db_iter->change_status[user_preregist_item_iter->main_status].update_status;
                //move_item.equipment.value = change_equipment_statue(type_grade, move_item.equipment.value);
                set_tier_status(move_item.equipment.value, equipitem_db_iter->tier);
            });

            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_equipment_inventory += 1;
            });
            user_preregist_item_table.erase(user_preregist_item_iter);

        }

        else if (user_mail_iter->mail_type == 4) 
        {
            
        }
        else if (user_mail_iter->mail_type == 5) //UTS
        {
            std::string temp = "UTS";
            eosio_assert(check_inventory(_user) == true, "mailopen : your servant inventory is full 2");
            uts_db uts_db_table(NFT_CONTRACT, NFT_CONTRACT.value);
            auto uts_db_iter = uts_db_table.find(user_mail_iter->type_index);
            eosio_assert(uts_db_iter != uts_db_table.end(), "mailopen : end table error");
            std::string temp_type = "servant";
            nftexchange(uts_db_iter->owner,
                        uts_db_iter->master, 
                        temp_type,
                        uts_db_iter->t_idx);
            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_servant_inventory += 1;
            });
            action(permission_level{get_self(), "active"_n},
                   NFT_CONTRACT, "deletedata"_n,
                   std::make_tuple(temp, uts_db_iter->idx))
                .send();
        }

        else if (user_mail_iter->mail_type == 6) //UTM
        {
            std::string temp = "UTM";
            eosio_assert(check_inventory(_user) == true, "mailopen : your monster inventory is full 2");
            utm_db utm_db_table(NFT_CONTRACT, NFT_CONTRACT.value);
            auto utm_db_iter = utm_db_table.find(user_mail_iter->type_index);
            eosio_assert(utm_db_iter != utm_db_table.end(), "mailopen : end table error");
            std::string temp_type = "monster";
            nftexchange(utm_db_iter->owner, 
                        utm_db_iter->master, 
                        temp_type, 
                        utm_db_iter->t_idx);
            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_monster_inventory += 1;
            });
            action(permission_level{get_self(), "active"_n},
                   NFT_CONTRACT, "deletedata"_n,
                   std::make_tuple(temp, utm_db_iter->idx))
                .send();
        }
        else if (user_mail_iter->mail_type == 7) //UTI
        {
            std::string temp = "UTI";
            eosio_assert(check_inventory(_user) == true, "mailopen : your equipment inventory is full 2");
            uti_db uti_db_table(NFT_CONTRACT, NFT_CONTRACT.value);
            auto uti_db_iter = uti_db_table.find(user_mail_iter->type_index);
             eosio_assert(uti_db_iter != uti_db_table.end(), "mailopen : end table error");
            std::string temp_type = "equipment";
            nftexchange(uti_db_iter->owner, uti_db_iter->master, temp_type, uti_db_iter->t_idx);

            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                change_auth_user.current_equipment_inventory += 1;
            });
            action(permission_level{get_self(), "active"_n},
                   NFT_CONTRACT, "deletedata"_n,
                   std::make_tuple(temp, uti_db_iter->idx))
                .send();
        }

        user_mail_table.erase(user_mail_iter);
        
        user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
            if(update_log.mail - _mail_index[i] <=0)
            {
                update_log.mail = 0;
            }
            else
            {
                update_log.mail -= _mail_index[i];                            
            }   
        });
    }
}

// eosio.token recipient
// memo description spec
//-------------------------------------------------------------------------

ACTION battletest::eostransfer(eosio::name sender, eosio::name receiver)
{
    eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
        eosio_assert(ad.action.size() != 0, "Eos Transfer : Wrong Action");
        if (ad.action == action_signup)
        {
            signup(sender, ad.amount);
        }
        if (ad.action == action_gacha)
        {
            start_gacha(sender, ad.type, ad.amount);
        }
        else if (ad.action == action_inventory)
        {
            inventory_buy(sender, ad.type, ad.count);
        }
        else if (ad.action == action_referral)
        {
            refer_signup(sender, ad.from, ad.amount);
        }
    });
}

template <typename T>
void battletest::eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func)
{
    require_auth(sender);
    auto transfer_data = eosio::unpack_action_data<st_transfer>();
    require_auth(transfer_data.from);
    eosio_assert(transfer_data.to == receiver, "Eos Transfer : Reciver Miss Match"); 
    eosio_assert(transfer_data.from == sender, "Eos Transfer : Sender Miss Match");  
    eosio_assert(transfer_data.quantity.symbol == symbol("EOS", 4), "Eos Transfer : Only Accepts EOS");
    eosio_assert(transfer_data.quantity.is_valid(), "Eos Transfer : Invalid Token Transfer");
    eosio_assert(transfer_data.quantity.amount > 0, "Eos Transfer : Quantity Must Be Positive");

    transfer_action res;
    size_t l_center = transfer_data.memo.find(':');

    res.action = transfer_data.memo.substr(0, l_center);
    if (transfer_data.from == _self)        
    {
        bool flag = false;
        if(transfer_data.to == "eosio.ram"_n)
        {
            flag = true;
        }
        if(transfer_data.to == "eosio.ramfee"_n)
        {
            flag = true;
        }
        if(transfer_data.to == "eosio.stake"_n)
        {
            flag = true;
        }

        if (flag == false)
        {
            system_master system_master_table(_self, _self.value);
            auto system_master_iter = system_master_table.find(transfer_data.to.value);
            eosio_assert(system_master_iter != system_master_table.end(), "Eos Transfer : Impossible Send EOS");
        }
    }
    else        
    {
        system_master system_master_table(_self, _self.value);
        auto system_master_iter = system_master_table.find(transfer_data.from.value);
        if (system_master_iter == system_master_table.end())   
        {
            system_check(transfer_data.from);
            if (res.action == "gacha")
            {
                size_t l_next = transfer_data.memo.find(':', l_center + 1);
                size_t l_end = transfer_data.memo.length() - (l_next + 1);

                eosio_assert(transfer_data.memo.find(':') != std::string::npos, "Eos Transfer Gacha : Seed Memo [:] Error");
                eosio_assert(transfer_data.memo.find(':', l_center + 1) != std::string::npos, "Eos Transfer Gacha : Seed Memo [:] Error");
                eosio_assert(transfer_data.quantity.amount == 10000, "Eos Transfer Gacha : Gacha need 1.0000 EOS");        

                std::string l_seed = transfer_data.memo.substr(l_center + 1, (l_next - l_center - 1));
                std::string l_sha = transfer_data.memo.substr(l_next + 1, l_end);

                res.type = safeseed::check_seed(l_seed, l_sha);

                eosio_assert(res.type != 0, "Eos Transfer Gacha : Wrong Seed Convert");
                set_eos_log(transfer_data.quantity.amount);
                res.amount = transfer_data.quantity.amount;
            }
            else if (res.action == "signup")
            {
                eosio_assert(transfer_data.quantity.amount == 10000, "Eos Transfer Signup : Signup Need 1.0000 EOS"); 
                set_eos_log(transfer_data.quantity.amount);
                res.amount = transfer_data.quantity.amount;
            }
            else if (res.action == "refer_signup")
            {
                eosio_assert(transfer_data.quantity.amount == 10000, "Eos Transfer Refer Signup : Signup Need 1.0000 EOS"); 
                size_t l_end = transfer_data.memo.length() - (l_center + 1);
                std::string result = transfer_data.memo.substr(l_center + 1, l_end);

                eosio::name refer_account(result);
                res.from = refer_account;
                set_eos_log(transfer_data.quantity.amount);
                res.amount = transfer_data.quantity.amount;
            }
            else if (res.action == "inventorybuy") 
            {
                size_t l_next = transfer_data.memo.find(':', l_center + 1);
                size_t l_end = transfer_data.memo.length() - (l_next + 1);

                eosio_assert(transfer_data.memo.find(':') != std::string::npos, "Eos Transfer inventorybuy : Seed Memo [:] Error");
                eosio_assert(transfer_data.memo.find(':', l_center + 1) != std::string::npos, "Eos Transfer inventorybuy : Seed Memo [:] Error");


                std::string result_type = transfer_data.memo.substr(l_center + 1, (l_next - l_center - 1));
                std::string result_count = transfer_data.memo.substr(l_next + 1, l_end);
                uint64_t result_seed = atoll(result_type.c_str());
                uint64_t result_amount = atoll(result_count.c_str());

                res.type = result_seed;
                res.count = result_amount;

                eosio_assert(res.count * 1000 == transfer_data.quantity.amount, "Eos Transfer inventorybuy : Not same Eos");
                set_eos_log(transfer_data.quantity.amount);
                eosio_assert(res.type != 0, "Eos Transfer inventorybuy : Wrong Seed Convert");
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
// ACTION battletest::initmaster()
// {
//     require_auth(owner_auth);
//     system_master system_master_table(_self, _self.value);
//     auto system_master_iter = system_master_table.begin();

//     total_token_logs total_token_log_table(_self, _self.value);
//     auto total_token_log_iter = total_token_log_table.find(system_master_iter->master.value);

//     if (total_token_log_iter != total_token_log_table.end())
//     {
//         total_token_log_table.emplace(_self, [&](auto &move_master) {
//             move_master.owner = _self;
//             move_master.total_token_amount = total_token_log_iter->total_token_amount;
//         });
//         total_token_log_table.erase(total_token_log_iter);
//     }

//     system_master_table.erase(system_master_iter);

//     system_master_table.emplace(_self, [&](auto &owner_master) {
//         owner_master.master = _self;
//         owner_master.state = system_state::pause;
//     });
// }

#pragma endregion

#pragma reion init all table


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
    auto status_iter = servant_lv_status_db_table.find(5); 
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

uint32_t battletest::get_monster_passive_skill(uint32_t _seed)
{
    uint32_t passive_id = 0;

    main_gacha_db main_gacha_db_table(_self, _self.value);
    auto p_passive_iter = main_gacha_db_table.find(GACHA_PUBLIC_PASSIVE_COUNT_ID);
    eosio_assert(p_passive_iter != main_gacha_db_table.end(), "Get Monster Passive Skill : Empty Public Passive / Wrong Gacha ID");

    uint32_t random_rate = safeseed::get_random_value(_seed, p_passive_iter->db_index, 0, 1);
    random_rate += 1;
    random_rate += (10000 * p_passive_iter->gacha_id);

    auto passvie = main_gacha_db_table.find(random_rate);
    eosio_assert(passvie != main_gacha_db_table.end(), "Get Monster Passvie Skill : Empty Gacha ID / Wrong Gacha ID");
    passive_id = passvie->db_index;

    passive_db passsive_db_table(_self, _self.value);
    auto passive_iter = passsive_db_table.find(passive_id);
    eosio_assert(passive_iter != passsive_db_table.end(),"Get Monster Passvie Skill : Empty Passvie ID / Wrong Passvie ID");

    return passive_id;
}


uint32_t battletest::get_servant_passive_skill(uint32_t _job, uint32_t _seed)
{
    uint32_t passive_id = 0;

    main_gacha_db main_gacha_db_table(_self, _self.value);
    auto p_passive_iter = main_gacha_db_table.find(GACHA_PUBLIC_PASSIVE_COUNT_ID);
    eosio_assert(p_passive_iter != main_gacha_db_table.end(), "Get Servant Passive Skill : Empty Public Passive / Wrong Gacha ID");

    uint32_t random_rate = safeseed::get_random_value(_seed, p_passive_iter->db_index, 0, 1);
    random_rate += 1;
    random_rate += (10000 * p_passive_iter->gacha_id);

    auto passvie = main_gacha_db_table.find(random_rate);
    eosio_assert(passvie != main_gacha_db_table.end(), "Get Servant Passvie Skill : Empty Gacha ID / Wrong Gacha ID");
    passive_id = passvie->db_index;

    passive_db passsive_db_table(_self, _self.value);
    auto passive_iter = passsive_db_table.find(passive_id);
    eosio_assert(passive_iter != passsive_db_table.end(),"Get Servant Passvie Skill : Empty Passvie ID / Wrong Passvie ID");

    return passive_id;
}

uint32_t battletest::get_servant_active_skill(uint32_t _job, uint32_t _seed)
{
    uint32_t active_id = 0;
    main_gacha_db main_gacha_db_table(_self, _self.value);
    uint32_t gacha_activ_per_id = JOB_ACTIVE_START_ID + JOB_PRIVATE_ACTIVE_PER_ID;
    auto private_per_iter = main_gacha_db_table.find(gacha_activ_per_id);
    eosio_assert(private_per_iter != main_gacha_db_table.end(),"Get Servant Active Skill : Empty Active Per / Wrong Gacha ID");

    uint32_t random_rate = safeseed::get_random_value(_seed, 100, 0 ,0);
    if(random_rate < private_per_iter->db_index)
    {
        uint32_t job_active_gacha_id = JOB_ACTIVE_START_ID + _job;
        auto p_active_iter = main_gacha_db_table.find(job_active_gacha_id);
        eosio_assert(p_active_iter != main_gacha_db_table.end(), "Get Servant Active Skill : Empty Theif Active / Wrong Gacha ID");
        if (p_active_iter->db_index == 0)
        {
            auto p_active_iter = main_gacha_db_table.find(GACHA_PUBLIC_ACTIVE_COUNT_ID);
            eosio_assert(p_active_iter != main_gacha_db_table.end(), "Get Servant Active Skill : Empty Public Active / Wrong Gacha ID");
            uint32_t random_rate = safeseed::get_random_value(_seed, p_active_iter->db_index, 0, 1);
            random_rate += 1;
            random_rate += (10000 * p_active_iter->gacha_id);
            auto active = main_gacha_db_table.find(random_rate);
            eosio_assert(active != main_gacha_db_table.end(), "Get Servant Active Skill : Empty Gacha ID / Wrong Gacha ID");
            active_id = active->db_index;
        }
        else
        {
            uint32_t random_rate = safeseed::get_random_value(_seed, p_active_iter->db_index, 0, 1);
            random_rate += 1;
            random_rate += (10000 * p_active_iter->gacha_id);
            auto active = main_gacha_db_table.find(random_rate);
            eosio_assert(active != main_gacha_db_table.end(), "Get Servant Active Skill : Empty Theif Active Gacha ID / Wrong Gacha ID");
            active_id = active->db_index;
        }
    }
    else
    {
        auto p_active_iter = main_gacha_db_table.find(GACHA_PUBLIC_ACTIVE_COUNT_ID);
        eosio_assert(p_active_iter != main_gacha_db_table.end(),"Get Servant Active Skill : Empty Public Active / Wrong Gacha ID");
        uint32_t random_rate = safeseed::get_random_value(_seed, p_active_iter->db_index, 0 ,1);
        random_rate += 1;
        random_rate += (10000 * p_active_iter->gacha_id);
        auto active = main_gacha_db_table.find(random_rate);
        eosio_assert(active != main_gacha_db_table.end(),"Get Servant Active Skill : Empty Gacha ID / Wrong Gacha ID");
        active_id = active->db_index;
    }
    active_db active_db_table(_self, _self.value);
    auto active_iter = active_db_table.find(active_id);
    eosio_assert(active_iter != active_db_table.end(),"Get Servant Active Skill : Empty Active ID / Wrong Active ID");

    return active_id;
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
   
    serstat_db servant_base_table(_self, _self.value);
    uint32_t servant_job_base = (servant_id_db_iter.job * 1000) + (servant_id_db_iter.grade * 100) + 1;
    const auto &ser_iter = servant_base_table.get(servant_job_base, "Gacha Servant : Empty Servant Stat");

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

        new_servant.status.basic_str = change_servant_statue(new_servant.status.basic_str) + ser_iter.base_str;
        new_servant.status.basic_dex = change_servant_statue(new_servant.status.basic_dex) + ser_iter.base_dex;
        new_servant.status.basic_int = change_servant_statue(new_servant.status.basic_int)+ ser_iter.base_int;

        new_servant.equip_slot.resize(3);
        new_servant.state = object_state::on_inventory;

        uint32_t active_id = get_servant_active_skill(servant_id_db_iter.job, _seed);
        new_servant.active_skill.push_back(active_id);

        uint32_t passive_id = get_servant_passive_skill(servant_id_db_iter.job, _seed);
        new_servant.passive_skill.push_back(passive_id);

        result.index = update_user_servant_list.index;
        result.type = result::servant;

        update_user_servant_list.party_number = EMPTY_PARTY;
        update_user_servant_list.servant = new_servant;

        // new_data += to_string(update_user_servant_list.index) + ":";
        // new_data += to_string(update_user_servant_list.party_number) + ":";
        // new_data += to_string(update_user_servant_list.servant.state) + ":";
        // new_data += to_string(update_user_servant_list.servant.exp) + ":";
        // new_data += to_string(update_user_servant_list.servant.id) + ":";
        // new_data += to_string(update_user_servant_list.servant.level) + ":";
        // new_data += to_string(update_user_servant_list.servant.grade) + ":";
        // new_data += to_string(update_user_servant_list.servant.status.basic_str) + ":";
        // new_data += to_string(update_user_servant_list.servant.status.basic_dex) + ":";
        // new_data += to_string(update_user_servant_list.servant.status.basic_int) + ":";
        // new_data += "[";
        // new_data += to_string(update_user_servant_list.servant.equip_slot[0]) + ":";
        // new_data += to_string(update_user_servant_list.servant.equip_slot[1]) + ":";
        // new_data += to_string(update_user_servant_list.servant.equip_slot[2]);
        // new_data += "]:";
        // new_data += "[";
        // for (uint32_t i = 0; i < update_user_servant_list.servant.passive_skill.size(); ++i)
        // {
        //     if (i + 1 == update_user_servant_list.servant.passive_skill.size())
        //     {
        //         new_data += to_string(update_user_servant_list.servant.passive_skill[i]);
        //     }
        //     else
        //     {
        //         new_data += to_string(update_user_servant_list.servant.passive_skill[i]) + ":";
        //     }
        // }
        // new_data += "]:";
        // new_data += "[";
        // for (uint32_t i = 0; i < update_user_servant_list.servant.active_skill.size(); ++i)
        // {
        //     if (i + 1 == update_user_servant_list.servant.active_skill.size())
        //     {
        //         new_data += to_string(update_user_servant_list.servant.active_skill[i]);
        //     }
        //     else
        //     {
        //         new_data += to_string(update_user_servant_list.servant.active_skill[i]) + ":";
        //     }
        // }
        // new_data += "]";
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
        update_log.servant_num += 1;
        update_log.gacha_num += 1;
    });

    auth_user_table.modify(auth_user_iter, _self, [&](auto &update_auth_user) {
        update_auth_user.current_servant_inventory += 1;
    });

    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "resultgacha"_n,
    //        std::make_tuple(_user, std::string("servant"), new_data))
    //     .send();
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
    auto status_iter = monster_lv_status_db_table.find(_grade); 
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
    auto gacha_db_iter = main_gacha_db_table.find(GACHA_MONSTER_COUNT_ID); //1 ser 2 mon 3 item
    eosio_assert(gacha_db_iter != main_gacha_db_table.end(), "Gacha Monster : Empty Max Count");
    uint32_t random_monster_id = safeseed::get_random_value(_seed, gacha_db_iter->db_index + 1, DEFAULT_MIN_DB, monster_random_count);
    random_monster_id += MONSTER_GACHA_ID_START;

    auto gacha_monster_db_iter = main_gacha_db_table.find(random_monster_id);
    eosio_assert(gacha_monster_db_iter != main_gacha_db_table.end(), "Gacha Monster : Empty Gacha ID");

    monster_db monster_id_db_table(_self, _self.value);
    const auto &monster_id_db_iter = monster_id_db_table.get(gacha_monster_db_iter->db_index, "Gacha Monster : Empty Monster ID");

    tribe_db tribe_db_table(_self, _self.value);
    const auto &tribe_iter = tribe_db_table.get(monster_id_db_iter.tribe, "Gacha Monster : Empty Monster Tribe");

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
        new_monster.tribe = monster_id_db_iter.tribe;
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

        new_monster.status.basic_str = (new_monster.status.basic_str * tribe_iter.base_str) / 100;
        new_monster.status.basic_dex = (new_monster.status.basic_dex * tribe_iter.base_dex) / 100;
        new_monster.status.basic_int = (new_monster.status.basic_int * tribe_iter.base_int) / 100;

        new_monster.state = object_state::on_inventory;

        uint32_t passive_id = get_monster_passive_skill(_seed);
        new_monster.passive_skill.push_back(passive_id);

        result.index = update_user_monster_list.index;
        result.type = result::monster;

        update_user_monster_list.party_number = EMPTY_PARTY;
        update_user_monster_list.monster = new_monster;

        // new_data += to_string(update_user_monster_list.index) + ":";
        // new_data += to_string(update_user_monster_list.party_number) + ":";
        // new_data += to_string(update_user_monster_list.monster.id) + ":";
        // new_data += to_string(update_user_monster_list.monster.state) + ":";
        // new_data += to_string(update_user_monster_list.monster.exp) + ":";
        // new_data += to_string(update_user_monster_list.monster.type) + ":";
        // new_data += to_string(update_user_monster_list.monster.tribe) + ":";
        // new_data += to_string(update_user_monster_list.monster.grade) + ":";
        // new_data += to_string(update_user_monster_list.monster.upgrade) + ":";
        // new_data += to_string(update_user_monster_list.monster.level) + ":";
        // new_data += to_string(update_user_monster_list.monster.status.basic_str) + ":";
        // new_data += to_string(update_user_monster_list.monster.status.basic_dex) + ":";
        // new_data += to_string(update_user_monster_list.monster.status.basic_int) + ":";
        // new_data += "[";
        // for (uint32_t i = 0; i < update_user_monster_list.monster.passive_skill.size(); ++i)
        // {
        //     if (i + 1 == update_user_monster_list.monster.passive_skill.size())
        //     {
        //         new_data += to_string(update_user_monster_list.monster.passive_skill[i]);
        //     }
        //     else
        //     {
        //         new_data += to_string(update_user_monster_list.monster.passive_skill[i]) + ":";
        //     }
        // }
        // new_data += "]:";
        // new_data += "[";
        // for (uint32_t i = 0; i < update_user_monster_list.monster.active_skill.size(); ++i)
        // {
        //     if (i + 1 == update_user_monster_list.monster.active_skill.size())
        //     {
        //         new_data += to_string(update_user_monster_list.monster.active_skill[i]);
        //     }
        //     else
        //     {
        //         new_data += to_string(update_user_monster_list.monster.active_skill[i]) + ":";
        //     }
        // }
        // new_data += "]";
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

    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "resultgacha"_n,
    //        std::make_tuple(_user, std::string("monster"), new_data))
    //     .send();
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

void battletest::set_tier_status(uint32_t &_value, uint32_t _tier)
{

    if(_tier == 1)
    {
        _value = (_value * 29) / 100;
    }
    else if(_tier == 2)
    {
        _value = (_value * 44) / 100;
    }
    else if(_tier == 3)
    {
        _value = (_value * 66) / 100;
    }
    else if(_tier == 4)
    {
        _value = (_value * 100) / 100;
    }

}

void battletest::gacha_equipment_id(eosio::name _user, uint64_t _seed)
{
    main_gacha_db main_gacha_db_table(_self, _self.value);
    auto gacha_db_iter = main_gacha_db_table.find(GACHA_EQUIPMENT_COUNT_ID); //1 ser 2 mon 3 item
    eosio_assert(gacha_db_iter != main_gacha_db_table.end(), "Gacha Equipment : Empty Max Count / Not Set Count");
    uint32_t random_item_id = safeseed::get_random_value(_seed, gacha_db_iter->db_index + 1, DEFAULT_MIN_DB, item_random_count);
    random_item_id += ITEM_GACHA_ID_START;

    auto gacha_id_db_iter = main_gacha_db_table.find(random_item_id);
    eosio_assert(gacha_id_db_iter != main_gacha_db_table.end(), "Gacha Equipment : Empty Gacha ID / Not Set Gacha ID");

    equipment_db equip_item_table(_self, _self.value);
    const auto &equip_item_iter = equip_item_table.get(gacha_id_db_iter->db_index, "Gacha Equipment : Empty Equipment ID / Not Set Equipment ID");

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
        uint32_t type_grade = ((equip_item_iter.type + 1) * 10)  + item_grade_db_iter.grade;
        new_item.value = safeseed::get_random_value(_seed, 10, 0, item_random_count);
        new_item.value = change_equipment_statue(type_grade, new_item.value);
        set_tier_status(new_item.value, equip_item_iter.tier);


        new_item.state = object_state::on_inventory;

        result.index = update_user_item_list.index;
        result.type = result::item;

        update_user_item_list.equipment = new_item;

        // new_data += to_string(update_user_item_list.index) + ":";
        // new_data += to_string(update_user_item_list.equipment.id) + ":";
        // new_data += to_string(update_user_item_list.equipment.state) + ":";
        // new_data += to_string(update_user_item_list.equipment.type) + ":";
        // new_data += to_string(update_user_item_list.equipment.tier) + ":";
        // new_data += to_string(update_user_item_list.equipment.job) + ":";
        // new_data += to_string(update_user_item_list.equipment.grade) + ":";
        // new_data += to_string(update_user_item_list.equipment.upgrade) + ":";
        // new_data += to_string(update_user_item_list.equipment.value) + ":";
        // new_data += to_string(update_user_item_list.equipment.equipservantindex);
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

    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "resultgacha"_n,
    //        std::make_tuple(_user, std::string("equipment"), new_data))
    //     .send();
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

void battletest::start_gacha(eosio::name _user, uint64_t _seed, uint64_t _use_eos)
{
    user_auths user_auth_table(_self, _self.value);
    auto users_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(users_auth_iter != user_auth_table.end(), "Start Gacha : Empty Auth Table / Not Yet Signup");
    eosio_assert(users_auth_iter->state == user_state::lobby,"Start Gacha :  It Is Possible Lobby");

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Start Gacha : Log Table Empty / Not yet signup");
    user_log_table.modify(user_log_iter, _self, [&](auto &new_data)
    {
        new_data.use_eos += _use_eos;
    });

    eosio_assert(check_inventory(_user) == true, "Start Gacha : Inventory Is Full");

    uint64_t l_user = get_user_seed_value(_user.value);
    uint64_t l_seed = safeseed::get_seed_value(l_user, _seed);

    uint64_t l_gacha_result_type = safeseed::get_random_value(l_seed, 1000, DEFAULT_MIN, DEFAULT_RANDOM_COUNT);
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
    eosio_assert(users_auth_iter != user_auth_table.end(), "check_inventory: Not Exist User");

    if (users_auth_iter->current_servant_inventory > users_auth_iter->servant_inventory)
    {
        return false;
    }
    else if (users_auth_iter->current_monster_inventory > users_auth_iter->monster_inventory)
    {
        return false;
    }
    else if (users_auth_iter->current_equipment_inventory > users_auth_iter->equipment_inventory)
    {
        return false;
    }
    else if (users_auth_iter->current_item_inventory > users_auth_iter->item_inventory)
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
    master_active_check();

    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter != blacklist_table.end(), "User Not Black List");

    blacklist_table.erase(blacklist_iter);
}

ACTION battletest::addblack(eosio::name _user)
{
    master_active_check();

    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "Black List User 4");

    blacklist_table.emplace(_self, [&](auto &new_black_user) {
        new_black_user.user = _user;
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

void battletest::master_active_check()
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);
}

void battletest::system_check(eosio::name _user)
{
    require_auth(_user);
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter->state != system_state::pause, "System Check : Server Pause");

    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "System Check : BlackList User");

    // whitelist whitelist_table(_self, _self.value);
    // auto whitelist_iter = whitelist_table.find(_user.value);
    // eosio_assert(whitelist_iter != whitelist_table.end(), "System Check : Server Pause");
}

#pragma endregion

// ACTION battletest::resultgacha(eosio::name _who, std::string _type, std::string _result)
// {
//     require_auth(_self);
//     require_recipient(_self);
// }

// ACTION battletest::resultparty(eosio::name _who, std::string _party_info, std::string _servant_list, std::string _monster_list)
// {
//     require_auth(_self);
//     require_recipient(_self);
// }

// ACTION battletest::battlestate(eosio::name _who, std::string _stage_info, std::vector<std::string> &_my_state_list, std::vector<std::string> &_enemy_state_list)
// {
//     require_auth(_self);
//     require_recipient(_self);
// }

// ACTION battletest::battleaction(eosio::name _who, std::string _turn, std::vector<std::string> &_data)
// {
//     require_auth(_self);
//     require_recipient(_self);
// }

// ACTION battletest::battleresult(eosio::name _who, std::vector<std::string> &_reward)
// {
//     require_auth(_self);
//     require_recipient(_self);
// }
#pragma endresion

#pragma region party function

//------------------------------------------------------------------------//
//-------------------------------party_function---------------------------//
//------------------------------------------------------------------------//

bool battletest::check_same_party(eosio::name _user, uint32_t _party_number, const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list)
{
    uint32_t servant_same_count = 0;
    uint32_t monster_same_count = 0;

    user_partys user_party_table(_self, _user.value);            
    auto user_party_iter = user_party_table.find(_party_number); 
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
    system_check(_user);
    user_auths user_auth_table(_self, _self.value);
    auto users_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(users_auth_iter != user_auth_table.end(), "Save Party : Empty Auth Table / Not Yet Signup");
    eosio_assert(users_auth_iter->state == user_state::lobby, "Save Party :  It Is Possible Lobby");

    eosio_assert(_party_number == 1, "Save Party : Wrong Party Number / Party Number Only 1"); 
    eosio_assert(false == check_empty_party(_servant_list, _monster_list),"Save Party : Empty Party List");                                           
    user_partys user_party_table(_self, _user.value);                                                        
    auto user_party_iter = user_party_table.find(_party_number);                                            
    eosio_assert(user_party_iter != user_party_table.end(), "Save Party : Empty Auth Table / Not Yet Signup");                              
    eosio_assert(user_party_iter->state != party_state::on_tower_defense, "Save Party : Impossible Save Party / On Tower");
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
                set_party.servant.state = object_state::on_inventory;
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
                set_party.monster.state = object_state::on_inventory;
            });
        }
    }

    user_party_table.modify(user_party_iter, _self, [&](auto &save_party) {
        servant_list += "[";
        for (uint32_t i = 0; i < 5; ++i) 
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
                set_party.servant.state = object_state::on_party;
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
            eosio_assert(save_party.servant_list[i] != 0,"Save Party : Empty Servant / Need Set Servant");
            auto user_monster_iter = user_monster_table.find(_monster_list[i]);
            eosio_assert(user_monster_iter != user_monster_table.end(), "Save Party : Empty Monster Table / Wrong Monster Index");
            eosio_assert(user_monster_iter->party_number == EMPTY_PARTY, "Save Party : Already Set Monster / Monster Set Other Party");
            eosio_assert(user_monster_iter->monster.state == object_state::on_inventory, "Save Party : Monster State Not On Inventory / Monster Not On Inventory");
            user_monster_table.modify(user_monster_iter, owner, [&](auto &set_party) {
                set_party.party_number = _party_number;
                set_party.monster.state = object_state::on_party;
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

    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "resultparty"_n,
    //        std::make_tuple(_user, party_info, servant_list, monster_list))
    //     .send();
}

// void battletest::add_party_list(eosio::name _user)
// {
//    user_logs user_log_table(_self, _self.value);

//     auto user_log_iter = user_log_table.find(_user.value);
//     eosio_assert(user_log_iter != user_log_table.end(), "not find user information to log");
//     uint32_t l_p_count = user_log_iter->add_party_count;
//     l_p_count++;
//     user_log_table.modify(user_log_iter, _self, [&](auto &buy_party_log) {
//         buy_party_log.add_party_count = l_p_count;
//     });
//     user_partys user_party_table(_self, _user.value);
//     user_party_table.emplace(_self, [&](auto &new_party) {
//         new_party.index = user_party_table.available_primary_key();
//         new_party.servant_list.resize(4);
//         new_party.monster_list.resize(5);
//     });
// }




#pragma region item equipment function
//------------------------------------------------------------------------//
//-------------------------item_equipment_function------------------------//
//------------------------------------------------------------------------//

#pragma endregion

#pragma region battle function
//------------------------------------------------------------------------//
//---------------------------------battle_function------------------------//
//------------------------------------------------------------------------//
uint32_t battletest::get_max_hp(status_info _status, uint32_t _level)
{
    uint32_t hp;
    hp = _status.basic_str * 66;    
    hp = hp * ((_level * decimal) / 50) + (_status.basic_str * 22 * decimal) ;
    return hp / decimal ;
}
uint32_t battletest::get_physical_attack(status_info _status, uint32_t _level)
{
    uint32_t physical_attack;
    physical_attack = (_status.basic_str + _status.basic_dex) * 8;     
    physical_attack = physical_attack * (((_level * decimal) / 50)) + ((_status.basic_str + _status.basic_dex) * 2 * decimal);
    return physical_attack / decimal;
}
uint32_t battletest::get_magic_attack(status_info _status, uint32_t _level)
{
    uint32_t magic_attack;
    magic_attack = (_status.basic_int * 8);                  
    magic_attack = magic_attack * (((_level * decimal) / 50)) + (_status.basic_int * 2 * decimal);
    return magic_attack / decimal;
}

uint32_t battletest::get_physical_defense(status_info _status, uint32_t _level)
{
    uint32_t physical_defense;
    physical_defense = (_status.basic_str + _status.basic_dex) * 4;
    physical_defense = physical_defense * (((_level * decimal) / 50)) + ((_status.basic_str + _status.basic_dex) * 1 * decimal);
    return physical_defense / decimal;
}

uint32_t battletest::get_magic_defense(status_info _status, uint32_t _level)
{
    uint32_t magic_defense;
    magic_defense = _status.basic_int * 4;
    magic_defense = magic_defense * (((_level * decimal) / 50)) + (_status.basic_int * 1 * decimal);
    return magic_defense / decimal;
}


uint32_t battletest::get_cri_per(status_info _status, uint32_t _level)
{
    uint32_t cri_per;
    cri_per = ((_status.basic_int * 1000) / 500) / 1000;
    cri_per += 5;  
    return cri_per;
}

uint32_t battletest::get_cri_dmg_per(status_info _status, uint32_t _level)
{
    uint32_t cri_dmg_per;
    cri_dmg_per = (_status.basic_int * 5) / 10; 
    cri_dmg_per += 120; 
    return cri_dmg_per;
}

void battletest::set_upgrade_equip_status(uint64_t _grade, uint32_t _value, uint32_t _upgrade)
{
    if (_upgrade != 0)
    {
        upgrade_equipment_db table(_self, _self.value);
        auto iter = table.find(_upgrade);
        eosio_assert(iter != table.end(), "Set Upgrade Equipment : Empty Upgrade ID / Wrong Upgrade ID");

        uint32_t index = 5 - _grade;

        _value += (_value * iter->grade_list[index]) / 100;
    }
}


void battletest::set_upgrade_monster_status(uint64_t _grade, status_info &_status, uint32_t _upgrade)
{
    upgrade_monster_db table(_self, _self.value);
    auto iter = table.find(_upgrade);
    if(_upgrade == 0)
    {
        return;
    }
    else{
    eosio_assert(iter != table.end(),"Set Upgrade Monster : Empty Upgrade ID / Wrong Upgrade ID");
    }
    
    uint32_t index = 5 - _grade;

    _status.basic_str += (_status.basic_str * iter->grade_list[index]) / 100;
    _status.basic_dex += (_status.basic_dex * iter->grade_list[index]) / 100;
    _status.basic_int += (_status.basic_int * iter->grade_list[index]) / 100;
}
 

uint32_t battletest::get_damage(uint32_t _atk, uint32_t _dfs)
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

        status_info status;
        status = stage_status;    

        uint64_t level = safeseed::get_random_value(now(), stage_db_iter->enemy_level_max + 1, stage_db_iter->enemy_level_min, 0);

        get_state.level = level;
        get_state.grade = stage_enemy_db_iter->grade;
        get_state.id = stage_db_iter->monster_list[i].monster_id;
        get_state.index = 0;
        get_state.position = stage_db_iter->monster_list[i].position;

        active_db active_db_table(_self, _self.value);
        for (uint32_t i = 0; i < stage_enemy_db_iter->active_list.size(); ++i)
        {
            auto active_db_iter = active_db_table.find(stage_enemy_db_iter->active_list[i]);
            eosio_assert(active_db_iter != active_db_table.end(), "Set Enemy State : Empty Active ID / Wrong Active ID");

            get_state.active_skill_list.push_back(active_db_iter->active_id);
        }

        uint32_t increase_hp = 0;
        uint32_t increase_hp_per = 0;
        passive_db passive_db_table(_self, _self.value);
        for (uint32_t i = 0; i < stage_enemy_db_iter->passive_list.size(); ++i)
        {
            auto passive_db_iter = passive_db_table.find(stage_enemy_db_iter->passive_list[i]);
            eosio_assert(passive_db_iter != passive_db_table.end(), "Set Enemy State : Empty Passive ID / Wrong Passive ID");
            if (passive_db_iter->effect_id == passive_effect_id::b_str)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    status.basic_str += (status.basic_str * passive_db_iter->effect_value_a) / 100;
                }
            }
            else if (passive_db_iter->effect_id == passive_effect_id::b_dex)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    status.basic_dex += (status.basic_dex * passive_db_iter->effect_value_a) / 100;
                }
            }
            else if (passive_db_iter->effect_id == passive_effect_id::b_int)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    status.basic_int += (status.basic_int * passive_db_iter->effect_value_a) / 100;
                }
            }
            else if (passive_db_iter->effect_id == passive_effect_id::hp)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    increase_hp_per += passive_db_iter->effect_value_a;
                }
            }
            get_state.passive_skill_list.push_back(passive_db_iter->passive_id);
        }

        get_state.now_hp = get_max_hp(status, level) + increase_hp;
        get_state.now_hp += (get_state.now_hp * increase_hp_per) / 100;
        get_state.max_hp = get_state.now_hp;
        get_state.state = battle_member_state::live;
        get_state.status = stage_status;
        get_state.type = character_type::t_monster; 
        get_state.upgrade = 0;

        _enemy_state_list.push_back(get_state);

        // std::string state;
        // state += to_string(get_state.grade) + ":";
        // state += to_string(get_state.position) + ":";
        // state += to_string(get_state.index) + ":";
        // state += to_string(get_state.id) + ":";
        // state += to_string(get_state.now_hp) + ":";
        // state += to_string(get_state.state) + ":";
        // state += to_string(get_state.type) + ":";
        // state += to_string(get_state.upgrade) + ":";
        // state += to_string(get_state.max_hp) + ":";
        // state += "[";
        // for (uint32_t i = 0; i < get_state.passive_skill_list.size(); ++i)
        // {
        //     if (get_state.passive_skill_list.size() - 1 == i)
        //     {
        //         state += to_string(get_state.passive_skill_list[i]);
        //     }
        //     else
        //     {
        //         state += to_string(get_state.passive_skill_list[i]) + ":";
        //     }
        // }
        // state += "]";
        // state += ":";

        // state += "[";
        // for (uint32_t i = 0; i < get_state.active_skill_list.size(); ++i)
        // {
        //     if (get_state.active_skill_list.size() - 1 == i)
        //     {
        //         state += to_string(get_state.active_skill_list[i]);
        //     }
        //     else
        //     {
        //         state += to_string(get_state.active_skill_list[i]) + ":";
        //     }
        // }
        // state += "]";
        // state += ":";

        // state += to_string(get_state.status.basic_str) + ":";
        // state += to_string(get_state.status.basic_dex) + ":";
        // state += to_string(get_state.status.basic_int);

        // _state.push_back(state);
    }
}

battletest::character_state_data battletest::get_user_state(eosio::name _user, std::string _type, uint64_t _index, uint32_t _position, std::vector<std::string> &_state)
{
    character_state_data get_state;
    status_info status;
    status_info basic_status;
    uint32_t increase_hp = 0;
    uint32_t increase_hp_per = 0;

    if (_type == "ser")
    {
        user_servants user_servant_table(_self, _user.value);
        auto user_servant_iter = user_servant_table.find(_index);
        eosio_assert(user_servant_iter != user_servant_table.end(), "Set User State : Empty Servant Index / Wrong Servant Index");

        basic_status = user_servant_iter->servant.status;   
        status = user_servant_iter->servant.status;        
     
        servant_db servant_db_table(_self, _self.value);
        auto servant_db_iter = servant_db_table.find(user_servant_iter->servant.id);
        eosio_assert(servant_db_iter != servant_db_table.end(), "Set User State : Empty Servant ID / Wrong Servant ID");

        user_equip_items user_equipment_table(_self, _user.value);
        equipment_db equipment_db_table(_self, _self.value);
        for (uint32_t i = 0; i < user_servant_iter->servant.equip_slot.size(); ++i)       
        {
            if (user_servant_iter->servant.equip_slot[i] == 0)
            {
                continue;
            }
            auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[i]);
            eosio_assert(user_equipment_iter != user_equipment_table.end(), "Set User State : Empty Equipment Index / Wrong Equipment Index");

            auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
            eosio_assert(db_equipment_iter != equipment_db_table.end(), "Set User State : Empty Equipment ID / Wrong Equipment ID");

            uint32_t value = user_equipment_iter->equipment.value;
            set_upgrade_equip_status(user_equipment_iter->equipment.grade, value, user_equipment_iter->equipment.upgrade);

            for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
            {
                if (db_equipment_iter->option_list[a] == option_list::status_str)   
                {
                    status.basic_str += value;
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_dex) 
                {
                    status.basic_dex += value;
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_int)  
                {
                    status.basic_int += value;
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_hp)   
                {
                    increase_hp += value;
                }
            }
        }

      
        for (uint32_t i = 0; i < user_servant_iter->servant.passive_skill.size(); ++i)
        {
            passive_db passive_db_table(_self, _self.value);
            auto passive_db_iter = passive_db_table.find(user_servant_iter->servant.passive_skill[i]);
            eosio_assert(passive_db_iter != passive_db_table.end(), "Set User State : Empty Servant Passive ID / Wrong Servant Passive ID");
            if (passive_db_iter->effect_id == passive_effect_id::b_str)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    status.basic_str += (status.basic_str * passive_db_iter->effect_value_a) / 100;
                }
            }
            else if (passive_db_iter->effect_id == passive_effect_id::b_dex)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    status.basic_dex += (status.basic_dex * passive_db_iter->effect_value_a) / 100;
                }
            }
            else if (passive_db_iter->effect_id == passive_effect_id::b_int)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    status.basic_int += (status.basic_int * passive_db_iter->effect_value_a) / 100;
                }
            }
            else if (passive_db_iter->effect_id == passive_effect_id::hp)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    increase_hp_per += passive_db_iter->effect_value_a;
                }
            }
            get_state.passive_skill_list.push_back(passive_db_iter->passive_id);
        }

     
        for (uint32_t i = 0; i < user_servant_iter->servant.active_skill.size(); ++i)
        {
            active_db active_db_table(_self, _self.value);
            auto active_db_iter = active_db_table.find(user_servant_iter->servant.active_skill[i]);
            eosio_assert(active_db_iter != active_db_table.end(), "Set User State : Empty Servant Active ID / Wrong Servant Active ID");

            get_state.active_skill_list.push_back(active_db_iter->active_id);
        }

        get_state.level = user_servant_iter->servant.level;
        get_state.grade = 5;
        get_state.index = _index;
        get_state.id = user_servant_iter->servant.id;
        get_state.position = _position;
        get_state.now_hp = get_max_hp(status, user_servant_iter->servant.level) + increase_hp;
        get_state.now_hp += (get_state.now_hp * increase_hp_per) / 100;
        get_state.max_hp = get_state.now_hp;
        get_state.type = character_type::t_servant; 
        get_state.upgrade = 0;
    }
    else if (_type == "mon")
    {
        user_monsters user_monster_table(_self, _user.value);
        auto user_monster_iter = user_monster_table.find(_index);
        eosio_assert(user_monster_iter != user_monster_table.end(), "Set User State : Empty Monster Index / Wrong Monster Index");

       
        monster_db monster_db_table(_self, _self.value);
        auto monster_db_iter = monster_db_table.find(user_monster_iter->monster.id);
        eosio_assert(monster_db_iter != monster_db_table.end(), "Set User State : Empty Monster ID / Wrong Monster ID");

        basic_status = user_monster_iter->monster.status; 
        status = user_monster_iter->monster.status;        
        
        set_upgrade_monster_status(user_monster_iter->monster.grade, status, user_monster_iter->monster.upgrade); 
        
        for (uint32_t i = 0; i < user_monster_iter->monster.passive_skill.size(); ++i)
        {
            passive_db passive_db_table(_self, _self.value);
            auto passive_db_iter = passive_db_table.find(user_monster_iter->monster.passive_skill[i]);
            eosio_assert(passive_db_iter != passive_db_table.end(), "Set User State : Empty Monster Passive ID / Wrong Monster Passive ID");
            if (passive_db_iter->effect_id == passive_effect_id::b_str)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    status.basic_str += (status.basic_str * passive_db_iter->effect_value_a) / 100;
                }
            }
            else if (passive_db_iter->effect_id == passive_effect_id::b_dex)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    status.basic_dex += (status.basic_dex * passive_db_iter->effect_value_a) / 100;
                }
            }
            else if (passive_db_iter->effect_id == passive_effect_id::b_int)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    status.basic_int += (status.basic_int * passive_db_iter->effect_value_a) / 100;
                }
            }
            else if (passive_db_iter->effect_id == passive_effect_id::hp)
            {
                if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                {
                    increase_hp_per += passive_db_iter->effect_value_a;
                }
            }
            get_state.passive_skill_list.push_back(passive_db_iter->passive_id);
        }

       
        active_db active_db_table(_self, _self.value);
        for (uint32_t i = 0; i < user_monster_iter->monster.active_skill.size(); ++i)
        {
            auto active_db_iter = active_db_table.find(user_monster_iter->monster.active_skill[i]);
            eosio_assert(active_db_iter != active_db_table.end(), "Set User State : Empty Monster Active ID / Wrong Monster Active ID");

            get_state.active_skill_list.push_back(active_db_iter->active_id);
        }


        get_state.level = user_monster_iter->monster.level;
        get_state.grade = user_monster_iter->monster.grade;
        get_state.index = _index;
        get_state.id = user_monster_iter->monster.id;
        get_state.position = _position;
        get_state.now_hp = get_max_hp(status, user_monster_iter->monster.level) + increase_hp;
        get_state.now_hp += (get_state.now_hp * increase_hp_per) / 100;
        get_state.max_hp = get_state.now_hp;
        get_state.type = character_type::t_monster; 
        get_state.upgrade = user_monster_iter->monster.upgrade;
    }

    get_state.state = battle_member_state::live;
    get_state.status = basic_status;

    // std::string state;
    // state += to_string(get_state.grade) + ":";
    // state += to_string(get_state.position) + ":";
    // state += to_string(get_state.index) + ":";
    // state += to_string(get_state.id) + ":";
    // state += to_string(get_state.now_hp) + ":";
    // state += to_string(get_state.state) + ":";
    // state += to_string(get_state.type) + ":";
    // state += to_string(get_state.upgrade) + ":";
    // state += to_string(get_state.max_hp) + ":";

    // state += "[";
    // for (uint32_t i = 0; i < get_state.passive_skill_list.size(); ++i)
    // {
    //     if (get_state.passive_skill_list.size() - 1 == i)
    //     {
    //         state += to_string(get_state.passive_skill_list[i]);
    //     }
    //     else
    //     {
    //         state += to_string(get_state.passive_skill_list[i]) + ":";
    //     }
    // }
    // state += "]";
    // state += ":";

    // state += "[";
    // for (uint32_t i = 0; i < get_state.active_skill_list.size(); ++i)
    // {
    //     if (get_state.active_skill_list.size() - 1 == i)
    //     {
    //         state += to_string(get_state.active_skill_list[i]);
    //     }
    //     else
    //     {
    //         state += to_string(get_state.active_skill_list[i]) + ":";
    //     }
    // }
    // state += "]";
    // state += ":";

    // state += to_string(get_state.status.basic_str) + ":";
    // state += to_string(get_state.status.basic_dex) + ":";
    // state += to_string(get_state.status.basic_int);

    // _state.push_back(state);
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
   if (_avoid_per <= rand_avoid_percent)
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

uint32_t battletest::get_tribe_count(std::vector<character_state_data> &_my_state_list, uint32_t _tribe)
{
    uint32_t count = 0;
    for (uint32_t i = 0; i < _my_state_list.size(); ++i)
    {
        if (_my_state_list[i].type == character_type::t_monster)
        {
            monster_db monster_db_table(_self, _self.value);
            auto monster_iter = monster_db_table.find(_my_state_list[i].id);
            eosio_assert(monster_iter != monster_db_table.end(), "Get Tribe Count : Empty Monster ID / Wrong Monster ID");

            if(monster_iter->tribe == _tribe)
            {
                count+=1;
            }
        }
    }
    return count;
}

void battletest::set_synergy(
    std::vector<character_state_data> &_my_state_list, std::vector<uint32_t> &_synergy_list)
{
    for (uint32_t i = 0; i < _my_state_list.size(); ++i)
    {
        if(_my_state_list[i].type == character_type::t_monster)
        {
            monster_db monster_db_table(_self, _self.value);
            auto monster_iter = monster_db_table.find(_my_state_list[i].id);
            eosio_assert(monster_iter != monster_db_table.end(),"Synergy Set : Empty Monster ID / Wrong Monster ID");

            tribe_db tribe_db_table(_self, _self.value);
            auto tribe_iter = tribe_db_table.find(monster_iter->tribe);
            eosio_assert(tribe_iter != tribe_db_table.end(),"Synergy Set : Empty Monser Tribe / Wrong Tribe");

            uint32_t tribe_count = get_tribe_count(_my_state_list, tribe_iter->id);
            uint32_t synergy_id = 0;
            if (tribe_count == 2)
            {
                synergy_id = tribe_iter->pair;

                int flag = false;
                for (uint32_t a = 0; a < _synergy_list.size(); ++a)
                {
                    if (_synergy_list[a] == synergy_id)
                    {
                        flag = true;
                        break;
                    }
                }

                if ((flag == false) && (synergy_id != 0)) 
                {
                    _synergy_list.push_back(synergy_id);
                }
            }
            else if (tribe_count >= 3 && tribe_count < 5)
            {
                synergy_id = tribe_iter->triple;

                int flag = false;
                for (uint32_t a = 0; a < _synergy_list.size(); ++a)
                {
                    if (_synergy_list[a] == synergy_id) 
                    {
                        flag = true;
                        break;
                    }
                }

                if ((flag == false) && (synergy_id != 0))
                {
                    _synergy_list.push_back(synergy_id);
                }
            }
            else if (tribe_count >= 5)
            {
                synergy_id = tribe_iter->penta;

                int flag = false;
                for (uint32_t a = 0; a < _synergy_list.size(); ++a)
                {
                    if (_synergy_list[a] == synergy_id) 
                    {
                        flag = true;
                        break;
                    }
                }

                if ((flag == false) && (synergy_id != 0)) 
                {
                    _synergy_list.push_back(synergy_id);
                }
            }
        }
    }
}

void battletest::set_hp_synergy(
    std::vector<character_state_data> &_my_state_list, std::vector<uint32_t> &_synergy_list)
{
    for(uint32_t i = 0; i < _synergy_list.size(); ++i)
    {
        passive_db passive_db_table(_self, _self.value);
        auto passive_iter = passive_db_table.find(_synergy_list[i]);
        eosio_assert(passive_iter != passive_db_table.end(), "Hp Synergy Set : Empty Passive ID / Wrong Synergy ID");

        if (passive_iter->effect_id == passive_effect_id::hp)       
        {
            if (passive_iter->effect_type_id == passive_effect_type_id::per_up) 
            {
                if (passive_iter->target_id == passvie_target_id::t_spirit)    
                {
                    for(uint32_t my = 0; my < _my_state_list.size(); ++my)
                    {
                        if(_my_state_list[my].type == character_type::t_monster)
                        {
                            monster_db monster_db_table(_self, _self.value);
                            auto monster_iter = monster_db_table.find(_my_state_list[my].id);
                            eosio_assert(monster_iter != monster_db_table.end(),"Hp Synergy Set : Empty Monster ID / Wrong Monster ID");
                            if(passvie_target_id::t_spirit == (monster_iter->tribe + 900))
                            {
                                _my_state_list[my].now_hp += (_my_state_list[my].now_hp * passive_iter->effect_value_a) / 100;
                                _my_state_list[my].max_hp += (_my_state_list[my].max_hp * passive_iter->effect_value_a) / 100;
                            }
                        }
                    }
                }
            }
        }
    }
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

    if(servant_count == 5 && monster_count == 5)   
    {
        return false;
    }

    return true;
}

ACTION battletest::stagestart(eosio::name _user, uint32_t _party_number, uint32_t _stage_type, uint32_t _stage_floor)
{
     system_check(_user);

    eosio_assert(check_inventory(_user) == true, "stagestart : Inventory Is Full");

    std::vector<uint32_t> servant_pos_list = {0, 1, 2, 3, 4};
    std::vector<uint32_t> monster_pos_list = {5, 6, 7, 8, 9};

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
                if (user_party_iter->servant_list[i] == 0) 
                {
                    continue;
                }
                character_state_data servant_battle_state = get_user_state(_user, "ser", user_party_iter->servant_list[i], servant_pos_list[i], my_state);
                new_battle_set.my_state_list.push_back(servant_battle_state);
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter->monster_list[i] == 0) 
                {
                    continue;
                }
                character_state_data monster_battle_state = get_user_state(_user, "mon", user_party_iter->monster_list[i], monster_pos_list[i], my_state);
                new_battle_set.my_state_list.push_back(monster_battle_state);
            }
            set_stage_state(stage_db_iter->stage_id, new_battle_set.enemy_state_list, enemy_state);
            set_synergy(new_battle_set.my_state_list, new_battle_set.my_synergy_list);
            set_hp_synergy(new_battle_set.my_state_list, new_battle_set.my_synergy_list);

            set_synergy(new_battle_set.enemy_state_list, new_battle_set.enemy_synergy_list);
            set_hp_synergy(new_battle_set.enemy_state_list, new_battle_set.enemy_synergy_list);
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
            new_battle_set.my_synergy_list.clear();
            new_battle_set.enemy_synergy_list.clear();

            auto n = name{new_battle_set.user};
            auto e = name{new_battle_set.enemy_user};
            stage_info += n.to_string() + ":";
            stage_info += to_string(new_battle_set.stage_type) + ":";
            stage_info += e.to_string() + ":";
            stage_info += to_string(new_battle_set.stage_number) + ":";
            stage_info += to_string(new_battle_set.turn);

            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter->servant_list[i] == 0)
                {
                    continue;
                }
                character_state_data servant_battle_state = get_user_state(_user, "ser", user_party_iter->servant_list[i], servant_pos_list[i], my_state);
                new_battle_set.my_state_list.push_back(servant_battle_state);
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (user_party_iter->monster_list[i] == 0)
                {
                    continue;
                }
                character_state_data monster_battle_state = get_user_state(_user, "mon", user_party_iter->monster_list[i], monster_pos_list[i], my_state);
                new_battle_set.my_state_list.push_back(monster_battle_state);
            }
            set_stage_state(stage_db_iter->stage_id, new_battle_set.enemy_state_list, enemy_state);

            set_synergy(new_battle_set.my_state_list, new_battle_set.my_synergy_list);
            set_hp_synergy(new_battle_set.my_state_list, new_battle_set.my_synergy_list);

            set_synergy(new_battle_set.enemy_state_list, new_battle_set.enemy_synergy_list);
            set_hp_synergy(new_battle_set.enemy_state_list, new_battle_set.enemy_synergy_list);
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

    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "battlestate"_n,
    //        std::make_tuple(_user, stage_info, my_state, enemy_state))
    //     .send();
}

#pragma endregion

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#pragma region active turn

uint32_t battletest::check_under_minus_flow(uint32_t _a, uint32_t _b)
{
    int result = _a - _b;
    if(result <= 0)
    {
        return 0;
    }
    return uint32_t(result);
}

uint32_t battletest::check_over_plus_flow(uint32_t _a, uint32_t _b)
{
    uint32_t result = _a + _b;
    if(result < _a || result < _b)
    {
        return 0;
    }
    return result;
}

uint32_t battletest::check_over_mult_flow(uint32_t _a, uint32_t _b)
{
    uint32_t result = _a * _b;
    if ( (_a != 0) && ((result / _a) != _b) )
    {
        return 0;
    }
    return result;
}
uint32_t battletest::check_under_divide_flow(uint32_t _a, uint32_t _b)
{
    uint32_t result = _a / _b;
    if(_b == 0)
    {
        return 0;
    }
    return result;
}



int battletest::get_heal_target(const std::vector<battle_status_info> &_enemy_state_list)
{
   int target_key = -1;
   std::vector<heal_compare> list;
   list.clear();

   for (uint32_t i = 0; i < _enemy_state_list.size(); ++i)
   {
       if(_enemy_state_list[i].now_hp == 0)
       {
           continue;
       }
       heal_compare heal;
       heal.max_hp = _enemy_state_list[i].max_hp;
       heal.now_hp = _enemy_state_list[i].now_hp;
       heal.key = i;
       list.push_back(heal);
   }

   std::sort(list.begin(), list.end(), new_sort_heal_compare);
   for (uint32_t i = 0; i < list.size(); ++i)
   {
       if (list[i].max_hp > list[i].now_hp)
       {
           if (list[i].now_hp != 0)
           {
               target_key = list[i].key;
               return target_key;
           }
       }
   }
   return target_key;
}


int battletest::get_random_target(const std::vector<battle_status_info> &_enemy_state_list, uint64_t _seed, uint32_t _max, uint32_t _min)
{
    int target_key = safeseed::get_random_value(_seed, _max, _min, 0);
    if ((_enemy_state_list[target_key].state == battle_member_state::dead))
    {
        target_key = -1;
        for (uint32_t i = _min; i < _max; ++i)
        {
            if ((_enemy_state_list[i].state != battle_member_state::dead))
            {
                return i;
            }
        }
        return target_key;
    }
    else
    {
        return target_key;
    }
}


bool battletest::new_sort_compare(const battle_status_info &a, const battle_status_info &b)
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

bool battletest::new_sort_heal_compare(const heal_compare &a, const heal_compare &b)
{
   uint32_t a_per = (a.now_hp * 100) / a.max_hp;
   uint32_t b_per = (b.now_hp * 100) / b.max_hp;
   if (a_per < b_per)
   {
       return true;
   }
   else
   {
       return false;
   }
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


void battletest::init_buff_turn_self(battle_status_info &_status)
{
    for (uint32_t buff = 0; buff < _status.buff_list.size(); ++buff)
    {
        if (_status.buff_list[buff].turn - 1 <= 0)
        {
            _status.buff_list.erase(_status.buff_list.begin() + buff);
        }
        else
        {
            _status.buff_list[buff].turn -= 1;
            if(_status.buff_list[buff].id == active_option::option_defense_decrease)
            {
                _status.p_dfs -= (_status.p_dfs * 30) / 100;
            }
        }
    }
}

void battletest::set_random_damage(action_info &_action, uint64_t _seed)
{
    uint32_t seed = _seed >> 1;
    uint64_t rate = safeseed::get_random_value(seed, 111, 90, 0);
    _action.damage = (_action.damage * rate) / 100;
}

void battletest::result_buff(action_info &_action, battle_status_info &_status)
{
    for(uint32_t i = 0; i < _status.buff_list.size(); ++i)
    {
        if(_status.buff_list[i].id == active_option::option_defense)
        {
            _action.damage -= (_action.damage * 50) / 100; 
        }
    }
}

void battletest::result_type_skill(eosio::name _user ,action_info &_action, std::vector<battle_status_info> &_my_status_list,
                                    std::vector<battle_status_info> &_enemy_status_list,
                                    uint64_t _my_key, uint32_t _enemy_key)
{
    if (_enemy_status_list[_enemy_key].type == character_type::t_monster)
    {
        monster_db monster_db_table(_self, _self.value);
        auto defender_monster_iter = monster_db_table.find(_enemy_status_list[_enemy_key].id);
        eosio_assert(defender_monster_iter != monster_db_table.end(), "Check Skill Type : Empty Monster ID / Wrong Monster ID");

        active_db active_db_table(_self, _self.value);
        auto active_iter = active_db_table.find(_my_status_list[_my_key].active_skill_list[0]);
        eosio_assert(active_iter != active_db_table.end(),"Check Skill Type : Empty Active ID/ Wrong Actvie ID");

        if (active_iter->elemental_type == 0)
        {
            return;
        }
        else
        {
            type_db type_db_table(_self, _self.value);
            auto attack_iter = type_db_table.find(active_iter->elemental_type);
            eosio_assert(attack_iter != type_db_table.end(), "Check Skill Type : Empty Type ID / Wrong Type ID");

            if (attack_iter->strong == defender_monster_iter->type)
            {
                _action.damage += uint32_t((_action.damage * attack_iter->strong_per) / 100);
            }
            else if (attack_iter->weak == defender_monster_iter->type)
            {
                _action.damage -= uint32_t((_action.damage * attack_iter->weak_per) / 100);
            }
        }
    }
}

void battletest::result_type_damage(eosio::name _user ,action_info &_action, std::vector<battle_status_info> &_my_status_list,
                                    std::vector<battle_status_info> &_enemy_status_list,
                                    uint64_t _my_key, uint32_t _enemy_key)
{
    if ((_my_status_list[_my_key].type == character_type::t_monster) && 
        (_enemy_status_list[_enemy_key].type == character_type::t_monster))
    {
        monster_db monster_db_table(_self, _self.value);
        auto attack_monster_iter = monster_db_table.find(_my_status_list[_my_key].id);
        eosio_assert(attack_monster_iter != monster_db_table.end(), "Check Monster Type Attack : Empty Monster ID / Wrong Monster ID");

        auto defender_monster_iter = monster_db_table.find(_enemy_status_list[_enemy_key].id);
        eosio_assert(defender_monster_iter != monster_db_table.end(), "Check Monster Type Defender : Empty Monster ID / Wrong Monster ID");

        type_db type_db_table(_self, _self.value);
        auto type_iter = type_db_table.find(attack_monster_iter->type);
        eosio_assert(type_iter != type_db_table.end(),"Check Monster Type : Empty Type ID / Wrong Type ID");

        if(type_iter->strong == defender_monster_iter->type)
        {
            _action.damage += uint32_t((_action.damage * type_iter->strong_per) / 100);
        }
        else if(type_iter->weak == defender_monster_iter->type)
        {
            _action.damage -= uint32_t((_action.damage * type_iter->weak_per) / 100);
        }
    }
}

bool battletest::set_action(eosio::name _user,
                            uint32_t _action,
                            uint64_t _seed,
                            std::vector<battle_status_info> &_my_status_list,
                            std::vector<battle_status_info> &_enemy_status_list,
                            uint64_t _my_key, character_action_data &_action_info,
                            std::vector<std::string> &_data)
{
    std::string action_data;
    _action_info.my_position = _my_status_list[_my_key].position;
    _action_info.action_type = _action;
    if (_action == action_type::attack)
    {
        int enemy_key = get_random_target(_enemy_status_list, _seed, _enemy_status_list.size(), 0);
        if (enemy_key == -1) 
        {
            return false;
        }
        action_info new_action;
        new_action = get_target_action(_action, _seed, _my_key, enemy_key, _my_status_list, _enemy_status_list);

        set_random_damage(new_action, _seed);       
        result_type_damage(_user, new_action, _my_status_list, _enemy_status_list, _my_key, enemy_key);
        result_buff(new_action, _enemy_status_list[enemy_key]);    

        if (_enemy_status_list[enemy_key].now_hp <= new_action.damage)
        {
            _enemy_status_list[enemy_key].now_hp = 0;
            _enemy_status_list[enemy_key].state = battle_member_state::dead;
        }
        else
        {
            _enemy_status_list[enemy_key].now_hp -= new_action.damage;
        }
        _action_info.action_info_list.push_back(new_action);
    }
    else if (_action == action_type::skill)
    {
        active_db active_db_table(_self, _self.value);
        auto active_iter = active_db_table.find(_my_status_list[_my_key].active_skill_list[0]);
        eosio_assert(active_iter != active_db_table.end(), "Set Action : Empty Active ID / Wrong Active ID");

        if (active_iter->skill_type == active_skill_type::type_heal) 
        {
            int enemy_key = get_heal_target(_my_status_list);
            if (enemy_key == -1)
            {
               enemy_key = _my_key;
            }
            action_info new_action;
            new_action = get_target_action(_action, _seed, _my_key, enemy_key, _my_status_list, _my_status_list);
            set_random_damage(new_action, _seed);      
            if (_my_status_list[enemy_key].max_hp > _my_status_list[enemy_key].now_hp + new_action.damage)
            {
                _my_status_list[enemy_key].now_hp += new_action.damage;
            }
            else
            {
                _my_status_list[enemy_key].now_hp = _my_status_list[enemy_key].max_hp;
            }
            _action_info.action_info_list.push_back(new_action);
        }
        else if(active_iter->skill_type == active_skill_type::type_buff)
        {
            buff_info new_buff;
            new_buff.id = active_iter->option_id;
            new_buff.turn = active_iter->active_turn;
            _my_status_list[_my_key].buff_list.push_back(new_buff);
        }
        else if(active_iter->skill_type == active_skill_type::type_attack_debuff)
        {
            buff_info new_buff;
            new_buff.id = active_iter->option_id;
            new_buff.turn = active_iter->active_turn;
            for (uint32_t i = 0; i < active_iter->target_count; ++i)
            {
                uint64_t new_seed = safeseed::get_seed_value(now(), _seed);
                int enemy_key = get_random_target(_enemy_status_list, new_seed, _enemy_status_list.size(), 0);
                if (enemy_key == -1) 
                {
                    return false;
                }
                new_seed = new_seed >> 1;

                action_info new_action;
                new_action = get_target_action(_action, _seed, _my_key, enemy_key, _my_status_list, _enemy_status_list);
                _enemy_status_list[enemy_key].buff_list.push_back(new_buff);
                _enemy_status_list[enemy_key].p_dfs -=  (_enemy_status_list[enemy_key].p_dfs * active_iter->atk_per_2) / 100;

                set_random_damage(new_action, _seed);      
                result_buff(new_action, _enemy_status_list[enemy_key]);                  

                uint64_t rate = safeseed::get_random_value(new_seed, 111, 90, 0);
                new_action.damage = (new_action.damage * rate) / 100;

                if (_enemy_status_list[enemy_key].now_hp <= new_action.damage)
                {
                    _enemy_status_list[enemy_key].now_hp = 0;
                    _enemy_status_list[enemy_key].state = battle_member_state::dead;
                }
                else
                {
                    _enemy_status_list[enemy_key].now_hp -= new_action.damage;
                }
                _action_info.action_info_list.push_back(new_action);
            }
        }
        else
        {
            for (uint32_t i = 0; i < active_iter->target_count; ++i)
            {
                uint64_t new_seed = safeseed::get_seed_value(now(), _seed);
                int enemy_key = get_random_target(_enemy_status_list, new_seed, _enemy_status_list.size(), 0);
                if (enemy_key == -1) 
                {
                    return false;
                }
                new_seed = new_seed >> 1;

                action_info new_action;
                new_action = get_target_action(_action, _seed, _my_key, enemy_key, _my_status_list, _enemy_status_list);

                set_random_damage(new_action, _seed);      
                result_type_skill(_user, new_action, _my_status_list, _enemy_status_list, _my_key, enemy_key);  
                result_buff(new_action, _enemy_status_list[enemy_key]);                  

                uint64_t rate = safeseed::get_random_value(new_seed, 111, 90, 0);
                new_action.damage = (new_action.damage * rate) / 100;

                if (_enemy_status_list[enemy_key].now_hp <= new_action.damage)
                {
                    _enemy_status_list[enemy_key].now_hp = 0;
                    _enemy_status_list[enemy_key].state = battle_member_state::dead;
                }
                else
                {
                    _enemy_status_list[enemy_key].now_hp -= new_action.damage;
                }
                _action_info.action_info_list.push_back(new_action);
            }
        }
    }

    // action_data += to_string(_action_info.my_position) + ":";
    // action_data += to_string(_action_info.action_type) + ":";
    // action_data += "[";
    // for (uint32_t i = 0; i < _action_info.action_info_list.size(); ++i)
    // {
    //     action_data += to_string(_action_info.action_info_list[i].target_position) + ":";
    //     action_data += to_string(_action_info.action_info_list[i].avoid) + ":";
    //     action_data += to_string(_action_info.action_info_list[i].critical) + ":";
    //     if (i + 1 == _action_info.action_info_list.size())
    //     {
    //         action_data += to_string(_action_info.action_info_list[i].damage);
    //     }
    //     else
    //     {
    //         action_data += to_string(_action_info.action_info_list[i].damage) + ":";
    //     }
    // }
    // action_data += "]";

    // _data.push_back(action_data);
    return true;
}

void battletest::set_skill_damage(uint32_t _skill_id, uint32_t &_attack, uint32_t _cur_skill_per)
{
    active_db active_db_table(_self, _self.value);
    auto active_iter = active_db_table.find(_skill_id);
    eosio_assert(active_iter != active_db_table.end(), "Skill Dmg Type : Empty Active ID / Wrong Activd ID");

    if(active_iter->skill_type == active_skill_type::type_attack)
    {
        _attack = (_attack * active_iter->atk_per_1) / 100;
    }
    else if(active_iter->skill_type == active_skill_type::type_heal)
    {
        uint32_t heal_per = active_iter->heal_per;
        // if (_cur_skill_per != 0)
        // {
        //     heal_per += (active_iter->heal_per * _cur_skill_per) / 100;
        // }
        _attack = (_attack * heal_per) / 100;
    }
    else
    {
        eosio_assert(1 == 0 ,"Skill Dmg Type : Empty Skill Type / Wrong Skill Type");
    }
    
}

battletest::action_info battletest::get_target_action(uint32_t _active_id, uint64_t _seed, uint64_t _my_key, uint64_t _target_key,
                                                      std::vector<battle_status_info> &_my_status_list, std::vector<battle_status_info> &_enemy_status_list)
{
    uint32_t cur_damage = 0;
    uint32_t cur_attack = 0;
    uint32_t cur_cirtical_dmg = 0;
    uint32_t cur_critical_dmg_per = 0;
    uint32_t cur_cri_per = 0;
    uint32_t cur_heal_skill_per = _my_status_list[_my_key].add_heal_skill_value_per;

    uint32_t target_defense = 0;
    uint32_t target_avoid = 0;


    switch (_active_id)
    {
    case action_type::attack:
    {
        cur_attack = _my_status_list[_my_key].p_atk;
        cur_cirtical_dmg = (_my_status_list[_my_key].p_atk * _my_status_list[_my_key].cri_dmg_per) / 100;
        cur_critical_dmg_per = _my_status_list[_my_key].cri_dmg_per;
        cur_cri_per = _my_status_list[_my_key].cri_per;

        target_defense = _enemy_status_list[_target_key].p_dfs;
        target_avoid = _enemy_status_list[_target_key].avoid;
        break;
    }
    case action_type::skill:
    {
        active_db active_db_table(_self, _self.value);
        auto active_iter = active_db_table.find(_my_status_list[_my_key].active_skill_list[0]);
        eosio_assert(active_iter != active_db_table.end(), "Skill Atk Dmg Type : Empty Active ID / Wrong Activd ID");

        cur_critical_dmg_per = _my_status_list[_my_key].cri_dmg_per;
        cur_cri_per = _my_status_list[_my_key].cri_per;

       
        if(active_iter->attack_type == atk_type::physical_atk)
        {
            cur_attack = _my_status_list[_my_key].p_atk;
        }
        else if(active_iter->attack_type == atk_type::magic_physical_atk)
        {
            cur_attack = _my_status_list[_my_key].p_atk;
            cur_attack += _my_status_list[_my_key].m_atk;
        }
        else if(active_iter->attack_type == atk_type::magic_atk)
        {
            cur_attack = _my_status_list[_my_key].m_atk;
        }

     
        if (active_iter->dmg_type == dmg_type::physical_dfs)
        {
            target_defense = _enemy_status_list[_target_key].p_dfs;
            target_avoid = _enemy_status_list[_target_key].avoid;
        }
        else if(active_iter->dmg_type == dmg_type::true_dmg)
        {
            target_defense = 0;
            target_avoid = _enemy_status_list[_target_key].avoid;
        }
        else if(active_iter->dmg_type == dmg_type::magic_dfs)
        {
            target_defense = _enemy_status_list[_target_key].m_dfs;
            target_avoid = _enemy_status_list[_target_key].avoid;
        }

       
        set_skill_damage(_my_status_list[_my_key].active_skill_list[0],
                         cur_attack, cur_heal_skill_per);
        
                    

        if(active_iter->option_id == active_option::option_perfectcri)
        {
            cur_cri_per = 100;
        }
        else if(active_iter->option_id == active_option::option_ignoreevade)
        {
            target_avoid = 100;
        }
        else if(active_iter->option_id == active_option::option_nocritical)
        {
            cur_cri_per = 0;
        }
        else if(active_iter->option_id == active_option::option_nocri_noavoid)
        {
            cur_cri_per = 0;
            target_avoid = 0;
            if (active_iter->skill_type == active_skill_type::type_heal)
            {
                target_defense = 0;
            }
        }

        cur_cirtical_dmg = (cur_attack * cur_critical_dmg_per) / 100;
        break;
    }
    default:
    {
        eosio_assert(1 == 0, "Target Action : Wrong Action Type");
        break;
    }
    }

    action_info new_action;
    if (true == check_avoid(target_avoid, _seed))
    {
        new_action.target_position = _enemy_status_list[_target_key].position;
        new_action.avoid = 1;
        new_action.critical = 0;
        new_action.damage = 0;
        return new_action;
    }
    else
    {
        if (false == check_critical(cur_cri_per, _seed))
        {
            cur_damage = get_damage(cur_attack, target_defense);
            new_action.target_position = _enemy_status_list[_target_key].position;
            new_action.avoid = 0;
            new_action.critical = 0;
            new_action.damage = cur_damage;
        }
        else
        {
            cur_damage = get_damage(cur_cirtical_dmg, target_defense);
            new_action.target_position = _enemy_status_list[_target_key].position;
            new_action.avoid = 0;
            new_action.critical = 1;
            new_action.damage = cur_damage;
        }
        return new_action;
    }
}


int battletest::get_state_position_key(std::vector<character_state_data> &_state_list, uint32_t _position)
{
    int target = -1;
    for(uint32_t i = 0 ; i < _state_list.size(); ++i)
    {
        if(_state_list[i].position == _position)
        {
            return i;
        }
    }
    return target;
}

int battletest::get_status_position_key(std::vector<battle_status_info> &_status_list, uint32_t _position)
{
    int target = -1;
    for(uint32_t i = 0 ; i < _status_list.size(); ++i)
    {
        if(_status_list[i].position == _position)
        {
            return i;
        }
    }
    return target;
}


void battletest::set_equipment_basic_status(eosio::name _from,
                                            eosio::name _to,
                                            std::vector<uint64_t> & _second_seed_list,
                                            std::vector<battle_status_info> &_my_status_list,
                                            std::vector<battle_status_info> &_enemy_status_list,
                                            const std::vector<character_state_data> &_my_state_list,
                                            const std::vector<character_state_data> &_enemy_state_list,
                                            const std::vector<uint32_t> &_my_synergy_list,
                                            const std::vector<uint32_t> &_enemy_synergy_list)
{
    
    for (uint32_t i = 0; i < _my_state_list.size(); ++i)
    {
        battle_status_info battle_status;
        battle_status.status = _my_state_list[i].status;
        battle_status.id = _my_state_list[i].id;
        battle_status.index = _my_state_list[i].index;
        battle_status.now_hp = _my_state_list[i].now_hp;
        battle_status.max_hp = _my_state_list[i].max_hp;
        battle_status.state = _my_state_list[i].state;
        battle_status.position = _my_state_list[i].position;
        battle_status.second_speed = _second_seed_list[i];
        battle_status.type = _my_state_list[i].type;
        battle_status.upgrade = _my_state_list[i].upgrade;
        battle_status.buff_list = _my_state_list[i].buff_list;
        battle_status.passive_skill_list = _my_state_list[i].passive_skill_list;
        battle_status.active_skill_list = _my_state_list[i].active_skill_list;
        battle_status.level = _my_state_list[i].level;

        if (_my_state_list[i].type == character_type::t_servant)   
        {
            user_servants user_servant_table(_self, _from.value);
            auto user_servant_iter = user_servant_table.find(_my_state_list[i].index);
            eosio_assert(user_servant_iter != user_servant_table.end(), "Set User State : Empty Servant Index / Wrong Servant Index");

            servant_db servant_db_table(_self, _self.value);
            auto servant_iter = servant_db_table.find(_my_state_list[i].id);
            eosio_assert(servant_iter != servant_db_table.end(), "Set Battle Status My : Empty Servant ID / Wrong Servant ID");

            serstat_db job_stat_db_table(_self, _self.value);
            uint32_t servant_job_base = (servant_iter->job * 1000) + (servant_iter->grade * 100) + 1;
            auto job_iter = job_stat_db_table.find(servant_job_base);
            eosio_assert(job_iter != job_stat_db_table.end(), "Set Battle Status My : Empty Servant Job / Wrong Servant Job");
            
            user_equip_items user_equipment_table(_self, _from.value);
            equipment_db equipment_db_table(_self, _self.value);
            for (uint32_t equip = 0; equip < user_servant_iter->servant.equip_slot.size(); ++equip) 
            {
                if (user_servant_iter->servant.equip_slot[equip] == 0)
                {
                    continue;
                }
                auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[equip]);
                eosio_assert(user_equipment_iter != user_equipment_table.end(), "Set User State : Empty Equipment Index / Wrong Equipment Index");

                auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
                eosio_assert(db_equipment_iter != equipment_db_table.end(), "Set User State : Empty Equipment ID / Wrong Equipment ID");

                uint32_t value = user_equipment_iter->equipment.value;
                set_upgrade_equip_status(user_equipment_iter->equipment.grade, value, user_equipment_iter->equipment.upgrade);

            
                for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
                {
                    if (db_equipment_iter->option_list[a] == option_list::status_str)
                    {
                        battle_status.status.basic_str += value;
                    }
                    else if (db_equipment_iter->option_list[a] == option_list::status_dex) 
                    {
                        battle_status.status.basic_dex += value;
                    }
                    else if (db_equipment_iter->option_list[a] == option_list::status_int)
                    {
                        battle_status.status.basic_int += value;
                    }
                }
            }
            set_passive_basic_status_self(battle_status);   
            
            battle_status.p_atk = get_physical_attack(battle_status.status, battle_status.level);
            battle_status.m_atk = get_magic_attack(battle_status.status, battle_status.level);
            battle_status.p_dfs = get_physical_defense(battle_status.status, battle_status.level);
            battle_status.m_dfs = get_magic_defense(battle_status.status, battle_status.level);
            battle_status.cri_per = get_cri_per(battle_status.status, battle_status.level);
            battle_status.cri_dmg_per = get_cri_dmg_per(battle_status.status, battle_status.level);
            battle_status.avoid = job_iter->avoid;
            battle_status.speed = job_iter->speed;

            for (uint32_t equip = 0; equip < user_servant_iter->servant.equip_slot.size(); ++equip) 
            {
                if (user_servant_iter->servant.equip_slot[equip] == 0)
                {
                    continue;
                }
                auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[equip]);
                eosio_assert(user_equipment_iter != user_equipment_table.end(), "Set User State : Empty Equipment Index / Wrong Equipment Index");

                auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
                eosio_assert(db_equipment_iter != equipment_db_table.end(), "Set User State : Empty Equipment ID / Wrong Equipment ID");

                uint32_t value = user_equipment_iter->equipment.value;
                set_upgrade_equip_status(user_equipment_iter->equipment.grade, value, user_equipment_iter->equipment.upgrade);

                for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
                {
                    if (db_equipment_iter->option_list[a] == option_list::physical_attack) 
                    {
                        battle_status.p_atk += value;
                    }
                    else if (db_equipment_iter->option_list[a] == option_list::magic_attack) 
                    {
                        battle_status.m_atk += value;
                    }
                    else if (db_equipment_iter->option_list[a] == option_list::physical_defense) 
                    {
                        battle_status.p_dfs += value;
                    }
                    else if (db_equipment_iter->option_list[a] == option_list::magic_defense) 
                    {
                        battle_status.m_dfs += value;
                    }
                }
            }
        }
        else       
        {
            monster_db monster_db_table(_self, _self.value);
            auto monster_iter = monster_db_table.find(_my_state_list[i].id);
            eosio_assert(monster_iter != monster_db_table.end(), "Set Battle Status My : Empty Monster ID / Wrong Monster ID");

            tribe_db class_stat_db_table(_self, _self.value);
            auto class_stat_iter = class_stat_db_table.find(monster_iter->tribe);
            eosio_assert(class_stat_iter != class_stat_db_table.end(), "Set Battle Status My : Empty Monster Tribe / Wrong Monster Tribe");

            set_upgrade_monster_status(_my_state_list[i].grade, battle_status.status, battle_status.upgrade);      

            set_passive_basic_status_self(battle_status);

            battle_status.p_atk = get_physical_attack(battle_status.status, battle_status.level);
            battle_status.m_atk = get_magic_attack(battle_status.status, battle_status.level);
            battle_status.p_dfs = get_physical_defense(battle_status.status, battle_status.level);
            battle_status.m_dfs = get_magic_defense(battle_status.status, battle_status.level);
            battle_status.cri_per = get_cri_per(battle_status.status, battle_status.level);
            battle_status.cri_dmg_per = get_cri_dmg_per(battle_status.status, battle_status.level);
            battle_status.avoid = class_stat_iter->avoid;
            battle_status.speed = class_stat_iter->speed;
        }
        init_buff_turn_self(battle_status);

        _my_status_list.push_back(battle_status);
    }



    for(uint32_t i = 0 ; i < _enemy_state_list.size(); ++i)
    {
        battle_status_info battle_status;
        battle_status.status = _enemy_state_list[i].status;
        battle_status.id = _enemy_state_list[i].id;
        battle_status.index = _enemy_state_list[i].index;
        battle_status.now_hp = _enemy_state_list[i].now_hp;
        battle_status.max_hp = _enemy_state_list[i].max_hp;
        battle_status.state = _enemy_state_list[i].state;
        battle_status.position = _enemy_state_list[i].position;
        battle_status.second_speed = _second_seed_list[i + 10];
        battle_status.type = _enemy_state_list[i].type;
        battle_status.upgrade = _enemy_state_list[i].upgrade;
        battle_status.level = _enemy_state_list[i].level;
        battle_status.buff_list = _enemy_state_list[i].buff_list;
        battle_status.passive_skill_list = _enemy_state_list[i].passive_skill_list;
        battle_status.active_skill_list = _enemy_state_list[i].active_skill_list;

        if (_enemy_state_list[i].type == character_type::t_servant)     
        {
            user_servants user_servant_table(_self, _to.value);
            auto user_servant_iter = user_servant_table.find(_enemy_state_list[i].index);
            eosio_assert(user_servant_iter != user_servant_table.end(), "Set User State : Empty Servant Index / Wrong Servant Index");

            servant_db servant_db_table(_self, _self.value);
            auto servant_iter = servant_db_table.find(_enemy_state_list[i].id);
            eosio_assert(servant_iter != servant_db_table.end(), "Set Battle Status My : Empty Servant ID / Wrong Servant ID");

            serstat_db job_stat_db_table(_self, _self.value);
            uint32_t servant_job_base = (servant_iter->job * 1000) + (servant_iter->grade * 100) + 1;
            auto job_iter = job_stat_db_table.find(servant_job_base);
            eosio_assert(job_iter != job_stat_db_table.end(), "Set Battle Status My : Empty Servant Job / Wrong Servant Job");

           
            if (_to != _from)  
            {
                user_equip_items user_equipment_table(_self, _to.value);
                equipment_db equipment_db_table(_self, _self.value);
                for (uint32_t equip = 0; equip < user_servant_iter->servant.equip_slot.size(); ++equip) 
                {
                    if (user_servant_iter->servant.equip_slot[equip] == 0)
                    {
                        continue;
                    }
                    auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[equip]);
                    eosio_assert(user_equipment_iter != user_equipment_table.end(), "Set User State : Empty Equipment Index / Wrong Equipment Index");

                    auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
                    eosio_assert(db_equipment_iter != equipment_db_table.end(), "Set User State : Empty Equipment ID / Wrong Equipment ID");

                    uint32_t value = user_equipment_iter->equipment.value;
                    set_upgrade_equip_status(user_equipment_iter->equipment.grade, value, user_equipment_iter->equipment.upgrade);

                    for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
                    {
                        if (db_equipment_iter->option_list[a] == option_list::status_str) 
                        {
                            battle_status.status.basic_str += value;
                        }
                        else if (db_equipment_iter->option_list[a] == option_list::status_dex) 
                        {
                            battle_status.status.basic_dex += value;
                        }
                        else if (db_equipment_iter->option_list[a] == option_list::status_int) 
                        {
                            battle_status.status.basic_int += value;
                        }
                    }
                }
            }
            set_passive_basic_status_self(battle_status);

            battle_status.p_atk = get_physical_attack(battle_status.status, battle_status.level);
            battle_status.m_atk = get_magic_attack(battle_status.status, battle_status.level);
            battle_status.p_dfs = get_physical_defense(battle_status.status, battle_status.level);
            battle_status.m_dfs = get_magic_defense(battle_status.status, battle_status.level);
            battle_status.cri_per = get_cri_per(battle_status.status, battle_status.level);
            battle_status.cri_dmg_per = get_cri_dmg_per(battle_status.status, battle_status.level);
            battle_status.avoid = job_iter->avoid;
            battle_status.speed = job_iter->speed;

            if (_to != _from)       
            {
                user_equip_items user_equipment_table(_self, _to.value);
                equipment_db equipment_db_table(_self, _self.value);
                for (uint32_t equip = 0; equip < user_servant_iter->servant.equip_slot.size(); ++equip) 
                {
                    if (user_servant_iter->servant.equip_slot[equip] == 0)
                    {
                        continue;
                    }
                    auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[equip]);
                    eosio_assert(user_equipment_iter != user_equipment_table.end(), "Set User State : Empty Equipment Index / Wrong Equipment Index");

                    auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
                    eosio_assert(db_equipment_iter != equipment_db_table.end(), "Set User State : Empty Equipment ID / Wrong Equipment ID");

                    uint32_t value = user_equipment_iter->equipment.value;
                    set_upgrade_equip_status(user_equipment_iter->equipment.grade, value, user_equipment_iter->equipment.upgrade);

                    for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
                    {
                        if (db_equipment_iter->option_list[a] == option_list::physical_attack) 
                        {
                            battle_status.p_atk += value;
                        }
                        else if (db_equipment_iter->option_list[a] == option_list::magic_attack) 
                        {
                            battle_status.m_atk += value;
                        }
                        else if (db_equipment_iter->option_list[a] == option_list::physical_defense) 
                        {
                            battle_status.p_dfs += value;
                        }
                        else if (db_equipment_iter->option_list[a] == option_list::magic_defense)
                        {
                            battle_status.m_dfs += value;
                        }
                    }
                }
            }
        }
        else            
        {
            monster_db monster_db_table(_self, _self.value);
            auto monster_iter = monster_db_table.find(_enemy_state_list[i].id);
            eosio_assert(monster_iter != monster_db_table.end(), "Set Battle Status My : Empty Monster ID / Wrong Monster ID");

            tribe_db class_stat_db_table(_self, _self.value);
            auto class_stat_iter = class_stat_db_table.find(monster_iter->tribe);
            eosio_assert(class_stat_iter != class_stat_db_table.end(), "Set Battle Status My : Empty Monster Tribe / Wrong Monster Tribe");

            set_upgrade_monster_status(_enemy_state_list[i].grade, battle_status.status, battle_status.upgrade);      

            set_passive_basic_status_self(battle_status);     

            battle_status.p_atk = get_physical_attack(battle_status.status, battle_status.level);
            battle_status.m_atk = get_magic_attack(battle_status.status, battle_status.level);
            battle_status.p_dfs = get_physical_defense(battle_status.status, battle_status.level);
            battle_status.m_dfs = get_magic_defense(battle_status.status, battle_status.level);
            battle_status.cri_per = get_cri_per(battle_status.status, battle_status.level);
            battle_status.cri_dmg_per = get_cri_dmg_per(battle_status.status, battle_status.level);
            battle_status.avoid = class_stat_iter->avoid;
            battle_status.speed = class_stat_iter->speed;
        }
        init_buff_turn_self(battle_status);
        _enemy_status_list.push_back(battle_status);
    }
    set_synergy_battle_status(_my_status_list,_enemy_status_list, _my_synergy_list, _enemy_synergy_list);
    set_passive_battle_status(_my_status_list,_enemy_status_list);
    set_synergy_battle_status(_enemy_status_list, _my_status_list, _my_synergy_list, _enemy_synergy_list);
    set_passive_battle_status(_enemy_status_list, _my_status_list);
}
void battletest::set_passive_basic_status_self(battle_status_info &_status)
{
    for (uint32_t skill = 0; skill < _status.passive_skill_list.size(); ++skill)
    {
        passive_db passive_db_table(_self, _self.value);
        auto passive_db_iter = passive_db_table.find(_status.passive_skill_list[skill]);
        eosio_assert(passive_db_iter != passive_db_table.end(), "Set Passive Effect My : Empty Passive ID / Wrong Passive ID");

        switch (passive_db_iter->effect_id)
        {
        case passive_effect_id::b_str:
        {
            if (passive_db_iter->effect_type_id == passive_effect_type_id::up)
            {
                _status.status.basic_str = check_over_plus_flow(_status.status.basic_str, passive_db_iter->effect_value_a);
            }
            else if(passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
            {
                _status.status.basic_str += (_status.status.basic_str * passive_db_iter->effect_value_a) / 100;
            }
            break;
        }
        case passive_effect_id::b_dex:
        {
            if (passive_db_iter->effect_type_id == passive_effect_type_id::up)
            {
                _status.status.basic_dex = check_over_plus_flow(_status.status.basic_dex, passive_db_iter->effect_value_a);
            }
            else if(passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
            {
                _status.status.basic_dex += (_status.status.basic_dex * passive_db_iter->effect_value_a) / 100;
            }            
            break;
        }
        case passive_effect_id::b_int:
        {
            if (passive_db_iter->effect_type_id == passive_effect_type_id::up)
            {
                _status.status.basic_int = check_over_plus_flow(_status.status.basic_int, passive_db_iter->effect_value_a);
            }
            else if(passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
            {
                _status.status.basic_int += (_status.status.basic_int * passive_db_iter->effect_value_a) / 100;
            }
            break;
        }
        default:
            break;
        }
    }
}


void battletest::set_passive_battle_status(std::vector<battle_status_info> &_my_status_list, std::vector<battle_status_info> &_enemy_status_list)
{
    for(uint32_t i = 0; i < _my_status_list.size(); ++i)
    {
        for(uint32_t skill = 0; skill < _my_status_list[i].passive_skill_list.size(); ++skill)
        {
            passive_db passive_db_table(_self, _self.value);
            auto passive_db_iter = passive_db_table.find(_my_status_list[i].passive_skill_list[skill]);
            eosio_assert(passive_db_iter != passive_db_table.end(),"Set Passive Effect My : Empty Passive ID / Wrong Passive ID");

            switch (passive_db_iter->effect_id)
            {
                case passive_effect_id::p_atk:
                {
                    if (passive_db_iter->target_id == passvie_target_id::t_self)
                    {
                        if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                        {
                            _my_status_list[i].p_atk += (_my_status_list[i].p_atk * passive_db_iter->effect_value_a) / 100;
                        }
                    }
                    break;
                }
                case passive_effect_id::m_atk:
                {
                    if (passive_db_iter->target_id == passvie_target_id::t_self)
                    {
                        if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                        {
                            _my_status_list[i].m_atk += (_my_status_list[i].m_atk * passive_db_iter->effect_value_a) / 100;
                        }
                    }
                    break;
                }
                case passive_effect_id::p_dfs:
                {
                    if (passive_db_iter->target_id == passvie_target_id::t_self)
                    {
                        if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                        {
                            _my_status_list[i].p_dfs += (_my_status_list[i].p_dfs * passive_db_iter->effect_value_a) / 100;
                        }
                    }
                    break;
                }
                case passive_effect_id::m_dfs:
                {
                    if (passive_db_iter->target_id == passvie_target_id::t_self)
                    {
                        if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                        {
                            _my_status_list[i].m_dfs += (_my_status_list[i].m_dfs * passive_db_iter->effect_value_a) / 100;
                        }
                    }
                    break;
                }
                case passive_effect_id::avoid:
                {
                    if (passive_db_iter->target_id == passvie_target_id::t_self)
                    {
                        if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                        {
                            _my_status_list[i].avoid += (_my_status_list[i].avoid * passive_db_iter->effect_value_a) / 100;
                        }
                    }
                    break;
                }
                case passive_effect_id::cri_per:
                {
                    if (passive_db_iter->target_id == passvie_target_id::t_self)
                    {
                        if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)
                        {
                            _my_status_list[i].cri_per += (_my_status_list[i].cri_per * passive_db_iter->effect_value_a) / 100;
                        }
                    }
                    break;
                }
                default:
                    break;
            }
        }
    }
}

void battletest::set_synergy_battle_status(std::vector<battle_status_info> &_my_status_list, std::vector<battle_status_info> &_enemy_status_list,
                                           const std::vector<uint32_t> &_my_synergy_list,
                                           const std::vector<uint32_t> &_enemy_synergy_list)
{
    for(uint32_t i = 0; i < _my_synergy_list.size();++i)
    {
        passive_db passive_db_table(_self, _self.value);
        auto passive_db_iter = passive_db_table.find(_my_synergy_list[i]);
        eosio_assert(passive_db_iter != passive_db_table.end(), "Set Synergy Effect My : Empty Synergy ID / Wrong Synergy ID");

        switch (passive_db_iter->effect_id)
        {
        case passive_effect_id::p_atk:
        {
            if (passive_db_iter->target_id == passvie_target_id::t_beast)   
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (_my_status_list[my].type == character_type::t_monster)  
                    {
                        monster_db monster_db_table(_self, _self.value);
                        auto monster_iter = monster_db_table.find(_my_status_list[my].id);
                        eosio_assert(monster_iter != monster_db_table.end(), "Battle Synergy Set : Empty Monster ID / Wrong Monster ID");
                        if (passvie_target_id::t_beast == (monster_iter->tribe + 900))  
                        {
                            if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up) 
                            {
                                _my_status_list[my].p_atk += (_my_status_list[my].p_atk * passive_db_iter->effect_value_a) / 100;
                            }
                        }
                    }
                }
            }
            break;
        }
        case passive_effect_id::m_atk:
        {
            if (passive_db_iter->target_id == passvie_target_id::t_demon)   
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (_my_status_list[my].type == character_type::t_monster)  
                    {
                        monster_db monster_db_table(_self, _self.value);
                        auto monster_iter = monster_db_table.find(_my_status_list[my].id);
                        eosio_assert(monster_iter != monster_db_table.end(), "Battle Synergy Set : Empty Monster ID / Wrong Monster ID");
                        if (passvie_target_id::t_demon == (monster_iter->tribe + 900))  
                        {
                            if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)  
                            {
                                _my_status_list[my].m_atk += (_my_status_list[my].m_atk * passive_db_iter->effect_value_a) / 100;
                            }
                        }
                    }
                }
            }
            break;
        }
        case passive_effect_id::p_dfs:
        {
            if (passive_db_iter->target_id == passvie_target_id::t_human)   
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (_my_status_list[my].type == character_type::t_monster)  
                    {
                        monster_db monster_db_table(_self, _self.value);
                        auto monster_iter = monster_db_table.find(_my_status_list[my].id);
                        eosio_assert(monster_iter != monster_db_table.end(), "Battle Synergy Set : Empty Monster ID / Wrong Monster ID");
                        if (passvie_target_id::t_human == (monster_iter->tribe + 900)) 
                        {
                            if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)  
                            {
                                _my_status_list[my].p_dfs += (_my_status_list[my].p_dfs * passive_db_iter->effect_value_a) / 100;
                            }
                        }
                    }
                }
            }
            break;
        }
        case passive_effect_id::m_dfs:
        {
            if (passive_db_iter->target_id == passvie_target_id::t_partyall)   
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up) 
                    {
                        _my_status_list[my].m_dfs += (_my_status_list[my].m_dfs * passive_db_iter->effect_value_a) / 100;
                    }
                }
            }
            break;
        }
        case passive_effect_id::avoid:
        {
            if (passive_db_iter->target_id == passvie_target_id::t_insect)   
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (_my_status_list[my].type == character_type::t_monster) 
                    {
                        monster_db monster_db_table(_self, _self.value);
                        auto monster_iter = monster_db_table.find(_my_status_list[my].id);
                        eosio_assert(monster_iter != monster_db_table.end(), "Battle Synergy Set : Empty Monster ID / Wrong Monster ID");
                        if (passvie_target_id::t_insect == (monster_iter->tribe + 900))  
                        {
                            if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)  
                            {
                                _my_status_list[my].avoid += (_my_status_list[my].avoid * passive_db_iter->effect_value_a) / 100;
                            }
                        }
                    }
                }
            }
            break;
        }
        case passive_effect_id::cri_per:
        {
            if (passive_db_iter->target_id == passvie_target_id::t_mysterious)  
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (_my_status_list[my].type == character_type::t_monster) 
                    {
                        monster_db monster_db_table(_self, _self.value);
                        auto monster_iter = monster_db_table.find(_my_status_list[my].id);
                        eosio_assert(monster_iter != monster_db_table.end(), "Battle Synergy Set : Empty Monster ID / Wrong Monster ID");
                        if (passvie_target_id::t_mysterious == (monster_iter->tribe + 900))  
                        {
                            if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up) 
                            {
                                _my_status_list[my].cri_per += (_my_status_list[my].cri_per * passive_db_iter->effect_value_a) / 100;
                            }
                        }
                    }
                }
            }
            break;
        }
        default:
            break;
        }
    }
}

void battletest::set_result_state(std::vector<battle_status_info> &_my_status_list,
                                  std::vector<battle_status_info> &_enemy_status_list,
                                  std::vector<character_state_data> &_my_state_list,
                                  std::vector<character_state_data> &_enemy_state_list)
{
    for (uint32_t i = 0; i < _my_state_list.size(); ++i)
    {
        int state_key = get_status_position_key(_my_status_list, _my_state_list[i].position);
        if (state_key != -1)
        {
            _my_state_list[i].buff_list.clear();
            _my_state_list[i].buff_list = _my_status_list[state_key].buff_list;
            _my_state_list[i].now_hp = _my_status_list[state_key].now_hp;
            _my_state_list[i].state = _my_status_list[state_key].state;
        }
    }

    for (uint32_t i = 0; i < _enemy_state_list.size(); ++i)
    {
        int state_key = get_status_position_key(_enemy_status_list, _enemy_state_list[i].position);
        if (state_key != -1)
        {
            _enemy_state_list[i].buff_list.clear();
            _enemy_state_list[i].buff_list = _enemy_status_list[state_key].buff_list;
            _enemy_state_list[i].now_hp = _enemy_status_list[state_key].now_hp;
            _enemy_state_list[i].state = _enemy_status_list[state_key].state;
        }
    }
}

ACTION battletest::activeturn(eosio::name _user, uint32_t _turn, std::string _seed)
{
    system_check(_user);

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
    eosio_assert(user_auth_iter->state == user_state::stage || user_auth_iter->state == user_state::pvp || user_auth_iter->state == user_state::tower, "Active Turn ; User State Not Stage / Not Yet Stage Start");

    battle_actions battle_action_table(_self, _self.value);
    auto user_battle_action_iter = battle_action_table.find(_user.value);
    eosio_assert(user_battle_action_iter != battle_action_table.end(), "Actvie Turn : Empty Action Table / Not Yet Stage Start");
    eosio_assert(user_battle_action_iter->turn == _turn - 1, "Active Turn : Different Turn / Wrong Turn");

    std::vector<uint64_t> order_random_list;
    safeseed::get_battle_rand_list(order_random_list, battle_seed);
    for(uint32_t i = 0; i < order_random_list.size(); ++i)
    {
        order_random_list[i] = safeseed::get_seed_value(i, order_random_list[i]);
    }

    std::string turn;
    std::vector<std::string> data;
   
    std::vector<battle_status_info> second_attack_order_list;
    std::vector<battle_status_info> attack_order_list;
    std::vector<battle_status_info> skill_order_list;
   
    std::vector<battle_status_info> my_battle_status_list;
    std::vector<battle_status_info> enemy_battle_status_list;

   
    set_equipment_basic_status(user_battle_state_iter->user, 
    user_battle_state_iter->enemy_user, 
    order_random_list, 
    my_battle_status_list, 
    enemy_battle_status_list, 
    user_battle_state_iter->my_state_list, 
    user_battle_state_iter->enemy_state_list,
    user_battle_state_iter->my_synergy_list,
    user_battle_state_iter->enemy_synergy_list);

    skill_order_list.insert(skill_order_list.end(), my_battle_status_list.begin(), my_battle_status_list.end());
    skill_order_list.insert(skill_order_list.end(), enemy_battle_status_list.begin(), enemy_battle_status_list.end());
    std::sort(skill_order_list.begin(), skill_order_list.end(), new_sort_compare);

    battle_state_list_table.modify(user_battle_state_iter, _self, [&](auto &character_state_data) {
        character_state_data.turn += 1;
     
        battle_action_table.modify(user_battle_action_iter, _self, [&](auto &update_action) {
            update_action.turn += 1;
            turn += to_string(update_action.turn);
            update_action.character_action_list.clear();
          
            for (uint32_t i = 0; i < skill_order_list.size(); ++i)  
            {
                if (skill_order_list[i].position < max_party_count)
                {
                    int my_key = get_status_position_key(my_battle_status_list, skill_order_list[i].position);
                    if(my_key == -1)
                    {
                        continue;
                    }
                    else if(my_battle_status_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    else if (my_battle_status_list[my_key].active_skill_list.size() != 0) 
                    {
                        uint64_t action_rate = safeseed::get_random_value(skill_order_list[i].second_speed, 100, 0, 0);
                        if (true == check_activate_skill(my_battle_status_list[my_key].active_skill_list[0], action_rate)) 
                        {
                            active_db active_db_table(_self, _self.value);
                            auto active_iter = active_db_table.find(my_battle_status_list[my_key].active_skill_list[0]);
                            eosio_assert(active_iter != active_db_table.end(), "Actvie Turn My : Empty Active ID / Wrong Active ID");
                            if (active_iter->skill_type == active_skill_type::type_attack)  
                            {
                                if (active_iter->option_id == active_option::option_fastattack) 
                                {
                                    skill_order_list[i].action = action_type::skill;
                                    attack_order_list.push_back(skill_order_list[i]);
                                }
                                else
                                {
                                    skill_order_list[i].action = action_type::skill;
                                    second_attack_order_list.push_back(skill_order_list[i]);
                                }
                            }
                            else if (active_iter->skill_type == active_skill_type::type_buff) 
                            {
                                character_action_data action_info;
                                if (false == set_action(_user, action_type::skill,
                                                        my_battle_status_list[my_key].second_speed,
                                                        my_battle_status_list,
                                                        enemy_battle_status_list,
                                                        my_key, action_info, data))
                                {
                                    break;
                                }
                                update_action.character_action_list.push_back(action_info);
                            }
                            else if (active_iter->skill_type == active_skill_type::type_heal)
                            {
                                character_action_data action_info;
                                if (false == set_action(_user, action_type::skill,
                                                        my_battle_status_list[my_key].second_speed,
                                                        my_battle_status_list,
                                                        enemy_battle_status_list,
                                                        my_key, action_info, data))
                                {
                                    break;
                                }
                                update_action.character_action_list.push_back(action_info);
                            }
                        }
                        else 
                        {
                            skill_order_list[i].action = action_type::attack;
                            second_attack_order_list.push_back(skill_order_list[i]);
                        }
                    }
                    else 
                    {
                        skill_order_list[i].action = action_type::attack;
                        second_attack_order_list.push_back(skill_order_list[i]);
                    }
                }
                else 
                {
                    int my_key = get_status_position_key(enemy_battle_status_list, skill_order_list[i].position); 
                    if(my_key == -1)
                    {
                        continue;
                    }
                    else if (enemy_battle_status_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    else if (enemy_battle_status_list[my_key].active_skill_list.size() != 0)
                    {
                        uint64_t action_rate = safeseed::get_random_value(skill_order_list[i].second_speed, 100, 0, 0); 
                        if (true == check_activate_skill(enemy_battle_status_list[my_key].active_skill_list[0], action_rate)) 
                        {
                            active_db active_db_table(_self, _self.value);
                            auto active_iter = active_db_table.find(enemy_battle_status_list[my_key].active_skill_list[0]);
                            eosio_assert(active_iter != active_db_table.end(), "Actvie Turn My : Empty Active ID / Wrong Active ID");
                            if (active_iter->skill_type == active_skill_type::type_attack)  
                            {
                                if (active_iter->option_id == active_option::option_fastattack) 
                                {
                                    skill_order_list[i].action = action_type::skill;
                                    attack_order_list.push_back(skill_order_list[i]);
                                }
                                else
                                {
                                    skill_order_list[i].action = action_type::skill;
                                    second_attack_order_list.push_back(skill_order_list[i]);
                                }
                            }
                            else if (active_iter->skill_type == active_skill_type::type_buff) 
                            {
                                character_action_data action_info;
                                if (false == set_action(_user, action_type::skill,
                                                        enemy_battle_status_list[my_key].second_speed,
                                                        enemy_battle_status_list,
                                                        my_battle_status_list,
                                                        my_key, action_info, data))
                                {
                                    break;
                                }
                                update_action.character_action_list.push_back(action_info);
                            }
                            else if (active_iter->skill_type == active_skill_type::type_heal) 
                            {
                                character_action_data action_info;
                                if (false == set_action(_user, action_type::skill,
                                                        enemy_battle_status_list[my_key].second_speed,
                                                        enemy_battle_status_list,
                                                        my_battle_status_list,
                                                        my_key, action_info, data))
                                {
                                    break;
                                }
                                update_action.character_action_list.push_back(action_info);
                            }
                        }
                        else
                        {
                            skill_order_list[i].action = action_type::attack;
                            second_attack_order_list.push_back(skill_order_list[i]);
                        }
                    }
                    else   
                    {
                        skill_order_list[i].action = action_type::attack;
                        second_attack_order_list.push_back(skill_order_list[i]);
                    }
                }
            }
       
            for (uint32_t i = 0; i < attack_order_list.size(); ++i)
            {
                if (attack_order_list[i].position < max_party_count) 
                {
                    int my_key = get_status_position_key(my_battle_status_list, attack_order_list[i].position); 
                    if(my_key == -1)
                    {
                        continue;
                    }
                    else if (my_battle_status_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    character_action_data action_info;
                    if (false == set_action(_user, 
                                            action_type::skill,
                                            my_battle_status_list[my_key].second_speed,
                                            my_battle_status_list,
                                            enemy_battle_status_list,
                                            my_key, 
                                            action_info,
                                            data))
                    {
                        break;
                    }
                    update_action.character_action_list.push_back(action_info);
                }
                else 
                {
                    int my_key = get_status_position_key(enemy_battle_status_list, attack_order_list[i].position); 
                    if(my_key == -1)
                    {
                        continue;
                    }
                    else if (enemy_battle_status_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    character_action_data action_info;
                    if (false == set_action(_user, 
                                            action_type::skill,
                                            enemy_battle_status_list[my_key].second_speed,
                                            enemy_battle_status_list,
                                            my_battle_status_list,
                                            my_key, 
                                            action_info, 
                                            data))
                    {
                        break;
                    }
                    update_action.character_action_list.push_back(action_info);
                }
            }
          
            for (uint32_t i = 0; i < second_attack_order_list.size(); ++i)
            {
                if (second_attack_order_list[i].position < max_party_count) 
                {
                    int my_key = get_status_position_key(my_battle_status_list, second_attack_order_list[i].position); 
                    if(my_key == -1)
                    {
                        continue;
                    }
                    else if (my_battle_status_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    character_action_data action_info;
                    if (false == set_action(_user, 
                                            second_attack_order_list[i].action,
                                            my_battle_status_list[my_key].second_speed,
                                            my_battle_status_list,
                                            enemy_battle_status_list,
                                            my_key, 
                                            action_info, 
                                            data))
                    {
                        break;
                    }
                    update_action.character_action_list.push_back(action_info);
                }
                else 
                {
                    int my_key = get_status_position_key(enemy_battle_status_list, second_attack_order_list[i].position);
                    if(my_key == -1)
                    {
                        continue;
                    }
                    else if (enemy_battle_status_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    character_action_data action_info;
                    if (false == set_action(_user, 
                                            second_attack_order_list[i].action,
                                            enemy_battle_status_list[my_key].second_speed,
                                            enemy_battle_status_list,
                                            my_battle_status_list,
                                            my_key, 
                                            action_info, 
                                            data))
                    {
                        break;
                    }
                    update_action.character_action_list.push_back(action_info);
                }
            }
        
            for (uint32_t i = 0; i < my_battle_status_list.size(); ++i)
            {
                if (my_battle_status_list[i].state == battle_member_state::dead)
                {
                    user_dead_count += 1;
                }
            }

            for (uint32_t i = 0; i < enemy_battle_status_list.size(); ++i)
            {
                if (enemy_battle_status_list[i].state == battle_member_state::dead)
                {
                    enemy_dead_count += 1;
                }
            }
  
            set_result_state(my_battle_status_list, enemy_battle_status_list, character_state_data.my_state_list, character_state_data.enemy_state_list);
        });
    });

    if (user_auth_iter->state == user_state::stage)
    {
        if (enemy_dead_count == enemy_battle_status_list.size())
        {
            uint32_t stage_number = get_stage_id(user_battle_state_iter->stage_type, user_battle_state_iter->stage_number);
            win_reward(_user, stage_number, battle_seed);
        }
        else if (user_dead_count == my_battle_status_list.size())
        {
            uint32_t stage_number = get_stage_id(user_battle_state_iter->stage_type, user_battle_state_iter->stage_number);
            fail_reward(_user, stage_number);
        }
    }
    else
    {
        if (enemy_dead_count == enemy_battle_status_list.size())
        {
            pvp_log_index pvp_log_table(_self, _user.value);
            pvp_log_table.emplace(_user, [&](auto &data) {
                data.index = pvp_log_table.available_primary_key();
                data.user = user_battle_state_iter->enemy_user;
                data.result = "win";
            });
            user_auths user_auth_table(_self, _self.value);
            auto user_auth_iter = user_auth_table.find(_user.value);
            eosio_assert(user_auth_iter != user_auth_table.end(), "Pvp Result : Empty Auth Table / Not Yet Signup");
            eosio_assert(user_auth_iter->state == user_state::pvp, "Pvp Result ; User State Not Stage / Not Yet Stage Start");
            user_auth_table.modify(user_auth_iter, _self, [&](auto &end_pvp) {
                end_pvp.state = user_state::lobby;
            });
        }
        else if (user_dead_count == my_battle_status_list.size())
        {
            pvp_log_index pvp_log_table(_self, _user.value);
            pvp_log_table.emplace(_user, [&](auto &data) {
                data.index = pvp_log_table.available_primary_key();
                data.user = user_battle_state_iter->enemy_user;
                data.result = "lose";
            });
            user_auths user_auth_table(_self, _self.value);
            auto user_auth_iter = user_auth_table.find(_user.value);
            eosio_assert(user_auth_iter != user_auth_table.end(), "Pvp Result : Empty Auth Table / Not Yet Signup");
            eosio_assert(user_auth_iter->state == user_state::pvp, "Pvp Result ; User State Not Stage / Not Yet Stage Start");
            user_auth_table.modify(user_auth_iter, _self, [&](auto &end_pvp) {
                end_pvp.state = user_state::lobby;
            });
        }
    }

    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "battleaction"_n,
    //        std::make_tuple(_user, turn ,data))
    //     .send();
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

    serstat_db servant_base_table(_self, _self.value);
    uint32_t servant_job_base = (servant_id_db_iter.job * 1000) + (servant_id_db_iter.grade * 100) + 1;
    const auto &ser_iter = servant_base_table.get(servant_job_base, "Get Reward Servant : Empty Servant Stat");

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

        new_servant.status.basic_str = change_servant_statue(new_servant.status.basic_str) + ser_iter.base_str;
        new_servant.status.basic_dex = change_servant_statue(new_servant.status.basic_dex) + ser_iter.base_dex;
        new_servant.status.basic_int = change_servant_statue(new_servant.status.basic_int) + ser_iter.base_int;

        new_servant.equip_slot.resize(3);
        new_servant.state = object_state::on_inventory;

        uint32_t active_id = get_servant_active_skill(servant_id_db_iter.job, _seed);
        new_servant.active_skill.push_back(active_id);

        uint32_t passive_id = get_servant_passive_skill(servant_id_db_iter.job, _seed);
        new_servant.passive_skill.push_back(passive_id);

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

    tribe_db tribe_db_table(_self, _self.value);
    const auto &tribe_iter = tribe_db_table.get(monster_id_db_iter.tribe, "Gacha Reward Monster : Empty Monster Tribe");

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
        new_monster.tribe = monster_id_db_iter.tribe;
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

        new_monster.status.basic_str = (new_monster.status.basic_str * tribe_iter.base_str) / 100;
        new_monster.status.basic_dex = (new_monster.status.basic_dex * tribe_iter.base_dex) / 100;
        new_monster.status.basic_int = (new_monster.status.basic_int * tribe_iter.base_int) / 100;

        uint32_t passive_id = get_monster_passive_skill(_seed);
        new_monster.passive_skill.push_back(passive_id);

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
        uint32_t type_grade = ((equip_item_iter.type + 1) * 10)  + item_grade_db_iter.grade;
        new_item.value = safeseed::get_random_value(_seed, 10, 0, item_random_count);
        new_item.value = change_equipment_statue(type_grade, new_item.value);
        set_tier_status(new_item.value, equip_item_iter.tier);

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

    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.begin();
    eosio_assert(user_party_iter != user_party_table.end(), "Win Reward : Empty Party Number / Not Yet Signup");

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
            update_monster_exp.monster.level += level_up_count;
            update_monster_exp.monster.exp = get_exp;

            exp_info char_exp;
            char_exp.pos = monster_pos_list[i];
            char_exp.exp = reward_iter->char_exp;
            char_exp.lvup = level_up_count;
            get_char_exp_list.push_back(char_exp);
        });
    }

    for (uint32_t i = 0; i < reward_iter->reward_list.size(); ++i)
    {
        if (reward_iter->reward_list[i].type == 1) //servant
        {
            uint64_t seed = _seed;
            uint64_t rate = safeseed::get_random_value(seed, 10000, 0, 0);
            if (reward_iter->reward_list[i].per >= rate)
            {
                for (uint32_t ser = 0; ser < reward_iter->reward_list[i].count; ++ser)
                {
                    uint32_t job = reward_iter->reward_list[i].id / 1000000;
                    servant_data new_servant = get_reward_servant(_user, job, reward_iter->reward_list[i].grade, seed);
                    servant_list.push_back(new_servant);
                    seed = seed >> 1;
                }
            }
        }
        else if (reward_iter->reward_list[i].type == 2) //monster
        {
            uint64_t seed = _seed;
            uint64_t rate = safeseed::get_random_value(seed, 10000, 0, 1);
            if (reward_iter->reward_list[i].per >= rate)
            {
                for (uint32_t mon = 0; mon < reward_iter->reward_list[i].count; ++mon)
                {
                    monster_data new_monster = get_reward_monster(_user, reward_iter->reward_list[i].id, reward_iter->reward_list[i].grade, seed);
                    monster_list.push_back(new_monster);
                    seed = seed >> 1;
                }
            }
        }
        else if (reward_iter->reward_list[i].type == 3) //equipment
        {
            uint64_t seed = _seed;
            uint64_t rate = safeseed::get_random_value(seed, 10000, 0, 2);
            if (reward_iter->reward_list[i].per >= rate)
            {
                for (uint32_t equip = 0; equip < reward_iter->reward_list[i].count; ++equip)
                {
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
        update_log.get_utg += stage_reward_money.amount;
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
            set_reward.get_servant_list = servant_list;
            set_reward.get_monster_list = monster_list;
            set_reward.get_equipment_list = equipment_list;
            set_reward.get_item_list = item_list;
        });
    }
    else
    {
        battle_reward_list_table.modify(user_battle_reward_iter, _self, [&](auto &set_reward) {
            set_reward.get_char_exp_list.clear();
            set_reward.reward_money = stage_reward_money.amount;
            set_reward.get_rank_exp = get_rank_exp;
            set_reward.get_char_exp_list = get_char_exp_list;
            set_reward.get_servant_list = servant_list;
            set_reward.get_monster_list = monster_list;
            set_reward.get_equipment_list = equipment_list;
            set_reward.get_item_list = item_list;
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
    system_check(_user);

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Stage Exit : Empty Auth Table / Not Yet Signup");
    eosio_assert(user_auth_iter->state != user_state::lobby, "Stage Exit : Already End Stage");
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



//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


ACTION battletest::changetoken(eosio::name _user, std::string _type, uint64_t _index)
{
    require_auth("epochchasert"_n);
    if (_type == "servant")
    {
        user_servants user_servant_table(_self, _user.value);
        auto servant_iter = user_servant_table.find(_index);
        eosio_assert(servant_iter != user_servant_table.end(), "changetoken : Not Exist Servant");
        eosio_assert(servant_iter->servant.state == object_state::on_inventory, "changetoken : Not Inventory Servant");
        eosio_assert(servant_iter->party_number == 0, "changetoken : Already Party Servant");
        user_servant_table.modify(servant_iter, _self, [&](auto &new_token) {
            new_token.servant.state = object_state::on_tokenization;
        });
    }
    else if (_type == "monster")
    {
        user_monsters user_monster_table(_self, _user.value);
        auto monster_iter = user_monster_table.find(_index);
        eosio_assert(monster_iter != user_monster_table.end(), "changetoken : Not Exist Monster");
        eosio_assert(monster_iter->monster.state == object_state::on_inventory, "changetoken : Not Inventory Monster");
        eosio_assert(monster_iter->party_number == 0, "changetoken : Already Party Monster");
        user_monster_table.modify(monster_iter, _self, [&](auto &new_token) {
            new_token.monster.state = object_state::on_tokenization;
        });
    }
    else if (_type == "equipment")
    {
        user_equip_items user_equipment_table(_self, _user.value);
        auto equipment_iter = user_equipment_table.find(_index);
        eosio_assert(equipment_iter != user_equipment_table.end(), "changetoken : Not Exist equipment");
        eosio_assert(equipment_iter->equipment.state == object_state::on_inventory, "changetoken : Not Inventory Equipment");
        user_equipment_table.modify(equipment_iter, _self, [&](auto &new_token) {
            new_token.equipment.state = object_state::on_tokenization;
        });
    }
    else
    {
        eosio_assert(1 == 0, "changetoken : Wrong Type Token");
    }
}

#pragma endregion

#pragma region sell contents function
//------------------------------------------------------------------------//
//-------------------------sell_function------------------------//
//------------------------------------------------------------------------//

ACTION battletest::burn(eosio::name _user, uint64_t _type, const std::vector<uint64_t> &_list)
{    
    system_check(_user);

    switch(_type)
    {
        case 1:
        {
            servantburn(_user, _list);
            break;
        }
        case 2:
        {
            monsterburn(_user, _list);
            break;
        }
        case 3:
        {
            equipburn(_user, _list);
            break;
        }
        default:
        {
            eosio_assert(1==0, "burn : wrong Type");
        }
    }
}

void battletest::servantburn(eosio::name _user, const std::vector<uint64_t> &_list)
{
    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    user_logs user_logs_table(_self, _self.value);
    auto user_logs_iter = user_logs_table.find(_user.value);
    user_items user_items_table(_self, _user.value);
    user_servants user_servant_table(_self, _user.value);
    burnitem_db burnitem_db_table(_self, _self.value);
    servant_db servant_db_table(_self, _self.value);

    std::string contents_list;

    uint64_t check_item_count = 0;
    uint64_t check_inventory = 0;
    item_info items;

    asset servant_burn_result(0, symbol(symbol_code("UTG"), 4));

    for (uint32_t i = 0; i < _list.size(); ++i)
    {
        auto user_servant_iter = user_servant_table.find(_list[i]);
        eosio_assert(user_servant_iter != user_servant_table.end(), "servantburn : Empty servant info");
        eosio_assert(user_servant_iter->party_number == 0, "servantburn : this servant already in party");
        auto servant_db_iter = servant_db_table.find(user_servant_iter->servant.id);
        auto burnitem_db_iter = burnitem_db_table.find(servant_db_iter->job);
        auto user_items_iter = user_items_table.find(burnitem_db_iter->result_item_id);
        eosio_assert(user_auth_iter->current_servant_inventory >= 0, "servantburn : current_servant_inventory underflow error");

        for (uint64_t j = 0; j < user_servant_iter->servant.equip_slot.size(); j++)
        {
            eosio_assert(user_servant_iter->servant.equip_slot[j] == 0, "servantburn : this servant equip equipment");
        }

        if (user_servant_iter->servant.state == object_state::on_inventory)
        {
            if (user_items_iter == user_items_table.end())
            {
                user_items_table.emplace(_self, [&](auto &change_consumable) {
                    change_consumable.id = burnitem_db_iter->result_item_id;
                    items.index = 0;
                    change_consumable.type = 0;
                    if (user_servant_iter->servant.exp >= 960400)
                    {
                        items.count = 5;
                        change_consumable.item_list.push_back(items);
                        check_item_count = 5;
                        servant_burn_result.amount += 2500000;
                    }
                    else
                    {
                        items.count = 1;
                        change_consumable.item_list.push_back(items);
                        check_item_count = 1;
                        servant_burn_result.amount += 50000;
                    }
                    // contents_list += "[" + to_string(user_servant_iter->index) + ":";
                    // contents_list += to_string(user_servant_iter->servant.id) + ":";
                    // contents_list += to_string(user_items_iter->id) + ":";
                    // contents_list += to_string(user_items_iter->type) + ":";
                    // for (uint64_t i = 0; i < change_consumable.item_list.size(); i++)
                    // {
                    //     contents_list += to_string(user_items_iter->item_list[i].index) + ":";
                    //     contents_list += to_string(user_items_iter->item_list[i].count) + "]";
                    // }
                });

                check_inventory += 1;
                user_servant_table.erase(user_servant_iter);
            }
            else
            {
                user_items_table.modify(user_items_iter, _self, [&](auto &change_consumable) {
                    if (user_servant_iter->servant.exp >= 960400)
                    {
                        uint64_t count_diff = 5;
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
                            change_consumable.item_list.push_back(items);
                        }
                        check_item_count = 5;
                        servant_burn_result.amount += 2500000;
                    }
                    else
                    {
                        uint64_t count_diff = 1;
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
                        check_item_count = 1;
                        if (count_diff != 0)
                        {
                            items.index = change_consumable.item_list.size();
                            items.count += count_diff;
                            change_consumable.item_list.push_back(items);
                            check_inventory += 1;
                        }
                        servant_burn_result.amount += 50000;
                    }
                    // contents_list += to_string(user_servant_iter->index) + ":";
                    // contents_list += to_string(user_servant_iter->servant.id) + ":";
                    // contents_list += to_string(user_items_iter->id) + ":";
                    // contents_list += to_string(user_items_iter->type) + ":";
                    // for(uint64_t i =0; i< change_consumable.item_list.size();i++)
                    // {
                    // contents_list += to_string(user_items_iter->item_list[i].index) + ":";
                    // contents_list += to_string(user_items_iter->item_list[i].count) + ",";
                    // }
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
        eosio_assert(change_auth_user.current_servant_inventory >= _list.size(),"servant burn : The current number of servant and the number of units to sell are not correct.");
        if (change_auth_user.current_servant_inventory - _list.size() <= 0)
        {
            change_auth_user.current_servant_inventory = 0;
        }
        else
        {
            change_auth_user.current_servant_inventory -= _list.size();
        }
        change_auth_user.current_item_inventory += check_inventory;
    });

    user_logs_table.modify(user_logs_iter, _self, [&](auto &change_log){
        change_log.get_utg += servant_burn_result.amount;
    });


    action(permission_level{get_self(), "active"_n},
           get_self(), "transfer"_n,
           std::make_tuple(_self, _user, servant_burn_result, std::string("servant burn result")))
        .send();

    // std::string contents_type = "servantburn";
    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "contentslist"_n,
    //        std::make_tuple(_user, contents_type, contents_list))
    //     .send();
}



void battletest::monsterburn(eosio::name _user, const std::vector<uint64_t> &_list)
{
    
    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    user_logs user_logs_table(_self, _self.value);
    auto user_logs_iter = user_logs_table.find(_user.value);

    user_monsters user_monster_table(_self, _user.value);

    asset monster_burn_result(0, symbol(symbol_code("UTG"), 4));

    std::string contents_list;

    for (uint32_t i = 0; i < _list.size(); ++i)
    {
        //uint64_t UTG_Amount = 0;
        auto user_monster_iter = user_monster_table.find(_list[i]);
        eosio_assert(user_monster_iter != user_monster_table.end(), "monsterburn : not exist monster info");
        eosio_assert(user_monster_iter->party_number == 0, "monsterburn : this monster already in party");

        if (user_monster_iter->monster.state == object_state::on_inventory)
        {
            if (user_monster_iter->monster.exp >= 960400)
            {
                monster_burn_result.amount += 1000000;
            }
            else
            {
                monster_burn_result.amount += 50000;
            }

            user_monster_table.erase(user_monster_iter);
            // UTG_Amount = monster_burn_result.amount;
            // contents_list += to_string(user_monster_iter->index) + ":";
            // contents_list += to_string(user_monster_iter->monster.id) + ":";
            // contents_list += to_string(UTG_Amount) + ",";
        }
        else
        {
            eosio_assert(1 == 0, "monsterburn : this monster is not inventory state");
        }
    }

    action(permission_level{get_self(), "active"_n},
           get_self(), "transfer"_n,
           std::make_tuple(_self, _user, monster_burn_result, std::string("monster burn result")))
        .send();

    user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
        eosio_assert(change_auth_user.current_monster_inventory >= _list.size(),"monster burn : The current number of monster and the number of units to sell are not correct.");
        if (change_auth_user.current_monster_inventory - _list.size() <= 0)
        {
            change_auth_user.current_monster_inventory = 0;
        }
        else
        {
            change_auth_user.current_monster_inventory -= _list.size();
        }
    });

    user_logs_table.modify(user_logs_iter, _self, [&](auto &change_log) {
        change_log.get_utg += monster_burn_result.amount;
    });

    // std::string contents_type = "monsterburn";
    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "contentslist"_n,
    //        std::make_tuple(_user, contents_type, contents_list))
    //     .send();
}

void battletest::equipburn(eosio::name _user, const std::vector<uint64_t> &_list)
{

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    user_logs user_logs_table(_self, _self.value);
    auto user_logs_iter = user_logs_table.find(_user.value);

    user_equip_items user_equipment_table(_self, _user.value);

    asset equipment_burn_result(0, symbol(symbol_code("UTG"), 4));

    std::string contents_list;

    for (uint32_t i = 0; i < _list.size(); ++i)
    {
        //uint64_t UTG_Amount = 0;
        auto user_equipment_iter = user_equipment_table.find(_list[i]);
        eosio_assert(user_equipment_iter != user_equipment_table.end(), "equipburn : not exist equipment info");
        eosio_assert(user_equipment_iter->equipment.state != object_state::on_equip_slot, "equipburn : this equipment is on equip");

        if (user_equipment_iter->equipment.state == object_state::on_inventory)
        {
            if (user_equipment_iter->equipment.grade == 1)
            {
                equipment_burn_result.amount += 500000;
               // UTG_Amount += 500000;
            }
            else if (user_equipment_iter->equipment.grade == 2)
            {
                equipment_burn_result.amount += 150000;
                //UTG_Amount += 150000;
            }
            else if (user_equipment_iter->equipment.grade == 3)
            {
                equipment_burn_result.amount += 100000;
               // UTG_Amount += 100000;
            }
            else if (user_equipment_iter->equipment.grade == 4)
            {
                equipment_burn_result.amount += 70000;
                //UTG_Amount += 70000;
            }
            else if (user_equipment_iter->equipment.grade == 5)
            {
                equipment_burn_result.amount += 50000;
                //UTG_Amount += 50000;
            }
            else
            {
               eosio_assert(1 == 0 ,"equipburn : Wrong Type equipment grade");
            }
          
            // contents_list += to_string(user_equipment_iter->index) + ":";
            // contents_list += to_string(user_equipment_iter->equipment.id) + ":";
            // contents_list += to_string(user_equipment_iter->equipment.grade) + ":";
            // contents_list += to_string(UTG_Amount) + ",";
            user_equipment_table.erase(user_equipment_iter);
        }
        else
        {
            eosio_assert(1 == 0, "equipburn : this equipment is not inventory state");
        }
    }

    action(permission_level{get_self(), "active"_n},
           get_self(), "transfer"_n,
           std::make_tuple(_self, _user, equipment_burn_result, std::string("equipment burn result")))
        .send();

    user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
        eosio_assert(change_auth_user.current_equipment_inventory >= _list.size(), "equipburn : The current number of equipment and the number of units to sell are not correct.");
        if (change_auth_user.current_equipment_inventory - _list.size() <= 0)
        {
            change_auth_user.current_equipment_inventory = 0;
        }
        else
        {
            change_auth_user.current_equipment_inventory -= _list.size();
        }
    });

    user_logs_table.modify(user_logs_iter, _self, [&](auto &change_log) {
        change_log.get_utg += equipment_burn_result.amount;
    });

    // std::string contents_type = "equipburn";
    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "contentslist"_n,
    //        std::make_tuple(_user, contents_type, contents_list))
    //     .send();
}

// ACTION battletest::itemburn(eosio::name _user, const std::vector<uint64_t> &_item_list, const std::vector<uint64_t> &_count_list)
// {
//     system_check(_user);

//     user_auths user_auth_table(_self, _self.value);
//     auto user_auth_iter = user_auth_table.find(_user.value);
    // user_logs user_logs_table(_self, _self.value);
    // auto user_logs_iter = user_logs_table.find(_user.value);
//     user_items user_item_table(_self, _user.value);

//     asset item_burn_result(0, symbol(symbol_code("UTG"), 4));

//     std::string contents_list;


//     uint64_t check_inventory = 0;
//         uint64_t check_count = 0;
//     for (uint32_t i = 0; i < _item_list.size(); ++i)
//     {
//         uint64_t UTG_Amount = 0;
//         item_info items;
//         auto user_item_iter = user_item_table.find(_item_list[i]);
//         eosio_assert(user_item_iter != user_item_table.end(), "itemburn : not exist item info");
//         eosio_assert(_item_list.size() == _count_list.size(), "itemburn : The lengths of the two do not match");

//         user_item_table.modify(user_item_iter, _self, [&](auto &change_user_item) {
//             uint64_t temp_count = _count_list[i];
//             uint64_t temp_sum;

//             for (uint64_t h = 0; h < change_user_item.item_list.size(); h++)
//             {
//                 temp_sum += change_user_item.item_list[h].count;
//             }
//             eosio_assert(temp_sum >= _count_list[i], "itemburn : not sell item count");

//             auto item_iter = change_user_item.item_list.end() - 1;
            
//             for (uint64_t k = change_user_item.item_list.size() - 1; k >= 0; k--)
//             {
//                 if (change_user_item.item_list[k].count < temp_count)
//                 {                    
//                     temp_count = (temp_count - change_user_item.item_list[k].count);
//                     change_user_item.item_list[k].count = 0;
//                     change_user_item.item_list.erase(item_iter);
//                     check_inventory +=1;
//                 }
//                 else
//                 {
//                     change_user_item.item_list[k].count -= temp_count;
//                     if (change_user_item.item_list[k].count == 0)
//                     {
//                         change_user_item.item_list.erase(item_iter);
//                         check_inventory +=1;
//                         item_iter--;
//                     }
//                     break;
//                 }
//             }
//             //UTG_Amount += temp_count;
//             check_count += temp_count;
//             // contents_list += to_string(user_item_iter->id) + ":";
//             // contents_list += to_string(user_item_iter->type) + ":";
//             // for(uint64_t i=0 ; i< change_user_item.item_list.size();i++)
//             // {
//             //     contents_list += to_string(user_item_iter->item_list[i].index) + ":";
//             //     contents_list += to_string(user_item_iter->item_list[i].count) +":";
//             // }
//             // contents_list += to_string(UTG_Amount)+",";

//         });
//         item_burn_result.amount += 10000 * _count_list[i];

//     }

//     action(permission_level{get_self(), "active"_n},
//            get_self(), "transfer"_n,
//            std::make_tuple(_self, _user, item_burn_result, std::string("item burn result")))
//         .send();

//     user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
        // eosio_assert(change_auth_user.current_item_inventory >= _list.size(),"itemburn : The current number of item and the number of units to sell are not correct.");
        // if (change_auth_user.current_item_inventory - _count_list.size() <= 0)
//         {
//             change_auth_user.current_item_inventory = 0;
//         }
//         else
//         {
//             change_auth_user.current_item_inventory -= check_inventory; 
//         }
//     });

    // user_logs_table.modify(user_logs_iter, _self, [&](auto &change_log) {
    //     change_log.get_utg += item_burn_result.amount;
    // });

//     // std::string contents_type = "itemburn";  
//     // action(permission_level{get_self(), "active"_n},
//     //        get_self(), "contentslist"_n,
//     //        std::make_tuple(_user, contents_type, contents_list))
//     //     .send();
// }
#pragma endregion

#pragma region item equipment function
//------------------------------------------------------------------------//
//-------------------------item_equipment_function------------------------//
//------------------------------------------------------------------------//

ACTION battletest::equip(eosio::name _user, uint32_t _servant_index, uint32_t _item_index)
{
    system_check(_user);

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
    //eosio_assert(user_equip_item_iter->equipment.state != object_state::on_equip_slot, "equip : already equip this item");

    eosio_assert(compare_item(servant_db_iter->job, user_equip_item_iter->equipment.job), "equip : this item is not enough job equipment"); 

    eosio_assert(equipment_db_iter->type == user_equip_item_iter->equipment.type, "equip : this item is not same type");
    uint32_t slot;
    slot = user_equip_item_iter->equipment.type;

    if ((user_servant_iter->servant.state == object_state::on_inventory) || user_servant_iter->servant.state == object_state::on_party) // && 
    //user_equip_item_iter->equipment.state == object_state::on_inventory)  
    {
        if (user_equip_item_iter->equipment.tier == 1)
        {
            if (user_servant_iter->servant.equip_slot[slot] != 0) 
            {
                if(user_servant_iter->servant.equip_slot[slot] == user_equip_item_iter->index)
                {
                    user_equip_item_table.modify(user_equip_item_iter, _self, [&](auto &unequip_item) {
                        unequip_item.equipment.state = object_state::on_inventory;
                        unequip_item.equipment.equipservantindex = 0;
                    });

                    user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant) {
                        unequip_servant.servant.equip_slot[slot] = 0;
                    });
                }                
                else
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
            if (user_servant_iter->servant.level >= ((user_equip_item_iter->equipment.tier * 10) + 1) -10)
            {
                if (user_servant_iter->servant.equip_slot[slot] != 0)
                {
                    if (user_servant_iter->servant.equip_slot[slot] == user_equip_item_iter->index)
                    {
                        user_equip_item_table.modify(user_equip_item_iter, _self, [&](auto &unequip_item) {
                            unequip_item.equipment.state = object_state::on_inventory;
                            unequip_item.equipment.equipservantindex = 0;
                        });

                        user_servant_table.modify(user_servant_iter, _self, [&](auto &unequip_servant) {
                            unequip_servant.servant.equip_slot[slot] = 0;
                        });
                    }
                    else
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
            else 
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

    // std::string contents_result;
    // std::string contents_type = "equip";
    // // contents_result += to_string(user_servant_iter->index) + ":";
    // // contents_result += to_string(user_servant_iter->servant.id) + ":";
    // // contents_result += to_string(user_servant_iter->servant.equip_slot[slot]) + ":";
    // // contents_result += to_string(user_equip_item_iter->index) + ":";
    // // contents_result += to_string(user_equip_item_iter->equipment.id) + ":";
    // // contents_result += to_string(user_equip_item_iter->equipment.state) + ":";
    // // contents_result += to_string(user_equip_item_iter->equipment.equipservantindex) + ",";

    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "contents"_n,
    //        std::make_tuple(_user, contents_type, contents_result))
    //     .send();    
    
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
    system_check(_user);

    uint64_t equipment_type, equipment_grade, equipment_upgrade, db_ratio;

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);

    user_logs user_logs_table(_self, _self.value);
    auto user_logs_iter = user_logs_table.find(_user.value);

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
    uint64_t check_total_item = 0;
    std::string contents_result;
    std::string contents_type = "equipmentup";
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
            if (db_ratio >= upgrade_ratio) 
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

        if (updatecheck == 0)
        {
            user_equip_item_table.erase(user_equipment_iter);
            user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
                eosio_assert(change_auth_user.current_item_inventory >= _item_list.size(),"equipmentup : The current number of item and the number of units to sell are not correct.");
                if(change_auth_user.current_item_inventory - _item_list.size() <= 0)
                {
                    change_auth_user.current_item_inventory = 0;
                }
                else
                {
                    change_auth_user.current_item_inventory -= check_inventory;
                }
                if(change_auth_user.current_equipment_inventory <=0)
                {
                    change_auth_user.current_equipment_inventory = 0;
                }
                else
                {
                    change_auth_user.current_equipment_inventory -= 1;
                }                   
            });
        }

        contents_result += to_string(user_equipment_iter->index) + ":";
        contents_result += to_string(user_equipment_iter->equipment.id) + ":";
        contents_result += to_string(user_equipment_iter->equipment.upgrade) + ":";
        contents_result += to_string(user_item_iter->id) + ":";
        contents_result += to_string(user_item_iter->type) + ":";
        for (uint64_t i = 0; i < user_item_iter->item_list.size(); i++)
        {
            contents_result += to_string(user_item_iter->item_list[i].index) + ":";
            contents_result += to_string(user_item_iter->item_list[i].count) + ":";
        }
        contents_result += to_string( user_upgrade_equipment_iter->use_UTG[equipment_upgrade]) +",";
    }

    asset upgrade_use_UTG_result(0, symbol(symbol_code("UTG"), 4));
    upgrade_use_UTG_result.amount = user_upgrade_equipment_iter->use_UTG[equipment_upgrade] * 10000; 

    transfer(_user, _self, upgrade_use_UTG_result, std::string("upgrade use UTG result"));

    user_logs_table.modify(user_logs_iter, _self, [&](auto &change_log) {
        change_log.use_utg += upgrade_use_UTG_result.amount;
    });

    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "contents"_n,
    //        std::make_tuple(_user, contents_type, contents_result))
    //     .send();  


}

ACTION battletest::monsterup(eosio::name _user, uint32_t _monster, uint32_t _monster2)
{
    system_check(_user);
    
    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);

    user_logs user_logs_table(_self, _self.value);
    auto user_logs_iter = user_logs_table.find(_user.value);

    user_monsters user_monster_table(_self, _user.value);
    auto user_monster_iter = user_monster_table.find(_monster);

    user_monsters second_user_monster_table(_self, _user.value);
    auto user_monster_iter2 = second_user_monster_table.find(_monster2);
    asset nomal_order_buy_result(0, symbol(symbol_code("UTG"), 4));

    uint64_t db_ratio = 0;
    uint64_t find_sum_upgrade_iter = user_monster_iter->monster.grade * 10 + user_monster_iter->monster.upgrade;
    //uint64_t find_sum_lv_iter = user_monster_iter->monster.tribe * 10 + user_monster_iter->monster.grade;

    upgrade_monster_ratio_db upgrade_monsterratio_db_table(_self, _self.value);
    auto user_upgrade_monster_iter = upgrade_monsterratio_db_table.find(find_sum_upgrade_iter);

    db_ratio = user_upgrade_monster_iter->sub[user_monster_iter2->monster.upgrade].ratio;

    eosio_assert(user_monster_iter != user_monster_table.end(), "monsterup : not exist monster info");
    eosio_assert(user_monster_iter2 != second_user_monster_table.end(), "monsterup : not exist monster info");

    eosio_assert((user_monster_iter->party_number == 0 || user_monster_iter2->party_number == 0), "monsterup : this monster already in party");
    eosio_assert((user_monster_iter->monster.upgrade != 9 || user_monster_iter2->monster.upgrade != 9), "monsterup : this monster upgrade is MAX");
    eosio_assert(user_monster_iter->monster.id == user_monster_iter2->monster.id, "monsterup : same monster not use");

    uint64_t l_seed = safeseed::get_seed_value(_user.value, now());
    uint64_t upgrade_ratio = safeseed::get_random_value(l_seed, MONSTER_UPGRADE_RATE, DEFAULT_MIN, DEFAULT_RANDOM_COUNT);
    uint64_t fail_upgrade = safeseed::get_random_value(l_seed, user_monster_iter->monster.upgrade + 1, DEFAULT_MIN, DEFAULT_RANDOM_COUNT);

    eosio_assert(user_monster_iter->monster.grade == user_monster_iter2->monster.grade, "monsterup : sub monster grade is high by main monster");       
    eosio_assert(user_monster_iter->monster.upgrade >= user_monster_iter2->monster.upgrade, "monsterup : sub monster upgrade is high by main monster"); 

    if (db_ratio >= upgrade_ratio)
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
            change_auth_user.current_monster_inventory -= 1;
        });
    }
    std::string contents_result;
    std::string contents_type = "monsterup";

    // contents_result += to_string(user_monster_iter->index) + ":";
    // contents_result += to_string(user_monster_iter->monster.id) + ":";
    // contents_result += to_string(user_monster_iter->monster.upgrade) + ":";
    // contents_result += to_string(user_monster_iter2->index) + ":";
    // contents_result += to_string(user_monster_iter2->monster.id) + ":";
    // contents_result += to_string(user_upgrade_monster_iter->use_UTG) + ",";
    action(permission_level{get_self(), "active"_n},
           get_self(), "contents"_n,
           std::make_tuple(_user, contents_type, contents_result))
        .send();

    second_user_monster_table.erase(user_monster_iter2);

    nomal_order_buy_result.amount = user_upgrade_monster_iter->use_UTG * 10000;
    transfer(_user, _self, nomal_order_buy_result, std::string("nomal order buy result"));

    user_logs_table.modify(user_logs_iter, _self, [&](auto &change_log) {
        change_log.use_utg += nomal_order_buy_result.amount;
    });

}

#pragma endregion



ACTION battletest::pvpstart(eosio::name _from, eosio::name _to)
{
    system_check(_from);

    eosio_assert(_from != _to, "PVP Start : Self PVP Impossible");
    std::vector<uint32_t> servant_pos_list = {0, 1, 2, 3, 4};
    std::vector<uint32_t> monster_pos_list = {5, 6, 7, 8, 9};

 
    user_auths user_auth(_self, _self.value);
    auto from_user_iter = user_auth.find(_from.value);

    eosio_assert(from_user_iter != user_auth.end(), "PVP Start : Empty Auth Table / Not Yet Signup");
    eosio_assert(from_user_iter->state == user_state::lobby, "PVP Start : Already Battle");

    auto to_user_iter = user_auth.find(_to.value);

    eosio_assert(to_user_iter != user_auth.end(), "PVP Start : Empty Auth Table / Not Yet Signup");

    user_auth.modify(from_user_iter, _self, [&](auto &data) {
        data.state = user_state::pvp;
    });

   
    user_partys from_party(_self, _from.value);
    uint32_t from_party_num = 1;
    auto from_party_iter = from_party.find(from_party_num);
    eosio_assert(from_party_iter != from_party.end(), "PVP Start : Empty Party Table / Not Yet Signup");
        eosio_assert(from_party_iter->state == party_state::on_wait, "PVP Start : Party State Wrong");
    eosio_assert(true == possible_start(_from, from_party_num), "PVP Start : Empty Your Party");

    user_partys to_party(_self, _to.value);
    uint32_t to_party_num = 1;
    auto to_party_iter = to_party.find(to_party_num);
    eosio_assert(to_party_iter != to_party.end(), "PVP Start : Empty Party Table / Not Yet Signup");
    eosio_assert(true == possible_start(_to, to_party_num), "PVP Start : Empty Enemy Party");


    std::string stage_info;
    std::vector<std::string> from_state;
    std::vector<std::string> to_state;

    battle_state_list pvp_table(_self, _self.value);
    auto pvp_iter = pvp_table.find(_from.value);

    if (pvp_iter == pvp_table.end())
    {
        pvp_table.emplace(_self, [&](auto &data) {
            data.user = _from;
            data.stage_type = 0;
            data.enemy_user = _to;
            data.stage_number = 1;
            data.turn = 0;

            stage_info += _from.to_string() + ":";
            stage_info += to_string(data.stage_type) + ":";
            stage_info += _to.to_string() + ":";
            stage_info += to_string(data.stage_number) + ":";
            stage_info += to_string(data.turn) + ":";


            for (uint32_t i = 0; i < 5; ++i)
            {
                if (from_party_iter->servant_list[i] != 0)
                {
                    character_state_data from_servant_battle_state = get_user_state(_from, "ser", from_party_iter->servant_list[i], servant_pos_list[i], from_state);
                    data.my_state_list.push_back(from_servant_battle_state);
                }
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (from_party_iter->monster_list[i] != 0)
                {
                    character_state_data from_monster_battle_state = get_user_state(_from, "mon", from_party_iter->monster_list[i], monster_pos_list[i], from_state);
                    data.my_state_list.push_back(from_monster_battle_state);
                }
            }


            for (uint32_t i = 0; i < 5; ++i)
            {
                if (to_party_iter->servant_list[i] != 0)
                {
                    character_state_data to_servant_battle_state = get_user_state(_to, "ser", to_party_iter->servant_list[i], servant_pos_list[i] + 10, to_state);
                    data.enemy_state_list.push_back(to_servant_battle_state);
                }
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (to_party_iter->monster_list[i] != 0)
                {
                    character_state_data to_monster_battle_state = get_user_state(_to, "mon", to_party_iter->monster_list[i], monster_pos_list[i] + 10, to_state);
                    data.enemy_state_list.push_back(to_monster_battle_state);
                }
            }
            set_synergy(data.my_state_list, data.my_synergy_list);
           set_hp_synergy(data.my_state_list, data.my_synergy_list);

           set_synergy(data.enemy_state_list, data.enemy_synergy_list);
           set_hp_synergy(data.enemy_state_list, data.enemy_synergy_list);
        });
    }
    else
    {
        pvp_table.modify(pvp_iter, _self, [&](auto &data) {
            data.stage_type = 0;
            data.enemy_user = _to;
            data.stage_number = 1;
            data.turn = 0;
            data.my_state_list.clear();
            data.enemy_state_list.clear();
            data.my_synergy_list.clear();
            data.enemy_synergy_list.clear();

            stage_info += _from.to_string() + ":";
            stage_info += to_string(data.stage_type) + ":";
            stage_info += _to.to_string() + ":";
            stage_info += to_string(data.stage_number) + ":";
            stage_info += to_string(data.turn) + ":";


            for (uint32_t i = 0; i < 5; ++i)
            {
                if (from_party_iter->servant_list[i] != 0)
                {
                    character_state_data from_servant_battle_state = get_user_state(_from, "ser", from_party_iter->servant_list[i], servant_pos_list[i], from_state);
                    data.my_state_list.push_back(from_servant_battle_state);
                }
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (from_party_iter->monster_list[i] != 0)
                {
                    character_state_data from_monster_battle_state = get_user_state(_from, "mon", from_party_iter->monster_list[i], monster_pos_list[i], from_state);
                    data.my_state_list.push_back(from_monster_battle_state);
                }
            }


            for (uint32_t i = 0; i < 5; ++i)
            {
                if (to_party_iter->servant_list[i] != 0)
                {
                    character_state_data to_servant_battle_state = get_user_state(_to, "ser", to_party_iter->servant_list[i], servant_pos_list[i] + 10, to_state);
                    data.enemy_state_list.push_back(to_servant_battle_state);
                }
            }
            for (uint32_t i = 0; i < 5; ++i)
            {
                if (to_party_iter->monster_list[i] != 0)
                {
                    character_state_data to_monster_battle_state = get_user_state(_to, "mon", to_party_iter->monster_list[i], monster_pos_list[i] + 10, to_state);
                    data.enemy_state_list.push_back(to_monster_battle_state);
                }
            }
            set_synergy(data.my_state_list, data.my_synergy_list);
           set_hp_synergy(data.my_state_list, data.my_synergy_list);

           set_synergy(data.enemy_state_list, data.enemy_synergy_list);
           set_hp_synergy(data.enemy_state_list, data.enemy_synergy_list);
        });
    }

    battle_actions from_battle_action_table(_self, _self.value);
    auto from_battle_action_iter = from_battle_action_table.find(_from.value);
    if (from_battle_action_iter == from_battle_action_table.end())
    {
        from_battle_action_table.emplace(_self, [&](auto &data) {
            data.user = _from;
            data.turn = START_BATTLE;
            data.character_action_list.clear();
        });
    }
    else
    {
        from_battle_action_table.modify(from_battle_action_iter, _self, [&](auto &data) {
            data.turn = START_BATTLE;
            data.character_action_list.clear();
        });
    }

    // action(permission_level(get_self(), "active"_n),
    //        get_self(), "battlestate"_n,
    //        std::make_tuple(_from, stage_info, from_state, to_state))
    //     .send();
}


#pragma region store function
//------------------------------------------------------------------------//
//-------------------------------store_function---------------------------//
//------------------------------------------------------------------------//

ACTION battletest::itembuy(eosio::name _user, uint32_t _item_id, uint32_t _count)
{
    system_check(_user);

    shop_list shop_list_table(_self, _self.value);
    auto shop_list_iter = shop_list_table.find(_item_id);
    eosio_assert(shop_list_iter != shop_list_table.end(), "itembuy : Not exist item shop data");

    item_shop item_shop_table(_self, _self.value);
    auto item_shop_iter = item_shop_table.find(shop_list_iter->shop_item_id);
    eosio_assert(item_shop_iter != item_shop_table.end(), "itembuy : Not exist item_shop data");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "itembuy : Not exist user_auths data");

    user_items user_items_table(_self, _user.value);
    auto user_items_iter = user_items_table.find(item_shop_iter->product_id);

    item_info items;
    uint64_t count_diff = _count;
    uint64_t check_inventory = 0;

    std::string contents_result;
    std::string contents_type = "itembuy";
    uint64_t UTG_Amount = 0;

 if (shop_list_iter->shop_type == 3)
    {
    }

    else if (shop_list_iter->shop_type == 2)
    {
        if (user_items_iter == user_items_table.end())
        {
            user_items_table.emplace(_self, [&](auto &change_consumable) {
                change_consumable.id = item_shop_iter->product_id;
                change_consumable.type = item_shop_iter->shop_type;

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
                change_consumable.type = item_shop_iter->shop_type;

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
                    change_consumable.item_list.push_back(items);
                    check_inventory += 1;
                }
            });
        }
        eosio_assert(user_auth_iter->current_item_inventory >= 0, "itembuy : current_item_inventory underflow error");
        user_auth_table.modify(user_auth_iter, _self, [&](auto &add_auth) {
            add_auth.current_item_inventory += check_inventory;
        });

        asset nomal_order_buy_result(0, symbol(symbol_code("UTG"), 4));
        nomal_order_buy_result.amount = _count * item_shop_iter->price_count;

        transfer(_user, _self, nomal_order_buy_result, std::string("nomal order buy result"));

        // UTG_Amount = _count * shop_list_iter->price_count;
        // contents_result += to_string(user_items_iter->id) + ":";
        // contents_result += to_string(user_items_iter->type) + ":";
        // for (uint64_t i = 0; i < user_items_iter->item_list.size(); i++)
        // {
        //     contents_result += to_string(user_items_iter->item_list[i].index) + ":";
        //     contents_result += to_string(user_items_iter->item_list[i].count) + ":";
        //     }
        //     contents_result += to_string(UTG_Amount) + ",";

        // action(permission_level{get_self(), "active"_n},
        //        get_self(), "contents"_n,
        //        std::make_tuple(_user, contents_type, contents_result))
        //     .send();
    }
    else
    {
        eosio_assert(0 == 1, "itembuy : Invalid purchase request");
    }
}

void battletest::inventory_buy(eosio::name _user, uint32_t _type, uint32_t _count)
{
    system_check(_user);

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    uint64_t plus_inventory = 5;

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "inventory_buy : Empty Log Table / Not Yet Signup");
   
    user_log_table.modify(user_log_iter, _self, [&](auto &new_log) {
        new_log.use_eos += 1000 * _count;
    });

    shop_list shop_list_table(_self, _self.value);
    auto shop_list_iter = shop_list_table.find(_type);
    eosio_assert(shop_list_iter != shop_list_table.end(), "inventory_buy : Not exist item shop data");

    item_shop item_shop_table(_self, _self.value);
    auto item_shop_iter = item_shop_table.find(shop_list_iter->shop_item_id);
    eosio_assert(item_shop_iter != item_shop_table.end(), "inventory_buy : Not exist item_shop data");

    if (shop_list_iter->shop_item_id == 2001 && _type == 1)
    {
        eosio_assert((user_auth_iter->servant_inventory + (plus_inventory * _count)) < 200, "inventroy_buy : Max inventory is 200");

        user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.servant_inventory += (plus_inventory * _count);
        });
    }
    else if (shop_list_iter->shop_item_id == 2002 && _type == 2)
    {
        eosio_assert((user_auth_iter->monster_inventory + (plus_inventory * _count)) < 200, "inventroy_buy : Max inventory is 200");
        user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.monster_inventory += (plus_inventory * _count);
        });
    }
    else if (shop_list_iter->shop_item_id == 2003 && _type == 3)
    {
        eosio_assert((user_auth_iter->equipment_inventory + (plus_inventory * _count)) < 200, "inventroy_buy : Max inventory is 200");
        user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.equipment_inventory += (plus_inventory * _count);
        });
    }
    else if (shop_list_iter->shop_item_id == 2004 && _type == 4)
    {
        eosio_assert((user_auth_iter->item_inventory + (plus_inventory * _count)) < 200, "inventroy_buy : Max inventory is 200");
        user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
            change_auth_user.item_inventory += (plus_inventory * _count);
        });
    }
    else
    {
        eosio_assert(0 == 1, "inventory_buy : not exsit this action type");
    }

    std::string contents_result;
    std::string contents_type = "inventorybuy";
    contents_result += to_string(_type) + ":";
    contents_result += to_string(plus_inventory);
    contents_result += to_string(_count);
    action(permission_level{get_self(), "active"_n},
           get_self(), "contents"_n,
           std::make_tuple(_user, contents_type, contents_result))
        .send();

}

void battletest::nftexchange(eosio::name _owner, eosio::name _master, std::string _type, uint64_t _master_index)
{
    require_auth(_owner);
    std::string contents_list;
    if (_type == "servant")
    {
        if (_owner == _master)
        {
            user_servants user_servant_table(_self, _owner.value);
            auto servant_iter = user_servant_table.find(_master_index);
            eosio_assert(servant_iter != user_servant_table.end(), "nftexchange : Not Exist Servant");
            user_servant_table.modify(servant_iter, _self, [&](auto &new_servant) {
                new_servant.servant.state = object_state::on_inventory;

            // contents_list += "[" + to_string(new_servant.index) + ":";
            // contents_list += to_string(new_servant.party_number) + ":";
            // contents_list += to_string(new_servant.servant.state) + ":";
            // contents_list += to_string(new_servant.servant.exp) + ":";
            // contents_list += to_string(new_servant.servant.id) + ":";
            // contents_list += to_string(new_servant.servant.grade) + ":";
            // contents_list += to_string(new_servant.servant.status.basic_str) + ":";
            // contents_list += to_string(new_servant.servant.status.basic_dex) + ":";
            // contents_list += to_string(new_servant.servant.status.basic_int) + ":";
            // for (uint64_t j = 0; j < new_servant.servant.equip_slot.size(); j++)
            // {
            //     contents_list += to_string(new_servant.servant.equip_slot[j]) + ":";
            // }
            // for (uint64_t j = 0; j < new_servant.servant.active_skill.size(); j++)
            // {
            //     contents_list += to_string(new_servant.servant.active_skill[j]) + ":";
            // }
            // for (uint64_t j = 0; j < new_servant.servant.passive_skill.size(); j++)
            // {
            //     contents_list += to_string(new_servant.servant.passive_skill[j]) + "]";
            // }
            });
        }
        else
        {
            user_servants master_table(_self, _master.value);
            auto master_iter = master_table.find(_master_index);
            eosio_assert(master_iter != master_table.end(), "nftexchange : Wrong Master Index 1");

            user_servants owner_table(_self, _owner.value);
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
                new_servant.party_number = 0;
                new_servant.servant = master_iter->servant;
                new_servant.servant.state = object_state::on_inventory;

            // contents_list += "[" + to_string(new_servant.index) + ":";
            // contents_list += to_string(new_servant.party_number) + ":";
            // contents_list += to_string(new_servant.servant.state) + ":";
            // contents_list += to_string(new_servant.servant.exp) + ":";
            // contents_list += to_string(new_servant.servant.id) + ":";
            // contents_list += to_string(new_servant.servant.grade) + ":";
            // contents_list += to_string(new_servant.servant.status.basic_str) + ":";
            // contents_list += to_string(new_servant.servant.status.basic_dex) + ":";
            // contents_list += to_string(new_servant.servant.status.basic_int) + ":";
            // for (uint64_t j = 0; j < new_servant.servant.equip_slot.size(); j++)
            // {
            //     contents_list += to_string(new_servant.servant.equip_slot[j]) + ":";
            // }
            // for (uint64_t j = 0; j < new_servant.servant.active_skill.size(); j++)
            // {
            //     contents_list += to_string(new_servant.servant.active_skill[j]) + ":";
            // }
            // for (uint64_t j = 0; j < new_servant.servant.passive_skill.size(); j++)
            // {
            //     contents_list += to_string(new_servant.servant.passive_skill[j]) + "]";
            // }

            });
            user_auths user_auths_table(_self,_self.value);
            auto user_auths_iter = user_auths_table.find(_master.value);
            user_auths_table.modify(user_auths_iter, _self, [&](auto &delete_index){
                if(delete_index.current_servant_inventory <= 0)
                {
                    delete_index.current_servant_inventory = 0;
                }
                else
                {
                    delete_index.current_servant_inventory -= 1;
                }              

            });
            master_table.erase(master_iter);
        }
       
    }
    else if(_type == "monster")
    {
        if (_owner == _master)
        {
            user_monsters user_monster_table(_self, _owner.value);
            auto monster_iter = user_monster_table.find(_master_index);
            eosio_assert(monster_iter != user_monster_table.end(), "nftexchange : Not Exist Monster");
            user_monster_table.modify(monster_iter, _self, [&](auto &new_monster) {
                new_monster.monster.state = object_state::on_inventory;

                // contents_list += "[" + to_string(new_monster.index) + ":";
                // contents_list += to_string(new_monster.party_number) + ":";
                // contents_list += to_string(new_monster.monster.state) + ":";
                // contents_list += to_string(new_monster.monster.exp) + ":";
                // contents_list += to_string(new_monster.monster.id) + ":";
                // contents_list += to_string(new_monster.monster.type) + ":";
                // contents_list += to_string(new_monster.monster.tribe) + ":";
                // contents_list += to_string(new_monster.monster.grade) + ":";
                // contents_list += to_string(new_monster.monster.upgrade) + ":";
                // contents_list += to_string(new_monster.monster.status.basic_str) + ":";
                // contents_list += to_string(new_monster.monster.status.basic_dex) + ":";
                // contents_list += to_string(new_monster.monster.status.basic_int) + ":";
                // for (uint64_t j = 0; j < new_monster.monster.active_skill.size(); j++)
                // {
                //     contents_list += to_string(new_monster.monster.active_skill[j]) + ":";
                // }
                // for (uint64_t j = 0; j < new_monster.monster.passive_skill.size(); j++)
                // {
                //     contents_list += to_string(new_monster.monster.passive_skill[j]) + "]";
                // }
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

                // contents_list += "[" + to_string(new_monster.index) + ":";
                // contents_list += to_string(new_monster.party_number) + ":";
                // contents_list += to_string(new_monster.monster.state) + ":";
                // contents_list += to_string(new_monster.monster.exp) + ":";
                // contents_list += to_string(new_monster.monster.id) + ":";
                // contents_list += to_string(new_monster.monster.type) + ":";
                // contents_list += to_string(new_monster.monster.tribe) + ":";
                // contents_list += to_string(new_monster.monster.grade) + ":";
                // contents_list += to_string(new_monster.monster.upgrade) + ":";
                // contents_list += to_string(new_monster.monster.status.basic_str) + ":";
                // contents_list += to_string(new_monster.monster.status.basic_dex) + ":";
                // contents_list += to_string(new_monster.monster.status.basic_int) + ":";
                // for (uint64_t j = 0; j < new_monster.monster.active_skill.size(); j++)
                // {
                //     contents_list += to_string(new_monster.monster.active_skill[j]) + ":";
                // }
                // for (uint64_t j = 0; j < new_monster.monster.passive_skill.size(); j++)
                // {
                //     contents_list += to_string(new_monster.monster.passive_skill[j]) + "]";
                // }

            });

            user_auths user_auths_table(_self, _self.value);
            auto user_auths_iter = user_auths_table.find(_master.value);
            user_auths_table.modify(user_auths_iter, _self, [&](auto &delete_index) {
                if(delete_index.current_monster_inventory <= 0 )
                {
                    delete_index.current_monster_inventory = 0;
                }
                else
                {
                    delete_index.current_monster_inventory -= 1;
                }                

            });
            master_table.erase(master_iter);
        }
    }
    else if(_type == "equipment")
    {
        if (_owner == _master)
        {
            user_equip_items user_equipment_table(_self, _owner.value);
            auto equipment_iter = user_equipment_table.find(_master_index);
            eosio_assert(equipment_iter != user_equipment_table.end(), "nftexchange : Not Exist equipment");
            user_equipment_table.modify(equipment_iter, _self, [&](auto &new_equip) {
                new_equip.equipment.state = object_state::on_inventory;

                // contents_list += "[" + to_string(new_equip.index) + ":";
                // contents_list += to_string(new_equip.equipment.state) + ":";
                // contents_list += to_string(new_equip.equipment.id) + ":";
                // contents_list += to_string(new_equip.equipment.type) + ":";
                // contents_list += to_string(new_equip.equipment.tier) + ":";
                // contents_list += to_string(new_equip.equipment.job) + ":";
                // contents_list += to_string(new_equip.equipment.grade) + ":";
                // contents_list += to_string(new_equip.equipment.upgrade) + ":";
                // contents_list += to_string(new_equip.equipment.value) + "]";
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

                // contents_list += "[" + to_string(new_equip.index) + ":";
                // contents_list += to_string(new_equip.equipment.state) + ":";
                // contents_list += to_string(new_equip.equipment.id) + ":";
                // contents_list += to_string(new_equip.equipment.type) + ":";
                // contents_list += to_string(new_equip.equipment.tier) + ":";
                // contents_list += to_string(new_equip.equipment.job) + ":";
                // contents_list += to_string(new_equip.equipment.grade) + ":";
                // contents_list += to_string(new_equip.equipment.upgrade) + ":";
                // contents_list += to_string(new_equip.equipment.value) + "]";

            });
            user_auths user_auths_table(_self, _self.value);
            auto user_auths_iter = user_auths_table.find(_master.value);
            user_auths_table.modify(user_auths_iter, _self, [&](auto &delete_index) {
                if(delete_index.current_equipment_inventory <= 0 )
                {
                    delete_index.current_equipment_inventory = 0;
                }
                else
                {
                    delete_index.current_equipment_inventory -= 1;
                }                

            });
            master_table.erase(master_iter);
        }
    }
    else
    {
        eosio_assert(1 == 0 ,"nftexchange : Wrong Type Token");
    }
    // std::string contents_type = "nftmailopen";
    // action(permission_level{get_self(), "active"_n},
    //        get_self(), "contentslist"_n,
    //        std::make_tuple(_owner, contents_type, contents_list))
    //     .send();
}

// ACTION battletest::chat(name _user, asset _price, string _text)
// {
//    require_auth(_user);

//    eosio_assert(_price.symbol == symbol("UTG", 4), "Only UTG can be uses.");

//    // Get User's UTG balance
//    account acnt_table(_self, _user.value);
//    auto a_iter = acnt_table.find(_price.symbol.code().raw());

//    // Get Chat table info
//    chat_index chat_table(_self, _self.value);
//    auto iter = chat_table.find(_self.value);

//    // Set basic price
//    string _symbol = "UTG";
//    asset basic_price(1000000, symbol(symbol_code(_symbol), 4));

//    if(iter == chat_table.end()){
//        eosio_assert(a_iter->balance.amount >= basic_price.amount, "Not enough UTG.");
//        eosio_assert(basic_price.amount == _price.amount, "Can not register at current price.");

//        chat_table.emplace(_user, [&](auto &data){
//            data.owner = _self;
//            data.price = basic_price;
//            data.user = _user;
//            data.text = _text;
//            data.start_time = now();
//        });
//    }
//    else{
//        if(iter->start_time < now() - 86400){
//            eosio_assert(a_iter->balance.amount >= basic_price.amount, "Not enough UTG.");
//            eosio_assert(basic_price.amount == _price.amount, "Can not register at current price.");

//            chat_table.modify(iter, _user, [&](auto &data){
//                data.price = basic_price;
//                data.user = _user;
//                data.text = _text;
//                data.start_time = now();
//            });
//        }
//        else{
//            eosio_assert(a_iter->balance.amount >= _price.amount, "Not enough UTG.");
//            basic_price.amount = iter->price.amount * 2;
//            eosio_assert(basic_price.amount == _price.amount, "Can not register at current price.");

//            chat_table.modify(iter, _user, [&](auto &data){
//                data.price = _price;
//                data.user = _user;
//                data.text = _text;
//                data.start_time = now();
//            });
//        }
//    }

//    string msg = "Regist chat message. " + std::to_string(_price.amount) + " UTG";
//    transfer(_user, _self, _price, msg);
// }
 
ACTION battletest::addrefer(eosio::name _referer)
{
    master_active_check();

    user_auths user_auth_table(_self, _self.value);
    auto iter = user_auth_table.find(_referer.value);
    eosio_assert(iter != user_auth_table.end(),"Add Refer : Not Yet Signup");

    referlist referlist_tabe(_self, _self.value);
    auto referlist_iter = referlist_tabe.find(_referer.value);
    eosio_assert(referlist_iter == referlist_tabe.end(), "Add Refer : Already Exist");
    referlist_tabe.emplace(_self, [&](auto &new_refer) {
        new_refer.referer = _referer;
    });
}
    
ACTION battletest::deleterefer(eosio::name _referer)
{
    master_active_check();

    referlist referlist_tabe(_self, _self.value);
    auto referlist_iter = referlist_tabe.find(_referer.value);
    eosio_assert(referlist_iter != referlist_tabe.end(), "Delete Refer : Not Exist");
    referlist_tabe.erase(referlist_iter);
}


// ACTION battletest::addwhite(eosio::name _user)
// {
//     master_active_check();

//     whitelist referlist_tabe(_self, _self.value);
//     auto referlist_iter = referlist_tabe.find(_user.value);
//     eosio_assert(referlist_iter == referlist_tabe.end(), "Add White : Already Exist");
//     referlist_tabe.emplace(_self, [&](auto &new_refer) {
//         new_refer.user = _user;
//     });
// }
    
// ACTION battletest::deletewhite(eosio::name _user)
// {
//     master_active_check();

//     whitelist referlist_tabe(_self, _self.value);
//     auto referlist_iter = referlist_tabe.find(_user.value);
//     eosio_assert(referlist_iter != referlist_tabe.end(), "Delete White : Not Exist");
//     referlist_tabe.erase(referlist_iter);
// }

// ACTION battletest::anothercheck()
// {
//     require_auth(_self);

//     user_logs user_logs_table(_self, _self.value);

//     for(auto iter = user_logs_table.begin(); iter != user_logs_table.end();)
//     {
//         auto iter2 = user_logs_table.find(iter->primary_key());

//         user_mail user_mail_table(_self, iter2->user.value);
//         uint64_t count = 0;
//         for(auto mail = user_mail_table.begin(); mail != user_mail_table.end();)
//         {
//             count++;
//             mail++;
//         }
//         user_logs_table.modify(iter2, _self, [&](auto &new_data) {
//             new_data.mail = count;
//         });
//         iter++;
//     }
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


EOSIO_DISPATCH(battletest,
                (addrefer)(deleterefer)//(addwhite)(deletewhite)
                //(setdata)                                                                                                                     
               (transfer)(changetoken)(create)(issue)                                                                                                                        
               //(chat)                                                                                                                              
               (setmaster)(eostransfer)(deleteblack)(addblack)(setpause)//(dbinsert)(dberase)(dblistinsert)(insertequipr)(dbinit)(initmaster)
               (stageexit)(stagestart)(activeturn)(pvpstart)//(battlestate)(battleaction)(resultparty)
               (saveparty)//(resultgacha)(itemburn)                                                                                                                         
               (itembuy)(monsterup)(equipmentup)(mailopen)(equip)(nftmail)//(anothercheck)
               (burn))
