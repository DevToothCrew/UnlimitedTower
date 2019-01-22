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

    [SerializeField] GameObject sellCheckedObj;



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
        teartext.text = mountitemdata.tearNum + "T";
        upgradetext.text = "+"+mountitemdata.upgradeCount;

        // 해당 서번트가 판매리스트에 포함되어있다면
        if (PopupUIsManager.instance.inventoryInfoPopup.sellingMountItemList.Contains(mountitemdata) &&
            PopupUIsManager.instance.inventoryInfoPopup.modeType == InventoryInfoPopup.MODE_TYPE.SELLING)
        {
            sellCheckedObj.SetActive(true);
        }

        //
        InventoryInfoPopup.mountitemListChanged += mountitemListChanged;
        InventoryInfoPopup.modeChanged += modeChanged;
        modeChanged();

    }
    public void Register(UserEtcItemData etcitemdata)
    {
        inventorySlotState = InventorySlotState.ETC;
        this.etcItemData = etcitemdata;

        OffAllImages();

        ItemImage.gameObject.SetActive(true);
        ItemImage.sprite = ErdManager.instance.etcitemSprite[etcitemdata.etcItemNum];

        // 해당 서번트가 판매리스트에 포함되어있다면
        if (PopupUIsManager.instance.inventoryInfoPopup.sellingETCItemList.Contains(etcitemdata) &&
            PopupUIsManager.instance.inventoryInfoPopup.modeType == InventoryInfoPopup.MODE_TYPE.SELLING)
        {
            sellCheckedObj.SetActive(true);
        }

        //
        InventoryInfoPopup.etcitemListChanged += etcitemListChanged;
        InventoryInfoPopup.modeChanged += modeChanged;
        modeChanged();
    }
    public void ToLocked()
    {
        inventorySlotState = InventorySlotState.Locked;

        OffAllImages();

        Lockimg.gameObject.SetActive(true);

        // 이벤트 떼기
        switch (inventorySlotState)
        {
            case InventorySlotState.EquipMent:
                InventoryInfoPopup.mountitemListChanged -= mountitemListChanged;
                break;
            case InventorySlotState.ETC:
                InventoryInfoPopup.etcitemListChanged -= etcitemListChanged;
                break;
        }
        InventoryInfoPopup.modeChanged -= modeChanged;
        sellCheckedObj.SetActive(false);
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
        //
        switch (PopupUIsManager.instance.inventoryInfoPopup.modeType)
        {

            // 보여주기 모드 -> 아이템 정보 띄우기
            case InventoryInfoPopup.MODE_TYPE.JUST_DISPLAY:
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
                break;

            // 셀링 모드이면 -> 셀링리스트에 포함시키기
            case InventoryInfoPopup.MODE_TYPE.SELLING:
                {
                    switch (inventorySlotState)
                    {
                        case InventorySlotState.EquipMent:
                            {
                                PopupUIsManager.instance.inventoryInfoPopup.OnClickInSellingmode(mountItemData);
                            }
                            break;
                        case InventorySlotState.ETC:
                            {
                                PopupUIsManager.instance.inventoryInfoPopup.OnClickInSellingmode(etcItemData);
                            }
                            break;
                    }
                }
                break;
        }
        
    }


    // 
    public void mountitemListChanged(UserMountItemData monsterdata)
    {
        // 셀링모드가 아니라면, 끄고 return
        if (PopupUIsManager.instance.inventoryInfoPopup.modeType == InventoryInfoPopup.MODE_TYPE.JUST_DISPLAY)
        {
            sellCheckedObj.SetActive(false);
            return;
        }

        // 내몬스터가 아니라면, return
        if (monsterdata != mountItemData)
        {
            return;
        }


        //
        sellCheckedObj.SetActive(PopupUIsManager.instance.inventoryInfoPopup.sellingMountItemList.Contains(monsterdata));
    }
    public void etcitemListChanged(UserEtcItemData servantdata)
    {
        // 셀링모드가 아니라면, 끄고 return
        if (PopupUIsManager.instance.partnerInfoPopup.modeType == PartnerInfoPopup.MODE_TYPE.JUST_DISPLAY)
        {
            sellCheckedObj.SetActive(false);
            return;
        }

        // 내몬스터가 아니라면, return
        if (etcItemData != servantdata)
        {
            return;
        }


        //
        sellCheckedObj.SetActive(PopupUIsManager.instance.inventoryInfoPopup.sellingETCItemList.Contains(etcItemData));
    }
    //
    public void modeChanged()
    {
        // 셀링모드가 아니면,  꺼주기
        if (PopupUIsManager.instance.inventoryInfoPopup.modeType == InventoryInfoPopup.MODE_TYPE.JUST_DISPLAY)
        {
            sellCheckedObj.SetActive(false);
        }

    }



    private void OnDisable()
    {
        if (inventorySlotState != InventorySlotState.Locked)
        {
            ToLocked();
        }
    }

}
