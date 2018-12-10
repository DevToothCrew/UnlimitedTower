using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_PacketManager : MonoSingleton<Test_PacketManager>
{
    // Use this for initialization


    public string _Packet = null;


    public void  Test_Receive(string packet)
    {
        _Packet = packet;
    }

    public string GetPakcet()
    {
        return _Packet;
    }
}
