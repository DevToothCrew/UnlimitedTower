using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAttack : MonoBehaviour
{
    public bool isOneDeal;
    public GameObject Arrow;
    private CharacterInformation characterInformation;
    private Animator ani;

    private void Start()
    {
        ani = GetComponent<Animator>();
        characterInformation = GetComponent<CharacterInformation>();
    }

    public void Attack(SendValue sendValue)
    {
        // if (isOneDeal)
        //     StartCoroutine(FarAttackAction(sendValue));
        // else
        StartCoroutine(NearAttackAction(sendValue));
    }

    IEnumerator NearAttackAction(SendValue sendValue)
    {
        Transform attacker;
        Transform target;
        Vector3 attackerStartPos;
        Vector3 attackerEndPos;

        attacker = BattleSystem.Inst.characterObject[sendValue.Attacker].transform;
        target = BattleSystem.Inst.characterObject[sendValue.Target].transform;

        attackerStartPos = attacker.position;
        attackerEndPos = target.position;

        yield return StartCoroutine(AttackMove(attacker, target, attackerStartPos, attackerEndPos - (attackerEndPos - attackerStartPos).normalized * characterInformation.AttackRange));

        attackerEndPos = attackerStartPos;
        attackerStartPos = attacker.position;

        yield return new WaitForSeconds(characterInformation.AttackDelay);

        DamageTextSystem.Inst.TextAction(sendValue, target);

        yield return new WaitForSeconds(characterInformation.AttackAfterDelay);

        yield return AttackRecall(attacker, target, attackerEndPos, attackerStartPos);

        attacker.rotation = sendValue.Attacker < 10 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

        BattleSystem.Inst.battleInformation.attackerIndex = -1;
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
        ani.SetTrigger("isIdle");
    }












    // 원거리 공격은 보류
    // IEnumerator FarAttackAction(SendValue sendValue)
    // {
    //     Transform attacker;
    //     Transform target;
    // 
    //     if (sendValue.isPlayer)
    //     {
    //         attacker = BattleSystem.Inst.playerCharacter[sendValue.Attacker].transform;
    //         target = BattleSystem.Inst.enemyCharacter[sendValue.Target].transform;
    //     }
    //     else
    //     {
    //         attacker = BattleSystem.Inst.enemyCharacter[sendValue.Attacker].transform;
    //         target = BattleSystem.Inst.playerCharacter[sendValue.Target].transform;
    //     }
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
    //     if (sendValue.isPlayer)
    //         attacker.rotation = Quaternion.Euler(0, 0, 0);
    //     else
    //         attacker.rotation = Quaternion.Euler(0, 180, 0);
    // 
    //     yield return new WaitForSeconds(0.7f);
    // 
    //     DamageTextSystem.Inst.DamageShow(sendValue.Target, !sendValue.isPlayer, 10, false);
    // }
    // 
    // IEnumerator ArrowShot(Vector3 target)
    // {
    //     GameObject arrow = Instantiate(Arrow, transform.position + new Vector3(0, 0.4f, 0), transform.rotation);
    //     float Speed = Vector3.Distance(arrow.transform.position, target) * 0.02f;
    //     for (int i = 0; i < 50; i += BattleSystem.Inst.TimeScale)
    //     {
    //         arrow.transform.Translate(0, 0, Speed * BattleSystem.Inst.TimeScale);
    //         yield return new WaitForSeconds(0.01f);
    //     }
    //     Destroy(arrow);
    // }
}
