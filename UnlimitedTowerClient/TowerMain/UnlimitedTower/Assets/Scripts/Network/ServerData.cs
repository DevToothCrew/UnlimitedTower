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

    /// <summary>
    ///  새로 추가된 데이터
    ///  islegend : 해당서번트가 전설직업을 갖는 서번트인지 
    ///  name: 해당서번트에게 유저가 붙인 이름
    /// </summary>
    public bool islegend;   
    public string name;
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


    /// <summary>
    /// 추가된 데이터
    /// </summary>
    public string name;
}

[Serializable]
public class TestItemInfo
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

[Serializable]
public class TestItemData
{
    public int index;
    public TestItemInfo item = new TestItemInfo();
}


[Serializable]
public class TestGachaItemData
{
    public int result_type;
    public TestItemData data = new TestItemData();
}


[Serializable]
public class itemInfo
{
    /// <summary>
    /// 추가된 데이터
    /// </summary>
    public int itemnum;

    public bool ismounted;
    public int mounted_serv_num;


    /// <summary>
    /// 기존데이터 중 사용하는 데이터
    /// </summary>
    public int tier;
    public int upgrade;
}

[Serializable]
public class assetInfo
{
    public string symbol_name;
    public int amount;
}

//battle
[Serializable]
public class battleState
{
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

#endregion



#region Data

[Serializable]
public class UserLoginData
{
    public goldData gameMoney = new goldData();
    public userData userinfo = new userData();
    public List<servantData> servant_list = new List<servantData>();
    public List<monsterData> monster_list = new List<monsterData>();
    public List<itemData> item_list = new List<itemData>();
    public partyData party_info = new partyData();
}

[Serializable]
public class goldData
{
    public assetInfo balance = new assetInfo();
}

[Serializable]
public class userData
{
    public string user;
    public int state; //씬 상태
    public servantInfo hero = new servantInfo();
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

[Serializable]
public class TestbattleStateData
{
    public string user;
    public int party_number;
    public List<battleState> state_list = new List<battleState>();
}

//add by canie
[Serializable]
public class TestbattleActionInfoData
{
    public string user;
    public int turn;
    public List<battleActionInfo> battle_info_list = new List<battleActionInfo>();
}

//add by canie
[Serializable]
public class TestbattleRewardData
{
    public string user;
    public int reward_money;
    public List<int> get_exp_list = new List<int>();
    public List<servantInfo> get_servant_list = new List<servantInfo>();
    public List<monsterInfo> get_monster_list = new List<monsterInfo>();
    public List<itemInfo> get_item_list = new List<itemInfo>();
}


[Serializable]
public class TestStageData
{
    public int stage_num;
    public List<monsterInfo> enemy_list = new List<monsterInfo>();
}

[Serializable]
public class actionInfo
{
    public int target_index;
    public bool avoid;
    public bool critical;
    public int damage;
}

[Serializable]
public class battleActionInfo
{
    public int index;
    public int action_type;

    public List<actionInfo> battle_action_list = new List<actionInfo>();
}

//------------------현재 배틀 시스템에서 사용중이라 제거를 못함-----------//
[Serializable]
public class BattleActionData
{
    public int turn;
    public List<battleActionInfo> info_list = new List<battleActionInfo>();
}


[Serializable]
public class stageStateInfo
{
    public int party_index;

    public int now_hp;
    public int damage;
    public int defence;

    public int crit_per;
    public int crit_dmg;
    public int avoid;    

    public int state;

    // TODO : 타워배틀에서 전부 다 필요
    public Status status;
    public int status_type;
    public int exp;
    public int speed;
    public int type;
    public int index;
}

[Serializable]
public class StageStateData
{
    public int turn;

    public int party_num;
    public int stage_num;
    public List<stageStateInfo> my_team_list = new List<stageStateInfo>();
    public List<stageStateInfo> enemy_team_list = new List<stageStateInfo>();
}

[Serializable]
public class StageResultData
{
    public int get_gold;
    public List<int> get_exp_list = new List<int>();
    public List<servantData> get_servant_list = new List<servantData>();
    public List<monsterData> get_monster_list = new List<monsterData>();
    public List<itemData> get_item_list = new List<itemData>();
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
public class JsonBattleAction
{
    public int targetIndex = 0;
    public int actionType = 0;

    public JsonBattleAction(int targetIndex, int actionType)
    {
        this.targetIndex = targetIndex;
        this.actionType = actionType;
    }
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
    public int stageNum = 0;
    public int partyNum = 0;
}
#endregion