using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSystem : MonoBehaviour
{
    private GameObject[] hpObject = new GameObject[20];
    private Image[] hpImage = new Image[20];
    private Image attackerImage;
    private Image targetImage;
    private readonly Color on = new Color(1, 1, 1, 1);
    private readonly Color off = new Color(1, 1, 1, 0);

    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            hpObject[i] = transform.GetChild(0).GetChild(i).gameObject;
            hpObject[i + 10] = transform.GetChild(1).GetChild(i).gameObject;
            hpImage[i] = transform.GetChild(0).GetChild(i).GetChild(1).transform.GetComponent<Image>();
            hpImage[i + 10] = transform.GetChild(1).GetChild(i).GetChild(1).transform.GetComponent<Image>();
        }
        attackerImage = transform.GetChild(2).GetComponent<Image>();
        targetImage = transform.GetChild(3).GetComponent<Image>();
    }

    void Update()
    {
        // 추후 최적화 작업
        if (BattleSystem.Inst.battleInformation.attackerIndex != -1)
        {
            // attackerImage.color = on;
            // targetImage.color = on;
            // if (BattleSystem.Inst.battleInformation.isPlayerTurn)
            // {
            //     attackerImage.transform.position = playerHp[BattleSystem.Inst.battleInformation.attackerIndex].transform.position + new Vector3(0, 50, 0);
            //     targetImage.transform.position = enemyHp[BattleSystem.Inst.battleInformation.targetIndex].transform.position + new Vector3(0, 50, 0);
            // }
            // else
            // {
            //     attackerImage.transform.position = enemyHp[BattleSystem.Inst.battleInformation.attackerIndex].transform.position + new Vector3(0, 50, 0);
            //     targetImage.transform.position = playerHp[BattleSystem.Inst.battleInformation.targetIndex].transform.position + new Vector3(0, 50, 0);
            // }
        }
        else
        {
            attackerImage.color = off;
            targetImage.color = off;
        }

        for (int i = 0; i < 20; i++)
        {
            if (BattleSystem.Inst.characterisPlace[i] == true)
            {
                hpObject[i].transform.position = Camera.main.WorldToScreenPoint(
                  BattleSystem.Inst.characterObject[i].transform.position +
                  new Vector3(0, BattleSystem.Inst.characterControl[i].transform.GetChild(0).GetComponent<CharacterInformation>().Height, 0));
                hpImage[i].fillAmount = (float)BattleSystem.Inst.characterControl[i].nowHp / BattleSystem.Inst.characterControl[i].maxHp;
                if (BattleSystem.Inst.characterControl[i].nowHp <= 0)
                    hpObject[i].SetActive(false);
            }
            else
            {
                hpObject[i].SetActive(false);
            }
        }
    }

}
