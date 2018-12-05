using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class FormationDeck : MonoBehaviour, IPointerClickHandler
{

    public int DeckNum = -1;
    public bool HeroDeck = false;

    // 덱과 연결된 캐릭터 목록
    public GameObject LinkedChar = null;


    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log("On Click : Formation");

        // 이미 덱에 캐릭터가 존재하면
        if (UserDataManager.Inst.formationDic.ContainsKey(DeckNum))
        {
            RemoveDeck();
            LinkedChar.GetComponent<CharContent>().RemoveCharImage();
            LinkedChar = null;
        }  
    }

    public void RemoveDeck()
    {
        transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;

        //캐릭터 삭제

        Debug.Log("formatonDeck : " + DeckNum);
        int charIndex = UserDataManager.Inst.formationDic[DeckNum];
        UserDataManager.Inst.formationDic.Remove(DeckNum);
        if(DeckNum<5)
        {
            UserDataManager.Inst.monsterDic[charIndex].OnFormation = false;
            UserDataManager.Inst.monsterDic[charIndex].FormationIndex = -1;
        }
        else
        {
            UserDataManager.Inst.servantDic[charIndex].OnFormation = false;
            UserDataManager.Inst.servantDic[charIndex].FormationIndex = -1;
        }


     
        ShowEmptyText(true);

    }
    public void ShowEmptyText(bool on)
    {
        if(on)
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
        
    }
}
