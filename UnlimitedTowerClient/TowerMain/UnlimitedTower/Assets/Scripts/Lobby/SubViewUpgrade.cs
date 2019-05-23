using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubViewUpgrade : MonoSingleton<SubViewUpgrade>
{
    //Memo
    //19.04.12 : 장비강화의 경우, 일단 보유중인 주문서가 자동으로 삽입되도록 함. (추후, 주문서의 종류가 다양해질 경우 선택하도록 변경)
    // -> 스크롤뷰 X


    public Text textTitle;

    //선택한 오브젝트 정보(이미지) : 몬스터 or 장비
    public Image FrameSelectedObject;
    public Image ImageSelectedObject;
    public Text textSelectedObjectUpgrade;

    //강화에 사용되는 오브젝트 정보(이미지) : 같은 종류/등급 몬스터 or 장비 강화 주문서
    public Image FrameInsertObject;
    public Image ImageInsertObject;
    public Text textInsertObjectUpgrade;

    public Text textUgt;
    public Text textSuccessPer;
    public Text textNeedPerCount;

    //강화 성공시 만들어질 오브젝트 정보(이미지)
    public Image FrameResultSlot;
    public Image imageResultSlot;
    public Text textResultUpgrade;

    public Button buttonUpgrade;

    public GameObject prefabPartyUnit;
    public GameObject prefabItemUnit;

    public GameObject FrameScroll;
    public ScrollListManager scrollList;
    private SORT_TYPE sort_type;
    private bool reverse_sort = false;

    public List<int> scrollListData = new List<int>();

    private PartyInfoVC partyInfo;
    private EquipmentInfoManager equipmentInfo;

    private UPGRADE_TYPE upgradeType = 0;
    private int selected_object_idx;    //선택된 오브젝트 Idx
    private int inserted_object_idx;    //강화에 사용할 오브젝트 Idx

    //몬스터 일때
    private UserMonsterData monsterData;

    //장비 일때
    private UserEquipmentData equipmentData;

    public UPGRADE_TYPE GetUpgradeType()
    {
        return upgradeType;
    }

    public void SetUpgradeType(UPGRADE_TYPE type)
    {
        upgradeType = type;

        if (upgradeType == UPGRADE_TYPE.MONSTER)
        {
            partyInfo = PartyInfoVC.Inst;
            scrollList.prefabUnit = prefabPartyUnit;
            FrameScroll.SetActive(true);

            //선택된 몬스터 Data
            monsterData = partyInfo.MonsterList[partyInfo.selected_unit_idx];
        }
        else//upgradeType == UPGRADE_TYPE.EQUIPMENT
        {
            equipmentInfo = EquipmentInfoManager.Inst;
            scrollList.prefabUnit = prefabItemUnit;
            FrameScroll.SetActive(false);

            //선택된 장비 Data
            equipmentData = equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][equipmentInfo.GetUnitIdx()];

            InsertEquipmentUpgradeScroll();
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

        switch (upgradeType)
        {
            case UPGRADE_TYPE.MONSTER:
                for (int i = 0; i < partyInfo.MonsterList.Count; i++)
                {
                    if (monsterData.id == partyInfo.MonsterList[i].id && monsterData.grade == partyInfo.MonsterList[i].grade && monsterData.upgrade >= partyInfo.MonsterList[i].upgrade && monsterData.index != partyInfo.MonsterList[i].index
                        && partyInfo.MonsterList[i].state == 1 && partyInfo.MonsterList[i].partyIndex == 0)
                    {
                        scrollListData.Add(i);
                    }
                }
                break;
            case UPGRADE_TYPE.EQUIPMENT:
                //추후 아이템 중, 주문서류만 scrollListData에 삽입.
                break;
            default:
                DebugLog.Log(false, "Warning : 잘못된 타입 분해!");
                break;

        }
    }

    public void ResetScrollListBySortType(SORT_TYPE type)
    {
        sort_type = type;
        if (sort_type == type)
        {
            reverse_sort = !reverse_sort;
        }
        scrollList.scrollRect.velocity = Vector2.zero;
        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
        scrollList.SetItemOrder(getOrder());
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
                        if (upgradeType == UPGRADE_TYPE.MONSTER)
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
                        else if (upgradeType == UPGRADE_TYPE.EQUIPMENT)
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
                        else
                        {
                            DebugLog.Log(false, "잘못된 분해 타입!");
                        }
                    }
                }
                break;
            case SORT_TYPE.GRADE:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (upgradeType == UPGRADE_TYPE.MONSTER)
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
                        else if (upgradeType == UPGRADE_TYPE.EQUIPMENT)
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
                        else
                        {
                            DebugLog.Log(false, "잘못된 분해 타입!");
                        }
                    }
                }
                break;
            case SORT_TYPE.LEVEL_OR_TIER:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (upgradeType == UPGRADE_TYPE.MONSTER)
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
                        else if (upgradeType == UPGRADE_TYPE.EQUIPMENT)
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
                        else
                        {
                            DebugLog.Log(false, "잘못된 분해 타입!");
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
                        if (upgradeType == UPGRADE_TYPE.MONSTER)
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
                        else if (upgradeType == UPGRADE_TYPE.EQUIPMENT)
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
                        else
                        {
                            DebugLog.Log(false, "잘못된 분해 타입!");
                        }
                    }
                }
                break;
            default:
                break;
        }

        if (reverse_sort)
        {
            for (int i = 0; i < data_order.Length; i++)
            {
                data_order[i] = total_list_num - 1 - data_order[i];
            }
        }

        return data_order;
    }

    void updateView()
    {
        //선택중인 오브젝트 정보
        if (upgradeType == UPGRADE_TYPE.MONSTER)
        {
            FrameSelectedObject.sprite = CSVData.Inst.GetSpriteGrade(monsterData.gradeType);
            ImageSelectedObject.sprite = CSVData.Inst.GetMonsterData(monsterData.id).monsterIcon;
            textSelectedObjectUpgrade.text = string.Format("+{0}", monsterData.upgrade);
        }
        else
        {
            FrameSelectedObject.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)equipmentData.grade);
            ImageSelectedObject.sprite = CSVData.Inst.GetEquipmentData(equipmentData.id).equipmentIcon;
            textSelectedObjectUpgrade.text = string.Format("+{0}", equipmentData.upgrade);
        }

        if (upgradeType == UPGRADE_TYPE.MONSTER)//강화에 사용될 오브젝트를 선택했을때
        {
            if (inserted_object_idx > 0)
            {
                FrameInsertObject.gameObject.SetActive(true);
                FrameResultSlot.gameObject.SetActive(true);
                textUgt.gameObject.SetActive(true);

                //강화에 사용될 오브젝트 정보
                UserMonsterData inserted_monster_data = UserDataManager.Inst.GetMonsterInfo(inserted_object_idx);
                if (inserted_monster_data == null)
                {
                    DebugLog.Log(false, "Invalid Insert monster data by SubViewUpgrade");
                    return;
                }

                FrameInsertObject.sprite = CSVData.Inst.GetSpriteGrade(inserted_monster_data.gradeType);
                ImageInsertObject.sprite = CSVData.Inst.GetMonsterData(inserted_monster_data.id).monsterIcon;
                textInsertObjectUpgrade.text = string.Format("+{0}", inserted_monster_data.upgrade);
                textNeedPerCount.text = "";

                DBMonsterUpgradeData upgradeData = CSVData.Inst.GetMonsterUpgradeData(monsterData.grade, monsterData.upgrade, inserted_monster_data.upgrade);
                if (upgradeData == null)
                {
                    DebugLog.Log(false, "Invalid Upgrade Data : " + monsterData.grade + ", " + monsterData.upgrade + ", " + inserted_monster_data.upgrade);
                    return;
                }

                //강화 비용, 성공률
                textUgt.text = string.Format("{0}", (upgradeData.needUTGCount / 10000));
                textSuccessPer.text = string.Format("{0}%", upgradeData.successPer);

                //강화 성공시 오브젝트 정보
                FrameResultSlot.sprite = CSVData.Inst.GetSpriteGrade(monsterData.gradeType);
                imageResultSlot.sprite = CSVData.Inst.GetMonsterData(monsterData.id).monsterIcon;
                textResultUpgrade.text = string.Format("+{0}", monsterData.upgrade + 1);

                buttonUpgrade.interactable = true;
            }
            else
            {
                FrameInsertObject.gameObject.SetActive(false);
                textUgt.gameObject.SetActive(false);
                textSuccessPer.text = "";
                FrameResultSlot.gameObject.SetActive(false);

                buttonUpgrade.interactable = false;
            }
        }
        else if (upgradeType == UPGRADE_TYPE.EQUIPMENT)
        {
            FrameInsertObject.gameObject.SetActive(true);
            FrameResultSlot.gameObject.SetActive(true);
            textUgt.gameObject.SetActive(true);

            DBEquipmentUpgradeData upgradeData = CSVData.Inst.GetEquipmentUpgradeData(equipmentData.grade, (int)equipmentData.equipmentType, equipmentData.upgrade);
            if (upgradeData == null)
            {
                DebugLog.Log(false, "Invalid Upgrade Data : " + equipmentData.grade + ", " + equipmentData.equipmentType + ", " + equipmentData.upgrade);
                return;
            }

            DBItemData scrollData = CSVData.Inst.GetItemData(upgradeData.needItemID);
            if (scrollData == null)
            {
                DebugLog.Log(false, "Invalid Scroll Data : " + upgradeData.needItemID);
                return;
            }

            FrameInsertObject.sprite = CSVData.Inst.GetSpriteGrade(GRADE_TYPE.COMMON);
            ImageInsertObject.sprite = scrollData.ItemIcon;
            //주문서는 업그레이드 표시 X
            textInsertObjectUpgrade.text = "";
            int itemCount = UserDataManager.Inst.GetItemCount(scrollData.id);
            textNeedPerCount.text = upgradeData.needItemCount + "/" + itemCount;
            if (itemCount < upgradeData.needItemCount)
            {
                textNeedPerCount.color = Color.red;
            }
            else
            {
                textNeedPerCount.color = Color.white;
            }

            //강화 비용, 성공률
            textUgt.text = string.Format("{0}", upgradeData.needUTGCount);
            textSuccessPer.text = string.Format("{0}%", upgradeData.successPer);

            //강화 성공시 오브젝트 정보
            FrameResultSlot.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)equipmentData.grade);
            imageResultSlot.sprite = CSVData.Inst.GetEquipmentData(equipmentData.id).equipmentIcon;
            textResultUpgrade.text = string.Format("+{0}", equipmentData.upgrade + 1);

            buttonUpgrade.interactable = true;
        }
        else
        {
            FrameInsertObject.gameObject.SetActive(false);
            textUgt.gameObject.SetActive(false);
            textSuccessPer.text = "";
            FrameResultSlot.gameObject.SetActive(false);

            buttonUpgrade.interactable = false;
        }

    }

    //19.04.12 : 장비강화의 경우, 일단 보유중인 주문서가 자동으로 삽입되도록 함. (추후, 주문서의 종류가 다양해질 경우 선택하도록 변경)
    public void InsertEquipmentUpgradeScroll()
    {
        //아이템(스크롤) 보유 여부 확인

        //inserted_object_idx에 아이템 idx 설정
    }

    public void InsertUnit(int scroll_unit_idx)
    {
        //재료를 다른 몬스터(또는 다른 주문서)로 바꿀 수 있기 때문에 필요 없을듯.
        //if (inserted_object_idx > 0)
        //{
        //    DebugLog.Log(false, "Warning : 이미 필요한 아이템 삽입됨!");
        //}

        UserDataManager u_data = UserDataManager.Inst;
        int unit_idx = 0;

        if (checkInsertedObject(scroll_unit_idx))
        {
            DebugLog.Log(false, "Warning : 이미 분해 슬롯에 등록된 유닛 입니다.");
        }
        else
        {
            if (upgradeType == UPGRADE_TYPE.MONSTER)
            {
                UserMonsterData insertMonsterData = UserDataManager.Inst.GetMonsterInfo(scroll_unit_idx);
                if (insertMonsterData == null)
                {
                    DebugLog.Log(false, "Invalid Request Monster ID : " + scroll_unit_idx);
                    return;
                }

                if (insertMonsterData.state != 1)
                {
                    DebugLog.Log(false, "Invalid Monster State : " + insertMonsterData.state);
                    SimpleErrorPopupVC.Inst.UpdateErrorText("Monster Invalid Monster State");
                    return;
                }

                if (insertMonsterData.partyIndex != 0)
                {
                    DebugLog.Log(false, "Invalid Monster Index : " + insertMonsterData.partyIndex);
                    SimpleErrorPopupVC.Inst.UpdateErrorText("Monster Already In Party");
                    return;
                }

                //같은 종류의 몬스터, 같은 등급, 선택된 몬스터보다 등급이 낮거나 같은 것들만, 
                //if (insertMonsterData.id == partyInfo.MonsterList[i].id && insertMonsterData.grade == partyInfo.MonsterList[i].grade && insertMonsterData.upgrade >= partyInfo.MonsterList[i].upgrade)

                //현재 선택된 몬스터 제외 : insertMonsterData.index >= partyInfo.MonsterList[i].index
            }
            else//Equip
            {
                UserEquipmentData insertEquipmentData = UserDataManager.Inst.GetEquipmentInfo(scroll_unit_idx);
                if (insertEquipmentData == null)
                {
                    DebugLog.Log(false, "Invalid Request Servant ID : " + scroll_unit_idx);
                    return;
                }

                if (insertEquipmentData.state != 1)
                {
                    DebugLog.Log(false, "Invalid Equip State : " + insertEquipmentData.state);
                    return;
                }

                if (insertEquipmentData.isEquiped)
                {
                    DebugLog.Log(false, "Invalid Equip isEquiped : " + insertEquipmentData.isEquiped);
                    return;
                }

                //같은 종류의 장비, 같은 등급, 선택된 장비보다 등급이 낮거나 같은 것들만
            }

            unit_idx = scroll_unit_idx;
            inserted_object_idx = unit_idx;
            updateView();
        }

    }

    bool checkInsertedObject(int unit_idx)
    {
        return unit_idx == inserted_object_idx;
    }

    public void ResetView()
    {
        inserted_object_idx = 0;
        updateView();
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

    public void OnClickButtonUpgrade()
    {
        if ((ulong)(Convert.ToInt32(textUgt.text) * 10000) > UserDataManager.Inst.GetUserUTG())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Not Enough UTG");
            return;
        }

        if (upgradeType == UPGRADE_TYPE.MONSTER)  // Monster
        {
            TopUIManager.Inst.ShowPopupMessage(POPUP_TYPE.CONFIRM, MESSAGE_IDX.UPGRADE_MONSTER_CONFIRM);
        }
        else if (upgradeType == UPGRADE_TYPE.EQUIPMENT) // Equip
        {
            TopUIManager.Inst.ShowPopupMessage(POPUP_TYPE.CONFIRM, MESSAGE_IDX.UPGRADE_EQUIPMENT_CONFIRM);
        }
    }

    public void UpgradeMonster()
    {
        UserMonsterData subMonsterData = UserDataManager.Inst.GetMonsterInfo(inserted_object_idx);
        if (subMonsterData == null)
        {
            DebugLog.Log(false, "Invalid Request Monster ID : " + inserted_object_idx);
            return;
        }

        if (monsterData.state != 1 || subMonsterData.state != 1)
        {
            DebugLog.Log(false, "Invalid Monster State : " + subMonsterData.state);
            SimpleErrorPopupVC.Inst.UpdateErrorText("Invalid Monster State");
            return;
        }

        if (subMonsterData.partyIndex != 0)
        {
            DebugLog.Log(false, "Invalid Monster Index : " + subMonsterData.partyIndex);
            SimpleErrorPopupVC.Inst.UpdateErrorText("Monster Already In Party");
            return;
        }

#if UNITY_EDITOR
        Cheat.Inst.RequestMonsterUpgradeCheat(monsterData.index, subMonsterData.index);
#else
        PacketManager.Inst.RequestMonsterUpgrade(monsterData.index, subMonsterData.index);
#endif
    }

    public void UpgradeEquipment()
    {
        if (textNeedPerCount.color == Color.red)
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Not Enough Scroll");
            return;
        }

        UserEquipmentData userEquipmentData = UserDataManager.Inst.GetEquipmentInfo(equipmentData.index);
        if (userEquipmentData == null)
        {
            DebugLog.Log(false, "Invalid Request Equipment ID : " + equipmentData.index);
            return;
        }

        if (userEquipmentData.state != 1)
        {
            DebugLog.Log(false, "Invalid Equipment State : " + userEquipmentData.state);
            return;
        }

        if (userEquipmentData.equipServantIndex != 0)
        {
            DebugLog.Log(false, "Invalid Equipment Already Equip Servant Index : " + userEquipmentData.equipServantIndex);
            return;
        }

        List<int> addItem = new List<int>();

        DBEquipmentUpgradeData upgradeData = CSVData.Inst.GetEquipmentUpgradeData(equipmentData.grade, (int)equipmentData.equipmentType, equipmentData.upgrade);
        if (upgradeData == null)
        {
            DebugLog.Log(false, "Invalid Upgrade Data : " + equipmentData.grade + ", " + equipmentData.equipmentType + ", " + equipmentData.upgrade);
            return;
        }

        addItem.Add(upgradeData.needItemID);

#if UNITY_EDITOR
        Cheat.Inst.RequestEquipmentUpgradeCheat(userEquipmentData.index);
#else
        PacketManager.Inst.RequestEquipmentUpgrade(equipmentData.index, addItem);
#endif
    }
}
