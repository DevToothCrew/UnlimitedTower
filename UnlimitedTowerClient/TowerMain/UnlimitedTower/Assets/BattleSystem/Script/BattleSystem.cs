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
    public int TimeScale = 1;
    [HideInInspector]
    public readonly int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    public GameObject[] characterObject = new GameObject[20];
    public CharacterControl[] characterControl = new CharacterControl[20];
    public bool[] characterisPlace = new bool[20];

    public TargetSettingInfo targetSettingInfo = new TargetSettingInfo();

    public bool isTestPlay;

    private CaracterCustom caracterCustom;
    private DefenceEffect defenceEffect;
    public bool isBattleStart;
    private bool isSpaceCheck;
    public bool isAuto;

    [HideInInspector]
    public GameObject delayImage;

    // Test
    private GameObject testMyTurn;
    private GameObject testReward;
    private GameObject testReTageting;
    private GameObject testDefeat;
    private GameObject testTargetDie;


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
        TimeScale = (int)System.Math.Round(Time.timeScale);
        
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

        defenceEffect = GameObject.Find("DefenceActionObject").GetComponent<DefenceEffect>();
        delayImage = GameObject.Find("DelayImage");

        defenceEffect.gameObject.SetActive(false);
        delayImage.SetActive(false);

        testMyTurn = GameObject.Find("마이턴");
        testReward = GameObject.Find("보상");
        testReTageting = GameObject.Find("공격대상");
        testDefeat = GameObject.Find("패배보상");
        testTargetDie = GameObject.Find("죽은대상");

        testMyTurn.SetActive(false);
        testReward.SetActive(false);
        testReTageting.SetActive(false);
        testDefeat.SetActive(false);
        testTargetDie.SetActive(false);
        UserDataManager.Inst.stageReward = null;


        prefabList = GetComponent<PrefabList>();
        battleInformation.attackerIndex = -1;

        stageStateData stageStateInfo = UserDataManager.Inst.GetStageState();
        if (stageStateInfo == null)
        {
            Debug.LogError("버그 : stageStateInfo is NULL");
            return;
        }

        if (!isTestPlay)
        {
            IsPlaceCheck(stageStateInfo);
            SettingScript(stageStateInfo);
            SettingHero();
            SettingCharacter(stageStateInfo);
            SettingMonster(stageStateInfo);
            SettingHp(stageStateInfo);
            SettingPosition();
        }
        else
        {
            TestIsPlaceCheck();
            TestSettingScript();
            TestSettingMonster();
            TestSettingHp();
            SettingPosition();
        }
    }

    public void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //나가기 패킷 보내기
            delayImage.SetActive(true);
            PacketManager.Inst.RequestStageExit();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            delayImage.SetActive(true);
            SceneManager.LoadScene("Lobby");
        }
    }

    #region 테스트용

    public void TestIsPlaceCheck()
    {
        characterisPlace[0] = true;
        characterisPlace[10] = true;
    }

    public void TestSettingScript()
    {
        characterControl[0] = characterObject[0]?.AddComponent<CharacterControl>();
        characterControl[0].index = 0;
        characterControl[10] = characterObject[10]?.AddComponent<CharacterControl>();
        characterControl[10].index = 10;
    }

    public void TestSettingMonster()
    {
        GameObject servant = Instantiate(caracterCustom.Create(5, 0, 0, 0, 0), characterObject[0].transform);
        servant.transform.position = characterObject[0].transform.position;
        
        Instantiate(Resources.Load<GameObject>("InGameCharacterPrefabs/" + CharacterCSVData.Inst.monsterDataInspector[0].resource), characterObject[10].transform);
    }

    public void TestSettingHp()
    {
        characterControl[0].maxHp = 1000;
        characterControl[0].nowHp = 1000;
        characterControl[10].maxHp = 1000;
        characterControl[10].nowHp = 1000;
    }

    // 1번부터 10번까지 번갈아 가며 공격
    public void TestBattleTarget()
    {
        battleInformation.attackerIndex = 0;
        battleInformation.targetIndex = 10;
        battleInformation.damage = 10;
        battleInformation.isCritical = false;
        battleInformation.isAvoid = false;

        characterControl[0].Attack(new SendValue(0, 10, 10, false, false));
    }

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
        delayImage.SetActive(false);

        isSpaceCheck = false;
        stageStateData stageStateInfo = UserDataManager.Inst.GetStageState();
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
                battleInformation.damage = (int)(stageActionInfo.battle_info_list[i].battle_action_list[0].damage * 0.01f);
                battleInformation.isCritical = stageActionInfo.battle_info_list[i].battle_action_list[0].critical;
                battleInformation.isAvoid = stageActionInfo.battle_info_list[i].battle_action_list[0].avoid;
                characterControl[battleInformation.attackerIndex].Attack(new SendValue(
                        battleInformation.attackerIndex,
                        battleInformation.targetIndex,
                        battleInformation.damage,
                        battleInformation.isCritical,
                        battleInformation.isAvoid));
                yield return new WaitForSeconds(7.0f);
            }
            else
            {
                defenceEffect.gameObject.transform.position = characterObject[stageActionInfo.battle_info_list[i].my_position].transform.position + new Vector3(0,0.1f,0);
                defenceEffect.transform.localScale = characterControl[stageActionInfo.battle_info_list[i].my_position].select.transform.localScale * 2;
                defenceEffect.gameObject.SetActive(true);
                defenceEffect.EffectAction();
                yield return new WaitForSeconds(4.0f);
            }
        }

        if (UserDataManager.Inst.stageReward != null)
        {
            stageRewardData rewardData = UserDataManager.Inst.GetStageReward();
            if (rewardData == null)
            {
                Debug.LogError("버그 : rewardData is Null");
                yield break;
            }

            string temp = "";
            testReward.SetActive(true);

            if (rewardData.get_exp_list.Count != 0)
            {
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
                testDefeat.SetActive(true);
            }

            UserDataManager.Inst.stageReward = null;
        }
        else
        {
            BattleUIManager.Inst.ResetTargetImage();
            StartCoroutine(TestMyTurn());
            BattleUIManager.Inst.BattleEndAction();
            isBattleStart = false;

            if (isAuto)
            {
                AutoTargeting();
                TurnEnd();
            }
        }
    }

    public void ReTargeting()
    {
        isAuto = false;
        delayImage.SetActive(false);
        BattleUIManager.Inst.ResetTargetImage();
        BattleUIManager.Inst.BattleEndAction();
        isBattleStart = false;
    }

    public void AutoTargeting()
    {
        List<int> Live = new List<int>();
        for (int i = 10; i < 20; i++)
            if (characterControl[i]?.nowHp > 0)
                Live.Add(i);

        if (characterControl[0]?.nowHp > 0)
        {
            targetSettingInfo.heroAction = 2;
            targetSettingInfo.heroTargetIndex = Live[Random.Range(0, 10) % Live.Count];
        }
        else
        {
            targetSettingInfo.heroAction = 3;
        }

        if (characterControl[5]?.nowHp > 0)
        {
            targetSettingInfo.monsterAction = 2;
            targetSettingInfo.monsterTargetIndex = Live[Random.Range(0, 10) % Live.Count];
        }
        else
        {
            targetSettingInfo.monsterAction = 3;
        }
    }

    public void TurnEnd()
    {
        // 다중 if문 추후 수정 필요할듯

        Debug.Log("isAuto : " + isAuto);
        Debug.Log("isTestPlay : " + isTestPlay);

        if (!isTestPlay)
        {
            if (isBattleStart == false)
            {

                if (characterControl[0]?.nowHp <= 0)
                {
                    targetSettingInfo.heroAction = 3;
                }
                if (characterControl[5]?.nowHp <= 0)
                {
                    targetSettingInfo.monsterAction = 3;
                }

                if (((targetSettingInfo.heroAction == 2 && targetSettingInfo.heroTargetIndex > 9) || targetSettingInfo.heroAction == 3) &&
                ((targetSettingInfo.monsterAction == 2 && targetSettingInfo.monsterTargetIndex > 9) || targetSettingInfo.monsterAction == 3))
                {
                    if (isSpaceCheck == false)
                    {
                        isBattleStart = true;
                        delayImage.SetActive(true);
                        UTUMSProvider.Instance?.RequestBattleAction(targetSettingInfo.heroTargetIndex, targetSettingInfo.heroAction, targetSettingInfo.monsterTargetIndex, targetSettingInfo.monsterAction);
                        targetSettingInfo = new TargetSettingInfo();
                        BattleUIManager.Inst.BattleStartAction();
                    }
                }
                else
                    StartCoroutine(TestReTargeting());
            }
        }
        else
        {
            TestBattleTarget();
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
                    [stageStateInfo.my_state_list[i].id].resource) as GameObject,
                    characterObject[stageStateInfo.my_state_list[i].position].transform);
            }
        }
    }

    // 몬스터 정보 셋팅
    public void SettingMonster(stageStateData stageStateInfo)
    {
        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            Instantiate(Resources.Load<GameObject>("InGameCharacterPrefabs/" + CharacterCSVData.Inst.monsterDataDic
                    [stageStateInfo.enemy_state_list[i].id].resource),
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
        characterControl[0].maxHp = Calculator.GetMaxHp(UserDataManager.Inst.GetHeroInfo().status);
        characterControl[stageStateInfo.my_state_list[0].position].nowHp = stageStateInfo.my_state_list[0].now_hp / 100;

        for (int i = 1; i < stageStateInfo.my_state_list.Count; i++)
        {
            int index = stageStateInfo.my_state_list[i].index;
            int position = stageStateInfo.my_state_list[i].position;
            if (position < 5)
                characterControl[stageStateInfo.my_state_list[i].position].maxHp = Calculator.GetMaxHp(UserDataManager.Inst.GetServantInfo(index).status);
            else
                characterControl[stageStateInfo.my_state_list[i].position].maxHp = Calculator.GetMaxHp(UserDataManager.Inst.GetMonsterInfo(index).status);
            characterControl[stageStateInfo.my_state_list[i].position].nowHp = stageStateInfo.my_state_list[i].now_hp / 100;
        }
        
        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            characterControl[stageStateInfo.enemy_state_list[i].position].maxHp = stageStateInfo.enemy_state_list[i].now_hp / 100;
            characterControl[stageStateInfo.enemy_state_list[i].position].nowHp = stageStateInfo.enemy_state_list[i].now_hp / 100;
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

    public IEnumerator TestMyTurn()
    {
        testMyTurn.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        testMyTurn.SetActive(false);
    }

    public IEnumerator TestReTargeting()
    {
        testReTageting.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        testReTageting.SetActive(false);
    }

    public IEnumerator TestTargetDie()
    {
        testTargetDie.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        testTargetDie.SetActive(false);
    }
}