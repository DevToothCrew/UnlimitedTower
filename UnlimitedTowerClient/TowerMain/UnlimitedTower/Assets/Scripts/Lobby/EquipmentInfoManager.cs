using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInfoManager : MonoSingleton<EquipmentInfoManager>
{
    public GameObject FrameEquipmentInfo;
    public GameObject FrameNoneEquipment;
    public Text textNoneEquipment;

    //Detail Info UI
    public Image imageEquipment;
    public Text textEquipmentName;
    public Text textGradeText;
    public Text textGrade;
    public Text textUpgradeText;
    public Text textUpgrade;
    public Text textTierText;
    public Text textTier;
    public Image[] imageJobIcon = new Image[5];

    public Text textStatsText;
    public Image imageStatsIcon;
    public Text textStats;

    //ScrollView UI
    public Text textOwned;
    public Text textTotal;
    public GameObject frameScroll;
    public ScrollListManager scrollList;

    private SORT_TYPE sort_type;

    public List<List<UserEquipmentData>> EquipmentList = new List<List<UserEquipmentData>>();

    private int unit_idx;

    private Inventory_Menu_Type selectedMenu;

    private bool is_init_scroll = false;

    public Inventory_Menu_Type GetSelectedMenu()
    {
        return selectedMenu;
    }

    public void updateAllView()
    {
        FrameEquipmentInfo.SetActive(false);
        FrameNoneEquipment.SetActive(false);

        if (InventoryVC.Inst.selectedMenu == Inventory_Menu_Type.WEAPON || InventoryVC.Inst.selectedMenu == Inventory_Menu_Type.ARMOR ||
            InventoryVC.Inst.selectedMenu == Inventory_Menu_Type.ACCESSORY)
        {
            selectedMenu = InventoryVC.Inst.selectedMenu;

            setData();

            if (EquipmentList[(int)selectedMenu].Count > 0)
            {
                FrameEquipmentInfo.SetActive(true);

                if (is_init_scroll)
                {
                    scrollList.SetItemOrder(getOrder());
                }
                else
                {
                    is_init_scroll = true;
                    initScrollList();
                }

                textOwned.text = string.Format("{0}", EquipmentList[(int)selectedMenu].Count);
                textTotal.text = string.Format("/ {0}", UserDataManager.Inst.GetUserInventoryInfo().equipmentInventory);
                if (EquipmentList[(int)selectedMenu].Count >= UserDataManager.Inst.GetUserInventoryInfo().equipmentInventory)
                {
                    textOwned.color = Color.red;
                }
                else
                {
                    textOwned.color = Color.white;
                }

                updateDetailInfo(scrollList.getFirstItemOrder());
            }
            else
            {
                FrameNoneEquipment.SetActive(true);

                textNoneEquipment.text = string.Format("None {0}", selectedMenu);
            }
        }
        else
        {   
            DebugLog.Log(false, "Invalid Equipment Menu");
            return;
        }
    }

    public void setData()
    {
        for (int i = 0; i < EquipmentList.Count; i++)
        {
            EquipmentList[i].Clear();
        }

        EquipmentList.Clear();

        for (int i = 0; i < 3; i++) // 0:weapon, 1:armor, 2:acc
        {
            List<UserEquipmentData> item_list = new List<UserEquipmentData>();
            EquipmentList.Add(item_list);
        }


        for (int i = 0; i < UserDataManager.Inst.GetEquipmentList().Count; i++)
        {
            UserEquipmentData equip_info = UserDataManager.Inst.GetEquipmentList()[i];

            if (EQUIPMENT_TYPE.WEAPON == equip_info.equipmentType)
            {
                EquipmentList[(int)Inventory_Menu_Type.WEAPON].Add(equip_info);
            }
            else if (EQUIPMENT_TYPE.ARMOR == equip_info.equipmentType)
            {
                EquipmentList[(int)Inventory_Menu_Type.ARMOR].Add(equip_info);
            }
            else if (EQUIPMENT_TYPE.ACCESSSORY == equip_info.equipmentType)
            {
                EquipmentList[(int)Inventory_Menu_Type.ACCESSORY].Add(equip_info);
            }
            else
            {
                DebugLog.Log(false, "Equipment type : max");
            }
        }
    }

    public int GetUnitIdx()
    {
        return unit_idx;
    }

    //상세정보창 Update
    public void updateDetailInfo(int _selected_unit_idx)
    {
        unit_idx = _selected_unit_idx;

        UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(EquipmentList[(int)selectedMenu][unit_idx].index);
        if (equipmentData == null)
        {
            DebugLog.Log(false, "Invalid Equipment Inddex : " + unit_idx);
        }

        DBEquipmentData dBEquipmentData = CSVData.Inst.GetEquipmentData(equipmentData.id);
        if (dBEquipmentData == null)
        {
            DebugLog.Log(false, "Invalid equipment ID : " + equipmentData.id);
        }

        imageEquipment.sprite = dBEquipmentData.equipmentIcon;
        textEquipmentName.text = dBEquipmentData.name;
        //textCurrentGradeText.text;
        textGrade.text = string.Format("{0}", ((GRADE_TYPE)equipmentData.grade));
        //textCurrentUpgradeText.text;
        textUpgrade.text = string.Format("+{0}", (equipmentData.upgrade)); ;
        //textCurrentTierText.text;
        textTier.text = string.Format("{0}T", dBEquipmentData.tier);
        //textCurrentJobText.text;

        Color temp_color = imageJobIcon[0].color;
        for (int i = 0; i < 5; i++)
        {
            imageJobIcon[i].color = new Color(temp_color.r, temp_color.g, temp_color.b, 0.2f);
        }

        if (dBEquipmentData.jobLimit == SERVANT_JOB_FLAG.All)
        {
            for (int i = 0; i < 5; i++)
            {
                imageJobIcon[i].color = new Color(temp_color.r, temp_color.g, temp_color.b, 1f);
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                SERVANT_JOB_FLAG checkJob = (SERVANT_JOB_FLAG)Math.Pow(2, (double)(i + 1));
                if (dBEquipmentData.isEquipAble(checkJob) == true)
                {
                    imageJobIcon[i].color = new Color(temp_color.r, temp_color.g, temp_color.b, 1f);
                }
            }
        }

        imageStatsIcon.sprite = CSVData.Inst.GetSpriteOptionType(dBEquipmentData.optionType);
        textStats.text = string.Format("{0}", equipmentData.value);
    }

    public void ResetScrollListBySortType(SORT_TYPE type)
    {
        sort_type = type;
        scrollList.SetItemOrder(getOrder());
        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
        scrollList.ScrollViewDidScroll();
        EquipmentInfoManager.Inst.updateDetailInfo(scrollList.getFirstItemOrder());
    }

    //현재 화면에 따른 스크롤 UI 재설정 
    public void resetScroll()
    {
        textOwned.text = string.Format("{0}", EquipmentList[(int)selectedMenu].Count);
        textTotal.text = string.Format("/ {0}", UserDataManager.Inst.GetUserInventoryInfo().equipmentInventory);
        if (EquipmentList[(int)selectedMenu].Count >= UserDataManager.Inst.GetUserInventoryInfo().equipmentInventory)
        {
            textOwned.color = Color.red;
        }
        else
        {
            textOwned.color = Color.white;
        }
        scrollList.SetItemOrder(getOrder());
        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
    }

    //스크롤 생성
    void initScrollList()
    {
        scrollList.Init(this, 20, EquipmentList[(int)selectedMenu].Count, getOrder());
    }


    //스크롤 정렬
    private int[] getOrder()
    {
        int[] data_order;
        int total_list_num = EquipmentList[(int)selectedMenu].Count;

        data_order = new int[total_list_num];

        for (int i = 0; i < data_order.Length; i++)
        {
            data_order[i] = 0;
        }

        UserDataManager u_data = UserDataManager.Inst;

        switch (sort_type)
        {
            case SORT_TYPE.POWER:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (u_data.GetEquipmentInfo(EquipmentList[(int)selectedMenu][i].index).value < u_data.GetEquipmentInfo(EquipmentList[(int)selectedMenu][j].index).value)
                        {
                            data_order[i]++;
                        }
                        else
                        {
                            data_order[j]++;
                        }
                    }
                }
                break;
            case SORT_TYPE.GRADE:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (u_data.GetEquipmentInfo(EquipmentList[(int)selectedMenu][i].index).grade > u_data.GetEquipmentInfo(EquipmentList[(int)selectedMenu][j].index).grade)
                        {
                            data_order[i]++;
                        }
                        else
                        {
                            data_order[j]++;
                        }
                    }
                }
                break;
            case SORT_TYPE.LEVEL_OR_TIER:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (CSVData.Inst.GetEquipmentData(EquipmentList[(int)selectedMenu][i].id).tier < CSVData.Inst.GetEquipmentData(EquipmentList[(int)selectedMenu][j].id).tier)
                        {
                            data_order[i]++;
                        }
                        else
                        {
                            data_order[j]++;
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
                        if (u_data.GetEquipmentInfo(EquipmentList[(int)selectedMenu][i].index).upgrade < u_data.GetEquipmentInfo(EquipmentList[(int)selectedMenu][j].index).upgrade)
                        {
                            data_order[i]++;
                        }
                        else
                        {
                            data_order[j]++;
                        }
                    }
                }
                break;
            default:
                break;
        }

        return data_order;
    }


    //장비 업그레이드
    public void OnClickUpgrade()
    {
        InventoryVC.Inst.ShowUpgrade();
    }

    //분해 버튼
    public void OnClickDeconstruction()
    {
        InventoryVC.Inst.ShowDeconstruction();
    }
}
