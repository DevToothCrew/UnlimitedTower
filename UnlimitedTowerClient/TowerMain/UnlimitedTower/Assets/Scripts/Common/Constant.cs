using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//공통 사항을 모아둔 클래스.
public class DEFINE
{

	// GACHA COST
	public readonly int GACHA_COST = 1;
    
	// MIN - MAX Status
	public static readonly int MIN_STATUS = 10;
	public static readonly int MAX_STATUS = 30;
    public static readonly int RAND_STATUS = 10;

    // MIN - MAX Exp
    public static readonly int MIN_EXP = 0;
    public static readonly int MAX_EXP = 1024;

    // SHADER Name
    public static readonly string OUTLINE_SHADER = "Outlined/UltimateOutline";
    public static readonly string BASIC_SHADER = "Mobile/Unlit (Supports Lightmap)";

    // Status posOffset
    public static readonly int MAX_HP_MULTI_posOffset = 1;
    public static readonly int AVOID_MULTI_posOffset = 1;
    public static readonly int SPEED_MULTI_posOffset = 1;

    public static readonly int DAMAGE_STR_MUlTI_posOffset = 3;
    public static readonly int DAMAGE_DEX_MUlTI_posOffset = 2;
    public static readonly int DAMAGE_INT_MUlTI_posOffset = 1;

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
    public static readonly int TEST_HP_NUM = 10;

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
    
    #region stauts Calcuate
    public static int GetMaxHp(int Str)
    {
        return Str * DEFINE.MAX_HP_MULTI_posOffset * DEFINE.TEST_HP_NUM;
    }
    public static int GetAvoid(int Dex)
    {
        return Dex * DEFINE.AVOID_MULTI_posOffset;
    }
    public static int GetDamage(int Job, int Str, int Dex, int Int)
    {
        if (Job / 100 == 0)
        {
            return Str * DEFINE.DAMAGE_STR_MUlTI_posOffset;
        }
        else if (Job / 100 == 1)
        {
            return Dex * DEFINE.DAMAGE_DEX_MUlTI_posOffset;
        }
        else if (Job / 100 == 2)
        {
            return Int * DEFINE.DAMAGE_INT_MUlTI_posOffset;
        }
        else
        {
            return 0;
        }
    }
    public static int GetSpeed(int Dex)
    {
        return Dex * DEFINE.SPEED_MULTI_posOffset;
    }

    public static int GetLevelForExp(int exp)
    {
        // TODO : 추후 Servant Exp에 따른 Level 공식을 추가해 레벨 적용 필요
        return 1;
    }
    #endregion
}

public class Status
{
    public int basicStr;
    public int basicDex;
    public int basicInt;

    public int plusStr;
    public int plusDex;
    public int plusInt;
}

// 서번트 클래스에 히로도 포함된다.
// TODO  : New Code
public class Servant
{
    // 캐릭터를 구분하는 고유값이 되어야함
    public int index;

    // TODO : Servant별 State 추가 필요
    public int state;
    public int exp;
    public int level;
    public int job;

    // 현재는 appear를 통해 간단히 사용하고 추후 appearInfo와 job을 통해 캐릭터 생성이 되어야함
    public int appear;

    public int head;
    public int hair;
    public int body;

    public Status status;
    public int statPoint;

    public SIZE_TYPE size;
    public string name;

    public bool onFormation = false;
    public int formationIndex = -1;

    // 장착한 장비 리스트
    public List<int> equipmentList = new List<int>();

    public Servant()
    {
        state = 0;
        exp = 0;
        level = 1;

        status = new Status();
        status.basicStr = DEFINE.TEST_STATUS_VALUE;
        status.basicDex = DEFINE.TEST_STATUS_VALUE;
        status.basicInt = DEFINE.TEST_STATUS_VALUE;

        // TODO : 추후 수정 필요 임시 코드
        CHARACTER_NUM charNum = CHARACTER_NUM.Hero_Novice_1001;
        // 리소스 인덱스
        appear = (int)charNum;
        size = DEFINE.GetCharacterSize(charNum);
        name = charNum.ToString();
    }
}

public class UserInfo
{
    public string userName;
    public int userMoney;
    public int userEOS;
    public Servant userHero = new Servant();
}

public class Monster
{
    // 캐릭터를 구분하는 고유값이 되어야함
    public int index;


    // TODO : Monster별 State 추가 필요
    public int look;
    public int state;
    public int exp;
    public int level;
    public int type;
    public int grade;
    public int upgrade;

    // TODO : Type과 Appear를 통해 외형 결정
    public int appear;
    public Status status;

    public SIZE_TYPE size;
    public string name;

    public bool onFormation = false;
    public int formationIndex = -1;


