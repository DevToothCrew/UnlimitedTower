using UnityEngine.UI;
using UnityEngine;

public class PVPInfoPage : MonoBehaviour {

    public Text accountName;

    public void OnClickExitButton()
    {
        LobbyManager.Inst.OnClickBackButton();
    }

    public void OnClickStartPVP()
    {
        //DebugLog.Log(false, "Enemy Account Name : " + accountName.text);

        PacketManager.Inst.RequestTowerStart(1);
        //PacketManager.Inst.RequestPVPStart(accountName.text);
    }
}
