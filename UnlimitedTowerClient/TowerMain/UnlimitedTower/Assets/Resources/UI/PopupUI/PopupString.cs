using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupString : MonoBehaviour {

    public Text popupString;
    public int fadeTickCount;
    public float fadeTickMultiplier;

    private void OnEnable()
    {
        StartCoroutine(BecomeFade());
    }
    
    public IEnumerator BecomeFade()
    {
        for (int i = 0; i < fadeTickCount; i++)
        {
            Color color = popupString.color;
            color.a *= fadeTickMultiplier;
            popupString.color = color;
            
            yield return null;
        }

        Destroy(gameObject);
    }
    
}
