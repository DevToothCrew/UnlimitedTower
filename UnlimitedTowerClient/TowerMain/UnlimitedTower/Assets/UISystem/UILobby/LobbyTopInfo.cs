using UnityEngine;
using UnityEngine.UI;

public class LobbyTopInfo : MonoSingleton<LobbyTopInfo> {

    public Text EOSCount;
    public Text UTGCount;
    public Text UserName;
    public Text UserLevelText;
    public Image MainCharImage;
    public Image GradeCharImage;
    public Image ExpSlide;

    void Awake ()
    {
        UpdateTopInfo();
    }

    public void UpdateTopInfo()
    {
        UserInfo userInfo = UserDataManager.Inst.GetUserInfo();
        if (userInfo != null)
        {
            UserName.text = "Rank." + userInfo.level + " " + userInfo.userName;
            UserLevelText.text = userInfo.level.ToString();

            DBExpData dbExpData = CSVData.Inst.GetExpData(userInfo.level);
            if (dbExpData != null)
            {
                int exExp = 0;
                if (userInfo.level - 1 > 0)
                {
                    DBExpData exDBExpData = CSVData.Inst.GetExpData(userInfo.level - 1);
                    if (exDBExpData == null)
                    {
                        Debug.Log("Invalid Level Data");
                    }
                    else
                    {
                        exExp = exDBExpData.rankExp;
                    }
                }

                ExpSlide.fillAmount = (exExp - userInfo.userEXP) / (float)(exExp - dbExpData.rankExp);
            }
        }
        if (UserDataManager.Inst.GetMainCharInfo() != null)
        {
            DBServantData servantData = CSVData.Inst.GetServantData(UserDataManager.Inst.GetMainCharInfo().mainCharID);
            if (servantData != null)
            {
                MainCharImage.sprite = servantData.servantIcon;
                GradeCharImage.sprite = CSVData.Inst.GetSpriteGrade(servantData.grade);
            }
        }
    }
}
