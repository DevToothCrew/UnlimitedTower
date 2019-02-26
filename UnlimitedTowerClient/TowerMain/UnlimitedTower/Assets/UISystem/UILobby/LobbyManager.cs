using UnityEngine.UI;
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

    // Idle UI
    public GameObject centerUI;
    public GameObject topUI;
    public GameObject leftUI;
    public GameObject bottomUI;
    public GameObject rightUI;
    public GameObject chatUI;

    // Low UI
    public GameObject popupUI;
    public GameObject accountInfoUI;
    public GameObject popupInfoUI;
    public Text popupName;

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
        switch (state)
        {
            case SCENE_STATE.Lobby:
                topUI.SetActivateWithAnimation(true);
                chatUI.SetActivateWithAnimation(true);
                EtcSetActiveWithAnimation(true);
                popupUI.SetActive(false);
                accountInfoUI.SetActive(true);
                popupInfoUI.SetActive(false);
                break;

            case SCENE_STATE.Login:
                break;
        }
    }

    public void EtcSetActiveWithAnimation(bool isTrue)
    {
        leftUI.SetActivateWithAnimation(isTrue);
        centerUI.SetActivateWithAnimation(isTrue);
        bottomUI.SetActivateWithAnimation(isTrue);
        rightUI.SetActivateWithAnimation(isTrue);
    }

    public void OnClickPartyButton()
    {
        popupName.text = "PartyInfo";

        EtcSetActiveWithAnimation(false);

        popupUI.SetActive(true);
        accountInfoUI.SetActive(false);
        popupInfoUI.SetActive(true);
    }

    public void OnClickShopButton()
    {
        popupName.text = "ShopInfo";

        EtcSetActiveWithAnimation(false);

        popupUI.SetActive(true);
        accountInfoUI.SetActive(false);
        popupInfoUI.SetActive(true);
    }

    public void OnClickBackButton()
    {
        ChangeSceneState(SCENE_STATE.Lobby);
    }
}
