﻿using System.Collections;
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

    //강화 성공시 만들어질 오브젝트 정보(이미지)
    public Image FrameResultSlot;
    public Image imageResultSlot;
    public Text textResultUpgrade;

    public Button buttonUpgrade;

    public GameObject prefabPartyUnit;
    public GameObject prefabItemUnit;

    public GameObject FrameScroll;
    public ScrollListManager scrollList;

    public List<int> scrollListData = new List<int>();
    SORT_TYPE sortType = 0;

    private PartyInfoVC partyInfo;
    private InventoryVC inventoryInfo;

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
            inventoryInfo = InventoryVC.Inst;
            scrollList.prefabUnit = prefabItemUnit;
            FrameScroll.SetActive(false);

            //선택된 장비 Data
            equipmentData = inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu][inventoryInfo.selected_unit_idx];

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
                    if (monsterData.id == partyInfo.MonsterList[i].id && monsterData.grade == partyInfo.MonsterList[i].grade && monsterData.upgrade >= partyInfo.MonsterList[i].upgrade && monsterData.index != partyInfo.MonsterList[i].index)
                    {
                        scrollListData.Add(i);
                    }
                }
                break;
            case UPGRADE_TYPE.EQUIPMENT:
                //추후 아이템 중, 주문서류만 scrollListData에 삽입.
                break;
            default:
                Debug.Log("Warning : 잘못된 타입 분해!");
                break;

        }
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

        switch (sortType)
        {
            case 0:
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
                            if (inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu][scrollListData[i]].grade < inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu][scrollListData[j]].grade)
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
                            Debug.Log("잘못된 분해 타입!");
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

        if (inserted_object_idx > 0)//강화에 사용될 오브젝트를 선택했을때
        {
            FrameInsertObject.gameObject.SetActive(true);
            FrameResultSlot.gameObject.SetActive(true);
            textUgt.gameObject.SetActive(true);

            if (upgradeType == UPGRADE_TYPE.MONSTER)
            {
                //강화에 사용될 오브젝트 정보
                UserMonsterData inserted_monster_data = UserDataManager.Inst.GetMonsterInfo(inserted_object_idx);
                if (inserted_monster_data == null)
                {
                    Debug.Log("Invalid Insert monster data by SubViewUpgrade");
                    return;
                }

                FrameInsertObject.sprite = CSVData.Inst.GetSpriteGrade(inserted_monster_data.gradeType);
                ImageInsertObject.sprite = CSVData.Inst.GetMonsterData(inserted_monster_data.id).monsterIcon;
                textInsertObjectUpgrade.text = string.Format("+{0}", inserted_monster_data.upgrade);

                DBMonsterUpgradeData upgradeData = CSVData.Inst.GetMonsterUpgradeData(monsterData.grade, monsterData.upgrade, inserted_monster_data.upgrade);
                if(upgradeData == null)
                {
                    Debug.Log("Invalid Upgrade Data : " + monsterData.grade + ", " + monsterData.upgrade + ", " + inserted_monster_data.upgrade);
                    return;
                }

                //강화 비용, 성공률
                textUgt.text = string.Format("{0}", (upgradeData.needUTGCount / 10000) );
                textSuccessPer.text = string.Format("{0}%", upgradeData.successPer);

                //강화 성공시 오브젝트 정보
                FrameResultSlot.sprite = CSVData.Inst.GetSpriteGrade(monsterData.gradeType);
                imageResultSlot.sprite = CSVData.Inst.GetMonsterData(monsterData.id).monsterIcon;
                textResultUpgrade.text = string.Format("+{0}", monsterData.upgrade + 1);
            }
            else
            {
                //FrameInsertObject.sprite = 주문서 등급;
                //ImageInsertObject.sprite = 주문서 이미지;
                //주문서는 업그레이드 표시 X
                textInsertObjectUpgrade.text = "";

                //강화 비용, 성공률
                textUgt.text = string.Format("{0}", 0);
                textSuccessPer.text = string.Format("{0}%", 0);

                //강화 성공시 오브젝트 정보
                FrameResultSlot.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)equipmentData.grade);
                imageResultSlot.sprite = CSVData.Inst.GetEquipmentData(equipmentData.id).equipmentIcon;
                textResultUpgrade.text = string.Format("+{0}", monsterData.upgrade + 1);
            }

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
        //    Debug.Log("Warning : 이미 필요한 아이템 삽입됨!");
        //}

        UserDataManager u_data = UserDataManager.Inst;
        int unit_idx = 0;

        if (checkInsertedObject(scroll_unit_idx))
        {
            Debug.Log("Warning : 이미 분해 슬롯에 등록된 유닛 입니다.");
        }
        else
        {
            if (upgradeType == UPGRADE_TYPE.MONSTER)
            {
                UserMonsterData insertMonsterData = UserDataManager.Inst.GetMonsterInfo(scroll_unit_idx);
                if (insertMonsterData == null)
                {
                    Debug.Log("Invalid Request Monster ID : " + scroll_unit_idx);
                    return;
                }

                if (insertMonsterData.state != 1)
                {
                    Debug.Log("Invalid Monster State : " + insertMonsterData.state);
                    TopUIManager.Inst.ShowSimpleErrorPopup("Monster Invalid Monster State");
                    return;
                }

                if (insertMonsterData.partyIndex != 0)
                {
                    Debug.Log("Invalid Monster Index : " + insertMonsterData.partyIndex);
                    TopUIManager.Inst.ShowSimpleErrorPopup("Monster Already In Party");
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
                    Debug.Log("Invalid Request Servant ID : " + scroll_unit_idx);
                    return;
                }

                if (insertEquipmentData.state != 1)
                {
                    Debug.Log("Invalid Equip State : " + insertEquipmentData.state);
                    return;
                }

                if (insertEquipmentData.isEquiped)
                {
                    Debug.Log("Invalid Equip isEquiped : " + insertEquipmentData.isEquiped);
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

    //강화 완료(서버에서 응답) 후 화면 전체 갱신
    public void updateViewFinishRequest()
    {
        ResetView();

        if (PartyInfoVC.checkInst())
        {
            PartyInfoVC patyInfo = PartyInfoVC.Inst;
            patyInfo.setData();
            patyInfo.resetScroll(patyInfo.currentScrollType);
            patyInfo.updateDetailInfo(patyInfo.scrollList.getFirstItemOrder());
        }
        else if (InventoryVC.checkInst())
        {
            InventoryVC inventory = InventoryVC.Inst;
            inventory.setData();
            inventory.resetScroll(inventory.currentScrollType);
            inventory.updateDetailInfo(inventory.scrollList.getFirstItemOrder());
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
            InventoryVC.Inst.frameScroll.SetActive(true);
        }

        Destroy(this.gameObject);
    }

    public void OnClickButtonUpgrade()
    {
        if (upgradeType == UPGRADE_TYPE.MONSTER)  // Monster
        {
            UserMonsterData subMonsterData = UserDataManager.Inst.GetMonsterInfo(inserted_object_idx);
            if (subMonsterData == null)
            {
                Debug.Log("Invalid Request Monster ID : " + inserted_object_idx);
                return;
            }

            if (monsterData.state != 1 || subMonsterData.state != 1)
            {
                Debug.Log("Invalid Monster State : " + subMonsterData.state);
                TopUIManager.Inst.ShowSimpleErrorPopup("Invalid Monster State");
                return;
            }

            if (subMonsterData.partyIndex != 0)
            {
                Debug.Log("Invalid Monster Index : " + subMonsterData.partyIndex);
                TopUIManager.Inst.ShowSimpleErrorPopup("Monster Already In Party");
                return;
            }

#if UNITY_EDITOR
            Cheat.Inst.RequestMonsterUpgradeCheat(monsterData.index, subMonsterData.index);
#else
            PacketManager.Inst.RequestMonsterUpgrade(monsterData.index, subMonsterData.index);
#endif
        }
        else if (upgradeType == UPGRADE_TYPE.EQUIPMENT) // Equip
        {
            UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(inserted_object_idx);
            if (equipmentData == null)
            {
                Debug.Log("Invalid Request Equipment ID : " + inserted_object_idx);
                return;
            }

            if (equipmentData.state != 1)
            {
                Debug.Log("Invalid Equipment State : " + equipmentData.state);
                return;
            }

            if (equipmentData.equipServantIndex != 0)
            {
                Debug.Log("Invalid Equipment Already Equip Servant Index : " + equipmentData.equipServantIndex);
                return;
            }

            //TODO: 서버요청
        }
    }
}
