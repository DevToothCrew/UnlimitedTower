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
    public Text popupTitle;
    public Text popupMenu;

    public POPUP_STATE popupState;
    public GameObject heroMenu;
    public GameObject shopMenu;

    public void Awake()
    {
        SCENE_STATE state = UserDataManager.Inst.GetSceneState();
        if(state == SCENE_STATE.None)
        {
            state = SCENE_STATE.Lobby;
            UserDataManager.Inst.SetSceneState(state);
        }

        EtcSetActiveWithAnimation(true);
        topUI.SetActivateWithAnimation(true);
        chatUI.SetActivateWithAnimation(true);
        popupUI.SetActive(false);
        accountInfoUI.SetActive(true);
        popupInfoUI.SetActive(false);

        popupState = POPUP_STATE.Hero;
        heroMenu.SetActive(true);
        shopMenu.SetActive(false);
    }

    public void ChangeSceneState(SCENE_STATE state)
    {
        switch (state)
        {
            case SCENE_STATE.Lobby:
                EtcSetActiveWithAnimation(true);
                topUI.SetActivateWithAnimation(true);
                chatUI.SetActivateWithAnimation(true);
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
        SetPopupTitle("Party");
        SetPopupMenu("Hero");

        EtcSetActiveWithAnimation(false);

        popupUI.SetActive(true);
        accountInfoUI.SetActive(false);
        popupInfoUI.SetActive(true);

        heroMenu.SetActive(true);
        shopMenu.SetActive(false);
    }

    public void OnClickShopButton()
    {
        SetPopupTitle("Store");
        SetPopupMenu("EOS");

        EtcSetActiveWithAnimation(false);

        popupUI.SetActive(true);
        accountInfoUI.SetActive(false);
        popupInfoUI.SetActive(true);

        heroMenu.SetActive(false);
        shopMenu.SetActive(true);
    }

    public void OnClickBackButton()
    {
        EtcSetActiveWithAnimation(true);
        popupUI.SetActive(false);

        accountInfoUI.SetActive(true);
        popupInfoUI.SetActive(false);
    }

    public void SetPopupTitle(string text)
    {
        popupTitle.text = text;
    }

    public void SetPopupMenu(string text)
    {
        popupMenu.text = text;
    }

    public void OnClickMenu(int getState)
    {
        // 버튼과 현재 상태가 다를 경우만 반응
        if (popupState != (POPUP_STATE)getState)
        {
            popupState = (POPUP_STATE)getState;
            SetPopupMenu(popupState.ToString());
        }
    }
}

public enum POPUP_STATE
{
    None = 0,

    // Hero
    Hero = 1,
    Servant = 2,
    Monster = 3,
    Item = 4,
    Formation = 5,

    // Shop
    EOS = 11,
    UTG = 12,
    Gacha = 13,
}