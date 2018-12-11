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

    private void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            InGameUIFinished.SetupAndPlay(InGameUIFinished.FinishedType.Clear);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            InGameUIFinished.SetupAndPlay(InGameUIFinished.FinishedType.Failed);
        }
#endif
    }
}
