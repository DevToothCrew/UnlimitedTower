// ACTION battletest::dbinsert(std::string _table, std::string _value)
// {
//     system_master system_master_table(_self, _self.value);
//     auto system_master_iter = system_master_table.begin();

//     permission_level master_auth;
//     master_auth.actor = system_master_iter->master;
//     master_auth.permission = "active"_n;
//     require_auth(master_auth);

//     //eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 1");

//     std::vector<size_t> size_list;
//     std::vector<std::string> value_list;
//     uint32_t value;

// // //     // if (_table == "dbbody")
// // //     // {
// // //     //     value = atoll(_value.c_str());
// // //     //     insert_body(value);
// // //     // }
// // //     // if (_table == "dbhair")
// // //     // {
// // //     //     value = atoll(_value.c_str());
// // //     //     insert_hair(value);
// // //     // }
// // //     // if (_table == "dbhead")
// // //     // {
// // //     //     value = atoll(_value.c_str());
// // //     //     insert_head(value);
// // //     // }
// // //     // if (_table == "dbgender")
// // //     // {
// // //     //     value = atoll(_value.c_str());
// // //     //     insert_gender(value);
// // //     // }
//     if (_table == "dbservantjob")
//     {
//         substr_value(_value, value_list, size_list, 4);
//         insert_job(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
//     }
// // //     // else if (_table == "dbitemgrade")
// // //     // {
// // //     //     substr_value(_value, value_list, size_list, 4);
// // //     //     insert_item_grade(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
// // //     // }
// // //     // else if (_table == "dbmonstergd")
// // //     // {
// // //     //     substr_value(_value, value_list, size_list, 4);
// // //     //     insert_monster_grade(value_list[0], atoll(value_list[1].c_str()), atoll(value_list[2].c_str()), atoll(value_list[3].c_str()));
// // //     // }
// // //     // if (_table == "dbgraderatio")
// // //     // {
// // //     //     substr_value(_value, value_list, size_list, 2);
// // //     //     insert_grade_ratio(atoi(value_list[0].c_str()), atoi(value_list[1].c_str()));
// // //     // }

// // //     // if (_table == "dblevel")
// // //     // {
// // //     //     substr_value(_value, value_list, size_list, 3);
// // //     //     insert_level(atoll(value_list[0].c_str()),
// // //     //                  atoi(value_list[1].c_str()),
// // //     //                  atoi(value_list[2].c_str()));
// // //     // }
// // //     // else if (_table == "dbservants")
// // //     // {
// // //     //     substr_value(_value, value_list, size_list, 7);
// // //     //     insert_servant(atoll(value_list[0].c_str()),
// // //     //                    atoll(value_list[1].c_str()),
// // //     //                    atoi(value_list[2].c_str()),
// // //     //                    atoi(value_list[3].c_str()),
// // //     //                    atoi(value_list[4].c_str()),
// // //     //                    atoi(value_list[5].c_str()),
// // //     //                    atoi(value_list[6].c_str()));
// // //     // }

// // //     // if (_table == "dbmonsters")
// // //     // {
// // //     //     substr_value(_value, value_list, size_list, 3);
// // //     //     insert_monster(atoll(value_list[0].c_str()),
// // //     //                    atoll(value_list[1].c_str()),
// // //     //                    atoll(value_list[2].c_str()));
// // //     // }

// //     // if (_table == "dballitem")
// //     // {
// //     //     substr_value(_value, value_list, size_list, 3);
// //     //     insert_all_item_id(atoll(value_list[0].c_str()),
// //     //                        atoi(value_list[1].c_str()),
// //     //                        atoll(value_list[2].c_str()));
// //     // }
// //     // if (_table == "dbequipment")
// //     // {
// //     //     substr_value(_value, value_list, size_list, 6);

// //     //     insert_equip_item_id(atoll(value_list[0].c_str()),
// //     //                          atoll(value_list[1].c_str()),
// //     //                          atoll(value_list[2].c_str()),
// //     //                          atoll(value_list[3].c_str()),
// //     //                          atoll(value_list[4].c_str()),
// //     //                          atoll(value_list[5].c_str()));
// //     // }

// //     //     if (_table == "dbserstat")
// //     //     {
// //     //         substr_value(_value, value_list, size_list, 10);
// //     //         insert_job_stat(atoll(value_list[0].c_str()),
// //     //                         atoi(value_list[1].c_str()),
// //     //                         atoi(value_list[2].c_str()),
// //     //                         atoi(value_list[3].c_str()),
// //     //                         atoi(value_list[4].c_str()),
// //     //                         atoi(value_list[5].c_str()),
// //     //                         atoi(value_list[6].c_str()),
// //     //                         atoi(value_list[7].c_str()),
// //     //                         atoi(value_list[8].c_str()),
// //     //                         atoi(value_list[9].c_str()));
// //     //     }
// //     //     // if (_table == "dbgachapool")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 2);
// //     //     //     insert_gacha_pool(atoll(value_list[0].c_str()),
// //     //     //                       atoll(value_list[1].c_str()));
// //     //     // }

// //     //     // if (_table == "dbprepool")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 2);
// //     //     //     insert_pre_gacha_pool(atoll(value_list[0].c_str()),
// //     //     //                           atoll(value_list[1].c_str()));
// //     //     // }
// //     //     // if (_table == "dbstatusserv")
// //     //     // {
// //     //     //     // substr_value(_value, value_list, size_list, 2);
// //     //     //     // insert_servant_lv_status(atoll(value_list[0].c_str()),
// //     //     //     //                   atoll(value_list[1].c_str()));
// //     //     //     servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
// //     //     //     servant_lv_status_db_table.emplace(_self, [&](auto &new_data) {
// //     //     //         new_data.grade = 5;
// //     //     //         for (uint32_t i = 0; i < servant_status_list.size(); ++i)
// //     //     //         {
// //     //     //             lv_status_sub data;
// //     //     //             data.pre_status = i;
// //     //     //             data.update_status = servant_status_list[i];
// //     //     //             new_data.change_status.push_back(data);
// //     //     //         }
// //     //     //     });
// //     //     // }

// //     //     // if (_table == "dbstatusmon")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 11);
// //     //     //     std::vector<uint32_t> grade_list;
// //     //     //     grade_list.push_back(atoi(value_list[1].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[2].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[3].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[4].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[5].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[6].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[7].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[8].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[9].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[10].c_str()));

// //     //     //     insert_status_monster(atoll(value_list[0].c_str()),grade_list);
// //     //     // }

// //     //     // if (_table == "dbstatusequi")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 11);
// //     //     //     std::vector<uint32_t> grade_list;
// //     //     //     grade_list.push_back(atoi(value_list[1].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[2].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[3].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[4].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[5].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[6].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[7].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[8].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[9].c_str()));
// //     //     //     grade_list.push_back(atoi(value_list[10].c_str()));

// //     //     //     insert_status_equipment(atoll(value_list[0].c_str()), grade_list);
// //     //     // }

// //     //     if (_table == "dbstageenemy")
// //     //     {
// //     //         substr_value(_value, value_list, size_list, 11);
// //     //         insert_stage_enemy(atoll(value_list[0].c_str()),
// //     //                            atoi(value_list[1].c_str()),
// //     //                            atoi(value_list[2].c_str()),
// //     //                            atoi(value_list[3].c_str()),
// //     //                            atoi(value_list[4].c_str()),
// //     //                            atoi(value_list[5].c_str()),
// //     //                            atoi(value_list[6].c_str()),
// //     //                            atoi(value_list[7].c_str()),
// //     //                            atoi(value_list[8].c_str()),
// //     //                            atoi(value_list[9].c_str()),
// //     //                            atoi(value_list[10].c_str()));
// //     //     }

// //     //     // if (_table == "dbserstat")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 10);
// //     //     //     insert_job_stat(atoll(value_list[0].c_str()),
// //     //     //                     atoi(value_list[1].c_str()),
// //     //     //                     atoi(value_list[2].c_str()),
// //     //     //                     atoi(value_list[3].c_str()),
// //     //     //                     atoi(value_list[4].c_str()),
// //     //     //                     atoi(value_list[5].c_str()),
// //     //     //                     atoi(value_list[6].c_str()),
// //     //     //                     atoi(value_list[7].c_str()),
// //     //     //                     atoi(value_list[8].c_str()),
// //     //     //                     atoi(value_list[9].c_str()));
// //     //     // }
// //     //     // if (_table == "dbtribe")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 11);
// //     //     //     insert_tribe_stat(atoll(value_list[0].c_str()),
// //     //     //                       atoi(value_list[1].c_str()),
// //     //     //                       atoi(value_list[2].c_str()),
// //     //     //                       atoi(value_list[3].c_str()),
// //     //     //                       atoi(value_list[4].c_str()),
// //     //     //                       atoi(value_list[5].c_str()),
// //     //     //                       atoi(value_list[6].c_str()),
// //     //     //                       atoi(value_list[7].c_str()),
// //     //     //                       atoi(value_list[8].c_str()),
// //     //     //                       atoi(value_list[9].c_str()),
// //     //     //                       atoi(value_list[10].c_str()));
// //     //     // }
// //     //     // if (_table == "dbtype")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 8);
// //     //     //     insert_type(atoll(value_list[0].c_str()),
// //     //     //                       atoi(value_list[1].c_str()),
// //     //     //                       atoi(value_list[2].c_str()),
// //     //     //                       atoi(value_list[3].c_str()),
// //     //     //                       atoi(value_list[4].c_str()),
// //     //     //                       atoi(value_list[5].c_str()),
// //     //     //                       atoi(value_list[6].c_str()),
// //     //     //                       atoi(value_list[7].c_str()));
// //     //     // }
// //     //     // if (_table == "dbmoupstat")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 6);
// //     //     //     insert_monster_upgrade(atoll(value_list[0].c_str()),
// //     //     //                            atoi(value_list[1].c_str()),
// //     //     //                            atoi(value_list[2].c_str()),
// //     //     //                            atoi(value_list[3].c_str()),
// //     //     //                            atoi(value_list[4].c_str()),
// //     //     //                            atoi(value_list[5].c_str()));
// //     //     // }
// //     //     // if (_table == "dbequpstat")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 6);
// //     //     //     insert_equipment_upgrade(atoll(value_list[0].c_str()),
// //     //     //                              atoi(value_list[1].c_str()),
// //     //     //                              atoi(value_list[2].c_str()),
// //     //     //                              atoi(value_list[3].c_str()),
// //     //     //                              atoi(value_list[4].c_str()),
// //     //     //                              atoi(value_list[5].c_str()));
// //     //     // }
// //     //     // if (_table == "dbpassive")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 9);
// //     //     //     insert_passive(atoll(value_list[0].c_str()),
// //     //     //                    atoi(value_list[1].c_str()),
// //     //     //                    atoi(value_list[2].c_str()),
// //     //     //                    atoi(value_list[3].c_str()),
// //     //     //                    atoi(value_list[4].c_str()),
// //     //     //                    atoi(value_list[5].c_str()),
// //     //     //                    atoi(value_list[6].c_str()),
// //     //     //                    atoi(value_list[7].c_str()),
// //     //     //                    atoi(value_list[8].c_str()));
// //     //     // }
// //     //     // if (_table == "dbactive")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 19);
// //     //     //     insert_active(atoll(value_list[0].c_str()),
// //     //     //                   atoi(value_list[1].c_str()),
// //     //     //                   atoi(value_list[2].c_str()),
// //     //     //                   atoi(value_list[3].c_str()),
// //     //     //                   atoi(value_list[4].c_str()),
// //     //     //                   atoi(value_list[5].c_str()),
// //     //     //                   atoi(value_list[6].c_str()),
// //     //     //                   atoi(value_list[7].c_str()),
// //     //     //                   atoi(value_list[8].c_str()),
// //     //     //                   atoi(value_list[9].c_str()),
// //     //     //                   atoi(value_list[10].c_str()),
// //     //     //                   atoi(value_list[11].c_str()),
// //     //     //                   atoi(value_list[12].c_str()),
// //     //     //                   atoi(value_list[13].c_str()),
// //     //     //                   atoi(value_list[14].c_str()),
// //     //     //                   atoi(value_list[15].c_str()),
// //     //     //                   atoi(value_list[16].c_str()),
// //     //     //                   atoi(value_list[17].c_str()),
// //     //     //                   atoi(value_list[18].c_str()));
// //     //     // }
// //     if (_table == "dbgachapool")
// //     {
// //         substr_value(_value, value_list, size_list, 2);
// //         insert_gacha_pool(atoll(value_list[0].c_str()),
// //                           atoll(value_list[1].c_str()));
// //     }
// //     if (_table == "dbenemystat")
// //     {
// //         substr_value(_value, value_list, size_list, 4);
// //         std::vector<uint32_t> info;
// //         for (uint32_t i = 0; i < value_list.size(); ++i)
// //         {
// //             info.push_back(atoi(value_list[i].c_str()));
// //         }
// //         insert_enemy_stat(info);
// //     }
// //         if (_table == "dbenemyinfo")
// //     {
// //         substr_value(_value, value_list, size_list, 6);
// //         std::vector<uint32_t> info;
// //         for (uint32_t i = 0; i < value_list.size(); ++i)
// //         {
// //             info.push_back(atoi(value_list[i].c_str()));
// //         }
// //         insert_enemy_info(info);
// //     }

