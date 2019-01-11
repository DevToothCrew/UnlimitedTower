using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSelectAnimation : MonoBehaviour {
    private Transform tf;
    private Vector3 ScaleUnit = new Vector3(0.004f, 0.004f, 0.004f);
    private Vector3 ScaleVector;
    private Vector3 StartScale;
    private SpriteRenderer sprite;
    private bool isActive;
    private Color Yellow = new Color(1, 1, 0, 1);
    private Color Red = new Color(1, 0, 0, 1);

    void Start () {
        tf = gameObject.transform;
        ScaleVector = tf.transform.localScale;
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void AniStart()
    {
        if (!isActive)
        {
            isActive = true;
            StartCoroutine(AniScale());
            StartCoroutine(AniRotate());
        }
    }
    
    public void OnDisable()
    {
        isActive = false;
        tf.localScale = ScaleVector;
        sprite.color = Yellow;
    }

    public void Click()
    {
        sprite.color = Red;
    }

    IEnumerator AniRotate()
    {
        while (true)
        {
            tf.Rotate(0, 1, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator AniScale()
    {
        while (true)
        {
            for (int i = 0; i < 50; i++)
            {
                tf.localScale += ScaleUnit;
                yield return new WaitForSeconds(0.01f);
            }
            for (int i = 0; i < 50; i++)
            {
                tf.localScale -= ScaleUnit;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
