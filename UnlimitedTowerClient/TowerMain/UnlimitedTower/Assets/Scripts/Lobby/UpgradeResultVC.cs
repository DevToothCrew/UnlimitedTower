using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeResultVC : MonoBehaviour {
    public Text textResult;

    public Image imageResultEffect;
    public Transform FrameResultImage;
    
    public GameObject resultMonsterPrefab;
    public GameObject resultItemPrefab;
    

    public void updateView(bool is_success, UserMonsterData monsterData)
    {
        if (is_success)
        {
            textResult.text = "Upgrade Sucess!!!";
        }
        else
        {
            textResult.text = "Upgrade Fail!!!";
        }

        GameObject resultMonster = Instantiate(resultItemPrefab);
        resultMonster.transform.SetParent(FrameResultImage);
        //resultItem.GetComponent<BurnResultItemInfo>().UpdateItem(getItemID, getItemCount);
        
    }

    public void updateView(bool is_success, UserEquipmentData equipmentData)
    {
        if (is_success)
        {
            textResult.text = "Upgrade Sucess!!!";
        }
        else
        {
            textResult.text = "Upgrade Fail!!!";
        }

        GameObject resultMonster = Instantiate(resultItemPrefab);
        resultMonster.transform.SetParent(FrameResultImage);
        //resultItem.GetComponent<BurnResultItemInfo>().UpdateItem(getItemID, getItemCount);
    }

    public void OnClickButtonOK()
    {
        Destroy(this.gameObject);
    }
}
