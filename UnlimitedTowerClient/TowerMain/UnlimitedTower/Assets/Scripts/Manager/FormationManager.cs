using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FormationManager : MonoSingleton<FormationManager> {

    // Use this for initialization
  public  GameObject NewDropChar = null;


    void Awake () {

        UserDataManager.Inst.LoadFormation();

        for (int i = 0; i < 10; i++)
        {
            int deckNum = UserDataManager.Inst.formationOrderList[i];
            GameObject goDeck = GetDeck(deckNum);
            if (goDeck.transform.GetChild(0).gameObject.GetComponent<Image>().sprite)
            {
                Debug.Log("Load Open Close : " + deckNum);

                // 연결된 캐릭터를 찾을 방법이 없다?
                // 캐릭터 value값을 알면 연결할 수 있을 것이다.
                // 그런데 이것을 어떻게 찾을 것인가?
               if(UserDataManager.Inst.formationDic.ContainsKey(deckNum))
                {
                    UserDataManager.Inst.formationDic[deckNum]
                    goDeck.GetComponent<FormationDeck>().LinkedChar
                }
                

                //goDeck.GetComponent<FormationDeck>().LinkedChar = gameObject;
                OpenNewDeck(deckNum);

                // 이미 덱이 존재했던 내용을 채운다.
                LoadDeck(deckNum);
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
    void ActivateDeck(int deckNum)
    {
        gameObject.transform.GetChild(deckNum).GetComponent<FormationDeck>().Active = true;
        gameObject.transform.GetChild(deckNum).transform.GetChild(1).gameObject.SetActive(true);
        gameObject.transform.GetChild(deckNum).transform.GetChild(2).gameObject.SetActive(false);
    }
    void LoadDeck(int deckNum)
    {
        gameObject.transform.GetChild(deckNum).GetComponent<FormationDeck>().Active = true;
        gameObject.transform.GetChild(deckNum).transform.GetChild(1).gameObject.SetActive(false);
        gameObject.transform.GetChild(deckNum).transform.GetChild(2).gameObject.SetActive(false);
    }

    GameObject GetDeck(int deckNum)
    {
        return gameObject.transform.GetChild(deckNum).gameObject;
    }
}
