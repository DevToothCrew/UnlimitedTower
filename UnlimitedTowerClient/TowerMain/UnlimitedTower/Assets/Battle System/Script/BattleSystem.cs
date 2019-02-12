using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoSingleton<BattleSystem>
{
    public PrefabList prefabList;
    public BattleInformation battleInformation;
    public StageStateData stageStateData;

    public List<BattleActionData> battleData;

    public GameObject[] playerCharacter = new GameObject[10];
    public CharacterControl[] playerCharacterControl = new CharacterControl[10];

    public GameObject[] enemyCharacter = new GameObject[10];
    public CharacterControl[] enemyCharacterControl = new CharacterControl[10];

    public int TimeScale = 1;
    public GameObject testEffect;

    public UTLobbyUIManager UTLobbyUIManager_;
    public readonly int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    // 0~9는 플레이어, 10~19는 적
    public bool[] characterisVoid = new bool[20];

    public struct BattleInformation
    {
        public int attackerIndex;
        public int targetIndex;
        public int damage;
        public bool isAvoid;
        public bool isCritical;
        public bool isPlayerTurn;
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        UTLobbyUIManager_ = GameObject.Find("Framework").GetComponent<UTLobbyUIManager>();

        characterisVoid[3] = true;
        characterisVoid[0] = true;
        characterisVoid[4] = true;
        characterisVoid[8] = true;
        characterisVoid[5] = true;
        characterisVoid[9] = true;
        characterisVoid[11] = true;
        characterisVoid[12] = true;
        characterisVoid[16] = true;
        characterisVoid[17] = true;
    }

    public void Start()
    {
        UTLobbyUIManager_.StageStart();

        battleInformation.attackerIndex = -1;
        prefabList = GetComponent<PrefabList>();

        // 캐릭터 컨트롤 스크립트 추가
        for (int i = 0; i < 10; i++)
        {
            if (characterisVoid[i] == true)
                playerCharacter[i].AddComponent<CharacterControl>();
            if (characterisVoid[i + 10] == true)
                enemyCharacter[i].AddComponent<CharacterControl>();
        }

        // 캐릭터 인덱스를 받아와 playerCharacter 오브젝트를 부모로 소환
        for (int i = 0; i < 10; i++)
        {
            if (characterisVoid[i] == true)
            {
                Instantiate(prefabList.prefabList[stageStateData.info_list[i].index].Prefab, playerCharacter[i].transform);
            }
            if (characterisVoid[i + 10] == true)
            {
                Instantiate(prefabList.prefabList[stageStateData.info_list[i].index].Prefab, enemyCharacter[i].transform);
            }
        }
        
        // 캐릭터 위치 배치
        for (int i = 0; i < 10; i++)
        {
            if (i < 5)
            {
                    playerCharacter[i].transform.position = new Vector3(-4.2f + 2.1f * (positionOrder[i] % 5), 0, -4.2f);
                    enemyCharacter[i].transform.position = new Vector3(4.2f - 2.1f * (positionOrder[i] % 5), 0, 4.2f);
            }
            else
            {
                    playerCharacter[i].transform.position = new Vector3(-4.2f + 2.1f * (positionOrder[i] % 5), 0, -2.1f);
                    enemyCharacter[i].transform.position = new Vector3(4.2f - 2.1f * (positionOrder[i] % 5), 0, 2.1f);
            }
        }

        // 캐릭터별 스크립트 캐싱
        for (int i = 0; i < 10; i++)
        {
            if (characterisVoid[i] == true)
            {
                playerCharacterControl[i] = playerCharacter[i].GetComponent<CharacterControl>();
                playerCharacterControl[i].index = i;
                playerCharacterControl[i].isPlayer = true;
            }
            if (characterisVoid[i + 10] == true)
            {
                enemyCharacterControl[i] = enemyCharacter[i].GetComponent<CharacterControl>();
                enemyCharacterControl[i].index = i;
                enemyCharacterControl[i].isPlayer = false;
            }
        }

        // 캐릭터별 체력 설정
        for (int i = 0; i < 10; i++)
        {
            if (characterisVoid[i] == true)
            {
                playerCharacterControl[i].maxHp = stageStateData.info_list[i].now_hp; // 나중에 최대체력 변경
                playerCharacterControl[i].nowHp = stageStateData.info_list[i].now_hp;
                playerCharacterControl[i].maxHp = 300;
                playerCharacterControl[i].nowHp = 300;
            }
            if (characterisVoid[i + 10] == true)
            {
                enemyCharacterControl[i].maxHp = stageStateData.info_list[i].now_hp;  // 나중에 최대체력 변경
                enemyCharacterControl[i].nowHp = stageStateData.info_list[i].now_hp;
                enemyCharacterControl[i].maxHp = 300;
                enemyCharacterControl[i].nowHp = 300;
            }
        }
    }

    [ContextMenu("AttackTest")]
    public void AttackTest()
    {
        StartCoroutine(BattleTest());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackTest();
        }
    }

    IEnumerator BattleTest()
    {

        battleInformation.attackerIndex = 0;
        battleInformation.targetIndex = 1;
        battleInformation.damage = 10;
        battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
        battleInformation.isAvoid = false;
        battleInformation.isPlayerTurn = true;

        playerCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
            battleInformation.attackerIndex,
            battleInformation.targetIndex,
            battleInformation.damage,
            battleInformation.isCritical,
            battleInformation.isAvoid,
            battleInformation.isPlayerTurn));
        yield return new WaitForSeconds(7);
        battleInformation.attackerIndex = 1;
        battleInformation.targetIndex = 0;
        battleInformation.damage = 10;
        battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
        battleInformation.isAvoid = false;
        battleInformation.isPlayerTurn = false;

        enemyCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
            battleInformation.attackerIndex,
            battleInformation.targetIndex,
            battleInformation.damage,
            battleInformation.isCritical,
            battleInformation.isAvoid,
            battleInformation.isPlayerTurn));
        yield return new WaitForSeconds(7);
    }

    IEnumerator BattleTestTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            battleInformation.attackerIndex = i;
            battleInformation.targetIndex = i;
            battleInformation.damage = 10;
            battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
            battleInformation.isAvoid = false;
            battleInformation.isPlayerTurn = true;

            playerCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
                battleInformation.attackerIndex,
                battleInformation.targetIndex,
                battleInformation.damage,
                battleInformation.isCritical,
                battleInformation.isAvoid,
                battleInformation.isPlayerTurn));
            yield return new WaitForSeconds(7);
            battleInformation.attackerIndex = i;
            battleInformation.targetIndex = i;
            battleInformation.damage = 10;
            battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
            battleInformation.isAvoid = false;
            battleInformation.isPlayerTurn = false;

            enemyCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
                battleInformation.attackerIndex,
                battleInformation.targetIndex,
                battleInformation.damage,
                battleInformation.isCritical,
                battleInformation.isAvoid,
                battleInformation.isPlayerTurn));
            yield return new WaitForSeconds(7);
        }
    }

    IEnumerator BattleTestRandom()
    {
        for (int i = 0; i < 10; i++)
        {
            battleInformation.attackerIndex = Random.Range(0, 10);
            battleInformation.targetIndex = Random.Range(0, 10);
            battleInformation.damage = Random.Range(100, 200);
            battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
            battleInformation.isAvoid = Random.Range(0, 5) == 1 ? true : false;
            battleInformation.isPlayerTurn = true;

            playerCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
                battleInformation.attackerIndex,
                battleInformation.targetIndex,
                battleInformation.damage,
                battleInformation.isCritical,
                battleInformation.isAvoid,
                battleInformation.isPlayerTurn));
            yield return new WaitForSeconds(7);
            {
                battleInformation.attackerIndex = Random.Range(0, 10);
                battleInformation.targetIndex = Random.Range(0, 10);
                battleInformation.damage = Random.Range(100, 200);
                battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
                battleInformation.isAvoid = Random.Range(0, 5) == 1 ? true : false;
                battleInformation.isPlayerTurn = false;

                enemyCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
                    battleInformation.attackerIndex,
                    battleInformation.targetIndex,
                    battleInformation.damage,
                    battleInformation.isCritical,
                    battleInformation.isAvoid,
                    battleInformation.isPlayerTurn));
            }
            yield return new WaitForSeconds(7);
        }
    }

    IEnumerator BattleStart()
    {
        for (int i = 0; i < battleData.Count; i++)
        {
            for (int j = 0; j < battleData[i].info_list.Count; j++)
            {
                if (battleData[i].info_list[j].index < 10)
                {
                    battleInformation.attackerIndex = battleData[i].info_list[j].index;
                    battleInformation.targetIndex = battleData[i].info_list[j].action_list[0].target_index;
                    battleInformation.damage = battleData[i].info_list[j].action_list[0].damage;
                    battleInformation.isCritical = battleData[i].info_list[j].action_list[0].critical;
                    battleInformation.isAvoid = battleData[i].info_list[j].action_list[0].avoid;
                    battleInformation.isPlayerTurn = true;

                    playerCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
                        battleInformation.attackerIndex,
                        battleInformation.targetIndex,
                        battleInformation.damage,
                        battleInformation.isCritical,
                        battleInformation.isAvoid,
                        battleInformation.isPlayerTurn));
                }
                else
                {
                    battleInformation.attackerIndex = battleData[i].info_list[j].index - 10;
                    battleInformation.targetIndex = battleData[i].info_list[j].action_list[0].target_index - 10;
                    battleInformation.damage = battleData[i].info_list[j].action_list[0].damage;
                    battleInformation.isCritical = battleData[i].info_list[j].action_list[0].critical;
                    battleInformation.isAvoid = battleData[i].info_list[j].action_list[0].avoid;
                    battleInformation.isPlayerTurn = false;

                    enemyCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
                        battleInformation.attackerIndex,
                        battleInformation.targetIndex,
                        battleInformation.damage,
                        battleInformation.isCritical,
                        battleInformation.isAvoid,
                        battleInformation.isPlayerTurn));
                }
                yield return new WaitForSeconds(7);
            }
        }
    }
}