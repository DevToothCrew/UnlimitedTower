#pragma once

#include <eosiolib/eosio.hpp>
#include <eosiolib/asset.hpp>
#include <string>

using namespace eosio;

namespace devtooth_tower
{
using std::string;
typedef uint64_t id_type;

CONTRACT devtooth_tower : public contract
{
  public:
    devtooth_tower(name _self, name _code, datastream<const char *> ds) : contract(_self, _code, ds), floortable(_self, _self.value) {}

    ACTION changestate( name payer, uint64_t fnum);

    void resultwin(name winner, uint64_t fnum, uint64_t pnum);

    void resultlose(name loser, uint64_t fnum);

    ACTION endflag(name winner, uint64_t fnum);

    ACTION claim(name who, uint64_t fnum);

    // Structs for Servant table
    struct appear_info
    {
        uint32_t head = 0;
        uint32_t hair = 0;
        uint32_t body = 0;
        uint32_t gender = 0;
    };

    struct status_info
    {
        uint32_t basic_str = 0;
        uint32_t basic_dex = 0;
        uint32_t basic_int = 0;
        uint32_t plus_str = 0;
        uint32_t plus_dex = 0;
        uint32_t plus_int = 0;
    };

    // Structs for Hero table
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

    // Structs for Monster table
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

    // Structs for Item table
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

    // Init Party state 
    enum party_state
    {
        on_wait = 1,
        on_tower_defense,
    };

    TABLE account
    {

        asset balance;

        uint64_t primary_key() const { return balance.symbol.code().raw(); }
    };

    TABLE stats
    {
        asset supply;
        name issuer;

        uint64_t primary_key() const { return supply.symbol.code().raw(); }
        uint64_t get_issuer() const { return issuer.value; }
    };

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

    TABLE tnpc
    {
        uint64_t fnum;
        hero_info hero;
        uint64_t primary_key() const { return fnum; }
    };


    // Tables for get winner's party info
    TABLE tuserauth
    {
        eosio::name user;
        uint32_t state = 2;
        hero_info hero;
        uint64_t primary_key() const { return user.value; }
    };

    TABLE tservant
    {
        uint64_t index;
        uint32_t party_number = 0;
        servant_info servant;
        uint64_t primary_key() const { return index; }
    };

    TABLE tmonster
    {
        uint64_t index;
        uint32_t party_number = 0;
        monster_info monster;
        uint64_t primary_key() const { return index; }
    };

    TABLE titem
    {
        uint64_t index;
        item_info item;
        uint64_t primary_key() const { return index; }
    };

    TABLE tparty
    {
        uint32_t index;
        uint32_t state = party_state::on_wait;
        std::vector<uint64_t> servant_list;
        std::vector<uint64_t> monster_list;
        uint64_t primary_key() const {return index;}
    };
    
    // For Use 
    typedef eosio::multi_index<"accounts"_n, account> account_index;
    typedef eosio::multi_index<"stat"_n, stats, indexed_by<"byissuer"_n, const_mem_fun<stats, uint64_t, &stats::get_issuer>>> currency_index;
    typedef eosio::multi_index<"floorinfos"_n, floorinfo> floor_index;
    typedef eosio::multi_index<"tnpc"_n, tnpc> npc_index;

    // For Get data
    typedef eosio::multi_index<"tuserauth"_n, tuserauth> auth_users;
    typedef eosio::multi_index<"tservant"_n, tservant> user_servants;
    typedef eosio::multi_index<"tmonster"_n, tmonster> user_monsters;
    typedef eosio::multi_index<"titem"_n, titem> user_items;
    typedef eosio::multi_index<"tpartylist"_n,tparty> user_partys;
    
    floor_index floortable;

    void sub_balance(name owner, asset value);
    void add_balance(name owner, asset value, name ram_payer);
    void sub_supply(asset quantity);
    void add_supply(asset quantity);
};

} // namespace eosio