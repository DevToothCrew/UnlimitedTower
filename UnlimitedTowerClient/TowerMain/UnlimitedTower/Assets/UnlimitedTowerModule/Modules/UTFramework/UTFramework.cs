using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UTFramework : MonoBehaviour {

    static public UTFramework Instance = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
