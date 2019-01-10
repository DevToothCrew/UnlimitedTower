using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationInfoPopup : MonoBehaviour {
    
    public Sprite bgsprite;

    public static FormationInfoPopup instance;
    private void Awake()
    {
        instance = this;
    }

    [Header("debugdata")]

    public List<FormationSlot_Bottom> bottomslotlist;
    public List<Formationslot_Upper> upperslotlist;




    // FSM //

    // 팀 번호
    public int curTeamNum;
    // 팀 Set함수
    public System.Action displayteamChanged;
    public void setTeamIndex(int teamNum)
    {
        this.curTeamNum = teamNum;

        // 해당팀번호로 초기화
        for (int i = 0; i < upperslotlist.Count; i++)
        {
            upperslotlist[i].SetDisplayteam(this.curTeamNum);
        }


    }

    // 디스플레이 타입
    public enum displayType
    {
        servant,
        monster
    }
    public displayType displaytype;
    // 디스플레이타입 Set함수
    public void setdisplayType(displayType disType)
    {
        this.displaytype = disType;
        // 초기화후
        for (int i = 0; i < bottomslotlist.Count; i++)
        {
            bottomslotlist[i].to_none();
        }

        // bottom에 서번트만
        if (disType == displayType.servant)
        {
            List<UserServantData> list = new List<UserServantData>(UserDataManager.Inst.servantDic.Values);
            for (int i = 0; i < list.Count && i < bottomslotlist.Count; i++)
            {
                // 메인히어로는 선택지에 안나온다.
                if (list[i].isMainHero)
                {
                    continue;
                }

                bottomslotlist[i].to_servant(list[i]);
            }
        }
        // bottom에 몬스터만
        else
        {
            List<UserMonsterData> list = new List<UserMonsterData>(UserDataManager.Inst.monsterDic.Values);
            for (int i = 0; i < list.Count && i < bottomslotlist.Count; i++)
            {
                bottomslotlist[i].to_monster(list[i]);
            }
        }
    }

    // 정렬상태 
    public enum SortType
    {
        Grade,
        Level,
        Power,
        Obtain
    }
    public SortType sortType;
    // 정렬상태 Set함수
    public void setSortType(SortType sortType)
    {
        this.sortType = sortType;
        // 해당하는 소트타입으로 다시 소트해주기
    }


    // 대기중인 칸
    public bool isWaiting;
    public int waitingFormNum;
    //
    public void TryWaiting(int formationIndex)
    {
        // 현재 팀번호에서 해당포메이션칸을 Waiting State로 만든다.
    }
    



    


    private void OnEnable()
    {
        setTeamIndex(0);
        setdisplayType(displayType.servant);
        setSortType(SortType.Power);
    }
    private void OnDisable()
    {
        
    }

    // 버튼 클릭
    public void setServantMode()
    {
        setdisplayType(displayType.servant);
    }
    public void setMonsterMode()
    {
        setdisplayType(displayType.monster);
    }
    public void Sortonclick(int sortTypeNum)
    {
        setSortType((SortType)sortTypeNum);
    }
}


