using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharContent : MonoBehaviour, IPointerClickHandler
{
    public int CharDicKey;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log("On Click : CharContent");


        if(UserDataManager.Inst.characterDic[CharDicKey].OnFormation == true)
        {
            int formationIndex = UserDataManager.Inst.characterDic[CharDicKey].FormationIndex;
            GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(formationIndex).gameObject;
            deck.GetComponent<FormationDeck>().RemoveDeck();
            deck.GetComponent<FormationDeck>().SetEmptyText(true);
        }
        //   덱에 새로 캐릭터 추가.
        else
        {
            for (int i = 0; i < 10; i++)
            {
                int deckNum = UserDataManager.Inst.formationOrderList[i];
                if (UserDataManager.Inst.formationDic.ContainsKey(deckNum) != true)
                {
                    // 캐릭터 넣기.
                    GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
                    Sprite sprite = Resources.Load<Sprite>("UI/CharaterImage/" + UserDataManager.Inst.characterDic[CharDicKey].Name);


                    // 덱에 캐릭터 오브젝트 연결
                    deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                    deck.GetComponent<FormationDeck>().LinkedChar = gameObject;
                    deck.GetComponent<FormationDeck>().SetEmptyText(false);


                    // 캐릭터 사용중이라는 표시하기.
                    UserDataManager.Inst.formationDic.Add(deckNum, CharDicKey);
                    UserDataManager.Inst.characterDic[CharDicKey].OnFormation = true;
                    UserDataManager.Inst.characterDic[CharDicKey].FormationIndex = deckNum;

                    GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                    return;

                }
            }
        }


       


    }




}
