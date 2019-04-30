using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInfoManager : MonoBehaviour {
    public Text textUnitName;
    public Text textLevel;
    public Image imageExp;
    public Text textExp;

    public Button buttonUpgrade;
    public Button buttonBurn;
    public Button buttonEtc;

    public Text textGradeText;
    public Text textGrade;
    public Text textUpgradeText;
    public Text textUpgrade;
    public Text textTribeText;
    public Image imageTribe;
    public Text textTypeText;
    public Image imageType;

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
    public Text textMCri;
    public Text textMCriPer;

    private int unit_idx;


    public void updateMonsterInfo(UserMonsterData m_data)
    {
        //unit_idx = u_idx;

        UserMonsterData unit_data = m_data;// UserDataManager.Inst.GetMonsterInfo(unit_idx);
        if (unit_data == null)
        {
            DebugLog.Log(false, "Invalid Monster data By MonsterInfoManager");
        }
        DBMonsterData db_unit_data = CSVData.Inst.GetMonsterData(unit_data.id);
        if (db_unit_data == null)
        {
            DebugLog.Log(false, "Invalid DB_Monster data By MonsterInfoManager");
        }

        textUnitName.text = db_unit_data.name;
        textLevel.text = string.Format("{0}", unit_data.level);


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
        //textUpgradeText.text;
        textUpgrade.text = string.Format("+{0}", unit_data.upgrade);
        //textClassText.text;
        imageTribe.sprite = CSVData.Inst.GetSpriteTribeType(db_unit_data.tribeType);
        //textTypeText.text;
        imageType.sprite = CSVData.Inst.GetSpriteElementType(db_unit_data.elementType);

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
        textPCri.text = string.Format("{0}", db_unit_data.criDmg);
        textPCriPer.text = string.Format("{0}", db_unit_data.criPer);
        //마법 크리관련 표시X
        //textMCri.text = string.Format("{0}", db_unit_data.mcriDmg);
        //textMCriPer.text = string.Format("{0}", db_unit_data.mcriPer);
    }

    //업그레이드 버튼
    public void OnClickUpgrade()
    {
        PartyInfoVC.Inst.ShowUpgrade();
    }

    //분해 버튼
    public void OnClickDeconstruction()
    {
        PartyInfoVC.Inst.ShowDeconstruction();
    }
}
