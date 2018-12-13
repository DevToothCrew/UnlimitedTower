using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FormationManager : MonoSingleton<FormationManager>
{
    public GameObject[] Decks = new GameObject[10];
    public GameObject[] DeckImages = new GameObject[10];
    public GameObject[] DeckTexts = new GameObject[10];


    public bool BeSaved = false;

    // 하이어라키 뷰 오브젝트가 바뀌면
    // GetChild 접근이 에러가 날 확률이 급격히 증가한다.
    // 즉, 하이어라키에서 값이 바껴도 에러가 최소한으로 나도록 설계하면 된다.
    private void Awake()
    {
        Debug.Log("Awake : FormationManager");
        for(int i=0; i<10; i++)
        {
            if(Decks[i])
            {
                DeckImages[i] = Decks[i].transform.GetChild(0).gameObject;
                DeckTexts[i] = Decks[i].transform.GetChild(1).gameObject;           
            }
        }
    }

    public void OnClickSaveFormation()
    { 
        // ### 포메이션 패킷을 보낸다.
  

        PacketManager.Inst.Request_SaveFormation();
        BeSaved = true;
    }
    public void ResetFormation()
    {
        for(int i=0; i<10; i++)
        {
            if(i != DEFINE.HERO_FORMATION_NUM)
            {
                if (Decks[i].GetComponent<FormationDeck>())
                {
                    Decks[i].GetComponent<FormationDeck>().LinkedChar = null;
                }
                if (DeckImages[i].GetComponent<Image>())
                {
                    DeckImages[i].GetComponent<Image>().sprite = null;
                }
                if (DeckTexts[i].GetComponent<Text>())
                {
                    DeckTexts[i].SetActive(true);
                }
            }

           


        }
    }

}





