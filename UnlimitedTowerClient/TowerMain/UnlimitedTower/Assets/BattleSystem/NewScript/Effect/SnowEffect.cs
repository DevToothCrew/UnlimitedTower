﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowEffect : MonoBehaviour {
    public GameObject Snow;

    private void OnEnable()
    {
        StartCoroutine(SnowStart());
    }

    IEnumerator SnowStart()
    {
        // 추후 필요시 오브젝트 풀링 기법 사용
        for (; ; )
        {
            Instantiate(Snow, transform.position + new Vector3(Random.Range(0.0f, 16.0f) - 8, transform.position.y, Random.Range(0.0f,16.0f) - 8) , transform.rotation, transform);
            yield return new WaitForSeconds(0.02f);
        }
    }
}
