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
        PacketManager.Inst.Request_SaveFormation();
        BeSaved = true;
        // 저장 버튼을 안눌렀으면

        // 저장 버튼을 안눌렸으면
        // 이전 덱으로 되돌아 가야하나?
    }

}

// 히어로 넘버를 0, 1, 2 중 하나로