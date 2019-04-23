using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortManager : MonoSingleton<SortManager> {
    public Button[] buttonSort = new Button[5];
    public Text[] textSort = new Text[5];

    //스크롤 정렬 UI
    public GameObject FrameSort;
    bool isShowSort = false;
    private SORT_TYPE sortType;

    public SORT_TYPE GetSortType()
    {
        return sortType;
    }

    //스크롤 종류에 따른 Sort버튼 Text 업데이트
    private void updateViewText()
    {
        if (PartyInfoVC.checkInst() && !SubViewEquipment.checkInst())
        {
            PartyInfoVC party_info = PartyInfoVC.Inst;
            if (party_info.currentScrollType == PartyInfoVC.scroll_type.SERVANT_INFO)
            {
                textSort[0].text = "POWER";
                textSort[1].text = "GRADE";
                textSort[2].text = "LEVEL";
                textSort[3].text = "GOT TIME";
                textSort[4].text = "JOB";
            }
            else
            {
                textSort[0].text = "POWER";
                textSort[1].text = "GRADE";
                textSort[2].text = "LEVEL";
                textSort[3].text = "GOT TIME";
                textSort[4].text = "UPGRADE";
            }
            
        }
        else if (InventoryVC.checkInst() || SubViewEquipment.checkInst())
        {
            textSort[0].text = "POWER";
            textSort[1].text = "GRADE";
            textSort[2].text = "TIER";
            textSort[3].text = "GOT TIME";
            textSort[4].text = "UPGRADE";
        }
    }

    public void updateSortView()
    {
        for (int i = 0; i < textSort.Length; i++)
        {
            textSort[i].color = Color.white;
            //buttonSort[i].enabled = true;
        }

        textSort[(int)sortType].color = Color.cyan;
        //buttonSort[(int)sortType].enabled = false;
    }

    public void OnClickSortType(int btn_idx)
    {
        sortType = (SORT_TYPE)btn_idx;
        updateSortView();

        if (PartyInfoVC.checkInst())
        {
            if (SubViewEquipment.checkInst())
            {
                Debug.Log("Reset Equip List");
                SubViewEquipment.Inst.ResetScrollListBySortType(sortType);
            }
            else if (SubViewDeconstruction.checkInst())
            {
                Debug.Log("Reset deconstruct List");
                SubViewDeconstruction.Inst.ResetScrollListBySortType(sortType);
            }
            else if(SubViewUpgrade.checkInst())
            {
                Debug.Log("Reset SubViewUpgrade List");
                SubViewUpgrade.Inst.ResetScrollListBySortType(sortType);
            }
            else {
                Debug.Log("Reset Partyinfo List");
                PartyInfoVC.Inst.ResetScrollListBySortType(sortType);
            }   
        }
        else if (InventoryVC.checkInst())
        {
            if (SubViewDeconstruction.checkInst())
            {
                Debug.Log("Reset deconstruct List");
                SubViewDeconstruction.Inst.ResetScrollListBySortType(sortType);
            }
            else if (SubViewUpgrade.checkInst())
            {
                Debug.Log("Reset SubViewUpgrade List");
                SubViewUpgrade.Inst.ResetScrollListBySortType(sortType);
            }
            else
            {
                Debug.Log("Reset Inven List");
                InventoryVC.Inst.ResetScrollListBySortType(sortType);
            }
            
        }
    }

    public void ShowFrameSort()
    {
        isShowSort = !isShowSort;

        if (isShowSort)
        {
            updateViewText();
            Animation anim = FrameSort.GetComponent<Animation>();
            foreach (AnimationState state in anim)
            {
                state.speed = -1f;
                state.time = state.length;
            }
            anim.Play();
            //FrameSort.SetActive(true);
        }
        else
        {
            Animation anim = FrameSort.GetComponent<Animation>();
            foreach (AnimationState state in anim)
            {
                state.speed = 1f;
                //state.time = state.length;
            }
            anim.Play();
            //FrameSort.SetActive(false);
        }


    }
}
