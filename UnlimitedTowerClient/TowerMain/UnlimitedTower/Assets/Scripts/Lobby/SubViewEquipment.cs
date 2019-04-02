using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubViewEquipment : MonoSingleton<SubViewEquipment>
{
    //Current Item UI
    public Image imageCurrentItem;
    public Text textCurrentItemName;
    public Text textCurrentGradeText;
    public Text textCurrentGrade;
    public Text textCurrentUpgradeText;
    public Text textCurrentUpgrade;
    public Text textCurrentTierText;
    public Text textCurrentTier;
    public Text textCurrentJobText;
    public Image[] imageCurrentJos = new Image[3];
    public Image[] imageCurrentStat = new Image[3];
    public Text[] textCurrentStat = new Text[3];


    //Change Item UI
    public Image imageChangeItem;
    public Text textChangeItemName;
    public Text textChangeGradeText;
    public Text textChangeGrade;
    public Text textChangeUpgradeText;
    public Text textChangeUpgrade;
    public Text textChangeTierText;
    public Text textChangeTier;
    public Text textChangeJobText;
    public Image[] imageChangeJos = new Image[3];
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

    private int selectedChangeItemIdx;
    private PartyInfoVC partyInfo;

    void Start () {
        partyInfo = PartyInfoVC.Inst;
        updateView();
    }
	
	
	void updateView () {
        //Current Item
        UserServantData servantData = partyInfo.ServantList[partyInfo.selected_unit_idx];
        if (servantData == null)
        {
            Debug.Log("Invalid Servant Inddex : " + partyInfo.selected_unit_idx);
        }

        //DBServantData dBServantData = CSVData.Inst.GetServantData(servantData.id);
        //if (dBServantData == null)
        //{
        //    Debug.Log("Invalid Servant ID : " + servantData.id);
        //}

        int item_idx = 0;

        //imageCurrentItem;
        //textCurrentItemName;
        //textCurrentGradeText;
        //textCurrentGrade;
        //textCurrentUpgradeText;
        //textCurrentUpgrade;
        //textCurrentTierText;
        //textCurrentTier;
        //textCurrentJobText;
        //imageCurrentJos = new Image[3];
        //imageCurrentStat = new Image[3];
        //textCurrentStat = new Text[3];


        //textLevel.text = string.Format("{0}", servantData.level);
        //textCharacterName.text = string.Format("{0}", dBServantData.name);

        //textStr.text = string.Format("{0}", servantData.status.basicStr);
        //textDex.text = string.Format("{0}", servantData.status.basicDex);
        //textInt.text = string.Format("{0}", servantData.status.basicInt);

        //textPAtk.text = string.Format("{0}", servantData.atk);
        //textPDef.text = string.Format("{0}", servantData.def);

        //textMAtk.text = string.Format("{0}", servantData.mAtk);
        //textMDef.text = string.Format("{0}", servantData.mDef);

        //textPCri.text = string.Format("{0}", dBServantData.criDmg);
        //textPCriPer.text = string.Format("{0}", dBServantData.criPer);

        //textMCri.text = string.Format("{0}", dBServantData.mcriDmg);
        //textMCriPer.text = string.Format("{0}", dBServantData.mcriPer);

        //textSpeed.text = string.Format("{0}", dBServantData.speed);

        //Change Item

        //Stats
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

    }

    public void OnClickClose()
    {

    }
}
