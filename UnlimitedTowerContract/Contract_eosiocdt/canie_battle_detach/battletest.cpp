#include "Common/common_header.hpp"
#include "Common/common_seed.hpp"
#include "Common/common_math.hpp"
#include "battletest.hpp"

battletest::servant_db::const_iterator battletest::get_servant_db(uint64_t _id)
{
    servant_db servant_db_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
    auto servant_db_iter = servant_db_table.find(_id);
    eosio_assert(servant_db_iter != servant_db_table.end(), "Servant DB : Empty Servant ID");
    return servant_db_iter;
}

battletest::monster_db::const_iterator battletest::get_monster_db(uint64_t _id)
{
    monster_db my_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
    auto iter = my_table.find(_id);
    eosio_assert(iter != my_table.end(), "Monster DB : Empty Monster ID");
    return iter;
}
battletest::tribe_db::const_iterator battletest::get_tribe_db(uint64_t _id)
{
    tribe_db my_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
    auto iter = my_table.find(_id);
    eosio_assert(iter != my_table.end(), "Tribe DB : Empty Tribe ID");
    return iter;
}

battletest::passive_db::const_iterator battletest::get_passive_db(uint64_t _id)
{
    passive_db my_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
    auto iter = my_table.find(_id);
    eosio_assert(iter != my_table.end(), "Passive DB : Empty Passive ID");
    return iter;
}
battletest::allitem_db::const_iterator battletest::get_allitem_db(uint64_t _id)
{
    allitem_db my_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
    auto iter = my_table.find(_id);
    eosio_assert(iter != my_table.end(), "all item DB : Empty Shop ID");
    return iter;
}

void battletest::system_check(eosio::name _user)
{
    require_auth(_user);
    system_master system_master_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter != system_master_table.end(),"System Check : Need Master");
    if (system_master_iter->state == system_state::pause)
    {
        whitelist whitelist_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
        auto whitelist_iter = whitelist_table.find(_user.value);
        eosio_assert(whitelist_iter != whitelist_table.end(), "System Check : Server Pause");
    }
    blacklist blacklist_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "System Check : BlackList User");
}

bool battletest::check_inventory(eosio::name _user, uint32_t _count)
{
    user_auths user_auth_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
    auto users_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(users_auth_iter != user_auth_table.end(), "check_inventory : Not Exist User");

    if (users_auth_iter->current_servant_inventory + _count > users_auth_iter->servant_inventory)
    {
        return false;
    }
    else if (users_auth_iter->current_monster_inventory + _count > users_auth_iter->monster_inventory)
    {
        return false;
    }
    else if (users_auth_iter->current_equipment_inventory + _count > users_auth_iter->equipment_inventory)
    {
        return false;
    }
    else if (users_auth_iter->current_item_inventory + _count > users_auth_iter->item_inventory)
    {
        return false;
    }
    else
    {
        return true;
    }
}

void battletest::check_enter_stage(eosio::name _user, uint32_t _stage_id)
{
    stageinfo_db stage_db_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
    auto stage_db_iter = stage_db_table.find(_stage_id);
    eosio_assert(stage_db_iter != stage_db_table.end(), "Enter Stage Check : Empty Stage / Wrong Stage ID");

    //인라인 처리
    if (stage_db_iter->need_entrance_item_id != 0)
    {
        uint32_t check_inventory = 0;
        user_items user_item_table(MAIN_GAME_CONTRACT, _user.value);
        auto item_iter = user_item_table.find(stage_db_iter->need_entrance_item_id);
        eosio_assert(item_iter != user_item_table.end(), "Enter Stage Check : Empty Enter Stage Item");

        uint64_t check_total_item_count = 0;
        for (uint64_t h = 0; h < item_iter->item_list.size(); h++)
        {
            check_total_item_count += item_iter->item_list[h].count; //내가 가지고 있는 아이템 갯수 총 합
        }

        if(check_total_item_count == stage_db_iter->need_entrance_item_count)
        {
            check_inventory += 1;
            user_item_table.erase(item_iter);
        }
        else
        {
            uint32_t need_stage_item_count = stage_db_iter->need_entrance_item_count;
            user_item_table.modify(item_iter, _self, [&](auto &new_data) {
                for (uint32_t i = new_data.item_list.size() - 1; i >= 0; i--)
                {
                    if (new_data.item_list[i].count > need_stage_item_count) //필요한 입장권 수보다 많으면
                    {
                        new_data.item_list[i].count -= need_stage_item_count;
                        need_stage_item_count = 0;
                        break;
                    }
                    else if (new_data.item_list[i].count == need_stage_item_count) //필요한 입장권 수랑 같으면
                    {
                        need_stage_item_count = 0;
                        new_data.item_list.erase(new_data.item_list.begin() + i);
                        check_inventory += 1;
                        break;
                    }
                    else if (new_data.item_list[i].count < need_stage_item_count) //필요한 입장권 수보다 적으면
                    {
                        need_stage_item_count -= new_data.item_list[i].count;
                        new_data.item_list.erase(new_data.item_list.begin() + i);
                        check_inventory += 1;
                    }
                }
            });
            eosio_assert(need_stage_item_count == 0, "Enter Stage Check : Need More Enter Stage Item");
        }
       
        user_auths user_auth_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
        auto user_auth_iter = user_auth_table.find(_user.value);
        user_auth_table.modify(user_auth_iter, _self, [&](auto &add_auth) {
            add_auth.current_item_inventory -= check_inventory;
        });
    }
}

bool battletest::timecheck(uint64_t user_checktime)
{    
    uint64_t server_standard_time = now();    
    uint64_t server_check_time = ( server_standard_time  / 86400);           

    if(user_checktime == server_check_time)
    {
        return false;
    }
    else
    {
        return true;
    }
}

uint64_t battletest::get_user_seed_value(uint64_t _user)
{
    uint64_t user;

    user_logs user_log_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
    const auto &user_log_iter = user_log_table.get(_user, "Seed Value : Log Table Empty / Not Yet Signup");
    user = _user + user_log_iter.gacha_num + now();
    if (user <= _user)
    {
        user = user_log_iter.gacha_num + now();
    }

    return user;
}

uint64_t battletest::get_random_grade(uint64_t _rate)
{
    uint64_t grade;
    grade_ratio_db grade_ratio_db_table(MAIN_GAME_CONTRACT, MAIN_GAME_CONTRACT.value);
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

uint32_t battletest::get_servant_index(uint32_t _job, uint32_t _body, uint32_t _gender, uint32_t _head, uint32_t _hair)
{
    uint32_t result_id;
    result_id = (_job * 1000000) + (_body * 100000) + (_gender * 10000) + (_head * 100) + (_hair * 1);
    return result_id;
}
battletest::servant_info battletest::get_servant_random_state(uint32_t _id, uint64_t _seed, uint32_t _job, uint32_t _base_str, uint32_t _base_dex, uint32_t _base_int)
{
        //servant_data new_data;
        servant_info new_servant;
        new_servant.id = _id;
        new_servant.exp = 0;
        new_servant.grade = 5;
        new_servant.level = 1;
        new_servant.limit_break = 0;

        new_servant.status.basic_str = safeseed::get_random_value(_seed, 10, 0, 4);
        new_servant.status.basic_dex = safeseed::get_random_value(_seed, 10, 0, 5);
        new_servant.status.basic_int = safeseed::get_random_value(_seed, 10, 0, 6);

        new_servant.status.basic_str = change_servant_status(new_servant.status.basic_str) + _base_str;
        new_servant.status.basic_dex = change_servant_status(new_servant.status.basic_dex) + _base_dex;
        new_servant.status.basic_int = change_servant_status(new_servant.status.basic_int) + _base_int;

        new_servant.equip_slot.resize(3);
        new_servant.state = object_state::on_inventory;

        uint32_t active_id = get_servant_active_skill(_job, _seed);
        new_servant.active_skill.push_back(active_id);

        uint32_t passive_id = get_passive_skill(1, _job, _seed);
        new_servant.passive_skill.push_back(passive_id);

        return new_servant;
}

battletest::monster_info battletest::get_monster_random_state(uint32_t _id, uint64_t _seed, uint32_t _grade, uint32_t _tribe, uint32_t _type, uint32_t _base_str, uint32_t _base_dex, uint32_t _base_int)
{
        monster_info new_monster;
        new_monster.id = _id;
        new_monster.grade = _grade;
        new_monster.tribe = _tribe;
        new_monster.type = _type;
        new_monster.exp = 0;
        new_monster.upgrade = 0;
        new_monster.level = 1;
        new_monster.limit_break = 0;

        new_monster.status.basic_str = safeseed::get_random_value(_seed, 10, 0, 3);
        new_monster.status.basic_dex = safeseed::get_random_value(_seed, 10, 0, 4);
        new_monster.status.basic_int = safeseed::get_random_value(_seed, 10, 0, 5);

        new_monster.status.basic_str = change_monster_status(_grade, new_monster.status.basic_str);
        new_monster.status.basic_dex = change_monster_status(_grade, new_monster.status.basic_dex);
        new_monster.status.basic_int = change_monster_status(_grade, new_monster.status.basic_int);

        new_monster.status.basic_str = (new_monster.status.basic_str * _base_str) / 100;
        new_monster.status.basic_dex = (new_monster.status.basic_dex * _base_dex) / 100;
        new_monster.status.basic_int = (new_monster.status.basic_int * _base_int) / 100;

        new_monster.state = object_state::on_inventory;

        uint32_t passive_id = get_passive_skill(2, _tribe, _seed);
        new_monster.passive_skill.push_back(passive_id);

        return new_monster;

}
battletest::equipment_info battletest::get_equip_random_state(uint32_t _id, uint64_t _seed, uint32_t _type, uint32_t _tier, uint32_t _job, uint32_t _grade)
{
    equipment_info new_item;
    new_item.id = _id;
    new_item.type = _type;
    new_item.tier = _tier;
    new_item.job = _job;
    new_item.grade = _grade;

    uint32_t type_grade = ((_type + 1) * 10) + _grade;
    new_item.value = safeseed::get_random_value(_seed, 10, 0, 3);
    new_item.value = change_equipment_statue(type_grade, new_item.value);
    set_tier_status(new_item.value, _tier);

    new_item.state = object_state::on_inventory;

    return new_item;
}

void battletest::write_log(eosio::name _user, uint32_t _gold_type, uint32_t _gacha_type, uint32_t _gacha_index, uint32_t _inventory_count)
{
    user_auths auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(_user.value);
    eosio_assert(auth_user_iter != auth_user_table.end(), "Write log : Empty Auth Table / Not Yet Signup");

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Write log : Log Table Empty / Not yet signup");

    // eosio_assert(gold_logs_iter != gold_logs_table.end(), "Write log : Gold Log Table Empty / Not yet signup");

    result_info result;

    result.index = _gacha_index;

    uint32_t servant_add_inventory = 0;
    uint32_t monster_add_inventory = 0;
    uint32_t equipment_add_inventory = 0;
    uint32_t item_add_inventory = 0;

    uint32_t log_servant_num = 0;
    uint32_t log_monster_num = 0;
    uint32_t log_equip_num = 0;
    uint32_t log_item_num = 0;
    uint32_t log_gacha_num = 0;
    uint32_t log_mail_count = 0;

    if (_gold_type == use_money_type::EOS_GACHA) // EOS 가차
    {
        if (_gacha_type == 1) //서번트
        {
            result.type = result::servant;
            log_gacha_num += 1;
            log_servant_num += 1;
            servant_add_inventory += 1;
        }
        else if (_gacha_type == 2)
        {
            result.type = result::monster;
            log_gacha_num += 1;
            log_monster_num += 1;
            monster_add_inventory += 1;
        }
        else if (_gacha_type == 3)
        {
            result.type = result::equipment;
            log_gacha_num += 1;
            log_equip_num += 1;
            equipment_add_inventory += 1;
        }

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
    }

    else if (_gold_type == 2) //UTG 가차
    {
        if (_gacha_type == 2)
        {
            result.type = result::monster;
            log_gacha_num += 1;
            log_monster_num +=1;
            monster_add_inventory += 1;
        }
        else if (_gacha_type == 3)
        {
            result.type = result::equipment;
            log_gacha_num += 1;
            log_equip_num +=1;
            equipment_add_inventory += 1;
        }
        else if (_gacha_type == 4)
        {
            result.type = result::item;
            log_gacha_num += 1;
            log_item_num += 1;
            item_add_inventory += _inventory_count;
        }

        gold_gacha_results gold_gacha_result_table(_self, _self.value);
        gold_gacha_totals gold_gacha_total_table(_self, _self.value);

        auto gold_gacha_result_iter = gold_gacha_result_table.find(_user.value);
        if (gold_gacha_result_iter == gold_gacha_result_table.end())
        {
            gold_gacha_result_table.emplace(_self, [&](auto &new_result) {
                new_result.user = _user;
                new_result.result = result;
            });
        }
        else
        {
            gold_gacha_result_table.modify(gold_gacha_result_iter, _self, [&](auto &new_result) {
                new_result.result = result;
            });
        }

        auto gold_gacha_total_iter = gold_gacha_total_table.find(_user.value);
        if (gold_gacha_total_iter == gold_gacha_total_table.end())
        {
            gold_gacha_total_table.emplace(_self, [&](auto &new_result) {
                new_result.user = _user;
                new_result.result_list.push_back(result);
            });
        }
        else
        {
            gold_gacha_total_table.modify(gold_gacha_total_iter, _self, [&](auto &new_result) {
                new_result.result_list.push_back(result);
            });
        }
    }
    else if (_gold_type == 3) //ETC
    {
        if (_gacha_type == 1)
        {
            log_servant_num += 1;
        }
        else if (_gacha_type == 2)
        {
            log_monster_num += 1;
        }
        else if (_gacha_type == 3)
        {
            log_equip_num += 1;
        }
        else if (_gacha_type == 4)
        {
            log_item_num += 1;
        }
        log_mail_count += 1;
    }
    else if (_gold_type == 4) //배틀
    {
        if (_gacha_type == 1)
        {
            servant_add_inventory += 1;
            log_servant_num +=1;
        }
        else if (_gacha_type == 2)
        {
            monster_add_inventory += 1;
            log_monster_num +=1;
        }
        else if (_gacha_type == 3)
        {
            equipment_add_inventory += 1;
            log_equip_num +=1;
        }
        else if (_gacha_type == 4)
        {
            item_add_inventory += _inventory_count;
        }
    }
    else if(_gold_type ==5) //이벤트
    {
        log_mail_count += 1;
    }

    user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
        update_log.servant_num += log_servant_num;
        update_log.monster_num += log_monster_num;
        update_log.equipment_num += log_equip_num;
        update_log.gacha_num += log_gacha_num;
        update_log.mail += log_mail_count;
    });

    gold_logs gold_logs_table(_self, _self.value);
    auto gold_logs_iter = gold_logs_table.find(_user.value);
    if (gold_logs_iter == gold_logs_table.end())
    {
        gold_logs_table.emplace(_self, [&](auto &new_user) {
            new_user.user = _user;
            new_user.monster_num = log_monster_num;
            new_user.equipment_num = log_equip_num;
            new_user.item_num = log_item_num;
            new_user.gold_gacha_num = log_gacha_num;
            new_user.use_utg = 0;
        });
    }
    else
    {
        gold_logs_table.modify(gold_logs_iter, _self, [&](auto &new_data) {
            new_data.gold_gacha_num += log_gacha_num;
            new_data.monster_num += log_monster_num;
            new_data.equipment_num += log_equip_num;
            new_data.item_num += log_item_num;
        });
    }
    auth_user_table.modify(auth_user_iter, _self, [&](auto &update_auth_user) {
        update_auth_user.current_servant_inventory += servant_add_inventory;
        update_auth_user.current_monster_inventory += monster_add_inventory;
        update_auth_user.current_equipment_inventory += equipment_add_inventory;
        update_auth_user.current_item_inventory += item_add_inventory;
    });
}
uint32_t battletest::sum_item_check(eosio::name _user, uint32_t _item_id, uint32_t _count)
{
    uint64_t add_inventory = 0;

    // allitem_db allitem_db_table(_self, _self.value);
    // const auto &allitem_db_iter = allitem_db_table.get(_item_id, " Gacha Item : Empty Item ID / Not Set Item ID");
    auto allitem_db_iter = get_allitem_db(_item_id);
    
    user_auths auth_user_table(_self, _self.value);
    auto auth_user_iter = auth_user_table.find(_user.value);
    eosio_assert(auth_user_iter != auth_user_table.end(), " Gacha Item : Empty Auth Table / Not Yet Signup");

    user_items user_items_table(_self, _user.value);
        auto user_items_iter = user_items_table.find(allitem_db_iter->id);
    if (user_items_iter == user_items_table.end())
    {
        user_items_table.emplace(_self, [&](auto &change_consumable) {

            change_consumable.id = allitem_db_iter->id;
            change_consumable.type = allitem_db_iter->type;

            if(_count > 99)
            {
                item_info get_item_info;
                get_item_info.index = 0;
                get_item_info.count = 99;
    
                change_consumable.item_list.push_back(get_item_info);

                item_info get_second_item_info;
                get_second_item_info.index = 1;
                get_second_item_info.count = _count - 99;
    
                change_consumable.item_list.push_back(get_second_item_info);
                add_inventory = 2;
            }
            else
            {
                item_info get_item_info;
                get_item_info.index = 0;
                get_item_info.count = _count;

                change_consumable.item_list.push_back(get_item_info);
                add_inventory = 1;
            }
       });
    }
    else
    {
        user_items_table.modify(user_items_iter, _self, [&](auto &change_consumable) {
            
            uint64_t size_count = change_consumable.item_list.size();

            for (uint64_t i = 0; i < size_count; i++)
            {
                if(change_consumable.item_list[i].count < 99)
                {
                    if(change_consumable.item_list[i].count + _count > 99)
                    {
                        uint64_t new_count = change_consumable.item_list[i].count + _count - 99;
                        change_consumable.item_list[i].count = 99;

                        if(new_count >= 99)
                        {

                            item_info get_item_info;
                            get_item_info.index = size_count;
                            get_item_info.count = 99;
                            change_consumable.item_list.push_back(get_item_info);

                            add_inventory = 1;

                            uint64_t new_second_count = new_count - 99;
                            if(new_second_count > 0)
                            {
                                item_info get_second_item_info;
                                get_second_item_info.index = size_count + 1;
                                get_second_item_info.count = new_second_count;
                                change_consumable.item_list.push_back(get_second_item_info);
                                
                                add_inventory = 2;
                            }
                        }
                        else
                        {
                            item_info get_item_info;
                            get_item_info.index = size_count;
                            get_item_info.count = new_count;
                            change_consumable.item_list.push_back(get_item_info);
                            add_inventory = 1;
                        }
                    }
                    else
                    {
                        change_consumable.item_list[i].count += _count;
                    }
                }
                else if(change_consumable.item_list[i].count == 99 && i == (size_count - 1))
                {
                        uint64_t new_count = _count;

                        if(new_count >= 99)
                        {

                            item_info get_item_info;
                            get_item_info.index = size_count;
                            get_item_info.count = 99;
                            change_consumable.item_list.push_back(get_item_info);

                            add_inventory = 1;

                            uint64_t new_second_count = new_count - 99;
                            if(new_second_count > 0)
                            {
                                item_info get_second_item_info;
                                get_second_item_info.index = size_count + 1;
                                get_second_item_info.count = new_second_count;
                                change_consumable.item_list.push_back(get_second_item_info);
                                
                                add_inventory = 2;
                            }
                        }
                        else
                        {
                            item_info get_item_info;
                            get_item_info.index = size_count;
                            get_item_info.count = new_count;
                            change_consumable.item_list.push_back(get_item_info);
                            add_inventory = 1;
                        }
                }
            }
        });
    }
    return add_inventory;
}

