﻿using LitJson;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public class PacketManager : MonoSingleton<PacketManager> {

    #region ServerConnect

    [DllImport("__Internal")]
    private static extern void Login();

    [DllImport("__Internal")]
    private static extern void SignUp();

    [DllImport("__Internal")]
    private static extern void Logout();

    [DllImport("__Internal")]
    private static extern void Gacha();

    [DllImport("__Internal")]
    private static extern void GetItem();

    [DllImport("__Internal")]
    private static extern void SetFormation(string formation);

    [DllImport("__Internal")]
    private static extern void GetStageInfo (int stage_num);

    [DllImport("__Internal")]
    private static extern void BattleAction (string battleAction);

    [DllImport("__Internal")]
    private static extern void StartBattle (int stage_num, int party_num);

    public bool receiveGacha = false;




    void Start()
    {
    }

    #endregion

    #region Request

    public void RequestLoginWithScatter()
    {
        Debug.Log("RequestLoginWithScatter");
        Login();
    }

    public void RequestGacha()
    {
        Debug.Log("RequestGacha");
        Gacha();
    }

    public void RequestSaveParty(Party partyInfo)
    {
        if(partyInfo == null)
        {
            Debug.Log("Invalid PartyInfo : " + partyInfo.partyIndex);
        }

        if(partyInfo.partyIndex == 0)
        {
            Debug.Log("Invalid PartyIndex : " + 0);
        }

        if(UserDataManager.Inst.partydic.ContainsKey(partyInfo.partyIndex) == false)
        {
            Debug.Log("NotEnough PartyIndex : " + partyInfo.partyIndex);
        }

        Debug.Log("RequestSaveParty");

        JsonParty data = new JsonParty();
        data.partyNum = partyInfo.partyIndex;

        for (int i=0; i< partyInfo.characterList.Count; ++i)
        {
            data.partyList.Add(partyInfo.characterList[i].index);
        }

        string json = JsonUtility.ToJson(data);
        Debug.Log("print Jsson : : " + json);

        SetFormation(json);
    }

    public void RequestGetStageInfo(int stageNum)
    {
        Debug.Log("RequsetGetStageInfo : " + stageNum);
        Response_GetStageInfo(stageNum);
        SceneManager.LoadScene("CharacterBattleScene");
        Debug.Log("Success BattleSceene Loading");
    }

    public void RequestBattleAction(int action_1,  int action_2, int target_1, int target_2)
    {
        Debug.Log("RequestBattleAction");

        List<JsonBattleAction> actionList = new List<JsonBattleAction>();
        actionList.Add(new JsonBattleAction(target_1, action_1));
        actionList.Add(new JsonBattleAction(target_2, action_2));

        string json = JsonUtility.ToJson(actionList);

        BattleAction(json);
        Debug.Log("Json action : " + json);
    }

    public void RequestLogout()
    {
        Debug.Log("RequestLogout");
        Logout();
        ResponseLogout();
    }

#endregion



#region Response

    public void ResponseLogin(string getLoginInfo)
    {
        // 
        if (getLoginInfo.StartsWith("{\"sign"))
        {
            SignUp();
            return;
        }
        
        // 
        UserLoginData userLoginData = JsonUtility.FromJson<UserLoginData>(getLoginInfo); 
        if(userLoginData == null)
        {
            Debug.Log("Invalid Login Data : " + getLoginInfo);
        }

        Login(userLoginData);
    }

    public void ResponseGetLobbyInfo()
    {
        Debug.Log("ResponseGetLobbyInfo");
    }

    public void ResponseGacha(string getGachaInfo)
    {
        Debug.Log("ResponseGacha : " + getGachaInfo);

        JsonData getInfo = JsonMapper.ToObject(getGachaInfo);
        int type = Convert.ToInt32(getInfo["result_type"].ToString());

        if (type == (int)GACHA_RESULT_TYPE.Servant)
        {
            // Debug.Log(getGachaInfo);

            gachaServantData gachaData = JsonUtility.FromJson<gachaServantData>(getGachaInfo);
            UserServantData getServant = ParseServant(gachaData.data.index, gachaData.data.servant);

            UserDataManager.Inst.AddServantInfo(getServant);

            GachaImage.Inst.SetServantGachaImage(getServant);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Monster)
        {
            // Debug.Log(getGachaInfo);

            gachaMonsterData gachaData = JsonUtility.FromJson<gachaMonsterData>(getGachaInfo);
            UserMonsterData getMonster = ParseMonster(gachaData.data.index, gachaData.data.monster);

            UserDataManager.Inst.SetMonsterInfo(getMonster);

            GachaImage.Inst.SetMonsterGachaImage(getMonster);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Item)
        {
            // Debug.Log(getGachaInfo);

            gachaItemData gachaData = JsonUtility.FromJson<gachaItemData>(getGachaInfo);
            Item getItem = ParseItem(gachaData.data.index, gachaData.data.item);

            UserDataManager.Inst.SetItemInfo(getItem);

            GachaImage.Inst.SetItemGachaImage(getItem);
        }
    }

    public void ResponseGetParty(string getPartyInfo)
    {
        Debug.Log("ResponseGetParty : " + getPartyInfo);
        partyData partyInfo = JsonUtility.FromJson<partyData>(getPartyInfo);

        Party getParty = ParseParty(partyInfo.index, partyInfo);
        if (UserDataManager.Inst.partydic.ContainsKey(getParty.partyIndex) == false)
        {
            UserDataManager.Inst.partydic.Add(getParty.partyIndex, getParty);
        }
        else
        {
            UserDataManager.Inst.partydic[getParty.partyIndex] = getParty;
        }
    }

    public void Response_GetStageInfo(int stageNum)
    {
        Debug.Log("Response_GetStageInfo");
        LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Stage);
    }

    public void ResponseLogout()
    {
        Debug.Log("ResponseLogout");
        LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Login);
    }

