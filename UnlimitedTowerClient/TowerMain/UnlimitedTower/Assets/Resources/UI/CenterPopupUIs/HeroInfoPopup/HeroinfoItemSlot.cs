using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HeroinfoItemSlot : MonoBehaviour {

    [SerializeField] MountitemType mountItemType;

    [SerializeField] HeroInfoPopup heroinfopopup;

    public Image itemImage;
    public Image lockImage;
    public Text tearText;
    public Text upgradeText;

    // FSM 상태이동 함수
    public UserServantData servantData;
    public UserMountItemData mountItemData;
    public void ToRegister(UserServantData servantData)
    {
        this.servantData = servantData;
        servantData.mountItemListChangeEvent += mountitemchanged;
        mountitemchanged();
    }
    public void ToDeregister()
    {
        servantData.mountItemListChangeEvent -= mountitemchanged;
    }

    private void OnDisable()
    {
        ToDeregister();
    }
    

    public void mountitemchanged()
    {
        UserMountItemData mountItemData = servantData.mountItemList.Find((rowdata) => {

            MountItemEntity.Param info = ErdManager.instance.getmountitemEntityTable_nullPossible(rowdata.mountitemNum);
            MountitemType itemtype = info.mountitemType;

            return rowdata.isMounted && HeroInfoPopup.instance.servantData.index == rowdata.mountServantIndex && itemtype == mountItemType;
        });
        if (mountItemData != null)
        {
            this.mountItemData = mountItemData;

            itemImage.gameObject.SetActive(true);
            tearText.gameObject.SetActive(true);
            upgradeText.gameObject.SetActive(true);
            lockImage.gameObject.SetActive(false);

            tearText.text = mountItemData.tierNum + "T";
            upgradeText.text = "+" + mountItemData.upgradeCount;
            itemImage.sprite = ErdManager.instance.MountitemSprite[mountItemData.mountitemNum];

        }
        else
        {
            itemImage.gameObject.SetActive(false);
            tearText.gameObject.SetActive(false);
            upgradeText.gameObject.SetActive(false);
            lockImage.gameObject.SetActive(true);
        }
    }


    // 버튼클릭시
    public void OnClick()
    {
        // 껏다가
        HeroInfoPopup.instance.SetHeroinfoPannel(false);

        // 켜기
        HeroInfoPopup.instance.heroinfo_invenpannel.displayType = mountItemType;
        HeroInfoPopup.instance.SetHeroinfoPannel(true);
    }
}
