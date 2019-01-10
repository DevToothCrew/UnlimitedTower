using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextSystem : MonoSingleton<DamageTextSystem>
{
    public int[] numberIndex = new int[5];
    public Sprite[] numberSprite = new Sprite[10];
    public GameObject[] TextPool = new GameObject[50];
    public DamageText[] chsing = new DamageText[50];

    private int Index = 0;
    private GameObject Camera_;
    private Color RedColor = new Color(1, 0, 0, 1);
    private Color YellowColor = new Color(1, 1, 0, 1);

    private void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            TextPool[i] = transform.GetChild(i).gameObject;
            chsing[i] = TextPool[i].GetComponent<DamageText>();
            TextPool[i].SetActive(false);
        }
        Camera_ = GameObject.Find("Main Camera");
    }

    public void DamageShow(int target, bool isPlayer, int damage, bool isCritical)
    {
        numberIndex[0] = damage % 10;
        numberIndex[1] = (damage % 100 - numberIndex[0]) / 10;
        numberIndex[2] = (damage % 1000 - numberIndex[0] - numberIndex[1] * 10) / 100;
        numberIndex[3] = (damage % 10000 - numberIndex[0] - numberIndex[1] * 10 - numberIndex[2] * 100) / 1000;
        numberIndex[4] = (damage - numberIndex[0] - numberIndex[1] * 10 - numberIndex[2] * 100 - numberIndex[4] * 1000) / 10000;

        TextPool[Index].SetActive(true);
        if (isPlayer)
        {
            TextPool[Index].transform.position =
                  Camera.main.WorldToScreenPoint(BattleSystem.Inst.PlayerCharacter[target].transform.position +
                  new Vector3(0, BattleSystem.Inst.PlayerCharacterControl[target].child.GetComponent<CharacterInformation>().Height, 0));
            BattleSystem.Inst.PlayerCharacterControl[target].NowHp -= damage;
        }
        else
        {
            TextPool[Index].transform.position =
                  Camera.main.WorldToScreenPoint(BattleSystem.Inst.EnemyCharacter[target].transform.position +
                  new Vector3(0, BattleSystem.Inst.EnemyCharacterControl[target].child.GetComponent<CharacterInformation>().Height, 0));
            BattleSystem.Inst.EnemyCharacterControl[target].NowHp -= damage;
        }

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
            TextPool[Index].GetComponent<RectTransform>().position += new Vector3(-66.0f, 0, 0);
        }
        else if (damage < 100)
        {
            chsing[Index].image[2].gameObject.SetActive(false);
            chsing[Index].image[3].gameObject.SetActive(false);
            chsing[Index].image[4].gameObject.SetActive(false);
            TextPool[Index].GetComponent<RectTransform>().position += new Vector3(-49.5f, 0, 0);
        }
        else if (damage < 1000)
        {
            chsing[Index].image[3].gameObject.SetActive(false);
            chsing[Index].image[4].gameObject.SetActive(false);
            TextPool[Index].GetComponent<RectTransform>().position += new Vector3(-33.0f, 0, 0);
        }
        else if (damage < 10000)
        {
            chsing[Index].image[4].gameObject.SetActive(false);
            Debug.Log(TextPool[Index].GetComponent<RectTransform>().position);
            TextPool[Index].GetComponent<RectTransform>().position += new Vector3(-16.5f, 0, 0);
            Debug.Log(TextPool[Index].GetComponent<RectTransform>().position);
        }

        if (isCritical)
            StartCoroutine(CriticalAttackEffect());

        Index++;
    }

    IEnumerator CriticalAttackEffect()
    {
        Vector2 temp = new Vector2(Random.Range(0.0f, 2.0f), Random.Range(0.0f, 2.0f));
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
