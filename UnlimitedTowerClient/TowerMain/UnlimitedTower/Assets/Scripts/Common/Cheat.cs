using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using System.Linq;

public class Cheat : MonoSingleton<Cheat>
{
    private System.Random rand = new System.Random();

    public string GetUserLoginData(string user, string eos, string utg)
    {
        UserLoginData userLoginData = new UserLoginData();

        userLoginData.utg = utg;
        userLoginData.eos = eos;

        userLoginData.user_data.user = user;
        userLoginData.user_data.state = 2;
        userLoginData.user_data.exp = rand.Next(0, 20560);
        userLoginData.user_data.rank = CSVData.Inst.GetRankLevelByExp(userLoginData.user_data.exp);

        userLoginData.user_data.inventory_info.servant_inventory = 50;
        userLoginData.user_data.inventory_info.monster_inventory = 50;
        userLoginData.user_data.inventory_info.equipment_inventory = 50;
        userLoginData.user_data.inventory_info.item_inventory = 50;

        partyData partyData = new partyData();
        partyData.index = 1;
        partyData.state = 0;

        for (int i = 1; i < 45; i++)
        {
            userLoginData.servant_list.Add(GetRandomServantData(i));

            if (i < 6)
            {
                partyData.servant_list.Add(i);
            }
        }

        for (int i = 1; i < 45; i++)
        {
            userLoginData.monster_list.Add(GetRandomMonster(i));

            if (i < 6)
            {
                partyData.monster_list.Add(i);
            }
        }

        // Item 추가 예정
        for(int i = 1; i < 100; i++)
        {
            userLoginData.equipment_list.Add(GetRandomEquipment(i));
        }

        userLoginData.party_info = partyData;

        return JsonMapper.ToJson(userLoginData).ToString();
    }

    public string GetGachaResultData(int gachaIndex)
    {
        UserInfo userInfo = UserDataManager.Inst.GetUserInfo();
        if (userInfo == null)
        {
            return null;
        }

        if(userInfo.userEOS < 10000)
        {
            return null;
        }

        int type = rand.Next((int)GACHA_RESULT_TYPE.Servant, (int)GACHA_RESULT_TYPE.Max);

        if (type == (int)GACHA_RESULT_TYPE.Servant)
        {
            gachaServantData gachaResult = new gachaServantData();
            gachaResult.result_type = type;
            gachaResult.data = GetRandomServantData(UserDataManager.Inst.servantDic.Count + 1);

            return JsonMapper.ToJson(gachaResult).ToString();
        }
        else if (type == (int)GACHA_RESULT_TYPE.Monster)
        {
            gachaMonsterData gachaResult = new gachaMonsterData();
            gachaResult.result_type = type;
            gachaResult.data = GetRandomMonster(UserDataManager.Inst.monsterDic.Count + 1);

            return JsonMapper.ToJson(gachaResult).ToString();
        }
        else if (type == (int)GACHA_RESULT_TYPE.Equipment)
        {
            // 아이템은 아직
            gachaEquipmentData gachaResult = new gachaEquipmentData();
            gachaResult.result_type = type;
            gachaResult.data = GetRandomEquipment(UserDataManager.Inst.equipmentDic.Count + 1);

            return JsonMapper.ToJson(gachaResult).ToString();
        }
        else
        {
            return null;
        }
    }

