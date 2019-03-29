using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyListUnit : ScrollListUnit {
    public Image ImageGrade;
    public Image imageCharacter;
    public Text textLevel;
    public RectTransform rectExp;
    public Text textStr;
    public Text textDex;
    public Text textInt;

    PartyInfoVC partyInfo;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void UpdateUnit(int _main_idx)
    {
        main_idx = _main_idx;

        partyInfo = ((PartyInfoVC)unit_controller);

        //Todo :set Image
        if (partyInfo.getMenuType() == PartyInfoVC.menu_type.SERVANT)
        {
            ImageGrade.sprite = CSVData.Inst.getSpriteGrade((GRADE_TYPE)partyInfo.ServantList[main_idx].grade);
            imageCharacter.sprite = CSVData.Inst.GetServantData(partyInfo.ServantList[main_idx].id).servantIcon;
            textLevel.text = string.Format("{0}", partyInfo.ServantList[main_idx].level);
            textStr.text = string.Format("{0}", partyInfo.ServantList[main_idx].status.basicStr);
            textDex.text = string.Format("{0}", partyInfo.ServantList[main_idx].status.basicDex);
            textInt.text = string.Format("{0}", partyInfo.ServantList[main_idx].status.basicInt);
        }
        else if (partyInfo.getMenuType() == PartyInfoVC.menu_type.MONSTER)
        {
            ImageGrade.sprite = CSVData.Inst.getSpriteGrade((GRADE_TYPE)partyInfo.MonsterList[main_idx].grade);
            imageCharacter.sprite = CSVData.Inst.GetMonsterData(partyInfo.MonsterList[main_idx].id).monsterIcon;
            textLevel.text = string.Format("{0}", partyInfo.MonsterList[main_idx].level);
            textStr.text = string.Format("{0}", partyInfo.MonsterList[main_idx].status.basicStr);
            textDex.text = string.Format("{0}", partyInfo.MonsterList[main_idx].status.basicDex);
            textInt.text = string.Format("{0}", partyInfo.MonsterList[main_idx].status.basicInt);
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
            partyInfo.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
            if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.SERVANT_INFO || partyInfo.currentScrollType == PartyInfoVC.scroll_type.MONSTER_INFO)
            {
                partyInfo.updateDetailInfo(getUnitIdx());
            }
        }
            
    }
}
