﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoSingleton<LobbyManager> {

    public GameObject LoginPopup;
    public GameObject RightPopup;
    public GameObject TowerGrid;
    public GameObject TowerInfo;

    public GameObject CenterPopup;
    private LOBBY_RIGHT_BUTTON centerPopupState;

    public GameObject HeroInfoPopup;
    public GameObject PartnerInfoPopup;
    public GameObject FormationInfoPopup;
    public GameObject GachaInfoPopup;
    public GameObject StageInfoPopup;
    public GameObject SettingInfoPopup;

    public void Awake()
    {
        InitCenterPopup();
        ChangeSceneState(SCENE_STATE.Login);
    }
    public void ChangeSceneState(SCENE_STATE state)
    {
        switch (state)
        {
            case SCENE_STATE.Login:
                SetTowerInfo(true, -100.0f);
                SetRightPopup(LoginPopup);
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
        // 그리드 세팅도 여기에 넣는다
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

    public void OnClickLoginButton()
    {
        PacketManager.Inst.Request_Login();        
    }

    public void OnClickCreatePlayerButton()
    {
        PacketManager.Inst.Request_CreatePlayer();
    }

    public void OnClickEnterLobbyButton()
    {
        PacketManager.Inst.Request_GetLobbyInfo();
    }

    public void OnClickLogoutButton()
    {
        PacketManager.Inst.Request_Logout();
    }

    public void OnClickRightButton(int rightButton)
    {
        InitCenterPopup();
        CenterPopup.SetActive(true);

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
    }

    public void OnClickStageButton(int stageNum)
    {
        PacketManager.Inst.Request_GetStageInfo(stageNum);
    }

    public void EnterStage(int stageNum)
    {
        PacketManager.Inst.Request_EnterStage(stageNum);
    }
}
