using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoSingleton<BattleSystem>
{
    [HideInInspector]
    public PrefabList prefabList;
    [HideInInspector]
    public BattleInformation battleInformation;

    public GameObject[] characterObject = new GameObject[20];
    public CharacterControl[] characterControl = new CharacterControl[20];
    public bool[] characterisPlace = new bool[20];
    [HideInInspector]
    public int TimeScale = 1;
    [HideInInspector]
    public readonly int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    public TargetSettingInfo targetSettingInfo = new TargetSettingInfo();

    private CaracterCustom caracterCustom;
    private bool isBattleStart;

    // Test
    private int turn;

    private GameObject testMyTurn;
    private GameObject testReward;
    private GameObject testReTageting;

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

        characterObject[0] = GameObject.Find("CharacterPlayer03").gameObject;
        characterObject[1] = GameObject.Find("CharacterPlayer02").gameObject;
        characterObject[2] = GameObject.Find("CharacterPlayer04").gameObject;
        characterObject[3] = GameObject.Find("CharacterPlayer01").gameObject;
        characterObject[4] = GameObject.Find("CharacterPlayer05").gameObject;
        characterObject[5] = GameObject.Find("CharacterPlayer08").gameObject;
        characterObject[6] = GameObject.Find("CharacterPlayer07").gameObject;
        characterObject[7] = GameObject.Find("CharacterPlayer09").gameObject;
        characterObject[8] = GameObject.Find("CharacterPlayer06").gameObject;
        characterObject[9] = GameObject.Find("CharacterPlayer10").gameObject;

        characterObject[10] = GameObject.Find("CharacterEnemy03").gameObject;
        characterObject[11] = GameObject.Find("CharacterEnemy02").gameObject;
        characterObject[12] = GameObject.Find("CharacterEnemy04").gameObject;
        characterObject[13] = GameObject.Find("CharacterEnemy01").gameObject;
        characterObject[14] = GameObject.Find("CharacterEnemy05").gameObject;
        characterObject[15] = GameObject.Find("CharacterEnemy08").gameObject;
        characterObject[16] = GameObject.Find("CharacterEnemy07").gameObject;
        characterObject[17] = GameObject.Find("CharacterEnemy09").gameObject;
        characterObject[18] = GameObject.Find("CharacterEnemy06").gameObject;
        characterObject[19] = GameObject.Find("CharacterEnemy10").gameObject;

        testMyTurn = GameObject.Find("마이턴");
        testReward = GameObject.Find("보상");
        testReTageting = GameObject.Find("공격대상");

        testMyTurn.SetActive(false);
        testReward.SetActive(false);
        testReTageting.SetActive(false);
    }

    public void Start()
    {
        prefabList = GetComponent<PrefabList>();
        battleInformation.attackerIndex = -1;

        stageStateData stageStateInfo = UserDataManager.Inst.GetStageState();
        if (stageStateInfo == null)
        {
            Debug.LogError("버그 : stageStateInfo is NULL");
            return;
        }

        IsPlaceCheck(stageStateInfo);
        SettingScript(stageStateInfo);
        SettingHero();
        SettingCharacter(stageStateInfo);
        SettingMonster(stageStateInfo);
        SettingHp(stageStateInfo);
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
                if (characterControl[0].nowHp < 0)
                {
                    targetSettingInfo.heroAction = 3;
                }
                if (characterControl[5].nowHp < 0)
                {
                    targetSettingInfo.monsterAction = 3;
                }

                if (((targetSettingInfo.heroAction == 2 && targetSettingInfo.heroTargetIndex > 9) || targetSettingInfo.heroAction == 3) &&
                ((targetSettingInfo.monsterAction == 2 && targetSettingInfo.monsterTargetIndex > 9) || targetSettingInfo.monsterAction == 3))
                {
                    UTUMSProvider.Instance.RequestBattleAction(targetSettingInfo.heroTargetIndex, targetSettingInfo.heroAction, targetSettingInfo.monsterTargetIndex, targetSettingInfo.monsterAction);
                    targetSettingInfo = new TargetSettingInfo();
                    isBattleStart = true;
                }
                else
                    StartCoroutine(TestReTargeting());
            }
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            //나가기 패킷 보내기
            SceneManager.LoadScene("Lobby");
        }
    }

    #region 테스트용

    // 1번부터 10번까지 번갈아 가며 공격
    //IEnumerator BattleTestTarget()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        battleInformation.attackerIndex = i;
    //        battleInformation.targetIndex = i;
    //        battleInformation.damage = 10;
    //        battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
    //        battleInformation.isAvoid = false;
    //        battleInformation.isPlayerTurn = true;

    //        playerCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
    //            battleInformation.attackerIndex,
    //            battleInformation.targetIndex,
    //            battleInformation.damage,
    //            battleInformation.isCritical,
    //            battleInformation.isAvoid,
    //            battleInformation.isPlayerTurn));
    //        yield return new WaitForSeconds(7);
    //        battleInformation.attackerIndex = i;
    //        battleInformation.targetIndex = i;
    //        battleInformation.damage = 10;
    //        battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
    //        battleInformation.isAvoid = false;
    //        battleInformation.isPlayerTurn = false;

    //        enemyCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
    //            battleInformation.attackerIndex,
    //            battleInformation.targetIndex,
    //            battleInformation.damage,
    //            battleInformation.isCritical,
    //            battleInformation.isAvoid,
    //            battleInformation.isPlayerTurn));
    //        yield return new WaitForSeconds(7);
    //    }
    //}

    //// 랜덤하게 번갈아가며 공격
    //IEnumerator BattleTestRandom()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        battleInformation.attackerIndex = Random.Range(0, 10);
    //        battleInformation.targetIndex = Random.Range(0, 10);
    //        battleInformation.damage = Random.Range(100, 200);
    //        battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
    //        battleInformation.isAvoid = Random.Range(0, 5) == 1 ? true : false;
    //        battleInformation.isPlayerTurn = true;

    //        playerCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
    //            battleInformation.attackerIndex,
    //            battleInformation.targetIndex,
    //            battleInformation.damage,
    //            battleInformation.isCritical,
    //            battleInformation.isAvoid,
    //            battleInformation.isPlayerTurn));
    //        yield return new WaitForSeconds(7);
    //        {
    //            battleInformation.attackerIndex = Random.Range(0, 10);
    //            battleInformation.targetIndex = Random.Range(0, 10);
    //            battleInformation.damage = Random.Range(100, 200);
    //            battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
    //            battleInformation.isAvoid = Random.Range(0, 5) == 1 ? true : false;
    //            battleInformation.isPlayerTurn = false;

    //            enemyCharacterControl[battleInformation.attackerIndex].Attack(new SendValue(
    //                battleInformation.attackerIndex,
    //                battleInformation.targetIndex,
    //                battleInformation.damage,
    //                battleInformation.isCritical,
    //                battleInformation.isAvoid,
    //                battleInformation.isPlayerTurn));
    //        }
    //        yield return new WaitForSeconds(7);
    //    }
    //}

    #endregion

    // 배틀데이터를 받아와 공격 ( 메인 배틀 한턴 )
    public IEnumerator BattleStart()
    {
        stageActionInfoData stageActionInfo = UserDataManager.Inst.GetStageAction();
        if (stageActionInfo == null)
        {
            Debug.LogError("버그 : stageActionInfo is Null");
            yield break;
        }

        for (int i = 0; i < stageActionInfo.battle_info_list.Count; i++)
        {
            if (stageActionInfo.battle_info_list[i].action_type == 2)
            {
                battleInformation.attackerIndex = stageActionInfo.battle_info_list[i].my_position;
                battleInformation.targetIndex = stageActionInfo.battle_info_list[i].battle_action_list[0].target_position;
                battleInformation.damage = stageActionInfo.battle_info_list[i].battle_action_list[0].damage;
                battleInformation.isCritical = stageActionInfo.battle_info_list[i].battle_action_list[0].critical;
                battleInformation.isAvoid = stageActionInfo.battle_info_list[i].battle_action_list[0].avoid;

                battleInformation.attackerIndex = stageActionInfo.battle_info_list[i].my_position;
                battleInformation.targetIndex = stageActionInfo.battle_info_list[i].battle_action_list[0].target_position;
                battleInformation.damage = stageActionInfo.battle_info_list[i].battle_action_list[0].damage;
                battleInformation.isCritical = stageActionInfo.battle_info_list[i].battle_action_list[0].critical;
                battleInformation.isAvoid = stageActionInfo.battle_info_list[i].battle_action_list[0].avoid;
                characterControl[battleInformation.attackerIndex].Attack(new SendValue(
                        battleInformation.attackerIndex,
                        battleInformation.targetIndex,
                        battleInformation.damage,
                        battleInformation.isCritical,
                        battleInformation.isAvoid));
            }
            else
            {
                Debug.Log("막기");
            }
            yield return new WaitForSeconds(7.0f);
        }

        if (stageActionInfo.turn == 0)
        {
            stageRewardData rewardData = UserDataManager.Inst.GetStageReward();
            if (rewardData == null)
            {
                Debug.LogError("버그 : rewardData is Null");
                yield break;
            }

            PacketManager.Inst.RequestStageResult();
            string temp = "";
            testReward.SetActive(true);
            temp += "User Name : " + rewardData.user + "\n";
            temp += "Reward Money : " + rewardData.reward_money.ToString() + "\nExp";
            for (int i = 0; i < rewardData.get_exp_list.Count; i++)
                temp += " : " + rewardData.get_exp_list[i].ToString();
            temp += "\nServant";
            for (int i = 0; i < rewardData.get_servant_list.Count; i++)
                temp += " : " + rewardData.get_servant_list[i].job;
            temp += "\nMonster";
            for (int i = 0; i < rewardData.get_monster_list.Count; i++)
                temp += " : " + rewardData.get_monster_list[i].id;
            temp += "\nItem";
            for (int i = 0; i < rewardData.get_item_list.Count; i++)
                temp += " : " + rewardData.get_item_list[i].id;
            testReward.transform.GetChild(0).GetComponent<Text>().text = temp;
        }
        else
        {
            StartCoroutine(TestMyTurn());
            isBattleStart = false;
        }
    }

    // 캐릭터 존재 여부 체크
    public void IsPlaceCheck(stageStateData stageStateInfo)
    {
        for (int i = 0; i < stageStateInfo.my_state_list.Count; i++)
        {
            characterisPlace[stageStateInfo.my_state_list[i].position] = true;
        }

        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            characterisPlace[stageStateInfo.enemy_state_list[i].position] = true;
        }
    }

    // 모든 캐릭터 스크립트 생성 
    public void SettingScript(stageStateData stageStateInfo)
    {
        for (int i = 0; i < stageStateInfo.my_state_list.Count; i++)
        {
            characterControl[stageStateInfo.my_state_list[i].position] = characterObject[stageStateInfo.my_state_list[i].position]?.AddComponent<CharacterControl>();
            characterControl[stageStateInfo.my_state_list[i].position].index = stageStateInfo.my_state_list[i].position;
        }

        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            characterControl[stageStateInfo.enemy_state_list[i].position] = characterObject[stageStateInfo.enemy_state_list[i].position]?.AddComponent<CharacterControl>();
            characterControl[stageStateInfo.enemy_state_list[i].position].index = stageStateInfo.enemy_state_list[i].position;
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
            ), characterObject[0].transform);
        servant.transform.position = characterObject[0].transform.position;
    }

    // 히어로를 제외한 파티 셋팅
    public void SettingCharacter(stageStateData stageStateInfo)
    {
        for (int i = 1; i < stageStateInfo.my_state_list.Count; i++)
        {
            if (stageStateInfo.my_state_list[i].position < 5)
            {
                UserServantData servantInfo = UserDataManager.Inst.GetServantInfo(stageStateInfo.my_state_list[i].index);
                if (servantInfo == null)
                {
                    Debug.LogError(stageStateInfo.my_state_list[i].index);
                    Debug.LogError("버그다");
                    return;
                }
                GameObject servant = Instantiate(caracterCustom.Create(
                    servantInfo.jobNum,
                    servantInfo.headNum - 1,
                    servantInfo.hairNum - 1,
                    servantInfo.gender == 1 ? 1 : 0,
                    servantInfo.body == 1 ? 0 : 1
                    ), characterObject[stageStateInfo.my_state_list[i].position].transform);
                servant.transform.position = characterObject[stageStateInfo.my_state_list[i].position].transform.position;
            }
            else
            {
                Instantiate(Resources.Load("InGameCharacterPrefabs/" + CharacterCSVData.Inst.monsterDataDic
                    [UserDataManager.Inst.GetMonsterInfo(stageStateInfo.my_state_list[i].index).id].resource) as GameObject,
                    characterObject[stageStateInfo.my_state_list[i].position].transform);
            }
        }
    }

    // 몬스터 정보 셋팅
    public void SettingMonster(stageStateData stageStateInfo)
    {
        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            Instantiate(Resources.Load("InGameCharacterPrefabs/" + CharacterCSVData.Inst.monsterDataDic
                    [stageStateInfo.enemy_state_list[i].index].resource) as GameObject,
                    characterObject[stageStateInfo.enemy_state_list[i].position].transform);
        }
    }

    // 캐릭터 위치 배치
    public void SettingPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i < 5)
            {
                characterObject[i].transform.position = new Vector3(-4.2f + 2.1f * (positionOrder[i] % 5), 0, -4.2f);
                characterObject[i + 10].transform.position = new Vector3(4.2f - 2.1f * (positionOrder[i] % 5), 0, 4.2f);
            }
            else
            {
                characterObject[i].transform.position = new Vector3(-4.2f + 2.1f * (positionOrder[i] % 5), 0, -2.1f);
                characterObject[i + 10].transform.position = new Vector3(4.2f - 2.1f * (positionOrder[i] % 5), 0, 2.1f);
            }
        }
    }

    // 캐릭터별 체력 설정
    public void SettingHp(stageStateData stageStateInfo)
    {
        for (int i = 0; i < stageStateInfo.my_state_list.Count; i++)
        {
            characterControl[stageStateInfo.my_state_list[i].position].maxHp = stageStateInfo.my_state_list[i].now_hp;
            characterControl[stageStateInfo.my_state_list[i].position].nowHp = stageStateInfo.my_state_list[i].now_hp;
        }

        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            characterControl[stageStateInfo.enemy_state_list[i].position].maxHp = stageStateInfo.enemy_state_list[i].now_hp;
            characterControl[stageStateInfo.enemy_state_list[i].position].nowHp = stageStateInfo.enemy_state_list[i].now_hp;
        }
    }

    public stageState GetEnemyState(int position)
    {
        stageStateData stageStateInfo = UserDataManager.Inst.GetStageState();
        if (stageStateInfo == null)
        {
            Debug.LogError("버그 : stageStateInfo is NULL");
            return null;
        }

        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            if (stageStateInfo.enemy_state_list[i].position == position)
            {
                return stageStateInfo.enemy_state_list[i];
            }
        }

        Debug.LogError(position + "번째 몬스터의 정보가 없는 오류");
        return null;
    }

    public stageState GetMyState(int position)
    {
        stageStateData stageStateInfo = UserDataManager.Inst.GetStageState();
        if (stageStateInfo == null)
        {
            Debug.LogError("버그 : stageStateInfo is NULL");
            return null;
        }

        for (int i = 0; i < stageStateInfo.my_state_list.Count; i++)
        {
            if (stageStateInfo.my_state_list[i].position == position)
            {
                return stageStateInfo.my_state_list[i];
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

    IEnumerator TestReTargeting()
    {
        testReTageting.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        testReTageting.SetActive(false);
    }
}