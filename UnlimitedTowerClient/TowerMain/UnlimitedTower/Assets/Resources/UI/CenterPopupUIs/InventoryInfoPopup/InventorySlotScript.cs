using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotScript : MonoBehaviour {

    // 공통
    public Image Lockimg;
    public Image ItemImage;

    // 마운트 아이템
    public Text teartext;
    public Text upgradetext;
    

    
    public enum InventorySlotState
    {
        Locked,
        EquipMent,
        ETC
    }
    public InventorySlotState inventorySlotState;
    UserMountItemData mountItemData;
    UserEtcItemData etcItemData;

    public void Register(UserMountItemData mountitemdata)
    {
        inventorySlotState = InventorySlotState.EquipMent;
        this.mountItemData = mountitemdata;

        OffAllImages();

        ItemImage.gameObject.SetActive(true);
        teartext.gameObject.SetActive(true);
        upgradetext.gameObject.SetActive(true);
        
        ItemImage.sprite = ErdManager.instance.MountitemSprite[mountitemdata.mountitemNum];
        teartext.text = mountitemdata.tearNum + "T" ;
        upgradetext.text = "+"+mountitemdata.upgradeCount;
    }
    public void Register(UserEtcItemData etcitemdata)
    {
        inventorySlotState = InventorySlotState.ETC;
        this.etcItemData = etcitemdata;

        OffAllImages();

        ItemImage.gameObject.SetActive(true);
        ItemImage.sprite = ErdManager.instance.etcitemSprite[etcitemdata.etcItemNum];
    }
    public void ToLocked()
    {
        inventorySlotState = InventorySlotState.Locked;

        OffAllImages();

        Lockimg.gameObject.SetActive(true);
    }

    // 모든 이미지 OFF
    public void OffAllImages()
    {
        Lockimg.gameObject.SetActive(false);
        ItemImage.gameObject.SetActive(false);
        teartext.gameObject.SetActive(false);
        upgradetext.gameObject.SetActive(false);
    }
    // 클릭시
    public void OnClick()
    {
        switch (inventorySlotState)
        {
            case InventorySlotState.Locked:
                break;
            case InventorySlotState.EquipMent:
                PopupUIsManager.instance.iteminfopannel.Register(mountItemData);
                break;
            case InventorySlotState.ETC:
                break;
        }
    }


    private void OnDisable()
    {
        if (inventorySlotState == InventorySlotState.Locked)
        {
            ToLocked();
        }
    }

}
