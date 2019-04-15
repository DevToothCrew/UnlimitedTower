using UnityEngine;
using System.Collections;


public class FrameChecker : MonoBehaviour
{
    GUIStyle style;
    Rect rect;
    string text;

    void Awake()
    {
        int w = Screen.width, h = Screen.height;

        rect = new Rect(0, 0, w, h * 4 / 100);

        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = Color.black;
    }
    
    void OnGUI()
    {
        if (Time.timeScale < 0.1f)
        {
            text = "Stop";
        }
        else
        text = " " + ((1 / Time.deltaTime) * Time.timeScale).ToString("#.00") + "FPS";
        GUI.Label(rect, text, style);
    }
}