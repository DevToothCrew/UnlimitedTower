using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopResourceList : MonoSingleton<ShopResourceList> {

    public Text[] textResourceCount = new Text[8];

    private void OnEnable()
    {
        SetResourceCount();
    }

    public void SetResourceCount()
    {
        textResourceCount[0].text = " x " + UserDataManager.Inst.GetItemCount(500200);
        textResourceCount[1].text = " x " + UserDataManager.Inst.GetItemCount(500210);
        textResourceCount[2].text = " x " + UserDataManager.Inst.GetItemCount(500220);
        textResourceCount[3].text = " x " + UserDataManager.Inst.GetItemCount(500230);
        textResourceCount[4].text = " x " + UserDataManager.Inst.GetItemCount(510010);
        textResourceCount[5].text = " x " + UserDataManager.Inst.GetItemCount(510020);
        textResourceCount[6].text = " x " + UserDataManager.Inst.GetItemCount(510030);
        textResourceCount[7].text = " x " + UserDataManager.Inst.GetItemCount(510040);
    }
}
