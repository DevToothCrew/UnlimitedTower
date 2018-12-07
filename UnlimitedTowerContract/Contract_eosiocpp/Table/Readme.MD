테이블 구조

[table] - auth_user_table.hpp


- primary_key : account_name user : 유저의 계정 명이다.

- a_hero_list : 유저의 영웅 정보를 담고 있는 벡터이다.
[head,hair,body]
[str,dex,int]
[current_state]
enum ehero_state
{   
    set_look = 1,
    set_status,
    set_change_status,
    set_complete,
    set_tower_party,
    set_travel_party,
};

- a_game_money : 유저의 게임 재화 정보를 갖는 값이다.

- a_state : 유저의 상태값을 같는 상태다.
enum euser_state
{
    login = 1,
    lobby,
    battle,
    battle_win,
    battle_lose,
    tower,
    travel,
    pause,
    black,
};

- a_hero_slot : 유저가 보유하고 있는 히어로의 슬롯 갯수 입니다.





[table] - log_table.hpp


- primary_key : account_name user : 유저의 계정 명이다.

- l_servant_num : 유저가 획득한 용벙의 숫자이다

- l_monster_num : 유저가 획득한 몬스터의 숫자이다

- l_item_num    : 유저가 획득한 아이템의 숫자이다

- l_gacha_num   : 유저가 뽑기를 한 횟수이다

- l_login_time  : 유저가 마지막으로 로그인한 시간이다

- l_get_gold    : 유저가 얻은 게임내의 재화량이다.

- l_get_eos     : 유저가 얻은 EOS의 양이다.

- l_use_gold    : 유저가 사용한 게임내의 재화량이다.

- l_use_eos     : 유저가 사용한 EOS의 양이다.

- l_battle_count: 유저가 진행한 전투의 횟수이다.

- l_last_stage_num : 유저가 마지막으로 클리어한 스테이지의 번호이다.

- l_last_tower_num : 유저가 마지막으로 클리어한 타워의 번호이다.

- l_top_clear_stage : 유저가 클리어한 최고 스테이지의 번호이다.

- l_top_clear_tower : 유저가 클리어한 최고 타워의 층 번호이다.

- l_add_party_count     : 유저가 보유한 파티의 갯수이다.

- l_character_count : 유저가 보유한 캐릭터 갯수이다.



[table] - party_table


- primary_key : account_name user : 유저의 계정 명이다.

- party_list : 유저의 파티별 정보를 담고 있는 리스트입니다.
해당 리스트는 아래 구조체로 이루어줘 있습니다.

struct sparty_info
{
    std::vector<uint32_t> party_id_list;
}



[table] - battle_table


- primary_key : account_name user : 유저의 계정 명이다.

- b_turn_count  : 배틀동안 진행되는 턴의 횟수 입니다.

- b_stage_number : 배틀이 진행되는 스테이지 넘버 입니다.

- b_party_number: 배틀을 진행하고 있는 파티 넘버 입니다.


- b_my_party_list : 현재 배틀 진행중인 유저 자신의 파티 정보들입니다.

- b_enemy_party_list : 현재 배틀 진행중인 적의 파티 정보들입니다.

두개의 리스트는 아래의 구조체의 자료형을 갖고 있습니다.

- struct sbattle_member_state
- {
-     turn_count;       //오브젝트의 상태 지속 턴 횟수
-     state;           //오브젝트의 현재 상태
- }
- struct sbattle_staus_info
- {
-     now_hp;         //오브젝트의 현재 hp
-     std::vector<sbattle_member_state> state_list //오브젝트의 현재 상태 리스트입니다.
-     uint64_t party_object_index //오브젝트의 고유 인덱스 값입니다.
- }









------------------------------------------game object table----------------------------------------

enum object_state
{
    in_ineventory,      //인벤토리에 들어있는 상태
    
    in_party,           //파티에 속해있는 상태
    in_tower,           //타워 파티에 속해있는 상태
    object_presale,     //사전예약때 뽑은 상태
};
- 몬스터와 서번트가 사용하는 상태 값 입니다.





* [table] - servant_table

- primary_key : account_name user : 유저의 계정 명이다.

- servant_list : 유저가 소유한 용병의 리스트 입니다.

해당 리스트는 cservantinfo 자료형으로 이루어져 있습니다.
struct sobject_appear
{
    uint8_t head = 0;       //오브젝트 얼굴 정보
    uint8_t hair = 0;       //오브젝트 머리 정보
    uint8_t body = 0;       //오브젝트 몸 정보
};

struct sobject_status
{
    uint32_t basic_str = 0; //오브젝트 기본 힘
    uint32_t basic_dex = 0; //오브젝트 기본 민첩
    uint32_t basic_int = 0; //오브젝트 기본 지능
    uint8_t job = 0;        //오브젝트 기본 직업
};

struct sobject_plus_status
{
    uint32_t plus_str = 0;  //오브젝트 추가 힘
    uint32_t plus_dex = 0;  //오브젝트 추가 민첩
    uint32_t plus_int = 0;  //오브젝트 추가 지능
    uint32_t stat_point = 0;//오브젝트 추가 스테이터스 포인트
};

struct cservantinfo
{
    uint64_t s_index;           //서번트 인덱스번호
    sobject_appear s_appear;    //서번트 외형 정보
    sobject_status s_status;    //서번트 힘민지 직업
    sobject_plus_status s_plus_status;  //서번트 추가힘민지
    std::vector<uint32_t> s_equip_slot; //서번트 장비 리스트
    uint32_t s_exp = 0; //서번트 경험치
    uint32_t s_state = object_state::in_ineventory; //서번트 상태
};



* [table] - monster_table

- primary_key : account_name user : 유저의 계정 명이다.

- monster_list : 유저가 소유한 몬스터의 리스트입니다.

해당리스트는 cmonsterinfo 자료형으로 이루어져 있습니다.

    uint64_t m_index;  //고유 인덱스
    uint32_t m_type;   //외형 - 타입
    sobject_status m_status; // 힘민지
    sobject_plus_status m_plus_status; //추가 힘민지
    uint32_t m_exp = 0; //경험치
    uint32_t m_grade; // 등급
    uint32_t m_reinforce = 0; //강화수치
    uint32_t m_state = object_state::in_ineventory;; //몬스터 상태값




* [table] - item_table

- primary_key : account_name user : 유저의 계정 명이다.

- item_list : 유저가 소유한 아이템의 리스트입니다.

해당리스트는 citeminfo 자료형으로 이루어져 있습니다.
struct citeminfo
{
    uint64_t i_index; //아이템 인덱스
    uint32_t i_id; //아이템 고유 아이디
    uint32_t i_type = 0;  //종류
    uint32_t i_slot;  //장착 타입
    uint32_t i_tier;  //티어
    sobject_status i_status; //힘민지 직업
    uint32_t i_state; //아이템 현재 상태
    uint32_t i_grade; //아이템 등급
    uint32_t i_reinforce = 0; //아이템 강화 수치
};


enum item_state -  아이템의 상태값입니다.
{
    item_inventory = 0, //방금막 뽑아서 인벤토리에 들어온 상태
    item_equip,         //아이템 슬롯에 장착이 되어있는 상태
    item_notsell,       //판매가 불가능한 상태
    item_presale,       //사전예약때 뽑은 상태
};