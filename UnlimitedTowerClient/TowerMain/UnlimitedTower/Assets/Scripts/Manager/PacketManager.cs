using LitJson;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

using PairedDelegates = System.Collections.Generic.KeyValuePair<System.Delegate, System.Action<string>>;

[Serializable]
public class PacketManager : MonoSingleton<PacketManager> {

    #region Server design

    /// <summary>
    /// 앞으로 Client => Blockchain 으로 보내는 유일한 Packet 통로가 됩니다.
    /// </summary>
    /// <param name="header"></param>
    /// <param name="body"></param>
    [DllImport("__Internal")]
    private static extern void SendPacket(string header, string body);
    

    private static Dictionary<string, PairedDelegates>  _recvCbs = new Dictionary<string, PairedDelegates>();
    private static Dictionary<string, string>           _recvDatas = new Dictionary<string, string>();
    private const float _MAX_LIMIT_TIME = 20.0f;

    /// <summary>
    /// 아래의 형태로 데이터를 받아야 합니다.
    /// </summary>
    [Serializable]
    private class DefaultPacket
    {
        // 모든 패킷을 분류하는 기준이 됩니다.
        public string header;

        // 패킷에 저장된 내용입니다. (실패시에는 실패 메시지가 될 수 있습니다.)
        public string body;

        // 패킷 요청 성공 여부입니다.
        public bool isSuccess;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"> 반환 받을 데이터의 형태입니다. </typeparam>
    /// <param name="header"></param>
    /// <param name="body"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailed"></param>
    static public void Request<T>(string header, string body = "", Action<T> onSuccess = null, Action<string> onFailed = null)
        => Inst.StartCoroutine(Inst.RequestRoutine(header, body, onSuccess, onFailed));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="header"></param>
    /// <param name="body"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailed"></param>
    static public void Request(string header, string body = "", Action onSuccess = null, Action<string> onFailed = null) 
        => Inst.StartCoroutine(Inst.RequestRoutine(header, body, onSuccess, onFailed));

    /// <summary>
    /// 모든 결과는 이곳으로 전송이 되어야 합니다.
    /// pakcet의 형태는 DefaultPacket의 형태를 따릅니다.
    /// </summary>
    /// <param name="packet"></param>
    public void Response(string packet)
    {
        DefaultPacket packedData = JsonUtility.FromJson<DefaultPacket>(packet);
        _recvDatas.Add(packedData.header, packet);
    }

    private IEnumerator WaitResponse(string header, string body = "", Action<string> onSuccess = null, Action<string> onFailed = null)
    {
        yield return new WaitUntil(() => !_recvCbs.ContainsKey(header));

        PairedDelegates pair = new PairedDelegates(onSuccess, onFailed);
        _recvCbs.Add(header, pair);
        SendPacket(header, body);

        float timer = _MAX_LIMIT_TIME;
        yield return new WaitUntil(() => _recvDatas.ContainsKey(header) || (timer -= Time.unscaledDeltaTime) < 0.0f);

        string packet = _recvDatas[header];
        DefaultPacket packedData = JsonUtility.FromJson<DefaultPacket>(packet);

        //time limited or failed
        if (timer < 0.0f)
        {
            onFailed?.Invoke("EXCEEDED MAXIMUM TIME");
        }
        else if (!packedData.isSuccess)
        {
            onFailed?.Invoke(packedData.body);
        }
        else
        {
            onSuccess?.Invoke(packedData.body);
        }
    }
    
    private IEnumerator RequestRoutine<T>(string header, string body = "", Action<T> onSuccess = null, Action<string> onFailed = null)
    {
        yield return WaitResponse(header, body,
            onSuccess: res =>
            {
                T data = JsonUtility.FromJson<T>(res);

                if (data != null)
                {
                    onSuccess?.Invoke(data);
                }
                else
                {
                    onFailed?.Invoke($"UNEXPECTED TYPE : {res}");
                }
            },
            onFailed: onFailed);
    }

