using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonReversEvent : MonoBehaviour {
    public Sprite on;
    public Sprite child_On;
    public Sprite off;
    public Sprite child_Off;

    private Image image;
    private Image child_Image;
    private bool isOn;

    public void Awake()
    {
        image = GetComponent<Image>();
        child_Image = transform.GetChild(0).GetComponent<Image>();
    }

    public void Revers()
    {
        if (isOn)
        {
            image.sprite = off;
            child_Image.sprite = child_Off;
            isOn = false;
        }
        else
        {
            image.sprite = on;
            child_Image.sprite = child_On;
            isOn = true;
        }
    }
}
