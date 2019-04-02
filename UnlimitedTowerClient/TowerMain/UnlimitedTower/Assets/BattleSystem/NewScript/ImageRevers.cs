using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageRevers : MonoBehaviour
{
    public Sprite on;
    public Sprite off;
    private Image image;
    private bool isOn;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Revers()
    {
        if (isOn)
        {
            isOn = false;
            image.sprite = off;
        }
        else
        {
            isOn = true;
            image.sprite = on;
        }
    }
}
