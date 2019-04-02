﻿using System;
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

    public itemInfo sellItemInfo = new itemInfo();

    public string resourceIcon;
}

[Serializable]
public class DBEquipmentData
{
    public int id;
    public string name;
    public SERVANT_EQUIPMENT_FLAG jobLimit;
    public bool isEquipable(SERVANT_EQUIPMENT_FLAG job)
    {
        if(jobLimit == SERVANT_EQUIPMENT_FLAG.All)
        {
            return true;
        }

        if((jobLimit & job) == job && job != SERVANT_EQUIPMENT_FLAG.All)
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
    public itemInfo sellItemInfo = new itemInfo();
}

[Serializable]
public class DBMonsterData
{
    public int id;
    public string name;
    public int elementType;
    public int classType;

    public int speed;
    public int avoid;
    public int criPer;
    public int criDmg;
    public int mcriPer;
    public int mcriDmg;

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
public class DBMonsterStatData
{
    public MONSTER_CLASS classEnum;
    public int speed;
    public int avoid;
    public int criPer;
    public int criDmg;
    public int mcriPer;
    public int mcriDmg;
}

[Serializable]
public class DBServantData
{
    public int id;
    public int job;

    public int speed;
    public int avoid;
    public int criPer;
    public int criDmg;
    public int mcriPer;
    public int mcriDmg;

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
    public SERVANT_JOB jobEnum;
    public int speed;
    public int avoid;
    public int criPer;
    public int criDmg;
    public int mcriPer;
    public int mcriDmg;
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
    public int bossLevel;
    public List<int> bossPassiveList = new List<int>();
    public List<int> bossActiveList = new List<int>();
    public int rankExp;
    public int charExp;
    public List<int> rewardIdList = new List<int>();
    public List<int> rewardCountList = new List<int>();
    public int firstRewardId;
    public int firstRewardCount;
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
    public int mcriPer;
    public int mcriDmg;
    public int speed;
    public int avoid;
    public List<int> activeList = new List<int>();
    public List<int> passiveList = new List<int>();
}

[Serializable]
public class DBMonsterUpgradeData
{
    public int id;
    public double successPer;
    public itemInfo needItem;
}

#endregion