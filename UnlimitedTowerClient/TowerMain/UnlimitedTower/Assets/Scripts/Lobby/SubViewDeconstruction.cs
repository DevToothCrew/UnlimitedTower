using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubViewDeconstruction : MonoSingleton<SubViewDeconstruction>
{
    public Text textTitle;

    public DecontructionSlot[] slotList = new DecontructionSlot[9];

    public Text textEos;
    public Text textUgt;

    public Button buttonGrind;
    public Button buttonClear;

    //set Data
    int unitType = 0;   //0:servant , 1: monster
    int[] deconstructionUnitList = new int[9];  //분해할 유닛 idx
    int unit_count = 0;

    private PartyInfoVC partyInfo;

    void Awake()
    {
        partyInfo = PartyInfoVC.Inst;
    }

    private void OnEnable()
    {
        OnClickButtonClear();
        updateView();
    }

    void updateView()
    {
        for (int i=0; i<deconstructionUnitList.Length; i++)
        {
            if (i < unit_count)
            {
                slotList[i].updateSlot(deconstructionUnitList[i]);
            }
            else
            {
                slotList[i].updateSlot(0);
            }
        }

        textEos.text = string.Format("{0}", 0);
        textUgt.text = string.Format("{0}", 0);
    }

    public void InsertUnit(int chracter_unit_idx)
    {
        if (unit_count >= deconstructionUnitList.Length)
        {
            Debug.Log("Warning : 분해 슬롯 최대치!");
        }
        else
        {
            UserDataManager u_data = UserDataManager.Inst;
            int unit_idx = 0;

            if (checkInsertedUnit(chracter_unit_idx))
            {
                Debug.Log("Warning : 이미 분해 슬롯에 등록된 유닛 입니다.");
            }
            else
            {
                if (unitType == 0)  // Servant
                {
                    UserServantData servantData = UserDataManager.Inst.GetServantInfo(chracter_unit_idx);
                    if (servantData == null)
                    {
                        Debug.Log("Invalid Request Servant ID : " + chracter_unit_idx);
                        return;
                    }

                    if (servantData.state != 0)
                    {
                        Debug.Log("Invalid Servant State : " + servantData.state);
                        return;
                    }

                    if (servantData.partyIndex != 0)
                    {
                        Debug.Log("Invalid Servant Party Index : " + servantData.partyIndex);
                        return;
                    }
                }
                else if(unitType == 1)  //Monster
                {
                    UserMonsterData monsterData = UserDataManager.Inst.GetMonsterInfo(chracter_unit_idx);
                    if (monsterData == null)
                    {
                        Debug.Log("Invalid Request Monster ID : " + chracter_unit_idx);
                        return;
                    }

                    if (monsterData.state != 0)
                    {
                        Debug.Log("Invalid Monster State : " + monsterData.state);
                        return;
                    }

                    if (monsterData.partyIndex != 0)
                    {
                        Debug.Log("Invalid Monster Index : " + monsterData.partyIndex);
                        return;
                    }
                }

                unit_idx = chracter_unit_idx;
                deconstructionUnitList[unit_count] = unit_idx;
                unit_count++;
                updateView();
            }
        }
        
    }

    bool checkInsertedUnit(int unit_idx)
    {
        bool is_insert = false;
        for (int i = 0; i < unit_count; i++)
        {
            if (unit_idx == deconstructionUnitList[i])
            {
                is_insert = true;
                break;
            }
        }

        return is_insert;
    }

    //슬롯 클릭
    void OnClickSlot(int btn_idx)
    {
        //슬롯에 유닛이 있을때만 해제해주면 됨 -> 등록은 리스트에서 클릭시 등록됨.
        if (deconstructionUnitList[btn_idx] > 0)
        {
            deconstructionUnitList[btn_idx] = 0;
            unit_count--;

            if (unit_count < 0)
                unit_count = 0;
        }
        else
        {
            Debug.Log("warning : 비어있는 슬롯");
        }
    }

    public void OnClickCancelSlot(int btn_idx)
    {
        
        if (btn_idx >= unit_count - 1)
        {
            deconstructionUnitList[btn_idx] = 0;
        }
        else
        {
            for (int i = btn_idx; i < unit_count-1; i++)
            {
                deconstructionUnitList[i] = deconstructionUnitList[i + 1];
            }
        }
        
        unit_count--;

        if (unit_count < 0)
            unit_count = 0;

        updateView();
    }

    //Slot 초기화
    public void OnClickButtonClear()
    {
        unit_count = 0;

        for (int i=0; i< deconstructionUnitList.Length; i++)
        {
            deconstructionUnitList[i] = 0;
        }
    }

    public void OnClickClose()
    {
        this.gameObject.SetActive(false);
    }

    public void OnClickButtonGrind()
    {
        if (unitType == 0)  // Servant
        {
            List<int> servantIndexList = new List<int>();

            for (int i = 0; i < deconstructionUnitList.Length; i++)
            {
                UserServantData servantData = UserDataManager.Inst.GetServantInfo(deconstructionUnitList[i]);
                if(servantData == null)
                {
                    Debug.Log("Invalid Request Servant ID : " + deconstructionUnitList[i]);
                    return;
                }

                if(servantData.state != 0)
                {
                    Debug.Log("Invalid Servant State : " + servantData.state);
                    return;
                }

                if(servantData.partyIndex != 0)
                {
                    Debug.Log("Invalid Servant Index : " + servantData.partyIndex);
                    return;
                }

                servantIndexList.Add(deconstructionUnitList[i]);
            }
#if UNITY_EDITOR
            Cheat.Inst.RequestServantBurnCheat(servantIndexList);
#else
            PacketManager.Inst.RequestServantGrind(servantIndexList);
#endif
        }
        else if(unitType == 1)  // Monster
        {
            List<int> monsterIndexList = new List<int>();

            for (int i = 0; i < deconstructionUnitList.Length; i++)
            {
                UserMonsterData monsterData = UserDataManager.Inst.GetMonsterInfo(deconstructionUnitList[i]);
                if (monsterData == null)
                {
                    Debug.Log("Invalid Request Monster ID : " + deconstructionUnitList[i]);
                    return;
                }

                if (monsterData.state != 0)
                {
                    Debug.Log("Invalid Monster State : " + monsterData.state);
                    return;
                }

                if (monsterData.partyIndex != 0)
                {
                    Debug.Log("Invalid Monster Index : " + monsterData.partyIndex);
                    return;
                }

                monsterIndexList.Add(deconstructionUnitList[i]);
            }

#if UNITY_EDITOR
            Cheat.Inst.GetMonsterSellData(monsterIndexList);
#else
            PacketManager.Inst.RequestMonsterSell(monsterIndexList);
#endif
        }
    }
}
