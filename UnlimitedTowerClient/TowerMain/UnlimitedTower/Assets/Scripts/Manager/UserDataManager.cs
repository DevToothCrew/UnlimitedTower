using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataManager : MonoSingleton<UserDataManager>
{
    public UserInfo userInfo = default(UserInfo);

    // 대표 캐릭터
    public MainCharInfo userMainCharInfo = new MainCharInfo();
    public UserLobbyInfo userLobbyInfo = new UserLobbyInfo();

    public Dictionary<int, UserServantData> servantDic = new Dictionary<int, UserServantData>();

    public Dictionary<int, UserMonsterData> monsterDic = new Dictionary<int, UserMonsterData>();

    // 장비와 기타 아이템 소모품 등은 Equipment와 Item으로 변경 필요
    public Dictionary<int, UserEquipmentData> equipmentDic = new Dictionary<int, UserEquipmentData>();
    public Dictionary<int, UserEquipmentData> weaponDic = new Dictionary<int, UserEquipmentData>();
    public Dictionary<int, UserEquipmentData> armorDic = new Dictionary<int, UserEquipmentData>();
    public Dictionary<int, UserEquipmentData> accessoryDic = new Dictionary<int, UserEquipmentData>();

    // Dic<ID , Data>
    public Dictionary<int, UserItemData> itemDic = new Dictionary<int, UserItemData>();
    
    // 현재 파티는 1개, 파티 안에 Formation Info 포함
    public UserPartyData partyInfo = new UserPartyData();

    public UserStageStateData stageState =  new UserStageStateData();
    public battleActionData stageActionInfo = new battleActionData();
    public stageRewardData stageReward = new stageRewardData();

    public int currentDayCount = 0;
    public int dayCount = 0;
    public int currentDayTime = 0;

    public int usingPartyNum = 1;

    #region InitFunction

    public void InitUserInfo()
    {
        userInfo = null;
        userMainCharInfo = new MainCharInfo();
        userLobbyInfo = new UserLobbyInfo();
        servantDic = new Dictionary<int, UserServantData>();
        monsterDic = new Dictionary<int, UserMonsterData>();
        equipmentDic = new Dictionary<int, UserEquipmentData>();
        weaponDic = new Dictionary<int, UserEquipmentData>();
        armorDic = new Dictionary<int, UserEquipmentData>();
        accessoryDic = new Dictionary<int, UserEquipmentData>();
        itemDic = new Dictionary<int, UserItemData>();
        partyInfo = new UserPartyData();
        stageState = new UserStageStateData();
        stageActionInfo = new battleActionData();
        stageReward = new stageRewardData();
    }

    #endregion

    #region SetFunction

    public void SetUserInfo(UserInfo getUserInfo)
    {
        userInfo = getUserInfo;
    }

    public void SetUserEOS(ulong getEOS)
    {
        userInfo.userEOS = getEOS;
    }

    public void SetUserUTG(ulong getUTG)
    {
        userInfo.userUTG = getUTG;
    }

    public void SetUserInventoryInfo(UserInventoryInfo getInventoryInfo)
    {
        userInfo.inventoryInfo = getInventoryInfo;
    }

    public void SetUserLobbyInfo(UserLobbyInfo getLobbyInfo)
    {
        userLobbyInfo = getLobbyInfo;
    }

    public void SetServantDic(Dictionary<int, UserServantData> getServantDic)
    {
        servantDic = getServantDic;
    }

    public void SetMonsterDic(Dictionary<int, UserMonsterData> getMonsterDic)
    {
        monsterDic = getMonsterDic;
    }

    public void SetEquipmentDic(Dictionary<int, UserEquipmentData> getEquipmentDic)
    {
        equipmentDic = getEquipmentDic;

        foreach(KeyValuePair<int,UserEquipmentData> data in getEquipmentDic)
        {
            if(data.Value.equipmentType == EQUIPMENT_TYPE.WEAPON)
            {
                weaponDic.Add(data.Key, data.Value);
            }
            else if(data.Value.equipmentType == EQUIPMENT_TYPE.ARMOR)
            {
                armorDic.Add(data.Key, data.Value);
            }
            else if(data.Value.equipmentType == EQUIPMENT_TYPE.ACCESSSORY)
            {
                accessoryDic.Add(data.Key, data.Value);
            }
            else
            {
                DebugLog.Log(false, "Invalid Equipment Type : " + data.Value.equipmentType);
                return;
            }
        }
    }

    public void SetItemDic(Dictionary<int, UserItemData> getItemDic)
    {
        itemDic = getItemDic;
    }
    
    public void SetSceneState(SCENE_STATE state)
    {
        userInfo.sceneState = state;
    }

    public void SetPartyInfo(UserPartyData getPartyInfo)
    {
        if (partyInfo.formationDataDic.Count > 0)
        {
            for (int i = 0; i < 10; i++)
            {
                if (partyInfo.formationDataDic[i] != null)
                {
                    if (partyInfo.formationDataDic[i].index != 0)
                    {
                        if (i <= DEFINE.ServantMaxFormationNum)
                        {
                            servantDic[partyInfo.formationDataDic[i].index].partyIndex = 0;
                            servantDic[partyInfo.formationDataDic[i].index].state = 1;
                            servantDic[partyInfo.formationDataDic[i].index].isPlaced = false;
                        }
                        else
                        {
                            monsterDic[partyInfo.formationDataDic[i].index].partyIndex = 0;
                            monsterDic[partyInfo.formationDataDic[i].index].state = 1;
                            monsterDic[partyInfo.formationDataDic[i].index].isPlaced = false;
                        }
                    }
                }
            }
        }

        partyInfo = getPartyInfo;

        for (int i = 0; i < 10; i++)
        {
            if (partyInfo.formationDataDic[i].index != 0)
            {
                if (i <= DEFINE.ServantMaxFormationNum)
                {
                    servantDic[partyInfo.formationDataDic[i].index].partyIndex = partyInfo.partyIndex;
                    servantDic[partyInfo.formationDataDic[i].index].state = 5;
                    servantDic[partyInfo.formationDataDic[i].index].isPlaced = true;
                }
                else
                {
                    monsterDic[partyInfo.formationDataDic[i].index].partyIndex = partyInfo.partyIndex;
                    monsterDic[partyInfo.formationDataDic[i].index].state = 5;
                    monsterDic[partyInfo.formationDataDic[i].index].isPlaced = true;
                }
            }
        }

        if(partyInfo.formationDataDic[0].index > 0)
        {
            userMainCharInfo.mainCharID = servantDic[partyInfo.formationDataDic[0].index].id;
            userMainCharInfo.mainCharType = CHAR_TYPE.SERVANT;
            userMainCharInfo.grade = 5; // 현재는 Common Servant 뿐
        }
    }

    public void UpdateMainCharInfo(MainCharInfo charInfo)
    {
        userMainCharInfo = charInfo;
    }

    public void InitStageInfo()
    {
        stageState = new UserStageStateData();
        stageActionInfo = new battleActionData();
        stageReward = new stageRewardData();
        userInfo.sceneState = SCENE_STATE.Lobby;
    }

    public void SetStageState(UserStageStateData getStageStateData)
    {
        stageState = getStageStateData;
    }

    public void SetStageAction(battleActionData getBattleActionData)
    {
        stageActionInfo = getBattleActionData;
    }

    public void SetStageReward(stageRewardData getStageReward)
    {
        stageReward = getStageReward;
    }

    public void SetMonster(UserMonsterData monsterData)
    {
        if(monsterDic.ContainsKey(monsterData.index) == true)
        {
            monsterDic[monsterData.index] = monsterData;
        }
        else
        {
            monsterDic.Add(monsterData.index, monsterData);
        }
    }

    public void DelMailCount(int count)
    {
        if(userLobbyInfo.mailCount - count <= 0)
        {
            userLobbyInfo.mailCount = 0;
        }
        else
        {
            userLobbyInfo.mailCount -= count;
        }
    }

    public void SetServantExp(int index, int getExp, int getLvup)
    {
        if (servantDic.ContainsKey(index) == false)
        {
            DebugLog.Log(false, "Wrong Servant Index");
            return;
        }

        servantDic[index].exp += getExp;

        if (getLvup > 0)
        {
            servantDic[index].level += getLvup;
            //레벨업시 스탯 증가 반영해주는 코드 필요

            // Status addStatus = CSVData.Inst.GetServantLevelPerAddStatus(servantDic[index].jobType, servantDic[index].gradeType);
            // 
            // servantDic[index].status.basicStr += (getLvup * addStatus.basicStr);
            // servantDic[index].status.basicDex += (getLvup * addStatus.basicDex);
            // servantDic[index].status.basicInt += (getLvup * addStatus.basicInt);
        }
    }

    public void SetMonsterExp(int index, int getExp, int getLvup)
    {
        if (monsterDic.ContainsKey(index) == false)
        {
            DebugLog.Log(false, "Wrong Monster Index");
            return;
        }

        monsterDic[index].exp += getExp;

        if (getLvup > 0)
        {
            monsterDic[index].level += getLvup;
            //레벨업시 스탯 증가 반영해주는 코드 필요

            Status addStatus = new Status();

            monsterDic[index].status.basicStr += (getLvup * addStatus.basicStr);
            monsterDic[index].status.basicDex += (getLvup * addStatus.basicDex);
            monsterDic[index].status.basicInt += (getLvup * addStatus.basicInt);
        }
    }


    public void SetEquipment(UserEquipmentData equipmentData)
    {
        if(equipmentDic.ContainsKey(equipmentData.index) == true)
        {
            equipmentDic[equipmentData.index] = equipmentData;
            if (equipmentData.equipmentType == EQUIPMENT_TYPE.WEAPON)
            {
                weaponDic[equipmentData.index] = equipmentData;
            }
            else if (equipmentData.equipmentType == EQUIPMENT_TYPE.ARMOR)
            {
                armorDic[equipmentData.index] = equipmentData;
            }
            else if (equipmentData.equipmentType == EQUIPMENT_TYPE.ACCESSSORY)
            {
                accessoryDic[equipmentData.index] = equipmentData;
            }
        }
        else
        {
            equipmentDic.Add(equipmentData.index, equipmentData);
            if (equipmentData.equipmentType == EQUIPMENT_TYPE.WEAPON)
            {
                weaponDic.Add(equipmentData.index, equipmentData);
            }
            else if (equipmentData.equipmentType == EQUIPMENT_TYPE.ARMOR)
            {
                armorDic.Add(equipmentData.index, equipmentData);
            }
            else if (equipmentData.equipmentType == EQUIPMENT_TYPE.ACCESSSORY)
            {
                accessoryDic.Add(equipmentData.index, equipmentData);
            }
        }
    }

    public void SetServant(UserServantData servantData)
    {
        if (servantDic.ContainsKey(servantData.index) == true)
        {
            servantDic[servantData.index] = servantData;
        }
        else
        {
            servantDic.Add(servantData.index, servantData);
        }
    }

    public bool SetItem(UserItemData itemData)
    {
        if (itemDic.ContainsKey(itemData.id) == true)
        {
            if (itemData.itemInfoList.Count == 0)
            {
                itemDic.Remove(itemData.id);
            }
            else
            {
                itemDic[itemData.id] = itemData;
            }
        }
        else
        {
            itemDic.Add(itemData.id, itemData);
        }

        return true;
    }

    public bool checkItemIndex(int id, int index, int arrayIndex, UserItemData itemData)
    {
        if (itemDic.ContainsKey(id) == true)
        {
            for (int j = 0; j < itemDic[id].itemInfoList.Count; ++j)
            {
                if (itemDic[id].itemInfoList[j].index == index)
                {
                    itemDic[id].itemInfoList[j].count += itemData.itemInfoList[arrayIndex].count;
                    return true;
                }
            }
        }
        else
        {
            return false;
        }
        return false;
    }

    public bool SetItemList(List<UserItemData> itemDataList)
    {
        for(int i = 0; i < itemDataList.Count; i++)
        {
            if(SetItem(itemDataList[i]) == false)
            {
                return false;
            }
        }

        return true;
    }

    //public void SetMail(UserMailData mailData)
    //{
    //    if (mailDic.ContainsKey(mailData.index) == true)
    //    {
    //        mailDic[mailData.index] = mailData;
    //    }
    //    else
    //    {
    //        mailDic.Add(mailData.index, mailData);
    //    }
    //}

    #endregion

    #region GetFunction

    public SCENE_STATE GetSceneState()
    {
        if(userInfo == null)
        {
            return SCENE_STATE.Lobby;
        }

        return userInfo.sceneState;
    }

    public UserInfo GetUserInfo()
    {
        return userInfo;
    }

    public UserInventoryInfo GetUserInventoryInfo()
    {
        if (userInfo == null)
            return null;

        if (userInfo.inventoryInfo == null)
            return null;

        return userInfo.inventoryInfo;
    }

    public bool CheckInventoryCount()
    {
        if(userInfo == null)
        {
            return false;
        }

        if (userInfo.inventoryInfo.servantInventory <= GetServantCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Servant Inventory is Full");
            return false;
        }

        if (userInfo.inventoryInfo.monsterInventory <= GetMonsterCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Monster Inventory is Full");
            return false;
        }

        if (userInfo.inventoryInfo.equipmentInventory <= GetEquipmentCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Equipment Inventory is Full");
            return false;
        }

        if (userInfo.inventoryInfo.itemInventory <= GetItemTotalCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Item Inventory is Full");
            return false;
        }

        return true;
    }

    public ulong GetUserEOS() /* 기본적으로 EOS와 UTG는 1이 10000으로 처리됩니다. */
    {
        return userInfo.userEOS;
    }

    public ulong GetUserUTG() /* 기본적으로 EOS와 UTG는 1이 10000으로 처리됩니다. */
    {
        return userInfo.userUTG;
    }

    public UserLobbyInfo GetUserLobbyInfo()
    {
        return userLobbyInfo;
    }

    public MainCharInfo GetMainCharInfo()
    {
        return userMainCharInfo;
    }

    public UserPartyData GetUserPartyInfo()
    {
        return partyInfo;
    }

    public UserServantData GetServantInfo(int index)
    {
        if (servantDic.ContainsKey(index) == false)
        {
            DebugLog.Log(false, "Invalid GetServantInfo : " + index);
            return null;
        }

        return servantDic[index];
    }

    public UserServantData GetServantInfoFromFormation(int formationIndex)
    {
        if(formationIndex < 0 || formationIndex > DEFINE.ServantMaxFormationNum)
        {
            return null;
        }

        UserFormationData formationData = GetFormationData(formationIndex);
        if (formationData == null)
        {
            DebugLog.Log(false, "버그");
            return null;
        }

        return GetServantInfo(formationData.index);
    }

    public UserMonsterData GetMonsterInfo(int index)
    {
        if (monsterDic.ContainsKey(index) == false)
        {
            DebugLog.Log(false, "Invalid GetMonsterInfo : " + index);
            return null;
        }

        return monsterDic[index];
    }

    public UserMonsterData GetMonsterInfoFromFormation(int formationIndex)
    {
        if (formationIndex < DEFINE.MonsterMinFormationNum || formationIndex > DEFINE.MonsterMaxFormationNum)
        {
            return null;
        }

        UserFormationData formationData = GetFormationData(formationIndex);
        if (formationData == null)
        {
            DebugLog.Log(false, "버그");
            return null;
        }

        return GetMonsterInfo(formationData.index);
    }

    public UserEquipmentData GetEquipmentInfo(int index)
    {
        if(equipmentDic.ContainsKey(index) == false)
        {
            DebugLog.Log(false, "Invalid GetEquipmentInfo : " + index);
            return null;
        }

        return equipmentDic[index];
    }

    public UserItemData GetItemInfo(int id)
    {
        if(itemDic.ContainsKey(id) == false)
        {
            return null;
        }

        return itemDic[id];
    }

    public int GetItemTotalCount()
    {
        return itemDic.Count;
    }

    public int GetItemCount(int id)
    {
        int count = 0;

        if (itemDic.ContainsKey(id) == false)
        {
            return count;
        }

        for(int i = 0; i < itemDic[id].itemInfoList.Count; i++)
        {
            count += itemDic[id].itemInfoList[i].count;
        }

        return count;
    }

    public UserFormationData GetFormationData(int formationIndex)
    {
        if(partyInfo == null)
        {
            DebugLog.Log(false, "Invalid GetFormaData_nullPossible_1");
            return null;
        }

        if(partyInfo.formationDataDic == null)
        {
            DebugLog.Log(false, "Invalid GetFormaData_nullPossible_2");
            return null;
        }

        if (partyInfo.formationDataDic.ContainsKey(formationIndex) == false)
        {
            DebugLog.Log(false, "Invalid GetFormaData_nullPossible_3");
            return null;
        }

        return partyInfo.formationDataDic[formationIndex];
    }

    public bool GetFomationIsPlaced(int formationIndex)
    {
        UserFormationData formationdata = GetFormationData(formationIndex);
        if (formationdata == null)
        {
            return false;
        }

        return true;
    }

    public bool GetServantIsPlaced(int servantIndex)
    {
        if(servantDic.ContainsKey(servantIndex) == false)
        {
            DebugLog.Log(true, "Invalid Servant Index : " + servantIndex);
            return false;
        }

        return servantDic[servantIndex].isPlaced;
    }

    public bool GetMonsterIsPlaced(int monsterIndex)
    {
        if (monsterDic.ContainsKey(monsterIndex) == false)
        {
            DebugLog.Log(true, "Invalid Monster Index : " + monsterIndex);
            return false;
        }

        return monsterDic[monsterIndex].isPlaced;
    }


    public int GetServantEmptyFormation()
    {
        return 0;
    }

    public int GetMonsterEmptyFormation()
    {
        return 0;
    }

    public int GetServantCount()
    {
        return servantDic.Count;
    }

    public int GetMonsterCount()
    {
        return monsterDic.Count;
    }

    public int GetEquipmentCount()
    {
        return equipmentDic.Count;
    }

    public int GetWeaponCount()
    {
        return weaponDic.Count;
    }

    public int GetArmorCount()
    {
        return armorDic.Count;
    }

    public int GetAccessoryCount()
    {
        return accessoryDic.Count;
    }

    public int GetItemDicCount()
    {
        return itemDic.Count;
    }

    public List<UserServantData> GetServantList()
    {
        if(servantDic.Count == 0)
        {
            return null;
        }

        return servantDic.Values.ToList();
    }

    public List<UserMonsterData> GetMonsterList()
    {
        if(monsterDic.Count == 0)
        {
            return null;
        }

        return monsterDic.Values.ToList();
    }

    public List<UserEquipmentData> GetEquipmentList()
    {
        if(equipmentDic.Count == 0)
        {
            return null;
        }

        return equipmentDic.Values.ToList();
    }

    public List<UserEquipmentData> GetWeaponList()
    {
        if(weaponDic.Count == 0)
        {
            return null;
        }

        return weaponDic.Values.ToList();
    }

    public List<UserEquipmentData> GetArmorList()
    {
        if(armorDic.Count == 0)
        {
            return null;
        }

        return armorDic.Values.ToList();
    }

    public List<UserEquipmentData> GetAccessoryList()
    {
        if(accessoryDic.Count == 0)
        {
            return null;
        }

        return accessoryDic.Values.ToList();
    }

    public List<UserItemData> GetItemList()
    {
        if(itemDic.Count == 0)
        {
            return null;
        }

        return itemDic.Values.ToList();
    }

    public UserStageStateData GetStageState()
    {
        if (stageState == null)
        {
            DebugLog.Log(true, "스테이지 스테이트 버그");
            return null;
        }

        return stageState;
    }

    public battleActionData GetStageAction()
    {
        if (stageActionInfo == null)
        {
            DebugLog.Log(true, "액션 버그");
            return null;
        }

        return stageActionInfo;
    }

    public stageRewardData GetStageReward()
    {
        if (stageReward == null)
        {
            DebugLog.Log(true, "보상 버그");
            return null;
        }

        return stageReward;
    }

    #endregion

    #region AddFunction

    // 이름 겹치는 버그에 대한 널처리 필요
    public bool AddServantData(UserServantData servantData)
    {
        if(servantDic.ContainsKey(servantData.index) == true)
        {
            return false;
        }

        servantDic.Add(servantData.index, servantData);
        return true;
    }

    public bool AddMonsterData(UserMonsterData monsterData)
    {
        if(monsterDic.ContainsKey(monsterData.index) == true)
        {
            return false;
        }

        monsterDic.Add(monsterData.index, monsterData);
        return true;
    }

    public bool AddEquipmentData(UserEquipmentData equipmentData)
    {
        if (equipmentDic.ContainsKey(equipmentData.index) == true)
        {
            return false;
        }

        equipmentDic.Add(equipmentData.index, equipmentData);

        if (equipmentData.equipmentType == EQUIPMENT_TYPE.WEAPON)
        {
            weaponDic.Add(equipmentData.index, equipmentData);
        }
        else if (equipmentData.equipmentType == EQUIPMENT_TYPE.ARMOR)
        {
            armorDic.Add(equipmentData.index, equipmentData);
        }
        else if (equipmentData.equipmentType == EQUIPMENT_TYPE.ACCESSSORY)
        {
            accessoryDic.Add(equipmentData.index, equipmentData);
        }

        return true;
    }

    #endregion

    #region DelFunction

    public bool DelServant(int index)
    {
        if(servantDic.ContainsKey(index) == false)
        {
            return false;
        }

        servantDic.Remove(index);
        return true;
    }

    public bool DelServantList(List<int> delServantIndexList)
    {
        for(int i = 0; i < delServantIndexList.Count; i++)
        {
            if(DelServant(delServantIndexList[i]) == false)
            {
                return false;
            }
        }

        return true;
    }

    public bool DelMonster(int index)
    {
        if(monsterDic.ContainsKey(index) == false)
        {
            return false;
        }

        monsterDic.Remove(index);
        return true;
    }

    public bool DelMonsterList(List<int> delMonsterIndexList)
    {
        for(int i = 0; i < delMonsterIndexList.Count; i++)
        {
            if(DelMonster(delMonsterIndexList[i]) == false)
            {
                return false;
            }
        }

        return true;
    }

    public bool DelEquipment(int index)
    {
        if(equipmentDic.ContainsKey(index) == false)
        {
            return false;
        }

        if (equipmentDic[index].equipmentType == EQUIPMENT_TYPE.WEAPON)
        {
            weaponDic.Remove(index);
        }
        else if (equipmentDic[index].equipmentType == EQUIPMENT_TYPE.ARMOR)
        {
            armorDic.Remove(index);
        }
        else if (equipmentDic[index].equipmentType == EQUIPMENT_TYPE.ACCESSSORY)
        {
            accessoryDic.Remove(index);
        }

        equipmentDic.Remove(index);

        return true;
    }

    public bool DelEquipmentList(List<int> delEquipmentIndexList)
    {
        for(int i = 0; i < delEquipmentIndexList.Count; i++)
        {
            if(DelEquipment(delEquipmentIndexList[i]) == false)
            {
                return false;
            }
        }

        return true;
    }

    public bool UseEOS(ulong EOSCount)
    {
        if (userInfo.userEOS < EOSCount)
        {
            return false;            
        }

        userInfo.userEOS -= EOSCount;
        LobbyTopInfo.Inst.UpdateTopInfo();

        return true;
    }

    public void addUTG(ulong UTGCount)
    {
        if (UTGCount == 0)
            return;
        userInfo.userUTG += UTGCount;
    }

    public void addRankExp(int Exp, int Level)
    {
        userInfo.userEXP += Exp;
        userInfo.level += Level;
    }

    public bool EquipServant(int servantIndex, EQUIPMENT_TYPE equipmentType, int equipmentIndex)
    {
        if(equipmentIndex == 0)
        {
            DebugLog.Log(false, "Unequip");
            return true;
        }

        if (servantDic.ContainsKey(servantIndex) == false)
        {
            DebugLog.Log(false, "Invalid Servant Index : " + servantIndex);
            return false;
        }

        if (equipmentDic.ContainsKey(equipmentIndex) == false)
        {
            DebugLog.Log(false, "Invalid Equipment Index : " + equipmentIndex);
            return false;
        }

        if (equipmentType == EQUIPMENT_TYPE.WEAPON)
        {
            if (weaponDic.ContainsKey(equipmentIndex) == false)
            {
                DebugLog.Log(false, "Invalid Weapon Index : " + equipmentIndex);
                return false;
            }
        }
        else if (equipmentType == EQUIPMENT_TYPE.ARMOR)
        {
            if (armorDic.ContainsKey(equipmentIndex) == false)
            {
                DebugLog.Log(false, "Invalid Armor Index : " + equipmentIndex);
                return false;
            }
        }
        else if (equipmentType == EQUIPMENT_TYPE.ACCESSSORY)
        {
            if (accessoryDic.ContainsKey(equipmentIndex) == false)
            {
                DebugLog.Log(false, "Invalid Accessory Index : " + equipmentIndex);
                return false;
            }
        }
        else
        {
            DebugLog.Log(false, "Invalid Equipment Type : " + equipmentType.ToString());
            return false;
        }

        if (servantDic[servantIndex].equipmentDic[equipmentType] != 0)
        {
            DebugLog.Log(false, "Invalid Equipment is Already Equip");
            return false;
        }

        servantDic[servantIndex].equipmentDic[equipmentType] = equipmentIndex;
        equipmentDic[equipmentIndex].equipServantIndex = servantIndex;
        equipmentDic[equipmentIndex].state = 2;
        equipmentDic[equipmentIndex].isEquiped = true;

        if (equipmentType == EQUIPMENT_TYPE.WEAPON)
        {
            weaponDic[equipmentIndex].equipServantIndex = equipmentIndex;
            weaponDic[equipmentIndex].state = 2;
            weaponDic[equipmentIndex].isEquiped = true;
        }
        else if (equipmentType == EQUIPMENT_TYPE.ARMOR)
        {
            armorDic[equipmentIndex].equipServantIndex = equipmentIndex;
            armorDic[equipmentIndex].state = 2;
            armorDic[equipmentIndex].isEquiped = true;
        }
        else if (equipmentType == EQUIPMENT_TYPE.ACCESSSORY)
        {
            accessoryDic[equipmentIndex].equipServantIndex = equipmentIndex;
            accessoryDic[equipmentIndex].state = 2;
            accessoryDic[equipmentIndex].isEquiped = true;
        }

        return true;
    }

    public bool UnequipServant(int servantIndex, EQUIPMENT_TYPE equipmentType)
    {
        if(servantDic.ContainsKey(servantIndex) == false)
        {
            DebugLog.Log(false, "Invalid Servant Index : " + servantIndex);
            return false;
        }

        int equipmentIndex = servantDic[servantIndex].equipmentDic[equipmentType];

        if (equipmentDic.ContainsKey(equipmentIndex) == false)
        {
            DebugLog.Log(false, "Invalid Equipment Index : " + equipmentIndex);
            return false;
        }

        if(equipmentType == EQUIPMENT_TYPE.WEAPON)
        {
            if(weaponDic.ContainsKey(equipmentIndex) == false)
            {
                DebugLog.Log(false, "Invalid Weapon Index : " + equipmentIndex);
                return false;
            }
        }
        else if(equipmentType == EQUIPMENT_TYPE.ARMOR)
        {
            if (armorDic.ContainsKey(equipmentIndex) == false)
            {
                DebugLog.Log(false, "Invalid Armor Index : " + equipmentIndex);
                return false;
            }
        }
        else if(equipmentType == EQUIPMENT_TYPE.ACCESSSORY)
        {
            if (accessoryDic.ContainsKey(equipmentIndex) == false)
            {
                DebugLog.Log(false, "Invalid Accessory Index : " + equipmentIndex);
                return false;
            }
        }
        else
        {
            DebugLog.Log(false, "Invalid Equipment Type : " + equipmentType.ToString());
            return false;
        }

        servantDic[servantIndex].equipmentDic[equipmentType] = 0;
        equipmentDic[equipmentIndex].equipServantIndex = 0;
        equipmentDic[equipmentIndex].state = 1;
        equipmentDic[equipmentIndex].isEquiped = false;

        if(equipmentType == EQUIPMENT_TYPE.WEAPON)
        {
            weaponDic[equipmentIndex].equipServantIndex = 0;
            weaponDic[equipmentIndex].state = 1;
            weaponDic[equipmentIndex].isEquiped = false;
        }
        else if(equipmentType == EQUIPMENT_TYPE.ARMOR)
        {
            armorDic[equipmentIndex].equipServantIndex = 0;
            armorDic[equipmentIndex].state = 1;
            armorDic[equipmentIndex].isEquiped = false;
        }
        else if(equipmentType == EQUIPMENT_TYPE.ACCESSSORY)
        {
            accessoryDic[equipmentIndex].equipServantIndex = 0;
            accessoryDic[equipmentIndex].state = 1;
            accessoryDic[equipmentIndex].isEquiped = false;
        }

        return true;
    }

    #endregion
}
