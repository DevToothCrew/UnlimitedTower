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
    //public GameObject[] FrameItemInfo = new GameObject[1];  //나중에 아이템 탭을 나눌 경우 여러 개로 변경


    //Menu Buttons UI
    public Text textButtonBack;
    public Button[] buttonMenu = new Button[4];

    [HideInInspector]
    public int selected_tab = 0;
    [HideInInspector]
    public int selected_unit_idx = 0;

    public Item_Menu_Type selectedMenu = Item_Menu_Type.ITEM_0;

    void Start()
    {
        OnClickMenuButton(0);
    }


    //화면 전체 Update (메뉴버튼, 상세정보창, 스크롤 등)
    void updateAllView()
    {
        for (int i = 0; i < 4; i++)
        {
            buttonMenu[i].image.sprite = buttonMenu[i].spriteState.disabledSprite;
            buttonMenu[i].GetComponentInChildren<Text>().color = Color.white;
        }

        FrameItemInfo.SetActive(true);
    }

    //좌측 메뉴버튼 클릭
    public void OnClickMenuButton(int tag)
    {
        if (tag == (int)Item_Menu_Type.ITEM_0)
        {
            selectedMenu = Item_Menu_Type.ITEM_0;
        }
        else if(tag == (int)Item_Menu_Type.ITEM_1)
        {
            selectedMenu = Item_Menu_Type.ITEM_1;
        }
        updateAllView();
    }

}
