﻿using System.Collections;
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
    public int[] NowAtk = new int[20];
    public bool isAfterDelay;
    public int turnIndex = 1;
    public int TimeScale = 1;
    public TumbAnimation tumbAnimation;

    private bool isAuto;
    private bool isTurnEnd;
    private bool isBattleStart;
    private GameObject CharacterParent;
    private SkillManager skillManager;
    private CharacterCustom characterCustom;

    public GameObject selectEffect;
    // Canvas
    [Header("Canvas")]
    private GameObject rewardParent;
    private GameObject battleFail;

    [HideInInspector]
    public readonly int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    private void Awake()
    {
        TimeScale = (int)Time.timeScale;

        CharacterParent = GameObject.Find("Character Object");
        characterCustom = GameObject.Find("CharacterCustomInstance").GetComponent<CharacterCustom>();
        tumbAnimation = GetComponent<TumbAnimation>();

        rewardParent = GameObject.Find("RewardCanvas");
        battleFail = GameObject.Find("Stage Fail");
        rewardParent.SetActive(false);
        battleFail.SetActive(false);

        UserDataManager.Inst.stageReward = null;
    }

    private void Start()
    {
        UserStageStateData stateData = UserDataManager.Inst.GetStageState();
        if (stateData == null)
        {
            Debug.LogError("버그 : stageStateInfo is NULL");
            return;
        }

        turnIndex = stateData.turn + 1;

        MapChange.Inst.MapEneble(stateData.stageType);

        StartCoroutine(SetStartImage(stateData));
        IsPlaceCheck(stateData);
        SettingMyTeam(stateData);
        SettingEnemyTeam(stateData);
        SettingScript(stateData);
        SettingHp(stateData);
        SettingPosition();
        SettingDieCheck();
    }

    // 배틀데이터를 받아와 공격 ( 메인 배틀 한턴 )
    public IEnumerator BattleStart()
    {
        BattleUIManager.Inst.OffDelay();
        BattleUIManager.Inst.StageInfoOff();
        isTurnEnd = false;
        isBattleStart = true;

        battleActionData stageActionInfo = UserDataManager.Inst.GetStageAction();
        UserDataManager.Inst.stageState.turn = stageActionInfo.turn;
        if (stageActionInfo == null)
        {
            Debug.LogError("버그 : stageActionInfo is Null");
            yield break;
        }
        // TODO : Skill 관련 코드 정리 필요
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
                        if (GetMyState(stageActionInfo.character_action_list[i].my_position).activeSkillList.Count == 0)
                        {
                            Debug.Log("ActiveSkillList is Null");
                        }
                        else
                        {
                            BattleUIManager.Inst.MySkAction(GetMyState(stageActionInfo.character_action_list[i].my_position).activeSkillList[0].id);
                            SkillManager.Inst.SendMessage("Skill_" + GetMyState(stageActionInfo.character_action_list[i].my_position).activeSkillList[0].id.ToString(), stageActionInfo.character_action_list[i]);
                            yield return new WaitUntil(() => isAfterDelay == true);
                        }
                        isAfterDelay = false;
                    }
                }
            }
        }

        isBattleStart = false;

        turnIndex++;
        BattleUIManager.Inst.StageInfoOn();

        int myHp = 0, enemyHp = 0;
        for (int i = 0; i < 10; i++)
        {
            myHp += NowHp[i];
            enemyHp += NowHp[i + 10];
        }

        if (enemyHp == 0)
        {
#if UNITY_EDITOR
            Cheat.Inst.RequestStageRewardCheat();
#else
            PacketManager.Inst.RequestStageReward();
#endif
        }
        else if (myHp == 0)
        {
            battleFail.SetActive(true);
        }
        else if (isAuto)
        {
            yield return new WaitForSeconds(2.0f);
            TurnEnd();
        }
    }

    public void Auto()
    {
        isAuto = !isAuto;
        if (isAuto)
        {
            TurnEnd();
        }
    }

    public void TurnEnd()
    {
        if (isTurnEnd == false && isBattleStart == false)
        {
            isTurnEnd = true;
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

    // 배틀 종료 후 보상 산정
    public void SetReward()
    {
        rewardParent.SetActive(true);

        UserStageStateData stateData = UserDataManager.Inst.GetStageState();
        stageRewardData rewardData = UserDataManager.Inst.GetStageReward();

        GameObject Exp = GameObject.Find("Reward Exp");
        GameObject rewardItemParent = GameObject.Find("Reward Item").transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        Image rewardItemBack = rewardItemParent.transform.GetChild(0).GetComponent<Image>();
        Image rewardItemImage = rewardItemParent.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        Text rewardUTG = GameObject.Find("Reward UTG Text").GetComponent<Text>();

        foreach (KeyValuePair<int, UserCharacterStateData> state in stateData.myStateList)
        {
            if (state.Value.charType == CHAR_TYPE.SERVANT)
            {
                Exp.transform.GetChild(positionOrder[state.Value.position]).GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBServantDataDic[state.Value.id].servantIcon;
            }
            else if (state.Value.charType == CHAR_TYPE.MONSTER)
            {
                Exp.transform.GetChild(positionOrder[state.Value.position]).GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBMonsterDataDic[state.Value.id].monsterIcon;
            }
            Exp.transform.GetChild(positionOrder[state.Value.position]).GetComponent<Image>().sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)state.Value.grade);
        }

        for (int i = 0; i < rewardData.get_char_exp_list.Count; i++)
        {
            Exp.transform.GetChild(positionOrder[rewardData.get_char_exp_list[i].pos]).GetChild(1).gameObject.SetActive(true);
            Exp.transform.GetChild(positionOrder[rewardData.get_char_exp_list[i].pos]).GetChild(1).GetComponent<Text>().text = "+ " + rewardData.get_char_exp_list[i].exp;
        }

        for (int i = 0; i < rewardData.get_servant_list.Count; i++)
        {
            rewardItemBack.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)rewardData.get_servant_list[i].servant.grade);
            rewardItemImage.sprite = CSVData.Inst.GetServantData(rewardData.get_servant_list[i].servant.id).servantIcon;
            Instantiate(rewardItemBack, rewardItemParent.transform).gameObject.SetActive(true);
        }

        for (int i = 0; i < rewardData.get_monster_list.Count; i++)
        {
            rewardItemBack.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)rewardData.get_monster_list[i].monster.grade);
            rewardItemImage.sprite = CSVData.Inst.GetMonsterData(rewardData.get_monster_list[i].monster.id).monsterIcon;
            Instantiate(rewardItemBack, rewardItemParent.transform).gameObject.SetActive(true);
        }


        rewardUTG.text = (rewardData.reward_money / 10000.0f).ToString("#.0000");

    }

    // 시작화면 파티 초상화 셋팅
    public IEnumerator SetStartImage(UserStageStateData stateData)
    {
        GameObject temp = GameObject.Find("StartUI");
        GameObject.Find("Player Name Text").GetComponent<Text>().text = stateData.user;

        foreach (KeyValuePair<int, UserCharacterStateData> state in stateData.myStateList)
        {
            if (state.Value.charType == CHAR_TYPE.SERVANT)
                temp.transform.GetChild(0).GetChild(positionOrder[state.Value.position]).GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBServantDataDic[state.Value.id].servantIcon;
            else if (state.Value.charType == CHAR_TYPE.MONSTER)
                temp.transform.GetChild(0).GetChild(positionOrder[state.Value.position]).GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBMonsterDataDic[state.Value.id].monsterIcon;
            Debug.Log("등급 : " + (GRADE_TYPE)state.Value.grade);
            temp.transform.GetChild(0).GetChild(positionOrder[state.Value.position]).GetComponent<Image>().sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)state.Value.grade);
        }

        foreach (KeyValuePair<int, UserCharacterStateData> state in stateData.enemyStateList)
        {
            if (state.Value.charType == CHAR_TYPE.SERVANT)
                temp.transform.GetChild(1).GetChild(positionOrder[state.Value.position - 10]).GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBServantDataDic[state.Value.id].servantIcon;
            else if (state.Value.charType == CHAR_TYPE.MONSTER)
                temp.transform.GetChild(1).GetChild(positionOrder[state.Value.position - 10]).GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBMonsterDataDic[state.Value.id].monsterIcon;
            temp.transform.GetChild(1).GetChild(positionOrder[state.Value.position - 10]).GetComponent<Image>().sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)state.Value.grade);
        }

        yield return new WaitForSeconds(5.0f);
        temp.SetActive(false);
    }

    // 캐릭터 존재 여부 체크
    public void IsPlaceCheck(UserStageStateData stateData)
    {
        for (int i = 0; i < 10; i++)
        {
            if (stateData.myStateList.ContainsKey(i) == true)
            {
                isPlace[i] = true;
            }
        }

        for (int i = 10; i < 20; i++)
        {
            if (stateData.enemyStateList.ContainsKey(i) == true)
            {
                isPlace[i] = true;
            }
        }
    }

    // 캐릭터 박스 콜라이더 셋팅
    public void SettingBoxCollider(GameObject character)
    {
        BoxCollider box = character.AddComponent<BoxCollider>();
        box.size = new Vector3(0.8f, 0.6f, 0.8f) * (1 / character.transform.localScale.x);
        box.center = new Vector3(0.0f, 0.3f, 0.0f) * (1 / character.transform.localScale.x);
        box.isTrigger = true;
        character.tag = "Character";
    }

    // 캐릭터 그리드 셋팅
    public void SettinGrid(int index)
    {
        GameObject temp = Instantiate(selectEffect, new Vector3(0, 0.1f, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        temp.transform.SetParent(character[index].transform);
        grid[index] = temp;
        grid[index].SetActive(false);
    }

    // 아군 파티 셋팅
    public void SettingMyTeam(UserStageStateData stateData)
    {
        foreach (KeyValuePair<int, UserCharacterStateData> state in stateData.myStateList)
        {
            if (state.Value.charType == CHAR_TYPE.SERVANT)
            {
                UserServantData servantInfo = UserDataManager.Inst.GetServantInfo(state.Value.index);
                if (servantInfo == null)
                {
                    // TODO : LogError를 쓰면 Web에서 멈춘다...
                    Debug.Log("Invalid Servant Index : " + stateData.myStateList[state.Value.position].index);
                    return;
                }

                DBServantData dbServantData = CSVData.Inst.GetServantData(servantInfo.id);
                if (dbServantData == null)
                {
                    Debug.Log("Invalid Servant ID : " + servantInfo.id);
                    return;
                }

                character[state.Value.position] = Instantiate(characterCustom.Create(
                    dbServantData.job,
                    dbServantData.head,
                    dbServantData.hair,
                    dbServantData.gender,
                    dbServantData.body
                    ), CharacterParent.transform.GetChild(0));

                character[state.Value.position].name = "Servant : " + state.Value.position + " - " + dbServantData.name;
                character[state.Value.position].AddComponent<CharacterIndex>().index = state.Value.position;
                SettingBoxCollider(character[state.Value.position]);
                animator[state.Value.position] = character[state.Value.position].GetComponent<Animator>();
            }
            else if (state.Value.charType == CHAR_TYPE.MONSTER)
            {
                UserMonsterData monsterInfo = UserDataManager.Inst.GetMonsterInfo(state.Value.index);
                if (monsterInfo == null)
                {
                    Debug.Log("Invalid Monster Index : " + stateData.myStateList[state.Value.position].index);
                    return;
                }

                DBMonsterData dbMonsterData = CSVData.Inst.GetMonsterData(monsterInfo.id);
                if (dbMonsterData == null)
                {
                    Debug.Log("Invalid Monster ID : " + monsterInfo.id);
                    return;
                }

                character[state.Value.position] = Instantiate(Resources.Load("InGameCharacterPrefabs/" + CSVData.Inst.GetMonsterDBResourceModel(monsterInfo.id)) as GameObject,
                    CharacterParent.transform.GetChild(0));

                character[state.Value.position].name = "Monster : " + state.Value.position + " - " + dbMonsterData.name;
                character[state.Value.position].AddComponent<CharacterIndex>().index = state.Value.position;
                SettingBoxCollider(character[state.Value.position]);
                animator[state.Value.position] = character[state.Value.position].GetComponent<Animator>();
            }
            SettinGrid(state.Value.position);
        }
    }

    // 몬스터 정보 셋팅
    public void SettingEnemyTeam(UserStageStateData stateData)
    {
        for (int i = 10; i < 20; i++)
        {
            if (stateData.enemyStateList.ContainsKey(i) == false)
            {
                continue;
            }

            int enemyID = stateData.enemyStateList[i].id;

            DBMonsterData dbMonsterData = CSVData.Inst.GetMonsterData(enemyID);
            if (dbMonsterData == null)
            {
                Debug.Log("Invalid Monster ID : " + enemyID);
                return;
            }

            character[i] = Instantiate(Resources.Load<GameObject>("InGameCharacterPrefabs/" + CSVData.Inst.GetMonsterDBResourceModel(enemyID)),
                    CharacterParent.transform.GetChild(1));

            character[i].name = "Monster : " + i + " - " + dbMonsterData.name;
            character[i].AddComponent<CharacterIndex>().index = i;
            SettingBoxCollider(character[i]);
            animator[i] = character[i].GetComponent<Animator>();
            SettinGrid(i);
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
    public void SettingScript(UserStageStateData stateData)
    {
        for (int i = 0; i < 10; i++)
        {
            if (stateData.myStateList.ContainsKey(i) == false)
            {
                continue;
            }

            character[i].AddComponent<BasicAttack>();
            charInfo[i] = character[i].GetComponent<CharInfo>();
        }

        for (int i = 10; i < 20; i++)
        {
            if (stateData.enemyStateList.ContainsKey(i) == false)
            {
                continue;
            }

            character[i].AddComponent<BasicAttack>();
            charInfo[i] = character[i].GetComponent<CharInfo>();
        }
    }

    // 캐릭터별 체력 설정
    public void SettingHp(UserStageStateData stateData)
    {
        for (int i = 0; i < 10; i++)
        {
            if (stateData.myStateList.ContainsKey(i) == false)
            {
                continue;
            }

            MaxHp[i] = stateData.myStateList[i].maxHP;
            NowHp[i] = stateData.myStateList[i].nowHp;
            NowAtk[i] = stateData.myStateList[i].atk;
            // TODO : 추후 MATK 등등 여기서 추가
        }

        for (int i = 10; i < 20; i++)
        {
            if (stateData.enemyStateList.ContainsKey(i) == false)
            {
                continue;
            }

            MaxHp[i] = stateData.enemyStateList[i].maxHP;
            NowHp[i] = stateData.enemyStateList[i].nowHp;
            NowAtk[i] = stateData.enemyStateList[i].atk;
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

    public UserCharacterStateData GetEnemyState(int position)
    {
        UserStageStateData stateData = UserDataManager.Inst.GetStageState();
        if (stateData == null)
        {
            Debug.LogError("버그 : stateData is NULL");
            return null;
        }

        if (stateData.enemyStateList[position] == null)
        {
            Debug.LogError(position + "th Monster is Null");
            return null;
        }

        return stateData.enemyStateList[position];
    }

    public UserCharacterStateData GetMyState(int position)
    {
        UserStageStateData stateData = UserDataManager.Inst.GetStageState();
        if (stateData == null)
        {
            Debug.LogError("버그 : stateData is NULL");
            return null;
        }

        if (stateData.myStateList[position] == null)
        {
            Debug.LogError(position + "th Servant is Null");
            return null;
        }

        return stateData.myStateList[position];
    }
}
