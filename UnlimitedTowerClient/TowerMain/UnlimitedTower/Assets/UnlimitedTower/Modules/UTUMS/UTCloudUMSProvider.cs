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

    public override void RequestGacha()
    {
        PacketManager.Inst.RequestGacha();
    }

    public override void RequestSaveParty(Party partyInfo)
    {
        PacketManager.Inst.RequestSaveParty(partyInfo);
    }

    public override void RequestLogout()
    {
        PacketManager.Inst.RequestLogout();
    }
}
