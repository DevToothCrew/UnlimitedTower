using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotScript : MonoBehaviour {

    public Image Lockimg;
    public Image ItemImage;
    
    public enum InventorySlotState
    {
        Locked,
        EquipMent,
        ETC
    }
    public InventorySlotState inventorySlotState;

    public void register(UserMountItemData mountitemdata)
    {
        Lockimg.gameObject.SetActive(false);
        ItemImage.gameObject.SetActive(false);
        
        ItemImage.sprite = ErdManager.instance.MountitemSprite[mountitemdata.mountitemNum];
    }
    public void register(UserEtcItemData etcitemdata)
    {
        Lockimg.gameObject.SetActive(false);
        ItemImage.gameObject.SetActive(false);

        ItemImage.sprite = ErdManager.instance.etcitemSprite[etcitemdata.etcItemNum];
    }
    public void to_locked()
    {
        inventorySlotState = InventorySlotState.Locked;

        Lockimg.gameObject.SetActive(true);
        ItemImage.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if (inventorySlotState == InventorySlotState.Locked)
        {
            to_locked();
        }
    }
}
