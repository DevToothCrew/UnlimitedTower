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
            if (gameObject.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<Image>().sprite)
            {
                Debug.Log("Load Open Close : " + i);
                OpenNewDeck(i);

                // 이미 덱이 존재했던 내용을 채운다.
                LoadDeck(i);

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
        gameObject.transform.GetChild(deckNum).GetComponent<FormationDeck>().Enable = true;
        gameObject.transform.GetChild(deckNum).transform.GetChild(1).gameObject.SetActive(true);
        gameObject.transform.GetChild(deckNum).transform.GetChild(2).gameObject.SetActive(false);
    }
    void LoadDeck(int deckNum)
    {
        gameObject.transform.GetChild(deckNum).GetComponent<FormationDeck>().Enable = true;
        gameObject.transform.GetChild(deckNum).transform.GetChild(1).gameObject.SetActive(false);
        gameObject.transform.GetChild(deckNum).transform.GetChild(2).gameObject.SetActive(false);
    }
}
