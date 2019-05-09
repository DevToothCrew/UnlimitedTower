using UnityEngine.UI;
using UnityEngine;

public class LobbyManager : MonoSingleton<LobbyManager> {
    // Idle UI
    public GameObject centerUI;
    public GameObject topUI;
    public GameObject upperUI;  //최상단 화면 전체 덮어야 하는 화면이 위치할(Parent) 곳.(Delay Image 보다는 아래임)
    public GameObject leftUI;
    public GameObject bottomUI;
    public GameObject rightUI;
    public GameObject chatUI;

    // Low UI
    public GameObject accountInfoUI;
    public GameObject BackbuttonUI;

    public Image imageBackButtonIcon;
    public Text textBackButton;

    public POPUP_STATE popupState;
    public GameObject[] popupPage = new GameObject[1];

    // 추후 추가할때마다 숫자 변경
    readonly public int popupCount = 1;

    //Running sub view
    public GameObject objSubView;

    // Sound
    public GameObject LobbySound;
    public GameObject GachaSound;

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
        //chatUI.SetActivateWithAnimation(true); //채팅창 임시 주석
        accountInfoUI.SetActive(true);
        BackbuttonUI.SetActive(false);
        ModelViewManager.Inst.ChangeMainCharacterModel();
        Time.timeScale = 1.0f;

        LobbySound.SetActive(true);
        GachaSound.SetActive(false);
    }

    public void ChangeSceneState(SCENE_STATE state)
    {
        switch (state)
        {
            case SCENE_STATE.Lobby:
                EtcSetActiveWithAnimation(true);
                topUI.SetActivateWithAnimation(true);
                //chatUI.SetActivateWithAnimation(true); //채팅창 임시 주석
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
        popupState = (POPUP_STATE)num;
        SetImageIconBackButton(popupState);
        switch ((POPUP_STATE)num)
        {
            case POPUP_STATE.Servant:
                {
                    SetTextBackButton("Party");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/PartyInfoVC")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    BackbuttonUI.SetActive(true);
                    imageBackButtonIcon.sprite = Resources.Load<Sprite>("UI/Lobby/Image/icon_party_info");
                }
                break;

            case POPUP_STATE.Weapon:
                {
                    SetTextBackButton("Inventory");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/InventoryVC")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    BackbuttonUI.SetActive(true);
                    imageBackButtonIcon.sprite = Resources.Load<Sprite>("UI/Lobby/Image/icon_inventory");
                }
                break;

            case POPUP_STATE.EOS:
                {
                    SetTextBackButton("Shop");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/ShopInfoVC")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    BackbuttonUI.SetActive(true);
                    imageBackButtonIcon.sprite = Resources.Load<Sprite>("UI/Lobby/Image/icon_shop");

                    PacketManager.Inst.RequestLocalShopInfo(SHOP_TYPE.EOS);
                }
                break;

            case POPUP_STATE.Gacha:
                {
                    SetTextBackButton("Gacha");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/GachaScene")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    BackbuttonUI.SetActive(true);

                    LobbySound.SetActive(false);
                    GachaSound.SetActive(true);
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
                DebugLog.Log(false, "Invalid State : " + num.ToString());
                popupState = POPUP_STATE.Servant;
                return;
        }

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
                        if (AccountInfoPage.checkInst())
                        {
                            DebugLog.Log(false, "running Account Info Page");
                        }
                        else
                        {
                            SetTextBackButton("Account Info");
                            GameObject objAccountInfoPage = Instantiate(Resources.Load("UI/Lobby/AccountInfoPage")) as GameObject;
                            objAccountInfoPage.transform.SetParent(upperUI.transform);
                            objAccountInfoPage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        }
                        
                    }
                }
                break;

            case POPUP_STATE.Mail:
                {
                    if (MailInfoPage.checkInst())
                    {
                        DebugLog.Log(false, "running Mail Info Page");
                    }
                    else
                    {
                        GameObject objMailInfoPage = Instantiate(Resources.Load("UI/Lobby/MailInfoPage")) as GameObject;
                        objMailInfoPage.transform.SetParent(upperUI.transform);
                        objMailInfoPage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -45);

#if UNITY_EDITOR
                        Cheat.Inst.RequestMailListCheat();
#else
                        PacketManager.Inst.RequestMailList(1);
#endif
                    }

                }
                break;

            case POPUP_STATE.Setting:
                {
                    objSubView = Instantiate(Resources.Load("UI/Lobby/Setting")) as GameObject;
                    objSubView.transform.SetParent(upperUI.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
                break;

            case POPUP_STATE.PVP:
                {
                    SetImageIconBackButton((POPUP_STATE)num);
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

        LobbySound.SetActive(true);
        GachaSound.SetActive(false);

        ModelViewManager.Inst.ChangeMainCharacterModel();
    }

    public void OnClosePopupWindow()//Main, Account Info , Setting...
    {
        //EtcSetActiveWithAnimation(true);

        //accountInfoUI.SetActive(true);
        //BackbuttonUI.SetActive(false);

        LobbySound.SetActive(true);
        GachaSound.SetActive(false);

        //ModelViewManager.Inst.ChangeMainCharacterModel();
    }

    public void SetTextBackButton(string text)
    {
        textBackButton.text = text;
    }

    public void SetImageIconBackButton(POPUP_STATE state)
    {
        if (state == POPUP_STATE.Servant || state == POPUP_STATE.Weapon || state == POPUP_STATE.EOS)
        {
            imageBackButtonIcon.gameObject.SetActive(true);
            textBackButton.rectTransform.anchoredPosition = new Vector2(52, 6);
        }
        else
        {
            imageBackButtonIcon.gameObject.SetActive(false);
            textBackButton.rectTransform.anchoredPosition = new Vector2(0, 6);
        }
    }



    public void OnClickChatButton()
    {
#if UNITY_EDITOR
        Cheat.Inst.RequestLobbyInfo();
#else
        PacketManager.Inst.RequestLobbyInfo();
#endif
    }
}