using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowEffect : MonoBehaviour {
    public ObjectPool pool;

    private void Awake()
    {
        pool = GetComponent<ObjectPool>();
    }

    private void OnEnable()
    {
        StartCoroutine(SnowStart());
    }

    IEnumerator SnowStart()
    {
        for (; ; )
        {
            yield return new WaitForSecondsRealtime(0.02f);
            pool.ObjectSpawn().transform.SetPositionAndRotation(new Vector3(Random.Range(0.0f, 14.0f) - 7, 15, Random.Range(0.0f, 14.0f) - 7), Quaternion.identity);
        }
    }
}
