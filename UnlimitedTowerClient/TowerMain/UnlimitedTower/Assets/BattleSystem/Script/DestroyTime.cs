using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTime : MonoBehaviour {
    public float time;
    public bool isFade;
    public bool isActive;
    public float FadeTime;

    private void OnEnable()
    {
        StartCoroutine(_Destroy());
    }

    IEnumerator _Destroy()
    {
        yield return new WaitForSeconds(time);

        if (isFade)
        {
            Material material = GetComponent<Renderer>().material;

            for (int i = 0; i < FadeTime * 100; i++)
            {
                material.color -= new Color(0, 0, 0, 1 / (FadeTime * 100));
                yield return new WaitForSeconds(0.01f);
            }
        }
        if (isActive)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
