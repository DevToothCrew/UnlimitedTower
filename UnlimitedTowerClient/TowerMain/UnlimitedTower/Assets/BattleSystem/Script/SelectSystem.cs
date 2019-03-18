using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectSystem : MonoSingleton<SelectSystem>
{
    public CheckSelectAnimation[] chsing = new CheckSelectAnimation[20];
    public int mouseIndex = -1;
    public int selectIndex = -1;
    public bool isPlayer;
    public ActionState actionState;

    [Header("- Character State")]
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

    // Test
    public Text testAd;
    public Text testAp;
    public Text testDd;
    public Text testDp;
    public Text testSp;
    public Text testMaxHp;
    public Text testNowHp;

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
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        nemeText = GameObject.Find("NameText").GetComponent<Text>();
        selectCharacterImage = GameObject.Find("Character Portrait Image").GetComponent<Image>();
        selectHpBar = GameObject.Find("Hp Bar").GetComponent<Image>();
        selectHpText = GameObject.Find("Hp Text").GetComponent<Text>();
        characterInfo = GameObject.Find("Character Information");
        characterInfo.SetActive(false);

        testAd = GameObject.Find("Test Ad").GetComponent<Text>();
        testAp = GameObject.Find("Test Ap").GetComponent<Text>();
        testDd = GameObject.Find("Test Dd").GetComponent<Text>();
        testDp = GameObject.Find("Test Dp").GetComponent<Text>();
        testSp = GameObject.Find("Test Sp").GetComponent<Text>();
        testMaxHp = GameObject.Find("Test MaxHp").GetComponent<Text>();
        testNowHp = GameObject.Find("Test NowHp").GetComponent<Text>();
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

                        if (!BattleSystem.Inst.isBattleStart)
                        {
                            if (actionState == ActionState.HeroSelected || actionState == ActionState.MonsterSelected)
                            {
                                if (selectIndex > 9)
                                {
                                    if (BattleSystem.Inst.characterControl[selectIndex].nowHp > 0)
                                    {
                                        BattleUIManager.Inst.attackButtonani.SetBool("isOn", true);
                                        BattleUIManager.Inst.attackButtonani.SetBool("isOff", false);
                                    }
                                }
                                else
                                {
                                    BattleUIManager.Inst.attackButtonani.SetBool("isOn", false);
                                    BattleUIManager.Inst.attackButtonani.SetBool("isOff", true);
                                }
                            }
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
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        selectIndex = -1;
                        characterInfo.SetActive(false);
                    }
                }
            }
        }
        if (selectIndex != -1)
        selectHpText.text = BattleSystem.Inst.characterControl[selectIndex].nowHp.ToString();
    }

    public void DefenceChick()
    {
        if (actionState == ActionState.HeroSelected)
        {
            BattleSystem.Inst.targetSettingInfo.heroAction = 3;
            BattleUIManager.Inst.heroTargetBorder.SetActive(false);
            BattleUIManager.Inst.heroTargetImage.sprite = BattleUIManager.Inst.defenceImage;
        }
        else if (actionState == ActionState.MonsterSelected)
        {
            BattleSystem.Inst.targetSettingInfo.monsterAction = 3;
            BattleUIManager.Inst.monsterTargetBorder.SetActive(false);
            BattleUIManager.Inst.monsterTargetImage.sprite = BattleUIManager.Inst.defenceImage;
        }
        actionState = ActionState.Non;
        BattleUIManager.Inst.defenceButtonani.SetBool("isOff", true);
        BattleUIManager.Inst.defenceButtonani.SetBool("isOn", false);
        BattleUIManager.Inst.attackButtonani.SetBool("isOff", true);
        BattleUIManager.Inst.attackButtonani.SetBool("isOn", false);
    }

    public void AttackChick()
    {
        if (actionState == ActionState.HeroSelected)
        {
            // BattleSystem.Inst.targetSettingInfo.heroTargetIndex = selectIndex;
            // BattleSystem.Inst.targetSettingInfo.heroAction = 2;
            // BattleUIManager.Inst.SetHeroTargetImage(BattleSystem.Inst.GetEnemyState(selectIndex).id);
            // BattleUIManager.Inst.heroTargetBorder.SetActive(false);
            actionState = ActionState.HeroTargetSelected;
        }
        else if (actionState == ActionState.MonsterSelected)
        {
            // BattleSystem.Inst.targetSettingInfo.monsterAction = 2;
            // BattleSystem.Inst.targetSettingInfo.monsterTargetIndex = selectIndex;
            // BattleUIManager.Inst.SetMonsterTargetImage(BattleSystem.Inst.GetEnemyState(selectIndex).id);
            // BattleUIManager.Inst.monsterTargetBorder.SetActive(false);
            actionState = ActionState.MonsterTargetSelected;
        }
        // BattleUIManager.Inst.attackButtonani.SetBool("isOff", true);
        // BattleUIManager.Inst.attackButtonani.SetBool("isOn", false);
        // BattleUIManager.Inst.defenceButtonani.SetBool("isOff", true);
        // BattleUIManager.Inst.defenceButtonani.SetBool("isOn", false);
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
            nemeText.text = CharacterCSVData.Inst.monsterDataDic[selectStateInfo.id].engName;

            selectCharacterImage.sprite = Resources.Load<Sprite>("BattleUI/Character Portrait Image/Monster/" +
                CharacterCSVData.Inst.monsterDataDic[selectStateInfo.id].inGameIconName);

            levelText.text = UserDataManager.Inst.GetMonsterInfo(UserDataManager.Inst.GetStageState().my_state_list[selectIndex].index).level.ToString();
        }
        else
        {
            nemeText.text = CharacterCSVData.Inst.monsterDataDic[selectStateInfo.id].engName;
            
            selectCharacterImage.sprite = Resources.Load<Sprite>("BattleUI/Character Portrait Image/Monster/" +
                CharacterCSVData.Inst.monsterDataDic[selectStateInfo.id].inGameIconName);

            levelText.text = "?";
        }


        testMaxHp.text = BattleSystem.Inst.characterControl[selectIndex].maxHp.ToString();
        testNowHp.text = BattleSystem.Inst.characterControl[selectIndex].nowHp.ToString();
        testAd.text = (selectStateInfo.physical_attack / 100).ToString();
        testAp.text = (selectStateInfo.magic_attack / 100).ToString();
        testDd.text = (selectStateInfo.physical_defense / 100).ToString();
        testDp.text = (selectStateInfo.magic_defense / 100).ToString();
        testSp.text = selectStateInfo.speed.ToString();

        selectHpBar.fillAmount = (float)BattleSystem.Inst.characterControl[selectIndex].nowHp / BattleSystem.Inst.characterControl[selectIndex].maxHp;
    }

    public void ChoiceEndChick()
    {
        if (selectIndex > 9)
        {
            if (actionState == ActionState.HeroTargetSelected)
            {
                BattleSystem.Inst.targetSettingInfo.heroTargetIndex = selectIndex;
                BattleSystem.Inst.targetSettingInfo.heroAction = 2;
                BattleUIManager.Inst.SetHeroTargetImage(BattleSystem.Inst.GetEnemyState(selectIndex).id);
                actionState = ActionState.Non;
            }
            else if (actionState == ActionState.MonsterTargetSelected)
            {
                BattleSystem.Inst.targetSettingInfo.monsterAction = 2;
                BattleSystem.Inst.targetSettingInfo.monsterTargetIndex = selectIndex;
                BattleUIManager.Inst.SetMonsterTargetImage(BattleSystem.Inst.GetEnemyState(selectIndex).id);
                actionState = ActionState.Non;
            }
        }
    }
}

