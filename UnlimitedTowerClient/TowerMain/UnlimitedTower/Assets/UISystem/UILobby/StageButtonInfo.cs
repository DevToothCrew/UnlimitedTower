using UnityEngine;
using UnityEngine.UI;

public class StageButtonInfo : MonoBehaviour {

    public int stageNum;
    public Image enemyImage;

	// Use this for initialization
	void OnEnable () {
        DBStageData stageData = CSVData.Inst.GetStageData(stageNum/100, stageNum%100);
        if(stageData == null)
        {
            Debug.Log("Invalid Stage Num : " + stageNum);
            return;
        }

        int enemyID = stageData.enemyIdList[0];
        if(enemyID == 0)
        {
            Debug.Log("Invalid Enemy ID Stage Num : " + stageNum);
            return;
        }
        enemyImage.sprite = CSVData.Inst.GetMonsterIcon(enemyID);
    }
}
