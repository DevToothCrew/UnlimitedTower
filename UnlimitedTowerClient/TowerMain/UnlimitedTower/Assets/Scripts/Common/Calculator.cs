using System.Collections.Generic;
using UnityEngine;
using System;

public class Calculator : MonoBehaviour {

    public static int GetMaxHp(totalStatus status)
    {
        return ((status.total_str) * 14) + ((status.total_dex) * 5) + ((status.total_int) * 3);
    }

    public static int GetAttack(totalStatus status)
    {
        return (((status.total_str + status.total_dex) * 14) / 10);
    }
    public static int GetMagicAttack(totalStatus status)
    {
        return ((status.total_int * 22) / 10);
    }

    public static int GetDefence(totalStatus status)
    {
        return ((status.total_dex * 13) / 10);
    }

    public static int GetMagicDefence(totalStatus status)
    {
        return ((status.total_int * 13) / 10);
    }
}
