using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    /// <summary>
    /// 단순히 변수값을 바꾸는 것이지만, 서버에 데이터를 요청해서 바꾸어야 하기때문에
    /// 이것들을 모두 함수의 형태로 모아 두었습니다.
    /// </summary>

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



    /* 배치 */

    // 누군가의 배치가 바뀌었을때
    public System.Action placeChangedEvent;

    // 배치 요청
    public void request_Placement(PlayerType type, int index, int teamnum, int formationIndex)
    {
        // 이미 배치된친구라면 return
        if (isPlaced(type, index))
        {
            return;
        }

        // 
        Debug.Log(type + " " + index + " " + teamnum + " " + formationIndex);
        UserFormationData formdata = UserDataManager.Inst.GetFormaData_nullPossible(teamnum, formationIndex);
        formdata.isPlaced = true;
        formdata.index = index;


        // 조회용데이터에 바뀐것 넣어주기
        switch (type)
        {
            case PlayerType.servant:
                UserDataManager.Inst.servantDic[index].partyNum = teamnum;
                UserDataManager.Inst.servantDic[index].isPlaced = true;
                UserDataManager.Inst.servantDic[index].formationNum = formationIndex;
                break;

            case PlayerType.monster:
                UserDataManager.Inst.monsterDic[index].isPlaced = true;
                UserDataManager.Inst.monsterDic[index].teamNum = teamnum;
                UserDataManager.Inst.monsterDic[index].formationNum = formationIndex;
                break;
        }


        if (GameDataManager.instance != null && GameDataManager.instance.placeChangedEvent != null)
        {
            GameDataManager.instance.placeChangedEvent();
        }
    }
    // 배치해제 요청
    public void request_deplace(PlayerType type, int playerindex)
    {
        // 배치되어있지않다면 return
        if (!isPlaced(type, playerindex))
        {
            Debug.Log("걘 배치 안되어있는데?");
            return;
        }

        // 메인히어로를 빼려고한다면 return
        if (type == PlayerType.servant && playerindex == 0)
        {
            return;
        }



        // ERD 데이터 업데이트
        switch (type)
        {
            case PlayerType.servant:
                {
                    UserFormationData formData = UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.isPlaced && rowdata.index == playerindex && rowdata.charType == CHAR_TYPE.SERVANT; });
                    formData.isPlaced = false;
                }
                break;
            case PlayerType.monster:
                {
                    UserFormationData formData = UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.isPlaced && rowdata.index == playerindex && rowdata.charType == CHAR_TYPE.MONSTER; });
                    formData.isPlaced = false;
                }
                break;
        }

        // ERD 역참조데이터 업데이트 
        switch (type)
        {
            case PlayerType.servant:
                {
                    UserServantData servantdata = UserDataManager.Inst.servantDic[playerindex];
                    servantdata.isPlaced = false;
                }
                break;
            case PlayerType.monster:
                {
                    UserMonsterData servantdata = UserDataManager.Inst.monsterDic[playerindex];
                    servantdata.isPlaced = false;
                }
                break;
        }




        if (GameDataManager.instance != null && GameDataManager.instance.placeChangedEvent != null)
        {
            Debug.Log("이벤트완료");
            GameDataManager.instance.placeChangedEvent();
        }
    }

    // 배치 조회
    public bool isPlacedAt(int teamNum, int formationIndex)
    {
        UserFormationData formationdata = UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.isPlaced && rowdata.partyIndex == teamNum && rowdata.formationIndex == formationIndex; });
        if (formationdata != null)
        {

            Debug.Log(teamNum + "," + formationIndex + ":" + formationdata.isPlaced);
            return formationdata.isPlaced;
        }

        Debug.Log(teamNum + "," + formationIndex + ":" + false);
        return false;
    }
    public bool isPlaced(PlayerType type, int index)
    {
        if (type == PlayerType.servant)
        {
            return UserDataManager.Inst.servantDic[index].isPlaced;
        }
        else
        {
            return UserDataManager.Inst.monsterDic[index].isPlaced;
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
        if (formData.charType == CHAR_TYPE.MONSTER)
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

        // 몬스터일경우 return
        if (formData.charType != CHAR_TYPE.MONSTER)
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
        for (int i = 5; i < 10; i++)
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






    /* 아이템 */
    
    public void DemountItem(UserMountItemData mountitem)
    {
        if (mountitem.isMounted)
        {
            // isMounted에서 set함수로 이벤트 실행 됨
            mountitem.isMounted = false;

            // ERD역참조데이터 업데이트
            int servIndex = mountitem.mountServantIndex;
            if (UserDataManager.Inst.servantDic.ContainsKey(servIndex))
            {
                UserDataManager.Inst.servantDic[servIndex].Demount(mountitem);
            }
        }


    }
    public void MountItem(UserMountItemData mountitem, UserServantData servantdata)
    {
        mountitem.isMounted = true;
        mountitem.mountServantIndex = servantdata.index;

        // ERD 역참조데이터 업데이트
        if (UserDataManager.Inst.servantDic.ContainsKey(servantdata.index))
        {
            UserDataManager.Inst.servantDic[servantdata.index].Mount(mountitem);
        }
    }





}
