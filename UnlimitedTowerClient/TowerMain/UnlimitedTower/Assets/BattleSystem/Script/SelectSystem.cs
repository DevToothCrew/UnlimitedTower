using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSystem : MonoSingleton<SelectSystem>
{
    public CheckSelectAnimation[] chsing = new CheckSelectAnimation[20];
    public int selectIndex = -1;
    public bool isPlayer;
    public Image selectHpBar;
    public Text selectHpText;
    public Image selectCharacterImage;
    public Animator controlButton;
    public ActionState actionState;

    [Header("- Character State")]
    public Text attackDamageText;
    public Text defenceText;
    public Text speedText;
    public Text levelText;
    public Text nemeText;

    private RaycastHit hit;
    private Ray ray;
    private CheckSelectAnimation temp;
    public bool isStart = false;

    public enum ActionState
    {
        Non,
        HeroSelected,
        MonsterSelected
    }

    private void Start()
    {
        attackDamageText = GameObject.Find("StatueAttackText").GetComponent<Text>();
        defenceText = GameObject.Find("StatueDefenceText").GetComponent<Text>();
        speedText = GameObject.Find("StatueSpeedText").GetComponent<Text>();
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        nemeText = GameObject.Find("NameText").GetComponent<Text>();
        controlButton = GameObject.Find("ControlButton").GetComponent<Animator>();
        selectCharacterImage = GameObject.Find("Character Portrait Image").GetComponent<Image>();
        selectHpBar = GameObject.Find("Hp Bar").GetComponent<Image>();
        selectHpText = GameObject.Find("Hp Text").GetComponent<Text>();
    }

    // 추후 최적화 작업, timeScale도 바꿔야함
    void Update()
    {
        if (!isStart)
        {
            if (BattleSystem.Inst.playerCharacterControl[0].select != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (BattleSystem.Inst.characterisPlace[i] == true)
                        chsing[i] = BattleSystem.Inst.playerCharacterControl[i]?.select.GetComponent<CheckSelectAnimation>();
                    if (BattleSystem.Inst.characterisPlace[i + 10] == true)
                        chsing[i + 10] = BattleSystem.Inst.enemyCharacterControl[i]?.select.GetComponent<CheckSelectAnimation>();
                }
                isStart = true;
            }
            else
            {
                Debug.Log("오류다~~");
            }
        }

        if (BattleSystem.Inst.TimeScale != 0)
        {
            for (int i = 0; i < 20; i++)
                if (i != selectIndex)
                    chsing[i]?.gameObject.SetActive(false);

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Character"))
                {
                    temp = hit.transform.GetComponent<CharacterControl>().select.GetComponent<CheckSelectAnimation>();
                    isPlayer = hit.transform.GetComponent<CharacterControl>().isPlayer;

                    temp.gameObject.SetActive(true);
                    temp.AniStart();

                    if (Input.GetMouseButtonUp(0))
                    {
                        if (isPlayer)
                        {
                            selectIndex = hit.transform.GetComponent<CharacterControl>().index;
                            chsing[BattleSystem.Inst.GetServant(selectIndex).position].Click();
                            attackDamageText.text = BattleSystem.Inst.GetServant(selectIndex).attack.ToString();
                            defenceText.text = BattleSystem.Inst.GetServant(selectIndex).defense.ToString();
                            speedText.text = BattleSystem.Inst.GetServant(selectIndex).speed.ToString();
                        }
                        else
                        {
                            selectIndex = hit.transform.GetComponent<CharacterControl>().index + 10;
                            chsing[BattleSystem.Inst.GetMonster(selectIndex).position].Click();
                            // 나중에 Get 한번만하게
                            attackDamageText.text = BattleSystem.Inst.GetMonster(selectIndex).attack.ToString();
                            defenceText.text = BattleSystem.Inst.GetMonster(selectIndex).defense.ToString();
                            speedText.text = BattleSystem.Inst.GetMonster(selectIndex).speed.ToString();
                        }
                        if (selectIndex != -1)
                        {
                            if (selectIndex < 10)
                            {
                                selectHpBar.fillAmount = (float)BattleSystem.Inst.playerCharacterControl[selectIndex].nowHp / BattleSystem.Inst.playerCharacterControl[selectIndex].maxHp;
                                selectHpText.text = BattleSystem.Inst.playerCharacterControl[selectIndex]?.nowHp.ToString();
                                selectCharacterImage.sprite =
                                    Resources.Load<Sprite>("BattleUI/Character Portrait Image/Monster/" +
                                    CharacterCSVData.Inst.monsterDataDic[
                                    UserDataManager.Inst.GetMonsterInfo(
                                    BattleSystem.Inst.testStageStateData.my_state_list[selectIndex < 5 ? 5 + selectIndex : selectIndex].index).monsterNum].inGameIconName);

                                if (selectIndex != 0)
                                {
                                    levelText.text = selectIndex < 5 ?
                                      UserDataManager.Inst.GetServantInfo(
                                      BattleSystem.Inst.testStageStateData.my_state_list[selectIndex].index).level.ToString() :
                                       UserDataManager.Inst.GetMonsterInfo(
                                      BattleSystem.Inst.testStageStateData.my_state_list[selectIndex].index).level.ToString();
                                }
                                else
                                {
                                    levelText.text = UserDataManager.Inst.GetHeroInfo().level.ToString();
                                }

                                nemeText.text = CharacterCSVData.Inst.monsterDataDic[
                                    UserDataManager.Inst.GetMonsterInfo(
                                    BattleSystem.Inst.testStageStateData.my_state_list[selectIndex < 5 ? 5 + selectIndex : selectIndex].index).monsterNum].engName;
                            }
                            else
                            {
                                selectHpBar.fillAmount = (float)BattleSystem.Inst.enemyCharacterControl[selectIndex - 10].nowHp / BattleSystem.Inst.enemyCharacterControl[selectIndex - 10].maxHp;
                                selectHpText.text = BattleSystem.Inst.enemyCharacterControl[selectIndex - 10]?.nowHp.ToString();
                                selectCharacterImage.sprite =
                                    Resources.Load<Sprite>("BattleUI/Character Portrait Image/Monster/" +
                                    CharacterCSVData.Inst.monsterDataDic[
                                    BattleSystem.Inst.GetMonster(selectIndex).index].inGameIconName);

                                nemeText.text = CharacterCSVData.Inst.monsterDataDic[
                                    BattleSystem.Inst.GetMonster(selectIndex).index].engName;
                            }
                        }

                        if (selectIndex == 0)
                        {
                            controlButton.SetTrigger("On");
                            actionState = ActionState.HeroSelected;
                        }
                        else if (selectIndex == 5)
                        {
                            controlButton.SetTrigger("On");
                            actionState = ActionState.MonsterSelected;
                        }

                        if (selectIndex > 10)
                        {
                            if (actionState == ActionState.HeroSelected)
                            {
                                BattleSystem.Inst.targetSettingInfo.heroTargetIndex = selectIndex;
                                BattleSystem.Inst.targetSettingInfo.heroAction = 2;
                                actionState = ActionState.Non;
                            }
                            else if (actionState == ActionState.MonsterSelected)
                            {
                                BattleSystem.Inst.targetSettingInfo.monsterTargetIndex = selectIndex;
                                BattleSystem.Inst.targetSettingInfo.monsterAction = 2;
                                actionState = ActionState.Non;
                            }
                        }
                        else if (selectIndex != 0 && selectIndex != 5)
                        {
                            if (actionState != ActionState.Non)
                            {
                                actionState = ActionState.Non;
                                controlButton.SetTrigger("Off");
                            }
                        }


                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectIndex = -1;
                actionState = ActionState.Non;
                controlButton.SetTrigger("Off");
            }

        }
    }

    public void DefenceChick()
    {
        if (actionState == ActionState.HeroSelected)
            BattleSystem.Inst.targetSettingInfo.heroAction = 3;
        else if (actionState == ActionState.MonsterSelected)
            BattleSystem.Inst.targetSettingInfo.monsterAction = 3;
        actionState = ActionState.Non;
        controlButton.SetTrigger("Off");
    }

    public void AttackChick()
    {
        if (selectIndex == 0)
        {
            actionState = ActionState.HeroSelected;
            controlButton.SetTrigger("Off");
        }
        else if (selectIndex == 5)
        {
            actionState = ActionState.MonsterSelected;
            controlButton.SetTrigger("Off");
        }
    }
}
