using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceEffect : MonoBehaviour {
    public SpriteRenderer[] sprite = new SpriteRenderer[4];
    private Light lighting;
    
	void Awake () {
        for (int i = 0; i < 4; i++)
            sprite[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
        lighting = GetComponent<Light>();
    }
    
    public void EffectAction()
    {
        StartCoroutine(DefenceEffectSprite());
        StartCoroutine(DefenceEffectLight());
    }

    IEnumerator DefenceEffectLight()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 25; i++)
        {
            lighting.intensity += 0.4f;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 25; i++)
        {
            lighting.intensity -= 0.4f;
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator DefenceEffectSprite()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, 0.4f, 0);

        for (int i = 0; i < 20; i++)
        {
            transform.position = Vector3.Lerp(startPos, endPos, i * 0.05f);
            foreach (SpriteRenderer sp in sprite)
            {
                sp.color = new Color(1, 1, 1, 0.05f * i);
            }
            yield return new WaitForSeconds(0.02f);
        }

        for (int i = 0; i < 60; i++)
        {
            foreach (SpriteRenderer sp in sprite)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 6 * i, 0));
            }
            yield return new WaitForSeconds(0.02f);
        }

        for (int i = 0; i < 20; i++)
        {
            transform.position = Vector3.Lerp(endPos, startPos, i * 0.05f);
            foreach (SpriteRenderer sp in sprite)
            {
                sp.color = new Color(1, 1, 1, 1 - 0.05f * i);
            }
            yield return new WaitForSeconds(0.02f);
        }
        gameObject.SetActive(false);
    }
}
