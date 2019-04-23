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

    private POPUP_TYPE popupType;
    private int messageIdx;

    public void SetPopupMasseage(POPUP_TYPE type, int message_idx)
    {
        popupType = type;
        messageIdx = message_idx;
        updateView();
    }

    void updateView()
    {
        //TODO : CSVDATA의 LocalizationData에 언어별 Text가 있는데 해당 Data에 Message를 추가하거나, 아에 따로 만들어서 가져오면 될듯
        textMessage.text = ""; //ex. getLocalizationData(messageIdx);

        switch (popupType)
        {
            case POPUP_TYPE.NOTIVE:
                textTitle.text = "Notice";

                buttonFrame.gameObject.SetActive(true);
                buttonCancel.gameObject.SetActive(false);
                buttonOK.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.WARNING:
                textTitle.text = "Warning!";

                buttonFrame.gameObject.SetActive(true);
                buttonCancel.gameObject.SetActive(false);
                buttonOK.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.CONFIRM:
                textTitle.text = "Confirm";

                buttonFrame.gameObject.SetActive(false);
                buttonCancel.gameObject.SetActive(true);
                buttonOK.gameObject.SetActive(true);

                //TODO : 메세지 IDX에 따라서 OK버튼에 해당 함수를 연결하면 됨.
                //if (messageIdx == ??) {
                //  buttonOK.onClick.AddListener(/*해당 클래스의 해당 함수*/);
                //}

                break;
            default:
                break;
        }
    }

    public void OnClickClose()
    {
        Destroy(this.gameObject);
    }
}
