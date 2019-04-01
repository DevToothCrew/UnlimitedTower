﻿using LitJson;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Linq;

[Serializable]
public class PacketManager : MonoSingleton<PacketManager> {

    #region Server design

    /// <summary>
    /// 앞으로 Client => Blockchain 으로 보내는 유일한 Packet 통로가 됩니다.
    /// </summary>
    /// <param name="header"></param>
    /// <param name="body"></param>
    [DllImport("__Internal")]
    private static extern void SendPacket(string packet);
    
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
    static public void Request(string header, string body = "", Action<string> onSuccess = null, Action<string> onFailed = null)
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
        DefaultPacket sendPackedData = new DefaultPacket();
        sendPackedData.header = header;
        sendPackedData.body = body;
        sendPackedData.isSuccess = true;
        string sendPacket = JsonUtility.ToJson(sendPackedData);
        SendPacket(sendPacket);

        float timer = _MAX_LIMIT_TIME;
        yield return new WaitUntil(() => _recvDatas.ContainsKey(header) || (timer -= Time.unscaledDeltaTime) < 0.0f);

        string packet = _recvDatas[header];
        DefaultPacket recvPackedData = JsonUtility.FromJson<DefaultPacket>(packet);

        //time limited or failed
        if (timer < 0.0f)
        {
            onFailed?.Invoke("EXCEEDED MAXIMUM TIME");
        }
        else if (!recvPackedData.isSuccess)
        {
            onFailed?.Invoke(recvPackedData.body);
        }
        else
        {
            onSuccess?.Invoke(recvPackedData.body);
        }
        Debug.Log("recv packet : " + _recvDatas[header]);
        _recvDatas.Remove(header);
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

