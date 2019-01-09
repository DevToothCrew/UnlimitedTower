using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_Novice : MonoBehaviour {
    private const float AttackDelay = 0.44f;
    private const float AttackAfterDelay = 1.6f;
    private const float Height = 0.8f;
    private Animator ani;

    private void Start()
    {
        ani = GetComponent<Animator>();
        GetComponent<CharacterInformation>().InputCharacterInformation(AttackDelay, AttackAfterDelay, Height);
    }

    public void Attack(SendValue sendValue)
    {
        StartCoroutine(AttackAction(sendValue));
    }

    IEnumerator AttackAction(SendValue sendValue)
    {
        Transform attacker;
        Transform target;

        if (sendValue.isPlayer)
        {
            attacker = BattleSystem.Inst.PlayerCharacter[sendValue.Attacker].transform;
            target = BattleSystem.Inst.EnemyCharacter[sendValue.Target].transform;
        }
        else
        {
            attacker = BattleSystem.Inst.EnemyCharacter[sendValue.Attacker].transform;
            target = BattleSystem.Inst.PlayerCharacter[sendValue.Target].transform;
        }

        float MoveSpeed = (Vector3.Distance(attacker.position, target.position) - 0.5f) * 0.01f;
        attacker.LookAt(target);
        ani.SetTrigger("isRun");

        for (int i = 0; i < 100; i += BattleSystem.Inst.TimeScale)
        {
            attacker.Translate(0, 0, MoveSpeed * BattleSystem.Inst.TimeScale);
            yield return new WaitForSeconds(0.015f);
        }

        ani.SetTrigger("isAttack");

        yield return new WaitForSeconds(AttackDelay);

        DamageTextSystem.Inst.DamageShow(sendValue.Target, !sendValue.isPlayer, 10, false);

        target.GetChild(0).GetComponent<Animator>().SetTrigger("isHit");

        yield return new WaitForSeconds(AttackAfterDelay);

        ani.SetTrigger("isRun");

        attacker.Rotate(0, 180, 0);

        for (int i = 0; i < 100; i += BattleSystem.Inst.TimeScale)
        {
            attacker.Translate(0, 0, MoveSpeed * BattleSystem.Inst.TimeScale);
            yield return new WaitForSeconds(0.015f);
        }

        ani.SetTrigger("isIdle");

        if (sendValue.isPlayer)
            attacker.rotation = Quaternion.Euler(0, 0, 0);
        else
            attacker.rotation = Quaternion.Euler(0, 180, 0);
    }
}
