using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpManager : MonoBehaviour {
    private GameObject[] HpObj = new GameObject[20];
    private Image[] Hpimg = new Image[20];

    private void Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            HpObj[i] = transform.GetChild(0).GetChild(i).gameObject;
            HpObj[i + 10] = transform.GetChild(1).GetChild(i).gameObject;
            Hpimg[i] = transform.GetChild(0).GetChild(i).GetChild(1).transform.GetComponent<Image>();
            Hpimg[i + 10] = transform.GetChild(1).GetChild(i).GetChild(1).transform.GetComponent<Image>();
        }
    }

    private void Update()
    {
        for (int i = 0; i < 20; i++)
        {
            if (BattleManager.Inst.isPlace[i] == true)
            {
                HpObj[i].transform.position = Camera.main.WorldToScreenPoint(
                  BattleManager.Inst.character[i].transform.position +
                  new Vector3(0, BattleManager.Inst.charInfo[i].Height, 0));
                Hpimg[i].fillAmount = (float)BattleManager.Inst.status[i].NowHp / BattleManager.Inst.status[i].Status[EFFECT_ID.HP];
                if (BattleManager.Inst.status[i].NowHp <= 0)
                    HpObj[i].SetActive(false);
            }
            else
            {
                if (HpObj[i].activeSelf)
                HpObj[i].SetActive(false);
            }
        }
    }
}
