using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 실제 모든 UMS들을 제공하는 클래스입니다.
/// </summary>
public class UTCloudUMSProvider : UTUMSProvider
{
    public override void RequestLoginWithScatter()
    {
        PacketManager.Inst.RequestLoginWithScatter();
    }

    public override void RequestLogout()
    {
        PacketManager.Inst.RequestLogout();
    }

    public override void RequestGacha(int getGachaIndex)
    {
        PacketManager.Inst.RequestGacha(getGachaIndex);
    }

    public override void RequestSaveParty(UserPartyData partyInfo)
    {
        PacketManager.Inst.RequestSaveParty(partyInfo);
    }

    public override void RequestBattleAction(int getTurn)
    {
        PacketManager.Inst.RequestBattleAction(getTurn);
    }

    public override void RequestStageStart(int stageNum, int partyNum)
    {
        PacketManager.Inst.RequestStageStart(stageNum, partyNum);
    }

    public override void RequestStageExit()
    {
        PacketManager.Inst.RequestStageExit();
    }

    public override void RequestTowerStart()
    {
        throw new System.NotImplementedException();
    }

    public override void RequestTowerResult()
    {
        throw new System.NotImplementedException();
    }
}
