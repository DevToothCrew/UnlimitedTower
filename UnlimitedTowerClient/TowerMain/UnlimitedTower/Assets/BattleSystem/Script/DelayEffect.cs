using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayEffect : MonoBehaviour
{
    public GameObject[] effect;

    public void OnEnable()
    {
        StartCoroutine(EffectInstante());
    }

    IEnumerator EffectInstante()
    {
        while (true)
        {
            int radIdx = Random.Range(0, effect.Length);

            var go = Instantiate(effect[radIdx], transform);

            go.transform.position =
                transform.position +
                Vector3.right * Random.Range(-150, 150) +
                Vector3.up * Random.Range(-150, 150);

            yield return new WaitForSeconds(0.2f);
        }
    }
}
