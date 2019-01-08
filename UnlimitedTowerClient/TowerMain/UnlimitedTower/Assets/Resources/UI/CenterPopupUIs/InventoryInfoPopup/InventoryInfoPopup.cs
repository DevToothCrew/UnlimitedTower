using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryInfoPopup : MonoBehaviour {

    //
    public Transform slotparent;
    List<InventorySlotScript> invenslotlist;
    //
    public List<GameObject> sortBtnList;
    
    public enum InvenState
    {
        deregistered,
        EquipMent,
        ETC
    }
    public InvenState inventoryState;

    
    public void to_equipment()
    {
        if (inventoryState == InvenState.EquipMent)
        {
            return;
        }
        inventoryState = InvenState.EquipMent;

        // 창 초기화
        for (int i = 0; i < invenslotlist.Count; i++)
        {
            invenslotlist[i].to_locked();
        }

        // 서번트 등록
        int index = 0;
        foreach (var mountitem in UserDataManager.Inst.mountitemDic)
        {
            invenslotlist[index].register(mountitem.Value);
            index++;
        }



        // 버튼들 생기게하기
        for (int i = 0; i < sortBtnList.Count; i++)
        {
            sortBtnList[i].gameObject.SetActive(true);
        }
    }
    public void to_ETC()
    {
        if (inventoryState == InvenState.ETC)
        {
            return;
        }
        inventoryState = InvenState.ETC;

        // 창 초기화
        for (int i = 0; i < invenslotlist.Count; i++)
        {
            invenslotlist[i].to_locked();
        }

        // 서번트 등록
        int index = 0;
        foreach (var etcitem in UserDataManager.Inst.etcitemDic)
        {
            invenslotlist[index].register(etcitem.Value);
            index++;
        }


        // 버튼들 없애기
        for (int i = 0; i < sortBtnList.Count; i++)
        {
            sortBtnList[i].gameObject.SetActive(false);
        }
    }
    public void to_deregistered()
    {
        inventoryState = InvenState.deregistered;

        // 
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
        to_equipment();
    }
    private void OnDisable()
    {
        to_deregistered();
    }
}
