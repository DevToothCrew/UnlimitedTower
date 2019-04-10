using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyInfoVC : MonoSingleton<PartyInfoVC>
{
    public GameObject framePartyInfo;

    //SubView
    public GameObject FrameSubView;
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

    public GameObject frameScroll;
    public ScrollListManager scrollList;

    //Detail Info UI
    public Text textLevel;
    public Text textExp;
    public Image imageExp;
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
    public Sprite spriteEmptySlot;


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
    [HideInInspector]
    public int selected_unit_idx = 0;

    public enum scroll_type
    {
        SERVANT_INFO = 0,       //서번트 정보(기본)
        MONSTER_INFO,           //몬스터 정보
        EQUIPMENT_WEAPON,       //무기 설정
        EQUIPMENT_ARMOR,        //방어구 설정
        EQUIPMENT_ACC,          //장식품 설정
        SKILL,                  //스킬
        FORMATION                //포메이션
    }
    public scroll_type currentScrollType = 0;


    void setData()
    {
        ServantList.Clear();
        MonsterList.Clear();

        ServantList = UserDataManager.Inst.GetServantList();
        MonsterList = UserDataManager.Inst.GetMonsterList();
    }

    void Start() {
        initScrollList();
        OnClickMenuButton(0);
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

        for (int i = 0; i < buttonEquipment.Length; i++)
            buttonEquipment[i].gameObject.SetActive(false);

        if (selectedMenu == menu_type.SERVANT)
        {   
            frameScroll.SetActive(true);
            resetScroll(currentScrollType);

            for (int i = 0; i < 3; i++)
                buttonEquipment[i].gameObject.SetActive(true);

            framePartyInfo.SetActive(true);
            frameFormation.SetActive(false);
        }
        else if (selectedMenu == menu_type.MONSTER)
        {
            frameScroll.SetActive(true);
            resetScroll(currentScrollType);

            framePartyInfo.SetActive(true);
            frameFormation.SetActive(false);
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


    //현재 화면에 따른 스크롤 UI 재설정 
    public void resetScroll(scroll_type type)
    {
        currentScrollType = type;
        if (currentScrollType == scroll_type.SERVANT_INFO)
        {
            textOwned.text = string.Format("{0}", ServantList.Count);
            textTotal.text = string.Format("/ {0}", 100);
        }
        else if (currentScrollType == scroll_type.MONSTER_INFO)
        {
            textOwned.text = string.Format("{0}", MonsterList.Count);
            textTotal.text = string.Format("/ {0}", 100);
        }

        scrollList.SetItemOrder(getOrder());
        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
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

        switch (sortType)
        {
            case 0:
                for (int i=0; i<total_list_num-1; i++)
                {
                    for (int j = i+1; j < total_list_num; j++)
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
            UserServantData servantData = ServantList[selected_unit_idx];
            if(servantData == null)
            {
                Debug.Log("Invalid Servant Inddex : " + selected_unit_idx);
            }

            DBServantData dBServantData = CSVData.Inst.GetServantData(servantData.id);
            if (dBServantData == null)
            {
                Debug.Log("Invalid Servant ID : " + servantData.id);
            }

            for (EQUIPMENT_TYPE type = EQUIPMENT_TYPE.WEAPON; type < EQUIPMENT_TYPE.MAX; type++)
            {
                if (servantData.equipmentDic[type] != 0)
                {
                    UserEquipmentData equip_info = UserDataManager.Inst.GetEquipmentInfo(servantData.equipmentDic[type]);
                    if(equip_info == null)
                    {
                        Debug.LogError("Invalid Index : " + servantData.equipmentDic[type]);
                        return;
                    }

                    buttonEquipment[(int)type].image.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)equip_info.grade);
                    buttonEquipment[(int)type].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    buttonEquipment[(int)type].transform.GetChild(0).GetComponent<Image>().sprite = CSVData.Inst.GetEquipmentData(equip_info.id).equipmentIcon;
                }
                else
                {
                    buttonEquipment[(int)type].image.sprite = spriteEmptySlot;
                    buttonEquipment[(int)type].transform.GetChild(0).GetComponent<Image>().enabled = false;
                }
                
            }
            

            textLevel.text = string.Format("{0}", servantData.level);
            textCharacterName.text = string.Format("{0}", dBServantData.name);

            DBExpData dbExpData = CSVData.Inst.GetExpData(servantData.level);
            if (dbExpData == null)
            {
                Debug.Log("Invalid Level Data");
            }
            else
            {
                int exExp = 0;
                if (servantData.level - 1 > 0)
                {
                    DBExpData exDBExpData = CSVData.Inst.GetExpData(servantData.level - 1);
                    if(exDBExpData == null)
                    {
                        Debug.Log("Invalid Level Data");
                    }
                    else
                    {
                        exExp = exDBExpData.charExp;
                    }
                }

                textExp.text = servantData.exp + " / " + dbExpData.charExp;
                imageExp.fillAmount = (exExp - servantData.exp) / (float)(exExp - dbExpData.charExp);
            }

            textStr.text = string.Format("{0}", servantData.status.basicStr);
            textDex.text = string.Format("{0}", servantData.status.basicDex);
            textInt.text = string.Format("{0}", servantData.status.basicInt);

            textPAtk.text = string.Format("{0}", servantData.atk);
            textPDef.text = string.Format("{0}", servantData.def);

            textMAtk.text = string.Format("{0}", servantData.mAtk);
            textMDef.text = string.Format("{0}", servantData.mDef);

            textPCri.text = string.Format("{0}", dBServantData.criDmg);
            textPCriPer.text = string.Format("{0}", dBServantData.criPer);

            textMCri.text = string.Format("{0}", dBServantData.mcriDmg);
            textMCriPer.text = string.Format("{0}", dBServantData.mcriPer);

            textSpeed.text = string.Format("{0}", dBServantData.speed);
        }
        else if (selectedMenu == menu_type.MONSTER)
        {
            UserMonsterData monsterData = MonsterList[selected_unit_idx];
            if (monsterData == null)
            {
                Debug.Log("Invalid Monster Inddex : " + selected_unit_idx);
            }

            DBMonsterData dBMonsterData = CSVData.Inst.GetMonsterData(monsterData.id);
            if(dBMonsterData == null)
            {
                Debug.Log("Invalid Monster ID : " + monsterData.id);
            }

            textLevel.text = string.Format("{0}", monsterData.level);
            textCharacterName.text = string.Format("{0}", dBMonsterData.name);
            DBExpData dbExpData = CSVData.Inst.GetExpData(monsterData.level);
            if (dbExpData == null)
            {
                Debug.Log("Invalid Level Data");
            }
            else
            {
                int exExp = 0;
                if (monsterData.level - 1 > 0)
                {
                    DBExpData exDBExpData = CSVData.Inst.GetExpData(monsterData.level - 1);
                    if (exDBExpData == null)
                    {
                        Debug.Log("Invalid Level Data");
                    }
                    else
                    {
                        exExp = exDBExpData.charExp;
                    }
                }

                textExp.text = monsterData.exp + " / " + dbExpData.charExp;
                imageExp.fillAmount = (exExp - monsterData.exp) / (float)(exExp - dbExpData.charExp);
            }

            textStr.text = string.Format("{0}", monsterData.status.basicStr);
            textDex.text = string.Format("{0}", monsterData.status.basicDex);
            textInt.text = string.Format("{0}", monsterData.status.basicInt);

            textPAtk.text = string.Format("{0}", monsterData.atk);
            textPDef.text = string.Format("{0}", monsterData.def);

            textMAtk.text = string.Format("{0}", monsterData.mAtk);
            textMDef.text = string.Format("{0}", monsterData.mDef);

            textPCri.text = string.Format("{0}", dBMonsterData.criDmg);
            textPCriPer.text = string.Format("{0}", dBMonsterData.criPer);

            textMCri.text = string.Format("{0}", dBMonsterData.mcriDmg);
            textMCriPer.text = string.Format("{0}", dBMonsterData.mcriPer);

            textSpeed.text = string.Format("{0}", dBMonsterData.speed);
        }

        if (LobbyManager.checkInst())
        {
            LobbyManager.Inst.ChangeSelectedUnitModel();
        }
        
    }
    
	
    public void OnClickDeconstruction()
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
    public void OnClickEquipment(int btn_tag)
    {
        frameScroll.SetActive(false);
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
        GameObject objSubView = Instantiate(Resources.Load("UI/Lobby/SubViewEquipment")) as GameObject;
        objSubView.name = "SubViewEquipment";
        objSubView.transform.SetParent(FrameSubView.transform);
        objSubView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        /*if (objSubView == null)
        {
            
        }
        else
        {
            Debug.Log("Running Equipment View!");
        }*/

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
            Debug.Log("Invalid Party Info");
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
                    Debug.Log("Invalid Servant ID : " + formationSlot[i]);
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
                    Debug.Log("Invalid Level Data");
                }
                else
                {
                    int exExp = 0;
                    if (servant.level - 1 > 0)
                    {
                        DBExpData exDBExpData = CSVData.Inst.GetExpData(servant.level - 1);
                        if (exDBExpData == null)
                        {
                            Debug.Log("Invalid Level Data");
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
                    Debug.Log("Invalid Monster ID : " + formationSlot[i + 5]);
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
                    Debug.Log("Invalid Level Data");
                }
                else
                {
                    int exExp = 0;
                    if (monster.level - 1 > 0)
                    {
                        DBExpData exDBExpData = CSVData.Inst.GetExpData(monster.level - 1);
                        if (exDBExpData == null)
                        {
                            Debug.Log("Invalid Level Data");
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
                selectedFormationSlot = -1;
                frameScroll.gameObject.SetActive(false);

                Debug.Log("Index : " + btn_tag + " / Ex Party Info Index : " + formationSlot[btn_tag] + " / Ex User Party Info Index : " + UserDataManager.Inst.GetUserPartyInfo().formationDataDic[btn_tag].index);

                formationSlot[btn_tag] = 0;

                Debug.Log("Index : " + btn_tag + " / Now Party Info Index : " + formationSlot[btn_tag] + " / Ex User Party Info Index : " + UserDataManager.Inst.GetUserPartyInfo().formationDataDic[btn_tag].index);

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
                resetScroll(currentScrollType);
            }
        }
    }

    public void InsertUnit(int character_unit_idx)
    {
        if(character_unit_idx == 0)
        {
            Debug.Log("Index is 0");
            return;
        }

        if (formationSlot[selectedFormationSlot] == character_unit_idx)
        {
            Debug.Log("Warning : 이미 포메이션에 등록된 유닛 입니다.");
            return;
        }

        if (selectedFormationSlot <= DEFINE.ServantMaxFormationNum)
        {
            // 서번트일 경우
            for(int i = DEFINE.ServantMinFormationNum; i <= DEFINE.ServantMaxFormationNum; i++)
            {
                if(i == selectedFormationSlot)
                {
                    continue;
                }

                if(character_unit_idx == formationSlot[i])
                {
                    Debug.Log("이동");
                    formationSlot[i] = 0;
                }
            }
        }
        else if(selectedFormationSlot <= DEFINE.MonsterMaxFormationNum)
        {
            // 몬스터일 경우
            for (int i = DEFINE.MonsterMinFormationNum; i <= DEFINE.MonsterMaxFormationNum; i++)
            {
                if (i == selectedFormationSlot)
                {
                    continue;
                }

                if (character_unit_idx == formationSlot[i])
                {
                    Debug.Log("이동");
                    formationSlot[i] = 0;
                }
            }
        }

        Debug.Log("Index : " + selectedFormationSlot + " / Now Party Info Index : " + formationSlot[selectedFormationSlot] + " / Ex User Party Info Index : " + UserDataManager.Inst.GetUserPartyInfo().formationDataDic[selectedFormationSlot].index);

        formationSlot[selectedFormationSlot] = character_unit_idx;

        Debug.Log("Index : " + selectedFormationSlot + " / Now Party Info Index : " + formationSlot[selectedFormationSlot] + " / Ex User Party Info Index : " + UserDataManager.Inst.GetUserPartyInfo().formationDataDic[selectedFormationSlot].index);

        frameScroll.SetActive(false);
        updateFormation();
    }

    public void OnClickSaveFormation()
    {
        List<int> indexList = new List<int>();
        for(int i = 0; i < 10; i++)
        {
            indexList.Add(formationSlot[i]);
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
