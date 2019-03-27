using UnityEngine;
using UnityEngine.UI;

public class LobbyTopInfo : MonoBehaviour {

    public Text accountName;
    public Text EOSCount;
    public Text UTGCount;

    void Awake ()
    {
        if (UserDataManager.Inst.GetUserInfo() != null)
        {
            SetTopInfo(UserDataManager.Inst.GetUserInfo());
        }
    }

    public void SetTopInfo(UserInfo userInfo)
    {
        accountName.text = "LV." + userInfo.userHero.level + " " + userInfo.userName;
        //heroExp.text = userInfo.userHero.exp.ToString() + " / 128000" + " EXP";

        EOSCount.text = userInfo.userEOS.ToString("N4");
        UTGCount.text = userInfo.userMoney.ToString("N4");
    }

    public void OnClickAccountButton()
    {
        // 로딩씬에 추가가 되야할듯 임시로 로그인에 넣어둠
        CSVData.Inst.SetCSVData();
        // 임시로 치트키 추가
        Cheat.Inst.RequestLoginCheat();
    }
}
