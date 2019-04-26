using UnityEngine;
using UnityEngine.UI;

public class ShopObjectInfo : MonoBehaviour {

    private int index;
    private SHOP_TYPE shopType;
    private int id;
    private ShopProductInfo shopProductInfo;

    public Text textName;

    public Image imageObject;
    public Text textCount;

    public Image imageCost;
    public Text textCost;

    public void SetShopObjectInfo(ShopProductInfo getInfo)
    {
        DBShopData shopData = CSVData.Inst.GetShopData(getInfo.index);
        if(shopData == null)
        {
            Debug.Log("Invalid Shop Object ID : " + getInfo.index);
            return;
        }

        index = getInfo.index;
        shopType = getInfo.type;
        id = getInfo.id;

        textCount.text = "X " + shopData.productCount;
        textCost.text = (shopData.priceCount * 0.0001).ToString("N4");
        textName.text = CSVData.Inst.GetItemData(shopData.productID).name;

        imageObject.sprite = CSVData.Inst.GetItemIcon(shopData.productID); ;
        imageCost.sprite = CSVData.Inst.GetItemIcon(shopData.priceID);

        shopProductInfo = getInfo;
    }

    public void OnClickObjectButton()
    {
        ShopInfoPage.Inst.SetShopBuyPopup(shopProductInfo);
    }
}
