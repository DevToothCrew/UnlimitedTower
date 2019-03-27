using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyListUnit : ScrollListUnit {
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
            textLevel.text = string.Format("{0}", partyInfo.ServantList[main_idx].level);
            textStr.text = string.Format("{0}", partyInfo.ServantList[main_idx].status.basicStr + partyInfo.ServantList[main_idx].status.plusStr);
            textDex.text = string.Format("{0}", partyInfo.ServantList[main_idx].status.basicDex + partyInfo.ServantList[main_idx].status.plusDex);
            textInt.text = string.Format("{0}", partyInfo.ServantList[main_idx].status.basicInt + partyInfo.ServantList[main_idx].status.plusInt);
        }
        else if (partyInfo.getMenuType() == PartyInfoVC.menu_type.MONSTER)
        {
            textLevel.text = string.Format("{0}", partyInfo.MonsterList[main_idx].level);
            textStr.text = string.Format("{0}", partyInfo.MonsterList[main_idx].status.basicStr + partyInfo.MonsterList[main_idx].status.plusStr);
            textDex.text = string.Format("{0}", partyInfo.MonsterList[main_idx].status.basicDex + partyInfo.MonsterList[main_idx].status.plusDex);
            textInt.text = string.Format("{0}", partyInfo.MonsterList[main_idx].status.basicInt + partyInfo.MonsterList[main_idx].status.plusInt);
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
        partyInfo.scrollList.MoveScrollSelectedUnit(this.RectTr.anchoredPosition, main_idx);
        partyInfo.updateDetailInfo(getUnitIdx());
    }
}
