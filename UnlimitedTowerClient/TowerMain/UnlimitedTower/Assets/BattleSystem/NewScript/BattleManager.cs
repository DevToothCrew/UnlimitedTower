using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoSingleton<BattleManager>
{
    public GameObject[] character = new GameObject[20];
    public GameObject[] grid = new GameObject[20];
    public BattleStatus[] status = new BattleStatus[20];
    public CharInfo[] charInfo = new CharInfo[20];
    public List<buffInfo>[] buffInfo = new List<buffInfo>[20];
    public Animator[] animator = new Animator[20];
    public bool isAfterDelay;
    public int turnIndex = 1;
    public int TimeScale = 2;
    public TumbAnimation tumbAnimation;
    public int[] CheetKey = new int[3];
    public int CheetIndex;
    public bool adminMode;

    private bool isAuto;
    public bool isBattleStart;
    private GameObject CharacterParent;
    private SkillManager skillManager;
    private CharacterCustom characterCustom;

    public GameObject selectEffect;
    // Canvas
    [Header("Canvas")]
    public GameObject rewardParent;
    public GameObject battleFail;

    [HideInInspector]
    public readonly int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    private void Awake()
    {
        TimeScale = 2;
        Time.timeScale = 2;

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
        BattleUIManager.Inst.BattleTurn.text = stateData.turn.ToString();

        turnIndex = stateData.turn + 1;

        MapChange.Inst.MapEneble(stateData.stageType);

        StartCoroutine(SetStartImage(stateData));
        SettingCharacter(stateData);
        SettingPosition();
        SettingDieCheck();
        BattleUIManager.Inst.StageInfoOn();
    }

    // 배틀데이터를 받아와 공격 ( 메인 배틀 한턴 )
    public IEnumerator BattleStart()
    {
        BattleUIManager.Inst.StageInfoOff();
        isBattleStart = true;

        battleActionData stageActionInfo = UserDataManager.Inst.GetStageAction();
        UserDataManager.Inst.stageState.turn = stageActionInfo.turn;

        for (int i = 0; i < stageActionInfo.character_action_list.Count; i++)
        {
            if (stageActionInfo.character_action_list[i].action_type == 2)
            {
                character[stageActionInfo.character_action_list[i].my_position].GetComponent<BasicAttack>().Attack(stageActionInfo.character_action_list[i]);
            }
            else if (stageActionInfo.character_action_list[i].action_type == 3)
            {
                if (stageActionInfo.character_action_list[i].my_position < 10)
                    BattleUIManager.Inst.MySkAction(GetCharState(stageActionInfo.character_action_list[i].my_position).activeSkillList[0].id);
                else
                    BattleUIManager.Inst.EnemySkAction(GetCharState(stageActionInfo.character_action_list[i].my_position).activeSkillList[0].id);

                SkillManager.Inst.SendMessage("Skill_" + GetCharState(stageActionInfo.character_action_list[i].my_position).activeSkillList[0].id.ToString(), stageActionInfo.character_action_list[i]);
                
            }

            if (i + 1 != stageActionInfo.character_action_list.Count && (
                stageActionInfo.character_action_list[i + 1].action_info_list[0].target_position == stageActionInfo.character_action_list[i].my_position ||
                 stageActionInfo.character_action_list[i + 1].my_position == stageActionInfo.character_action_list[i].action_info_list[0].target_position))
            {
                yield return new WaitForSeconds(7.0f);
            }
            else
            {
                yield return new WaitForSeconds(3.0f);
            }
            isAfterDelay = false;
        }

        yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < stageActionInfo.character_buff_list.Count; i++)
        {
            actionInfo tump = new actionInfo();
            tump.damage = stageActionInfo.character_buff_list[i].damage;
            tump.target_position = stageActionInfo.character_buff_list[i].position;
            if (stageActionInfo.character_buff_list[i].type == 1 || stageActionInfo.character_buff_list[i].type == 3)
                DamageManager.Inst.DamageShow(tump, true, ELEMENT_TYPE.None, false);
            else if (stageActionInfo.character_buff_list[i].type == 2 || stageActionInfo.character_buff_list[i].type == 4)
                DamageManager.Inst.DamageShow(tump, false, ELEMENT_TYPE.None, false);

            yield return new WaitForSeconds(0.1f);
        }

        isBattleStart = false;

        turnIndex++;
        BattleUIManager.Inst.StageInfoOn();

        int myHp = 0, enemyHp = 0;
        for (int i = 0; i < 10; i++)
        {
            myHp += status[i].NowHp;
            enemyHp += status[i + 10].NowHp;
        }

        if (enemyHp == 0)
        {
            
#if UNITY_EDITOR
            Cheat.Inst.RequestStageRewardCheat();
#else
            if (UserDataManager.Inst.GetUserInfo().sceneState == SCENE_STATE.PVP)
            {
                PacketManager.Inst.RequestPVPResult();
            }
            else
            {
                PacketManager.Inst.RequestStageReward();
            }
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
        if (isBattleStart == false)
        {
#if UNITY_EDITOR
            {
                string battleActionInfo = Cheat.Inst.GetBattleActionData("devtooth", turnIndex);
                DebugLog.Log(false, "[SUCCESS] user battleaction :" + battleActionInfo);

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
                Transform tf = Exp.transform.GetChild(positionOrder[state.Value.position + 5]);
                tf.GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBServantDataDic[state.Value.id].servantIcon;
                tf.GetChild(2).gameObject.SetActive(true);
                tf.GetChild(2).GetChild(0).GetComponent<Text>().text = UserDataManager.Inst.GetServantInfo(state.Value.index).level.ToString();
                tf.GetChild(3).GetComponent<Image>().sprite = CSVData.Inst.GetSpriteServantJob((SERVANT_JOB)state.Value.job);
            }
            else if (state.Value.charType == CHAR_TYPE.MONSTER)
            {
                Transform tf = Exp.transform.GetChild(positionOrder[state.Value.position - 5]);
                tf.GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBMonsterDataDic[state.Value.id].monsterIcon;
                tf.GetChild(2).gameObject.SetActive(true);
                tf.GetChild(2).GetChild(0).GetComponent<Text>().text = UserDataManager.Inst.GetMonsterInfo(state.Value.index).level.ToString();
                tf.GetChild(3).GetComponent<Image>().sprite = CSVData.Inst.GetSmallSpriteTribeType((TRIBE_TYPE)state.Value.tribeType);
                tf.GetChild(4).GetComponent<Image>().sprite = CSVData.Inst.GetSpriteElementType((ELEMENT_TYPE)state.Value.elementType);
            }
            Exp.transform.GetChild(positionOrder[state.Value.position < 5 ? state.Value.position + 5 : state.Value.position - 5]).GetComponent<Image>().sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)state.Value.grade);
        }

        for (int i = 0; i < rewardData.get_char_exp_list.Count; i++)
        {
            Transform tf = Exp.transform.GetChild(positionOrder[rewardData.get_char_exp_list[i].pos < 5 ? rewardData.get_char_exp_list[i].pos + 5 : rewardData.get_char_exp_list[i].pos - 5]);
            if (rewardData.get_char_exp_list[i].lvup != 0)
            {
                tf.GetChild(5).gameObject.SetActive(true);
                tf.GetChild(5).GetChild(0).GetComponent<Text>().text = "+" + rewardData.get_char_exp_list[i].lvup.ToString();
            }
            tf.GetChild(1).gameObject.SetActive(true);
            tf.GetChild(1).GetComponent<Text>().text = "+ " + rewardData.get_char_exp_list[i].exp + " Exp";
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

        for (int i = 0; i < rewardData.get_equipment_list.Count; i++)
        {
            rewardItemBack.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)rewardData.get_equipment_list[i].equipment.grade);
            rewardItemImage.sprite = CSVData.Inst.GetEquipmentData(rewardData.get_equipment_list[i].equipment.id).equipmentIcon;
            Instantiate(rewardItemBack, rewardItemParent.transform).gameObject.SetActive(true);
        }

        for (int i = 0; i < rewardData.get_item_list.Count; i++)
        {
            rewardItemBack.sprite = CSVData.Inst.GetSpriteGrade(CSVData.Inst.GetItemData(rewardData.get_item_list[i].id).grade);
            rewardItemImage.sprite = CSVData.Inst.GetItemData(rewardData.get_item_list[i].id).ItemIcon;
            Instantiate(rewardItemBack, rewardItemParent.transform).gameObject.SetActive(true);
        }

        rewardUTG.text = (rewardData.reward_money / 10000.0f).ToString("#.0000");
    }

    // 시작화면 파티 초상화 셋팅
    public IEnumerator SetStartImage(UserStageStateData stateData)
    {
        GameObject temp = GameObject.Find("StartUI");
        GameObject.Find("Player Name Text").GetComponent<Text>().text = stateData.user;
        if (stateData.stageType == 0)
        {
            GameObject.Find("Enemy Name Text").GetComponent<Text>().text = stateData.enemyUser;
        }
        else
        {
            GameObject.Find("Enemy Name Text").GetComponent<Text>().text = "Floor : " + stateData.stageFloor;
        }

        foreach (KeyValuePair<int, UserCharacterStateData> state in stateData.myStateList)
        {
            Transform tf = temp.transform.GetChild(0).GetChild(positionOrder[state.Value.position]);
            if (state.Value.charType == CHAR_TYPE.SERVANT)
            {
                tf.GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBServantDataDic[state.Value.id].servantIcon;
                tf.GetChild(1).gameObject.SetActive(true);
                tf.GetChild(1).GetChild(0).GetComponent<Text>().text = UserDataManager.Inst.GetServantInfo(state.Value.index).level.ToString();
                tf.GetChild(2).GetComponent<Image>().sprite = CSVData.Inst.GetSpriteServantJob((SERVANT_JOB)state.Value.job);
            }
            else if (state.Value.charType == CHAR_TYPE.MONSTER)
            {
                tf.GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBMonsterDataDic[state.Value.id].monsterIcon;
                tf.GetChild(1).gameObject.SetActive(true);
                tf.GetChild(1).GetChild(0).GetComponent<Text>().text = UserDataManager.Inst.GetMonsterInfo(state.Value.index).level.ToString();
                tf.GetChild(2).GetComponent<Image>().sprite = CSVData.Inst.GetSmallSpriteTribeType((TRIBE_TYPE)state.Value.tribeType);
                tf.GetChild(3).GetComponent<Image>().sprite = CSVData.Inst.GetSpriteElementType((ELEMENT_TYPE)state.Value.elementType);
            }
            tf.GetComponent<Image>().sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)state.Value.grade);
        }

        foreach (KeyValuePair<int, UserCharacterStateData> state in stateData.enemyStateList)
        {
            Transform tf = temp.transform.GetChild(1).GetChild(positionOrder[state.Value.position - 10]);
            if (state.Value.charType == CHAR_TYPE.SERVANT)
            {
                tf.GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBServantDataDic[state.Value.id].servantIcon;
            }
            else if (state.Value.charType == CHAR_TYPE.MONSTER)
            {
                tf.GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.DBMonsterDataDic[state.Value.id].monsterIcon;
                tf.GetChild(2).GetComponent<Image>().sprite = CSVData.Inst.GetSmallSpriteTribeType((TRIBE_TYPE)state.Value.tribeType);
                tf.GetChild(3).GetComponent<Image>().sprite = CSVData.Inst.GetSpriteElementType((ELEMENT_TYPE)state.Value.elementType);
            }
            tf.GetComponent<Image>().sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)state.Value.grade);
            tf.GetChild(1).GetChild(0).GetComponent<Text>().text = "?";
            tf.GetChild(1).gameObject.SetActive(true);
        }

        yield return new WaitForSecondsRealtime(5.0f);
        temp.SetActive(false);
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
    public void SettingCharacter(UserStageStateData stateData)
    {
        foreach (KeyValuePair<int, UserCharacterStateData> state in stateData.myStateList)
        {
            if (state.Value.charType == CHAR_TYPE.SERVANT)
            {
                DBServantData ServantInfo = CSVData.Inst.GetServantData(state.Value.id);

                character[state.Value.position] = Instantiate(characterCustom.Create(ServantInfo.job, ServantInfo.head, ServantInfo.hair, ServantInfo.gender, ServantInfo.body), CharacterParent.transform.GetChild(0));
                character[state.Value.position].name = "Servant : " + state.Value.position + " - " + ServantInfo.name;
            }
            else if (state.Value.charType == CHAR_TYPE.MONSTER)
            {
                DBMonsterData MonsterInfo = CSVData.Inst.GetMonsterData(state.Value.id);

                character[state.Value.position] = Instantiate(Resources.Load("InGameCharacterPrefabs/" + CSVData.Inst.GetMonsterDBResourceModel(state.Value.id)) as GameObject, CharacterParent.transform.GetChild(0));
                character[state.Value.position].name = "Monster : " + state.Value.position + " - " + MonsterInfo.name;
            }
            SettinGrid(state.Value.position);
            character[state.Value.position].AddComponent<CharacterIndex>().index = state.Value.position;
            SettingBoxCollider(character[state.Value.position]);
            animator[state.Value.position] = character[state.Value.position].GetComponent<Animator>();
            character[state.Value.position].AddComponent<BasicAttack>();
            charInfo[state.Value.position] = character[state.Value.position].GetComponent<CharInfo>();
            status[state.Value.position] = new BattleStatus(stateData.myStateList[state.Value.position], true);
        }

        foreach (KeyValuePair<int, UserCharacterStateData> state in stateData.enemyStateList)
        {
            if (state.Value.charType == CHAR_TYPE.SERVANT)
            {
                DBServantData ServantInfo = CSVData.Inst.GetServantData(state.Value.id);

                character[state.Value.position] = Instantiate(characterCustom.Create(ServantInfo.job, ServantInfo.head, ServantInfo.hair, ServantInfo.gender, ServantInfo.body), CharacterParent.transform.GetChild(1));
                character[state.Value.position].name = "Servant : " + state.Value.position + " - " + ServantInfo.name;
            }
            else if (state.Value.charType == CHAR_TYPE.MONSTER)
            {
                DBMonsterData MonsterInfo = CSVData.Inst.GetMonsterData(state.Value.id);

                character[state.Value.position] = Instantiate(Resources.Load("InGameCharacterPrefabs/" + CSVData.Inst.GetMonsterDBResourceModel(state.Value.id)) as GameObject, CharacterParent.transform.GetChild(1));
                character[state.Value.position].name = "Monster : " + state.Value.position + " - " + MonsterInfo.name;
            }
            SettinGrid(state.Value.position);
            character[state.Value.position].AddComponent<CharacterIndex>().index = state.Value.position;
            SettingBoxCollider(character[state.Value.position]);
            animator[state.Value.position] = character[state.Value.position].GetComponent<Animator>();
            character[state.Value.position].AddComponent<BasicAttack>();
            charInfo[state.Value.position] = character[state.Value.position].GetComponent<CharInfo>();
            status[state.Value.position] = new BattleStatus(stateData.enemyStateList[state.Value.position], false);
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

    // 재접속시 죽었는지 체크
    public void SettingDieCheck()
    {
        for (int i = 0; i < 20; i++)
        {
            if (character[i])
            {
                if (status[i].NowHp <= 0)
                {
                    animator[i].SetTrigger("isDie");
                    tumbAnimation.DieTumb(i);
                }
            }
        }
    }

    public UserCharacterStateData GetCharState(int index)
    {
        if (index < 10)
        {
            if (UserDataManager.Inst.GetStageState().myStateList.ContainsKey(index))
            {
                return UserDataManager.Inst.GetStageState().myStateList[index];
            }
            else
            {
                return null;
            }
        }
        else
        {
            if (UserDataManager.Inst.GetStageState().enemyStateList.ContainsKey(index))
            {
                return UserDataManager.Inst.GetStageState().enemyStateList[index];
            }
            else
            {
                return null;
            }
        }
    }
}
