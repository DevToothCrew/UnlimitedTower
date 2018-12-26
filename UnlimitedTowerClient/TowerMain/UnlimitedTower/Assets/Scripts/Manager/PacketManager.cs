using LitJson;
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

    public void Request_ScatterLogin()
    {
        Debug.Log("Request_Login");

#if UNITY_EDITOR
        Response_Instant_Login();
#else
        Login();
#endif

    }

    public void Request_Gacha()
    {
        Debug.Log("Request_Gacha");
        Gacha();
    }

    public void Request_SaveParty(int partyNum)
    {
        Party party_info = new Party();
        party_info.partyIndex = partyNum;

        party_info = UserDataManager.Inst.partyDic[partyNum];

        JsonParty data = new JsonParty();
        data.partyNum = party_info.partyIndex;

        for (int i=0; i< party_info.characterList.Count; ++i)
        {
            data.partyList.Add(party_info.characterList[i].index);
        }            

        string json = JsonUtility.ToJson(data);
        Debug.Log("print Jsson : : " + json);

        SetFormation(json);
    }

    public void Request_GetStageInfo(int stageNum)
    {
        Debug.Log("Requset_GetStageInfo : " + stageNum);
        Response_GetStageInfo(stageNum);
        SceneManager.LoadScene("CharacterBattleScene");
        Debug.Log("Success BattleSceene Loading");
    }

    public void Request_BattleAction(int action_1,  int action_2, int target_1, int target_2)
    {
        Debug.Log("Request_BattleAction");

        List<JsonBattleAction> actionList = new List<JsonBattleAction>();
        actionList.Add(new JsonBattleAction(target_1, action_1));
        actionList.Add(new JsonBattleAction(target_2, action_2));

        string json = JsonUtility.ToJson(actionList);

        BattleAction(json);
        Debug.Log("Json action : " + json);
    }

    public void Request_Logout()
    {
        Debug.Log("Request_Logout");
        Logout();
        Response_Logout();
    }

#endregion



