using UnityEngine;
using UnityEngine.UI;

public class PartyListUnit : ScrollListUnit {
    public Image ImageGrade;
    public Image imageCharacter;
    public Text textCharacterName;
    public Text textLevel;
    public Text textPower;
    public Image imageClass;
    public Image imageType;
    public Text textUpgrade;
    public GameObject imageDisable;

    PartyInfoVC partyInfo = null;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void UpdateUnit(int _main_idx)
    {
        main_idx = _main_idx;

        partyInfo = PartyInfoVC.Inst;

        int chracter_unit_idx = 0;
        //Todo :set Image
        if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.SERVANT_INFO)
        {   
            int selected_idx;
            if (SubViewDeconstruction.checkInst() && unit_controller.Equals(SubViewDeconstruction.Inst))
            {
                selected_idx = SubViewDeconstruction.Inst.scrollListData[main_idx];
                chracter_unit_idx = partyInfo.ServantList[SubViewDeconstruction.Inst.scrollListData[main_idx]].index;
            }
            else
            {
                selected_idx = main_idx;
                chracter_unit_idx = partyInfo.ServantList[main_idx].index;
            }

            ImageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)partyInfo.ServantList[selected_idx].grade);
            imageCharacter.sprite = CSVData.Inst.GetServantData(partyInfo.ServantList[selected_idx].id).servantIcon;
            textCharacterName.text = CSVData.Inst.GetServantData(partyInfo.ServantList[selected_idx].id).name;
            textLevel.text = string.Format("{0}", partyInfo.ServantList[selected_idx].level);
            textPower.text = string.Format("{0}", Calculator.GetPower(partyInfo.ServantList[selected_idx].status, partyInfo.ServantList[selected_idx].level));
            textUpgrade.text = string.Format(" ");
            imageClass.sprite = CSVData.Inst.GetSpriteServantJob(partyInfo.ServantList[selected_idx].jobType);
            imageType.gameObject.SetActive(false);
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
                chracter_unit_idx = partyInfo.MonsterList[SubViewDeconstruction.Inst.scrollListData[main_idx]].index;
            }
            else
            {
                selected_idx = main_idx;
                chracter_unit_idx = partyInfo.MonsterList[main_idx].index;
            }

            ImageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)partyInfo.MonsterList[selected_idx].grade);
            imageCharacter.sprite = CSVData.Inst.GetMonsterData(partyInfo.MonsterList[selected_idx].id).monsterIcon;
            textCharacterName.text = CSVData.Inst.GetMonsterData(partyInfo.MonsterList[selected_idx].id).name;
            textLevel.text = string.Format("{0}", partyInfo.MonsterList[selected_idx].level);
            textPower.text = string.Format("{0}", Calculator.GetPower(partyInfo.MonsterList[selected_idx].status, partyInfo.MonsterList[selected_idx].level));
            string upgrade = string.Format(" ");
            if (partyInfo.MonsterList[selected_idx].upgrade > 0)
            {
                upgrade = string.Format("+{0}", partyInfo.MonsterList[selected_idx].upgrade);
            }
            textUpgrade.text = upgrade;

            imageClass.sprite = CSVData.Inst.GetSmallSpriteTribeType(partyInfo.MonsterList[selected_idx].tribeType);
            imageType.gameObject.SetActive(true);
            imageType.sprite = CSVData.Inst.GetSpriteElementType(partyInfo.MonsterList[selected_idx].elementType);
        }

        if (partyInfo.selectedMenu == PartyInfoVC.menu_type.FORMATION && chracter_unit_idx != 0)
        {
            if (partyInfo.checkSetFormationUnit(chracter_unit_idx))
            {
                GetComponent<Image>().color = Color.cyan;
            }
            else
            {
                GetComponent<Image>().color = Color.white;
            }
        }   
        else if (SubViewDeconstruction.checkInst())
        {   
            if (SubViewDeconstruction.Inst.checkInsertedUnit(chracter_unit_idx))
            {
                GetComponent<Button>().interactable = false;
                imageDisable.SetActive(true);
            }
            else
            {
                GetComponent<Button>().interactable = true;
                imageDisable.SetActive(false);
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

                if (subview_deconstruction.checkInsertMax())
                {
                    DebugLog.Log(false, "Warning : 분해 슬롯 최대치!");
                    SimpleErrorPopupVC.Inst.UpdateErrorText("Max Burn List");
                }
                else
                {
                    if (subview_deconstruction.checkInsertedUnit(chracter_unit_idx))
                    {
                        DebugLog.Log(false, "Warning : 이미 분해 슬롯에 등록된 유닛 입니다.");
                        SimpleErrorPopupVC.Inst.UpdateErrorText("Already Listed");
                    }
                    else
                    {
                        GetComponent<Button>().interactable = false;
                        imageDisable.SetActive(true);
                        SubViewDeconstruction.Inst.InsertUnit(chracter_unit_idx);
                    }
                }
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
