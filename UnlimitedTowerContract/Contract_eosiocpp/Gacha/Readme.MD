Gamble System

void user_inventory_init(account_name _user)

- 처음 유저가 회원가입시에 몬스터,용병,아이템 테이블을 셋팅해주는 함수 입니다.
- 인자값으로는 유저의 이름을 받습니다.

void start_gacha(account_name _user)

- 뽑기를 진행하는 함수이다
- 인자로는 유저의 이름을 받는다.


void gacha_servant_job(account_name _user,uint64_t _seed)

-  서번트를 뽑게 되었을 경우 실행되는 함수입니다.
-  인자값으로는 유저 이름과, 난수의 시드 값을 받습니다.


uint8_t gacha_servant_head(uint64_t _seed,uint32_t _count)

- 서번트의 얼굴 정보를 랜덤하게 반환해 주는 함수입니다.

uint8_t gacha_servant_hair(uint64_t _seed,uint32_t _count)

- 서번트의 머리 정보를 랜덤하게 반환해주는 함수입니다.

uint8_t gacha_servant_body(uint64_t _seed,uint32_t _count)

- 서번트의 몸 정보를 랜덤하게 반환해주는 함수입니다.

void gacha_monster_id(account_name _user,uint64_t _seed)

- 몬스터를 뽑게 되었을 경우 실행되는 함수입니다.
- 인자값으로는 유저 이름과, 난수의 시드 값을 받습니다.

void gacha_item_id(account_name _user,uint64_t _seed)

- 아이템을 뽑게 되었을 경우 실행되는 함수입니다.
- 인자값으로는 유저 이름과, 난수의 시드 값을 받습니다.


void reset_all_user_object_data()

- cmonster, cservant , citem 테이블을 리셋 시켜주는 함수 입니다.
- 컨트랙트 배포자만 실행이 가능합니다.
