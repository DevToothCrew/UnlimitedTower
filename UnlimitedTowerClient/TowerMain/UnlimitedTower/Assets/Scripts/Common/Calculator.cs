using System.Collections.Generic;
using UnityEngine;
using System;

public class Calculator : MonoBehaviour {

    public static int GetMaxHp(statusInfo status)
    {
        return ((status.basic_str) * 14) + ((status.basic_dex) * 5) + ((status.basic_int) * 3);
    }

    public static int GetAttack(statusInfo status)
    {
        return (((status.basic_str + status.basic_dex) * 14) / 10);
    }
    public static int GetMagicAttack(statusInfo status)
    {
        return ((status.basic_int * 22) / 10);
    }

    public static int GetDefence(statusInfo status)
    {
        return ((status.basic_dex * 13) / 10);
    }

    public static int GetMagicDefence(statusInfo status)
    {
        return ((status.basic_int * 13) / 10);
    }
}
