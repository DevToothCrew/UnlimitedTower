using UnityEngine;
using UnityEngine.UI;

public class ETCButton : MonoBehaviour
{
    private readonly string imagePath = "UI/LoginUI/";
    public Image objectImage;

    private Sprite etcOn;
    private Sprite etcOff;

    public void Awake()
    {
        etcOn = Resources.Load<Sprite>(imagePath + "4_EtcOnButton") as Sprite;
        etcOff = Resources.Load<Sprite>(imagePath + "7_EtcOffButton") as Sprite;
    }

    public void ETCButtonOn()
    {
        objectImage.sprite = etcOn;
    }

    public void ETCButtonOff()
    {
        objectImage.sprite = etcOff;
    }

    public void ETCButtonClick()
    {
        Debug.Log("ETC Click");
        UTUMSProvider.Instance.RequestLoginWithScatter();
        objectImage.sprite = etcOff;
    }
}