// //     //     // if (_table == "dbprepool")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 2);
// //     //     //     insert_pre_gacha_pool(atoll(value_list[0].c_str()),
// //     //     //                       atoll(value_list[1].c_str()));
// //     //     // }

// //     //     // if (_table == "dbitemshop")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 7);
// //     //     //     insert_itemshop(atoll(value_list[0].c_str()),
// //     //     //                     atoi(value_list[1].c_str()),
// //     //     //                     atoi(value_list[2].c_str()),
// //     //     //                     atoi(value_list[3].c_str()),
// //     //     //                     atoi(value_list[4].c_str()),
// //     //     //                     atoi(value_list[5].c_str()),
// //     //     //                     atoi(value_list[6].c_str()));
// //     //     // }
// //     //     // if (_table == "tshoplist")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 4);
// //     //     //     insert_shoplist(atoll(value_list[0].c_str()),
// //     //     //                     atoi(value_list[1].c_str()),
// //     //     //                     atoi(value_list[2].c_str()),
// //     //     //                     atoi(value_list[3].c_str()));
// //     //     // }
// //     //     // if (_table == "dbmonsterup")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 2);
// //     //     //     insert_upgrade_monster_ratio(atoll(value_list[0].c_str()),
// //     //     //                                  atoll(value_list[1].c_str()));
// //     //     // }
// //     //     // if (_table == "dbburn")
// //     //     // {
// //     //     //     substr_value(_value, value_list, size_list, 2);
// //     //     //     insert_servant_burn_item(atoll(value_list[0].c_str()),
// //     //     //                               atoll(value_list[1].c_str()));
// //     //     // }
// //     //     // else if (_table == "dbitemup")
// //     //     // {
// //     //     // }
// // }

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
//         new_reward_db reward_db_table(_self, _self.value);
//         auto stage_db_iter = reward_db_table.find(stage_id);
//         eosio_assert(stage_db_iter != reward_db_table.end(), "DB List Insert : Not Exist Reward");

//         reward_db_table.modify(stage_db_iter, _self, [&](auto &new_data) {
//             for (uint32_t i = 0; i < _value_list.size();)
//             {
//                 reward_item_info new_reward;
//                 new_reward.id = atoi(_value_list[i].c_str());
//                 new_reward.per = atoi(_value_list[i + 1].c_str());
//                 new_reward.count = atoi(_value_list[i + 2].c_str());
//                 new_data.reward_list.push_back(new_reward);
//                 i += 3;
//             }
//         });
//     }
// }


// void battletest::insert_status_servant(uint64_t _grade, std::vector<uint32_t> _status_list)
// {
//     servant_lv_status_db monster_lv_status_db_table(_self, _self.value);
//     auto iter = monster_lv_status_db_table.find(_grade);
//     if(iter == monster_lv_status_db_table.end())
//     {
//         monster_lv_status_db_table.emplace(_self, [&](auto &new_data)
//         {
//             new_data.grade = _grade;
//             for(uint32_t i = 0; i < _status_list.size(); ++i)
//             {
//                 lv_status_sub sub;
//                 sub.pre_status = i;
//                 sub.update_status = _status_list[i];
//                 new_data.change_status.push_back(sub);
//             }
//         });
//     }
//     else
//     {
//         monster_lv_status_db_table.modify(iter, _self, [&](auto &new_data) {
//             for (uint32_t i = 0; i < _status_list.size(); ++i)
//             {
//                 new_data.change_status[i].update_status = _status_list[i];
//             }
//         });
//     }
// }


// void battletest::insert_status_monster(uint64_t _grade, std::vector<uint32_t> _status_list)
// {
//     monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
//     auto iter = monster_lv_status_db_table.find(_grade);
//     if(iter == monster_lv_status_db_table.end())
//     {
//         monster_lv_status_db_table.emplace(_self, [&](auto &new_data)
//         {
//             new_data.grade = _grade;
//             for(uint32_t i = 0; i < _status_list.size(); ++i)
//             {
//                 lv_status_sub sub;
//                 sub.pre_status = i;
//                 sub.update_status = _status_list[i];
//                 new_data.change_status.push_back(sub);
//             }
//         });
//     }
//     else
//     {
//         monster_lv_status_db_table.modify(iter, _self, [&](auto &new_data) {
//             for (uint32_t i = 0; i < _status_list.size(); ++i)
//             {
//                 new_data.change_status[i].update_status = _status_list[i];
//             }
//         });
//     }
// }

// void battletest::insert_status_equipment(uint64_t _grade, std::vector<uint32_t> _status_list)
// {
//     equipment_lv_status_db monster_lv_status_db_table(_self, _self.value);
//     auto iter = monster_lv_status_db_table.find(_grade);
//     if(iter == monster_lv_status_db_table.end())
//     {
//         monster_lv_status_db_table.emplace(_self, [&](auto &new_data)
//         {
//             new_data.type_grade = _grade;
//             for(uint32_t i = 0; i < _status_list.size(); ++i)
//             {
//                 lv_status_sub sub;
//                 sub.pre_status = i;
//                 sub.update_status = _status_list[i];
//                 new_data.change_status.push_back(sub);
//             }
//         });
//     }
//     else
//     {
//         monster_lv_status_db_table.modify(iter, _self, [&](auto &new_data) {
//             for (uint32_t i = 0; i < _status_list.size(); ++i)
//             {
//                 new_data.change_status[i].update_status = _status_list[i];
//             }
//         });
//     }
// }

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

// void battletest::insert_body(uint64_t _appear)
// {
//     body_db body_db_table(_self, _self.value);
//     body_db_table.emplace(_self, [&](auto &new_body) {
//         new_body.body = _appear;
//     });
// }

// void battletest::insert_hair(uint64_t _appear)
// {
//     hair_db hair_db_table(_self, _self.value);
//     hair_db_table.emplace(_self, [&](auto &new_hair) {
//         new_hair.hair = _appear;
//     });
// }
// void battletest::insert_head(uint64_t _appear)
// {
//     head_db head_db_table(_self, _self.value);
//     head_db_table.emplace(_self, [&](auto &new_head) {
//         new_head.head = _appear;
//     });
// }

// void battletest::insert_gender(uint64_t _appear)
// {
//     gender_db gender_db_table(_self, _self.value);
//     gender_db_table.emplace(_self, [&](auto &new_gender) {
//         new_gender.gender = _appear;
//     });
// }

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

// void battletest::insert_monster(uint64_t _monster_id, uint64_t _tribe, uint64_t _type)
// {
//     monster_db monster_id_db_table(_self, _self.value);
//     auto monster_id_iter = monster_id_db_table.find(_monster_id);
//     if (monster_id_iter == monster_id_db_table.end())
//     {
//         monster_id_db_table.emplace(_self, [&](auto &new_monster_id) {
//             new_monster_id.id = _monster_id;
//             new_monster_id.tribe = _tribe;
//             new_monster_id.type = _type;
//         });
//     }
//     else
//     {
//         monster_id_db_table.modify(monster_id_iter, _self, [&](auto &new_monster_id) {
//             new_monster_id.tribe = _tribe;
//             new_monster_id.type = _type;
//         });
//     }
// }

void battletest::insert_equip_item_id(uint64_t _item_id,
                                      uint64_t _item_set_id,
                                      uint64_t _type,
                                      uint64_t _tier,
                                      uint64_t _job,
                                      uint64_t _option)
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
        });
    }
}

// void battletest::insert_monster_grade(std::string _status, uint64_t _grade, uint64_t _min, uint64_t _max)
// {
//     monster_grade_db monster_grade_db_table(_self, _self.value);
//     auto monster_grade_iter = monster_grade_db_table.find(_grade);
//     if (monster_grade_iter == monster_grade_db_table.end())
//     {
//         monster_grade_db_table.emplace(_self, [&](auto &new_monster_grade) {
//             new_monster_grade.grade = _grade;
//             if (_status == "all")
//             {
//                 new_monster_grade.min_range.base_dex = _min;
//                 new_monster_grade.min_range.base_int = _min;
//                 new_monster_grade.min_range.base_str = _min;
//                 new_monster_grade.max_range.base_dex = _max;
//                 new_monster_grade.max_range.base_int = _max;
//                 new_monster_grade.max_range.base_str = _max;
//             }
//             else if (_status == "str")
//             {
//                 new_monster_grade.max_range.base_str = _max;
//                 new_monster_grade.min_range.base_str = _min;
//             }
//             else if (_status == "dex")
//             {
//                 new_monster_grade.max_range.base_dex = _max;
//                 new_monster_grade.min_range.base_dex = _min;
//             }
//             else if (_status == "int")
//             {
//                 new_monster_grade.max_range.base_int = _max;
//                 new_monster_grade.min_range.base_int = _min;
//             }
//         });
//     }
//     else
//     {
//         monster_grade_db_table.modify(monster_grade_iter, _self, [&](auto &new_monster_grade) {
//             if (_status == "all")
//             {
//                 new_monster_grade.min_range.base_dex = _min;
//                 new_monster_grade.min_range.base_int = _min;
//                 new_monster_grade.min_range.base_str = _min;
//                 new_monster_grade.max_range.base_dex = _max;
//                 new_monster_grade.max_range.base_int = _max;
//                 new_monster_grade.max_range.base_str = _max;
//             }
//             else if (_status == "str")
//             {
//                 new_monster_grade.max_range.base_str = _max;
//                 new_monster_grade.min_range.base_str = _min;
//             }
//             else if (_status == "dex")
//             {
//                 new_monster_grade.max_range.base_dex = _max;
//                 new_monster_grade.min_range.base_dex = _min;
//             }
//             else if (_status == "int")
//             {
//                 new_monster_grade.max_range.base_int = _max;
//                 new_monster_grade.min_range.base_int = _min;
//             }
//         });
//     }
// }

// void battletest::insert_all_item_id(uint64_t _item_id, uint32_t _type, uint32_t _grade, uint64_t _max_count, uint64_t _sell_item_id, uint64_t _sell_item_count)
// {
//     allitem_db item_id_db_table(_self, _self.value);
//     auto item_id_iter = item_id_db_table.find(_item_id);
//     if (item_id_iter == item_id_db_table.end())
//     {
//         item_id_db_table.emplace(_self, [&](auto &new_item_id) {
//             new_item_id.id = _item_id;
//             new_item_id.type = _type;
//             new_item_id.grade = _grade;
//             new_item_id.max_count = _max_count;
//             new_item_id.sell_item_id = _sell_item_id;
//             new_item_id.sell_item_count = _sell_item_count;
//         });
//     }
//     else
//     {
//         item_id_db_table.modify(item_id_iter, _self, [&](auto &new_item_id) {
//             new_item_id.id = _item_id;
//             new_item_id.type = _type;
//             new_item_id.grade = _grade;
//             new_item_id.max_count = _max_count;
//             new_item_id.sell_item_id = _sell_item_id;
//             new_item_id.sell_item_count = _sell_item_count;
//         });
//     }
// }
 

// void battletest::insert_grade_ratio(uint64_t _grade, uint64_t _ratio)
// {
//     grade_ratio_db grade_ratio_db_table(_self, _self.value);
//     auto grade_ratio_iter = grade_ratio_db_table.find(_grade);
//     if (grade_ratio_iter == grade_ratio_db_table.end())
//     {
//         grade_ratio_db_table.emplace(_self, [&](auto &new_grade_ratio) {
//             new_grade_ratio.grade = _grade;
//             new_grade_ratio.ratio = _ratio;
//         });
//     }
//     else
//     {
//         grade_ratio_db_table.modify(grade_ratio_iter, _self, [&](auto &new_grade_ratio) {
//             new_grade_ratio.ratio = _ratio;
//         });
//     }
// }

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

// // ACTION battletest::insertequipr(uint64_t _main, std::vector<uint64_t> &_upgrade_ratio, uint64_t _material_id, std::vector<uint64_t> &_material_count, std::vector<uint64_t> &_use_UTG)
// // {
// //     system_master system_master_table(_self, _self.value);
// //     auto system_master_iter = system_master_table.begin();

// //     permission_level master_auth;
// //     master_auth.actor = system_master_iter->master;
// //     master_auth.permission = "active"_n;
// //     require_auth(master_auth);

