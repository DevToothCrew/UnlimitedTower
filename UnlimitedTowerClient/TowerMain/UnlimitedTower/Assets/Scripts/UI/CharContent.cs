using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharContent : MonoBehaviour, IPointerClickHandler
{
    public int charDicKey;
    public CHAR_TYPE charType;

    public GameObject ChildCheckingImage;

    void Awake()
    {
        ChildCheckingImage.SetActive(false);
    }
 
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // 이미 덱에 캐릭터가 존재하면, 그 캐릭터를 뺀다.

        //if(charType == CHAR_TYPE.SERVANT)
        //{
        //    Debug.Log("서번트 추가");
        //    // CheckAddOrRemove(ref UserDataManager.Inst.servantDic, CharType);

        //}
        //else if (charType == CHAR_TYPE.MONSTER)
        //{
        //    Debug.Log("먼스터 추가");
        //    //CheckAddOrRemove(ref UserDataManager.Inst.monsterDic, CharType);   
        //}
        CheckAddOrRemove(charType);

    }


    // 이제 Character 딕셔너리가 두 개로 분리되어
    // Monster와 Servant로 되었다.
    // 그러면 Monster와 Servant 추가 혹은 제거하는 함수 역시 두 개의 함수로 분리되어야할까?
    private void CheckAddOrRemove(CHAR_TYPE getCharType)
    {
        if (UserDataManager.Inst.newServantDic.ContainsKey(charDicKey))
        {
            if (UserDataManager.Inst.newServantDic[charDicKey].onFormation == true)
            {
                if (getCharType == CHAR_TYPE.SERVANT)
                {
                    RemoveServantDeck();
                }
                else if (getCharType == CHAR_TYPE.MONSTER)
                {
                    RemoveMonsterDeck();
                }
            }
            else
            {
                if (getCharType == CHAR_TYPE.MONSTER)
                {
                    if (CheckAddDeck() == false)
                    {
                        Debug.Log("용병 수가 적어 몬스터를 추가할 수 없습니다.");
                        return;
                    }
                    AddMonsterDeck();
                }
                else if (getCharType == CHAR_TYPE.SERVANT)
                {
                    AddServantDeck();
                }

            }
        }
    }
    private void AddServantDeck()
    {
        int startNum = 0;
        string imageFath = "UI/CharaterImage/";
         
        for (int i = startNum; i < DEFINE.PARTY_MAX_NUM / 2 + startNum; i++)
        {
            int deckNum = UserDataManager.Inst.formationOrderList[i];
            // 빈 덱 검색
            if (UserDataManager.Inst.formationDic.ContainsKey(deckNum) == false)
            {
                // 캐릭터 넣기.
                //GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
                GameObject deck = FormationManager.Inst.Decks[deckNum];
                Sprite sprite = Resources.Load<Sprite>(imageFath + UserDataManager.Inst.newServantDic[charDicKey].name);

                // 덱에 캐릭터 오브젝트 연결
                deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                deck.GetComponent<FormationDeck>().LinkedChar = gameObject;
                deck.GetComponent<FormationDeck>().ShowEmptyText(false);


                // 캐릭터 사용중이라는 표시하기.
                UserDataManager.Inst.formationDic.Add(deckNum, charDicKey);
                Debug.Log("CharDicKey : " + charDicKey);
                UserDataManager.Inst.newServantDic[charDicKey].onFormation = true;
                UserDataManager.Inst.newServantDic[charDicKey].formationIndex = deckNum;

                ChildCheckingImage.SetActive(true);
                transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                return;

            }
        }
    }
    private void AddMonsterDeck()
    {
        int startNum = 5;
        string imageFath = "UI/MonsterImage/";

        for (int i = startNum; i < DEFINE.PARTY_MAX_NUM / 2 + startNum; i++)
        {
            int deckNum = UserDataManager.Inst.formationOrderList[i];
            // 빈 덱 검색
            if (UserDataManager.Inst.formationDic.ContainsKey(deckNum) == false)
            {
                // 캐릭터 넣기.
                //GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
                GameObject deck = FormationManager.Inst.Decks[deckNum];
                Sprite sprite = Resources.Load<Sprite>(imageFath + UserDataManager.Inst.newMonsterDic[charDicKey].name);

                // 덱에 캐릭터 오브젝트 연결
                deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                deck.GetComponent<FormationDeck>().LinkedChar = gameObject;
                deck.GetComponent<FormationDeck>().ShowEmptyText(false);


                // 캐릭터 사용중이라는 표시하기.
                UserDataManager.Inst.formationDic.Add(deckNum, charDicKey);
                Debug.Log("CharDicKey : " + charDicKey);
                UserDataManager.Inst.newMonsterDic[charDicKey].onFormation = true;
                UserDataManager.Inst.newMonsterDic[charDicKey].formationIndex = deckNum;

                ChildCheckingImage.SetActive(true);
                transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                return;

            }
        }
    }


    private void RemoveMonsterDeck()
    {
        Monster getMonster = null;
        if (UserDataManager.Inst.newMonsterDic[charDicKey].onFormation == true)
        {
            if (UserDataManager.Inst.newMonsterDic.TryGetValue(charDicKey, out getMonster))
            {
                int deckNum = getMonster.formationIndex;
                //GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
                GameObject deck = FormationManager.Inst.Decks[deckNum];
                RemoveCharImage();
                deck.GetComponent<FormationDeck>().RemoveDeck();

                ChildCheckingImage.SetActive(false);
                UserDataManager.Inst.newMonsterDic[charDicKey].onFormation = false;
            }
        }
    }
    private void RemoveServantDeck()
    {
        Servant getServant = null;
        if (UserDataManager.Inst.newServantDic[charDicKey].onFormation == true)
        {
            if (UserDataManager.Inst.newServantDic.TryGetValue(charDicKey, out getServant))
            {
                int deckNum = getServant.formationIndex;
                //GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
                GameObject deck = FormationManager.Inst.Decks[deckNum];
                RemoveCharImage();
                deck.GetComponent<FormationDeck>().RemoveDeck();

                ChildCheckingImage.SetActive(false);
                UserDataManager.Inst.newServantDic[charDicKey].onFormation = false;
            }
        }
    }


    public void RemoveCharImage()
    {
        transform.GetChild(0).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
        ChildCheckingImage.SetActive(false);
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





    //Old Code
    //private void CheckAddOrRemove(ref Dictionary<int, Character> charDic, CHAR_TYPE charType)
    //{
    //    if (charDic.ContainsKey(charDicKey))
    //    {
    //        if (charDic[charDicKey].OnFormation == true)
    //        {
    //            RemoveDeck(ref charDic, charType);
    //        }
    //        else
    //        {
    //            if(charType == CHAR_TYPE.MONSTER)
    //            {
    //                if (CheckAddDeck() == false)
    //                {
    //                    Debug.Log("용병 수가 적어 몬스터를 추가할 수 없습니다.");
    //                    return;
    //                }
    //            }
    //            AddDeck(ref charDic, charType);
    //        }
    //    }
    //}
    //private void RemoveDeck(ref Dictionary<int, Character> charDic, CHAR_TYPE charType)
    //{
    //    Character character;
    //    if (charDic[charDicKey].OnFormation == true)
    //    {
    //        if(charDic.TryGetValue(charDicKey, out character))
    //        {
    //            int deckNum = character.FormationIndex;
    //            //GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
    //            GameObject deck = FormationManager.Inst.Decks[deckNum];
    //            RemoveCharImage();
    //            deck.GetComponent<FormationDeck>().RemoveDeck();

    //            ChildCheckingImage.SetActive(false);
    //            charDic[charDicKey].OnFormation = false;
    //        }
    //    }          
    //}
    //private void AddDeck(ref Dictionary<int, Character> charDic, CHAR_TYPE charType)
    //{
    //    int startNum = -1;
    //    string imageFath = null;
    //    if (charType == CHAR_TYPE.SERVANT)
    //    {
    //        startNum = 0;
    //        imageFath = "UI/CharaterImage/";
    //    }
    //    else if(charType == CHAR_TYPE.MONSTER)
    //    {
    //        startNum = 5;
    //        imageFath = "UI/MonsterImage/";
    //    }
    //    else
    //    {
    //        Debug.Log("error : both servant and monster not.");
    //            return;
    //    }


    //    for (int i = startNum; i < DEFINE.PARTY_MAX_NUM / 2 + startNum; i++)
    //    {
    //        int deckNum = UserDataManager.Inst.formationOrderList[i];
    //        // 빈 덱 검색
    //        if (UserDataManager.Inst.formationDic.ContainsKey(deckNum) == false)
    //        {
    //            // 캐릭터 넣기.
    //            //GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
    //            GameObject deck = FormationManager.Inst.Decks[deckNum];
    //            Sprite sprite = Resources.Load<Sprite>(imageFath + charDic[charDicKey].Name);

    //            // 덱에 캐릭터 오브젝트 연결
    //            deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
    //            deck.GetComponent<FormationDeck>().LinkedChar = gameObject;
    //            deck.GetComponent<FormationDeck>().ShowEmptyText(false);


    //            // 캐릭터 사용중이라는 표시하기.
    //            UserDataManager.Inst.formationDic.Add(deckNum, charDicKey);
    //            Debug.Log("CharDicKey : " + charDicKey);
    //            charDic[charDicKey].OnFormation = true;
    //            charDic[charDicKey].FormationIndex = deckNum;

    //            ChildCheckingImage.SetActive(true);
    //            transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
    //            return;

    //        }
    //    }
    //}


  
}
