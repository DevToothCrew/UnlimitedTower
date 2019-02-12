using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public GameObject child;
    public BoxCollider checkBox;
    public GameObject select;
    public int index;
    public bool isPlayer;
    public int maxHp;
    public int nowHp;
    public bool isDie;
    private bool isStart = false;
    
    private void FixedUpdate()
    {
        
        // int[] 숫자배열 = new int[10];
        // int maxSum = 0, currentSum = 0;
        // for (int i = 0; i < 숫자배열.Length; i++) {
        //     currentSum = currentSum + 숫자배열[i] > 숫자배열[i] ? currentSum + 숫자배열[i] : 숫자배열[i];
        //     maxSum = currentSum > maxSum ? currentSum : maxSum; }
        
        if (!isStart)
        {
            isStart = true;
            checkBox = GetComponent<BoxCollider>();
            checkBox.enabled = true;
            select = transform.GetChild(0).GetChild(0).gameObject;
            child = transform.GetChild(0).gameObject;
        }
        if (nowHp <= 0)
        {
            if (!isDie)
            {
                // 바꿔라아ㅏㅏㅏㅏㅏㅏㅏ
                child.GetComponent<Animator>().SetTrigger("isDie");
                isDie = true;
            }
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
            if (isPlayer)
            {
                if (BattleSystem.Inst.enemyCharacterControl[sendValue.Target].nowHp - sendValue.Damage <= 0)
                {
                    DieCameraMove.Inst.Test(sendValue.Target, !isPlayer);
                }
            }
            else if (BattleSystem.Inst.playerCharacterControl[sendValue.Target].nowHp - sendValue.Damage <= 0)
            {
                DieCameraMove.Inst.Test(sendValue.Target, !isPlayer);
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
