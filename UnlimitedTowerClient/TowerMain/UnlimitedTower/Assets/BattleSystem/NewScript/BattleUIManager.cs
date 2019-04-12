using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleUIManager : MonoSingleton<BattleUIManager> {

    public Image StageInfoBack;
    public Image StageInfoLine;
    public Text StageInfoFloor;
    public Text StageInfoName;
    public Text BattleTurn;

    public GameObject rewardWindows;

    public GameObject battleOutCheck;

    public Animator mySkill;
    public Animator enemySkill;
    public Image mySklImage;
    public Text mySkText;
    public Image enemySklImage;
    public Text enemySkText;

    public Dictionary<int, SkillData> skillDataDic = new Dictionary<int, SkillData>();
    public List<SkillData> skillDataList = new List<SkillData>();

    [System.Serializable]
    public struct SkillData
    {
        public int Index;
        public Sprite SkillImage;
        public string SkillText;

        public SkillData(int Index, Sprite SkillImage, string SkillText)
        {
            this.Index = Index;
            this.SkillImage = SkillImage;
            this.SkillText = SkillText;
        }
    }

    private float timeScale;

    private void Awake()
    {
        StageInfoBack = GameObject.Find("Stage Info Back").GetComponent<Image>();
        StageInfoLine = GameObject.Find("Stage Info Line").GetComponent<Image>();
        StageInfoFloor = GameObject.Find("Stage Info Floor").GetComponent<Text>();
        StageInfoName = GameObject.Find("Stage Info Name").GetComponent<Text>();
        BattleTurn = GameObject.Find("Battle Turn Text").GetComponent<Text>();

        battleOutCheck = GameObject.Find("Battle Exit Check Window");
        battleOutCheck.SetActive(false);

        mySkill = GameObject.Find("MySkill").GetComponent<Animator>();
        mySklImage = mySkill.transform.GetChild(1).GetComponent<Image>();
        mySkText = mySkill.transform.GetChild(2).GetComponent<Text>();
        enemySkill = GameObject.Find("MySkill").GetComponent<Animator>();
        enemySklImage = enemySkill.transform.GetChild(1).GetComponent<Image>();
        enemySkText = enemySkill.transform.GetChild(2).GetComponent<Text>();

        foreach (SkillData skillData in skillDataList)
        {
            skillDataDic.Add(skillData.Index, skillData);
        }

        StageInfoOn();
    }

    public void MySkAction(int SkillIndex)
    {
        mySkill.SetTrigger("Start");
        mySklImage.sprite = skillDataDic[SkillIndex].SkillImage;
        mySkText.text = skillDataDic[SkillIndex].SkillText;
    }

    public void EnemySkAction(int SkillIndex)
    {
        enemySkill.SetTrigger("Start");
        enemySklImage.sprite = skillDataDic[SkillIndex].SkillImage;
        enemySkText.text = skillDataDic[SkillIndex].SkillText;
    }
    
    public void TimeScaleX2()
    {
        int Speed = 2;
        if (BattleManager.Inst.TimeScale != Speed)
        {
            BattleManager.Inst.TimeScale = Speed;
            Time.timeScale = Speed;
        }
        else
        {
            BattleManager.Inst.TimeScale = 1;
            Time.timeScale = 1;
        }
    }

    public void TimeScaleX10()
    {
        int Speed = 10;
        if (BattleManager.Inst.TimeScale != Speed)
        {
            BattleManager.Inst.TimeScale = Speed;
            Time.timeScale = Speed;
        }
        else
        {
            BattleManager.Inst.TimeScale = 1;
            Time.timeScale = 1;
        }
    }
    
    public void StageInfoOn()
    {
        StartCoroutine(StageInfoOn_Co());
    }

    public IEnumerator StageInfoOn_Co()
    {
        UserStageStateData StageData = UserDataManager.Inst.GetStageState();
        StageInfoFloor.text = "Stage " + StageData.stageType + " : " + StageData.stageFloor + "F";
        BattleTurn.text = BattleManager.Inst.turnIndex + " Turn";

        Color plus = new Color(0, 0, 0, 0.02f);
        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSeconds(0.02f);
            StageInfoBack.color += plus;
            StageInfoLine.color += plus;
            StageInfoFloor.color += plus;
            StageInfoName.color += plus;
            BattleTurn.color += plus;
        }
        StageInfoBack.color = Color.white;
        StageInfoLine.color = Color.white;
        StageInfoFloor.color = Color.white;
        StageInfoName.color = Color.white;
        BattleTurn.color = Color.white;
    }

    public void StageInfoOff()
    {
        StartCoroutine(StageInfoOff_Co());
    }

    public IEnumerator StageInfoOff_Co()
    {
        Color minus = new Color(0, 0, 0, 0.02f);
        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSeconds(0.02f);
            StageInfoBack.color -= minus;
            StageInfoLine.color -= minus;
            StageInfoFloor.color -= minus;
            StageInfoName.color -= minus;
            BattleTurn.color -= minus;
        }
        StageInfoBack.color = new Color(1, 1, 1, 0);
        StageInfoLine.color = new Color(1, 1, 1, 0);
        StageInfoFloor.color = new Color(1, 1, 1, 0);
        StageInfoName.color = new Color(1, 1, 1, 0);
        BattleTurn.color = new Color(1, 1, 1, 0);
    }

    public void BattleActionOut()
    {
#if UNITY_EDITOR
        Cheat.Inst.RequestStageExitCheat();
#else
        PacketManager.Inst.RequestStageExit();
#endif
    }

    public void BattleOut()
    {
        PacketManager.Inst.ResponseStageExit();
    }

    public void BattleOutCheck()
    {
        timeScale = BattleManager.Inst.TimeScale;
        BattleManager.Inst.TimeScale = 0;
        Time.timeScale = 0;
        battleOutCheck.SetActive(true);
    }

    public void BattleOutCheckCancel()
    {
        BattleManager.Inst.TimeScale = (int)timeScale;
        Time.timeScale = timeScale;
        battleOutCheck.SetActive(false);
    }

    public void NextStage()
    {
        if (UserDataManager.Inst.GetUserPartyInfo().partyIndex == 0)
        {
            Debug.LogError("Invalid User Data");
        }

#if UNITY_EDITOR
        {
            if (UserDataManager.Inst.stageState.stageFloor != 10)
            Cheat.Inst.RequestStageStartCheat(UserDataManager.Inst.stageState.stageType, UserDataManager.Inst.stageState.stageFloor + 1, 1);
        }
#else
        {
        if (UserDataManager.Inst.stageState.stageFloor != 10)
            PacketManager.Inst.RequestStageStart(UserDataManager.Inst.stageState.stageType, UserDataManager.Inst.stageState.stageFloor + 1, 1);
        }
#endif
        Time.timeScale = 1;
        BattleManager.Inst.rewardParent.SetActive(false);
    }

    public void StageContinue()
    {
        if (UserDataManager.Inst.GetUserPartyInfo().partyIndex == 0)
        {
            Debug.LogError("Invalid User Data");
        }

#if UNITY_EDITOR
        {
            Cheat.Inst.RequestStageStartCheat(UserDataManager.Inst.stageState.stageType, UserDataManager.Inst.stageState.stageFloor, 1);
        }
#else
        {
            PacketManager.Inst.RequestStageStart(UserDataManager.Inst.stageState.stageType, UserDataManager.Inst.stageState.stageFloor, 1);
        }
#endif
        Time.timeScale = 1;
        BattleManager.Inst.battleFail.SetActive(false);
    }
}
