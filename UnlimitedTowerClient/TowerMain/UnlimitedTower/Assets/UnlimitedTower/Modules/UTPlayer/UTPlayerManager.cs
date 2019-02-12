using UnityEngine;
using LitJson;
using Listener = System.Action<IJSONableData>;

public class UTPlayerManager : MonoBehaviour {

    // 싱글톤
    static public UTPlayerManager Instance = null;
    private void Awake()
    {
        //이 객체는 게임이 완전히 종료될때 까지 유지됩니다.
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    [SerializeField]
    /// <summary>
    /// 해당 구조체는 전반적인 플레이어의 정보를 포함합니다.
    /// </summary>
    public class UTPlayerData : IJSONableData
    {
        public string user;
        public int gameMoney;

        public string ToJson()
        {
            //JsonData data = Cheat.Inst.GetUserLoginData(user, gameMoney);
            JsonData data = Cheat.Inst.TestGetUserLoginData(user);

            return data.ToString();
        }
    }

    //파티 저장 로컬 테스트용
    public class UTPartyData : IJSONableData
    {
        public int partyNum;

        public string ToJson()
        {
            //JsonData data = Cheat.Inst.GetUserLoginData(user, gameMoney);
            JsonData data = Cheat.Inst.TestGetPartyData(partyNum);

            return data.ToString();
        }
    }

    [SerializeField]
    public class UTGachaData : IJSONableData
    {
        public string ToJson()
        {
            JsonData data = Cheat.Inst.GetGachaResultData();

            return data.ToString();
        }
    }

    [SerializeField]
    public class UTBattleActionData : IJSONableData
    {
        public int heroTarget;
        public int heroAction;
        public int monsterTarget;
        public int monsterAction;

        public string ToJson()
        {
            //JsonData data = Cheat.Inst.GetBattleActionData(heroTarget, heroAction, monsterTarget, monsterAction);
            JsonData data = Cheat.Inst.TestGetBattleActionData("devtooth",heroTarget, heroAction, monsterTarget, monsterAction);
            return data.ToString();
        }
    }

    [SerializeField]
    public class UTStageStartData : IJSONableData
    {
        public int stageNum;
        public int partyNum;

        public string ToJson()
        {
            //JsonData data = Cheat.Inst.GetStageStartData(stageNum, partyNum);
            JsonData data = Cheat.Inst.TestGetStageStartData("devtooth", stageNum, partyNum);
            return data.ToString();
        }
    }

    [SerializeField]
    public class UTStageResultData : IJSONableData
    {
        public int stageNum;

        public string ToJson()
        {
            JsonData data = Cheat.Inst.GetStageResultData(stageNum);

            return data.ToString();
        }
    }

    [SerializeField]
    public class UTTowerStartData : IJSONableData
    {
        public string ToJson()
        {
            JsonData data = Cheat.Inst.GetTowerStartData();

            return data.ToString();
        }
    }

    [SerializeField]
    public class UTTowerResultData : IJSONableData
    {
        public string ToJson()
        {
            JsonData data = Cheat.Inst.GetTowerResultData();

            return data.ToString();
        }
    }


    /// <summary>
    /// 로그인했을 때 발생하는 이벤트 리스너입니다.
    /// </summary>
    public event Listener OnLogin;

    /// <summary>
    /// 가챠를 실행했을 때 발생하는 이벤트 리스너입니다.
    /// </summary>
    public event Listener OnGacha;

    /// <summary>
    /// 파티 편성을 변경했을 때 발생하는 이벤트 리스너입니다.
    /// </summary>
    public event Listener OnSaveParty;

    /// <summary>
    /// 로그아웃했을 때 발생하는 이벤트 리스너입니다.
    /// </summary>
    public event Listener OnLogout;

    /// <summary>
    /// 전투씬에서 액션을 입력했을 때 발생하는 이벤트 리스너입니다.
    /// </summary>
    public event Listener OnBattleAction;

    /// <summary>
    /// 스테이지 진입했을 때 발생하는 이벤트 리스너입니다.
    /// </summary>
    public event Listener OnStageStart;

    /// <summary>
    /// 스테이지가 끝났을 때 발생하는 이벤트 리스너입니다.
    /// </summary>
    public event Listener OnStageResult;

    /// <summary>
    /// 타워 전투 진입했을 때 발생하는 이벤트 리스너입니다.
    /// </summary>
    public event Listener OnTowerStart;

    /// <summary>
    /// 타워 전투가 끝났을 때 발생하는 이벤트 리스너입니다.
    /// </summary>
    public event Listener OnTowerResult;

    //현재 저장하려는 파티 정보를 가져 옵니다.
    public UTPartyData thisParty { get; private set; }

    //현재 로그인중인 유저의 정보를 가져옵니다.
    public UTPlayerData thisUser { get; private set; }
    
    //가챠 결과 정보를 가져옵니다.
    public UTGachaData thisGacha { get; private set; }

    //전투 액션 정보를 가져옵니다.
    public UTBattleActionData thisBattleAction { get; private set; }

    //스테이지 시작 정보를 가져옵니다.
    public UTStageStartData thisStageStart { get; private set; }

    //스테이지 결과 정보를 가져옵니다.
    public UTStageResultData thisStageResult { get; private set; }

    //타워 시작 정보를 가져옵니다.
    public UTTowerStartData thisTowerStart { get; private set; }

    //타워 결과 정보를 가져옵니다.
    public UTTowerResultData thisTowerResult { get; private set; }

    private void Start()
    {
        //이 객체가 싱글톤인 경우 (아닌경우 어떠한 동작도 하지 않습니다.)
        if(Instance == this)
        {
            // 기본 리스너를 정의합니다

            OnLogin += (data) =>
            {
                if(data is UTFailedData)
                {
                    Debug.LogWarning("[FAILED MESSAGE]" + (data as UTFailedData).msg);
                }
                else
                {
                    thisUser = data as UTPlayerData ?? thisUser;
                    string loginInfo = thisUser.ToJson();
                    Debug.Log("[SUCCESS] user login :" + loginInfo);
                    //PacketManager.Inst.ResponseLogin(loginInfo);
                    PacketManager.Inst.TestResponseLogin(loginInfo);
                }
            };

            OnGacha += (data) =>
            {
                if (data is UTFailedData)
                {
                    Debug.LogWarning("[FAILED MESSAGE]" + (data as UTFailedData).msg);
                }
                else
                {
                    thisGacha = data as UTGachaData ?? thisGacha;
                    string gachaInfo = thisGacha.ToJson();
                    Debug.Log("[SUCCESS] user gacha :" + gachaInfo);
                    PacketManager.Inst.ResponseGacha(gachaInfo);
                }
            };

            OnSaveParty += (data) =>
            {
                if (data is UTFailedData)
                {
                    Debug.LogWarning("[FAILED MESSAGE]" + (data as UTFailedData).msg);
                }
                else
                {
                    //thisUser = data as UTPlayerData ?? thisUser;
                    //Debug.Log("[SUCCESS] user saveParty :" + thisUser.ToJson());
                    thisParty = data as UTPartyData ?? thisParty;
                    Debug.Log("[SUCCESS] party saveParty :" + thisParty.ToJson());
                    //원래 리스폰스 함수는 없었음
                }
            };

            OnLogout += (data) =>
            {
                if (data is UTFailedData)
                {
                    Debug.LogWarning("[FAILED MESSAGE]" + (data as UTFailedData).msg);
                }
                else
                {
                    thisUser = data as UTPlayerData ?? thisUser;
                    Debug.Log("[SUCCESS] user logout :" + thisUser.ToJson());
                }
            };

            OnBattleAction += (data) =>
            {
                if (data is UTFailedData)
                {
                    Debug.LogWarning("[FAILED MESSAGE]" + (data as UTFailedData).msg);
                }
                else
                {
                    thisBattleAction = data as UTBattleActionData ?? thisBattleAction;
                    string battleActionInfo = thisBattleAction.ToJson();
                    Debug.Log("[SUCCESS] user battleaction :" + battleActionInfo);
                    PacketManager.Inst.ResponseBattleAction(battleActionInfo);
                }
            };

            OnStageStart += (data) =>
            {
                if (data is UTFailedData)
                {
                    Debug.LogWarning("[FAILED MESSAGE]" + (data as UTFailedData).msg);
                }
                else
                {
                    thisStageStart = data as UTStageStartData ?? thisStageStart;
                    string stageStartInfo = thisStageStart.ToJson();
                    Debug.Log("[SUCCESS] user stagestart :" + stageStartInfo);
                    PacketManager.Inst.ResponseStageStart(stageStartInfo);
                }
            };

            OnStageResult += (data) =>
            {
                if (data is UTFailedData)
                {
                    Debug.LogWarning("[FAILED MESSAGE]" + (data as UTFailedData).msg);
                }
                else
                {
                    thisStageResult = data as UTStageResultData ?? thisStageResult;
                    string stageResultInfo = thisStageResult.ToJson();
                    Debug.Log("[SUCCESS] user stageresult :" + stageResultInfo);
                    PacketManager.Inst.ResponseStageResult(stageResultInfo);
                }
            };

            OnTowerStart += (data) =>
            {
                if (data is UTFailedData)
                {
                    Debug.LogWarning("[FAILED MESSAGE]" + (data as UTFailedData).msg);
                }
                else
                {
                    thisTowerStart = data as UTTowerStartData ?? thisTowerStart;
                    string towerStartInfo = thisTowerStart.ToJson();
                    Debug.Log("[SUCCESS] user towerstart :" + towerStartInfo);
                    PacketManager.Inst.ResponseTowerStart(towerStartInfo);
                }
            };

            OnTowerResult += (data) =>
            {
                if (data is UTFailedData)
                {
                    Debug.LogWarning("[FAILED MESSAGE]" + (data as UTFailedData).msg);
                }
                else
                {
                    thisTowerResult = data as UTTowerResultData ?? thisTowerResult;
                    string towerResultInfo = thisTowerResult.ToJson();
                    Debug.Log("[SUCCESS] user towerresult :" + towerResultInfo);
                    PacketManager.Inst.ResponseTowerResult(towerResultInfo);
                }
            };

            //이벤트들을 등록합니다.
            UTEventPoolInterface.AddEventListener("login", OnLogin);
            UTEventPoolInterface.AddEventListener("gacha", OnGacha);
            UTEventPoolInterface.AddEventListener("saveparty", OnSaveParty);
            UTEventPoolInterface.AddEventListener("logout", OnLogout);
            UTEventPoolInterface.AddEventListener("battleaction", OnBattleAction);
            UTEventPoolInterface.AddEventListener("stagestart", OnStageStart);
            UTEventPoolInterface.AddEventListener("stageresult", OnStageResult);
            UTEventPoolInterface.AddEventListener("towerstart", OnTowerStart);
            UTEventPoolInterface.AddEventListener("towerresult", OnTowerResult);
        }
    }
}
