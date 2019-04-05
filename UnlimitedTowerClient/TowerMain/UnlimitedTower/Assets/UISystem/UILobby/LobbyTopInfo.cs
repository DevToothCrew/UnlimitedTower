using UnityEngine;
using UnityEngine.UI;

public class LobbyTopInfo : MonoSingleton<LobbyTopInfo> {

    public Text accountName;
    public Text EOSCount;
    public Text UTGCount;
    public Text UserName;
    public Text UserLevelText;
    public Image MainCharImage;
    public Image GradeCharImage;
    public Image ExpSlide;

    void Awake ()
    {
        if (UserDataManager.Inst.GetUserInfo() != null)
        {
            UpdateTopInfo();
        }
        if(UserDataManager.Inst.GetMainCharInfo() != null)
        {
            DBServantData servantData = CSVData.Inst.GetServantData(UserDataManager.Inst.GetMainCharInfo().mainCharID);
            if (servantData != null)
            {
                MainCharImage.sprite = servantData.servantIcon;
                GradeCharImage.sprite = CSVData.Inst.GetSpriteGrade(servantData.grade);
            }
        }
        UserName.text = "LV." + UserDataManager.Inst.userInfo.level + " " + UserDataManager.Inst.userInfo.userName;
        UserLevelText.text = UserDataManager.Inst.userInfo.level.ToString();
        ExpSlide.fillAmount = UserDataManager.Inst.userInfo.userEXP / (float)CSVData.Inst.DBExpDataDic[UserDataManager.Inst.userInfo.level].rankExp;
    }

    public void OnClickAccountButton()
    {
        CSVData.Inst.InitCSVData();
    }

    public void UpdateTopInfo()
    {
        UserInfo userInfo = UserDataManager.Inst.GetUserInfo();
        if (userInfo == null)
        {
            return;
        }

        accountName.text = "LV." + userInfo.level + " " + userInfo.userName;

        EOSCount.text = (userInfo.userEOS * 0.0001).ToString("N4");
        UTGCount.text = (userInfo.userUTG * 0.0001).ToString("N4");
    }
}
