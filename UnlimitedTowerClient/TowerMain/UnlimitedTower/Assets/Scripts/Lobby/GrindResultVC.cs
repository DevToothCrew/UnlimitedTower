using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrindResultVC : MonoBehaviour {
    public Image imageSuccess;
    public Image imageItemFrame;
    public Image imageItem;

    public Text textItemNum;
    public Text textItemName;

    public void updateView(bool is_success, int item_type, int item_idx, int item_num) 
    {
        if (is_success)
        {
            //imageSuccess.sprite = success sprite     //성공시 이미지
            //imageItemFrame.sprite = grade sprite     //등급 이미지
            //imageItem.sprite = item sprite           //아이템 이미지(서번트, 몬스터, 무기/갑옷/악세사리, etc)
            if (item_num > 0)
            {
                textItemNum.name = string.Format("X{0}", item_num);                     //아이템 갯수
            }
            else
            {
                textItemNum.name = "";
            }
            
            textItemName.name = "";                     //아이템 이름
        }
        else
        {
            //imageSuccess.sprite = fail sprite      //실패시 이미지
            //imageItemFrame.sprite = grade sprite  //등급 이미지
            //imageItem.sprite = item sprite    //아이템 이미지(서번트, 몬스터, 무기/갑옷/악세사리, etc)
            textItemNum.name = "";           //아이템 갯수
            textItemName.name = "";         //아이템 이름
        }
    }
	

    public void OnClickButtonOK()
    {
        Destroy(this.gameObject);
    }
}
