using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_Zombie: MonoBehaviour {
    private const float AttackDelay = 1.0f;
    private const float AttackAfterDelay = 2.0f;
    private const float Height = 0.8f;
    
    public GameObject select;
    public Collider checkBox;

    private RaycastHit hit;
    private Ray ray;
    private Animator ani;

    private void Start()
    {
        select = transform.GetChild(0).gameObject;
        ani = GetComponent<Animator>();
        checkBox = GetComponent<BoxCollider>();
    }

    public void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && hit.collider.Equals(checkBox))
        {
            select.SetActive(true);
            select.SendMessage("AniStart");
            if (Input.GetMouseButtonDown(0))
            {
                select.SendMessage("Click");
            }
        }
        else
        {
            select.SetActive(false);
        }
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
            attacker = TestBattle.Inst.PlayerCharacter[sendValue.Attacker].transform;
            target = TestBattle.Inst.EnemyCharacter[sendValue.Target].transform;
        }
        else
        {
            attacker = TestBattle.Inst.EnemyCharacter[sendValue.Attacker].transform;
            target = TestBattle.Inst.PlayerCharacter[sendValue.Target].transform;
        }

        float MoveSpeed = (Vector3.Distance(attacker.position, target.position) - 0.5f) * 0.01f;
        attacker.LookAt(target);
        ani.SetTrigger("isRun");

        for (int i = 0; i < 100; i++)
        {
            attacker.Translate(0, 0, MoveSpeed);
            yield return new WaitForSeconds(0.015f);
        }

        ani.SetTrigger("isAttack");

        yield return new WaitForSeconds(AttackDelay);

        // StartCoroutine(TestAttack(EnemyCharacter[target].transform.position + new Vector3(0, EnemyCharacterControl[target].information.Height, 0), false, target)); 데미지 계산

        target.GetChild(0).GetComponent<Animator>().SetTrigger("isHit");

        yield return new WaitForSeconds(AttackAfterDelay);

        ani.SetTrigger("isRun");

        attacker.Rotate(0, 180, 0);

        for (int i = 0; i < 100; i++)
        {
            attacker.Translate(0, 0, MoveSpeed);
            yield return new WaitForSeconds(0.015f);
        }

        ani.SetTrigger("isIdle");

        if (sendValue.isPlayer)
            attacker.rotation = Quaternion.Euler(0, 0, 0);
        else
            attacker.rotation = Quaternion.Euler(0, 180, 0);
    }
}
