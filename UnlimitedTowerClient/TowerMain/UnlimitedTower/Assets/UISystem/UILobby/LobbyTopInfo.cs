using UnityEngine;
using UnityEngine.UI;

public class LobbyTopInfo : MonoBehaviour {

    public Text heroLevel;
    public Text accountName;
    public Text EOSCount;
    public Text UTGCount;

    void Awake ()
    {
        if (UserDataManager.Inst.GetUserInfo().userName != null)
        {
            SetTopInfo(UserDataManager.Inst.GetUserInfo());
        }
    }

    public void SetTopInfo(UserInfo userInfo)
    {
        heroLevel.text = userInfo.userHero.level.ToString();
        accountName.text = userInfo.userName;
        EOSCount.text = userInfo.userEOS.ToString();
        UTGCount.text = userInfo.userMoney.ToString();
    }

    public void OnClickAccountButton()
    {
        // 임시로 치트키 추가
        Cheat.Inst.SetLoginCheat();
    }

    public void OnClickLogoutButton()
    {
        UTUMSProvider.Instance.RequestLogout();
    }
}
