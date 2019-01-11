using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPopup : MonoBehaviour {

    // 이미지 슬롯들
    public List<stageui_charslot> charslotList;
    // 현재 디스플레이되고있는 팀넘버
    public int CurDisplayTeam;




    private void OnEnable()
    {
        DisplayTeam(CurDisplayTeam);
    }
    public void TeambtnOnclick(int teamNum)
    {
        this.CurDisplayTeam = teamNum;
        DisplayTeam(CurDisplayTeam);
    }


    public void DisplayTeam(int teamNum)
    {

        for (int i = 0; i < charslotList.Count; i++)
        {
            int formationIndex = i;

            // 배치되어있다면, 이미지로 띄워주기
            if (GameDataManager.instance.isPlacedAt(CurDisplayTeam, formationIndex))
            {
                // 서번트
                if (formationIndex <= 4)
                {
                    UserServantData servantdata = GameDataManager.instance.getServantPlacedAt_nullPossible(CurDisplayTeam, formationIndex);
                    charslotList[i].to_servant(servantdata);
                }
                // 몬스터
                else
                {
                    UserMonsterData servantdata = GameDataManager.instance.getMonsterPlacedAt_nullPossible(CurDisplayTeam, formationIndex);
                    charslotList[i].to_monster(servantdata); 
                }
            }
            // 배치되어있지 않다면, 흰아이콘으로
            else
            {
                charslotList[i].to_empty();
            }
        }
    }
    
}
