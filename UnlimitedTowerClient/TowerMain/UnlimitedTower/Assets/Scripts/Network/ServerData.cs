using System;
using System.Collections.Generic;

#region Info

[Serializable]
public class appearInfo
{
    public int head;
    public int hair;
    public int body;
    public int gender;
};

[Serializable]
public class statusInfo
{
    public int basic_str = 0;
    public int basic_dex = 0;
    public int basic_int = 0;
    public int plus_str = 0;
    public int plus_dex = 0;
    public int plus_int = 0;
    
};

[Serializable]
public class servantInfo
{
    public int state;
    public int exp;
    public int stat_point;
    public int job;
    public appearInfo appear = new appearInfo();
    public statusInfo status = new statusInfo();
    public List<int> equip_slot = new List<int>();
};


[Serializable]
public class monsterInfo
{
    public int state;
    public int type;
    public int id;
    public int exp;
    public int grade;
    public int upgrade;
    public statusInfo status = new statusInfo();
}


[Serializable]
public class itemInfo
{
    public int id;          //아이템 리소스 아이디
    public int state;       //아이템 현재 상태
    public int type;        //장착 타입
    public int tier;        //티어
    public int job;         //직업제한
    public int grade;       //아이템 등급
    public int upgrade; //아이템 강화 수치
    public int atk;
    public int def;
    public statusInfo status = new statusInfo(); //기본 힘,민,지 추가 힘,민,지
};


//[Serializable]
//public class servantInfo
//{
//    public int state;
//    public int exp;
//    public int stat_point;
//    public int job;

//    public appearInfo appear = new appearInfo();
//    public statusInfo status = new statusInfo();
//    public List<int> equip_slot = new List<int>();

//    /// <summary>
//    ///  새로 추가된 데이터
//    ///  islegend : 해당서번트가 전설직업을 갖는 서번트인지 
//    ///  name: 해당서번트에게 유저가 붙인 이름
//    /// </summary>
//    public bool islegend;
//    public string name;
//};

//[Serializable]
//public class monsterInfo
//{
//    public int state;
//    public int type;
//    public int id;
//    public int exp;
//    public int grade;
//    public int upgrade;

//    public statusInfo status = new statusInfo();


//    /// <summary>
//    /// 추가된 데이터
//    /// </summary>
//    public string name;
//}


//[Serializable]
//public class itemInfo
//{
//    /// <summary>
//    /// 추가된 데이터
//    /// </summary>
//    public int itemnum;

//    public bool ismounted;
//    public int mounted_serv_num;


//    /// <summary>
//    /// 기존데이터 중 사용하는 데이터
//    /// </summary>
//    public int tier;
//    public int upgrade;
//}

//[Serializable]
//public class assetInfo
//{
//    public string symbol_name;
//    public int amount;
//}

////battle
//[Serializable]
//public class battleState
//{
//    public int index;
//    public int position;
//    public int now_hp;
//    public int attack;
//    public int defense;
//    public int crit_per;
//    public int crit_dmg;
//    public int avoid;
//    public int state;
//    public int speed;
//}

#endregion



#region Data

[Serializable]
public class UserLoginData
{ 
    public List<servantData> servant_list = new List<servantData>();
    public List<monsterData> monster_list = new List<monsterData>();
    public List<itemData> item_list = new List<itemData>();
    public goldData token = new goldData();
    public partyData party_info = new partyData();
    public userData userinfo = new userData();
}

[Serializable]
public class goldData
{
    public string balance;
}

[Serializable]
public class userData
{
    public string user;
    public int state; //씬 상태
    public servantInfo hero = new servantInfo();    // Hero는 Index 0 , Party_Num 1 을 무조건 포함
}

[Serializable]
public class servantData
{
    public int index;
    public int party_number;
    public servantInfo servant = new servantInfo();
}

[Serializable]
public class monsterData
{
    public int index;
    public int party_number;
    public monsterInfo monster = new monsterInfo();
}

[Serializable]
public class itemData
{
    public int index;
    public itemInfo item = new itemInfo();
}

[Serializable]
public class gachaServantData
{
    public int result_type;
    public servantData data = new servantData();
}

[Serializable]
public class gachaMonsterData
{
    public int result_type;
    public monsterData data = new monsterData();
}

[Serializable]
public class gachaItemData
{
    public int result_type;
    public itemData data = new itemData();
}

[Serializable]
public class partyData
{
    public int index;
    public int state;
    public List<int> servant_list = new List<int>();
    public List<int> monster_list = new List<int>();
}
//--------------battle fix--------------------//
//add by canie
//-----------------------------------------------------------------------//

[Serializable]
public class TestActionInfo
{
    public int target_position;
    public bool avoid;
    public bool critical;
    public int damage;
}

[Serializable]
public class TestBattleActionInfo
{
    public int my_position;
    public int action_type;

    public List<TestActionInfo> battle_action_list = new List<TestActionInfo>();
}

[Serializable]
public class TestStageActionInfoData
{
    public string user;
    public int turn;
    public List<TestBattleActionInfo> battle_info_list = new List<TestBattleActionInfo>();
}


[Serializable]
public class TestStageState
{
    public int position;
    public int index;
    public int now_hp;
    public int attack;
    public int defense;
    public int crit_per;
    public int crit_dmg;
    public int avoid;
    public int state;
    public int speed;
}
[Serializable]
public class TestStageStateData
{
    public string user;
    public int stage_type;
    public string enemy_user;
    public int stage_number;
    public List<TestStageState> my_state_list = new List<TestStageState>();
    public List<TestStageState> enemy_state_list = new List<TestStageState>();
}


//add by canie
[Serializable]
public class TestStageRewardData
{
    public string user;
    public int reward_money;
    public List<int> get_exp_list = new List<int>();
    public List<servantInfo> get_servant_list = new List<servantInfo>();
    public List<monsterInfo> get_monster_list = new List<monsterInfo>();
    public List<itemInfo> get_item_list = new List<itemInfo>();
}

#endregion

#region ScatterData

[System.Serializable]
public class JsonParty
{
    public int partyNum = 0;
    public List<int> partyList = new List<int>();
}

[System.Serializable]
public class TestParty
{
    public int partyNum = 0;
    public List<int> servantList = new List<int>();
    public List<int> monsterList = new List<int>();
}

[System.Serializable]
public class TestJsonBattleAction
{
    public int heroTargetIndex = 0;
    public int heroActionType = 0;

    public int monsterTargetIndex = 0;
    public int monsterActionType = 0;

}

[System.Serializable]
public class TestJsonStartBattle
{
    public int partyNum = 0;
    public int stageNum = 0;
}

#endregion