    private IEnumerator RequestRoutine(string header, string body = "", Action<string> onSuccess = null, Action<string> onFailed = null)
    {
        yield return WaitResponse(header, body,
            onSuccess: res =>
            {
                if (!string.IsNullOrWhiteSpace(res))
                {
                    onSuccess?.Invoke(res);
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

    // 예제
    public void RequestResourceData()
    {
        Debug.Log("Resource Request");
        //ResourceInfo();

        // 사용 예시 
        // Header 부분에 호출할 함수를 적어서 데이터를 보냅니다.
        // Blockchain 부분에서 Header를 기준으로 데이터를 나누어서 Invoke해줍니다.
        // 이와 동시에 별도의 Response 함수를 구현할 필요가 없어집니다.
        // 이곳 뿐만아니라 어떤 스크립트 내부에서도 사용할 수 있습니다.

        Request<userResourceData>("ResourceInfo",
            onSuccess: data =>
            {
                if (data == null)
                {
                    Debug.Log($"Invalid Resource Data : {data}");
                }
            }, 
            onFailed: msg =>
            {
                Debug.Log($"[Failed Requesting ResourceInfo] {msg}");
            });
    }

    // 로그인
    public void RequestLoginWithScatter()
    {
        Debug.Log("RequestLoginWithScatter");

        //반환받을 객체가 json 자체라면 형태를 적지 않습니다.
        Request("Login", 
            onSuccess: ResponseLogin /* 이미 정의된 함수가 있다면 이렇게 대입만 해주어도 됩니다.*/,
            onFailed: msg => /* 람다 함수를 사용하는 경우 */
            {
                Debug.Log($"[Failed Requesting Login] {msg}");
            });
    }

    // 로그아웃
    public void RequestLogout()
    {
        Debug.Log("RequestLogout");

        Request("Logout",
            onSuccess: ResponseLogout,
            onFailed: msg => {
                Debug.Log($"[Failed Requesting Logout] {msg}");
            });
    }

    // 가챠
    public void RequestGacha(int getGachaIndex)
    {
        Debug.Log("RequestGacha");

        GachaJson gacha = new GachaJson();
        gacha.gachaIndex = getGachaIndex;

        string json = JsonUtility.ToJson(gacha);

        Request("Gacha",
            body: json,
            onSuccess: ResponseGacha,
            onFailed: msg =>
            {
                Debug.Log($"[Failed Requesting Gacha] {msg}");
            });
    }

    // 파티 저장
    public void RequestSaveParty(UserPartyData partyInfo)
    {
        if(partyInfo == null)
        {
            Debug.Log("Invalid PartyInfo");
        }

        Debug.Log("RequestSaveParty");

        PartySaveJson data = new PartySaveJson();
        data.partyNum = partyInfo.partyIndex;

        for(int i = 0; i < partyInfo.formationDataDic.Count; ++i)
        {
            if (partyInfo.formationDataDic.ContainsKey(i) == false)
            {
                if(i <= DEFINE.ServantMaxFormationNum)
                {
                    data.servantList.Add(0);
                }
                else
                {
                    data.monsterList.Add(0);
                }
            }
            else
            {
                if(i <= DEFINE.ServantMaxFormationNum)
                {
                    data.servantList.Add(partyInfo.formationDataDic[i].index);
                }
                else
                {
                    data.monsterList.Add(partyInfo.formationDataDic[i].index);
                }
            }
        }

        string json = JsonUtility.ToJson(data);
        Debug.Log("print Jsson : : " + json);

        Request<partyData>("SaveParty",
                body: json,
                onSuccess: ResponseSaveParty,
                onFailed: msg => { Debug.Log($"[Failed Requesting SaveParty] {msg}"); }
                );
    }

    // 배틀 액션 시작
    public void RequestBattleAction(int getTurn)
    {
        Debug.Log("RequestBattleAction");
        BattleActionJson action = new BattleActionJson();
        action.turn = getTurn;

        string json = JsonUtility.ToJson(action);
        Request<battleActionData>("BattleAction", 
                body: json,
                onSuccess: ResponseBattleAction,
                onFailed: msg => { Debug.Log($"[Failed Requesting BattleAction] {msg}"); }
                );
    }

    // 스테이지 시작
    public void RequestStageStart(int stageType, int partyNum, int stageFloor)
    {
        Debug.Log("Request Start Battle");

        StageStartJson startBattle = new StageStartJson();
        startBattle.stageType = stageType;
        startBattle.partyNum = partyNum;
        startBattle.stageFloor = stageFloor;

        string json = JsonUtility.ToJson(startBattle);

        Debug.Log("Json start : " + json);

        Request<stageStateData>("StageStart",
                body: json,
                onSuccess: ResponseStageStart,
                onFailed: msg => { Debug.Log($"[Failed Requesting StageStart] {msg}"); }
                );
    }

    // 스테이지 보상
    public void RequestStageReward()
    {
        Debug.Log("Request Stage Reward");

        Request<stageRewardData>("StageReward",
        onSuccess: ResponseStageReward,
        onFailed: msg => { Debug.Log($"[Failed Requesting StageStart] {msg}"); }
        );
    }

    // 스테이지 종료
    public void RequestStageExit()
    {
        Debug.Log("Request Battle Exit");

        Request("StageExit",
            onSuccess: ResponseStageExit,
            onFailed: msg =>
            {
                Debug.Log($"[Failed Requesting StageResult] {msg}");
            });
    }

    // 서번트 분해
    public void RequestServantGrind(List<int> servantIndexList)
    {
        Debug.Log("Request Grind Servant");
        if(servantIndexList == null)
        {
            Debug.Log("Invalid Request");
            return;
        }

        ServantGrindJson servantGrind = new ServantGrindJson();
        servantGrind.servantIndexList = servantIndexList;

        string json = JsonUtility.ToJson(servantGrind);

        Debug.Log("Json start : " + json);

        Request<servantGrindResultData>("ServantGrind",
                body: json,
                onSuccess: ResponseServantGrind,
                onFailed: msg => { Debug.Log($"[Failed Requesting ServantGrind] {msg}"); }
                );
    }

    // 몬스터 판매
    public void RequestMonsterSell(List<int> monsterIndexList)
    {
        Debug.Log("Request Monster Sell");
        if(monsterIndexList == null)
        {
            Debug.Log("Invalid Request");
            return;
        }

        MonsterSellJson monsterSell = new MonsterSellJson();
        monsterSell.monsterIndexList = monsterIndexList;

        string json = JsonUtility.ToJson(monsterSell);

        Debug.Log("Json start : " + json);

        Request<sellMonsterResultData>("MonsterSell",
                onSuccess: ResponseMonsterSell,
                onFailed: msg => 
                {
                    Debug.Log($"[Failed Requesting MonsterSell] {msg}");
                });
    }

    // 장비 판매
    public void RequestEquipmentSell(List<int> equipmentIndexList)
    {
        Debug.Log("Request Equipment Sell");
        if (equipmentIndexList == null)
        {
            Debug.Log("Invalid Request");
            return;
        }

        EquipmentSellJson equipmentSell = new EquipmentSellJson();
        equipmentSell.equipmentIndexList = equipmentIndexList;

        string json = JsonUtility.ToJson(equipmentSell);

        Debug.Log("Json start : " + json);

        Request<sellEquipmentResultData>("EquipmentSell",
                onSuccess: ResponseEquipmentSell,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting EquipmentSell] {msg}");
                });

    }

    // 아이템 판매
    public void RequestItemSell(List<itemData> itemDataList)
    {
        Debug.Log("Request Item Sell");
        if (itemDataList == null)
        {
            Debug.Log("Invalid Request");
            return;
        }

        ItemSellJson itemSell = new ItemSellJson();
        itemSell.itemDataList = itemDataList;
        string json = JsonUtility.ToJson(itemSell);

        Debug.Log("Json start : " + json);

        Request<sellItemResultData>("ItemSell",
                onSuccess: ResponseItemSell,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting ItemSell] {msg}");
                });
    }

