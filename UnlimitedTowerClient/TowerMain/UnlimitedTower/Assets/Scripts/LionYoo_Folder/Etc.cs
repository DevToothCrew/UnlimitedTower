using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Etc : MonoBehaviour
{
    public const int MainServantFormationNum = 2;
    public const int maxItemUpgradeNum = 9;
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

    public StatInfo getTotalstat(List<MountItemStatEntity.Param> list)
    {
        StatInfo newstat = new StatInfo();

        for (int i = 0; i < list.Count; i++)
        {
            switch (list[i].statType)
            {
                case StatType.Str:
                    newstat.str += list[i].plusValue;
                    break;
                case StatType.Dex:
                    newstat.dex += list[i].plusValue;
                    break;
                case StatType.Int:
                    newstat.INT += list[i].plusValue;
                    break;
                case StatType.Def:
                    newstat.def += list[i].plusValue;
                    break;
                case StatType.Atk:
                    newstat.atk += list[i].plusValue;
                    break;
                case StatType.CriDmg:
                    newstat.cridmg += list[i].plusValue;
                    break;
                case StatType.CriPercent:
                    newstat.criprob += list[i].plusValue;
                    break;
                case StatType.Spd:
                    newstat.spd += list[i].plusValue;
                    break;
                case StatType.Hp:
                    newstat.hp += list[i].plusValue;
                    break;
                default:
                    break;
            }
        }

        return newstat;
    }

    
    public void SortTotal(UNIT_TYPE playertype, sortType sorttype)
    {
        switch (playertype)
        {
            case UNIT_TYPE.SERVANT:
                {
                    List<UserServantData> servantList = UserDataManager.Inst.GetServantList();
                    if (servantList.Count > 0)
                    {

                        switch (sorttype)
                        {
                            case sortType.Grade:
                                {
                                    servantList.Sort((a, b) =>
                                    {
                                        double aValue = a.isLegend ? 10 : 1;
                                        double bValue = b.isLegend ? 10 : 1;
                                        if (aValue < bValue)
                                        {
                                            return 1;
                                        }
                                        else if (aValue == bValue)
                                        {
                                            return 0;
                                        }
                                        else
                                        {
                                            return -1;
                                        }

                                    });
                                }
                                break;
                            case sortType.Level:
                                {
                                    servantList.Sort((a, b) =>
                                    {
                                        if (a.exp < b.exp)
                                        {
                                            return 1;
                                        }
                                        else if (a.exp == b.exp)
                                        {
                                            return 0;
                                        }
                                        else
                                        {
                                            return -1;
                                        }

                                    });
                                }
                                break;
                            case sortType.Power:
                                {
                                    servantList.Sort((a, b) =>
                                    {
                                        double aPower = Etc.instance.Getatk(a);
                                        double bPower = Etc.instance.Getatk(b);
                                        if (aPower < bPower)
                                        {
                                            return 1;
                                        }
                                        else if (aPower == bPower)
                                        {
                                            return 0;
                                        }
                                        else
                                        {
                                            return -1;
                                        }

                                    });
                                }
                                break;
                            case sortType.Obtain:
                                {
                                    servantList.Sort((a, b) =>
                                    {
                                        double aValue = a.index;
                                        double bValue = b.index;
                                        if (aValue < bValue)
                                        {
                                            return 1;
                                        }
                                        else if (aValue == bValue)
                                        {
                                            return 0;
                                        }
                                        else
                                        {
                                            return -1;
                                        }

                                    });
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                break;
            case UNIT_TYPE.MONSTER:
                {
                    List<UserMonsterData> monsterList = UserDataManager.Inst.GetMonsterList();
                    if (monsterList.Count > 0)
                    {
                        switch (sorttype)
                        {
                            case sortType.Grade:
                                {
                                    monsterList.Sort((a, b) =>
                                    {
                                        double aValue = a.gradeNum;
                                        double bValue = b.gradeNum;
                                        if (aValue < bValue)
                                        {
                                            return 1;
                                        }
                                        else if (aValue == bValue)
                                        {
                                            return 0;
                                        }
                                        else
                                        {
                                            return -1;
                                        }

                                    });
                                }
                                break;
                            case sortType.Level:
                                {
                                    monsterList.Sort((a, b) =>
                                    {
                                        double aValue = a.level;
                                        double bValue = b.level;
                                        if (aValue < bValue)
                                        {
                                            return 1;
                                        }
                                        else if (aValue == bValue)
                                        {
                                            return 0;
                                        }
                                        else
                                        {
                                            return -1;
                                        }

                                    });
                                }
                                break;
                            case sortType.Power:
                                {
                                    monsterList.Sort((a, b) =>
                                    {
                                        double aPower = Etc.instance.Getatk(a);
                                        double bPower = Etc.instance.Getatk(b);
                                        if (aPower < bPower)
                                        {
                                            return 1;
                                        }
                                        else if (aPower == bPower)
                                        {
                                            return 0;
                                        }
                                        else
                                        {
                                            return -1;
                                        }

                                    });
                                }
                                break;
                            case sortType.Obtain:
                                {
                                    monsterList.Sort((a, b) =>
                                    {
                                        double aValue = a.index;
                                        double bValue = b.index;
                                        if (aValue < bValue)
                                        {
                                            return 1;
                                        }
                                        else if (aValue == bValue)
                                        {
                                            return 0;
                                        }
                                        else
                                        {
                                            return -1;
                                        }

                                    });
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                break;
        }
    }
    public void SortByType(List<UserServantData> servantList, sortType sortType)
    {
        switch (sortType)
        {
            case sortType.Grade:
                {
                    servantList.Sort((a, b) =>
                    {
                        double aValue = a.isLegend ? 10 : 1;
                        double bValue = b.isLegend ? 10 : 1;
                        if (aValue < bValue)
                        {
                            return 1;
                        }
                        else if (aValue == bValue)
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }

                    });
                }
                break;
            case sortType.Level:
                {
                    servantList.Sort((a, b) =>
                    {
                        if (a.exp < b.exp)
                        {
                            return 1;
                        }
                        else if (a.exp == b.exp)
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }

                    });
                }
                break;
            case sortType.Power:
                {
                    servantList.Sort((a, b) =>
                    {
                        double aPower = Etc.instance.Getatk(a);
                        double bPower = Etc.instance.Getatk(b);
                        if (aPower < bPower)
                        {
                            return 1;
                        }
                        else if (aPower == bPower)
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }

                    });
                }
                break;
            case sortType.Obtain:
                {
                    servantList.Sort((a, b) =>
                    {
                        double aValue = a.index;
                        double bValue = b.index;
                        if (aValue < bValue)
                        {
                            return 1;
                        }
                        else if (aValue == bValue)
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }

                    });
                }
                break;
            default:
                break;
        }
    }
    public void SortByType(List<UserMonsterData> monsterList, sortType sortType)
    {

        switch (sortType)
        {
            case sortType.Grade:
                {
                    monsterList.Sort((a, b) =>
                    {
                        double aValue = a.gradeNum;
                        double bValue = b.gradeNum;
                        if (aValue < bValue)
                        {
                            return 1;
                        }
                        else if (aValue == bValue)
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }

                    });
                }
                break;
            case sortType.Level:
                {
                    monsterList.Sort((a, b) =>
                    {
                        double aValue = a.level;
                        double bValue = b.level;
                        if (aValue < bValue)
                        {
                            return 1;
                        }
                        else if (aValue == bValue)
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }

                    });
                }
                break;
            case sortType.Power:
                {
                    monsterList.Sort((a, b) =>
                    {
                        double aPower = Etc.instance.Getatk(a);
                        double bPower = Etc.instance.Getatk(b);
                        if (aPower < bPower)
                        {
                            return 1;
                        }
                        else if (aPower == bPower)
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }

                    });
                }
                break;
            case sortType.Obtain:
                {
                    monsterList.Sort((a, b) =>
                    {
                        double aValue = a.index;
                        double bValue = b.index;
                        if (aValue < bValue)
                        {
                            return 1;
                        }
                        else if (aValue == bValue)
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }

                    });
                }
                break;
            default:
                break;
        }

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
    Accessory
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

// 토탈스탯
public class StatInfo
{
    public double str;
    public double dex;
    public double INT;

    public double atk;
    public double def;
    public double spd;
    public double hp;
    public double criprob;
    public double cridmg;

}