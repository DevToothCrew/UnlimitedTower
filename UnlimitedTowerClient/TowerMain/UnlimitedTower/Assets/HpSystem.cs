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

    // void Update()
    // {
    //     for (int i = 0; i < 10; i++)
    //     {
    //         playerHp[i].transform.position = Camera.main.WorldToScreenPoint(
    //             TestBattle.Inst.PlayerCharacter[i].transform.position + new Vector3(0, TestBattle.Inst.PlayerCharacterControl[i].information.Height, 0));
    //             
    //         enemyHp[i].transform.position = Camera.main.WorldToScreenPoint(
    //             TestBattle.Inst.EnemyCharacter[i].transform.position + new Vector3(0, TestBattle.Inst.EnemyCharacterControl[i].information.Height, 0));
    //         
    //         playerHpImage[i].fillAmount = (float)TestBattle.Inst.PlayerCharacterControl[i].NowHp / TestBattle.Inst.PlayerCharacterControl[i].MaxHp;
    //         enemyHpImage[i].fillAmount = (float)TestBattle.Inst.EnemyCharacterControl[i].NowHp / TestBattle.Inst.EnemyCharacterControl[i].MaxHp;
    // 
    //         if (TestBattle.Inst.PlayerCharacterControl[i].isDie)
    //             playerHp[i].SetActive(false);
    //         if (TestBattle.Inst.EnemyCharacterControl[i].isDie)
    //             enemyHp[i].SetActive(false);
    //     }
    // }

}
