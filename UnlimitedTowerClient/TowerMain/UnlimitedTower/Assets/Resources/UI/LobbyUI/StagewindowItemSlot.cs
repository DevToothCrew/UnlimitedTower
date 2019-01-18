using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagewindowItemSlot : MonoBehaviour {

    public Image itemimage;
    public Text tearText;
    public Text upgradeText;

    public Image noneObj;



    // FSM
    bool registered = false;
    UserMountItemData itemdata;


    public void RegisterItem(UserMountItemData itemdata)
    {
        this.itemdata = itemdata;
        registered = true;
        Initialize();


        itemimage.gameObject.SetActive(true);
        itemimage.sprite = ErdManager.instance.MountitemSprite[itemdata.mountitemNum];
        tearText.gameObject.SetActive(true);
        tearText.text = itemdata.tearNum + "T";
        upgradeText.gameObject.SetActive(true);
        upgradeText.text = "+"+itemdata.upgradeCount;
    }
    public void Deregister()
    {
        registered = false;

        Initialize();
        noneObj.gameObject.SetActive(true);
    }

    public void OnClick()
    {
        if (registered == false)
        {
            return;
        }



    }
    public void Initialize()
    {
        itemimage.gameObject.SetActive(false);
        tearText.gameObject.SetActive(false);
        upgradeText.gameObject.SetActive(false);

        noneObj.gameObject.SetActive(true);
    }
}
