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

        userLoginData.token = utg;
        userLoginData.eos = eos;

        userLoginData.user_data.user = user;
        userLoginData.user_data.state = 2;
        //userLoginData.userinfo.hero = GetRandomServant(GetRandomServantJob());

        partyData partyData = new partyData();
        partyData.index = 1;
        partyData.state = 0;

        for (int i = 1; i < 6; i++)
        {
            userLoginData.servant_list.Add(GetRandomServantData(i, GetRandomServantJob()));

            if (i < 5)
            {
                partyData.servant_list.Add(i);
            }
        }

        for (int i = 1; i < 6; i++)
        {
            userLoginData.monster_list.Add(GetRandomMonster(i));

            if (i < 6)
            {
                partyData.monster_list.Add(i);
            }
        }

        // Item 추가 예정

        userLoginData.party_info = partyData;

        return JsonMapper.ToJson(userLoginData).ToString();
    }

    public SERVANT_JOB GetRandomServantJob()
    {
        int job = rand.Next(1, 6);

        if (job == 1)
        {
            return SERVANT_JOB.Warrior;
        }
        else if (job == 2)
        {
            return SERVANT_JOB.Thief;
        }
        else if (job == 3)
        {
            return SERVANT_JOB.Archer;
        }
        else if (job == 4)
        {
            return SERVANT_JOB.Magician;
        }
        else if (job == 5)
        {
            return SERVANT_JOB.Cleric;
        }
        else
        {
            return SERVANT_JOB.WhiteHand;
        }
    }

    public string GetGachaResultData(int gachaIndex)
    {
        int type = rand.Next((int)GACHA_RESULT_TYPE.Servant, (int)GACHA_RESULT_TYPE.Equipment);

        if (type == (int)GACHA_RESULT_TYPE.Servant)
        {
            gachaServantData gachaResult = new gachaServantData();
            gachaResult.result_type = type;
            gachaResult.data = GetRandomServantData(UserDataManager.Inst.servantDic.Count + 1, GetRandomServantJob());

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
            gachaItemData gachaResult = new gachaItemData();

            return JsonMapper.ToJson(gachaResult).ToString();
        }
        else
        {
            return null;
        }
    }

    public string GetBattleActionData(string user, int getTurn)
    {
        stageStateData stageStateInfo = UserDataManager.Inst.GetStageState();

        battleActionData battleactiondata = new battleActionData();
        battleactiondata.user = user;
        battleactiondata.turn = getTurn;

        for (int i = 0; i < stageStateInfo.my_state_list.Count; ++i)
        {
            if (BattleManager.Inst.NowHp[i] == 0)
            {
                continue;
            }
            if(stageStateInfo.my_state_list[i].position < 5)
            {
                characterActionData actioninfo = new characterActionData();
                actioninfo.my_position = stageStateInfo.my_state_list[i].position;
                actioninfo.action_type = 3;

                if (stageStateInfo.my_state_list[i].active_skill_list[0].id == 200001)
                {
                    for (int target = 0; target < 2; ++target)
                    {
                        actionInfo action = new actionInfo();
                        do
                        {
                            action.target_position = UnityEngine.Random.Range(10, 20);
                        } while (BattleManager.Inst.NowHp[action.target_position] == 0);
                        action.avoid = false;
                        action.critical = false;
                        action.damage = rand.Next(200, 500);
                        actioninfo.action_info_list.Add(action);
                    }
                }
                else if (stageStateInfo.my_state_list[i].active_skill_list[0].id == 200002)
                {
                    actionInfo action = new actionInfo();
                    do
                    {
                        action.target_position = UnityEngine.Random.Range(10, 20);
                    } while (BattleManager.Inst.NowHp[action.target_position] == 0);
                    action.avoid = false;
                    action.critical = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
                    action.damage = rand.Next(200, 500);
                    actioninfo.action_info_list.Add(action);
                }
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
                } while (BattleManager.Inst.NowHp[action.target_position] == 0);
                action.avoid = false;
                action.critical = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
                action.damage = rand.Next(200, 500);

                characterActionData actioninfo = new characterActionData();
                actioninfo.my_position = stageStateInfo.my_state_list[i].position;
                actioninfo.action_type = 2;
                actioninfo.action_info_list.Add(action);

                battleactiondata.character_action_list.Add(actioninfo);
            }
        }


        for (int i = 10; i < 20; ++i)
        {
            if (BattleManager.Inst.NowHp[i] == 0)
            {
                continue;
            }
            actionInfo action = new actionInfo();
            do
            {
                action.target_position = UnityEngine.Random.Range(0, 10);
            } while (BattleManager.Inst.NowHp[action.target_position] == 0);
            action.avoid = false;
            action.critical = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
            action.damage = rand.Next(200, 500);

            characterActionData actioninfo = new characterActionData();
            actioninfo.my_position = i;
            actioninfo.action_type = 2;
            actioninfo.action_info_list.Add(action);

            battleactiondata.character_action_list.Add(actioninfo);
        }


        return JsonMapper.ToJson(battleactiondata);
    }

    public string GetStageStartData(string user, int stageNum, int partyNum)
    {
        stageStateData battlestatedata = new stageStateData();
        battlestatedata.user = user;
        battlestatedata.stage_type = 0;
        battlestatedata.enemy_user = user;
        battlestatedata.stage_number = 0;
        battlestatedata.turn = 0;


        for (int i = 0; i < 10; ++i)
        {
            characterStateData newMember = new characterStateData();
            if (i < 5)
            {
                newMember.position = i;
                newMember.index = i + 1;
                skillInfo skill = new skillInfo();
                if (i < 2)
                    skill.id = 200001;
                else
                    skill.id = 200002;
                newMember.active_skill_list = new List<skillInfo>();
                newMember.active_skill_list.Add(skill);
            }
            else
            {
                newMember.position = i;
                newMember.index = i - 4;
            }

            newMember.id = CSVData.Inst.GetRandomMonsterIndex();
            newMember.now_hp = 10000;
            newMember.physical_attack = 10000;
            newMember.physical_defense = 10;
            newMember.physical_crit_dmg = 1;
            newMember.magic_attack = 10000;
            newMember.magic_defense = 10;
            newMember.magic_crit_dmg = 1;
            newMember.physical_crit_per = 5;
            newMember.magic_crit_per = 5;
            newMember.avoid = 5;
            newMember.state = 0;
            newMember.speed = 25;
            newMember.status = new totalStatus();
            newMember.status.total_str = 10;
            newMember.status.total_dex = 10;
            newMember.status.total_int = 10;

            battlestatedata.my_state_list.Add(newMember);
        }

        for (int i = 0; i < 10; ++i)
        {
            characterStateData newMember = new characterStateData();
            newMember.position = i + 10;
            newMember.index = 0;
            newMember.id = CSVData.Inst.GetRandomMonsterIndex();
            newMember.now_hp = 10000;
            newMember.physical_attack = 10000;
            newMember.physical_defense = 10;
            newMember.physical_crit_dmg = 1;
            newMember.magic_attack = 10000;
            newMember.magic_defense = 10;
            newMember.magic_crit_dmg = 1;
            newMember.physical_crit_per = 5;
            newMember.magic_crit_per = 5;
            newMember.avoid = 5;
            newMember.state = 0;
            newMember.speed = 25;
            newMember.status = new totalStatus();
            newMember.status.total_str = 10;
            newMember.status.total_dex = 10;
            newMember.status.total_int = 10;


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

        if (mainMonster.gradeNum != subMonster.gradeNum)
        {
            Debug.Log("Different Monster Grade Main : " + mainMonster.gradeNum + " , Sub : " + subMonster.gradeNum);
            return null;
        }

        if (mainMonster.upgradeCount < subMonster.upgradeCount || mainMonster.upgradeCount >= DEFINE.MAX_MONSTER_UPGRADE_COUNT)
        {
            Debug.Log("InvalidUpgradeCount Monster UpgradeCount Main : " + mainMonster.gradeNum + " , Sub : " + subMonster.gradeNum);
            return null;
        }

        DBMonsterUpgradeData dbMonsterUpgradeData = CSVData.Inst.GetMonsterUpgradeData(mainMonster.upgradeCount, subMonster.upgradeCount);
        if(dbMonsterUpgradeData == null)
        {
            Debug.Log("Invalid Monster Key : " + ((mainMonster.upgradeCount * 100) + subMonster.upgradeCount));
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
        getMonsterUpgradeResultData.need_item_list = new List<itemData>();
        itemData needItem = new itemData();
        needItem.item = dbMonsterUpgradeData.needItem;
        getMonsterUpgradeResultData.need_item_list.Add(needItem);

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

        newMonsterData.monster.type = monsterData.elementType;
        newMonsterData.monster.grade = getMonsterData.gradeNum;
        newMonsterData.monster.exp = getMonsterData.exp;
        newMonsterData.monster.upgrade = getMonsterData.upgradeCount;
        newMonsterData.monster.monster_class = monsterData.classType;
        newMonsterData.monster.status.basic_str = getMonsterData.status.basicStr;
        newMonsterData.monster.status.basic_dex = getMonsterData.status.basicDex;
        newMonsterData.monster.status.basic_int = getMonsterData.status.basicInt;
        newMonsterData.monster.status.plus_str = getMonsterData.status.plusStr;
        newMonsterData.monster.status.plus_dex = getMonsterData.status.plusDex;
        newMonsterData.monster.status.plus_int = getMonsterData.status.plusInt;
        // 스킬 추가 필요

        return newMonsterData;
    }

    public string GetServantGrindData(List<int> grindServantIndexList)
    {
        sellMonsterResultData resultData = new sellMonsterResultData();
        resultData.sellMonsterIndexList = grindServantIndexList;
        resultData.itemList = new List<itemData>();

        Dictionary<int, itemData> addItemDic = new Dictionary<int, itemData>();
        for (int i = 0; i < grindServantIndexList.Count; i++)
        {
            UserServantData servantData = UserDataManager.Inst.GetServantInfo(grindServantIndexList[i]);
            if (servantData == null)
            {
                Debug.Log("Invalid Servant Index : " + servantData.index);
                return null;
            }

            if (servantData.partyNum != 0)
            {
                Debug.Log("Invalid Request Servant Already placed Party : " + servantData.partyNum);
                return null;
            }

            itemData getItem = new itemData();
            getItem.item = new itemInfo();
            getItem.item.count = 1;

            switch (servantData.jobNum)
            {
                case 1:
                    getItem.item.id = 110010;
                    break;

                case 2:
                    getItem.item.id = 110020;
                    break;

                case 3:
                    getItem.item.id = 110030;
                    break;

                case 4:
                    getItem.item.id = 110040;
                    break;

                case 5:
                    getItem.item.id = 110050;
                    break;

                default:
                    Debug.Log("Invalid Request servantData JobNum : " + servantData.jobNum);
                    return null;
            }

            if(addItemDic.ContainsKey(getItem.item.id) == true)
            {
                addItemDic[getItem.item.id].item.count += getItem.item.count;
            }
            else
            {
                addItemDic.Add(getItem.item.id, getItem);
            }
        }

        List<int> keyList = addItemDic.Keys.ToList();
        for(int i = 0; i < keyList.Count; i++)
        {
            UserItemData itemData = UserDataManager.Inst.GetItemInfo(keyList[i]);
            if (itemData == null)
            {
                resultData.itemList.Add(addItemDic[keyList[i]]);
            }
            else
            {
                addItemDic[keyList[i]].item.count += itemData.count;

                resultData.itemList.Add(addItemDic[keyList[i]]);
            }
        }

        return JsonMapper.ToJson(resultData).ToString();
    }

    public string GetMonsterSellData(List<int> sellMonsterIndexList)
    {
        itemData token = new itemData();
        token.index = 0;
        token.item.id = 10001;
        token.item.count = Convert.ToInt32(UserDataManager.Inst.GetUserInfo().userUTG);

        for(int i = 0; i < sellMonsterIndexList.Count; i++)
        {
            UserMonsterData monsterData = UserDataManager.Inst.GetMonsterInfo(sellMonsterIndexList[i]);
            if(monsterData == null)
            {
                Debug.Log("Invalid Monster Index : " + monsterData.index);
                return null;
            }

            if(monsterData.partyNum != 0)
            {
                Debug.Log("Invalid Request Monster Already placed Party : " + monsterData.partyNum);
                return null;
            }

            token.item.count += 1000000;
        }

        sellMonsterResultData resultData = new sellMonsterResultData();
        resultData.sellMonsterIndexList = sellMonsterIndexList;
        resultData.itemList = new List<itemData>();
        resultData.itemList.Add(token);

        return JsonMapper.ToJson(resultData).ToString();
    }

    public string GetEquipmentSellData(List<int> sellEquipmentIndexList)
    {
        itemData token = new itemData();
        token.index = 0;
        token.item.id = 10001;
        token.item.count = Convert.ToInt32(UserDataManager.Inst.GetUserInfo().userUTG);

        for (int i = 0; i < sellEquipmentIndexList.Count; i++)
        {
            UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(sellEquipmentIndexList[i]);
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

            token.item.count += 1000000;
        }

        sellEquipmentResultData resultData = new sellEquipmentResultData();
        resultData.sellEquipmentIndexList = sellEquipmentIndexList;
        resultData.itemList = new List<itemData>();
        resultData.itemList.Add(token);

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
        rewardData.user = "devtooth";
        rewardData.reward_money = 100000;

        for (int i = 0; i < 10; ++i)
        {
            rewardData.get_exp_list.Add(100);
        }

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

    public servantData GetRandomServantData(int index, SERVANT_JOB job)
    {
        servantData servant = new servantData();
        servant.index = index;
        servant.servant = GetRandomServant(job);

        return servant;
    }

    public servantInfo GetRandomServant(SERVANT_JOB job)
    {
        servantInfo servant = new servantInfo();

        servant.state = 0;
        servant.exp = rand.Next(0, DEFINE.MAX_EXP);
        servant.job = 1; // rand.Next(0, 6);
        servant.stat_point = (Calculator.GetLevelForExp(servant.exp) - 1) * DEFINE.BONUS_STAT;
        servant.appear = GetRandomAppear();
        servant.status = GetRandomStatusInfo();
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
        monsterData.monster.state = 0;
        monsterData.monster.exp = rand.Next(0, DEFINE.MAX_EXP);

        monsterData.monster.type = 0;

        monsterData.monster.id = CSVData.Inst.GetRandomMonsterIndex();
        monsterData.monster.grade = rand.Next(1, 6);
        monsterData.monster.upgrade = 0;
        monsterData.monster.status = GetRandomStatusInfo();
        // TODO : 업그레이드에 따른 스테이터스 가중치 추가 필요

        return monsterData;
    }

    public appearInfo GetRandomAppear()
    {
        appearInfo appear = new appearInfo();
        appear.hair = rand.Next((int)APPEAR_HAIR.BASE, (int)APPEAR_HAIR.MAX);
        appear.head = rand.Next((int)APPEAR_HEAD.BASE, (int)APPEAR_HEAD.MAX);
        appear.body = rand.Next((int)APPEAR_BODY.BASE, (int)APPEAR_BODY.MAX);
        appear.gender = rand.Next(0, 2);

        return appear;
    }

    public statusInfo GetRandomStatusInfo()
    {
        statusInfo status = new statusInfo();
        status.basic_str = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
        status.basic_dex = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
        status.basic_int = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);

        status.plus_str = 0;
        status.plus_dex = 0;
        status.plus_int = 0;

        return status;
    }

    public Status GetRandomStatus()
    {
        Status status = new Status();
        status.basicStr = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
        status.basicDex = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
        status.basicInt = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);

        status.plusStr = 0;
        status.plusDex = 0;
        status.plusInt = 0;

        return status;
    }

    #region RequestPacketCheat

    public void RequestLoginCheat()
    {
        if (UserDataManager.Inst.GetUserInfo() == null)
        {
            Debug.Log("Start SetLoginCheat");

            string loginJson = GetUserLoginData("devtooth", "1000", "9999999");
            Debug.Log("[SUCCESS] User Login :" + loginJson);

            PacketManager.Inst.ResponseLogin(loginJson);
        }
        else
        {
            Debug.Log("Already Get UserInfo");
        }
    }

    public void RequestStageStartCheat(int stageNum, int partyNum)
    {
        if (UserDataManager.Inst.GetUserInfo() == null)
        {
            Debug.Log("Invalid UserInfo, Please First SetLoginCheat");
            return;
        }

        Debug.Log("Start SetStageStartCheat");

        string stageStateJson = GetStageStartData(UserDataManager.Inst.GetUserInfo().userName, stageNum, partyNum);
        Debug.Log("[SUCCESS] User Stage Start :" + stageStateJson);

        stageStateData getBattleStageData = JsonUtility.FromJson<stageStateData>(stageStateJson);
        PacketManager.Inst.ResponseStageStart(getBattleStageData);
    }

    public void RequestGachaCheat(int gachaIndex)
    {
        string gachaDataJson = GetGachaResultData(gachaIndex);
        if(gachaDataJson == null)
        {
            Debug.Log("Invalid Request Gacha");
            return;
        }

        Debug.Log("[SUCCESS] Gacha : " + gachaDataJson);
        PacketManager.Inst.ResponseGacha(gachaDataJson);
    }

    public void RequestMonsterUpgradeCheat(int mainMonsterIndex, int subMonsterIndex)
    {
        string monsterUpgradeResultJson = GetMonsterUpgradeData(mainMonsterIndex, subMonsterIndex);
        Debug.Log("[SUCCESS] Monster Upgrade : " + monsterUpgradeResultJson);

        monsterUpgradeResultData getMonsterUpgradeResultData = JsonUtility.FromJson<monsterUpgradeResultData>(monsterUpgradeResultJson);
        PacketManager.Inst.ResponseMonsterUpgrade(getMonsterUpgradeResultData);
    }

    public void RequestServantGrindCheat(List<int> servantIndexList)
    {
        string servantGrindResultJson = GetServantGrindData(servantIndexList);
        Debug.Log("[SUCCESS] Servant Grind : " + servantGrindResultJson);

        servantGrindResultData getServantGrindResultData = JsonUtility.FromJson<servantGrindResultData>(servantGrindResultJson);
        PacketManager.Inst.ResponseServantGrind(getServantGrindResultData);
    }

    public void RequestMonsterSellCheat(List<int> monsterIndexList)
    {
        string monsterSellResultJson = GetMonsterSellData(monsterIndexList);
        Debug.Log("[SUCCESS] Monster Sell : " + monsterSellResultJson);

        sellMonsterResultData getSellMonsterResultData = JsonUtility.FromJson<sellMonsterResultData>(monsterSellResultJson);
        PacketManager.Inst.ResponseMonsterSell(getSellMonsterResultData);
    }

    public void RequestEquipmentSellCheat(List<int> equipmentIndexList)
    {
        string equipmentSellResultJson = GetEquipmentSellData(equipmentIndexList);
        Debug.Log("[SUCCESS] Equipment Sell : " + equipmentSellResultJson);

        sellEquipmentResultData getSellEquipmentResultData = JsonUtility.FromJson<sellEquipmentResultData>(equipmentSellResultJson);
        PacketManager.Inst.ResponseEquipmentSell(getSellEquipmentResultData);
    }

    #endregion
}