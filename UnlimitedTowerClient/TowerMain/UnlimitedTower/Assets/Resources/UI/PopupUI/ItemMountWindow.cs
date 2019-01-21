using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMountWindow : MonoBehaviour {

    public UserMountItemData mountitemdata;
    public Text mountText;

    public void Register(UserMountItemData mountitemdata)
    {
        this.mountitemdata = mountitemdata;

        gameObject.SetActive(true);
    }

    // BTN OnClick
    public void OnClickMountBtn()
    {

        // 장착되어있다면 -> 해제
        if (mountitemdata.isMounted)
        {
            GameDataManager.instance.DemountItem(mountitemdata);
        }
        // 장착안되어있다면 -> 해당 서번트에게 장착
        else
        {
            GameDataManager.instance.MountItem(mountitemdata, HeroInfoPopup.instance.servant);
        }


        gameObject.SetActive(false);
    }
    public void OnClickInfoBtn()
    {
        PopupUIsManager.instance.iteminfopannel.Register(mountitemdata);
        gameObject.SetActive(false);
    }
}
