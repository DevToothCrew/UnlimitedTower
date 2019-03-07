﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public GameObject child;
    public BoxCollider checkBox;
    public GameObject select;
    public int index;
    public int maxHp;
    public int nowHp;
    public bool isDie;
    private bool isStart = false;

    private void FixedUpdate()
    {
        if (!isStart)
        {
            checkBox = GetComponent<BoxCollider>();
            checkBox.enabled = true;
            select = transform.GetChild(0)?.GetChild(0)?.gameObject;
            child = transform.GetChild(0)?.gameObject;

            if (checkBox != null && select != null && child != null)
                isStart = true;
        }
        if (nowHp <= 0)
        {
            if (!isDie)
            {
                child.GetComponent<Animator>().SetTrigger("isDie");
                isDie = true;
            }
            nowHp = 0;
        }
    }

    private void OnEnable()
    {
        if (transform.childCount != 0)
            select = transform.GetChild(0).GetChild(0).gameObject;
        checkBox = GetComponent<BoxCollider>();
        if (transform.childCount != 0)
            child = transform.GetChild(0).gameObject;
    }

    public void Attack(SendValue sendValue)
    {
        child.SendMessage("Attack", sendValue);

        if (!sendValue.isAvoid)
        {
            if (BattleSystem.Inst.characterControl[sendValue.Target].nowHp - sendValue.Damage <= 0)
            {
                DieCameraMove.Inst.Test(sendValue.Target);
            }
        }
    }

    public void Miss()
    {
        StartCoroutine(Avoid());
    }

    // 바꿔라아ㅏㅏ
    IEnumerator Avoid()
    {
        for (int i = 0; i < 25; i += BattleSystem.Inst.TimeScale)
        {
            child.transform.Translate(0, 0, -0.03f * BattleSystem.Inst.TimeScale);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 25; i += BattleSystem.Inst.TimeScale)
        {
            child.transform.Translate(0, 0, 0.03f * BattleSystem.Inst.TimeScale);
            yield return new WaitForSeconds(0.01f);
        }
    }
}