using UnityEngine;
using UnityEngine.UI;

public class StagePage : MonoBehaviour {

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

    public GameObject[] OnActiveImage = new GameObject[6];
    public GameObject[] OnActiveStageList = new GameObject[6];
    public GameObject StageScreenBackButton;
    private int maxStageList = 6;

    void Awake ()
    {
        StageInfoPage.SetActive(false);
        InitStageButton();
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
            Debug.LogError("Invalid Stage Type & Floor : " + stageType + " & " + stageFloor);
            return;
        }

        RewardRankExp.text = "Rank Exp - " + rewardData.rankExp;
        RewardCharExp.text = "Char Exp - " + rewardData.charExp;
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
            Debug.LogError("Invalid Stage Type & Floor : " + stageType + " & " + stageFloor);
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

    public void OnClickStageButton(int stageIndex)
    {
        Debug.Log(stageIndex);

        stageType = stageIndex / 100;
        stageFloor = stageIndex % 100;

        // 임시로 때려박기
        if (stageType == 1)
        {
            StageText.text = "Stage - Garden";
        }
        else if (stageType == 2)
        {
            StageText.text = "Stage - Lava";
        }
        else if (stageType == 3)
        {
            StageText.text = "Stage - Ice";
        }
        else if (stageType == 4)
        {
            StageText.text = "Stage - Under";
        }
        else if (stageType == 5)
        {
            StageText.text = "Stage - Light";
        }
        else if (stageType == 6)
        {
            StageText.text = "Stage - Dark";
        }

        switch (stageFloor)
        {
            case 1:
                StageDetailText.text = "1F - Grass";
                break;
            case 2:
                StageDetailText.text = "2F - G Trio";
                break;
            case 3:
                StageDetailText.text = "3F - Fever";
                break;
            case 4:
                StageDetailText.text = "4F - Happy Bears";
                break;
            case 5:
                StageDetailText.text = "5F - Earth";
                break;
            case 6:
                StageDetailText.text = "6F - Grassland";
                break;
            case 7:
                StageDetailText.text = "7F - GrassKing";
                break;
            case 8:
                StageDetailText.text = "8F - Lost Earth";
                break;
            case 9:
                StageDetailText.text = "9F - GrassEmperor";
                break;
            case 10:
                StageDetailText.text = "10F - End of Earth";
                break;
            default:
                break;
        }

        SetRewardInfo();
        SetEnemyInfo();

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
            Debug.LogError("Invalid User Data");
        }

#if UNITY_EDITOR
        {
            Cheat.Inst.RequestStageStartCheat(stageType, stageFloor, 1);
        }
#else
        {
            PacketManager.Inst.RequestStageStart(stageType, stageFloor, 1);
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
        StageScreenBackButton.SetActive(false);
    }

    public void OnClickStageScreen(int stageType)
    {
        InitStageButton();

        if (stageType >= maxStageList)
        {
            Debug.LogError("Invalid Stage Type : " + stageType);
            return;
        }

        StageScreenBackButton.SetActive(true);
        OnActiveImage[stageType].SetActive(true);
        OnActiveStageList[stageType].SetActive(true);
    }
}
