using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollListManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public int SelectedMainIdx
    {
        get { return selected_main_idx; }
        set
        {
            selected_main_idx = value;
            UpdateScrollView();
        }
    }


    public int StartMainIdx
    {
        get { return start_main_idx; }
        set { start_main_idx = value; }
    }


    public float AnchorPositionY
    {
        get { return rectTrScrollLayer.anchoredPosition.y; }
        set
        {
            rectTrScrollLayer.anchoredPosition = new Vector2(rectTrScrollLayer.anchoredPosition.x, value);
            UpdateScrollView();
            DrawScrollView();
        }
    }


    public float ContentLayerOffset
    {
        get { return content_layer_offset; }
        set
        {
            content_layer_offset = value;
            UpdateScrollView();
            DrawScrollView();
        }
    }


    public RectTransform rectTrScrollLayer;
    public GameObject prefabUnit;


    private List<ScrollListUnit> unit_list;
    private int unit_height;
    private int start_main_idx;
    private int unit_num;
    private int total_item_num;
    public int selected_main_idx = -1; //data idx
    private int selected_unit_idx = -1; //prefab unit idx
    private MonoBehaviour unit_controller;
    private int[] data_order; // data_order[data_idx] = main_idx (main_idx = order(rank))
    private int[] item_order; // item_order[main_idx] = data_idx 
    private float view_height; // the height of scrollview


    private float content_layer_offset = 0;
    private float margin_layer_offset = 0;
    private float default_offset = 0;

    public ScrollRect scrollRect;
    private bool isScroll;
    private int selected_unit_tag = 0;

    private Vector2 moveScrollPos = Vector2.zero;


    private void Start()
    {
        Canvas canvas = transform.root.GetComponent<Canvas>();
        RectTransform rect_tr_canvas = canvas.GetComponent<RectTransform>();
        RectTransform rect_tr_scrollview = GetComponent<RectTransform>();
        Rect rect = rect_tr_scrollview.rect; // GetScreenRect(canvas);


        // 스크롤뷰 높이가 기기 화면 비율에 따라 달라지는 경우가 있기 때문에 sizeDelta 로 구할 수 없다.
        view_height = rect.height / Screen.height * rect_tr_canvas.sizeDelta.y;
    }


    public void setContractLayerOffset(int offset)
    {
        content_layer_offset = offset;
    }

    //set Select unit center focus : scroll Window size - unit height;
    private void setCenterFocusScroll()
    {
        scrollRect = GetComponent<ScrollRect>();
        margin_layer_offset = GetComponent<RectTransform>().rect.height - prefabUnit.GetComponent<RectTransform>().rect.height;
        default_offset = -(GetComponent<RectTransform>().rect.height / 2 + prefabUnit.GetComponent<RectTransform>().rect.height / 2);
    }

    
    public virtual void selectedUnit()
    {
        unit_list[selected_unit_tag % unit_num].Selected(false);
        int arr_idx = (int)((rectTrScrollLayer.anchoredPosition.y + (unit_height / 2)) / unit_height);
        if (arr_idx >= 0 && arr_idx < item_order.Length)
        {
            selected_main_idx = item_order[(int)((rectTrScrollLayer.anchoredPosition.y + (unit_height / 2)) / unit_height)];
            selected_unit_tag = arr_idx;
            unit_list[selected_unit_tag % unit_num].Selected(true);

            selected_unit_idx = (int)(rectTrScrollLayer.anchoredPosition.y / unit_height);
        }
        
    }

    public void updateSelectedUnitInfo()
    {
        if (LobbyManager.Inst.popupState == POPUP_STATE.Servant)
        {
            if (PartyInfoVC.Inst != null)
            {
                PartyInfoVC partyInfo = PartyInfoVC.Inst;
                if (partyInfo.selectedMenu == PartyInfoVC.menu_type.SERVANT || partyInfo.selectedMenu == PartyInfoVC.menu_type.MONSTER)
                {
                    if (partyInfo.currentScrollType == PartyInfoVC.scroll_type.SERVANT_INFO || partyInfo.currentScrollType == PartyInfoVC.scroll_type.MONSTER_INFO)
                    {
                        partyInfo.updateDetailInfo(selected_main_idx);
                    }
                }
            }
        }
        else if (LobbyManager.Inst.popupState == POPUP_STATE.Weapon)
        {
            InventoryVC.Inst.updateDetailInfo(selected_main_idx);
        }

    }

    bool move_scroll;
    public void OnBeginDrag(PointerEventData data)
    {
        //Debug.Log("OnBeginDrag");

        isScroll = true;
        move_scroll = true;
        //Debug.Log("Dragging started");
    }
    

    public void OnEndDrag(PointerEventData data)
    {
        //Debug.Log("Stopped dragging " + this.name + "!");
        isScroll = false;
    }

    
    //Snap effect
    private void setSnapping()
    {
        if (isScroll == false && moveScrollPos.y == 0)
        {
            if (Mathf.Abs(scrollRect.velocity.y) < unit_height / 2)
            {
                scrollRect.velocity = new Vector2(0, scrollRect.velocity.y * 0.5f);
            }
            
            if (Mathf.Abs(scrollRect.velocity.y) < 2.0f)
            {
                scrollSnap();
            }
            
        }
        
    }

    public virtual void scrollSnap()
    {
        
        if (unit_height * selected_unit_idx + unit_height / 2 < rectTrScrollLayer.anchoredPosition.y)
        {
            moveScrollPos = new Vector2(0, unit_height * (selected_unit_idx + 1));
        }
        else if (unit_height * selected_unit_idx - unit_height / 2 > rectTrScrollLayer.anchoredPosition.y)
        {
            moveScrollPos = new Vector2(0, unit_height * (selected_unit_idx - 1));
        }
        else
        {
            moveScrollPos = new Vector2(0, unit_height * (selected_unit_idx));
        }

        rectTrScrollLayer.anchoredPosition = Vector2.Lerp(new Vector2(0, rectTrScrollLayer.anchoredPosition.y), moveScrollPos, 0.2f);
        
        moveScrollPos = Vector2.zero;

        if (move_scroll)
        {
            updateSelectedUnitInfo();
            move_scroll = false;
        }
    }

    public int getFirstItemOrder()
    {
        return item_order[0];
    }

    public int getSelectedUnitIdx()
    {
        return selected_unit_idx;
    }

    public void Init(MonoBehaviour _unit_controller, int _unit_num, int _total_item_num, int[] _data_order = null)
    {
        unit_num = _unit_num;
        total_item_num = _total_item_num;
        unit_controller = _unit_controller;


        InitScrollView();
        SetItemOrder(_data_order);


        Canvas.ForceUpdateCanvases();
    }


    public void SetItemOrder(int[] _data_order)
    {
        total_item_num = _data_order.Length;
        if (_data_order != null)
        {
            data_order = _data_order;
            item_order = new int[total_item_num];
            for (int i = 0; i < total_item_num; i++)
                item_order[data_order[i]] = i;
        }
        else
        {
            data_order = new int[total_item_num];
            item_order = new int[total_item_num];
            for (int i = 0; i < total_item_num; i++)
            {
                data_order[i] = i;
                item_order[i] = i;
            }
        }
        
        UpdateScrollView();
        DrawScrollView();
        ScrollViewDidScroll();
    }


    // anchor_type 0: center, 1: top
    public void SetContentOffset(int data_idx, int anchor_type = 0)
    {
        int idx = data_order[data_idx];

        Vector2 pos = rectTrScrollLayer.anchoredPosition;
        if (anchor_type == 0)
        {
            float offset_y = view_height * 0.5f - unit_height * 0.5f;
            pos.y = unit_height * idx - offset_y; // 선택한 유닛이 가운데 위치하도록 ScrolllLayer 위치 조정 
        }
        else
        {
            float offset_y = unit_height * 0.5f;
            pos.y = unit_height * idx - offset_y;
        }

        if (pos.y < 0f)
            pos.y = 0f;
        else if (pos.y > rectTrScrollLayer.sizeDelta.y - view_height)
            pos.y = rectTrScrollLayer.sizeDelta.y - view_height;


        rectTrScrollLayer.anchoredPosition = pos;
        ScrollViewDidScroll();
    }


    private void InitScrollView()
    {
        setCenterFocusScroll();
        unit_list = new List<ScrollListUnit>();
        start_main_idx = 0;
        unit_height = (int)prefabUnit.GetComponent<RectTransform>().sizeDelta.y + 2;


        for (int i = 0; i < unit_num; i++)
        {
            GameObject obj = Instantiate(prefabUnit) as GameObject;
            obj.name = "-1";
            obj.transform.SetParent(rectTrScrollLayer.transform, false);
            ScrollListUnit unit = obj.GetComponent<ScrollListUnit>();
            unit.Init(unit_controller);
            unit_list.Add(unit);
        }
    }


    private void UpdateScrollViewUnit(int idx, int main_idx)
    {
        if (main_idx < total_item_num)
        {
            unit_list[idx].gameObject.SetActive(true);
            unit_list[idx].gameObject.name = main_idx.ToString();
            unit_list[idx].UpdateUnit(item_order[main_idx]);
            unit_list[idx].Selected(item_order[main_idx] == selected_main_idx);
        }
        else
            unit_list[idx].gameObject.SetActive(false);
    }


    public void UpdateScrollView()
    {
        for (int i = 0; i < unit_num; i++)
        {
            int main_idx = i + start_main_idx;
            int idx = main_idx % unit_num;

            UpdateScrollViewUnit(idx, main_idx);
        }

        rectTrScrollLayer.sizeDelta = new Vector2(rectTrScrollLayer.sizeDelta.x, (total_item_num) * unit_height + content_layer_offset + margin_layer_offset);
    }


    private void DrawScrollView()
    {
        for (int i = 0; i < unit_num; i++)
        {
            int tag = int.Parse(unit_list[i].gameObject.name);
            if (tag >= 0)
            {//Mathf.Abs(unit_list[i].RectTr.anchoredPosition.y)%20
                float x_offset = Mathf.Abs(-default_offset - unit_list[i].RectTr.position.y);
                float rotate_unit = -default_offset - unit_list[i].RectTr.position.y;

                x_offset /= 3.5f;
                rotate_unit /= 7f;
                unit_list[i].RectTr.anchoredPosition = new Vector2(x_offset, -tag * unit_height - margin_layer_offset / 2);
                //unit_list[i].RectTr.eulerAngles = new Vector3(-rotate_unit, Mathf.Abs(rotate_unit / 2), 0);
            }
            
        }
    }
    

    public void ScrollViewDidScroll()
    {
        float delta = (rectTrScrollLayer.anchoredPosition.y - content_layer_offset - margin_layer_offset) / unit_height;
        
        if (start_main_idx + unit_num < total_item_num && delta > start_main_idx + 1)
        {
            int loop_cnt = (int)delta - start_main_idx;

            for (int i = 0; i < loop_cnt; i++)
            {
                UpdateScrollViewUnit(start_main_idx % unit_num, start_main_idx + unit_num);
                start_main_idx++;
                if (start_main_idx == total_item_num - 1)
                    break;
            }

            DrawScrollView();
        }
        else if (start_main_idx > 0 && delta < start_main_idx)
        {
            int loop_cnt = start_main_idx - (int)delta;

            for (int i = 0; i < loop_cnt; i++)
            {
                start_main_idx--;
                UpdateScrollViewUnit(start_main_idx % unit_num, start_main_idx);
                if (start_main_idx == 0)
                    break;
            }

            DrawScrollView();
        }


        DrawScrollView();
        if (moveScrollSelectedUnit == false)
        {
            Debug.Log("Move Scroll");
            selectedUnit();
            setSnapping();// if (scrollRect.velocity.y > 0f)
                
        }
        SaveScrollOffset();
    }

    private bool moveScrollSelectedUnit = false;
    Vector2 movePos = Vector2.zero;
    
    public void MoveScrollSelectedUnit(Vector2 unit_pos, int main_idx)
    {
        moveScrollSelectedUnit = true;
        movePos = unit_pos;
    }

    void Update()
    {
        if (moveScrollSelectedUnit)
        {
            rectTrScrollLayer.anchoredPosition = Vector2.Lerp(new Vector2(0, rectTrScrollLayer.anchoredPosition.y), new Vector2(0, default_offset + unit_height - movePos.y), 0.2f);

            if (rectTrScrollLayer.anchoredPosition.y > (default_offset + unit_height - movePos.y)- unit_height/2 && rectTrScrollLayer.anchoredPosition.y < (default_offset + unit_height - movePos.y)+ unit_height / 2) {
                selectedUnit();
                updateSelectedUnitInfo();
                moveScrollSelectedUnit = false;
            }   
        }
    }

    public virtual void SaveScrollOffset()
    {//Saved Scroll Offset
        //if (g.long_term_contract_list_vc != null)
        //    g.gl.SaveContractBidScrollOffset = g.long_term_contract_list_vc.scrollView.rectTrScrollLayer.anchoredPosition.y;
    }

}