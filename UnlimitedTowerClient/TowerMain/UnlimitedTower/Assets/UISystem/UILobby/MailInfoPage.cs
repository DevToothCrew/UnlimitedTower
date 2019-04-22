using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailInfoPage : MonoSingleton<MailInfoPage> {

    public Text mailCount;
    public Transform mailList;
    public GameObject mailObjectPrefab;
    public GameObject[] mailObjects = null;

    public void OnClickExitButton()
    {
        LobbyManager.Inst.OnClickBackButton();
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
        }
    }
}
