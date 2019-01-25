using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInfoPopup : MonoBehaviour
{

    public enum ITEMINVENTORY_ITEM_TYPE
    {
        deregistered,
        EquipMent,
        ETC
    }
    public enum ITEMINVENTORY_MODE_TYPE
    {
        JUST_DISPLAY,
        SELLING
    }
    public enum ITEMINVENTORY_SORT_TYPE
    {
        GRADE,
        TIER,
        UPGRADE,
        OBTAIN
    }

    // 아이콘 개수
    int totalIconCount
    {
        get
        {
            return invenslotlist.Count;
        }
    }



    /* INSPECTOR */

    // 아이콘들
    public Transform slotparent;
    List<InventorySlotScript> invenslotlist = new List<InventorySlotScript>();
    // 정렬버튼들 
    public List<GameObject> sortBtnList;
    // 페이지번호Text
    public Text pageText;
    // 하단셀링바
    public GameObject SellingModeObj;
    public GameObject DisplayModeObj;

    [Space(10)]
    [Header("Window State Variables")]
    //
    [SerializeField] ITEMINVENTORY_ITEM_TYPE inventoryState;
    //
    [SerializeField] int _curDisplayNum = 0;
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
                case ITEMINVENTORY_ITEM_TYPE.EquipMent:
                    pageText.text = (_curDisplayNum + 1) + "/" + (UserDataManager.Inst.MountItemList.Count / slotparent.childCount + 1) + "P";
                    break;
                case ITEMINVENTORY_ITEM_TYPE.ETC:
                    pageText.text = (_curDisplayNum + 1) + "/" + (UserDataManager.Inst.EtcItemList.Count / slotparent.childCount + 1) + "P";
                    break;
            }
        }
    }
    //
    public ITEMINVENTORY_MODE_TYPE modeType;
    public List<UserMountItemData> sellingMountItemList;
    public List<UserEtcItemData> sellingETCItemList;
    public static System.Action<UserMountItemData> mountitemListChanged;
    public static System.Action<UserEtcItemData> etcitemListChanged;
    public static System.Action modeChanged;
    // SORT TYPE
    public ITEMINVENTORY_SORT_TYPE sortType;

    // 초기 상태
    const ITEMINVENTORY_ITEM_TYPE DEFAULT_ITEM_TYPE = ITEMINVENTORY_ITEM_TYPE.EquipMent;
    const ITEMINVENTORY_SORT_TYPE DEFAULT_SORT_TYPE = ITEMINVENTORY_SORT_TYPE.GRADE;
    const int DEFAULT_PAGE_NUM = 0;
    const ITEMINVENTORY_MODE_TYPE DEFAULT_MODE = ITEMINVENTORY_MODE_TYPE.JUST_DISPLAY;










    /* FSM 상태변경 */

    // DISPLAY
    public void DisplayItems(ITEMINVENTORY_ITEM_TYPE itemType, ITEMINVENTORY_SORT_TYPE sortType, int pageNum)
    {
        this.inventoryState = itemType;
        this.curDisplayNum = pageNum;
        this.sortType = sortType;

        // 창 초기화
        for (int i = 0; i < invenslotlist.Count; i++)
        {
            invenslotlist[i].ToLocked();
        }





        // 아이콘 디스플레이 / 정렬버튼 확인
        int startindex = curDisplayNum * totalIconCount;
        int endindex = (curDisplayNum + 1) * totalIconCount;
        switch (inventoryState)
        {
            // 정렬 후 디스플레이
            case ITEMINVENTORY_ITEM_TYPE.EquipMent:
                {
                    // 정렬
                    switch (sortType)
                    {
                        case ITEMINVENTORY_SORT_TYPE.GRADE:
                            UserDataManager.Inst.MountItemList.Sort(
                                (a, b) =>
                                {
                                    // grade가 같다면 -> index순으로
                                    if (a.gradeNum == b.gradeNum)
                                    {
                                        return (a.index < b.index) ? 1 : -1;
                                    }
                                    else
                                    {
                                        return a.gradeNum < b.gradeNum ? 1 : -1;
                                    }
                                });
                            break;
                        case ITEMINVENTORY_SORT_TYPE.TIER:
                            UserDataManager.Inst.MountItemList.Sort(
                                (a, b) =>
                                {
                                    // grade가 같다면 -> index순으로
                                    if (a.tierNum == b.tierNum)
                                    {
                                        return (a.index < b.index) ? 1 : -1;
                                    }
                                    else
                                    {
                                        return a.tierNum < b.tierNum ? 1 : -1;
                                    }
                                });
                            break;
                        case ITEMINVENTORY_SORT_TYPE.UPGRADE:
                            UserDataManager.Inst.MountItemList.Sort(
                                (a, b) =>
                                {
                                    // grade가 같다면 -> index순으로
                                    if (a.upgradeCount == b.upgradeCount)
                                    {
                                        return (a.index < b.index) ? 1 : -1;
                                    }
                                    else
                                    {
                                        return a.upgradeCount < b.upgradeCount ? 1 : -1;
                                    }
                                });
                            break;
                        case ITEMINVENTORY_SORT_TYPE.OBTAIN:
                            UserDataManager.Inst.MountItemList.Sort(
                                (a, b) =>
                                {
                                    return (a.index < b.index) ? 1 : -1;
                                });
                            break;
                    }

                    // 디스플레이
                    for (int itemIndex = startindex; itemIndex < endindex && itemIndex < UserDataManager.Inst.MountItemList.Count; itemIndex++)
                    {
                        int iconIndex = itemIndex - startindex;
                        InventorySlotScript slot = invenslotlist[iconIndex];
                        slot.Register(UserDataManager.Inst.MountItemList[itemIndex]);
                    }

                    // 정렬버튼 보이기
                    for (int i = 0; i < sortBtnList.Count; i++)
                    {
                        sortBtnList[i].gameObject.SetActive(true);
                    }
                }
                break;

            // 디스플레이 
            case ITEMINVENTORY_ITEM_TYPE.ETC:
                {
                    for (int itemIndex = startindex; itemIndex < endindex && itemIndex < UserDataManager.Inst.EtcItemList.Count; itemIndex++)
                    {
                        int iconIndex = itemIndex - curDisplayNum * totalIconCount;
                        InventorySlotScript slot = invenslotlist[iconIndex];
                        slot.Register(UserDataManager.Inst.EtcItemList[itemIndex]);
                    }

                    // 정렬버튼 없애기
                    for (int i = 0; i < sortBtnList.Count; i++)
                    {
                        sortBtnList[i].gameObject.SetActive(false);
                    }
                }
                break;
        }
    }
    public void ToDeregistered()
    {
        inventoryState = ITEMINVENTORY_ITEM_TYPE.deregistered;
    }

    // MODE 변경
    public void ToJustDisplayMode()
    {
        modeType = ITEMINVENTORY_MODE_TYPE.JUST_DISPLAY;

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
        modeType = ITEMINVENTORY_MODE_TYPE.SELLING;

        if (modeChanged != null)
        {
            modeChanged();
        }

        // 셀링바 업데이트
        SellingModeObj.SetActive(true);
        DisplayModeObj.SetActive(false);
    }




    /* 버튼 온클릭 */
    // SORT 버튼
    public void OnClickGradesort()
    {
        DisplayItems(inventoryState, ITEMINVENTORY_SORT_TYPE.GRADE, curDisplayNum);
    }
    public void OnClickTiersort()
    {
        DisplayItems(inventoryState, ITEMINVENTORY_SORT_TYPE.TIER, curDisplayNum);
    }
    public void OnClickUpgradesort()
    {
        DisplayItems(inventoryState, ITEMINVENTORY_SORT_TYPE.UPGRADE, curDisplayNum);
    }
    public void OnClickObtainsort()
    {
        DisplayItems(inventoryState, ITEMINVENTORY_SORT_TYPE.OBTAIN, curDisplayNum);
    }

    // Equipment버튼 클릭
    public void OnclickEquipBtn()
    {
        // 현재 장비디스플레이중이면 -> RETURN
        if (inventoryState == ITEMINVENTORY_ITEM_TYPE.EquipMent)
        {
            return;
        }



        DisplayItems(ITEMINVENTORY_ITEM_TYPE.EquipMent, sortType, 0);
    }
    // ETC버튼 클릭
    public void OnclickETCbtn()
    {
        // 현재 ETC디스플레이중이면 -> RETURN
        if (inventoryState == ITEMINVENTORY_ITEM_TYPE.ETC)
        {
            return;
        }


        DisplayItems(ITEMINVENTORY_ITEM_TYPE.ETC, sortType, 0);
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
            case ITEMINVENTORY_ITEM_TYPE.deregistered:
                break;
            case ITEMINVENTORY_ITEM_TYPE.EquipMent:
                {
                    // 오른쪽 윈도우인덱스에 표시될 아이템이 있다면
                    if (UserDataManager.Inst.MountItemList.Count - 1 >= startindex)
                    {
                        DisplayItems(ITEMINVENTORY_ITEM_TYPE.EquipMent, sortType, curDisplayNum + 1);
                    }
                }
                break;
            case ITEMINVENTORY_ITEM_TYPE.ETC:
                {
                    // 오른쪽 윈도우인덱스에 표시될 아이템이 있다면
                    if (UserDataManager.Inst.EtcItemList.Count - 1 >= startindex)
                    {
                        DisplayItems(ITEMINVENTORY_ITEM_TYPE.ETC, sortType, curDisplayNum + 1);
                    }

                }
                break;
        }
    }
    // 왼쪽 버튼 클릭
    public void OnClickLeftarrow()
    {
        if (inventoryState == ITEMINVENTORY_ITEM_TYPE.EquipMent)
        {
            curDisplayNum = Mathf.Max(curDisplayNum - 1, 0);
            DisplayItems(ITEMINVENTORY_ITEM_TYPE.EquipMent, sortType, Mathf.Max(curDisplayNum - 1, 0));
        }
        else
        {
            curDisplayNum = Mathf.Max(curDisplayNum - 1, 0);
            DisplayItems(ITEMINVENTORY_ITEM_TYPE.ETC, sortType, Mathf.Max(curDisplayNum - 1, 0));
        }
    }
    // 셀링모드일때 버튼 클릭시
    public void OnClickSellingBtn()
    {
        if (modeType == ITEMINVENTORY_MODE_TYPE.JUST_DISPLAY)
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
        if (modeType != ITEMINVENTORY_MODE_TYPE.SELLING)
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
        if (modeType != ITEMINVENTORY_MODE_TYPE.SELLING)
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
        // Default 상태로 켠다.
        DisplayItems(InventoryInfoPopup.DEFAULT_ITEM_TYPE, sortType, InventoryInfoPopup.DEFAULT_PAGE_NUM);
    }
    private void OnDisable()
    {
        ToDeregistered();
    }
}

