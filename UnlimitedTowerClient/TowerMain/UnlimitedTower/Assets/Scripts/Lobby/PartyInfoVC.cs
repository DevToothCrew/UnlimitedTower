using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyInfoVC : MonoSingleton<PartyInfoVC>
{
    public GameObject framePartyInfo;

    //SubView
    public GameObject SubViewEquipment;
    public GameObject SubViewDeconstruction;
    public GameObject SubViewSkill;
    public GameObject SubViewUpgradeMonster;


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

    public Text textPAtk;
    public Text textPDef;
    public Text textPCri;
    public Text textPCriPer;
    public Text textMAtk;
    public Text textMDef;
    public Text textMCri;
    public Text textMCriPer;
    public Text textSpeed;

    public Button[] buttonEquipment = new Button[6];


    //public Text textA

    //UI Set Data
    public enum menu_type
    {
        SERVANT = 0,
        MONSTER,
        FORMATION
    }
    public menu_type selectedMenu = menu_type.SERVANT;

    enum sort_type
    {
        GRADE = 0,
        LEVEL,
        POWER,
        OPTAIN
    }
    sort_type sortType = 0;


    public List<UserServantData> ServantList = new List<UserServantData>();
    public List<UserMonsterData> MonsterList = new List<UserMonsterData>();

    [HideInInspector]
    public int selected_tab = 0;
    private int selected_party_arr_idx;

    public enum scroll_type
    {
        SERVANT_INFO = 0,       //서번트 정보(기본)
        MONSTER_INFO,           //몬스터 정보
        EQUIPMENT_WEAPON,       //무기 설정
        EQUIPMENT_ARMOR,        //방어구 설정
        EQUIPMENT_ACC,          //장식품 설정
        SKILL,                  //스킬
        DECONSTRUCTION_SERVANT, //서번트 분해
        DECONSTRUCTION_MONSTER,  //몬스터 분해
        FORMATION                //포메이션
    }
    public scroll_type currentScrollType = 0;


    //임시 데이터 생성
    void setData()
    {
        ServantList.Clear();
        MonsterList.Clear();

        //for (int i = 0; i < 20; i++)
        //{
        //    UserDataManager.Inst.GetServantList();
        //    ServantList.Add(member);
        //}

        ServantList = UserDataManager.Inst.GetServantList();
        MonsterList = UserDataManager.Inst.GetMonsterList();

    }

    void Start() {
        initScrollList();
        OnClickMenuButton(0);
    }

    public menu_type getMenuType()
    {
        return selectedMenu;
    }

    //화면 전체 Update (메뉴버튼, 상세정보창, 스크롤 등)
    void updateAllView()
    {
        for (int i = 0; i < 3; i++)
        {
            buttonMenu[i].image.sprite = buttonMenu[i].spriteState.disabledSprite;
            buttonMenu[i].GetComponentInChildren<Text>().color = Color.white;
        }

        buttonMenu[(int)selectedMenu].image.sprite = buttonMenu[(int)selectedMenu].spriteState.pressedSprite;
        buttonMenu[(int)selectedMenu].GetComponentInChildren<Text>().color = Color.black;

        if (selectedMenu == menu_type.SERVANT)
        {
            textOwned.text = string.Format("{0}", ServantList.Count);
            textTotal.text = string.Format("/ {0}", 100);
            scrollList.SetItemOrder(getOrder());
            scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
        }
        else if (selectedMenu == menu_type.MONSTER)
        {
            textOwned.text = string.Format("{0}", MonsterList.Count);
            textTotal.text = string.Format("/ {0}", 100);
            scrollList.SetItemOrder(getOrder());
            scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
        }
        else
        {

        }

    }

    //좌측 메뉴버튼 클릭
    public void OnClickMenuButton(int tag)
    {
        for (int i = 0; i < buttonEquipment.Length; i++)
            buttonEquipment[i].gameObject.SetActive(false);

        if (tag == (int)menu_type.SERVANT)
        {
            selectedMenu = menu_type.SERVANT;
            currentScrollType = scroll_type.SERVANT_INFO;
            for (int i = 0; i < 3; i++)
                buttonEquipment[i].gameObject.SetActive(true);

            framePartyInfo.SetActive(true);
            frameFormation.SetActive(false);
            scrollList.gameObject.SetActive(true);
        }
        else if (tag == (int)menu_type.MONSTER)
        {
            selectedMenu = menu_type.MONSTER;
            currentScrollType = scroll_type.MONSTER_INFO;

            framePartyInfo.SetActive(true);
            frameFormation.SetActive(false);
            scrollList.gameObject.SetActive(true);
        }
        else
        {
            selectedMenu = menu_type.FORMATION;
            currentScrollType = scroll_type.FORMATION;

            framePartyInfo.SetActive(false);
            frameFormation.SetActive(true);
            updateFormation();
            scrollList.gameObject.SetActive(false);
        }
        updateAllView();

    }

    //스크롤 생성
    void initScrollList()
    {

        ServantList.Clear();

        setData();

        scrollList.Init(this, 20, ServantList.Count, getOrder());
    }


    //스크롤 정렬
    private int[] getOrder()
    {
        int[] data_order;
        int total_list_num = 0;

        if (selectedMenu == menu_type.SERVANT)
        {
            total_list_num = ServantList.Count;
        }
        else if (selectedMenu == menu_type.MONSTER)
        {
            total_list_num = MonsterList.Count;
        }

        data_order = new int[total_list_num];

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
                        if (selectedMenu == menu_type.SERVANT)
                        {
                            if (ServantList[i].level * 100 + ServantList[i].exp < ServantList[j].level * 100 + +ServantList[j].exp)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (selectedMenu == menu_type.MONSTER)
                        {
                            if (MonsterList[i].level * 100 + MonsterList[i].exp < MonsterList[j].level * 100 + +MonsterList[j].exp)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }


        return data_order;
    }

    //상세정보창 Update
    public void updateDetailInfo(int _selected_unit_idx)
    {
        int selected_unit_idx = _selected_unit_idx;

        if (getMenuType() == menu_type.SERVANT)
        {
            DBServantData dBServantData = CSVData.Inst.GetServantData(ServantList[selected_unit_idx].id);
            if(dBServantData == null)
            {
                Debug.Log("Invalid Servant ID : " + ServantList[selected_unit_idx].id);
            }

            textLevel.text = string.Format("{0}", ServantList[selected_unit_idx].level);
            textCharacterName.text = string.Format("{0}", dBServantData.name);

            textStr.text = string.Format("{0}", ServantList[selected_unit_idx].status.basicStr);
            textDex.text = string.Format("{0}", ServantList[selected_unit_idx].status.basicDex);
            textInt.text = string.Format("{0}", ServantList[selected_unit_idx].status.basicInt);
        }
        else if (getMenuType() == menu_type.MONSTER)
        {
            DBMonsterData dBMonsterData = CSVData.Inst.GetMonsterData(MonsterList[selected_unit_idx].id);
            if(dBMonsterData == null)
            {
                Debug.Log("Invalid Monster ID : " + MonsterList[selected_unit_idx].id);
            }

            textLevel.text = string.Format("{0}", MonsterList[selected_unit_idx].level);
            textCharacterName.text = string.Format("{0}", dBMonsterData.name);

            textStr.text = string.Format("{0}", MonsterList[selected_unit_idx].status.basicStr);
            textDex.text = string.Format("{0}", MonsterList[selected_unit_idx].status.basicDex);
            textInt.text = string.Format("{0}", MonsterList[selected_unit_idx].status.basicInt);
        }
           
    }
    
	
    public void OnClickDeconstruction()
    {
        SubViewDeconstruction.SetActive(true);

        if (selectedMenu == menu_type.SERVANT)
            currentScrollType = scroll_type.DECONSTRUCTION_SERVANT;
        else
            currentScrollType = scroll_type.DECONSTRUCTION_MONSTER;

    }

    //장비 설정 버튼
    public void OnClickEquipment(int btn_tag)
    {
        SubViewEquipment.SetActive(true);
        
        switch (btn_tag)
        {
            case 0:
                currentScrollType = scroll_type.EQUIPMENT_WEAPON;
                break;
            case 1:
                currentScrollType = scroll_type.EQUIPMENT_ARMOR;
                break;
            case 2:
                currentScrollType = scroll_type.EQUIPMENT_ACC;
                break;
            default:
                break;
        }

    }

    //현재 화면에 따른 스크롤 UI 재설정 
    public void resetScroll()
    {

    }




    //////////////////////*  Formation 관련 UI  *////////////////////////
    public GameObject frameFormation;
    public Button[] buttonMonsterFormation = new Button[5];
    public Image[] imageMonsterFormation = new Image[5];
    public Button[] buttonServantFormation = new Button[5];
    public Image[] imageServantFormation = new Image[5];

    public Sprite[] spriteSlot = new Sprite[2];

    public Button buttonSaveFormation;

    public void updateFormation()
    {
        UserDataManager u_data = UserDataManager.Inst;
        for (int i = 0; i < 5; i++)
        {
            UserServantData servant = u_data.GetServantInfo(u_data.partyInfo.formationDataDic[i].index);
            
            if (servant.grade > 0)
            {
                buttonServantFormation[i].image.sprite = CSVData.Inst.getSpriteGrade((GRADE_TYPE)servant.grade);
                imageServantFormation[i].enabled = true;
                imageServantFormation[i].sprite = CSVData.Inst.GetServantData(servant.id).servantIcon;
            }
            else
            {
                buttonServantFormation[i].image.sprite = spriteSlot[0];
                buttonServantFormation[i].GetComponentInChildren<Image>().enabled = false;
            }
        }

        for (int i = 0; i < 5; i++)
        {
            UserMonsterData monster = u_data.GetMonsterInfo(u_data.partyInfo.formationDataDic[i + 5].index);

            if (monster.grade > 0)
            {
                buttonMonsterFormation[i].image.sprite = CSVData.Inst.getSpriteGrade((GRADE_TYPE)monster.grade);
                imageMonsterFormation[i].enabled = true;
                imageMonsterFormation[i].sprite = CSVData.Inst.GetMonsterData(monster.id).monsterIcon;
            }
            else
            {
                buttonMonsterFormation[i].image.sprite = spriteSlot[0];
                buttonMonsterFormation[i].GetComponentInChildren<Image>().enabled = false;
            }
        }

        {
            
        }
    }

    public int updateGrade(int grade)
    {
        int sprite_idx = 0;
        if (grade == (int)GRADE_TYPE.COMMON)
        {
            sprite_idx = 2;
        }
        else if (grade == (int)GRADE_TYPE.UNCOMMON)
        {
            sprite_idx = 3;
        }
        else if (grade == (int)GRADE_TYPE.RARE)
        {
            sprite_idx = 4;
        }
        else if (grade == (int)GRADE_TYPE.UNIQUE)
        {
            sprite_idx = 5;
        }
        else if (grade == (int)GRADE_TYPE.LEGENDARY)
        {
            sprite_idx = 6;
        }

        return sprite_idx;
    }

    public void OnClickMonsterSlot(int btn_tag)
    {

    }

    public void OnClickServantSlot(int btn_tag)
    {

    }

    public void OnClickSaveFormation()
    {

    }



}
