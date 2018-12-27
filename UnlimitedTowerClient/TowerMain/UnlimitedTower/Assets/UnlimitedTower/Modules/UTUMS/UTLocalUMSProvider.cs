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

    public override void RequestGacha()
    {
        UTEventPoolInterface.SendEventData("gacha", new UTPlayerManager.UTGachaData() { });
    }

    public override void RequestSaveParty(Party partyInfo)
    {
        UTEventPoolInterface.SendEventData("login", new UTPlayerManager.UTPlayerData() { user = "devtooth", gameMoney = 999999 });
    }

    public override void RequestLogout()
    {
        PacketManager.Inst.ResponseLogout();
    }
}
