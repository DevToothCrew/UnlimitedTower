﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;

public class CSVData : MonoSingleton<CSVData>
{
    public Dictionary<int, DBItemData> DBItemDataDic = new Dictionary<int, DBItemData>();
    public Dictionary<int, DBEquipmentData> DBEquipmentDataDic = new Dictionary<int, DBEquipmentData>();
    public Dictionary<int, DBStageData> DBStageDataDic = new Dictionary<int, DBStageData>();
    public Dictionary<int, DBStageEnemyData> DBStageEnemyDataDic = new Dictionary<int, DBStageEnemyData>();
    public Dictionary<int, DBLocalizationData> DBLocalDataDic = new Dictionary<int, DBLocalizationData>();
    private LOCALIZATION_TYPE localType;
    public Dictionary<int, DBServantData> DBServantDataDic = new Dictionary<int, DBServantData>();
    public Dictionary<SERVANT_JOB, DBServantStatData> DBServantStatDataDic = new Dictionary<SERVANT_JOB, DBServantStatData>();
    public Dictionary<int, DBMonsterData> DBMonsterDataDic = new Dictionary<int, DBMonsterData>();
    public Dictionary<MONSTER_CLASS, DBMonsterStatData> DBMonsterStatDataDic = new Dictionary<MONSTER_CLASS, DBMonsterStatData>();
    public Dictionary<GRADE_TYPE, DBGradeResourceData> DBGradeResourceDataDic = new Dictionary<GRADE_TYPE, DBGradeResourceData>();
    public Dictionary<int, DBMonsterUpgradeData> DBMonsterUpgradeDataDic = new Dictionary<int, DBMonsterUpgradeData>();

    //  인스펙터에서 보여주기 위한...
    public List<DBMonsterData> monsterDataInspector = new List<DBMonsterData>();
    public List<DBServantData> servantDataInspector = new List<DBServantData>();

    public void Awake()
    {
        SetCSVData();
    }

    public void InitCSVData()
    {
        SetCSVData();

#if UNITY_EDITOR
        {
            Cheat.Inst.RequestLoginCheat();
        }
#else
        {
            PacketManager.Inst.RequestLoginWithScatter();
        }
#endif
    }

    public void SetCSVData()
    {
        //Debug.Log("SetCSVData Start");

        //SetLocalizationData();

        // 스킬 데이터 추가 필요
        // 스텟 데이터 추가 필요
        // 기타 데이터 추가 필요
        if (DBItemDataDic.Count == 0)
        {
            //Debug.Log("SetItemData Start");
            if(SetItemData() == false)
            {
                Debug.Log("Invalid DBSetItemData");
            }
            //Debug.Log("SetItemData Success");
        }
        if (DBEquipmentDataDic.Count == 0)
        {
            //Debug.Log("SetEquipmentData Start");
            if (SetEquipmentData() == false)
            {
                Debug.Log("Invalid DBSetEquipmentData");
            }
            //Debug.Log("SetEquipmentData Success");
        }
        if (DBStageDataDic.Count == 0)
        {
            //Debug.Log("SetStageData Start");
            if(SetStageData() == false)
            {
                Debug.Log("Invalid DBStageData");
            }
            //Debug.Log("SetStageData Success");
        }
        if (DBStageEnemyDataDic.Count == 0)
        {
            //Debug.Log("SetStageEnemyData Start");
            if(SetStageEnemyData() == false)
            {
                Debug.Log("Invalid DBStageEnemyData");
            }
            //Debug.Log("SetStageEnemyData Success");
        }
        if(DBServantStatDataDic.Count == 0)
        {
            //Debug.Log("SetServantStatData Start");
            if(SetServantStatData() == false)
            {
                Debug.Log("Invalid DBServantStatData");
            }
            //Debug.Log("SetServantStatData Success");
        }
        if (DBServantDataDic.Count == 0)
        {
            //Debug.Log("SetServantData Start");
            if(SetServantData() == false)
            {
                Debug.Log("Invalid DBServantData");
            }
            //Debug.Log("SetServantData Success");
        }
        if (DBMonsterStatDataDic.Count == 0)
        {
            //Debug.Log("SetMonsterStatData Start");
            if(SetMonsterStatData() == false)
            {
                Debug.Log("Invalid DBMonsterStatData");
            }
            //Debug.Log("SetMonsterStatData Success");
        }
        if (DBMonsterDataDic.Count == 0)
        {
            //Debug.Log("SetMonsterData Start");
            if(SetMonsterData() == false)
            {
                Debug.Log("Invalid DBMonsterData");
            }
            //Debug.Log("SetMonsterData Success");
        }
        if (DBMonsterUpgradeDataDic.Count == 0)
        {
            //Debug.Log("SetMonsterUpgradeData Start");
            if(SetMonsterUpgradeData() == false)
            {
                Debug.Log("Invalid DBMonsterUpgradeData");
            }
            //Debug.Log("SetMonsterUpgradeData Success");
        }
        if (DBGradeResourceDataDic.Count == 0)
        {
            //Debug.Log("SetGradeResourceData Start");
            if(SetGradeResourceData() == false)
            {
                Debug.Log("Invalid DBGradeResourceData");
            }
            //Debug.Log("SetGradeResourceData Success");
        }
        localType = LOCALIZATION_TYPE.EN;
    }

