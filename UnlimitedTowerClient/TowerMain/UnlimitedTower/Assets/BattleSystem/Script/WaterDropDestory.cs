using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropDestory : MonoBehaviour {
    public Renderer renderering;

    private void OnEnable()
    {
        renderering = GetComponent<Renderer>();
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2.0f);
        for (int i = 0; i < 35; i++)
        {
            renderering.material.color -= new Color(0, 0, 0, 0.02f);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }
}