#endregion

#region Function

    public void Login(UserLoginData getUserLoginData)
    {
        UserInfo userInfo = new UserInfo();
        if (ParseUserInfo(getUserLoginData.userinfo, ref userInfo) == false)
        {
            Debug.Log("Invalid ParseUserInfo Info");
            // 재 로그인 시켜야함
        }
        UserDataManager.Inst.SetUserInfo(userInfo);
        LeftInfoPopup.Inst.SetLeftInfoUserInfoUpdate(userInfo);

        Dictionary<int, UserServantData> servantList = new Dictionary<int, UserServantData>();
        if (ParseServantList(getUserLoginData.servant_list, ref servantList) == false)
        {
            Debug.Log("Invalid ParseServantList Info");
            // 재 로그인 시켜야함
        }
        UserDataManager.Inst.SetServantDic(servantList);

        Dictionary<int, UserMonsterData> monsterList = new Dictionary<int, UserMonsterData>();
        if (ParseMonsterList(getUserLoginData.monster_list, ref monsterList) == false)
        {
            Debug.Log("Invalid ParseMonsterList Info");
            // 재 로그인 시켜야함
        }
        UserDataManager.Inst.SetMonsterDic(monsterList);



        
        Dictionary<int, Item> itemDic = new Dictionary<int, Item>();
        if (ParseItemList(getUserLoginData.item_list, ref itemDic) == false)
        {
            Debug.Log("Invalid ParseItemList Info");
            // 재 로그인 시켜야함
        }
        UserDataManager.Inst.SetItemDic(itemDic);

        Dictionary<int, Party> partyDic = new Dictionary<int, Party>();
        if (ParsePartyList(getUserLoginData.party_list, ref partyDic) == false)
        {
            Debug.Log("invalid ParsePartyList info");
            // 재 로그인 시켜야함
        }
        UserDataManager.Inst.SetPartyDic(partyDic);

        // 모든 데이터가 저장이 된 후 화면 전환
        LobbyManager.Inst.ChangeSceneState(userInfo.sceneState);
    }

    public bool ParseUserInfo(userData getUserData, ref UserInfo userInfo)
    {
        userInfo.userName = getUserData.user;
        userInfo.userEOS = 0;
        // TODO : EOS는 따로 구하는 코드 추가 필요?
        // userInfo.userEOS = ??
        userInfo.userMoney = getUserData.game_money;
        userInfo.sceneState = (SCENE_STATE)getUserData.state;

        Debug.Log("State : " + (SCENE_STATE)getUserData.state);

        userInfo.userHero = ParseServant(0, getUserData.hero);
        if (userInfo.userHero == null)
        {
            Debug.Log("Invalid UserHero Info");
            return false;
        }

        return true;
    }

    public bool ParseServantList(List<servantData> getServantList, ref Dictionary<int, UserServantData> servantList)
    {
        for (int i = 0; i < getServantList.Count; i++)
        {
            UserServantData servant = ParseServant(getServantList[i].index, getServantList[i].servant);
            if (servant == null)
            {
                Debug.Log("Invalid Servant Info");
                return false;
            }

            servantList.Add(servant.index, servant);
        }

        return true;
    }

    public UserServantData ParseServant(int getServantIndex, servantInfo getServantInfo)
    {
        if (getServantInfo == null)
        {
            return null;
        }

        UserServantData servant = new UserServantData();

        servant.jobNum = getServantInfo.job;

        servant.isMainHero = getServantInfo.isMainServant;

        servant.index = getServantIndex;
        servant.exp = getServantInfo.exp;
        // TODO : 추후 Servant Exp에 따른 Level 공식을 추가해 레벨 적용 필요
        servant.level = Calculator.GetLevelForExp(getServantInfo.exp);
        servant.jobNum = getServantInfo.job;

        // TODO : 추후 Appear의 값에 따른 리소스가 저장되어야함
        servant.headNum = getServantInfo.appear.head;
        servant.hairNum = getServantInfo.appear.hair;

        servant.status = ParseStatus(getServantInfo.status);

        //TODO : 임시 코드 
        CHARACTER_NUM servantNum = servant.jobNum + CHARACTER_NUM.Hero_Novice_1001;

        if (servant.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }
        

        return servant;
    }

    public Status ParseStatus(statusInfo getStatusInfo)
    {
        if (getStatusInfo == null)
        {
            return null;
        }

        Status status = new Status();

        status.basicStr = getStatusInfo.basic_str;
        status.basicDex = getStatusInfo.basic_dex;
        status.basicInt = getStatusInfo.basic_int;

        status.plusStr = getStatusInfo.plus_str;
        status.plusDex = getStatusInfo.plus_dex;
        status.plusInt = getStatusInfo.plus_int;

        return status;
    }

    public bool ParseMonsterList(List<monsterData> getMonsterList, ref Dictionary<int, UserMonsterData> monsterList)
    {
        for (int i = 0; i < getMonsterList.Count; i++)
        {
            UserMonsterData monster = ParseMonster(getMonsterList[i].index, getMonsterList[i].monster);
            if (monster == null)
            {
                Debug.Log("Invalid Monster Info");
                return false;
            }

            monsterList.Add(monster.index, monster);
        }

        return true;
    }

    public UserMonsterData ParseMonster(int getMonsterIndex, monsterInfo getMonsterInfo)
    {
        if (getMonsterInfo == null)
        {
            return null;
        }

        UserMonsterData monster = new UserMonsterData();

        monster.monsterNum = getMonsterInfo.look;

        monster.index = getMonsterIndex;
        monster.exp = getMonsterInfo.exp;
        // TODO : 추후 Servant Exp에 따른 Level 공식을 추가해 레벨 적용 필요
        monster.level = Calculator.GetLevelForExp(getMonsterInfo.exp);
        monster.monsterTypeNum = getMonsterInfo.type;
        monster.gradeNum = getMonsterInfo.grade;
        monster.enforceNum = getMonsterInfo.upgrade;
        

        return monster;
    }

    public bool ParseItemList(List<itemData> getItemList, ref Dictionary<int, Item> itemDic)
    {
        for (int i = 0; i < getItemList.Count; i++)
        {
            Item item = ParseItem(getItemList[i].index, getItemList[i].item);
            if (item == null)
            {
                Debug.Log("Invalid Item Info");
                return false;
            }
            itemDic.Add(item.index, item);
        }

        return true;
    }

    public Item ParseItem(int getItemIndex, itemInfo getItemInfo)
    {
        if (getItemInfo == null)
        {
            return null;
        }

        Item item = new Item();

        item.index = getItemIndex;

        item.state = getItemInfo.state;
        item.id = getItemInfo.id;
        item.slot = getItemInfo.slot;
        item.tier = getItemInfo.tier;
        item.job = getItemInfo.job;
        item.grade = getItemInfo.grade;
        item.upgrade = getItemInfo.upgrade;
        item.atk = getItemInfo.atk;
        item.def = getItemInfo.def;

        item.status = ParseStatus(getItemInfo.status);
        if (item.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }

        return item;
    }

    public bool ParsePartyList(List<partyData> getPartyList, ref Dictionary<int, Party> partyDic)
    {
        for (int i = 0; i < getPartyList.Count; i++)
        {
            Party party = ParseParty(getPartyList[i].index, getPartyList[i]);

            if (party == null)
            {
                Debug.Log("Invalid party Info");
                return false;
            }

            partyDic.Add(party.partyIndex, party);
        }

        return true;
    }

    public Party ParseParty(int getPartyIndex, partyData getParty)
    {
        if (getParty == null)
        {
            return null;
        }

        Party party = new Party();

        party.partyIndex = getPartyIndex;
        party.state = getParty.state;

        for (int i = 0; i < getParty.party.Count; ++i)
        {
            PartyCharacterInfo partyInfo = new PartyCharacterInfo();

            if (i == 0)
            {
                partyInfo.type = CHAR_TYPE.HERO;
            }
            else if (i < 5)
            {
                partyInfo.type = CHAR_TYPE.SERVANT;
            }
            else if (i < DEFINE.PARTY_MAX_NUM)
            {
                partyInfo.type = CHAR_TYPE.MONSTER;
            }
            else
            {
                return null;
            }

            partyInfo.partyPosition = i;
            partyInfo.index = getParty.party[i];

            party.characterList.Add(i, partyInfo);
        }

        return party;
    }

#endregion
}
