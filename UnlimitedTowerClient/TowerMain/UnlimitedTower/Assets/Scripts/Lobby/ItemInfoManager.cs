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

    //아이템은 최대 99개로 묶이기에 리스트 표시용으로 따로 생성
    public struct item_unit
    {
        public  int id;     //아이템 타입
        public  int idx;
        public  int count;
    }
    public List<item_unit> ItemList = new List<item_unit>();

    private bool is_init_scroll = false;

    private void OnEnable()
    {
        FrameItemInfo.SetActive(false);
        FrameNoneItem.SetActive(false);

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
            textNoneItem.text = string.Format("None ITEM");
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

        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
        scrollList.SetItemOrder(getOrder());
        scrollList.ScrollViewDidScroll();
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

        List<UserItemData> userItemList = new List<UserItemData>();
        userItemList = UserDataManager.Inst.GetItemList();

        if (userItemList == null)
        {
            DebugLog.Log(false, "Invalid User Item List");
        }
        else
        {
            for (int i = 0; i < userItemList.Count; i++)
            {
                //Item 갯수가 1 이상인 것만 ItemList에 Add
                if (UserDataManager.Inst.GetItemCount(userItemList[i].id) > 0)
                {
                    UserItemData i_data = UserDataManager.Inst.GetItemInfo(userItemList[i].id);
                    for (int j = 0; j < i_data.itemInfoList.Count; j++)
                    {
                        if (i_data.itemInfoList[j].count > 0)
                        {
                            item_unit i_unit = new item_unit();
                            i_unit.id = userItemList[i].id;
                            i_unit.idx = i_data.itemInfoList[j].index;
                            i_unit.count = i_data.itemInfoList[j].count;
                            ItemList.Add(i_unit);
                        }
                    }
                }
            }
        }
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
            DebugLog.Log(false, "Invalid Item  Index : " + unit_idx);
        }

        DBItemData dBItemData = CSVData.Inst.GetItemData(itemData.id);
        if (dBItemData == null)
        {
            DebugLog.Log(false, "Invalid equipment ID : " + dBItemData.id);
        }

        imageItem.sprite = dBItemData.ItemIcon;
        textItemName.text = dBItemData.name;
        //textTypeText.text;
        textType.text = string.Format("{0}", dBItemData.itemType);
        textItemContext.text = string.Format("{0}", dBItemData.description);

    }
}
