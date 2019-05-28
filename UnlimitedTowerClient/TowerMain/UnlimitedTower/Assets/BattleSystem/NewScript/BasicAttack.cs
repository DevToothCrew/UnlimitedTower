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

    public void Attack(characterActionData attackInfo, ELEMENT_TYPE type = ELEMENT_TYPE.None)
    {
        if (charInfo.AttackRange > 0.1f)
            StartCoroutine(NearAttackAction(attackInfo, type));
        else
            StartCoroutine(FarAttackAction(attackInfo));
    }

    IEnumerator NearAttackAction(characterActionData attackInfo, ELEMENT_TYPE type = ELEMENT_TYPE.None)
    {
        Transform attacker = BattleManager.Inst.character[attackInfo.my_position].transform;
        Transform target = BattleManager.Inst.character[attackInfo.action_info_list[0].target_position].transform;
        Vector3 attackerStartPos = attacker.position;
        Vector3 attackerEndPos = target.position;

        yield return StartCoroutine(AttackMove(attacker, target, attackerStartPos, attackerEndPos - (attackerEndPos - attackerStartPos).normalized * charInfo.AttackRange));

        attackerEndPos = attackerStartPos;
        attackerStartPos = attacker.position;

        yield return new WaitForSeconds(charInfo.AttackDelay);

        DamageManager.Inst.DamageShow(attackInfo.action_info_list[0], false, type == ELEMENT_TYPE.None ? (ELEMENT_TYPE)BattleManager.Inst.GetCharState(attackInfo.my_position).elementType : type);
        SoundManager.Inst.SoundPlay(Random.Range(0, 5));

        if (BattleManager.Inst.status[attackInfo.action_info_list[0].target_position].NowHp > 0)
        {
            BattleManager.Inst.animator[attackInfo.action_info_list[0].target_position].SetTrigger("isHit");
        }
        else
        {
            BattleManager.Inst.animator[attackInfo.action_info_list[0].target_position].SetTrigger("isDie");
            BattleManager.Inst.tumbAnimation.DieTumb(attackInfo.action_info_list[0].target_position);
        }
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
        Vector3 distance = attackerEndPos - attackerStartPos;
        for (float i = Vector3.Distance(attackerEndPos, attackerStartPos); i > 0; i -= 0.1f * BattleManager.Inst.TimeScale)
        {
            attacker.position += (distance.normalized / 10 * BattleManager.Inst.TimeScale);// * BattleManager.Inst.TimeScale);
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
        Vector3 distance = attackerStartPos - attackerEndPos;
        for (float i = Vector3.Distance(attackerEndPos, attackerStartPos); i > 0; i -= 0.1f * BattleManager.Inst.TimeScale)
        {
            attacker.position += (distance.normalized / 10 * BattleManager.Inst.TimeScale);// * BattleManager.Inst.TimeScale);
            yield return new WaitForSecondsRealtime(0.015f);
        }
        attacker.transform.position = attackerStartPos;
        ani.SetTrigger("isIdle");
    }

    IEnumerator FarAttackAction(characterActionData attackInfo)
    {
        Transform attacker;
        Transform target;

        attacker = BattleManager.Inst.character[attackInfo.my_position].transform;
        target = BattleManager.Inst.character[attackInfo.action_info_list[0].target_position].transform;

        attacker.transform.LookAt(target);
    
        ani.SetTrigger("isAttack");
    
        yield return new WaitForSeconds(charInfo.AttackDelay);
    
        StartCoroutine(ArrowShot(target.position));
    
        ani.SetTrigger("isIdle");
    
        attacker.rotation = attackInfo.my_position < 10 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    
        yield return new WaitForSeconds(charInfo.AttackAfterDelay);
    
        DamageManager.Inst.DamageShow(attackInfo.action_info_list[0], false, (ELEMENT_TYPE)BattleManager.Inst.GetCharState(attackInfo.my_position).elementType);
        if (BattleManager.Inst.status[attackInfo.action_info_list[0].target_position].NowHp > 0)
            BattleManager.Inst.animator[attackInfo.action_info_list[0].target_position].SetTrigger("isHit");
        else
        {
            BattleManager.Inst.animator[attackInfo.action_info_list[0].target_position].SetTrigger("isDie");
            BattleManager.Inst.tumbAnimation.DieTumb(attackInfo.action_info_list[0].target_position);
        }
        yield return new WaitForSeconds(1.0f);
    
        BattleManager.Inst.isAfterDelay = true;
    }
    
    IEnumerator ArrowShot(Vector3 target)
    {
        GameObject arrow = Instantiate(GetComponent<Bullet>().bullet, transform.position + new Vector3(0, 0.4f, 0), transform.rotation);
        Vector3 startPos = arrow.transform.position;
        Vector3 endPos = target - (target - startPos).normalized * 0.2f;
        
        for (int i = 0; i < 30; i += BattleManager.Inst.TimeScale)
        {
            arrow.transform.position = Vector3.Lerp(startPos, endPos, i * 0.033f);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        Destroy(arrow);
    }
}