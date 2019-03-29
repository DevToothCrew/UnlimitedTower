using UnityEngine;
using UnityEngine.UI;

public class LobbyTopInfo : MonoBehaviour {

    public Text accountName;
    public Text EOSCount;
    public Text UTGCount;
    public Image MainCharImage;
    public Image GradeCharImage;

    void Awake ()
    {
        if (UserDataManager.Inst.GetUserInfo() != null)
        {
            SetTopInfo(UserDataManager.Inst.GetUserInfo());
        }
        if(UserDataManager.Inst.GetMainCharInfo() != null)
        {
            MainCharImage.sprite = CSVData.Inst.GetServantData(UserDataManager.Inst.GetMainCharInfo().mainCharID).servantIcon;
            GradeCharImage.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)UserDataManager.Inst.GetMainCharInfo().grade);
        }
    }

    public void SetTopInfo(UserInfo userInfo)
    {
        accountName.text = "LV." + userInfo.level + " " + userInfo.userName;

        EOSCount.text = userInfo.userEOS.ToString("N4");
        UTGCount.text = userInfo.userUTG.ToString("N4");
    }

    public void OnClickAccountButton()
    {
        CSVData.Inst.InitCSVData();
    }
}
