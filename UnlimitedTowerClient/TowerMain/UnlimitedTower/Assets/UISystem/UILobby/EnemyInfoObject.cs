using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoObject : MonoBehaviour {

    public Image GradeImage;
    public Image ResourceImage;

    public void SetEnemyImage(int enemyID)
    {
        DBStageEnemyData enemyData = CSVData.Inst.GetStageEnemyData(enemyID);
        if(enemyData == null)
        {
            Debug.LogError("Invalid Enemy Data ID : " + enemyID);
            return;
        }
        
        GradeImage.sprite = CSVData.Inst.GetSpriteGrade(enemyData.grade);
        ResourceImage.sprite = CSVData.Inst.GetMonsterIcon(enemyData.charID);
    }

}
