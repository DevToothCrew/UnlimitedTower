using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBattle : MonoSingleton<TestBattle>
{

    public Dictionary<int, Servant> servantDic = new Dictionary<int, Servant>();
    public Dictionary<int, Monster> monsterDic = new Dictionary<int, Monster>();
    public Dictionary<int, Item> itemDic = new Dictionary<int, Item>();
    public Dictionary<int, Party> partyDic = new Dictionary<int, Party>();

    public Servant[] servant = new Servant[5];
    public Monster[] monster = new Monster[5];
    public Party party = new Party();
    public PartyCharacterInfo temp = new PartyCharacterInfo();

    public GameObject[] PlayerCharacter = new GameObject[10];
    public CharacterControl[] PlayerCharacterControl = new CharacterControl[10];

    public GameObject[] EnemyCharacter = new GameObject[10];
    public CharacterControl[] EnemyCharacterControl = new CharacterControl[10];

    public GameObject damage_04;
    public GameObject damage_06;

    public GameObject camera;
    public AudioSource As;
    public AudioSource Boom;

    public GameObject TestSkill;

    private int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    private void Start()
    {
        // 오브젝트 정렬
        // 추후 오브젝트 instance는 Awake에서 한후 정렬

        for (int i = 0; i < 10; i++)
        {
            if (i < 5)
            {
                PlayerCharacter[i].transform.position = new Vector3(-4.2f + 2.1f * (i % 5), 0, -4.2f);
                EnemyCharacter[i].transform.position = new Vector3(4.2f - 2.1f * (i % 5), 0, 4.2f);
            }
            else
            {
                PlayerCharacter[i].transform.position = new Vector3(-4.2f + 2.1f * (i % 5), 0, -2.1f);
                EnemyCharacter[i].transform.position = new Vector3(4.2f - 2.1f * (i % 5), 0, 2.1f);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            servant[i] = new Servant();
            servant[i].status.basicDex = 3;
            servant[i].status.basicDex = 3;
            servant[i].status.basicDex = 3;
            servantDic.Add(i, servant[i]);
        }

        for (int i = 0; i < 5; i++)
        {
            monster[i] = new Monster();
            monster[i].status.basicDex = 2;
            monster[i].status.basicDex = 2;
            monster[i].status.basicDex = 2;
            monsterDic.Add(i, monster[i]);
        }

        for (int i = 0; i < 10; i++)
        {
            temp.index = i;
            temp.partyPosition = positionOrder[i];

            if (i == 0)
                temp.type = CHAR_TYPE.HERO;
            else if (i < 5)
                temp.type = CHAR_TYPE.SERVANT;
            else
                temp.type = CHAR_TYPE.MONSTER;

            party.characterList.Add(i, temp);
            PlayerCharacterControl[i] = PlayerCharacter[i].GetComponent<CharacterControl>();
            EnemyCharacterControl[i] = EnemyCharacter[i].GetComponent<CharacterControl>();
        }


    }

    [ContextMenu("AttackTest")]
    public void AttackTest()
    {
        StartCoroutine(BattleStart());
        // StartCoroutine(AttackArcher(2, 4));
        // StartCoroutine(Magic());
        // PlayerCharacterControl[2].Attack(new SendValue(2, 3, true));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackTest();
        }
    }

    IEnumerator BattleStart()
    {
        // 임시로 대에에에에에충 만든거니 나중에 바꿀것
        /// 둘중 한팀이 전멸하면 팅긴다 조심 

        int temp1;
        int temp2;
        while (true)
        {
            temp1 = Random.Range(0, 10);
            temp2 = Random.Range(0, 10);
            while (PlayerCharacterControl[temp1].NowHp <= 0 || EnemyCharacterControl[temp2].NowHp <= 0)
            {
                temp1 = Random.Range(0, 10);
                temp2 = Random.Range(0, 10);
            }
            PlayerCharacterControl[temp1].Attack(new SendValue(temp1, temp2, true));
            yield return new WaitForSeconds(10);
            temp1 = Random.Range(0, 10);
            temp2 = Random.Range(0, 10);
            while (EnemyCharacterControl[temp1].NowHp <= 0 || PlayerCharacterControl[temp2].NowHp <= 0)
            {
                temp1 = Random.Range(0, 10);
                temp2 = Random.Range(0, 10);
            }
            EnemyCharacterControl[temp1].Attack(new SendValue(temp1, temp2, false));
            yield return new WaitForSeconds(10);
        }
    }
    //  
    //   IEnumerator Attack(int Attacker, int target)
    //   {
    //       float MoveSpeed = (Vector3.Distance(PlayerCharacter[Attacker].transform.position, EnemyCharacter[target].transform.position) - 0.5f) * 0.01f;
    //       PlayerCharacter[Attacker].transform.LookAt(EnemyCharacter[target].transform);
    //       PlayerCharacterControl[Attacker].ani.SetTrigger("isRun");
    //   
    //       for (int i = 0; i < 100; i++)
    //       {
    //           PlayerCharacter[Attacker].transform.Translate(0, 0, MoveSpeed);
    //           yield return new WaitForSeconds(0.015f);
    //       }
    //   
    //       PlayerCharacterControl[Attacker].ani.SetTrigger("isAttack");
    //   
    //       yield return new WaitForSeconds(PlayerCharacterControl[Attacker].information.AttackDelay);
    //   
    //       StartCoroutine(TestAttack(EnemyCharacter[target].transform.position + new Vector3(0, EnemyCharacterControl[target].information.Height, 0), false, target));
    //   
    //       EnemyCharacterControl[target].ani.SetTrigger("isHit");
    //   
    //       yield return new WaitForSeconds(PlayerCharacterControl[Attacker].information.AttackAfterDelay);
    //   
    //       PlayerCharacterControl[Attacker].ani.SetTrigger("isRun");
    //   
    //       PlayerCharacterControl[Attacker].transform.Rotate(0, 180, 0);
    //   
    //       for (int i = 0; i < 100; i++)
    //       {
    //           PlayerCharacter[Attacker].transform.Translate(0, 0, MoveSpeed);
    //           yield return new WaitForSeconds(0.015f);
    //       }
    //   
    //       PlayerCharacterControl[Attacker].ani.SetTrigger("isIdle");
    //   
    //       PlayerCharacter[Attacker].transform.rotation = Quaternion.Euler(0, 0, 0);
    //   }
    //   
    //   IEnumerator Defence(int Attacker, int target)
    //   {
    //       float MoveSpeed = (Vector3.Distance(EnemyCharacter[Attacker].transform.position, PlayerCharacter[target].transform.position) - 0.5f) * 0.01f;
    //       EnemyCharacter[Attacker].transform.LookAt(PlayerCharacter[target].transform);
    //       EnemyCharacterControl[Attacker].ani.SetTrigger("isRun");
    //   
    //       for (int i = 0; i < 100; i++)
    //       {
    //           EnemyCharacter[Attacker].transform.Translate(0, 0, MoveSpeed);
    //           yield return new WaitForSeconds(0.015f);
    //       }
    //   
    //       EnemyCharacterControl[Attacker].ani.SetTrigger("isAttack");
    //   
    //       yield return new WaitForSeconds(EnemyCharacterControl[Attacker].information.AttackDelay);
    //   
    //       StartCoroutine(TestAttack(PlayerCharacter[target].transform.position + new Vector3(0, PlayerCharacterControl[target].information.Height, 0), true, target));
    //   
    //       PlayerCharacterControl[target].ani.SetTrigger("isHit");
    //   
    //       yield return new WaitForSeconds(EnemyCharacterControl[Attacker].information.AttackAfterDelay);
    //   
    //       EnemyCharacterControl[Attacker].ani.SetTrigger("isRun");
    //   
    //       EnemyCharacterControl[Attacker].transform.Rotate(0, 180, 0);
    //   
    //       for (int i = 0; i < 100; i++)
    //       {
    //           EnemyCharacter[Attacker].transform.Translate(0, 0, MoveSpeed);
    //           yield return new WaitForSeconds(0.015f);
    //       }
    //   
    //       PlayerCharacterControl[Attacker].ani.SetTrigger("isIdle");
    //   
    //       EnemyCharacter[Attacker].transform.rotation = Quaternion.Euler(0, 180, 0);
    //   }
    //   
    //   IEnumerator AttackArcher(int Attacker, int target)
    //   {
    //       PlayerCharacter[Attacker].transform.LookAt(EnemyCharacter[target].transform);
    //   
    //       PlayerCharacterControl[Attacker].ani.SetTrigger("isAttack");
    //   
    //       yield return new WaitForSeconds(PlayerCharacterControl[Attacker].information.AttackDelay);
    //   
    //       StartCoroutine(ArrowShot(PlayerCharacterControl[Attacker], EnemyCharacter[target].transform.position));
    //   
    //       PlayerCharacterControl[Attacker].ani.SetTrigger("isIdle");
    //   
    //       PlayerCharacter[Attacker].transform.rotation = Quaternion.Euler(0, 0, 0);
    //   
    //       yield return new WaitForSeconds(0.5f);
    //   
    //       StartCoroutine(TestAttack(EnemyCharacter[target].transform.position + new Vector3(0, PlayerCharacterControl[target].information.Height, 0), false, target));
    //   }
    //   
    //   IEnumerator DefenceArcher(int Attacker, int target)
    //   {
    //       EnemyCharacter[Attacker].transform.LookAt(PlayerCharacter[target].transform);
    //   
    //       EnemyCharacterControl[Attacker].ani.SetTrigger("isAttack");
    //   
    //       yield return new WaitForSeconds(EnemyCharacterControl[Attacker].information.AttackDelay);
    //   
    //       StartCoroutine(ArrowShot(EnemyCharacterControl[Attacker], PlayerCharacter[target].transform.position));
    //   
    //       EnemyCharacterControl[Attacker].ani.SetTrigger("isIdle");
    //   
    //       EnemyCharacter[Attacker].transform.rotation = Quaternion.Euler(0, 180, 0);
    //   
    //       yield return new WaitForSeconds(0.5f);
    //   
    //       StartCoroutine(TestAttack(PlayerCharacter[target].transform.position + new Vector3(0, PlayerCharacterControl[target].information.Height, 0), false, target));
    //   }
    //   
    //   IEnumerator ArrowShot(CharacterControl temp, Vector3 target)
    //   {
    //       GameObject arrow = Instantiate(temp.information.Arrow, temp.transform.position + new Vector3(0, 0.4f, 0), temp.transform.rotation);
    //       float Speed = Vector3.Distance(arrow.transform.position, target) * 0.02f;
    //       arrow.SetActive(true);
    //       for (int i = 0; i < 50; i++)
    //       {
    //           arrow.transform.Translate(0, 0, Speed);
    //           yield return new WaitForSeconds(0.01f);
    //       }
    //       Destroy(arrow);
    //   }
    //   
    //   IEnumerator TestAttack(Vector3 position, bool isPlayer, int target)
    //   {
    //       As.Play();
    //       yield return new WaitForSeconds(0.2f);
    //       if (Random.Range(0, 2) == 0)
    //       {
    //           if (isPlayer)
    //               PlayerCharacterControl[target].NowHp -= 4;
    //           else
    //               EnemyCharacterControl[target].NowHp -= 4;
    //           damage_04.transform.position = Camera.main.WorldToScreenPoint(position);
    //           damage_04.gameObject.SetActive(true);
    //           yield return new WaitForSeconds(1.0f);
    //           damage_04.gameObject.SetActive(false);
    //       }
    //       else
    //       {
    //           StartCoroutine(CriticalAttackEffect());
    //           if (isPlayer)
    //               PlayerCharacterControl[target].NowHp -= 6;
    //           else
    //               EnemyCharacterControl[target].NowHp -= 6;
    //           damage_06.transform.position = Camera.main.WorldToScreenPoint(position);
    //           damage_06.gameObject.SetActive(true);
    //           yield return new WaitForSeconds(1.0f);
    //           damage_06.gameObject.SetActive(false);
    //       }
    //   }
    //   
    //   IEnumerator CriticalAttackEffect()
    //   {
    //       Vector2 temp = new Vector2(Random.Range(0.0f, 2.0f), Random.Range(0.0f, 2.0f));
    //       temp = temp.normalized;
    //   
    //       for (int i = 0; i < 5; i++)
    //       {
    //           camera.transform.Translate(temp.x * 0.03f, temp.y * 0.03f, 0);
    //           yield return new WaitForSeconds(0.01f);
    //       }
    //       for (int i = 0; i < 10; i++)
    //       {
    //           camera.transform.Translate(-temp.x * 0.03f, -temp.y * 0.03f, 0);
    //           yield return new WaitForSeconds(0.01f);
    //       }
    //       for (int i = 0; i < 5; i++)
    //       {
    //           camera.transform.Translate(temp.x * 0.03f, temp.y * 0.03f, 0);
    //           yield return new WaitForSeconds(0.01f);
    //       }
    //   }
    //   
    //   IEnumerator Magic()
    //   {
    //       int temp = 0;
    //       // StartCoroutine(MagicEffect());
    //       for (int i = 0; i < 300; i++)
    //       {
    //           if (temp == 83)
    //           {
    //               StartCoroutine(CriticalAttackEffect());
    //               Boom.Play();
    //           }
    //           else if (temp == 100)
    //           {
    //               StartCoroutine(CriticalAttackEffect());
    //               Boom.Play();
    //           }
    //           else if (temp == 111)
    //           {
    //               StartCoroutine(CriticalAttackEffect());
    //               Boom.Play();
    //           }
    //           else if (temp == 128)
    //           {
    //               StartCoroutine(CriticalAttackEffect());
    //               Boom.Play();
    //           }
    //           else if (temp == 152)
    //           {
    //               StartCoroutine(CriticalAttackEffect());
    //               Boom.Play();
    //           }
    //           else if (temp == 160)
    //           {
    //               StartCoroutine(CriticalAttackEffect());
    //               Boom.Play();
    //           }
    //           else if (temp == 190)
    //           {
    //               StartCoroutine(CriticalAttackEffect());
    //               Boom.Play();
    //           }
    //           else if (temp == 209)
    //           {
    //               StartCoroutine(CriticalAttackEffect());
    //               Boom.Play();
    //           }
    //           else if (temp == 224)
    //           {
    //               StartCoroutine(CriticalAttackEffect());
    //               Boom.Play();
    //           }
    //           else if (temp == 237)
    //           {
    //               StartCoroutine(CriticalAttackEffect());
    //               Boom.Play();
    //           }
    //           else if (temp == 250)
    //           {
    //               StartCoroutine(CriticalAttackEffect());
    //               Boom.Play();
    //           }
    //   
    //           TestSkill.transform.Translate(0.2f, 0, 0);
    //           yield return new WaitForSeconds(0.01f);
    //           temp++;
    //       }
    //   }

    IEnumerator MagicEffect()
    {
        int temp = 0;
        for (int i = 0; i < 30; i++)
        {
            Debug.Log(temp++);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
