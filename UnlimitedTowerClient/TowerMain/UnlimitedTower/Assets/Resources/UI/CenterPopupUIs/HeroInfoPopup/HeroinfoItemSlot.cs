using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroinfoItemSlot : MonoBehaviour {

    [SerializeField] MountitemType mountItemType;

    [SerializeField] HeroInfoPopup heroinfopopup;

    public Image itemimage;
    public Image lockimage;
    public Text teartext;
    public Text upgradetext;

    // 상태
    public UserServantData servantdata;
    public UserMountItemData mountitemdata;
    public void Register(UserServantData servantdata)
    {
        this.servantdata = servantdata;
        servantdata.mountItemListChangeEvent += mountitemchanged;
        mountitemchanged();
    }
    private void OnDisable()
    {
        servantdata.mountItemListChangeEvent -= mountitemchanged;
    }

    public void mountitemchanged()
    {
        UserMountItemData mountitemdata = servantdata.mountItemList.Find((rowdata) => {

            MountItemEntity.Param info = ErdManager.instance.getmountitemEntityTable_nullPossible(rowdata.mountitemNum);
            MountitemType itemtype = info.mountitemType;

            return rowdata.isMounted && HeroInfoPopup.instance.servant.index == rowdata.mountServantIndex && itemtype == mountItemType;
        });
        if (mountitemdata != null)
        {
            this.mountitemdata = mountitemdata;

            itemimage.gameObject.SetActive(true);
            teartext.gameObject.SetActive(true);
            upgradetext.gameObject.SetActive(true);
            lockimage.gameObject.SetActive(false);

            teartext.text = mountitemdata.tearNum + "T";
            upgradetext.text = "+" + mountitemdata.upgradeCount;
            itemimage.sprite = ErdManager.instance.MountitemSprite[mountitemdata.mountitemNum];

        }
        else
        {
            itemimage.gameObject.SetActive(false);
            teartext.gameObject.SetActive(false);
            upgradetext.gameObject.SetActive(false);
            lockimage.gameObject.SetActive(true);
        }
    }

    public void OnClick()
    {
        // 껏다가
        HeroInfoPopup.instance.heroinfo_invenpannel.gameObject.SetActive(false);

        // 켜기
        HeroInfoPopup.instance.heroinfo_invenpannel.displayType = mountItemType;
        HeroInfoPopup.instance.heroinfo_invenpannel.gameObject.SetActive(true);
    }
}
