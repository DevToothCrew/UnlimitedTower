using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAttack : MonoBehaviour {
    public bool isOneDeal;
    public GameObject Arrow;
    public GameObject effect;
    private CharacterInformation characterInformation;
    private Animator ani;

    private void Start()
    {
        effect = BattleSystem.Inst.testEffect;
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
        // 공격자와 타겟의 위치
        Transform attacker;
        Transform target;
        Vector3 attackerStartPos;
        Vector3 attackerEndPos;

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
        attackerStartPos = attacker.transform.position;
        
        attacker.LookAt(target);
        ani.SetTrigger("isRun");

        for (int i = 0; i < 100; i += BattleSystem.Inst.TimeScale)
        {
            attacker.transform.position = Vector3.Lerp(attackerStartPos, target.transform.position, i * 0.0095f);
            yield return new WaitForSeconds(0.015f);
        }

        ani.SetTrigger("isAttack");

        yield return new WaitForSeconds(characterInformation.AttackDelay);

        if (!sendValue.isAvoid)
        {
            // 데미지 텍스트 표시와 데미지 주기
            DamageTextSystem.Inst.DamageShow(sendValue.Target, !sendValue.isPlayer, sendValue.Damage, Random.Range(0,2) == 0 ? true : false);

            // 이펙트 보류
            // if (sendValue.isPlayer)
            //     Instantiate(effect, BattleSystem.Inst.EnemyCharacter[sendValue.Target].transform.position +
            //         new Vector3(0, BattleSystem.Inst.EnemyCharacter[sendValue.Target].transform.GetChild(0).GetComponent<CharacterInformation>().Height * 0.3f, 0), transform.rotation);
            // else
            //     Instantiate(effect, BattleSystem.Inst.PlayerCharacter[sendValue.Target].transform.position +
            //         new Vector3(0, BattleSystem.Inst.PlayerCharacter[sendValue.Target].transform.GetChild(0).GetComponent<CharacterInformation>().Height * 0.3f, 0), transform.rotation);

            target.GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
        }
        else
        {
            // Miss 텍스트
            DamageTextSystem.Inst.Avoid(sendValue.Target, !sendValue.isPlayer);
            if (sendValue.isPlayer)
                BattleSystem.Inst.EnemyCharacterControl[sendValue.Target].Miss();
            else
                BattleSystem.Inst.PlayerCharacterControl[sendValue.Target].Miss();
        }

        yield return new WaitForSeconds(characterInformation.AttackAfterDelay);

        ani.SetTrigger("isRun");

        attacker.Rotate(0, 180, 0);
        attackerEndPos = attacker.transform.position;

        for (int i = 0; i < 100; i += BattleSystem.Inst.TimeScale)
        {
            attacker.transform.position = Vector3.Lerp(attackerEndPos, attackerStartPos, i * 0.01f);
            yield return new WaitForSeconds(0.015f);
        }

        ani.SetTrigger("isIdle");

        if (sendValue.isPlayer)
            attacker.rotation = Quaternion.Euler(0, 0, 0);
        else
            attacker.rotation = Quaternion.Euler(0, 180, 0);

        BattleSystem.Inst.battleInformation.AttackerIndex = -1;
    }

    // 원거리 공격은 보류
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
