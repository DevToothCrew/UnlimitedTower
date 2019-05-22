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

        battleOutCheck = GameObject.Find("Pause Background");
        battleOutCheck.SetActive(false);

        mySkill = GameObject.Find("MySkill").GetComponent<Animator>();
        mySklImage = mySkill.transform.GetChild(1).GetComponent<Image>();
        mySkText = mySkill.transform.GetChild(2).GetComponent<Text>();
        enemySkill = GameObject.Find("EnemySkill").GetComponent<Animator>();
        enemySklImage = enemySkill.transform.GetChild(1).GetComponent<Image>();
        enemySkText = enemySkill.transform.GetChild(2).GetComponent<Text>();
    }

    public void MySkAction(int SkillIndex)
    {
        mySkill.SetTrigger("Start");
        mySklImage.sprite = CSVData.Inst.GetSkillActiveData(SkillIndex).activeIcon;
        mySkText.text = CSVData.Inst.GetSkillActiveData(SkillIndex).name;
    }

    public void EnemySkAction(int SkillIndex)
    {
        enemySkill.SetTrigger("Start");
        enemySklImage.sprite = CSVData.Inst.GetSkillActiveData(SkillIndex).activeIcon;
        enemySkText.text = CSVData.Inst.GetSkillActiveData(SkillIndex).name;
    }
    
    public void TimeScaleX2()
    {
        int Speed = 4;
        if (BattleManager.Inst.TimeScale != Speed)
        {
            BattleManager.Inst.TimeScale = Speed;
            Time.timeScale = Speed;
        }
        else
        {
            BattleManager.Inst.TimeScale = 2;
            Time.timeScale = 2;
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
        if (StageData.stageType == 0)
        {
            StageInfoFloor.text = "PVP";
            StageInfoName.text = StageData.user + " VS " + StageData.enemyUser;
        }
        else
        {
            StageInfoFloor.text = "Stage " + StageData.stageType + " : " + StageData.stageFloor + "F";
            StageInfoName.text = CSVData.Inst.GetStageData(StageData.stageType, StageData.stageFloor, StageData.stageDifficult).stageString;
        }
        BattleTurn.text = (StageData.turn == 0 ? "1" : (StageData.turn + 1).ToString()) + " Turn";

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

    // 배틀중 나가기
    public void BattleActionOut()
    {
        int myHp = 0, enemyHp = 0;
        for (int i = 0; i < 10; i++)
        {
            myHp += BattleManager.Inst.status[i].NowHp;
            enemyHp += BattleManager.Inst.status[i + 10].NowHp;
        }

        if (myHp <= 0 || enemyHp <= 0)
        {
            PacketManager.Inst.ResponseStageExit();
        }
        else if (UserDataManager.Inst.stageReward?.reward_money == 0 || UserDataManager.Inst.stageReward == null)
        {
#if UNITY_EDITOR
            Cheat.Inst.RequestStageExitCheat();
#else
        PacketManager.Inst.RequestStageExit();
#endif
        }
        else
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Last Turn");
        }
    }

    // 배틀 종료후 나가기
    public void BattleOut()
    {
        PacketManager.Inst.ResponseStageExit();
    }

    // 일시정지 버튼
    public void BattleOutCheck()
    {
        timeScale = BattleManager.Inst.TimeScale;
        BattleManager.Inst.TimeScale = 0;
        Time.timeScale = 0;
        battleOutCheck.SetActive(true);
    }

    // 일시정지 버튼 캔슬
    public void BattleOutCheckCancel()
    {
        BattleManager.Inst.TimeScale = (int)timeScale;
        Time.timeScale = timeScale;
        battleOutCheck.SetActive(false);
    }

    public void NextStage()
    {
        UserInventoryInfo inventoryInfo = UserDataManager.Inst.GetUserInventoryInfo();

        if (UserDataManager.Inst.GetUserPartyInfo().partyIndex == 0)
        {
            DebugLog.Log(true, "Invalid User Data");
        }

        if (inventoryInfo.servantInventory < UserDataManager.Inst.GetServantCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Servant Inventory is Full");
            return;
        }
        else if (inventoryInfo.monsterInventory < UserDataManager.Inst.GetMonsterCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Monster Inventory is Full");
            return;
        }
        else if (inventoryInfo.equipmentInventory < UserDataManager.Inst.GetEquipmentCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Equipment Inventory is Full");
            return;
        }
        else
        {

#if UNITY_EDITOR
            {
                if (UserDataManager.Inst.stageState.stageFloor != 10)
                {
                    Cheat.Inst.RequestStageStartCheat(UserDataManager.Inst.stageState.stageType, UserDataManager.Inst.stageState.stageFloor + 1, UserDataManager.Inst.stageState.stageDifficult, 1);
                    UserDataManager.Inst.stageState = new UserStageStateData();
                    UserDataManager.Inst.stageActionInfo = new battleActionData();
                    UserDataManager.Inst.stageReward = new stageRewardData();
                }
            }
#else
        {
        if (UserDataManager.Inst.stageState.stageFloor != 10)
            PacketManager.Inst.RequestStageStart(UserDataManager.Inst.stageState.stageType, UserDataManager.Inst.stageState.stageFloor + 1, UserDataManager.Inst.stageState.stageDifficult,1);
            UserDataManager.Inst.stageState = new UserStageStateData();
            UserDataManager.Inst.stageActionInfo = new battleActionData();
            UserDataManager.Inst.stageReward = new stageRewardData();
        }
#endif
            Time.timeScale = 2;
            BattleManager.Inst.rewardParent?.SetActive(false);
        }
    }

    public void StageContinue()
    {
        UserInventoryInfo inventoryInfo = UserDataManager.Inst.GetUserInventoryInfo();

        if (UserDataManager.Inst.GetUserPartyInfo().partyIndex == 0)
        {
            DebugLog.Log(true, "Invalid User Data");
        }

        if (inventoryInfo.servantInventory < UserDataManager.Inst.GetServantCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Servant Inventory is Full");
            return;
        }
        else if (inventoryInfo.monsterInventory < UserDataManager.Inst.GetMonsterCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Monster Inventory is Full");
            return;
        }
        else if (inventoryInfo.equipmentInventory < UserDataManager.Inst.GetEquipmentCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Equipment Inventory is Full");
            return;
        }
        else
        {

#if UNITY_EDITOR
            {
                Cheat.Inst.RequestStageStartCheat(UserDataManager.Inst.stageState.stageType, UserDataManager.Inst.stageState.stageFloor, UserDataManager.Inst.stageState.stageDifficult, 1);
                UserDataManager.Inst.stageState = new UserStageStateData();
                UserDataManager.Inst.stageActionInfo = new battleActionData();
                UserDataManager.Inst.stageReward = new stageRewardData();
            }
#else
        {
            PacketManager.Inst.RequestStageStart(UserDataManager.Inst.stageState.stageType, UserDataManager.Inst.stageState.stageFloor, UserDataManager.Inst.stageState.stageDifficult,1);
            UserDataManager.Inst.stageState = new UserStageStateData();
            UserDataManager.Inst.stageActionInfo = new battleActionData();
            UserDataManager.Inst.stageReward = new stageRewardData();
        }
#endif
            Time.timeScale = 2;
            BattleManager.Inst.battleFail?.SetActive(false);
            BattleManager.Inst.rewardParent?.SetActive(false);
        }
    }
}