    public string GetBattleActionData(string user, int getTurn)
    {
        UserStageStateData stateData = UserDataManager.Inst.GetStageState();
        stateData.turn += 1;

        battleActionData battleactiondata = new battleActionData();
        battleactiondata.user = user;
        battleactiondata.turn = getTurn;

        int[] tempHp = new int[20];

        for (int i = 0; i < 20; i++)
        {
            tempHp[i] = BattleManager.Inst.NowHp[i].GetHashCode();
        }

        for (int i = 0; i < 10; ++i)
        {
            if (stateData.myStateList.ContainsKey(i) == false)
            {
                continue;
            }

            if (tempHp[i] <= 0)
            {
                continue;
            }
            
            int tempHpSum = 0;

            for (int j = 10; j < 20; j++)
            {
                tempHpSum += tempHp[j] < 0 ? 0 : tempHp[j];
            }

            if (tempHpSum <= 0)
            {
                continue;
            }

            if (stateData.myStateList[i].charType == CHAR_TYPE.SERVANT)
            {
                characterActionData actioninfo = new characterActionData();
                actioninfo.my_position = i;
                // TODO : Skill 관련 코드 정리 필요
                //if (stateData.myStateList[i].activeSkillList.Count <= 0)
                {
                    // TODO : 세팅값만 있어야하는지
                    actioninfo.action_type = 2;
                    Debug.Log("Invalid ActiveSkill List");
                    actionInfo action = new actionInfo();
                    do
                    {
                        action.target_position = UnityEngine.Random.Range(10, 20);
                    } while (tempHp[action.target_position] <= 0);
                    action.avoid = false;
                    action.critical = UnityEngine.Random.Range(0, 10) == 1 ? true : false;

                    float atkDmg = (float)rand.Next(stateData.myStateList[i].atk, stateData.myStateList[i].atk + 200);
                    float enemyDef = 2000 / (2000 + (float)stateData.enemyStateList[action.target_position].def);
                    action.damage = (int)(atkDmg * enemyDef);
                    tempHp[action.target_position] -= action.damage;
                    actioninfo.action_info_list.Add(action);
                }
                // else
                // {
                //     if (stateData.myStateList[i].activeSkillList[0].id == 200001)
                //     {
                //         actioninfo.action_type = 3;
                //         for (int target = 0; target < 1; ++target)
                //         {
                //             actionInfo action = new actionInfo();
                //             do
                //             {
                //                 action.target_position = UnityEngine.Random.Range(10, 20);
                //             } while (tempHp[action.target_position] <= 0);
                //             action.avoid = false;
                //             action.critical = false;
                // 
                //             float atkDmg = (float)rand.Next(stateData.myStateList[i].mAtk, stateData.myStateList[i].mAtk + 200);
                //             float enemyDef = 2000 / (2000 + (float)stateData.enemyStateList[action.target_position].mDef);
                //             action.damage = (int)(atkDmg * enemyDef);
                //             tempHp[action.target_position] -= action.damage;
                //             actioninfo.action_info_list.Add(action);
                //         }
                //     }
                //     else if (stateData.myStateList[i].activeSkillList[0].id == 200002)
                //     {
                //         actioninfo.action_type = 3;
                //         actionInfo action = new actionInfo();
                //         do
                //         {
                //             action.target_position = UnityEngine.Random.Range(10, 20);
                //         } while (tempHp[action.target_position] <= 0);
                //         action.avoid = false;
                //         action.critical = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
                // 
                //         float atkDmg = (float)rand.Next(stateData.myStateList[i].atk, stateData.myStateList[i].atk + 200);
                //         float enemyDef = 2000 / (2000 + (float)stateData.enemyStateList[action.target_position].def);
                //         action.damage = (int)(atkDmg * enemyDef);
                //         tempHp[action.target_position] -= action.damage;
                //         actioninfo.action_info_list.Add(action);
                //     }
                // }

                battleactiondata.character_action_list.Add(actioninfo);
            }
            else
            {
                if (BattleManager.Inst.NowHp[i] == 0)
                {
                    continue;
                }
                actionInfo action = new actionInfo();
                do
                {
                    action.target_position = UnityEngine.Random.Range(10, 20);
                } while (tempHp[action.target_position] <= 0);
                action.avoid = false;
                action.critical = UnityEngine.Random.Range(0, 2) == 1 ? true : false;

                float atkDmg = (float)rand.Next(stateData.myStateList[i].atk, stateData.myStateList[i].atk + 200);
                float enemyDef = 2000 / (2000 + (float)stateData.enemyStateList[action.target_position].def);
                action.damage = (int)(atkDmg * enemyDef);
                tempHp[action.target_position] -= action.damage;
                characterActionData actioninfo = new characterActionData();
                actioninfo.my_position = stateData.myStateList[i].position;
                actioninfo.action_type = 2;
                actioninfo.action_info_list.Add(action);

                battleactiondata.character_action_list.Add(actioninfo);
            }
        }

        for (int i = 10; i < 20; ++i)
        {
            if (stateData.enemyStateList.ContainsKey(i) == false)
            {
                continue;
            }

            if (tempHp[i] <= 0)
            {
                continue;
            }

            int tempHpSum = 0;

            for (int j = 0; j < 10; j++)
            {
                tempHpSum += tempHp[j] < 0 ? 0 : tempHp[j];
            }

            if (tempHpSum <= 0)
            {
                tempHpSum = 0;
                continue;
            }

            actionInfo action = new actionInfo();
            do
            {
                action.target_position = UnityEngine.Random.Range(0, 10);
            } while (tempHp[action.target_position] <= 0);
            action.avoid = false;
            action.critical = UnityEngine.Random.Range(0, 2) == 1 ? true : false;

            float atkDmg = (float)rand.Next(stateData.enemyStateList[i].atk, stateData.enemyStateList[i].atk + 200);
            float enemyDef = 2000 / (2000 + (float)stateData.myStateList[action.target_position].def);
            action.damage = (int)(atkDmg * enemyDef);
            tempHp[action.target_position] -= action.damage;
            characterActionData actioninfo = new characterActionData();
            actioninfo.my_position = i;
            actioninfo.action_type = 2;
            actioninfo.action_info_list.Add(action);

            battleactiondata.character_action_list.Add(actioninfo);
        }


        return JsonMapper.ToJson(battleactiondata);
    }

