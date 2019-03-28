using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataManager : MonoSingleton<UserDataManager>
{
    public UserInfo userInfo = default(UserInfo);

    public Dictionary<int, UserServantData> servantDic = new Dictionary<int, UserServantData>();

    public Dictionary<int, UserMonsterData> monsterDic = new Dictionary<int, UserMonsterData>();

    // 장비와 기타 아이템 소모품 등은 Equipment와 Item으로 변경 필요
    public Dictionary<int, UserEquipmentData> equipmentDic = new Dictionary<int, UserEquipmentData>();
    public Dictionary<int, UserItemData> itemDic = new Dictionary<int, UserItemData>();
    
    // 현재 파티는 1개, 파티 안에 Formation Info 포함
    public UserPartyData partyInfo = new UserPartyData();

    public stageStateData stageState =  new stageStateData();
    public battleActionData stageActionInfo = new battleActionData();
    public stageRewardData stageReward = new stageRewardData();

    public int usingPartyNum = 1;

    #region InitFunction

    public void InitUserInfo()
    {
        userInfo = new UserInfo();
        servantDic = new Dictionary<int, UserServantData>();
        monsterDic = new Dictionary<int, UserMonsterData>();
        equipmentDic = new Dictionary<int, UserEquipmentData>();
        itemDic = new Dictionary<int, UserItemData>();
        partyInfo = new UserPartyData();
    }

    #endregion

    #region SetFunction

    public void SetUserInfo(UserInfo getUserInfo)
    {
        userInfo = getUserInfo;
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
        partyInfo = getPartyInfo;

        foreach(KeyValuePair<int, UserFormationData> dic in partyInfo.formationDataDic)
        {
            if(dic.Key > 0 && dic.Key <= DEFINE.ServantMaxFormationNum)
            {
                if(servantDic.ContainsKey(dic.Value.index) == false)
                {
                    Debug.LogError("Invalid Servant Index : " + dic.Value.index);
                }
                servantDic[dic.Value.index].isPlaced = true;
            }
            else if (dic.Key > DEFINE.ServantMaxFormationNum && dic.Key <= DEFINE.MonsterMaxFormationNum)
            {
                if (monsterDic.ContainsKey(dic.Value.index) == false)
                {
                    Debug.LogError("Invalid Monster Index : " + dic.Value.index);
                }
                monsterDic[dic.Value.index].isPlaced = true;
            }
        }
    }

    public void SetStageState(stageStateData getStageStateData)
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

    public void SetEquipment(UserEquipmentData equipmentData)
    {
        if(equipmentDic.ContainsKey(equipmentData.index) == true)
        {
            equipmentDic[equipmentData.index] = equipmentData;
        }
        else
        {
            equipmentDic.Add(equipmentData.index, equipmentData);
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

    public void SetItem(UserItemData itemData)
    {
        if (itemDic.ContainsKey(itemData.index) == true)
        {
            itemDic[itemData.index] = itemData;
        }
        else
        {
            itemDic.Add(itemData.index, itemData);
        }
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

    public ulong GetUserEOS() /* 기본적으로 EOS와 UTG는 1이 10000으로 처리됩니다. */
    {
        return userInfo.userEOS;
    }

    public ulong GetUserUTG() /* 기본적으로 EOS와 UTG는 1이 10000으로 처리됩니다. */
    {
        return userInfo.userMoney;
    }

    public UserServantData GetHeroInfo()
    {
        if (userInfo == null)
        {
            Debug.Log("Invalid UserInfo");
            return null;
        }

        return userInfo.userHero;
    }

    public UserPartyData GetUserPartyInfo()
    {
        return partyInfo;
    }

    public UserServantData GetServantInfo(int index)
    {
        if (servantDic.ContainsKey(index) == false)
        {
            Debug.Log("Invalid GetServantInfo : " + index);
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
            Debug.Log("버그");
            return null;
        }

        return GetServantInfo(formationData.index);
    }

    public UserMonsterData GetMonsterInfo(int index)
    {
        if (monsterDic.ContainsKey(index) == false)
        {
            Debug.Log("Invalid GetMonsterInfo : " + index);
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
            Debug.Log("버그");
            return null;
        }

        return GetMonsterInfo(formationData.index);
    }

    public UserEquipmentData GetEquipmentInfo(int index)
    {
        if(equipmentDic.ContainsKey(index) == false)
        {
            Debug.Log("Invalid GetEquipmentInfo : " + index);
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

    public UserFormationData GetFormationData(int formationIndex)
    {
        if(partyInfo == null)
        {
            Debug.Log("Invalid GetFormaData_nullPossible_1");
            return null;
        }

        if(partyInfo.formationDataDic == null)
        {
            Debug.Log("Invalid GetFormaData_nullPossible_2");
            return null;
        }

        if (partyInfo.formationDataDic.ContainsKey(formationIndex) == false)
        {
            Debug.Log("Invalid GetFormaData_nullPossible_3");
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
            Debug.LogError("Invalid Servant Index : " + servantIndex);
            return false;
        }

        return servantDic[servantIndex].isPlaced;
    }

    public bool GetMonsterIsPlaced(int monsterIndex)
    {
        if (monsterDic.ContainsKey(monsterIndex) == false)
        {
            Debug.LogError("Invalid Monster Index : " + monsterIndex);
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

    public int GetItemCount()
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

    public List<UserItemData> GetItemList()
    {
        if(itemDic.Count == 0)
        {
            return null;
        }

        return itemDic.Values.ToList();
    }

    public stageStateData GetStageState()
    {
        if (stageState == null)
        {
            Debug.LogError("스테이지 스테이트 버그");
            return null;
        }

        return stageState;
    }

    public battleActionData GetStageAction()
    {
        if (stageActionInfo == null)
        {
            Debug.LogError("액션 버그");
            return null;
        }

        return stageActionInfo;
    }

    public stageRewardData GetStageReward()
    {
        if (stageReward == null)
        {
            Debug.LogError("보상 버그");
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
        return true;
    }

    public bool AddItemData(UserItemData itemData)
    {
        if (itemDic.ContainsKey(itemData.index) == true)
        {
            // 최대 개수 넘었을때에 대한 처리가 필요
            itemDic[itemData.index].count = itemDic[itemData.index].count + itemData.count;
        }
        else
        {
            itemDic.Add(itemData.index, itemData);
        }
        return true;
    }

    public bool AddItemDataList(List<UserItemData> itemDataList)
    {
        for(int i = 0; i < itemDataList.Count; i++)
        {
            if(AddItemData(itemDataList[i]) == false)
            {
                return false;
            }
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

    public bool DelItem(UserItemData itemData)
    {
        if(itemDic.ContainsKey(itemData.index) == false)
        {
            return false;
        }

        if(itemDic[itemData.index].count > itemData.count)
        {
            itemDic[itemData.index].count = itemDic[itemData.index].count - itemData.count;
        }
        else if(itemDic[itemData.index].count == itemData.count)
        {
            itemDic.Remove(itemData.index);
        }
        else
        {
            return false;
        }

        return true;
    }

    public bool DelItemList(List<UserItemData> delItemDataList)
    {
        for(int i = 0; i < delItemDataList.Count; i++)
        {
            if(DelItem(delItemDataList[i]) == false)
            {
                return false;
            }
        }

        return true;
    }

    #endregion
}
