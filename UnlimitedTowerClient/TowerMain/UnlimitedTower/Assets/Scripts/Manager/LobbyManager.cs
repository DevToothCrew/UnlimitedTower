using System.Collections;
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


    // TODO : TestCode
    public GameObject GachaScene;

    public GameObject LeftPop;
    public GameObject BaseBackground;

    public GameObject CharacterListScroll;
    public GameObject ServantContentList;
    public GameObject MonsterContentList;

    public GameObject FormationList;




    public void Awake()
    {
        InitCenterPopup();
        if (!UserDataManager.Inst.UserLoginFlag)
        {
            Debug.Log("첫 로그인 화면");        
            ChangeSceneState(SCENE_STATE.Login);
        }
        else
        {
            FormationManager.Inst.BeSaved = false;
            UserDataManager.Inst.oldFormationDic = UserDataManager.Inst.formationDic;
            Debug.Log("로비로 리턴");
         
            ChangeSceneState(SCENE_STATE.Lobby);
            UserDataManager.Inst.LoadUserData();
        }       
    }


    public void ChangeSceneState(SCENE_STATE state)
    {
        //TODO :테스트 코드 필요없을시 삭제.
        UserDataManager.Inst.sceneState = state;

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

        //MonsterContentList.SetActive(false);
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
        //TODO : 임시 코드. 필요없을시 삭제.
        // ### 로그인 패킷을 보낸다.

#if UNITY_EDITOR
        UserDataManager.Inst.Test_InitCharacter();
        PacketManager.Inst.Request_GetLobbyInfo();
        Test_PacketManager.Inst.CheckPacket("OnClickEnterLobbyButton : not recive packet");
#else
        PacketManager.Inst.Request_Login();
         PacketManager.Inst.Request_GetLobbyInfo();
        Test_PacketManager.Inst.CheckPacket("OnClickEnterLobbyButton : not recive packet");
  
#endif

    }

    //###
    public void OnClickLogoutButton()
    {
        PacketManager.Inst.Request_Logout();
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
                OnClickFormationServantButton();
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

    public void EnterGachaScene(int gachaNum)
    {
        GachaScene.SetActive(true);

        RightPopup.SetActive(false);
        TowerGrid.SetActive(false);
        CenterPopup.SetActive(false);
        LeftPop.SetActive(false);
        BaseBackground.SetActive(false);
    }

    public void OnClickExitCenterPopup()
    {
        InitCenterPopup();
        TowerInfo.SetActive(true);
        TowerGrid.SetActive(true);
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
    public void OnClickFormationServantButton()
    {
        ServantContentList.SetActive(true);
        MonsterContentList.SetActive(false);
        CharacterListScroll.GetComponent<ScrollRect>().content = ServantContentList.gameObject.GetComponent<RectTransform>();
       
#if UNITY_EDITOR
     
#else
         PacketManager.Inst.Request_AllServant();  
#endif

    }

    public void OnClickFormationMonsterButton()
    {
        MonsterContentList.SetActive(true);
        ServantContentList.SetActive(false);
        CharacterListScroll.GetComponent<ScrollRect>().content = MonsterContentList.gameObject.GetComponent<RectTransform>();

#if UNITY_EDITOR

#else
            PacketManager.Inst.Request_AllMonster();
#endif
    }



    public void OnClickStageButton(int stageNum)
    {
        // ### 스테이지 넘어가는 패킷을 보낸대.(전투씬으로감)

        if (FormationManager.Inst.BeSaved == false)
        {
            UserDataManager.Inst.SetOldFormation();
        }

        Debug.Log("OnClickStageButton : " + stageNum);
        PacketManager.Inst.Request_GetStageInfo(stageNum);
    }

    public void EnterStage(int stageNum)
    {
        PacketManager.Inst.Request_EnterStage(stageNum);
    }
}
