using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoSingleton<BattleSystem>
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

    public int TimeScale = 1;

    private int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    private void Awake()
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
            PlayerCharacterControl[i].index = i;
            EnemyCharacterControl[i].index = i;
            PlayerCharacterControl[i].isPlayer = true;
            EnemyCharacterControl[i].isPlayer = false;
        }
    }


    [ContextMenu("AttackTest2")]
    public void DamageShow()
    {
        DamageTextSystem.Inst.DamageShow(2, false, 653, true);
    }

    [ContextMenu("AttackTest")]
    public void AttackTest()
    {
        StartCoroutine(BattleStart());
    }

    [ContextMenu("AttackTest2")]
    public void AttackTest2()
    {
        PlayerCharacterControl[2].Attack(new SendValue(2, Random.Range(0,10), true));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackTest();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            AttackTest2();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Time.timeScale = 1;
            TimeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Time.timeScale = 2;
            TimeScale = 2;
        }
    }

    IEnumerator BattleStart()
    {
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
            yield return new WaitForSeconds(7);
            temp1 = Random.Range(0, 10);
            temp2 = Random.Range(0, 10);
            while (EnemyCharacterControl[temp1].NowHp <= 0 || PlayerCharacterControl[temp2].NowHp <= 0)
            {
                temp1 = Random.Range(0, 10);
                temp2 = Random.Range(0, 10);
            }
            EnemyCharacterControl[temp1].Attack(new SendValue(temp1, temp2, false));
            yield return new WaitForSeconds(7);
        }
    }
   
    
}
