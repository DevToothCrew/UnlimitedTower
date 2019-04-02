using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryVC : MonoBehaviour {
    public GameObject frameItemInfo;

    //SubView
    public GameObject SubViewDeconstruction;
    public GameObject SubViewUpgrade;


    //Menu Buttons UI
    public Text textButtonBack;
    public Button[] buttonMenu = new Button[3];

    //ScrollView UI
    public Button buttonSort;
    public Text textOwned;
    public Text textTotal;

    public GameObject frameSortType;
    public Button[] buttonSortType = new Button[4];

    public GameObject frameScroll;
    public ScrollListManager scrollList;

    //Detail Info UI
    public Text textLevel;
    public RectTransform rectExp;
    public Text textCharacterName;

    public Text textStr;
    public Text textDex;
    public Text textInt;

    public Text textPAtk;
    public Text textPDef;
    public Text textPCri;
    public Text textPCriPer;
    public Text textMAtk;
    public Text textMDef;
    public Text textMCri;
    public Text textMCriPer;
    public Text textSpeed;

    //public Text textA

    //UI Set Data
    public enum menu_type
    {
        WEAPON = 0,
        ARMOR,
        ACCESSORY
    }
    public menu_type selectedMenu = menu_type.WEAPON;

    enum sort_type
    {
        GRADE = 0,
        LEVEL,
        POWER,
        OPTAIN
    }
    sort_type sortType = 0;


    public List<List<int>> EquipmentList = new List<List<int>>();

    [HideInInspector]
    public int selected_tab = 0;
    [HideInInspector]
    public int selected_unit_idx = 0;

    public enum scroll_type
    {
        WEAPON_INFO = 0,      
        ARMOR_INFO,           
        ACCESSORY_INFO,
        DECONSTRUCTION_WEAPON,
        DECONSTRUCTION_ARMOR,
        DECONSTRUCTION_ACCESSORY,
    }
    public scroll_type currentScrollType = 0;


    void setData()
    {
        for (int i=0; i< EquipmentList.Count; i++)
        {
            EquipmentList[i].Clear();
        }

        EquipmentList.Clear();

        for (int i = 0; i < 3; i++) // 0:weapon, 1:armor, 2:acc
        {
            List<int> item_list = new List<int>();
            EquipmentList.Add(item_list);
        }
            

        for (int i = 0; i < UserDataManager.Inst.GetEquipmentList().Count; i++)
        {
            UserEquipmentData equip_info = UserDataManager.Inst.GetEquipmentInfo(i);

            if (EQUIPMENT_TYPE.WEAPON == equip_info.equipmentType)
            {

                EquipmentList[(int)menu_type.WEAPON].Add(equip_info.index);
            }
            else if (EQUIPMENT_TYPE.WEAPON == equip_info.equipmentType)
            {
                EquipmentList[(int)menu_type.ARMOR].Add(equip_info.index);
            }
            else if (EQUIPMENT_TYPE.WEAPON == equip_info.equipmentType)
            {
                EquipmentList[(int)menu_type.ACCESSORY].Add(equip_info.index);
            }
            else
            {
                Debug.Log("Equipment type : max");
            }
        }

    }

    void Start()
    {
        initScrollList();
        OnClickMenuButton(0);
    }


    //화면 전체 Update (메뉴버튼, 상세정보창, 스크롤 등)
    void updateAllView()
    {
        for (int i = 0; i < 3; i++)
        {
            buttonMenu[i].image.sprite = buttonMenu[i].spriteState.disabledSprite;
            buttonMenu[i].GetComponentInChildren<Text>().color = Color.white;
        }

        buttonMenu[(int)selectedMenu].image.sprite = buttonMenu[(int)selectedMenu].spriteState.pressedSprite;
        buttonMenu[(int)selectedMenu].GetComponentInChildren<Text>().color = Color.black;

        resetScroll(currentScrollType);

    }

    //좌측 메뉴버튼 클릭
    public void OnClickMenuButton(int tag)
    {
        if (tag == (int)menu_type.WEAPON)
        {
            selectedMenu = menu_type.WEAPON;
            currentScrollType = scroll_type.WEAPON_INFO;
        }
        else if (tag == (int)menu_type.ARMOR)
        {
            selectedMenu = menu_type.ARMOR;
            currentScrollType = scroll_type.ARMOR_INFO;
        }
        else
        {
            selectedMenu = menu_type.ACCESSORY;
            currentScrollType = scroll_type.ACCESSORY_INFO;
        }
        updateAllView();

    }


    //현재 화면에 따른 스크롤 UI 재설정 
    public void resetScroll(scroll_type type)
    {
        currentScrollType = type;
        textOwned.text = string.Format("{0}", EquipmentList[(int)selectedMenu].Count);
        textTotal.text = string.Format("/ {0}", 100);

        scrollList.SetItemOrder(getOrder());
        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
    }


    //스크롤 생성
    void initScrollList()
    {
        setData();

        scrollList.Init(this, 20, EquipmentList[0].Count, getOrder());
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

        switch (sortType)
        {
            case 0:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (UserDataManager.Inst.GetEquipmentInfo(EquipmentList[(int)selectedMenu][i]).grade < UserDataManager.Inst.GetEquipmentInfo(EquipmentList[(int)selectedMenu][j]).grade)
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

    //상세정보창 Update
    public void updateDetailInfo(int _selected_unit_idx)
    {
        selected_unit_idx = _selected_unit_idx;

        UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(EquipmentList[(int)selectedMenu][selected_unit_idx]);
        if (equipmentData == null)
        {
            Debug.Log("Invalid Equipment Inddex : " + selected_unit_idx);
        }

        DBEquipmentData dBEquipmentData = CSVData.Inst.GetEquipmentData(equipmentData.id);
        if (dBEquipmentData == null)
        {
            Debug.Log("Invalid equipment ID : " + equipmentData.id);
        }

    }


    public void OnClickDeconstruction()
    {
        SubViewDeconstruction.SetActive(true);

        if (selectedMenu == menu_type.WEAPON)
            currentScrollType = scroll_type.DECONSTRUCTION_WEAPON;
        else if (selectedMenu == menu_type.ARMOR)
            currentScrollType = scroll_type.DECONSTRUCTION_ARMOR;
        else if (selectedMenu == menu_type.ACCESSORY)
            currentScrollType = scroll_type.DECONSTRUCTION_ACCESSORY;

        scrollList.SetItemOrder(getOrder());
    }

}
