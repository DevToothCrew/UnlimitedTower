#include "devtooth_tower.hpp"

namespace devtooth_tower {
    using std::string;
    using eosio::asset;

    void devtooth_tower::sub_balance( name owner, asset value ) 
    {
        account_index from_acnts( _self, owner.value );
        const auto& from = from_acnts.get( value.symbol.code().raw(), "no balance object found" );
        eosio_assert( from.balance.amount >= value.amount, "overdrawn balance" );


        if( from.balance.amount == value.amount ) {
            from_acnts.erase( from );
        } else {
            from_acnts.modify( from, owner, [&]( auto& a ) {
                a.balance -= value;
            });
        }
    }

    void devtooth_tower::add_balance( name owner, asset value, name ram_payer )
    {
        account_index to_accounts( _self, owner.value );
        auto to = to_accounts.find( value.symbol.code().raw() );
        if( to == to_accounts.end() ) {
            to_accounts.emplace( ram_payer, [&]( auto& a ){
                a.balance = value;
            });
        } else {
            to_accounts.modify( to, owner, [&]( auto& a ) {
                a.balance += value;
            });
        }
    }

    void devtooth_tower::sub_supply( asset quantity ) {
        auto symbol_name = quantity.symbol.code().raw();
        currency_index currency_table( _self, symbol_name );
        auto current_currency = currency_table.find( symbol_name );

        currency_table.modify( current_currency, _self, [&]( auto& currency ) {
            currency.supply -= quantity;
        });
    }

    void devtooth_tower::add_supply( asset quantity )
    {
        auto symbol_name = quantity.symbol.code().raw();
        currency_index currency_table( _self, symbol_name );
        auto current_currency = currency_table.find( symbol_name );

        currency_table.modify( current_currency, _self, [&]( auto& currency ) {
            currency.supply += quantity;
        });
    }

    ACTION devtooth_tower::changestate( name payer, uint64_t fnum)
    {
        const auto& f_iter = floortable.get(fnum, "Floor info does not exist");

        if (f_iter.state == "idle"){
            floortable.modify(f_iter, payer, [&](auto& floordata){
                floordata.state = "battle";
            });
        }

        else if( f_iter.state == "battle"){
            floortable.modify(f_iter, payer, [&](auto& floordata){
                floordata.state = "idle";
            });
        }
    }

    void devtooth_tower::resultwin(name winner, uint64_t fnum, uint64_t pnum)
    {
        const auto& f_iter = floortable.get(fnum, "Floor info does not exist");
        
        floortable.modify(f_iter, winner, [&](auto& floordata){
            floordata.owner = winner;
            floordata.state = "idle";
            floordata.bnum = floordata.bnum + 1;
            floordata.pnum = pnum;              // 패배시 유저의 어떤 파티인지 알기 위해 기록
            floordata.endtime = now();
        });
        /***********************/
        /**** 파티 정보 저장 ****/
        /***********************/

        // 우승자 히어로 저장
        auth_users user("unlimittest1"_n, "unlimittest1"_n.value);
        auto u_iter = user.get(winner.value, "Not exist user");

        npc_index npc(_self, _self.value);
        auto n_iter = npc.find(fnum);

        if(n_iter == npc.end()){
            npc.emplace(winner, [&](auto& data){
                data.hero = u_iter.hero;
            });
        }
        else{
            const auto& npc_iter = *n_iter;
            npc.modify(npc_iter, winner, [&](auto& data){
                data.hero = u_iter.hero;
            });
        }

        // 우승자 히어로 아이템 저장
        user_items uitem("unlimittest1"_n, winner.value);
        user_items nitem(_self, fnum);

        uint64_t i_idx = 0;
        
        auto ni_iter = nitem.find(i_idx);

        if(ni_iter == nitem.end()){
            for(auto iter = u_iter.hero.equip_slot.begin(); iter != u_iter.hero.equip_slot.end(); iter++){
                auto ui_iter = uitem.get(*iter, "Not exist item");

                nitem.emplace(winner, [&](auto& data){
                    data.index = i_idx;
                    data.item = ui_iter.item;
                });

                i_idx++;
            }
        }
        else{
            for(auto iter = u_iter.hero.equip_slot.begin(); iter != u_iter.hero.equip_slot.end(); iter++){
                auto ui_iter = uitem.get(*iter, "Not exist item");
                auto temp_iter = nitem.find(i_idx);
                const auto& st = *temp_iter;

                nitem.modify(st, winner, [&](auto& data){
                    data.index = i_idx;
                    data.item = ui_iter.item;
                });

                i_idx++;
            }
        } 

        // 우승자 파티 정보 저장
        user_partys party("unlimittest1"_n, winner.value);
        auto p_iter = party.get(pnum, "Not exist party.");

        user_servants uservant("unlimittest1"_n, winner.value);
        user_servants nservant(_self, fnum);

        user_monsters umonster("unlimittest1"_n, winner.value);
        user_monsters nmonster(_self, fnum);
        
        // 서번트 정보 이식
        uint64_t s_idx = 0;
        auto ns_iter = nservant.find(s_idx);

        if(ns_iter == nservant.end()){
            for(auto iter = p_iter.servant_list.begin(); iter != p_iter.servant_list.end(); iter++){
                auto us_iter = uservant.get(*iter, "Not exist servant");

                nservant.emplace(winner, [&](auto& data){
                    data.index = s_idx;
                    data.party_number = fnum;
                    data.servant = us_iter.servant;
                });

                s_idx++;

                // 서번트 아이템 복사
                for(auto item_iter = us_iter.servant.equip_slot.begin(); item_iter != us_iter.servant.equip_slot.end(); item_iter++){
                    auto ui_iter = uitem.get(*item_iter, "Not exist item");

                    nitem.emplace(winner, [&](auto& data){
                        data.index = i_idx;
                        data.item = ui_iter.item;
                    });

                    i_idx++;
                }
            }
        }
        else{
            for(auto iter = p_iter.servant_list.begin(); iter != p_iter.servant_list.end(); iter++){
                auto us_iter = uservant.get(*iter, "Not exist servant");
                auto temp_iter = nservant.find(s_idx);
                const auto& st = *temp_iter;

                nservant.modify(st, winner, [&](auto& data){
                    data.index = s_idx;
                    data.party_number = fnum;
                    data.servant = us_iter.servant;
                });

                s_idx++;

                // 서번트 아이템 복사
                for(auto item_iter = us_iter.servant.equip_slot.begin(); item_iter != us_iter.servant.equip_slot.end(); item_iter++){
                    auto ui_iter = uitem.get(*item_iter, "Not exist item");
                    auto temp_iter = nitem.find(i_idx);
                    const auto& st = *temp_iter;

                    nitem.modify(st, winner, [&](auto& data){
                        data.index = i_idx;
                        data.item = ui_iter.item;
                    });

                    i_idx++;
                }
            }
        }

        // 몬스터 정보 이식
        uint64_t m_idx = 0;
        auto nm_iter = nmonster.find(m_idx);

        if(nm_iter == nmonster.end()){
            for(auto iter = p_iter.monster_list.begin(); iter != p_iter.monster_list.end(); iter++){
                auto um_iter = umonster.get(*iter, "Not exist servant");

                nmonster.emplace(winner, [&](auto& data){
                    data.index = m_idx;
                    data.party_number = fnum;
                    data.monster = um_iter.monster;
                });

                m_idx++;
            }
        }
        else{
            for(auto iter = p_iter.monster_list.begin(); iter != p_iter.monster_list.end(); iter++){
                auto um_iter = umonster.get(*iter, "Not exist monster");

                nmonster.modify(um_iter, winner, [&](auto& data){
                    data.index = m_idx;
                    data.party_number = fnum;
                    data.monster = um_iter.monster;
                });

                m_idx++;
            }
        }

        // 우승자 테이블에서 정지
        action(permission_level{get_self(), "active"_n},
          get_self(), "transfer"_n,
          std::make_tuple(_self, "unlimitedmas"_n, "battle_result", std::string("battle reward"))).send();
    }

