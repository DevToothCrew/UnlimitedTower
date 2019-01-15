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

    public void Register(UserMountItemData mountitemdata)
    {
        this.mountitemdata = mountitemdata;
        isregistered = true;

        itemimage.sprite = ErdManager.instance.MountitemSprite[mountitemdata.mountitemNum];
        teartext.text = mountitemdata.tearNum + "T";
        upgradetext.text = "+"+ mountitemdata.enforceCount;
        UpdateMounted();

        mountitemdata.mountedChanged += UpdateMounted;
    }
    public void Deregister()
    {
        isregistered = false;
        mountitemdata.mountedChanged -= UpdateMounted;
    }

    public void UpdateMounted()
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
            Deregister();
        }
        Deregister();
    }


    public void OnClick()
    {
        PopupUIsManager.instance.iteminfopannel.Register(mountitemdata);
    }
}
