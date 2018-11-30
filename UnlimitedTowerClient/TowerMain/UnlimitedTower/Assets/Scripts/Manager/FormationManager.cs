using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoSingleton<FormationManager> {


    public Dictionary<int, GameObject> decDic = new Dictionary<int, GameObject>();

	// Use this for initialization
	void Awake () {
		//decDid
        for(int i=0; i<10; i++)
        {
            decDic.Add(i, gameObject.transform.GetChild(i).gameObject);
        }

	}
	
    public void OpenNewDeck(int deckNum)
    {
        switch(deckNum)
        {
            case 7:
                {
                    decDic[6].GetComponent<FormationDeck>().Enable = true;
                    decDic[6].transform.GetChild(1).gameObject.SetActive(true);
                    decDic[6].transform.GetChild(2).gameObject.SetActive(false);


                    decDic[8].GetComponent<FormationDeck>().Enable = true;
                    decDic[8].transform.GetChild(1).gameObject.SetActive(true);
                    decDic[8].transform.GetChild(2).gameObject.SetActive(false);

                    Debug.Log("1 Deck & 3 Deck Active");
                    // 이거 그냥 덱을 decDic에 포함하는 낫지 않을려나?
                    // 일일이 찾기 넘 까다롭다.


                    //그러면 이 처리를 어느 스크립트에서 하는게 적절할가?
                   
                    break;
                }
            case 6:
                {
                    decDic[5].GetComponent<FormationDeck>().Enable = true;
                    decDic[5].transform.GetChild(1).gameObject.SetActive(true);
                    decDic[5].transform.GetChild(2).gameObject.SetActive(false);
                    break;
                }
            case 8:
                {
                    decDic[9].GetComponent<FormationDeck>().Enable = true;
                    decDic[9].transform.GetChild(1).gameObject.SetActive(true);
                    decDic[9].transform.GetChild(2).gameObject.SetActive(false);
                    break;
                }
        }
    }
}
