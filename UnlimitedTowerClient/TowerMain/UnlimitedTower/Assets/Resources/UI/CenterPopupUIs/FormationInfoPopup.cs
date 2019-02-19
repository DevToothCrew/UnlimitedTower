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



    // DEFAULT PARAMETERS
    const UNIT_TYPE DEFAULT_UNIT_TYPE = UNIT_TYPE.SERVANT;
    const sortType DEFAULT_SORT_TYPE = sortType.Grade;
    const int DEFAULT_PAGE_NUM = 0;
    


    /* FSM PARAMETERS */

    // 상단
    public int curTeamNum;
    public static System.Action teamNumChanged;
    public void SetTeamIndex(int teamNum)
    {
        this.curTeamNum = teamNum;

        SetPlaceMode(PlaceMode.JUST_DISPLAY);

        // 해당팀번호로 초기화
        for (int i = 0; i < upperslotlist.Count; i++)
        {
            upperslotlist[i].SetDisplayteam(this.curTeamNum);
        }
    }

    public enum PlaceMode
    {
        JUST_DISPLAY,
        SERVANT_PLACE,
        MONSTER_PLACE,
    }
    public PlaceMode placeMode;
    public UserServantData registeredServantData;
    public UserMonsterData registeredMonsterData;
    public static System.Action placeModeChanged;
    public void SetPlaceMode(PlaceMode placeMode)
    {
        this.placeMode = placeMode;



        if (placeModeChanged != null)
        {
            placeModeChanged();
        }
    }


    // 하단 상태변수
    public sortType sortType;
    public UNIT_TYPE unitType;
    public int bottomWindowPageNum = 0;








    // SORT TYPE, UNIT TYPE, PAGE NUM -> 하단창
    public void DisplayBottomUnits(sortType sortType, UNIT_TYPE unitType, int bottomWindowPageNum)
    {
        this.unitType = unitType;
        this.sortType = sortType;
        this.bottomWindowPageNum = bottomWindowPageNum;

        // 보여주기모드로
        SetPlaceMode(PlaceMode.JUST_DISPLAY);

        // UI텍스트 표시하기
        switch (unitType)
        {
            case UNIT_TYPE.SERVANT:
                {
                    // Servant List의 Count가 없을때 오류가 생길 수 있음
                    int maxPageNum = Mathf.CeilToInt(UserDataManager.Inst.GetServantCount() / bottomslotParent.childCount);
                    pageText.text = (bottomWindowPageNum + 1) + "/" + (maxPageNum + 1) + "P";
                }
                break;
            case UNIT_TYPE.MONSTER:
                {
                    int maxPageNum = Mathf.CeilToInt(UserDataManager.Inst.GetMonsterCount() / bottomslotParent.childCount);
                    pageText.text = (bottomWindowPageNum + 1) + "/" + (maxPageNum + 1) + "P";
                }
                break;
        }

        // 초기화
        for (int i = 0; i < bottomslotlist.Count; i++)
        {
            bottomslotlist[i].ToNone();
        }


        // unitType과 curTeamNum을 보고 해당하는 애들을 가져온다
        switch (unitType)
        {
            case UNIT_TYPE.SERVANT:
                {
                    // 페이지윈도우에 해당하는 애들 가져오기
                    int startIndex = bottomWindowPageNum * bottomslotlist.Count;
                    int endIndex = (bottomWindowPageNum + 1) * (bottomslotlist.Count);
                    List<UserServantData> list = new List<UserServantData>();
                    List<UserServantData> servantList = UserDataManager.Inst.GetServantList();
                    if(servantList != null)
                    {
                        for (int i = startIndex; i < endIndex && i < servantList.Count; i++)
                        {
                            list.Add(servantList[i]);
                        }
                    }

                    // sort하기
                    Etc.instance.SortByType(list, sortType);

                    // display 하기
                    for (int i = 0; i < bottomslotlist.Count && i < list.Count; i++)
                    {
                        bottomslotlist[i].ToServant(list[i]);
                    }

                }
                break;


            case UNIT_TYPE.MONSTER:
                {
                    // 페이지윈도우에 해당하는 애들 가져오기
                    int startIndex = bottomWindowPageNum * bottomslotlist.Count;
                    int endIndex = (bottomWindowPageNum + 1) * (bottomslotlist.Count);
                    List<UserMonsterData> list = new List<UserMonsterData>();
                    List<UserMonsterData> monsterList = UserDataManager.Inst.GetMonsterList();
                    if (monsterList != null)
                    {
                        for (int i = startIndex; i < endIndex && i < monsterList.Count; i++)
                        {
                            list.Add(monsterList[i]);
                        }
                    }

                    // sort하기
                    Etc.instance.SortByType(list, sortType);

                    // display 하기
                    for (int i = 0; i < bottomslotlist.Count && i < list.Count; i++)
                    {
                        bottomslotlist[i].ToMonster(list[i]);
                    }
                }
                break;
        }
    }
    public void DisplayUpperUnits(int teamNum)
    {

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
        DisplayBottomUnits(DEFAULT_SORT_TYPE, DEFAULT_UNIT_TYPE, DEFAULT_PAGE_NUM);
        DisplayUpperUnits(0);
        SetPlaceMode(PlaceMode.JUST_DISPLAY);

        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            upperslotlist[i].SetDisplayteam(0);
        }
    }
    private void OnDisable()
    {

    }

    // 버튼 클릭 : sort
    public void setServantMode()
    {
        DisplayBottomUnits(sortType, UNIT_TYPE.SERVANT, 0);
    }
    public void setMonsterMode()
    {
        DisplayBottomUnits(sortType, UNIT_TYPE.MONSTER, 0);
    }
    public void Sortonclick(int sortTypeNum)
    {
        DisplayBottomUnits((sortType)sortTypeNum, unitType, bottomWindowPageNum);
    }

    // 버튼클릭: 오른쪽, 왼쪽
    public void OnclickRightBtn()
    {
        switch (unitType)
        {
            case UNIT_TYPE.SERVANT:
                {
                    // 다음윈도우안에 보여줄수있는애가 있으면, 다음으로 넘긴다.
                    int startindex = bottomslotParent.childCount * (bottomWindowPageNum + 1);
                    if (UserDataManager.Inst.GetServantCount() - 1 >= startindex)
                    {
                        DisplayBottomUnits(sortType, UNIT_TYPE.SERVANT, bottomWindowPageNum + 1);
                    }
                }
                break;
            case UNIT_TYPE.MONSTER:
                {
                    // 다음윈도우안에 보여줄수있는애가 있으면, 다음으로 넘긴다.
                    int startindex = bottomslotParent.childCount * (bottomWindowPageNum + 1);
                    if (UserDataManager.Inst.GetMonsterCount() - 1 >= startindex)
                    {
                        DisplayBottomUnits(sortType, UNIT_TYPE.MONSTER, bottomWindowPageNum + 1);
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
            DisplayBottomUnits(sortType, unitType, bottomWindowPageNum - 1);
        }
    }
}


