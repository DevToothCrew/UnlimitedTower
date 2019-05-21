using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBuyPackagePopup : MonoBehaviour {


    public bool SetShopPackageBuyPopup(ShopProductInfo getInfo)
    {
        DBShopData shopData = CSVData.Inst.GetShopData(getInfo.index);
        if (shopData == null)
        {
            DebugLog.Log(false, "Invalid Shop Object ID : " + getInfo.index);
            return false;
        }

        DBPackageData packageData = CSVData.Inst.GetPackageData(shopData.id);

        return true;
    }
}
