using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSystem : MonoSingleton<SelectSystem>
{
    public CheckSelectAnimation[] chsing = new CheckSelectAnimation[20];
    public int mouseIndex = -1;
    public int selectIndex = -1;
    public bool isPlayer;
    public Animator controlButton;
    public ActionState actionState;

    [Header("- Character State")]
    public Text attackDamageText;
    public Text defenceText;
    public Text speedText;
    public Text levelText;
    public Text nemeText;
    public Text selectHpText;
    public Image selectHpBar;
    public Image selectCharacterImage;
    public GameObject characterInfo;

    private RaycastHit hit;
    private Ray ray;
    private CheckSelectAnimation selectAnimation;
    public bool isStart = false;

    public enum ActionState
    {
        Non,
        HeroSelected,
        HeroTargetSelected,
        MonsterSelected,
        MonsterTargetSelected
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
        characterInfo = GameObject.Find("Character Information");
        characterInfo.SetActive(false);
    }

    // 추후 최적화 작업, timeScale도 바꿔야함
    void Update()
    {
        if (!isStart)
        {
            if (BattleSystem.Inst.characterControl[0].select != null)
            {
                for (int i = 0; i < 20; i++)
                {
                    if (BattleSystem.Inst.characterisPlace[i] == true)
                        chsing[i] = BattleSystem.Inst.characterControl[i]?.select.GetComponent<CheckSelectAnimation>();
                }
                isStart = true;
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
                    selectAnimation = hit.transform.GetComponent<CharacterControl>().select.GetComponent<CheckSelectAnimation>();
                    selectAnimation.gameObject.SetActive(true);
                    mouseIndex = hit.transform.GetComponent<CharacterControl>().index;

                    if (Input.GetMouseButtonUp(0))
                    {
                        CharacterClick();

                        if (selectIndex == 0)
                        {
                            controlButton.SetBool("isOn", true);
                            actionState = ActionState.HeroSelected;
                        }
                        else if (selectIndex == 5)
                        {
                            controlButton.SetBool("isOn", true);
                            actionState = ActionState.MonsterSelected;
                        }
                        else
                        {
                            controlButton.SetBool("isOn", false);
                        }

                        if (actionState == ActionState.HeroTargetSelected)
                        {
                            if (selectIndex > 9)
                            {
                                if (BattleSystem.Inst.characterControl[selectIndex].nowHp > 0)
                                {
                                    BattleSystem.Inst.targetSettingInfo.heroAction = 2;
                                    BattleSystem.Inst.targetSettingInfo.heroTargetIndex = selectIndex;
                                }
                                else
                                {
                                    StartCoroutine(BattleSystem.Inst.TestTargetDie());
                                }
                            }
                            actionState = ActionState.Non;
                        }
                        else if (actionState == ActionState.MonsterTargetSelected)
                        {
                            if (selectIndex > 9)
                            {
                                if (BattleSystem.Inst.characterControl[selectIndex].nowHp > 0)
                                {
                                    BattleSystem.Inst.targetSettingInfo.monsterAction = 2;
                                    BattleSystem.Inst.targetSettingInfo.monsterTargetIndex = selectIndex;
                                }
                                else
                                {
                                    StartCoroutine(BattleSystem.Inst.TestTargetDie());
                                }
                            }
                            actionState = ActionState.Non;
                        }
                    }
                }
                else
                {
                    mouseIndex = -1;

                    if (Input.GetMouseButtonUp(0))
                    {
                        selectIndex = -1;
                        characterInfo.SetActive(false);
                    }
                }
            }
            else
            {
                mouseIndex = -1;

                if (Input.GetMouseButtonUp(0))
                {
                    selectIndex = -1;
                    characterInfo.SetActive(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectIndex = -1;
                actionState = ActionState.Non;
                controlButton.SetBool("isOn", false);
            }
        }
        if (selectIndex != -1)
        selectHpText.text = BattleSystem.Inst.characterControl[selectIndex].nowHp.ToString();
    }

    public void DefenceChick()
    {
        if (actionState == ActionState.HeroSelected)
            BattleSystem.Inst.targetSettingInfo.heroAction = 3;
        else if (actionState == ActionState.MonsterSelected)
            BattleSystem.Inst.targetSettingInfo.monsterAction = 3;
        actionState = ActionState.Non;
        controlButton.SetBool("isOn", false);
    }

    public void AttackChick()
    {
        if (selectIndex == 0)
        {
            actionState = ActionState.HeroTargetSelected;
            controlButton.SetBool("isOn", false);
        }
        else if (selectIndex == 5)
        {
            actionState = ActionState.MonsterTargetSelected;
            controlButton.SetBool("isOn", false);
        }
    }

    public void CharacterClick()
    {

        characterInfo.SetActive(true);
        selectIndex = hit.transform.GetComponent<CharacterControl>().index;
        selectAnimation.Click();
        stageState selectStateInfo;

        if (selectIndex < 10)
        {
            selectStateInfo = BattleSystem.Inst.GetMyState(selectIndex);
        }
        else
        {
            selectStateInfo = BattleSystem.Inst.GetEnemyState(selectIndex);
        }

        attackDamageText.text = selectStateInfo.attack.ToString();
        defenceText.text = selectStateInfo.defense.ToString();
        speedText.text = selectStateInfo.speed.ToString();
        selectHpText.text = BattleSystem.Inst.characterControl[selectIndex].nowHp.ToString();
        
        if (selectIndex == 0)
        {
            levelText.text = UserDataManager.Inst.GetHeroInfo().level.ToString();
        }
        else if (selectIndex < 5)
        {
            levelText.text = UserDataManager.Inst.GetServantInfo(UserDataManager.Inst.GetStageState().my_state_list[selectIndex].index).level.ToString();

            selectCharacterImage.sprite = null;
        }
        else if (selectIndex < 10)
        {
            nemeText.text = CharacterCSVData.Inst.monsterDataDic[
            UserDataManager.Inst.GetMonsterInfo(
            UserDataManager.Inst.GetStageState().my_state_list[selectIndex].index).id].engName;

            selectCharacterImage.sprite = Resources.Load<Sprite>("BattleUI/Character Portrait Image/Monster/" +
                CharacterCSVData.Inst.monsterDataDic[
            UserDataManager.Inst.GetMonsterInfo(
            UserDataManager.Inst.GetStageState().my_state_list[selectIndex].index).id].inGameIconName);

            levelText.text = UserDataManager.Inst.GetMonsterInfo(UserDataManager.Inst.GetStageState().my_state_list[selectIndex].index).level.ToString();
        }
        else
        {
            nemeText.text = CharacterCSVData.Inst.monsterDataDic[selectStateInfo.index].engName;
            
            selectCharacterImage.sprite = Resources.Load<Sprite>("BattleUI/Character Portrait Image/Monster/" +
                CharacterCSVData.Inst.monsterDataDic[
            BattleSystem.Inst.GetEnemyState(selectIndex).index].inGameIconName);

            levelText.text = "?";
        }

        selectHpBar.fillAmount = (float)BattleSystem.Inst.characterControl[selectIndex].nowHp / BattleSystem.Inst.characterControl[selectIndex].maxHp;
    }
}
