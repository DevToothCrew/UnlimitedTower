using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroinfoItemSlot : MonoBehaviour {

    public Image itemimage;
    public Image lockimage;
    public Text teartext;
    public Text upgradetext;

    [SerializeField] int itemslottype;

    public UserMountItemData mountitemdata;


    public void OnEnable()
    {
        displayUpdate();
    }
    public void displayUpdate()
    {
        UserMountItemData mountitemdata = UserDataManager.Inst.MountItemList.Find((rowdata) => { return rowdata.isMounted &&  HeroInfoPopup.instance.servant.index == rowdata.mountServantIndex; });
        if (mountitemdata != null)
        {
            this.mountitemdata = mountitemdata;

            itemimage.gameObject.SetActive(true);
            teartext.gameObject.SetActive(true);
            upgradetext.gameObject.SetActive(true);
            lockimage.gameObject.SetActive(false);

            teartext.text = mountitemdata.tearNum + "T";
            upgradetext.text = "+" + mountitemdata.enforceCount;
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
        HeroInfoPopup.instance.heroinfo_invenpannel.gameObject.SetActive(true);
    }
}
