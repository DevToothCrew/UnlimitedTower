using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyInfoVC : MonoSingleton<PartyInfoVC>
{
    //Menu Buttons UI
    public Text textButtonBack;
    public Button[] buttonMenu = new Button[3];

    //ScrollView UI
    public Button buttonSort;
    public Text textOwned;
    public Text textTotal;

    public GameObject frameSortType;
    public Button[] buttonSortType = new Button[4];

    public ScrollListManager scrollList;

    //Detail Info UI
    public Text textLevel;
    public RectTransform rectExp;
    public Text textCharacterName;

    public Text textStr;
    public Text textDex;
    public Text textInt;

    //public Text textA

    //UI Set Data
    int selectedMenu = 0;
    int sortType = 0;

    public List<UserServantData> PartyInfoList = new List<UserServantData>();

    [HideInInspector]
    public int selected_tab = 0;
    private int selected_party_arr_idx;

    //임시 데이터 생성
    void setData()
    {
        PartyInfoList.Clear();

        //for (int i = 0; i < 20; i++)
        //{
        //    UserDataManager.Inst.GetServantList();
        //    PartyInfoList.Add(member);
        //}

        PartyInfoList = UserDataManager.Inst.GetServantList();

    }

    void Start () {
        initScrollList();
        //updateView();
        

    }

    void updateView()
    {
        textOwned.text = string.Format("{0}", PartyInfoList.Count);
        textTotal.text = string.Format("{0}", 100);
    }

    void initScrollList()
    {
      
        PartyInfoList.Clear();
        
        setData();
        
        scrollList.Init(this, 20, PartyInfoList.Count, getOrder());
    }

    private int[] getOrder()
    {
        int[] data_order;
        int total_list_num;

        data_order = new int[PartyInfoList.Count];
        total_list_num = PartyInfoList.Count;

        for (int i=0; i<data_order.Length; i++)
        {
            data_order[i] = 0;
        }

        switch (sortType)
        {
            case 0:
                for (int i=0; i<total_list_num-1; i++)
                {
                    for (int j = i+1; j < total_list_num; j++)
                    {
                        if (PartyInfoList[i].level*100 + PartyInfoList[i].exp < PartyInfoList[j].level*100 + +PartyInfoList[j].exp)
                        {
                            data_order[i]++;
                        }
                        else
                        {
                            data_order[j]++;
                        }
                    }
                }
                break;
            default:
                break;
        }


        return data_order;
    }

    public void updateDetailInfo()
    {
        int selected_unit_idx = scrollList.getSelectedUnitIdx();

        textLevel.text = string.Format("{0}", PartyInfoList[selected_unit_idx].level);
        //xtLevel.text = string.Format("{0}", PartyInfoList[selected_unit_idx].level);
        textCharacterName.text = string.Format("{0}", PartyInfoList[selected_unit_idx].name);

        textStr.text = string.Format("{0}", PartyInfoList[selected_unit_idx].status.basicStr + PartyInfoList[selected_unit_idx].status.plusStr);
        textDex.text = string.Format("{0}", PartyInfoList[selected_unit_idx].status.basicDex + PartyInfoList[selected_unit_idx].status.plusDex);
        textInt.text = string.Format("{0}", PartyInfoList[selected_unit_idx].status.basicInt + PartyInfoList[selected_unit_idx].status.plusInt);
    }

    public void OnButtonBack()
    {


    }


	
}