    //TODO : TestCode
    public Monster()
    {
        exp = 0;
        level = 1;

        status = new Status();
        status.basicStr = DEFINE.TEST_STATUS_VALUE;
        status.basicDex = DEFINE.TEST_STATUS_VALUE;
        status.basicInt = DEFINE.TEST_STATUS_VALUE;

        // job값으로 현재는 서번트 구분 추후에 합의하여 수정해야할듯.
        CHARACTER_NUM charNum = CHARACTER_NUM.Mst_BirdMan;

        // 리소스 인덱스
        index = (int)charNum;
        //UniqueIndex = servantinfo.s_index - 1;

        size = DEFINE.GetCharacterSize(charNum);
        name = charNum.ToString();
    }


    public Monster(CHARACTER_NUM charNum)
    {
        exp = 0;
        level = 1;

        status = new Status();
        status.basicStr = DEFINE.TEST_STATUS_VALUE;
        status.basicDex = DEFINE.TEST_STATUS_VALUE;
        status.basicInt = DEFINE.TEST_STATUS_VALUE;

        // job값으로 현재는 서번트 구분 추후에 합의하여 수정해야할듯.
      

        // 리소스 인덱스
        index = (int)charNum;
        //UniqueIndex = servantinfo.s_index - 1;

        size = DEFINE.GetCharacterSize(charNum);
        name = charNum.ToString();
    }
}

public class Item
{
    public int index;

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


//add by canie
public class Party
{
    public int partyIndex;
    public int state;
    public Dictionary<int, PartyCharacterInfo> characterList = new Dictionary<int, PartyCharacterInfo>();
}

public class PartyCharacterInfo
{
    public int partyLocation; //OrderList 매칭시킨 
    public int type;
    public int index; // 캐릭터 구분하는 인덱스

    public PartyCharacterInfo(int getPartyLocation, int getType, int getIndex)
    {
        partyLocation = getPartyLocation;
        type = getType;
        index = getIndex;
    }
    public PartyCharacterInfo()
    {

    }
}

// TODO : Old Code
public class Character
{
	public int Exp;
	public int Level;
	public int Str;
	public int Dex;
	public int Int;
    public int Job;
    public SIZE_TYPE Size;
    
	public string Name;

    //리소스)구분하는 값
    public int Index;
    // 캐릭터 구분하는 고유값
    public long UniqueIndex;
    public int Appear;

    public bool OnFormation = false;
    public int FormationIndex = -1;

    public Character(int getExp, int getLv, int getStr, int getDex, int getInt, 
        int getJob, string getName, int getChartIndex, SIZE_TYPE getSizeType)
    {
        Exp = getExp;
        Level = getLv;
        Str = getStr;
        Dex = getDex;
        Int = getInt;
        Job = getJob;
        Name = getName;
        Index = getChartIndex;
        Size = getSizeType;
    }
    public Character(int index, GACHA_RESULT_TYPE gachaType)
    {
        List<CHARACTER_NUM> myIndexList = new List<CHARACTER_NUM>();
        if(gachaType == GACHA_RESULT_TYPE.Servant)
        {
            myIndexList.Add(CHARACTER_NUM.Hero_Novice_1001);
            myIndexList.Add(CHARACTER_NUM.Hero_Novice_1002);
            myIndexList.Add(CHARACTER_NUM.Hero_Novice_1003);
            myIndexList.Add(CHARACTER_NUM.Hero_Novice_1004);
        }
        else
        {
            for(int i = (int)CHARACTER_NUM.Mst_Cat; i<= (int)CHARACTER_NUM.Mst_Snail; i++)
            {
                myIndexList.Add((CHARACTER_NUM)i);
            }
        }

      

        int myIndex = Random.Range(0, myIndexList.Count);
        CHARACTER_NUM charType = myIndexList[myIndex];

        Size = GetSize(charType);

        Exp = 0;
        Level = 1;

        Str = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);
        Dex = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);
        Int = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);

        Job = Random.Range(0, 3) * 100 + Random.Range(1, 3);

 
        Name = charType.ToString();
        Index = (int)charType;

        Appear = Random.Range(0, 9999);
    }

    // Test Code : Hero
    public Character(CHAR_TYPE charType)
    {
        CHARACTER_NUM char_index;
        if (charType == CHAR_TYPE.SERVANT)
        {
            char_index = CHARACTER_NUM.Hero_Novice_1001;
        }
        else
        {
            char_index = CHARACTER_NUM.Mst_BirdMan;
        }

        Size = GetSize(char_index);

        Exp = 0;
        Level = 1;

        Str = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);
        Dex = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);
        Int = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);

        Job = Random.Range(0, 3) * 100 + Random.Range(1, 3);


        Name = char_index.ToString();
        Index = (int)char_index;

        Appear = Random.Range(0, 9999);
    }

    // Test Code :
    public Character(CHARACTER_NUM charNum)
    {
        Size = GetSize(charNum);

        Exp = 0;
        Level = 1;

        Str = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);
        Dex = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);
        Int = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);

        Job = Random.Range(0, 3) * 100 + Random.Range(1, 3);


        Name = charNum.ToString();
        Index = (int)(charNum);

        Appear = Random.Range(0, 9999);
    }

    // TODO : 위 Calculate에 있는 함수가 왜 여기 또?
    public int GetMaxHp()
    {
        return Str * DEFINE.MAX_HP_MULTI_posOffset * DEFINE.TEST_HP_NUM;
    }
    public int GetAvoid()
    {
        return Dex * DEFINE.AVOID_MULTI_posOffset;
    }
    public int GetDamage()
    {
        if (Job / 100 == 0)
        {
            return Str * DEFINE.DAMAGE_STR_MUlTI_posOffset;
        }
        else if (Job / 100 == 1)
        {
            return Dex * DEFINE.DAMAGE_DEX_MUlTI_posOffset;
        }
        else if (Job / 100 == 2)
        {
            return Int * DEFINE.DAMAGE_INT_MUlTI_posOffset;
        }
        else
        {
            return 0;
        }
    }
    public int GetSpeed()
    {
        return Dex * DEFINE.SPEED_MULTI_posOffset;
    }
    public SIZE_TYPE GetSize(CHARACTER_NUM charType)
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



