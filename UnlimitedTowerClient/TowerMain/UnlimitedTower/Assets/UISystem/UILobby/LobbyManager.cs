using System;
using UnityEngine;

#region Extensions 

static public class LobbyAnimationManager
{
    static public void SetActivateWithAnimation(this GameObject uiGO, bool value)
    {
        var animator = uiGO.GetComponent<Animator>();
        if (animator != null)
        {
            uiGO.SetActive(true);
            animator.SetTrigger(value ? "SetVisible" : "SetInvisible");
        }
        else
        {
            uiGO.SetActive(value);
        }
    }
}
#endregion

public class LobbyManager : MonoSingleton<LobbyManager> {

    public GameObject CenterUI;
    public GameObject TopUI;
    public GameObject LeftUI;
    public GameObject BottomUI;

    public void Awake()
    {
        SCENE_STATE state = UserDataManager.Inst.GetSceneState();
        if(state == SCENE_STATE.None)
        {
            state = SCENE_STATE.Lobby;
            UserDataManager.Inst.SetSceneState(state);
        }
        ChangeSceneState(state);
    }

    public void ChangeSceneState(SCENE_STATE state)
    {
        UserDataManager.Inst.SetSceneState(state);

        switch (state)
        {
            case SCENE_STATE.Lobby:
                LeftUI.SetActivateWithAnimation(true);
                CenterUI.SetActivateWithAnimation(true);
                TopUI.SetActivateWithAnimation(true);
                BottomUI.SetActivateWithAnimation(true);
                break;
        }
    }
}
