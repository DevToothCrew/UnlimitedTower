using UnityEngine;
using UnityEngine.UI;

public class RewardInfoObject : MonoBehaviour {

    public Image GradeImage;
    public Image ResourceImage;

    public void SetResourceImage(DBRewardItemData rewardData)
    {
        DBItemData itemData = CSVData.Inst.GetItemData(rewardData.rewardID);
        if(itemData == null)
        {
            DebugLog.Log(true, "Invalid Reward ID : " + rewardData.rewardID);
            return;
        }
        GradeImage.sprite = CSVData.Inst.GetSpriteGrade(itemData.grade);
        ResourceImage.sprite = itemData.ItemIcon;
    }
}
