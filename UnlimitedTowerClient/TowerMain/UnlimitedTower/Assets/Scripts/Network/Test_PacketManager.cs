using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_PacketManager : MonoSingleton<Test_PacketManager>
{
    // Use this for initialization


    public string _Packet = null;


    public void TestReceive(string packet)
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


    public void CheckPacket(string errorLog)
    {

        //if (Test_PacketManager.Inst._Packet.Length <= 0)
        //{
        //    LobbyManager.Inst.TestText.GetComponent<Text>().text = "errorLog";
        //    LobbyManager.Inst.TestText.SetActive(true);
        //}
        //else
        //{
        //    LobbyManager.Inst.TestText.GetComponent<Text>().text = Test_PacketManager.Inst._Packet;
        //    LobbyManager.Inst.TestText.SetActive(true);
        //}
        //Test_PacketManager.Inst._Packet = null;


        _Packet = "";
    }
}


