using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEffect : MonoBehaviour {

    private void OnEnable()
    {
        Invoke("Off", 1.0f);
    }

    private void Off()
    {
        gameObject.SetActive(false);
    }
}
