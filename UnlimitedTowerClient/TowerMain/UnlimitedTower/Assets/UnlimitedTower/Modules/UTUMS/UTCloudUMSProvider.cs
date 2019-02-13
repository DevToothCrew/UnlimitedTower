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

    public override void RequestGacha()
    {
        PacketManager.Inst.RequestGacha();
    }

    public override void RequestSaveParty(Party partyInfo)
    {
        PacketManager.Inst.RequestSaveParty(partyInfo);
    }

    public override void RequestBattleAction(int heroTarget, int heroAction, int monsterTarget, int monsterAction)
    {
        PacketManager.Inst.RequestBattleAction(heroTarget, heroAction, monsterTarget, monsterAction);
    }

    public override void RequestStageStart(int stageNum, int partyNum)
    {
        PacketManager.Inst.RequestStageStart(stageNum, partyNum);
    }

    public override void RequestStageResult(int stageNum)
    {
        //PacketManager.Inst.RequestStageResult(stageNum);
        PacketManager.Inst.RequestStageResult();
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
