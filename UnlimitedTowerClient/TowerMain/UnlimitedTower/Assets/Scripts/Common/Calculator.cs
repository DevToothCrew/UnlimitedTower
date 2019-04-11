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
        return (int)((status.basicStr + status.basicDex) * 2.2f);
    }

    public static int GetMagicAttack(Status status)
    {
        return status.basicInt * 3;
    }

    public static int GetDefence(Status status)
    {
        return (int)(status.basicDex * 1.3f);
    }

    public static int GetMagicDefence(Status status)
    {
        return (int)(status.basicInt * 1.3f);
    }

    public static BattleStatus GetBattleStatus(UserCharacterStateData stateData, List<UserSkillInfo> passiveList)
    {
        BattleStatus battleStatus = new BattleStatus(stateData.maxHP, stateData.atk, stateData.mAtk, stateData.def, stateData.mDef);

        foreach (UserSkillInfo skillInfo in passiveList)
        {
            switch (skillInfo.id)
            {
                case 100001:
                    battleStatus.atk += battleStatus.atk * (int)CSVData.Inst.GetSkillPassiveData(skillInfo.id).effectPer;
                    break;
                case 100002:
                    battleStatus.mAtk += battleStatus.mAtk * (int)CSVData.Inst.GetSkillPassiveData(skillInfo.id).effectPer;
                    break;
                case 100003:
                    battleStatus.def += battleStatus.def * (int)CSVData.Inst.GetSkillPassiveData(skillInfo.id).effectPer;
                    break;
                case 100004:
                    battleStatus.mDef += battleStatus.mDef * (int)CSVData.Inst.GetSkillPassiveData(skillInfo.id).effectPer;
                    break;
                case 100005:
                    battleStatus.maxHp += battleStatus.maxHp * (int)CSVData.Inst.GetSkillPassiveData(skillInfo.id).effectPer;
                    break;
            }
        }

        return battleStatus;
    }
}
