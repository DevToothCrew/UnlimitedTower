using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeResultMonster : MonoBehaviour {
    public Image imageMonster;
    public Image imageTribe;
    public Image imageType;

    public void updateView(UserMonsterData monsterData)
    {
        imageMonster.sprite = CSVData.Inst.GetMonsterIcon(monsterData.id);
        imageTribe.sprite = CSVData.Inst.GetSmallSpriteTribeType(monsterData.tribeType);
        imageType.sprite = CSVData.Inst.GetSpriteElementType(monsterData.elementType);
    }
}
