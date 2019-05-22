using UnityEngine;
using UnityEngine.UI;

public class ShopBuyPopup : MonoBehaviour {

    public int index;
    public SHOP_TYPE shopType;
    public int id;
    public int buyCount;
    public int priceCount;
    public Text textBuyCount;

    public Text textName;
    public Text textExplain;

    public Image imageObject;
    public Text textCount;

    public Image imageCost;
    public Text textCost;

    public bool SetShopBuyPopup(ShopProductInfo getInfo)
    {
        DBShopData shopData = CSVData.Inst.GetShopData(getInfo.index);
        if (shopData == null)
        {
            DebugLog.Log(false, "Invalid Shop Object ID : " + getInfo.index);
            return false;
        }

        index = getInfo.index;
        shopType = getInfo.type;
        id = getInfo.id;

        textCount.text = "X " + shopData.productCount;
        priceCount = shopData.priceCount;

        if (shopType == SHOP_TYPE.ETC)
        {
            textCost.text = priceCount.ToString();
        }
        else
        {
            textCost.text = (priceCount * 0.0001).ToString("N4");
        }

        DBItemData itemData = CSVData.Inst.GetItemData(shopData.productID);
        if(itemData == null)
        {
            itemData = CSVData.Inst.GetItemData(500002);
        }

        textName.text = itemData.name;
        textExplain.text = itemData.description;

        imageObject.sprite = itemData.ItemIcon;
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
        if (shopType == SHOP_TYPE.ETC)
        {
            textCost.text = (priceCount * buyCount).ToString();
        }
        else
        {
            textCost.text = (priceCount * buyCount * 0.0001).ToString("N4");
        }
    }

    public void OnClickDownButton()
    {
        if (buyCount <= 1)
        {
            return;
        }

        buyCount -= 1;
        textBuyCount.text = buyCount.ToString();
        if (shopType == SHOP_TYPE.ETC)
        {
            textCost.text = (priceCount * buyCount).ToString();
        }
        else
        {
            textCost.text = (priceCount * buyCount * 0.0001).ToString("N4");
        }
    }

    public void OnClickOKButton()
    {
        DBShopData shopData = CSVData.Inst.GetShopData(index);
        if(shopData == null)
        {
            DebugLog.Log(false, "Invalid ShopData");
        }

        if (shopData.priceID == 500001)
        {
            if((ulong)shopData.priceCount > UserDataManager.Inst.GetUserUTG())
            {
                SimpleErrorPopupVC.Inst.UpdateErrorText("Not Enough UTG");
                return;
            }

            PacketManager.Inst.RequestShopBuyItem(index, 2, buyCount);
        }
        else if (shopData.priceID == 500002)
        {
            if ((ulong)shopData.priceCount > UserDataManager.Inst.GetUserEOS())
            {
                SimpleErrorPopupVC.Inst.UpdateErrorText("Not Enough EOS");
                return;
            }

            PacketManager.Inst.RequestShopBuyItem(index, 1, buyCount);
        }
        else
        {
            if(shopData.priceCount > UserDataManager.Inst.GetItemCount(shopData.priceID))
            {
                SimpleErrorPopupVC.Inst.UpdateErrorText("Not Enough Item");
                return;
            }

            PacketManager.Inst.RequestShopBuyItem(index, 2, buyCount);
        }
    }

    public void OnClickMaxButton()
    {
        // 금액 맥스로 할것인지 카운트 맥스로 할것인지?
        buyCount = 99;
        textBuyCount.text = buyCount.ToString();
        if (shopType == SHOP_TYPE.ETC)
        {
            textCost.text = (priceCount * buyCount).ToString();
        }
        else
        {
            textCost.text = (priceCount * buyCount * 0.0001).ToString("N4");
        }
    }
}
