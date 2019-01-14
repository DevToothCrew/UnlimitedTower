using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoInventoryItemicon : MonoBehaviour {

    public Image itemimage;
    public Text teartext;
    public Text upgradetext;
    public Image mountedImage;

    public bool isregistered = false;
    public UserMountItemData mountitemdata;

    public void register(UserMountItemData mountitemdata)
    {
        this.mountitemdata = mountitemdata;
        isregistered = true;

        itemimage.sprite = ErdManager.instance.MountitemSprite[mountitemdata.mountitemNum];
        teartext.text = mountitemdata.tearNum + "T";
        upgradetext.text = "+"+ mountitemdata.enforceCount;
        mountedUpdate();

        mountitemdata.mountedChanged += mountedUpdate;
    }
    public void deregister()
    {
        isregistered = false;
        mountitemdata.mountedChanged -= mountedUpdate;
    }

    public void mountedUpdate()
    {
        mountedImage.gameObject.SetActive(mountitemdata.isMounted);
    }



    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        if (isregistered)
        {
            deregister();
        }
        deregister();
    }


    public void OnClick()
    {
        PopupUIsManager.instance.iteminfopannel.register(mountitemdata);
    }
}
