using UnityEngine;

public class LoginUI : MonoBehaviour {

    public GameObject bannerImage;
    public GameObject loginPopup;
    public GameObject languageButton;
    public GameObject nodeButton;

    public bool buttonClick;

	public void Awake () {
        bannerImage.SetActivateWithAnimation(true);
        loginPopup.SetActivateWithAnimation(true);
        languageButton.SetActivateWithAnimation(true);
        nodeButton.SetActivateWithAnimation(true);

        buttonClick = false;
    }

    public void OnClickButton()
    {
        if(buttonClick == false)
        {
            buttonClick = true;

            // 로딩씬에 추가가 되야할듯 임시로 로그인에 넣어둠
            CSVData.Inst.SetCSVData();

#if UNITY_EDITOR
            {
                Cheat.Inst.RequestLoginCheat();
            }
#else
            {
                PacketManager.Inst.RequestLoginWithScatter();
            }
#endif
        }
    }
}
