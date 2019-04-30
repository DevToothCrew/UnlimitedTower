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

    public GameObject prefabPartyUnit;
    public GameObject prefabItemUnit;

    public ScrollListManager scrollList;

    private DECONSTRUCTION_TYPE dType = 0;



    //set Data
    int[] deconstructionUnitList = new int[9];  //분해할 유닛 idx
    int unit_count = 0;

    //분해 가능한 Unit idx만 존재하는 리스트
    public List<int> scrollListData = new List<int>();
    private SORT_TYPE sort_type = 0;

    private PartyInfoVC partyInfo;
    private EquipmentInfoManager equipmentInfo;

    void Start()
    {

    }

    public DECONSTRUCTION_TYPE GetDeconstructionType()
    {
        return dType;
    }

    public void SetDeconstructionType(DECONSTRUCTION_TYPE type)
    {
        dType = type;

        if (dType == DECONSTRUCTION_TYPE.SERVANT || dType == DECONSTRUCTION_TYPE.MONSTER)
        {
            partyInfo = PartyInfoVC.Inst;
            scrollList.prefabUnit = prefabPartyUnit;
        }
        else
        {
            equipmentInfo = EquipmentInfoManager.Inst;
            scrollList.prefabUnit = prefabItemUnit;
        }

        initScrollList();
        updateView();
    }

    //스크롤 생성
    void initScrollList()
    {
        setData();

        scrollList.Init(this, 20, scrollListData.Count, getOrder());
    }

    void setData()
    {
        scrollListData.Clear();

        switch (dType)
        {
            case DECONSTRUCTION_TYPE.SERVANT:
                for (int i=0; i < partyInfo.ServantList.Count; i++)
                {
                    if (partyInfo.ServantList[i].state == 1 && partyInfo.ServantList[i].partyIndex == 0)
                    {
                        scrollListData.Add(i);
                    }
                }
                break;
            case DECONSTRUCTION_TYPE.MONSTER:
                for (int i = 0; i < partyInfo.MonsterList.Count; i++)
                {
                    if (partyInfo.MonsterList[i].state == 1 && partyInfo.MonsterList[i].partyIndex == 0)
                    {
                        scrollListData.Add(i);
                    }
                }
                break;
            case DECONSTRUCTION_TYPE.EQUIPMENT:
                for (int i = 0; i < equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()].Count; i++)
                {
                    if (equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][i].state == 1 && equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][i].equipServantIndex == 0)
                    {
                        scrollListData.Add(i);
                    }
                }
                break;
            default:
                DebugLog.Log(false, "Warning : 잘못된 타입 분해!");
                SimpleErrorPopupVC.Inst.UpdateErrorText("Invalid Type");
                break;

        }
    }

    public void ResetScrollListBySortType(SORT_TYPE type)
    {
        sort_type = type;
        scrollList.SetItemOrder(getOrder());
        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
        scrollList.ScrollViewDidScroll();
    }

    //스크롤 정렬
    private int[] getOrder()
    {
        int[] data_order;
        int total_list_num = scrollListData.Count;

        data_order = new int[total_list_num];

        for (int i = 0; i < data_order.Length; i++)
        {
            data_order[i] = 0;
        }

        switch (sort_type)
        {
            case SORT_TYPE.POWER:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (dType == DECONSTRUCTION_TYPE.SERVANT)
                        {

                            if (Calculator.GetPower(partyInfo.ServantList[scrollListData[i]].status, partyInfo.ServantList[scrollListData[i]].level) <
                                Calculator.GetPower(partyInfo.ServantList[scrollListData[j]].status, partyInfo.ServantList[scrollListData[j]].level))
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (dType == DECONSTRUCTION_TYPE.MONSTER)
                        {
                            if (Calculator.GetPower(partyInfo.MonsterList[scrollListData[i]].status, partyInfo.MonsterList[scrollListData[i]].level) <
                                Calculator.GetPower(partyInfo.MonsterList[scrollListData[j]].status, partyInfo.MonsterList[scrollListData[j]].level))
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (dType == DECONSTRUCTION_TYPE.EQUIPMENT)
                        {
                            if (equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][scrollListData[i]].value < equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][scrollListData[j]].value)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                    }
                }
                break;
            case SORT_TYPE.GRADE:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (dType == DECONSTRUCTION_TYPE.SERVANT)
                        {
                            if (partyInfo.ServantList[scrollListData[i]].grade > partyInfo.ServantList[scrollListData[j]].grade)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (dType == DECONSTRUCTION_TYPE.MONSTER)
                        {
                            if (partyInfo.MonsterList[scrollListData[i]].grade > partyInfo.MonsterList[scrollListData[j]].grade)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (dType == DECONSTRUCTION_TYPE.EQUIPMENT)
                        {
                            if (equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][scrollListData[i]].grade > equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][scrollListData[j]].grade)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                    }
                }
                break;
            case SORT_TYPE.LEVEL_OR_TIER:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (dType == DECONSTRUCTION_TYPE.SERVANT)
                        {
                            if (partyInfo.ServantList[scrollListData[i]].level * 100 + partyInfo.ServantList[scrollListData[i]].exp < partyInfo.ServantList[scrollListData[j]].level * 100 + partyInfo.ServantList[scrollListData[j]].exp)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (dType == DECONSTRUCTION_TYPE.MONSTER)
                        {
                            if (partyInfo.MonsterList[scrollListData[i]].level * 100 + partyInfo.MonsterList[scrollListData[i]].exp < partyInfo.MonsterList[scrollListData[j]].level * 100 + partyInfo.MonsterList[scrollListData[j]].exp)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (dType == DECONSTRUCTION_TYPE.EQUIPMENT)
                        {   
                            if (CSVData.Inst.GetEquipmentData(equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][scrollListData[i]].id).tier < CSVData.Inst.GetEquipmentData(equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][scrollListData[j]].id).tier)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                    }
                }
                break;
            case SORT_TYPE.GOT_TIME:
                for (int i = 0; i < total_list_num; i++)
                {
                    data_order[i] = i;
                }
                break;
            case SORT_TYPE.JOB_OR_UPGRADE:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (dType == DECONSTRUCTION_TYPE.SERVANT)
                        {
                            if (partyInfo.ServantList[scrollListData[i]].job > partyInfo.ServantList[scrollListData[j]].job)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (dType == DECONSTRUCTION_TYPE.MONSTER)
                        {
                            if (partyInfo.MonsterList[scrollListData[i]].upgrade < partyInfo.MonsterList[scrollListData[j]].upgrade)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (dType == DECONSTRUCTION_TYPE.EQUIPMENT)
                        {
                            if (equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][scrollListData[i]].upgrade < equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][scrollListData[j]].upgrade)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }


        return data_order;
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

    public void InsertUnit(int scroll_unit_idx)
    {
        if (unit_count >= deconstructionUnitList.Length)
        {
            DebugLog.Log(false, "Warning : 분해 슬롯 최대치!");
            SimpleErrorPopupVC.Inst.UpdateErrorText("Max Burn List");
        }
        else
        {
            UserDataManager u_data = UserDataManager.Inst;
            int unit_idx = 0;

            if (checkInsertedUnit(scroll_unit_idx))
            {
                DebugLog.Log(false, "Warning : 이미 분해 슬롯에 등록된 유닛 입니다.");
                SimpleErrorPopupVC.Inst.UpdateErrorText("Already Listed");
            }
            else
            {
                if (dType == DECONSTRUCTION_TYPE.SERVANT || dType == DECONSTRUCTION_TYPE.MONSTER)
                {
                    if (dType == DECONSTRUCTION_TYPE.SERVANT)  // Servant
                    {
                        UserServantData servantData = UserDataManager.Inst.GetServantInfo(scroll_unit_idx);
                        if (servantData == null)
                        {
                            DebugLog.Log(false, "Invalid Request Servant ID : " + scroll_unit_idx);
                            return;
                        }

                        if (servantData.state != 1)
                        {
                            DebugLog.Log(false, "Invalid Servant State : " + servantData.state);
                            SimpleErrorPopupVC.Inst.UpdateErrorText("Invalid Servant State");
                            return;
                        }

                        if (servantData.partyIndex != 0)
                        {
                            DebugLog.Log(false, "Invalid Servant Party Index : " + servantData.partyIndex);
                            SimpleErrorPopupVC.Inst.UpdateErrorText("Already In Party");
                            return;
                        }
                    }
                    else//Monster
                    {
                        UserMonsterData monsterData = UserDataManager.Inst.GetMonsterInfo(scroll_unit_idx);
                        if (monsterData == null)
                        {
                            DebugLog.Log(false, "Invalid Request Monster ID : " + scroll_unit_idx);
                            return;
                        }

                        if (monsterData.state != 1)
                        {
                            DebugLog.Log(false, "Invalid Monster State : " + monsterData.state);
                            SimpleErrorPopupVC.Inst.UpdateErrorText("Invalid Monster State");
                            return;
                        }

                        if (monsterData.partyIndex != 0)
                        {
                            DebugLog.Log(false, "Invalid Monster Index : " + monsterData.partyIndex);
                            SimpleErrorPopupVC.Inst.UpdateErrorText("Already In Party");
                            return;
                        }
                    }
                }
                else//Equip
                {
                    UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(scroll_unit_idx);
                    if (equipmentData == null)
                    {
                        DebugLog.Log(false, "Invalid Request Servant ID : " + scroll_unit_idx);
                        return;
                    }

                    if (equipmentData.state != 1)
                    {
                        DebugLog.Log(false, "Invalid Equip State : " + equipmentData.state);
                        SimpleErrorPopupVC.Inst.UpdateErrorText("Invalid Equipment State");
                        return;
                    }

                    if (equipmentData.isEquiped)
                    {
                        DebugLog.Log(false, "Invalid Equip isEquiped : " + equipmentData.isEquiped);
                        SimpleErrorPopupVC.Inst.UpdateErrorText("Already Equiped Servant");
                        return;
                    }
                }

                

                unit_idx = scroll_unit_idx;
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

    public void OnClickCancelSlot(int btn_idx)
    {

        for (int i = btn_idx; i < unit_count - 1; i++)
        {
            deconstructionUnitList[i] = deconstructionUnitList[i + 1];
        }

        deconstructionUnitList[unit_count - 1] = 0;
        
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

        updateView();
    }

    //분해 완료(서버에서 응답) 후 화면 전체 갱신
    public void updateViewFinishRequest()
    {
        OnClickButtonClear();

        if (PartyInfoVC.checkInst())
        {
            PartyInfoVC patyInfo = PartyInfoVC.Inst;
            patyInfo.setData();
            patyInfo.resetScroll();
            patyInfo.updateDetailInfo(patyInfo.scrollList.getFirstItemOrder());
        }
        else if (InventoryVC.checkInst())
        {
            InventoryVC inventory = InventoryVC.Inst;
            if (inventory.FrameEquipmentInfo.activeSelf)
            {
                EquipmentInfoManager equipmentInfo = EquipmentInfoManager.Inst;
                equipmentInfo.setData();
                equipmentInfo.resetScroll();
                equipmentInfo.updateDetailInfo(equipmentInfo.scrollList.getFirstItemOrder());
                //OnClickClose();
            }
        }

        setData();
        scrollList.SetItemOrder(getOrder());

    }

    public void OnClickClose()
    {
        if (PartyInfoVC.checkInst())
        {
            PartyInfoVC.Inst.frameScroll.SetActive(true);
        }
        else if (InventoryVC.checkInst())
        {
            InventoryVC.Inst.FrameMain.SetActive(true);
        }

        Destroy(this.gameObject);
    }

    public void OnClickButtonGrind()
    {
        if (dType == DECONSTRUCTION_TYPE.SERVANT)  // Servant
        {
            List<int> servantIndexList = new List<int>();

            for (int i = 0; i < deconstructionUnitList.Length; i++)
            {
                if (deconstructionUnitList[i] == 0)
                {
                    continue;
                }

                UserServantData servantData = UserDataManager.Inst.GetServantInfo(deconstructionUnitList[i]);
                if (servantData == null)
                {
                    DebugLog.Log(false, "Invalid Request Servant ID : " + deconstructionUnitList[i]);
                    return;
                }

                if (servantData.state != 1)
                {
                    DebugLog.Log(false, "Invalid Servant State : " + servantData.state);
                    SimpleErrorPopupVC.Inst.UpdateErrorText("Invalid Servant State");
                    return;
                }

                if (servantData.partyIndex != 0)
                {
                    DebugLog.Log(false, "Invalid Servant Index : " + servantData.partyIndex);
                    SimpleErrorPopupVC.Inst.UpdateErrorText("Already In Party");
                    return;
                }

                servantIndexList.Add(deconstructionUnitList[i]);
            }
#if UNITY_EDITOR
            Cheat.Inst.RequestServantBurnCheat(servantIndexList);
#else
            PacketManager.Inst.RequestServantBurn(servantIndexList);
#endif
        }
        else if (dType == DECONSTRUCTION_TYPE.MONSTER)  // Monster
        {
            List<int> monsterIndexList = new List<int>();

            for (int i = 0; i < deconstructionUnitList.Length; i++)
            {
                if (deconstructionUnitList[i] == 0)
                {
                    continue;
                }

                UserMonsterData monsterData = UserDataManager.Inst.GetMonsterInfo(deconstructionUnitList[i]);
                if (monsterData == null)
                {
                    DebugLog.Log(false, "Invalid Request Monster ID : " + deconstructionUnitList[i]);
                    return;
                }

                if (monsterData.state != 1)
                {
                    DebugLog.Log(false, "Invalid Monster State : " + monsterData.state);
                    SimpleErrorPopupVC.Inst.UpdateErrorText("Invalid Monster State");
                    return;
                }

                if (monsterData.partyIndex != 0)
                {
                    DebugLog.Log(false, "Invalid Monster Index : " + monsterData.partyIndex);
                    SimpleErrorPopupVC.Inst.UpdateErrorText("Already In Party");
                    return;
                }

                monsterIndexList.Add(deconstructionUnitList[i]);
            }

#if UNITY_EDITOR
            Cheat.Inst.RequestMonsterBurnCheat(monsterIndexList);
#else
            PacketManager.Inst.RequestMonsterBurn(monsterIndexList);
#endif
        }
        else if (dType == DECONSTRUCTION_TYPE.EQUIPMENT) // Equip
        {
            List<int> equipmentIndexList = new List<int>();

            for (int i = 0; i < deconstructionUnitList.Length; i++)
            {
                if(deconstructionUnitList[i] == 0)
                {
                    continue;
                }

                UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(deconstructionUnitList[i]);
                if (equipmentData == null)
                {
                    DebugLog.Log(false, "Invalid Request Equipment ID : " + deconstructionUnitList[i]);
                    return;
                }

                if (equipmentData.state != 1)
                {
                    DebugLog.Log(false, "Invalid Equipment State : " + equipmentData.state);
                    SimpleErrorPopupVC.Inst.UpdateErrorText("Invalid Equipment State");
                    return;
                }

                if (equipmentData.equipServantIndex != 0)
                {
                    DebugLog.Log(false, "Invalid Equipment Already Equip Servant Index : " + equipmentData.equipServantIndex);
                    SimpleErrorPopupVC.Inst.UpdateErrorText("Already Equiped Servant");
                    return;
                }

                equipmentIndexList.Add(deconstructionUnitList[i]);
            }
#if UNITY_EDITOR
            Cheat.Inst.RequestEquipmentBurnCheat(equipmentIndexList);
#else
            PacketManager.Inst.RequestEquipmentBurn(equipmentIndexList);
#endif
        }
    }
}
