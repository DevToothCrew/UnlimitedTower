#pragma once
#include "../common_header.hpp"



class [[eosio::table]] caccounthero
{
    private:
        uint64_t owner;
        uint32_t h_index;
        uint32_t h_exp;
        uint32_t h_job;
        uint32_t h_strength;
        uint32_t h_dexterity;
        uint32_t h_intelligence;
    public:
        uint64_t primary_key()const {return owner;}
        void set_owner(uint64_t _user) {owner = _user; }
        void add_exp(uint32_t _exp){h_exp+=_exp;}
        void add_strength(uint32_t _str){h_strength+=_str;}
        void add_dexterity(uint32_t _dex){h_dexterity+=_dex;}
        void add_intelligence(uint32_t _int){h_intelligence+=_int;}
        uint32_t get_hero_index()const {return h_index;}
        void set_rand_hero_ability(uint32_t _index)
        {
            h_index = _index;
            h_exp = 0;
            h_job = 1;
            h_strength = 10 +_index;
            h_dexterity = 10 +_index;
            h_intelligence = 10 + _index;
        }
        EOSLIB_SERIALIZE(
            caccounthero,
            (owner)
            (h_index)
            (h_exp)
            (h_job)
            (h_strength)
            (h_dexterity)
            (h_intelligence)
        )
};
typedef multi_index<name("caccounthero"), caccounthero> account_hero_list_table;


//@abi table chero i64
class chero
{
    private:
        uint32_t h_index;
        uint32_t h_exp;
        uint32_t h_job;
        uint32_t h_strength;
        uint32_t h_dexterity;
        uint32_t h_intelligence;
    public:
        void add_exp(uint32_t _exp){h_exp+=_exp;}
        void add_strength(uint32_t _str){h_strength+=_str;}
        void add_dexterity(uint32_t _dex){h_dexterity+=_dex;}
        void add_intelligence(uint32_t _int){h_intelligence+=_int;}
        uint32_t primary_key()const {return h_index;}
        uint32_t get_hero_index()const {return h_index;}
        void set_rand_hero_ability(uint32_t _index)
        {
            h_index = _index;
            h_exp = 0;
            h_job = 1;
            h_strength = 10 +_index;
            h_dexterity = 10 +_index;
            h_intelligence = 10 + _index;
        }
        EOSLIB_SERIALIZE(
            chero,
            (h_index)
            (h_exp)
            (h_job)
            (h_strength)
            (h_dexterity)
            (h_intelligence)
        )
};
typedef multi_index<name("chero"), chero> hero_list_table;