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

    //
    //
    //
    //
    //
};

[Serializable]
public class servantInfo
{

    public int state;   // state 의미를 모르겠습니다.
    public int exp;     // 
    public int stat_point;      
    public bool onformation;
    public int formationindex;
    public int job;     // 


    public bool isLegend;
    public string name;


    public bool isMainServant;

    public appearInfo appear = new appearInfo();
    public statusInfo status = new statusInfo();
    public List<int> equip_slot = new List<int>();
};

[Serializable]
public class monsterInfo
{
    public int type;    
    public int look;
    
    public int exp;
    public string name;

    public int grade;
    public int upgrade;

    public statusInfo status = new statusInfo();


    /* 사용 x */
    public int state;
}

[Serializable]
public class itemInfo
{
    // 보유아이템 정보
    public int itemNum;

    // 
    public int tearNum;
    public int upgradeCount;

    public bool isMounted;
    public int mountedServantNum;
}

[Serializable]
public class formationInfo
{
    public int formationIndex;

    public bool isPlaced;
    public int unitIndex;
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
    public List<formationInfo> formationList = new List<formationInfo>();
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