// //     upgrade_equipment_ratio_dbs upgrade_equipment_ratio_db_table(_self, _self.value);
// //     auto upgrade_equipment_ratio_db_iter = upgrade_equipment_ratio_db_table.find(_main);
// //     if (upgrade_equipment_ratio_db_iter == upgrade_equipment_ratio_db_table.end())
// //     {
// //         upgrade_equipment_ratio_db_table.emplace(_self, [&](auto &new_upgrade_equipment_iter) {
// //             new_upgrade_equipment_iter.equipment_type_grade = _main;
// //             new_upgrade_equipment_iter.upgrade_ratio = _upgrade_ratio;
// //             new_upgrade_equipment_iter.material_id = _material_id;
// //             new_upgrade_equipment_iter.material_count = _material_count;
// //             new_upgrade_equipment_iter.use_UTG = _use_UTG;
// //         });
// //     }
// //     else
// //     {
// //         upgrade_equipment_ratio_db_table.modify(upgrade_equipment_ratio_db_iter, _self, [&](auto &new_upgrade_equipment_iter) {
// //             new_upgrade_equipment_iter.equipment_type_grade = _main;
// //             new_upgrade_equipment_iter.upgrade_ratio = _upgrade_ratio;
// //             new_upgrade_equipment_iter.material_id = _material_id;
// //             new_upgrade_equipment_iter.material_count = _material_count;
// //             new_upgrade_equipment_iter.use_UTG = _use_UTG;
// //         });
// //     }
// // }

// void battletest::insert_level(uint32_t _level, uint32_t _rank_exp, uint32_t _char_exp)
// {
//     lv_exp lv_exp_table(_self, _self.value);
//     auto iter = lv_exp_table.find(_level);
//     if(iter ==  lv_exp_table.end())
//     {
//         lv_exp_table.emplace(_self, [&](auto &new_lv_exp) {
//             new_lv_exp.lv = _level;
//             new_lv_exp.rank_exp = _rank_exp;
//             new_lv_exp.char_exp = _char_exp;
//         });
//     }
//     else
//     {
//         lv_exp_table.modify(iter, _self, [&](auto &new_lv_exp) {
//             new_lv_exp.lv = _level;
//             new_lv_exp.rank_exp = _rank_exp;
//             new_lv_exp.char_exp = _char_exp;
//         });
//     }
// }




// void battletest::insert_passive(uint64_t _passive_id, uint32_t _passive_type, uint32_t _job_class, uint32_t _enable_stack_max, uint32_t _effect_id,
//                                 uint32_t _effect_type_id, uint32_t _effect_value_a, uint32_t _effect_value_add_b, uint32_t _target_id)
// {
//     passive_db passive_db_table(_self, _self.value);
//     auto passive_db_iter = passive_db_table.find(_passive_id);
//     if (passive_db_iter == passive_db_table.end())
//     {
//         passive_db_table.emplace(_self, [&](auto &new_passive) {
//             new_passive.passive_id = _passive_id;
//             new_passive.passive_type = _passive_type;
//             new_passive.job_class = _job_class;
//             new_passive.enable_stack_max = _enable_stack_max;
//             new_passive.effect_id = _effect_id;
//             new_passive.effect_type_id = _effect_type_id;
//             new_passive.effect_value_a = _effect_value_a;
//             new_passive.effect_value_add_b = _effect_value_add_b;
//             new_passive.target_id = _target_id;
//         });
//     }
//     else
//     {
//         passive_db_table.modify(passive_db_iter, _self, [&](auto &new_passive) {
//             new_passive.passive_type = _passive_type;
//             new_passive.job_class = _job_class;
//             new_passive.enable_stack_max = _enable_stack_max;
//             new_passive.effect_id = _effect_id;
//             new_passive.effect_type_id = _effect_type_id;
//             new_passive.effect_value_a = _effect_value_a;
//             new_passive.effect_value_add_b = _effect_value_add_b;
//             new_passive.target_id = _target_id;
//         });
//     }
// }

// void battletest::insert_active(uint64_t _active_id,  uint32_t _job, uint32_t _tribe, uint32_t _active_per,
//                                uint32_t _skill_type, uint32_t _active_turn, uint32_t _attack_type, uint32_t _dmg_type, uint32_t _elemental_type, uint32_t _target, uint32_t _target_count, 
//                                uint32_t _hit_count, uint32_t _atk_per, uint32_t _atk_per_add, uint32_t _atk_per_2, uint32_t _atk_per_add_2, uint32_t _heal_per, uint32_t _heal_per_add,uint32_t _option_id)
// {
//     active_db active_db_table(_self, _self.value);
//     auto active_db_iter = active_db_table.find(_active_id);
//     if (active_db_iter == active_db_table.end())
//     {
//         active_db_table.emplace(_self, [&](auto &new_active) {
//             new_active.active_id = _active_id;
//             new_active.job = _job;
//             new_active.tribe = _tribe;
//             new_active.active_per = _active_per;
//             new_active.skill_type = _skill_type;
//             new_active.attack_type = _attack_type;
//             new_active.dmg_type = _dmg_type;
//             new_active.elemental_type = _elemental_type;
//             new_active.target_type = _target;
//             new_active.target_count = _target_count;
//             new_active.active_turn = _active_turn;
//             new_active.hit_count = _hit_count;
//             new_active.atk_per_1 = _atk_per;
//             new_active.atk_per_add_1 = _atk_per_add;
//             new_active.atk_per_2 = _atk_per_2;
//             new_active.atk_per_add_2= _atk_per_add_2;
//             new_active.heal_per = _heal_per;
//             new_active.heal_per_add = _heal_per_add;
//             new_active.option_id = _option_id;
//         });
//     }
//     else
//     {
//         active_db_table.modify(active_db_iter, _self, [&](auto &new_active) {
//             new_active.job = _job;
//             new_active.tribe = _tribe;
//             new_active.active_per = _active_per;
//             new_active.skill_type = _skill_type;
//             new_active.attack_type = _attack_type;
//             new_active.dmg_type = _dmg_type;
//             new_active.elemental_type = _elemental_type;
//             new_active.target_type = _target;
//             new_active.target_count = _target_count;
//             new_active.active_turn = _active_turn;
//             new_active.hit_count = _hit_count;
//             new_active.atk_per_1 = _atk_per;
//             new_active.atk_per_add_1 = _atk_per_add;
//             new_active.atk_per_2 = _atk_per_2;
//             new_active.atk_per_add_2= _atk_per_add_2;
//             new_active.heal_per = _heal_per;
//             new_active.heal_per_add = _heal_per_add;
//             new_active.option_id = _option_id;
//         });
//     }
// }

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

// void battletest::insert_pre_gacha_pool(uint64_t _gacha_id, uint64_t _db_index)
// {
//     pre_gacha_db pre_gacha_db_table(_self, _self.value);
//     auto pre_gacha_db_iter = pre_gacha_db_table.find(_gacha_id);
//     if (pre_gacha_db_iter == pre_gacha_db_table.end())
//     {
//         pre_gacha_db_table.emplace(_self, [&](auto &new_gacha) {
//             new_gacha.gacha_id = _gacha_id;
//             new_gacha.db_index = _db_index;
//         });
//     }
//     else
//     {
//         pre_gacha_db_table.modify(pre_gacha_db_iter, _self, [&](auto &new_gacha) {
//             new_gacha.db_index = _db_index;
//         });
//     }
// }

void battletest::insert_gold_gacha_pool(uint64_t _gacha_id, uint64_t _db_index)
{
    gold_gacha_db gold_gacha_db_table(_self, _self.value);
    auto gold_gacha_db_iter = gold_gacha_db_table.find(_gacha_id);
    if (gold_gacha_db_iter == gold_gacha_db_table.end())
    {
        gold_gacha_db_table.emplace(_self, [&](auto &new_gacha) {
            new_gacha.gacha_id = _gacha_id;
            new_gacha.db_index = _db_index;
        });
    }
    else
    {
        gold_gacha_db_table.modify(gold_gacha_db_iter, _self, [&](auto &new_gacha) {
            new_gacha.db_index = _db_index;
        });
    }
}


// void battletest::insert_tribe_stat(uint64_t _id,
//                                     uint32_t _base_str,
//                                     uint32_t _base_dex,
//                                     uint32_t _base_int,
//                                    uint32_t _speed,
//                                    uint32_t _avoid,
//                                    uint32_t _cri_per,
//                                    uint32_t _cri_dmg,
//                                    uint32_t _pair,
//                                    uint32_t _triple,
//                                    uint32_t _penta)
// {
//     tribe_db class_stat_db_table(_self, _self.value);
//     auto class_iter = class_stat_db_table.find(_id);
//     if (class_iter == class_stat_db_table.end())
//     {
//         class_stat_db_table.emplace(_self, [&](auto &new_data) {
//             new_data.id = _id;
//             new_data.base_str = _base_str;
//             new_data.base_dex = _base_dex;
//             new_data.base_int = _base_int;
//             new_data.speed = _speed;
//             new_data.avoid = _avoid;
//             new_data.cri_per = _cri_per;
//             new_data.cri_dmg = _cri_dmg;
//             new_data.pair = _pair;
//             new_data.triple = _triple;
//             new_data.penta = _penta;
//         });
//     }
//     else
//     {
//         class_stat_db_table.modify(class_iter, _self, [&](auto &new_data) {
//             new_data.base_str = _base_str;
//             new_data.base_dex = _base_dex;
//             new_data.base_int = _base_int;
//             new_data.speed = _speed;
//             new_data.avoid = _avoid;
//             new_data.cri_per = _cri_per;
//             new_data.cri_dmg = _cri_dmg;
//             new_data.pair = _pair;
//             new_data.triple = _triple;
//             new_data.penta = _penta;
//         });
//     }
// }

// void battletest::insert_job_stat(uint64_t _id,
//                                 uint32_t _job,
//                                 uint32_t _grade,
//                                  uint32_t _base_str,
//                                  uint32_t _base_dex,
//                                  uint32_t _base_int,
//                                  uint32_t _speed,
//                                  uint32_t _avoid,
//                                  uint32_t _cri_per,
//                                  uint32_t _cri_dmg)
// {
//     serstat_db class_stat_db_table(_self, _self.value);
//     auto class_iter = class_stat_db_table.find(_id);
//     if (class_iter == class_stat_db_table.end())
//     {
//         class_stat_db_table.emplace(_self, [&](auto &new_data) {
//             new_data.id = _id;
//             new_data.job = _job;
//             new_data.grade = _grade;
//             new_data.base_str = _base_str;
//             new_data.base_dex = _base_dex;
//             new_data.base_int = _base_int;
//             new_data.speed = _speed;
//             new_data.avoid = _avoid;
//             new_data.cri_per = _cri_per;
//             new_data.cri_dmg = _cri_dmg;
//         });
//     }
//     else
//     {
//         class_stat_db_table.modify(class_iter, _self, [&](auto &new_data) {
//             new_data.job = _job;
//             new_data.grade = _grade;
//             new_data.base_str = _base_str;
//             new_data.base_dex = _base_dex;
//             new_data.base_int = _base_int;
//             new_data.speed = _speed;
//             new_data.avoid = _avoid;
//             new_data.cri_per = _cri_per;
//             new_data.cri_dmg = _cri_dmg;
//         });
//     }
// }

// void battletest::insert_type(uint64_t _id, uint32_t _strong, uint32_t _weak, uint32_t _strong_per, uint32_t _weak_per, uint32_t _pair, uint32_t _triple, uint32_t _penta)
// {
//     type_db my_table(_self, _self.value);
//     auto class_iter = my_table.find(_id);
//     if (class_iter == my_table.end())
//     {
//         my_table.emplace(_self, [&](auto &new_data) {
//             new_data.id = _id;
//             new_data.strong = _strong;
//             new_data.weak = _weak;
//             new_data.strong_per = _strong_per;
//             new_data.weak_per = _weak_per;
//             new_data.pair = _pair;
//             new_data.triple = _triple;
//             new_data.penta = _penta;
//         });
//     }
//     else
//     {
//         my_table.modify(class_iter, _self, [&](auto &new_data) {
//             new_data.strong = _strong;
//             new_data.weak = _weak;
//             new_data.strong_per = _strong_per;
//             new_data.weak_per = _weak_per;
//             new_data.pair = _pair;
//             new_data.triple = _triple;
//             new_data.penta = _penta;
//         });
//     }
// }
// void battletest::insert_monster_upgrade(uint64_t _upgrade, uint32_t _common, uint32_t _uncommon, uint32_t _rare, uint32_t _unique, uint32_t _legendary)
// {
//     upgrade_monster_db upgrade_db_table(_self, _self.value);
//     auto upgrade_iter = upgrade_db_table.find(_upgrade);
//     if(upgrade_iter == upgrade_db_table.end())
//     {
//         upgrade_db_table.emplace(_self, [&](auto &new_data)
//         {
//             new_data.upgrade = _upgrade;
//             new_data.grade_list.resize(5);
//             new_data.grade_list[0] = _common;
//             new_data.grade_list[1] = _uncommon;
//             new_data.grade_list[2] = _rare;
//             new_data.grade_list[3] = _unique;
//             new_data.grade_list[4] = _legendary;
//         });
//     }
//     else
//     {
//         upgrade_db_table.modify(upgrade_iter ,_self, [&](auto &new_data)
//         {
//             new_data.grade_list[0] = _common;
//             new_data.grade_list[1] = _uncommon;
//             new_data.grade_list[2] = _rare;
//             new_data.grade_list[3] = _unique;
//             new_data.grade_list[4] = _legendary;
//         });
//     }
// }

