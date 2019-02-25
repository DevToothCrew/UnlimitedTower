using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurUniformController : MonoBehaviour
{
    public float uniformBlurRange;

    public float uniformDarkness;

    private Material material;

    private void OnEnable()
    {
        var image = GetComponentInChildren<UnityEngine.UI.Image>();
        material = material ?? image.material;

        Debug.Log("<color=#FFFF00> Material : " + material + "</color>");
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("_BlurRange", uniformBlurRange);
        material.SetFloat("_Darkness", uniformDarkness);
    }
}
