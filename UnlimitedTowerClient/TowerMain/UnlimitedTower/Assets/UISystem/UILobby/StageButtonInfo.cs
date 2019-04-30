using UnityEngine;
using UnityEngine.UI;

public class StageButtonInfo : ScrollListUnit {
    public int stageNum;
    public Image enemyImage;
    public Text textStageType;
    public Text textStageFloor;

    public override void UpdateUnit(int main_idx)
    {
        int stage_type = StagePage.Inst.GetStageType();
        stageNum = (stage_type * 100) + main_idx + 1;
        int stage_floor = (stageNum % 100);
        DBStageData stageData = CSVData.Inst.GetStageData(stage_type, stage_floor);
        if (stageData == null)
        {
            DebugLog.Log(false, "Invalid Stage Num : " + stageNum);
            return;
        }

        int enemyID = stageData.enemyIdList[0];
        if (enemyID == 0)
        {
            DebugLog.Log(false, "Invalid Enemy ID Stage Num : " + stageNum);
            return;
        }

        if (stage_type == 1)
        {
            textStageType.text = "Stage - Under Ruins";
        }
        else if (stage_type == 2)
        {
            textStageType.text = "Stage - Lava Zone";
        }
        else if (stage_type == 3)
        {
            textStageType.text = "Stage - Ice Berg";
        }
        else if (stage_type == 4)
        {
            textStageType.text = "Stage - Sky Temple";
        }
        else if (stage_type == 5)
        {
            textStageType.text = "Stage - Grassland";
        }
        else if (stage_type == 6)
        {
            textStageType.text = "Stage - Dark Forest";
        }

        textStageFloor.text = string.Format("{0}F ", stage_floor);
        enemyImage.sprite = CSVData.Inst.GetMonsterIcon(enemyID);
    }

    public override void Selected(bool selected)
    {
        //if (selected)
        //{
        //    GetComponent<Image>().sprite = GetComponent<Button>().spriteState.pressedSprite;vv
        //}
        //else
        //{
        //    GetComponent<Image>().sprite = GetComponent<Button>().spriteState.disabledSprite;
        //}
    }

    public override void OnClickUnit()
    {
        StagePage.Inst.OnClickStageButton(stageNum);
    }
}
