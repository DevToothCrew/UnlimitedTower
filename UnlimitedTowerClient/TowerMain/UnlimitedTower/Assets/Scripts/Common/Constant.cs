using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]


public class JsonFomation
{
    // public List<int>  formation = new List<int>();

    public int partyNum = 0;
    public List<int> formation = new List<int>();
}
[System.Serializable]
public class JsonBattleAction
{
    public int targetIndex = 0;
    public int actionType = 0;

    public JsonBattleAction(int targetIndex, int actionType)
    {
        this.targetIndex = targetIndex;
        this.actionType = actionType;
    }
}

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
}

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
    public Character(int index, GACHA_TYPE gachaType)
    {
        List<CHARACTER_NUM> myIndexList = new List<CHARACTER_NUM>();
        if(gachaType == GACHA_TYPE.Servant)
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




    public Character(shero_info heroInfo)
    {
        // 저장하지 않은 데이터
        // 등급, 강화수치, 추가 힘민지

    

        Str = heroInfo.h_status.basic_str;
        Dex = heroInfo.h_status.basic_dex;
        Int = heroInfo.h_status.basic_int;

        Level = 1;
        // job값으로 현재는 서번트 구분 추후에 합의하여 수정해야할듯.
        // CHARACTER_NUM charNum = (CHARACTER_NUM)(heroInfo.m_type + (int)CHARACTER_NUM.Mst_Cat);
        CHARACTER_NUM charNum = CHARACTER_NUM.Hero_Novice_1001;
        Index = (int)charNum;
        Size = GetSize(charNum);


        Name = charNum.ToString();
        Appear = Random.Range(0, 9999);
    }

    public Character(cservantinfo servantinfo)
    {
        // 저장하지 않은 데이터
        // 외모, 
        // 장비 리스트. 
        // 상태, 
        // 추가 스텟
        // 서번트 -105
        // 몬스터 - 201

        Exp = servantinfo.s_exp;

        Str = servantinfo.s_status.basic_str;
        Dex = servantinfo.s_status.basic_dex;
        Int = servantinfo.s_status.basic_int;

        // 파티에 들어가는 고유 유니크값.
        //servantinfo.s_index;

        Level = 1;
        // job값으로 현재는 서번트 구분 추후에 합의하여 수정해야할듯.
        CHARACTER_NUM charNum = (CHARACTER_NUM)(servantinfo.s_status.job + (int)CHARACTER_NUM.Hero_Novice_1001);

        // 리소스 인덱스
        Index = (int)charNum;
        UniqueIndex = servantinfo.s_index - 1;
        Size = GetSize(charNum);


        Name = charNum.ToString();     
    }

    public Character(cmonsterinfo monsterinfo)
    {
        // 저장하지 않은 데이터
        // 등급, 강화수치, 추가 힘민지

        Exp = monsterinfo.m_exp;

        Str = monsterinfo.m_status.basic_str;
        Dex = monsterinfo.m_status.basic_dex;
        Int = monsterinfo.m_status.basic_int;

        Level = 1;
        // job값으로 현재는 서번트 구분 추후에 합의하여 수정해야할듯.
        CHARACTER_NUM charNum = (CHARACTER_NUM)(monsterinfo.m_type + (int)CHARACTER_NUM.Mst_Cat);
        Index = (int)charNum;
        UniqueIndex = monsterinfo.m_index - 1;
        Size = GetSize(charNum);


        Name = charNum.ToString();
        Appear = Random.Range(0, 9999);
    }


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

public class PartyInfo
{
    public int partyNum;
    public Dictionary<int, int> partyDic;
}

// 배틀시 각 캐릭터 정보
public class Battle_Character_Status 
{
    public int partyIndex;

    public Character character;

    public int maxHp;
    public int nowHp;

    public int damage;

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

    // 캐릭터 최초 위치.
   // public Vector3 firstPos;
    //public Vector3 firstDir;



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
public enum GACHA_TYPE
{
    Servant,
    Monster,
    Item,
}

#endregion