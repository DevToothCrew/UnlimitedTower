using UnityEngine;
using UnityEngine.UI;

public class StagePage : MonoBehaviour {

    public GameObject StageInfoPage;
    public Text StageText;
    public Text StageDetailText;

    private int stageType = 0;
    private int stageFloor = 0;

    void Awake ()
    {
        StageInfoPage.SetActive(false);
	}

    public void SetRewardInfo()
    {

    }

    public void SetEnemyInfo()
    {

    }

    public void OnClickStageButton(int stageIndex)
    {
        stageType = stageIndex / 100;
        stageFloor = stageIndex % 100;

        // 임시로 때려박기
        if(stageType == 1)
        {
            StageText.text = "Stage - Garden";
        }

        switch(stageFloor)
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


}