// 배틀시 각 캐릭터 정보
public class Battle_Character_Status 
{
    public int partyIndex;

    public Character character;

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

    //TODO : Old Code
    public Battle_Character_Status(Character getChar, int getPartyIndex, int getCharacterIndex, SIZE_TYPE getSizeType)
    {
        if (getPartyIndex < 0 || getPartyIndex > DEFINE.PARTY_MAX_NUM)
        {
            return;
        }
        partyIndex = getPartyIndex;

        // 캐릭터 수치.
        character = getChar;

        maxHp = character.GetMaxHp();
        nowHp = maxHp;

        damage = character.GetDamage();

        avoid = character.GetAvoid();
        speed = character.GetSpeed();

        stateType = STATE_TYPE.IDLE;
        sizeType = getSizeType;

        lived = true;
    }

    // New Setting ServantInfo
    public Battle_Character_Status(Servant servant, int getPartyIndex, int getCharacterIndex, SIZE_TYPE getSizeType)
    {
        if (getPartyIndex < 0 || getPartyIndex > DEFINE.PARTY_MAX_NUM)
        {
            return;
        }
        partyIndex = getPartyIndex;

        // 캐릭터 수치.
        name = servant.name;

        maxHp = DEFINE.GetMaxHp(servant.status.basicStr + servant.status.plusStr);
        nowHp = maxHp;

        // 이런식으로 쓰면 안됩니다 DEFINE은
        damage = DEFINE.GetDamage(servant.job, servant.status.basicStr + servant.status.plusStr, servant.status.basicDex+ servant.status.plusDex, servant.status.basicInt + servant.status.plusInt);

        avoid = DEFINE.GetAvoid(servant.status.basicDex + servant.status.plusDex);
        speed = DEFINE.GetSpeed(servant.status.basicDex + servant.status.plusDex);

        stateType = STATE_TYPE.IDLE;
        sizeType = getSizeType;

        level = servant.level;
        lived = true;
    }
    // New Setting MonsterInfo
    public Battle_Character_Status(Monster monster, int getPartyIndex, int getCharacterIndex, SIZE_TYPE getSizeType)
    {
        if (getPartyIndex < 0 || getPartyIndex > DEFINE.PARTY_MAX_NUM)
        {
            return;
        }
        partyIndex = getPartyIndex;

        // 캐릭터 수치.
        name = monster.name;

        maxHp = DEFINE.GetMaxHp(monster.status.basicStr + monster.status.plusStr);
        nowHp = maxHp;

        damage = DEFINE.GetDamage(monster.type, monster.status.basicStr + monster.status.plusStr,
            monster.status.basicDex + monster.status.plusDex, monster.status.basicInt + monster.status.plusInt);

        avoid = DEFINE.GetAvoid(monster.status.basicDex + monster.status.plusDex);
        speed = DEFINE.GetSpeed(monster.status.basicDex + monster.status.plusDex);

        stateType = STATE_TYPE.IDLE;
        sizeType = getSizeType;

        level = monster.level;
        lived = true;
    }



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
        //if (instance == null)
        //{
        //    return default<T>;
        //}
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
    HERO = 0,
    SERVANT,
    MONSTER
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

public enum CHARACTER_JOB
{
    WhiteHand = 0,

    // STR
    Knight = 1,
    SwordMan = 2,

    // DEX
    Thief = 101,
    Archer = 102,

    // INT
    MAGICIAN = 201,
    CLERIC = 202,
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
    Setting = 6,
    Max = 7,
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
    Item
}

#endregion