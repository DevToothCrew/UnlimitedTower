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
    public Image CPUImage;
    public Image NETImage;
    public Image RAMImage;
    public GameObject MailCountImage;
    public Text MailCountText;

    private Sprite alertImage;
    private Sprite notAlertImage;

    void Awake()
    {
        alertImage = Resources.Load<Sprite>("UI/AccountInfo/red_button");
        notAlertImage = Resources.Load<Sprite>("UI/AccountInfo/green_button");
        CPUImage.sprite = notAlertImage;
        NETImage.sprite = notAlertImage;
        RAMImage.sprite = notAlertImage;
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
                        DebugLog.Log(false, "Invalid Level Data");
                    }
                    else
                    {
                        exExp = exDBExpData.rankExp;
                    }
                }

                ExpSlide.fillAmount = (exExp - userInfo.userEXP) / (float)(exExp - dbExpData.rankExp);
            }

            EOSCount.text = (userInfo.userEOS * 0.0001).ToString("N4");
            UTGCount.text = (userInfo.userUTG * 0.0001).ToString("N4");
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

        UserLobbyInfo lobbyInfo = UserDataManager.Inst.GetUserLobbyInfo();
        if(lobbyInfo == null)
        {
            return;
        }

        if (lobbyInfo.IsCPUAlert == true)
        {
            CPUImage.sprite = alertImage;
        }
        else
        {
            CPUImage.sprite = notAlertImage;
        }
        if (lobbyInfo.IsNETAlert == true)
        {
            NETImage.sprite = alertImage;
        }
        else
        {
            NETImage.sprite = notAlertImage;
        }
        if (lobbyInfo.IsRAMAlert == true)
        {
            RAMImage.sprite = alertImage;
        }
        else
        {
            RAMImage.sprite = notAlertImage;
        }

        if(lobbyInfo.mailCount > 0)
        {
            MailCountText.text = lobbyInfo.mailCount.ToString();

            MailCountImage.SetActive(true);
        }
        else
        {
            MailCountImage.SetActive(false);
        }
    }
}
