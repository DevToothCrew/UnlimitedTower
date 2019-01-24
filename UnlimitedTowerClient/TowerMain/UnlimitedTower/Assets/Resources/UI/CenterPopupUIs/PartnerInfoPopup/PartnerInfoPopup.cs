using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartnerInfoPopup : MonoBehaviour
{

    // 해당 윈도우의 상태는 2개의 변수로 정의됩니다
    // 1. windowstate -> 현재 Servant만 보여주고있는지, Monster만 보여주고있는지 결정
    // 2. sortstate   -> 현재 어떠한 값을 기준으로 정렬중인지 결정

    // 슬롯 리스트 //
    [SerializeField] List<SlotScript> SlotList = new List<SlotScript>();

    [SerializeField] Text pageText;
    [SerializeField] GameObject SellingModeObj;
    [SerializeField] GameObject DisplayModeObj;


    const DISPLAY_UNIT_TYPE defaultUnitType = DISPLAY_UNIT_TYPE.Servant;
    const SORT_TYPE defaultSortType = SORT_TYPE.Grade;
    const int defaultPageNum = 0;


    /* FSM 변수 */

    // UNIT_TYPE
    public enum DISPLAY_UNIT_TYPE
    {
        Servant,
        Monster
    }
    public DISPLAY_UNIT_TYPE unitType;
    [SerializeField] int _pageNum;
    int pageNum
    {
        get
        {
            return _pageNum;
        }
        set
        {
            _pageNum = value;

            switch (unitType)
            {
                case DISPLAY_UNIT_TYPE.Servant:
                    pageText.text = (_pageNum + 1) + "/" + (Mathf.FloorToInt(UserDataManager.Inst.ServantList.Count / SlotList.Count) + 1);
                    break;
                case DISPLAY_UNIT_TYPE.Monster:
                    pageText.text = (_pageNum + 1) + "/" + (Mathf.FloorToInt(UserDataManager.Inst.MonsterList.Count / SlotList.Count) + 1);
                    break;
            }
        }
    }

    // SORT
    public enum SORT_TYPE
    {
        Grade,
        Level,
        Power,
        Obtain
    }
    public SORT_TYPE sortState;

    // MODE 
    public enum MODE_TYPE
    {
        JUST_DISPLAY,
        SELLING
    }
    public MODE_TYPE modeType;
    public List<UserMonsterData> sellingMonsterList;
    public List<UserServantData> sellingServantList;
    public static System.Action<UserMonsterData> monsterListChanged;
    public static System.Action<UserServantData> servantListChanged;
    public static System.Action modeChanged;

    private void OnEnable()
    {
        ToJustDisplayMode();
        DisplayUnits(PartnerInfoPopup.defaultUnitType, PartnerInfoPopup.defaultSortType, PartnerInfoPopup.defaultPageNum);
    }






    /* FSM 상태이동 함수*/

    // UNIT_TYPE
    public void DisplayUnits(DISPLAY_UNIT_TYPE unitType, SORT_TYPE sortType, int pageNum)
    {
        this.unitType = unitType;
        this.sortState = sortType;
        this.pageNum = pageNum;


        // 창 초기화
        for (int i = 0; i < SlotList.Count; i++)
        {
            SlotList[i].LockSlot();
        }


        // 정렬 
        int startIndex = SlotList.Count * pageNum;
        int endIndex = SlotList.Count * (pageNum + 1);
        switch (unitType)
        {
            case DISPLAY_UNIT_TYPE.Servant:
                {
                    List<UserServantData> list = new List<UserServantData>();
                    for (int i = startIndex; i < endIndex && i < UserDataManager.Inst.ServantList.Count; i++)
                    {
                        list.Add(UserDataManager.Inst.ServantList[i]);
                    }

                    List<Value> sortedSlotList = list.ConvertAll((servantdata) =>
                    {
                        switch (sortState)
                        {
                            case SORT_TYPE.Grade:
                                return new Value(servantdata, servantdata.isLegend ? 1 : 0);
                            case SORT_TYPE.Level:
                                return new Value(servantdata, servantdata.level);
                            case SORT_TYPE.Power:
                                return new Value(servantdata, Etc.instance.Getatk(servantdata));
                            default:
                                return new Value(servantdata, servantdata.index);
                        }
                    });

                    // 디스플레이
                    for (int i = 0; i < SlotList.Count && i < sortedSlotList.Count; i++)
                    {
                        SlotList[i].Register(sortedSlotList[i].servantdata);
                    }
                }
                break;
            case DISPLAY_UNIT_TYPE.Monster:
                {
                    List<UserMonsterData> list = new List<UserMonsterData>();
                    for (int i = startIndex; i < endIndex && i < UserDataManager.Inst.MonsterList.Count; i++)
                    {
                        list.Add(UserDataManager.Inst.MonsterList[i]);
                    }

                    List<Value> sortedSlotList = list.ConvertAll((monsterdata) =>
                    {
                        switch (sortState)
                        {
                            case SORT_TYPE.Grade:
                                return new Value(monsterdata, monsterdata.gradeNum);
                            case SORT_TYPE.Level:
                                return new Value(monsterdata, monsterdata.level);
                            case SORT_TYPE.Power:
                                return new Value(monsterdata, Etc.instance.Getatk(monsterdata));
                            default:
                                return new Value(monsterdata, monsterdata.index);
                        }
                    });

                    // 디스플레이
                    for (int i = 0; i < SlotList.Count && i < sortedSlotList.Count; i++)
                    {
                        SlotList[i].Register(sortedSlotList[i].monsterdata);
                    }
                }
                break;
        }

    }

    // SORT_STATE
    public void SortByGrade()
    {
        DisplayUnits(unitType, SORT_TYPE.Grade, pageNum);
    }
    public void SortByLevel()
    {
        DisplayUnits(unitType, SORT_TYPE.Level, pageNum);
    }
    public void SortByPower()
    {
        DisplayUnits(unitType, SORT_TYPE.Power, pageNum);
    }
    public void SortByObtain()
    {
        DisplayUnits(unitType, SORT_TYPE.Obtain, pageNum);
    }
    public void SetSortState(int sortstateNum)
    {
        Debug.Log("click");

        // 이미같은 sort면 -> return
        if (sortstateNum == (int)this.sortState)
        {
            return;
        }


        DisplayUnits(unitType, (SORT_TYPE)sortstateNum, pageNum);
    }

    // MODE_TYPE
    public void ToJustDisplayMode()
    {
        modeType = MODE_TYPE.JUST_DISPLAY;

        sellingMonsterList = new List<UserMonsterData>();
        sellingServantList = new List<UserServantData>();

        if (modeChanged != null)
        {
            modeChanged();
        }

        // 셀링버튼 업데이트
        SellingModeObj.SetActive(false);
        DisplayModeObj.SetActive(true);
    }
    public void ToSellingMode()
    {
        modeType = MODE_TYPE.SELLING;

        if (modeChanged != null)
        {
            modeChanged();
        }

        // 셀링버튼 업데이트
        SellingModeObj.SetActive(true);
        DisplayModeObj.SetActive(false);
    }




    /* 버튼 온클릭 함수 */

    // 상단버튼 온클릭
    public void OnClickServantBtn()
    {
        if (unitType == DISPLAY_UNIT_TYPE.Servant)
        {
            return;
        }

        DisplayUnits(DISPLAY_UNIT_TYPE.Servant, sortState, 0);
    }
    public void OnClickMonsterBtn()
    {
        if (unitType == DISPLAY_UNIT_TYPE.Monster)
        {
            return;
        }

        DisplayUnits(DISPLAY_UNIT_TYPE.Monster, sortState, 0);
    }

    // 우측화살표, 좌측화살표 버튼 온클릭
    public void OnClickRightArrow()
    {
        // 다음윈도우안에 서번트 혹은 몬스터가 있다면 넘어간다.
        int startIndex = (pageNum + 1) * SlotList.Count;
        int endIndex = (pageNum + 2) * SlotList.Count;
        switch (unitType)
        {
            case DISPLAY_UNIT_TYPE.Servant:
                {
                    if (UserDataManager.Inst.ServantList.Count - 1 >= startIndex)
                    {
                        DisplayUnits(DISPLAY_UNIT_TYPE.Servant, sortState, pageNum + 1);
                    }
                }
                break;
            case DISPLAY_UNIT_TYPE.Monster:
                {
                    if (UserDataManager.Inst.MonsterList.Count - 1 >= startIndex)
                    {
                        DisplayUnits(DISPLAY_UNIT_TYPE.Monster, sortState, pageNum + 1);
                    }
                }
                break;
        }
    }
    public void OnClickLeftArrow()
    {
        // 이미 페이지가 0페이지면 -> return
        if (pageNum == 0)
        {
            return;
        }


        switch (unitType)
        {
            case DISPLAY_UNIT_TYPE.Servant:
                {
                    DisplayUnits(DISPLAY_UNIT_TYPE.Servant, sortState, Mathf.Max(pageNum - 1, 0));
                }
                break;
            case DISPLAY_UNIT_TYPE.Monster:
                {
                    DisplayUnits(DISPLAY_UNIT_TYPE.Monster, sortState, Mathf.Max(pageNum - 1, 0));
                }
                break;
        }
    }

    // 셀링모드일때 버튼 클릭시
    public void OnClickSellingBtn()
    {
        if (modeType == MODE_TYPE.JUST_DISPLAY)
        {
            ToSellingMode();
        }
        else
        {
            ToJustDisplayMode();
        }
    }
    public void OnClickInSellingmode(UserServantData servantdata)
    {
        if (modeType != MODE_TYPE.SELLING)
        {
            return;
        }

        Debug.Log("servant Onclick! : " + servantdata.index);

        if (sellingServantList.Contains(servantdata))
        {
            sellingServantList.Remove(servantdata);
        }
        else
        {
            sellingServantList.Add(servantdata);
        }

        if (servantListChanged != null)
        {
            servantListChanged(servantdata);
        }
    }
    public void OnClickInSellingmode(UserMonsterData monsterdata)
    {
        if (modeType != MODE_TYPE.SELLING)
        {
            return;
        }


        if (sellingMonsterList.Contains(monsterdata))
        {
            sellingMonsterList.Remove(monsterdata);
        }
        else
        {
            sellingMonsterList.Add(monsterdata);
        }


        if (monsterListChanged != null)
        {
            monsterListChanged(monsterdata);
        }
    }




    // 정렬할때 transform 과 각칸의 value를 함께 갖는 클래스
    [System.Serializable]
    public class Value
    {
        public UserServantData servantdata;
        public UserMonsterData monsterdata;
        public double value;

        public Value(UserServantData servantdata, double value)
        {
            this.servantdata = servantdata;
            this.value = value;
        }
        public Value(UserMonsterData monsterdata, double value)
        {
            this.monsterdata = monsterdata;
            this.value = value;
        }
    }
}