// void battletest::insert_equipment_upgrade(uint64_t _upgrade, uint32_t _common, uint32_t _uncommon, uint32_t _rare, uint32_t _unique, uint32_t _legendary)
// {
//     upgrade_equipment_db upgrade_db_table(_self, _self.value);
//     auto upgrade_iter = upgrade_db_table.find(_upgrade);
//     if(upgrade_iter == upgrade_db_table.end())
//     {
//         upgrade_db_table.emplace(_self, [&](auto &new_data)
//         {
//             new_data.upgrade = _upgrade;
//             new_data.grade_list.resize(5);
//             new_data.grade_list[0] = _common;
//             new_data.grade_list[1] = _uncommon;
//             new_data.grade_list[2] = _rare;
//             new_data.grade_list[3] = _unique;
//             new_data.grade_list[4] = _legendary;
//         });
//     }
//     else
//     {
//         upgrade_db_table.modify(upgrade_iter ,_self, [&](auto &new_data)
//         {
//             new_data.grade_list[0] = _common;
//             new_data.grade_list[1] = _uncommon;
//             new_data.grade_list[2] = _rare;
//             new_data.grade_list[3] = _unique;
//             new_data.grade_list[4] = _legendary;
//         });
//     }
// }


void battletest::insert_itemshop(uint64_t _id, uint64_t _shop_type, uint64_t _product_id, uint64_t _product_count,
                         uint64_t _product_limit_max, uint64_t _price_id, uint64_t _price_count)
{
    item_shop item_table_shop_table(_self, _self.value);
    auto item_table_shop_iter = item_table_shop_table.find(_id);

    if (item_table_shop_iter == item_table_shop_table.end())
    {
        item_table_shop_table.emplace(_self, [&](auto &new_data) {
            new_data.id = _id;
            new_data.shop_type = _shop_type;
            new_data.product_id = _product_id;
            new_data.product_count = _product_count;
            new_data.product_limit_max = _product_limit_max;
            new_data.price_id = _price_id;
            new_data.price_count = _price_count;
        });
    }
    else
    {
        item_table_shop_table.modify(item_table_shop_iter, _self, [&](auto &new_data) {
            new_data.shop_type = _shop_type;
            new_data.product_id = _product_id;
            new_data.product_count = _product_count;
            new_data.product_limit_max = _product_limit_max;
            new_data.price_id = _price_id;
            new_data.price_count = _price_count;
        });
    }    
}

// void battletest::insert_shoplist(uint64_t _id, uint64_t _shop_type, uint64_t _shop_item_id, uint64_t _limit_count)
// {
//     shop_list shop_list_table(_self, _self.value);
//     auto shop_list_iter = shop_list_table.find(_id);
//     if(shop_list_iter == shop_list_table.end())
//     {
//         shop_list_table.emplace(_self, [&](auto &new_data){
//             new_data.id = _id;
//             new_data.shop_type = _shop_type;
//             new_data.shop_item_id = _shop_item_id;
//             new_data.limit_count = _limit_count;
//         });
//     }
//     else
//     {
//         shop_list_table.modify(shop_list_iter, _self, [&](auto &new_data){
//             new_data.shop_type = _shop_type;
//             new_data.shop_item_id = _shop_item_id;
//             new_data.limit_count = _limit_count;
//         });
//     }
// }

void battletest::insert_packageshop(uint64_t _id, uint64_t _get_utg, uint64_t _limit_count, uint64_t _price_id, uint64_t _price_count)
{
    package_shop package_shop_table(_self, _self.value);
    auto package_shop_iter = package_shop_table.find(_id);
    if(package_shop_iter == package_shop_table.end())
    {
        package_shop_table.emplace(_self, [&](auto &new_data){
            new_data.id = _id;
            new_data.GET_UTG = _get_utg;
            new_data.private_limit_max = _limit_count;
            new_data.price_id =  _price_id;
            new_data.price_count = _price_count;

        });
    }
    else
    {
        package_shop_table.modify(package_shop_iter, _self, [&](auto &new_data) {
            new_data.GET_UTG = _get_utg;
            new_data.private_limit_max = _limit_count;
            new_data.price_id = _price_id;
            new_data.price_count = _price_count;
        });
    }
}

// void battletest::insert_servant_burn_item(uint64_t _servant_job, uint64_t _result_item_id)
// {
//     burnitem_db burnitem_db_table(_self, _self.value);
//     auto iter = burnitem_db_table.find(_servant_job);
//     if(iter == burnitem_db_table.end())
//     {
//         burnitem_db_table.emplace(_self, [&](auto &new_data)
//         {
//             new_data.servant_job = _servant_job;
//             new_data.result_item_id = _result_item_id;
//         });
//     }
//     else
//     {
//         burnitem_db_table.modify(iter, _self, [&](auto &new_data) {
//             new_data.result_item_id = _result_item_id;
//         });
//     }
// }


// // ACTION battletest::dberase(std::string _table, std::string _value)
// // {
// //     system_master system_master_table(_self, _self.value);
// //     auto system_master_iter = system_master_table.begin();

// //     permission_level master_auth;
// //     master_auth.actor = system_master_iter->master;
// //     master_auth.permission = "active"_n;
// //     require_auth(master_auth);

// //     eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 3");

// //     std::vector<size_t> size_list;
// //     std::vector<std::string> value_list;
// //     uint64_t value;
// //    // if (_table == "dbbody")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_body(value);
// //     // }
// //     // else if (_table == "dbhair")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_hair(value);
// //     // }
// //     // else if (_table == "dbhead")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_head(value);
// //     // }
// //     // else if (_table == "dbgender")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_gender(value);
// //     // }
// //     // if (_table == "dbservantjob")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_job(value);
// //     // }
// //     // else if (_table == "dbitemgrade")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_item_grade(value);
// //     // }
// //     //     else if (_table == "dbmonstergd")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_monster_grade(value);
// //     // }
// //     //     else if (_table == "dbgraderatio")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_grade_ratio(value);
// //     // }
// //     // if (_table == "dbmonsterup")
// //     // {
// //     //     value = atoi(_value.c_str());
// //     //     erase_upgrade_monster_ratio(value);
// //     // }
// //     // if (_table == "dblevel")
// //     // {
// //     //     value = atoi(_value.c_str());
// //     //     erase_level(value);
// //     // }
// //     // if (_table == "dbburn")
// //     // {
// //     //     value = atoi(_value.c_str());
// //     // }
// //     // if (_table == "dbservants")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_servant_id(value);
// //     // }
// //     // if (_table == "dbmonsters")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_monster_id(value);
// //     // }
// //     // if (_table == "dballitem")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_all_item_id(value);
// //     // }
// //     // if (_table == "dbequipment")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_equip_item_id(value);
// //     // }
// //     // else if (_table =="dbservnatlv")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_servant_lv(value);
// //     // }
// //     // if (_table =="dbmonsterlv")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_monster_lv(value);
// //     // }
// //     // if (_table =="dbstatusserv")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_servant_lv_status(value);
// //     // }
// //     // if (_table =="dbstatusmon")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_monster_lv_status(value);
// //     // }
// //     // if (_table == "dbstatusmon_change_status")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_monster_lv_status_list(value);
// //     // }
// //     // if (_table == "dbpassive")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_passive(value);
// //     // }
// //     // if (_table == "dbactive")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_active(value);
// //     // }
// //     // if (_table == "dbgachapool")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_gacha_pool(value);
// //     // }
// //     // if (_table == "dbprepool")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_pre_gacha_pool(value);
// //     // }
// //     // if (_table == "dbstatusequi")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_equipment_lv_status(value);
// //     // }
// //     // if( _table == "dbstatusequi_change_status")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_equipment_lv_status_list(value);
// //     // }
// //     // if (_table == "dbstage_monsterlist") 
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_stage_monster_list(value);
// //     // }
// //     // if (_table == "dbclassstat")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_class_state(value);
// //     // }
// //     // if (_table == "dbjobstat")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_job_state(value);
// //     // }
// //     // else
// //     // {
// //     //     eosio_assert(1 == 0, "Not Exist Table");
// //     // }
// //     // if (_table == "dbitemshop")
// //     // {
// //     //     value = atoll(_value.c_str());
// //     //     erase_itemshop(value);
// //     // }

// // }

// void battletest::erase_job(uint64_t _job)
// {
//     servant_job_db servant_job_db_table(_self, _self.value);
//     auto job_iter = servant_job_db_table.find(_job);
//     eosio_assert(job_iter != servant_job_db_table.end(), "Not Exist Job Data");
//     servant_job_db_table.erase(job_iter);
// }

// // void battletest::erase_body(uint64_t _appear)
// // {
// //     body_db body_db_table(_self, _self.value);
// //     auto body_iter = body_db_table.find(_appear);
// //     eosio_assert(body_iter != body_db_table.end(), "Not Find Body Data");
// //     body_db_table.erase(body_iter);
// // }

// // void battletest::erase_hair(uint64_t _appear)
// // {
// //     hair_db hair_db_table(_self, _self.value);
// //     auto hair_iter = hair_db_table.find(_appear);
// //     eosio_assert(hair_iter != hair_db_table.end(), "Not Find Hair Data");
// //     hair_db_table.erase(hair_iter);
// // }

// // void battletest::erase_head(uint64_t _appear)
// // {
// //     head_db head_db_table(_self, _self.value);
// //     auto head_iter = head_db_table.find(_appear);
// //     eosio_assert(head_iter != head_db_table.end(), "Not Find Head Data");
// //     head_db_table.erase(head_iter);
// // }

// // void battletest::erase_gender(uint64_t _appear)
// // {
// //     gender_db gender_db_table(_self, _self.value);
// //     auto gender_iter = gender_db_table.find(_appear);
// //     eosio_assert(gender_iter != gender_db_table.end(), "Not Find Gender Data");
// //     gender_db_table.erase(gender_iter);
// // }

// void battletest::erase_servant_id(uint64_t _id)
// {
//     servant_db servant_id_db_table(_self, _self.value);
//     auto servant_id_iter = servant_id_db_table.find(_id);
//     eosio_assert(servant_id_iter != servant_id_db_table.end(), "Not Find Servant ID Data");
//     servant_id_db_table.erase(servant_id_iter);
// }

// void battletest::erase_monster_id(uint64_t _id)
// {
//     monster_db monster_id_db_table(_self, _self.value);
//     auto monster_id_iter = monster_id_db_table.find(_id);
//     eosio_assert(monster_id_iter != monster_id_db_table.end(), "Not Find Monster ID Data");
//     monster_id_db_table.erase(monster_id_iter);
// }

// // void battletest::erase_monster_grade(uint64_t _grade)
// // {
// //     monster_grade_db monster_grade_db_table(_self, _self.value);
// //     auto monster_grade_iter = monster_grade_db_table.find(_grade);
// //     eosio_assert(monster_grade_iter != monster_grade_db_table.end(), "Not Exist Monster Grade");
// //     monster_grade_db_table.erase(monster_grade_iter);
// // }

 

// // void battletest::erase_equip_item_id(uint64_t _id)
// // {
// //     equipment_db item_id_db_table(_self, _self.value);
// //     auto item_id_iter = item_id_db_table.find(_id);
// //     eosio_assert(item_id_iter != item_id_db_table.end(), "Not Exist Item ID 2");
// //     item_id_db_table.erase(item_id_iter);
// // }

// // void battletest::erase_all_item_id(uint64_t _id)
// // {
// //     allitem_db item_id_db_table(_self, _self.value);
// //     auto item_id_iter = item_id_db_table.find(_id);
// //     eosio_assert(item_id_iter != item_id_db_table.end(), "Not Exist Item ID 2");
// //     item_id_db_table.erase(item_id_iter);
// // }

// // void battletest::erase_grade_ratio(uint64_t _grade)
// // {
// //     grade_ratio_db grade_ratio_db_table(_self, _self.value);
// //     auto grade_ratio_db_iter = grade_ratio_db_table.find(_grade);
// //     eosio_assert(grade_ratio_db_iter != grade_ratio_db_table.end(), "Not Exist Grade Ratio Data");
// //     grade_ratio_db_table.erase(grade_ratio_db_iter);
// // }

// // void battletest::erase_upgrade_monster_ratio(uint32_t _main)
// // {
// //     upgrade_monster_ratio_db upgrade_monster_ratio_db_table(_self, _self.value);
// //     auto upgrade_monster_ratio_db_iter = upgrade_monster_ratio_db_table.find(_main);
// //     eosio_assert(upgrade_monster_ratio_db_iter != upgrade_monster_ratio_db_table.end(), "Not Exist upgrade Main Monster Data");
// //     upgrade_monster_ratio_db_table.erase(upgrade_monster_ratio_db_iter);
// // }

