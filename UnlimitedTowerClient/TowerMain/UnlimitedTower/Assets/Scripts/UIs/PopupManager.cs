using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour {
    public GameObject FramePopup;

    public Button buttonFrame;
    public Text textTitle;
    public Text textMessage;
    public Button buttonCancel;
    public Button buttonOK;

    enum popup_type
    {
        NOTIVE = 0, //안내 타입 팝업 : Cancel/OK 버튼 표시안함, 창 터치로 닫기 가능
        WARNING,    //경고, 오류 타입 팝업 : Cancel/OK 버튼 표시안함, 창 터치로 닫기 가능
        CONFIRM     //확인용 팝업 : Cancel, OK 버튼 표시, 창 터치로 닫기 불가
    };
    popup_type popupType;

	void Start () {
        popupType = popup_type.NOTIVE;

    }
	
    void updateView()
    {
        switch(popupType)
        {
            case popup_type.NOTIVE:
                textTitle.text = "Notice";

                textMessage.text = "";

                buttonFrame.gameObject.SetActive(true);
                buttonCancel.gameObject.SetActive(false);
                buttonOK.gameObject.SetActive(false);
                break;
            case popup_type.WARNING:
                textTitle.text = "Warning!";

                textMessage.text = "";

                buttonFrame.gameObject.SetActive(true);
                buttonCancel.gameObject.SetActive(false);
                buttonOK.gameObject.SetActive(false);
                break;
            case popup_type.CONFIRM:
                textTitle.text = "Confirm";

                textMessage.text = "";

                buttonFrame.gameObject.SetActive(false);
                buttonCancel.gameObject.SetActive(true);
                buttonOK.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}
