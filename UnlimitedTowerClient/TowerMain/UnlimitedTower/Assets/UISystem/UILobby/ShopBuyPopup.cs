using UnityEngine;
using UnityEngine.UI;

public class ShopBuyPopup : MonoBehaviour {

    private int index;
    private SHOP_TYPE shopType;
    private int id;
    public int buyCount;
    public int priceCount;
    public Text textBuyCount;

    public Text textName;

    public Image imageObject;
    public Text textCount;

    public Image imageCost;
    public Text textCost;

    public bool SetShopBuyPopup(ShopProductInfo getInfo)
    {
        DBShopData shopData = CSVData.Inst.GetShopData(getInfo.index);
        if (shopData == null)
        {
            Debug.Log("Invalid Shop Object ID : " + getInfo.index);
            return false;
        }

        index = getInfo.index;
        shopType = getInfo.type;
        id = getInfo.id;

        textCount.text = "X " + shopData.productCount;
        priceCount = shopData.priceCount;
        textCost.text = (priceCount * 0.0001).ToString("N4");
        textName.text = CSVData.Inst.GetItemData(shopData.productID).name;

        imageObject.sprite = CSVData.Inst.GetItemIcon(shopData.productID); ;
        imageCost.sprite = CSVData.Inst.GetItemIcon(shopData.priceID);

        buyCount = 1;
        textBuyCount.text = buyCount.ToString();

        return true;
    }

    public void OnClickUpButton()
    {
        if(buyCount >= 99)
        {
            return;
        }

        buyCount += 1;
        textBuyCount.text = buyCount.ToString();
        textCost.text = (priceCount * buyCount * 0.0001).ToString("N4");
    }

    public void OnClickDownButton()
    {
        if (buyCount <= 1)
        {
            return;
        }

        buyCount -= 1;
        textBuyCount.text = buyCount.ToString();
        textCost.text = (priceCount * buyCount * 0.0001).ToString("N4");
    }

    public void OnClickOKButton()
    {
        // 구매 신청
        PacketManager.Inst.RequestShopBuyItem(index, buyCount);
    }
}
