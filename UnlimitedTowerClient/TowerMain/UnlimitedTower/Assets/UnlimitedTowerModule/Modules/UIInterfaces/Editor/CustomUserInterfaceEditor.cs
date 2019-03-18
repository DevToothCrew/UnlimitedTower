using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CustomUserInterfaceEditor
{
    [MenuItem("GameObject/UI/Button UI Extension")]
    static void CreateButtonUIExtension()
    {
        Transform parent = Selection.gameObjects?[0]?.transform;

        //creation button gameObject
        GameObject button = new GameObject("UT UI Button");
        RectTransform rectTransform = button.AddComponent<RectTransform>();
        button.AddComponent<CanvasRenderer>();
        ButtonUIExtension buttonExt = button.AddComponent<ButtonUIExtension>();

        //creation button gameObject
        GameObject imageGo = new GameObject("Image");
        Image image = imageGo.AddComponent<Image>();
        image.transform.SetParent(button.transform, false);
        buttonExt.targetGraphic = image;

        button.transform.parent = parent;
        rectTransform.anchoredPosition = Vector2.zero;
    }

}
