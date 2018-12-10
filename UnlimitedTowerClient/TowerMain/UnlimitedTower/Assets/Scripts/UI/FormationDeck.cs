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
            }
        }
        else
        {
            Debug.Log("Do not remove hEro");
        }  
    }

    public void RemoveDeck()
    {
        //캐릭터 삭제

        Debug.Log("formatonDeck : " + DeckNum);
        int charIndex = UserDataManager.Inst.formationDic[DeckNum];
        if(DeckNum<5)
        {
            Reorder(ref UserDataManager.Inst.monsterDic, CHAR_TYPE.MONSTER);
        }
        else
        {
            Reorder(ref UserDataManager.Inst.servantDic, CHAR_TYPE.SERVANT);
            CheckPairReorder();
        }

    }


    void Reorder(ref Dictionary<int, Character> charDic, CHAR_TYPE charType)
    {
       GameObject FormationList =  LobbyManager.Inst.FormationList.gameObject;
        int orderIndex = -1;

        int startNum = -1;
        if (charType == CHAR_TYPE.SERVANT)
        {
            startNum = 0;
        }
        else if (charType == CHAR_TYPE.MONSTER)
        {
            startNum = 5;
        }
        else
        {
            Debug.Log("Error : hero or error");
        }

        // 범위 0~4 & 5~9로 바꿀것.
        for (int i= startNum; i<UserDataManager.Inst.formationOrderList.Count + 0; i++)
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
        transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
        LinkedChar.GetComponent<CharContent>().RemoveCharImage();
        LinkedChar = null;

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
            if(UserDataManager.Inst.formationDic.TryGetValue(preDeckNum, out nextCharDicKey))
            {
                UserDataManager.Inst.formationDic.Add(preDeckNum, nextCharDicKey);
                charDic[nextCharDicKey].OnFormation = true;
                charDic[nextCharDicKey].FormationIndex = preDeckNum;

            }

          

            lastCharKey = UserDataManager.Inst.formationDic[nextDeckNum];

            UserDataManager.Inst.formationDic.Remove(nextDeckNum);
            orderIndex++;

        }
           preDeckNum = UserDataManager.Inst.formationOrderList[orderIndex - 1];



        // 마지막 덱의 체크가 해제되었다.
        charDic[lastCharKey].OnFormation = false;
        charDic[lastCharKey].FormationIndex = -1;

        FormationList.transform.GetChild(preDeckNum).gameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
        FormationList.transform.GetChild(preDeckNum).gameObject.transform.GetChild(1).gameObject.SetActive(true);
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

    private void CheckPairReorder()
    {
        int backLineCharNum = 0;
        int frontLineCharNum = 0;

        for(int i=0; i<10; i++)
        {
            if(i<5)
            {
                if (UserDataManager.Inst.formationDic.ContainsKey(i))
                {
                    frontLineCharNum++;
                }          
            }
            else
            {
                if (UserDataManager.Inst.formationDic.ContainsKey(i))
                {
                    backLineCharNum++;
                }
            }
        }
        // 몬스터 라인의 수가 더 많으면
        if(backLineCharNum < frontLineCharNum)
        {
            GameObject FormationList = LobbyManager.Inst.FormationList.gameObject;
            FormationList.transform.GetChild(DeckNum - 5).gameObject.GetComponent<FormationDeck>().Reorder(ref UserDataManager.Inst.monsterDic, CHAR_TYPE.MONSTER);         
        }
    }
}
