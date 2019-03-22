using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoSingleton<BattleUIManager> {
    public GameObject delayImage;

    private void Awake()
    {
        delayImage = GameObject.Find("DelayImage");
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

    public void BattleOut()
    {

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
