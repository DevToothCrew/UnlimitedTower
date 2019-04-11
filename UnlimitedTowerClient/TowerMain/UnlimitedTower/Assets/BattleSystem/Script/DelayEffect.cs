using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayEffect : MonoBehaviour
{
    public GameObject[] effect;
    public Animator characterImage;

    public void Awake()
    {
        characterImage = GameObject.Find("DelayCharacter").GetComponent<Animator>();
    }

    public void OnEnable()
    {
        StartCoroutine(EffectInstante());
        if (Time.timeScale > 0.1f)
            characterImage.SetFloat("Speed", 1.0f / Time.timeScale);
        else
            characterImage.SetFloat("Speed", 1.0f);
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