    public string GetStageStartData(string user, int stageType, int stageFloor, int partyNum)
    {
        stageStateData battlestatedata = new stageStateData();
        battlestatedata.user = user;
        battlestatedata.stage_type = stageType;
        battlestatedata.enemy_user = user;
        battlestatedata.stage_number = stageFloor;
        battlestatedata.turn = 0;

        UserPartyData partyData = UserDataManager.Inst.GetUserPartyInfo();
        if (partyData == null)
        {
            return null;
        }
        if (partyData.formationDataDic.Count != 10)
        {
            return null;
        }

        for (int i = 0; i < 10; ++i)
        {
            if(partyData.formationDataDic[i].index == 0)
            {
                break;
            }

            characterStateData newMember = new characterStateData();
            if (i < 5)
            {
                UserServantData servantData = UserDataManager.Inst.GetServantInfo(partyData.formationDataDic[i].index);
                if(servantData == null)
                {
                    Debug.Log("Invalid Servant Data : " + partyData.formationDataDic[i].index);
                    return null;
                }
                newMember.grade = 5;
                newMember.position = i;
                newMember.index = servantData.index;
                newMember.id = servantData.id;
                newMember.type = 1;
                newMember.upgrade = 0;

                int skill = 0;
                if (i < 2)
                    skill = 200001;
                else
                    skill = 200002;

                newMember.passive_skill_list = new List<int>();
                newMember.passive_skill_list.Add(100005);

                newMember.active_skill_list = new List<int>();
                newMember.active_skill_list.Add(skill);

                newMember.status.basic_str = servantData.status.basicStr;
                newMember.status.basic_dex = servantData.status.basicDex;
                newMember.status.basic_int = servantData.status.basicInt;
                newMember.now_hp = Calculator.GetMaxHp(servantData.status);
            }
            else
            {
                UserMonsterData monsterData = UserDataManager.Inst.GetMonsterInfo(partyData.formationDataDic[i].index);
                if (monsterData == null)
                {
                    return null;
                }
                newMember.grade = UnityEngine.Random.Range(1, 6);
                newMember.position = i;
                newMember.index = monsterData.index;
                newMember.id = monsterData.id;
                newMember.type = 2;
                newMember.upgrade = monsterData.upgrade;

                newMember.status.basic_str = monsterData.status.basicStr;
                newMember.status.basic_dex = monsterData.status.basicDex;
                newMember.status.basic_int = monsterData.status.basicInt;
                newMember.now_hp = Calculator.GetMaxHp(monsterData.status);
            }

            battlestatedata.my_state_list.Add(newMember);
        }

        DBStageData stageData = CSVData.Inst.GetStageData(stageType, stageFloor);
        if(stageData == null)
        {
            return null;
        }

        for (int i = 0; i < stageData.enemyIdList.Count; ++i)
        {
            characterStateData newMember = new characterStateData();
            newMember.grade = 5;
            newMember.position = stageData.enemyPositionList[i];
            newMember.index = 0;
            newMember.id = stageData.enemyIdList[i];
            newMember.type = 2;

            DBStageEnemyData enemyData = CSVData.Inst.GetStageEnemyData(newMember.id);
            if(enemyData == null)
            {
                Debug.Log("Invalid Enemy ID : " + newMember.id);
                return null;
            }

            DBMonsterData monsterData = CSVData.Inst.GetMonsterData(enemyData.charID);
            if(monsterData == null)
            {
                Debug.Log("Invalid Monster ID : " + enemyData.charID);
                return null;
            }

            int level = UnityEngine.Random.Range(stageData.enemyLevelMin, stageData.enemyLevelMax + 1);
            Status addStatus = CSVData.Inst.GetMonsterLevelPerAddStatus(monsterData.GetClass, enemyData.grade);

            newMember.status = new statusInfo();
            newMember.status.basic_str = enemyData.status.basicStr + (level * addStatus.basicStr);
            newMember.status.basic_dex = enemyData.status.basicDex + (level * addStatus.basicDex);
            newMember.status.basic_int = enemyData.status.basicInt + (level * addStatus.basicInt);
            newMember.now_hp = ((newMember.status.basic_str) * 14) + ((newMember.status.basic_dex) * 5) + ((newMember.status.basic_int) * 3);

            battlestatedata.enemy_state_list.Add(newMember);
        }

        return JsonMapper.ToJson(battlestatedata).ToString();
    }

