using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class FormationDeck : MonoBehaviour, IPointerClickHandler
{

    public int deckNum = -1;
    public bool heroDeck = false;

    // 덱과 연결된 캐릭터 목록
    public GameObject LinkedChar = null;
    private GameObject ChildCharImage;
    private GameObject ChildEmptyText;

    void Awake()
    {
        ChildCharImage = transform.GetChild(0).gameObject;
        ChildEmptyText = transform.GetChild(1).gameObject;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {   
        Debug.Log("On Click : Formation");
           

        if (deckNum != DEFINE.HERO_FORMATION_NUM)
        {
            RemoveDeck();
        }
        else
        {
            Debug.Log("Do not remove hEro");
        }  
    }

    public void RemoveDeck()
    {
        //캐릭터 삭제
        Debug.Log("formatonDeck : " + deckNum);
     
        if(deckNum<5)
        {
            if (CheckMonsterEnableRemove())
            {
                ReorderMonster();
            }
            else
            {
                Debug.Log("마지막 몬스터덱이 아니라 삭제 할 수 없습니다");
            }
        }
        else
        {
            // 두 번째 서번트 덱은 사라지는 버그가 있다.
            if (CheckServantEnableRemove())
            {
                ReorderServant();
                CheckPairReorder();
            }
            else
            {
                Debug.Log("마지막 서번트 덱이 아니라 삭제 할 수 없습니다");
            }
          
        }
    }


    private bool CheckServantEnableRemove()
    {
        int usingPartyNum = UserDataManager.Inst.usingPartyNum;
        int lastIndex = 4;
        //히어로는 항상 존재하기 때문에 1부터 시작한다.
        for(int i=1; i<DEFINE.PARTY_MAX_NUM/2; ++i)
        {
            if(UserDataManager.Inst.partyDic[usingPartyNum].characterList[i].index ==0)
            {
                lastIndex = i-1;
                break;
            }

        }
        
        // 테스트 용도로 마지막 덱만 제거 되도록한다.
        // 지금 덱이 마지막덱인지 판정 조건?
        //

        // 수정할것
        //if(lastIndex == 4)
        //{
        //    return true;
        //}


        if(deckNum  ==  UserDataManager.Inst.partyDic[usingPartyNum].characterList[lastIndex].partyLocation)
        {
            return true;
        }
        return false;
    }
    private bool CheckMonsterEnableRemove()
    {
        int usingPartyNum = UserDataManager.Inst.usingPartyNum;
        int lastIndex = DEFINE.PARTY_MAX_NUM-1;
        //히어로는 항상 존재하기 때문에 1부터 시작한다.
        for (int i = 5; i < DEFINE.PARTY_MAX_NUM ; ++i)
        {
            if (UserDataManager.Inst.partyDic[usingPartyNum].characterList[i].index == 0)
            {
                lastIndex = i - 1;
                break;
            }
        }

        if (lastIndex == DEFINE.PARTY_MAX_NUM - 1)
        {
            return true;
        }


        if (deckNum == UserDataManager.Inst.partyDic[usingPartyNum].characterList[lastIndex].partyLocation)
        {
            return true;
        }
        return false;
    }
    private void ReorderServant()
    {
        GameObject FormationList = LobbyManager.Inst.FormationList.gameObject;
        int orderIndex = -1;

        int startNum = 0;
        int usingPartyNum = UserDataManager.Inst.usingPartyNum;
 

        // 범위 0~4 & 5~9로 바꿀것.
        for (int i = startNum; i < UserDataManager.Inst.formationOrderList.Count + 0; i++)
        {
            if (UserDataManager.Inst.formationOrderList[i] == deckNum)
            {
                orderIndex = i;
                break;
            }
        }

        // 덱 삭제
        int charIndex = -1;
        for(int i=0; i<DEFINE.PARTY_MAX_NUM; i++)
        {
            if(UserDataManager.Inst.partyDic[usingPartyNum].characterList[i].partyLocation == deckNum)
            {
                charIndex = UserDataManager.Inst.partyDic[usingPartyNum].characterList[i].index;
                UserDataManager.Inst.partyDic[usingPartyNum].characterList[i].index = 0;
                break;
            }
        }


       // UserDataManager.Inst.formationDic.Remove(deckNum);
        UserDataManager.Inst.newServantDic[charIndex].onFormation = false;
        UserDataManager.Inst.newServantDic[charIndex].formationIndex = -1;

        if (ChildCharImage.GetComponent<Image>())
        {
            Sprite sprite = Resources.Load<Sprite>("UI/LobbyUI/CenterPopupUI/15_Formation_Empty_Slot");
            ChildCharImage.GetComponent<Image>().sprite = sprite;
        }
        if (LinkedChar && LinkedChar.GetComponent<CharContent>())
        {
            LinkedChar.GetComponent<CharContent>().RemoveCharImage();
            LinkedChar = null;
        }


        if (orderIndex++ == startNum + 4)
        {
            Debug.Log("이 덱이 마지막 덱입니다.");
            ShowEmptyText(true);
            return;
        }

    }
    private void ReorderMonster()
    {
        int usingPartyNum = UserDataManager.Inst.usingPartyNum;
        GameObject FormationList = LobbyManager.Inst.FormationList.gameObject;
        int orderIndex = -1;

        int startNum = 0;


        // 범위 0~4 & 5~9로 바꿀것.
        for (int i = startNum; i < UserDataManager.Inst.formationOrderList.Count + 0; i++)
        {
            if (UserDataManager.Inst.formationOrderList[i] == deckNum)
            {
                orderIndex = i;
                break;
            }
        }


        // 덱 삭제
        int charIndex = -1;
        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            if (UserDataManager.Inst.partyDic[usingPartyNum].characterList[i].partyLocation == deckNum)
            {
                charIndex = UserDataManager.Inst.partyDic[usingPartyNum].characterList[i].index;
                UserDataManager.Inst.partyDic[usingPartyNum].characterList[i].index = 0;
                break;
            }
        }

      
        //UserDataManager.Inst.formationDic.Remove(deckNum);
        UserDataManager.Inst.newMonsterDic[charIndex].onFormation = false;
        UserDataManager.Inst.newMonsterDic[charIndex].formationIndex = -1;

        if (ChildCharImage.GetComponent<Image>())
        {
            Sprite sprite = Resources.Load<Sprite>("UI/LobbyUI/CenterPopupUI/15_Formation_Empty_Slot");
            ChildCharImage.GetComponent<Image>().sprite = sprite;
        }
        if (LinkedChar && LinkedChar.GetComponent<CharContent>())
        { 
            LinkedChar.GetComponent<CharContent>().RemoveCharImage();
            LinkedChar = null;
        }


        if (orderIndex++ == startNum + 4)
        {
            Debug.Log("이 덱이 마지막 덱입니다.");
            ShowEmptyText(true);
            return;
        }
    
    }

    public void ShowEmptyText(bool on)
    {
        if (on)
        {
            ChildEmptyText.SetActive(true);
        }
        else
        {
            ChildEmptyText.SetActive(false);
        }
    }
    private void CheckPairReorder()
    {
        // 히어로를 기본으로 포함하기 때문에 1을 증가 시켯다.
        int backLineCharNum = 1;
        int frontLineCharNum = 0;
        int usingPartyNum = UserDataManager.Inst.usingPartyNum;

        for (int i = 1; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            if (i < 5)
            {
                if (UserDataManager.Inst.partyDic[usingPartyNum].characterList[i].index != 0)
                {
                    backLineCharNum++;
                }
            }
            else
            {
                if (UserDataManager.Inst.partyDic[usingPartyNum].characterList[i].index != 0)
                {
                    frontLineCharNum++;

                }
            }
        }
        // 몬스터 라인의 수가 더 많으면
        if (backLineCharNum < frontLineCharNum)
        {
            GameObject FormationList = LobbyManager.Inst.FormationList.gameObject;
            FormationList.transform.GetChild(deckNum - 5).gameObject.GetComponent<FormationDeck>().ReorderMonster();       
        }
    }



}
