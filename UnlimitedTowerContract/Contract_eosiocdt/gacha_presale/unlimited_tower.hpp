#include "Common/common_header.hpp"

CONTRACT unlimited_tower : public contract
{
#pragma resgion contract constructor
  public:
    using contract::contract;
    name _owner;
    unlimited_tower(eosio::name _self, eosio::name _code, datastream<const char *> ds) : contract(_self, _code, ds)
    {
        _owner = _self;
    }
#pragma endregion
//------------------------------------------------------------------------//
//-----------------------------------DB-----------------------------------//
//------------------------------------------------------------------------//
#pragma region cdbbody
TABLE cdbbody
{
    uint64_t body;
    uint64_t primary_key() const {return body;}
};
typedef eosio::multi_index<"cdbbody"_n,cdbbody> body_db;
#pragma endregion

#pragma region cdbhair
TABLE cdbhair
{
    uint64_t hair;
    uint64_t primary_key() const {return hair;}
};
typedef eosio::multi_index<"cdbhair"_n,cdbhair> hair_db;
#pragma endregion

#pragma region cdbhead
TABLE cdbhead
{
    uint64_t head;
    uint64_t primary_key() const { return head; }
};
typedef eosio::multi_index<"cdbhead"_n,cdbhead> head_db;
#pragma endregion

#pragma region cdbitemgrade
TABLE cdbitemgrade
{
    uint64_t grade;
    object_status min_range;
    object_status max_range;
    uint64_t primary_key() const {return grade;}
};
typedef eosio::multi_index<"cdbitemgrade"_n,cdbitemgrade> item_grade_db;
#pragma endregion

#pragma region cdbitemid
TABLE cdbitemid
{
    uint64_t id;
    uint64_t slot;
    uint64_t job;
    uint64_t primary_key() const {return id;}
};
typedef eosio::multi_index<"cdbitemid"_n,cdbitemid> item_id_db;
#pragma endregion

#pragma region cdbitemtier
TABLE cdbitemtier
{
    uint64_t tier;
    uint64_t level;
    uint64_t primary_key() const {return tier;}
};
typedef eosio::multi_index<"cdbitemtier"_n,cdbitemtier> item_tier_db;
#pragma endregion

#pragma region cdbmgrade
TABLE cdbmgrade
{
    uint64_t monster_grade;
    object_status min_range;
    object_status max_range;
    uint64_t primary_key() const {return monster_grade;}
};
typedef eosio::multi_index<"cdbmgrade"_n,cdbmgrade> monster_grade_db;
#pragma endregion

#pragma region cdbmonsterid
TABLE cdbmonsterid
{
    uint64_t look;
    uint64_t primary_key() const {return look;}
};
typedef eosio::multi_index<"cdbmonsterid"_n,cdbmonsterid> monster_id_db;
#pragma endregion

#pragma region cdbservant
struct object_status
{
    uint64_t base_str;
    uint64_t base_dex;
    uint64_t base_int;
};

TABLE cdbservant
{
    uint64_t job;
    object_status min_range;
    object_status max_range;
    uint64_t primary_key() const {return job;}
};
typedef eosio::multi_index<"cdbservant"_n,cdbservant> servant_db;
#pragma endregion

#pragma region db action
    ACTION setdata();
    ACTION initdata();
#pragma endregion



#pragma endregion
//------------------------------------------------------------------------//
//-----------------------------------token--------------------------------//
//------------------------------------------------------------------------//
#pragma region token action
  public:
    ACTION create(name issuer, asset maximum_supply);
    ACTION issue(name to, asset quantity, string memo);
    ACTION transfer(name from, name to, asset quantity, string memo);
#pragma endregion

#pragma region token table
  private:
    TABLE account
    {
        asset balance;
        uint64_t primary_key() const { return balance.symbol.code().raw(); }
    };

    TABLE stats
    {
        asset supply;
        asset max_supply;
        name issuer;
        uint64_t primary_key() const { return supply.symbol.code().raw(); }
    };

    typedef eosio::multi_index<"account"_n, firstToken::account> accounts;
    typedef eosio::multi_index<"stats"_n, stats> stat;
#pragma endregion

#pragma region token function
    void sub_balance(name owner, asset value);
    void add_balance(name owner, asset value, name ram_payer);
#pragma endregion

#pragma region token delete init
    void delete_user_balance(name _user);
    void init_stat(asset _token);
    void init_all_balance();
#pragma endregion
//------------------------------------------------------------------------//
//-----------------------------------gacha--------------------------------//
//------------------------------------------------------------------------//
    private:
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
    uint64_t primary_key() const {return user.value;}
};
typedef eosio::multi_index<"caccumulate"_n,caccumulate> user_gacha_accumulates;

#pragma endregion

#pragma region gacha values
    private:
        user_gacha_results user_gacha_result_table(_self, _self.value);
        user_gacha_accumulates user_gacha_accumulate_table(_self, _self.value);

        uint32_t servant_random_count;
        uint32_t monster_random_count;
        uint32_t item_random_count;
    private:
      const uint32_t default_min = 0;
      const uint32_t max_rate = 100000;
      const uint32_t grade_three_rate = 89000;
      const uint32_t grade_four_rate = 9000;
      const uint32_t grade_five_rate = 2000;
#pragma endregion

    public:
#pragma region gacha function
        void gacha_servant_job(eosio::name _user,uint64_t _seed);
        uint8_t gacha_servant_head(uint64_t _seed,uint32_t _count);
        uint8_t gacha_servant_hair(uint64_t _seed,uint32_t _count);
        uint8_t gacha_servant_body(uint64_t _seed,uint32_t _count);
        void gacha_monster_id(eosio::name _user,uint64_t _seed);
        void gacha_item_id(eosio::name _user,uint64_t _seed);
        void start_gacha(eosio::name _user);
#pragma endregion

#pragma region init 
	    void reset_all_user_object_data(eosio::name _user);
        void reset_user_gacha_result_data(eosio::name _user)
        void reset_all_object_gacha_data();
        void delete_object_data(eosio::name _user);
#pragma endregion

#pragma region gacha cheat
        void gacha_cheat(eosio::name _user);
#pragma endregion
    };





};