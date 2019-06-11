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
static const eosio::name NFT_CONTRACT = "nftbaymain11"_n;

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

    std::vector<uint32_t> item_in = {1,2,4,8,16,32,64,128};
    std::vector<uint32_t> level_in = {0,1,2,4,8,16,32,64};

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
        uint64_t sub_monster_upgrade;
        uint64_t ratio;
    };
    TABLE dbmonsterup
    {
        uint64_t main_monster_grade_upgrade;
        std::vector<upgrade_monster_sub> sub;
        uint64_t use_UTG;
        uint64_t primary_key() const { return main_monster_grade_upgrade; }
    };
    typedef eosio::multi_index<"dbmonsterup"_n, dbmonsterup> upgrade_monster_ratio_db;

    TABLE dbequipup
    {
        uint64_t equipment_type_grade; 
        std::vector<uint64_t> upgrade_ratio;
        uint64_t material_id;
        std::vector<uint64_t> material_count;
        std::vector<uint64_t> use_UTG;
        uint64_t primary_key() const { return equipment_type_grade; }
    };
    typedef eosio::multi_index<"dbequipup"_n, dbequipup> upgrade_equipment_ratio_dbs;

    TABLE dblevel
    {
        uint32_t lv;
        uint64_t rank_exp;
        uint64_t char_exp;
        uint64_t primary_key() const { return lv; }
    };
    typedef eosio::multi_index<"dblevel"_n, dblevel> lv_exp;

  struct lv_status_sub
    {
        uint64_t pre_status;
        uint64_t update_status;
    };

    TABLE dbstatusserv
    {
        uint64_t grade;
        std::vector<lv_status_sub> change_status;
        uint64_t primary_key() const { return grade; }
    };
    typedef eosio::multi_index<"dbstatusserv"_n,dbstatusserv> servant_lv_status_db;
    typedef eosio::multi_index<"dbstatusmon"_n, dbstatusserv> monster_lv_status_db;
    typedef eosio::multi_index<"dbstatusequi"_n, dbstatusserv> equipment_lv_status_db;

	  TABLE dbpassive
    {
        uint64_t passive_id;
        uint32_t passive_type;
        uint32_t job_class = 0;
        uint32_t enable_stack_max;
        uint32_t effect_id;
        uint32_t effect_type_id;
        uint32_t effect_value_a;
        uint32_t effect_value_add_b;
        uint32_t target_id;
        uint64_t primary_key() const {return passive_id;}        
    };
    typedef eosio::multi_index<"dbpassive"_n, dbpassive> passive_db;

    enum passvie_target_id
    {
        t_self = 1,
        t_partyall,
        t_enemyall,
        t_self_front = 11,
        t_beast = 901,
        t_human,
        t_demon,
        t_spirit,
        t_insect,
        t_mermaid,
        t_mysterious,
    };

    enum passive_effect_type_id
    {
        up = 1,
        down,
        per_up,
        per_down,
    };

    enum passive_effect_id
    {
        p_atk = 1,
        m_atk,
        p_dfs,
        m_dfs,
        hp,
        b_str,
        b_dex,
        b_int,
        speed,
        avoid,
        cri_per,
        cri_dmg,
        heal_per,
        physical_magic_dfs,
    };

    TABLE dbactive
    {
        uint64_t active_id = 0;
        uint32_t job = 0;
        uint64_t tribe = 0;
        uint32_t active_per = 0;
        uint32_t skill_type = 0;
        uint32_t active_turn = 0;
        uint32_t attack_type = 0;
        uint32_t dmg_type = 0;
        uint32_t elemental_type = 0;
        uint32_t target_type = 0;
        uint32_t target_count = 0;
        uint32_t hit_count = 0;
        uint32_t atk_per_1 = 0;
        uint32_t atk_per_add_1 = 0;
        uint32_t atk_per_2 = 0;
        uint32_t atk_per_add_2 = 0;
        uint32_t heal_per = 0;
        uint32_t heal_per_add = 0;
        uint32_t option_id;
        std::vector<uint32_t> buff_id_list;
        uint64_t primary_key() const { return active_id; }
    };
    typedef eosio::multi_index<"dbactive"_n, dbactive> active_db;

    enum active_target_type
    {
        self = 1,
        enemy,
        myteam,
        enemies,
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
        true_dmg,
    };

    enum active_option
    {
        option_defense = 1001,

        option_fastattack = 2001,
        option_perfectcri,
        option_ignoreevade,
        option_nocritical,
        option_nocri_noavoid,

        option_defense_decrease = 3001,
    };

    enum active_target
    {
        target_self = 1,
        target_enemy,
        target_myteam,
        target_enemies,
    };

    enum active_skill_type
    {
        type_buff = 1,
        type_attack,    //2
        type_heal,      //3
        type_attack_debuff, //4
    };

