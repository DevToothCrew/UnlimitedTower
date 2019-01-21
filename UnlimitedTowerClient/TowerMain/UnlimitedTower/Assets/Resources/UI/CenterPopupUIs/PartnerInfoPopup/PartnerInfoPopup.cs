﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartnerInfoPopup : MonoBehaviour {

    // 해당 윈도우의 상태는 2개의 변수로 정의됩니다
    // 1. windowstate -> 현재 Servant만 보여주고있는지, Monster만 보여주고있는지 결정
    // 2. sortstate   -> 현재 어떠한 값을 기준으로 정렬중인지 결정
    
    // 슬롯 리스트 //
    [SerializeField] List<SlotScript> SlotList = new List<SlotScript>();

    [SerializeField] Text pageText;


    // FSM 변수 //
    public enum WindowState
    {
        Servant,
        Monster
    }
    public WindowState windowState;
    int _pageNum;
    int pageNum
    {
        get
        {
            return _pageNum;
        }
        set
        {
            _pageNum = value;

            switch (windowState)
            {
                case WindowState.Servant:
                    pageText.text = (_pageNum + 1) + "/" + (UserDataManager.Inst.ServantList.Count/ SlotList.Count + 1);
                    break;
                case WindowState.Monster:
                    pageText.text = (_pageNum + 1) + "/" + (UserDataManager.Inst.MonsterList.Count / SlotList.Count + 1);
                    break;
            }
        }
    }

    public enum SortState
    {
        Grade,
        Level,
        Power,
        Obtain  
    }
    public SortState sortstate;
    
    private void OnEnable()
    {
        // 
        switch (windowState)
        {
            case WindowState.Servant:
                ToServantState(0);
                break;
            case WindowState.Monster:
                ToMonsterState(0);
                break;
        }
    }


    // FSM 상태이동 함수들 // 
    // WindowState 이동
    public void ToServantState(int pageNum)
    {
        this.pageNum = pageNum;

        // 창 초기화
        for (int i = 0; i < SlotList.Count; i++)
        {
            SlotList[i].LockSlot();
        }

        // 서번트 등록
        for (int i = 0; i < UserDataManager.Inst.ServantList.Count && i<SlotList.Count; i++)
        {
            SlotList[i].Register(UserDataManager.Inst.ServantList[i]);
        }

        // 다시정렬
        SetSortState((int)sortstate);

        windowState = WindowState.Servant;
    }
    public void ToMonsterState(int pageNum)
    {
        this.pageNum = pageNum;

        // 창 초기화
        for (int i = 0; i < SlotList.Count; i++)
        {
            SlotList[i].LockSlot();
        }

        // 몬스터 등록
        for (int i = 0; i < UserDataManager.Inst.MonsterList.Count && i < SlotList.Count; i++)
        {
            SlotList[i].Register(UserDataManager.Inst.MonsterList[i]);
        }

        // 다시정렬
        SetSortState((int)sortstate);

        windowState = WindowState.Monster;
    }
    // sortState 이동
    [SerializeField] List<Slot_Value_Tuple> _TupleList = new List<Slot_Value_Tuple>();
    public List<Slot_Value_Tuple> _tupleList
    {
        get
        {
            // 슬롯이 등록안되어있다면 일단 등록해줌
            if (_TupleList.Count < SlotList.Count)
            {
                for (int i = 0; i < SlotList.Count; i++)
                {
                    Slot_Value_Tuple newval = new Slot_Value_Tuple(SlotList[i]);
                    _TupleList.Add(newval);
                }
            }

            return _TupleList;
            
        }
        set
        {
            _TupleList = value;
        }
    }
    public void SetSortState(int sortstateNum)
    {
        this.sortstate = (SortState)sortstateNum;

        // 값 세팅하기
        for (int i = 0; i < _tupleList.Count; i++)
        {
            _tupleList[i].setValue(this.sortstate);
        }

        // 내림차순 정렬
        _tupleList.Sort((a, b) =>
        {
            if (a.value < b.value)
            {
                return 1;
            }
            else if (a.value == b.value)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        });

        // display
        for (int i = 0; i < _tupleList.Count; i++)
        {
            _tupleList[i].slotscript.transform.SetAsLastSibling();
        }
    }



    // 온클릭
    public void OnClickServantBtn()
    {
        if (windowState == WindowState.Servant)
        {
            return;
        }

        ToServantState(0);
    }
    public void OnClickMonsterBtn()
    {
        if (windowState == WindowState.Monster)
        {
            return;
        }

        ToMonsterState(0);
    }

    public void OnClickRightArrow()
    {
        // 다음윈도우안에 서번트 혹은 몬스터가 있다면 넘어간다.
        int startIndex = (pageNum+1) * SlotList.Count;
        int endIndex = (pageNum+2) * SlotList.Count;
        switch (windowState)
        {
            case WindowState.Servant:
                {
                    if (UserDataManager.Inst.ServantList.Count-1 >= startIndex)
                    {
                        ToServantState(pageNum+1);
                    }
                }
                break;
            case WindowState.Monster:
                {
                    if (UserDataManager.Inst.MonsterList.Count - 1 >= startIndex)
                    {
                        ToMonsterState(pageNum + 1);
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


        switch (windowState)
        {
            case WindowState.Servant:
                {
                    ToServantState(pageNum - 1);
                }
                break;
            case WindowState.Monster:
                {
                    ToMonsterState(pageNum - 1);
                }
                break;
        }
    }


    // 정렬할때 transform 과 각칸의 value를 함께 갖는 클래스
    [System.Serializable]
    public class Slot_Value_Tuple
    {
        public SlotScript slotscript;
        public double value;

        public void setValue(SortState sortstate)
        {
            // Early Exit 조건 => slot이 unlock상태이면, 가장 낮은값을 주고 나간다.
            if (slotscript.slottype == SlotScript.SlotType.locked)
            {
                value = double.MinValue;
                return;
            }


            // 분류방법에 따라서 어떻게 값을 할당할지 정한다.
            switch (sortstate)
            {
                case SortState.Grade:   // 등급순
                    {
                        switch (slotscript.slottype)
                        {
                            case SlotScript.SlotType.servant:
                                {
                                    // 서번트가 영웅이면 grade 1
                                    // 영웅이 아니면 grade 0
                                    value = slotscript.servant.isLegend ? 1 : 0;
                                }
                                break;
                                
                            case SlotScript.SlotType.monster:
                                {
                                    value = slotscript.monster.gradeNum;
                                }
                                break;
                        }
                    }
                    break;
                case SortState.Level:   // 레벨순
                    {
                        switch (slotscript.slottype)
                        {
                            case SlotScript.SlotType.servant:
                                {
                                    value = slotscript.servant.level;
                                }
                                break;

                            case SlotScript.SlotType.monster:
                                {
                                    value = slotscript.monster.level;
                                }
                                break;
                        }
                    }
                    break;
                case SortState.Power:   // 파워순
                    {
                        switch (slotscript.slottype)
                        {
                            case SlotScript.SlotType.servant:
                                {
                                    value = Etc.instance.Getatk(slotscript.servant);
                                }
                                break;

                            case SlotScript.SlotType.monster:
                                {
                                    value = Etc.instance.Getatk(slotscript.monster);
                                }
                                break;
                        }
                    }
                    break;
                case SortState.Obtain:  // 이 캐릭터를 얻은순서를 의미하는것인지 ... 
                    {

                    }
                    break;
            }
        }
        public Slot_Value_Tuple(SlotScript trans)
        {
            this.slotscript = trans;
        }
    }
}


