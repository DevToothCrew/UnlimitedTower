using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAttack : MonoBehaviour
{
    public bool isOnedeal;
    public GameObject bullet;
    private CharacterInformation characterInformation;
    private Animator ani;
    
    private void Start()
    {
        ani = GetComponent<Animator>();
        characterInformation = GetComponent<CharacterInformation>();
    }

    public void Attack(battleActionInfo attackInfo)
    {
        if (!isOnedeal)
            StartCoroutine(NearAttackAction(attackInfo));
        else 
            StartCoroutine(FarAttackAction(attackInfo));
    }

    IEnumerator NearAttackAction(battleActionInfo attackInfo)
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

        yield return new WaitForSeconds(characterInformation.AttackDelay);

        DamageTextSystem.Inst.DamageTextAction(attackInfo.battle_action_list[0]);

        yield return new WaitForSeconds(characterInformation.AttackAfterDelay);

        yield return AttackRecall(attacker, target, attackerEndPos, attackerStartPos);

        attacker.rotation = attackInfo.my_position < 10 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
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
        ani.SetTrigger("isAttack");
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
    
    IEnumerator FarAttackAction(battleActionInfo attackInfo)
    {
        Transform attacker;
        Transform target;

        attacker = BattleSystem.Inst.characterObject[attackInfo.my_position].transform;
        target = BattleSystem.Inst.characterObject[attackInfo.battle_action_list[0].target_position].transform;

        attacker.transform.LookAt(target);
    
        ani.SetTrigger("isAttack");
    
        yield return new WaitForSeconds(characterInformation.AttackDelay);
    
        StartCoroutine(ArrowShot(target.position));
    
        ani.SetTrigger("isIdle");

        attacker.rotation = attackInfo.my_position < 10 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        
        yield return new WaitForSeconds(characterInformation.AttackAfterDelay);

        DamageTextSystem.Inst.DamageTextAction(attackInfo.battle_action_list[0]);
    }
    
    IEnumerator ArrowShot(Vector3 target)
    {
        GameObject arrow = Instantiate(bullet, transform.position + new Vector3(0, 0.4f, 0), transform.rotation);
        float Speed = Vector3.Distance(arrow.transform.position, target) * 0.02f;
        for (int i = 0; i < 45; i += BattleSystem.Inst.TimeScale)
        {
            arrow.transform.Translate(0, 0, Speed * BattleSystem.Inst.TimeScale);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(arrow);
    }
}
