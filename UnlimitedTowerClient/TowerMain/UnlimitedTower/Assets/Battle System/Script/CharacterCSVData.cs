using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CharacterCSVData : MonoBehaviour
{
    public Dictionary<int, DBCharacterData> monsterDataBaseDic = new Dictionary<int, DBCharacterData>();
    private TextAsset monsterData;
    private TextAsset servantData;
    
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

        monsterData = Resources.Load("CSV/DB_monster") as TextAsset;
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
            monsterDataBaseDic.Add(int.Parse(textValue[0]), new DBCharacterData(textValue));
            textLine = sr.ReadLine();
        }
    }
}
