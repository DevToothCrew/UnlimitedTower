using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoSingleton<BattleSystem>
{
    public PrefabList prefabList;
    public BattleInformation battleInformation;

    public GameObject[] playerCharacter = new GameObject[10];
    public CharacterControl[] playerCharacterControl = new CharacterControl[10];

    public GameObject[] enemyCharacter = new GameObject[10];
    public CharacterControl[] enemyCharacterControl = new CharacterControl[10];

    public int TimeScale = 1;
    public readonly int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };
    
    public bool[] characterisPlace = new bool[20];

    public TargetSettingInfo targetSettingInfo = new TargetSettingInfo();

    private CaracterCustom caracterCustom;
    private bool isBattleStart;

    // Test
    public int turn;
    public TestStageStateData testStageStateData;
    public TestStageActionInfoData testStageActionInfoData;
    public TestStageRewardData testStageRewardData;

    public GameObject testMyTurn;
    public GameObject testReward;
    
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

    [System.Serializable]
    public class TargetSettingInfo
    {
        public int heroTargetIndex;
        public int heroAction;
        public int monsterTargetIndex;
        public int monsterAction;
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

        testMyTurn = GameObject.Find("마이턴");
        testReward = GameObject.Find("보상");

        testReward.transform.GetChild(0).GetComponent<Text>().text = "sdfsdf\nsdfsdf";

        testMyTurn.SetActive(false);
        testReward.SetActive(false);
    }

    public void Start()
    {
        prefabList = GetComponent<PrefabList>();

        testStageStateData = UserDataManager.Inst.GetStageState();
        testStageActionInfoData = UserDataManager.Inst.GetStageAction();
        turn = testStageActionInfoData.turn;
        testStageRewardData = UserDataManager.Inst.GetStageReward();
        
        battleInformation.attackerIndex = -1;

        IsPlaceCheck();
        SettingScript();
        SettingHero();
        SettingCharacter();
        SettingMonster();
        SettingHp();
        SettingPosition();
        
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
            if (isBattleStart == false)
            {
                PacketManager.Inst.RequestBattleAction(targetSettingInfo.heroTargetIndex, targetSettingInfo.heroAction, targetSettingInfo.monsterTargetIndex, targetSettingInfo.monsterAction);
                isBattleStart = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            //나가기 패킷 보내기
        }
    }

    #region 테스트용

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

    #endregion

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
                    battleInformation.attackerIndex = testStageActionInfoData.battle_info_list[i].my_position;
                    battleInformation.targetIndex = testStageActionInfoData.battle_info_list[i].battle_action_list[0].target_position - 10;
                    battleInformation.damage = testStageActionInfoData.battle_info_list[i].battle_action_list[0].damage;
                    battleInformation.isCritical = testStageActionInfoData.battle_info_list[i].battle_action_list[0].critical;
                    battleInformation.isAvoid = testStageActionInfoData.battle_info_list[i].battle_action_list[0].avoid;
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
                    battleInformation.attackerIndex = testStageActionInfoData.battle_info_list[i].my_position - 10;
                    battleInformation.targetIndex = testStageActionInfoData.battle_info_list[i].battle_action_list[0].target_position;
                    battleInformation.damage = testStageActionInfoData.battle_info_list[i].battle_action_list[0].damage;
                    battleInformation.isCritical = testStageActionInfoData.battle_info_list[i].battle_action_list[0].critical;
                    battleInformation.isAvoid = testStageActionInfoData.battle_info_list[i].battle_action_list[0].avoid;
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
            else
            {
                Debug.Log("막기");
            }
        }
        if (testStageActionInfoData.turn == 0)
        {
            PacketManager.Inst.RequestStageResult();
            string temp = "";
            testReward.SetActive(true);
            temp += "User Name : " + testStageRewardData.user + "\n";
            temp += "Reward Money : " + testStageRewardData.reward_money.ToString() + "\nExp";
            for (int i = 0; i < testStageRewardData.get_exp_list.Count; i++)
            temp += " : " + testStageRewardData.get_exp_list[i].ToString();
            temp += "\nServant";
            for (int i = 0; i < testStageRewardData.get_servant_list.Count; i++)
                temp += " : " + testStageRewardData.get_servant_list[i].name;
            temp += "\nMonster";
            for (int i = 0; i < testStageRewardData.get_monster_list.Count; i++)
                temp += " : " + testStageRewardData.get_monster_list[i].name;
            temp += "\nItem";
            for (int i = 0; i < testStageRewardData.get_item_list.Count; i++)
                temp += " : " + testStageRewardData.get_item_list[i].itemnum;
            testReward.transform.GetChild(0).GetComponent<Text>().text = temp;
        }
        else
        {
            StartCoroutine(TestMyTurn());
        }
    }
    
    // 캐릭터 존재 여부 체크
    public void IsPlaceCheck()
    {
        for (int i = 0; i < testStageStateData.my_state_list.Count; i++)
        {
            characterisPlace[testStageStateData.my_state_list[i].position] = true;
        }

        for (int i = 0; i < testStageStateData.enemy_state_list.Count; i++)
        {
            characterisPlace[testStageStateData.enemy_state_list[i].position] = true;
        }
    }

    // 모든 캐릭터 스크립트 생성 
    public void SettingScript()
    {
        for (int i = 0; i < testStageStateData.my_state_list.Count; i++)
        {
            Debug.Log(testStageStateData.my_state_list[i].position);
            playerCharacterControl[testStageStateData.my_state_list[i].position] = playerCharacter[testStageStateData.my_state_list[i].position]?.AddComponent<CharacterControl>();
            playerCharacterControl[testStageStateData.my_state_list[i].position].index = testStageStateData.my_state_list[i].position;
            playerCharacterControl[testStageStateData.my_state_list[i].position].isPlayer = true;
        }

        for (int i = 0; i < testStageStateData.enemy_state_list.Count; i++)
        {
            enemyCharacterControl[testStageStateData.enemy_state_list[i].position - 10] = enemyCharacter[testStageStateData.enemy_state_list[i].position - 10]?.AddComponent<CharacterControl>();
            enemyCharacterControl[testStageStateData.enemy_state_list[i].position - 10].index = testStageStateData.enemy_state_list[i].position - 10;
            enemyCharacterControl[testStageStateData.enemy_state_list[i].position - 10].isPlayer = false;
        }
    }
    
    // 히어로 셋팅
    public void SettingHero()
    {
        UserServantData heroInfo = UserDataManager.Inst.GetHeroInfo();
        if (heroInfo == null)
        {
            Debug.LogError("버그다");
            return;
        }
        GameObject servant = Instantiate(caracterCustom.Create(
            heroInfo.jobNum,
            heroInfo.headNum - 1,
            heroInfo.hairNum - 1,
            heroInfo.gender == 1 ? 1 : 0,
            heroInfo.body == 1 ? 0 : 1
            ), playerCharacter[0].transform);
        servant.transform.position = playerCharacter[0].transform.position;
    }

    // 히어로를 제외한 파티 셋팅
    public void SettingCharacter()
    {
        for (int i = 1; i < testStageStateData.my_state_list.Count; i++)
        {
            if (testStageStateData.my_state_list[i].position < 5)
            {
                UserServantData servantInfo = UserDataManager.Inst.GetServantInfo(testStageStateData.my_state_list[i].index);
                if (servantInfo == null)
                {
                    Debug.LogError(testStageStateData.my_state_list[i].index);
                    Debug.LogError("버그다");
                    return;
                }
                GameObject servant = Instantiate(caracterCustom.Create(
                    servantInfo.jobNum,
                    servantInfo.headNum - 1,
                    servantInfo.hairNum - 1,
                    servantInfo.gender == 1 ? 1 : 0,
                    servantInfo.body == 1 ? 0 : 1
                    ), playerCharacter[testStageStateData.my_state_list[i].position].transform);
                servant.transform.position = playerCharacter[testStageStateData.my_state_list[i].position].transform.position;
            }
            else
            {
                Instantiate(Resources.Load("InGameCharacterPrefabs/" + CharacterCSVData.Inst.monsterDataDic
                    [UserDataManager.Inst.GetMonsterInfo(testStageStateData.my_state_list[i].index).monsterNum].resource) as GameObject, 
                    playerCharacter[testStageStateData.my_state_list[i].position].transform);
            }
        }
    }

    // 몬스터 정보 셋팅
    public void SettingMonster()
    {
        for (int i = 0; i < testStageStateData.enemy_state_list.Count; i++)
        {
            Instantiate(Resources.Load("InGameCharacterPrefabs/" + CharacterCSVData.Inst.monsterDataDic
                    [testStageStateData.enemy_state_list[i].index].resource) as GameObject, 
                    enemyCharacter[testStageStateData.enemy_state_list[i].position - 10].transform);
        }
    }

    // 캐릭터 위치 배치
    public void SettingPosition()
    {
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
    }

    // 캐릭터별 체력 설정
    public void SettingHp()
    {
        for (int i = 0; i < testStageStateData.my_state_list.Count; i++)
        {
            playerCharacterControl[testStageStateData.my_state_list[i].position].maxHp = testStageStateData.my_state_list[i].now_hp;
            playerCharacterControl[testStageStateData.my_state_list[i].position].nowHp = testStageStateData.my_state_list[i].now_hp;
        }

        for (int i = 0; i < testStageStateData.enemy_state_list.Count; i++)
        {
            enemyCharacterControl[testStageStateData.enemy_state_list[i].position - 10].maxHp = testStageStateData.enemy_state_list[i].now_hp;
            enemyCharacterControl[testStageStateData.enemy_state_list[i].position - 10].nowHp = testStageStateData.enemy_state_list[i].now_hp;
        }
    }

    public TestStageState GetMonster(int position)
    {
        for (int i = 0; i < testStageStateData.enemy_state_list.Count; i++)
        {
            if (testStageStateData.enemy_state_list[i].position == position)
            {
                return testStageStateData.enemy_state_list[i];
            }
        }
        Debug.LogError(position + "번째 몬스터의 정보가 없는 오류");
        return null;
    }

    public TestStageState GetServant(int position)
    {
        for (int i = 0; i < testStageStateData.my_state_list.Count; i++)
        {
            if (testStageStateData.my_state_list[i].position == position)
            {
                return testStageStateData.my_state_list[i];
            }
        }
        Debug.LogError(position + "번째 서번트의 정보가 없는 오류");
        return null;
    }
    
    IEnumerator TestMyTurn()
    {
        testMyTurn.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        testMyTurn.SetActive(false);
    }
}