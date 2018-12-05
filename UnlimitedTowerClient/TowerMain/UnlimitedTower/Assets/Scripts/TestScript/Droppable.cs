using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using UnityEngine.EventSystems;

public class Droppable : MonoBehaviour, IDropHandler, 
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region OnPointerEnter
    // 드롭 영역에 표시되어 있는 아이콘의 하이라이트 색
    [SerializeField] private Color highlightedColor;
    // 드롭 영역에 표시하고 있는 아이콘의 본래 색
    [SerializeField] private Color normalColor;
    [SerializeField] private Color closeColor;
    [SerializeField] private Color usedColor;

    // Deck으로 이름을 정정할지 생각해볼것.
    private FormationDeck formationDeckCS;
    private Image charImage;



    // 인스턴스를 로드할 때 Awake 메서드가 처리된 다음에 호출된다
    void Awake()
    {
        // 드롭 영역에 표시되어 있는 아이콘의 본래 색을 보존해둔다
        normalColor = new Color(1.0f, 1.0f, 1.0f);
        highlightedColor = new Color(0.5f, 0.5f, 1.0f);
        closeColor = new Color(1.0f, 0.5f, 0.5f);
        usedColor = new Color(0.35f, 0.35f, 0.35f);
        formationDeckCS = GetComponent<FormationDeck>();
        charImage = GetChildObject(0).GetComponent<Image>();


    }

    // 마우스 커서가 영역에 들어왔을 때 호출된다
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (pointerEventData.dragging)
        {
            // 드래그 중이라며 드롭 영역에 표시되어 있는 아이콘 색을 하이라이트 색으로 변경한다


            //if (GetComponent<FormationDeck>().Active)
            //{
            //    this.charImage.color = highlightedColor;
            //}
            //else
            //{
            //    this.charImage.color = closeColor;
            //}


          
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
            this.charImage.color = normalColor;
        }
    }
    #endregion

    #region OnDrop
    public void OnDrop(PointerEventData pointerEventData)
    {
        int charIndex = -1;
        int deckNum = formationDeckCS.DeckNum;

        // 활성화된 덱이 아니면 드롭을 종료한다.
        //if(formationDeckCS.Active == false)
        //{
        //    Debug.Log("This Slot is close ");
        //    this.charImage.color = normalColor;
        //    return;
        //}

        // 현재 곧 드랍할 오브젝트의 캐릭터의 인덱스값을 저장한다.
        if (FormationManager.NewDropChar)
        {         
            charIndex = FormationManager.NewDropChar.GetComponent<CharContent>().CharDicKey;
        }
        else
        {
            Debug.Log("**********Error**********Not Find : PutChar GameObject");
            return;
        }

        // 같은 캐릭터를 같은 자리에 올려둘 경우
        if(formationDeckCS.LinkedChar)
        {
            if(charIndex == formationDeckCS.LinkedChar.GetComponent<CharContent>().CharDicKey)
            {
                Debug.Log("This is same character!");
                this.charImage.color = normalColor;
                return;
            }
        }
       


        // 기존 덱을 새로운 덱의 위치로 이동시킬 것이라면
        if (UserDataManager.Inst.formationDic.ContainsValue(charIndex))
        {
            if (GetChildObject(0).GetComponent<Image>().sprite == null)
            {
                // 새로운 덱 위치로 기존 덱을 이동시킨다.
                MoveNewDeck(charIndex);
            }
            else
            {
                return;
            }

        }
        // 기존 덱 자리에 덮어 씌을 것이라면
        else if(GetChildObject(0).GetComponent<Image>().sprite)
        {
            // 기존에 덱에 있는 이미지를 삭제한다.
            RemoveOldDeck(charIndex, deckNum);
        }

        // 둘다 이미 덱에 위에 있는데
        // 겹치는 경우, 빠져나가는 캐릭은 어떻게 처리하나

        // 조건 1: 캐릭터 초상화가 이미 덱에 있다.
        // 조건 2:
        else if(true)
        {

        }



        AddNewDeck(deckNum, charIndex, pointerEventData.pointerDrag.GetComponent<Image>());
    }
    #endregion





    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log("On Click : Formation");

        // 이미 덱에 캐릭터가 존재하면
        if(transform.GetChild(0).gameObject.GetComponent<Image>().sprite)
        {
            transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
            GetComponent<FormationDeck>().LinkedChar.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);

            //캐릭터 삭제
            UserDataManager.Inst.formationDic.Remove(GetComponent<FormationDeck>().DeckNum);
          //  GetComponent<FormationDeck>().LinkedChar.GetComponent<CharContent>().OnFormation = false;

        }
      




    }



    void AddNewDeck(int deckNum, int charIndex, Image charImage)
    {
        Debug.Log("UserDataManager.Inst.formationDic : " + deckNum + " " + charIndex);
        // 덱에 추가되었으므로 포메이션 딕셔너리에 추가한다.
        UserDataManager.Inst.formationDic.Add(deckNum, charIndex);
       
        // 드래그하고 있었던 Image 컴포넌트를 가져온다
        Image droppedImage = charImage;

        this.charImage.sprite = droppedImage.sprite;
        LobbyManager.Inst.FormationList.GetComponent<FormationManager>().OpenNewDeck(deckNum);
        UserDataManager.Inst.servantDic[charIndex].OnFormation = true;
        UserDataManager.Inst.servantDic[charIndex].FormationIndex = deckNum;
        FormationManager.NewDropChar.GetComponent<Image>().color = usedColor;





        formationDeckCS.LinkedChar = FormationManager.NewDropChar;
        GetChildObject(1).gameObject.SetActive(false);
        this.charImage.color = normalColor;
    }

    void MoveNewDeck(int charIndex)
    {
        // 일단 캐릭터 인덱스가 dic에 있다는 것은 확인했다.
        // 문제는 캐릭터 인덱스가 어느 키 값(어느 덱)인지를 확인하면 된다.

        int deckNum = KeyByValue(UserDataManager.Inst.formationDic, charIndex);

        if (UserDataManager.Inst.formationDic[deckNum] == charIndex)
        {
            UserDataManager.Inst.formationDic.Remove(deckNum);
            UserDataManager.Inst.servantDic[charIndex].FormationIndex = -1;
            GameObject oldDeck = GetOldDeckObject(deckNum);
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
    void RemoveOldDeck(int charIndex, int deckNum)
    {
        UserDataManager.Inst.servantDic[charIndex].FormationIndex = -1;
        UserDataManager.Inst.formationDic.Remove(deckNum);
        GetChildObject(0).GetComponent<Image>().sprite  = null;

        formationDeckCS.LinkedChar.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }



    GameObject GetOldDeckObject(int deckNum)
    {
        GameObject formationList = LobbyManager.Inst.FormationList.GetComponent<FormationManager>().gameObject;
        for (int i=0; i<DEFINE.PARTY_MAX_NUM; i++)
        {      
            if (formationList.transform.GetChild(i).GetComponent<FormationDeck>().DeckNum == deckNum)
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

    private GameObject GetChildObject(int childNum)
    {
        return transform.GetChild(childNum).gameObject;
    }

}
