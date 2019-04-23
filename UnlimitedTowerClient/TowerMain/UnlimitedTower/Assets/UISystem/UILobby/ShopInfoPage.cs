using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInfoPage : MonoSingleton<ShopInfoPage>
{

    public Transform shopObjectList;
    public GameObject shopObjectPrefab;
    public GameObject[] shopObjects = null;

    public void SetShopInfo(List<ShopProductInfo> getShopProductInfo)
    {
        if(shopObjects != null)
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
#if UNITY_EDITOR
        Cheat.Inst.RequestShopInfo( (SHOP_TYPE)type );
#else
                    PacketManager.Inst.RequestShopInfo(SHOP_TYPE.EOS);
#endif
    }
}
