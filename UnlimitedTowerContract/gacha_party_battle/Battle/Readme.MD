Battle System

void set_battle(account_name _user,uint32_t _party_number,uint8_t _stage)

 - 전투 기본정보를 셋팅해주는 함수
 - 액션을 실행한 유저의 이름, 파티 넘버 ,진행하려는 스테이지의 넘버를 받는다.
 
void start_battle(account_name _user,uint8_t _party_number)

 - 전투를 시작하는 함수이며 세부정보를 셋팅해준다.
 - 유저의 이름과 전투를 실행하는 유저의 파티넘버를 받는다.


 void active_turn( account_name _user,
 uint8_t _hero_action,
 uint8_t _monster_action,
 uint8_t _hero_target,
 uint8_t _monster_target)  
 
 - 유저가 자신이 조종 하는 캐릭터와 몬스터의 행동을 지정하고 턴을 진행하는 함수
 - 액션을 실행한 유저의 이름,자신의 캐릭터의 행동에 대한 액션값 , 자신의 몬스터의 행동에 대한 액션값 , 캐릭터의 타겟 인덱스, 몬스터의 타겟 인덱스를 인자로 받는다


void win_reward(account_name _user)

- 유저가 전투에서 승리하였을 경우 보상과 로그를 셋팅해주는 함수 입니다.
- 유저의 이름을 인자값으로 받습니다.

void fail_reward(account_name _user)

- 유저가 전투에서 패배하였을 경우 보상과 로그를 셋팅해주는 함수 입니다.
- 유저의 이름을 인자값으로 받습니다.

void get_battle_reward(account_name _user)

- 유저가 보상을 획득하게 해주는 함수 입니다.
- 유저의 이름을 인자값으로 받습니다.

void reset_all_battle_data()

- cbattle 테이블을 리셋 해주는 함수입니다.
- 컨트랙트의 배포자만 실행이 가능합니다.
