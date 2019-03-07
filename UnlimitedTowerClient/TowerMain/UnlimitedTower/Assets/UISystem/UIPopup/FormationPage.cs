using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationPage : MonoBehaviour {

    public GameObject[] monsterObject = new GameObject[5];
    public GameObject[] servantObject = new GameObject[5];

    public GameObject monsterObjectList;
    public GameObject servantObjectList;

    public readonly int[] positionOrder = { 2, 1, 3, 0, 4, 2, 1, 3, 0, 4 };
    public UserPartyData partyInfo;

    public void OnEnable()
    {
        SetFormationData();
        OnClickServantListButton();
    }
    
    public void SetFormationData()
    {
        partyInfo = UserDataManager.Inst.GetUserPartyInfo();
        if (partyInfo == null)
        {
            Debug.Log("Invalid Request Party Info is Empty");
            return;
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

    public void OnClickMonsterButton(int num)
    {
        Debug.Log("Monster Button Num : " + num + ", position : " + positionOrder[num]);
        //monsterObject[positionOrder[num]].SetActive(false);
    }

    public void OnClickServantButton(int num)
    {
        Debug.Log("Servant Button Num : " + num + ", position : " + positionOrder[num]);
        //servantObject[positionOrder[num]].SetActive(false);
    }

    public void OnClickHeroButton()
    {
        Debug.Log("Hero's position cannot be changed.");
    }
}
