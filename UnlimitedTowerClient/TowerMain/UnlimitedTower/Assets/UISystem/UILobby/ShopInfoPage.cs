using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInfoPage : MonoSingleton<ShopInfoPage>
{

    public Transform shopObjectList;
    public GameObject shopObjectPrefab;
    public GameObject[] shopObjects = null;

    public GameObject shopBuyPopup;

    public void SetShopInfo(List<ShopProductInfo> getShopProductInfo)
    {
        shopBuyPopup.SetActive(false);

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
        Cheat.Inst.RequestShopInfo( (SHOP_TYPE)type );
        //PacketManager.Inst.RequestShopInfo((SHOP_TYPE)type);
    }

    public void SetShopBuyPopup(ShopProductInfo getProductInfo)
    {
        if(shopBuyPopup.activeSelf == true)
        {
            return;
        }

        if(shopBuyPopup.GetComponent<ShopBuyPopup>().SetShopBuyPopup(getProductInfo) == false)
        {
            return;
        }

        shopBuyPopup.SetActive(true);
    }

    public void OnClickPopupCancelButton()
    {
        shopBuyPopup.SetActive(false);
    }
}
