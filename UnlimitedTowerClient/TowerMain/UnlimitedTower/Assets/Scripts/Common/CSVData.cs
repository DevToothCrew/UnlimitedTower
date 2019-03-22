using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;

public class CSVData : MonoSingleton<CSVData> {

    public Dictionary<int, DBItemData> DBItemDataDic = new Dictionary<int, DBItemData>();
    public Dictionary<int, DBStageData> DBStageDataDic = new Dictionary<int, DBStageData>();
    public Dictionary<int, DBLocalizationData> DBLocalDataDic = new Dictionary<int, DBLocalizationData>();
    private LOCALIZATION_TYPE localType;
    public Dictionary<int, DBServantData> DBServantDataDic = new Dictionary<int, DBServantData>();
    public Dictionary<int, DBMonsterData> DBMonsterDataDic = new Dictionary<int, DBMonsterData>();
    public Dictionary<int, DBMonsterUpgradeData> DBMonsterUpgradeDataDic = new Dictionary<int, DBMonsterUpgradeData>();

    //  인스펙터에서 보여주기 위한...
    public List<DBMonsterData> monsterDataInspector = new List<DBMonsterData>();

    void Start()
    {
        //SetLocalizationData();

        // 스킬 데이터 추가 필요
        // 스텟 데이터 추가 필요
        // 기타 데이터 추가 필요
        SetItemData();
        SetStageData();
        SetServantData();
        SetMonsterData();
        SetMonsterUpgradeData();
        localType = LOCALIZATION_TYPE.EN;
    }

    #region SetFunction

    public void SetItemData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_item");

