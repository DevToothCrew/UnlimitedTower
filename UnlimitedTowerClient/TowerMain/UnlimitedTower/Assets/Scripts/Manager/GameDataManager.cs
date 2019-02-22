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
    public bool request_Placement(UNIT_TYPE type, int index, int teamnum, int formationIndex)
    {
        // 이미 배치된친구라면 return
        if (type == UNIT_TYPE.SERVANT)
        {
            if (UserDataManager.Inst.GetServantIsPlaced(index) == true)
            {
                return false;
            }
        }
        else if (type == UNIT_TYPE.MONSTER)
        {
            if (UserDataManager.Inst.GetMonsterIsPlaced(index) == true)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        // 
        Debug.Log(type + " " + index + " " + teamnum + " " + formationIndex);
        
        //// 널에 대한 예외처리 안되어있음
        //UserFormationData formdata = UserDataManager.Inst.GetFormaData_nullPossible(teamnum, formationIndex);
        //formdata.isPlaced = true;
        //formdata.index = index;


        // 조회용데이터에 바뀐것 넣어주기
        switch (type)
        {
            case UNIT_TYPE.SERVANT:
                UserDataManager.Inst.servantDic[index].partyNum = teamnum;
                UserDataManager.Inst.servantDic[index].isPlaced = true;
                UserDataManager.Inst.servantDic[index].formationNum = formationIndex;
                break;

            case UNIT_TYPE.MONSTER:
                UserDataManager.Inst.monsterDic[index].isPlaced = true;
                UserDataManager.Inst.monsterDic[index].teamNum = teamnum;
                UserDataManager.Inst.monsterDic[index].formationNum = formationIndex;
                break;
        }


        if (GameDataManager.instance != null && GameDataManager.instance.placeChangedEvent != null)
        {
            GameDataManager.instance.placeChangedEvent();
        }

        return true;
    }
    // 배치해제 요청
    public bool request_deplace(UNIT_TYPE type, int playerindex)
    {
        // 배치되어있지않다면 return
        if (type == UNIT_TYPE.SERVANT)
        {
            if (UserDataManager.Inst.GetServantIsPlaced(playerindex) == false)
            {
                return false;
            }
        }
        else if (type == UNIT_TYPE.MONSTER)
        {
            if (UserDataManager.Inst.GetMonsterIsPlaced(playerindex) == false)
            {
                return false;
            }
        }

        // 메인히어로를 빼려고한다면 return
        if (type == UNIT_TYPE.SERVANT && playerindex == 0)
        {
            return false;
        }



        // ERD 데이터 업데이트
        switch (type)
        {
            //case UNIT_TYPE.SERVANT:
            //    {
            //        UserFormationData formData = UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.isPlaced && rowdata.index == playerindex && rowdata.charType == CHAR_TYPE.SERVANT; });
            //        formData.isPlaced = false;
            //    }
            //    break;
            //case UNIT_TYPE.MONSTER:
            //    {
            //        UserFormationData formData = UserDataManager.Inst.UserFormationList.Find((rowdata) => { return rowdata.isPlaced && rowdata.index == playerindex && rowdata.charType == CHAR_TYPE.MONSTER; });
            //        formData.isPlaced = false;
            //    }
            //    break;
        }

        // ERD 역참조데이터 업데이트 
        switch (type)
        {
            case UNIT_TYPE.SERVANT:
                {
                    UserServantData servantdata = UserDataManager.Inst.servantDic[playerindex];
                    servantdata.isPlaced = false;
                }
                break;
            case UNIT_TYPE.MONSTER:
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

        return true;
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

        // 서번트가 같은 타입의 아이템장착하고있다면 -> 해당아이템 탈착
        MountitemType itemtype = ErdManager.instance.getmountitemEntityTable_nullPossible(mountitem.mountitemNum).mountitemType;
        UserMountItemData mountedItem = servantdata.mountItemList.Find((rowdata) =>
        {
            MountItemEntity.Param param = ErdManager.instance.getmountitemEntityTable_nullPossible(rowdata.mountitemNum);

            if (param.mountitemType == itemtype)
            {
                return true;
            }
            else
                return false;
        });
        if (mountedItem != null)
        {
            GameDataManager.instance.DemountItem(mountedItem);
        }


        // ERD 역참조데이터 업데이트
        if (UserDataManager.Inst.servantDic.ContainsKey(servantdata.index))
        {
            UserDataManager.Inst.servantDic[servantdata.index].Mount(mountitem);
        }
    }





}
