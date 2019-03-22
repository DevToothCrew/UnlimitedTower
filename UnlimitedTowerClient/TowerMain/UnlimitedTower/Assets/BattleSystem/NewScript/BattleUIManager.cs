using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoSingleton<BattleUIManager> {
    public GameObject delayImage;
    public GameObject myTrun;

    private void Awake()
    {
        delayImage = GameObject.Find("DelayImage");
        delayImage.SetActive(false);

        myTrun = GameObject.Find("마이턴");
        myTrun.SetActive(false);
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

    public void MyTurn()
    {
        StartCoroutine(MyTurnCo());
    }

    private IEnumerator MyTurnCo()
    {
        myTrun.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        myTrun.SetActive(false);
    }

    public void OnDelay()
    {
        delayImage.SetActive(true);
    }

    public void OffDelay()
    {
        delayImage.SetActive(false);
    }
}
