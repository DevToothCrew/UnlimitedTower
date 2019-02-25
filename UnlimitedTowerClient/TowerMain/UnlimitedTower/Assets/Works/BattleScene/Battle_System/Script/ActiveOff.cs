using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOff : MonoBehaviour {
    public float time;

    private void OnEnable()
    {
        StartCoroutine(OFF());
    }

    IEnumerator OFF()
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
