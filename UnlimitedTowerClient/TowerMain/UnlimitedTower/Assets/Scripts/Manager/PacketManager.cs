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
    private const float _MAX_LIMIT_TIME = 120.0f;

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
            TopUIManager.Inst.ShowSimpleErrorPopup("EXCEEDED MAXIMUM TIME");
            onFailed?.Invoke("EXCEEDED MAXIMUM TIME");
        }
        else if (!recvPackedData.isSuccess)
        {
            TopUIManager.Inst.ShowSimpleErrorPopup(recvPackedData.body);
            onFailed?.Invoke(recvPackedData.body);
        }
        else
        {
            onSuccess?.Invoke(recvPackedData.body);
        }
        Debug.Log("recv packet : " + _recvDatas[header]);
        _recvDatas.Remove(header);

        PacketLoadingUI.Inst.SetLoading(false);
    }
    
    private IEnumerator RequestRoutine<T>(string header, string body = "", Action<T> onSuccess = null, Action<string> onFailed = null)
    {
        PacketLoadingUI.Inst.SetLoading(true);

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
        PacketLoadingUI.Inst.SetLoading(true);

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
        PacketLoadingUI.Inst.SetLoading(true);

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
    public void RequestSaveParty(int partyIndex, List<int> indexList)
    {
        Debug.Log("RequestSaveParty");

        PartySaveJson data = new PartySaveJson();
        data.partyNum = partyIndex;

        for(int i = 0; i < 5; ++i)
        {
            data.servantList.Add(indexList[i]);
            Debug.Log("Print Party Servant Formation : " + i + ", Index : " + indexList[i]);

            data.monsterList.Add(indexList[i + 5]);
            Debug.Log("Print Party Monster Formation : " + (i + 5) + ", Index : " + indexList[i + 5]);
        }

        string json = JsonUtility.ToJson(data);
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
    public void RequestStageStart(int stageType, int stageFloor, int partyNum)
    {
        Debug.Log("Request Start Battle");

        StageStartJson startBattle = new StageStartJson();
        startBattle.stageType = stageType;
        startBattle.stageFloor = stageFloor;
        startBattle.partyNum = partyNum;

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

    // 스테이지 보상
    public void RequestPVPResult()
    {
        Debug.Log("Request PVP Result");

        Request<pvpResultData>("PVPResult",
        onSuccess: ResponsePVPResult,
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
    public void RequestServantBurn(List<int> servantIndexList)
    {
        Debug.Log("Request Servant Burn");
        if(servantIndexList == null)
        {
            Debug.Log("Invalid Request");
            return;
        }

        ServantBurnJson servantBurn = new ServantBurnJson();
        servantBurn.servantIndexList = servantIndexList;

        string json = JsonUtility.ToJson(servantBurn);

        Debug.Log("Json start : " + json);

        Request<servantBurnResultData>("ServantBurn",
                body: json,
                onSuccess: ResponseServantBurn,
                onFailed: msg => { Debug.Log($"[Failed Requesting ServantBurn] {msg}"); }
                );
    }

    // 몬스터 판매
    public void RequestMonsterBurn(List<int> monsterIndexList)
    {
        Debug.Log("Request Monster Burn");
        if(monsterIndexList == null)
        {
            Debug.Log("Invalid Request");
            return;
        }

        MonsterBurnJson monsterBurn = new MonsterBurnJson();
        monsterBurn.monsterIndexList = monsterIndexList;

        string json = JsonUtility.ToJson(monsterBurn);

        Debug.Log("Json start : " + json);

        Request<monsterBurnResultData>("MonsterBurn",
                body: json,
                onSuccess: ResponseMonsterBurn,
                onFailed: msg => 
                {
                    Debug.Log($"[Failed Requesting MonsterBurn] {msg}");
                });
    }

    // 장비 판매
    public void RequestEquipmentBurn(List<int> equipmentIndexList)
    {
        Debug.Log("Request Equipment Burn");
        if (equipmentIndexList == null)
        {
            Debug.Log("Invalid Request");
            return;
        }

        EquipmentBurnJson equipmentBurn = new EquipmentBurnJson();
        equipmentBurn.equipmentIndexList = equipmentIndexList;

        string json = JsonUtility.ToJson(equipmentBurn);

        Debug.Log("Json start : " + json);

        Request<equipmentBurnResultData>("EquipmentBurn",
                body: json,
                onSuccess: ResponseEquipmentBurn,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting EquipmentBurn] {msg}");
                });

    }

    // 아이템 판매
    public void RequestItemBurn(List<itemData> itemDataList)
    {
        Debug.Log("Request Item Burn");
        if (itemDataList == null)
        {
            Debug.Log("Invalid Request");
            return;
        }

        ItemBurnJson itemBurn = new ItemBurnJson();
        itemBurn.itemDataList = itemDataList;
        string json = JsonUtility.ToJson(itemBurn);

        Debug.Log("Json start : " + json);

        Request<itemBurnResultData>("ItemBurn",
                body: json,
                onSuccess: ResponseItemBurn,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting ItemBurn] {msg}");
                });
    }

    // 장비 장착
    public void RequestEquipServant(int servantIndex, EQUIPMENT_TYPE type, int equipmentIndex)
    {
        Debug.Log("Request Equip Servant");
        if (servantIndex == 0 || equipmentIndex == 0)
        {
            Debug.Log("Invalid Request");
            return;
        }

        // 서번트 인덱스 검사
        UserServantData servantData = UserDataManager.Inst.GetServantInfo(servantIndex);
        if (servantData == null)
        {
            Debug.LogError("Invalid Servant Index : " + servantIndex);
            return;
        }

        // 장비 타입 검사
        if (servantData.equipmentDic.ContainsKey(type) == false)
        {
            Debug.LogError("Invalid Servant Data");
            return;
        }

        // 서번트 동일 장비 검사
        if (servantData.equipmentDic[type] == equipmentIndex)
        {
            Debug.Log("Already Equip");
            return;
        }

        // 장비 인덱스 검사
        UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(equipmentIndex);
        if (equipmentData == null)
        {
            Debug.LogError("Invalid Equipment Index : " + equipmentIndex);
            return;
        }

        // 장비 인덱스에 대한 타입 검사
        if (equipmentData.equipmentType != type)
        {
            Debug.Log("Invalid Type : " + type.ToString() + ", ");
            return;
        }

        // DB 장비 ID 검사
        DBEquipmentData dbEquipmentData = CSVData.Inst.GetEquipmentData(equipmentData.id);
        if (dbEquipmentData == null)
        {
            Debug.Log("Invalid Equipment Data ID : " + equipmentData.id);
            return;
        }

        // DB 서번트 ID 검사
        DBServantData dbServantData = CSVData.Inst.GetServantData(servantData.id);
        if (dbServantData == null)
        {
            Debug.Log("Invalid Servant Data ID : " + servantData.id);
            return;
        }

        // 장착 가능 직업 검사
        if (dbEquipmentData.isEquipAble(dbServantData.GetJobFlag) == false)
        {
            Debug.Log("Invalid Servant Equipable");
            return;
        }

        EquipServantJson equipServant = new EquipServantJson();
        equipServant.servantIndex = servantIndex;
        equipServant.equipmentSlot = (int)type;
        equipServant.equipitemIndex = equipmentIndex;

        string json = JsonUtility.ToJson(equipServant);

        Debug.Log("Json start : " + json);

        Request<servantEquipData>("EquipServant",
                body: json,
                onSuccess: ResponseEquipServant,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting EquipServant] {msg}");
                });
    }

    // 장비 해제
    public void RequestUnequipServant(int servantIndex, EQUIPMENT_TYPE type)
    {
        Debug.Log("Request Unequip Servant");
        if (servantIndex == 0)
        {
            Debug.Log("Invalid Request");
            return;
        }

        // 서번트 인덱스 검사
        UserServantData servantData = UserDataManager.Inst.GetServantInfo(servantIndex);
        if (servantData == null)
        {
            Debug.LogError("Invalid Servant Index : " + servantIndex);
            return;
        }

        // 장비 타입 검사
        if (servantData.equipmentDic.ContainsKey(type) == false)
        {
            Debug.LogError("Invalid Servant Data");
            return;
        }

        // 장비 검사
        if (servantData.equipmentDic[type] == 0)
        {
            Debug.LogError("Invalid Servant Equip Data");
            return;
        }

        UnequipServantJson unequipServant = new UnequipServantJson();
        unequipServant.servantIndex = servantIndex;
        unequipServant.equipmentSlot = (int)type;

        string json = JsonUtility.ToJson(unequipServant);

        Debug.Log("Json start : " + json);

        Request<servantUnequipData>("UnequipServant",
                body: json,
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
                body: json,
                onSuccess: ResponseMonsterUpgrade,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting MonsterUpgrade] {msg}");
                });
    }

    // 아이템 강화
    public void RequestEquipmentUpgrade(int mainEquipmentIndex, List<int> addItemIDList)
    {
        Debug.Log("Request Equipment Upgrade");
        if (mainEquipmentIndex == 0 || addItemIDList == null)
        {
            Debug.Log("Invalid Request");
            return;
        }

        EquipmentUpgradeJson equipmentUpgrade = new EquipmentUpgradeJson();
        equipmentUpgrade.mainEquipmentIndex = mainEquipmentIndex;
        equipmentUpgrade.addItemIDList = addItemIDList;
        string json = JsonUtility.ToJson(equipmentUpgrade);

        Debug.Log("Json start : " + json);

        Request<equipmentUpgradeResultData>("EquipmentUpgrade",
                body: json,
                onSuccess: ResponseEquipmentUpgrade,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting EquipmentUpgrade] {msg}");
                });
    }

    // 상점 아이템 구매
    public void RequestShopBuyItem(int index, int itemCount)
    {
        Debug.Log("Request ShopBuy Item");
        if (index == 0 || itemCount == 0)
        {
            Debug.Log("Invalid Request");
            return;
        }

        ShopBuyItemJson buyItem = new ShopBuyItemJson();
        buyItem.index = index;
        buyItem.itemCount = itemCount;

        string json = JsonUtility.ToJson(buyItem);

        Debug.Log("Json start : " + json);

        Request<shopBuyResultData>("ShopBuyItem",
                body: json,
                onSuccess: ResponseShopBuyItem,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting ShopBuyItem] {msg}");
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
                body: json,
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
                body: json,
                onSuccess: ResponseBuyRoyalServant,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting BuyRoyalServant] {msg}");
                });
    }

    public void RequestLobbyInfo()
    {
        Request<lobbyInfoResultData>("LobbyInfo",
                onSuccess: ResponseLobbyInfo,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting LobbyInfo] {msg}");
                });
    }

    public void RequestMailList()
    {
        Debug.Log("Request Mail List");

        Request<mailListResultData>("MailList",
                onSuccess: ResponseMailList,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting MailList] {msg}");
                });
    }

    // 우편 수령
    public void RequestMailOpen(int mailIndex)
    {
        Debug.Log("Request Maill Open");
        if (mailIndex == 0)
        {
            Debug.Log("Invalid Request");
            return;
        }

        List<int> mailOpenIndexList = new List<int>();
        mailOpenIndexList.Add(mailIndex);

        MailOpenJson mailOpen = new MailOpenJson();
        mailOpen.mailOpenIndexList = mailOpenIndexList;

        string json = JsonUtility.ToJson(mailOpen);

        Debug.Log("Json start : " + json);

        Request<mailOpenResultData>("MailOpen",
                body: json,
                onSuccess: ResponseMailOpen,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting MailOpen] {msg}");
                });
    }

    // 타워 Start
    public void RequestTowerStart(int floorNumber)
    {
        Debug.Log("Request TowerStart");

        TowerJson towerJson = new TowerJson();
        towerJson.floor = floorNumber;

        string json = JsonUtility.ToJson(towerJson);

        Debug.Log("Json start : " + json);

        Request<stageStateData>("TowerStart",
                body: json,
                onSuccess: ResponseTowerStart,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting PVPStart] {msg}");
                });
    }

    // 타워 Exit
    public void RequestTowerExit()
    {
        throw new NotImplementedException();
    }

    // 타워 End
    public void RequestTowerReward()
    {
        Debug.Log("Request Tower Result");

        Request("TowerResult",
        onSuccess: ResponseTowerReward,
        onFailed: msg => { Debug.Log($"[Failed Requesting Tower Result] {msg}"); }
        );
    }

    // 채팅
    public void RequestEnterChat()
    {
        throw new NotImplementedException();
    }

    // PVP Start
    public void RequestPVPStart(string accountName)
    {
        Debug.Log("Request PVPStart");

        PVPJson pvpJson = new PVPJson();
        pvpJson.enemy = accountName;

        string json = JsonUtility.ToJson(pvpJson);

        Debug.Log("Json start : " + json);

        Request<stageStateData>("PVPStart",
                body: json,
                onSuccess: ResponsePVPStart,
                onFailed: msg =>
                {
                    Debug.Log($"[Failed Requesting PVPStart] {msg}");
                });
    }

    public void RequestShopInfo(SHOP_TYPE type)
    {
        Debug.Log("Request ShopInfo");

        ShopJson shop = new ShopJson();
        shop.type = (int)type;

        string json = JsonUtility.ToJson(shop);

        Debug.Log("Json start : " + json);

        Request<shopInfoResultData>("ShopInfo",
            body: json,
            onSuccess: ResponseShopInfo,
            onFailed: msg =>
            {
                Debug.Log($"[Failed Requesting ShopInfo] {msg}");
            });
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

        if (userInfo.sceneState == SCENE_STATE.StageBattle || userInfo.sceneState == SCENE_STATE.PVP || userInfo.sceneState == SCENE_STATE.TowerBattle)
        {
            Request<stageStateData>("GetBattle",
                    onSuccess: ResponseStageStart,
                    onFailed: msg => { Debug.LogError($"[Failed Requesting GetBattle] {msg}"); }
                    );
        }
        else
        {
            StartCoroutine(LoadSceneAsync("Lobby", "Log in ... "));
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

            gachaEquipmentData gachaData = JsonUtility.FromJson<gachaEquipmentData>(getGachaInfo);
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

        LobbyTopInfo.Inst.UpdateTopInfo();
    }

    // 스테이지 시작
    public void ResponseStageStart(stageStateData getBattleStateData)
    {
        UserStageStateData stageData = ParseStageStateData(getBattleStateData);
        if(stageData == null)
        {
            Debug.Log("Invalid StageData");
        }

        UserDataManager.Inst.SetStageState(stageData);
        UserDataManager.Inst.SetSceneState(SCENE_STATE.StageBattle);
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
        UserDataManager.Inst.InitStageInfo();
        SceneManager.LoadScene("Lobby");
        Time.timeScale = 1.0f;
    }

    // 스테이지 보상
    public void ResponseStageReward(stageRewardData getReward)
    {
        Debug.Log("배틀 끝 보상 획득!");
        // 보여주기용 Reward Set
        UserDataManager.Inst.SetStageReward(getReward);

        UserDataManager.Inst.addUTG(getReward.reward_money);    //유저 보상 UTG 반영
        UserDataManager.Inst.addRankExp(getReward.get_rank_exp.exp, getReward.get_rank_exp.lvup); //유저 랭크, 경험치 반영

        // 경험치 추가
        if (getReward.get_char_exp_list.Count > 0)
        {
            UserStageStateData stateData = UserDataManager.Inst.GetStageState();
            if(stateData.turn == 0)
            {
                Debug.Log("Invalid State Data");
                return;
            }

            for (int i = 0; i < getReward.get_char_exp_list.Count; ++i)
            {
                int position = getReward.get_char_exp_list[i].pos;
                if(stateData.myStateList.ContainsKey(position) == false)
                {
                    Debug.LogError("Invalid ExpData Position : " + position);
                    return;
                }

                if(stateData.myStateList[position].charType == CHAR_TYPE.SERVANT)
                {
                    UserDataManager.Inst.SetServantExp(stateData.myStateList[position].index, getReward.get_char_exp_list[i].exp, getReward.get_char_exp_list[i].lvup);
                }
                else if(stateData.myStateList[position].charType == CHAR_TYPE.MONSTER)
                {
                    UserDataManager.Inst.SetMonsterExp(stateData.myStateList[position].index, getReward.get_char_exp_list[i].exp, getReward.get_char_exp_list[i].lvup);
                }
                else
                {
                    Debug.Log("Invalid Char Type");
                    return;
                }
            }
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
                UserDataManager.Inst.SetItem(itemData);
            }

        }

        BattleManager.Inst.SetReward();
    }
    public void ResopnseResource(userResourceData getResourceInfo)
    {
        Debug.Log("Eos Account Resource Info");
    }

    public void ResponseServantBurn(servantBurnResultData getServantBurnData)
    {
        Debug.Log("서번트 분해, 영혼 획득!");

        Debug.Log("Start Servant IndexList Count");
        if (getServantBurnData.servantIndexList.Count == 0)
        {
            Debug.Log("Invalid Grind Result");
            return;
        }

        Debug.Log("Start Del Servant List");
        if (UserDataManager.Inst.DelServantList(getServantBurnData.servantIndexList) == false)
        {
            Debug.Log("Invalid Grind Index");
            return;
        }

        Debug.Log("Start Parse Item List");
        Dictionary<int, UserItemData> getItemDataDic = new Dictionary<int, UserItemData>();
        if (ParseItemDic(getServantBurnData.itemList, ref getItemDataDic) == false)
        {
            Debug.Log("Invalid ParseItemDic");
            return;
        }

        ulong getUTG = Convert.ToUInt64(getServantBurnData.utg) - UserDataManager.Inst.GetUserUTG();
        if (getUTG > 0)
        {
            UserDataManager.Inst.SetUserUTG(Convert.ToUInt64(getServantBurnData.utg));
        }

        Debug.Log("Start Update View");
        SubViewDeconstruction.Inst.updateViewFinishRequest();

        Debug.Log("Start Show Burn Result");
        TopUIManager.Inst.ShowBurnResult(getItemDataDic.Values.ToList(), getUTG);
    }
    
    public void ResponseMonsterBurn(monsterBurnResultData getBurnMonsterResultData)
    {
        Debug.Log("몬스터 판매, UTG 획득!");

        if(getBurnMonsterResultData.monsterIndexList.Count == 0)
        {
            Debug.Log("Invalid Burn Result");
            return;
        }

        if(UserDataManager.Inst.DelMonsterList(getBurnMonsterResultData.monsterIndexList) == false)
        {
            Debug.Log("Invalid DelMonsterList");
            return;
        }

        Dictionary<int, UserItemData> getItemDataDic = new Dictionary<int, UserItemData>();
        if (ParseItemDic(getBurnMonsterResultData.itemList, ref getItemDataDic) == false)
        {
            Debug.Log("Invalid ParseItemDic");
            return;
        }

        if (UserDataManager.Inst.SetItemList(getItemDataDic.Values.ToList()) == false)
        {
            Debug.Log("Invalid AdditemDataList");
            return;
        }

        ulong getUTG = Convert.ToUInt64(getBurnMonsterResultData.utg) - UserDataManager.Inst.GetUserUTG();
        if (getUTG > 0)
        {
            UserDataManager.Inst.SetUserUTG(Convert.ToUInt64(getBurnMonsterResultData.utg));
        }

        SubViewDeconstruction.Inst.updateViewFinishRequest();

        TopUIManager.Inst.ShowBurnResult(getItemDataDic.Values.ToList(), getUTG);
    }

    public void ResponseEquipmentBurn(equipmentBurnResultData getBurnEquipmentResultData)
    {
        Debug.Log("장비 판매, UTG 획득!");

        if(getBurnEquipmentResultData.equipmentIndexList.Count == 0)
        {
            Debug.Log("Invalid Burn Result");
            return;
        }

        if(UserDataManager.Inst.DelEquipmentList(getBurnEquipmentResultData.equipmentIndexList) == false)
        {
            Debug.Log("Invalid DelEquipmentList");
            return;
        }

        Dictionary<int, UserItemData> getItemDataDic = new Dictionary<int, UserItemData>();
        if (ParseItemDic(getBurnEquipmentResultData.itemList, ref getItemDataDic) == false)
        {
            Debug.Log("Invalid ParseItemDic");
            return;
        }

        if (UserDataManager.Inst.SetItemList(getItemDataDic.Values.ToList()) == false)
        {
            Debug.Log("Invalid AdditemDataList");
            return;
        }

        ulong getUTG = Convert.ToUInt64(getBurnEquipmentResultData.utg) - UserDataManager.Inst.GetUserUTG();
        if (getUTG > 0)
        {
            UserDataManager.Inst.SetUserUTG(Convert.ToUInt64(getBurnEquipmentResultData.utg));
        }

        SubViewDeconstruction.Inst.updateViewFinishRequest();

        TopUIManager.Inst.ShowBurnResult(getItemDataDic.Values.ToList(), getUTG);
    }

    public void ResponseItemBurn(itemBurnResultData getBurnItemResultData)
    {
        Debug.Log("아이템 판매, UTG 획득!");

        if (getBurnItemResultData.itemDataList.Count == 0)
        {
            Debug.Log("Invalid Burn Result");
            return;
        }

        Dictionary<int, UserItemData> burnItemDataDic = new Dictionary<int, UserItemData>();
        if(ParseItemDic(getBurnItemResultData.itemDataList, ref burnItemDataDic) == false)
        {
            Debug.Log("Invalid ParseItemDic");
            return;
        }

        if (UserDataManager.Inst.SetItemList(burnItemDataDic.Values.ToList()) == false)
        {
            Debug.Log("Invalid DelItemList");
            return;
        }

        Dictionary<int, UserItemData> getItemDataDic = new Dictionary<int, UserItemData>();
        if (ParseItemDic(getBurnItemResultData.itemList, ref getItemDataDic) == false)
        {
            Debug.Log("Invalid ParseItemDic");
            return;
        }

        if (UserDataManager.Inst.SetItemList(getItemDataDic.Values.ToList()) == false)
        {
            Debug.Log("Invalid AdditemDataList");
            return;
        }

        ulong getUTG = Convert.ToUInt64(getBurnItemResultData.utg) - UserDataManager.Inst.GetUserUTG();
        if (getUTG > 0)
        {
            UserDataManager.Inst.SetUserUTG(Convert.ToUInt64(getBurnItemResultData.utg));
        }

        TopUIManager.Inst.ShowBurnResult(getItemDataDic.Values.ToList(), getUTG);
    }

    // 장비 장착
    public void ResponseEquipServant(servantEquipData getServantEquipData)
    {
        Debug.Log("서번트 장비 장착 !");

        UserServantData servantData = UserDataManager.Inst.GetServantInfo(getServantEquipData.servant_index);
        if(servantData == null)
        {
            Debug.Log("Invalid Servant");
            return;
        }

        EQUIPMENT_TYPE type = (EQUIPMENT_TYPE)getServantEquipData.equipment_slot;
        if (servantData.equipmentDic.ContainsKey(type) == false)
        {
            Debug.Log("Invalid Servant EquipmentDic");
            return;
        }

        if(servantData.equipmentDic[type] == getServantEquipData.equipment_index)
        {
            Debug.Log("Invalid Servant Already Equip Each Equipment");
            return;
        }

        if(servantData.equipmentDic[type] != 0)
        {
            if(UserDataManager.Inst.UnequipServant(servantData.index, type) == false)
            {
                Debug.Log("Invalid Servant Unequip");
                return;
            }
        }

        if (UserDataManager.Inst.EquipServant(servantData.index, type, getServantEquipData.equipment_index) == false)
        {
            Debug.Log("Invalid Servant Unequip");
            return;
        }

        if (SubViewEquipment.Inst != null)
        {
            SubViewEquipment.Inst.UpdateViewChangeEquipment();
            SubViewEquipment.Inst.ResetScrollDataByChangeSetData();
            if (PartyInfoVC.Inst != null)
            {
                PartyInfoVC.Inst.updateDetailInfo(PartyInfoVC.Inst.selected_unit_idx);
            }
                
        }
    }

    // 장비 해제
    public void ResponseUnequipServant(servantUnequipData getServantUnequipData)
    {
        UserServantData servantData = UserDataManager.Inst.GetServantInfo(getServantUnequipData.servant_index);
        if (servantData == null)
        {
            Debug.Log("Invalid Servant");
            return;
        }

        EQUIPMENT_TYPE type = (EQUIPMENT_TYPE)getServantUnequipData.equipment_slot;
        if (servantData.equipmentDic.ContainsKey(type) == false)
        {
            Debug.Log("Invalid Servant EquipmentDic");
            return;
        }

        if (servantData.equipmentDic[type] == 0)
        {
            Debug.Log("Invalid Servant Already Equip Each Equipment");
            return;
        }

        if (UserDataManager.Inst.UnequipServant(servantData.index, type) == false)
        {
            Debug.Log("Invalid Servant Unequip");
            return;
        }

        if (SubViewEquipment.Inst != null)
        {
            SubViewEquipment.Inst.UpdateViewChangeEquipment();
            SubViewEquipment.Inst.ResetScrollDataByChangeSetData();
            if (PartyInfoVC.Inst != null)
            {
                PartyInfoVC.Inst.updateDetailInfo(PartyInfoVC.Inst.selected_unit_idx);
            }

        }
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
        UserDataManager.Inst.SetUserUTG(Convert.ToUInt64(getMonsterUpgradeResultData.utg));

        LobbyTopInfo.Inst.UpdateTopInfo();

        //SubViewUpgrade.Inst.updateViewFinishRequest();
        PartyInfoVC.Inst.updateViewFinishRequest();
    }

    // 아이템 강화
    public void ResponseEquipmentUpgrade(equipmentUpgradeResultData getEquipmentUpgradeResultData)
    {
        Debug.Log("장비 업그레이드 !");

        if (getEquipmentUpgradeResultData.is_success == true)
        {
            UserEquipmentData equipmentData = ParseEquipment(getEquipmentUpgradeResultData.main_equipment_data);
            if(equipmentData == null)
            {
                Debug.Log("Invalid Equipment Data : " + getEquipmentUpgradeResultData.main_equipment_data);
                return;
            }
            UserDataManager.Inst.SetEquipment(equipmentData);
        }
        else
        {
            if(UserDataManager.Inst.DelEquipment(getEquipmentUpgradeResultData.del_equipment_index) == false)
            {
                Debug.Log("Invalid Equipment Index : " + getEquipmentUpgradeResultData.del_equipment_index);
            }
        }

        for (int i = 0; i < getEquipmentUpgradeResultData.add_item_list.Count; i++)
        {
            UserItemData itemData = ParseItem(getEquipmentUpgradeResultData.add_item_list[i]);
            UserDataManager.Inst.SetItem(itemData);
        }

        UserDataManager.Inst.SetUserUTG(Convert.ToUInt64(getEquipmentUpgradeResultData.utg));

        LobbyTopInfo.Inst.UpdateTopInfo();

        SubViewUpgrade.Inst.updateViewFinishRequest();
        //PartyInfoVC.Inst.updateViewFinishRequest();
    }

    // 상점 아이템 구매
    public void ResponseShopBuyItem(shopBuyResultData getBuyItemData)
    {
        Debug.Log("상점 아이템 구매 !");

        UserDataManager.Inst.SetUserEOS(Convert.ToUInt64(getBuyItemData.eos));
        UserDataManager.Inst.SetUserUTG(Convert.ToUInt64(getBuyItemData.utg));

        for (int i = 0; i < getBuyItemData.item_list.Count; i++)
        {
            Debug.Log("item ID : " + getBuyItemData.item_list[i].id);
        }

        LobbyTopInfo.Inst.UpdateTopInfo();
        ShopInfoPage.Inst.OnClickPopupCancelButton();
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

    public void ResponseLobbyInfo(lobbyInfoResultData getLobbyInfo)
    {
        Debug.Log("로비 인포!");

        Debug.Log("Chat : " + getLobbyInfo.chat_string);
        Debug.Log("EOS : " + getLobbyInfo.eos);
        Debug.Log("UTG : " + getLobbyInfo.utg);
        Debug.Log("Mail : " + getLobbyInfo.mail_count);
        Debug.Log("CPU Limit : " + getLobbyInfo.resource_data.cpu_limit);
        Debug.Log("CPU Weight : " + getLobbyInfo.resource_data.cpu_weight);
        Debug.Log("NET Limit : " + getLobbyInfo.resource_data.net_limit);
        Debug.Log("NET Weight : " + getLobbyInfo.resource_data.net_weight);
        Debug.Log("RAM Usage : " + getLobbyInfo.resource_data.ram_usage);
        Debug.Log("RAM Quota : " + getLobbyInfo.resource_data.ram_quota);

        UserLobbyInfo lobbyInfo = new UserLobbyInfo();
        lobbyInfo.cpuLimit = getLobbyInfo.resource_data.cpu_limit;
        lobbyInfo.cpuWeight = getLobbyInfo.resource_data.cpu_weight;
        lobbyInfo.netLimit = getLobbyInfo.resource_data.net_limit;
        lobbyInfo.netWeight = getLobbyInfo.resource_data.net_weight;
        lobbyInfo.ramUsage = getLobbyInfo.resource_data.ram_usage;
        lobbyInfo.ramQuota = getLobbyInfo.resource_data.ram_quota;
        lobbyInfo.mailCount = getLobbyInfo.mail_count;
        lobbyInfo.chatting = getLobbyInfo.chat_string;

        UserDataManager.Inst.SetUserLobbyInfo(lobbyInfo);
        UserDataManager.Inst.SetUserEOS(Convert.ToUInt64(getLobbyInfo.eos));
        UserDataManager.Inst.SetUserUTG(Convert.ToUInt64(getLobbyInfo.utg));

        LobbyTopInfo.Inst.UpdateTopInfo();
        LobbyChatInfo.Inst.UpdateChatting();
    }

    public void ResponseMailList(mailListResultData mailList)
    {
        List<MailInfo> mailInfoList = new List<MailInfo>();

        for(int i = 0; i < mailList.mail_data_list.Count; i++)
        {
            Debug.Log("Mail Num : " + i);
            Debug.Log("Mail Index : " + mailList.mail_data_list[i].mail_index);
            Debug.Log("Mail TYPE : " + mailList.mail_data_list[i].mail_type);
            Debug.Log("Mail INDEX : " + mailList.mail_data_list[i].type_index);
            Debug.Log("Mail COUNT : " + mailList.mail_data_list[i].count);
            Debug.Log("Mail ICON ID : " + mailList.mail_data_list[i].icon_id);

            MailInfo mailInfo = ParseMailInfo(mailList.mail_data_list[i]);
            if(mailInfo == null)
            {
                Debug.Log("Invalid Mail Info");
                return;
            }
            mailInfoList.Add(mailInfo);
        }

        MailInfoPage.Inst.SetMailList(mailInfoList);
    }

    // 우편 수령
    public void ResponseMailOpen(mailOpenResultData getMailOpenResultData)
    {
        Debug.Log("메일 오픈 !");

        if(getMailOpenResultData == null)
        {
            Debug.Log("Invalid Result Data");
            return;
        }

        if (getMailOpenResultData.mail_open_index_list == null || getMailOpenResultData.mail_open_index_list.Count == 0)
        {
            Debug.Log("Invalid Result Index");
            return;
        }
        else
        {
            for (int i = 0; i < getMailOpenResultData.mail_open_index_list.Count; i++)
            {
                // 인덱스 삭제
                Debug.Log("Index : " + getMailOpenResultData.mail_open_index_list[i]);

                MailInfoPage.Inst.DelMail(getMailOpenResultData.mail_open_index_list[i]);
            }

            UserDataManager.Inst.DelMailCount(getMailOpenResultData.mail_open_index_list.Count);
        }

        if (getMailOpenResultData.servant_data_list != null && getMailOpenResultData.servant_data_list.Count > 0)
        {
            for (int i = 0; i < getMailOpenResultData.servant_data_list.Count; i++)
            {
                Debug.Log("Servant : " + getMailOpenResultData.servant_data_list[i].servant.id);

                UserServantData getServant = ParseServant(getMailOpenResultData.servant_data_list[i]);

                if (getServant.state != 4)
                {
                    UserDataManager.Inst.AddServantData(getServant);
                }
            }
        }

        if (getMailOpenResultData.monster_data_list != null && getMailOpenResultData.monster_data_list.Count > 0)
        {
            for (int i = 0; i < getMailOpenResultData.monster_data_list.Count; i++)
            {
                Debug.Log("Monster : " + getMailOpenResultData.monster_data_list[i].monster.id);

                UserMonsterData getMonster = ParseMonster(getMailOpenResultData.monster_data_list[i]);

                UserDataManager.Inst.AddMonsterData(getMonster);
            }
        }

        if (getMailOpenResultData.equip_data_list != null && getMailOpenResultData.equip_data_list.Count > 0)
        {
            for (int i = 0; i < getMailOpenResultData.equip_data_list.Count; i++)
            {
                Debug.Log("Equipment : " + getMailOpenResultData.equip_data_list[i].equipment.id);

                UserEquipmentData getEquipment = ParseEquipment(getMailOpenResultData.equip_data_list[i]);

                UserDataManager.Inst.AddEquipmentData(getEquipment);
            }
        }

        if (getMailOpenResultData.item_data_list != null && getMailOpenResultData.item_data_list.Count > 0)
        {
            for (int i = 0; i < getMailOpenResultData.item_data_list.Count; i++)
            {
                Debug.Log("Item : " + getMailOpenResultData.item_data_list[i].id);
            }
        }

        MailInfoPage.Inst.RefreshMailList();
    }

    // 타워 시작
    public void ResponseTowerStart(stageStateData getBattleStateData)
    {
        UserDataManager.Inst.GetUserInfo().sceneState = SCENE_STATE.PVP;

        UserStageStateData stageData = ParseStageStateData(getBattleStateData);
        if (stageData == null)
        {
            Debug.Log("Invalid StageData");
        }

        UserDataManager.Inst.SetStageState(stageData);
        StartCoroutine(LoadSceneAsync("CharacterBattleScene", "Now, Loading battle field ... "));
    }

    // 타워 나가기
    public void ResponseTowerExit()
    {

    }

    // 타워 보상
    public void ResponseTowerReward()
    {
        UserDataManager.Inst.InitStageInfo();
        SceneManager.LoadScene("Lobby");
        Time.timeScale = 1.0f;
    }

    // 채팅
    public void ResponseEnterChat()
    {

    }

    // PVP 시작
    public void ResponsePVPStart(stageStateData getBattleStateData)
    {
        UserDataManager.Inst.GetUserInfo().sceneState = SCENE_STATE.PVP;

        UserStageStateData stageData = ParseStageStateData(getBattleStateData);
        if (stageData == null)
        {
            Debug.Log("Invalid StageData");
        }

        UserDataManager.Inst.SetStageState(stageData);
        StartCoroutine(LoadSceneAsync("CharacterBattleScene", "Now, Loading battle field ... "));
    }

    // PVP 끝
    public void ResponsePVPResult(pvpResultData getPVPResult)
    {
        UserDataManager.Inst.GetUserInfo().sceneState = SCENE_STATE.Lobby;

        Debug.Log(getPVPResult.ToString());

        UserDataManager.Inst.InitStageInfo();
        SceneManager.LoadScene("Lobby");
        Time.timeScale = 1.0f;
    }

    public void ResponseShopInfo(shopInfoResultData getShopInfo)
    {
        Debug.Log("Response ShopInfo : " + getShopInfo.shop_type);

        List<ShopProductInfo> productInfoList = new List<ShopProductInfo>();
        ParseShopProductInfoList(getShopInfo, ref productInfoList);

        ShopInfoPage.Inst.SetShopInfo(productInfoList);
    }

    #endregion

    #region Function

    public bool ParseShopProductInfoList(shopInfoResultData getShopInfo, ref List<ShopProductInfo> productInfoList)
    {
        for (int i = 0; i < getShopInfo.shop_product_list.Count; i++)
        {
            Debug.Log("Index : " + getShopInfo.shop_product_list[i].index);
            Debug.Log("Type : " + getShopInfo.shop_product_list[i].type);
            Debug.Log("ID : " + getShopInfo.shop_product_list[i].id);
            Debug.Log("Limit Count : " + getShopInfo.shop_product_list[i].limit_count);

            ShopProductInfo info = ParseShopProduct(getShopInfo.shop_product_list[i]);
            productInfoList.Add(info);
        }

        return true;
    }

    public ShopProductInfo ParseShopProduct(shopProductData getProductData)
    {
        ShopProductInfo info = new ShopProductInfo();
        info.index = getProductData.index;
        info.type = (SHOP_TYPE)getProductData.type;
        info.id = getProductData.id;
        info.limitCount = getProductData.limit_count;

        return info;
    }

    public bool ParseUserInfo(UserLoginData getUserData, ref UserInfo userInfo)
    {
        userInfo.userName = getUserData.user_data.user;
        userInfo.userEXP = getUserData.user_data.exp;

        userInfo.userEOS = ulong.Parse(getUserData.eos);
        // token은 UTG로 바꿀 필요가 있지 않을까요
        userInfo.userUTG = ulong.Parse(getUserData.utg);
        userInfo.level = getUserData.user_data.rank;

        //Debug.Log("getEOS : " + getUserData.eos);
        //Debug.Log("getUTG : " + getUserData.token);

        userInfo.sceneState = (SCENE_STATE)getUserData.user_data.state;
        //Debug.Log("State : " + (SCENE_STATE)getUserData.user_data.state);

        userInfo.inventoryInfo.servantInventory = getUserData.user_data.inventory_info.servant_inventory;
        userInfo.inventoryInfo.monsterInventory = getUserData.user_data.inventory_info.monster_inventory;
        userInfo.inventoryInfo.equipmentInventory = getUserData.user_data.inventory_info.equipment_inventory;
        userInfo.inventoryInfo.itemInventory = getUserData.user_data.inventory_info.item_inventory;

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

            if (servant.state != 4)
            {
                servantDic.Add(servant.index, servant);
            }
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
        userServant.id = getServantData.servant.id;
        userServant.level = getServantData.servant.level;
        userServant.grade = getServantData.servant.grade;
        userServant.state = getServantData.servant.state;

        DBServantData servantData = CSVData.Inst.GetServantData(userServant.id);
        if (servantData == null)
        {
            Debug.Log("Invalid Servant ID : " + userServant.id);
            return null;
        }
        userServant.job = servantData.job;

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

        for(int i = 0; i < getServantData.servant.equip_slot.Count; i++)
        {
            userServant.equipmentDic.Add((EQUIPMENT_TYPE)i, getServantData.servant.equip_slot[i]);
        }

        for(int i = 0; i < getServantData.servant.active_skill.Count; i++)
        {
            // Active Skill Data 넣기
        }

        for (int i = 0; i < getServantData.servant.passive_skill.Count; i++)
        {
            // Passive Skill Data 넣기
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

            if (monster.state != 4)
            {
                monsterDic.Add(monster.index, monster);
            }
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
        monster.level = getMonsterData.monster.level;
        if(monster.level == 0)
        {
            Debug.LogError("Invalid Monster Level");
        }

        DBMonsterData monsterData = CSVData.Inst.GetMonsterData(monster.id);
        if(monsterData == null)
        {
            Debug.Log("Invalid Monster ID : " + monster.id);
            return null;
        }
        monster.monsterTribe = (int)monsterData.tribeType;
        monster.element = (int)monsterData.elementType;

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
            UserEquipmentData equipment = ParseEquipment(getEquipmentList[i]);
            if (equipment == null)
            {
                Debug.Log("Invalid Item Info");
                return false;
            }

            if (equipment.state != 4)
            {
                equipmentDic.Add(equipment.index, equipment);
            }
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

        DBEquipmentData dbEquipmentData = CSVData.Inst.GetEquipmentData(equipmentData.id);
        if(dbEquipmentData == null)
        {
            Debug.Log("Invalid Equipment Data : " + equipmentData.id);
            return null;
        }

        equipmentData.equipmentType = dbEquipmentData.equipmentType;

        equipmentData.grade = getEquipmentData.equipment.grade;
        equipmentData.upgrade = getEquipmentData.equipment.upgrade;
        equipmentData.state = getEquipmentData.equipment.state;

        equipmentData.optionType = dbEquipmentData.optionType;
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
            itemDic.Add(item.id, item);
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

        itemData.id = getItemData.id;
        for (int i = 0; i < getItemData.item_list.Count; i++)
        {
            UserItemInfo info = new UserItemInfo();
            info.index = getItemData.item_list[i].index;
            info.count = getItemData.item_list[i].count;
            itemData.itemInfoList.Add(info);
        }

        return itemData;
    }

    public List<UserSkillInfo> ParseSkillList(List<int> getSkillList)
    {
        List<UserSkillInfo> skillList = new List<UserSkillInfo>();

        for (int i = 0; i < getSkillList.Count; i++)
        {
            // TODO : 스킬 DB 읽어오기 추가
            UserSkillInfo skillInfo = new UserSkillInfo();
            skillInfo.id = getSkillList[i];
            skillList.Add(skillInfo);
        }
        return skillList;
    }

    public UserStageStateData ParseStageStateData(stageStateData getStageData)
    {
        UserStageStateData stageData = new UserStageStateData();
        stageData.user = getStageData.user;
        stageData.enemyUser = getStageData.enemy_user;
        stageData.stageType = getStageData.stage_type;
        stageData.stageFloor = getStageData.stage_number;
        stageData.turn = getStageData.turn;
        stageData.mySynergyList = ParseSkillList(getStageData.my_synergy_list);
        stageData.enemySynergyList = ParseSkillList(getStageData.enemy_synergy_list);

        for (int i = 0; i < getStageData.my_state_list.Count; i++)
        {
            UserCharacterStateData stateData = ParseCharacterStateData(getStageData.my_state_list[i]);
            if(stateData == null)
            {
                return null;
            }

            stageData.myStateList.Add(stateData.position, stateData);
        }

        bool isMonster = false;
        if(stageData.stageType == 0)
        {
            isMonster = true;
        }

        for (int i = 0; i < getStageData.enemy_state_list.Count; i++)
        {
            UserCharacterStateData stateData = ParseCharacterStateData(getStageData.enemy_state_list[i], isMonster);
            if (stateData == null)
            {
                return null;
            }

            stageData.enemyStateList.Add(stateData.position, stateData);
        }


        return stageData;
    }

    public UserCharacterStateData ParseCharacterStateData(characterStateData getStateData, bool isMonster = false)
    {
        UserCharacterStateData stateData = new UserCharacterStateData();

        stateData.id = getStateData.id;
        stateData.grade = getStateData.grade;
        stateData.position = getStateData.position;
        stateData.index = getStateData.index;
        stateData.nowHp = getStateData.now_hp;
        stateData.state = getStateData.state;
        stateData.type = getStateData.type;
        stateData.upgrade = getStateData.upgrade;
        stateData.maxHP = getStateData.max_hp;
        stateData.level = getStateData.level;
        stateData.status = ParseStatus(getStateData.status);
        stateData.buffList = getStateData.buff_list;
        stateData.activeSkillList = ParseSkillList(getStateData.active_skill_list);
        stateData.passiveSkillList = ParseSkillList(getStateData.passive_skill_list);

        if (stateData.charType == CHAR_TYPE.SERVANT)
        {
            DBServantData servantData = CSVData.Inst.GetServantData(getStateData.id);
            if(servantData == null)
            {
                Debug.Log("Invalid Data");
                return null;
            }
            stateData.criPer = servantData.criPer;
            stateData.mCriPer = servantData.mcriPer;
            stateData.criDmg = servantData.criDmg;
            stateData.mCriDmg = servantData.mcriDmg;
            stateData.avoid = servantData.avoid;
            stateData.speed = servantData.speed;
            stateData.job = servantData.job;
        }
        else
        {
            DBMonsterData monsterData = CSVData.Inst.GetMonsterData(getStateData.id);
            if (monsterData == null)
            {
                Debug.Log("Invalid Data");
                return null;
            }
            stateData.criPer = monsterData.criPer;
            stateData.mCriPer = monsterData.mcriPer;
            stateData.criDmg = monsterData.criDmg;
            stateData.mCriDmg = monsterData.mcriDmg;
            stateData.avoid = monsterData.avoid;
            stateData.speed = monsterData.speed;
            stateData.tribeType = (int)monsterData.tribeType;
            stateData.elementType = (int)monsterData.elementType;
        }

        return stateData;
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

    public MailInfo ParseMailInfo(mailData getMailData)
    {
        MailInfo mailInfo = new MailInfo();
        mailInfo.index = getMailData.mail_index;
        mailInfo.type = (MAIL_TYPE)getMailData.mail_type;
        mailInfo.typeIndex = getMailData.type_index;
        mailInfo.count = getMailData.count;

        switch(mailInfo.type)
        {
            case MAIL_TYPE.PRE_SERVANT:
            case MAIL_TYPE.NFT_SERVANT:
                DBServantData dbServantData = CSVData.Inst.GetServantData(getMailData.icon_id);
                if (dbServantData == null)
                {
                    Debug.Log("Invalid Servant Icon ID : " + getMailData.icon_id);
                    return null;
                }
                mailInfo.name = dbServantData.name;
                mailInfo.resourceIcon = dbServantData.servantIcon;
                break;
            case MAIL_TYPE.PRE_MONSTER:
            case MAIL_TYPE.NFT_MONSTER:
                DBMonsterData dbMonsterData = CSVData.Inst.GetMonsterData(getMailData.icon_id);
                if(dbMonsterData == null)
                {
                    Debug.Log("Invalid Monster Icon ID : " + getMailData.icon_id);
                    return null;
                }
                mailInfo.name = dbMonsterData.name;
                mailInfo.resourceIcon = dbMonsterData.monsterIcon;
                break;
            case MAIL_TYPE.PRE_EQUIPMENT:
            case MAIL_TYPE.NFT_EQUIPMENT:
                DBEquipmentData dbEquipmentData = CSVData.Inst.GetEquipmentData(getMailData.icon_id);
                if(dbEquipmentData == null)
                {
                    Debug.Log("Invalid Equipment Icon ID : " + getMailData.icon_id);
                }
                mailInfo.name = dbEquipmentData.name;
                mailInfo.resourceIcon = dbEquipmentData.equipmentIcon;
                break;
            case MAIL_TYPE.UTG:
                DBItemData dbItemData = CSVData.Inst.GetItemData(500001);
                if (dbItemData == null)
                {
                    Debug.Log("Invalid UTG Icon ID : " + 500001);
                }
                mailInfo.name = dbItemData.name;
                mailInfo.resourceIcon = dbItemData.ItemIcon;
                break;
            case MAIL_TYPE.ETC_ITEM:
                DBItemData dbETCItemData = CSVData.Inst.GetItemData(getMailData.icon_id);
                if (dbETCItemData == null)
                {
                    Debug.Log("Invalid ETC Item Icon ID : " + getMailData.icon_id);
                }
                mailInfo.name = dbETCItemData.name;
                mailInfo.resourceIcon = dbETCItemData.ItemIcon;
                break;
            default:
                Debug.Log("Invalid Mail Type");
                return null;
        }

        return mailInfo;
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

