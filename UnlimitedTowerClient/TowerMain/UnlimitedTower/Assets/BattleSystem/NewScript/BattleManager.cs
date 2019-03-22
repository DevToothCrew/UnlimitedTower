﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoSingleton<BattleManager>
{
    public GameObject[] character = new GameObject[20];
    public CharInfo[] charInfo = new CharInfo[20];
    public Animator[] animator = new Animator[20];
    public bool[] isPlace = new bool[20];
    public int[] MaxHp = new int[20];
    public int[] NowHp = new int[20];
    public bool isAfterDelay;
    public int TimeScale = 1;

    private int turnIndex = 1;
    private bool isSpaceCheck;
    private GameObject CharacterParent;
    private GameObject delayImage;
    private SkillManager skillManager;
    private CharacterCustom characterCustom;

    // Test
    private GameObject testMyTurn;
    private GameObject testReward;
    private GameObject testReTageting;
    private GameObject testDefeat;
    private GameObject testTargetDie;
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

        testReward = GameObject.Find("보상");
        testDefeat = GameObject.Find("패배보상");
        delayImage = GameObject.Find("DelayImage");

        testReward.SetActive(false);
        testDefeat.SetActive(false);
        delayImage.SetActive(false);
        
        UserDataManager.Inst.stageReward = null;
    }

    private void Start()
    {
        stageStateData stageStateInfo = UserDataManager.Inst.GetStageState();
        if (stageStateInfo == null)
        {
            Debug.LogError("버그 : stageStateInfo is NULL");
            return;
        }

        IsPlaceCheck(stageStateInfo);
        SettingHero();
        SettingCharacter(stageStateInfo);
        SettingMonster(stageStateInfo);
        SettingScript(stageStateInfo);
        SettingHp(stageStateInfo);
        SettingPosition();
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

            string battleActionInfo = Cheat.Inst.GetBattleActionData("devtooth", turnIndex);
            Debug.Log("[SUCCESS] user battleaction :" + battleActionInfo);
            PacketManager.Inst.ResponseBattleAction(battleActionInfo);
        }
    }

    // 배틀데이터를 받아와 공격 ( 메인 배틀 한턴 )
    public IEnumerator BattleStart()
    {
        delayImage.SetActive(false);
        isSpaceCheck = false;

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
                character[stageActionInfo.battle_info_list[i].my_position].GetComponent<BasicAttack>().Attack(stageActionInfo.battle_info_list[i]);

                yield return new WaitUntil(() => isAfterDelay == true);
                isAfterDelay = false;
            }
            else if (stageActionInfo.battle_info_list[i].action_type == 3)
            {
                if (stageActionInfo.battle_info_list[i].my_position < 10)
                {
                    SkillManager.Inst.SendMessage("Skill_" + GetMyState(stageActionInfo.battle_info_list[i].my_position).active_skill_list[0].id.ToString(), stageActionInfo.battle_info_list[i]);
                    
                    yield return new WaitUntil(() => isAfterDelay == true);
                    isAfterDelay = false;
                }
            }
        }

        turnIndex++;
        isSpaceCheck = false;

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
    }


    public void TestBattleTarget()
    {
        battleActionInfo battleActionInfo = new battleActionInfo();
        actionInfo actionInfo = new actionInfo();
        actionInfo.target_position = 10;
        actionInfo.damage = 1000;
        battleActionInfo.action_type = 302;
        battleActionInfo.battle_action_list.Add(actionInfo);
        actionInfo.target_position = 14;
        actionInfo.damage = 1000;
        battleActionInfo.action_type = 302;
        battleActionInfo.battle_action_list.Add(actionInfo);
        battleActionInfo.my_position = 0;

        SkillManager.Inst.Skill_200007(battleActionInfo);

        isSpaceCheck = false;
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

    public void SettingBoxCollider(GameObject charactor)
    {
        BoxCollider box = charactor.AddComponent<BoxCollider>();
        box.size = new Vector3(0.8f, 0.8f, 0.8f);
        box.center = new Vector3(0.0f, 0.4f, 0.0f);
        box.isTrigger = true;
        charactor.tag = "Character";
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
        character[0] = Instantiate(characterCustom.Create(
            heroInfo.jobNum,
            heroInfo.headNum - 1,
            heroInfo.hairNum - 1,
            heroInfo.gender == 1 ? 1 : 0,
            heroInfo.body == 1 ? 0 : 1
            ), CharacterParent.transform.GetChild(0));
        character[0].name = "Hero";
        character[0].AddComponent<CharacterIndex>().index = 0;
        SettingBoxCollider(character[0]);
        animator[0] = character[0].GetComponent<Animator>();
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
                character[stageStateInfo.my_state_list[i].position] = Instantiate(characterCustom.Create(
                    servantInfo.jobNum,
                    servantInfo.headNum - 1,
                    servantInfo.hairNum - 1,
                    servantInfo.gender == 1 ? 1 : 0,
                    servantInfo.body == 1 ? 0 : 1
                    ), CharacterParent.transform.GetChild(0));
                character[stageStateInfo.my_state_list[i].position].name = "Servant : " + stageStateInfo.my_state_list[i].position.ToString();
                character[stageStateInfo.my_state_list[i].position].AddComponent<CharacterIndex>().index = stageStateInfo.my_state_list[i].position;
                SettingBoxCollider(character[stageStateInfo.my_state_list[i].position]);
                animator[stageStateInfo.my_state_list[i].position] = character[stageStateInfo.my_state_list[i].position].GetComponent<Animator>();
            }
            else
            {
                character[stageStateInfo.my_state_list[i].position] = Instantiate(Resources.Load("InGameCharacterPrefabs/" + CSVData.Inst.monsterDataDic
                    [stageStateInfo.my_state_list[i].id].resource) as GameObject,
                    CharacterParent.transform.GetChild(0));
                character[stageStateInfo.my_state_list[i].position].name = "Monster : " + stageStateInfo.my_state_list[i].position.ToString();
                character[stageStateInfo.my_state_list[i].position].AddComponent<CharacterIndex>().index = stageStateInfo.my_state_list[i].position;
                SettingBoxCollider(character[stageStateInfo.my_state_list[i].position]);
                animator[stageStateInfo.my_state_list[i].position] = character[stageStateInfo.my_state_list[i].position].GetComponent<Animator>();
            }
        }
    }

    // 몬스터 정보 셋팅
    public void SettingMonster(stageStateData stageStateInfo)
    {
        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            character[stageStateInfo.enemy_state_list[i].position] = Instantiate(Resources.Load<GameObject>("InGameCharacterPrefabs/" + CSVData.Inst.monsterDataDic
                    [stageStateInfo.enemy_state_list[i].id].resource),
                    CharacterParent.transform.GetChild(1));
            character[stageStateInfo.enemy_state_list[i].position].name = "Monster : " + stageStateInfo.enemy_state_list[i].position.ToString();
            character[stageStateInfo.enemy_state_list[i].position].AddComponent<CharacterIndex>().index = stageStateInfo.enemy_state_list[i].position;
            SettingBoxCollider(character[stageStateInfo.enemy_state_list[i].position]);
            animator[stageStateInfo.enemy_state_list[i].position] = character[stageStateInfo.enemy_state_list[i].position].GetComponent<Animator>();
        }
    }
    
    // 캐릭터 위치 배치
    public void SettingPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i < 5)
            {
                character[i].transform.position = new Vector3(-4.2f + 2.1f * (positionOrder[i] % 5), 0, -4.2f);
                character[i + 10].transform.position = new Vector3(4.2f - 2.1f * (positionOrder[i] % 5), 0, 4.2f);
            }
            else
            {
                character[i].transform.position = new Vector3(-4.2f + 2.1f * (positionOrder[i] % 5), 0, -2.1f);
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
            NowHp[stageStateInfo.my_state_list[i].position] = stageStateInfo.my_state_list[i].now_hp / 100;
        }

        for (int i = 0; i < stageStateInfo.enemy_state_list.Count; i++)
        {
            MaxHp[stageStateInfo.enemy_state_list[i].position] = Calculator.GetMaxHp(stageStateInfo.enemy_state_list[i].status);
            NowHp[stageStateInfo.enemy_state_list[i].position] = stageStateInfo.enemy_state_list[i].now_hp / 100;
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

        Debug.LogError(position + "th Monster is Null");
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

        Debug.LogError(position + "th Servant is Null");
        return null;
    }
}
