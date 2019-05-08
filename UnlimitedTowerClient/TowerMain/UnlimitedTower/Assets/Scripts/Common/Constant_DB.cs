using System;
using System.Collections.Generic;
using UnityEngine;

#region DBClass

[Serializable]
public class DBExpData
{
    public int level;
    public int rankExp;
    public int charExp;
}

[Serializable]
public class DBItemData
{
    public int id;
    public string name;
    public string description;
    public int tier;
    public string itemType;

    public List<int> itemParamIDList;

    public int sellItemID;
    public int sellItemCount;

    public string resourceIcon;
    public Sprite ItemIcon;
}

[Serializable]
public class DBEquipmentData
{
    public int id;
    public string name;
    public SERVANT_JOB_FLAG jobLimit;
    public bool isEquipAble(SERVANT_JOB_FLAG job)
    {
        if (jobLimit == SERVANT_JOB_FLAG.None)
        {
            return false;
        }

        if (jobLimit == SERVANT_JOB_FLAG.All)
        {
            return true;
        }

        if((jobLimit & job) == job)
        {
            return true;
        }

        return false;
    }
    public int tier;

    public EQUIPMENT_TYPE equipmentType;
    public EQUIPMENT_OPTION_TYPE optionType;
    public int optionMin;
    public int optionMax;
    public int upgradeValue;    // 강화당 %
    public Dictionary<GRADE_TYPE, double> gradeMultiValueDic = new Dictionary<GRADE_TYPE, double>();
    public string resourceIcon;
    public Sprite equipmentIcon = new Sprite();
    public int sellItemID;
    public int sellItemCount;
}

[Serializable]
public class DBMonsterData
{
    public int id;
    public string name;
    public ELEMENT_TYPE elementType;
    public TRIBE_TYPE tribeType;

    public int speed;
    public int avoid;
    public int criPer;
    public int criDmg;

    public string resourceModel;
    public string resourceIcon;
    public Sprite monsterIcon;
}

[Serializable]
public class DBGradeResourceData
{
    public int grade;
    public GRADE_TYPE gradeType;
    public Sprite gradeIcon;
}

[Serializable]
public class DBElementResourceData
{
    public ELEMENT_TYPE elementType;
    public Sprite elementIcon;
}

[Serializable]
public class DBTribeResourceData
{
    public TRIBE_TYPE tribeType;
    public Sprite tribeIcon;
    public Sprite tribeIconSmall;
}

[Serializable]
public class DBServantJobResourceData
{
    public SERVANT_JOB servantJob;
    public Sprite jobIcon;
}

[Serializable]
public class DBOptionTypeResourceData
{
    public EQUIPMENT_OPTION_TYPE optionType;
    public Sprite optionIcon;
}

[Serializable]
public class DBMonsterStatData
{
    public TRIBE_TYPE tribeEnum;
    public int speed;
    public int avoid;
    public int criPer;
    public int criDmg;
    public int synergePair;
    public int synergeTriple;
    public int synergePenta;
}

[Serializable]
public class DBServantData
{
    public int id;
    public int job;
    public SERVANT_JOB GetJob
    {
        get
        {
            return (SERVANT_JOB)job;
        }
    }
    public SERVANT_JOB_FLAG GetJobFlag
    {
        get
        {
            return (SERVANT_JOB_FLAG)Math.Pow(2, job);
        }
    }

    public Status basicStatus = new Status();
    public int speed;
    public int avoid;
    public int criPer;
    public int criDmg;

    public int body;
    public int gender;
    public int head;
    public int hair;
    public GRADE_TYPE grade;

    public string name;
    public string jobName;
    public string resourceBody;
    public string resourceHead;
    public string resourceHair;
    public string resourceIcon;
    public Sprite servantIcon;
}

[Serializable]
public class DBServantStatData
{
    public int id;
    public SERVANT_JOB jobEnum;
    public int grade;
    public Status status = new Status();
    public int speed;
    public int avoid;
    public int criPer;
    public int criDmg;
}

