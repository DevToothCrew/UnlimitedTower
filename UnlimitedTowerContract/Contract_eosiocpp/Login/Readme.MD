Login System

- 유저의 로그인 정보를 관리해주는 클래스 입니다.


[Function]

void create_account(const account_name _user)

- 유저가 처음 게임을 접헀을 때 계정을 생성해주는 함수입니다.
- 인자값으로는 유저의 이름을 받습니다.

void set_look(const account_name _user, uint8_t _hero_slot, uint8_t _head, uint8_t _hair_,uint8_t _body)

- 유저가 캐릭터를 생성했을 때 외모를 셋팅해 주는 함수 입니다.
- 인자값으로는 유저의 이름, 셋팅해줄 캐릭터의 슬롯 넘버, 캐릭터의 머리 정보, 캐릭터의 얼굴 정보를 받습니다.
 
void set_status(const account_name _user, uint8_t _hero_slot)
 
- 유저가 캐릭터를 생성했을 때 스테이터스를 셋팅해 주는 함수 입니다.
- 인자값으로는 유저의 이름, 그리고 셋팅해줄 캐릭터의 슬롯 넘버입니다.
 

void add_hero_slot(account_name _user)

- 유저가 히어로 슬롯을 추가 구매에 대한 값을 남겨주는 함수 입니다.
- 인자값으로는 구매한 유저의 이름을 받습니다.


void eosiotoken_transfer(account_name sender, account_name receiver, T func) 

 - 유저가 토큰을 보냈을때 보낸 토큰에 대해 어떤 행동에 대해 토큰을 보냈는지 판별해주는 함수입니다.
 - 보낸이와 수신자 그리고 템플릿을 리턴해 줍니다.



void set_status(account_name _user, uint8_t _hero_slot)

- 처음 히어로를 생성했을 때 무료로 히어로 스테이터스를 랜덤하게 셋팅해주는 함수 입니다.
- 인자값으로 유저 이름과 히어로의 슬롯 번호를 받습니다.

void change_status(account_name _user,uint8_t _hero_slot)

- 유저가 EOS를 통해 히어로의 초기 스테이터스를 다시 설정할때 실행되는 함수 입니다.
- 인자값으로는 유저 이름과 히어로 슬롯 번호를 받습니다.

void complete_hero_set(account_name _user, uint8_t _hero_slot)

- 유저가 히어로의 초기 설정을 확정지을 때 실행되는 함수 입니다.
- 인자값으로는 유저 이름과 히어로 슬롯 번호를 받습니다. 

void reset_all_user_auth_data()

- cuserauth 테이블을 리셋 시키는 함수 입니다.
- 해당 컨트랙트의 배포자만 실행 가능합니다.

void reset_all_user_log_data()

- cuserlog  테이블을 리셋 시키는 함수 입니다.
- 해당 컨트랙트의 배포자만 실행 가능합니다.