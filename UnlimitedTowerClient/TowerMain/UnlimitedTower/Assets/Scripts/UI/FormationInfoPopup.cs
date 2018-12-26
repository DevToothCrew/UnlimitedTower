using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FormationInfoPopup : MonoBehaviour
{
    public GameObject[] formation = new GameObject[10];
    public Party partyInfo = new Party();
    public int lastPartyNum = 1;

    private void Awake()
    {
        if (UserDataManager.Inst.partyDic.ContainsKey(lastPartyNum) == false)
        {
            Debug.Log("Invalid Load Party Dic : " + lastPartyNum);
        }

        partyInfo = UserDataManager.Inst.partyDic[lastPartyNum];
    }

    private void SetFormationInfo()
    {
        foreach(KeyValuePair<int, PartyCharacterInfo> charInfo in partyInfo.characterList)
        {
            SetFormationImage(charInfo.Value.type, charInfo.Value.partyPosition);
        }
    }

    private void SetFormationImage(CHAR_TYPE type, int num)
    {

    }

    private void SetFormationText(int num)
    {
        //GameObject textObject = formation[num - 1].GetComponentInChildren<GameObject>("Text");

    }
}
