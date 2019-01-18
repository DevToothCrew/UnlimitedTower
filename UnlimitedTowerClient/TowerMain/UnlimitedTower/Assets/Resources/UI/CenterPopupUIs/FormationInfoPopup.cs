using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationInfoPopup : MonoBehaviour
{

    public static FormationInfoPopup instance;
    private void Awake()
    {
        instance = this;
    }


    [Header("UI COMPONENTS")]
    public Sprite bgsprite;
    public Text pageText;

    [Header("ETC")]
    public Transform bottomslotParent;

    [Header("debugdata")]
    public List<FormationSlot_Bottom> bottomslotlist;
    public List<Formationslot_Upper> upperslotlist;




    // FSM //

    // 팀 번호
    public int curTeamNum;
    // 팀 Set함수
    public System.Action displayteamChanged;
    public void SetTeamIndex(int teamNum)
    {
        this.curTeamNum = teamNum;

        // 해당팀번호로 초기화
        for (int i = 0; i < upperslotlist.Count; i++)
        {
            upperslotlist[i].SetDisplayteam(this.curTeamNum);
        }


    }


    // 하단슬롯 디스플레이 타입
    public PlayerType displaytype;
    public int bottomWindowPageNum = 0;
    // 하단슬롯 디스플레이 Set함수
    public void SetdisplayType(PlayerType displaytype, int pageNum)
    {
        this.displaytype = displaytype;
        this.bottomWindowPageNum = pageNum;

        // UI텍스트
        switch (displaytype)
        {
            case PlayerType.servant:
                {
                    int maxPageNum = Mathf.CeilToInt(UserDataManager.Inst.ServantList.Count / bottomslotParent.childCount);
                    pageText.text = (bottomWindowPageNum + 1) + "/" + (maxPageNum + 1) + "P";
                }
                break;
            case PlayerType.monster:
                {
                    int maxPageNum = Mathf.CeilToInt(UserDataManager.Inst.MonsterList.Count / bottomslotParent.childCount);
                    pageText.text = (bottomWindowPageNum + 1) + "/" + (maxPageNum + 1) + "P";
                }
                break;
        }

        // 초기화후
        for (int i = 0; i < bottomslotlist.Count; i++)
        {
            bottomslotlist[i].to_none();
        }

        // 정렬
        Etc.instance.sort(displaytype, sortType);

        // 해당하는 윈도우 디스플레이
        int startIndex = pageNum * bottomslotParent.childCount;
        int endIndex = (1 + pageNum) * bottomslotParent.childCount;
        switch (displaytype)
        {
            case PlayerType.servant:
                {
                    List<UserServantData> list = UserDataManager.Inst.ServantList;
                    for (int dataindex = startIndex; dataindex < list.Count && dataindex < endIndex; dataindex++)
                    {
                        int slotIndex = dataindex - startIndex;
                        // 메인히어로는 선택지에 안나온다.
                        if (list[slotIndex].isMainHero)
                        {
                            continue;
                        }

                        bottomslotlist[slotIndex].to_servant(list[slotIndex]);
                    }
                }
                break;

            case PlayerType.monster:
                {

                    List<UserMonsterData> list = UserDataManager.Inst.MonsterList;
                    for (int dataindex = startIndex; dataindex < list.Count && dataindex < endIndex; dataindex++)
                    {
                        int slotIndex = dataindex - startIndex;

                        bottomslotlist[slotIndex].to_monster(list[slotIndex]);
                    }

                }
                break;
        }
    }


    // 하단슬롯 정렬타입 
    public SortType sortType;
    // 하단슬롯 정렬타입 Set함수
    public void SetSortType(SortType sortType)
    {
        this.sortType = sortType;

        // 해당하는 소트타입으로 다시 소트해주기
        Etc.instance.sort(displaytype, sortType);

        // 보여주기
        SetdisplayType(displaytype, bottomWindowPageNum);

    }





    // 상단윈도우 깜빡임 넘버
    public bool isWaiting;
    public int waitingFormNum;
    //
    public void TryWaiting(int formationIndex)
    {
        // 현재 팀번호에서 해당포메이션칸을 Waiting State로 만든다.
    }







    private void OnEnable()
    {
        SetTeamIndex(0);
        SetdisplayType(PlayerType.servant, 0);
        SetSortType(SortType.Power);
    }
    private void OnDisable()
    {

    }

    // 버튼 클릭 : sort
    public void setServantMode()
    {
        SetdisplayType(PlayerType.servant, 0);
    }
    public void setMonsterMode()
    {
        SetdisplayType(PlayerType.monster, 0);
    }
    public void Sortonclick(int sortTypeNum)
    {
        SetSortType((SortType)sortTypeNum);
    }

    // 버튼클릭: 오른쪽, 왼쪽
    public void OnclickRightBtn()
    {
        switch (displaytype)
        {
            case PlayerType.servant:
                {
                    // 다음윈도우안에 보여줄수있는애가 있으면, 다음으로 넘긴다.
                    int startindex = bottomslotParent.childCount * (bottomWindowPageNum + 1);
                    if (UserDataManager.Inst.ServantList.Count - 1 >= startindex)
                    {
                        SetdisplayType(PlayerType.servant, bottomWindowPageNum + 1);
                    }
                }
                break;
            case PlayerType.monster:
                {
                    // 다음윈도우안에 보여줄수있는애가 있으면, 다음으로 넘긴다.
                    int startindex = bottomslotParent.childCount * (bottomWindowPageNum + 1);
                    if (UserDataManager.Inst.MonsterList.Count - 1 >= startindex)
                    {
                        SetdisplayType(PlayerType.monster, bottomWindowPageNum + 1);
                    }
                }
                break;
        }
    }
    public void OnclickLeftBtn()
    {
        // 현재 페이지가 0이 아니면, 1낮춘다
        if (bottomWindowPageNum > 0)
        {
            SetdisplayType(displaytype, bottomWindowPageNum - 1);
        }
    }
}


