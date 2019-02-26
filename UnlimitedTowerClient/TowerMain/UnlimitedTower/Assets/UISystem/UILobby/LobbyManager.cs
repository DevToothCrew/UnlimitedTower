using UnityEngine.SceneManagement;
using UnityEngine;

#region Extensions 

static public class ActiveAnimationManager
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

    public GameObject centerUI;
    public GameObject topUI;
    public GameObject leftUI;
    public GameObject bottomUI;
    public GameObject rightUI;
    public GameObject chatUI;

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
                leftUI.SetActivateWithAnimation(true);
                centerUI.SetActivateWithAnimation(true);
                topUI.SetActivateWithAnimation(true);
                bottomUI.SetActivateWithAnimation(true);
                rightUI.SetActivateWithAnimation(true);
                chatUI.SetActivateWithAnimation(true);
                break;

            case SCENE_STATE.Login:
                break;
        }
    }
}
