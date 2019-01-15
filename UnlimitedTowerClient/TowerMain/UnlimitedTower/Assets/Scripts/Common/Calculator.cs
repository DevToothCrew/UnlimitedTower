using System.Collections.Generic;
using UnityEngine;
using System;

public class Calculator : MonoBehaviour {

    public static int GetMaxHp(Status status)
    {
        return (status.basicStr + status.plusStr) * DEFINE.HP_MULTI_VALUE;
    }

    public static int GetDamage(STATUS_TYPE statusType, Status status)
    {
        if (statusType == STATUS_TYPE.STR)
        {
            return (status.basicStr + status.plusStr) * DEFINE.DAMAGE_MUlTI_VALUE;
        }
        else if (statusType == STATUS_TYPE.DEX)
        {
            return (status.basicDex + status.plusDex) * DEFINE.DAMAGE_MUlTI_VALUE;
        }
        else if (statusType == STATUS_TYPE.INT)
        {
            return (status.basicInt + status.plusInt) * DEFINE.DAMAGE_MUlTI_VALUE;
        }
        else
        {
            return 0;
        }
    }

    public static int GetDefence(Status status)
    {
        return (status.basicDex + status.plusDex) * DEFINE.DEFENCE_MULTI_VALUE;
    }

    public static int GetCriticalPer(Status status)
    {
        return (status.basicInt + status.plusInt) * DEFINE.CRITICAL_PER_MULTI_VALUE;
    }

    // 일단 박아서 쓰다가 이후 CSV 파일로 변경
    public static int GetSpeed(SERVANT_JOB job)
    {
        if(job == SERVANT_JOB.WhiteHand)
        {
            return 30;
        }
        else if(job == SERVANT_JOB.Warrior)
        {
            return 34;
        }
        else if(job == SERVANT_JOB.Theif)
        {
            return 50;
        }
        else if (job == SERVANT_JOB.Archer)
        {
            return 42;
        }
        else if (job == SERVANT_JOB.Magician)
        {
            return 29;
        }
        else if (job == SERVANT_JOB.Cleric)
        {
            return 32;
        }
        else
        {
            return 0;
        }
    }

    public static int GetLevelForExp(int exp)
    {
        // TODO : 추후 Servant Exp에 따른 Level 공식을 추가해 레벨 적용 필요
        // 일단 박아서 쓰다가 이후 CSV 파일로 변경

        int preExp = 100;
        for(int i = 0; i < DEFINE.MAX_LEVEL; i++)
        {
            if (exp < preExp)
            {
                return i + 1;
            }

            preExp = preExp + 100;
        }

        return DEFINE.MAX_LEVEL;
    }

    public static int GetAvoid(Status status)
    {
        float avoid = (status.basicDex + status.plusDex) * DEFINE.DEFENCE_MULTI_VALUE;

        if(avoid > 30)
        {
            avoid = 30;
        }

        return Convert.ToInt32(avoid);
    }
}
