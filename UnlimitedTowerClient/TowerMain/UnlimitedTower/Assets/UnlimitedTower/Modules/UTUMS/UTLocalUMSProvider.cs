using UnityEngine;


/// <summary>
/// Debug를 위해 사용되는 로컬 UMS 제공자입니다.
/// </summary>
public class UTLocalUMSProvider : UTUMSProvider
{
    public override void RequestLoginWithScatter()
    {
        UTEventPoolInterface.SendEventData("login", new UTPlayerManager.UTPlayerData() { userId = "dev", email = "dev@devtooth.com", scatterId = "scatterID" });
    }
}
