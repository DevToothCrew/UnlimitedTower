using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//

[System.Serializable]
public class sobject_appear
{
    public int head;
    public int hair;
    public int body;
};
// 4 + 4 + 4 + 1 = 13
[System.Serializable]
public class sobject_status
{
    public int basic_str;
    public int basic_dex;
    public int basic_int;
    public int job;
};
// 4 + 4 + 4 + 4 = 16
[System.Serializable]
public class sobject_plus_status
{
    public int plus_str;
    public int plus_dex;
    public int plus_int;
    public int stat_point;
};

// 4 + 3 + 16 + 16 + 12 = 51
[System.Serializable]
public class shero_info
{
    public int h_state;
    public sobject_appear h_appear = new sobject_appear();
    public sobject_status h_status = new sobject_status();
    public sobject_plus_status h_plus_status = new sobject_plus_status();
    public List<int> h_equip_slot = new List<int>();
};

[System.Serializable]
public class cuserauth
{

    public int a_game_money;
    public int a_state;
    public int a_hero_slot;
    public List<shero_info> a_hero_List = new List<shero_info>();
}
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
[System.Serializable]
public class cservantinfo
{
    public sobject_appear s_appear = new sobject_appear();    //서번트 외형 정보
    public sobject_status s_status = new sobject_status();    //서번트 힘 민 지 직업<- 리소스0~4
    public sobject_plus_status s_plus_status = new sobject_plus_status();  //서번트 추가힘민지
    public long s_index;           //서번트 인덱스번호
    public List<int> s_equip_slot = new List<int>(); //서번트 장비 리스트
    public int s_exp; //서번트 경험치
    public int s_state; //서번트 상태
};

[System.Serializable]
public class cservant
{
    public List<cservantinfo> servant_List = new List<cservantinfo>();
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
[System.Serializable]
public class cmonsterinfo
{
    public sobject_status m_status = new sobject_status(); // 힘민지
    public sobject_plus_status m_plus_status = new sobject_plus_status(); //추가 힘민지
    public long m_index;  //고유 인덱스
    public int m_type;   //외형 - 타입 - 0~29
    public int m_exp; //경험치
    public int m_grade; // 등급
    public int m_reinforce; //강화수치
    public int m_state;
};
[System.Serializable]
public class cmonster
{
    public List<cmonsterinfo> monster_List = new List<cmonsterinfo>();
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
[System.Serializable]
public class citeminfo
{
    public sobject_status i_status = new sobject_status(); //힘민지 직업
    public long i_index; //아이템 인덱스
    public int i_id; //아이템 고유 아이디 <-리소스 0~
    public int i_type;  //종류
    public int i_slot;  //장착 타입
    public int i_tier;  //티어
    public int i_state;
    public int i_grade; //아이템 등급
    public int i_reinforce; //아이템 강화 수치
};
[System.Serializable]
public class citem
{
    public List<citeminfo> item_List = new List<citeminfo>();
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
[System.Serializable]
public class asset
{
    public long symbol;
    public long amount;
}
[System.Serializable]
public class cuserlog
{
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
    List<ulong> party_id_List = new List<ulong>();
};
[System.Serializable]
public class cparty
{
   public List<sparty_info> party_List = new List<sparty_info>();
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
[System.Serializable]
public class sbattle_member_state
{
    public int turn_count;       //캐릭터 상태의 지속 턴 횟수
    public int turn_state;           //캐릭터의 현재 상태
};
[System.Serializable]
public class sbattle_staus_info
{
    public int action;
    public int speed;
    public int critical;
    public int defense;
    public int attack;
    public int now_hp;
    public List<sbattle_member_state> state_List = new List<sbattle_member_state>();
    public long party_object_index;
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

    public int b_turn_count;
    public int b_stage_number;
    public int b_party_number;
    public int b_preference;
    public List<int> b_reward_List = new List<int>();
    public List<sbattle_staus_info> b_battle_state_List = new List<sbattle_staus_info>();
    public List<attack_speed> attack_order_List = new List<attack_speed>();

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
    public List<stage_info> enemy_List = new List<stage_info>();

};

// UserInfomation
public class UserInfo : MonoSingleton<UserInfo>
{
    // public string Packet;

    public cstagedata current_stage_info = new cstagedata();

    public cuserauth user_auth = new cuserauth();
    public cservant user_servant = new cservant();
    public cmonster user_monster = new cmonster();
    public citem user_item = new citem();
    public cbattle user_battle = new cbattle();
    public cparty user_party = new cparty();

    public cservantinfo gacha_result_servant = new cservantinfo();
    public cmonsterinfo gacha_result_monster =  new cmonsterinfo();
    public citeminfo gacha_result_item =  new citeminfo();


    public void getLogin(string _login_info)
    {
        var user_auth_info = JsonUtility.FromJson<cuserauth>(_login_info);

        user_auth.a_game_money = user_auth_info.a_game_money;
        user_auth.a_hero_slot = user_auth_info.a_hero_slot;
        user_auth.a_state = user_auth_info.a_state;
        foreach (var v in user_auth_info.a_hero_List)
        {
            user_auth.a_hero_List.Add(v);
        }

        Debug.Log(user_auth_info.a_game_money);
        Debug.Log(user_auth_info.a_hero_slot);
        foreach (var v in user_auth_info.a_hero_List)
        {
            Debug.Log(v);
        }
    }

