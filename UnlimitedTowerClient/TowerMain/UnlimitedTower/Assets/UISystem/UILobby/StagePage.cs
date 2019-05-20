using UnityEngine;
using UnityEngine.UI;

public class StagePage : MonoSingleton<StagePage> {

    public GameObject StageInfoPage;
    public Text StageText;
    public Text StageDetailText;

    public Text RewardRankExp;
    public Text RewardCharExp;
    public Text RewardUTG;

    public GameObject enemyInfo;
    public GameObject enemyObjectPrefab;
    public GameObject[] enemyObjects = null;

    public GameObject rewardInfo;
    public GameObject rewardObjectPrefab;
    public GameObject[] rewardObjects = null;

    private int stageType = 0;
    private int stageFloor = 0;
    private int stageDifficult = 0;

    public GameObject[] OnActiveImage = new GameObject[6];
    public GameObject[] OnActiveStageList = new GameObject[6];
    public GameObject StageScreenBackButton;
    private int maxStageList = 6;

    public ScrollListManager scrollList;

    public GameObject NeedTicket;
    public Image NeedTicketImage;
    public Text NeedTicketCount;

    public Text textSelectDifficult;
    public GameObject FrameDifficult;
    private bool isShowDifficultList = false;


    void Awake ()
    {
        StageInfoPage.SetActive(false);
        InitStageButton();

        stageType = 1;
        initScrollList();
        scrollList.gameObject.SetActive(false);
    }

    public void SetRewardInfo()
    {
        if(rewardObjects != null)
        {
            for(int i = 0; i < rewardObjects.Length; i++)
            {
                Destroy(rewardObjects[i]);
            }

            rewardObjects = null;
        }

        DBStageRewardData rewardData = CSVData.Inst.GetStageRewardData(stageType, stageFloor);
        if(rewardData == null)
        {
            DebugLog.Log(true, "Invalid Stage Type & Floor : " + stageType + " & " + stageFloor);
            return;
        }

        RewardRankExp.text = rewardData.rankExp.ToString();
        RewardCharExp.text = rewardData.charExp.ToString();
        RewardUTG.text = rewardData.rewardUTGString;

        rewardObjects = new GameObject[rewardData.rewardDataList.Count];
        for(int i = 0; i < rewardData.rewardDataList.Count; i++)
        {
            GameObject rewardObject = Instantiate(rewardObjectPrefab);
            rewardObject.transform.SetParent(rewardInfo.transform);
            rewardObject.GetComponent<RewardInfoObject>().SetResourceImage(rewardData.rewardDataList[i]);
            rewardObjects[i] = rewardObject;
        }

        rewardInfo.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2( (100 + (111 * rewardData.rewardDataList.Count)), 100);
    }

    public void SetEnemyInfo()
    {
        if (enemyObjects != null)
        {
            for (int i = 0; i < enemyObjects.Length; i++)
            {
                Destroy(enemyObjects[i]);
            }

            enemyObjects = null;
        }

        DBStageData stageData = CSVData.Inst.GetStageData(stageType, stageFloor);
        if (stageData == null)
        {
            DebugLog.Log(true, "Invalid Stage Type & Floor : " + stageType + " & " + stageFloor);
            return;
        }
        

       enemyObjects = new GameObject[stageData.enemyIdList.Count];
        for (int i = 0; i < stageData.enemyIdList.Count; i++)
        {
            GameObject enemyObject = Instantiate(enemyObjectPrefab);
            enemyObject.transform.SetParent(enemyInfo.transform);
            enemyObject.GetComponent<EnemyInfoObject>().SetEnemyImage(stageData.enemyIdList[i]);
            enemyObjects[i] = enemyObject;
        }

        enemyInfo.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2((100 + (111 * stageData.enemyIdList.Count)), 100);
    }

    public void ShowDifficultList()
    {
        isShowDifficultList = !isShowDifficultList;

        if (isShowDifficultList)
        {
            Animation anim = FrameDifficult.GetComponent<Animation>();
            foreach (AnimationState state in anim)
            {
                state.speed = -1f;
                state.time = state.length;
            }
            anim.Play();
        }
        else
        {
            Animation anim = FrameDifficult.GetComponent<Animation>();
            foreach (AnimationState state in anim)
            {
                state.speed = 1f;
                //state.time = state.length;
            }
            anim.Play();
        }
    }

    public void SetDifficultInfo(int difficult)
    {
        stageDifficult = difficult;
        int itemID = 0;
        switch(difficult)
        {
            case 0:
                NeedTicket.SetActive(false);
                StageDetailText.text = stageFloor + "F - Easy";
                textSelectDifficult.text = "Easy";

                ShowDifficultList();
                return;
            case 1:
                itemID = 500200;
                StageDetailText.text = stageFloor + "F - Normal";
                textSelectDifficult.text = "Normal";
                break;
            case 2:
                itemID = 500210;
                StageDetailText.text = stageFloor + "F - Hard";
                textSelectDifficult.text = "Hard";
                break;
            case 3:
                itemID = 500220;
                StageDetailText.text = stageFloor + "F - Nightmare";
                textSelectDifficult.text = "Nightmare";
                break;
            case 4:
                itemID = 500230;
                StageDetailText.text = stageFloor + "F - Hell";
                textSelectDifficult.text = "Hell";
                break;
            default:
                DebugLog.Log(false, "InvalidRequest" + difficult);
                return;
        }
        ShowDifficultList();

        NeedTicket.SetActive(true);
        NeedTicketImage.sprite = CSVData.Inst.GetSpriteItemIcon(itemID);
        int itemCount = UserDataManager.Inst.GetItemCount(itemID);
        NeedTicketCount.text = " x 1 / " + itemCount;
        if (itemCount < 1)
        {
            NeedTicketCount.color = Color.red;
        }
        else
        {
            NeedTicketCount.color = Color.white;
        }
    }

