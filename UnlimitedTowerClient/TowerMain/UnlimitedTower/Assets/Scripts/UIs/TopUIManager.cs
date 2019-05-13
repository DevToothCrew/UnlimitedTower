using System.Collections.Generic;
using UnityEngine;

public class TopUIManager : MonoSingleton<TopUIManager> {
    public Transform objUpperUI;

    public void ShowBurnResult(List<UserItemData> getItemList, ulong getUTG)
    {
        GameObject objSubView = Instantiate(Resources.Load("UI/Misc/BurnResultVC")) as GameObject;
        objSubView.transform.SetParent(this.transform);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        objSubView.GetComponent<BurnResultVC>().updateView(getItemList, getUTG);

        LobbyTopInfo.Inst.UpdateTopInfo();
    }

    public void ShowPopupMessage(POPUP_TYPE popup_type, MESSAGE_IDX msg_idx)
    {
        GameObject objSubView = Instantiate(Resources.Load("UI/Misc/PopupManager")) as GameObject;
        objSubView.transform.SetParent(objUpperUI);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        objSubView.GetComponent<PopupManager>().SetPopupMasseage(popup_type, msg_idx);
    }
}
