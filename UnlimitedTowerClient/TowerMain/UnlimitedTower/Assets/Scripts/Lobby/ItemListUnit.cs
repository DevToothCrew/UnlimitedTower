using UnityEngine.UI;

public class ItemListUnit : ScrollListUnit {
    public Image ImageGrade;
    public Image imageItem;
    // TODO : 추후 추가
    //public Text textUpgrade;
    public Image imageStats;
    public Text textStats;

    InventoryVC inventoryInfo;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void UpdateUnit(int _main_idx)
    {
        main_idx = _main_idx;

        if (LobbyManager.Inst.popupState == POPUP_STATE.Servant)
        {
            ImageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)SubViewEquipment.Inst.EquipmentList[main_idx].grade);
            imageItem.sprite = CSVData.Inst.GetEquipmentData(SubViewEquipment.Inst.EquipmentList[main_idx].id).equipmentIcon;
            //textUpgrade.text = string.Format("+{0}", SubViewEquipment.Inst.EquipmentList[main_idx].upgrade);

            imageStats.sprite = CSVData.Inst.GetSpriteOptionType(SubViewEquipment.Inst.EquipmentList[main_idx].optionType);
            textStats.text = string.Format("{0}", SubViewEquipment.Inst.EquipmentList[main_idx].value);
        }
        else if (LobbyManager.Inst.popupState == POPUP_STATE.Weapon)
        {
            inventoryInfo = InventoryVC.Inst;

            //Todo :set Image
            ImageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu][main_idx].grade);
            imageItem.sprite = CSVData.Inst.GetEquipmentData(inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu][main_idx].id).equipmentIcon;
            //textUpgrade.text = string.Format("+{0}", inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu][main_idx].upgrade);

            imageStats.sprite = CSVData.Inst.GetSpriteOptionType(inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu][main_idx].optionType);
            textStats.text = string.Format("{0}", inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu][main_idx].value);

        }
        
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
        if (LobbyManager.Inst.popupState == POPUP_STATE.Servant)
        {
            SubViewEquipment.Inst.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
            SubViewEquipment.Inst.updateChangeItemInfo(main_idx);
        }
        else if (LobbyManager.Inst.popupState == POPUP_STATE.Weapon)
        {
            if (!SubViewDeconstruction.checkInst())
            {
                InventoryVC.Inst.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
                InventoryVC.Inst.updateDetailInfo(main_idx);
            }
            else
            {
                SubViewDeconstruction.Inst.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
                int item_unit_idx = 0;
                item_unit_idx = inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu][main_idx].index;
                SubViewDeconstruction.Inst.InsertUnit(item_unit_idx);

            }   
        }
    }
}
