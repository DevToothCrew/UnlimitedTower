using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    public Sprite[] animationFrame;
    public Image image;
    public int index;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(Animation());
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Animation()
    {
        for (; ; )
        {
            yield return new WaitForSecondsRealtime(0.2f);
            image.sprite = animationFrame[index];
            index++;
            if (index >= animationFrame.Length)
            {
                index = 0;
            }
        }
    }
}
