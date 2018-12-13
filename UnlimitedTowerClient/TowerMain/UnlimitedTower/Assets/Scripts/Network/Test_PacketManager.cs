using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_PacketManager : MonoSingleton<Test_PacketManager>
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


    public void Test_LoginReceive(string packet)
    {
        Debug.Log("Test_LoginReceive : " + packet);
        _Packet = packet;
    }
    public void Test_ExecuteGacha(string packet)
    {
        Debug.Log("Test_ExecuteGacha : " + packet);
        _Packet = packet;
    }
    public void Test_SaveFormation(string packet)
    {
        Debug.Log("Test_ExecuteGacha : " + packet);
        _Packet = packet;
    }




}
