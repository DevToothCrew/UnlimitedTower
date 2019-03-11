using UnityEngine;
using UnityEngine.UI;

public class ScatterButton : MonoBehaviour {

    private string imagePath = "UI/LoginUI/";
    public Image objectImage;

    public void ScatterButtonOn()
    {
        objectImage.sprite = Resources.Load<Sprite>(imagePath + "2_ScatterOnButton") as Sprite;
    }

    public void ScatterButtonOff()
    {
        objectImage.sprite = Resources.Load<Sprite>(imagePath + "5_ScatterOffButton") as Sprite;
    }

    public void ScatterButtonClick()
    {
        Debug.Log("Scatter Click");
        UTUMSProvider.Instance.RequestLoginWithScatter();
        objectImage.sprite = Resources.Load<Sprite>(imagePath + "5_ScatterOffButton") as Sprite;
    }
}
