using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginRewardUnit : MonoBehaviour {
    public Image imageFrame;
    public Image imageDay;
    public Text textDay;
    public Image imageReward;
    public Text textReward;
    public Image imageStamp;
    public GameObject objSpecialReward;

    public void updateUnit(int index)
    {

        //csv data 완성 전까지 리턴 처리 

        if ((index+1)%7 == 0)
        {
            objSpecialReward.SetActive(true);
        }
        else
        {
            objSpecialReward.SetActive(false);
        }

        textDay.text = string.Format("DAY {0}", index + 1);

        /*
        DBLoginRewardData reward_data = CSVData.Inst.GetLoginRewardData(index);
        imageReward.sprite = reward_data.imageReward;
        textReward.text = string.Format("{0}", reward_data.rewardCount);
        */

        if (index < UserDataManager.Inst.dayCount)
        {
            //imageFrame.sprite = 
            //imageDay.sprite =
            imageStamp.enabled = true;
        }
        else
        {
            //imageFrame.sprite = 
            //imageDay.sprite =
            imageStamp.enabled = false;
        }

    }

    //출석체크 요청 날리기 (Index날릴 필요 없을 듯 : 1씩 증가)
    public void OnClickReward()
    {
        PacketManager.Inst.RequestDailyCheck();
    }
}