/////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////NEW DB ///////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
#pragma region db dbmonster

    enum monster_type
    {
        fire = 1,
        water,
        earth,
        wind,
        light,
        dark,
    };

    TABLE dbmonster
    {
        uint64_t id;
        uint64_t tribe;         //종족
        uint64_t type;          //수풍지화 암명
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbmonsters"_n, dbmonster> monster_db;

#pragma endregion

#pragma region db dbitemcommon
    TABLE dballitem
    {
        uint64_t id;
        uint32_t type;
        uint32_t grade;
        uint64_t max_count;
        uint64_t sell_item_id;
        uint64_t sell_item_count;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dballitem"_n, dballitem> allitem_db;
#pragma endregion

#pragma region db dbitemequip

    enum option_list
    {
        physical_attack = 1,
        magic_attack,
        physical_defense,
        magic_defense,
        status_hp,
        status_str,
        status_dex,
        status_int,
    };

    TABLE dbequipment
    {
        uint64_t item_id;
        uint64_t set_id;        //셋트 아이템일 경우를 대비
        uint64_t type;
        uint64_t tier;
        uint64_t job;
        std::vector<uint64_t> option_list;
        uint64_t primary_key() const { return item_id; }
        uint64_t get_tier() const { return tier; }
    };
    typedef eosio::multi_index<"dbequipment"_n, dbequipment, indexed_by<"tier"_n, const_mem_fun<dbequipment, uint64_t, &dbequipment::get_tier>>> equipment_db;
#pragma endregion

#pragma region db dbitemburn
    TABLE dbburn
    {
        uint64_t servant_job;
        uint64_t result_item_id;
        uint64_t primary_key() const { return servant_job; }
    };
    typedef eosio::multi_index<"dbburn"_n, dbburn> burnitem_db;
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
   typedef eosio::multi_index<"dbprepool"_n, dbgachapool> pre_gacha_db;
   typedef eosio::multi_index<"dbgoldpool"_n, dbgachapool> gold_gacha_db;

    //servant_db servant_db_table(_self, _self.value);
    //auto servant_db_iter = servant_db_table.get_index<"second"_n>();   //샘플

    //------------------------------------------------------------------------//
    //----------------------------db_system-----------------------------------//
    //------------------------------------------------------------------------//

    enum tribe_type
    {
        tribe_beast = 1,
        tribe_human,
        tribe_demon,
        tribe_spirit,
        tribe_insect,
        tribe_mermaid,
        tribe_mysterious,
    };

    TABLE dbtribe
    {
        uint64_t id;
        uint32_t base_str;
        uint32_t base_dex;
        uint32_t base_int;
        uint32_t speed;
        uint32_t avoid;
        uint32_t cri_per;
        uint32_t cri_dmg;
        uint32_t pair;
        uint32_t triple;
        uint32_t penta;

        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbtribe"_n, dbtribe> tribe_db;

    enum servant_job
    {
        job_warrior = 1,
        job_theif,
        job_cleric,
        job_archer,
    };

    TABLE dbserstat
    {
        uint64_t id;
        uint32_t job;
        uint32_t grade;
        uint32_t base_str;
        uint32_t base_dex;
        uint32_t base_int;
        uint32_t speed;
        uint32_t avoid;
        uint32_t cri_per;
        uint32_t cri_dmg;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbserstat"_n, dbserstat> serstat_db;

    TABLE dbtype
    {
        uint64_t id;
        uint32_t strong;
        uint32_t weak;
        uint32_t strong_per;
        uint32_t weak_per;
        uint32_t pair;
        uint32_t triple;
        uint32_t penta;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbtype"_n, dbtype> type_db;

    void insert_type(uint64_t _id, uint32_t _strong, uint32_t _weak, uint32_t _strong_per, uint32_t _weak_per, uint32_t _pair, uint32_t _triple, uint32_t _penta);

    TABLE dbupstatus
    {
        uint64_t upgrade;
        std::vector<uint32_t> grade_list;
        uint64_t primary_key() const { return upgrade; }
    };
    typedef eosio::multi_index<"dbmoupstat"_n, dbupstatus> upgrade_monster_db;
    typedef eosio::multi_index<"dbequpstat"_n, dbupstatus> upgrade_equipment_db;

    void insert_monster_upgrade(uint64_t _upgrade, uint32_t _common, uint32_t _uncommon, uint32_t _rare, uint32_t _unique, uint32_t _legendary);
    void insert_equipment_upgrade(uint64_t _upgrade, uint32_t _common, uint32_t _uncommon, uint32_t _rare, uint32_t _unique, uint32_t _legendary);

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

    ACTION setmaster(eosio::name _master, uint32_t _type);
#pragma endregion


#pragma region db function
  public:
    void substr_value(std::string _value, std::vector<std::string> & _value_list, std::vector<size_t> & _size_list, uint32_t _size);
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
    struct status_info
    {
        uint32_t basic_str = 0;
        uint32_t basic_dex = 0;
        uint32_t basic_int = 0;
    };

#pragma region gacha tservant
    struct servant_info
    {
        uint32_t state;     //서번트 상태
        uint32_t exp = 0;   //서번트 경험치
        uint64_t id = 0;
        uint32_t level = 1;
        uint32_t grade = 5;
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
        uint64_t id;                //몬스터 id 값
        uint32_t state;             //몬스터 상태값
        uint32_t exp = 0;           //경험치
        uint32_t type = 0;          //속성 타입
        uint32_t tribe = 0; //몬스터의 클래스
        uint32_t grade;             // 등급
        uint32_t upgrade = 0;       //강화수치
        uint32_t level = 1;
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
    typedef eosio::multi_index<"tequipments"_n, tequipment> user_equip_items;

    struct item_info
    {
        uint64_t index;
        uint64_t count;
    };

    TABLE titem
    {
        uint64_t id;
        uint64_t type;
        std::vector<item_info> item_list;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"titem"_n, titem> user_items;

    TABLE tpackage
    {
        uint64_t id;
        uint64_t count;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"tpackage"_n, tpackage> user_packages;

#pragma endregion

#pragma region gacha tgacharesult
    enum result
    {
        servant = 1,
        monster,
        equipment,
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
    typedef eosio::multi_index<"tgoldresult"_n, tgacharesult> gold_gacha_results;
#pragma endregion

#pragma region gacha ttotalgacha

    TABLE ttotalgacha
    {
        eosio::name user;
        std::vector<result_info> result_list;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"ttotalgacha"_n, ttotalgacha> user_gacha_totals;
    typedef eosio::multi_index<"ttotalgold"_n, ttotalgacha> gold_gacha_totals;

#pragma endregion

#pragma region item shop
  
    TABLE itemshop
    {
        uint64_t id;
        uint64_t shop_type;
        uint64_t product_id;
        uint64_t product_count;
        uint64_t product_limit_max;
        uint64_t price_id;
        uint64_t price_count;

        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbitemshop"_n, itemshop> item_shop;

    TABLE packageshop
    {
        uint64_t id;
        uint64_t GET_UTG;
        uint64_t private_limit_max;
        uint64_t price_id;
        uint64_t price_count;

        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbpackagshop"_n, packageshop> package_shop;


    TABLE tshoplist
    {
        uint64_t id;
        uint64_t shop_type;        
        uint64_t shop_item_id;
        uint64_t limit_count;
        uint64_t primary_key() const { return id; }
        uint64_t get_type() const { return shop_type; }
    };
    typedef eosio::multi_index<"tshoplist"_n, tshoplist, indexed_by<"bytype"_n, const_mem_fun<tshoplist, uint64_t, &tshoplist::get_type>>> shop_list;



#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------mail_system--------------------------------//
    //------------------------------------------------------------------------//

#pragma region db dbmail

    TABLE tmail
    {
        uint64_t mail_index = 0;
        uint64_t mail_type; //서번트1, 몬스터2, 아이템 3, 재화 4
        uint64_t type_index;   //프리에선 고유 인덱스 , nft는 토큰 인덱스
        uint64_t count;
        uint64_t icon_id;   //pool에 있는 db_index 
        uint64_t get_time;  //메일 받은 시간 

        uint64_t primary_key() const { return mail_index; }
    };
    typedef eosio::multi_index<"tmail"_n, tmail> user_mail;

    TABLE utstoken
    {
        uint64_t idx;      // Unique 64 bit identifier,
        uint32_t t_idx;    // 유저 테이블 상에서의 고유 인덱스
        std::string state; // 토큰 상태

        eosio::name owner;  // token owner
        eosio::name master; // token master for search detail info
        asset value;        // token value (1 UTS)

        uint64_t primary_key() const { return idx; }
        uint64_t get_owner() const { return owner.value; }
    };
    typedef eosio::multi_index<"utstokens"_n, utstoken, indexed_by<"byowner"_n, const_mem_fun<utstoken, uint64_t, &utstoken::get_owner>>> uts_db;

    TABLE utmtoken
    {
        uint64_t idx;      // Unique 64 bit identifier,
        uint32_t t_idx;    // 유저 테이블 상에서의 고유 인덱스
        std::string state; // 토큰 상태

        eosio::name owner;  // token owner
        eosio::name master; // token master for search detail info
        asset value;        // token value (1 UTM)

        uint64_t primary_key() const { return idx; }
        uint64_t get_owner() const { return owner.value; }
    };
    typedef eosio::multi_index<"utmtokens"_n, utmtoken, indexed_by<"byowner"_n, const_mem_fun<utmtoken, uint64_t, &utmtoken::get_owner>>> utm_db;

    TABLE utitoken
    {
        uint64_t idx;      // Unique 64 bit identifier,
        uint32_t t_idx;    // 유저 테이블 상에서의 고유 인덱스
        std::string state; // 토큰 상태

        eosio::name owner;  // token owner
        eosio::name master; // token master for search detail info
        asset value;        // token value (1 UTI)

        uint64_t primary_key() const { return idx; }
        uint64_t get_owner() const { return owner.value; }
    };
    typedef eosio::multi_index<"utitokens"_n, utitoken, indexed_by<"byowner"_n, const_mem_fun<utitoken, uint64_t, &utitoken::get_owner>>> uti_db;

   TABLE tusermail
   {
       uint64_t index;
       uint64_t type;   //1은 서번트 2는 몬스터 3은 장비 4는 아이템 5는 UTG
       std::string body;    //형식은 ID:state:exp:type:
       uint64_t primary_key() const { return index; }
   };
    typedef eosio::multi_index<"tusermail"_n, tusermail> mail_reward_list;


//id : type : item_list { index : count }

#pragma endregion



    //------------------------------------------------------------------------//
    //-----------------------------gacha_system--------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region gacha values
    const char *action_gacha = "gacha";
    const char *action_signup = "signup";
    const char *action_referral = "refer_signup";
    const char *action_exchange = "exchange";
    const char *action_shopbuyitem = "shopbuyitem";

    uint32_t servant_random_count;
    uint32_t monster_random_count;
    uint32_t equipment_random_count;
    uint32_t item_random_count;

    enum use_money_type
    {
        EOS_GACHA = 1,
        UTG_GACHA = 2,
        PACKAGE = 3,
        BATTLE = 4,
        EVENT = 5,
    };


#pragma endregion

  public:
#pragma region gacha function
    uint64_t get_random_grade(uint64_t _rate);
    uint32_t get_servant_index(uint32_t _job, uint32_t _body, uint32_t _gender, uint32_t _head, uint32_t _hair);
    uint32_t get_servant_active_skill(uint32_t _job, uint32_t _seed);
    uint32_t get_passive_skill(uint32_t _type, uint32_t _job_or_tribe, uint64_t _seed);

    //void gacha_servant_id(eosio::name _user, uint64_t _seed, uint32_t _job, uint32_t _min, uint32_t _max, uint32_t _gold_type);

    uint8_t gacha_servant_head(uint64_t _seed, uint32_t _count);
    uint8_t gacha_servant_hair(uint64_t _seed, uint32_t _count);
    uint8_t gacha_servant_body(uint64_t _seed, uint32_t _count);
    uint32_t change_servant_status(uint32_t _status_grade);
    uint32_t change_monster_status(uint32_t _grade, uint32_t _status_grade);
    uint32_t change_equipment_statue(uint32_t _grade, uint32_t _status_grade);

    //void gacha_monster_id(eosio::name _user, uint64_t _seed, uint32_t _grade, uint32_t _max, uint32_t _gold_type);
    void set_tier_status(uint32_t &_value, uint32_t _tier);
   // void gacha_equipment_id(eosio::name _user, uint64_t _seed, uint32_t _grade, uint32_t _max, uint32_t _gold_type);
   // void get_new_item(eosio::name _user, uint32_t _item_id, uint32_t _count);

    uint64_t get_user_seed_value(uint64_t _user);

    void start_gacha(eosio::name _user, uint64_t _seed, uint64_t _use_eos);    
    void start_gacha_10(eosio::name _user, uint64_t _seed, uint64_t _use_eos, uint32_t _count);
    void gacha_get_object(eosio::name _user, uint64_t _seed, uint32_t _grade);

    bool check_inventory(eosio::name _user, uint32_t _count);
    ACTION mailopen(eosio::name _user, const std::vector<uint64_t> &_mail_index);
    
    //void gold_gacha_item_id(eosio::name _user, uint64_t _seed);

    void write_log(eosio::name _user, uint32_t _gold_type, uint32_t _gacha_type, uint32_t _gacha_index, uint32_t _inventory_count);

    ACTION goldgacha(eosio::name _user, string _memo);
    void gold_gacha(eosio::name _user, uint64_t _seed, uint32_t _second_seed);

#pragma endregion

//------------------------------------------------------------------------//
//------------------------------user_log_table----------------------------//
//------------------------------------------------------------------------//
#pragma region login table prelog
    TABLE tuserlog
    {
        eosio::name user;
        uint64_t servant_num = 0;
        uint64_t monster_num = 0;
        uint64_t equipment_num = 0;
        uint64_t gacha_num = 0;
        uint64_t item_num = 0;
        uint64_t get_utg = 0;
        uint64_t use_utg = 0;
        uint64_t use_eos = 0;
        uint64_t battle_count = 0;
        uint64_t last_stage_num = 0;
        uint64_t last_tower_num = 0;
        uint64_t top_clear_stage = 0;
        uint64_t top_clear_tower = 0;
        uint64_t add_party_count = 0;
        uint64_t soul_powder = 0;
        uint64_t mail = 0;

        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"tuserlog"_n, tuserlog> user_logs;
  TABLE tgoldlogs
    {
        eosio::name user;
        uint64_t monster_num = 0;
        uint64_t equipment_num = 0;
        uint64_t item_num = 0;
        uint64_t gold_gacha_num = 0;
        uint64_t use_utg = 0;
        
        uint64_t primary_key() const {return user.value;}
    };
    typedef eosio::multi_index<"tgoldlogs"_n, tgoldlogs> gold_logs;

    void set_eos_log(uint64_t _total_amount);

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
        empty,
        stage, 
        tower,
        pvp,
    };

    enum object_state
    {
        on_inventory = 1,
        on_equip_slot,
        on_mail,
        on_tokenization,
        on_party,
    };
#pragma endregion

#pragma region login table
    TABLE tuserauth
    {
        eosio::name user;
        uint32_t state = user_state::lobby;
        uint32_t exp = 0;
        uint32_t rank = 1;
        uint32_t current_servant_inventory = 0;
        uint32_t current_monster_inventory = 0;
        uint32_t current_equipment_inventory = 0;
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
        uint64_t type;
        uint64_t amount;
        uint32_t count;
        uint64_t seed;
        name to;
        asset quantity;
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
    void signup(eosio::name _user, eosio::name _refer, uint64_t _use_eos);
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
    void black(eosio::name _user, std::string _type);
#pragma endregion

#pragma region master system
    ACTION setpause(uint64_t _state);
    void master_active_check();
    void system_check(eosio::name _user);
#pragma endrgion

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
    bool check_same_party(eosio::name _user, uint32_t _party_number, const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list);
    bool check_empty_party(const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list);
    ACTION saveparty(eosio::name _user, uint32_t _party_number, const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list);
#pragma endregion

    //------------------------------------------------------------------------//
    //-------------------------Item_equipment_function------------------------//
    //------------------------------------------------------------------------//
#pragma region item system

    ACTION burn(eosio::name _user, uint64_t _type, const std::vector<uint64_t> &_list);
    void servantburn(eosio::name _user, const std::vector<uint64_t> &_list);
    void monsterburn(eosio::name _user, const std::vector<uint64_t> &_list);
    void equipburn(eosio::name _user, const std::vector<uint64_t> &_list);

    //ACTION itemburn(eosio::name _user, const std::vector<uint64_t> &_item_list, const std::vector<uint64_t> &_count_list);

    ACTION equip(eosio::name _user, uint32_t _servant_index, uint32_t _item_index);
    
    bool compare_item(uint32_t _user_servant, uint32_t _user_item);

#pragma endregion

    //------------------------------------------------------------------------//
    //-------------------------Item_equipment_function------------------------//
    //------------------------------------------------------------------------//

#pragma region upgrade system

    //ACTION equipmentup(eosio::name _user, uint32_t _equipment, const std::vector<uint64_t> &_item_list);
    ACTION monsterup(eosio::name _user, uint32_t _monster, uint32_t _monster2);
    ACTION upgrade(eosio::name _user, uint32_t _type, uint32_t _main, const std::vector<uint64_t> &_item_list);
    void equip_upgrade(eosio::name _user, uint32_t _equipment, const std::vector<uint64_t> &_item_list); 
    //void skill_lvup(eosio::name _user, uint32_t _main, const std::vector<uint64_t> &_item_list);
#pragma endregion

    //------------------------------------------------------------------------//
    //----------------------------Item_store_function-------------------------//
    //------------------------------------------------------------------------//

#pragma region store system

    ACTION itembuy(eosio::name _user, uint32_t _item_id, uint32_t _count);

    void utg_item_buy(eosio::name _user, uint32_t _item_id, uint32_t _count);
    void etc_item_buy(eosio::name _user, uint32_t _item_id, uint32_t _count);
    void skill_lvup_buy(eosio::name _user, uint32_t _item_id, uint32_t _count);
    void common_skill_change_buy(eosio::name _user, uint32_t _item_id, uint32_t _count);
    void main_skill_change_buy(eosio::name _user, uint32_t _item_id, uint32_t _count);

    void shop_buy_item(eosio::name _user, uint32_t _type, uint32_t _count, uint64_t _seed);
    void inventory_buy(eosio::name _user, uint32_t _type, uint32_t _count);
    void ticket_buy(eosio::name _user, uint32_t _type, uint32_t _count);
    void package_buy(eosio::name _user, uint32_t _type, uint32_t _count, uint64_t _seed);
    void grade_package(eosio::name _user, uint32_t _type);
    void start_package(eosio::name _user);
    
#pragma endregion

    //------------------------------------------------------------------------//
    //---------------------- -------battle_state_table-------------------------//
    //------------------------------------------------------------------------//
#pragma region battle state table


    // 4 + 4 + 4 + 4 + 4 + 4 + 8 + sbattle_member_state(9) = 41
    // sbattle_member_state 당 9 총 5개의 버프창이 있으면 45 + 32 = 77
  private:
    const uint32_t defense_constant = 1000;
    const uint32_t decimal = 100;
    const uint32_t max_party_count = 10;

  public:
    enum stage_state
    {
        start = 1,
        win,
        lose,
    };

    enum character_type
    {
        t_servant = 1,
        t_monster,
    };

    struct buff_info //4 +4 = 8
    {
        uint32_t id = 0;   //4
        uint32_t turn = 0; //4
    };

    struct battle_status_info
    {
        uint32_t type = 0; //servant 1 //monster 2
        uint32_t upgrade = 0;
        uint32_t max_hp = 0;
        uint32_t p_atk = 0;
        uint32_t m_atk = 0;
        uint32_t cri_dmg_per = 0;
        uint32_t cri_per = 0;
        uint32_t p_dfs = 0;
        uint32_t m_dfs = 0;
        uint32_t add_heal_skill_value_per = 0;
        uint32_t avoid = 0;
        uint32_t speed = 0;
        uint32_t position = 0;
        uint64_t second_speed = 0;
        uint32_t action = 0;
        status_info status;
        uint32_t id = 0;
        uint32_t index = 0;
        uint32_t now_hp = 0;
        uint32_t state = 0;
        uint32_t level = 0;

        std::vector<buff_info> buff_list;
        std::vector<uint32_t> passive_skill_list;
        std::vector<uint32_t> active_skill_list;
    };

    struct character_state_data //32 + 8 + 12 = 52 * 20 = 1040
    {
        uint32_t grade;    //4
        uint32_t position; //4
        uint32_t index;    //4
        uint32_t id;       //4
        uint32_t now_hp;   //4
        uint32_t state;    //4
        uint32_t type;      //서번트인지 몬스터인지
        uint32_t upgrade;
        uint32_t max_hp;
        uint32_t level;

        std::vector<buff_info> buff_list;         //8
        std::vector<uint32_t> passive_skill_list; //4
        std::vector<uint32_t> active_skill_list;  //4
        
        status_info status;                       //4+4+4 = 12
    };

#pragma endregion

    //------------------------------------------------------------------------//
    //---------------------------battle_action_table--------------------------//
    //------------------------------------------------------------------------//
#pragma region battle action table
    struct heal_compare
    {
        uint32_t max_hp;
        uint32_t now_hp;
        uint32_t key;
    };

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
       std::vector<item_info> item_list;
   };
   struct exp_info
   {
       uint64_t pos = 0;
       uint32_t exp = 0;
       uint32_t lvup = 0;
   };
    
   TABLE tclearreward
   {
       eosio::name user;
       uint64_t reward_money;
       exp_info get_rank_exp;
       std::vector<exp_info> get_char_exp_list;
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
    void set_upgrade_equip_status(uint64_t _grade, uint32_t _value, uint32_t _upgrade);
    void set_upgrade_monster_status(uint64_t _grade, status_info &_status, uint32_t _upgrade);
    uint32_t get_stage_id(uint32_t _tier, uint32_t _type, uint32_t _grade);
    uint32_t get_max_hp(status_info _status, uint32_t _level);
    uint32_t get_magic_attack(status_info _status, uint32_t _level);
    uint32_t get_physical_attack(status_info _status, uint32_t _level);
    uint32_t get_magic_defense(status_info _status, uint32_t _level);
    uint32_t get_physical_defense(status_info _status, uint32_t _level);
    uint32_t get_cri_per(status_info _status, uint32_t _level);
    uint32_t get_cri_dmg_per(status_info _status, uint32_t _level);
    uint32_t get_tribe_count(std::vector<character_state_data> &_my_state_list, uint32_t _tribe);
    void set_synergy(
        std::vector<character_state_data> & _my_state_list, std::vector<uint32_t> &_synergy_list);
    void set_hp_synergy(
        std::vector<character_state_data> & _my_state_list, std::vector<uint32_t> &_synergy_list);
    character_state_data get_user_state(eosio::name _user, std::string _type, uint64_t _index, uint32_t _position, std::vector<std::string> & _state);
    bool possible_start(eosio::name _user, uint32_t _party_number);
    ACTION stagestart(eosio::name _user, uint32_t _party_number, uint32_t _floor, uint32_t _type, uint32_t _difficult);

    void init_buff_turn_self(battle_status_info & _status);
    bool check_activate_skill(uint32_t _skill, uint64_t _rate);
    uint32_t get_damage(uint32_t _atk, uint32_t _dfs);
    bool check_critical(uint64_t _critcal_per, uint64_t _seed);
    bool check_avoid(uint64_t _avoid_per, uint64_t _seed);

    static bool new_sort_compare(const battle_status_info &a, const battle_status_info &b);
    static bool new_sort_heal_compare(const heal_compare &a, const heal_compare &b);

    void set_equipment_basic_status(eosio::name _from,
                                    eosio::name _to,
                                    std::vector<uint64_t> & _second_seed_list,
                                    std::vector<battle_status_info> & _my_status_list,
                                    std::vector<battle_status_info> & _enemy_status_list,
                                    const std::vector<character_state_data> &_my_state_list,
                                    const std::vector<character_state_data> &_enemy_state_list,
                                    const std::vector<uint32_t> &_my_synergy_list,
                                    const std::vector<uint32_t> &_enemy_synergy_list);
    void set_passive_basic_status_self(battle_status_info & _status);

    void set_passive_battle_status(std::vector<battle_status_info> & _my_status_list,
                                   std::vector<battle_status_info> & _enemy_status_list);
    void set_synergy_battle_status(std::vector<battle_status_info> & _my_status_list,
                                   std::vector<battle_status_info> & _enemy_status_list,
                                   const std::vector<uint32_t> &_my_synergy_list,
                                   const std::vector<uint32_t> &_enemy_synergy_list);

    //=====================skill======================//
    int get_state_position_key(std::vector<character_state_data> & _state_list, uint32_t _position);
    int get_status_position_key(std::vector<battle_status_info> & _status_list, uint32_t _position);

    void set_skill_damage(battle_status_info &_my_status, battle_status_info &_enemy_status, uint32_t &_attack, uint32_t &_target_defense, uint32_t &_target_avoid);

    //================================================//
    void result_type_skill(eosio::name _user ,action_info &_action, std::vector<battle_status_info> &_my_status_list,
                                    std::vector<battle_status_info> &_enemy_status_list,
                                    uint64_t _my_key, uint32_t _enemy_key);
    void set_random_damage(action_info &_action, uint64_t _seed);
    void result_buff(action_info &_action, battle_status_info &_status);
    void result_type_damage(eosio::name _user, action_info & _action, std::vector<battle_status_info> & _my_status_list,
                            std::vector<battle_status_info> & _enemy_status_list,
                            uint64_t _my_key, uint32_t _enemy_key);
    bool set_action(eosio::name _user,
                    uint32_t _action, uint64_t _seed,
                    std::vector<battle_status_info> & _my_status_list,
                    std::vector<battle_status_info> & _enemy_status_list,
                    uint64_t _my_key, character_action_data & _action_info,
                    std::vector<std::string> & _data);
    action_info get_target_action(uint32_t _actvie_id, uint64_t _seed, uint64_t _my_key, uint64_t _target_key,
                                  std::vector<battle_status_info> & _my_status_list, std::vector<battle_status_info> & _enemy_status_list);
    int get_heal_target(const std::vector<battle_status_info> &_enemy_state_list);
    int get_random_target(const std::vector<battle_status_info> &_enemy_state_list, uint64_t _seed, uint32_t _max, uint32_t _min);

    void set_result_state(std::vector<battle_status_info> & _my_status_list,
                          std::vector<battle_status_info> & _enemy_status_list,
                          std::vector<character_state_data> & _my_state_list,
                          std::vector<character_state_data> & _enemy_state_list);

    uint32_t check_char_level_up(uint32_t _cur_level, uint64_t _get_exp);
    uint32_t check_rank_level_up(uint32_t _cur_level, uint64_t _get_exp);

    // servant_data get_reward_servant(eosio::name _user, uint32_t _job, uint64_t _seed, uint32_t _type);
    // monster_data get_reward_monster(eosio::name _user, uint32_t _id, uint32_t _grade, uint64_t _seed, uint32_t _type);
    // equip_data get_reward_equip(eosio::name _user, uint32_t _id, uint32_t _grade, uint64_t _seed, uint32_t _type);
    // item_data get_reward_item(eosio::name _user, uint32_t _id, uint32_t _count, uint32_t _type);
    void get_reward_utg(eosio::name _user, uint32_t _count);
    void fail_reward(eosio::name _user, uint64_t _stage_number);

    ACTION activeturn(eosio::name _user, uint32_t _turn, std::string _seed);

    ACTION stageexit(eosio::name _user);


    servant_data get_servant(eosio::name _user, uint32_t _job, uint32_t _min, uint32_t _max, uint32_t _gold_type, uint64_t _seed);
    monster_data get_monster(eosio::name _user, uint32_t _id, uint32_t _grade, uint32_t _max, uint32_t _gold_type, uint64_t _seed);
    equip_data get_equip(eosio::name _user, uint32_t _id, uint32_t _grade, uint32_t _max, uint32_t _gold_type, uint64_t _seed);
    item_data get_item(eosio::name _user, uint32_t _id, uint32_t _count, uint32_t _gold_type, uint64_t _seed);
#pragma endregion

#pragma region tower_system

    uint64_t nftexchange(eosio::name _owner, eosio::name _master, std::string _type, uint64_t _master_index);
    ACTION nftmail(eosio::name _user, std::string _type, uint64_t _token_index,uint64_t _icon_id);


    //------------------------------------------------------------------------------//
    //-----------------------------preregist_table-----------------------------------//
    //-------------------------------------------------------------------------------//

#pragma region preregist
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
#pragma region tokenization
    ACTION changetoken(eosio::name _user, std::string _type, uint64_t _index);       //0.1 EOS
#pragma endregion

#pragma region chat
    ACTION chat(name _user, asset _price, string _text);
    TABLE tchat
    {
        name owner;
        asset price;
        name user;
        string text;
        uint64_t start_time;
        uint64_t primary_key() const { return owner.value; }
    };
    typedef eosio::multi_index<"tchat"_n, tchat> chat_index;
#pragma endregion


#pragma region pvp
    TABLE pvplog
    {
        uint64_t index;
        name user;
        string result;

        uint64_t primary_key() const { return index; }
        uint64_t get_user() const { return user.value; }
    };
    typedef eosio::multi_index<"pvplog"_n, pvplog> pvp_log_index;
    //ACTION pvpstart(eosio::name _from, eosio::name _to);
#pragma endregion

#pragma region tower
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
        eosio::name winner;
        uint64_t primary_key() const { return snapshot_count; }
    };
    typedef eosio::multi_index<"tsnapshot"_n, tsnapshot> eos_snapshots;

    void towersnap(uint64_t fnum);
    void settower(eosio::name _loser, eosio::name _winner, uint64_t _loser_party_num, uint64_t _winner_party_num);

    TABLE floorinfo
    {
        uint64_t fnum;
        name owner;
        uint64_t bnum;
        uint64_t pnum;
        string state;
        uint64_t endtime;

        uint64_t primary_key() const { return fnum; }
    };
    typedef eosio::multi_index<"floorinfos"_n, floorinfo> floor_index;

    ACTION toweropen();                                  //1층에 아무도 없을때 우리가 열어주는 기능
    ACTION endflag(eosio::name _winner, uint64_t _fnum); //24시간 체크
    void resetparty(eosio::name _user, uint64_t _pnum);
    ACTION claim(eosio::name who, uint64_t funm);        //인출하고  다음층여는기능
    void towerwin(eosio::name winner, uint64_t fnum, uint64_t pnum, uint64_t bnum);
    void towerlose(eosio::name loser);
    void get_tower_state(uint64_t _fnum, std::vector<character_state_data> &_enemy_state_list, std::vector<std::string> &_state);
    ACTION towerstart(eosio::name _from, uint64_t _fnum);
    ACTION deletetower();

#pragma endregion

#pragma region refer
    TABLE treferlist
    {
        eosio::name referer;
        uint64_t primary_key() const { return referer.value; }
    };
    typedef eosio::multi_index<"treferlist"_n, treferlist> referlist;

    void refer(eosio::name _referer, std::string _type);
#pragma endregion

#pragma region white
    TABLE twhitelist
    {
        eosio::name user;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"twhitelist"_n, twhitelist> whitelist;

    void white(eosio::name _user, std::string _type);
#pragma endregion


void utg_exchange(eosio::name _user);


TABLE stageinfo
{
    uint64_t id;
    uint32_t type;
    uint32_t floor;
    uint32_t difficult;
    uint32_t need_entrance_item_id;
    uint32_t need_entrance_item_count;
    uint32_t enemy_level_min;
    uint32_t enemy_level_max;
    uint32_t enemy_count;
    std::vector<uint32_t> option_list;
    uint64_t primary_key() const { return id; }
};

typedef eosio::multi_index<"dbstageinfo"_n, stageinfo> stageinfo_db;


TABLE enemyinfo
{
    uint64_t index;
    uint64_t id;
    uint32_t type;
    std::vector<uint32_t> active_list;
    std::vector<uint32_t> passive_list;
    uint64_t primary_key() const { return index; }
};

typedef eosio::multi_index<"dbenemyinfo"_n, enemyinfo> enemyinfo_db;

TABLE enemystat
{
    uint64_t id;
    uint32_t base_str;
    uint32_t base_dex;
    uint32_t base_int;
    uint64_t primary_key() const { return id; }
};  

typedef eosio::multi_index<"dbenemystat"_n, enemystat> enemystat_db;


struct reward_item_info
{
    uint32_t id;
    uint32_t per;
    uint32_t count;
};

TABLE reward
{
    uint64_t id;
    uint32_t reward_utg;
    uint32_t rank_exp;
    uint32_t char_exp;
    uint32_t reward_count;
    uint32_t per_monster;
    uint32_t per_equipment;
    std::vector<reward_item_info> reward_list;
    uint64_t primary_key() const { return id; }
};

typedef eosio::multi_index<"dbnewreward"_n, reward> new_reward_db;




TABLE stagestateinfo
{
    eosio::name user;
    eosio::name enemy_user;
    uint8_t type;
    uint8_t difficult;
    uint8_t floor;
    uint64_t turn = 0;
    std::vector<character_state_data> my_state_list;
    std::vector<character_state_data> enemy_state_list;
    std::vector<uint32_t> my_synergy_list;
    std::vector<uint32_t> enemy_synergy_list;

    uint64_t primary_key() const { return user.value; }
};
typedef eosio::multi_index<"tstgstates"_n, stagestateinfo> new_battle_state_list;

void check_enter_stage(eosio::name _user, uint32_t _stage_id);
void new_set_stage_state(uint64_t _stage_id, uint64_t _seed, std::vector<character_state_data> & _enemy_state_list, std::vector<std::string> & _state);
void new_win_reward(eosio::name _user, uint64_t _stage_id, uint64_t _seed, std::vector<uint32_t> _reward_monster_id);

uint32_t sum_item_check(eosio::name _user, uint32_t _item_id, uint32_t _count);
uint32_t sub_item_check(eosio::name _user, uint32_t _item_id, uint32_t _count);

void change_user_state(eosio::name _user, uint32_t _check_state, uint32_t _state);
void init_action_reward_table(eosio::name _user);

ACTION systemact(std::string _function, eosio::name _user, std::string _type);

//------------------------------------------------------------------------//
//--------------------------daily_check_table-----------------------------//
//------------------------------------------------------------------------//

#pragma region dailycheck

TABLE tdaily
{
    eosio::name user;
    uint64_t total_day;
    uint64_t check_time;
    uint64_t primary_key() const { return user.value; }
};
typedef eosio::multi_index<"tdaily"_n, tdaily> dailychecks;

ACTION dailycheck(eosio::name _user, string _seed);
bool timecheck(uint64_t user_checktime);
void daily_check_reward(eosio::name _user, uint64_t totalday, uint64_t _seed);
//ACTION resetdaily(eosio::name _user);

servant_info get_servant_random_state(uint32_t _id, uint64_t _seed, uint32_t _job, uint32_t _base_str, uint32_t _base_dex, uint32_t _base_int);
monster_info get_monster_random_state(uint32_t _id, uint64_t _seed, uint32_t _grade, uint32_t _tribe, uint32_t _type, uint32_t _base_str, uint32_t _base_dex, uint32_t _base_int);
equipment_info get_equip_random_state(uint32_t _id, uint64_t _seed, uint32_t _type, uint32_t _tier, uint32_t _job, uint32_t _grade);

#pragma endregion

//dailychecks::const_iterator get_daily(eosio::name _user);

floor_index::const_iterator get_floor(uint64_t _fnum);
servant_db::const_iterator get_servant_db(uint64_t _id);
monster_db::const_iterator get_monster_db(uint64_t _id);
tribe_db::const_iterator get_tribe_db(uint64_t _id);
// equipment_db::const_iterator get_equipment_db(uint64_t _id);    //
// active_db::const_iterator get_active_db(uint64_t _id);          //
passive_db::const_iterator get_passive_db(uint64_t _id);
// user_servants::const_iterator get_user_servant(eosio::name _user, uint64_t _index); //
// user_monsters::const_iterator get_user_monster(eosio::name _user, uint64_t _index); //
// user_equip_items::const_iterator get_user_equipment(eosio::name _user, uint64_t _index);
shop_list::const_iterator get_shop_list(uint64_t _id);
item_shop::const_iterator get_item_shop(uint64_t _id);
allitem_db::const_iterator get_allitem_db(uint64_t _id);

//end
};