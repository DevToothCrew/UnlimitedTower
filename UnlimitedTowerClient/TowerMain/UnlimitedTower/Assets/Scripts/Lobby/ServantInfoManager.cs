using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServantInfoManager : MonoSingleton<ServantInfoManager> {
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
    public Text textMCri;
    public Text textMCriPer;

    public Text textSkillText;
    public Text textActiveText;
    public Text textPassiveText;
    public Image imageActive;
    public Image imagePassive;

    private int servant_idx;


    public void updateServantInfo(int s_idx)
    {
        servant_idx = s_idx;

        UserServantData s_data = UserDataManager.Inst.GetServantInfo(servant_idx);
        DBServantData db_s_data = CSVData.Inst.GetServantData(s_data.id);

        //textGradeText.text;
        textGrade.text = string.Format("{0}", s_data.gradeType);
        //textJobText.text;
        imagejob.sprite = CSVData.Inst.GetSpriteServantJob(s_data.jobType);
        //textStatsText.text;

        textStr.text = string.Format("{0}", s_data.status.basicStr);
        textDex.text = string.Format("{0}", s_data.status.basicDex);
        textInt.text = string.Format("{0}", s_data.status.basicInt);

        textHP.text = string.Format("{0}", s_data.maxHP);
        textSpeed.text = string.Format("{0}", db_s_data.speed);
        textPAtk.text = string.Format("{0}", s_data.atk);
        textPDef.text = string.Format("{0}", s_data.def);
        textMAtk.text = string.Format("{0}", s_data.mAtk);
        textMDef.text = string.Format("{0}", s_data.mDef);
        textPCri.text = string.Format("{0}", db_s_data.criDmg);
        textPCriPer.text = string.Format("{0}", db_s_data.criPer);
        textMCri.text = string.Format("{0}", db_s_data.mcriDmg);
        textMCriPer.text = string.Format("{0}", db_s_data.mcriPer);

        //imageActive.sprite = CSVData.Inst.GetSkillActiveData();
        //imagePassive.sprite = CSVData.Inst.GetSkillPassiveData();

    }
}
