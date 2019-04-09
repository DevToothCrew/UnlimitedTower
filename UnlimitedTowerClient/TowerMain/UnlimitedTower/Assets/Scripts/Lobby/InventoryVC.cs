using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryVC : MonoSingleton<InventoryVC> {
    public GameObject frameItemInfo;

    //SubView
    public GameObject SubViewUpgrade;
    public GameObject FrameSubView;


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
    public Image imageItem;
    public Text textItemName;
    public Text textGradeText;
    public Text textGrade;
    public Text textUpgradeText;
    public Text textUpgrade;
    public Text textTierText;
    public Text textTier;
    public Text textJobText;
    public Text textJob;
    public Image[] imageJobIcon = new Image[5];

    public Text textStatsText;
    public Image[] imageStatsIcon = new Image[3];
    public Text[] textStats = new Text[3];


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


    public List<List<UserEquipmentData>> EquipmentList = new List<List<UserEquipmentData>>();

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
            List<UserEquipmentData> item_list = new List<UserEquipmentData>();
            EquipmentList.Add(item_list);
        }
            

        for (int i = 0; i < UserDataManager.Inst.GetEquipmentList().Count; i++)
        {
            UserEquipmentData equip_info = UserDataManager.Inst.GetEquipmentList()[i];

            if (EQUIPMENT_TYPE.WEAPON == equip_info.equipmentType)
            {
                EquipmentList[(int)menu_type.WEAPON].Add(equip_info);
            }
            else if (EQUIPMENT_TYPE.ARMOR == equip_info.equipmentType)
            {
                EquipmentList[(int)menu_type.ARMOR].Add(equip_info);
            }
            else if (EQUIPMENT_TYPE.ACCESSSORY == equip_info.equipmentType)
            {
                EquipmentList[(int)menu_type.ACCESSORY].Add(equip_info);
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

            if (EquipmentList[i].Count > 0)
            {
                buttonMenu[i].interactable = true;
            }
            else
            {
                buttonMenu[i].interactable = false;
            }
        }

        if (EquipmentList[(int)selectedMenu].Count > 0)
        {
            frameItemInfo.SetActive(true);
            frameScroll.SetActive(true);

            buttonMenu[(int)selectedMenu].image.sprite = buttonMenu[(int)selectedMenu].spriteState.pressedSprite;
            buttonMenu[(int)selectedMenu].GetComponentInChildren<Text>().color = Color.black;

            resetScroll(currentScrollType);
        }
        else
        {
            frameItemInfo.SetActive(false);
            frameScroll.SetActive(false);
        }

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
                        if (UserDataManager.Inst.GetEquipmentInfo(EquipmentList[(int)selectedMenu][i].index).grade < UserDataManager.Inst.GetEquipmentInfo(EquipmentList[(int)selectedMenu][j].index).grade)
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

        UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(EquipmentList[(int)selectedMenu][selected_unit_idx].index);
        if (equipmentData == null)
        {
            Debug.Log("Invalid Equipment Inddex : " + selected_unit_idx);
        }

        DBEquipmentData dBEquipmentData = CSVData.Inst.GetEquipmentData(equipmentData.id);
        if (dBEquipmentData == null)
        {
            Debug.Log("Invalid equipment ID : " + equipmentData.id);
        }

        imageItem.sprite = dBEquipmentData.equipmentIcon;
        textItemName.text = dBEquipmentData.name;
        //textCurrentGradeText.text;
        textGrade.text = string.Format("{0}", ((GRADE_TYPE)equipmentData.grade));
        //textCurrentUpgradeText.text;
        textUpgrade.text = string.Format("+{0}", (equipmentData.upgrade)); ;
        //textCurrentTierText.text;
        textTier.text = string.Format("{0}T", dBEquipmentData.tier);
        //textCurrentJobText.text;
        for (int i = 0; i < 5; i++)
        {
            imageJobIcon[i].enabled = false;
        }

        if (dBEquipmentData.jobLimit == SERVANT_JOB_FLAG.None)
        {
            textJob.text = "None";
        }
        else if (dBEquipmentData.jobLimit == SERVANT_JOB_FLAG.All)
        {
            textJob.text = "All";
        }
        else
        {
            textJob.text = "";
            int able_job_count = 0;
            int check_job = (int)SERVANT_JOB_FLAG.Last;
            while (check_job >= (int)SERVANT_JOB_FLAG.First)
            {
                if (dBEquipmentData.isEquipAble((SERVANT_JOB_FLAG)check_job))
                {
                    Debug.Log("check Job :" + (SERVANT_JOB_FLAG)check_job);

                    imageJobIcon[able_job_count].enabled = true;
                    able_job_count++;

                    if (able_job_count >= imageJobIcon.Length)
                    {
                        Debug.Log("up to 5 job");
                        break;
                    }
                }

                check_job = check_job >> 1;

            }
        }

        for (EQUIPMENT_OPTION_TYPE type = EQUIPMENT_OPTION_TYPE.NONE; type < EQUIPMENT_OPTION_TYPE.MAX; type++)
        {
            if (equipmentData.optionType == type)
            {
                //imageStatsIcon[0].sprite = spriteStat[(int)type];
                textStats[0].text = string.Format("{0}", equipmentData.value);
            }
        }

    }


    public void OnClickDeconstruction()
    {
        frameScroll.SetActive(false);

        GameObject objSubView = Instantiate(Resources.Load("UI/Lobby/SubViewDeconstruction")) as GameObject;
        objSubView.name = "SubViewDeconstruction";
        objSubView.transform.SetParent(FrameSubView.transform);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        SubViewDeconstruction s_deconstruction = objSubView.GetComponent<SubViewDeconstruction>();
        s_deconstruction.SetDeconstructionType(DECONSTRUCTION_TYPE.EQUIPMENT);
    }

}