[Serializable]
public class DBLocalizationData
{
    public int id;
    public string krText;
    public string chText;
    public string enText;
}

[Serializable]
public class DBStageData
{
    public int id;
    public int stageType;
    public int stageFloor;
    public string stageString;
    public int needStageId;
    public int stageGroupIndex;
    public int needEntranceItemId;
    public int needEntranceItemCount;
    public int enemyLevelMin;
    public int enemyLevelMax;
    public List<int> enemyIdList = new List<int>();
    public List<int> enemyPositionList = new List<int>();
    public string mapResource;
    public int bgmSoundId;
}

[Serializable]
public class DBStageEnemyData
{
    public int id;
    public CHAR_TYPE charType;
    public int charID;
    public GRADE_TYPE grade;
    public Status status = new Status();
    public int criPer;
    public int criDmg;
    public int speed;
    public int avoid;
    public List<int> activeList = new List<int>();
    public List<int> passiveList = new List<int>();
}

[Serializable]
public class DBStageRewardData
{
    public int id;
    public int rankExp;
    public int charExp;
    public DBRewardData firstRewardData;
    public int rewardMinUTG;
    public int rewardMaxUTG;
    public string rewardUTGString;
    public List<DBRewardData> rewardDataList = new List<DBRewardData>();
}

[Serializable]
public class DBRewardData
{
    public int rewardID;
    public int rewardCount;
    public int rewardPer;
    public REWARD_TYPE rewardType;
    public GRADE_TYPE rewardGrade;
}

[Serializable]
public class DBMonsterUpgradeData
{
    public int id;
    public GRADE_TYPE grade;
    public double successPer;
    public int needUTGCount;
}

[Serializable]
public class DBEquipmentUpgradeData
{
    public int id;
    public EQUIPMENT_TYPE type;
    public GRADE_TYPE grade;
    public int upgradeLevel;
    public double successPer;
    public int needItemID;
    public int needItemCount;
    public int needUTGCount;
}

[Serializable]
public class DBSkillActiveData
{
    public int id;
    public string name;
    public string explain;
    public string resourceIcon;
    public Sprite activeIcon;

    public SERVANT_JOB enableJob;
    public TRIBE_TYPE enableTribe;

    public double activePer;
    public SKILL_TYPE skillType;
    public int activeTurn;
    public ATTACK_TYPE attackType;
    public DAMAGE_TYPE damageType;
    public ELEMENT_TYPE elementType;
    public TARGET_TYPE targetType;
    public int targetCount;
    public int hitCount;
    public double atkPer1;
    public int atkAdd1;
    public double atkPer2;
    public int atkAdd2;
    public double healPer;
    public int healAdd;
    public List<int> optionIDList = new List<int>();
    public List<int> buffIDList = new List<int>();
}

[Serializable]
public class DBSkillPassiveData
{
    public int id;
    public int passiveType;
    public string name;
    public string explain;
    public string resourceIcon;
    public Sprite passiveIcon;
    public int jobClass;

    public bool enableStackMax;
    public string effectKeyword;
    public EFFECT_ID effectID;
    public EFFECT_TYPE effectType;
    public int effectAdd;
    public int effectPlusAdd;
    //public TARGET_TYPE targetType;
    public int targetID;
}

[Serializable]
public class DBShopData
{
    public int id;
    public SHOP_TYPE type;
    public int productID;
    public int productCount;
    public int limitMaxCount;
    public int priceID;
    public int priceCount;
}

[Serializable]
public class DBMonsterUpgradeStatData
{
    public int id;
    public Dictionary<GRADE_TYPE, float> upStatDic = new Dictionary<GRADE_TYPE, float>();
}

[Serializable]
public class DBEquipmentUpgradeStatData
{
    public int id;
    public Dictionary<GRADE_TYPE, float> upStatDic = new Dictionary<GRADE_TYPE, float>();
}

#endregion