uint32_t battletest::sub_item_check(eosio::name _user, uint32_t _item_id, uint32_t _count)
{
    // allitem_db allitem_db_table(_self, _self.value);
    // auto allitem_db_iter = allitem_db_table.find(_item_id);
    // eosio_assert(allitem_db_iter != allitem_db_table.end(), "sub_item_check : Not exist allitem data");
    auto allitem_db_iter = get_allitem_db(_item_id);

    item_data new_item;
    new_item.id = allitem_db_iter->id;
    new_item.type = allitem_db_iter->type;

    user_items user_items_table(_self, _user.value);
    auto user_items_iter = user_items_table.find(allitem_db_iter->id);
    eosio_assert(user_items_iter != user_items_table.end(), "sub_item_check : Invalid User Item Table");

    uint64_t sub_item_count = _count;
    //uint64_t sub_item_count = item_shop_iter->price_count * _count;
    uint64_t sub_inventory = 0;

    uint32_t target_item_count = user_items_iter->item_list.size();
    uint32_t total_item_count = 0;
    for (uint32_t i = 0; i < target_item_count; i++)
    {
        total_item_count += user_items_iter->item_list[i].count;
    }
    eosio_assert(total_item_count >= sub_item_count, "sub_item_check : Not Enough Total Item Count");

    if (total_item_count == sub_item_count)
    {
                sub_inventory += 1;
        user_items_table.erase(user_items_iter);
    }
    else
    {
        user_items_table.modify(user_items_iter, _self, [&](auto &change_user_item) {
            auto sub_item_iter = change_user_item.item_list.end() - 1;
            for (uint32_t j = target_item_count - 1; j >= 0; j--)
            {
                if (change_user_item.item_list[j].count > sub_item_count)
                {
                    change_user_item.item_list[j].count -= sub_item_count;
                    break;
                }
                else
                {
                    sub_item_count -= change_user_item.item_list[j].count;
                    change_user_item.item_list.erase(sub_item_iter);
                    sub_inventory += 1;
                    sub_item_iter--;
                }
            }
        });
    }

    return sub_inventory;
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

    //auto active_tier = get_active_db(active_id);

    return active_id;
}

uint32_t battletest::change_servant_status(uint32_t _status_grade)
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

uint32_t battletest::get_passive_skill(uint32_t _type, uint32_t _job_or_tribe, uint64_t _seed)
{
    uint32_t passive_id = 0;

    main_gacha_db main_gacha_db_table(_self, _self.value);
    auto p_passive_iter = main_gacha_db_table.find(GACHA_PUBLIC_PASSIVE_COUNT_ID);
    eosio_assert(p_passive_iter != main_gacha_db_table.end(), "Get Passive Skill : Empty Public Passive / Wrong Gacha ID");

    uint32_t random_rate = safeseed::get_random_value(_seed, p_passive_iter->db_index, 0, 1);
    random_rate += 1;
    random_rate += (10000 * p_passive_iter->gacha_id);

    auto passvie = main_gacha_db_table.find(random_rate);
    eosio_assert(passvie != main_gacha_db_table.end(), "Get Passvie Skill : Empty Gacha ID / Wrong Gacha ID");
    passive_id = passvie->db_index;

    // passive_db passsive_db_table(_self, _self.value);
    // auto passive_iter = passsive_db_table.find(passive_id);
    // eosio_assert(passive_iter != passsive_db_table.end(), "Get Passvie Skill : Empty Passvie ID / Wrong Passvie ID");
    auto passive_iter = get_passive_db(passive_id);

    return passive_id;
}
uint32_t battletest::change_monster_status(uint32_t _grade, uint32_t _status_grade)
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
    eosio_assert(a != 0, "change_monster_status : Not Exist Status Monster 1");
    return 0;
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
    //장비 티어/1티어=0.29, 2티어=0.44, 3티어=0.66,4티어=1
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


//------------------------------------------------------------------------//
//---------------------------------battle_function------------------------//
//------------------------------------------------------------------------//
uint32_t battletest::get_max_hp(status_info _status, uint32_t _level)
{
    uint32_t hp;
    hp = _status.basic_str * 66;    //str * 66 * (cur_level / 50) + str * 22
    hp = hp * ((_level * decimal) / 50) + (_status.basic_str * 22 * decimal) ;
    return hp / decimal ;
}
uint32_t battletest::get_physical_attack(status_info _status, uint32_t _level)
{
    uint32_t physical_attack;
    physical_attack = (_status.basic_str + _status.basic_dex) * 8;     //(str + dex) * 8 * (cur_level / 50) + (str + dex) * 2  19.05.03 영호 수정(힘민지에 따른 스탯 공식 수정)
    physical_attack = physical_attack * (((_level * decimal) / 50)) + ((_status.basic_str + _status.basic_dex) * 2 * decimal);
    return physical_attack / decimal;
}
uint32_t battletest::get_magic_attack(status_info _status, uint32_t _level)
{
    uint32_t magic_attack;
    magic_attack = (_status.basic_int * 8);                    //(int * 8) * (cur_level / 50) + (int * 2) 
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
    cri_per = ((_status.basic_dex * 1000) / 500) / 10;
    cri_per += 5;   //5 기본값이 5%
    return cri_per;
}

uint32_t battletest::get_cri_dmg_per(status_info _status, uint32_t _level)
{
    uint32_t cri_dmg_per;
    cri_dmg_per = (_status.basic_int * 5) / 10; // 지능 * 0.5
    cri_dmg_per += 120; 
    return cri_dmg_per;
}


