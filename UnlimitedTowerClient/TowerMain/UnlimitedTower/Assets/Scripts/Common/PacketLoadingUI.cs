using UnityEngine;

public class PacketLoadingUI : MonoSingleton<PacketLoadingUI>
{
    public GameObject packetLoadingUI;

    void Start () {
        packetLoadingUI.SetActive(false);
    }

    public void SetLoading(bool isTrue)
    {
        packetLoadingUI.SetActive(isTrue);
    }
}
