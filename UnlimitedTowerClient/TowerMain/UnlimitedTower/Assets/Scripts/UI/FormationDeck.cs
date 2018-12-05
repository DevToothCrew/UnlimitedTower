using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class FormationDeck : MonoBehaviour, IPointerClickHandler
{

    public int DeckNum = -1;
    public bool Active = false;
    public bool HeroDeck = false;

    // 덱과 연결된 캐릭터 목록
    public GameObject LinkedChar = null;


    public void Activate()
    {
        GetComponent<FormationDeck>().Active = true;
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }




    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log("On Click : Formation");

        // 이미 덱에 캐릭터가 존재하면
        RemoveDeck();
    }

    public void RemoveDeck()
    {
        if (transform.GetChild(0).gameObject.GetComponent<Image>().sprite)
        {
            transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
            LinkedChar.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);

            //캐릭터 삭제
            int charIndex = UserDataManager.Inst.formationDic[DeckNum];
            UserDataManager.Inst.formationDic.Remove(DeckNum);

            UserDataManager.Inst.characterDic[charIndex].OnFormation = false;
            UserDataManager.Inst.characterDic[charIndex].FormationIndex = -1;


        }
    }
}
