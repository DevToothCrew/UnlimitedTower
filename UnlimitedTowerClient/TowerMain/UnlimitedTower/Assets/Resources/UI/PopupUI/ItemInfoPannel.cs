using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPannel : MonoBehaviour
{
    public UserMountItemData mountitemdata;

    public Text itemname;

    // icon
    public Image itemimage;
    public Text teartext;
    public Text upgradeText;

    // 
    public Text teartext2;
    public Text upgradetext2;
    public Text totalPowertext;

    public List<Image> jobimageList;

    public Text str;
    public Text dex;
    public Text INT;
    public Text ATK;
    public Text DEF;
    public Text SPD;
    public Text HP;
    public Text CRIPROB;
    public Text CRIDMG;


    public void Register(UserMountItemData mountitemdata)
    {
        this.mountitemdata = mountitemdata;
        MountItemEntity.Param param = ErdManager.instance.getmountitemEntityTable_nullPossible(mountitemdata.mountitemNum);


        // 아이템 아이콘, 티어, 
        itemname.text = param.itemName;
        itemimage.sprite = ErdManager.instance.MountitemSprite[mountitemdata.mountitemNum];
        teartext.text = mountitemdata.tearNum + "T";
        upgradeText.text = "+" + mountitemdata.enforceCount;

        teartext2.text = mountitemdata.tearNum + "T";
        upgradetext2.text = "+" + mountitemdata.enforceCount +"/"+ Etc.maxItemUpgradeNum;

        // job
        for (int i = 0; i < jobimageList.Count; i++)
        {
            jobimageList[i].gameObject.SetActive(false);
        }
        jobimageList[param.possibleJobNum].gameObject.SetActive(true);


        // 스탯
        List<MountItemStatEntity.Param> statlist = ErdManager.instance.getMountItemStatEntityTable_nullPossible(mountitemdata.mountitemNum);
        StatInfo statclass = Etc.instance.getTotalstat(statlist);

        str.gameObject.SetActive(statclass.str > 0);
        str.text = (int)statclass.str + "";

        dex.gameObject.SetActive(statclass.dex > 0);
        dex.text = (int)statclass.dex + "";

        INT.gameObject.SetActive(statclass.INT > 0);
        INT.text = (int)statclass.INT + "";

        ATK.gameObject.SetActive(statclass.atk > 0);
        ATK.text = (int)statclass.atk + "";

        DEF.gameObject.SetActive(statclass.def > 0);
        DEF.text = (int)statclass.def + "";

        SPD.gameObject.SetActive(statclass.spd > 0);
        SPD.text = (int)statclass.spd + "";

        HP.gameObject.SetActive(statclass.hp > 0);
        HP.text = (int)statclass.hp + "";

        CRIPROB.gameObject.SetActive(statclass.criprob > 0);
        CRIPROB.text = (int)statclass.criprob + "%";

        CRIDMG.gameObject.SetActive(statclass.cridmg > 0);
        CRIDMG.text = (int)statclass.cridmg + "";

        gameObject.SetActive(true);
    }

    public void BackbtnOnclick()
    {
        gameObject.SetActive(false);
    }
}
