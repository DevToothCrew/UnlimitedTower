using UnityEngine.UI;
using UnityEngine;

public class PopupUI : MonoBehaviour {

    public POPUP_STATE state;
    public GameObject heroMenu;
    public GameObject shopMenu;

    public void OnEnable()
    {
        state = POPUP_STATE.Hero;
        heroMenu.SetActive(true);
        shopMenu.SetActive(false);
    }

    public void OnClickMenu(int getState)
    {
        // 버튼과 현재 상태가 다를 경우만 반응
        if (state != (POPUP_STATE)getState)
        {
            state = (POPUP_STATE)getState;
            LobbyManager.Inst.SetPopupMenu(state.ToString());
        }
    }
}

