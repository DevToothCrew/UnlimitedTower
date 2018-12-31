﻿
using UnityEngine;

public abstract class UTUMSProvider
{
    static private UTUMSProvider instance = null;
    static public UTUMSProvider Instance
    {
        get
        {
            if(instance == null)
            {
#if UNITY_EDITOR
                /// 유니티 에디터 (Test용)인 경우 UTLocalUMSProvider를 사용합니다.
                instance = new UTLocalUMSProvider();
#else
                /// 유니티 애디터가 아닌 릴리즈 버전인 경우 UTCloudUMSProvider를 사용합니다.
                instance = new UTCloudUMSProvider();
#endif
            }

            return instance;
        }
    }

    /// <summary>
    /// Request login via Scatter
    /// </summary>
    public abstract void RequestLoginWithScatter();
    public abstract void RequestGacha();
    public abstract void RequestSaveParty(Party partyInfo);
    public abstract void RequestLogout();
}