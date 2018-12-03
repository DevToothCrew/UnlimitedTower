using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharContentList : MonoSingleton<CharContentList>
{

	// Use this for initialization
	void Awake () {
       UserDataManager.Inst.LoadCharList();
    }
    void LoadCharContentList()
    {

    }

	
}
