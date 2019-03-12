using UnityEngine;
using UnityEngine.UI;

public class EOStockButton : MonoBehaviour {

    private string imagePath = "UI/LoginUI/";
    public Image objectImage;

    private Sprite eostockOn;
    private Sprite eostockOff;

    public void Awake()
    {
        eostockOn = Resources.Load<Sprite>(imagePath + "3_EOStockOnButton") as Sprite;
        eostockOff = Resources.Load<Sprite>(imagePath + "6_EOStockOffButton") as Sprite;
    }

    public void EOStockButtonOn()
    {
        objectImage.sprite = eostockOn;
    }

    public void EOStockButtonOff()
    {
        objectImage.sprite = eostockOff;
    }

    public void EOStockButtonClick()
    {
        Debug.Log("EOStock Click");
        UTUMSProvider.Instance.RequestLoginWithScatter();
        objectImage.sprite = eostockOff;
    }
}
