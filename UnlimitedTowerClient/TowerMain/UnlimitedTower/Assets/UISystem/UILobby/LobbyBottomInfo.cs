using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyBottomInfo : MonoBehaviour {

    public GameObject gachaScene;

    public void OnClickStageStartButton()
    {
        // 임시로 치트키 추가
        Cheat.Inst.SetStageStartCheat();
    }

    public void OnClickGachaButton()
    {
        gachaScene.SetActive(true);
    }
}
