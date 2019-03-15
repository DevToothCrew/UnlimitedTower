using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayEffect : MonoBehaviour {
    public GameObject[] effect = new GameObject[11];

    private void OnEnable()
    {
        StartCoroutine(EffectInstante());
    }

    IEnumerator EffectInstante()
    {
        for (; ; )
        {
            Instantiate(effect[Random.Range(0, effect.Length)], transform).transform.position = transform.position + new Vector3(Random.Range(-150, 150), Random.Range(-150, 150), 0);
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }
}
