using UnityEngine;
using UnityEngine.UI;

public class LoginPopupUI : MonoBehaviour {

    public Text LoginText;
    private int state = 0;

	void OnEnable () {
        LoginText.text = "Scatter Login";
        state = 0;
	}

    public void ScatterLogin()
    {
        LoginText.text = "Scatter Login";
        state = 0;
    }

    public void EOSTockLogin()
    {
        LoginText.text = "EOSTock Login";
        state = 1;
    }

    public void ETCLogin()
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