    #region SetFunction

    public bool SetItemData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_item");
        for (var i = 2; i < data.Count; i++)
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
            if (DBItemDataDic.ContainsKey(itemData.id) == true)
            {
                Debug.Log("Item ID가 겹쳐요 : " + itemData.id);
                return false;
            }
            itemData.name = Convert.ToString(data[i]["name"]);
            itemData.resourceIcon = Convert.ToString(data[i]["resource_icon"]);
            itemData.description = Convert.ToString(data[i]["description"]);
            itemData.tier = Convert.ToInt32(data[i]["tier"]);
            itemData.itemType = Convert.ToString(data[i]["item_type"]);
        
            itemData.itemParamIDList = new List<int>();
            // Param List검사 추가 필요

            itemData.sellItemInfo.id = Convert.ToInt32(data[i]["sell_item_id"]);
            itemData.sellItemInfo.count = Convert.ToInt32(data[i]["sell_item_count"]);

            DBItemDataDic.Add(itemData.id, itemData);
        }

        return true;
    }

    public bool SetEquipmentData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_equipment");
        // 관리 텍스트 버그로 삭제로 인한 1부터 시작
        for (var i = 1; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["name"]
            //    + " " + data[i]["job_limit_bit"]
            //    + " " + data[i]["tier"]
            //    + " " + data[i]["equip_type"]
            //    + " " + data[i]["option_list"]
            //    + " " + data[i]["option_value_min"]
            //    + " " + data[i]["option_value_max"]
            //    + " " + data[i]["upgrade_option_value"]
            //    + " " + data[i]["grade_multi_list"]
            //    + " " + data[i]["resource_icon"]
            //    + " " + data[i]["sell_item_id"]
            //    + " " + data[i]["sell_item_count"]
            //    );

            DBEquipmentData equipmentData = new DBEquipmentData();
            equipmentData.id = Convert.ToInt32(data[i]["id"]);
            if (DBEquipmentDataDic.ContainsKey(equipmentData.id) == true)
            {
                Debug.Log("Equipment ID가 겹쳐요 : " + equipmentData.id);
                return false;
            }
            equipmentData.name = Convert.ToString(data[i]["name"]);
            equipmentData.jobLimit = (SERVANT_EQUIPMENT_FLAG)Convert.ToInt32(Convert.ToString(data[i]["job_limit_bit"]), 2);
            //Debug.Log("Equip Test Warrior : " + equipmentData.isEquipable(SERVANT_EQUIPMENT_FLAG.Warrior));
            //Debug.Log("Equip Test Thief : " + equipmentData.isEquipable(SERVANT_EQUIPMENT_FLAG.Thief));
            //Debug.Log("Equip Test Cleric : " + equipmentData.isEquipable(SERVANT_EQUIPMENT_FLAG.Cleric));
            //Debug.Log("Equip Test Archer : " + equipmentData.isEquipable(SERVANT_EQUIPMENT_FLAG.Archer));
            //Debug.Log("Equip Test Magician : " + equipmentData.isEquipable(SERVANT_EQUIPMENT_FLAG.Magician));
            //Debug.Log("Equip Test All : " + equipmentData.isEquipable(SERVANT_EQUIPMENT_FLAG.All));

            equipmentData.tier = Convert.ToInt32(data[i]["tier"]);
            equipmentData.equipmentType = (EQUIPMENT_TYPE)Convert.ToInt32(data[i]["equip_type"]);
            equipmentData.optionType = (EQUIPMENT_OPTION_TYPE)Convert.ToInt32(data[i]["option_list"]);
            equipmentData.optionMin = Convert.ToInt32(data[i]["option_value_min"]);
            equipmentData.optionMax = Convert.ToInt32(data[i]["option_value_max"]);
            equipmentData.upgradeValue = Convert.ToInt32(data[i]["upgrade_option_value"]);

            string[] gmList = Convert.ToString(data[i]["grade_multi_list"]).Split('/');
            for (GRADE_TYPE j = GRADE_TYPE.COMMON; j != GRADE_TYPE.LEGENDARY; j--)
            {
                equipmentData.gradeMultiValueDic.Add(j, Convert.ToDouble(gmList[(5 - (int)j)]));
            }

            equipmentData.resourceIcon = Convert.ToString(data[i]["resource_icon"]);
            equipmentData.sellItemInfo.id = Convert.ToInt32(data[i]["sell_item_id"]);
            equipmentData.sellItemInfo.count = Convert.ToInt32(data[i]["sell_item_count"]);
            // TODO : Type을 Item에서 받아오는걸 나중에 추가??
            equipmentData.sellItemInfo.type = 0;

            DBEquipmentDataDic.Add(equipmentData.id, equipmentData);
        }

        return true;
    }

    public bool SetStageData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_stage");
        for (var i = 2; i < data.Count; i++)
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
            string[] eiList = Convert.ToString(data[i]["enemy_id_list"]).Split('/');
            for (int j = 0; j < eiList.Length; j++)
            {
                stageData.enemyIdList.Add(Convert.ToInt32(eiList[j]));
            }
        
            string[] epList = Convert.ToString(data[i]["enemy_position"]).Split('/');
            for (int j = 0; j < epList.Length; j++)
            {
                stageData.enemyPositionList.Add(Convert.ToInt32(epList[j]));
            }
        
            stageData.bossLevel = Convert.ToInt32(data[i]["boss_level"]);
            // List로 넣기 data[i]["boss_passive_list"]
            // List로 넣기 data[i]["boss_skill_list"]
            stageData.rankExp = Convert.ToInt32(data[i]["rank_exp"]);
            stageData.charExp = Convert.ToInt32(data[i]["char_exp"]);
            // List로 넣기 data[i]["reward_id"]
            // List로 넣기 data[i]["reward_count"]
            stageData.firstRewardId = Convert.ToInt32(data[i]["first_reward_id"]);
            stageData.firstRewardCount = Convert.ToInt32(data[i]["first_reward_count"]);
            stageData.mapResource = Convert.ToString(data[i]["map_resource"]);
            stageData.bgmSoundId = Convert.ToInt32(data[i]["bgm_sound_id"]);
        
            DBStageDataDic.Add(stageData.id, stageData);
        }

        return true;
    }

    public bool SetStageEnemyData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_stage_enemy");
        for (var i = 2; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["char_type"]
            //    + " " + data[i]["char_id"]
            //    + " " + data[i]["char_grade"]
            //    + " " + data[i]["str"]
            //    + " " + data[i]["dex"]
            //    + " " + data[i]["int"]
            //    + " " + data[i]["cri_per"]
            //    + " " + data[i]["cri_dmg"]
            //    + " " + data[i]["mcri_per"]
            //    + " " + data[i]["mcri_dmg"]
            //    + " " + data[i]["spd"]
            //    + " " + data[i]["avd"]
            //    + " " + data[i]["active_list"]
            //    + " " + data[i]["passive_list"]
            //    );

            DBStageEnemyData stageEnemyData = new DBStageEnemyData();
            stageEnemyData.id = Convert.ToInt32(data[i]["id"]);

            switch (Convert.ToString(data[i]["char_type"]))
            {
                case "monster":
                    stageEnemyData.charType = CHAR_TYPE.MONSTER;
                    break;
                case "servant":
                    stageEnemyData.charType = CHAR_TYPE.SERVANT;
                    break;

                default:
                    return false;
            }

            stageEnemyData.charID = Convert.ToInt32(data[i]["char_id"]);
            switch (Convert.ToString(data[i]["char_grade"]))
            {
                case "common":
                    stageEnemyData.grade = GRADE_TYPE.COMMON;
                    break;
                case "uncommon":
                    stageEnemyData.grade = GRADE_TYPE.UNCOMMON;
                    break;
                case "rare":
                    stageEnemyData.grade = GRADE_TYPE.RARE;
                    break;
                case "unique":
                    stageEnemyData.grade = GRADE_TYPE.UNIQUE;
                    break;
                case "legendary":
                    stageEnemyData.grade = GRADE_TYPE.LEGENDARY;
                    break;
                default:
                    Debug.Log("Invalid Request ID : " + i + " charGrade : " + Convert.ToString(data[i]["char_grade"]));
                    return false;
            }

            stageEnemyData.status.basicStr = Convert.ToInt32(data[i]["str"]);
            stageEnemyData.status.basicDex = Convert.ToInt32(data[i]["dex"]);
            stageEnemyData.status.basicInt = Convert.ToInt32(data[i]["int"]);

            stageEnemyData.criPer = Convert.ToInt32(data[i]["cri_per"]);
            stageEnemyData.criDmg = Convert.ToInt32(data[i]["cri_dmg"]);
            stageEnemyData.mcriPer = Convert.ToInt32(data[i]["mcri_per"]);
            stageEnemyData.mcriDmg = Convert.ToInt32(data[i]["mcri_dmg"]);
            stageEnemyData.speed = Convert.ToInt32(data[i]["spd"]);
            stageEnemyData.avoid = Convert.ToInt32(data[i]["avd"]);

            DBStageEnemyDataDic.Add(stageEnemyData.id, stageEnemyData);
        }

        return true;
    }

    public bool SetServantStatData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_stat_servant");
        for (var i = 2; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["job"]
            //    + " " + data[i]["speed"]
            //    + " " + data[i]["avoid"]
            //    + " " + data[i]["cri_per"]
            //    + " " + data[i]["cri_dmg"]
            //    + " " + data[i]["mcri_per"]
            //    + " " + data[i]["mcri_dmg"]
            //    );

            DBServantStatData servantStatData = new DBServantStatData();
            servantStatData.jobEnum = (SERVANT_JOB)Convert.ToInt32(data[i]["id"]);
            servantStatData.speed = Convert.ToInt32(data[i]["speed"]);
            servantStatData.avoid = Convert.ToInt32(data[i]["avoid"]);
            servantStatData.criPer = Convert.ToInt32(data[i]["cri_per"]);
            servantStatData.criDmg = Convert.ToInt32(data[i]["cri_dmg"]);
            servantStatData.mcriPer = Convert.ToInt32(data[i]["mcri_per"]);
            servantStatData.mcriDmg = Convert.ToInt32(data[i]["mcri_dmg"]);

            DBServantStatDataDic.Add(servantStatData.jobEnum, servantStatData);
        }

        return true;
    }

    public bool SetServantData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_servant");
        for (var i = 2; i < data.Count; i++)
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
            servantData.job = Convert.ToInt32(data[i]["job"]);

            if(DBServantStatDataDic.ContainsKey((SERVANT_JOB)servantData.job) == true)
            {
                DBServantStatData statData = DBServantStatDataDic[(SERVANT_JOB)servantData.job];
                servantData.speed = statData.speed;
                servantData.avoid = statData.avoid;
                servantData.criPer = statData.criPer;
                servantData.criDmg = statData.criDmg;
                servantData.mcriPer = statData.mcriPer;
                servantData.mcriDmg = statData.mcriDmg;
            }
            else
            {
                Debug.Log("Invalid Job Num : " + servantData.job);
                return false;
            }

            servantData.body = Convert.ToInt32(data[i]["body"]);
            servantData.gender = Convert.ToInt32(data[i]["gender"]);
            servantData.head = Convert.ToInt32(data[i]["head"]);
            servantData.hair = Convert.ToInt32(data[i]["hair"]);

            servantData.name = ((SERVANT_JOB)servantData.job).ToString();
            switch (Convert.ToString(data[i]["grade"]))
            {
                case "legendary":
                    servantData.grade = GRADE_TYPE.LEGENDARY;
                    break;

                case "common":
                    servantData.grade = GRADE_TYPE.COMMON;
                    break;

                default:
                    Debug.LogError("Invalid Request DBServant");
                    return false;
            }

            servantData.resourceBody = Convert.ToString(data[i]["resource_body"]);
            servantData.resourceHead = Convert.ToString(data[i]["resource_head"]);
            servantData.resourceHair = Convert.ToString(data[i]["resource_hair"]);
            servantData.resourceIcon = Convert.ToString(data[i]["resource_icon"]);
            servantData.servantIcon = Resources.Load<Sprite>("Character Portrait Image/Servant/" + servantData.resourceIcon);
            if (servantData.servantIcon == null)
            {
                Debug.Log("Invalid Icon Resource : " + servantData.resourceIcon + " No : " + (DBServantDataDic.Count));
            }

            DBServantDataDic.Add(servantData.id, servantData);
        }

        servantDataInspector = DBServantDataDic.Values.ToList();

        return true;
    }

    public bool SetLocalizationData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/LocalData");
        for (var i = 2; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["index"]
            //    + " " + data[i]["kr"]
            //    + " " + data[i]["ch"]
            //    + " " + data[i]["en"]
            //    );

            DBLocalizationData localData = new DBLocalizationData();
            localData.id = Convert.ToInt32(data[i]["index"]);
            localData.krText = Convert.ToString(data[i]["kr"]);
            localData.chText = Convert.ToString(data[i]["ch"]);
            localData.enText = Convert.ToString(data[i]["en"]);

            DBLocalDataDic.Add(localData.id, localData);
        }

        return true;
    }

    public bool SetMonsterStatData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_stat_monster");
        for (var i = 2; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["class"]
            //    + " " + data[i]["speed"]
            //    + " " + data[i]["avoid"]
            //    + " " + data[i]["cri_per"]
            //    + " " + data[i]["cri_dmg"]
            //    + " " + data[i]["mcri_per"]
            //    + " " + data[i]["mcri_dmg"]
            //    );

            DBMonsterStatData monsterStatData = new DBMonsterStatData();
            monsterStatData.classEnum = (MONSTER_CLASS)Convert.ToInt32(data[i]["id"]);
            monsterStatData.speed = Convert.ToInt32(data[i]["speed"]);
            monsterStatData.avoid = Convert.ToInt32(data[i]["avoid"]);
            monsterStatData.criPer = Convert.ToInt32(data[i]["cri_per"]);
            monsterStatData.criDmg = Convert.ToInt32(data[i]["cri_dmg"]);
            monsterStatData.mcriPer = Convert.ToInt32(data[i]["mcri_per"]);
            monsterStatData.mcriDmg = Convert.ToInt32(data[i]["mcri_dmg"]);

            DBMonsterStatDataDic.Add(monsterStatData.classEnum, monsterStatData);
        }

        return true;
    }

    public bool SetMonsterData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_monster");
        for (var i = 2; i < data.Count; i++)
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
            monsterData.id = Convert.ToInt32(data[i]["id"]);
            monsterData.name = Convert.ToString(data[i]["enname"]);
            // TODO : 로컬 적용 후 아래로 변경
            // monsterData.name            = Convert.ToString(data[i]["name"]);
            monsterData.elementType = Convert.ToInt32(data[i]["element_type"]);
            monsterData.classType = Convert.ToInt32(data[i]["class_type"]);

            if (DBMonsterStatDataDic.ContainsKey((MONSTER_CLASS)monsterData.classType) == true)
            {
                DBMonsterStatData statData = DBMonsterStatDataDic[(MONSTER_CLASS)monsterData.classType];
                monsterData.speed = statData.speed;
                monsterData.avoid = statData.avoid;
                monsterData.criPer = statData.criPer;
                monsterData.criDmg = statData.criDmg;
                monsterData.mcriPer = statData.mcriPer;
                monsterData.mcriDmg = statData.mcriDmg;
            }
            else
            {
                Debug.Log("Invalid Class Num : " + monsterData.classType);
                return false;
            }

            monsterData.resourceModel = Convert.ToString(data[i]["resource_model"]);
            monsterData.resourceIcon = Convert.ToString(data[i]["resource_icon"]);
            monsterData.monsterIcon = Resources.Load<Sprite>("Character Portrait Image/Monster/" + monsterData.resourceIcon);
            if (monsterData.monsterIcon == null)
            {
                Debug.Log("Invalid Icon Resource : " + monsterData.resourceIcon + " No : " + (monsterDataInspector.Count));
                return false;
            }

            DBMonsterDataDic.Add(monsterData.id, monsterData);
        }

        monsterDataInspector = DBMonsterDataDic.Values.ToList();

        return true;
    }

    public bool SetGradeResourceData()
    {
        for(GRADE_TYPE i = GRADE_TYPE.LEGENDARY; i <= GRADE_TYPE.COMMON; i++)
        {
            DBGradeResourceData resourceData = new DBGradeResourceData();
            resourceData.gradeType = i;
            resourceData.grade = Convert.ToInt32(i);
            resourceData.gradeIcon = Resources.Load<Sprite>(string.Format("UI/Common/grade_{0}", resourceData.grade));
        
            DBGradeResourceDataDic.Add(resourceData.gradeType, resourceData);
        }
        return true;
    }

    public Sprite GetSpriteGrade(GRADE_TYPE grade)
    {
        return DBGradeResourceDataDic[grade].gradeIcon;
    }

    public bool SetMonsterUpgradeData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_monster_upgrade");
        for (var i = 2; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["grade_1"]
            //    + " " + data[i]["grade_2"]
            //    + " " + data[i]["success_per"]
            //    + " " + data[i]["upgrade_price_id"]
            //    + " " + data[i]["upgrade_price_count"]
            //    );

            DBMonsterUpgradeData upgradeData = new DBMonsterUpgradeData();
            upgradeData.id = (Convert.ToInt32(data[i]["grade_1"]) * 100) + Convert.ToInt32(data[i]["grade_2"]);
            upgradeData.successPer = Convert.ToDouble(data[i]["success_per"]);
            upgradeData.needItem = new itemInfo();
            upgradeData.needItem.id = Convert.ToInt32(data[i]["upgrade_price_id"]);
            upgradeData.needItem.count = Convert.ToInt32(data[i]["upgrade_price_count"]);

            DBMonsterUpgradeDataDic.Add(upgradeData.id, upgradeData);
        }

        return true;
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

    public DBItemData GetItemData(int id)
    {
        if(DBItemDataDic.ContainsKey(id) == false)
        {
            Debug.LogError("Invalid ItemID");
            return null;
        }

        return DBItemDataDic[id];
    }

    public DBEquipmentData GetEquipmentData(int id)
    {
        if (DBEquipmentDataDic.ContainsKey(id) == false)
        {
            Debug.LogError("Invalid EquipmentID");
            return null;
        }

        return DBEquipmentDataDic[id];
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

    public int GetRandomMonsterID()
    {
        int monsterID = monsterDataInspector[UnityEngine.Random.Range(0, monsterDataInspector.Count)].id;

        return monsterID;
    }

    public int GetRandomServantID()
    {
        int servantID = servantDataInspector[UnityEngine.Random.Range(3, servantDataInspector.Count)].id;

        return servantID;
    }

    public string GetMonsterIcon(int index)
    {
        if (DBMonsterDataDic.ContainsKey(index) == false)
        {
            return null;
        }

        return DBMonsterDataDic[index].resourceIcon;
    }

    public string GetMonsterName(int monsterIndex)
    {
        if (DBMonsterDataDic.ContainsKey(monsterIndex) == false)
        {
            return null;
        }

        string monsterName = DBMonsterDataDic[monsterIndex].name;

        return monsterName;

        // Monster Name을 통한 Local Return
        //int nameIndex = 1;
        //return GetLocalizationText(nameIndex);
    }

    public DBMonsterData GetMonsterData(int id)
    {
        if (DBMonsterDataDic.ContainsKey(id) == false)
        {
            return null;
        }

        return DBMonsterDataDic[id];
    }

    public DBMonsterUpgradeData GetMonsterUpgradeData(int mainMonsterUpgrade, int subMonsterUpgrade)
    {
        int id = (mainMonsterUpgrade * 100) + subMonsterUpgrade;

        if (DBMonsterUpgradeDataDic.ContainsKey(id) == false)
        {
            Debug.LogError("MonsterUpgradeDataDic Error");
            return null;
        }

        return DBMonsterUpgradeDataDic[id];
    }

    public DBServantData GetServantData(int id)
    {
        if (DBServantDataDic.ContainsKey(id) == false)
        {
            return null;
        }

        return DBServantDataDic[id];
    }

    public int GetUserLevelByExp(int exp)
    {
        // User Exp Table 추가 후 수정

        return 1;
    }

    public int GetServantID(int grade, int job, int body, int gender, int head, int hair)
    {
        if (grade == 1)
        {
            if (job == 1)
            {
                return 1000001;
            }
            else if (job == 2)
            {
                return 1000002;
            }
            else if (job == 3)
            {
                return 1000003;
            }
        }

        int id = (job * 1000000) + (body * 100000) + (gender * 10000) + (head * 100) + hair;
        return id;
    }

    public DBStageData GetStageData(int stageType, int stageFloor)
    {
        int id = 100000 + (stageType * 100) + stageFloor;

        if (DBStageDataDic.ContainsKey(id) == false)
        {
            Debug.LogError("DBStageDataDic Error");
            return null;
        }

        return DBStageDataDic[id];
    }

    public DBStageEnemyData GetStageEnemyData(int id)
    {
        if (DBStageEnemyDataDic.ContainsKey(id) == false)
        {
            Debug.LogError("DBStageEnemyDataDic Error");
            return null;
        }

        return DBStageEnemyDataDic[id];
    }

    public int GetRankForExp(int exp)
    {
        return 1;
    }

    public int GetLevelForExp(int exp)
    {
        return 1;
    }

    #endregion
}
