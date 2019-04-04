using UnityEngine.UI;
using UnityEngine;

public class LobbyManager : MonoSingleton<LobbyManager> {

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
        switch ((POPUP_STATE)num)
        {
            case POPUP_STATE.Hero:
                {
                    SetTextBackButton("Party");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/PartyInfoVC")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
                break;

            case POPUP_STATE.Weapon:
                {
                    SetTextBackButton("Inventory");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/InventoryVC")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
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
                }
                break;

            case POPUP_STATE.Stage:
                {
                    SetTextBackButton("Stage");
                    objSubView = Instantiate(Resources.Load("UI/Lobby/StageScene")) as GameObject;
                    objSubView.transform.SetParent(this.transform);
                    objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
                break;

            default:
                return;
        }

        EtcSetActiveWithAnimation(false);

        accountInfoUI.SetActive(false);
        BackbuttonUI.SetActive(true);

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