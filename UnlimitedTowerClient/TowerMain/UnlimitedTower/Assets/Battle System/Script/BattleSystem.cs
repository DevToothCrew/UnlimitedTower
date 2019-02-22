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
    
    // test
    public TestbattleStateData testbattleStateData;

    public GameObject[] playerCharacter = new GameObject[10];
    public CharacterControl[] playerCharacterControl = new CharacterControl[10];

    public GameObject[] enemyCharacter = new GameObject[10];
    public CharacterControl[] enemyCharacterControl = new CharacterControl[10];

    public int TimeScale = 1;
    public GameObject testEffect;

    public UTLobbyUIManager UTLobbyUIManager_;
    public readonly int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    // 0~9는 플레이어, 10~19는 적
    public bool[] characterisPlace = new bool[20];

    private CaracterCustom caracterCustom;

    [System.Serializable]
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
        UTLobbyUIManager_ = GameObject.Find("Framework")?.GetComponent<UTLobbyUIManager>();
        caracterCustom = GameObject.Find("CharacterCustomInstance").GetComponent<CaracterCustom>();
        
        playerCharacter[0] = GameObject.Find("CharacterPlayer03").gameObject;
        playerCharacter[1] = GameObject.Find("CharacterPlayer02").gameObject;
        playerCharacter[2] = GameObject.Find("CharacterPlayer04").gameObject;
        playerCharacter[3] = GameObject.Find("CharacterPlayer01").gameObject;
        playerCharacter[4] = GameObject.Find("CharacterPlayer05").gameObject;
        playerCharacter[5] = GameObject.Find("CharacterPlayer08").gameObject;
        playerCharacter[6] = GameObject.Find("CharacterPlayer07").gameObject;
        playerCharacter[7] = GameObject.Find("CharacterPlayer09").gameObject;
        playerCharacter[8] = GameObject.Find("CharacterPlayer06").gameObject;
        playerCharacter[9] = GameObject.Find("CharacterPlayer10").gameObject;

        enemyCharacter[0] = GameObject.Find("CharacterEnemy03").gameObject;
        enemyCharacter[1] = GameObject.Find("CharacterEnemy02").gameObject;
        enemyCharacter[2] = GameObject.Find("CharacterEnemy04").gameObject;
        enemyCharacter[3] = GameObject.Find("CharacterEnemy01").gameObject;
        enemyCharacter[4] = GameObject.Find("CharacterEnemy05").gameObject;
        enemyCharacter[5] = GameObject.Find("CharacterEnemy08").gameObject;
        enemyCharacter[6] = GameObject.Find("CharacterEnemy07").gameObject;
        enemyCharacter[7] = GameObject.Find("CharacterEnemy09").gameObject;
        enemyCharacter[8] = GameObject.Find("CharacterEnemy06").gameObject;
        enemyCharacter[9] = GameObject.Find("CharacterEnemy10").gameObject;
    }

    public void Start()
    {
        prefabList = GetComponent<PrefabList>();
        UTLobbyUIManager_?.StageStart();

        battleInformation.attackerIndex = -1;

        for (int i = 0; i < 20; i++)
        {
            // characterisPlace[i] = testbattleStateData.state_list[i].index == 0 ? false : true;
            characterisPlace[i] = true;
        }

        // 캐릭터 컨트롤 스크립트 추가
        // 나중엔 아래에서 초기화 하는 것들 전부 여기서 초기화
        for (int i = 0; i < 10; i++)
        {
            if (characterisPlace[i] == true)
            {
                playerCharacterControl[i] = playerCharacter[i]?.AddComponent<CharacterControl>();
                playerCharacterControl[i].index = i;
                playerCharacterControl[i].isPlayer = true;
            }
            if (characterisPlace[i + 10] == true)
            {
                enemyCharacterControl[i] = enemyCharacter[i]?.AddComponent<CharacterControl>();
                enemyCharacterControl[i].index = i;
                enemyCharacterControl[i].isPlayer = false;
            }
        }

        // 캐릭터 생성
        for (int i = 0; i < 10; i++)
        {
            if (characterisPlace[i] == true)
            {
                if (i < 5)
                {

                    // 서번트 형태 불러와서 생성
                    // Instantiate(caracterCustom.Create(
                    //     UserDataManager.Inst.servantDic[testbattleStateData.state_list[i].index].jobNum,
                    //     UserDataManager.Inst.servantDic[testbattleStateData.state_list[i].index].headNum,
                    //     UserDataManager.Inst.servantDic[testbattleStateData.state_list[i].index].hairNum,
                    //     UserDataManager.Inst.servantDic[testbattleStateData.state_list[i].index].gender == 1 ? 1 : 0,
                    //     UserDataManager.Inst.servantDic[testbattleStateData.state_list[i].index].body == 1 ? 0 : 1
                    //     ), playerCharacter[i].transform);
                    
                    if (i == 0)
                        Instantiate(prefabList.prefabList[22], playerCharacter[i].transform);
                    else
                    Instantiate(prefabList.prefabList[Random.Range(0,57)], playerCharacter[i].transform);
                    
                }
                else
                {
                    // Instantiate(Resources.Load("InGameCharacterPrefabs/" + CharacterCSVData.Inst.monsterDataBaseDic[UserDataManager.Inst.monsterDic[testbattleStateData.state_list[i].index].index].resource) as GameObject, playerCharacter[i].transform);
                    
                    Instantiate(prefabList.prefabList[Random.Range(0, 57)], playerCharacter[i].transform);
                }
            }
            if (characterisPlace[i + 10] == true)
            {
                if (i < 5)
                {
                    // Instantiate(caracterCustom.Create(
                    //     UserDataManager.Inst.servantDic[testbattleStateData.state_list[i].index].jobNum,
                    //     UserDataManager.Inst.servantDic[testbattleStateData.state_list[i].index].headNum,
                    //     UserDataManager.Inst.servantDic[testbattleStateData.state_list[i].index].hairNum,
                    //     UserDataManager.Inst.servantDic[testbattleStateData.state_list[i].index].gender == 1 ? 1 : 0,
                    //     UserDataManager.Inst.servantDic[testbattleStateData.state_list[i].index].body == 1 ? 0 : 1
                    //     ), playerCharacter[i].transform);
                    if (i == 0)
                    Instantiate(prefabList.prefabList[30], enemyCharacter[i].transform);
                    else
                    Instantiate(prefabList.prefabList[Random.Range(0, 57)], enemyCharacter[i].transform);
                }
                else
                {
                    // Instantiate(Resources.Load("InGameCharacterPrefabs/" + CharacterCSVData.Inst.monsterDataBaseDic[UserDataManager.Inst.monsterDic[testbattleStateData.state_list[i + 10].index].index].resource) as GameObject, playerCharacter[i].transform);
                    
                    Instantiate(prefabList.prefabList[Random.Range(0, 57)], enemyCharacter[i].transform);
                }
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
        
        // 캐릭터별 체력 설정
        for (int i = 0; i < 10; i++)
        {
            if (characterisPlace[i] == true)
            {
                // playerCharacterControl[i].maxHp = testbattleStateData.state_list[i].now_hp;
                // playerCharacterControl[i].nowHp = testbattleStateData.state_list[i].now_hp;
                playerCharacterControl[i].maxHp = 1000;
                playerCharacterControl[i].nowHp = 1000;
            }
            if (characterisPlace[i + 10] == true)
            {
                // enemyCharacterControl[i].maxHp = testbattleStateData.state_list[i + 10].now_hp;
                // enemyCharacterControl[i].nowHp = testbattleStateData.state_list[i + 10].now_hp;
                enemyCharacterControl[i].maxHp = 1000;
                enemyCharacterControl[i].nowHp = 1000;
            }
        }
    }

    [ContextMenu("AttackTest")]
    public void AttackTest()
    {
        StartCoroutine(BattleTestTarget());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackTest();
        }
    }

    // 0번과 1번이 번갈아 공격
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

    // 1번부터 10번까지 번갈아 가며 공격
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

    // 랜덤하게 번갈아가며 공격
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

    // 배틀데이터를 받아와 공격 ( 메인 배틀 )
    IEnumerator BattleStart()
    {
        for (int i = 0; i < battleData.Count; i++)
        {
            for (int j = 0; j < battleData[i].info_list.Count; j++)
            {
                if (battleData[i].info_list[j].index < 10)
                {
                    battleInformation.attackerIndex = battleData[i].info_list[j].index;
                    battleInformation.targetIndex = battleData[i].info_list[j].battle_action_list[0].target_index;
                    battleInformation.damage = battleData[i].info_list[j].battle_action_list[0].damage;
                    battleInformation.isCritical = battleData[i].info_list[j].battle_action_list[0].critical;
                    battleInformation.isAvoid = battleData[i].info_list[j].battle_action_list[0].avoid;
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
                    battleInformation.targetIndex = battleData[i].info_list[j].battle_action_list[0].target_index - 10;
                    battleInformation.damage = battleData[i].info_list[j].battle_action_list[0].damage;
                    battleInformation.isCritical = battleData[i].info_list[j].battle_action_list[0].critical;
                    battleInformation.isAvoid = battleData[i].info_list[j].battle_action_list[0].avoid;
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