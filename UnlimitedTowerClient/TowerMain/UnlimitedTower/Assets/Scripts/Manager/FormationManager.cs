using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FormationManager : MonoBehaviour
{


    // Use this for initialization
   static  public GameObject NewDropChar;
    
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
        gameObject.transform.GetChild(deckNum).GetComponent<FormationDeck>().Active = true;
        gameObject.transform.GetChild(deckNum).transform.GetChild(1).gameObject.SetActive(true);
        gameObject.transform.GetChild(deckNum).transform.GetChild(2).gameObject.SetActive(false);
    }
    // 이름 반드시 바꿀것. 오해하기 쉬운 이름이다.
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
