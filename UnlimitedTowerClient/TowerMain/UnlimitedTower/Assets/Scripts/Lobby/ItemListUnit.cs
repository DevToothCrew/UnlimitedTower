using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListUnit : ScrollListUnit {
    public Image ImageGrade;
    public Image imageItem;
    public Text textUpgrade;
    public Image[] imageStats = new Image[3];
    public Text[] textStats = new Text[3];

    PartyInfoVC partyInfo;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void UpdateUnit(int _main_idx)
    {
        main_idx = _main_idx;

        partyInfo = PartyInfoVC.Inst;

        //Todo :set Image
        ImageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)SubViewEquipment.Inst.EquipmentList[main_idx].grade);
        imageItem.sprite = CSVData.Inst.GetEquipmentData(SubViewEquipment.Inst.EquipmentList[main_idx].id).equipmentIcon;
        textUpgrade.text = string.Format("+{0}", SubViewEquipment.Inst.EquipmentList[main_idx].upgrade);

        for (EQUIPMENT_OPTION_TYPE type = EQUIPMENT_OPTION_TYPE.NONE; type < EQUIPMENT_OPTION_TYPE.MAX; type++)
        {
            if (SubViewEquipment.Inst.EquipmentList[main_idx].optionType == type)
            {
                //imageStats[0].sprite = spriteStat[(int)type];
                textStats[0].text = string.Format("{0}", SubViewEquipment.Inst.EquipmentList[main_idx].value);
            }
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
        SubViewEquipment.Inst.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
        SubViewEquipment.Inst.updateChangeItemInfo(SubViewEquipment.Inst.EquipmentList[main_idx].index);
    }
}
