using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataManager : MonoSingleton<UserDataManager>
{
    public UserInfo userInfo = new UserInfo();

    public Dictionary<int, UserServantData> servantDic = new Dictionary<int, UserServantData>();
   
    public Dictionary<int, UserMonsterData> monsterDic = new Dictionary<int, UserMonsterData>();

    // 장비와 기타 아이템 소모품 등은 Equipment와 Item으로 변경 필요
    public Dictionary<int, UserMountItemData> mountItemDic = new Dictionary<int, UserMountItemData>();

    public Dictionary<int, UserEtcItemData> etcItemDic = new Dictionary<int, UserEtcItemData>();
    
    // 현재 파티는 1개, 파티 안에 Formation Info 포함
    public UserPartyData partyInfo = new UserPartyData();

    // TODO : Test용
    //public TestbattleStateData stageState = new TestbattleStateData();
    public TestStageStateData StageStateData =  new TestStageStateData();
    public TestStageActionInfoData StageActionInfoData = new TestStageActionInfoData();
    public TestStageRewardData StageRewardData = new TestStageRewardData();

    public int usingPartyNum = 1;

    #region InitFunction

    public void InitUserInfo()
    {
        userInfo = new UserInfo();
        servantDic = new Dictionary<int, UserServantData>();
        monsterDic = new Dictionary<int, UserMonsterData>();
        mountItemDic = new Dictionary<int, UserMountItemData>();
        etcItemDic = new Dictionary<int, UserEtcItemData>();
        partyInfo = new UserPartyData();
        stageState = new TestbattleStateData();
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

    public void SetItemDic(Dictionary<int, UserMountItemData> getItemDic)
    {
        mountItemDic = getItemDic;
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

    //public void SetStageState(TestbattleStateData testStageState)
    //{
    //    stageState = testStageState;
    //}

    public void SetStageState(TestStageStateData testStageState)
    {
        StageStateData = testStageState;
    }

    public void SetStageAction(TestStageActionInfoData testStageState)
    {
        StageActionInfoData = testStageState;
    }

    public void SetStageReward(TestStageRewardData testStageState)
    {
        StageRewardData = testStageState;
    }

    #endregion

    #region GetFunction

    public SCENE_STATE GetSceneState()
    {
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

    public int GetMountItemCount()
    {
        return mountItemDic.Count;
    }

    public int GetEtcItemCount()
    {
        return etcItemDic.Count;
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

    public List<UserMountItemData> GetMountItemList()
    {
        if(mountItemDic.Count == 0)
        {
            return null;
        }

        return mountItemDic.Values.ToList();
    }

    public List<UserEtcItemData> GetEtcItemList()
    {
        if(etcItemDic.Count == 0)
        {
            return null;
        }

        return etcItemDic.Values.ToList();
    }

    //public TestbattleStateData GetStageState()
    //{
    //    if(stageState == null)
    //    {
    //        Debug.LogError("버그");
    //        return null;
    //    }

    //    return stageState;
    //}

    public TestStageStateData GetStageState()
    {
        if (StageStateData == null)
        {
            Debug.LogError("버그");
            return null;
        }

        return StageStateData;
    }

    public TestStageActionInfoData GetStageAction()
    {
        if (StageActionInfoData == null)
        {
            Debug.LogError("버그");
            return null;
        }

        return StageActionInfoData;
    }

    public TestStageRewardData GetStageReward()
    {
        if (StageRewardData == null)
        {
            Debug.LogError("버그");
            return null;
        }

        return StageRewardData;
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

    public bool AddMountItemData(UserMountItemData itemData)
    {
        if (mountItemDic.ContainsKey(itemData.index) == true)
        {
            return false;
        }

        mountItemDic.Add(itemData.index, itemData);
        return true;
    }

    #endregion
}
