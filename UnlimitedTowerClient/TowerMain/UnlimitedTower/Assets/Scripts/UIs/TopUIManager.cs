using System.Collections.Generic;
using UnityEngine;

public class TopUIManager : MonoSingleton<TopUIManager> {

    public void ShowBurnResult(List<UserItemData> getItemList, ulong getUTG)
    {
        GameObject objSubView = Instantiate(Resources.Load("UI/Misc/BurnResultVC")) as GameObject;
        objSubView.transform.SetParent(this.transform);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        objSubView.GetComponent<BurnResultVC>().updateView(getItemList, getUTG);

        LobbyTopInfo.Inst.UpdateTopInfo();
    }
}
