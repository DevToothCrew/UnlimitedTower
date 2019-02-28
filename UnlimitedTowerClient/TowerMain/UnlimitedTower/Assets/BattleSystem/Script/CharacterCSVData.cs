using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public class CharacterCSVData : MonoSingleton<CharacterCSVData>
{
    public Dictionary<int, DBCharacterData> monsterDataDic = new Dictionary<int, DBCharacterData>();

    //  인스펙터에서 보여주기 위한...
    public List<DBCharacterData> monsterDataInspector = new List<DBCharacterData>();
    private TextAsset monsterData;
    private TextAsset servantData;
    
    [System.Serializable]
    public class DBCharacterData
    {
        public int indexNumber;
        public string animationGroup;
        public string korName;
        public string engName;
        public int bossType;
        public string tribe;
        public string attribute;
        public string codeName;
        public string size;
        public string resource;
        public string basicAnimation;
        public string inGameIconName;
        public string skill_1;
        public string skill_2;
        public string skill_3;

        public DBCharacterData(string[] textValue)
        {
            if (textValue[0] != string.Empty)
            {
                indexNumber = int.Parse(textValue[0]);
                animationGroup = textValue[1];
                korName = textValue[2];
                engName = textValue[3];
                if (textValue[4] != string.Empty)
                    bossType = int.Parse(textValue[4]);
                tribe = textValue[5];
                attribute = textValue[6];
                codeName = textValue[7];
                size = textValue[8];
                resource = textValue[9];
                basicAnimation = textValue[10];
                inGameIconName = textValue[11];
                skill_1 = textValue[12];
                skill_2 = textValue[13];
                skill_3 = textValue[14];
            }
        }
    }
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        monsterData = Resources.Load("CSV/MonsterData") as TextAsset;
        StringReader sr = new StringReader(monsterData.text);
        string textLine;
        string[] textValue;

        textLine = sr.ReadLine();
        textLine = sr.ReadLine();
        textLine = sr.ReadLine();
        textLine = sr.ReadLine();
        textLine = sr.ReadLine();

        while (textLine != null)
        {
            textValue = textLine.Split(',');
            if (textValue[0] != string.Empty)
            monsterDataDic.Add(int.Parse(textValue[0]), new DBCharacterData(textValue));
            textLine = sr.ReadLine();
        }

        //  인스펙터에서 보여주기 위한...
        foreach (DBCharacterData asdf in monsterDataDic.Values)
        {
            monsterDataInspector.Add(asdf);
        }
    }

    [ContextMenu("asdf")]
    public void asdf()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public List<int> GetMonsterIndexList()
    {
        if(monsterDataDic == null)
        {
            Debug.LogError("MonsterDataBaseDic Error");
            return null;
        }

        return monsterDataDic.Keys.ToList();
    }

    public string GetMonsterDBResource(int index)
    {
        if(monsterDataDic.ContainsKey(index) == false)
        {
            return null;
        }

        return monsterDataDic[index].resource;
    }
}
