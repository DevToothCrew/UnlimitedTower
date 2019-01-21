using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoInventoryItemicon : MonoBehaviour {

    public Image itemimage;
    public Text teartext;
    public Text upgradetext;
    public Image mountedImage;
    
    
    // FSM
    public bool isRegistered = false;
    public UserMountItemData mountItemData;

    public void Register(UserMountItemData mountitemdata)
    {
        this.mountItemData = mountitemdata;
        isRegistered = true;
        
        itemimage.sprite = ErdManager.instance.MountitemSprite[mountitemdata.mountitemNum];
        teartext.text = mountitemdata.tearNum + "T";
        upgradetext.text = "+"+ mountitemdata.upgradeCount;

        mountitemdata.mountedChanged += UpdateMounted;
        UpdateMounted();
    }
    public void Deregister()
    {
        isRegistered = false;
        mountItemData.mountedChanged -= UpdateMounted;
    }
    private void OnDisable()
    {
        if (isRegistered)
        {
            Deregister();
        }
        Deregister();
    }




    public void OnClickSlot()
    {
        PopupUIsManager.instance.itemmountwindow.Register(mountItemData);
    }


    public void UpdateMounted()
    {
        mountedImage.gameObject.SetActive(mountItemData.isMounted);
    }

}
