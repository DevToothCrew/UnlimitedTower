using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_Archer : MonoBehaviour {
    private const float AttackDelay = 2.0f;
    private const float AttackAfterDelay = 2.0f;
    private const float Height = 0.8f;
    public GameObject Arrow;

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

        attacker.transform.LookAt(target);

        ani.SetTrigger("isAttack");

        yield return new WaitForSeconds(AttackDelay);

        StartCoroutine(ArrowShot(target.position));

        ani.SetTrigger("isIdle");

        if (sendValue.isPlayer)
            attacker.rotation = Quaternion.Euler(0, 0, 0);
        else
            attacker.rotation = Quaternion.Euler(0, 180, 0);

        yield return new WaitForSeconds(0.5f);

        DamageTextSystem.Inst.DamageShow(sendValue.Target, sendValue.isPlayer, 10, false);

    }

    IEnumerator ArrowShot(Vector3 target)
    {
        GameObject arrow = Instantiate(Arrow, transform.position + new Vector3(0, 0.4f, 0), transform.rotation);
        float Speed = Vector3.Distance(arrow.transform.position, target) * 0.02f;
        for (int i = 0; i < 50; i++)
        {
            arrow.transform.Translate(0, 0, Speed);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(arrow);
    }
}
