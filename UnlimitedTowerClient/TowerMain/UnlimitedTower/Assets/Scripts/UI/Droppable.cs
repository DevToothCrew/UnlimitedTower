using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using UnityEngine.EventSystems;

public class Droppable :
    MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
// 드롭 조작을 제어하기 위한 인터페이스를 상속한다
{
    #region OnPointerEnter
    // 드롭 영역에 표시되어 있는 아이콘
    [SerializeField] private Image iconImage;
    // 드롭 영역에 표시되어 있는 아이콘의 하이라이트 색
    [SerializeField] private Color highlightedColor;
    // 드롭 영역에 표시하고 있는 아이콘의 본래 색
    private Color normalColor;


    public GameObject FormationList;

    // 인스턴스를 로드할 때 Awake 메서드가 처리된 다음에 호출된다
    void Awake()
    {
        // 드롭 영역에 표시되어 있는 아이콘의 본래 색을 보존해둔다
        normalColor = iconImage.color;
        FormationList = GameObject.Find("FormationList");
    }

    // 마우스 커서가 영역에 들어왔을 때 호출된다
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (pointerEventData.dragging)
        {
            // 드래그 중이라며 드롭 영역에 표시되어 있는 아이콘 색을 하이라이트 색으로 변경한다
            iconImage.color = highlightedColor;
            //Debug.Log("OnPointerEnter Color : " + iconImage.color.ToString());
        }
    }
    #endregion

    #region OnPointerExit   
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (pointerEventData.dragging)
        {
            // 드래그 도중이라면 드롭 영역에 표시되어 있는 아이콘 색을 본래 색으로 되돌린다
            iconImage.color = normalColor;
        }
    }
    #endregion

    #region OnDrop
    public void OnDrop(PointerEventData pointerEventData)
    {
        int charIndex = -1;
        int deckNum = gameObject.GetComponent<FormationDeck>().DeckNum;
        if (UserDataManager.Inst.PutChar)
        {
            charIndex =  UserDataManager.Inst.PutChar.GetComponent<CharListContent>().CharDicKey;
        }
        else
        {
            Debug.Log("**********Error**********Not Find : PutChar GameObject");
            return;
        }


       


        // 만약 이미 포메이션에 놓인 캐릭터가 있다면
        if (UserDataManager.Inst.formationDic.ContainsValue(charIndex))
        {
            if (gameObject.GetComponent<Image>().sprite == null)
            {
                // 일단 캐릭터 인덱스가 dic에 있다는 것은 확인했다.
                // 문제는 캐릭터 인덱스가 어느 키 값(어느 덱)인지를 확인하면 된다.

               int key =  KeyByValue(UserDataManager.Inst.formationDic, charIndex);

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
            else
            {
                return;
            }

        }

        UserDataManager.Inst.formationDic.Add(deckNum, charIndex);

        Debug.Log("UserDataManager.Inst.formationDic : " + deckNum + " " + charIndex);


        // 드래그하고 있었던 아이콘의 Image 컴포넌트를 가져온다
        Image droppedImage = pointerEventData.pointerDrag.GetComponent<Image>();



        // 드랍했을 때 드랍한 본체를 알아야한다.

        // 드롭 영역에 표시되어 있는 아이콘의 스프라이트를
        // 드롭된 아이콘과 동일한 스프라이트로 변경하고 색을 본래 색으로 되돌린다

        // 스프라이트를 넣어준다.
        //gameObject.GetComponent<Image>().sprite = droppedImage.sprite;
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = droppedImage.sprite;










        // Debug.Log("확인 : " + pointerEventData.gameObject.name);

        //iconImage.sprite = droppedImage.sprite;
        iconImage.color = normalColor;

        Color color = UserDataManager.Inst.PutChar.GetComponent<Image>().color;
        Debug.Log("선택된 캐릭터");
        color.r = color.g = color.b = 0.35f;
        UserDataManager.Inst.PutChar.GetComponent<Image>().color = color;

        //Debug.Log("드랍 : " + iconImage.sprite.name + ", 이미지 색상 : " + iconImage.color);

    }
    #endregion


    GameObject GetOldDeckObject(int key)
    {
        for(int i=0; i<DEFINE.PARTY_MAX_NUM; i++)
        {
            if(FormationList.transform.GetChild(i).GetComponent<FormationDeck>().DeckNum == key)
            {
                return FormationList.transform.GetChild(i).gameObject;
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
