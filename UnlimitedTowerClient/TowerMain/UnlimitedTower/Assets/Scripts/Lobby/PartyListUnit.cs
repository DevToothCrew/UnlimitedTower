using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyListUnit : ScrollListUnit {
    public Image ImageGrade;
    public Image imageCharacter;
    public Text textCharacterName;
    public Text textLevel;
    public Text textPower;
    public Image imageType;
    public Text textUpgrade;

    PartyInfoVC partyInfo = null;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void UpdateUnit(int _main_idx)
    {
        main_idx = _main_idx;

        partyInfo = PartyInfoVC.Inst;

        //Todo :set Image
        if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.SERVANT_INFO)
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

            ImageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)partyInfo.ServantList[selected_idx].grade);
            imageCharacter.sprite = CSVData.Inst.GetServantData(partyInfo.ServantList[selected_idx].id).servantIcon;
            textCharacterName.text = CSVData.Inst.GetServantData(partyInfo.ServantList[selected_idx].id).name;
            textLevel.text = string.Format("{0}", partyInfo.ServantList[selected_idx].level);
            textPower.text = string.Format("{0}", Calculator.GetPower(partyInfo.ServantList[selected_idx].status, partyInfo.ServantList[selected_idx].level));
            textUpgrade.text = string.Format(" ");
            imageType.sprite = CSVData.Inst.GetSpriteServantJob(partyInfo.ServantList[selected_idx].jobType);
        }
        else if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.MONSTER_INFO)
        {
            int selected_idx;
            if (SubViewUpgrade.checkInst() && unit_controller.Equals(SubViewUpgrade.Inst))
            {
                selected_idx = SubViewUpgrade.Inst.scrollListData[main_idx];
            }
            else if (SubViewDeconstruction.checkInst() && unit_controller.Equals(SubViewDeconstruction.Inst))
            {
                selected_idx = SubViewDeconstruction.Inst.scrollListData[main_idx];
            }
            else
            {
                selected_idx = main_idx;
            }

            ImageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)partyInfo.MonsterList[selected_idx].grade);
            imageCharacter.sprite = CSVData.Inst.GetMonsterData(partyInfo.MonsterList[selected_idx].id).monsterIcon;
            textCharacterName.text = CSVData.Inst.GetMonsterData(partyInfo.MonsterList[selected_idx].id).name;
            textLevel.text = string.Format("{0}", partyInfo.MonsterList[selected_idx].level);
            textPower.text = string.Format("{0}", Calculator.GetPower(partyInfo.MonsterList[selected_idx].status, partyInfo.MonsterList[selected_idx].level));
            string upgrade = string.Format(" ");
            if(partyInfo.MonsterList[selected_idx].upgrade > 0)
            {
                upgrade = string.Format("+{0}", partyInfo.MonsterList[selected_idx].upgrade);
            }
            textUpgrade.text = upgrade;

            imageType.sprite = CSVData.Inst.GetSpriteElementType(partyInfo.MonsterList[selected_idx].elementType);
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
        if (partyInfo != null)
        {
            if (SubViewDeconstruction.checkInst())
            {
                SubViewDeconstruction subview_deconstruction = SubViewDeconstruction.Inst;
                subview_deconstruction.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
                int chracter_unit_idx = 0;
                if (subview_deconstruction.GetDeconstructionType() == DECONSTRUCTION_TYPE.SERVANT)
                {
                    chracter_unit_idx = partyInfo.ServantList[subview_deconstruction.scrollListData[main_idx]].index;
                }
                else if (subview_deconstruction.GetDeconstructionType() == DECONSTRUCTION_TYPE.MONSTER)
                {
                    chracter_unit_idx = partyInfo.MonsterList[subview_deconstruction.scrollListData[main_idx]].index;
                }
                SubViewDeconstruction.Inst.InsertUnit(chracter_unit_idx);
                
            }
            else if (SubViewUpgrade.checkInst())
            {
                SubViewUpgrade.Inst.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
                int selected_unit_idx = partyInfo.MonsterList[SubViewUpgrade.Inst.scrollListData[main_idx]].index;

                SubViewUpgrade.Inst.InsertUnit(selected_unit_idx);
            }
            else
            {
                partyInfo.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
                if (partyInfo.selectedMenu == PartyInfoVC.menu_type.SERVANT || partyInfo.selectedMenu == PartyInfoVC.menu_type.MONSTER)
                {
                    partyInfo.updateDetailInfo(getUnitIdx());
                }
                else if (partyInfo.selectedMenu == PartyInfoVC.menu_type.FORMATION)
                {
                    int chracter_unit_idx = 0;
                    if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.SERVANT_INFO)
                    {
                        chracter_unit_idx = partyInfo.ServantList[main_idx].index;
                    }
                    else if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.MONSTER_INFO)
                    {
                        chracter_unit_idx = partyInfo.MonsterList[main_idx].index;
                    }
                    partyInfo.InsertUnit(chracter_unit_idx);
                }
            }
        }
            
    }
}
