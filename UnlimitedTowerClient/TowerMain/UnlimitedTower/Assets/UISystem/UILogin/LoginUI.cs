using UnityEngine;

public class LoginUI : MonoBehaviour {

    public GameObject bannerImage;
    public GameObject loginPopup;

	public void Awake () {
        bannerImage.SetActivateWithAnimation(true);
        loginPopup.SetActivateWithAnimation(true);
    }
}
