using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainEffect : MonoBehaviour {
    public GameObject Rain;

    private void OnEnable()
    {
        StartCoroutine(RainStart());
    }

    IEnumerator RainStart()
    {
        // 추후 필요시 오브젝트 풀링 기법 사용
        for (; ; )
        {
            Instantiate(Rain, transform.position + new Vector3(Random.Range(0.0f, 16.0f) - 8, 0, Random.Range(0.0f,16.0f) - 8) , Quaternion.Euler(new Vector3(Random.Range(0,180), Random.Range(0, 180), Random.Range(0, 180))), transform);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
