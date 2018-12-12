using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharContent : MonoBehaviour, IPointerClickHandler
{
    public int CharDicKey;
    public CHAR_TYPE CharType;

    public GameObject childCheckingImage;

    void Awake()
    {
        childCheckingImage.SetActive(false);
    }
 
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // 이미 덱에 캐릭터가 존재하면, 그 캐릭터를 뺀다.

        if(CharType == CHAR_TYPE.SERVANT)
        {
            CheckAddOrRemove(ref UserDataManager.Inst.servantDic, CharType);

        }
        else if (CharType == CHAR_TYPE.MONSTER)
        {
            Debug.Log("먼스터 추가");
            CheckAddOrRemove(ref UserDataManager.Inst.monsterDic, CharType);
        }
       
    }


    private void CheckAddOrRemove(ref Dictionary<int, Character> charDic, CHAR_TYPE charType)
    {
        if (charDic.ContainsKey(CharDicKey))
        {
            if (charDic[CharDicKey].OnFormation == true)
            {
                RemoveDeck(ref charDic, charType);
            }
            else
            {
                if(charType == CHAR_TYPE.MONSTER)
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
    private void RemoveDeck(ref Dictionary<int, Character> charDic, CHAR_TYPE charType)
    {
        Character character;
        if (charDic[CharDicKey].OnFormation == true)
        {
            if(charDic.TryGetValue(CharDicKey, out character))
            {
                int deckNum = character.FormationIndex;
                //GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
                GameObject deck = FormationManager.Inst.Decks[deckNum];
                RemoveCharImage();
                deck.GetComponent<FormationDeck>().RemoveDeck();

                childCheckingImage.SetActive(false);
                charDic[CharDicKey].OnFormation = false;
            }
        }

           
          
    }
    private void AddDeck(ref Dictionary<int, Character> charDic, CHAR_TYPE charType)
    {
        int startNum = -1;
        string imageFath = null;
        if (charType == CHAR_TYPE.SERVANT)
        {
            startNum = 0;
            imageFath = "UI/CharaterImage/";
        }
        else if(charType == CHAR_TYPE.MONSTER)
        {
            startNum = 5;
            imageFath = "UI/MonsterImage/";
        }
        else
        {
            Debug.Log("error : both servant and monster not.");
                return;
        }


        for (int i = startNum; i < DEFINE.PARTY_MAX_NUM / 2 + startNum; i++)
        {
            int deckNum = UserDataManager.Inst.formationOrderList[i];
            // 빈 덱 검색
            if (UserDataManager.Inst.formationDic.ContainsKey(deckNum) == false)
            {
                // 캐릭터 넣기.
                //GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
                GameObject deck = FormationManager.Inst.Decks[deckNum];
                Sprite sprite = Resources.Load<Sprite>(imageFath + charDic[CharDicKey].Name);

                // 덱에 캐릭터 오브젝트 연결
                deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                deck.GetComponent<FormationDeck>().LinkedChar = gameObject;
                deck.GetComponent<FormationDeck>().ShowEmptyText(false);


                // 캐릭터 사용중이라는 표시하기.
                UserDataManager.Inst.formationDic.Add(deckNum, CharDicKey);
                Debug.Log("CharDicKey : " + CharDicKey);
                charDic[CharDicKey].OnFormation = true;
                charDic[CharDicKey].FormationIndex = deckNum;

                childCheckingImage.SetActive(true);
                transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                return;

            }
        }
    }

    public void RemoveCharImage()
    {
       transform.GetChild(0).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
       childCheckingImage.SetActive(false);
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
