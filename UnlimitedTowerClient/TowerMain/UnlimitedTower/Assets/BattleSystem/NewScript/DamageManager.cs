using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoSingleton<DamageManager>
{
    public Sprite[] numberSprite = new Sprite[10];
    public GameObject[] textPool = new GameObject[50];
    public DamageText[] testInfo = new DamageText[50];
    public GameObject missText;

    private int Index = 0;
    private Camera camera_;
    private readonly Color RedColor = new Color(1, 0, 0, 1);
    private readonly Color YellowColor = new Color(1, 1, 0, 1);
    private readonly Color GreenColor = new Color(0, 1, 0, 1);

    public enum Attribute
    {
        DEFAULT,
        CRITICAL,
        HEAL
    };

    private void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            textPool[i] = transform.GetChild(0).GetChild(i).gameObject;
            testInfo[i] = textPool[i].GetComponent<DamageText>();
            textPool[i].SetActive(false);
        }
        camera_ = Camera.main;
        missText = GameObject.Find("Miss");
    }

    public void DamageAciton(actionInfo attackInfo, bool isHeal)
    {
        if (!attackInfo.avoid)
        {
            // 데미지 텍스트 표시와 데미지 주기
            DamageShow(attackInfo, isHeal);
        }
        else
        {
            // Miss 텍스트
            // Avoid(attackInfo.target_position);
            // BattleManager.Inst.characterControl[attackInfo.target_position].Miss();
        }
    }

    public void DamageShow(actionInfo attackInfo, bool isHeal) // 맞은 대상의 인덱스와 플레이어 여부, 데미지, 크리티컬, 힐 여부
    {
        int[] numberIndex = new int[5];

        numberIndex[0] = attackInfo.damage % 10;
        numberIndex[1] = (attackInfo.damage % 100 - numberIndex[0]) / 10;
        numberIndex[2] = (attackInfo.damage % 1000 - numberIndex[0] - numberIndex[1] * 10) / 100;
        numberIndex[3] = (attackInfo.damage % 10000 - numberIndex[0] - numberIndex[1] * 10 - numberIndex[2] * 100) / 1000;
        numberIndex[4] = (attackInfo.damage - numberIndex[0] - numberIndex[1] * 10 - numberIndex[2] * 100 - numberIndex[4] * 1000) / 10000;

        textPool[Index].SetActive(true);
        textPool[Index].transform.position =
              Camera.main.WorldToScreenPoint(BattleManager.Inst.character[attackInfo.target_position].transform.position +
              new Vector3(0, BattleManager.Inst.character[attackInfo.target_position].GetComponent<CharInfo>().Height, 0));

        for (int i = 0; i < 5; i++)
        {
            testInfo[Index].image[i].gameObject.SetActive(true);
            if (isHeal)
            {
                testInfo[Index].image[i].color = GreenColor;
            }
            else
            {
                if (!attackInfo.critical)
                    testInfo[Index].image[i].color = RedColor;
                else
                    testInfo[Index].image[i].color = YellowColor;
            }

            testInfo[Index].image[i].sprite = numberSprite[numberIndex[i]];
        }

        if (isHeal)
            BattleManager.Inst.NowHp[attackInfo.target_position] += attackInfo.damage;
        else
            BattleManager.Inst.NowHp[attackInfo.target_position] -= attackInfo.damage;

        if (BattleManager.Inst.NowHp[attackInfo.target_position] < 0)
            BattleManager.Inst.NowHp[attackInfo.target_position] = 0;
        if (BattleManager.Inst.NowHp[attackInfo.target_position] > BattleManager.Inst.MaxHp[attackInfo.target_position])
            BattleManager.Inst.NowHp[attackInfo.target_position] = BattleManager.Inst.MaxHp[attackInfo.target_position];
        
        if (attackInfo.damage < 10)
        {
            testInfo[Index].image[1].gameObject.SetActive(false);
            testInfo[Index].image[2].gameObject.SetActive(false);
            testInfo[Index].image[3].gameObject.SetActive(false);
            testInfo[Index].image[4].gameObject.SetActive(false);
            textPool[Index].GetComponent<RectTransform>().position += new Vector3(-66.0f, 0, 0);
        }
        else if (attackInfo.damage < 100)
        {
            testInfo[Index].image[2].gameObject.SetActive(false);
            testInfo[Index].image[3].gameObject.SetActive(false);
            testInfo[Index].image[4].gameObject.SetActive(false);
            textPool[Index].GetComponent<RectTransform>().position += new Vector3(-49.5f, 0, 0);
        }
        else if (attackInfo.damage < 1000)
        {
            testInfo[Index].image[3].gameObject.SetActive(false);
            testInfo[Index].image[4].gameObject.SetActive(false);
            textPool[Index].GetComponent<RectTransform>().position += new Vector3(-33.0f, 0, 0);
        }
        else if (attackInfo.damage < 10000)
        {
            testInfo[Index].image[4].gameObject.SetActive(false);
            textPool[Index].GetComponent<RectTransform>().position += new Vector3(-16.5f, 0, 0);
        }

        if (!isHeal)
        {
            if (!attackInfo.critical)
                StartCoroutine(NotCriticalAttackEffect());
            else
                StartCoroutine(CriticalAttackEffect());
        }

        Index++;
        if (Index >= 50)
            Index = 0;
    }

    IEnumerator NotCriticalAttackEffect()
    {
        Vector2 temp = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        temp = temp.normalized;
    
        for (int i = 0; i < 5; i++)
        {
            camera_.transform.Translate(temp.x * 0.03f, temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 10; i++)
        {
            camera_.transform.Translate(-temp.x * 0.03f, -temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 5; i++)
        {
            camera_.transform.Translate(temp.x * 0.03f, temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
    
    IEnumerator CriticalAttackEffect()
    {
        Vector2 temp = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        temp = temp.normalized;
    
        for (int i = 0; i < 5; i++)
        {
            camera_.transform.Translate(temp.x * 0.09f, temp.y * 0.09f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 10; i++)
        {
            camera_.transform.Translate(-temp.x * 0.09f, -temp.y * 0.09f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 5; i++)
        {
            camera_.transform.Translate(temp.x * 0.09f, temp.y * 0.09f, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