uint32_t battletest::get_damage(uint32_t _atk, uint32_t _dfs)
{
    uint32_t damage = ((_atk * ((defense_constant * decimal) / (defense_constant + _dfs))));
    damage = damage / decimal;
    return damage;
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
    if (_upgrade == 0)
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
        //auto user_servant_iter = get_user_servant(_user, _index);

        basic_status = user_servant_iter->servant.status;   //테이블에 들어갈 기본 스탯
        status = user_servant_iter->servant.status;         //hp를 계산하기 위한 증가 스탯
        //서번트 아이디가 제대로 된 아이디인지 확인
        // servant_db servant_db_table(_self, _self.value);
        // auto servant_db_iter = servant_db_table.find(user_servant_iter->servant.id);
        // eosio_assert(servant_db_iter != servant_db_table.end(), "Set User State : Empty Servant ID / Wrong Servant ID");

        auto servant_db_iter = get_servant_db(user_servant_iter->servant.id);

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
            //auto user_equipment_iter = get_user_equipment(_user, user_servant_iter->servant.equip_slot[i]);

            auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
            eosio_assert(db_equipment_iter != equipment_db_table.end(), "Set User State : Empty Equipment ID / Wrong Equipment ID");
            //auto db_equipment_iter = get_equipment_db(user_equipment_iter->equipment.id);

            uint32_t value = user_equipment_iter->equipment.value;
            set_upgrade_equip_status(user_equipment_iter->equipment.grade, value, user_equipment_iter->equipment.upgrade);

            for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
            {
                if (db_equipment_iter->option_list[a] == option_list::status_str)   //힘 증가
                {
                    status.basic_str += value;
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_dex)  //민 증가
                {
                    status.basic_dex += value;
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_int)  //지 증가
                {
                    status.basic_int += value;
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_hp)   //hp 증가
                {
                    increase_hp += value;
                }
            }
        }

        //힘민지에 대한 패시브 적용
        for (uint32_t i = 0; i < user_servant_iter->servant.passive_skill.size(); ++i)
        {
            // passive_db passive_db_table(_self, _self.value);
            // auto passive_db_iter = passive_db_table.find(user_servant_iter->servant.passive_skill[i]);
            // eosio_assert(passive_db_iter != passive_db_table.end(), "Set User State : Empty Servant Passive ID / Wrong Servant Passive ID");
            auto passive_db_iter = get_passive_db(user_servant_iter->servant.passive_skill[i]);
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

        //액티브 스킬의 존재 여부 확인
        for (uint32_t i = 0; i < user_servant_iter->servant.active_skill.size(); ++i)
        {
            active_db active_db_table(_self, _self.value);
            auto active_db_iter = active_db_table.find(user_servant_iter->servant.active_skill[i]);
            eosio_assert(active_db_iter != active_db_table.end(), "Set User State : Empty Servant Active ID / Wrong Servant Active ID");
            //auto active_db_iter = get_active_db(user_servant_iter->servant.active_skill[i]);

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
        //auto user_monster_iter = get_user_monster(_user, _index);

        //몬스터 아이디가 제대로된 아이디인지
        // monster_db monster_db_table(_self, _self.value);
        // auto monster_db_iter = monster_db_table.find(user_monster_iter->monster.id);
        // eosio_assert(monster_db_iter != monster_db_table.end(), "Set User State : Empty Monster ID / Wrong Monster ID");

        auto monster_db_iter = get_monster_db(user_monster_iter->monster.id);

        basic_status = user_monster_iter->monster.status; //몬스터 초기 스탯
        status = user_monster_iter->monster.status;         //hp계산을 위해 증가할 스탯
        
        set_upgrade_monster_status(user_monster_iter->monster.grade, status, user_monster_iter->monster.upgrade); //몬스터 강화 수치 스탯 적용
        //몬스터 패시브 처리
        for (uint32_t i = 0; i < user_monster_iter->monster.passive_skill.size(); ++i)
        {
            // passive_db passive_db_table(_self, _self.value);
            // auto passive_db_iter = passive_db_table.find(user_monster_iter->monster.passive_skill[i]);
            // eosio_assert(passive_db_iter != passive_db_table.end(), "Set User State : Empty Monster Passive ID / Wrong Monster Passive ID");
            auto passive_db_iter = get_passive_db(user_monster_iter->monster.passive_skill[i]);
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

        //몬스터 액티브 스킬 확인
        for (uint32_t i = 0; i < user_monster_iter->monster.active_skill.size(); ++i)
        {
            active_db active_db_table(_self, _self.value);
            auto active_db_iter = active_db_table.find(user_monster_iter->monster.active_skill[i]);
            eosio_assert(active_db_iter != active_db_table.end(), "Set User State : Empty Monster Active ID / Wrong Monster Active ID");
            //auto active_db_iter = get_active_db(user_monster_iter->monster.active_skill[i]);

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
    return get_state;
}

uint32_t battletest::get_stage_id(uint32_t _stage_type, uint32_t _tier, uint32_t _type, uint32_t _grade)
{
    uint32_t stage_id;
    if (_stage_type == 1)
    {
        stage_id = (1000 * _type) + (10 * _tier) + _grade;
    }
    else
    {
        stage_id = (10000 * _stage_type) + (1000 * _type) + (10 * _tier) + _grade;
    }
    return stage_id;
}

uint32_t battletest::get_tribe_count(std::vector<character_state_data> &_my_state_list, uint32_t _tribe)
{
    uint32_t count = 0;
    for (uint32_t i = 0; i < _my_state_list.size(); ++i)
    {
        if (_my_state_list[i].type == character_type::t_monster)
        {
            // monster_db monster_db_table(_self, _self.value);
            // auto monster_iter = monster_db_table.find(_my_state_list[i].id);
            // eosio_assert(monster_iter != monster_db_table.end(), "Get Tribe Count : Empty Monster ID / Wrong Monster ID");
            auto monster_iter = get_monster_db(_my_state_list[i].id);

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
            // monster_db monster_db_table(_self, _self.value);
            // auto monster_iter = monster_db_table.find(_my_state_list[i].id);
            // eosio_assert(monster_iter != monster_db_table.end(),"Synergy Set : Empty Monster ID / Wrong Monster ID");
            auto monster_iter = get_monster_db(_my_state_list[i].id);

            // tribe_db tribe_db_table(_self, _self.value);
            // auto tribe_iter = tribe_db_table.find(monster_iter->tribe);
            // eosio_assert(tribe_iter != tribe_db_table.end(),"Synergy Set : Empty Monser Tribe / Wrong Tribe");
            auto tribe_iter = get_tribe_db(monster_iter->tribe);

            uint32_t tribe_count = get_tribe_count(_my_state_list, tribe_iter->id);
            uint32_t synergy_id = 0;
            if (tribe_count == 2)
            {
                synergy_id = tribe_iter->pair;

                //해당 시너지가 이미 존재하는지 체크
                int flag = false;
                for (uint32_t a = 0; a < _synergy_list.size(); ++a)
                {
                    if (_synergy_list[a] == synergy_id) //같은 시너지가 있으면 들어가면 안된다.
                    {
                        flag = true;
                        break;
                    }
                }

                if ((flag == false) && (synergy_id != 0)) //같은 시너지가 없을때만 들어가게
                {
                    _synergy_list.push_back(synergy_id);
                }
            }
            else if (tribe_count >= 3 && tribe_count < 5)
            {
                synergy_id = tribe_iter->triple;

                //해당 시너지가 이미 존재하는지 체크
                int flag = false;
                for (uint32_t a = 0; a < _synergy_list.size(); ++a)
                {
                    if (_synergy_list[a] == synergy_id) //같은 시너지가 있으면 들어가면 안된다.
                    {
                        flag = true;
                        break;
                    }
                }

                if ((flag == false) && (synergy_id != 0)) //같은 시너지가 없을때만 들어가게
                {
                    _synergy_list.push_back(synergy_id);
                }
            }
            else if (tribe_count >= 5)
            {
                synergy_id = tribe_iter->penta;

                //해당 시너지가 이미 존재하는지 체크
                int flag = false;
                for (uint32_t a = 0; a < _synergy_list.size(); ++a)
                {
                    if (_synergy_list[a] == synergy_id) //같은 시너지가 있으면 들어가면 안된다.
                    {
                        flag = true;
                        break;
                    }
                }

                if ((flag == false) && (synergy_id != 0)) //같은 시너지가 없을때만 들어가게
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
        // passive_db passive_db_table(_self, _self.value);
        // auto passive_iter = passive_db_table.find(_synergy_list[i]);
        // eosio_assert(passive_iter != passive_db_table.end(), "Hp Synergy Set : Empty Passive ID / Wrong Synergy ID");
        auto passive_iter = get_passive_db(_synergy_list[i]);

        if (passive_iter->effect_id == passive_effect_id::hp)       //hp 증가 시너지면
        {
            if (passive_iter->effect_type_id == passive_effect_type_id::per_up) //% 연산일 경우
            {
                if (passive_iter->target_id == passvie_target_id::t_spirit)     //아군 정령족한테만 적용
                {
                    for(uint32_t my = 0; my < _my_state_list.size(); ++my)
                    {
                        if(_my_state_list[my].type == character_type::t_monster)
                        {
                            // monster_db monster_db_table(_self, _self.value);
                            // auto monster_iter = monster_db_table.find(_my_state_list[my].id);
                            // eosio_assert(monster_iter != monster_db_table.end(),"Hp Synergy Set : Empty Monster ID / Wrong Monster ID");

                            auto monster_iter = get_monster_db(_my_state_list[my].id);
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

    if(servant_count == 5 && monster_count == 5)    //파티가 전부 비어있으면
    {
        return false;
    }

    return true;
}

void battletest::new_set_stage_state(uint64_t _stage_id, uint64_t _seed, std::vector<character_state_data> &_enemy_state_list, std::vector<std::string> &_state)
{
    
    std::vector<uint32_t> stage_position = {10, 11, 12, 13, 14, 15, 16, 17, 18, 19};
    std::vector<bool> flag = {false, false, false, false, false, false, false, false, false, false};

    uint32_t enemy_count = 0;
    uint32_t stage_difficult = 0;
    uint32_t stage_enemy_level_max = 0;
    uint32_t stage_enemy_level_min = 0;
    uint32_t stage_elemental_type = 0;

    if (_stage_id / 10000 == 0)
    {
        stageinfo_db stage_db_table(_self, _self.value);
        auto stage_db_iter = stage_db_table.find(_stage_id);
        eosio_assert(stage_db_iter != stage_db_table.end(), "Set Enemy State : Empty Stage / Wrong Stage ID");
        enemy_count = stage_db_iter->enemy_count;
        stage_difficult = stage_db_iter->difficult;
        stage_enemy_level_max = stage_db_iter->enemy_level_max;
        stage_enemy_level_min = stage_db_iter->enemy_level_min;
        stage_elemental_type = stage_db_iter->elemental_type;
    }
    else
    {
        daily_stage_db stage_db_table(_self, _self.value);
        auto stage_db_iter = stage_db_table.find(_stage_id);
        eosio_assert(stage_db_iter != stage_db_table.end(), "Set Enemy State : Empty Daily / Wrong Daily Stage ID");
        enemy_count = stage_db_iter->enemy_count;
        stage_difficult = stage_db_iter->difficult;
        stage_enemy_level_max = stage_db_iter->enemy_level_max;
        stage_enemy_level_min = stage_db_iter->enemy_level_min;
        stage_elemental_type = stage_db_iter->elemental_type;
    }

    enemyinfo_db enemyinfo_db_table(_self, stage_elemental_type);
    auto iter = enemyinfo_db_table.find(0);
    eosio_assert(iter != enemyinfo_db_table.end(), "Set Enmey State : Empty Max Count");
    uint32_t max_count = iter->id;

    for (uint32_t i = 0; i < enemy_count; ++i)
    {
        character_state_data get_state;

        uint32_t new_seed = safeseed::get_seed_value(i, _seed);
        uint32_t random_index = safeseed::get_random_value(new_seed, max_count, 1, 0);

        auto enemy = enemyinfo_db_table.find(random_index);
        eosio_assert(enemy != enemyinfo_db_table.end(), "Set Enemy State : Empty Enemy Index");

        // monster_db monster_db_table(_self, _self.value);
        // auto monster_iter = monster_db_table.find(enemy->id);
        // eosio_assert(monster_iter != monster_db_table.end(), "Set Enemy State : Empty Monster ID");
        
        auto monster_iter = get_monster_db(enemy->id);

        uint32_t enemy_stat_id = (100 * monster_iter->tribe) + stage_difficult;

        enemystat_db enemystat_db_table(_self, _self.value);
        auto stat_iter = enemystat_db_table.find(enemy_stat_id);
        eosio_assert(stat_iter != enemystat_db_table.end(), "Set Enemy State : Empty Stat ID");

        status_info stage_status;
        stage_status.basic_str = stat_iter->base_str;
        stage_status.basic_dex = stat_iter->base_dex;
        stage_status.basic_int = stat_iter->base_int;

        status_info status;
        status = stage_status;

        uint64_t level = safeseed::get_random_value(new_seed, stage_enemy_level_max + 1, stage_enemy_level_min, 1);

        get_state.level = level;
        get_state.grade = stage_difficult;
        get_state.id = monster_iter->id;
        get_state.index = 0;

        //random position
        uint32_t random_pos = safeseed::get_random_value(new_seed, 10, 0, 2);
        if (flag[random_pos] == false)
        {
            flag[random_pos] = true;
            get_state.position = stage_position[random_pos];
        }
        else
        {
            for (uint32_t f = 0; f < 10; ++f)
            {
                if (flag[f] == false)
                {
                    flag[f] = true;
                    get_state.position = stage_position[f];
                    break;
                }
            }
        }

        //raondom active
        if (enemy->active_list.size() != 0)
        {
            uint32_t random_active = safeseed::get_random_value(new_seed, enemy->active_list.size(), 0, 3);
            if (enemy->active_list[random_active] != 0)
            {
                active_db active_db_table(_self, _self.value);
                auto active_db_iter = active_db_table.find(enemy->active_list[random_active]);
                eosio_assert(active_db_iter != active_db_table.end(), "Set Enemy State : Empty Active ID / Wrong Active ID");
                //auto active_db_iter = get_active_db(enemy->active_list[random_active]);

                get_state.active_skill_list.push_back(active_db_iter->active_id);
            }
        }

        uint32_t increase_hp = 0;
        uint32_t increase_hp_per = 0;
        //random passive
        if (enemy->passive_list.size() != 0)
        {
            uint32_t random_passive = safeseed::get_random_value(new_seed, enemy->passive_list.size(), 0, 3);
            if (enemy->passive_list[random_passive] != 0)
            {
                // passive_db passive_db_table(_self, _self.value);
                // auto passive_db_iter = passive_db_table.find(enemy->passive_list[random_passive]);
                // eosio_assert(passive_db_iter != passive_db_table.end(), "Set Enemy State : Empty Passive ID / Wrong Passive ID");
                auto passive_db_iter = get_passive_db(enemy->passive_list[random_passive]);
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
        }

        get_state.now_hp = get_max_hp(status, level) + increase_hp;
        get_state.now_hp += (get_state.now_hp * increase_hp_per) / 100;
        get_state.max_hp = get_state.now_hp;
        get_state.state = battle_member_state::live;
        get_state.status = stage_status;
        get_state.type = character_type::t_monster;
        get_state.upgrade = 0;

        _enemy_state_list.push_back(get_state);
    }
}





void battletest::change_user_state(eosio::name _user, uint32_t _check_state, uint32_t _state)
{
    user_auths user_auth(_self, _self.value);
    auto from_user_iter = user_auth.find(_user.value);
    eosio_assert(from_user_iter != user_auth.end(), "Change User State : Empty Auth Table / Not Yet Signup");
    if (_check_state != 0)
    {
        eosio_assert(from_user_iter->state == _check_state, "Change User State : Check State Not Same");
    }
    if (_state != 0)
    {
        user_auth.modify(from_user_iter, _self, [&](auto &data) {
            data.state = _state;
        });
    }
}

void battletest::init_action_reward_table(eosio::name _user)
{
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
}

uint64_t battletest::get_day_type()
{
    uint64_t day = (now() / 86400) % 7;
    uint64_t type = 0 ;
    if (day == 0)
    {
        type = 4;
    }
    else if (day == 1)
    {
        type = 5;
    }
    else if (day == 2)
    {
        type = 3;
    }
    else if (day == 4)
    {
        type = 6;
    }
    else if (day == 5)
    {
        type = 1;
    }
    else if (day == 6)
    {
        type = 2;
    }
    else //일요일
    {
        type = 0;
    }
    return type;
}

ACTION battletest::stagestart(eosio::name _user, uint32_t _party_number, uint32_t _stage_type, uint32_t _floor, uint32_t _type, uint32_t _difficult)
{
    system_check(_user);

    eosio_assert(check_inventory(_user, 1) == true, "Stage Start : Inventory Is Full");

    std::vector<uint32_t> servant_pos_list = {0, 1, 2, 3, 4};
    std::vector<uint32_t> monster_pos_list = {5, 6, 7, 8, 9};

    user_auths user_auth(_self, _self.value);
    auto user = user_auth.find(_user.value);
    eosio_assert(user != user_auth.end(), "Change User State : Empty Auth Table / Not Yet Signup");
    eosio_assert(user->state == user_state::lobby, "Change User State : Check State Not Same");

    uint32_t stage_id = get_stage_id(_stage_type, _floor, _type, _difficult);

    if (_stage_type == 1)
    {
        stageinfo_db stage_db_table(_self, _self.value);
        auto stage_db_iter = stage_db_table.find(stage_id);
        eosio_assert(stage_db_iter != stage_db_table.end(), "Stage Start : Empty Stage / Not Set Stage");

        check_enter_stage(_user, stage_id);

        user_auth.modify(user, _self, [&](auto &data) {
            data.state = user_state::stage;
        });
    }
    else if(_stage_type == 2)
    {
        //해당 타입의 요일 던전이 입장이 가능한 요일인지 체크하는 예외처리 필요
        uint32_t today_stage = get_day_type();
        if (today_stage != 0)
        {
            eosio_assert(today_stage == _type, "Stage Start : This Stage Another Day");
        }

        daily_stage_db daily_stage_db_table(_self, _self.value);
        auto daily_stage = daily_stage_db_table.find(stage_id);
        eosio_assert(daily_stage != daily_stage_db_table.end(), "Stage Start : Empty Daily Stage / Not Set Daily Stage");

        //데일리 던전 진입이 가능한지 체크
        if(user->daily_init_time == 0)
        {
            user_auth.modify(user, _self, [&](auto &data) {
                data.daily_init_time = (now() / 86400);
                data.daily_enter_count -= 1;
                data.total_enter_count += 1;
                data.state = user_state::stage;
            });
        }
        else
        {
            if (timecheck(user->daily_init_time) == true) //초기화 시간이면
            {
                user_auth.modify(user, _self, [&](auto &data) {
                    data.daily_init_time = (now() / 86400);
                    data.daily_enter_count = 2;
                    data.total_enter_count = 1;
                    data.state = user_state::stage;
                });
            }
            else if (user->total_enter_count >= daily_stage->max_entrance_count) //유저가 총 입장한 횟수와 최대 입장가능 횟수를 비교한다.
            {
                eosio_assert(user->total_enter_count < daily_stage->real_max_entrance_count, "Stage Start : It is impossible to enter today"); //최대 입장 횟수를 했을때 추가 입장 가능 여부를 체크 한다.
                eosio_assert(user->daily_enter_count != 0, "Stage Start : Buy Add Enter Daily Count");
                user_auth.modify(user, _self, [&](auto &data) {
                    data.daily_enter_count -= 1;
                    data.total_enter_count += 1;
                    data.state = user_state::stage;
                });
            }
            else //유저가 최대 입장 가능한 횟수를 안넘었으면
            {
                user_auth.modify(user, _self, [&](auto &data) {
                    data.daily_enter_count -= 1;
                    data.total_enter_count += 1;
                    data.state = user_state::stage;
                });
            }
        }


    }

    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.find(_party_number);
    eosio_assert(user_party_iter != user_party_table.end(), "Stage Start : Empty Party Table / Not Yet Signup");
    eosio_assert(user_party_iter->state == party_state::on_wait, "Stage Start : Party State Wrong");
    eosio_assert(true == possible_start(_user, _party_number), "Stage Start : Empty Party");

    std::vector<std::string> my_state;
    std::vector<std::string> enemy_state;
    
    new_battle_state_list user_battle_table(_self, _self.value);
    auto user_battle_iter = user_battle_table.find(_user.value);
    if (user_battle_iter == user_battle_table.end())
    {
        user_battle_table.emplace(_self, [&](auto &new_battle_set) {
            new_battle_set.user = _user;
            new_battle_set.enemy_user = _user;
            new_battle_set.stage_type = _stage_type;
            new_battle_set.type = _type;
            new_battle_set.floor = _floor;
            new_battle_set.difficult = _difficult;
            new_battle_set.turn = 0;

            auto n = name{new_battle_set.user};
            auto e = name{new_battle_set.enemy_user};

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
            new_set_stage_state(stage_id, now(), new_battle_set.enemy_state_list, enemy_state);
            set_synergy(new_battle_set.my_state_list, new_battle_set.my_synergy_list);
            set_hp_synergy(new_battle_set.my_state_list, new_battle_set.my_synergy_list);

            set_synergy(new_battle_set.enemy_state_list, new_battle_set.enemy_synergy_list);
            set_hp_synergy(new_battle_set.enemy_state_list, new_battle_set.enemy_synergy_list);
        });
    }
    else
    {
        user_battle_table.modify(user_battle_iter, _self, [&](auto &new_battle_set) {
            new_battle_set.enemy_user = _user;
            new_battle_set.stage_type = _stage_type;
            new_battle_set.type = _type;
            new_battle_set.floor = _floor;
            new_battle_set.difficult = _difficult;
            new_battle_set.turn = 0;
            new_battle_set.my_state_list.clear();
            new_battle_set.enemy_state_list.clear();
            new_battle_set.my_synergy_list.clear();
            new_battle_set.enemy_synergy_list.clear();

            auto n = name{new_battle_set.user};
            auto e = name{new_battle_set.enemy_user};

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
            new_set_stage_state(stage_id, now(), new_battle_set.enemy_state_list, enemy_state);

            set_synergy(new_battle_set.my_state_list, new_battle_set.my_synergy_list);
            set_hp_synergy(new_battle_set.my_state_list, new_battle_set.my_synergy_list);

            set_synergy(new_battle_set.enemy_state_list, new_battle_set.enemy_synergy_list);
            set_hp_synergy(new_battle_set.enemy_state_list, new_battle_set.enemy_synergy_list);
        });
    }
    init_action_reward_table(_user);
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
    //auto active_iter = get_active_db(_skill);
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
    uint64_t rate = safeseed::get_random_value(seed, 21, 0, 0);
    rate += 90;
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
        // monster_db monster_db_table(_self, _self.value);
        // auto defender_monster_iter = monster_db_table.find(_enemy_status_list[_enemy_key].id);
        // eosio_assert(defender_monster_iter != monster_db_table.end(), "Check Skill Type : Empty Monster ID / Wrong Monster ID");

        auto defender_monster_iter = get_monster_db(_enemy_status_list[_enemy_key].id);


        active_db active_db_table(_self, _self.value);
        auto active_iter = active_db_table.find(_my_status_list[_my_key].active_skill_list[0]);
        eosio_assert(active_iter != active_db_table.end(),"Check Skill Type : Empty Active ID/ Wrong Actvie ID");
        //auto active_iter = get_active_db(_my_status_list[_my_key].active_skill_list[0]);

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
    if ((_my_status_list[_my_key].type == character_type::t_monster) && //둘다 몬스터 일때
        (_enemy_status_list[_enemy_key].type == character_type::t_monster))
    {
              // monster_db monster_db_table(_self, _self.value);
        // auto attack_monster_iter = monster_db_table.find(_my_status_list[_my_key].id);
        // eosio_assert(attack_monster_iter != monster_db_table.end(), "Check Monster Type Attack : Empty Monster ID / Wrong Monster ID");

        auto attack_monster_iter = get_monster_db(_my_status_list[_my_key].id);
        auto defender_monster_iter = get_monster_db(_enemy_status_list[_enemy_key].id);

        // auto defender_monster_iter = monster_db_table.find(_enemy_status_list[_enemy_key].id);
        // eosio_assert(defender_monster_iter != monster_db_table.end(), "Check Monster Type Defender : Empty Monster ID / Wrong Monster ID");

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
    //처음 기본 스테이터스에대한 장비 장착한 스테이터스 반영
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

        if (_my_state_list[i].type == character_type::t_servant)    //아군 서번트의 경우
        {
            //장비 장착에 대한 기본 능력치 예외 처리 [ 힘, 민, 지]
            user_servants user_servant_table(_self, _from.value);
            auto user_servant_iter = user_servant_table.find(_my_state_list[i].index);
            eosio_assert(user_servant_iter != user_servant_table.end(), "Set User State : Empty Servant Index / Wrong Servant Index");
            //auto user_servant_iter = get_user_servant(_from, _my_state_list[i].index);

            // servant_db servant_db_table(_self, _self.value);
            // auto servant_iter = servant_db_table.find(_my_state_list[i].id);
            // eosio_assert(servant_iter != servant_db_table.end(), "Set Battle Status My : Empty Servant ID / Wrong Servant ID");

            auto servant_iter = get_servant_db(_my_state_list[i].id);

            serstat_db job_stat_db_table(_self, _self.value);
            uint32_t servant_job_base = (servant_iter->job * 1000) + (servant_iter->grade * 100) + 1;
            auto job_iter = job_stat_db_table.find(servant_job_base);
            eosio_assert(job_iter != job_stat_db_table.end(), "Set Battle Status My : Empty Servant Job / Wrong Servant Job");
            
            user_equip_items user_equipment_table(_self, _from.value);
            equipment_db equipment_db_table(_self, _self.value);
            for (uint32_t equip = 0; equip < user_servant_iter->servant.equip_slot.size(); ++equip) //서번트 장비 슬롯을 돌면서 힘민지에 대한 증가 효과 적용
            {
                if (user_servant_iter->servant.equip_slot[equip] == 0)
                {
                    continue;
                }
                auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[equip]);
                eosio_assert(user_equipment_iter != user_equipment_table.end(), "Set User State : Empty Equipment Index / Wrong Equipment Index");
                //auto user_equipment_iter = get_user_equipment(_from, user_servant_iter->servant.equip_slot[equip]);

                auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
                eosio_assert(db_equipment_iter != equipment_db_table.end(), "Set User State : Empty Equipment ID / Wrong Equipment ID");
                //auto db_equipment_iter = get_equipment_db(user_equipment_iter->equipment.id);

                uint32_t value = user_equipment_iter->equipment.value;
                set_upgrade_equip_status(user_equipment_iter->equipment.grade, value, user_equipment_iter->equipment.upgrade);

                //장비 강화 수치에 따라 능력치 증가
                for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
                {
                    if (db_equipment_iter->option_list[a] == option_list::status_str) //힘 증가
                    {
                        battle_status.status.basic_str += value;
                    }
                    else if (db_equipment_iter->option_list[a] == option_list::status_dex) //민 증가
                    {
                        battle_status.status.basic_dex += value;
                    }
                    else if (db_equipment_iter->option_list[a] == option_list::status_int) //지 증가
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

            for (uint32_t equip = 0; equip < user_servant_iter->servant.equip_slot.size(); ++equip) //서번트 장비 슬롯을 돌면서 힘민지에 대한 증가 효과 적용
            {
                if (user_servant_iter->servant.equip_slot[equip] == 0)
                {
                    continue;
                }
                auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[equip]);
                eosio_assert(user_equipment_iter != user_equipment_table.end(), "Set User State : Empty Equipment Index / Wrong Equipment Index");
                //auto user_equipment_iter = get_user_equipment(_from, user_servant_iter->servant.equip_slot[equip]);

                auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
                eosio_assert(db_equipment_iter != equipment_db_table.end(), "Set User State : Empty Equipment ID / Wrong Equipment ID");

                //auto db_equipment_iter = get_equipment_db(user_equipment_iter->equipment.id);

                uint32_t value = user_equipment_iter->equipment.value;
                set_upgrade_equip_status(user_equipment_iter->equipment.grade, value, user_equipment_iter->equipment.upgrade);

                for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
                {
                    if (db_equipment_iter->option_list[a] == option_list::physical_attack) //물공 증가
                    {
                        battle_status.p_atk += value;
                    }
                    else if (db_equipment_iter->option_list[a] == option_list::magic_attack) //마공 증가
                    {
                        battle_status.m_atk += value;
                    }
                    else if (db_equipment_iter->option_list[a] == option_list::physical_defense) //물방 증가
                    {
                        battle_status.p_dfs += value;
                    }
                    else if (db_equipment_iter->option_list[a] == option_list::magic_defense) //마방 증가
                    {
                        battle_status.m_dfs += value;
                    }
                }
            }
        }
        else        //아군 몬스터의 경우
        {
            // monster_db monster_db_table(_self, _self.value);
            // auto monster_iter = monster_db_table.find(_my_state_list[i].id);
            // eosio_assert(monster_iter != monster_db_table.end(), "Set Battle Status My : Empty Monster ID / Wrong Monster ID");

            auto monster_iter = get_monster_db(_my_state_list[i].id);

            // tribe_db class_stat_db_table(_self, _self.value);
            // auto class_stat_iter = class_stat_db_table.find(monster_iter->tribe);
            // eosio_assert(class_stat_iter != class_stat_db_table.end(), "Set Battle Status My : Empty Monster Tribe / Wrong Monster Tribe");
            auto class_stat_iter = get_tribe_db(monster_iter->tribe);

            set_upgrade_monster_status(_my_state_list[i].grade, battle_status.status, battle_status.upgrade);       //몬스터 강화 스테이터스 반영

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

        if (_enemy_state_list[i].type == character_type::t_servant)     //적군 서번트의 경우
        {
            user_servants user_servant_table(_self, _to.value);
            auto user_servant_iter = user_servant_table.find(_enemy_state_list[i].index);
            eosio_assert(user_servant_iter != user_servant_table.end(), "Set User State : Empty Servant Index / Wrong Servant Index");
            //auto user_servant_iter = get_user_servant(_to, _enemy_state_list[i].index);

            // servant_db servant_db_table(_self, _self.value);
            // auto servant_iter = servant_db_table.find(_enemy_state_list[i].id);
            // eosio_assert(servant_iter != servant_db_table.end(), "Set Battle Status My : Empty Servant ID / Wrong Servant ID");

            auto servant_iter = get_servant_db(_enemy_state_list[i].id);

            serstat_db job_stat_db_table(_self, _self.value);
            uint32_t servant_job_base = (servant_iter->job * 1000) + (servant_iter->grade * 100) + 1;
            auto job_iter = job_stat_db_table.find(servant_job_base);
            eosio_assert(job_iter != job_stat_db_table.end(), "Set Battle Status My : Empty Servant Job / Wrong Servant Job");

            //장비 장착에 대한 기본 능력치 예외 처리 [ 힘, 민, 지]
            if (_to != _from)   //스테이지를 진행하는 거지만 서번트가 있을수 있으므로 체크
            {
                user_equip_items user_equipment_table(_self, _to.value);
                equipment_db equipment_db_table(_self, _self.value);
                for (uint32_t equip = 0; equip < user_servant_iter->servant.equip_slot.size(); ++equip) //서번트 장비 슬롯을 돌면서 힘민지에 대한 증가 효과 적용
                {
                    if (user_servant_iter->servant.equip_slot[equip] == 0)
                    {
                        continue;
                    }
                    auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[equip]);
                    eosio_assert(user_equipment_iter != user_equipment_table.end(), "Set User State : Empty Equipment Index / Wrong Equipment Index");
                    //auto user_equipment_iter = get_user_equipment(_to, user_servant_iter->servant.equip_slot[equip]);

                    auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
                    eosio_assert(db_equipment_iter != equipment_db_table.end(), "Set User State : Empty Equipment ID / Wrong Equipment ID");
                    //auto db_equipment_iter = get_equipment_db(user_equipment_iter->equipment.id);

                    uint32_t value = user_equipment_iter->equipment.value;
                    set_upgrade_equip_status(user_equipment_iter->equipment.grade, value, user_equipment_iter->equipment.upgrade);

                    for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
                    {
                        if (db_equipment_iter->option_list[a] == option_list::status_str) //힘 증가
                        {
                            battle_status.status.basic_str += value;
                        }
                        else if (db_equipment_iter->option_list[a] == option_list::status_dex) //민 증가
                        {
                            battle_status.status.basic_dex += value;
                        }
                        else if (db_equipment_iter->option_list[a] == option_list::status_int) //지 증가
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

            if (_to != _from)       //스테이지를 진행하는 거지만 서번트가 있을수 있으므로 체크
            {
                user_equip_items user_equipment_table(_self, _to.value);
                equipment_db equipment_db_table(_self, _self.value);
                for (uint32_t equip = 0; equip < user_servant_iter->servant.equip_slot.size(); ++equip) //서번트 장비 슬롯을 돌면서 힘민지에 대한 증가 효과 적용
                {
                    if (user_servant_iter->servant.equip_slot[equip] == 0)
                    {
                        continue;
                    }
                    auto user_equipment_iter = user_equipment_table.find(user_servant_iter->servant.equip_slot[equip]);
                    eosio_assert(user_equipment_iter != user_equipment_table.end(), "Set User State : Empty Equipment Index / Wrong Equipment Index");
                    //auto user_equipment_iter = get_user_equipment(_to, user_servant_iter->servant.equip_slot[equip]);

                    auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
                    eosio_assert(db_equipment_iter != equipment_db_table.end(), "Set User State : Empty Equipment ID / Wrong Equipment ID");

                    //auto db_equipment_iter = get_equipment_db(user_equipment_iter->equipment.id);

                    uint32_t value = user_equipment_iter->equipment.value;
                    set_upgrade_equip_status(user_equipment_iter->equipment.grade, value, user_equipment_iter->equipment.upgrade);

                    for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
                    {
                        if (db_equipment_iter->option_list[a] == option_list::physical_attack) //물공 증가
                        {
                            battle_status.p_atk += value;
                        }
                        else if (db_equipment_iter->option_list[a] == option_list::magic_attack) //마공 증가
                        {
                            battle_status.m_atk += value;
                        }
                        else if (db_equipment_iter->option_list[a] == option_list::physical_defense) //물방 증가
                        {
                            battle_status.p_dfs += value;
                        }
                        else if (db_equipment_iter->option_list[a] == option_list::magic_defense) //마방 증가
                        {
                            battle_status.m_dfs += value;
                        }
                    }
                }
            }
        }
        else            //적군 몬스터의 경우
        {
            // monster_db monster_db_table(_self, _self.value);
            // auto monster_iter = monster_db_table.find(_enemy_state_list[i].id);
            // eosio_assert(monster_iter != monster_db_table.end(), "Set Battle Status My : Empty Monster ID / Wrong Monster ID");
            auto monster_iter = get_monster_db(_enemy_state_list[i].id);

            // tribe_db class_stat_db_table(_self, _self.value);
            // auto class_stat_iter = class_stat_db_table.find(monster_iter->tribe);
            // eosio_assert(class_stat_iter != class_stat_db_table.end(), "Set Battle Status My : Empty Monster Tribe / Wrong Monster Tribe");
            auto class_stat_iter = get_tribe_db(monster_iter->tribe);

            set_upgrade_monster_status(_enemy_state_list[i].grade, battle_status.status, battle_status.upgrade);       //몬스터 강화 스테이터스 반영

            set_passive_basic_status_self(battle_status);       //몬스터 패시브 스킬 반영

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
        // passive_db passive_db_table(_self, _self.value);
        // auto passive_db_iter = passive_db_table.find(_status.passive_skill_list[skill]);
        // eosio_assert(passive_db_iter != passive_db_table.end(), "Set Passive Effect My : Empty Passive ID / Wrong Passive ID");
        auto passive_db_iter = get_passive_db(_status.passive_skill_list[skill]);

        switch (passive_db_iter->effect_id)
        {
        case passive_effect_id::b_str:
        {
            if (passive_db_iter->effect_type_id == passive_effect_type_id::up)
            {
                _status.status.basic_str = safemath::check_over_plus_flow(_status.status.basic_str, passive_db_iter->effect_value_a);
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
                _status.status.basic_dex = safemath::check_over_plus_flow(_status.status.basic_dex, passive_db_iter->effect_value_a);
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
                _status.status.basic_int = safemath::check_over_plus_flow(_status.status.basic_int, passive_db_iter->effect_value_a);
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
            // passive_db passive_db_table(_self, _self.value);
            // auto passive_db_iter = passive_db_table.find(_my_status_list[i].passive_skill_list[skill]);
            // eosio_assert(passive_db_iter != passive_db_table.end(),"Set Passive Effect My : Empty Passive ID / Wrong Passive ID");
            auto passive_db_iter = get_passive_db(_my_status_list[i].passive_skill_list[skill]);

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
        // passive_db passive_db_table(_self, _self.value);
        // auto passive_db_iter = passive_db_table.find(_my_synergy_list[i]);
        // eosio_assert(passive_db_iter != passive_db_table.end(), "Set Synergy Effect My : Empty Synergy ID / Wrong Synergy ID");
        auto passive_db_iter = get_passive_db(_my_synergy_list[i]);

        switch (passive_db_iter->effect_id)
        {
        case passive_effect_id::p_atk:
        {
            if (passive_db_iter->target_id == passvie_target_id::t_beast)   //아군 비스트 일 경우
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (_my_status_list[my].type == character_type::t_monster)  //전체 애들중에 몬스터만 검사
                    {
                        // monster_db monster_db_table(_self, _self.value);
                        // auto monster_iter = monster_db_table.find(_my_status_list[my].id);
                        // eosio_assert(monster_iter != monster_db_table.end(), "Battle Synergy Set : Empty Monster ID / Wrong Monster ID");

                        auto monster_iter = get_monster_db(_my_status_list[my].id);
                        if (passvie_target_id::t_beast == (monster_iter->tribe + 900))  // 타겟과 종족이 같으면
                        {
                            if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)  //퍼센트 연산이면
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
            if (passive_db_iter->target_id == passvie_target_id::t_demon)   //아군 악마 일 경우
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (_my_status_list[my].type == character_type::t_monster)  //전체 애들중에 몬스터만 검사
                    {
                        // monster_db monster_db_table(_self, _self.value);
                        // auto monster_iter = monster_db_table.find(_my_status_list[my].id);
                        // eosio_assert(monster_iter != monster_db_table.end(), "Battle Synergy Set : Empty Monster ID / Wrong Monster ID");
                        auto monster_iter = get_monster_db(_my_status_list[my].id);
                        if (passvie_target_id::t_demon == (monster_iter->tribe + 900))  // 타겟과 종족이 같으면
                        {
                            if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)  //퍼센트 연산이면
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
            if (passive_db_iter->target_id == passvie_target_id::t_human)   //아군 인간 일 경우
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (_my_status_list[my].type == character_type::t_monster)  //전체 애들중에 몬스터만 검사
                    {
                        // monster_db monster_db_table(_self, _self.value);
                        // auto monster_iter = monster_db_table.find(_my_status_list[my].id);
                        // eosio_assert(monster_iter != monster_db_table.end(), "Battle Synergy Set : Empty Monster ID / Wrong Monster ID");
                        auto monster_iter = get_monster_db(_my_status_list[my].id);
                        if (passvie_target_id::t_human == (monster_iter->tribe + 900))  // 타겟과 종족이 같으면
                        {
                            if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up)  //퍼센트 연산이면
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
            if (passive_db_iter->target_id == passvie_target_id::t_partyall)   //아군 전체 일 경우
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (passive_db_iter->effect_type_id == passive_effect_type_id::per_up) //퍼센트 연산이면
                    {
                        _my_status_list[my].m_dfs += (_my_status_list[my].m_dfs * passive_db_iter->effect_value_a) / 100;
                    }
                }
            }
            break;
        }
        case passive_effect_id::avoid:
        {
            if (passive_db_iter->target_id == passvie_target_id::t_insect)   //아군 곤충 일 경우
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (_my_status_list[my].type == character_type::t_monster)  //전체 애들중에 몬스터만 검사
                    {
                        // monster_db monster_db_table(_self, _self.value);
                        // auto monster_iter = monster_db_table.find(_my_status_list[my].id);
                        // eosio_assert(monster_iter != monster_db_table.end(), "Battle Synergy Set : Empty Monster ID / Wrong Monster ID");
                        auto monster_iter = get_monster_db(_my_status_list[my].id);
                        if (passvie_target_id::t_insect == (monster_iter->tribe + 900))  // 타겟과 종족이 같으면
                        {
                            if (passive_db_iter->effect_type_id == passive_effect_type_id::up) //퍼센트 연산이면
                            {
                                _my_status_list[my].avoid += passive_db_iter->effect_value_a;
                            }
                        }
                    }
                }
            }
            break;
        }
        case passive_effect_id::cri_per:
        {
            if (passive_db_iter->target_id == passvie_target_id::t_mysterious)   //아군 미스테리어스 일 경우
            {
                for (uint32_t my = 0; my < _my_status_list.size(); ++my)
                {
                    if (_my_status_list[my].type == character_type::t_monster)  //전체 애들중에 몬스터만 검사
                    {
                        // monster_db monster_db_table(_self, _self.value);
                        // auto monster_iter = monster_db_table.find(_my_status_list[my].id);
                        // eosio_assert(monster_iter != monster_db_table.end(), "Battle Synergy Set : Empty Monster ID / Wrong Monster ID");
                        auto monster_iter = get_monster_db(_my_status_list[my].id);
                        if (passvie_target_id::t_mysterious == (monster_iter->tribe + 900))  // 타겟과 종족이 같으면
                        {
                            if (passive_db_iter->effect_type_id == passive_effect_type_id::up) //퍼센트 연산이면
                            {
                                _my_status_list[my].cri_per += passive_db_iter->effect_value_a;
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



void battletest::set_skill_damage(battle_status_info &_my_status, battle_status_info &_enemy_status, uint32_t &_attack, uint32_t &_target_defense, uint32_t &_target_avoid)
{
    active_db active_db_table(_self, _self.value);
    auto active_iter = active_db_table.find(_my_status.active_skill_list[0]);
    eosio_assert(active_iter != active_db_table.end(), "Skill Atk Dmg Type : Empty Active ID / Wrong Activd ID");
    //auto active_iter = get_active_db(_my_status.active_skill_list[0]);

    //액티브 스킬의 공격타입과 설정
    if (active_iter->attack_type == atk_type::physical_atk)
    {
        _attack = (_my_status.p_atk * active_iter->atk_per_1) / 100;
    }
    else if (active_iter->attack_type == atk_type::magic_physical_atk)
    {
        _attack = (_my_status.p_atk * active_iter->atk_per_1) / 100;
        _attack += (_my_status.m_atk * active_iter->atk_per_2) / 100;
    }
    else if (active_iter->attack_type == atk_type::magic_atk)
    {
        if (active_iter->skill_type == active_skill_type::type_heal)
        {
            _attack = (_my_status.m_atk * active_iter->heal_per) / 100;
            _target_defense = 0;
        }
        else
        {
            _attack = (_my_status.m_atk * active_iter->atk_per_1) / 100;
        }
    }

    //액티스 스킬의 방어타입 설정
    if (active_iter->dmg_type == dmg_type::physical_dfs)
    {
        _target_defense = _enemy_status.p_dfs;
        _target_avoid = _enemy_status.avoid;
    }
    else if (active_iter->dmg_type == dmg_type::true_dmg)
    {
        _target_defense = 0;
        _target_avoid = _enemy_status.avoid;
    }
    else if (active_iter->dmg_type == dmg_type::magic_dfs)
    {
        _target_defense = _enemy_status.m_dfs;
        _target_avoid = _enemy_status.avoid;
    }


    if (active_iter->option_id == active_option::option_perfectcri)
    {
        _my_status.cri_per = 100;
    }
    else if (active_iter->option_id == active_option::option_ignoreevade)
    {
        _target_avoid = 0;
    }
    else if (active_iter->option_id == active_option::option_nocritical)
    {
        _my_status.cri_per = 0;
    }
    else if (active_iter->option_id == active_option::option_nocri_noavoid)
    {
        _my_status.cri_per = 0;
        _target_avoid = 0;
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

        target_defense = _enemy_status_list[_target_key].p_dfs;
        target_avoid = _enemy_status_list[_target_key].avoid;
        break;
    }
    case action_type::skill:
    {
        set_skill_damage(_my_status_list[_my_key],
                            _enemy_status_list[_target_key],
                            cur_attack,
                            target_defense,
                            target_avoid);
        cur_cirtical_dmg = (cur_attack * _my_status_list[_my_key].cri_dmg_per) / 100;
        break;
    }
    default:
    {
        eosio_assert(1 == 0, "Target Action : Wrong Action Type");
        break;
    }
    }

    action_info new_action;
    uint64_t seed = _seed >> 5;
    if (true == check_avoid(target_avoid, seed))
    {
        new_action.target_position = _enemy_status_list[_target_key].position;
        new_action.avoid = 1;
        new_action.critical = 0;
        new_action.damage = 0;
        return new_action;
    }
    else
    {
        if (false == check_critical(_my_status_list[_my_key].cri_per, seed))
        {
            cur_damage = get_damage(cur_attack, target_defense);
            // if(_active_id == action_type::attack)
            // {
            //     cur_damage += get_damage(_my_status_list[_my_key].m_atk, _enemy_status_list[_target_key].m_dfs);
            // }
            new_action.target_position = _enemy_status_list[_target_key].position;
            new_action.avoid = 0;
            new_action.critical = 0;
            new_action.damage = cur_damage;
        }
        else
        {
            cur_damage = get_damage(cur_cirtical_dmg, target_defense);
            // if (_active_id == action_type::attack)
            // {
            //     cur_damage += get_damage( (_my_status_list[_my_key].m_atk * _my_status_list[_my_key].cri_dmg_per) / 100,
            //                                  _enemy_status_list[_target_key].m_dfs);
            // }
            new_action.target_position = _enemy_status_list[_target_key].position;
            new_action.avoid = 0;
            new_action.critical = 1;
            new_action.damage = cur_damage;
        }
        return new_action;
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
        if (enemy_key == -1) //상대 파티가 모두 죽은 상태
        {
            return false;
        }
        action_info new_action;
        new_action = get_target_action(_action, _seed, _my_key, enemy_key, _my_status_list, _enemy_status_list);

        set_random_damage(new_action, _seed);       //90~110% 사이의 랜덤 데미지
        result_type_damage(_user, new_action, _my_status_list, _enemy_status_list, _my_key, enemy_key); //속성 추뎀 체크
        result_buff(new_action, _enemy_status_list[enemy_key]);     //버프에 따른 데미지 처리

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
        //auto active_iter = get_active_db(_my_status_list[_my_key].active_skill_list[0]);

        if (active_iter->skill_type == active_skill_type::type_heal) //힐스킬
        {
            int enemy_key = get_heal_target(_my_status_list);
            if (enemy_key == -1) //상대 파티가 모두 죽은 상태
            {
               enemy_key = _my_key;
            }
            action_info new_action;
            new_action = get_target_action(_action, _seed, _my_key, enemy_key, _my_status_list, _my_status_list);
            set_random_damage(new_action, _seed);       //90~110% 사이의 랜덤 데미지
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
                if (enemy_key == -1) //상대 파티가 모두 죽은 상태
                {
                    return false;
                }
                new_seed = new_seed >> 1;

                action_info new_action;
                new_action = get_target_action(_action, _seed, _my_key, enemy_key, _my_status_list, _enemy_status_list);
                _enemy_status_list[enemy_key].buff_list.push_back(new_buff);
                _enemy_status_list[enemy_key].p_dfs -=  (_enemy_status_list[enemy_key].p_dfs * active_iter->atk_per_2) / 100;

                set_random_damage(new_action, _seed);       //90~110% 사이의 랜덤 데미지
                result_buff(new_action, _enemy_status_list[enemy_key]);                   //버프 스킬 체크

                // uint64_t rate = safeseed::get_random_value(new_seed, 111, 90, 0);
                // new_action.damage = (new_action.damage * rate) / 100;

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
                if (enemy_key == -1) //상대 파티가 모두 죽은 상태
                {
                    return false;
                }
                new_seed = new_seed >> (i + 2);

                action_info new_action;
                new_action = get_target_action(_action, _seed, _my_key, enemy_key, _my_status_list, _enemy_status_list);

                set_random_damage(new_action, _seed);       //90~110% 사이의 랜덤 데미지
                result_type_skill(_user, new_action, _my_status_list, _enemy_status_list, _my_key, enemy_key);  //스킬의 속성 추뎀 체크
                result_buff(new_action, _enemy_status_list[enemy_key]);                   //버프 스킬 체크

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

    new_battle_state_list battle_state_list_table(_self, _self.value);
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
    //공격순서 정렬하는 부분
    std::vector<battle_status_info> second_attack_order_list;
    std::vector<battle_status_info> attack_order_list;
    std::vector<battle_status_info> skill_order_list;
    //배틀에 대한 상세 능력치가 들어있는 부분
    std::vector<battle_status_info> my_battle_status_list;
    std::vector<battle_status_info> enemy_battle_status_list;

    //배틀의 상태를 바꿔주는 부분
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
        //배틀에 액션테이블에 데이터를 추가해주는 부분
        battle_action_table.modify(user_battle_action_iter, _self, [&](auto &update_action) {
            update_action.turn += 1;
            turn += to_string(update_action.turn);
            update_action.character_action_list.clear();
            //먼저 스킬 발동 여부 확인
            for (uint32_t i = 0; i < skill_order_list.size(); ++i)  
            {
                if (skill_order_list[i].position < max_party_count) //산 애들인데 내 파티이면
                {
                    int my_key = get_status_position_key(my_battle_status_list, skill_order_list[i].position); //자신의 스테이터스를 찾는다
                    if(my_key == -1)
                    {
                        continue;
                    }
                    else if(my_battle_status_list[my_key].state == battle_member_state::dead)
                    {
                        continue;
                    }
                    else if (my_battle_status_list[my_key].active_skill_list.size() != 0) //액티브 스킬이 있으면
                    {
                        uint64_t action_rate = safeseed::get_random_value(skill_order_list[i].second_speed, 100, 0, 0); //액티브 스킬의 확률을 구한다

                        if (true == check_activate_skill(my_battle_status_list[my_key].active_skill_list[0], action_rate)) //액티브 스킬이 발동하면
                        {
                            active_db active_db_table(_self, _self.value);
                            auto active_iter = active_db_table.find(my_battle_status_list[my_key].active_skill_list[0]);
                            eosio_assert(active_iter != active_db_table.end(), "Actvie Turn My : Empty Active ID / Wrong Active ID");
                            //auto active_iter = get_active_db(my_battle_status_list[my_key].active_skill_list[0]);
                            if (active_iter->skill_type == active_skill_type::type_attack)   //공격하는 스킬
                            {
                                if (active_iter->option_id == active_option::option_fastattack) //선공 옵션이 붙은 경우
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
                            else if (active_iter->skill_type == active_skill_type::type_buff) //버프 스킬
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
                            else if (active_iter->skill_type == active_skill_type::type_heal) //힐 스킬
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
                    int my_key = get_status_position_key(enemy_battle_status_list, skill_order_list[i].position); //자신의 스테이터스를 찾는다
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
                        uint64_t action_rate = safeseed::get_random_value(skill_order_list[i].second_speed, 100, 0, 0); //액티브 스킬의 확률을 구한다
                        if (true == check_activate_skill(enemy_battle_status_list[my_key].active_skill_list[0], action_rate)) //액티브 스킬이 발동하면
                        {
                            active_db active_db_table(_self, _self.value);
                            auto active_iter = active_db_table.find(enemy_battle_status_list[my_key].active_skill_list[0]);
                            eosio_assert(active_iter != active_db_table.end(), "Actvie Turn My : Empty Active ID / Wrong Active ID");
                            //auto active_iter = get_active_db(enemy_battle_status_list[my_key].active_skill_list[0]);
                            if (active_iter->skill_type == active_skill_type::type_attack)   //공격하는 스킬
                            {
                                if (active_iter->option_id == active_option::option_fastattack) //선공 옵션이 붙은 경우
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
                            else if (active_iter->skill_type == active_skill_type::type_buff) //버프 스킬
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
                            else if (active_iter->skill_type == active_skill_type::type_heal) //힐 스킬
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
                        else //스킬이 발동하지 않으면
                        {
                            skill_order_list[i].action = action_type::attack;
                            second_attack_order_list.push_back(skill_order_list[i]);
                        }
                    }
                    else    // 스킬이 없는 경우
                    {
                        skill_order_list[i].action = action_type::attack;
                        second_attack_order_list.push_back(skill_order_list[i]);
                    }
                }
            }
            //패스트 어택 애들 처리
            for (uint32_t i = 0; i < attack_order_list.size(); ++i)
            {
                if (attack_order_list[i].position < max_party_count) //자기 파티에 대한 처리
                {
                    int my_key = get_status_position_key(my_battle_status_list, attack_order_list[i].position); //자신의 스테이터스를 찾는다
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
                else // 상대 파티의 경우
                {
                    int my_key = get_status_position_key(enemy_battle_status_list, attack_order_list[i].position); //자신의 스테이터스를 찾는다
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
            //그냥 일반 공격하는 애들에 대한 처리 ===============================================================
            for (uint32_t i = 0; i < second_attack_order_list.size(); ++i)
            {
                if (second_attack_order_list[i].position < max_party_count) //자기 파티에 대한 처리
                {
                    int my_key = get_status_position_key(my_battle_status_list, second_attack_order_list[i].position); //자신의 스테이터스를 찾는다
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
                else // 상대 파티의 경우
                {
                    int my_key = get_status_position_key(enemy_battle_status_list, second_attack_order_list[i].position); //자신의 스테이터스를 찾는다
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
            //게임의 종료 여부 체크
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
            //계산했던 스테이트 결과를 테이블에 넣어준다
            set_result_state(my_battle_status_list, enemy_battle_status_list, character_state_data.my_state_list, character_state_data.enemy_state_list);
        });
    });

    if (user_auth_iter->state == user_state::stage)
    {
        if (enemy_dead_count == enemy_battle_status_list.size())
        {
            std::vector<uint32_t> monster_list;
            for(uint32_t a = 0 ; a < user_battle_state_iter->enemy_state_list.size(); ++a)
            {
                monster_list.push_back(user_battle_state_iter->enemy_state_list[a].id);
            }
            uint32_t stage_number = get_stage_id(user_battle_state_iter->stage_type, user_battle_state_iter->floor, user_battle_state_iter->type, user_battle_state_iter->difficult);
            new_win_reward(_user, stage_number, battle_seed, monster_list);
        }
        else if (user_dead_count == my_battle_status_list.size())
        {
            uint32_t stage_number = get_stage_id(user_battle_state_iter->stage_type, user_battle_state_iter->floor, user_battle_state_iter->type, user_battle_state_iter->difficult);
            fail_reward(_user, stage_number);
        }
    }
    // else if (user_auth_iter->state == user_state::tower) //tower
    // {
    //     if (enemy_dead_count == enemy_battle_status_list.size())
    //     {
    //         user_auths user_auth_table(_self, _self.value);
    //         auto user_auth_iter = user_auth_table.find(_user.value);
    //         eosio_assert(user_auth_iter != user_auth_table.end(), "Tower Result Win : Empty Auth Table / Not Yet Signup");
    //         eosio_assert(user_auth_iter->state == user_state::tower, "Tower Result Win : User State Not Stage / Not Yet Stage Start");
    //         user_auth_table.modify(user_auth_iter, _self, [&](auto &end_pvp) {
    //             end_pvp.state = user_state::lobby;
    //         });

    //         floor_index floortable(_self, _self.value);
    //         const auto &f_iter = floortable.get(user_battle_state_iter->enemy_user.value, "Floor info does not exist");
    //         towerwin(user_battle_state_iter->user, user_battle_state_iter->enemy_user.value, 1, f_iter.bnum);
    //     }
    //     else if (user_dead_count == my_battle_status_list.size())
    //     {
    //         user_auths user_auth_table(_self, _self.value);
    //         auto user_auth_iter = user_auth_table.find(_user.value);
    //         eosio_assert(user_auth_iter != user_auth_table.end(), "Tower Result Lose : Empty Auth Table / Not Yet Signup");
    //         eosio_assert(user_auth_iter->state == user_state::tower, "Tower Result Lose : User State Not Stage / Not Yet Stage Start");
    //         user_auth_table.modify(user_auth_iter, _self, [&](auto &end_pvp) {
    //             end_pvp.state = user_state::lobby;
    //         });
    //     }
    // }

    else
    {
        if (enemy_dead_count == enemy_battle_status_list.size())
        {
            pvp_log_index pvp_log_table(_self, _user.value);
            pvp_log_table.emplace(_self, [&](auto &data) {
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
            pvp_log_table.emplace(_self, [&](auto &data) {
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
}


uint32_t battletest::check_char_level_up(uint32_t _cur_level, uint64_t _get_exp, uint32_t _limit_break)
{
    uint32_t level_up_count = 0;
    lv_exp lv_exp_table(_self, _self.value);
    auto lv_exp_iter = lv_exp_table.find(_cur_level);
    if (lv_exp_iter->lv == (MAX_LEVEL + _limit_break))
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
    if (lv_exp_iter->lv == MAX_LEVEL)
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

battletest::servant_data battletest::get_servant(eosio::name _user, uint32_t _job, uint32_t _min, uint32_t _max, uint32_t _gold_type, uint64_t _seed)
{
    servant_job_db servant_job_table(_self, _self.value);
    uint32_t random_job;
    uint32_t gacha_result_index = 0;
 
    //직업값 0이면 랜덤, 숫자면 고정 
    if(_job !=0)
    {
        random_job = _job;
    }
    else if(_job == 0 && _min ==0 && _max == 0)
    {
        random_job = safeseed::get_random_value(_seed, SERVANT_JOB_COUNT, DEFAULT_MIN_DB, 1);
    }
    else    //직업값이 0이 아니면, min과 max에서 나오는 범위 고정 
    {
           random_job = safeseed::get_random_value(_seed, _max +1, _min, 1);        //1,2,3 중에 하나 나오려면 _max = 3, _min = 1 
    }

    const auto &servant_job_db_iter = servant_job_table.get(random_job, "Get Servant : Empty Servant Job / Wrong Servant Job");

    uint32_t random_body = gacha_servant_body(_seed, 1);

    gender_db gender_table(_self, _self.value);
    uint32_t random_gender = safeseed::get_random_value(_seed, GEMDER_COUNT, DEFAULT_MIN_DB, 2);
    const auto &gender_db_iter = gender_table.get(random_gender, "Get Servant : Empty Servant Gender / Wrong Servant Gender");

    uint32_t random_head = gacha_servant_head(_seed, 3);
    uint32_t random_hair = gacha_servant_hair(_seed, 4);

    uint32_t servant_index = get_servant_index(random_job, random_body, random_gender, random_head, random_hair);

    auto servant_id_db_iter = get_servant_db(servant_index);

    serstat_db servant_base_table(_self, _self.value);
    uint32_t servant_job_base = (servant_id_db_iter->job * 1000) + (servant_id_db_iter->grade * 100) + 1;
    const auto &ser_iter = servant_base_table.get(servant_job_base, "Get Servant : Empty Servant Stat");

    std::string body_data;
    
    servant_data new_data;
    user_servants user_servant_table(_self, _user.value);
    
    servant_info new_servant = get_servant_random_state(servant_id_db_iter->id, _seed, random_job, ser_iter.base_str, ser_iter.base_dex, ser_iter.base_int);

    if(_gold_type == use_money_type::EOS_GACHA || _gold_type == use_money_type::UTG_GACHA || _gold_type == use_money_type::BATTLE)
    {
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
            update_user_servant_list.party_number = EMPTY_PARTY;
            update_user_servant_list.servant = new_servant;

            // result.index = update_user_servant_list.index;
            // result.type = result::servant;
            new_data.index = update_user_servant_list.index;
            new_data.party_number = update_user_servant_list.party_number;
            new_data.servant = new_servant;
            gacha_result_index = update_user_servant_list.index;
        });
    }
    else if(_gold_type == use_money_type::PACKAGE || _gold_type == use_money_type::EVENT)   //메일 
    {
            mail_reward_list mail_reward_list_table(_self, _user.value);
            user_mail user_mail_table(_self, _user.value);
            std::string body_data;
            uint32_t mail_reward_first_index = 0;
            uint32_t new_first_index = mail_reward_list_table.available_primary_key();
            mail_reward_list_table.emplace(_self, [&](auto &update_reward) {
                if (new_first_index == 0)
                {
                    update_reward.index = 1;
                }
                else
                {
                    update_reward.index = mail_reward_list_table.available_primary_key();
                }
                update_reward.type = 1;

                new_data.index = update_reward.index;
                new_data.party_number = EMPTY_PARTY;
                new_data.servant = new_servant;
                gacha_result_index = update_reward.index;

                // result.index = update_reward.index;
                // result.type = result::servant;

                body_data += to_string(new_servant.id) + ":",
                    body_data += to_string(new_servant.status.basic_str) + ":",
                    body_data += to_string(new_servant.status.basic_dex) + ":",
                    body_data += to_string(new_servant.status.basic_int) + ":",
                    body_data += to_string(new_servant.passive_skill[0]) + ":",
                    body_data += to_string(new_servant.active_skill[0]);
                update_reward.body = body_data;
            });

            user_mail_table.emplace(_self, [&](auto &move_mail) {
                uint32_t first_index = user_mail_table.available_primary_key();
                if (first_index == 0)
                {
                    move_mail.mail_index = 1;
                }
                else
                {
                    move_mail.mail_index = user_mail_table.available_primary_key();
                }
                move_mail.mail_type = 8;
                move_mail.type_index = new_data.index;
                move_mail.count = 1;
                move_mail.icon_id = servant_id_db_iter->id;
                move_mail.get_time = now();
            });         

    }
    write_log(_user, _gold_type, 1, gacha_result_index, 1); 
    return new_data;

}

battletest::monster_data battletest::get_monster(eosio::name _user, uint32_t _id, uint32_t _grade, uint32_t _max, uint32_t _gold_type, uint64_t _seed)
{                         
    uint32_t random_monster_id = 0;
    uint32_t gacha_db_index = _id;

    uint64_t random_rate = 0;
    uint64_t random_grade = 0;

    uint32_t gacha_result_index =0;

    if (_id == 0)
    {
        if (_gold_type == 1 || _gold_type == 3 || _gold_type == 4 || _gold_type == 5) //EOS 가차
        {
            main_gacha_db main_gacha_db_table(_self, _self.value);
            auto gacha_db_iter = main_gacha_db_table.find(GACHA_MONSTER_COUNT_ID); //1 ser 2 mon 3 item
            eosio_assert(gacha_db_iter != main_gacha_db_table.end(), "Gacha Monster : Empty Max Count");
            random_monster_id = safeseed::get_random_value(_seed, gacha_db_iter->db_index + 1, DEFAULT_MIN_DB, 1);
            random_monster_id += MONSTER_GACHA_ID_START;

            auto gacha_monster_db_iter = main_gacha_db_table.find(random_monster_id);
            eosio_assert(gacha_monster_db_iter != main_gacha_db_table.end(), "Gacha Monster : Empty Gacha ID");
            gacha_db_index = gacha_monster_db_iter->db_index;
        }
        else if (_gold_type == 2) //UTG 가차
        {
            gold_gacha_db gold_gacha_db_table(_self, _self.value);
            auto gacha_db_iter = gold_gacha_db_table.find(GACHA_MONSTER_COUNT_ID); //2 mon 3 equip 6 item
            eosio_assert(gacha_db_iter != gold_gacha_db_table.end(), "Gold Gacha Monster : Empty Max Count");
            random_monster_id = safeseed::get_random_value(_seed, gacha_db_iter->db_index + 1, DEFAULT_MIN_DB, 1);
            random_monster_id += MONSTER_GACHA_ID_START;

            auto gacha_monster_db_iter = gold_gacha_db_table.find(random_monster_id);
            eosio_assert(gacha_monster_db_iter != gold_gacha_db_table.end(), "Gold Gacha Monster : Empty Gacha ID");
            gacha_db_index = gacha_monster_db_iter->db_index;
        }
    }
    auto monster_id_db_iter = get_monster_db(gacha_db_index);
    auto tribe_iter = get_tribe_db(monster_id_db_iter->tribe);

   if(_max == 0 && _grade == 0)
   {
       random_rate = safeseed::get_random_value(_seed, GACHA_MAX_RATE, DEFAULT_MIN, 2);
       random_grade = get_random_grade(random_rate);
   } 
   else if(_max != 0 && _grade == 0)
   {
       random_rate = safeseed::get_random_value(_seed, GACHA_MAX_RATE, DEFAULT_MIN, 2);
       random_grade = get_random_grade(random_rate);
       if(random_grade < _max) //등급은 1,2 나올 수 있으니까 
       {
           random_grade = 5;
       }
   }
   else if( _max ==0 && _grade != 0)
   {
       random_grade = _grade;
   }

    monster_data new_data;
    monster_info new_monster = get_monster_random_state(monster_id_db_iter->id,
                                                            _seed,
                                                            random_grade,
                                                            monster_id_db_iter->tribe,
                                                            monster_id_db_iter->type,
                                                            tribe_iter->base_str,
                                                            tribe_iter->base_dex,
                                                            tribe_iter->base_int);

    user_monsters user_monster_table(_self, _user.value);
    if(_gold_type == use_money_type::EOS_GACHA || _gold_type == use_money_type::UTG_GACHA || _gold_type == use_money_type::BATTLE)
    {
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

            update_user_monster_list.party_number = EMPTY_PARTY;
            update_user_monster_list.monster = new_monster;

            new_data.index = update_user_monster_list.index;
            new_data.party_number = update_user_monster_list.party_number;
            new_data.monster = new_monster;
            gacha_result_index = update_user_monster_list.index;
        });
    }

    else if (_gold_type == use_money_type::PACKAGE || _gold_type == use_money_type::EVENT)
    {
        std::string body_data;
        uint32_t mail_reward_first_index = 0;
        mail_reward_list mail_reward_list_table(_self, _user.value);
        uint32_t new_first_index = mail_reward_list_table.available_primary_key();
        mail_reward_list_table.emplace(_self, [&](auto &update_reward) {
            if (new_first_index == 0)
            {
                update_reward.index = 1;
            }
            else
            {
                update_reward.index = mail_reward_list_table.available_primary_key();
            }
            update_reward.type = 2;  

            new_data.index = update_reward.index;
            new_data.party_number = EMPTY_PARTY;
            new_data.monster = new_monster;
            gacha_result_index = update_reward.index;
            
            body_data += to_string(new_monster.id) + ":",
                body_data += to_string(new_monster.grade) + ":",
                body_data += to_string(new_monster.status.basic_str) + ":",
                body_data += to_string(new_monster.status.basic_dex) + ":",
                body_data += to_string(new_monster.status.basic_int) + ":",
                body_data += to_string(new_monster.passive_skill[0]) ;

            update_reward.body = body_data;
        });

        user_mail user_mail_table(_self, _user.value);
        user_mail_table.emplace(_self, [&](auto &move_mail) {
            uint32_t first_index = user_mail_table.available_primary_key();
            if (first_index == 0)
            {
                move_mail.mail_index = 1;
            }
            else
            {
                move_mail.mail_index = user_mail_table.available_primary_key();
            }
            move_mail.mail_type = 9;
            move_mail.type_index = new_data.index;
            move_mail.count = 1;
            move_mail.icon_id = monster_id_db_iter->id;
            move_mail.get_time = now();
        });
    }    
    write_log(_user, _gold_type, 2, gacha_result_index, 1);
    return new_data;
}

battletest::equip_data battletest::get_equip(eosio::name _user, uint32_t _id, uint32_t _grade, uint32_t _max, uint32_t _gold_type, uint64_t _seed)
{
    uint32_t random_equipment_id = 0;
    uint32_t gacha_db_index = _id;
    uint64_t random_rate = 0;
    uint64_t random_grade = 0;

    uint32_t gacha_result_index = 0;

    equipment_db equip_item_table(_self, _self.value);

    if (_id == 0)
    {
        if (_gold_type == 2)
        {
            gold_gacha_db gold_gacha_db_table(_self, _self.value);
            auto gacha_db_iter = gold_gacha_db_table.find(GACHA_EQUIPMENT_COUNT_ID); //2 mon 3 equip 6 item
            eosio_assert(gacha_db_iter != gold_gacha_db_table.end(), "Gold Gacha Equipment : Empty Max Count / Not Set Count");
            random_equipment_id = safeseed::get_random_value(_seed, gacha_db_iter->db_index + 1, DEFAULT_MIN_DB, 1);
            random_equipment_id += EQUIP_GACHA_ID_START;
            auto gacha_id_db_iter = gold_gacha_db_table.find(random_equipment_id);
            eosio_assert(gacha_id_db_iter != gold_gacha_db_table.end(), "Gold Gacha Equipment : Empty Gacha ID / Not Set Gacha ID");
            gacha_db_index = gacha_id_db_iter->db_index;
        }

        else
        {
            main_gacha_db main_gacha_db_table(_self, _self.value);
            auto gacha_db_iter = main_gacha_db_table.find(GACHA_EQUIPMENT_COUNT_ID); //1 ser 2 mon 3 item
            eosio_assert(gacha_db_iter != main_gacha_db_table.end(), "Gacha Equipment : Empty Max Count / Not Set Count");
            random_equipment_id = safeseed::get_random_value(_seed, gacha_db_iter->db_index + 1, DEFAULT_MIN_DB, 1);
            random_equipment_id += EQUIP_GACHA_ID_START;
            auto gacha_equipment_db_iter = main_gacha_db_table.find(random_equipment_id);
            eosio_assert(gacha_equipment_db_iter != main_gacha_db_table.end(), "Gacha Equipment : Empty Gacha ID / Not Set Gacha ID");
            gacha_db_index = gacha_equipment_db_iter->db_index;
        }
    }
    const auto &equip_item_iter = equip_item_table.get(gacha_db_index, "Get Reward Equipment : Empty Equipment ID / Wrong Equipment ID");

    mail_reward_list mail_reward_list_table(_self, _user.value);
    user_mail user_mail_table(_self, _user.value);

    if (_max != 0 && _grade == 0)
    {
        random_rate = safeseed::get_random_value(_seed, GACHA_MAX_RATE, DEFAULT_MIN, 2);
        random_grade = get_random_grade(random_rate);
        if (random_grade <= _max)
        {
            random_grade = 5;
        }
    }
    else if (_max == 0 && _grade != 0)
    {
        random_grade = _grade;
    }
    else
    {
        random_rate = safeseed::get_random_value(_seed, GACHA_MAX_RATE, DEFAULT_MIN, 2);
        random_grade = get_random_grade(random_rate);
    }

    equip_data new_data;
    equipment_info new_item = get_equip_random_state(equip_item_iter.item_id,
                                                     _seed,
                                                     equip_item_iter.type,
                                                     equip_item_iter.tier,
                                                     equip_item_iter.job,
                                                     random_grade);

    user_equip_items user_item_table(_self, _user.value);
    if (_gold_type == use_money_type::EOS_GACHA || _gold_type == use_money_type::UTG_GACHA || _gold_type == use_money_type::BATTLE)
    {
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

            update_user_item_list.equipment = new_item;

            new_data.index = update_user_item_list.index;
            new_data.equipment = new_item;
            gacha_result_index = update_user_item_list.index;
        });
    }
    else if (_gold_type == use_money_type::PACKAGE || _gold_type == use_money_type::EVENT)
    {
        std::string body_data;
        uint32_t mail_reward_first_index = 0;
        uint32_t new_first_index = mail_reward_list_table.available_primary_key();
        mail_reward_list_table.emplace(_self, [&](auto &update_reward) {
            if (new_first_index == 0)
            {
                update_reward.index = 1;
            }
            else
            {
                update_reward.index = mail_reward_list_table.available_primary_key();
            }
            update_reward.type = 3;  

            new_data.index = update_reward.index;
            new_data.equipment = new_item;
            gacha_result_index = update_reward.index;

            body_data += to_string(new_item.id) + ":",
                body_data += to_string(new_item.grade) + ":",
                body_data += to_string(new_item.value);

            update_reward.body = body_data;
        });

        user_mail user_mail_table(_self, _user.value);
        user_mail_table.emplace(_self, [&](auto &move_mail) {
            uint32_t first_index = user_mail_table.available_primary_key();
            if (first_index == 0)
            {
                move_mail.mail_index = 1;
            }
            else
            {
                move_mail.mail_index = user_mail_table.available_primary_key();
            }
            move_mail.mail_type = 10;
            move_mail.type_index = new_data.index;
            move_mail.count = 1;
            move_mail.icon_id = equip_item_iter.item_id;
            move_mail.get_time = now();
        });
    }
    write_log(_user, _gold_type, 3, gacha_result_index, 1);
    return new_data;
}

battletest::item_data battletest::get_item(eosio::name _user, uint32_t _id, uint32_t _count, uint32_t _gold_type, uint64_t _seed)
{
    uint32_t add_inventory = 0;
    uint32_t gacha_result_index = 0;
    uint32_t random_item_id = 0;
    uint32_t gacha_db_index = _id;

    // gold_logs gold_logs_table(_self, _self.value);
    // auto gold_logs_iter = gold_logs_table.find(_user.value);
    // if (gold_logs_iter == gold_logs_table.end())
    // {
    //     gold_logs_table.emplace(_self, [&](auto &new_user) {
    //         new_user.user = _user;
    //         new_user.monster_num = 0;
    //         new_user.equipment_num = 0;
    //         new_user.item_num = 0;
    //         new_user.gold_gacha_num = 0;
    //         new_user.use_utg = 0;
    //     });
    // }

    if(_id == 0)//골드 가차 
    {
        uint64_t random_rate = safeseed::get_random_value(_seed, GACHA_MAX_RATE, DEFAULT_MIN, 1);
        uint64_t random_grade = get_random_grade(random_rate);
        random_item_id = ITEM_GACHA_ID_START;
        if (random_grade < 2)
        {
            random_item_id += 7;
        }
        else if (random_grade < 3)
        {
            uint64_t grade_seed = safeseed::get_seed_value(2, _seed);
            random_item_id += safeseed::get_random_value(grade_seed, 7, 5, 2);
        }
        else if (random_grade >= 3 && random_grade <= 5)
        {
            uint64_t grade_seed = safeseed::get_seed_value(3, _seed);
            random_item_id += safeseed::get_random_value(grade_seed, 5, 1, 2);
        }
        gold_gacha_db gold_gacha_db_table(_self, _self.value);
        auto gacha_id_db_iter = gold_gacha_db_table.find(random_item_id);
        eosio_assert(gacha_id_db_iter != gold_gacha_db_table.end(), "Get Item : Empty Gacha ID / Not Set Gacha ID");
        gacha_db_index = gacha_id_db_iter->db_index;
      
    }
    auto allitem_db_iter = get_allitem_db(gacha_db_index);
    
   
    mail_reward_list mail_reward_list_table(_self, _user.value);
    user_mail user_mail_table(_self, _user.value);
    item_data new_item;
    new_item.id = allitem_db_iter->id;
    new_item.type = allitem_db_iter->type;
    gacha_result_index = allitem_db_iter->id;
    user_items user_items_table(_self, _user.value);
    if (_gold_type == use_money_type::UTG_GACHA)
    {
         add_inventory = sum_item_check(_user, allitem_db_iter->id, 1);   
         write_log(_user, _gold_type, 4, gacha_result_index, add_inventory);  
    }
    else if (_gold_type == use_money_type::PACKAGE || _gold_type == use_money_type::EVENT)
    {
       std::string body_data;
        uint32_t mail_reward_first_index = 0;
        uint32_t new_first_index = mail_reward_list_table.available_primary_key();
        mail_reward_list_table.emplace(_self, [&](auto &update_reward) {
            if (new_first_index == 0)
            {
                update_reward.index = 1;
            }
            else
            {
                update_reward.index = mail_reward_list_table.available_primary_key();
            }
            update_reward.type = 4;  
            mail_reward_first_index = update_reward.index;
            body_data += to_string(gacha_db_index) + ":",
            body_data += to_string(_count);
            update_reward.body = body_data;
        });
        user_mail user_mail_table(_self, _user.value);
        user_mail_table.emplace(_self, [&](auto &move_mail) {
            uint32_t first_index = user_mail_table.available_primary_key();
            if (first_index == 0)
            {
                move_mail.mail_index = 1;
            }
            else
            {
                move_mail.mail_index = user_mail_table.available_primary_key();
            }
            
            move_mail.mail_type = 11;
            move_mail.type_index = mail_reward_first_index;
            move_mail.count = _count;
            move_mail.icon_id = gacha_db_index;
            move_mail.get_time = now();
        });
        write_log(_user, _gold_type, 4, gacha_result_index, add_inventory);
    }
    else if(_gold_type == use_money_type::BATTLE)
    {
        auto user_items_iter = user_items_table.find(allitem_db_iter->id);
        if (user_items_iter == user_items_table.end())
        {
            user_items_table.emplace(_self, [&](auto &change_consumable) {
                change_consumable.id = allitem_db_iter->id;
                change_consumable.type = allitem_db_iter->type;
                item_info get_item_info;
                get_item_info.index = 0;
                get_item_info.count = _count;
                change_consumable.item_list.push_back(get_item_info);
                add_inventory = 1;
                new_item.item_list = change_consumable.item_list;
            });
        }
        else
        {
            user_items_table.modify(user_items_iter, _self, [&](auto &change_consumable) {
                uint64_t size_count = change_consumable.item_list.size();
                for (uint64_t i = 0; i < size_count; i++)
                {
                    if (change_consumable.item_list[i].count < 99)
                    {
                        if (change_consumable.item_list[i].count + _count > 99)
                        {
                            uint64_t new_count = change_consumable.item_list[i].count + _count - 99;
                            change_consumable.item_list[i].count = 99;
                            item_info get_item_info;
                            get_item_info.index = size_count;
                            get_item_info.count = new_count;
                            change_consumable.item_list.push_back(get_item_info);
                            add_inventory = 1;
                        }
                        else
                        {
                            change_consumable.item_list[i].count += _count;
                        }
                    }
                    else if (change_consumable.item_list[i].count == 99 && i == (size_count - 1))
                    {
                        item_info get_item_info;
                        get_item_info.index = size_count;
                        get_item_info.count = _count;
                        change_consumable.item_list.push_back(get_item_info);
                        add_inventory = 1;
                    }
                }
                new_item.item_list = change_consumable.item_list;
            });
        }
        user_auths user_auth_table(_self, _self.value);
        auto user_auth_iter = user_auth_table.find(_user.value);
        user_auth_table.modify(user_auth_iter, _self, [&](auto &add_auth) {
            add_auth.current_item_inventory += add_inventory;
        });
    }
    return new_item;
}


void battletest::fail_reward(eosio::name _user, uint64_t _stage_number)
{
    uint32_t id = 0;
    if (_stage_number / 10000 == 0)
    {
        stageinfo_db stage_db_table(_self, _self.value);
        auto stage_db_iter = stage_db_table.find(_stage_number);
        eosio_assert(stage_db_iter != stage_db_table.end(), "Fail Reward : Empty Stage ID / Wrong Stage ID");
        id = stage_db_iter->id;
    }
    else
    {
        daily_stage_db stage_db_table(_self, _self.value);
        auto stage_db_iter = stage_db_table.find(_stage_number);
        eosio_assert(stage_db_iter != stage_db_table.end(), "Fail Reward : Empty Daily Stage ID / Wrong Daily Stage ID");
        id = stage_db_iter->id;
    }

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
        update_log.last_stage_num = id;
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
}


void battletest::new_win_reward(eosio::name _user, uint64_t _stage_id, uint64_t _seed, std::vector<uint32_t> _reward_monster_id)
{
    exp_info get_rank_exp;
    std::vector<exp_info> get_char_exp_list;
    std::vector<monster_data> monster_list;
    std::vector<equip_data> equipment_list;
    std::vector<servant_data> servant_list;
    std::vector<item_data> item_list;
    std::vector<uint32_t> servant_pos_list = {0, 1, 2, 3, 4};
    std::vector<uint32_t> monster_pos_list = {5, 6, 7, 8, 9};


    uint32_t rank_exp = 0;
    uint32_t char_exp = 0;
    uint32_t reward_count = 0;
    uint32_t reward_per_monster = 0;
    uint32_t reward_per_equipment = 0;
    uint32_t id = 0;
    uint64_t reward_utg = 0;
    std::vector<reward_item_info> reward_list;
    uint64_t top_user_stage_id = 0;

    user_logs user_log_table(_self, _self.value);
    auto user_log_iter = user_log_table.find(_user.value);
    eosio_assert(user_log_iter != user_log_table.end(), "Win Reward : Empty Log Table / Not Yet Signup");

    if (_stage_id / 10000 == 0)
    {
        new_reward_db reward_db_table(_self, _self.value);
        auto reward_iter = reward_db_table.find(_stage_id);
        eosio_assert(reward_iter != reward_db_table.end(), "Win Reward : Empty Reward ID / Wrong Reward ID");

        rank_exp = reward_iter->rank_exp;
        char_exp = reward_iter->char_exp;
        reward_count = reward_iter->reward_count;
        reward_per_monster = reward_iter->per_monster;
        reward_per_equipment = reward_iter->per_equipment;
        id = reward_iter->id;
        reward_list = reward_iter->reward_list;
        reward_utg = reward_iter->reward_utg;

        auto stage_log_iter = reward_db_table.find(user_log_iter->top_clear_stage);

        if (stage_log_iter == reward_db_table.end())
        {
            top_user_stage_id = id;
        }
        else
        {
            if (stage_log_iter->char_exp < char_exp)
            {
                top_user_stage_id = id;
            }
        }
    }
    else
    {
        day_reward_db reward_db_table(_self, _self.value);
        auto reward_iter = reward_db_table.find(_stage_id);
        eosio_assert(reward_iter != reward_db_table.end(), "Win Reward : Empty Reward ID / Wrong Reward ID");

        rank_exp = reward_iter->rank_exp;
        char_exp = reward_iter->char_exp;
        reward_count = reward_iter->reward_count;
        reward_per_monster = reward_iter->per_monster;
        reward_per_equipment = reward_iter->per_equipment;
        id = reward_iter->id;
        reward_list = reward_iter->reward_list;
        reward_utg = reward_iter->reward_utg;
    }

    user_partys user_party_table(_self, _user.value);
    auto user_party_iter = user_party_table.begin();
    eosio_assert(user_party_iter != user_party_table.end(), "Win Reward : Empty Party Number / Not Yet Signup");

    user_auths user_auth_table(_self, _self.value);
    auto user_auth_iter = user_auth_table.find(_user.value);
    eosio_assert(user_auth_iter != user_auth_table.end(), "Win Reward : Empty Auth Table / Not Yet Signup");

    lv_exp lv_exp_table(_self, _self.value);
    uint32_t get_exp = user_auth_iter->exp + rank_exp;
    uint32_t level_up_count = check_rank_level_up(user_auth_iter->rank, get_exp);
    if(user_auth_iter->rank + level_up_count >= MAX_LEVEL)
    {
        auto lv_iter = lv_exp_table.find(MAX_LEVEL - 1);
        level_up_count = MAX_LEVEL - user_auth_iter->rank;
        if (get_exp >= lv_iter->rank_exp)
        {
            get_exp = lv_iter->rank_exp;
        }
    }
    get_rank_exp.pos = 0;
    get_rank_exp.exp = get_exp - user_auth_iter->exp;
    get_rank_exp.lvup = level_up_count;

    user_auth_table.modify(user_auth_iter, _self, [&](auto &upadate_exp) {
        upadate_exp.rank += level_up_count;
        upadate_exp.exp = get_exp;
        upadate_exp.state = user_state::lobby;
    });

    
    user_servants user_servant_table(_self, _user.value);
    for (uint32_t i = 0; i < 5; ++i)
    {
        if (user_party_iter->servant_list[i] == 0)
        {
            continue;
        }
        auto user_servant_iter = user_servant_table.find(user_party_iter->servant_list[i]);
        eosio_assert(user_servant_iter != user_servant_table.end(), "Win Reward : Empty Servant Index / Wrong Servant Index");
        uint32_t servant_max_level = (MAX_LEVEL + user_servant_iter->servant.limit_break);
        uint64_t get_exp = user_servant_iter->servant.exp + char_exp;
        uint32_t level_up_count = check_char_level_up(user_servant_iter->servant.level, get_exp, user_servant_iter->servant.limit_break);
        if (user_servant_iter->servant.level + level_up_count >= servant_max_level)
        {
            auto lv_iter = lv_exp_table.find(servant_max_level - 1);
            level_up_count = servant_max_level - user_servant_iter->servant.level;
            if (get_exp >= lv_iter->char_exp)
            {
                get_exp = lv_iter->char_exp;
            }
        }

        exp_info char_exp;
        char_exp.pos = servant_pos_list[i];
        char_exp.exp =  get_exp - user_servant_iter->servant.exp;
        char_exp.lvup = level_up_count;
        get_char_exp_list.push_back(char_exp);

        // servant_db servant_db_table(_self, _self.value);
        // auto servant_db_iter = servant_db_table.find(user_servant_iter->servant.id);
        // eosio_assert(servant_db_iter != servant_db_table.end(), "Win Reward : Empty Servant ID / Wrong Servant UD");

        auto servant_db_iter = get_servant_db(user_servant_iter->servant.id);
        user_servant_table.modify(user_servant_iter, _self, [&](auto &update_servant_exp) {
            update_servant_exp.servant.level += level_up_count;
            update_servant_exp.servant.exp = get_exp;
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

        uint32_t monster_max_level = MAX_LEVEL + user_monster_iter->monster.limit_break;

        uint64_t get_exp = user_monster_iter->monster.exp + char_exp;
        uint32_t level_up_count = check_char_level_up(user_monster_iter->monster.level, get_exp, user_monster_iter->monster.limit_break);
        if(user_monster_iter->monster.level + level_up_count >= monster_max_level)
        {
            auto lv_iter = lv_exp_table.find(monster_max_level - 1);
            level_up_count = monster_max_level - user_monster_iter->monster.level;
            if (get_exp >= lv_iter->char_exp)
            {
                get_exp = lv_iter->char_exp;
            }
        }

        exp_info char_exp;
        char_exp.pos = monster_pos_list[i];
        char_exp.exp = get_exp - user_monster_iter->monster.exp;
        char_exp.lvup = level_up_count;
        get_char_exp_list.push_back(char_exp);

        user_monster_table.modify(user_monster_iter, _self, [&](auto &update_monster_exp) {
            update_monster_exp.monster.level += level_up_count;
            update_monster_exp.monster.exp = get_exp;
        });
    }

    uint32_t type = _stage_id % 1000;
    uint32_t grade = type % 10;
    uint32_t tier = type / 10;
    
    for(uint32_t i = 0; i < reward_count; ++i)
    {
        uint64_t seed = _seed >> (i + 1);
        uint32_t reward_rate = reward_per_monster;
        uint64_t random_rate = safeseed::get_random_value(seed, 1000000, 0, i);
        if(random_rate < reward_rate)
        {
            uint64_t random_rate = safeseed::get_random_value(seed, GACHA_MAX_RATE, DEFAULT_MIN, 1);
            uint64_t random_grade = get_random_grade(random_rate);
            if(grade > random_grade)
            {
                random_grade = 5;
            }
            uint64_t random_monster = safeseed::get_random_value((seed >> 1), _reward_monster_id.size(), 0, 0);
            //monster_data new_monster = get_reward_monster(_user, _reward_monster_id[random_monster], random_grade, seed,1);
            monster_data new_monster = get_monster(_user, _reward_monster_id[random_monster],random_grade,0,4,seed);
            monster_list.push_back(new_monster);
        }
        else
        {
            reward_rate += reward_per_equipment;
            if(random_rate < reward_rate)
            {
                uint32_t max_tier_count = 0;
                uint32_t reward_tier = 0;
                if(tier == 1)
                {
                    reward_tier = 1;
                    max_tier_count = TIER_1_MAX_COUNT;
                }
                else if(tier == 2)
                {
                    reward_tier = safeseed::get_random_value(seed, 3, 1, 4);
                    if(reward_tier == 1)
                    {
                        max_tier_count = TIER_1_MAX_COUNT;
                    }
                    else
                    {
                        max_tier_count = TIER_2_MAX_COUNT;
                    }
                }
                else if(tier == 3)
                {
                    reward_tier = 2;
                     max_tier_count = TIER_2_MAX_COUNT;
                }
                else if(tier == 4)
                {
                    reward_tier = safeseed::get_random_value(seed, 4, 2, 4);
                    if(reward_tier == 2)
                    {
                        max_tier_count = TIER_2_MAX_COUNT;
                    }
                    else
                    {
                        max_tier_count = TIER_3_MAX_COUNT;
                    }
                }
                else if(tier == 5)
                {
                    reward_tier = 3;
                     max_tier_count = TIER_3_MAX_COUNT;
                }
                else if(tier == 6)
                {
                    reward_tier = safeseed::get_random_value(seed, 5, 3, 4);
                    if(reward_tier == 3)
                    {
                        max_tier_count = TIER_3_MAX_COUNT;
                    }
                    else
                    {
                        max_tier_count = TIER_4_MAX_COUNT;
                    }
                }
                else if(tier == 7)
                {
                    reward_tier = 4;
                    max_tier_count = TIER_4_MAX_COUNT;
                }

                equipment_db equipment_db_table(_self, _self.value);
                auto equip_iter = equipment_db_table.get_index<"tier"_n>();
                auto tier_iter = equip_iter.lower_bound(reward_tier);
                uint64_t random_equipment = safeseed::get_random_value((seed >> 2), max_tier_count + 1, 1, 0);
                uint32_t count = 0;
                while(tier_iter != equip_iter.end())
                {
                    count++;
                    if(count == random_equipment)
                    {
                        uint64_t random_rate = safeseed::get_random_value(seed, GACHA_MAX_RATE, DEFAULT_MIN, 1);
                        uint64_t random_grade = get_random_grade(random_rate);
                        if (grade > random_grade)
                        {
                            random_grade = 5;
                        }
                        //equip_data new_equipment = get_reward_equip(_user, tier_iter->item_id, random_grade, seed,1);
                        equip_data new_equipment = get_equip(_user, tier_iter->item_id, random_grade, 0, 4, seed);
                        equipment_list.push_back(new_equipment);
                        break;
                    }
                    tier_iter++;
                }
            }
            else
            {
                for(uint32_t reward = 0; reward < reward_list.size(); ++reward)
                {
                    reward_rate += reward_list[reward].per;
                    if(random_rate < reward_rate)
                    {
                       // item_data new_item = get_reward_item(_user, reward_iter->reward_list[reward].id, reward_iter->reward_list[reward].count,1);
                        item_data new_item = get_item(_user, reward_list[reward].id, reward_list[reward].count, 4,0);
                        item_list.push_back(new_item);
                        break;
                    }
                }
            }
        }
    }

    asset stage_reward_money(0, symbol(symbol_code("UTG"), 4));

    stats statstable(_self, stage_reward_money.symbol.code().raw());
    auto existing = statstable.find(stage_reward_money.symbol.code().raw());
    eosio_assert(existing != statstable.end(),"Win Reward : Empty Stat");

    account from_acnts(_self, _self.value);
    auto from = from_acnts.find(stage_reward_money.symbol.code().raw());    
    eosio_assert(from != from_acnts.end(),"Win Reward : Empty Balance");

    uint64_t contract_token_amount = from->balance.amount / 10000;
    uint64_t total_token_amount = existing->supply.amount / 10000;

    uint64_t token_per = (contract_token_amount * 100) / total_token_amount;
    if((contract_token_amount % total_token_amount) > 0)
    {
        token_per += 1;
    }

    stage_reward_money.amount = (reward_utg * token_per) / 100;
    uint64_t remain_utg = stage_reward_money.amount % 10000;
    if (remain_utg > 0)
    {
        stage_reward_money.amount += (10000 - remain_utg);
    }

    action(permission_level{_self, "active"_n},
           _self, "transfer"_n,
           std::make_tuple(_self, _user, stage_reward_money, std::string("stage reward")))
        .send();



    user_log_table.modify(user_log_iter, _self, [&](auto &update_log) {
        if (top_user_stage_id != 0)
        {
            update_log.top_clear_stage = top_user_stage_id;
        }
        update_log.last_stage_num = _stage_id;
        update_log.battle_count += 1;
        update_log.get_utg += stage_reward_money.amount;
    });


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

    new_battle_state_list battle_state_list_table(_self, _self.value);
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


//pvp

ACTION battletest::pvpstart(eosio::name _from, eosio::name _to)
{
    system_check(_from);

    eosio_assert(_from != _to, "PVP Start : Self PVP Impossible");
    std::vector<uint32_t> servant_pos_list = {0, 1, 2, 3, 4};
    std::vector<uint32_t> monster_pos_list = {5, 6, 7, 8, 9};

    // _from 이 유효한 계정인지, pvp가 가능한 상태인지 체크. _to 가 유효한 계정인지 체크
    change_user_state(_from, user_state::lobby, user_state::pvp);

    user_auths user_auth(_self, _self.value);
    auto to_user_iter = user_auth.find(_to.value);
    eosio_assert(to_user_iter != user_auth.end(), "PVP Start : Empty Auth Table / Not Yet Signup");


    // _from과 _to 가 유효한 파티를 가지고 있는지 확인
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

    std::vector<std::string> from_state;
    std::vector<std::string> to_state;

    new_battle_state_list pvp_table(_self, _self.value);
    auto pvp_iter = pvp_table.find(_from.value);

    if (pvp_iter == pvp_table.end())
    {
        pvp_table.emplace(_self, [&](auto &data) {
            data.user = _from;
            data.enemy_user = _to;
            data.type = 0;
            data.floor = 1;
            data.difficult = 5;
            data.turn = 0;


            // _from setting
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

            // _to setting
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
            data.enemy_user = _to;
            data.type = 0;
            data.floor = 1;
            data.difficult = 5;
            data.turn = 0;
            data.my_state_list.clear();
            data.enemy_state_list.clear();
            data.my_synergy_list.clear();
            data.enemy_synergy_list.clear();

            // _from setting
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

            // _to setting
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
}




///tower

void battletest::towersnap(uint64_t fnum) //층수 인자값 추가
{
    // eos_logs eos_log_table(_self, _self.value);
    // auto eos_log_iter = eos_log_table.find(_self.value);
    // eosio_assert(eos_log_iter != eos_log_table.end(), "Need Set Eos Log");

    // eos_snapshots eos_snapshot_table(_self, _self.value);
    // eos_snapshot_table.emplace(_self, [&](auto &new_snap_shot) {
    //     uint32_t first_index = eos_snapshot_table.available_primary_key();
    //     new_snap_shot.snapshot_count = fnum;
    //     new_snap_shot.signup_eos = eos_log_iter->signup_eos;
    //     new_snap_shot.status_change_num = eos_log_iter->status_change_num;
    //     new_snap_shot.gacha_eos = eos_log_iter->gacha_eos;
    //     new_snap_shot.total_eos = eos_log_iter->signup_eos + eos_log_iter->status_change_num + eos_log_iter->gacha_eos;
    // });

    // eos_log_table.modify(eos_log_iter, _self, [&](auto &new_log) {
    //     new_log.signup_eos = 0;
    //     new_log.status_change_num = 0;
    //     new_log.gacha_eos = 0;
    // });
}

// ACTION battletest::toweropen()
// {
//     require_auth(_self);

//     uint64_t _floor = 1;
//     floor_index floortable(_self, _self.value);
//     auto iter = floortable.find(_floor);

//     eosio_assert(iter == floortable.end(), "Tower is already open.");

//     floortable.emplace(_self, [&](auto &floordata) {
//         floordata.fnum = _floor;
//         floordata.owner = _self;
//         floordata.bnum = 0;
//         floordata.pnum = 0;
//         floordata.state = "lock";
//         floordata.endtime = 0;
//     });
// }

// ACTION battletest::endflag(name _winner, uint64_t _fnum)
// {
//     require_auth(_self);
//     floor_index floortable(_self, _self.value);
//     const auto &f_iter = floortable.get(_fnum, "Floor info does not exist");

//     eosio_assert(f_iter.owner == _winner, "It does not match the Floor Master.");

//     eosio_assert(f_iter.endtime <= now(), "Not enough time.");

//     floortable.modify(f_iter, _self, [&](auto &floordata) {
//         floordata.state = "end";
//     });

//     towersnap(_fnum);
// }


void battletest::resetparty(eosio::name _user, uint64_t _pnum)
{
    user_partys user_party_table(_self, _user.value);
    auto party_iter = user_party_table.find(_pnum);
    eosio_assert(party_iter != user_party_table.end(), "Not Exist Party");

    for (uint32_t i = 0; i < party_iter->servant_list.size(); ++i)
    {
        if (party_iter->servant_list[i] == 0)
        {
            continue;
        }
        user_servants user_servant_table(_self, _user.value);
        auto servant = user_servant_table.find(party_iter->servant_list[i]);
        eosio_assert(servant != user_servant_table.end(), "Not Exist Servant");
        for (uint32_t equip = 0; equip < servant->servant.equip_slot.size(); ++equip)
        {
            if (servant->servant.equip_slot[equip] == 0)
            {
                continue;
            }
            user_equip_items user_equip_table(_self, _user.value);
            auto equip_iter = user_equip_table.find(servant->servant.equip_slot[equip]);
            eosio_assert(equip_iter != user_equip_table.end(), "Not Exist Equipment");
            user_equip_table.erase(equip_iter);
        }
        user_servant_table.erase(servant);
    }

    for (uint32_t i = 0; i < party_iter->monster_list.size(); ++i)
    {
        if (party_iter->monster_list[i] == 0)
        {
            continue;
        }
        user_monsters user_servant_table(_self, _user.value);
        auto monster = user_servant_table.find(party_iter->monster_list[i]);
        eosio_assert(monster != user_servant_table.end(), "Not Exist Monster");
        user_servant_table.erase(monster);
    }
}

// ACTION battletest::claim(name who, uint64_t fnum)
// {
//     system_check(who);
//     floor_index floortable(_self, _self.value);
//     const auto &f_iter = floortable.get(fnum, "Floor info does not exist");

//     eosio_assert(f_iter.owner == who, "It does not match the Floor Master.");
//     eosio_assert(f_iter.endtime <= now(), "Not enough time.");
//     eosio_assert(f_iter.state == "end", "Impossible state");

//     // 우승자 테이블에서 파티 정보 삭제
//     //지우는 처리 들어감
//     resetparty(who, 1);

//     // 다음층 테이블 추가

//     floortable.emplace(who, [&](auto &floordata) {
//         floordata.fnum = fnum + 1;
//         floordata.owner = _self;
//         floordata.bnum = 0;
//         floordata.pnum = 0;
//         floordata.state = "lock";
//         floordata.endtime = 0;
//     });

//     // 우승자 정보 수정
//     user_logs user_log(_self, _self.value);
//     auto log_iter = user_log.find(who.value);
//     user_log.modify(log_iter, who, [&](auto &data) {
//         data.top_clear_tower = fnum + 1;
//     });

//     // EOS 스냅샷 확인후 지급
//     eos_snapshots eos_snapshot_table(_self, _self.value);
//     auto eos_snapshot_iter = eos_snapshot_table.find(fnum);
//     eosio_assert(eos_snapshot_iter != eos_snapshot_table.end(), "Wrong SnapShot");

//     asset tower_reward(0, symbol(symbol_code("EOS"), 4));
//     tower_reward.amount = eos_snapshot_iter->total_eos;

//     std::string memo;
//     memo += "tower:";
//     memo += to_string(fnum);
//     //이오스 보내는것에 대한 예외처리 필요
//     action(permission_level{get_self(), "active"_n},
//            "eosio.token"_n, "transfer"_n,
//            std::make_tuple(_self, who, tower_reward, memo))
//         .send();
// }

void battletest::towerwin(eosio::name winner, uint64_t fnum, uint64_t pnum, uint64_t bnum)
{
    // floor_index floortable(_self, _self.value);
    // auto f_iter = floortable.find(fnum);

    // eosio_assert(f_iter->bnum == bnum, "Another user has already conquered.");

    // if (f_iter->owner == _self)
    // {
    //     // 비어있는걸 차지한 경우
    //     uint64_t temp = 0;
    //     settower(_self, winner, temp, pnum);
    // }
    // else
    // {
    //     // 도전해서 이긴 경우
    //     settower(f_iter->owner, winner, f_iter->pnum, pnum);
    // }
    // // 층이 이미 정복된 경우에는 사용자 정보만 변경
    // if (f_iter->state == "end")
    // {
    //     user_logs user_log(_self, _self.value);
    //     auto iter = user_log.find(winner.value);

    //     if (iter->top_clear_tower == fnum)
    //     {
    //         user_log.modify(iter, _self, [&](auto &data) {
    //             data.top_clear_tower = data.top_clear_tower + 1;
    //         });
    //     }
    // }
    // // 층을 정복한 경우, 혹은 최초 등록시에는 NPC화
    // else
    // {
    //     floortable.modify(f_iter, _self, [&](auto &floordata) {
    //         floordata.owner = winner;
    //         floordata.bnum = bnum + 1;
    //         floordata.pnum = pnum; // 패배시 유저의 어떤 파티인지 알기 위해 기록
    //         floordata.endtime = now() + 86400;
    //         floordata.state = "idle";
    //     });
    //     /***********************/
    //     /**** 파티 정보 저장 ****/
    //     /***********************/

    //     // 우승자가 존재하는 유저인지 체크
    //     user_auths user(_self, _self.value);
    //     auto user_exist_iter = user.find(winner.value);
    //     eosio_assert(user_exist_iter != user.end(), "Not Exist User.");

    //     // 우승자 파티 정보 저장
    //     user_equip_items user_equipment(_self, winner.value);
    //     user_equip_items npc_equipment(_self, fnum);

    //     user_partys user_party(_self, winner.value);
    //     auto user_party_iter = user_party.get(pnum, "Not exist party.");

    //     user_servants user_servant(_self, winner.value);
    //     user_servants npc_servant(_self, fnum);

    //     user_monsters user_monster(_self, winner.value);
    //     user_monsters npc_monster(_self, fnum);

    //     // 서번트 정보 이식
    //     uint64_t servant_idx = 1;
    //     uint64_t equipment_idx = 1;

    //     // NPC 테이블이 비어있나 확인
    //     auto npc_servant_iter = npc_servant.find(servant_idx);

    //     // NPC 테이블이 비어 있는 경우
    //     if (npc_servant_iter == npc_servant.end())
    //     {
    //         // 유저 파티 정보에서 해당 파티에 배치된 서번트 리스트를 가져옴
    //         for (auto iter = user_party_iter.servant_list.begin(); iter != user_party_iter.servant_list.end(); iter++)
    //         {
    //             if(*iter == 0){
    //                 npc_servant.emplace(_self, [&](auto &npc_data) {
    //                     npc_data.index = servant_idx;
    //                     npc_data.party_number = 0;
    //                 });
    //                 servant_idx += 1;
    //                 for(uint32_t i = 0; i < 3; ++i)
    //                 {
    //                     equipment_info temp;
    //                     temp.id = 0;
    //                     temp.state = 0;
    //                     temp.type = 0;
    //                     temp.tier = 0;
    //                     temp.job = 0;
    //                     temp.grade = 0;
    //                     temp.upgrade = 0;
    //                     temp.value = 0;
    //                     temp.equipservantindex = servant_idx;

    //                     npc_equipment.emplace(_self, [&](auto &equipment_data) {
    //                         equipment_data.index = equipment_idx;
    //                         equipment_data.equipment = temp;
    //                     });
    //                     equipment_idx += 1;
    //                 }
    //                 continue;
    //             }
    //             // 서번트 리스트에 있는 서번트 인덱스로 서번트 정보를 가져옴
    //             auto user_servant_iter = user_servant.get(*iter, "Not exist servant");

    //             // 각 층마다 서번트 몬스터는 고정이므로, 1번부터 5번인덱스로 삽입
    //             npc_servant.emplace(_self, [&](auto &npc_data) {
    //                 npc_data.index = servant_idx;
    //                 npc_data.party_number = fnum;
    //                 npc_data.servant = user_servant_iter.servant;

    //                 // 서번트 아이템 복사
    //                 uint8_t temp_index = 0;
    //                 for (auto item_iter = user_servant_iter.servant.equip_slot.begin(); item_iter != user_servant_iter.servant.equip_slot.end(); item_iter++)
    //                 {
    //                     if (*item_iter != 0)
    //                     {
    //                         // 장착을 했다면 해당 인덱스로 유저 테이블에서 장착 아이템 정보를 가져옴
    //                         auto user_equipment_iter = user_equipment.get(*item_iter, "Not exist item");

    //                         npc_equipment.emplace(_self, [&](auto &equipment_data) {
    //                             equipment_data.index = equipment_idx;
    //                             equipment_data.equipment = user_equipment_iter.equipment;
    //                             equipment_data.equipment.equipservantindex = servant_idx;
    //                         });
    //                         npc_data.servant.equip_slot[temp_index] = equipment_idx;
    //                     }
    //                     else
    //                     {
    //                         // 장착된 아이템이 없다면 id값을 0으로 해서 데이터 삽입
    //                         equipment_info temp;
    //                         temp.id = 0;
    //                         temp.state = 0;
    //                         temp.type = 0;
    //                         temp.tier = 0;
    //                         temp.job = 0;
    //                         temp.grade = 0;
    //                         temp.upgrade = 0;
    //                         temp.value = 0;
    //                         temp.equipservantindex = servant_idx;

    //                         npc_equipment.emplace(_self, [&](auto &equipment_data) {
    //                             equipment_data.index = equipment_idx;
    //                             equipment_data.equipment = temp;
    //                         });

    //                         npc_data.servant.equip_slot[temp_index] = 0;
    //                     }
    //                     equipment_idx++;
    //                     temp_index++;
    //                 }

    //             });

    //             servant_idx++;
    //         }
    //     }
    //     else
    //     {
    //         for (auto iter = user_party_iter.servant_list.begin(); iter != user_party_iter.servant_list.end(); iter++)
    //         {
    //             if (*iter == 0)
    //             {
    //                 auto temp_servant_iter = npc_servant.find(*iter);
    //                 npc_servant.modify(temp_servant_iter, _self, [&](auto &npc_data) {
    //                     npc_data.party_number = 0;
    //                 });
    //                 servant_idx += 1;
    //                 for (uint32_t i = 0; i < 3; ++i)
    //                 {
    //                     auto temp_iter = npc_equipment.find(equipment_idx);
    //                     equipment_info temp;
    //                     temp.id = 0;
    //                     temp.state = 0;
    //                     temp.type = 0;
    //                     temp.tier = 0;
    //                     temp.job = 0;
    //                     temp.grade = 0;
    //                     temp.upgrade = 0;
    //                     temp.value = 0;
    //                     temp.equipservantindex = servant_idx;

    //                     npc_equipment.modify(temp_iter, _self, [&](auto &equipment_data) {
    //                         equipment_data.equipment = temp;
    //                     });
    //                     equipment_idx += 1;
    //                 }
    //                 continue;
    //             }
    //             auto user_servant_iter = user_servant.get(*iter, "Not exist servant");
    //             auto temp_iter = npc_servant.find(servant_idx);

    //             npc_servant.modify(temp_iter, _self, [&](auto &npc_data) {
    //                 npc_data.party_number = fnum;
    //                 npc_data.servant = user_servant_iter.servant;

    //                 // 서번트 아이템 복사
    //                 uint8_t temp_index = 0;
    //                 for (auto item_iter = user_servant_iter.servant.equip_slot.begin(); item_iter != user_servant_iter.servant.equip_slot.end(); item_iter++)
    //                 {
    //                     auto npc_temp_iter = npc_equipment.find(equipment_idx);

    //                     if (*item_iter != 0)
    //                     {
    //                         auto user_equipment_iter = user_equipment.get(*item_iter, "Not exist item");

    //                         npc_equipment.modify(npc_temp_iter, _self, [&](auto &equipment_data) {
    //                             equipment_data.equipment = user_equipment_iter.equipment;
    //                             equipment_data.equipment.equipservantindex = servant_idx;
    //                         });

    //                         npc_data.servant.equip_slot[temp_index] = equipment_idx;
    //                     }
    //                     else
    //                     {
    //                         equipment_info temp;
    //                         temp.id = 0;
    //                         temp.state = 0;
    //                         temp.type = 0;
    //                         temp.tier = 0;
    //                         temp.job = 0;
    //                         temp.grade = 0;
    //                         temp.upgrade = 0;
    //                         temp.value = 0;
    //                         temp.equipservantindex = servant_idx;

    //                         npc_equipment.modify(npc_temp_iter, _self, [&](auto &equipment_data) {
    //                             equipment_data.equipment = temp;
    //                         });

    //                         npc_data.servant.equip_slot[temp_index] = 0;
    //                     }
    //                     equipment_idx++;
    //                     temp_index++;
    //                 }
    //             });

    //             servant_idx++;
    //         }
    //     }

    //     // 몬스터 정보 이식
    //     uint64_t monster_idx = 1;
    //     auto npc_monster_iter = npc_monster.find(monster_idx);

    //     if (npc_monster_iter == npc_monster.end())
    //     {
    //         for (auto iter = user_party_iter.monster_list.begin(); iter != user_party_iter.monster_list.end(); iter++)
    //         {
    //             if(*iter == 0)
    //             {
    //                 npc_monster.emplace(_self, [&](auto &npc_data) {
    //                     npc_data.index = monster_idx;
    //                     npc_data.party_number = 0;
    //                 });
    //                 monster_idx += 1;
    //                 continue;
    //             }
    //             auto user_monster_iter = user_monster.get(*iter, "Not exist servant");

    //             npc_monster.emplace(_self, [&](auto &npc_data) {
    //                 npc_data.index = monster_idx;
    //                 npc_data.party_number = fnum;
    //                 npc_data.monster = user_monster_iter.monster;
    //             });

    //             monster_idx++;
    //         }
    //     }
    //     else
    //     {
    //         for (auto iter = user_party_iter.monster_list.begin(); iter != user_party_iter.monster_list.end(); iter++)
    //         {
    //             if (*iter == 0)
    //             {
    //                 auto temp_monster_iter = npc_monster.find(monster_idx);
    //                 npc_monster.modify(temp_monster_iter, _self, [&](auto &npc_data) {
    //                     npc_data.party_number = 0;
    //                 });
    //                 monster_idx += 1;
    //                 continue;
    //             }
    //             auto user_monster_iter = user_monster.find(*iter);

    //             auto npc_monster_iter = npc_monster.find(monster_idx);

    //             npc_monster.modify(npc_monster_iter, _self, [&](auto &npc_data) {
    //                 npc_data.index = monster_idx;
    //                 npc_data.party_number = fnum;
    //                 npc_data.monster = user_monster_iter->monster;
    //             });

    //             monster_idx++;
    //         }
    //     }
    // }
}

void battletest::towerlose(name loser)
{
    // battle_state_list battle_state_list_table(_self, _self.value);
    // auto user_battle_state_iter = battle_state_list_table.find(_user.value);
    // eosio_assert(user_battle_state_iter != battle_state_list_table.end(), "End Battle 1");

    // battle_state_list_table.modify(user_battle_state_iter, _self, [&](auto &end_battle) {
    //     end_battle.turn = stage_state::lose;
    // });
}

void battletest::get_tower_state(uint64_t _fnum, std::vector<character_state_data> &_enemy_state_list, std::vector<std::string> &_state)
{

    status_info status;
    status_info basic_status;

    uint32_t temp_equip_iter = 1;
    std::vector<uint32_t> servant_pos_list = {10, 11, 12, 13, 14};
    std::vector<uint32_t> monster_pos_list = {15, 16, 17, 18, 19};

    user_equip_items npc_equipment(_self, _fnum);
    user_servants npc_servant(_self, _fnum);
    for (uint32_t b = 1; b < 6; ++b)
    {
        uint32_t increase_hp = 0;
        uint32_t increase_hp_per = 0;
        character_state_data get_state;
        auto user_servant_iter = npc_servant.find(b);
        eosio_assert(user_servant_iter != npc_servant.end(), "Tower User State : Empty Servant Index / Wrong Servant Index");
        if (user_servant_iter->party_number == 0)
        {
            continue;
        }

        //서번트 아이디가 제대로 된 아이디인지 확인
        servant_db servant_db_table(_self, _self.value);
        auto servant_db_iter = servant_db_table.find(user_servant_iter->servant.id);
        eosio_assert(servant_db_iter != servant_db_table.end(), "Tower User State : Empty Servant ID / Wrong Servant ID");

        status = user_servant_iter->servant.status;       //증가시킬 스테이터스
        basic_status = user_servant_iter->servant.status; //기본 스테이터스

        user_equip_items npc_equipment(_self, _fnum);
        equipment_db equipment_db_table(_self, _self.value);
        for (uint32_t equip = 0; equip < user_servant_iter->servant.equip_slot.size(); ++equip) //서번트 장비 슬롯을 돌면서 힘민지에 대한 증가 효과 적용
        {
            if (user_servant_iter->servant.equip_slot[equip] == 0)
            {
                temp_equip_iter += 1;
                continue;
            }
            auto user_equipment_iter = npc_equipment.find(temp_equip_iter);
            eosio_assert(user_equipment_iter != npc_equipment.end(), "Tower User State : Empty Equipment Index / Wrong Equipment Index");

            auto db_equipment_iter = equipment_db_table.find(user_equipment_iter->equipment.id);
            eosio_assert(db_equipment_iter != equipment_db_table.end(), "Tower User State : Empty Equipment ID / Wrong Equipment ID");

            for (uint32_t a = 0; a < db_equipment_iter->option_list.size(); ++a)
            {
                if (db_equipment_iter->option_list[a] == option_list::status_str) //힘 증가
                {
                    status.basic_str += (user_equipment_iter->equipment.value + ((user_equipment_iter->equipment.upgrade * 10) / 100));
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_dex) //민 증가
                {
                    status.basic_dex += (user_equipment_iter->equipment.value + ((user_equipment_iter->equipment.upgrade * 10) / 100));
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_int) //지 증가
                {
                    status.basic_int += (user_equipment_iter->equipment.value + ((user_equipment_iter->equipment.upgrade * 10) / 100));
                }
                else if (db_equipment_iter->option_list[a] == option_list::status_hp) //hp 증가
                {
                    increase_hp += (user_equipment_iter->equipment.value + ((user_equipment_iter->equipment.upgrade * 10) / 100));
                }
            }
            temp_equip_iter += 1;
        }

        //힘민지에 대한 패시브 적용
        for (uint32_t pass = 0; pass < user_servant_iter->servant.passive_skill.size(); ++pass)
        {
            passive_db passive_db_table(_self, _self.value);
            auto passive_db_iter = passive_db_table.find(user_servant_iter->servant.passive_skill[pass]);
            eosio_assert(passive_db_iter != passive_db_table.end(), "Tower User State : Empty Servant Passive ID / Wrong Servant Passive ID");
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

        //액티브 스킬의 존재 여부 확인
        for (uint32_t active = 0; active < user_servant_iter->servant.active_skill.size(); ++active)
        {
            active_db active_db_table(_self, _self.value);
            auto active_db_iter = active_db_table.find(user_servant_iter->servant.active_skill[active]);
            eosio_assert(active_db_iter != active_db_table.end(), "Tower User State : Empty Servant Active ID / Wrong Servant Active ID");

            get_state.active_skill_list.push_back(active_db_iter->active_id);
        }

        get_state.level = user_servant_iter->servant.level;
        get_state.grade = 5;
        get_state.index = user_servant_iter->index;
        get_state.id = user_servant_iter->servant.id;
        get_state.position = servant_pos_list[b - 1];
        get_state.now_hp = get_max_hp(status, user_servant_iter->servant.level) + increase_hp;
        get_state.now_hp += (get_state.now_hp * increase_hp_per) / 100;
        get_state.max_hp = get_state.now_hp;
        get_state.type = character_type::t_servant;
        get_state.upgrade = 0;
        get_state.state = battle_member_state::live;
        get_state.status = basic_status;

        _enemy_state_list.push_back(get_state);
    }

    user_monsters npc_monster(_self, _fnum);
    for (uint32_t a = 1; a < 6; ++a)
    {
        uint32_t increase_hp = 0;
        uint32_t increase_hp_per = 0;
        character_state_data get_state;
        auto user_monster_iter = npc_monster.find(a);
        eosio_assert(user_monster_iter != npc_monster.end(), "Tower User State : Empty Monster Index / Wrong Monster Index");
        if (user_monster_iter->party_number == 0)
        {
            continue;
        }

        //몬스터 아이디가 제대로된 아이디인지
        monster_db monster_db_table(_self, _self.value);
        auto monster_db_iter = monster_db_table.find(user_monster_iter->monster.id);
        eosio_assert(monster_db_iter != monster_db_table.end(), "Set User State : Empty Monster ID / Wrong Monster ID");

        status = user_monster_iter->monster.status;
        basic_status = user_monster_iter->monster.status;

        //몬스터 패시브 처리
        for (uint32_t i = 0; i < user_monster_iter->monster.passive_skill.size(); ++i)
        {
            passive_db passive_db_table(_self, _self.value);
            auto passive_db_iter = passive_db_table.find(user_monster_iter->monster.passive_skill[i]);
            eosio_assert(passive_db_iter != passive_db_table.end(), "Tower User State : Empty Monster Passive ID / Wrong Monster Passive ID");
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

        //몬스터 액티브 스킬 확인
        active_db active_db_table(_self, _self.value);
        for (uint32_t i = 0; i < user_monster_iter->monster.active_skill.size(); ++i)
        {
            auto active_db_iter = active_db_table.find(user_monster_iter->monster.active_skill[i]);
            eosio_assert(active_db_iter != active_db_table.end(), "Set User State : Empty Monster Active ID / Wrong Monster Active ID");

            get_state.active_skill_list.push_back(active_db_iter->active_id);
        }

        get_state.level = user_monster_iter->monster.level;
        get_state.grade = user_monster_iter->monster.grade;
        get_state.index = user_monster_iter->index;
        get_state.id = user_monster_iter->monster.id;
        get_state.position = monster_pos_list[a - 1];
        get_state.now_hp = get_max_hp(status, user_monster_iter->monster.level) + increase_hp;
        get_state.now_hp += (get_state.now_hp * increase_hp_per) / 100;
        get_state.max_hp = get_state.now_hp;
        get_state.type = character_type::t_monster;
        get_state.upgrade = user_monster_iter->monster.upgrade;
        get_state.state = battle_member_state::live;
        get_state.status = basic_status;

        _enemy_state_list.push_back(get_state);
      
    }
}

// ACTION battletest::towerstart(eosio::name _from, uint64_t _fnum)
// {
    // system_check(_from);

    // user_logs user_log_table(_self, _self.value);
    // auto log_iter = user_log_table.find(_from.value);
    // eosio_assert(log_iter != user_log_table.end(), "Tower Start : Empty Log Table / Not Yet Signup");
    // //eosio_assert(log_iter->top_clear_tower >= _fnum, "Tower Start : Impossilbe Challenge Floor");

    // eosio::name fnum;
    // fnum.value = _fnum;

    // floor_index floor_index_table(_self, _self.value);
    // auto floor_iter = floor_index_table.find(_fnum);
    // eosio_assert(floor_iter != floor_index_table.end(), "Tower Start : Empty Floor");
    // if (floor_iter->owner == _self)
    // {
    //     towerwin(_from, _fnum, 1, 0);
    // }
    // else
    // {
    //     std::vector<uint32_t> servant_pos_list = {0, 1, 2, 3, 4};
    //     std::vector<uint32_t> monster_pos_list = {5, 6, 7, 8, 9};

    //     change_user_state(_from,user_state::lobby, user_state::tower);        

    //     user_partys from_party(_self, _from.value);
    //     uint32_t from_party_num = 1;
    //     auto from_party_iter = from_party.find(from_party_num);
    //     eosio_assert(from_party_iter != from_party.end(), "Tower Start : Empty Party Table / Not Yet Signup");
    //     eosio_assert(from_party_iter->state == party_state::on_wait, "Tower Start : Party State Wrong");

    //     std::vector<std::string> from_state;
    //     std::vector<std::string> enemy_state;

    //     new_battle_state_list pvp_table(_self, _self.value);
    //     auto pvp_iter = pvp_table.find(_from.value);
    //     if (pvp_iter == pvp_table.end())
    //     {
    //         pvp_table.emplace(_from, [&](auto &data) {
    //             data.user = _from;
    //             data.enemy_user = fnum;
    //             data.type = 0;
    //             data.floor = _fnum;
    //             data.difficult = 5;
    //             data.turn = 0;

    //             // _from setting
    //             for (uint32_t i = 0; i < 5; ++i)
    //             {
    //                 if (from_party_iter->servant_list[i] != 0)
    //                 {
    //                     character_state_data from_servant_battle_state = get_user_state(_from, "ser", from_party_iter->servant_list[i], servant_pos_list[i], from_state);
    //                     data.my_state_list.push_back(from_servant_battle_state);
    //                 }
    //             }
    //             for (uint32_t i = 0; i < 5; ++i)
    //             {
    //                 if (from_party_iter->monster_list[i] != 0)
    //                 {
    //                     character_state_data from_monster_battle_state = get_user_state(_from, "mon", from_party_iter->monster_list[i], monster_pos_list[i], from_state);
    //                     data.my_state_list.push_back(from_monster_battle_state);
    //                 }
    //             }
    //             get_tower_state(_fnum, data.enemy_state_list, enemy_state);
    //             set_synergy(data.my_state_list, data.my_synergy_list);
    //             set_hp_synergy(data.my_state_list, data.my_synergy_list);

    //             set_synergy(data.enemy_state_list, data.enemy_synergy_list);
    //             set_hp_synergy(data.enemy_state_list, data.enemy_synergy_list);
    //         });
    //     }
    //     else
    //     {
    //         pvp_table.modify(pvp_iter, _from, [&](auto &data) {
    //             data.enemy_user = fnum;
    //             data.type = 0;
    //             data.floor = _fnum;
    //             data.difficult = 5;
    //             data.turn = 0;
    //             data.my_state_list.clear();
    //             data.enemy_state_list.clear();


    //             // _from setting
    //             for (uint32_t i = 0; i < 5; ++i)
    //             {
    //                 if (from_party_iter->servant_list[i] != 0)
    //                 {
    //                     character_state_data from_servant_battle_state = get_user_state(_from, "ser", from_party_iter->servant_list[i], servant_pos_list[i], from_state);
    //                     data.my_state_list.push_back(from_servant_battle_state);
    //                 }
    //             }
    //             for (uint32_t i = 0; i < 5; ++i)
    //             {
    //                 if (from_party_iter->monster_list[i] != 0)
    //                 {
    //                     character_state_data from_monster_battle_state = get_user_state(_from, "mon", from_party_iter->monster_list[i], monster_pos_list[i], from_state);
    //                     data.my_state_list.push_back(from_monster_battle_state);
    //                 }
    //             }
    //             get_tower_state(_fnum, data.enemy_state_list, enemy_state);
                
    //             set_synergy(data.my_state_list, data.my_synergy_list);
    //             set_hp_synergy(data.my_state_list, data.my_synergy_list);

    //             set_synergy(data.enemy_state_list, data.enemy_synergy_list);
    //             set_hp_synergy(data.enemy_state_list, data.enemy_synergy_list);

    //         });
    //     }
    //     init_action_reward_table(_from);
    // }
//}

// ACTION battletest::deletetower()
// {
//     require_auth(_self);

// floor_index floor_index_table(_self, _self.value);
// for(auto iter = floor_index_table.begin(); iter != floor_index_table.end();)
// {
//     auto fl = floor_index_table.find(iter->primary_key());

//     // user_servants table(_self, fl->fnum);
//     // for(auto ser = table.begin(); ser != table.end();)
//     // {
//     //     auto s = table.find(ser->primary_key());
//     //     table.erase(s);
//     //     ser++;
//     // }
//     // user_monsters npc_mon(_self, fl->fnum);
//     // for (auto ser = npc_mon.begin(); ser != npc_mon.end();)
//     // {
//     //     auto s = npc_mon.find(ser->primary_key());
//     //     npc_mon.erase(s);
//     //     ser++;
//     // }

//     // user_equip_items npc_equip(_self, fl->fnum);
//     // for (auto ser = npc_equip.begin(); ser != npc_equip.end();)
//     // {
//     //     auto s = npc_equip.find(ser->primary_key());
//     //     npc_equip.erase(s);
//     //     ser++;
//     // }
//     user_partys user_party_table(_self, fl->owner.value);
//     auto party = user_party_table.begin();
//     user_party_table.modify(party, _self, [&](auto &data)
//     {
//         data.state = party_state::on_wait;
//     });

//     iter++;
//     floor_index_table.erase(fl);
// }
// }

EOSIO_DISPATCH(battletest,
              //battle 
              (pvpstart)(activeturn)(stagestart)(stageexit)
              //tower
              //(chat)//(claim)(endflag)(toweropen)(towerstart)(deletetower)(simulate)(usersimul)
              )