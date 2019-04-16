﻿using System.Collections;
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
        ResultItemUnits = new GameObject[getItemList.Count];
        for (int i = 0; i < getItemList.Count; i++)
        {
            GameObject resultItem = Instantiate(resultItemPrefab);
            resultItem.transform.SetParent(ResultItemList);
            resultItem.GetComponent<BurnResultItemInfo>().UpdateItem(getItemList[i]);
            ResultItemUnits[i] = resultItem;
        }

        textResultUTG.text = (getUTG * 0.0001).ToString("N4");
    }

    public void OnClickButtonOK()
    {
        Destroy(this.gameObject);
    }
}
