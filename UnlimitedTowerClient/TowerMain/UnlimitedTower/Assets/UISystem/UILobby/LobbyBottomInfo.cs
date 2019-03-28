using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyBottomInfo : MonoBehaviour {

    public GameObject gachaScene;

    public void OnClickStageStartButton()
    {
        // 현재 스테이지 1,1을 바로 시작하지만 스테이지창이 추가 필요


#if UNITY_EDITOR
        {
            Cheat.Inst.RequestStageStartCheat(1, Random.Range(1,11), 1);
        }
        #else
        {
            PacketManager.Inst.RequestStageStart(1, 1);
        }
#endif
    }

    public void OnClickGachaButton()
    {
        gachaScene.SetActive(true);
    }
}
