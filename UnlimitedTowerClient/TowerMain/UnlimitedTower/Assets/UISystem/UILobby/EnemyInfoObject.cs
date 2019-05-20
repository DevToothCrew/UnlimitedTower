using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoObject : MonoBehaviour {

    public Image GradeImage;
    public Image ResourceImage;

    public void SetEnemyImage(int enemyID, int difficult)
    {
        DBStageEnemyData enemyData = CSVData.Inst.GetStageEnemyData(enemyID);
        if(enemyData == null)
        {
            DebugLog.Log(true, "Invalid Enemy Data ID : " + enemyID);
            return;
        }
        
        GradeImage.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)difficult);
        ResourceImage.sprite = CSVData.Inst.GetMonsterIcon(enemyData.id);
    }

}
