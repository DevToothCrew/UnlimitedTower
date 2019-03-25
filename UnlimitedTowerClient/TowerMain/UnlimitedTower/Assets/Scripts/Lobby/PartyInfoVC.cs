using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyInfoVC : MonoBehaviour {
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

    //UI Set Data
    int selectedMenu = 0;
    int sortType = 0;

    public struct party_info
    {
        public int idx;    //Db index
        public int type;   //서번트 or 몬스터
        public int model;  //서번트일 경우: 직업, 몬스터일 경우: 종류
        public int tier;   //티어
        public int level;  //레벨
        public int exp;    //경험치
        public int str;    //힘
        public int dex;    //민첩
        public int mInt;   //지력
    }
    public List<List<party_info>> PartyInfoList = new List<List<party_info>>();

    [HideInInspector]
    public int selected_tab = 0;
    private int selected_party_arr_idx;

    //임시 데이터 생성
    void setData()
    {

        if (PartyInfoList.Count > 0)
        {
            for (int i = 0; i < 2; i++)
            {
                PartyInfoList[i].Clear();
            }
        }

        PartyInfoList.Clear();

        for (int i = 0; i < 2; i++)
        {
            PartyInfoList.Add(new List<party_info>());
        }


        for (int type = 0; type < 2; type++)
        {
            for (int i = 0; i < 20; i++)
            {
                party_info member = new party_info();
                member.idx = type * 20 + i;
                member.type = type;
                member.model = Random.Range(0, 5);
                member.tier = Random.Range(0, 5);
                member.level = Random.Range(0, 99);
                member.exp = Random.Range(0, 99);
                member.str = member.level * 2;
                member.dex = member.level * 1;
                member.mInt = member.level / 2;
           

                PartyInfoList[type].Add(member);
            }
        }
        
    }

    void Start () {
        initScrollList();
        //updateView();
        

    }

    void updateView()
    {
        textOwned.text = string.Format("{0}", PartyInfoList[selectedMenu].Count);
        textTotal.text = string.Format("{0}", 100);
    }

    void initScrollList()
    {
        
        if (PartyInfoList.Count > 0)
        {
            for (int i = 0; i < 2; i++)
            {
                PartyInfoList[i].Clear();
            }
        }
        

        PartyInfoList.Clear();

        for (int i = 0; i < 2; i++)
        {
            PartyInfoList.Add(new List<party_info>());
        }

        setData();
        
        scrollList.Init(this, 20, PartyInfoList[selectedMenu].Count, getOrder());
    }

    private int[] getOrder()
    {
        int[] data_order;
        int total_list_num;

        data_order = new int[PartyInfoList[selectedMenu].Count];
        total_list_num = PartyInfoList[selectedMenu].Count;

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
                        if (PartyInfoList[selectedMenu][i].level*100 + PartyInfoList[selectedMenu][i].exp < PartyInfoList[selectedMenu][j].level*100 + +PartyInfoList[selectedMenu][j].exp)
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
            case 1:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (PartyInfoList[selectedMenu][i].tier < PartyInfoList[selectedMenu][j].tier)
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
	
}
