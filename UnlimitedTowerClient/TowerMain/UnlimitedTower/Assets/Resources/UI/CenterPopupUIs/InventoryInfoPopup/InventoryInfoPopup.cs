using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInfoPopup : MonoBehaviour {
    
    // 인벤토리 슬롯
    public Transform slotparent;
    List<InventorySlotScript> invenslotlist = new List<InventorySlotScript>();
    // SORT버튼들 
    public List<GameObject> sortBtnList;
    // 페이지번호 Text
    public Text pageText;

    public GameObject SellingModeObj;
    public GameObject DisplayModeObj;

    // STATE VARIABLES
    public enum InvenState
    {
        deregistered,
        EquipMent,
        ETC
    }
    [Space(10)]
    [Header("Window State Variables")]
    // 상태
    public InvenState inventoryState;
    // 몇번째 단계에 있는지
    int _curDisplayNum = 0;
    public int curDisplayNum
    {
        get
        {
            return _curDisplayNum;
        }
        set
        {
            _curDisplayNum = value;

            switch (inventoryState)
            {
                case InvenState.EquipMent:
                    pageText.text = (_curDisplayNum+1) + "/" + (UserDataManager.Inst.MountItemList.Count / slotparent.childCount+1) + "P";
                    break;
                case InvenState.ETC:
                    pageText.text = (_curDisplayNum+1) + "/" + (UserDataManager.Inst.EtcItemList.Count / slotparent.childCount + 1) + "P";
                    break;
            }
        }
    }


    public enum MODE_TYPE
    {
        JUST_DISPLAY,
        SELLING
    }
    public MODE_TYPE modeType;
    public List<UserMountItemData> sellingMountItemList;
    public List<UserEtcItemData> sellingETCItemList;
    public static System.Action<UserMountItemData> mountitemListChanged;
    public static System.Action<UserEtcItemData> etcitemListChanged;
    public static System.Action modeChanged;

    // MODE_TYPE
    public void ToJustDisplayMode()
    {
        modeType = MODE_TYPE.JUST_DISPLAY;

        sellingMountItemList = new List<UserMountItemData>();
        sellingETCItemList = new List<UserEtcItemData>();

        if (modeChanged != null)
        {
            modeChanged();
        }

        // 셀링바 업데이트
        SellingModeObj.SetActive(false);
        DisplayModeObj.SetActive(true);
    }
    public void ToSellingMode()
    {
        modeType = MODE_TYPE.SELLING;

        if (modeChanged != null)
        {
            modeChanged();
        }

        // 셀링바 업데이트
        SellingModeObj.SetActive(true);
        DisplayModeObj.SetActive(false);
    }


    // 아이콘 개수
    int totalIconCount
    {
        get
        {
            return invenslotlist.Count;
        }
    }




    public void Display_equipitems(int displayNum)
    {
        inventoryState = InvenState.EquipMent;
        this.curDisplayNum = displayNum;

        // 창 초기화
        for (int i = 0; i < invenslotlist.Count; i++)
        {
            invenslotlist[i].ToLocked();
        }

        // 아이템 등록
        int startindex = curDisplayNum * totalIconCount;
        int endindex = (curDisplayNum + 1) * totalIconCount;
        for (int itemIndex = startindex; itemIndex < endindex && itemIndex < UserDataManager.Inst.MountItemList.Count; itemIndex++)
        {
            int iconIndex = itemIndex - startindex;
            InventorySlotScript slot = invenslotlist[iconIndex];
            slot.Register(UserDataManager.Inst.MountItemList[itemIndex]);
        }



        // 버튼들 생기게하기
        for (int i = 0; i < sortBtnList.Count; i++)
        {
            sortBtnList[i].gameObject.SetActive(true);
        }
    }
    public void Display_ETCitems(int displayNum)
    {
        inventoryState = InvenState.ETC;
        this.curDisplayNum = displayNum;


        // 창 초기화
        for (int i = 0; i < invenslotlist.Count; i++)
        {
            invenslotlist[i].ToLocked();
        }

        // ETC아이템 등록
        int startindex = curDisplayNum * totalIconCount;
        int endindex = (curDisplayNum + 1) * totalIconCount;
        for (int itemIndex = startindex; itemIndex < endindex && itemIndex < UserDataManager.Inst.EtcItemList.Count; itemIndex++)
        {
            int iconIndex = itemIndex - curDisplayNum * totalIconCount;
            InventorySlotScript slot = invenslotlist[iconIndex];
            slot.Register(UserDataManager.Inst.EtcItemList[itemIndex]);
        }


        // 버튼들 없애기
        for (int i = 0; i < sortBtnList.Count; i++)
        {
            sortBtnList[i].gameObject.SetActive(false);
        }
    }
    public void ToDeregistered()
    {
        inventoryState = InvenState.deregistered;

        // 
    }

    // Equipment버튼 클릭
    public void OnclickEquipBtn()
    {
        if (inventoryState == InvenState.EquipMent)
        {
            return;
        }
        else
        {
            Display_equipitems(0);
        }
    }
    // ETC버튼 클릭
    public void OnclickETCbtn()
    {
        if (inventoryState == InvenState.ETC)
        {
            return;
        }
        else
        {
            Display_ETCitems(0);
        }
    }


    // 오른쪽 버튼 클릭
    public void OnClickRightarrow()
    {
        // 다음윈도우 startIndex, endIndex
        int startindex = totalIconCount * (curDisplayNum + 1);
        int endindex = totalIconCount * (curDisplayNum + 2);

        // 현재 아이템 타입에 따라서
        switch (inventoryState)
        {
            case InvenState.deregistered:
                break;
            case InvenState.EquipMent:
                {
                    // 오른쪽 윈도우인덱스에 표시될 아이템이 있다면
                    if (UserDataManager.Inst.MountItemList.Count-1 >= startindex)
                    {
                        curDisplayNum++;
                        Display_equipitems(curDisplayNum);
                    }
                }
                break;
            case InvenState.ETC:
                {
                    // 오른쪽 윈도우인덱스에 표시될 아이템이 있다면
                    if (UserDataManager.Inst.EtcItemList.Count - 1 >= startindex)
                    {
                        curDisplayNum++;
                        Display_ETCitems(curDisplayNum);
                    }
                    
                }
                break;
        }
    }
    // 왼쪽 버튼 클릭
    public void OnClickLeftarrow()
    {
        if (inventoryState == InvenState.EquipMent)
        {
            curDisplayNum = Mathf.Max(curDisplayNum-1, 0);
            Display_equipitems(curDisplayNum);
        }
        else
        {
            curDisplayNum = Mathf.Max(curDisplayNum-1, 0);
            Display_ETCitems(curDisplayNum);
        }
    }
    // 셀링모드일때 버튼 클릭시
    public void OnClickSellingBtn()
    {
        if (modeType == MODE_TYPE.JUST_DISPLAY)
        {
            ToSellingMode();
        }
        else
        {
            ToJustDisplayMode();
        }
    }
    public void OnClickInSellingmode(UserMountItemData servantdata)
    {
        if (modeType != MODE_TYPE.SELLING)
        {
            return;
        }

        Debug.Log("servant Onclick! : " + servantdata.index);

        if (sellingMountItemList.Contains(servantdata))
        {
            sellingMountItemList.Remove(servantdata);
        }
        else
        {
            sellingMountItemList.Add(servantdata);
        }

        if (mountitemListChanged != null)
        {
            mountitemListChanged(servantdata);
        }
    }
    public void OnClickInSellingmode(UserEtcItemData monsterdata)
    {
        if (modeType != MODE_TYPE.SELLING)
        {
            return;
        }


        if (sellingETCItemList.Contains(monsterdata))
        {
            sellingETCItemList.Remove(monsterdata);
        }
        else
        {
            sellingETCItemList.Add(monsterdata);
        }


        if (etcitemListChanged != null)
        {
            etcitemListChanged(monsterdata);
        }
    }



    private void Awake()
    {
        for (int i = 0; i < slotparent.childCount; i++)
        {
            invenslotlist.Add(slotparent.GetChild(i).GetComponent<InventorySlotScript>());
        }
    }
    private void OnEnable()
    {
        ToJustDisplayMode();
        Display_equipitems(0);
    }
    private void OnDisable()
    {
        ToDeregistered();
    }
}
