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

    public static BattleStatus GetBattleStatus(UserCharacterStateData stateData)
    {
        UserStageStateData statgData = UserDataManager.Inst.GetStageState();
        BattleStatus battleStatus = new BattleStatus(stateData);

        if (stateData.charType == CHAR_TYPE.SERVANT)
        {
            UserServantData servant = UserDataManager.Inst.GetServantInfo(stateData.index);

            if (servant == null)
            {
                Debug.Log(stateData.index + "th Servant is Null");
            }

            foreach (KeyValuePair<EQUIPMENT_TYPE, int> state in servant.equipmentDic)
            {
                if (state.Value != 0)
                {
                    UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(state.Value);

                    if (equipmentData == null)
                    {
                        Debug.Log(state.Value + "th Equipment is Null");
                    }
                    switch (equipmentData.optionType)
                    {
                        case EQUIPMENT_OPTION_TYPE.STR:
                            battleStatus.Status[EFFECT_ID.STR] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.DEX:
                            battleStatus.Status[EFFECT_ID.DEX] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.INT:
                            battleStatus.Status[EFFECT_ID.INT] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                    }
                }
            }

            foreach (UserSkillInfo skillInfo in stateData.passiveSkillList)
            {
                DBSkillPassiveData passive = CSVData.Inst.GetDBSkillPassiveData(skillInfo.id);
                if (passive.effectID != EFFECT_ID.HP && passive.effectID != EFFECT_ID.ATK && passive.effectID != EFFECT_ID.MATK && passive.effectID != EFFECT_ID.DEF && passive.effectID != EFFECT_ID.MDEF)
                    battleStatus.Status[passive.effectID] += battleStatus.Status[passive.effectID] * passive.effectAdd / 100;
            }

            battleStatus.Status[EFFECT_ID.ATK] = GetAttack(stateData.status, servant.level);
            battleStatus.Status[EFFECT_ID.MATK] = GetMagicAttack(stateData.status, servant.level);
            battleStatus.Status[EFFECT_ID.DEF] = GetDefence(stateData.status, servant.level);
            battleStatus.Status[EFFECT_ID.MDEF] = GetMagicDefence(stateData.status, servant.level);

            foreach (KeyValuePair<EQUIPMENT_TYPE, int> state in servant.equipmentDic)
            {
                if (state.Value != 0)
                {
                    UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(state.Value);

                    if (equipmentData == null)
                    {
                        Debug.Log(state.Value + "th Equipment is Null");
                    }
                    switch (equipmentData.optionType)
                    {
                        case EQUIPMENT_OPTION_TYPE.ATK:
                            battleStatus.Status[EFFECT_ID.ATK] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.MATK:
                            battleStatus.Status[EFFECT_ID.MATK] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.DEF:
                            battleStatus.Status[EFFECT_ID.DEF] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.MDEF:
                            battleStatus.Status[EFFECT_ID.MDEF] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                    }
                }
            }

            foreach (UserSkillInfo skillInfo in stateData.passiveSkillList)
            {
                DBSkillPassiveData passive = CSVData.Inst.GetDBSkillPassiveData(skillInfo.id);
                if (passive.effectID != EFFECT_ID.HP && passive.effectID != EFFECT_ID.STR && passive.effectID != EFFECT_ID.DEX && passive.effectID != EFFECT_ID.INT)
                    battleStatus.Status[passive.effectID] += battleStatus.Status[passive.effectID] * passive.effectAdd / 100;
            }

            battleStatus.Status[EFFECT_ID.CRI_PER] = GetCriticalPercent(stateData.status, servant.level);
            battleStatus.Status[EFFECT_ID.CRI_DMG] = GetCriticalDamage(stateData.status, servant.level);
        }
        else if (stateData.charType == CHAR_TYPE.MONSTER && stateData.position < 10)
        {
            UserMonsterData monster = UserDataManager.Inst.GetMonsterInfo(stateData.index);

            if (monster == null)
            {
                Debug.Log(stateData.index + "th Monster is Null");
            }

            battleStatus.Status[EFFECT_ID.INT] += (int)(battleStatus.Status[EFFECT_ID.INT] * (stateData.upgrade * 0.1f));
            battleStatus.Status[EFFECT_ID.DEX] += (int)(battleStatus.Status[EFFECT_ID.DEX] * (stateData.upgrade * 0.1f));
            battleStatus.Status[EFFECT_ID.STR] += (int)(battleStatus.Status[EFFECT_ID.STR] * (stateData.upgrade * 0.1f));

            foreach (UserSkillInfo skillInfo in stateData.passiveSkillList)
            {
                DBSkillPassiveData passive = CSVData.Inst.GetDBSkillPassiveData(skillInfo.id);
                if (passive.effectID != EFFECT_ID.HP && passive.effectID != EFFECT_ID.ATK && passive.effectID != EFFECT_ID.MATK && passive.effectID != EFFECT_ID.DEF && passive.effectID != EFFECT_ID.MDEF)
                    battleStatus.Status[passive.effectID] += passive.effectAdd;
            }

            foreach (UserSkillInfo skillInfo in statgData.mySynergyList)
            {
                DBSkillPassiveData passive = CSVData.Inst.GetDBSkillPassiveData(skillInfo.id);
                if (passive.effectID != EFFECT_ID.HP && passive.effectID != EFFECT_ID.ATK && passive.effectID != EFFECT_ID.MATK && passive.effectID != EFFECT_ID.DEF && passive.effectID != EFFECT_ID.MDEF)
                {
                    if (passive.jobClass == stateData.tribeType + 900)
                    {
                        battleStatus.Status[passive.effectID] += (battleStatus.Status[passive.effectID] * passive.effectAdd) / 100;
                    }
                }
            }

            battleStatus.Status[EFFECT_ID.ATK] = GetAttack(stateData.status, monster.level);
            battleStatus.Status[EFFECT_ID.MATK] = GetMagicAttack(stateData.status, monster.level);
            battleStatus.Status[EFFECT_ID.DEF] = GetDefence(stateData.status, monster.level);
            battleStatus.Status[EFFECT_ID.MDEF] = GetMagicDefence(stateData.status, monster.level);

            foreach (UserSkillInfo skillInfo in stateData.passiveSkillList)
            {
                DBSkillPassiveData passive = CSVData.Inst.GetDBSkillPassiveData(skillInfo.id);
                if (passive.effectID != EFFECT_ID.HP && passive.effectID != EFFECT_ID.STR && passive.effectID != EFFECT_ID.DEX && passive.effectID != EFFECT_ID.INT)
                    battleStatus.Status[passive.effectID] += passive.effectAdd;
            }

            battleStatus.Status[EFFECT_ID.CRI_PER] = GetCriticalPercent(stateData.status, monster.level);
            battleStatus.Status[EFFECT_ID.CRI_DMG] = GetCriticalDamage(stateData.status, monster.level);

            foreach (UserSkillInfo skillInfo in statgData.mySynergyList)
            {
                DBSkillPassiveData passive = CSVData.Inst.GetDBSkillPassiveData(skillInfo.id);
                if (passive.effectID != EFFECT_ID.HP && passive.effectID != EFFECT_ID.STR && passive.effectID != EFFECT_ID.DEX && passive.effectID != EFFECT_ID.INT)
                {
                    if (passive.jobClass == stateData.tribeType + 900)
                    {
                        battleStatus.Status[passive.effectID] += (battleStatus.Status[passive.effectID] * passive.effectAdd) / 100;
                    }
                }
            }
        }
        return battleStatus;
    }
}
