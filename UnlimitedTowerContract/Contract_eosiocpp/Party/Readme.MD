Party System
- 파티에 관련된 내용들을 관리하는 클래스 입니다. 



[Function]

void party_init(account_name _user)

- 처음 회원가입 시에 플레이어의 파티를 셋팅해주는 함수입니다.
- 인자값으로는 유저의 이름을 받습니다.



void set_party(account_name _user,uint8_t _party_number,uint8_t _party_location_index,uint32_t _object_type,uint64_t _object_index)

- 플레이어의 파티원들을 셋팅해주는 함수입니다.
- 인자값으로는 유저의 이름, 배치할 파티 번호, 배치할 파티 위치, 해당 오브젝트의 타입 ,해당 오브젝트의 고유 인덱스값을 받습니다.


