using System;
using System.Collections.Generic;
using UnityEngine;

#region Info

[Serializable]
public class NodeInfo
{
    public NODE_LIST nodeName;
    public int nodeSpeed; // 00 ms
}

[Serializable]
public class UserInfo
{
    public string userName;
    public int userEXP;
    public ulong userUTG;
    public ulong userEOS;
    public SCENE_STATE sceneState;

    public int level;

    public UserInventoryInfo inventoryInfo = new UserInventoryInfo();
}

public class UserInventoryInfo
{
    public int servantInventory;
    public int monsterInventory;
    public int equipmentInventory;
    public int itemInventory;
}

[Serializable]
public class UserLobbyInfo
{
    public resourceLimit cpuLimit;
    public ulong cpuWeight;

    public resourceLimit netLimit;
    public ulong netWeight;

    public ulong ramUsage;
    public ulong ramQuota;

    public int mailCount;

    public string chatting;


    public bool IsCPUAlert
    {
        get
        {
            if (cpuLimit == null)
            {
                return false;
            }

            float cpuPer = cpuLimit.used / (float)cpuLimit.available;
            if (cpuPer >= 0.9)
            {
                return true;
            }

            return false;
        }
    }

    public bool IsNETAlert
    {
        get
        {
            if (netLimit == null)
            {
                return false;
            }

            float netPer = netLimit.used / (float)netLimit.available;
            if (netPer >= 0.9)
            {
                return true;
            }

            return false;
        }
    }

    public bool IsRAMAlert
    {
        get
        {
            if (ramUsage == 0 || ramQuota == 0)
            {
                return false;
            }

            float ramPer = ramUsage / (float)ramQuota;
            if (ramPer >= 0.9)
            {
                return true;
            }

            return false;
        }
    }
}

[Serializable]
public class MainCharInfo
{
    public CHAR_TYPE mainCharType;
    public int mainCharID;
    public int grade;
}

[Serializable]
public class UserServantData
{
    public int index;
    public int id;
    public int grade;
    public GRADE_TYPE gradeType
    {
        get
        {
            return (GRADE_TYPE)grade;
        }
    }
    public int state;
    public int job;
    public SERVANT_JOB jobType
    {
        get
        {
            return (SERVANT_JOB)job;
        }
    }

    public int partyIndex;
    public bool isPlaced;

    public Status status = new Status();

    public int exp;
    public int level;

    public int maxHP { get { return Calculator.GetMaxHp(status); } }
    public int atk { get { return Calculator.GetAttack(status); } }
    public int mAtk { get { return Calculator.GetMagicAttack(status); } }
    public int def { get { return Calculator.GetDefence(status); } }
    public int mDef { get { return Calculator.GetMagicDefence(status); } }

    public Dictionary<EQUIPMENT_TYPE, int> equipmentDic = new Dictionary<EQUIPMENT_TYPE, int>();
    public List<UserSkillInfo> passiveSkillList = new List<UserSkillInfo>();
    public List<UserSkillInfo> activeSkillList = new List<UserSkillInfo>();
}

[Serializable]
public class UserMonsterData
{
    public int index;
    public int id;
    public int grade;
    public GRADE_TYPE gradeType
    {
        get
        {
            return (GRADE_TYPE)grade;
        }
    }
    public int upgrade;
    public int state;
    public int monsterClass;
    public MONSTER_CLASS classType
    {
        get
        {
            return (MONSTER_CLASS)monsterClass;
        }
    }
    public int element;
    public ELEMENT_TYPE elementType
    {
        get
        {
            return (ELEMENT_TYPE)element;
        }
    }

    public int partyIndex;
    public bool isPlaced;

    public Status status = new Status();

    public int exp;
    public int level;

    public int maxHP { get { return Calculator.GetMaxHp(status); } }
    public int atk { get { return Calculator.GetAttack(status); } }
    public int mAtk { get { return Calculator.GetMagicAttack(status); } }
    public int def { get { return Calculator.GetDefence(status); } }
    public int mDef { get { return Calculator.GetMagicDefence(status); } }

    public List<UserSkillInfo> passiveSkillList = new List<UserSkillInfo>();
    public List<UserSkillInfo> activeSkillList = new List<UserSkillInfo>();
}

[Serializable]
public class UserEquipmentData
{
    public int index;
    public EQUIPMENT_TYPE equipmentType;
    public int id;

    public int grade;
    public int upgrade;
    public int state;

    public EQUIPMENT_OPTION_TYPE optionType;
    public int value;

    public bool isEquiped;
    public int equipServantIndex;
}

[Serializable]
public class Status
{
    public int basicStr;
    public int basicDex;
    public int basicInt;
}

[Serializable]
public class BattleStatus
{
    public Dictionary<EFFECT_ID, int> Status = new Dictionary<EFFECT_ID, int>();
    public int NowHp;

