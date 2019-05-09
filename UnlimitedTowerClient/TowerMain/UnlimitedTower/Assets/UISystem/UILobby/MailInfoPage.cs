using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MailInfoPage : MonoSingleton<MailInfoPage> {

    public Text textMailCount;
    public int mailCount;
    public Text textPageCount;
    public int pageCount;
    public Transform mailList;
    public GameObject mailObjectPrefab;
    public GameObject[] mailObjects = null;
    public Dictionary<int, MailInfo> mailInfoDic = new Dictionary<int, MailInfo>();

    public void OnEnable()
    {
        pageCount = 1;
        textPageCount.text = pageCount.ToString();
    }

    public void OnClickExitButton()
    {
        Destroy(this.gameObject);
    }

    public void SetMailList(List<MailInfo> getMailInfoList, int getMailCount)
    {
        if(mailObjects != null)
        {
            for(int i = 0; i < mailObjects.Length; i++)
            {
                Destroy(mailObjects[i]);
            }

            mailObjects = null;
        }

        mailInfoDic = new Dictionary<int, MailInfo>();

        mailCount = getMailCount;
        textMailCount.text = mailCount.ToString();

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
        mailCount -= 1;
    }

    public void OnClickNextButton()
    {
        if(pageCount > (mailCount / 8))
        {
            return;
        }
        pageCount += 1;
        textPageCount.text = pageCount.ToString();

        PacketManager.Inst.RequestMailList(pageCount);
    }

    public void OnClickBeforeButton()
    {
        if(pageCount == 1)
        {
            return;
        }
        pageCount -= 1;
        textPageCount.text = pageCount.ToString();

        PacketManager.Inst.RequestMailList(pageCount);
    }

    public void RefreshMailList()
    {
        if (mailObjects != null)
        {
            for (int i = 0; i < mailObjects.Length; i++)
            {
                Destroy(mailObjects[i]);
            }

            mailObjects = null;
        }

        textMailCount.text = mailCount.ToString();

        if (mailCount > 0)
        {
            if ( mailCount > (8 * (pageCount - 1)) )
            {
                PacketManager.Inst.RequestMailList(pageCount);
            }
            else
            {
                pageCount -= 1;
                textPageCount.text = pageCount.ToString();
                PacketManager.Inst.RequestMailList(pageCount);
            }
        }
    }
}
