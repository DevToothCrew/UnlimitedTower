using System;
using System.Collections.Generic;

#region Info

[Serializable]
public class appearInfo
{
    public int head;
    public int hair;
    public int body;
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
    public int look;
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


    /// <summary>
    /// 기존데이터중 사용하지않는 데이터
    /// </summary>
    public int state;
    public int id;
    public int slot;
    public int job;
    public int grade;
    public int atk;
    public int def;
    public statusInfo status = new statusInfo();
}

//add by canie
[Serializable]
public class partyInfo
{
    public int state;
    public List<int> indexList = new List<int>();
}

#endregion



#region Data

[Serializable]
public class UserLoginData
{
    public userData userinfo = new userData();
    public List<servantData> servant_list = new List<servantData>();
    public List<monsterData> monster_list = new List<monsterData>();
    public List<itemData> item_list = new List<itemData>();
    public List<partyData> party_list = new List<partyData>();
}


[Serializable]
public class userData
{
    public string user;
    public int game_money;
    public int state; //씬 상태
    public servantInfo hero = new servantInfo();
}

[Serializable]
public class servantData
{
    public int index;
    public servantInfo servant = new servantInfo();
}

[Serializable]
public class monsterData
{
    public int index;
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

//add by canie
[Serializable]
public class partyData
{
    public int index;
    public int state;
    public List<int> party = new List<int>();
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

    public List<actionInfo> action_list = new List<actionInfo>();
}

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

    // 버프 리스트, 디버프 리스트, 스킬 리스트 추가 예정

    // DB 나오기 전까지 임시로 사용
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
    public List<stageStateInfo> info_list = new List<stageStateInfo>();
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