    private IEnumerator RequestRoutine(string header, string body = "", Action onSuccess = null, Action<string> onFailed = null)
    {
        yield return WaitResponse(header, body, onSuccess: res => onSuccess?.Invoke(), onFailed: onFailed);

    }


    #endregion

    #region Request
    public void RequestResourceData()
    {
        Debug.Log("Resource Request");
        //ResourceInfo();

        // 사용 예시 
        // Header 부분에 호출할 함수를 적어서 데이터를 보냅니다.
        // Blockchain 부분에서 Header를 기준으로 데이터를 나누어서 Invoke해줍니다.
        // 이와 동시에 별도의 Response 함수를 구현할 필요가 없어집니다.
        // 이곳 뿐만아니라 어떤 스크립트 내부에서도 사용할 수 있습니다.

        PacketManager.Request<userResourceData>("ResourceInfo",
            onSuccess: data =>
            {
                if (data == null)
                {
                    Debug.Log($"Invalid Resource Data : {data}");
                }
            }, 
            onFailed: msg =>
            {
                Debug.LogError($"[Failed Requesting ResourceInfo] {msg}");
            });
    }


    public void RequestLoginWithScatter()
    {
        Debug.Log("RequestLoginWithScatter");

        Request<string>("Login", 
            onSuccess: ResponseLogin /* 이미 정의된 함수가 있다면 이렇게 대입만 해주어도 됩니다.*/,
            onFailed: msg => /* 람다 함수를 사용하는 경우 */
            {
                Debug.LogError($"[Failed Requesting Login] {msg}");
            });
    }

    public void RequestLogout()
    {
        Debug.Log("RequestLogout");

        Request("Logout",
            onSuccess: ResponseLogout,
            onFailed: msg => {
                Debug.LogError($"[Failed Requesting Logout] {msg}");
            });

        ResponseLogout();
    }

    public void RequestGacha()
    {
        Debug.Log("RequestGacha");
        Request<string>("Gacha",
            onSuccess: ResponseGacha,
            onFailed: msg =>
            {
                Debug.LogError($"[Failed Requesting Gacha] {msg}");
            });
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

        string json = JsonUtility.ToJson(data);
        Debug.Log("print Jsson : : " + json);

        // TODO : Party Save Response
        // SetFormation(json);
    }

    public void RequestBattleAction(int heroTarget, int heroAction, int monsterTarget, int monsterAction)
    {
        Debug.Log("RequestBattleAction");
        TestJsonBattleAction action = new TestJsonBattleAction();
        action.heroTargetIndex = heroTarget;
        action.heroActionType = heroAction;
        action.monsterTargetIndex = monsterTarget;
        action.monsterActionType = monsterAction;

        string json = JsonUtility.ToJson(action);
        Request<stageActionInfoData>("BattleAction", 
                body: json,
                onSuccess: ResponseBattleAction,
                onFailed: msg => { Debug.LogError($"[Failed Requesting BattleAction] {msg}"); }
                );
    }

    public void RequestStageStart(int stageNum, int partyNum)
    {
        Debug.Log("Request Start Battle");

        TestJsonStartBattle startBattle = new TestJsonStartBattle();
        startBattle.stageNum = stageNum;
        startBattle.partyNum = partyNum;

        string json = JsonUtility.ToJson(startBattle);

        Debug.Log("Json start : " + json);

        Request<stageStateData>("StageStart",
                body: json,
                onSuccess: ResponseStageStart,
                onFailed: msg => { Debug.LogError($"[Failed Requesting StageStart] {msg}"); }
                );
    }

    public void RequestStageResult()
    {
        Debug.Log("Request Get Battle Reward");

        Request<stageRewardData>("StageResult",
            onSuccess: ResponseStageReward,
            onFailed: msg =>
            {
                Debug.LogError($"[Failed Requesting StageResult] {msg}");
            });
    }

    public void RequestStageExit()
    {
        Debug.Log("Request Battle Exit");

        Request("StageResult",
            onSuccess: ResponseStageExit,
            onFailed: msg =>
            {
                Debug.LogError($"[Failed Requesting StageResult] {msg}");
            });
    }

    #endregion

    #region Response
    
