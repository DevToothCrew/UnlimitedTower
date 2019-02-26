using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoSingleton<BattleSystem>
{
    /*
     *  0번 살아있음
     *  1번 죽음
     *  2번 공격
     *  3번 방어
     * **/

    public PrefabList prefabList;
    public BattleInformation battleInformation;
    // public StageStateData stageStateData;

    // test
    // public TestbattleStateData testbattleStateData;

    public GameObject[] playerCharacter = new GameObject[10];
    public CharacterControl[] playerCharacterControl = new CharacterControl[10];

    public GameObject[] enemyCharacter = new GameObject[10];
    public CharacterControl[] enemyCharacterControl = new CharacterControl[10];

    public int TimeScale = 1;
    public readonly int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    // 0~9는 플레이어, 10~19는 적
    public bool[] characterisPlace = new bool[20];

    private CaracterCustom caracterCustom;

    // Test
    public int turn;
    public TestStageStateData testStageStateData;
    public TestStageActionInfoData testStageActionInfoData;
    public TestStageRewardData testStageRewardData;

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

        testStageStateData = UserDataManager.Inst.GetStageState();
        testStageActionInfoData = UserDataManager.Inst.GetStageAction();
        turn = testStageActionInfoData.turn;
        testStageRewardData = UserDataManager.Inst.GetStageReward();

        // testbattleStateData = UserDataManager.Inst.GetStageState();

        battleInformation.attackerIndex = -1;

        for (int i = 0; i < testStageStateData.my_state_list.Count; i++)
        {
            characterisPlace[testStageStateData.my_state_list[i].position] = true;
        }

        for (int i = 0; i < testStageStateData.enemy_state_list.Count; i++)
        {
            characterisPlace[testStageStateData.enemy_state_list[i].position] = true;
        }

        // 캐릭터 컨트롤 스크립트 추가
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
                if (i == 0)
                {
                    // 서번트 형태 불러와서 생성
                    UserServantData heroInfo = UserDataManager.Inst.GetHeroInfo();
                    if (heroInfo == null)
                    {
                        Debug.LogError("버그다");
                        return;
                    }
                    GameObject servant = Instantiate(caracterCustom.Create(
                        heroInfo.jobNum,
                        heroInfo.headNum,
                        heroInfo.hairNum,
                        heroInfo.gender == 1 ? 1 : 0,
                        heroInfo.body == 1 ? 0 : 1
                        ), playerCharacter[i].transform);
                    servant.transform.position = playerCharacter[i].transform.position;
                }
                else if (i < 5)
                {
                    UserServantData servantInfo = UserDataManager.Inst.GetServantInfo(testStageStateData.my_state_list[i].index);
                    if (servantInfo == null)
                    {
                        Debug.LogError("버그다");
                        return;
                    }
                    GameObject servant = Instantiate(caracterCustom.Create(
                        servantInfo.jobNum,
                        servantInfo.headNum,
                        servantInfo.hairNum,
                        servantInfo.gender == 1 ? 1 : 0,
                        servantInfo.body == 1 ? 0 : 1
                        ), playerCharacter[i].transform);
                    servant.transform.position = playerCharacter[i].transform.position;
                }
                else
                {
                    // Debug.Log(CharacterCSVData.Inst.monsterDataDic
                    //     [UserDataManager.Inst.GetMonsterInfo(testStageStateData.my_state_list[i].index).monsterNum].resource);
                    // Instantiate(Resources.Load("InGameCharacterPrefabs/" + CharacterCSVData.Inst.monsterDataDic
                    //     [UserDataManager.Inst.GetMonsterInfo(testStageStateData.my_state_list[i].index).monsterNum].resource) as GameObject, playerCharacter[i].transform);
                    Instantiate(Resources.Load("InGameCharacterPrefabs/" + CharacterCSVData.Inst.monsterDataDic
                        [100001].resource) as GameObject, playerCharacter[i].transform);
                }
            }
            if (characterisPlace[i + 10] == true)
            {
                Instantiate(Resources.Load("InGameCharacterPrefabs/" + CharacterCSVData.Inst.monsterDataDic
                    [testStageStateData.enemy_state_list[i].index].resource) as GameObject, enemyCharacter[i].transform);
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
                playerCharacterControl[i].maxHp = testStageStateData.my_state_list[i].now_hp;
                playerCharacterControl[i].nowHp = testStageStateData.my_state_list[i].now_hp;
            }
            if (characterisPlace[i + 10] == true)
            {
                enemyCharacterControl[i].maxHp = testStageStateData.enemy_state_list[i].now_hp;
                enemyCharacterControl[i].nowHp = testStageStateData.enemy_state_list[i].now_hp;
            }
        }
    }

    [ContextMenu("AttackTest")]
    public void AttackTest()
    {
        StartCoroutine(BattleStart());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PacketManager.Inst.RequestBattleAction(12, 2, 15, 2);
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            PacketManager.Inst.RequestStageResult();
        }
        else if(Input.GetKeyDown(KeyCode.G))
        {
            //나가기 패킷 보내기
        }

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

    // 배틀데이터를 받아와 공격 ( 메인 배틀 한턴 )
    public IEnumerator BattleStart()
    {
        for (int i = 0; i < testStageActionInfoData.battle_info_list.Count; i++)
        {
            if (testStageActionInfoData.battle_info_list[i].action_type == 2)
            {
                battleInformation.attackerIndex = testStageActionInfoData.battle_info_list[i].my_position;
                battleInformation.targetIndex = testStageActionInfoData.battle_info_list[i].battle_action_list[0].target_position;
                battleInformation.damage = testStageActionInfoData.battle_info_list[i].battle_action_list[0].damage;
                battleInformation.isCritical = testStageActionInfoData.battle_info_list[i].battle_action_list[0].critical;
                battleInformation.isAvoid = testStageActionInfoData.battle_info_list[i].battle_action_list[0].avoid;

                if (battleInformation.attackerIndex < 10)
                {
                    battleInformation.isPlayerTurn = true;
                    playerCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
                            battleInformation.attackerIndex,
                            battleInformation.targetIndex - 10,
                            battleInformation.damage,
                            battleInformation.isCritical,
                            battleInformation.isAvoid,
                            battleInformation.isPlayerTurn));
                }
                else
                {
                    battleInformation.isPlayerTurn = false;
                    enemyCharacterControl[battleInformation.attackerIndex-10].Attack(new SendValue(
                             battleInformation.attackerIndex-10,
                             battleInformation.targetIndex,
                             battleInformation.damage,
                             battleInformation.isCritical,
                             battleInformation.isAvoid,
                             battleInformation.isPlayerTurn));
                }
                yield return new WaitForSeconds(7);
            }
            else
            {
                Debug.Log("막기");
            }
        }
    }
}