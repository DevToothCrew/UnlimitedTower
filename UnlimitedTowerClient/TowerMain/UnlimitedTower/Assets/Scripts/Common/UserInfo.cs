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

//// UserInfomation
//public class UserInfo : MonoSingleton<UserInfo>
//{
//    // public string Packet;

//    public cstagedata current_stage_info = new cstagedata();

//    public cuserauth user_auth = new cuserauth();
//    public cservant user_servant = new cservant();
//    public cmonster user_monster = new cmonster();
//    public citem user_item = new citem();
//    public cbattle user_battle = new cbattle();
//    public cparty user_party = new cparty();

//    public cservantinfo gacha_result_servant = new cservantinfo();
//    public cmonsterinfo gacha_result_monster =  new cmonsterinfo();
//    public citeminfo gacha_result_item =  new citeminfo();


//    public void getLogin(string _login_info)
//    {
//        var user_auth_info = JsonUtility.FromJson<cuserauth>(_login_info);

//        user_auth.a_game_money = user_auth_info.a_game_money;
//        user_auth.a_hero_slot = user_auth_info.a_hero_slot;
//        user_auth.a_state = user_auth_info.a_state;
//        foreach (var v in user_auth_info.a_hero_list)
//        {
//            user_auth.a_hero_list.Add(v);
//        }

//        Debug.Log(user_auth_info.a_game_money);
//        Debug.Log(user_auth_info.a_hero_slot);
//        foreach (var v in user_auth_info.a_hero_list)
//        {
//            Debug.Log(v);
//        }
//    }

//    public void getAllServant(string _all_servant_info)
//    {
//        var user_all_servant_info = JsonUtility.FromJson<cservant>(_all_servant_info);
//        foreach (var servant in user_all_servant_info.servant_list)
//        {
//            user_servant.servant_list.Add(servant);
//            Debug.Log(servant.s_status.basic_str);
//            Debug.Log(servant.s_status.basic_dex);
//            Debug.Log(servant.s_status.basic_int);
//            Debug.Log(servant.s_appear.hair);
//            Debug.Log(servant.s_appear.head);
//            Debug.Log(servant.s_appear.body);
//        }

//    }

//    public void getAddServant(string _servant_info)
//    {
//        Debug.Log("Packet Servant : " + _servant_info);

        
//        var add_servant_info = JsonUtility.FromJson<cservantinfo>(_servant_info);
//        user_servant.servant_list.Add(add_servant_info);
//        gacha_result_servant = add_servant_info;
//        foreach (var servant  in   user_servant.servant_list)
//        {
//            Debug.Log(servant.s_status.basic_str);
//            Debug.Log(servant.s_status.basic_dex);
//            Debug.Log(servant.s_status.basic_int);
//            Debug.Log(servant.s_appear.hair);
//            Debug.Log(servant.s_appear.head);
//            Debug.Log(servant.s_appear.body);
//        }

//        Debug.Log(add_servant_info.s_status.basic_str);
//        Debug.Log(add_servant_info.s_status.basic_dex);
//        Debug.Log(add_servant_info.s_status.basic_int);
//        Debug.Log(add_servant_info.s_appear.hair);
//        Debug.Log(add_servant_info.s_appear.head);
//        Debug.Log(add_servant_info.s_appear.body);


      
//    }

//    public void getAllMonster(string _all_monster_info)
//    {
//        var user_all_monster_info = JsonUtility.FromJson<cmonster>(_all_monster_info);
//        foreach (var monster in user_all_monster_info.monster_list)
//        {
//            user_monster.monster_list.Add(monster);
//            Debug.Log(monster.m_status.basic_str);
//            Debug.Log(monster.m_status.basic_dex);
//            Debug.Log(monster.m_status.basic_int);
//        }
//    }

//    public void getAddMonster(string _monster_info)
//    {
//        Debug.Log("Packet Monster : " + _monster_info);

//        var add_monster_info = JsonUtility.FromJson<cmonsterinfo>(_monster_info);
//        user_monster.monster_list.Add(add_monster_info);
//        gacha_result_monster = add_monster_info;
//        foreach (var monster in user_monster.monster_list)
//        {
//            Debug.Log(monster.m_status.basic_str);
//            Debug.Log(monster.m_status.basic_dex);
//            Debug.Log(monster.m_status.basic_int);
//        }

//        Debug.Log(add_monster_info.m_status.basic_str);
//        Debug.Log(add_monster_info.m_status.basic_dex);
//        Debug.Log(add_monster_info.m_status.basic_int);
//    }

//    public void getAllItem(string _all_item_info)
//    {
//        var user_all_item_info = JsonUtility.FromJson<citem>(_all_item_info);
//        foreach (var item in user_all_item_info.item_list)
//        {
//            user_item.item_list.Add(item);
//            Debug.Log(item.i_status.basic_str);
//            Debug.Log(item.i_status.basic_dex);
//            Debug.Log(item.i_status.basic_int);
//        }
//    }

//    public void getAddItem(string _item_info)
//    {
//        Debug.Log("Packet Item : " + _item_info);

//        var add_item_info = JsonUtility.FromJson<citeminfo>(_item_info);
//        user_item.item_list.Add(add_item_info);
//        gacha_result_item = add_item_info;
//        //gacharesultcs.
//        foreach (var item in user_item.item_list)
//        {
//            Debug.Log(item.i_status.basic_str);
//            Debug.Log(item.i_status.basic_dex);
//            Debug.Log(item.i_status.basic_int);
//        }
//        Debug.Log(add_item_info.i_status.basic_str);
//        Debug.Log(add_item_info.i_status.basic_dex);
//        Debug.Log(add_item_info.i_status.basic_int);
//    }



//    public void getBattle(string _battle_info)
//    {
//        var user_battle_info = JsonUtility.FromJson<cbattle>(_battle_info);
//        // 0되면 끝나는 턴 카운트
//        user_battle.b_turn_count = user_battle_info.b_turn_count;
//        user_battle.b_party_number = user_battle_info.b_party_number;
//        user_battle.b_preference = user_battle_info.b_preference;
//        user_battle.b_stage_number = user_battle_info.b_stage_number;
//        foreach(var reward in user_battle_info.b_reward_list)
//        {
//            user_battle.b_reward_list.Add(reward);
//        }

//        foreach (var battle_state in user_battle_info.b_battle_state_list)
//        {
//            user_battle.b_battle_state_list.Add(battle_state);
//        }

//        foreach (var attack_order in user_battle_info.attack_order_list)
//        {
//            user_battle.attack_order_list.Add(attack_order);
//        }
//    }

//    public void getParty(string _party_info)
//    {
//        var user_party_info = JsonUtility.FromJson<cparty>(_party_info);
//        foreach (var my_party in user_party_info.party_list)
//        {
//            user_party_info.party_list.Add(my_party);
//            for (int i = 0; i < 10; ++i)
//            {
//                Debug.Log(my_party);
//            }
//        }
//    }

//    public void getStage(string _stage_info)
//    {
//        var stage_info = JsonUtility.FromJson<cstagedata>(_stage_info);
//        current_stage_info.stage = stage_info.stage;
//        foreach (var enemy in stage_info.enemy_list)
//        {
//            current_stage_info.enemy_list.Add(enemy);
//            Debug.Log(enemy.type_index);
//            Debug.Log(enemy.base_str);
//            Debug.Log(enemy.base_dex);
//            Debug.Log(enemy.base_int);
//        }
//    }
//}

//// Gacha


