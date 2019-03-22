using System.Collections.Generic;
using UnityEngine;
using System;

#region Extensions

static public class ActiveAnimationManager
{
    static public void SetActivateWithAnimation(this GameObject uiGO, bool value)
    {
        var animator = uiGO.GetComponent<Animator>();
        if (animator != null)
        {
            uiGO.SetActive(true);
            animator.SetTrigger(value ? "SetVisible" : "SetInvisible");
        }
        else
        {
            uiGO.SetActive(value);
        }
    }
}

#endregion

#region DEFINE

//공통 사항을 모아둔 클래스.
public class DEFINE
{
    public static readonly int MonsterMinFormationNum = 5;
    public static readonly int MonsterMaxFormationNum = 9;
    public static readonly int ServantMinFormationNum = 0;
    public static readonly int ServantMaxFormationNum = 4;
    public static readonly int MAINHERO_FORMINDEX = 2;

	// MIN - MAX Status
	public static readonly int MIN_STATUS = 0;
	public static readonly int MAX_STATUS = 9;
    public static readonly int RAND_STATUS = 10;
    public static readonly int MAX_LEVEL = 50;
    public static readonly int MAX_EXP = 5000;
    public static readonly int BONUS_STAT = 1;

    // SHADER Name
    public static readonly string OUTLINE_SHADER = "Outlined/UltimateOutline";
    public static readonly string BASIC_SHADER = "Mobile/Unlit (Supports Lightmap)";

    // Status Formula
    public static readonly int HP_MIN_VALUE = 300;
    public static readonly int HP_MULTI_VALUE = 22;
    public static readonly int DAMAGE_MUlTI_VALUE = 2;
    public static readonly int DEFENCE_MULTI_VALUE = 1;
    public static readonly int CRITICAL_PER_MULTI_VALUE = 1;
    public static readonly float AVOID_MULTI_VALUE = 0.1f;


    // Party Info
    public static readonly int PARTY_MAX_NUM = 10;

    // Battle Position
    public static readonly float START_X = -1.5f;
    public static readonly float START_Z = -5.0f;
    public static readonly float DISTANCE_OFFSET = 1.0f;

    // 캐릭터 끼리 최소 만나기 직전의 최소 거리
    public static readonly float STOP_DISTANCE = 0.5f;

    // 공격 끝나고 되돌아 올때 거리 보정하는 기준.
    public static readonly float MIN_DISTANCE = 0.3f;

    // 움직이는 속도
    public static readonly float MOVING_SPEED = 4.0f;
    public  static float ANIMATION_SPEED = 2.0f;

    // 테스트 상수
    public static Vector3 PLAYER_BACKLINE_CENTER_POS = new Vector3(0.0f, 0.0f, -5.0f);
    public static Vector3 ENEMY_BACKLINE_CENTER_POS = new Vector3(0.0f, 0.0f, 5.0f);

    public static readonly int HERO_FORMATION_NUM = 7;
    public static readonly int TEST_STATUS_VALUE = 1;



    public static Vector3 GetBattlePosition(int partyIndex, FORMATION_TYPE type, int num)
    {
        Vector3 position = new Vector3();
        int posOffset = 0;
        float y = 0.0f;
        if(type == FORMATION_TYPE.PLAYER)
        {
            posOffset = 1;
        }
        else
        {
            posOffset = -1;
        }

        switch (partyIndex)
        {
            case 0:
                position = new Vector3(START_X + DISTANCE_OFFSET * 1, y, START_Z * posOffset + 1.0f);
                break;
            case 1:
                position = new Vector3(START_X + DISTANCE_OFFSET * 2, y, START_Z * posOffset + 1.0f);
                break;
            case 2:
                position = new Vector3(START_X + DISTANCE_OFFSET * 3, y, START_Z * posOffset + 1.0f);
                break;
            case 3:
                position = new Vector3(START_X + DISTANCE_OFFSET * 1, y, START_Z * posOffset);
                break;
            case 4:
                position = new Vector3(START_X + DISTANCE_OFFSET * 2, y, START_Z * posOffset);
                break;
            case 5:
                position = new Vector3(START_X + DISTANCE_OFFSET * 3, y, START_Z * posOffset);
                break;
        }

        return position;
    }