// // void battletest::erase_level(uint32_t _id)
// // {
// //     lv_exp lv_exp_table(_self, _self.value);
// //     auto lv_exp_iter = lv_exp_table.find(_id);
// //     eosio_assert(lv_exp_iter != lv_exp_table.end(), "Not exist Level to Exp Data");
// //     lv_exp_table.erase(lv_exp_iter);
// // }

// // void battletest::erase_servant_lv_status(uint64_t _type)
// // {
// //     servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
// //     auto servant_lv_status_db_iter = servant_lv_status_db_table.find(_type);
// //     eosio_assert(servant_lv_status_db_iter != servant_lv_status_db_table.end(), "Not exist servant_lv_status Data");
// //     servant_lv_status_db_table.erase(servant_lv_status_db_iter);
// // }

// // void battletest::erase_servant_lv_status_list(uint64_t _type)
// // {
// //     servant_lv_status_db servant_lv_status_db_table(_self, _self.value);
// //     auto servant_lv_status_db_iter = servant_lv_status_db_table.find(_type);
// //     eosio_assert(servant_lv_status_db_iter != servant_lv_status_db_table.end(), "Not exist servant_lv_status Data");
// //     servant_lv_status_db_table.modify(servant_lv_status_db_iter, _self, [&](auto &new_data) {
// //         new_data.change_status.clear();
// //     });
// // }

// // void battletest::erase_monster_lv_status(uint64_t _type)
// // {
// //     monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
// //     auto monster_lv_status_db_iter = monster_lv_status_db_table.find(_type);
// //     eosio_assert(monster_lv_status_db_iter != monster_lv_status_db_table.end(), "Not exist monster_lv_status Data");
// //     monster_lv_status_db_table.erase(monster_lv_status_db_iter);
// // }

// // void battletest::erase_monster_lv_status_list(uint64_t _type)
// // {
// //     monster_lv_status_db monster_lv_status_db_table(_self, _self.value);
// //     auto monster_lv_status_db_iter = monster_lv_status_db_table.find(_type);
// //     eosio_assert(monster_lv_status_db_iter != monster_lv_status_db_table.end(), "Not exist monster_lv_status Data");
// //     monster_lv_status_db_table.modify(monster_lv_status_db_iter, _self, [&](auto &new_data) {
// //         new_data.change_status.clear();
// //     });
// // }

// // void battletest::erase_equipment_lv_status(uint64_t _type)
// // {
// //     equipment_lv_status_db equipment_lv_status_db_table(_self, _self.value);
// //     auto equipment_lv_status_db_iter = equipment_lv_status_db_table.find(_type);
// //     eosio_assert(equipment_lv_status_db_iter != equipment_lv_status_db_table.end(), "Not exist equipment_lv_status Data");
// //     equipment_lv_status_db_table.erase(equipment_lv_status_db_iter);
// // }

// // void battletest::erase_equipment_lv_status_list(uint64_t _type)
// // {
// //     equipment_lv_status_db equipment_lv_status_db_table(_self, _self.value);
// //     auto equipment_lv_status_db_iter = equipment_lv_status_db_table.find(_type);
// //     eosio_assert(equipment_lv_status_db_iter != equipment_lv_status_db_table.end(), "Not exist equipment_lv_status Data");
// //     equipment_lv_status_db_table.modify(equipment_lv_status_db_iter, _self, [&](auto &new_data)
// //     {
// //         new_data.change_status.clear();
// //     });
// // }

// // void battletest::erase_passive(uint64_t _id)
// // {
// //     passive_db passive_db_table(_self, _self.value);
// //     auto passive_db_iter = passive_db_table.find(_id);
// //     eosio_assert(passive_db_iter != passive_db_table.end(), "Not Exist Passive 1");
// //     passive_db_table.erase(passive_db_iter);
// // }

// // void battletest::erase_active(uint64_t _id)
// // {
// //     active_db active_db_table(_self, _self.value);
// //     auto active_db_iter = active_db_table.find(_id);
// //     eosio_assert(active_db_iter != active_db_table.end(), "Not Exist Active 1");
// //     active_db_table.erase(active_db_iter);
// // }

// void battletest::erase_gacha_pool(uint64_t _id)
// {
//     main_gacha_db main_gacha_db_table(_self, _self.value);
//     auto main_gacha_db_iter = main_gacha_db_table.find(_id);
//     eosio_assert(main_gacha_db_iter != main_gacha_db_table.end(), "Not Exist Gacha 1");
//     main_gacha_db_table.erase(main_gacha_db_iter);
// }

// // void battletest::erase_pre_gacha_pool(uint64_t _id)
// // {
// //     pre_gacha_db pre_gacha_db_table(_self, _self.value);
// //     auto pre_gacha_db_iter = pre_gacha_db_table.find(_id);
// //     eosio_assert(pre_gacha_db_iter != pre_gacha_db_table.end(), "Not Exist Pre Gacha 1");
// //     pre_gacha_db_table.erase(pre_gacha_db_iter);
// // }


// void battletest::erase_stage_reward_list(uint64_t _id)
// {
//     new_reward_db reward_db_table(_self, _self.value);
//     auto reward_iter = reward_db_table.find(_id);
//     eosio_assert(reward_iter != reward_db_table.end(), "Not Exist Stage ID 5");
//     reward_db_table.modify(reward_iter, _self, [&](auto &new_data) {
//         new_data.reward_list.clear();
//     });
// }

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
//     shop_list item_shop_table(_self, _self.value);
//     auto item_shop_iter = item_shop_table.find(_id);
//     eosio_assert(item_shop_iter != item_shop_table.end(), "Not Exist itemshop 1");
//     item_shop_table.erase(item_shop_iter);
// }
// void battletest::erase_itemshop2(uint64_t _id)
// {
//     item_shop item_shop_table(_self, _self.value);
//     auto item_shop_iter = item_shop_table.find(_id);
//     eosio_assert(item_shop_iter != item_shop_table.end(), "Not Exist itemshop 1");
//     item_shop_table.erase(item_shop_iter);
// }




// ACTION battletest::dbinit(std::string _table)
// {
//     system_master system_master_table(_self, _self.value);
//     auto system_master_iter = system_master_table.begin();

//     permission_level master_auth;
//     master_auth.actor = system_master_iter->master;
//     master_auth.permission = "active"_n;
//     require_auth(master_auth);

//     //eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 4");
//     if(_table == "dbenemyinfo")
//     {
//         for(uint32_t  i = 1; i <= 6; ++i)
//         {
//             enemyinfo_db my_table(_self, i);
//             for(auto iter = my_table.begin(); iter != my_table.end();)
//             {
//                 auto e_iter = my_table.find(iter->primary_key());
//                 iter++;
//                 my_table.erase(e_iter);
//             }
//         }
//     }

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
//     // if (_table == "dbmonsterup")
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
//     // if(_table == "dbnewreward")
//     // {
//     //     new_reward_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // if(_table == "dbstageinfo")
//     // {
//     //     stageinfo_db my_table(_self, _self.value);
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
//     // if (_table == "dballitem")
//     // {
//     //     allitem_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }

//     // // if (_table == "dbgachapool")
//     // // {
//     // //     main_gacha_db my_table(_self, _self.value);
//     // //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     // //     {
//     // //         auto erase_iter = my_table.find(iter->primary_key());
//     // //         iter++;
//     // //         my_table.erase(erase_iter);
//     // //     }
//     // // }

//     // if (_table == "dbgoldpool")
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
//     // if (_table == "dbreward")
//     // {
//     //     reward_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     // if (_table == "dbstage")
//     // {
//     //     stage_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     //     if (_table == "dbmoupstat")
//     // {
//     //     upgrade_monster_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }
//     //     if (_table == "dbequpstat")
//     // {
//     //     upgrade_equipment_db my_table(_self, _self.value);
//     //     for (auto iter = my_table.begin(); iter != my_table.end();)
//     //     {
//     //         auto erase_iter = my_table.find(iter->primary_key());
//     //         iter++;
//     //         my_table.erase(erase_iter);
//     //     }
//     // }

//     //     // else
//     //     // {
//     //     //     eosio_assert(1 == 0, "Not Exist Table");
//     //     // }
// }

// //  ACTION battletest::setdata(eosio::name _contract, eosio::name _user, std::string _table)
// //  {
// //     require_auth(_self);

// //     if (_table == "preauth")
// //     {
// //         pre_users pre_users_table(_contract, _contract.value);
// //         pre_users my_table(_self, _self.value);
// //         for (auto iter = pre_users_table.begin(); iter != pre_users_table.end();)
// //         {
// //             const auto &get_iter = pre_users_table.get(iter->primary_key(), "not exist data ");

// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.user = get_iter.user;
// //                 new_data.state = get_iter.state;
// //             });
// //             iter++;
// //         }
// //     }
// //     else if (_table == "prelog")
// //     {
// //         pre_logs pre_logs_table(_contract, _contract.value);
// //         pre_logs my_table(_self, _self.value);
// //         for (auto iter = pre_logs_table.begin(); iter != pre_logs_table.end();)
// //         {
// //             const auto &get_iter = pre_logs_table.get(iter->primary_key(), "not exist data ");

// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.user = get_iter.user;
// //                 new_data.servant_num = get_iter.servant_num;
// //                 new_data.monster_num = get_iter.monster_num;
// //                 new_data.item_num = get_iter.item_num;
// //                 new_data.gacha_num = get_iter.gacha_num;
// //                 new_data.use_eos = get_iter.use_eos;
// //             });
// //             iter++;
// //         }
// //     }
// //     if (_table == "preservant")
// //     {
// //         user_preregist_servants user_preregist_servants_table(_contract, _user.value);
// //         user_preregist_servants my_table(_self, _user.value);
// //         for (auto iter = user_preregist_servants_table.begin(); iter != user_preregist_servants_table.end();)
// //         {
// //             const auto &get_iter = user_preregist_servants_table.get(iter->primary_key(), "not exist data ");

// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.index = get_iter.index;
// //                 new_data.id = get_iter.id;
// //                 new_data.status.basic_str = get_iter.status.basic_str;
// //                 new_data.status.basic_dex = get_iter.status.basic_dex;
// //                 new_data.status.basic_int = get_iter.status.basic_int;

// //             });
// //             iter++;
// //         }
// //     }

// //     if (_table == "premonster")
// //     {
// //         user_preregist_monsters user_preregist_monsters_table(_contract, _user.value);
// //         user_preregist_monsters my_table(_self, _user.value);
// //         for (auto iter = user_preregist_monsters_table.begin(); iter != user_preregist_monsters_table.end();)
// //         {
// //             const auto &get_iter = user_preregist_monsters_table.get(iter->primary_key(), "not exist data ");

// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.index = get_iter.index;
// //                 new_data.id = get_iter.id;
// //                 new_data.grade = get_iter.grade;
// //                 new_data.status.basic_str = get_iter.status.basic_str;
// //                 new_data.status.basic_dex = get_iter.status.basic_dex;
// //                 new_data.status.basic_int = get_iter.status.basic_int;

// //             });
// //             iter++;
// //         }
// //     }

// //     else if (_table == "preitem")
// //     {
// //         user_preregist_items user_preregist_items_table(_contract, _user.value);
// //         user_preregist_items my_table(_self, _user.value);
// //         for (auto iter = user_preregist_items_table.begin(); iter != user_preregist_items_table.end();)
// //         {
// //             const auto &get_iter = user_preregist_items_table.get(iter->primary_key(), "not exist data ");

// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.index = get_iter.index;
// //                 new_data.id = get_iter.id;
// //                 new_data.type = get_iter.type;
// //                 new_data.tier = get_iter.tier;
// //                 new_data.job = get_iter.job;
// //                 new_data.grade = get_iter.grade;
// //                 new_data.main_status = get_iter.main_status;
// //             });
// //             iter++;
// //         }
// //     }

// //     if(_table == "dbstatusequi")
// //     {
// //         equipment_lv_status_db upgrade_item_ratio_db_table(_contract, _contract.value);
// //         equipment_lv_status_db my_table(_self, _self.value);
// //         for (auto iter = upgrade_item_ratio_db_table.begin(); iter != upgrade_item_ratio_db_table.end();)
// //         {
// //             const auto &data = upgrade_item_ratio_db_table.get(iter->primary_key(), "nost exist data");
// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.type_grade = data.type_grade;
// //                 new_data.change_status = data.change_status;
// //             });
// //             iter++;
// //         }
// //     }
// //     if (_table == "dbbody")
// //     {
// //         body_db body_table(_contract, _contract.value);
// //         for (auto iter = body_table.begin(); iter != body_table.end();)
// //         {
// //             const auto &data_iter = body_table.get(iter->primary_key(), "Not Exist Data");
// //             insert_body(data_iter.body);
// //             iter++;
// //         }
// //     }
// //     if (_table == "dbhair")
// //     {
// //         hair_db hair_table(_contract, _contract.value);
// //         for (auto iter = hair_table.begin(); iter != hair_table.end();)
// //         {
// //             const auto &data_iter = hair_table.get(iter->primary_key(), "Not Exist Data");
// //             insert_hair(data_iter.hair);
// //             iter++;
// //         }
// //     }
// //     if (_table == "dbhead")
// //     {
// //         head_db head_table(_contract, _contract.value);
// //         for (auto iter = head_table.begin(); iter != head_table.end();)
// //         {
// //             const auto &data_iter = head_table.get(iter->primary_key(), "Not Exist Data");
// //             insert_head(data_iter.head);
// //             iter++;
// //         }
// //     }
// //     if (_table == "dbgender")
// //     {
// //         gender_db gender_table(_contract, _contract.value);
// //         for (auto iter = gender_table.begin(); iter != gender_table.end();)
// //         {
// //             const auto &data_iter = gender_table.get(iter->primary_key(), "Not Exist Data");
// //             insert_gender(data_iter.gender);
// //             iter++;
// //         }
// //     }