    public string GetMonsterUpgradeData(int mainMonsterIndex, int subMonsterIndex)
    {
        UserMonsterData mainMonster = UserDataManager.Inst.GetMonsterInfo(mainMonsterIndex);
        if (mainMonster == null)
        {
            Debug.Log("Invalid Main Monster Index : " + mainMonsterIndex);
            return null;
        }

        UserMonsterData subMonster = UserDataManager.Inst.GetMonsterInfo(subMonsterIndex);
        if (subMonster == null)
        {
            Debug.Log("Invalid Sub Monster Index : " + subMonsterIndex);
            return null;
        }

        if (mainMonster.id != subMonster.id)
        {
            Debug.Log("Different Monster ID Main : " + mainMonster.id + " , Sub : " + subMonster.id);
            return null;
        }

        if (mainMonster.grade != subMonster.grade)
        {
            Debug.Log("Different Monster Grade Main : " + mainMonster.grade + " , Sub : " + subMonster.grade);
            return null;
        }

        if (mainMonster.upgrade < subMonster.upgrade || mainMonster.upgrade >= DEFINE.MAX_MONSTER_UPGRADE_COUNT)
        {
            Debug.Log("InvalidUpgradeCount Monster UpgradeCount Main : " + mainMonster.grade + " , Sub : " + subMonster.grade);
            return null;
        }

        DBMonsterUpgradeData dbMonsterUpgradeData = CSVData.Inst.GetMonsterUpgradeData(mainMonster.grade, mainMonster.upgrade, subMonster.upgrade);
        if(dbMonsterUpgradeData == null)
        {
            Debug.Log("Invalid Monster Key : " + ((mainMonster.upgrade * 100) + subMonster.upgrade));
            return null;
        }
        
        int successNum = Convert.ToInt32(dbMonsterUpgradeData.successPer * (double)100000);
        Debug.Log("Success Per : " + dbMonsterUpgradeData.successPer + ", Int : " + successNum);
        int randNum = UnityEngine.Random.Range(0, (100000 * 100));

        monsterUpgradeResultData getMonsterUpgradeResultData = new monsterUpgradeResultData();
        getMonsterUpgradeResultData.main_monster_data = GetMonsterData(mainMonster);

        if (randNum <= successNum)
        {
            Debug.Log("Success");
            getMonsterUpgradeResultData.is_success = true;
            getMonsterUpgradeResultData.main_monster_data.monster.upgrade += 1;
        }
        else
        {
            Debug.Log("Fail");
            getMonsterUpgradeResultData.is_success = false;

            if (getMonsterUpgradeResultData.main_monster_data.monster.upgrade > 0)
            {
                getMonsterUpgradeResultData.main_monster_data.monster.upgrade -= 1;
            }
        }

        getMonsterUpgradeResultData.sub_monster_index = subMonster.index;
        getMonsterUpgradeResultData.utg = UserDataManager.Inst.GetUserUTG() - (ulong)dbMonsterUpgradeData.needUTGCount;

        return JsonMapper.ToJson(getMonsterUpgradeResultData).ToString();
    }

    public monsterData GetMonsterData(UserMonsterData getMonsterData)
    {
        // 임시
        monsterData newMonsterData = new monsterData();
        newMonsterData.index = getMonsterData.index;
        newMonsterData.monster = new monsterInfo();
        newMonsterData.monster.state = 0;
        newMonsterData.monster.id = getMonsterData.id;
        DBMonsterData monsterData = CSVData.Inst.GetMonsterData(newMonsterData.monster.id);
        if(monsterData == null)
        {
            Debug.Log("Invalid monster Data");
            return null;
        }

        newMonsterData.monster.type = (int)monsterData.elementType;
        newMonsterData.monster.grade = getMonsterData.grade;
        newMonsterData.monster.level = getMonsterData.level;
        newMonsterData.monster.exp = getMonsterData.exp;
        newMonsterData.monster.upgrade = getMonsterData.upgrade;
        newMonsterData.monster.monster_class = monsterData.classType;
        newMonsterData.monster.status.basic_str = getMonsterData.status.basicStr;
        newMonsterData.monster.status.basic_dex = getMonsterData.status.basicDex;
        newMonsterData.monster.status.basic_int = getMonsterData.status.basicInt;
        // 스킬 추가 필요

        return newMonsterData;
    }

