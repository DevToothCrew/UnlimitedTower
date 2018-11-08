using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSV_CharData : MonoBehaviour {

	public int _exp = 0;

    static List<Dictionary<string, object>> data;

    void Start () {

          data = CSVReader.Read("CSV/characterData");
        for (var i = 0; i < data.Count; i++)
        {
            Debug.Log("index " + (i).ToString() + " : " + data[i]["Exp"] + " " + data[i]["Lv"] + " " + data[i]["Str"]
                        + " " + data[i]["Dex"] + " " + data[i]["Int"] + " " + data[i]["Job"] + " " + data[i]["Name"] + " " + data[i]["CharIndex"]);

        }
        _exp = (int)data[0]["Exp"];
        Debug.Log(_exp);
    }
}
