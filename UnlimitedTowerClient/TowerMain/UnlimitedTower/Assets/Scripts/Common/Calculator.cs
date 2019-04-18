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

    public static BattleStatus GetBattleStatus(UserCharacterStateData stateData)
    {
        BattleStatus battleStatus = new BattleStatus(stateData.maxHP, stateData.atk, stateData.mAtk, stateData.def, stateData.mDef, stateData.status);

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
                        case EQUIPMENT_OPTION_TYPE.ATK:
                            battleStatus.atk += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.MATK:
                            battleStatus.mAtk += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.DEF:
                            battleStatus.def += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.MDEF:
                            battleStatus.mDef += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.HP:
                            battleStatus.maxHp += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.STR:
                            battleStatus.str_ += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.DEX:
                            battleStatus.dex_ += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                        case EQUIPMENT_OPTION_TYPE.INT:
                            battleStatus.int_ += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
                    }
                }
            }
        }
        else if (stateData.charType == CHAR_TYPE.MONSTER && stateData.position < 10)
        {
            UserMonsterData monster = UserDataManager.Inst.GetMonsterInfo(stateData.index);

            if (monster == null)
            {
                Debug.Log(stateData.index + "th Monster is Null");
            }

            battleStatus.int_ = monster.status.basicInt + (int)(monster.upgrade * 0.1f);
            battleStatus.dex_ = monster.status.basicDex + (int)(monster.upgrade * 0.1f);
            battleStatus.str_ = monster.status.basicStr + (int)(monster.upgrade * 0.1f);
        }

        foreach (UserSkillInfo skillInfo in stateData.passiveSkillList)
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
                case 100006:
                    battleStatus.str_ += battleStatus.str_ * (int)CSVData.Inst.GetSkillPassiveData(skillInfo.id).effectPer;
                    break;
                case 100007:
                    battleStatus.dex_ += battleStatus.dex_ * (int)CSVData.Inst.GetSkillPassiveData(skillInfo.id).effectPer;
                    break;
                case 100008:
                    battleStatus.int_ += battleStatus.int_ * (int)CSVData.Inst.GetSkillPassiveData(skillInfo.id).effectPer;
                    break;
            }
        }

        return battleStatus;
    }
}