    public string GetServantBurnData(List<int> burnServantIndexList)
    {
        servantBurnResultData resultData = new servantBurnResultData();
        resultData.servantIndexList = burnServantIndexList;
        resultData.itemList = new List<itemData>();

        Dictionary<int, itemData> addItemDic = new Dictionary<int, itemData>();
        for (int i = 0; i < burnServantIndexList.Count; i++)
        {
            UserServantData servantData = UserDataManager.Inst.GetServantInfo(burnServantIndexList[i]);
            if (servantData == null)
            {
                Debug.Log("Invalid Servant Index : " + servantData.index);
                return null;
            }

            if (servantData.partyIndex != 0)
            {
                Debug.Log("Invalid Request Servant Already placed Party : " + servantData.partyIndex);
                return null;
            }
            // TODO : Item 정리중

            DBServantData dbServantData = CSVData.Inst.GetServantData(servantData.id);
            if(dbServantData == null)
            {
                Debug.Log("Invalid Request Servant DB : " + servantData.id);
                return null;
            }

            itemData getItemData = new itemData();
            getItemData.item_list = new List<itemInfo>();
            itemInfo addItem = new itemInfo();

            switch (dbServantData.GetJob)
            {
                case SERVANT_JOB.Warrior:
                    getItemData.id = 510010;
                    addItem.index = 0;
                    addItem.count = 1;
                    getItemData.item_list.Add(addItem);
                    break;
                case SERVANT_JOB.Thief:
                    getItemData.id = 510020;
                    addItem.index = 0;
                    addItem.count = 1;
                    getItemData.item_list.Add(addItem);
                    break;
                case SERVANT_JOB.Cleric:
                    getItemData.id = 510030;
                    addItem.index = 0;
                    addItem.count = 1;
                    getItemData.item_list.Add(addItem);
                    break;
                case SERVANT_JOB.Archer:
                    getItemData.id = 510040;
                    addItem.index = 0;
                    addItem.count = 1;
                    getItemData.item_list.Add(addItem);
                    break;
                case SERVANT_JOB.Magician:
                    getItemData.id = 510050;
                    addItem.index = 0;
                    addItem.count = 1;
                    getItemData.item_list.Add(addItem);
                    break;
                default:
                    break;
            }

            if (addItemDic.ContainsKey(getItemData.id) == false)
            {
                addItemDic.Add(getItemData.id, getItemData);
            }
            else
            {
                addItemDic[getItemData.id].item_list[0].count += 1;
            }
        }

        List<UserItemData> itemInfoList = UserDataManager.Inst.GetItemList();
        if(itemInfoList != null)
        {
            for(int i = 0; i < itemInfoList.Count; i++)
            {
                int count = 0;
                for (int j = 0; j < itemInfoList[i].itemInfoList.Count; j++)
                {
                    count += itemInfoList[i].itemInfoList[j].count;
                }

                if (addItemDic.ContainsKey(itemInfoList[i].id) == true)
                {
                    addItemDic[itemInfoList[i].id].item_list[0].count += count;
                }
            }
        }

        resultData.itemList = addItemDic.Values.ToList();
        resultData.utg = Convert.ToString(UserDataManager.Inst.GetUserUTG());

        return JsonMapper.ToJson(resultData).ToString();
    }

    public string GetMonsterBurnData(List<int> burnMonsterIndexList)
    {
        for(int i = 0; i < burnMonsterIndexList.Count; i++)
        {
            UserMonsterData monsterData = UserDataManager.Inst.GetMonsterInfo(burnMonsterIndexList[i]);
            if(monsterData == null)
            {
                Debug.Log("Invalid Monster Index : " + monsterData.index);
                return null;
            }

            if(monsterData.partyIndex != 0)
            {
                Debug.Log("Invalid Request Monster Already placed Party : " + monsterData.partyIndex);
                return null;
            }
        }

        monsterBurnResultData resultData = new monsterBurnResultData();
        resultData.monsterIndexList = burnMonsterIndexList;
        resultData.itemList = new List<itemData>();
        resultData.utg = Convert.ToString( (ulong)(100 * 10000 * burnMonsterIndexList.Count) + UserDataManager.Inst.GetUserUTG());

        return JsonMapper.ToJson(resultData).ToString();
    }

    public string GetEquipmentBurnData(List<int> burnEquipmentIndexList)
    {
        for (int i = 0; i < burnEquipmentIndexList.Count; i++)
        {
            UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(burnEquipmentIndexList[i]);
            if (equipmentData == null)
            {
                Debug.Log("Invalid Equipment Index : " + equipmentData.index);
                return null;
            }

            if (equipmentData.equipServantIndex != 0)
            {
                Debug.Log("Invalid Request Equipment Already Equiped Servant : " + equipmentData.equipServantIndex);
                return null;
            }
        }

        equipmentBurnResultData resultData = new equipmentBurnResultData();
        resultData.equipmentIndexList = burnEquipmentIndexList;
        resultData.itemList = new List<itemData>();
        resultData.utg = Convert.ToString((ulong)(100 * 10000 * burnEquipmentIndexList.Count) + UserDataManager.Inst.GetUserUTG());

        return JsonMapper.ToJson(resultData).ToString();
    }

    public string GetEquipServantData(int servantIndex, EQUIPMENT_TYPE type, int equipmentIndex)
    {
        servantEquipData resultData = new servantEquipData();
        resultData.servant_index = servantIndex;
        resultData.equipment_slot = (int)type;
        resultData.equipment_index = equipmentIndex;

        return JsonMapper.ToJson(resultData).ToString();
    }

    public string GetUnequipServantData(int servantIndex, EQUIPMENT_TYPE type)
    {
        // 서번트 인덱스 검사
        UserServantData servantData = UserDataManager.Inst.GetServantInfo(servantIndex);
        if (servantData == null)
        {
            Debug.LogError("Invalid Servant Index : " + servantIndex);
            return null;
        }

        // 장비 타입 검사
        if (servantData.equipmentDic.ContainsKey(type) == false)
        {
            Debug.LogError("Invalid Servant Data");
            return null;
        }

        // 서번트 동일 장비 검사
        if (servantData.equipmentDic[type] == 0)
        {
            Debug.Log("Already Unequip");
            return null;
        }

        servantUnequipData resultData = new servantUnequipData();
        resultData.servant_index = servantIndex;
        resultData.equipment_slot = (int)type;

        return JsonMapper.ToJson(resultData).ToString();
    }

