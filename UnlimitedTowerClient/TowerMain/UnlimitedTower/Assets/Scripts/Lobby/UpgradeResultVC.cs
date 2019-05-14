using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeResultVC : MonoBehaviour {
    public Text textResult;

    public Image imageResultEffect;
    public Transform FrameResultImage;
    public GameObject objBreak;
    
    public GameObject resultMonsterPrefab;
    public GameObject resultItemPrefab;

    public Sprite spriteSuccess;
    public Sprite spriteFail;


    public void updateView(bool is_success, UserMonsterData monsterData)
    {
        objBreak.SetActive(false);
        if (is_success)
        {
            textResult.text = "Upgrade Success!!!";
            textResult.GetComponent<UITextGradient>().m_color1 = new Color(1, 245 / 255f, 138 / 255f);
            textResult.GetComponent<UITextGradient>().m_color2 = new Color(1, 176 / 255f, 45 / 255f);
            imageResultEffect.sprite = spriteSuccess;
        }
        else
        {
            textResult.text = "Upgrade Fail...";
            textResult.GetComponent<UITextGradient>().m_color1 = new Color(150 / 255f, 150 / 255f, 150 / 255f);
            textResult.GetComponent<UITextGradient>().m_color2 = new Color(50 / 255f, 50 / 255f, 50 / 255f);
            imageResultEffect.sprite = spriteFail;
        }

        GameObject resultMonster = Instantiate(resultMonsterPrefab);
        resultMonster.transform.SetParent(FrameResultImage);
        resultMonster.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        resultMonster.GetComponent<UpgradeResultMonster>().updateView(monsterData);

    }

    public void updateView(bool is_success, UserEquipmentData equipmentData)
    {
        if (is_success)
        {
            objBreak.SetActive(false);
            textResult.text = "Upgrade Sucess!!!";
            textResult.GetComponent<UITextGradient>().m_color1 = new Color(1, 245 / 255f, 138 / 255f);
            textResult.GetComponent<UITextGradient>().m_color2 = new Color(1, 176 / 255f, 45 / 255f);
            imageResultEffect.sprite = spriteSuccess;
        }
        else
        {
            objBreak.SetActive(true);
            textResult.text = "Upgrade Fail...";
            textResult.GetComponent<UITextGradient>().m_color1 = new Color(150 / 255f, 150 / 255f, 150 / 255f);
            textResult.GetComponent<UITextGradient>().m_color2 = new Color(50 / 255f, 50 / 255f, 50 / 255f);
            imageResultEffect.sprite = spriteFail;
        }

        GameObject resultEquipment = Instantiate(resultItemPrefab);
        resultEquipment.transform.SetParent(FrameResultImage);
        resultEquipment.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        resultEquipment.GetComponent<UpgradeResuiltItem>().updateView(equipmentData);
    }

    public void OnClickButtonOK()
    {
        Destroy(this.gameObject);
    }
}
