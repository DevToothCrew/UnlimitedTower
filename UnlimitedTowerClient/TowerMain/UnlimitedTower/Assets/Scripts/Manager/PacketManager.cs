using LitJson;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;


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

        //JsonParty data = new JsonParty();
        //data.partyNum = partyInfo.partyIndex;

        //for (int i=0; i< partyInfo.characterList.Count; ++i)
        //{
        //    data.partyList.Add(partyInfo.characterList[i].index);
        //}

        string json = JsonUtility.ToJson(" ");
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
            UserServantData getServant = ParseServant(gachaData.data.index, gachaData.data.servant);

            UserDataManager.Inst.AddServantData(getServant);

            GachaImage.Inst.SetServantGachaImage(getServant);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Monster)
        {
            // Debug.Log(getGachaInfo);

            gachaMonsterData gachaData = JsonUtility.FromJson<gachaMonsterData>(getGachaInfo);
            UserMonsterData getMonster = ParseMonster(gachaData.data.index, gachaData.data.monster);

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

    public void ResponseGetParty(string getPartyInfo)
    {
        Debug.Log("ResponseGetParty : " + getPartyInfo);
        partyData partyInfo = JsonUtility.FromJson<partyData>(getPartyInfo);

        // 수정 필요
    }

    public void ResponseLogout()
    {
        Debug.Log("ResponseLogout");
        LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Login);
    }

    public void ResponseBattleAction(string getBattleActionInfo)
    {
        TestbattleActionInfoData actionData = JsonUtility.FromJson<TestbattleActionInfoData>(getBattleActionInfo);
        if (actionData == null)
        {
            Debug.Log("Invalid ResponseBattleAction Data : " + getBattleActionInfo);
        }
    }

    public void ResponseStageStart(string getStageStartInfo)
    {
        TestbattleStateData stateData = JsonUtility.FromJson<TestbattleStateData>(getStageStartInfo);
        if (stateData == null)
        {
            Debug.Log("Invalid ResponseStageStart Data : " + getStageStartInfo);
        }
    }

    public void ResponseGetStageInfo(string getStageInfo)
    {
        TestStageData stageInfo = JsonUtility.FromJson<TestStageData>(getStageInfo);
        if(stageInfo == null)
        {
            Debug.Log("Invalid ResponseStageInfo Data : " + getStageInfo);
        }
    }

    public void ResponseStageResult(string getStageResultInfo)
    {
        TestbattleRewardData resultData = JsonUtility.FromJson<TestbattleRewardData>(getStageResultInfo);
        if(resultData == null)
        {
            Debug.Log("Invalid ResponseStageResult Data : " + getStageResultInfo);
        }
    }

    public void ResponseTowerStart(string getTowerStartInfo)
    {

    }

    public void ResponseTowerResult(string getTowerResultInfo)
    {

    }

    #endregion

    #region Function

    // TODO : 병신같은 로그인부터 다시만들어야함, 포메이션 정보에 대한 세팅값이 모두 잘못되어있음.
    public void Login(UserLoginData getUserLoginData)
    {
        // 로그인에서 플랫폼이 유니티일때 새로 만드는게 아닌 하나의 String으로 모두 받아 오고 동일한 Login 트리를 탈수있도록 짜야함.
        // 즉, 같은 데이터에서 접근 방법만 다를 뿐 데이터 처리 방식은 항상 같아야함.

        UserInfo userInfo = new UserInfo();
        if (TestParseUserInfo(getUserLoginData.userinfo, ref userInfo) == false)
        {
            Debug.Log("Invalid ParseUserInfo Info");
        }
        TestParseGoldInfo(getUserLoginData.gameMoney, ref userInfo);

        UserDataManager.Inst.SetUserInfo(userInfo);
        LeftInfoPopup.Inst.SetLeftInfoUserInfoUpdate(userInfo);

        // 한글로 설명을 적어두지 말고 영어를 읽자
        Dictionary<int, UserServantData> servantDic = new Dictionary<int, UserServantData>();
        if (TestParseServantList(getUserLoginData.servant_list, ref servantDic) == false)
        {
            Debug.Log("Invalid ParseServantList Info");
        }
        // 포이치로 servantList를 Add한다? 그냥 뒤집어 씌워야한다. 이전 로그인 기록이 있을수 있으니.
        UserDataManager.Inst.SetServantDic(servantDic);

        Dictionary<int, UserMonsterData> monsterDic = new Dictionary<int, UserMonsterData>();
        if (TestParseMonsterList(getUserLoginData.monster_list, ref monsterDic) == false)
        {
            Debug.Log("Invalid ParseMonsterList Info");
        }
        UserDataManager.Inst.SetMonsterDic(monsterDic);

        Dictionary<int, UserMountItemData> itemDic = new Dictionary<int, UserMountItemData>();
        if (TestParseItemList(getUserLoginData.item_list, ref itemDic) == false)
        {
            Debug.Log("Invalid ParseItemList Info");
        }
        // 아이템 정보는 왜 추가 안함
        UserDataManager.Inst.SetItemDic(itemDic);

        UserPartyData partyInfo = ParsePartyInfo(getUserLoginData.party_info);
        if (partyInfo == null)
        {
            Debug.Log("invalid ParsePartyList info");
        }
        UserDataManager.Inst.SetPartyInfo(partyInfo);

        LobbyManager.Inst.ChangeSceneState(userInfo.sceneState);
    }

    public bool ParseUserInfo(userData getUserData, ref UserInfo userInfo)
    {
        userInfo.userName = getUserData.user;
        userInfo.userEOS = 0;
        // TODO : EOS는 따로 구하는 코드 추가 필요?
        // userInfo.userEOS = ?
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

        // TODO : 버그 수정 필요
        //servant.isMainHero = getServantInfo.isMainServant;

        servant.index = getServantIndex;
        servant.exp = getServantInfo.exp;
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


        monster.monsterNum = getMonsterInfo.id;


        monster.index = getMonsterIndex;
        monster.exp = getMonsterInfo.exp;
        monster.monsterTypeNum = getMonsterInfo.type;
        monster.gradeNum = getMonsterInfo.grade;
        monster.enforceNum = getMonsterInfo.upgrade;
        

        return monster;
    }

    public bool ParseItemList(List<itemData> getItemList, ref Dictionary<int, UserMountItemData> itemDic)
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

        item.index = getItemInfo.itemnum;
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

    #endregion

    #region TestResponse
    //battle 테스트 함수
    public void TestResponseAction(string getBattleActionInfo)
    {
        TestbattleActionInfoData actiondata = JsonUtility.FromJson<TestbattleActionInfoData>(getBattleActionInfo);
        if (actiondata == null)
        {
            Debug.Log("Invalid Battle Action Data : " + getBattleActionInfo);
        }

        TestAction(actiondata);
    }

    public void TestAction(TestbattleActionInfoData getBattleStateData)
    {

    }

    public void TestResponseBatte(string getBattleStateInfo)
    {
        TestbattleStateData battlestatedata = JsonUtility.FromJson<TestbattleStateData>(getBattleStateInfo);
        if (battlestatedata == null)
        {
            Debug.Log("Invalid Battle Data : " + getBattleStateInfo);
        }


        TestBattleStart(battlestatedata);
    }

    
    public void TestBattleStart(TestbattleStateData getBattleStateData)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            Debug.Log("배틀 스타트!");


            //// 유저인포
            //UserInfo userInfo = new UserInfo();
            //if (TestParseUserInfo(getUserLoginData.userinfo, ref userInfo) == false)
            //{
            //    Debug.Log("Invalid ParseUserInfo Info");
            //    // 재 로그인 시켜야함
            //}

        }
        // 웹이라면, 서버데이터 받아오는것으로
        else
        {

        }
    }

    public bool TestParseUserInfo(userData getUserData, ref UserInfo userInfo)
    {
        userInfo.userName = getUserData.user;
        userInfo.userEOS = 0;
        // TODO : EOS는 따로 구하는 코드 추가 필요?
        // userInfo.userEOS = ??
        userInfo.sceneState = (SCENE_STATE)getUserData.state;

        Debug.Log("State : " + (SCENE_STATE)getUserData.state);

        userInfo.userHero = TestParseServant(0, 0, getUserData.hero);
        if (userInfo.userHero == null)
        {
            Debug.Log("Invalid UserHero Info");
            return false;
        }

        return true;
    }

    public bool TestParseGoldInfo(goldData getgoldData, ref UserInfo userInfo)
    {
        userInfo.userMoney = getgoldData.balance.amount;

        Debug.Log("Gold : " + getgoldData.balance.amount);

        return true;
    }
    
    // Servant List 에서 Dic으로 바꼈으니 이름 변경 필요
    public bool TestParseServantList(List<servantData> getServantList, ref Dictionary<int, UserServantData> servantList)
    {
        for (int i = 0; i < getServantList.Count; i++)
        {
            UserServantData servant = TestParseServant(getServantList[i].index, getServantList[i].party_number, getServantList[i].servant);
            if (servant == null)
            {
                Debug.Log("Invalid Servant Info");
                return false;
            }

            servantList.Add(servant.index, servant);
        }

        return true;
    }

    public UserServantData TestParseServant(int getServantIndex, int getPartyNum, servantInfo getServantInfo)
    {
        if (getServantInfo == null)
        {
            return null;
        }

        UserServantData userServant = new UserServantData();

        userServant.index = getServantIndex;

        userServant.jobNum = getServantInfo.job;
        userServant.isMainHero = getServantIndex == 0 ? true : false;
        userServant.exp = getServantInfo.exp;

        userServant.body = getServantInfo.appear.body;
        userServant.headNum = getServantInfo.appear.head;
        userServant.hairNum = getServantInfo.appear.hair;
        userServant.gender = getServantInfo.appear.gender;


        userServant.partyNum = getPartyNum;


        //TODO : 임시 코드 
        userServant.status = TestParseStatus(getServantInfo.status);
        if (userServant.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }


        return userServant;
    }

    // MonsterList를 Dic으로 바꿨으니 Dic으로 이름 변경 필요
    public bool TestParseMonsterList(List<monsterData> getMonsterList, ref Dictionary<int, UserMonsterData> monsterList)
    {
        for (int i = 0; i < getMonsterList.Count; i++)
        {
            UserMonsterData monster = TestParseMonster(getMonsterList[i].index, getMonsterList[i].party_number, getMonsterList[i].monster);
            if (monster == null)
            {
                Debug.Log("Invalid Monster Info");
                return false;
            }

            monsterList.Add(monster.index, monster);
        }

        return true;
    }
    public UserMonsterData TestParseMonster(int getMonsterIndex, int getPartyNum, monsterInfo getMonsterInfo)
    {
        if (getMonsterInfo == null)
        {
            return null;
        }

        UserMonsterData monster = new UserMonsterData();

        monster.index = getMonsterIndex;

        //monster.monsterNum = getMonsterInfo.look;
        monster.name = getMonsterInfo.name;

        monster.exp = getMonsterInfo.exp;

        monster.monsterTypeNum = getMonsterInfo.type;
        monster.gradeNum = getMonsterInfo.grade;
        monster.enforceNum = getMonsterInfo.upgrade;

        monster.teamNum = getPartyNum;

        //몬스터 스테이터스도 서버로 부터 받아 올 거기 때문에 추가
        monster.status = TestParseStatus(getMonsterInfo.status);
        if (monster.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }

        return monster;
    }

    // Item List를 Dic으로 변경 필요
    public bool TestParseItemList(List<itemData> getItemList, ref Dictionary<int, UserMountItemData> itemDic)
    {
        for (int i = 0; i < getItemList.Count; i++)
        {
            UserMountItemData item = TestParseItem(getItemList[i].index, getItemList[i].item);
            if (item == null)
            {
                Debug.Log("Invalid Item Info");
                return false;
            }
            itemDic.Add(item.index, item);
        }

        return true;
    }

    public UserMountItemData TestParseItem(int getItemIndex, itemInfo getItemInfo)
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

    public Status TestParseStatus(statusInfo getStatusInfo)
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
    #endregion

    public void SetBattleAction(BattleActionData getBattleActionData)
    {

    }

    public void SetStageState(StageStateData getStageStateData)
    {
        BattleSystem.Inst.stageStateData = getStageStateData;
    }


    public void SetStageResult(StageResultData getStageResultData)
    {

    }
}

