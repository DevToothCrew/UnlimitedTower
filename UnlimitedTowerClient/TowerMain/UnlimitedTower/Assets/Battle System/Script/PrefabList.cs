using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabList : MonoBehaviour {
    public CharacterList[] prefabList = new CharacterList[400];

    [System.Serializable]
    public class CharacterList
    {
        public string name;
        public int Index;
        public GameObject Prefab;
    }

    private void Start()
    {
        for (int i = 0; i < prefabList.Length; i++)
        {
            prefabList[i].Index = i;
        }
    }
}
