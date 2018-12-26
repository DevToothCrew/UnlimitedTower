using System;
using UnityEngine;

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
    public GameObject SettingInfoPopup;

    public GameObject GachaScene;

    public GameObject LeftPop;
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
                SetTowerInfo(true, -100.0f);
                SetRightPopup(LoginPopup);
                TowerInfo.SetActive(false);
                break;
            case SCENE_STATE.Lobby:
                SetTowerInfo(true, 100.0f);
                SetRightPopup(RightPopup);
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
        TowerGrid.SetActive(active);
        TowerInfo.SetActive(active);
    }
    
    public void SetRightPopup(GameObject obj)
    {
        LoginPopup.SetActive(false);
        RightPopup.SetActive(false);
        CenterPopup.SetActive(false);

        obj.SetActive(true);
    }

    public void InitCenterPopup()
    {
        centerPopupState = LOBBY_RIGHT_BUTTON.None;   
       
        CenterPopup.SetActive(false);
        HeroInfoPopup.SetActive(false);
        PartnerInfoPopup.SetActive(false);
        FormationInfoPopup.SetActive(false);
        GachaInfoPopup.SetActive(false);
        StageInfoPopup.SetActive(false);
        SettingInfoPopup.SetActive(false);
    }

    public void OnClickRightButton(int rightButton)
    {
        InitCenterPopup();
        CenterPopup.SetActive(true);
        TowerInfo.SetActive(false);
        TowerGrid.SetActive(false);

        centerPopupState = (LOBBY_RIGHT_BUTTON)rightButton;

        switch (centerPopupState)
        {
            case LOBBY_RIGHT_BUTTON.Hero:
                HeroInfoPopup.SetActive(true);
                break;

            case LOBBY_RIGHT_BUTTON.Partner:
                PartnerInfoPopup.SetActive(true);
                break;

            case LOBBY_RIGHT_BUTTON.Formation:
                FormationInfoPopup.SetActive(true);
                break;

            case LOBBY_RIGHT_BUTTON.Gacha:
                GachaInfoPopup.SetActive(true);
                break;

            case LOBBY_RIGHT_BUTTON.Stage:
                StageInfoPopup.SetActive(true);
                break;

            case LOBBY_RIGHT_BUTTON.Setting:
                SettingInfoPopup.SetActive(true);
                break;
        }
    }

    public void OnClickExitCenterPopup()
    {
        InitCenterPopup();
        TowerInfo.SetActive(true);
        TowerGrid.SetActive(true);
    }

    public void EnterGachaScene(int gachaNum)
    {
        GachaScene.SetActive(true);

        RightPopup.SetActive(false);
        TowerGrid.SetActive(false);
        CenterPopup.SetActive(false);
        LeftPop.SetActive(false);
        BaseBackground.SetActive(false);
    }

    public void OnClickExitGacha()
    {
        GachaScene.SetActive(false);

        RightPopup.SetActive(true);
        TowerGrid.SetActive(true);
        TowerInfo.SetActive(true);

        LeftPop.SetActive(true);
        BaseBackground.SetActive(true);
    }
}
