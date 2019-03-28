using System;
using System.Collections.Generic;
using UnityEngine;

#region Info

[Serializable]
public class NodeInfo
{
    public NODE_LIST nodeName;
    public int nodeSpeed; // 00 ms
}

[Serializable]
public class UserInfo
{
    public string userName;
    public int userEXP;
    public ulong userUTG;
    public ulong userEOS;
    public SCENE_STATE sceneState;

    public int level
    {
        get
        {
            return CSVData.Inst.GetUserLevelByExp(userEXP);
        }
    }
}

[Serializable]
public class MainCharInfo
{
    public CHAR_TYPE mainCharType;
    public int mainCharID;
    public int grade;
}

[Serializable]
public class UserServantData
{
    public int index;
    public int id;
    public int grade;
    public int state;

    public int partyIndex;
    public bool isPlaced;

    public Status status = new Status();

    public int exp;
    public int level
    {
        get
        {
            // CSV로 변경 필요
            return Calculator.GetLevelForExp(exp);
        }
    }
}

[Serializable]
public class UserMonsterData
{
    public int index;
    public int id;
    public int grade;
    public int upgrade;
    public int state;

    public int partyIndex;
    public bool isPlaced;

    public int exp;
    public int level
    {
        get
        {
            // CSV로 변경 필요
            return Calculator.GetLevelForExp(exp);
        }
    }

    public Status status = new Status();
}

[System.Serializable]
public class UserEquipmentData
{
    public int index;
    public int id;
    public int grade;
    public int upgrade;
    public int state;

    public int value;

    public bool isEquiped;
    public int equipServantIndex;
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
            if (formationIndex <= DEFINE.ServantMaxFormationNum)
            {
                return CHAR_TYPE.SERVANT;
            }

            return CHAR_TYPE.MONSTER;
        }
    }
}




[System.Serializable]
public class UserItemData
{
    public int index;

    public int id;
    public int count;
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
    public static T CheckReferenceTo<T>(T instance)
    {
        return instance;

    }
    public static bool IsNull(this object obj)//literally all i did was add "this"
    {
        return obj == null || ReferenceEquals(obj, null) || obj.Equals(null);
    }
}

#endregion
