using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;

public class CSVData : MonoSingleton<CSVData> {

    public Dictionary<int, DBStageData> DBStageDataDic = new Dictionary<int, DBStageData>();
    public Dictionary<int, DBLocalizationData> DBLocalDataDic = new Dictionary<int, DBLocalizationData>();
    private LOCALIZATION_TYPE localType;
    public Dictionary<int, DBMonsterData> DBMonsterDataDic = new Dictionary<int, DBMonsterData>();

    //  인스펙터에서 보여주기 위한...
    public List<DBMonsterData> monsterDataInspector = new List<DBMonsterData>();

    void Start()
    {
        //SetStageData();
        //SetLocalizationData();
        SetMonsterData();
        localType = LOCALIZATION_TYPE.EN;
    }

    #region SetFunction

    public void SetStageData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DBStageData");

        for (var i = 3; i < data.Count; i++)
        {
            Debug.Log("index " + (i).ToString()
                + " : " + data[i]["id"]
                + " " + data[i]["type"]
                + " " + data[i]["stage_string"]
                + " " + data[i]["need_stage_id"]
                + " " + data[i]["next_stage_id"]
                + " " + data[i]["stage_group_list"]
                + " " + data[i]["need_entrance_item_id"]
                + " " + data[i]["need_entrance_item_count"]
                + " " + data[i]["monster_level_min"]
                + " " + data[i]["monster_level_max"]
                + " " + data[i]["monster_id_list"]
                + " " + data[i]["monster_position"]
                + " " + data[i]["boss_monster_level"]
                + " " + data[i]["boss_passive_list"]
                + " " + data[i]["boss_skill_list"]
                + " " + data[i]["char_exp"]
                + " " + data[i]["reward_id_list"]
                + " " + data[i]["reward_count_list"]
                + " " + data[i]["first_reward_id"]
                + " " + data[i]["first_reward_count"]
                + " " + data[i]["map_filename"]
                + " " + data[i]["bgm_sound_id"]
                );

            DBStageData stageData = new DBStageData();
            stageData.id = Convert.ToInt32(data[i]["id"]);
            stageData.type = Convert.ToInt32(data[i]["type"]);
            stageData.stageString = Convert.ToString(data[i]["stage_string"]);
            stageData.needStageId = Convert.ToInt32(data[i]["need_stage_id"]);
            stageData.nextStageId = Convert.ToInt32(data[i]["next_stage_id"]);
            stageData.stageGroupList = Convert.ToInt32(data[i]["stage_group_list"]);
            stageData.needEntranceItemId = Convert.ToInt32(data[i]["need_entrance_item_id"]);
            stageData.needEntranceItemCount = Convert.ToInt32(data[i]["need_entrance_item_count"]);
            stageData.monsterLevelMin = Convert.ToInt32(data[i]["monster_level_min"]);
            stageData.monsterLevelMax = Convert.ToInt32(data[i]["monster_level_max"]);
            stageData.monsterIdList = new List<int>();
            // List로 넣기 data[i]["monster_id_list"]
            stageData.monsterPositionList = new List<int>();
            // List로 넣기 data[i]["monster_position_list"]
            stageData.bossMonsterLevel = Convert.ToInt32(data[i]["boss_monster_level"]);
            stageData.bossPassiveList = new List<int>();
            // List로 넣기 data[i]["boss_passive_list"]
            stageData.bossSkillList = new List<int>();
            // List로 넣기 data[i]["boss_skill_list"]
            stageData.charExp = Convert.ToInt32(data[i]["char_exp"]);
            stageData.rewardIdList = new List<int>();
            // List로 넣기 data[i]["reward_id"]
            stageData.rewardIdList = new List<int>();
            // List로 넣기 data[i]["reward_count"]
            stageData.firstRewardId = Convert.ToInt32(data[i]["first_reward_id"]);
            stageData.firstRewardCount = Convert.ToInt32(data[i]["first_reward_count"]);
            stageData.mapFileName = Convert.ToString(data[i]["map_filename"]);
            stageData.bgmSoundId = Convert.ToInt32(data[i]["bgm_sound_id"]);

            DBStageDataDic.Add(stageData.id, stageData);
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
            localData.index     = Convert.ToInt32(data[i]["index"]);
            localData.krText    = Convert.ToString(data[i]["kr"]);
            localData.chText    = Convert.ToString(data[i]["ch"]);
            localData.enText    = Convert.ToString(data[i]["en"]);

            DBLocalDataDic.Add(localData.index, localData);
        }
    }

    public void SetMonsterData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/MonsterData");

        for (var i = 3; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["name"]
            //    + " " + data[i]["resource_base"]
            //    + " " + data[i]["potrait_base"]
            //    + " " + data[i]["stat_main"]
            //    );

            DBMonsterData monsterData = new DBMonsterData();
            monsterData.indexNumber     = Convert.ToInt32(data[i]["id"]);
            monsterData.name = Convert.ToString(data[i]["enname"]);
            // TODO : 로컬 적용 후 아래로 변경
            // monsterData.name            = Convert.ToString(data[i]["name"]);
            monsterData.resource        = Convert.ToString(data[i]["resource_base"]);
            monsterData.inGameIconName  = Convert.ToString(data[i]["potrait_base"]);

            DBMonsterDataDic.Add(monsterData.indexNumber, monsterData);
            monsterDataInspector.Add(monsterData);
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
            Debug.LogError("MonsterDataBaseDic Error");
            return null;
        }

        return DBMonsterDataDic.Keys.ToList();
    }

    public string GetMonsterDBResource(int index)
    {
        if (DBMonsterDataDic.ContainsKey(index) == false)
        {
            return null;
        }

        return DBMonsterDataDic[index].resource;
    }

    public int GetRandomMonsterIndex()
    {
        int monsterIndex = monsterDataInspector[UnityEngine.Random.Range(0, monsterDataInspector.Count)].indexNumber;

        return monsterIndex;
    }

    public string GetMonsterInGameIconName(int index)
    {
        if (DBMonsterDataDic.ContainsKey(index) == false)
        {
            return null;
        }

        return DBMonsterDataDic[index].inGameIconName;
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

    #endregion
}
