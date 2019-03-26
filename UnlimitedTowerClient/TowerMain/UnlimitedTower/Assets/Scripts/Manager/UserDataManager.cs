﻿using System.Collections.Generic;
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

    public int GetEtcItemCount()
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

    public List<UserItemData> GetEtcItemList()
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
            return false;
        }

        itemDic.Add(itemData.index, itemData);
        return true;
    }

    #endregion
}
