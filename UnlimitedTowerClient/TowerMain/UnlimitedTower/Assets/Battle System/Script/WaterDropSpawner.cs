using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropSpawner : MonoBehaviour {
    public GameObject water;
    private float PosX;
    private float PosZ;
    public GameObject temp;
    
    private void OnEnable()
    {
        StartCoroutine(WaterSpawnStart());
    }

    IEnumerator WaterSpawnStart()
    {
        for (; ; )
        {
            PosX = Random.Range(0.0f, 14.0f) - 7;
            PosZ = Random.Range(0.0f, 14.0f) - 7;
            temp = Instantiate(water,
                transform.position + new Vector3(transform.position.x + PosX, 0, transform.position.z + PosZ),
                Quaternion.Euler(new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180))));
            temp.transform.SetParent(transform);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        }
    }
}
