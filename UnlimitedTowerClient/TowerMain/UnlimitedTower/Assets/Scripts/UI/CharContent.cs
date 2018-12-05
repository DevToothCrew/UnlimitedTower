using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharContent : MonoBehaviour, IPointerClickHandler
{

    // Use this for initialization

    public int CharDicKey;
    public bool OnFormation = false;



    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log("On Click : CharContent");


        if(UserDataManager.Inst.characterDic[CharDicKey].OnFormation == true)
        {
            int formationIndex = UserDataManager.Inst.characterDic[CharDicKey].FormationIndex;
            GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(formationIndex).gameObject;
            deck.GetComponent<FormationDeck>().RemoveDeck();
        }
        //   덱에 새로 캐릭터 추가.
        else
        {
            for (int i = 0; i < 10; i++)
            {
                int deckNum = UserDataManager.Inst.formationOrderList[i];
                if (UserDataManager.Inst.formationDic.ContainsKey(deckNum) != true
                    && UserDataManager.Inst.characterDic[CharDicKey].OnFormation == false)
                {
                    // 캐릭터 넣기.
                    GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
                    Sprite sprite = Resources.Load<Sprite>("UI/CharaterImage/" + UserDataManager.Inst.characterDic[CharDicKey].Name);
                    deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;

                    // 덱에 캐릭터 오브젝트 연결
                    deck.GetComponent<FormationDeck>().LinkedChar = gameObject;
                    UserDataManager.Inst.formationDic.Add(deckNum, CharDicKey);

                    // 캐릭터 사용중이라는 표시하기.
                    GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                    UserDataManager.Inst.characterDic[CharDicKey].OnFormation = true;
                    UserDataManager.Inst.characterDic[CharDicKey].FormationIndex = deckNum;
                    return;

                }
            }
        }


       


    }




}
