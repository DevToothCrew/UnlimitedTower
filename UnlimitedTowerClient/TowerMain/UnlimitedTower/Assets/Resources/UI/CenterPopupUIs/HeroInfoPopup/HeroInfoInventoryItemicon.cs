using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoInventoryItemicon : MonoBehaviour {

    public Image itemimage;
    public Text teartext;
    public Text upgradetext;
    public Image mountedImage;

    public GameObject infoUIObj;    // 정보/장착
    public Text mountText;  // 장착인지 해제인지



    // FSM
    public bool isregistered = false;
    public UserMountItemData mountitemdata;

    public void Register(UserMountItemData mountitemdata)
    {
        this.mountitemdata = mountitemdata;
        isregistered = true;

        infoUIObj.SetActive(false);
        itemimage.sprite = ErdManager.instance.MountitemSprite[mountitemdata.mountitemNum];
        teartext.text = mountitemdata.tearNum + "T";
        upgradetext.text = "+"+ mountitemdata.upgradeCount;
        UpdateMounted();

        mountitemdata.mountedChanged += UpdateMounted;
    }
    public void Deregister()
    {
        isregistered = false;
        mountitemdata.mountedChanged -= UpdateMounted;
    }
    private void OnDisable()
    {
        if (isregistered)
        {
            Deregister();
        }
        Deregister();
    }






    public void UpdateMounted()
    {
        mountedImage.gameObject.SetActive(mountitemdata.isMounted);
    }

    // BTN OnClick
    public void OnClickSlot()
    {
        infoUIObj.SetActive(true);
        mountText.text = mountitemdata.isMounted ? "해제" : "장착";
    }
    public void OnClickMountBtn()
    {
        infoUIObj.SetActive(false);

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
    }
    public void OnClickInfoBtn()
    {
        infoUIObj.SetActive(false);
        PopupUIsManager.instance.iteminfopannel.Register(mountitemdata);
    }
}
