using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour {
    
    public Text LoginText;
    private int state = 0;

    public GameObject bannerImage;
    public GameObject loginPopup;

	public void Awake () {
        LoginText.text = "Scatter Login";
        state = 0;
        bannerImage.SetActivateWithAnimation(true);
        loginPopup.SetActivateWithAnimation(true);
    }

    public void OnClickScatterLogin()
    {
        LoginText.text = "Scatter Login";
        state = 0;
    }

    public void OnClickEOSTockLogin()
    {
        LoginText.text = "EOSTock Login";
        state = 1;
    }

    public void OnClickETCLogin()
    {
        LoginText.text = "ETC Login";
        state = 2;
    }

    public void OnClickLogin()
    {
        switch(state)
        {
            case 0:
            case 1:
            case 2:
                UTUMSProvider.Instance.RequestLoginWithScatter();
                break;

            default:
                break;
        }
    }
}
