using UnityEngine.UI;

public class LobbyChatInfo : MonoSingleton<LobbyChatInfo> {

    public Text chatting;

    public void UpdateChatting()
    {
        if (UserDataManager.Inst.GetUserLobbyInfo().chatting == null)
        {
            chatting.text = "Welcome to the Unlimited Tower Alpha Test! The Next Generation On - Chain Battle SRPG Dapp built on EOS. Upgrade your party and become the final hero at the Unlimited Tower. ⚔️  -Alpha On Jungle Net - ";
        }
        else
        {
            chatting.text = UserDataManager.Inst.GetUserLobbyInfo().chatting;
        }
    }
}