    public void getAllServant(string _all_servant_info)
    {
        var user_all_servant_info = JsonUtility.FromJson<cservant>(_all_servant_info);
        foreach (var servant in user_all_servant_info.servant_List)
        {
            user_servant.servant_List.Add(servant);
            Debug.Log(servant.s_index);
            Debug.Log(servant.s_status.basic_str);
            Debug.Log(servant.s_status.basic_dex);
            Debug.Log(servant.s_status.basic_int);
            Debug.Log(servant.s_appear.hair);
            Debug.Log(servant.s_appear.head);
            Debug.Log(servant.s_appear.body);
        }

    }

    public void getAddServant(string _servant_info)
    {
        Debug.Log("Packet Servant : " + _servant_info);

        
        var add_servant_info = JsonUtility.FromJson<cservantinfo>(_servant_info);
        user_servant.servant_List.Add(add_servant_info);
        gacha_result_servant = add_servant_info;
        foreach (var servant  in   user_servant.servant_List)
        {
            Debug.Log(servant.s_index);
            Debug.Log(servant.s_status.basic_str);
            Debug.Log(servant.s_status.basic_dex);
            Debug.Log(servant.s_status.basic_int);
            Debug.Log(servant.s_appear.hair);
            Debug.Log(servant.s_appear.head);
            Debug.Log(servant.s_appear.body);
        }

        Debug.Log(add_servant_info.s_index);
        Debug.Log(add_servant_info.s_status.basic_str);
        Debug.Log(add_servant_info.s_status.basic_dex);
        Debug.Log(add_servant_info.s_status.basic_int);
        Debug.Log(add_servant_info.s_appear.hair);
        Debug.Log(add_servant_info.s_appear.head);
        Debug.Log(add_servant_info.s_appear.body);


      
    }

    public void getAllMonster(string _all_monster_info)
    {
        var user_all_monster_info = JsonUtility.FromJson<cmonster>(_all_monster_info);
        foreach (var monster in user_all_monster_info.monster_List)
        {
            user_monster.monster_List.Add(monster);
            Debug.Log(monster.m_index);
            Debug.Log(monster.m_status.basic_str);
            Debug.Log(monster.m_status.basic_dex);
            Debug.Log(monster.m_status.basic_int);
        }
    }

    public void getAddMonster(string _monster_info)
    {
        Debug.Log("Packet Monster : " + _monster_info);

        var add_monster_info = JsonUtility.FromJson<cmonsterinfo>(_monster_info);
        user_monster.monster_List.Add(add_monster_info);
        gacha_result_monster = add_monster_info;
        foreach (var monster in user_monster.monster_List)
        {
            Debug.Log(monster.m_index);
            Debug.Log(monster.m_status.basic_str);
            Debug.Log(monster.m_status.basic_dex);
            Debug.Log(monster.m_status.basic_int);
        }

        Debug.Log(add_monster_info.m_index);
        Debug.Log(add_monster_info.m_status.basic_str);
        Debug.Log(add_monster_info.m_status.basic_dex);
        Debug.Log(add_monster_info.m_status.basic_int);
    }

    public void getAllItem(string _all_item_info)
    {
        var user_all_item_info = JsonUtility.FromJson<citem>(_all_item_info);
        foreach (var item in user_all_item_info.item_List)
        {
            user_item.item_List.Add(item);
            Debug.Log(item.i_index);
            Debug.Log(item.i_status.basic_str);
            Debug.Log(item.i_status.basic_dex);
            Debug.Log(item.i_status.basic_int);
        }
    }

    public void getAddItem(string _item_info)
    {
        Debug.Log("Packet Item : " + _item_info);

        var add_item_info = JsonUtility.FromJson<citeminfo>(_item_info);
        user_item.item_List.Add(add_item_info);
        gacha_result_item = add_item_info;
        //gacharesultcs.
        foreach (var item in user_item.item_List)
        {
            Debug.Log(item.i_index);
            Debug.Log(item.i_status.basic_str);
            Debug.Log(item.i_status.basic_dex);
            Debug.Log(item.i_status.basic_int);
        }

        Debug.Log(add_item_info.i_index);
        Debug.Log(add_item_info.i_status.basic_str);
        Debug.Log(add_item_info.i_status.basic_dex);
        Debug.Log(add_item_info.i_status.basic_int);
    }



    public void getBattle(string _battle_info)
    {
        var user_battle_info = JsonUtility.FromJson<cbattle>(_battle_info);
        user_battle.b_turn_count = user_battle_info.b_turn_count;
        user_battle.b_party_number = user_battle_info.b_party_number;
        user_battle.b_preference = user_battle_info.b_preference;
        user_battle.b_stage_number = user_battle_info.b_stage_number;
        foreach(var reward in user_battle_info.b_reward_List)
        {
            user_battle.b_reward_List.Add(reward);
        }

        foreach (var battle_state in user_battle_info.b_battle_state_List)
        {
            user_battle.b_battle_state_List.Add(battle_state);
        }

        foreach (var attack_order in user_battle_info.attack_order_List)
        {
            user_battle.attack_order_List.Add(attack_order);
        }
    }

    public void getParty(string _party_info)
    {
        var user_party_info = JsonUtility.FromJson<cparty>(_party_info);
        foreach (var my_party in user_party_info.party_List)
        {
            user_party_info.party_List.Add(my_party);
            for (int i = 0; i < 10; ++i)
            {
                Debug.Log(my_party);
            }
        }
    }

    public void getStage(string _stage_info)
    {
        var stage_info = JsonUtility.FromJson<cstagedata>(_stage_info);
        current_stage_info.stage = stage_info.stage;
        foreach (var enemy in stage_info.enemy_List)
        {
            current_stage_info.enemy_List.Add(enemy);
            Debug.Log(enemy.type_index);
            Debug.Log(enemy.base_str);
            Debug.Log(enemy.base_dex);
            Debug.Log(enemy.base_int);
        }
    }
}

// Gacha


