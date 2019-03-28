using System;
using System.Collections.Generic;
using UnityEngine;

#region DBClass

[Serializable]
public class DBItemData
{
    public int id;
    public string name;
    public string resourceIcon;
    public string description;
    public int tier;
    public string itemType;
    public List<int> itemParamIDList;
}

[Serializable]
public class DBMonsterData
{
    public int id;
    public string name;
    public int elementType;
    public int classType;
    public string resourceModel;
    public string resourceIcon;
    public Sprite monsterIcon;
}

[Serializable]
public class DBServantData
{
    public int id;
    public int job;
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
    public List<int> enemyIdList;
    public List<int> enemyPositionList;
    public int bossLevel;
    public List<int> bossPassiveList;
    public List<int> bossActiveList;
    public int rankExp;
    public int charExp;
    public List<int> rewardIdList;
    public List<int> rewardCountList;
    public int firstRewardId;
    public int firstRewardCount;
    public string mapResource;
    public int bgmSoundId;
}

[Serializable]
public class DBMonsterUpgradeData
{
    public int id;
    public double successPer;
    public itemInfo needItem;
}

#endregion