using System.Collections.Generic;
using UnityEngine;
using System;

public class Calculator : MonoBehaviour
{
    public static int GetPower(Status status, int level = 1)
    {
        int attack = GetAttack(status, level);
        int magic_attack = GetMagicAttack(status, level);

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

    //캐릭터 전체 장비(무기,갑옷,악세사리)에 존재하는 Stats의 총합을 리턴
    //해당 능력치 가져올때(리턴된 Array변수명[EQUIPMENT_OPTION_TYPE.해당능력치])
    //EX) equipmentValue[EQUIPMENT_OPTION_TYPE.STR] ->> 장비로 증가하는 STR 값
    public static int[] GetStatsOnlyAllEquipment(UserServantData servant)
    {
        int type_max = (int)EQUIPMENT_OPTION_TYPE.MAX;
        int[] statsValue = new int[type_max];
        for (int i=0; i<type_max; i++)
        {
            statsValue[i] = 0;
        }

        int equip_type_max = (int)EQUIPMENT_TYPE.MAX;
        for (int equip_type = 0; equip_type < equip_type_max; equip_type++)
        {
            UserEquipmentData equipData = UserDataManager.Inst.GetEquipmentInfo(servant.equipmentDic[(EQUIPMENT_TYPE)equip_type]);
            if (equipData != null)
            {
                statsValue[(int)equipData.optionType] += equipData.value;
            }
            else
            {
                DebugLog.Log(false, "Select servant not equip " + (EQUIPMENT_TYPE)equip_type);
            }
        }

        return statsValue;
    }

    //착용중인 장비를 포함한 Servant Stats를 리턴
    public static int[] GetServantStatsEquipAllItem(UserServantData servant)
    {
        int[] current_stats_equipment_item = Calculator.GetStatsOnlyAllEquipment(servant);    //장비에 부여된 Stats으로 올라가는 능력.

        //servant의 Status 추가
        current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.STR] += servant.status.basicStr;
        current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.DEX] += servant.status.basicDex;
        current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.INT] += servant.status.basicInt;

        Status total_status = new Status(current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.STR], current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.DEX], current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.INT]);

        //장비 Status가 반영된 Servant의 Status로 나머지 Stats 계산
        current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.HP] += Calculator.GetMaxHp(total_status, servant.level);

        current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.ATK] += Calculator.GetAttack(total_status, servant.level);
        current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.DEF] += Calculator.GetDefence(total_status, servant.level);
        current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.MATK] += Calculator.GetMagicAttack(total_status, servant.level);
        current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.MDEF] += Calculator.GetMagicDefence(total_status, servant.level);

        current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.CriDmg] += Calculator.GetCriticalDamage(total_status, servant.level);
        current_stats_equipment_item[(int)EQUIPMENT_OPTION_TYPE.CriPer] += Calculator.GetCriticalPercent(total_status, servant.level);

        return current_stats_equipment_item;
    }

}
