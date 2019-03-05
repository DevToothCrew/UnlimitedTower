﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextSystem : MonoSingleton<DamageTextSystem>
{
    public int[] numberIndex = new int[5];
    public Sprite[] numberSprite = new Sprite[10];
    public GameObject[] textPool = new GameObject[50];
    public DamageText[] chsing = new DamageText[50];
    public GameObject missText;

    private int Index = 0;
    private GameObject Camera_;
    private readonly Color RedColor = new Color(1, 0, 0, 1);
    private readonly Color YellowColor = new Color(1, 1, 0, 1);

    private void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            textPool[i] = transform.GetChild(0).GetChild(i).gameObject;
            chsing[i] = textPool[i].GetComponent<DamageText>();
            textPool[i].SetActive(false);
        }
        Camera_ = GameObject.Find("Main Camera");
        missText = GameObject.Find("Miss");
    }

    public void TextAction(SendValue sendValue, Transform target)
    {
        if (!sendValue.isAvoid)
        {
            // 데미지 텍스트 표시와 데미지 주기
            DamageShow(sendValue.Target, sendValue.Damage, sendValue.isCritical);
            target.GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
        }
        else
        {
            // Miss 텍스트
            Avoid(sendValue.Target);
            BattleSystem.Inst.characterControl[sendValue.Target].Miss();
        }
    }

    public void Avoid(int target) // 피한 대상의 인덱스와 플레이어 여부
    {
        if (missText != null)
        {
            missText.transform.GetChild(0).gameObject.SetActive(true);
            missText.transform.position =
                  Camera.main.WorldToScreenPoint(BattleSystem.Inst.characterObject[target].transform.position +
                  new Vector3(0, BattleSystem.Inst.characterControl[target].child.GetComponent<CharacterInformation>().Height, 0));
        }
    }

    public void DamageShow(int target, int damage, bool isCritical) // 맞은 대상의 인덱스와 플레이어 여부, 데미지, 크리티컬 여부
    {
        numberIndex[0] = damage % 10;
        numberIndex[1] = (damage % 100 - numberIndex[0]) / 10;
        numberIndex[2] = (damage % 1000 - numberIndex[0] - numberIndex[1] * 10) / 100;
        numberIndex[3] = (damage % 10000 - numberIndex[0] - numberIndex[1] * 10 - numberIndex[2] * 100) / 1000;
        numberIndex[4] = (damage - numberIndex[0] - numberIndex[1] * 10 - numberIndex[2] * 100 - numberIndex[4] * 1000) / 10000;

        textPool[Index].SetActive(true);
        textPool[Index].transform.position =
              Camera.main.WorldToScreenPoint(BattleSystem.Inst.characterObject[target].transform.position +
              new Vector3(0, BattleSystem.Inst.characterControl[target].child.GetComponent<CharacterInformation>().Height, 0));
        BattleSystem.Inst.characterControl[target].nowHp -= damage;

        for (int i = 0; i < 5; i++)
        {
            chsing[Index].image[i].gameObject.SetActive(true);
            chsing[Index].image[i].color = isCritical ? YellowColor : RedColor;
            chsing[Index].image[i].sprite = numberSprite[numberIndex[i]];
        }

        if (damage < 10)
        {
            chsing[Index].image[1].gameObject.SetActive(false);
            chsing[Index].image[2].gameObject.SetActive(false);
            chsing[Index].image[3].gameObject.SetActive(false);
            chsing[Index].image[4].gameObject.SetActive(false);
            textPool[Index].GetComponent<RectTransform>().position += new Vector3(-66.0f, 0, 0);
        }
        else if (damage < 100)
        {
            chsing[Index].image[2].gameObject.SetActive(false);
            chsing[Index].image[3].gameObject.SetActive(false);
            chsing[Index].image[4].gameObject.SetActive(false);
            textPool[Index].GetComponent<RectTransform>().position += new Vector3(-49.5f, 0, 0);
        }
        else if (damage < 1000)
        {
            chsing[Index].image[3].gameObject.SetActive(false);
            chsing[Index].image[4].gameObject.SetActive(false);
            textPool[Index].GetComponent<RectTransform>().position += new Vector3(-33.0f, 0, 0);
        }
        else if (damage < 10000)
        {
            chsing[Index].image[4].gameObject.SetActive(false);
            textPool[Index].GetComponent<RectTransform>().position += new Vector3(-16.5f, 0, 0);
        }

        if (!isCritical) 
            StartCoroutine(NotCriticalAttackEffect());
        else
            StartCoroutine(CriticalAttackEffect());

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
            Camera_.transform.Translate(temp.x * 0.03f, temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 10; i++)
        {
            Camera_.transform.Translate(-temp.x * 0.03f, -temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 5; i++)
        {
            Camera_.transform.Translate(temp.x * 0.03f, temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator CriticalAttackEffect()
    {
        Vector2 temp = new Vector2(Random.Range(0.0f, 6.0f), Random.Range(0.0f, 6.0f));
        temp = temp.normalized;

        for (int i = 0; i < 5; i++)
        {
            Camera_.transform.Translate(temp.x * 0.03f, temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 10; i++)
        {
            Camera_.transform.Translate(-temp.x * 0.03f, -temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 5; i++)
        {
            Camera_.transform.Translate(temp.x * 0.03f, temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }

}
