using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoSingleton<FormationManager> {


    // public Dictionary<int, GameObject> decDic = new Dictionary<int, GameObject>();
    public List<GameObject> deckList= new List <GameObject>();
    public GameObject formation;

    // Use this for initialization
    void Awake () {

        UserDataManager.Inst.LoadFormation();
        //decDid

        // gameObject.name = "ddd";


        //for(int i=0; i<10; i++)
        //{
        //    deckList.Add(formation.transform.GetChild(i).gameObject);
        //    // decDic.Add(i, gameObject.transform.GetChild(i).gameObject);
        //   // deckList.Add(gameObject.transform.GetChild(i).gameObject);
        //}

        Debug.Log("Awake Formaiton");

        //UserDataManager.Inst.loaderEvent += LoadFormation;

	}


    public void LoadFormation()
    {

    }

    public void OpenNewDeck(int deckNum)
    {
        switch (deckNum)
        {
            case 7:
                {
                    ActiveDeck(6);
                    ActiveDeck(8);
                

                    Debug.Log("1 Deck & 3 Deck Active");
                    // 이거 그냥 덱을 decDic에 포함하는 낫지 않을려나?
                    // 일일이 찾기 넘 까다롭다.


                    //그러면 이 처리를 어느 스크립트에서 하는게 적절할가?

                    break;
                }
            case 6:
                {
                    ActiveDeck(5);
                  
                    break;
                }
            case 8:
                {
                    ActiveDeck(9);
                    break;
                }
        }

    }
    void ActiveDeck(int deckNum)
    {
        //gameObject.transform.GetChild(deckNum).GetComponent<FormationDeck>().Enable = true;
        //gameObject.transform.GetChild(deckNum).transform.GetChild(1).gameObject.SetActive(true);
        //gameObject.transform.GetChild(deckNum).transform.GetChild(2).gameObject.SetActive(false);

        deckList[deckNum].GetComponent<FormationDeck>().Enable = true;
        deckList[deckNum].transform.GetChild(1).gameObject.SetActive(true);
        deckList[deckNum].transform.GetChild(2).gameObject.SetActive(false);
    }
}
