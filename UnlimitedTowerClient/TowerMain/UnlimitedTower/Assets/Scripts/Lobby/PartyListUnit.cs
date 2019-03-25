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

        textLevel.text = string.Format("{0}", partyInfo.PartyInfoList[partyInfo.selected_tab][main_idx].level);
        textStr.text = string.Format("{0}", partyInfo.PartyInfoList[partyInfo.selected_tab][main_idx].str);
        textDex.text = string.Format("{0}", partyInfo.PartyInfoList[partyInfo.selected_tab][main_idx].dex);
        textInt.text = string.Format("{0}", partyInfo.PartyInfoList[partyInfo.selected_tab][main_idx].mInt);


    }
}
