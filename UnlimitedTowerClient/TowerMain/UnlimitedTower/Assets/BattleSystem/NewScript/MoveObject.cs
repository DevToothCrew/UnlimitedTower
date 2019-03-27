using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {
    public Vector3 Speed;
    public bool isRealTime;
    public float destroyTime;

    private void OnEnable()
    {
        StartCoroutine(Move());
        Destroy(gameObject, destroyTime);
    }

    IEnumerator Move()
    {
        for (; ; )
        {
            transform.Translate(Speed);
            if (isRealTime)
                yield return new WaitForSecondsRealtime(0.01f);
            else
                yield return new WaitForSeconds(0.01f);
        }
    }
}