    //임시코드 : 추후에 적절하게 수정 필요
    public static SIZE_TYPE GetCharacterSize(CHARACTER_NUM charType)
    {
        SIZE_TYPE charSize;

        if (charType == CHARACTER_NUM.Mst_Knight || charType == CHARACTER_NUM.Mst_Anubis || charType == CHARACTER_NUM.Mst_DarkKnight
            || charType == CHARACTER_NUM.Mst_WolfMan || charType == CHARACTER_NUM.Mst_Robot || charType == CHARACTER_NUM.Mst_Minotaurus
            || charType == CHARACTER_NUM.Mst_Robot_2)
        {
            charSize = SIZE_TYPE.MIDDLE;
        }
        else if (charType == CHARACTER_NUM.Mst_Death || charType == CHARACTER_NUM.Mst_Giant || charType == CHARACTER_NUM.Mst_BirdMan)
        {
            charSize = SIZE_TYPE.BIG;
        }
        else
        {
            charSize = SIZE_TYPE.SMALL;
        }
        return charSize;
    }    
}

#endregion

#region Info

[Serializable]
public class NodeInfo
{
    public NODE_LIST nodeName;
    public int nodeSpeed; // 00 ms
}

// 서번트 클래스에 히어로도 포함된다.
// TODO : Servant 삭제 영웅 경험치만 포함으로 변경
[Serializable]
public class UserInfo
{
    public string userName;
    public ulong userMoney;
    public ulong userEOS;
    public UserServantData userHero = new UserServantData();
    public SCENE_STATE sceneState;
}

// erd완
[Serializable]
public class UserServantData
{
    // 캐릭터를 구분하는 고유값이 되어야함
    public int index;

    // 영웅서번트인지 아닌지
    public bool isLegend;

    public string name;
    public int partyIndex;

    // TODO : Servant별 State 추가 필요
    public int exp;
    public int level
    {
        get
        {
            return Calculator.GetLevelForExp(exp);
        }
    }
    public int jobNum;

    // 현재는 appear를 통해 간단히 사용하고 추후 appearInfo와 job을 통해 캐릭터 생성이 되어야함
    public int body;
    public int headNum;
    public int hairNum;
    public int gender;

    public Status status = new Status();

    /* 조회용 데이터 __________________________________________________________ */

    // 착용 아이템 리스트 -> 역참조초기화x, 역참조업데이트x
    public List<UserMountItemData> mountItemList = new List<UserMountItemData>();
    public event System.Action mountItemListChangeEvent;
    public void Mount(UserMountItemData usermountitemdata)
    {
        mountItemList.Add(usermountitemdata);

        if (mountItemListChangeEvent != null)
        {
            mountItemListChangeEvent();
        }
    }
    public void Demount(UserMountItemData usermountitemdata)
    {
        // 장착하고있는 아이템이 아닐경우 -> return
        if (!mountItemList.Contains(usermountitemdata))
        {
            return;
        }

        // 장착 해제
        mountItemList.Remove(usermountitemdata);

        if (mountItemListChangeEvent != null)
        {
            mountItemListChangeEvent();
        }
    }

    // 배치 데이터 
    public bool isPlaced;       
    public int partyNum;
    public int formationNum;
    




    public UserServantData()
    {
        exp = 0;

        status = new Status();
        status.basicStr = DEFINE.TEST_STATUS_VALUE;
        status.basicDex = DEFINE.TEST_STATUS_VALUE;
        status.basicInt = DEFINE.TEST_STATUS_VALUE;
    }
    public UserServantData(int getCharNum)
    {
        body = getCharNum;
        exp = 0;

        status = new Status();
        status.basicStr = DEFINE.TEST_STATUS_VALUE;
        status.basicDex = DEFINE.TEST_STATUS_VALUE;
        status.basicInt = DEFINE.TEST_STATUS_VALUE;

    }
}

[System.Serializable]
public class UserMonsterData
{
    // 서버에서 캐릭터를 구분하는 고유값
    public int index;

    // 몬스터ID
    public int id;

    // TODO : 추후 추가 예정
    public int monsterTypeNum;

    // TODO : Monster별 State 추가 필요
    public int gradeNum;
    public int upgradeCount;
    public int exp;
    public int level
    {
        get
        {
            return Calculator.GetLevelForExp(exp);
        }
    }

    public Status status = new Status();

    public bool isPlaced;
    public int teamNum;
    public int formationNum;
}

[System.Serializable]
public class UserMountItemData
{
    // 
    public int index;

    //
    public int mountitemNum;

    public int gradeNum;
    public int tierNum;
    public int upgradeCount;
    public int value;

