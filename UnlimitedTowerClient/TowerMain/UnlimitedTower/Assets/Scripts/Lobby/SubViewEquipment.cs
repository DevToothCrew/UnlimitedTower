using System;
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

    private int selectedItemIdx = -1;
    private PartyInfoVC partyInfo;
    public List<UserEquipmentData> EquipmentList = new List<UserEquipmentData>();

    private int[] current_stat = new int[10];
    private int[] change_stat = new int[10];
    private Status changeStatus = new Status();

    private EQUIPMENT_TYPE selectedEquipType;

    //Scroll 
    public ScrollListManager scrollList;

    private SORT_TYPE sort_type;
    

    void Start() {
        partyInfo = PartyInfoVC.Inst;
        initScrollList();
        updateAllView();
    }

    public void UpdateViewChangeEquipment ()
    {
        selectedItemIdx = -1;
        updateAllView();
    }

    void setData()
    {
        EquipmentList.Clear();

        if (partyInfo.getSelectedEquipType() == EQUIPMENT_TYPE.MAX)
        {
            Debug.Log("Invalid Equipment type 'MAX'");
            return;
        }
        selectedEquipType = partyInfo.getSelectedEquipType();

        for (int i = 0; i < UserDataManager.Inst.GetEquipmentList().Count; i++)
        {
            UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentList()[i];
            DBEquipmentData dbEquipmentData = CSVData.Inst.GetEquipmentData(equipmentData.id);
            UserServantData servantData = partyInfo.ServantList[partyInfo.selected_unit_idx];
            DBServantData dbServantData = CSVData.Inst.GetServantData(servantData.id);

            // 장착 가능 레벨 검사
            bool enable_equip_level = false;
            if (dbEquipmentData.tier == 2)
            {
                if (servantData.level > 10)
                {
                    enable_equip_level = true;
                }
            }
            else if (dbEquipmentData.tier == 3)
            {
                if (servantData.level > 20)
                {
                    enable_equip_level = true;
                }
            }
            else if (dbEquipmentData.tier == 4)
            {
                if (servantData.level > 30)
                {
                    enable_equip_level = true;
                }
            }

            //선택된 파츠, 선택중인 서번트 직업, 장착가능한 레벨, 다른 서번트가 장착한 장비 , 장착할수 있는 상태인 장비
            if (equipmentData.equipmentType == selectedEquipType && equipmentData.isEquiped == false
                && dbEquipmentData.isEquipAble(dbServantData.GetJobFlag) && enable_equip_level
                && equipmentData.equipServantIndex == 0 && equipmentData.state == 1)
            {
                EquipmentList.Add(equipmentData);
            }
        }

    }


    //스크롤 생성
    void initScrollList()
    {
        setData();

        scrollList.Init(this, 20, EquipmentList.Count, getOrder());
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
        int total_list_num = EquipmentList.Count;

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
                        if (EquipmentList[i].value < EquipmentList[j].value)
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
                    for (int j = i + 1; j > total_list_num; j++)
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
            case SORT_TYPE.LEVEL_OR_TIER:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {   
                        if (CSVData.Inst.GetEquipmentData(EquipmentList[i].id).tier < CSVData.Inst.GetEquipmentData(EquipmentList[j].id).tier)
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
                        if (EquipmentList[i].upgrade < EquipmentList[j].upgrade)
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
            Debug.Log("Invalid Servant Index : " + partyInfo.selected_unit_idx);
        }

        UserEquipmentData currentEquipmentData = null;

        if (partyInfo.getSelectedEquipType() == EQUIPMENT_TYPE.MAX)
        {
            Debug.Log("Invalid Equipment type 'MAX'");
            return;
        }

        int current_item_idx = servantData.equipmentDic[partyInfo.getSelectedEquipType()];

        if (current_item_idx > 0)
        {
            FrameCurrentItemInfo.SetActive(true);
            FrameCurrentItemNone.SetActive(false);
            currentEquipmentData = UserDataManager.Inst.GetEquipmentInfo(current_item_idx);

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

            Color temp_color = imageCurrentJob[0].color;
            for (int i = 0; i < 5; i++)
            {
                imageCurrentJob[i].color = new Color(temp_color.r, temp_color.g, temp_color.b, 0.2f);
            }

            if (dBEquipment.jobLimit == SERVANT_JOB_FLAG.All)
            {
                for (int i = 0; i < 5; i++)
                {
                    imageCurrentJob[i].color = new Color(temp_color.r, temp_color.g, temp_color.b, 1f);
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    SERVANT_JOB_FLAG checkJob = (SERVANT_JOB_FLAG)Math.Pow(2, (double)(i + 1));
                    if (dBEquipment.isEquipAble(checkJob) == true)
                    {
                        imageCurrentJob[i].color = new Color(temp_color.r, temp_color.g, temp_color.b, 1f);
                    }
                }
            }

            //초기화
            for (int i = 0; i < 10; i++)
            {
                current_stat[i] = 0;
            }

            imageCurrentStat[0].sprite = CSVData.Inst.GetSpriteOptionType(currentEquipmentData.optionType);
            textCurrentStat[0].text = string.Format("{0}", currentEquipmentData.value);
            current_stat[(int)currentEquipmentData.optionType] = currentEquipmentData.value;

            //장착된 아이템이 있을땐 Clear버튼 활성화
            buttonClear.interactable = true;
        }
        else
        {
            Debug.Log("current item none");
            FrameCurrentItemInfo.SetActive(false);
            FrameCurrentItemNone.SetActive(true);

            //장착된 아이템이 없을땐 Clear버튼 비활성화
            buttonClear.interactable = false;
        }

        updateChangeItemInfo(selectedItemIdx);

        SetChangeAllValue();
    }

    public void updateChangeItemInfo(int selectedChangeItemIdx)
    {

        if (selectedChangeItemIdx >= 0)
        {
            selectedItemIdx = selectedChangeItemIdx;
            int change_item_idx = EquipmentList[selectedItemIdx].index;

            FrameChangeItemInfo.SetActive(true);
            FrameChangeItemNone.SetActive(false);
            UserEquipmentData changeEquipmentData = UserDataManager.Inst.GetEquipmentInfo(change_item_idx);

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

            Color temp_color = imageChangeJob[0].color;
            for (int i = 0; i < 5; i++)
            {
                imageChangeJob[i].color = new Color(temp_color.r, temp_color.g, temp_color.b, 0.2f);
            }
            if (dBChangeEquipment.jobLimit == SERVANT_JOB_FLAG.All)
            {
                for (int i = 0; i < 5; i++)
                {
                    imageChangeJob[i].color = new Color(temp_color.r, temp_color.g, temp_color.b, 1f);
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    SERVANT_JOB_FLAG checkJob = (SERVANT_JOB_FLAG)Math.Pow(2, (double)(i + 1));
                    if (dBChangeEquipment.isEquipAble(checkJob) == true)
                    {
                        imageChangeJob[i].color = new Color(temp_color.r, temp_color.g, temp_color.b, 1f);
                    }
                }
            }

            //초기화
            for (int i = 0; i < 10; i++)
            {
                change_stat[i] = 0;
            }

            imageChangeStat[0].sprite = CSVData.Inst.GetSpriteOptionType(changeEquipmentData.optionType);
            textChangeStat[0].text = string.Format("{0}", changeEquipmentData.value);
            change_stat[(int)changeEquipmentData.optionType] = changeEquipmentData.value;

            buttonChange.interactable = true;
        }
        else
        {
            Debug.Log("change item nono");
            FrameChangeItemInfo.SetActive(false);
            FrameChangeItemNone.SetActive(true);

            buttonChange.interactable = false;
        }

        SetChangeAllValue();
    }

    public void SetChangeAllValue()
    {
        UserServantData servantData = partyInfo.ServantList[partyInfo.selected_unit_idx];
        if(servantData == null)
        {
            Debug.Log("Invalid Select Index : " + partyInfo.ServantList[partyInfo.selected_unit_idx]);
            return;
        }

        DBServantData dBServantData = CSVData.Inst.GetServantData(servantData.id);
        if (dBServantData == null)
        {
            Debug.Log("Invalid Servant ID : " + servantData.id);
            return;
        }

        changeStatus = new Status();
        changeStatus.basicStr = change_stat[(int)EQUIPMENT_OPTION_TYPE.STR];
        changeStatus.basicDex = change_stat[(int)EQUIPMENT_OPTION_TYPE.DEX];
        changeStatus.basicInt = change_stat[(int)EQUIPMENT_OPTION_TYPE.INT];

        textStr.text = string.Format("{0}", servantData.status.basicStr + SetChangeValue(textStrChange, EQUIPMENT_OPTION_TYPE.STR));
        textDex.text = string.Format("{0}", servantData.status.basicDex + SetChangeValue(textDexChange, EQUIPMENT_OPTION_TYPE.DEX));
        textInt.text = string.Format("{0}", servantData.status.basicInt + SetChangeValue(textIntChange, EQUIPMENT_OPTION_TYPE.INT));

        textHp.text = string.Format("{0}", servantData.maxHP + SetChangeValue(textHpChange, EQUIPMENT_OPTION_TYPE.HP));
        textSpeed.text = string.Format("{0}", dBServantData.speed);

        textPAtk.text = string.Format("{0}", servantData.atk + SetChangeValue(textPAtkChange, EQUIPMENT_OPTION_TYPE.ATK));
        textPDef.text = string.Format("{0}", servantData.def + SetChangeValue(textPDefChange, EQUIPMENT_OPTION_TYPE.DEF));

        textMAtk.text = string.Format("{0}", servantData.mAtk + SetChangeValue(textMAtkChange, EQUIPMENT_OPTION_TYPE.MATK));
        textMDef.text = string.Format("{0}", servantData.mDef + SetChangeValue(textMDefChange, EQUIPMENT_OPTION_TYPE.MDEF));

        textPCri.text = string.Format("{0}", dBServantData.criDmg);
        textPCriPer.text = string.Format("{0}", dBServantData.criPer);

        textMCri.text = string.Format("{0}", dBServantData.mcriDmg);
        textMCriPer.text = string.Format("{0}", dBServantData.mcriPer);
    }

    public int SetChangeValue(Text textChangeValue, EQUIPMENT_OPTION_TYPE type)
    {
        int addValue = 0;
        if (type == EQUIPMENT_OPTION_TYPE.HP)
        {
            addValue = Calculator.GetMaxHp(changeStatus);
        }
        else if (type == EQUIPMENT_OPTION_TYPE.ATK)
        {
            addValue = Calculator.GetAttack(changeStatus);
        }
        else if (type == EQUIPMENT_OPTION_TYPE.MATK)
        {
            addValue = Calculator.GetMagicAttack(changeStatus);
        }
        else if (type == EQUIPMENT_OPTION_TYPE.DEF)
        {
            addValue = Calculator.GetDefence(changeStatus);
        }
        else if (type == EQUIPMENT_OPTION_TYPE.MDEF)
        {
            addValue = Calculator.GetMagicDefence(changeStatus);
        }

        int change_value = change_stat[(int)type] - current_stat[(int)type] + addValue;

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


    public void OnClickClear()
    {
        int servantIndex = partyInfo.ServantList[partyInfo.selected_unit_idx].index;

#if UNITY_EDITOR
        Cheat.Inst.RequestUnequipServantCheat(servantIndex, selectedEquipType);
#else
        PacketManager.Inst.RequestUnequipServant(servantIndex, selectedEquipType);
#endif
    }

    public void OnClickChange()
    {
        // 선택 서번트 인덱스
        int servantIndex = partyInfo.ServantList[partyInfo.selected_unit_idx].index;

        int equipmentIndex = EquipmentList[selectedItemIdx].index;

        // 서번트 인덱스 검사
        UserServantData servantData = UserDataManager.Inst.GetServantInfo(servantIndex);
        if (servantData == null)
        {
            Debug.LogError("Invalid Servant Index : " + servantIndex);
            TopUIManager.Inst.ShowSimpleErrorPopup("Invalid Servant Index");
            return;
        }

        // 장비 타입 검사
        if (servantData.equipmentDic.ContainsKey(selectedEquipType) == false)
        {
            Debug.LogError("Invalid Servant Data");
            TopUIManager.Inst.ShowSimpleErrorPopup("Invalid Servant Data");
            return;
        }

        // 서번트 동일 장비 검사
        if (servantData.equipmentDic[selectedEquipType] == equipmentIndex)
        {
            Debug.Log("Already Equiped");
            TopUIManager.Inst.ShowSimpleErrorPopup("Already Equiped");
            return;
        }

        // 장비 인덱스 검사
        UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(equipmentIndex);
        if (equipmentData == null)
        {
            Debug.LogError("Invalid Equipment Index : " + equipmentIndex);
            TopUIManager.Inst.ShowSimpleErrorPopup("Invalid Equipment");
            return;
        }

        // 장비 인덱스에 대한 타입 검사
        if (equipmentData.equipmentType != selectedEquipType)
        {
            Debug.Log("Invalid Equipment Type : " + selectedEquipType.ToString() + ", ");
            TopUIManager.Inst.ShowSimpleErrorPopup("Invalid Equipment Type");
            return;
        }

        // 장착중인 장비인지 검사
        if (equipmentData.isEquiped == true)
        {
            Debug.Log("Already ServantEquiped : " + equipmentData.equipServantIndex);
            TopUIManager.Inst.ShowSimpleErrorPopup("Already Servant Equiped");
            return;
        }

        // DB 장비 ID 검사
        DBEquipmentData dbEquipmentData = CSVData.Inst.GetEquipmentData(equipmentData.id);
        if (dbEquipmentData == null)
        {
            Debug.Log("Invalid Equipment Data ID : " + equipmentData.id);
            return;
        }

        // DB 서번트 ID 검사
        DBServantData dbServantData = CSVData.Inst.GetServantData(servantData.id);
        if (dbServantData == null)
        {
            Debug.Log("Invalid Servant Data ID : " + servantData.id);
            return;
        }

        // 장착 가능 직업 검사
        if (dbEquipmentData.isEquipAble(dbServantData.GetJobFlag) == false)
        {
            Debug.Log("Invalid Servant Equipable Job : " + dbServantData.GetJobFlag + ", Need Job : " + dbEquipmentData.jobLimit);
            TopUIManager.Inst.ShowSimpleErrorPopup("Invalid Servant Equipable Job");
            return;
        }

        // 장착 가능 레벨 검사
        if (dbEquipmentData.tier == 2)
        {
            if (servantData.level <= 10)
            {
                Debug.Log("Invalid Servant Equipable Level : " + servantData.level + ", Need Level : 11");
                TopUIManager.Inst.ShowSimpleErrorPopup("Invalid Servant Equipable Level, Need Level : 11");
                return;
            }
        }
        else if (dbEquipmentData.tier == 3)
        {
            if (servantData.level <= 20)
            {
                Debug.Log("Invalid Servant Equipable Level : " + servantData.level + ", Need Level : 21");
                TopUIManager.Inst.ShowSimpleErrorPopup("Invalid Servant Equipable Level, Need Level : 21");
                return;
            }
        }
        else if (dbEquipmentData.tier == 4)
        {
            if (servantData.level <= 30)
            {
                Debug.Log("Invalid Servant Equipable Level : " + servantData.level + ", Need Level : 31");
                TopUIManager.Inst.ShowSimpleErrorPopup("Invalid Servant Equipable Level, Need Level : 31");
                return;
            }
        }

#if UNITY_EDITOR
        Cheat.Inst.RequestEquipServantCheat(servantIndex, selectedEquipType, equipmentIndex);
#else
        PacketManager.Inst.RequestEquipServant(servantIndex, selectedEquipType, equipmentIndex);        
#endif

    }

    public void OnClickClose()
    {
        partyInfo.frameScroll.SetActive(true);
        Destroy(this.gameObject);
    }
}
