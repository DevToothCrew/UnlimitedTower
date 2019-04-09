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
        Debug.Log("Enemy Account Name : " + accountName.text);

        PacketManager.Inst.RequestPVPStart(accountName.text);
    }
}
