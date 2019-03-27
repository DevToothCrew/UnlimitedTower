using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEffect : MonoBehaviour {
    public GameObject[] cloud = new GameObject[3];

    private void OnEnable()
    {
        StartCoroutine(CloudSpawner());
    }

    IEnumerator CloudSpawner()
    {
        for(; ; )
        {
            if (Random.Range(0, 2) == 0)
                Instantiate(cloud[Random.Range(0, cloud.Length)], new Vector3(12, 5, Random.Range(-10.0f, -15.0f)), Quaternion.identity).transform.SetParent(transform);
            else
                Instantiate(cloud[Random.Range(0, cloud.Length)], new Vector3(12, 5, Random.Range(3.0f, 7.0f)), Quaternion.identity).transform.SetParent(transform);
            yield return new WaitForSeconds(Random.Range(3,6));
        }
    }
}
