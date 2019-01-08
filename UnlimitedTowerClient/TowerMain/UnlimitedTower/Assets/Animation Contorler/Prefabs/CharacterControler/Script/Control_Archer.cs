using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_Archer : MonoBehaviour {
    private const float AttackDelay = 2.0f;
    private const float AttackAfterDelay = 2.0f;
    private const float Height = 0.8f;

    public GameObject Arrow;
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

        // StartCoroutine(TestAttack(EnemyCharacter[target].transform.position + new Vector3(0, PlayerCharacterControl[target].information.Height, 0), false, target));

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
