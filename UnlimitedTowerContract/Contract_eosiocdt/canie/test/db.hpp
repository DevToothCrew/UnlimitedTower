ACTION dbinsert(std::string _table, std::string _value);
ACTION dberase(std::string _table, std::string _value);
ACTION dblistinsert(std::string _list, std::string _primary_key, std::vector<std::string> _value_list);
ACTION dbinit(std::string _table);
ACTION insertequipr(uint64_t _main, std::vector<uint64_t>&_upgrade_ratio, uint64_t _material_id , std::vector<uint64_t>&_material_count , std::vector<uint64_t>&_use_UTG );

ACTION setdata(eosio::name _contract, eosio::name _user, std::string _table);


/////////////////////////////////insert//////////////////////////////////////////////////////

void insert_job(std::string _status, uint64_t _job, uint64_t _min, uint64_t _max);
void insert_head(uint64_t _appear);
void insert_hair(uint64_t _appear);
void insert_body(uint64_t _appear);
void insert_gender(uint64_t _appear);
void insert_servant(uint64_t _servant_id, uint32_t _job, uint32_t _body, uint32_t _gender, uint32_t _head, uint32_t _hair, uint32_t _grade);

void insert_monster_grade(std::string _status, uint64_t _grade, uint64_t _min, uint64_t _max);

void insert_all_item_id(uint64_t _item_id, uint32_t _type, uint32_t _grade, uint64_t _max_count, uint64_t _sell_item_id, uint64_t _sell_item_count);
void insert_item_grade(std::string _status, uint64_t _grade, uint64_t _min, uint64_t _max);
void insert_grade_ratio(uint64_t _grade, uint64_t _ratio);
void insert_upgrade_monster_ratio(uint32_t _main, uint64_t _upgrade_price_count);
void insert_servant_burn_item(uint64_t _servant_job, uint64_t _result_item_id);

void insert_status_monster_up(uint64_t _type, uint64_t _first, uint64_t _second);
void insert_itemshop(uint64_t _id, uint64_t _shop_type, uint64_t _product_id, uint64_t _product_count,
                     uint64_t _product_limit_max, uint64_t _price_id, uint64_t _price_count);
void insert_shoplist(uint64_t _id, uint64_t _shop_type, uint64_t _shop_item_id, uint64_t _limit_count);
void insert_packageshop(uint64_t _id, uint64_t _get_utg, uint64_t _limit_count, uint64_t _price_id, uint64_t _price_count);

void insert_level(uint32_t _level, uint32_t _rank_exp, uint32_t _char_exp);
void insert_passive(uint64_t _passive_id, uint32_t _passive_type, uint32_t _job_class, uint32_t _enable_stack_max, uint32_t _effect_id,
                    uint32_t _effect_type_id, uint32_t _effect_value_a, uint32_t _effect_value_add_b, uint32_t _target_id);
void insert_active(uint64_t _active_id, uint32_t _job, uint32_t _tribe, uint32_t _active_per,
                   uint32_t _skill_type, uint32_t _active_turn, uint32_t _attack_type, uint32_t _dmg_type, uint32_t _elemental_type, uint32_t _target, uint32_t _target_count,
                   uint32_t _hit_count, uint32_t _atk_per, uint32_t _atk_per_add, uint32_t _atk_per_2, uint32_t _atk_per_add_2, uint32_t _heal_per, uint32_t _heal_per_add, uint32_t _option_id);
void insert_gacha_pool(uint64_t _gacha_id, uint64_t _db_index);
void insert_pre_gacha_pool(uint64_t _gacha_id, uint64_t _db_index);
void insert_gold_gacha_pool(uint64_t _gacha_id, uint64_t _db_index);
void insert_equip_item_id(uint64_t _item_id,
                          uint64_t _item_set_id,
                          uint64_t _type,
                          uint64_t _tier,
                          uint64_t _job,
                          uint64_t _option);
void insert_monster(uint64_t _monster_id, uint64_t _tribe, uint64_t _type);
void insert_stage_info(std::vector<uint32_t> _stage_info);
void insert_enemy_info(std::vector<uint32_t> _enemy_info);
void insert_enemy_stat(std::vector<uint32_t> _enemy_stat);
void insert_new_reward(std::vector<uint32_t> _reward);
void insert_status_servant(uint64_t _grade, std::vector<uint32_t> _status_list);
void insert_status_monster(uint64_t _grade, std::vector<uint32_t> _status_list);
void insert_status_equipment(uint64_t _grade, std::vector<uint32_t> _status_list);
void insert_stage_enemy(uint64_t _id,
                        uint32_t _grade,
                        uint32_t _enemy_class,
                        uint32_t _type,
                        uint32_t _enemy_str,
                        uint32_t _enemy_dex,
                        uint32_t _enemy_int,
                        uint32_t _cri_per,
                        uint32_t _cri_dmg,
                        uint32_t _speed,
                        uint32_t _avoid);