        for (var i = 3; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["name"]
            //    + " " + data[i]["resource_icon"]
            //    + " " + data[i]["description"]
            //    + " " + data[i]["tier"]
            //    + " " + data[i]["item_type"]
            //    + " " + data[i]["item_param_list"]
            //    );

            DBItemData itemData = new DBItemData();
            itemData.id = Convert.ToInt32(data[i]["id"]);
            itemData.name = Convert.ToString(data[i]["name"]);
            itemData.resourceIcon = Convert.ToString(data[i]["resource_icon"]);
            itemData.description = Convert.ToString(data[i]["description"]);
            itemData.tier = Convert.ToInt32(data[i]["tier"]);
            itemData.itemType = Convert.ToString(data[i]["item_type"]);

            itemData.itemParamIDList = new List<int>();
            // Param List검사 추가 필요

            DBItemDataDic.Add(itemData.id, itemData);
        }
    }

    public void SetStageData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_stage");

        for (var i = 3; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["stage_type"]
            //    + " " + data[i]["stage_floor"]
            //    + " " + data[i]["stage_string"]
            //    + " " + data[i]["need_stage_id"]
            //    + " " + data[i]["stage_group_index"]
            //    + " " + data[i]["need_entrance_item_id"]
            //    + " " + data[i]["need_entrance_item_count"]
            //    + " " + data[i]["enemy_level_min"]
            //    + " " + data[i]["enemy_level_max"]
            //    + " " + data[i]["enemy_id_list"]
            //    + " " + data[i]["enemy_position"]
            //    + " " + data[i]["boss_level"]
            //    + " " + data[i]["boss_passive_list"]
            //    + " " + data[i]["boss_active_list"]
            //    + " " + data[i]["rank_exp"]
            //    + " " + data[i]["char_exp"]
            //    + " " + data[i]["reward_id_list"]
            //    + " " + data[i]["reward_count_list"]
            //    + " " + data[i]["first_reward_id"]
            //    + " " + data[i]["first_reward_count"]
            //    + " " + data[i]["map_resource"]
            //    + " " + data[i]["bgm_sound_id"]
            //    );

            DBStageData stageData = new DBStageData();
            stageData.id = Convert.ToInt32(data[i]["id"]);
            stageData.stageType = Convert.ToInt32(data[i]["stage_type"]);
            stageData.stageFloor = Convert.ToInt32(data[i]["stage_floor"]);
            stageData.stageString = Convert.ToString(data[i]["stage_string"]);
            stageData.needStageId = Convert.ToInt32(data[i]["need_stage_id"]);
            stageData.stageGroupIndex = Convert.ToInt32(data[i]["stage_group_index"]);
            stageData.needEntranceItemId = Convert.ToInt32(data[i]["need_entrance_item_id"]);
            stageData.needEntranceItemCount = Convert.ToInt32(data[i]["need_entrance_item_count"]);
            stageData.enemyLevelMin = Convert.ToInt32(data[i]["enemy_level_min"]);
            stageData.enemyLevelMax = Convert.ToInt32(data[i]["enemy_level_max"]);
            stageData.enemyIdList = new List<int>();
            // List로 넣기 data[i]["monster_id_list"]
            stageData.enemyPositionList = new List<int>();
            // List로 넣기 data[i]["monster_position_list"]
            stageData.bossLevel = Convert.ToInt32(data[i]["boss_level"]);
            stageData.bossPassiveList = new List<int>();
            // List로 넣기 data[i]["boss_passive_list"]
            stageData.bossActiveList = new List<int>();
            // List로 넣기 data[i]["boss_skill_list"]
            stageData.rankExp = Convert.ToInt32(data[i]["rank_exp"]);
            stageData.charExp = Convert.ToInt32(data[i]["char_exp"]);
            stageData.rewardIdList = new List<int>();
            // List로 넣기 data[i]["reward_id"]
            stageData.rewardCountList = new List<int>();
            // List로 넣기 data[i]["reward_count"]
            stageData.firstRewardId = Convert.ToInt32(data[i]["first_reward_id"]);
            stageData.firstRewardCount = Convert.ToInt32(data[i]["first_reward_count"]);
            stageData.mapResource = Convert.ToString(data[i]["map_resource"]);
            stageData.bgmSoundId = Convert.ToInt32(data[i]["bgm_sound_id"]);

            DBStageDataDic.Add(stageData.id, stageData);
        }
    }

    public void SetServantData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_servant");

        for (var i = 3; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["name"]
            //    + " " + data[i]["job"]
            //    + " " + data[i]["resource_body"]
            //    + " " + data[i]["resource_head"]
            //    + " " + data[i]["resource_hair"]
            //    + " " + data[i]["resource_icon"]
            //    );

            DBServantData servantData = new DBServantData();
            servantData.id = Convert.ToInt32(data[i]["id"]);
            servantData.name = Convert.ToString(data[i]["name"]);
            servantData.job = Convert.ToString(data[i]["job"]);
            servantData.resourceBody = Convert.ToString(data[i]["resource_body"]);
            servantData.resourceHead = Convert.ToString(data[i]["resource_head"]);
            servantData.resourceHair = Convert.ToString(data[i]["resource_hair"]);
            // Data 추가 예정
            //servantData.resourceIcon = Convert.ToString(data[i]["ingame_icon_name"]);

            DBServantDataDic.Add(servantData.id, servantData);
        }
    }

    public void SetLocalizationData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/LocalData");

        for (var i = 3; i < data.Count; i++)
        {
            Debug.Log("index " + (i).ToString()
                + " : " + data[i]["index"]
                + " " + data[i]["kr"]
                + " " + data[i]["ch"]
                + " " + data[i]["en"]
                );

            DBLocalizationData localData = new DBLocalizationData();
            localData.id     = Convert.ToInt32(data[i]["index"]);
            localData.krText    = Convert.ToString(data[i]["kr"]);
            localData.chText    = Convert.ToString(data[i]["ch"]);
            localData.enText    = Convert.ToString(data[i]["en"]);

            DBLocalDataDic.Add(localData.id, localData);
        }
    }

    public void SetMonsterData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_monster");

        for (var i = 3; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["name"]
            //    + " " + data[i]["element_type"]
            //    + " " + data[i]["class_type"]
            //    + " " + data[i]["resource_model"]
            //    + " " + data[i]["resource_icon"]
            //    );

            DBMonsterData monsterData = new DBMonsterData();
            monsterData.id     = Convert.ToInt32(data[i]["id"]);
            monsterData.name = Convert.ToString(data[i]["enname"]);
            // TODO : 로컬 적용 후 아래로 변경
            // monsterData.name            = Convert.ToString(data[i]["name"]);
            monsterData.elementType = Convert.ToInt32(data[i]["element_type"]);
            monsterData.classType = Convert.ToInt32(data[i]["class_type"]);
            monsterData.resourceModel = Convert.ToString(data[i]["resource_model"]);
            monsterData.resourceIcon = Convert.ToString(data[i]["resource_icon"]);

            DBMonsterDataDic.Add(monsterData.id, monsterData);
            monsterDataInspector.Add(monsterData);
        }
    }

    public void SetMonsterUpgradeData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_monster_upgrade");

        for (var i = 3; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["grade_1"]
            //    + " " + data[i]["grade_2"]
            //    + " " + data[i]["success_per"]
            //    );

            DBMonsterUpgradeData upgradeData = new DBMonsterUpgradeData();
            upgradeData.id = (Convert.ToInt32(data[i]["grade_1"]) * 100) + Convert.ToInt32(data[i]["grade_2"]);
            upgradeData.successPer = Convert.ToDouble(data[i]["success_per"]);

            DBMonsterUpgradeDataDic.Add(upgradeData.id, upgradeData);
        }
    }

    #endregion

    #region GetFunction

    public string GetLocalizationText(int index)
    {
        if (DBLocalDataDic.ContainsKey(index) == false)
        {
            return null;
        }

        switch (localType)
        {
            case LOCALIZATION_TYPE.KR:
                return DBLocalDataDic[index].krText;

            case LOCALIZATION_TYPE.CH:
                return DBLocalDataDic[index].chText;

            case LOCALIZATION_TYPE.EN:
                return DBLocalDataDic[index].enText;

            default:
                return null;
        }
    }

    public List<int> GetMonsterIndexList()
    {
        if (DBMonsterDataDic == null)
        {
            Debug.LogError("MonsterDataDic Error");
            return null;
        }

        return DBMonsterDataDic.Keys.ToList();
    }

    public string GetMonsterDBResourceModel(int index)
    {
        if (DBMonsterDataDic.ContainsKey(index) == false)
        {
            return null;
        }

        return DBMonsterDataDic[index].resourceModel;
    }

    public int GetRandomMonsterIndex()
    {
        int monsterIndex = monsterDataInspector[UnityEngine.Random.Range(0, monsterDataInspector.Count)].id;

        return monsterIndex;
    }

    public string GetMonsterInGameIconName(int index)
    {
        if (DBMonsterDataDic.ContainsKey(index) == false)
        {
            return null;
        }

        return DBMonsterDataDic[index].resourceIcon;
    }

    public string GetMonsterName(int monsterIndex)
    {
        if(DBMonsterDataDic.ContainsKey(monsterIndex) == false)
        {
            return null;
        }

        string monsterName = DBMonsterDataDic[monsterIndex].name;

        return monsterName;

        // Monster Name을 통한 Local Return
        //int nameIndex = 1;
        //return GetLocalizationText(nameIndex);
    }

    public double GetMonsterUpgradePer(int mainMonsterUpgrade, int subMonsterUpgrade)
    {
        int id = (mainMonsterUpgrade * 100) + subMonsterUpgrade;

        if (DBMonsterUpgradeDataDic.ContainsKey(id) == false)
        {
            Debug.LogError("MonsterUpgradeDataDic Error");
            return 0;
        }

        return DBMonsterUpgradeDataDic[id].successPer;
    }

    #endregion
}
