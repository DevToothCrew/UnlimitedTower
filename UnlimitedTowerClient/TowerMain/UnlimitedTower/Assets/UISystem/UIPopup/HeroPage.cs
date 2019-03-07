using UnityEngine.UI;
using UnityEngine;

public class HeroPage : MonoBehaviour {

    public GameObject[] page = new GameObject[4];
    public HERO_PAGE_STATE state;

    public Text userHeroName;
    public Text userHeroLevel;
    public Text userHeroJob;

    public Text expText;

    public Text strText;
    public Text dexText;
    public Text intText;

    public Text atkText;
    public Text defText;
    public Text criPerText;
    public Text criDmgText;
    public Text spdText;

    private void OnEnable()
    {
        state = HERO_PAGE_STATE.INFO;

        UserInfo userInfo = UserDataManager.Inst.GetUserInfo();
        if (userInfo != null)
        {
            userHeroName.text = userInfo.userHero.name + "NoName";
            userHeroLevel.text = userInfo.userHero.level.ToString();
            userHeroJob.text = ((SERVANT_JOB)userInfo.userHero.jobNum).ToString();
        }

        OnClickButton((int)state);
    }

    public void OnClickButton(int num)
    {
        for (int i = 0; i < (int)HERO_PAGE_STATE.MAX; i++)
        {
            if (i == num)
            {
                page[i].SetActive(true);
            }
            else
            {
                page[i].SetActive(false);
            }
        }

        switch((HERO_PAGE_STATE)num)
        {
            case HERO_PAGE_STATE.INFO:
                {
                    UserInfo userInfo = UserDataManager.Inst.GetUserInfo();
                    if (userInfo != null)
                    {
                        // TODO : 추후 Max Exp Setting 필요
                        expText.text = userInfo.userHero.exp + " / 9999";

                        strText.text = userInfo.userHero.status.basicStr + " + " + userInfo.userHero.status.plusStr;
                        dexText.text = userInfo.userHero.status.basicDex + " + " + userInfo.userHero.status.plusDex;
                        intText.text = userInfo.userHero.status.basicInt + " + " + userInfo.userHero.status.plusInt;

                        atkText.text = Calculator.GetAttack(STATUS_TYPE.STR, userInfo.userHero.status).ToString();
                        defText.text = Calculator.GetDefence(userInfo.userHero.status).ToString();
                        criPerText.text = Calculator.GetCriticalPer(userInfo.userHero.status).ToString();

                        // TODO : 추후 Critical Damage 필요
                        criDmgText.text = "150%";
                        spdText.text = Calculator.GetSpeed((SERVANT_JOB)userInfo.userHero.jobNum).ToString();
                    }
                }
                break;

            default:
                break;
        }
    }

}

public enum HERO_PAGE_STATE
{
    INFO        = 0,
    EQUIPMENT   = 1,
    CUSTOMIZING = 2,
    CHANGE_JOB  = 3,
    MAX         = 4,
}