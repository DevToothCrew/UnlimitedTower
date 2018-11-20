using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// TODO Test Class: if deleted
public class TestResultButton : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnGUI()
    {
        //if (BattleManager.Inst.BattleState == BATTLE_STATE.END)
        {
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, 100, 70), "GameOver"))
            {
                BattleManager.Inst.BattleState = BATTLE_STATE.NONE;
                //LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Lobby);
                SceneManager.LoadScene("Lobby");
                
               // LobbyManager.Inst.InitCenterPopup();
                //LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Lobby);
                //LobbyManager.Inst.OnClickEnterLobbyButton();
                //Debug.Log("로비로 다시 전환");



            }




        }
    }
}
