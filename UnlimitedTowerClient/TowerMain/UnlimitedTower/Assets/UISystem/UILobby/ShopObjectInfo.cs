using UnityEngine;
using UnityEngine.UI;

public class ShopObjectInfo : MonoBehaviour {

    private int index;
    private SHOP_TYPE shopType;
    private int id;

    public Text textName;

    public Image imageObject;
    public Text textCount;

    public Image imageCost;
    public Text textCost;

    public void SetShopObjectInfo(ShopProductInfo getInfo)
    {
        index = getInfo.index;
        shopType = getInfo.type;
        id = getInfo.id;

        textCount.text = "X " + getInfo.productCount;
        textCost.text = "1,000";

        if(shopType == SHOP_TYPE.UTG)
        {
            imageCost.sprite = CSVData.Inst.GetItemIcon(500001);
        }

        // DB Item 정보에 따라 Sprite와 Count 설정
    }

    public void OnClickObjectButton()
    {
        // 구매 팝업을 띄운다
        Debug.Log(index);
        Debug.Log(shopType.ToString());
        Debug.Log(id);
    }
}
