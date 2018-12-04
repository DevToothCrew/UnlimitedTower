using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FormationManager : MonoSingleton<FormationManager>
{


    // Use this for initialization
    public GameObject NewDropChar;


    void Awake()
    {

        Debug.Log("FormationManager call:)");

        // LoadFormation();




        //for (int i = 0; i < 10; i++)
        //{
        //    int deckNum = UserDataManager.Inst.formationOrderList[i];
        //    GameObject goDeck = GetDeck(deckNum);
        //    if (goDeck.transform.GetChild(0).gameObject.GetComponent<Image>().sprite)
        //    {
        //        Debug.Log("Load Open Close : " + deckNum);

        //        // 연결된 캐릭터를 찾을 방법이 없다?
        //        // 캐릭터 value값을 알면 연결할 수 있을 것이다.
        //        // 그런데 이것을 어떻게 찾을 것인가?

        //        // 복잡하게 생각하지말고
        //        // 가장 쉬운 방법
        //        // 데이터를 미리 저장한다든지

        //        //검색 속도가 넘 느리다?

        //        // 해당 덱에 캐릭터가 존재한다면
        //        // 그 캐릭터 원본을 찾아야한다.

        //        // 유저 데이터를 받는다.
        //        // 만약 awake함수가 호출되었을 때 기존 데이터가 있다면
        //        // 두 배로 증가한다.

        //        // hz


        //        //if(UserDataManager.Inst.formationDic.ContainsKey(deckNum))
        //        // {
        //        //     int charKey = UserDataManager.Inst.formationDic[deckNum];

        //        //     // 자식 개수가 0이 되서 for문에 접근하질 못한다.
        //        //     // 메모리를 낭비하는 쪽으로 가는건?
        //        //     int charCount = CharContentList.Inst.gameObject.transform.childCount;
        //        //     for (int j=0; j< charCount; j++)
        //        //     {
        //        //         GameObject charElement = CharContentList.Inst.gameObject.transform.GetChild(j).gameObject;
        //        //         if(charElement.GetComponent<CharContent>().CharDicKey == charKey)
        //        //         {
        //        //             goDeck.GetComponent<FormationDeck>().LinkedChar = charElement;
        //        //         }
        //        //     }                                      
        //        // }



        //        if (UserDataManager.Inst.formationDic.ContainsKey(deckNum))
        //        {
        //            int charKey = UserDataManager.Inst.formationDic[deckNum];

        //            // 자식 개수가 0이 되서 for문에 접근하질 못한다.
        //            // 메모리를 낭비하는 쪽으로 가는건?
        //            int charCount = LobbyManager.Inst.CharacterContentList.transform.childCount;
        //            for (int j = 0; j < charCount; j++)
        //            {
        //                GameObject charElement = CharContentList.Inst.gameObject.transform.GetChild(j).gameObject;
        //                if (charElement.GetComponent<CharContent>().CharDicKey == charKey)
        //                {
        //                    goDeck.GetComponent<FormationDeck>().LinkedChar = charElement;
        //                    //주변 덱을 연다.
        //                    OpenNewDeck(deckNum);
        //                    // 이미 덱이 존재했던 내용을 채운다.
        //                    LoadDeck(deckNum);
        //                }
        //            }
        //        }




        //    }
        //}


    }
    private void LoadFormation()
    {
        string deckName = null;
        foreach (KeyValuePair<int, int> dic in UserDataManager.Inst.formationDic)
        {
            Debug.Log("포메이션 세팅");

            // 포메이션 위치
            deckName = "Deck" + dic.Key;
            for (int i = 0; i < 10; i++)
            {
                 gameObject.transform.GetChild(i);
                if (gameObject.transform.GetChild(i).gameObject.name == deckName)
                {
                    Sprite sprite = Resources.Load<Sprite>("UI/CharaterImage/" + UserDataManager.Inst.characterDic[dic.Value].Name);
                    gameObject.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = sprite;
                }
            }
        }
    }

    public void OpenNewDeck(int deckNum)
    {
        switch (deckNum)
        {
            case 7:
                {
                    ActivateDeck(6);
                    ActivateDeck(8);

                    ActivateDeck(2);
                    break;
                }
            case 6:
            case 8:
                {
                    if(deckNum == 8)
                    {
                        ActivateDeck(deckNum + 1);
                    }
                    else
                    {
                        ActivateDeck(deckNum - 1);
                    }
                
                    ActivateDeck(deckNum - 5);
                    break;
                }


            case 5:
            case 9:
                {
                    ActivateDeck(deckNum-5);
                    break;
                }
        }

    }
    public void ActivateDeck(int deckNum)
    {
        // awake 가 끝나냐 자식을 호출할 수 있다?
        gameObject.transform.GetChild(deckNum).GetComponent<FormationDeck>().Active = true;
        gameObject.transform.GetChild(deckNum).transform.GetChild(1).gameObject.SetActive(true);
        gameObject.transform.GetChild(deckNum).transform.GetChild(2).gameObject.SetActive(false);
    }
    public void LoadDeck(int deckNum)
    {
        gameObject.transform.GetChild(deckNum).GetComponent<FormationDeck>().Active = true;
        gameObject.transform.GetChild(deckNum).transform.GetChild(1).gameObject.SetActive(false);
        gameObject.transform.GetChild(deckNum).transform.GetChild(2).gameObject.SetActive(false);
    }

    public GameObject GetDeck(int deckNum)
    {
        return gameObject.transform.GetChild(deckNum).gameObject;
    }
}
