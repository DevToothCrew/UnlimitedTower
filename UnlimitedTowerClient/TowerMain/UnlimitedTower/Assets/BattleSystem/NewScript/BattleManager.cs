using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoSingleton<BattleManager>
{
    public GameObject[] character = new GameObject[20];
    public GameObject[] grid = new GameObject[20];
    public CharInfo[] charInfo = new CharInfo[20];
    public Animator[] animator = new Animator[20];
    public bool[] isPlace = new bool[20];
    public int[] MaxHp = new int[20];
    public int[] NowHp = new int[20];
    public bool isAfterDelay;
    public int TimeScale = 1;
    public TumbAnimation tumbAnimation;

    private int turnIndex = 1;
    private bool isSpaceCheck;
    private GameObject CharacterParent;
    private SkillManager skillManager;
    private CharacterCustom characterCustom;

    // Test
    [Header("Test")]
    public GameObject testGrid;
    private GameObject testReward;
    private GameObject testDefeat;
    private Text ErrorText;
    private GameObject ErrorBox;

    [HideInInspector]
    public bool isBattleStart;
    [HideInInspector]
    public readonly int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    private void Awake()
    {
        TimeScale = (int)Time.timeScale;

        CharacterParent = GameObject.Find("Character Object");
        characterCustom = GameObject.Find("CharacterCustomInstance").GetComponent<CharacterCustom>();
        tumbAnimation = GetComponent<TumbAnimation>();


        testReward = GameObject.Find("보상");
        testDefeat = GameObject.Find("패배보상");

        testReward.SetActive(false);
        testDefeat.SetActive(false);

        UserDataManager.Inst.stageReward = null;
    }

    private void Start()
    {
        stageStateData stageStateInfo = UserDataManager.Inst.GetStageState();
        turnIndex = stageStateInfo.turn + 1;
        if (stageStateInfo == null)
        {
            Debug.LogError("버그 : stageStateInfo is NULL");
            return;
        }

        // SetStartImage(stageStateInfo);
        IsPlaceCheck(stageStateInfo);
        // SettingHero();
        SettingCharacter(stageStateInfo);
        SettingMonster(stageStateInfo);
        SettingScript(stageStateInfo);
        SettingHp(stageStateInfo);
        SettingPosition();
        SettingDieCheck();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // if (isSpaceCheck == false)
            // {
            //     // TestBattleTarget();
            //     isSpaceCheck = true;
            // }
            BattleUIManager.Inst.OnDelay();

            #if UNITY_EDITOR
            {
                string battleActionInfo = Cheat.Inst.GetBattleActionData("devtooth", turnIndex);
                Debug.Log("[SUCCESS] user battleaction :" + battleActionInfo);

                PacketManager.Inst.ResponseBattleAction(JsonUtility.FromJson<battleActionData>(battleActionInfo));
            }
            #else
            {
                PacketManager.Inst.RequestBattleAction(turnIndex);
            }
            #endif

        }
    }

    // 배틀데이터를 받아와 공격 ( 메인 배틀 한턴 )
    public IEnumerator BattleStart()
    {
        BattleUIManager.Inst.OffDelay();
        isSpaceCheck = false;

        battleActionData stageActionInfo = UserDataManager.Inst.GetStageAction();
        if (stageActionInfo == null)
        {
            Debug.LogError("버그 : stageActionInfo is Null");
            yield break;
        }

        for (int i = 0; i < stageActionInfo.character_action_list.Count; i++)
        {
            if (NowHp[stageActionInfo.character_action_list[i].my_position] > 0)
            {
                if (stageActionInfo.character_action_list[i].action_type == 2)
                {
                    character[stageActionInfo.character_action_list[i].my_position].GetComponent<BasicAttack>().Attack(stageActionInfo.character_action_list[i]);

                    yield return new WaitUntil(() => isAfterDelay == true);
                    isAfterDelay = false;
                }
                else if (stageActionInfo.character_action_list[i].action_type == 3)
                {
                    if (stageActionInfo.character_action_list[i].my_position < 10)
                    {
                        SkillManager.Inst.SendMessage("Skill_" + GetMyState(stageActionInfo.character_action_list[i].my_position).active_skill_list[0].id.ToString(), stageActionInfo.character_action_list[i]);

                        yield return new WaitUntil(() => isAfterDelay == true);
                        isAfterDelay = false;
                    }
                }
            }
        }

        turnIndex++;
        isSpaceCheck = false;
        BattleUIManager.Inst.MyTurn();

        int myHp = 0, enemyHp = 0;
        for (int i = 0; i < 10; i++)
        {
            myHp += NowHp[i];
            enemyHp += NowHp[i + 10];
        }
        Debug.Log("myHp : " + myHp);
        Debug.Log("enemyHp : " + enemyHp);


        if (myHp == 0 || enemyHp == 0)
        {
            PacketManager.Inst.RequestStageReward();
        }
    }

    public void BattleEnd()
    {
        stageRewardData rewardData = UserDataManager.Inst.GetStageReward();
        if (rewardData == null)
        {
            Debug.LogError("버그 : rewardData is Null");
            return;
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
                temp += " : " + rewardData.get_servant_list[i].servant.job;
            temp += "\nMonster";
            for (int i = 0; i < rewardData.get_monster_list.Count; i++)
                temp += " : " + rewardData.get_monster_list[i].monster.id;
            temp += "\nEquipment";
            for (int i = 0; i < rewardData.get_equipment_list.Count; i++)
                temp += " : " + rewardData.get_equipment_list[i].equipment.id;
            // item 추가 필요 및 Gold / EOS 처리 필요
            testReward.transform.GetChild(0).GetComponent<Text>().text = temp;
        }
        else
        {
            testDefeat.SetActive(true);
        }
        UserDataManager.Inst.stageReward = null;
    }

    public void TestBattleTarget()
    {
        characterActionData battleActionInfo = new characterActionData();
        actionInfo actionInfo = new actionInfo();
        actionInfo.target_position = 10;
        actionInfo.damage = 1000;
        battleActionInfo.action_type = 302;
        battleActionInfo.action_info_list.Add(actionInfo);
        actionInfo.target_position = 14;
        actionInfo.damage = 1000;
        battleActionInfo.action_type = 302;
        battleActionInfo.action_info_list.Add(actionInfo);
        battleActionInfo.my_position = 0;

        SkillManager.Inst.Skill_200007(battleActionInfo);

        isSpaceCheck = false;
    }

    // 시작화면 파티 초상화 셋팅
    public void SetStartImage(stageStateData stageStateInfo)
    {
        Image[] image = new Image[20];
        GameObject temp = GameObject.Find("StartUI");
        
        for (int i = 0; i < stageStateInfo.my_state_list.Count; i++)
        {
            Debug.Log(stageStateInfo.my_state_list[i].id);
            if (stageStateInfo.my_state_list[i].position < 5)
                temp.transform.GetChild(0).GetChild(stageStateInfo.my_state_list[i].position).GetComponent<Image>().sprite = CSVData.Inst.DBServantDataDic[stageStateInfo.my_state_list[i].id].servantIcon;
            else
                temp.transform.GetChild(0).GetChild(stageStateInfo.my_state_list[i].position).GetComponent<Image>().sprite = CSVData.Inst.DBMonsterDataDic[stageStateInfo.my_state_list[i].id].monsterIcon;
        }

        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            Debug.Log(stageStateInfo.enemy_state_list[i].id);
            temp.transform.GetChild(1).GetChild(stageStateInfo.enemy_state_list[i].position - 10).GetComponent<Image>().sprite = CSVData.Inst.DBMonsterDataDic[stageStateInfo.enemy_state_list[i].id].monsterIcon;
        }
    }

    // 캐릭터 존재 여부 체크
    public void IsPlaceCheck(stageStateData stageStateInfo)
    {
        for (int i = 0; i < stageStateInfo.my_state_list.Count; i++)
        {
            isPlace[stageStateInfo.my_state_list[i].position] = true;
        }

        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            isPlace[stageStateInfo.enemy_state_list[i].position] = true;
        }
    }

    // 캐릭터 박스 콜라이더 셋팅
    public void SettingBoxCollider(GameObject character)
    {
        BoxCollider box = character.AddComponent<BoxCollider>();
        box.size = new Vector3(0.8f, 0.8f, 0.8f) * (1 / character.transform.localScale.x);
        box.center = new Vector3(0.0f, 0.4f, 0.0f) * (1 / character.transform.localScale.x);
        box.isTrigger = true;
        character.tag = "Character";
    }

    // 캐릭터 그리드 셋팅
    public void SettinGrid(int index)
    {
        GameObject temp = Instantiate(testGrid, Vector3.zero, Quaternion.Euler(new Vector3(90, 0, 0)));
        temp.transform.SetParent(character[index].transform);
        grid[index] = temp;
        grid[index].SetActive(false);
    }

    // 아군 파티 셋팅
    public void SettingCharacter(stageStateData stageStateInfo)
    {
        for (int i = 0; i < stageStateInfo.my_state_list.Count; i++)
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

                DBServantData dbServantData = CSVData.Inst.GetServantData(servantInfo.id);
                if (dbServantData == null)
                {
                    Debug.LogError(servantInfo.id);
                    Debug.LogError("버그다");
                    return;
                }

                character[stageStateInfo.my_state_list[i].position] = Instantiate(characterCustom.Create(
                    dbServantData.job,
                    dbServantData.head,
                    dbServantData.hair,
                    dbServantData.gender,
                    dbServantData.body
                    ), CharacterParent.transform.GetChild(0));
                character[stageStateInfo.my_state_list[i].position].name = "Servant : " + stageStateInfo.my_state_list[i].position.ToString();
                character[stageStateInfo.my_state_list[i].position].AddComponent<CharacterIndex>().index = stageStateInfo.my_state_list[i].position;
                SettingBoxCollider(character[stageStateInfo.my_state_list[i].position]);
                animator[stageStateInfo.my_state_list[i].position] = character[stageStateInfo.my_state_list[i].position].GetComponent<Animator>();
            }
            else
            {
                character[stageStateInfo.my_state_list[i].position] = Instantiate(Resources.Load("InGameCharacterPrefabs/" + CSVData.Inst.GetMonsterDBResourceModel(stageStateInfo.my_state_list[i].id)) as GameObject,
                    CharacterParent.transform.GetChild(0));
                character[stageStateInfo.my_state_list[i].position].name = "Monster : " + stageStateInfo.my_state_list[i].position.ToString();
                character[stageStateInfo.my_state_list[i].position].AddComponent<CharacterIndex>().index = stageStateInfo.my_state_list[i].position;
                SettingBoxCollider(character[stageStateInfo.my_state_list[i].position]);
                animator[stageStateInfo.my_state_list[i].position] = character[stageStateInfo.my_state_list[i].position].GetComponent<Animator>();
            }
            SettinGrid(stageStateInfo.my_state_list[i].position);
        }
    }

    // 몬스터 정보 셋팅
    public void SettingMonster(stageStateData stageStateInfo)
    {
        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            Debug.Log(stageStateInfo.enemy_state_list[i].id);
            character[stageStateInfo.enemy_state_list[i].position] = Instantiate(Resources.Load<GameObject>("InGameCharacterPrefabs/" + CSVData.Inst.GetMonsterDBResourceModel(stageStateInfo.enemy_state_list[i].id)),
                    CharacterParent.transform.GetChild(1));
            character[stageStateInfo.enemy_state_list[i].position].name = "Monster : " + stageStateInfo.enemy_state_list[i].position.ToString();
            character[stageStateInfo.enemy_state_list[i].position].AddComponent<CharacterIndex>().index = stageStateInfo.enemy_state_list[i].position;
            SettingBoxCollider(character[stageStateInfo.enemy_state_list[i].position]);
            animator[stageStateInfo.enemy_state_list[i].position] = character[stageStateInfo.enemy_state_list[i].position].GetComponent<Animator>();
            SettinGrid(stageStateInfo.enemy_state_list[i].position);
        }
    }

    // 캐릭터 위치 배치
    public void SettingPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i < 5)
            {
                if (character[i] != null)
                    character[i].transform.position = new Vector3(-4.2f + 2.1f * (positionOrder[i] % 5), 0, -4.2f);
                if (character[i + 10] != null)
                    character[i + 10].transform.position = new Vector3(4.2f - 2.1f * (positionOrder[i] % 5), 0, 4.2f);
            }
            else
            {
                if (character[i] != null)
                    character[i].transform.position = new Vector3(-4.2f + 2.1f * (positionOrder[i] % 5), 0, -2.1f);
                if (character[i + 10] != null)
                    character[i + 10].transform.position = new Vector3(4.2f - 2.1f * (positionOrder[i] % 5), 0, 2.1f);
            }
        }
    }

    // 모든 캐릭터 스크립트 생성 
    public void SettingScript(stageStateData stageStateInfo)
    {
        for (int i = 0; i < stageStateInfo.my_state_list.Count; i++)
        {
            character[stageStateInfo.my_state_list[i].position].AddComponent<BasicAttack>();
            charInfo[stageStateInfo.my_state_list[i].position] = character[stageStateInfo.my_state_list[i].position].GetComponent<CharInfo>();
        }

        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            character[stageStateInfo.enemy_state_list[i].position].AddComponent<BasicAttack>();
            charInfo[stageStateInfo.enemy_state_list[i].position] = character[stageStateInfo.enemy_state_list[i].position].GetComponent<CharInfo>();
        }
    }

    // 캐릭터별 체력 설정
    public void SettingHp(stageStateData stageStateInfo)
    {
        for (int i = 0; i < stageStateInfo.my_state_list.Count; i++)
        {
            MaxHp[stageStateInfo.my_state_list[i].position] = Calculator.GetMaxHp(stageStateInfo.my_state_list[i].status);
            NowHp[stageStateInfo.my_state_list[i].position] = stageStateInfo.my_state_list[i].now_hp;
        }

        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            MaxHp[stageStateInfo.enemy_state_list[i].position] = Calculator.GetMaxHp(stageStateInfo.enemy_state_list[i].status);
            NowHp[stageStateInfo.enemy_state_list[i].position] = stageStateInfo.enemy_state_list[i].now_hp;
        }
    }

    // 재접속시 죽었는지 체크
    public void SettingDieCheck()
    {
        for (int i = 0; i < 20; i++)
        {
            if (character[i])
            {
                if (NowHp[i] <= 0)
                {
                    animator[i].SetTrigger("isDie");
                    tumbAnimation.DieTumb(i);
                }
            }
        }
    }

    public characterStateData GetEnemyState(int position)
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

        Debug.LogError(position + "th Monster is Null");
        return null;
    }

    public characterStateData GetMyState(int position)
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

        Debug.LogError(position + "th Servant is Null");
        return null;
    }
}
