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

CONTRACT untpreregist : public contract
{
  public:
#pragma resgion contract constructor
    //using contract::contract;
    eosio::name owner;
    permission_level owner_auth;
    
    untpreregist(eosio::name _self, eosio::name _code, datastream<const char *> ds) : contract(_self, _code, ds)
    {
        owner = _self;
        owner_auth.actor = _self;
        owner_auth.permission = "owner"_n;
    }
#pragma endregion
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
#pragma region db dbbody
    TABLE dbbody
    {
        uint64_t body;
        uint64_t primary_key() const { return body; }
    };
    typedef eosio::multi_index<"dbbody"_n, dbbody> body_db;
#pragma endregion

#pragma region db dbhair
    TABLE dbhair
    {
        uint64_t hair;
        uint64_t primary_key() const { return hair; }
    };
    typedef eosio::multi_index<"dbhair"_n, dbhair> hair_db;
#pragma endregion

#pragma region db dbhead
    TABLE dbhead
    {
        uint64_t head;
        uint64_t primary_key() const { return head; }
    };
    typedef eosio::multi_index<"dbhead"_n, dbhead> head_db;
#pragma endregion

#pragma region db dbgender
    TABLE dbgender
    {
        uint64_t gender;
        uint64_t primary_key() const { return gender; }
    };
    typedef eosio::multi_index<"dbgender"_n, dbgender> gender_db;
#pragma endregion

#pragma region db dbitemgrade
    TABLE dbitemgrade
    {
        uint64_t grade;
        object_status min_range;
        object_status max_range;
        uint64_t primary_key() const { return grade; }
    };
    typedef eosio::multi_index<"dbitemgrade"_n, dbitemgrade> item_grade_db;
#pragma endregion

#pragma region db dbitemid
    TABLE dbitemid
    {
        uint64_t id;
        uint64_t type;
        uint64_t job;
        uint64_t tier;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbitemid"_n, dbitemid> item_id_db;
#pragma endregion

#pragma region db dbmonstergd
    TABLE dbmonstergd
    {
        uint64_t grade;
        object_status min_range;
        object_status max_range;
        uint64_t primary_key() const { return grade; }
    };
    typedef eosio::multi_index<"dbmonstergd"_n, dbmonstergd> monster_grade_db;
#pragma endregion

#pragma region db dbmonsterid
    TABLE dbmonsterid
    {
        uint64_t id;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbmonsterid"_n, dbmonsterid> monster_id_db;
#pragma endregion

#pragma region db dbservantjob
    TABLE dbservantjob
    {
        uint64_t job;
        object_status min_range;
        object_status max_range;
        uint64_t primary_key() const { return job; }
    };
    typedef eosio::multi_index<"dbservantjob"_n, dbservantjob> servant_job_db;
#pragma endregion

#pragma region db dbservantid
    TABLE dbservantid
    {
        uint64_t index;
        uint64_t id;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"dbservantid"_n, dbservantid> servant_id_db;
#pragma endregion

#pragma region db dbgraderatio
    TABLE dbgraderatio
    {
        uint64_t grade;
        uint64_t ratio;
        uint64_t primary_key() const { return grade; }
    };
    typedef eosio::multi_index<"dbgraderatio"_n, dbgraderatio> grade_ratio_db;
#pragma endregion

enum db_index
{
    job =1, 
    body,
    hair,
    head,
    gender,
    servant_id,
    monster_id,
    monster_grade,
    item_id,
    item_grade,
    grade_ratio,
};

    //------------------------------------------------------------------------//
    //----------------------------db_system-----------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region db values
    
#pragma endregion

  public:
#pragma region db action
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

    //-------------------------------------------------------//
    ACTION setmaster(eosio::name _master);
    ACTION initmaster();
    //------------------------------------------------------//

    ACTION settokenlog();
    ACTION dbinsert(uint32_t _kind, uint32_t _appear, uint32_t _id, uint32_t _index, uint32_t _job, uint32_t _tier, uint32_t _type, uint32_t _grade, uint32_t _min, uint32_t _max, uint32_t _ratio);
    ACTION dbmodify(uint32_t _kind, uint32_t _appear, uint32_t _id, uint32_t _index, uint32_t _job, uint32_t _tier, uint32_t _type, uint32_t _grade, uint32_t _min, uint32_t _max, uint32_t _ratio);
    ACTION dberase(uint32_t _kind, uint32_t _appear, uint32_t _id, uint32_t _job, uint32_t _tier, uint32_t _type, uint32_t _grade, uint32_t _min, uint32_t _max);
    ACTION dbinit();
    ACTION stageinsert(uint32_t _kind, uint32_t _stage_num, uint32_t _id, uint32_t _str, uint32_t _dex, uint32_t _int);
    ACTION stagemodify(uint32_t _stage_num, uint32_t _id, uint32_t _str, uint32_t _dex, uint32_t _int);
    ACTION stageerease(uint32_t _kind, uint32_t _stage_num, uint32_t _id);
#pragma endregion

  public:
#pragma region db function

    void insert_job(uint32_t _job,uint32_t _min, uint32_t _max);
    void insert_head(uint32_t _appear);
    void insert_hair(uint32_t _appear);
    void insert_body(uint32_t _appear);
    void insert_gender(uint32_t _appear);
    void insert_servant_id(uint32_t _id, uint32_t _index);
    void insert_monster_id(uint32_t _id);
    void insert_monster_grade(uint32_t _grade, uint32_t _min, uint32_t _max);
    void insert_item_id(uint32_t id, uint32_t type, uint32_t _job, uint32_t tier);
    void insert_item_grade(uint32_t _grade, uint32_t _min, uint32_t _max); 
    void insert_grade_ratio(uint32_t _grade, uint32_t _ratio);
    void insert_stage(uint64_t _stage_type);
    void insert_stage_monster(uint64_t _stage_num, uint32_t _id, uint32_t _str, uint32_t _dex, uint32_t _int);


    void modify_job(uint32_t _job, uint32_t _min, uint32_t _max);
    void modify_head(uint32_t _appear);
    void modify_hair(uint32_t _appear);
    void modify_body(uint32_t _appear);
    void modify_gender(uint32_t _appear);
    void modify_servant_id(uint32_t _id, uint32_t _index);
    void modify_monster_id(uint32_t _id);
    void modify_monster_grade(uint32_t _grade, uint32_t _min, uint32_t _max);
    void modify_item_id(uint32_t id, uint32_t type, uint32_t _job, uint32_t tier);
    void modify_item_grade(uint32_t _grade, uint32_t _min, uint32_t _max); 
    void modify_grade_ratio(uint32_t _grade, uint32_t _ratio);
    void modify_stage_monster(uint64_t _stage_num, uint32_t _id, uint32_t _str, uint32_t _dex, uint32_t _int);    


    void erase_job(uint32_t _job);
    void erase_head(uint32_t _appear);
    void erase_hair(uint32_t _appear);
    void erase_body(uint32_t _appear);
    void erase_gender(uint32_t _appear);
    void erase_servant_id(uint32_t _id);
    void erase_monster_id(uint32_t _id);
    void erase_monster_grade(uint32_t _grade);
    void erase_item_id(uint32_t id);
    void erase_item_grade(uint32_t _grade);
    void erase_grade_ratio(uint32_t _grade);
    void erase_stage(uint64_t _stage_num);
    void erase_stage_monster(uint64_t _stage_num, uint32_t _id);
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
      //struct appear_info
    struct appear_info
    {
        uint32_t head = 0;
        uint32_t hair = 0;
        uint32_t body = 0;
        uint32_t gender = 0;
    };

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
        uint32_t state;                   //서번트 상태
        uint32_t exp = 0;                 //서번트 경험치
        appear_info appear;               //서번트의 외모 정보
        uint32_t job = 0;                 //서번트의 직업 정보
        uint32_t stat_point = 0;
        status_info status;               //기본 힘,민,지 추가 힘,민,지
        std::vector<uint32_t> equip_slot; //서번트 장비 리스트
    };

    TABLE tservant
    {
        uint64_t index;
        uint32_t party_number = 0;
        servant_info servant;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"tservant"_n, tservant> user_servants;

    TABLE preservant
    {
        uint64_t index;
        uint32_t id;
        status_info status;
        uint64_t primary_key() const { return index; }
    };

    typedef eosio::multi_index<"preservant"_n, preservant> user_preregist_servants;
#pragma endregion

#pragma region gacha tmonster
    struct monster_info
    {
        uint32_t id;        //몬스터 id 값
        uint32_t state;    //몬스터 상태값
        uint32_t exp = 0;       //경험치
        uint32_t type = 0;     //속성 타입
        uint32_t grade;       // 등급
        uint32_t upgrade = 0; //강화수치
        status_info status;   //기본 힘,민,지 추가 힘,민,지
    };

    TABLE tmonster
    {
        uint64_t index;
        uint32_t party_number = 0;
        monster_info monster;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"tmonster"_n, tmonster> user_monsters;


    TABLE premonster
    {
        uint64_t index;
        uint32_t id;
        uint32_t grade;
        status_info status;
        uint64_t primary_key() const { return index; }
    };

    typedef eosio::multi_index<"premonster"_n, premonster> user_preregist_monsters;
#pragma endregion

#pragma region gacha titem
    struct item_info
    {
        uint32_t id;          //아이템 리소스 아이디
        uint32_t state;       //아이템 현재 상태
        uint32_t type;        //장착 타입
        uint32_t tier;        //티어
        uint32_t job;         //직업제한
        uint32_t grade;       //아이템 등급
        uint32_t upgrade = 0; //아이템 강화 수치
        uint32_t atk = 0;
        uint32_t def = 0;
        status_info status; //기본 힘,민,지 추가 힘,민,지
    };

    TABLE titem
    {
        uint64_t index;
        item_info item;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"titem"_n, titem> user_items;


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

    //------------------------------------------------------------------------//
    //-----------------------------gacha_system--------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region gacha values
    const char *action_gacha="gacha";
    const char *action_signup="signup";

    uint32_t servant_random_count;
    uint32_t monster_random_count;
    uint32_t item_random_count;

    const uint32_t default_min = 0;
    const uint32_t max_rate = 1000;

#pragma endregion

  public:
#pragma region gacha function
    uint32_t get_random_grade(uint64_t _rate);
    uint32_t get_servant_index(uint32_t _job, uint32_t _body, uint32_t _gender, uint32_t _head, uint32_t _hair);

    void gacha_servant_id(eosio::name _user, uint64_t _seed);
    uint8_t gacha_servant_head(uint64_t _seed, uint32_t _count);
    uint8_t gacha_servant_hair(uint64_t _seed, uint32_t _count);
    uint8_t gacha_servant_body(uint64_t _seed, uint32_t _count);

    void gacha_monster_id(eosio::name _user, uint64_t _seed);
    void gacha_item_id(eosio::name _user, uint64_t _seed);
    
    uint64_t get_user_seed_value(uint64_t _user);

    void start_gacha(eosio::name _user, uint64_t _seed);
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


        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"tuserlog"_n, tuserlog> user_logs;

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


#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------login_table--------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region login enum
    enum user_state
    {        
        pre_regist = 1,        
        lobby,
        battle,
        battle_win,
        battle_lose,
        tower,
        travel,
    };
    enum hero_state
    {
        set_look = 1,
        set_status,
        set_change_status,
        set_complete,
        set_tower_party,
        set_travel_party,
    };

    enum object_state
    {
        on_inventory = 1,
        on_equip_slot,
        on_party,
        on_tower,
        object_preregist,
    };
#pragma endregion

#pragma region login table preauth
    //struct hero_info
    struct hero_info
    {
        uint32_t state;                   //히어로 상태
        uint32_t exp = 0;                 //히어로 경험치
        uint32_t job = 0;                 //히어로 직업
        uint32_t stat_point = 0;
        appear_info appear;               //히어로 외형 정보 <-젠더 추가해야함
        status_info status;               //기본 힘,민,지 추가 힘,민,지
        std::vector<uint32_t> equip_slot; //히어로 장비 리스트
    };


    TABLE tuserauth
    {
        eosio::name user;
        uint32_t state = user_state::lobby;
        hero_info hero;
        uint64_t primary_key() const { return user.value; }
    };

    typedef eosio::multi_index<"tuserauth"_n, tuserauth> auth_users;

    TABLE preauth
    {
        eosio::name user;
        uint32_t state = user_state::lobby;
        uint64_t primary_key() const { return user.value; }
    };

    typedef eosio::multi_index<"preauth"_n, preauth> pre_users;

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
    const uint32_t max_equip_slot = 3;
    const uint32_t hero_min_status = 1;
    uint32_t hero_total_status = 24;

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
    //-----------------------------init_function------------------------------//
    //------------------------------------------------------------------------//
ACTION inittokenlog();



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
ACTION resultgacha(eosio::name _from, eosio::name _to ,std::string _result);
ACTION resultpre(eosio::name _from, eosio::name _to ,std::string _result);


    //------------------------------------------------------------------------//
    //-------------------------------party_table------------------------------//
    //------------------------------------------------------------------------//

#pragma region party_table 

enum party_state
{
    on_wait = 1,
    on_tower_defense,
};
TABLE tparty
{
public:
    uint32_t index;
    uint32_t state = party_state::on_wait;
    std::vector<uint64_t> servant_list;
    std::vector<uint64_t> monster_list;

    uint64_t primary_key() const {return index;}
};

typedef eosio::multi_index<"tpartylist"_n,tparty> user_partys;

#pragma endregion

    //------------------------------------------------------------------------//
    //-------------------------------party_system-----------------------------//
    //------------------------------------------------------------------------//
#pragma region party_system
    public:
    ACTION setparty(eosio::name _user, uint32_t _party_number, const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list);
    void add_party_list(eosio::name _user);


#pragma region party cheat

    ACTION partycheat(eosio::name _user);
    ACTION herocheat(eosio::name _user);
    ACTION setdata();
     
#pragma endregion

    //------------------------------------------------------------------------//
    //-------------------------Item_equipment_function------------------------//
    //------------------------------------------------------------------------//
#pragma region item system 


void sell_servant(eosio::name _user, uint32_t _index);
void sell_monster(eosio::name _user, uint32_t _index);
void sell_item(eosio::name _user, uint32_t _index);
ACTION sellobject(eosio::name _user, uint32_t _type, uint32_t _index);

ACTION equipment(eosio::name _user, uint32_t _type ,uint32_t _servant_index ,uint32_t _item_index);
ACTION unequipment(eosio::name _user, uint32_t _type ,uint32_t _servant_index ,uint32_t _item_index);
void unequip_servant(eosio::name _user, uint32_t _servant_index ,uint32_t _item_index);
void equip_servant(eosio::name _user, uint32_t _servant_index ,uint32_t _item_index);
void unequip_hero(eosio::name _user, uint32_t _item_index);
void equip_hero(eosio::name _user, uint32_t _item_index);

#pragma endregion





    //------------------------------------------------------------------------//
    //---------------------- -------battle_state_table-------------------------//
    //------------------------------------------------------------------------//
#pragma region battle state table

enum battle_action_state
{
    wait = 0,
    attack,
    defense,
    state_count,
};

enum battle_buff_state
{
    none = 0,
    sleep,
    poison,
    strength_collect_wait,
};

enum active_skill_list
{
    none_active = 0,
    double_attack, //더블 어택 - 15% 확률로 2번 공격한다.
    all_attack,    //참격 - 적 앞라인 전체 적을 공격력의 35%로 공격한다.
    heal,          //치유 - 대상 아군을 지능 수치의 100%만큼 치유한다.
    no_think,      //무아지경 - 공격력의 50%로 랜덤한 적 4개체 타격 (같은 개체 타격 가능)
    poison_arrow,  //독화살 - 적에게 공격력 40%의 데미지를 입히고, 상태이상 : 중독을 건다 (중독 : 4턴 동안 턴 마다 공격력의 30%의 데미지를 입힌다.)
    active_skill_count,
};

enum passive_skill_list
{
    none_passive = 0,
    iron_wall,      //철벽 - 방어율 10% 상승 (방어력 아님)
    blood_attack,   //피의 일격 - 치명타 시 데미지 10%만큼 회복
    deceiver,       //기만자 - HP 100%일 때 자신의 속도 10 증가
    strength_flag,  //힘의 깃발 - 자신의 양 옆 캐릭터에게 공격력 15% 상승 버프를 부여
    sniper,         //저격수 - 자신의 앞에 캐릭터가 존재할 경우 공격력 50% 증가
    strength_collect,//힘모으기 - 방어시 다음 턴 공격력 25% 증가
    passive_skill_count,
};

// 4 + 4 + 4 + 4 + 4 + 4 + 8 + sbattle_member_state(9) = 41
// sbattle_member_state 당 9 총 5개의 버프창이 있으면 45 + 32 = 77
private:
    const uint32_t warrior_speed = 34;
    const uint32_t wizard_speed = 29;
    const uint32_t priest_speed = 32;
    const uint32_t archer_speed = 42;
    const uint32_t thief_speed = 50;
    const uint32_t beginner_speed = 30;

  private:
    const uint32_t oper_hp = 220;
    const uint32_t oper_attack = 20;
    const uint32_t oper_defense = 10;
    const uint32_t oper_critical = 5;
    const uint32_t oper_critical_damage = 15000;
    const uint32_t oper_avoid = 5;
    const uint32_t defense_constant = 200;
    
    const uint32_t decimal = 100;
    const uint32_t max_battle_member_count = 20;
    const uint32_t max_party_count = 10;
    
  public:
    struct battle_state
    {
        uint32_t party_position;
        uint32_t index;
        uint32_t now_hp;
        uint32_t attack;
        uint32_t defense;
        uint32_t crit_per;
        uint32_t crit_dmg;
        uint32_t avoid;
        uint32_t state;
        uint32_t speed;
    };

    TABLE tstagestate
    {
        eosio::name user;
        uint64_t stage_type; // Tower / Field 1~5
        eosio::name enemy_user; // if : Tower = account Name / else : untowermain1
        uint64_t stage_number;
        std::vector<battle_state> my_state_list;
        std::vector<battle_state> enemy_state_list;

        uint64_t primary_key() const { return user.value; }
    };

    typedef eosio::multi_index<"tstagestate"_n, tstagestate> battle_state_list;
#pragma endregion

    
    //------------------------------------------------------------------------//
    //---------------------------battle_action_table--------------------------//
    //------------------------------------------------------------------------//
#pragma region battle action table

struct battle_order_struct
{
    uint32_t speed;
    uint32_t key;
    uint32_t party_position;
    uint32_t second_speed;
};

struct battle_action
{
     uint32_t target_position = 0;
     uint32_t avoid = 0;
     uint32_t critical = 0;
     uint32_t damage = 0;
};

struct battle_action_info
{
    uint32_t my_position = 0;
    uint32_t action_type = 0;
    std::vector<battle_action> battle_action_list;
};

TABLE tbattleact
{
    eosio::name user;
    uint32_t turn;
    std::vector<battle_action_info> battle_info_list;
    uint64_t primary_key() const { return user.value; }
};
typedef eosio::multi_index<"tbattleact"_n, tbattleact> battle_actions;
#pragma endregion

enum db_battle_choice
{
    db_stage_num =1,
    db_stage_monster,    
};

struct stage_monster_info
{
    uint64_t index;
    uint64_t level;
    status_info status;
    uint64_t count;
    std::vector<uint64_t> position_list;
};

struct stage_info
{
    uint64_t index;
    std::vector<stage_monster_info> monster_list;
};

TABLE dbstage
{
    uint64_t stage_type;
    std::vector<stage_info> enemy_list;
    uint64_t primary_key()const { return stage_type; }
   
};
typedef eosio::multi_index<"dbstage"_n, dbstage> stage_db;



    //------------------------------------------------------------------------//
    //---------------------------battle_reward_table--------------------------//
    //------------------------------------------------------------------------//
#pragma region battle action table
TABLE tclearreward
{
    eosio::name user;
    uint64_t reward_money;
    std::vector<uint32_t> get_exp_list;
    std::vector<servant_info> get_servant_list;
    std::vector<monster_info> get_monster_list;
    std::vector<item_info> get_item_list;
    uint64_t primary_key() const { return user.value; }
};
typedef eosio::multi_index<"tclearreward"_n, tclearreward> battle_reward_list;
#pragma endregion
    //------------------------------------------------------------------------//
    //-------------------------------battle_function--------------------------//
    //------------------------------------------------------------------------//
#pragma region battle function 

uint32_t get_attack(uint32_t _job, status_info _status);
uint32_t get_speed(uint32_t _job);
uint64_t get_damage(uint32_t _atk, uint32_t _dfs);
uint32_t get_buff_turn(uint32_t _buff);
bool check_critical(uint64_t _critcal_per, uint64_t _seed);
bool check_avoid(uint64_t _avoid_per,uint64_t _seed);

battle_action get_attack_action(const std::vector<battle_state> &_my_state_list, const std::vector<battle_state> &_enemy_state_list,uint64_t _seed, uint64_t _my_position, uint64_t _target_position);

ACTION startbattle(eosio::name _user, uint32_t _party_number, uint32_t _stage);

int get_random_target(const std::vector<battle_state> &_enemy_state_list, uint64_t _seed, uint32_t _max, uint32_t _min);
int get_target_key(const std::vector<battle_state> &_enemy_state_list,  uint64_t _target_position);

static bool sort_compare(const battle_order_struct &a,const battle_order_struct &b);
ACTION activeturn(eosio::name _user, uint32_t _hero_action, uint32_t _monster_action, uint32_t _hero_target, uint32_t _monster_target, std::string _seed);
void win_reward(eosio::name _user);
void fail_reward(eosio::name _user);


ACTION exitbattle(eosio::name _user);
ACTION getreward(eosio::name _user);

//테스트용 함수
ACTION deletebattle(eosio::name _user);
ACTION deleteuser(eosio::name _user);

#pragma endregion


};
