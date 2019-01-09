#include "Common/common_header.hpp"

//즐겨찾기 
//테이블 
//db_table, token_table, gacha_table, login_table
//시스템 
//db_system, token_system, login_system, gacha_system, init_function, delete_function, free_sale_function
//로그
//freesale_log_table, user_log_table

CONTRACT unlimitgacha : public contract
{
  public:
#pragma resgion contract constructor
    //using contract::contract;
    eosio::name owner;
    unlimitgacha(eosio::name _self, eosio::name _code, datastream<const char *> ds) : contract(_self, _code, ds)
    {
        owner = _self;
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
        uint64_t slot;
        uint64_t job;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"dbitemid"_n, dbitemid> item_id_db;
#pragma endregion

#pragma region db dbitemtier
    TABLE dbitemtier
    {
        uint64_t tier;
        uint64_t level;
        uint64_t primary_key() const { return tier; }
    };
    typedef eosio::multi_index<"dbitemtier"_n, dbitemtier> item_tier_db;
#pragma endregion

#pragma region db dbmgrade
    TABLE dbmgrade
    {
        uint64_t monster_grade;
        object_status min_range;
        object_status max_range;
        uint64_t primary_key() const { return monster_grade; }
    };
    typedef eosio::multi_index<"dbmgrade"_n, dbmgrade> monster_grade_db;
#pragma endregion

#pragma region db dbmonsterid
    TABLE dbmonsterid
    {
        uint64_t look;
        uint64_t primary_key() const { return look; }
    };
    typedef eosio::multi_index<"dbmonsterid"_n, dbmonsterid> monster_id_db;
#pragma endregion

#pragma region db dbservant
    TABLE dbservant
    {
        uint64_t job;
        object_status min_range;
        object_status max_range;
        uint64_t primary_key() const { return job; }
    };
    typedef eosio::multi_index<"dbservant"_n, dbservant> servant_db;
#pragma endregion

    //------------------------------------------------------------------------//
    //----------------------------db_system-----------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region db values
    const uint8_t servant_job_count = 6;
    const uint8_t monster_id_count = 30;
    const uint8_t monster_grade_count = 5;
    const uint8_t item_id_count = 70;
    const uint8_t item_tier_count = 4;
    const uint8_t item_grade_count = 5;
    const uint8_t item_slot_count = 3;
    const uint8_t head_count = 3;
    const uint8_t hair_count = 3;
    const uint8_t body_count = 4;
    uint32_t random_count = 0;
#pragma endregion

  public:
#pragma region db action
    ACTION setdata();
    ACTION initdata();
    ACTION setfreesale();
#pragma endregion

  public:
#pragma region db function

#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------token_table--------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region token account
  private:
    TABLE account
    {
        asset balance;
        uint64_t primary_key() const { return balance.symbol.code().raw(); }
    };
    typedef eosio::multi_index<"account"_n, account> accounts;
#pragma endregion

#pragma region token stats
    TABLE stats
    {
        asset supply;
        asset max_supply;
        eosio::name issuer;
        uint64_t primary_key() const { return supply.symbol.code().raw(); }
    };
    typedef eosio::multi_index<"stats"_n, stats> stat;
#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------token_system--------------------------------//
    //------------------------------------------------------------------------//
  public:
#pragma region token function
    void sub_balance(name _user, asset _value);
    void add_balance(name _user, asset _value, name _ram_payer);
#pragma endregion

#pragma region token action
    ACTION create(name _issuer, asset _maximum_supply);
    ACTION issue(name _to, asset _quantity, string _memo);
    ACTION tokentrans(name _from, name _to, asset _quantity, string _memo);
    ACTION inittoken(asset _token);
#pragma endregion



#pragma region token delete init
    void delete_user_balance(eosio::name _user);
    void init_stat(asset _token);
    void init_all_balance();
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
        uint32_t job;   
        uint32_t stat_point = 0;
        appear_info appear;               //서번트 외형 정보
        status_info status;               //기본 힘,민,지 추가 힘,민,지
        std::vector<uint32_t> equip_slot; //서번트 장비 리스트
    };

    TABLE tservant
    {
        uint64_t index;
        uint32_t party_number;
        servant_info servant;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"tservant"_n, tservant> user_servants;
    typedef eosio::multi_index<"freeservant"_n, servant> user_free_sale_servants;
#pragma endregion

#pragma region gacha tmonster
    struct monster_info
    {
        uint32_t state;    //몬스터 상태값
        uint32_t exp = 0;  //경험치
        uint32_t type = 0; //속성 타입
        uint32_t look;
        uint32_t grade;       // 등급
        uint32_t upgrade = 0; //강화수치
        status_info status;   //기본 힘,민,지 추가 힘,민,지
    };

    TABLE tmonster
    {
        uint64_t index;
        uint32_t party_number;
        monster_info monster;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"tmonster"_n, tmonster> user_monsters;
#pragma endregion

#pragma region gacha titem
    struct item_info
    {
        uint32_t state;       //아이템 현재 상태
        uint32_t id;          //아이템 리소스 아이디
        uint32_t slot;        //장착 타입
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
    typedef eosio::multi_index<"freeitem"_n, titem> user_free_sale_items;
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

#pragma region gacha taccumulate

    TABLE taccumulate
    {
        eosio::name user;
        std::vector<result_info> result_list;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"taccumulate"_n, taccumulate> user_gacha_accumulates;

#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------gacha_system--------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region gacha values
    const char *action_gacha="gacha";

    uint32_t servant_random_count;
    uint32_t monster_random_count;
    uint32_t item_random_count;

    const uint32_t default_min = 0;
    const uint32_t max_rate = 100;
    const uint32_t two_grade_ratio = 89;
    const uint32_t three_grade_ratio = 9;
    const uint32_t four_grade_ratio = 2;

    const uint64_t limit_token_amount = 150000000;

#pragma endregion

  public:
#pragma region gacha function
    void gacha_servant_job(eosio::name _user, uint64_t _seed);
    uint8_t gacha_servant_head(uint64_t _seed, uint32_t _count);
    uint8_t gacha_servant_hair(uint64_t _seed, uint32_t _count);
    uint8_t gacha_servant_body(uint64_t _seed, uint32_t _count);
    void gacha_monster_id(eosio::name _user, uint64_t _seed);
    void gacha_item_id(eosio::name _user, uint64_t _seed);
    
    uint64_t get_user_seed_value(uint64_t _user);

    void start_gacha(eosio::name _user, uint64_t _seed);
//-----------------------------free_sale_function--------------------------------//
    void freesale_gacha_servant_job(eosio::name _user, uint64_t _seed);
    void freesale_gacha_monster_id(eosio::name _user, uint64_t _seed);
    void freesale_gacha_item_id(eosio::name _user, uint64_t _seed);

    void freesale_gacha(eosio::name _user, uint64_t _seed);

#pragma endregion


#pragma region gacha cheat
    void gacha_cheat(eosio::name _user);
#pragma endregion

//------------------------------------------------------------------------//
//--------------------------freesale_log_table----------------------------//
//------------------------------------------------------------------------//
#pragma region freesale table log
TABLE freesalelog
{   
    eosio::name owner;
    uint64_t gacha_participation = 0;
    uint64_t accumulate_token_amount = 0;
    uint64_t primary_key() const { return owner.value; }
};
    typedef eosio::multi_index<"freesalelog"_n, freesalelog> participation_logs;
#pragma endregion
//------------------------------------------------------------------------//
//------------------------------user_log_table----------------------------//
//------------------------------------------------------------------------//
#pragma region login table tuserlog
    TABLE tuserlog
    {
        eosio::name user;
        uint32_t servant_num = 0;
        uint32_t monster_num = 0;
        uint32_t item_num = 0;
        uint32_t gacha_num = 0;
        uint32_t login_time = 0;
        uint32_t get_gold = 0;
        //asset get_eos = eosio::asset(0, symbol(symbol_code("EOS"),4));;
        uint32_t use_gold = 0;
        //asset use_eos = eosio::asset(0, symbol(symbol_code("EOS"),4)); 
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
#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------login_table--------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region login enum
    enum euser_state
    {        
        freesale = 1,        
        lobby,
        battle,
        battle_win,
        battle_lose,
        tower,
        travel,
        pause,
        black,
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

    enum eobject_state
    {
        on_inventory = 1,
        on_equip_slot,
        on_party,
        on_tower,
        object_presale,
    };
#pragma endregion

#pragma region login table tuserauth
    //struct hero_info
    struct hero_info
    {
        uint32_t state;                   //히어로 상태
        uint32_t exp = 0;                 //히어로 경험치
        uint32_t job;
        uint32_t stat_point = 0;
        appear_info appear;               //히어로 외형 정보
        status_info status;               //기본 힘,민,지 추가 힘,민,지
        std::vector<uint32_t> equip_slot; //히어로 장비 리스트
    };

    TABLE tuserauth
    {
        eosio::name user;
        uint32_t game_money = 100;
        uint32_t state = euser_state::lobby;
        hero_info hero;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"tuserauth"_n, tuserauth> auth_users;

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
    const uint32_t max_charcterslot = 3;
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
    ACTION freesalesign(eosio::name _user);
    ACTION signup(eosio::name _user);
#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------init_function------------------------------//
    //------------------------------------------------------------------------//
void init_all_user_auth_data();
void init_all_user_log_data();
void init_all_object_gacha_data();
ACTION initalluser();
void init_freesale_log();
ACTION initfreelog();


    //------------------------------------------------------------------------//
    //-----------------------------delete_function----------------------------//
    //------------------------------------------------------------------------//
void delete_user_data(eosio::name _user);
void delete_user_object_data(eosio::name _user);
void delete_user_gacha_result_data(eosio::name _user);
void delete_user_freesale_data(eosio::name _user);
ACTION deleteuser(eosio::name _user);


};