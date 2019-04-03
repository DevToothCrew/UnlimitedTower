using System;
using System.Collections.Generic;

#region Info

[Serializable]
public class resourceLimit
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
};

[Serializable]
public class inventoryInfo
{
    public int servant_inventory;
    public int monster_inventory;
    public int equipment_inventory;
    public int item_inventory;
}

[Serializable]
public class servantInfo
{
    public int state;
    public int exp;

    // TODO : job / appear 에서 id로 변경 필요
    //public int job;
    //public appearInfo appear = new appearInfo();
    public int id;
    public int level;
    public int grade;

    public statusInfo status = new statusInfo();
    public List<int> equip_slot = new List<int>();
    public List<int> passive_skill = new List<int>();
    public List<int> active_skill = new List<int>();
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
    public int monster_class;
    public int level;
    public statusInfo status = new statusInfo();
    public List<int> passive_skill = new List<int>();
    public List<int> active_skill = new List<int>();
};


[Serializable]
public class equipmentInfo
{
    public int id;          //아이템 리소스 아이디
    public int state;       //아이템 현재 상태
    public int type;        //장착 타입
    public int tier;        //티어
    public int job;         //직업제한
    public int grade;       //아이템 등급
    public int upgrade; //아이템 강화 수치
    public int value;
    public int equipservantindex;
};

[Serializable]
public class itemInfo
{
    public int id;          //아이템 리소스 아이디
    public int type;        //장착 타입
    public int count;       //갯수
};

#endregion

#region Data

[Serializable]
public class UserLoginData
{ 
    public List<servantData> servant_list = new List<servantData>();
    public List<monsterData> monster_list = new List<monsterData>();
    public List<equipmentData> equipment_list = new List<equipmentData>();
    public List<itemData> item_list = new List<itemData>();
    public partyData party_info = new partyData();
    public userData user_data = new userData();
    public string eos;
    public string token;
}

[Serializable]
public class userData
{
    public string user;
    public int state; //씬 상태
    public int exp;
    public int rank;
    public inventoryInfo inventory_info = new inventoryInfo();
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

[Serializable]  //equipment
public class equipmentData
{
    public int index;
    public equipmentInfo equipment = new equipmentInfo();
}

[Serializable]
public class itemData
{
    public int index;
    public itemInfo item = new itemInfo();
}

[Serializable]
public class servantGrindResultData
{
    // 무엇이 어떻게 추가될지 모른다.
    public List<int> grindServantIndexList = new List<int>();
    public List<itemData> itemList = new List<itemData>();
}

[Serializable]
public class sellMonsterResultData
{
    // 무엇이 어떻게 추가될지 모른다.
    public List<int> sellMonsterIndexList = new List<int>();
    public List<itemData> itemList = new List<itemData>();
}

[Serializable]
public class sellEquipmentResultData
{
    // 무엇이 어떻게 추가될지 모른다.
    public List<int> sellEquipmentIndexList = new List<int>();
    public List<itemData> itemList = new List<itemData>();
}

[Serializable]
public class sellItemResultData
{
    // 무엇이 어떻게 추가될지 모른다.
    public List<itemData> sellItemDataList = new List<itemData>();
    public List<itemData> itemList = new List<itemData>();
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
    public equipmentData data = new equipmentData();
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
public class mailOpenResultData
{
    public List<int> mail_open_index_list;
    public List<servantData> servant_data_list = new List<servantData>();
    public List<monsterData> monster_data_list = new List<monsterData>();
    public List<equipmentData> equip_data_list = new List<equipmentData>();
    public List<itemData> item_data_list = new List<itemData>();
}

[Serializable]
public class actionInfo
{
    public int target_position;
    public bool avoid;
    public bool critical;
    public int damage;
}

[Serializable]
public class characterActionData
{
    public int my_position;
    public int action_type;         //2 공격 , 3 스킬

