using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoSingleton<UserDataManager>
{
    public UserInfo userInfo = new UserInfo();

    public Dictionary<int, UserServantData> servantDic = new Dictionary<int, UserServantData>();
    public List<UserServantData> ServantList = new List<UserServantData>();

    public Dictionary<int, UserMonsterData> monsterDic = new Dictionary<int, UserMonsterData>();
    public List<UserMonsterData> MonsterList = new List<UserMonsterData>();

    public Dictionary<int, UserMountItemData> mountitemDic = new Dictionary<int, UserMountItemData>();
    public List<UserMountItemData> MountItemList = new List<UserMountItemData>();

    public Dictionary<int, UserEtcItemData> etcitemDic = new Dictionary<int, UserEtcItemData>();
    public List<UserEtcItemData> EtcItemList = new List<UserEtcItemData>();

    public List<UserFormationData> UserFormationList = new List<UserFormationData>();
    public Dictionary<int, UserPartyData> partyDic = new Dictionary<int, UserPartyData>();
    
    public int usingPartyNum = 1;



    // Add Remove
    public void addServantData(UserServantData getServant)
    {
        if (servantDic.ContainsKey(getServant.index) == false)
        {
            servantDic.Add(getServant.index, getServant);
            ServantList.Add(getServant);
        }
        else
        {
            Debug.Log("Invalid SetServant Info : " + getServant.index);
        }
    }
    public void removeServantData(int index)
    {
        if (servantDic.ContainsKey(index))
        {
            UserServantData servData = servantDic[index];
            servantDic.Remove(index);

            if (ServantList.Contains(servData))
            {
                ServantList.Remove(servData);
            }
        }
    }

    public void addMonsterData(UserMonsterData getMonster)
    {
        if (monsterDic.ContainsKey(getMonster.index) == false)
        {
            monsterDic.Add(getMonster.index, getMonster);
            MonsterList.Add(getMonster);
        }
        else
        {
            Debug.Log("Invalid SetMonster Info : " + getMonster.index);
        }
    }
    public void removeMonsterData(int index)
    {
        if (monsterDic.ContainsKey(index))
        {
            UserMonsterData servData = monsterDic[index];
            monsterDic.Remove(index);

            if (MonsterList.Contains(servData))
            {
                MonsterList.Remove(servData);
            }
        }
    }

    public void addMountitemData(UserMountItemData mountitemdata)
    {
        MountItemList.Add(mountitemdata);
        mountitemDic.Add(mountitemdata.index, mountitemdata);
    }
    public void removeMountiemData(UserMountItemData mountitemdata)
    {
        // list
        if (MountItemList.Contains(mountitemdata))
        {
            MountItemList.Remove(mountitemdata);
        }
        // dictionary
        if (mountitemDic.ContainsKey(mountitemdata.index))
        {
            mountitemDic.Remove(mountitemdata.index);
        }

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






    // get
    public UserFormationData getFormaData_nullPossible(int team, int formindex)
    {
        return UserFormationList.Find((rowdata) => { return rowdata.partyIndex == team && rowdata.formationIndex == formindex; });
    }
    #region SetFunction

    public void SetUserInfo(UserInfo getUserInfo)
    {
        userInfo = getUserInfo;
    }
    public void SetMonsterDic(Dictionary<int, UserMonsterData> getMonsterDic)
    {
        monsterDic = getMonsterDic;
    }
    
    
    public void SetSceneState(SCENE_STATE state)
    {
        userInfo.sceneState = state;
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
    
    #endregion
}
