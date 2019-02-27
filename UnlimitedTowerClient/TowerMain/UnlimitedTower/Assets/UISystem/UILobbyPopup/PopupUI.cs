﻿using UnityEngine.UI;
using UnityEngine;

public class PopupUI : MonoBehaviour {

    public GameObject[] popupPages = new GameObject[5];

    public void Awake()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                popupPages[i].SetActive(true);
            }
            else
            {
                popupPages[i].SetActive(false);
            }
        }
    }

    public void OnClickPopupPageButton(int num)
    {
        for(int i = 0; i < 5; i++)
        {
            if(i == num)
            {
                popupPages[i].SetActive(true);
            }
            else
            {
                popupPages[i].SetActive(false);
            }
        }
    }
}

