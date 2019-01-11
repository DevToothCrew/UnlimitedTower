using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSystem : MonoBehaviour
{
    private GameObject[] playerHp = new GameObject[10];
    private GameObject[] enemyHp = new GameObject[10];
    public Image[] playerHpImage = new Image[10];
    private Image[] enemyHpImage = new Image[10];

    void Start () {
        for (int i = 0; i < 10; i++)
        {
            playerHp[i] = transform.GetChild(0).GetChild(i).gameObject;
            enemyHp[i] = transform.GetChild(1).GetChild(i).gameObject;
            playerHpImage[i] = transform.GetChild(0).GetChild(i).GetChild(1).transform.GetComponent<Image>();
            enemyHpImage[i] = transform.GetChild(1).GetChild(i).GetChild(1).transform.GetComponent<Image>();
        }
    }

    void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            playerHp[i].transform.position = Camera.main.WorldToScreenPoint(
                BattleSystem.Inst.PlayerCharacter[i].transform.position + new Vector3(0, BattleSystem.Inst.PlayerCharacterControl[i].transform.GetChild(0).GetComponent<CharacterInformation>().Height, 0));
                
            enemyHp[i].transform.position = Camera.main.WorldToScreenPoint(
                BattleSystem.Inst.EnemyCharacter[i].transform.position + new Vector3(0, BattleSystem.Inst.EnemyCharacterControl[i].transform.GetChild(0).GetComponent<CharacterInformation>().Height, 0));
            
            playerHpImage[i].fillAmount = (float)BattleSystem.Inst.PlayerCharacterControl[i].NowHp / BattleSystem.Inst.PlayerCharacterControl[i].MaxHp;
            enemyHpImage[i].fillAmount = (float)BattleSystem.Inst.EnemyCharacterControl[i].NowHp / BattleSystem.Inst.EnemyCharacterControl[i].MaxHp;
        
            if (BattleSystem.Inst.PlayerCharacterControl[i].NowHp <= 0)
                playerHp[i].SetActive(false);
            if (BattleSystem.Inst.EnemyCharacterControl[i].NowHp <= 0)
                enemyHp[i].SetActive(false);
        }
    }

}
