using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using UnityEngine.EventSystems;

public class Droppable : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region OnPointerEnter
    // 드롭 영역에 표시되어 있는 아이콘의 하이라이트 색
    [SerializeField] private Color highlightedColor;
    // 드롭 영역에 표시하고 있는 아이콘의 본래 색
    [SerializeField] private Color normalColor;
    [SerializeField] private Color closeColor;



    // 인스턴스를 로드할 때 Awake 메서드가 처리된 다음에 호출된다
    void Awake()
    {
        // 드롭 영역에 표시되어 있는 아이콘의 본래 색을 보존해둔다
        normalColor = new Color(1.0f, 1.0f, 1.0f);
        highlightedColor = new Color(0.5f, 0.5f, 1.0f);
        closeColor = new Color(1.0f, 0.5f, 0.5f);
     
    }

    // 마우스 커서가 영역에 들어왔을 때 호출된다
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (pointerEventData.dragging)
        {
            // 드래그 중이라며 드롭 영역에 표시되어 있는 아이콘 색을 하이라이트 색으로 변경한다
            if (GetComponent<FormationDeck>().Active)
            {
               gameObject.transform.GetChild(0).GetComponent<Image>().color = highlightedColor;
            }
            else
            {
                gameObject.transform.GetChild(0).GetComponent<Image>().color = closeColor;
            }


          
        }
    }
    #endregion

    #region OnPointerExit   
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (pointerEventData.dragging)
        {
            // 드래그 도중이라면 드롭 영역에 표시되어 있는 아이콘 색을 본래 색으로 되돌린다
            Debug.Log("OnPointerExit");
            gameObject.transform.GetChild(0).GetComponent<Image>().color = normalColor;
        }
    }
    #endregion

    #region OnDrop
    public void OnDrop(PointerEventData pointerEventData)
    {
        int charIndex = -1;
        int deckNum = gameObject.GetComponent<FormationDeck>().DeckNum;

        if(gameObject.GetComponent<FormationDeck>().Active == false)
        {
            Debug.Log("This Slot is close ");
            gameObject.transform.GetChild(0).GetComponent<Image>().color = normalColor;
            return;
        }


        if (FormationManager.NewDropChar)
        {         
            charIndex = FormationManager.NewDropChar.GetComponent<CharContent>().CharDicKey;
        }
        else
        {
            Debug.Log("**********Error**********Not Find : PutChar GameObject");
            return;
        }

        if(GetComponent<FormationDeck>().LinkedChar)
        {
            if(charIndex == GetComponent<FormationDeck>().LinkedChar.GetComponent<CharContent>().CharDicKey)
            {
                Debug.Log("This is same character!");
                gameObject.transform.GetChild(0).GetComponent<Image>().color = normalColor;
                return;
            }
        }
       


        // 기존 덱을 새로운 덱의 위치로 이동시킬 것이라면
        // error : 같은거 같은 곳에 드래그하면 생기는 에러
        if (UserDataManager.Inst.formationDic.ContainsValue(charIndex))
        {
            if (gameObject.transform.GetChild(0).GetComponent<Image>().sprite == null)
            {
                // 새로운 덱 위치로 기존 덱을 이동시킨다.
                MoveNewDeckPos(charIndex);
            }
            else
            {
                return;
            }

        }
        // 기존 덱 자리에 덮어 씌을 것이라면(error)
        else if(gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite)
        {
            // 기존에 덱에 있는 이미지를 삭제한다.
            RemoveOldDeck(deckNum);
        }

        AddNewDeck(deckNum, charIndex, pointerEventData.pointerDrag.GetComponent<Image>());



 

    }
    #endregion



   


   
    void AddNewDeck(int deckNum, int charIndex, Image charImage)
    {
        Debug.Log("UserDataManager.Inst.formationDic : " + deckNum + " " + charIndex);
        UserDataManager.Inst.formationDic.Add(deckNum, charIndex);
       

        // 드래그하고 있었던 아이콘의 Image 컴포넌트를 가져온다
        Image droppedImage = charImage;

        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = droppedImage.sprite;
        LobbyManager.Inst.FormationList.GetComponent<FormationManager>().OpenNewDeck(deckNum);



        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        Color color = FormationManager.NewDropChar.GetComponent<Image>().color;
        int charKey = FormationManager.NewDropChar.GetComponent<CharContent>().CharDicKey;
        UserDataManager.Inst.characterDic[charKey].OnFormation = true;


        Debug.Log("선택된 캐릭터");
        color.r = color.g = color.b = 0.35f;
        FormationManager.NewDropChar.GetComponent<Image>().color = color;

        gameObject.transform.GetChild(0).GetComponent<Image>().color = normalColor;
        gameObject.GetComponent<FormationDeck>().LinkedChar = FormationManager.NewDropChar;
    }

    void MoveNewDeckPos(int charIndex)
    {
        // 일단 캐릭터 인덱스가 dic에 있다는 것은 확인했다.
        // 문제는 캐릭터 인덱스가 어느 키 값(어느 덱)인지를 확인하면 된다.

        int key = KeyByValue(UserDataManager.Inst.formationDic, charIndex);

        if (UserDataManager.Inst.formationDic[key] == charIndex)
        {
            UserDataManager.Inst.formationDic.Remove(key);
            GameObject oldDeck = GetOldDeckObject(key);
            if (oldDeck)
            {
                if (oldDeck.transform.GetChild(0).GetComponent<Image>().sprite)
                {
                    Debug.Log("이미지 교체");
                    oldDeck.transform.GetChild(0).GetComponent<Image>().sprite = null;
                }
            }

        }
    }
    void RemoveOldDeck(int deckNum)
    {
        UserDataManager.Inst.formationDic.Remove(deckNum);
        gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite
            = null;

        gameObject.GetComponent<FormationDeck>().LinkedChar.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }



    GameObject GetOldDeckObject(int key)
    {
        GameObject formationList = LobbyManager.Inst.FormationList.GetComponent<FormationManager>().gameObject;
        for (int i=0; i<DEFINE.PARTY_MAX_NUM; i++)
        {

            //if (FormationManager.Inst.gameObject.transform.GetChild(i).GetComponent<FormationDeck>().DeckNum == key)
            //{
            //    return FormationManager.Inst.gameObject.transform.GetChild(i).gameObject;
            //}
         
            if (formationList.transform.GetChild(i).GetComponent<FormationDeck>().DeckNum == key)
            {
                return formationList.transform.GetChild(i).gameObject;
            }     
        }
        return null;
      
    }
    public static int KeyByValue(Dictionary<int, int> dic, int val)
    {
        int key = -1;
        foreach (KeyValuePair<int, int> pair in dic)
        {
            if (pair.Value == val)
            {
                key = pair.Key;
                break;
            }
        }
        return key;
    }



}
