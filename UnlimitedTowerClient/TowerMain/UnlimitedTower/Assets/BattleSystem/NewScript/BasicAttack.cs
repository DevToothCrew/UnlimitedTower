﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour {
    public CharInfo charInfo;
    private Animator ani;

    private void Start()
    {
        charInfo = GetComponent<CharInfo>();
        ani = GetComponent<Animator>();
    }

    public void Attack(battleActionInfo attackInfo)
    {
        if (charInfo.AttackRange > 0)
            StartCoroutine(NearAttackAction(attackInfo));
        // else
        //     StartCoroutine(FarAttackAction(attackInfo));
    }

    IEnumerator NearAttackAction(battleActionInfo attackInfo)
    {
        Transform attacker;
        Transform target;
        Vector3 attackerStartPos;
        Vector3 attackerEndPos;

        attacker = BattleManager.Inst.character[attackInfo.my_position].transform;
        target = BattleManager.Inst.character[attackInfo.battle_action_list[0].target_position].transform;

        attackerStartPos = attacker.position;
        attackerEndPos = target.position;

        yield return StartCoroutine(AttackMove(attacker, target, attackerStartPos, attackerEndPos - (attackerEndPos - attackerStartPos).normalized * charInfo.AttackRange));

        attackerEndPos = attackerStartPos;
        attackerStartPos = attacker.position;

        yield return new WaitForSeconds(charInfo.AttackDelay);

        DamageManager.Inst.DamageAciton(attackInfo.battle_action_list[0]);
        BattleManager.Inst.animator[attackInfo.battle_action_list[0].target_position].SetTrigger("isHit");

        yield return new WaitForSeconds(charInfo.AttackAfterDelay);

        yield return AttackRecall(attacker, target, attackerEndPos, attackerStartPos);

        attacker.rotation = attackInfo.my_position < 10 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

        yield return new WaitForSeconds(1.0f);

        BattleManager.Inst.isAfterDelay = true;
    }

    // 공격하러 이동시킴
    IEnumerator AttackMove(Transform attacker, Transform target, Vector3 attackerStartPos, Vector3 attackerEndPos)
    {
        attacker.LookAt(target);
        ani.SetTrigger("isRun");
        for (int i = 0; i < 100; i += BattleManager.Inst.TimeScale)
        {
            attacker.transform.position = Vector3.Lerp(attackerStartPos, attackerEndPos, i * 0.01f);
            yield return new WaitForSecondsRealtime(0.015f);
        }
        attacker.transform.position = attackerEndPos;
        ani.SetTrigger("isAttack");
    }

    // 공격후 제자리로 귀환
    IEnumerator AttackRecall(Transform attacker, Transform target, Vector3 attackerStartPos, Vector3 attackerEndPos)
    {
        attacker.Rotate(0, 180, 0);
        ani.SetTrigger("isRun");
        for (int i = 0; i < 100; i += BattleManager.Inst.TimeScale)
        {
            attacker.transform.position = Vector3.Lerp(attackerEndPos, attackerStartPos, i * 0.01f);
            yield return new WaitForSecondsRealtime(0.015f);
        }
        attacker.transform.position = attackerStartPos;
        ani.SetTrigger("isIdle");
    }

    // IEnumerator FarAttackAction(battleActionInfo attackInfo)
    // {
    //     Transform attacker;
    //     Transform target;
    // 
    //     attacker = BattleManager.Inst.characterObject[attackInfo.my_position].transform;
    //     target = BattleManager.Inst.characterObject[attackInfo.battle_action_list[0].target_position].transform;
    // 
    //     attacker.transform.LookAt(target);
    // 
    //     ani.SetTrigger("isAttack");
    // 
    //     yield return new WaitForSeconds(characterInformation.AttackDelay);
    // 
    //     StartCoroutine(ArrowShot(target.position));
    // 
    //     ani.SetTrigger("isIdle");
    // 
    //     attacker.rotation = attackInfo.my_position < 10 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    // 
    //     yield return new WaitForSeconds(characterInformation.AttackAfterDelay);
    // 
    //     DamageTextSystem.Inst.DamageTextAction(attackInfo.battle_action_list[0]);
    //     BattleManager.Inst.characterControl[attackInfo.battle_action_list[0].target_position].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
    // 
    //     yield return new WaitForSeconds(1.0f);
    // 
    //     BattleManager.Inst.isAttackAfterDelay = true;
    // }
    // 
    // IEnumerator ArrowShot(Vector3 target)
    // {
    //     GameObject arrow = Instantiate(bullet, transform.position + new Vector3(0, 0.4f, 0), transform.rotation);
    //     float Speed = Vector3.Distance(arrow.transform.position, target) * 0.02f;
    //     for (int i = 0; i < 45; i += BattleManager.Inst.TimeScale)
    //     {
    //         arrow.transform.Translate(0, 0, Speed * BattleManager.Inst.TimeScale);
    //         yield return new WaitForSecondsRealtime(0.01f);
    //     }
    //     Destroy(arrow);
    // }

}
