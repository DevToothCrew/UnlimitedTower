using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDataManager : MonoSingleton<UserDataManager> {

    public UserInfo userInfo = new UserInfo();
    public Dictionary<int, Servant> servantDic = new Dictionary<int, Servant>();
    public Dictionary<int, Monster> monsterDic = new Dictionary<int, Monster>();
    public Dictionary<int, Item> itemDic = new Dictionary<int, Item>();
    public Dictionary<int, Party> partyDic = new Dictionary<int, Party>();

    public void Awake()
    {

    }

    #region SetFunction

    public void SetUserInfo(UserInfo getUserInfo)
    {
        userInfo = getUserInfo;
    }

    public void SetServantDic(Dictionary<int, Servant> getServantDic)
    {
        servantDic = getServantDic;
    }

    public void SetMonsterDic(Dictionary<int, Monster> getMonsterDic)
    {
        monsterDic = getMonsterDic;
    }

    public void SetItemDic(Dictionary<int, Item> getItemDic)
    {
        itemDic = getItemDic;
    }

    public void SetPartyDic(Dictionary<int, Party> getPartyDic)
    {
        partyDic = getPartyDic;
    }

    public void SetServantInfo(Servant getServant)
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

    public void SetMonsterInfo(Monster getMonster)
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

    #endregion
}



