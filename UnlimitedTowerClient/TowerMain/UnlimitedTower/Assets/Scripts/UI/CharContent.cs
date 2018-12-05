﻿using System.Collections;
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

        if(CharType == 0)
        {
            AddDeck(ref UserDataManager.Inst.servantDic, CharType);
        }
        else if (CharType == 1)
        {
            AddDeck(ref UserDataManager.Inst.monsterDic, CharType);
        }

        #region
        //if(UserDataManager.Inst.servantDic.ContainsKey(CharDicKey))
        //{
        //    if (UserDataManager.Inst.servantDic[CharDicKey].OnFormation == true)
        //    {
        //        int deckNum = UserDataManager.Inst.servantDic[CharDicKey].FormationIndex;
        //        GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
        //        RemoveCharImage();
        //        deck.GetComponent<FormationDeck>().RemoveDeck();
        //        deck.GetComponent<FormationDeck>().ShowEmptyText(true);

        //        transform.GetChild(1).gameObject.SetActive(false);
        //    }
        //    //   덱에 새로 캐릭터 추가.
        //    else
        //    {
        //        for (int i = 0; i < DEFINE.PARTY_MAX_NUM/2; i++)
        //        {
        //            int deckNum = UserDataManager.Inst.formationOrderList[i];
        //            // 빈 덱 검색
        //            if (UserDataManager.Inst.formationDic.ContainsKey(deckNum) == false)
        //            {
        //                // 캐릭터 넣기.
        //                GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
        //                Sprite sprite = Resources.Load<Sprite>("UI/CharaterImage/" + UserDataManager.Inst.servantDic[CharDicKey].Name);

        //                // 덱에 캐릭터 오브젝트 연결
        //                deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        //                deck.GetComponent<FormationDeck>().LinkedChar = gameObject;
        //                deck.GetComponent<FormationDeck>().ShowEmptyText(false);


        //                // 캐릭터 사용중이라는 표시하기.
        //                UserDataManager.Inst.formationDic.Add(deckNum, CharDicKey);
        //                UserDataManager.Inst.servantDic[CharDicKey].OnFormation = true;
        //                UserDataManager.Inst.servantDic[CharDicKey].FormationIndex = deckNum;

        //                transform.GetChild(1).gameObject.SetActive(true);
        //                transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        //                return;

        //            }
        //        }
        //    }
        //}     
        #endregion
    }

    public void AddDeck(ref Dictionary<int, Character> _charDic, int charType)
    {
        if (_charDic.ContainsKey(CharDicKey))
        {
            if (_charDic[CharDicKey].OnFormation == true)
            {
                int deckNum = _charDic[CharDicKey].FormationIndex;
                GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
                RemoveCharImage();
                deck.GetComponent<FormationDeck>().RemoveDeck();
                deck.GetComponent<FormationDeck>().ShowEmptyText(true);

                transform.GetChild(1).gameObject.SetActive(false);
            }
            //   덱에 새로 캐릭터 추가.
            else
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
                        Sprite sprite = Resources.Load<Sprite>(imageFath + _charDic[CharDicKey].Name);

                        // 덱에 캐릭터 오브젝트 연결
                        deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                        deck.GetComponent<FormationDeck>().LinkedChar = gameObject;
                        deck.GetComponent<FormationDeck>().ShowEmptyText(false);


                        // 캐릭터 사용중이라는 표시하기.
                        UserDataManager.Inst.formationDic.Add(deckNum, CharDicKey);
                        _charDic[CharDicKey].OnFormation = true;
                        _charDic[CharDicKey].FormationIndex = deckNum;

                        transform.GetChild(1).gameObject.SetActive(true);
                        transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                        return;

                    }
                }
            }
        }
    }



    public void RemoveCharImage()
    {
       transform.GetChild(0).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
       transform.GetChild(1).transform.gameObject.SetActive(false);
    }




}
