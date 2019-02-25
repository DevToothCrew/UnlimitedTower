using UnityEngine;


/// <summary>
/// Debug를 위해 사용되는 로컬 UMS 제공자입니다.
/// </summary>
public class UTLocalUMSProvider : UTUMSProvider
{
    public override void RequestLoginWithScatter()
    {
        UTEventPoolInterface.SendEventData("login", new UTPlayerManager.UTPlayerData() { user = "devtooth", gameMoney = 999999 });
    }

    public override void RequestLogout()
    {
        PacketManager.Inst.ResponseLogout();
    }

    public override void RequestGacha()
    {
        UTEventPoolInterface.SendEventData("gacha", new UTPlayerManager.UTGachaData() { });
    }

    public override void RequestSaveParty(UserPartyData partyInfo)
    {
        UTEventPoolInterface.SendEventData("saveparty", new UTPlayerManager.UTPartyData() { partyNum = 1});
    }

    public override void RequestBattleAction(int heroTarget, int heroAction, int monsterTarget, int monsterAction)
    {
        PacketManager.Inst.RequestBattleAction(heroTarget, heroAction, monsterTarget, monsterAction);
    }

    public override void RequestStageStart(int getStageNum, int getPartyNum)
    {
        UTEventPoolInterface.SendEventData("stagestart", new UTPlayerManager.UTStageStartData() { stageNum = getStageNum, partyNum = getPartyNum });
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