// //     if (_table == "dbservantjob")
// //     {
// //         servant_job_db servant_job_table(_contract, _contract.value);
// //         for (auto iter = servant_job_table.begin(); iter != servant_job_table.end();)
// //         {
// //             const auto &data_iter = servant_job_table.get(iter->primary_key(), "Not Exist Data");
// //             insert_job(std::string("all"), data_iter.job, 0, 10);
// //             iter++;
// //         }
// //     }
// //     else if (_table == "dbgraderatio")
// //     {
// //         grade_ratio_db other_table(_contract, _contract.value);
// //         grade_ratio_db my_table(_self, _self.value);
// //         for (auto iter = other_table.begin(); iter != other_table.end();)
// //         {
// //             const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");

// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.grade = get_iter.grade;
// //                 new_data.ratio = get_iter.ratio;
// //             });
// //             iter++;
// //         }
// //     }
// //     if (_table == "dbburn")
// //     {
// //         burnitem_db other_table(_contract, _contract.value);
// //          burnitem_db my_table(_self, _self.value);
// //                  for (auto iter2 = other_table.begin(); iter2 != other_table.end();)
// //         {
// //             const auto &upgrade_monster_iter = other_table.get(iter2->primary_key(), "nost exist data");
// //             my_table.emplace(_self, [&](auto &new_data)
// //             {
// //                 new_data.servant_job = upgrade_monster_iter.servant_job;
// //                 new_data.result_item_id = upgrade_monster_iter.result_item_id;
// //             });
// //             iter2++;
// //         }
// //     }
// //     if (_table == "dbmonsterup")
// //     {
// //         upgrade_monster_ratio_db upgrade_monster_ratio_db_table(_contract, _contract.value);
// //         upgrade_monster_ratio_db my_table(_self, _self.value);
// //         for (auto iter2 = upgrade_monster_ratio_db_table.begin(); iter2 != upgrade_monster_ratio_db_table.end();)
// //         {
// //             const auto &upgrade_monster_iter = upgrade_monster_ratio_db_table.get(iter2->primary_key(), "nost exist data");
// //             my_table.emplace(_self, [&](auto &new_data)
// //             {
// //                 new_data.main_monster_grade_upgrade = upgrade_monster_iter.main_monster_grade_upgrade;
// //                 new_data.sub = upgrade_monster_iter.sub;
// //                 new_data.use_UTG = upgrade_monster_iter.use_UTG;
// //             });
// //             iter2++;
// //         }
// //     }
// //     if (_table == "dbequipup")
// //     {
// //         upgrade_equipment_ratio_dbs upgrade_item_ratio_db_table(_contract, _contract.value);
// //         upgrade_equipment_ratio_dbs my_table(_self, _self.value);
// //         for (auto iter = upgrade_item_ratio_db_table.begin(); iter != upgrade_item_ratio_db_table.end();)
// //         {
// //             const auto &data = upgrade_item_ratio_db_table.get(iter->primary_key(), "nost exist data");
// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.equipment_type_grade = data.equipment_type_grade;
// //                 new_data.material_id = data.material_id;
// //                 new_data.upgrade_ratio = data.upgrade_ratio;
// //                 new_data.material_count= data.material_count;
// //                 new_data.use_UTG = data.use_UTG;
// //             });
// //             iter++;
// //         }
// //     }
// //     if (_table == "dblevel")
// //     {
// //         lv_exp other_lv_exp_table(_contract, _contract.value);
// //         lv_exp my_table(_self, _self.value);
// //         for (auto iter15 = other_lv_exp_table.begin(); iter15 != other_lv_exp_table.end();)
// //         {
// //             const auto &lv_exp_iter = other_lv_exp_table.get(iter15->primary_key(), "nost exist data");
// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.lv = lv_exp_iter.lv;
// //                 new_data.rank_exp = lv_exp_iter.rank_exp;
// //                 new_data.char_exp = lv_exp_iter.char_exp;
// //             });
// //             iter15++;
// //         }
// //     }
// //     else if (_table == "dbstatusserv")
// //     {
// //         servant_lv_status_db other_table(_contract, _contract.value);
// //         servant_lv_status_db my_table(_self, _self.value);
// //         for (auto iter = other_table.begin(); iter != other_table.end();)
// //         {
// //             const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");

// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.grade = get_iter.grade;
// //                 new_data.change_status.insert(new_data.change_status.end(), get_iter.change_status.begin(), get_iter.change_status.end());
// //             });
// //             iter++;
// //         }
// //     }
// //     else if (_table == "dbstatusmon")
// //     {
// //         monster_lv_status_db other_table(_contract, _contract.value);
// //         monster_lv_status_db my_table(_self, _self.value);
// //         for (auto iter = other_table.begin(); iter != other_table.end();)
// //         {
// //             const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");

// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.grade = get_iter.grade;
// //                 new_data.change_status.insert(new_data.change_status.end(), get_iter.change_status.begin(), get_iter.change_status.end());
// //             });
// //             iter++;
// //         }
// //     }
// //         else if (_table == "dbpassive")
// //         {
// //             passive_db other_table(_contract, _contract.value);
// //             passive_db my_table(_self, _self.value);
// //             for (auto iter = other_table.begin(); iter != other_table.end();)
// //             {
// //                 const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");

// //                 my_table.emplace(_self, [&](auto &new_data) {
// //                     new_data.passive_id = get_iter.passive_id;
// //                     new_data.enable_stack = get_iter.enable_stack;
// //                     new_data.max_stack = get_iter.max_stack;
// //                     new_data.effect_type = get_iter.effect_type;
// //                     new_data.effect_value = get_iter.effect_value;
// //                     new_data.effect_value_add = get_iter.effect_value_add;
// //                     new_data.target = get_iter.target;
// //                     new_data.role_target = get_iter.role_target;
// //                 });
// //                 iter++;
// //             }
// //         }
// //         else if (_table == "dbactive")
// //         {
// //             active_db other_table(_contract, _contract.value);
// //             active_db my_table(_self, _self.value);
// //             for (auto iter = other_table.begin(); iter != other_table.end();)
// //             {
// //                 const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");

// //                 my_table.emplace(_self, [&](auto &new_data) {
// //                     new_data.active_id = get_iter.active_id;
// //                     new_data.job = get_iter.job;
// //                     new_data.active_per = get_iter.active_per;
// //                     new_data.skill_type = get_iter.skill_type;
// //                     new_data.attack_type = get_iter.attack_type;
// //                     new_data.dmg_type = get_iter.dmg_type;
// //                     new_data.target = get_iter.target;
// //                     new_data.target_count = get_iter.target_count;
// //                     new_data.active_turn = get_iter.target_active_turnrange;
// //                     new_data.hit_count = get_iter.hit_count;
// //                     new_data.atk_per_1 = get_iter.atk_per_1;
// //                     new_data.atk_per_add_1 = get_iter.atk_per_add_1;
// //                     new_data.heal_per = get_iter.heal_per;
// //                     new_data.heal_per_add = get_iter.heal_per_add;
// //                 });
// //                 iter++;
// //             }
// //         }
// //     if (_table == "dbmonsters")
// //     {
// //         monster_db monster_table(_contract, _contract.value);
// //         monster_db my_table(_self, _self.value);
// //         for (auto iter = monster_table.begin(); iter != monster_table.end();)
// //         {
// //             const auto &data_iter = monster_table.get(iter->primary_key(), "Not Exist Data");
// //             my_table.emplace(_self, [&](auto &new_monster)
// //             {
// //                 new_monster.id = data_iter.id;
// //                 new_monster.tribe = data_iter.tribe;
// //                 new_monster.type = data_iter.type;
// //             });
// //             iter++;
// //         }
// //     }
// //     if (_table == "dbitemshop")
// //    {
// //        item_shop other_table(_contract, _contract.value);
// //        item_shop my_table(_self, _self.value);
// //        for (auto iter = other_table.begin(); iter != other_table.end();)
// //        {
// //            const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
// //            my_table.emplace(_self, [&](auto &new_data) {
// //                new_data.id = get_iter.id;
// //                new_data.shop_type = get_iter.shop_type;
// //                new_data.product_id = get_iter.product_id;
// //                new_data.product_count = get_iter.product_count;
// //                new_data.product_limit_max = get_iter.product_limit_max;
// //                new_data.price_id = get_iter.price_id;
// //                new_data.price_count = get_iter.price_count;
// //            });
// //            iter++;
// //        }
// //    }

// //    if (_table == "tshoplist")
// //    {
// //        shop_list other_table(_contract, _contract.value);
// //        shop_list my_table(_self, _self.value);
// //        for (auto iter = other_table.begin(); iter != other_table.end();)
// //        {
// //            const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
// //            my_table.emplace(_self, [&](auto &new_data) {
// //                new_data.id = get_iter.id;
// //                new_data.shop_type = get_iter.shop_type;
// //                new_data.shop_item_id = get_iter.shop_item_id;
// //                new_data.limit_count = get_iter.limit_count;
// //            });
// //            iter++;
// //        }
// //    }
// //     if (_table == "dbservants")
// //     {
// //         servant_db servant_table(_contract, _contract.value);
// //         servant_db my_table(_self, _self.value);
// //         for (auto iter = servant_table.begin(); iter != servant_table.end();)
// //         {
// //             const auto &data_iter = servant_table.get(iter->primary_key(), "Not Exist Data");
// //             my_table.emplace(_self, [&](auto &new_ser) {
// //                 new_ser.id = data_iter.id;
// //                 new_ser.body = data_iter.body;
// //                 new_ser.head = data_iter.head;
// //                 new_ser.gender = data_iter.gender;
// //                 new_ser.head = data_iter.head;
// //                 new_ser.hair = data_iter.hair;
// //                 new_ser.job = data_iter.job;
// //                 new_ser.grade = data_iter.grade;
// //             });
// //             iter++;
// //         }
// //     }
// //     if (_table == "dbitemshop")
// //     {
// //         item_shop other_table(_contract, _contract.value);
// //         item_shop my_table(_self, _self.value);
// //         for (auto iter = other_table.begin(); iter != other_table.end();)
// //         {
// //             const auto &data_iter = other_table.get(iter->primary_key(), "Not Exist Data");
// //             my_table.emplace(_self, [&](auto &new_ser) {
// //                 new_ser.id = data_iter.id;
// //                 new_ser.goods_type = data_iter.goods_type;
// //                 new_ser.goods_limited = data_iter.goods_limited;
// //                 new_ser.goods_count = data_iter.goods_count;
// //                 new_ser.price_type = data_iter.price_type;
// //                 new_ser.price_count = data_iter.price_count;
// //             });
// //             iter++;
// //         }
// //     }
// //     if (_table == "dbequipment")
// //     {
// //         equipment_db equip_item_table(_contract, _contract.value);
// //         equipment_db my_table(_self, _self.value);
// //         for (auto iter = equip_item_table.begin(); iter != equip_item_table.end();)
// //         {
// //             const auto &data_iter = equip_item_table.get(iter->primary_key(), "Not Exist Data");
// //             my_table.emplace(_self, [&](auto &new_a) {
// //                 new_a.item_id = data_iter.item_id;
// //                 new_a.set_id = data_iter.set_id;
// //                 new_a.type = data_iter.type;
// //                 new_a.tier = data_iter.tier;
// //                 new_a.job = data_iter.job;

// //                 new_a.option_list=data_iter.option_list;
// //                 new_a.option_value_min_list=data_iter.option_value_min_list;
// //                 new_a.option_value_max_list= data_iter.option_value_max_list;
// //                 new_a.upgrade_option_value_list=data_iter.upgrade_option_value_list;
// //                 new_a.random_option_id_list=data_iter.random_option_id_list;
// //                 new_a.grade_multi_list = data_iter.grade_multi_list;

