using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;

public class CSVData : MonoSingleton<CSVData> {

    public Dictionary<int, DBMonsterData> monsterDataDic = new Dictionary<int, DBMonsterData>();
    public Dictionary<int, DBLocalizationData> localDataDic = new Dictionary<int, DBLocalizationData>();
    private LOCALIZATION_TYPE localType;

    //  인스펙터에서 보여주기 위한...
    public List<DBMonsterData> monsterDataInspector = new List<DBMonsterData>();

    void Start()
    {
        //SetLocalizationData();
        SetMonsterData();
        localType = LOCALIZATION_TYPE.EN;
    }

    #region SetFunction

    public void SetLocalizationData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/LocalData");

        for (var i = 3; i < data.Count; i++)
        {
            Debug.Log("index " + (i).ToString()
                + " : " + data[i]["index"]
                + " " + data[i]["kr"]
                + " " + data[i]["ch"]
                + " " + data[i]["en"]
                );

            DBLocalizationData localData = new DBLocalizationData();
            localData.index     = Convert.ToInt32(data[i]["index"]);
            localData.krText    = Convert.ToString(data[i]["kr"]);
            localData.chText    = Convert.ToString(data[i]["ch"]);
            localData.enText    = Convert.ToString(data[i]["en"]);

            localDataDic.Add(localData.index, localData);
        }
    }

    public void SetMonsterData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/MonsterData");

        for (var i = 3; i < data.Count; i++)
        {
            //Debug.Log("index " + (i).ToString()
            //    + " : " + data[i]["id"]
            //    + " " + data[i]["name"]
            //    + " " + data[i]["resource_base"]
            //    + " " + data[i]["potrait_base"]
            //    + " " + data[i]["stat_main"]
            //    );

            DBMonsterData monsterData = new DBMonsterData();
            monsterData.indexNumber     = Convert.ToInt32(data[i]["id"]);
            monsterData.name = Convert.ToString(data[i]["enname"]);
            // TODO : 로컬 적용 후 아래로 변경
            // monsterData.name            = Convert.ToString(data[i]["name"]);
            monsterData.resource        = Convert.ToString(data[i]["resource_base"]);
            monsterData.inGameIconName  = Convert.ToString(data[i]["potrait_base"]);

            monsterDataDic.Add(monsterData.indexNumber, monsterData);
            monsterDataInspector.Add(monsterData);
        }
    }

    #endregion

    #region GetFunction

    public List<int> GetMonsterIndexList()
    {
        if (monsterDataDic == null)
        {
            Debug.LogError("MonsterDataBaseDic Error");
            return null;
        }

        return monsterDataDic.Keys.ToList();
    }

    public string GetMonsterDBResource(int index)
    {
        if (monsterDataDic.ContainsKey(index) == false)
        {
            return null;
        }

        return monsterDataDic[index].resource;
    }

    public int GetRandomMonsterIndex()
    {
        int monsterIndex = monsterDataInspector[UnityEngine.Random.Range(0, monsterDataInspector.Count)].indexNumber;

        return monsterIndex;
    }

    public string GetMonsterInGameIconName(int index)
    {
        if (monsterDataDic.ContainsKey(index) == false)
        {
            return null;
        }

        return monsterDataDic[index].inGameIconName;
    }

    public string GetLocalizationText(int index)
    {
        if(localDataDic.ContainsKey(index) == false)
        {
            return null;
        }

        switch(localType)
        {
            case LOCALIZATION_TYPE.KR:
                return localDataDic[index].krText;

            case LOCALIZATION_TYPE.CH:
                return localDataDic[index].chText;

            case LOCALIZATION_TYPE.EN:
                return localDataDic[index].enText;

            default:
                return null;
        }
    }

    public string GetMonsterName(int monsterIndex)
    {
        if(monsterDataDic.ContainsKey(monsterIndex) == false)
        {
            return null;
        }

        string monsterName = monsterDataDic[monsterIndex].name;

        return monsterName;

        // Monster Name을 통한 Local Return
        //int nameIndex = 1;
        //return GetLocalizationText(nameIndex);
    }

    #endregion
}
