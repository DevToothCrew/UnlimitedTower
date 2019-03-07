﻿using UnityEngine.UI;
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
    public GameObject[] popupPage = new GameObject[3];

    public void OnEnable()
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
        for (int i = 0; i < 3; i++)
        {
            popupPage[i].SetActive(false);
        }

        Time.timeScale = 1.0f;
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

    public void OnClickLeftButton(int num)
    {
        switch((POPUP_STATE)num)
        {
            case POPUP_STATE.Hero:
                {
                    SetPopupTitle("Party");
                    SetPopupMenu(num);
                }
                break;

            case POPUP_STATE.Weapon:
                {
                    SetPopupTitle("Inventory");
                    SetPopupMenu(num);
                }
                break;

            case POPUP_STATE.EOS:
                {
                    SetPopupTitle("Shop");
                    SetPopupMenu(num);
                }
                break;

            default:
                return;
        }

        EtcSetActiveWithAnimation(false);

        popupUI.SetActive(true);
        accountInfoUI.SetActive(false);
        popupInfoUI.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            if (i == num / 10)
            {
                popupPage[i].SetActive(true);
            }
            else
            {
                popupPage[i].SetActive(false);
            }
        }
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

    public void SetPopupMenu(int pageNum)
    {
        popupMenu.text = ((POPUP_STATE)pageNum).ToString();
    }

    public void OnClickMenu(int getState)
    {
        // 버튼과 현재 상태가 다를 경우만 반응
        if (popupState != (POPUP_STATE)getState)
        {
            popupState = (POPUP_STATE)getState;
            SetPopupMenu(getState);
        }
    }
}

public enum POPUP_STATE
{
    None = 0,

    // Hero
    Hero        = 1,
    Servant     = 2,
    Monster     = 3,
    Formation   = 4,

    // Inventory
    Weapon      = 11,
    Armor       = 12,
    Accesory    = 13,
    Etc         = 14,

    // Shop
    EOS     = 21,
    UTG     = 22,
    Gacha   = 23,
}