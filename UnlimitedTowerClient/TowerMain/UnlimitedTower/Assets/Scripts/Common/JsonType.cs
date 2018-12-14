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


public struct FormationParty
{

    public  int num0;
    public   int num1;
    public int num2;
    public int num3;
    public int num4;
    public int num5;
    public int num6;
    public int num7;
    public int num8;
    public int num9;
}


//**** 전부 uint타입이기 때문에 시작을 1로 한다
//**** 0은 값이 없음을 의미한다.

//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//

public struct sobject_appear
{
    int head;
    int hair;
    int body;
};
// 4 + 4 + 4 + 1 = 13
public struct sobject_status
{
    int basic_str;
    int basic_dex;
    int basic_int;
    int job;
};
// 4 + 4 + 4 + 4 = 16
public struct sobject_plus_status
{
    int plus_str;
    int plus_dex;
    int plus_int;
    int stat_point;
};

// 4 + 3 + 16 + 16 + 12 = 51
public struct shero_info
{
    int h_state;
    sobject_appear h_appear;
    sobject_status h_status;
    sobject_plus_status h_plus_status;
    List<int> h_equip_slot;
};


public class cuserauth
{

    public int a_game_money;
    public int a_state;
    public int a_hero_slot;
    public List<shero_info> a_hero_List;
}
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cservant
{
    public struct cservantinfo
    {
        double s_index;           //서번트 인덱스번호
        sobject_appear s_appear;    //서번트 외형 정보
        sobject_status s_status;    //서번트 힘민지 직업
        sobject_plus_status s_plus_status;  //서번트 추가힘민지
        List<int> s_equip_slot; //서번트 장비 리스트
        int s_exp; //서번트 경험치
        int s_state; //서번트 상태
    };

    public List<cservantinfo> servant_List;
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cmonster
{
    public struct cmonsterinfo
    {
        double m_index;  //고유 인덱스
        int m_type;   //외형 - 타입
        sobject_status m_status; // 힘민지
        sobject_plus_status m_plus_status; //추가 힘민지
        int m_exp; //경험치
        int m_grade; // 등급
        int m_reinforce; //강화수치
        int m_state;
    };

    public List<cmonsterinfo> monster_List;
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class citem
{
    public struct citeminfo
    {
        double i_index; //아이템 인덱스
        int i_id; //아이템 고유 아이디
        int i_type ;  //종류
        int i_slot;  //장착 타입
        int i_tier;  //티어
        sobject_status i_status; //힘민지 직업
        int i_state;
        int i_grade; //아이템 등급
        int i_reinforce ; //아이템 강화 수치
    };

    public List<citeminfo> item_List;

};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cuserlog
{

    public struct asset
    {
        double symbol;
        double amount;
    }

    public int l_servant_num;
    public int l_monster_num;
    public int l_item_num;
    public int l_gacha_num;
    public int l_login_time;
    public int l_get_gold;
    public asset l_get_eos;
    public int l_use_gold;
    public asset l_use_eos;
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
public class cparty
{
    public struct sparty_info
    {
        List<ulong> party_id_List;
    };

    public List<sparty_info> party_List;
};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cbattle
{
    public struct sbattle_member_state
    {
        int turn_count;       //캐릭터 상태의 지속 턴 횟수
        int turn_state;           //캐릭터의 현재 상태
    };

    public struct sbattle_staus_info
    {
        int action;
        int speed;
        int critical;
        int defense;
        int attack;
        int now_hp ;
        List<sbattle_member_state> state_List;
        double party_object_index ;
    };

    public struct attack_speed
    {
        int member_array_index;
        int member_speed;
        int member_target;
    };


    public int b_turn_count;
    public int b_stage_number;
    public int b_party_number;
    public int b_preference;
    public List<int> b_reward_List;
    public List<sbattle_staus_info> b_battle_state_List;
    public List<attack_speed> attack_order_List;

};
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
//---------------------------------------------------------------------------------------//
public class cstagedata
{
    public struct stage_info
    {
        int type_index;
        int base_str ;
        int base_dex ;
        int base_int ;
        int base_job ;
    };

    public double stage;
    public List<stage_info> enemy_List;

};


class UserInfo
{
public:
    cstagedata current_stage_info;
 
public:
    cuserauth user_auth;
    cservant user_servant;
    cmonster user_monster;
    citem user_item;
    cbattle user_battle;
    cparty user_party;
public:
    void getLogin(std::string _login_info)
    { 
        var user_auth_info = JsonUtility.FromJson<cuserauth>(_login_info);

        user_auth.a_game_money = user_auth_info.a_game_money;
        user_auth.a_hero_slot = user_auth_info.a_hero_slot;
        user_auth.a_state = user_auth_info.a_state;
        foreach(var v in user_auth_info.a_hero_List)
        {
            user_auth.a_hero_List.push_back(v);
        }

        Debug.Log(user_auth_info.a_game_money);
        Debug.Log(user_auth_info.a_hero_slot); 
        foreach (var v in user_auth_info.a_hero_List)
        {
            Debug.Log (v);
        }
    }
    
    void getAllServant(std::string _all_servant_info)
    {
        var user_all_servant_info = JsonUtility.FromJson<cservant>(_all_servant_info);
        foreach(var servant in user_all_servant_info.servant_List)
        {
            user_servant.servant_List.push_back(servant);
            Debug.Log(servant.s_index);
            Debug.Log(servant.s_status.base_str);
            Debug.Log(servant.s_status.base_dex);
            Debug.Log(servant.s_status.base_int);
            Debug.Log(servant.s_appear.hair);
            Debug.Log(servant.s_appear.head);
            Debug.Log(servant.s_appear.body);
        }

    }
    
    void getAddServant(std::string _servant_info)
    {
        var add_servant_info = JsonUtility.FromJson<cservantinfo>(_servant_info);
        user_servant.servant_List.push_back(add_servant_info);
        Debug.Log(add_servant_info.s_index);
        Debug.Log(add_servant_info.s_status.base_str);
        Debug.Log(add_servant_info.s_status.base_dex);
        Debug.Log(add_servant_info.s_status.base_int);
        Debug.Log(add_servant_info.s_appear.hair);
        Debug.Log(add_servant_info.s_appear.head);
        Debug.Log(add_servant_info.s_appear.body);
    }

    void getAllMonster(std::string _all_monster_info)
    {
        var user_all_monster_info = JsonUtility.FromJson<cmonster>(_all_monster_info);
        foreach(var monster in user_all_monster_info.monster_List)
        {
            user_monster.monster_List.push_back(monster);
            Debug.Log(monster.m_index);
            Debug.Log(monster.m_status.base_str);
            Debug.Log(monster.m_status.base_dex);
            Debug.Log(monster.m_status.base_int);
        }
    }
    
    void getAddMonster(std::string _monster_info)
    {
        var add_monster_info = JsonUtility.FromJson<cmonsterinfo>(_monster_info);
        user_monster.monster_List.push_back(add_monster_info);
        Debug.Log(add_servant_info.m_index);
        Debug.Log(add_servant_info.m_status.base_str);
        Debug.Log(add_servant_info.m_status.base_dex);
        Debug.Log(add_servant_info.m_status.base_int);
    }

    void getAllItem(std::string _all_item_info)
    {
        var user_all_item_info = JsonUtility.FromJson<citem>(_item_info);
        foreach(var item in user_all_item_info.item_List)
        {
            user_item.item_List.push_back(item);
            Debug.Log(item.i_index);
            Debug.Log(item.i_status.base_str);
            Debug.Log(item.i_status.base_dex);
            Debug.Log(item.i_status.base_int);
        }
    }

    void getAddItem(std::string _item_info)
    {
        var add_item_info = JsonUtility.FromJson<citeminfo>(_item_info);
        user_item.item_list.push_back(add_item_info);
        Debug.Log(add_item_info.i_index);
        Debug.Log(add_item_info.i_status.base_str);
        Debug.Log(add_item_info.i_status.base_dex);
        Debug.Log(add_item_info.i_status.base_int);
    }



    void getBattle(std::string _battle_info)
    {
        var user_battle_info = JsonUtility.FromJson<cbattle>(_battle_info);
        user_battle.b_turn_count = user_battle_info.b_turn_count;
        user_battle.b_party_number = user_battle_info.b_party_number;
        user_battle.b_preference = user_battle_info.b_preference;
        user_battle.b_stage_number = user_battle_info.b_stage_number;



    }

    void getParty(std::string _party_info)
    {
        var user_party_info = JsonUtility.FromJson<cparty>(_party_info);
        foreach(var my_party in user_party_info.party_List)
        {
            user_party_info.party_List.push_back(my_party);   
            for(int i=0;i<10;++i)
            {
                Debug.Log(my_party[i]);
            }
        }
    }

    void getStage(std::string _stage_info)
    {
        var stage_info = JsonUtility.FromJson<cstagedata>(_stage_info);
        current_stage_info.stage = stage_info.stage;
        foreach(var enemy in stage_info.enemy_List)
        {
            current_stage_info.enemy_List.push_back(enemy);
            Debug.Log(enemy.type_index);
            Debug.Log(enemy.base_str);
            Debug.Log(enemy.base_dex);
            Debug.Log(enemy.base_int);
        }
    }

    // void recv_packet(std::string _sendMessage)
    // {
    //     std::string type = _sendMessage;
    //     std::string data = _sendMessage;

    //     var packet = JsonUtility.FromJson<protocol>(type[0]);
    //     switch(packet)
    //     {
    //         case protocol_type::auth :
    //         get_login_info(data);
    //         break;

    //         case protocol_type::servant :
    //         get_servant_info(data);
    //         break;

    //         case protocol_type::monster :
    //         get_monster_info(data);
    //         break;

    //         case protocol_type::item :
    //         get_item_info(data);
    //         break;

    //         case protocol_type::battle :
    //         get_battle_info(data);
    //         break;

    //         case protocol_type::party :
    //         get_party_info(data);
    //         break;

    //         case protocol_type::stage :
    //         get_login_info(data);
    //         break;

    //         default:
    //         Debug.Log("recv error");
    //         break;
    //     };

    // }    

}