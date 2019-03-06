using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleUIManager : MonoBehaviour {
    public void Start()
    {
    }

    public void BattleLeave()
    {
        // 나가기 버튼
        PacketManager.Inst.RequestLoginWithScatter();
    }

    public void TimeScaleX10()
    {
        // 배속 올리거나 줄이시려면 Speed를 바꿔주세요!
        int Speed = 10;
        if (BattleSystem.Inst.TimeScale != Speed)
        {
            BattleSystem.Inst.TimeScale = Speed;
            Time.timeScale = Speed;
        }
        else
        {
            BattleSystem.Inst.TimeScale = 1;
            Time.timeScale = 1;
        }
    }
    
}
