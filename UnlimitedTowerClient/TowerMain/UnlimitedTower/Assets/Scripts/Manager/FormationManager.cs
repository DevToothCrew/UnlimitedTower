using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoSingleton<FormationManager> {


    public Dictionary<int, GameObject> decDic = new Dictionary<int, GameObject>();

	// Use this for initialization
	void Awake () {
		//decDid
        for(int i=0; i<10; i++)
        {
            decDic.Add(i, gameObject.transform.GetChild(i).gameObject);
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
