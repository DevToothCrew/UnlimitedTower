using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoSingleton<BattleSystem>
{

    public Dictionary<int, UserServantData> servantDic = new Dictionary<int, UserServantData>();
    public Dictionary<int, UserMonsterData> monsterDic = new Dictionary<int, UserMonsterData>();
    public Dictionary<int, Item> itemDic = new Dictionary<int, Item>();
    public Dictionary<int, Party> partyDic = new Dictionary<int, Party>();
    public PrefabList prefabList;
    public BattleInformation battleInformation;

    public UserServantData[] servant = new UserServantData[5];
    public UserMonsterData[] monster = new UserMonsterData[5];
    public Party party = new Party();
    public PartyCharacterInfo temp = new PartyCharacterInfo();

    public GameObject[] PlayerCharacter = new GameObject[10];
    public CharacterControl[] PlayerCharacterControl = new CharacterControl[10];

    public GameObject[] EnemyCharacter = new GameObject[10];
    public CharacterControl[] EnemyCharacterControl = new CharacterControl[10];

    public int TimeScale = 1;
    public GameObject testEffect;

    private int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    public struct BattleInformation
    {
        public int AttackerIndex;
        public int TargetIndex;
        public bool isPlayerTurn;
    }

    private void Awake()
    {
        battleInformation.AttackerIndex = -1;
        Application.targetFrameRate = 60;
        prefabList = GetComponent<PrefabList>();

        for (int i = 0; i < 10; i++)
        {
            if (i > 4)
            Instantiate(prefabList.prefabList[201 + i].Prefab, PlayerCharacter[i].transform);
            Instantiate(prefabList.prefabList[211 + i].Prefab, EnemyCharacter[i].transform);
        }



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
            servant[i] = new UserServantData();
            servant[i].status.basicDex = 3;
            servant[i].status.basicDex = 3;
            servant[i].status.basicDex = 3;
            servantDic.Add(i, servant[i]);
        }

        for (int i = 0; i < 5; i++)
        {
            monster[i] = new UserMonsterData();
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
    
    [ContextMenu("AttackTest")]
    public void AttackTest()
    {
        StartCoroutine(BattleStart());
        //PlayerCharacterControl[0].Attack(new SendValue(0, 3, true));
    }

    private void Update()
    {
        // Debug.Log(UserDataManager.Inst.partyDic[1]);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackTest();
        }
    }

    IEnumerator BattleStart()
    {
        while (true)
        {
            battleInformation.isPlayerTurn = true;
            battleInformation.AttackerIndex = Random.Range(0, 10);
            battleInformation.TargetIndex = Random.Range(0, 10);
            while (PlayerCharacterControl[battleInformation.AttackerIndex].NowHp <= 0 || EnemyCharacterControl[battleInformation.TargetIndex].NowHp <= 0)
            {
                battleInformation.AttackerIndex = Random.Range(0, 10);
                battleInformation.TargetIndex = Random.Range(0, 10);
            }
            PlayerCharacterControl[battleInformation.AttackerIndex].Attack(new SendValue(battleInformation.AttackerIndex, battleInformation.TargetIndex, true, Random.Range(1000, 2000)));
            yield return new WaitForSeconds(7);

            battleInformation.isPlayerTurn = false;
            battleInformation.AttackerIndex = Random.Range(0, 10);
            battleInformation.TargetIndex = Random.Range(0, 10);
            while (EnemyCharacterControl[battleInformation.AttackerIndex].NowHp <= 0 || PlayerCharacterControl[battleInformation.TargetIndex].NowHp <= 0)
            {
                battleInformation.AttackerIndex = Random.Range(0, 10);
                battleInformation.TargetIndex = Random.Range(0, 10);
            }
            EnemyCharacterControl[battleInformation.AttackerIndex].Attack(new SendValue(battleInformation.AttackerIndex, battleInformation.TargetIndex, false, Random.Range(1000, 2000)));
            yield return new WaitForSeconds(7);
        }
    }
}
