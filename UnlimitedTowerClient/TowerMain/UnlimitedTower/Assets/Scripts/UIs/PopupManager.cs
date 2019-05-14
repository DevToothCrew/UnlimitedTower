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
    private MESSAGE_IDX messageIdx;

    public void SetPopupMasseage(POPUP_TYPE type, MESSAGE_IDX msg_idx)
    {
        popupType = type;
        messageIdx = msg_idx;
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

                buttonFrame.interactable = true;
                buttonCancel.gameObject.SetActive(false);
                buttonOK.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.WARNING:
                textTitle.text = "Warning!";

                buttonFrame.interactable = true;
                buttonCancel.gameObject.SetActive(false);
                buttonOK.gameObject.SetActive(false);
                break;
            case POPUP_TYPE.CONFIRM:
                textTitle.text = "Confirm";

                buttonFrame.interactable = false;
                buttonCancel.gameObject.SetActive(true);
                buttonOK.gameObject.SetActive(true);

                break;
            default:
                break;
        }

        textMessage.text = MessageList.GetMessage(messageIdx);
    }

    public void OnClickClose()
    {
        Destroy(this.gameObject);
    }

    public void OnClickConfirm()
    {
        switch(messageIdx)
        {
            case MESSAGE_IDX.UPGRADE_MONSTER_CONFIRM:
                if (SubViewUpgrade.Inst != null)
                {
                    SubViewUpgrade.Inst.UpgradeMonster();
                }
                break;
            case MESSAGE_IDX.UPGRADE_EQUIPMENT_CONFIRM:
                if (SubViewUpgrade.Inst != null)
                {
                    SubViewUpgrade.Inst.UpgradeEquipment();
                }
                break;
            default:
                break;
        }
        Destroy(this.gameObject);
    }
}
