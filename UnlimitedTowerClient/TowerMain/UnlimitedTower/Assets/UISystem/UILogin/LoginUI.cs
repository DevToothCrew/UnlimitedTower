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

#if UNITY_EDITOR
            Cheat.Inst.SetLoginCheat();
#else
            PacketManager.Inst.RequestLoginWithScatter();
#endif

        }
    }
}
