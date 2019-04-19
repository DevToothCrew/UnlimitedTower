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
        BattleStatus battleStatus = new BattleStatus(stateData);
        Debug.Log("패시브 계산 시작 : 01");
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
                        case EQUIPMENT_OPTION_TYPE.HP:
                            battleStatus.Status[EFFECT_ID.HP] += (int)(equipmentData.value * ((equipmentData.upgrade * 0.1f) + 1));
                            break;
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
        }
        else if (stateData.charType == CHAR_TYPE.MONSTER && stateData.position < 10)
        {
            UserMonsterData monster = UserDataManager.Inst.GetMonsterInfo(stateData.index);

            if (monster == null)
            {
                Debug.Log(stateData.index + "th Monster is Null");
            }

            battleStatus.Status[EFFECT_ID.INT] = monster.status.basicInt + (int)(monster.upgrade * 0.1f);
            battleStatus.Status[EFFECT_ID.DEX] = monster.status.basicDex + (int)(monster.upgrade * 0.1f);
            battleStatus.Status[EFFECT_ID.STR] = monster.status.basicStr + (int)(monster.upgrade * 0.1f);
        }

        Debug.Log("패시브 계산 시작 : 02");
        foreach (UserSkillInfo skillInfo in stateData.passiveSkillList)
        {
            DBSkillPassiveData passive = CSVData.Inst.GetDBSkillPassiveData(skillInfo.id);
            battleStatus.Status[passive.effectID] += passive.effectAdd;
        }

        Debug.Log("패시브 계산 시작 : 03");
        return battleStatus;
    }
}