void insert_tribe_stat(uint64_t _id,
                       uint32_t _base_str,
                       uint32_t _base_dex,
                       uint32_t _base_int,
                       uint32_t _speed,
                       uint32_t _avoid,
                       uint32_t _cri_per,
                       uint32_t _cri_dmg,
                       uint32_t _pair,
                       uint32_t _triple,
                       uint32_t _penta);

void insert_job_stat(uint64_t _id,
                     uint32_t _job,
                     uint32_t _grade,
                     uint32_t _base_str,
                     uint32_t _base_dex,
                     uint32_t _base_int,
                     uint32_t _speed,
                     uint32_t _avoid,
                     uint32_t _cri_per,
                     uint32_t _cri_dmg);
void insert_type(uint64_t _id, uint32_t _strong, uint32_t _weak, uint32_t _strong_per, uint32_t _weak_per, uint32_t _pair, uint32_t _triple, uint32_t _penta);
void insert_monster_upgrade(uint64_t _upgrade, uint32_t _common, uint32_t _uncommon, uint32_t _rare, uint32_t _unique, uint32_t _legendary);
void insert_equipment_upgrade(uint64_t _upgrade, uint32_t _common, uint32_t _uncommon, uint32_t _rare, uint32_t _unique, uint32_t _legendary);



//////////////////////////////////////////////////////erase/////////////////////////////////////

