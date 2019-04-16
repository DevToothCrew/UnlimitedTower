using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopUIManager : MonoSingleton<TopUIManager> {

    enum popup_type
    {
        NOTICE = 0,
        WARNING,
        CONFIRM
    };
    private popup_type popupType;

    public void ShowPopupMessage(int message_idx)
    {

    }

    public void ShowBurnResult(List<UserItemData> getItemList, ulong getUTG)
    {
        GameObject objSubView = Instantiate(Resources.Load("UI/Misc/BurnResultVC")) as GameObject;
        objSubView.transform.SetParent(this.transform);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        objSubView.GetComponent<BurnResultVC>().updateView(getItemList, getUTG);
    }

    public void ShowSimpleErrorPopup(string Text)
    {
        GameObject objSubView = Instantiate(Resources.Load("UI/Misc/SimpleErrorPopupVC")) as GameObject;
        objSubView.transform.SetParent(this.transform);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        objSubView.GetComponent<SimpleErrorPopupVC>().UpdateErrorText(Text);
    }
}
