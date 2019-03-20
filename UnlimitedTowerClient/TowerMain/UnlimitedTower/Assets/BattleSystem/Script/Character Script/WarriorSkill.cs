using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorSkill : MonoBehaviour {
    private CharacterInformation characterInformation;
    private Animator ani;

    private void Start()
    {
        ani = GetComponent<Animator>();
        characterInformation = GetComponent<CharacterInformation>();
    }

    public void Skill_1(battleActionInfo attackInfo)
    {
        StartCoroutine(Skill_1_co(attackInfo));
    }

    IEnumerator Skill_1_co(battleActionInfo attackInfo)
    {
        Transform attacker;
        Transform target;
        Vector3 attackerStartPos;
        Vector3 attackerEndPos;

        attacker = BattleSystem.Inst.characterObject[attackInfo.my_position].transform;
        target = BattleSystem.Inst.characterObject[attackInfo.battle_action_list[0].target_position].transform;

        attackerStartPos = attacker.position;
        attackerEndPos = target.position;

        yield return StartCoroutine(AttackMove(attacker, target, attackerStartPos, attackerEndPos - (attackerEndPos - attackerStartPos).normalized * characterInformation.AttackRange));

        attackerEndPos = attackerStartPos;
        attackerStartPos = attacker.position;

        yield return new WaitForSeconds(1.0f);
        
        DamageTextSystem.Inst.DamageTextAction(attackInfo.battle_action_list[0]);
        BattleSystem.Inst.characterControl[attackInfo.battle_action_list[0].target_position].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isHit");

        yield return new WaitForSeconds(0.5f);

        DamageTextSystem.Inst.DamageTextAction(attackInfo.battle_action_list[1]);

        yield return new WaitForSeconds(2.0f);

        yield return AttackRecall(attacker, target, attackerEndPos, attackerStartPos);

        attacker.rotation = attackInfo.my_position < 10 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

        yield return new WaitForSeconds(1.0f);

        BattleSystem.Inst.isAttackAfterDelay = true;
    }

    // 공격하러 이동시킴
    IEnumerator AttackMove(Transform attacker, Transform target, Vector3 attackerStartPos, Vector3 attackerEndPos)
    {
        attacker.LookAt(target);
        ani.SetTrigger("isRun");
        for (int i = 0; i < 100; i += BattleSystem.Inst.TimeScale)
        {
            attacker.transform.position = Vector3.Lerp(attackerStartPos, attackerEndPos, i * 0.01f);
            yield return new WaitForSeconds(0.015f);
        }
        attacker.transform.position = attackerEndPos;
        ani.SetTrigger("isDoubleAttack");
    }

    // 공격후 제자리로 귀환
    IEnumerator AttackRecall(Transform attacker, Transform target, Vector3 attackerStartPos, Vector3 attackerEndPos)
    {
        attacker.Rotate(0, 180, 0);
        ani.SetTrigger("isRun");
        for (int i = 0; i < 100; i += BattleSystem.Inst.TimeScale)
        {
            attacker.transform.position = Vector3.Lerp(attackerEndPos, attackerStartPos, i * 0.01f);
            yield return new WaitForSeconds(0.015f);
        }
        attacker.transform.position = attackerStartPos;
        ani.SetTrigger("isIdle");
    }
}
