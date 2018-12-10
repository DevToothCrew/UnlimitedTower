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
    private GameObject childCharImage;
    private GameObject childEmptyText;

    void Awake()
    {
        childCharImage = transform.GetChild(0).gameObject;
        childEmptyText = transform.GetChild(1).gameObject;
    }

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
        int charIndex = -1;
        if(UserDataManager.Inst.formationDic.TryGetValue(DeckNum, out charIndex) == false)
        {
            Debug.Log("Error : UserDataManager.Inst.formationDic.TryGetValue(DeckNum, out charIndex) == false");
            return;
        }
       
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
        int charIndex = -1;
        if (UserDataManager.Inst.formationDic.TryGetValue(DeckNum, out charIndex))
        {
            UserDataManager.Inst.formationDic.Remove(DeckNum);
            charDic[charIndex].OnFormation = false;
            charDic[charIndex].FormationIndex = -1;

            if (childCharImage.GetComponent<Image>())
            {
                childCharImage.GetComponent<Image>().sprite = null;
            }
            if (LinkedChar && LinkedChar.GetComponent<CharContent>())
            {
                LinkedChar.GetComponent<CharContent>().RemoveCharImage();
                LinkedChar = null;
            }
            childEmptyText.SetActive(true);
        }
        else
        {
            Debug.Log("Error : UserDataManager.Inst.formationDic.TryGetValue");
        }

      
     
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
            Debug.Log("This deck is last deck!");
            ShowEmptyText(true);
            return;
        }

        int nextCharDicKey = -1;
        int lastCharKey = -1;
        if (UserDataManager.Inst.formationDic.TryGetValue(nextDeckNum, out nextCharDicKey) == false)
        {
            Debug.Log("Error : UserDataManager.Inst.formationDic.TryGetValue(nextDeckNum, out nextCharDicKey)");
            return;
        }

        while (orderIndex != 5 + startNum && UserDataManager.Inst.formationDic.ContainsKey(UserDataManager.Inst.formationOrderList[orderIndex])) 
        {

            preDeckNum = UserDataManager.Inst.formationOrderList[orderIndex - 1];
            nextDeckNum = UserDataManager.Inst.formationOrderList[orderIndex];

            if (UserDataManager.Inst.formationDic.TryGetValue(nextDeckNum, out nextCharDicKey))
            {
                if (FormationManager.Inst.DeckImages[preDeckNum].GetComponent<Image>()
                    && FormationManager.Inst.DeckImages[nextDeckNum].GetComponent<Image>())
                {
                    FormationManager.Inst.DeckImages[preDeckNum].GetComponent<Image>().sprite =
                  FormationManager.Inst.DeckImages[nextDeckNum].GetComponent<Image>().sprite;
                }

             

                FormationManager.Inst.Decks[preDeckNum].GetComponent<FormationDeck>().LinkedChar
                    = FormationManager.Inst.Decks[nextDeckNum].GetComponent<FormationDeck>().LinkedChar;

                // 캐릭터 사용중이라는 표시하기.
                UserDataManager.Inst.formationDic.Add(preDeckNum, nextCharDicKey);
                charDic[nextCharDicKey].OnFormation = true;
                charDic[nextCharDicKey].FormationIndex = preDeckNum;



                if (UserDataManager.Inst.formationDic.TryGetValue(nextDeckNum, out lastCharKey) == false)
                {
                    Debug.Log("Error : UserDataManager.Inst.formationDic.TryGetValue(nextDeckNum, out lastCharKey");
                }

             
                UserDataManager.Inst.formationDic.Remove(nextDeckNum);
                orderIndex++;
            }
            else
            {
                Debug.Log("Error : UserDataManager.Inst.formationDic.TryGetValue(nextDeckNum, out nextCharDicKey)");
            }
        }
           preDeckNum = UserDataManager.Inst.formationOrderList[orderIndex - 1];



        // 마지막 덱의 체크가 해제되었다.
        charDic[lastCharKey].OnFormation = false;
        charDic[lastCharKey].FormationIndex = -1;



        FormationManager.Inst.DeckImages[preDeckNum].GetComponent<Image>().sprite = null;
        FormationManager.Inst.DeckTexts[preDeckNum].gameObject.SetActive(true);
        FormationManager.Inst.Decks[preDeckNum].GetComponent<FormationDeck>().LinkedChar = null;
    }


    public void ShowEmptyText(bool on)
    {
        if(on)
        {
            childEmptyText.SetActive(true);
        }
        else
        {
            childEmptyText.SetActive(false);
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
