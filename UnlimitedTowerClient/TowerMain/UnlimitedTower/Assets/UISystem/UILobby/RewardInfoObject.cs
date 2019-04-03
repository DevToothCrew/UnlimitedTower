using UnityEngine;
using UnityEngine.UI;

public class RewardInfoObject : MonoBehaviour {

    public Image GradeImage;
    public Image ResourceImage;

    public void SetResourceImage(DBRewardData rewardData)
    {
        switch(rewardData.rewardType)
        {
            case REWARD_TYPE.SERVANT:
                DBServantData servantData = CSVData.Inst.GetServantData(rewardData.rewardID);
                if(servantData == null)
                {
                    Debug.LogError("Invalid Reward ID : " + rewardData.rewardID);
                    return;
                }

                GradeImage.sprite = CSVData.Inst.GetSpriteGrade(rewardData.rewardGrade);
                ResourceImage.sprite = servantData.servantIcon;
                break;

            case REWARD_TYPE.MONSTER:
                DBMonsterData monsterData = CSVData.Inst.GetMonsterData(rewardData.rewardID);
                if(monsterData == null)
                {
                    Debug.LogError("Invalid Reward ID : " + rewardData.rewardID);
                    return;
                }

                GradeImage.sprite = CSVData.Inst.GetSpriteGrade(rewardData.rewardGrade);
                ResourceImage.sprite = monsterData.monsterIcon;
                break;

            case REWARD_TYPE.EQUIPMENT:
                DBEquipmentData equipmentData = CSVData.Inst.GetEquipmentData(rewardData.rewardID);
                if(equipmentData == null)
                {
                    Debug.LogError("Invalid Reard ID : " + rewardData.rewardID);
                    return;
                }

                GradeImage.sprite = CSVData.Inst.GetSpriteGrade(rewardData.rewardGrade);
                ResourceImage.sprite = equipmentData.equipmentIcon;
                break;

            case REWARD_TYPE.ITEM:
                // TODO : CSV 추가 이후 아이템 추가 이후 추가 예정
                break;
            default:
                return;
        }
    }

}
