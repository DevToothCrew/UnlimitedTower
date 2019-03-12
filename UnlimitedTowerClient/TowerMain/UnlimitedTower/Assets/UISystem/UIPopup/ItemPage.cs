using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPage : MonoBehaviour {

    public GameObject[] page = new GameObject[3];
    public GameObject itemObject;
    public Transform itemListTransform;

    public ITEM_PAGE_STATE state;

    public void Awake()
    {
        state = ITEM_PAGE_STATE.LIST;
        OnClickButton(0);
    }

    public void OnClickButton(int num)
    {
        for (int i = 0; i < (int)ITEM_PAGE_STATE.MAX; i++)
        {
            if (i == num)
            {
                page[i].SetActive(true);
            }
            else
            {
                page[i].SetActive(false);
            }
        }

        if (num == (int)ITEM_PAGE_STATE.LIST)
        {
            List<UserMountItemData> itemList = UserDataManager.Inst.GetMountItemList();

            if (itemList == null)
            {
                return;
            }

            GameObject newObject;
            ItemObject itemObjectTemp;

            int itemCount = itemList.Count;
            if (itemCount > 50)
            {
                itemCount = 50;
            }

            for (int i = 0; i < itemCount; i++)
            {
                newObject = Instantiate(itemObject) as GameObject;
                itemObjectTemp = newObject.GetComponent<ItemObject>();
                itemObjectTemp.SetMountItemData(itemList[i]);

                newObject.transform.SetParent(itemListTransform);
            }
        }
    }
}

public enum ITEM_PAGE_STATE
{
    LIST = 0,
    UPGRADE = 1,
    SELL = 2,
    MAX = 3,
}