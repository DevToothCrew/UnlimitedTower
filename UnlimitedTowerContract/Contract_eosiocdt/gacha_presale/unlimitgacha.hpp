#include "Common/common_header.hpp"

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

#pragma region db cdbbody
    TABLE cdbbody
    {
        uint64_t body;
        uint64_t primary_key() const { return body; }
    };
    typedef eosio::multi_index<"cdbbody"_n, cdbbody> body_db;
#pragma endregion

#pragma region db cdbhair
    TABLE cdbhair
    {
        uint64_t hair;
        uint64_t primary_key() const { return hair; }
    };
    typedef eosio::multi_index<"cdbhair"_n, cdbhair> hair_db;
#pragma endregion

#pragma region db cdbhead
    TABLE cdbhead
    {
        uint64_t head;
        uint64_t primary_key() const { return head; }
    };
    typedef eosio::multi_index<"cdbhead"_n, cdbhead> head_db;
#pragma endregion

#pragma region db cdbitemgrade
    TABLE cdbitemgrade
    {
        uint64_t grade;
        object_status min_range;
        object_status max_range;
        uint64_t primary_key() const { return grade; }
    };
    typedef eosio::multi_index<"cdbitemgrade"_n, cdbitemgrade> item_grade_db;
#pragma endregion

#pragma region db cdbitemid
    TABLE cdbitemid
    {
        uint64_t id;
        uint64_t slot;
        uint64_t job;
        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"cdbitemid"_n, cdbitemid> item_id_db;
#pragma endregion

#pragma region db cdbitemtier
    TABLE cdbitemtier
    {
        uint64_t tier;
        uint64_t level;
        uint64_t primary_key() const { return tier; }
    };
    typedef eosio::multi_index<"cdbitemtier"_n, cdbitemtier> item_tier_db;
#pragma endregion

#pragma region db cdbmgrade
    TABLE cdbmgrade
    {
        uint64_t monster_grade;
        object_status min_range;
        object_status max_range;
        uint64_t primary_key() const { return monster_grade; }
    };
    typedef eosio::multi_index<"cdbmgrade"_n, cdbmgrade> monster_grade_db;
#pragma endregion

#pragma region db cdbmonsterid
    TABLE cdbmonsterid
    {
        uint64_t look;
        uint64_t primary_key() const { return look; }
    };
    typedef eosio::multi_index<"cdbmonsterid"_n, cdbmonsterid> monster_id_db;
#pragma endregion

#pragma region db cdbservant
    TABLE cdbservant
    {
        uint64_t job;
        object_status min_range;
        object_status max_range;
        uint64_t primary_key() const { return job; }
    };
    typedef eosio::multi_index<"cdbservant"_n, cdbservant> servant_db;
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
    void delete_user_balance(uint64_t _user);
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

#pragma region gacha cservant
    struct servant_info
    {
        uint32_t state;   //서번트 상태
        uint32_t exp = 0; //서번트 경험치
        uint32_t job;
        uint32_t stat_point = 0;
        appear_info appear;               //서번트 외형 정보
        status_info status;               //기본 힘,민,지 추가 힘,민,지
        std::vector<uint32_t> equip_slot; //서번트 장비 리스트
    };

    TABLE cservant
    {
        uint64_t index;
        uint32_t party_number;
        servant_info servant;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"cservant"_n, cservant> user_servants;
#pragma endregion

#pragma region gacha cmonster
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

    TABLE cmonster
    {
        uint64_t index;
        uint32_t party_number;
        monster_info monster;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"cmonster"_n, cmonster> user_monsters;
#pragma endregion

#pragma region gacha citem
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

    TABLE citem
    {
        uint64_t index;
        item_info item;
        uint64_t primary_key() const { return index; }
    };
    typedef eosio::multi_index<"citem"_n, citem> user_items;
#pragma endregion

#pragma region gacha cgacharesult
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

    TABLE cgacharesult
    {
        eosio::name user;
        result_info result;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"cgacharesult"_n, cgacharesult> user_gacha_results;
#pragma endregion

#pragma region gacha caccumulate

    TABLE caccumulate
    {
        eosio::name user;
        std::vector<result_info> result_list;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"caccumulate"_n, caccumulate> user_gacha_accumulates;

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
#pragma endregion


#pragma region gacha cheat
    void gacha_cheat(eosio::name _user);
#pragma endregion

    //------------------------------------------------------------------------//
    //-----------------------------login_table--------------------------------//
    //------------------------------------------------------------------------//
  private:
#pragma region login enum
    enum euser_state
    {        
        freesale = 1,       //owen change 
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
        on_freesale =1,
        on_inventory,
        on_equip_slot,
        on_party,
        on_tower,
        object_presale,
    };
#pragma endregion

#pragma region login table cuserauth
    //struct hero_info
    struct hero_info
    {
        uint32_t state;   //히어로 상태
        uint32_t exp = 0; //히어로 경험치
        uint32_t job;
        uint32_t stat_point = 0;
        appear_info appear;               //히어로 외형 정보
        status_info status;               //기본 힘,민,지 추가 힘,민,지
        std::vector<uint32_t> equip_slot; //히어로 장비 리스트
    };

    TABLE cuserauth
    {
        eosio::name user;
        uint32_t game_money = 100;
        uint32_t state = euser_state::lobby;
        hero_info hero;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"cuserauth"_n, cuserauth> auth_users;

#pragma endregion
//------------------------------------------------------------------------//
//--------------------------freesale_log_table----------------------------//
//------------------------------------------------------------------------//
#pragma region freesale table log
TABLE freesalelog
{   
    eosio::name owner;
    uint64_t gacha_participation;
    uint64_t primary_key() const { return owner.value; }
};
    typedef eosio::multi_index<"freesalelog"_n, freesalelog> participation_logs;
#pragma endregion
//------------------------------------------------------------------------//
//-----------------------------------log_table----------------------------//
//------------------------------------------------------------------------//
#pragma region login table cuserlog
    TABLE cuserlog
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
    typedef eosio::multi_index<"cuserlog"_n, cuserlog> user_logs;
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
void delete_user_data(uint64_t _user);
void delete_user_object_data(uint64_t _user);
void delete_user_gacha_result_data(uint64_t _user);

ACTION deleteuser(eosio::name _user);


};