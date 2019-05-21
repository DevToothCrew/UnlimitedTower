using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInfoPage : MonoSingleton<ShopInfoPage>
{

    public Transform shopObjectList;
    public GameObject shopObjectPrefab;
    public GameObject[] shopObjects = null;

    public GameObject shopBuyPopup;
    public GameObject shopBuyPackagePopup;

    public void SetShopInfo(List<ShopProductInfo> getShopProductInfo)
    {
        shopBuyPopup.SetActive(false);
        shopBuyPackagePopup.SetActive(false);

        if (shopObjects != null)
        {
            for(int i = 0; i < shopObjects.Length; i++)
            {
                Destroy(shopObjects[i]);
            }

            shopObjects = null;
        }

        shopObjects = new GameObject[getShopProductInfo.Count];

        for(int i = 0; i < getShopProductInfo.Count; i++)
        {
            GameObject shopProductObject = Instantiate(shopObjectPrefab);
            shopProductObject.transform.SetParent(shopObjectList);
            shopProductObject.GetComponent<ShopObjectInfo>().SetShopObjectInfo(getShopProductInfo[i]);
            shopObjects[i] = shopProductObject;
        }
    }

    public void OnClickShopMenu(int type)
    {
        PacketManager.Inst.RequestLocalShopInfo((SHOP_TYPE)type);
    }

    public void SetShopBuyPopup(ShopProductInfo getProductInfo)
    {
        if (getProductInfo.type != SHOP_TYPE.PACKAGE)
        {
            if (shopBuyPopup.activeSelf == true)
            {
                return;
            }

            if (shopBuyPopup.GetComponent<ShopBuyPopup>().SetShopBuyPopup(getProductInfo) == false)
            {
                return;
            }

            shopBuyPopup.SetActive(true);
        }
        else
        {
            if (shopBuyPackagePopup.activeSelf == true)
            {
                return;
            }

            if (shopBuyPackagePopup.GetComponent<ShopBuyPackagePopup>().SetShopPackageBuyPopup(getProductInfo) == false)
            {
                return;
            }

            shopBuyPackagePopup.SetActive(true);
        }
    }

    public void OnClickPopupCancelButton()
    {
        shopBuyPopup.SetActive(false);
        shopBuyPackagePopup.SetActive(false);
    }
}
