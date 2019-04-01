#include "Common/common_header.hpp"

//즐겨찾기
//테이블
//db_table, token_table, gacha_table, login_table, blacklist_table
//시스템
//db_system, token_system, login_system, gacha_system, init_function, delete_function, preregist_function ,blacklist_system
//로그
//preregist_log_table, user_log_table
//컨트랙트 계정용
//owner_system

CONTRACT battletest : public contract
{
  public:
#pragma resgion contract constructor
    //using contract::contract;
    eosio::name owner;
    permission_level owner_auth;

    battletest(eosio::name _self, eosio::name _code, datastream<const char *> ds) : contract(_self, _code, ds)
    {
        owner = _self;
        owner_auth.actor = _self;
        owner_auth.permission = "owner"_n;
    }
#pragma endregion
    enum grade_list
    {
        ser = 0,
        legenary = 1,
        unique,
        rare,
        uncommon,
        common = 5,
    };
    enum status_grade_list
    {
        common_d = 0,
        plus_d,
        common_c,
        plus_c,
        common_b,
        plus_b,
        common_a,
        plus_a,
        common_s,
        plus_s = 9,
    };
    enum monster_main_status
    {
        main_str = 1,
        main_dex = 2,
        main_int = 3,
    };

    std::vector<uint32_t> servant_status_list = {10, 12, 15, 18, 22, 25, 30, 35, 50, 70};

    std::vector<uint32_t> monster_common_status_list = {25, 27, 30, 32, 35, 37, 40, 42, 45, 47};
    std::vector<uint32_t> monster_uncommon_status_list = {50, 52, 55, 57, 60, 62, 65, 67, 70, 72};
    std::vector<uint32_t> monster_rare_status_list = {75, 77, 80, 82, 85, 87, 90, 92, 95, 97};
    std::vector<uint32_t> monster_unique_status_list = {100, 105, 110, 115, 120, 125, 130, 135, 140, 145};
    std::vector<uint32_t> monster_legendary_status_list = {150, 160, 170, 180, 190, 200, 210, 220, 230, 240};

    std::vector<uint32_t> warrior_level_up = {9, 4, 2};
    std::vector<uint32_t> thief_level_up = {4, 9, 2};
    std::vector<uint32_t> cleric_level_up = {5, 3, 7};
    std::vector<uint32_t> archer_level_up = {4, 7, 4};
    std::vector<uint32_t> magician_level_up = {3, 3, 9};

    std::vector<uint32_t> common_str = {6, 2, 2};
    std::vector<uint32_t> common_dex = {2, 6, 2};
    std::vector<uint32_t> common_int = {2, 2, 6};

    std::vector<uint32_t> uncommon_str = {9, 3, 3};
    std::vector<uint32_t> uncommon_dex = {3, 9, 3};
    std::vector<uint32_t> uncommon_int = {3, 3, 9};

    std::vector<uint32_t> rare_str = {12, 5, 5};
    std::vector<uint32_t> rare_dex = {5, 12, 5};
    std::vector<uint32_t> rare_int = {5, 5, 12};

    std::vector<uint32_t> unique_str = {15, 7, 7};
    std::vector<uint32_t> unique_dex = {7, 15, 7};
    std::vector<uint32_t> unique_int = {7, 7, 15};

    std::vector<uint32_t> legenary_str = {24, 9, 9};
    std::vector<uint32_t> legenary_dex = {9, 24, 9};
    std::vector<uint32_t> legenary_int = {9, 9, 24};

    //------------------------------------------------------------------------//
    //-----------------------------db_table-----------------------------------//
    //------------------------------------------------------------------------//
  private:
    struct object_status
    {
        uint64_t base_str;
        uint64_t base_dex;
        uint64_t base_int;
    };

    TABLE dbbody
    {
        uint64_t body;
        uint64_t primary_key() const { return body; }
    };
    typedef eosio::multi_index<"dbbody"_n, dbbody> body_db;

    TABLE dbhair
    {
        uint64_t hair;
        uint64_t primary_key() const { return hair; }
    };
    typedef eosio::multi_index<"dbhair"_n, dbhair> hair_db;

    TABLE dbhead
    {
        uint64_t head;
        uint64_t primary_key() const { return head; }
    };
    typedef eosio::multi_index<"dbhead"_n, dbhead> head_db;

    TABLE dbgender
    {
        uint64_t gender;
        uint64_t primary_key() const { return gender; }
    };
    typedef eosio::multi_index<"dbgender"_n, dbgender> gender_db;

    TABLE dbitemgrade
    {
        uint64_t grade;
        object_status min_range;
        object_status max_range;
        uint64_t primary_key() const { return grade; }
    };
    typedef eosio::multi_index<"dbitemgrade"_n, dbitemgrade> item_grade_db;

    TABLE dbmonstergd
    {
        uint64_t grade;
        object_status min_range;
        object_status max_range;
        uint64_t primary_key() const { return grade; }
    };
    typedef eosio::multi_index<"dbmonstergd"_n, dbmonstergd> monster_grade_db;

    TABLE dbservantjob
    {
        uint64_t job;
        object_status min_range;
        object_status max_range;
        uint64_t primary_key() const { return job; }
    };
    typedef eosio::multi_index<"dbservantjob"_n, dbservantjob> servant_job_db;

    TABLE dbgraderatio
    {
        uint64_t grade;
        uint64_t ratio;
        uint64_t primary_key() const { return grade; }
    };
    typedef eosio::multi_index<"dbgraderatio"_n, dbgraderatio> grade_ratio_db;

    struct upgrade_monster_sub
    {
        uint32_t sub_monster;
        uint64_t ratio;
    };
    TABLE dbmonsterup
    {
        uint64_t main_monster;
        std::vector<upgrade_monster_sub> sub;
        uint64_t primary_key() const { return main_monster; }
    };
    typedef eosio::multi_index<"dbmonsterup"_n, dbmonsterup> upgrade_monster_ratio_db;

    TABLE dbequipupg
    {
        uint64_t equipment_type_grade; 
        std::vector<uint64_t> upgrade_ratio;
        uint64_t material_id;
        std::vector<uint64_t> material_count;
        std::vector<uint64_t> use_UTG;
        uint64_t primary_key() const { return equipment_type_grade; }
    };
    typedef eosio::multi_index<"dbequipupg"_n, dbequipupg> upgrade_equipment_ratio_dbs;

    TABLE dbequipup
    {
        uint64_t equipment_type_grade_upgrade; 
        uint64_t upgrade_ratio;
        uint64_t material_id;
        uint64_t material_count;
        uint64_t use_UTG;

        uint64_t primary_key() const { return equipment_type_grade_upgrade; }
    };
    typedef eosio::multi_index<"dbequipup"_n, dbequipup> upgrade_equipment_ratio_db;

    TABLE dblevel
    {
        uint32_t lv;
        uint64_t exp;
        uint64_t primary_key() const { return lv; }
    };
    typedef eosio::multi_index<"dblevel"_n, dblevel> lv_exp;

    TABLE dbservantlv
    {
        uint64_t job;
        uint64_t lvup_str;
        uint64_t lvup_dex;
        uint64_t lvup_int;
        uint64_t primary_key() const { return job; }
    };
    typedef eosio::multi_index<"dbservantlv"_n, dbservantlv> servant_lv_db;

    TABLE dbmonsterlv
    {
        uint64_t monster_class_grade;
        uint64_t lvup_str;
        uint64_t lvup_dex;
        uint64_t lvup_int;
        uint64_t primary_key() const { return monster_class_grade; }
    };
    typedef eosio::multi_index<"dbmonsterlv"_n, dbmonsterlv> monster_lv_db;

  struct lv_status_sub
    {
        uint64_t pre_status;
        uint64_t update_status;
    };

    TABLE dbstatusserv
    {
        uint64_t grade;
        std::vector<lv_status_sub> lv_status;
        uint64_t primary_key() const { return grade; }
    };
    typedef eosio::multi_index<"dbstatusserv"_n,dbstatusserv> servant_lv_status_db;

    TABLE dbstatusmon
    {
        uint64_t grade;
        std::vector<lv_status_sub> lv_status;
        uint64_t primary_key() const { return grade; }
    };
    typedef eosio::multi_index<"dbstatusmon"_n, dbstatusmon> monster_lv_status_db;
	
	  TABLE dbpassive
    {
        uint64_t passive_id;
        uint32_t enable_stack;
        uint32_t max_stack;
        uint32_t effect_type;
        uint32_t effect_value;
        uint32_t effect_value_add;
        uint32_t target;
        uint32_t role_target;
        uint64_t primary_key() const {return passive_id;}        
    };
    typedef eosio::multi_index<"dbpassive"_n, dbpassive> passive_db;

    TABLE dbactive
    {
        uint64_t active_id = 0;
        uint32_t job = 0;
        uint32_t active_per = 0;
        uint32_t skill_type = 0;
        uint32_t attack_type = 0;
        uint32_t dmg_type = 0;
        uint32_t target = 0;
        uint32_t target_count = 0;
        uint32_t target_range = 0;
        uint32_t hit_count = 0;
        uint32_t atk_per = 0;
        uint32_t atk_per_add = 0;
        uint32_t heal_per = 0;
        uint32_t heal_per_add = 0;
        std::vector<uint32_t> array_option_id_list;
        std::vector<uint32_t> array_buff_id_list;
        uint64_t primary_key() const {return active_id;}    
    };
    typedef eosio::multi_index<"dbactive"_n, dbactive> active_db;
	

/////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////NEW DB ///////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
#pragma region db dbmonster


    TABLE dbmonster
    {
        uint64_t id;
        uint64_t tribe;      //종족
        uint64_t type;       //수풍지화 암명
        uint64_t monster_class; //몬스터 타입 클래스
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbmonsters"_n, dbmonster> monster_db;

    TABLE startmonster
    {
        uint64_t id;
        uint64_t type;
        uint64_t main_status;
        uint64_t primary_key() const { return id; }     
    };
    typedef eosio::multi_index<"startmonster"_n, startmonster> start_monster;
#pragma endregion

#pragma region db dbitemcommon
    TABLE dballitem
    {
        uint64_t id;
        uint32_t type;
        std::vector<uint32_t>  item_param_list;
        uint64_t sell_id;
        uint64_t sell_cost;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dballitem"_n, dballitem> allitem_db;
#pragma endregion

#pragma region db dbitemequip
    TABLE dbequipment
    {
        uint64_t item_id;
        uint64_t set_id;        //셋트 아이템일 경우를 대비
        uint64_t type;
        uint64_t tier;
        uint64_t job;
        std::vector<uint32_t> option_list;
        uint32_t option_value_min;
        uint32_t option_value_max;
        uint32_t upgrade_option_value;
        std::vector<uint64_t> random_option_id_list;
        std::vector<uint32_t> grade_multi_list;

        uint64_t primary_key() const { return item_id; }
    };
    typedef eosio::multi_index<"dbequipment"_n, dbequipment> equipment_db;
#pragma endregion

#pragma region db dbgrinditem
    TABLE dbgrind
    {
        uint64_t item_id;
        uint64_t servant_job;
        uint64_t primary_key() const { return servant_job; }
    };
    typedef eosio::multi_index<"dbgrind"_n, dbgrind> grinditem_db;
#pragma endregion


#pragma region db dbservant
    TABLE dbservant
    {
        uint64_t id;
        uint32_t job;
        uint32_t body;
        uint32_t gender;
        uint32_t head;
        uint32_t hair;
        uint32_t grade;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbservants"_n, dbservant> servant_db;
#pragma endregion 

   TABLE dbgachapool
   {
       uint64_t gacha_id;
       uint64_t db_index;
       uint64_t primary_key() const { return gacha_id; }
   };
   typedef eosio::multi_index<"dbgachapool"_n, dbgachapool> main_gacha_db;

    //servant_db servant_db_table(_self, _self.value);
    //auto servant_db_iter = servant_db_table.get_index<"second"_n>();   //샘플

    //------------------------------------------------------------------------//
    //----------------------------db_system-----------------------------------//
    //------------------------------------------------------------------------//
    TABLE dbstageenemy
    {
        uint64_t id;
        uint32_t grade;
        uint32_t enemy_class;
        uint32_t type;
        uint32_t enemy_str;
        uint32_t enemy_dex;
        uint32_t enemy_int;
        uint32_t physical_cri_per;
        uint32_t physical_cri_dmg;
        uint32_t magic_cri_per;
        uint32_t magic_cri_dmg;
        uint32_t speed;
        uint32_t avoid;
        std::vector<uint32_t> active_list;
        std::vector<uint32_t> passive_list;

        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbstageenemy"_n, dbstageenemy> stage_enemy_db;

    void insert_stage_enemy(uint64_t _id,
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
                            uint32_t _passive);

    void erase_stage_enemy(uint64_t _id);


    TABLE dbclassstat
    {
        uint64_t id;
        uint32_t speed;
        uint32_t avoid;
        uint32_t physical_cri_per;
        uint32_t physical_cri_dmg;
        uint32_t magic_cri_per;
        uint32_t magic_cri_dmg;
        std::vector<uint32_t> passive_private_list;
        uint32_t passive_private_per;
        std::vector<uint32_t> passive_public_list;
        std::vector<uint32_t> active_private_list;
        uint32_t active_private_per;
        std::vector<uint32_t> active_public_list;

        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbclassstat"_n, dbclassstat> class_stat_db;

    void insert_class_stat(uint64_t _id,
                           uint32_t _speed,
                           uint32_t _avoid,
                           uint32_t _physical_cri_per,
                           uint32_t _physical_cri_dmg,
                           uint32_t _magic_cri_per,
                           uint32_t _magic_cri_dmg);
                           
    void insert_class_stat_passive(uint64_t _claas, uint64_t _private_id,
                                   uint32_t _per,
                                   uint32_t _public_id);
    void insert_class_stat_active(uint64_t _claas, uint64_t _private_id,
                                  uint32_t _per,
                                  uint32_t _public_id);

    void erase_class_state(uint64_t _id);

    TABLE dbjobstat
    {
        uint64_t id;
        uint32_t speed;
        uint32_t avoid;
        uint32_t physical_cri_per;
        uint32_t physical_cri_dmg;
        uint32_t magic_cri_per;
        uint32_t magic_cri_dmg;
        std::vector<uint32_t> passive_private_list;
        uint32_t passive_private_per;
        std::vector<uint32_t> passive_public_list;
        std::vector<uint32_t> active_private_list;
        uint32_t active_private_per;
        std::vector<uint32_t> active_public_list;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbjobstat"_n, dbjobstat> job_stat_db;

    void insert_job_stat(uint64_t _id,
                         uint32_t _speed,
                         uint32_t _avoid,
                         uint32_t _physical_cri_per,
                         uint32_t _physical_cri_dmg,
                         uint32_t _magic_cri_per,
                         uint32_t _magic_cri_dmg);
    void insert_job_stat_passive(uint64_t _job, uint64_t _private_id,
                                 uint32_t _per,
                                 uint32_t _public_id);

    void insert_job_stat_active(uint64_t _job, uint64_t _private_id,
                                uint32_t _per,
                                uint32_t _public_id);

    void erase_job_state(uint64_t _id);

  public:
#pragma region master
    enum system_state
    {
        normal = 0,
        pause,
    };

    TABLE systemmaster
    {
        eosio::name master;
        uint64_t state;
        uint64_t primary_key() const { return master.value; }
    };
    typedef eosio::multi_index<"systemmaster"_n, systemmaster> system_master;

    ACTION setmaster(eosio::name _master);
    ACTION initmaster();
#pragma endregion


#pragma region db function
  public:
    void substr_value(std::string _value, std::vector<std::string> & _value_list, std::vector<size_t> & _size_list, uint32_t _size);
    ACTION dbinsert(std::string _table, std::string _value);
    ACTION dberase(std::string _table, std::string _value);
    ACTION dbinit(std::string _table);
    ACTION setdata(eosio::name _contract, std::string _table);

    void insert_job(std::string _status, uint64_t _job, uint64_t _min, uint64_t _max);
    void insert_head(uint64_t _appear);
    void insert_hair(uint64_t _appear);
    void insert_body(uint64_t _appear);
    void insert_gender(uint64_t _appear);
    void insert_servant(uint64_t _servant_id, uint32_t _job, uint32_t _body, uint32_t _gender, uint32_t _head, uint32_t _hair, uint32_t _grade);
    void insert_monster(uint64_t _monster_id, uint64_t _tribe, uint64_t _type, uint64_t _monster_class);
    void insert_monster_grade(std::string _status, uint64_t _grade, uint64_t _min, uint64_t _max);
    void insert_equip_item_id(uint64_t _item_id,
                              uint64_t _item_set_id,
                              uint64_t _tier,
                              uint64_t _type,
                              uint64_t _job,
                              uint64_t _option,
                              int32_t _option_value_min,
                              uint32_t _option_value_max,
                              uint32_t _upgrade_option_value,
                              uint64_t _random_option_id,
                              uint32_t _grade_multi);
    void insert_all_item_id(uint64_t _item_id, uint32_t _type, uint32_t _param, uint64_t _sell_id, uint64_t _sell_cost);
    void insert_item_grade(std::string _status, uint64_t _grade, uint64_t _min, uint64_t _max);
    void insert_grade_ratio(uint64_t _grade, uint64_t _ratio);

  






	void insert_passive(uint64_t _id, uint32_t _enable_stack, uint32_t _max_stack,
    uint32_t _effect_type, uint32_t _effect_value, uint32_t _effect_value_add, uint32_t _target, uint32_t _role_target);
    void insert_active(uint64_t _id, uint32_t _job, uint32_t _active_per,
                               uint32_t _skill_type, uint32_t _attack_type, uint32_t _dmg_type,uint32_t _target, uint32_t _target_count, uint32_t _target_range,
                               uint32_t _hit_count, uint32_t _atk_per, uint32_t _atk_per_add, uint32_t _heal_per, uint32_t _heal_per_add);
    void insert_gacha_pool(uint64_t _gacha_id, uint64_t _db_index);



    void erase_job(uint64_t _job);
    void erase_head(uint64_t _appear);
    void erase_hair(uint64_t _appear);
    void erase_body(uint64_t _appear);
    void erase_gender(uint64_t _appear);
    void erase_servant_id(uint64_t _id);
    void erase_monster_id(uint64_t _id);
    void erase_monster_grade(uint64_t _grade);
    void erase_equip_item_id(uint64_t _id);
    void erase_common_item_id(uint64_t _id);
    void erase_item_grade(uint64_t _grade);
    void erase_grade_ratio(uint64_t _grade);
    // void erase_upgrade_monster_ratio(uint32_t _main);
    // void erase_servant_grind_item(uint32_t _item_id);
    // void erase_level(uint32_t _id);
    // void erase_servant_lv(uint64_t _job);
    // void erase_monster_lv(uint64_t _monster_class_grade);
    // void erase_servant_lv_status(uint64_t _type);
    // void erase_monster_lv_status(uint64_t _type);
	void erase_passive(uint64_t _id);
    void erase_active(uint64_t _id);
    void erase_gacha_pool(uint64_t _id);
#pragma endregion

#pragma region stage

    struct stage_monster_info
    {
        uint64_t monster_id;
        uint64_t position;
    };


    TABLE dbstage
    {
        uint64_t stage_id;
        uint32_t stage_type;
        uint32_t stage_floor;
        uint32_t need_stage_id;
        uint32_t stage_group_index;
        uint32_t need_entrance_item_id;
        uint32_t need_entrance_item_count;
        uint32_t enemy_level_min;
        uint32_t enemy_level_max;
        std::vector<stage_monster_info> monster_list;
        uint32_t boss_level;
        std::vector<uint32_t> boss_passive_list;
        std::vector<uint32_t> boss_skill_list;

        uint64_t primary_key() const { return stage_id; }
    };
    typedef eosio::multi_index<"dbstage"_n, dbstage> stage_db;

    void insert_stage(uint64_t _stage_id,
    uint32_t _stage_type,
    uint32_t _stage_floor,
    uint32_t _need_stage_id,
    uint32_t _stage_group_index,
    uint32_t _need_entrance_item_id,
    uint32_t _need_entrance_item_count,
    uint32_t _enemy_level_min,
    uint32_t _enemy_level_max);

    void insert_stage_monster(uint64_t _stage_id ,uint32_t _monster_id, uint32_t _position);


    void erase_stage(uint64_t _id);
    void erase_stage_monster(uint64_t _id);

    struct reward_info
    {
        uint32_t type;
        uint64_t id;
        uint32_t grade;
        uint64_t per;
        uint64_t count;
    };
    TABLE dbreward
    {
        uint64_t stage_id;
        uint32_t rank_exp;
        uint32_t char_exp;
        uint32_t first_reward_type;
        uint32_t first_reward_id;
        uint32_t first_reward_grade;
        uint32_t first_reward_count;
        uint64_t reward_utg_min;
        uint64_t reward_utg_max;
        std::vector<reward_info> reward_list;
        uint64_t primary_key() const { return stage_id; }
    };
    typedef eosio::multi_index<"dbreward"_n, dbreward> reward_db;

    void insert_stage_reward(uint64_t _stage_id,
                             uint32_t _rank_exp,
                             uint32_t _char_exp,
                             uint32_t _first_reward_type,
                             uint32_t _first_reward_id,
                             uint32_t _first_reward_grade,
                             uint32_t _first_reward_count,
                             uint32_t _reward_utg_min,
                             uint32_t _reward_utg_max);
    void insert_reward(uint64_t _stage_id, uint32_t _type, uint32_t _id, uint32_t _grade, uint32_t _per, uint32_t _count);
    void erase_stage_reward(uint64_t _id);
    void erase_reward(uint64_t _id);

#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------token_table--------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region token account
  private:
    TABLE accounts
    {
        asset balance;
        uint64_t primary_key() const { return balance.symbol.code().raw(); }
    };
    typedef eosio::multi_index<"accounts"_n, accounts> account;
#pragma endregion

#pragma region token stats
    TABLE stat
    {
        asset supply;
        asset max_supply;
        eosio::name issuer;
        uint64_t primary_key() const { return supply.symbol.code().raw(); }
    };
    typedef eosio::multi_index<"stat"_n, stat> stats;
#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------token_system--------------------------------//
    //------------------------------------------------------------------------//
  public:
#pragma region token function
    void sub_balance(name user, asset value);
    void add_balance(name user, asset value, name ram_payer);
#pragma endregion

#pragma region token action
    ACTION create(name issuer, asset maximum_supply);
    ACTION issue(name to, asset quantity, string memo);
    ACTION transfer(name from, name to, asset quantity, string memo);
#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------gacha_table--------------------------------//
    //------------------------------------------------------------------------//
  private:
    //struct status_info
    struct status_info
    {
        uint32_t basic_str = 0;
        uint32_t basic_dex = 0;
        uint32_t basic_int = 0;
        uint32_t plus_str = 0;
        uint32_t plus_dex = 0;
        uint32_t plus_int = 0;
    };

#pragma region gacha tservant
    struct servant_info
    {
        uint32_t state;     //서번트 상태
        uint32_t exp = 0;   //서번트 경험치
        uint32_t stat_point = 0;
        uint64_t id = 0;
        status_info status;
        std::vector<uint32_t> equip_slot; //서번트 장비 리스트
        std::vector<uint32_t> passive_skill;
        std::vector<uint32_t> active_skill;
    };

    TABLE tservant
    {
        uint64_t index;
        uint32_t party_number = 0;
        servant_info servant;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"tservant"_n, tservant> user_servants;
#pragma endregion

#pragma region gacha tmonster
    struct monster_info
    {
        uint64_t id;          //몬스터 id 값
        uint32_t state;       //몬스터 상태값
        uint32_t exp = 0;     //경험치
        uint32_t type = 0;    //속성 타입
        uint32_t monster_class = 0; //몬스터의 클래스
        uint32_t grade;       // 등급
        uint32_t upgrade = 0; //강화수치
        status_info status;   //기본 힘,민,지 추가 힘,민,지
        std::vector<uint32_t> passive_skill;
        std::vector<uint32_t> active_skill;
    };

    TABLE tmonster
    {
        uint64_t index;
        uint32_t party_number = 0;
        monster_info monster;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"tmonster"_n, tmonster> user_monsters;

#pragma endregion

#pragma region gacha tequipment
    struct equipment_info
    {
        uint64_t id;          //아이템 리소스 아이디
        uint32_t state;       //아이템 현재 상태
        uint32_t type;        //장착 타입
        uint32_t tier;        //티어
        uint32_t job;         //직업제한
        uint32_t grade;       //아이템 등급
        uint32_t upgrade = 0; //아이템 강화 수치
        uint32_t value = 0;
        uint64_t equipservantindex = 0;
    };

    TABLE tequipment
    {
        uint64_t index;
        equipment_info equipment;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"tequipment"_n, tequipment> user_equip_items;

    TABLE titem
    {
        uint64_t id;
        uint64_t type;
        uint64_t count;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"titem"_n, titem> user_items;

#pragma endregion

#pragma region gacha tgacharesult
    enum result
    {
        servant = 1,
        monster,
        item,
    };

    struct result_info
    {
        uint64_t index;
        uint32_t type;
    };

    TABLE tgacharesult
    {
        eosio::name user;
        result_info result;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"tgacharesult"_n, tgacharesult> user_gacha_results;
#pragma endregion

#pragma region gacha ttotalgacha

    TABLE ttotalgacha
    {
        eosio::name user;
        std::vector<result_info> result_list;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"ttotalgacha"_n, ttotalgacha> user_gacha_totals;

#pragma endregion


#pragma region db dbmail
    TABLE tmaildb
    {
        uint64_t mail_index = 0;
        uint64_t type_id;
        uint64_t type;  //서번트1, 몬스터2, 아이템 3, 재화 4
        uint64_t grade; //재화 수치는 grade에 넣고 대신 이용하기 
        uint64_t job;
        uint64_t tier;
        uint32_t atk = 0;
        uint32_t def = 0;
        status_info status; 
        uint64_t primary_key() const { return mail_index; }
    };
    typedef eosio::multi_index<"tmaildb"_n, tmaildb> mail_db;

#pragma endregion



    //------------------------------------------------------------------------//
    //-----------------------------gacha_system--------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region gacha values
    const char *action_gacha = "gacha";
    const char *action_signup = "signup";
    const char *action_inventory = "buyinventory";
    uint32_t servant_random_count;
    uint32_t monster_random_count;
    uint32_t item_random_count;
#pragma endregion

  public:
#pragma region gacha function
    uint64_t get_random_grade(uint64_t _rate);
    uint32_t get_servant_index(uint32_t _job, uint32_t _body, uint32_t _gender, uint32_t _head, uint32_t _hair);
    uint32_t get_monster_passive_skill(uint32_t _monster_class, uint32_t _seed);
    uint32_t get_monster_active_skill(uint32_t _monster_class, uint32_t _seed);

    uint32_t get_servant_passive_skill(uint32_t _job, uint32_t _seed);
    uint32_t get_servant_active_skill(uint32_t _job, uint32_t _seed);

    void gacha_servant_id(eosio::name _user, uint64_t _seed);
    uint8_t gacha_servant_head(uint64_t _seed, uint32_t _count);
    uint8_t gacha_servant_hair(uint64_t _seed, uint32_t _count);
    uint8_t gacha_servant_body(uint64_t _seed, uint32_t _count);
    uint32_t chang_statue(uint32_t _status_grade);
    uint32_t chang_monster_statue(uint32_t _grade,uint32_t _status_grade);

    void gacha_monster_id(eosio::name _user, uint64_t _seed);
    void gacha_item_id(eosio::name _user, uint64_t _seed);

    uint64_t get_user_seed_value(uint64_t _user);

    void start_gacha(eosio::name _user, uint64_t _seed);

#pragma endregion

//------------------------------------------------------------------------//
//------------------------------user_log_table----------------------------//
//------------------------------------------------------------------------//
#pragma region login table prelog
    TABLE tuserlog
    {
        eosio::name user;
        uint32_t servant_num = 0;
        uint32_t monster_num = 0;
        uint32_t item_num = 0;
        uint32_t gacha_num = 0;
        uint32_t login_time = 0;
        uint32_t get_gold = 0;
        uint32_t use_gold = 0;
        uint64_t use_eos = 0;
        uint32_t battle_count = 0;
        uint32_t last_stage_num = 0;
        uint32_t last_tower_num = 0;
        uint32_t top_clear_stage = 0;
        uint32_t top_clear_tower = 0;
        uint32_t add_party_count = 0;
        uint32_t soul_powder = 0;
        uint32_t mail = 0;

        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"tuserlog"_n, tuserlog> user_logs;

#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------login_table--------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region login enum
    enum user_state
    {
        pre_regist = 1,
        lobby = 2,
        tower,
        stage,
        auth_regist,
    };

    enum object_state
    {
        on_inventory = 1,
        on_equip_slot,
        on_mail,
    };
    enum tutorial_state
    {
        start_tutorial =1,
        servant_tutorial,
        monster_tutorial,
        party_tutorial,
        battle_tutorial,
    };
#pragma endregion

#pragma region login table
    TABLE tuserauth
    {
        eosio::name user;
        uint32_t state = user_state::lobby;
        uint32_t exp = 0;
        uint32_t current_servant_inventory = 0;
        uint32_t current_monster_inventory = 0;
        uint32_t current_equipitem_inventory = 0;
        uint32_t current_item_inventory = 0;
        uint32_t servant_inventory = 50;
        uint32_t monster_inventory = 50;
        uint32_t equipment_inventory = 50;
        uint32_t item_inventory = 50;
        
        uint64_t primary_key() const { return user.value; }
    };

    typedef eosio::multi_index<"tuserauth"_n, tuserauth> user_auths;

#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------login_system-------------------------------//
    //------------------------------------------------------------------------//

#pragma region login enum
    struct transfer_action
    {
        name from;
        std::string action;
        std::string param;
        uint32_t type;
        name to;
        asset quantity;
    };
    enum job_list
    {
        beginner = 0,
        warrior,
        archer,
        wizard,
        priest,
        thief,
    };
#pragma endregion

#pragma region login values

    struct st_transfer
    {
        eosio::name from;
        eosio::name to;
        asset quantity;
        string memo;
    };

#pragma endregion

  public:
#pragma region login function

    template <typename T>
    void eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func);

#pragma endregion

#pragma region login action
    ACTION eostransfer(eosio::name sender, eosio::name receiver);
    void signup(eosio::name _user);
#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------whitelist------------------------------//
    //------------------------------------------------------------------------//

#pragma region whitelist
    TABLE twhitelist
    {
        eosio::name user;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"twhitelist"_n, twhitelist> whitelist;

    ACTION deletewhite(eosio::name _user);
    ACTION addwhite(eosio::name _user);

#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------blacklist_table---------------------------//
    //------------------------------------------------------------------------//
#pragma region blacklist table tblacklist
    TABLE tblacklist
    {
        eosio::name user;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"tblacklist"_n, tblacklist> blacklist;
#pragma endregion
    //------------------------------------------------------------------------//
    //——————————————blacklist_system—————————————//
    //————————————————————————————————————//
#pragma region blacklist action

    ACTION deleteblack(eosio::name _user);
    ACTION addblack(eosio::name _user);

#pragma endregion

    //————————————————————————————————————//
    //————————————————owner_system—————————————//
    //————————————————————————————————————//
    ACTION setpause(uint64_t _state);
    ACTION resultgacha(eosio::name _from, eosio::name _to, std::string _result);
    ACTION resultpre(eosio::name _from, eosio::name _to, std::string _result);
	ACTION battlestate(eosio::name _who, std::vector<std::string> &_my_state_list, std::vector<std::string> &_enemy_state_list);
    ACTION battleaction(eosio::name _who, std::string _header  ,std::vector<std::string> &_action_data);

    //------------------------------------------------------------------------//
    //-------------------------------party_table------------------------------//
    //------------------------------------------------------------------------//

#pragma region party_table

    enum party_state
    {
        on_wait = 1,
        on_tower_defense,
		on_stage,
    };

    TABLE tparty
    {
      public:
        uint32_t index;
        uint32_t state = party_state::on_wait;
        std::vector<uint64_t> servant_list;
        std::vector<uint64_t> monster_list;

        uint64_t primary_key() const { return index; }
    };

    typedef eosio::multi_index<"tpartylist"_n, tparty> user_partys;

#pragma endregion

    //------------------------------------------------------------------------//
    //-------------------------------party_system-----------------------------//
    //------------------------------------------------------------------------//
#pragma region party_system
  public:
    ACTION saveparty(eosio::name _user, uint32_t _party_number, const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list);
    void add_party_list(eosio::name _user);

#pragma endregion





    //------------------------------------------------------------------------//
    //---------------------- -------battle_state_table-------------------------//
    //------------------------------------------------------------------------//
#pragma region battle state table


    // 4 + 4 + 4 + 4 + 4 + 4 + 8 + sbattle_member_state(9) = 41
    // sbattle_member_state 당 9 총 5개의 버프창이 있으면 45 + 32 = 77
  private:
    const uint32_t warrior_speed = 22;
    const uint32_t wizard_speed = 10;
    const uint32_t priest_speed = 20;
    const uint32_t archer_speed = 28;
    const uint32_t thief_speed = 30;
    const uint32_t beginner_speed = 12;

  private:
    const uint32_t oper_attack = 2;
    const uint32_t oper_defense = 1;
    const uint32_t oper_critical = 5;
    const uint32_t oper_critical_damage = 15000;
    const uint32_t oper_avoid = 5;
    const uint32_t defense_constant = 2000;

    const uint32_t decimal = 100;
    const uint32_t max_battle_member_count = 20;
    const uint32_t max_party_count = 10;

  public:
    enum active_name
    {
        active_defense = 200001,
        active_bash,
        active_fast_attack,
        active_critical_strike,
        active_heal,
        active_magic_strike,
        active_multi_shot,
        active_guided_arrow,
    };

    enum stage_state
    {
        start = 1,
        win,
        lose,
    };

    enum buff_state
    {
        defense = 1,
    };

    enum target_type
    {
        self = 0,
        enemy,
        ally,
        global,
    };

    enum atk_type
    {
        buff_none = 0,
        physical_atk = 1,
        magic_atk,
        magic_physical_atk,
    };

    enum dmg_type
    {
        buff_heal_none = 0,
        physical_dfs = 1,
        magic_dfs,
    };

    struct skill_info
    {
        uint32_t id = 0;
        uint32_t per = 0;
        uint32_t attack_type = 0;
        uint32_t dmg_type = 0;
        uint32_t target = 0;
        uint32_t target_count = 0;
    };
    struct total_status
    {
        uint32_t total_str = 0;
        uint32_t total_dex = 0;
        uint32_t total_int = 0;
    };
    struct buff_info
    {
        uint32_t id = 0;
        uint32_t turn = 0;
    };

    struct character_state_data
    {
        uint32_t position;
        uint32_t index;
        uint32_t id;
        uint32_t now_hp;
        uint32_t physical_attack;
        uint32_t magic_attack;
        uint32_t physical_defense;
        uint32_t magic_defense;
        uint32_t physical_crit_per;
        uint32_t magic_crit_per;
        uint32_t physical_crit_dmg;
        uint32_t magic_crit_dmg;
        uint32_t avoid;
        uint32_t state;
        uint32_t speed;
        uint32_t type;
        uint32_t job_class;
        std::vector<buff_info> buff_list;
        std::vector<skill_info> passive_skill_list;
        std::vector<skill_info> active_skill_list;
        total_status status;
    };

    TABLE tstagestate
    {
        eosio::name user;
        uint64_t stage_type;    // Tower / Field 1~5
        eosio::name enemy_user; // if : Tower = account Name / else : untowermain1
        uint64_t stage_number;
        uint64_t turn = 0;
        std::vector<character_state_data> my_state_list;
        std::vector<character_state_data> enemy_state_list;

        uint64_t primary_key() const { return user.value; }
    };

    typedef eosio::multi_index<"tstgstate"_n, tstagestate> battle_state_list;
#pragma endregion

    //------------------------------------------------------------------------//
    //---------------------------battle_action_table--------------------------//
    //------------------------------------------------------------------------//
#pragma region battle action table
    enum battle_member_state
    {
        live = 0,
        dead,
    };

    enum action_type
    {
        attack = 2,
        skill,
    };

    struct battle_order_struct
    {
        uint32_t speed;
        uint32_t key;
        uint32_t position;
        uint32_t second_speed;
        uint32_t action = 0;
    };

    struct action_info
    {
        uint32_t target_position = 0;
        uint32_t avoid = 0;
        uint32_t critical = 0;
        uint32_t damage = 0;
    };

    struct character_action_data
    {
        uint32_t my_position = 0;
        uint32_t action_type = 0;
        std::vector<action_info> action_info_list;
    };

    TABLE tbattleact
    {
        eosio::name user;
        uint32_t turn;
        std::vector<character_action_data> character_action_list;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"tbattleact"_n, tbattleact> battle_actions;
#pragma endregion

    //------------------------------------------------------------------------//
    //---------------------------battle_reward_table--------------------------//
    //------------------------------------------------------------------------//
#pragma region battle action table
    struct servant_data
   {
       uint64_t index;
       uint32_t party_number;
       servant_info servant;
   };
   struct monster_data
   {
       uint64_t index;
       uint32_t party_number;
       monster_info monster;
   };
   struct equip_data
   {
       uint64_t index;
       equipment_info equipment;
   };
   struct item_data
   {
       uint64_t id;
       uint32_t type;
       uint64_t count;
   };
    
    
   TABLE tclearreward
   {
       eosio::name user;
       uint64_t reward_money;
       std::vector<uint32_t> get_exp_list;
       std::vector<servant_data> get_servant_list;
       std::vector<monster_data> get_monster_list;
       std::vector<equip_data> get_equipment_list;
       std::vector<item_data> get_item_list;
       uint64_t primary_key() const { return user.value; }
   };
    typedef eosio::multi_index<"tclearreward"_n, tclearreward> battle_reward_list;
#pragma endregion
    //------------------------------------------------------------------------//
    //-------------------------------battle_function--------------------------//
    //------------------------------------------------------------------------//
#pragma region battle function
    status_info get_level_up_monster_status(uint64_t _class, uint64_t _grade, status_info _status);
    status_info get_level_up_servant_status(uint64_t _job, status_info _status);
    status_info get_grade_status(uint64_t _grade, status_info _status);
    uint32_t get_max_hp(total_status _status);
    uint32_t get_magic_attack(total_status _status);
    uint32_t get_physical_attack(total_status _status);
    uint32_t get_magic_defense(total_status _status);
    uint32_t get_physical_defense(total_status _status); 
    uint32_t get_speed(uint32_t _job);
    void set_stage_state(uint64_t _stage_id, std::vector<character_state_data> &_enemy_state_list, std::vector<std::string> &_state);
    character_state_data get_user_state(eosio::name _user, std::string _type, uint64_t _index, uint32_t _position, std::vector<std::string> &_state);
    ACTION stagestart(eosio::name _user, uint32_t _party_number, uint32_t _stage_type, uint32_t _stage_floor);

    void init_buff_effect(character_state_data &_state, buff_info _buff);
    void init_buff_turn(std::vector<character_state_data> &_state_list);
    bool check_buff_state(buff_info &_buff);
    bool check_activate_skill(skill_info _skill, uint64_t _rate);
    uint64_t get_damage(uint32_t _atk, uint32_t _dfs);
    bool check_critical(uint64_t _critcal_per, uint64_t _seed);
    bool check_avoid(uint64_t _avoid_per, uint64_t _seed);
    
     //=====================skill======================//
    void set_skill_damage(uint32_t _skill_id, character_state_data &_state ,uint32_t &_attack, uint32_t &_cri_dmg);
    void set_skill_type(skill_info _skill,character_state_data &_state ,uint32_t &_attack, uint32_t &_cri_dmg, uint32_t &_cri_per, uint32_t &_defense);
    void set_dmg_type(uint32_t _dmg_type, character_state_data &_state , uint32_t &_defense);
    void set_attack_type(uint32_t _atk_type, character_state_data &_state ,uint32_t &_attack, uint32_t &_cri_dmg, uint32_t &_cri_per);
    //================================================//
    bool set_action(uint32_t _action,uint64_t _seed,
                                                      std::vector<character_state_data> &_my_state_list,
                                                      std::vector<character_state_data> &_enemy_state_list,
                                                      uint64_t _my_key, character_action_data &_action_info,
                                                      std::vector<std::string> &_data);
    action_info get_target_action(uint32_t _actvie_id , std::vector<character_state_data> &_my_state_list, std::vector<character_state_data> &_enemy_state_list, uint64_t _seed, uint64_t _my_key, uint64_t _target_key);
    int get_random_target(const std::vector<character_state_data> &_enemy_state_list, uint64_t _seed, uint32_t _max, uint32_t _min);
    int get_target_key(const std::vector<character_state_data> &_enemy_state_list, uint64_t _target_position);
    static bool sort_compare(const battle_order_struct &a, const battle_order_struct &b);
    bool check_level_up(uint64_t _cur_exp, uint64_t _pre_exp);

    servant_data get_reward_servant(eosio::name _user, uint32_t _job, uint32_t _grade, uint64_t _seed);
    monster_data get_reward_monster(eosio::name _user, uint32_t _id, uint32_t _grade, uint64_t _seed);
    equip_data get_reward_equip(eosio::name _user, uint32_t _id, uint32_t _grade ,uint64_t _seed);
    void win_reward(eosio::name _user,uint64_t _stage_number);
    void fail_reward(eosio::name _user,uint64_t _stage_number);
    ACTION activeturn(eosio::name _user, uint32_t _turn, std::string _seed);

    ACTION stageexit(eosio::name _user);

#pragma endregion

#pragma region tower_system

    TABLE teoslog
    {
        eosio::name user;
        uint64_t signup_eos = 0;
        uint64_t status_change_num = 0;
        uint64_t gacha_eos = 0;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"teoslog"_n, teoslog> eos_logs;

    TABLE tsnapshot
    {
        uint64_t snapshot_count = 0;
        uint64_t signup_eos = 0;
        uint64_t status_change_num = 0;
        uint64_t gacha_eos = 0;
        uint64_t total_eos = 0;
        uint64_t primary_key() const { return snapshot_count; }
    };
    typedef eosio::multi_index<"tsnapshot"_n, tsnapshot> eos_snapshots;

    ACTION towersnap();
    ACTION claim(eosio::name _user, uint64_t _snapcount);
    ACTION settower(eosio::name _loser, eosio::name _winner, uint64_t _loser_party_num, uint64_t _winner_party_num);

#pragma endregion

    //테스트용 함수
    ACTION testsnap(eosio::name _user);

    void cheat_servant(eosio::name _user, uint64_t _seed);
    void cheat_monster(eosio::name _user, uint64_t _seed);
    ACTION partycheat(eosio::name _user);
    ACTION herocheat(eosio::name _user);
    
    //ACTION change(eosio::name _user, std::string _kind, uint64_t _grade, uint64_t _id, std::string _status);

    ACTION deletebattle(eosio::name _user);
    ACTION deleteuser(eosio::name _user);

    //-------------------------------------------------------------------------------//
    //-----------------------------preregist_table-----------------------------------//
    //-------------------------------------------------------------------------------//

#pragma region preregist
    ACTION movedb(eosio::name _user);


    TABLE dbservantid
    {
        uint64_t index;
        uint64_t id;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"dbservantid"_n, dbservantid> servant_id_db;

    TABLE dbmonsterid
    {
        uint64_t id;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbmonsterid"_n, dbmonsterid> monster_id_db;

    TABLE dbitemid
    {
        uint64_t id;
        uint64_t type;
        uint64_t job;
        uint64_t tier;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbitemid"_n, dbitemid> item_id_db;

    TABLE preauth
    {
        eosio::name user;
        uint32_t state = user_state::lobby;
        uint64_t primary_key() const { return user.value; }
    };

    typedef eosio::multi_index<"preauth"_n, preauth> pre_users;

    TABLE prelog
    {
        eosio::name user;
        uint32_t servant_num = 0;
        uint32_t monster_num = 0;
        uint32_t item_num = 0;
        uint32_t gacha_num = 0;
        uint64_t use_eos = 0;

        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"prelog"_n, prelog> pre_logs;

    TABLE preservant
    {
        uint64_t index;
        uint32_t id;
        status_info status;
        uint64_t primary_key() const { return index; }
    };

    typedef eosio::multi_index<"preservant"_n, preservant> user_preregist_servants;

    TABLE premonster
    {
        uint64_t index;
        uint32_t id;
        uint32_t grade;
        status_info status;
        uint64_t primary_key() const { return index; }
    };

    typedef eosio::multi_index<"premonster"_n, premonster> user_preregist_monsters;

    TABLE preitem
    {
        uint64_t index;
        uint32_t id;
        uint32_t type;
        uint32_t tier;
        uint32_t job;
        uint32_t grade;
        uint32_t main_status;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"preitem"_n, preitem> user_preregist_items;
#pragma endregion

//------------------------------------------------------------------------//
//--------------------------preregist_log_table----------------------------//
//------------------------------------------------------------------------//
#pragma region tokenlog table log
    TABLE tokenlog
    {
        eosio::name owner;
        uint64_t total_token_amount = 0;
        uint64_t primary_key() const { return owner.value; }
    };
    typedef eosio::multi_index<"tokenlog"_n, tokenlog> total_token_logs;
    ACTION inittokenlog();
    ACTION settokenlog();
#pragma endregion



// ////복사용 테이블

//     struct skill_object
//     {
//         uint64_t passive_skill_1 = 0;
//         uint64_t active_skill_2 = 0;
//         uint64_t active_skill_3 = 0;
//     };

//     TABLE monsterdb
//     {
//         uint64_t gacha_id;
//         uint64_t monster_id;
//         uint64_t gacha_type; //일반, 이벤트 구분
//         uint64_t tribe;      //종족
//         uint64_t type;       //수풍지화 암명
//         uint64_t monster_class; //몬스터 타입 클래스
//         std::vector<skill_object> skills;
//         uint64_t primary_key() const { return monster_id; }
//         uint64_t secondary_key() const { return gacha_id; }
//     };
//     typedef eosio::multi_index<"dbmonster"_n, monsterdb, indexed_by<"second"_n, const_mem_fun<monsterdb, uint64_t, &monsterdb::secondary_key>>> mon_db;

// #pragma endregion

// #pragma region db dbitemcommon
//     TABLE commonitemdb
//     {
//         uint64_t gacha_id;
//         uint64_t item_id;
//         uint32_t type;
//         uint32_t param_1;
//         uint32_t param_2;
//         uint32_t param_3;
//         uint64_t sell_id;
//         uint64_t sell_cost;
//         uint64_t primary_key() const { return item_id; }
//         uint64_t secondary_key() const { return gacha_id; }
//     };
//     typedef eosio::multi_index<"dbcommonitem"_n, commonitemdb, indexed_by<"second"_n, const_mem_fun<commonitemdb, uint64_t, &commonitemdb::secondary_key>>> commonitem_db;
// #pragma endregion

// #pragma region db dbitemequip
//     TABLE equipitemdb
//     {
//         uint64_t item_id;
//         uint64_t set_id;        //셋트 아이템일 경우를 대비
//         uint64_t type;
//         uint64_t tier;
//         uint64_t job;
//         uint64_t option;
//         uint32_t status_value_min;
//         uint32_t status_value_max;
//         uint32_t upgrade_status_value;
//         uint64_t random_option_id;

//         uint64_t primary_key() const { return item_id; }
//         uint64_t secondary_key() const { return set_id; }
//     };
//     typedef eosio::multi_index<"dbequipitems"_n, equipitemdb, indexed_by<"second"_n, const_mem_fun<equipitemdb, uint64_t, &equipitemdb::secondary_key>>> equipitem_db;
// #pragma endregion

//    TABLE servantdb
//     {
//         uint64_t gacha_id; 
//         uint64_t servant_id;
//         uint64_t primary_key() const { return servant_id; }
//         uint64_t secondary_key() const { return gacha_id; }
//     };
//     typedef eosio::multi_index<"dbservant"_n, servantdb, indexed_by<"second"_n, const_mem_fun<servantdb, uint64_t, &servantdb::secondary_key>>> ser_db;
};