    public void ResponseLogin(string getLoginInfo)
    {
        if (getLoginInfo.StartsWith("{\"sign"))
        {
            Request<string>("SignUp",
                onSuccess: ResponseLogin,
                onFailed: msg => { Debug.LogError($"[Failed Requesting SignUp] {msg}"); }
                );
            return;
        }
        Debug.Log("Login Data : " + getLoginInfo);

        UserLoginData userLoginData = JsonUtility.FromJson<UserLoginData>(getLoginInfo); 
        if(userLoginData == null)
        {
            Debug.Log("Invalid Login Data : " + getLoginInfo);
        }

        Login(userLoginData);
    }

    public void Login(UserLoginData getUserLoginData)
    {
        UserInfo userInfo = new UserInfo();
        if (ParseUserInfo(getUserLoginData.userinfo, ref userInfo) == false)
        {
            Debug.Log("Invalid ParseUserInfo Info");
        }
        ParseGoldInfo(getUserLoginData.token, getUserLoginData.eos, ref userInfo);

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

        if (userInfo.sceneState == SCENE_STATE.StageBattle)
        {
            Request<stageStateData>("GetBattle",
                    onSuccess: ResponseStageStart,
                    onFailed: msg => { Debug.LogError($"[Failed Requesting GetBattle] {msg}"); }
                    );
        }
        else
        {
            StartCoroutine(LoadSceneAsync("Lobby", "Logging in ... "));
        }
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
        StartCoroutine(LoadSceneAsync("login", "Loading scene ... "));
    }

    public void ResponseError(string errorMessage)
    {
        errorCode error = JsonUtility.FromJson<errorCode>(errorMessage);

        if(error.code == "battle")
        {
            BattleSystem.Inst.ReTargeting();
        }
        BattleSystem.Inst.ErrorLog(error.message);
    }


    public void ResponseStageStart(stageStateData getBattleStateData)
    {
        Debug.Log("배틀 스타트!");
        UserDataManager.Inst.SetStageState(getBattleStateData);
        StartCoroutine(LoadSceneAsync("CharacterBattleScene", "Now, Loading battle field ... "));
    }

    public void ResponseBattleAction(stageActionInfoData getBattleActionData)
    {
        Debug.Log("턴 진행!");
        if (getBattleActionData.turn == UserDataManager.Inst.stageActionInfo.turn)
        {
            Debug.Log("데이터 중복");
            return;
        }
        else
        {
            UserDataManager.Inst.SetStageAction(getBattleActionData);
            BattleSystem.Inst.StartCoroutine(BattleSystem.Inst.BattleStart());
        }
    }

    public void ResponseStageReward(stageRewardData getReward)
    {
        Debug.Log("배틀 끝 보상 획득!");
        UserDataManager.Inst.SetStageReward(getReward);
    }

    public void ResponseStageExit()
    {
        SceneManager.LoadScene("Lobby");
    }

    #endregion

    #region Function

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

    public bool ParseGoldInfo(string getgoldData, string getEOS , ref UserInfo userInfo)
    {
        userInfo.userEOS = ulong.Parse(getEOS);
        userInfo.userMoney = ulong.Parse(getgoldData);

        Debug.Log("getEOS : " + getEOS);
        Debug.Log("getGold : " + getgoldData);

        Debug.Log("EOS : " + userInfo.userEOS);
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
        monster.upgradeCount = getMonsterInfo.upgrade;

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
    
    private IEnumerator LoadSceneAsync(string name, string loadingMsg)
    {
        AsyncOperation ao = null;

        UTLoadingManager.Description desc = new UTLoadingManager.Description
        {
            startComment = loadingMsg,
            finishedComment = loadingMsg,
            predicate = () => ao?.isDone ?? false,
        };

        UTLoadingManager.Instance.BeginScene(desc);

        yield return new WaitForSecondsRealtime(1.0f);

        ao = SceneManager.LoadSceneAsync(name);

        while (!ao.isDone)
        {
            UTLoadingManager.Instance.SetProgress(ao?.progress ?? 0.0f, loadingMsg);
            yield return null;
        }
    }

    #endregion
}

