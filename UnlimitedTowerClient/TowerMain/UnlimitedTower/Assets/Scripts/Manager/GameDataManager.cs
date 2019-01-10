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

    // 배치 요청
    public void request_Placement(PlayerType type, int index, int teamnum, int formationIndex)
    {
        // 이미 배치된친구라면 return
        if (isPlaced(type,index))
        {
            return;
        }


        // 배치
        UserFormationData formdata = UserDataManager.Inst.getFormaData_nullPossible(teamnum, formationIndex);
        formdata.isPlaced = true;
        formdata.isServant = type == PlayerType.servant ? true : false;
        
        if (GameDataManager.instance != null && GameDataManager.instance.placeChangedEvent != null)
        {
            GameDataManager.instance.placeChangedEvent();
        }
    }
    // 배치해제 요청
    public void request_deplace(PlayerType type, int playerindex)
    {
        // 배치되어있지않다면 return
        if (!isPlaced(type,playerindex))
        {
            return;
        }

        // 저위에놈이 배치되어있는곳을 찾아서, 해제해준다.
        UserFormationData data= UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.isServant == (type == PlayerType.servant) && rowdata.index == playerindex; });
        if (data == null)
        {
            return;
        }
        data.isPlaced = false;
        
        if (GameDataManager.instance != null && GameDataManager.instance.placeChangedEvent != null)
        {
            GameDataManager.instance.placeChangedEvent();
        }
    }


    // 배치 조회
    public bool isPlacedAt(int teamNum, int formationIndex)
    {
        return UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.isPlaced && rowdata.partyIndex == teamNum && rowdata.formationIndex == formationIndex; }) != null;
        
    }
    public bool isPlaced(PlayerType type, int index)
    {
        if (type == PlayerType.servant)
        {
            return UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.isServant && rowdata.index == index && rowdata.isPlaced; }) != null;
        }
        else
        {
            return UserDataManager.Inst.UserFormationList.Find((rowdata) => { return !rowdata.isServant && rowdata.index == index && rowdata.isPlaced; }) != null;
        }
        
    }

    // 배치된놈 가져오기
    public UserServantData getServantPlacedAt_nullPossible(int teamNum, int formationindex)
    {

        UserFormationData formData = UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.isPlaced && rowdata.partyIndex == teamNum && rowdata.formationIndex == formationindex; });
        // 그자리에 배치 안되어있을경우 return
        if (!formData.isPlaced)
        {
            Debug.Log("버그");
            return null;
        }

        // 서번트가아닐경우 return
        if (!formData.isServant)
        {
            return null;
        }
        
        return UserDataManager.Inst.ServantList.Find((rowdata) => { return rowdata.index == formData.index; });
    }
    public UserMonsterData getMonsterPlacedAt_nullPossible(int teamNum, int formationindex)
    {
        UserFormationData formData = UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.isPlaced && rowdata.partyIndex == teamNum && rowdata.formationIndex == formationindex; });
        // 그자리에 배치 안되어있을경우 return
        if (!formData.isPlaced)
        {
            Debug.Log("버그");
            return null;
        }

        // 서번트가아닐경우 return
        if (formData.isServant)
        {
            return null;
        }

        return UserDataManager.Inst.MonsterList.Find((rowdata) => { return rowdata.index == formData.index; });
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
