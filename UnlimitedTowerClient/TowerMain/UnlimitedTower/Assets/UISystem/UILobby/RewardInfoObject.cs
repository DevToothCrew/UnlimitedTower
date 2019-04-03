using UnityEngine;
using UnityEngine.UI;

public class RewardInfoObject : MonoBehaviour {

    public Image GradeImage;
    public Image ResourceImage;

    public void SetResourceImage(REWARD_TYPE type, int id)
    {
        switch(type)
        {
            case REWARD_TYPE.SERVANT:
                break;
            case REWARD_TYPE.MONSTER:
                break;
            case REWARD_TYPE.EQUIPMENT:
                break;
            case REWARD_TYPE.ITEM:
                break;
            default:
                return;
        }
    }

}
