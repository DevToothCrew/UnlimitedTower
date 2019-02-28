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
    private static extern void StartBattle (string battleStart);

    [DllImport("__Internal")]
    private static extern void GetReward();

    public bool receiveGacha = false;

    void Start()
    {
    }

    #endregion

    #region Request

    public void RequestLoginWithScatter()
    {
        UTLoadingManager.Description desc = new UTLoadingManager.Description
        {
            startComment = "Try to login ...",
            finishedComment = "Success!",
            predicate = () => UserDataManager.Inst.userInfo != default(UserInfo),
        };

        UTLoadingManager.Instance.BeginScene(desc);

        Debug.Log("RequestLoginWithScatter");
        Login();
    }

    public void RequestLogout()
    {
        Debug.Log("RequestLogout");
        Logout();
        ResponseLogout();
    }

    public void RequestGacha()
    {
        Debug.Log("RequestGacha");
        Gacha();
    }

    // TODO : 수정 필요
    public void RequestSaveParty(UserPartyData partyInfo)
    {
        if(partyInfo == null)
        {
            Debug.Log("Invalid PartyInfo");
        }

        Debug.Log("RequestSaveParty");

        TestParty data = new TestParty();
        data.partyNum = partyInfo.partyIndex;
        for(int i=0; i< partyInfo.formationDataDic.Count; ++i)
        {
            //비어있는 경우 index는 0 이다.
            //파티 하나 고정 히어로도 고정이기 때문에 히어로에 대한 처리는 안해준다.
            if (partyInfo.formationDataDic[i].formationIndex == 0)
            {
                continue;
            }
            else if(partyInfo.formationDataDic[i].formationIndex <= 4)
            {
                data.servantList.Add(partyInfo.formationDataDic[i].index);
            }
            else
            {
                data.monsterList.Add(partyInfo.formationDataDic[i].index);
            }
        }

        //JsonParty data = new JsonParty();
        //data.partyNum = partyInfo.partyIndex;

        //for (int i=0; i< partyInfo.characterList.Count; ++i)
        //{
        //    data.partyList.Add(partyInfo.characterList[i].index);
        //}

        string json = JsonUtility.ToJson(data);
        Debug.Log("print Jsson : : " + json);

        SetFormation(json);
    }

    public void RequestBattleAction(int heroTarget, int heroAction, int monsterTarget, int monsterAction)
    {
        Debug.Log("RequestBattleAction");

        //List<JsonBattleAction> actionList = new List<JsonBattleAction>();
        //actionList.Add(new JsonBattleAction(heroTarget, heroAction));
        //actionList.Add(new JsonBattleAction(monsterTarget, monsterAction));

        //string json = JsonUtility.ToJson(actionList);
        TestJsonBattleAction action = new TestJsonBattleAction();
        action.heroTargetIndex = heroTarget;
        action.heroActionType = heroAction;
        action.monsterTargetIndex = monsterTarget;
        action.monsterActionType = monsterAction;


        string json = JsonUtility.ToJson(action);

        Debug.Log("Json action : " + json);
        BattleAction(json);
        
    }

    public void RequestStageStart(int stageNum, int partyNum)
    {
        Debug.Log("Request Start Battle");

        TestJsonStartBattle startBattle = new TestJsonStartBattle();
        startBattle.stageNum = stageNum;
        startBattle.partyNum = partyNum;

        string json = JsonUtility.ToJson(startBattle);

        Debug.Log("Json start : " + json);
        StartBattle(json);
    }

    public void RequestStageResult()
    {
        Debug.Log("Request Get Battle Reward");
        GetReward();
    }

    public void RequestTowerStart(int towerFloor, int partyNum)
    {

    }

    public void RequestTowerResult(int towerFloor)
    {

    }

    #endregion



    #region Response

    public void ResponseLogin(string getLoginInfo)
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

    public void ResponseGacha(string getGachaInfo)
    {
        Debug.Log("ResponseGacha : " + getGachaInfo);

        JsonData getInfo = JsonMapper.ToObject(getGachaInfo);
        int type = Convert.ToInt32(getInfo["result_type"].ToString());

        if (type == (int)GACHA_RESULT_TYPE.Servant)
        {
            // Debug.Log(getGachaInfo);

            gachaServantData gachaData = JsonUtility.FromJson<gachaServantData>(getGachaInfo);
            UserServantData getServant = ParseServant(gachaData.data.index, gachaData.data.party_number, gachaData.data.servant);

            UserDataManager.Inst.AddServantData(getServant);

            GachaImage.Inst.SetServantGachaImage(getServant);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Monster)
        {
            // Debug.Log(getGachaInfo);

            gachaMonsterData gachaData = JsonUtility.FromJson<gachaMonsterData>(getGachaInfo);
            UserMonsterData getMonster = ParseMonster(gachaData.data.index, gachaData.data.party_number, gachaData.data.monster);

            UserDataManager.Inst.AddMonsterData(getMonster);

            GachaImage.Inst.SetMonsterGachaImage(getMonster);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Item)
        {
            // Debug.Log(getGachaInfo);

            gachaItemData gachaData = JsonUtility.FromJson<gachaItemData>(getGachaInfo);
            UserMountItemData getItem = ParseItem(gachaData.data.index, gachaData.data.item);

            UserDataManager.Inst.AddMountItemData(getItem);

            GachaImage.Inst.SetItemGachaImage(getItem);
        }
    }

    public void ResponseLogout()
    {
        Debug.Log("ResponseLogout");
        UserDataManager.Inst.InitUserInfo();
        SceneManager.LoadScene("Login");
    }

    public void ResponseBattleAction(string getBattleActionInfo)
    {
        TestStageActionInfoData actionData = JsonUtility.FromJson<TestStageActionInfoData>(getBattleActionInfo);
        if (actionData == null)
        {
            Debug.Log("Invalid ResponseBattleAction Data : " + getBattleActionInfo);
        }
        UpdateAction(actionData);
    }

    public void ResponseStageStart(string getStageStartInfo)
    {
        TestStageStateData stateData = JsonUtility.FromJson<TestStageStateData>(getStageStartInfo);
        if (stateData == null)
        {
            Debug.Log("Invalid ResponseStageStart Data : " + getStageStartInfo);
        }
        BattleStart(stateData);
    }

    public void ResponseStageResult(string getStageResultInfo)
    {
        TestStageRewardData resultData = JsonUtility.FromJson<TestStageRewardData>(getStageResultInfo);
        if(resultData == null)
        {
            Debug.Log("Invalid ResponseStageResult Data : " + getStageResultInfo);
        }
        GetReward(resultData);
    }

    public void ResponseTowerStart(string getTowerStartInfo)
    {

    }

    public void ResponseTowerResult(string getTowerResultInfo)
    {

    }

    #endregion

    #region Function

    public void Login(UserLoginData getUserLoginData)
    {
        UserInfo userInfo = new UserInfo();
        if (ParseUserInfo(getUserLoginData.userinfo, ref userInfo) == false)
        {
            Debug.Log("Invalid ParseUserInfo Info");
        }
        ParseGoldInfo(getUserLoginData.token, ref userInfo);

        UserDataManager.Inst.SetUserInfo(userInfo);

        Dictionary<int, UserServantData> servantDic = new Dictionary<int, UserServantData>();
        if (ParseServantDic(getUserLoginData.servant_list, ref servantDic) == false)
        {
            Debug.Log("Invalid ParseServantList Info");
        }
        UserDataManager.Inst.SetServantDic(servantDic);

        Dictionary<int, UserMonsterData> monsterDic = new Dictionary<int, UserMonsterData>();
        if (ParseMonsterDic(getUserLoginData.monster_list, ref monsterDic) == false)
        {
            Debug.Log("Invalid ParseMonsterList Info");
        }
        UserDataManager.Inst.SetMonsterDic(monsterDic);

        Dictionary<int, UserMountItemData> itemDic = new Dictionary<int, UserMountItemData>();
        if (ParseItemDic(getUserLoginData.item_list, ref itemDic) == false)
        {
            Debug.Log("Invalid ParseItemList Info");
        }
        UserDataManager.Inst.SetItemDic(itemDic);

        UserPartyData partyInfo = ParsePartyInfo(getUserLoginData.party_info);
        if (partyInfo == null)
        {
            Debug.Log("invalid ParsePartyList info");
        }
        UserDataManager.Inst.SetPartyInfo(partyInfo);

        SceneManager.LoadScene("Lobby");
    }

    public bool ParseUserInfo(userData getUserData, ref UserInfo userInfo)
    {
        userInfo.userName = getUserData.user;
        userInfo.userEOS = 0;

        userInfo.sceneState = (SCENE_STATE)getUserData.state;

        Debug.Log("State : " + (SCENE_STATE)getUserData.state);

        userInfo.userHero = ParseServant(0, 1, getUserData.hero);
        if (userInfo.userHero == null)
        {
            Debug.Log("Invalid UserHero Info");
            return false;
        }

        return true;
    }

    public bool ParseGoldInfo(goldData getgoldData, ref UserInfo userInfo)
    {
        char[] splitchar = { ' ' };
        char[] splitmoney = { '.' };

        // TODO : EOS도 여기다 넣어야 하는지 생각 필요

        string[] token = getgoldData.balance.Split(splitchar);
        string[] money = token[0].Split(splitmoney);

        userInfo.userMoney = Int32.Parse(money[0]);

        Debug.Log("Gold : " + userInfo.userMoney);

        return true;
    }

    public bool ParseServantDic(List<servantData> getServantList, ref Dictionary<int, UserServantData> servantDic)
    {
        for (int i = 0; i < getServantList.Count; i++)
        {
            UserServantData servant = ParseServant(getServantList[i].index, getServantList[i].party_number, getServantList[i].servant);
            if (servant == null)
            {
                Debug.Log("Invalid Servant Info");
                return false;
            }

            servantDic.Add(servant.index, servant);
        }

        return true;
    }

    public UserServantData ParseServant(int getServantIndex, int getPartyNum, servantInfo getServantInfo)
    {
        if (getServantInfo == null)
        {
            return null;
        }

        UserServantData userServant = new UserServantData();

        userServant.index = getServantIndex;

        userServant.jobNum = getServantInfo.job;
        userServant.exp = getServantInfo.exp;

        userServant.body = getServantInfo.appear.body;
        userServant.headNum = getServantInfo.appear.head;
        userServant.hairNum = getServantInfo.appear.hair;
        userServant.gender = getServantInfo.appear.gender;

        userServant.partyNum = getPartyNum;

        userServant.status = ParseStatus(getServantInfo.status);
        if (userServant.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }


        return userServant;
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

    public bool ParseMonsterDic(List<monsterData> getMonsterList, ref Dictionary<int, UserMonsterData> monsterDic)
    {
        for (int i = 0; i < getMonsterList.Count; i++)
        {
            UserMonsterData monster = ParseMonster(getMonsterList[i].index, getMonsterList[i].party_number, getMonsterList[i].monster);
            if (monster == null)
            {
                Debug.Log("Invalid Monster Info");
                return false;
            }

            monsterDic.Add(monster.index, monster);
        }

        return true;
    }
    
    public UserMonsterData ParseMonster(int getMonsterIndex, int getPartyNum, monsterInfo getMonsterInfo)
    {
        if (getMonsterInfo == null)
        {
            return null;
        }

        UserMonsterData monster = new UserMonsterData();

        monster.index = getMonsterIndex;

        monster.id = getMonsterInfo.id;

        monster.exp = getMonsterInfo.exp;

        monster.monsterTypeNum = getMonsterInfo.type;

        monster.gradeNum = getMonsterInfo.grade;
        monster.enforceNum = getMonsterInfo.upgrade;

        monster.teamNum = getPartyNum;

        //몬스터 스테이터스도 서버로 부터 받아 올 거기 때문에 추가
        monster.status = ParseStatus(getMonsterInfo.status);
        if (monster.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }

        return monster;
    }

    public bool ParseItemDic(List<itemData> getItemList, ref Dictionary<int, UserMountItemData> itemDic)
    {
        for (int i = 0; i < getItemList.Count; i++)
        {
            UserMountItemData item = ParseItem(getItemList[i].index, getItemList[i].item);
            if (item == null)
            {
                Debug.Log("Invalid Item Info");
                return false;
            }
            itemDic.Add(item.index, item);
        }

        return true;
    }

    public UserMountItemData ParseItem(int getItemIndex, itemInfo getItemInfo)
    {
        if (getItemInfo == null)
        {
            return null;
        }

        UserMountItemData item = new UserMountItemData();

        item.index = getItemIndex;
        item.tierNum = getItemInfo.tier;
        item.upgradeCount = getItemInfo.upgrade;

        return item;
    }

    public UserPartyData ParsePartyInfo(partyData getPartyData)
    {
        if (getPartyData == null)
        {
            return null;
        }

        UserPartyData partyInfo = new UserPartyData();
        partyInfo.partyIndex = getPartyData.index;
        partyInfo.partyState = getPartyData.state;

        UserFormationData heroFormationData = new UserFormationData();
        heroFormationData.index = 0;
        heroFormationData.formationIndex = 0;
        partyInfo.formationDataDic.Add(0, heroFormationData);

        for(int i = 0; i < getPartyData.servant_list.Count; i++)
        {
            if(getPartyData.servant_list[i] > 0)
            {
                UserFormationData servantFormationData = new UserFormationData();
                servantFormationData.index = getPartyData.servant_list[i];
                servantFormationData.formationIndex = i + 1;
                partyInfo.formationDataDic.Add(servantFormationData.formationIndex, servantFormationData);
            }
        }

        for (int i = 0; i < getPartyData.monster_list.Count; i++)
        {
            if (getPartyData.monster_list[i] > 0)
            {
                UserFormationData monsterFormationData = new UserFormationData();
                monsterFormationData.index = getPartyData.monster_list[i];
                monsterFormationData.formationIndex = i + 5;
                partyInfo.formationDataDic.Add(monsterFormationData.formationIndex, monsterFormationData);
            }
        }

        return partyInfo;
    }
    
    public void BattleStart(TestStageStateData getBattleStateData)
    {
        Debug.Log("배틀 스타트!");
        UserDataManager.Inst.SetStageState(getBattleStateData);
        SceneManager.LoadScene("CharacterBattleScene");
    }

    public void UpdateAction(TestStageActionInfoData getBattleActionData)
    {
        Debug.Log("턴 진행!");
        UserDataManager.Inst.SetStageAction(getBattleActionData);
        BattleSystem.Inst.StartCoroutine(BattleSystem.Inst.BattleStart());
    }

    public void GetReward(TestStageRewardData getReward)
    {
        Debug.Log("배틀 끝 보상 획득!");
        UserDataManager.Inst.SetStageReward(getReward);
    }

    #endregion
}

