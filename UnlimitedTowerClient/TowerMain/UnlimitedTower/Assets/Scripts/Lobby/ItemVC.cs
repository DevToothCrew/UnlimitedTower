using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemVC : MonoSingleton<ItemVC>
{
    //SubView
    public GameObject FrameSubView;

    //Info
    public GameObject FrameItemInfo;


    //Menu Buttons UI
    public Text textButtonBack;
    public Button[] buttonMenu = new Button[4];

    void Start()
    {
        updateAllView();
    }

    //화면 전체 Update (메뉴버튼, 상세정보창, 스크롤 등)
    void updateAllView()
    {
        FrameItemInfo.SetActive(true);
    }
}
