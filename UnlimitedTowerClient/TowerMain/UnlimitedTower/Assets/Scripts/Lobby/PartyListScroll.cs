using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyListScroll : ScrollListManager {
    public RectTransform rectScrollLayer;
    public GameObject objPrefabUnit;

    private void Awake()
    {
        rectTrScrollLayer = rectScrollLayer;
        prefabUnit = objPrefabUnit;
    }

    public override void selectedUnit()
    {
        base.selectedUnit();

        if (scrollRect.velocity.y == 0f) {
            PartyInfoVC.Inst.updateDetailInfo(selected_main_idx);
        }
    }
}
