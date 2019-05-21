using UnityEngine;
using UnityEngine.UI;

public class ShopObjectInfo : MonoBehaviour {

    public int index;
    public SHOP_TYPE shopType;
    public int id;
    public ShopProductInfo shopProductInfo;

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
            DebugLog.Log(false, "Invalid Shop Object ID : " + getInfo.index);
            return;
        }

        index = getInfo.index;
        shopType = getInfo.type;
        id = getInfo.id;

        textCount.text = "X " + shopData.productCount;
        if(shopType == SHOP_TYPE.ETC)
        {
            // 기타등등은 소수점없다
            textCost.text = shopData.priceCount.ToString();
        }
        else
        {
            textCost.text = (shopData.priceCount * 0.0001).ToString("N4");
        }

        if (shopType == SHOP_TYPE.PACKAGE)
        {
            // 패키지면 패키지에서 찾아야한다...
            DBPackageData packageData = CSVData.Inst.GetPackageData(shopData.productID);
            if(packageData == null)
            {
                DebugLog.Log(true, "Error");
                return;
            }
            imageObject.sprite = packageData.resourceIcon;
            textName.text = packageData.name;
        }
        else
        {
            imageObject.sprite = CSVData.Inst.GetItemIcon(shopData.productID);
            textName.text = CSVData.Inst.GetItemData(shopData.productID).name;
        }
        imageCost.sprite = CSVData.Inst.GetItemIcon(shopData.priceID);

        shopProductInfo = getInfo;
    }

    public void OnClickObjectButton()
    {
        ShopInfoPage.Inst.SetShopBuyPopup(shopProductInfo);
    }
}
