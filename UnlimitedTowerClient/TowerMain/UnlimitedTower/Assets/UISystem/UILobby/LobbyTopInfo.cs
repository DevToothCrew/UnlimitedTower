using System.Collections;
using System.Collections.Generic;
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

    private ulong lastEOS = 0;
    private ulong lastUTG = 0;

    void Awake()
    {
        UserInfo userInfo = UserDataManager.Inst.GetUserInfo();
        if (userInfo != null)
        {
            lastEOS = userInfo.userEOS;
            lastUTG = userInfo.userUTG;
        }
            

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



            if (lastEOS != userInfo.userEOS)
            {
                NumberCountingEffect(EOSCount, lastEOS, userInfo.userEOS);
            }
            else
            {
                EOSCount.text = (userInfo.userEOS * 0.0001).ToString("N4");
            }

            if (lastUTG != userInfo.userUTG)
            {
                NumberCountingEffect(UTGCount, lastUTG, userInfo.userUTG);
            }
            else
            {
                UTGCount.text = (userInfo.userUTG * 0.0001).ToString("N4");
            }

            lastEOS = userInfo.userEOS;
            lastUTG = userInfo.userUTG;

            //EOSCount.text = (userInfo.userEOS * 0.0001).ToString("N4");
            //UTGCount.text = (userInfo.userUTG * 0.0001).ToString("N4");
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


    //////////////////////////////////////////
    //////* Text Counting Animation */////////
    //////////////////////////////////////////
    private ulong changeValue;

    public void NumberCountingEffect(Text text, ulong start, ulong finish)
    {
        StopCoroutine("IncreaseCounting");
        StopCoroutine("DicreaseCounting");

        if (start > finish)
        {
            changeValue = (start - finish) / 60;

            if (changeValue == 0)
            {
                changeValue = 1;
            }

            StartCoroutine(DicreaseCounting(text, start, finish));
        }
        else if (start < finish)
        {
            changeValue = (finish - start) / 60;

            if (changeValue == 0)
            {
                changeValue = 1;
            }

            StartCoroutine(IncreaseCounting(text, start, finish));
        }
        else
        {
            //None change
            return;
        }
    }

    IEnumerator IncreaseCounting(Text textNumber, ulong startValue, ulong finishValue)
    {
        ulong changeValue_ = changeValue;
        while (startValue < finishValue)
        {
            startValue += changeValue_;
            if (startValue > finishValue)
            {
                startValue = finishValue;
            }

            textNumber.text = (startValue * 0.0001).ToString("N4");

            yield return new WaitForSeconds(.015f);
        }
    }

    IEnumerator DicreaseCounting(Text textNumber, ulong startValue, ulong finishValue)
    {
        ulong changeValue_ = changeValue;
        while (startValue > finishValue)
        {
            if (startValue >= changeValue_)
            {
                startValue -= changeValue_;
            }
            else
            {
                startValue = 0;
            }
            
            if (startValue < finishValue)
            {
                startValue = finishValue;
            }

            textNumber.text = (startValue * 0.0001).ToString("N4");

            yield return new WaitForSeconds(.015f);
        }
    }
}