    // 장비 장착
    public void RequestEquipServant(int servantIndex, int equipitemIndex)
    {
        Debug.Log("Request Equip Servant");
        if (servantIndex == 0 || equipitemIndex == 0)
        {
            Debug.Log("Invalid Request");
            return;
        }

        EquipServantJson equipServant = new EquipServantJson();
        equipServant.servantIndex = servantIndex;
        equipServant.equipitemIndex = equipitemIndex;

        string json = JsonUtility.ToJson(equipServant);

        Debug.Log("Json start : " + json);

        Request<servantData>("EquipServant",
                onSuccess: ResponseEquipServant,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting EquipServant] {msg}");
                });
    }

    // 장비 해제
    public void RequestUnequipServant(int servantIndex, int equipitemIndex)
    {
        Debug.Log("Request Unequip Servant");
        if (servantIndex == 0 || equipitemIndex == 0)
        {
            Debug.Log("Invalid Request");
            return;
        }


        UnequipServantJson unequipServant = new UnequipServantJson();
        unequipServant.servantIndex = servantIndex;
        unequipServant.equipitemIndex = equipitemIndex;

        string json = JsonUtility.ToJson(unequipServant);

        Debug.Log("Json start : " + json);

        Request<servantData>("UnequipServant",
                onSuccess: ResponseUnequipServant,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting UnequipServant] {msg}");
                });
    }

    // 몬스터 강화
    public void RequestMonsterUpgrade(int mainMonsterIndex, int subMonsterIndex)
    {
        Debug.Log("Request Monster Upgrade");
        if (mainMonsterIndex == 0 || subMonsterIndex == 0)
        {
            Debug.Log("Invalid Request");
            return;
        }

        MonsterUpgradeJson monsterUpgrade = new MonsterUpgradeJson();
        monsterUpgrade.mainMonsterIndex = mainMonsterIndex;
        monsterUpgrade.subMonsterIndex = subMonsterIndex;

        string json = JsonUtility.ToJson(monsterUpgrade);

        Debug.Log("Json start : " + json);

        Request<monsterUpgradeResultData>("MonsterUpgrade",
                onSuccess: ResponseMonsterUpgrade,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting MonsterUpgrade] {msg}");
                });
    }

    // 아이템 강화
    public void RequestEquipmentUpgrade(int mainEquipmentIndex, List<int> addItemIndexList)
    {
        Debug.Log("Request Equipment Upgrade");
        if (mainEquipmentIndex == 0 || addItemIndexList == null)
        {
            Debug.Log("Invalid Request");
            return;
        }

        EquipmentUpgradeJson equipmentUpgrade = new EquipmentUpgradeJson();
        equipmentUpgrade.mainEquipmentIndex = mainEquipmentIndex;
        equipmentUpgrade.addItemIndexList = addItemIndexList;
        string json = JsonUtility.ToJson(equipmentUpgrade);

        Debug.Log("Json start : " + json);

        Request<equipmentUpgradeResultData>("EquipmentUpgrade",
                onSuccess: ResponseEquipmentUpgrade,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting EquipmentUpgrade] {msg}");
                });
    }

    // 상점 아이템 구매
    public void RequestBuyItem(int index, int itemCount)
    {
        Debug.Log("Request Buy Item");
        if (index == 0 || itemCount == 0)
        {
            Debug.Log("Invalid Request");
            return;
        }

        BuyItemJson buyItem = new BuyItemJson();
        buyItem.index = index;
        buyItem.itemCount = itemCount;

        string json = JsonUtility.ToJson(buyItem);

        Debug.Log("Json start : " + json);

        Request<itemData>("BuyItem",
                onSuccess: ResponseBuyItem,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting BuyItem] {msg}");
                });
    }

    // 인벤토리 구매
    public void RequestBuyInventory(int type, int itemCount)
    {
        Debug.Log("Request Buy Inventory");
        if (type == 0 || itemCount == 0)
        {
            Debug.Log("Invalid Request");
            return;
        }

        BuyInventoryJson buyInventory = new BuyInventoryJson();
        buyInventory.type = type;
        buyInventory.itemCount = itemCount;

        string json = JsonUtility.ToJson(buyInventory);

        Debug.Log("Json start : " + json);

        Request<inventoryInfo>("BuyInventory",
                onSuccess: ResponseBuyInventory,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting BuyInventory] {msg}");
                });
    }

    // 로열 서번트 구매
    public void RequestBuyRoyalServant(int servantIndex)
    {
        Debug.Log("Request Buy Royalservant");
        if (servantIndex == 0)
        {
            Debug.Log("Invalid Request");
            return;
        }

        BuyRoyalservantJson buyRoyalServant = new BuyRoyalservantJson();
        buyRoyalServant.servantIndex = servantIndex;
       
        string json = JsonUtility.ToJson(buyRoyalServant);

        Debug.Log("Json start : " + json);

        Request<servantData>("BuyRoyalServant",
                onSuccess: ResponseBuyRoyalServant,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting ServantGrind] {msg}");
                });
    }

    // 우편 수령
    public void RequestMailOpen(List<int> mailOpenIndexList)
    {
        Debug.Log("Request Maill Open");
        if (mailOpenIndexList == null)
        {
            Debug.Log("Invalid Request");
            return;
        }

        MailOpenJson mailOpen = new MailOpenJson();
        mailOpen.mailOpenIndexList = mailOpenIndexList;

        string json = JsonUtility.ToJson(mailOpen);

        Debug.Log("Json start : " + json);

        Request<mailOpenResultData>("MailOpen",
                onSuccess: ResponseMailOpen,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting ServantGrind] {msg}");
                });
    }

    // 타워 Start
    public void RequestTowerStart()
    {
        throw new NotImplementedException();
    }

    // 타워 Exit
    public void RequestTowerExit()
    {
        throw new NotImplementedException();
    }

    // 타워 End
    public void RequestTowerReward()
    {
        throw new NotImplementedException();
    }

    // 채팅
    public void RequestEnterChat()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Response
    
    // 로그인
    public void ResponseLogin(string getLoginInfo)
    {
        if (getLoginInfo.StartsWith("{\"sign"))
        {
            // 가입
            Request<string>("SignUp",
                onSuccess: ResponseLogin,
                onFailed: msg => { Debug.LogError($"[Failed Requesting SignUp] {msg}"); }
                );
            return;
        }
        // Debug.Log("Login Data : " + getLoginInfo);

        UserLoginData userLoginData = JsonUtility.FromJson<UserLoginData>(getLoginInfo); 
        if(userLoginData == null)
        {
            Debug.Log("Invalid Login Data : " + getLoginInfo);
        }

        UserInfo userInfo = new UserInfo();
        if (ParseUserInfo(userLoginData, ref userInfo) == false)
        {
            Debug.Log("Invalid ParseUserInfo Info");
        }
        UserDataManager.Inst.SetUserInfo(userInfo);

        Dictionary<int, UserServantData> servantDic = new Dictionary<int, UserServantData>();
        if (ParseServantDic(userLoginData.servant_list, ref servantDic) == false)
        {
            Debug.Log("Invalid ParseServantList Info");
        }
        UserDataManager.Inst.SetServantDic(servantDic);

        Dictionary<int, UserMonsterData> monsterDic = new Dictionary<int, UserMonsterData>();
        if (ParseMonsterDic(userLoginData.monster_list, ref monsterDic) == false)
        {
            Debug.Log("Invalid ParseMonsterList Info");
        }
        UserDataManager.Inst.SetMonsterDic(monsterDic);

        Dictionary<int, UserEquipmentData> equipmentDic = new Dictionary<int, UserEquipmentData>();
        if (ParseEquipmentDic(userLoginData.equipment_list, ref equipmentDic) == false)
        {
            Debug.Log("Invalid ParseEquipmentList Info");
        }
        UserDataManager.Inst.SetEquipmentDic(equipmentDic);

        Dictionary<int, UserItemData> itemDic = new Dictionary<int, UserItemData>();
        if (ParseItemDic(userLoginData.item_list, ref itemDic) == false)
        {
            Debug.Log("Invalid ParseItemList Info");
        }
        UserDataManager.Inst.SetItemDic(itemDic);

        UserPartyData partyInfo = ParsePartyInfo(userLoginData.party_info);
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

    // 가챠
    public void ResponseGacha(string getGachaInfo)
    {
        Debug.Log("ResponseGacha : " + getGachaInfo);

        JsonData getInfo = JsonMapper.ToObject(getGachaInfo);
        int type = Convert.ToInt32(getInfo["result_type"].ToString());

        UserDataManager.Inst.UseEOS(10000);

        if (type == (int)GACHA_RESULT_TYPE.Servant)
        {
            Debug.Log(getGachaInfo);

            gachaServantData gachaData = JsonUtility.FromJson<gachaServantData>(getGachaInfo);
            UserServantData getServant = ParseServant(gachaData.data);

            UserDataManager.Inst.AddServantData(getServant);

            GachaManager.Instance.ResultGacha(getServant);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Monster)
        {
            Debug.Log(getGachaInfo);

            gachaMonsterData gachaData = JsonUtility.FromJson<gachaMonsterData>(getGachaInfo);
            UserMonsterData getMonster = ParseMonster(gachaData.data);

            UserDataManager.Inst.AddMonsterData(getMonster);

            GachaManager.Instance.ResultGacha(getMonster);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Equipment)
        {
            Debug.Log(getGachaInfo);

            gachaItemData gachaData = JsonUtility.FromJson<gachaItemData>(getGachaInfo);
            UserEquipmentData getEquipment = ParseEquipment(gachaData.data);

            UserDataManager.Inst.AddEquipmentData(getEquipment);

            GachaManager.Instance.ResultGacha(getEquipment);
        }

    }

    // 로그아웃
    public void ResponseLogout()
    {
        Debug.Log("ResponseLogout");
        UserDataManager.Inst.InitUserInfo();
        StartCoroutine(LoadSceneAsync("login", "Loading scene ... "));
    }

    public void ResponseSaveParty(partyData getPartyData)
    {
        Debug.Log("ResponseSaveParty");

        UserPartyData partyInfo = ParsePartyInfo(getPartyData);
        if (partyInfo == null)
        {
            Debug.Log("invalid ParsePartyList info");
        }
        UserDataManager.Inst.SetPartyInfo(partyInfo);
    }

    // 스테이지 시작
    public void ResponseStageStart(stageStateData getBattleStateData)
    {
        //Debug.Log("스테이지 시작");
        UserDataManager.Inst.SetStageState(getBattleStateData);
        StartCoroutine(LoadSceneAsync("CharacterBattleScene", "Now, Loading battle field ... "));
    }

    // 배틀 액션
    public void ResponseBattleAction(battleActionData getBattleActionData)
    {
        //Debug.Log("턴 진행!");
        if (getBattleActionData.turn == UserDataManager.Inst.stageActionInfo.turn)
        {
            Debug.Log("데이터 중복");
            return;
        }
        else
        {
            UserDataManager.Inst.SetStageAction(getBattleActionData);
            BattleManager.Inst.StartCoroutine(BattleManager.Inst.BattleStart());
        }
    }

    // 스테이지 종료
    public void ResponseStageExit()
    {
        UserDataManager.Inst.SetStageState(null);
        UserDataManager.Inst.SetStageAction(null);
        UserDataManager.Inst.SetStageReward(null);
        SceneManager.LoadScene("Lobby");
    }

    // 스테이지 보상
    public void ResponseStageReward(stageRewardData getReward)
    {
        Debug.Log("배틀 끝 보상 획득!");
        // 보여주기용 Reward Set
        UserDataManager.Inst.SetStageReward(getReward);

        // 경험치 추가
        if (getReward.get_exp_list.Count > 0)
        {
            // 파티에 추가해야 하는지, 이걸 캐릭터 인덱스 + get exp로 해야하는지??
        }

        if (getReward.get_servant_list.Count > 0)
        {
            for (int i = 0; i < getReward.get_servant_list.Count; i++)
            {
                servantData servant = getReward.get_servant_list[i];

                UserServantData servantData = ParseServant(servant);
                if (servantData == null)
                {
                    Debug.Log("Invalid Servant Info : " + i);
                    return;
                }
                UserDataManager.Inst.AddServantData(servantData);
            }
        }

        if (getReward.get_monster_list.Count > 0)
        {
            for (int i = 0; i < getReward.get_monster_list.Count; i++)
            {
                monsterData monster = getReward.get_monster_list[i];

                UserMonsterData monsterData = ParseMonster(monster);
                if(monsterData == null)
                {
                    Debug.Log("Invalid Monster Info : " + i);
                    return;
                }
                UserDataManager.Inst.AddMonsterData(monsterData);
            }
        }

        if (getReward.get_equipment_list.Count > 0)
        {
            for(int i = 0; i < getReward.get_equipment_list.Count; i++)
            {
                equipmentData equipment = getReward.get_equipment_list[i];

                UserEquipmentData equipmentData = ParseEquipment(equipment);
                if(equipmentData == null)
                {
                    Debug.Log("Invalid Equipment Info : " + i);
                    return;
                }
                UserDataManager.Inst.AddEquipmentData(equipmentData);
            }

        }

        if (getReward.get_item_list.Count > 0)
        {
            for(int i = 0; i < getReward.get_item_list.Count; i++)
            {
                itemData item = getReward.get_item_list[i];

                UserItemData itemData = ParseItem(item);
                if(itemData == null)
                {
                    Debug.Log("Invalid Item Info : " + i);
                    return;
                }
                UserDataManager.Inst.AddItemData(itemData);
            }

        }
        BattleManager.Inst.BattleEnd();
    }
    public void ResopnseResource(userResourceData getResourceInfo)
    {
        Debug.Log("Eos Account Resource Info");
    }

    // 서번트 분해
    public void ResponseServantGrind(servantGrindResultData getServantGrindData)
    {
        Debug.Log("서번트 분해, 영혼 획득!");

        if(getServantGrindData.grindServantIndexList.Count == 0)
        {
            Debug.Log("Invalid Grind Result");
            return;
        }

        if (UserDataManager.Inst.DelServantList(getServantGrindData.grindServantIndexList) == false)
        {
            Debug.Log("Invalid Grind Index");
            return;
        }

        Dictionary<int, UserItemData> getItemDataDic = new Dictionary<int, UserItemData>();
        if (ParseItemDic(getServantGrindData.itemList, ref getItemDataDic) == false)
        {
            Debug.Log("Invalid ParseItemDic");
            return;
        }

        if(UserDataManager.Inst.AddItemDataList(getItemDataDic.Values.ToList()) == false)
        {
            Debug.Log("Invalid AdditemDataList");
            return;
        }
    }
    
    // 몬스터 판매
    public void ResponseMonsterSell(sellMonsterResultData getSellMonsterResultData)
    {
        Debug.Log("몬스터 판매, UTG 획득!");

        if(getSellMonsterResultData.sellMonsterIndexList.Count == 0)
        {
            Debug.Log("Invalid Sell Result");
            return;
        }

        if(UserDataManager.Inst.DelMonsterList(getSellMonsterResultData.sellMonsterIndexList) == false)
        {
            Debug.Log("Invalid DelMonsterList");
            return;
        }

        Dictionary<int, UserItemData> getItemDataDic = new Dictionary<int, UserItemData>();
        if (ParseItemDic(getSellMonsterResultData.itemList, ref getItemDataDic) == false)
        {
            Debug.Log("Invalid ParseItemDic");
            return;
        }

        if (UserDataManager.Inst.AddItemDataList(getItemDataDic.Values.ToList()) == false)
        {
            Debug.Log("Invalid AdditemDataList");
            return;
        }
    }

    // 장비 판매
    public void ResponseEquipmentSell(sellEquipmentResultData getSellEquipmentResultData)
    {
        Debug.Log("장비 판매, UTG 획득!");

        if(getSellEquipmentResultData.sellEquipmentIndexList.Count == 0)
        {
            Debug.Log("Invalid Sell Result");
            return;
        }

        if(UserDataManager.Inst.DelEquipmentList(getSellEquipmentResultData.sellEquipmentIndexList) == false)
        {
            Debug.Log("Invalid DelEquipmentList");
            return;
        }

        Dictionary<int, UserItemData> getItemDataDic = new Dictionary<int, UserItemData>();
        if (ParseItemDic(getSellEquipmentResultData.itemList, ref getItemDataDic) == false)
        {
            Debug.Log("Invalid ParseItemDic");
            return;
        }

        if (UserDataManager.Inst.AddItemDataList(getItemDataDic.Values.ToList()) == false)
        {
            Debug.Log("Invalid AdditemDataList");
            return;
        }
    }

    // 아이템 판매
    public void ResponseItemSell(sellItemResultData getSellItemResultData)
    {
        Debug.Log("아이템 판매, UTG 획득!");

        if (getSellItemResultData.sellItemDataList.Count == 0)
        {
            Debug.Log("Invalid Sell Result");
            return;
        }

        Dictionary<int, UserItemData> sellItemDataDic = new Dictionary<int, UserItemData>();
        if(ParseItemDic(getSellItemResultData.sellItemDataList, ref sellItemDataDic) == false)
        {
            Debug.Log("Invalid ParseItemDic");
            return;
        }

        if (UserDataManager.Inst.DelItemList(sellItemDataDic.Values.ToList()) == false)
        {
            Debug.Log("Invalid DelItemList");
            return;
        }

        Dictionary<int, UserItemData> getItemDataDic = new Dictionary<int, UserItemData>();
        if (ParseItemDic(getSellItemResultData.itemList, ref getItemDataDic) == false)
        {
            Debug.Log("Invalid ParseItemDic");
            return;
        }

        if (UserDataManager.Inst.AddItemDataList(getItemDataDic.Values.ToList()) == false)
        {
            Debug.Log("Invalid AdditemDataList");
            return;
        }
    }

    // 장비 장착
    public void ResponseEquipServant(servantData getServantData)
    {
        Debug.Log("서번트 장비 장착 !");

        if(getServantData.index == 0)
        {
            Debug.Log("Invalid Servant");
            return;
        }
        
        UserServantData equipmentServantData = ParseServant(getServantData);

        if (equipmentServantData == null)
        {
            Debug.Log("Invalid ParseEquipment");
            return;
        }
        UserDataManager.Inst.SetServant(equipmentServantData);
        
    }

    // 장비 해제
    public void ResponseUnequipServant(servantData getServantData)
    {
        Debug.Log("서번트 장비 장착해제 !");

        UserServantData unequipmentServantData = ParseServant(getServantData);

        UserDataManager.Inst.DelEquipment(unequipmentServantData.index);
    }

    // 몬스터 강화
    public void ResponseMonsterUpgrade(monsterUpgradeResultData getMonsterUpgradeResultData)
    {
        Debug.Log("몬스터 업그레이드 !");

        if(getMonsterUpgradeResultData.is_success == true)
        {
            Debug.Log("성공");
        }
        else
        {
            Debug.Log("실패");
        }

        UserMonsterData monsterData = ParseMonster(getMonsterUpgradeResultData.main_monster_data);
        if (monsterData == null)
        {
            Debug.Log("Invalid ParseMonster");
            return;
        }
        UserDataManager.Inst.SetMonster(monsterData);
        UserDataManager.Inst.DelMonster(getMonsterUpgradeResultData.sub_monster_index);

        for (int i = 0; i < getMonsterUpgradeResultData.need_item_list.Count; i++)
        {
            UserItemData itemData = ParseItem(getMonsterUpgradeResultData.need_item_list[i]);
            if(itemData == null)
            {
                Debug.Log("Invalid ParseItem");
                return;
            }
            UserDataManager.Inst.SetItem(itemData);
        }
    }

    // 아이템 강화
    public void ResponseEquipmentUpgrade(equipmentUpgradeResultData getEquipmentUpgradeResultData)
    {
        Debug.Log("장비 업그레이드 !");

        UserEquipmentData equipmentData = ParseEquipment(getEquipmentUpgradeResultData.main_equipment_data);

        if (getEquipmentUpgradeResultData.is_success == true)
        {
            UserDataManager.Inst.SetEquipment(equipmentData);
        }
        else
        {
            UserDataManager.Inst.DelEquipment(getEquipmentUpgradeResultData.main_equipment_data.index);
        }
        for (int i = 0; i < getEquipmentUpgradeResultData.add_item_list.Count; i++)
        {
            UserDataManager.Inst.DelEquipment(getEquipmentUpgradeResultData.add_item_list[i].index);
        }
    }

    // 상점 아이템 구매
    public void ResponseBuyItem(itemData getBuyItemData)
    {
        Debug.Log("소모품 구매 !");

        UserItemData itemData = ParseItem(getBuyItemData);
    }

    // 인벤토리 구매
    public void ResponseBuyInventory(inventoryInfo getInventoryInfo)
    {
        Debug.Log("인벤토리 구매 !");
    }

    // 로열 서번트 구매
    public void ResponseBuyRoyalServant(servantData getBuyRoyalservantData)
    {
        Debug.Log("로얄 서번트 구매 !");
    }

    // 우편 수령
    public void ResponseMailOpen(mailOpenResultData getMailOpenResultData)
    {
        Debug.Log("메일 오픈 !");
    }

    // 타워 시작
    public void ResponseTowerStart()
    {

    }

    // 타워 나가기
    public void ResponseTowerExit()
    {

    }

    // 타워 보상
    public void ResponseTowerReward()
    {

    }

    // 채팅
    public void ResponseEnterChat()
    {

    }

    
    #endregion

    #region Function

    public bool ParseUserInfo(UserLoginData getUserData, ref UserInfo userInfo)
    {
        userInfo.userName = getUserData.user_data.user;
        userInfo.userEXP = getUserData.user_data.exp;

        userInfo.userEOS = ulong.Parse(getUserData.eos);
        // token은 UTG로 바꿀 필요가 있지 않을까요
        userInfo.userUTG = ulong.Parse(getUserData.token);

        //Debug.Log("getEOS : " + getUserData.eos);
        //Debug.Log("getUTG : " + getUserData.token);

        userInfo.sceneState = (SCENE_STATE)getUserData.user_data.state;
        //Debug.Log("State : " + (SCENE_STATE)getUserData.user_data.state);

        return true;
    }

    public bool ParseServantDic(List<servantData> getServantList, ref Dictionary<int, UserServantData> servantDic)
    {
        for (int i = 0; i < getServantList.Count; i++)
        {
            UserServantData servant = ParseServant(getServantList[i]);
            if (servant == null)
            {
                Debug.Log("Invalid Servant Info");
                return false;
            }

            servantDic.Add(servant.index, servant);
        }

        return true;
    }

    public UserServantData ParseServant(servantData getServantData)
    {
        if (getServantData == null)
        {
            return null;
        }

        if(getServantData.servant == null)
        {
            return null;
        }

        UserServantData userServant = new UserServantData();
        userServant.index = getServantData.index;
        //userServant.id = CSVData.Inst.GetServantID(5, getServantData.servant.job, getServantData.servant.appear.body, getServantData.servant.appear.gender, getServantData.servant.appear.head, getServantData.servant.appear.hair);
        userServant.id = getServantData.servant.id;
        if (CSVData.Inst.GetServantData(userServant.id) == null)
        {
            Debug.Log("Invalid Servant ID : " + userServant.id);
        }

        userServant.grade = 5; // grade 필요, 현재는 그냥 서번트만 얻을수 있으니 일단 이렇게
        userServant.state = getServantData.servant.state;

        userServant.partyIndex = getServantData.party_number;
        if(userServant.partyIndex > 0)
        {
            userServant.isPlaced = true;
        }

        userServant.exp = getServantData.servant.exp;

        userServant.status = ParseStatus(getServantData.servant.status);
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

        return status;
    }

    public bool ParseMonsterDic(List<monsterData> getMonsterList, ref Dictionary<int, UserMonsterData> monsterDic)
    {
        for (int i = 0; i < getMonsterList.Count; i++)
        {
            UserMonsterData monster = ParseMonster(getMonsterList[i]);
            if (monster == null)
            {
                Debug.Log("Invalid Monster Info");
                return false;
            }

            monsterDic.Add(monster.index, monster);
        }

        return true;
    }
    
    public UserMonsterData ParseMonster(monsterData getMonsterData)
    {
        if (getMonsterData == null)
        {
            return null;
        }

        if(getMonsterData.monster == null)
        {
            return null;
        }

        UserMonsterData monster = new UserMonsterData();
        monster.index = getMonsterData.index;
        monster.id = getMonsterData.monster.id;
        monster.grade = getMonsterData.monster.grade;
        monster.upgrade = getMonsterData.monster.upgrade;
        monster.state = getMonsterData.monster.state;

        monster.partyIndex = getMonsterData.party_number;
        if(monster.partyIndex > 0)
        {
            monster.isPlaced = true;
        }

        monster.exp = getMonsterData.monster.exp;
        monster.status = ParseStatus(getMonsterData.monster.status);
        if (monster.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }

        return monster;
    }

    public bool ParseEquipmentDic(List<equipmentData> getEquipmentList, ref Dictionary<int, UserEquipmentData> equipmentDic)
    {
        for (int i = 0; i < getEquipmentList.Count; i++)
        {
            UserEquipmentData item = ParseEquipment(getEquipmentList[i]);
            if (item == null)
            {
                Debug.Log("Invalid Item Info");
                return false;
            }
            equipmentDic.Add(item.index, item);
        }

        return true;
    }

    public UserEquipmentData ParseEquipment(equipmentData getEquipmentData)
    {
        if (getEquipmentData == null)
        {
            return null;
        }

        UserEquipmentData equipmentData = new UserEquipmentData();
        equipmentData.index = getEquipmentData.index;
        equipmentData.id = getEquipmentData.equipment.id;
        equipmentData.grade = getEquipmentData.equipment.grade;
        equipmentData.upgrade = getEquipmentData.equipment.upgrade;
        equipmentData.state = getEquipmentData.equipment.state;

        equipmentData.value = getEquipmentData.equipment.value;
        equipmentData.equipServantIndex = getEquipmentData.equipment.equipservantindex;
        if(equipmentData.equipServantIndex > 0)
        {
            equipmentData.isEquiped = true;
        }

        return equipmentData;
    }

    public bool ParseItemDic(List<itemData> getItemList, ref Dictionary<int, UserItemData> itemDic)
    {
        for (int i = 0; i < getItemList.Count; i++)
        {
            UserItemData item = ParseItem(getItemList[i]);
            if (item == null)
            {
                Debug.Log("Invalid Item Info");
                return false;
            }
            itemDic.Add(item.index, item);
        }

        return true;
    }

    public UserItemData ParseItem(itemData getItemData)
    {
        if (getItemData == null)
        {
            return null;
        }

        UserItemData itemData = new UserItemData();

        itemData.index = getItemData.index;
        itemData.id = getItemData.item.id;
        // Type이 필요한지?
        itemData.count = getItemData.item.count;

        return itemData;
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

        for(int i = 0; i < getPartyData.servant_list.Count; i++)
        {
            UserFormationData servantFormationData = new UserFormationData();
            servantFormationData.formationIndex = i;
            if (getPartyData.servant_list[i] > 0)
            {
                servantFormationData.index = getPartyData.servant_list[i];
                partyInfo.formationDataDic.Add(servantFormationData.formationIndex, servantFormationData);
            }
            else
            {
                servantFormationData.index = 0;
                partyInfo.formationDataDic.Add(i, servantFormationData);
            }
        }

        for (int i = 0; i < getPartyData.monster_list.Count; i++)
        {
            UserFormationData monsterFormationData = new UserFormationData();
            monsterFormationData.formationIndex = i + 5;
            if (getPartyData.monster_list[i] > 0)
            {
                monsterFormationData.index = getPartyData.monster_list[i];
                partyInfo.formationDataDic.Add(monsterFormationData.formationIndex, monsterFormationData);
            }
            else
            {
                monsterFormationData.index = 0;
                partyInfo.formationDataDic.Add(i + 5, monsterFormationData);
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

