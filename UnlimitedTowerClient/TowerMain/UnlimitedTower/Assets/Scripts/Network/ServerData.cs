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
    //public int look;
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
    //public int slot;
    public int type;
    public int job;
    public int grade;
    public int atk;
    public int def;
    public statusInfo status = new statusInfo();
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

[Serializable]
public class battleAction
{
    public int target_index;
    public int avoid;
    public int critical;
    public int damage;
}

[Serializable]
public class battleActionInfo
{
    public int index;
    public int action_type;
    public List<battleAction> battle_action_list = new List<battleAction>();
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
    public List<partyData> party_list = new List<partyData>();
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
    //public int game_money;
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

//add by canie
[Serializable]
public class partyData
{
    public int index;
    public int state;
    public List<int> party = new List<int>();
}

[Serializable]
public class battleStateData
{
    public string user;
    public int turn;
    public int party_number;
    public List<battleState> state_list = new List<battleState>();
}

[Serializable]
public class battleActionInfoData
{
    public string user;
    public List<battleActionInfo> state_list = new List<battleActionInfo>();
}

[Serializable]
public class battleRewardData
{
    public string user;
    public int reward_money;
    public List<int> get_exp_list = new List<int>();
    public List<servantInfo> get_servant_list = new List<servantInfo>();
    public List<monsterInfo> get_monster_list = new List<monsterInfo>();
    public List<itemInfo> get_item_list = new List<itemInfo>();
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
