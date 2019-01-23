using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HeroInfoInventoryItemicon : MonoBehaviour , IPointerClickHandler
{

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
        teartext.text = mountitemdata.tierNum + "T";
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


    
    public void UpdateMounted()
    {
        mountedImage.gameObject.SetActive(mountItemData.isMounted);
    }

    // 좌 클릭시  => 정보창
    // 좌 클릭 드래그시 => 드래그됨
    // 우 클릭시    => 장착시도
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PopupUIsManager.instance.iteminfopannel.Register(mountItemData);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 장착되어있다면 -> 해제
            if (mountItemData.isMounted)
            {
                GameDataManager.instance.DemountItem(mountItemData);
            }
            // 장착안되어있다면 -> 해당 서번트에게 장착
            else
            {
                GameDataManager.instance.MountItem(mountItemData, HeroInfoPopup.instance.servantData);
            }
        }
    }
}
