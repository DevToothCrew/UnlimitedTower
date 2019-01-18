using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUIsManager : MonoBehaviour {

    // Singleton 아님. 로비씬에서만 쓰이는 static class
    public static PopupUIsManager instance;
    private void Awake()
    {
        instance = this;
    }


    public ItemMountWindow itemmountwindow;
    public ItemInfoPannel iteminfopannel;
}
