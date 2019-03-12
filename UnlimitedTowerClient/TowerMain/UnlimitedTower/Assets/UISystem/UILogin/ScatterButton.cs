using UnityEngine;
using UnityEngine.UI;

public class ScatterButton : MonoBehaviour
{
    private readonly string imagePath = "UI/LoginUI/";
    public Image objectImage;

    private Sprite scatterOn;
    private Sprite scatterOff;

    public void Awake()
    {
        scatterOn = Resources.Load<Sprite>(imagePath + "2_ScatterOnButton") as Sprite;
        scatterOff = Resources.Load<Sprite>(imagePath + "5_ScatterOffButton") as Sprite;
    }

    public void ScatterButtonOn()
    {
        objectImage.sprite = scatterOn;
    }

    public void ScatterButtonOff()
    {
        objectImage.sprite = scatterOff;
    }

    public void ScatterButtonClick()
    {
        Debug.Log("Scatter Click");
        UTUMSProvider.Instance.RequestLoginWithScatter();
        objectImage.sprite = scatterOff;
    }
}