    public string GetBattleActionData(int heroTarget, int heroAction, int monsterTarget, int monsterAction)
    {

        return null;
    }

    public string GetStageExit()
    {
        return null;
    }

    public string GetStageResultData()
    {
        stageRewardData rewardData = new stageRewardData();
        rewardData.reward_money = 100000;

        for (int i = 0; i < 10; ++i)
        {
            expInfo rewardCharExp = new expInfo();
            rewardCharExp.pos = i;
            rewardCharExp.exp = 100;
            rewardCharExp.lvup = 1;
            rewardData.get_char_exp_list.Add(rewardCharExp);
        }

        rewardData.get_equipment_list.Add(GetRandomEquipment(UserDataManager.Inst.GetEquipmentCount() + 1));
        rewardData.get_equipment_list.Add(GetRandomEquipment(UserDataManager.Inst.GetEquipmentCount() + 2));
        rewardData.get_equipment_list.Add(GetRandomEquipment(UserDataManager.Inst.GetEquipmentCount() + 3));

        rewardData.get_servant_list.Add(GetRandomServantData(UserDataManager.Inst.servantDic.Count + 2));
        rewardData.get_servant_list.Add(GetRandomServantData(UserDataManager.Inst.servantDic.Count + 3));

        rewardData.get_monster_list.Add(GetRandomMonster(UserDataManager.Inst.monsterDic.Count + 2));
        rewardData.get_monster_list.Add(GetRandomMonster(UserDataManager.Inst.monsterDic.Count + 3));

        return JsonMapper.ToJson(rewardData);
    }

    public string GetTowerStartData()
    {
        return null;
    }

    public string GetTowerResultData()
    {
        return null;
    }

    public servantData GetRandomServantData(int index)
    {
        servantData servant = new servantData();
        servant.index = index;
        servant.servant = GetRandomServant();

        return servant;
    }

    public servantInfo GetRandomServant()
    {
        servantInfo servant = new servantInfo();

        servant.state = 1;
        servant.exp = rand.Next(0, 160000);
        servant.level = CSVData.Inst.GetCharLevelByExp(servant.exp);
        //servant.job = rand.Next(1, 5);
        //servant.appear = GetRandomAppear();
        servant.id = CSVData.Inst.GetRandomServantID();
        servant.grade = 5;
        servant.status = GetRandomStatusInfo(CHAR_TYPE.SERVANT);
        servant.equip_slot.Add(0);
        servant.equip_slot.Add(0);
        servant.equip_slot.Add(0);

        return servant;
    }

    public monsterData GetRandomMonster(int index)
    {
        monsterData monsterData = new monsterData();
        monsterData.index = index;

        monsterData.monster = new monsterInfo();
        monsterData.monster.state = 1;
        monsterData.monster.exp = rand.Next(0, 160000);
        monsterData.monster.level = CSVData.Inst.GetCharLevelByExp(monsterData.monster.exp);

        monsterData.monster.type = 0;

        monsterData.monster.id = CSVData.Inst.GetRandomMonsterID();
        monsterData.monster.grade = rand.Next(1, 6);
        monsterData.monster.upgrade = 0;
        monsterData.monster.status = GetRandomStatusInfo(CHAR_TYPE.MONSTER, monsterData.monster.grade);
        // TODO : 업그레이드에 따른 스테이터스 가중치 추가 필요

        return monsterData;
    }

    public equipmentData GetRandomEquipment(int index)
    {
        equipmentData equipmentData = new equipmentData();
        equipmentData.index = index;

        equipmentData.equipment = new equipmentInfo();
        equipmentData.equipment.state = 1;
        equipmentData.equipment.id = CSVData.Inst.GetRandomEquipmentID();
        equipmentData.equipment.grade = rand.Next(1, 6);
        equipmentData.equipment.upgrade = 0;
        equipmentData.equipment.equipservantindex = 0;

        DBEquipmentData dbEquipmentData = CSVData.Inst.GetEquipmentData(equipmentData.equipment.id);
        if(dbEquipmentData == null)
        {
            Debug.Log("Invalid DBEquipmentData");
            return null;
        }

        equipmentData.equipment.type = (int)dbEquipmentData.equipmentType;
        equipmentData.equipment.job = (int)dbEquipmentData.jobLimit;
        equipmentData.equipment.tier = dbEquipmentData.tier;
        equipmentData.equipment.value = (int)(UnityEngine.Random.Range(dbEquipmentData.optionMin, dbEquipmentData.optionMax) * dbEquipmentData.gradeMultiValueDic[(GRADE_TYPE)equipmentData.equipment.grade]);

        return equipmentData;
    }

