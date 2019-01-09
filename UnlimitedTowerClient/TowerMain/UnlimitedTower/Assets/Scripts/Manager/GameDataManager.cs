using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour {
    
    public static GameDataManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // 누군가의 배치가 바뀌었을때
    public System.Action placeChangedEvent;

    // 배치 조회
    public bool isPlacedAt(int teamNum, int formationIndex)
    {
        // 몬스터 자리
        if (formationIndex >= 5)
        {
            return isMonsterPlacedAt(teamNum, formationIndex);
        }
        // 서번트 자리
        else
        {
            return isServantPlacedAt(teamNum, formationIndex);
        }
    }
    
    public bool isServantPlacedAt(int teamNum, int formationindex)
    {
        return UserDataManager.Inst.ServantList.Find((rowdata) => { return rowdata.onFormation && rowdata.partyIndex == teamNum && rowdata.formationIndex == formationindex; }) != null;
    }
    public bool isMonsterPlacedAt(int teamNum, int formationindex)
    {
        return UserDataManager.Inst.MonsterList.Find((rowdata) => { return rowdata.OnFormation && rowdata.partyIndex == teamNum && rowdata.formationIndex == formationindex; }) != null;
    }

    // 배치된놈 가져오기
    public UserServantData getServantPlacedAt_nullPossible(int teamNum, int formationindex)
    {
        return UserDataManager.Inst.ServantList.Find((rowdata) => { return rowdata.onFormation && rowdata.partyIndex == teamNum && rowdata.formationIndex == formationindex; });
    }
    public UserMonsterData getMonsterPlacedAt_nullPossible(int teamNum, int formationindex)
    {
        return UserDataManager.Inst.MonsterList.Find((rowdata) => { return rowdata.OnFormation && rowdata.partyIndex == teamNum && rowdata.formationIndex == formationindex; });
    }

    // 배치가능 조회
    public bool isServantPlaceExist(int teamNum)
    {
        for (int i = 0; i < 5; i++)
        {
            if (isPlacedAt(teamNum, i) == false)
            {
                return true;
            }
        }

        return false;
    }
    public bool isMonsterPlaceExist(int teamNum)
    {
        for (int i = 5; i < 10 ; i++)
        {
            if (isPlacedAt(teamNum, i) == false)
            {
                return true;
            }
        }

        return false;
    }
    // 배치가능자리 가져오기
    public int GetServantPlaceExist(int teamNum)
    {
        for (int i = 0; i < 5; i++)
        {
            if (isPlacedAt(teamNum, i) == false)
            {
                return i;
            }
        }

        Debug.Log("자리 잘못된요청");
        return -1;
    }
    public int GetMonsterPlaceExist(int teamNum)
    {
        for (int i = 5; i < 10; i++)
        {
            if (isPlacedAt(teamNum, i) == false)
            {
                return i;
            }
        }

        Debug.Log("버그");
        return -1;
    }
}
