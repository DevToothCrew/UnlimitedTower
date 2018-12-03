using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharContentList : MonoBehaviour {

	// Use this for initialization
	void Awake () {
       UserDataManager.Inst.LoadCharList();
    }
	
}
