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

// 서번트 클래스에 히어로도 포함된다.
// TODO : Servant 삭제 영웅 경험치만 포함으로 변경
[Serializable]
public class UserInfo
{
    public string userName;
    public int userEXP;
    public ulong userUTG;
    public ulong userEOS;
    public SCENE_STATE sceneState;

    public int Level()
    {
        return CSVData.Inst.GetUserLevelByExp(userEXP);
    }
}

public class MainCharInfo
{
    public CHAR_TYPE mainCharType;
    public int mainCharID;
    public int grade;
}

// erd완
[Serializable]
public class UserServantData
{
    // 캐릭터를 구분하는 고유값이 되어야함
    public int index;


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
    public int grade;

    public int ServantID()
    {
        if (grade == 5)
        {
            if (jobNum == 1)
            {
                return 1000001;
            }
            else if (jobNum == 2)
            {
                return 1000002;
            }
            else if (jobNum == 3)
            {
                return 1000003;
            }
        }

        int id = (jobNum * 1000000) + (body * 100000) + (gender * 10000) + (headNum * 100) + hairNum;
        return id;
    }

    public Status status = new Status();

    /* 조회용 데이터 __________________________________________________________ */

    // 착용 아이템 리스트 -> 역참조초기화x, 역참조업데이트x
    public List<UserEquipmentData> equipmentList = new List<UserEquipmentData>();
    public event System.Action mountItemListChangeEvent;
    public void Equip(UserEquipmentData userEquipmentData)
    {
        equipmentList.Add(userEquipmentData);

        if (mountItemListChangeEvent != null)
        {
            mountItemListChangeEvent();
        }
    }
    public void Unequip(UserEquipmentData userEquipmentData)
    {
        // 장착하고있는 아이템이 아닐경우 -> return
        if (!equipmentList.Contains(userEquipmentData))
        {
            return;
        }

        // 장착 해제
        equipmentList.Remove(userEquipmentData);

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
    public int partyNum;
    public int formationNum;
}

[System.Serializable]
public class UserEquipmentData
{
    // 
    public int index;

    //
    public int id;

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
