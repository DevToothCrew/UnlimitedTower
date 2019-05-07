using System.Collections.Generic;
using UnityEngine;
using System;

public class Calculator : MonoBehaviour
{

    public static int GetPower(Status status, int level = 1)
    {
        int attack = (int)(((status.basicStr + status.basicDex) * 10) * (level / (float)DEFINE.MAX_LEVEL));
        int magic_attack = (int)((status.basicInt * 10.0f) * (level / (float)DEFINE.MAX_LEVEL));

        return (attack + magic_attack);
    }

    public static int GetMaxHp(Status status, int level = 1)
    {
        return (int)((status.basicStr * 66 * ((float)level / DEFINE.MAX_LEVEL)) + status.basicStr * 22);
    }

    public static int GetAttack(Status status, int level = 1)
    {
        return (int)(((status.basicStr + status.basicDex) * 8 * ((float)level / DEFINE.MAX_LEVEL)) + (status.basicStr + status.basicDex) * 2);
    }

    public static int GetMagicAttack(Status status, int level = 1)
    {
        return (int)((status.basicInt * 8 * ((float)level / DEFINE.MAX_LEVEL)) + status.basicInt * 2);
    }

    public static int GetDefence(Status status, int level = 1)
    {
        return (int)(((status.basicStr + status.basicDex) * 4 * ((float)level / DEFINE.MAX_LEVEL)) + (status.basicStr + status.basicDex) * 1);
    }

    public static int GetMagicDefence(Status status, int level = 1)
    {
        return (int)((status.basicInt * 4 * ((float)level / DEFINE.MAX_LEVEL)) + status.basicInt * 1);
    }

    public static int GetCriticalPercent(Status status, int level = 1)
    {
        return (int)((status.basicDex / 500.0f) * 100 + 5);
    }

    public static int GetCriticalDamage(Status status, int level = 1)
    {
        return (int)((status.basicInt * 0.5f) + 120);
    }
}
