using System.Collections.Generic;
using UnityEngine;
using System;

public class Calculator : MonoBehaviour {

    public static int GetMaxHp(Status status)
    {
        return ((status.basicStr) * 14) + ((status.basicDex) * 5) + ((status.basicInt) * 3);
    }

    public static int GetAttack(Status status)
    {
        return (((status.basicStr + status.basicDex) * 14) / 10);
    }
    public static int GetMagicAttack(Status status)
    {
        return ((status.basicInt * 22) / 10);
    }

    public static int GetDefence(Status status)
    {
        return ((status.basicDex * 13) / 10);
    }

    public static int GetMagicDefence(Status status)
    {
        return ((status.basicInt * 13) / 10);
    }
}