void erase_job(uint64_t _job);
//  void erase_head(uint64_t _appear);
//  void erase_hair(uint64_t _appear);
//  void erase_body(uint64_t _appear);
//  void erase_gender(uint64_t _appear);
void erase_servant_id(uint64_t _id);
void erase_monster_id(uint64_t _id);
//  void erase_monster_grade(uint64_t _grade);
//  void erase_equip_item_id(uint64_t _id);
//  void erase_all_item_id(uint64_t _id);
//  void erase_item_grade(uint64_t _grade);
//   void erase_grade_ratio(uint64_t _grade);
// void erase_upgrade_monster_ratio(uint32_t _main);
// void erase_level(uint32_t _id);
// void erase_servant_lv(uint64_t _job);
// void erase_monster_lv(uint64_t _monster_class_grade);
// void erase_servant_lv_status(uint64_t _type);
// void erase_servant_lv_status_list(uint64_t _type);
// void erase_monster_lv_status(uint64_t _type);
// void erase_monster_lv_status_list(uint64_t _type);
// void erase_equipment_lv_status(uint64_t _type);
// void erase_equipment_lv_status_list(uint64_t _type);
// void erase_passive(uint64_t _id);
// void erase_active(uint64_t _id);
void erase_gacha_pool(uint64_t _id);
void erase_pre_gacha_pool(uint64_t _id);
// void erase_status_monster_up(uint64_t _id);
//void erase_itemshop(uint64_t _id);
// void erase_itemshop2(uint64_t _id);
void erase_stage_enemy(uint64_t _id);
void erase_class_state(uint64_t _id);
void erase_job_state(uint64_t _id);


    void insert_status_servant(uint64_t _grade, std::vector<uint32_t> _status_list);


    void insert_status_monster(uint64_t _grade, std::vector<uint32_t> _status_list);
	


    void insert_status_equipment(uint64_t _grade, std::vector<uint32_t> _status_list);


    void insert_tribe_stat(uint64_t _id,
                            uint32_t _base_str,
                            uint32_t _base_dex,
                            uint32_t _base_int,
                           uint32_t _speed,
                           uint32_t _avoid,
                           uint32_t _cri_per,
                           uint32_t _cri_dmg,
                           uint32_t _pair,
                           uint32_t _triple,
                           uint32_t _penta);


    void erase_class_state(uint64_t _id);

        void insert_job_stat(uint64_t _id,
                         uint32_t _job,
                         uint32_t _grade,
                         uint32_t _base_str,
                         uint32_t _base_dex,
                         uint32_t _base_int,
                         uint32_t _speed,
                         uint32_t _avoid,
                         uint32_t _cri_per,
                         uint32_t _cri_dmg);


    void erase_job_state(uint64_t _id);


    //ACTION dbinsert(std::string _table, std::string _value);
    //ACTION dberase(std::string _table, std::string _value);
    //ACTION dblistinsert(std::string _list, std::string _primary_key, std::vector<std::string> _value_list);
    //ACTION dbinit(std::string _table);
    // ACTION insertequipr(uint64_t _main, std::vector<uint64_t>&_upgrade_ratio, uint64_t _material_id , std::vector<uint64_t>&_material_count , std::vector<uint64_t>&_use_UTG );

	//ACTION setdata(eosio::name _contract, eosio::name _user, std::string _table);
    void insert_job(std::string _status, uint64_t _job, uint64_t _min, uint64_t _max);
    void insert_head(uint64_t _appear);
    void insert_hair(uint64_t _appear);
    void insert_body(uint64_t _appear);
    void insert_gender(uint64_t _appear);
    void insert_servant(uint64_t _servant_id, uint32_t _job, uint32_t _body, uint32_t _gender, uint32_t _head, uint32_t _hair, uint32_t _grade);
    
     void insert_all_item_id(uint64_t _item_id, uint32_t _type, uint32_t _grade, uint64_t _max_count, uint64_t _sell_item_id, uint64_t _sell_item_count);

    void insert_grade_ratio(uint64_t _grade, uint64_t _ratio);
    void insert_upgrade_monster_ratio(uint32_t _main, uint64_t _upgrade_price_count);
    void insert_servant_burn_item(uint64_t _servant_job, uint64_t _result_item_id);

    void insert_status_monster_up(uint64_t _type, uint64_t _first, uint64_t _second);
    void insert_itemshop(uint64_t _id, uint64_t _shop_type, uint64_t _product_id, uint64_t _product_count,
                         uint64_t _product_limit_max, uint64_t _price_id, uint64_t _price_count);
    void insert_shoplist(uint64_t _id, uint64_t _shop_type, uint64_t _shop_item_id, uint64_t _limit_count);
    void insert_packageshop(uint64_t _id, uint64_t _get_utg, uint64_t _limit_count, uint64_t _price_id, uint64_t _price_count);

    void insert_level(uint32_t _level, uint32_t _rank_exp, uint32_t _char_exp);
    void insert_passive(uint64_t _passive_id, uint32_t _passive_type, uint32_t _job_class, uint32_t _enable_stack_max, uint32_t _effect_id,
                        uint32_t _effect_type_id, uint32_t _effect_value_a, uint32_t _effect_value_add_b, uint32_t _target_id);
    void insert_active(uint64_t _active_id, uint32_t _job, uint32_t _tribe, uint32_t _active_per,
                       uint32_t _skill_type, uint32_t _active_turn, uint32_t _attack_type, uint32_t _dmg_type, uint32_t _elemental_type, uint32_t _target, uint32_t _target_count,
                       uint32_t _hit_count, uint32_t _atk_per, uint32_t _atk_per_add, uint32_t _atk_per_2, uint32_t _atk_per_add_2, uint32_t _heal_per, uint32_t _heal_per_add, uint32_t _option_id);
    void insert_gacha_pool(uint64_t _gacha_id, uint64_t _db_index);
    void insert_pre_gacha_pool(uint64_t _gacha_id, uint64_t _db_index);
    void insert_gold_gacha_pool(uint64_t _gacha_id, uint64_t _db_index);
    void insert_equip_item_id(uint64_t _item_id,
                              uint64_t _item_set_id,
                              uint64_t _type,
                              uint64_t _tier,
                              uint64_t _job,
                              uint64_t _option);
    void insert_monster(uint64_t _monster_id, uint64_t _tribe, uint64_t _type);

    //void erase_job(uint64_t _job);
    //  void erase_head(uint64_t _appear);
    //  void erase_hair(uint64_t _appear);
    //  void erase_body(uint64_t _appear);
    //  void erase_gender(uint64_t _appear);
    void erase_servant_id(uint64_t _id);
    void erase_monster_id(uint64_t _id);
  //  void erase_monster_grade(uint64_t _grade);
  //  void erase_equip_item_id(uint64_t _id);
  //  void erase_all_item_id(uint64_t _id);
  //  void erase_item_grade(uint64_t _grade);
 //   void erase_grade_ratio(uint64_t _grade);
    // void erase_upgrade_monster_ratio(uint32_t _main);
    // void erase_level(uint32_t _id);
    // void erase_servant_lv(uint64_t _job);
    // void erase_monster_lv(uint64_t _monster_class_grade);
    // void erase_servant_lv_status(uint64_t _type);
    // void erase_servant_lv_status_list(uint64_t _type);
    // void erase_monster_lv_status(uint64_t _type);
    // void erase_monster_lv_status_list(uint64_t _type);
    // void erase_equipment_lv_status(uint64_t _type);
    // void erase_equipment_lv_status_list(uint64_t _type);
	// void erase_passive(uint64_t _id);
    // void erase_active(uint64_t _id);
    void erase_gacha_pool(uint64_t _id);
    void erase_pre_gacha_pool(uint64_t _id);
    // void erase_status_monster_up(uint64_t _id);
     //void erase_itemshop(uint64_t _id);
    // void erase_itemshop2(uint64_t _id);

void insert_stage_info(std::vector<uint32_t> _stage_info);
void insert_enemy_info(std::vector<uint32_t> _enemy_info);
void insert_enemy_stat(std::vector<uint32_t> _enemy_stat);
void insert_new_reward(std::vector<uint32_t> _reward);

