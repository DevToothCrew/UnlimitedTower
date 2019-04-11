using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;

public class CSVData : MonoSingleton<CSVData>
{
    public Dictionary<int, DBExpData> DBExpDataDic = new Dictionary<int, DBExpData>();
    public Dictionary<int, DBItemData> DBItemDataDic = new Dictionary<int, DBItemData>();
    public Dictionary<int, DBEquipmentData> DBEquipmentDataDic = new Dictionary<int, DBEquipmentData>();
    public Dictionary<int, DBStageData> DBStageDataDic = new Dictionary<int, DBStageData>();
    public Dictionary<int, DBStageEnemyData> DBStageEnemyDataDic = new Dictionary<int, DBStageEnemyData>();
    public Dictionary<int, DBStageRewardData> DBStageRewardDataDic = new Dictionary<int, DBStageRewardData>();
    public Dictionary<int, DBLocalizationData> DBLocalDataDic = new Dictionary<int, DBLocalizationData>();
    private LOCALIZATION_TYPE localType;
    public Dictionary<int, DBServantData> DBServantDataDic = new Dictionary<int, DBServantData>();
    public Dictionary<SERVANT_JOB, DBServantStatData> DBServantStatDataDic = new Dictionary<SERVANT_JOB, DBServantStatData>();
    public Dictionary<int, DBMonsterData> DBMonsterDataDic = new Dictionary<int, DBMonsterData>();
    public Dictionary<MONSTER_CLASS, DBMonsterStatData> DBMonsterStatDataDic = new Dictionary<MONSTER_CLASS, DBMonsterStatData>();
    public Dictionary<int, DBMonsterUpgradeData> DBMonsterUpgradeDataDic = new Dictionary<int, DBMonsterUpgradeData>();
    public Dictionary<int, DBSkillActiveData> DBSkillActiveDataDic = new Dictionary<int, DBSkillActiveData>();
    public Dictionary<int, DBSkillPassiveData> DBSkillPassiveDataDic = new Dictionary<int, DBSkillPassiveData>();

    // Resource Data
    public Dictionary<GRADE_TYPE, DBGradeResourceData> DBGradeResourceDataDic = new Dictionary<GRADE_TYPE, DBGradeResourceData>();
    public Dictionary<GRADE_TYPE, DBGradeResourceData> DBGachaGradeResourceDataDic = new Dictionary<GRADE_TYPE, DBGradeResourceData>();
    public Dictionary<EQUIPMENT_OPTION_TYPE, DBOptionTypeResourceData> DBOptionTypeResourceDataDic = new Dictionary<EQUIPMENT_OPTION_TYPE, DBOptionTypeResourceData>();
    public Dictionary<ELEMENT_TYPE, DBElementResourceData> DBElementTypeResourceDataDic = new Dictionary<ELEMENT_TYPE, DBElementResourceData>();
    public Dictionary<SERVANT_JOB, DBServantJobResourceData> DBServantJobResourceDataDic = new Dictionary<SERVANT_JOB, DBServantJobResourceData>();

    //  인스펙터에서 보여주기 위한...
    public List<DBMonsterData> monsterDataInspector = new List<DBMonsterData>();
    public List<DBServantData> servantDataInspector = new List<DBServantData>();

    public void Awake()
    {
        SetCSVData();
    }

