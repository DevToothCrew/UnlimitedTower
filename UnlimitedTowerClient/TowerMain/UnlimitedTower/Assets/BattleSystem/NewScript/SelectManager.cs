﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectManager : MonoBehaviour {
    public int mouseIndex = -1;
    public int selectIndex = -1;

    [Header("- Character State")]
    public Text levelText;
    public Text nemeText;
    public Text selectHpText;
    public Text selectAtkText;
    public Image selectHpBar;
    public Image selectCharImg;
    public GameObject characterInfo;

    private RaycastHit hit;
    private Ray ray;

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
        selectCharImg = GameObject.Find("Character Portrait Image").GetComponent<Image>();
        selectHpBar = GameObject.Find("Hp Bar").GetComponent<Image>();
        selectHpText = GameObject.Find("Hp Text").GetComponent<Text>();
        selectAtkText = GameObject.Find("Power Text").GetComponent<Text>();
        characterInfo = GameObject.Find("Character Information");
        characterInfo.SetActive(false);

        //testAd = GameObject.Find("Test Ad").GetComponent<Text>();
        //testAp = GameObject.Find("Test Ap").GetComponent<Text>();
        //testDd = GameObject.Find("Test Dd").GetComponent<Text>();
        //testDp = GameObject.Find("Test Dp").GetComponent<Text>();
        //testSp = GameObject.Find("Test Sp").GetComponent<Text>();
        //testMaxHp = GameObject.Find("Test MaxHp").GetComponent<Text>();
        //testNowHp = GameObject.Find("Test NowHp").GetComponent<Text>();
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
                    characterInfo.SetActive(true);

                    selectIndex = hit.transform.GetComponent<CharacterIndex>().index;

                    characterStateData selectStateInfo;

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

                    if (BattleManager.Inst.MaxHp[selectIndex] != 0)
                        selectHpBar.fillAmount = (float)BattleManager.Inst.NowHp[selectIndex] / BattleManager.Inst.MaxHp[selectIndex];
                    else
                        selectHpBar.fillAmount = BattleManager.Inst.NowHp[selectIndex] / 100;

                    selectHpText.text = BattleManager.Inst.NowHp[selectIndex].ToString();
                    selectAtkText.text = BattleManager.Inst.NowAtk[selectIndex].ToString();

                    if (selectIndex < 5)
                    {
                        selectCharImg.sprite = CSVData.Inst.DBServantDataDic[selectStateInfo.id].servantIcon;
                        levelText.text = UserDataManager.Inst.GetServantInfo(UserDataManager.Inst.GetStageState().my_state_list[selectIndex].index).level.ToString();
                    }
                    else if (selectIndex < 10)
                    {
                        selectCharImg.sprite = CSVData.Inst.DBMonsterDataDic[selectStateInfo.id].monsterIcon;
                        nemeText.text = CSVData.Inst.GetMonsterName(selectStateInfo.id);
                        levelText.text = UserDataManager.Inst.GetMonsterInfo(UserDataManager.Inst.GetStageState().my_state_list[selectIndex].index).level.ToString();
                    }
                    else
                    {
                        selectCharImg.sprite = CSVData.Inst.DBMonsterDataDic[selectStateInfo.id].monsterIcon;
                        nemeText.text = CSVData.Inst.GetMonsterName(selectStateInfo.id);
                        levelText.text = "?";
                    }

                    //testMaxHp.text = BattleManager.Inst.MaxHp[selectIndex].ToString();
                    //testNowHp.text = BattleManager.Inst.NowHp[selectIndex].ToString();
                    //testAd.text = (selectStateInfo.physical_attack).ToString();
                    //testAp.text = (selectStateInfo.magic_attack).ToString();
                    //testDd.text = (selectStateInfo.physical_defense).ToString();
                    //testDp.text = (selectStateInfo.magic_defense).ToString();
                    //testSp.text = selectStateInfo.speed.ToString();
                }
                else
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (selectIndex > 0)
                            BattleManager.Inst.grid[selectIndex].SetActive(false);
                        characterInfo.SetActive(false);
                    }
                }
            }
            else
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    if (selectIndex > 0)
                        BattleManager.Inst.grid[selectIndex].SetActive(false);
                    characterInfo.SetActive(false);
                }
            }
        }
    }

    public void SelectGridReset()
    {
        for (int i = 0; i < 20; i++)
        {
            BattleManager.Inst.grid[i]?.SetActive(false);
        }
    }
}
