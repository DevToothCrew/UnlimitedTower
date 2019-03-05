using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour {
    private GameObject testReTageting;

    public void Start()
    {
        testReTageting = GameObject.Find("죽은대상");
        testReTageting.SetActive(false);
    }

    public void BattleLeave()
    {
        // 나가기 버튼
        Debug.Log("나가기 버튼 클릭");
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

    public void DieSelect()
    {
        StartCoroutine(TestReTargeting());
    }

    IEnumerator TestReTargeting()
    {
        testReTageting.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        testReTageting.SetActive(false);
    }
}