    public void InitCSVData()
    {
        PacketLoadingUI.Inst.SetLoading(true);

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
        if(DBExpDataDic.Count == 0)
        {
            //Debug.Log("SetExpData Start");
            if (SetExpData() == false)
            {
                Debug.Log("Invalid DBSetExpData");
            }
            //Debug.Log("SetExpData Success");
        }

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
        if (DBStageRewardDataDic.Count == 0)
        {
            //Debug.Log("SetStageRewardData Start");
            if (SetStageRewardData() == false)
            {
                Debug.Log("Invalid DBStageRewardData");
            }
            //Debug.Log("SetStageRewardData Success");
        }
        if (DBServantStatDataDic.Count == 0)
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
        if (DBSkillActiveDataDic.Count == 0)
        {
            //Debug.Log("SetSkillActiveData Start");
            if (SetSkillActiveData() == false)
            {
                Debug.Log("Invalid DBSkillActiveData");
            }
            //Debug.Log("SetSkillActiveData Success");
        }
        if (DBSkillPassiveDataDic.Count == 0)
        {
            //Debug.Log("SetSkillPassiveData Start");
            if (SetSkillPassiveData() == false)
            {
                Debug.Log("Invalid DBSkillPassiveData");
            }
            //Debug.Log("SetSkillPassiveData Success");
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
        if(DBGachaGradeResourceDataDic.Count == 0)
        {
            //Debug.Log("SetGachaGradeResourceData Start");
            if (SetGachaGradeResourceData() == false)
            {
                Debug.Log("Invalid DBGachaGradeResourceData");
            }
            //Debug.Log("SetGachaGradeResourceData Success");
        }
        if (DBOptionTypeResourceDataDic.Count == 0)
        {
            //Debug.Log("SetOptionTypeResourceData Start");
            if (SetOptionTypeResourceData() == false)
            {
                Debug.Log("Invalid DBOptionTypeResourceData");
            }
            //Debug.Log("SetOptionTypeResourceData Success");
        }
        if(DBElementTypeResourceDataDic.Count == 0)
        {
            if(SetElementTypeResourceData() == false)
            {
                Debug.Log("Invalid DBElementTypeResourceData");
            }
        }
        if (DBServantJobResourceDataDic.Count == 0)
        {
            if (SetServantJobResourceData() == false)
            {
                Debug.Log("Invalid DBServantJobResourceData");
            }
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

            //Debug.Log("Equip Job Able : " + Convert.ToString(data[i]["job"]));
            equipmentData.jobLimit = (SERVANT_JOB_FLAG)Convert.ToInt32(Convert.ToString(data[i]["job_limit"]), 2);
            if(equipmentData.jobLimit == SERVANT_JOB_FLAG.None)
            {
                Debug.Log("Invalid Data ID : " + equipmentData.id);
            }
            //Debug.Log("Equip Test Warrior : " + equipmentData.isEquipAble(SERVANT_JOB_FLAG.Warrior));
            //Debug.Log("Equip Test Thief : " + equipmentData.isEquipAble(SERVANT_JOB_FLAG.Thief));
            //Debug.Log("Equip Test Cleric : " + equipmentData.isEquipAble(SERVANT_JOB_FLAG.Cleric));
            //Debug.Log("Equip Test Archer : " + equipmentData.isEquipAble(SERVANT_JOB_FLAG.Archer));
            //Debug.Log("Equip Test Magician : " + equipmentData.isEquipAble(SERVANT_JOB_FLAG.Magician));
            //Debug.Log("Equip Test All : " + equipmentData.isEquipAble(SERVANT_JOB_FLAG.All));

            equipmentData.tier = Convert.ToInt32(data[i]["tier"]);
            equipmentData.equipmentType = (EQUIPMENT_TYPE)Convert.ToInt32(data[i]["equip_type"]);
            equipmentData.optionType = (EQUIPMENT_OPTION_TYPE)Convert.ToInt32(data[i]["option_list"]);
            equipmentData.optionMin = Convert.ToInt32(data[i]["option_value_min"]);
            equipmentData.optionMax = Convert.ToInt32(data[i]["option_value_max"]);
            equipmentData.upgradeValue = Convert.ToInt32(data[i]["upgrade_option_value"]);

            string[] gmList = Convert.ToString(data[i]["grade_multi_list"]).Split('/');
            for (GRADE_TYPE j = GRADE_TYPE.COMMON; j != GRADE_TYPE.NONE; j--)
            {
                //Debug.Log("Test GRADE_TYPE : " + j.ToString());
                //Debug.Log("Test gmList : " + Convert.ToDouble(gmList[(5 - (int)j)]));

                equipmentData.gradeMultiValueDic.Add(j, Convert.ToDouble(gmList[(5 - (int)j)]));
            }

            equipmentData.resourceIcon = Convert.ToString(data[i]["resource_icon"]);
            equipmentData.equipmentIcon = Resources.Load<Sprite>("UI/ItemIcons/" + equipmentData.resourceIcon);
            if (equipmentData.equipmentIcon == null)
            {
                Debug.Log("Invalid Icon Resource : " + equipmentData.resourceIcon);
            }

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
            stageEnemyData.grade = (GRADE_TYPE)Convert.ToInt32(data[i]["char_grade"]);

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

    public bool SetStageRewardData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_stage_reward");
        for (var i = 2; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["rank_exp"]
            //    + " " + data[i]["char_exp"]
            //    + " " + data[i]["first_reward_type"]
            //    + " " + data[i]["first_reward_id"]
            //    + " " + data[i]["first_reward_grade"]
            //    + " " + data[i]["first_reward_count"]
            //    + " " + data[i]["reward_utg_min"]
            //    + " " + data[i]["reward_utg_max"]
            //    + " " + data[i]["reward_type_1"]
            //    + " " + data[i]["reward_id_1"]
            //    + " " + data[i]["reward_grade_1"]
            //    + " " + data[i]["reward_per_1"]
            //    + " " + data[i]["reward_count_1"]
            //    + " " + data[i]["reward_type_2"]
            //    + " " + data[i]["reward_id_2"]
            //    + " " + data[i]["reward_grade_2"]
            //    + " " + data[i]["reward_per_2"]
            //    + " " + data[i]["reward_count_2"]
            //    + " " + data[i]["reward_type_3"]
            //    + " " + data[i]["reward_id_3"]
            //    + " " + data[i]["reward_grade_3"]
            //    + " " + data[i]["reward_per_3"]
            //    + " " + data[i]["reward_count_3"]
            //    + " " + data[i]["reward_type_4"]
            //    + " " + data[i]["reward_id_4"]
            //    + " " + data[i]["reward_grade_4"]
            //    + " " + data[i]["reward_per_4"]
            //    + " " + data[i]["reward_count_4"]
            //    + " " + data[i]["reward_type_5"]
            //    + " " + data[i]["reward_id_5"]
            //    + " " + data[i]["reward_grade_5"]
            //    + " " + data[i]["reward_per_5"]
            //    + " " + data[i]["reward_count_5"]
            //    + " " + data[i]["reward_type_6"]
            //    + " " + data[i]["reward_id_6"]
            //    + " " + data[i]["reward_grade_6"]
            //    + " " + data[i]["reward_per_6"]
            //    + " " + data[i]["reward_count_6"]
            //    );

            DBStageRewardData rewardData = new DBStageRewardData();
            rewardData.id = Convert.ToInt32(data[i]["id"]);
            if(DBStageRewardDataDic.ContainsKey(rewardData.id) == true)
            {
                Debug.LogError("Invalid Reward ID : " + rewardData.id);
                return false;
            }

            rewardData.rankExp = Convert.ToInt32(data[i]["rank_exp"]);
            rewardData.charExp = Convert.ToInt32(data[i]["char_exp"]);
            rewardData.firstRewardData = new DBRewardData();
            rewardData.firstRewardData.rewardType = GetRewardType(Convert.ToString(data[i]["first_reward_type"]));
            if(rewardData.firstRewardData.rewardType != REWARD_TYPE.NONE)
            {
                rewardData.firstRewardData.rewardID = Convert.ToInt32(data[i]["first_reward_id"]);
                rewardData.firstRewardData.rewardGrade = GetGradeType(Convert.ToString(data[i]["first_reward_grade"]));
                rewardData.firstRewardData.rewardCount = Convert.ToInt32(data[i]["first_reward_count"]);
            }

            rewardData.rewardMinUTG = Convert.ToInt32(data[i]["reward_utg_min"]);
            rewardData.rewardMaxUTG = Convert.ToInt32(data[i]["reward_utg_max"]);
            rewardData.rewardUTGString = rewardData.rewardMinUTG.ToString("N0") + " ~ " + rewardData.rewardMaxUTG.ToString("N0");

            for(int count = 1; count <= 6; count++)
            {
                DBRewardData reward = new DBRewardData();
                reward.rewardType = GetRewardType(Convert.ToString(data[i]["reward_type_" + count]));
                if (reward.rewardType == REWARD_TYPE.NONE)
                {
                    continue;
                }
                reward.rewardID = Convert.ToInt32(data[i]["reward_id_" + count]);
                reward.rewardGrade = GetGradeType(Convert.ToString(data[i]["reward_grade_" + count]));
                reward.rewardCount = Convert.ToInt32(data[i]["reward_count_" + count]);

                rewardData.rewardDataList.Add(reward);
            }

            DBStageRewardDataDic.Add(rewardData.id, rewardData);
        }

        return true;
    }

    public GRADE_TYPE GetGradeType(string type)
    {
        switch (type)
        {
            case "common":
                return GRADE_TYPE.COMMON;
            case "uncommon":
                return GRADE_TYPE.UNCOMMON;
            case "rare":
                return GRADE_TYPE.RARE;
            case "unique":
                return GRADE_TYPE.UNIQUE;
            case "legendary":
                return GRADE_TYPE.LEGENDARY;
            default:
                Debug.Log("Invalid Grade Type String : " + type);
                return GRADE_TYPE.NONE;
        }
    }

    public REWARD_TYPE GetRewardType(string type)
    {
        switch(type)
        {
            case "none":
                return REWARD_TYPE.NONE;
            case "servant":
                return REWARD_TYPE.SERVANT;
            case "monster":
                return REWARD_TYPE.MONSTER;
            case "equip":
                return REWARD_TYPE.EQUIPMENT;
            case "item":
                return REWARD_TYPE.ITEM;
            default:
                Debug.Log("Invalid Reward Type String : " + type);
                return REWARD_TYPE.NONE;
        }
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
            monsterData.elementType = (ELEMENT_TYPE)Convert.ToInt32(data[i]["element_type"]);
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

    public bool SetOptionTypeResourceData()
    {
        for (EQUIPMENT_OPTION_TYPE i = EQUIPMENT_OPTION_TYPE.ATK; i < EQUIPMENT_OPTION_TYPE.MAX; i++)
        {
            if(DBOptionTypeResourceDataDic.ContainsKey(i) == true)
            {
                Debug.Log("Invalid Key : " + i.ToString());
                return false;
            }

            DBOptionTypeResourceData resourceData = new DBOptionTypeResourceData();
            resourceData.optionType = i;
            resourceData.optionIcon = Resources.Load<Sprite>(string.Format("UI/Status/Status_{0}", resourceData.optionType.ToString()));

            DBOptionTypeResourceDataDic.Add(resourceData.optionType, resourceData);
        }
        return true;
    }

    public bool SetElementTypeResourceData()
    {
        for (ELEMENT_TYPE i = ELEMENT_TYPE.Fire; i <= ELEMENT_TYPE.Dark; i++)
        {
            DBElementResourceData resourceData = new DBElementResourceData();
            resourceData.elementType = i;
            resourceData.elementIcon = Resources.Load<Sprite>(string.Format("UI/ElementType/Element_{0}", resourceData.elementType.ToString()));

            DBElementTypeResourceDataDic.Add(resourceData.elementType, resourceData);
        }
        return true;
    }

    public bool SetServantJobResourceData()
    {
        for (SERVANT_JOB i = SERVANT_JOB.Warrior; i <= SERVANT_JOB.Magician; i++)
        {
            DBServantJobResourceData resourceData = new DBServantJobResourceData();
            resourceData.servantJob = i;
            resourceData.jobIcon = Resources.Load<Sprite>(string.Format("UI/ServantJob/Job_{0}", resourceData.servantJob.ToString()));

            DBServantJobResourceDataDic.Add(resourceData.servantJob, resourceData);
        }
        return true;
    }

    public bool SetGachaGradeResourceData()
    {
        for (GRADE_TYPE i = GRADE_TYPE.LEGENDARY; i <= GRADE_TYPE.COMMON; i++)
        {
            DBGradeResourceData resourceData = new DBGradeResourceData();
            resourceData.gradeType = i;
            resourceData.grade = Convert.ToInt32(i);
            resourceData.gradeIcon = Resources.Load<Sprite>(string.Format("UI/Common/gacha_grade_{0}", resourceData.grade));

            DBGachaGradeResourceDataDic.Add(resourceData.gradeType, resourceData);
        }
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

    public bool SetSkillActiveData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_skill_active");
        for (var i = 2; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["name"]
            //    + " " + data[i]["explain"]
            //    + " " + data[i]["resource_icon"]
            //    + " " + data[i]["job"]
            //    + " " + data[i]["job_limit"]
            //    + " " + data[i]["class"]
            //    + " " + data[i]["class_limit"]
            //    + " " + data[i]["active_per"]
            //    + " " + data[i]["skill_type"]
            //    + " " + data[i]["attack_type"]
            //    + " " + data[i]["dmg_type"]
            //    + " " + data[i]["target_type"]
            //    + " " + data[i]["target_count"]
            //    + " " + data[i]["hit_count"]
            //    + " " + data[i]["atk_per"]
            //    + " " + data[i]["atk_add"]
            //    + " " + data[i]["heal_per"]
            //    + " " + data[i]["heal_add"]
            //    + " " + data[i]["option_id_list"]
            //    + " " + data[i]["buff_id_list"]
            //    );

            DBSkillActiveData activeData = new DBSkillActiveData();
            activeData.id = Convert.ToInt32(data[i]["id"]);
            if(DBSkillActiveDataDic.ContainsKey(activeData.id) == true)
            {
                Debug.Log("Invalid Active Skill ID : " + activeData.id);
                return false;
            }

            activeData.name = Convert.ToString(data[i]["name"]);
            activeData.explain = Convert.ToString(data[i]["explain"]);
            activeData.resourceIcon = Convert.ToString(data[i]["resource_icon"]);

            //Debug.Log("ActiveSkill Job Able : " + Convert.ToString(data[i]["job"]));
            activeData.jobLimit = (SERVANT_JOB_FLAG)Convert.ToInt32(Convert.ToString(data[i]["job_limit"]), 2);
            //Debug.Log("ActiveSkill Test Warrior : " + activeData.isJobAble(SERVANT_JOB_FLAG.Warrior));
            //Debug.Log("ActiveSkill Test Thief : " + activeData.isJobAble(SERVANT_JOB_FLAG.Thief));
            //Debug.Log("ActiveSkill Test Cleric : " + activeData.isJobAble(SERVANT_JOB_FLAG.Cleric));
            //Debug.Log("ActiveSkill Test Archer : " + activeData.isJobAble(SERVANT_JOB_FLAG.Archer));
            //Debug.Log("ActiveSkill Test Magician : " + activeData.isJobAble(SERVANT_JOB_FLAG.Magician));
            //Debug.Log("ActiveSkill Test All : " + activeData.isJobAble(SERVANT_JOB_FLAG.All));

            //Debug.Log("ActiveSkill Class Able : " + Convert.ToString(data[i]["class"]));
            activeData.classLimit = (MONSTER_CLASS_FLAG)Convert.ToInt32(Convert.ToString(data[i]["class_limit"]), 2);
            //Debug.Log("ActiveSkill Test Fighter : " + activeData.isClassAble(MONSTER_CLASS_FLAG.Fighter));
            //Debug.Log("ActiveSkill Test Kngiht : " + activeData.isClassAble(MONSTER_CLASS_FLAG.Kngiht));
            //Debug.Log("ActiveSkill Test Priest : " + activeData.isClassAble(MONSTER_CLASS_FLAG.Priest));
            //Debug.Log("ActiveSkill Test Assassin : " + activeData.isClassAble(MONSTER_CLASS_FLAG.Assassin));
            //Debug.Log("ActiveSkill Test Hunter : " + activeData.isClassAble(MONSTER_CLASS_FLAG.Hunter));
            //Debug.Log("ActiveSkill Test Mage : " + activeData.isClassAble(MONSTER_CLASS_FLAG.Mage));
            //Debug.Log("ActiveSkill Test Warlock : " + activeData.isClassAble(MONSTER_CLASS_FLAG.Warlock));
            //Debug.Log("ActiveSkill Test Druid : " + activeData.isClassAble(MONSTER_CLASS_FLAG.Druid));
            //Debug.Log("ActiveSkill Test Shaman : " + activeData.isClassAble(MONSTER_CLASS_FLAG.Shaman));
            //Debug.Log("ActiveSkill Test All : " + activeData.isClassAble(MONSTER_CLASS_FLAG.All));

            activeData.activePer = Convert.ToDouble(data[i]["active_per"]);

            switch(Convert.ToString(data[i]["skill_type"]))
            {
                case "attack":
                    activeData.skillType = SKILL_TYPE.ATTACK;
                    break;
                case "buff":
                    activeData.skillType = SKILL_TYPE.BUFF;
                    break;
                case "heal":
                    activeData.skillType = SKILL_TYPE.HEAL;
                    break;
                case "debuff":
                    activeData.skillType = SKILL_TYPE.DEBUFF;
                    break;

                default:
                    Debug.Log("Invalid Skill Type" + Convert.ToString(data[i]["skill_type"]));
                    return false;
            }

            activeData.attackType = (ATTACK_TYPE)Convert.ToInt32(data[i]["attack_type"]);
            activeData.damageType = (DAMAGE_TYPE)Convert.ToInt32(data[i]["dmg_type"]);

            switch(Convert.ToString(data[i]["target_type"]))
            {
                case "enemy":
                    activeData.targetType = TARGET_TYPE.ENEMY;
                    break;
                case "enemies":
                    activeData.targetType = TARGET_TYPE.ENEMIES;
                    break;
                case "allenemy":
                    activeData.targetType = TARGET_TYPE.ALLENEMY;
                    break;

                case "self":
                    activeData.targetType = TARGET_TYPE.SELF;
                    break;
                case "myteam":
                    activeData.targetType = TARGET_TYPE.MYTEAM;
                    break;
                case "myteams":
                    activeData.targetType = TARGET_TYPE.MYTEAMS;
                    break;
                case "allmyteam":
                    activeData.targetType = TARGET_TYPE.ALLMYTEAM;
                    break;

                case "every":
                    activeData.targetType = TARGET_TYPE.EVERY;
                    break;

                default:
                    Debug.Log("Invalid Target Type" + Convert.ToString(data[i]["target_type"]));
                    return false;
            }

            activeData.targetCount = Convert.ToInt32(data[i]["target_count"]);
            activeData.hitCount = Convert.ToInt32(data[i]["hit_count"]);
            activeData.atkPer = Convert.ToDouble(data[i]["atk_per"]);
            activeData.atkAdd = Convert.ToInt32(data[i]["atk_add"]);
            activeData.healPer = Convert.ToDouble(data[i]["heal_per"]);
            activeData.healAdd = Convert.ToInt32(data[i]["heal_add"]);
            // Option ID List / Buff ID List는 추후 추가

            DBSkillActiveDataDic.Add(activeData.id, activeData);
        }

        return true;
    }

    public bool SetSkillPassiveData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_skill_passive");
        for (var i = 2; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["name"]
            //    + " " + data[i]["explain"]
            //    + " " + data[i]["resource_icon"]
            //    + " " + data[i]["job"]
            //    + " " + data[i]["job_limit"]
            //    + " " + data[i]["class"]
            //    + " " + data[i]["class_limit"]
            //    + " " + data[i]["enable_stack"]
            //    + " " + data[i]["max_stack"]
            //    + " " + data[i]["effect_keyword"]
            //    + " " + data[i]["effect_type"]
            //    + " " + data[i]["effect_per"]
            //    + " " + data[i]["effect_add"]
            //    + " " + data[i]["target"]
            //    + " " + data[i]["role_target"]
            //    );

            DBSkillPassiveData passiveData = new DBSkillPassiveData();
            passiveData.id = Convert.ToInt32(data[i]["id"]);
            if(DBSkillPassiveDataDic.ContainsKey(passiveData.id) == true)
            {
                Debug.Log("겹치는 ID : " + passiveData.id);
                return false;
            }

            passiveData.name = Convert.ToString(data[i]["name"]);
            passiveData.explain = Convert.ToString(data[i]["explain"]);
            passiveData.resourceIcon = Convert.ToString(data[i]["resource_icon"]);

            //Debug.Log("PassiveSkill Job Able : " + Convert.ToString(data[i]["job"]));
            passiveData.jobLimit = (SERVANT_JOB_FLAG)Convert.ToInt32(Convert.ToString(data[i]["job_limit"]), 2);
            //Debug.Log("PassiveSkill Test Warrior : " + passiveData.isJobAble(SERVANT_JOB_FLAG.Warrior));
            //Debug.Log("PassiveSkill Test Thief : " + passiveData.isJobAble(SERVANT_JOB_FLAG.Thief));
            //Debug.Log("PassiveSkill Test Cleric : " + passiveData.isJobAble(SERVANT_JOB_FLAG.Cleric));
            //Debug.Log("PassiveSkill Test Archer : " + passiveData.isJobAble(SERVANT_JOB_FLAG.Archer));
            //Debug.Log("PassiveSkill Test Magician : " + passiveData.isJobAble(SERVANT_JOB_FLAG.Magician));
            //Debug.Log("PassiveSkill Test All : " + passiveData.isJobAble(SERVANT_JOB_FLAG.All));

            //Debug.Log("PassiveSkill Class Able : " + Convert.ToString(data[i]["class"]));
            passiveData.classLimit = (MONSTER_CLASS_FLAG)Convert.ToInt32(Convert.ToString(data[i]["class_limit"]), 2);
            //Debug.Log("PassiveSkill Test Fighter : " + passiveData.isClassAble(MONSTER_CLASS_FLAG.Fighter));
            //Debug.Log("PassiveSkill Test Kngiht : " + passiveData.isClassAble(MONSTER_CLASS_FLAG.Kngiht));
            //Debug.Log("PassiveSkill Test Priest : " + passiveData.isClassAble(MONSTER_CLASS_FLAG.Priest));
            //Debug.Log("PassiveSkill Test Assassin : " + passiveData.isClassAble(MONSTER_CLASS_FLAG.Assassin));
            //Debug.Log("PassiveSkill Test Hunter : " + passiveData.isClassAble(MONSTER_CLASS_FLAG.Hunter));
            //Debug.Log("PassiveSkill Test Mage : " + passiveData.isClassAble(MONSTER_CLASS_FLAG.Mage));
            //Debug.Log("PassiveSkill Test Warlock : " + passiveData.isClassAble(MONSTER_CLASS_FLAG.Warlock));
            //Debug.Log("PassiveSkill Test Druid : " + passiveData.isClassAble(MONSTER_CLASS_FLAG.Druid));
            //Debug.Log("PassiveSkill Test Shaman : " + passiveData.isClassAble(MONSTER_CLASS_FLAG.Shaman));
            //Debug.Log("PassiveSkill Test All : " + passiveData.isClassAble(MONSTER_CLASS_FLAG.All));

            passiveData.stackAble = Convert.ToBoolean(data[i]["enable_stack"]);
            passiveData.maxStack = Convert.ToInt32(data[i]["max_stack"]);
            passiveData.effectKeyword = Convert.ToString(data[i]["effect_keyword"]);
            passiveData.effectType = (EFFECT_TYPE)Convert.ToInt32(data[i]["effect_type"]);
            passiveData.effectPer = Convert.ToDouble(data[i]["effect_per"]);
            passiveData.effectAdd = Convert.ToInt32(data[i]["effect_add"]);

            switch(Convert.ToString(data[i]["target"]))
            {
                case "none":
                    passiveData.targetType = TARGET_TYPE.NONE;
                    break;
                case "enemy":
                    passiveData.targetType = TARGET_TYPE.ENEMY;
                    break;
                case "enemies":
                    passiveData.targetType = TARGET_TYPE.ENEMIES;
                    break;
                case "allenemy":
                    passiveData.targetType = TARGET_TYPE.ALLENEMY;
                    break;
                case "self":
                    passiveData.targetType = TARGET_TYPE.SELF;
                    break;
                case "myteam":
                    passiveData.targetType = TARGET_TYPE.MYTEAM;
                    break;
                case "myteams":
                    passiveData.targetType = TARGET_TYPE.MYTEAMS;
                    break;
                case "allmyteam":
                    passiveData.targetType = TARGET_TYPE.ALLMYTEAM;
                    break;
                case "every":
                    passiveData.targetType = TARGET_TYPE.EVERY;
                    break;
                default:
                    Debug.Log("Invalid Target Type");
                    return false;
            }
            //Debug.Log("TargetType : " + passiveData.targetType.ToString());

            switch (Convert.ToString(data[i]["role_target"]))
            {
                case "none":
                    passiveData.roleTargetType = TARGET_TYPE.NONE;
                    break;
                case "enemy":
                    passiveData.roleTargetType = TARGET_TYPE.ENEMY;
                    break;
                case "enemies":
                    passiveData.roleTargetType = TARGET_TYPE.ENEMIES;
                    break;
                case "allenemy":
                    passiveData.roleTargetType = TARGET_TYPE.ALLENEMY;
                    break;
                case "self":
                    passiveData.roleTargetType = TARGET_TYPE.SELF;
                    break;
                case "myteam":
                    passiveData.roleTargetType = TARGET_TYPE.MYTEAM;
                    break;
                case "myteams":
                    passiveData.roleTargetType = TARGET_TYPE.MYTEAMS;
                    break;
                case "allmyteam":
                    passiveData.roleTargetType = TARGET_TYPE.ALLMYTEAM;
                    break;
                case "every":
                    passiveData.roleTargetType = TARGET_TYPE.EVERY;
                    break;
                default:
                    Debug.Log("Invalid Role Target Type");
                    return false;
            }
            //Debug.Log("RoleTargetType : " + passiveData.roleTargetType.ToString());

            DBSkillPassiveDataDic.Add(passiveData.id, passiveData);
        }

        return true;
    }

    public bool SetExpData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/DB_exp");
        for (var i = 2; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["level"]
            //    + " " + data[i]["rank_exp"]
            //    + " " + data[i]["char_exp"]
            //    );

            DBExpData expData = new DBExpData();
            expData.level = Convert.ToInt32(data[i]["level"]);
            if(DBExpDataDic.ContainsKey(expData.level) == true)
            {
                Debug.Log("겹치는 ID : " + expData.level);
                return false;
            }

            expData.rankExp = Convert.ToInt32(data[i]["rank_exp"]);
            expData.charExp = Convert.ToInt32(data[i]["char_exp"]);

            DBExpDataDic.Add(expData.level, expData);
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

    public int GetRandomEquipmentID()
    {
        int equipmentID = DBEquipmentDataDic.Values.ToList()[UnityEngine.Random.Range(0, DBEquipmentDataDic.Count)].id;

        return equipmentID;
    }

    public Sprite GetServantIcon(int id)
    {
        if (DBServantDataDic.ContainsKey(id) == false)
        {
            return null;
        }

        return DBServantDataDic[id].servantIcon;
    }

    public Sprite GetMonsterIcon(int id)
    {
        if (DBMonsterDataDic.ContainsKey(id) == false)
        {
            return null;
        }

        return DBMonsterDataDic[id].monsterIcon;
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
            Debug.LogError("DBStageDataDic Error ID : " + id);
            return null;
        }

        return DBStageDataDic[id];
    }

    public DBStageEnemyData GetStageEnemyData(int id)
    {
        if (DBStageEnemyDataDic.ContainsKey(id) == false)
        {
            Debug.LogError("Invalid Request DBStageEnemyDataDic Not Contain Key : " + id);
            return null;
        }

        return DBStageEnemyDataDic[id];
    }

    public DBStageRewardData GetStageRewardData(int stageType, int stageFloor)
    {
        int id = 100000 + (stageType * 100) + stageFloor;

        if(DBStageRewardDataDic.ContainsKey(id) == false)
        {
            Debug.Log("DBStageREwardDataDic Error ID : " + id);
            return null;
        }

        return DBStageRewardDataDic[id];
    }

    public DBExpData GetExpData(int level)
    {
        if(DBExpDataDic.ContainsKey(level) == false)
        {
            Debug.Log("Invalid Exp Data Level : " + level);
            return null;
        }

        return DBExpDataDic[level];
    }

    public int GetRankLevelByExp(int exp)
    {
        foreach(KeyValuePair<int, DBExpData> data in DBExpDataDic)
        {
            if(exp < data.Value.rankExp)
            {
                return data.Key;
            }
        }

        Debug.Log("Invalid Exp : " + exp);

        return 0;
    }

    public int GetCharLevelByExp(int exp)
    {
        foreach (KeyValuePair<int, DBExpData> data in DBExpDataDic)
        {
            if (exp < data.Value.charExp)
            {
                return data.Key;
            }
        }

        Debug.Log("Invalid Exp : " + exp);

        return 0;
    }

    public Sprite GetSpriteGachaGrade(GRADE_TYPE grade)
    {
        return DBGachaGradeResourceDataDic[grade].gradeIcon;
    }

    public Sprite GetSpriteGrade(GRADE_TYPE grade)
    {
        return DBGradeResourceDataDic[grade].gradeIcon;
    }

    public Sprite GetSpriteOptionType(EQUIPMENT_OPTION_TYPE type)
    {
        return DBOptionTypeResourceDataDic[type].optionIcon;
    }

    public Sprite GetSpriteElementType(ELEMENT_TYPE type)
    {
        return DBElementTypeResourceDataDic[type].elementIcon;
    }

    public Sprite GetSpriteServantJob(SERVANT_JOB type)
    {
        return DBServantJobResourceDataDic[type].jobIcon;
    }

    public DBSkillActiveData GetSkillActiveData(int id)
    {
        if(DBSkillActiveDataDic.ContainsKey(id) == false)
        {
            Debug.LogError("Invalid Request DBSkillActiveDataDic Not Contain Key : " + id);
            return null;
        }

        return DBSkillActiveDataDic[id];
    }

    public DBSkillPassiveData GetSkillPassiveData(int id)
    {
        if (DBSkillPassiveDataDic.ContainsKey(id) == false)
        {
            Debug.LogError("Invalid Request DBSkillPassiveDataDic Not Contain Key : " + id);
            return null;
        }

        return DBSkillPassiveDataDic[id];
    }

    public Status GetServantLevelPerAddStatus(SERVANT_JOB jobType, GRADE_TYPE gradeType)
    {
        Status addStatus = new Status();

        switch(jobType)
        {
            case SERVANT_JOB.Warrior:
                addStatus.basicStr = 6;
                addStatus.basicDex = 3;
                addStatus.basicInt = 1;
                break;

            case SERVANT_JOB.Thief:
                addStatus.basicStr = 1;
                addStatus.basicDex = 8;
                addStatus.basicInt = 1;
                break;

            case SERVANT_JOB.Cleric:
                addStatus.basicStr = 1;
                addStatus.basicDex = 2;
                addStatus.basicInt = 7;
                break;

            case SERVANT_JOB.Archer:
                addStatus.basicStr = 2;
                addStatus.basicDex = 7;
                addStatus.basicInt = 1;
                break;

            case SERVANT_JOB.Magician:
                addStatus.basicStr = 1;
                addStatus.basicDex = 1;
                addStatus.basicInt = 8;
                break;

            default:
                Debug.Log("Job Error : " + jobType.ToString());
                break;
        }

        return addStatus;
    }
    
    public DBSkillPassiveData GetDBSkillPassiveData(int id)
    {
        if(DBSkillPassiveDataDic.ContainsKey(id) == false)
        {
            return null;
        }

        return DBSkillPassiveDataDic[id];
    }

    // DB 생길때까지 임시
    public Status GetMonsterLevelPerAddStatus(MONSTER_CLASS classType, GRADE_TYPE gradeType)
    {
        Status addStatus = new Status();

        switch(classType)
        {
            case MONSTER_CLASS.Fighter:
                if (gradeType == GRADE_TYPE.COMMON)
                {
                    addStatus.basicStr = 7;
                    addStatus.basicDex = 2;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.UNCOMMON)
                {
                    addStatus.basicStr = 7;
                    addStatus.basicDex = 3;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.RARE)
                {
                    addStatus.basicStr = 8;
                    addStatus.basicDex = 3;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.UNIQUE)
                {
                    addStatus.basicStr = 9;
                    addStatus.basicDex = 3;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.LEGENDARY)
                {
                    addStatus.basicStr = 10;
                    addStatus.basicDex = 4;
                    addStatus.basicInt = 1;
                }
                else
                {
                    Debug.Log("Grade Error : " + gradeType.ToString());
                }
                break;
            case MONSTER_CLASS.Kngiht:
                if (gradeType == GRADE_TYPE.COMMON)
                {
                    addStatus.basicStr = 8;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.UNCOMMON)
                {
                    addStatus.basicStr = 9;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.RARE)
                {
                    addStatus.basicStr = 10;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.UNIQUE)
                {
                    addStatus.basicStr = 10;
                    addStatus.basicDex = 2;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.LEGENDARY)
                {
                    addStatus.basicStr = 12;
                    addStatus.basicDex = 2;
                    addStatus.basicInt = 1;
                }
                else
                {
                    Debug.Log("Grade Error : " + gradeType.ToString());
                }
                break;
            case MONSTER_CLASS.Priest:
                if (gradeType == GRADE_TYPE.COMMON)
                {
                    addStatus.basicStr = 2;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 7;
                }
                else if (gradeType == GRADE_TYPE.UNCOMMON)
                {
                    addStatus.basicStr = 2;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 8;
                }
                else if (gradeType == GRADE_TYPE.RARE)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 8;
                }
                else if (gradeType == GRADE_TYPE.UNIQUE)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 9;
                }
                else if (gradeType == GRADE_TYPE.LEGENDARY)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 11;
                }
                else
                {
                    Debug.Log("Grade Error : " + gradeType.ToString());
                }
                break;
            case MONSTER_CLASS.Assassin:
                if (gradeType == GRADE_TYPE.COMMON)
                {
                    addStatus.basicStr = 1;
                    addStatus.basicDex = 8;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.UNCOMMON)
                {
                    addStatus.basicStr = 1;
                    addStatus.basicDex = 9;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.RARE)
                {
                    addStatus.basicStr = 1;
                    addStatus.basicDex = 10;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.UNIQUE)
                {
                    addStatus.basicStr = 2;
                    addStatus.basicDex = 10;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.LEGENDARY)
                {
                    addStatus.basicStr = 2;
                    addStatus.basicDex = 12;
                    addStatus.basicInt = 1;
                }
                else
                {
                    Debug.Log("Grade Error : " + gradeType.ToString());
                }
                break;
            case MONSTER_CLASS.Hunter:
                if (gradeType == GRADE_TYPE.COMMON)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 6;
                    addStatus.basicInt = 1;
                }
                else if (gradeType == GRADE_TYPE.UNCOMMON)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 6;
                    addStatus.basicInt = 2;
                }
                else if (gradeType == GRADE_TYPE.RARE)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 7;
                    addStatus.basicInt = 2;
                }
                else if (gradeType == GRADE_TYPE.UNIQUE)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 8;
                    addStatus.basicInt = 2;
                }
                else if (gradeType == GRADE_TYPE.LEGENDARY)
                {
                    addStatus.basicStr = 4;
                    addStatus.basicDex = 9;
                    addStatus.basicInt = 2;
                }
                else
                {
                    Debug.Log("Grade Error : " + gradeType.ToString());
                }
                break;
            case MONSTER_CLASS.Mage:
                if (gradeType == GRADE_TYPE.COMMON)
                {
                    addStatus.basicStr = 1;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 8;
                }
                else if (gradeType == GRADE_TYPE.UNCOMMON)
                {
                    addStatus.basicStr = 1;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 9;
                }
                else if (gradeType == GRADE_TYPE.RARE)
                {
                    addStatus.basicStr = 1;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 10;
                }
                else if (gradeType == GRADE_TYPE.UNIQUE)
                {
                    addStatus.basicStr = 1;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 11;
                }
                else if (gradeType == GRADE_TYPE.LEGENDARY)
                {
                    addStatus.basicStr = 1;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 13;
                }
                else
                {
                    Debug.Log("Grade Error : " + gradeType.ToString());
                }
                break;
            case MONSTER_CLASS.Warlock:
                if (gradeType == GRADE_TYPE.COMMON)
                {
                    addStatus.basicStr = 2;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 7;
                }
                else if (gradeType == GRADE_TYPE.UNCOMMON)
                {
                    addStatus.basicStr = 2;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 8;
                }
                else if (gradeType == GRADE_TYPE.RARE)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 8;
                }
                else if (gradeType == GRADE_TYPE.UNIQUE)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 9;
                }
                else if (gradeType == GRADE_TYPE.LEGENDARY)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 1;
                    addStatus.basicInt = 11;
                }
                else
                {
                    Debug.Log("Grade Error : " + gradeType.ToString());
                }
                break;
            case MONSTER_CLASS.Druid:
                if (gradeType == GRADE_TYPE.COMMON)
                {
                    addStatus.basicStr = 5;
                    addStatus.basicDex = 2;
                    addStatus.basicInt = 3;
                }
                else if (gradeType == GRADE_TYPE.UNCOMMON)
                {
                    addStatus.basicStr = 5;
                    addStatus.basicDex = 3;
                    addStatus.basicInt = 3;
                }
                else if (gradeType == GRADE_TYPE.RARE)
                {
                    addStatus.basicStr = 6;
                    addStatus.basicDex = 3;
                    addStatus.basicInt = 3;
                }
                else if (gradeType == GRADE_TYPE.UNIQUE)
                {
                    addStatus.basicStr = 7;
                    addStatus.basicDex = 3;
                    addStatus.basicInt = 3;
                }
                else if (gradeType == GRADE_TYPE.LEGENDARY)
                {
                    addStatus.basicStr = 8;
                    addStatus.basicDex = 3;
                    addStatus.basicInt = 4;
                }
                else
                {
                    Debug.Log("Grade Error : " + gradeType.ToString());
                }
                break;
            case MONSTER_CLASS.Shaman:
                if (gradeType == GRADE_TYPE.COMMON)
                {
                    addStatus.basicStr = 2;
                    addStatus.basicDex = 5;
                    addStatus.basicInt = 3;
                }
                else if (gradeType == GRADE_TYPE.UNCOMMON)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 5;
                    addStatus.basicInt = 3;
                }
                else if (gradeType == GRADE_TYPE.RARE)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 6;
                    addStatus.basicInt = 3;
                }
                else if (gradeType == GRADE_TYPE.UNIQUE)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 7;
                    addStatus.basicInt = 3;
                }
                else if (gradeType == GRADE_TYPE.LEGENDARY)
                {
                    addStatus.basicStr = 3;
                    addStatus.basicDex = 8;
                    addStatus.basicInt = 4;
                }
                else
                {
                    Debug.Log("Grade Error : " + gradeType.ToString());
                }
                break;

            default:
                Debug.Log("Class Error : " + classType.ToString());
                break;
        }

        return addStatus;
    }

    #endregion
}
