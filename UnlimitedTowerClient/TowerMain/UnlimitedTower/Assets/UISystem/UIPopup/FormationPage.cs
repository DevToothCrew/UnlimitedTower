using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationPage : MonoBehaviour {

    public GameObject[] monsterObject = new GameObject[5];
    public GameObject[] servantObject = new GameObject[5];

    public readonly int[] positionOrder = { 2, 1, 3, 0, 4, 2, 1, 3, 0, 4 };

    public void OnEnable()
    {
        SetFormationData();
        SetServantList(true);
        SetMonsterList(false);
    }
    
    public void SetFormationData()
    {
        UserPartyData partyInfo = UserDataManager.Inst.GetUserPartyInfo();
        if (partyInfo == null)
        {
            Debug.Log("Invalid Request Party Info is Empty");
        }

        for (int i = 0; i < 10; i++)
        {
            if (partyInfo.formationDataDic.ContainsKey(i) == true)
            {
                if (i == 0)
                {
                    UserServantData heroInfo = UserDataManager.Inst.GetHeroInfo();
                    if (heroInfo == null)
                    {
                        Debug.Log("Invalid Hero Index : " + partyInfo.formationDataDic[i].index);
                    }

                    servantObject[positionOrder[i]].SetActive(true);
                    servantObject[positionOrder[i]].GetComponent<ServantObject>().SetServantData(heroInfo);
                }
                else if (i <= DEFINE.ServantMaxFormationNum)
                {
                    UserServantData servantInfo = UserDataManager.Inst.GetServantInfo(partyInfo.formationDataDic[i].index);
                    if (servantInfo == null)
                    {
                        Debug.Log("Invalid Servant Index : " + partyInfo.formationDataDic[i].index);
                    }

                    servantObject[positionOrder[i]].SetActive(true);
                    servantObject[positionOrder[i]].GetComponent<ServantObject>().SetServantData(servantInfo);
                }
                else
                {
                    UserMonsterData monsterInfo = UserDataManager.Inst.GetMonsterInfo(partyInfo.formationDataDic[i].index);
                    if (monsterInfo == null)
                    {
                        Debug.Log("Invalid Monster Index : " + partyInfo.formationDataDic[i].index);
                    }

                    monsterObject[positionOrder[i]].SetActive(true);
                    monsterObject[positionOrder[i]].GetComponent<MonsterObject>().SetMonsterData(monsterInfo);
                }
            }
            else
            {
                if (i <= DEFINE.ServantMaxFormationNum)
                {
                    servantObject[positionOrder[i]].SetActive(false);
                }
                else
                {
                    monsterObject[positionOrder[i]].SetActive(false);
                }
            }
        }
    }

    public void SetServantList(bool enable)
    {

    }

    public void SetMonsterList(bool enable)
    {

    }

    public void OnClickServantListButton()
    {
        SetServantList(true);
        SetMonsterList(false);
    }

    public void OnClickMonsterListButton()
    {
        SetServantList(false);
        SetMonsterList(true);
    }
}
