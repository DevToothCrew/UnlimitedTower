using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDB
{
    // 캐릭터 세팅하는 예시. 실제로는 서버에서 값을 받아온다.
    public static Dictionary<int, Character> LoadCharactersData()
    {
        Dictionary<int, Character> getCharactrerDic = new Dictionary<int, Character>();

        // 랜덤값으로 뽑기 위한
        List<CHARACTER_NUM> myIndexList = new List<CHARACTER_NUM>();

        myIndexList.Add(CHARACTER_NUM.Hero_Novice_1001);
        myIndexList.Add(CHARACTER_NUM.Hero_Novice_1002);
        myIndexList.Add(CHARACTER_NUM.Hero_Novice_1003);
        myIndexList.Add(CHARACTER_NUM.Hero_Novice_1004);

        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            int getStr = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int getDex = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int getInt = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int myIndex = Random.Range(0, myIndexList.Count);
            CHARACTER_NUM charType = myIndexList[myIndex];
            
            getCharactrerDic.Add(i, new Character(0, 1, getStr, getDex, getInt, 0, (charType.ToString()), (int)charType));
        }

        return getCharactrerDic;
    }

    public static Dictionary<int, Character> LoadMonstersData()
    {
        Dictionary<int, Character> getMonsterDic = new Dictionary<int, Character>();

        List<CHARACTER_NUM> myIndexList = new List<CHARACTER_NUM>();
        //myIndexList.Add(CHARACTER_NUM.Mst_Anubis);
        myIndexList.Add(CHARACTER_NUM.Mst_Ninja);
        myIndexList.Add(CHARACTER_NUM.Mst_Fairy);




        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            int getStr = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int getDex = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int getInt = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int myIndex = Random.Range(0, myIndexList.Count);
            CHARACTER_NUM charType = myIndexList[myIndex];

            getMonsterDic.Add(i, new Character(0, 1, getStr, getDex, getInt, 0, (charType.ToString()), (int)charType));
        }

        return getMonsterDic;
    }
}
