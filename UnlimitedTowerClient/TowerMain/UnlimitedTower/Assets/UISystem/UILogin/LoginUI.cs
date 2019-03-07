using UnityEngine;

public class LoginUI : MonoBehaviour {

    public GameObject bannerImage;
    public GameObject loginPopup;

	public void Awake () {
        bannerImage.SetActivateWithAnimation(true);
        loginPopup.SetActivateWithAnimation(true);
    }

    public void OnClickScatterLogin()
    {
        UTUMSProvider.Instance.RequestLoginWithScatter();
    }

    public void OnClickEOSTockLogin()
    {
        UTUMSProvider.Instance.RequestLoginWithScatter();
    }

    public void OnClickETCLogin()
    {
        UTUMSProvider.Instance.RequestLoginWithScatter();
    }
}
