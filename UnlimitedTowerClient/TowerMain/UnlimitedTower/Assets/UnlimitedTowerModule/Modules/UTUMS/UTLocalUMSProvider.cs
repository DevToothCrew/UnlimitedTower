﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    public override void RequestGacha(int getGachaIndex)
    {
        UTEventPoolInterface.SendEventData("gacha", new UTPlayerManager.UTGachaData() { gachaIndex = getGachaIndex });
    }

    public override void RequestSaveParty(UserPartyData partyInfo)
    {
        UTEventPoolInterface.SendEventData("saveparty", new UTPlayerManager.UTPartyData() { partyNum = 1});
    }

    public override void RequestBattleAction(int getTurn)
    {
        UTEventPoolInterface.SendEventData("battleaction", new UTPlayerManager.UTBattleActionData() { turn = getTurn });
    }

    public override void RequestStageStart(int getStageNum, int getPartyNum)
    {
        UTEventPoolInterface.SendEventData("stagestart", new UTPlayerManager.UTStageStartData() { stageNum = getStageNum, partyNum = getPartyNum });
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
