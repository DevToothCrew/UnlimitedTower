using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIRoot : MonoBehaviour {

    public GameObject debugButtonWrap;

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
        debugButtonWrap.SetActive(true);
#endif
    }
}
