using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoManager : MonoSingleton<ItemInfoManager> {
    public GameObject FrameItemInfo;
    public GameObject FrameNoneItem;
    public Text textNoneItem;

    public Image imageItem;

    public Text textItemName;
    public Text textTypeText;
    public Text textType;
    public Text textItemContext;


    //ScrollView UI
    public Text textOwned;
    public Text textTotal;
    public GameObject frameScroll;
    public ScrollListManager scrollList;

    private int unit_idx;

    public List<UserItemData> ItemList = new List<UserItemData>();

    private Inventory_Menu_Type selectedMenu;
    private bool is_init_scroll = false;

    private void OnEnable()
    {
        FrameItemInfo.SetActive(false);
        FrameNoneItem.SetActive(false);

        if (InventoryVC.Inst.selectedMenu == Inventory_Menu_Type.ITEM)
        {
            selectedMenu = InventoryVC.Inst.selectedMenu;

            setData();

            if (ItemList.Count > 0)
            {
                FrameItemInfo.SetActive(true);

                if (is_init_scroll)
                {
                    scrollList.SetItemOrder(getOrder());
                }
                else
                {
                    is_init_scroll = true;
                    initScrollList();
                }

                textOwned.text = string.Format("{0}", ItemList.Count);
                textTotal.text = string.Format("/ {0}", UserDataManager.Inst.GetUserInventoryInfo().itemInventory);
                if (ItemList.Count >= UserDataManager.Inst.GetUserInventoryInfo().itemInventory)
                {
                    textOwned.color = Color.red;
                }
                else
                {
                    textOwned.color = Color.white;
                }

                updateItemDetailInfo(scrollList.getFirstItemOrder());
            }
            else
            {
                FrameNoneItem.SetActive(true);
                textNoneItem.text = string.Format("None Item");
            }
        }
        else
        {
            Debug.Log("Invalid Item Menu");
            return;
        }

    }


    //현재 화면에 따른 스크롤 UI 재설정 
    public void resetScroll()
    {
        textOwned.text = string.Format("{0}", ItemList.Count);
        textTotal.text = string.Format("/ {0}", UserDataManager.Inst.GetUserInventoryInfo().itemInventory);
        if (ItemList.Count >= UserDataManager.Inst.GetUserInventoryInfo().itemInventory)
        {
            textOwned.color = Color.red;
        }
        else
        {
            textOwned.color = Color.white;
        }


        scrollList.SetItemOrder(getOrder());
        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
    }

    //스크롤 생성
    void initScrollList()
    {
        setData();

        scrollList.Init(this, 20, ItemList.Count, getOrder());
    }

    public void setData()
    {
        ItemList.Clear();

        ItemList = UserDataManager.Inst.GetItemList();
    }


    //스크롤 정렬
    private int[] getOrder()
    {
        int[] data_order;
        int total_list_num = ItemList.Count;

        data_order = new int[total_list_num];

        for (int i = 0; i < data_order.Length; i++)
        {
            data_order[i] = 0;
        }

        UserDataManager u_data = UserDataManager.Inst;

        for (int i = 0; i < total_list_num - 1; i++)
        {
            for (int j = i + 1; j < total_list_num; j++)
            {
                if (CSVData.Inst.GetItemData(ItemList[i].id).id < CSVData.Inst.GetItemData(ItemList[j].id).id)
                {
                    data_order[i]++;
                }
                else
                {
                    data_order[j]++;
                }
            }
        }

        return data_order;
    }

    //상세정보창 Update
    public void updateItemDetailInfo(int _selected_unit_idx)
    {
        unit_idx = _selected_unit_idx;

        UserItemData itemData = UserDataManager.Inst.GetItemInfo(ItemList[unit_idx].id);
        if (itemData == null)
        {
            Debug.Log("Invalid Item  Index : " + unit_idx);
        }

        DBItemData dBItemData = CSVData.Inst.GetItemData(itemData.id);
        if (dBItemData == null)
        {
            Debug.Log("Invalid equipment ID : " + dBItemData.id);
        }

        imageItem.sprite = dBItemData.ItemIcon;
        textItemName.text = dBItemData.name;
        //textTypeText.text;
        textType.text = string.Format("{0}", dBItemData.itemType);
        textItemContext.text = string.Format("{0}", "아이템 설명");

    }
}
