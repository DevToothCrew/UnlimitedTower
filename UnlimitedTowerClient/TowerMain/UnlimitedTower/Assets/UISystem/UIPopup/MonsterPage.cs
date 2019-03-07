using System.Collections.Generic;
using UnityEngine;

public class MonsterPage : MonoBehaviour {

    public GameObject[] page = new GameObject[4];
    public GameObject monsterObject;
    public Transform monsterListTransform;

    public MONSTER_PAGE_STATE state;

    public void OnEnable()
    {
        state = MONSTER_PAGE_STATE.LIST;
        OnClickButton((int)state);
    }

    public void OnClickButton(int num)
    {
        for (int i = 0; i < (int)MONSTER_PAGE_STATE.MAX; i++)
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

        if (num == (int)MONSTER_PAGE_STATE.LIST)
        {
            List<UserMonsterData> monsterList = UserDataManager.Inst.GetMonsterList();

            if (monsterList == null)
            {
                return;
            }

            GameObject newObject;
            MonsterObject monsterObjectTemp;

            int monsterCount = monsterList.Count;
            if (monsterCount > 50)
            {
                monsterCount = 50;
            }

            for (int i = 0; i < monsterCount; i++)
            {
                newObject = Instantiate(monsterObject) as GameObject;
                monsterObjectTemp = newObject.GetComponent<MonsterObject>();
                monsterObjectTemp.SetMonsterData(monsterList[i]);

                newObject.transform.SetParent(monsterListTransform);
            }
        }
    }
}

public enum MONSTER_PAGE_STATE
{
    LIST    = 0,
    DETAIL  = 1,
    UPGRADE = 2,
    SELL    = 3,
    MAX     = 4,
}