// //             });
// //             iter++;
// //         }
// //     }
// //     if (_table == "dballitem")
// //     {
// //         allitem_db common_item_table(_contract, _contract.value);
// //         allitem_db my_table(_self, _self.value);
// //         for (auto iter = common_item_table.begin(); iter != common_item_table.end();)
// //         {
// //             const auto &data_iter = common_item_table.get(iter->primary_key(), "Not Exist Data");
// //             my_table.emplace(_self, [&](auto &new_a) {
// //                 new_a.id = data_iter.id;
// //                 new_a.type = data_iter.type;
// //                 new_a.item_param_list.insert(new_a.item_param_list.begin(), data_iter.item_param_list.begin(), data_iter.item_param_list.end());
// //                 new_a.sell_id = data_iter.sell_id;
// //                 new_a.sell_cost = data_iter.sell_cost;
// //             });
// //             iter++;
// //         }
// //     }
// //     if (_table == "dbgachapool")
// //     {
// //         main_gacha_db other_table(_contract, _contract.value);
// //         main_gacha_db my_table(_self, _self.value);
// //         for (auto iter = other_table.begin(); iter != other_table.end();)
// //         {
// //             const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.gacha_id = get_iter.gacha_id;
// //                 new_data.db_index = get_iter.db_index;
// //             });
// //             iter++;
// //         }
// //     }

// //     else if (_table == "dbprepool")
// //     {
// //         pre_gacha_db other_table(_contract, _contract.value);
// //         pre_gacha_db my_table(_self, _self.value);
// //         for (auto iter = other_table.begin(); iter != other_table.end();)
// //         {
// //             const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
// //             my_table.emplace(_self, [&](auto &new_data) {
// //                 new_data.gacha_id = get_iter.gacha_id;
// //                 new_data.db_index = get_iter.db_index;
// //             });
// //             iter++;
// //         }
// //     }

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
//             new_data.enemy_tribe = get_iter.enemy_tribe;
//             new_data.type = get_iter.type;
//             new_data.enemy_str = get_iter.enemy_str;
//             new_data.enemy_dex = get_iter.enemy_dex;
//             new_data.enemy_int = get_iter.enemy_int;
//             new_data.cri_per = get_iter.cri_per;
//             new_data.cri_dmg = get_iter.cri_dmg;
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
//             new_data.reward_list = get_iter.reward_list;
//         });
//         iter++;
//     }
// }

// else if (_table == "dbstage")
// {
//     stage_db other_table(_contract, _contract.value);
//     stage_db my_table(_self, _self.value);
//     for (auto iter = other_table.begin(); iter != other_table.end();)
//     {
//         const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
//         my_table.emplace(_self, [&](auto &new_data) {
//             new_data.stage_id = get_iter.stage_id;
//             new_data.stage_type = get_iter.stage_type;
//             new_data.stage_floor = get_iter.stage_floor;
//             new_data.need_stage_id = get_iter.need_stage_id;
//             new_data.stage_group_index = get_iter.stage_group_index;
//             new_data.need_entrance_item_id = get_iter.need_entrance_item_id;
//             new_data.need_entrance_item_count = get_iter.need_entrance_item_count;
//             new_data.enemy_level_min = get_iter.enemy_level_min;
//             new_data.enemy_level_max = get_iter.enemy_level_max;
//             new_data.monster_list = get_iter.monster_list;
//             new_data.boss_level = get_iter.boss_level;
//             new_data.boss_passive_list = get_iter.boss_passive_list;
//             new_data.boss_skill_list = get_iter.boss_skill_list;
//         });
//         iter++;
//     }
// }

// else if (_table == "dbserstat")
// {
//     serstat_db other_table(_contract, _contract.value);
//     serstat_db my_table(_self, _self.value);
//     for (auto iter = other_table.begin(); iter != other_table.end();)
//     {
//         const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
//         my_table.emplace(_self, [&](auto &new_data) {
//             new_data.id = get_iter.id;
//             new_data.job = get_iter.job;
//             new_data.grade = get_iter.grade;
//             new_data.base_str = get_iter.base_str;
//             new_data.base_dex = get_iter.base_dex;
//             new_data.base_int = get_iter.base_int;
//             new_data.speed = get_iter.speed;
//             new_data.avoid = get_iter.avoid;
//             new_data.cri_per = get_iter.cri_per;
//             new_data.cri_dmg = get_iter.cri_dmg;
//         });
//         iter++;
//     }
// }

// else if (_table == "dbtribe")
// {
//     tribe_db other_table(_contract, _contract.value);
//     tribe_db my_table(_self, _self.value);
//     for (auto iter = other_table.begin(); iter != other_table.end();)
//     {
//         const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
//         my_table.emplace(_self, [&](auto &new_data) {
//             new_data.id = get_iter.id;
//             new_data.base_str = get_iter.base_str;
//             new_data.base_dex = get_iter.base_dex;
//             new_data.base_int = get_iter.base_int;
//             new_data.speed = get_iter.speed;
//             new_data.avoid = get_iter.avoid;
//             new_data.cri_per = get_iter.cri_per;
//             new_data.cri_dmg = get_iter.cri_dmg;
//             new_data.pair = get_iter.pair;
//             new_data.triple = get_iter.triple;
//             new_data.penta = get_iter.penta;
//         });
//         iter++;
//     }
// }

// else if (_table == "dbmoupstat")
// {
//     upgrade_monster_db other_table(_contract, _contract.value);
//     upgrade_monster_db my_table(_self, _self.value);
//     for (auto iter = other_table.begin(); iter != other_table.end();)
//     {
//         const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
//         my_table.emplace(_self, [&](auto &new_data) {
//             new_data.upgrade = get_iter.upgrade;
//             new_data.grade_list = get_iter.grade_list;
//         });
//         iter++;
//     }
// }

// else if (_table == "dbequpstat")
// {
//     upgrade_equipment_db other_table(_contract, _contract.value);
//     upgrade_equipment_db my_table(_self, _self.value);
//     for (auto iter = other_table.begin(); iter != other_table.end();)
//     {
//         const auto &get_iter = other_table.get(iter->primary_key(), "not exist data ");
//         my_table.emplace(_self, [&](auto &new_data) {
//             new_data.upgrade = get_iter.upgrade;
//             new_data.grade_list = get_iter.grade_list;
//         });
//         iter++;
//     }
// }

// else
// {
//     eosio_assert(1 == 0, "Not Exist Table");
// }
//}


void battletest::insert_stage_info(std::vector<uint32_t> _stage_info)
{
    stageinfo_db my_table(_self, _self.value);
    auto iter = my_table.find(_stage_info[0]);
    if(iter == my_table.end())
    {
        my_table.emplace(_self, [&](auto &new_data)
        {
            new_data.id = _stage_info[0];
            new_data.type = _stage_info[1];
            new_data.floor = _stage_info[2];
            new_data.difficult = _stage_info[3];
            new_data.need_entrance_item_id = _stage_info[4];
            new_data.need_entrance_item_count = _stage_info[5];
            new_data.enemy_level_min = _stage_info[6];
            new_data.enemy_level_max = _stage_info[7];
            new_data.enemy_count = _stage_info[8];
        }); 
    }
    else
    {
        my_table.modify(iter, _self, [&](auto &new_data) {
            new_data.type = _stage_info[1];
            new_data.floor = _stage_info[2];
            new_data.difficult = _stage_info[3];
            new_data.need_entrance_item_id = _stage_info[4];
            new_data.need_entrance_item_count = _stage_info[5];
            new_data.enemy_level_min = _stage_info[6];
            new_data.enemy_level_max = _stage_info[7];
            new_data.enemy_count = _stage_info[8];
        });
    }
}

void battletest::insert_enemy_info(std::vector<uint32_t> _enemy_info)
{
    enemyinfo_db my_table(_self, _enemy_info[0]);
    auto iter = my_table.find(_enemy_info[1]);
    if(iter == my_table.end())
    {
        my_table.emplace(_self, [&](auto &new_data)
        {
            new_data.index = _enemy_info[1];
            new_data.id = _enemy_info[2];
            new_data.type = _enemy_info[3];
            new_data.active_list.push_back(_enemy_info[4]);
            new_data.passive_list.push_back(_enemy_info[5]);
        }); 
    }
    else
    {
        my_table.modify(iter, _self, [&](auto &new_data) {
            new_data.id = _enemy_info[2];
            new_data.type = _enemy_info[3];
            new_data.active_list[0] = (_enemy_info[4]);
            new_data.passive_list[0] = (_enemy_info[5]);
        });
    }
}

void battletest::insert_enemy_stat(std::vector<uint32_t> _enemy_stat)
{
    enemystat_db my_table(_self, _self.value);
    auto iter = my_table.find(_enemy_stat[0]);
    if(iter == my_table.end())
    {
        my_table.emplace(_self, [&](auto &new_data)
        {
            new_data.id = _enemy_stat[0];
            new_data.base_str = _enemy_stat[1];
            new_data.base_dex = _enemy_stat[2];
            new_data.base_int = _enemy_stat[3];
        }); 
    }
    else
    {
        my_table.modify(iter, _self, [&](auto &new_data) {
            new_data.base_str = _enemy_stat[1];
            new_data.base_dex = _enemy_stat[2];
            new_data.base_int = _enemy_stat[3];
        });
    }
}

