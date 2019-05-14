using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeResuiltItem : MonoBehaviour {
    public Image imageItem;
    public Image imageGrade;
    public Text textTier;
    public Text textUpgrade;

    public void updateView(UserEquipmentData equipmentData)
    {
        imageItem.sprite = CSVData.Inst.GetEquipmentIcon(equipmentData.id);
        imageGrade.sprite = CSVData.Inst.GetSpriteGrade(equipmentData.gradeType);
        textTier.text = string.Format("{0}T", CSVData.Inst.GetEquipmentData(equipmentData.id).tier);
        textUpgrade.text = string.Format("+{0}", equipmentData.upgrade);
    }
}
