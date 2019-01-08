using System.Collections;
using UnityEngine;

public class AttackNear : MonoBehaviour
{
    public void Attack(int Attacker, int Target, Animator Ani, float AttackDelay, float AttackAfterDelay, bool isPlayer)
    {
        StartCoroutine(AttackAction(Attacker, Target, Ani, AttackDelay, AttackAfterDelay, isPlayer));
    }

    public IEnumerator AttackAction(int Attacker, int Target, Animator Ani, float AttackDelay, float AttackAfterDelay, bool isPlayer)
    {
        Transform attacker;
        Transform target;

        if (isPlayer)
        {
            attacker = TestBattle.Inst.PlayerCharacter[Attacker].transform;
            target = TestBattle.Inst.EnemyCharacter[Target].transform;
        }
        else
        {
            attacker = TestBattle.Inst.EnemyCharacter[Target].transform;
            target = TestBattle.Inst.PlayerCharacter[Attacker].transform;
        }

        float MoveSpeed = (Vector3.Distance(attacker.position, target.position) - 0.5f) * 0.01f;
        attacker.LookAt(target);
        Ani.SetTrigger("isRun");

        for (int i = 0; i < 100; i++)
        {
            attacker.Translate(0, 0, MoveSpeed);
            yield return new WaitForSeconds(0.015f);
        }

        Ani.SetTrigger("isAttack");

        yield return new WaitForSeconds(AttackDelay);

        // StartCoroutine(TestAttack(EnemyCharacter[target].transform.position + new Vector3(0, EnemyCharacterControl[target].information.Height, 0), false, target));

        Ani.SetTrigger("isHit");

        yield return new WaitForSeconds(AttackAfterDelay);

        Ani.SetTrigger("isRun");

        attacker.Rotate(0, 180, 0);

        for (int i = 0; i < 100; i++)
        {
            attacker.Translate(0, 0, MoveSpeed);
            yield return new WaitForSeconds(0.015f);
        }

        Ani.SetTrigger("isIdle");

        attacker.rotation = Quaternion.Euler(0, 0, 0);
    }
}
