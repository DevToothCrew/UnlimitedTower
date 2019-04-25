using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    public int mouseIndex = -1;
    public int selectIndex = -1;

    [Header("- Character State")]
    public Text levelText;
    public Text nemeText;
    public Text selectHpText;
    public Image selectHpBar;
    public Image selectCharImg;
    public Image selectCharJob;
    public Image selectCharElement;
    public GameObject characterInfo;
    public GameObject BuffImage;
    public Sprite blankImage;

    private RaycastHit hit;
    private Ray ray;
    private bool isAttribute;
    
    private GameObject characterStats;
    private Text _Str;
    private Text _Dex;
    private Text _Int;

    private Text Atk;
    private Text mAtk;
    private Text Def;
    private Text mDef;
    private Text Speed;
    private Text CriPer;
    private Text mCriPer;
    private Text CriDmg;
    private Text mCriDmg;
    private Transform BuffParent;

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
        selectCharImg = GameObject.Find("Character Portrait Image").GetComponent<Image>();
        selectCharJob = GameObject.Find("Character Portrait Job(Class)").GetComponent<Image>();
        selectCharElement = GameObject.Find("Character Portrait  Element").GetComponent<Image>();
        selectHpBar = GameObject.Find("Hp Bar").GetComponent<Image>();
        selectHpText = GameObject.Find("Hp Text").GetComponent<Text>();
        characterInfo = GameObject.Find("Character Information");

        characterStats = GameObject.Find("Character Stats");
        _Str = GameObject.Find("Str Text").GetComponent<Text>();
        _Dex = GameObject.Find("Dex Text").GetComponent<Text>();
        _Int = GameObject.Find("Int Text").GetComponent<Text>();
        Atk = GameObject.Find("Atk Text").GetComponent<Text>();
        mAtk = GameObject.Find("mAtk Text").GetComponent<Text>();
        Def = GameObject.Find("Def Text").GetComponent<Text>();
        mDef = GameObject.Find("mDef Text").GetComponent<Text>();
        Speed = GameObject.Find("Speed Text").GetComponent<Text>();
        CriPer = GameObject.Find("CriPer Text").GetComponent<Text>();
        mCriPer = GameObject.Find("mCriPer Text").GetComponent<Text>();
        CriDmg = GameObject.Find("CriDmg Text").GetComponent<Text>();
        mCriDmg = GameObject.Find("mCriDmg Text").GetComponent<Text>();
        BuffParent = GameObject.Find("Buff List").transform;
        characterInfo.SetActive(false);
        characterStats.SetActive(false);
    }

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Character"))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        characterInfo.SetActive(true);

                        selectIndex = hit.transform.GetComponent<CharacterIndex>().index;
                        selectCharJob.sprite = blankImage;
                        selectCharElement.sprite = blankImage;
                        SelectGridReset();

                        BattleManager.Inst.grid[selectIndex].SetActive(true);

                        UserCharacterStateData selectStateInfo = BattleManager.Inst.GetCharState(selectIndex);

                        if (selectStateInfo.charType == CHAR_TYPE.SERVANT)
                        {
                            selectCharJob.sprite = CSVData.Inst.GetSpriteServantJob((SERVANT_JOB)selectStateInfo.job);
                            selectCharImg.sprite = CSVData.Inst.DBServantDataDic[selectStateInfo.id].servantIcon;

                            switch (selectStateInfo.job)
                            {
                                case 0:
                                    nemeText.text = "WhiteHand";
                                    break;
                                case 1:
                                    nemeText.text = "Warrior";
                                    break;
                                case 2:
                                    nemeText.text = "Theif";
                                    break;
                                case 3:
                                    nemeText.text = "Cleric";
                                    break;
                                case 4:
                                    nemeText.text = "Archer";
                                    break;
                                case 5:
                                    nemeText.text = "Magician";
                                    break;
                            }
                        }
                        else if (selectStateInfo.charType == CHAR_TYPE.MONSTER)
                        {
                            selectCharElement.sprite = CSVData.Inst.GetSpriteElementType((ELEMENT_TYPE)selectStateInfo.elementType);
                            selectCharImg.sprite = CSVData.Inst.DBMonsterDataDic[selectStateInfo.id].monsterIcon;

                            nemeText.text = CSVData.Inst.GetMonsterName(selectStateInfo.id);
                        }

                        for (int i = 0; i < BuffParent.childCount; i++)
                        {
                            Destroy(BuffParent.GetChild(i).gameObject);
                        }

                        if (selectStateInfo.passiveSkillList.Count > 0)
                        {
                            foreach (UserSkillInfo skill in selectStateInfo.passiveSkillList)
                            {
                                Instantiate(BuffImage, BuffParent).GetComponent<Image>().sprite = CSVData.Inst.GetSkillPassiveData(skill.id).passiveIcon;
                            }
                        }

                        if (selectIndex < 5)
                        {
                            levelText.text = selectStateInfo.level.ToString();

                            if (UserDataManager.Inst.GetStageState().mySynergyList.Count > 0)
                            {
                                foreach (UserSkillInfo skill in UserDataManager.Inst.GetStageState().mySynergyList)
                                {
                                    Instantiate(BuffImage, BuffParent).GetComponent<Image>().sprite = CSVData.Inst.GetSkillPassiveData(skill.id).passiveIcon;
                                }
                            }
                        }
                        else if (selectIndex < 10)
                        {
                            levelText.text = selectStateInfo.level.ToString();

                            if (UserDataManager.Inst.GetStageState().mySynergyList.Count > 0)
                            {
                                foreach (UserSkillInfo skill in UserDataManager.Inst.GetStageState().mySynergyList)
                                {
                                    Instantiate(BuffImage, BuffParent).GetComponent<Image>().sprite = CSVData.Inst.GetSkillPassiveData(skill.id).passiveIcon;
                                }
                            }
                        }
                        else
                        {
                            if (BattleManager.Inst.adminMode)
                            {
                                levelText.text = selectStateInfo.level.ToString();
                            }
                            else
                            {
                                levelText.text = "?";
                            }
                            if (UserDataManager.Inst.GetStageState().enemySynergyList.Count > 0)
                            {
                                foreach (UserSkillInfo skill in UserDataManager.Inst.GetStageState().enemySynergyList)
                                {
                                    Instantiate(BuffImage, BuffParent).GetComponent<Image>().sprite = CSVData.Inst.GetSkillPassiveData(skill.id).passiveIcon;
                                }
                            }
                        }

                        if (selectIndex < 10 || BattleManager.Inst.adminMode == true)
                        {
                            BattleStatus status = BattleManager.Inst.status[selectIndex];
                            _Str.text = status.Status[EFFECT_ID.STR].ToString();
                            _Dex.text = status.Status[EFFECT_ID.DEX].ToString();
                            _Int.text = status.Status[EFFECT_ID.INT].ToString();
                            Atk.text = status.Status[EFFECT_ID.ATK].ToString();
                            mAtk.text = status.Status[EFFECT_ID.MATK].ToString();
                            Def.text = status.Status[EFFECT_ID.DEF].ToString();
                            mDef.text = status.Status[EFFECT_ID.MDEF].ToString();
                            Speed.text = status.Status[EFFECT_ID.SPEED].ToString();
                            CriPer.text = status.Status[EFFECT_ID.CRI_PER].ToString();
                            mCriPer.text = status.Status[EFFECT_ID.CRI_PER].ToString();
                            CriDmg.text = status.Status[EFFECT_ID.CRI_DMG].ToString();
                            mCriDmg.text = status.Status[EFFECT_ID.CRI_DMG].ToString();
                        }
                        else
                        {
                            _Str.text = "?";
                            _Dex.text = "?";
                            _Int.text = "?";
                            Atk.text = "?";
                            mAtk.text = "?";
                            Def.text = "?";
                            mDef.text = "?";
                            Speed.text = "?";
                            CriPer.text = "?";
                            mCriPer.text = "?";
                            CriDmg.text = "?";
                            mCriDmg.text = "?";
                        }
                    }
                }
                else
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (selectIndex >= 0)
                            BattleManager.Inst.grid[selectIndex].SetActive(false);
                        characterInfo.SetActive(false);
                    }
                }
            }
            else
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    if (selectIndex >= 0)
                        BattleManager.Inst.grid[selectIndex].SetActive(false);
                    characterInfo.SetActive(false);
                }
            }
        }

        if (selectIndex != -1)
        {
            if (BattleManager.Inst.status[selectIndex].Status[EFFECT_ID.HP] != 0)
                selectHpBar.fillAmount = (float)BattleManager.Inst.status[selectIndex].NowHp / BattleManager.Inst.status[selectIndex].Status[EFFECT_ID.HP];

            selectHpText.text = BattleManager.Inst.status[selectIndex].NowHp.ToString();
        }
    }

    public void SelectGridReset()
    {
        for (int i = 0; i < 20; i++)
        {
            BattleManager.Inst.grid[i]?.SetActive(false);
        }
    }

    public void OnAttribute()
    {
        if (isAttribute)
        {
            characterStats.SetActive(false);
            isAttribute = false;
        }
        else
        {
            characterStats.SetActive(true);
            isAttribute = true;
        }
    }
}
