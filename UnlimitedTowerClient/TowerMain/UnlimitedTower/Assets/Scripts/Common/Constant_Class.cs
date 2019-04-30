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
public class MailInfo
{
    public int index;
    public MAIL_TYPE type;
    public int typeIndex;
    public int count;
    public string name;
    public Sprite resourceIcon;
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
    public int monsterTribe;
    public TRIBE_TYPE tribeType
    {
        get
        {
            return (TRIBE_TYPE)monsterTribe;
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

    public Status(int basicStr, int basicDex, int basicInt)
    {
        this.basicStr = basicStr;
        this.basicDex = basicDex;
        this.basicInt = basicInt;
    }

    public Status()
    {
    }
}

[Serializable]
public class BattleStatus
{
    public Dictionary<EFFECT_ID, int> Status = new Dictionary<EFFECT_ID, int>();
    public List<int> buff = new List<int>();
    public int NowHp;

    public BattleStatus(UserCharacterStateData data, bool isMyParty)
    {
        Status.Add(EFFECT_ID.HP, data.maxHP);
        Status.Add(EFFECT_ID.SPEED, data.speed);
        NowHp = data.nowHp;

        if (isMyParty)
        {
            if (data.charType == CHAR_TYPE.SERVANT)
            {
                UserStageStateData statgData = UserDataManager.Inst.GetStageState();
                UserServantData servant = UserDataManager.Inst.GetServantInfo(data.index);
                if (data.job == 1) // 워리어
                {
                    Status.Add(EFFECT_ID.STR, data.status.basicStr + (servant.level - 1) * 6);
                    Status.Add(EFFECT_ID.DEX, data.status.basicDex + (servant.level - 1) * 3);
                    Status.Add(EFFECT_ID.INT, data.status.basicInt + (servant.level - 1) * 1);
                }
                else if (data.job == 2) // 도적
                {
                    Status.Add(EFFECT_ID.STR, data.status.basicStr + (servant.level - 1) * 1);
                    Status.Add(EFFECT_ID.DEX, data.status.basicDex + (servant.level - 1) * 8);
                    Status.Add(EFFECT_ID.INT, data.status.basicInt + (servant.level - 1) * 1);
                }
                else if (data.job == 3) // 사제
                {
                    Status.Add(EFFECT_ID.STR, data.status.basicStr + (servant.level - 1) * 1);
                    Status.Add(EFFECT_ID.DEX, data.status.basicDex + (servant.level - 1) * 2);
                    Status.Add(EFFECT_ID.INT, data.status.basicInt + (servant.level - 1) * 7);
                }
                else if (data.job == 4) // 아처
                {
                    Status.Add(EFFECT_ID.STR, data.status.basicStr + (servant.level - 1) * 2);
                    Status.Add(EFFECT_ID.DEX, data.status.basicDex + (servant.level - 1) * 7);
                    Status.Add(EFFECT_ID.INT, data.status.basicInt + (servant.level - 1) * 1);
                }
                else if (data.job == 5) // 마법사
                {
                    Status.Add(EFFECT_ID.STR, data.status.basicStr + (servant.level - 1) * 1);
                    Status.Add(EFFECT_ID.DEX, data.status.basicDex + (servant.level - 1) * 1);
                    Status.Add(EFFECT_ID.INT, data.status.basicInt + (servant.level - 1) * 8);
                }
                else
                {
                    Status.Add(EFFECT_ID.STR, data.status.basicStr);
                    Status.Add(EFFECT_ID.DEX, data.status.basicDex);
                    Status.Add(EFFECT_ID.INT, data.status.basicInt);
                }

                Equipment(servant.equipmentDic, true);

                for (int i = 0; i < statgData.mySynergyList.Count; i++)
                {
                    Buff(statgData.mySynergyList[i].id, true, 0);
                }

                for (int i = 0; i < servant.passiveSkillList.Count; i++)
                {
                    Buff(servant.passiveSkillList[i].id, true);
                }


                StatusReCalculation(servant.level);

                Equipment(servant.equipmentDic, false);

                for (int i = 0; i < statgData.mySynergyList.Count; i++)
                {
                    Buff(statgData.mySynergyList[i].id, false, 0);
                }

                for (int i = 0; i < servant.passiveSkillList.Count; i++)
                {
                    Buff(servant.passiveSkillList[i].id, false);
                }
            }
            else if (data.charType == CHAR_TYPE.MONSTER)
            {
                if (data.position < 10)
                {
                    UserStageStateData statgData = UserDataManager.Inst.GetStageState();
                    UserMonsterData monster = UserDataManager.Inst.GetMonsterInfo(data.index);
                    Status.Add(EFFECT_ID.STR, (int)(data.status.basicStr + data.status.basicStr * ((monster.level - 1) * 0.1f)));
                    Status.Add(EFFECT_ID.DEX, (int)(data.status.basicDex + data.status.basicDex * ((monster.level - 1) * 0.1f)));
                    Status.Add(EFFECT_ID.INT, (int)(data.status.basicInt + data.status.basicInt * ((monster.level - 1) * 0.1f)));
                    StatusReCalculation(monster.level);

                    Upgrade(monster.upgrade);

                    for (int i = 0; i < statgData.mySynergyList.Count; i++)
                    {
                        Buff(statgData.mySynergyList[i].id, true, (int)monster.tribeType);
                    }

                    for (int i = 0; i < monster.passiveSkillList.Count; i++)
                    {
                        Buff(monster.passiveSkillList[i].id, true);
                    }


                    StatusReCalculation(monster.level);

                    for (int i = 0; i < statgData.mySynergyList.Count; i++)
                    {
                        Buff(statgData.mySynergyList[i].id, false, (int)monster.tribeType);
                    }

                    for (int i = 0; i < monster.passiveSkillList.Count; i++)
                    {
                        Buff(monster.passiveSkillList[i].id, false);
                    }
                }
            }
        }
    }

    /// <param name="isBasicStatue"> 힘민지만 계산할지 </param>
    public void Buff(int id, bool isBasicStatue = true, int? tribeType = null)
    {
        DBSkillPassiveData passive = CSVData.Inst.GetSkillPassiveData(id);
        
        if (tribeType == null || tribeType + 900 == passive.targetID || passive.targetID == 2)
        {
            if ((isBasicStatue && (passive.effectID == EFFECT_ID.STR || passive.effectID == EFFECT_ID.DEX || passive.effectID == EFFECT_ID.INT)) ||
            (!isBasicStatue && passive.effectID != EFFECT_ID.STR && passive.effectID != EFFECT_ID.DEX && passive.effectID != EFFECT_ID.INT))
            {
                if (passive.effectID == EFFECT_ID.HP)
                {
                    buff.Add(id);
                    return;
                }
                
                if (passive.effectType == EFFECT_TYPE.ADD)
                {
                    Status[passive.effectID] += passive.effectAdd;
                }
                else if (passive.effectType == EFFECT_TYPE.ADD_PER)
                {
                    Status[passive.effectID] += (int)(Status[passive.effectID] * (passive.effectAdd / 100.0f));
                }
                else if (passive.effectType == EFFECT_TYPE.MINUS)
                {
                    Status[passive.effectID] -= passive.effectAdd;
                }
                else if (passive.effectType == EFFECT_TYPE.MINUS_PER)
                {
                    Status[passive.effectID] -= (int)(Status[passive.effectID] * (passive.effectAdd / 100.0f));
                }
                buff.Add(id);
            }
        }
    }

    /// <param name="isBasicStatue"> 힘민지만 계산할지 </param>
    public void Equipment(Dictionary<EQUIPMENT_TYPE, int> equipmentDic, bool isBasicStatue = true)
    {
        foreach (KeyValuePair<EQUIPMENT_TYPE, int> state in equipmentDic)
        {
            if (state.Value != 0)
            {
                UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(state.Value);

                if (isBasicStatue)
                {
                    switch (equipmentData.optionType)
                    {
                        case EQUIPMENT_OPTION_TYPE.STR:
                            Status[EFFECT_ID.STR] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.DEX:
                            Status[EFFECT_ID.DEX] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.INT:
                            Status[EFFECT_ID.INT] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                    }
                }
                else
                {
                    switch (equipmentData.optionType)
                    {
                        case EQUIPMENT_OPTION_TYPE.ATK:
                            Status[EFFECT_ID.ATK] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.MATK:
                            Status[EFFECT_ID.MATK] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.DEF:
                            Status[EFFECT_ID.DEF] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.MDEF:
                            Status[EFFECT_ID.MDEF] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                    }
                }
            }
        }
    }

    public void Upgrade(int upgrade)
    {
        Status[EFFECT_ID.STR] += (int)(Status[EFFECT_ID.STR] * (upgrade * 0.1f));
        Status[EFFECT_ID.DEX] += (int)(Status[EFFECT_ID.DEX] * (upgrade * 0.1f));
        Status[EFFECT_ID.INT] += (int)(Status[EFFECT_ID.INT] * (upgrade * 0.1f));
    }

    public void StatusReCalculation(int level)
    {
        Status status = new Status(Status[EFFECT_ID.STR], Status[EFFECT_ID.DEX], Status[EFFECT_ID.INT]);
        if (Status.ContainsKey(EFFECT_ID.ATK))
        {
            Status[EFFECT_ID.ATK] = Calculator.GetAttack(status, level);
            Status[EFFECT_ID.MATK] = Calculator.GetMagicAttack(status, level);
            Status[EFFECT_ID.DEF] = Calculator.GetDefence(status, level);
            Status[EFFECT_ID.MDEF] = Calculator.GetMagicDefence(status, level);
            Status[EFFECT_ID.CRI_PER] = Calculator.GetCriticalPercent(status, level);
            Status[EFFECT_ID.CRI_DMG] = Calculator.GetCriticalDamage(status, level);
        }
        else
        {
            Status.Add(EFFECT_ID.ATK, Calculator.GetAttack(status, level));
            Status.Add(EFFECT_ID.MATK, Calculator.GetMagicAttack(status, level));
            Status.Add(EFFECT_ID.DEF, Calculator.GetDefence(status, level));
            Status.Add(EFFECT_ID.MDEF, Calculator.GetMagicDefence(status, level));
            Status.Add(EFFECT_ID.CRI_PER, Calculator.GetCriticalPercent(status, level));
            Status.Add(EFFECT_ID.CRI_DMG, Calculator.GetCriticalDamage(status, level));
        }
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

    public List<UserSkillInfo> mySynergyList = new List<UserSkillInfo>();
    public List<UserSkillInfo> enemySynergyList = new List<UserSkillInfo>();
    //내 시너지 리스트
    //적 시너지 리스트
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
            if (type == 1)
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

    public int maxHP;
    //public int maxHP { get { return Calculator.GetMaxHp(status); } }
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
    public int tribeType;
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
public class ShopProductInfo
{
    public int id;
    public int index;
    public SHOP_TYPE type;

    public int limitCount;
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
