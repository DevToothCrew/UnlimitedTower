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

        if (DeckNum != DEFINE.HERO_FORMATION_NUM)
        {
            if (UserDataManager.Inst.formationDic.ContainsKey(DeckNum))
            {
                RemoveDeck();
                LinkedChar.GetComponent<CharContent>().RemoveCharImage();
                LinkedChar = null;
            }
        }
        else
        {
            Debug.Log("Do not remove hEro");
        }
     
    }

    public void RemoveDeck()
    {
        transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;

        //캐릭터 삭제

        Debug.Log("formatonDeck : " + DeckNum);
        int charIndex = UserDataManager.Inst.formationDic[DeckNum];
        //UserDataManager.Inst.formationDic.Remove(DeckNum);
        if(DeckNum<5)
        {
            UserDataManager.Inst.monsterDic[charIndex].OnFormation = false;
            UserDataManager.Inst.monsterDic[charIndex].FormationIndex = -1;
        }
        else
        {
            //UserDataManager.Inst.servantDic[charIndex].OnFormation = false;
            //UserDataManager.Inst.servantDic[charIndex].FormationIndex = -1;
            Reorder(ref UserDataManager.Inst.servantDic, 0);
        }


     
        //ShowEmptyText(true);

    }

    // 5 6 7 8 9
    // 순서 7 -> 6  -> 8 -> 5 -> 9

    void Reorder(ref Dictionary<int, Character> charDic, int charType)
    {
       GameObject FormationList =  LobbyManager.Inst.FormationList.gameObject;
        int index = -1;



        for(int i=0; i<UserDataManager.Inst.formationOrderList.Count; i++)
        {
            if(UserDataManager.Inst.formationOrderList[i] == DeckNum)
            {
                index = i;
                break;
            }
        }


        // 덱 삭제
        int charIndex = UserDataManager.Inst.formationDic[DeckNum];



        UserDataManager.Inst.formationDic.Remove(DeckNum);
        charDic[charIndex].OnFormation = false;
        charDic[charIndex].FormationIndex = -1;
        // empty 활성화

        int preDeckNum = UserDataManager.Inst.formationOrderList[index];
        int nextDeckNum = UserDataManager.Inst.formationOrderList[++index];

    


        int nextCharDicKey = UserDataManager.Inst.formationDic[nextDeckNum];

        while (nextDeckNum != 2)
        {

            FormationList.transform.GetChild(preDeckNum).gameObject.transform.GetChild(0).GetComponent<Image>().sprite =
          FormationList.transform.GetChild(nextDeckNum).gameObject.transform.GetChild(0).GetComponent<Image>().sprite;


            FormationList.transform.GetChild(preDeckNum).gameObject.GetComponent<FormationDeck>().LinkedChar = 
            FormationList.transform.GetChild(nextDeckNum).gameObject.GetComponent<FormationDeck>().LinkedChar;

            // 캐릭터 사용중이라는 표시하기.
            UserDataManager.Inst.formationDic.Add(preDeckNum, nextCharDicKey);
            charDic[nextCharDicKey].OnFormation = true;
            charDic[nextCharDicKey].FormationIndex = preDeckNum;

           
        
            UserDataManager.Inst.formationDic.Remove(nextDeckNum);

            preDeckNum = UserDataManager.Inst.formationOrderList[index];
            nextDeckNum = UserDataManager.Inst.formationOrderList[++index];
            if(nextDeckNum ==2)
            {
                UserDataManager.Inst.formationDic.Remove(preDeckNum);
                FormationList.transform.GetChild(preDeckNum).gameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
                FormationList.transform.GetChild(preDeckNum).gameObject.transform.GetChild(1).gameObject.SetActive(true);
                FormationList.transform.GetChild(nextDeckNum).GetComponent<FormationDeck>().LinkedChar = null;

                //charDic[nextCharDicKey].OnFormation = true;
                //charDic[nextCharDicKey].FormationIndex = preDeckNum;
                break;
            }
            nextCharDicKey = UserDataManager.Inst.formationDic[nextDeckNum];

        }


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