void battletest::insert_new_reward(std::vector<uint32_t> _reward)
{
    new_reward_db my_table(_self, _self.value);
    auto iter = my_table.find(_reward[0]);
    if(iter == my_table.end())
    {
        my_table.emplace(_self, [&](auto &new_data)
        {
            new_data.id = _reward[0];
            new_data.reward_utg = _reward[1];
            new_data.rank_exp = _reward[2];
            new_data.char_exp = _reward[3];
            new_data.reward_count = _reward[4];
            new_data.per_monster = _reward[5];
            new_data.per_equipment = _reward[6];
        }); 
    }
    else
    {
        my_table.modify(iter, _self, [&](auto &new_data) {
            new_data.reward_utg = _reward[1];
            new_data.rank_exp = _reward[2];
            new_data.char_exp = _reward[3];
            new_data.reward_count = _reward[4];
            new_data.per_monster = _reward[5];
            new_data.per_equipment = _reward[6];
        });
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

    //eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 1");

    std::vector<size_t> size_list;
    std::vector<std::string> value_list;
    uint32_t value;
    if(_table == "dblevel")
    {
        substr_value(_value, value_list, size_list, 10);
        lv_exp my_table(_self, _self.value);
        auto iter = my_table.find(atoll(value_list[0].c_str()));
        if (iter == my_table.end())
        {
            my_table.emplace(_self, [&](auto &new_data) {
                new_data.lv = atoll(value_list[0].c_str());
                new_data.rank_exp = atoi(value_list[1].c_str());
                new_data.char_exp = atoi(value_list[2].c_str());
            });
        }
        else
        {
            my_table.modify(iter, _self, [&](auto &new_data) {
                new_data.rank_exp = atoi(value_list[1].c_str());
                new_data.char_exp = atoi(value_list[2].c_str());
            });
        }
    }
    if (_table == "dbstageinfo")
    {
        substr_value(_value, value_list, size_list, 10);
        stageinfo_db my_table(_self, _self.value);
        auto iter = my_table.find(atoll(value_list[0].c_str()));
        if (iter == my_table.end())
        {
            my_table.emplace(_self, [&](auto &new_data) {
                new_data.id = atoll(value_list[0].c_str());
                new_data.elemental_type = atoi(value_list[1].c_str());
                new_data.floor = atoi(value_list[2].c_str());
                new_data.difficult = atoi(value_list[3].c_str());
                new_data.need_entrance_item_id = atoi(value_list[4].c_str());
                new_data.need_entrance_item_count = atoi(value_list[5].c_str());
                new_data.enemy_level_min = atoi(value_list[6].c_str());
                new_data.enemy_level_max = atoi(value_list[7].c_str());
                new_data.enemy_count = atoi(value_list[8].c_str());
            });
        }
        else
        {
            my_table.modify(iter, _self, [&](auto &new_data) {
                new_data.elemental_type = atoi(value_list[1].c_str());
                new_data.floor = atoi(value_list[2].c_str());
                new_data.difficult = atoi(value_list[3].c_str());
                new_data.need_entrance_item_id = atoi(value_list[4].c_str());
                new_data.need_entrance_item_count = atoi(value_list[5].c_str());
                new_data.enemy_level_min = atoi(value_list[6].c_str());
                new_data.enemy_level_max = atoi(value_list[7].c_str());
                new_data.enemy_count = atoi(value_list[8].c_str());
            });
        }
    }
    if (_table == "dbdailystage")
    {
        substr_value(_value, value_list, size_list, 9);
        daily_stage_db my_table(_self, _self.value);
        auto iter = my_table.find(atoll(value_list[0].c_str()));
        if (iter == my_table.end())
        {
            my_table.emplace(_self, [&](auto &new_data) {
                new_data.id = atoll(value_list[0].c_str());
                new_data.stage_type = atoi(value_list[1].c_str());
                new_data.elemental_type = atoi(value_list[2].c_str());
                new_data.difficult = atoi(value_list[3].c_str());
                new_data.max_entrance_count = atoi(value_list[4].c_str());
                new_data.real_max_entrance_count = atoi(value_list[5].c_str());
                new_data.enemy_level_min = atoi(value_list[6].c_str());
                new_data.enemy_level_max = atoi(value_list[7].c_str());
                new_data.enemy_count = atoi(value_list[8].c_str());
            });
        }
        else
        {
            my_table.modify(iter, _self, [&](auto &new_data) {
                new_data.stage_type = atoi(value_list[1].c_str());
                new_data.elemental_type = atoi(value_list[2].c_str());
                new_data.difficult = atoi(value_list[3].c_str());
                new_data.max_entrance_count = atoi(value_list[4].c_str());
                new_data.real_max_entrance_count = atoi(value_list[5].c_str());
                new_data.enemy_level_min = atoi(value_list[6].c_str());
                new_data.enemy_level_max = atoi(value_list[7].c_str());
                new_data.enemy_count = atoi(value_list[8].c_str());
            });
        }
    }
    // if(_table == "dblimitbreak")
    // {
    //     substr_value(_value, value_list, size_list, 7);
    //     limit_break_db my_table(_self, _self.value);
    //     auto iter = my_table.find(atoll(value_list[0].c_str()));
    //     if (iter == my_table.end())
    //     {
    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.id = atoll(value_list[0].c_str());
    //             new_data.type = atoi(value_list[1].c_str());
    //             new_data.available_level = atoi(value_list[2].c_str());
    //             new_data.need_item_id = atoi(value_list[3].c_str());
    //             new_data.need_item_count = atoi(value_list[4].c_str());
    //             new_data.use_utg = atoi(value_list[5].c_str());
    //             new_data.up_level = atoi(value_list[6].c_str());
    //         });
    //     }
    //     else
    //     {
    //         my_table.modify(iter, _self, [&](auto &new_data) {
    //             new_data.type = atoi(value_list[1].c_str());
    //             new_data.available_level = atoi(value_list[2].c_str());
    //             new_data.need_item_id = atoi(value_list[3].c_str());
    //             new_data.need_item_count = atoi(value_list[4].c_str());
    //             new_data.use_utg = atoi(value_list[5].c_str());
    //             new_data.up_level = atoi(value_list[6].c_str());
    //         });
    //     }
    // }
    if(_table == "dbnewreward")
    {
        substr_value(_value, value_list, size_list, 7);
        new_reward_db my_table(_self, _self.value);
        auto iter = my_table.find(atoll(value_list[0].c_str()));
        if (iter == my_table.end())
        {
            my_table.emplace(_self, [&](auto &new_data) {
                new_data.id = atoll(value_list[0].c_str());
                new_data.reward_utg = atoi(value_list[1].c_str());
                new_data.rank_exp = atoi(value_list[2].c_str());
                new_data.char_exp = atoi(value_list[3].c_str());
                new_data.reward_count = atoi(value_list[4].c_str());
                new_data.per_monster = atoi(value_list[5].c_str());
                new_data.per_equipment = atoi(value_list[6].c_str());
            });
        }
        else
        {
            my_table.modify(iter, _self, [&](auto &new_data) {
                new_data.reward_utg = atoi(value_list[1].c_str());
                new_data.rank_exp = atoi(value_list[2].c_str());
                new_data.char_exp = atoi(value_list[3].c_str());
                new_data.reward_count = atoi(value_list[4].c_str());
                new_data.per_monster = atoi(value_list[5].c_str());
                new_data.per_equipment = atoi(value_list[6].c_str());
            });
        }
    }
    if(_table == "dbdayreward")
    {
        substr_value(_value, value_list, size_list, 7);
        day_reward_db my_table(_self, _self.value);
        auto iter = my_table.find(atoll(value_list[0].c_str()));
        if (iter == my_table.end())
        {
            my_table.emplace(_self, [&](auto &new_data) {
                new_data.id = atoll(value_list[0].c_str());
                new_data.reward_utg = atoi(value_list[1].c_str());
                new_data.rank_exp = atoi(value_list[2].c_str());
                new_data.char_exp = atoi(value_list[3].c_str());
                new_data.reward_count = atoi(value_list[4].c_str());
                new_data.per_monster = atoi(value_list[5].c_str());
                new_data.per_equipment = atoi(value_list[6].c_str());
            });
        }
        else
        {
            my_table.modify(iter, _self, [&](auto &new_data) {
                new_data.reward_utg = atoi(value_list[1].c_str());
                new_data.rank_exp = atoi(value_list[2].c_str());
                new_data.char_exp = atoi(value_list[3].c_str());
                new_data.reward_count = atoi(value_list[4].c_str());
                new_data.per_monster = atoi(value_list[5].c_str());
                new_data.per_equipment = atoi(value_list[6].c_str());
            });
        }
    }
    // if(_table == "dballitem")
    // {
    //         substr_value(_value, value_list, size_list, 6);
    //     allitem_db my_table(_self, _self.value);
    //     auto iter = my_table.find(atoll(value_list[0].c_str()));
    //     if (iter == my_table.end())
    //     {
    //         my_table.emplace(_self, [&](auto &new_data) {
    //             new_data.id = atoll(value_list[0].c_str());
    //             new_data.type = atoi(value_list[1].c_str());
    //             new_data.grade = atoi(value_list[2].c_str());
    //             new_data.max_count = atoi(value_list[3].c_str());
    //             new_data.sell_item_id = atoi(value_list[4].c_str());
    //             new_data.sell_item_count = atoi(value_list[5].c_str());
    //         });
    //     }
    //     else
    //     {
    //         my_table.modify(iter, _self, [&](auto &new_data) {
    //             new_data.type = atoi(value_list[1].c_str());
    //             new_data.grade = atoi(value_list[2].c_str());
    //             new_data.max_count = atoi(value_list[3].c_str());
    //             new_data.sell_item_id = atoi(value_list[4].c_str());
    //             new_data.sell_item_count = atoi(value_list[5].c_str());
    //         });
    //     }
    // }
    // if (_table == "dblimitpool")
    // {
    //     substr_value(_value, value_list, size_list, 2);
    //     insert_limit_pool(atoll(value_list[0].c_str()), atoll(value_list[1].c_str()));
    // }
    // if (_table == "limit_log")
    // {
    //     value = atoll(_value.c_str());
    //     insert_limit_log(value);
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

    //eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 1");
    if (_list == "dbnewreward_rewardlist")
    {
        uint64_t stage_id = atoll(_primary_key.c_str());
        new_reward_db reward_db_table(_self, _self.value);
        auto stage_db_iter = reward_db_table.find(stage_id);
        eosio_assert(stage_db_iter != reward_db_table.end(), "DB List Insert : Not Exist Reward");

        reward_db_table.modify(stage_db_iter, _self, [&](auto &new_data) {
            for (uint32_t i = 0; i < _value_list.size();)
            {
                reward_item_info new_reward;
                new_reward.id = atoi(_value_list[i].c_str());
                new_reward.per = atoi(_value_list[i + 1].c_str());
                new_reward.count = atoi(_value_list[i + 2].c_str());
                new_data.reward_list.push_back(new_reward);
                i += 3;
            }
        });
    }
    if (_list == "dbdayreward_rewardlist")
    {
        uint64_t stage_id = atoll(_primary_key.c_str());
        day_reward_db reward_db_table(_self, _self.value);
        auto stage_db_iter = reward_db_table.find(stage_id);
        eosio_assert(stage_db_iter != reward_db_table.end(), "DB List Insert : Not Exist Reward");

        reward_db_table.modify(stage_db_iter, _self, [&](auto &new_data) {
            for (uint32_t i = 0; i < _value_list.size();)
            {
                reward_item_info new_reward;
                new_reward.id = atoi(_value_list[i].c_str());
                new_reward.per = atoi(_value_list[i + 1].c_str());
                new_reward.count = atoi(_value_list[i + 2].c_str());
                new_data.reward_list.push_back(new_reward);
                i += 3;
            }
        });
    }
}

ACTION battletest::dbinit(std::string _table)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    // eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 4");
    if (_table == "dbstageinfo")
    {
        stageinfo_db my_table(_self, _self.value);
        for (auto iter = my_table.begin(); iter != my_table.end();)
        {
            auto erase_iter = my_table.find(iter->primary_key());
            iter++;
            my_table.erase(erase_iter);
        }
    }
    if (_table == "dbdailystage")
    {
        daily_stage_db my_table(_self, _self.value);
        for (auto iter = my_table.begin(); iter != my_table.end();)
        {
            auto erase_iter = my_table.find(iter->primary_key());
            iter++;
            my_table.erase(erase_iter);
        }
    }
    if (_table == "dbdayreward")
    {
        day_reward_db my_table(_self, _self.value);
        for (auto iter = my_table.begin(); iter != my_table.end();)
        {
            auto erase_iter = my_table.find(iter->primary_key());
            iter++;
            my_table.erase(erase_iter);
        }
    }
    if (_table == "tlimit")
    {
        limit_log my_table(_self, _self.value);
        for (auto iter = my_table.begin(); iter != my_table.end();)
        {
            auto erase_iter = my_table.find(iter->primary_key());
            iter++;
            my_table.erase(erase_iter);
        }
    }
    if (_table == "dbnewreward")
    {
        new_reward_db my_table(_self, _self.value);
        for (auto iter = my_table.begin(); iter != my_table.end();)
        {
            auto erase_iter = my_table.find(iter->primary_key());
            iter++;
            my_table.erase(erase_iter);
        }
    }
    if (_table == "dblimitpool")
    {
        limit_gacha_db my_table(_self, _self.value);
        for (auto iter = my_table.begin(); iter != my_table.end();)
        {
            auto erase_iter = my_table.find(iter->primary_key());
            iter++;
            my_table.erase(erase_iter);
        }
    }
}

ACTION battletest::dberase(std::string _table, std::string _value)
{
    // system_master system_master_table(_self, _self.value);
    // auto system_master_iter = system_master_table.begin();

    // permission_level master_auth;
    // master_auth.actor = system_master_iter->master;
    // master_auth.permission = "active"_n;
    // require_auth(master_auth);

    // //eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 3");

    // std::vector<size_t> size_list;
    // std::vector<std::string> value_list;
    // uint64_t value;

    // if(_table == "dbdayreward_rewardlist")
    // {
    //     value = atoll(_value.c_str());
    //     day_reward_db day_reward_db_table(_self, _self.value);
    //     auto iter = day_reward_db_table.find(value);
    //     day_reward_db_table.modify(iter, _self, [&](auto &new_data)
    //     {
    //         new_data.reward_list.clear();
    //     });
        
    // }

}

ACTION battletest::dbinsert(std::string _table, std::string _value)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    //eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 1");

    std::vector<size_t> size_list;
    std::vector<std::string> value_list;
    uint32_t value;

    if(_table == "dbdayreward")
    {
        substr_value(_value, value_list, size_list, 7);
        day_reward_db my_table(_self, _self.value);
        auto iter = my_table.find(atoll(value_list[0].c_str()));
        if (iter == my_table.end())
        {
            my_table.emplace(_self, [&](auto &new_data) {
                new_data.id = atoll(value_list[0].c_str());
                new_data.reward_utg = atoi(value_list[1].c_str());
                new_data.rank_exp = atoi(value_list[2].c_str());
                new_data.char_exp = atoi(value_list[3].c_str());
                new_data.reward_count = atoi(value_list[4].c_str());
                new_data.per_monster = atoi(value_list[5].c_str());
                new_data.per_equipment = atoi(value_list[6].c_str());
            });
        }
        else
        {
            my_table.modify(iter, _self, [&](auto &new_data) {
                new_data.reward_utg = atoi(value_list[1].c_str());
                new_data.rank_exp = atoi(value_list[2].c_str());
                new_data.char_exp = atoi(value_list[3].c_str());
                new_data.reward_count = atoi(value_list[4].c_str());
                new_data.per_monster = atoi(value_list[5].c_str());
                new_data.per_equipment = atoi(value_list[6].c_str());
            });
        }
    }
}

ACTION battletest::dblistinsert(std::string _list, std::string _primary_key, std::vector<std::string> _value_list)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    //eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 1");
    if (_list == "dbdayreward_rewardlist")
    {
        uint64_t stage_id = atoll(_primary_key.c_str());
        day_reward_db reward_db_table(_self, _self.value);
        auto stage_db_iter = reward_db_table.find(stage_id);
        eosio_assert(stage_db_iter != reward_db_table.end(), "DB List Insert : Not Exist Reward");

        reward_db_table.modify(stage_db_iter, _self, [&](auto &new_data) {
            for (uint32_t i = 0; i < _value_list.size();)
            {
                reward_item_info new_reward;
                new_reward.id = atoi(_value_list[i].c_str());
                new_reward.per = atoi(_value_list[i + 1].c_str());
                new_reward.count = atoi(_value_list[i + 2].c_str());
                new_data.reward_list.push_back(new_reward);
                i += 3;
            }
        });
    }
}

ACTION battletest::dbinit(std::string _table)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    //eosio_assert(system_master_iter->state == system_state::pause, "Not Server Pause 4");
    if (_table == "dbdayreward")
    {
        day_reward_db my_table(_self, _self.value);
        for (auto iter = my_table.begin(); iter != my_table.end();)
        {
            auto e_iter = my_table.find(iter->primary_key());
            iter++;
            my_table.erase(e_iter);
        }
    }
}