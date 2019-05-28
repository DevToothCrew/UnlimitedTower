using UnityEngine.UI;

public class ItemListUnit : ScrollListUnit {
    public Image ImageGrade;
    public Image imageItem;
    public Text textItemCount;
    public Text textItemName;
    public Text textTypeText;
    public Text textType;
    

    ItemInfoManager itemInfo;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void UpdateUnit(int _main_idx)
    {
        main_idx = _main_idx;

        int selected_idx = main_idx;
        itemInfo = ItemInfoManager.Inst;

        DBItemData itemData = CSVData.Inst.GetItemData(itemInfo.ItemList[selected_idx].id);
        ImageGrade.sprite = CSVData.Inst.GetSpriteGrade(itemData.grade);
        imageItem.sprite = itemData.ItemIcon;

        int item_count = itemInfo.ItemList[selected_idx].count;
        if (item_count < 0)
        {
            DebugLog.Log(true, "Item Count 0");
            textItemCount.text = "";
        }
        else if (item_count == 1)
        {
            textItemCount.text = "";
        }
        else
        {
            textItemCount.text = string.Format("X{0}", item_count);
        }
        
        textItemName.text = itemData.name;

        textType.text = itemData.itemType;
    }

    public override void Selected(bool selected)
    {
        if (selected)
        {
            GetComponent<Image>().sprite = GetComponent<Button>().spriteState.pressedSprite;
        }
        else
        {
            GetComponent<Image>().sprite = GetComponent<Button>().spriteState.disabledSprite;
        }
    }

    public override void OnClickUnit()
    {
        ItemInfoManager.Inst.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
        ItemInfoManager.Inst.updateItemDetailInfo(main_idx);
    }
}
