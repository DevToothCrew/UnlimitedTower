using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MailInfoPage : MonoSingleton<MailInfoPage> {

    public Text mailCount;
    public Transform mailList;
    public GameObject mailObjectPrefab;
    public GameObject[] mailObjects = null;
    public Dictionary<int, MailInfo> mailInfoDic = new Dictionary<int, MailInfo>();

    public void OnClickExitButton()
    {
        LobbyManager.Inst.OnClosePopupWindow();
        Destroy(this.gameObject);
    }

    public void SetMailList(List<MailInfo> getMailInfoList)
    {
        if(mailObjects != null)
        {
            for(int i = 0; i < mailObjects.Length; i++)
            {
                Destroy(mailObjects[i]);
            }

            mailObjects = null;
        }

        mailCount.text = UserDataManager.Inst.GetUserLobbyInfo().mailCount.ToString();

        mailObjects = new GameObject[getMailInfoList.Count];
        for (int i = 0; i < getMailInfoList.Count; i++)
        {
            GameObject mailObject = Instantiate(mailObjectPrefab);
            mailObject.transform.SetParent(mailList);
            mailObject.GetComponent<MailInfoObject>().SetMailInfo(getMailInfoList[i]);
            mailObjects[i] = mailObject;

            mailInfoDic.Add(getMailInfoList[i].index, getMailInfoList[i]);
        }
    }

    public void OnClickReceiveAllButton()
    {
        List<MailInfo> mailInfoList = mailInfoDic.Values.ToList();

        List<int> receiveMailList = new List<int>();
        for (int i = 0; i < mailInfoList.Count; i++)
        {
            if(i >= 8)
            {
                break;
            }

            receiveMailList.Add(mailInfoList[i].index);
        }

        if(receiveMailList.Count > 0)
        {
            PacketManager.Inst.RequestMailOpen(receiveMailList);
        }
    }

    public void DelMail(int mailIndex)
    {
        mailInfoDic.Remove(mailIndex);
    }

    //public void RefreshMailList()
    //{
    //    if (mailObjects != null)
    //    {
    //        for (int i = 0; i < mailObjects.Length; i++)
    //        {
    //            Destroy(mailObjects[i]);
    //        }

    //        mailObjects = null;
    //    }

    //    mailCount.text = UserDataManager.Inst.GetUserLobbyInfo().mailCount.ToString();

    //    List<MailInfo> mailInfoList = mailInfoDic.Values.ToList();

    //    mailObjects = new GameObject[mailInfoList.Count];
    //    for (int i = 0; i < mailInfoList.Count; i++)
    //    {
    //        GameObject mailObject = Instantiate(mailObjectPrefab);
    //        mailObject.transform.SetParent(mailList);
    //        mailObject.GetComponent<MailInfoObject>().SetMailInfo(mailInfoList[i]);
    //        mailObjects[i] = mailObject;
    //    }
    //}
}