    public BattleStatus(UserCharacterStateData data)
    {
        Status.Add(EFFECT_ID.ATK, data.atk);
        Status.Add(EFFECT_ID.MATK, data.mAtk);
        Status.Add(EFFECT_ID.DEF, data.def);
        Status.Add(EFFECT_ID.MDEF, data.mDef);
        Status.Add(EFFECT_ID.HP, data.maxHP);
        Status.Add(EFFECT_ID.STR, data.status.basicStr);
        Status.Add(EFFECT_ID.DEX, data.status.basicDex);
        Status.Add(EFFECT_ID.INT, data.status.basicInt);
        Status.Add(EFFECT_ID.SPEED, data.speed);
        Status.Add(EFFECT_ID.AVOID, data.avoid);
        Status.Add(EFFECT_ID.CRI_PER, data.criPer);
        Status.Add(EFFECT_ID.CRI_DMG, data.criDmg);
    }
}

[Serializable]
public class UserFormationData
{
    // 인덱스
    public int index;

    // 파티 정보를 가져올것인지 포메이션 정보를 가져올것인지... 포메이션 정보를 가져올꺼면 파티 하나당 10개의 정보가 필요한게 아닌지?
    public int formationIndex;

    public CHAR_TYPE charType
    {
        get
        {
            if (formationIndex <= DEFINE.ServantMaxFormationNum)
            {
                return CHAR_TYPE.SERVANT;
            }

            return CHAR_TYPE.MONSTER;
        }
    }
}

[Serializable]
public class UserItemData
{
    public int id;
    public List<UserItemInfo> itemInfoList = new List<UserItemInfo>();
}

[Serializable]
public class UserItemInfo
{
    public int index;
    public int count;
}

[Serializable]
public class UserPartyData
{
    public int partyIndex;
    public int partyState;

    public Dictionary<int, UserFormationData> formationDataDic = new Dictionary<int, UserFormationData>();
}

[Serializable]
public class PartyCharacterInfo
{
    public CHAR_TYPE type;
    public int partyPosition;
    public int index;
}

[Serializable]
public class UserStageStateData
{
    public string user;
    public string enemyUser;

    public int stageType; // PVP : 0 으로 주세요
    public int stageFloor;

    public int turn;

    public Dictionary<int, UserCharacterStateData> myStateList = new Dictionary<int, UserCharacterStateData>(); // stateData.position, stateData
    public Dictionary<int, UserCharacterStateData> enemyStateList = new Dictionary<int, UserCharacterStateData>(); // stateData.position, stateData
}

[Serializable]
public class UserCharacterStateData
{
    public int grade;
    public int id;
    public int position;
    public int upgrade;
    public int type;
    public CHAR_TYPE charType
    {
        get
        {
            if(type == 1)
            {
                return CHAR_TYPE.SERVANT;
            }
            else
            {
                return CHAR_TYPE.MONSTER;
            }
        }
    }

    public int index;
    public int nowHp;

    public int maxHP { get { return Calculator.GetMaxHp(status); } }
    public int atk { get { return Calculator.GetAttack(status); } }
    public int mAtk { get { return Calculator.GetMagicAttack(status); } }
    public int def { get { return Calculator.GetDefence(status); } }
    public int mDef { get { return Calculator.GetMagicDefence(status); } }

    public int criPer;
    public int mCriPer;
    public int criDmg;
    public int mCriDmg;
    public int avoid;
    public int speed;

    // Servant 전용
    public int job;
    // Monster 전용
    public int classType;
    public int elementType;

    public int state;       //0 살있음, 1 죽어있음 
    public List<buffInfo> buffList = new List<buffInfo>();
    public List<UserSkillInfo> passiveSkillList = new List<UserSkillInfo>();
    public List<UserSkillInfo> activeSkillList = new List<UserSkillInfo>();

    public Status status = new Status();
}

[Serializable]
public class UserSkillInfo
{
    public int id;
    public Sprite skillIcon;

    // 아래는 DB 정보 참조 가능할것으로 보입니다.
    public int per;
    public int attack_type;
    public int dmg_type;
    public int target;
    public int target_count;
}

[Serializable]
public class CharacterAction
{
    public int myIndex;
    public int targetIndex;
    public ACTION_TYPE actionType;
    public FORMATION_TYPE formationType;
    public CharacterAction(int _myIndex, int _targetIndex, ACTION_TYPE _actionType, FORMATION_TYPE _formationType)
    {
        myIndex = _myIndex;
        targetIndex = _targetIndex;
        actionType = _actionType;
        formationType = _formationType;
    }
}

[Serializable]
public static class ExtensionMethod
{
    public static T GetReferenceTo<T>(this GameObject go, T instance)
    {
        if (instance == null)
        {
            instance = go.GetComponent<T>();
        }
        return instance;
    }
    public static T CheckReferenceTo<T>(T instance)
    {
        return instance;

    }
    public static bool IsNull(this object obj)//literally all i did was add "this"
    {
        return obj == null || ReferenceEquals(obj, null) || obj.Equals(null);
    }
}

#endregion
