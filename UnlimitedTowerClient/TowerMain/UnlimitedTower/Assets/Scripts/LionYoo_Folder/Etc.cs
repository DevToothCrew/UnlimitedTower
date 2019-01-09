using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Etc : MonoBehaviour
{
    // singleton
    public static Etc instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // 힘 민 지
    public double getstr(UserServantData servant)
    {
        return servant.status.basicStr + servant.status.plusStr;
    }
    public double getstr(UserMonsterData monster)
    {
        return 0;
    }

    public double getdex(UserServantData servant)
    {
        return servant.status.basicDex + servant.status.plusDex;
    }
    public double getdex(UserMonsterData monster)
    {
        return 0;
    }

    public double getwis(UserServantData servant)
    {
        return servant.status.basicInt + servant.status.plusInt;
    }
    public double getwis(UserMonsterData monster)
    {
        return 0;
    }


    // 생명력 방어력 공격력 치확 치피 속도
    public double getatk(UserServantData servant)
    {
        SERVANT_JOB job = (SERVANT_JOB)servant.jobNum;
        int multiplier = 2;
        switch (job)
        {
            case SERVANT_JOB.WhiteHand:
                return multiplier * (servant.status.basicStr + servant.status.plusStr);
            case SERVANT_JOB.Warrior:
                return multiplier * (servant.status.basicStr + servant.status.plusStr);
            case SERVANT_JOB.Theif:
                return multiplier * (servant.status.basicDex + servant.status.plusDex);
            case SERVANT_JOB.Archer:
                return multiplier * (servant.status.basicDex + servant.status.plusDex);
            case SERVANT_JOB.Magician:
                return multiplier * (servant.status.basicInt + servant.status.plusInt);
            case SERVANT_JOB.Cleric:
                return multiplier * (servant.status.basicInt + servant.status.plusInt);
            default:
                Debug.Log("missing job");
                return 0;
        }
    }
    public double getatk(UserMonsterData monster)
    {
        // 아직 미정인상태
        return 0;
    }
    public double getHP(double str)
    {
        return str * 22d;
    }
    public double getDef(double dex)
    {
        return dex;
    }
    public double getCriticalProb(double wis)
    {
        return wis;
    }

}

// Event보유하고있는 클래스
public class EventClass
{
    event System.Action eventData;

    public void Execute_Event()
    {
        if (eventData != null)
        {
            eventData();
        }
    }

}

public enum MountitemType
{
    Weapon,
    Defense,
    Accesory
}
public enum StatType
{
    Str,
    Dex,
    Int,

    Def,
    Atk,
    CriDmg,
    CriPercent,
    Spd,
    Hp,

}

