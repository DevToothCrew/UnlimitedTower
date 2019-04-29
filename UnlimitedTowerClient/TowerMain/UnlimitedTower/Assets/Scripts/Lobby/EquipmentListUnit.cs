using UnityEngine.UI;

public class EquipmentListUnit : ScrollListUnit
{
    public Image ImageGrade;
    public Image imageItem;
    public Text textItemName;
    // TODO : 추후 추가
    public Text textUpgrade;
    public Image imageStats;
    public Text textStats;

    EquipmentInfoManager equipmentInfo;

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
            textItemName.text = CSVData.Inst.GetEquipmentData(SubViewEquipment.Inst.EquipmentList[main_idx].id).name;

            string upgrade = string.Format(" ");
            if (SubViewEquipment.Inst.EquipmentList[main_idx].upgrade > 0)
            {
                upgrade = string.Format("+{0}", SubViewEquipment.Inst.EquipmentList[main_idx].upgrade);
            }
            textUpgrade.text = upgrade;

            imageStats.sprite = CSVData.Inst.GetSpriteOptionType(SubViewEquipment.Inst.EquipmentList[main_idx].optionType);
            textStats.text = string.Format("{0}", SubViewEquipment.Inst.EquipmentList[main_idx].value);
        }
        else if (LobbyManager.Inst.popupState == POPUP_STATE.Weapon)
        {
            int selected_idx;
            if (SubViewDeconstruction.checkInst() && unit_controller.Equals(SubViewDeconstruction.Inst))
            {
                selected_idx = SubViewDeconstruction.Inst.scrollListData[main_idx];
            }
            else
            {
                selected_idx = main_idx;
            }

            equipmentInfo = EquipmentInfoManager.Inst;

            //Todo :set Image
            ImageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][selected_idx].grade);
            imageItem.sprite = CSVData.Inst.GetEquipmentData(equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][selected_idx].id).equipmentIcon;
            textItemName.text = CSVData.Inst.GetEquipmentData(equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][selected_idx].id).name;

            string upgrade = string.Format(" ");
            if (equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][selected_idx].upgrade > 0)
            {
                upgrade = string.Format("+{0}", equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][selected_idx].upgrade);
            }
            textUpgrade.text = upgrade;

            imageStats.sprite = CSVData.Inst.GetSpriteOptionType(equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][selected_idx].optionType);
            textStats.text = string.Format("{0}", equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][selected_idx].value);

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
                equipmentInfo.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
                equipmentInfo.updateDetailInfo(main_idx);
            }
            else
            {
                SubViewDeconstruction.Inst.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
                int item_unit_idx = 0;
                item_unit_idx = equipmentInfo.EquipmentList[(int)equipmentInfo.GetSelectedMenu()][SubViewDeconstruction.Inst.scrollListData[main_idx]].index;
                SubViewDeconstruction.Inst.InsertUnit(item_unit_idx);

            }
        }
    }
}