    public appearInfo GetRandomAppear()
    {
        appearInfo appear = new appearInfo();
        appear.hair = rand.Next((int)APPEAR_HAIR.BASE, (int)APPEAR_HAIR.MAX);
        appear.head = rand.Next((int)APPEAR_HEAD.BASE, (int)APPEAR_HEAD.MAX);
        appear.body = rand.Next((int)APPEAR_BODY.BASE, (int)APPEAR_BODY.MAX);
        appear.gender = rand.Next((int)GENDER_BODY.BASE, (int)GENDER_BODY.MAX);

        return appear;
    }

    public statusInfo GetRandomStatusInfo(CHAR_TYPE type, int grade = 5)
    {
        statusInfo status = new statusInfo();
        if (type == CHAR_TYPE.SERVANT)
        {
            status.basic_str = rand.Next(DEFINE.SERVANT_MIN_STATUS, DEFINE.SERVANT_MAX_STATUS);
            status.basic_dex = rand.Next(DEFINE.SERVANT_MIN_STATUS, DEFINE.SERVANT_MAX_STATUS);
            status.basic_int = rand.Next(DEFINE.SERVANT_MIN_STATUS, DEFINE.SERVANT_MAX_STATUS);
        }
        else
        {
            status.basic_str = rand.Next(DEFINE.MONSTER_MIN_STATUS, DEFINE.MONSTER_MAX_STATUS) - (grade * DEFINE.MONSTER_GRADE_ADD_STATUS);
            status.basic_dex = rand.Next(DEFINE.MONSTER_MIN_STATUS, DEFINE.MONSTER_MAX_STATUS) - (grade * DEFINE.MONSTER_GRADE_ADD_STATUS);
            status.basic_int = rand.Next(DEFINE.MONSTER_MIN_STATUS, DEFINE.MONSTER_MAX_STATUS) - (grade * DEFINE.MONSTER_GRADE_ADD_STATUS);
        }

        return status;
    }

    #region RequestPacketCheat

    public void RequestLoginCheat()
    {
        if (UserDataManager.Inst.GetUserInfo() == null)
        {
            Debug.Log("Start SetLoginCheat");

            string loginJson = GetUserLoginData("devtooth1111", "100000", "99999990000");
            Debug.Log("[SUCCESS] User Login :" + loginJson);

            PacketManager.Inst.ResponseLogin(loginJson);
        }
        else
        {
            Debug.Log("Already Get UserInfo");
        }
    }

    public void RequestStageRewardCheat()
    {
        string stageRewardJson = GetStageResultData();
        if (stageRewardJson == null)
        {
            Debug.Log("[Fail] RequestStageRewardCheat");
            return;
        }
        Debug.Log("[SUCCESS] User Stage Reward :" + stageRewardJson);
        stageRewardData getStageRewardData = JsonUtility.FromJson<stageRewardData>(stageRewardJson);
        PacketManager.Inst.ResponseStageReward(getStageRewardData);
    }

    public void RequestStageExitCheat()
    {
        PacketManager.Inst.ResponseStageExit();
    }

    public void RequestStageStartCheat(int stageType, int stageFloor, int partyNum)
    {
        if (UserDataManager.Inst.GetUserInfo() == null)
        {
            Debug.Log("Invalid UserInfo, Please First SetLoginCheat");
            return;
        }

        Debug.Log("Start SetStageStartCheat");

        string stageStateJson = GetStageStartData(UserDataManager.Inst.GetUserInfo().userName, stageType, stageFloor, partyNum);
        Debug.Log("[SUCCESS] User Stage Start :" + stageStateJson);

        stageStateData getBattleStageData = JsonUtility.FromJson<stageStateData>(stageStateJson);
        PacketManager.Inst.ResponseStageStart(getBattleStageData);
    }

    public void RequestGachaCheat(int gachaIndex)
    {
        string gachaDataJson = GetGachaResultData(gachaIndex);
        if (gachaDataJson == null)
        {
            Debug.Log("[Fail] RequestGachaCheat");
            return;
        }

        Debug.Log("[SUCCESS] Gacha : " + gachaDataJson);
        PacketManager.Inst.ResponseGacha(gachaDataJson);
    }

    public void RequestMonsterUpgradeCheat(int mainMonsterIndex, int subMonsterIndex)
    {
        string monsterUpgradeResultJson = GetMonsterUpgradeData(mainMonsterIndex, subMonsterIndex);
        if (monsterUpgradeResultJson == null)
        {
            Debug.Log("[Fail] RequestMonsterUpgradeCheat");
            return;
        }
        Debug.Log("[SUCCESS] Monster Upgrade : " + monsterUpgradeResultJson);

        monsterUpgradeResultData getMonsterUpgradeResultData = JsonUtility.FromJson<monsterUpgradeResultData>(monsterUpgradeResultJson);
        PacketManager.Inst.ResponseMonsterUpgrade(getMonsterUpgradeResultData);
    }

