#region ENUM

public enum GRADE_TYPE
{
    LEGENDARY = 1,
    UNIQUE = 2,
    RARE = 3,
    UNCOMMON = 4,
    COMMON = 5,
}

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
    HERO = 1,
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
    NULL = 0,

    Girl_Kid_1 = 1,
    Girl_Kid_2 = 2,

    Man_Kid_1 = 101,
    Man_Kid_2,
    Man_Kid_3,
    Man_Kid_4,

    Hero_Novice_1001 = 105,
    Hero_Novice_1002,
    Hero_Novice_1003,
    Hero_Novice_1004,


    Mst_Cat = 201,
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
    Mst_Zombie,
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
    RUN = 0,
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
    Equipment,
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
    BASE = 1,

    MAX = 3
}

public enum GENDER_BODY
{
    BASE = 1,

    MAX = 3,
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
