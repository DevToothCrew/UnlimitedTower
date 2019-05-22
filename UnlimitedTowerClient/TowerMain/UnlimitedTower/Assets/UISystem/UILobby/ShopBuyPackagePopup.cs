using UnityEngine;
using UnityEngine.UI;

public class ShopBuyPackagePopup : MonoBehaviour {

    public int index;
    public Text textName;
    public Text textGetUTG;
    public GameObject utgObject;
    public Image imageObjectIcon;
    public Image imagePriceIcon;
    public Text textPrice;
    public GameObject[] packageList = new GameObject[8];

    public bool SetShopPackageBuyPopup(ShopProductInfo getInfo)
    {
        DBShopData shopData = CSVData.Inst.GetShopData(getInfo.index);
        if (shopData == null)
        {
            DebugLog.Log(false, "Invalid Shop Object ID : " + getInfo.index);
            return false;
        }
        index = getInfo.index;
        DBPackageData packageData = CSVData.Inst.GetPackageData(shopData.id);
        if(packageData == null)
        {
            DebugLog.Log(false, "Invalid Package ID : " + shopData.id);
            return false;
        }

        for(int i = 0; i < 8; i++)
        {
            packageList[i].SetActive(false);
        }
        packageList[getInfo.index - 16].SetActive(true);

        textName.text = packageData.name;
        if (packageData.utg > 0)
        {
            textGetUTG.text = (packageData.utg * 0.0001).ToString("N4");
            utgObject.SetActive(true);
        }
        else
        {
            utgObject.SetActive(false);
        }
        imageObjectIcon.sprite = packageData.resourceIcon;
        imagePriceIcon.sprite = CSVData.Inst.GetItemIcon(packageData.priceID);
        textPrice.text = (packageData.priceCount * 0.0001).ToString("N4");

        return true;
    }

    public void OnClickOKButton()
    {
        DBShopData shopData = CSVData.Inst.GetShopData(index);
        if (shopData == null)
        {
            DebugLog.Log(false, "Invalid ShopData");
        }

        if((ulong)shopData.priceCount > UserDataManager.Inst.GetUserEOS())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Not Enough EOS");
            return;
        }

        PacketManager.Inst.RequestShopBuyItem(index, 1, 1);
    }
}
