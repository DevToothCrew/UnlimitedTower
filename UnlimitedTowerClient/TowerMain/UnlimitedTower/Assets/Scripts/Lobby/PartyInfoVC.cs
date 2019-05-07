using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyInfoVC : MonoSingleton<PartyInfoVC>
{
    public GameObject framePartyInfo;

    //Info
    public GameObject FrameServantInfo;
    public GameObject FrameMonsterInfo;

    //SubView
    public GameObject FrameSubView;

    //Menu Buttons UI
    public Text textButtonBack;
    public Button[] buttonMenu = new Button[3];

    //ScrollView UI
    public Text textOwned;
    public Text textTotal;

    public GameObject frameScroll;
    public ScrollListManager scrollList;
    private SORT_TYPE sort_type;

    //UI Set Data
    public enum menu_type
    {
        SERVANT = 0,
        MONSTER,
        FORMATION
    }
    public menu_type selectedMenu = menu_type.SERVANT;

    public List<UserServantData> ServantList = new List<UserServantData>();
    public List<UserMonsterData> MonsterList = new List<UserMonsterData>();

    [HideInInspector]
    public int selected_tab = 0;
    [HideInInspector]
    public int selected_unit_idx = 0;

    public enum scroll_type
    {
        SERVANT_INFO = 0,       //서번트 정보(기본)
        MONSTER_INFO,           //몬스터 정보
        SKILL,                  //스킬
        FORMATION                //포메이션
    }
    public scroll_type currentScrollType = 0;


    public void setData()
    {
        ServantList.Clear();
        MonsterList.Clear();

        ServantList = UserDataManager.Inst.GetServantList();
        MonsterList = UserDataManager.Inst.GetMonsterList();
    }

    void Start() {
        initScrollList();
        OnClickMenuButton(0);

        //몬스터가 없을경우
        if (MonsterList.Count <= 0)
        {
            buttonMenu[1].interactable = false;
        }
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

        

        if (selectedMenu == menu_type.SERVANT || selectedMenu == menu_type.MONSTER)
        {
            frameScroll.SetActive(true);
            resetScroll();

            framePartyInfo.SetActive(true);
            frameFormation.SetActive(false);

            updateDetailInfo(scrollList.getFirstItemOrder());
        }
        else if (selectedMenu == menu_type.FORMATION)
        {
            frameScroll.SetActive(false);

            framePartyInfo.SetActive(false);
            frameFormation.SetActive(true);
            updateFormation();
        }

    }

    //좌측 메뉴버튼 클릭
    public void OnClickMenuButton(int tag)
    {
        if (tag == (int)menu_type.SERVANT)
        {
            selectedMenu = menu_type.SERVANT;
            currentScrollType = scroll_type.SERVANT_INFO;
        }
        else if (tag == (int)menu_type.MONSTER)
        {
            selectedMenu = menu_type.MONSTER;
            currentScrollType = scroll_type.MONSTER_INFO;
        }
        else if (tag == (int)menu_type.FORMATION)
        {
            selectedMenu = menu_type.FORMATION;
            SetFormation();
            //currentScrollType = scroll_type.FORMATION;
        }
        updateAllView();
    }

    public void ResetScrollListBySortType(SORT_TYPE type)
    {
        sort_type = type;
        scrollList.SetItemOrder(getOrder());
        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
        scrollList.ScrollViewDidScroll();
        updateDetailInfo(scrollList.getFirstItemOrder());
    }

    //현재 화면에 따른 스크롤 UI 재설정 
    public void resetScroll()
    {
        if (currentScrollType == scroll_type.SERVANT_INFO)
        {
            textOwned.text = string.Format("{0}", ServantList.Count);
            textTotal.text = string.Format("/ {0}", UserDataManager.Inst.GetUserInventoryInfo().servantInventory);
            if (ServantList.Count >= UserDataManager.Inst.GetUserInventoryInfo().servantInventory )
            {
                textOwned.color = Color.red;
            }
            else
            {
                textOwned.color = Color.white;
            }
        }
        else if (currentScrollType == scroll_type.MONSTER_INFO)
        {
            textOwned.text = string.Format("{0}", MonsterList.Count);
            textTotal.text = string.Format("/ {0}", UserDataManager.Inst.GetUserInventoryInfo().monsterInventory);
            if (MonsterList.Count >= UserDataManager.Inst.GetUserInventoryInfo().monsterInventory)
            {
                textOwned.color = Color.red;
            }
            else
            {
                textOwned.color = Color.white;
            }
        }

        scrollList.SetItemOrder(getOrder());
        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
        scrollList.ScrollViewDidScroll();
    }


    //스크롤 생성
    void initScrollList()
    {
        setData();

        scrollList.Init(this, 20, ServantList.Count, getOrder());
    }

    //스크롤 정렬
    private int[] getOrder()
    {
        int[] data_order;
        int total_list_num = 0;

        if (currentScrollType == scroll_type.SERVANT_INFO)
        {
            total_list_num = ServantList.Count;
        }
        else if (currentScrollType == scroll_type.MONSTER_INFO)
        {
            total_list_num = MonsterList.Count;
        }

        data_order = new int[total_list_num];

        for (int i=0; i<data_order.Length; i++)
        {
            data_order[i] = 0;
        }

        if (!SortManager.checkInst())
        {
            DebugLog.Log(false, "Invalid sort manager!");
            return data_order;
        }

        switch (sort_type)
        {
            case SORT_TYPE.POWER:
                for (int i=0; i<total_list_num-1; i++)
                {
                    for (int j = i+1; j < total_list_num; j++)
                    {
                        if (currentScrollType == scroll_type.SERVANT_INFO)
                        {
                            if (Calculator.GetPower(ServantList[i].status, ServantList[i].level) < Calculator.GetPower(ServantList[j].status, ServantList[j].level))//TODO: 임시로 STR
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (currentScrollType == scroll_type.MONSTER_INFO)
                        {
                            if (Calculator.GetPower(MonsterList[i].status, MonsterList[i].level) < Calculator.GetPower(MonsterList[j].status, MonsterList[j].level))//TODO: 임시로 STR
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
            case SORT_TYPE.GRADE:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (currentScrollType == scroll_type.SERVANT_INFO)
                        {
                            if (ServantList[i].grade > ServantList[j].grade)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (currentScrollType == scroll_type.MONSTER_INFO)
                        {
                            if (MonsterList[i].grade > MonsterList[j].grade)
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
            case SORT_TYPE.LEVEL_OR_TIER:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (currentScrollType == scroll_type.SERVANT_INFO)
                        {
                            if (ServantList[i].level * 100 + ServantList[i].exp < ServantList[j].level * 100 + ServantList[j].exp)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (currentScrollType == scroll_type.MONSTER_INFO)
                        {
                            if (MonsterList[i].level * 100 + MonsterList[i].exp < MonsterList[j].level * 100 + MonsterList[j].exp)
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
            case SORT_TYPE.GOT_TIME:
                for (int i = 0; i < total_list_num; i++)
                {
                    data_order[i] = i;
                }
                break;
            case SORT_TYPE.JOB_OR_UPGRADE:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (currentScrollType == scroll_type.SERVANT_INFO)//Servant는 직업 정렬
                        {
                            if (ServantList[i].jobType > ServantList[j].jobType)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (currentScrollType == scroll_type.MONSTER_INFO)//Monster는 업그레이드 정렬
                        {
                            if (MonsterList[i].upgrade < MonsterList[j].upgrade)
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
        selected_unit_idx = _selected_unit_idx;

        if (selectedMenu == menu_type.SERVANT)
        {
            updateServantInfo(ServantList[selected_unit_idx]);
        }
        else if (selectedMenu == menu_type.MONSTER)
        {
            updateMonsterInfo(MonsterList[selected_unit_idx]);
        }

        if (ModelViewManager.checkInst())
        {
            ModelViewManager.Inst.ChangeSelectedUnitModel();
        }
        
    }
    
	public void updateServantInfo(UserServantData s_data)
    {
        FrameServantInfo.GetComponent<ServantInfoManager>().updateServantInfo(s_data);
        FrameServantInfo.SetActive(true);
        FrameMonsterInfo.SetActive(false);
    }

    public void updateMonsterInfo(UserMonsterData m_data)
    {   
        FrameServantInfo.SetActive(false);
        FrameMonsterInfo.GetComponent<MonsterInfoManager>().updateMonsterInfo(m_data);
        FrameMonsterInfo.SetActive(true);
    }

    //강화 완료(서버에서 응답) 후 화면 전체 갱신
    public void updateViewFinishRequest()
    {
        setData();
        frameScroll.SetActive(true);
        if (SubViewUpgrade.checkInst() == true)
        {
            Destroy(SubViewUpgrade.Inst.gameObject);
        }

        scrollList.SetItemOrder(getOrder());
        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
        scrollList.ScrollViewDidScroll();
        updateDetailInfo(scrollList.getFirstItemOrder());
    }

    public void ShowUpgrade()
    {
        //Upgrade는 몬스터일때만
        if (selectedMenu == menu_type.MONSTER)
        {
            frameScroll.SetActive(false);

            GameObject objSubView = Instantiate(Resources.Load("UI/Lobby/SubViewUpgrade")) as GameObject;
            objSubView.name = "SubViewUpgrade";
            objSubView.transform.SetParent(FrameSubView.transform);
            objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            SubViewUpgrade s_upgrade = objSubView.GetComponent<SubViewUpgrade>();
            s_upgrade.SetUpgradeType(UPGRADE_TYPE.MONSTER);
        }
        else
        {
            DebugLog.Log(false, "Warning : Montser upgrade only");
        }
    }

    public void ShowDeconstruction()
    {
        frameScroll.SetActive(false);

        GameObject objSubView = Instantiate(Resources.Load("UI/Lobby/SubViewDeconstruction")) as GameObject;
        objSubView.name = "SubViewDeconstruction";
        objSubView.transform.SetParent(FrameSubView.transform);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        SubViewDeconstruction s_deconstruction = objSubView.GetComponent<SubViewDeconstruction>();
        if (selectedMenu == menu_type.SERVANT)
        {
            s_deconstruction.SetDeconstructionType(DECONSTRUCTION_TYPE.SERVANT);
        }
        else if (selectedMenu == menu_type.MONSTER)
        {
            s_deconstruction.SetDeconstructionType(DECONSTRUCTION_TYPE.MONSTER);
        }

    }


    //장비 설정 버튼
    private EQUIPMENT_TYPE selectedEquipType;

    public EQUIPMENT_TYPE getSelectedEquipType()
    {
        return selectedEquipType;
    }

    public void ShowEquipmentInfo(int btn_tag)
    {
        frameScroll.SetActive(false);
        switch (btn_tag)
        {
            case 0:
                selectedEquipType = EQUIPMENT_TYPE.WEAPON;
                break;
            case 1:
                selectedEquipType = EQUIPMENT_TYPE.ARMOR;
                break;
            case 2:
                selectedEquipType = EQUIPMENT_TYPE.ACCESSSORY;
                break;
            default:
                break;
        }
        GameObject objSubView = Instantiate(Resources.Load("UI/Lobby/SubViewEquipment")) as GameObject;
        objSubView.name = "SubViewEquipment";
        objSubView.transform.SetParent(FrameSubView.transform);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }


    //////////////////////*  Formation 관련 UI  *////////////////////////
    public GameObject frameFormation;
    public Button[] buttonMonsterFormation = new Button[5];
    public Image[] imageMonsterFormation = new Image[5];

    public GameObject[] objectMonsterInfo = new GameObject[5];
    public Image[] imageMonsterType = new Image[5];
    public Image[] imageMonsterExp = new Image[5];
    public Text[] textMonsterLevel = new Text[5];
    public Text[] textMonsterExpPer = new Text[5];

    public Button[] buttonServantFormation = new Button[5];
    public Image[] imageServantFormation = new Image[5];

    public GameObject[] objectServantInfo = new GameObject[5];
    public Image[] imageServantType = new Image[5];
    public Image[] imageServantExp = new Image[5];
    public Text[] textServantLevel = new Text[5];
    public Text[] textServantExpPer = new Text[5];

    public Sprite[] spriteSlot = new Sprite[2];

    public Button buttonSaveFormation;

    public int selectedFormationSlot = -1;     //Selected formation slot idx : 0~4 : Servant, 5~9:Monster
    private int[] formationSlot = new int[10];  //temp formation slot;

    public void SetFormation()
    {
        UserPartyData partyInfo = UserDataManager.Inst.GetUserPartyInfo();
        if (partyInfo == null)
        {
            DebugLog.Log(false, "Invalid Party Info");
            return;
        }

        for (int i = 0; i < 10; i++)
        {
            formationSlot[i] = partyInfo.formationDataDic[i].index;
        }
    }

    public void updateFormation()
    {
        for (int i = 0; i < 5; i++)
        {
            if (formationSlot[i] == 0)
            {
                buttonServantFormation[i].image.sprite = spriteSlot[1];
                imageServantFormation[i].enabled = false;

                objectServantInfo[i].SetActive(false);
            }
            else
            {
                UserServantData servant = UserDataManager.Inst.GetServantInfo(formationSlot[i]);
                if(servant == null)
                {
                    DebugLog.Log(false, "Invalid Servant ID : " + formationSlot[i]);
                    return;
                }

                buttonServantFormation[i].image.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)servant.grade);
                imageServantFormation[i].enabled = true;
                imageServantFormation[i].sprite = CSVData.Inst.GetServantData(servant.id).servantIcon;

                imageServantType[i].sprite = CSVData.Inst.GetSpriteServantJob(servant.jobType);
                textServantLevel[i].text = servant.level.ToString();

                DBExpData dbExpData = CSVData.Inst.GetExpData(servant.level);
                if (dbExpData == null)
                {
                    DebugLog.Log(false, "Invalid Level Data");
                }
                else
                {
                    int exExp = 0;
                    if (servant.level - 1 > 0)
                    {
                        DBExpData exDBExpData = CSVData.Inst.GetExpData(servant.level - 1);
                        if (exDBExpData == null)
                        {
                            DebugLog.Log(false, "Invalid Level Data");
                        }
                        else
                        {
                            exExp = exDBExpData.charExp;
                        }
                    }
                    float expPer = (exExp - servant.exp) / (float)(exExp - dbExpData.charExp);
                    textServantExpPer[i].text = (int)(expPer * 100) + "%";
                    imageServantExp[i].fillAmount = expPer;
                }

                objectServantInfo[i].SetActive(true);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            if(formationSlot[i + 5] == 0)
            {
                buttonMonsterFormation[i].image.sprite = spriteSlot[1];
                imageMonsterFormation[i].enabled = false;

                objectMonsterInfo[i].SetActive(false);
            }
            else
            {
                UserMonsterData monster = UserDataManager.Inst.GetMonsterInfo(formationSlot[i + 5]);
                if (monster == null)
                {
                    DebugLog.Log(false, "Invalid Monster ID : " + formationSlot[i + 5]);
                    return;
                }

                buttonMonsterFormation[i].image.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)monster.grade);
                imageMonsterFormation[i].enabled = true;
                imageMonsterFormation[i].sprite = CSVData.Inst.GetMonsterData(monster.id).monsterIcon;

                imageMonsterType[i].sprite = CSVData.Inst.GetSpriteElementType(monster.elementType);
                textMonsterLevel[i].text = monster.level.ToString();

                DBExpData dbExpData = CSVData.Inst.GetExpData(monster.level);
                if (dbExpData == null)
                {
                    DebugLog.Log(false, "Invalid Level Data");
                }
                else
                {
                    int exExp = 0;
                    if (monster.level - 1 > 0)
                    {
                        DBExpData exDBExpData = CSVData.Inst.GetExpData(monster.level - 1);
                        if (exDBExpData == null)
                        {
                            DebugLog.Log(false, "Invalid Level Data");
                        }
                        else
                        {
                            exExp = exDBExpData.charExp;
                        }
                    }
                    float expPer = (exExp - monster.exp) / (float)(exExp - dbExpData.charExp);
                    textMonsterExpPer[i].text = (int)(expPer * 100) + "%";
                    imageMonsterExp[i].fillAmount = expPer;
                }

                objectMonsterInfo[i].SetActive(true);
            }
        }

    }
    
    public void OnClickFormationSlot(int btn_tag)
    {
        if (frameScroll.gameObject.activeSelf)
        {
            frameScroll.gameObject.SetActive(false);
        }
        else
        {
            selectedFormationSlot = btn_tag;
            if (formationSlot[selectedFormationSlot] > 0)//캐릭터가 설정되어있는 슬롯을 눌렀을때 -> 캐릭터 해제
            {
                if(btn_tag <= DEFINE.ServantMaxFormationNum)
                {
                    if (formationSlot[btn_tag + 5] != 0)
                    {
                        SimpleErrorPopupVC.Inst.UpdateErrorText("THERE IS MONSTER IN FRONT OF SERVANT");
                        return;
                    }
                }

                selectedFormationSlot = -1;
                frameScroll.gameObject.SetActive(false);

                DebugLog.Log(false, "Index : " + btn_tag + " / Ex Party Info Index : " + formationSlot[btn_tag] + " / Ex User Party Info Index : " + UserDataManager.Inst.GetUserPartyInfo().formationDataDic[btn_tag].index);

                formationSlot[btn_tag] = 0;

                DebugLog.Log(false, "Index : " + btn_tag + " / Now Party Info Index : " + formationSlot[btn_tag] + " / Ex User Party Info Index : " + UserDataManager.Inst.GetUserPartyInfo().formationDataDic[btn_tag].index);

                formationSlot[btn_tag] = 0;
                updateFormation();
            }
            else //비어있는 슬롯을 눌렀을때 -> 캐릭터 선택 스크롤 활성화
            {
                selectedFormationSlot = btn_tag;

                if (btn_tag < 5)
                {
                    currentScrollType = scroll_type.SERVANT_INFO;
                }
                else
                {
                    currentScrollType = scroll_type.MONSTER_INFO;
                }

                frameScroll.gameObject.SetActive(true);
                resetScroll();
            }
        }
    }

    public void InsertUnit(int character_unit_idx)
    {
        if(character_unit_idx == 0)
        {
            DebugLog.Log(false, "Index is 0");
            return;
        }

        if (formationSlot[selectedFormationSlot] == character_unit_idx)
        {
            DebugLog.Log(false, "Warning : 이미 포메이션에 등록된 유닛 입니다.");
            return;
        }

        if (selectedFormationSlot <= DEFINE.ServantMaxFormationNum)
        {
            // 서번트일 경우
            for (int i = DEFINE.ServantMinFormationNum; i <= DEFINE.ServantMaxFormationNum; i++)
            {

                if (i == selectedFormationSlot)
                {
                    continue;
                }

                if(character_unit_idx == formationSlot[i])
                {
                    DebugLog.Log(false, "이동");
                    formationSlot[i] = 0;
                }

            }
        }
        else if(selectedFormationSlot <= DEFINE.MonsterMaxFormationNum)
        {
            if(formationSlot[selectedFormationSlot - 5] == 0)
            {
                SimpleErrorPopupVC.Inst.UpdateErrorText("THERE IS NO SERVANT IN THE BACK");
                return;
            }

            // 몬스터일 경우
            for (int i = DEFINE.MonsterMinFormationNum; i <= DEFINE.MonsterMaxFormationNum; i++)
            {

                if (i == selectedFormationSlot)
                {
                    continue;
                }

                if (character_unit_idx == formationSlot[i])
                {
                    DebugLog.Log(false, "이동");
                    formationSlot[i] = 0;
                }
            }
        }

        DebugLog.Log(false, "Index : " + selectedFormationSlot + " / Now Party Info Index : " + formationSlot[selectedFormationSlot] + " / Ex User Party Info Index : " + UserDataManager.Inst.GetUserPartyInfo().formationDataDic[selectedFormationSlot].index);

        formationSlot[selectedFormationSlot] = character_unit_idx;

        DebugLog.Log(false, "Index : " + selectedFormationSlot + " / Now Party Info Index : " + formationSlot[selectedFormationSlot] + " / Ex User Party Info Index : " + UserDataManager.Inst.GetUserPartyInfo().formationDataDic[selectedFormationSlot].index);

        frameScroll.SetActive(false);
        updateFormation();
    }

    public void OnClickSaveFormation()
    {
        bool isDiff = false;

        UserPartyData partyData = UserDataManager.Inst.GetUserPartyInfo();
        List<int> indexList = new List<int>();
        for(int i = 0; i < 10; i++)
        {
            indexList.Add(formationSlot[i]);
            if(partyData.formationDataDic[i].index != formationSlot[i])
            {
                isDiff = true;
            }
        }

        if(isDiff == false)
        {
            DebugLog.Log(false, "Party Already Save");
            SimpleErrorPopupVC.Inst.UpdateErrorText("Party Already Save");
            return;
        }

        // TODO : 추후 파티가 늘어날수도
        int partyIndex = 1;

#if UNITY_EDITOR
        Cheat.Inst.RequestSavePartyCheat(partyIndex, indexList);
#else
        PacketManager.Inst.RequestSaveParty(partyIndex, indexList);
#endif
    }

}
