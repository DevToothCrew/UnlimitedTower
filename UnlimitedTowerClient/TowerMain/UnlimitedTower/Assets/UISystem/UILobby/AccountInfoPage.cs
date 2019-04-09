using UnityEngine;
using UnityEngine.UI;

public class AccountInfoPage : MonoBehaviour {

    public Image GradeImage;
    public Image ServantImage;

    public Text AccountText;
    public Text RankLevelText;
    public Text RankExpText;
    public Image ExpSlide;

    public Text EOSText;
    public Text UTGText;

	void Awake() {

        if (UserDataManager.Inst.GetMainCharInfo() != null)
        {
            MainCharInfo charInfo = UserDataManager.Inst.GetMainCharInfo();
            GradeImage.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)charInfo.grade);
            ServantImage.sprite = CSVData.Inst.GetServantIcon(charInfo.mainCharID);
        }

        UserInfo userInfo = UserDataManager.Inst.GetUserInfo();
        if (userInfo != null)
        {
            AccountText.text = "Rank." + userInfo.level + " " + userInfo.userName;
            RankLevelText.text = userInfo.level.ToString();

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
                RankExpText.text = userInfo.userEXP + " / " + dbExpData.rankExp;
                ExpSlide.fillAmount = (exExp - userInfo.userEXP) / (float)(exExp - dbExpData.rankExp);
            }
        }
    }

    public void ExitButton()
    {
        LobbyManager.Inst.OnClickBackButton();
    }
}
