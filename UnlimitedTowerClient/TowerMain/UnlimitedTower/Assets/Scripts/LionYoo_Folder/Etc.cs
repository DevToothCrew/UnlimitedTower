using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Etc : MonoBehaviour
{
    public const int MainServantFormationNum = 2;

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
    public double Getstr(UserServantData servant)
    {
        return servant.status.basicStr + servant.status.plusStr;
    }
    public double Getstr(UserMonsterData monster)
    {
        return 0;
    }

    public double Getdex(UserServantData servant)
    {
        return servant.status.basicDex + servant.status.plusDex;
    }
    public double Getdex(UserMonsterData monster)
    {
        return 0;
    }

    public double Getint(UserServantData servant)
    {
        return servant.status.basicInt + servant.status.plusInt;
    }
    public double Getint(UserMonsterData monster)
    {
        return 0;
    }


    // 공격력 방어력 생명력 치확 치피 속도
    public double Getatk(UserServantData servant)
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
    public double Getatk(UserMonsterData monster)
    {
        // 아직 미정인상태
        return 0;
    }

    public double GetDef(UserServantData servant)
    {
        return servant.status.basicDex;
    }
    public double GetDef(UserMonsterData monster)
    {
        return monster.status.basicDex;
    }

    public double GetHP(UserServantData servant)
    {
        return Getstr(servant) * 22d;
    }
    public double GetHP(UserMonsterData monster)
    {
        return Getstr(monster) * 22d;
    }

    public double GetCriticalProb(UserServantData servant)
    {
        return 20;
    }
    public double GetCriticalProb(UserMonsterData servant)
    {
        return 20;
    }

    public double GetCriDmg(UserServantData servant)
    {
        return 10;
    }
    public double GetCriDmg(UserMonsterData servant)
    {
        return 10;
    }

    public double GetSpd(UserServantData servant)
    {
        return 30;
    }
    public double GetSpd(UserMonsterData servant)
    {
        return 30;
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
public enum PlayerType
{
    servant,
    monster
}
