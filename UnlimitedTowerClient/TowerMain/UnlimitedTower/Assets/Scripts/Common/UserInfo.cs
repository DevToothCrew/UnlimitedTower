using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

[System.Serializable]
public class asset
{
    public long symbol;
    public long amount;
}
[System.Serializable]
public class cuserlog
{
    public ulong l_user;
    public int l_servant_num;
    public int l_monster_num;
    public int l_item_num;
    public int l_gacha_num;
    public int l_login_time;
    public int l_get_gold;
    public asset l_get_eos = new asset();
    public int l_use_gold;
    public asset l_use_eos = new asset();
    public int l_battle_count;
    public int l_last_stage_num;
    public int l_last_tower_num;
    public int l_top_clear_stage;
    public int l_top_clear_tower;
    public int l_add_party_count;
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
[System.Serializable]
public class sparty_info
{
    //List<ulong> party_id_list = new List<ulong>();
};
[System.Serializable]
public class cparty
{
    public ulong p_user;
   public List<sparty_info> party_list = new List<sparty_info>();
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
[System.Serializable]
public class sbattle_member_state //당장은 안씀
{
    public int turn_count;       //캐릭터 상태의 지속 턴 횟수
    public int turn_state;           //캐릭터의 현재 상태
};
[System.Serializable]
public class sbattle_staus_info
{
    public int action; // 애니메이션 (변수)
    public int speed; //상관 ㄴㄴ
    public int critical; 
    public int defense;
    public int attack;
    public int now_hp; //현재 hp
    public List<sbattle_member_state> state_list = new List<sbattle_member_state>(); //신경 쓰지말것
    public long party_object_index; //멤버들의 유일한 아이디 값
};
[System.Serializable]
public class attack_speed
{
    public int member_array_index;
    public int member_speed;
    public int member_target;
};
[System.Serializable]
public class cbattle
{
    public ulong b_user;
    public int b_turn_count; // 진행 횟수 1부터 시작 : 0이되면 배틀끝남. ->체그할 때 사용
    public int b_stage_number; 
    public int b_party_number;
    public int b_preference; //우선 순위
    public List<int> b_reward_list = new List<int>(); //졌으면 일다 보상이 없음.
    public List<sbattle_staus_info> b_battle_state_list = new List<sbattle_staus_info>(); 
    public List<attack_speed> attack_order_list = new List<attack_speed>(); //0 번째가 가장 먼저 공격
    // attack_order_list[0] <- 공격순서
    // attack_order_list[0].member_array_index <- 공격하는 파티원의 위치
    // target - >상대방 진영의 상대적인 위치값.
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
[System.Serializable]
public class stage_info
{
    public int type_index;
    public int base_str;
    public int base_dex;
    public int base_int;
    public int base_job;
};

[System.Serializable]
public class cstagedata
{
    public long stage;
    public List<stage_info> enemy_list = new List<stage_info>();

};