    public void OnClickStageButton(int stageIndex)
    {
        //DebugLog.Log(false, stageIndex);

        stageType = stageIndex / 100;
        stageFloor = stageIndex % 100;

        // 임시로 때려박기
        if (stageType == 1)
        {
            StageText.text = "Stage - Under Ruins";
        }
        else if (stageType == 2)
        {
            StageText.text = "Stage - Lava Zone";
        }
        else if (stageType == 3)
        {
            StageText.text = "Stage - Ice Berg";
        }
        else if (stageType == 4)
        {
            StageText.text = "Stage - Sky Temple";
        }
        else if (stageType == 5)
        {
            StageText.text = "Stage - Grassland";
        }
        else if (stageType == 6)
        {
            StageText.text = "Stage - Dark Forest";
        }

        SetRewardInfo();
        SetEnemyInfo();
        
        //난이도 선택 Default
        stageDifficult = 0;
        NeedTicket.SetActive(false);
        StageDetailText.text = stageFloor + "F - Easy";
        textSelectDifficult.text = "Easy";
        //end 난이도 선택 Default

        StageInfoPage.SetActive(true);

    }

    public void OnClickExitButton()
    {
        StageInfoPage.SetActive(false);
    }

    public void OnClickStartButton()
    {
        if(UserDataManager.Inst.GetUserPartyInfo().partyIndex == 0)
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Invalid User State");
            return;
        }

        UserInventoryInfo inventoryInfo = UserDataManager.Inst.GetUserInventoryInfo();
        if (inventoryInfo == null)
        {
            DebugLog.Log(false, "Invalid Inventory Info");
            return;
        }

        if(inventoryInfo.servantInventory < UserDataManager.Inst.GetServantCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Servant Inventory is Full");
            return;
        }

        if (inventoryInfo.monsterInventory < UserDataManager.Inst.GetMonsterCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Monster Inventory is Full");
            return;
        }

        if (inventoryInfo.equipmentInventory < UserDataManager.Inst.GetEquipmentCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Equipment Inventory is Full");
            return;
        }

        if (inventoryInfo.itemInventory < UserDataManager.Inst.GetItemTotalCount())
        {
            SimpleErrorPopupVC.Inst.UpdateErrorText("Item Inventory is Full");
            return;
        }

        // 재화 검사 추가 필요

#if UNITY_EDITOR
        {
            Cheat.Inst.RequestStageStartCheat(stageType, stageFloor, stageDifficult, 1);
        }
#else
        {
            PacketManager.Inst.RequestStageStart(stageType, stageFloor, stageDifficult, 1);
        }
#endif
    }

    public void InitStageButton()
    {
        for (int i = 0; i < maxStageList; i++)
        {
            OnActiveImage[i].SetActive(false);
            OnActiveStageList[i].SetActive(false);
        }
        scrollList.gameObject.SetActive(false);
        StageScreenBackButton.SetActive(false);
    }

    public void OnClickStageScreen(int stage_type)
    {
        stageType = stage_type + 1;//인자값 stage_type가 0부터 시작함
        InitStageButton();

        if (stageType > maxStageList)
        {
            DebugLog.Log(true, "Invalid Stage Type : " + stageType);
            return;
        }

        StageScreenBackButton.SetActive(true);
        OnActiveImage[stageType - 1].SetActive(true);
        //OnActiveStageList[stageType - 1].SetActive(true);
        
        scrollList.SetItemOrder(getOrder());
        scrollList.rectTrScrollLayer.anchoredPosition = Vector2.zero;
        scrollList.ScrollViewDidScroll();
        scrollList.gameObject.SetActive(true);
    }

    public int GetStageType()
    {
        return stageType;
    }

    private int total_scoll_unit_num;
    //스크롤 생성
    void initScrollList()
    {
        total_scoll_unit_num = 10; //스테이지 타입별 스테이지 갯수 (현재 고정 10개, 나중에 유동적으로 바뀔 수 있음)

        scrollList.Init(this, 20, total_scoll_unit_num, getOrder());
    }

    //스크롤 정렬
    private int[] getOrder()
    {
        int[] data_order;

        data_order = new int[total_scoll_unit_num];

        for (int i = 0; i < data_order.Length; i++)
        {
            data_order[i] = 0;
        }

        for (int i = 0; i < total_scoll_unit_num; i++)
        {
            data_order[i] = i;
        }

        return data_order;
    }
}
