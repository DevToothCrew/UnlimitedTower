using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//공통 사항을 모아둔 클래스.

public class DEFINE
{

	// GACHA COST
	public const int GACHA_COST = 1;

	// MIN - MAX Status
	public const int MIN_STATUS = 10;
	public const int MAX_STATUS = 30;
    public const int RAND_STATUS = 10;

    // MIN - MAX Exp
    public const int MIN_EXP = 0;
	public const int MAX_EXP = 1024;

    // SHADER Name
    public const string OUTLINE_SHADER = "Outlined/UltimateOutline";
    public const string BASIC_SHADER = "Mobile/Unlit (Supports Lightmap)";

    // Status posOffset
    public const int MAX_HP_MULTI_posOffset = 1;
    public const int AVOID_MULTI_posOffset = 1;
    public const int SPEED_MULTI_posOffset = 1;

    public const int DAMAGE_STR_MUlTI_posOffset = 3;
    public const int DAMAGE_DEX_MUlTI_posOffset = 2;
    public const int DAMAGE_INT_MUlTI_posOffset = 1;

    // Party Info
    public const int PARTY_MAX_NUM = 10;

    // Battle Position
    public const float START_X = -1.5f;
    public const float START_Z = -5.0f;
    public const float DISTANCE_OFFSET = 1.0f;

    // 캐릭터 끼리 최소 만나기 직전의 최소 거리
    public const float STOP_DISTANCE = 0.5f;

    // 공격 끝나고 되돌아 올때 거리 보정하는 기준.
    public const float MIN_DISTANCE = 0.3f;

    // 움직이는 속도
    public const float MOVING_SPEED = 4.0f;
    public  static float ANIMATION_SPEED = 2.0f;

    // 테스트 상수
    public static Vector3 PLAYER_BACKLINE_CENTER_POS = new Vector3(0.0f, 0.0f, -5.0f);
    public static Vector3 ENEMY_BACKLINE_CENTER_POS = new Vector3(0.0f, 0.0f, 5.0f);




    public static Vector3 GetBattlePosition(int partyIndex, CHAR_TYPE type, int num)
    {
        Vector3 position = new Vector3();
        int posOffset = 0;
        float y = 0.0f;
        if(type == CHAR_TYPE.PLAYER)
        {
            posOffset = 1;
        }
        else
        {
            posOffset = -1;
        }

        if(num == (int)CHARACTER_NUM.Mst_Chest)
        {
            y = 0.1f;
        }

        // Vector3(0.0f, 0.0f, 5.0f);
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

    //캐릭터 구분하는 값
    public int Index;
    public int Appear;

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
    public Character(int index)
    {
        Exp = 0;
        Level = 1;

        Str = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);
        Dex = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);
        Int = DEFINE.MIN_STATUS + Random.Range(0, DEFINE.RAND_STATUS);

        Job = Random.Range(0, 3) * 100 + Random.Range(1, 3);
        Index = index;
        Name = "Test" + Index;
        Appear = Random.Range(0, 9999);
    }

    public int GetMaxHp()
    {
        return Str * DEFINE.MAX_HP_MULTI_posOffset;
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
    public CHAR_TYPE charType;
    public CharacterAction(int _myIndex, int _targetIndex, ACTION_TYPE _actionType, CHAR_TYPE _charType)
    {
        myIndex = _myIndex;
        targetIndex = _targetIndex;
        actionType = _actionType;
        charType = _charType;
    }
}

#region ENUM

public enum CHAR_TYPE
{
    PLAYER = 0,
    ENEMY = 1,
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
    Man_Kid_2           = 102,
    Man_Kid_3           = 103,
    Man_Kid_4           = 104,

    Hero_Novice_1001    = 105,
    Hero_Novice_1002    = 106,
    Hero_Novice_1003    = 107,
    Hero_Novice_1004    = 108,


    Mst_Cat             = 201,
    Mst_Demon           = 202,
    Mst_Giant           = 203,
    Mst_Monkey          = 204,
    Mst_Wolf            = 205,
    Mst_Chest           = 206,
    Mst_Anubis          = 207,
    Mst_Ninja           = 208,
    Mst_FireFairy       = 209,
    Mst_Death           = 210,
    Mst_WolfMan         = 211,
    Mst_Knight          = 212,
    Mst_DarkMagician    = 213,
    Mst_Egg             = 214,
    Mst_Goblen_Blue     = 215,
    Mst_Goblen_Green    = 216,
    Mst_Leaf_fairy      = 217,
    Mst_Sham            = 218,
    Mst_Shamcat         = 219,
    Mst_Water_fairy     = 210



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

#endregion