    public List<actionInfo> action_info_list = new List<actionInfo>();
}

[Serializable]
public class battleActionData
{
    public string user;
    public int turn;
    public List<characterActionData> character_action_list = new List<characterActionData>();
}

[Serializable]
public class buffInfo
{
    public int id;      //1 defense
    public int turn;
}

[Serializable]
public class characterStateData
{
    public int grade;
    public int id;
    public int position;
    public int index;
    public int now_hp;

    public int state;       //0 살있음, 1 죽어있음 
    public List<buffInfo> buff_list = new List<buffInfo>();
    public List<int> passive_skill_list = new List<int>();
    public List<int> active_skill_list = new List<int>();
    public statusInfo status = new statusInfo();
}

[Serializable]
public class stageStateData
{
    public string user;
    public int stage_type;
    public string enemy_user;
    public int stage_number;
    public int turn;
    public List<characterStateData> my_state_list = new List<characterStateData>();
    public List<characterStateData> enemy_state_list = new List<characterStateData>();
}

[Serializable]
public class stageRewardData
{
    public ulong reward_money;
    public List<int> get_exp_list = new List<int>();
    public List<servantData> get_servant_list = new List<servantData>();
    public List<monsterData> get_monster_list = new List<monsterData>();
    public List<equipmentData> get_equipment_list = new List<equipmentData>();
    public List<itemData> get_item_list = new List<itemData>();
}

[Serializable]
public class monsterUpgradeResultData
{
    public bool is_success;
    public monsterData main_monster_data;
    public int sub_monster_index;
    public List<itemData> need_item_list;
}

[Serializable]
public class equipmentUpgradeResultData
{
    public bool is_success;
    public equipmentData main_equipment_data;
    public List<itemData> add_item_list;
}

[Serializable]
public class userResourceData
{
    public UInt64 ram_quota;        //총 램
    public UInt64 net_weight;       
    public UInt64 cpu_weight;
    public resourceLimit net_limit = new resourceLimit();      //넷밴드위스 사용 정보
    public resourceLimit cpu_limit = new resourceLimit();      //시피유 사용 정보
    public UInt64 ram_usage;        //사용중 램 용량
};

[Serializable]
public class errorCode
{
    public string code;
    public string message;        
};

#endregion

#region RequestJsonData

[Serializable]
public class GachaJson
{
    public int gachaIndex;
}

[Serializable]
public class PartySaveJson
{
    public int partyNum = 0;
    public List<int> servantList = new List<int>();
    public List<int> monsterList = new List<int>();
}

[Serializable]
public class StageStartJson
{
    public int partyNum = 0;
    public int stageType = 0;
    public int stageFloor = 0;
}

[Serializable]
public class BattleActionJson
{
    public int turn;
}

[Serializable]
public class StageRewardJson
{
    public int turn;
}

//add owen

[Serializable]
public class ServantGrindJson
{
    public List<int> servantIndexList;
}

[Serializable]
public class MonsterSellJson
{
    public List<int> monsterIndexList;
}

[Serializable]
public class EquipmentSellJson
{
    public List<int> equipmentIndexList;
}

[Serializable]
public class ItemSellJson
{
    public List<itemData> itemDataList;
}

[Serializable]
public class EquipServantJson
{
    public int servantIndex;
    public int equipitemIndex;
}

[Serializable]
public class UnequipServantJson
{
    public int servantIndex;
    public int equipitemIndex;
}

[Serializable]
public class MonsterUpgradeJson
{
    public int mainMonsterIndex;
    public int subMonsterIndex;
}

[Serializable]
public class EquipmentUpgradeJson
{
    public int mainEquipmentIndex;
    public List<int> addItemIndexList;
}

[Serializable]
public class BuyItemJson
{
    public int index;
    public int itemCount;
}

[Serializable]
public class BuyInventoryJson
{
    public int type;
    public int itemCount;
}

[Serializable]
public class BuyRoyalservantJson
{
    public int servantIndex;
}

[Serializable]
public class MailOpenJson
{
    public List<int> mailOpenIndexList;
}




#endregion