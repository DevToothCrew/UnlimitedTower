using UnityEngine;
using UnityEngine.UI;

public class ETCButton : MonoBehaviour
{

    private string imagePath = "UI/LoginUI/";
    public Image objectImage;

    public void ETCButtonOn()
    {
        objectImage.sprite = Resources.Load<Sprite>(imagePath + "4_EtcOnButton") as Sprite;
    }

    public void ETCButtonOff()
    {
        objectImage.sprite = Resources.Load<Sprite>(imagePath + "7_EtcOffButton") as Sprite;
    }

    public void ETCButtonClick()
    {
        Debug.Log("ETC Click");
        UTUMSProvider.Instance.RequestLoginWithScatter();
        objectImage.sprite = Resources.Load<Sprite>(imagePath + "7_EtcOffButton") as Sprite;
    }
}
