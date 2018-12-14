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


    public char b_turn_count;
    public int b_stage_number;
    public char b_party_number;
    public char b_preference;
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


struct protocol
{
    int type;
};
enum protocol_type
{
    auth = 0,
    servant,
    monster,
    item,
    battle,
    party,
    stage,
};

class Test_Packet_Manager
{
private:
    std::string sendMessage;
public:
    cstagedata stage_data;
 
public:
    cuserauth user_auth;
    cservant user_servant;
    cmonster user_monster;
    citem user_item;
    cbattle user_battle;
    cparty user_party;
public:
    void get_login_info(std::string _login_info)
    { 
        var user_auth_info = JsonUtility.FromJson<cuserauth>(_login_info);

        user_auth.a_game_money = user_auth_info.a_game_money;
        user_auth.a_hero_slot = user_auth_info.a_hero_slot;
        user_auth.a_state = user_auth_info.a_state;
        foreach(var v in user_auth_info.a_hero_List)
        {
            user_auth.a_hero_List.push_back(v);
        }

        Debug.Log(data1.action);
        Debug.Log (data1.state); 
        foreach (var v in data1.others)
        {
            Debug.Log (v);
        }
    }
    
    void get_servant_info(std::string _servant_info)
    {
        var user_servant_info = JsonUtility.FromJson<cservant>(_servant_info);
    }
    
    void get_monster_info(std::string _monster_info)
    {
        var user_monster_info = JsonUtility.FromJson<cmonster>(_monster_info);
    }
    
    void get_item_info(std::string _item_info)
    {
        var user_item_info = JsonUtility.FromJson<citem>(_item_info);
    }

    void get_battle_info(std::string _battle_info)
    {
        var user_battle_info = JsonUtility.FromJson<cbattle>(_battle_info);
    }

    void get_party_info(std::string _party_info)
    {
        var user_party_info = JsonUtility.FromJson<cparty>(_party_info);
    }

    void get_stage_info(std::string _stage_info)
    {
        var stage_info = JsonUtility.FromJson<cstagedata>(_stage_info);
    }

    void recv_packet(std::string _sendMessage)
    {
        std::string type = _sendMessage;
        std::string data = _sendMessage;

        var packet = JsonUtility.FromJson<protocol>(type);
        switch(packet)
        {
            case protocol_type::auth :
            get_login_info(data);
            break;

            case protocol_type::servant :
            get_servant_info(data);
            break;

            case protocol_type::monster :
            get_monster_info(data);
            break;

            case protocol_type::item :
            get_item_info(data);
            break;

            case protocol_type::battle :
            get_battle_info(data);
            break;

            case protocol_type::party :
            get_party_info(data);
            break;

            case protocol_type::stage :
            get_login_info(data);
            break;

            default:
            Debug.Log("recv error");
            break;
        };

    }    

}