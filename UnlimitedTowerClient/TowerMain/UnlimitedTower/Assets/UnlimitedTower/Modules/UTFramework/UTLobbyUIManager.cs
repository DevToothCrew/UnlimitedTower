using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class UTLobbyUIManager : MonoBehaviour {

    public void Login()
    {
        UTUMSProvider.Instance.RequestLoginWithScatter();
        //JsonData data = Cheat.Inst.GetUserLoginData("devtooth", 10000);
        //PacketManager.Inst.ResponseLogin(data.ToString());
    }

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

    public void BattleAction(int heroTarget, int heroAction, int monsterTarget, int monsterAction)
    {
        UTUMSProvider.Instance.RequestBattleAction(heroTarget, heroAction, monsterTarget, monsterAction);
    }

    [ContextMenu("Start")]
    public void StageStart()
    {
        int stageNum = 1;
        int partyNum = 1;
        UTUMSProvider.Instance.RequestStageStart(stageNum, partyNum);
        //JsonData data = Cheat.Inst.TestGetStageStartData("devtooth", stageNum, partyNum);
        //PacketManager.Inst.ResponseStageStart(data.ToString());

    }

    public void StageResult(int stageNum)
    {
        UTUMSProvider.Instance.RequestStageResult(stageNum);
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
