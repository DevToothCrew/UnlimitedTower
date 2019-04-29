using System.Collections.Generic;
using UnityEngine;
using System;

public class Calculator : MonoBehaviour
{

    public static int GetPower(Status status, int level = 1)
    {
        int attack = (int)( ((status.basicStr + status.basicDex) * 10) * (level / (float)DEFINE.MAX_LEVEL) );
        int magic_attack = (int)((status.basicInt * 10.0f) * (level / (float)DEFINE.MAX_LEVEL));

        return (attack + magic_attack);
    }

    public static int GetMaxHp(Status status, int level = 1)
    {
        return (int)((status.basicStr * 88.0f) * (level / (float)DEFINE.MAX_LEVEL));
    }

    public static int GetAttack(Status status, int level = 1)
    {
        return (int)(((status.basicStr + status.basicDex) * 10.0f) * (level / (float)DEFINE.MAX_LEVEL));
    }

    public static int GetMagicAttack(Status status, int level = 1)
    {
        return (int)((status.basicInt * 10.0f) * (level / (float)DEFINE.MAX_LEVEL));
    }

    public static int GetDefence(Status status, int level = 1)
    {
        return (int)(((status.basicStr + status.basicDex) * 5.0f) * (level / (float)DEFINE.MAX_LEVEL));
    }

    public static int GetMagicDefence(Status status, int level = 1)
    {
        return (int)((status.basicInt * 5.0f) * (level / (float)DEFINE.MAX_LEVEL));
    }

    public static int GetCriticalPercent(Status status, int level = 1)
    {
        return (int)((status.basicInt / 500.0f) * 100 + 5);
    }

    public static int GetCriticalDamage(Status status, int level = 1)
    {
        return (int)((status.basicInt * 0.5f) + 120);
    }
}
