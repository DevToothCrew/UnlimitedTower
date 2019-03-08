using UnityEngine;
using UnityEngine.UI;

public class EOStockButton : MonoBehaviour {

    private string imagePath = "UI/LoginUI/";
    public Image objectImage;

    public void EOStockButtonOn()
    {
        objectImage.sprite = Resources.Load<Sprite>(imagePath + "3_EOStockOnButton") as Sprite;
    }

    public void EOStockButtonOff()
    {
        objectImage.sprite = Resources.Load<Sprite>(imagePath + "6_EOStockOffButton") as Sprite;
    }

    public void EOStockButtonClick()
    {
        Debug.Log("EOStock Click");
        UTUMSProvider.Instance.RequestLoginWithScatter();
    }
}
