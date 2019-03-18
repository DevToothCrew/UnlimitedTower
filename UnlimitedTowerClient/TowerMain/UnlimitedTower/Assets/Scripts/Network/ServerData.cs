using System;
using System.Collections.Generic;

#region Info

[Serializable]
public class cpuLimit
{
    public UInt64 used;
    public UInt64 available;
    public UInt64 max;
};

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
};


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

#endregion



#region Data

[Serializable]
public class UserLoginData
{ 
    public List<servantData> servant_list = new List<servantData>();
    public List<monsterData> monster_list = new List<monsterData>();
    public List<itemData> item_list = new List<itemData>();
    public string  token;
    public partyData party_info = new partyData();
    public userData userinfo = new userData();
    public string eos;
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
public class actionInfo
{
    public int target_position;
    public bool avoid;
    public bool critical;
    public int damage;
}

[Serializable]
public class battleActionInfo
{
    public int my_position;
    public int action_type;         //2 공격 , 3 방어 , 202 광폭화[공격증가], 203 자연치유[체력회복]
    public List<int> skill_type;

    public List<actionInfo> battle_action_list = new List<actionInfo>();
}

[Serializable]
public class stageActionInfoData
{
    public string user;
    public int turn;
    public List<battleActionInfo> battle_info_list = new List<battleActionInfo>();
}


[Serializable]
public class stageState
{
    public int position;
    public int index;
    public int id;
    public int now_hp;
    public int physical_attack;
    public int magic_attack;
    public int physical_defense;
    public int magic_defense;
    public int crit_per;
    public int crit_physical_dmg;
    public int crit_magic_dmg;
    public int avoid;
    public int state;       //0 살있음, 1 죽어있음, 2 공격했음, 3 방어했음, 
    public int speed;
    public List<int> skill_list;
    public statusInfo status;
}
[Serializable]
public class stageStateData
{
    public string user;
    public int stage_type;
    public string enemy_user;
    public int stage_number;
    public int state;
    public List<stageState> my_state_list = new List<stageState>();
    public List<stageState> enemy_state_list = new List<stageState>();
}


//add by canie
[Serializable]
public class stageRewardData
{
    public string user;
    public UInt64 reward_money;
    public List<int> get_exp_list = new List<int>();
    public List<servantInfo> get_servant_list = new List<servantInfo>();
    public List<monsterInfo> get_monster_list = new List<monsterInfo>();
    public List<itemInfo> get_item_list = new List<itemInfo>();
}



[Serializable]
public class userResourceData
{
    public UInt64 ram_quota;        //총 램
    public UInt64 net_weight;       
    public UInt64 cpu_weight;
    public cpuLimit net_limit;      //넷밴드위스 사용 정보
    public cpuLimit cpu_limit;      //시피유 사용 정보
    public UInt64 ram_usage;        //사용중 램 용량
};

[Serializable]
public class errorCode
{
    public string code;
    public string message;        
};

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