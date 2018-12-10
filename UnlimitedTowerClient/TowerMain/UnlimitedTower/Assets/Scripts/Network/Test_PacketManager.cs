using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_PacketManager : MonoBehaviour
{
    // Use this for initialization


    public string _Packet = null;


    public void  TestReceive(string packet)
    {
        Debug.Log("Recive : packet : " + packet);
        _Packet = packet;
    }

    public string GetPakcet()
    {
        return _Packet;
    }
}
