using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurnResultVC : MonoBehaviour {

    public GameObject[] ResultItemUnits = new GameObject[10];
    public GameObject resultItemPrefab;
    public Transform ResultItemList;

    public Text textResultUTG;

    public void updateView(List<UserItemData> getItemList, ulong getUTG) 
    {
        if(ResultItemList != null)
        {
            for(int i = 0; i < ResultItemUnits.Length; i++)
            {
                Destroy(ResultItemUnits[i]);
            }

            ResultItemUnits = null;
        }
        if (getItemList.Count > 0)
        {
            ResultItemUnits = new GameObject[getItemList.Count];
            for (int i = 0; i < getItemList.Count; i++)
            {
                int getItemID = getItemList[i].id;
                int getItemCount = 0;

                for (int j = 0; j < getItemList[i].itemInfoList.Count; j++)
                {
                    getItemCount += getItemList[i].itemInfoList[j].count;
                }

                UserItemData itemData = UserDataManager.Inst.GetItemInfo(getItemID);
                if (itemData != null)
                {
                    int nowItemCount = 0;
                    for (int j = 0; j < itemData.itemInfoList.Count; j++)
                    {
                        nowItemCount += itemData.itemInfoList[j].count;
                    }

                    if (getItemCount != nowItemCount)
                    {
                        getItemCount -= nowItemCount;
                    }
                    else
                    {
                        getItemCount = 0;
                    }
                }

                if (getItemCount != 0)
                {
                    GameObject resultItem = Instantiate(resultItemPrefab);
                    resultItem.transform.SetParent(ResultItemList);
                    resultItem.GetComponent<BurnResultItemInfo>().UpdateItem(getItemID, getItemCount);
                    ResultItemUnits[i] = resultItem;
                }
            }

            UserDataManager.Inst.SetItemList(getItemList);
        }

        textResultUTG.text = (getUTG * 0.0001).ToString("N4");
    }

    public void OnClickButtonOK()
    {
        Destroy(this.gameObject);
    }
}
