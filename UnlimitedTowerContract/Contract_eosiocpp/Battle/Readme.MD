Battle System

void set_battle(account_name _user, uint8_t _stage )

 - 전투 기본정보를 셋팅해주는 함수
 - 액션을 실행한 유저의 이름, 진행하려는 스테이지의 넘버를 받는다.
 
void start_battle(account_name _user,uint8_t _party_number)

 - 전투를 시작하는 함수이며 세부정보를 셋팅해준다.
 - 유저의 이름과 전투를 실행하는 유저의 파티넘버를 받는다.


 void active_turn( account_name _user,
 uint8_t _characteraction,
 uint8_t _monsteraction,
 uint8_t _character_target,
 uint8_t _monster_target)  
 
 - 유저가 자신이 조종 하는 캐릭터와 몬스터의 행동을 지정하고 턴을 진행하는 함수
 - 액션을 실행한 유저의 이름,자신의 캐릭터의 행동에 대한 액션값 , 자신의 몬스터의 행동에 대한 액션값 , 캐릭터의 타겟 인덱스, 몬스터의 타겟 인덱스를 인자로 받는다
 

 void attack(account_name _user,uint8_t _from,uint8_t _target)
 
 - Battle System 내부에서 반영하는 함수로 공격하는 상태를 처리 한다.
 - 인자로는 유저 이름, 공격을 진행하는 인덱스, 공격을 받는 타겟 인덱스
 
 void defense(account_name _user,uint8_t _from,uint8_t _target)
 
  - Battle System 내부에서 반영하는 함수로 방어,버프하는 상태를 처리 한다.
  - 인자로는 유저 이름, 공격을 진행하는 인덱스, 공격을 받는 타겟 인덱스
 
 void result(account_name _user)
 
  - Battle System 내부에서 반영하는 함수로 배틀의 결과를 반영해준다.
  - 인자로는 유저 이름을 받는다.
 
 bool battlestate(account_name _user)
  - Battle System 내부에서 반영하는 함수로 배틀 상태를 체크한다.
  - 인자로는 유저 이름을 받는다.
 
