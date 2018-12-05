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
    set_complete,
};

- a_game_money : 유저의 게임 재화 정보를 갖는 값이다.

- a_state : 유저의 상태값을 같는 상태다.
enum euser_state
{
    create = 1,
    look,
    status,
    lobby,
    battle_result,
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

- p_party_list : 유저의 파티별 정보를 담고 있는 리스트입니다.
해당 리스트는 아래 구조체로 이루어줘 있습니다.

struct sparty_info
{
    std::vector<uint8_t> object_id_list;
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


[table] - servant_table

- primary_key : account_name user : 유저의 계정 명이다.

- s_servant_list : 유저가 소유한 용병의 리스트 입니다.

해당 리스트는 cservantinfo 자료형으로 이루어져 있습니다.
- s_index             : 해당 용병의 인덱스 번호입니다.
- s_appear       : 해당 용병의 외모 정보입니다.
- [body,hair,hair]
- s_status       : 해당 용병의 스테이터스 정보입니다.
- [str,dex,int,job]
- s_plus_status  : 해당 용병의 증가한 스테이터스 정보입니다.
- [plus str,plus dex, plus int, stat_point]
- s_exp               : 해당 용병의 경험치 정보입니다.
- s_equip_slot             : 해당 용병의 장비 리스트 입니다.

[table] - monster_table

- primary_key : account_name user : 유저의 계정 명이다.

- m_monster_list : 유저가 소유한 몬스터의 리스트입니다.

해당리스트는 cmonsterinfo 자료형으로 이루어져 있습니다.
- m_index             : 해당 몬스터의 인덱스 번호입니다.
- m_type        : 해당 몬스터의 타입 번호입니다.
- m_status       : 해당 몬스터의 스테이터스 정보입니다.
- [str,dex,int,job][job은 무시]
- m_plus_status  : 해당 몬스터의 증가한 스테이터스 정보입니다.
- [plus str,plus dex, plus int, stat_point]
- m_exp               : 해당 몬스터의 경험치 정보입니다.


[table] - item_table

- primary_key : account_name user : 유저의 계정 명이다.

- i_item_list : 유저가 소유한 아이템의 리스트입니다.

해당리스트는 citeminfo 자료형으로 이루어져 있습니다.
- i_index             : 해당 아이템의 고유 인덱스 번호입니다.
- i_slot        : 해당 아이템의 장비 종류 번호입니다.[무기,방어구,악세사리]
- i_type        : 해당 아이템의 리소스 번호 입니다.
- i_tier              : 해당 아이템의 등급 번호 입니다.
- i_status       : 해당 아이템의 스테이터스 정보입니다.
- [str,dex,int,job][job은 무시]