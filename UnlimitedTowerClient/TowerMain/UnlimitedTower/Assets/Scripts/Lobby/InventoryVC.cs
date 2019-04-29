using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryVC : MonoSingleton<InventoryVC> {
    public GameObject FrameMain;

    //SubView
    public GameObject FrameSubView;

    //Info
    public GameObject FrameEquipmentInfo;
    public GameObject FrameItemInfo;


    //Menu Buttons UI
    public Text textButtonBack;
    public Button[] buttonMenu = new Button[4];

    [HideInInspector]
    public int selected_tab = 0;
    [HideInInspector]
    public int selected_unit_idx = 0;

    public Inventory_Menu_Type selectedMenu = Inventory_Menu_Type.WEAPON;

    void Start()
    {
        OnClickMenuButton(0);
    }


    //화면 전체 Update (메뉴버튼, 상세정보창, 스크롤 등)
    void updateAllView()
    {
        FrameMain.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            buttonMenu[i].image.sprite = buttonMenu[i].spriteState.disabledSprite;
            buttonMenu[i].GetComponentInChildren<Text>().color = Color.white;
        }

        if (selectedMenu == Inventory_Menu_Type.ITEM)
        {
            FrameEquipmentInfo.SetActive(false);
            FrameItemInfo.SetActive(true);

            buttonMenu[(int)selectedMenu].image.sprite = buttonMenu[(int)selectedMenu].spriteState.pressedSprite;
            buttonMenu[(int)selectedMenu].GetComponentInChildren<Text>().color = Color.black;
        }
        else
        {
            FrameItemInfo.SetActive(false);
            FrameEquipmentInfo.SetActive(true);

            buttonMenu[(int)selectedMenu].image.sprite = buttonMenu[(int)selectedMenu].spriteState.pressedSprite;
            buttonMenu[(int)selectedMenu].GetComponentInChildren<Text>().color = Color.black;

            EquipmentInfoManager.Inst.updateAllView();
        }

    }

    //좌측 메뉴버튼 클릭
    public void OnClickMenuButton(int tag)
    {
        if (tag == (int)Inventory_Menu_Type.WEAPON)
        {
            selectedMenu = Inventory_Menu_Type.WEAPON;
        }
        else if (tag == (int)Inventory_Menu_Type.ARMOR)
        {
            selectedMenu = Inventory_Menu_Type.ARMOR;
        }
        else if (tag == (int)Inventory_Menu_Type.ACCESSORY)
        {
            selectedMenu = Inventory_Menu_Type.ACCESSORY;
        }
        else
        {
            selectedMenu = Inventory_Menu_Type.ITEM;
        }
        updateAllView();
    }

    public void ShowDeconstruction()
    {
        FrameMain.SetActive(false);

        GameObject objSubView = Instantiate(Resources.Load("UI/Lobby/SubViewDeconstruction")) as GameObject;
        objSubView.name = "SubViewDeconstruction";
        objSubView.transform.SetParent(FrameSubView.transform);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        SubViewDeconstruction s_deconstruction = objSubView.GetComponent<SubViewDeconstruction>();
        s_deconstruction.SetDeconstructionType(DECONSTRUCTION_TYPE.EQUIPMENT);
    }

    public void ShowUpgrade()
    {
        FrameMain.SetActive(false);

        GameObject objSubView = Instantiate(Resources.Load("UI/Lobby/SubViewUpgrade")) as GameObject;
        objSubView.name = "SubViewUpgrade";
        objSubView.transform.SetParent(FrameSubView.transform);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        SubViewUpgrade s_upgrade = objSubView.GetComponent<SubViewUpgrade>();
        s_upgrade.SetUpgradeType(UPGRADE_TYPE.EQUIPMENT);
    }

}
