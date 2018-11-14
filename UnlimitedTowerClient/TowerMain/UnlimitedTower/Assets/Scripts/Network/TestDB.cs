using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO : if deleted
public class TestDB
{
    // 캐릭터 세팅하는 예시. 실제로는 서버에서 값을 받아온다.
    public static Dictionary<int, Character> LoadCharactersData()
    {
        Debug.Log("캐릭터 정보를 받아옴");
        Dictionary<int, Character> getCharactrerDic = new Dictionary<int, Character>();

        // 랜덤값으로 뽑기 위한
        // 랜덤으로 10개를 저장한뒤 가져온다.
        List<CHARACTER_NUM> myIndexList = new List<CHARACTER_NUM>();

        myIndexList.Add(CHARACTER_NUM.Hero_Novice_1001);
        myIndexList.Add(CHARACTER_NUM.Hero_Novice_1002);
        myIndexList.Add(CHARACTER_NUM.Hero_Novice_1003);
        myIndexList.Add(CHARACTER_NUM.Hero_Novice_1004);

        for (int i = 0; i < 10; i++)
        {
            int getStr = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int getDex = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int getInt = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int myIndex = Random.Range(0, myIndexList.Count);
            CHARACTER_NUM charType = myIndexList[myIndex];
            

            // 사이즈 구분 번호가 없다.
            getCharactrerDic.Add(i, new Character(0, 1, getStr, getDex, getInt, 0, (charType.ToString()), (int)charType
                , SIZE_TYPE.SMALL));
        }

        return getCharactrerDic;
    }

    public static Dictionary<int, Character> LoadMonstersData()
    {
        Dictionary<int, Character> getMonsterDic = new Dictionary<int, Character>();

        List<CHARACTER_NUM> myIndexList = new List<CHARACTER_NUM>();
        myIndexList.Add(CHARACTER_NUM.Mst_Anubis);
        myIndexList.Add(CHARACTER_NUM.Mst_Ninja);
        myIndexList.Add(CHARACTER_NUM.Mst_Fire_Fairy);
       // myIndexList.Add(CHARACTER_NUM.Mst_Leaf_Fairy);
        myIndexList.Add(CHARACTER_NUM.Mst_Death);
        myIndexList.Add(CHARACTER_NUM.Mst_Knight);
        myIndexList.Add(CHARACTER_NUM.Mst_WolfMan);
        myIndexList.Add(CHARACTER_NUM.Mst_Cat);
        myIndexList.Add(CHARACTER_NUM.Mst_DarkMagician);
        myIndexList.Add(CHARACTER_NUM.Mst_Demon);
        myIndexList.Add(CHARACTER_NUM.Mst_Egg);
        //myIndexList.Add(CHARACTER_NUM.Mst_Goblin_Blue);
        //myIndexList.Add(CHARACTER_NUM.Mst_Goblin_Green);



        for (int i = 0; i < 10; i++)
        {
            int getStr = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int getDex = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int getInt = Random.Range(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
            int myIndex = Random.Range(0, myIndexList.Count);
            CHARACTER_NUM charType = myIndexList[myIndex];
            SIZE_TYPE charSize;

            if(charType == CHARACTER_NUM.Mst_Knight)
            {
                charSize = SIZE_TYPE.MIDDLE;
            }
            else if(charType == CHARACTER_NUM.Mst_Death)
            {
                charSize = SIZE_TYPE.BIG;
            }
            else
            {
                charSize = SIZE_TYPE.SMALL;
            }

                getMonsterDic.Add(i, new Character(0, 1, getStr, getDex, getInt, 0, (charType.ToString()), (int)charType, charSize));
        }

        return getMonsterDic;
    }
}
