using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartnerInfoPopup : MonoBehaviour {

    // 해당 윈도우의 상태는 2개의 변수로 정의됩니다
    // 1. windowstate -> 현재 Servant만 보여주고있는지, Monster만 보여주고있는지 결정
    // 2. sortstate   -> 현재 어떠한 값을 기준으로 정렬중인지 결정
    
    // 슬롯 리스트 //
    [SerializeField] List<SlotScript> SlotList = new List<SlotScript>();
    
    // FSM 변수 //
    public enum WindowState
    {
        Servant,
        Monster
    }
    public WindowState windowstate;

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
        switch (windowstate)
        {
            case WindowState.Servant:
                toServantState();
                break;
            case WindowState.Monster:
                toMonsterState();
                break;
        }
    }


    // FSM 상태이동 함수들 // 
    // WindowState 이동
    public void toServantState()
    {
        // 창 초기화
        for (int i = 0; i < SlotList.Count; i++)
        {
            SlotList[i].LockSlot();
        }

        // 서번트 등록
        int index = 0;
        foreach (var servantpair in UserDataManager.Inst.servantDic)
        {
            SlotList[index].register(servantpair.Value);
            index++;
        }

        // 다시정렬
        setSortState((int)sortstate);

        windowstate = WindowState.Servant;
    }
    public void toMonsterState()
    {
        // 창 초기화
        for (int i = 0; i < SlotList.Count; i++)
        {
            SlotList[i].LockSlot();
        }

        // 몬스터 등록
        // 서번트 등록
        int index = 0;
        foreach (var monsterpair in UserDataManager.Inst.monsterDic)
        {
            SlotList[index].register(monsterpair.Value);
            index++;
        }

        // 다시정렬
        setSortState((int)sortstate);

        windowstate = WindowState.Monster;
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
    public void setSortState(int sortstateNum)
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
                                    value = Etc.instance.getatk(slotscript.servant);
                                }
                                break;

                            case SlotScript.SlotType.monster:
                                {
                                    value = Etc.instance.getatk(slotscript.monster);
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


