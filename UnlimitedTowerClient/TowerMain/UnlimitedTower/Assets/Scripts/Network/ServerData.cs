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
    public int job;
    public int stat_point;
    public appearInfo appear = new appearInfo();
    public statusInfo status = new statusInfo();
    public List<int> equip_slot = new List<int>();
};

[Serializable]
public class monsterInfo
{
    public int state;
    public int exp;
    public int type;
    public int look;
    public int grade;
    public int upgrade;
    public statusInfo status = new statusInfo();
}

[Serializable]
public class itemInfo
{
    public int state;
    public int id;
    public int slot;
    public int tier;
    public int job;
    public int grade;
    public int upgrade;
    public int atk;
    public int def;
    public statusInfo status = new statusInfo();
}

#endregion

#region Data

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

[Serializable]
public class UserLoginData
{
    public userData userinfo = new userData();
    public List<servantData> servant_list = new List<servantData>();
    public List<monsterData> monster_list = new List<monsterData>();
    public List<itemData> item_list = new List<itemData>();
}

#endregion