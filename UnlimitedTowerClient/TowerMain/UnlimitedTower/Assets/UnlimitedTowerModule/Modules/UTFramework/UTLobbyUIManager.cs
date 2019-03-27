using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UTLobbyUIManager : MonoBehaviour {

    // Fix : Login은 LoginScene에서 관리, Manager 불필요 - 2019.02.25 TomJeong

    public void Gacha()
    {
        GachaManager.Instance.ExecuteGacha();
    }

    public void SaveParty()
    {
        // TODO : 파티 상황을 로컬에서 저장
        UserPartyData party = new UserPartyData();
        UTUMSProvider.Instance.RequestSaveParty(party);
    }

    public void Logout()
    {
        UTUMSProvider.Instance.RequestLogout();
    }

    public void BattleAction(int getTurn)
    {
        UTUMSProvider.Instance.RequestBattleAction(getTurn);
    }

    [ContextMenu("Start")]
    public void StageStart()
    {
        int stageNum = 1;
        int partyNum = 1;
        UTUMSProvider.Instance.RequestStageStart(stageNum, partyNum);
    }

    public void StageExit()
    {
        UTUMSProvider.Instance.RequestStageExit();
    }

    public void TowerStart()
    {
        UTUMSProvider.Instance.RequestTowerStart();
    }

    public void TowerResult()
    {
        UTUMSProvider.Instance.RequestTowerResult();
    }
}
