using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UTLobbyUIManager : MonoBehaviour {

    public void Login()
    {
        UTUMSProvider.Instance.RequestLoginWithScatter();
    }

    public void Gacha()
    {
        GachaImage.Inst.OnClickGachaOKButton();
    }

    public void SaveParty()
    {
        // TODO : 파티 상황을 로컬에서 저장
        Party party = new Party();
        UTUMSProvider.Instance.RequestSaveParty(party);
    }

    public void Logout()
    {
        UTUMSProvider.Instance.RequestLogout();
    }
}
