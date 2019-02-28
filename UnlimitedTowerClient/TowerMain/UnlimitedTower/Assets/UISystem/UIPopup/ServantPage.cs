using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServantPage : MonoBehaviour {

    public GameObject[] page = new GameObject[3];
    public GameObject servantObject;

    public SERVANT_PAGE_STATE state;

    public void Awake()
    {
        state = SERVANT_PAGE_STATE.LIST;
        OnClickButton(0);
    }

    public void OnClickButton(int num)
    {
        if(num == (int)SERVANT_PAGE_STATE.LIST)
        {
            List<UserServantData> servantList = UserDataManager.Inst.GetServantList();
            for(int i = 0; i < servantList.Count; i++)
            {
                var newServant = Instantiate(servantObject);
                newServant.transform.SetParent(this.transform);
                newServant.GetComponent<ServantObject>().SetServantData(servantList[i]);                
            }
        }

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
    }
}

public enum SERVANT_PAGE_STATE
{
    LIST = 0,
    DETAIL = 1,
    GRIND = 2,
    MAX = 3,
}
