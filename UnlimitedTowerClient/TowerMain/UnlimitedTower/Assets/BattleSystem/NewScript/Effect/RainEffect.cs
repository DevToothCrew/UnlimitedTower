using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainEffect : MonoBehaviour {
    public GameObject Rain;
    public ObjectPool pool;

    private void Awake()
    {
        pool = GetComponent<ObjectPool>();
    }

    private void OnEnable()
    {
        StartCoroutine(RainStart());
    }

    IEnumerator RainStart()
    {
        // 추후 필요시 오브젝트 풀링 기법 사용
        for (; ; )
        {
            yield return new WaitForSeconds(0.2f);
            pool.ObjectSpawn().transform.SetPositionAndRotation(new Vector3(Random.Range(0.0f, 16.0f) - 8, 15, Random.Range(0.0f, 16.0f) - 8), Quaternion.Euler(new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180))));
        }
    }
}
