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
    public GameObject characterInfo;

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

                        UserCharacterStateData selectStateInfo;

                        if (selectIndex < 10)
                        {
                            selectStateInfo = BattleManager.Inst.GetMyState(selectIndex);
                        }
                        else
                        {
                            selectStateInfo = BattleManager.Inst.GetEnemyState(selectIndex);
                        }

                        SelectGridReset();
                        BattleManager.Inst.grid[selectIndex].SetActive(true);

                        if (selectIndex < 5)
                        {
                            selectCharImg.sprite = CSVData.Inst.DBServantDataDic[selectStateInfo.id].servantIcon;

                            switch (UserDataManager.Inst.GetStageState().myStateList[selectIndex].job)
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
                            levelText.text = UserDataManager.Inst.GetServantInfo(UserDataManager.Inst.GetStageState().myStateList[selectIndex].index).level.ToString();
                        }
                        else if (selectIndex < 10)
                        {
                            selectCharImg.sprite = CSVData.Inst.DBMonsterDataDic[selectStateInfo.id].monsterIcon;
                            nemeText.text = CSVData.Inst.GetMonsterName(selectStateInfo.id);
                            levelText.text = UserDataManager.Inst.GetMonsterInfo(UserDataManager.Inst.GetStageState().myStateList[selectIndex].index).level.ToString();
                        }
                        else
                        {
                            selectCharImg.sprite = CSVData.Inst.DBMonsterDataDic[selectStateInfo.id].monsterIcon;
                            nemeText.text = CSVData.Inst.GetMonsterName(selectStateInfo.id);
                            levelText.text = "?";
                        }

                        _Str.text = selectStateInfo.status.basicStr.ToString();
                        _Dex.text = selectStateInfo.status.basicDex.ToString();
                        _Int.text = selectStateInfo.status.basicInt.ToString();
                        Atk.text = BattleManager.Inst.status[selectIndex].atk.ToString();
                        mAtk.text = BattleManager.Inst.status[selectIndex].mAtk.ToString();
                        Def.text = BattleManager.Inst.status[selectIndex].def.ToString();
                        mDef.text = BattleManager.Inst.status[selectIndex].mDef.ToString();
                        Speed.text = selectStateInfo.speed.ToString();
                        CriPer.text = selectStateInfo.criPer.ToString();
                        mCriPer.text = selectStateInfo.mCriPer.ToString();
                        CriDmg.text = selectStateInfo.criDmg.ToString();
                        mCriDmg.text = selectStateInfo.mCriDmg.ToString();
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
            if (BattleManager.Inst.status[selectIndex].maxHp != 0)
                selectHpBar.fillAmount = (float)BattleManager.Inst.NowHp[selectIndex] / BattleManager.Inst.status[selectIndex].maxHp;
            else
                selectHpBar.fillAmount = BattleManager.Inst.NowHp[selectIndex] / 100;

            selectHpText.text = BattleManager.Inst.NowHp[selectIndex].ToString();
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
