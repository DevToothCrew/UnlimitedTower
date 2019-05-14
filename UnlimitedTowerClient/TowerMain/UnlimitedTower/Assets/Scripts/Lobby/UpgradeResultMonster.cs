using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeResultMonster : MonoBehaviour {
    public Image imageMonster;
    public Image imageGrade;
    public Image imageTribe;
    public Image imageType;
    public Text textUpgrade;

    public void updateView(UserMonsterData monsterData)
    {
        imageMonster.sprite = CSVData.Inst.GetMonsterIcon(monsterData.id);
        imageGrade.sprite = CSVData.Inst.GetSpriteGrade(monsterData.gradeType);
        imageTribe.sprite = CSVData.Inst.GetSmallSpriteTribeType(monsterData.tribeType);
        imageType.sprite = CSVData.Inst.GetSpriteElementType(monsterData.elementType);
        textUpgrade.text = string.Format("+{0}", monsterData.upgrade);
    }
}
