using UnityEngine.UI;
using UnityEngine;

public class LobbyManager : MonoSingleton<LobbyManager> {
    //Load Main Unit Model 
    public CharacterCustom characterCustom;
    private GameObject objServantModel = null;
    private GameObject objMonsterModel = null;

    // Idle UI
    public GameObject centerUI;
    public GameObject topUI;
    public GameObject leftUI;
    public GameObject bottomUI;
    public GameObject rightUI;
    public GameObject chatUI;

    // Low UI
    public GameObject accountInfoUI;
    public GameObject BackbuttonUI;

    public Text textBackButton;

    public POPUP_STATE popupState;
    public GameObject[] popupPage = new GameObject[1];

    // 추후 추가할때마다 숫자 변경
    readonly public int popupCount = 1;

    //Running sub view
    public GameObject objSubView;


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
        accountInfoUI.SetActive(true);
        BackbuttonUI.SetActive(false);
        ChangeMainCharacterModel();
        Time.timeScale = 1.0f;
    }

    //메인 캐릭터 모델 표시
    public void ChangeMainCharacterModel()
    {
        if (objMonsterModel != null)
        {
            Destroy(objMonsterModel);
        }

        if (UserDataManager.Inst.GetMainCharInfo().mainCharType == CHAR_TYPE.SERVANT)
        {
            int servant_id = UserDataManager.Inst.GetMainCharInfo().mainCharID;
            characterCustom.jobIndex = CSVData.Inst.GetServantData(servant_id).job;
            characterCustom.headIndex = CSVData.Inst.GetServantData(servant_id).head;
            characterCustom.hairIndex = CSVData.Inst.GetServantData(servant_id).hair;
            characterCustom.isMan = CSVData.Inst.GetServantData(servant_id).gender;
            characterCustom.isChildren = CSVData.Inst.GetServantData(servant_id).body;
            characterCustom.Refresh();
            characterCustom.gameObject.SetActive(true);
        }
        else if (UserDataManager.Inst.GetMainCharInfo().mainCharType == CHAR_TYPE.MONSTER)
        {
            characterCustom.gameObject.SetActive(false);
            objMonsterModel = Instantiate(Resources.Load<GameObject>("InGameCharacterPrefabs/" + CSVData.Inst.GetMonsterDBResourceModel(UserDataManager.Inst.GetMainCharInfo().mainCharID)),
                    characterCustom.transform);
        }

        
    }

    //PartyInfo 화면에서 유닛을 선택했을때 해당 모델로 교체
    public void ChangeSelectedUnitModel()
    {
        Debug.Log("change update model");
        if (PartyInfoVC.checkInst())
        {
            PartyInfoVC partyInfo = PartyInfoVC.Inst;

            if (objMonsterModel != null)
            {
                Destroy(objMonsterModel);
            }

            if (partyInfo.selectedMenu == PartyInfoVC.menu_type.SERVANT)
            {
                characterCustom.gameObject.SetActive(false);
                int servant_id = partyInfo.ServantList[partyInfo.selected_unit_idx].id;
                characterCustom.jobIndex = CSVData.Inst.GetServantData(servant_id).job;
                characterCustom.headIndex = CSVData.Inst.GetServantData(servant_id).head;
                characterCustom.hairIndex = CSVData.Inst.GetServantData(servant_id).hair;
                characterCustom.isMan = CSVData.Inst.GetServantData(servant_id).gender;
                characterCustom.isChildren = CSVData.Inst.GetServantData(servant_id).body;
                characterCustom.Refresh();
                characterCustom.gameObject.SetActive(true);
            }
            else if (partyInfo.selectedMenu == PartyInfoVC.menu_type.MONSTER)
            {
                characterCustom.gameObject.SetActive(false);
                objMonsterModel = Instantiate(Resources.Load<GameObject>("InGameCharacterPrefabs/" + CSVData.Inst.GetMonsterDBResourceModel(partyInfo.MonsterList[partyInfo.selected_unit_idx].id)),
                        characterCustom.transform);
            }
        }
        


    }

    public void ChangeSceneState(SCENE_STATE state)
    {
        switch (state)
        {
            case SCENE_STATE.Lobby:
                EtcSetActiveWithAnimation(true);
                topUI.SetActivateWithAnimation(true);
                chatUI.SetActivateWithAnimation(true);
                accountInfoUI.SetActive(true);
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
        switch ((POPUP_STATE)num)
        {
            case POPUP_STATE.Servant:
                {
                    SetTextBackButton("Party");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/PartyInfoVC")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    BackbuttonUI.SetActive(true);
                }
                break;

            case POPUP_STATE.Weapon:
                {
                    SetTextBackButton("Inventory");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/InventoryVC")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    BackbuttonUI.SetActive(true);
                }
                break;

            case POPUP_STATE.EOS:
                {
                    SetTextBackButton("Shop");
                }
                break;

            case POPUP_STATE.Gacha:
                {
                    SetTextBackButton("Gacha");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/GachaScene")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    BackbuttonUI.SetActive(true);
                }
                break;

            case POPUP_STATE.Stage:
                {
                    SetTextBackButton("Stage");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/StageScene")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    BackbuttonUI.SetActive(true);
                }
                break;

            default:
                return;
        }

        popupState = (POPUP_STATE)num;
        EtcSetActiveWithAnimation(false);
        accountInfoUI.SetActive(false);
        BackbuttonUI.SetActive(true);
    }

    public void OnClickTopButton(int num)
    {
        switch ((POPUP_STATE)num)
        {
            case POPUP_STATE.AccountInfo:
                {
                    if (UserDataManager.Inst.GetUserInfo() == null)
                    {
                        CSVData.Inst.InitCSVData();
                        return;
                    }
                    else
                    {
                        SetTextBackButton("Account Info");
                        objSubView = Instantiate(Resources.Load("UI/Lobby/AccountInfoPage")) as GameObject;
                        objSubView.transform.SetParent(this.transform);
                        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                        BackbuttonUI.SetActive(true);
                    }
                }
                break;

            case POPUP_STATE.Mail:
                {
                    SetTextBackButton("Mail");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/MailInfoPage")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -45); 

                    BackbuttonUI.SetActive(true);
                }
                break;

            case POPUP_STATE.Setting:
                {
                    SetTextBackButton("Setting");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/MailInfoPage")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    BackbuttonUI.SetActive(true);
                }
                break;

            case POPUP_STATE.PVP:
                {
                    SetTextBackButton("PVP");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/PVPInfoPage")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    BackbuttonUI.SetActive(true);
                }
                break;

            default:
                return;
        }
    }

    public void OnClickBackButton()
    {
        if (objSubView != null)
            Destroy(objSubView);

        EtcSetActiveWithAnimation(true);

        accountInfoUI.SetActive(true);
        BackbuttonUI.SetActive(false);
    }

    public void SetTextBackButton(string text)
    {
        textBackButton.text = text;
    }

}