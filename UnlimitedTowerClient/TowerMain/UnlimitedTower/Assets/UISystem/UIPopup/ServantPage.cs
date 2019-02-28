using System.Collections.Generic;
using UnityEngine;

public class ServantPage : MonoBehaviour {

    public GameObject[] page = new GameObject[3];
    public GameObject servantObject;
    public Transform servnatListTransform;

    public SERVANT_PAGE_STATE state;

    public void Awake()
    {
        state = SERVANT_PAGE_STATE.LIST;
        OnClickButton(0);
    }

    public void OnClickButton(int num)
    {
        for(int i = 0; i < (int)SERVANT_PAGE_STATE.MAX; i++)
        {
            if (i == num)
            {
                page[i].SetActive(true);
            }
            else
            {
                page[i].SetActive(false);
            }
        }

        if (num == (int)SERVANT_PAGE_STATE.LIST)
        {
            List<UserServantData> servantList = UserDataManager.Inst.GetServantList();

            if(servantList == null)
            {
                return;
            }

            GameObject newObject;
            ServantObject servantObjectTemp;

            int servantCount = servantList.Count;
            if(servantCount > 50)
            {
                servantCount = 50;
            }

            for (int i = 0; i < servantCount; i++)
            {
                newObject = Instantiate(servantObject) as GameObject;
                servantObjectTemp = newObject.GetComponent<ServantObject>();
                servantObjectTemp.SetServantData(servantList[i]);

                newObject.transform.SetParent(this.servnatListTransform);
            }
        }
    }
}

public enum SERVANT_PAGE_STATE
{
    LIST = 0,
    DETAIL = 1,
    GRIND = 2,
    MAX = 3,
}
