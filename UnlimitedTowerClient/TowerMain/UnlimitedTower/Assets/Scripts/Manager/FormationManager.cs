using UnityEngine;
using UnityEngine.UI;

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

        Debug.Log("OnClickSaveFormation");

        //UserInfo.Inst.getAddServant("dd");


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
                    GameObject charContent = Decks[i].GetComponent<FormationDeck>().LinkedChar;
                   // charContent.GetComponent<CharContent>()

                }
                if (DeckImages[i].GetComponent<Image>())
                {
                    DeckImages[i].GetComponent<Image>().sprite = null;
                }
                if (DeckTexts[i].GetComponent<Text>())
                {
                    DeckTexts[i].SetActive(true);
                }

                // 어떠한 값을 리셋하여 해결하면 되는가???
            }







        }
    }

}

// attackingOrder myIndex targetIndex actionType
// 0~9까지는 플레이어
// 10~19






