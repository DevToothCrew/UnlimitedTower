using System;

#region ENUM

public enum GRADE_TYPE
{
    NONE = 0,
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
    SERVANT = 1,
    MONSTER = 2,
    BOSS = 3,
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
    Warrior = 1,

    // DEX
    Thief = 2,
    Archer = 4,

    // INT
    Cleric = 3,
    Magician = 5,
}

[Flags]
public enum SERVANT_JOB_FLAG
{
    None = 0,
    All = 1,
    Warrior = 2,
    Thief = 4,
    Cleric = 8,
    Archer = 16,
    Magician = 32,
    First = Warrior,
    Last = Magician
}

public enum ELEMENT_TYPE
{
    None = 0,
    Fire = 1,
    Water = 2,
    Earth = 3,
    Wind = 4,
    Light = 5,
    Dark = 6,
}

public enum TRIBE_TYPE
{
    None = 0,
    Beast = 1,
    Human = 2,
    Demon = 3,
    Spirit = 4,
    Insect = 5,
    Mermaid = 6,
    Mysterious = 7,
    Metal = 8,
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
    PVP = 6,
    Max = 7
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
    Max,
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
    Servant = 0,
    Monster = 1,
    Formation = 2,

    // Inventory
    Weapon = 10,
    Armor = 11,
    Accessory = 12,
    ETC = 13,

    // Shop
    EOS = 20,
    UTG = 21,
    Gacha = 22,

    // Stage
    Stage = 30,

    // TopInfo
    AccountInfo = 40,
    Mail = 41,
    Setting = 42,
    PVP = 43,
}

public enum EQUIPMENT_TYPE
{
    WEAPON = 0,
    ARMOR = 1,
    ACCESSSORY = 2,
    MAX = 3,
}

public enum EQUIPMENT_OPTION_TYPE
{
    NONE = 0,

    ATK = 1,
    MATK = 2,
    DEF = 3,
    MDEF = 4,

    HP = 5,

    STR = 6,
    DEX = 7,
    INT = 8,

    CriDmg = 9,
    CriPer = 10,

    MAX = 11,
}

public enum SKILL_TYPE
{
    NONE = 0,
    BUFF = 1,
    ATTACK = 2,
    HEAL = 3,
    DEBUFF = 4,
}

public enum ATTACK_TYPE
{
    NONE = 0,
    PHYSICAL = 1,
    MAGICAL = 2,
    MIX = 3,
}

public enum DAMAGE_TYPE
{
    NONE = 0,
    PHYSICAL = 1,
    MAGICAL = 2,
    PURE = 3,
}

public enum TARGET_TYPE
{
    NONE = 0,

    SELF = 1,
    ENEMY = 2,
    MYTEAM = 3,

    ENEMIES = 4,
}

public enum EFFECT_ID
{
    ATK = 1,
    MATK = 2,
    DEF = 3,
    MDEF = 4,
    HP = 5,
    STR = 6,
    DEX = 7,
    INT = 8,
    SPEED = 9,
    AVOID = 10,
    CRI_PER = 11,
    CRI_DMG = 12,
    HEAL_MULTI = 13,
    DEF_MDEF = 14,
}

public enum EFFECT_TYPE
{
    ADD = 1,
    MINUS = 2,
    ADD_PER = 3,
    MINUS_PER = 4
}

public enum REWARD_TYPE
{
    NONE = 0,
    SERVANT = 1,
    MONSTER = 2,
    EQUIPMENT = 3,
    ITEM = 4,
}

public enum DECONSTRUCTION_TYPE
{
    SERVANT = 0,
    MONSTER = 1,
    EQUIPMENT = 2,
}

public enum UPGRADE_TYPE
{
    MONSTER = 0,
    EQUIPMENT = 1
}

public enum SORT_TYPE
{
    POWER = 0,      //전투력
    GRADE,          //등급
    LEVEL_OR_TIER,  //장비는 Tier
    GOT_TIME,       //획득 순서
    JOB_OR_UPGRADE  //몬스터, 장비는 Upgrade
}

public enum MAIL_TYPE
{
    NONE = 0,
    PRE_SERVANT = 1,
    PRE_MONSTER = 2,
    PRE_EQUIPMENT = 3,
    UTG = 4,
    NFT_SERVANT = 5,
    NFT_MONSTER = 6,
    NFT_EQUIPMENT = 7,
    ETC_ITEM = 8,
    MAX = 9,
}

public enum SHOP_TYPE
{
    EOS = 1,
    UTG = 2,
    ETC = 3,
}

public enum POPUP_TYPE
{
    NOTIVE = 0, //안내 타입 팝업 : Cancel/OK 버튼 표시안함, 창 터치로 닫기 가능
    WARNING,    //경고, 오류 타입 팝업 : Cancel/OK 버튼 표시안함, 창 터치로 닫기 가능
    CONFIRM     //확인용 팝업 : Cancel, OK 버튼 표시, 창 터치로 닫기 불가
}

public enum Inventory_Menu_Type
{
    WEAPON = 0,
    ARMOR,
    ACCESSORY,
    ITEM
}

public enum MESSAGE_IDX
{
    UPGRADE_CONFIRM = 0,    //무기 업그레이드시 확인(파괴 가능성 언급)
    MAX = 1 //메세지 갯수
}

#endregion
