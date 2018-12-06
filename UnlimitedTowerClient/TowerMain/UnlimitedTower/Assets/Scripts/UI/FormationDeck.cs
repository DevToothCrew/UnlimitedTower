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
                //LinkedChar.GetComponent<CharContent>().RemoveCharImage();
                //LinkedChar = null;
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
            //UserDataManager.Inst.monsterDic[charIndex].OnFormation = false;
            //UserDataManager.Inst.monsterDic[charIndex].FormationIndex = -1;
            Reorder(ref UserDataManager.Inst.monsterDic, 1);
        }
        else
        {
            //UserDataManager.Inst.servantDic[charIndex].OnFormation = false;
            //UserDataManager.Inst.servantDic[charIndex].FormationIndex = -1;
            Reorder(ref UserDataManager.Inst.servantDic, 0);
        }


     
        //ShowEmptyText(true);

    }


    void Reorder(ref Dictionary<int, Character> charDic, int charType)
    {
       GameObject FormationList =  LobbyManager.Inst.FormationList.gameObject;
        int orderIndex = -1;

        int startNum = -1;
        if(charType == 0)
        {
            startNum = 0;
        }
        else
        {
            startNum = 5;
        }

        // 범위 0~4 & 5~9로 바꿀것.
        for(int i= startNum; i<UserDataManager.Inst.formationOrderList.Count + 0; i++)
        {
            if(UserDataManager.Inst.formationOrderList[i] == DeckNum)
            {
                orderIndex = i;
                break;
            }
        }


        // 덱 삭제
        int charIndex = UserDataManager.Inst.formationDic[DeckNum];
        UserDataManager.Inst.formationDic.Remove(DeckNum);
        charDic[charIndex].OnFormation = false;
        charDic[charIndex].FormationIndex = -1;

        LinkedChar.GetComponent<CharContent>().RemoveCharImage();
        LinkedChar = null;

        // 5 6 7 8 9
        // 순서 7 -> 6 -> 8 -> 5 -> 9
        // 순서 2 -> 1 -> 3 -> 0 -> 4


        if (orderIndex++ == startNum + 4)
        {
            Debug.Log("이 덱이 마지막 덱입니다.");
            ShowEmptyText(true);
            return;
        }

      


        // 현재 덱 넘버
        int preDeckNum = UserDataManager.Inst.formationOrderList[orderIndex - 1];
        int nextDeckNum = UserDataManager.Inst.formationOrderList[orderIndex];

        // 다음덱이 존재하지 않을 때
        if (UserDataManager.Inst.formationDic.ContainsKey(nextDeckNum) == false)
        {
            Debug.Log("이 덱이 마지막 덱입니다.(1번 덱만  다음 없삼");
            ShowEmptyText(true);
            return;
        }

        int nextCharDicKey = UserDataManager.Inst.formationDic[nextDeckNum];


        int lastCharKey = -1;

        // 순서값이 아니라
        // 다른 값으로 확인을 해야할거 같다.
        // 
        while (orderIndex != 4 + 1 + startNum && UserDataManager.Inst.formationDic.ContainsKey(UserDataManager.Inst.formationOrderList[orderIndex])) //7 
        {

            preDeckNum = UserDataManager.Inst.formationOrderList[orderIndex - 1];
            nextDeckNum = UserDataManager.Inst.formationOrderList[orderIndex];
            nextCharDicKey = UserDataManager.Inst.formationDic[nextDeckNum];

            FormationList.transform.GetChild(preDeckNum).gameObject.transform.GetChild(0).GetComponent<Image>().sprite =
          FormationList.transform.GetChild(nextDeckNum).gameObject.transform.GetChild(0).GetComponent<Image>().sprite;


            FormationList.transform.GetChild(preDeckNum).gameObject.GetComponent<FormationDeck>().LinkedChar = 
            FormationList.transform.GetChild(nextDeckNum).gameObject.GetComponent<FormationDeck>().LinkedChar;

            // 캐릭터 사용중이라는 표시하기.
            UserDataManager.Inst.formationDic.Add(preDeckNum, nextCharDicKey);
            charDic[nextCharDicKey].OnFormation = true;
            charDic[nextCharDicKey].FormationIndex = preDeckNum;

            lastCharKey = UserDataManager.Inst.formationDic[nextDeckNum];

            UserDataManager.Inst.formationDic.Remove(nextDeckNum);
            orderIndex++;

        }
           preDeckNum = UserDataManager.Inst.formationOrderList[orderIndex - 1];



        // 마지막께 체크가 해제되었다.
        charDic[lastCharKey].OnFormation = false;
        charDic[lastCharKey].FormationIndex = -1;

        FormationList.transform.GetChild(preDeckNum).gameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
        FormationList.transform.GetChild(preDeckNum).gameObject.transform.GetChild(1).gameObject.SetActive(true);
       // FormationList.transform.GetChild(preDeckNum).GetComponent<FormationDeck>().LinkedChar.GetComponent<CharContent>().RemoveCharImage();
        FormationList.transform.GetChild(preDeckNum).GetComponent<FormationDeck>().LinkedChar = null;
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
