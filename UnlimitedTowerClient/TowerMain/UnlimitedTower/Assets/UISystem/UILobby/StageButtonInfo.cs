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
        stageNum = (stage_type * 100) + 1 + main_idx;
        DBStageData stageData = CSVData.Inst.GetStageData(stageNum / 100, stageNum % 100);

        if (stageData == null)
        {
            Debug.Log("Invalid Stage Num : " + stageNum);
            return;
        }

        int enemyID = stageData.enemyIdList[0];
        if (enemyID == 0)
        {
            Debug.Log("Invalid Enemy ID Stage Num : " + stageNum);
            return;
        }

        //스테이지 타입명 표시해야함.
        textStageType.text = "Stage Type";
        textStageFloor.text = string.Format("{0}F - {1}", stageData.stageFloor, stageData.stageString);
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