    bool _isMounted;
    public bool isMounted
    {
        get
        {
            return _isMounted;
        }
        set
        {
            _isMounted = value;

            if (mountedChanged != null)
            {
                mountedChanged();
            }
        }
    }
    public System.Action mountedChanged;

    public int mountServantIndex;
}

[System.Serializable]
public class Status
{
    public int basicStr;
    public int basicDex;
    public int basicInt;

    public int plusStr;
    public int plusDex;
    public int plusInt;
}

[System.Serializable]
public class UserFormationData
{
    // 인덱스
    public int index;

    // 파티 정보를 가져올것인지 포메이션 정보를 가져올것인지... 포메이션 정보를 가져올꺼면 파티 하나당 10개의 정보가 필요한게 아닌지?
    public int formationIndex;

    public CHAR_TYPE charType
    {
        get
        {
            if (formationIndex == 0)
            {
                return CHAR_TYPE.HERO;
            }

            if (formationIndex <= 4)
            {
                return CHAR_TYPE.SERVANT;
            }

            return CHAR_TYPE.MONSTER;
        }
    }
}




[System.Serializable]
public class UserEtcItemData
{
    public int index;

    public int etcItemNum;
    public int Count;
}

[System.Serializable]
public class UserPartyData
{
    public int partyIndex;
    public int partyState;

    public Dictionary<int, UserFormationData> formationDataDic = new Dictionary<int, UserFormationData>();
}

public class PartyCharacterInfo
{
    public CHAR_TYPE type;
    public int partyPosition;
    public int index;
}

// 배틀시 각 캐릭터 정보
public class CharacterBattleStatus
{
    public int partyIndex;
    public int index;

    public int maxHp;
    public int nowHp;

    public int damage;
    public int defense;

    public int avoid;
    public int speed;

    // 공격 타겟의 파티 인덱스
    public int targetIndex;
   
    // 살아있는가
    public bool lived;

    // 캐릭터 상태
    public STATE_TYPE stateType;

    // 캐릭터 크기
    public SIZE_TYPE sizeType;

    public string name;

    public int level;
}

public class CharacterAction
{
    public int myIndex;
    public int targetIndex;
    public ACTION_TYPE actionType;
    public FORMATION_TYPE formationType;
    public CharacterAction(int _myIndex, int _targetIndex, ACTION_TYPE _actionType, FORMATION_TYPE _formationType)
    {
        myIndex = _myIndex;
        targetIndex = _targetIndex;
        actionType = _actionType;
        formationType = _formationType;
    }
}

public static class ExtensionMethod
{
    public static T GetReferenceTo<T>(this GameObject go, T instance)
    {
        if (instance == null)
        {
            instance = go.GetComponent<T>();
        }
        return instance;
    }
    public static T CheckReferenceTo<T>( T instance)
    {
        return instance;

    }
    public static bool IsNull(this object obj)//literally all i did was add "this"
    {
        return obj == null || ReferenceEquals(obj, null) || obj.Equals(null);
    }
}

#endregion

#region ENUM

public enum LOCALIZATION_TYPE
{
    KR = 0,
    CH = 1,
    EN = 2,
}

public enum NODE_LIST
{
    ITAM = 0,
    EOSYS = 1,
    EOSeoul = 2,
    MAX = 3,
}

// 유닛타입
public enum UNIT_TYPE
{
    SERVANT,
    MONSTER
}

public enum PARTY_STATE
{
    FREE,
    FIXED
}
// Battle Formation Type
public enum FORMATION_TYPE
{
    PLAYER = 0,
    ENEMY
}

public enum CHAR_TYPE
{
    HERO    = 1,
    SERVANT = 2,
    MONSTER = 3
}

public enum STATE_TYPE
{
    NONE,
    IDLE,
    RUN,
    RETURN,
    ATK,
    DEF,
    SKILL,
    HIT,
    DIE
}

public enum BATTLE_STATE
{
    NONE,
    USER_TURN,
    BATTLE,
    END,
}

public enum CHARACTER_NUM
{
    NULL                = 0,

    Girl_Kid_1          = 1,
    Girl_Kid_2          = 2,

    Man_Kid_1           = 101,
    Man_Kid_2,
    Man_Kid_3,
    Man_Kid_4,

    Hero_Novice_1001    = 105,
    Hero_Novice_1002,
    Hero_Novice_1003,
    Hero_Novice_1004,


