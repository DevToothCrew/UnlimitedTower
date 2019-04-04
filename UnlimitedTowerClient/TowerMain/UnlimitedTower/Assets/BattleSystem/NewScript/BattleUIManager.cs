using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleUIManager : MonoSingleton<BattleUIManager> {
    public GameObject delayImage;

    public Image StageInfoBack;
    public Image StageInfoLine;
    public Text StageInfoFloor;
    public Text StageInfoName;
    public Text BattleTurn;

    public GameObject rewardWindows;

    public GameObject battleOutCheck;

    private float timeScale;

    private void Awake()
    {
        delayImage = GameObject.Find("DelayImage");
        delayImage.SetActive(false);
        
        StageInfoBack = GameObject.Find("Stage Info Back").GetComponent<Image>();
        StageInfoLine = GameObject.Find("Stage Info Line").GetComponent<Image>();
        StageInfoFloor = GameObject.Find("Stage Info Floor").GetComponent<Text>();
        StageInfoName = GameObject.Find("Stage Info Name").GetComponent<Text>();
        BattleTurn = GameObject.Find("Battle Turn Text").GetComponent<Text>();

        battleOutCheck = GameObject.Find("Battle Exit Check Window");
        battleOutCheck.SetActive(false);

        StageInfoOn();
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
    
    public void OnDelay()
    {
        delayImage.SetActive(true);
    }

    public void OffDelay()
    {
        delayImage.SetActive(false);
    }

    public void BattleActionOut()
    {
        PacketManager.Inst.RequestStageExit();
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
}
