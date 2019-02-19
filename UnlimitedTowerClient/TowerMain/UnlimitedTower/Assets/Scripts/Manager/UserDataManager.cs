using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataManager : MonoSingleton<UserDataManager>
{
    public UserInfo userInfo = new UserInfo();

    public Dictionary<int, UserServantData> servantDic = new Dictionary<int, UserServantData>();
    // 사실 Dic가 있으면 그냥 toList로 불러 올수있음, 그리고 왜 이걸 대문자로 받아서 하지?

    public Dictionary<int, UserMonsterData> monsterDic = new Dictionary<int, UserMonsterData>();

    // 장비와 기타 아이템 소모품 등은 Equipment와 Item으로 변경 필요
    public Dictionary<int, UserMountItemData> mountItemDic = new Dictionary<int, UserMountItemData>();

    public Dictionary<int, UserEtcItemData> etcItemDic = new Dictionary<int, UserEtcItemData>();
    
    public Dictionary<int, UserPartyData> partyDic = new Dictionary<int, UserPartyData>();
    
    public Dictionary<int, UserFormationData> formationDic = new Dictionary<int, UserFormationData>();

    public int usingPartyNum = 1;

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

    public void SetFormationDic(Dictionary<int, UserFormationData> userInformationDic)
    {
        formationDic = userInformationDic;
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

    public UserPartyData GetUserPartyInfo(int partyNum)
    {
        if(partyDic.ContainsKey(partyNum) == false)
        {
            Debug.Log("Invalid PartyNum");
            return null;
        }

        return partyDic[partyNum];
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
    public UserMonsterData GetMonsterInfo(int index)
    {
        if (monsterDic.ContainsKey(index) == false)
        {
            Debug.Log("Invalid GetMonsterInfo : " + index);
            return null;
        }

        return monsterDic[index];
    }

    // 이름이 병신같음
    public UserFormationData GetFormaData_nullPossible(int team, int formationIndex)
    {
        // TODO : 이런식으로 쓸꺼면 team을 Dictionary의 Key로 사용

        if(formationDic.ContainsKey(team) == true)
        {
            if(formationDic[team].formationIndex == formationIndex)
            {
                return formationDic[team];
            }
        }

        return null;
    }

    public UserFormationData GetFormationData(int index)
    {
        if(formationDic.ContainsKey(index) == false)
        {
            return null;
        }

        return formationDic[index];
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