    public void RequestServantBurnCheat(List<int> servantIndexList)
    {
        string servantBurnResultJson = GetServantBurnData(servantIndexList);
        if (servantBurnResultJson == null)
        {
            Debug.Log("[Fail] RequestServantBurnCheat");
            return;
        }
        Debug.Log("[SUCCESS] Servant Burn : " + servantBurnResultJson);

        servantBurnResultData getServantBurnResultData = JsonUtility.FromJson<servantBurnResultData>(servantBurnResultJson);
        getServantBurnResultData.servantIndexList = servantIndexList;
        PacketManager.Inst.ResponseServantBurn(getServantBurnResultData);
    }

    public void RequestMonsterBurnCheat(List<int> monsterIndexList)
    {
        string monsterSellResultJson = GetMonsterBurnData(monsterIndexList);
        if (monsterSellResultJson == null)
        {
            Debug.Log("[Fail] RequestMonsterSellCheat");
            return;
        }
        Debug.Log("[SUCCESS] Monster Sell : " + monsterSellResultJson);

        monsterBurnResultData getSellMonsterResultData = JsonUtility.FromJson<monsterBurnResultData>(monsterSellResultJson);
        PacketManager.Inst.ResponseMonsterBurn(getSellMonsterResultData);
    }

    public void RequestEquipmentBurnCheat(List<int> equipmentIndexList)
    {
        string equipmentSellResultJson = GetEquipmentBurnData(equipmentIndexList);
        if (equipmentSellResultJson == null)
        {
            Debug.Log("[Fail] RequestEquipmentSellCheat");
            return;
        }
        Debug.Log("[SUCCESS] Equipment Sell : " + equipmentSellResultJson);

        equipmentBurnResultData getSellEquipmentResultData = JsonUtility.FromJson<equipmentBurnResultData>(equipmentSellResultJson);
        PacketManager.Inst.ResponseEquipmentBurn(getSellEquipmentResultData);
    }

    public void RequestSavePartyCheat(int partyIndex, List<int> indexList)
    {
        partyData getPartyData = new partyData();
        getPartyData.index = partyIndex;
        getPartyData.state = 0;

        for (int i = 0; i < 5; i++)
        {
            getPartyData.servant_list.Add(indexList[i]);
            getPartyData.monster_list.Add(indexList[i + 5]);
        }

        PacketManager.Inst.ResponseSaveParty(getPartyData);
    }

    public void RequestEquipServantCheat(int servantIndex, EQUIPMENT_TYPE type, int equipmentIndex)
    {
        string equipmentJson = GetEquipServantData(servantIndex, type, equipmentIndex);
        if(equipmentJson == null)
        {
            Debug.Log("[Fail] RequestEquipServantCheat");
            return;
        }
        Debug.Log("[SUCCESS] Equip Servant : " + equipmentJson);

        servantEquipData getServantEquipResultData = JsonUtility.FromJson<servantEquipData>(equipmentJson);
        PacketManager.Inst.ResponseEquipServant(getServantEquipResultData);
    }

    public void RequestUnequipServantCheat(int servantIndex, EQUIPMENT_TYPE type)
    {
        string unequipmentJson = GetUnequipServantData(servantIndex, type);
        if (unequipmentJson == null)
        {
            Debug.Log("[Fail] RequestUnequipServantCheat");
            return;
        }
        Debug.Log("[SUCCESS] Unequip Servant : " + unequipmentJson);

        servantUnequipData getServantUnequipResultData = JsonUtility.FromJson<servantUnequipData>(unequipmentJson);
        PacketManager.Inst.ResponseUnequipServant(getServantUnequipResultData);
    }

    public void RequestMailListCheat()
    {
        mailListResultData resultData = new mailListResultData();
        mailData data = new mailData();
        data.mail_index = 1;
        data.mail_type = (int)REWARD_TYPE.MONSTER;
        data.type_id = CSVData.Inst.GetRandomMonsterID();
        data.count = 1;
    

        resultData.mail_data_list.Add(data);

        PacketManager.Inst.ResponseMailList(resultData);
    }

    public void RequestLobbyInfo()
    {
        lobbyInfoResultData resultData = new lobbyInfoResultData();
        resultData.chat_string = "로컬입니다";
        resultData.mail_count = 1;
        resultData.resource_data = new userResourceData();
        resultData.resource_data.cpu_limit = new resourceLimit();
        resultData.resource_data.cpu_limit.used = 1;
        resultData.resource_data.cpu_limit.available = 1;
        resultData.resource_data.cpu_limit.max = 1;
        resultData.resource_data.net_limit = new resourceLimit();
        resultData.resource_data.net_limit.used = 1;
        resultData.resource_data.net_limit.available = 1;
        resultData.resource_data.net_limit.max = 1;
        resultData.resource_data.ram_usage = 1;
        resultData.resource_data.ram_quota = 1;
        resultData.eos = "1000000";
        resultData.utg = "1000000";

        PacketManager.Inst.ResponseLobbyInfo(resultData);
    }

    #endregion
}