using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSystem : MonoBehaviour
{
    private GameObject[] playerHp = new GameObject[10];
    private GameObject[] enemyHp = new GameObject[10];
    private Image[] playerHpImage = new Image[10];
    private Image[] enemyHpImage = new Image[10];
    private Image attackerImage;
    private Image targetImage;
    private readonly Color on = new Color(1, 1, 1, 1);
    private readonly Color off = new Color(1, 1, 1, 0);

    void Start () {
        for (int i = 0; i < 10; i++)
        {
            playerHp[i] = transform.GetChild(0).GetChild(i).gameObject;
            enemyHp[i] = transform.GetChild(1).GetChild(i).gameObject;
            playerHpImage[i] = transform.GetChild(0).GetChild(i).GetChild(1).transform.GetComponent<Image>();
            enemyHpImage[i] = transform.GetChild(1).GetChild(i).GetChild(1).transform.GetComponent<Image>();
        }
        attackerImage = transform.GetChild(2).GetComponent<Image>();
        targetImage = transform.GetChild(3).GetComponent<Image>();
    }

    void Update()
    {
        // 추후 최적화 작업
        if (BattleSystem.Inst.battleInformation.attackerIndex != -1)
        {
            attackerImage.color = on;
            targetImage.color = on;
            if (BattleSystem.Inst.battleInformation.isPlayerTurn)
            {
                attackerImage.transform.position = playerHp[BattleSystem.Inst.battleInformation.attackerIndex].transform.position + new Vector3(0, 50, 0);
                targetImage.transform.position = enemyHp[BattleSystem.Inst.battleInformation.targetIndex].transform.position + new Vector3(0, 50, 0);
            }
            else
            {
                attackerImage.transform.position = enemyHp[BattleSystem.Inst.battleInformation.attackerIndex].transform.position + new Vector3(0, 50, 0);
                targetImage.transform.position = playerHp[BattleSystem.Inst.battleInformation.targetIndex].transform.position + new Vector3(0, 50, 0);
            }
        }
        else
        {
            attackerImage.color = off;
            targetImage.color = off;
        }
        
        for (int i = 0; i < 10; i++)
        {
            if (BattleSystem.Inst.characterisVoid[i] == true)
            {
                playerHp[i].transform.position = Camera.main.WorldToScreenPoint(
                  BattleSystem.Inst.playerCharacter[i].transform.position +
                  new Vector3(0, BattleSystem.Inst.playerCharacterControl[i].transform.GetChild(0).GetComponent<CharacterInformation>().Height, 0));
                playerHpImage[i].fillAmount = (float)BattleSystem.Inst.playerCharacterControl[i].nowHp / BattleSystem.Inst.playerCharacterControl[i].maxHp;
                if (BattleSystem.Inst.playerCharacterControl[i].nowHp <= 0)
                    playerHp[i].SetActive(false);
            }
            else
            {
                playerHp[i].SetActive(false);
            }

            if (BattleSystem.Inst.characterisVoid[i + 10] == true)
            {
                enemyHp[i].transform.position = Camera.main.WorldToScreenPoint(
                 BattleSystem.Inst.enemyCharacter[i].transform.position +
                 new Vector3(0, BattleSystem.Inst.enemyCharacterControl[i].transform.GetChild(0).GetComponent<CharacterInformation>().Height, 0));
                enemyHpImage[i].fillAmount = (float)BattleSystem.Inst.enemyCharacterControl[i].nowHp / BattleSystem.Inst.enemyCharacterControl[i].maxHp;
                if (BattleSystem.Inst.enemyCharacterControl[i].nowHp <= 0)
                    enemyHp[i].SetActive(false);
            }
            else
            {
                enemyHp[i].SetActive(false);
            }
        }
    }

}
