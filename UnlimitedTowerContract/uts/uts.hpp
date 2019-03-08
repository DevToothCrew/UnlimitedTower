#pragma once

#include <eosiolib/eosio.hpp>
#include <eosiolib/asset.hpp>
#include <string>

namespace eosio {
    using std::string;
    typedef uint64_t id_type;

    class uts : public contract {
    public:
        uts(account_name self) : contract(self), tokens(_self, _self) {}

        //@abi action
        void create(account_name issuer, string symbol);

        //@abi action
        void issue(account_name to, asset quantity, uint64_t index);

        //@abi action
        void transferid(account_name from, account_name to, id_type id );

	    // @abi table accounts i64
        struct account {

            asset balance;

            uint64_t primary_key() const { return balance.symbol.name(); }
        };

        // @abi table stat i64
        struct stats {
            asset supply;
            account_name issuer;

            uint64_t primary_key() const { return supply.symbol.name(); }
            account_name get_issuer() const { return issuer; }
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
        
        struct servant_info
        {
            uint32_t id;
            uint32_t state; //서번트 상태
            uint32_t exp = 0; //서번트 경험치
            uint32_t stat_point = 0;
            status_info status;    //기본 힘,민,지 추가 힘,민,지
            std::vector<uint32_t> equip_slot; //서번트 장비 리스트
        };

        // @abi table utstokens i64
        class utstoken {
            public: 
                id_type idx;          // Unique 64 bit identifier,
                uint32_t t_idx;       // 유저 테이블 상에서의 고유 인덱스
                uint32_t s_idx;       // 서번트를 구분하기 위한 서번트 인덱스
                uint32_t state;
                uint32_t exp;
                uint32_t stat_point;
                status_info status;
                std::vector<uint32_t> equip_slot;

                account_name owner;  // token owner
                asset value;         // token value (1 SYS)

                id_type primary_key() const { return idx; }
                account_name get_owner() const { return owner; }
        };

        // @abi table tservant i64
        struct tservant {
            uint64_t index;
            uint32_t id;
            status_info status;
            
            uint64_t primary_key() const { return index; }
        };

	    using account_index = eosio::multi_index<N(accounts), account>;

	    using currency_index = eosio::multi_index<N(stat), stats,
	                       indexed_by< N( byissuer ), const_mem_fun< stats, account_name, &stats::get_issuer> > >;

	    using token_index = eosio::multi_index<N(utstokens), utstoken,
	                    indexed_by< N( byowner ), const_mem_fun< utstoken, account_name, &utstoken::get_owner> >>;

        using servant_table = eosio::multi_index<N(tservant), tservant>;

	    token_index tokens;

        void sub_balance(account_name owner, asset value);
        void add_balance(account_name owner, asset value, account_name ram_payer);
        void sub_supply(asset quantity);
        void add_supply(asset quantity);
    };

} /// namespace eosio