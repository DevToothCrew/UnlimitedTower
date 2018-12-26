using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UTLobbyUIManager : MonoBehaviour {

    public void Login()
    {
        UTUMSProvider.Instance.RequestLoginWithScatter();
    }
}
