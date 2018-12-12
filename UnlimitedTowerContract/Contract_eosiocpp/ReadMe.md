
Action function 정리

* void signup(account_name _user)
    - 회원 가입을 하는 액션 입니다.
    - 인자값으로는 유저 이름이 필요합니다.
    - cuserauth, cuserlog, cservant, citem, cmonster, cparty
    
* void lookset(account_name _user,uint8_t _hero_slot ,uint8_t _head,uint8_t _hair,uint8_t _body)
    - 유저의 히어로 외모 정보를 셋팅하는 액션입니다.
    - 유저 이름, 셋팅할 히어로 슬롯 번호, 얼굴,머리,몸통 정보가 들어갑니다.
    - cuserauth, cuserlog

* void statset(account_name _user,uint8_t _hero_slot)
    - 히어로 처음 무료 스탯 셋팅을 해주는 액션입니다.
    - 유저 이름, 히어로 슬롯 번호가 들어갑니다.
    - cuserauth, cuserlog

* void completehero(account_name _user,uint32_t _hero_slot)
    - 히어로 셋팅을 확정짓는 액션입니다.
    - 유저의 이름, 히어로의 슬롯 번호가 들어갑니다.
    - cuserauth, cuserlog

* void transfer(account_name sender, account_name receiver)
    - 해당 액션은 eosio.token 컨트랙트의 transfer 실행과 동시에 실행되는 액션입니다.
    - transfer 메모에 들어가는 내용에 따라 역활이 갈라집니다.
    - eosio.token transfer의 인자값의 경우 구매하는 유저이름, 게임컨트랙트 이름, 금액, 메모가 들어갑니다.
    - 
    - [뽑기]
    - 금액     : 1.0000  EOS
    - memo : gacha   
    - cuserauth, cuserlog, cservant, citem, cmonster

    - [추가 파티 슬롯 구매]
    - 금액     : 1.0000  EOS
    - memo : addparty
    - cuserauth, cuserlog

    - [추가 히어로 슬롯 구매]
    - 금액     : 1.0000  EOS
    - memo : addhero
    - cuserauth, cuserlog

    - [히어로 초기 스탯 주사위]
    - 금액     : 0.1000  EOS
    - memo : changestat:hero_slot
    - ex) changestat:0 (edited)
    - cuserauth, cuserlog
    


* void startbattle(account_name _user,uint8_t _party_number,const std::vector<uint32_t> _party_list)
    - 배틀을 시작하는 액션 입니다.
    - 인자값으로는 유저의 이름, 파티넘버, 파티리스트가 들어갑니다.
    - cbattledata, cbattle

* void activeturn(account_name _user,uint8_t _hero_action,uint8_t _monster_action,uint8_t _hero_target,uint8_t _monster_target)
    - 턴을 진행하는 액션입니다.
    - 유저의 이름, 히어로의 실행할 액션, 몬스터의 실행할 액션, 히어로의 타겟, 몬스터의 타겟 입니다.
    - cbattle, cuserauth, cuserlog


* void sellitem(account_name _user,uint8_t _item_location,uint64_t _item_index)
    - 아이템을 판매할 때 사용하는 액션입니다.
    - 유저의 이름, 아이템의 인벤토리내의 현재 위치, 아이템의 인덱스 번호를 받습니다.
    - cuserauth, cuserlog, citem

* void buyitem(account_name _user,uint8_t _item_location,uint64_t _item_index)
    - 현재 미구현 상태 입니다.

* void equipser(account_name _user,uint8_t _item_location,uint64_t _item_index,uint64_t _object_index,uint8_t _item_slot)
    - 서번트의 장비를 장착하는 액션입니다.
    - 유저의 이름, 아이템의 인벤토리내의 현재위치, 아이템의 인덱스번호, 장착할 서번트의 인덱스번호, 장착할 아이템 슬롯을 받습니다.
    - cservant, citem


* void unequipser(account_name _user,uint32_t _servant_location,uint8_t _item_slot)
    - 서번트의 장비를 해제하는 액션입니다.
    - 유저의 이름, 해제할 서번트의 인벤토리내의 위치, 해제할 아이템 슬롯을 받습니다.
    - cservant, citem

* void equiphero(account_name _user, uint8_t _hero_slot,uint8_t _item_location, uint64_t _item_index,uint8_t _item_slot)
    - 히어로의 장비를 장착하는 액션입니다.
    - 유저의 이름, 히어로의 슬롯 정보, 아이템의 인벤토리내의 현재위치, 아이템의 인덱스번호, 장착할 아이템 슬롯을 받습니다.
    - cuserauth, citem

* void unequiphero(account_name _user, uint8_t _hero_slot,uint8_t _item_slot)
    - 히어로의 장비를 해제하는 액션입니다.
    - 유저의 이름, 해제할 히어로의 슬롯 번호 , 해제할 아이템 슬롯을 받습니다.
    - cuserauth, citem

