using UnityEngine.UI;
using UnityEngine;

public class LeftInfoPopup : MonoSingleton<LeftInfoPopup>
{
    public Text level;
    public Text account;
    public Text eos;
    public Text gameMoney;

    private void Awake()
    {
        SetLeftInfoUserInfoUpdate(UserDataManager.Inst.GetUserInfo());
    }

    public void SetLeftInfoUserInfoUpdate(UserInfo userInfo)
    {
        if (userInfo != null)
        {
            int userExp = userInfo.userHero.exp;
            level.text = "Lv." + Calculator.GetLevelForExp(userExp).ToString();
            account.text = userInfo.userName;

            eos.text = userInfo.userEOS.ToString();
            gameMoney.text = userInfo.userMoney.ToString();
        }
    }

    public void SetLeftInfoUserLevel(int getExp)
    {
        level.text = "Lv." + Calculator.GetLevelForExp(getExp).ToString();
    }

    public void SetLeftInfoUserEOS(int getEOS)
    {
        eos.text = getEOS.ToString();
    }

    public void SetLeftInfoUserGameMoney(int getGameMoney)
    {
        gameMoney.text = getGameMoney.ToString();
    }
}