#pragma region seed check

// void battletest::set_seed(std::string _type, uint64_t _seed, uint64_t _result)
// {
//     seed_log seed_log_table(_self, _self.value);
//     seed_info data;
//     data.type = _type;
//     data.seed = _seed;
//     data.result = _result;
//     seed_log_table.emplace(_self, [&](auto &new_data)
//     {
//         new_data.index = seed_log_table.available_primary_key();
//         new_data.value = data;
//     });

// }
#pragma endregion


#pragma region recipient
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
#pragma endregion


#pragma region party
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
#pragma endregion


#pragma region delete funcition
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

    new_battle_state_list batlle_state_list_table(_self, _self.value);
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

void battletest::deleteuser(eosio::name _user)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    user_auths user_auths_table(_self, _self.value);
    auto user_auths_iter = user_auths_table.find(_user.value);
    if (user_auths_iter != user_auths_table.end())
    {
        user_auths_table.erase(user_auths_iter);
    }

    user_logs user_logs_table(_self, _self.value);
    auto user_logs_iter = user_logs_table.find(_user.value);
    if (user_logs_iter != user_logs_table.end())
    {
        user_logs_table.erase(user_logs_iter);
    }

    gold_logs user_gold_log_table(_self, _self.value);
    auto gold_iter = user_gold_log_table.find(_user.value);
    if (gold_iter != user_gold_log_table.end())
    {
        user_gold_log_table.erase(gold_iter);
    }

    gold_gacha_results user_gold_result_table(_self, _self.value);
    auto gold_result_iter = user_gold_result_table.find(_user.value);
    if (gold_result_iter != user_gold_result_table.end())
    {
        user_gold_result_table.erase(gold_result_iter);
    }

    gold_gacha_totals user_gold_total_table(_self, _self.value);
    auto gold_result_total_iter = user_gold_total_table.find(_user.value);
    if (gold_result_total_iter != user_gold_total_table.end())
    {
        user_gold_total_table.erase(gold_result_total_iter);
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
            item++;
            user_preregist_item_table.erase(iter);
        }
    }

    user_items user_consum_item_table(_self, _user.value);
    auto user_consum_item_iter = user_consum_item_table.begin();
    if (user_consum_item_iter != user_consum_item_table.end())
    {
        for (auto consumable = user_consum_item_table.begin(); consumable != user_consum_item_table.end();)
        {
            auto iter = user_consum_item_table.find(consumable->primary_key());
            consumable++;
            user_consum_item_table.erase(iter);
        }
    }

    user_monsters user_preregist_monster_table(_self, _user.value);
    auto mon_iter = user_preregist_monster_table.begin();
    if (mon_iter != user_preregist_monster_table.end())
    {
        for (auto mon = user_preregist_monster_table.begin(); mon != user_preregist_monster_table.end();)
        {
            auto iter = user_preregist_monster_table.find(mon->primary_key());
            mon++;
            user_preregist_monster_table.erase(iter);
        }
    }

    user_servants user_preregist_servant_table(_self, _user.value);
    auto ser_iter = user_preregist_servant_table.begin();
    if (ser_iter != user_preregist_servant_table.end())
    {
        for (auto ser = user_preregist_servant_table.begin(); ser != user_preregist_servant_table.end();)
        {
            auto iter = user_preregist_servant_table.find(ser->primary_key());
            ser++;
            user_preregist_servant_table.erase(iter);
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


    user_mail mail_db_table(_self, _user.value);
    for (auto item = mail_db_table.begin(); item != mail_db_table.end();)
    {
        auto iter = mail_db_table.find(item->primary_key());
        mail_db_table.erase(iter);
        item++;
    }
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// ACTION battletest::alluserdel()
// {
//     require_auth(_self);
//     user_auths user_auth_table(_self, _self.value);
//     for (auto iter = user_auth_table.begin(); iter != user_auth_table.end();)
//     {
//         auto iter_2 = user_auth_table.find(iter->primary_key());
//         //deletebattle(iter_2->user);
//         iter++;
//         deletebattle(iter_2->user);
//         deleteuser(iter_2->user);
//     }

//     // seed_log seed_log_table(_self, _self.value);
//     // for (auto see = seed_log_table.begin(); see != seed_log_table.end();)
//     // {
//     //     auto iter_2 = seed_log_table.find(see->primary_key());
//     //     see = seed_log_table.erase(iter_2);
//     // }
// }

// ACTION battletest::allbattle()
// {
//     require_auth(_self);
//     user_auths user_auth_table(_self, _self.value);
//     for (auto iter = user_auth_table.begin(); iter != user_auth_table.end();)
//     {
//         auto iter_2 = user_auth_table.find(iter->primary_key());
//         //deletebattle(iter_2->user);
//         iter++;
//         deletebattle(iter_2->user);
//         //deleteuser(iter_2->user);
//     }
// }

// ACTION battletest::deleteuser2(eosio::name _user)
// {
//     system_master system_master_table(_self, _self.value);
//     auto system_master_iter = system_master_table.begin();

//     permission_level master_auth;
//     master_auth.actor = system_master_iter->master;
//     master_auth.permission = "active"_n;
//     require_auth(master_auth);

//     user_auths user_auths_table(_self, _self.value);
//     auto user_auths_iter = user_auths_table.find(_user.value);
//     if (user_auths_iter != user_auths_table.end())
//     {
//         user_auths_table.erase(user_auths_iter);
//     }

//     user_logs user_logs_table(_self, _self.value);
//     auto user_logs_iter = user_logs_table.find(_user.value);
//     if (user_logs_iter != user_logs_table.end())
//     {
//         user_logs_table.erase(user_logs_iter);
//     }

//     gold_logs user_gold_log_table(_self, _self.value);
//     auto gold_iter = user_gold_log_table.find(_user.value);
//     if (gold_iter != user_gold_log_table.end())
//     {
//         user_gold_log_table.erase(gold_iter);
//     }

//     gold_gacha_results user_gold_result_table(_self, _self.value);
//     auto gold_result_iter = user_gold_result_table.find(_user.value);
//     if (gold_result_iter != user_gold_result_table.end())
//     {
//         user_gold_result_table.erase(gold_result_iter);
//     }

//     gold_gacha_totals user_gold_total_table(_self, _self.value);
//     auto gold_result_total_iter = user_gold_total_table.find(_user.value);
//     if (gold_result_total_iter != user_gold_total_table.end())
//     {
//         user_gold_total_table.erase(gold_result_total_iter);
//     }

//     user_gacha_results user_gacha_result_table(_self, _self.value);
//     auto gacha_result_iter = user_gacha_result_table.find(_user.value);
//     if (gacha_result_iter != user_gacha_result_table.end())
//     {
//         user_gacha_result_table.erase(gacha_result_iter);
//     }

//     user_gacha_totals user_gacha_total_table(_self, _self.value);
//     auto gacha_total_iter = user_gacha_total_table.find(_user.value);
//     if (gacha_total_iter != user_gacha_total_table.end())
//     {
//         user_gacha_total_table.erase(gacha_total_iter);
//     }

//     user_equip_items user_preregist_item_table(_self, _user.value);
//     auto item_iter = user_preregist_item_table.begin();
//     if (item_iter != user_preregist_item_table.end())
//     {
//         for (auto item = user_preregist_item_table.begin(); item != user_preregist_item_table.end();)
//         {
//             auto iter = user_preregist_item_table.find(item->primary_key());
//             item++;
//             user_preregist_item_table.erase(iter);
//         }
//     }

//     user_items user_consum_item_table(_self, _user.value);
//     auto user_consum_item_iter = user_consum_item_table.begin();
//     if (user_consum_item_iter != user_consum_item_table.end())
//     {
//         for (auto consumable = user_consum_item_table.begin(); consumable != user_consum_item_table.end();)
//         {
//             auto iter = user_consum_item_table.find(consumable->primary_key());
//             consumable++;
//             user_consum_item_table.erase(iter);
//         }
//     }

//     user_monsters user_preregist_monster_table(_self, _user.value);
//     auto mon_iter = user_preregist_monster_table.begin();
//     if (mon_iter != user_preregist_monster_table.end())
//     {
//         for (auto mon = user_preregist_monster_table.begin(); mon != user_preregist_monster_table.end();)
//         {
//             auto iter = user_preregist_monster_table.find(mon->primary_key());
//             mon++;
//             user_preregist_monster_table.erase(iter);
//         }
//     }

//     user_servants user_preregist_servant_table(_self, _user.value);
//     auto ser_iter = user_preregist_servant_table.begin();
//     if (ser_iter != user_preregist_servant_table.end())
//     {
//         for (auto ser = user_preregist_servant_table.begin(); ser != user_preregist_servant_table.end();)
//         {
//             auto iter = user_preregist_servant_table.find(ser->primary_key());
//             ser++;
//             user_preregist_servant_table.erase(iter);
//         }
//     }

//     account to_acnts(_self, _user.value);
//     auto to = to_acnts.begin();
//     if (to != to_acnts.end())
//     {
//         add_balance(_self, to->balance, _self);
//         sub_balance(_user, to->balance);
//     }

//     user_partys user_party_table(_self, _user.value);
//     auto party_iter = user_party_table.begin();
//     if (party_iter != user_party_table.end())

//     {
//         user_party_table.erase(party_iter);
//     }


//     user_mail mail_db_table(_self, _user.value);
//     for (auto item = mail_db_table.begin(); item != mail_db_table.end();)
//     {
//         auto iter = mail_db_table.find(item->primary_key());
//         mail_db_table.erase(iter);
//         item++;
//     }
// }


#pragma endregion


#pragma region item burn

// ACTION battletest::itemburn(eosio::name _user, const std::vector<uint64_t> &_item_list, const std::vector<uint64_t> &_count_list)
// {
//     system_check(_user);

//     user_auths user_auth_table(_self, _self.value);
//     auto user_auth_iter = user_auth_table.find(_user.value);
//     user_logs user_logs_table(_self, _self.value);
//     auto user_logs_iter = user_logs_table.find(_user.value);
//     user_items user_item_table(_self, _user.value);
//     allitem_db allitem_db_table(_self, _self.value);

//     asset item_burn_result(0, symbol(symbol_code("UTG"), 4));

//     std::string contents_list;

//     uint64_t check_inventory = 0;
//     uint64_t check_count = 0;
//     for (uint32_t i = 0; i < _item_list.size(); ++i)
//     {
//         auto user_item_iter = user_item_table.find(_item_list[i]);
//         auto allitem_db_iter = allitem_db_table.find(user_item_iter->id);
  
//         uint64_t temp_count = _count_list[i];
//         uint64_t check_total_item_count = 0;

//         for (uint64_t h = 0; h < user_item_iter->item_list.size(); h++) 
//         {
//             check_total_item_count += user_item_iter->item_list[h].count;   //내가 가지고 있는 아이템 갯수 총 합 
//         }
//         eosio_assert(check_total_item_count >= _count_list[i], "itemburn : not enough sell item count");
//         eosio_assert(user_item_iter != user_item_table.end(), "itemburn : not exist item info");
//         eosio_assert(_item_list.size() == _count_list.size(), "itemburn : The lengths of the two do not match");

//         if (check_total_item_count == temp_count)
//         {
//             user_item_table.erase(user_item_iter);
//         }

//         else
//         {
//             user_item_table.modify(user_item_iter, _self, [&](auto &change_user_item) {

//                 auto item_iter = change_user_item.item_list.end() - 1;

//                 for (uint64_t k = change_user_item.item_list.size() - 1; k >= 0; k--)
//                 {
//                     if (change_user_item.item_list[k].count < temp_count)
//                     {
//                         temp_count = (temp_count - change_user_item.item_list[k].count);
//                         change_user_item.item_list[k].count = 0;
//                         change_user_item.item_list.erase(item_iter);
//                         check_inventory += 1;
//                     }
//                     else
//                     {
//                         change_user_item.item_list[k].count -= temp_count;
//                         if (change_user_item.item_list[k].count == 0)
//                         {
//                             change_user_item.item_list.erase(item_iter);
//                             check_inventory += 1;
//                             item_iter--;
//                         }
//                         break;
//                     }
//                 }
//                 check_count += temp_count;
//             });
//         }
//         item_burn_result.amount += allitem_db_iter->sell_item_count * _count_list[i];
//     }

//     action(permission_level{get_self(), "active"_n},
//            get_self(), "transfer"_n,
//            std::make_tuple(_self, _user, item_burn_result, std::string("item burn result")))
//         .send();

//     user_auth_table.modify(user_auth_iter, _self, [&](auto &change_auth_user) {
//         eosio_assert(change_auth_user.current_item_inventory >= _count_list.size(), "itemburn : The current number of item and the number of units to sell are not correct.");
//         if (change_auth_user.current_item_inventory - _count_list.size() <= 0)
//         {
//             change_auth_user.current_item_inventory = 0;
//         }
//         else
//         {
//             change_auth_user.current_item_inventory -= check_inventory;
//         }
//     });

//     user_logs_table.modify(user_logs_iter, _self, [&](auto &change_log) {
//         change_log.get_utg += item_burn_result.amount;
//     });
// }

#pragma endregion

#pragma region chat
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

#pragma endregion


#pragma region test function

// ACTION battletest::anothercheck(uint32_t _start_count)
// {
//     require_auth(_self);

//     uint64_t limit_count = 3000;
//     uint64_t cur_total_item_count = 0;

//     uint32_t iter_start = 1;
//     uint32_t cur_count = _start_count;

//     user_auths user_auths_table(_self, _self.value);

//     for (auto all_log = user_auths_table.begin(); all_log != user_auths_table.end();)
//     {
//         if (iter_start < _start_count)
//         {
//             iter_start++;
//             all_log++;
//             continue;
//         }
//         auto user_log = user_auths_table.find(all_log->primary_key());

//         user_items user_item_table(_self, user_log->user.value);
//         uint64_t count = 0;
//         for (auto item = user_item_table.begin(); item != user_item_table.end();)
//         {
//             auto user_item_log = user_item_table.find(item->primary_key());
//             item++;

//             if (user_item_log->item_list.size() == 0)
//             {
//                 user_item_table.erase(user_item_log);
//             }
//             else
//             {
//                 count++;
//             }
//         }

//         if ((cur_total_item_count + count) >= limit_count) //ttemp에 넣기
//         {
//             global_count global_count_table(_self, _self.value);
//             auto g_iter = global_count_table.find(_self.value);
//             global_count_table.emplace(_self, [&](auto &new_data) {
//                 new_data.count = cur_count;
//             });
//             break;
//         }
//         else
//         {
//             cur_total_item_count += count;
//             user_auths_table.modify(user_log, _self, [&](auto &new_data) {
//                 new_data.user = user_log->user;
//                 new_data.current_item_inventory = count;
//             });
//             cur_count += 1;
//             all_log++;
//         }
//     }
// }

// ACTION battletest::deletelog()
// {
//     require_auth(_self);

//     global_count global_count_table(_self, _self.value);

//     for (auto gloiter = global_count_table.begin(); gloiter != global_count_table.end();)
//     {
//         auto glo2 = global_count_table.find(gloiter->primary_key());
//         gloiter++;
//         global_count_table.erase(glo2);
//     }
// }


// ACTION battletest::simulate(eosio::name _user, std::string _type, std::string _value)
// {
//    master_active_check();

//    std::vector<size_t> size_list;
//    std::vector<std::string> value_list;
//    uint32_t value;

//    lv_exp lv_exp_table(_self, _self.value);

//    user_auths user_auths_table(_self, _self.value);
//    auto user_auths_iter = user_auths_table.find(_user.value);
//    user_equip_items user_equipment_table(_self, _user.value);
//    user_servants user_servant_table(_self, _user.value);

//    user_monsters user_monster_table(_self, _user.value);

//    if (_type == "servant")
//    {
//        substr_value(_value, value_list, size_list, 7); //id, level, str, dex, int, passive, active

//        auto iter = lv_exp_table.find(atoi(value_list[1].c_str()));

//        user_servant_table.emplace(_self, [&](auto &new_data) {
//            new_data.index = user_servant_table.available_primary_key();
//            new_data.party_number = 0;

//            servant_info new_servant;
//            new_servant.id = atoi(value_list[0].c_str());
//            new_servant.grade = 5;
//            new_servant.level = iter->lv;
//            new_servant.exp = iter->char_exp;
//            new_servant.state = object_state::on_inventory;
//            new_servant.status.basic_str = atoi(value_list[2].c_str());
//            new_servant.status.basic_dex = atoi(value_list[3].c_str());
//            new_servant.status.basic_int = atoi(value_list[4].c_str());
//            new_servant.passive_skill.push_back(atoi(value_list[5].c_str()));
//            new_servant.active_skill.push_back(atoi(value_list[6].c_str()));
//            new_servant.equip_slot.resize(3);

//            new_data.servant = new_servant;
//        });

//        user_auths_table.modify(user_auths_iter, _self, [&](auto &new_data) {
//            new_data.current_servant_inventory += 1;
//        });
//    }
//    else if (_type == "monster")
//    {
//        substr_value(_value, value_list, size_list, 8); //id, level, grade, upgrade, str, dex, int, passive
//        auto iter = lv_exp_table.find(atoi(value_list[1].c_str()));

//        monster_db monster_db_table(_self, _self.value);
//        auto monster_db_iter = monster_db_table.find(atoi(value_list[0].c_str()));

//        user_monster_table.emplace(_self, [&](auto &new_data) {
//            new_data.index = user_monster_table.available_primary_key();
//            new_data.party_number = 0;

//            monster_info new_servant;
//            new_servant.id = monster_db_iter->id;
//            new_servant.state = object_state::on_inventory;
//            new_servant.type = monster_db_iter->type;
//            new_servant.tribe = monster_db_iter->tribe;
//            new_servant.grade = atoi(value_list[2].c_str());
//            new_servant.level = iter->lv;
//            new_servant.exp = iter->char_exp;
//            new_servant.upgrade = atoi(value_list[3].c_str());
//            new_servant.status.basic_str = atoi(value_list[4].c_str());
//            new_servant.status.basic_dex = atoi(value_list[5].c_str());
//            new_servant.status.basic_int = atoi(value_list[6].c_str());
//            new_servant.passive_skill.push_back(atoi(value_list[7].c_str()));

//            new_data.monster = new_servant;
//        });
//        user_auths_table.modify(user_auths_iter, _self, [&](auto &new_data) {
//            new_data.current_monster_inventory += 1;
//        });
//    }
//    else if (_type == "equipment")
//    {
//        substr_value(_value, value_list, size_list, 4); //id,  grade, upgrade, value

//        equipment_db equipment_db_table(_self, _self.value);
//        auto equipment_db_iter = equipment_db_table.find(atoi(value_list[0].c_str()));

//        user_equipment_table.emplace(_self, [&](auto &new_data) {
//            new_data.index = user_equipment_table.available_primary_key();

//            equipment_info new_servant;
//            new_servant.id = equipment_db_iter->item_id;
//            new_servant.state = object_state::on_inventory;
//            new_servant.type = equipment_db_iter->type;
//            new_servant.tier = equipment_db_iter->tier;
//            new_servant.job = equipment_db_iter->job;
//            new_servant.grade = atoi(value_list[1].c_str());
//            new_servant.upgrade = atoi(value_list[2].c_str());
//            new_servant.value = atoi(value_list[3].c_str());
//            new_servant.equipservantindex = 0;

//            new_data.equipment = new_servant;
//        });
//        user_auths_table.modify(user_auths_iter, _self, [&](auto &new_data) {
//            new_data.current_equipment_inventory += 1;
//        });
//    }

//    else if (_type == "count")
//    {
//        uint32_t count1 = 0;
//        uint32_t count2 = 0;
//        uint32_t count3 = 0;
//        for (auto iter1 = user_servant_table.begin(); iter1 != user_servant_table.end();)
//        {
//            count1++;
//            iter1++;
//        }
//        for (auto iter2 = user_monster_table.begin(); iter2 != user_monster_table.end();)
//        {
//            count2++;
//            iter2++;
//        }
//        for (auto iter3 = user_equipment_table.begin(); iter3 != user_equipment_table.end();)
//        {
//            count3++;
//            iter3++;
//        }

//        user_auths_table.modify(user_auths_iter, _self, [&](auto &new_data) {
//            new_data.current_servant_inventory = count1;
//            new_data.current_monster_inventory = count2;
//            new_data.current_equipment_inventory = count3;
//        });
//    }
// }

// ACTION battletest::monstercheat(eosio::name _user, uint32_t _grade, uint32_t _id, uint32_t _count)
// {
//     require_auth(_self);

//     monster_db monster_db_table(_self, _self.value);
//     auto iter = monster_db_table.find(_id);
//     eosio_assert(iter != monster_db_table.end(),"Monster Cheat : Empty Monster ID / Wrong Monster ID");

//     user_monsters my_table(_self, _user.value);
//     for(uint32_t i = 0; i < _count; ++i)
//     {
//         uint64_t _seed = safeseed::get_seed_value(_user.value, i);
        
//         monster_grade_db monster_grade_db_table(_self, _self.value);
//         const auto &monster_grade_db_iter = monster_grade_db_table.get(_grade, "Monster Cheat : Empty Grade");

//         my_table.emplace(_self, [&](auto &update_user_monster_list) {
//             uint32_t first_index = my_table.available_primary_key();
//             if (first_index == 0)
//             {
//                 update_user_monster_list.index = 1;
//             }
//             else
//             {
//                 update_user_monster_list.index = my_table.available_primary_key();
//             }

//             monster_info new_monster;
//             new_monster.id = iter->id;
//             new_monster.grade = monster_grade_db_iter.grade;
//             new_monster.tribe = iter->tribe;
//             new_monster.type = iter->type;
//             new_monster.exp = 0;
//             new_monster.upgrade = 0;
//             new_monster.level = 1;
//             monster_random_count += 1;
//             new_monster.status.basic_str = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_str, monster_grade_db_iter.min_range.base_str, monster_random_count);
//             monster_random_count += 1;
//             new_monster.status.basic_dex = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_dex, monster_grade_db_iter.min_range.base_dex, monster_random_count);
//             monster_random_count += 1;
//             new_monster.status.basic_int = safeseed::get_random_value(_seed, monster_grade_db_iter.max_range.base_int, monster_grade_db_iter.min_range.base_int, monster_random_count);

//             new_monster.status.basic_str = change_monster_status(new_monster.grade, new_monster.status.basic_str);
//             new_monster.status.basic_dex = change_monster_status(new_monster.grade, new_monster.status.basic_dex);
//             new_monster.status.basic_int = change_monster_status(new_monster.grade, new_monster.status.basic_int);

//             new_monster.state = object_state::on_inventory;

//             uint32_t passive_id = get_passive_skill(2, iter->tribe, _seed);
//             new_monster.passive_skill.push_back(passive_id);

//             update_user_monster_list.party_number = EMPTY_PARTY;
//             update_user_monster_list.monster = new_monster;
//         });
//         monster_random_count = 0;
//     }

//     user_logs user_log_table(_self, _self.value);
//     auto user_log_iter = user_log_table.find(_user.value);
//     eosio_assert(user_log_iter != user_log_table.end(), "Monster Cheat : Already Log Table / Already Signup");
//     user_log_table.modify(user_log_iter, _self, [&](auto &new_log) {
//         new_log.monster_num += _count;
//         new_log.gacha_num += _count;
//     });


//     eosio_assert(check_inventory(_user) == true, "Monster Cheat : Invetory Full");
//     user_auths auth_user_table(_self, _self.value);
//     auto new_user_iter = auth_user_table.find(_user.value);
//     eosio_assert(new_user_iter != auth_user_table.end(), "Monster Cheat : Already Auth Table / Already Signup");
//     auth_user_table.emplace(_self, [&](auto &new_user) {
//         new_user.current_monster_inventory = _count;
//     });
// }


// ACTION battletest::balancetest(eosio::name _user, std::string _type, std::string _value)
// {
//     require_auth(_self);
//     std::vector<size_t> size_list;
//     std::vector<std::string> value_list;
//     uint32_t value;

//     if(_type == "servant")
//     {
//         substr_value(_value, value_list, size_list, 4); //level, str,dex,int
//         uint32_t level = atoi(value_list[0].c_str());
//         uint32_t b_str = atoi(value_list[1].c_str());
//         uint32_t b_dex = atoi(value_list[2].c_str());
//         uint32_t b_int = atoi(value_list[3].c_str());

//         lv_exp level_table(_self, _self.value);
//         auto lv_iter = level_table.find(level);

//         user_servants user_servant_table(_self, _user.value);
//         for(auto iter = user_servant_table.begin(); iter != user_servant_table.end();)
//         {
//             auto ser = user_servant_table.find(iter->primary_key());
//             user_servant_table.modify(ser, _self, [&](auto &new_data)
//             {
//                 new_data.servant.level = lv_iter->lv;
//                 new_data.servant.exp = lv_iter->char_exp;
//                 if(b_str != 0)
//                 {
//                     new_data.servant.status.basic_str = b_str;
//                 }
//                 if(b_dex != 0)
//                 {
//                     new_data.servant.status.basic_dex = b_dex;
//                 }
//                 if(b_int != 0)
//                 {
//                     new_data.servant.status.basic_int = b_int;
//                 }
//             });
//             iter++;
//         }

//     }
//     if(_type == "monster")
//     {
//         substr_value(_value, value_list, size_list, 6);//level, str,dex,int,grade,upgrade 
//         uint32_t level = atoi(value_list[0].c_str());
//         uint32_t b_str = atoi(value_list[1].c_str());
//         uint32_t b_dex = atoi(value_list[2].c_str());
//         uint32_t b_int = atoi(value_list[3].c_str());
//         uint32_t grade = atoi(value_list[4].c_str());
//         uint32_t upgrade = atoi(value_list[5].c_str());
    
//         lv_exp level_table(_self, _self.value);
//         auto lv_iter = level_table.find(level);

//         user_monsters my_table(_self, _user.value);
//         for(auto iter = my_table.begin(); iter != my_table.end();)
//         {
//             auto ser = my_table.find(iter->primary_key());
//             my_table.modify(ser, _self, [&](auto &new_data)
//             {
//                 new_data.monster.level = lv_iter->lv;
//                 new_data.monster.exp = lv_iter->char_exp;
//                 if(b_str != 0)
//                 {
//                     new_data.monster.status.basic_str = b_str;
//                 }
//                 if(b_dex != 0)
//                 {
//                     new_data.monster.status.basic_dex = b_dex;
//                 }
//                 if(b_int != 0)
//                 {
//                     new_data.monster.status.basic_int = b_int;
//                 }
//                 new_data.monster.grade = grade;
//                 new_data.monster.upgrade = upgrade;
//             });
//             iter++;
//         }

//     }
//     if(_type == "equipment")
//     {
//         substr_value(_value, value_list, size_list, 3);//value,grade,upgrade 
//         uint32_t e_value = atoi(value_list[0].c_str());
//         uint32_t grade = atoi(value_list[1].c_str());
//         uint32_t upgrade = atoi(value_list[2].c_str());
    
//         user_equip_items my_table(_self, _user.value);
//         for(auto iter = my_table.begin(); iter != my_table.end();)
//         {
//             auto ser = my_table.find(iter->primary_key());
//             my_table.modify(ser, _self, [&](auto &new_data)
//             {
//                 if(e_value != 0)
//                 {
//                     new_data.equipment.value = e_value;
//                 }
//                 new_data.equipment.grade = grade;
//                 new_data.equipment.upgrade = upgrade;
//             });
//             iter++;
//         }
//     }
//     if(_type == "item")
//     {
//         substr_value(_value, value_list, size_list, 2);//id,type
//         uint32_t id = atoi(value_list[0].c_str());
//         uint32_t type = atoi(value_list[1].c_str());

//         user_items my_table(_self, _user.value);
//         auto iter = my_table.find(id);
//         if(iter == my_table.end())
//         {
//             my_table.emplace(_self, [&](auto &new_data) {
//                 item_info new_item;
//                 new_item.index = 0;
//                 new_item.count = 99;
//                 new_data.id = id;
//                 new_data.type = type;
//                 new_data.item_list.push_back(new_item);
//             });
//         }
//         else
//         {
//             my_table.modify(iter, _self, [&](auto &new_data) {
//                 new_data.item_list[0].count = 99;
//             });
//         }
//     }

    
// }



// ACTION battletest::accountset(eosio::name _user, uint32_t _ser_lv, uint32_t _mon_lv, uint32_t _equip_up, uint32_t _monster_up)
// {
//     uint32_t servant_count = 0;
//     uint32_t monster_count = 0;
//     uint32_t equipment_count = 0;

//     require_auth(_self);

//     user_partys user_party_table(_self, _user.value);
//     auto user_party_iter = user_party_table.find(1);
//     eosio_assert(user_party_iter == user_party_table.end(), "Account Test : Already Party Table / Already Signup");
//     user_party_table.emplace(_self, [&](auto &automatic_party) {
//         automatic_party.index = 1;
//         automatic_party.servant_list.resize(5);
//         automatic_party.monster_list.resize(5);
//         for(uint32_t i = 1; i < 6; ++ i)
//         {
//             automatic_party.servant_list[i - 1] = i;
//             automatic_party.monster_list[i - 1] = i;
//         }
//     });

//     uint32_t job = 1;
//     servant_job_db servant_job_table(_self, _self.value);
//     for (uint32_t i = 0; i < 50; ++i)
//     {
//         uint64_t _seed = safeseed::get_seed_value(_user.value, i);
//         uint32_t random_job = job;
//         if (job == 4)
//         {
//             job = 1;
//         }
//         else{
//         job += 1;
//         }
//         const auto &servant_job_db_iter = servant_job_table.get(random_job, "Account Test : Empty Servant Job / Wrong Servant Job");
//         uint32_t random_body = gacha_servant_body(_seed, servant_random_count);

//         servant_random_count += 1;
//         gender_db gender_table(_self, _self.value);
//         uint32_t random_gender = safeseed::get_random_value(_seed, GEMDER_COUNT, DEFAULT_MIN_DB, servant_random_count);
//         const auto &gender_db_iter = gender_table.get(random_gender, "Account Test : Empty Servant Gender / Wrong Servant Gender");

//         servant_random_count += 1;
//         uint32_t random_head = gacha_servant_head(_seed, servant_random_count);

//         servant_random_count += 1;
//         uint32_t random_hair = gacha_servant_hair(_seed, servant_random_count);

//         servant_db servant_id_table(_self, _self.value);
//         uint32_t servant_index = get_servant_index(random_job, random_body, random_gender, random_head, random_hair);
//         const auto &servant_id_db_iter = servant_id_table.get(servant_index, "Account Test : Empty Servant ID / Wrong Servant ID");

//         serstat_db servant_base_table(_self, _self.value);
//         uint32_t servant_job_base = (servant_id_db_iter.job * 1000) + (servant_id_db_iter.grade * 100) + 1;
//         const auto &ser_iter = servant_base_table.get(servant_job_base, "Account Test Servant : Empty Servant Stat");

//         user_servants user_servant_table(_self, _user.value);
//         user_servant_table.emplace(_self, [&](auto &update_user_servant_list) {
//             uint32_t first_index = user_servant_table.available_primary_key();
//             if (first_index == 0)
//             {
//                 update_user_servant_list.index = 1;
//             }
//             else
//             {
//                 update_user_servant_list.index = user_servant_table.available_primary_key();
//             }

//             servant_info new_servant;
//             new_servant.id = servant_id_db_iter.id;
//             new_servant.exp = 0;
//             new_servant.grade = 5;
//             new_servant.level = _ser_lv;
//             servant_random_count += 1;
//             new_servant.status.basic_str = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_str, servant_job_db_iter.min_range.base_str, servant_random_count);
//             servant_random_count += 1;
//             new_servant.status.basic_dex = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_dex, servant_job_db_iter.min_range.base_dex, servant_random_count);
//             servant_random_count += 1;
//             new_servant.status.basic_int = safeseed::get_random_value(_seed, servant_job_db_iter.max_range.base_int, servant_job_db_iter.min_range.base_int, servant_random_count);
//             new_servant.status.basic_str = change_servant_status(new_servant.status.basic_str) + ser_iter.base_str;
//             new_servant.status.basic_dex = change_servant_status(new_servant.status.basic_dex) + ser_iter.base_dex;
//             new_servant.status.basic_int = change_servant_status(new_servant.status.basic_int) + ser_iter.base_int;
//             new_servant.equip_slot.resize(3);
//             new_servant.state = object_state::on_inventory;

//             uint32_t active_id = get_servant_active_skill(servant_id_db_iter.job, _seed);
//             new_servant.active_skill.push_back(active_id);

//             uint32_t passive_id = get_passive_skill(1, servant_id_db_iter.job, _seed);
//             new_servant.passive_skill.push_back(passive_id);

//             update_user_servant_list.party_number = EMPTY_PARTY;
//             update_user_servant_list.servant = new_servant;
//         });
//         servant_random_count = 0;
//         servant_count += 1;
//     }

//     /////////monster
//     main_gacha_db main_gacha_db_table(_self, _self.value);
//     auto gacha_db_iter = main_gacha_db_table.find(GACHA_MONSTER_COUNT_ID); //1 ser 2 mon 3 item
//     eosio_assert(gacha_db_iter != main_gacha_db_table.end(), "Account Test Monster : Empty Max Count");
//     for (uint32_t a = 0; a < 3; ++a)
//     {
//         for (uint32_t i = 1; i <= gacha_db_iter->db_index; ++i)
//         {
//             uint64_t _seed = safeseed::get_seed_value(_user.value, i);

//             uint32_t id = i + MONSTER_GACHA_ID_START;
//             auto gacha_monster_db_iter = main_gacha_db_table.find(id);
//             eosio_assert(gacha_monster_db_iter != main_gacha_db_table.end(), "Account Test Monster : Empty Gacha ID");

//             monster_db monster_id_db_table(_self, _self.value);
//             const auto &monster_id_db_iter = monster_id_db_table.get(gacha_monster_db_iter->db_index, "Account Test Monster : Empty Monster ID");

//             tribe_db tribe_db_table(_self, _self.value);
//             const auto &tribe_iter = tribe_db_table.get(monster_id_db_iter.tribe, "Gacha Monster : Empty Monster Tribe");

//             uint64_t random_rate = safeseed::get_random_value(_seed, GACHA_MAX_RATE, DEFAULT_MIN, 3);
//             uint64_t random_grade = get_random_grade(random_rate);

//             user_monsters user_monster_table(_self, _user.value);
//             user_monster_table.emplace(_self, [&](auto &update_user_monster_list) {
//                 uint32_t first_index = user_monster_table.available_primary_key();
//                 if (first_index == 0)
//                 {
//                     update_user_monster_list.index = 1;
//                 }
//                 else
//                 {
//                     update_user_monster_list.index = user_monster_table.available_primary_key();
//                 }

//                 monster_info new_monster;
//                 new_monster.id = monster_id_db_iter.id;
//                 new_monster.grade = random_grade;
//                 new_monster.tribe = monster_id_db_iter.tribe;
//                 new_monster.type = monster_id_db_iter.type;
//                 new_monster.exp = 0;
//                 new_monster.upgrade = _monster_up;
//                 new_monster.level = _mon_lv;
  
//                 new_monster.status.basic_str = safeseed::get_random_value(_seed, 10,0, 4);
//                 new_monster.status.basic_dex = safeseed::get_random_value(_seed, 10,0, 5);
//                 new_monster.status.basic_int = safeseed::get_random_value(_seed, 10,0, 6);

//                 new_monster.status.basic_str = change_monster_status(new_monster.grade, new_monster.status.basic_str);
//                 new_monster.status.basic_dex = change_monster_status(new_monster.grade, new_monster.status.basic_dex);
//                 new_monster.status.basic_int = change_monster_status(new_monster.grade, new_monster.status.basic_int);

//                 new_monster.status.basic_str = (new_monster.status.basic_str * tribe_iter.base_str) / 100;
//                 new_monster.status.basic_dex = (new_monster.status.basic_dex * tribe_iter.base_dex) / 100;
//                 new_monster.status.basic_int = (new_monster.status.basic_int * tribe_iter.base_int) / 100;

//                 new_monster.state = object_state::on_inventory;

//                 uint32_t passive_id = get_passive_skill(2, monster_id_db_iter.tribe, _seed);
//                 new_monster.passive_skill.push_back(passive_id);

//                 update_user_monster_list.party_number = EMPTY_PARTY;
//                 update_user_monster_list.monster = new_monster;
//             });
//             monster_count += 1;
//             monster_random_count = 0;
//         }
//     }

//     ///////////equipment///////////////////
//     main_gacha_db equipment_main_gacha(_self, _self.value);
//     auto equipment_gacha_db_iter = equipment_main_gacha.find(GACHA_EQUIPMENT_COUNT_ID); //1 ser 2 mon 3 item
//     eosio_assert(equipment_gacha_db_iter != equipment_main_gacha.end(), "Account Test Equipment : Empty Max Count / Not Set Count");
//     for(uint32_t i = 1 ; i <= equipment_gacha_db_iter->db_index; ++i)
//     {
//         uint64_t _seed = safeseed::get_seed_value(_user.value, i);

//         uint32_t id = i + EQUIP_GACHA_ID_START;

//         auto gacha_id_db_iter = equipment_main_gacha.find(id);
//         eosio_assert(gacha_id_db_iter != equipment_main_gacha.end(), "Account Test Equipment : Empty Gacha ID / Not Set Gacha ID");

//         equipment_db equip_item_table(_self, _self.value);
//         const auto &equip_item_iter = equip_item_table.get(gacha_id_db_iter->db_index, "Account Test Equipmnet : Empty Equipment ID / Not Set Equipment ID");

//         item_random_count += 1;
//         uint64_t random_rate = safeseed::get_random_value(_seed, GACHA_MAX_RATE, DEFAULT_MIN, item_random_count);
//         uint64_t random_grade = get_random_grade(random_rate);

//         user_equip_items user_item_table(_self, _user.value);
//         user_item_table.emplace(_self, [&](auto &update_user_item_list) {
//             uint32_t first_index = user_item_table.available_primary_key();
//             if (first_index == 0)
//             {
//                 update_user_item_list.index = 1;
//             }
//             else
//             {
//                 update_user_item_list.index = user_item_table.available_primary_key();
//             }

//             equipment_info new_item;
//             new_item.id = equip_item_iter.item_id;
//             new_item.type = equip_item_iter.type;
//             new_item.tier = equip_item_iter.tier;
//             new_item.job = equip_item_iter.job;
//             new_item.grade = random_grade;
//             new_item.upgrade = _equip_up;
//             item_random_count += 1;
//             uint32_t type_grade = ((equip_item_iter.type + 1) * 10) + random_grade;
//             new_item.value = safeseed::get_random_value(_seed, 10, 0, item_random_count);
//             new_item.value = change_equipment_statue(type_grade, new_item.value);
//             set_tier_status(new_item.value, equip_item_iter.tier);

//             new_item.state = object_state::on_inventory;

//             update_user_item_list.equipment = new_item;
//         });
//         equipment_count += 1;
//         item_random_count = 0;
//     }

//     user_items user_item_table(_self, _user.value);
//     user_item_table.emplace(_self, [&](auto &new_data) {
//         item_info new_item;
//         new_item.index = 0;
//         new_item.count = 99;

//         new_data.id = 500200;
//         new_data.type = 3;
//         new_data.item_list.push_back(new_item);
//     });

//     user_item_table.emplace(_self, [&](auto &new_data) {
//         item_info new_item;
//         new_item.index = 0;
//         new_item.count = 99;

//         new_data.id = 500210;
//         new_data.type = 3;
//         new_data.item_list.push_back(new_item);
//     });

//     user_item_table.emplace(_self, [&](auto &new_data) {
//         item_info new_item;
//         new_item.index = 0;
//         new_item.count = 1;

//         new_data.id = 500220;
//         new_data.type = 3;
//         new_data.item_list.push_back(new_item);
//     });

//     user_item_table.emplace(_self, [&](auto &new_data) {
//         item_info new_item;
//         new_item.index = 0;
//         new_item.count = 1;

//         new_data.id = 500230;
//         new_data.type = 3;
//         new_data.item_list.push_back(new_item);
//     });

//     user_auths auth_user_table(_self, _self.value);
//     auto new_user_iter = auth_user_table.find(_user.value);
//     eosio_assert(new_user_iter == auth_user_table.end(), "Account Test : Already Auth Table / Already Signup");
//     auth_user_table.emplace(_self, [&](auto &new_user) {
//         new_user.user = _user;
//         new_user.state = user_state::lobby;
//         new_user.exp = 0;
//         new_user.rank = 1;
//         new_user.current_servant_inventory = servant_count;
//         new_user.current_monster_inventory = monster_count;
//         new_user.current_item_inventory = 4;
//         new_user.current_equipment_inventory = equipment_count;
//         new_user.servant_inventory = 200;
//         new_user.monster_inventory = 200;
//         new_user.item_inventory = 200;
//         new_user.equipment_inventory = 200;
//     });

//     user_logs user_log_table(_self, _self.value);
//     auto user_log_iter = user_log_table.find(_user.value);
//     eosio_assert(user_log_iter == user_log_table.end(), "Account Test : Already Log Table / Already Signup");
//     user_log_table.emplace(_self, [&](auto &new_log) {
//         new_log.user = _user;
//         new_log.servant_num = servant_count;
//         new_log.monster_num = monster_count;
//         new_log.equipment_num = equipment_count;
//         new_log.gacha_num = servant_count + monster_count + equipment_count;
//     });
// }


// ACTION battletest::leveltest(eosio::name _user)
// {
//     require_auth(_self);

//     lv_exp lv_exp_table(_self, _self.value);

//     user_monsters user_monster_table(_self, _user.value);
//     for(auto mon = user_monster_table.begin(); mon != user_monster_table.end();)
//     {
//         auto iter = user_monster_table.find(mon->primary_key());
        
//         auto lv_iter = lv_exp_table.find(49);

//         user_monster_table.modify(iter, _self, [&](auto &new_data)
//         {
//             new_data.monster.level = 49;
//             new_data.monster.exp = lv_iter->char_exp - 100;
//         });
//         mon++;
//     }

//     user_servants user_servant_table(_self, _user.value);
//     for (auto ser = user_servant_table.begin(); ser != user_servant_table.end();)
//     {
//         auto iter = user_servant_table.find(ser->primary_key());

//         auto lv_iter = lv_exp_table.find(49);

//         user_servant_table.modify(iter, _self, [&](auto &new_data) {
//             new_data.servant.level = 49;
//             new_data.servant.exp = lv_iter->char_exp - 100;
//         });
//         ser++;
//     }
// }

// ACTION battletest::updatecheack(uint32_t _start_count)
// {
//     require_auth(_self);

//     uint64_t limit_mail_count = 2000;
//     uint64_t cur_total_mail_count = 0;

//     uint32_t iter_start = 1;
//     uint32_t cur_count = _start_count;

//     user_logs user_logs_table(_self, _self.value);

//     for (auto all_log = user_logs_table.begin(); all_log != user_logs_table.end();)
//     {
//         if (iter_start < _start_count)
//         {
//             iter_start++;
//             all_log++;
//             continue;
//         }
//         auto user_log = user_logs_table.find(all_log->primary_key());

//         user_monsters user_monster_table(_self, user_log->user.value);
//         user_servants user_servant_tabla(_self, user_log->user.value);
//         uint64_t count = 0;
//         for (auto mail = user_monster_table.begin(); mail != user_monster_table.end();)
//         {
//             mail++;
//             count++;
//         }
//         for(auto ser = user_servant_tabla.begin(); ser != user_servant_tabla.end();)
//         {
//             ser++;
//             count++;
//         }

//         if ((cur_total_mail_count + count) >= limit_mail_count)
//         {
//             global_count global_count_table(_self, _self.value);
//             auto g_iter = global_count_table.find(_self.value);
//             global_count_table.emplace(_self, [&](auto &new_data) {
//                 new_data.count = cur_count;
//             });
//             break;
//         }
//         else
//         {
//             cur_total_mail_count += count;
//             lv_exp lv_exp_table(_self, _self.value);
//             for(auto mon = user_monster_table.begin(); mon != user_monster_table.end();)
//             {
//                 auto iter = user_monster_table.find(mon->primary_key());
//                 auto lv_iter = lv_exp_table.find(49);
//                 if (iter->monster.level == 50)
//                 {
//                     if (lv_iter->char_exp < iter->monster.exp)
//                     {
//                         user_monster_table.modify(iter, _self, [&](auto &new_data) {
//                             new_data.monster.exp = lv_iter->char_exp;
//                         });
//                     }
//                 }
//                 mon++;
//             }

//             for (auto ser = user_servant_tabla.begin(); ser != user_servant_tabla.end();)
//             {
//                 auto iter = user_servant_tabla.find(ser->primary_key());
//                 auto lv_iter = lv_exp_table.find(49);
//                 if (iter->servant.level == 50)
//                 {
//                     if (lv_iter->char_exp < iter->servant.exp)
//                     {
//                         user_servant_tabla.modify(iter, _self, [&](auto &new_data) {
//                             new_data.servant.exp = lv_iter->char_exp;
//                         });
//                     }
//                 }
//                 ser++;
//             }
//             new_battle_state_list battle_state_list_table(_self, _self.value);
//             auto battle_iter = battle_state_list_table.find(user_log->user.value);
//             if(battle_iter != battle_state_list_table.end())
//             {
//                 battle_state_list_table.erase(battle_iter);
//             }

//             cur_count += 1;
//             all_log++;
//         }
//     }
// }


// ACTION battletest::movedata(uint32_t _start_count)
// {
//     require_auth(_self);

//     uint64_t limit_mail_count = 1000;
//     uint64_t cur_total_mail_count = 0;

//     uint32_t iter_start = 1;
//     uint32_t cur_count = _start_count;

//     user_auths user_auth_table(_self, _self.value);

//     for (auto all_log = user_auth_table.begin(); all_log != user_auth_table.end();)
//     {
//         if (iter_start < _start_count)
//         {
//             iter_start++;
//             all_log++;
//             continue;
//         }
//         auto user_log = user_auth_table.find(all_log->primary_key());

//         user_monsters user_monster_table(_self, user_log->user.value);
//         user_servants user_servant_tabla(_self, user_log->user.value);
//         uint64_t count = 0;
//         for (auto mail = user_monster_table.begin(); mail != user_monster_table.end();)
//         {
//             mail++;
//             count++;
//         }
//         for(auto ser = user_servant_tabla.begin(); ser != user_servant_tabla.end();)
//         {
//             ser++;
//             count++;
//         }

//         if ((cur_total_mail_count + count) >= limit_mail_count)
//         {
//             global_count global_count_table(_self, _self.value);
//             auto g_iter = global_count_table.find(_self.value);
//             global_count_table.emplace(_self, [&](auto &new_data) {
//                 new_data.count = cur_count;
//             });
//             break;
//         }
//         else
//         {
//             cur_total_mail_count += count;

//             new_user_monsters new_user_monster_table(_self, user_log->user.value);
//             new_user_servants new_user_servant_table(_self, user_log->user.value);

//             for(auto mon = user_monster_table.begin(); mon != user_monster_table.end();)
//             {
//                 auto iter = user_monster_table.find(mon->primary_key());
//                 mon++;
//                 new_user_monster_table.emplace(_self, [&](auto &new_data) {
//                     new_data.index = iter->index;
//                     new_data.party_number = iter->party_number;

//                     new_data.monster.id = iter->monster.id;
//                     new_data.monster.state = iter->monster.state;
//                     new_data.monster.exp = iter->monster.exp;
//                     new_data.monster.type = iter->monster.type;
//                     new_data.monster.tribe = iter->monster.tribe;
//                     new_data.monster.grade = iter->monster.grade;
//                     new_data.monster.upgrade = iter->monster.upgrade;
//                     new_data.monster.level = iter->monster.level;
//                     new_data.monster.status = iter->monster.status;
                    
//                     skill_info new_passive_skill;
//                     new_passive_skill.id = iter->monster.passive_skill[0];
//                     new_passive_skill.level = 1;

//                     new_data.monster.passive_skill.push_back(new_passive_skill);
//                 });
//                 user_monster_table.erase(iter);
//             }

//             for (auto ser = user_servant_tabla.begin(); ser != user_servant_tabla.end();)
//             {
//                 auto iter = user_servant_tabla.find(ser->primary_key());
//                 ser++;
//                 new_user_servant_table.emplace(_self, [&](auto &new_data)
//                 {
//                     new_data.index = iter->index;
//                     new_data.party_number = iter->party_number;
                    
//                     new_data.servant.state = iter->servant.state;
//                     new_data.servant.exp = iter->servant.exp;
//                     new_data.servant.id = iter->servant.id;
//                     new_data.servant.level = iter->servant.level;
//                     new_data.servant.grade = iter->servant.grade;
//                     new_data.servant.status = iter->servant.status;
//                     new_data.servant.equip_slot = iter->servant.equip_slot;

//                     skill_info new_passive_skill;
//                     new_passive_skill.id = iter->servant.passive_skill[0];
//                     new_passive_skill.level = 1;

//                     new_data.servant.passive_skill.push_back(new_passive_skill);

//                     skill_info new_active_skill;
//                     new_active_skill.id = iter->servant.active_skill[0];
//                     new_active_skill.level = 1;

//                     new_data.servant.active_skill.push_back(new_active_skill);
//                 });
//                 user_servant_tabla.erase(iter);
//             }

//             cur_count += 1;
//             all_log++;
//         }
//     }
// }

#pragma endregion


ACTION battletest::recorduser(uint32_t _start_count)
{
    require_auth(_self);

    uint64_t limit_count = 500;
    uint64_t cur_total_item_count = 0;

    uint32_t iter_start = 1;
    uint32_t cur_count = _start_count;

    user_auths user_auth_table(_self, _self.value);
    for (auto iter = user_auth_table.begin(); iter != user_auth_table.end();)
    {
        if (iter_start < _start_count)
        {
            iter_start++;
            iter++;
            continue;
        }
        auto user = user_auth_table.find(iter->primary_key());
        user_servants user_servants_table(_self, user->user.value);
        user_equip_items user_equip_items_table(_self, user->user.value);
        uint64_t count = 0;

        for (auto mail = user_equip_items_table.begin(); mail != user_equip_items_table.end();)
        {
            mail++;
            count++;
        }
        for(auto ser = user_servants_table.begin(); ser != user_servants_table.end();)
        {
            ser++;
            count++;
        }

        if ((cur_total_item_count + count) >= limit_count) //ttemp에 넣기
        {
            global_count global_count_table(_self, _self.value);
            auto g_iter = global_count_table.find(_self.value);
            global_count_table.emplace(_self, [&](auto &new_data) {
                new_data.count = cur_count;
            });
            break;
        }
        else
        {
            for (auto servant = user_servants_table.begin(); servant != user_servants_table.end();)
            {
                auto servant_iter = user_servants_table.find(servant->primary_key());

                for (uint32_t i = 0; i < 3; i++)
                {
                    if (servant_iter->servant.equip_slot[i] == 0)
                    {
                       servant++;
                        continue;
                    }
                    else
                    {
                        auto equipment_iter = user_equip_items_table.find(servant_iter->servant.equip_slot[i]);
                        if (equipment_iter == user_equip_items_table.end())
                        {
                            user_servants_table.modify(servant_iter, _self, [&](auto &new_data) {
                                new_data.servant.equip_slot[i] = 0;
                            });
                        }
                        else if (equipment_iter != user_equip_items_table.end())
                        {
                            if (equipment_iter->equipment.equipservantindex != servant_iter->index)
                            {

                                user_servants_table.modify(servant_iter, _self, [&](auto &new_data) {
                                    new_data.servant.equip_slot[i] = 0;
                                });
                            }
                        }
                    }
                }
                servant++;
            }
       
            cur_total_item_count += count;
            cur_count += 1;
            iter++;
        }
    }

}


ACTION battletest::recorduser2(uint32_t _start_count)
{
    require_auth(_self);

    uint64_t limit_count = 500;
    uint64_t cur_total_item_count = 0;

    uint32_t iter_start = 1;
    uint32_t cur_count = _start_count;

    user_auths user_auth_table(_self, _self.value);
    for (auto iter = user_auth_table.begin(); iter != user_auth_table.end();)
    {
        if (iter_start < _start_count)
        {
            iter_start++;
            iter++;
            continue;
        }
        auto user = user_auth_table.find(iter->primary_key());
        user_servants user_servants_table(_self, user->user.value);
        user_equip_items user_equip_items_table(_self, user->user.value);
        uint64_t count = 0;

        for (auto mail = user_equip_items_table.begin(); mail != user_equip_items_table.end();)
        {
            mail++;
            count++;
        }
        for(auto ser = user_servants_table.begin(); ser != user_servants_table.end();)
        {
            ser++;
            count++;
        }

        if ((cur_total_item_count + count) >= limit_count) //ttemp에 넣기
        {
            global_count global_count_table(_self, _self.value);
            auto g_iter = global_count_table.find(_self.value);
            global_count_table.emplace(_self, [&](auto &new_data) {
                new_data.count = cur_count;
            });
            break;
        }
        else
        {
            for (auto equipment = user_equip_items_table.begin(); equipment != user_equip_items_table.end();)
            {
                auto equipment_iter = user_equip_items_table.find(equipment->primary_key());

                if (equipment_iter->equipment.equipservantindex == 0)
                {
                    equipment++;
                    continue;
                }
                else
                {
                    auto servant_iter = user_servants_table.find(equipment_iter->equipment.equipservantindex);
                    if (servant_iter == user_servants_table.end())
                    {
                        user_equip_items_table.modify(equipment_iter, _self, [&](auto &new_data) {
                            new_data.equipment.equipservantindex = 0;
                        });
                    }
                    else if (servant_iter != user_servants_table.end())
                    {                        
                        if (servant_iter->servant.equip_slot[equipment_iter->equipment.type] == 0)
                        {                            
                            user_servants_table.modify(servant_iter, _self, [&](auto &new_data) {
                                new_data.servant.equip_slot[equipment_iter->equipment.type] = equipment_iter->index;
                            });
                        }
                        else if(servant_iter->servant.equip_slot[equipment_iter->equipment.type] != equipment_iter->index)
                        {
                            user_equip_items_table.modify(equipment_iter, _self, [&](auto &new_data) {
                                new_data.equipment.equipservantindex = 0;
                                new_data.equipment.state = 1;
                            });
                        }
                        
                    }
                equipment++;
                }

           }
            cur_total_item_count += count;
            cur_count += 1;
            iter++;
        }
    }

}