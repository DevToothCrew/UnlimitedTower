using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharContent : MonoBehaviour, IPointerClickHandler
{
    public int CharDicKey;
    public int CharType = -1;
   
    void Awake()
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // 이미 덱에 캐릭터가 존재하면, 그 캐릭터를 뺀다.


        // 캐릭터 타입 : Hero Mosnter Servaent
        // 진영? 타입 : Player : Enemy
        // FormationType : FormationType.Player, FormationType, Enemy

        if(CharType == 0)
        {
            CheckAddOrRemove(ref UserDataManager.Inst.servantDic, CharType);

        }
        else if (CharType == 1)
        {
            CheckAddOrRemove(ref UserDataManager.Inst.monsterDic, CharType);
        }


    }


    private void CheckAddOrRemove(ref Dictionary<int, Character> charDic, int charType)
    {
        if (charDic.ContainsKey(CharDicKey))
        {
            if (charDic[CharDicKey].OnFormation == true)
            {
                RemoveDeck(ref charDic, charType);
            }
            else
            {
                if(charType == 1)
                {
                    if (CheckAddDeck() == false)
                    {
                        Debug.Log("용병 수가 적어 몬스터를 추가할 수 없습니다.");
                        return;
                    }
                }
                AddDeck(ref charDic, charType);
            }
        }
    }
    private void RemoveDeck(ref Dictionary<int, Character> charDic, int charType)
    {
        if (charDic[CharDicKey].OnFormation == true)
        {
            int deckNum = charDic[CharDicKey].FormationIndex;
            GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
            RemoveCharImage();
            deck.GetComponent<FormationDeck>().RemoveDeck();
            //deck.GetComponent<FormationDeck>().ShowEmptyText(true);

            transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    private void AddDeck(ref Dictionary<int, Character> charDic, int charType)
    {
        int startNum = -1;
        string imageFath = null;
        if (charType == 0)
        {
            startNum = 0;
            imageFath = "UI/CharaterImage/";
        }
        else
        {
            startNum = 5;
            imageFath = "UI/MonsterImage/";
        }


        for (int i = startNum; i < DEFINE.PARTY_MAX_NUM / 2 + startNum; i++)
        {
            int deckNum = UserDataManager.Inst.formationOrderList[i];
            // 빈 덱 검색
            if (UserDataManager.Inst.formationDic.ContainsKey(deckNum) == false)
            {
                // 캐릭터 넣기.
                GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
                Sprite sprite = Resources.Load<Sprite>(imageFath + charDic[CharDicKey].Name);

                // 덱에 캐릭터 오브젝트 연결
                deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                deck.GetComponent<FormationDeck>().LinkedChar = gameObject;
                deck.GetComponent<FormationDeck>().ShowEmptyText(false);


                // 캐릭터 사용중이라는 표시하기.
                UserDataManager.Inst.formationDic.Add(deckNum, CharDicKey);
                charDic[CharDicKey].OnFormation = true;
                charDic[CharDicKey].FormationIndex = deckNum;

                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                return;

            }
        }
    }

    public void RemoveCharImage()
    {
       transform.GetChild(0).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
       transform.GetChild(1).transform.gameObject.SetActive(false);
    }
    private bool CheckAddDeck()
    {
        int backLineCharNum = 0;
        int frontLineCharNum = 0;

        for (int i = 0; i < 10; i++)
        {
            if (i < 5)
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
        if (backLineCharNum <= frontLineCharNum)
        {
            return false;

        }
        return true;
    }


}