#region Response

    public void Response_Login(string getLoginInfo)
    {
        if (getLoginInfo.StartsWith("{\"sign"))
        {
            SignUp();
            return;
        }
        
        UserLoginData userLoginData = JsonUtility.FromJson<UserLoginData>(getLoginInfo); 
        if(userLoginData == null)
        {
            Debug.Log("Invalid Login Data : " + getLoginInfo);
        }

        Login(userLoginData);
    }

    public void Response_Instant_Login()
    {
        InstantLogin();
    }

    public void Response_GetLobbyInfo()
    {
        Debug.Log("Response_GetLobbyInfo");
    }

    public void Response_Gacha(string getGachaInfo)
    {
        Debug.Log("Response_Gacha : " + getGachaInfo);

        JsonData getInfo = JsonMapper.ToObject(getGachaInfo);
        int type = Convert.ToInt32(getInfo["result_type"].ToString());

        if (type == (int)GACHA_RESULT_TYPE.Servant)
        {
            // Debug.Log(getGachaInfo);

            gachaServantData gachaData = JsonUtility.FromJson<gachaServantData>(getGachaInfo);
            Servant getServant = ParseServant(gachaData.data.index, gachaData.data.servant);

            UserDataManager.Inst.SetServantInfo(getServant);

            GachaImage.Inst.SetServantGachaImage(getServant);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Monster)
        {
            // Debug.Log(getGachaInfo);

            gachaMonsterData gachaData = JsonUtility.FromJson<gachaMonsterData>(getGachaInfo);
            Monster getMonster = ParseMonster(gachaData.data.index, gachaData.data.monster);

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

    public void Response_GetParty(string getPartyInfo)
    {
        Debug.Log("Response_GetParty : " + getPartyInfo);
        partyData partyInfo = JsonUtility.FromJson<partyData>(getPartyInfo);

        Party getParty = ParseParty(partyInfo.index, partyInfo);
        if (UserDataManager.Inst.partyDic.ContainsKey(getParty.partyIndex) == false)
        {
            UserDataManager.Inst.partyDic.Add(getParty.partyIndex, getParty);
        }
        else
        {
            UserDataManager.Inst.partyDic[getParty.partyIndex] = getParty;
        }
    }

    public void Response_GetBattle(string battle_info)
    {
        Debug.Log("Response_GetBattle : " + battle_info);
    }

    public void Response_SaveFormation()
    {
        Debug.Log("Response_SaveFormation");
    }

    public void Response_GetStageInfo(int stageNum)
    {
        Debug.Log("Response_GetStageInfo");
        LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Stage);
    }

    public void Response_Logout()
    {
        Debug.Log("Response_Logout");
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

        Dictionary<int, Servant> servantDic = new Dictionary<int, Servant>();
        if (ParseServantList(getUserLoginData.servant_list, ref servantDic) == false)
        {
            Debug.Log("Invalid ParseServantList Info");
            // 재 로그인 시켜야함
        }
        UserDataManager.Inst.SetServantDic(servantDic);

        Dictionary<int, Monster> monsterDic = new Dictionary<int, Monster>();
        if (ParseMonsterList(getUserLoginData.monster_list, ref monsterDic) == false)
        {
            Debug.Log("Invalid ParseMonsterList Info");
            // 재 로그인 시켜야함
        }
        UserDataManager.Inst.SetMonsterDic(monsterDic);

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

    public void InstantLogin()
    {
        UserInfo userInfo = new UserInfo();
        userInfo.userName = "devtooth";
        userInfo.userMoney = 0;
        userInfo.userEOS = 0;
        userInfo.sceneState = SCENE_STATE.Lobby;
        userInfo.userHero = Cheat.Inst.GetRandomServant();
        UserDataManager.Inst.SetUserInfo(userInfo);

        Dictionary<int, Servant> servantDic = new Dictionary<int, Servant>();
        UserDataManager.Inst.SetServantDic(servantDic);

        Dictionary<int, Monster> monsterDic = new Dictionary<int, Monster>();
        UserDataManager.Inst.SetMonsterDic(monsterDic);

        Dictionary<int, Item> itemDic = new Dictionary<int, Item>();
        UserDataManager.Inst.SetItemDic(itemDic);

        Dictionary<int, Party> partyDic = new Dictionary<int, Party>();
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

        userInfo.userHero = ParseServant(0, getUserData.hero);
        if (userInfo.userHero == null)
        {
            Debug.Log("Invalid UserHero Info");
            return false;
        }

        return true;
    }

    public bool ParseServantList(List<servantData> getServantList, ref Dictionary<int, Servant> servantDic)
    {
        for (int i = 0; i < getServantList.Count; i++)
        {
            Servant servant = ParseServant(getServantList[i].index, getServantList[i].servant);
            if (servant == null)
            {
                Debug.Log("Invalid Servant Info");
                return false;
            }

            servantDic.Add(servant.index, servant);
        }

        return true;
    }

    public Servant ParseServant(int getServantIndex, servantInfo getServantInfo)
    {
        if (getServantInfo == null)
        {
            return null;
        }

        Servant servant = new Servant();

        servant.job = getServantInfo.job;

        servant.index = getServantIndex;
        servant.state = getServantInfo.state;
        servant.exp = getServantInfo.exp;
        // TODO : 추후 Servant Exp에 따른 Level 공식을 추가해 레벨 적용 필요
        servant.level = DEFINE.GetLevelForExp(getServantInfo.exp);
        servant.job = getServantInfo.job;

        // TODO : 추후 Appear의 값에 따른 리소스가 저장되어야함
        servant.head = getServantInfo.appear.head;
        servant.hair = getServantInfo.appear.hair;
        servant.body = getServantInfo.appear.body;

        servant.status = ParseStatus(getServantInfo.status);

        //TODO : 임시 코드 
        CHARACTER_NUM servantNum = servant.job + CHARACTER_NUM.Hero_Novice_1001;
        servant.name = servantNum.ToString();

        if (servant.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }

        servant.equipmentList = getServantInfo.equip_slot;

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

    public bool ParseMonsterList(List<monsterData> getMonsterList, ref Dictionary<int, Monster> monsterDic)
    {
        for (int i = 0; i < getMonsterList.Count; i++)
        {
            Monster monster = ParseMonster(getMonsterList[i].index, getMonsterList[i].monster);
            if (monster == null)
            {
                Debug.Log("Invalid Monster Info");
                return false;
            }

            monsterDic.Add(monster.index, monster);
        }

        return true;
    }

    public Monster ParseMonster(int getMonsterIndex, monsterInfo getMonsterInfo)
    {
        if (getMonsterInfo == null)
        {
            return null;
        }

        Monster monster = new Monster();

        monster.look = getMonsterInfo.look;

        monster.index = getMonsterIndex;
        monster.state = getMonsterInfo.state;
        monster.exp = getMonsterInfo.exp;
        // TODO : 추후 Servant Exp에 따른 Level 공식을 추가해 레벨 적용 필요
        monster.level = DEFINE.GetLevelForExp(getMonsterInfo.exp);
        monster.type = getMonsterInfo.type;
        monster.grade = getMonsterInfo.grade;
        monster.upgrade = getMonsterInfo.upgrade;

        // TODO : 추후 Appear + Type 의 값에 따른 리소스가 저장되어야함
        // monster.appear = ???

        monster.status = ParseStatus(getMonsterInfo.status);

        //TODO : 임시 코드 
        CHARACTER_NUM monsterNum = monster.look + CHARACTER_NUM.Mst_Cat;
        monster.name = monsterNum.ToString();

        if (monster.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }

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

        for (int i = 0; i < getParty.partyList.Count; ++i)
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
            partyInfo.index = getParty.partyList[i];

            party.characterList.Add(i, partyInfo);
        }

        return party;
    }

#endregion
}
