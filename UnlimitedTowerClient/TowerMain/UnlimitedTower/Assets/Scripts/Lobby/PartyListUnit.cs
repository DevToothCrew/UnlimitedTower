using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyListUnit : ScrollListUnit {
    public Image ImageGrade;
    public Image imageCharacter;
    public Text textLevel;
    public Text textExpPer;
    public Image imageExp;
    public Text textStr;
    public Text textDex;
    public Text textInt;
    public Image imageType;

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
            ImageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)partyInfo.ServantList[main_idx].grade);
            imageCharacter.sprite = CSVData.Inst.GetServantData(partyInfo.ServantList[main_idx].id).servantIcon;
            textLevel.text = string.Format("{0}", partyInfo.ServantList[main_idx].level);
            textStr.text = string.Format("{0}", partyInfo.ServantList[main_idx].status.basicStr);
            textDex.text = string.Format("{0}", partyInfo.ServantList[main_idx].status.basicDex);
            textInt.text = string.Format("{0}", partyInfo.ServantList[main_idx].status.basicInt);

            imageType.sprite = CSVData.Inst.GetSpriteServantJob(partyInfo.ServantList[main_idx].jobType);

            DBExpData dbExpData = CSVData.Inst.GetExpData(partyInfo.ServantList[main_idx].level);
            if (dbExpData == null)
            {
                Debug.Log("Invalid Level Data");
            }
            else
            {
                int exExp = 0;
                if (partyInfo.ServantList[main_idx].level - 1 > 0)
                {
                    DBExpData exDBExpData = CSVData.Inst.GetExpData(partyInfo.ServantList[main_idx].level - 1);
                    if (exDBExpData == null)
                    {
                        Debug.Log("Invalid Level Data");
                    }
                    else
                    {
                        exExp = exDBExpData.charExp;
                    }
                }
                float expPer = (exExp - partyInfo.ServantList[main_idx].exp) / (float)(exExp - dbExpData.charExp);
                textExpPer.text = (int)(expPer * 100) + "%";
                imageExp.fillAmount = expPer;
            }
        }
        else if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.MONSTER_INFO)
        {
            ImageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)partyInfo.MonsterList[main_idx].grade);
            imageCharacter.sprite = CSVData.Inst.GetMonsterData(partyInfo.MonsterList[main_idx].id).monsterIcon;
            textLevel.text = string.Format("{0}", partyInfo.MonsterList[main_idx].level);
            textStr.text = string.Format("{0}", partyInfo.MonsterList[main_idx].status.basicStr);
            textDex.text = string.Format("{0}", partyInfo.MonsterList[main_idx].status.basicDex);
            textInt.text = string.Format("{0}", partyInfo.MonsterList[main_idx].status.basicInt);

            imageType.sprite = CSVData.Inst.GetSpriteElementType(partyInfo.MonsterList[main_idx].elementType);

            DBExpData dbExpData = CSVData.Inst.GetExpData(partyInfo.MonsterList[main_idx].level);
            if (dbExpData == null)
            {
                Debug.Log("Invalid Level Data");
            }
            else
            {
                int exExp = 0;
                if (partyInfo.MonsterList[main_idx].level - 1 > 0)
                {
                    DBExpData exDBExpData = CSVData.Inst.GetExpData(partyInfo.MonsterList[main_idx].level - 1);
                    if (exDBExpData == null)
                    {
                        Debug.Log("Invalid Level Data");
                    }
                    else
                    {
                        exExp = exDBExpData.charExp;
                    }
                }

                float expPer = (exExp - partyInfo.MonsterList[main_idx].exp) / (float)(exExp - dbExpData.charExp);
                textExpPer.text = (int)(expPer * 100) + "%";
                imageExp.fillAmount = expPer;
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
            if (!SubViewDeconstruction.checkInst())
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
            else
            {
                SubViewDeconstruction.Inst.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
                int chracter_unit_idx = 0;
                if (SubViewDeconstruction.Inst.GetDeconstructionType() == DECONSTRUCTION_TYPE.SERVANT)
                {
                    chracter_unit_idx = partyInfo.ServantList[main_idx].index;
                }
                else if (SubViewDeconstruction.Inst.GetDeconstructionType() == DECONSTRUCTION_TYPE.MONSTER)
                {
                    chracter_unit_idx = partyInfo.MonsterList[main_idx].index;
                }
                SubViewDeconstruction.Inst.InsertUnit(chracter_unit_idx);
            }
            
        }
            
    }
}
