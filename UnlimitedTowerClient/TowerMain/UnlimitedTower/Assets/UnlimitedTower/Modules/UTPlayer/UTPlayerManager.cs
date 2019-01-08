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
            JsonData data = Cheat.Inst.GetUserLoginData(user, gameMoney);

            return data.ToString();
        }
    }

    public class UTGachaData : IJSONableData
    {
        public string ToJson()
        {
            JsonData data = Cheat.Inst.GetGachaResultData();

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
    /// 배틀씬에서 상대 유저가 입장헀을때 발생하는 이벤트 리스너입니다.
    /// </summary>
    public event Listener OnEnemyJoined;

    //현재 로그인중인 유저의 정보를 가져옵니다.
    public UTPlayerData thisUser { get; private set; }
    public UTGachaData thisGacha { get; private set; }

    //현재 배틀씬에서 상대방의 유저 정보를 가져옵니다.
    public UTPlayerData otherUserInThisBattle { get; private set; }


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
                    PacketManager.Inst.ResponseLogin(loginInfo);
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
                    thisUser = data as UTPlayerData ?? thisUser;
                    Debug.Log("[SUCCESS] user saveParty :" + thisUser.ToJson());
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

            OnEnemyJoined += (data) =>
            {
                if (data is UTFailedData)
                {
                    Debug.LogWarning("[FAILED MESSAGE]" + (data as UTFailedData).msg);
                }
                else
                {
                    otherUserInThisBattle = data as UTPlayerData ?? otherUserInThisBattle;
                }
            };

            //이벤트들을 등록합니다.
            UTEventPoolInterface.AddEventListener("login", OnLogin);
            UTEventPoolInterface.AddEventListener("gacha", OnGacha);
            UTEventPoolInterface.AddEventListener("saveparty", OnSaveParty);
            UTEventPoolInterface.AddEventListener("logout", OnLogout);
            UTEventPoolInterface.AddEventListener("enemyJoined", OnEnemyJoined);
        }
    }
}
