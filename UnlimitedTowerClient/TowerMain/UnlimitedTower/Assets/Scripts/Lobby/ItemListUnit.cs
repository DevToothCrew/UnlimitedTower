using UnityEngine.UI;

public class ItemListUnit : ScrollListUnit {
    public Image ImageGrade;
    public Image imageItem;
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
        //if (SubViewDeconstruction.checkInst() && unit_controller.Equals(SubViewDeconstruction.Inst))
        //{
        //    selected_idx = SubViewDeconstruction.Inst.scrollListData[main_idx];
        //}
        //else
        //{
        //    selected_idx = main_idx;
        //}

        itemInfo = ItemInfoManager.Inst;

        //Todo :set Image
        //ImageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][selected_idx].grade);
        imageItem.sprite = CSVData.Inst.GetItemData(itemInfo.ItemList[selected_idx].id).ItemIcon;
        textItemName.text = CSVData.Inst.GetItemData(itemInfo.ItemList[selected_idx].id).name;

        textType.text = CSVData.Inst.GetItemData(itemInfo.ItemList[selected_idx].id).itemType;
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
