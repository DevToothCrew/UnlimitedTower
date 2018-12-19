using System.Collections;
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
}
