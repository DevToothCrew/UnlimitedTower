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
            MainCharImage.sprite = CSVData.Inst.GetServantData(UserDataManager.Inst.GetMainCharInfo().mainCharID).servantIcon;
            GradeCharImage.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)UserDataManager.Inst.GetMainCharInfo().grade);
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
