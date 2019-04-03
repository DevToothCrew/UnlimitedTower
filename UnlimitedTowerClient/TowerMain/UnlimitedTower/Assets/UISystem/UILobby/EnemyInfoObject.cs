using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoObject : MonoBehaviour {

    public Image GradeImage;
    public Image ResourceImage;

    // 이 데이터를 이용해 뭔가 쓸일이 있을까?
    private DBStageEnemyData enemyData;

    public void SetEnemyImage(int enemyID)
    {
        enemyData = CSVData.Inst.GetStageEnemyData(enemyID);
        if(enemyData == null)
        {
            Debug.LogError("Invalid Enemy Data ID : " + enemyID);
            return;
        }
        
        GradeImage.sprite = CSVData.Inst.GetSpriteGrade(enemyData.grade);
        ResourceImage.sprite = CSVData.Inst.GetMonsterIcon(enemyData.charID);
    }

}
