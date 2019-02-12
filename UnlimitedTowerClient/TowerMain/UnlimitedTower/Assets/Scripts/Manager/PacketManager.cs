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

        if (UserDataManager.Inst.partyDic.ContainsKey(partyInfo.partyIndex) == false)
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

    public void RequestBattleAction(int heroTarget, int heroAction, int monsterTarget, int monsterAction)
    {
        Debug.Log("RequestBattleAction");

        List<JsonBattleAction> actionList = new List<JsonBattleAction>();
        actionList.Add(new JsonBattleAction(heroTarget, heroAction));
        actionList.Add(new JsonBattleAction(monsterTarget, monsterAction));

        string json = JsonUtility.ToJson(actionList);

        BattleAction(json);
        Debug.Log("Json action : " + json);
    }

    public void RequestStageStart(int stageNum, int partyNum)
    {

    }

    public void RequestStageResult(int stageNum)
    {

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

    public void ResponseGacha(string getGachaInfo)
    {
        Debug.Log("ResponseGacha : " + getGachaInfo);

        JsonData getInfo = JsonMapper.ToObject(getGachaInfo);
        int type = Convert.ToInt32(getInfo["result_type"].ToString());

        if (type == (int)GACHA_RESULT_TYPE.Servant)
        {

            gachaServantData gachaData = JsonUtility.FromJson<gachaServantData>(getGachaInfo);
            UserServantData getServant = ParseServant(gachaData.data.index, gachaData.data.servant);

            UserDataManager.Inst.AddServantData(getServant);
            GachaResultPopup.Instance.Popup(gachaData.data);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Monster)
        {

            gachaMonsterData gachaData = JsonUtility.FromJson<gachaMonsterData>(getGachaInfo);
            UserMonsterData getMonster = ParseMonster(gachaData.data.index, gachaData.data.monster);

            UserDataManager.Inst.AddMonsterData(getMonster);
            GachaResultPopup.Instance.Popup(gachaData.data);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Item)
        {

            gachaItemData gachaData = JsonUtility.FromJson<gachaItemData>(getGachaInfo);
            UserMountItemData getItem = ParseItem(gachaData.data.index, gachaData.data.item);

            //UserDataManager.Inst.SetItemInfo(getItem);
            GachaResultPopup.Instance.Popup(gachaData.data);
        }
    }

    public void ResponseGetParty(string getPartyInfo)
    {
        Debug.Log("ResponseGetParty : " + getPartyInfo);
        partyData partyInfo = JsonUtility.FromJson<partyData>(getPartyInfo);

        UserPartyData getParty = ParseParty(partyInfo.index, partyInfo);
        //if (UserDataManager.Inst.partydic.ContainsKey(getParty.partyIndex) == false)
        //{
        //    UserDataManager.Inst.partydic.Add(getParty.partyIndex, getParty);
        //}
        //else
        //{
        //    UserDataManager.Inst.partydic[getParty.partyIndex] = getParty;
        //}
    }

    public void ResponseLogout()
    {
        Debug.Log("ResponseLogout");
        LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Login);
    }

    public void ResponseBattleAction(string getBattleActionInfo)
    {

    }

    public void ResponseStageStart(string getStageStartInfo)
    {

    }

    public void ResponseStageResult(string getStageResultInfo)
    {

    }

    public void ResponseTowerStart(string getTowerStartInfo)
    {

    }

    public void ResponseTowerResult(string getTowerResultInfo)
    {

    }

