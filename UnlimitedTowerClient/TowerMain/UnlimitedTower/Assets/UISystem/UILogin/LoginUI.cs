using UnityEngine;

public class LoginUI : MonoBehaviour {

    public GameObject bannerImage;
    public GameObject loginPopup;
    public GameObject languageButton;
    public GameObject nodeButton;

	public void Awake () {
        bannerImage.SetActivateWithAnimation(true);
        loginPopup.SetActivateWithAnimation(true);
        languageButton.SetActivateWithAnimation(true);
        nodeButton.SetActivateWithAnimation(true);
    }

    public void OnClickButton()
    {
        DebugLog.Log(false, "로그인 버튼 클릭");
        // 로딩씬에 추가가 되야할듯 임시로 로그인에 넣어둠
        CSVData.Inst.InitCSVData();
    }
}
