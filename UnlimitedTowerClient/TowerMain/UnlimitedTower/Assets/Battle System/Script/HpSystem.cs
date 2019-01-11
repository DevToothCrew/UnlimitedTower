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
    private Color on = new Color(1, 1, 1, 1);
    private Color off = new Color(1, 1, 1, 0);

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
        if (BattleSystem.Inst.battleInformation.AttackerIndex != -1)
        {
            attackerImage.color = on;
            targetImage.color = on;
            if (BattleSystem.Inst.battleInformation.isPlayerTurn)
            {
                attackerImage.transform.position = playerHp[BattleSystem.Inst.battleInformation.AttackerIndex].transform.position + new Vector3(0, 50, 0);
                targetImage.transform.position = enemyHp[BattleSystem.Inst.battleInformation.TargetIndex].transform.position + new Vector3(0, 50, 0);
            }
            else
            {
                attackerImage.transform.position = enemyHp[BattleSystem.Inst.battleInformation.AttackerIndex].transform.position + new Vector3(0, 50, 0);
                targetImage.transform.position = playerHp[BattleSystem.Inst.battleInformation.TargetIndex].transform.position + new Vector3(0, 50, 0);
            }
        }
        else
        {
            attackerImage.color = off;
            targetImage.color = off;
        }
        
        for (int i = 0; i < 10; i++)
        {
            playerHp[i].transform.position = Camera.main.WorldToScreenPoint(
                BattleSystem.Inst.PlayerCharacter[i].transform.position + 
                new Vector3(0, BattleSystem.Inst.PlayerCharacterControl[i].transform.GetChild(0).GetComponent<CharacterInformation>().Height, 0));
                
            enemyHp[i].transform.position = Camera.main.WorldToScreenPoint(
                BattleSystem.Inst.EnemyCharacter[i].transform.position + 
                new Vector3(0, BattleSystem.Inst.EnemyCharacterControl[i].transform.GetChild(0).GetComponent<CharacterInformation>().Height, 0));
            
            playerHpImage[i].fillAmount = (float)BattleSystem.Inst.PlayerCharacterControl[i].NowHp / BattleSystem.Inst.PlayerCharacterControl[i].MaxHp;
            enemyHpImage[i].fillAmount = (float)BattleSystem.Inst.EnemyCharacterControl[i].NowHp / BattleSystem.Inst.EnemyCharacterControl[i].MaxHp;
        
            if (BattleSystem.Inst.PlayerCharacterControl[i].NowHp <= 0)
                playerHp[i].SetActive(false);
            if (BattleSystem.Inst.EnemyCharacterControl[i].NowHp <= 0)
                enemyHp[i].SetActive(false);
        }
    }

}
