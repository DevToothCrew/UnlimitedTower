using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoSingleton<UserDataManager>
{
    public UserInfo userInfo = new UserInfo();

    public Dictionary<int, UserServantData> servantDic = new Dictionary<int, UserServantData>();
    public Dictionary<int, UserMonsterData> monsterDic = new Dictionary<int, UserMonsterData>();
    public Dictionary<int, UserMountItemData> mountitemDic = new Dictionary<int, UserMountItemData>();
    public Dictionary<int, UserEtcItemData> etcitemDic = new Dictionary<int, UserEtcItemData>();

    public Dictionary<int, UserPartyData> partyDic = new Dictionary<int, UserPartyData>();

    public Dictionary<int, Item> itemDic = new Dictionary<int, Item>();
    public Dictionary<int, Party> partydic = new Dictionary<int, Party>();

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

    public void SetItemDic(Dictionary<int, Item> getItemDic)
    {
        itemDic = getItemDic;
    }

    public void SetPartyDic(Dictionary<int, Party> getPartyDic)
    {
        partydic = getPartyDic;
    }

    public void AddServantInfo(UserServantData getServant)
    {
        if (servantDic.ContainsKey(getServant.index) == false)
        {
            servantDic.Add(getServant.index, getServant);
        }
        else
        {
            Debug.Log("Invalid SetServant Info : " + getServant.index);
        }
    }

    public void SetMonsterInfo(UserMonsterData getMonster)
    {
        if (monsterDic.ContainsKey(getMonster.index) == false)
        {
            monsterDic.Add(getMonster.index, getMonster);
        }
        else
        {
            Debug.Log("Invalid SetMonster Info : " + getMonster.index);
        }
    }

    public void SetItemInfo(Item getItem)
    {
        if (itemDic.ContainsKey(getItem.index) == false)
        {
            itemDic.Add(getItem.index, getItem);
        }
        else
        {
            Debug.Log("Invalid SetItem Info : " + getItem.index);
        }
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

    public Item GetItemInfo(int index)
    {
        if (itemDic.ContainsKey(index) == false)
        {
            Debug.Log("Invalid GetItemInfo : " + index);
            return null;
        }

        return itemDic[index];
    }

    #endregion
}