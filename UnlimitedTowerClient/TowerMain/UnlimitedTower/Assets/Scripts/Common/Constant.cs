using System.Collections.Generic;
using UnityEngine;


//공통 사항을 모아둔 클래스.
public class DEFINE
{
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
    public static readonly int HP_MULTI_VALUE = 22;
    public static readonly int DAMAGE_MUlTI_VALUE = 2;
    public static readonly int DEFENCE_MULTI_VALUE = 1;
    public static readonly int CRITICAL_PER_MULTI_VALUE = 1;
    

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


// 서번트 클래스에 히로도 포함된다.
[System.Serializable]
public class UserInfo
{
    public string userName;
    public int userMoney;
    public int userEOS;
    public UserServantData userHero = new UserServantData();
    public SCENE_STATE sceneState;
}

// erd완
[System.Serializable]
public class UserServantData
{
    // 캐릭터를 구분하는 고유값이 되어야함
    public int index;

    // 메인히어로인지 아닌지
    public bool isMainHero;

    // 영웅서번트인지 아닌지
    public bool isLegend;

    public string name;

    // TODO : Servant별 State 추가 필요
    public int exp;
    public int level;
    public int jobNum;

    // 현재는 appear를 통해 간단히 사용하고 추후 appearInfo와 job을 통해 캐릭터 생성이 되어야함
    public int charNum;
    public int headNum;
    public int hairNum;

    public Status status = new Status();
    public int leftStatPoint;
    



    public UserServantData()
    {
        exp = 0;
        level = 1;

        status = new Status();
        status.basicStr = DEFINE.TEST_STATUS_VALUE;
        status.basicDex = DEFINE.TEST_STATUS_VALUE;
        status.basicInt = DEFINE.TEST_STATUS_VALUE;
    }
    public UserServantData(int getCharNum)
    {
        charNum = getCharNum;
        exp = 0;
        level = 1;

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

    // 몬스터, 타입
    public int monsterNum;
    public int monsterTypeNum;

    // TODO : Monster별 State 추가 필요
    public int gradeNum;
    public int enforceNum;
    public int exp;
    public int level;

    public Status status = new Status();

    public string name;
    

    //TODO : TestCode
    public UserMonsterData()
    {
        exp = 0;
        level = 1;
        
        // job값으로 현재는 서번트 구분 추후에 합의하여 수정해야할듯.
        CHARACTER_NUM charNum = CHARACTER_NUM.Mst_BirdMan;

        // 리소스 인덱스
        index = (int)charNum;
        //UniqueIndex = servantinfo.s_index - 1;
        
        name = charNum.ToString();

        status = new Status();
        status.basicStr = DEFINE.TEST_STATUS_VALUE;
        status.basicDex = DEFINE.TEST_STATUS_VALUE;
        status.basicInt = DEFINE.TEST_STATUS_VALUE;
    }
    public UserMonsterData(int index)
    {
        exp = 0;
        level = 1;
        
        // 리소스 인덱스
        this.index = index;

        name = index.ToString();

        status = new Status();
        status.basicStr = DEFINE.TEST_STATUS_VALUE;
        status.basicDex = DEFINE.TEST_STATUS_VALUE;
        status.basicInt = DEFINE.TEST_STATUS_VALUE;
    }
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
// erd완
[System.Serializable]
public class UserFormationData
{
    public int partyIndex;
    public int formationIndex;

    public bool isPlaced;
    public bool isServant;
    public int index;
}



// erd완
[System.Serializable]
public class UserMountItemData
{
    // 
    public int index;

    //
    public int mountitemNum;

    public int tearNum;
    public int enforceCount;

    bool IsMounted;
    public bool isMounted
    {
        get
        {
            return IsMounted;
        }
        set
        {
            IsMounted = value;

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
public class UserEtcItemData
{
    //
    public int index;

    public int etcItemNum;
    public int Count;
}
// erd완
[System.Serializable]
public class UserPartyData
{
    public int partyIndex;

    public bool isFixedAtFloor;
}






// 사용안함...
public class Item
{
    // 서버에 저장되어야할 고유 index
    public int index;

    // 장비템인지 아닌지
    public bool isMountingItem;
    public bool itemNum;

    // 착용상태인지 아닌지
    public bool isMounted;
    public bool mountingServantIndex;

    public int state;
    public int id;
    public int slot;
    public int tier;
    public int job;
    public int grade;
    public int upgrade;
    public int atk;
    public int def;

    public Status status;
}
public class Party
{
    public int partyIndex;
    public int state;
    public Dictionary<int, PartyCharacterInfo> characterList = new Dictionary<int, PartyCharacterInfo>();
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


#region ENUM

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
    Hero_Novice_1005,
    Hero_Novice_1006,


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
    WhiteHand = 5,
    Warrior = 0,

    // DEX
    Theif = 2,
    Archer = 1,

    // INT
    Magician = 3,
    Cleric = 4,
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
    BASE = 0,

    MAX = 2
}

public enum APPEAR_HEAD
{
    BASE = 0,

    MAX = 2
}

public enum APPEAR_BODY
{
    BASE = 0,

    MAX = 3
}


#endregion