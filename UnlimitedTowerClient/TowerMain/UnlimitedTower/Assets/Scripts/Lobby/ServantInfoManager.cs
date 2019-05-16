using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServantInfoManager : MonoSingleton<ServantInfoManager> {
    public Text textUnitName;
    public Text textLevel;
    public Image imageExp;
    public Text textExp;

    public Button[] buttonEquip = new Button[6];

    public Button buttonSkill;
    public Button buttonBurn;
    public Button buttonEtc;

    public Text textGradeText;
    public Text textGrade;
    public Text textJobText;
    public Image imagejob;

    public Text textStatsText;

    public Text textStr;
    public Text textDex;
    public Text textInt;

    public Text textHP;
    public Text textSpeed;
    public Text textPAtk;
    public Text textPDef;
    public Text textMAtk;
    public Text textMDef;
    public Text textPCri;
    public Text textPCriPer;

    public Text textSkillText;
    public Text textActiveText;
    public Text textPassiveText;
    public Image imageActive;
    public Image imagePassive;

    public Sprite spriteEmptySlot;

    private int unit_idx;


    public void updateServantInfo(UserServantData s_data)
    {
        //unit_idx = u_idx;
        for (int i = 0; i < buttonEquip.Length; i++)
            buttonEquip[i].gameObject.SetActive(false);
        for (int i = 0; i < 3; i++)
            buttonEquip[i].gameObject.SetActive(true);

        UserServantData unit_data = s_data;// UserDataManager.Inst.GetServantInfo(unit_idx);
        if (unit_data == null)
        {
            DebugLog.Log(false, "Invalid Servant data By ServantInfoManager");
        }
        DBServantData db_unit_data = CSVData.Inst.GetServantData(unit_data.id);
        if (db_unit_data == null)
        {
            DebugLog.Log(false, "Invalid DB_Servant data By ServantInfoManager");
        }

        textUnitName.text = db_unit_data.name;
        textLevel.text = string.Format("{0}", unit_data.level);


        for (EQUIPMENT_TYPE type = EQUIPMENT_TYPE.WEAPON; type < EQUIPMENT_TYPE.MAX; type++)
        {
            if (unit_data.equipmentDic[type] != 0)
            {
                UserEquipmentData equip_info = UserDataManager.Inst.GetEquipmentInfo(unit_data.equipmentDic[type]);
                if (equip_info == null)
                {
                    DebugLog.Log(true, "Invalid Index : " + unit_data.equipmentDic[type]);
                    return;
                }

                buttonEquip[(int)type].image.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)equip_info.grade);
                buttonEquip[(int)type].transform.GetChild(1).GetComponent<Image>().enabled = true;
                buttonEquip[(int)type].transform.GetChild(1).GetComponent<Image>().sprite = CSVData.Inst.GetEquipmentData(equip_info.id).equipmentIcon;
            }
            else
            {
                buttonEquip[(int)type].image.sprite = spriteEmptySlot;
                buttonEquip[(int)type].transform.GetChild(1).GetComponent<Image>().enabled = false;
            }

        }

        DBExpData dbExpData = CSVData.Inst.GetExpData(unit_data.level);
        if (dbExpData == null)
        {
            DebugLog.Log(false, "Invalid Level Data");
        }
        else
        {
            int exExp = 0;
            if (unit_data.level - 1 > 0)
            {
                DBExpData exDBExpData = CSVData.Inst.GetExpData(unit_data.level - 1);
                if (exDBExpData == null)
                {
                    DebugLog.Log(false, "Invalid Level Data");
                }
                else
                {
                    exExp = exDBExpData.charExp;
                }
            }

            textExp.text = unit_data.exp + " / " + dbExpData.charExp;
            imageExp.fillAmount = (exExp - unit_data.exp) / (float)(exExp - dbExpData.charExp);
        }


        //textGradeText.text;
        textGrade.text = string.Format("{0}", unit_data.gradeType);
        textGrade.color = DEFINE.GetGradeColor(unit_data.gradeType);
        //textJobText.text;
        imagejob.sprite = CSVData.Inst.GetSpriteServantJob(unit_data.jobType);
        //textStatsText.text;

        textStr.text = string.Format("{0}", unit_data.status.basicStr);
        textDex.text = string.Format("{0}", unit_data.status.basicDex);
        textInt.text = string.Format("{0}", unit_data.status.basicInt);

        textHP.text = string.Format("{0}", unit_data.maxHP);
        textSpeed.text = string.Format("{0}", db_unit_data.speed);
        textPAtk.text = string.Format("{0}", unit_data.atk);
        textPDef.text = string.Format("{0}", unit_data.def);
        textMAtk.text = string.Format("{0}", unit_data.mAtk);
        textMDef.text = string.Format("{0}", unit_data.mDef);
        textPCri.text = string.Format("{0}", unit_data.criDmg);
        textPCriPer.text = string.Format("{0}", unit_data.criPer);

        if (unit_data.activeSkillList.Count > 0)
        {
            imageActive.gameObject.SetActive(true);
            imageActive.sprite = CSVData.Inst.GetSkillActiveData(unit_data.activeSkillList[0].id).activeIcon;
        }
        else
        {
            imageActive.gameObject.SetActive(false);
        }

        if (unit_data.passiveSkillList.Count > 0)
        {
            imagePassive.gameObject.SetActive(true);
            imagePassive.sprite = CSVData.Inst.GetSkillPassiveData(unit_data.passiveSkillList[0].id).passiveIcon;
        }
        else
        {
            imagePassive.gameObject.SetActive(false);
        }

    }

    //장비 설정 버튼
    public void OnClickEquipment(int btn_tag)
    {
        PartyInfoVC.Inst.ShowEquipmentInfo(btn_tag);
    }

    //분해 버튼
    public void OnClickSkill()
    {
        
    }

    //분해 버튼
    public void OnClickDeconstruction()
    {
        PartyInfoVC.Inst.ShowDeconstruction();
    }
}
