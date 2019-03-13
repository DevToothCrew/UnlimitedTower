﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveGameObject : MonoBehaviour {
    public bool isRandom;
    public float Speed;
    public float DestroyTime;
    public Vector3 rotation;

    private bool isStopped = false;

	void Start () {
		if (isRandom)
        {
            transform.Rotate(0, 0, Random.Range(0, 360));
            Speed *= Random.Range(1.0f, 1.5f);
        }
        StartCoroutine(Move());
        Destroy(gameObject, DestroyTime);
	}
	
	void FixedUpdate () {
        transform.Translate(Speed, 0, 0);
	}

    public void End()
    {
        isStopped = true;
    }

    IEnumerator Move()
    {
        Image image = GetComponent<Image>();

        for (int i = 0; i < 10; i++)
        {
            image.color += new Color(0, 0, 0, 0.1f);
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitUntil(() => {
            DestroyTime -= Time.deltaTime;
            return DestroyTime - 0.6f < 0.0f || isStopped;
            });

        for (int i = 0; i < 10; i++)
        {
            image.color -= new Color(0, 0, 0, 0.1f);
            yield return new WaitForSeconds(0.02f);
        }
    }
}
