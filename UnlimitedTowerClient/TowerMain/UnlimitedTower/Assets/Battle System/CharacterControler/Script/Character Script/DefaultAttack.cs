using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAttack : MonoBehaviour {
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
        if (isOneDeal)
            StartCoroutine(FarAttackAction(sendValue));
        else
            StartCoroutine(NearAttackAction(sendValue));
    }

    IEnumerator NearAttackAction(SendValue sendValue)
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

        yield return new WaitForSeconds(characterInformation.AttackDelay);

        DamageTextSystem.Inst.DamageShow(sendValue.Target, !sendValue.isPlayer, 10, false);

        target.GetChild(0).GetComponent<Animator>().SetTrigger("isHit");

        yield return new WaitForSeconds(characterInformation.AttackAfterDelay);

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

    IEnumerator FarAttackAction(SendValue sendValue)
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

        attacker.transform.LookAt(target);

        ani.SetTrigger("isAttack");

        yield return new WaitForSeconds(characterInformation.AttackDelay);

        StartCoroutine(ArrowShot(target.position));

        ani.SetTrigger("isIdle");

        if (sendValue.isPlayer)
            attacker.rotation = Quaternion.Euler(0, 0, 0);
        else
            attacker.rotation = Quaternion.Euler(0, 180, 0);

        yield return new WaitForSeconds(0.7f);

        DamageTextSystem.Inst.DamageShow(sendValue.Target, !sendValue.isPlayer, 10, false);
    }

    IEnumerator ArrowShot(Vector3 target)
    {
        GameObject arrow = Instantiate(Arrow, transform.position + new Vector3(0, 0.4f, 0), transform.rotation);
        float Speed = Vector3.Distance(arrow.transform.position, target) * 0.02f;
        for (int i = 0; i < 50; i += BattleSystem.Inst.TimeScale)
        {
            arrow.transform.Translate(0, 0, Speed * BattleSystem.Inst.TimeScale);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(arrow);
    }
}
