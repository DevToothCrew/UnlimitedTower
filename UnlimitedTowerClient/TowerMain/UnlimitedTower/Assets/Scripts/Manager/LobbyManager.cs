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

    public GameObject LoginPopup;
    public GameObject RightPopup;
    public GameObject TowerGrid;
    public GameObject TowerInfo;

    public GameObject CenterPopup;
    public LOBBY_RIGHT_BUTTON centerPopupState;

    public GameObject HeroInfoPopup;
    public GameObject PartnerInfoPopup;
    public GameObject FormationInfoPopup;
    public GameObject GachaInfoPopup;
    public GameObject StageInfoPopup;
    public GameObject InventoryInfoPopup;
    public GameObject SettingInfoPopup;

    public GameObject GachaScene;

    public GameObject LeftPopup;
    public GameObject BaseBackground;

    public GameObject FormationList;

    public void Awake()
    {
        InitCenterPopup();
        SCENE_STATE state = UserDataManager.Inst.GetSceneState();
        if(state == SCENE_STATE.None)
        {
            state = SCENE_STATE.Login;
            UserDataManager.Inst.SetSceneState(state);
        }
        ChangeSceneState(state);
    }

    public void ChangeSceneState(SCENE_STATE state)
    {
        UserDataManager.Inst.SetSceneState(state);

        switch (state)
        {
            case SCENE_STATE.Login:
                TowerGrid.SetActivateWithAnimation(true);
                TowerInfo.SetActivateWithAnimation(false);
                LeftPopup.SetActivateWithAnimation(false);
                RightPopup.SetActivateWithAnimation(false);
                CenterPopup.SetActivateWithAnimation(false);
                LoginPopup.SetActivateWithAnimation(true);
                break;

            case SCENE_STATE.Lobby:
                SetTowerInfo(true, 100.0f);
                SetRightPopup(RightPopup);
                LeftPopup.SetActivateWithAnimation(true);
                break;

            case SCENE_STATE.Stage:
                SetTowerInfo(false, 100.0f);
                SetRightPopup(RightPopup);
                break;
        }

        InitCenterPopup();
    }

    public void SetTowerInfo(bool active, float xPos)
    {
        TowerGrid.SetActivateWithAnimation(active);
        TowerInfo.SetActivateWithAnimation(active);
    }
    
    public void SetRightPopup(GameObject obj)
    {
        LoginPopup.SetActivateWithAnimation(false);
        RightPopup.SetActivateWithAnimation(false);
        CenterPopup.SetActivateWithAnimation(false);

        obj.SetActivateWithAnimation(true);
    }

    public void InitCenterPopup()
    {
        centerPopupState = LOBBY_RIGHT_BUTTON.None;   
       
        CenterPopup.SetActivateWithAnimation(false);
        HeroInfoPopup.SetActivateWithAnimation(false);
        PartnerInfoPopup.SetActivateWithAnimation(false);
        FormationInfoPopup.SetActivateWithAnimation(false);
        GachaInfoPopup.SetActivateWithAnimation(false);
        StageInfoPopup.SetActivateWithAnimation(false);
        InventoryInfoPopup.SetActivateWithAnimation(false);
        SettingInfoPopup.SetActivateWithAnimation(false);
    }

    public void OnClickRightButton(int rightButton)
    {
        if (centerPopupState == LOBBY_RIGHT_BUTTON.None)
        {
            TowerInfo.SetActivateWithAnimation(false);
            TowerGrid.SetActivateWithAnimation(false);
        }
        else
        {
            TowerInfo.SetActive(false);
            TowerGrid.SetActive(false);
        }

        InitCenterPopup();
        CenterPopup.SetActivateWithAnimation(true);


        centerPopupState = (LOBBY_RIGHT_BUTTON)rightButton;

        switch (centerPopupState)
        {
            case LOBBY_RIGHT_BUTTON.Hero:
                HeroInfoPopup.SetActivateWithAnimation(true);
                break;

            case LOBBY_RIGHT_BUTTON.Partner:
                PartnerInfoPopup.SetActivateWithAnimation(true);
                break;

            case LOBBY_RIGHT_BUTTON.Formation:
                FormationInfoPopup.SetActivateWithAnimation(true);
                break;

            case LOBBY_RIGHT_BUTTON.Gacha:
                GachaInfoPopup.SetActivateWithAnimation(true);
                break;

            case LOBBY_RIGHT_BUTTON.Stage:
                StageInfoPopup.SetActivateWithAnimation(true);
                break;

            case LOBBY_RIGHT_BUTTON.Inventory:
                InventoryInfoPopup.SetActivateWithAnimation(true);
                break;

            case LOBBY_RIGHT_BUTTON.Setting:
                SettingInfoPopup.SetActivateWithAnimation(true);
                break;
        }
    }

    public void OnClickExitCenterPopup()
    {
        InitCenterPopup();
        TowerInfo.SetActivateWithAnimation(true);
        TowerGrid.SetActivateWithAnimation(true);
    }

    public void EnterGachaScene(int gachaNum)
    {
        GachaScene.SetActivateWithAnimation(true);

        RightPopup.SetActivateWithAnimation(false);
        TowerGrid.SetActivateWithAnimation(false);
        CenterPopup.SetActivateWithAnimation(false);
        LeftPopup.SetActivateWithAnimation(false);
        BaseBackground.SetActivateWithAnimation(false);
    }

    public void OnClickExitGacha()
    {
        GachaScene.SetActivateWithAnimation(false);

        RightPopup.SetActivateWithAnimation(true);
        TowerGrid.SetActivateWithAnimation(true);
        TowerInfo.SetActivateWithAnimation(true);

        LeftPopup.SetActivateWithAnimation(true);
        BaseBackground.SetActivateWithAnimation(true);
    }
}
