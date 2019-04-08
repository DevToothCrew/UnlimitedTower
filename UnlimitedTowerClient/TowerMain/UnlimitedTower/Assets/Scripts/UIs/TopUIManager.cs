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

    public void ShowGrindResult(bool is_success, int item_type, int item_idx, int item_num) //함수 인자 임시 설정... -> 값으로 결과 데이터를 넘겨주면 될듯. ex) 성공/실패, 어떤 아이템인지, 몇개인지.
    {
        GameObject objSubView = Instantiate(Resources.Load("UI/Misc/GrindResultVC")) as GameObject;
        objSubView.transform.SetParent(this.transform);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }



}
