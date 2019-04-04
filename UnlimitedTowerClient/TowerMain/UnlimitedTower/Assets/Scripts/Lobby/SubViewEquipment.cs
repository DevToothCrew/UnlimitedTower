using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubViewEquipment : MonoSingleton<SubViewEquipment>
{
    //Current Item UI
    public GameObject FrameCurrentItemInfo;
    public GameObject FrameCurrentItemNone;
    public Image imageCurrentItem;
    public Text textCurrentItemName;
    public Text textCurrentGradeText;
    public Text textCurrentGrade;
    public Text textCurrentUpgradeText;
    public Text textCurrentUpgrade;
    public Text textCurrentTierText;
    public Text textCurrentTier;
    public Text textCurrentJobText;
    public Text textCurrentJob;
    public Image[] imageCurrentJob = new Image[5];
    public Text textCurrentStatsText;
    public Image[] imageCurrentStat = new Image[3];
    public Text[] textCurrentStat = new Text[3];


    //Change Item UI
    public GameObject FrameChangeItemInfo;
    public GameObject FrameChangeItemNone;
    public Image imageChangeItem;
    public Text textChangeItemName;
    public Text textChangeGradeText;
    public Text textChangeGrade;
    public Text textChangeUpgradeText;
    public Text textChangeUpgrade;
    public Text textChangeTierText;
    public Text textChangeTier;
    public Text textChangeJobText;
    public Text textChangeJob;
    public Image[] imageChangeJob = new Image[5];
    public Text textChangeStatsText;
    public Image[] imageChangeStat = new Image[3];
    public Text[] textChangeStat = new Text[3];


    //Change State UI
    public Text textStats;
    public Text textStr;
    public Text textStrChange;
    public Text textDex;
    public Text textDexChange;
    public Text textInt;
    public Text textIntChange;

    public Text textHp;
    public Text textHpChange;
    public Text textSpeed;
    public Text textSpeedChange;

    public Text textPAtk;
    public Text textPAtkChange;
    public Text textPDef;
    public Text textPDefChange;
    public Text textPCri;
    public Text textPCriChange;
    public Text textPCriPer;
    public Text textPCriPerChange;

    public Text textMAtk;
    public Text textMAtkChange;
    public Text textMDef;
    public Text textMDefChange;
    public Text textMCri;
    public Text textMCriChange;
    public Text textMCriPer;
    public Text textMCriPerChange;

    public Button buttonClear;
    public Button buttonChange;

    public Sprite[] spriteStat = new Sprite[10];

    private int selectedItemIdx;
    private PartyInfoVC partyInfo;
    public List<UserEquipmentData> EquipmentList = new List<UserEquipmentData>();

    private int[] current_stat = new int[10];
    private int[] change_stat = new int[10];

    private EQUIPMENT_TYPE selectedEquipType;

    //Scroll 
    public ScrollListManager scrollList;
    enum sort_type
    {
        GRADE = 0,
        LEVEL,
        POWER,
        OPTAIN
    }
    sort_type sortType = 0;



    void Awake () {
        partyInfo = PartyInfoVC.Inst;
        initScrollList();
        updateAllView();
    }

    void setData()
    {
        EquipmentList.Clear();

        if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.EQUIPMENT_WEAPON)
        {
            selectedEquipType = EQUIPMENT_TYPE.WEAPON;
        }
        else if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.EQUIPMENT_ARMOR)
        {
            selectedEquipType = EQUIPMENT_TYPE.ARMOR;
        }
        else if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.EQUIPMENT_ACC)
        {
            selectedEquipType = EQUIPMENT_TYPE.ACCESSSORY;
        }
        else
        {
            selectedEquipType = EQUIPMENT_TYPE.MAX;
            Debug.Log("Equipment type : max");
        }
        
        for (int i = 0; i < UserDataManager.Inst.GetEquipmentList().Count; i++)
        {
            UserEquipmentData equip_info = UserDataManager.Inst.GetEquipmentList()[i];

            if (equip_info.equipmentType == selectedEquipType)
            {
                Debug.Log("Item type [ " + i + " ]" + equip_info.equipmentType);
                EquipmentList.Add(equip_info);
            }
        }

    }


    //스크롤 생성
    void initScrollList()
    {
        setData();

        scrollList.Init(this, 20, EquipmentList.Count, getOrder());
    }


    //스크롤 정렬
    private int[] getOrder()
    {
        int[] data_order;
        int total_list_num = EquipmentList.Count;

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
                        if (EquipmentList[i].grade < EquipmentList[j].grade)
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

    void updateAllView () {
        //Current Item
        UserServantData servantData = partyInfo.ServantList[partyInfo.selected_unit_idx];
        if (servantData == null)
        {
            Debug.Log("Invalid Servant Inddex : " + partyInfo.selected_unit_idx);
        }

        DBServantData dBServantData = CSVData.Inst.GetServantData(servantData.id);
        if (dBServantData == null)
        {
            Debug.Log("Invalid Servant ID : " + servantData.id);
        }

        UserEquipmentData currentEquipmentData = null;
        int current_item_idx = 0;
        if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.EQUIPMENT_WEAPON)
        {
            current_item_idx = servantData.equipmentDic[EQUIPMENT_TYPE.WEAPON];
        }
        else if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.EQUIPMENT_ARMOR)
        {
            current_item_idx = servantData.equipmentDic[EQUIPMENT_TYPE.ARMOR];
        }
        else if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.EQUIPMENT_ACC)
        {
            current_item_idx = servantData.equipmentDic[EQUIPMENT_TYPE.ACCESSSORY];
        }
        else//EQUIPMENT_TYPE.MAX
        {
            current_item_idx = servantData.equipmentDic[EQUIPMENT_TYPE.MAX];
        }

        if (current_item_idx >= 0)
        {
            FrameCurrentItemInfo.SetActive(true);
            FrameCurrentItemNone.SetActive(false);
            currentEquipmentData = UserDataManager.Inst.GetEquipmentList()[current_item_idx];

            if (currentEquipmentData == null)
            {
                Debug.Log("Invalid Equipment data");
            }


            DBEquipmentData dBEquipment = CSVData.Inst.GetEquipmentData(currentEquipmentData.id);
            if (dBEquipment == null)
            {
                Debug.Log("Invalid equipmentData ID : " + currentEquipmentData.id);
            }


            imageCurrentItem.sprite = dBEquipment.equipmentIcon;
            textCurrentItemName.text = dBEquipment.name;
            //textCurrentGradeText.text;
            textCurrentGrade.text = string.Format("{0}", ((GRADE_TYPE)currentEquipmentData.grade));
            //textCurrentUpgradeText.text;
            textCurrentUpgrade.text = string.Format("+{0}", (currentEquipmentData.upgrade)); ;
            //textCurrentTierText.text;
            textCurrentTier.text = string.Format("{0}T", dBEquipment.tier);
            //textCurrentJobText.text;
            for (int i = 0; i < 5; i++)
            {
                imageCurrentJob[i].enabled = false;
            }

            int able_job = 0;   //착용 가능한 직업 수
            for (SERVANT_JOB_FLAG i = SERVANT_JOB_FLAG.Magician; i > SERVANT_JOB_FLAG.None; i--)
            {
                if (dBEquipment.isEquipAble(i))
                {
                    imageCurrentJob[able_job].enabled = true;
                    //imageCurrentJob[able_job].sprite = dBEquipment.;
                }

            }

            if (dBEquipment.jobLimit == SERVANT_JOB_FLAG.None)
            {
                textCurrentJob.text = "None";
            }
            else if (dBEquipment.jobLimit == SERVANT_JOB_FLAG.All)
            {
                textCurrentJob.text = "All";
            }
            else
            {
                textCurrentJob.text = "";
            }
            

            //초기화
            for (int i = 0; i < 10; i++)
            {
                current_stat[i] = 0;
            }

            for (EQUIPMENT_OPTION_TYPE type = EQUIPMENT_OPTION_TYPE.NONE; type < EQUIPMENT_OPTION_TYPE.MAX; type++)
            {
                if (currentEquipmentData.optionType == type)
                {
                    imageCurrentStat[0].sprite = spriteStat[(int)type];
                    textCurrentStat[0].text = string.Format("{0}", currentEquipmentData.value);
                    current_stat[(int)type] = currentEquipmentData.value;
                }
            }

        }
        else
        {
            Debug.Log("current item none");
            FrameCurrentItemInfo.SetActive(false);
            FrameCurrentItemNone.SetActive(true);
        }

        updateChangeItemInfo(selectedItemIdx);

        /* ATK = 1,
           MATK = 2,
           DEF = 3,
           MDEF = 4,

           HP = 5,

           STR = 6,
           DEX = 7,
           INT = 8,

           현재 크리티컬 관련, 스피드는 없음
         */
        
        textStr.text = string.Format("{0}", servantData.status.basicStr + SetChangeValue(textStrChange, current_stat[6], change_stat[6]));
        textDex.text = string.Format("{0}", servantData.status.basicDex + SetChangeValue(textDexChange, current_stat[7], change_stat[7]));
        textInt.text = string.Format("{0}", servantData.status.basicInt + SetChangeValue(textIntChange, current_stat[8], change_stat[8]));

        textHp.text = string.Format("{0}", servantData.maxHP + SetChangeValue(textIntChange, current_stat[8], change_stat[5]));
        textSpeed.text = string.Format("{0}", dBServantData.speed);

        textPAtk.text = string.Format("{0}", servantData.atk + SetChangeValue(textPAtkChange, current_stat[1], change_stat[1]));
        textPDef.text = string.Format("{0}", servantData.def + SetChangeValue(textPDefChange, current_stat[3], change_stat[3]));

        textMAtk.text = string.Format("{0}", servantData.mAtk + SetChangeValue(textMAtkChange, current_stat[2], change_stat[2]));
        textMDef.text = string.Format("{0}", servantData.mDef + SetChangeValue(textMDefChange, current_stat[4], change_stat[4]));

        textPCri.text = string.Format("{0}", dBServantData.criDmg);
        textPCriPer.text = string.Format("{0}", dBServantData.criPer);

        textMCri.text = string.Format("{0}", dBServantData.mcriDmg);
        textMCriPer.text = string.Format("{0}", dBServantData.mcriPer);

    }

    public void updateChangeItemInfo(int selectedChangeItemIdx)
    {
        UserServantData servantData = partyInfo.ServantList[partyInfo.selected_unit_idx];
        //Change Item

        selectedItemIdx = selectedChangeItemIdx;
        int change_item_idx = selectedItemIdx;

        if (change_item_idx >= 0)
        {
            FrameChangeItemInfo.SetActive(true);
            FrameChangeItemNone.SetActive(false);
            UserEquipmentData changeEquipmentData = UserDataManager.Inst.GetEquipmentList()[change_item_idx];

            if (changeEquipmentData == null)
            {
                Debug.Log("Invalid Equipment data");
            }


            DBEquipmentData dBChangeEquipment = CSVData.Inst.GetEquipmentData(changeEquipmentData.id);
            if (dBChangeEquipment == null)
            {
                Debug.Log("Invalid equipmentData ID : " + changeEquipmentData.id);
            }


            imageChangeItem.sprite = dBChangeEquipment.equipmentIcon;
            textChangeItemName.text = dBChangeEquipment.name;
            //textCurrentGradeText.text;
            textChangeGrade.text = string.Format("{0}", ((GRADE_TYPE)changeEquipmentData.grade));
            //textCurrentUpgradeText.text;
            textChangeUpgrade.text = string.Format("+{0}", (changeEquipmentData.upgrade)); ;
            //textCurrentTierText.text;
            textChangeTier.text = string.Format("{0}T", dBChangeEquipment.tier);
            //textCurrentJobText.text;
            for (int i = 0; i < 5; i++)
            {
                imageChangeJob[i].enabled = false;
            }

            if (dBChangeEquipment.jobLimit == SERVANT_JOB_FLAG.None)
            {
                textChangeJob.text = "None";
            }
            else if (dBChangeEquipment.jobLimit == SERVANT_JOB_FLAG.All)
            {
                textChangeJob.text = "All";
            }
            else
            {
                textChangeJob.text = "";
                int able_job_count = 0;
                int check_job = (int)SERVANT_JOB_FLAG.Last;
                while (check_job >= (int)SERVANT_JOB_FLAG.First)
                {
                    if (dBChangeEquipment.isEquipAble((SERVANT_JOB_FLAG)check_job))
                    {
                        Debug.Log("check Job :" + (SERVANT_JOB_FLAG)check_job);

                        imageChangeJob[able_job_count].enabled = true;
                        able_job_count++;

                        if (able_job_count >= imageChangeJob.Length)
                        {
                            Debug.Log("up to 5 job");
                            break;
                        }
                    }

                    check_job = check_job >> 1;
                    
                }

                for (SERVANT_JOB_FLAG i = SERVANT_JOB_FLAG.None; i < SERVANT_JOB_FLAG.Magician; i++)
                {
                    //Debug.Log("Job name? :" + i);
                    //if (dBChangeEquipment.isEquipAble(i))
                    //{
                    //    imageChangeJob[able_job].enabled = true;
                    //    //imageChangeJob[able_job].sprite = dBChangeEquipment.;
                    //    able_job++;
                    //}

                }
            }

            //초기화
            for (int i = 0; i < 10; i++)
            {
                change_stat[i] = 0;
            }

            for (EQUIPMENT_OPTION_TYPE type = EQUIPMENT_OPTION_TYPE.NONE; type < EQUIPMENT_OPTION_TYPE.MAX; type++)
            {
                if (changeEquipmentData.optionType == type)
                {
                    imageChangeStat[0].sprite = spriteStat[(int)type];
                    textChangeStat[0].text = string.Format("{0}", changeEquipmentData.value);
                    change_stat[(int)type] = changeEquipmentData.value;
                }
            }
        }
        else
        {
            Debug.Log("change item nono");
            FrameChangeItemInfo.SetActive(false);
            FrameChangeItemNone.SetActive(true);
        }
    }

    public int SetChangeValue(Text textChangeValue, int current_item_stat, int change_item_stat)
    {
        int change_value = change_item_stat- current_item_stat;
        if (change_value > 0)
        {
            textChangeValue.gameObject.SetActive(true);
            textChangeValue.text = string.Format("(+{0})", change_value);
            textChangeValue.color = Color.green;
        }
        else if (change_value < 0)
        {
            textChangeValue.gameObject.SetActive(true);
            textChangeValue.text = string.Format("(-{0})", change_value);
            textChangeValue.color = Color.red;
        }
        else
        {
            textChangeValue.gameObject.SetActive(false);
        }

        return change_value;
    }

    public void SearchUnitItemIdx()
    {
        List<UserEquipmentData> user_equipment_list = UserDataManager.Inst.GetEquipmentList();
        
        int equip_idx = 0;
        for (int i = 0; i < user_equipment_list.Count; i++)
        {

        }

        if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.EQUIPMENT_WEAPON)
        {
            
        }
        else if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.EQUIPMENT_ARMOR)
        {

        }
        else if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.EQUIPMENT_ACC)
        {

        }
    }

    public void OnClickClear()
    {

    }

    public void OnClickChange()
    {
        // 선택 서번트 인덱스
        int servantIndex = partyInfo.selected_unit_idx;

#if UNITY_EDITOR
        Cheat.Inst.RequestEquipServantCheat(servantIndex, selectedEquipType, selectedItemIdx);
#else
        PacketManager.Inst.RequestEquipServant(servantIndex, selectedEquipType, selectedItemIdx);        
#endif

    }

    public void OnClickClose()
    {
        partyInfo.frameScroll.SetActive(true);
        Destroy(this.gameObject);
    }
}
