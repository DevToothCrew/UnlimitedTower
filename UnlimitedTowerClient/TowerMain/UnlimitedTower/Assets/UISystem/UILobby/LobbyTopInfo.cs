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
        UserInfo userInfo = UserDataManager.Inst.GetUserInfo();
        if (userInfo != null)
        {
            UpdateTopInfo();
            UserName.text = "Rank." + userInfo.level + " " + userInfo.userName;
            UserLevelText.text = userInfo.level.ToString();

            DBExpData dbExpData = CSVData.Inst.GetExpData(userInfo.level);
            if(dbExpData != null)
            {
                ExpSlide.fillAmount = userInfo.userEXP / (float)dbExpData.rankExp;
            }
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
