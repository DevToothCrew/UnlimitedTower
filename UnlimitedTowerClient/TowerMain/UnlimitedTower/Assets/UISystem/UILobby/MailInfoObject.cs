using UnityEngine;
using UnityEngine.UI;
using System;

public class MailInfoObject : MonoBehaviour {

    public int index;
    public Image imageIcon;
    public Text textName;
    public Text textCount;
    public Text textType;

    public void SetMailInfo(MailInfo getMailInfo)
    {
        if(getMailInfo == null)
        {
            return;
        }

        index = getMailInfo.index;
        imageIcon.sprite = getMailInfo.resourceIcon;
        textName.text = getMailInfo.name;
        textCount.text = "x" + getMailInfo.count.ToString();
        textType.text = getMailInfo.type.ToString();
    }

    public void OnClickReceiveButton()
    {
        PacketManager.Inst.RequestMailOpen(index);
    }
}