    Mst_Cat             = 201,
    Mst_Demon,
    Mst_Giant,
    Mst_Anubis,
    Mst_Ninja,
    Mst_Fire_Fairy,
    Mst_Leaf_Fairy,
    Mst_Water_Fairy,
    Mst_Death,
    Mst_WolfMan,
    Mst_Knight,
    Mst_DarkMagician,
    Mst_Egg,
    Mst_Goblin_Blue,
    Mst_Goblin_Green,
    Mst_Card_Black,
    Mst_Card_Green,
    Mst_Card_Red,
    Mst_Card_Yellow,
    Mst_Robot,
    Mst_Robot_2,
    Mst_Gangsi,         
    Mst_Zombie ,         
    Mst_DarkKnight,      
    Mst_BirdMan,         
    Mst_ShadowCat,
    Mst_Minotaurus,
    Mst_Meka,
    Mst_Snail
}

public enum SERVANT_JOB
{
    // STR
    WhiteHand = 0,
    Warrior = 1,

    // DEX
    Thief = 2,
    Archer = 4,

    // INT
    Cleric = 3,
    Magician = 5,
}

public enum ACTION_TYPE
{
    None = 0,
    Attack = 1,
    Defence = 2,
    Skill_1 = 3,
    Skill_2 = 4,
    Skill_3 = 5,
}
public enum MOVE_TYPE
{
    RUN= 0,
    RETURN = 1
}
public enum SIZE_TYPE
{
    SMALL = 0,
    MIDDLE = 1,
    BIG = 2
}

public enum LOBBY_RIGHT_BUTTON
{
    None = 0,
    Hero = 1,
    Partner = 2,
    Formation = 3,
    Gacha = 4,
    Stage = 5,
    Inventory = 6,
    Setting = 7,
    Max = 8,
}

public enum SCENE_STATE
{
    None = 0,
    Login = 1,
    Lobby = 2,
    Stage = 3,
    StageBattle = 4,
    TowerBattle = 5,
    Max = 6,
}


//  가챠 타입은 무료, 특수, 이벤트 등을 의미한다.
public enum GACHA_TYPE
{
    // TODO : 예시로 쓴 부분임
    Default,
    Specail
}
public enum GACHA_RESULT_TYPE
{
    Servant = 1,
    Monster,
    Item,
}

public enum APPEAR_HAIR
{
    BASE = 1,

    MAX = 4
}

public enum APPEAR_HEAD
{
    BASE = 1,

    MAX = 4
}

public enum APPEAR_BODY
{
    BASE = 0,

    MAX = 3
}

public enum STATUS_TYPE
{
    STR = 0,
    DEX = 1,
    INT = 2
}

public enum POPUP_STATE
{
    // Hero
    Hero = 0,
    Servant = 1,
    Monster = 2,
    Formation = 3,

    // Inventory
    Weapon = 10,
    Armor = 11,
    Accesory = 12,
    ETC = 13,

    // Shop
    EOS = 20,
    UTG = 21,
    Gacha = 22,
}

#endregion

#region DBClass

[Serializable]
public class DBItemData
{
    public int id;
    public string name;
    public string resourceIcon;
    public string description;
    public int tier;
    public string itemType;
    public List<int> itemParamIDList;
}

[Serializable]
public class DBMonsterData
{
    public int id;
    public string name;
    public int elementType;
    public int classType;
    public string resourceModel;
    public string resourceIcon;
}

[Serializable]
public class DBServantData
{
    public int id;
    public string name;
    public string job;
    public string resourceBody;
    public string resourceHead;
    public string resourceHair;
    public string resourceIcon;
}

[Serializable]
public class DBLocalizationData
{
    public int id;
    public string krText;
    public string chText;
    public string enText;
}

[Serializable]
public class DBStageData
{
    public int id;
    public int stageType;
    public int stageFloor;
    public string stageString;
    public int needStageId;
    public int stageGroupIndex;
    public int needEntranceItemId;
    public int needEntranceItemCount;
    public int enemyLevelMin;
    public int enemyLevelMax;
    public List<int> enemyIdList;
    public List<int> enemyPositionList;
    public int bossLevel;
    public List<int> bossPassiveList;
    public List<int> bossActiveList;
    public int rankExp;
    public int charExp;
    public List<int> rewardIdList;
    public List<int> rewardCountList;
    public int firstRewardId;
    public int firstRewardCount;
    public string mapResource;
    public int bgmSoundId;
}

#endregion