#endregion

    #region Function

    // 서버의 string을 파싱해서 생성한 UserLoginData를 고대로 실제 유니티클래스로옮기는부분.
    // 여기서그냥 데이터를 원하는대로 생성해버리면 되겠네. 
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


        /* GET - 유저 ERD데이터 */

        // 서번트아이템리스트 가져오기
        Dictionary<int, UserServantData> servantList = new Dictionary<int, UserServantData>();
        if (ParseServantList(getUserLoginData.servant_list, ref servantList))
        {
            foreach (var item in servantList)
            {
                UserDataManager.Inst.AddServantData(item.Value);
            }
        }
        else
        {
            Debug.Log("Invalid ParseServantList Info");
            // 재 로그인 시켜야함
        }
        
        // 몬스터리스트 가져오기
        Dictionary<int, UserMonsterData> monsterList = new Dictionary<int, UserMonsterData>();
        if (ParseMonsterList(getUserLoginData.monster_list, ref monsterList))
        {
            foreach (var item in monsterList)
            {
                UserDataManager.Inst.AddMonsterData(item.Value);
            }
        }
        else
        {
            Debug.Log("Invalid ParseMonsterList Info");
            // 재 로그인 시켜야함
        }
        
        // 마운트아이템리스트 가져오기
        Dictionary<int, UserMountItemData> itemDic = new Dictionary<int, UserMountItemData>();
        if (ParseItemList(getUserLoginData.item_list, ref itemDic))
        {
            foreach (var item in itemDic)
            {
                UserDataManager.Inst.AddMountitemData(item.Value);
            }
        }
        else
        {
            Debug.Log("Invalid ParseItemList Info");
            // 재 로그인 시켜야함
        }
        
        // 파티리스트 가져오기
        Dictionary<int, UserPartyData> partyDic = new Dictionary<int, UserPartyData>();
        if (ParsePartyList(getUserLoginData.party_list, ref partyDic))
        { 
            foreach(var item in partyDic)
            {
                // 파티데이터 넣기
                UserDataManager.Inst.partyDic.Add(item.Key, item.Value);
                UserDataManager.Inst.partyList.Add(item.Value);
                
            }
        }
        else
        {
            Debug.Log("invalid ParsePartyList info");
            // 재 로그인 시켜야함
        }

        // 포메이션리스트 가져오기
        for (int i = 0; i < getUserLoginData.party_list.Count; i++)
        {
            partyData partydata = getUserLoginData.party_list[i];
            for (int j = 0; j < partydata.party.Count; j++)
            {
                int serverarrayIndex = j;
                int realformindex = ErdManager.instance.getformindextrans(serverarrayIndex);
                formationInfo forminfo = new formationInfo();
                forminfo.form_index = realformindex;

                // 서번트자리면 항상 서번트 가리키고있기
                if (realformindex == 2)
                {
                    forminfo.isplaced = true;
                    forminfo.unit_index = 0;
                }
                else
                {
                    // 배치 안되어있을때
                    if (partydata.party[j] == 0)
                    {
                        forminfo.isplaced = false;
                    }
                    // 배치 되어있을때
                    else
                    {
                        forminfo.isplaced = true;
                        forminfo.unit_index = partydata.party[j];
                    }
                   
                }


                UserFormationData formdata = new UserFormationData();
                formdata.partyIndex = partydata.index;
                formdata.formationIndex = forminfo.form_index;
                formdata.isPlaced = forminfo.isplaced;
                formdata.index = forminfo.unit_index;

                UserDataManager.Inst.UserFormationList.Add(formdata);
            }
        }





        /* INITIALIZE - ERD 역참조데이터 */

        // 서번트
        for (int i = 0; i < UserDataManager.Inst.ServantList.Count; i++)
        {
            UserServantData servantdata = UserDataManager.Inst.ServantList[i];

            // 착용아이템 역참조 정보
            servantdata.mountItemList = UserDataManager.Inst.MountItemList.FindAll((rowdata) => { return rowdata.isMounted && rowdata.mountServantIndex == servantdata.index; });

            // 배치 역참조 정보
            UserFormationData formationdata = UserDataManager.Inst.UserFormationList.Find((rowdata) =>
            {
                return rowdata.isPlaced &&
                 (rowdata.charType == CHAR_TYPE.SERVANT || rowdata.charType == CHAR_TYPE.HERO) &&
                 rowdata.index == servantdata.index;
            });
            if (formationdata != null)
            {
                servantdata.isPlaced = true;
                servantdata.partyNum = formationdata.partyIndex;
                servantdata.formationNum = formationdata.formationIndex;
            }
            else
            {
                servantdata.isPlaced = false;
            }

        }
        // 몬스터
        for (int i = 0; i < UserDataManager.Inst.MonsterList.Count; i++)
        {
            UserMonsterData monsterdata = UserDataManager.Inst.MonsterList[i];

            // 배치 역참조 정보
            UserFormationData formationdata = UserDataManager.Inst.UserFormationList.Find((rowdata) =>
            {
                return rowdata.isPlaced &&
                 (rowdata.charType == CHAR_TYPE.MONSTER) &&
                 rowdata.index == monsterdata.index;
            });
            if (formationdata != null)
            {
                monsterdata.isPlaced = true;
                monsterdata.teamNum = formationdata.partyIndex;
                monsterdata.formationNum = formationdata.formationIndex;
            }
            else
            {
                monsterdata.isPlaced = false;
            }

        }
        // 파티 역참조
        for (int i = 0; i < UserDataManager.Inst.partyList.Count; i++)
        {
            UserPartyData partydata = UserDataManager.Inst.partyList[i];
            for (int j = 0; j < 10; j++)
            {
                int partynum = partydata.partyIndex;
                int formNum = j;

                UserFormationData formdata = UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.partyIndex == partynum && rowdata.formationIndex == formNum; });
                partydata.formationDataDic.Add(formdata.formationIndex, formdata);
                partydata.UserFormationList.Add(formdata);

            }
        }




        // 모든 데이터가 저장이 된 후 화면 전환
        LobbyManager.Inst.ChangeSceneState(userInfo.sceneState);
    }

    public bool ParseUserInfo(userData getUserData, ref UserInfo userInfo)
    {
        userInfo.userName = getUserData.user;
        userInfo.userEOS = 0;
        // TODO : EOS는 따로 구하는 코드 추가 필요?
        // userInfo.userEOS = ??
        userInfo.userMoney = 0;
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

        UserServantData userServant = new UserServantData();

        userServant.index = getServantIndex;

        userServant.jobNum = getServantInfo.job;
        userServant.isMainHero = getServantIndex == 0 ? true : false;
        userServant.exp = getServantInfo.exp;
        
        userServant.body = getServantInfo.appear.body;
        userServant.headNum = getServantInfo.appear.head;
        userServant.hairNum = getServantInfo.appear.hair;
        
        //TODO : 임시 코드 
        userServant.status = ParseStatus(getServantInfo.status);
        if (userServant.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }
        

        return userServant;
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

        monster.index = getMonsterIndex;

        //monster.monsterNum = getMonsterInfo.look;
        monster.name = getMonsterInfo.name;

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
        item.mountitemNum = getItemInfo.itemnum;
        item.tierNum = getItemInfo.tier;
        item.upgradeCount = getItemInfo.upgrade;

        
        return item;
    }

    public bool ParsePartyList(List<partyData> getPartyList, ref Dictionary<int, UserPartyData> partyDic)
    {
        for (int i = 0; i < getPartyList.Count; i++)
        {
            UserPartyData party = ParseParty(getPartyList[i].index, getPartyList[i]);

            if (party == null)
            {
                Debug.Log("Invalid party Info");
                return false;
            }

            partyDic.Add(party.partyIndex, party);
        }

        return true;
    }
    public UserPartyData ParseParty(int getPartyIndex, partyData getParty)
    {
        // 
        UserPartyData party = new UserPartyData();
        party.partyIndex = getPartyIndex;
       
        return party;
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

    #endregion



    #region TestResponse
    public void TestResponseLogin(string getLoginInfo)
    {
        // 
        if (getLoginInfo.StartsWith("{\"sign"))
        {
            SignUp();
            return;
        }

        // 
        UserLoginData userLoginData = JsonUtility.FromJson<UserLoginData>(getLoginInfo);
        if (userLoginData == null)
        {
            Debug.Log("Invalid Login Data : " + getLoginInfo);
        }


        TestLogin(userLoginData);
    }


    public void TestLogin(UserLoginData getUserLoginData)
    {

        UserInfo userInfo = new UserInfo();
        if (TestParseUserInfo(getUserLoginData.userinfo, ref userInfo) == false)
        {
            Debug.Log("Invalid ParseUserInfo Info");
            // 재 로그인 시켜야함
        }
        TestParseGoldInfo(getUserLoginData.gameMoney, ref userInfo);

        UserDataManager.Inst.SetUserInfo(userInfo);
        LeftInfoPopup.Inst.SetLeftInfoUserInfoUpdate(userInfo);


        /* GET - 유저 ERD데이터 */

        // 서번트아이템리스트 가져오기
        Dictionary<int, UserServantData> servantList = new Dictionary<int, UserServantData>();
        if (TestParseServantList(getUserLoginData.servant_list, ref servantList))
        {
            foreach (var item in servantList)
            {
                UserDataManager.Inst.AddServantData(item.Value);
            }
        }
        else
        {
            Debug.Log("Invalid ParseServantList Info");
            // 재 로그인 시켜야함
        }

        // 몬스터리스트 가져오기
        Dictionary<int, UserMonsterData> monsterList = new Dictionary<int, UserMonsterData>();
        if (TestParseMonsterList(getUserLoginData.monster_list, ref monsterList))
        {
            foreach (var item in monsterList)
            {
                UserDataManager.Inst.AddMonsterData(item.Value);
            }
        }
        else
        {
            Debug.Log("Invalid ParseMonsterList Info");
            // 재 로그인 시켜야함
        }

        // 마운트아이템리스트 가져오기
        Dictionary<int, UserMountItemData> itemDic = new Dictionary<int, UserMountItemData>();
        if (TestParseItemList(getUserLoginData.item_list, ref itemDic))
        {
            foreach (var item in itemDic)
            {
                UserDataManager.Inst.AddMountitemData(item.Value);
            }
        }
        else
        {
            Debug.Log("Invalid ParseItemList Info");
            // 재 로그인 시켜야함
        }

        // 파티리스트 가져오기
        Dictionary<int, UserPartyData> partyDic = new Dictionary<int, UserPartyData>();
        if (TestParsePartyList(getUserLoginData.party_list, ref partyDic))
        {
            foreach (var item in partyDic)
            {
                // 파티데이터 넣기
                UserDataManager.Inst.partyDic.Add(item.Key, item.Value);
                UserDataManager.Inst.partyList.Add(item.Value);

            }
        }
        else
        {
            Debug.Log("invalid ParsePartyList info");
            // 재 로그인 시켜야함
        }

        // 포메이션리스트 가져오기
        for (int i = 0; i < getUserLoginData.party_list.Count; i++)
        {
            partyData partydata = getUserLoginData.party_list[i];
            for (int j = 0; j < partydata.party.Count; j++)
            {
                int serverarrayIndex = j;
                int realformindex = ErdManager.instance.getformindextrans(serverarrayIndex);
                formationInfo forminfo = new formationInfo();
                forminfo.form_index = realformindex;

                // 서번트자리면 항상 서번트 가리키고있기
                if (realformindex == 2)
                {
                    forminfo.isplaced = true;
                    forminfo.unit_index = 0;
                }
                else
                {
                    // 배치 안되어있을때
                    if (partydata.party[j] == 0)
                    {
                        forminfo.isplaced = false;
                    }
                    // 배치 되어있을때
                    else
                    {
                        forminfo.isplaced = true;
                        forminfo.unit_index = partydata.party[j];
                    }

                }


                UserFormationData formdata = new UserFormationData();
                formdata.partyIndex = partydata.index;
                formdata.formationIndex = forminfo.form_index;
                formdata.isPlaced = forminfo.isplaced;
                formdata.index = forminfo.unit_index;

                UserDataManager.Inst.UserFormationList.Add(formdata);
            }
        }





        /* INITIALIZE - ERD 역참조데이터 */

        // 서번트
        for (int i = 0; i < UserDataManager.Inst.ServantList.Count; i++)
        {
            UserServantData servantdata = UserDataManager.Inst.ServantList[i];

            // 착용아이템 역참조 정보
            servantdata.mountItemList = UserDataManager.Inst.MountItemList.FindAll((rowdata) => { return rowdata.isMounted && rowdata.mountServantIndex == servantdata.index; });

            // 배치 역참조 정보
            UserFormationData formationdata = UserDataManager.Inst.UserFormationList.Find((rowdata) =>
            {
                return rowdata.isPlaced &&
                 (rowdata.charType == CHAR_TYPE.SERVANT || rowdata.charType == CHAR_TYPE.HERO) &&
                 rowdata.index == servantdata.index;
            });
            if (formationdata != null)
            {
                servantdata.isPlaced = true;
                servantdata.partyNum = formationdata.partyIndex;
                servantdata.formationNum = formationdata.formationIndex;
            }
            else
            {
                servantdata.isPlaced = false;
            }

        }
        // 몬스터
        for (int i = 0; i < UserDataManager.Inst.MonsterList.Count; i++)
        {
            UserMonsterData monsterdata = UserDataManager.Inst.MonsterList[i];

            // 배치 역참조 정보
            UserFormationData formationdata = UserDataManager.Inst.UserFormationList.Find((rowdata) =>
            {
                return rowdata.isPlaced &&
                 (rowdata.charType == CHAR_TYPE.MONSTER) &&
                 rowdata.index == monsterdata.index;
            });
            if (formationdata != null)
            {
                monsterdata.isPlaced = true;
                monsterdata.teamNum = formationdata.partyIndex;
                monsterdata.formationNum = formationdata.formationIndex;
            }
            else
            {
                monsterdata.isPlaced = false;
            }

        }
        // 파티 역참조
        for (int i = 0; i < UserDataManager.Inst.partyList.Count; i++)
        {
            UserPartyData partydata = UserDataManager.Inst.partyList[i];
            for (int j = 0; j < 10; j++)
            {
                int partynum = partydata.partyIndex;
                int formNum = j;

                UserFormationData formdata = UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.partyIndex == partynum && rowdata.formationIndex == formNum; });
                partydata.formationDataDic.Add(formdata.formationIndex, formdata);
                partydata.UserFormationList.Add(formdata);

            }
        }




        // 모든 데이터가 저장이 된 후 화면 전환
        LobbyManager.Inst.ChangeSceneState(userInfo.sceneState);
    }

    public bool TestParseUserInfo(userData getUserData, ref UserInfo userInfo)
    {
        userInfo.userName = getUserData.user;
        userInfo.userEOS = 0;
        // TODO : EOS는 따로 구하는 코드 추가 필요?
        // userInfo.userEOS = ??
        userInfo.userMoney = 0;
        userInfo.sceneState = (SCENE_STATE)getUserData.state;

        Debug.Log("State : " + (SCENE_STATE)getUserData.state);

        userInfo.userHero = TestParseServant(0, 0 ,getUserData.hero);
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

    public bool TestParseServantList(List<servantData> getServantList, ref Dictionary<int, UserServantData> servantList)
    {
        for (int i = 0; i < getServantList.Count; i++)
        {
            UserServantData servant = TestParseServant(getServantList[i].index, getServantList[i].party_number , getServantList[i].servant);
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
    public UserMonsterData TestParseMonster(int getMonsterIndex, int getPartyNum ,monsterInfo getMonsterInfo)
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
        item.mountitemNum = getItemInfo.id;
        item.tierNum = getItemInfo.tier;
        item.upgradeCount = getItemInfo.upgrade;


        return item;
    }

    public bool TestParsePartyList(List<partyData> getPartyList, ref Dictionary<int, UserPartyData> partyDic)
    {
        for (int i = 0; i < getPartyList.Count; i++)
        {
            UserPartyData party = TestParseParty(getPartyList[i].index, getPartyList[i]);

            if (party == null)
            {
                Debug.Log("Invalid party Info");
                return false;
            }

            partyDic.Add(party.partyIndex, party);
        }

        return true;
    }
    public UserPartyData TestParseParty(int getPartyIndex, partyData getParty)
    {
        // 
        UserPartyData party = new UserPartyData();
        party.partyIndex = getPartyIndex;

        for(int i=0; i<getParty.party.Count; ++i)
        {
            UserFormationData member = new UserFormationData();
            member.partyIndex = getPartyIndex;
            if (getParty.party[i] == 0 && i != 0)      //받아온 파티 정보에 0이 있으면 컨트랙트 상에는 비어있는것
            {
                member.isPlaced = false;
                member.index = 0;
            }
            else
            {
                member.isPlaced = true;
                if (i == 0)
                {
                    member.formationIndex = 2;
                }
                else if (i < 5)
                {
                    if (i == 2)
                    {
                        member.formationIndex = 0;
                    }
                }
                else
                {
                    member.formationIndex = i;
                }
                member.index = i;
            }
            party.UserFormationList.Add(member);
            
        }

        return party;
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
}

[Serializable]
public class formationInfo
{
    public int form_index;

    public bool isplaced;
    public int unit_index;
}





