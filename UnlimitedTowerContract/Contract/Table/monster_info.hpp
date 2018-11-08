#pragma once
#include "../Common/common_header.hpp"


class [[eosio::table]] cmonster
{
    private:
        uint32_t m_index;
        uint32_t m_exp;
        uint32_t m_job;
        uint32_t m_strength;
        uint32_t m_dexterity;
        uint32_t m_intelligence;
    public:
        void add_exp(uint32_t _exp){m_exp+=_exp;}
        void add_strength(uint32_t _str){m_strength+=_str;}
        void add_dexterity(uint32_t _dex){m_dexterity+=_dex;}
        void add_intelligence(uint32_t _int){m_intelligence+=_int;}
        uint32_t primary_key()const {return m_index;}
        uint32_t get_monster_index()const {return m_index;}
        void set_rand_monster_ability(uint32_t _index)
        {
            m_index = _index;
            m_exp = 0;
            m_job = 1;
            m_strength = 10 +_index;
            m_dexterity = 10 +_index;
            m_intelligence = 10 + _index;
        }
        
        EOSLIB_SERIALIZE(
            cmonster,
            (m_index)
            (m_exp)
            (m_job)
            (m_strength)
            (m_dexterity)
            (m_intelligence)
        )
};

typedef multi_index<name("cmonster"), cmonster> monster_list_table;   