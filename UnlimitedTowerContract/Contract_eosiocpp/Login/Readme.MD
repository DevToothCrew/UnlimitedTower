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
 - 보낸이와 수신자 그리고 템플리을 리턴해 줍니다.


