using System.Collections.Generic;
using UnityEngine;

public class Calculator : MonoBehaviour {

   public static int GetMonsterNum()
    {
        List<CHARACTER_NUM> myIndexList = new List<CHARACTER_NUM>();

        for (int i = (int)CHARACTER_NUM.Mst_Cat; i <= (int)CHARACTER_NUM.Mst_Snail; i++)
        {
            myIndexList.Add((CHARACTER_NUM)i);
        }

        return Random.Range(0, myIndexList.Count); 
    }

    public static int GetMaxHp(int Str)
    {
        return Str * DEFINE.MAX_HP_MULTI_posOffset * DEFINE.TEST_HP_NUM;
    }
    public static int GetAvoid(int Dex)
    {
        return Dex * DEFINE.AVOID_MULTI_posOffset;
    }
    public static int GetDamage(int Job, int Str, int Dex, int Int)
    {
        if (Job / 100 == 0)
        {
            return Str * DEFINE.DAMAGE_STR_MUlTI_posOffset;
        }
        else if (Job / 100 == 1)
        {
            return Dex * DEFINE.DAMAGE_DEX_MUlTI_posOffset;
        }
        else if (Job / 100 == 2)
        {
            return Int * DEFINE.DAMAGE_INT_MUlTI_posOffset;
        }
        else
        {
            return 0;
        }
    }
    public static int GetSpeed(int Dex)
    {
        return Dex * DEFINE.SPEED_MULTI_posOffset;
    }

    public static int GetLevelForExp(int exp)
    {
        // TODO : 추후 Servant Exp에 따른 Level 공식을 추가해 레벨 적용 필요
        return 1;
    }
}
