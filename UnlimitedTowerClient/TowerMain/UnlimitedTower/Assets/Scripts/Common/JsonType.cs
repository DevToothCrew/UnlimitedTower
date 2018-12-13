using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Json 파일 형태
// Sample

public enum AnimalType
{
    DOG, CAT, RABBIT
}

// 보내는 데이터 형식을 아래와 같이 정의해줄것.
public class JSONStructure
{
    public AnimalType type;
    public string name;
    public double age;
    public bool cute;
}

//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cuserauth
{
    struct sobject_appear
{
    unsigned_int head = 0;
    unsigned_int hair = 0;
    unsigned_int body = 0;
};
// 4 + 4 + 4 + 1 = 13
struct sobject_status
{
    unsigned_int basic_str = 0;
    unsigned_int basic_dex = 0;
    unsigned_int basic_int = 0;
    unsigned_int job = 0;
};
// 4 + 4 + 4 + 4 = 16
struct sobject_plus_status
{
    unsigned_int plus_str = 0;
    unsigned_int plus_dex = 0;
    unsigned_int plus_int = 0;
    unsigned_int stat_point = 0;
};

// 4 + 3 + 16 + 16 + 12 = 51
struct shero_info
{
    unsigned_int h_state;
    sobject_appear h_appear;
    sobject_status h_status;
    sobject_plus_status h_plus_status;
    std::vector<unsigned_int> h_equip_slot;
};
    public unsigned_int a_game_money;
    public unsigned_int a_state;
    public unsigned_int a_hero_slot;
    public std::vector<shero_info> a_hero_list;
}
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cservant
{
struct cservantinfo
{
    unsigned_double s_index;           //서번트 인덱스번호
    sobject_appear s_appear;    //서번트 외형 정보
    sobject_status s_status;    //서번트 힘민지 직업
    sobject_plus_status s_plus_status;  //서번트 추가힘민지
    std::vector<unsigned_int> s_equip_slot; //서번트 장비 리스트
    unsigned_int s_exp; //서번트 경험치
    unsigned_int s_state; //서번트 상태
};

    public std::vector<cservantinfo> servant_list;
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cmonster
{
struct cmonsterinfo
{
    unsigned_double m_index;  //고유 인덱스
    unsigned_int m_type;   //외형 - 타입
    sobject_status m_status; // 힘민지
    sobject_plus_status m_plus_status; //추가 힘민지
    unsigned_int m_exp; //경험치
    unsigned_int m_grade; // 등급
    unsigned_int m_reinforce; //강화수치
    unsigned_int m_state;
};

    public std::vector<cmonsterinfo> monster_list;
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class citem
{

struct citeminfo
{
    unsigned_double i_index; //아이템 인덱스
    unsigned_int i_id; //아이템 고유 아이디
    unsigned_int i_type = 0;  //종류
    unsigned_int i_slot;  //장착 타입
    unsigned_int i_tier;  //티어
    sobject_status i_status; //힘민지 직업
    unsigned_int i_state = eobject_state::on_inventory; //아이템 현재 상태
    unsigned_int i_grade; //아이템 등급
    unsigned_int i_reinforce = 0; //아이템 강화 수치
};

    public std::vector<citeminfo> item_list;

};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cuserlog
{

    struct asset
    {
        unsigned_double symbol;
        unsigned_double amount;
    }

    public    unsigned_int l_servant_num;
    public    unsigned_int l_monster_num;
    public    unsigned_int l_item_num;
    public    unsigned_int l_gacha_num;
    public    unsigned_int l_login_time;
    public    unsigned_int l_get_gold;
    public    asset l_get_eos;
    public    unsigned_int l_use_gold;
    public    asset l_use_eos;
    public    unsigned_int l_battle_count;
    public    unsigned_int l_last_stage_num;
    public    unsigned_int l_last_tower_num;
    public    unsigned_int l_top_clear_stage;
    public    unsigned_int l_top_clear_tower;
    public    unsigned_int l_add_party_count;
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cparty
{

struct sparty_info
{
    std::vector<uint64_t> party_id_list;
};

    public std::vector<sparty_info> party_list;

};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cbattle
{

struct sbattle_member_state
{
    unsigned_int turn_count;       //캐릭터 상태의 지속 턴 횟수
    unsigned_int turn_state;           //캐릭터의 현재 상태
};

struct sbattle_staus_info
{
    unsigned_int action;
    unsigned_int speed;
    unsigned_int critical;
    unsigned_int defense;
    unsigned_int attack;
    unsigned_int now_hp = 0;
    std::vector<sbattle_member_state> state_list;
    unsigned_double party_object_index = 0;
};

struct attack_speed
{
    unsigned_int member_array_index;
    unsigned_int member_speed;
    unsigned_int member_target;
};


public  unsigned_char b_turn_count;
public  unsigned_int b_stage_number;
public  unsigned_char b_party_number;
public  unsigned_char b_preference;
public  std::vector<unsigned_int> b_reward_list;
public  std::vector<sbattle_staus_info> b_battle_state_list;
public  std::vector<attack_speed> attack_order_list;

};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cstagedata
{

struct stage_info
{
    unsigned_int type_index;
    unsigned_int base_str = 0;
    unsigned_int base_dex = 0;
    unsigned_int base_int = 0;
    unsigned_int base_job = 0;
};

public unsigned_double stage;
public std::vector<stage_info> enemy_list;

};