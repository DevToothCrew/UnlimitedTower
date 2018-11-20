using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// TODO Test Class: if deleted
public class TestResultButton : MonoBehaviour
{

    private void OnGUI()
    {
        // 배틀 끝났을 때 뜨는 버튼.
        //if (BattleManager.Inst.BattleState == BATTLE_STATE.END)
        {
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, 100, 70), "GameOver"))
            {
                BattleManager.Inst.BattleState = BATTLE_STATE.NONE;
                SceneManager.LoadScene("Lobby");                      
            }
        }
    }
}