    void devtooth_tower::resultlose(name loser, uint64_t fnum)
    {
        const auto& f_iter = floortable.get(fnum, "Floor info does not exist");
        
        floortable.modify(f_iter, loser, [&](auto& floordata){
            floordata.state = "idle";
            floordata.bnum = floordata.bnum + 1;
        });
    }

    ACTION devtooth_tower::endflag(name winner, uint64_t fnum)
    {
        const auto& f_iter = floortable.get(fnum, "Floor info does not exist");

        eosio_assert(f_iter.owner == winner, "It does not match the Floor Master.");

        eosio_assert(f_iter.endtime <= now(), "Not enough time.");

        floortable.modify(f_iter, winner, [&](auto& floordata){
            floordata.state = "end";
        });

        // EOS 스냅샷하는 부분 추가
        action(permission_level{get_self(), "active"_n},
          get_self(), "transfer"_n,
          std::make_tuple(_self, "unlimitedmas"_n, "battle_result", std::string("battle reward"))).send();
    }

    ACTION devtooth_tower::claim(name who, uint64_t fnum)
    {
        require_auth( who );

        const auto& f_iter = floortable.get(fnum, "Floor info does not exist");

        eosio_assert(f_iter.owner == who, "It does not match the Floor Master.");
        eosio_assert(f_iter.endtime <= now(), "Not enough time.");
        eosio_assert(f_iter.state == "end", "Impossible state");

        // 우승자 테이블에서 파티 정보 삭제 및 히어로 레벨 1로
        action(permission_level{get_self(), "active"_n},
          get_self(), "transfer"_n,
          std::make_tuple(_self, "unlimitedmas"_n, "battle_result", std::string("battle reward"))).send();

        // 다음층 테이블 추가
        floortable.emplace(who, [&](auto& floordata){
            floordata.fnum = fnum + 1;
            floordata.owner = "unlimittest1"_n;
            floordata.bnum = 0;
            floordata.state = "lock";
            floordata.endtime = 0;
        });

        // EOS 스냅샷 확인후 지급
        action(permission_level{get_self(), "active"_n},
          get_self(), "transfer"_n,
          std::make_tuple(_self, "unlimitedmas"_n, "battle_result", std::string("battle reward"))).send();
    }



EOSIO_DISPATCH( devtooth_tower, (changestate)(endflag)(claim) )

} /// namespace eosio
