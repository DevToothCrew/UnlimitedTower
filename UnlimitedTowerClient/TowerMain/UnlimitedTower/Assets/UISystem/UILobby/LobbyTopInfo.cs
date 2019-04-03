using UnityEngine;
using UnityEngine.UI;

public class LobbyTopInfo : MonoSingleton<LobbyTopInfo> {

    public Text accountName;
    public Text EOSCount;
    public Text UTGCount;
    public Image MainCharImage;
    public Image GradeCharImage;

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
